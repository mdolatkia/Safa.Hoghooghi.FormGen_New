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


        //  DP_SearchRepository firstRepository { set; get; }
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
                            propertyControl.EditNdTypeArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
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
        public bool ShowSearchRepository(DP_SearchRepository item)
        {
            //** 2a9ef0e0-b74b-4502-85de-cd004ddc85ff

            foreach (var phrase in item.Phrases)
            {
                if (phrase is SearchProperty)
                {
                    if (!SimpleColumnControls.Any(x => x.EntitySearchColumn.ID == (phrase as SearchProperty).SearchColumnID))
                        return false;
                }
                else
                    return false;
            }

            foreach (SearchProperty phrase in item.Phrases)
            {

                var columnControl = SimpleColumnControls.FirstOrDefault(x => x.EntitySearchColumn.ID == (phrase as SearchProperty).SearchColumnID);

                //برای عادی ها جواب میده اگر خودش کمنترل بود چی
                columnControl.ControlManager.GetUIControlManager().SetValue(phrase.Value);
                //  if (columnControl.ControlManager.HasOperator())
                columnControl.ControlManager.GetUIControlManager().SetOperator(phrase.Operator);

            }
            return true;
        }


        private List<SearchProperty> GetSearchProperties(List<Phrase> phrase, List<SearchProperty> result = null)
        {
            if (result == null)
                result = new List<SearchProperty>();
            foreach (SearchProperty prop in phrase.Where(x => x is SearchProperty))
            {
                result.Add(prop);
            }
            foreach (LogicPhraseDTO logic in phrase.Where(x => x is LogicPhraseDTO))
            {
                return GetSearchProperties(logic.Phrases, result);
            }
            return result;
        }

        public DP_SearchRepository GetSearchRepository()
        {
            //** c056cb29-86d4-4003-b6dc-b1b5fa67fe2d
            //      if (firstRepository == null)

            DP_SearchRepository quickSearchRepository = null;
            if (!string.IsNullOrEmpty(SimpleSearchView.QuickSearchText))
            {
                quickSearchRepository = GetQuickSearchLogicPhrase(SimpleSearchView.QuickSearchText);
            }


            var columnsSearchRepository = new DP_SearchRepository(SearchInitializer.EntityID);
            columnsSearchRepository.Title = SearchInitializer.Title;
            columnsSearchRepository.IsSimpleSearch = true;
            columnsSearchRepository.EntitySearchID = EntitySearchDTO == null ? 0 : EntitySearchDTO.ID;
            foreach (var property in SimpleColumnControls)
            {
                var value = property.ControlManager.GetUIControlManager().GetValue();
                if (PropertyHasValue(property, value))
                {
                    LogicPhraseDTO logic = null;
                    if (property.EntitySearchColumn.RelationshipTail == null)
                        logic = columnsSearchRepository;
                    else
                        logic = GetOrCreateSearchRepositoryFromRelationshipTail(columnsSearchRepository, property.EntitySearchColumn.RelationshipTail);

                    SearchProperty searchProperty = new SearchProperty();
                    searchProperty.SearchColumnID = property.EntitySearchColumn != null ? property.EntitySearchColumn.ID : 0;
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
                    var logic = GetOrCreateSearchRepositoryFromRelationshipTail(columnsSearchRepository, relControl.EntitySearchColumn.RelationshipTail);
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




            if (quickSearchRepository != null && quickSearchRepository.Phrases.Any() && !columnsSearchRepository.Phrases.Any())
            {
                return quickSearchRepository;
            }
            else if ((quickSearchRepository == null || !quickSearchRepository.Phrases.Any()) && columnsSearchRepository.Phrases.Any())
            {
                return columnsSearchRepository;
            }

            var resultSearchRepository = new DP_SearchRepository(SearchInitializer.EntityID);
            resultSearchRepository.Title = SearchInitializer.Title;
            resultSearchRepository.IsSimpleSearch = true;
            resultSearchRepository.EntitySearchID = EntitySearchDTO == null ? 0 : EntitySearchDTO.ID;
            if ((quickSearchRepository != null && quickSearchRepository.Phrases.Any()) && columnsSearchRepository.Phrases.Any())
            {
                resultSearchRepository.Phrases.Add(quickSearchRepository);
                resultSearchRepository.Phrases.Add(columnsSearchRepository);
            }
            return resultSearchRepository;
        }
        internal static DP_SearchRepository GetOrCreateSearchRepositoryFromRelationshipTail(LogicPhraseDTO result, EntityRelationshipTailDTO relationshipTail)
        {
            if (relationshipTail == null)
            {
                var searchRep = result as DP_SearchRepository;
                return searchRep;
            }
            else
            {
                if (result.Phrases.Any(x => x is DP_SearchRepository && (x as DP_SearchRepository).SourceRelationship.ID == relationshipTail.Relationship.ID))
                {
                    var childSearchPhrase = result.Phrases.First(x => x is DP_SearchRepository && (x as DP_SearchRepository).SourceRelationship.ID == relationshipTail.Relationship.ID);
                    return GetOrCreateSearchRepositoryFromRelationshipTail(childSearchPhrase as LogicPhraseDTO, relationshipTail.ChildTail);
                }
                else
                {
                    DP_SearchRepository childSearchPhrase = new DP_SearchRepository(relationshipTail.Relationship.EntityID2);
                    childSearchPhrase.SourceRelationship = relationshipTail.Relationship;
                    childSearchPhrase.Title = relationshipTail.EntityPath;
                    //childSearchPhrase.TargetEntityID = ;
                    //   childSearchPhrase.SourceToTargetRelationshipType = relationshipTail.Relationship.TypeEnum;
                    //  childSearchPhrase.SourceToTargetMasterRelationshipType = relationshipTail.Relationship.MastertTypeEnum;
                    result.Phrases.Add(childSearchPhrase);
                    return GetOrCreateSearchRepositoryFromRelationshipTail(childSearchPhrase, relationshipTail.ChildTail);
                }
            }
        }
        public DP_SearchRepository GetQuickSearchLogicPhrase(string text)
        {
            //** 2e5350c5-667f-40af-b346-8384c544000f
            var quickSearchLogic = new DP_SearchRepository(SearchInitializer.EntityID);
            quickSearchLogic.Title = SearchInitializer.Title;
            quickSearchLogic.IsSimpleSearch = true;
            quickSearchLogic.EntitySearchID = EntitySearchDTO == null ? 0 : EntitySearchDTO.ID;
            quickSearchLogic.AndOrType = AndOREqualType.Or;
            foreach (var item in EntitySearchDTO.EntitySearchAllColumns)
            {
                if (item.ColumnID != 0 && item.ExcludeInQuickSearch == false)
                {
                    SearchProperty searchProperty = new SearchProperty();
                    searchProperty.SearchColumnID = item.ID;
                    searchProperty.ColumnID = item.ColumnID;
                    searchProperty.IsKey = item.Column.PrimaryKey;
                    searchProperty.Value = text;
                    DP_SearchRepository logic = null;
                    if (item.RelationshipTail == null)
                        logic = quickSearchLogic;
                    else
                    {
                        logic = GetOrCreateSearchRepositoryFromRelationshipTail(quickSearchLogic, item.RelationshipTail);
                        logic.AndOrType = AndOREqualType.Or;
                    }

                    int n;
                    var isNumeric = int.TryParse(text, out n);
                    DateTime a;
                    var isDateTime = DateTime.TryParse(text, out a);

                    if (item.Column.ColumnType == Enum_ColumnType.Numeric)
                    {
                        if (!isNumeric)
                            continue;
                        searchProperty.Operator = CommonOperator.Equals;
                    }
                    else if (item.Column.ColumnType == Enum_ColumnType.Date)
                        continue;
                    else if (item.Column.ColumnType == Enum_ColumnType.Boolean)
                        continue;
                    else if (item.Column.ColumnType == Enum_ColumnType.String)
                        searchProperty.Operator = CommonOperator.Contains;
                    else
                        continue;
                    logic.Phrases.Add(searchProperty);
                }
            }
            return quickSearchLogic;
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



        public void OnSearchDataDefined(DP_SearchRepository searchData)
        {
            if (SearchDataDefined != null)
            {
                SearchDataDefined(this, new SearchDataArg() { SearchItems = searchData });
            }
        }


    }
}
