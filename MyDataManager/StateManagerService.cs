using ModelEntites;

using MyDataManagerBusiness;



using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyFormulaManagerService
{
    public class StateManagerService
    {

        //public StateResult CalculateState(int stateID, DP_DataRepository mainDataItem, DR_Requester requester)
        //{
        //    StateHandler handler = new StateHandler();
        //    return handler.GetStateResult(stateID, mainDataItem,requester);
        //}
        public StateResult CalculateState(EntityStateDTO state, DP_DataRepository mainDataItem, DR_Requester requester)
        {
            StateHandler handler = new StateHandler();
            return handler.GetStateResult(state, mainDataItem, requester);
        }
        public object GetValueSomeHow(DR_Requester requester, DP_DataRepository sourceData, EntityRelationshipTailDTO valueRelationshipTail, int valueColumnID)
        {
            DataitemRelatedColumnValueHandler dataitemRelatedColumnValueHandler = new DataitemRelatedColumnValueHandler();
            return dataitemRelatedColumnValueHandler.GetValueSomeHow(requester, sourceData, valueRelationshipTail, valueColumnID);
        }
        //public FormulaDTO GetFormula(int formulaID)
        //{
        //    BizFormula bizFormula = new BizFormula();
        //    return bizFormula.GetFormula(formulaID);
        //}

    }
}
