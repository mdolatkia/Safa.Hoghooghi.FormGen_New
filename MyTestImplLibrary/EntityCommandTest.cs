using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    public class EntityCommandTest
    {
        public FunctionResult TestCommand(CodeFunctionParamOneDataItem data)
        {
            FunctionResult result = new FunctionResult();
            System.Windows.MessageBox.Show("asdasd");
            return result;
        }
    }
}
