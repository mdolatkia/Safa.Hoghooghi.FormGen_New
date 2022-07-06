using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelEntites;
using ProxyLibrary;
using MyUILibrary.EntityArea.Commands;
using CommonDefinitions.UISettings;
using MyUILibraryInterfaces.FormulaCalculationArea;
using MyUILibrary.FormulaArea;

namespace MyUILibrary.EntityArea
{
    public class UIFomulaManager : I_UIFomulaManager
    {
        BaseEditEntityArea EditArea { set; get; }

        public UIFomulaManager(BaseEditEntityArea editArea)
        {
            EditArea = editArea;
            editArea.DataViewGenerated += EditArea_DataViewGenerated;
        }
        List<SimpleColumnControlGenerel> FormulaColumns = new List<SimpleColumnControlGenerel>();
        private void EditArea_DataViewGenerated(object sender, EventArgs e)
        {
            foreach (var columnControl in EditArea.SimpleColumnControls)
            {
                if (columnControl.Column.ColumnCustomFormula != null)
                {
                    FormulaColumns.Add(columnControl);
                }
            }
            if (FormulaColumns.Any())
            {
                //  AddMenu();
                EditArea.DataItemShown += EditArea_DataItemLoaded;
                // EditArea.DataItemUnShown += EditArea_DataItemUnShown;
            }
        }

        private void EditArea_DataItemLoaded(object sender, EditAreaDataItemLoadedArg e)
        {
            if (e.InEditMode)
            {
                foreach (var columnControl in FormulaColumns)
                {
                    var childSimpleContorlProperties = e.DataItem.ChildSimpleContorlProperties.FirstOrDefault(x => x.SimpleColumnControl.Column.ID == columnControl.Column.ID);
                    if (childSimpleContorlProperties == null)
                        continue;//حذف شود
                    var cpMenuFormulaCalculation = new ConrolPackageMenu();
                    cpMenuFormulaCalculation.Name = "mnuFormulaCalculation";
                    cpMenuFormulaCalculation.Title = "محاسبه فرمول";
                    cpMenuFormulaCalculation.Tooltip = columnControl.Column.ColumnCustomFormula.Formula.Tooltip;
                    childSimpleContorlProperties.GetUIControlManager.AddButtonMenu(cpMenuFormulaCalculation);
                    cpMenuFormulaCalculation.MenuClicked += (sender1, e1) => CpMenuFormulaCalculation_MenuClicked(sender1, e1, childSimpleContorlProperties);

                    string generalKey = "formulaColumn" + AgentHelper.GetUniqueDataPostfix(e.DataItem);
                    string usageKey = columnControl.Column.ID.ToString();
                    if (e.DataItem.ChangeMonitorExists(generalKey, usageKey))
                        return;
                    var fullFormula = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.GetFormula(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), columnControl.Column.ColumnCustomFormula.FormulaID);
                    if (fullFormula.FormulaItems.Any(x => x.ItemType == FormuaItemType.Column || !string.IsNullOrEmpty(x.RelationshipIDTail)))
                    {
                        e.DataItem.RelatedDataTailOrColumnChanged += (sender1, e1) => DataItem_RelatedDataTailOrColumnChanged(sender1, e1, childSimpleContorlProperties);
                    }
                    var columnItems = fullFormula.FormulaItems.Where(x => x.ItemType == FormuaItemType.Column);
                    if (columnItems.Any())
                    {
                        foreach (var item in columnItems)
                        {
                            e.DataItem.AddChangeMonitorIfNotExists(generalKey, usageKey, item.RelationshipIDTail, item.ItemID);
                        }
                    }
                    var relationshipItems = fullFormula.FormulaItems.Where(x => !string.IsNullOrEmpty(x.RelationshipIDTail)).GroupBy(x => x.RelationshipIDTail);
                    if (relationshipItems.Any())
                    {
                        foreach (var item in relationshipItems)
                        {
                            e.DataItem.AddChangeMonitorIfNotExists(generalKey, usageKey, item.Key, 0);
                        }
                    }
                }
            }
        }

