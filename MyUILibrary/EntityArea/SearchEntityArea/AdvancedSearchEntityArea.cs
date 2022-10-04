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
        public AdvancedSearchEntityArea(SearchAreaInitializer newAreaInitializer)
        {
            SearchCommands = new List<I_Command>();
            SearchInitializer = newAreaInitializer;

            AdvancedSearchView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfAdvancedSearch();
            ManageAdvancedSearchView();

            var clearSearchcommand = new SearchClearCommand(this);
            AdvancedSearchView.AddCommand(clearSearchcommand.CommandManager);
            var searchConfirmcommand = new AdvancedSearchConfirmCommand(this);
            AdvancedSearchView.AddCommand(searchConfirmcommand.CommandManager);
        }
        public I_View_AdvancedSearchEntityArea AdvancedSearchView { set; get; }

        public List<I_Command> SearchCommands
        {
            set;
            get;
        }


        public SearchAreaInitializer SearchInitializer { set; get; }

        //public event EventHandler<Arg_PackageSelected> DataPackageSelected;
        public event EventHandler<SearchDataArg> SearchDataDefined;

        TableDrivedEntityDTO _FullEntity;
        public TableDrivedEntityDTO FullEntity
        {
            get
            {
                if (_FullEntity == null)
                    _FullEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetPermissionedEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), SearchInitializer.EntityID);
                return _FullEntity;
            }
        }

        public DP_SearchRepository RootSearchRepository { get; private set; }

        //     AdvanceSearchNode RootNode;

        private void ManageAdvancedSearchView()
        {
            RootSearchRepository = new DP_SearchRepository(SearchInitializer.EntityID);
            RootSearchRepository.Title = FullEntity.Alias;
            AddNode(null, RootSearchRepository);
        }

        public bool ShowSearchRepository(DP_SearchRepository item)
        {
            //   RootNode = null;
            RootSearchRepository = item;
            AdvancedSearchView.ClearTreeItems();

            AddNode(null, item);
            return true;
        }

        private void AddNode(AdvanceSearchNode parentNode, Phrase phrase)
        {
            if (phrase is LogicPhraseDTO)
            {
                var logicPhrase = phrase as LogicPhraseDTO;

                var newnode = new AdvanceSearchNode();
                newnode.Phrase = logicPhrase;
                newnode.ParentNode = parentNode;
                if (parentNode != null)
                    parentNode.ChildItems.Add(newnode);
                if (logicPhrase is DP_SearchRepository)
                {
                    newnode.Title = (logicPhrase as DP_SearchRepository).Title;
                }
                else
                {
                    newnode.Title = "عبارت منطقی";
                }
                //if (parentNode != null)
                //{
                //    if (logicPhrase is DP_SearchRepository)
                //        newnode.EntityID = (logicPhrase as DP_SearchRepository).TargetEntityID;
                //    else
                //        newnode.EntityID = parentNode.EntityID;
                //}
                //else
                //    newnode.EntityID = SearchInitializer.EntityID;

                var searchRepositories = GetParentSearchRepositories(newnode);
                if (parentNode != null)
                    newnode.NodeManager = parentNode.NodeManager.AddChildItem();
                else
                    newnode.NodeManager = AdvancedSearchView.AddTreeItem();

                newnode.NodeManager.SetHeader(newnode.Title);
                newnode.NodeManager.AddLogicComboBox(GetAndOrList());
                newnode.NodeManager.LogicComboBoxChanged += (sender, e) => LogicComboBoxChanged(sender, e, newnode, logicPhrase);

                foreach (var item in logicPhrase.Phrases)
                {
                    AddNode(newnode, item);
                }

                I_AdvanceSearchMenu rootAndMenu = newnode.NodeManager.AddMenu("Add Logic Phrase");
                rootAndMenu.Clicked += (sender, e) => AddLogicPhraseClicked(sender, e, newnode);

                var last = searchRepositories.Last();

                I_AdvanceSearchMenu simpleSearchMenu = newnode.NodeManager.AddMenu("افزودن لیست خصوصیات");
                simpleSearchMenu.Clicked += (sender1, e1) => RootAndMenu_Clicked1(sender1, e1, newnode, last);


                var entity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetPermissionedEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), last.TargetEntityID);
                foreach (var relationship in entity.Relationships)
                {
                    I_AdvanceSearchMenu relationshipSearchMenu = newnode.NodeManager.AddMenu(relationship.Alias);
                    relationshipSearchMenu.Clicked += (sender1, e1) => Relationship_ClickedNew(sender1, e1, newnode, relationship);
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
                newnode.Title = propertyPhrase.ColumnID.ToString();
                parentNode.ChildItems.Add(newnode);
                newnode.NodeManager = parentNode.NodeManager.AddChildItem();
                newnode.NodeManager.SetHeader(newnode.Title);
                newnode.Phrase = phrase;

                I_AdvanceSearchMenu removeMenu = newnode.NodeManager.AddMenu("حذف");
                removeMenu.Clicked += (sender1, e1) => RemoveMenu_Clicked(sender1, e1, parentNode, newnode);
                //   return newnode;
            }
            //return null;
        }
        private List<DP_SearchRepository> GetParentSearchRepositories(AdvanceSearchNode newnode)
        {
            List<DP_SearchRepository> result = new List<DP_SearchRepository>();
            GetParentSearchRepositories(newnode, result);
            result.Reverse();
            return result;
        }
        private void GetParentSearchRepositories(AdvanceSearchNode newnode, List<DP_SearchRepository> result)
        {
            if (result == null)
                result = new List<DP_SearchRepository>();
            if (newnode.Phrase is DP_SearchRepository)
            {
                result.Add(newnode.Phrase as DP_SearchRepository);

            }
            if (newnode.ParentNode != null)
            {
                GetParentSearchRepositories(newnode.ParentNode, result);
            }
            else
            {
                result.Add(RootSearchRepository);
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

        private void AddLogicPhraseClicked(object sender, EventArgs e, AdvanceSearchNode newnode)
        {
            AddNode(newnode, new LogicPhraseDTO());
        }

        private void Relationship_ClickedNew(object sender1, EventArgs e1, AdvanceSearchNode node
            , RelationshipDTO relationship)
        {
            var relSearchRepository = new DP_SearchRepository(relationship.EntityID2);
            relSearchRepository.Title = relationship.Alias;
            relSearchRepository.SourceRelationship = relationship;

            AddNode(node, relSearchRepository);
            //var searchViewInitializer = new SearchAreaInitializer();
            //searchViewInitializer.EntityID = relationship.EntityID2;
            //searchViewInitializer.Title = relationship.Alias;
            //// searchViewInitializer.SourceRelationship = relationship;
            //SearchEntityArea advancedAndRaw = new EntityArea.SearchEntityArea(searchViewInitializer);
            //advancedAndRaw.SearchDataDefined += (sender, e) => AdvancedAndRaw_SearchDataDefinedNew(sender, e, relationship, node, advancedAndRaw.SearchView);

            //AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(advancedAndRaw.SearchView, searchViewInitializer.Title, Enum_WindowSize.Big);
        }
        //private void AdvancedAndRaw_SearchDataDefinedNew(object sender, SearchDataArg e, RelationshipDTO relationship, AdvanceSearchNode node, object view)
        //{
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(view);

        //    if (node.Phrase is LogicPhraseDTO)
        //        (node.Phrase as LogicPhraseDTO).Phrases.Add(e.SearchItems);

        //    AddNode(node, e.SearchItems);
        //}
        //private void Relationship_ClickedEdit(object sender1, EventArgs e1, AdvanceSearchNode node, DP_SearchRepository dP_SearchRepository)
        //{

        //    var searchViewInitializer = new SearchAreaInitializer();
        //    searchViewInitializer.PreDefinedSearch = dP_SearchRepository;
        //    searchViewInitializer.EntityID = dP_SearchRepository.TargetEntityID;
        //    searchViewInitializer.Title = dP_SearchRepository.Title;
        //    //   searchViewInitializer.SourceRelationship = dP_SearchRepository.SourceRelationship;
        //    SearchEntityArea advancedAndRaw = new EntityArea.SearchEntityArea(searchViewInitializer);
        //    advancedAndRaw.SearchDataDefined += (sender, e) => AdvancedAndRaw_SearchDataDefinedEdit(sender, e, node, advancedAndRaw.SearchView);
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(advancedAndRaw.SearchView, searchViewInitializer.Title, Enum_WindowSize.Big);
        //}
        //private void AdvancedAndRaw_SearchDataDefinedEdit(object sender, SearchDataArg e, AdvanceSearchNode node, object view)
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
        //    if (newnode.Phrase is DP_SearchRepository)
        //    {
        //        if ((newnode.Phrase as DP_SearchRepository) == firstRepository)
        //            return true;
        //        else
        //            return false;
        //    }

        //    if (newnode.ParentNode == null)
        //        return false;
        //    else if (newnode.ParentNode.Phrase is DP_SearchRepository)
        //    {
        //        if ((newnode.ParentNode.Phrase as DP_SearchRepository) == firstRepository)
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

        //Tuple<int,>
        //  object lastSearchView;
        private void RootAndMenu_Clicked1(object sender1, EventArgs e1, AdvanceSearchNode andOrNode, DP_SearchRepository last)
        {
            var searchViewInitializer = new SearchAreaInitializer();
            searchViewInitializer.EntityID = last.TargetEntityID;
            //if (SearchInitializer.TempEntity != null && SearchInitializer.TempEntity.ID == SearchInitializer.EntityID)
            //    searchViewInitializer.TempEntity = SearchInitializer.TempEntity;
            I_RawSearchEntityArea rawSearchEntityArea = new RawSearchEntityArea(searchViewInitializer);
            rawSearchEntityArea.SearchDataDefined += (sender, e) => RawSearchEntityArea_SearchDataDefined(sender, e, andOrNode, rawSearchEntityArea.RawSearchView);
            //lastSearchView = rawSearchEntityArea.RawSearchView;
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(rawSearchEntityArea.RawSearchView, "خصوصیات", Enum_WindowSize.Big);

        }
        private void RawSearchEntityArea_SearchDataDefined(object sender, SearchPropertyArg e, AdvanceSearchNode andOrNode, object view)
        {
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(view);
            foreach (var phrase in e.SearchItems)
            {
                (andOrNode.Phrase as LogicPhraseDTO).Phrases.Add(phrase);
                AddNode(andOrNode, phrase);
            }
        }
        public DP_SearchRepository GetSearchRepository()
        {
            if (RootSearchRepository != null)
            {
                RootSearchRepository.IsSimpleSearch = false;
                return RootSearchRepository;
            }
            return null;
        }



        public void ClearSearchData()
        {


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
