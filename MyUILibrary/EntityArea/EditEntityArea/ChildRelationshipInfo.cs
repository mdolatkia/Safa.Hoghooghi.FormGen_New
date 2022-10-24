using CommonDefinitions.UISettings;
using ModelEntites;
using MyRelationshipDataManager;
using MyUILibrary.EntityArea.Commands;
using MyUILibrary.Temp;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{
    public class ChildRelationshipInfo : ChildRelationshipData
    {
        public RelationshipColumnControlGeneral RelationshipControl { set; get; }

        public List<PropertyFormulaComment> PropertyFormulaCommentItems = new List<PropertyFormulaComment>();
        //    public DP_FormDataRepository SourceData { set; get; }
        public bool LableIsShared { get { return (SourceData as DP_FormDataRepository).EditEntityArea is I_EditEntityAreaMultipleData; } }

        public List<ControlStateItem> ControlReadonlyStateItems = new List<ControlStateItem>();
        public List<ControlStateItem> ControlHiddenStateItems = new List<ControlStateItem>();


        public new List<DP_FormDataRepository> OriginalRelatedData { get { return base.OriginalRelatedData.Cast<DP_FormDataRepository>().ToList(); } }

        public new List<DP_FormDataRepository> RemovedDataForUpdate { get { return base.RemovedDataForUpdate.Cast<DP_FormDataRepository>().ToList(); } }
        public new List<DP_FormDataRepository> RelatedData { get { return base.RelatedData.Cast<DP_FormDataRepository>().ToList(); } }


        public ChildRelationshipInfo(RelationshipColumnControlGeneral relationshipControl, DP_FormDataRepository sourceData) : base(sourceData, relationshipControl.Relationship)
        {
            //    SourceData = sourceData;
            RelationshipControl = relationshipControl;
            // RelationshipControl = relationshipControl;
            //   HiddenData = new ObservableCollection<DP_FormDataRepository>();


            //کلا متدش باز نویسی بشه بره تو اد و ریمو
            //RelatedData.CollectionChanged += RelatedData_CollectionChanged;



            //RemovedItems = new List<ProxyLibrary.DP_FormDataRepository>();
            ReadonlyStateFromTails = new List<string>();
            // this.IsReadonlyChanged += ChildRelationshipInfo_IsReadonlyChanged;



        }
        I_View_TemporaryView GetTempView
        {
            get
            {
                return GetMainUIControlManager as I_View_TemporaryView;
                //else if (RelationshipControl is RelationshipColumnControlMultiple)
                //    return (RelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(SourceData);
                //  return null;
            }

        }



        List<I_UIElementManager> GetUIControlManager
        {
            get
            {
                List<I_UIElementManager> list = new List<I_UIElementManager>();
                list.Add(GetMainUIControlManager);
                var indirectDataView = GetIndirectDataviewUIManager;
                if (indirectDataView != null)
                    list.Add(indirectDataView);

                return list;
            }

        }
        I_UIElementManager GetMainUIControlManager
        {
            get
            {

                if (RelationshipControl is RelationshipColumnControlOne)
                {
                    return (RelationshipControl as RelationshipColumnControlOne).RelationshipControlManager.GetView();
                }
                else if (RelationshipControl is RelationshipColumnControlMultiple)
                    return (RelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(SourceData);

                return null;
            }

        }
        I_UIElementManager GetIndirectDataviewUIManager
        {
            get
            {
                if (!IsDirect)
                {
                    if (RelationshipControl is RelationshipColumnControlOne)
                    {
                        if (IsDataviewOpen)
                            return (RelationshipControl as RelationshipColumnControlOne).GenericEditNdTypeArea.DataViewGeneric;
                    }
                }
                return null;
            }
        }
        public List<I_UIElementManager> GetColumnControlDataManagers(ControlOrLabelAsTarget controlOrLabelAsTarget)
        {
            if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
            {
                return GetUIControlManager;
            }
            else
            {
                List<I_UIElementManager> result = new List<I_UIElementManager>();
                result.Add(RelationshipControl.LabelControlManager);
                return result;
            }
        }

        public void DecideButtonsEnablity1()
        {
            if (IsHidden)
            {
                foreach (var item in GetUIControlManager)
                    item.Visiblity(false);
                if (!LableIsShared)
                    RelationshipControl.LabelControlManager.Visiblity(false);

                var key = "";
                foreach (var item in ControlHiddenStateItems)
                {
                    key += (key == "" ? "" : ",") + "غیر قابل دسترسی بودن رابطه" + ":" + item.Message;
                }
                if (DataIsOneAndHidden)
                {
                    foreach (var item in RelatedData)
                    {
                        foreach (var item1 in item.ToParentRelationshipHiddenStateItems)
                        {
                            key += (key == "" ? "" : ",") + "غیر قابل دسترسی بودن رابطه" + ":" + item1.Message;
                        }
                    }
                }
                var message = "رابطه" + " " + RelationshipControl.Alias + " " + "برای داده" + " " + SourceData.ViewInfo + " " + "غیر قابل دسترسی می باشد";
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(message, key);
            }
            else
            {
                foreach (var item in GetUIControlManager)
                    item.Visiblity(true);
                if (!LableIsShared)
                    RelationshipControl.LabelControlManager.Visiblity(true);


                bool clearCommandEnablity = true;
                if (IsReadonly || !RelatedData.Any() || (RelatedData.All(x => x.IsDBRelationship && (x.ToParentRelationshipIsReadonly || x.ToParentRelationshipIsHidden))))
                    clearCommandEnablity = false;
                bool searchCommandEnablity = true;
                if (IsReadonly || (RelatedData.Any() && RelatedData.All(x => x.IsDBRelationship && (x.ToParentRelationshipIsReadonly || x.ToParentRelationshipIsHidden))))
                    searchCommandEnablity = false;
                if (IsDataviewOpen)
                {
                    bool addCommandEnablity = true;
                    var addCommand = RelationshipControl.GenericEditNdTypeArea.GetCommand(typeof(AddCommand));
                    if (addCommand != null)
                    {
                        if (IsReadonly)
                            addCommandEnablity = false;
                        else
                            addCommandEnablity = true;
                        addCommand.CommandManager.SetEnabled(addCommandEnablity);
                    }


                    bool removeCommandEnablity = true;
                    var removeCommand = RelationshipControl.GenericEditNdTypeArea.GetCommand(typeof(RemoveCommand));
                    if (removeCommand != null)
                    {
                        //در دل خودش 
                        if (IsReadonly || !RelatedData.Any() || RelatedData.All(x => x.IsDBRelationship && (x.ToParentRelationshipIsReadonly || x.ToParentRelationshipIsHidden)))
                        {
                            // باید رو ریمو خود داده کنترل شودIsDBRelationship ها
                            removeCommandEnablity = false;
                        }
                        removeCommand.CommandManager.SetEnabled(removeCommandEnablity);
                    }




                    var searchCommand = RelationshipControl.GenericEditNdTypeArea.GetCommand(typeof(SearchCommand));
                    if (searchCommand != null)
                        searchCommand.CommandManager.SetEnabled(searchCommandEnablity);





                    var clearCommand = RelationshipControl.GenericEditNdTypeArea.GetCommand(typeof(ClearCommand));
                    if (clearCommand != null)
                    {
                        clearCommand.CommandManager.SetEnabled(clearCommandEnablity);
                    }
                }


                if (!IsDirect)
                {
                    var tempView = GetTempView;

                    tempView.ButtonClearEnabled = clearCommandEnablity;
                    tempView.ButtonSearchFormEnabled = searchCommandEnablity;
                    tempView.ButtonPopupEnabled = searchCommandEnablity;
                    tempView.InfoTextboxReadOnly = !searchCommandEnablity;

                    //bool quickSearchEnablity = true;
                    //if (!searchCommandEnablity)
                       
                    //else
                    //    quickSearchEnablity = RelationshipControl.GenericEditNdTypeArea.TemporaryDisplayView.QuickSearchVisibility;
                    //tempView.DisableEnable(TemporaryLinkType.QuickSearch, quickSearchEnablity);

                    bool dataViewEnablity = true;
                    if (IsReadonly && !RelatedData.Any())
                        dataViewEnablity = false;
                    tempView.ButtonDataEditEnabled = dataViewEnablity;
                }



                List<ColumnControlColorItem> columnControlColorItems = new List<ColumnControlColorItem>();
                List<ColumnControlMessageItem> columnControlMessageItems = new List<ColumnControlMessageItem>();
                //**5410c49f-5d04-40f3-8b48-a579903e12bd
                if (RelationshipControl.Relationship.IsOtherSideMandatory)
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Label, ControlColorTarget.Foreground, "mandatory", ControlItemPriority.Normal));


                if (IsReadonly)
                {
                    var key = "";
                    if (RelationshipControl.Relationship.IsReadonly)
                    {
                        key += (key == "" ? "" : ",") + "تعریف رابطه";
                    }
                    foreach (var item in ControlReadonlyStateItems)
                    {
                        key += (key == "" ? "" : ",") + "فقط خواندنی بودن رابطه" + ":" + item.Message;
                    }

                    if (DataIsOneAndReadonly)
                    {
                        foreach (var item in RelatedData)
                        {
                            foreach (var item1 in item.ToParentRelationshipReadonlyStateItems)
                            {
                                key += (key == "" ? "" : ",") + "فقط خواندنی بودن داده" + ":" + item1.Message;
                            }
                        }
                    }
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "relationReadonly", ControlItemPriority.Normal));
                    columnControlMessageItems.Add(new ColumnControlMessageItem("رابطه فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد" + Environment.NewLine + key, ControlOrLabelAsTarget.Control, "relationReadonly", ControlItemPriority.Normal));
                }

                SetItemColor(columnControlColorItems);
                SetItemMessage(columnControlMessageItems);
            }
        }
        //private void ChildRelationshipInfo_IsReadonlyChanged(object sender, EventArgs e)
        //{
        //    if (SourceData.DataIsInEditMode())
        //    {
        //        RelationshipControl.EditNdTypeArea.SetChildRelationshipInfo(this);
        //        RelationshipControl.EditNdTypeArea.DecideButtonsEnablity1();
        //    }
        //}

        //private void RelatedData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //    if (CollectionChanged != null)
        //    //      CollectionChanged(sender, e);


        //    //       DataCollectionChanged(ChildRelationshipInfo.RelatedData, e);
        //    //        DecideButtonsEnablity1();






        //}



        //    RelationshipDeleteOption RelationshipDeleteOption { set; get; }
        //public RelationshipDeleteOption RelationshipDeleteOption { set; get; }

        //public int SourceRelationID;
        //public int SourceEntityID;
        //public int SourceTableID;
        //public int TargetTableID;
        //public Enum_RelationshipType SourceToTargetRelationshipType;
        //public Enum_MasterRelationshipType SourceToTargetMasterRelationshipType;
        //public void AddDataToChildRelationshipInfo(DP_DataRepository dataItem, bool fromDB)
        //{

        //}
        public void AddDataToChildRelationshipInfo(DP_FormDataRepository dataItem)
        {
            if (IsReadonly && dataItem.IsNewItem && (!dataItem.IsDefaultData || !IsDataviewOpen))
                throw new Exception();
            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData && RelatedData.Count > 0)
                throw new Exception("sdfsdfsdfsdf");

            //    dataItem.ToParentRelationshipID = Relationship.PairRelationshipID;
            if (!dataItem.IsNewItem)
            {
                //عملا میشه داده ای که یا انتخاب شده از سلکت ویو یا از رابطه اصلی اومده
                dataItem.EditEntityArea.AreaInitializer.ActionActivityManager.SetExistingDataFirstLoadStates(dataItem);

                if (!dataItem.IsDBRelationship)
                {
                    //عملا میشه داده ای که انتخاب شده از سلکت ویو
                    if (dataItem.ToParentRelationshipIsHidden)
                    {
                        var key = "";
                        foreach (var item in dataItem.ToParentRelationshipHiddenStateItems)
                        {
                            key += (key == "" ? "" : ",") + "غیر قابل دسترسی بودن رابطه" + ":" + item.Message;
                        }
                        var message = "رابطه" + " " + RelationshipControl.Alias + " " + "برای داده" + " " + dataItem.ViewInfo + " " + "غیر قابل دسترسی می باشد";
                        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(message, key);
                        return;
                    }
                    else
                    {
                        if (dataItem.ToParentRelationshipIsReadonly)
                        {
                            var key = "";
                            foreach (var item in dataItem.ToParentRelationshipReadonlyStateItems)
                            {
                                key += (key == "" ? "" : ",") + "فقط خواندنی بودن رابطه" + ":" + item.Message;
                            }
                            var message = "رابطه" + " " + RelationshipControl.Alias + " " + "برای داده" + " " + dataItem.ViewInfo + " " + "غیر قابل دسترسی می باشد";
                            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(message, key);
                            return;
                        }
                    }
                }
            }
            dataItem.ParantChildRelationshipData = new ParentRelationshipInfo(this);


            //   childRelationshipData.
            //  dataItem.ParantChildRelationshipData = new ParentRelationshipData();
            base.RelatedData.Add(dataItem);
            if (dataItem.IsDBRelationship)
            {
                //اینجا میتونه از این نوع نباشه بلکه DP_DataRepository
                DP_FormDataRepository orgData = new DP_FormDataRepository(dataItem, dataItem.EditEntityArea, dataItem.IsDBRelationship, dataItem.IsNewItem);
                //         orgData.ParantChildRelationshipInfo = parentRelationshipInfo;
                foreach (var item in dataItem.KeyProperties)
                    orgData.AddCopyProperty(item);
                base.OriginalRelatedData.Add(orgData);
            }
            else
                CheckRelationshipChanged(enum_AddRemove.Add, dataItem);


            if (IsDataviewOpen)
            {
                ShowDataInDataView(dataItem);

            }

            if (!IsDirect)
            {
                SetTempText();
            }
            //CheckDataParentRelationship(dataItem);

            DecideButtonsEnablity1();
        }

        private void ShowDataInDataView(DP_FormDataRepository dataItem)
        {
            RelationshipControl.GenericEditNdTypeArea.ShowDataInDataView(dataItem);


            foreach (var changeMonitor in ChangeMonitorItems.Where(x => !string.IsNullOrEmpty(x.RestTail) || x.columnID != 0))
            {
                dataItem.AddChangeMonitorIfNotExists(changeMonitor.GeneralKey, changeMonitor.UsageKey, changeMonitor.RestTail, changeMonitor.columnID, changeMonitor.DataToCall);
            }
            //if (ChangeMonitorItems.Any(x => x.columnID == 0))
            //{


            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaMultipleData)
            {
                if (dataItem.ToParentRelationshipIsReadonly)
                {
                    var tooltip = "رابطه داده به علت فقط خواندنی بودن غیر قابل تغییر است";
                    foreach (var item in dataItem.ToParentRelationshipReadonlyStateItems)
                        tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
                    (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.SetTooltip(dataItem, tooltip);
                }
                else if (dataItem.ToParentRelationshipIsHidden)
                {
                    var tooltip = "داده به علت عدم دسترسی به رابطه غیرفعال است";
                    foreach (var item in dataItem.ToParentRelationshipHiddenStateItems)
                        tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
                    (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.SetTooltip(dataItem, tooltip);
                    (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.EnableDisable(dataItem, false);
                }
            }
        }

        //private void CheckDataParentRelationship(DP_FormDataRepository dataItem)
        //{
        //    if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
        //    {
        //        if (!dataItem.IsNewItem)
        //        {
        //            if (dataItem.IsDBRelationship)
        //            {
        //                //if (dataItem.ToParentRelationshipReadonlyStateItems.Any())
        //                //{
        //                //    foreach (var item in dataItem.ToParentRelationshipReadonlyStateItems)
        //                //        AddReadonlyState(item.Key, item.Message, item.Permanent, true);
        //                //}
        //                //if (dataItem.ToParentRelationshipHiddenStateItems.Any())
        //                //{
        //                //    foreach (var item in dataItem.ToParentRelationshipHiddenStateItems)
        //                //        AddHiddenState(item.Key, item.Message, item.Permanent, true);
        //                //}
        //            }
        //            else
        //            {
        //              

        //                //     بعدا باید وضعیتهایی که ستون یا رابطه را حذف میکند یعنی حالتههای اولیه رو برای داده هاییی که تمپ هستند و بعدا رو دیتا ویو کلیک می شود تست شود و فرکی شود
        //            }
        //        }
        //    }
        //}

        //public bool CheckRelationshipIsChanged(ChildRelationshipInfo relation)
        //{
        //    if (OriginalRelatedData.Any(x => x.KeyProperties.All(y => !RelatedData.Any(z => z.IsNewItem == false && z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
        //    {
        //        //داده ای حذف شده است
        //        return true;
        //    }
        //    if (RelatedData.Any(x => x.KeyProperties.All(y => !OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
        //    {
        //        //داده ای اضافه شده است
        //        return true;
        //    }
        //    return false;
        //}

        public void SetTempText()
        {
            string text = "";
            //اینجا باید داده هایی که اسکیپ میشن رو در نظر نگیره
            if (RelatedData.Count > 1)
                text = "تعداد" + " " + RelatedData.Count + " " + "مورد";
            foreach (var item in RelatedData)
                text += (text == "" ? "" : Environment.NewLine) + item.ViewInfo;


            //      var relationshipControl = RelationshipControl.ParentEditArea.RelationshipColumnControls.First(x => x.Relationship.ID == Relationship.ID) as RelationshipColumnControlMultiple;
            GetTempView.SetLinkText(text);
            //if (string.IsNullOrEmpty(text) && relationshipControl.EditNdTypeArea.TemporaryLinkState.searchView)
            //    relationshipControl.RelationshipControlManager.SetQuickSearchVisibility(ChildRelationshipInfo.SourceData, true);
            //else
            //    relationshipControl.RelationshipControlManager.SetQuickSearchVisibility(ChildRelationshipInfo.SourceData, false);
            //GetTempViewForMultipleRelationshipColumn.QuickSearchVisibility = false;

            //}


            //////foreach (var dataItem in relatedData)
            //////    OnDataItemShown(new EditAreaDataItemLoadedArg() { DataItem = dataItem, InEditMode = false });

        }

        public bool CheckRelationshipIsChanged()
        {
            foreach (var deleted in OriginalRelatedData.Where(x => !x.KeyProperties.All(y => RelatedData.Any(z => z.IsNewItem == false && z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
            {
                return true;
            }
            foreach (var added in RelatedData.Where(x => !x.KeyProperties.All(y => OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
            {
                return true;
            }
            return false;
        }
        public bool DataItemIsAdded(DP_FormDataRepository item)
        {
            return item.IsNewItem || !item.KeyProperties.All(y => OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)));
        }
        //public bool OriginalDataItemIsRemoved(DP_FormDataRepository originalData)
        //{
        //}
        //public void CheckAddedRemovedRelationships()
        //{
        //    //if (IsReadonly)
        //    //    return;
        //    foreach (var deleted in OriginalRelatedData.Where(x => !x.KeyProperties.All(y => RelatedData.Any(z => z.IsNewItem == false && z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
        //    {
        //        if (deleted.IsHidden)
        //        {
        //            throw (new Exception("داده غیر فعال امکان حذف شدن را ندارد"));
        //        }
        //        deleted.RelationshipIsRemoved = true;
        //    }
        //    foreach (var added in RelatedData.Where(x => !x.KeyProperties.All(y => OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
        //    {
        //        added.RelationshipIsAdded = true;
        //        if (added.IsHidden)
        //        {
        //            throw (new Exception("داده غیر فعال امکان اضافه شدن را ندارد"));
        //        }
        //    }
        //}
        //public bool RelationshipIsChangedForUpdate
        //{
        //    get
        //    {
        //        return RemovedDataForUpdate.Any() || RelatedData.Any(x => DataItemIsAdded(x));
        //    }
        //}
        public List<DP_FormDataRepository> GetRelatedData(int relationshipID)
        {
            return RelatedData.ToList();
            //    else return new List<ProxyLibrary.DP_FormDataRepository>();
        }
        public bool RemoveRelatedData(DP_FormDataRepository DP_FormDataRepository)
        {
            //var childRelationshipInfo = ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationshipID);
            //if (childRelationshipInfo != null)

            //اینجا باید کنترل بشه که میشه حذف بشه اصلا یا نه
            var clearIsOk = CheckRemoveData(DP_FormDataRepository);

            if (clearIsOk)
            {
                base.RelatedData.Remove(DP_FormDataRepository);


                CheckRelationshipChanged(enum_AddRemove.Remove, DP_FormDataRepository);
                if (!IsDirect)
                {
                    SetTempText();
                }
                if (IsDataviewOpen)
                {
                    RelationshipControl.GenericEditNdTypeArea.ClearUIData(DP_FormDataRepository);

                    if (RelatedData.Count == 0 && RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                    {
                        CreateDefaultData();
                    }
                }
                DecideButtonsEnablity1();
                return true;
            }
            else
                return false;
        }

        private void CheckRelationshipChanged(enum_AddRemove mode, DP_FormDataRepository dataItem)
        {
            foreach (var item in ChangeMonitorItems)
            {
                item.DataToCall.OnRelatedDataOrColumnChanged(item);
            }
            //برای ریمو باید پیش از این نال بشنن مقادیر
            if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign && mode == enum_AddRemove.Remove)
                return;
            //List<Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>> changeFkItemProperties = new List<Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>>();
            DP_FormDataRepository fkDataItem = null;
            DP_FormDataRepository pkData = null;
            List<Tuple<EntityInstanceProperty, EntityInstanceProperty>> PkFkColumns = new List<Tuple<EntityInstanceProperty, EntityInstanceProperty>>();
            //    RelationChangedMode? changedmode = null;

            if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
            {
                pkData = dataItem;
                fkDataItem = SourceData as DP_FormDataRepository;
                foreach (var col in Relationship.RelationshipColumns)
                {
                    var pkProp = pkData.GetProperty(col.SecondSideColumnID);
                    var fkProp = fkDataItem.GetProperty(col.FirstSideColumnID);
                    PkFkColumns.Add(new Tuple<EntityInstanceProperty, EntityInstanceProperty>(pkProp, fkProp));
                }
            }
            else if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign && mode == enum_AddRemove.Add)
            {
                pkData = SourceData as DP_FormDataRepository;
                fkDataItem = dataItem;
                foreach (var col in Relationship.RelationshipColumns)
                {
                    var pkProp = pkData.GetProperty(col.FirstSideColumnID);
                    var fkProp = fkDataItem.GetProperty(col.SecondSideColumnID);
                    PkFkColumns.Add(new Tuple<EntityInstanceProperty, EntityInstanceProperty>(pkProp, fkProp));

                }
            }
            List<UIColumnValueDTO> uIColumnValue = new List<UIColumnValueDTO>();
            foreach (var item in PkFkColumns)
            {
                if (mode == enum_AddRemove.Add)
                {
                    uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = item.Item2.ColumnID, ExactValue = item.Item1.Value?.ToString(), EvenHasValue = true, EvenIsNotNew = true });
                }
                else if (mode == enum_AddRemove.Remove)
                {
                    uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = item.Item2.ColumnID, ExactValue = null, EvenHasValue = true, EvenIsNotNew = true });

                }
            }

            fkDataItem.SetColumnValue(uIColumnValue, null, null, true);

            if (mode == enum_AddRemove.Add)
            {
                //if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
                //{

                //int foreignToPrimaryRelID = 0;
                //if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
                //{
                //    foreignToPrimaryRelID = Relationship.ID;
                //}
                //else if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign && mode == enum_AddRemove.Add)
                //{
                //    foreignToPrimaryRelID = Relationship.PairRelationshipID;
                //}

                //       fkDataItem.RelatedDataTailOrColumnChanged += FkDataItem_RelatedDataTailOrColumnChanged;
                foreach (var item in PkFkColumns)
                {
                    var childProperty = pkData.Properties.First(x => x.ColumnID == item.Item1.ColumnID);
                    childProperty.PropertyValueChanged += (sender, e) => Property_PropertyValueChanged(sender, e, fkDataItem, item.Item2);
                    //  pkData.AddChangeMonitorIfNotExists("changePKProperties", item.Item2.ColumnID.ToString(), foreignToPrimaryRelID.ToString(), item.Item1.ColumnID);
                }
                //}
                //else if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign && mode == enum_AddRemove.Add)
                //{

                //}
            }
        }

        private void Property_PropertyValueChanged(object sender, PropertyValueChangedArg e, DP_FormDataRepository fkDataItem, EntityInstanceProperty item2)
        {
            //if (e.GeneralKey.StartsWith("changePKProperties"))
            //{
            //       var column = e.SourceData.Properties.First(x => x.ColumnID == e.columnID);
            List<UIColumnValueDTO> uIColumnValue = new List<UIColumnValueDTO>();
            uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = item2.ColumnID, ExactValue = e.NewValue, EvenHasValue = true, EvenIsNotNew = true });
            fkDataItem.SetColumnValue(uIColumnValue, null, null, true);
            //}
        }



        //private void FkDataItem_RelatedDataTailOrColumnChanged(object sender, ChangeMonitor e)
        //{
        //}

        public bool IsDataviewOpen
        {
            get
            {
                return IsDirect ||
                                (RelationshipControl.GenericEditNdTypeArea.DataViewGeneric != null &&
                                RelationshipControl.GenericEditNdTypeArea.DataViewGeneric.IsOpenedTemporary);
            }
        }
        public bool IsDirect
        {
            get
            {
                return (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                  RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
            }
        }
        public bool CheckRemoveData(DP_FormDataRepository data)
        {
            bool clearIsOk = true;
            if (data.IsDBRelationship)
            {
                //برای روابط پرایمری به فارن که وضعیت اعمال میشه
                //if (data.ToParantChildRelationshipInfo.IsHidden)// || data.IsReadonlyOnState)
                //    return false;

                //برای روابط فارن به پرایمری که وضعیت اعمال میشه
                if (data.ToParentRelationshipIsHidden || data.ToParentRelationshipIsReadonly)
                    return false;
            }
            bool shouldDeleteFromDB = false;
            //   var existingdatas = datas.Where(x => x.IsDBRelationship);
            ///   if (existingdatas.Count() != 0)
            //   {


            if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
            {
                if (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.SourceRelationColumnControl.Relationship.DBDeleteRule == RelationshipDeleteUpdateRule.Cascade)//|| RelationshipControl.GenericEditNdTypeArea.AreaInitializer.SourceRelationColumnControl.Relationship.RelationshipColumns.Any(x => !x.SecondSideColumn.IsNull))
                {
                    shouldDeleteFromDB = true;
                }
            }
            //بعدا به این فکر شود
            //var relationship = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipManager.GetRelationship(AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID);
            //if (relationship.IsOtherSideMandatory)
            //    shouldDeleteFromDB = true;

            //  }

            if (shouldDeleteFromDB)
            {
                //////if (AreaInitializer.SecurityEditAndDelete == false)
                //////{
                //////    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("پیام", "به علت رابطه اجباری داده نیاز به حذف شدن دارد اما دسترسی حذف وجود ندارد");
                //////    clearIsOk = false;
                //////}
                // var deleteList = existingdatas.ToList();
                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                DR_DeleteInquiryRequest request = new DR_DeleteInquiryRequest(requester);
                request.DataItems = new List<DP_BaseData>() { data };
                var reuslt = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendDeleteInquiryRequest(request);
                I_ViewDeleteInquiry view = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDeleteInquiryView();
                view.SetTreeItems(reuslt.DataTreeItems);
                if (reuslt.Loop == true)
                {
                    clearIsOk = false;
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("بعلت وجود حلقه وابستگی بین داده ها امکان حذف داده (داده های) انتخاب شده وجود ندارد");
                }
                else
                {
                    view.SetUserConfirmMode(UserDialogMode.YesNo);
                    // if (reuslt.DataTreeItems.Any(x => x.ChildRelationshipDatas.Any(y => y.RelationshipDeleteOption == ModelEntites.RelationshipDeleteOption.DeleteCascade && y.RelatedData.Any())))
                    //        view.SetMessage("داده های وابسته نمایش داده شده نیز حذف خواهند شد. آیا مطمئن هستید؟");
                    //   else
                    view.SetMessage("داده های نمایش داده شده نیز بروزرسانی و یا حذف خواهند شد. آیا مطمئن هستید؟");

                    var result = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowPromptDialog(view, "");
                    if (result == UserDialogResult.Ok || result == UserDialogResult.No)
                    {
                        clearIsOk = false;
                    }
                    else if (result == UserDialogResult.Yes)
                    {
                        clearIsOk = true;
                    }
                }
            }

            return clearIsOk;
        }
        internal void RemoveRelatedData(List<DP_FormDataRepository> datas)
        {
            foreach (var item in datas)
                RemoveRelatedData(item);
        }
        public bool RemoveRelatedData()
        {
            //var childRelationshipInfo = ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationshipID);
            //if (childRelationshipInfo != null)
            bool result = true;
            foreach (var item in RelatedData)
            {
                if (!RemoveRelatedData(item))
                    result = false;
            }
            return result;
        }


        //internal void AddDataObserver(string key, string restTail, int columnID, DP_FormDataRepository targetDataItem, bool setSourceOrChilds)
        //{
        //    ObserverListForColumn.Add(new ObserverData()
        //    {
        //        Key = key,
        //        ColumnID = columnID,
        //        TargetDataItem = targetDataItem,
        //        SourceRalationshipTail = restTail
        //    });

        //    if (setSourceOrChilds)
        //    {
        //        SourceData.AddDataObserver(key, restTail, columnID, targetDataItem);
        //    }
        //    else
        //    {
        //        foreach (var relatedData in RelatedData)
        //        {
        //            relatedData.AddDataObserver(key, restTail, columnID, targetDataItem);
        //        }
        //    }
        //}


        //internal void RemoveDataObserver(string key, string restTail, DP_FormDataRepository targetDataItem)
        //{
        //    foreach (var item in ObserverList.ToList())
        //    {
        //        if (key == item.Key && item.TargetDataItem == targetDataItem)
        //        {
        //            ObserverList.Remove(item);
        //        }
        //    }

        //}
        //public RelationshipDTO Relationship { get { return RelationshipControl.Relationship; } }

        //public ObservableCollection<DP_FormDataRepository> RemovedDataForUpdate { set; get; }
        //public ObservableCollection<DP_FormDataRepository> RelatedData { set; get; }
        public ObservableCollection<DP_FormDataRepository> RealData
        {
            get
            {
                ObservableCollection<DP_FormDataRepository> result = new ObservableCollection<DP_FormDataRepository>();
                foreach (var item in RelatedData)
                {
                    if (item.ShoudBeCounted)
                    {
                        if (item.ToParentRelationshipIsHidden || this.IsReadonlyOnState)// || item.IsReadonlyOnState)
                        {
                            if (!DataItemIsAdded(item))
                                result.Add(item);
                        }
                        else
                            result.Add(item);
                    }
                }
                foreach (var item in RemovedOriginalDatas)
                {
                    if (item.ToParentRelationshipIsHidden || this.IsReadonlyOnState)// || item.IsReadonlyOnState)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
        }

        public bool SecurityIssue { get; set; }
        //    public ObservableCollection<DP_FormDataRepository> OriginalRelatedData { get; private set; }
        public List<DP_FormDataRepository> RemovedOriginalDatas
        {
            get
            {
                return OriginalRelatedData.Where(x => !x.KeyProperties.All(y => RelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value.Equals(y.Value))))).ToList();
            }
        }
        public List<string> ReadonlyStateFromTails { set; get; }

        //میشه ریدونلی بودن ریلیشنشیپ رو هم داخل این گذاشت یا لازم نیست؟

        public bool IsReadonly
        {
            get
            {

                //** 4a3e4287-2824-4019-ad03-e276ef44e0fb


                return Relationship.IsReadonly || ControlReadonlyStateItems.Any() || DataIsOneAndReadonly;


                //|| (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                //              (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
            }
        }
        public bool DataIsOneAndReadonly
        {
            get
            {
                if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                {
                    if (RelatedData.Any() && RelatedData.All(x => x.IsDBRelationship && x.ToParentRelationshipReadonlyStateItems.Any()))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsHidden
        {
            get
            {
                return ControlHiddenStateItems.Any() || DataIsOneAndHidden || DateSecurityIssue;
            }
        }
        public bool DataIsOneAndHidden
        {
            get
            {
                if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                {
                    if (RelatedData.Any() && RelatedData.All(x => x.IsDBRelationship && x.ToParentRelationshipHiddenStateItems.Any()))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsReadonlyOnState
        {
            get
            {
                return ControlReadonlyStateItems.Any();
            }
        }
        //این باید خود رابطه را چک کنه همچنین اینکه رابطه کریتورش فقط خواندنی هست یا نه؟ خود موجودیت طرفین هم فکر بشه

        List<ChangeMonitor> ChangeMonitorItems = new List<ChangeMonitor>();

        //اینجا وظیفه چک کردن هم داده ها و هم ستونهای داده را دارد
        internal void AddChangeMonitor(string generalKey, string usageKey, string restTail, int columnID, DP_FormDataRepository dataToCall)
        {
            ChangeMonitorItems.Add(new ChangeMonitor()
            {
                GeneralKey = generalKey,
                UsageKey = usageKey,
                DataToCall = dataToCall,
                columnID = columnID,
                RestTail = restTail

            });

            //if (!string.IsNullOrEmpty(restTail))
            //{


            //foreach (var relatedData in RelatedData)
            //{
            //    relatedData.AddChangeMonitor(generalKey, usageKey, restTail, columnID, dataToCall);
            //}
            //}
        }



        public void RemoveChangeMonitorByGenaralKey(string key)
        {
            foreach (var item in ChangeMonitorItems.Where(x => x.GeneralKey == key).ToList())
            {
                ChangeMonitorItems.Remove(item);
                foreach (var data in RelatedData)
                {
                    data.RemoveChangeMonitorByGenaralKey(key);
                }
            }
        }

        //public bool OriginalDataHasBecomeHidden(DP_FormDataRepository orginalData)
        //{
        //    var currentData = RelatedData.FirstOrDefault(z => z.IsNewItem == false && orginalData.KeyProperties.All(x => z.KeyProperties.Any(u => x.ColumnID == u.ColumnID && x.Value == u.Value)));
        //    if (currentData != null && currentData.IsHidden)
        //        return true;
        //    else
        //        return false;
        //}

        public DP_FormDataRepository GetOroginalDataOfOriginalData(DP_FormDataRepository data)
        {
            return OriginalRelatedData.First(z => data.KeyProperties.All(y => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)));
        }

        public DP_FormDataRepository GetRelatedDataOfOriginalData(DP_FormDataRepository orginalData)
        {
            return RelatedData.First(z => orginalData.KeyProperties.All(y => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)));
        }




        //public ObservableCollection<DP_FormDataRepository> HiddenData { set; get; }
        //public void AddHiddenDataRelationship(DP_FormDataRepository dataItem)
        //{
        //    if (!HiddenData.Any(x => x == dataItem))
        //        HiddenData.Add(dataItem);
        //}
        //public void RemoveHiddenDataRelationship(DP_FormDataRepository dataItem)
        //{
        //    if (HiddenData.Any(x => x == dataItem))
        //    {
        //        HiddenData.Remove(HiddenData.First(x => x == dataItem));
        //    }
        //}




        //public bool OriginalDataHasBecomeReadonlyAndNotExists(DP_FormDataRepository orginalRelationships)
        //{
        //    var currentData = RelatedData.FirstOrDefault(z => z.IsNewItem == false && orginalRelationships.KeyProperties.All(x => z.KeyProperties.Any(u => x.ColumnID == u.ColumnID && x.Value == u.Value)));
        //    if (currentData != null && currentData.IsReadonly)
        //        return true;
        //    else
        //        return false;
        //}
        //   public bool RelationshipIsChanged { get; set; }




        bool dataLoaded = false;


        public bool SetBinding()
        {
            //اینکه اینجا هیدن ها بایند نمیشن خوب نیست چون برای ارث بری مثلا شخص فرمهای شخص حقیقی و حقوقی مخفی هستن اولش و بایند نمیشن. بعدا یه فکری بشه
            RelationshipControl.GenericEditNdTypeArea.ChildRelationshipInfoBinded = this;
            if (!IsHidden)
            {
                //if (!DateSecurityIssue)
                //{
                if (IsDataviewOpen)
                {
                    ClearUIData();
                }
                if (!IsDirect)
                    GetTempView.SetLinkText("");
                //bool SecurityIssue = false;
                //bool result = true;
                if (dataLoaded)
                {
                    if (IsDataviewOpen)
                    {
                        // RelationshipControl.GenericEditNdTypeArea.ClearUIData();
                        foreach (var item in RelatedData)
                        {
                            ShowDataInDataView(item);
                        }
                    }

                    if (!IsDirect)
                    {
                        SetTempText();
                    }
                }
                //  childData = specificDate.ChildRelationshipInfos.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                else
                {
                    dataLoaded = true;

                    bool relationshipFirstSideHasValue = RelationshipControl.Relationship.RelationshipColumns.Any()
                        && RelationshipControl.Relationship.RelationshipColumns.All(x => SourceData.GetProperties().Any(y => !y.ValueIsEmpty() && y.ColumnID == x.FirstSideColumnID));
                    if (!relationshipFirstSideHasValue)
                    {
                        //childData = specificDate.AddChildRelationshipInfo(relationshipControl);

                    }
                    else
                    {
                        //bool childIsDataView = (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                        //                       RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
                        Tuple<bool, List<DP_FormDataRepository>> dbSearch;
                        if (IsDataviewOpen)
                            dbSearch = SerachDataFromParentRelationForChildDataView();
                        else
                            dbSearch = SerachDataFromParentRelationForChildTempView();
                        if (dbSearch.Item1)
                        {
                            DateSecurityIssue = true;
                            DecideButtonsEnablity1();
                            return false;
                            //   AddHiddenState("DataIssue", "عدم دسترسی به داده", true, true);

                        }
                        else
                        {
                            foreach (var item in dbSearch.Item2)
                                AddDataToChildRelationshipInfo(item);
                        }
                    }
                }

                if (IsDataviewOpen && RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                {
                    if (!RelatedData.Any())
                    {

                        CreateDefaultData();
                    }
                }


                if (RelationshipControl is RelationshipColumnControlMultiple)
                {
                    GetTempView.TemporaryDisplayViewRequested += GetTempView_TemporaryDisplayViewRequested;
                }
                DecideButtonsEnablity1();
                return true;

                //}
            }
            else
            {

            }
            return false;

        }

        private void ClearUIData()
        {
            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
            {

            }
            else if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaMultipleData)
            {
                (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).RemoveDataContainers();
            }

        }

        private void CreateDefaultData()
        {
            var newData = AgentHelper.CreateAreaInitializerNewData(RelationshipControl.GenericEditNdTypeArea);
            newData.IsDefaultData = true;
            if (IsReadonly)
            {
                newData.IsUseLessBecauseNewAndReadonly = true;
                foreach (var property in newData.ChildSimpleContorlProperties)
                {
                    property.AddReadonlyState("", "DataNewAndReadonly", true);
                }
                foreach (var rel in newData.ChildRelationshipDatas)
                {
                    rel.AddReadonlyState("", "DataNewAndReadonly", true);
                }
            }
            AddDataToChildRelationshipInfo(newData);
        }
        private void GetTempView_TemporaryDisplayViewRequested(object sender, Arg_TemporaryDisplayViewRequested e)
        {
            TemporaryViewActionRequested(sender as I_View_TemporaryView, e.LinkType);
        }

        RelationshipDataManager relationshipManager = new RelationshipDataManager();

        private Tuple<bool, List<DP_FormDataRepository>> SerachDataFromParentRelationForChildTempView()
        {
            var searchDataItem = relationshipManager.GetSecondSideSearchDataItemByRelationship(SourceData, RelationshipControl.Relationship.ID);



            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
            DR_SearchViewRequest request = new DR_SearchViewRequest(requester, searchDataItem);
            if (RelationshipControl.GenericEditNdTypeArea.DefaultEntityListViewDTO != null)
                request.EntityViewID = RelationshipControl.GenericEditNdTypeArea.DefaultEntityListViewDTO.ID;
            //request.CheckStates = true;
            //      request.ToParentRelationshipID = relationship.PairRelationshipID;

            var childViewData = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(request).ResultDataItems;

            var countRequest = new DR_SearchCountRequest(requester);
            countRequest.SearchDataItems = searchDataItem;
            countRequest.Requester.SkipSecurity = true;
            var count = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchCountRequest(countRequest);
            //     bool secutrityImposed = false;
            if (count.ResultCount != childViewData.Count)
                return new Tuple<bool, List<DP_FormDataRepository>>(true, null);
            //if (!secutrityImposed)
            //{
            List<DP_FormDataRepository> list = new List<DP_FormDataRepository>();

            EditAreaDataManager EditAreaDataManager = new EditAreaDataManager();
            foreach (var item in childViewData)
            {
                var dpItem = new DP_FormDataRepository(item, RelationshipControl.GenericEditNdTypeArea, true, false);
                //dpItem.IsDBRelationship = true;

                list.Add(dpItem);
            }
            // }
            //else
            //    childRelationshipInfo.SecurityIssue = true;

            return new Tuple<bool, List<DP_FormDataRepository>>(false, list);

        }


        public Tuple<bool, List<DP_FormDataRepository>> SerachDataFromParentRelationForChildDataView()
        {
            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();

            //ChildRelationshipInfo childRelationshipInfo = null;
            //childRelationshipInfo = parentRelationData.ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationship.ID);
            //if (childRelationshipInfo == null)
            //{
            //    childRelationshipInfo = parentRelationData.AddChildRelationshipInfo(relationshipColumnControl);
            //}
            //else
            //{
            //    throw new Exception("Asd");
            //}

            //سکوریتی داده اعمال میشود

            var searchDataItem = relationshipManager.GetSecondSideSearchDataItemByRelationship(SourceData, Relationship.ID);

            // DR_SearchEditRequest request = new DR_SearchEditRequest(requester, searchDataItem, targetEditEntityArea.AreaInitializer.SecurityReadOnly, true);
            DR_SearchEditRequest request = new DR_SearchEditRequest(requester, searchDataItem);

            var childFullData = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchEditRequest(request).ResultDataItems;
            var countRequest = new DR_SearchCountRequest(requester);
            countRequest.SearchDataItems = searchDataItem;
            countRequest.Requester.SkipSecurity = true;
            var count = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchCountRequest(countRequest);
            if (count.ResultCount != childFullData.Count)
            {
                return new Tuple<bool, List<DP_FormDataRepository>>(true, null);
            }


            //if (!secutrityImposed)
            //{

            List<DP_FormDataRepository> list = new List<DP_FormDataRepository>();
            EditAreaDataManager EditAreaDataManager = new EditAreaDataManager();
            foreach (var data in childFullData)
            {
                //  data.IsDBRelationship = true;
                data.DataView = EditAreaDataManager.GetDataView(data);

                DP_FormDataRepository formData = new DP_FormDataRepository(data, RelationshipControl.GenericEditNdTypeArea, true, false);
                //   formData.IsDBRelationship = true;
                list.Add(formData);
            }

            //}
            //else
            //    childRelationshipInfo.SecurityIssue = true;
            return new Tuple<bool, List<DP_FormDataRepository>>(false, list);

            //return childRelationshipInfo;
            //foreach (var item in childFullData)
            //    searchedData.Add(new Tuple<DP_FormDataRepository, DP_DataView>(item, null));

            //return AddEditSearchData(searchedData, editEntityArea);
        }


        public void AddReadonlyState(string key, string message, bool permanent)
        {
            if (ControlReadonlyStateItems.Any(x => x.Key == key))
                ControlReadonlyStateItems.Remove(ControlReadonlyStateItems.First(x => x.Key == key));
            ControlReadonlyStateItems.Add(new ControlStateItem(key, message, permanent));

            //if (checkInUI)
            //{
            //    CheckColumnReadonly();
            //    SetMessageAndColor();
            //}
        }



        //public void RemoveReadonlyState(string key, bool checkInUI)
        //{
        //    if (ControlReadonlyStateItems.Any(x => x.Key == key && x.Permanent == false))
        //        ControlReadonlyStateItems.RemoveAll(x => x.Key == key && x.Permanent == false);

        //    if (checkInUI)
        //    {
        //        CheckColumnReadonly();
        //        SetMessageAndColor();
        //    }
        //}
        private void CheckColumnReadonly()
        {

        }

        //public void SetRelatoinsipColumnReadonlyFromState(string message, string key, bool permanent, bool checkInUI)
        //{
        //    if (checkInUI)
        //    {
        //        if (SourceData.DataIsInEditMode())
        //        {
        //            AddReadonlyState(key, message, permanent, checkInUI);
        //        }
        //    }
        //    else
        //    {
        //        AddReadonlyState(key, message, permanent, checkInUI);
        //    }
        //}
        //public void ResetColumnReadonlyFromState(string key)
        //{
        //    if (SourceData.DataIsInEditMode())
        //    {
        //        RemoveReadonlyState(key);
        //        CheckColumnReadonly();
        //    }
        //}


        //public void SetRelatoinsipColumnHiddenFromState(string message, string key, bool permanent, bool checkInUI)
        //{
        //    if (checkInUI)
        //    {
        //        if (SourceData.DataIsInEditMode())
        //        {
        //            AddHiddenState(key, message, permanent, checkInUI);

        //        }
        //    }
        //    else
        //        AddHiddenState(key, message, permanent, checkInUI);

        //}

        public void AddHiddenState(string key, string message, bool permanent)
        {
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.Remove(ControlHiddenStateItems.First(x => x.Key == key));
            ControlHiddenStateItems.Add(new ControlStateItem(key, message, permanent));


            //   DecideVisiblity();
            //SetMessageAndColor();
            DecideButtonsEnablity1();

        }
        public void RemoveHiddenState(string key)
        {
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.RemoveAll(x => x.Key == key);

            DecideButtonsEnablity1();
            //  DecideVisiblity();
            //  SetMessageAndColor();

        }

        //private void DecideVisiblity()
        //{

        //}




        public void SelectFromParent(Dictionary<int, object> colAndValues)
        {
            if ((SourceData as DP_FormDataRepository).DataIsInEditMode())
            {
                if (!IsReadonly && !IsHidden)
                {
                    //    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);

                    bool fromDataview = (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                               RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
                    RelationshipControl.GenericEditNdTypeArea.SelectFromParent(fromDataview, Relationship, SourceData, colAndValues);
                }
            }
        }

        internal void DataSelected(DP_FormDataRepository result)
        {
            if (RemoveRelatedData())
                AddDataToChildRelationshipInfo(result);
        }

        internal void DataViewRequested()
        {
            //   ObservableCollection<DP_FormDataRepository> existingData = RealData;

            if (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.FormComposed == false)
            {
                RelationshipControl.GenericEditNdTypeArea.GenerateDataView();
            }

            RelationshipControl.GenericEditNdTypeArea.DataViewGeneric.IsOpenedTemporary = true;

            //////if (AreaInitializer.SourceRelationColumnControl != null)
            //////{
            //////    AreaInitializer.SourceRelationColumnControl.OnDataViewForTemporaryViewShown(ChildRelationshipInfo);
            //////}
            ///
            ClearUIData();

            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData && RelatedData.Count == 0)
            {

                //اینجا باید فقط برای فرم اصلی باشد
                //(this as I_EditEntityAreaOneData).CreateDefaultData();

                CreateDefaultData();

                //var newData = AgentHelper.CreateAreaInitializerNewData(RelationshipControl.GenericEditNdTypeArea);
                //newData.IsDefaultData = true;
                //AddDataToChildRelationshipInfo(newData);

            }
            else
            {
                //if (this is I_EditEntityAreaMultipleData)
                //    (this as I_EditEntityAreaMultipleData).RemoveDataContainers();


                foreach (var data in RelatedData)
                {
                    if (!data.IsFullData)
                    {
                        //if (!data.KeyProperties.Any())
                        //    throw new Exception("asdad");
                        var resConvert = ConvertDataViewToFullData(RelationshipControl.GenericEditNdTypeArea.AreaInitializer.EntityID, data, RelationshipControl.GenericEditNdTypeArea);
                        if (!resConvert)
                        {
                            //ممکن است اینجا داده وابسته فول شود اما در نمایش فرم بعلت عدم دسترسی به داده  وابسته برای داده وابسته جاری فرم نمایش داده نشود
                            //بنابراین هر فولی به معنی اصلاح شدن داده نیست و باید خصوصیت دیگری در نظر گرفت
                            RelationshipControl.GenericEditNdTypeArea.DataViewGeneric.IsOpenedTemporary = false;

                            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", data.ViewInfo, Temp.InfoColor.Red);
                            return;
                        }
                    }
                }
                foreach (var data in RelatedData)
                {
                    ShowDataInDataView(data);
                }
            }
            DecideButtonsEnablity1();
            var dialogManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow();
            dialogManager.WindowClosed += DialogManager_WindowClosed;
            dialogManager.ShowDialog(RelationshipControl.GenericEditNdTypeArea.DataViewGeneric, RelationshipControl.GenericEditNdTypeArea.SimpleEntity.Alias, Enum_WindowSize.Big);
        }
        private void DialogManager_WindowClosed(object sender, EventArgs e)
        {
            RelationshipControl.GenericEditNdTypeArea.DataViewGeneric.IsOpenedTemporary = false;
            //   SetTempText(GetDataList());
            //   CheckRelationshipLabel();
            //foreach (var item in GetDataList())
            //{
            //    OnDataItemUnShown(new EditAreaDataItemArg() { DataItem = item });
            //}
        }
        private bool ConvertDataViewToFullData(int entityID, DP_FormDataRepository dataITem, I_EditEntityArea editEntityArea)
        {
            //اوکی نشده
            DP_SearchRepositoryMain SearchDataItem = new DP_SearchRepositoryMain(entityID);
            foreach (var col in dataITem.KeyProperties)
            {
                SearchDataItem.Phrases.Add(new SearchProperty() { ColumnID = col.ColumnID, Value = col.Value });
            }
            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();

            // var requestSearchEdit = new DR_SearchEditRequest(requester, SearchDataItem, editEntityArea.AreaInitializer.SecurityReadOnly, false);

            //int toRelationsipID = 0;
            //if (editEntityArea.AreaInitializer.SourceRelationColumnControl != null)
            //    editEntityArea.AreaInitializer.SourceRelationColumnControl
            var requestSearchEdit = new DR_SearchEditRequest(requester, SearchDataItem);
            var foundItem = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchEditRequest(requestSearchEdit).ResultDataItems;
            if (foundItem.Any())
            {
                dataITem.ClearProperties();
                dataITem.SetProperties(foundItem[0].GetProperties());
                dataITem.IsFullData = true;
                dataITem.SetProperties();
                return true;
            }
            else
            {
                return false;
            }
        }
       // public I_View_TemporaryView LastTemporaryView { set; get; }
        public bool DateSecurityIssue { get; private set; }

        public void TemporaryViewActionRequested(I_View_TemporaryView TemporaryView, TemporaryLinkType linkType)
        {
            RelationshipControl.GenericEditNdTypeArea.ChildRelationshipInfoBinded = this;
            //if (LastTemporaryView != null)
            //{
            //    if (LastTemporaryView.HasPopupView)
            //        LastTemporaryView.RemovePopupView(RelationshipControl.GenericEditNdTypeArea.ViewEntityArea.ViewForViewEntityArea);
            //}
         //   LastTemporaryView = TemporaryView;
            if (linkType == TemporaryLinkType.DataView)
            {
                DataViewRequested();
            }
            else if (linkType == TemporaryLinkType.SerachView)
            {
                ShowSearchView(false);
            }
            //else if (linkType == TemporaryLinkType.QuickSearch)
            //{

            //    TemporaryView.QuickSearchVisibility = !TemporaryView.QuickSearchVisibility;
            //    if (TemporaryView.QuickSearchVisibility)
            //        TemporaryView.QuickSearchSelectAll();
            //}
            else if (linkType == TemporaryLinkType.Popup)
            {
                ////if (!TemporaryView.PopupVisibility)
                ////{
                //    RelationshipControl.GenericEditNdTypeArea.ShowTemproraryUIViewArea(TemporaryView);
                ////    if (!TemporaryView.HasPopupView)
                ////        TemporaryView.AddPopupView(RelationshipControl.GenericEditNdTypeArea.ViewEntityArea.ViewForViewEntityArea);
                ////}
                //TemporaryView.PopupVisibility = !TemporaryView.PopupVisibility;

                if (!TemporaryView.PopupVisibility)
                {
                    RelationshipControl.GenericEditNdTypeArea.ShowTemproraryUIViewArea(TemporaryView);
                }
                else
                    TemporaryView.PopupVisibility = false;

            }
            else if (linkType == TemporaryLinkType.Clear)
            {
                RemoveRelatedData();
            }
            else if (linkType == TemporaryLinkType.Info)
            {

                AgentHelper.ShowEditEntityAreaInfo(RelationshipControl.GenericEditNdTypeArea);
            }
        }

        public void ShowSearchView(bool fromDataView)
        {
            //if (LastTemporaryView != null)
            //    LastTemporaryView.RemovePopupView(RelationshipControl.GenericEditNdTypeArea.ViewEntityArea.ViewForViewEntityArea);
            RelationshipControl.GenericEditNdTypeArea.ShowSearchView(fromDataView);
        }
        //List<ColumnControlColorItem> GeneralColumnControlColorItems = new List<ColumnControlColorItem>();
        //List<ColumnControlMessageItem> GeneralColumnControlMessageItems = new List<ColumnControlMessageItem>();

        //public void SetMessageAndColor()
        //{


        //}



        private void SetItemColor(List<ColumnControlColorItem> columnControlColorItems)
        {
            SetItemColor(ControlOrLabelAsTarget.Control, columnControlColorItems);
            SetItemColor(ControlOrLabelAsTarget.Label, columnControlColorItems);
        }
        public void SetItemColor(ControlOrLabelAsTarget controlOrLabel, List<ColumnControlColorItem> columnControlColorItems)
        {

            var colorBackground = GetColor(controlOrLabel, ControlColorTarget.Background, columnControlColorItems);
            var colorForeground = GetColor(controlOrLabel, ControlColorTarget.Foreground, columnControlColorItems);
            var colorBorder = GetColor(controlOrLabel, ControlColorTarget.Border, columnControlColorItems);


            var controlManagers = GetColumnControlDataManagers(controlOrLabel);


            foreach (var controlManager in controlManagers)
            {

                controlManager.SetBackgroundColor(colorBackground);
                controlManager.SetForegroundColor(colorForeground);
                controlManager.SetBorderColor(colorBorder);
            }
        }

        private InfoColor GetColor(ControlOrLabelAsTarget controlOrLabel, ControlColorTarget colorTarget, List<ColumnControlColorItem> columnControlColorItems)
        {
            var color = columnControlColorItems.OrderByDescending(x => x.Priority).FirstOrDefault(x => x.ControlOrLabel == controlOrLabel && x.ColorTarget == colorTarget);
            if (color != null)
                return color.Color;
            else
                return InfoColor.Default;
        }


        private void SetItemMessage(List<ColumnControlMessageItem> columnControlMessageItems)
        {
            SetItemMessage(ControlOrLabelAsTarget.Control, columnControlMessageItems);
            SetItemMessage(ControlOrLabelAsTarget.Label, columnControlMessageItems);
        }
        public void SetItemMessage(ControlOrLabelAsTarget controlOrLabel, List<ColumnControlMessageItem> columnControlMessageItems)
        {
            var tooltip = GetTooltip(controlOrLabel, columnControlMessageItems);
            var controlManagers = GetColumnControlDataManagers(controlOrLabel);
            foreach (var view in controlManagers)
            {
                view.SetTooltip(tooltip);
            }
        }
        private string GetTooltip(ControlOrLabelAsTarget controlOrLabel, List<ColumnControlMessageItem> columnControlMessageItems)
        {
            var tooltip = "";
            foreach (var item in columnControlMessageItems.Where(x => x.ControlOrLabel == controlOrLabel).OrderByDescending(x => x.Priority))
                tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
            return tooltip;
        }






    }
    public enum enum_AddRemove
    {
        Add,
        Remove
    }
    public enum RelationChangedMode
    {
        PkToFkRemoved,
        PkToFkAdded,
        FkToPkRemoved,
        FkToPkAdded,
    }
}
