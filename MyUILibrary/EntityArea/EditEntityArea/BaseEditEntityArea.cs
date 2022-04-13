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
        public List<UIControlPackageTree> UIControlPackageTree { set; get; }

        public event EventHandler<EditAreaGeneratedArg> RelationshipAreaGenerated;
        //public event EventHandler<EditAreaDataItemLoadedArg> DataItemLoaded;
        public event EventHandler<EditAreaDataItemLoadedArg> DataItemShown;

        public event EventHandler<DisableEnableChangedArg> DisableEnableChanged;
        public event EventHandler DataViewGenerated;
        public event EventHandler UIGenerated;

        public abstract void DataItemVisiblity(object data, bool visiblity);
        public abstract void DataItemEnablity(object data, bool visiblity);
        public abstract bool AddData(DP_FormDataRepository data, bool showDataInDataView);
        public bool AddData(DP_DataRepository data, bool showDataInDataView)
        {
            //Mapper.Initialize(cfg => cfg.CreateMap<DP_DataRepository, DP_FormDataRepository>());
            //var result = AutoMapper.Mapper.Map<DP_DataRepository, DP_FormDataRepository>(data);

            DP_FormDataRepository formData = new DP_FormDataRepository(data, this);
            return AddData(formData, showDataInDataView);
        }
        public abstract void GenerateUIComposition(List<EntityUICompositionDTO> UICompositions);

        public abstract bool ShowDataInDataView(DP_FormDataRepository dataItem);
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
        ChildRelationshipInfo _ChildRelationshipInfo;
        public ChildRelationshipInfo ChildRelationshipInfo
        {
            get
            {
                return _ChildRelationshipInfo;
            }
            set
            {
                _ChildRelationshipInfo = value;
            }
        }
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
        EntityListViewDTO _DefaultEntityListViewDTO;
        public EntityListViewDTO DefaultEntityListViewDTO
        {
            get
            {
                if (_DefaultEntityListViewDTO == null)
                    _DefaultEntityListViewDTO = AgentUICoreMediator.GetAgentUICoreMediator.EntityListViewManager.GetDefaultEntityListView(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                return _DefaultEntityListViewDTO;
            }
        }




        TableDrivedEntityDTO _EntityWithSimpleColumns;
        public TableDrivedEntityDTO EntityWithSimpleColumns
        {
            get
            {
                if (_EntityWithSimpleColumns == null)
                {
                    if (FullEntity != null)
                        return FullEntity;
                    else
                        _EntityWithSimpleColumns = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetEntityWithSimpleColumns(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                }
                return _EntityWithSimpleColumns;
            }
        }

        List<EntityStateDTO> _EntityStates;
        public List<EntityStateDTO> EntityStates1
        {
            get
            {
                if (_EntityStates == null)
                {
                    _EntityStates = AgentUICoreMediator.GetAgentUICoreMediator.entityStateManagerService.GetEntityStates(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);

                }
                return _EntityStates;
            }
        }
        I_SearchViewEntityArea _SearchViewEntityArea;
        public I_SearchViewEntityArea SearchViewEntityArea
        {

            get
            {
                if (_SearchViewEntityArea == null)
                    _SearchViewEntityArea = GenerateSearchViewArea();
                return _SearchViewEntityArea;
            }
        }

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

        DataEntryEntityDTO _DataEntryEntity;
        public DataEntryEntityDTO DataEntryEntity
        {
            get
            {
                if (_DataEntryEntity == null)
                {
                    if (AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.DataEntryRelationship.TargetDataEntryEntity != null)
                        return AreaInitializer.SourceRelationColumnControl.DataEntryRelationship.TargetDataEntryEntity;
                    else
                        _DataEntryEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetDataEntryEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID,
                            AreaInitializer.SourceRelationColumnControl?.DataEntryRelationship);
                }
                return _DataEntryEntity;
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
        bool UICompositionsCalled;
        EntityUICompositionCompositeDTO _UICompositions;
        public EntityUICompositionCompositeDTO UICompositions
        {
            get
            {
                if (_UICompositions == null && !UICompositionsCalled)
                {
                    UICompositionsCalled = true;
                    //if (AreaInitializer.Preview)
                    //    _UICompositions = AreaInitializer.PreviewUICompositionItems;
                    //else
                    _UICompositions = AgentUICoreMediator.GetAgentUICoreMediator.entityUICompositionService.GetEntityUICompositionTree(AreaInitializer.EntityID);
                }
                return _UICompositions;
            }
            set
            {
                _UICompositions = value;
            }
        }
        public RelationshipUISettingDTO GetRelationshipUISetting(RelationshipDTO relationship, bool isTemporaryView)
        {
            RelationshipUISettingDTO setting;
            if (UICompositions != null && UICompositions.RelationshipItems != null
            && UICompositions.RelationshipItems.Any(x => x.RelationshipID == relationship.ID))
            {
                setting = UICompositions.RelationshipItems.First(x => x.RelationshipID == relationship.ID);
            }
            else
            {
                setting = new RelationshipUISettingDTO();
                if (isTemporaryView)
                {
                    setting.UIColumnsType = Enum_UIColumnsType.Normal;
                }
                else
                {
                    setting.Expander = true;
                    setting.UIColumnsType = Enum_UIColumnsType.Full;
                }
                UICompositions.RelationshipItems.Add(setting);
            }
            return setting;
        }

        private ColumnUISettingDTO GetColumnUISetting(ColumnDTO column)
        {
            ColumnUISettingDTO setting;
            if (UICompositions != null && UICompositions.ColumnItems != null
                && UICompositions.ColumnItems.Any(x => x.ColumnID == column.ID))
            {
                setting = UICompositions.ColumnItems.First(x => x.ColumnID == column.ID);
            }
            else
            {
                setting = new ColumnUISettingDTO();
                setting.UIColumnsType = Enum_UIColumnsType.Normal;
                setting.UIRowsCount = 1;
                setting.ColumnID = column.ID;
                UICompositions.ColumnItems.Add(setting);
            }
            return setting;
        }
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
        public ObservableCollection<DP_FormDataRepository> GetDataList()
        {
            if (AreaInitializer.SourceRelationColumnControl == null)
            {
                return AreaInitializer.Datas;
            }
            else
            {
                if (ChildRelationshipInfo != null)
                    return ChildRelationshipInfo.RelatedData;
                else return new ObservableCollection<DP_FormDataRepository>();
            }
        }
        TemporaryLinkState _TemporaryLinkState;
        public TemporaryLinkState TemporaryLinkState
        {
            get
            {
                if (_TemporaryLinkState == null)
                    _TemporaryLinkState = new TemporaryLinkState();
                return _TemporaryLinkState;
            }
        }
        public EntityUISettingDTO GetEntityUISetting()
        {
            if (UICompositions != null && UICompositions.RootItem != null && UICompositions.RootItem.EntityUISetting != null)
            {
                var entityUISetting = UICompositions.RootItem.EntityUISetting;
                return entityUISetting;
            }
            else
            {
                var setting = new EntityUISettingDTO();
                setting.UIColumnsCount = 4;
                return setting;
            }
        }



        public void SetAreaInitializer(EditEntityAreaInitializer initParam)
        {
            AreaInitializer = initParam;
            DetermineInteractionMode();
            // ManageSecurity();
            GenerateUIifNeeded();
            if (AreaInitializer.SourceRelationColumnControl == null)
            {
                AreaInitializer.Datas.CollectionChanged += Datas_CollectionChanged;
            }
        }
        private void DetermineInteractionMode()
        {
            if (AreaInitializer.IntracionMode == IntracionMode.None)
            {
                AreaInitializer.IntracionMode = CommonDefinitions.UISettings.IntracionMode.CreateSelectDirect;
            }
        }
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

        //    if (AreaInitializer.SourceRelationColumnControl != null)
        //    {
        //        if (AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
        //        {
        //            if (AreaInitializer.SecurityReadOnly)
        //            {
        //                //اینجا بیخوده چون ریدونلی بالاتر از ریدونلی بای پرنته
        //                AreaInitializer.SecurityReadOnlyByParent = true;
        //            }
        //            else
        //            {
        //                var rel = AreaInitializer.SourceRelationColumnControl.Relationship;
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

        public void GenerateUIifNeeded()
        {

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
            if (UIGenerated != null)
                UIGenerated(this, null);
        }

        private void GenerateTempView()
        {
            TemporaryLinkState.info = true;
            TemporaryLinkState.clear = true;
            if (AreaInitializer.IntracionMode == IntracionMode.Select || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            {
                TemporaryLinkState.searchView = true;
                TemporaryLinkState.popup = true;
                //if (!SimpleEntity.SearchInitially == true && !(AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship.SearchInitially))
                TemporaryLinkState.quickSearch = true;
            }
            if (AreaInitializer.IntracionMode == IntracionMode.CreateInDirect || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            {
                TemporaryLinkState.edit = true;
            }
            if (AreaInitializer.SourceRelationColumnControl == null || !(AreaInitializer.SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaMultipleData))
            {
                TemporaryDisplayView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTemporaryLinkUI(TemporaryLinkState);
                if (TemporaryLinkState.quickSearch)
                {
                    TemporaryDisplayView.SearchTextChanged += TemporaryDisplayView_TextChanged;
                }
                if (TemporaryLinkState.popup)
                    TemporaryDisplayView.FocusLost += TemporaryDisplayView_FocusLost;
                TemporaryDisplayView.TemporaryDisplayViewRequested += TemporaryDisplayView_TemporaryDisplayViewRequested;


            }

            DecideButtonsEnablity1();
        }


        void TemporaryDisplayView_TemporaryDisplayViewRequested(object sender, Arg_TemporaryDisplayViewRequested e)
        {
            TemporaryViewActionRequestedInternal(sender as I_View_TemporaryView, e.LinkType);
        }
        private void TemporaryDisplayView_FocusLost(object sender, EventArgs e)
        {
            (sender as I_View_TemporaryView).PopupVisibility = false;
        }
        private void TemporaryDisplayView_TextChanged(object sender, Arg_TemporaryDisplaySerachText e)
        {
            TemporaryViewSearchTextChanged(sender as I_View_TemporaryView, e);
        }


        public void GenerateDataView()
        {
            //بعدا بررسی شود احتمالا بیخود
            //if (DataView != null)
            //    ClearDataViewControls();

            if (this is I_EditEntityAreaOneData)
                DataView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateEditEntityAreaOneDataView(GetEntityUISetting());
            else if (this is I_EditEntityAreaMultipleData)
                DataView = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateEditEntityAreaMultipleDataView();

            ManageDataView();

            if (AreaInitializer.Preview)
                return;
            GenerateCommands();
            DecideButtonsEnablity1();
            //////ImposeDataViewSecurity();
        }
        public I_View_Area DataView
        {
            get; set;
        }


        private void ClearDataViewControls()
        {
            //ظاهرا بیخوده چون با صدا زدن جنریت دیتاویو همه چی از ازول ساخته میشه؟؟؟
            //برای فرم چینش داخل فرم که هر بار فرم ساخته نمیشود و فقط کنترلها تولید میشوند
            DataView.ClearControls();

            foreach (var item in SimpleColumnControls)
            {
                item.Visited = false;
                //     DataView.RemoveUIControlPackage(item.ControlManager);
            }
            foreach (var item in RelationshipColumnControls)
            {
                item.Visited = false;
                //   DataView.RemoveView(item.ControlManager);
            }
        }
        public void ManageDataView()
        {
            if (FullEntity != null)
            {
                //یبار بگیره که ولید کنترلها ساخته بشن
            }
            SimpleColumnControls.Clear();
            RelationshipColumnControls.Clear();
            List<BaseColumnControl> sortedListOfColumnControls = new List<EntityArea.BaseColumnControl>();

            //بر اساس همه ستونها ساخته می شود
            foreach (var column in DataEntryEntity.Columns.OrderBy(x => x.Position))
            {
                if (DataEntryEntity.Relationships.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID)))
                {
                    if (!RelationshipColumnControls.Any(x => x.Relationship.RelationshipColumns.Any(z => z.FirstSideColumnID == column.ID)))
                    {
                        var relationship = DataEntryEntity.Relationships.First(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID));
                        //کنترلهای رابطه ساخته میشن
                        AddRelationshipControl(relationship, sortedListOfColumnControls);
                    }
                }
                else
                {
                    var propertyControl = new SimpleColumnControl() { Column = column };

                    //propertyControl.ControlPackage = new UIControlPackageForSimpleColumn();
                    bool hasRangeOfValues = column.ColumnValueRange != null && column.ColumnValueRange.Details.Any();
                    // bool valueIsTitleOrValue = false;
                    //if (hasRangeOfValues)
                    //    valueIsTitleOrValue = column.ColumnValueRange.ValueFromTitleOrValue;

                    AgentHelper.SetPropertyTitle(propertyControl);

                    if (this is I_EditEntityAreaOneData)
                        propertyControl.SimpleControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateSimpleControlManagerForOneDataForm(column, GetColumnUISetting(column), hasRangeOfValues, null, true, propertyControl.Alias);
                    else if (this is I_EditEntityAreaMultipleData)
                        propertyControl.SimpleControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateSimpleControlManagerForMultipleDataForm(column, GetColumnUISetting(column), hasRangeOfValues, true, propertyControl.Alias);

                    var info = column.ID + "," + column.Name;
                    // propertyControl.AddColumnControlMessage(new ColumnControlMessageItem(propertyControl, ControlOrLabelAsTarget.Label) { Key = "columninfo", Message = info });


                    //  DecideSimpleColumnReadony(propertyControl, true);



                    SimpleColumnControls.Add(propertyControl);
                    sortedListOfColumnControls.Add(propertyControl);
                    if (hasRangeOfValues)
                    {
                        SetColumnValueRange(propertyControl, propertyControl.Column.ColumnValueRange.Details);
                    }

                }
            }
            //if (AreaInitializer.SourceRelationColumnControl != null)
            //{
            //    if (AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.SubToSuper
            //        || AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.SuperToSub)
            //    {
            //        //////if (!AreaInitializer.SourceRelationColumnControl.TargetRelationColumns.Select(x => x.ID).Contains(column.ID))
            //        //////    if (AreaInitializer.SourceRelationColumnControl.SourceEditArea.FullEntity.Columns.Any(x => x.ID == column.ID))
            //        //////        continue;
            //    }
            //}

            foreach (var relationship in DataEntryEntity.Relationships
                            .Where(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign))
            {
                AddRelationshipControl(relationship, sortedListOfColumnControls);
            }

            if (UICompositions != null && UICompositions.RootItem != null)
            {
                if (this is I_EditEntityAreaOneData)
                    (this as I_EditEntityAreaOneData).GenerateUIComposition(UICompositions.RootItem.ChildItems);
                else if (this is I_EditEntityAreaMultipleData)
                    (this as I_EditEntityAreaMultipleData).GenerateUIComposition(UICompositions.RootItem.ChildItems);
            }

            foreach (var columnControl in sortedListOfColumnControls.Where(x => x.Visited == false))
            {
                //برای تغییر کرده ها
                if (columnControl.Visited == true)
                    continue;
                if (columnControl is SimpleColumnControlGenerel)
                {
                    var simpleColumn = (columnControl as SimpleColumnControlGenerel);
                    columnControl.Visited = true;
                    DataView.AddUIControlPackage(simpleColumn.SimpleControlManager, simpleColumn.SimpleControlManager.LabelControlManager);
                }
                else if (columnControl is RelationshipColumnControlGeneral)
                {

                    var relationshipControl = (columnControl as RelationshipColumnControlGeneral);

                    //if (relationshipControl.EditNdTypeArea.TemporaryDisplayView != null)
                    //{
                    ////بعدا بررسی شود
                    //if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
                    //    ManageSuperToSubControl(sortedListOfColumnControls, relationshipControl);

                    //if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion_UnionHoldsKeys)
                    //    ManageUnionToSubUnionControl(sortedListOfColumnControls, relationshipControl);

                    //if (!relationshipControl.Visited)
                    //{
                    columnControl.Visited = true;
                    DataView.AddView(relationshipControl.RelationshipControlManagerGeneral.LabelControlManager, relationshipControl.RelationshipControlManagerGeneral);
                    //}

                    //}
                }
            }

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



            foreach (var columnControl in SimpleColumnControls)
            {
                if (columnControl.Column.IsMandatory)
                {
                    var text = columnControl.LabelControlManager.Text;
                    if (!text.StartsWith("*"))
                    {
                        text = "* " + text;
                        columnControl.LabelControlManager.Text = text;
                    }
                }
            }
            foreach (var columnControl in RelationshipColumnControls)
            {
                if (columnControl.Relationship.IsOtherSideMandatory)
                {
                    var text = columnControl.LabelControlManager.Text;
                    if (!text.StartsWith("*"))
                    {
                        text = "* " + text;
                        columnControl.LabelControlManager.Text = text;
                    }
                }
            }
            if ((AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
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

        private void AddRelationshipControl(DataEntryRelationshipDTO dataEntryRelationship, List<BaseColumnControl> sortedListOfColumnControls)
        {
            RelationshipColumnControlGeneral relationshipColumnControl;
            if (this is I_EditEntityAreaOneData)
                relationshipColumnControl = new RelationshipColumnControlOne();
            else
                relationshipColumnControl = new RelationshipColumnControlMultiple();
            relationshipColumnControl.DataEntryRelationship = dataEntryRelationship;
            relationshipColumnControl.ParentEditArea = this;
            var relationshipAlias = relationshipColumnControl.Relationship.Alias;
            relationshipColumnControl.Alias = (string.IsNullOrEmpty(relationshipAlias) ? "" : relationshipAlias + " : ");

            //اینجا ادیت اریا رابطه و همچنین کنترل منیجر رابطه مشخص میشوند. اگر مثلا کاربر به موجودیت رابطه دسترسی نداشته باشد این مقادیر تولید نمی شوند و نال بر میگردد

            bool generated = false;
            if (this is I_EditEntityAreaOneData)
            {
                generated = GenerateOneEditEntityAreaWithUIControlPackage(relationshipColumnControl as RelationshipColumnControlOne);
             
            }
            else
            {
                generated = GenerateMultipleEditEntityAreaWithUIControlPackage(relationshipColumnControl as RelationshipColumnControlMultiple);
           
            }
            if (generated != false)
            {
              
                RelationshipColumnControls.Add(relationshipColumnControl);
                sortedListOfColumnControls.Add(relationshipColumnControl);

                //برای ادمین فعال شود بد نیست
                var info = dataEntryRelationship.Relationship.ID + "," + dataEntryRelationship.Relationship.Name + Environment.NewLine + dataEntryRelationship.Relationship.TypeStr + Environment.NewLine + dataEntryRelationship.Relationship.Info;
                //AddColumnControlMessage(new ColumnControlMessageItem(relationshipColumnControl, ControlOrLabelAsTarget.Label) { Key = "relationshipinfo", Message = info });
                //    AddRelationshipColumnMessageItem(propertyControl, info, InfoColor.Black, "permanentCaption", null, true);

            }
        }
        public List<RelationshipColumnControlGeneral> SkippedRelationshipColumnControl { set; get; }
        private bool GenerateOneEditEntityAreaWithUIControlPackage(RelationshipColumnControlOne relationshipColumnControl)
        {
            var newAreaInitializer = GenereateAreaInitializer(relationshipColumnControl);

            var editAreaResult = EditEntityAreaConstructor.GetEditEntityArea(newAreaInitializer);
            if (editAreaResult.Item1 != null)
            {
                var editArea = editAreaResult.Item1;
                //اینجا اریا اینیشیالایزر ست می شود و جزئیات ظاهر رابطه نیز تولید می شود
                editArea.SetAreaInitializer(newAreaInitializer);
                I_RelationshipControlManagerOne controlManager = null;

                if (editArea.TemporaryDisplayView != null)
                {
                    controlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateRelationshipControlManagerForOneDataForm(editArea.TemporaryDisplayView, GetRelationshipUISetting(relationshipColumnControl.Relationship, true), true, relationshipColumnControl.Alias);
                    relationshipColumnControl.DataViewForTemporaryViewShown += RelationshipColumnControl_DataViewForTemporaryViewShown;
                }
                else if (editArea.DataView != null)
                {
                    controlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateRelationshipControlManagerForOneDataForm(editArea.DataView, GetRelationshipUISetting(relationshipColumnControl.Relationship, false), true, relationshipColumnControl.Alias);
                }


                relationshipColumnControl.EditNdTypeArea = editArea as I_EditEntityAreaOneData;
                relationshipColumnControl.RelationshipControlManager = controlManager;

                if (RelationshipAreaGenerated != null)
                    RelationshipAreaGenerated(this, new EditAreaGeneratedArg() { GeneratedEditArea = editArea });
                //  return new Tuple<I_EditEntityAreaOneData, I_RelationshipControlManagerOne>(, );
                return true;
            }
            else
                return false;
        }
        private bool GenerateMultipleEditEntityAreaWithUIControlPackage(RelationshipColumnControlMultiple relationshipColumnControl)
        {
            var newAreaInitializer = GenereateAreaInitializer(relationshipColumnControl);

            var editAreaResult = EditEntityAreaConstructor.GetEditEntityArea(newAreaInitializer);
            if (editAreaResult.Item1 != null)
            {
                var editArea = editAreaResult.Item1;
                //اینجا اریا اینیشیالایزر ست می شود و جزئیات ظاهر رابطه نیز تولید می شود
                editArea.SetAreaInitializer(newAreaInitializer);

                var controlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateRelationshipControlManagerForMultipleDataForm(editArea.TemporaryLinkState, GetRelationshipUISetting(relationshipColumnControl.Relationship, true), true, relationshipColumnControl.Alias);
                //   controlManager.TemporaryViewRequested += (sender, e) => View_TemporaryViewRequested(sender, e, editArea);
                //  controlManager.TemporaryViewLoaded += (sender, e) => View_TemporaryViewLoaded(sender, e, relationshipColumnControl as RelationshipColumnControl);
                //  controlManager.TemporaryViewSerchTextChanged += (sender, e) => View_TemporaryViewSearchText(sender, e, editArea);
                //       controlManager.FocusLost += ControlManager_FocusLost;
                relationshipColumnControl.DataViewForTemporaryViewShown += RelationshipColumnControl_DataViewForTemporaryViewShown;


                if (RelationshipAreaGenerated != null)
                    RelationshipAreaGenerated(this, new EditAreaGeneratedArg() { GeneratedEditArea = editArea });
                relationshipColumnControl.EditNdTypeArea = editArea as I_EditEntityAreaMultipleData;
                relationshipColumnControl.RelationshipControlManager = controlManager;
                return true;
            }
            else
                return false;
        }
        private void RelationshipColumnControl_DataViewForTemporaryViewShown(object sender, ChildRelationshipInfo e)
        {
            var relationshipColumnControl = sender as RelationshipColumnControlGeneral;
            if (relationshipColumnControl != null)
            {
                e.SetMessageAndColor();
                relationshipColumnControl.EditNdTypeArea.DecideButtonsEnablity1();
            }
        }



        private bool ParentIsSubToSuperSkipCondition(RelationshipColumnControlGeneral relationshipColumnControl)
        {
            if (relationshipColumnControl.Relationship.IsOtherSideDirectlyCreatable)
            {
                if (SimpleEntity.IndependentDataEntry == true && relationshipColumnControl.Relationship.Entity2IsIndependent)
                {
                    //منظور اینه که اگر روابط زیر نباشند اسکیپ میشوند
                    if (relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.OneToMany &&
                      relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.ManyToOne &&
                      relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.ExplicitOneToOne)
                        return true;
                }
            }
            else
            {
                //یعنی اگر این شرط برقرار باشد در هر صورت اسکیپ میشود چه مستقیم باشد یا نباشد چه ایندپندنت باشد و..
                if (AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
                {
                    var parentIsaRelationship = (AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship;
                    if (parentIsaRelationship.IsDisjoint)
                    {
                        if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
                        {
                            var isaRelationship = (relationshipColumnControl.Relationship as SuperToSubRelationshipDTO).ISARelationship;
                            if (isaRelationship.ID == parentIsaRelationship.ID)
                                return true;
                        }
                    }
                }
                if (AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
                {
                    var parentUnionRelationship = (AreaInitializer.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
                    if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion)
                    {
                        var unionRelationship = (relationshipColumnControl.Relationship as UnionToSubUnionRelationshipDTO).UnionRelationship;
                        if (unionRelationship.ID == parentUnionRelationship.ID)
                            return true;
                    }

                }
            }
            return false;
        }

        private bool ParentIsNotSubToSuperSkipCondition(RelationshipColumnControlGeneral relationshipColumnControl)
        {
            if (relationshipColumnControl.Relationship.IsOtherSideDirectlyCreatable)
            {
                if (SimpleEntity.IndependentDataEntry == true && relationshipColumnControl.Relationship.Entity2IsIndependent)
                {
                    return true;
                }
            }
            return false;
        }

        private bool RelationshipSkipable(RelationshipColumnControlGeneral relationshipColumnControl)
        {

            if (relationshipColumnControl.Relationship.IsOtherSideMandatory)
            {
                return false;
            }
            else
            {
                if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
                    return true;
                else if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
                {
                    var isaRelationship = (relationshipColumnControl.Relationship as SuperToSubRelationshipDTO).ISARelationship;

                    int parentIsaRelationshipID = 0;
                    if (AreaInitializer.SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO)
                        parentIsaRelationshipID = (AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.ID;
                    if (isaRelationship.ID == parentIsaRelationshipID)
                    {
                        if (isaRelationship.IsDisjoint)
                            return false;
                        else
                            return SimpleEntity.IndependentDataEntry == true || relationshipColumnControl.Relationship.Entity2IsIndependent;
                    }
                    else
                        return SimpleEntity.IndependentDataEntry == true || relationshipColumnControl.Relationship.Entity2IsIndependent;
                    return isaRelationship.IsTolatParticipation;
                }
                else if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
                {
                    var unionRelationship = (relationshipColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
                    return unionRelationship.IsTolatParticipation;
                }
                else
                {
                    //relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub
                    var unionRelationship = (relationshipColumnControl.Relationship as UnionToSubUnionRelationshipDTO).UnionRelationship;
                    var parentIsNotSubToSuperOrIsDifferent = false;
                    if (!(AreaInitializer.SourceRelationColumnControl.Relationship is SubUnionToSuperUnionRelationshipDTO))
                        parentIsNotSubToSuperOrIsDifferent = true;
                    else if ((AreaInitializer.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship.ID != unionRelationship.ID)
                        parentIsNotSubToSuperOrIsDifferent = true;
                    return parentIsNotSubToSuperOrIsDifferent;
                }
            }


        }

        //private bool CheckRelationshipControlIsaUnionSkip(RelationshipColumnControl relationshipColumnControl)
        //{
        //    if (AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper ||
        //      AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion_UnionHoldsKeys)
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
        //        if (!(AreaInitializer.SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO))
        //            parentIsNotSubToSuperOrIsDifferent = true;
        //        else if ((AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.ID != isaRelationship.ID)
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

        //    if (AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion_UnionHoldsKeys)
        //    {
        //        if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion_UnionHoldsKeys)
        //        {
        //            var parentUnionRelationship = (AreaInitializer.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
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

        private void ControlManager_FocusLost(object sender, EventArgs e)
        {
            (sender as I_View_TemporaryView).PopupVisibility = false;
        }
        private void View_TemporaryViewSearchText(object sender, Arg_TemporaryDisplaySerachText e, I_EditEntityArea editEntityArea)
        {
            if (editEntityArea is I_EditEntityAreaOneData)
                (editEntityArea as I_EditEntityAreaOneData).TemporaryViewSearchTextChanged(sender as I_View_TemporaryView, e);
        }
        //private void View_TemporaryViewRequested(object sender, Arg_MultipleTemporaryDisplayViewRequested e, I_EditEntityArea editEntityArea)
        //{
        //    editEntityArea.TemporaryViewActionRequestedFromMultipleEditor(sender as I_View_TemporaryView, e.LinkType, editEntityArea.AreaInitializer.SourceRelationColumnControl.Relationship, e.DataItem as DP_FormDataRepository);
        //}
        //private void View_TemporaryViewLoaded(object sender, Arg_MultipleTemporaryDisplayLoaded e, RelationshipColumnControlGeneral relationshipColumnControl)
        //{
        //    //////if (relationshipColumnControl.EditNdTypeArea.AreaInitializer.SecurityReadOnlyByParent)
        //    //////{
        //    //////    relationshipColumnControl.RelationshipControlManager.EnableDisable(e.DataItem, TemporaryLinkType.SerachView, false);
        //    //////    relationshipColumnControl.RelationshipControlManager.EnableDisable(e.DataItem, TemporaryLinkType.Clear, false);
        //    //////}
        //}

        private EditEntityAreaInitializer GenereateAreaInitializer(RelationshipColumnControlGeneral relationshipColumnControl)
        {
            EditEntityAreaInitializer newAreaInitializer = new EditEntityAreaInitializer();
            newAreaInitializer.EditAreaDataManager = AreaInitializer.EditAreaDataManager;
            newAreaInitializer.ActionActivityManager = AreaInitializer.ActionActivityManager;
            newAreaInitializer.Preview = AreaInitializer.Preview;
            newAreaInitializer.EntityID = relationshipColumnControl.Relationship.EntityID2;

            newAreaInitializer.SourceRelationColumnControl = relationshipColumnControl;
            newAreaInitializer.DataMode = relationshipColumnControl.DataEntryRelationship.DataMode;
            newAreaInitializer.IntracionMode = relationshipColumnControl.DataEntryRelationship.IntracionMode;
            //if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
            //{
            //    newAreaInitializer.DataMode = DataMode.Multiple;
            //}
            //else
            //    newAreaInitializer.DataMode = DataMode.One;

            //if (this is I_EditEntityAreaMultipleData)
            //    relationshipColumnControl.Relationship.IsOtherSideDirectlyCreatable = false;

            //if (relationshipColumnControl.Relationship.IsOtherSideCreatable == true)
            //{
            //    if (relationshipColumnControl.Relationship.IsOtherSideDirectlyCreatable == true)
            //    {
            //        if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
            //            newAreaInitializer.IntracionMode = IntracionMode.CreateDirect;
            //        else
            //            newAreaInitializer.IntracionMode = IntracionMode.CreateSelectDirect;
            //    }
            //    else
            //    {
            //        if (relationshipColumnControl.Relationship.TypeEnum != Enum_RelationshipType.OneToMany)
            //            newAreaInitializer.IntracionMode = IntracionMode.CreateSelectInDirect;
            //        else
            //            newAreaInitializer.IntracionMode = IntracionMode.CreateInDirect;
            //    }
            //}
            //else if (relationshipColumnControl.Relationship.IsOtherSideCreatable == false)
            //{
            //    if (relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
            //        return null;
            //    else
            //        newAreaInitializer.IntracionMode = IntracionMode.Select;
            //}

            //if (newAreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
            //  newAreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
            //    CheckDirectPossiblity(newAreaInitializer);


            //////var relationshipPermission = Permission.ChildsPermissions.FirstOrDefault(x => x.SecurityObjectID == relationshipColumnControl.Relationship.ID);
            //////if (relationshipPermission != null && relationshipPermission.GrantedActions.Any(x => x == SecurityAction.ReadOnly))
            //////{
            //////    newAreaInitializer.SecurityReadOnlyByParent = true;
            //////}
            //////if (relationshipColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
            //////{
            //////    if (AreaInitializer.SecurityReadOnly)
            //////    {
            //////        newAreaInitializer.SecurityReadOnlyByParent = true;
            //////    }
            //////    else
            //////    {
            //////        var rel = relationshipColumnControl.Relationship;
            //////        foreach (var relCol in rel.RelationshipColumns)
            //////        {
            //////            if (relCol.FirstSideColumn.IsReadonly)
            //////                newAreaInitializer.SecurityReadOnlyByParent = true;
            //////            if (Permission.ChildsPermissions.Any(x => x.SecurityObjectID == relCol.FirstSideColumnID && x.GrantedActions.Any(y => y == SecurityAction.ReadOnly)))
            //////                newAreaInitializer.SecurityReadOnlyByParent = true;
            //////        }
            //////    }
            //////}
            return newAreaInitializer;

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
            if (AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
                items.Add(new Tuple<RelationshipDTO, I_EditEntityArea>(AreaInitializer.SourceRelationColumnControl.Relationship, AreaInitializer.SourceRelationColumnControl.ParentEditArea));
            foreach (var item in RelationshipColumnControls.Where(x => x.Relationship is SuperToSubRelationshipDTO && (x.Relationship as SuperToSubRelationshipDTO).ISARelationship.InternalTable))
            {
                items.Add(new Tuple<RelationshipDTO, I_EditEntityArea>(item.Relationship, item.EditNdTypeArea));

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
                EntityStates1.Add(entityState);
            }
            if (AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
            {
                //////if (FullEntity.Columns.Any(x => x.ID == AreaInitializer.SourceRelationColumnControl.SourceEditArea.FullEntity.DeterminerColumnID))
                //////{
                //////    var determinerColumn = FullEntity.Columns.First(x => x.ID == AreaInitializer.SourceRelationColumnControl.SourceEditArea.FullEntity.DeterminerColumnID);
                //////    determinerColumn.DefaultValue = AreaInitializer.SourceRelationColumnControl.SourceEditArea.FullEntity.DeterminerColumnValue;
                //////}
            }
            if (AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO && (AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.InternalTable)
            {
                if (AreaInitializer.SourceRelationColumnControl.ParentEditArea.AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.ParentEditArea.AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
                {
                    foreach (var item in SimpleColumnControls.Where(x => AreaInitializer.SourceRelationColumnControl.ParentEditArea.AreaInitializer.SourceRelationColumnControl.Relationship.RelationshipColumns.Any(y => y.SecondSideColumnID == x.Column.ID)))
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
            if (Commands == null)
                Commands = new List<I_Command>();

            var clearCommand = new ClearCommand(this);
            Commands.Add(clearCommand);
            DataView.AddCommand(clearCommand.CommandManager, (!(this is I_EditEntityAreaMultipleData) && AreaInitializer.SourceRelationColumnControl != null));
            //if (this.AreaInitializer.SourceRelationColumnControl == null && DataEntryEntity.IsReadonly)
            //    clearCommand.CommandManager.SetEnabled(false);
            //if (this.AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly)
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
            if (AreaInitializer.SourceRelationColumnControl == null)
            {
                var saveCommand = new SaveCommand(this);
                Commands.Add(saveCommand);
                DataView.AddCommand(saveCommand.CommandManager);
                //if (DataEntryEntity.IsReadonly)
                //saveCommand.CommandManager.SetEnabled(false);

                var deleteCommand = new DeleteCommand(this);
                Commands.Add(deleteCommand);
                DataView.AddCommand(deleteCommand.CommandManager);


                //var archivePermission = AgentUICoreMediator.GetSubSystemAssignedPermissions("Archive");
                //if (!archivePermission.GrantedActions.Any(y => y == SecurityAction.NoAccess))
                //{


                //}

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
                var saveCloseCommand = new SaveAndCloseDialogCommand(this);
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
                DataView.AddCommand(searchCommand.CommandManager, AreaInitializer.SourceRelationColumnControl != null);
                //if (this.AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly)
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

            if (AreaInitializer.SourceRelationColumnControl != null)
            {
                if (AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect
                  || AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
                {
                    DataView.DeHighlightCommands();
                }
            }


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
        public UpdateResult UpdateData()
        {
            //SetDataShouldBeCounted();
            UpdateResult result = new UpdateResult();
            //if (editEntityArea == this)
            //{
            ApplyStatesBeforeUpdate();
            AreaInitializer.UIFomulaManager.UpdateFromulas();
            CheckEmptyOneDirectData(this);
            //if (!formulaResult)
            //{
            //    result.IsValid = false;
            //    result.Message = "نتیجه فرمولهای محاسبه شده مورد تایید نمی باشد";
            //}
            //else
            //{

            //var dataList = GetData();

            bool validationResult = AreaInitializer.UIValidationManager.ValidateData(true);
            if (!validationResult)
            {
                result.IsValid = false;
                result.Message = "اعتبارسنجی: مقادیر وارد شده در برخی از فرمها معتبر نمی باشند";
            }
            else
                result.IsValid = true;
            //}
            //}
            //باید اینجا شوال پرسده شود که اصلا کاربر میخواد با فرمولهای خطادار ادامه دهد



            return result;
        }


        public void ApplyStatesBeforeUpdate(bool shouldCheckChilds = true, ChildRelationshipInfo parentChildRelInfo = null)
        {
            var dataList = GetDataList().ToList();
            foreach (var data in dataList)
            {
                if (EntityStates1 != null && EntityStates1.Count != 0)
                    AreaInitializer.ActionActivityManager.CheckAndImposeEntityStates(data, ActionActivitySource.BeforeUpdate);

                if (shouldCheckChilds)
                {
                    foreach (var relationshipControl in RelationshipColumnControls)
                    {
                        var childRelInfo = data.ChildRelationshipInfos.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                        relationshipControl.EditNdTypeArea.SetChildRelationshipInfo(childRelInfo);

                        if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
                                 || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                            relationshipControl.EditNdTypeArea.ApplyStatesBeforeUpdate(true, childRelInfo);
                        else
                            relationshipControl.EditNdTypeArea.ApplyStatesBeforeUpdate(false, childRelInfo);

                        //اگر مستقیم نباشه فقط خود داده ها چک میشوند زیرا حضور دارند در تمپ ویو و در مخفی بودن یا ریدونلی بودن اثر داردند

                    }
                }
            }
            if (parentChildRelInfo != null)
            {
                foreach (var removedData in parentChildRelInfo.RemovedOriginalDatas)
                    AreaInitializer.ActionActivityManager.CheckAndImposeEntityStates(removedData, ActionActivitySource.BeforeUpdate);
            }
        }
        public void CheckEmptyOneDirectData(I_EditEntityArea editEntityArea)
        {
            if (this is I_EditEntityAreaMultipleData)
                return;
            var dataList = GetDataList();

            if (editEntityArea != this)
            {
                foreach (var dataItem in dataList)
                {
                    dataItem.IsEmptyOneDirectData = !AgentHelper.DataOrRelatedChildDataHasValue(dataItem, null);
                }
            }
            foreach (var relationshipControl in RelationshipColumnControls)
            {
                if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
                   || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
                {
                    relationshipControl.EditNdTypeArea.CheckEmptyOneDirectData(editEntityArea);
                }

            }
            //}
            //else
            //{
            //    foreach (var item in dataList)
            //        item.ShouldBeSkipped = true;

            //    //به نظر میاد اینجا بیخوده
            //    //foreach (var relationshipControl in RelationshipColumnControls)
            //    //{
            //    //    if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
            //    //       || relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
            //    //    {
            //    //        relationshipControl.EditNdTypeArea.CheckRedundantData(editEntityArea);
            //    //    }

            //    //}
            //}
        }



        //public bool DataIsNewInOneEditAreaAndReadonly(DP_FormDataRepository dataItem)
        //{
        //    //یعنی چون حالت مستقیم بوده مجبور بوده ایجاد بشه وگرنه نباید ایجاد میشده
        //    if (this is I_EditEntityAreaOneData && dataItem.IsNewItem &&
        //        (DataEntryEntity.IsReadonly
        //        || (AreaInitializer.SourceRelationColumnControl != null && AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly))
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
        //    if (AreaInitializer.SourceRelationColumnControl != null)
        //    {
        //        if (AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
        //        {
        //            if (data.Any())
        //                return true;

        //        }
        //        else
        //        {

        //            if (AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly && data.Any(x => x.IsNewItem))
        //                return false;
        //            else
        //                return data.Any(x => AgentHelper.DataOrRelatedChildDataHasValue(x, AreaInitializer.SourceRelationColumnControl.Relationship));
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
        //    if (AreaInitializer.SourceRelationColumnControl == null)
        //    {
        //        datalist = AreaInitializer.Datas.ToList();
        //    }
        //    else
        //    {
        //        datalist = ChildRelationshipInfo.GetRelatedData(AreaInitializer.SourceRelationColumnControl.Relationship.ID);
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
        //        relationshipControl.EditNdTypeArea.SetChildRelationshipInfo(data.ChildRelationshipInfos.First(x => x.Relationship == relationshipControl.Relationship));
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
        //    SetChildRelationshipInfo(parentData.ChildRelationshipInfos.First(x => x.Relationship == relationship));
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
        //    if (AreaInitializer.SourceRelationColumnControl == null)
        //    {

        //    }
        //    else
        //    {
        //        if (AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
        //        {
        //            var otherCommands = Commands.Where(x => x is ClearCommand || x is SearchCommand);
        //            editCommands.AddRange(otherCommands);
        //        }
        //        else
        //        {

        //        }
        //    }

        //    if (AreaInitializer.SourceRelationColumnControl != null)
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
        public void TemporaryViewSearchTextChanged(I_View_TemporaryView view, Arg_TemporaryDisplaySerachText searchArg)
        {
            if (!string.IsNullOrEmpty(searchArg.Text))
            {
                SearchViewEntityArea.SearchTextBox(searchArg.Text);
                SearchViewEntityArea.RemoveViewEntityAreaView();
                if (!view.HasPopupView)
                    view.AddPopupView(SearchViewEntityArea.ViewEntityArea.ViewView);

                view.PopupVisibility = true;
            }
        }
        public void TemporaryViewActionRequestedInternal(I_View_TemporaryView TemporaryView, TemporaryLinkType linkType)
        {
            if (LastTemporaryView != null)
            {
                if (LastTemporaryView.HasPopupView)
                    LastTemporaryView.RemovePopupView(SearchViewEntityArea.ViewEntityArea.ViewView);
            }
            LastTemporaryView = TemporaryView;
            if (linkType == TemporaryLinkType.DataView)
            {
                ShowTemporaryDataView();
            }
            else if (linkType == TemporaryLinkType.SerachView)
            {
                //if (AreaInitializer.SourceRelationColumnControl != null)
                //    AreaInitializer.SourceRelationColumnControl.RelatedData = parentData;
                ShowTemporarySearchView(false);
            }
            else if (linkType == TemporaryLinkType.QuickSearch)
            {
                //if (AreaInitializer.SourceRelationColumnControl != null)
                //    AreaInitializer.SourceRelationColumnControl.RelatedData = parentData;
                TemporaryView.QuickSearchVisibility = !TemporaryView.QuickSearchVisibility;
                if (TemporaryView.QuickSearchVisibility)
                    TemporaryView.QuickSearchSelectAll();
            }
            else if (linkType == TemporaryLinkType.Popup)
            {
                //if (AreaInitializer.SourceRelationColumnControl != null)
                //    AreaInitializer.SourceRelationColumnControl.RelatedData = parentData;
                if (!TemporaryView.PopupVisibility)
                {
                    SearchViewEntityArea.RemoveViewEntityAreaView();
                    if (!TemporaryView.HasPopupView)
                        TemporaryView.AddPopupView(SearchViewEntityArea.ViewEntityArea.ViewView);
                }
                TemporaryView.PopupVisibility = !TemporaryView.PopupVisibility;
            }
            else if (linkType == TemporaryLinkType.Clear)
            {
                ClearData(false);
                //TemporaryView.SetLinkText("");
            }
            else if (linkType == TemporaryLinkType.Info)
            {

                AgentHelper.ShowEditEntityAreaInfo(this);
            }
        }
        public void ShowTemporarySearchView(bool fromDataView)
        {
            if (this is I_EditEntityAreaOneData)
            {
                if (GetDataList().Any())
                {
                    var data = GetDataList().First();
                    if (data.IsDBRelationship && (data.ParantChildRelationshipInfo.IsHidden || data.IsReadonlyOnState))
                        return;
                    //البته بهتره فانکشنی نوشته بشه که بعد از اعمال وضعیت خود دکمه ها غیر فعال شوند
                    //فقط باید مواظب بود که اگر مثلا خالی شد دوباره دکمه ها فعال شوند
                    //مثلا فانکشن بعد از ست چایلد صدا زده بشود
                }
            }
            if (LastTemporaryView != null)
                LastTemporaryView.RemovePopupView(SearchViewEntityArea.ViewEntityArea.ViewView);
            SearchViewEntityArea.IsCalledFromDataView = fromDataView;
            SearchViewEntityArea.ShowTemporarySearchView();
        }
        public void ShowTemporaryDataView()
        {

            ObservableCollection<DP_FormDataRepository> existingData = GetDataList();
            //if (existingData == null)
            //    return;
            //////if (AreaInitializer.BusinessReadOnlyByParent || (ChildRelationshipInfo != null && AreaInitializer.ParentDataItemBusinessReadOnly.Any(x => x == ChildRelationshipInfo.SourceData)) || AreaInitializer.SecurityReadOnlyByParent || AreaInitializer.SecurityReadOnly)
            //////{
            //////    if (!existingData.Any())
            //////    {
            //////        return;
            //////    }
            //////}


            if (AreaInitializer.FormComposed == false)
            {
                GenerateDataView();
            }
            DataView.IsOpenedTemporary = true;

            if (AreaInitializer.SourceRelationColumnControl != null)
            {
                AreaInitializer.SourceRelationColumnControl.OnDataViewForTemporaryViewShown(ChildRelationshipInfo);
            }
            if (this is I_EditEntityAreaOneData && existingData.Count == 0)
            {
                (this as I_EditEntityAreaOneData).CreateDefaultData();
            }
            else
            {
                if (this is I_EditEntityAreaMultipleData)
                    (this as I_EditEntityAreaMultipleData).RemoveDataContainers();

                foreach (var data in existingData)
                {
                    if (!data.IsFullData)
                    {
                        //if (!data.KeyProperties.Any())
                        //    throw new Exception("asdad");
                        var resConvert = AreaInitializer.EditAreaDataManager.ConvertDataViewToFullData(AreaInitializer.EntityID, data, this);
                        if (!resConvert)
                        {
                            //ممکن است اینجا داده وابسته فول شود اما در نمایش فرم بعلت عدم دسترسی به داده  وابسته برای داده وابسته جاری فرم نمایش داده نشود
                            //بنابراین هر فولی به معنی اصلاح شدن داده نیست و باید خصوصیت دیگری در نظر گرفت
                            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", data.ViewInfo, Temp.InfoColor.Red);
                            return;
                        }
                    }
                    if (!this.ShowDataInDataView(data))
                    {
                        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", data.ViewInfo, Temp.InfoColor.Red);
                        return;
                    }
                }
            }
            var dialogManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow();
            dialogManager.WindowClosed += DialogManager_WindowClosed;
            dialogManager.ShowDialog(DataView, SimpleEntity.Alias, Enum_WindowSize.Big);
        }

        private void DialogManager_WindowClosed(object sender, EventArgs e)
        {
            DataView.IsOpenedTemporary = false;
            SetTempText(GetDataList());
            CheckRelationshipLabel();
            //foreach (var item in GetDataList())
            //{
            //    OnDataItemUnShown(new EditAreaDataItemArg() { DataItem = item });
            //}
        }

        private I_SearchViewEntityArea GenerateSearchViewArea()
        {
            var searchViewEntityArea = new SearchViewEntityArea();
            var searchViewInit = new SearchViewAreaInitializer();
            searchViewInit.SourceEditArea = this as I_EditEntityArea;
            //  searchViewInit.TempEntity = FullEntity;
            searchViewInit.EntityID = AreaInitializer.EntityID;
            searchViewEntityArea.SetAreaInitializer(searchViewInit);

            searchViewEntityArea.DataSelected += SearchViewEntityArea_DataSelected;
            return searchViewEntityArea;
        }
        private void SearchViewEntityArea_DataSelected(object sender, DataSelectedEventArg e)
        {
            if (e.DataItem.Count > 1)
            {
                throw new Exception("asdasd");
            }
            DP_FormDataRepository result = null;
            if (SearchViewEntityArea.IsCalledFromDataView)
            {
                result = AreaInitializer.EditAreaDataManager.GetFullDataFromDataViewSearch(AreaInitializer.EntityID, e.DataItem.First(), this);

            }
            else
            {
                result = AreaInitializer.EditAreaDataManager.ConvertDP_DataViewToDP_DataRepository(e.DataItem.First(), this);
            }
            //بهتره چک شه اگر دادهی ای موجود بود بعد کلیر بشه
            //بعدا بررسی شود که آیا لازم است ابتدا کلیر شود
            ClearData(false);

            if (result != null)
            {

                //اینجا تست شود
                bool dataCleared = true;
                if (AreaInitializer.SourceRelationColumnControl != null)
                {
                    if (GetDataList().Any())
                    {
                        //وقتی داده هنوز موجود باشد یعنی کلیر انجام نشده است
                        //چون ممکنه داده باید دیلیت بشه و کاربر موافقت نکنه
                        dataCleared = false;
                    }
                }
                if (dataCleared)
                {
                    bool addResult = false;
                    addResult = AddData(result, SearchViewEntityArea.IsCalledFromDataView);
                    if (!addResult)
                        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", result.ViewInfo, Temp.InfoColor.Red);
                }
            }
            else
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", e.DataItem.First().ViewInfo, Temp.InfoColor.Red);
            }
            //}
        }
        private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DataCollectionChanged(AreaInitializer.Datas, e);
            DecideButtonsEnablity1();
        }
        public bool CheckRemoveData(List<DP_FormDataRepository> datas)
        {
            bool clearIsOk = true;
            if (datas.Any(x => x.IsDBRelationship))
            {
                //برای روابط پرایمری به فارن که وضعیت اعمال میشه
                if (datas.Where(x => x.IsDBRelationship).Any(x => x.ParantChildRelationshipInfo.IsHidden || x.IsReadonlyOnState))
                    return false;

                //برای روابط فارن به پرایمری که وضعیت اعمال میشه
                if (datas.Where(x => x.IsDBRelationship).Any(x => x.ParantChildRelationshipInfo.IsHidden || x.ParantChildRelationshipInfo.IsReadonly))
                    return false;
            }
            bool shouldDeleteFromDB = false;
            var existingdatas = datas.Where(x => x.IsDBRelationship);
            if (existingdatas.Count() != 0)
            {
                if (AreaInitializer.SourceRelationColumnControl != null)
                {

                    if (AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
                    {
                        if (AreaInitializer.SourceRelationColumnControl.Relationship.DeleteOption == RelationshipDeleteOption.DeleteCascade || AreaInitializer.SourceRelationColumnControl.Relationship.RelationshipColumns.Any(x => !x.SecondSideColumn.IsNull))
                        {
                            shouldDeleteFromDB = true;
                        }
                    }
                    //بعدا به این فکر شود
                    //var relationship = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipManager.GetRelationship(AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID);
                    //if (relationship.IsOtherSideMandatory)
                    //    shouldDeleteFromDB = true;
                }
            }

            if (shouldDeleteFromDB)
            {
                //////if (AreaInitializer.SecurityEditAndDelete == false)
                //////{
                //////    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("پیام", "به علت رابطه اجباری داده نیاز به حذف شدن دارد اما دسترسی حذف وجود ندارد");
                //////    clearIsOk = false;
                //////}
                var deleteList = existingdatas.ToList();
                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                DR_DeleteInquiryRequest request = new DR_DeleteInquiryRequest(requester);
                request.DataItems = deleteList.Cast<DP_BaseData>().ToList();
                var reuslt = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendDeleteInquiryRequest(request);
                I_ViewDeleteInquiry view = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDeleteInquiryView();
                view.SetTreeItems(reuslt.DataTreeItems);
                if (reuslt.Loop == true)
                {
                    clearIsOk = false;
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("بعلت وجود حلقه وابستگی بین داده ها امکان حذف داده (داده های) انتخاب شده وجود ندارد");
                }
                else
                {
                    view.SetUserConfirmMode(UserDialogMode.YesNo);
                    if (reuslt.DataTreeItems.Any(x => x.ChildRelationshipDatas.Any(y => y.RelationshipDeleteOption == ModelEntites.RelationshipDeleteOption.DeleteCascade && y.RelatedData.Any())))
                        view.SetMessage("داده های وابسته نمایش داده شده نیز حذف خواهند شد. آیا مطمئن هستید؟");
                    else
                        view.SetMessage("داده نمایش داده شده حذف خواهد شد. آیا مطمئن هستید؟");
                    var result = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowPromptDialog(view, "");
                    if (result == UserDialogResult.Ok || result == UserDialogResult.No)
                    {
                        clearIsOk = false;
                    }
                    else if (result == UserDialogResult.Yes)
                    {
                        clearIsOk = true;
                    }
                }
            }

            return clearIsOk;
        }
        public bool ClearData(bool createDefaultData)
        {
            var datas = GetDataList();
            //if (datas == null)
            //    return false;
            //if (this.AreaInitializer.SourceRelationColumnControl == null && DataEntryEntity.IsReadonly)
            //    return false;
            //if (this.AreaInitializer.SourceRelationColumnControl != null && this.AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly)
            //    return false;


            foreach (var item in datas)
            {
                if (this.AreaInitializer.SourceRelationColumnControl != null && this.AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly && item.IsDBRelationship)
                    throw new Exception("Relationship is readonly!");
            }

            var clearIsOk = CheckRemoveData(datas.ToList());
            if (clearIsOk)
            {
                //foreach (var dataItem in GetDataList())
                //{
                //    ResetStatesColorAndText(dataItem);
                //}

                //if (this is I_EditEntityAreaOneData)
                //{
                //    ResetStatesColorAndText();
                //}
                GetDataList().Clear();
                if (this is I_EditEntityAreaOneData)
                {
                    if (createDefaultData)
                        (this as I_EditEntityAreaOneData).CreateDefaultData();

                    return true;
                }
                else if (this is I_EditEntityAreaMultipleData)
                {
                    (this as I_EditEntityAreaMultipleData).RemoveDataContainers();
                    return true;
                }
                return false;
            }
            else
                return false;
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

        public bool BaseAddData(DP_FormDataRepository data)
        {
            if (data == null)
                throw new Exception("sdf");
            if (data.EntityListView == null)
                data.EntityListView = DefaultEntityListViewDTO;
            if (AreaInitializer.SourceRelationColumnControl == null)
            {
                AreaInitializer.Datas.Add(data);
            }
            else
            {
                if (ChildRelationshipInfo != null)
                    ChildRelationshipInfo.AddDataToChildRelationshipInfo(data, false);
                else
                    return false;
            }
            //OnDataItemLoaded(new EditAreaDataItemLoadedArg() { DataItem = data, InEditMode = true });
            return true;
        }

        List<ChildRelationshipInfo> RegisteredParentDataItems = new List<ChildRelationshipInfo>();
        public void SetChildRelationshipInfo(ChildRelationshipInfo value)
        {

            //اگر قبلا لود شده بود دیگر لود نمیشود. به احتمال زیاد اگر پرنت هم فرم یک داده ای باشد قبلا به هنگام نمایش داده در پرنت این هم یکبار صدا زده شده و داده لود شده است
            if (value == null)
                throw new Exception("dxvxcv");
            ChildRelationshipInfo = value;
            if (!RegisteredParentDataItems.Contains(ChildRelationshipInfo))
            {
                //برای این است که کالکشن چنج تنها یکبار برای هر مجموعه داده صدا زده شود
                ChildRelationshipInfo.CollectionChanged += (sender1, e1) => NewValue_RelatedDataChanged(sender1, e1, ChildRelationshipInfo);
                RegisteredParentDataItems.Add(ChildRelationshipInfo);
            }

            //تنها در صورت نمایش پرنت این یکبار صدا زده میشود و در حالات دیگر به مانند صدا زدن به هنگام آپدیت یا ولیدشن کاربردی ندارد


        }

        public bool SetChildRelationshipInfoAndShow(ChildRelationshipInfo value)
        {
            bool result = true;
            //if (this is I_EditEntityAreaOneData && !value.RelatedData.Any())
            //{
            //    ResetStatesColorAndText();
            //}
            SetChildRelationshipInfo(value);

            bool isDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
         AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
            if (isDataView)
            {
                if (this is I_EditEntityAreaOneData && !ChildRelationshipInfo.RelatedData.Any())
                    (this as I_EditEntityAreaOneData).CreateDefaultData();
                else
                {
                    if (this is I_EditEntityAreaMultipleData)
                        (this as I_EditEntityAreaMultipleData).RemoveDataContainers();
                    foreach (var data in ChildRelationshipInfo.RelatedData)
                    {
                        if (data.EntityListView == null)
                            data.EntityListView = DefaultEntityListViewDTO;
                        var dataViewResult = this.ShowDataInDataView(data);
                        if (!dataViewResult)
                            result = false;
                    }
                }

            }
            else
            {
                SetTempText(ChildRelationshipInfo.RelatedData);
            }

            CheckRelationshipLabel();
            DecideButtonsEnablity1();

            return result;
        }

        //میتونه یه فنکشن کلی برای برسسی فعال بودن فرم بشه. مثلا وقتی وضعیتها هم بخوان دستکاری کنن فعال بودن فرم رو


        public I_View_TemporaryView TemporaryDisplayView { set; get; }
        //public void OnDataItemLoaded(EditAreaDataItemLoadedArg arg)
        //{
        //    if (DataItemLoaded != null)
        //        DataItemLoaded(this, arg);
        //}


        //اصلاح و بهتر شود برای تمپ ویوها
        public void OnDataItemShown(EditAreaDataItemLoadedArg arg)
        {
            if (DataItemShown != null)
                DataItemShown(this, arg);
        }
        //public void OnDataItemUnShown(EditAreaDataItemArg arg)
        //{
        //    if (DataItemUnShown != null)
        //        DataItemUnShown(this, arg);
        //}
        public void SetTempText(ObservableCollection<DP_FormDataRepository> relatedData)
        {
            string text = "";
            //اینجا باید داده هایی که اسکیپ میشن رو در نظر نگیره
            if (relatedData.Count > 1)
                text = "تعداد" + " " + relatedData.Count + " " + "مورد";
            foreach (var item in relatedData)
                text += (text == "" ? "" : Environment.NewLine) + item.ViewInfo;

            if (AreaInitializer.SourceRelationColumnControl == null || AreaInitializer.SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaOneData)
            {
                TemporaryDisplayView.SetLinkText(text);
                //if (string.IsNullOrEmpty(text))
                //{
                //    TemporaryDisplayView.ClearSearchText();
                //}
                if (TemporaryDisplayView.QuickSearchVisibility)
                    TemporaryDisplayView.QuickSearchVisibility = false;
            }
            else
            {
                var relationshipControl = (AreaInitializer.SourceRelationColumnControl.ParentEditArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.First(x => x.Relationship.ID == ChildRelationshipInfo.Relationship.ID);
                relationshipControl.RelationshipControlManagerGeneral.SetTemporaryViewText(ChildRelationshipInfo.SourceData, text);
                //if (string.IsNullOrEmpty(text) && relationshipControl.EditNdTypeArea.TemporaryLinkState.searchView)
                //    relationshipControl.RelationshipControlManager.SetQuickSearchVisibility(ChildRelationshipInfo.SourceData, true);
                //else
                //    relationshipControl.RelationshipControlManager.SetQuickSearchVisibility(ChildRelationshipInfo.SourceData, false);
                relationshipControl.RelationshipControlManagerGeneral.SetQuickSearchVisibility(ChildRelationshipInfo.SourceData, false);

            }

            //خیلی ایده آل نیست
            //برای اینه که وضعیتها اونم فقط توحالتی که رابطه فارن گه پرنت باشه چک بشه که اگه لازم بود دور ویو تمپ قرمز بشه 
            foreach (var dataItem in relatedData)
                OnDataItemShown(new EditAreaDataItemLoadedArg() { DataItem = dataItem, InEditMode = false });

        }

        private void NewValue_RelatedDataChanged(object sender, NotifyCollectionChangedEventArgs e, ChildRelationshipInfo childRelationshipInfo)
        {
            List<Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>> changeFkItemProperties = new List<Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>>();
            DataCollectionChanged(ChildRelationshipInfo.RelatedData, e);
            DecideButtonsEnablity1();

            if (AreaInitializer.SourceRelationColumnControl != null)
            {
                if (AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
                {
                    List<UIColumnValueDTO> uIColumnValue = new List<UIColumnValueDTO>();
                    var fkDataItem = ChildRelationshipInfo.SourceData;
                    foreach (var col in AreaInitializer.SourceRelationColumnControl.Relationship.RelationshipColumns)
                    {
                        var fkProp = fkDataItem.GetProperty(col.FirstSideColumnID);
                        if (fkProp != null)
                        {
                            if (ChildRelationshipInfo.RelatedData.Any())
                            {
                                var pkProp = ChildRelationshipInfo.RelatedData.First().GetProperty(col.SecondSideColumnID);
                                if (pkProp != null)
                                {
                                    uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = fkProp.ColumnID, ExactValue = pkProp.Value.ToString(), EvenHasValue = true, EvenIsNotNew = true });
                                }
                            }
                            else
                                uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = fkProp.ColumnID, ExactValue = null, EvenHasValue = true, EvenIsNotNew = true });
                        }
                    }
                    changeFkItemProperties.Add(new Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>(fkDataItem, uIColumnValue));
                }
                else if (AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
                {
                    //اینجا بدیش اینه که همه موارد رو مجددا مقداردهی مینکه و نه فقط اون رکوردی که اضافه شده رو
                    foreach (var fkDataItem in ChildRelationshipInfo.RelatedData)
                    {
                        List<UIColumnValueDTO> uIColumnValue = new List<UIColumnValueDTO>();
                        foreach (var col in AreaInitializer.SourceRelationColumnControl.Relationship.RelationshipColumns)
                        {
                            var pkProp = ChildRelationshipInfo.SourceData.GetProperty(col.FirstSideColumnID);
                            if (pkProp != null)
                            {
                                var fkProp = fkDataItem.GetProperty(col.SecondSideColumnID);
                                if (fkProp != null)
                                {
                                    uIColumnValue.Add(new UIColumnValueDTO() { ColumnID = fkProp.ColumnID, ExactValue = pkProp.Value.ToString(), EvenHasValue = true, EvenIsNotNew = true });
                                }
                            }
                        }
                        changeFkItemProperties.Add(new Tuple<DP_FormDataRepository, List<UIColumnValueDTO>>(fkDataItem, uIColumnValue));
                    }
                }
            }
            foreach (var item in changeFkItemProperties)
            {
                if (item.Item2.Any())
                {
                    SetColumnValueFromState(item.Item1, item.Item2, null, null, true);
                }
            }
        }
        public I_View_TemporaryView LastTemporaryView { set; get; }

        public void DataCollectionChanged(ObservableCollection<DP_FormDataRepository> dataList, NotifyCollectionChangedEventArgs e)
        {
            //bool result = true;
            bool childIsDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
             AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);

            if (this is I_EditEntityAreaOneData)
            {
                if (LastTemporaryView != null || TemporaryDisplayView != null)
                {
                    if (LastTemporaryView != null)
                    {
                        LastTemporaryView.PopupVisibility = false;
                        //   LastTemporaryView.QuickSearchVisibility = false;
                    }
                    else
                    {
                        TemporaryDisplayView.PopupVisibility = false;
                        //    TemporaryDisplayView.QuickSearchVisibility = false;
                    }
                }

                if (!childIsDataView)
                {
                    SetTempText(dataList);

                    bool eventOk = false;
                    if (GetDataList().Count == 1 && !GetDataList().First().IsNewItem)
                    {
                        eventOk = true;
                    }
                    else if (GetDataList().Count == 0)
                        eventOk = true;
                    if (eventOk)
                    {
                        (this as I_EditEntityAreaOneData).OnDataItemSelected(GetDataList().FirstOrDefault());

                    }
                }
            }
            else if (this is I_EditEntityAreaMultipleData)
            {
                if (!childIsDataView)
                    SetTempText(dataList);
            }
            CheckRelationshipLabel();
        }

        private void CheckRelationshipLabel()
        {
            //////if (AreaInitializer.SourceRelationColumnControl != null)
            //////{

            //////    if (AreaInitializer.SourceRelationColumnControl.SourceRelationshipColumnControl.RelationshipControlManager.TabPageContainer != null ||
            //////                AreaInitializer.SourceRelationColumnControl.SourceRelationshipColumnControl.RelationshipControlManager.HasExpander)
            //////    {
            //////        AreaInitializer.SourceRelationColumnControl.SourceRelationshipColumnControl.RemoveLabelControlManagerColorByKey("hasdata");
            //////        bool color = false;
            //////        if (this is I_EditEntityAreaMultipleData)
            //////        {
            //////            if (GetDataList().Count() != 0)
            //////                color = true;
            //////        }
            //////        else if (this is I_EditEntityAreaOneData)
            //////        {
            //////            if (AreaInitializer.SourceRelationColumnControl.Relationship.IsOtherSideDirectlyCreatable)
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
            //////            AreaInitializer.SourceRelationColumnControl.SourceRelationshipColumnControl.AddLabelControlManagerColor(new BaseColorItem() { Key = "hasdata", Color = InfoColor.LightGray, ColorTarget = ControlColorTarget.Background });
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
        //    if (AreaInitializer.SourceRelationColumnControl == null || AreaInitializer.SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaOneData)
        //    {
        //        if (TemporaryDisplayView != null)
        //            result.Add(new Tuple<I_DataControlManager, DP_FormDataRepository>(TemporaryDisplayView, null));
        //    }
        //    else if (ChildRelationshipInfo != null)
        //    {
        //        var relationshipControl = AreaInitializer.SourceRelationColumnControl;
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

            var dataEntryEntityIsReadonly = DataEntryEntity.IsReadonly;
            //if (AreaInitializer.SourceRelationColumnControl != null && ChildRelationshipInfo == null)
            //            throw new Exception("!!!");
            var sourceRelationshipIsReadonly = ChildRelationshipInfo != null && ChildRelationshipInfo.IsReadonly;
            var dataList = GetDataList();

            bool saveCommandEnablity = true;
            var saveCommand = GetCommand(typeof(SaveCommand));
            if (saveCommand != null)
            {
                if (dataEntryEntityIsReadonly && dataList.Any(x => x.IsNewItem))
                    saveCommandEnablity = false;
                else
                    saveCommandEnablity = dataList.Any(x => !x.IsReadonlyOnState);

                saveCommand.CommandManager.SetEnabled(saveCommandEnablity);
            }
            bool deleteCommandEnablity = true;
            var deleteCommand = GetCommand(typeof(DeleteCommand));
            if (deleteCommand != null)
            {
                if (dataEntryEntityIsReadonly)
                    deleteCommandEnablity = false;
                else
                {
                    deleteCommandEnablity = dataList.Any(x => !x.IsReadonlyOnState);
                }
                deleteCommand.CommandManager.SetEnabled(deleteCommandEnablity);
            }

            bool addCommandEnablity = true;
            var addCommand = GetCommand(typeof(AddCommand));
            if (addCommand != null)
            {
                if (dataEntryEntityIsReadonly
                    || sourceRelationshipIsReadonly)
                    addCommandEnablity = false;
                else
                    addCommandEnablity = true;
                addCommand.CommandManager.SetEnabled(addCommandEnablity);
            }

            bool removeCommandEnablity = true;
            var removeCommand = GetCommand(typeof(RemoveCommand));
            if (removeCommand != null)
            {
                if (dataList.Any())
                {
                    if (AreaInitializer.SourceRelationColumnControl != null)
                    {
                        //در دل خودش 
                        if (dataList.Any(x => !x.IsDBRelationship))
                        {
                            // باید رو ریمو خود داده کنترل شودIsDBRelationship ها
                            removeCommandEnablity = true;
                        }
                        else
                        {
                            //  if ((AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign
                            //      && (dataEntryEntityIsReadonly || dataList.All(x => x.IsReadonlyBecauseOfState)))
                            //      || dataList.All(x => x.ParantChildRelationshipInfo.IsReadonly)
                            //      || sourceRelationshipIsReadonly
                            //)

                            //همشون IsDBRelationship هستند
                            if (sourceRelationshipIsReadonly || dataList.All(x => x.DataRelationshipIsReadonly))
                                removeCommandEnablity = false;
                            else
                            {
                                //باید IsReadonlySomeHow رو خود داده هم موقع ریمو کردن چک شود
                                removeCommandEnablity = true;
                            }
                        }
                    }
                }
                else
                {
                    removeCommandEnablity = false;
                }
                removeCommand.CommandManager.SetEnabled(removeCommandEnablity);
            }

            bool searchCommandEnablity = true;
            if (AreaInitializer.SourceRelationColumnControl != null)
            {
                if (sourceRelationshipIsReadonly || dataList.All(x => x.IsDBRelationship && x.DataRelationshipIsReadonly))
                    searchCommandEnablity = false;
                // باید رو ریمو خود داده کنترل شودIsDBRelationshipغیر  بقیه

                //شرط آخر مثلا برای رابطه پدر یک به یک اصلی به فرعی بود و داده موجود فقط خواندنی باشد
            }
            var searchCommand = GetCommand(typeof(SearchCommand));
            if (searchCommand != null)
                searchCommand.CommandManager.SetEnabled(searchCommandEnablity);

            bool clearCommandEnablity = true;
            if (AreaInitializer.SourceRelationColumnControl != null)
            {
                if (sourceRelationshipIsReadonly || dataList.All(x => x.IsDBRelationship && x.DataRelationshipIsReadonly))
                    clearCommandEnablity = false;

                //شرط آخر مثلا برای رابطه پدر یک به یک اصلی به فرعی بود و داده موجود فقط خواندنی باشد
            }

            var clearCommand = GetCommand(typeof(ClearCommand));
            if (clearCommand != null)
            {
                clearCommand.CommandManager.SetEnabled(clearCommandEnablity);
            }

            if (TemporaryDisplayView != null && AreaInitializer.SourceRelationColumnControl != null)
            {

                bool quickSearchEnablity = true;


                if (sourceRelationshipIsReadonly)
                    quickSearchEnablity = false;
                else
                    quickSearchEnablity = TemporaryLinkState.quickSearch;


                bool dataViewEnablity = true;
                if (sourceRelationshipIsReadonly)
                    dataViewEnablity = dataList.Count != 0;


                TemporaryLinkState.clear = clearCommandEnablity;
                TemporaryLinkState.searchView = searchCommandEnablity;
                TemporaryLinkState.popup = searchCommandEnablity;
                TemporaryLinkState.quickSearch = quickSearchEnablity;
                TemporaryLinkState.edit = dataViewEnablity;
                TemporaryDisplayView.DisableEnable(TemporaryLinkType.Clear, clearCommandEnablity);
                TemporaryDisplayView.DisableEnable(TemporaryLinkType.SerachView, searchCommandEnablity);
                TemporaryDisplayView.DisableEnable(TemporaryLinkType.Popup, searchCommandEnablity);
                TemporaryDisplayView.DisableEnable(TemporaryLinkType.QuickSearch, quickSearchEnablity);
                TemporaryDisplayView.DisableEnable(TemporaryLinkType.DataView, dataViewEnablity);
                if (AreaInitializer.SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaMultipleData)
                {



                    //var relationshipControl = (AreaInitializer.SourceRelationColumnControl.SourceEditArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.First(x => x.Relationship.ID == ChildRelationshipInfo.Relationship.ID);
                    //relationshipControl.RelationshipControlManager.EnableDisable(ChildRelationshipInfo.SourceData, TemporaryLinkType.DataView, allowdataview);
                    var parentRelationshipControl = AreaInitializer.SourceRelationColumnControl;
                    var parentData = ChildRelationshipInfo.SourceData;


                    parentRelationshipControl.RelationshipControlManagerGeneral.EnableDisable(parentData, TemporaryLinkType.Clear, clearCommandEnablity);
                    parentRelationshipControl.RelationshipControlManagerGeneral.EnableDisable(parentData, TemporaryLinkType.SerachView, searchCommandEnablity);
                    parentRelationshipControl.RelationshipControlManagerGeneral.EnableDisable(parentData, TemporaryLinkType.Popup, searchCommandEnablity);


                    parentRelationshipControl.RelationshipControlManagerGeneral.EnableDisable(parentData, TemporaryLinkType.QuickSearch, quickSearchEnablity);
                    parentRelationshipControl.RelationshipControlManagerGeneral.EnableDisable(parentData, TemporaryLinkType.DataView, dataViewEnablity);


                    //if (isReadonly)
                    //{
                    //    parentRelationshipControl.RelationshipControlManager.EnableDisable(parentData, TemporaryLinkType.QuickSearch, false);
                    //    parentRelationshipControl.RelationshipControlManager.SetQuickSearchVisibility(parentData, dataList.Count != 0);

                    //    if (dataList.Count == 0)
                    //    {
                    //        allowdataview = false;
                    //    }
                    //}
                    //else
                    //{
                    //    parentRelationshipControl.RelationshipControlManager.EnableDisable(parentData, TemporaryLinkType.QuickSearch, TemporaryLinkState.quickSearch);
                    //    parentRelationshipControl.RelationshipControlManager.SetQuickSearchVisibility(parentData, false);
                    //}
                    //parentRelationshipControl.RelationshipControlManager.EnableDisable(parentData, TemporaryLinkType.DataView, allowdataview);

                }
            }



            //if (AreaInitializer.SourceRelationColumnControl != null)
            //{
            //    if (!isDataView)
            //    {
            //        bool allowdataview = true;
            //        if (dataList.Count == 0)
            //        {
            //            allowdataview = false;
            //        }
            //        if (AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly)
            //        {
            //            if (AreaInitializer.SourceRelationColumnControl.ParentEditArea is I_EditEntityAreaOneData)
            //            {
            //                TemporaryDisplayView.DisableEnable(TemporaryLinkType.DataView, allowdataview);
            //                TemporaryLinkState.edit = allowdataview;
            //            }
            //            else
            //            {

            //                var relationshipControl = (AreaInitializer.SourceRelationColumnControl.ParentEditArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.First(x => x.Relationship.ID == ChildRelationshipInfo.Relationship.ID);
            //                relationshipControl.RelationshipControlManager.EnableDisable(ChildRelationshipInfo.SourceData, TemporaryLinkType.DataView, allowdataview);
            //            }
            //        }

            //    }


            //}

            //      if (AreaInitializer.SourceRelationColumnControl == null || AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly || ChildRelationshipInfo == null)
            //          return;

            //      bool isDataView = (AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
            //AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);

            //if (isDataView || (DataView != null && DataView.IsOpenedTemporary))
            //{


            //    var searchCommand = GetCommand(typeof(SearchCommand));
            //    if (searchCommand != null)


            //    var clearCommand = GetCommand(typeof(ClearCommand));
            //    if (clearCommand != null)
            //        clearCommand.CommandManager.SetEnabled(enabled);
            //}

            //if (!isDataView)
            //{

            //}

        }


        //public void DecideTempViewStaticButtons()
        //{
        //    if (AreaInitializer.SourceRelationColumnControl != null)
        //    {
        //        if (AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly)
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


        private I_Command GetCommand(Type type)
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
        public void SetColumnValueFromState(DP_FormDataRepository dataItem, List<UIColumnValueDTO> uIColumnValue, EntityStateDTO state, FormulaDTO formula, bool setFkRelColumns)
        {
            if (dataItem.DataIsInEditMode())
            {

                if (setFkRelColumns == false)
                {
                    string title = "";
                    string key = "";
                    if (state != null)
                    {
                        key = "state" + "_" + state.ID;
                        title = "بر اساس وضعیت" + " " + state.Title;
                    }
                    else if (formula != null)
                    {
                        key = "formula" + "_" + formula.ID;
                        title = "بر اساس فرمول" + " " + formula.Title;
                    }

                    List<Tuple<ChildSimpleContorlProperty, string>> simpleColumnValues = new List<Tuple<ChildSimpleContorlProperty, string>>();
                    List<Tuple<DP_FormDataRepository, RelationshipColumnControl, Dictionary<int, string>>> relationshipColumnValues = new List<Tuple<DP_FormDataRepository, RelationshipColumnControl, Dictionary<int, string>>>();

                    foreach (var columnValue in uIColumnValue)
                    {
                        if (!columnValue.EvenIsNotNew && !dataItem.IsNewItem)
                            continue;

                        var column = dataItem.GetProperty(columnValue.ColumnID);
                        if (column != null)
                        {
                            if (!columnValue.EvenHasValue && !AgentHelper.ValueIsEmptyOrDefaultValue(column))
                                continue;

                            //اینجا باید بیزینسی ریدونلی شدن داده هم تست شود
                            if (RelationshipColumnControls.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ColumnID)))
                            {
                                var relationshipColumn = RelationshipColumnControls.First(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ColumnID));

                                if (!relationshipColumn.Relationship.IsReadonly)
                                {
                                    if (relationshipColumn.Relationship.RelationshipColumns.All(x => uIColumnValue.Any(z => z.ColumnID == x.FirstSideColumnID)))
                                    {
                                        Dictionary<int, string> listColumns = new Dictionary<int, string>();
                                        foreach (var relCol in relationshipColumn.Relationship.RelationshipColumns)
                                        {
                                            listColumns.Add(relCol.FirstSideColumnID, uIColumnValue.First(x => x.ColumnID == relCol.FirstSideColumnID).ExactValue);
                                        }
                                        relationshipColumnValues.Add(new Tuple<DP_FormDataRepository, RelationshipColumnControl, Dictionary<int, string>>(dataItem, relationshipColumn, listColumns));

                                    }
                                }
                            }
                            else if (dataItem.ChildSimpleContorlProperties.Any(x => x.SimpleColumnControl.Column.ID == column.ColumnID))
                            {
                                //اینجا باید بیزینسی ریدونلی شدن داده هم تست شود
                                var simpleColumn = dataItem.ChildSimpleContorlProperties.First(x => x.SimpleColumnControl.Column.ID == column.ColumnID);
                                simpleColumnValues.Add(new Tuple<ChildSimpleContorlProperty, string>(simpleColumn, columnValue.ExactValue));
                            }
                        }
                    }

                    foreach (var item in simpleColumnValues)
                    {
                        //اینجا باید بررسی بشه که نوع مقدار و پراپرتی مناسب هستند
                        item.Item1.SetValue(item.Item2);
                    }
                    foreach (var item in relationshipColumnValues)
                    {
                        var childInfo = dataItem.ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == item.Item2.Relationship.ID);
                        if (childInfo != null)
                        {
                            if (!childInfo.Relationship.IsReadonly && !childInfo.IsReadonlyOnState && !childInfo.IsHiddenOnState)
                            {
                                childInfo.SelectFromParent(item.Item3);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var columnValue in uIColumnValue)
                    {
                        if (!columnValue.EvenIsNotNew && !dataItem.IsNewItem)
                            continue;
                        var property = dataItem.GetProperty(columnValue.ColumnID);
                        if (!columnValue.EvenHasValue && !AgentHelper.ValueIsEmptyOrDefaultValue(property))
                            continue;
                        //اینجا باید بررسی بشه که نوع مقدار و پراپرتی مناسب هستند
                        //اگر ارتباط یک به یک روی کلید بود چی میشه؟
                        property.Value = columnValue.ExactValue;

                    }
                }
            }
        }
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

        public void SetColumnValueRange(SimpleColumnControlGenerel propertyControl, List<ColumnValueRangeDetailsDTO> details)
        {
            propertyControl.SimpleControlManager.SetColumnValueRange(details);
        }








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
}
