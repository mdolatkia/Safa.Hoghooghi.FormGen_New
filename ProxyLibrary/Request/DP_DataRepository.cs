using ModelEntites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
namespace ProxyLibrary
{
    public class DP_BaseData
    {
        public DP_BaseData(int dataItemID)
        {
            DataItemID = dataItemID;
        }

        public DP_BaseData(int targetEntityID, string targetEntityAlias)
        {
            Properties = new List<EntityInstanceProperty>();
            _TargetEntityID = targetEntityID;
            _TargetEntityAlias = targetEntityAlias;
        }
        public List<EntityInstanceProperty> Properties { set; get; }
        public int _TargetEntityID;

        //بعدا بررسی شود این فیلد به چه درد میخورد
        public string _TargetEntityAlias;
        public int TargetEntityID { get { return _TargetEntityID; } }
        public string TargetEntityAlias { get { return _TargetEntityAlias; } }
        public virtual List<EntityInstanceProperty> KeyProperties
        {
            get
            {
                if (this is DP_DataRepository)
                    return (this as DP_DataRepository).KeyProperties;
                else
                {
                    var listKey = Properties.Where(x => string.IsNullOrEmpty(x.RelationshipIDTailPath) && x.IsKey).ToList();
                    if (listKey.Count == 0)
                    {
                        throw new Exception("no key");
                    }
                    return listKey;
                }
            }
        }
        public int DataItemID { set; get; }
        public bool DataItemSearched { set; get; }

        public virtual EntityInstanceProperty GetProperty(int columnID)
        {
            if (this is DP_DataRepository)
                return (this as DP_DataRepository).GetProperty(columnID);
            else

            {
                return Properties.FirstOrDefault(x => x.ColumnID == columnID);
            }
        }
    }
    public class DP_DataRepository : DP_BaseData
    {
        public event EventHandler<PropertyValueChangedArg> PropertyValueChanged;

        //public event EventHandler<ChangeMonitor> RelatedDataCollectionChanged;
        //public event EventHandler<PropertyValueChangedArg> PropertyValueChanged;
        //public Dictionary<int, List<ColumnValueRangeDetailsDTO>> ColumnKeyValueRanges = new Dictionary<int, List<ColumnValueRangeDetailsDTO>>();

        public virtual List<ChildRelationshipData> ChildRelationshipDatas { set; get; }

        public virtual ParentRelationshipData ParantChildRelationshipData { get; set; }

        public DP_DataRepository(int TargetEntityID, string TargetEntityAlias) : base(TargetEntityID, TargetEntityAlias)
        {
            OriginalProperties = new List<ProxyLibrary.EntityInstanceProperty>();
            //SourceRelatedData = new List<DP_DataRepository>();
            //DataInstance = new EntityInstance();
            //DataInstance.Properties = new List<EntityInstanceProperty>();
            //RelationshipColumns = new List<ModelEntites.RelationshipColumnDTO>();
            ChildRelationshipDatas = new List<ChildRelationshipData>();
            //StateIds = new List<int>();

            GUID = Guid.NewGuid();
            //    DataTypes = new List<DP_DataRepository>();
            //ViewEntityProperties = new List<Tuple<int, List<ProxyLibrary.EntityInstanceProperty>>>();
        }

        public void ClearProperties()
        {
            Properties.Clear();
        }

        public EntityListViewDTO EntityListView { set; get; }
        public void SetProperties(List<EntityInstanceProperty> list)
        {
            ClearProperties();
            foreach (var item in list)
                AddProperty(item.Column, item.Value);
        }


        //public int TargetEntityID;
        //public bool Edited { set; get; }
        //   public bool RecentlyEdited { set; get; }
        public List<EntityInstanceProperty> OriginalProperties;

