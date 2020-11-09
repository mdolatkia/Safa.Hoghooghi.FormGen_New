

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary.DBProductService
{
   public class DateTimeHelper
    {
        public static DateTime GetNow(CodeFunctionParamOneDataItem data)
        {
            return DateTime.Now;
        }
    }
}
