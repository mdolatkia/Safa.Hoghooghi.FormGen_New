using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    public class BeforeLoadTest
    {
        public FunctionResult SetUserID(CodeFunctionParamManyDataItems  datas)
        {
            FunctionResult result = new FunctionResult();
            foreach (var data in datas.DataItems)
            {
                if (data != null)
                {
                    var useridColumn = data.Properties.FirstOrDefault(x => x.Column != null && x.Column.Name.ToLower() == "userid");
                    if (useridColumn != null)
                    {
                        useridColumn.Value = datas.Requester.Identity;
                    }
                }
            }
            return result;
        }
    }
}