        public string ViewInfo
        {
            get
            {
                string text = "";
                if (DataView != null)
                {
                    return DataView.ViewInfo;
                }
                else if (IsFullData)
                {
                    if (IsNewItem)
                        text = "داده جدید";
                    else
                        text = "داده اصلاحی شده";
                    if (EntityListView != null)
                    {
                        foreach (var listViewColumn in EntityListView.EntityListViewAllColumns.Where(x => x.IsDescriptive))
                        {
                            var value = GetValueSomeHow(listViewColumn.RelationshipTail, listViewColumn.ColumnID);
                            if (value != null)
                                text += (text == "" ? "" : ", ") + listViewColumn.Column.Alias + ": " + value;
                        }
                    }
                    else
                        throw new Exception("asdasdad");
                }
                else
                {
                    return "خطای داده بعدا بررسی شود";
                }


                //این بد نیست
                //string text = "";
                //if (IsFullData)
                //{
                //    if (IsNewItem)
                //        text = "داده جدید";
                //    else
                //        text = "داده موجود";
                //    if (EntityListView != null)
                //    {
                //        foreach (var prop in Properties.Where(x => EntityListView.EntityListViewAllColumns.Any(y => y.ColumnID == x.ColumnID)))
                //        {
                //            text += (text == "" ? "" : ", ") + prop.Column.Alias + ":" + prop.Value;
                //        }
                //    }
                //    else
                //    {
                //        foreach (var prop in KeyProperties)
                //        {
                //            text += (text == "" ? "" : ", ") + prop.Column.Alias + ":" + prop.Value;
                //        }
                //    }

                //}
                //else if (DataView != null)
                //{
                //    var list = DataView.Properties.Where(x => x.IsDescriptive);
                //    if (list.Count() <= 15)
                //    {
                //        foreach (var prop in list)
                //        {
                //            if (!string.IsNullOrEmpty(prop.Value) && prop.Value != "<Null>")
                //                text += (text == "" ? "" : ", ") + prop.Column.Alias + ":" + prop.Value;
                //        }
                //    }
                //    else
                //    {
                //        foreach (var prop in DataView.Properties.Where(x => x.IsDescriptive))
                //        {
                //            if (!string.IsNullOrEmpty(prop.Value) && prop.Value != "<Null>")
                //                text += (text == "" ? "" : ", ") + prop.Value;
                //        }
                //    }
                //}
                //else
                //{
                //    throw (new Exception("asfxcv"));
                //}
                return text;

            }
        }
        public object GetValueSomeHow(EntityRelationshipTailDTO valueRelationshipTail, int valueColumnID)
        {
            if (valueRelationshipTail == null)
            {
                var proprty = GetProperty(valueColumnID);
                return proprty?.Value;
            }
            else
            {
                DP_DataRepository relatedData = null;
                if (ParantChildRelationshipData != null && ParantChildRelationshipData.ToParentRelationshipID == valueRelationshipTail.Relationship.ID)
                {
                    relatedData = ParantChildRelationshipData.SourceData;
                }
                else if (ChildRelationshipDatas.Any(x => x.Relationship.ID == valueRelationshipTail.Relationship.ID))
                {
                    var childInfo = ChildRelationshipDatas.First(x => x.Relationship.ID == valueRelationshipTail.Relationship.ID);
                    if (childInfo.RelatedData.Count != 1)
                    {
                        throw new Exception("asav");
                    }
                    else
                        relatedData = childInfo.RelatedData.First();
                }
                if (relatedData != null)
                    return relatedData.GetValueSomeHow(valueRelationshipTail.ChildTail, valueColumnID);
                else
                {
                    if (DataView != null)
                    {
                        if (DataView.Properties.Any(x => x.RelationshipIDTailPath == valueRelationshipTail.RelationshipIDPath && x.ColumnID == valueColumnID))
                        {
                            return DataView.Properties.First(x => x.RelationshipIDTailPath == valueRelationshipTail.RelationshipIDPath && x.ColumnID == valueColumnID).Value;
                        }
                    }

                    return null;
                }

            }
            //return "";
        }


        public override EntityInstanceProperty GetProperty(int columnID)
        {
            if (IsFullData || DataView == null)
                return Properties.FirstOrDefault(x => x.ColumnID == columnID);
            else
            {

                return DataView.Properties.FirstOrDefault(x => x.ColumnID == columnID);
            }
        }
        public EntityInstanceProperty GetOriginalProperty(int columnID)
        {
            return OriginalProperties.FirstOrDefault(x => x.ColumnID == columnID);
        }

        public string Error { get; set; }
        public bool IsFullData { set; get; }
        //public List<int> ViewEntityColumns { set; get; }
        public DP_DataView DataView { set; get; }

        //public DP_DataRepository PairData { set; get; }



        public Guid GUID;
        public bool IsNewItem;



        public bool IsDBRelationship { get; set; }
        //public bool RelationshipIsAdded { get;  }
        public bool IsEdited { get; set; }



        //internal void OnRelatedDataOrColumnChanged(ChangeMonitor item)
        //{
        //    if (RelatedDataTailOrColumnChanged != null)
        //    {
        //        //var changeMonitor = ChangeMonitorItems.First(x => x.Key == item.Key);
        //        RelatedDataTailOrColumnChanged(this, item);
        //    }
        //}

        //internal void OnRelatedDataTailChanged(ChangeMonitor item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void RemoveDataObserver(string key, string relationshipTail, DP_DataRepository target)
        //{

        //    if (string.IsNullOrEmpty(relationshipTail))
        //    {
        //        foreach (var item in FinalObserverListForColumn.ToList())
        //        {
        //            if (key == item.Key && item.TargetDataItem == target)
        //            {
        //                FinalObserverListForColumn.Remove(item);
        //            }
        //        }
        //        //ObserverList.Remove(ObserverList.FirstOrDefault(x => x.Key == key && x.TargetDataItem == target));
        //    }
        //    else
        //    {
        //        string firstRel = "", Rest = "";
        //        if (relationshipTail.Contains(','))
        //        {
        //            var splt = relationshipTail.Split(",".ToCharArray(), 1);
        //            firstRel = splt[0];
        //            Rest = splt[1];
        //        }
        //        else
        //            firstRel = relationshipTail;

        //        foreach (var item in ChildRelationshipInfos)
        //        {
        //            if (item.Relationship.ID.ToString() == firstRel)
        //            {
        //                item.RemoveDataObserverForColumn(key, Rest, target);

        //                foreach (var relatedData in item.RelatedData)
        //                {
        //                    relatedData.RemoveDataObserver(key, Rest, target);
        //                }
        //            }
        //        }

        //        if (ParantChildRelationshipInfo != null)
        //        {
        //            if (ParantChildRelationshipInfo.Relationship.PairRelationshipID.ToString() == firstRel)
        //            {
        //                ParantChildRelationshipInfo.RemoveDataObserver(key, Rest, target);
        //                ParantChildRelationshipInfo.SourceData.RemoveDataObserver(key, Rest, target);
        //            }
        //        }

