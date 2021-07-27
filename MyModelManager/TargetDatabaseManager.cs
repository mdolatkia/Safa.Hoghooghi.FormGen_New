using ModelEntites;
using MyConnectionManager;
using MyModelManager;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTargetDatabaseManager
{
    public class TargetDatabaseManager
    {
        public static bool InsertFromQueryToTargetDB(int entityID, string targetTableName, string selectQuery)
        {
            BizDatabase bizDatabase = new BizDatabase();
            var database = bizDatabase.GetDatabaseByEntityID(entityID);
            if (database.DBType == enum_DBType.SQLServer)
            {
                return InsertFromQueryToTargetSQLDB(database, targetTableName, selectQuery);
            }
            return false;
        }

        private static bool InsertFromQueryToTargetSQLDB(DatabaseDTO database, string targetTableName, string selectQuery)
        {
            

            //}
            //}
        }
    }
}
