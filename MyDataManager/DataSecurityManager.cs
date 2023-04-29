using ModelEntites;

using MyDataManagerBusiness;



using MyDataItemManager;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyDataManagerService
{
    public class DataSecurityManager
    {
        BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();

        public bool EntityHasDirectSecurities(DR_Requester requester, int entityID)
        {
            return bizRoleSecurity.EntityHasDirectSecurities(requester, entityID);
        }

        public bool EntityHasDirectSecurities(DR_Requester requester, int entityID, DataDirectSecurityMode mode)
        {
            return bizRoleSecurity.EntityHasDirectSecurities(requester, entityID,  mode);
        }
        //public bool EntityHasInDirectSecurities(DR_Requester requester, int entityID, DataDirectSecurityMode mode)
        //{
        //    return bizRoleSecurity.EntityHasInDirectSecurities(requester, entityID, mode);
        //}
        //public bool EntityHasInDirectSecurities(DR_Requester requester, int entityID)
        //{
        //    return bizRoleSecurity.EntityHasInDirectSecurities(requester, entityID);
        //}

        public List<EntitySecurityDirectDTO> GetEntitySecurityDirects(DR_Requester requester, int entityID)
        {
            return bizRoleSecurity.GetEntitySecurityDirects(requester, entityID, true);
        }
        //public EntitySecurityInDirectDTO GetEntitySecurityInDirect(DR_Requester requester, int entityID)
        //{
        //    return bizRoleSecurity.GetEntitySecurityInDirect(requester, entityID, true);
        //}
    }
}
