using AutoMapper;
using CommonDefinitions.UISettings;
using ModelEntites;

using MyUILibrary.EntityArea.Commands;
using MyUILibrary.Temp;
using MyUILibraryInterfaces.DataReportArea;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{
    public abstract class BaseEditEntityArea : I_EditEntityArea
    {
        //  public UIControlComposition UIControlComposition { set; get; }
        public event EventHandler DataItemsCleared;
        public event EventHandler<List<DP_FormDataRepository>> DataItemSelected;

        //   public event EventHandler<EditAreaGeneratedArg> RelationshipAreaGenerated;
        //public event EventHandler<EditAreaDataItemLoadedArg> DataItemLoaded;
        //   public event EventHandler<EditAreaDataItemLoadedArg> DataItemShown;
        public RelationshipColumnControlGeneral SourceRelationColumnControl { set; get; }
        public event EventHandler<DisableEnableChangedArg> DisableEnableChanged;
        public event EventHandler DataViewGenerated;
        public event EventHandler UIGenerated;
        public IAgentUIManager UIManager { get { return AgentUICoreMediator.GetAgentUICoreMediator.UIManager; } }

        //     public abstract void DataItemVisiblity(object data, bool visiblity);
        //   public abstract void DataItemEnablity(object data, bool visiblity);
        //public abstract bool AddData(DP_FormDataRepository data, bool showDataInDataView);



        //public bool AddData(DP_DataRepository data, bool showDataInDataView)
        //{
        //    //Mapper.Initialize(cfg => cfg.CreateMap<DP_DataRepository, DP_FormDataRepository>());
        //    //var result = AutoMapper.Mapper.Map<DP_DataRepository, DP_FormDataRepository>(data);

        //    DP_FormDataRepository formData = new DP_FormDataRepository(data, this);
        //    return AddData(formData, showDataInDataView);
        //}
        public abstract void GenerateUIControlsByCompositionDTO(EntityUICompositionDTO UICompositions);

        //    public abstract bool ShowDataInDataView(DP_FormDataRepository dataItem);
        public BaseEditEntityArea(TableDrivedEntityDTO simpleEntity)
        {
            RunningActionActivities = new List<UIActionActivityDTO>();
            SimpleEntity = simpleEntity;

            Commands = new List<I_Command>();
            //MessageItems = new ObservableCollection<BaseMessageItem>();
            //  ValidationItems.CollectionChanged += ValidationItems_CollectionChanged;
            //  BusinesMessageItems = new ObservableCollection<BaseMessageItem>();
            //   BusinesMessageItems.CollectionChanged += ControlMessageItems_CollectionChanged;
            FormulaExceptionReulsts = new ObservableCollection<FunctionResult>();
            FormulaExceptionReulsts.CollectionChanged += FormulaExceptionReulsts_CollectionChanged;
        }

        public void ShowDataFromExternalSource(DP_BaseData dataRepository = null)
        {
            var date = new List<DP_BaseData>();
            if (dataRepository != null)
                date.Add(dataRepository);
            ShowDataFromExternalSource(date);
        }
        public void ShowDataFromExternalSource(List<DP_BaseData> dataRepositories)
        {
            // BaseEditEntityArea.ShowDataFromExternalSource: 0594c59749b4
            bool dataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                 AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);




            if (dataRepositories != null && dataRepositories.Count > 0)
            {
                foreach (var dataRepository in dataRepositories)
                {
                    if (!dataRepository.KeyProperties.Any())
                        throw new Exception("asdad");
                }

                List<Tuple<DP_DataView, DP_FormDataRepository, string>> items = new List<Tuple<DP_DataView, DP_FormDataRepository, string>>();

                List<DP_FormDataRepository> searchedData = null;
                if (dataView)
                    searchedData = SearchDataForEditFromExternalSource(AreaInitializer.EntityID, dataRepositories, this);
                else
                    searchedData = SearchDataForViewFromExternalSource(AreaInitializer.EntityID, dataRepositories, this);
                foreach (var data in searchedData)
                {
                    AddData(data);
                }

                //if (data != null)
                //{
                //    AddData(data);
                //    //var addResult = AddData(data);
                //    //if (!addResult)
                //    //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", "", Temp.InfoColor.Red);
                //}
                //else
                //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", "", Temp.InfoColor.Red);


            }
            else
            {
                if (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                       AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                {
                    if (this is I_EditEntityAreaOneData)
                        (this as I_EditEntityAreaOneData).CreateDefaultData();
                }
            }
        }

        private List<DP_FormDataRepository> SearchDataForEditFromExternalSource(int entityID, List<DP_BaseData> listData, I_EditEntityArea editEntityArea)
        {

            // BaseEditEntityArea.SearchDataForEditFromExternalSource: 12958ab8fff9

            List<DP_FormDataRepository> result = new List<DP_FormDataRepository>();
            DP_SearchRepositoryMain searchItems = new DP_SearchRepositoryMain(entityID);
            searchItems.AndOrType = AndOREqualType.Or;
            foreach (var item in listData)
            {
                LogicPhraseDTO logic = new LogicPhraseDTO();
                foreach (var col in item.Properties.Where(x => x.IsKey))
                    logic.Phrases.Add(new SearchProperty(col.ColumnID) { Value = col.Value });
                searchItems.Phrases.Add(logic);
            }
            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();

            //   var requestSearchEdit = new DR_SearchEditRequest(requester, searchDataItem, editEntityArea.AreaInitializer.SecurityReadOnly, true);
            var requestSearchEdit = new DR_SearchEditRequest(requester, searchItems);
            var foundItem = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchEditRequest(requestSearchEdit).ResultDataItems;

            foreach (var item in foundItem)
            {
                result.Add(new DP_FormDataRepository(foundItem[0], editEntityArea, false, false));
            }
            return result;
        }
        private List<DP_FormDataRepository> SearchDataForViewFromExternalSource(int entityID, List<DP_BaseData> listData, I_EditEntityArea editEntityArea)
        {
            // BaseEditEntityArea.SearchDataForViewFromExternalSource: 7a04c6c350ac

            List<DP_FormDataRepository> result = new List<DP_FormDataRepository>();

            //DP_SearchRepositoryMain SearchDataItem = new DP_SearchRepositoryMain(entityID);
            //foreach (var col in searchViewData.KeyProperties)
            //{
            //    SearchDataItem.Phrases.Add(new SearchProperty(col.Column) { Value = col.Value });
            //}
            DP_SearchRepositoryMain searchItems = new DP_SearchRepositoryMain(entityID);
            searchItems.AndOrType = AndOREqualType.Or;
            foreach (var item in listData)
            {
                LogicPhraseDTO logic = new LogicPhraseDTO();
                foreach (var col in item.Properties.Where(x => x.IsKey))
                    logic.Phrases.Add(new SearchProperty(col.ColumnID) { Value = col.Value });
                searchItems.Phrases.Add(logic);
            }
            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();

            var requestSearchView = new DR_SearchViewRequest(requester, searchItems);
            var foundItem = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(requestSearchView).ResultDataItems;

            foreach (var item in foundItem)
            {
                result.Add(new DP_FormDataRepository(foundItem[0], editEntityArea, false, false));
            }
            return result;

        }
        private void FormulaExceptionReulsts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ////if (e.Action == NotifyCollectionChangedAction.Reset)
            ////{
            ////    foreach (var columnControl in SimpleColumnControls)
            ////    {
            ////        columnControl.ControlManager.ClearFormulaMessage,...();
            ////    }
            ////    foreach (var columnControl in RelationshipColumnControls)
            ////    {
            ////        columnControl.ControlManager.ClearValidation();
            ////    }
            ////}
            ////else if (e.Action == NotifyCollectionChangedAction.Add)
            ////{
            ////    foreach (BaseValidationItem item in e.NewItems)
            ////    {
            ////        if (item is SimpleColumnValidationItem)
            ////        {
            ////            var simpleColumncontrol = (item as SimpleColumnValidationItem).ColumnControl;
            ////            (simpleColumncontrol as SimpleColumnControl).ControlManager.SetValidation(item.Message, item.Color);
            ////        }
            ////        else if (item is RelationshipColumnValidationItem)
            ////        {
            ////            var relationshipColumncontrol = (item as RelationshipColumnValidationItem).RelationshipControl;
            ////            (relationshipColumncontrol as RelationshipColumnControl).ControlManager.SetValidation(item.Message, item.Color);
            ////        }
            ////    }
            ////}
        }
        public EditEntityAreaInitializer AreaInitializer
        {
            set;
            get;
        }
        List<EntityValidationDTO> _EntityValidations;
        public List<EntityValidationDTO> EntityValidations
        {
            get
            {
                if (_EntityValidations == null)
                    _EntityValidations = AgentUICoreMediator.GetAgentUICoreMediator.entityValidationManagerService.GetEntityValidations(AreaInitializer.EntityID);
                return _EntityValidations;
            }
        }

        public List<I_Command> Commands { set; get; }
        List<EntityCommandDTO> _EntityCommands;
        public List<EntityCommandDTO> EntityCommands
        {
            get
            {
                if (_EntityCommands == null)
                    _EntityCommands = AgentUICoreMediator.GetAgentUICoreMediator.entityCommandManagerService.GetEntityCommands(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                return _EntityCommands;
            }
        }
        //public ObservableCollection<BaseMessageItem> MessageItems
        //{
        //    set; get;
        //}
        //public ObservableCollection<BaseMessageItem> BusinesMessageItems
        //{
        //    set; get;
        //}
        //ChildRelationshipInfo _ChildRelationshipInfo;
        //public ChildRelationshipInfo ChildRelationshipInfo
        //{
        //    get
        //    {
        //        return _ChildRelationshipInfo;
        //    }
        //    set
        //    {
        //        _ChildRelationshipInfo = value;
        //    }
        //}
        public I_EditEntityLetterArea EditLetterArea
        {
            set; get;
        }
        public I_DataListReportAreaContainer DataListReportAreaContainer
        {
            set; get;
        }
        public I_EntityLettersArea EntityLettersArea
        {
            set; get;
        }
        //EntityListViewDTO _DefaultEntityListViewDTO;
        //public EntityListViewDTO DefaultEntityListViewDTO
        //{
        //    get
        //    {
        //        if (_DefaultEntityListViewDTO == null)
        //            _DefaultEntityListViewDTO = AgentUICoreMediator.GetAgentUICoreMediator.EntityListViewManager.GetOrCreateEntityListViewDTO(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
        //        return _DefaultEntityListViewDTO;
        //    }
        //}




        //TableDrivedEntityDTO _EntityWithSimpleColumns;
        //public TableDrivedEntityDTO EntityWithSimpleColumns
        //{
        //    get
        //    {
        //        if (_EntityWithSimpleColumns == null)
        //        {
        //            if (FullEntity != null)
        //                return FullEntity;
        //            else
        //                _EntityWithSimpleColumns = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetEntityWithSimpleColumns(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
        //        }
        //        return _EntityWithSimpleColumns;
        //    }
        //}


        //  I_SearchViewEntityArea _SearchViewEntityArea;
        // public I_SearchAndViewEntityArea SearchViewEntityArea { set; get; }


        List<RelationshipColumnControlGeneral> _RelationshipColumnControls;
        public List<RelationshipColumnControlGeneral> RelationshipColumnControls
        {
            get
            {
                if (_RelationshipColumnControls == null)
                    _RelationshipColumnControls = new List<RelationshipColumnControlGeneral>();
                return _RelationshipColumnControls;
            }
        }

        List<SimpleColumnControlGenerel> _SimpleColumnControls;
        public List<SimpleColumnControlGenerel> SimpleColumnControls
        {
            get
            {
                if (_SimpleColumnControls == null)
                    _SimpleColumnControls = new List<SimpleColumnControlGenerel>();
                return _SimpleColumnControls;
            }
        }
        TableDrivedEntityDTO _FullEntity;
        //public List<RelationshipDTO> ValidRelationships = new List<RelationshipDTO>();
        //public List<ColumnDTO> ValidColumns = new List<ColumnDTO>();
        public TableDrivedEntityDTO FullEntity
        {
            get
            {
                if (_FullEntity == null)
                {
                    _FullEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetFullEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                    //var item = AgentHelper.GetValidColumnsAndRelationships(_FullEntity, Permission);
                }
                //foreach (var rel in _FullEntity.Relationships.Where(x => x.TypeEnum == Enum_RelationshipType.SubToSuper && (x as SubToSuperRelationshipDTO).ISARelationship.InternalTable).ToList())
                //{
                //    var pItem
                //}
                //روابطی که فعال نباشند اصلا در لیست روابط موجودیت نمی آیند

                //_FullEntity.Relationships.Clear();
                // _FullEntity.Relationships.RemoveAll(x => x.ID != 23 && x.ID != 25 && x.ID != 24 && x.ID != 26);
                //_FullEntity.Relationships.RemoveAll(x => x.ID != 15 && x.ID != 16);
                return _FullEntity;
            }
        }
        EntitySearchDTO _EntitySearch;
        public EntitySearchDTO EntitySearchDTO
        {
            get
            {
                if (_EntitySearch == null)
                {
                    //if (AreaInitializer.EntitySearchID != 0)
                    //    _EntitySearch = AgentUICoreMediator.GetAgentUICoreMediator.DataSearchManager.GetEntitySearch(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntitySearchID);
                    //else
                    _EntitySearch = AgentUICoreMediator.GetAgentUICoreMediator.DataSearchManager.GetOrCreateEntitySearchDTO(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                }
                return _EntitySearch;
            }
        }
        DataEntryEntityDTO _DataEntryEntity;
        public DataEntryEntityDTO DataEntryEntity
        {
            //** BaseEditEntityArea.DataEntryEntity: fc084e9c81fc
            get
            {
                if (AreaInitializer.DataEntryRelationship != null && AreaInitializer.DataEntryRelationship.TargetDataEntryEntity != null)
                {
                    //if (SourceRelationColumnControl.DataEntryRelationship.TargetDataEntryEntity == null)
                    //{
                    //    SourceRelationColumnControl.DataEntryRelationship.TargetDataEntryEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetDataEntryEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID,
                    // SourceRelationColumnControl.DataEntryRelationship);
                    //}
                    //return SourceRelationColumnControl.DataEntryRelationship.TargetDataEntryEntity;

                    return AreaInitializer.DataEntryRelationship.TargetDataEntryEntity;
                }
                else
                {
                    if (_DataEntryEntity == null)
                    {
                        if (!AreaInitializer.Preview)
                            _DataEntryEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetDataEntryEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                    }
                    return _DataEntryEntity;
                }
            }
        }

        TableDrivedEntityDTO _SimpleEntity;
        public TableDrivedEntityDTO SimpleEntity
        {
            get
            {
                return _SimpleEntity;
            }
            set
            {
                _SimpleEntity = value;
            }
        }

        //موجودیت درخت تنظیمات ظاهری فرم را دریافت و نگهداری میکند
        //bool UICompositionsCalled;
        //EntityUICompositionCompositeDTO _UICompositions;
        //public EntityUICompositionCompositeDTO UICompositions
        //{
        //    get
        //    {
        //        if (_UICompositions == null && !UICompositionsCalled)
        //        {
        //            UICompositionsCalled = true;
        //            //if (AreaInitializer.Preview)
        //            //    _UICompositions = AreaInitializer.PreviewUICompositionItems;
        //            //else
        //            _UICompositions = AgentUICoreMediator.GetAgentUICoreMediator.entityUICompositionService.GetEntityUICompositionTree(AreaInitializer.EntityID);
        //        }
        //        return _UICompositions;
        //    }
        //    set
        //    {
        //        _UICompositions = value;
        //    }
        //}
        //public RelationshipUISettingDTO GetRelationshipUISetting(RelationshipDTO relationship, bool isTemporaryView)
        //{
        //    RelationshipUISettingDTO setting;
        //    if (UICompositions != null && UICompositions.RelationshipItems != null
        //    && UICompositions.RelationshipItems.Any(x => x.RelationshipID == relationship.ID))
        //    {
        //        setting = UICompositions.RelationshipItems.First(x => x.RelationshipID == relationship.ID);
        //    }
        //    else
        //    {
        //        setting = new RelationshipUISettingDTO();
        //        if (isTemporaryView)
        //        {
        //            setting.UIColumnsType = Enum_UIColumnsType.Normal;
        //        }
        //        else
        //        {
        //            setting.Expander = true;
        //            setting.UIColumnsType = Enum_UIColumnsType.Full;
        //        }
        //        UICompositions.RelationshipItems.Add(setting);
        //    }
        //    return setting;
        //}

        //private ColumnUISettingDTO GetColumnUISetting(ColumnDTO column)
        //{
        //    ColumnUISettingDTO setting;
        //    if (UICompositions != null && UICompositions.ColumnItems != null
        //        && UICompositions.ColumnItems.Any(x => x.ColumnID == column.ID))
        //    {
        //        setting = UICompositions.ColumnItems.First(x => x.ColumnID == column.ID);
        //    }
        //    else
        //    {
        //        setting = new ColumnUISettingDTO();
        //        setting.UIColumnsType = Enum_UIColumnsType.Normal;
        //        setting.UIRowsCount = 1;
        //        setting.ColumnID = column.ID;
        //        UICompositions.ColumnItems.Add(setting);
        //    }
        //    return setting;
        //}
        AssignedPermissionDTO _Permission;
        public AssignedPermissionDTO Permission
        {
            get
            {
                if (_Permission == null)
                    _Permission = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetEntityAssignedPermissions(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID, true);
                return _Permission;
            }
        }
        public List<DP_FormDataRepository> GetDataList()
        {
            if (SourceRelationColumnControl == null)
            {
                return AreaInitializer.Datas;
            }
            else
            {
                throw (new Exception("sss2222"));

            }
            //else
            //{
            //    if (ChildRelationshipInfo != null)
            //        return ChildRelationshipInfo.RelatedData;
            //    else return new ObservableCollection<DP_FormDataRepository>();
            //}
        }
        //TemporaryLinkState _TemporaryLinkState;
        //public TemporaryLinkState TemporaryLinkState
        //{
        //    get
        //    {
        //        if (_TemporaryLinkState == null)
        //            _TemporaryLinkState = new TemporaryLinkState();
        //        return _TemporaryLinkState;
        //    }
        //}
        //public EntityUISettingDTO GetEntityUISetting()
        //{
        //    if (UICompositions != null && UICompositions.RootItem != null && UICompositions.RootItem.EntityUISetting != null)
        //    {
        //        var entityUISetting = UICompositions.RootItem.EntityUISetting;
        //        return entityUISetting;
        //    }
        //    else
        //    {
        //        var setting = new EntityUISettingDTO();
        //        setting.UIColumnsCount = 4;
        //        return setting;
        //    }
        //}


        public static Tuple<I_EditEntityArea, string> GetEditEntityArea(EditEntityAreaInitializer initializer)
        {
            //** BaseEditEntityArea.GetEditEntityArea: 9ad46f1525db
            BaseEditEntityArea result = null;
            var simpleEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetSimpleEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), initializer.EntityID);
            if (simpleEntity == null)
                return new Tuple<I_EditEntityArea, string>(null, "عدم دسترسی به موجودیت به شناسه" + " " + initializer.EntityID);

            var _Permission = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetEntityAssignedPermissions(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), initializer.EntityID, false);
            if (!_Permission.GrantedActions.Any(x => x == SecurityAction.ReadOnly
            || x == SecurityAction.Edit || x == SecurityAction.EditAndDelete))
                return new Tuple<I_EditEntityArea, string>(null, "عدم دسترسی فرمی به موجودیت به شناسه" + " " + initializer.EntityID);

            if (initializer.DataMode == DataMode.None)
            {
                if (simpleEntity.BatchDataEntry == false)
                    initializer.DataMode = DataMode.One;
                else
                    initializer.DataMode = DataMode.Multiple;

            }
            if (initializer.IntracionMode == IntracionMode.None)
            {
                initializer.IntracionMode = CommonDefinitions.UISettings.IntracionMode.CreateSelectDirect;
            }
            if (initializer.DataMode == DataMode.One)
            {
                result = new EditEntityAreaOneData(simpleEntity);
            }
            else if (initializer.DataMode == DataMode.Multiple)
                result = new EditEntityAreaMultipleData(simpleEntity);
            if (result != null)
            {
                //initializer.EditAreaDataManager = new EditAreaDataManager();
                initializer.ActionActivityManager = new UIActionActivityManager(result);
                //   initializer.RelationshipFilterManager = new RelationshipFilterManager(result);
                initializer.EntityAreaLogManager = new EntityAreaLogManager();
                initializer.UIFomulaManager = new UIFomulaManager(result as BaseEditEntityArea);
                initializer.UIValidationManager = new UIValidationManager(result as BaseEditEntityArea);
                result.SetAreaInitializerAndGenerateView(initializer);
            }
            return new Tuple<I_EditEntityArea, string>(result, "");
        }

        public void SetAreaInitializerAndGenerateView(EditEntityAreaInitializer initParam)
        {
            //** BaseEditEntityArea.SetAreaInitializerAndGenerateView: 079c6e89e544
            AreaInitializer = initParam;

            if (initParam.Preview)
            {
                _DataEntryEntity = initParam.PreviewDataEntryEntity;
            }

            if (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
             AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
            {
                GenerateDataView();
            }
            if (AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
           AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
            AreaInitializer.IntracionMode == IntracionMode.Select)
            {
                GenerateTempView();
            }

            //   GenerateViewEntityArea();

            if (UIGenerated != null)
                UIGenerated(this, null);
            //}
        }

        public void ShowSearchView(bool fromDataView)
        {
            //** BaseEditEntityArea.ShowSearchView: 397915c7e37f


            //CalculateFilterValues();
            //bool filtersChanged = false;
            //if (CurrentValues.Any(x => !LastFilterValues.Any(y => x.Item1 == y.Item1 && x.Item2 == y.Item2)) ||
            // LastFilterValues.Any(x => !CurrentValues.Any(y => x.Item1 == y.Item1 && x.Item2 == y.Item2)))
            //    filtersChanged = true;
            ////بعدا که حالت کمبو هم اضافه شد اینها اعمال شوند
            //if (filtersChanged)
            //{
            //    searchInitialyDone = false;
            //    ViewEntityArea.AddData(new List<DP_DataView>(), true);
            //}


            //if (searchInitialyDone)
            //    sarchInitially = false;
            //if (sarchInitially == true)
            //{
            //    if (RelationshipFilters == null || RelationshipFilters.Count == 0)
            //    {
            //        if (searchInitialyDone)
            //            sarchInitially = false;
            //    }
            //}
            // if (ViewEntityArea == null)


            if (ViewEntityArea.LastTemporaryView != null && ViewEntityArea.LastTemporaryView.HasPopupView)
                ViewEntityArea.LastTemporaryView.RemovePopupView(ViewEntityArea.ViewForViewEntityArea);

            if (ViewForSearchAndView == null)
                ViewForSearchAndView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfSearchViewEntityArea();
            if (!ViewForSearchAndView.HasSearchAreaView)
                ViewForSearchAndView.AddSearchAreaView(SearchEntityArea.SearchView);


            ViewEntityArea.IsCalledFromDataView = fromDataView;

            if (!ViewForSearchAndView.HasViewAreaView)
                ViewForSearchAndView.AddViewAreaView(ViewEntityArea.ViewForViewEntityArea);
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(ViewForSearchAndView, SimpleEntity.Alias, Enum_WindowSize.Big);
        }

        //public void RemoveViewEntityAreaView()
        //{

        //    //
        //}

        public void ShowTemproraryUIViewArea(I_View_TemporaryView view)
        {
            //** BaseEditEntityArea.ShowTemproraryUIViewArea: 0cfe0a61872b

            ViewEntityArea.IsCalledFromDataView = false;
            if (ViewEntityArea.LastTemporaryView != null && ViewEntityArea.LastTemporaryView.HasPopupView)
                ViewEntityArea.LastTemporaryView.RemovePopupView(ViewEntityArea.ViewForViewEntityArea);
            if (ViewForSearchAndView != null && ViewForSearchAndView.HasViewAreaView)
                ViewForSearchAndView.RemoveViewAreaView(ViewEntityArea.ViewForViewEntityArea);
            if (!view.HasPopupView)
            {
                ViewEntityArea.LastTemporaryView = view;
                view.AddPopupView(ViewEntityArea.ViewForViewEntityArea);
            }
            view.PopupVisibility = true;
        }
        //private void GenerateViewEntityArea()
        //{


        //    //ViewForSearchAndView.AddViewAreaView(ViewEntityArea.ViewForViewEntityArea);

        //    //  CheckSearchInitially();

        //    //DataSelected += SearchViewEntityArea_DataSelected;

        //}




        private void SearchEntityArea_SearchDataDefined(object sender, DP_SearchRepositoryMain e)
        {
            //** BaseEditEntityArea.SearchEntityArea_SearchDataDefined: 3841062ffaca
            var result = GetDataViewItemsFromSearchResult(e);
            ViewEntityArea.AddData(result);

        }
        //  public List<Tuple<int, object>> LastFilterValues = new List<Tuple<int, object>>();
        // public List<Tuple<int, object>> CurrentValues = new List<Tuple<int, object>>();
        private List<DP_DataView> GetDataViewItemsFromSearchResult(DP_SearchRepositoryMain searchItems)
        {
            //BaseEditEntityArea.GetDataViewItemsFromSearchResult: 7e4651a61540
            //CalculateFilterValues();
            //if (FilterCalculationError != null)
            //{
            //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("خطا در جستجو", FilterCalculationError.Message, Temp.InfoColor.Red);
            //    return null;
            //}
            //LastFilterValues.Clear();
            //foreach (var item in CurrentValues)
            //    LastFilterValues.Add(new Tuple<int, object>(item.Item1, item.Item2));

            if (RelationshipFilters != null)
            {
                foreach (var filter in RelationshipFilters)
                {
                    DP_SearchRepositoryRelationship searchItem = CreateSearchItem(searchItems, filter.SearchRelationshipTail);
                    var searchColumn = new SearchProperty(filter.SearchColumn) { NotIgnoreZeroValue = true };
                    searchItem.Phrases.Add(searchColumn);
                    searchColumn.Value = ChildRelationshipInfoBinded.SourceData.GetValueSomeHow(filter.ValueRelationshipTail, filter.ValueColumnID);
                }
            }

            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
            DR_SearchViewRequest request = new DR_SearchViewRequest(requester, searchItems);
            var result = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(request);


            if (result.Result != Enum_DR_ResultType.ExceptionThrown)
            {
                return result.ResultDataItems;

            }
            else
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage(result.Message);
                return null;
            }

            //    return result;
        }
        //public void SearchConfirmed(DP_SearchRepositoryMain searchItems, bool select)
        //{
        //    //try
        //    //{

        //    //  if (result != null)
        //    //      UseSearchResult(result, select);

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("خطا در جستجو", ex.Message, Temp.InfoColor.Red);
        //    //}
        //}

        //private void UseSearchResult(DR_ResultSearchView result, bool select)
        //{

        //}

        private DP_SearchRepositoryRelationship CreateSearchItem(LogicPhraseDTO logicPhrase, EntityRelationshipTailDTO searchRelationshipTail)
        {
            if (searchRelationshipTail == null)
                return logicPhrase as DP_SearchRepositoryRelationship;
            else
            {
                var foundItem = new DP_SearchRepositoryRelationship();
                foundItem.SourceRelationship = searchRelationshipTail.Relationship;
                //foundItem.SourceEntityID = searchRelationshipTail.Relationship.EntityID1;
                //foundItem.SourceToTargetRelationshipType = searchRelationshipTail.Relationship.TypeEnum;
                //foundItem.SourceToTargetMasterRelationshipType = searchRelationshipTail.Relationship.MastertTypeEnum;
                logicPhrase.Phrases.Add(foundItem);
                return CreateSearchItem(foundItem, searchRelationshipTail.ChildTail);
            }
        }

        public I_View_SearchViewEntityArea ViewForSearchAndView
        {
            // ViewForSearchAndView: f3b7615955e1
            set; get;
        }

        //public List<I_Command> SearchViewCommands
        //{
        //    set; get;
        //}



        List<RelationshipFilterDTO> _RelationshipFilters;
        public List<RelationshipFilterDTO> RelationshipFilters
        {
            // BaseEditEntityArea.RelationshipFilters: ab2ed0528c76
            get
            {
                if (_RelationshipFilters == null)
                {
                    if (SourceRelationColumnControl != null)
                    {
                        _RelationshipFilters = AgentUICoreMediator.GetAgentUICoreMediator.relationshipFilterManagerService.GetRelationshipFilters(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), SourceRelationColumnControl.Relationship.ID);
                    }
                    else
                        return null;
                }
                return _RelationshipFilters;
            }
            set
            {
                _RelationshipFilters = value;
            }
        }

        //private bool IsCalledFromDataView
        //{
        //    set; get;
        //}



        // public event EventHandler<DataSelectedEventArg> DataSelected;

        public bool SearchInitialyDone { get; set; }


        public void CheckSearchInitially()
        {
            //**b07a3760-ff93-410f-b6b9-e40244e72ac7
            if (SearchInitialyDone == false)
            {
                //   bool? sarchInitially = null;
                if (SimpleEntity.SearchInitially == true || (SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship.SearchInitially))
                {
                    DP_SearchRepositoryMain searchItems = new DP_SearchRepositoryMain(AreaInitializer.EntityID);
                    SearchInitialyDone = true;
                    //     ShowSearchResultInViewEntityArea(searchItems);
                    SearchEntityArea.OnSearchDataDefined(searchItems);
                }
            }
        }

        //public void SearchInitialy()
        //{

        //}

        //Exception FilterCalculationError = null;
        //private void CalculateFilterValues()
        //{
        //    FilterCalculationError = null;
        //    CurrentValues.Clear();
        //    try
        //    {
        //        //////if (AreaInitializer.SourceEditArea.ChildRelationshipInfo != null)
        //        //////{
        //        //////    if (RelationshipFilters != null && RelationshipFilters.Any())
        //        //////    {
        //        //////        foreach (var filter in RelationshipFilters)
        //        //////        {
        //        //////            var value = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.GetValueSomeHow(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.SourceEditArea.ChildRelationshipInfo.SourceData, filter.ValueRelationshipTail, filter.ValueColumnID);
        //        //////            if (value != null && !string.IsNullOrEmpty(value.ToString()))
        //        //////            {
        //        //////                CurrentValues.Add(new Tuple<int, object>(filter.ID, value));
        //        //////            }
        //        //////        }
        //        //////    }
        //        //////}
        //    }
        //    catch (Exception ex)
        //    {
        //        FilterCalculationError = ex;
        //    }
        //}

        //public void SelectFromParent(RelationshipDTO relationship, DP_DataRepository parentDataItem, Dictionary<int, object> colAndValues)
        //{

        //    //** d1b77658-4eb8-4a8b-8f0d-ecc5e3d40fa0
        //    //IsCalledFromDataView = isCalledFromDataView;
        //    //DP_SearchRepositoryMain searchItems = new DP_SearchRepositoryMain(AreaInitializer.EntityID);
        //    //foreach (var item in relationship.RelationshipColumns)
        //    //{
        //    //    if (colAndValues.ContainsKey(item.FirstSideColumnID))
        //    //    {
        //    //        searchItems.Phrases.Add(new SearchProperty(item.SecondSideColumn) { Value = colAndValues[item.FirstSideColumnID] });
        //    //    }
        //    //}

        //    //  var result = GetDataViewItemsFromSearchResult(searchItems);

        //    //bool isCalledFromDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
        //    //                            AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);



        //}
        //public void SelectDataFromExternal(List<Dictionary<ColumnDTO, object>> items)
        //{
        //    //** d6d63e62-6484-40a2-8512-4241e0aed153
        //    if (SourceRelationColumnControl != null)
        //        throw new Exception("errorrrr");

        //    List<DP_BaseData> dataitems = new List<DP_BaseData>();
        //    foreach (var item in items)
        //    {
        //        DP_BaseData dataItem = new DP_BaseData(AreaInitializer.EntityID, "");
        //        foreach (var col in item)
        //            dataItem.Properties.Add(new EntityInstanceProperty(col.Key) { Value = col.Value });
        //        dataitems.Add(dataItem);
        //    }
        //    //    var result = GetDataViewItemsFromSearchResult(searchItems);
        //    //bool isCalledFromDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
        //    //                           AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);

        //    SelectData(dataitems);
        //}

        I_SearchEntityArea _SearchEntityArea;
        public I_SearchEntityArea SearchEntityArea
        {
            get
            {
                if (_SearchEntityArea == null)
                {
                    //** BaseEditEntityArea.SearchEntityArea: 54e3b4a36ac9
                    var searchViewInitializer = new SearchAreaInitializer();
                    searchViewInitializer.EntityID = AreaInitializer.EntityID;
                    searchViewInitializer.EntitySearchID = AreaInitializer.EntitySearchID;
                    _SearchEntityArea = new SearchEntityArea(searchViewInitializer);
                    _SearchEntityArea.SearchDataDefined += SearchEntityArea_SearchDataDefined;

                }
                return _SearchEntityArea;
            }
        }
        //public I_View_SearchEntityArea SearchEntityAreaView
        //{
        //    get
        //    {
        //        return SearchEntityArea.SearchView;
        //    }
        //}
        I_ViewEntityArea _ViewEntityArea;
        public I_ViewEntityArea ViewEntityArea
        {
            get
            {
                if (_ViewEntityArea == null)
                {
                    //** BaseEditEntityArea.ViewEntityArea: 81a1c5c88a88
                    var viewAreaInitializer = new ViewEntityAreaInitializer();
                    viewAreaInitializer.EntityID = AreaInitializer.EntityID;
                    viewAreaInitializer.EntityListViewID = AreaInitializer.EntityListViewID;
                    viewAreaInitializer.MultipleSelection = this is I_EditEntityAreaMultipleData;
                    _ViewEntityArea = new ViewEntityArea(viewAreaInitializer);
                    _ViewEntityArea.DataSelected += ViewEntityArea_DataSelected;
                }
                return _ViewEntityArea;
            }
        }
        //I_View_ViewEntityArea _ViewForViewEntityArea;
        //public I_View_ViewEntityArea ViewForViewEntityArea
        //{
        //    get
        //    {
        //        if (_ViewForViewEntityArea == null)
        //        {
        //            return ViewEntityArea.ViewForViewEntityArea;
        //        }
        //        return _ViewForViewEntityArea;
        //    }
        //}
        //private void DetermineInteractionMode()
        //{

        //}
        //public void ManageSecurity()
        //{
        //    if (SimpleEntity.IsReadonly)
        //        AreaInitializer.SecurityReadOnly = true;
        //    //اگر فرم حالت ادیت داشت چک شود
        //    if (Permission.GrantedActions.Any(x => x == SecurityAction.NoAccess ||
        //       x == SecurityAction.ReadOnly || x == SecurityAction.EditAndDelete ||
        //       x == SecurityAction.Edit))
        //    {
        //        if (Permission.GrantedActions.Any(x => x == SecurityAction.NoAccess))
        //            AreaInitializer.SecurityNoAccess = true;
        //        else if (Permission.GrantedActions.Any(x => x == SecurityAction.ReadOnly))
        //        {
        //            AreaInitializer.SecurityReadOnly = true;
        //        }
        //        else if (Permission.GrantedActions.Any(x => x == SecurityAction.Edit))
        //        {
        //            AreaInitializer.SecurityEditOnly = true;
        //        }
        //        else
        //        {
        //            AreaInitializer.SecurityEditAndDelete = true;
        //        }
        //    }
        //    else
        //        AreaInitializer.SecurityNoAccess = true;

        //    if (SourceRelationColumnControl != null)
        //    {
        //        if (SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
        //        {
        //            if (AreaInitializer.SecurityReadOnly)
        //            {
        //                //اینجا بیخوده چون ریدونلی بالاتر از ریدونلی بای پرنته
        //                AreaInitializer.SecurityReadOnlyByParent = true;
        //            }
        //            else
        //            {
        //                var rel = SourceRelationColumnControl.Relationship;
        //                foreach (var relCol in rel.RelationshipColumns)
        //                {
        //                    if (relCol.SecondSideColumn.IsReadonly)
        //                        AreaInitializer.SecurityReadOnlyByParent = true;
        //                    if (Permission.ChildsPermissions.Any(x => x.SecurityObjectID == relCol.SecondSideColumnID && x.GrantedActions.Any(y => y == SecurityAction.ReadOnly)))
        //                        AreaInitializer.SecurityReadOnlyByParent = true;
        //                }
        //            }
        //        }
        //    }

        //    //ImposeCommandSecurity();
        //}

        //public void GenerateUIifNeeded()
        //{


        //}
        //    public TemporaryLinkState TemporaryLinkState { set; get; }
        private void GenerateTempView()
        {
            //** BaseEditEntityArea.GenerateTempView: d932102d6454
            if (TemporaryDisplayView == null)
                TemporaryDisplayView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTemporaryLinkUI();

            TemporaryDisplayView.ButtonInfoVisible = true;
            TemporaryDisplayView.ButtonClearVisible = true;

            TemporaryDisplayView.ButtonSearchFormVisible = false;
            TemporaryDisplayView.ButtonPopupVisible = false;
            //   TemporaryDisplayView.ButtonQuickSearchVisible = false;
            TemporaryDisplayView.ButtonDataEditVisible = false;
            //    TemporaryDisplayView.InfoTextboxVisible = false;
            if (AreaInitializer.IntracionMode == IntracionMode.Select || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            {
                TemporaryDisplayView.ButtonSearchFormVisible = true;
                TemporaryDisplayView.ButtonPopupVisible = true;
                //   TemporaryDisplayView.ButtonQuickSearchVisible = true;
                //    TemporaryDisplayView.InfoTextboxVisible = true;
                if (!AreaInitializer.Preview)
                    TemporaryDisplayView.SearchTextChanged += TemporaryDisplayView_TextChanged;

                if (!AreaInitializer.Preview)
                    TemporaryDisplayView.FocusLost += TemporaryDisplayView_FocusLost;
            }
            if (AreaInitializer.IntracionMode == IntracionMode.CreateInDirect || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            {
                TemporaryDisplayView.ButtonDataEditVisible = true;
            }

            if (!AreaInitializer.Preview)
                TemporaryDisplayView.TemporaryDisplayViewRequested += TemporaryDisplayView_TemporaryDisplayViewRequested;
        }


        void TemporaryDisplayView_TemporaryDisplayViewRequested(object sender, Arg_TemporaryDisplayViewRequested e)
        {
            //** BaseEditEntityArea.TemporaryDisplayView_TemporaryDisplayViewRequested: 95a95d63fa08
            if (SourceRelationColumnControl == null)
            {
                var TemporaryView = sender as I_View_TemporaryView;
                if (e.LinkType == TemporaryLinkType.DataView)
                {
                    throw new Exception("Asdasdas");
                }
                else if (e.LinkType == TemporaryLinkType.SerachView)
                {

                    ShowSearchView(false);
                }
                //else if (e.LinkType == TemporaryLinkType.QuickSearch)
                //{
                //    TemporaryView.QuickSearchVisibility = !TemporaryView.QuickSearchVisibility;
                //    if (TemporaryView.QuickSearchVisibility)
                //        TemporaryView.QuickSearchSelectAll();
                //}
                else if (e.LinkType == TemporaryLinkType.Popup)
                {
                    if (!TemporaryView.PopupVisibility)
                    {
                        ShowTemproraryUIViewArea(TemporaryView);
                    }
                    else
                        TemporaryView.PopupVisibility = false;
                }
                else if (e.LinkType == TemporaryLinkType.Clear)
                {
                    ClearData();
                }
                else if (e.LinkType == TemporaryLinkType.Info)
                {
                    AgentHelper.ShowEditEntityAreaInfo(this);
                }
            }
            else
            {

                // throw new Exception("inja miad??");
                ChildRelationshipInfoBinded.TemporaryViewActionRequested(sender as I_View_TemporaryView, e.LinkType);

            }

        }
        private void TemporaryDisplayView_FocusLost(object sender, EventArgs e)
        {
            (sender as I_View_TemporaryView).PopupVisibility = false;
        }
        private void TemporaryDisplayView_TextChanged(object sender, Arg_TemporaryDisplaySerachText e)
        {
            //TemporaryViewSearchTextChanged(sender as I_View_TemporaryView, e);
            // ** 95c85682-3cbc-4602-8e56-9875364a9f76
            if (!string.IsNullOrEmpty(e.Text))
            {
                SearchInitialyDone = true;
                var searchItems = AgentHelper.GetQuickSearchLogicPhrase(EntitySearchDTO, AreaInitializer.EntityID, e.Text);
                SearchEntityArea.OnSearchDataDefined(searchItems);

                //   ShowSearchResultInViewEntityArea(searchItems);


                // UseSearchResult(result);
                ShowTemproraryUIViewArea(sender as I_View_TemporaryView);
            }

        }


        public void GenerateDataView()
        {
            //** BaseEditEntityArea.GenerateDataView: 4ba73472fca2
            if (DataView == null)
            {
                if (this is I_EditEntityAreaOneData)
                    (this as I_EditEntityAreaOneData).DataView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateEditEntityAreaOneDataView(DataEntryEntity.UICompositions.EntityUISetting);
                else if (this is I_EditEntityAreaMultipleData)
                    (this as I_EditEntityAreaMultipleData).DataView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateEditEntityAreaMultipleDataView(DataEntryEntity.UICompositions.EntityUISetting);
            }
            GenerateCommands();
            GenerateControls();

            if (AreaInitializer.Preview)
                return;
            if (SourceRelationColumnControl == null)
                DecideButtonsEnablity1();
            //////ImposeDataViewSecurity();
        }


        public I_View_Area DataView
        {
            get
            {
                if (this is I_EditEntityAreaOneData)
                    return (this as I_EditEntityAreaOneData).DataView;
                else if (this is I_EditEntityAreaMultipleData)
                    return (this as I_EditEntityAreaMultipleData).DataView;
                else return null;
            }
        }
        public I_View_Area FirstView
        {
            get
            {
                I_View_Area view = null;
                if (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                        AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                    view = DataView;
                else if (AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
                        AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect
                        || AreaInitializer.IntracionMode == IntracionMode.Select)
                    view = TemporaryDisplayView;
                return view;
            }
        }

        //private void ClearDataViewControls()
        //{
        //    //ظاهرا بیخوده چون با صدا زدن جنریت دیتاویو همه چی از ازول ساخته میشه؟؟؟
        //    //برای فرم چینش داخل فرم که هر بار فرم ساخته نمیشود و فقط کنترلها تولید میشوند
        //    DataView.ClearControls();

        //    foreach (var item in SimpleColumnControls)
        //    {
        //        item.Visited = false;
        //        //     DataView.RemoveUIControlPackage(item.ControlManager);
        //    }
        //    foreach (var item in RelationshipColumnControls)
        //    {
        //        item.Visited = false;
        //        //   DataView.RemoveView(item.ControlManager);
        //    }
        //}
        public void GenerateControls()
        {
            //**  BaseEditEntityArea.GenerateControls: ddfdfcbe4f89

            SimpleColumnControls.Clear();
            RelationshipColumnControls.Clear();
            //DataEntryEntity.Relationships.Clear();
            GenerateUIControlsByCompositionDTO(DataEntryEntity.UICompositions);
            //List<BaseColumnControl> sortedListOfColumnControls = new List<EntityArea.BaseColumnControl>();

            //foreach (var column in DataEntryEntity.Columns.OrderBy(x => x.Position))
            //{
            //    if (DataEntryEntity.Relationships.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID)))
            //    {
            //        if (!RelationshipColumnControls.Any(x => x.Relationship.RelationshipColumns.Any(z => z.FirstSideColumnID == column.ID)))
            //        {
            //            var relationship = DataEntryEntity.Relationships.First(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID));
            //            AddRelationshipControl(relationship, sortedListOfColumnControls);
            //        }
            //    }
            //    else
            //    {
            //        bool hasRangeOfValues = column.ColumnValueRange != null && column.ColumnValueRange.Details.Any();

            //        SimpleColumnControlGenerel propertyControl = null;
            //        if (this is I_EditEntityAreaOneData)
            //        {
            //            propertyControl = new SimpleColumnControlOne() { Column = column };

            //            (propertyControl as SimpleColumnControlOne).SimpleControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateSimpleControlManagerForOneDataForm(column, column.ColumnUISetting, hasRangeOfValues, null);
            //        }
            //        else if (this is I_EditEntityAreaMultipleData)
            //        {
            //            propertyControl = new SimpleColumnControlMultiple() { Column = column };

            //            (propertyControl as SimpleColumnControlMultiple).SimpleControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateSimpleControlManagerForMultipleDataForm(column, column.ColumnUISetting, hasRangeOfValues);
            //        }
            //        propertyControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(propertyControl.Column.Alias);
            //        AgentHelper.SetPropertyTitle(propertyControl);

            //        var info = column.ID + "," + column.Name;

            //        SimpleColumnControls.Add(propertyControl);
            //        sortedListOfColumnControls.Add(propertyControl);

            //    }
            //}
            //if (SourceRelationColumnControl != null)
            //{
            //    if (SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.SubToSuper
            //        || SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.SuperToSub)
            //    {
            //        //////if (!SourceRelationColumnControl.TargetRelationColumns.Select(x => x.ID).Contains(column.ID))
            //        //////    if (SourceRelationColumnControl.SourceEditArea.FullEntity.Columns.Any(x => x.ID == column.ID))
            //        //////        continue;
            //    }
            //}

            //foreach (var relationship in DataEntryEntity.Relationships
            //                .Where(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign))
            //{
            //    AddRelationshipControl(relationship, sortedListOfColumnControls);
            //}





            //foreach (var columnControl in sortedListOfColumnControls.Where(x => x.Visited == false))
            //{
            //    //برای تغییر کرده ها
            //    if (columnControl.Visited == true)
            //        continue;
            //    if (columnControl is SimpleColumnControlGenerel)
            //    {
            //        var simpleColumn = (columnControl as SimpleColumnControlGenerel);
            //        columnControl.Visited = true;
            //        if (columnControl is SimpleColumnControlOne)
            //            (this as I_EditEntityAreaOneData).DataView.AddUIControlPackage((columnControl as SimpleColumnControlOne).SimpleControlManager, columnControl.LabelControlManager);
            //        else if (columnControl is SimpleColumnControlMultiple)
            //            (this as I_EditEntityAreaMultipleData).DataView.AddUIControlPackage((columnControl as SimpleColumnControlMultiple).SimpleControlManager, columnControl.LabelControlManager);
            //    }
            //    else if (columnControl is RelationshipColumnControlGeneral)
            //    {

            //        var relationshipControl = (columnControl as RelationshipColumnControlGeneral);

            //        //if (relationshipControl.EditNdTypeArea.TemporaryDisplayView != null)
            //        //{
            //        ////بعدا بررسی شود
            //        //if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
            //        //    ManageSuperToSubControl(sortedListOfColumnControls, relationshipControl);

            //        //if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion_UnionHoldsKeys)
            //        //    ManageUnionToSubUnionControl(sortedListOfColumnControls, relationshipControl);

            //        //if (!relationshipControl.Visited)
            //        //{
            //        columnControl.Visited = true;

            //        if (relationshipControl is RelationshipColumnControlOne)
            //            (this as I_EditEntityAreaOneData).DataView.AddView(columnControl.LabelControlManager, (columnControl as RelationshipColumnControlOne).RelationshipControlManager);
            //        else if (relationshipControl is RelationshipColumnControlMultiple)
            //            (this as I_EditEntityAreaMultipleData).DataView.AddView(columnControl.LabelControlManager, (columnControl as RelationshipColumnControlMultiple).RelationshipControlManager);

            //        //}

            //        //}
            //    }
            //}

            ////بعدا بررسی شود
            //foreach (var relationshipControl in RelationshipColumnControls.Where(x => x.Visited == false))
            //{
            //    //اون بالایی مخصوص اوناییه که ویو موقت دارند
            //    //if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
            //    //    ManageSuperToSubControl(sortedListOfColumnControls, relationshipControl);

            //    //if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion_UnionHoldsKeys)
            //    //    ManageUnionToSubUnionControl(sortedListOfColumnControls, relationshipControl);

            //    if (!relationshipControl.Visited)
            //    {
            //        relationshipControl.Visited = true;
            //        (this as I_EditEntityAreaOneData).DataView.AddView(relationshipControl.ControlManager, relationshipControl.ControlManager.LabelControlManager);
            //    }
            //}


            //می توانند یکی شوند
            foreach (var columnControl in SimpleColumnControls)
            {
                if (columnControl.DataEntryColumn.IsMandatory)
                {
                    var text = columnControl.LabelControlManager.GetValue();
                    if (text != null && !text.ToString().StartsWith("*"))
                    {
                        text = "* " + text;
                        columnControl.LabelControlManager.SetValue(text);
                    }
                }
            }
            foreach (var columnControl in RelationshipColumnControls)
            {
                if (columnControl.Relationship.IsOtherSideMandatory)
                {
                    var text = columnControl.LabelControlManager.GetValue();
                    if (text != null && !text.ToString().StartsWith("*"))
                    {
                        {
                            text = "* " + text;
                            columnControl.LabelControlManager.SetValue(text);
                        }
                    }
                }
                if ((SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
                || RelationshipColumnControls.Any(x => x.Relationship is SuperToSubRelationshipDTO && (x.Relationship as SuperToSubRelationshipDTO).ISARelationship.InternalTable))
                {
                    AddDeterminerStates();
                }
                //برای برعکسش فکر بشه//یعنی اگر زیر مجموعه وضعیتش تغییر کنه و رابطه پدر مخفی شود مثلا

                //foreach (var columnControl in SimpleColumnControls)
                //{
                //    //فقط ستونهای ساده اضافه میشوند
                //    ColumnAttributes columnAtrributes = new ColumnAttributes();
                //    columnAtrributes.ColumnID = columnControl.Column.ID;
                //    columnAtrributes.ColumnControl = columnControl;
                //    AreaInitializer.FormAttributes.ColumnAttributes.Add(columnAtrributes);
                //}

                if (DataViewGenerated != null)
                    DataViewGenerated(this, null);

            }
            AreaInitializer.FormComposed = true;

        }



        //private void DecideSimpleColumnReadony(SimpleColumnControl propertyControl)
        //{


        //    //bool isReadonly = false;

        //    //if (propertyControl.Column.DataEntryEnabled == false)
        //    //    isReadonly = true;

        //    //if (propertyControl.Column.IsReadonly)
        //    //{
        //    //    isReadonly = true;
        //    //}
        //    //  propertyControl.IsReadonly = isReadonly;
        //    //if (isReadonly && impose)
        //    //{

        //    //}
        //    //   return isReadonly;
        //}

        //private void AddRelationshipControl(DataEntryRelationshipDTO dataEntryRelationship, List<BaseColumnControl> sortedListOfColumnControls)
        //{
        //    RelationshipColumnControlGeneral relationshipColumnControl;
        //    if (this is I_EditEntityAreaOneData)
        //        relationshipColumnControl = new RelationshipColumnControlOne();
        //    else
        //        relationshipColumnControl = new RelationshipColumnControlMultiple();
        //    relationshipColumnControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(dataEntryRelationship.Relationship.Alias);
        //    relationshipColumnControl.DataEntryRelationship = dataEntryRelationship;
        //    relationshipColumnControl.ParentEditArea = this;
        //    var relationshipAlias = relationshipColumnControl.Relationship.Alias;
        //    relationshipColumnControl.Alias = (string.IsNullOrEmpty(relationshipAlias) ? "" : relationshipAlias + " : ");

        //    //اینجا ادیت اریا رابطه و همچنین کنترل منیجر رابطه مشخص میشوند. اگر مثلا کاربر به موجودیت رابطه دسترسی نداشته باشد این مقادیر تولید نمی شوند و نال بر میگردد

        //    bool generated = GenerateMultipleEditEntityAreaWithUIControlPackage(relationshipColumnControl as RelationshipColumnControlMultiple);


        //    if (generated != false)
        //    {

        //        RelationshipColumnControls.Add(relationshipColumnControl);
        //        //  sortedListOfColumnControls.Add(relationshipColumnControl);

        //        //برای ادمین فعال شود بد نیست
        //        //  var info = dataEntryRelationship.Relationship.ID + "," + dataEntryRelationship.Relationship.Name + Environment.NewLine + dataEntryRelationship.Relationship.TypeStr + Environment.NewLine + dataEntryRelationship.Relationship.Info;
        //        //AddColumnControlMessage(new ColumnControlMessageItem(relationshipColumnControl, ControlOrLabelAsTarget.Label) { Key = "relationshipinfo", Message = info });
        //        //    AddRelationshipColumnMessageItem(propertyControl, info, InfoColor.Black, "permanentCaption", null, true);

        //    }
        //}
        public List<RelationshipColumnControlGeneral> SkippedRelationshipColumnControl { set; get; }


        //private void RelationshipColumnControl_DataViewForTemporaryViewShown(object sender, ChildRelationshipInfo e)
        //{
        //    var relationshipColumnControl = sender as RelationshipColumnControlGeneral;
        //    if (relationshipColumnControl != null)
        //    {
        //        e.SetMessageAndColor();
        //        relationshipColumnControl.GenericEditNdTypeArea.DecideButtonsEnablity1();
        //    }
        //}



        //private bool ParentIsSubToSuperSkipCondition(RelationshipColumnControlGeneral relationshipColumnControl)
        //{
        //    if (relationshipColumnControl.Relationship.IsOtherSideDirectlyCreatable)
        //    {
        //        if (SimpleEntity.IndependentDataEntry == true && relationshipColumnControl.Relationship.Entity2IsIndependent)
        //        {
        //            //منظور اینه که اگر روابط زیر نباشند اسکیپ میشوند
        //            if (relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.OneToMany &&
        //              relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.ManyToOne &&
        //              relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.ExplicitOneToOne)
        //                return true;
        //        }
        //    }
        //    else
        //    {
        //        //یعنی اگر این شرط برقرار باشد در هر صورت اسکیپ میشود چه مستقیم باشد یا نباشد چه ایندپندنت باشد و..
        //        if (SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
        //        {
        //            var parentIsaRelationship = (SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship;
        //            if (parentIsaRelationship.IsDisjoint)
        //            {
        //                if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
        //                {
        //                    var isaRelationship = (relationshipColumnControl.Relationship as SuperToSubRelationshipDTO).ISARelationship;
        //                    if (isaRelationship.ID == parentIsaRelationship.ID)
        //                        return true;
        //                }
        //            }
        //        }
        //        if (SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
        //        {
        //            var parentUnionRelationship = (SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
        //            if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion)
        //            {
        //                var unionRelationship = (relationshipColumnControl.Relationship as UnionToSubUnionRelationshipDTO).UnionRelationship;
        //                if (unionRelationship.ID == parentUnionRelationship.ID)
        //                    return true;
        //            }

        //        }
        //    }
        //    return false;
        //}

        //private bool ParentIsNotSubToSuperSkipCondition(RelationshipColumnControlGeneral relationshipColumnControl)
        //{
        //    if (relationshipColumnControl.Relationship.IsOtherSideDirectlyCreatable)
        //    {
        //        if (SimpleEntity.IndependentDataEntry == true && relationshipColumnControl.Relationship.Entity2IsIndependent)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private bool RelationshipSkipable(RelationshipColumnControlGeneral relationshipColumnControl)
        //{
        //    //** 5410c49f-5d04-40f3-8b48-a579903e12bd                                                       
        //    if (relationshipColumnControl.Relationship.IsOtherSideMandatory)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
        //            return true;
        //        else if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
        //        {
        //            var isaRelationship = (relationshipColumnControl.Relationship as SuperToSubRelationshipDTO).ISARelationship;

        //            int parentIsaRelationshipID = 0;
        //            if (SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO)
        //                parentIsaRelationshipID = (SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.ID;
        //            if (isaRelationship.ID == parentIsaRelationshipID)
        //            {
        //                if (isaRelationship.IsDisjoint)
        //                    return false;
        //                else
        //                    return SimpleEntity.IndependentDataEntry == true || relationshipColumnControl.Relationship.Entity2IsIndependent;
        //            }
        //            else
        //                return SimpleEntity.IndependentDataEntry == true || relationshipColumnControl.Relationship.Entity2IsIndependent;
        //            return isaRelationship.IsTolatParticipation;
        //        }
        //        else if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
        //        {
        //            var unionRelationship = (relationshipColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
        //            return unionRelationship.IsTolatParticipation;
        //        }
        //        else
        //        {
        //            //relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub
        //            var unionRelationship = (relationshipColumnControl.Relationship as UnionToSubUnionRelationshipDTO).UnionRelationship;
        //            var parentIsNotSubToSuperOrIsDifferent = false;
        //            if (!(SourceRelationColumnControl.Relationship is SubUnionToSuperUnionRelationshipDTO))
        //                parentIsNotSubToSuperOrIsDifferent = true;
        //            else if ((SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship.ID != unionRelationship.ID)
        //                parentIsNotSubToSuperOrIsDifferent = true;
        //            return parentIsNotSubToSuperOrIsDifferent;
        //        }
        //    }


        //}

        //private bool CheckRelationshipControlIsaUnionSkip(RelationshipColumnControl relationshipColumnControl)
        //{
        //    if (SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper ||
        //      SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion_UnionHoldsKeys)
        //    {
        //        if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.OneToMany ||
        //            relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.ManyToOne ||
        //            relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.ExplicitOneToOne ||
        //            relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.ImplicitOneToOne)
        //            return false;
        //    }

        //    if (relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.SubToSuper &&
        //              relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.SubUnionToUnion_UnionHoldsKeys)
        //    {

        //    }

        //    if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
        //    {
        //        var isaRelationship = (relationshipColumnControl.Relationship as SuperToSubRelationshipDTO).ISARelationship;
        //        var parentIsNotSubToSuperOrIsDifferent = false;
        //        if (!(SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO))
        //            parentIsNotSubToSuperOrIsDifferent = true;
        //        else if ((SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.ID != isaRelationship.ID)
        //            parentIsNotSubToSuperOrIsDifferent = true;
        //        if (isaRelationship.IsTolatParticipation && parentIsNotSubToSuperOrIsDifferent)
        //        {
        //            return true;
        //        }
        //        if (isaRelationship.IsDisjoint)
        //        {
        //            if (isaRelationship.ID == parentIsaRelationship.ID)
        //                return false;
        //        }
        //    }

        //    if (SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion_UnionHoldsKeys)
        //    {
        //        if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion_UnionHoldsKeys)
        //        {
        //            var parentUnionRelationship = (SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
        //            var unionRelationship = (relationshipColumnControl.Relationship as SuperUnionToSubUnionRelationshipDTO).UnionRelationship;
        //            if (unionRelationship.IsTolatParticipation && isaRelationship.ID != parentIsaRelationship.ID)
        //            {
        //                return true;
        //            }
        //            if (parentIsaRelationship.IsDisjoint)
        //            {
        //                if (isaRelationship.ID == parentIsaRelationship.ID)
        //                    return false;
        //            }
        //        }
        //    }

        //    return true;
        //}

        //private bool RelationshipColumnControlIsLogicallyMandatory(RelationshipColumnControl relationshipColumnControl)
        //{
        //    if (relationshipColumnControl.Relationship.IsOtherSideMandatory)
        //        return true;
        //    if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
        //        return true;
        //    if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
        //    {
        //        var isaRelationship = (relationshipColumnControl.Relationship as SuperToSubRelationshipDTO).ISARelationship;
        //        if(isaRelationship.IsTolatParticipation)
        //        {
        //            isaRelationship.SubTypeEntities
        //        }
        //    }
        //}

        //private void ControlManager_FocusLost(object sender, EventArgs e)
        //{
        //    (sender as I_View_TemporaryView).PopupVisibility = false;
        //}
        //private void View_TemporaryViewSearchText(object sender, Arg_TemporaryDisplaySerachText e, I_EditEntityArea editEntityArea)
        //{
        //    if (editEntityArea is I_EditEntityAreaOneData)
        //        (editEntityArea as I_EditEntityAreaOneData).TemporaryViewSearchTextChanged(sender as I_View_TemporaryView, e);
        //}
        //private void View_TemporaryViewRequested(object sender, Arg_MultipleTemporaryDisplayViewRequested e, I_EditEntityArea editEntityArea)
        //{
        //    editEntityArea.TemporaryViewActionRequestedFromMultipleEditor(sender as I_View_TemporaryView, e.LinkType, editEntityArea.SourceRelationColumnControl.Relationship, e.DataItem as DP_FormDataRepository);
        //}
        //private void View_TemporaryViewLoaded(object sender, Arg_MultipleTemporaryDisplayLoaded e, RelationshipColumnControlGeneral relationshipColumnControl)
        //{
        //    //////if (relationshipColumnControl.EditNdTypeArea.AreaInitializer.SecurityReadOnlyByParent)
        //    //////{
        //    //////    relationshipColumnControl.RelationshipControlManager.EnableDisable(e.DataItem, TemporaryLinkType.SerachView, false);
        //    //////    relationshipColumnControl.RelationshipControlManager.EnableDisable(e.DataItem, TemporaryLinkType.Clear, false);
        //    //////}
        //}

        //public EditEntityAreaInitializer GenereateAreaInitializerFromRelationshipColumn(RelationshipColumnControlGeneral relationshipColumnControl)
        //{
        //    //**BaseEditEntityArea.GenereateAreaInitializer 33596d4b-21fc-4799-9f24-e432367abf73

        //}

        //private bool GenerateMultipleEditEntityAreaWithUIControlPackage(RelationshipColumnControlMultiple relationshipColumnControl, RelationshipUISettingDTO relationshipUISetting)
        //{
        //    var newAreaInitializer = GenereateAreaInitializer(relationshipColumnControl);
        //    var editAreaResult = BaseEditEntityArea.GetEditEntityArea(newAreaInitializer);
        //    if (editAreaResult.Item1 != null)
        //    {
        //        var editArea = editAreaResult.Item1;


        //        relationshipColumnControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(relationshipColumnControl.Alias);
        //        relationshipColumnControl.GenericEditNdTypeArea = editArea;
        //        relationshipColumnControl.RelationshipControlManager = controlManager;
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        public I_EditEntityArea GenerateRelationshipControlEditArea(DataEntryRelationshipDTO dataEntryRelationship)
        {
            //** BaseEditEntityArea.GenerateRelationshipControlEditArea: 7b5673de233c
            EditEntityAreaInitializer newAreaInitializer = new EditEntityAreaInitializer();
            // newAreaInitializer.EditAreaDataManager = AreaInitializer.EditAreaDataManager;
            newAreaInitializer.ActionActivityManager = AreaInitializer.ActionActivityManager;
            newAreaInitializer.Preview = AreaInitializer.Preview;
            newAreaInitializer.EntityID = dataEntryRelationship.Relationship.EntityID2;
            // newAreaInitializer.SourceRelationColumnControl = relationshipColumnControl;
            newAreaInitializer.DataMode = dataEntryRelationship.DataMode;
            newAreaInitializer.IntracionMode = dataEntryRelationship.IntracionMode;
            newAreaInitializer.DataEntryRelationship = dataEntryRelationship;
            //return newAreaInitializer;

            //var newAreaInitializer = GenereateAreaInitializerFromRelationshipColumn(relationshipColumnControl);
            var editAreaResult = BaseEditEntityArea.GetEditEntityArea(newAreaInitializer);
            return editAreaResult.Item1;
        }

        //private void CheckDirectPossiblity(EditEntityAreaInitializer newAreaInitializer)
        //{
        //    if (RelationHistoryContainsEntityID(newAreaInitializer, newAreaInitializer.EntityID))
        //    {
        //        if (newAreaInitializer.IntracionMode == IntracionMode.CreateDirect)
        //            newAreaInitializer.IntracionMode = IntracionMode.CreateInDirect;
        //        else if (newAreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //            newAreaInitializer.IntracionMode = IntracionMode.CreateSelectInDirect;

        //    }
        //}


        //private void ManageSuperToSubControl(List<BaseColumnControl> sortedListOfColumnControls, RelationshipColumnControl relationshipControl)
        //{
        //    if (relationshipControl.Relationship is SuperToSubRelationshipDTO)
        //    {
        //        var mainSuperToSub = (relationshipControl.Relationship as SuperToSubRelationshipDTO);
        //        var listSuperToSubsRelationshipControlList = new List<RelationshipColumnControl>();
        //        foreach (var subRelationship in sortedListOfColumnControls.Where(x => x.Visited == false
        //         && (x is RelationshipColumnControl) && ((x as RelationshipColumnControl).Relationship is SuperToSubRelationshipDTO) &&
        //         ((x as RelationshipColumnControl).Relationship as SuperToSubRelationshipDTO).ISARelationship.ID == mainSuperToSub.ISARelationship.ID))
        //        {
        //            listSuperToSubsRelationshipControlList.Add((subRelationship as RelationshipColumnControl));
        //        }
        //        if (listSuperToSubsRelationshipControlList.Count > 1)
        //        {
        //            var tabgroupSetting = new TabGroupUISettingDTO();
        //            tabgroupSetting.Expander = true;
        //            tabgroupSetting.UIColumnsType = Enum_UIColumnsType.Full;

        //            var tabGroupContainer = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabGroup(tabgroupSetting);
        //            (this as I_EditEntityAreaOneData).SpecializedDataView.AddTabGroup(tabGroupContainer, (listSuperToSubsRelationshipControlList.First().Relationship as SuperToSubRelationshipDTO).ISARelationship.Name, tabgroupSetting);
        //            foreach (var suptoSubRelationshipControl in listSuperToSubsRelationshipControlList)
        //            {
        //                var tabpageSetting = new TabPageUISettingDTO();
        //                tabpageSetting.InternalColumnsCount = GetEntityUISetting().UIColumnsCount;
        //                var groupItem = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabPage(tabpageSetting);
        //                //هدر چک شود 
        //                tabGroupContainer.AddTabPage(groupItem, suptoSubRelationshipControl.Relationship.Alias, tabpageSetting, false);
        //                groupItem.AddView(suptoSubRelationshipControl.ControlManager, suptoSubRelationshipControl.ControlManager.LabelControlManager);
        //                suptoSubRelationshipControl.Visited = true;
        //            }
        //        }
        //    }
        //}
        //private void ManageUnionToSubUnionControl(List<BaseColumnControl> sortedListOfColumnControls, RelationshipColumnControl relationshipControl)
        //{
        //    if (relationshipControl.Relationship is SuperUnionToSubUnionRelationshipDTO)
        //    {
        //        var mainSuperToSub = (relationshipControl.Relationship as SuperUnionToSubUnionRelationshipDTO);
        //        var listSuperToSubsRelationshipControlList = new List<RelationshipColumnControl>();
        //        foreach (var subRelationship in sortedListOfColumnControls.Where(x => x.Visited == false
        //         && (x is RelationshipColumnControl) && ((x as RelationshipColumnControl).Relationship is SuperUnionToSubUnionRelationshipDTO) &&
        //         ((x as RelationshipColumnControl).Relationship as SuperUnionToSubUnionRelationshipDTO).UnionRelationship.ID == mainSuperToSub.UnionRelationship.ID))
        //        {
        //            listSuperToSubsRelationshipControlList.Add((subRelationship as RelationshipColumnControl));
        //        }
        //        if (listSuperToSubsRelationshipControlList.Count > 1)
        //        {
        //            var tabgroupSetting = new TabGroupUISettingDTO();
        //            tabgroupSetting.Expander = true;
        //            tabgroupSetting.UIColumnsType = Enum_UIColumnsType.Full;

        //            var tabGroupContainer = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabGroup(tabgroupSetting);
        //            (this as I_EditEntityAreaOneData).SpecializedDataView.AddTabGroup(tabGroupContainer, (listSuperToSubsRelationshipControlList.First().Relationship as SuperUnionToSubUnionRelationshipDTO).UnionRelationship.Name, tabgroupSetting);
        //            foreach (var suptoSubRelationshipControl in listSuperToSubsRelationshipControlList)
        //            {
        //                var tabpageSetting = new TabPageUISettingDTO();
        //                tabpageSetting.InternalColumnsCount = GetEntityUISetting().UIColumnsCount;
        //                var groupItem = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabPage(tabpageSetting);
        //                tabGroupContainer.AddTabPage(groupItem, suptoSubRelationshipControl.Relationship.Alias, tabpageSetting, false);
        //                groupItem.AddView(suptoSubRelationshipControl.ControlManager, suptoSubRelationshipControl.ControlManager.LabelControlManager);
        //                suptoSubRelationshipControl.Visited = true;
        //            }
        //        }
        //    }
        //}
        private void AddDeterminerStates()
        {
            //بهتره اضافه بشه به یو ای اکتیویتی منیجر و نسبت به دیتا حساس باشه
            List<Tuple<RelationshipDTO, I_EditEntityArea>> items = new List<Tuple<RelationshipDTO, I_EditEntityArea>>();
            if (SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
                items.Add(new Tuple<RelationshipDTO, I_EditEntityArea>(SourceRelationColumnControl.Relationship, SourceRelationColumnControl.ParentEditArea));
            foreach (var item in RelationshipColumnControls.Where(x => x.Relationship is SuperToSubRelationshipDTO && (x.Relationship as SuperToSubRelationshipDTO).ISARelationship.InternalTable))
            {
                items.Add(new Tuple<RelationshipDTO, I_EditEntityArea>(item.Relationship, item.GenericEditNdTypeArea));

            }
            foreach (var item in items)
            {
                //    var relationship = item.Item1 as SuperToSubRelationshipDTO;

                var entityState = new EntityStateDTO();
                //////entityState.ColumnID = item.Item2.SimpleEntity.DeterminerColumnID;
                //////entityState.Value = item.Item2.SimpleEntity.DeterminerColumnValue;
                ///
                //////EntityStateConditionDTO condition = new EntityStateConditionDTO();
                //////  condition.ColumnID = item.Item2.SimpleEntity.DeterminerColumnID;
                //////  condition.Values.Add(new  EntityStateValueDTO() { Value = item.Item2.SimpleEntity.DeterminerColumnValue });
                //////  entityState.StateConditions.Add(condition);


                entityState.Title = "determiner";// entityState.ColumnID + ">";// + entityState.Value;
                                                 //     entityState.EntityStateOperator = Enum_EntityStateOperator.NotEquals;
                var stateAction = new UIActionActivityDTO();
                stateAction.Title = item.Item1.Name + ">Hidden";
                stateAction.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { RelationshipID = item.Item1.ID, Hidden = true });
                entityState.ActionActivities.Add(stateAction);
                AreaInitializer.ActionActivityManager.EntityStates1.Add(entityState);
            }
            if (SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
            {
                //////if (FullEntity.Columns.Any(x => x.ID == SourceRelationColumnControl.SourceEditArea.FullEntity.DeterminerColumnID))
                //////{
                //////    var determinerColumn = FullEntity.Columns.First(x => x.ID == SourceRelationColumnControl.SourceEditArea.FullEntity.DeterminerColumnID);
                //////    determinerColumn.DefaultValue = SourceRelationColumnControl.SourceEditArea.FullEntity.DeterminerColumnValue;
                //////}
            }
            if (SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
            {
                if (SourceRelationColumnControl.ParentEditArea.SourceRelationColumnControl != null && SourceRelationColumnControl.ParentEditArea.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
                {
                    foreach (var item in SimpleColumnControls.Where(x => SourceRelationColumnControl.ParentEditArea.SourceRelationColumnControl.Relationship.RelationshipColumns.Any(y => y.SecondSideColumnID == x.DataEntryColumn.ID)))
                    {
                        //(item as SimpleColumnControl).BusinessReadOnly[] = true;
                        //(item as SimpleColumnControl).BusinessHidden = true;
                        //(item as SimpleColumnControl).ControlManager.EnableDisable(false);
                    }
                }
            }
        }
        private void GenerateCommands()
        {
            //BaseEditEntityArea.GenerateCommands: ada6635d8043
            if (Commands == null)
                Commands = new List<I_Command>();
            DataView.ClearCommands();

            //if (this.SourceRelationColumnControl == null && DataEntryEntity.IsReadonly)
            //    clearCommand.CommandManager.SetEnabled(false);
            //if (this.SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship.IsReadonly)
            //    clearCommand.CommandManager.SetEnabled(false);

            var infoCommand = new InfoCommand(this);
            Commands.Add(infoCommand);
            DataView.AddCommand(infoCommand.CommandManager, true);

            var logReportCommand = new LogReportCommand(this);
            Commands.Add(logReportCommand);
            DataView.AddCommand(logReportCommand.CommandManager, true);

            if (Permission.GrantedActions.Any(x => x == SecurityAction.ArchiveView || x == SecurityAction.ArchiveEdit))
            {
                var archiveCommand = new ArchiveCommand(this);
                Commands.Add(archiveCommand);
                DataView.AddCommand(archiveCommand.CommandManager, true);
            }

            if (Permission.GrantedActions.Any(x => x == SecurityAction.LetterView || x == SecurityAction.LetterEdit))
            {
                var letterCommand = new LetterCommand(this);
                Commands.Add(letterCommand);
                DataView.AddCommand(letterCommand.CommandManager, true);
            }

            var clearCommand = new ClearCommand(this);
            Commands.Add(clearCommand);
            DataView.AddCommand(clearCommand.CommandManager);//, (!(this is I_EditEntityAreaMultipleData) && SourceRelationColumnControl != null));

            if (SourceRelationColumnControl == null)
            {
                var saveCommand = new SaveCommand(this);
                Commands.Add(saveCommand);
                DataView.AddCommand(saveCommand.CommandManager);
                //if (DataEntryEntity.IsReadonly)
                //saveCommand.CommandManager.SetEnabled(false);



                var deleteCommand = new DeleteCommand(this);
                Commands.Add(deleteCommand);
                DataView.AddCommand(deleteCommand.CommandManager);

                var dataViewCommand = new DataViewCommand(this);
                Commands.Add(dataViewCommand);
                DataView.AddCommand(dataViewCommand.CommandManager, true);

                var gridViewCommand = new GridViewCommand(this);
                Commands.Add(gridViewCommand);
                DataView.AddCommand(gridViewCommand.CommandManager, true);

                var dataLinkCommand = new DataLinkCommand(this);
                Commands.Add(dataLinkCommand);
                DataView.AddCommand(dataLinkCommand.CommandManager, true);


                var graphCommand = new GraphCommand(this);
                Commands.Add(graphCommand);
                DataView.AddCommand(graphCommand.CommandManager, true);
                //var dataListReportCommand = new DataListReportCommand(this);
                //Commands.Add(dataListReportCommand);
                //DataView.AddCommand(dataListReportCommand.CommandManager, true);

            }
            if (AreaInitializer.IntracionMode == IntracionMode.CreateInDirect
         || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            {
                var saveCloseCommand = new UpdateAndCloseDialogCommand(this);
                Commands.Add(saveCloseCommand);
                DataView.AddCommand(saveCloseCommand.CommandManager);
                //این بعدا چک شود که ضروری است؟


                //var clearAndCloseCommand = new ClearAndClose(this);
                //Commands.Add(clearAndCloseCommand);
                //DataView.AddCommand(clearAndCloseCommand.CommandManager);
            }

            if (AreaInitializer.IntracionMode == IntracionMode.Select
               || AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect
                 || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            {
                var searchCommand = new SearchCommand(this);
                Commands.Add(searchCommand);
                DataView.AddCommand(searchCommand.CommandManager);//, SourceRelationColumnControl != null);
                                                                  //if (this.SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship.IsReadonly)
                                                                  //    searchCommand.CommandManager.SetEnabled(false);
            }

            if (this is I_EditEntityAreaMultipleData)
            {

                var addCommand = new AddCommand((this as I_EditEntityAreaMultipleData));
                Commands.Add(addCommand);
                DataView.AddCommand(addCommand.CommandManager);

                var removeCommand = new RemoveCommand((this as I_EditEntityAreaMultipleData));
                Commands.Add(removeCommand);
                DataView.AddCommand(removeCommand.CommandManager);

            }


            foreach (var command in EntityCommands)
            {
                var commandNew = new EntityCommand(command, this);// CommandManager.GenerateCommand(commandAttribute); ;
                Commands.Add(commandNew);
                DataView.AddCommand(commandNew.CommandManager, true);
            }

            //if (SourceRelationColumnControl != null)
            //{
            //    if (AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect
            //      || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            //    {
            //        DataView.DeHighlightCommands();
            //    }
            //}


        }
        //////public void ImposeDataViewSecurity()
        //////{
        //////    if (AreaInitializer.SecurityNoAccess == true)
        //////    {
        //////        if (DataView != null)
        //////            DataView.EnableDisable(false);
        //////        ////اینجا نباید بیاد قاعدتاً
        //////    }
        //////    else
        //////    {
        //////        if (AreaInitializer.SecurityReadOnly == true)
        //////        {
        //////            MakeFormReadOnly();
        //////        }
        //////        else if (AreaInitializer.SecurityEditOnly == true)
        //////        {
        //////            var deleteCommand = Commands.FirstOrDefault(x => x is DeleteCommand);
        //////            if (deleteCommand != null)
        //////                DisableEnableCommand(deleteCommand, false);
        //////        }

        //////        if (AreaInitializer.BusinessReadOnlyByParent || AreaInitializer.SecurityReadOnlyByParent)
        //////        {
        //////            var relChangeCommands = Commands.Where(x => x is ClearCommand || x is AddCommand || x is SearchCommand || x is RemoveCommand).ToList();
        //////            foreach (var command in relChangeCommands)
        //////            {
        //////                DisableEnableCommand(command, false);
        //////            }
        //////        }
        //////    }
        //////    foreach (var relationshipControl in RelationshipColumnControls)
        //////    {
        //////        if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
        //////           || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //////        {
        //////            relationshipControl.EditNdTypeArea.ImposeDataViewSecurity();
        //////        }
        //////        else if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect
        //////          || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
        //////        {
        //////            relationshipControl.EditNdTypeArea.ImposeTemporaryViewSecurity();
        //////        }

        //////    }
        //////}
        //////public void ImposeTemporaryViewSecurity()
        //////{
        //////    if (AreaInitializer.SecurityNoAccess == true)
        //////    {
        //////        TemporaryDisplayView.DisableEnable(false);
        //////    }
        //////    else
        //////    {
        //////        if (AreaInitializer.BusinessReadOnlyByParent || AreaInitializer.SecurityReadOnlyByParent)
        //////        {
        //////            TemporaryDisplayView.DisableEnable(TemporaryLinkType.SerachView, false);
        //////            TemporaryDisplayView.DisableEnable(TemporaryLinkType.Clear, false);
        //////        }
        //////    }
        //////}
        public UpdateResult UpdateDataAndValidate(List<DP_FormDataRepository> datas)
        {
            UpdateResult result = new UpdateResult();
            UpdateData(datas);
            result.IsValid = ValidateData(datas);
            if (!result.IsValid)
                result.Message = "اعتبارسنجی: برخی از داده های معتبر نمی باشند";
            else
            {
                SetDataIsUpdated(datas);
            }
            return result;

        }
        public void UpdateData(List<DP_FormDataRepository> datas)
        {
            //BaseEditEntityArea.UpdateData: 9376d8dbc2e0
            foreach (var data in datas.Where(x => x.ShoudBeCounted))
            {
                foreach (var childSimpleContorlProperty in data.ChildSimpleContorlProperties.Where(x => x.SimpleColumnControl.DataEntryColumn.ColumnCustomFormula != null && x.SimpleColumnControl.DataEntryColumn.ColumnCustomFormula.Formula != null))
                    AreaInitializer.UIFomulaManager.CalculateProperty(childSimpleContorlProperty);
                AreaInitializer.ActionActivityManager.CheckAndImposeEntityStates(data);
                foreach (var childRelInfo in data.ChildRelationshipDatas)
                {
                    if (childRelInfo.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
                             || childRelInfo.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                    {
                        childRelInfo.RelationshipControl.GenericEditNdTypeArea.UpdateData(childRelInfo.RelatedData.Cast<DP_FormDataRepository>().ToList());
                    }
                }
            }
        }

        public bool ValidateData(List<DP_FormDataRepository> datas)
        {
            bool result = true;
            //   result.IsValid = true;
            foreach (var data in datas.Where(x => x.ShoudBeCounted))
            {
                bool validationResult = AreaInitializer.UIValidationManager.ValidateData(data);
                if (!validationResult)
                {
                    result = false;
                }

                foreach (var childRelInfo in data.ChildRelationshipDatas)
                {
                    if (childRelInfo.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
                             || childRelInfo.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                    {
                        var childResult = childRelInfo.RelationshipControl.GenericEditNdTypeArea.ValidateData(childRelInfo.RelatedData.Cast<DP_FormDataRepository>().ToList());
                        if (!childResult)
                            result = false;
                    }
                }
            }
            return result;
        }

        public void SetDataIsUpdated(List<DP_FormDataRepository> datas)
        {
            foreach (var data in datas.Where(x => x.ShoudBeCounted))
            {
                data.IsUpdated = true;
                foreach (var childRelInfo in data.ChildRelationshipDatas)
                {
                    if (childRelInfo.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
                             || childRelInfo.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                    {
                        childRelInfo.RelationshipControl.GenericEditNdTypeArea.SetDataIsUpdated(childRelInfo.RelatedData.Cast<DP_FormDataRepository>().ToList());
                    }
                }
            }
        }

        //اینم دیگه بیخوده چوم فقط محاسبه فرمول برای هر پراپرتی رو میخوایمcalculatedPropertyTree
        //بعدا حذف بشه
        //  CalculatedPropertyTree calculatedPropertyTree = new CalculatedPropertyTree();
        // calculatedPropertyTree.DataItem = data;
        //if (relationship != null)
        //   //    calculatedPropertyTree.RelationshipInfo = relationship.Alias;
        //  result.Add(calculatedPropertyTree);



        //foreach (var relationshipControl in EditArea.RelationshipColumnControls)
        //{
        //    var childRelInfo = data.ChildRelationshipInfos.First(x => x.Relationship == relationshipControl.Relationship);
        //    if (!childRelInfo.IsHiddenOnState)
        //    {
        //        relationshipControl.EditNdTypeArea.SetChildRelationshipInfo(childRelInfo);
        //        if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
        //           || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //        {
        //            relationshipControl.EditNdTypeArea.AreaInitializer.UIFomulaManager.UpdateFromulas(calculatedPropertyTree.ChildItems, relationshipControl.Relationship);
        //        }
        //    }
        //}



        //اگر مستقیم نباشه فقط خود داده ها چک میشوند زیرا حضور دارند در تمپ ویو و در مخفی بودن یا ریدونلی بودن اثر داردند



        //AreaInitializer.UIFomulaManager.UpdateFromulas();
        //CheckEmptyOneDirectData(this);
        //if (!formulaResult)
        //{
        //    result.IsValid = false;
        //    result.Message = "نتیجه فرمولهای محاسبه شده مورد تایید نمی باشد";
        //}
        //else
        //{

        //var dataList = GetData();





        //}
        //}
        //باید اینجا شوال پرسده شود که اصلا کاربر میخواد با فرمولهای خطادار ادامه دهد



        //     return result;
        // }


        //public void ApplyStatesBeforeUpdate(DP_FormDataRepository data)
        //{
        //    var dataList = GetDataList().ToList();
        //    foreach (var data in dataList)
        //    {
        //        //if (shouldCheckChilds)
        //        //{

        //        //}
        //    }
        //    //if (parentChildRelInfo != null)
        //    //{
        //    //    foreach (var removedData in parentChildRelInfo.RemovedOriginalDatas)
        //    //        AreaInitializer.ActionActivityManager.CheckAndImposeEntityStates(removedData, ActionActivitySource.BeforeUpdate);
        //    //}
        //}
        //public void CheckEmptyOneDirectData(I_EditEntityArea editEntityArea)
        //{
        //    if (this is I_EditEntityAreaMultipleData)
        //        return;
        //    var dataList = GetDataList();

        //    if (editEntityArea != this)
        //    {
        //        foreach (var dataItem in dataList)
        //        {
        //            dataItem.IsEmptyOneDirectData = !AgentHelper.DataOrRelatedChildDataHasValue(dataItem, null);
        //        }
        //    }
        //    foreach (var relationshipControl in RelationshipColumnControls)
        //    {
        //        if (relationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
        //           || relationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //        {
        //            relationshipControl.GenericEditNdTypeArea.CheckEmptyOneDirectData(editEntityArea);
        //        }

        //    }
        //    //}
        //    //else
        //    //{
        //    //    foreach (var item in dataList)
        //    //        item.ShouldBeSkipped = true;

        //    //    //به نظر میاد اینجا بیخوده
        //    //    //foreach (var relationshipControl in RelationshipColumnControls)
        //    //    //{
        //    //    //    if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
        //    //    //       || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //    //    //    {
        //    //    //        relationshipControl.EditNdTypeArea.CheckRedundantData(editEntityArea);
        //    //    //    }

        //    //    //}
        //    //}
        //}



        //public bool DataIsNewInOneEditAreaAndReadonly(DP_FormDataRepository dataItem)
        //{
        //    //یعنی چون حالت مستقیم بوده مجبور بوده ایجاد بشه وگرنه نباید ایجاد میشده
        //    if (this is I_EditEntityAreaOneData && dataItem.IsNewItem &&
        //        (DataEntryEntity.IsReadonly
        //        || (SourceRelationColumnControl != null && SourceRelationColumnControl.Relationship.IsReadonly))
        //        )
        //        return true;
        //    else
        //        return false;

        //}

        //private bool AreaShouldBeReviewed()
        //{
        //    //if (this == editEntityArea)
        //    //    return true;
        //    var data = GetDataList();
        //    if (SourceRelationColumnControl != null)
        //    {
        //        if (SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
        //        {
        //            if (data.Any())
        //                return true;

        //        }
        //        else
        //        {

        //            if (SourceRelationColumnControl.Relationship.IsReadonly && data.Any(x => x.IsNewItem))
        //                return false;
        //            else
        //                return data.Any(x => AgentHelper.DataOrRelatedChildDataHasValue(x, SourceRelationColumnControl.Relationship));
        //        }
        //    }
        //    else
        //    {
        //        if (DataEntryEntity.IsReadonly && data.Any(x => x.IsNewItem))
        //            return false;

        //    }

        //    return false;

        //}




        //public void SetDataShouldBeCounted()
        //{
        //    List<DP_FormDataRepository> datalist = null;
        //    if (SourceRelationColumnControl == null)
        //    {
        //        datalist = AreaInitializer.Datas.ToList();
        //    }
        //    else
        //    {
        //        datalist = ChildRelationshipInfo.GetRelatedData(SourceRelationColumnControl.Relationship.ID);
        //    }
        //    foreach (var data in datalist)
        //    {
        //        SetDataShouldBeCounted(data);
        //    }
        //}
        //private void SetDataShouldBeCounted(DP_FormDataRepository data)
        //{
        //    data.HasDirectData = true;
        //    foreach (var relationshipControl in RelationshipColumnControls)
        //    {
        //        relationshipControl.EditNdTypeArea.SetChildRelationshipInfo(data.ChildRelationshipDatas.First(x => x.Relationship == relationshipControl.Relationship));
        //        if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
        //           || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //        {
        //            relationshipControl.EditNdTypeArea.SetDataShouldBeCounted();
        //        }
        //    }
        //}






        //public void RemoveDataBusinessMessage(DP_FormDataRepository dataItem, string key)
        //{
        //    RemoveDataItemMessage(dataItem, key);
        //    RemoveDataItemColor(dataItem, key);
        //}

        //public void RemoveMessageItem(BaseMessageItem baseMessageItem)
        //{
        //    MessageItems.Remove(baseMessageItem);
        //    if (baseMessageItem.ColumnControl != null)
        //    {
        //        if (baseMessageItem.ColumnControl is SimpleColumnControl)
        //        {
        //            (baseMessageItem.ColumnControl as SimpleColumnControl).RemoveMessage(baseMessageItem);
        //        }
        //        else if (baseMessageItem.ColumnControl is RelationshipColumnControl)
        //        {
        //            (baseMessageItem.ColumnControl as RelationshipColumnControl).RemoveMessage(baseMessageItem);
        //        }
        //    }
        //}
        //public void RemoveMessagesItems(string key)
        //{
        //    foreach (var baseMessageItem in MessageItems.Where(x => x.Key == key).ToList())
        //    {
        //        RemoveMessageItem(baseMessageItem);
        //    }
        //}
        //public void RemoveMessagesItems(DP_FormDataRepository dataItem, string key)
        //{
        //    foreach (var baseMessageItem in MessageItems.Where(x => x.CausingDataItem == dataItem && x.Key == key).ToList())
        //    {
        //        RemoveMessageItem(baseMessageItem);
        //    }
        //}

        //SimpleColumnControl LastFormulaColumnControl1 { set; get; }


        //public void TemporaryViewActionRequestedFromMultipleEditor(I_View_TemporaryView TemporaryView, TemporaryLinkType linkType, RelationshipDTO relationship, DP_FormDataRepository parentData)
        //{
        //    SetChildRelationshipInfo(parentData.ChildRelationshipDatas.First(x => x.Relationship == relationship));
        //    TemporaryViewActionRequestedInternal(TemporaryView, linkType);
        //}
        //public void ReadonlySimpleColumnControl(SimpleColumnControl column, bool readonlity)
        //{

        //}
        public void DisableEnableCommand(I_Command command, bool enabled)
        {
            command.CommandManager.SetEnabled(enabled);
        }
        //public void MakeFormReadOnly()
        //{
        //    List<I_Command> editCommands = Commands.Where(x => x is SaveCommand || x is SaveAndCloseDialogCommand || x is AddCommand).ToList();
        //    if (SourceRelationColumnControl == null)
        //    {

        //    }
        //    else
        //    {
        //        if (SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
        //        {
        //            var otherCommands = Commands.Where(x => x is ClearCommand || x is SearchCommand);
        //            editCommands.AddRange(otherCommands);
        //        }
        //        else
        //        {

        //        }
        //    }

        //    if (SourceRelationColumnControl != null)
        //    {
        //        var otherCommands = Commands.Where(x => x is ClearCommand || x is RemoveCommand);
        //        editCommands.AddRange(otherCommands);
        //    }

        //    foreach (var command in editCommands)
        //    {
        //        DisableEnableCommand(command, false);
        //    }

        //    foreach (var col in SimpleColumnControls)
        //    {
        //        ReadonlyDataItemColumn(col, true);
        //    }
        //    //foreach (var rel in RelationshipColumnControls.Where(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary))
        //    //{
        //    //    rel.EditNdTypeArea.AreaInitializer.SecurityReadOnlyByParent = true;
        //    //}
        //}
        //public void TemporaryViewSearchTextChanged(I_View_TemporaryView view, Arg_TemporaryDisplaySerachText searchArg)
        //{

        //}

        //private void ShowSearchResultInViewEntityArea(DP_SearchRepositoryMain searchItems)
        //{

        //    ShowSearchResultInViewEntityArea(searchItems);
        //}

        //private Task<DR_ResultSearchView> SearchAsync(DP_SearchRepositoryMain searchItems)
        //{
        //    return Task.Run(() =>
        //    {

        //        return result;
        //    });
        //}
        // public I_View_TemporaryView LastTemporaryView { set; get; }

        //private void TemporaryViewActionRequestedInternal(I_View_TemporaryView TemporaryView, TemporaryLinkType linkType)
        //{
        //    //if (LastTemporaryView != null)
        //    //{
        //    //    if (LastTemporaryView.HasPopupView)
        //    //        LastTemporaryView.RemovePopupView(ViewEntityArea.ViewForViewEntityArea);
        //    //}
        //    // LastTemporaryView = TemporaryView;

        //}
        //public void ShowSearchView(bool fromDataView)
        //{

        //    SearchViewEntityArea.ShowSearchView(fromDataView);
        //}

        private void ViewEntityArea_DataSelected(object sender, DataViewDataSelectedEventArg e)
        {
            //** BaseEditEntityArea.ViewEntityArea_DataSelected: a377695e6d06
            if (ViewForSearchAndView != null && ViewForSearchAndView.HasViewAreaView)
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(ViewForSearchAndView);

            SelectData(e.DataItem.Cast<DP_BaseData>().ToList());

        }
        public void SelectData(List<DP_BaseData> datas)
        {
            //** BaseEditEntityArea.SelectData: 1625660fbdfb
            //if (dataItems.Count > 1 && SourceRelationColumnControl != null)
            //{
            //    throw new Exception("asdasd");
            //}
            //اینجا منطقش عوض شه برای رابطه هم جدا بشه


            DP_SearchRepositoryMain searchItems = new DP_SearchRepositoryMain(AreaInitializer.EntityID);
            searchItems.AndOrType = AndOREqualType.Or;
            foreach (var item in datas)
            {
                LogicPhraseDTO logic = new LogicPhraseDTO();
                foreach (var col in item.Properties.Where(x => x.IsKey))
                    logic.Phrases.Add(new SearchProperty(col.ColumnID) { Value = col.Value });
                searchItems.Phrases.Add(logic);
            }



            List<DP_FormDataRepository> result = new List<DP_FormDataRepository>();

            bool isDirect = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
               AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);

            if (isDirect || DataView.IsOpenedTemporary)
            {

                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                var requestSearchEdit = new DR_SearchEditRequest(requester, searchItems);
                if (SourceRelationColumnControl != null)
                    requestSearchEdit.ToParentRelationshipID = ChildRelationshipInfoBinded.ToParentRelationshipID;
                var res = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchEditRequest(requestSearchEdit).ResultDataItems;
                if (datas.Count != res.Count)
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به برخی داده ها", "", Temp.InfoColor.Red);
                foreach (var dataItem in res)
                {
                    //   var froundItem = res[0];
                    //if (dataItem is DP_DataView)
                    //    froundItem.DataView = dataItem as DP_DataView;
                    result.Add(new DP_FormDataRepository(dataItem, this, false, false));
                }

                //}
            }
            else
            {

                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                DR_SearchViewRequest request = new DR_SearchViewRequest(requester, searchItems);
                if (SourceRelationColumnControl != null)
                    request.ToParentRelationshipID = ChildRelationshipInfoBinded.ToParentRelationshipID;
                //  request.CheckEntityStates = true;
                var dataViews = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(request).ResultDataItems;
                if (datas.Count != dataViews.Count)
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به برخی داده ها", "", Temp.InfoColor.Red);

                foreach (var dataItem in dataViews)
                    result.Add(new DP_FormDataRepository(dataItem, this, false, false));
            }

            if (SourceRelationColumnControl == null)
            {
                ClearData(true);
                foreach (var data in result)
                {
                    AddData(data);
                    //var addResult = AddData(data);
                    //if (!addResult)
                    //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", data.ViewInfo, Temp.InfoColor.Red);
                    //else
                    //{

                    //}
                }
                if (DataItemSelected != null)
                    DataItemSelected(this, result);
            }
            else
            {
                if (result.Count > 1)
                {
                    throw new Exception("asdasd");
                }
                var data = result.FirstOrDefault();
                if (data.ParentRelationshipIsHidenOnLoad)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("داده انتخاب شده به رابطه انتخاب شده دسترسی ندارد", "", Temp.InfoColor.Red);
                    return;
                }
                if (data.ParentRelationshipIsReadonlyOnLoad)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("رابطه برای داده انتخاب شده فقط خواندنی می باشد", "", Temp.InfoColor.Red);
                    return;
                }
                if (ChildRelationshipInfoBinded.RemoveAllRelatedData())
                {
                    if (data != null)
                        ChildRelationshipInfoBinded.AddDataToChildRelationshipData(data);
                }

                //   ChildRelationshipInfoBinded.DataSelected(data);

            }

            //   if (!addResult)
            // AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", result.ViewInfo, Temp.InfoColor.Red);
            //}
            //}
            //else
            //{
            //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", e.DataItem.First().ViewInfo, Temp.InfoColor.Red);
            //}
            //}
        }


        //public DP_FormDataRepository GetFullDataFromDataViewSearch(int entityID, DP_DataView searchViewData, I_EditEntityArea editEntityArea)
        //{

        //}

        //private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    bool isDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
        //    AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);

        //    //if (this is I_EditEntityAreaOneData)
        //    //{
        //    //    if (LastTemporaryView != null || TemporaryDisplayView != null)
        //    //    {
        //    //        if (LastTemporaryView != null)
        //    //        {
        //    //            LastTemporaryView.PopupVisibility = false;
        //    //        }
        //    //        else
        //    //        {
        //    //            TemporaryDisplayView.PopupVisibility = false;
        //    //        }
        //    //    }

        //    //    if (!isDataView)
        //    //    {
        //    //        SetTempText(AreaInitializer.Datas);

        //    //        bool eventOk = false;
        //    //        if (GetDataList().Count == 1 && !GetDataList().First().IsNewItem)
        //    //        {
        //    //            eventOk = true;
        //    //        }
        //    //        else if (GetDataList().Count == 0)
        //    //            eventOk = true;
        //    //        if (eventOk)
        //    //        {
        //    //            (this as I_EditEntityAreaOneData).OnDataItemSelected(GetDataList().FirstOrDefault());

        //    //        }
        //    //    }
        //    //}
        //    //else if (this is I_EditEntityAreaMultipleData)
        //    //{

        //    //}
        //    if (!isDataView)
        //        SetTempText(AreaInitializer.Datas);
        //    CheckRelationshipLabel();


        //    DecideButtonsEnablity1();
        //}







        public bool ClearData(bool dataRemoveOnly = false)
        {
            // BaseEditEntityArea.ClearData: 2750b5bf6116
            if (this.SourceRelationColumnControl != null)
                throw new Exception("asvvvb");
            //  var datas = GetDataList();

            foreach (var dataItem in GetDataList().ToList())
            {
                RemoveData(dataItem, dataRemoveOnly);
            }

            if (SourceRelationColumnControl == null)
            {
                if (DataItemsCleared != null)
                    DataItemsCleared(this, new EventArgs());
            }
            return true;
            //}
            //else
            //    return false;
        }

        public void RemoveData(DP_FormDataRepository dataItem, bool dataRemoveOnly = false)
        {
            // BaseEditEntityArea.RemoveData: e06d6fbdd00e
            GetDataList().Remove(dataItem);
            bool isDirect = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                  AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
            if (!dataRemoveOnly)
            {
                if (!isDirect)
                {
                    SetTempText();
                }
                else
                {
                    ClearUIData(dataItem);
                    if (GetDataList().Count == 0 && this is I_EditEntityAreaOneData)
                    {
                        (this as I_EditEntityAreaOneData).CreateDefaultData();
                    }
                }

                DecideButtonsEnablity1();
            }
            else
            {
                if (isDirect)
                    ClearUIData(dataItem);
            }
        }
        public void RemoveMultipleData(List<DP_FormDataRepository> datas)
        {
            // BaseEditEntityArea.RemoveMultipleData: 7b47193da8b7
            foreach (var item in datas)
                RemoveData(item);
        }


        public void ClearUIData(DP_FormDataRepository dataItem)
        {
            // BaseEditEntityArea.ClearUIData: c3bf5a88417e
            if (this is I_EditEntityAreaOneData)
            {

            }
            else if (this is I_EditEntityAreaMultipleData)
            {
                (this as I_EditEntityAreaMultipleData).RemoveDataContainer(dataItem);
            }
        }



        //public void SetRemovedItem(ChildRelationshipInfo childRelationshipInfo, DP_FormDataRepository item, bool delete)
        //{
        //    //if (childRelationshipInfo.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
        //    //{
        //    if (!childRelationshipInfo.ParentData.IsNewItem)
        //    {
        //        //RemoveRelationshipInfo removeChildRelationshipInfo;
        //        //if (!childRelationshipInfo.RemovedItems.Any(x => x.Relationship.ID == childRelationshipInfo.Relationship.ID))
        //        //{
        //        //removeChildRelationshipInfo = new RemoveRelationshipInfo();
        //        //if (delete)
        //        //    removeChildRelationshipInfo.RelationshipDeleteOption = RelationshipDeleteOption.DeleteCascade;
        //        //else
        //        //    removeChildRelationshipInfo.RelationshipDeleteOption = RelationshipDeleteOption.SetNull;
        //        if (delete && childRelationshipInfo.RelationshipDeleteOption != RelationshipDeleteOption.DeleteCascade)
        //            throw new Exception("cbcbcxvbcvb");
        //        childRelationshipInfo.RemovedItems.Add(item);
        //        //}
        //        //else
        //        //    removeChildRelationshipInfo = childRelationshipInfo.RemovedItems.First(x => x.Relationship.ID == childRelationshipInfo.Relationship.ID);
        //        // removeChildRelationshipInfo.RelatedData.Add(item);
        //    }
        //    else
        //        throw new Exception("xvxcvb");
        //    //}
        //}

        //public bool BaseAddData(DP_FormDataRepository data)
        //{

        //    //////else
        //    //////{
        //    //////    if (ChildRelationshipInfo != null)
        //    //////        ChildRelationshipInfo.AddDataToChildRelationshipInfo(data, false);
        //    //////    else
        //    //////        return false;
        //    //////}
        //    //OnDataItemLoaded(new EditAreaDataItemLoadedArg() { DataItem = data, InEditMode = true });
        //    return true;
        //}

        public void AddData(DP_FormDataRepository dataItem)
        {
            //  var result = true;
            if (this.SourceRelationColumnControl != null)
                throw new Exception("asvvvb");

            if (dataItem.IsNewItem && DataEntryEntity.IsReadonly && !dataItem.IsDefaultData)
                throw new Exception();

            //  if (dataItem.EntityListView == null)
            //      dataItem.EntityListView = ViewEntityArea.EntityListView;// DefaultEntityListViewDTO;

            //if (!dataItem.IsNewItem)
            //    AreaInitializer.ActionActivityManager.SetExistingDataFirstLoadStates(dataItem);

            AreaInitializer.Datas.Add(dataItem);

            bool isDirect = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
                          AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
            if (isDirect)
            {
                ShowDataInDataView(dataItem);
                //if (!ShowDataInDataView(dataItem))
                //{
                //    //نمایش ناموفق!!
                //    if (this is I_EditEntityAreaOneData)
                //    {
                //        ClearData();
                //    }
                //    result = false;
                //}
            }
            else
                SetTempText();

            AreaInitializer.ActionActivityManager.DataLoaded(dataItem);

            DecideButtonsEnablity1();

            //   return result;
        }
        public void ShowDataInDataView(DP_FormDataRepository specificDate)
        {
            // BaseEditEntityArea.ShowDataInDataView: 9f08862e5c0d
            bool result = true;
            if (!specificDate.IsFullData)
                throw new Exception("asdasd");
            if (this is I_EditEntityAreaMultipleData)
                (DataView as I_View_EditEntityAreaMultiple).AddDataContainer(specificDate);

            foreach (var propertyControl in specificDate.ChildSimpleContorlProperties)
            {
                propertyControl.SetBinding();
            }

            foreach (var childRelationshipInfo in specificDate.ChildRelationshipDatas)
            {
                childRelationshipInfo.SetBinding();

            }

            AreaInitializer.UIFomulaManager.DataToShowInDataview(specificDate);
            //if (result)
            //    OnDataItemShown(new EditAreaDataItemLoadedArg() { DataItem = specificDate, InEditMode = true });



            //  return result;

        }

        //   List<ChildRelationshipInfo> RegisteredParentDataItems = new List<ChildRelationshipInfo>();
        //public void SetChildRelationshipInfo(ChildRelationshipInfo value)
        //{

        //    //اگر قبلا لود شده بود دیگر لود نمیشود. به احتمال زیاد اگر پرنت هم فرم یک داده ای باشد قبلا به هنگام نمایش داده در پرنت این هم یکبار صدا زده شده و داده لود شده است
        //    if (value == null)
        //        throw new Exception("dxvxcv");
        //    ChildRelationshipInfo = value;
        //    if (!RegisteredParentDataItems.Contains(ChildRelationshipInfo))
        //    {
        //        //برای این است که کالکشن چنج تنها یکبار برای هر مجموعه داده صدا زده شود
        //        ChildRelationshipInfo.CollectionChanged += (sender1, e1) => NewValue_RelatedDataChanged(sender1, e1, ChildRelationshipInfo);
        //        RegisteredParentDataItems.Add(ChildRelationshipInfo);
        //    }

        //    //تنها در صورت نمایش پرنت این یکبار صدا زده میشود و در حالات دیگر به مانند صدا زدن به هنگام آپدیت یا ولیدشن کاربردی ندارد


        //}

        //////     public bool SetChildRelationshipInfoAndShow(ChildRelationshipInfo value)
        //////     {
        //////         bool result = true;
        //////         //if (this is I_EditEntityAreaOneData && !value.RelatedData.Any())
        //////         //{
        //////         //    ResetStatesColorAndText();
        //////         //}
        ////////         SetChildRelationshipInfo(value);

        //////         bool isDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
        //////      AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
        //////         if (isDataView)
        //////         {
        //////             if (this is I_EditEntityAreaOneData && !value.RelatedData.Any())
        //////                 (this as I_EditEntityAreaOneData).CreateDefaultData();
        //////             else
        //////             {
        //////                 if (this is I_EditEntityAreaMultipleData)
        //////                     (this as I_EditEntityAreaMultipleData).RemoveDataContainers();
        //////                 foreach (var data in value.RelatedData)
        //////                 {
        //////                     if (data.EntityListView == null)
        //////                         data.EntityListView = DefaultEntityListViewDTO;
        //////                     var dataViewResult = this.ShowDataInDataView(data);
        //////                     if (!dataViewResult)
        //////                         result = false;
        //////                 }
        //////             }

        //////         }
        //////         else
        //////         {
        //////             SetTempText(ChildRelationshipInfo.RelatedData);
        //////         }

        //////         CheckRelationshipLabel();
        //////         DecideButtonsEnablity1();

        //////         return result;
        //////     }

        //میتونه یه فنکشن کلی برای برسسی فعال بودن فرم بشه. مثلا وقتی وضعیتها هم بخوان دستکاری کنن فعال بودن فرم رو


        public I_View_TemporaryView TemporaryDisplayView { set; get; }
        //public void OnDataItemLoaded(EditAreaDataItemLoadedArg arg)
        //{
        //    if (DataItemLoaded != null)
        //        DataItemLoaded(this, arg);
        //}


        //اصلاح و بهتر شود برای تمپ ویوها
        //public void OnDataItemShown(EditAreaDataItemLoadedArg arg)
        //{

        //}
        //public void OnDataItemUnShown(EditAreaDataItemArg arg)
        //{
        //    if (DataItemUnShown != null)
        //        DataItemUnShown(this, arg);
        //}
        public void SetTempText()
        {
            // BaseEditEntityArea.SetTempText: f03945399e2a
            string text = "";
            //اینجا باید داده هایی که اسکیپ میشن رو در نظر نگیره
            if (AreaInitializer.Datas.Count > 1)
                text = "تعداد" + " " + AreaInitializer.Datas.Count + " " + "مورد";
            foreach (var item in AreaInitializer.Datas)
                text += (text == "" ? "" : Environment.NewLine) + item.ViewInfo;

            //if (SourceRelationColumnControl == null || SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaOneData)
            //{
            TemporaryDisplayView.SetLinkText(text);
            //if (string.IsNullOrEmpty(text))
            //{
            //    TemporaryDisplayView.ClearSearchText();
            //}
            //   if (TemporaryDisplayView.QuickSearchVisibility)
            //       TemporaryDisplayView.QuickSearchVisibility = false;
            //}
            //else
            //{
            //    var relationshipControl = SourceRelationColumnControl.ParentEditArea.RelationshipColumnControls.First(x => x.Relationship.ID == ChildRelationshipInfo.Relationship.ID) as RelationshipColumnControlMultiple;
            //    relationshipControl.RelationshipControlManager.GetView(ChildRelationshipInfo.SourceData).SetLinkText(text);
            //    //if (string.IsNullOrEmpty(text) && relationshipControl.EditNdTypeArea.TemporaryLinkState.searchView)
            //    //    relationshipControl.RelationshipControlManager.SetQuickSearchVisibility(ChildRelationshipInfo.SourceData, true);
            //    //else
            //    //    relationshipControl.RelationshipControlManager.SetQuickSearchVisibility(ChildRelationshipInfo.SourceData, false);
            //    relationshipControl.RelationshipControlManager.GetView(ChildRelationshipInfo.SourceData).QuickSearchVisibility = false;

            //}


            //  foreach (var dataItem in AreaInitializer.Datas)
            //        OnDataItemShown(new EditAreaDataItemLoadedArg() { DataItem = dataItem, InEditMode = false });

        }

        //private void NewValue_RelatedDataChanged(object sender, NotifyCollectionChangedEventArgs e, ChildRelationshipInfo childRelationshipInfo)
        //{

        //}

        //public void DataCollectionChanged(ObservableCollection<DP_FormDataRepository> dataList, NotifyCollectionChangedEventArgs e)
        //{
        //    //bool result = true;

        //}

        private void CheckRelationshipLabel()
        {
            //////if (SourceRelationColumnControl != null)
            //////{

            //////    if (SourceRelationColumnControl.SourceRelationshipColumnControl.RelationshipControlManager.TabPageContainer != null ||
            //////                SourceRelationColumnControl.SourceRelationshipColumnControl.RelationshipControlManager.HasExpander)
            //////    {
            //////        SourceRelationColumnControl.SourceRelationshipColumnControl.RemoveLabelControlManagerColorByKey("hasdata");
            //////        bool color = false;
            //////        if (this is I_EditEntityAreaMultipleData)
            //////        {
            //////            if (GetDataList().Count() != 0)
            //////                color = true;
            //////        }
            //////        else if (this is I_EditEntityAreaOneData)
            //////        {
            //////            if (SourceRelationColumnControl.Relationship.IsOtherSideDirectlyCreatable)
            //////            {
            //////                if (GetDataList().Count(x => !x.IsNewItem) != 0)
            //////                    color = true;
            //////            }
            //////            else
            //////            {
            //////                if (GetDataList().Count() != 0)
            //////                    color = true;
            //////            }
            //////        }
            //////        if (color)
            //////            SourceRelationColumnControl.SourceRelationshipColumnControl.AddLabelControlManagerColor(new BaseColorItem() { Key = "hasdata", Color = InfoColor.LightGray, ColorTarget = ControlColorTarget.Background });
            //////    }
            //////}
        }

        public ObservableCollection<FunctionResult> FormulaExceptionReulsts
        {
            set; get;
        }

        public List<UIActionActivityDTO> RunningActionActivities
        {
            set; get;
        }
        public ChildRelationshipInfo ChildRelationshipInfoBinded { get; set; }

        //  public List<EntityStateGroupDTO> EntityStateGroups { set; get; }




















        //private List<Tuple<I_DataControlManager, DP_FormDataRepository>> GetControlDataManagers()
        //{
        //    List<Tuple<I_DataControlManager, DP_FormDataRepository>> result = new List<Tuple<I_DataControlManager, DP_FormDataRepository>>();
        //    bool hasTempView = (AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
        //AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
        // AreaInitializer.IntracionMode == IntracionMode.Select);

        //    if (DataView != null)
        //    {
        //        result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>(DataView, null));
        //    }
        //    if (SourceRelationColumnControl == null || SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaOneData)
        //    {
        //        if (TemporaryDisplayView != null)
        //            result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>(TemporaryDisplayView, null));
        //    }
        //    else if (ChildRelationshipInfo != null)
        //    {
        //        var relationshipControl = SourceRelationColumnControl;
        //        result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>(relationshipControl.RelationshipControlManager, ChildRelationshipInfo.SourceData));
        //    }

        //    return result;
        //}
        //private List<Tuple<I_DataControlManager, DP_FormDataRepository>> GetColumnControlDataManagers(BaseColumnControl columnControl, ControlOrLabelAsTarget controlOrLabelAsTarget, DP_FormDataRepository dataItem)
        //{
        //    List<Tuple<I_DataControlManager, DP_FormDataRepository>> result = new List<Tuple<I_DataControlManager, DP_FormDataRepository>>();
        //    if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
        //    {
        //        if (columnControl is SimpleColumnControl)
        //        {
        //            result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>((columnControl as SimpleColumnControl).ControlManager, dataItem));
        //        }
        //        else if (columnControl is RelationshipColumnControl)
        //        {
        //            var relationshipControl = (columnControl as RelationshipColumnControl);
        //            result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>((columnControl as RelationshipColumnControl).ControlManager, dataItem));

        //            bool hasTempView = (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
        //  relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
        //   relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.Select);
        //            if (hasTempView)
        //            {
        //                if (relationshipControl.EditNdTypeArea.DataView != null)
        //                    if (relationshipControl.EditNdTypeArea.DataView.IsOpenedTemporary)
        //                        result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>(relationshipControl.EditNdTypeArea.DataView, null));
        //            }

        //        }
        //    }
        //    else
        //    {
        //        if (columnControl is SimpleColumnControl)
        //        {
        //            result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>((columnControl as SimpleColumnControl).ControlManager.LabelControlManager, null));
        //        }
        //        else if (columnControl is RelationshipColumnControl)
        //        {
        //            result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>((columnControl as RelationshipColumnControl).ControlManager.LabelControlManager, null));
        //        }
        //    }
        //    return result;
        //}
        //private InfoColor GetColor(List<BaseColorItem> list)
        //{
        //    var color = InfoColor.Null;
        //    foreach (var item in list.Where(x => x.Color != InfoColor.Null).OrderByDescending(x => x.Priority))
        //        color = item.Color;
        //    return color;
        //}
















        //public void DecideDataViewStaticButtons()
        //{

        //}
        public void DecideButtonsEnablity1()
        {
            // BaseEditEntityArea.DecideButtonsEnablity1: 195f736ab331
            if (SourceRelationColumnControl != null)
                throw new Exception("asdsdf");


            //  var dataEntryEntityIsReadonly = DataEntryEntity.IsReadonly;

            //var sourceRelationshipIsReadonly = false;// ChildRelationshipInfo != null && ChildRelationshipInfo.IsReadonly;
            var dataList = GetDataList();

            bool saveCommandEnablity = true;
            var saveCommand = GetCommand(typeof(SaveCommand));
            if (saveCommand != null)
            {
                if (!dataList.Any() || (DataEntryEntity.IsReadonly && dataList.Any(x => x.IsNewItem)))
                    saveCommandEnablity = false;

                saveCommand.CommandManager.SetEnabled(saveCommandEnablity);
            }

            var hasNotDeleteAccess = DataEntryEntity.HasNotDeleteAccess;
            bool deleteCommandEnablity = true;
            var deleteCommand = GetCommand(typeof(DeleteCommand));
            if (deleteCommand != null)
            {
                if (hasNotDeleteAccess || !dataList.Any())// || dataList.All(x => x.IsReadonlyOnState))
                    deleteCommandEnablity = false;
                deleteCommand.CommandManager.SetEnabled(deleteCommandEnablity);
            }

            bool clearCommandEnablity = true;
            bool searchCommandEnablity = true;
            bool addCommandEnablity = true;
            bool removeCommandEnablity = true;
            if (SourceRelationColumnControl != null)
            {
                if (ChildRelationshipInfoBinded.IsReadonly || !ChildRelationshipInfoBinded.RelatedData.Any())
                    clearCommandEnablity = false;


                if (ChildRelationshipInfoBinded.IsReadonly)// || (RelatedData.Any() && RelatedData.All(x => x.IsDBRelationship && (x.ToParentRelationshipIsReadonly || x.ToParentRelationshipIsHidden))))
                    searchCommandEnablity = false;

                if (ChildRelationshipInfoBinded.IsDataviewOpen)
                {
                    if (DataEntryEntity.IsReadonly || ChildRelationshipInfoBinded.IsReadonly)
                        addCommandEnablity = false;
                    else
                        addCommandEnablity = true;

                    if (DataEntryEntity.IsReadonly || !ChildRelationshipInfoBinded.RelatedData.Any())//|| RelatedData.All(x => x.IsDBRelationship && (x.ToParentRelationshipIsReadonly || x.ToParentRelationshipIsHidden)))
                    {
                        // باید رو ریمو خود داده کنترل شودIsDBRelationship ها
                        removeCommandEnablity = false;
                    }
                }
                if (!ChildRelationshipInfoBinded.IsDirect)
                {
                    var tempView = ChildRelationshipInfoBinded.GetTempView;

                    tempView.ButtonClearEnabled = clearCommandEnablity;
                    tempView.ButtonSearchFormEnabled = searchCommandEnablity;
                    tempView.ButtonPopupEnabled = searchCommandEnablity;
                    tempView.InfoTextboxReadOnly = !searchCommandEnablity;

                    //bool quickSearchEnablity = true;
                    //if (!searchCommandEnablity)

                    //else
                    //    quickSearchEnablity = RelationshipControl.GenericEditNdTypeArea.TemporaryDisplayView.QuickSearchVisibility;
                    //tempView.DisableEnable(TemporaryLinkType.QuickSearch, quickSearchEnablity);

                    bool dataViewEnablity = true;
                    if ((DataEntryEntity.IsReadonly || ChildRelationshipInfoBinded.IsReadonly) && !ChildRelationshipInfoBinded.RelatedData.Any())
                        dataViewEnablity = false;
                    tempView.ButtonDataEditEnabled = dataViewEnablity;
                }
            }
            else
            {
                if (!dataList.Any())
                {
                    clearCommandEnablity = false;
                }

                if (DataEntryEntity.IsReadonly)
                    addCommandEnablity = false;
                else
                    addCommandEnablity = true;


                if (!dataList.Any())
                {
                    removeCommandEnablity = false;
                }
            }

            var clearCommand = GetCommand(typeof(ClearCommand));
            if (clearCommand != null)
                clearCommand.CommandManager.SetEnabled(clearCommandEnablity);

            var searchCommand = SourceRelationColumnControl.GenericEditNdTypeArea.GetCommand(typeof(SearchCommand));
            if (searchCommand != null)
                searchCommand.CommandManager.SetEnabled(searchCommandEnablity);


            var addCommand = GetCommand(typeof(AddCommand));
            if (addCommand != null)
            {
                addCommand.CommandManager.SetEnabled(addCommandEnablity);
            }

            var removeCommand = GetCommand(typeof(RemoveCommand));
            if (removeCommand != null)
            {
                removeCommand.CommandManager.SetEnabled(removeCommandEnablity);
            }
            bool enableDisableDataSection = true;
            if (GetDataList().Any(x => x.IsUseLessBecauseNewAndReadonly))
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("موجودیت فقط خواندنی می باشد و امکان ثبت داده وجود ندارد", "", Temp.InfoColor.Red);
                enableDisableDataSection = false;
            }
            if (DataView != null)
                DataView.EnableDisableDataSection(enableDisableDataSection);
            //if (TemporaryDisplayView != null && SourceRelationColumnControl != null)
            //{

            //    bool quickSearchEnablity = true;


            //    if (sourceRelationshipIsReadonly)
            //        quickSearchEnablity = false;
            //    else
            //        quickSearchEnablity = TemporaryLinkState.quickSearch;


            //    bool dataViewEnablity = true;
            //    if (sourceRelationshipIsReadonly)
            //        dataViewEnablity = dataList.Count != 0;


            //    TemporaryLinkState.clear = clearCommandEnablity;
            //    TemporaryLinkState.searchView = searchCommandEnablity;
            //    TemporaryLinkState.popup = searchCommandEnablity;
            //    TemporaryLinkState.quickSearch = quickSearchEnablity;
            //    TemporaryLinkState.edit = dataViewEnablity;
            //    TemporaryDisplayView.DisableEnable(TemporaryLinkType.Clear, clearCommandEnablity);
            //    TemporaryDisplayView.DisableEnable(TemporaryLinkType.SerachView, searchCommandEnablity);
            //    TemporaryDisplayView.DisableEnable(TemporaryLinkType.Popup, searchCommandEnablity);
            //    TemporaryDisplayView.DisableEnable(TemporaryLinkType.QuickSearch, quickSearchEnablity);
            //    TemporaryDisplayView.DisableEnable(TemporaryLinkType.DataView, dataViewEnablity);
            //    if (SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaMultipleData)
            //    {
            //        //var relationshipControl = (SourceRelationColumnControl.SourceEditArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.First(x => x.Relationship.ID == ChildRelationshipInfo.Relationship.ID);
            //        //relationshipControl.RelationshipControlManager.EnableDisable(ChildRelationshipInfo.SourceData, TemporaryLinkType.DataView, allowdataview);
            //        //////var parentRelationshipControl = SourceRelationColumnControl;
            //        //////var parentData = ChildRelationshipInfo.SourceData;


            //        //////(parentRelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(parentData).DisableEnable(TemporaryLinkType.Clear, clearCommandEnablity);
            //        //////(parentRelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(parentData).DisableEnable(TemporaryLinkType.SerachView, searchCommandEnablity);
            //        //////(parentRelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(parentData).DisableEnable(TemporaryLinkType.Popup, searchCommandEnablity);


            //        //////(parentRelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(parentData).DisableEnable(TemporaryLinkType.QuickSearch, quickSearchEnablity);
            //        //////(parentRelationshipControl as RelationshipColumnControlMultiple).RelationshipControlManager.GetView(parentData).DisableEnable(TemporaryLinkType.DataView, dataViewEnablity);


            //        //if (isReadonly)
            //        //{
            //        //    parentRelationshipControl.RelationshipControlManager.EnableDisable(parentData, TemporaryLinkType.QuickSearch, false);
            //        //    parentRelationshipControl.RelationshipControlManager.SetQuickSearchVisibility(parentData, dataList.Count != 0);

            //        //    if (dataList.Count == 0)
            //        //    {
            //        //        allowdataview = false;
            //        //    }
            //        //}
            //        //else
            //        //{
            //        //    parentRelationshipControl.RelationshipControlManager.EnableDisable(parentData, TemporaryLinkType.QuickSearch, TemporaryLinkState.quickSearch);
            //        //    parentRelationshipControl.RelationshipControlManager.SetQuickSearchVisibility(parentData, false);
            //        //}
            //        //parentRelationshipControl.RelationshipControlManager.EnableDisable(parentData, TemporaryLinkType.DataView, allowdataview);

            //    }
            //}



            ////if (SourceRelationColumnControl != null)
            ////{
            ////    if (!isDataView)
            ////    {
            ////        bool allowdataview = true;
            ////        if (dataList.Count == 0)
            ////        {
            ////            allowdataview = false;
            ////        }
            ////        if (SourceRelationColumnControl.Relationship.IsReadonly)
            ////        {
            ////            if (SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaOneData)
            ////            {
            ////                TemporaryDisplayView.DisableEnable(TemporaryLinkType.DataView, allowdataview);
            ////                TemporaryLinkState.edit = allowdataview;
            ////            }
            ////            else
            ////            {

            ////                var relationshipControl = (SourceRelationColumnControl.ParentEditArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.First(x => x.Relationship.ID == ChildRelationshipInfo.Relationship.ID);
            ////                relationshipControl.RelationshipControlManager.EnableDisable(ChildRelationshipInfo.SourceData, TemporaryLinkType.DataView, allowdataview);
            ////            }
            ////        }

            ////    }


            ////}

            ////      if (SourceRelationColumnControl == null || SourceRelationColumnControl.Relationship.IsReadonly || ChildRelationshipInfo == null)
            ////          return;

            ////      bool isDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
            ////AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);

            ////if (isDataView || (DataView != null && DataView.IsOpenedTemporary))
            ////{


            ////    var searchCommand = GetCommand(typeof(SearchCommand));
            ////    if (searchCommand != null)


            ////    var clearCommand = GetCommand(typeof(ClearCommand));
            ////    if (clearCommand != null)
            ////        clearCommand.CommandManager.SetEnabled(enabled);
            ////}

            ////if (!isDataView)
            ////{

            ////}

        }


        //public void DecideTempViewStaticButtons()
        //{
        //    if (SourceRelationColumnControl != null)
        //    {
        //        if (SourceRelationColumnControl.Relationship.IsReadonly)
        //        {
        //            if (TemporaryDisplayView != null)
        //            {
        //                TemporaryDisplayView.DisableEnable(TemporaryLinkType.Clear, false);
        //                TemporaryDisplayView.DisableEnable(TemporaryLinkType.SerachView, false);
        //                TemporaryDisplayView.DisableEnable(TemporaryLinkType.Popup, false);
        //                TemporaryDisplayView.DisableEnable(TemporaryLinkType.QuickSearch, false);
        //                TemporaryDisplayView.QuickSearchVisibility = false;
        //            }
        //            TemporaryLinkState.clear = false;
        //            TemporaryLinkState.searchView = false;
        //            TemporaryLinkState.popup = false;
        //            TemporaryLinkState.quickSearch = false;
        //        }
        //    }
        //}


        public I_Command GetCommand(Type type)
        {
            return Commands.FirstOrDefault(x => x.GetType() == type);
        }



        //public bool DataItemIsDBRelationshipAndRemoved(DP_FormDataRepository dataItem)
        //{
        //    if (dataItem.ParantChildRelationshipInfo == ChildRelationshipInfo)
        //    {
        //        if (ChildRelationshipInfo.DataItemIsRemoved(dataItem))
        //            return true;
        //    }
        //    return false;
        //}
        //
        //{

        //}
        //private void SetDataItemRelationshipColumnValue(DP_FormDataRepository dataItem, RelationshipColumnControl relationshipColumnControl, Dictionary<int, string> values
        //    , string key, string title)
        //{

        //}

        //private void SetDataItemSimplePropertyValue(DP_FormDataRepository dataItem, SimpleColumnControl simpleColumnControl, string value, string key, string title)
        //{

        //}
        //private void SetPropertyValue(DP_FormDataRepository dataItem, int columnID, string value, string key, string title)
        //{
        //    var property = dataItem.GetProperty(columnID);
        //    if (!property.IsReadonly && !property.IsReadonly && !property.IsHidden)
        //    {      //اینجا باید بررسی بشه که نوع مقدار و پراپرتی مناسب هستند
        //        property.Value = value;
        //    }
        //}

        //public void SetColumnValueRange(SimpleColumnControlGenerel propertyControl, List<ColumnValueRangeDetailsDTO> details)
        //{
        //    propertyControl.SimpleControlManagerGeneral.SetColumnValueRange(details);
        //}








        //public void ChangeRelatoinsipColumnUnReadonlyFromState(ChildRelationshipInfo childRelationshipInfo, DP_FormDataRepository dataItem, RelationshipColumnControl relationshipControl, string message, string key)
        //{
        //    if (DataItemIsInEditMode(dataItem))
        //    {
        //        if (!childRelationshipInfo.Relationship.IsReadonly)
        //        {
        //            childRelationshipInfo.IsReadonly = false;
        //            relationshipControl.EditNdTypeArea.DecideButtonsReadonlityByState(false);


        //            //foreach (var relCol in childRelationshipInfo.Relationship.RelationshipColumns)
        //            //{
        //            //    var fkProp = dataItem.GetProperty(childRelationshipInfo.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary ? relCol.FirstSideColumnID : relCol.SecondSideColumnID);
        //            //    fkProp.IsReadonlyFromState = false;
        //            //}
        //        }

        //    }
        //}










        //private void ResetStatesColorAndText()
        //{
        //    //بهتر شه و عمومی نوشته بشه یعنی به دیتا کاری نداشته باشه و کلا پاک بشن ظاهر ها . بررسی شود
        //    if (this is I_EditEntityAreaOneData)
        //    {
        //        //برای وقتی که داده وضعیت دار رابطه بوجود آورنده خودش رو رنگی میکنه و در حال تمپ هم هست
        //        //چون وقتی کلیر میکنه داده ای دیگر موجود نیست که رنگ را به حالت اول برگرداند
        //        //اگر تو حالت دیتا ویو باشد چون خود داده وضعیتها را ریست میکند مشکلی پیش نمی آید
        //        //DataItemColorItems.Clear();
        //        //DataItemMessageItems.Clear();
        //        //ColumnControlColorItems.Clear();
        //        //ColumnControlMessageItems.Clear();
        //        var controlManagers = GetControlDataManagers();



        //        foreach (var view in controlManagers)
        //        {
        //            view.Item1.SetTooltip(view.Item2, "");
        //            view.Item1.SetBackgroundColor(view.Item2, InfoColor.Null);
        //            view.Item1.SetBorderColor(view.Item2, InfoColor.Null);
        //        }
        //    }
        //    //foreach (var item in SimpleColumnControls)
        //    //{
        //    //    SetItemColor(lastOneData, ControlColorTarget.Background, item, ControlOrLabelAsTarget.Label);
        //    //    SetItemColor(lastOneData, ControlColorTarget.Border, item, ControlOrLabelAsTarget.Label);
        //    //    SetItemColor(lastOneData, ControlColorTarget.Background, item, ControlOrLabelAsTarget.Control);
        //    //    SetItemColor(lastOneData, ControlColorTarget.Border, item, ControlOrLabelAsTarget.Control);
        //    //    SetItemMessage(lastOneData, item, ControlOrLabelAsTarget.Label);
        //    //    SetItemMessage(lastOneData, item, ControlOrLabelAsTarget.Control);
        //    //}
        //    //foreach (var item in RelationshipColumnControls)
        //    //{
        //    //    SetItemColor(lastOneData, ControlColorTarget.Background, item, ControlOrLabelAsTarget.Label);
        //    //    SetItemColor(lastOneData, ControlColorTarget.Border, item, ControlOrLabelAsTarget.Label);
        //    //    SetItemColor(lastOneData, ControlColorTarget.Background, item, ControlOrLabelAsTarget.Control);
        //    //    SetItemColor(lastOneData, ControlColorTarget.Border, item, ControlOrLabelAsTarget.Control);
        //    //    SetItemMessage(lastOneData, item, ControlOrLabelAsTarget.Label);
        //    //    SetItemMessage(lastOneData, item, ControlOrLabelAsTarget.Control);
        //    //}
        //}


    }
    public class BaseColumnControl
    {
        protected I_UIControlManager _LabelControlManager;
        public I_UIControlManager LabelControlManager { get { return _LabelControlManager; } }

        public BaseColumnControl()
        {
        }
        public SecurityAction Permission { get; set; }
        public EntityUICompositionDTO UICompositionDTO { set; get; }

    }
    public abstract class SimpleColumnControlGenerel : BaseColumnControl
    {
        public ColumnDTO DataEntryColumn { set; get; }
    }



    public class SimpleColumnControlOne : SimpleColumnControlGenerel
    {

        // SimpleColumnControlOne: 0c72888906d5 
        public I_SimpleControlManagerOne SimpleControlManager { get; }
        public SimpleColumnControlOne(IAgentUIManager uiManager, EntityUICompositionDTO uiCompositionItem)
        {
            DataEntryColumn = uiCompositionItem.Column;
            _LabelControlManager = uiManager.GenerateLabelControlManager(DataEntryColumn.Alias);

            SimpleControlManager = uiManager.GenerateSimpleControlManagerForOneDataForm(DataEntryColumn, uiCompositionItem.ColumnUISetting, null);
        }
        //public override I_SimpleControlManagerGeneral SimpleControlManagerGeneral
        //{
        //    get { return SimpleControlManager; }
        //}

    }
    public class SimpleColumnControlMultiple : SimpleColumnControlGenerel
    {
        // SimpleColumnControlMultiple: 0616c9171488
        public I_SimpleControlManagerMultiple SimpleControlManager { get; }
        public SimpleColumnControlMultiple(IAgentUIManager uiManager, EntityUICompositionDTO uiCompositionItem)
        {
            DataEntryColumn = uiCompositionItem.Column;
            _LabelControlManager = uiManager.GenerateLabelControlManager(DataEntryColumn.Alias);
            SimpleControlManager = uiManager.GenerateSimpleControlManagerForMultipleDataForm(DataEntryColumn, uiCompositionItem.ColumnUISetting);

        }
        //public override I_SimpleControlManagerGeneral SimpleControlManagerGeneral
        //{
        //    get { return SimpleControlManager; }
        //}

    }

    public abstract class RelationshipColumnControlGeneral : BaseColumnControl
    {
        //public event EventHandler<ChildRelationshipInfo> DataViewForTemporaryViewShown;
        public I_EditEntityArea ParentEditArea { set; get; }
        public RelationshipDTO Relationship { get { return DataEntryRelationship.Relationship; } }
        //    public List<ColumnDTO> RelationshipColumns { set; get; }
        public DataEntryRelationshipDTO DataEntryRelationship { set; get; }
        public I_EditEntityArea GenericEditNdTypeArea { set; get; }

    }
    public class RelationshipColumnControlOne : RelationshipColumnControlGeneral
    {
        public I_RelationshipControlManagerOne RelationshipControlManager
        {
            set; get;
        }
        public RelationshipColumnControlOne(IAgentUIManager uiManager, EntityUICompositionDTO uiCompositionItem, I_EditEntityArea parentEditArea, I_EditEntityArea relationshipEditArea)
        {
            //RelationshipColumnControlOne: 7a0bc2462763
            relationshipEditArea.SourceRelationColumnControl = this;
            GenericEditNdTypeArea = relationshipEditArea;
            DataEntryRelationship = uiCompositionItem.Relationship;
            RelationshipControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateRelControlManagerForOneDataForm(relationshipEditArea.FirstView
                     , uiCompositionItem.RelationshipUISetting);


            _LabelControlManager = uiManager.GenerateLabelControlManager(uiCompositionItem.Relationship.Relationship.Alias);
            ParentEditArea = parentEditArea;

        }
    }
    public class RelationshipColumnControlMultiple : RelationshipColumnControlGeneral
    {
        //RelationshipColumnControlMultiple: c0b7ea24c7e9
        public RelationshipColumnControlMultiple(IAgentUIManager uiManager, EntityUICompositionDTO uiCompositionItem, I_EditEntityArea parentEditArea, I_EditEntityArea relationshipEditArea)
        {
            relationshipEditArea.SourceRelationColumnControl = this;
            GenericEditNdTypeArea = relationshipEditArea;
            DataEntryRelationship = uiCompositionItem.Relationship;
            RelationshipControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateRelControlManagerForMultiDataForm(relationshipEditArea.TemporaryDisplayView
                       , uiCompositionItem.RelationshipUISetting);
            _LabelControlManager = uiManager.GenerateLabelControlManager(uiCompositionItem.Relationship.Relationship.Alias);
            ParentEditArea = parentEditArea;
        }
        public I_RelationshipControlManagerMultiple RelationshipControlManager
        {
            set; get;
        }
    }
    public class RelationshipSearchColumnControl : BaseColumnControl
    {
        public RelationshipSearchColumnControl(IAgentUIManager uiManager, I_EditEntityArea editArea, EntitySearchColumnsDTO entitySearchColumn)
        {
            EntitySearchColumn = entitySearchColumn;
            EditNdTypeArea = editArea;
            ControlManager = uiManager.GenerateRelControlManagerForOneDataForm(EditNdTypeArea.TemporaryDisplayView, EntitySearchColumn.RelationshipUISetting);
            _LabelControlManager = uiManager.GenerateLabelControlManager(EntitySearchColumn.Alias);

        }

        public I_EditEntityArea EditNdTypeArea { set; get; }
        //public RelationshipDTO Relationship { get {return EntitySearchColumn.rel } }
        public List<ColumnDTO> Columns { set; get; }
        //public event EventHandler<ColumnValueChangeArg> ValueChanged;

        public EntitySearchColumnsDTO EntitySearchColumn { get; set; }

        public I_RelationshipControlManagerOne ControlManager { get; set; }
        public EntityRelationshipTailDTO RelationshipTail
        {
            get
            {
                return EntitySearchColumn.RelationshipTail;
            }
        }

        //public bool IsFake { set; get; }
        //public SearchEnumerableType SearchEnumerableType { get; set; }

        //public List<AG_RelatedConttol> RelatedUIControls { set; get; }
    }

    public enum DataSearchAction
    {
        JustShowInViewEntityArea,
        AddDataToDataForm,
        AddDataToTempView

    }
}
