using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySampleCodeFunctions
{
    public class Class1
    {
        public FunctionResult GetValidMany(CodeFunctionParamManyDataItems Param)
        {
            FunctionResult result = new FunctionResult();

            result.Result = "y";
            try
            {
                Param.DataItems.First().GetProperties().Last().Value = "رشت";
            }
            catch
            {

            }
            return result;
        }

        public FunctionResult GetValidOne(CodeFunctionParamOneDataItem Param)
        {
            FunctionResult result = new FunctionResult();

            result.Result = "y";
            try
            {
                Param.DataItem.GetProperties().Last().Value = "رشت";
            }
            catch
            {

            }
            return result;
        }
        //کمبو شود تا تابع با اسم یکسان بشود
        public FunctionResult GetValidKeyColumn(int id, string date)
        {
            FunctionResult result = new FunctionResult();
            result.Result = date;
            //  result.Result = "y";
            try
            {
                //Param.DataItem.Properties.Last().Value = "رشت";
            }
            catch
            {

            }
            return result;
        }
    }
}
