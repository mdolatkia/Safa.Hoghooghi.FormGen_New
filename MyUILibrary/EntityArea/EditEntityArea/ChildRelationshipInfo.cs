using CommonDefinitions.UISettings;
using ModelEntites;
using MyRelationshipDataManager;
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
    public class ChildRelationshipInfo : BaseChildProperty
    {
        public RelationshipColumnControlGeneral RelationshipControl { get { return BaseColumnControl as RelationshipColumnControlGeneral; } }
        //     public event EventHandler<System.Collections.Specialized.NotifyCollectionChangedEventArgs> CollectionChanged;
        public ChildRelationshipInfo(RelationshipColumnControlGeneral relationshipControl, DP_FormDataRepository sourceData) : base(sourceData, relationshipControl)
        {
            SourceData = sourceData;
            // RelationshipControl = relationshipControl;
            RelatedData = new ObservableCollection<DP_FormDataRepository>();
            //   HiddenData = new ObservableCollection<DP_FormDataRepository>();
            RelatedData.CollectionChanged += RelatedData_CollectionChanged;
            OriginalRelatedData = new ObservableCollection<DP_FormDataRepository>();
            RemovedDataForUpdate = new ObservableCollection<DP_FormDataRepository>();
            //RemovedItems = new List<ProxyLibrary.DP_FormDataRepository>();
            ReadonlyStateFromTails = new List<string>();
            // this.IsReadonlyChanged += ChildRelationshipInfo_IsReadonlyChanged;



        }
        I_View_TemporaryView GetTempView
        {
            get
            {

                return GetUIControlManager as I_View_TemporaryView;


                //else if (RelationshipControl is RelationshipColumnControlMultiple)
                //    return (RelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(SourceData);
                //  return null;
            }

        }
        I_UIElementManager GetUIControlManager
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

        public override List<I_UIElementManager> GetColumnControlDataManagers(ControlOrLabelAsTarget controlOrLabelAsTarget)
        {
            List<I_UIElementManager> result = new List<I_UIElementManager>();


            if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
            {
                result.Add(GetUIControlManager);
            }
            else
            {
                result.Add(RelationshipControl.LabelControlManager);
            }
            return result;
        }


        //private void ChildRelationshipInfo_IsReadonlyChanged(object sender, EventArgs e)
        //{
        //    if (SourceData.DataIsInEditMode())
        //    {
        //        RelationshipControl.EditNdTypeArea.SetChildRelationshipInfo(this);
        //        RelationshipControl.EditNdTypeArea.DecideButtonsEnablity1();
        //    }
        //}

        private void RelatedData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //    if (CollectionChanged != null)
            //      CollectionChanged(sender, e);


            List<Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>> changeFkItemProperties = new List<Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>>();
            //       DataCollectionChanged(ChildRelationshipInfo.RelatedData, e);
            //        DecideButtonsEnablity1();


            if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
            {
                List<UIColumnValueDTO> uIColumnValue = new List<UIColumnValueDTO>();
                var fkDataItem = SourceData;
                foreach (var col in Relationship.RelationshipColumns)
                {
                    var fkProp = fkDataItem.GetProperty(col.FirstSideColumnID);
                    if (fkProp != null)
                    {
                        if (RelatedData.Any())
                        {
                            var pkProp = RelatedData.First().GetProperty(col.SecondSideColumnID);
                            if (pkProp != null)
                            {
                                uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = fkProp.ColumnID, ExactValue = pkProp.Value.ToString(), EvenHasValue = true, EvenIsNotNew = true });
                            }
                        }
                        else
                            uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = fkProp.ColumnID, ExactValue = null, EvenHasValue = true, EvenIsNotNew = true });
                    }
                }
                changeFkItemProperties.Add(new Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>(fkDataItem, uIColumnValue));
            }
            else if (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
            {
                //اینجا بدیش اینه که همه موارد رو مجددا مقداردهی مینکه و نه فقط اون رکوردی که اضافه شده رو
                foreach (var fkDataItem in RelatedData)
                {
                    List<UIColumnValueDTO> uIColumnValue = new List<UIColumnValueDTO>();
                    foreach (var col in Relationship.RelationshipColumns)
                    {
                        var pkProp = SourceData.GetProperty(col.FirstSideColumnID);
                        if (pkProp != null)
                        {
                            var fkProp = fkDataItem.GetProperty(col.SecondSideColumnID);
                            if (fkProp != null)
                            {
                                uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = fkProp.ColumnID, ExactValue = pkProp.Value.ToString(), EvenHasValue = true, EvenIsNotNew = true });
                            }
                        }
                    }
                    changeFkItemProperties.Add(new Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>(fkDataItem, uIColumnValue));
                }
            }

            foreach (var item in changeFkItemProperties)
            {
                if (item.Item2.Any())
                {
                    item.Item1.SetColumnValueFromState(item.Item2, null, null, true);
                }
            }


            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (DP_FormDataRepository item in e.NewItems)
                {
                    //    item.PropertyValueChanged += Item_PropertyValueChanged;
                    foreach (var changeMonitor in ChangeMonitorItems)//.Where(x => !string.IsNullOrEmpty(x.RestTail)))
                    {
                        item.AddChangeMonitor(changeMonitor.GeneralKey, changeMonitor.UsageKey, changeMonitor.RestTail, changeMonitor.columnID, changeMonitor.DataToCall);
                    }
                }

            }

            //if (ChangeMonitorItems.Any(x => x.columnID == 0))
            //{
            foreach (var item in ChangeMonitorItems)//.Where(x => x.columnID == 0))
            {
                item.DataToCall.OnRelatedDataOrColumnChanged(item);
            }

            //}


        }



        //    RelationshipDeleteOption RelationshipDeleteOption { set; get; }
        public RelationshipDeleteOption RelationshipDeleteOption { set; get; }

        //public int SourceRelationID;
        //public int SourceEntityID;
        //public int SourceTableID;
        //public int TargetTableID;
        //public Enum_RelationshipType SourceToTargetRelationshipType;
        //public Enum_MasterRelationshipType SourceToTargetMasterRelationshipType;
        //public void AddDataToChildRelationshipInfo(DP_DataRepository dataItem, bool fromDB)
        //{

        //}
        public void AddDataToChildRelationshipInfo(DP_FormDataRepository dataItem, bool show)
        {
            dataItem.EditEntityArea.AreaInitializer.ActionActivityManager.DataAdded(dataItem);
            var parentRelationshipInfo = new ParentRelationshipInfo(this);
            dataItem.ParantChildRelationshipInfo = parentRelationshipInfo;
            RelatedData.Add(dataItem);
            if (dataItem.FromDB)
            {
                DP_FormDataRepository orgData = new DP_FormDataRepository(dataItem, dataItem.EditEntityArea);
                //         orgData.ParantChildRelationshipInfo = parentRelationshipInfo;
                foreach (var item in dataItem.KeyProperties)
                    orgData.AddCopyProperty(item);
                OriginalRelatedData.Add(orgData);
            }
            if (show)
            {
                bool isDirect = (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                      RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);

                if (isDirect)
                {
                    RelationshipControl.GenericEditNdTypeArea.ShowDataInDataView(dataItem);
                }
                else
                {
                    SetTempText();
                }
            }
        }
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
        public bool RelationshipIsChangedForUpdate
        {
            get
            {
                return RemovedDataForUpdate.Any() || RelatedData.Any(x => DataItemIsAdded(x));
            }
        }
        public List<DP_FormDataRepository> GetRelatedData(int relationshipID)
        {
            return RelatedData.ToList();
            //    else return new List<ProxyLibrary.DP_FormDataRepository>();
        }
        public void RemoveRelatedData(DP_FormDataRepository DP_FormDataRepository)
        {
            //var childRelationshipInfo = ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationshipID);
            //if (childRelationshipInfo != null)

            //اینجا باید کنترل بشه که میشه حذف بشه اصلا یا نه
            RelatedData.Remove(DP_FormDataRepository);


        }
        public void RemoveRelatedData()
        {
            //var childRelationshipInfo = ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationshipID);
            //if (childRelationshipInfo != null)

            foreach (var item in RelatedData.ToList())
                RemoveRelatedData(item);
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
        public RelationshipDTO Relationship { get { return RelationshipControl.Relationship; } }

        public ObservableCollection<DP_FormDataRepository> RemovedDataForUpdate { set; get; }
        public ObservableCollection<DP_FormDataRepository> RelatedData { set; get; }
        public ObservableCollection<DP_FormDataRepository> RealData
        {
            get
            {
                ObservableCollection<DP_FormDataRepository> result = new ObservableCollection<DP_FormDataRepository>();
                foreach (var item in RelatedData)
                {
                    if (item.ShoudBeCounted)
                    {
                        if (item.ParantChildRelationshipInfo.IsHidden || this.IsReadonlyOnState || item.IsReadonlyOnState)
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
                    if (item.ParantChildRelationshipInfo.IsHidden || this.IsReadonlyOnState || item.IsReadonlyOnState)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
        }

        public bool SecurityIssue { get; set; }
        public ObservableCollection<DP_FormDataRepository> OriginalRelatedData { get; private set; }
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

                return Relationship.IsReadonly || IsReadonlyOnState;

                //|| (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                //              (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
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
            foreach (var relatedData in RelatedData)
            {
                relatedData.AddChangeMonitor(generalKey, usageKey, restTail, columnID, dataToCall);
            }
            //}
        }



        internal void RemoveChangeMonitorByGenaralKey(string key)
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

        public void SetRelatoinsipColumnReadonlyFromState(string message, string key, bool permanent, bool checkInUI)
        {
            if (checkInUI)
            {
                if (SourceData.DataIsInEditMode())
                {
                    AddReadonlyState(key, message, permanent);
                    CheckColumnReadonly();
                    //if (permanent)
                    //{
                    //    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);
                    //    RelationshipControl.GenericEditNdTypeArea.DecideButtonsEnablity1();
                    //}
                }
            }
            else
            {
                AddReadonlyState(key, message, permanent);
            }
        }
        public void ResetColumnReadonlyFromState(string key)
        {
            if (SourceData.DataIsInEditMode())
            {
                RemoveReadonlyState(key);
                CheckColumnReadonly();
            }
        }
        private void CheckColumnReadonly()
        {
            bool isreadonly = true;
            if (RelationshipControl.Relationship.IsReadonly)
            {
                isreadonly = true;
            }
            if (ControlReadonlyStateItems.Any())
                isreadonly = true;

            //set here
        }
        bool dataLoaded = false;
        public bool SetBinding()
        {
            RelationshipControl.GenericEditNdTypeArea.ChildRelationshipInfoBinded = this;
            bool isDirect = (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                       RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
            if (isDirect)
            {
                RelationshipControl.GenericEditNdTypeArea.ClearUIData();
            }
            bool SecurityIssue = false;
            bool result = true;
            List<DP_FormDataRepository> childItems = null;
            if (dataLoaded)
            {
                childItems = RelatedData.ToList();
            }
            //  childData = specificDate.ChildRelationshipInfos.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
            else
            {
                dataLoaded = true;

                bool relationshipFirstSideHasValue = RelationshipControl.Relationship.RelationshipColumns.Any()
                    && RelationshipControl.Relationship.RelationshipColumns.All(x => SourceData.GetProperties().Any(y => !AgentHelper.ValueIsEmpty(y) && y.ColumnID == x.FirstSideColumnID));
                if (!relationshipFirstSideHasValue)
                {
                    //childData = specificDate.AddChildRelationshipInfo(relationshipControl);
                }
                else
                {
                    bool childIsDataView = (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                                           RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
                    Tuple<bool, List<DP_FormDataRepository>> dbSearch;
                    if (childIsDataView)
                        dbSearch = SerachDataFromParentRelationForChildDataView();
                    else
                        dbSearch = SerachDataFromParentRelationForChildTempView();
                    if (dbSearch.Item1)
                    {
                        SetRelatoinsipColumnHiddenFromState("عدم دسترسی به داده", "DataIssue", true, true);
                        return false;
                    }
                    else
                    {
                        foreach (var item in dbSearch.Item2)
                            AddDataToChildRelationshipInfo(item, false);
                    }
                }
            }




            if (isDirect)
            {
                // RelationshipControl.GenericEditNdTypeArea.ClearUIData();
                foreach (var item in RelatedData)
                {
                    RelationshipControl.GenericEditNdTypeArea.ShowDataInDataView(item);
                }
            }
            else
            {
                SetTempText();
            }
            return false;

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
            bool secutrityImposed = false;
            if (count.ResultCount != childViewData.Count)
                return new Tuple<bool, List<DP_FormDataRepository>>(true, null);
            //if (!secutrityImposed)
            //{
            List<DP_FormDataRepository> list = new List<DP_FormDataRepository>();

            EditAreaDataManager EditAreaDataManager = new EditAreaDataManager();
            foreach (var item in childViewData)
            {
                var dpItem = EditAreaDataManager.ConvertDP_DataViewToDP_DataRepository(item, RelationshipControl.GenericEditNdTypeArea);
                dpItem.IsDBRelationship = true;
                dpItem.FromDB = true;
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
                data.IsDBRelationship = true;
                data.DataView = EditAreaDataManager.GetDataView(data);

                DP_FormDataRepository formData = new DP_FormDataRepository(data, RelationshipControl.GenericEditNdTypeArea);
                formData.FromDB = true;
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

        public void SetRelatoinsipColumnHiddenFromState(string message, string key, bool permanent, bool checkInUI)
        {
            if (checkInUI)
            {
                if (SourceData.DataIsInEditMode())
                {
                    AddHiddenState(key, message, permanent);
                    DecideVisiblity();

                }
            }
            else
                AddHiddenState(key, message, permanent);

        }
        public void ResetRelatoinsipColumnVisiblityFromState(string key)
        {
            if (SourceData.DataIsInEditMode())
            {
                RemoveHiddenState(key);
                DecideVisiblity();
            }
        }

        private void DecideVisiblity()
        {
            if (ControlHiddenStateItems.Any())
            {
                //     RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);
                GetUIControlManager.Visiblity(false);
                if (!LableIsShared)
                    RelationshipControl.LabelControlManager.Visiblity(false);
            }
            else
            {
                //    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);
                GetUIControlManager.Visiblity(true);
                if (!LableIsShared)
                    RelationshipControl.LabelControlManager.Visiblity(true);
            }
        }




        public void SelectFromParent(Dictionary<int, string> colAndValues)
        {
            if (SourceData.DataIsInEditMode())
            {
                if (!IsReadonly && !IsHiddenOnState)
                {
                    //    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);

                    bool fromDataview = (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                               RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
                    RelationshipControl.GenericEditNdTypeArea.SearchViewEntityArea.SelectFromParent(fromDataview, Relationship, SourceData, colAndValues);
                }
            }
        }

    }
}