        //        foreach (var item in WaitingObserverList.ToList())
        //        {
        //            if (item.Key == key && item.TargetDataItem == target)
        //                WaitingObserverList.Remove(item);
        //        }
        //    }
        //}





        //private void SplitRelationshipTail(string changingRelationshipTail, ref string firstRel, ref string rest)
        //{
        //    if (changingRelationshipTail.Contains(','))
        //    {
        //        var splt = changingRelationshipTail.Split(",".ToCharArray(), 2);
        //        firstRel = splt[0];
        //        rest = splt[1];
        //    }
        //    else
        //    {
        //        firstRel = changingRelationshipTail;
        //    }
        //}

        //public bool ChangeMonitorExists(string generalKey, string usageKey)
        //{
        //    return ChangeMonitorItems.Any(x => x.GeneralKey == generalKey && x.UsageKey == usageKey);
        //}

        //public void AddChangeMonitor(string generalKey, string usageKey, string restTail, int columnID = 0, DP_DataRepository dataToCall = null)
        //{
        //    if (string.IsNullOrEmpty(restTail) && columnID == 0)
        //        return;
        //    if (dataToCall == null)
        //        dataToCall = this;

        //    ChangeMonitorItems.Add(new ChangeMonitor()
        //    {
        //        GeneralKey = generalKey,
        //        UsageKey = usageKey,
        //        DataToCall = dataToCall,
        //        columnID = columnID,
        //        RestTail = restTail
        //    });
        //    if (!string.IsNullOrEmpty(restTail))
        //        CheckChildRelationshipInfoChangeMonitor();
        //}


        //public void RemoveChangeMonitorByGenaralKey(string key)
        //{
        //    foreach (var item in ChangeMonitorItems.Where(x => x.GeneralKey == key).ToList())
        //    {
        //        ChangeMonitorItems.Remove(item);
        //        foreach (var childRelationshipInfo in ChildRelationshipInfos)
        //        {
        //            childRelationshipInfo.RemoveChangeMonitorByGenaralKey(key);
        //        }

        //        if (ParantChildRelationshipInfo != null)
        //        {
        //            ParantChildRelationshipInfo.ParantChildRelationshipInfo.RemoveChangeMonitorByGenaralKey(key);
        //        }
        //    }
        //}

        public void AddProperty(ColumnDTO column, object value)
        {
            EntityInstanceProperty property = new ProxyLibrary.EntityInstanceProperty(column);
            //property.Name = column.Name;
            property.PropertyValueChanged += Property_PropertyValueChanged;
            property.Value = value;
            Properties.Add(property);
            OriginalProperties.Add(CopyProperty(property));
        }
        //public bool PropertyValueIsChanged(EntityInstanceProperty property)
        //{
        //    if (property.IsReadonly)
        //        return false;
        //    else
        //    {
        //        var orgProperty = OriginalProperties.First(x => x.ColumnID == property.ColumnID);
        //        return property.Value != orgProperty.Value;
        //    }
        //}
        public EntityInstanceProperty AddCopyProperty(EntityInstanceProperty currrentProperty)
        {
            var prop = CopyProperty(currrentProperty);
            Properties.Add(prop);
            return prop;
        }

        private EntityInstanceProperty CopyProperty(EntityInstanceProperty currrentProperty)
        {
            EntityInstanceProperty property = new ProxyLibrary.EntityInstanceProperty(currrentProperty.Column);
            //property.Name = currrentProperty.Name;
            property.Value = currrentProperty.Value;
            //    property.IsHidden = currrentProperty.IsHidden;
            property.ISFK = currrentProperty.ISFK;
            //      property.IsReadonlyFromState = currrentProperty.IsReadonlyFromState;
            //      property.PropertyValueChanged += Property_PropertyValueChanged;
            property.FormulaID = currrentProperty.FormulaID;
            property.FormulaException = currrentProperty.FormulaException;
            property.FormulaUsageParemeters = currrentProperty.FormulaUsageParemeters;
            return property;
        }

        private void Property_PropertyValueChanged(object sender, PropertyValueChangedArg e)
        {
            //if (ChangeMonitorItems.Any(x => x.columnID != 0 && string.IsNullOrEmpty(x.RestTail)))
            //{
            //    foreach (var item in ChangeMonitorItems.Where(x => x.columnID != 0 && string.IsNullOrEmpty(x.RestTail)))
            //    {
            //        if (e.ColumnID == item.columnID)
            //            item.DataToCall.OnRelatedDataOrColumnChanged(item);
            //    }
            //}
            if (PropertyValueChanged != null)
            {
                e.DataItem = this;
                PropertyValueChanged(this, e);
            }
        }


        public override List<EntityInstanceProperty> KeyProperties
        {
            get
            {
                var listKey = new List<EntityInstanceProperty>();
                if (IsFullData || DataView == null)
                    listKey = Properties.Where(x => x.IsKey).ToList();
                else
                {

                    listKey = DataView.Properties.Where(x => string.IsNullOrEmpty(x.RelationshipIDTailPath) && x.IsKey).ToList();
                }
                //if (listKey.Count == 0)
                //    throw new Exception("dfsdf");
                return listKey;
            }
        }

        public List<EntityInstanceProperty> GetProperties()
        {
            return Properties;
            //if (IsFullData || DataView == null)
            //    return Properties;
            //else
            //{

            //    return DataView.Properties;
            //}

        }
        //List<EntityInstanceProperty> Properties { set; get; }
        //List<EntityInstanceProperty> OriginalProperties { set; get; }
        //public List<int> StateIds { set; get; }

