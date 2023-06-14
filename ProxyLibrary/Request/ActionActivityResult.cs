using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyLibrary.Request
{
    public class BackendActionActivityResult
    {
        public int BackendActionActivityID { set; get; }
        public Exception Exception { set; get; }
     //   public string Message { set; get; }
    }
}
