using CommonDefinitions.UISettings;
using ModelEntites;
using MyUILibrary.Temp;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{
    public class DP_FormDataRepository : DP_DataRepository
    {
        //   public event EventHandler<PropertyValueChangedArg> PropertyValueChanged;
        public new event EventHandler<PropertyValueChangedArg> PropertyValueChanged;
        public event EventHandler<ChangeMonitor> RelatedDataTailOrColumnChanged;
        public new ParentRelationshipInfo ParantChildRelationshipData
        {
            set { base.ParantChildRelationshipData = value; }
            get { return base.ParantChildRelationshipData as ParentRelationshipInfo; }
        }

        //public event EventHandler<ChangeMonitor> RelatedDataCollectionChanged;
        //  public event EventHandler<PropertyValueChangedArg> PropertyValueChanged;
        public new List<ChildRelationshipInfo> ChildRelationshipDatas { get { return base.ChildRelationshipDatas.Cast<ChildRelationshipInfo>().ToList(); } }

        //   public List<ControlStateItem> ReadonlyStateItems = new List<ControlStateItem>();
        public List<ControlStateItem> ToParentRelationshipReadonlyStateItems = new List<ControlStateItem>();
        public List<ControlStateItem> ToParentRelationshipHiddenStateItems = new List<ControlStateItem>();
        public I_EditEntityArea EditEntityArea { set; get; }
        //      public Dictionary<int, List<ColumnValueRangeDetailsDTO>> ColumnKeyValueRanges = new Dictionary<int, List<ColumnValueRangeDetailsDTO>>();

        public DP_FormDataRepository(DP_DataView baseData, I_EditEntityArea editEntityArea, bool isDBRelationship, bool isNewItem) : base(baseData.TargetEntityID, baseData.TargetEntityAlias)
        {


            _TargetEntityID = baseData.TargetEntityID;
            _TargetEntityAlias = baseData.TargetEntityAlias;
            Properties = baseData.Properties;
            foreach (var property in Properties)
                property.PropertyValueChanged += Property_PropertyValueChanged;
            DataView = baseData;
            GUID = baseData.GUID;

            IsNewItem = isNewItem;
            IsDBRelationship = isDBRelationship;
            //     IsFullData = baseData.IsFullData;
            //       IsNewItem = baseData.IsNewItem;
            //         IsDBRelationship = baseData.IsDBRelationship;

            //if (childRelationshipInfo != null)
            //    ParantChildRelationshipInfo = new ParentRelationshipInfo(childRelationshipInfo);


            EditEntityArea = editEntityArea;

            OriginalProperties = new List<ProxyLibrary.EntityInstanceProperty>();
            //SourceRelatedData = new List<DP_FormDataRepository>();
            //DataInstance = new EntityInstance();
            //DataInstance.Properties = new List<EntityInstanceProperty>();
            //RelationshipColumns = new List<ModelEntites.RelationshipColumnDTO>();
            //  ChildRelationshipDatas = new List<ChildRelationshipInfo>();
            ChildSimpleContorlProperties = new List<ChildSimpleContorlProperty>();
            ChangeMonitorItems = new List<ChangeMonitor>();

            //   SetProperties();
            //StateIds = new List<int>();
            // this.IsReadonlyBecauseOfStateChanged += DP_FormDataRepository_IsReadonlyBecauseOfStateChanged;
            //    GUID = Guid.NewGuid();

            //    DataTypes = new List<DP_FormDataRepository>();
            //ViewEntityProperties = new List<Tuple<int, List<ProxyLibrary.EntityInstanceProperty>>>();
        }
        //public AddChildRelationshipInfo(ChildRelationshipInfo childInfo)
        //{
        //}
        public DP_FormDataRepository(DP_DataRepository baseData, I_EditEntityArea editEntityArea, bool isDBRelationship, bool isNewItem) : base(baseData.TargetEntityID, baseData.TargetEntityAlias)
        {
            baseData.PropertyValueChanged += DP_FormDataRepository_PropertyValueChanged;

            _TargetEntityID = baseData.TargetEntityID;
            _TargetEntityAlias = baseData.TargetEntityAlias;
            Properties = baseData.Properties;
            foreach (var property in Properties)
                property.PropertyValueChanged += Property_PropertyValueChanged;
            EntityListView = baseData.EntityListView;

            DataView = baseData.DataView;
            GUID = baseData.GUID;
            IsFullData = true;
            IsNewItem = isNewItem;
            IsDBRelationship = isDBRelationship;

            //if (childRelationshipInfo != null)
            //    ParantChildRelationshipInfo = new ParentRelationshipInfo(childRelationshipInfo);


            EditEntityArea = editEntityArea;

            OriginalProperties = new List<ProxyLibrary.EntityInstanceProperty>();
            //SourceRelatedData = new List<DP_FormDataRepository>();
            //DataInstance = new EntityInstance();
            //DataInstance.Properties = new List<EntityInstanceProperty>();
            //RelationshipColumns = new List<ModelEntites.RelationshipColumnDTO>();
            //  ChildRelationshipDatas = new List<ChildRelationshipInfo>();
            ChildSimpleContorlProperties = new List<ChildSimpleContorlProperty>();
            ChangeMonitorItems = new List<ChangeMonitor>();

            SetProperties();
            //StateIds = new List<int>();
            // this.IsReadonlyBecauseOfStateChanged += DP_FormDataRepository_IsReadonlyBecauseOfStateChanged;
            //    GUID = Guid.NewGuid();

            //    DataTypes = new List<DP_FormDataRepository>();
            //ViewEntityProperties = new List<Tuple<int, List<ProxyLibrary.EntityInstanceProperty>>>();
        }

        private void Property_PropertyValueChanged(object sender, PropertyValueChangedArg e)
        {
            if (PropertyValueChanged != null)
            {
                e.DataItem = this;
                PropertyValueChanged(this, e);
            }
        }
        public bool ExcludeFromDataEntry { set; get; }
        //   public bool IsFullData { set; get; }
        //public List<int> ViewEntityColumns { set; get; }
        //    public DP_DataView DataView { set; get; }
        public List<ChildSimpleContorlProperty> ChildSimpleContorlProperties { set; get; }
        public bool ToParentRelationshipIsReadonly
        {
            get
            {
                return ToParentRelationshipReadonlyStateItems.Any();
                // ||
                //(ToParantChildRelationshipData != null
                //&& ToParantChildRelationshipData.ToRelationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary
                // );//&& IsReadonlyOnState);
            }
        }
        public bool ToParentRelationshipIsHidden
        {
            get
            {
                return ToParentRelationshipHiddenStateItems.Any();
            }
        }
        //private void Property_PropertyValueChanged(object sender, PropertyValueChangedArg e)
        //{
        //    //if (ChangeMonitorItems.Any(x => x.columnID != 0 && string.IsNullOrEmpty(x.RestTail)))
        //    //{
        //    //    foreach (var item in ChangeMonitorItems.Where(x => x.columnID != 0 && string.IsNullOrEmpty(x.RestTail)))
        //    //    {
        //    //        if (e.ColumnID == item.columnID)
        //    //            item.DataToCall.OnRelatedDataOrColumnChanged(item);
        //    //    }
        //    //}

        //    if (PropertyValueChanged != null)
        //    {
        //        e.DataItem = this;
        //        PropertyValueChanged(this, e);
        //    }
        //}
        public void SetProperties()
        {
            if (IsFullData)
            {
                foreach (var simpleColumn in EditEntityArea.SimpleColumnControls)
                {
                    //var childProperty = ChildSimpleContorlProperties.FirstOrDefault(x => x.SimpleColumnControl == simpleColumnControl);
                    //if (childProperty == null)
                    //{
                    //    childProperty = new ChildSimpleContorlProperty(simpleColumnControl, this, property);
                    //    ChildSimpleContorlProperties.Add(childProperty);
                    //}

                    //childProperty.Binded();

                    var property = Properties.First(x => x.ColumnID == simpleColumn.Column.ID);
                    var simpleProperty = new ChildSimpleContorlProperty(simpleColumn, this, property);
                    ChildSimpleContorlProperties.Add(simpleProperty);
                    if (ListTempSimplePropertyReadonly.Any(x => x.Item1 == simpleColumn.Column.ID))
                    {
                        var item = ListTempSimplePropertyReadonly.First(x => x.Item1 == simpleColumn.Column.ID);
                        simpleProperty.AddReadonlyState(item.Item2, item.Item3, item.Item4);
                    }
                }

                foreach (var relationshipColumnControl in EditEntityArea.RelationshipColumnControls)
                {
                    //var childProperty = ChildSimpleContorlProperties.FirstOrDefault(x => x.SimpleColumnControl == simpleColumnControl);
                    //if (childProperty == null)
                    //{
                    //    childProperty = new ChildSimpleContorlProperty(simpleColumnControl, this, property);
                    //    ChildSimpleContorlProperties.Add(childProperty);
                    //}

                    //childProperty.Binded();

                    var childRelationshipInfo = new ChildRelationshipInfo(relationshipColumnControl, this);
                    base.ChildRelationshipDatas.Add(childRelationshipInfo);
                    if (ListTempRelationshipPropertyReadonly.Any(x => x.Item1 == relationshipColumnControl.Relationship.ID))
                    {
                        var item = ListTempRelationshipPropertyReadonly.First(x => x.Item1 == relationshipColumnControl.Relationship.ID);
                        childRelationshipInfo.AddReadonlyState(item.Item2, item.Item3, item.Item4);
                    }
                    //     ChildRelationshipDatas.Add(childRelationshipInfo);
                    CheckChildRelationshipInfoChangeMonitor();
                    //  return childRelationshipInfo;

                    //var property = baseData.Properties.FirstOrDefault(x => x.ColumnID == simpleColumn.Column.ID);
                    //if (property != null)
                    //    ChildSimpleContorlProperties.Add(new ChildSimpleContorlProperty(simpleColumn, this, property));
                }
            }
        }

        //private void DP_FormDataRepository_IsReadonlyBecauseOfStateChanged(object sender, EventArgs e)
        //{
        //    if (DataIsInEditMode())
        //    {
        //        EditEntityArea.DecideButtonsEnablity1();
        //        if (ParantChildRelationshipInfo != null)
        //        {
        //            ParantChildRelationshipInfo.SourceData.EditEntityArea.DecideButtonsEnablity1();
        //        }
        //        foreach (var item in ChildRelationshipDatas)
        //        {
        //            item.RelationshipControl.EditNdTypeArea.DecideButtonsEnablity1();
        //        }
        //    }
        //}

        private void DP_FormDataRepository_PropertyValueChanged(object sender, PropertyValueChangedArg e)
        {
            if (ChangeMonitorItems.Any(x => x.columnID != 0 && string.IsNullOrEmpty(x.RestTail)))
            {
                foreach (var item in ChangeMonitorItems.Where(x => x.columnID != 0 && string.IsNullOrEmpty(x.RestTail)))
                {
                    if (e.ColumnID == item.columnID)
                        item.DataToCall.OnRelatedDataOrColumnChanged(item);
                }
            }
            //if (PropertyValueChanged != null)
            //{
            //    e.DataItem = this;
            //    PropertyValueChanged(this, e);
            //}
        }
        public bool ChangeMonitorExists(string generalKey, string usageKey)
        {
            return ChangeMonitorItems.Any(x => x.GeneralKey == generalKey && x.UsageKey == usageKey);
        }

        public void AddChangeMonitor(string generalKey, string usageKey, string restTail, int columnID = 0, DP_FormDataRepository dataToCall = null)
        {
            if (string.IsNullOrEmpty(restTail) && columnID == 0)
                return;
            if (dataToCall == null)
                dataToCall = this;

            ChangeMonitorItems.Add(new ChangeMonitor()
            {
                GeneralKey = generalKey,
                UsageKey = usageKey,
                DataToCall = dataToCall,
                columnID = columnID,
                RestTail = restTail
            });
            if (!string.IsNullOrEmpty(restTail))
                CheckChildRelationshipInfoChangeMonitor();
        }


        public void RemoveChangeMonitorByGenaralKey(string key)
        {
            foreach (var item in ChangeMonitorItems.Where(x => x.GeneralKey == key).ToList())
            {
                ChangeMonitorItems.Remove(item);
                foreach (var childRelationshipInfo in ChildRelationshipDatas)
                {
                    childRelationshipInfo.RemoveChangeMonitorByGenaralKey(key);
                }

                if (ParantChildRelationshipData != null)
                {
                    ParantChildRelationshipData.ParantChildRelationshipInfo.RemoveChangeMonitorByGenaralKey(key);
                }
            }
        }

        //public DP_FormDataRepository ShallowCopy()
        //{
        //    return (DP_FormDataRepository)this.MemberwiseClone();
        //}

        //public int TargetEntityID;
        //public bool Edited { set; get; }
        //public bool RecentlyEdited { set; get; }

        //public bool DataRelationshipIsReadonly
        //{
        //    get
        //    {
        //        return ParantChildRelationshipInfo != null && (
        //            (ParantChildRelationshipInfo.IsReadonly || ParantChildRelationshipInfo.ParantChildRelationshipInfo.IsReadonly)
        //            || (ParantChildRelationshipInfo.ToRelationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
        //                        (EditEntityArea.DataEntryEntity.IsReadonly || IsReadonlyOnState)));

        //    }

        //}
        public new object GetValueSomeHow(EntityRelationshipTailDTO valueRelationshipTail, int valueColumnID)
        {
            if (valueRelationshipTail == null)
            {
                var proprty = GetProperty(valueColumnID);
                return proprty?.Value;
            }
            else
            {
                DP_FormDataRepository relatedData = null;
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


        //public EntityInstanceProperty GetOriginalProperty(int columnID)
        //{
        //    return OriginalProperties.FirstOrDefault(x => x.ColumnID == columnID);
        //}
        //public void AddProperty(EntityInstanceProperty property)
        //{
        //    Properties.Add(property);
        //}



        internal void OnRelatedDataOrColumnChanged(ChangeMonitor item)
        {
            if (RelatedDataTailOrColumnChanged != null)
            {
                //var changeMonitor = ChangeMonitorItems.First(x => x.Key == item.Key);
                RelatedDataTailOrColumnChanged(this, item);
            }
        }

        //internal void OnRelatedDataTailChanged(ChangeMonitor item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void RemoveDataObserver(string key, string relationshipTail, DP_FormDataRepository target)
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

        //        foreach (var item in ChildRelationshipDatas)
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





        private void SplitRelationshipTail(string changingRelationshipTail, ref string firstRel, ref string rest)
        {
            if (changingRelationshipTail.Contains(','))
            {
                var splt = changingRelationshipTail.Split(",".ToCharArray(), 2);
                firstRel = splt[0];
                rest = splt[1];
            }
            else
            {
                firstRel = changingRelationshipTail;
            }
        }



        //public void AddProperty(ColumnDTO column, object value)
        //{
        //    EntityInstanceProperty property = new ProxyLibrary.EntityInstanceProperty(column);
        //    //property.Name = column.Name;
        //    property.PropertyValueChanged += Property_PropertyValueChanged;
        //    property.Value = value;
        //    Properties.Add(property);
        //    OriginalProperties.Add(CopyProperty(property));
        //}
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
        //public EntityInstanceProperty AddCopyProperty(EntityInstanceProperty currrentProperty)
        //{
        //    var prop = CopyProperty(currrentProperty);
        //    Properties.Add(prop);
        //    return prop;
        //}

        //private EntityInstanceProperty CopyProperty(EntityInstanceProperty currrentProperty)
        //{
        //    EntityInstanceProperty property = new ProxyLibrary.EntityInstanceProperty(currrentProperty.Column);
        //    //property.Name = currrentProperty.Name;
        //    property.Value = currrentProperty.Value;
        //    property.IsHidden = currrentProperty.IsHidden;
        //    property.ISFK = currrentProperty.ISFK;
        //    property.IsReadonlyFromState = currrentProperty.IsReadonlyFromState;
        //    property.PropertyValueChanged += Property_PropertyValueChanged;
        //    property.FormulaID = currrentProperty.FormulaID;
        //    property.FormulaException = currrentProperty.FormulaException;
        //    property.FormulaUsageParemeters = currrentProperty.FormulaUsageParemeters;
        //    return property;
        //}




        //public override List<EntityInstanceProperty> KeyProperties
        //{
        //    get
        //    {
        //        var listKey = new List<EntityInstanceProperty>();
        //        if (IsFullData || DataView == null)
        //            listKey = Properties.Where(x => x.IsKey).ToList();
        //        else
        //        {

        //            listKey = DataView.Properties.Where(x => string.IsNullOrEmpty(x.RelationshipIDTailPath) && x.IsKey).ToList();
        //        }
        //        //if (listKey.Count == 0)
        //        //    throw new Exception("dfsdf");
        //        return listKey;
        //    }
        //}

        //public List<EntityInstanceProperty> GetProperties()
        //{
        //    return Properties;
        //    //if (IsFullData || DataView == null)
        //    //    return Properties;
        //    //else
        //    //{

        //    //    return DataView.Properties;
        //    //}

        //}
        //List<EntityInstanceProperty> Properties { set; get; }
        //List<EntityInstanceProperty> OriginalProperties { set; get; }
        //public List<int> StateIds { set; get; }


        //public DP_FormDataRepository PairData { set; get; }



        // public Guid GUID;
        //  public bool IsNewItem;



        //public EntityInstance DataInstance;

        //public bool ValueChanged { set; get; }

        //public string Info
        //{
        //    get
        //    {
        //        string info = "";
        //        if (!string.IsNullOrEmpty(TargetEntityAlias))
        //            info += TargetEntityAlias + ", ";
        //        foreach (var item in Properties)
        //        {
        //            info += (info == "" ? "" : Environment.NewLine) + item.Name + " : " + item.Value;
        //        }
        //        return info;
        //    }
        //}



        //   public string Error { get; set; }

        //public bool ShouldWriteSimpleColumnsQuery { get; set; }
        public bool ISValid { get; set; }

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
        //        if (ChildRelationshipDatas.Any(x => x.RelatedData.Any(y => y.HasDirectData)))
        //            return true;
        //        return false;
        //    }
        //}

        //باید تو این شرط زیر هم چک شود
        //!item.KeyProperties.All(y => OriginalRelatedData.Any(z => z.KeyProperties.Any(u => u.ColumnID == y.ColumnID && u.Value == y.Value))
        //که اگر داده دوباره انتخاب شد
        //  public bool IsDBRelationship { get; set; }
        public bool IsEmptyOneDirectData { get; set; }
        public bool? DataOrRelatedDataIsChanged { get; set; }
        //public bool RelationshipIsRemoved { get

        //    {

        //    }
        //}
        //public bool RelationshipIsAdded { get;  }
        //     public bool IsEdited { get; set; }

        //  public event EventHandler IsReadonlyBecauseOfStateChanged;
        //  bool _IsReadonlyBecauseOfState;
        //public bool IsReadonlyOnState
        //{
        //    get
        //    {
        //        return ReadonlyStateItems.Any();
        //    }
        //}


        // public bool IsReadonlyBecauseOfCreatorRelationshipOnState { get; set; }
        //   public bool IsReadonlyBecauseOfCreatorRelationshipOnShow { set; get; }

        //public bool IsReadonly
        //{
        //    get
        //    {
        //        return IsReadonlyBecauseOfCreatorRelationshipOnState || IsReadonlyBecauseOfCreatorRelationshipOnShow || IsReadonlyBecauseOfState;
        //    }
        //}

        //      public bool IsHiddenBecauseOfCreatorRelationshipOnState { set; get; }
        public bool ShoudBeCounted
        {
            get
            {
                if (IsEmptyOneDirectData)
                    return false;
                else if (IsUseLessBecauseNewAndReadonly)
                    return false;
                else if (ParantChildRelationshipData != null && ToParentRelationshipIsHidden)
                    return false;
                else if (ParantChildRelationshipData != null && ToParentRelationshipIsReadonly && !IsDBRelationship)
                    return false;
                //else if (IsReadonly && !IsDBRelationship)
                //    return false;
                return true;
            }
        }

        public bool IsUseLessBecauseNewAndReadonly { get; set; }

        //public bool IsHiddenBecauseOfCreatorRelationshipOnShow { set; get; }
        //public bool IsHidden
        //{
        //    get
        //    {
        //        return IsHiddenBecauseOfCreatorRelationshipOnState || IsHiddenBecauseOfCreatorRelationshipOnShow;
        //    }
        //}

        public List<ChangeMonitor> ChangeMonitorItems { set; get; }
        public bool IsDefaultData { get; internal set; }
        //public int ToParentRelationshipID { get; internal set; }

        // public bool FromDB { get; internal set; }


        //public bool ShouldWriteUpdateQuery { get; set; }

        //public void OnUpdated()
        //{
        //    if (Updated != null)
        //        Updated(this, null);
        //}

        //public ChildRelationshipInfo AddChildRelationshipInfo(RelationshipColumnControlGeneral relationshipColumnControl)
        //{

        //}

        private void CheckChildRelationshipInfoChangeMonitor()
        {
            if (ChangeMonitorItems != null)
            {
                foreach (var item in ChangeMonitorItems.ToList())
                {
                    if (!string.IsNullOrEmpty(item.RestTail))
                    {
                        string firstRel = "", Rest = "";
                        SplitRelationshipTail(item.RestTail, ref firstRel, ref Rest);
                        bool observerSet = false;
                        foreach (var childRelationshipInfo in ChildRelationshipDatas)
                        {

                            if (childRelationshipInfo.Relationship.ID.ToString() == firstRel)
                            {
                                childRelationshipInfo.AddChangeMonitor(item.GeneralKey, item.UsageKey, Rest, item.columnID, item.DataToCall);
                                ChangeMonitorItems.Remove(item);
                                observerSet = true;
                            }
                        }
                        if (!observerSet)
                        {
                            if (ParantChildRelationshipData != null)
                            {
                                //مطمئن نیستم این چک ستون درست باشه بعداً اضافه شده. بیشتر بررسی شود و چون وقتی ستون مدنظر نباشد
                                // و تغییر رابطه بخواد مونیتور بشه چم کردن پرنتها بیهوده است. چون پرنت عوض شه کلا همه چی عوض میشه
                                //if (item.columnID != 0)
                                //{
                                if (ParantChildRelationshipData.ToParentRelationshipID.ToString() == firstRel)
                                {
                                    ParantChildRelationshipData.SourceData.AddChangeMonitor(item.GeneralKey, item.UsageKey, Rest, item.columnID, item.DataToCall);
                                    ChangeMonitorItems.Remove(item);
                                    observerSet = true;

                                }
                                //}
                            }
                        }
                    }
                }
            }




        }

        public bool DataIsInEditMode()
        {

            //if (EditEntityArea.GetDataList().Any(x => x == this))
            //   {
            bool hasTempView = (EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
      EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
       EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.Select);
            if (hasTempView)
            {
                if (EditEntityArea.DataViewGeneric != null && EditEntityArea.DataViewGeneric.IsOpenedTemporary)
                    return true;
                else
                    return false;
            }
            else
            {
                if (EditEntityArea.AreaInitializer.SourceRelationColumnControl == null)
                {
                    if (EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect || EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return this.ParantChildRelationshipData.SourceData.DataIsInEditMode();
                }
            }
            //    }
            //return false;
        }
        List<Tuple<int, string, string, bool>> ListTempSimplePropertyReadonly = new List<Tuple<int, string, string, bool>>();
        internal void AddTempSimplePropertyReadonly(int id, string key, string title, bool permanent)
        {
            ListTempSimplePropertyReadonly.Add(new Tuple<int, string, string, bool>(id, key, title, permanent));
        }
        List<Tuple<int, string, string, bool>> ListTempRelationshipPropertyReadonly = new List<Tuple<int, string, string, bool>>();
        internal void AddTempRelationshipPropertyReadonly(int id, string key, string title, bool permanent)
        {
            ListTempRelationshipPropertyReadonly.Add(new Tuple<int, string, string, bool>(id, key, title, permanent));
        }
        public bool DataItemIsInViewMode()
        {
            return DataIsInEditMode() || DataItemIsInTempViewMode();
        }
        public bool DataItemIsInTempViewMode()
        {
            if (EditEntityArea.GetDataList().Any(x => x == this))
            {
                bool hasTempView = (EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
          EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
            EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.Select);
                //if ((DataView == null || DataView.IsOpenedTemporary == false) && (AreaInitializer.SourceRelationColumnControl == null || AreaInitializer.SourceRelationColumnControl.SourceEditArea.DataItemIsInEditMode(dataItem.ParantChildRelationshipInfo.SourceData)))
                if (hasTempView && (EditEntityArea.AreaInitializer.SourceRelationColumnControl == null || ParantChildRelationshipData.SourceData.DataIsInEditMode()))
                    return true;
            }
            return false;
        }

        //public void AddChildSimpleContorlProperty(SimpleColumnControlGenerel simpleColumnControl, EntityInstanceProperty property)
        //{
        //    var childProperty = ChildSimpleContorlProperties.FirstOrDefault(x => x.SimpleColumnControl == simpleColumnControl);
        //    if (childProperty == null)
        //    {
        //        childProperty = new ChildSimpleContorlProperty(simpleColumnControl, this, property);
        //        ChildSimpleContorlProperties.Add(childProperty);
        //    }

        //    childProperty.Binded();
        //}

        //public void SetDataItemParentRelationshipVisiblity(string message, string key, bool permanent)
        //{
        //    bool dataIsInValidMode = DataIsInEditMode() || DataItemIsInTempViewMode();
        //    if (dataIsInValidMode)
        //    {
        //        AddHiddenState(key, message, permanent);
        //        //   var sKey = "needSave";
        //        ParantChildRelationshipInfo.IsHiddenOnState = hidden;
        //        if (permanent)
        //            ParantChildRelationshipInfo.IsHiddenOnShow = hidden;

        //        if (hidden)
        //        {
        //            AddDataItemMessage(message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", key, ControlItemPriority.High);
        //            AddDataItemColor(InfoColor.Red, ControlColorTarget.Background, key, ControlItemPriority.High);
        //            AddDataItemColor(InfoColor.Red, ControlColorTarget.Border, key, ControlItemPriority.High);
        //        }
        //        else
        //        {
        //            RemoveDataItemColor(key);
        //            RemoveDataItemMessageByKey(key);
        //        }
        //        //}
        //        //else if (dataItem.IsReadonlyBecauseOfCreatorRelationshipOnShow)
        //        //{
        //        //    AddDataItemMessage(new DataMessageItem() { CausingDataItem = dataItem, Message = message + Environment.NewLine + "این رابطه بصورت فقط خواندنی بارگذاری شده است، در صورت نیاز به حذف ابتدا عملیات ثبت انجام شود", Key = key, Priority = ControlItemPriority.High });
        //        //}
        //        //}
        //    }
        //    //}
        //}

        //public void ResetDataItemParentRelationshipVisiblity(string key)
        //{
        //    if (DataIsInEditMode())
        //    {
        //        RemoveParentRelationshipHiddenState(key);
        //    }
        //}

        //public void SetDataItemReadonlyFromState(string message, string key)
        //{
        //    if (DataIsInEditMode())
        //    {
        //        AddReadonlyState(key, message);
        //        //if (permanent)
        //        //{
        //        //}

        //    }
        //}
        //public void ResetDataItemReadonlyFromState(string key)
        //{
        //    if (DataIsInEditMode())
        //    {
        //        RemoveReadonlyState(key);
        //    }
        //}

        //public void SetDataItemParentRelationshipReadonly(string message, string key, bool permanent)
        //{

        //        AddParentRelationshipReadonlyState(key, message, permanent);
        //    }
        //}
        //public void ResetDataItemParentRelationshipReadonly(string key)
        //{
        //    if (DataIsInEditMode())
        //    {
        //        if (ToParentRelationshipReadonlyStateItems.Any(x => x.Key == key && x.Permanent == false))
        //            ToParentRelationshipReadonlyStateItems.RemoveAll(x => x.Key == key && x.Permanent == false);
        //        CheckColumnReadonly();
        //        SetMessageAndColor();
        //    }
        //}

        public void AddParentRelationshipReadonlyState(string key, string message, bool permanent)
        {
            if (ToParentRelationshipReadonlyStateItems.Any(x => x.Key == key))
                ToParentRelationshipReadonlyStateItems.Remove(ToParentRelationshipReadonlyStateItems.First(x => x.Key == key));
            ToParentRelationshipReadonlyStateItems.Add(new ControlStateItem(key, message, permanent));

        }

        public void AddParentRelationshipHiddenState(string key, string message, bool permanent)
        {
            if (ToParentRelationshipHiddenStateItems.Any(x => x.Key == key))
                ToParentRelationshipHiddenStateItems.Remove(ToParentRelationshipHiddenStateItems.First(x => x.Key == key));
            ToParentRelationshipHiddenStateItems.Add(new ControlStateItem(key, message, permanent));
        }
        //public void RemoveParentRelationshipHiddenState(string key)
        //{
        //    if (ToParentRelationshipHiddenStateItems.Any(x => x.Key == key && x.Permanent == false))
        //        ToParentRelationshipHiddenStateItems.RemoveAll(x => x.Key == key && x.Permanent == false);
        //    //DecideVisiblity();
        //    //SetMessageAndColor();
        //}
        //private void DecideVisiblity()
        //{

        //}
        //public void AddReadonlyState(string key, string message)
        //{
        //    if (!ReadonlyStateItems.Any(x => x.Key == key))
        //        ReadonlyStateItems.Add(new ControlStateItem(key, message));

        //    SetMessageAndColor();
        //}
        //public void RemoveReadonlyState(string key)
        //{
        //    if (ReadonlyStateItems.Any(x => x.Key == key))
        //        ReadonlyStateItems.RemoveAll(x => x.Key == key);

        //    SetMessageAndColor();
        //}

        //public void SetMessageAndColor()
        //{
        //    List<BaseColorItem> columnControlColorItems = new List<BaseColorItem>();
        //    List<BaseMessageItem> columnControlMessageItems = new List<BaseMessageItem>();
        //    foreach (var item in ToParentRelationshipHiddenStateItems)
        //    {
        //        columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.Red, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, item.Key, ControlItemPriority.High));
        //        columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message + Environment.NewLine + "رابطه داده غیر فعال می باشد و تغییرات اعمال نخواهد شد", ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.High));
        //    }
        //    //foreach (var item in ReadonlyStateItems)
        //    //{
        //    //    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, item.Key, ControlItemPriority.High));
        //    //    columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message + Environment.NewLine + "این داده فقط خواندنی می باشد و تغییرات اعمال نخواهد شد", ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.High));
        //    //}
        //    foreach (var item in ToParentRelationshipReadonlyStateItems)
        //    {
        //        columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, item.Key, ControlItemPriority.High));
        //        columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message + Environment.NewLine + "رابطه داده فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد", ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.High));
        //    }

        //    SetItemColor(columnControlColorItems);
        //    SetItemMessage(columnControlMessageItems);

        //}

        //public void SetItemColor( List<BaseColorItem> columnControlColorItems)
        //{
        //    InfoColor color = GetColor( columnControlColorItems);
        //    var controlManagers = GetControlDataManagers();
        //    foreach (var view in controlManagers)
        //    {
        //        view.SetBorderColor(color);
        //    }
        //}

        //public void SetItemMessage(List<BaseMessageItem> columnControlMessageItems)
        //{
        //    var tooltip = GetTooltip(columnControlMessageItems);
        //    var controlManagers = GetControlDataManagers();
        //    foreach (var view in controlManagers)
        //    {
        //        view.SetTooltip(tooltip);
        //    }
        //}
        //private string GetTooltip(List<BaseMessageItem> columnControlMessageItems)
        //{
        //    var tooltip = "";
        //    foreach (var item in columnControlMessageItems.OrderByDescending(x => x.Priority))
        //        tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
        //    return tooltip;
        //}
        //private InfoColor GetColor( List<BaseColorItem> columnControlColorItems)
        //{
        //    var color = columnControlColorItems.OrderByDescending(x => x.Priority).FirstOrDefault();
        //    if (color != null)
        //        return color.Color;
        //    else
        //        return InfoColor.Default;
        //}


        //private List<DataColorItem> DataItemColorItems = new List<DataColorItem>();
        //private List<DataMessageItem> DataItemMessageItems = new List<DataMessageItem>();
        //public void AddDataItemMessage(string message, string key, ControlItemPriority priority)
        //{
        //    //    baseMessageItem.MultipleDataControlManager = GetControlDataManagers(baseMessageItem.CausingDataItem);
        //    if (!DataItemMessageItems.Any(x => x.Key == key))
        //        DataItemMessageItems.Add(new DataMessageItem(message, key, priority));
        //    SetItemMessage();
        //}

        //public void RemoveDataItemMessageByKey(string key)
        //{
        //    List<DP_FormDataRepository> datas = new List<DP_FormDataRepository>();
        //    foreach (var baseMessageItem in DataItemMessageItems.Where(x => x.Key == key).ToList())
        //    {
        //        DataItemMessageItems.Remove(baseMessageItem);
        //    }
        //    SetItemMessage();
        //}
        //public void SetItemMessage()
        //{

        //    string tooltip = "";

        //    var list = DataItemMessageItems.ToList<BaseMessageItem>();
        //    tooltip = GetTooltip(list);
        //    var controlManagers = GetControlDataManagers();

        //    foreach (var view in controlManagers)
        //    {
        //        view.SetTooltip(this, tooltip);
        //    }

        //}
        //private string GetTooltip(List<BaseMessageItem> MessageItems)
        //{
        //    var tooltip = "";
        //    foreach (var item in MessageItems.OrderBy(x => x.Priority))
        //        tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
        //    return tooltip;
        //}

        //public void AddDataItemColor(InfoColor infoColor, ControlColorTarget controlColorTarget, string key, ControlItemPriority priority)
        //{
        //    if (!DataItemColorItems.Any(x => x.Key == key && x.ColorTarget == controlColorTarget))
        //        DataItemColorItems.Add(new DataColorItem(infoColor, controlColorTarget, key, priority));
        //    SetItemColor(controlColorTarget);
        //}

        //public void RemoveDataItemColor(string key)
        //{
        //    foreach (var baseColorItem in DataItemColorItems.Where(x => x.Key == key).ToList())
        //    {
        //        DataItemColorItems.Remove(baseColorItem);
        //    }
        //    SetItemColor(ControlColorTarget.Background);
        //    SetItemColor(ControlColorTarget.Border);
        //    //      SetItemColor(dataItem, ControlColorTarget.Foreground);
        //}



        //private InfoColor GetColor(List<BaseColorItem> list)
        //{
        //    var color = InfoColor.Null;
        //    foreach (var item in list.Where(x => x.Color != InfoColor.Null).OrderByDescending(x => x.Priority))
        //        color = item.Color;
        //    return color;
        //}
        public void SetColumnValueFromState(List<UIColumnValueDTO> uIColumnValue, EntityStateDTO state, FormulaDTO formula, bool setFkRelColumns)
        {
            if (DataIsInEditMode())
            {

                if (setFkRelColumns == false)
                {
                    string title = "";
                    string key = "";
                    if (state != null)
                    {
                        key = "state" + "_" + state.ID;
                        title = "بر اساس وضعیت" + " " + state.Title;
                    }
                    else if (formula != null)
                    {
                        key = "formula" + "_" + formula.ID;
                        title = "بر اساس فرمول" + " " + formula.Title;
                    }

                    List<Tuple<ChildSimpleContorlProperty, string>> simpleColumnValues = new List<Tuple<ChildSimpleContorlProperty, string>>();
                    List<Tuple<DP_FormDataRepository, RelationshipColumnControlGeneral, Dictionary<int, string>>> relationshipColumnValues = new List<Tuple<DP_FormDataRepository, RelationshipColumnControlGeneral, Dictionary<int, string>>>();

                    foreach (var columnValue in uIColumnValue)
                    {
                        if (!columnValue.EvenIsNotNew && !IsNewItem)
                            continue;

                        var column = GetProperty(columnValue.ColumnID);
                        if (column != null)
                        {
                            if (!columnValue.EvenHasValue && !AgentHelper.ValueIsEmptyOrDefaultValue(column))
                                continue;

                            //اینجا باید بیزینسی ریدونلی شدن داده هم تست شود
                            if (ChildRelationshipDatas.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ColumnID)))
                            {
                                var relationshipColumn = ChildRelationshipDatas.First(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ColumnID));

                                if (!relationshipColumn.Relationship.IsReadonly)
                                {
                                    if (relationshipColumn.Relationship.RelationshipColumns.All(x => uIColumnValue.Any(z => z.ColumnID == x.FirstSideColumnID)))
                                    {
                                        Dictionary<int, string> listColumns = new Dictionary<int, string>();
                                        foreach (var relCol in relationshipColumn.Relationship.RelationshipColumns)
                                        {
                                            listColumns.Add(relCol.FirstSideColumnID, uIColumnValue.First(x => x.ColumnID == relCol.FirstSideColumnID).ExactValue);
                                        }
                                        relationshipColumnValues.Add(new Tuple<DP_FormDataRepository, RelationshipColumnControlGeneral, Dictionary<int, string>>(this, relationshipColumn.RelationshipControl, listColumns));

                                    }
                                }
                            }
                            else if (ChildSimpleContorlProperties.Any(x => x.SimpleColumnControl.Column.ID == column.ColumnID))
                            {
                                //اینجا باید بیزینسی ریدونلی شدن داده هم تست شود
                                var simpleColumn = ChildSimpleContorlProperties.First(x => x.SimpleColumnControl.Column.ID == column.ColumnID);
                                simpleColumnValues.Add(new Tuple<ChildSimpleContorlProperty, string>(simpleColumn, columnValue.ExactValue));
                            }
                        }
                    }

                    foreach (var item in simpleColumnValues)
                    {
                        //اینجا باید بررسی بشه که نوع مقدار و پراپرتی مناسب هستند
                        item.Item1.SetValue(item.Item2);
                    }
                    foreach (var item in relationshipColumnValues)
                    {
                        var childInfo = ChildRelationshipDatas.FirstOrDefault(x => x.Relationship.ID == item.Item2.Relationship.ID);
                        if (childInfo != null)
                        {
                            if (!childInfo.Relationship.IsReadonly && !childInfo.IsReadonlyOnState && !childInfo.IsHidden)
                            {
                                childInfo.SelectFromParent(item.Item3);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var columnValue in uIColumnValue)
                    {
                        if (!columnValue.EvenIsNotNew && !IsNewItem)
                            continue;
                        var property = GetProperty(columnValue.ColumnID);
                        if (!columnValue.EvenHasValue && !AgentHelper.ValueIsEmptyOrDefaultValue(property))
                            continue;
                        //اینجا باید بررسی بشه که نوع مقدار و پراپرتی مناسب هستند
                        //اگر ارتباط یک به یک روی کلید بود چی میشه؟
                        property.Value = columnValue.ExactValue;

                    }
                }
            }
        }

        //public void SetItemColor(ControlColorTarget controlColorTarget)
        //{
        //    InfoColor color = InfoColor.Null;
        //    var list = DataItemColorItems.Where(x => x.ColorTarget == controlColorTarget).ToList<BaseColorItem>();
        //    color = GetColor(list);
        //    var controlManagers = GetControlDataManagers();

        //    //     var list = ControlManagerColorItems.Where(x => x.CausingDataItem == baseColorItem.CausingDataItem && x.ColorTarget == baseColorItem.ColorTarget).ToList<BaseColorItem>();

        //    foreach (var view in controlManagers)
        //    {
        //        if (controlColorTarget == ControlColorTarget.Background)
        //        {
        //            view.SetBackgroundColor(this, color);
        //        }
        //        else if (controlColorTarget == ControlColorTarget.Foreground)
        //        {
        //            view.SetForegroundColor(this, color);
        //        }
        //        if (controlColorTarget == ControlColorTarget.Border)
        //        {
        //            view.SetBorderColor(this, color);
        //        }
        //    }
        //}

        //private List<I_UIElementManager> GetControlDataManagers()
        //{
        //    List<I_UIElementManager> result = new List<I_UIElementManager>();
        //    if (DataIsInEditMode())
        //        result.Add(EditEntityArea.DataViewGeneric);

        //    //اینجا
        //    if (DataItemIsInTempViewMode())
        //    {
        //        if (EditEntityArea is I_EditEntityAreaOneData)
        //        {
        //            //if (EditEntityArea.AreaInitializer.SourceRelationColumnControl == null || EditEntityArea.AreaInitializer.SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaOneData)
        //            //{
        //            //    result.Add(EditEntityArea.TemporaryDisplayView);
        //            //}
        //            //else
        //            //{
        //            //    var relationshipControl = EditEntityArea.AreaInitializer.SourceRelationColumnControl as RelationshipColumnControlMultiple;
        //            //    result.Add(relationshipControl.RelationshipControlManager.GetView(this));
        //            //}
        //        }
        //    }
        //    return result;
        //}
        //private bool DataItemIsTargetOrIsInParents(DP_FormDataRepository targetDataItem, ChildRelationshipInfo parentChildRelationshipInfo)
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





        //public void OnChildRelatedDataChanged(string relationshipTail, DP_FormDataRepository sourceDataItem, PropertyValueChangedArg column)
        //{
        //    if (RelatedDataChanged != null)
        //    {
        //        RelatedDataChanged(this, new ProxyLibrary.RelatedDataChangedArg() { RelationshipTail = relationshipTail, SourceDataItem = sourceDataItem, Column = column });
        //    }
        //    if (ParantChildRelationshipInfo != null)
        //        ParantChildRelationshipInfo.ParentData.OnChildRelatedDataChanged(ParantChildRelationshipInfo.Relationship.ID.ToString() + "," + relationshipTail, sourceDataItem, column);
        //}

        //public void OnParentRelatedDataChanged(string relationshipTail, DP_FormDataRepository sourceDataItem, PropertyValueChangedArg column)
        //{
        //    if (RelatedDataChanged != null)
        //    {
        //        RelatedDataChanged(this, new ProxyLibrary.RelatedDataChangedArg() { RelationshipTail = relationshipTail, SourceDataItem = sourceDataItem, Column = column });
        //    }
        //    foreach (var child in ChildRelationshipDatas)
        //    {
        //        foreach (var item in child.RelatedData)
        //            item.OnParentRelatedDataChanged(child.Relationship.PairRelationshipID.ToString() + "," + relationshipTail, sourceDataItem, column);
        //    }
        //}
    }
    public class PropertyFormulaComment
    {
        public PropertyFormulaComment(string key, string message)
        {
            Key = key;
            Message = message;
        }
        public string Key { get; set; }
        public string Message { get; set; }

    }
    public class ControlStateItem
    {
        //public ControlStateItem(string key, string message)
        //{
        //    Key = key;
        //    Message = message;
        //}
        public ControlStateItem(string key, string message, bool permanent)
        {
            Key = key;
            Message = message;
            Permanent = permanent;
            //    ImposeInUI = imposeInUI;
        }
        public bool Permanent { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        //  public bool ImposeInUI { get; set; }
    }

}
