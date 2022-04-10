﻿
using CommonDefinitions.UISettings;
using ModelEntites;
using MyUILibrary.EntityArea.Commands;
using ProxyLibrary;
using System;
using System.Collections.Generic;

using MyUILibraryInterfaces.DataViewArea;
using MyUILibraryInterfaces.DataReportArea;
using System.Collections.ObjectModel;
using MyUILibrary.Temp;
using System.Linq;
namespace MyUILibrary.EntityArea
{
    public interface I_EditEntityArea
    {
        //ObservableCollection<DP_FormDataRepository> GetData();
        event EventHandler<EditAreaGeneratedArg> RelationshipAreaGenerated;
        //event EventHandler<EditAreaDataItemLoadedArg> DataItemLoaded;
        event EventHandler<EditAreaDataItemLoadedArg> DataItemShown;
        event EventHandler UIGenerated;
        event EventHandler DataViewGenerated;
        DataEntryEntityDTO DataEntryEntity { get; }
        //I_UIActionActivityManager ActionActivityManager { set; get; }
        ///    void ManageSecurity();
        //void SetDataShouldBeCounted();
        //bool ValidateData(List<DP_FormDataRepository> datalist);
        //bool ShouldBeReviewed { set; get; }
        //List<RelationshipColumnControl> SkippedRelationshipColumnControl { set; get; }
        void GenerateDataView();
        bool SetChildRelationshipInfoAndShow(ChildRelationshipInfo value);
        bool ClearData(bool createDefaultData);

        bool AddData(DP_DataRepository data, bool showDataInDataView);
        bool AddData(DP_FormDataRepository data, bool showDataInDataView);
        bool ShowDataInDataView(DP_FormDataRepository dataItem);

        void DataItemVisiblity(object dataItem, bool visible);
        void DataItemEnablity(object dataItem, bool visible);

        EntityUICompositionCompositeDTO UICompositions { set; get; }
        ObservableCollection<DP_FormDataRepository> GetDataList();
        I_View_DataContainer DataView { set; get; }
        //void RemoveData(ProxyLibrary.DP_FormDataRepository data);
        ChildRelationshipInfo ChildRelationshipInfo { get; }
        //void DecideButtons();
   //     void RemoveDataItemMessageByKey(string v);
     //   void RemoveDataItemColorByKey(string v);
        void SetChildRelationshipInfo(ChildRelationshipInfo value);
        //event EventHandler<DataUpdatedArg> Updated;
        TableDrivedEntityDTO FullEntity { get; }
        //void ReadonlySimpleColumnControl(SimpleColumnControl column, bool readonlity);
        void DecideButtonsEnablity1();
    //    void AddDataBusinessMessage(string message, InfoColor infoColor, string key, DP_FormDataRepository causingData, ControlItemPriority priority);
     //   void RemoveDataBusinessMessage(DP_FormDataRepository dataItem, string key);
        TableDrivedEntityDTO SimpleEntity { set; get; }
        TableDrivedEntityDTO EntityWithSimpleColumns { get; }
        void SetColumnValueFromState(DP_FormDataRepository dataItem, List<UIColumnValueDTO> uIColumnValue, EntityStateDTO state, FormulaDTO formula, bool setFkRelColumns);
        EntityListViewDTO DefaultEntityListViewDTO { get; }

        event EventHandler<DisableEnableChangedArg> DisableEnableChanged;
        //event EventHandler<DisableEnableCommandByTypeChangedArg> DisableEnableCommandByTypeChanged;
        TemporaryLinkState TemporaryLinkState { get; }
        I_View_TemporaryView TemporaryDisplayView { set; get; }
        void SetTempText(ObservableCollection<DP_FormDataRepository> relatedData);
        List<EntityStateDTO> EntityStates1 { get; }
        //   List<EntityStateGroupDTO> EntityStateGroups { get; }
        //I_View_SearchEntityArea SearchView { set; get; }

        //I_View_ViewEntityArea ViewView { set; get; }
        I_SearchViewEntityArea SearchViewEntityArea { get; }
        I_EditEntityLetterArea EditLetterArea { set; get; }
        //   I_EditEntityArchiveArea EditArchiveArea { set; get; }
        //I_DataViewAreaContainer DataViewAreaContainer { set; get; }
        I_DataListReportAreaContainer DataListReportAreaContainer { set; get; }
        I_EntityLettersArea EntityLettersArea { set; get; }
        //I_ViewEntityArea ViewEntityArea { set; get; }

        //List<I_EntityAreaCommand> Commands
        //{
        //    get;
        //    set;
        //}
        //////void ImposeTemporaryViewSecurity();
        //////void ImposeDataViewSecurity();
        void GenerateUIComposition(List<EntityUICompositionDTO> UICompositions);

        // void DecideDataRelatedButtons();

        //       void DecideTempViewStaticButtons();
        //  void DecideDataViewStaticButtons();
        EditEntityAreaInitializer AreaInitializer
        {
            get;
            set;
        }
        //void OnTemporaryViewLoaded();
        //List<DP_FormDataRepository> DataRepository { set; get; }

        //void AddEmptyNDType();
        void SetAreaInitializer(EditEntityAreaInitializer initParam);
        //void GenerateDataViewSearchView();

        //void RemoveData(DP_FormDataRepository data);
        //void RemoveDataWithRelations(DP_FormDataRepository data);
        //void RemoveDataFromRelation(DP_FormDataRepository relationData);

        List<RelationshipColumnControl> RelationshipColumnControls
        {
            get;
        }

        List<SimpleColumnControl> SimpleColumnControls
        {
            get;
        }

        //void CommandExecuted(I_EntityAreaCommand command);

        //List<DP_FormDataRepository> GetExistingData();
        I_View_TemporaryView LastTemporaryView { set; get; }
        //bool UpdateFromulas(List<DP_FormDataRepository> datalist);
        //void ShowAllData();
        //void ShowDataByRelation(List<DP_FormDataRepository> relationData);
        //    ObservableCollection<BaseMessageItem> MessageItems { set; get; }

