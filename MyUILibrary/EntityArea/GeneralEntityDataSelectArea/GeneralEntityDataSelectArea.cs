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
    public class GeneralEntityDataSelectArea : I_GeneralEntityDataSelectArea
    {
        MySearchLookup entitySearchLookup;
        public EntityDataSelectAreaInitializer EntityDataSelectAreaInitializer { set; get; }


        public I_View_GeneralEntityDataSelectArea View
        {
            set; get;
        }
        I_EditEntityAreaOneData SelectDataArea
        {
            set; get;
        }
        public I_SearchEntityArea SearchArea
        {
            set; get;
        }

        public void SetAreaInitializer(EntityDataSelectAreaInitializer entityDataSelectAreaInitializer)
        {
            //تو این ویوی کاربردی مثلا آرشیو به این اصلی اضافه میشه ولی تو جنرال سرچ ویو اینجا به کاربردی مثلا دیتا ویو اضافه میشه. کدوم بهتره؟ اصلاح بشه
            EntityDataSelectAreaInitializer = entityDataSelectAreaInitializer;
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfGeneralEntityDataSelectArea();
            //if(EntitySelectAreaInitializer.ExternalView!=null)
            //View.AddExternalArea(EntitySelectAreaInitializer.ExternalView);
            View.RemoveEntitySelector();
            View.RemoveDataSelector();
            View.RemoveSearchRepositoy();


            if (EntityDataSelectAreaInitializer.EntityDataPurpose == Enum_EntityDataPurpose.SelectEntity
                || EntityDataSelectAreaInitializer.EntityDataPurpose == Enum_EntityDataPurpose.SelectData
                || EntityDataSelectAreaInitializer.EntityDataPurpose == Enum_EntityDataPurpose.SearchRepository)
            {
                if (!EntityDataSelectAreaInitializer.HideEntitySelector)
                {
                    entitySearchLookup = new MySearchLookup();
                    entitySearchLookup.DisplayMember = "Alias";
                    entitySearchLookup.SelectedValueMember = "ID";
                    entitySearchLookup.SearchFilterChanged += EntitySearchLookup_SearchFilterChanged;
                    entitySearchLookup.SelectionChanged += EntitySearchLookup_SelectionChanged;

                    View.AddEntitySelectorArea(entitySearchLookup);

                    if (EntityDataSelectAreaInitializer.EntityID != 0)
                    {
                        entitySearchLookup.SelectedValue = EntityDataSelectAreaInitializer.EntityID;
                    }
                    if (EntityDataSelectAreaInitializer.LockEntitySelector)
                        entitySearchLookup.IsEnabledLookup = false;
                }
                else
                {
                    if (EntityDataSelectAreaInitializer.EntityID != 0)
                    {
                        var entity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetSimpleEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EntityDataSelectAreaInitializer.EntityID);
                        EntityIsSelected(entity);
                    }
                }
            }
        }

        private void EntitySearchLookup_SearchFilterChanged(object sender, SearchFilterArg e)
        {
            if (!string.IsNullOrEmpty(e.SingleFilterValue))
            {
                if (e.FilterBySelectedValue)
                {
                    var entity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetSimpleEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), Convert.ToInt32(e.SingleFilterValue));
                    if (entity != null)
                        e.ResultItemsSource = new List<TableDrivedEntityDTO> { entity };
                }
                else
                {
                    var entities = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.SearchEntities(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), e.SingleFilterValue);
                    e.ResultItemsSource = entities;
                }
            }
        }
        public TableDrivedEntityDTO SelectedEntity
        {
            get
            {
                return entitySearchLookup.SelectedItem as TableDrivedEntityDTO;
            }
        }
        private void EntitySearchLookup_SelectionChanged(object sender, SelectionChangedArg e)
        {
            //** EntitySelectArea.EntitySearchLookup_SelectionChanged: c3861e5f6f81
            if (e.SelectedItem != null)
            {
                var entity = e.SelectedItem as TableDrivedEntityDTO;
                EntityIsSelected(entity);
            }
            else
            {
                if (EntityDataSelectAreaInitializer.EntityDataPurpose == Enum_EntityDataPurpose.SelectData)
                {
                    View.SetDataSelectorTitle("داده");
                    View.RemoveDataSelector();
                    if (DataItemChanged != null)
                        DataItemChanged(this, null);
                }
                else if (EntityDataSelectAreaInitializer.EntityDataPurpose == Enum_EntityDataPurpose.SearchRepository)
                {
                    View.SetSearchRepositoyTitle("جستجو");
                    View.RemoveSearchRepositoy();
                    if (SearchRepositoryChanged != null)
                        SearchRepositoryChanged(this, null);
                }
                if (EntityChanged != null)
                    EntityChanged(this, null);
            }
        }

        bool firstTime = true;
        private void EntityIsSelected(TableDrivedEntityDTO entity)
        {
            if (EntityChanged != null)
                EntityChanged(this, entity.ID);


            if (EntityDataSelectAreaInitializer.EntityDataPurpose == Enum_EntityDataPurpose.SelectData)
            {
                View.RemoveDataSelector();
                View.SetDataSelectorTitle(entity.Alias);
                EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
                editEntityAreaInitializer1.EntityID = entity.ID;
                editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                editEntityAreaInitializer1.DataMode = CommonDefinitions.UISettings.DataMode.One;
                editEntityAreaInitializer1.EntityListViewID = EntityDataSelectAreaInitializer.EntityListViewID;
                var FirstSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer1);
                if (FirstSideEditEntityAreaResult.Item1 != null && FirstSideEditEntityAreaResult.Item1 is I_EditEntityAreaOneData)
                {
                    SelectDataArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
                    SelectDataArea.DataItemSelected += FirstSideEditEntityArea_DataItemSelected;
                    SelectDataArea.DataItemsCleared += SelectDataArea_DataItemsCleared;
                    View.AddDataSelector(SelectDataArea.TemporaryDisplayView);
                    if (EntityDataSelectAreaInitializer.LockDataSelector)
                        SelectDataArea.FirstView.EnableDisable(false);
                }
                if (EntityDataSelectAreaInitializer.DataItem != null && firstTime)
                {
                    SelectDataArea.SelectData(new List<DP_BaseData>() { EntityDataSelectAreaInitializer.DataItem });
                }
                firstTime = false;
            }
            else if (EntityDataSelectAreaInitializer.EntityDataPurpose == Enum_EntityDataPurpose.SearchRepository)
            {
                //if (!EntityDataSelectAreaInitializer.HideSearchRepository)
                //{
                View.RemoveSearchRepositoy();
                View.SetSearchRepositoyTitle(entity.Alias);
                var searchViewInitializer = new SearchAreaInitializer();
                searchViewInitializer.EntityID = entity.ID;
                searchViewInitializer.EntitySearchID = EntityDataSelectAreaInitializer.EntitySearchID;
                if (!EntityDataSelectAreaInitializer.UserCanChangeSearchRepository)
                {
                    SearchArea.SearchView.DisableEnable(false);
                }
                if (firstTime)
                {
                    searchViewInitializer.AdvancedSearchDTOMessage = EntityDataSelectAreaInitializer.AdvancedSearchDTOMessage;
                    searchViewInitializer.PreDefinedSearchMessage = EntityDataSelectAreaInitializer.PreDefinedSearchMessage;
                }


                //if (AreaInitializer.PreDefinedSearch != null)
                //{
                // ////   searchViewInitializer.EntitySearchID = AreaInitializer.PreDefinedSearch.EntitySearchID;
                //////اینجا چطور؟؟؟
                //}
                SearchArea = new SearchEntityArea(searchViewInitializer);
                View.AddSearchRepository(SearchArea.SearchView);
                SearchArea.SearchDataDefined += SearchArea_SearchDataDefined;
                if (firstTime)
                {
                    firstTime = false;
                    if (EntityDataSelectAreaInitializer.SearchInitially)
                    {
                        if (searchViewInitializer.AdvancedSearchDTOMessage != null)
                            SearchArea.AdvancedSearchEntityArea.ConfirmSearch();
                        else if (searchViewInitializer.PreDefinedSearchMessage != null)
                            SearchArea.SimpleSearchEntityArea.ConfirmSearch();
                    }
                }

                //}
                //else
                //{
                //    if (firstTime && EntityDataSelectAreaInitializer.SearchInitially && EntityDataSelectAreaInitializer.AdvancedSearchDTOMessage != null)
                //    {
                //        firstTime = false;
                //        SearchArea_SearchDataDefined(this, EntityDataSelectAreaInitializer.AdvancedSearchDTOMessage);
                //    }
                //}
            }
        }

        public DP_FormDataRepository SelectedData
        {
            get
            {
                if (SelectDataArea != null && SelectDataArea.GetDataList().Any())
                    return SelectDataArea.GetDataList().First();
                else
                    return null;
            }
        }
        private void SelectDataArea_DataItemsCleared(object sender, EventArgs e)
        {
            if (DataItemChanged != null)
                DataItemChanged(this, null);
        }
        public event EventHandler<DP_SearchRepositoryMain> SearchRepositoryChanged;
        public event EventHandler<List<DP_FormDataRepository>> DataItemChanged;
        public event EventHandler<int?> EntityChanged;

        private void FirstSideEditEntityArea_DataItemSelected(object sender, List<DP_FormDataRepository> e)
        {
            if (DataItemChanged != null)
                DataItemChanged(this, e);
        }
        private void SearchArea_SearchDataDefined(object sender, DP_SearchRepositoryMain e)
        {
            //  AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(SearchArea.SearchView);
            if (SearchRepositoryChanged != null)
                SearchRepositoryChanged(this, e);
        }

        //public void EnableDisableSelectArea(bool enable)
        //{
        //    SelectDataArea.TemporaryDisplayView.DisableEnable(enable);
        //}

        public void SelectData(DP_DataView dataInstance)
        {
            //DP_SearchRepositoryMain searchItems = new DP_SearchRepositoryMain(dataInstance.TargetEntityID);
            //foreach (var item in dataInstance.KeyProperties)
            //{
            //    searchItems.Phrases.Add(new SearchProperty() { ColumnID = item.ColumnID, Value = item.Value });
            //}

            //var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
            //DR_SearchViewRequest request = new DR_SearchViewRequest(requester, searchItems);
            //var childViewData = AgentUICoreMediator.GetAgentUICoreMediator.SendSearchViewRequest(request);
            //if (childViewData.Result == Enum_DR_ResultType.SeccessfullyDone)
            //{
            //    if (childViewData.ResultDataItems.Count == 1)
            //    {

            SelectDataArea.ClearData();
            SelectDataArea.ShowDataFromExternalSource(dataInstance);
            //    }
            //}

        }



    }
}
