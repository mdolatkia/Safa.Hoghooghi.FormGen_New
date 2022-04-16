using CommonDefinitions.UISettings;
using ModelEntites;
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
        public RelationshipColumnControlGeneral RelationshipControl { set; get; }
        public event EventHandler<System.Collections.Specialized.NotifyCollectionChangedEventArgs> CollectionChanged;
        public ChildRelationshipInfo(RelationshipColumnControlGeneral relationshipControl, DP_FormDataRepository sourceData) : base(sourceData, relationshipControl)
        {
            SourceData = sourceData;
            RelationshipControl = relationshipControl;
            RelatedData = new ObservableCollection<DP_FormDataRepository>();
            //   HiddenData = new ObservableCollection<DP_FormDataRepository>();
            RelatedData.CollectionChanged += RelatedData_CollectionChanged;
            OriginalRelatedData = new ObservableCollection<DP_FormDataRepository>();
            RemovedDataForUpdate = new ObservableCollection<DP_FormDataRepository>();
            //RemovedItems = new List<ProxyLibrary.DP_FormDataRepository>();
            ReadonlyStateFromTails = new List<string>();
            // this.IsReadonlyChanged += ChildRelationshipInfo_IsReadonlyChanged;



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
            if (CollectionChanged != null)
                CollectionChanged(sender, e);
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
        public void AddDataToChildRelationshipInfo(DP_DataRepository dataItem, bool fromDB)
        {
            //اینجا
        }
        public void AddDataToChildRelationshipInfo(DP_FormDataRepository dataItem, bool fromDB)
        {
            //var parentRelationshipInfo = new ParentRelationshipInfo(this);
            //dataItem.ParantChildRelationshipInfo = parentRelationshipInfo;
            RelatedData.Add(dataItem);
            if (fromDB)
            {
                DP_FormDataRepository orgData = new DP_FormDataRepository(dataItem, dataItem.EditEntityArea);
                //         orgData.ParantChildRelationshipInfo = parentRelationshipInfo;
                foreach (var item in dataItem.KeyProperties)
                    orgData.AddCopyProperty(item);
                OriginalRelatedData.Add(orgData);
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
            RelatedData.Remove(DP_FormDataRepository);

        }
        public void RemoveRelatedData()
        {
            //var childRelationshipInfo = ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationshipID);
            //if (childRelationshipInfo != null)
            RelatedData.Clear();

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




        public void SetRelatoinsipColumnReadonlyFromState(string message, string key, bool permanent)
        {
            if (SourceData.DataIsInEditMode())
            {
                AddReadonlyState(key, message, permanent);
                if (permanent)
                {
                    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);
                    RelationshipControl.GenericEditNdTypeArea.DecideButtonsEnablity1();
                }
            }
        }
        public void ResetColumnReadonlyFromState(string key)
        {
            if (SourceData.DataIsInEditMode())
            {
                RemoveReadonlyState(key);
            }
        }
        public void SetRelatoinsipColumnHiddenFromState(string message, string key, bool permanent)
        {
            if (SourceData.DataIsInEditMode())
            {
                AddHiddenState(key, message, permanent);
                if (permanent)
                {
                    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);
                    GetUIControlManager.Visiblity(false);
                    if (!LableIsShared)
                        RelationshipControl.LabelControlManager.Visiblity(false);
                }
            }
        }
        public void ResetRelatoinsipColumnVisiblityFromState(string key)
        {
            if (SourceData.DataIsInEditMode())
            {
                RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);
                GetUIControlManager.Visiblity(true);
                if (!LableIsShared)
                    RelationshipControl.LabelControlManager.Visiblity(true);
                RemoveHiddenState(key);
            }
        }



        public void SelectFromParent(Dictionary<int, string> colAndValues)
        {
            if (SourceData.DataIsInEditMode())
            {
                if (!IsReadonly && !IsHiddenOnState)
                {
                    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);

                    bool fromDataview = (RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                               RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
                    RelationshipControl.GenericEditNdTypeArea.SearchViewEntityArea.SelectFromParent(fromDataview, Relationship, SourceData, colAndValues);
                }
            }
        }

    }
}
