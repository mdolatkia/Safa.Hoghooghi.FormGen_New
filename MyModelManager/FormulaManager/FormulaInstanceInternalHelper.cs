using ModelEntites;
using MyConnectionManager;
using MyDataSearchManagerBusiness;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class FormulaInstanceInternalHelper
    {

        static BizCodeFunction bizCodeFunction = new BizCodeFunction();
        static BizEntityState bizEntityState = new BizEntityState();
        static BizFormula bizFormula = new BizFormula();
        static BizDatabaseFunction bizDatabaseFunction = new BizDatabaseFunction();
        static BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();

        public static Dictionary<string, MyPropertyInfo> GetProperties(DR_Requester requester, TableDrivedEntityDTO entity, MyPropertyInfo parentPropetyInfo, bool definition)
        {
            Dictionary<string, MyPropertyInfo> m_properties = new Dictionary<string, MyPropertyInfo>();
            //روابط
            foreach (var relationship in entity.Relationships.OrderBy(x => x.Name))
            {
                string tooltip = "";
                var name = "";
                if (relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                {
                    name = "OTM_" + relationship.Entity2;// + +relationship.ID;// + "_" + relationship.ID;
                    tooltip = "رابطه یک به چند";
                }
                else if (relationship.TypeEnum == Enum_RelationshipType.ManyToOne)
                {
                    name = "MTO_" + relationship.Entity2;// +  + relationship.ID;// + "_" + relationship.ID;
                    tooltip = "رابطه چند به یک";
                }
                else
                {
                    name = "OTO_" + relationship.Entity2;// +  + relationship.ID;// + "_" + relationship.ID;
                    tooltip = "رابطه یک به یک";
                }
                tooltip += " " + "با" + " " + relationship.Entity2Alias + Environment.NewLine + relationship.Name;
                MyPropertyInfo propertyInfo = GeneratePropertyInfo(entity, PropertyType.Relationship, parentPropetyInfo, relationship.ID, name, relationship);
                //List<MyCustomData> aa;
                //aa.First()
                //else
                //    propertyInfo.RelationshipTail = relationship.ID.ToString();
                if (m_properties.Any(x => x.Key == propertyInfo.Name))
                    SetUniqueName(propertyInfo, m_properties);
                propertyInfo.PropertyRelationship = relationship;
                propertyInfo.Tooltip = tooltip;
                if (relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                    propertyInfo.Type = typeof(MyCustomMultipleData);
                else
                    propertyInfo.Type = typeof(MyCustomSingleData);
                m_properties.Add(propertyInfo.Name, propertyInfo);

            }

            foreach (var column in entity.Columns)
            {
                var name = "cl_" + column.Name;
                MyPropertyInfo propertyInfo = GeneratePropertyInfo(entity, PropertyType.Column, parentPropetyInfo, column.ID, name, column);

                //if (column.DateColumnType != null && column.DateColumnType.IsPersianDate)
                //{
                //    propertyInfo.Type = GetCustomTypePropertyType(propertyInfo, ValueCustomType.IsPersianDate);
                //}
                //else
                propertyInfo.Type = column.DotNetType;

                if (definition)
                {
                    //if (column.DateColumnType != null && column.DateColumnType.IsPersianDate)
                    //{
                    //    propertyInfo.Value = GetCustomTypePropertyDefaultValue(propertyInfo, ValueCustomType.IsPersianDate);
                    //}
                    //else
                    //{
                    propertyInfo.Value = GetPropertyDefaultValue(propertyInfo);
                    //}
                    propertyInfo.ValueSearched = true;
                }
                else
                {
                    propertyInfo.Value = null;
                }
                propertyInfo.Tooltip = "خصوصیت" + " " + column.Alias + Environment.NewLine + "نوع" + ":" + " " + column.DotNetType.ToString();

                m_properties.Add(propertyInfo.Name, propertyInfo);

            }

            //مدل رو تا فولدر فرمول رفتم یا استیت رابطه داره و استیت هم با فرمول چیکار کنم
            var formulaStates = bizEntityState.GetEntityStates(requester, entity.ID);
            foreach (var state in formulaStates)
            {
                var name = "st_" + state.Title;
                MyPropertyInfo propertyInfo = GeneratePropertyInfo(entity, PropertyType.State, parentPropetyInfo, state.ID, name, state);
                propertyInfo.Type = typeof(bool);
                propertyInfo.Name = "st_" + state.Title;
                if (definition)
                {
                    propertyInfo.Value = GetPropertyDefaultValue(propertyInfo);
                    propertyInfo.ValueSearched = true;
                }
                else
                {
                    propertyInfo.Value = null;
                }
                propertyInfo.Tooltip = "وضعیت" + " " + state.Title;
                m_properties.Add(propertyInfo.Name, propertyInfo);
            }


            var formulaParameters = bizFormula.GetFormulas(entity.ID, false);
            foreach (var formulaParameter in formulaParameters)
            {
                var name = "p_" + formulaParameter.Name;
                MyPropertyInfo propertyInfo = GeneratePropertyInfo(entity, PropertyType.FormulaParameter, parentPropetyInfo, formulaParameter.ID, name, formulaParameter);
                //propertyInfo.ParameterFormulaID = formulaParameter.ID;
                //if (formulaParameter.ValueCustomType != ValueCustomType.None)
                //{
                //    propertyInfo.Type = GetCustomTypePropertyType(propertyInfo, formulaParameter.ValueCustomType);
                //}
                //else
                propertyInfo.Type = formulaParameter.ResultDotNetType;
                if (definition)
                {
                    //if (formulaParameter.ValueCustomType != ValueCustomType.None)
                    //{
                    //    propertyInfo.Value = GetCustomTypePropertyDefaultValue(propertyInfo, formulaParameter.ValueCustomType);
                    //}
                    //else
                    //{
                    propertyInfo.Value = GetPropertyDefaultValue(propertyInfo);
                    //}
                    propertyInfo.ValueSearched = true;
                }
                else
                {
                    propertyInfo.Value = null;
                }
                propertyInfo.Tooltip = "فرمول" + " " + formulaParameter.Title + Environment.NewLine + "نوع" + ":" + " " + (formulaParameter.ResultDotNetType != null ? formulaParameter.ResultDotNetType.ToString() : "?");
                m_properties.Add(propertyInfo.Name, propertyInfo);
            }

            var databaseFunctionEntities = bizDatabaseFunction.GetDatabaseFunctionEntityByEntityID(requester, entity.ID);
            foreach (var dbfunction in databaseFunctionEntities)
            {
                var name = "";
                if (dbfunction.DatabaseFunction.Type == Enum_DatabaseFunctionType.Function)
                    name = "fn_" + dbfunction.Name;
                else if (dbfunction.DatabaseFunction.Type == Enum_DatabaseFunctionType.StoredProcedure)
                    name = "sp_" + dbfunction.Name;
                MyPropertyInfo propertyInfo = GeneratePropertyInfo(entity, PropertyType.DBFunction, parentPropetyInfo, dbfunction.ID, name, dbfunction);
                //if (dbfunction.ValueCustomType != ValueCustomType.None)
                //{
                //    propertyInfo.Type = GetCustomTypePropertyType(propertyInfo, dbfunction.ValueCustomType);
                //}
                //else
                propertyInfo.Type = dbfunction.DatabaseFunction.ReturnDotNetType;
                if (definition)
                {
                    //if (dbfunction.ValueCustomType != ValueCustomType.None)
                    //{
                    //    propertyInfo.Value = GetCustomTypePropertyDefaultValue(propertyInfo, dbfunction.ValueCustomType);
                    //}
                    //else
                    //{
                    propertyInfo.Value = GetPropertyDefaultValue(propertyInfo);
                    //}

                    propertyInfo.ValueSearched = true;

                }
                else
                {
                    propertyInfo.Value = null;
                }
                propertyInfo.Tooltip = "تابع پایگاه داده" + " " + dbfunction.Title + Environment.NewLine + "نوع" + ":" + " " + dbfunction.DatabaseFunction.ReturnDotNetType.ToString();
                m_properties.Add(propertyInfo.Name, propertyInfo);
            }


            var codeFunctionEntities = bizCodeFunction.GetCodeFunctionEntityByEntityID(requester, entity.ID);
            foreach (var codeFunction in codeFunctionEntities)
            {
                var name = "cd_" + codeFunction.Name;
                MyPropertyInfo propertyInfo = GeneratePropertyInfo(entity, PropertyType.Code, parentPropetyInfo, codeFunction.ID, name, codeFunction);
                //if (codeFunction.ValueCustomType != ValueCustomType.None)
                //{
                //    propertyInfo.Type = GetCustomTypePropertyType(propertyInfo, codeFunction.ValueCustomType);
                //}
                //else
                propertyInfo.Type = codeFunction.CodeFunction.RetrunDotNetType;
                if (definition)
                {
                    //if (codeFunction.ValueCustomType != ValueCustomType.None)
                    //{
                    //    propertyInfo.Value = GetCustomTypePropertyDefaultValue(propertyInfo, codeFunction.ValueCustomType);
                    //}
                    //else
                    //{
                    propertyInfo.Value = GetPropertyDefaultValue(propertyInfo);
                    //}
                    propertyInfo.ValueSearched = true;
                }
                else
                {
                    propertyInfo.Value = null;
                }
                propertyInfo.Tooltip = "کد تابع" + " " + codeFunction.Title + Environment.NewLine + "نوع" + ":" + " " + codeFunction.CodeFunction.RetrunDotNetType.ToString();
                //فانکشنها در تعریف  فرمول همینجا مقدار میگیرند 
                m_properties.Add(propertyInfo.Name, propertyInfo);
            }
            if (parentPropetyInfo == null)
            {
                //MyPropertyInfo numericHelperPropertyInfo = GeneratePropertyInfo(entity, PropertyType.Helper, parentPropetyInfo, 0, "NumericHelper", null);
                //numericHelperPropertyInfo.Type = typeof(NumericHelper);
                //numericHelperPropertyInfo.Value = new NumericHelper();
                //numericHelperPropertyInfo.ValueSearched = true;
                //m_properties.Add(numericHelperPropertyInfo.Name, numericHelperPropertyInfo);

                ////StringHelper
                //MyPropertyInfo stringHelperPropertyInfo = GeneratePropertyInfo(entity, PropertyType.Helper, parentPropetyInfo, 0, "StringHelper", null);
                //stringHelperPropertyInfo.Type = typeof(StringHelper);
                //stringHelperPropertyInfo.Value = new StringHelper();
                //stringHelperPropertyInfo.ValueSearched = true;
                //m_properties.Add(stringHelperPropertyInfo.Name, stringHelperPropertyInfo);

                //MyPropertyInfo persinaDateHelperPropertyInfo = GeneratePropertyInfo(entity, PropertyType.Helper, parentPropetyInfo, 0, "PersianDateHelper", null);
                //persinaDateHelperPropertyInfo.Type = typeof(PersianDateHelper);
                //persinaDateHelperPropertyInfo.Value = new PersianDateHelper();
                //persinaDateHelperPropertyInfo.ValueSearched = true;
                //m_properties.Add(persinaDateHelperPropertyInfo.Name, persinaDateHelperPropertyInfo);

                //MyPropertyInfo miladiDateHelperPropertyInfo = GeneratePropertyInfo(entity, PropertyType.Helper, parentPropetyInfo, 0, "MiladiDateHelper", null);
                //miladiDateHelperPropertyInfo.Type = typeof(MiladiDateHelper);
                //miladiDateHelperPropertyInfo.Value = new MiladiDateHelper();
                //miladiDateHelperPropertyInfo.ValueSearched = true;
                //m_properties.Add(miladiDateHelperPropertyInfo.Name, miladiDateHelperPropertyInfo);


                //MyPropertyInfo dbFunctionHelperHelperPropertyInfo = GeneratePropertyInfo(entity, PropertyType.Helper, parentPropetyInfo, 0, "DBFunctionHelper", null);
                //dbFunctionHelperHelperPropertyInfo.Type = typeof(DBFunctionHelper);
                //dbFunctionHelperHelperPropertyInfo.Value = new DBFunctionHelper(entity.DatabaseID);
                //dbFunctionHelperHelperPropertyInfo.ValueSearched = true;
                //m_properties.Add(dbFunctionHelperHelperPropertyInfo.Name, dbFunctionHelperHelperPropertyInfo);
            }
            return m_properties;
        }

        private static MyPropertyInfo GeneratePropertyInfo(TableDrivedEntityDTO entity, PropertyType propertyType, MyPropertyInfo parentProperty, int id, string name, object context)
        {

            MyPropertyInfo myPropertyInfo = new MyPropertyInfo();
            myPropertyInfo.Name = name;
            //   myPropertyInfo.FormulaObject = formulaObject;
            myPropertyInfo.ID = id;
            myPropertyInfo.ParentProperty = parentProperty;
            myPropertyInfo.Context = context;
            myPropertyInfo.PropertyType = propertyType;
            if (parentProperty != null)
            {
                // myPropertyInfo.RelationshipTail = parentProperty.RelationshipTail + (string.IsNullOrEmpty(parentProperty.RelationshipTail) ? "" : ",") + relationship.ID;
                myPropertyInfo.RelationshipLevel = parentProperty.RelationshipLevel + 1;

                if (parentProperty.PropertyType == PropertyType.Relationship)
                {
                    myPropertyInfo.RelationshipTail = parentProperty.RelationshipTail + (string.IsNullOrEmpty(parentProperty.RelationshipTail) ? "" : ",") + parentProperty.ID;
                    myPropertyInfo.RelationshipPropertyTail = parentProperty.RelationshipPropertyTail + (string.IsNullOrEmpty(parentProperty.RelationshipPropertyTail) ? "" : ".") + parentProperty.Name;
                }
            }
            else
            {
                myPropertyInfo.RelationshipTail = "";
                myPropertyInfo.RelationshipPropertyTail = "";
            }
            return myPropertyInfo;
        }

        private static void SetUniqueName(MyPropertyInfo propertyInfo, Dictionary<string, MyPropertyInfo> m_properties, int index = 0)
        {
            if (m_properties.Any(x => x.Key == propertyInfo.Name))
            {
                propertyInfo.Name += (index + 1).ToString();
                SetUniqueName(propertyInfo, m_properties, index + 1);
            }
        }
        public static object GetPropertyDefaultValue(MyPropertyInfo propertyInfo)
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
        static I_ExpressionHandler _GetExpressionHandler;
        private static I_ExpressionHandler GetExpressionHandler
        {
            get
            {
                if (_GetExpressionHandler == null)
                    _GetExpressionHandler = new DynamicExpressoExpressionHandler();
                return _GetExpressionHandler;
            }
        }
        private static List<Type> GetRefTypes()
        {
            List<Type> result = new List<Type>();
            result.Add(typeof(BooleanHelper));
            result.Add(typeof(NumericHelper));
            result.Add(typeof(StringHelper));
            result.Add(typeof(PersianDateHelper));
            result.Add(typeof(MiladiDateHelper));
            //result.Add(typeof(MyExtensions));

            return result;
        }
        //public static Dictionary<string, Type> GetExpressionBuiltinVariables()
        //{
        //    return GetExpressionHandler.GetExpressionBuiltinVariables();
        //}
        public static I_ExpressionEvaluator GetExpressionEvaluator(DP_DataRepository mainDataItem, DR_Requester requester, bool definition, List<int> usedFormulaIDs)
        {
            Dictionary<string, MyPropertyInfo> properties = new Dictionary<string, MyPropertyInfo>();
            List<object> variables = new List<object>();
            if (mainDataItem != null)
            {
                var entity = bizTableDrivedEntity.GetPermissionedEntity(requester, mainDataItem.TargetEntityID);
                properties = GetProperties(requester, entity, null, definition);
                variables = GetRefObjects(entity, requester);
            }
            MyCustomSingleData formulaObject = new MyCustomSingleData(mainDataItem, requester, definition, properties, usedFormulaIDs);
            var refTypes = GetRefTypes();
            var expressionEcaluator = GetExpressionHandler.GetExpressionEvaluator(formulaObject, refTypes, variables);
            formulaObject.PropertyCalled += (sender, e) => FormulaObject_PropertyCalled(sender, e, expressionEcaluator);
            return expressionEcaluator;
        }

        private static void FormulaObject_PropertyCalled(object sender, PropertyCalledArg e, I_ExpressionEvaluator expressionEcaluator)
        {
            expressionEcaluator.OnPropertyCalled(sender, e);
        }

        //private static DP_DataRepository GetMainDateItem(DR_Requester requester, DP_DataRepository mainDataItem)
        //{
        //    if (!mainDataItem.IsNewItem && MyDataHelper.DataItemPrimaryKeysHaveValue(mainDataItem) && !MyDataHelper.DataItemNonPrimaryKeysHaveValues(mainDataItem))
        //    {
        //        SearchRequestManager searchProcessor = new SearchRequestManager();
        //        DP_SearchRepositoryMain searchDataItem = new DP_SearchRepositoryMain(mainDataItem.TargetEntityID);
        //        foreach (var property in mainDataItem.GetProperties())
        //            searchDataItem.Phrases.Add(new SearchProperty() { ColumnID = property.ColumnID, Value = property.Value });

        //        //سکوریتی داده اعمال میشود
        //        //یعنی ممکن است به خود داده دسترسی نداشته باشد و یا حتی به بعضی از فیلدها و روابط
        //        DR_SearchFullDataRequest request = new DR_SearchFullDataRequest(requester, searchDataItem);
        //        var result = searchProcessor.Process(request);
        //        if (result.Result != Enum_DR_ResultType.ExceptionThrown)
        //            return result.ResultDataItems.FirstOrDefault(); // searchProcessor.GetDataItemsByListOFSearchProperties(Requester, searchDataItem).FirstOrDefault();
        //        else
        //            throw (new Exception(result.Message));
        //    }
        //    else
        //        return mainDataItem;
        //}

        private static List<object> GetRefObjects(TableDrivedEntityDTO entity, DR_Requester requester)
        {
            List<object> result = new List<object>();
            result.Add(new DBFunctionHelper(entity.DatabaseID, requester));
            result.Add(new UserInfo(requester));
            return result;
        }


        public static I_ExpressionDelegate GetExpressionDelegate(int entityID, DR_Requester requester)
        {
            var entity = bizTableDrivedEntity.GetPermissionedEntity(requester, entityID);
            var refTypes = GetRefTypes();
            var variables = GetRefObjects(entity, requester);
            return GetExpressionHandler.GetExpressionDelegate(refTypes, variables);
        }

        public static string GetObjectPrefrix()
        {
            return GetExpressionHandler.GetObjectPrefrix();
        }

    }
    public interface I_ExpressionHandler
    {
        I_ExpressionEvaluator GetExpressionEvaluator(MyCustomSingleData customData, List<Type> refTypes, List<object> variables);
        I_ExpressionDelegate GetExpressionDelegate(List<Type> refTypes, List<object> variables);
        string GetObjectPrefrix();
    }
    public interface I_ExpressionEvaluator
    {
        event EventHandler<PropertyCalledArg> PropertyCalled;
        List<BuiltinRefClass> GetExpressionBuiltinVariables();
        object Calculate(string expression);
        void OnPropertyCalled(object sender, PropertyCalledArg e);
        MyCustomSingleData MainCustomData { set; get; }
    }
    public interface I_ExpressionDelegate
    {
        T GetDelegate<T>(string expression, string key);
    }
    public class BuiltinRefClass
    {
        public Type Type { set; get; }
        public bool IsType { set; get; }
        public bool IsObject { set; get; }
        public string Name { set; get; }
    }
}
