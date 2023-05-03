using AutoMapper;
using CommonDefinitions.UISettings;
using ModelEntites;

using MyUILibrary;
using MyUILibrary.EntityArea;
using MyUILibrary.EntityArea.Commands;
using MyUILibrary.Temp;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{
    public class RawSearchEntityArea : I_RawSearchEntityArea
    {
        public event EventHandler<SimpleSearchColumnControl> FormulaSelectionRequested;
        public RawSearchEntityArea(SearchAreaInitializer newAreaInitializer)
        {
            SearchCommands = new List<I_Command>();
            SimpleColumnControls = new List<SimpleSearchColumnControl>();
            AreaInitializer = newAreaInitializer;
            GenerateSearchView();
        }
        public I_View_SimpleSearchEntityArea RawSearchView { set; get; }

        public List<I_Command> SearchCommands
        {
            set;
            get;
        }

        public SearchAreaInitializer AreaInitializer { set; get; }

        //public event EventHandler<Arg_PackageSelected> DataPackageSelected;
        public event EventHandler<SearchPropertyArg> SearchDataDefined;






        public List<SimpleSearchColumnControl> SimpleColumnControls
        {
            set;
            get;
        }


        TableDrivedEntityDTO _SimpleEntity;
        public TableDrivedEntityDTO SimpleEntity
        {
            get
            {
                if (_SimpleEntity == null)
                {
                    if (FullEntity != null)
                        return FullEntity;
                    else
                        _SimpleEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetSimpleEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                }
                return _SimpleEntity;
            }
        }

        TableDrivedEntityDTO _FullEntity;
        public TableDrivedEntityDTO FullEntity
        {
            get
            {
                if (_FullEntity == null)
                    _FullEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetPermissionedEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);

                //if (AreaInitializer.SourceRelationColumnControl != null)
                //    _FullEntity.Relationships.Clear();
                return _FullEntity;
            }
        }

        //List<UIControlPackageTree> uiControlPackageTree { set; get; }


        //AssignedPermissionDTO _Permission;
        //public AssignedPermissionDTO Permission
        //{
        //    get
        //    {
        //        if (_Permission == null)
        //            _Permission = AgentUICoreMediator.GetAgentUICoreMediator.SecurityHelper.GetAssignedPermissions(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), SearchInitializer.EntityID, true);
        //        return _Permission;
        //    }
        //}

        private void GenerateSearchView()
        {
            RawSearchView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfSearchEntityArea(GetEntityUISetting());
            GenerateUIControls();

            var searchClearCommand = new SearchClearCommand(this);
            RawSearchView.AddCommand(searchClearCommand.CommandManager);
            var simpleSearchconfirmcommand = new RawSearchConfirmCommand(this);
            RawSearchView.AddCommand(simpleSearchconfirmcommand.CommandManager);
        }


        private void GenerateUIControls()
        {
            //RawSearchEntityArea.GenerateUIControls: 527171b12fdc
            foreach (var column in FullEntity.Columns.OrderBy(x => x.Position))
            {
                var propertyControl = new SimpleSearchColumnControl();
                propertyControl.Column = column;
                SimpleColumnControls.Add(propertyControl);
            }
            foreach (var columnControl in SimpleColumnControls)
            {
                columnControl.Operators = GetSimpleColumnOperators(columnControl.Column);
                //  columnControl.ControlPackage = new UIControlPackageForSimpleColumn();
                //bool hasRangeOfValues = columnControl.Column.ColumnValueRange != null && columnControl.Column.ColumnValueRange.Details.Any();
                columnControl.ControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateSimpleControlManagerForOneDataForm(columnControl.Column, GetColumnUISetting(columnControl.Column), columnControl.Operators);
                columnControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(columnControl.Column.Alias);

                if (columnControl.ControlManager.UIControlManagerIsKeyValueList)
                {
                    columnControl.ControlManager.GetUIControlManager_ColumnValueRange().SetMultiSelect(true);
                }
                else
                {
                    if (columnControl.Operators.Any(x => x.IsDefault))
                        columnControl.ControlManager.GetUIControlManager().SetOperator(columnControl.Operators.First(x => x.IsDefault).Operator);
                }
                columnControl.FormulaSelectionRequested += PropertyControl_FormulaSelectionRequested;
                if (AreaInitializer.ForSave)
                    columnControl.SetSimpleColumnFormulaSelection();


                RawSearchView.AddUIControlPackage(columnControl.ControlManager, columnControl.LabelControlManager);
            }
        }
        private void PropertyControl_FormulaSelectionRequested(object sender, SimpleSearchColumnControl e)
        {
            if (FormulaSelectionRequested != null)
                FormulaSelectionRequested(sender, e);
        }
        private CommonOperator GetDefaultOperator(ColumnDTO column)
        {

            if (column.ColumnType == Enum_ColumnType.String)
            {
                return CommonOperator.Contains;
            }
            else if (column.ColumnType == Enum_ColumnType.Numeric)
            {
                return CommonOperator.Equals;
            }
            return CommonOperator.Equals;
        }

        private List<SimpleSearchOperator> GetSimpleColumnOperators(ColumnDTO column)
        {
            List<SimpleSearchOperator> result = new List<SimpleSearchOperator>();
            //if (column.ColumnType == Enum_ColumnType.String)
            //{
            //    result.Add(new SimpleSearchOperator() { Operator = CommonOperator.Equals, Title = "برابر" });
            //    result.Add(new SimpleSearchOperator() { Operator = CommonOperator.Contains, Title = "شامل" });
            //    result.Add(new SimpleSearchOperator() { Operator = CommonOperator.StartsWith, Title = "شروع شود با" });
            //    result.Add(new SimpleSearchOperator() { Operator = CommonOperator.EndsWith, Title = "تمام شود با" });
            //}
            //else if (column.ColumnType == Enum_ColumnType.Numeric)
            //{
            //    result.Add(new SimpleSearchOperator() { Operator = CommonOperator.Equals, Title = "برابر" });
            //    result.Add(new SimpleSearchOperator() { Operator = CommonOperator.SmallerThan, Title = "کوچکتر از" });
            //    result.Add(new SimpleSearchOperator() { Operator = CommonOperator.BiggerThan, Title = "بزرگتر از" });
            //}
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.Equals, Title = "برابر", IsDefault = true });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.NotEquals, Title = "نابرابر" });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.InValues, Title = "در مقادیر" });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.NotInValues, Title = "عدم وجود در مقادیر" });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.SmallerThan, Title = "کوچکتر از" });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.BiggerThan, Title = "بزرگتر از" });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.Contains, Title = "شامل" });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.StartsWith, Title = "شروع شود با" });
            result.Add(new SimpleSearchOperator() { Operator = CommonOperator.EndsWith, Title = "تمام شود با" });
            return result;
        }



        EntityUISettingDTO _EntityUISetting;
        private EntityUISettingDTO GetEntityUISetting()
        {
            if (_EntityUISetting == null)
            {
                _EntityUISetting = new EntityUISettingDTO();
                _EntityUISetting.UIColumnsCount = 4;
            }
            return _EntityUISetting;
        }

        ColumnUISettingDTO _ColumnUISetting;
        private ColumnUISettingDTO GetColumnUISetting(ColumnDTO column)
        {
            if (_ColumnUISetting == null)
            {
                _ColumnUISetting = new ColumnUISettingDTO();
                _ColumnUISetting.UIColumnsType = Enum_UIColumnsType.Normal;
                _ColumnUISetting.UIRowsCount = 1;
            }
            return _ColumnUISetting;
        }





        public void ClearSearchData()
        {



        }


        public List<SearchProperty> GetSearchRepository()
        {

            //** 23709caa-6bdc-4d4c-a4e7-79a3cc81a307
            List<SearchProperty> result = new List<SearchProperty>();
            foreach (var property in SimpleColumnControls)
            {
                var value = property.ControlManager.GetUIControlManager().GetValue();
                if (PropertyHasValue(property, value))
                {

                    SearchProperty searchProperty = new SearchProperty(property.Column);
                    searchProperty.IsKey = property.Column.PrimaryKey;
                    searchProperty.Value = value;
                    searchProperty.Operator = property.ControlManager.GetUIControlManager().GetOperator();
                    result.Add(searchProperty);
                }
                else if (property.Formula != null)
                {
                    SearchProperty searchProperty = new SearchProperty(property.Column);
                    searchProperty.IsKey = property.Column.PrimaryKey;
                    searchProperty.Formula = property.Formula;
                    searchProperty.FormulaID = property.Formula.ID;
                    searchProperty.Operator = property.ControlManager.GetUIControlManager().GetOperator();
                    result.Add(searchProperty);
                }
            }
            return result;
        }

        private bool PropertyHasValue(SimpleSearchColumnControl property, object value)
        {
            return value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString().ToLower() != "0";
        }

        //private bool PropertyHasValue(SearchProperty item)
        //{
        //    return !string.IsNullOrEmpty(item.Value)  && item.Value.ToLower() != "0";
        //}

        private bool SearchValueIsEmpty(SimpleSearchColumnControl typePropertyControl, string value)
        {
            if (typePropertyControl is NullColumnControl)
                return string.IsNullOrEmpty(value) || value == "false" || value == "0";
            else if (typePropertyControl is RelationCheckColumnControl || typePropertyControl is RelationCountCheckColumnControl)
                return string.IsNullOrEmpty(value) || value == "false" || value == "0";
            else
                return string.IsNullOrEmpty(value) || value == "0";
        }



        public void OnSearchDataDefined(List<SearchProperty> searchData)
        {
            if (SearchDataDefined != null)
            {
                SearchDataDefined(this, new SearchPropertyArg() { SearchItems = searchData });
            }
        }
    }
}