        //public bool ExcludeFromDataEntry { set; get; }


        //public EntityInstance DataInstance;

        //public bool ValueChanged { set; get; }

        public string Info
        {
            get
            {
                string info = "";
                if (!string.IsNullOrEmpty(TargetEntityAlias))
                    info += TargetEntityAlias + ", ";
                foreach (var item in Properties)
                {
                    info += (info == "" ? "" : Environment.NewLine) + item.Name + " : " + item.Value;
                }
                return info;
            }
        }




        //    public bool ShouldWriteSimpleColumnsQuery { get; set; }
        //   public bool ISValid { get; set; }

        //bool _HasDirectData;
        //public bool HasDirectData
        //{
        //    set { _HasDirectData = value; }
        //    get
        //    {
        //        if (!IsFullData)
        //            return true;
        //        else return _HasDirectData;
        //    }
        //}
        //bool _HasInDirectData;
        //public bool HasInDirectData
        //{
        //    set { _HasInDirectData = value; }
        //    get
        //    {//یا خودش داده مستقیم داده یا فرزندان از فرمهای وابسته به این داده
        //        if (HasDirectData)
        //            return true;
        //        if (ChildRelationshipInfos.Any(x => x.RelatedData.Any(y => y.HasDirectData)))
        //            return true;
        //        return false;
        //    }
        //}

        //باید تو این شرط زیر هم چک شود
        //!item.KeyProperties.All(y => OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value))
        //که اگر داده دوباره انتخاب شد

        //   public bool IsEmptyOneDirectData { get; set; }
        //  public bool? DataOrRelatedDataIsChanged { get; set; }
        //public bool RelationshipIsRemoved { get

        //    {

        //    }
        //}


        //  public bool IsReadonlyBecauseOfState { get; set; }
        //   public bool IsReadonlyBecauseOfCreatorRelationshipOnState { get; set; }
        //   public bool IsReadonlyBecauseOfCreatorRelationshipOnShow { set; get; }

        //public bool IsReadonlySomeHow
        //{
        //    get
        //    {
        //        return IsReadonlyBecauseOfCreatorRelationshipOnState || IsReadonlyBecauseOfCreatorRelationshipOnShow || IsReadonlyBecauseOfState;
        //    }
        //}

        //      public bool IsHiddenBecauseOfCreatorRelationshipOnState { set; get; }
        //public bool ShoudBeCounted
        //{
        //    get
        //    {
        //        if (IsEmptyOneDirectData)
        //            return false;
        //        else if (IsUseLessBecauseNewAndReadonly)
        //            return false;
        //        else if (ParantChildRelationshipInfo != null && ParantChildRelationshipInfo.IsHidden)
        //            return false;
        //        else if (ParantChildRelationshipInfo != null && ParantChildRelationshipInfo.IsReadonly && !IsDBRelationship)
        //            return false;
        //        else if (IsReadonlySomeHow &&  !IsDBRelationship)
        //            return false;
        //        return true;
        //    }
        //}

        //    public bool IsUseLessBecauseNewAndReadonly { get; set; }

        //public bool IsHiddenBecauseOfCreatorRelationshipOnShow { set; get; }
        //public bool IsHidden
        //{
        //    get
        //    {
        //        return IsHiddenBecauseOfCreatorRelationshipOnState || IsHiddenBecauseOfCreatorRelationshipOnShow;
        //    }
        //}

        // List<ChangeMonitor> ChangeMonitorItems = new List<ChangeMonitor>();


        //public bool ShouldWriteUpdateQuery { get; set; }

        //public void OnUpdated()
        //{
        //    if (Updated != null)
        //        Updated(this, null);
        //}

        //public ChildRelationshipInfo AddChildRelationshipInfo(RelationshipDTO relationship)
        //{
        //    var childRelationshipInfo = new ChildRelationshipInfo() { Relationship = relationship, SourceData = this };
        //    ChildRelationshipInfos.Add(childRelationshipInfo);
        //    CheckChildRelationshipInfoChangeMonitor();
        //    return childRelationshipInfo;
        //}

        //private void CheckChildRelationshipInfoChangeMonitor()
        //{
        //    if (ChangeMonitorItems != null)
        //    {
        //        foreach (var item in ChangeMonitorItems.ToList())
        //        {
        //            if (!string.IsNullOrEmpty(item.RestTail))
        //            {
        //                string firstRel = "", Rest = "";
        //                SplitRelationshipTail(item.RestTail, ref firstRel, ref Rest);
        //                bool observerSet = false;
        //                foreach (var childRelationshipInfo in ChildRelationshipInfos)
        //                {

        //                    if (childRelationshipInfo.Relationship.ID.ToString() == firstRel)
        //                    {
        //                        childRelationshipInfo.AddChangeMonitor(item.GeneralKey, item.UsageKey, Rest, item.columnID, item.DataToCall);
        //                        ChangeMonitorItems.Remove(item);
        //                        observerSet = true;
        //                    }
        //                }
        //                if (!observerSet)
        //                {
        //                    if (ParantChildRelationshipInfo != null)
        //                    {
        //                        //مطمئن نیستم این چک ستون درست باشه بعداً اضافه شده. بیشتر بررسی شود و چون وقتی ستون مدنظر نباشد
        //                        // و تغییر رابطه بخواد مونیتور بشه چم کردن پرنتها بیهوده است. چون پرنت عوض شه کلا همه چی عوض میشه
        //                        //if (item.columnID != 0)
        //                        //{
        //                        if (ParantChildRelationshipInfo.RelationshipID.ToString() == firstRel)
        //                        {
        //                            ParantChildRelationshipInfo.SourceData.AddChangeMonitor(item.GeneralKey, item.UsageKey, Rest, item.columnID, item.DataToCall);
        //                            ChangeMonitorItems.Remove(item);
        //                            observerSet = true;

