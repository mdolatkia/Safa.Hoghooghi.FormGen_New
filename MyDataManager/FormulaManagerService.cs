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
    public class FormulaManagerService
    {

        public FunctionResult CalculateFormula(int formulaID, DP_DataRepository mainDataItem, DR_Requester requester)
        {
            FormulaFunctionHandler FormulaFunctionHandler = new FormulaFunctionHandler();
            return FormulaFunctionHandler.CalculateFormula(formulaID, mainDataItem, requester);
        }

        public FormulaDTO GetFormula(DR_Requester requester, int formulaID)
        {
            BizFormula bizFormula = new BizFormula();
            return bizFormula.GetFormula(requester,formulaID, true);
        }
        public List<FormulaDTO> GetFormulas(DR_Requester requester, int entityID)
        {
            BizFormula bizFormula = new BizFormula();
            return bizFormula.GetFormulas(entityID, true);
        }
        public object GetValueSomeHow(DR_Requester requester, DP_DataRepository sourceData, EntityRelationshipTailDTO valueRelationshipTail, int valueColumnID)
        {
            DataitemRelatedColumnValueHandler dataitemRelatedColumnValueHandler = new DataitemRelatedColumnValueHandler();
            return dataitemRelatedColumnValueHandler.GetValueSomeHow(requester, sourceData, valueRelationshipTail, valueColumnID);
        }
    }
}
