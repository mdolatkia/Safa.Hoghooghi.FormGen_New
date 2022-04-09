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
        public RelationshipColumnControl RelationshipControl { set; get; }
        public event EventHandler<System.Collections.Specialized.NotifyCollectionChangedEventArgs> CollectionChanged;
        public ChildRelationshipInfo(RelationshipColumnControl relationshipControl, DP_FormDataRepository sourceData) : base(sourceData, relationshipControl)
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
            this.IsReadonlyChanged += ChildRelationshipInfo_IsReadonlyChanged;
        }

        private void ChildRelationshipInfo_IsReadonlyChanged(object sender, EventArgs e)
        {
            if (SourceData.DataIsInEditMode())
            {
                RelationshipControl.EditNdTypeArea.SetChildRelationshipInfo(this);
                RelationshipControl.EditNdTypeArea.DecideButtonsEnablity1();
            }
        }

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
                        if (item.ParantChildRelationshipInfo.IsHidden || this.IsReadonly || item.IsReadonlyBecauseOfState)
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
                    if (item.ParantChildRelationshipInfo.IsHidden || this.IsReadonly || item.IsReadonlyBecauseOfState)
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
        public bool IsHidden { get; set; }

        //میشه ریدونلی بودن ریلیشنشیپ رو هم داخل این گذاشت یا لازم نیست؟

        public bool IsReadonlyOrRelationshipIsReadonly
        {
            get
            {
                return Relationship.IsReadonly || IsReadonly
                    || (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                                (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
            }
        }
        public event EventHandler IsReadonlyChanged;
        bool _IsReadonly;
        public bool IsReadonly
        {
            get { return _IsReadonly; }
            set
            {
                if (value != _IsReadonly)
                {
                    _IsReadonly = value;
                    if (IsReadonlyChanged != null)
                    {
                        IsReadonlyChanged(this, null);
                    }
                }
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



        public void ChangeRelatoinsipColumnReadonlyFromState(bool isReadonly, string message, string key, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                IsReadonly = isReadonly;
                if (!Relationship.IsReadonly)
                {
                    //if (hiddenControlState == ImposeControlState.Impose || hiddenControlState == ImposeControlState.Both)
                    //{

                    //}

                    if (hiddenControlState == ImposeControlState.AddMessageColor || hiddenControlState == ImposeControlState.Both)
                    {
                        if (isReadonly)
                        {
                            AddColumnControlMessage(message + Environment.NewLine + "این رابطه فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد", ControlOrLabelAsTarget.Control, key, ControlItemPriority.High);
                            AddColumnControlColor(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Background, key, ControlItemPriority.High);
                            AddColumnControlColor(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, key, ControlItemPriority.High);
                            if (SourceData.EditEntityArea is I_EditEntityAreaOneData)
                            {
                                AddColumnControlColor(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Background, key, ControlItemPriority.High);
                                AddColumnControlColor(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, key, ControlItemPriority.High);
                                AddColumnControlMessage(message + Environment.NewLine + "این رابطه فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد", ControlOrLabelAsTarget.Control, key, ControlItemPriority.High);
                            }
                        }
                        else
                        {
                            RemoveColumnControlColor(ControlOrLabelAsTarget.Label, key);
                            RemoveColumnControlMessage(ControlOrLabelAsTarget.Label, key);
                            RemoveColumnControlColor(ControlOrLabelAsTarget.Control, key);
                            RemoveColumnControlMessage(ControlOrLabelAsTarget.Control, key);
                        }
                    }
                }
            }
        }

        public void ChangeRelatoinsipColumnVisiblityFromState(bool hidden, string message, string key, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                IsHidden = hidden;

                if (hiddenControlState == ImposeControlState.Impose || hiddenControlState == ImposeControlState.Both)
                {
                    RelationshipControl.EditNdTypeArea.SetChildRelationshipInfo(this);
                    RelationshipControl.ControlManager.Visiblity(SourceData, !hidden);
                }
                if (hiddenControlState == ImposeControlState.AddMessageColor || hiddenControlState == ImposeControlState.Both)
                {
                    if (!hidden)
                    {
                        RemoveColumnControlColor(ControlOrLabelAsTarget.Label, key);
                        RemoveColumnControlMessage(ControlOrLabelAsTarget.Label, key);
                        RemoveColumnControlColor(ControlOrLabelAsTarget.Control, key);
                        RemoveColumnControlMessage(ControlOrLabelAsTarget.Control, key);
                    }
                    else
                    {
                        AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Control, ControlColorTarget.Background, key, ControlItemPriority.High);
                        AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, key, ControlItemPriority.High);
                        AddColumnControlMessage(message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", ControlOrLabelAsTarget.Control, key, ControlItemPriority.High);
                        if (SourceData.EditEntityArea is I_EditEntityAreaOneData)
                        {
                            AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Label, ControlColorTarget.Background, key, ControlItemPriority.High);
                            AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Label, ControlColorTarget.Border, key, ControlItemPriority.High);
                            AddColumnControlMessage(message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", ControlOrLabelAsTarget.Label, key, ControlItemPriority.High);
                        }
                    }
                }
                //foreach (var relCol in childRelationshipInfo.Relationship.RelationshipColumns)
                //{
                //    var fkProp = dataItem.GetProperty(childRelationshipInfo.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary ? relCol.FirstSideColumnID : relCol.SecondSideColumnID);
                //    fkProp.IsHidden = hidden;
                //}
            }
        }




        public void SelectFromParent(Dictionary<int, string> colAndValues)
        {
            if (SourceData.DataIsInEditMode())
            {
                if (!IsReadonlyOrRelationshipIsReadonly && !IsHidden)
                {
                    RelationshipControl.EditNdTypeArea.SetChildRelationshipInfo(this);

                    bool fromDataview = (RelationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                               RelationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
                    RelationshipControl.EditNdTypeArea.SearchViewEntityArea.SelectFromParent(fromDataview, Relationship, SourceData, colAndValues);
                }
            }
        }

    }
}
