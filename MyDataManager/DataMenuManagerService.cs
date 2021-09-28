using ModelEntites;
using MyDataManagerBusiness;
using MyExternalReportLibrary;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDataManagerService
{
    public class DataMenuManagerService
    {
        BizDataMenuSetting bizDataMenuSetting = new BizDataMenuSetting();
        public DataMenuSettingDTO GetDataMenuSetting(DR_Requester requester, int ID)
        {
            return bizDataMenuSetting.GetDataMenuSetting(requester, ID, true);
        }
        public DataMenuSettingDTO GetDefaultDataMenuSetting(DR_Requester requester, int entityID)
        {
            return bizDataMenuSetting.GetDefaultDataMenuSetting(requester, entityID, true);
        }
        public DataMenuResult GetDataMenu(DR_Requester requester, DP_DataView dataItem, int dataMenuSettingID)
        {
            return bizDataMenuSetting.GetDataMenu(requester, dataItem, dataMenuSettingID);
        }
    }
}