        //                        }
        //                        //}
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}



        //private bool DataItemIsTargetOrIsInParents(DP_DataRepository targetDataItem, ChildRelationshipInfo parentChildRelationshipInfo)
        //{
        //    if (parentChildRelationshipInfo != null)
        //    {
        //        if (parentChildRelationshipInfo.SourceData == targetDataItem)
        //            return true;
        //        else
        //            return DataItemIsTargetOrIsInParents(targetDataItem, parentChildRelationshipInfo.SourceData.ParantChildRelationshipInfo);
        //    }
        //    return false;
        //}





        //public void OnChildRelatedDataChanged(string relationshipTail, DP_DataRepository sourceDataItem, PropertyValueChangedArg column)
        //{
        //    if (RelatedDataChanged != null)
        //    {
        //        RelatedDataChanged(this, new ProxyLibrary.RelatedDataChangedArg() { RelationshipTail = relationshipTail, SourceDataItem = sourceDataItem, Column = column });
        //    }
        //    if (ParantChildRelationshipInfo != null)
        //        ParantChildRelationshipInfo.ParentData.OnChildRelatedDataChanged(ParantChildRelationshipInfo.Relationship.ID.ToString() + "," + relationshipTail, sourceDataItem, column);
        //}

        //public void OnParentRelatedDataChanged(string relationshipTail, DP_DataRepository sourceDataItem, PropertyValueChangedArg column)
        //{
        //    if (RelatedDataChanged != null)
        //    {
        //        RelatedDataChanged(this, new ProxyLibrary.RelatedDataChangedArg() { RelationshipTail = relationshipTail, SourceDataItem = sourceDataItem, Column = column });
        //    }
        //    foreach (var child in ChildRelationshipInfos)
        //    {
        //        foreach (var item in child.RelatedData)
        //            item.OnParentRelatedDataChanged(child.Relationship.PairRelationshipID.ToString() + "," + relationshipTail, sourceDataItem, column);
        //    }
        //}
    }


    public class RemoveRelationshipInfo
    {
        public RemoveRelationshipInfo()
        {
            RelatedData = new ObservableCollection<ProxyLibrary.DP_DataRepository>();
        }
        public RelationshipDeleteOption RelationshipDeleteOption { set; get; }
        public RelationshipDTO Relationship { set; get; }
        public ObservableCollection<DP_DataRepository> RelatedData { set; get; }
    }
    //public class EntityInstance
    //{


    //    public EntityInstance()
    //    {


    //    }
    //    public int ID;

    //}


    //public class DP_DataViewItem
    //{
    //    public DP_DataViewItem()
    //    {
    //        KeyProperties = new List<EntityInstanceProperty>();
    //        Properties = new List<EntityInstanceProperty>();
    //    }
    //    public int RelationshipTailID { set; get; }
    //    public int EntityID { set; get; }
    //    public List<EntityInstanceProperty> KeyProperties { set; get; }
    //    public List<EntityInstanceProperty> Properties { set; get; }
    //}

    //public class DP_DataViewColumn
    //{
    //    public DP_DataViewItem()
    //    {
    //        KeyProperties = new List<EntityInstanceProperty>();
    //        Properties = new List<EntityInstanceProperty>();
    //    }
    //    public int RelationshipTailID { set; get; }
    //    public int EntityID { set; get; }
    //    public List<EntityInstanceProperty> KeyProperties { set; get; }
    //    public List<EntityInstanceProperty> Properties { set; get; }
    //}
    public class EntityInstanceProperty : INotifyPropertyChanged
    {
        public event EventHandler<PropertyValueChangedArg> PropertyValueChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public EntityInstanceProperty(ColumnDTO column)
        {
            FormulaUsageParemeters = new List<FormulaUsageParemetersDTO>();
            Column = column;
        }
        public EntityInstanceProperty PKIdentityColumn { set; get; }
        //public EntityInstanceProperty(int columnID, string value)
        //{
        //    ColumnID = columnID;
        //    Value = value;
        //}
        public ColumnDTO Column { set; get; }
        public int ColumnID
        {
            get { return Column.ID; }
        }

        public string RelativeName;
        public string Name
        {
            get { return Column.Name; }
        }
        protected void OnPropertyChanged(string name)
        {
            //PropertyChangedEventHandler handler = PropertyChanged;
            //if (handler != null)
            //{
            //    handler(this, new PropertyChangedEventArgs(name));
            //}

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }
        public object TestValue
        {
            set; get;
        }
        object _Value;
        public object Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {

                    PropertyValueChangedArg arg = new PropertyValueChangedArg();
                    arg.ColumnID = ColumnID;
                    arg.OldValue = _Value;
                    arg.NewValue = value;
                    _Value = value;


                    OnPropertyChanged("Value");
                    OnPropertyValueChanged(arg);
                }
            }
        }

        private void OnPropertyValueChanged(PropertyValueChangedArg arg)
        {
            if (PropertyValueChanged != null)
                PropertyValueChanged(this, arg);

        }

        public bool IsKey { get { return Column.PrimaryKey; } }

        //public bool IsIdentity
        //{
        //    get { return Column.IsIdentity; }
        //}

        public bool? IsDescriptive { get { return ListViewColumn != null ? ListViewColumn.IsDescriptive : (bool?)null; } }
        //public PropertyContitionType ContitionType;
        public List<FormulaUsageParemetersDTO> FormulaUsageParemeters { set; get; }
        public string FormulaException { set; get; }
        public int FormulaID { get; set; }
        public bool ValueIsChanged { get; set; }

        //public int ListViewColumnID { set; get; }
        public string RelationshipIDTailPath { get; set; }
        public bool HasForeignKeyData { get; set; }

        public bool ISFK { get; set; }
        public EntityListViewColumnsDTO ListViewColumn { get; set; }

        public bool ValueIsEmptyOrDefaultValue()
        {
            //**9b38140b-d290-4157-a617-b1b659034738
            return ValueIsEmpty() || Value.ToString() == Column.DefaultValue;
        }
        public bool ValueIsEmpty()
        {
            if (Value == null || string.IsNullOrEmpty(Value.ToString()) || Value.ToString() == "0")
                return true;
            else
                return false;
        }
        //public int EntityListViewColumnsID { get; set; }
    }
    public class PreDefinedSearchDTO : SavedSearchRepositoryDTO
    {
        public PreDefinedSearchDTO()
        {
            SimpleSearchProperties = new List<DP_PreDefinedSearchSimpleColumn>();
            RelationshipSearchProperties = new List<DP_PreDefinedSearchRelationship>();
        }
        public int EntitySearchID { get; set; }
        public string QuickSearchValue { get; set; }
        public List<DP_PreDefinedSearchSimpleColumn> SimpleSearchProperties { get; set; }
        public List<DP_PreDefinedSearchRelationship> RelationshipSearchProperties { get; set; }
    }
    public class DP_PreDefinedSearchSimpleColumn
    {
        public List<object> Value { get; set; }
        public CommonOperator Operator { set; get; }
        public int EntitySearchColumnsID { get; set; }
    }
    public class DP_PreDefinedSearchRelationship
    {
        public List<DP_PreDefinedSearchRelationshipData> DataItems { get; set; }
        public int EntitySearchColumnsID { get; set; }
    }
    public class DP_PreDefinedSearchRelationshipData
    {
        public List<DP_PreDefinedSearchRelationshipColumns> KeyProperties { get; set; }

    }
    public class DP_PreDefinedSearchRelationshipColumns
    {
        public int ColumnID { get; set; }
        public object Value { get; set; }
    }

    public class AdvancedSearchDTO : SavedSearchRepositoryDTO
    {
        public AdvancedSearchDTO()
        {
         
        }
      public DP_SearchRepositoryMain SearchRepositoryMain { set; get; }
    }

    public class SavedSearchRepositoryDTO
    {
        public int ID { set; get; }
        public string Title { set; get; }
        public int EntityID { set; get; }
        public bool IsPreDefinedOrAdvanced { get; set; }

        
    }
    public class DP_SearchRepositoryMain : LogicPhraseDTO /*: RootPhrase*/
    {
        public DP_SearchRepositoryMain()
        {
            //فقط برای سریالیز شدن در گزارشات
        }
        public DP_SearchRepositoryMain(int targetEntityID)
        {
            TargetEntityID = targetEntityID;

        }
        public string Title { get; set; }
      
        public int TargetEntityID
        { set; get; }
     
        public Guid GUID;
    }

    public class DP_SearchRepositoryRelationship : LogicPhraseDTO /*: RootPhrase*/
    {
        public DP_SearchRepositoryRelationship()
        {
            //فقط برای سریالیز شدن در گزارشات
        }
       
        public string Title { get; set; }
        public bool? HasNotRelationshipCheck { set; get; }
        public int? RelationshipFromCount { set; get; }
        public int? RelationshipToCount { set; get; }
     //   public int TargetEntityID
       // { set; get; }
        public RelationshipDTO SourceRelationship { set; get; }
        public Guid GUID;
    }


    //public class DP_SearchGroup
    //{
    //    public DP_SearchGroup()
    //    {
    //        SearchItems = new List<DP_SearchRepository>();
    //    }
    //    public AndORType AndOrType { set; get; }
    //    public List<DP_SearchRepository> SearchItems { set; get; }
    //}
    //public class PropertyListPhrase
    //{
    //    public List<SearchProperty> Properties { set; get; }
    //}
    public class Phrase
    {
        //public PropertyListPhrase PropertyListPhrase { set; get; }
        //public LogicPhrase LogicPhrase { set; get; }
    }
    //public class SearchPhrase : Phrase
    //{

    //}
    public class LogicPhraseDTO : Phrase
    {
        public LogicPhraseDTO()
        {
            Phrases = new List<ProxyLibrary.Phrase>();
        }
        public int ID { set; get; }
        public AndOREqualType AndOrType { set; get; }
        public List<Phrase> Phrases { set; get; }
    }
    public class SearchProperty : Phrase
    {
        public int ID;
        public int ColumnID;


        public string Name;
        public object Value;
        public bool IsKey { set; get; }
        //public AndORType AndORType { set; get; }
        public CommonOperator Operator { set; get; }
        public SearchEnumerableType SearchEnumerableType { set; get; }
        //   public int SearchColumnID { get; set; }
        public bool NotIgnoreZeroValue { set; get; }
        //public Tuple<AndORType, List<SearchProperty>> ChildProperties { set; get; }
    }

    public enum AndORType
    {
        And,
        Or
    }
    public enum AndOREqualType
    {
        And,
        Or,
        NotAnd,
        NotOr
    }
    public class AndORListItem
    {
        public AndOREqualType AndOR { set; get; }
        public string Title { set; get; }
        public bool IsDefault { get; set; }
    }

    public enum InORNotIn
    {
        In,
        NotIn
    }
    public class PropertyValueChangedArg : EventArgs
    {
        public int ColumnID { set; get; }
        public DP_DataRepository DataItem { set; get; }
        public object OldValue { set; get; }
        public object NewValue { set; get; }

    }
    //public class RelatedDataCollectionChangedArg : EventArgs
    //{
    //    public RelationshipDTO Relationship { set; get; }
    //    public DP_DataRepository DataItem { set; get; }
    //    public System.Collections.Specialized.NotifyCollectionChangedEventArgs CollectionChangedEventArgs { set; get; }


    //}
    //public class ObserverData
    //{
    //    public string Key { set; get; }
    //    public DP_DataRepository TargetDataItem { set; get; }
    //    public int TargetItemID { set; get; }
    //    public string ChangingSourceRalationshipTail { set; get; }
    //    public int ChangingColumnID { set; get; }
    //}




    //public class RelatedDataTailChangedArg : EventArgs
    //{
    //    //public string Key { set; get; }
    //    //public string RelationshipTail { set; get; }

    //    public ObserverData ObserverData { set; get; }
    //    //   public DP_DataRepository SourceDataItem { set; get; }
    //}
    public class RelatedDataTailOrColumnChangedArg : EventArgs
    {
        public string Key { set; get; }
        public int columnID { set; get; }
        public string RestTail { set; get; }

    }









    //        It is not possible the way you want it, exactly.

    //You can create a "new" property with the same name:

    //public new SunData Data
    //        {
    //            get { return (SunData)base.Data; }
    //            set { base.Data = value; }
    //        }
    //        It's not quite the same thing, but it's probably as close as you're going to get.



    //Another possible approach would be:

    //public SunData SunData { get; set; }

    //        public override OuterSpaceData Data
    //        {
    //            get { return SunData; }
    //            set { SunData = (SunData)value; }
    //        }
    //        The advantage of this approach is that it does guarantee that it's impossible to put something in your Data property that is not a SunData. The downside is that your Data property is not strongly typed and you have to use the SunData property if you want it statically typed to SunData.






    //public void AddDataToChildRelationshipInfo(DP_DataRepository dataItem, bool fromDB)
    //{
    //    var parentRelationshipInfo = new ParentRelationshipInfo() { ParantChildRelationshipInfo = this };
    //    dataItem.ParantChildRelationshipInfo = parentRelationshipInfo;
    //    RelatedData.Add(dataItem);
    //    if (fromDB)
    //    {
    //        ProxyLibrary.DP_DataRepository orgData = new ProxyLibrary.DP_DataRepository(dataItem.TargetEntityID, dataItem.TargetEntityAlias);
    //        orgData.ParantChildRelationshipInfo = parentRelationshipInfo;
    //        foreach (var item in dataItem.KeyProperties)
    //            orgData.AddCopyProperty(item);
    //        OriginalRelatedData.Add(orgData);
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

    //public bool CheckRelationshipIsChanged()
    //{
    //    foreach (var deleted in OriginalRelatedData.Where(x => !x.KeyProperties.All(y => RelatedData.Any(z => z.IsNewItem == false && z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
    //    {
    //        return true;
    //    }
    //    foreach (var added in RelatedData.Where(x => !x.KeyProperties.All(y => OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)))))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    //public bool DataItemIsAdded(DP_DataRepository item)
    //{
    //    return item.IsNewItem || !item.KeyProperties.All(y => OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)));
    //}
    //public bool OriginalDataItemIsRemoved(DP_DataRepository originalData)
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

    //public List<DP_DataRepository> GetRelatedData(int relationshipID)
    //{
    //    return RelatedData.ToList();
    //    //    else return new List<ProxyLibrary.DP_DataRepository>();
    //}
    //public void RemoveRelatedData(DP_DataRepository dP_DataRepository)
    //{
    //    //var childRelationshipInfo = ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationshipID);
    //    //if (childRelationshipInfo != null)
    //    RelatedData.Remove(dP_DataRepository);

    //}
    //public void RemoveRelatedData()
    //{
    //    //var childRelationshipInfo = ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == relationshipID);
    //    //if (childRelationshipInfo != null)
    //    RelatedData.Clear();

    //}


    //internal void AddDataObserver(string key, string restTail, int columnID, DP_DataRepository targetDataItem, bool setSourceOrChilds)
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


    //internal void RemoveDataObserver(string key, string restTail, DP_DataRepository targetDataItem)
    //{
    //    foreach (var item in ObserverList.ToList())
    //    {
    //        if (key == item.Key && item.TargetDataItem == targetDataItem)
    //        {
    //            ObserverList.Remove(item);
    //        }
    //    }

    //}

    //public ObservableCollection<DP_DataRepository> RealData
    //{
    //    get
    //    {
    //        ObservableCollection<DP_DataRepository> result = new ObservableCollection<DP_DataRepository>();
    //        foreach (var item in RelatedData)
    //        {
    //            if (item.ShoudBeCounted)
    //            {
    //                if (item.IsHiddenBecauseOfCreatorRelationshipOnState || this.IsReadonly || item.IsReadonlySomeHow)
    //                {
    //                    if (!DataItemIsAdded(item))
    //                        result.Add(item);
    //                }
    //                else
    //                    result.Add(item);
    //            }
    //        }
    //        foreach (var item in RemovedOriginalDatas)
    //        {
    //            if (item.IsHiddenBecauseOfCreatorRelationshipOnState || this.IsReadonly || item.IsReadonlySomeHow)
    //            {
    //                result.Add(item);
    //            }
    //        }
    //        return result;
    //    }
    //}

    // public bool SecurityIssue { get; set; }
    //public List<DP_DataRepository> RemovedOriginalDatas
    //{
    //    get
    //    {
    //        return OriginalRelatedData.Where(x => !x.KeyProperties.All(y => RelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value.Equals(y.Value))))).ToList();
    //    }
    //}
    //    public List<string> ReadonlyStateFromTails { set; get; }
    //   public bool IsHidden { get; set; }

    //میشه ریدونلی بودن ریلیشنشیپ رو هم داخل این گذاشت یا لازم نیست؟
    //    public bool IsReadonly { get; set; }
    //این باید خود رابطه را چک کنه همچنین اینکه رابطه کریتورش فقط خواندنی هست یا نه؟ خود موجودیت طرفین هم فکر بشه

    // List<ChangeMonitor> ChangeMonitorItems = new List<ChangeMonitor>();

    //اینجا وظیفه چک کردن هم داده ها و هم ستونهای داده را دارد
    //internal void AddChangeMonitor(string generalKey, string usageKey, string restTail, int columnID, DP_DataRepository dataToCall)
    //{
    //    ChangeMonitorItems.Add(new ChangeMonitor()
    //    {
    //        GeneralKey = generalKey,
    //        UsageKey = usageKey,
    //        DataToCall = dataToCall,
    //        columnID = columnID,
    //        RestTail = restTail

    //    });

    //    //if (!string.IsNullOrEmpty(restTail))
    //    //{
    //    foreach (var relatedData in RelatedData)
    //    {
    //        relatedData.AddChangeMonitor(generalKey, usageKey, restTail, columnID, dataToCall);
    //    }
    //    //}
    //}



    //internal void RemoveChangeMonitorByGenaralKey(string key)
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

    //public bool OriginalDataHasBecomeHidden(DP_DataRepository orginalData)
    //{
    //    var currentData = RelatedData.FirstOrDefault(z => z.IsNewItem == false && orginalData.KeyProperties.All(x => z.KeyProperties.Any(u => x.ColumnID == u.ColumnID && x.Value == u.Value)));
    //    if (currentData != null && currentData.IsHidden)
    //        return true;
    //    else
    //        return false;
    //}

    //public DP_DataRepository GetOroginalDataOfOriginalData(DP_DataRepository data)
    //{
    //    return OriginalRelatedData.First(z => data.KeyProperties.All(y => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)));
    //}

    //public DP_DataRepository GetRelatedDataOfOriginalData(DP_DataRepository orginalData)
    //{
    //    return RelatedData.First(z => orginalData.KeyProperties.All(y => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value)));
    //}


    //public ObservableCollection<DP_DataRepository> HiddenData { set; get; }
    //public void AddHiddenDataRelationship(DP_DataRepository dataItem)
    //{
    //    if (!HiddenData.Any(x => x == dataItem))
    //        HiddenData.Add(dataItem);
    //}
    //public void RemoveHiddenDataRelationship(DP_DataRepository dataItem)
    //{
    //    if (HiddenData.Any(x => x == dataItem))
    //    {
    //        HiddenData.Remove(HiddenData.First(x => x == dataItem));
    //    }
    //}




    //public bool OriginalDataHasBecomeReadonlyAndNotExists(DP_DataRepository orginalRelationships)
    //{
    //    var currentData = RelatedData.FirstOrDefault(z => z.IsNewItem == false && orginalRelationships.KeyProperties.All(x => z.KeyProperties.Any(u => x.ColumnID == u.ColumnID && x.Value == u.Value)));
    //    if (currentData != null && currentData.IsReadonly)
    //        return true;
    //    else
    //        return false;
    //}
    //   public bool RelationshipIsChanged { get; set; }


    //public class ParentRelationshipInfo

    //{
    //    public ParentRelationshipInfo(ChildRelationshipInfo parantChildRelationshipInfo, RelationshipColumnControl )
    //    {
    //        ParantChildRelationshipInfo = parantChildRelationshipInfo;
    //    }
    //    public ChildRelationshipInfo ParantChildRelationshipInfo { set; get; }
    //    public int RelationshipID { get { return ParantChildRelationshipInfo.Relationship.PairRelationshipID; } }
    //    public RelationshipDTO ToRelationship { get { return ParantChildRelationshipInfo.Relationship.PairRelationship; } }
    //    public DP_DataRepository SourceData { get { return ParantChildRelationshipInfo.SourceData; } }
    //    public bool IsHidden { get; set; }
    //    public bool IsReadonly { get; set; }
    //}
}
