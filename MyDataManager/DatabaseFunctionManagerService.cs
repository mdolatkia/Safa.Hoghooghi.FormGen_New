using ModelEntites;

using MyDataManagerBusiness;



using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDataManagerService
{
    public class DatabaseFunctionManagerService
    {

        public FunctionResult GetFunctionValue(DR_Requester requester, int dbFunctionID, DP_DataRepository dataItem)
        {
            DatabaseFunctionHandler handler = new DatabaseFunctionHandler();
            return handler.GetDatabaseFunctionValue(requester, dbFunctionID, dataItem);
        }

    }
}
