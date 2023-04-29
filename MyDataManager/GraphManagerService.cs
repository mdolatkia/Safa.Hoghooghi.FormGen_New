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
    public class GraphManagerService
    {
        BizGraph bizGraph = new BizGraph();
        public GraphDTO GetGraph(DR_Requester dR_Requester, int ID)
        {
            return bizGraph.GetGraph(dR_Requester,ID);
        }
     
        public List<GraphDTO> GetGraphs(DR_Requester dR_Requester, int entityID)
        {
            return bizGraph.GetGraphByEntitiyID(dR_Requester,entityID);
        }

        public List<GraphDTO> SearchGraphs(DR_Requester dR_Requester, string singleFilterValue)
        {
            return bizGraph.SearchGraphs( dR_Requester, singleFilterValue);
        }
    }
}
