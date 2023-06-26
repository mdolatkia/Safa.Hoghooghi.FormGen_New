using MyUILibraryInterfaces.GridViewArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using ModelEntites;

using MyUILibraryInterfaces.DataMenuArea;
using MyUILibraryInterfaces.EntityArea;

using MyUILibraryInterfaces.FormulaCalculationArea;

namespace MyUILibrary.FormulaArea
{
    public class FormulaCalculationArea : I_FormulaCalculationArea
    {
        public FormulaCalculationAreaInitializer AreaInitializer
        {
            set; get;
        }

        public I_View_FormulaCalculationArea View
        {
            set; get;
        }
        public FormulaCalculationArea(FormulaCalculationAreaInitializer initializer)
        {
            AreaInitializer = initializer;
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetViewOfFormulaCalculationArea();
            View.CloseRequested += View_CloseRequested;
            View.FromulaExpression = initializer.ColumnCustomFormula.Formula.Tooltip;
           // var dataProperty = initializer.DataItem.GetProperty(initializer.ColumnControl.Column.ID);

            AreaInitializer.FomulaManager.CalculateProperty(initializer.ChildSimpleContorlProperty);
            if (string.IsNullOrEmpty(initializer.ChildSimpleContorlProperty.Property.FormulaException))
            {
                View.ResultString = initializer.ChildSimpleContorlProperty.Property.Value;
            }
            else
            {
                View.ResultString = "Error";
                View.AddException(initializer.ChildSimpleContorlProperty.Property.FormulaException);
                View.ExceptionTabSelect();
            }

            if (initializer.ChildSimpleContorlProperty.Property.FormulaUsageParemeters.Any())
            {
                View.ClearTree();
                foreach (var item in initializer.ChildSimpleContorlProperty.Property.FormulaUsageParemeters)
                {
                    AddFormulaParameteNode(null, item);
                }
            }

            //if (FormulaOption == null)
            //{
            //    //همون اول اگر پراپرتی دیتیل یا خطا داره لود بشه
            //    //FormulaOption = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetFormulaOptionForm();
            //    //FormulaOption.ClaculateRequested += (sender1, e1) => FormulaOption_ClaculateRequested(sender1, e1, e.data as DP_DataRepository);
            //    //FormulaOption.ErrorDetailRequested += (sender1, e1) => FormulaOption_ErrorDetailRequested(sender1, e1, e.data as DP_DataRepository);
            //    //FormulaOption.ClaculationDetailsRequested += (sender1, e1) => FormulaOption_ClaculationDetailsRequested(sender1, e1, e.data as DP_DataRepository);
            //}
            //SetCalculatoinButtons(dataProperty);
            //}
            //else
            //{
            //    throw new Exception("asdasdF");
            //}

        }

        private void View_CloseRequested(object sender, EventArgs e)
        {
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(View);
        }

        private void AddFormulaParameteNode(object node, FormulaUsageParemetersDTO item)
        {
            var title = (string.IsNullOrEmpty(item.RelationshipPropertyTail) ? "" : item.RelationshipPropertyTail + ".") + item.ParameterName + " : " + item.ParameterValue;
            var pNode = View.AddTreeNode(node, title);
            //foreach (var property in item.ChildItems)
            //{
            //    AddFormulaParameteNode(pNode, property);
            //}
        }


    }
}
