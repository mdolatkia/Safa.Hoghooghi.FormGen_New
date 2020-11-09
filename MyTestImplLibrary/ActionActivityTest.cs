using ModelEntites;

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    public class ActionActivityTest
    {
        public FunctionResult BeforeLoad(CodeFunctionParamManyDataItems param)
        {
            var data = param.DataItems.First();
            data.GetProperty(53).Value = "aaaaaaa";
            return new FunctionResult();
        }
        public FunctionResult BeforeSave(CodeFunctionParamOneDataItem param)
        {
            var data = param.DataItem;
            data.GetProperty(53).Value = "aaaaaaa";
            return new FunctionResult();
        }
        public FunctionResult AfterSave(CodeFunctionParamOneDataItem param)
        {
            //var data = param.DataItems.First();
            //data.GetProperty(53).Value = "aaaaaaa";
            return new FunctionResult();
        }
        public FunctionResult BeforeDelete(CodeFunctionParamOneDataItem param)
        {
            
            return new FunctionResult() { };
        }
        public FunctionResult AfterDelete(CodeFunctionParamOneDataItem param)
        {
            //var data = param.DataItems.First();
            //data.GetProperty(53).Value = "aaaaaaa";
            return new FunctionResult();
        }
    }
}