        //private void EditArea_DataItemUnShown(object sender, EditAreaDataItemArg e)
        //{
        //    foreach (var columnControl in FormulaColumns)
        //    {
        //        var fullFormula = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.GetFormula(columnControl.Column.CustomFormula.ID);
        //        if (fullFormula.FormulaItems.Any(x => x.ItemType == FormuaItemType.Column || !string.IsNullOrEmpty(x.RelationshipIDTail)))
        //        {
        //            e.DataItem.RemoveChangeMonitorByGenaralKey("formulaColumn" + AgentHelper.GetUniqueDataPostfix(e.DataItem));
        //        }
        //    }
        //}

        //private void EditArea_DataItemShown(object sender, EditAreaDataItemArg e)
        //{

        //}

        private void DataItem_RelatedDataTailOrColumnChanged(object sender, ChangeMonitor e, ChildSimpleContorlProperty childSimpleContorlProperty)
        {
            if (e.GeneralKey.StartsWith("formulaColumn"))
            {
                if (e.DataToCall.DataIsInEditMode())
                {
                    foreach (var columnControl in FormulaColumns.Where(x => x.Column.ID.ToString() == e.UsageKey))
                    {
                        var formulaColumn = FormulaColumns.First(x => x.Column.ID == columnControl.Column.ID).Column.ColumnCustomFormula;
                        var dataProperty = e.DataToCall.GetProperty(columnControl.Column.ID);
                        if (dataProperty != null)
                        {
                            CalculateProperty(childSimpleContorlProperty);
                        }
                    }
                }
            }
        }




        //private void RelatedDataColumnValueChanged(object sender, RelatedDataColumnValueChangedArg e, FormulaDTO formula, DP_DataRepository dataItem, int columnID)
        //{

        //}


