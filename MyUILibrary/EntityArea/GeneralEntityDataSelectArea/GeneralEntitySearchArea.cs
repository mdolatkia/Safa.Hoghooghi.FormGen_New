using MyUILibraryInterfaces.EntityArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using MyCommonWPFControls;
using ModelEntites;
using MyUILibrary.EntityArea.Commands;

namespace MyUILibrary.EntitySelectArea
{
    public class GeneralEntityDataSearchArea : I_GeneralEntityDataSearchArea
    {
        MySearchLookup entitySearchLookup;
        public EntityDataSearchAreaInitializer EntityDataSearchAreaInitializer { set; get; }

        public GeneralEntityDataSearchArea()
        {
            // GeneralEntityDataSearchArea: 33b533adf734
        }
        public I_View_GeneralEntityDataSelectArea View
        {
            set; get;
        }

        public I_SearchEntityArea SearchArea
        {
            set; get;
        }
        public TableDrivedEntityDTO Entity { set; get; }

        public void SetAreaInitializer(EntityDataSearchAreaInitializer entityDataSearchAreaInitializer)
        {
            // GeneralEntitySearchArea.SetAreaInitializer: 68b475eb2fc9
            //تو این ویوی کاربردی مثلا آرشیو به این اصلی اضافه میشه ولی تو جنرال سرچ ویو اینجا به کاربردی مثلا دیتا ویو اضافه میشه. کدوم بهتره؟ اصلاح بشه
            EntityDataSearchAreaInitializer = entityDataSearchAreaInitializer;
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfGeneralEntityDataSelectArea();

            if (EntityDataSearchAreaInitializer.EntityID != 0)
            {
                Entity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetSimpleEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EntityDataSearchAreaInitializer.EntityID);
                View.SetSelectorTitle(Entity.Alias);
                var searchViewInitializer = new SearchAreaInitializer();
                searchViewInitializer.EntityID = Entity.ID;
                searchViewInitializer.EntitySearchID = EntityDataSearchAreaInitializer.EntitySearchID;
                if (!EntityDataSearchAreaInitializer.UserCanChangeSearchRepository)
                {
                    SearchArea.SearchView.DisableEnable(false);
                }
                if (firstTime)
                {
                    searchViewInitializer.AdvancedSearchDTOMessage = EntityDataSearchAreaInitializer.AdvancedSearchDTOMessage;
                    searchViewInitializer.PreDefinedSearchMessage = EntityDataSearchAreaInitializer.PreDefinedSearchMessage;
                }
                SearchArea = new SearchEntityArea(searchViewInitializer);
                View.AddSelector(SearchArea.SearchView);
                SearchArea.SearchDataDefined += SearchArea_SearchDataDefined;
                if (firstTime)
                {
                    firstTime = false;
                    if (EntityDataSearchAreaInitializer.SearchInitially)
                    {
                        if (searchViewInitializer.AdvancedSearchDTOMessage != null)
                            SearchArea.AdvancedSearchEntityArea.ConfirmSearch();
                        else if (searchViewInitializer.PreDefinedSearchMessage != null)
                            SearchArea.SimpleSearchEntityArea.ConfirmSearch();
                    }
                }
            }
        }


        bool firstTime = true;



        public event EventHandler<DP_SearchRepositoryMain> SearchRepositoryChanged;

        private void SearchArea_SearchDataDefined(object sender, DP_SearchRepositoryMain e)
        {
            if (SearchRepositoryChanged != null)
                SearchRepositoryChanged(this, e);
        }

    }
}