        //bool SecurityReadOnlyByParentData { get; set; }
        UpdateResult UpdateData();
        //DeleteResult DeleteData();
        //void RemoveFromRelation(List<DP_FormDataRepository> specifiData = null);

        //void ClearDataFromParent(DP_FormDataRepository parentData);


        //string FetchTypePorpertyValue(DP_FormDataRepository dataRepository, ColumnDTO nD_Type_Property);


        //void ShowData(Guid guid, Enum_DP_RelationSide enum_DP_RelationSide, List<DP_FormDataRepository> list);

        void ManageDataView();



        //       ColumnControl GerRelationSourceColumnControl(ColumnDTO column);
        //void ClearTemporaryLinkTitle();
        //void ShowTemporaryDataView(DP_FormDataRepository parenttData);

        void ShowTemporarySearchView(bool fromDataView);

        //   void DataSelected(List<DP_FormDataRepository> selectedData, I_ViewEntityArea packageArea);


        //List<DP_FormDataRepository> GetRemovedData(List<DP_FormDataRepository> dataList = null);

        //////IAG_View_TemporaryView ParentTemporaryView { get; set; }

        //DP_FormDataRepository ParentMultipleDataItem { set; get; }
        void TemporaryViewActionRequestedFromMultipleEditor(I_View_TemporaryView TemporaryView, TemporaryLinkType linkType, RelationshipDTO relationship, DP_FormDataRepository parentData);

        //void ShowValidationTips(UpdateValidationResult result);
        //I_View_Container DataView { set; get; }
        //List<DP_FormDataRepository> GetData(List<DP_FormDataRepository> dataList = null, DP_FormDataRepository pItem = null);
        List<I_Command> Commands { set; get; }

        List<EntityCommandDTO> EntityCommands { get; }
        List<UIActionActivityDTO> RunningActionActivities { get; set; }

        //void SelectFromParent(DP_FormDataRepository parentDataItem, Dictionary<int, string> colAndValues);
        void CheckEmptyOneDirectData(I_EditEntityArea editEntityArea);
        //void SetColumnValueFromState(DP_FormDataRepository dataItem, List<UIColumnValueDTO> uIColumnValue, EntityStateDTO state, FormulaDTO formula, bool setFkRelColumns);
        //void SetColumnValueRangeFromState(SimpleColumnControl propertyControl, List<ColumnValueRangeDetailsDTO> details, DP_FormDataRepository data, EntityStateDTO state);
        //void ResetColumnValueRangeFromState(SimpleColumnControl simpleColumn, DP_FormDataRepository dataItem, EntityStateDTO state);
        //void ChangeSimpleColumnVisiblityFromState(DP_FormDataRepository dataItem, SimpleColumnControl simpleColumn, bool hidden, string message, string key);//, ImposeControlState hiddenControlState);
        //void ChangeSimpleColumnReadonlyFromState(DP_FormDataRepository dataItem, SimpleColumnControl simpleColumn, bool isReadonly, string message, string key);//, ImposeControlState hiddenControlState);

        //void ChangeRelatoinsipColumnVisiblityFromState(ChildRelationshipInfo childRelationshipInfo, DP_FormDataRepository dataItem, RelationshipColumnControl relationshipControl, bool hidden, string message, string key, ImposeControlState hiddenControlState);
        //void ChangeDataItemVisiblityBecauseOfCreatorRelationshipFromState(DP_FormDataRepository dataItem, string message, string key, bool skipUICheck);
        //void ChangeClearDataItemVisiblityBecauseOfCreatorRelationshipFromState(DP_FormDataRepository dataItem, string key, bool skipUICheck);

        //void ChangeRelatoinsipColumnReadonlyFromState(ChildRelationshipInfo childRelationshipInfo, DP_FormDataRepository dataItem, RelationshipColumnControl relationshipControl, bool isReadonly, string message, string key, ImposeControlState hiddenControlState);
        ////void ChangeRelatoinsipColumnUnReadonlyFromState(ChildRelationshipInfo childRelationshipInfo, DP_FormDataRepository dataItem, RelationshipColumnControl relationshipControl, string message, string key);
        //void ChangeDataItemReadonlyBecauseOfCreatorRelationshipFromState(DP_FormDataRepository dataItem, string message, string key, bool skipUICheck);
        //void ChangeClearDataItemReadonlyBecauseOfCreatorRelationshipFromState(DP_FormDataRepository dataItem, string key, bool skipUICheck);
        //bool DataItemIsInEditMode(DP_FormDataRepository sourceData);
        //bool DataItemIsInTempViewMode(DP_FormDataRepository dataItem);
        //    void DecideButtonsReadonlityByState(bool isReadonly);
        void ApplyStatesBeforeUpdate(bool shouldCheckChilds, ChildRelationshipInfo parentChildRelInfo);
        //void ChangeDataItemReadonlyFromState(DP_FormDataRepository dataItem, string message, string key, bool skipUICheck);
        //void ChangeClearDataItemReadonlyFromState(DP_FormDataRepository dataItem, string key, bool skipUICheck);
        //void AddColumnControlMessage(ColumnControlMessageItem columnControlMessageItem);
        //void AddColumnControlColor(ColumnControlColorItem columnControlColorItem);
        //void RemoveColumnControlMessage(BaseColumnControl relationshipControl, ControlOrLabelAsTarget control, DP_FormDataRepository sourceData, string key);
        //void RemoveColumnControlColor(BaseColumnControl relationshipControl, ControlOrLabelAsTarget control, DP_FormDataRepository sourceData, string key);
        //void AddDataItemMessage(DataMessageItem dataMessageItem);
        //void AddDataItemColor(DataColorItem dataColorItem);
        //void RemoveDataItemColor(DP_FormDataRepository dP_FormDataRepository, string key);
        //void RemoveDataItemMessage(DP_FormDataRepository dP_FormDataRepository, string key);


