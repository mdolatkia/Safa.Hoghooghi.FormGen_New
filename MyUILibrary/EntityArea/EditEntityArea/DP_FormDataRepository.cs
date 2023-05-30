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
        public new ChildRelationshipInfo ParantChildRelationshipData
        {
            set
            {
                base.ParantChildRelationshipData = value;
                if (!IsDBRelationship)
                {

                }

            }
            get { return base.ParantChildRelationshipData as ChildRelationshipInfo; }
        }
        public new List<ChildRelationshipInfo> ChildRelationshipDatas { get { return base.ChildRelationshipDatas.Cast<ChildRelationshipInfo>().ToList(); } }
        public List<ControlStateItem> ToParentRelationshipReadonlyStateItems = new List<ControlStateItem>();
        public List<ControlStateItem> ToParentRelationshipHiddenStateItems = new List<ControlStateItem>();
        public I_EditEntityArea EditEntityArea { set; get; }
        public EntityListViewDTO EntityListView { get { return EditEntityArea.ViewEntityArea.EntityListView; } }

        public List<ChildSimpleContorlProperty> ChildSimpleContorlProperties { set; get; }
        //List<Tuple<int, string, string, bool>> ListTempSimplePropertyReadonly = new List<Tuple<int, string, string, bool>>();
        List<Tuple<int, string, string, bool>> ListTempRelationshipPropertyReadonly = new List<Tuple<int, string, string, bool>>();
        public DP_FormDataRepository(DP_DataView dataView, I_EditEntityArea editEntityArea, bool isDBRelationship, bool isNewItem)
            : base(dataView)
        {
            Properties = dataView.Properties;
            foreach (var property in Properties)
                property.PropertyValueChanged += Property_PropertyValueChanged;
            //   DataView = baseData;
            GUID = dataView.GUID;
            //باید همیشه فالس باشه؟؟
            IsNewItem = isNewItem;
            IsDBRelationship = isDBRelationship;
            EditEntityArea = editEntityArea;
            OriginalProperties = new List<ProxyLibrary.EntityInstanceProperty>();
            ChildSimpleContorlProperties = new List<ChildSimpleContorlProperty>();
            ChangeMonitorItems = new List<ChangeMonitor>();
        }
        public DP_FormDataRepository(DP_DataRepository dataRepository, I_EditEntityArea editEntityArea, bool isDBRelationship, bool isNewItem)
            : base(dataRepository)
        {
            dataRepository.PropertyValueChanged += DP_FormDataRepository_PropertyValueChanged;
            Properties = dataRepository.Properties;
            foreach (var property in Properties)
                property.PropertyValueChanged += Property_PropertyValueChanged;
            GUID = dataRepository.GUID;
            IsFullData = true;
            IsNewItem = isNewItem;
            IsDBRelationship = isDBRelationship;
            EditEntityArea = editEntityArea;
            OriginalProperties = new List<ProxyLibrary.EntityInstanceProperty>();
            ChildSimpleContorlProperties = new List<ChildSimpleContorlProperty>();
            ChangeMonitorItems = new List<ChangeMonitor>();
           
            SetProperties();
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

        public bool ToParentRelationshipIsReadonly
        {
            get
            {
                return ToParentRelationshipReadonlyStateItems.Any();
            }
        }
        public bool ToParentRelationshipIsHidden
        {
            get
            {
                return ToParentRelationshipHiddenStateItems.Any();
            }
        }

        public void SetProperties()
        {
            if (IsFullData)
            {
                foreach (var simpleColumn in EditEntityArea.SimpleColumnControls)
                {
                    var property = Properties.First(x => x.ColumnID == simpleColumn.DataEntryColumn.ID);
                    var simpleProperty = new ChildSimpleContorlProperty(simpleColumn, this, property);
                    ChildSimpleContorlProperties.Add(simpleProperty);
                    //if (ListTempSimplePropertyReadonly.Any(x => x.Item1 == simpleColumn.DataEntryColumn.ID))
                    //{
                    //    var item = ListTempSimplePropertyReadonly.First(x => x.Item1 == simpleColumn.DataEntryColumn.ID);
                    //    simpleProperty.AddReadonlyState(item.Item2, item.Item3, item.Item4);
                    //}
                }

                foreach (var relationshipColumnControl in EditEntityArea.RelationshipColumnControls)
                {


                    var childRelationshipInfo = new ChildRelationshipInfo(relationshipColumnControl, this);
                    base.ChildRelationshipDatas.Add(childRelationshipInfo);
                    if (ListTempRelationshipPropertyReadonly.Any(x => x.Item1 == relationshipColumnControl.Relationship.ID))
                    {
                        var item = ListTempRelationshipPropertyReadonly.First(x => x.Item1 == relationshipColumnControl.Relationship.ID);
                        childRelationshipInfo.AddReadonlyState(item.Item2, item.Item3, item.Item4);
                    }

                }
            }
        }

        private void DP_FormDataRepository_PropertyValueChanged(object sender, PropertyValueChangedArg e)
        {

            foreach (var item in ChangeMonitorItems.Where(x => x.columnID != 0 && string.IsNullOrEmpty(x.RestTail)))
            {
                if (e.ColumnID == item.columnID)
                    item.DataToCall.OnRelatedDataOrColumnChanged(item);
            }
        }
        public bool ChangeMonitorExists(string generalKey, string usageKey)
        {
            return ChangeMonitorItems.Any(x => x.GeneralKey == generalKey && x.UsageKey == usageKey);
        }

        public void AddChangeMonitorIfNotExists(string generalKey, string usageKey, string restTail, int columnID = 0, DP_FormDataRepository dataToCall = null)
        {
            if (!IsFullData)
                throw new Exception("asdasdasd");
            if (dataToCall == null)
                dataToCall = this;
            if (string.IsNullOrEmpty(restTail) && columnID == 0)
                return;

            if (ChangeMonitorItems.Any(x => x.GeneralKey == generalKey && x.UsageKey == usageKey && x.RestTail == restTail && x.columnID == columnID && x.DataToCall == dataToCall))
                return;

            if (string.IsNullOrEmpty(restTail))
            {
                var item = new ChangeMonitor()
                {
                    SourceData = this,
                    GeneralKey = generalKey,
                    UsageKey = usageKey,
                    DataToCall = dataToCall,
                    columnID = columnID,
                    RestTail = restTail
                };
                ChangeMonitorItems.Add(item);
            }
            else
            {
                string firstRel = "", Rest = "";
                SplitRelationshipTail(restTail, ref firstRel, ref Rest);
                foreach (var childRelationshipInfo in ChildRelationshipDatas)
                {
                    if (childRelationshipInfo.Relationship.ID.ToString() == firstRel)
                    {
                        childRelationshipInfo.AddChangeMonitor(generalKey, usageKey, Rest, columnID, dataToCall);
                        return;
                    }
                }
                if (ParantChildRelationshipData != null)
                {
                    if (ParantChildRelationshipData.ToParentRelationshipID.ToString() == firstRel)
                    {
                        ParantChildRelationshipData.SourceData.AddChangeMonitorIfNotExists(generalKey, usageKey, Rest, columnID, dataToCall);
                    }
                }
            }
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
                    ParantChildRelationshipData.RemoveChangeMonitorByGenaralKey(key);
                }
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
                        relatedData = childInfo.RelatedData.First() as DP_FormDataRepository;
                }
                if (relatedData != null)
                    return relatedData.GetValueSomeHow(valueRelationshipTail.ChildTail, valueColumnID);
                else
                {
                    //////if (DataView != null)
                    //////{
                    //////    if (DataView.Properties.Any(x => x.RelationshipIDTailPath == valueRelationshipTail.RelationshipIDPath && x.ColumnID == valueColumnID))
                    //////    {
                    //////        return DataView.Properties.First(x => x.RelationshipIDTailPath == valueRelationshipTail.RelationshipIDPath && x.ColumnID == valueColumnID).Value;
                    //////    }
                    //////}

                    return null;
                }

            }
            //return "";
        }

        //public override EntityInstanceProperty GetProperty(int columnID)
        //{
        //    return Properties.FirstOrDefault(x => x.ColumnID == columnID);
        //}




        internal void OnRelatedDataOrColumnChanged(ChangeMonitor item)
        {
            if (RelatedDataTailOrColumnChanged != null)
            {
                //var changeMonitor = ChangeMonitorItems.First(x => x.Key == item.Key);
                RelatedDataTailOrColumnChanged(this, item);
            }
        }




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


        public bool ISValid { get; set; }

        public bool? DataOrRelatedDataIsChanged { get; set; }

        public bool ShoudBeCounted
        {
            get
            {
                //if (IsEmptyOneDirectData)
                //    return false;
                //else
                if (IsUseLessBecauseNewAndReadonly)
                    return false;
                else if (ParantChildRelationshipData != null && ToParentRelationshipIsHidden)
                    return false;
                //else if (ParantChildRelationshipData != null && ToParentRelationshipIsReadonly && !IsDBRelationship)
                //    return false;
                //else if (IsReadonly && !IsDBRelationship)
                //    return false;
                return true;
            }
        }

       public bool IsUseLessBecauseNewAndReadonly { get; set; }

        public List<ChangeMonitor> ChangeMonitorItems { set; get; }
        public bool IsDefaultData { get; internal set; }
        public bool IsUpdated { get; internal set; }



        public bool DataIsInEditMode()
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
                if (EditEntityArea.SourceRelationColumnControl == null)
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

        //internal void AddTempSimplePropertyReadonly(int id, string key, string title, bool permanent)
        //{
        //    ListTempSimplePropertyReadonly.Add(new Tuple<int, string, string, bool>(id, key, title, permanent));

        //}

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
                //if ((DataView == null || DataView.IsOpenedTemporary == false) && (SourceRelationColumnControl == null || SourceRelationColumnControl.SourceEditArea.DataItemIsInEditMode(dataItem.ParantChildRelationshipInfo.SourceData)))
                if (hasTempView && (EditEntityArea.SourceRelationColumnControl == null || ParantChildRelationshipData.SourceData.DataIsInEditMode()))
                    return true;
            }
            return false;
        }


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

        public void SetColumnValue(List<UIColumnValueDTO> uIColumnValue, EntityStateDTO state, FormulaDTO formula, bool fromSetFkRelColumns)
        {
            //** DP_FormDataRepository.SetColumnValue: 5d7ba8eeba1a
            if (DataIsInEditMode())
            {
                if (fromSetFkRelColumns == false)
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

                    List<Tuple<ChildSimpleContorlProperty, object>> simpleColumnValues = new List<Tuple<ChildSimpleContorlProperty, object>>();
                    List<Tuple<DP_FormDataRepository, RelationshipColumnControlGeneral, Dictionary<int, object>>> relationshipColumnValues = new List<Tuple<DP_FormDataRepository, RelationshipColumnControlGeneral, Dictionary<int, object>>>();

                    foreach (var columnValue in uIColumnValue)
                    {
                        if (!columnValue.EvenIsNotNew && !IsNewItem)
                            continue;

                        var column = GetProperty(columnValue.ColumnID);
                        if (column != null)
                        {
                            if (!columnValue.EvenHasValue && !column.ValueIsEmptyOrDefaultValue())
                                continue;

                            //اینجا باید بیزینسی ریدونلی شدن داده هم تست شود
                            if (ChildRelationshipDatas.Any(x => !x.IsReadonly && !x.IsHidden && x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ColumnID)))
                            {
                                var relationshipColumn = ChildRelationshipDatas.First(x => !x.IsReadonly && !x.IsHidden && x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ColumnID));

                                if (relationshipColumn.Relationship.RelationshipColumns.All(x => uIColumnValue.Any(z => z.ColumnID == x.FirstSideColumnID)))
                                {
                                    Dictionary<int, object> listColumns = new Dictionary<int, object>();
                                    foreach (var relCol in relationshipColumn.Relationship.RelationshipColumns)
                                    {
                                        listColumns.Add(relCol.FirstSideColumnID, uIColumnValue.First(x => x.ColumnID == relCol.FirstSideColumnID).ExactValue);
                                    }
                                    relationshipColumnValues.Add(new Tuple<DP_FormDataRepository, RelationshipColumnControlGeneral, Dictionary<int, object>>(this, relationshipColumn.RelationshipControl, listColumns));
                                }

                            }
                            else if (ChildSimpleContorlProperties.Any(x => !x.IsReadonly && !x.IsHiddenOnState && x.SimpleColumnControl.DataEntryColumn.ID == column.ColumnID))
                            {
                                //اینجا باید بیزینسی ریدونلی شدن داده هم تست شود
                                var simpleColumn = ChildSimpleContorlProperties.First(x => !x.IsReadonly && !x.IsHiddenOnState && x.SimpleColumnControl.DataEntryColumn.ID == column.ColumnID);
                                simpleColumnValues.Add(new Tuple<ChildSimpleContorlProperty, object>(simpleColumn, columnValue.ExactValue));
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
                            //childInfo.SelectFromParent(item.Item3);


                            if ((childInfo.SourceData as DP_FormDataRepository).DataIsInEditMode())
                            {
                                if (!childInfo.IsReadonly && !childInfo.IsHidden)
                                {
                                    //    RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(this);


                                    List<DP_BaseData> dataitems = new List<DP_BaseData>();
                                    DP_BaseData dataItem = new DP_BaseData(childInfo.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.EntityID, "");
                                    foreach (var citem in childInfo.Relationship.RelationshipColumns)
                                    {
                                        if (item.Item3.ContainsKey(citem.FirstSideColumnID))
                                        {
                                            dataItem.Properties.Add(new EntityInstanceProperty(citem.SecondSideColumn) { Value = item.Item3[citem.FirstSideColumnID] });
                                        }
                                    }
                                    dataitems.Add(dataItem);

                                    childInfo.RelationshipControl.GenericEditNdTypeArea.SelectData(dataitems);

                                    //childInfo.RelationshipControl.GenericEditNdTypeArea.SelectFromParent( childInfo.Relationship, childInfo.SourceData, item.Item3);
                                }
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
                        if (!columnValue.EvenHasValue && !property.ValueIsEmptyOrDefaultValue())
                            continue;
                        //اینجا باید بررسی بشه که نوع مقدار و پراپرتی مناسب هستند
                        //اگر ارتباط یک به یک روی کلید بود چی میشه؟
                        property.Value = columnValue.ExactValue;

                    }
                }
            }
        }

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
