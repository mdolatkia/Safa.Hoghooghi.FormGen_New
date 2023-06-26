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
    public class AdvancedSearchEntityArea : I_AdvancedSearchEntityArea
    {
        public event EventHandler<SimpleSearchColumnControl> FormulaSelectionRequested;

        public AdvancedSearchEntityArea(SearchAreaInitializer newAreaInitializer)
        {
            //AdvancedSearchEntityArea: ef3528f2b0f5
            SearchCommands = new List<I_Command>();
            AreaInitializer = newAreaInitializer;

            AdvancedSearchView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfAdvancedSearch();

            AddCommands();

          

            if (newAreaInitializer.AdvancedSearchDTOMessage == null)
                ClearSearchData();
            else
                ShowSearchRepository(newAreaInitializer.AdvancedSearchDTOMessage);


        }

        private void AddCommands()
        {
            // AdvancedSearchEntityArea.AddCommands: 24fb53a7c80e
            var clearSearchcommand = new SearchClearCommand(this);
            AdvancedSearchView.AddCommand(clearSearchcommand.CommandManager);
            if (!AreaInitializer.ForSave)
            {
                var searchConfirmcommand = new AdvancedSearchConfirmCommand(this);
                AdvancedSearchView.AddCommand(searchConfirmcommand.CommandManager);
            }
        }

        public I_View_AdvancedSearchEntityArea AdvancedSearchView { set; get; }

        public List<I_Command> SearchCommands
        {
            set;
            get;
        }
        public SearchAreaInitializer AreaInitializer { set; get; }

        //public event EventHandler<Arg_PackageSelected> DataPackageSelected;
        public event EventHandler<DP_SearchRepositoryMain> SearchDataDefined;

        TableDrivedEntityDTO _FullEntity;
        public TableDrivedEntityDTO FullEntity
        {
            get
            {
                if (_FullEntity == null)
                    _FullEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetPermissionedEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                return _FullEntity;
            }
        }

        public DP_SearchRepositoryMain RootSearchRepository { get; private set; }

        //     AdvanceSearchNode RootNode;

        //public void GenerateSearchControls()
        //{
        //    RootSearchRepository = new DP_SearchRepositoryMain(SearchInitializer.EntityID);
        //    RootSearchRepository.Title = FullEntity.Alias;
        //    AddNode(null, RootSearchRepository);
        //}

        public bool ShowSearchRepository(DP_SearchRepositoryMain item)
        {
            //AdvancedSearchEntityArea.ShowSearchRepository:  8eb8b7ca6c92
            //   RootNode = null;
            RootSearchRepository = item;
            AdvancedSearchView.ClearTreeItems();
            AddNode(null, item);
            return true;
        }

        private void AddNode(AdvanceSearchNode parentNode, Phrase phrase)
        {
            // ** AdvancedSearchEntityArea.AddNode: dc5ef44a6503
            if (phrase is LogicPhraseDTO)
            {
                var logicPhrase = phrase as LogicPhraseDTO;

                var newnode = new AdvanceSearchNode();
                newnode.Phrase = logicPhrase;
                newnode.ParentNode = parentNode;
                if (parentNode != null)
                    parentNode.ChildItems.Add(newnode);
                if (logicPhrase is DP_SearchRepositoryMain)
                {
                    newnode.Title = (logicPhrase as DP_SearchRepositoryMain).Title;
                }
                else if (logicPhrase is DP_SearchRepositoryRelationship)
                {
                    newnode.Title = (logicPhrase as DP_SearchRepositoryRelationship).Title;
                }
                else
                {
                    newnode.Title = "عبارت منطقی";
                }
                //if (parentNode != null)
                //{
                //    if (logicPhrase is DP_SearchRepositoryMain)
                //        newnode.EntityID = (logicPhrase as DP_SearchRepositoryMain).TargetEntityID;
                //    else
                //        newnode.EntityID = parentNode.EntityID;
                //}
                //else
                //    newnode.EntityID = SearchInitializer.EntityID;


                if (parentNode != null)
                    newnode.NodeManager = parentNode.NodeManager.AddChildItem();
                else
                    newnode.NodeManager = AdvancedSearchView.AddTreeItem();

                newnode.NodeManager.SetHeader(newnode.Title, "");
                var list = GetAndOrList();
                newnode.NodeManager.AddLogicComboBox(list);
                newnode.NodeManager.SetLogicComboBoxValue(list.FirstOrDefault(x => x.AndOR == logicPhrase.AndOrType));
                newnode.NodeManager.LogicComboBoxChanged += (sender, e) => LogicComboBoxChanged(sender, e, newnode, logicPhrase);

                foreach (var item in logicPhrase.Phrases)
                {
                    AddNode(newnode, item);
                }

                I_AdvanceSearchMenu rootAndMenu = newnode.NodeManager.AddMenu("افزودن جمله عبارت");
                rootAndMenu.Clicked += (sender, e) => AddLogicPhraseClicked(sender, e, newnode, logicPhrase);

                var searchRepositories = GetParentSearchRepositories(newnode);
                var last = searchRepositories.Last();

                I_AdvanceSearchMenu simpleSearchMenu = newnode.NodeManager.AddMenu("افزودن لیست خصوصیات");
                simpleSearchMenu.Clicked += (sender1, e1) => AddListSearchProperty(sender1, e1, newnode, logicPhrase, last);


                var entity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetPermissionedEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), last);
                foreach (var relationship in entity.Relationships)
                {
                    I_AdvanceSearchMenu relationshipSearchMenu = newnode.NodeManager.AddMenu(relationship.Alias);
                    relationshipSearchMenu.Clicked += (sender1, e1) => RelationshipSearchRepository_AddNew(sender1, e1, newnode, logicPhrase, relationship);
                }
                if (parentNode != null)
                {
                    I_AdvanceSearchMenu removeMenu = newnode.NodeManager.AddMenu("حذف");
                    removeMenu.Clicked += (sender1, e1) => RemoveMenu_Clicked(sender1, e1, parentNode, newnode);
                }
                //   return newnode;
            }
            else if (phrase is SearchProperty)
            {
                var propertyPhrase = phrase as SearchProperty;
                var newnode = new AdvanceSearchNode();
                newnode.ParentNode = parentNode;
                if (AreaInitializer.ForSave)
                {
                    if (PropertyHasValue(propertyPhrase))
                    {
                        newnode.Title = propertyPhrase.Column.Alias + " , " + propertyPhrase.Operator.ToString() + " : " + propertyPhrase.Value;
                    }
                    else if (propertyPhrase.Formula != null)
                    {
                        newnode.Title = propertyPhrase.Column.Alias + " , " + propertyPhrase.Operator.ToString() + " : " + propertyPhrase.Formula.Title;
                    }
                }
                else
                {
                    newnode.Title = propertyPhrase.Column.Alias + " , " + propertyPhrase.Operator.ToString() + " : " + propertyPhrase.Value;
                }

                parentNode.ChildItems.Add(newnode);
                newnode.NodeManager = parentNode.NodeManager.AddChildItem();
                newnode.NodeManager.SetHeader(newnode.Title, propertyPhrase.Tooltip);
                newnode.Phrase = phrase;

                I_AdvanceSearchMenu removeMenu = newnode.NodeManager.AddMenu("حذف");
                removeMenu.Clicked += (sender1, e1) => RemoveMenu_Clicked(sender1, e1, parentNode, newnode);
                //   return newnode;
            }
            //return null;
        }
        private bool PropertyHasValue(SearchProperty property)
        {
            return property.Value != null && !string.IsNullOrEmpty(property.ToString()) && property.ToString().ToLower() != "0";
        }
        private List<int> GetParentSearchRepositories(AdvanceSearchNode newnode)
        {
            List<int> result = new List<int>();
            GetParentSearchRepositories(newnode, result);
            result.Reverse();
            return result;
        }
        private void GetParentSearchRepositories(AdvanceSearchNode newnode, List<int> result)
        {
            if (result == null)
                result = new List<int>();
            if (newnode.Phrase is DP_SearchRepositoryMain)
            {
                result.Add((newnode.Phrase as DP_SearchRepositoryMain).TargetEntityID);

            }
            else if (newnode.Phrase is DP_SearchRepositoryRelationship)
            {
                result.Add((newnode.Phrase as DP_SearchRepositoryRelationship).SourceRelationship.EntityID2);

            }
            if (newnode.ParentNode != null)
            {
                GetParentSearchRepositories(newnode.ParentNode, result);
            }
            else
            {
                result.Add(AreaInitializer.EntityID);
                return;
            }
        }

        private void LogicComboBoxChanged(object sender, LogicComboBoxChangedEventArg e, AdvanceSearchNode newnode, LogicPhraseDTO logicPhrase)
        {
            logicPhrase.AndOrType = e.Item.AndOR;
        }

        private List<AndORListItem> GetAndOrList()
        {
            List<AndORListItem> result = new List<AndORListItem>();
            result.Add(new AndORListItem() { AndOR = AndOREqualType.And, Title = "And", IsDefault = true });
            result.Add(new AndORListItem() { AndOR = AndOREqualType.Or, Title = "Or" });
            result.Add(new AndORListItem() { AndOR = AndOREqualType.NotAnd, Title = "NotAnd" });
            result.Add(new AndORListItem() { AndOR = AndOREqualType.NotOr, Title = "NotOr" });
            return result;
        }

        private void AddLogicPhraseClicked(object sender, EventArgs e, AdvanceSearchNode parentNode, LogicPhraseDTO parentLogicPhrase)
        {
            var newLogicPhrase = new LogicPhraseDTO();
            parentLogicPhrase.Phrases.Add(newLogicPhrase);
            AddNode(parentNode, newLogicPhrase);
        }

        private void RelationshipSearchRepository_AddNew(object sender1, EventArgs e1, AdvanceSearchNode parentNode, LogicPhraseDTO parentLogicPhrase
            , RelationshipDTO relationship)
        {
            //AdvancedSearchEntityArea.RelationshipSearchRepository_AddNew: bf90baf6917a
            var relSearchRepository = new DP_SearchRepositoryRelationship();
            relSearchRepository.Title = relationship.Alias;
            relSearchRepository.SourceRelationship = relationship;

            parentLogicPhrase.Phrases.Add(relSearchRepository);

            AddNode(parentNode, relSearchRepository);
            //var searchViewInitializer = new SearchAreaInitializer();
            //searchViewInitializer.EntityID = relationship.EntityID2;
            //searchViewInitializer.Title = relationship.Alias;
            //// searchViewInitializer.SourceRelationship = relationship;
            //SearchEntityArea advancedAndRaw = new EntityArea.SearchEntityArea(searchViewInitializer);
            //advancedAndRaw.SearchDataDefined += (sender, e) => AdvancedAndRaw_SearchDataDefinedNew(sender, e, relationship, node, advancedAndRaw.SearchView);

            //AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(advancedAndRaw.SearchView, searchViewInitializer.Title, Enum_WindowSize.Big);
        }
        //private void AdvancedAndRaw_SearchDataDefinedNew(object sender, DP_SearchRepositoryMain e, RelationshipDTO relationship, AdvanceSearchNode node, object view)
        //{
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(view);

        //    if (node.Phrase is LogicPhraseDTO)
        //        (node.Phrase as LogicPhraseDTO).Phrases.Add(e.SearchItems);

        //    AddNode(node, e.SearchItems);
        //}
        //private void Relationship_ClickedEdit(object sender1, EventArgs e1, AdvanceSearchNode node, DP_SearchRepositoryMain DP_SearchRepositoryMain)
        //{

        //    var searchViewInitializer = new SearchAreaInitializer();
        //    searchViewInitializer.PreDefinedSearch = DP_SearchRepositoryMain;
        //    searchViewInitializer.EntityID = DP_SearchRepositoryMain.TargetEntityID;
        //    searchViewInitializer.Title = DP_SearchRepositoryMain.Title;
        //    //   searchViewInitializer.SourceRelationship = DP_SearchRepositoryMain.SourceRelationship;
        //    SearchEntityArea advancedAndRaw = new EntityArea.SearchEntityArea(searchViewInitializer);
        //    advancedAndRaw.SearchDataDefined += (sender, e) => AdvancedAndRaw_SearchDataDefinedEdit(sender, e, node, advancedAndRaw.SearchView);
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(advancedAndRaw.SearchView, searchViewInitializer.Title, Enum_WindowSize.Big);
        //}
        //private void AdvancedAndRaw_SearchDataDefinedEdit(object sender, DP_SearchRepositoryMain e, AdvanceSearchNode node, object view)
        //{
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(view);

        //    if (e.SearchItems != null)
        //    {
        //        node.ParentNode.NodeManager.RemoveItem(node.NodeManager);
        //        node.ParentNode.ChildItems.Remove(node);
        //        (node.ParentNode.Phrase as LogicPhraseDTO).Phrases.Remove(node.Phrase);
        //        (node.ParentNode.Phrase as LogicPhraseDTO).Phrases.Add(e.SearchItems);
        //        AddNode(node.ParentNode, e.SearchItems);
        //    }


        //}
        //private bool GetParentSearchRepository(AdvanceSearchNode newnode)
        //{
        //    if (newnode.Phrase is DP_SearchRepositoryMain)
        //    {
        //        if ((newnode.Phrase as DP_SearchRepositoryMain) == firstRepository)
        //            return true;
        //        else
        //            return false;
        //    }

        //    if (newnode.ParentNode == null)
        //        return false;
        //    else if (newnode.ParentNode.Phrase is DP_SearchRepositoryMain)
        //    {
        //        if ((newnode.ParentNode.Phrase as DP_SearchRepositoryMain) == firstRepository)
        //            return true;
        //        else return false;
        //    }
        //    else
        //        return GetParentSearchRepository(newnode.ParentNode);
        //}




        private void RemoveMenu_Clicked(object sender, EventArgs e, AdvanceSearchNode parentNode, AdvanceSearchNode node)
        {
            if (parentNode != null)
            {
                parentNode.NodeManager.RemoveItem(node.NodeManager);
                parentNode.ChildItems.Remove(node);
                if (parentNode.Phrase is LogicPhraseDTO)
                {
                    (parentNode.Phrase as LogicPhraseDTO).Phrases.Remove(node.Phrase);
                }
            }
        }
        private void AddListSearchProperty(object sender1, EventArgs e1, AdvanceSearchNode parentNode, LogicPhraseDTO parentLogicPhrase, int entityID)
        {
            //** AdvancedSearchEntityArea.AddListSearchProperty: c1729589c7ea
            var searchViewInitializer = new SearchAreaInitializer();

            searchViewInitializer.EntityID = entityID;
            //if (SearchInitializer.TempEntity != null && SearchInitializer.TempEntity.ID == SearchInitializer.EntityID)
            //    searchViewInitializer.TempEntity = SearchInitializer.TempEntity;
            searchViewInitializer.ForSave = AreaInitializer.ForSave;
            RawSearchEntityArea rawSearchEntityArea = new RawSearchEntityArea(searchViewInitializer);
            rawSearchEntityArea.FormulaSelectionRequested += RawSearchEntityArea_FormulaSelectionRequested;
            rawSearchEntityArea.SearchDataDefined += (sender, e) => RawSearchEntityArea_SearchDataDefined(sender, e, parentNode, parentLogicPhrase, rawSearchEntityArea.RawSearchView);
            //lastSearchView = rawSearchEntityArea.RawSearchView;
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(rawSearchEntityArea.RawSearchView, "خصوصیات", Enum_WindowSize.Big);

        }

        private void RawSearchEntityArea_FormulaSelectionRequested(object sender, SimpleSearchColumnControl e)
        {
            if (FormulaSelectionRequested != null)
                FormulaSelectionRequested(this, e);
        }

        private void RawSearchEntityArea_SearchDataDefined(object sender, SearchPropertyArg e, AdvanceSearchNode parentNode, LogicPhraseDTO parentLogicPhrase
            , object view)
        {
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(view);
            foreach (var phrase in e.SearchItems)
            {
                (parentNode.Phrase as LogicPhraseDTO).Phrases.Add(phrase);
                AddNode(parentNode, phrase);
            }
        }
        public DP_SearchRepositoryMain GetSearchRepository()
        {
            // AdvancedSearchEntityArea.GetSearchRepository: 51644c6aa16f

            if (RootSearchRepository != null)
            {
                return RootSearchRepository;
            }
            return null;
        }
        public void ClearSearchData()
        {
            ShowSearchRepository(new DP_SearchRepositoryMain() { Title = "عبارت جستجو", TargetEntityID = AreaInitializer.EntityID });
        }
        public void OnSearchDataDefined(DP_SearchRepositoryMain searchData)
        {
            if (SearchDataDefined != null)
            {
                SearchDataDefined(this, searchData );
            }
        }

        public void ConfirmSearch()
        {
            var searchRepository = GetSearchRepository();
            OnSearchDataDefined(searchRepository);
        }
    }
}