        //      void AddRelationshipColumnMessageItem(RelationshipColumnControl relationshipControl, string message, InfoColor infoColor, string key, DP_FormDataRepository causingData, bool isPermanent);
        //void AddColumnControlValidationMessage(BaseColumnControl simplePropertyControl, string message, DP_FormDataRepository causingData);
        //void AddDataValidationMessage(string message, InfoColor infoColor, string key, DP_FormDataRepository causingData, bool showInfo = true);

        //   void AddDataMessageItem(string message, InfoColor infoColor, string key, DP_FormDataRepository causingData, bool showInfo = true);
        //   void RemoveMessageItem(BaseMessageItem baseMessageItem);
        //  void RemoveMessagesItems(string key);
        //  void RemoveMessagesItems(DP_FormDataRepository dataItem, string key);

    }
    public enum ImposeControlState
    {
        Impose,
        AddMessageColor,
        Both
    }
    public class UpdateResult
    {
        public bool IsValid { set; get; }
        public string Message { set; get; }
    }
    public interface I_UIActionActivityManager
    {
        //void DoStateActionActivity(I_EditEntityAreaOneData editEntityAreaOneData, DP_FormDataRepository dataItem, EntityStateDTO state);
        //   void ApplyStatesBeforeUpdate();

        //void ResetActionActivities(DP_FormDataRepository dataItem);
        void CheckAndImposeEntityStates(DP_FormDataRepository data, bool skipUICheck, ActionActivitySource actionActivitySource);
    }
    public enum ActionActivitySource
    {
        OnShowData,
        TailOrPropertyChange,
        BeforeUpdate
    }
    public interface I_UIFomulaManager
    {
        void UpdateFromulas();
        void CalculateProperty(EntityInstanceProperty dataProperty, ColumnCustomFormulaDTO columnCustomFormula, DP_FormDataRepository dataItem, bool asDefault);
        void UpdateFromulas(List<CalculatedPropertyTree> result, RelationshipDTO relationship = null);
    }
    public interface I_UIValidationManager
    {
        bool ValidateData(bool fromUpdate);
    }
    public interface I_FormulaDataTree
    {
        void AddTitle(string title);
        object AddTreeNode(object parentNode, string title, string tooltip, InfoColor color, bool expand);
        void ClearTree();
        event EventHandler NoClicked;
        event EventHandler YesClicked;
    }
    public class CalculatedPropertyTree
    {
        public CalculatedPropertyTree()
        {
            Properties = new List<EntityInstanceProperty>();
            ChildItems = new List<EntityArea.CalculatedPropertyTree>();
        }
        public DP_FormDataRepository DataItem { set; get; }
        public List<EntityInstanceProperty> Properties { set; get; }
        public string RelationshipInfo { set; get; }
        public List<CalculatedPropertyTree> ChildItems { set; get; }
        public bool? ItemOrChildHasData { set; get; }
    }
    public interface I_RelationshipFilterManager
    {
    }
    public interface I_EditAreaLogManager
    {
        I_DataTree GetLogDataTree(List<DP_DataRepository> datas);
    }
    public interface I_EditEntityAreaOneData : I_EditEntityArea
    {
        void CheckContainersVisiblity(List<BaseColumnControl> hiddenControls);

        event EventHandler<EditAreaDataItemArg> DataItemSelected;
        List<UIControlPackageTree> UIControlPackageTree { set; get; }
        //bool ShowDataInDataView(DP_FormDataRepository relatedData);
        void ShowDataFromExternalSource(DP_DataView dataRepository = null);


        void CreateDefaultData();
        void TemporaryViewSearchTextChanged(I_View_TemporaryView view, Arg_TemporaryDisplaySerachText searchArg);
        I_View_EditEntityAreaDataView SpecializedDataView { get; }
        void OnDataItemSelected(DP_FormDataRepository DP_FormDataRepository);
        void CheckContainerVisiblity(UIControlPackageTree container);
    }
    public interface I_EditEntityAreaMultipleData : I_EditEntityArea
    {
        //void GenerateUIComposition(List<EntityUICompositionDTO> UICompositions);
        event EventHandler<EditAreaDataItemArg> DataItemRemoved;
        void RemoveDataContainers();
        void RemoveData(List<DP_FormDataRepository> datas, bool fromDataView);
        void RemoveData(DP_FormDataRepository data, bool fromDataView);
        void ShowDataFromExternalSource(List<DP_DataView> specificDate);
        bool ShowDatasInDataView(List<DP_FormDataRepository> dataItems);

        I_View_EditEntityAreaMultiple SpecializedDataView { get; }
        object FetchTypePropertyControlValue(DP_FormDataRepository dataRepository, SimpleColumnControl typePropertyControl);
        bool ShowTypePropertyControlValue(DP_FormDataRepository dataItem, SimpleColumnControl typePropertyControl, string value);
        List<DP_FormDataRepository> GetSelectedData();
    }

    public class TemplateEntityUISettings
    {
        //   public FlowDirection FlowDirection { set; get; }
        public string Language { set; get; }

    }
    public enum FlowDirection
    {
        LeftToRight,
        RightToLeft
    }
    public class EditAreaRelationSource
    {
        public event EventHandler<SourceRelatedDataChangedArg> SourceRelatedDataChanged;
        public EditAreaRelationSource()
        {
            //RelationData = new List<DP_FormDataRepository>();
            RelationshipColumns = new List<RelationshipColumnDTO>();
            //   TargetRelationColumns = new List<ColumnDTO>();
        }
        //public Enum_DP_RelationSide SourceRelationSide { set; get; }
        public RelationshipColumnControl RelationshipColumnControl { set; get; }
        public int SourceEntityID { get { return RelationshipColumnControl.Relationship.EntityID1; } }
        public int SourceTableID { get { return RelationshipColumnControl.Relationship.TableID1; } }
        public List<RelationshipColumnDTO> RelationshipColumns { set; get; }
        public RelationshipDTO Relationship { get { return DataEntryRelationshiRelationship.Relationship; } }
        public DataEntryRelationshipDTO DataEntryRelationshiRelationship { set; get; }
        public int TargetEntityID { set; get; }
        public int TargetTableID { set; get; }
        //   public List<ColumnDTO> TargetRelationColumns { set; get; }

