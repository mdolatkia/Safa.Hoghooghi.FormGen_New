using ModelEntites;
using MyCodeFunctionLibrary;
using MyDatabaseFunctionLibrary;
using MyDataSearchManagerBusiness;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyFormulaFunctionStateFunctionLibrary
{
    public class MyCustomSingleData : DynamicObject
    {
        CodeFunctionHandler CodeFunctionHandler = new CodeFunctionHandler();
        DatabaseFunctionHandler DatabaseFunctionHandler = new DatabaseFunctionHandler();
        FormulaFunctionHandler FormulaFunctionHandler = new FormulaFunctionHandler();
        StateHandler StateHandler = new StateHandler();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        Dictionary<string, MyPropertyInfo> m_properties = new Dictionary<string, MyPropertyInfo>();
        public IReadOnlyDictionary<string, MyPropertyInfo> Properties { get { return m_properties; } }
        public DP_DataRepository DataItem { set; get; }
        DR_Requester Requester { set; get; }
        bool Definition { set; get; }
        public MyCustomSingleData ParentCustomData { get; private set; }
        public MyPropertyInfo ParentProperty { get; private set; }

        List<int> UsedFormulaIDs = new List<int>();
        public event EventHandler<PropertyCalledArg> PropertyCalled;
        public MyCustomSingleData(DP_DataRepository dataItem, DR_Requester requester, bool definition, Dictionary<string, MyPropertyInfo> properties, List<int> usedFormulaIDs)
        {
            DataItem = dataItem;
            Requester = requester;
            Definition = definition;
            m_properties = properties;
            UsedFormulaIDs = usedFormulaIDs;

            foreach (var item in m_properties)
                item.Value.FormulaObject = this;
        }

        public override bool TryGetMember(GetMemberBinder binder,
                                out object result)
        {
            if (m_properties.ContainsKey(binder.Name))
            {
                var property = m_properties.FirstOrDefault(x => x.Key == binder.Name).Value;
                OnProperyCalled(property);
                if (!property.ValueSearched)
                {
                    property.ValueSearched = true;
                    if (Definition)
                    {
                        if (property.PropertyType == PropertyType.Relationship)
                        {

                            var dataItem = new DP_DataRepository(property.PropertyRelationship.EntityID2, property.PropertyRelationship.Entity2);
                            var entity = bizTableDrivedEntity.GetPermissionedEntity(Requester, dataItem.TargetEntityID);
                            var properties = FormulaInstanceInternalHelper.GetProperties(Requester, entity, property, Definition);


                            //newObject.PropertyGetCalled += BindableTypeDescriptor_PropertyGetCalled;
                            //newObject.PropertySetChanged += FormulaObject_PropertySetChanged;
                            //newObject.PropertyChanged += FormulaObject_PropertyChanged;
                            if (property.PropertyRelationship.TypeEnum == Enum_RelationshipType.OneToMany)
                            {
                                var newObject = GetCustomSingleData(property, dataItem, Requester, Definition, properties, UsedFormulaIDs);
                                //var list = new List<MyCustomSingleData>();
                                //DataItems.Add(newObject);
                                property.Value = new MyCustomMultipleData(Requester, entity.ID, new List<MyCustomSingleData>() { newObject });

                            }
                            else
                            {
                                var newObject = GetCustomSingleData(property, dataItem, Requester, Definition, properties, UsedFormulaIDs);
                                property.Value = newObject;
                            }
                        }
                        else
                        {
                            property.Value = GetPropertyDefaultValue(property);
                        }
                    }
                    else
                    {
                        if (property.PropertyType == PropertyType.Relationship)
                        {
                            var entity = bizTableDrivedEntity.GetPermissionedEntity(Requester, property.PropertyRelationship.EntityID2);
                            var properties = FormulaInstanceInternalHelper.GetProperties(Requester, entity, property, Definition);
                            if (property.PropertyRelationshipProperties == null || property.PropertyRelationshipProperties.Count == 0)
                                throw new Exception("adasdasd");
                            List<DP_DataRepository> relatedDataItems = GetRelatedDataItems(DataItem, property.PropertyRelationship, property.PropertyRelationshipProperties);

                            if (property.PropertyRelationship.TypeEnum == Enum_RelationshipType.OneToMany)
                            {
                                var list = new List<MyCustomSingleData>();

                                foreach (var relatedDataItem in relatedDataItems)
                                {
                                    list.Add(GetCustomSingleData(property, relatedDataItem, Requester, Definition, GetCloneProperties(properties), UsedFormulaIDs));
                                }
                                var newObject = new MyCustomMultipleData(Requester, entity.ID, list);
                                //    DataItems.Add(newObject);
                                //    //newObject.PropertyGetCalled += BindableTypeDescriptor_PropertyGetCalled;
                                //}
                                property.Value = newObject;
                            }
                            else if (relatedDataItems.Any())
                            {

                                var newObject = GetCustomSingleData(property, relatedDataItems.First(), Requester, Definition, properties, UsedFormulaIDs);

                                //newObject.PropertyGetCalled += BindableTypeDescriptor_PropertyGetCalled;
                                //newObject.PropertySetChanged += FormulaObject_PropertySetChanged;
                                //newObject.PropertyChanged += FormulaObject_PropertyChanged;
                                property.Value = newObject;
                            }
                        }
                        else
                        {

                            if (property.PropertyType == PropertyType.Column)
                            {
                                EntityInstanceProperty dataproperty = DataItem.GetProperty(property.ID);
                                if (dataproperty != null)
                                {
                                    property.Value = dataproperty.Value;
                                }
                                else
                                    throw new Exception("Date property" + " " + property.Name + " not found!");
                            }
                            else if (property.PropertyType == PropertyType.FormulaParameter)
                            {
                                var formula = property.Context as FormulaDTO;
                                if (UsedFormulaIDs != null && UsedFormulaIDs.Contains(formula.ID))
                                    throw new Exception("Loop");
                                if (UsedFormulaIDs == null)
                                    UsedFormulaIDs = new List<int>();
                                UsedFormulaIDs.Add(formula.ID);
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {

                                    var value = FormulaFunctionHandler.CalculateFormula(formula.ID, DataItem, Requester, UsedFormulaIDs);
                                    if (value.Exception == null)
                                    {
                                        //if ((property.Context is FormulaDTO) && (property.Context as FormulaDTO).ValueCustomType != ValueCustomType.None)
                                        //    property.Value = GetCustomTypePropertyValue(property, (property.Context as FormulaDTO).ValueCustomType, value.Result);
                                        //else
                                        property.Value = value.Result;
                                        //   e.FormulaUsageParemeters = value.FormulaUsageParemeters;
                                    }
                                    else
                                        throw value.Exception;
                                });

                            }
                            else if (property.PropertyType == PropertyType.State)
                            {
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    DP_DataRepository dataItem = DataItem;

                                    var stateresult = StateHandler.GetStateResult(property.ID, DataItem, Requester);
                                    if (string.IsNullOrEmpty(stateresult.Message))
                                        property.Value = stateresult.Result;
                                    else
                                        throw new Exception(stateresult.Message);
                                });
                            }
                            else if (property.PropertyType == PropertyType.Code)
                            {
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    DP_DataRepository dataItem = DataItem;

                                    var coderesult = CodeFunctionHandler.GetCodeFunctionResult(Requester, property.ID, dataItem);
                                    if (coderesult.Exception == null)
                                    {
                                        property.Value = coderesult.Result;
                                    }
                                    else
                                    {
                                        throw coderesult.Exception;
                                    }

                                    //if ((property.Context is CodeFunctionDTO) && (property.Context as CodeFunctionDTO).ValueCustomType != ValueCustomType.None)
                                    //    property.Value = GetCustomTypePropertyValue(property, (property.Context as CodeFunctionDTO).ValueCustomType, result.Result);
                                    //else


                                });
                            }
                            else if (property.PropertyType == PropertyType.DBFunction)
                            {
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    DP_DataRepository dataItem = DataItem;

                                    var coderesult = DatabaseFunctionHandler.GetDatabaseFunctionValue(Requester, property.ID, dataItem);
                                    if (coderesult.Exception == null)
                                    {
                                        property.Value = coderesult.Result;
                                    }
                                    else
                                    {
                                        throw coderesult.Exception;
                                    }

                                    //if ((property.Context is CodeFunctionDTO) && (property.Context as CodeFunctionDTO).ValueCustomType != ValueCustomType.None)
                                    //    property.Value = GetCustomTypePropertyValue(property, (property.Context as CodeFunctionDTO).ValueCustomType, result.Result);
                                    //else


                                });
                            }
                        }
                    }
                }

                result = property.Value;
                return true;

            }
            else
            {
                throw new Exception("Property" + " " + binder.Name + " not found!");
            }
        }

        private Dictionary<string, MyPropertyInfo> GetCloneProperties(Dictionary<string, MyPropertyInfo> properties)
        {
            Dictionary<string, MyPropertyInfo> result = new Dictionary<string, MyFormulaFunctionStateFunctionLibrary.MyPropertyInfo>();
            foreach (var item in properties)
                result.Add(item.Key, CloneProperty(item.Value));
            return result;
        }

        private MyPropertyInfo CloneProperty(MyPropertyInfo value)
        {
            MyPropertyInfo result = new MyFormulaFunctionStateFunctionLibrary.MyPropertyInfo();
            result.Context = value.Context;
            result.FormulaObject = value.FormulaObject;
            result.ID = value.ID;
            result.Name = value.Name;
            result.ParentProperty = value.ParentProperty;
            result.PropertyRelationship = value.PropertyRelationship;
            result.PropertyType = value.PropertyType;
            result.RelationshipID = value.RelationshipID;
            result.RelationshipLevel = value.RelationshipLevel;
            result.RelationshipPropertyTail = value.RelationshipPropertyTail;
            result.RelationshipTail = value.RelationshipTail;
            result.Tooltip = value.Tooltip;
            result.Type = value.Type;
            result.Value = value.Value;
            result.ValueSearched = value.ValueSearched;

            return result;
        }

        private MyCustomSingleData GetCustomSingleData(MyPropertyInfo property, DP_DataRepository dataItem, DR_Requester requester, bool definition, Dictionary<string, MyPropertyInfo> properties, List<int> usedFormulaIDs)
        {
            var item = new MyCustomSingleData(dataItem, Requester, Definition, properties, UsedFormulaIDs);
            item.ParentCustomData = this;
            item.ParentProperty = property;
            return item;
        }

        private void OnProperyCalled(MyPropertyInfo property, string relationshipTailToProperty = "")
        {
            if (ParentCustomData != null)
            {
                if (string.IsNullOrEmpty(relationshipTailToProperty))
                    relationshipTailToProperty = ParentProperty.PropertyRelationship.ID.ToString();
                else
                    relationshipTailToProperty = ParentProperty.PropertyRelationship.ID + "," + relationshipTailToProperty;

                ParentCustomData.OnProperyCalled(property, relationshipTailToProperty);
            }
            if (PropertyCalled != null)
            {
                PropertyCalled(this, new MyFormulaFunctionStateFunctionLibrary.PropertyCalledArg() { CalledProperty = property, RelationshipPathToTargetProperty = relationshipTailToProperty });
            }
        }



        private List<DP_DataRepository> GetRelatedDataItems(DP_DataRepository dataItem, RelationshipDTO relationship, List<EntityInstanceProperty> relationshipProperties)
        {
            List<DP_DataRepository> result = new List<DP_DataRepository>();
            var childRelationshipInfo = dataItem.ChildRelationshipDatas.FirstOrDefault(x => x.Relationship.ID == relationship.ID);
            //فقط فول ها حسابند چون داده باید کامل باشه که تمامی فیلدها برای محاسبه حاضر باشند
            if (childRelationshipInfo != null && !childRelationshipInfo.RelatedData.Any(x => !x.IsFullData))
                result = childRelationshipInfo.RelatedData.ToList();
            else
            {
                bool gotFromParant = false;
                if (dataItem.ParantChildRelationshipData != null)
                {
                    //اگر داده پرنت بصورت بازگشتی صدا زده شده باشد از پایگاه داده نمی خواند و سعی میکند از همان داده پرنت هر چه که هست استفاده کند

                    //چون غیر از چند به یک همه داده های رابطه موجودند
                    if (dataItem.ParantChildRelationshipData.ToParentRelationship.TypeEnum != Enum_RelationshipType.OneToMany)
                    {
                        if (dataItem.ParantChildRelationshipData.ToParentRelationshipID == relationship.ID)
                        {
                            result.Add(dataItem.ParantChildRelationshipData.SourceData);
                            gotFromParant = true;
                        }
                    }
                }
                if (!gotFromParant)
                {
                    result = new List<DP_DataRepository>();
                    if (!relationshipProperties.Any(x => x.Value == null || string.IsNullOrEmpty(x.Value.ToString())))
                    {
                        SearchRequestManager searchProcessor = new SearchRequestManager();
                        DP_SearchRepositoryMain searchItem = new DP_SearchRepositoryMain(relationship.EntityID2);
                        foreach (var col in relationshipProperties)
                            searchItem.Phrases.Add(new SearchProperty(col.Column) { Value = col.Value });

                        //سکوریتی داده اعمال میشود
                        DR_SearchFullDataRequest request = new DR_SearchFullDataRequest(Requester, searchItem);

                        var searchResult = searchProcessor.Process(request);
                        if (searchResult.Result == Enum_DR_ResultType.SeccessfullyDone)
                            result = searchResult.ResultDataItems; // searchProcessor.GetDataItemsByListOFSearchProperties(Requester, searchDataItem).FirstOrDefault();
                        else if (searchResult.Result == Enum_DR_ResultType.ExceptionThrown)
                            throw (new Exception(searchResult.Message));

                        //result = searchProcessor.GetDataItemsByListOFSearchProperties(Requester, searchItem);

                        //nullدرست شود
                        //childRelationshipInfo = new ChildRelationshipInfo();
                        //childRelationshipInfo.SourceData = dataItem;
                        //childRelationshipInfo.Relationship = relationship;
                        //foreach (var item in result)
                        //    childRelationshipInfo.RelatedData.Add(item);
                    }
                }
            }
            return result;
        }
        public object GetPropertyDefaultValue(MyPropertyInfo propertyInfo)
        {
            if (propertyInfo.Type == typeof(long) || propertyInfo.Type == typeof(long?))
                return (long)1;
            else if (propertyInfo.Type == typeof(int?) || propertyInfo.Type == typeof(int))
                return 1;
            else if (propertyInfo.Type == typeof(short?) || propertyInfo.Type == typeof(short))
                return (short)1;
            else if (propertyInfo.Type == typeof(byte?) || propertyInfo.Type == typeof(byte))
                return (byte)1;
            else if (propertyInfo.Type == typeof(double?) || propertyInfo.Type == typeof(double))
                return (double)1;
            else if (propertyInfo.Type == typeof(decimal?) || propertyInfo.Type == typeof(decimal))
                return (decimal)1;
            else if (propertyInfo.Type == typeof(float?) || propertyInfo.Type == typeof(float))
                return (float)1;
            else if (propertyInfo.Type == typeof(Guid) || propertyInfo.Type == typeof(Guid?))
                return propertyInfo.Name;
            else if (propertyInfo.Type == typeof(string))
                return propertyInfo.Name;
            else if (propertyInfo.Type == typeof(DateTime) || propertyInfo.Type == typeof(DateTime?))
                return DateTime.Now;
            else if (propertyInfo.Type == typeof(bool?) || propertyInfo.Type == typeof(bool))
                return true;

            return propertyInfo.Name;
        }
    }

    public class MyCustomMultipleData
    {
        //  CodeFunctionHandler CodeFunctionHandler = new CodeFunctionHandler();
        //  FormulaFunctionHandler FormulaFunctionHandler = new FormulaFunctionHandler();
        //  StateHandler StateHandler = new StateHandler();
        //   BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        //  Dictionary<string, MyPropertyInfo> m_properties = new Dictionary<string, MyPropertyInfo>();
        // public IReadOnlyDictionary<string, MyPropertyInfo> Properties { get { return m_properties; } }
        List<MyCustomSingleData> DataItems { set; get; }
        public int EntityID { set; get; }

        DR_Requester Requester { set; get; }
        //    bool Definition { set; get; }
        //   List<int> UsedFormulaIDs = new List<int>();
        public MyCustomMultipleData(DR_Requester requester, int entityID, List<MyCustomSingleData> dataItems)
        {
            DataItems = dataItems;
            EntityID = entityID;
            Requester = requester;
            // Requester = requester;
            //  Definition = definition;
            //   m_properties = properties;
            //  UsedFormulaIDs = usedFormulaIDs;
        }
        private Func<MyCustomSingleData, bool> GetExpression(string criteria)
        {
            var keyCriteria = GetKeyAndCriteria(criteria);
            var interpreter = FormulaInstanceInternalHelper.GetExpressionDelegate(EntityID, Requester);
            return interpreter.GetDelegate<Func<MyCustomSingleData, bool>>(keyCriteria.Item2, keyCriteria.Item1);

        }
        private Func<MyCustomSingleData, double> GetExpressionDouble(string criteria)
        {
            var keyCriteria = GetKeyAndCriteria(criteria);
            var interpreter = FormulaInstanceInternalHelper.GetExpressionDelegate(EntityID, Requester);
            return interpreter.GetDelegate<Func<MyCustomSingleData, double>>(keyCriteria.Item2, keyCriteria.Item1);
        }
        private Func<MyCustomSingleData, Int64> GetExpressionInt64(string criteria)
        {
            var keyCriteria = GetKeyAndCriteria(criteria);
            var interpreter = FormulaInstanceInternalHelper.GetExpressionDelegate(EntityID, Requester);
            return interpreter.GetDelegate<Func<MyCustomSingleData, Int64>>(keyCriteria.Item2, keyCriteria.Item1);
        }
        private Tuple<string, string> GetKeyAndCriteria(string criteria)
        {
            string key = "";
            string where = "";
            if (criteria.Contains("=>"))
            {
                var splt = criteria.Split(new string[] { "=>" }, 2, StringSplitOptions.None);
                key = splt[0];
                where = splt[1];
            }
            else
            {
                throw new Exception("Criteria not in proper format : " + criteria);
            }
            return new Tuple<string, string>(key.Trim(), where.Trim());
        }
        public MyCustomSingleData First(string criteria = null)
        {
            try
            {
                return DataItems.First(GetExpression(criteria));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MyCustomSingleData First()
        {
            return DataItems.First();
        }
        public MyCustomSingleData FirstOrDefault(string criteria = null)
        {
            return DataItems.FirstOrDefault(GetExpression(criteria));
        }
        public MyCustomSingleData FirstOrDefault()
        {

            return DataItems.FirstOrDefault();
        }
        public MyCustomSingleData Last(string criteria = null)
        {


            return DataItems.Last(GetExpression(criteria));
        }
        public MyCustomSingleData Last()
        {

            return DataItems.Last();
        }
        public MyCustomSingleData LastOrDefault(string criteria = null)
        {

            return DataItems.LastOrDefault(GetExpression(criteria));
        }
        public MyCustomSingleData LastOrDefault()
        {
            return DataItems.LastOrDefault();
        }
        public bool All(string criteria = null)
        {

            return DataItems.All(GetExpression(criteria));
        }
        public int Count(string criteria = null)
        {

            return DataItems.Count(GetExpression(criteria));
        }
        public int Count()
        {
            return DataItems.Count();
        }
        public bool Any(string criteria = null)
        {

            return DataItems.Any(GetExpression(criteria));
        }
        public bool Any()
        {
            return DataItems.Any();
        }

        public double Sum(string criteria = null)
        {
            return DataItems.Sum(GetExpressionDouble(criteria));
        }

    }
    public class PropertyCalledArg : EventArgs
    {
        public MyPropertyInfo CalledProperty { set; get; }
        public string RelationshipPathToTargetProperty { set; get; }
    }
}