        public void CalculateProperty(ChildSimpleContorlProperty childSimpleContorlProperty)
        {
            var columnCustomFormula = childSimpleContorlProperty.SimpleColumnControl.Column.ColumnCustomFormula;
            var dataItem = childSimpleContorlProperty.SourceData;
            var dataProperty = childSimpleContorlProperty.Property;
            var key = "formulaCalculated" + "_" + columnCustomFormula.ID;
            childSimpleContorlProperty.RemovePropertyFormulaComment(key);
            if (columnCustomFormula.OnlyOnNewData)
            {
                if (!dataItem.IsNewItem)
                {
                    childSimpleContorlProperty.AddPropertyFormulaComment("فرمول" + " " + columnCustomFormula.Formula.Name + " " + "بروی داده موجود اعمال نمی شود", key);
                    return;
                }
            }
            if (columnCustomFormula.OnlyOnEmptyValue)
            {
                if (dataProperty.Value != null && !string.IsNullOrEmpty(dataProperty.Value.ToString()))
                {
                    childSimpleContorlProperty.AddPropertyFormulaComment("فرمول" + " " + columnCustomFormula.Formula.Name + " " + "بروی خصوصیت دارای مقدار اعمال نمی شود", key);
                    return;
                }
            }


            var result = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.CalculateFormula(columnCustomFormula.Formula.ID, dataItem, AgentUICoreMediator.GetAgentUICoreMediator.GetRequester());
            dataProperty.FormulaID = columnCustomFormula.Formula.ID;
            dataProperty.FormulaException = null;
            dataProperty.FormulaUsageParemeters = result.FormulaUsageParemeters;


            if (result.Exception == null)
            {
                //dataProperty.Value = result.Result;

                //    AddDataMessageItem(dataItem, key, ControlItemPriority.Normal, "محاسبه شده توسط فرمول" + " " + columnCustomFormula.Formula.Title);


            }
            else
            {
                dataProperty.FormulaException = result.Exception.Message;
                //  dataProperty.Value = "";

                //اینجا خطا روی ستون یا رابطه بدهد
                childSimpleContorlProperty.AddPropertyFormulaComment("خطا در محاسبه فرمول" + " " + columnCustomFormula.Formula.Title + ":" + " " + dataProperty.FormulaException, key);
            }


            var uiColumnValue = new UIColumnValueDTO();
            uiColumnValue.ColumnID = columnCustomFormula.ID;
            //ابجکت نشه؟ExactValue
            uiColumnValue.ExactValue = result.Result.ToString();
            uiColumnValue.EvenHasValue = !columnCustomFormula.OnlyOnEmptyValue;
            uiColumnValue.EvenIsNotNew = !columnCustomFormula.OnlyOnNewData;
            List<UIColumnValueDTO> uIColumnValues = new List<UIColumnValueDTO>() { uiColumnValue };
            dataItem.SetColumnValue(uIColumnValues, null, columnCustomFormula.Formula, false);

        }
        public void CalculateProperty(DP_DataRepository dataItem, EntityInstanceProperty dataProperty)
        {
            var columnCustomFormula = dataProperty.Column.ColumnCustomFormula;
            //   dataItem = childSimpleContorlProperty.SourceData;
            //  dataProperty = childSimpleContorlProperty.Property;
            // var key = "formulaCalculated" + "_" + columnCustomFormula.ID;
            //  childSimpleContorlProperty.RemovePropertyFormulaComment(key);
            if (columnCustomFormula.OnlyOnNewData)
            {
                if (!dataItem.IsNewItem)
                {
                    //  childSimpleContorlProperty.AddPropertyFormulaComment("فرمول" + " " + columnCustomFormula.Formula.Name + " " + "بروی داده موجود اعمال نمی شود", key);
                    return;
                }
            }
            if (columnCustomFormula.OnlyOnEmptyValue)
            {
                if (dataProperty.Value != null && !string.IsNullOrEmpty(dataProperty.Value.ToString()))
                {
                    //  childSimpleContorlProperty.AddPropertyFormulaComment("فرمول" + " " + columnCustomFormula.Formula.Name + " " + "بروی خصوصیت دارای مقدار اعمال نمی شود", key);
                    return;
                }
            }


            var result = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.CalculateFormula(columnCustomFormula.Formula.ID, dataItem, AgentUICoreMediator.GetAgentUICoreMediator.GetRequester());
            dataProperty.FormulaID = columnCustomFormula.Formula.ID;
            dataProperty.FormulaException = null;
            dataProperty.FormulaUsageParemeters = result.FormulaUsageParemeters;


            if (result.Exception == null)
            {
                //dataProperty.Value = result.Result;

                //    AddDataMessageItem(dataItem, key, ControlItemPriority.Normal, "محاسبه شده توسط فرمول" + " " + columnCustomFormula.Formula.Title);


            }
            else
            {
                dataProperty.FormulaException = result.Exception.Message;
                //  dataProperty.Value = "";

                //اینجا خطا روی ستون یا رابطه بدهد
                //     childSimpleContorlProperty.AddPropertyFormulaComment("خطا در محاسبه فرمول" + " " + columnCustomFormula.Formula.Title + ":" + " " + dataProperty.FormulaException, key);
            }


            dataProperty.Value = result.Result;


        }
        //private void AddDataMessageItem(DP_FormDataRepository dataItem, string key, ControlItemPriority priority, string message)
        //{
        //    DataMessageItem baseMessageItem = new DataMessageItem();
        //    baseMessageItem.CausingDataItem = dataItem;
        //    baseMessageItem.Key = key;
        //    baseMessageItem.Priority = priority;
        //    baseMessageItem.Message = message;
        //    EditArea.AddDataItemMessage(baseMessageItem);
        //}

        //private List<FormulaItemDTO> GetFlatList(List<FormulaItemDTO> treeFormulaItems, List<FormulaItemDTO> result = null)
        //{
        //    if (result == null)
        //        result = new List<FormulaItemDTO>();
        //    foreach (var item in treeFormulaItems)
        //    {
        //        if (item.ItemType == FormuaItemType.Column)
        //            result.Add(item);
        //        GetFlatList(item.ChildFormulaItems, result);
        //    }
        //    return result;
        //}

        //private void AddMenu()
        //{

        //}