        public Enum_RelationshipType RelationshipType
        {
            get
            {
                return Relationship.TypeEnum;
            }
        }

        public Enum_MasterRelationshipType MasterRelationshipType
        {
            get
            {
                return Relationship.MastertTypeEnum;
            }
        }
        public I_EditEntityArea SourceEditArea { set; get; }

        //DP_FormDataRepository _RelatedData;
        //public DP_FormDataRepository RelatedData
        //{

        //    set
        //    {
        //        var oldValue = _RelatedData;
        //        _RelatedData = value;
        //        if (oldValue != value)
        //            if (SourceRelatedDataChanged != null)
        //                SourceRelatedDataChanged(this, new SourceRelatedDataChangedArg() { OldValue = oldValue, NewValue = value });
        //    }
        //    get { return _RelatedData; }
        //}

        public bool TargetSideIsMandatory { set; get; }

        //public bool SourceHoldsKeys { set; get; }
    }


    //public class AG_MainNDTypeAndRelatedItems
    //{
    //    public DataMaster.EntityDefinition.ND_Type MainNDType { set; get; }
    //    public List<RelationEditorNDTypes> ChildNDTypes { set; get; }
    //    public List<RelationEditorNDTypes> ParentNDTypes { set; get; }
    //}
    //public class RelationEditorNDTypes
    //{
    //    public AG_Relation_EditorArea RelationEditorArea { set; get; }

    //    public List<DataMaster.EntityDefinition.ND_Type> RelatedNDTypes { set; get; }
    //}


    //public class BaseSimpleColumn : BaseColumnControl
    //{

    //}
    //public class RelationshipColumnControl : BaseColumnControl
    //{
    //    //public RelationshipColumnControl()
    //    //{
    //    //    Columns = new List<ColumnDTO>();
    //    //}
    //    //public I_EditEntityArea EditNdTypeArea { set; get; }
    //    //public RelationshipDTO Relationship { set; get; }
    //    //public List<ColumnDTO> Columns { set; get; }
    //}
    //public class SimpleColumnControl : BaseSimpleColumn
    //{
    //    public UIControlPackageOneDataSimpleColumn ControlPackage { set; get; }
    //    public bool SecurityNoAccess { get; set; }
    //    public bool SecurityReadOnly { get; set; }
    //    public bool SecurityEdit { get; set; }
    //    public bool BusinessHidden { get; set; }
    //    public bool BusinessReadonly { get; set; }
    //    //public List<ColumnKeyValueRangeDetailsDTO> CurrentItemsSource { get; set; }

    //    //public List<FormulaUsageParemetersDTO> FormulaUsageParemeters { get; set; }


    //    //public event EventHandler<ColumnValueChangeArg> ValueChanged;
    //    public SimpleColumnControl()
    //    {
    //        //ColumnSetting = new ColumnSetting();
    //    }


    //    //public ColumnDTO Column { set; get; }
    //    //public List<AG_RelatedConttol> RelatedUIControls { set; get; }
    //}
    //public class RelationshipColumnControl : RelationshipColumnControl
    //{
    //    public UIControlPackageOneDataRelationshipColumn ControlPackage { set; get; }
    //    public bool SecurityEdit { get; set; }
    //    public bool SecurityReadOnly { get; set; }
    //    public bool SecurityNoAccess { get; set; }
    //    public bool BusinessHidden { get; set; }

    //    //public bool SecurityReadOnlyByParent { get; set; }

    //    //public event EventHandler<ColumnValueChangeArg> ValueChanged;
    //    public RelationshipColumnControl()
    //    {

    //    }
    //    //public List<ColumnDTO> Columns { set; get; }




    //    //public List<AG_RelatedConttol> RelatedUIControls { set; get; }
    //}
    //public class RelationSourceControl : ColumnControl
    //{


    //    public RelationSourceControl(ColumnControl propertyControl)
    //    {
    //        // TODO: Complete member initialization
    //        ControlPackage = propertyControl.ControlPackage;
    //        //UI_PropertySetting = propertyControl.UI_PropertySetting;
    //        Column = propertyControl.Column;
    //        Relationship = propertyControl.Relationship;
    //    }
    //    //public SinglePropertyRelation propertyRelation { set; get; }
    //    //public SinglePropertyRelation propertyRelation { set; get; }
    //    //public RelationshipDTO Relation { set; get; }
    //    //public Enum_DP_RelationSide RelationSide { set; get; }




    //    //public List<AG_RelatedConttol> RelatedUIControls { set; get; }
    //}
    public interface I_TabGroupContainer
    {
        object UIMainControl { get; }
        UIControlPackageTree UIControlPackageTreeItem { get; set; }

        void SetVisibility(bool visible);
        void AddTabPage(I_TabPageContainer groupItem, string title, TabPageUISettingDTO tabpageSetting, bool skipTitle);
    }
    public interface I_TabPageContainer : I_View_GridContainer
    {
        bool HasHeader { get; }
        object UIMainControl { get; }
    }
    public interface I_UICompositionContainer : I_View_GridContainer
    {
        object UIMainControl { get; }
    }
    //public interface I_ControlManager
    //{
    //    void EnableDisable(bool enable);
    //    void SetReadonly(bool isreadonly);
    //    CommonOperator GetOperator();
    //    bool HasOperator();
    //    bool SetOperator(CommonOperator searchOperator);
    //    bool SetValue(string value);

    //    string GetValue();
    //    void ClearValidation();
    //    void AddValidation(BaseValidationItem item);
    //    void AddMenu(ConrolPackageMenu menu);
    //    void AddButtonMenu(ConrolPackageMenu menu);
    //    void SetBinding(EntityInstanceProperty property);
    //    void Visiblity(bool visible);
    //    void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> candidates);
    //}

    //public class UIControlPackageOneDataSimpleColumn
    //{
    //    public event EventHandler<ColumnValueChangeArg> ValueChanged;
    //    public UIControlPackageOneDataSimpleColumn()
    //    {

