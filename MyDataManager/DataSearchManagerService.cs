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
    public class DataSearchManagerService
    {
        BizEntitySearch bizEntitySearch = new BizEntitySearch();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        public EntitySearchDTO GetEntitySearch(DR_Requester requester, int ID)
        {
            return bizEntitySearch.GetEntitySearch(requester,ID);
        }

        public EntitySearchDTO GetOrCreateEntitySearchDTO(DR_Requester requester, int entityID)
        {
            return bizEntitySearch.GetOrCreateEntitySearchDTO(requester,entityID);
        }



    }
}
