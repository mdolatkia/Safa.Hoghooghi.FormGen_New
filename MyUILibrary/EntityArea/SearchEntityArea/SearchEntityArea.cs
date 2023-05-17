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
    public class SearchEntityArea : I_SearchEntityArea
    {
        public SearchEntityArea(SearchAreaInitializer newAreaInitializer)
        {
            AreaInitializer = newAreaInitializer;
            //if (SearchInitializer.PreDefinedSearch != null)
            //{
            //    ShowSearchRepository(SearchInitializer.PreDefinedSearch);
            //}

        }
        I_View_SearchEntityArea _SearchView;
        public I_View_SearchEntityArea SearchView
        {
            //** SearchEntityArea.SearchView: 3100cfe8e22f
            get
            {
                if (_SearchView == null)
                {
                    _SearchView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfSearchEntityArea();
                    SimpleSearchEntityArea = new EntityDefinedSearchArea(AreaInitializer);
                    SimpleSearchEntityArea.SearchDataDefined += SimpleSearchEntityArea_SearchDataDefined;
                    SearchView.AddSimpleSearchView(SimpleSearchEntityArea.SimpleSearchView);

                    AdvancedSearchEntityAre = new AdvancedSearchEntityArea(AreaInitializer);
                    AdvancedSearchEntityAre.SearchDataDefined += SimpleSearchEntityArea_SearchDataDefined;
                    SearchView.AddAdvancedSearchView(AdvancedSearchEntityAre.AdvancedSearchView);
                }
                return _SearchView;
            }
        }
        //I_View_SearchEntityArea _SearchView;
        //public I_View_SearchEntityArea SearchView
        //{
        //    get
        //    {
        //        //** 0fa106fa-203c-4249-b43b-b5efe4a26994
        //        if (_SearchView==null)
        //        {

        //        }
        //        return _SearchView;
        //    }
        //}

        public I_EntityDefinedSearchArea SimpleSearchEntityArea
        {
            set; get;
        }

        public I_AdvancedSearchEntityArea AdvancedSearchEntityAre
        {
            set; get;
        }

        public SearchAreaInitializer AreaInitializer { set; get; }

        public event EventHandler<DP_SearchRepositoryMain> SearchDataDefined;




        //public void ShowSearchRepository(DP_SearchRepositoryMain searchRepository)
        //{
        //    if (searchRepository == null)
        //        return;
        //    bool showInSimple = false;
        //    if (searchRepository.IsSimpleSearch == true)
        //    {
        //        showInSimple = SimpleSearchEntityArea.ShowSearchRepository(searchRepository);
        //        if (showInSimple)
        //            SearchView.IsSimpleSearchActiveOrAdvancedSearch = true;
        //    }
        //    if (!showInSimple)
        //    {
        //        if (!AdvancedSearchEntityAre.ShowSearchRepository(searchRepository))
        //        {
        //            throw (new Exception("AsdasdasD"));
        //        }
        //        else
        //            SearchView.IsSimpleSearchActiveOrAdvancedSearch = false;
        //    }
        //}
        //private DP_SearchRepositoryMain ConvertPreDefinedSearch(EntityPreDefinedSearchDTO preDefinedSearch)
        //{
        //    //DP_SearchRepositoryMain result = new DP_SearchRepositoryMain();
        //    //result.TargetEntityID
        //    return null; 
        //}

        public bool IsSimpleSearchActiveOrAdvancedSearch
        {
            get { return SearchView.IsSimpleSearchActiveOrAdvancedSearch; }
        }
        //public void ShowPreDefinedSearch(EntityPreDefinedSearchDTO message)
        //{
        //    ShowSearchRepository(message.SearchRepository);
        //}

        private void SimpleSearchEntityArea_SearchDataDefined(object sender, DP_SearchRepositoryMain e)
        {
            //** 294e2816-e090-40db-96a7-661edde3000b

            //DP_SearchRepositoryMain searchData = new DP_SearchRepositoryMain(SearchInitializer.EntityID);
            //if (e.SearchItems != null)
            //{
            //    searchData.AndOrType = e.SearchItems.AndOrType;
            //    searchData.Phrases = e.SearchItems.Phrases;
            //}
            //OnSearchDataDefined(searchData);

            OnSearchDataDefined(e);
        }
        public DP_SearchRepositoryMain LastSearch { set; get; }
        public void OnSearchDataDefined(DP_SearchRepositoryMain searchData)
        {
            //کاربردهای این متد گفته شود در داکیومنت


            //if (searchData == null)
            //    searchData = new DP_SearchRepositoryMain(SearchInitializer.EntityID);

            //var arg = new DP_SearchRepositoryMain();
            //arg.SearchItems = new DP_SearchRepositoryMain(SearchInitializer.EntityID);
            //arg.SearchItems.AndOrType = searchData.AndOrType;
            //arg.SearchItems.Phrases = searchData.Phrases;
            //LastSearch = arg.SearchItems;
            //if (SearchDataDefined != null)
            //    SearchDataDefined(this, searchData);

            LastSearch = searchData;

            if (SearchDataDefined != null)
                SearchDataDefined(this, searchData);
        }


        public void ClearSearchData()
        {
            throw new NotImplementedException();
        }

        //public DP_SearchRepositoryMain GetSearchRepository()
        //{
        //    //باید آخرین سرچ انجام شده نگهداری و اینجا برگردانده شود
        //    return lastSearch;
        //}
    }
}
