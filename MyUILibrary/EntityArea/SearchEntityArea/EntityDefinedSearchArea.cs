using AutoMapper;
using CommonDefinitions.UISettings;
using ModelEntites;

using MyUILibrary;
using MyUILibrary.EntityArea;
using MyUILibrary.EntityArea.Commands;

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
        public EntityDefinedSearchArea(SearchAreaInitializer newAreaInitializer)
        {
            //** d8ed853b-7d7a-4788-9ef6-de46ee61a50f
            SimpleColumnControls = new List<SimpleSearchColumnControl>();
            RelationshipColumnControls = new List<RelationshipSearchColumnControl>();
            SearchInitializer = newAreaInitializer;


            SimpleSearchView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfSearchEntityArea(EntitySearchDTO.EntityUISetting);
            ManageSimpleSearchView();

            var searchClearCommand = new SearchClearCommand(this);
            SimpleSearchView.AddCommand(searchClearCommand.CommandManager);
            var simpleSearchconfirmcommand = new SimpleSearchConfirmCommand(this);
            SimpleSearchView.AddCommand(simpleSearchconfirmcommand.CommandManager);

        }

        EntitySearchDTO _EntitySearch;
        public EntitySearchDTO EntitySearchDTO
        {
            get
            {
                if (_EntitySearch == null)
                {
                    if (SearchInitializer.EntitySearchID != 0)
                        _EntitySearch = AgentUICoreMediator.GetAgentUICoreMediator.DataSearchManager.GetEntitySearch(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), SearchInitializer.EntitySearchID);
                    else
                        _EntitySearch = AgentUICoreMediator.GetAgentUICoreMediator.DataSearchManager.GetDefaultEntitySearch(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), SearchInitializer.EntityID);
                }
                return _EntitySearch;
            }
        }

        //  DP_SearchRepositoryMain firstRepository { set; get; }
        public I_View_SimpleSearchEntityArea SimpleSearchView { set; get; }

        public SearchAreaInitializer SearchInitializer { set; get; }

        //public event EventHandler<Arg_PackageSelected> DataPackageSelected;
        public event EventHandler<SearchDataArg> SearchDataDefined;
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






        private void ManageSimpleSearchView()
        {
            //** 280ad2a0-760e-4ad2-9acb-31d456f709e0

            SimpleSearchView.QuickSearchVisiblity = EntitySearchDTO.EntitySearchAllColumns.Any(x => x.ColumnID != 0 && x.ExcludeInQuickSearch == false);
            foreach (var searchcolumn in EntitySearchDTO.EntitySearchAllColumns.OrderBy(x => x.OrderID))
            {
                if (searchcolumn.Column == null)
                {
                    if (searchcolumn.RelationshipTail != null)
                    {
                        var propertyControl = new RelationshipSearchColumnControl();
                        propertyControl.RelationshipTail = searchcolumn.RelationshipTail;
                        propertyControl.EntitySearchColumn = searchcolumn;

                        EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
                        editEntityAreaInitializer1.EntityID = searchcolumn.RelationshipTail.TargetEntityID;
                        editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                        editEntityAreaInitializer1.DataMode = CommonDefinitions.UISettings.DataMode.Multiple;
                        var FirstSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer1);
                        if (FirstSideEditEntityAreaResult.Item1 != null)
                        {
                            propertyControl.EditNdTypeArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaMultipleData;
                            propertyControl.ControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateRelControlManagerForOneDataForm(propertyControl.EditNdTypeArea.TemporaryDisplayView, searchcolumn.RelationshipUISetting);
                            propertyControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(propertyControl.EntitySearchColumn.Alias);

                            if (!string.IsNullOrEmpty(propertyControl.EntitySearchColumn.Tooltip))
                                propertyControl.LabelControlManager.SetTooltip(propertyControl.EntitySearchColumn.Tooltip);
                        }
                        RelationshipColumnControls.Add(propertyControl);
                        SimpleSearchView.AddView(propertyControl.LabelControlManager, propertyControl.ControlManager);
                        ////اینجا چیه بررسی شود چرا اینجا؟
                        //if (propertyControl.EditNdTypeArea.SimpleEntity.SearchInitially == true)
                        //    propertyControl.EditNdTypeArea.SearchViewEntityArea.SearchInitialy();

                    }

                }
                else
                {
                    var propertyControl = new SimpleSearchColumnControl();
                    propertyControl.Column = searchcolumn.Column;
                    propertyControl.EntitySearchColumn = searchcolumn;
                    propertyControl.Operators = searchcolumn.Operators;
                    propertyControl.ControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateSimpleControlManagerForOneDataForm(propertyControl.Column, searchcolumn.ColumnUISetting, false, propertyControl.Operators);
                    propertyControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(propertyControl.EntitySearchColumn.Alias);

                    if (!string.IsNullOrEmpty(propertyControl.EntitySearchColumn.Tooltip))
                        propertyControl.LabelControlManager.SetTooltip(propertyControl.EntitySearchColumn.Tooltip);

                    if (propertyControl.Operators.Any(x => x.IsDefault))
                        propertyControl.ControlManager.GetUIControlManager().SetOperator(propertyControl.Operators.First(x => x.IsDefault).Operator);
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



        public void ClearSearchData()
        {



        }
        public void ShowPreDefinedSearch(PreDefinedSearchDTO preDefinedSearch)
        {
            //** 2a9ef0e0-b74b-4502-85de-cd004ddc85ff

            SimpleSearchView.QuickSearchText = preDefinedSearch.QuickSearchValue;
            foreach (var phrase in preDefinedSearch.SimpleSearchProperties)
            {
                if (SimpleColumnControls.Any(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID))
                {
                    SimpleColumnControls.First(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID).ControlManager.GetUIControlManager().SetValue(phrase.Value);
                    SimpleColumnControls.First(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID).ControlManager.GetUIControlManager().SetOperator(phrase.Operator);
                }
            }
            foreach (var phrase in preDefinedSearch.RelationshipSearchProperties)
            {
                if (RelationshipColumnControls.Any(x => x.EntitySearchColumn.ID == phrase.EntitySearchColumnsID))
                {
                    List<Dictionary<int, object>> items = new List<Dictionary<int, object>>();
                    foreach (var item in phrase.DataItems)
                    {
                        Dictionary<int, object> data = new Dictionary<int, object>();
                        foreach(var col in item.KeyProperties)
                        {
                            data.Add(col.ColumnID, col.Value);
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
            //** c056cb29-86d4-4003-b6dc-b1b5fa67fe2d
            //      if (firstRepository == null)

        

            var columnsSearchRepository = new DP_SearchRepositoryMain(SearchInitializer.EntityID);
            columnsSearchRepository.Title = SearchInitializer.Title;
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

                    SearchProperty searchProperty = new SearchProperty();
                //    searchProperty.SearchColumnID = property.EntitySearchColumn != null ? property.EntitySearchColumn.ID : 0;
                    searchProperty.ColumnID = property.Column.ID;
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
                            SearchProperty searchProperty = new SearchProperty();
                            searchProperty.ColumnID = property.Column.ID;
                            searchProperty.IsKey = property.Column.PrimaryKey;
                            searchProperty.Value = property.Value;
                            searchProperty.Operator = CommonOperator.Equals;
                            logicPhraseSelectedDate.Phrases.Add(searchProperty);
                        }
                        logic.Phrases.Add(logicPhraseSelectedDate);
                    }
                }
                //else
                //{
                //    var text = relControl.EditNdTypeArea.TemporaryDisplayView.GetSearchText();
                //    if (!string.IsNullOrEmpty(text))
                //    {
                //        var entitySearch = relControl.EditNdTypeArea.SearchEntityArea.SimpleSearchEntityArea.EntitySearchDTO;
                //        var quickSearchLogic = GetQuickSearchLogicPhrase(text, entitySearch);

                //        firstRepository.Phrases.Add(quickSearchLogic);
                //    }
                //}
            }

            return columnsSearchRepository;


            //if (quickSearchRepository != null && quickSearchRepository.Phrases.Any() && !columnsSearchRepository.Phrases.Any())
            //{
            //    return quickSearchRepository;
            //}
            //else if ((quickSearchRepository == null || !quickSearchRepository.Phrases.Any()) && columnsSearchRepository.Phrases.Any())
            //{
            //    return columnsSearchRepository;
            //}

            //var resultSearchRepository = new DP_SearchRepositoryMain(SearchInitializer.EntityID);
            //resultSearchRepository.Title = SearchInitializer.Title;
            //resultSearchRepository.IsSimpleSearch = true;
            //resultSearchRepository.EntitySearchID = EntitySearchDTO == null ? 0 : EntitySearchDTO.ID;
            //if ((quickSearchRepository != null && quickSearchRepository.Phrases.Any()) && columnsSearchRepository.Phrases.Any())
            //{
            //    resultSearchRepository.Phrases.Add(quickSearchRepository);
            //    resultSearchRepository.Phrases.Add(columnsSearchRepository);
            //}
            //return resultSearchRepository;
        }


        public PreDefinedSearchDTO GetSearchRepositoryForSave()
        {
            var result = new PreDefinedSearchDTO();
            result.EntitySearchID = EntitySearchDTO.ID;
            result.QuickSearchValue = SimpleSearchView.QuickSearchText;
            foreach (var property in SimpleColumnControls)
            {
                var value = property.ControlManager.GetUIControlManager().GetValue();

                if (PropertyHasValue(property, value))
                {
                    List<object> values = new List<object>();
                    values.Add(value);
                    result.SimpleSearchProperties.Add(new DP_PreDefinedSearchSimpleColumn(){ EntitySearchColumnsID = property.EntitySearchColumn.ID, Value = values });

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
                            relData.KeyProperties.Add(new DP_PreDefinedSearchRelationshipColumns() { ColumnID= property .ColumnID,Value= property .Value} );
                        }
                        relItem.DataItems.Add(relData);
                    }
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



        public void OnSearchDataDefined(DP_SearchRepositoryMain searchData)
        {
            if (SearchDataDefined != null)
            {
                SearchDataDefined(this, new SearchDataArg() { SearchItems = searchData });
            }
        }


    }
}
