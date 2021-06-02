using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    public class BeforeDeleteTest
    {
        public FunctionResult ValidateServiceRequest(CodeFunctionParamOneDataItem data)
        {
            FunctionResult result = new FunctionResult();
            if (data != null)
            {
                var userIdColumn = data.DataItem.Properties.FirstOrDefault(x => x.Column.Name.ToLower() == "userid".ToLower());
                if (userIdColumn != null)
                {
                    if ((int)userIdColumn.Value != data.Requester.Identity)
                        throw new Exception("شناسه کاربر ایجاد کننده درخواست و کاربر جاری برابر نمی باشد");
                }
                else
                    throw new Exception("ستون شناسه کاربر موجود نمی باشد");
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
