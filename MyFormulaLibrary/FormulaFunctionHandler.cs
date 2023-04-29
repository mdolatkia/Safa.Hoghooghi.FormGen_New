using DynamicExpresso;
using ModelEntites;
using MyCodeFunctionLibrary;
using MyConnectionManager;
using MyDatabaseFunctionLibrary;
using MyDataSearchManagerBusiness;
using MyFormulaFunctionStateFunctionLibrary;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyFormulaFunctionStateFunctionLibrary
{

    public class FormulaFunctionHandler
    {
        //List<FormulaUsageParemetersDTO> FormulaUsageParemeters = new List<FormulaUsageParemetersDTO>();
        BizFormula bizFormula = new BizFormula();
        //FormulaDTO Formula { set; get; }
        public I_ExpressionEvaluator GetExpressionEvaluator(DP_DataRepository dataItem, DR_Requester requester, bool definition, List<int> usedFormulaIDs = null)
        {
            return FormulaInstanceInternalHelper.GetExpressionEvaluator(dataItem, requester, definition, usedFormulaIDs);
        }
        //public FunctionResult CalculateFormula(int formulaID, DP_DataView dataItem, DR_Requester requester, List<int> usedFormulaIDs = null)
        //{
        //    //DP_DataRepository data = new DP_DataRepository(mainDataItem.TargetEntityID, mainDataItem.TargetEntityAlias);
        //    //data.Properties = mainDataItem.Properties;
        //    return CalculateFormula(formulaID,dataItem, requester, usedFormulaIDs);
        //}
        public FunctionResult CalculateFormula(int formulaID, DP_BaseData dataItem, DR_Requester requester, List<int> usedFormulaIDs = null)
        {
            FunctionResult result = null;
            var formula = bizFormula.GetFormula(requester, formulaID, true);
            var mainDataItem = GetMainDateItem(requester, dataItem);
            if (formula.FormulaType == FormulaType.Linear)
            {
                var linearFormula = bizFormula.GetLinearFormula(requester, formulaID, false);
                result = CalculateFormula(linearFormula.FormulaText, mainDataItem, requester, usedFormulaIDs);
            }
            else
            {
                if (formula.FormulaType == FormulaType.CodeFunctionEntity)
                {
                    CodeFunctionHandler codeFunctionHandler = new CodeFunctionHandler();
                    result = codeFunctionHandler.GetCodeFunctionEntityResult(requester, formula.CodeFunctionEntityID, mainDataItem);
                }
                else if (formula.FormulaType == FormulaType.CodeFunction)
                {
                    CodeFunctionHandler codeFunctionHandler = new CodeFunctionHandler();
                    result = codeFunctionHandler.GetCodeFunctionResult(requester, formula.CodeFunctionID, mainDataItem);
                }
                else if (formula.FormulaType == FormulaType.DatabaseFunctionEntity)
                {
                    DatabaseFunctionHandler databaseFunctionHandler = new DatabaseFunctionHandler();
                    result = databaseFunctionHandler.GetDatabaseFunctionValue(requester, formula.DatabaseFunctionEntityID, mainDataItem);
                }
            }
            result.FormulaUsageParemeters = GetFormulaUsageParemeters(requester, formula, mainDataItem);

            return result;
        }

        private List<FormulaUsageParemetersDTO> GetFormulaUsageParemeters(DR_Requester requester, FormulaDTO formula, DP_DataRepository dataItem)
        {
            List<FormulaUsageParemetersDTO> result = new List<FormulaUsageParemetersDTO>();
            foreach (var item in formula.FormulaItems)
            {
                if (item.ItemType == FormuaItemType.Column)
                {
                    FormulaUsageParemetersDTO formulaUsageParemeter = new FormulaUsageParemetersDTO();
                    formulaUsageParemeter.ParameterName = item.ItemTitle;
                    DataitemRelatedColumnValueHandler dataitemRelatedColumnValueHandler = new DataitemRelatedColumnValueHandler();
                    var value = dataitemRelatedColumnValueHandler.GetValueSomeHow(requester, dataItem, item.RelationshipTail, item.ItemID, true);
                    formulaUsageParemeter.ParameterValue = (value != null ? value.ToString() : "<Null>");
                    result.Add(formulaUsageParemeter);
                }
            }
            return result;
        }

        private static DP_DataRepository GetMainDateItem(DR_Requester requester, DP_BaseData dataItem)
        {
            if (dataItem == null)
                return null;
            bool getDataItem = false;
            if (dataItem is DP_DataView)
                getDataItem = true;
            else
            {
                if (!(dataItem as DP_DataRepository).IsNewItem && MyDataHelper.DataItemPrimaryKeysHaveValue((dataItem as DP_DataRepository)) && !MyDataHelper.DataItemNonPrimaryKeysHaveValues((dataItem as DP_DataRepository)))
                    getDataItem = true;
            }

            if (getDataItem)
            {
                SearchRequestManager searchProcessor = new SearchRequestManager();
                DP_SearchRepositoryMain searchDataItem = new DP_SearchRepositoryMain(dataItem.TargetEntityID);
                foreach (var property in dataItem.KeyProperties)
                    searchDataItem.Phrases.Add(new SearchProperty(property.Column) { Value = property.Value });

                //سکوریتی داده اعمال میشود
                //یعنی ممکن است به خود داده دسترسی نداشته باشد و یا حتی به بعضی از فیلدها و روابط
                DR_SearchFullDataRequest request = new DR_SearchFullDataRequest(requester, searchDataItem);
                var result = searchProcessor.Process(request);
                if (result.Result != Enum_DR_ResultType.ExceptionThrown)
                    return result.ResultDataItems.FirstOrDefault(); // searchProcessor.GetDataItemsByListOFSearchProperties(Requester, searchDataItem).FirstOrDefault();
                else
                    throw (new Exception(result.Message));
            }
            else
                return dataItem as DP_DataRepository;
        }
        public FunctionResult CalculateFormulaTest(string expression, DP_DataRepository dataItem, DR_Requester requester, List<int> usedFormulaIDs = null)
        {
            var mainDataItem = GetMainDateItem(requester, dataItem);
            return CalculateFormula(expression, mainDataItem, requester, usedFormulaIDs);
        }
        private FunctionResult CalculateFormula(string expression, DP_DataRepository mainDataItem, DR_Requester requester, List<int> usedFormulaIDs = null)
        {
            FunctionResult result = new FunctionResult();
            //result.FormulaUsageParemeters = FormulaUsageParemeters;
            //FormulaInstance formulaInstance = null;
            try
            {


                var target = FormulaInstanceInternalHelper.GetExpressionEvaluator(mainDataItem, requester, false, usedFormulaIDs);

                result.Result = target.Calculate(expression);

                //formulaInstance = new FormulaInstance(mainDataItem, requester, usedFormulaIDs);
                //formulaInstance.PropertyGetCalled += FormulaInstance_PropertyGetCalled;
                //var instanceResult = formulaInstance.CalculateExpression(expression);
                //if (formulaInstance.Exceptions.Any())
                //    throw new Exception("instance Error");
                //else
                //    result.Result = instanceResult;
                //formulaInstance.PropertyGetCalled -= FormulaInstance_PropertyGetCalled;


            }
            catch (Exception ex)
            {
                result.Exception = new Exception("خطا در محاسبه فرمول" + Environment.NewLine + ex.Message);
            }
            return result;
        }



        //List<MyPropertyInfo> PropertyInfos = new List<MyPropertyInfo>();
        //private void FormulaInstance_PropertyGetCalled(object sender, PropertyGetArg e)
        //{
        //    if (e.PropertyInfo.PropertyType == PropertyType.Relationship)
        //        return;
        //    if (PropertyInfos.Any(x => x == e.PropertyInfo))
        //        return;
        //    if (!IsValidForUsage(e.PropertyInfo))
        //        return;
        //    PropertyInfos.Add(e.PropertyInfo);
        //    FormulaUsageParemetersDTO item = new FormulaUsageParemetersDTO();
        //    item.ParameterName = e.PropertyInfo.Name;
        //    item.ParameterValue = (e.PropertyInfo.Value != null ? e.PropertyInfo.Value.ToString() : "<Null>");
        //    item.RelationshipPropertyTail = e.PropertyInfo.RelationshipPropertyTail;// GetParentTail(e.PropertyInfo);

        //    if (e.FormulaUsageParemeters != null)
        //        foreach (var child in e.FormulaUsageParemeters)
        //        {
        //            FormulaUsageParemeters.Add(item);
        //        }
        //    FormulaUsageParemeters.Add(item);
        //}

        //private bool IsValidForUsage(MyPropertyInfo propertyInfo)
        //{
        //    if (propertyInfo.PropertyType == PropertyType.Helper)
        //        return false;
        //    return true;
        //}

        //private string GetParentTail(MyPropertyInfo propertyInfo, bool original = true)
        //{
        //    var RelationshipTail = "";

        //    if (propertyInfo.ParentProperty != null && propertyInfo.ParentProperty.PropertyType == PropertyType.Relationship)
        //    {
        //        var pKey = GetParentTail(propertyInfo.ParentProperty, false);
        //        //var key = propertyInfo.FormulaObject.DataItem.GUID.ToString();
        //        //foreach (var keyProperty in propertyInfo.FormulaObject.DataItem.KeyProperties)
        //        //{
        //        //    key += (key == "" ? "" : "&") + keyProperty.ColumnID + ":" + keyProperty.Value;
        //        //}
        //        if (!original)
        //            return (pKey != "" ? pKey + ">" : "") + key;
        //        else
        //            return propertyInfo.RelationshipTail + "@" + (pKey != "" ? pKey + ">" : "") + key;


        //    }
        //    return RelationshipTail;
        //}



        //////private string HandleException(Exception ex, FormulaInstance formulaInstance)
        //////{
        //////    var error = ex.Message;
        //////    if (error == null)
        //////        error = "";
        //////    if (formulaInstance != null)
        //////    {
        //////        foreach (var item in formulaInstance.Exceptions)
        //////        {
        //////            error += (error == "" ? "" : "<<>>") + item.Message;
        //////        }
        //////        //برای اکسپشن هندلینگ داخل قرمول اینستنس چون ممکن است تو در تو صدا زده شود باید لیست تهیه و مرتبا لیستها به هم اضافه شوند
        //////    }
        //////    var message = "خطا در محاسبه فرمول" + (error == "" ? "" : Environment.NewLine) + error;
        //////    return message;
        //////}
    }



    //public interface I_ExpressionEvaluator
    //{
    //    FormulaResult Calculate(string expression);
    //}
    //public class ExpressionEvaluator : I_ExpressionEvaluator
    //{
    //    I_Interpreter Interpreter { set; get; }
    //    public ExpressionEvaluator(DP_DataRepository myData, DR_Requester requester, bool definition)
    //    {
    //        Interpreter = FormulaInstanceInternalHelper.GetInterpreter(myData, requester, definition);
    //    }
    //    public FormulaResult Calculate(string expression)
    //    {
    //        return Interpreter.Calculate()
    //    }
    //}
}
