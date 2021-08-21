using ModelEntites;
using MyGeneralLibrary;

using MyModelManager;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeFunctionLibrary
{
    public class CodeFunctionHandler
    {
        BizCodeFunction bizCodeFunction = new BizCodeFunction();

        //فانکشنها یکی شوند
        public LetterConvertToExternalResult GetLetterSendingCodeFunctionResult(DR_Requester resuester, int codeFunctionID, LetterDTO letter)
        {
            var codeFunction = bizCodeFunction.GetCodeFunction(resuester, codeFunctionID);
            if (codeFunction.ParamType == ModelEntites.Enum_CodeFunctionParamType.LetterConvert)
            {
                var parameters = new List<object>();
                parameters.Add(new LetterFunctionParam(letter, resuester));
                return GetLetterSendingCodeFunctionResult(codeFunction, parameters);

            }
            else
                return null;
        }
        public FunctionResult GetCodeFunctionResult(DR_Requester resuester, int codeFunctionID, LetterDTO letter)
        {
            var codeFunction = bizCodeFunction.GetCodeFunction(resuester, codeFunctionID);
            if (codeFunction.ParamType == ModelEntites.Enum_CodeFunctionParamType.LetterFunction)
            {
                var parameters = new List<object>();
                parameters.Add(new LetterFunctionParam(letter, resuester));
                return GetCodeFunctionResult(codeFunction, parameters);
            }
            else
                return null;
        }
        public FunctionResult GetCodeFunctionEntityResult(DR_Requester resuester, int codeFunctionEntityID, DP_DataRepository dataItem)
        {
            var codeFunctionEntity = bizCodeFunction.GetCodeFunctionEntity(resuester, codeFunctionEntityID);
            var parameters = new List<object>();
            //var formulaUsageParemeters = new List<FormulaUsageParemetersDTO>();
            foreach (var column in codeFunctionEntity.CodeFunctionEntityColumns)
            {
                EntityInstanceProperty property = dataItem.GetProperty(column.ColumnID);
                if (property != null)
                {
                    //FormulaUsageParemetersDTO formulaUsageParemeter = new FormulaUsageParemetersDTO();
                    //formulaUsageParemeter.ParameterName = property.Name;
                    //formulaUsageParemeter.ParameterValue = (property.Value != null ? property.Value.ToString() : "<Null>");
                    //formulaUsageParemeters.Add(formulaUsageParemeter);
                    //stringParamList += (stringParamList == "" ? "" : ",") + column.FunctionColumnParamName;
                    //paramList.Add(column.FunctionColumnParamName);
                    if (property.Value != null)
                        parameters.Add(Convert.ChangeType(property.Value, column.FunctionColumnDotNetType));
                    else
                        parameters.Add(null);

                }
            }
            var result = GetCodeFunctionResult(resuester, codeFunctionEntity.CodeFunctionID, parameters);
            //   result.FormulaUsageParemeters = formulaUsageParemeters;
            return result;
        }
        public FunctionResult GetCodeFunctionResult(DR_Requester resuester, int codeFunctionID, DP_DataRepository dataItem)
        {
            var parameters = new List<object>();
            var codeFunction = bizCodeFunction.GetCodeFunction(resuester, codeFunctionID);
            parameters.Add(new CodeFunctionParamOneDataItem(dataItem, resuester));
            return GetCodeFunctionResult(codeFunction, parameters);
        }

        public FunctionResult GetCodeFunctionResult(DR_Requester resuester, int codeFunctionID, List<DP_DataRepository> allDataItems)
        {
            var codeFunction = bizCodeFunction.GetCodeFunction(resuester, codeFunctionID);
            if (codeFunction.ParamType == ModelEntites.Enum_CodeFunctionParamType.ManyDataItems)
            {
                var parameters = new List<object>() { new CodeFunctionParamManyDataItems(allDataItems, resuester) };
                return GetCodeFunctionResult(codeFunction, parameters);
            }
            else return null;
        }

        public FunctionResult GetCodeFunctionResult(DR_Requester resuester, int codeFunctionID, List<object> parameters)
        {
            var codeFunction = bizCodeFunction.GetCodeFunction(resuester, codeFunctionID);
            return GetCodeFunctionResult(codeFunction, parameters);
        }

        private FunctionResult GetCodeFunctionResult(CodeFunctionDTO codeFunction, List<object> parameters)
        {
            //FunctionResult result = new FunctionResult();
            try
            {
                return (FunctionResult)ReflectionHelper.CallMethod(codeFunction.Path, codeFunction.ClassName, codeFunction.FunctionName, parameters.ToArray());

            }
            catch (Exception ex)
            {
                FunctionResult result = new FunctionResult();
                result.Exception = ex;
                return result;
            }
            // return result;
        }
        private LetterConvertToExternalResult GetLetterSendingCodeFunctionResult(CodeFunctionDTO codeFunction, List<object> parameters)
        {
            try
            {
                var result = ReflectionHelper.CallMethod(codeFunction.Path, codeFunction.ClassName, codeFunction.FunctionName, parameters.ToArray());
                return (LetterConvertToExternalResult)result;
            }
            catch (Exception ex)
            {
                LetterConvertToExternalResult result = new LetterConvertToExternalResult();
                result.Exception = ex;
                return result;
            }
        }

        //private CommandFunctionResult GetCommandFunctionResult(CodeFunctionDTO codeFunction, List<object> parameters)
        //{
        //    var result = ReflectionHelper.CallMethod(codeFunction.Path, codeFunction.ClassName, codeFunction.FunctionName, parameters.ToArray());
        //    return result as CommandFunctionResult;
        //}
        //private LetterFunctionResult GetLetterFunctionResult(CodeFunctionDTO codeFunction, List<object> parameters)
        //{
        //    var result = ReflectionHelper.CallMethod(codeFunction.Path, codeFunction.ClassName, codeFunction.FunctionName, parameters.ToArray());
        //    return result as LetterFunctionResult;
        //}


    }
}
