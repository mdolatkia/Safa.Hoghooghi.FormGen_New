using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    public class BeforeSaveTest
    {
        public FunctionResult EditLegalPersonName(CodeFunctionParamOneDataItem data)
        {
            FunctionResult result = new FunctionResult();
            if (data != null)
            {
                foreach (var property in data.DataItem.Properties.Where(x => x.Column != null && x.Column.Name.ToLower() == "name"
                && x.Column.OriginalColumnType == Enum_ColumnType.String))
                {
                    if (property.Value != null && property.Value.ToString().Contains("شرکت "))
                    {
                        property.Value = property.Value.ToString().Replace("شرکت ", "");
                    }
                }
            }
            return result;
        }

        public FunctionResult CheckRequestIsValid(CodeFunctionParamOneDataItem data)
        {
            FunctionResult result = new FunctionResult();
            if (data.Requester.Identity == 666)
            {
            throw new Exception("کاربر غیر مجاز");
            }

            return result;
        }
    }
}