    //    }
    //    public I_ControlManager ControlManager { set; get; }

    //    public void Visiblity(bool visible)
    //    {

    //        ControlManager.Visiblity(visible);
    //    }

    //    public void EnableDisable(bool enable)
    //    {
    //        ControlManager.EnableDisable(enable);
    //    }
    //    public void SetReadonly(bool isreadonly)
    //    {
    //        ControlManager.SetReadonly(isreadonly);
    //    }
    //    public CommonOperator GetOperator()
    //    {
    //        return ControlManager.GetOperator();
    //    }
    //    public void SetOperator(CommonOperator operatorValue)
    //    {
    //        ControlManager.SetOperator(operatorValue);
    //    }
    //    public bool SetValue(string value)
    //    {
    //        return ControlManager.SetValue(value);
    //    }
    //    public string GetValue()
    //    {
    //        return ControlManager.GetValue();
    //    }

    //    //یک یوزر کنترل اضاقه شد برای آپشنهای فرمول
    //    //      که محاسبه و نمایش خطا و جزئیات محاسبه در آن قابل دسترسی باشد
    //    public void AddButtonMenu(ConrolPackageMenu menu)
    //    {
    //        //MenuItems.Add(menu);
    //        ControlManager.AddButtonMenu(menu);
    //    }
    //    public void AddMenu(ConrolPackageMenu menu)
    //    {
    //        //MenuItems.Add(menu);
    //        ControlManager.AddMenu(menu);
    //    }
    //    //private List<ConrolPackageMenu> MenuItems { set; get; }
    //    public void OnValueChanged(object sender, ColumnValueChangeArg arg)
    //    {
    //        if (ValueChanged != null)
    //            ValueChanged(sender, arg);
    //        else
    //        {

    //        }
    //    }

    //    public void ClearValidation()
    //    {
    //        ControlManager.ClearValidation();
    //    }

    //    public void AddValidation(BaseValidationItem item)
    //    {
    //        ControlManager.AddValidation(item);
    //    }

    //    public void SetBinding(EntityInstanceProperty property)
    //    {
    //        ControlManager.SetBinding(property);
    //    }

    //    public void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> candidates)
    //    {
    //        ControlManager.SetColumnValueRange(candidates);
    //    }


    //}





    //public interface I_ControlManagerRelationship
    //{
    //    void AddBusinessMessage(BaseValidationItem item);
    //    void ClearBusinessMessage();

    //    void ClearValidation();
    //    void AddValidation(BaseValidationItem item);
    //    void Visiblity(bool visible);
    //}

    //public class UIControlPackageOneDataRelationshipColumn
    //{
    //    public event EventHandler<ColumnValueChangeArg> ValueChanged;
    //    public UIControlPackageOneDataRelationshipColumn()
    //    {

    //        MenuItems = new List<MyUILibrary.ConrolPackageMenu>();
    //    }
    //    public I_ControlManagerRelationship ControlManager { set; get; }


    //    public List<ConrolPackageMenu> MenuItems { set; get; }


    //    public void OnValueChanged(object sender, ColumnValueChangeArg arg)
    //    {
    //        if (ValueChanged != null)
    //            ValueChanged(sender, arg);
    //        else
    //        {

    //        }
    //    }
    //    public void AddBusinessMessage(BaseValidationItem item)
    //    {
    //        ControlManager.AddBusinessMessage(item);
    //    }
    //    public void ClearBusinessMessage()
    //    {
    //        ControlManager.ClearBusinessMessage();
    //    }
    //    public void ClearValidation()
    //    {
    //        ControlManager.ClearValidation();
    //    }

    //    public void AddValidation(BaseValidationItem item)
    //    {
    //        ControlManager.AddValidation(item);
    //    }

    //    public void Visiblity(bool visible)
    //    {

    //        ControlManager.Visiblity(visible);
    //    }
    //}

    //public class UIContainerPackage : UIControlPackage
    //{

    //}
    //public class UIContainerPackage : UIControlPackage
    //{
    //    public I_View_GridContainer Container { set; get; }
    //}
    public interface I_LabelControlManager : I_DataControlManager
    {
        string Text { set; get; }
        void Visiblity(bool visible);
        //void SetTooltip(string tooltip);
        //void SetBorderColor(InfoColor color);
        //void SetBackgroundColor(InfoColor color);
        //void SetForegroundColor(InfoColor color);

    }
    public interface I_DataControlManager
    {
        void SetTooltip(object dataItem, string tooltip);
        void SetBorderColor(object dataItem, InfoColor color);
        void SetBackgroundColor(object dataItem, InfoColor color);
        void SetForegroundColor(object dataItem, InfoColor color);
    }
    public interface I_ControlManager : I_DataControlManager
    {
        void EnableDisable(object dataItem, bool enable);
        void Visiblity(object dataItem, bool visible);
        void EnableDisable(bool enable);
        void Visiblity(bool visible);
        object GetUIControl(object dataItem);


        I_LabelControlManager LabelControlManager { set; get; }
    }
    public interface I_SimpleControlManager : I_ControlManager
    {
        bool IsVisible { get; }
        CommonOperator GetOperator();
        bool HasOperator();
        bool SetOperator(CommonOperator searchOperator);
        void SetReadonly(object dataItem, bool isreadonly);
        void SetReadonly(bool isreadonly);
        bool SetValue(object dataItem, object value);
        object GetValue(object dataItem);
        void AddButtonMenu(ConrolPackageMenu menu);
        void AddButtonMenu(object dataItem, ConrolPackageMenu menu);

        void RemoveButtonMenu(string name);
        void RemoveButtonMenu(object dataItem, string name);