        private void CpMenuFormulaCalculation_MenuClicked(object sender, ConrolPackageMenuArg e, ChildSimpleContorlProperty childSimpleContorlProperty)
        {
            FormulaCalculationAreaInitializer initializer = new FormulaCalculationAreaInitializer();
            initializer.ChildSimpleContorlProperty = childSimpleContorlProperty;
            initializer.FomulaManager = this;
            initializer.ColumnCustomFormula = childSimpleContorlProperty.SimpleColumnControl.Column.ColumnCustomFormula;
            //initializer.ColumnControl = columnControl;
            var formulaCalculationArea = new FormulaCalculationArea(initializer);
            if (formulaCalculationArea.View != null)
            {
                var window = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow();
                window.ShowDialog(formulaCalculationArea.View, "محاسبه فرمول", Enum_WindowSize.Big);
            }
        }

        bool? decided = null;
        //public void UpdateFromulas()
        //{
        //   // List<CalculatedPropertyTree> calculatedColumns = new List<EntityArea.CalculatedPropertyTree>();
        // //   UpdateFromulas();
        //    //RemoveRedundantData(calculatedColumns);
        //    //if (calculatedColumns.Any())
        //    //{
        //    //    decided = null;
        //    //    I_FormulaDataTree formulaTree = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetViewOdFormulaTree();
        //    //    formulaTree.YesClicked += FormulaTree_YesClicked;
        //    //    formulaTree.NoClicked += FormulaTree_NoClicked; ;
        //    //    formulaTree.AddTitle("برای داده های مورد تایید فرمولهای زیر محاسبه شده اند");
        //    //    AddFormulaNode(formulaTree, calculatedColumns, null);
        //    //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(formulaTree, "تایید", Enum_WindowSize.Big, true);
        //    //    return decided == true;
        //    //}
        //    //else
        //    //    return true;

        //}

        //private void FormulaTree_NoClicked(object sender, EventArgs e)
        //{
        //    decided = false;
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(sender);
        //}

        //private void FormulaTree_YesClicked(object sender, EventArgs e)
        //{
        //    decided = true;
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(sender);

        //}

        //private void AddFormulaNode(I_FormulaDataTree formulaTree, List<CalculatedPropertyTree> calculatedColumns, object parentNode)
        //{
        //    foreach (var item in calculatedColumns)
        //    {

        //        var node = formulaTree.AddTreeNode(parentNode, item.DataItem.ViewInfo, item.RelationshipInfo, Temp.InfoColor.Default, true);
        //        foreach (var property in item.Properties)
        //        {
        //            if (!string.IsNullOrEmpty(property.FormulaException))
        //            {
        //                formulaTree.AddTreeNode(node, property.Column.Alias + ":" + " " + property.FormulaException, item.RelationshipInfo, Temp.InfoColor.Red, true);
        //            }
        //            else
        //            {
        //                formulaTree.AddTreeNode(node, property.Column.Alias + ":" +  property.Value, item.RelationshipInfo, Temp.InfoColor.Default, true);
        //            }
        //        }
        //        AddFormulaNode(formulaTree, item.ChildItems, node);
        //    }
        //}


        //private void RemoveRedundantData(List<CalculatedPropertyTree> calculatedColumns)
        //{
        //    foreach (var item in calculatedColumns.ToList())
        //    {
        //        if (!IsValid(item))
        //            calculatedColumns.Remove(item);
        //    }
        //    foreach (var item in calculatedColumns)
        //    {
        //        RemoveRedundantData(item.ChildItems);
        //    }
        //}
        //private bool IsValid(CalculatedPropertyTree item)
        //{
        //    if (item.Properties.Any())
        //        return true;
        //    bool childIsValid = false;
        //    foreach (var child in item.ChildItems)
        //    {
        //        if (IsValid(child))
        //            childIsValid = true;
        //    }
        //    return childIsValid;
        //}
        public void UpdateFromulas()
        {
            var datalist = EditArea.GetDataList().Where(x => x.ShoudBeCounted).ToList();
          
        }
    }
}
