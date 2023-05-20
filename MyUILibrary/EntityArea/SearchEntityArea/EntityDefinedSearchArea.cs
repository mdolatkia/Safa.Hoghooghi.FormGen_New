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
    public class EntityDefinedSearchArea : I_EntityDefinedSearchArea
    {
        public event EventHandler<SimpleSearchColumnControl> FormulaSelectionRequested;
        public EntityDefinedSearchArea(SearchAreaInitializer newAreaInitializer)
        {
            //**EntityDefinedSearchArea: de46ee61a50f
            SimpleColumnControls = new List<SimpleSearchColumnControl>();
            RelationshipColumnControls = new List<RelationshipSearchColumnControl>();
            AreaInitializer = newAreaInitializer;


            SimpleSearchView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfSearchEntityArea(EntitySearchDTO.EntityUISetting);
            GenerateUIControls();

            if (newAreaInitializer.PreDefinedSearchMessage != null)
                ShowPreDefinedSearch(newAreaInitializer.PreDefinedSearchMessage);

            AddCommands();
           

        }

        private void AddCommands()
        {
            //EntityDefinedSearchArea.AddCommands: 101c0aa583fe
            var searchClearCommand = new SearchClearCommand(this);
            SimpleSearchView.AddCommand(searchClearCommand.CommandManager);
            if (!AreaInitializer.ForSave)
            {
                var simpleSearchconfirmcommand = new SimpleSearchConfirmCommand(this);
                SimpleSearchView.AddCommand(simpleSearchconfirmcommand.CommandManager);
            }
        }

        EntitySearchDTO _EntitySearch;
        public EntitySearchDTO EntitySearchDTO
        {
            // ** EntityDefinedSearchArea.EntitySearchDTO: 5f09df3218c6
            get
            {
                if (_EntitySearch == null)
                {
                    int entitySearchID = 0;
                    if (AreaInitializer.PreDefinedSearchMessage != null)
                        entitySearchID = AreaInitializer.PreDefinedSearchMessage.EntitySearchID;
                    else
                    {
                        //این حالت فقط موقع تعریف پیش می آید
                        entitySearchID = AreaInitializer.EntitySearchID;
                    }

                    if (entitySearchID != 0)
                        _EntitySearch = AgentUICoreMediator.GetAgentUICoreMediator.DataSearchManager.GetEntitySearch(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), entitySearchID);
                    else
                        _EntitySearch = AgentUICoreMediator.GetAgentUICoreMediator.DataSearchManager.GetOrCreateEntitySearchDTO(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                }
                return _EntitySearch;
            }
        }

        List<FormulaDTO> _Formulas;
        public List<FormulaDTO> Formulas
        {
            get
            {
                if (_Formulas == null)
                {
                    _Formulas = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.GetFormulas(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                }
                return _Formulas;
            }
        }

        //  DP_SearchRepositoryMain firstRepository { set; get; }
        public I_View_SimpleSearchEntityArea SimpleSearchView { set; get; }

        public SearchAreaInitializer AreaInitializer { set; get; }

        //public event EventHandler<Arg_PackageSelected> DataPackageSelected;
        public event EventHandler<DP_SearchRepositoryMain> SearchDataDefined;
        public List<SimpleSearchColumnControl> SimpleColumnControls
        {
            set;
            get;
        }
        public List<RelationshipSearchColumnControl> RelationshipColumnControls
        {
            set;
            get;
        }

        private void GenerateUIControls()
        {
            //** EntityDefinedSearchArea.GenerateUIControls: 31d456f709e0

            SimpleSearchView.QuickSearchVisiblity = EntitySearchDTO.EntitySearchAllColumns.Any(x => x.ColumnID != 0 && x.ExcludeInQuickSearch == false);
            foreach (var searchcolumn in EntitySearchDTO.EntitySearchAllColumns.OrderBy(x => x.OrderID))
            {
                if (searchcolumn.Column == null)
                {
                    if (searchcolumn.RelationshipTail != null)
                    {


                        EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
                        editEntityAreaInitializer1.EntityID = searchcolumn.RelationshipTail.TargetEntityID;
                        editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                        editEntityAreaInitializer1.DataMode = CommonDefinitions.UISettings.DataMode.Multiple;
                        var FirstSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer1);
                        if (FirstSideEditEntityAreaResult.Item1 != null)
                        {
                            var propertyControl = new RelationshipSearchColumnControl(AgentUICoreMediator.GetAgentUICoreMediator.UIManager, FirstSideEditEntityAreaResult.Item1, searchcolumn);

                            if (!string.IsNullOrEmpty(propertyControl.EntitySearchColumn.Tooltip))
                                propertyControl.LabelControlManager.SetTooltip(propertyControl.EntitySearchColumn.Tooltip);
                            RelationshipColumnControls.Add(propertyControl);
                            SimpleSearchView.AddView(propertyControl.LabelControlManager, propertyControl.ControlManager);
                        }

                        ////اینجا چیه بررسی شود چرا اینجا؟
                        //if (propertyControl.EditNdTypeArea.SimpleEntity.SearchInitially == true)
                        //    propertyControl.EditNdTypeArea.SearchViewEntityArea.SearchInitialy();

                    }

                }
                else
                {
                    var propertyControl = new SimpleSearchColumnControl(AgentUICoreMediator.GetAgentUICoreMediator.UIManager, searchcolumn);
                    
                    if (!string.IsNullOrEmpty(propertyControl.EntitySearchColumn.Tooltip))
                        propertyControl.LabelControlManager.SetTooltip(propertyControl.EntitySearchColumn.Tooltip);

                    if (propertyControl.ControlManager.UIControlManagerIsKeyValueList)
                    {
                        propertyControl.ControlManager.GetUIControlManager_ColumnValueRange().SetMultiSelect(true);
                    }
                    else
                    {
                        if (propertyControl.Operators.Any(x => x.IsDefault))
                            propertyControl.ControlManager.GetUIControlManager().SetOperator(propertyControl.Operators.First(x => x.IsDefault).Operator);
                    }
                    propertyControl.FormulaSelectionRequested += PropertyControl_FormulaSelectionRequested;
                    if (AreaInitializer.ForSave)
                        propertyControl.SetSimpleColumnFormulaSelection();

                    SimpleColumnControls.Add(propertyControl);

                    SimpleSearchView.AddUIControlPackage(propertyControl.ControlManager, propertyControl.LabelControlManager);
                }
            }

            //foreach (var searchcolumn in EntitySearchDTO.EntitySearchAllColumns.OrderBy(x => x.OrderID))
            //{
            //    if (searchcolumn.Column == null)
            //    {
            //        if (searchcolumn.RelationshipTail != null)
            //        {
            //            var relControl = RelationshipColumnControls.First(x => x.EntitySearchColumn == searchcolumn);
            //        }
            //    }
            //    else
            //    {
            //        var simpleControl = SimpleColumnControls.First(x => x.EntitySearchColumn == searchcolumn);

            //    }
            //}
        }

        private void PropertyControl_FormulaSelectionRequested(object sender, SimpleSearchColumnControl e)
        {
            if (FormulaSelectionRequested != null)
                FormulaSelectionRequested(sender, e);
        }

        public void ClearSearchData()
        {
            SimpleSearchView.QuickSearchText = "";
            foreach (var searchcolumn in RelationshipColumnControls)
            {
                searchcolumn.EditNdTypeArea.ClearData();
            }
            foreach (var searchcolumn in SimpleColumnControls)
            {
                searchcolumn.ControlManager.GetUIControlManager().ClearValue();
                if (searchcolumn.Formula != null)
                    searchcolumn.RemoveSimpleColumnFormula();
            }
        }
        public void ShowPreDefinedSearch(PreDefinedSearchDTO preDefinedSearch)
        {
            //**  EntityDefinedSearchArea.ShowPreDefinedSearch: cd004ddc85ff

            SimpleSearchView.QuickSearchText = preDefinedSearch.QuickSearchValue;
            foreach (var phrase in preDefinedSearch.SimpleSearchProperties)
            {
                if (SimpleColumnControls.Any(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID))
                {
                    var simpleColumnControl = SimpleColumnControls.First(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID);
                    //مقدار چندتایی اوکی بشه
                    if (phrase.Value != null)
                        simpleColumnControl.ControlManager.GetUIControlManager().SetValue(phrase.Value);

                    if (phrase.FormulaID != 0)
                    {
                        if (AreaInitializer.ForSave)
                        {
                            simpleColumnControl.AddSimpleColumnFormula(phrase.Formula);
                        }
                        //else
                        //{

                        ////var result = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.CalculateFormula(phrase.FormulaID, null, AgentUICoreMediator.GetAgentUICoreMediator.GetRequester());
                        //if (phrase.Exception == null)
                        //    simpleColumnControl.ControlManager.GetUIControlManager().SetValue(result.Result);
                        //else
                        //{
                        //    simpleColumnControl.ControlManager.GetUIControlManager().SetTooltip(result.Exception.Message);
                        //}

                        //}
                    }
                    if (!string.IsNullOrWhiteSpace(phrase.Tooltip))
                        simpleColumnControl.ControlManager.GetUIControlManager().SetTooltip(phrase.Tooltip);
                    simpleColumnControl.ControlManager.GetUIControlManager().SetOperator(phrase.Operator);
                }
            }
            foreach (var phrase in preDefinedSearch.RelationshipSearchProperties)
            {
                if (RelationshipColumnControls.Any(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID))
                {
                    List<Dictionary<ColumnDTO, object>> items = new List<Dictionary<ColumnDTO, object>>();
                    foreach (var item in phrase.DataItems)
                    {
                        Dictionary<ColumnDTO, object> data = new Dictionary<ColumnDTO, object>();
                        foreach (var col in item.KeyProperties)
                        {
                            data.Add(col.Column, col.Value);
                        }
                        items.Add(data);

                    }
                    RelationshipColumnControls.First(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID).EditNdTypeArea.SelectData(items);
                }
            }
        }



        //private List<SearchProperty> GetSearchProperties(List<Phrase> phrase, List<SearchProperty> result = null)
        //{
        //    if (result == null)
        //        result = new List<SearchProperty>();
        //    foreach (SearchProperty prop in phrase.Where(x => x is SearchProperty))
        //    {
        //        result.Add(prop);
        //    }
        //    foreach (LogicPhraseDTO logic in phrase.Where(x => x is LogicPhraseDTO))
        //    {
        //        return GetSearchProperties(logic.Phrases, result);
        //    }
        //    return result;
        //}

        public DP_SearchRepositoryMain GetSearchRepository()
        {
            //** EntityDefinedSearchArea.GetSearchRepository: b1b5fa67fe2d

            var columnsSearchRepository = new DP_SearchRepositoryMain(AreaInitializer.EntityID);
            columnsSearchRepository.Title = AreaInitializer.Title;
            //       columnsSearchRepository.IsSimpleSearch = true;
            //     columnsSearchRepository.EntitySearchID = EntitySearchDTO == null ? 0 : EntitySearchDTO.ID;
            foreach (var property in SimpleColumnControls)
            {
                var value = property.ControlManager.GetUIControlManager().GetValue();
                if (PropertyHasValue(property, value))
                {
                    LogicPhraseDTO logic = null;
                    if (property.EntitySearchColumn.RelationshipTail == null)
                        logic = columnsSearchRepository;
                    else
                        logic = AgentHelper.GetOrCreateSearchRepositoryFromRelationshipTail(columnsSearchRepository, property.EntitySearchColumn.RelationshipTail);

                    SearchProperty searchProperty = new SearchProperty(property.Column);
                    //    searchProperty.SearchColumnID = property.EntitySearchColumn != null ? property.EntitySearchColumn.ID : 0;

                    searchProperty.IsKey = property.Column.PrimaryKey;
                    searchProperty.Value = value;
                    searchProperty.Operator = property.ControlManager.GetUIControlManager().GetOperator();
                    logic.Phrases.Add(searchProperty);
                }
            }

            foreach (var relControl in RelationshipColumnControls)
            {
                if (relControl.EditNdTypeArea.GetDataList().Any())
                {
                    var logic = AgentHelper.GetOrCreateSearchRepositoryFromRelationshipTail(columnsSearchRepository, relControl.EntitySearchColumn.RelationshipTail);
                    logic.AndOrType = AndOREqualType.Or;
                    foreach (var data in relControl.EditNdTypeArea.GetDataList())
                    {
                        var logicPhraseSelectedDate = new LogicPhraseDTO();
                        logicPhraseSelectedDate.AndOrType = AndOREqualType.And;
                        foreach (var property in data.KeyProperties)
                        {
                            SearchProperty searchProperty = new SearchProperty(property.Column);
                            searchProperty.IsKey = property.Column.PrimaryKey;
                            searchProperty.Value = property.Value;
                            searchProperty.Operator = CommonOperator.Equals;
                            logicPhraseSelectedDate.Phrases.Add(searchProperty);
                        }
                        logic.Phrases.Add(logicPhraseSelectedDate);
                    }
                }
            }

            DP_SearchRepositoryMain quickSearchRepository = null;
            if (!string.IsNullOrEmpty(SimpleSearchView.QuickSearchText))
                quickSearchRepository = AgentHelper.GetQuickSearchLogicPhrase(EntitySearchDTO, AreaInitializer.EntityID, SimpleSearchView.QuickSearchText);
            if (quickSearchRepository != null && quickSearchRepository.Phrases.Any() && !columnsSearchRepository.Phrases.Any())
            {
                return quickSearchRepository;
            }
            else if ((quickSearchRepository == null || !quickSearchRepository.Phrases.Any()) && columnsSearchRepository.Phrases.Any())
            {
                return columnsSearchRepository;
            }
            var resultSearchRepository = new DP_SearchRepositoryMain(AreaInitializer.EntityID);
            resultSearchRepository.Title = AreaInitializer.Title;
            if ((quickSearchRepository != null && quickSearchRepository.Phrases.Any()) && columnsSearchRepository.Phrases.Any())
            {
                resultSearchRepository.Phrases.Add(quickSearchRepository);
                resultSearchRepository.Phrases.Add(columnsSearchRepository);
            }
            return resultSearchRepository;
        }


        public PreDefinedSearchDTO GetSearchRepositoryForSave()
        {
            //** EntityDefinedSearchArea.GetSearchRepositoryForSave: ab359aa2461b
            var result = new PreDefinedSearchDTO();

            result.QuickSearchValue = SimpleSearchView.QuickSearchText;
            foreach (var property in SimpleColumnControls)
            {
                var value = property.ControlManager.GetUIControlManager().GetValue();

                if (PropertyHasValue(property, value))
                {
                    List<object> values = new List<object>();
                    values.Add(value);
                    result.SimpleSearchProperties.Add(new DP_PreDefinedSearchSimpleColumn() { EntitySearchColumnsID = property.EntitySearchColumn.ID, Value = values });
                }
                else if (property.Formula != null)
                {
                    result.SimpleSearchProperties.Add(new DP_PreDefinedSearchSimpleColumn() { EntitySearchColumnsID = property.EntitySearchColumn.ID, FormulaID = property.Formula.ID });
                }
            }

            foreach (var relControl in RelationshipColumnControls)
            {
                if (relControl.EditNdTypeArea.GetDataList().Any())
                {
                    var relItem = new DP_PreDefinedSearchRelationship();
                    relItem.EntitySearchColumnsID = relControl.EntitySearchColumn.ID;
                    foreach (var data in relControl.EditNdTypeArea.GetDataList())
                    {
                        var relData = new DP_PreDefinedSearchRelationshipData();
                        foreach (var property in data.KeyProperties)
                        {
                            relData.KeyProperties.Add(new DP_PreDefinedSearchRelationshipColumns() { Column = property.Column, Value = property.Column });
                        }
                        relItem.DataItems.Add(relData);
                    }
                    result.RelationshipSearchProperties.Add(relItem);
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

        //private bool SearchValueIsEmpty(SimpleSearchColumnControl typePropertyControl, string value)
        //{
        //    if (typePropertyControl is NullColumnControl)
        //        return string.IsNullOrEmpty(value) || value == "false" || value == "0";
        //    else if (typePropertyControl is RelationCheckColumnControl || typePropertyControl is RelationCountCheckColumnControl)
        //        return string.IsNullOrEmpty(value) || value == "false" || value == "0";
        //    else
        //        return string.IsNullOrEmpty(value) || value == "0";
        //}



        public void OnSearchDataDefined(DP_SearchRepositoryMain searchData)
        {
            if (SearchDataDefined != null)
            {
                SearchDataDefined(this, searchData);
            }
        }


    }
}
