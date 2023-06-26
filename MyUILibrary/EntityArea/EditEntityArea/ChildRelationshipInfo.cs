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
        public bool LableIsShared { get { return SourceData.EditEntityArea is I_EditEntityAreaMultipleData; } }

        public new DP_FormDataRepository SourceData { get { return base.SourceData as DP_FormDataRepository; } }

        //   public List<ControlStateItem> ControlReadonlyStateItems = new List<ControlStateItem>();
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
        public I_View_TemporaryView GetTempView
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
                            return (RelationshipControl as RelationshipColumnControlOne).GenericEditNdTypeArea.DataView;
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

        public void SetControlUIDetails()
        {

            //** ChildRelationshipInfo.SetControlUIDetails: a579903e12bd

            if (IsHiddenOnState)
            {
                foreach (var item in GetUIControlManager)
                    item.Visiblity(false);
                if (!LableIsShared)
                    RelationshipControl.LabelControlManager.Visiblity(false);

                var message = "رابطه" + " " + RelationshipControl.Relationship.Alias + " " + "برای داده" + " " + SourceData.ViewInfo + " " + "غیر قابل دسترسی می باشد";
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(message, IsHiddenText);
            }
            else
            {

                foreach (var item in GetUIControlManager)
                    item.Visiblity(true);
                if (!LableIsShared)
                    RelationshipControl.LabelControlManager.Visiblity(true);

                List<ColumnControlColorItem> columnControlColorItems = new List<ColumnControlColorItem>();
                List<ColumnControlMessageItem> columnControlMessageItems = new List<ColumnControlMessageItem>();

                if (RelationshipControl.Relationship.IsOtherSideMandatory)
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Label, ControlColorTarget.Foreground, "mandatory", ControlItemPriority.Normal));


                if (IsReadonly)
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "relationReadonly", ControlItemPriority.High));
                    columnControlMessageItems.Add(new ColumnControlMessageItem(IsReadonlyText, ControlOrLabelAsTarget.Control, "readonly", ControlItemPriority.High));
                }

                RelationshipControl.GenericEditNdTypeArea.DecideButtonsEnablity1();
                //if (IsReadonly)
                //{
                //    var key = "";
                //    if (RelationshipControl.Relationship.IsReadonly)
                //    {
                //        key += (key == "" ? "" : ",") + "تعریف رابطه";
                //    }
                //    foreach (var item in ControlReadonlyStateItems)
                //    {
                //        key += (key == "" ? "" : ",") + "فقط خواندنی بودن رابطه" + ":" + item.Message;
                //    }

                //    //if (DataIsOneAndReadonly)
                //    //{
                //    //    foreach (var item in RelatedData)
                //    //    {
                //    //        foreach (var item1 in item.ToParentRelationshipReadonlyStateItems)
                //    //        {
                //    //            key += (key == "" ? "" : ",") + "فقط خواندنی بودن داده" + ":" + item1.Message;
                //    //        }
                //    //    }
                //    //}
                //    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "relationReadonly", ControlItemPriority.Normal));
                //    columnControlMessageItems.Add(new ColumnControlMessageItem("رابطه فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد" + Environment.NewLine + key, ControlOrLabelAsTarget.Control, "relationReadonly", ControlItemPriority.Normal));
                //}
                List<DataColorItem> dataColorItems = new List<DataColorItem>();
                List<DataMessageItem> dataMessageItems = new List<DataMessageItem>();
                List<DP_FormDataRepository> disabledDataItems = new List<DP_FormDataRepository>();

                if (RelatedData.Any(x => x.ParentRelationshipIsHidenInUI))
                {
                    if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                    {
                        columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "parentRelationshipHiddenInUI", ControlItemPriority.Normal));
                        columnControlMessageItems.Add(new ColumnControlMessageItem(RelatedData.First(x => x.ParentRelationshipIsHidenOnLoad).ParentRelationshipIsHidenOnLoadText, ControlOrLabelAsTarget.Control, "parentRelationshipHiddenInUI", ControlItemPriority.Normal));
                    }
                    else
                    {
                        if (IsDataviewOpen)
                        {
                            foreach (var data in RelatedData.Where(x => x.ParentRelationshipIsHidenInUI))
                            {
                                dataColorItems.Add(new DataColorItem(data, InfoColor.DarkRed, data.GUID.ToString(), ControlItemPriority.Normal));
                                dataMessageItems.Add(new DataMessageItem(data, data.ParentRelationshipIsHidenInUIText, data.GUID.ToString(), ControlItemPriority.Normal));
                            }
                        }
                    }
                }


                bool enableDisableView = true;
                if (RelatedData.Any(x => x.ParentRelationshipIsHidenOnLoad))
                {
                    if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                    {
                        enableDisableView = false;
                        columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "parentRelationshipHiddenOnLoad", ControlItemPriority.Normal));
                        columnControlMessageItems.Add(new ColumnControlMessageItem(RelatedData.First(x => x.ParentRelationshipIsHidenOnLoad).ParentRelationshipIsHidenOnLoadText, ControlOrLabelAsTarget.Control, "parentRelationshipHiddenOnLoad", ControlItemPriority.Normal));
                    }
                    else
                    {
                        if (IsDataviewOpen)
                        {
                            foreach (var data in RelatedData.Where(x => x.ParentRelationshipIsHidenOnLoad))
                            {
                                dataMessageItems.Add(new DataMessageItem(data, data.ParentRelationshipIsHidenOnLoadText, data.GUID.ToString(), ControlItemPriority.Normal));
                                disabledDataItems.Add(data);
                            }
                        }
                    }
                }

                (GetMainUIControlManager as I_View_Area).EnableDisable(enableDisableView);



                bool enableDisableDataSection = true;
                if (RelatedData.Any(x => x.IsUseLessBecauseNewAndReadonly))
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "isUseLessBecauseNewAndReadonly", ControlItemPriority.Normal));
                    columnControlMessageItems.Add(new ColumnControlMessageItem("به علت فقط خواندنی بودن موجودیت، از داده های جدید صرف نظر خواهد شد", ControlOrLabelAsTarget.Control, "isUseLessBecauseNewAndReadonly", ControlItemPriority.Normal));
                    if (IsDataviewOpen)
                    {
                        enableDisableDataSection = false;
                    }
                }
                if (IsDataviewOpen)
                    RelationshipControl.GenericEditNdTypeArea.DataView.EnableDisableDataSection(enableDisableDataSection);




                SetItemColor(columnControlColorItems);
                SetItemMessage(columnControlMessageItems);

                SetDataItemsColor(dataColorItems);
                SetDataItemsMessage(dataMessageItems);
                SetDataItemsEnablity(disabledDataItems);

            }
        }

        private void SetDataItemsEnablity(List<DP_FormDataRepository> disabledDataItems)
        {
            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaMultipleData)
            {
                if (IsDataviewOpen)
                {
                    foreach (var data in RelatedData)
                    {
                        (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.EnableDisable(data, !disabledDataItems.Any(x => x == data));
                    }
                }
            }
        }
        private void SetDataItemsColor(List<DataColorItem> dataColorItems)
        {
            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaMultipleData)
            {
                if (IsDataviewOpen)
                {
                    foreach (var data in RelatedData)
                    {
                        var colorItems = dataColorItems.Where(x => x.DataItem == data).ToList();
                        var colorItem = colorItems.OrderByDescending(x => x.Priority).FirstOrDefault();
                        Temp.InfoColor color;
                        if (colorItem != null)
                            color = colorItem.Color;
                        else
                            color = InfoColor.Default;
                        (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.SetColor(data, color);
                    }
                }
            }
        }
        private void SetDataItemsMessage(List<DataMessageItem> dataMessageItemItems)
        {
            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaMultipleData)
            {
                if (IsDataviewOpen)
                {
                    foreach (var data in RelatedData)
                    {
                        var tooltip = "";
                        var messageItems = dataMessageItemItems.Where(x => x.DataItem == data).ToList();
                        foreach (var item in messageItems.OrderByDescending(x => x.Priority))
                            tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
                        (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.SetTooltip(data, tooltip);
                    }
                }
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
        public void AddDataToChildRelationshipData(DP_FormDataRepository dataItem)
        {
            //ChildRelationshipInfo.AddDataToChildRelationshipData: 6e846e90a6ff
            if (IsReadonly && dataItem.IsNewItem && (!dataItem.IsDefaultData || !IsDataviewOpen))
                throw new Exception();
            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData && RelatedData.Count > 0)
                throw new Exception("sdfsdfsdfsdf");

            //    dataItem.ToParentRelationshipID = Relationship.PairRelationshipID;
            //if (!dataItem.IsNewItem)
            //{
            //    //عملا میشه داده ای که یا انتخاب شده از سلکت ویو یا از رابطه اصلی اومده
            //    dataItem.EditEntityArea.AreaInitializer.ActionActivityManager.SetExistingDataFirstLoadStates(dataItem);

            //این تیکه اضافیه؟؟
            //if (!dataItem.IsDBRelationship)
            //{
            //    //عملا میشه داده ای که انتخاب شده از سلکت ویو
            //    if (dataItem.ToParentRelationshipIsHidden)
            //    {
            //        var key = "";
            //        foreach (var item in dataItem.ToParentRelationshipHiddenStateItems)
            //        {
            //            key += (key == "" ? "" : ",") + "غیر قابل دسترسی بودن رابطه" + ":" + item.Message;
            //        }
            //        var message = "رابطه" + " " + RelationshipControl.Relationship.Alias + " " + "برای داده" + " " + dataItem.ViewInfo + " " + "غیر قابل دسترسی می باشد";
            //        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(message, key);
            //        return;
            //    }
            //    else
            //    {
            //        if (dataItem.ToParentRelationshipIsReadonly)
            //        {
            //            var key = "";
            //            foreach (var item in dataItem.ToParentRelationshipReadonlyStateItems)
            //            {
            //                key += (key == "" ? "" : ",") + "فقط خواندنی بودن رابطه" + ":" + item.Message;
            //            }
            //            var message = "رابطه" + " " + RelationshipControl.Relationship.Alias + " " + "برای داده" + " " + dataItem.ViewInfo + " " + "غیر قابل دسترسی می باشد";
            //            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(message, key);
            //            return;
            //        }
            //    }
            //}
            //}
            dataItem.ParantChildRelationshipData = this;


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
                ShowDataInChildRelationshipDataView(dataItem);
            }

            if (!IsDirect)
            {
                SetTempTextChildRelationshipData();
            }
            foreach (var changeMonitorItem in GetChangeMonitorItemsFromSourceData())
            {
                dataItem.AddChangeMonitorIfNotExists(changeMonitorItem);
            }
            dataItem.EditEntityArea.AreaInitializer.ActionActivityManager.DataLoaded(dataItem);


            //CheckDataParentRelationship(dataItem);

            SetControlUIDetails();
        }
        public List<ChangeMonitorItem> GetChangeMonitorItemsFromSourceData()
        {
            List<ChangeMonitorItem> result = new List<ChangeMonitorItem>();
            foreach (var changeMonitorItem in SourceData.ChangeMonitorItems.Where(x => !string.IsNullOrEmpty(x.RelTailAndColumns.Item1)))
            {
                string firstRel = "", Rest = "";
                AgentHelper.SplitRelationshipTail(changeMonitorItem.RelTailAndColumns.Item1, ref firstRel, ref Rest);
                if (RelationshipControl.Relationship.ID.ToString() == firstRel)
                {
                    var newrelTailAndColumns = new Tuple<string, List<int>>(Rest, changeMonitorItem.RelTailAndColumns.Item2);
                    result.Add(new ChangeMonitorItem(changeMonitorItem.ChangeMonitorSource, changeMonitorItem.UsageKey, newrelTailAndColumns, changeMonitorItem.DataToCall));
                }
            }
            return result;
        }
        private void ShowDataInChildRelationshipDataView(DP_FormDataRepository dataItem)
        {
            //ChildRelationshipInfo.ShowDataInChildRelationshipDataView: 78184401047a
            RelationshipControl.GenericEditNdTypeArea.ShowDataInDataView(dataItem);



            //if (ChangeMonitorItems.Any(x => x.columnID == 0))
            //{


            //if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaMultipleData)
            //{
            //    if (dataItem.ToParentRelationshipIsReadonly)
            //    {
            //        var tooltip = "رابطه داده به علت فقط خواندنی بودن غیر قابل تغییر است";
            //        foreach (var item in dataItem.ToParentRelationshipReadonlyStateItems)
            //            tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
            //        (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.SetTooltip(dataItem, tooltip);
            //    }
            //    else if (dataItem.ToParentRelationshipIsHidden)
            //    {
            //        var tooltip = "داده به علت عدم دسترسی به رابطه غیرفعال است";
            //        foreach (var item in dataItem.ToParentRelationshipHiddenStateItems)
            //            tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
            //        (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.SetTooltip(dataItem, tooltip);
            //        (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).DataView.EnableDisable(dataItem, false);
            //    }
            //}
        }
        //////internal void AddChangeMonitorIfNotExists(ChangeMonitorItem changeMonitorItem)
        //////{
        //////    if (ChangeMonitorItems.Any(x => x.UsageKey == changeMonitorItem.UsageKey && x.DataToCall == changeMonitorItem.DataToCall))// x.RestTail == restTail && x.columnID == columnID && x.DataToCall == dataToCall))
        //////        return;
        //////    ChangeMonitorItems.Add(changeMonitorItem);
        //////    foreach (var data in RelatedData)
        //////    {
        //////        data.AddChangeMonitorIfNotExists(changeMonitorItem);
        //////    }
        //////}
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

        public void SetTempTextChildRelationshipData()
        {
            //ChildRelationshipInfo.SetTempTextChildRelationshipData: 2da7f695849f
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
            // ChildRelationshipInfo.IsDataviewOpen: fb8b8d086bbe
            get
            {
                return IsDirect ||
                                (RelationshipControl.GenericEditNdTypeArea.DataView != null &&
                                RelationshipControl.GenericEditNdTypeArea.DataView.IsOpenedTemporary);
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

        internal void RemoveRelatedMultipleData(List<DP_FormDataRepository> datas)
        {
            // ChildRelationshipInfo.RemoveRelatedMultipleData: 3757aab2e26e
            foreach (var item in datas)
                RemoveRelatedData(item);
        }
        public bool RemoveAllRelatedData()
        {
            //ChildRelationshipInfo.RemoveRelatedData: d5d649262e21
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


        public bool RemoveRelatedData(DP_FormDataRepository DP_FormDataRepository)
        {
            //ChildRelationshipInfo.RemoveRelatedData: 938077b01703



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
                    SetTempTextChildRelationshipData();
                }
                if (IsDataviewOpen)
                {
                    RelationshipControl.GenericEditNdTypeArea.ClearUIData(DP_FormDataRepository);

                    if (RelatedData.Count == 0 && RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                    {
                        CreateDefaultDataChildRelationship();
                    }
                }
                SetControlUIDetails();
                return true;
            }
            else
                return false;
        }

        private void CheckRelationshipChanged(enum_AddRemove mode, DP_FormDataRepository dataItem)
        {
            //ChildRelationshipInfo.CheckRelationshipChanged: e7c6d51dcb3d
            foreach (var item in GetChangeMonitorItemsFromSourceData())
            {
                item.ChangeMonitorSource.DataPropertyRelationshipChanged(item.DataToCall, item.UsageKey);
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
        public bool CheckRemoveData(DP_FormDataRepository data)
        {
            bool clearIsOk = true;
            if (data.IsDBRelationship)
            {
                //برای روابط پرایمری به فارن که وضعیت اعمال میشه
                //if (data.ToParantChildRelationshipInfo.IsHidden)// || data.IsReadonlyOnState)
                //    return false;

                //اینجا باید وضعیت داده و رابطه با پرنتش چک بشه ریدونلی و مخفی***

                //if (data.ToParentRelationshipIsHidden || data.ToParentRelationshipIsReadonly)
                //    return false;
            }
            bool shouldDeleteFromDB = false;
            //   var existingdatas = datas.Where(x => x.IsDBRelationship);
            ///   if (existingdatas.Count() != 0)
            //   {


            if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
            {
                if (RelationshipControl.GenericEditNdTypeArea.SourceRelationColumnControl.Relationship.DBDeleteRule == RelationshipDeleteUpdateRule.Cascade)//|| RelationshipControl.GenericEditNdTypeArea.SourceRelationColumnControl.Relationship.RelationshipColumns.Any(x => !x.SecondSideColumn.IsNull))
                {
                    shouldDeleteFromDB = true;
                }
            }
            //بعدا به این فکر شود
            //var relationship = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipManager.GetRelationship(SourceRelationColumnControl.Relationship.PairRelationshipID);
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
                        //if (item.ParentRelationshipIsHidden || this.IsReadonlyOnState)// || item.IsReadonlyOnState)
                        //{
                        //    if (!DataItemIsAdded(item))
                        //        result.Add(item);
                        //}
                        //else
                        result.Add(item);
                    }
                }
                foreach (var item in RemovedOriginalDatas)
                {
                    //if (item.ParentRelationshipIsHidden || this.IsReadonlyOnState)// || item.IsReadonlyOnState)
                    //{
                    //    result.Add(item);
                    //}
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

                //** ChildRelationshipInfo.IsReadonly: e276ef44e0fb


                return Relationship.IsReadonly || IsReadonlyOfState;//|| DataIsOneAndReadonly;


                //|| (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                //              (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
            }
        }
        public bool IsReadonlyOfState
        {
            get { return SourceData.ChildReadonlyRelationships.Any(x => x.Item1 == Relationship.ID); }
        }
        public string IsReadonlyText
        {
            get
            {
                string text = "";
                if (Relationship.IsReadonly)
                    text += "دسترسی به رابطه فقط خواندنی است";

                if (IsReadonlyOfState)
                {
                    text += (string.IsNullOrEmpty(text) ? "" : Environment.NewLine)
                              + "بر اساس وضعیتهای زیر دسترسی به ستون فقط خواندنی است";
                    foreach (var item in SourceData.ChildReadonlyRelationships.Where(x => x.Item1 == Relationship.ID))
                    {
                        text += Environment.NewLine + item.Item2;
                    }
                }
                return text;

                //|| (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                //              (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
            }
        }
        //public bool DataIsOneAndReadonly
        //{
        //    get
        //    {
        //        if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
        //        {
        //            if (RelatedData.Any() && RelatedData.All(x => x.IsDBRelationship && x.ToParentRelationshipReadonlyStateItems.Any()))
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}
        public bool IsHiddenOnState
        {
            get
            {
                return ControlHiddenStateItems.Any();//|| DataIsOneAndHidden;
            }
        }
        public string IsHiddenText
        {
            get
            {
                string text = "";
                foreach (var item in ControlHiddenStateItems)
                {
                    text += (text == "" ? "" : ",") + "غیر قابل دسترسی بودن رابطه" + ":" + item.Message;
                }
                //if (DataIsOneAndHidden)
                //{

                //    foreach (var item in RelatedData)
                //    {
                //        foreach (var item1 in item.ParentRelationshipTitle)
                //        {
                //            text += (text == "" ? "" : ",") + " وضعیت " + item1;
                //        }
                //    }
                //}
                return text;
            }
        }



        //public bool DataIsOneAndHidden
        //{
        //    get
        //    {
        //        if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
        //        {
        //            if (RelatedData.Any() && RelatedData.All(x => x.IsDBRelationship && x.ToParentRelationshipHiddenStateItems.Any()))
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}


        //public bool IsDirectOneEmptyAndEntityIsReadonly
        //{
        //    get
        //    {
        //        if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData && IsDirect)
        //        {
        //            if (dataLoaded && !RelatedData.Any() && RelationshipControl.GenericEditNdTypeArea.DataEntryEntity.IsReadonly)
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}
        //public bool IsReadonlyOnState
        //{
        //    get
        //    {
        //        return ControlReadonlyStateItems.Any();
        //    }
        //}
        //این باید خود رابطه را چک کنه همچنین اینکه رابطه کریتورش فقط خواندنی هست یا نه؟ خود موجودیت طرفین هم فکر بشه

        //  List<ChangeMonitorItem> ChangeMonitorItems = new List<ChangeMonitorItem>();

        //اینجا وظیفه چک کردن هم داده ها و هم ستونهای داده را دارد
        //internal void AddChangeMonitor(ChangeMonitorItem changeMonitorItem)
        //{


        //    //foreach (var relatedData in RelatedData)
        //    //{
        //    //    relatedData.AddChangeMonitor(generalKey, usageKey, restTail, columnID, dataToCall);
        //    //}
        //    //if (!string.IsNullOrEmpty(restTail))
        //    //{


        //    //}
        //}



        //public void RemoveChangeMonitorByGenaralKey(string key)
        //{
        //    foreach (var item in ChangeMonitorItems.Where(x => x.GeneralKey == key).ToList())
        //    {
        //        ChangeMonitorItems.Remove(item);
        //        foreach (var data in RelatedData)
        //        {
        //            data.RemoveChangeMonitorByGenaralKey(key);
        //        }
        //    }
        //}

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


        public void SetBinding()
        {
            // ChildRelationshipInfo.SetBinding: b0d3af993f9c
            //اینکه اینجا هیدن ها بایند نمیشن خوب نیست چون برای ارث بری مثلا شخص فرمهای شخص حقیقی و حقوقی مخفی هستن اولش و بایند نمیشن. بعدا یه فکری بشه
            RelationshipControl.GenericEditNdTypeArea.ChildRelationshipInfoBinded = this;
            if (!IsHiddenOnState)
            {
                //if (!DateSecurityIssue)
                //{
                if (IsDirect)// IsDataviewOpen)
                {
                    ClearUIData();
                }
                else //if (!IsDirect)
                    GetTempView.SetLinkText("");
                if (dataLoaded)
                {
                    if (IsDirect)//IsDataviewOpen)
                    {
                        foreach (var item in RelatedData)
                        {
                            ShowDataInChildRelationshipDataView(item);
                        }
                    }
                    else
                    {
                        SetTempTextChildRelationshipData();
                    }
                    foreach (var dataItem in RelatedData)
                    {
                        dataItem.EditEntityArea.AreaInitializer.ActionActivityManager.DataLoaded(dataItem);
                    }
                }
                else
                {
                    dataLoaded = true;
                    bool relationshipFirstSideHasValue = RelationshipControl.Relationship.RelationshipColumns.Any()
                        && RelationshipControl.Relationship.RelationshipColumns.All(x => SourceData.GetProperties().Any(y => !y.ValueIsEmpty() && y.ColumnID == x.FirstSideColumnID));
                    if (!relationshipFirstSideHasValue)
                    {

                    }
                    else
                    {
                        List<DP_FormDataRepository> dbSearch = null;
                        if (IsDirect)//IsDataviewOpen)
                            dbSearch = SerachDataFromParentRelationForChildDataView();
                        else
                            dbSearch = SerachDataFromParentRelationForChildTempView();
                        //if (dbSearch.Item1)
                        //{
                        //    DateSecurityIssue = true;
                        //    CheckRelationshipUI();
                        //    //  return false;
                        //}
                        //else
                        //{
                        foreach (var item in dbSearch)
                            AddDataToChildRelationshipData(item);
                        //}
                    }
                }
                //if (IsDataviewOpen && RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                if (IsDirect && RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                {
                    if (!RelatedData.Any())
                    {
                        CreateDefaultDataChildRelationship();
                    }
                }
                if (RelationshipControl is RelationshipColumnControlMultiple)
                {
                    GetTempView.TemporaryDisplayViewRequested += GetTempView_TemporaryDisplayViewRequested;
                }

                //   return true;
            }

            SetControlUIDetails();
            //  return false;

        }

        private void ClearUIData()
        {
            //ChildRelationshipInfo.ClearUIData: a9614860d724
            if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
            {

            }
            else if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaMultipleData)
            {
                (RelationshipControl.GenericEditNdTypeArea as I_EditEntityAreaMultipleData).RemoveDataContainers();
            }

        }

        private void CreateDefaultDataChildRelationship()
        {
            // ChildRelationshipInfo.CreateDefaultDataChildRelationship: db3903adaa4d
            var newData = AgentHelper.CreateAreaInitializerNewData(RelationshipControl.GenericEditNdTypeArea, true);
            //newData.IsDefaultData = true;
            //if (DataEntryEntity.IsReadonly || IsReadonly)
            //{

            //////foreach (var property in newData.ChildSimpleContorlProperties)
            //////{
            //////    property.AddReadonlyState("", "DataNewAndReadonly", true);
            //////}
            //////foreach (var rel in newData.ChildRelationshipDatas)
            //////{
            //////    rel.AddReadonlyState("", "DataNewAndReadonly", true);
            //////}
            //}
            AddDataToChildRelationshipData(newData);
        }
        private void GetTempView_TemporaryDisplayViewRequested(object sender, Arg_TemporaryDisplayViewRequested e)
        {
            TemporaryViewActionRequested(sender as I_View_TemporaryView, e.LinkType);
        }

        RelationshipDataManager relationshipManager = new RelationshipDataManager();

        private List<DP_FormDataRepository> SerachDataFromParentRelationForChildTempView()
        {
            //ChildRelationshipInfo.SerachDataFromParentRelationForChildTempView: 0f1244a3531e
            var searchDataItem = relationshipManager.GetSecondSideSearchItemByFirstSideColumns(SourceData, RelationshipControl.Relationship);

            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
            DR_SearchViewRequest request = new DR_SearchViewRequest(requester, searchDataItem);
            request.EntityViewID = RelationshipControl.GenericEditNdTypeArea.ViewEntityArea.EntityListView.ID;
            var childViewData = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(request).ResultDataItems;
            var countRequest = new DR_SearchCountRequest(requester);
            request.ToParentRelationshipID = Relationship.ID;
            //    request.CheckEntityStates = true;
            //  request.ToParentRelationshipIsFKToPK = ToParentRelationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary;
            countRequest.SearchDataItems = searchDataItem;
            countRequest.Requester.SkipSecurity = true;
            var count = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchCountRequest(countRequest);
            //if (count.ResultCount != childViewData.Count)
            //    return new Tuple<bool, List<DP_FormDataRepository>>(true, null);
            List<DP_FormDataRepository> list = new List<DP_FormDataRepository>();
            // EditAreaDataManager EditAreaDataManager = new EditAreaDataManager();
            foreach (var item in childViewData)
            {
                var dpItem = new DP_FormDataRepository(item, RelationshipControl.GenericEditNdTypeArea, true, false);
                list.Add(dpItem);
            }
            return list;
        }


        public List<DP_FormDataRepository> SerachDataFromParentRelationForChildDataView()
        {
            //ChildRelationshipInfo.SerachDataFromParentRelationForChildDataView: 847beee04f15
            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
            var searchDataItem = relationshipManager.GetSecondSideSearchItemByFirstSideColumns(SourceData, Relationship);
            DR_SearchEditRequest request = new DR_SearchEditRequest(requester, searchDataItem);
            request.ToParentRelationshipID = ToParentRelationship.ID;
            // request.ToParentRelationshipIsFKToPK = ToParentRelationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary;
            var childFullData = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchEditRequest(request).ResultDataItems;
            var countRequest = new DR_SearchCountRequest(requester);
            countRequest.SearchDataItems = searchDataItem;
            countRequest.Requester.SkipSecurity = true;
            var count = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchCountRequest(countRequest);
            //if (count.ResultCount != childFullData.Count)
            //{
            //    return new Tuple<bool, List<DP_FormDataRepository>>(true, null);
            //}
            List<DP_FormDataRepository> list = new List<DP_FormDataRepository>();
            //EditAreaDataManager EditAreaDataManager = new EditAreaDataManager();
            foreach (var data in childFullData)
            {
                //  data.DataView = EditAreaDataManager.GetDataView(data);
                DP_FormDataRepository formData = new DP_FormDataRepository(data, RelationshipControl.GenericEditNdTypeArea, true, false);
                list.Add(formData);
            }
            return list;
        }


        //public void AddReadonlyState(string key, string message, bool permanent)
        //{
        //    if (ControlReadonlyStateItems.Any(x => x.Key == key))
        //        ControlReadonlyStateItems.Remove(ControlReadonlyStateItems.First(x => x.Key == key));
        //    ControlReadonlyStateItems.Add(new ControlStateItem(key, message, permanent));

        //    //if (checkInUI)
        //    //{
        //    //    CheckColumnReadonly();
        //    //    SetMessageAndColor();
        //    //}
        //}



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
            //ChildRelationshipInfo.AddHiddenState: c77deede58e4
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.Remove(ControlHiddenStateItems.First(x => x.Key == key));
            ControlHiddenStateItems.Add(new ControlStateItem(key, message, permanent));


            //   DecideVisiblity();
            //SetMessageAndColor();
            SetControlUIDetails();

        }
        public void RemoveHiddenState(string key)
        {
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.RemoveAll(x => x.Key == key);

            SetControlUIDetails();
            //  DecideVisiblity();
            //  SetMessageAndColor();

        }

        //private void DecideVisiblity()
        //{

        //}




        //public void SelectFromParent(Dictionary<int, object> colAndValues)
        //{

        //}

        //internal void DataSelected(DP_FormDataRepository result)
        //{

        //}

        internal void DataViewRequested()
        {
            // ChildRelationshipInfo.DataViewRequested: e48af9744b57
            if (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.FormComposed == false)
            {
                RelationshipControl.GenericEditNdTypeArea.GenerateDataView();
            }
            RelationshipControl.GenericEditNdTypeArea.DataView.IsOpenedTemporary = true;

            ClearUIData();
            if (RelatedData.Count == 0)
            {
                if (RelationshipControl.GenericEditNdTypeArea is I_EditEntityAreaOneData)
                    CreateDefaultDataChildRelationship();
            }
            else
            {
                if (RelatedData.Any(x => !x.IsFullData))
                    ConvertDataViewToFullDataAndShow(RelationshipControl.GenericEditNdTypeArea.AreaInitializer.EntityID, RelatedData, RelationshipControl.GenericEditNdTypeArea);

                //foreach (var data in RelatedData)
                //{
                //    if (!data.IsFullData)
                //    {
                //        //var resConvert = ConvertDataViewToFullData(RelationshipControl.GenericEditNdTypeArea.AreaInitializer.EntityID, data, RelationshipControl.GenericEditNdTypeArea);
                //        //if (!resConvert)
                //        //{
                //        //    //ممکن است اینجا داده وابسته فول شود اما در نمایش فرم بعلت عدم دسترسی به داده  وابسته برای داده وابسته جاری فرم نمایش داده نشود
                //        //    //بنابراین هر فولی به معنی اصلاح شدن داده نیست و باید خصوصیت دیگری در نظر گرفت
                //        //    RelationshipControl.GenericEditNdTypeArea.DataView.IsOpenedTemporary = false;

                //        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", data.ViewInfo, Temp.InfoColor.Red);
                //        //    return;
                //        //}
                //    }
                //}

            }
            SetControlUIDetails();
            var dialogManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow();
            dialogManager.WindowClosed += DialogManager_WindowClosed;
            dialogManager.ShowDialog(RelationshipControl.GenericEditNdTypeArea.DataView, RelationshipControl.GenericEditNdTypeArea.SimpleEntity.Alias, Enum_WindowSize.Big);
        }
        private void DialogManager_WindowClosed(object sender, EventArgs e)
        {
            RelationshipControl.GenericEditNdTypeArea.DataView.IsOpenedTemporary = false;
            //   SetTempText(GetDataList());
            //   CheckRelationshipLabel();
            //foreach (var item in GetDataList())
            //{
            //    OnDataItemUnShown(new List<DP_FormDataRepository>() { DataItem = item });
            //}
        }
        private void ConvertDataViewToFullDataAndShow(int entityID, List<DP_FormDataRepository> dataItems, I_EditEntityArea editEntityArea)
        {
            // ChildRelationshipInfo.ConvertDataViewToFullDataAndShow: 28ad76d5595c
            //چندتایی شود مثل selectdata
            //اوکی نشده
            //DP_SearchRepositoryMain SearchDataItem = new DP_SearchRepositoryMain(entityID);
            //foreach (var col in dataITem.KeyProperties)
            //{
            //    SearchDataItem.Phrases.Add(new SearchProperty(col.Column) { Value = col.Value });
            //}

            DP_SearchRepositoryMain searchItems = new DP_SearchRepositoryMain(entityID);
            searchItems.AndOrType = AndOREqualType.Or;
            foreach (var item in dataItems)
            {
                LogicPhraseDTO logic = new LogicPhraseDTO();
                foreach (var col in item.Properties.Where(x => x.IsKey))
                    logic.Phrases.Add(new SearchProperty(col.ColumnID) { Value = col.Value });
                searchItems.Phrases.Add(logic);
            }

            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();

            // var requestSearchEdit = new DR_SearchEditRequest(requester, SearchDataItem, editEntityArea.AreaInitializer.SecurityReadOnly, false);

            //int toRelationsipID = 0;
            //if (editEntityArea.SourceRelationColumnControl != null)
            //    editEntityArea.SourceRelationColumnControl
            var requestSearchEdit = new DR_SearchEditRequest(requester, searchItems);
            //    requestSearchEdit.ParentRelationshipID = Relationship.ID;
            var foundItem = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchEditRequest(requestSearchEdit).ResultDataItems;
            foreach (var item in dataItems)
            {
                var fItem = foundItem.First(x => AgentHelper.DataItemsAreEqual(x, item));
                item.ClearProperties();
                item.SetProperties(fItem.GetProperties());
                item.IsFullData = true;
                item.SetProperties();
                ShowDataInChildRelationshipDataView(item);
                foreach (var changeMonitor in item.ChangeMonitorItems)
                    item.SetChangeMonitor(changeMonitor);
                item.EditEntityArea.AreaInitializer.ActionActivityManager.DataLoaded(item);

            }


            //if (foundItem.Any())
            //{
            //dataITem.ClearProperties();
            //dataITem.SetProperties(foundItem[0].GetProperties());
            //dataITem.IsFullData = true;
            //dataITem.SetProperties();
            //return true;


            //}
            //else
            //{
            //    return false;
            //}
        }
        // public I_View_TemporaryView LastTemporaryView { set; get; }
        // public bool DateSecurityIssue { get; private set; }

        public void TemporaryViewActionRequested(I_View_TemporaryView TemporaryView, TemporaryLinkType linkType)
        {
            //** ChildRelationshipInfo.TemporaryViewActionRequested: 40b8056e73af
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
                RelationshipControl.GenericEditNdTypeArea.ShowSearchView(false);
                //ShowSearchView(false);
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
                RemoveAllRelatedData();
            }
            else if (linkType == TemporaryLinkType.Info)
            {

                AgentHelper.ShowEditEntityAreaInfo(RelationshipControl.GenericEditNdTypeArea);
            }
        }

        //public void ShowSearchView(bool fromDataView)
        //{
        //    //if (LastTemporaryView != null)
        //    //    LastTemporaryView.RemovePopupView(RelationshipControl.GenericEditNdTypeArea.ViewEntityArea.ViewForViewEntityArea);

        //}
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