        void SetBinding(object dataItem, EntityInstanceProperty property);
        void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details);
        void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details, object data);
        //void AddMessage(BaseMessageItem baseMessageItem);
        //void RemoveMessage(BaseMessageItem baseMessageItem);


    }
    public interface I_Expander
    {

    }
    public interface I_RelationshipControlManager : I_ControlManager
    {

        I_TabPageContainer TabPageContainer { get; set; }
        bool HasExpander { get; }
        event EventHandler<Arg_MultipleTemporaryDisplayLoaded> TemporaryViewLoaded;
        event EventHandler<Arg_MultipleTemporaryDisplayViewRequested> TemporaryViewRequested;
        event EventHandler<Arg_TemporaryDisplaySerachText> TemporaryViewSerchTextChanged;
        event EventHandler FocusLost;
        void SetTemporaryViewText(object relatedData, string text);
        void EnableDisable(object dataItem, TemporaryLinkType link, bool enable);
        I_View_TemporaryView GetTemporaryView(object dataItem);
        void SetQuickSearchVisibility(object parentData, bool v);
        //void AddMessage(BaseMessageItem baseMessageItem);
        //void RemoveMessage(BaseMessageItem baseMessageItem);
    }

    //public class UIControlPackageForRelationshipColumn
    //{

    //  //  public event EventHandler<ColumnValueChangeArg> ValueChanged;
    //    public UIControlPackageForRelationshipColumn()
    //    {
    // //       MenuItems = new List<MyUILibrary.ConrolPackageMenu>();
    //    }
    //    public I_RelationshipControlManager ControlManager { set; get; }


    //  //  public List<ConrolPackageMenu> MenuItems { set; get; }
    //    //public void OnValueChanged(object sender, ColumnValueChangeArg arg)
    //    //{
    //    //    if (ValueChanged != null)
    //    //        ValueChanged(sender, arg);
    //    //    else
    //    //    {

    //    //    }
    //    //}

    //    //public void AddValidation(BaseMessageItem item)
    //    //{
    //    //    ControlManager.AddValidation(item);
    //    //}

    //    //public void ClearValidation(DP_FormDataRepository dataItem)
    //    //{
    //    //    ControlManager.ClearValidation(dataItem);
    //    //}

    //    //public void AddBusinessMessage(BaseMessageItem item)
    //    //{
    //    //    ControlManager.AddBusinessMessage(item);
    //    //}
    //    //public void ClearBusinessMessage(DP_FormDataRepository dataItem)
    //    //{
    //    //    ControlManager.ClearBusinessMessage(dataItem);
    //    //}

    //    //public void Visiblity(DP_FormDataRepository dataItem, bool visible)
    //    //{
    //    //    ControlManager.Visiblity(dataItem, visible);
    //    //}

    //    //public void ClearValidation()
    //    //{
    //    //    ControlManager.ClearAllValidations();
    //    //}
    //}

    //public class BaseColumnControlMultipleData
    //{
    //    public BaseColumnControlMultipleData()
    //    {
    //        //   ColumnSetting = new ColumnSetting();
    //    }
    //    public bool IsPermanentReadOnly { get; set; }
    //    public UIControlPackageOneData ControlPackage { set; get; }
    //    //public ColumnSetting ColumnSetting { set; get; }
    //    public bool Visited { set; get; }
    //    public string Alias { set; get; }
    //    public SecurityAction Permission { get; set; }
    //}
    public class BaseColumnControl
    {
        public BaseColumnControl()
        {
            //   ColumnSetting = new ColumnSetting();
        }
        //     public bool IsPermanentReadOnly { get; set; }
        //public ColumnSetting ColumnSetting { set; get; }
        public bool Visited { set; get; }
        public string Alias { set; get; }
        public I_ControlManager ControlManager { get; set; }
        public SecurityAction Permission { get; set; }
        public UIControlPackageTree UIControlPackageTreeItem { set; get; }

    }
    public class SimpleColumnControl : BaseColumnControl
    {

        //public Dictionary<DP_FormDataRepository, bool> BusinessReadOnly = new Dictionary<DP_FormDataRepository, bool>();
        //public Dictionary<DP_FormDataRepository, bool> BusinessHidden = new Dictionary<DP_FormDataRepository, bool>();
        //    public override I_SimpleControlManager ControlManager { set; get; }
        //public bool SecurityNoAccess { get; set; }
        // public bool SecurityReadOnly { get; set; }
        //public bool SecurityEdit { get; set; }
        public ColumnDTO Column { set; get; }
        public I_SimpleControlManager SimpleControlManager { get { return ControlManager as I_SimpleControlManager; } }

        //  public bool IsReadonly { get; set; }

        //    public List<ColumnValueRangeDetailsDTO> CurrentItemsSource { get; set; }

        //public event EventHandler<ColumnValueChangeArg> ValueChanged;
        public SimpleColumnControl()
        {
            //ColumnSetting = new ColumnSetting();
        }







        //public ColumnDTO Column { set; get; }
        //public List<AG_RelatedConttol> RelatedUIControls { set; get; }
    }
    public class RelationshipColumnControl : BaseColumnControl
    {
        public event EventHandler<ChildRelationshipInfo> DataViewForTemporaryViewShown;
        public I_EditEntityArea ParentEditArea { set; get; }
        public I_EditEntityArea EditNdTypeArea { set; get; }
        public RelationshipDTO Relationship { get { return DataEntryRelationship.Relationship; } }
        //    public List<ColumnDTO> RelationshipColumns { set; get; }
        public DataEntryRelationshipDTO DataEntryRelationship { set; get; }
        //public Dictionary<DP_FormDataRepository, bool> BusinessHidden = new Dictionary<DP_FormDataRepository, bool>();
        public I_RelationshipControlManager RelationshipControlManager { get { return ControlManager as I_RelationshipControlManager; } }

        //public bool SecurityNoAccess { get; set; }
        //public bool SecurityReadOnly { get; set; }
        //public bool SecurityEdit { get; set; }

        //public event EventHandler<ColumnValueChangeArg> ValueChanged;
        public RelationshipColumnControl()
        {
            //    RelationshipColumns = new List<ColumnDTO>();
        }

        public void OnDataViewForTemporaryViewShown(ChildRelationshipInfo childRelationshipInfo)
        {
            if (DataViewForTemporaryViewShown != null)
                DataViewForTemporaryViewShown(this, childRelationshipInfo);
        }
        //public List<ColumnDTO> Columns { set; get; }

        //public RelationshipDTO Relationship { set; get; }
        //public I_EditEntityArea EditNdTypeArea { set; get; }

        //public List<AG_RelatedConttol> RelatedUIControls { set; get; }
    }


    public class ColumnValueChangeArg : EventArgs
    {


        public ColumnValueChangeArg()
        {
            //   DataItems = new List<DP_FormDataRepository>();
        }
        public object DataItem { set; get; }

        public string OldValue { set; get; }
        public string NewValue { set; get; }




    }
    public class UIControlPackageTree
    {
        public UIControlPackageTree()
        {
            ChildItems = new List<UIControlPackageTree>();
        }
        public object Container { set; get; }
        public object Item { set; get; }
        public object UIItem { set; get; }
        public EntityUICompositionDTO UIComposition { set; get; }
        public List<UIControlPackageTree> ChildItems { set; get; }
        public UIControlPackageTree ParentItem { set; get; }
    }


    //public class RelationshipDTO
    //{
    //    public RelationshipDTO()
    //    {
    //        SourceRelationProperty = new List<ColumnDTO>();
    //        TargetRelationProperty = new List<ColumnDTO>();
    //    }
    //    public int SourceTableID { set; get; }
    //    public int SourceEntityID { set; get; }
    //    public List<ColumnDTO> SourceRelationProperty { set; get; }
    //    public RelationshipDTO Relationship { set; get; }
    //    public int TargetTableID { set; get; }
    //    public int TargetEntityID { set; get; }
    //    public List<ColumnDTO> TargetRelationProperty { set; get; }

    //    public bool OtherSideIsCreatable { set; get; }
    //    public bool? OtherSideIsDirectlyCreatable { set; get; }

    //}
    //public class ManyToOneRelationshipDTO : RelationshipDTO
    //{
    //    public bool OneSideIsMandatory { set; get; }
    //}

    //public class OneToManyRelationshipDTO : RelationshipDTO
    //{
    //    public bool ManySideIsMandatory { set; get; }
    //    public int DetailsCount { set; get; }
    //}
    //public class ExplicitOneToOneRelationshipDTO : RelationshipDTO
    //{
    //    public bool OtherOneSideIsMandatory { set; get; }
    //}

    //public class ImplicitOneToOneRelationshipDTO : RelationshipDTO
    //{
    //    public bool OtherOneSideIsMandatory { set; get; }
    //}


    //public class SuperToSubRelationshipDTO : RelationshipDTO
    //{
    //    public bool OtherSideIsMandatory { set; get; }
    //}
    //public class SubToSuperRelationshipDTO : RelationshipDTO
    //{
    //    public bool OtherSideIsMandatory { set; get; }
    //    //public List<ColumnDTO> TargetRelationProperty { set; get; }

    //    //public bool OtherOneSideIsMandatory { set; get; }
    //}
    //public class SuperUnionToSubUnionRelationshipDTO : RelationshipDTO
    //{

    //}
    //public class SuperUnionToSubUnionRelationshipDTO : RelationshipDTO
    //{

    //}
    //public class SubUnionToSuperUnionRelationshipDTO : RelationshipDTO
    //{
    //    public bool OtherSideIsMandatory { set; get; }

    //}
    //public class SubUnionToSuperUnionRelationshipDTO : RelationshipDTO
    //{
    //    public bool OtherSideIsMandatory { set; get; }

    //}

    public class BaseControlManager
    {
        public String Key { get; set; }
        //   public List<I_DataControlManager> MultipleDataControlManager { get; set; }
        //    public DP_FormDataRepository CausingDataItem { set; get; }
        public ControlItemPriority Priority { set; get; }
    }

    public class BaseMessageItem : BaseControlManager
    {
        public String Message { get; set; }
        public bool IsPermanentMessage { get; set; }
    }
    public class BaseColorItem : BaseControlManager
    {
        public ControlColorTarget ColorTarget { set; get; }
        public Temp.InfoColor Color { get; set; }
    }
    public class DataMessageItem : BaseMessageItem
    {
        public DataMessageItem(string message, string key, ControlItemPriority priority)
        {
            Message = message;
            Key = key;
            Priority = priority;
        }
    }
    public class DataColorItem : BaseColorItem
    {
        public DataColorItem(InfoColor infoColor, ControlColorTarget controlColorTarget, string key, ControlItemPriority priority)
        {
            Color = infoColor;
            ColorTarget = controlColorTarget;
            Key = key;
            Priority = priority;
        }
    }


    public class ColumnControlMessageItem : BaseMessageItem
    {
        public ColumnControlMessageItem(string message, ControlOrLabelAsTarget controlOrLabelAsTarget, string key, ControlItemPriority priority)
        {
            Message = message;
            ControlOrLabel = controlOrLabelAsTarget;
            Key = key;
            Priority = priority;
        }
        public BaseColumnControl ColumnControl { set; get; }
        public ControlOrLabelAsTarget ControlOrLabel { set; get; }

    }

    public class ColumnControlColorItem : BaseColorItem
    {
        public ColumnControlColorItem(InfoColor infoColor, ControlOrLabelAsTarget controlOrLabelAsTarget, ControlColorTarget controlColorTarget, string key, ControlItemPriority priority)
        {
            Color = infoColor;
            ControlOrLabel = controlOrLabelAsTarget;
            ColorTarget = controlColorTarget;
            Key = key;
            Priority = priority;
        }
        public ControlOrLabelAsTarget ControlOrLabel { set; get; }
    }
    public enum ControlOrLabelAsTarget
    {
        Control,
        Label

    }
    public enum ControlItemPriority
    {

        Normal,
        High
    }
    public enum ControlColorTarget
    {
        Border,
        Background,
        Foreground
    }
    //public class ColumnMessageItem : BaseMessageItem
    //{

    //}

    //public class DataValidationItem : BaseMessageItem
    //{

    //}
    //public class RelationshipColumnMessageItem : BaseMessageItem
    //{
    //    public BaseMessageItem RelationshipControl { set; get; }
    //}
    public class DeleteResult
    {

    }
    //public class ValidationItem
    //{
    //    public string Message { set; get; }
    //    public InfoColor Color { set; get; }
    //    public DP_FormDataRepository dataItem { set; get; }
    //}
    public class DisableEnableChangedArg : EventArgs
    {
        public bool Enabled { set; get; }
    }
    public class DisableEnableCommandByTypeChangedArg : EventArgs
    {
        public bool Enabled { set; get; }
        public Type Type { set; get; }
    }

    public class DataUpdatedArg : EventArgs
    {
        public DataUpdatedArg()
        {
            UpdateColumnIds = new List<int>();
        }
        public DP_FormDataRepository DateRepository { set; get; }
        public List<int> UpdateColumnIds { set; get; }
    }

    public class SourceRelatedDataChangedArg : EventArgs
    {
        public SourceRelatedDataChangedArg()
        {

        }
        public DP_FormDataRepository OldValue { set; get; }
        public DP_FormDataRepository NewValue { set; get; }
    }
    public interface I_DataTree
    {
        object AddTreeNode(object parentNode, string title, bool expand, InfoColor color = InfoColor.Black);
        void ClearTree();

        event EventHandler CloseRequested;
    }
    public interface I_ConfirmUpdate
    {
        event EventHandler<ConfirmUpdateDecision> Decided;
        event EventHandler DateTreeRequested;
    }
    public class ConfirmUpdateDecision : EventArgs
    {
        public bool Confirm { set; get; }
    }
    public class EditAreaGeneratedArg : EventArgs
    {
        public I_EditEntityArea GeneratedEditArea { set; get; }
    }
    public class EditAreaDataItemArg : EventArgs
    {
        public DP_FormDataRepository DataItem { set; get; }
    }
    public class EditAreaDataItemLoadedArg : EventArgs
    {
        public bool InEditMode { set; get; }
        public DP_FormDataRepository DataItem { set; get; }
    }
    public interface I_FormulaOptions
    {
        event EventHandler ClaculateRequested;
        event EventHandler ClaculationDetailsRequested;
        event EventHandler ErrorDetailRequested;
        bool CalculationEnablity { set; get; }
        bool CalculationDetailsEnablity { set; get; }
        bool ErrorDetailsEnablity { set; get; }
        //List<FormulaUsageParemetersDTO> FormulaUsageParemeters { get; set; }

        //void SetException(string formulaException);
        //void ClearException();
    }
    public interface I_FormulaUsageParameters
    {
        object AddTreeNode(object parentNode, string title);
        void ClearTree();

        event EventHandler CloseRequested;
    }
    public class EditAreaRelationship
    {
        public RelationshipDTO Relationship { set; get; }
        public EditAreaRelationshipType Type { set; get; }
    }
    public enum EditAreaRelationshipType
    {
        LogicalVisual,
        OnlyLogical
    }

    public class EditAreaDataActionLog
    {
        public EditAreaDataActionLog()
        {
            KeyProperties = new List<EntityInstanceProperty>();
            RelatedLog = new List<EntityArea.RelatedDataLog>();
            LogProperties = new List<ActionLogProperty>();
        }
        public List<EntityInstanceProperty> KeyProperties { set; get; }
        public int EntityID { set; get; }
        //  public string EntityName { get; set; }
        public string DataInfo { set; get; }
        public LogAction ActionType { set; get; }
        public List<RelatedDataLog> RelatedLog { set; get; }
        public string Comment { set; get; }
        public List<ActionLogProperty> LogProperties { set; get; }
        public InfoColor InfoColor { get; set; }


    }
    public class ActionLogProperty
    {
        public int ColumnID { set; get; }
        public string ColumnaName { set; get; }
        public string OldValue { set; get; }
        public string NewValue { set; get; }
        public InfoColor InfoColor { get; set; }
    }
    public class RelatedDataLog
    {
        public RelatedDataLog()
        {
            RelatedActions = new List<EntityArea.EditAreaDataActionLog>();
        }
        public int RelationshipID { set; get; }
        public string RelationshipInfo { set; get; }
        public List<EditAreaDataActionLog> RelatedActions { set; get; }
    }
    public enum LogAction
    {
        NewData,
        EditData,
        EditDataNotEdited,
        AddedToRelationshipNewData,
        AddedToRelationshipAndEdited,
        AddedToRelationshipAndNotEdited,
        DeleteData,
        RemoveRelationship
    }








    public class ParentRelationshipInfo

    {
        public ParentRelationshipInfo(ChildRelationshipInfo parantChildRelationshipInfo)
        {
            ParantChildRelationshipInfo = parantChildRelationshipInfo;
        }
        public RelationshipColumnControl RelationshipColumnControl { get { return ParantChildRelationshipInfo.RelationshipControl; } }
        public ChildRelationshipInfo ParantChildRelationshipInfo { set; get; }
        public int RelationshipID { get { return ParantChildRelationshipInfo.Relationship.PairRelationshipID; } }
        public RelationshipDTO ToRelationship { get { return ParantChildRelationshipInfo.Relationship.PairRelationship; } }
        public DP_FormDataRepository SourceData { get { return ParantChildRelationshipInfo.SourceData; } }
        public bool IsHidden { get { return IsHiddenOnState || IsHiddenOnShow; } }
        public bool IsHiddenOnState { get; set; }
        public bool IsHiddenOnShow { get; set; }
        public bool IsReadonly { get { return IsReadonlyOnState || IsReadonlyOnShow; } }
        public bool IsReadonlyOnState { get; set; }
        public bool IsReadonlyOnShow { get; set; }
    }
    public class ChangeMonitor
    {
        public string GeneralKey { set; get; }
        public string UsageKey { set; get; }
        public DP_FormDataRepository DataToCall { set; get; }
        public int columnID { set; get; }
        public string RestTail { set; get; }
    }
}
