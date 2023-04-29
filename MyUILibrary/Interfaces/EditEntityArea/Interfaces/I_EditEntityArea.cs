
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

        event EventHandler<EditAreaDataItemLoadedArg> DataItemShown;
        event EventHandler UIGenerated;
        event EventHandler DataViewGenerated;
        DataEntryEntityDTO DataEntryEntity { get; }
        I_View_Area FirstView { get; }
        void ClearUIData(DP_FormDataRepository dataItem);
        void RemoveDatas(List<DP_FormDataRepository> datas);
        bool GenerateRelationshipControlEditArea(RelationshipColumnControlGeneral relationshipColumnControl, RelationshipUISettingDTO relationshipUISetting);
        void RemoveData(DP_FormDataRepository data);
        void GenerateDataView();
        bool ClearData();
        I_Command GetCommand(Type type);
        bool AddData(DP_FormDataRepository data);
        bool ShowDataInDataView(DP_FormDataRepository dataItem);
        List<DP_FormDataRepository> GetDataList();
        I_View_Area DataViewGeneric { get; }
        TableDrivedEntityDTO FullEntity { get; }
        TableDrivedEntityDTO SimpleEntity { set; get; }
        EntityListViewDTO DefaultEntityListViewDTO { get; }
        event EventHandler<DisableEnableChangedArg> DisableEnableChanged;
        I_View_TemporaryView TemporaryDisplayView { set; get; }
        void SetTempText();
        List<EntityStateDTO> EntityStates1 { get; }
        //I_SearchAndViewEntityArea SearchViewEntityArea { set; get; }
        I_EditEntityLetterArea EditLetterArea { set; get; }
        I_DataListReportAreaContainer DataListReportAreaContainer { set; get; }
        I_EntityLettersArea EntityLettersArea { set; get; }
        void ShowDataFromExternalSource(DP_BaseData dataRepository = null);
        void ShowDataFromExternalSource(List<DP_BaseData> specificDate);
        void GenerateUIControlsByCompositionDTO(EntityUICompositionDTO UICompositions);
        void ShowTemproraryUIViewArea(I_View_TemporaryView view);
        EditEntityAreaInitializer AreaInitializer { get; set; }
        List<RelationshipColumnControlGeneral> RelationshipColumnControls { get; }
        List<SimpleColumnControlGenerel> SimpleColumnControls { get; }
        //     I_View_TemporaryView LastTemporaryView { set; get; }
        UpdateResult UpdateDataAndValidate(List<DP_FormDataRepository> datas);
        void UpdateData(List<DP_FormDataRepository> datas);
        bool ValidateData(List<DP_FormDataRepository> datas);
        void SetDataIsUpdated(List<DP_FormDataRepository> datas);
        List<I_Command> Commands { set; get; }
        List<EntityCommandDTO> EntityCommands { get; }
        List<UIActionActivityDTO> RunningActionActivities { get; set; }
        ChildRelationshipInfo ChildRelationshipInfoBinded { get; set; }

        I_SearchEntityArea SearchEntityArea { get;  }
        I_ViewEntityArea ViewEntityArea { get; }

        //   event EventHandler<DataSelectedEventArg> DataSelected;
        I_View_SearchViewEntityArea ViewForSearchAndView { set; get; }
        void CheckSearchInitially();
        List<RelationshipFilterDTO> RelationshipFilters { set; get; }
      //  void SearchInitialy();
        bool SearchInitialyDone { get; set; }
     //   void SearchConfirmed(DP_SearchRepositoryMain searchItems, bool select);
        void ShowSearchView(bool fromDataView);
        void SelectFromParent( RelationshipDTO relationship, DP_DataRepository parentDataItem, Dictionary<int, object> colAndValues);
        void SelectData(List<Dictionary<ColumnDTO, object>> items);
        //   void SearchTextBox(string text);
        // void RemoveViewEntityAreaView();

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
        void CheckAndImposeEntityStates(DP_FormDataRepository data, ActionActivitySource actionActivitySource);
        void SetExistingDataFirstLoadStates(DP_FormDataRepository dataItem);
        void DataToShowInDataview(DP_FormDataRepository specificDate);
    }

    public interface I_UIFomulaManager
    {
        void UpdateFromulas();
        void CalculateProperty(ChildSimpleContorlProperty ChildSimpleContorlProperty);
        void CalculateProperty(DP_DataRepository dataItem, EntityInstanceProperty dataProperty);

        //   void UpdateFromulas();
    }
    public interface I_UIValidationManager
    {
        bool ValidateData(DP_FormDataRepository data);
        //   bool ValidateData(bool fromUpdate);
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
        //     List<UIControlComposition> UIControlPackageTree { set; get; }
        //bool ShowDataInDataView(DP_FormDataRepository relatedData);



        void CreateDefaultData();
     //   void TemporaryViewSearchTextChanged(I_View_TemporaryView view, Arg_TemporaryDisplaySerachText searchArg);
        //     I_View_EditEntityAreaDataView SpecializedDataView { get; }
        I_View_EditEntityAreaDataView DataView { get; set; }

        //  void OnDataItemSelected(DP_FormDataRepository DP_FormDataRepository);
        //  void CheckContainerVisiblity(UIControlComposition container);
    }
    public interface I_EditEntityAreaMultipleData : I_EditEntityArea
    {
        //void GenerateUIComposition(List<EntityUICompositionDTO> UICompositions);
        event EventHandler<EditAreaDataItemArg> DataItemRemoved;
        void RemoveDataContainers();
        //  void RemoveData(List<DP_FormDataRepository> datas);

        void RemoveDataContainer(DP_FormDataRepository dataItem);

        //     void RemoveData(DP_FormDataRepository data);

        //      bool ShowDatasInDataView(List<DP_FormDataRepository> dataItems);

        //    I_View_EditEntityAreaMultiple SpecializedDataView { get; }
        I_View_EditEntityAreaMultiple DataView { get; set; }

        //   object FetchTypePropertyControlValue(DP_FormDataRepository dataRepository, SimpleColumnControlMultiple typePropertyControl);
        //    bool ShowTypePropertyControlValue(DP_FormDataRepository dataItem, SimpleColumnControlMultiple typePropertyControl, string value);
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
    //public class EditAreaRelationSource
    //{
    //    public event EventHandler<SourceRelatedDataChangedArg> SourceRelatedDataChanged;
    //    public EditAreaRelationSource()
    //    {
    //        //RelationData = new List<DP_FormDataRepository>();
    //        RelationshipColumns = new List<RelationshipColumnDTO>();
    //        //   TargetRelationColumns = new List<ColumnDTO>();
    //    }
    //    //public Enum_DP_RelationSide SourceRelationSide { set; get; }
    //    public RelationshipColumnControlGeneral RelationshipColumnControl { set; get; }
    //    public int SourceEntityID { get { return RelationshipColumnControl.Relationship.EntityID1; } }
    //    public int SourceTableID { get { return RelationshipColumnControl.Relationship.TableID1; } }
    //    public List<RelationshipColumnDTO> RelationshipColumns { set; get; }
    //    public RelationshipDTO Relationship { get { return DataEntryRelationshiRelationship.Relationship; } }
    //    public DataEntryRelationshipDTO DataEntryRelationshiRelationship { set; get; }
    //    public int TargetEntityID { set; get; }
    //    public int TargetTableID { set; get; }
    //    //   public List<ColumnDTO> TargetRelationColumns { set; get; }

    //    public Enum_RelationshipType RelationshipType
    //    {
    //        get
    //        {
    //            return Relationship.TypeEnum;
    //        }
    //    }

    //    public Enum_MasterRelationshipType MasterRelationshipType
    //    {
    //        get
    //        {
    //            return Relationship.MastertTypeEnum;
    //        }
    //    }
    //    public I_EditEntityArea SourceEditArea { set; get; }

    //    //DP_FormDataRepository _RelatedData;
    //    //public DP_FormDataRepository RelatedData
    //    //{

    //    //    set
    //    //    {
    //    //        var oldValue = _RelatedData;
    //    //        _RelatedData = value;
    //    //        if (oldValue != value)
    //    //            if (SourceRelatedDataChanged != null)
    //    //                SourceRelatedDataChanged(this, new SourceRelatedDataChangedArg() { OldValue = oldValue, NewValue = value });
    //    //    }
    //    //    get { return _RelatedData; }
    //    //}

    //    public bool TargetSideIsMandatory { set; get; }

    //    //public bool SourceHoldsKeys { set; get; }
    //}


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
        EntityUICompositionDTO UICompositionDTO { get; set; }

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

    public interface I_UIElementManager
    {
        void SetTooltip(string tooltip);
        //  void SetColor(InfoColor color);
        void Visiblity(bool visible);

        void SetBorderColor(InfoColor color);
        void SetBackgroundColor(InfoColor color);
        void SetForegroundColor(InfoColor color);

        //void SetBorderColorDefault();
        //void SetBackgroundColorDefault();
        //void SetForegroundColorDefault();
    }
    //public interface I_UIViewManager : I_UIElementManager
    //{

    //}
    public interface I_UIControlManager : I_UIElementManager
    {
        void SetBinding(EntityInstanceProperty property);
        //void SetBorderColor(InfoColor color);
        //void SetBackgroundColor(InfoColor color);
        //void SetForegroundColor(InfoColor color);
        void AddButtonMenu(ConrolPackageMenu menu);
        List<ConrolPackageMenu> GetButtonMenus();
        void EnableDisable(bool enable);
        void SetReadonly(bool isreadonly);
        void RemoveButtonMenu(string name);
        void RemoveButtonMenus();
        bool SetValue(object value);
        object GetValue();
        object GetUIControl();
        void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details,bool multiselect);
        void SetOperator(CommonOperator operatorValue);
        CommonOperator GetOperator();
        void ClearValue();
        void SetMenuColor(InfoColor color);
        void ClearMenuColor();
        //    {
        //        ControlManager.SetOperator(operatorValue);
        //    }
        //void SetTooltip(string tooltip);
        //void SetBorderColor(InfoColor color);
        //void SetBackgroundColor(InfoColor color);
        //void SetForegroundColor(InfoColor color);

    }
    //public interface I_UIControlManager : I_UIControlManager
    //{
    //    string Text { set; get; }
    //    //void SetTooltip(string tooltip);
    //    //void SetBorderColor(InfoColor color);
    //    //void SetBackgroundColor(InfoColor color);
    //    //void SetForegroundColor(InfoColor color);

    //}
    //public interface I_DataControlManager
    //{

    //}
    //public interface I_ControlManager : I_DataControlManager
    //{

    //}

    //public interface I_ControlHelper
    //{

    //    void EnableDisable(bool enable);
    //    void SetReadonly(bool isreadonly);
    //    CommonOperator GetOperator();
    //    bool SetOperator(CommonOperator searchOperator);
    //    bool SetValue(object value);
    //    object GetValue();
    //    // void SetTooltip( string tooltip);
    //    //void ClearTooltip();
    //    void SetBorderColor(InfoColor color);
    //    //void ClearBorderColor();
    //    void SetBinding(EntityInstanceProperty property);
    //    void AddButtonMenu(ConrolPackageMenu menu);
    //    void Visiblity(bool visible);
    //    bool IsVisible();
    //    bool HasOperator();
    //    void SetBackgroundColor(InfoColor color);
    //    void SetForegroundColor(InfoColor color);
    //    void RemoveButtonMenu(string name);
    //}

    public interface I_SimpleControlManagerGeneral
    {
        //  object MainControl { get; }
        //  object WholeControl { get; }
        ////  bool IsVisible { get; }
        //  CommonOperator GetOperator();
        //  bool HasOperator();
        //  bool SetOperator(CommonOperator searchOperator);

        //  void SetReadonly(bool isreadonly);
        //  void AddButtonMenu(ConrolPackageMenu menu);
        //  void RemoveButtonMenu(string name);
        //  void SetBinding(EntityInstanceProperty property);
        //  void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details);

        //  //void AddMessage(BaseMessageItem baseMessageItem);
        //  //void RemoveMessage(BaseMessageItem baseMessageItem);

        //  //  void EnableDisable(object dataItem, bool enable);
        //  // void Visiblity(object dataItem, bool visible);
        //  void EnableDisable(bool enable);
        //  void Visiblity(bool visible);
        // 

        //  void SetBorderColor(object dataItem, InfoColor color);
        //  void SetBackgroundColor(object dataItem, InfoColor color);
        // void SetForegroundColor(object dataItem, InfoColor color);

        //   I_UIControlManager LabelControlManager { set; get; }

        // void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details);
    }

    public interface I_SimpleControlManagerOne : I_SimpleControlManagerGeneral
    {

        I_UIControlManager GetUIControlManager();

        // 

        //  void SetBorderColor(object dataItem, InfoColor color);
        //  void SetBackgroundColor(object dataItem, InfoColor color);
        // void SetForegroundColor(object dataItem, InfoColor color);




    }
    public interface I_SimpleControlManagerMultiple : I_SimpleControlManagerGeneral
    {
        I_UIControlManager GetUIControlManager(object dataItem);

        // 
        //  void SetBorderColor(object dataItem, InfoColor color);
        //  void SetBackgroundColor(object dataItem, InfoColor color);
        // void SetForegroundColor(object dataItem, InfoColor color);
    }
    public interface I_Expander
    {

    }
    public interface I_RelationshipControlManagerGeneral
    {
        //    I_UIControlManager LabelControlManager { set; get; }
        I_TabPageContainer TabPageContainer { get; set; }
    }
    public interface I_RelationshipControlManagerOne : I_RelationshipControlManagerGeneral
    {
        I_View_Area GetView();
    }
    public interface I_RelationshipControlManagerMultiple : I_RelationshipControlManagerGeneral
    {
        //     I_UIElementManager GetDataViewUIElement();
        I_View_TemporaryView GetView(object dataItem);
        //I_View_TemporaryView GetTemporaryView(object dataItem);

        //     event EventHandler<Arg_MultipleTemporaryDisplayViewRequested> TemporaryViewRequested;
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
        public I_UIControlManager LabelControlManager { get; set; }

        public BaseColumnControl()
        {
            //   ColumnSetting = new ColumnSetting();
        }
        //     public bool IsPermanentReadOnly { get; set; }
        //public ColumnSetting ColumnSetting { set; get; }
        //   public bool Visited { set; get; }
        public string Alias { set; get; }
        //public I_ControlManager ControlManager
        //{
        //    get
        //    {
        //        {

        //        }
        //    }
        public SecurityAction Permission { get; set; }
        public EntityUICompositionDTO UICompositionDTO { set; get; }

    }
    public abstract class SimpleColumnControlGenerel : BaseColumnControl
    {
        // public I_UIControlManager LabelControlManager { get; set; }
        public ColumnDTO Column { set; get; }
        public abstract I_SimpleControlManagerGeneral SimpleControlManagerGeneral
        {
            get;
        }
    }
    public class SimpleColumnControlOne : SimpleColumnControlGenerel
    {
        public I_SimpleControlManagerOne SimpleControlManager { get; set; }
        public override I_SimpleControlManagerGeneral SimpleControlManagerGeneral
        {
            get { return SimpleControlManager; }
        }

    }
    public class SimpleColumnControlMultiple : SimpleColumnControlGenerel
    {
        public I_SimpleControlManagerMultiple SimpleControlManager { get; set; }
        public override I_SimpleControlManagerGeneral SimpleControlManagerGeneral
        {
            get { return SimpleControlManager; }
        }

    }
    public abstract class RelationshipColumnControlGeneral : BaseColumnControl
    {
        //public event EventHandler<ChildRelationshipInfo> DataViewForTemporaryViewShown;
        public I_EditEntityArea ParentEditArea { set; get; }
        public RelationshipDTO Relationship { get { return DataEntryRelationship.Relationship; } }
        //    public List<ColumnDTO> RelationshipColumns { set; get; }
        public DataEntryRelationshipDTO DataEntryRelationship { set; get; }
        public I_EditEntityArea GenericEditNdTypeArea { set; get; }

        //public void OnDataViewForTemporaryViewShown(ChildRelationshipInfo ChildRelationshipInfo)
        //{
        //    if (DataViewForTemporaryViewShown != null)
        //        DataViewForTemporaryViewShown(this, ChildRelationshipInfo);
        //}
        //   public I_RelationshipControlManagerGeneral RelationshipControlManagerGeneral { set; get; }
    }
    public class RelationshipColumnControlOne : RelationshipColumnControlGeneral
    {
        public I_RelationshipControlManagerOne RelationshipControlManager
        {
            set; get;
        }

        //   public I_EditEntityArea EditNdTypeArea { set; get; }

        //   public  I_EditEntityArea GenericEditNdTypeArea { get; set; }


    }
    public class RelationshipColumnControlMultiple : RelationshipColumnControlGeneral
    {
        public I_RelationshipControlManagerMultiple RelationshipControlManager
        {
            set; get;
        }

        //       public I_EditEntityArea EditNdTypeArea { set; get; }


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
    //public class UIControlComposition
    //{
    //    public UIControlComposition()
    //    {
    //        ChildItems = new List<UIControlComposition>();
    //    }
    //    public object Container { set; get; }
    //    public object Item { set; get; }
    //    public object UIItem { set; get; }
    //    public EntityUICompositionDTO UIComposition { set; get; }
    //    public List<UIControlComposition> ChildItems { set; get; }
    //    public UIControlComposition ParentItem { set; get; }
    //}


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








    public class ParentRelationshipInfo : ParentRelationshipData

    {
        public ParentRelationshipInfo(ChildRelationshipInfo parantChildRelationshipInfo) : base(parantChildRelationshipInfo)
        {
            RelationshipColumnControl = parantChildRelationshipInfo.RelationshipControl;
            //ParantChildRelationshipInfo = parantChildRelationshipInfo;
        }
        public new DP_FormDataRepository SourceData { get { return ParantChildRelationshipData.SourceData as DP_FormDataRepository; } }
        public ChildRelationshipInfo ParantChildRelationshipInfo { get { return ParantChildRelationshipData as ChildRelationshipInfo; } }
        public RelationshipColumnControlGeneral RelationshipColumnControl { set; get; }
        //public ChildRelationshipInfo ParantChildRelationshipInfo { set; get; }
        //  public int RelationshipID { get { return ParantChildRelationshipInfo.Relationship.PairRelationshipID; } }
        // public RelationshipDTO ToRelationship { get { return ParantChildRelationshipInfo.Relationship.PairRelationship; } }
        //   public DP_FormDataRepository SourceData { get { return ParantChildRelationshipInfo.SourceData as DP_FormDataRepository; } }
        //public bool IsHidden { get { return IsHiddenOnState || IsHiddenOnShow; } }
        //public bool IsHiddenOnState { get; set; }
        //public bool IsHiddenOnShow { get; set; }
        //public bool IsReadonly { get { return IsReadonlyOnState || IsReadonlyOnShow; } }
        //public bool IsReadonlyOnState { get; set; }
        //public bool IsReadonlyOnShow { get; set; }
    }
    public class ChangeMonitor
    {
        public ChangeMonitor()
        {

        }
        public DP_FormDataRepository SourceData { set; get; }
        public string GeneralKey { set; get; }
        public string UsageKey { set; get; }
        public DP_FormDataRepository DataToCall { set; get; }
        public int columnID { set; get; }
        public string RestTail { set; get; }
    }
}
