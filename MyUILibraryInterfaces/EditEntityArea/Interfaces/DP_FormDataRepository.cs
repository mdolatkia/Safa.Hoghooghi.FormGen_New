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
        public event EventHandler<ChangeMonitor> RelatedDataTailOrColumnChanged;
        public ParentRelationshipInfo ParantChildRelationshipInfo { get; set; }
        //public event EventHandler<ChangeMonitor> RelatedDataCollectionChanged;
        //  public event EventHandler<PropertyValueChangedArg> PropertyValueChanged;
        public I_EditEntityArea EditEntityArea { set; get; }
        //      public Dictionary<int, List<ColumnValueRangeDetailsDTO>> ColumnKeyValueRanges = new Dictionary<int, List<ColumnValueRangeDetailsDTO>>();
        public DP_FormDataRepository(DP_DataRepository baseData, I_EditEntityArea editEntityArea) : base(baseData.TargetEntityID, baseData.TargetEntityAlias)
        {
            base.PropertyValueChanged += DP_FormDataRepository_PropertyValueChanged;

            _TargetEntityID = baseData.TargetEntityID;
            _TargetEntityAlias = baseData.TargetEntityAlias;
            Properties = baseData.Properties;
            DataView = baseData.DataView;
            GUID = baseData.GUID;
            IsFullData = baseData.IsFullData;
            IsNewItem = baseData.IsNewItem;
            IsDBRelationship = baseData.IsDBRelationship;

            if (editEntityArea.ChildRelationshipInfo != null)
                ParantChildRelationshipInfo = new ParentRelationshipInfo(editEntityArea.ChildRelationshipInfo);


            EditEntityArea = editEntityArea;

            OriginalProperties = new List<ProxyLibrary.EntityInstanceProperty>();
            //SourceRelatedData = new List<DP_FormDataRepository>();
            //DataInstance = new EntityInstance();
            //DataInstance.Properties = new List<EntityInstanceProperty>();
            //RelationshipColumns = new List<ModelEntites.RelationshipColumnDTO>();
            ChildRelationshipInfos = new List<ChildRelationshipInfo>();
            ChildSimpleContorlProperties = new List<ChildSimpleContorlProperty>();
            ChangeMonitorItems = new List<ChangeMonitor>();
            //StateIds = new List<int>();
            this.IsReadonlyBecauseOfStateChanged += DP_FormDataRepository_IsReadonlyBecauseOfStateChanged;
            //    GUID = Guid.NewGuid();

            //    DataTypes = new List<DP_FormDataRepository>();
            //ViewEntityProperties = new List<Tuple<int, List<ProxyLibrary.EntityInstanceProperty>>>();
        }

        private void DP_FormDataRepository_IsReadonlyBecauseOfStateChanged(object sender, EventArgs e)
        {
            if (DataIsInEditMode())
            {
                EditEntityArea.DecideButtonsEnablity1();
                if(ParantChildRelationshipInfo!=null)
                {
                    ParantChildRelationshipInfo.SourceData.EditEntityArea.DecideButtonsEnablity1();
                }
                foreach(var item in ChildRelationshipInfos)
                {
                    item.RelationshipControl.EditNdTypeArea.DecideButtonsEnablity1();
                }
            }
        }

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
                foreach (var childRelationshipInfo in ChildRelationshipInfos)
                {
                    childRelationshipInfo.RemoveChangeMonitorByGenaralKey(key);
                }

                if (ParantChildRelationshipInfo != null)
                {
                    ParantChildRelationshipInfo.ParantChildRelationshipInfo.RemoveChangeMonitorByGenaralKey(key);
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

        public bool DataRelationshipIsReadonly
        {
            get
            {
                return ParantChildRelationshipInfo != null && (
                    (ParantChildRelationshipInfo.IsReadonly || ParantChildRelationshipInfo.ParantChildRelationshipInfo.IsReadonlyOrRelationshipIsReadonly)
                    || (ParantChildRelationshipInfo.ToRelationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                                (EditEntityArea.DataEntryEntity.IsReadonly || IsReadonlyBecauseOfState)));

            }

        }
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
                if (ParantChildRelationshipInfo != null && ParantChildRelationshipInfo.RelationshipID == valueRelationshipTail.Relationship.ID)
                {
                    relatedData = ParantChildRelationshipInfo.SourceData;
                }
                else if (ChildRelationshipInfos.Any(x => x.Relationship.ID == valueRelationshipTail.Relationship.ID))
                {
                    var childInfo = ChildRelationshipInfos.First(x => x.Relationship.ID == valueRelationshipTail.Relationship.ID);
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

        public bool ExcludeFromDataEntry { set; get; }
        //   public bool IsFullData { set; get; }
        //public List<int> ViewEntityColumns { set; get; }
        //    public DP_DataView DataView { set; get; }
        public List<ChildRelationshipInfo> ChildRelationshipInfos { set; get; }
        public List<ChildSimpleContorlProperty> ChildSimpleContorlProperties { set; get; }
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
        //        if (ChildRelationshipInfos.Any(x => x.RelatedData.Any(y => y.HasDirectData)))
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

        public event EventHandler IsReadonlyBecauseOfStateChanged;
        bool _IsReadonlyBecauseOfState;
        public bool IsReadonlyBecauseOfState
        {
            get { return _IsReadonlyBecauseOfState; }
            set
            {
                if (_IsReadonlyBecauseOfState != value)
                {
                    _IsReadonlyBecauseOfState = value;
                    if (IsReadonlyBecauseOfStateChanged != null)
                        IsReadonlyBecauseOfStateChanged(this, null);
                }
            }
        }
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
                else if (ParantChildRelationshipInfo != null && ParantChildRelationshipInfo.IsHidden)
                    return false;
                else if (ParantChildRelationshipInfo != null && ParantChildRelationshipInfo.IsReadonly && !IsDBRelationship)
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


        //public bool ShouldWriteUpdateQuery { get; set; }

        //public void OnUpdated()
        //{
        //    if (Updated != null)
        //        Updated(this, null);
        //}

        public ChildRelationshipInfo AddChildRelationshipInfo(RelationshipColumnControl relationshipColumnControl)
        {
            var childRelationshipInfo = new ChildRelationshipInfo(relationshipColumnControl, this);
            ChildRelationshipInfos.Add(childRelationshipInfo);
            CheckChildRelationshipInfoChangeMonitor();
            return childRelationshipInfo;
        }

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
                        foreach (var childRelationshipInfo in ChildRelationshipInfos)
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
                            if (ParantChildRelationshipInfo != null)
                            {
                                //مطمئن نیستم این چک ستون درست باشه بعداً اضافه شده. بیشتر بررسی شود و چون وقتی ستون مدنظر نباشد
                                // و تغییر رابطه بخواد مونیتور بشه چم کردن پرنتها بیهوده است. چون پرنت عوض شه کلا همه چی عوض میشه
                                //if (item.columnID != 0)
                                //{
                                if (ParantChildRelationshipInfo.RelationshipID.ToString() == firstRel)
                                {
                                    ParantChildRelationshipInfo.SourceData.AddChangeMonitor(item.GeneralKey, item.UsageKey, Rest, item.columnID, item.DataToCall);
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
            if (EditEntityArea.GetDataList().Any(x => x == this))
            {
                bool hasTempView = (EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
          EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
           EditEntityArea.AreaInitializer.IntracionMode == IntracionMode.Select);
                if (hasTempView)
                {
                    if (EditEntityArea.DataView != null && EditEntityArea.DataView.IsOpenedTemporary)
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
                        return this.ParantChildRelationshipInfo.SourceData.DataIsInEditMode();
                    }
                }
            }
            return false;
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
                if (hasTempView && (EditEntityArea.AreaInitializer.SourceRelationColumnControl == null || ParantChildRelationshipInfo.SourceData.DataIsInEditMode()))
                    return true;
            }
            return false;
        }

        public void AddChildSimpleContorlProperty(SimpleColumnControl simpleColumnControl, EntityInstanceProperty property)
        {
            if (!ChildSimpleContorlProperties.Any(x => x.SimpleColumnControl == simpleColumnControl))
            {
                ChildSimpleContorlProperties.Add(new ChildSimpleContorlProperty(simpleColumnControl, this, property));
            }
        }

        public void ChangeDataItemParentRelationshipVisiblity(bool hidden, string message, string key, bool skipUICheck)
        {
            if (skipUICheck)
            {
                ParantChildRelationshipInfo.IsHidden = hidden;
            }
            else
            {
                bool dataIsInValidMode = DataIsInEditMode() || DataItemIsInTempViewMode();
                if (dataIsInValidMode)
                {
                    //   var sKey = "needSave";
                    ParantChildRelationshipInfo.IsHidden = hidden;

                    if (hidden)
                    {
                        EditEntityArea.AddDataItemMessage(new DataMessageItem() { CausingDataItem = this, Message = message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", Key = key, Priority = ControlItemPriority.High });
                        EditEntityArea.AddDataItemColor(new DataColorItem() { CausingDataItem = this, Color = InfoColor.Red, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                        EditEntityArea.AddDataItemColor(new DataColorItem() { CausingDataItem = this, Color = InfoColor.Red, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });
                    }
                    else
                    {
                        EditEntityArea.RemoveDataItemColor(this, key);
                        EditEntityArea.RemoveDataItemMessage(this, key);
                    }
                    //}
                    //else if (dataItem.IsReadonlyBecauseOfCreatorRelationshipOnShow)
                    //{
                    //    AddDataItemMessage(new DataMessageItem() { CausingDataItem = dataItem, Message = message + Environment.NewLine + "این رابطه بصورت فقط خواندنی بارگذاری شده است، در صورت نیاز به حذف ابتدا عملیات ثبت انجام شود", Key = key, Priority = ControlItemPriority.High });
                    //}
                    //}
                }
            }
        }

        public void ChangeDataItemReadonlyFromState(bool isReadonly, string message, string key, bool skipUICheck)
        {
            if (!skipUICheck)
            {
                if (isReadonly)
                    EditEntityArea.AddDataBusinessMessage(message, Temp.InfoColor.Red, "DataReadonlyByState", this, ControlItemPriority.High);
                else
                    EditEntityArea.RemoveDataBusinessMessage(this, key);
            }
            IsReadonlyBecauseOfState = isReadonly;
        }


        public void ChangeDataItemParentRelationshipReadonly(bool isReadonly, string message, string key, bool skipUICheck)
        {
            if (skipUICheck)
            {
                ParantChildRelationshipInfo.IsReadonlyOnState = isReadonly;
            }
            else
            {
                bool dataIsInValidMode = DataIsInEditMode() || DataItemIsInTempViewMode();
                if (dataIsInValidMode)
                {
                    //var sKey = "needSave";

                    ParantChildRelationshipInfo.IsReadonlyOnState = isReadonly;

                    if (isReadonly)
                    {
                        EditEntityArea.AddDataItemMessage(new DataMessageItem() { CausingDataItem = this, Message = message + Environment.NewLine + "این رابطه فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد", Key = key, Priority = ControlItemPriority.High });
                        //if (!dataItem.IsDBRelationship)
                        //{
                        EditEntityArea.AddDataItemColor(new DataColorItem() { CausingDataItem = this, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                        EditEntityArea.AddDataItemColor(new DataColorItem() { CausingDataItem = this, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });

                        //بعدا باید فانکشنی نوشت که اگر فرم یک داده ای بود دکمه های جستجو حذف غیر فعال بشوند                      
                        //البته الان هم این دکمه ها در صورتی که ریدونلی باشد کار نمیکنند
                    }
                    else
                    {
                        EditEntityArea.RemoveDataItemColor(this, key);
                        EditEntityArea.RemoveDataItemMessage(this, key);
                        if (ParantChildRelationshipInfo.IsReadonlyOnShow)
                        {
                            EditEntityArea.AddDataItemMessage(new DataMessageItem() { CausingDataItem = this, Message = message + Environment.NewLine + "این رابطه بصورت فقط خواندنی بارگذاری شده است، در صورت نیاز به حذف ابتدا عملیات ثبت انجام شود", Key = key, Priority = ControlItemPriority.High });
                        }
                    }


                }
            }
        }

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
        //    foreach (var child in ChildRelationshipInfos)
        //    {
        //        foreach (var item in child.RelatedData)
        //            item.OnParentRelatedDataChanged(child.Relationship.PairRelationshipID.ToString() + "," + relationshipTail, sourceDataItem, column);
        //    }
        //}
    }

}
