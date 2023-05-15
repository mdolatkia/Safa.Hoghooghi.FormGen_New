

using CommonDefinitions.UISettings;
using ModelEntites;

using MyUILibrary;
using MyUILibrary.EntityArea.Commands;
using MyUILibraryInterfaces.DataViewArea;
using MyUILibraryInterfaces.EditEntityArea;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUILibraryInterfaces.DataReportArea;
using System.Collections.Specialized;
using MyUILibrary.Temp;

namespace MyUILibrary.EntityArea
{
    public class EditEntityAreaMultipleData : BaseEditEntityArea, I_EditEntityAreaMultipleData
    {



        public event EventHandler<DataUpdatedArg> Updated;
        public event EventHandler<EditAreaDataItemArg> DataItemRemoved;
        public EditEntityAreaMultipleData(TableDrivedEntityDTO simpleEntity) : base(simpleEntity)
        {
            //SimpleColumnControls = new List<SimpleColumnControl>();
            //RelationshipColumnControls = new List<RelationshipColumnControl>();
        }


        //private void View_ButtonClicked(object sender, ConfirmModeClickedArg e, List<DP_FormDataRepository> deleteDataList)
        //{
        //    I_ViewDeleteInquiry view = sender as I_ViewDeleteInquiry;
        //    if (view != null)
        //    {
        //        if (e.Result == UserDialogResult.Ok || e.Result == UserDialogResult.No)
        //        {

        //        }
        //        else if (e.Result == UserDialogResult.Yes)
        //        {
        //            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(view);
        //            foreach (var item in deleteDataList)
        //            {
        //                if (ChildRelationshipInfo != null)
        //                {
        //                    //SetRemovedItem(ChildRelationshipInfo, item, true);
        //                }
        //                RemoveData(item, true);
        //            }
        //        }
        //    }
        //}


        //public override bool AddData(DP_FormDataRepository data, bool showDataInDataView)
        //{
        //    if (base.BaseAddData(data))
        //    {
        //        if (showDataInDataView)
        //        {
        //            if (ShowDataInDataView(data))
        //                return true;
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //    //ManageDataSecurity();
        //}
        public void RemoveDataContainer(DP_FormDataRepository dataItem)
        {
            if (DataViewGeneric != null)
                (DataViewGeneric as I_View_EditEntityAreaMultiple).RemoveDataContainer(dataItem);
            //if (DataItemRemoved != null)
            //    DataItemRemoved(this, new EditAreaDataItemArg() { DataItem = dataItem });
        }
        public void RemoveDataContainers()
        {
            if (DataViewGeneric != null)
            {
                var list = (DataViewGeneric as I_View_EditEntityAreaMultiple).RemoveDataContainers();
                //if (DataItemRemoved != null)
                //{
                //    foreach (var item in list)
                //        DataItemRemoved(this, new EditAreaDataItemArg() { DataItem = item as DP_FormDataRepository });
                //}
            }
        }
        //public bool ShowDatasInDataView(List<DP_FormDataRepository> dataItems)
        //{
        //    bool result = true;
        //    foreach (var item in dataItems)
        //    {
        //        var itemResult = ShowDataInDataView(item);
        //        if (!itemResult)
        //            result = false;
        //    }
        //    return result;
        //}
        //public override bool ShowDataInDataView(DP_FormDataRepository specificDate)
        //{
        //    (DataViewGeneric as I_View_EditEntityAreaMultiple).AddDataContainer(specificDate);
        //    //  return InternalShowDataInDataView(dataItem);

        //    if (!specificDate.IsFullData)
        //        throw new Exception("asdasd");

        //    foreach (var propertyControl in specificDate.ChildSimpleContorlProperties)
        //    {
        //        propertyControl.SetBinding();
        //    }
        //    //foreach (var propertyControl in SimpleColumnControls)
        //    //{
        //    //    var property = specificDate.GetProperty(propertyControl.Column.ID);
        //    //    if (property != null)
        //    //    {
        //    //        //if (AreaInitializer.IntracionMode == IntracionMode.Create
        //    //        //                           || AreaInitializer.IntracionMode == IntracionMode.CreateSelect)
        //    //        //{

        //    //        //if (propertyControl.Column.ColumnValueRange != null && propertyControl.Column.ColumnValueRange.Details.Any())
        //    //        //{
        //    //        //    var columnKeyValue = propertyControl.Column.ColumnValueRange;
        //    //        //    if (!string.IsNullOrEmpty(property.Value))
        //    //        //    {
        //    //        //        if (columnKeyValue.ValueFromTitleOrValue)
        //    //        //        {
        //    //        //            if (!columnKeyValue.Details.Any(x => x.KeyTitle == property.Value))
        //    //        //                property.Value = "";
        //    //        //        }
        //    //        //        else
        //    //        //        {
        //    //        //            if (!columnKeyValue.Details.Any(x => x.Value == property.Value))
        //    //        //                property.Value = "";
        //    //        //        }
        //    //        //    }
        //    //        //}

        //    //        SetBinding(specificDate, propertyControl as SimpleColumnControlMultiple, property);
        //    //        //ShowTypePropertyControlValue(specificDate, propertyControl, property.Value);
        //    //        //}
        //    //    }
        //    //    else
        //    //    {
        //    //        //????
        //    //    }
        //    //}
        //    bool result = true;
        //    foreach (var relationshipControl in specificDate.ChildRelationshipInfos)
        //    {
        //        relationshipControl.SetBinding();
        //        // bool relationshipFirstSideHasValue = relationshipControl.Relationship.RelationshipColumns.Any()
        //        //&& relationshipControl.Relationship.RelationshipColumns.All(x => specificDate.GetProperties().Any(y => y.Value != null && !string.IsNullOrEmpty(y.Value.ToString()) && y.ColumnID == x.FirstSideColumnID));

        //        // bool childLoadedBefore = specificDate.ChildRelationshipInfos.Any(x => x.Relationship.ID == relationshipControl.Relationship.ID);
        //        // ChildRelationshipInfo childData = null;
        //        // if (childLoadedBefore)
        //        //     childData = specificDate.ChildRelationshipInfos.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
        //        // else
        //        // {
        //        //     if (!relationshipFirstSideHasValue)
        //        //         childData = specificDate.AddChildRelationshipInfo(relationshipControl);
        //        //     else
        //        //         childData = AreaInitializer.EditAreaDataManager.SerachDataFromParentRelationForChildTempView(relationshipControl.Relationship, this, relationshipControl.GenericEditNdTypeArea, relationshipControl, specificDate);

        //        // }
        //        // if (childData.SecurityIssue == false)
        //        // {
        //        //     relationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfoAndShow(childData);

        //        //     //if (relationshipControl.EditNdTypeArea.SecurityReadOnlyByParent || relationshipControl.EditNdTypeArea.AreaInitializer.SecurityReadOnly)
        //        //     //    if (!childData.RelatedData.Any())
        //        //     //    {
        //        //     //        relationshipControl.View.DisableEnable(specificDate, TemporaryLinkType.DataView, false);
        //        //     //    }
        //        // }
        //        // else
        //        // {
        //        //     result = false;
        //        // }
        //    }
        //    if (result)
        //        OnDataItemShown(new EditAreaDataItemLoadedArg() { DataItem = specificDate, InEditMode = true });
        //    //    OnDataItemLoaded(new EditAreaDataItemLoadedArg() { DataItem = specificDate, InEditMode = true });
        //    return result;

        //}
        //بعدا بررسی شود
        //private bool InternalShowDataInDataView(DP_FormDataRepository specificDate)
        //{

        //}
        //باشد Direct صدا زده میشود ، اگر LoadTemplate یکبار در
        //.باشد AreaInitializer.FormComposed == false کلیک میشود و اگر Data زمانی که لینک ShowTemporaryDataView یکبار هم در
        //public void ShowDataFromExternalSource(List<DP_DataView> dataRepositories)
        //{


        //    //if (dialog)
        //    //{
        //    //    if (dataView)
        //    //        AgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(DataView, SimpleEntity.Alias, Enum_WindowSize.Big);
        //    //    else
        //    //        AgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(TemporaryDisplayView, SimpleEntity.Alias, Enum_WindowSize.Big);
        //    //}
        //    //else
        //    //{
        //    //    if (dataView)
        //    //        AgentUICoreMediator.UIManager.ShowPane(DataView, SimpleEntity.Alias);
        //    //    else
        //    //        AgentUICoreMediator.UIManager.ShowPane(TemporaryDisplayView, SimpleEntity.Alias);
        //    //}
        //}
        public override I_View_Area DataViewGeneric
        {
            get { return DataView; }
        }
        public override void GenerateUIControlsByCompositionDTO(EntityUICompositionDTO UICompositions)
        {
            //**  EditEntityAreaMultipleData.GenerateUIControlsByCompositionDTO: 3c156446abf7
            //DataView.ClearControls();
            //SimpleColumnControls.Clear();
            //RelationshipColumnControls.Clear();
            // GenerateUIComposition(UICompositions);

            foreach (var uiCompositionItem in UICompositions.ChildItems.OrderBy(x => x.Position))
            {
                if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Entity)
                {
                    GenerateUIControlsByCompositionDTO(uiCompositionItem);
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Group)
                {
                    GenerateUIControlsByCompositionDTO(uiCompositionItem);
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    GenerateUIControlsByCompositionDTO(uiCompositionItem);
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.TabPage)
                {
                    GenerateUIControlsByCompositionDTO(uiCompositionItem);
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Column)
                {
                    //    var column = uiCompositionItem.Column;
                    //bool hasRangeOfValues = column.ColumnValueRange != null && column.ColumnValueRange.Details.Any();

                    var propertyControl = new SimpleColumnControlMultiple(UIManager, uiCompositionItem);
                    //  AgentHelper.SetPropertyTitle(propertyControl);

                    //    var info = column.ID + "," + column.Name;

                    SimpleColumnControls.Add(propertyControl);


                    DataView.AddUIControlPackage(propertyControl.SimpleControlManager, propertyControl.LabelControlManager);
                    //     columnControl.Visited = true;

                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Relationship)
                {
                    var editArea = GenerateRelationshipControlEditArea(uiCompositionItem.Relationship, uiCompositionItem.Relationship.Relationship);

                    // relationshipColumnControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(uiCompositionItem.Relationship.Relationship.Alias);
                    //  relationshipColumnControl.DataEntryRelationship = uiCompositionItem.Relationship;
                    //   relationshipColumnControl.ParentEditArea = this;
                    //  var relationshipAlias = relationshipColumnControl.Relationship.Alias;
                    //  relationshipColumnControl.Alias = (string.IsNullOrEmpty(relationshipAlias) ? "" : relationshipAlias + " : ");

                    //اینجا ادیت اریا رابطه و همچنین کنترل منیجر رابطه مشخص میشوند. اگر مثلا کاربر به موجودیت رابطه دسترسی نداشته باشد این مقادیر تولید نمی شوند و نال بر میگردد

                    //  bool generated = GenerateRelationshipControlEditArea(relationshipColumnControl, uiCompositionItem.RelationshipUISetting);


                    if (editArea != null)
                    {
                        var relationshipColumnControl = new RelationshipColumnControlMultiple(UIManager, uiCompositionItem, this as I_EditEntityArea, editArea);
                        RelationshipColumnControls.Add(relationshipColumnControl);
                        DataView.AddView(relationshipColumnControl.LabelControlManager, relationshipColumnControl.RelationshipControlManager);
                    }

                }
                //حالت تب اضافه شود
                //uiControlPackageList.Add(item);
                //if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Group)
                //{

                //}

            }

        }
        //private void GenerateUIComposition(EntityUICompositionDTO UICompositions)
        //{
        //    //dbc86c4272eb

        //}


        //private void SearchViewEntityArea_DataSelected(object sender, DataSelectedEventArg e)
        //{
        //    foreach (var data in e.DataItem)
        //    {
        //        DP_FormDataRepository result = null;
        //        if (SearchViewEntityArea.IsCalledFromDataView)
        //        {
        //            result = AreaInitializer.EditAreaDataManager.GetFullDataFromDataViewSearch(AreaInitializer.EntityID, data, this);

        //        }
        //        else
        //        {
        //            result = AreaInitializer.EditAreaDataManager.ConvertDP_DataViewToDP_DataRepository(data, this);
        //        }

        //        if (result != null)
        //        {
        //            bool addResult = AddData(result, SearchViewEntityArea.IsCalledFromDataView);
        //            if (!addResult)
        //                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", data.ViewInfo, Temp.InfoColor.Red);

        //        }
        //        else
        //        {
        //            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", e.DataItem.First().ViewInfo, Temp.InfoColor.Red);
        //        }
        //    }
        //}
        public I_View_EditEntityAreaMultiple DataView { set; get; }

        //public I_View_EditEntityAreaMultiple SpecializedDataView
        //{
        //    get
        //    {
        //        return DataViewGeneric as I_View_EditEntityAreaMultiple;
        //    }
        //}
        //public override void DataItemVisiblity(object dataItem, bool visible)
        //{
        //    (DataViewGeneric as I_View_EditEntityAreaMultiple).Visiblity(dataItem, visible);
        //}
        //public override void DataItemEnablity(object dataItem, bool visible)
        //{
        //    (DataViewGeneric as I_View_EditEntityAreaMultiple).EnableDisable(dataItem, visible);
        //}
        //private I_SearchViewEntityArea GenerateSearchViewArea()
        //{
        //    if (AreaInitializer.SourceRelationColumnControl != null)
        //        throw new Exception("asdasd");
        //    var searchViewEntityArea = new SearchViewEntityArea();
        //    var searchViewInit = new SearchViewAreaInitializer();
        //    searchViewInit.SourceEditArea = this;
        //    // searchViewInit.TempEntity = FullEntity;
        //    searchViewInit.EntityID = AreaInitializer.EntityID;
        //    searchViewInit.MultipleSelection = true;
        //    searchViewEntityArea.SetAreaInitializer(searchViewInit);
        //    searchViewEntityArea.DataSelected += SearchViewEntityArea_DataSelected;
        //    return searchViewEntityArea;
        //}

        public List<DP_FormDataRepository> GetSelectedData()
        {
            List<DP_FormDataRepository> selectedData = (DataViewGeneric as I_View_EditEntityAreaMultiple).GetSelectedData().Cast<DP_FormDataRepository>().ToList();
            return selectedData;
        }


        //public void SetBinding(DP_FormDataRepository dataItem, SimpleColumnControlMultiple typePropertyControl, EntityInstanceProperty property)
        //{
        //    var uiControl = typePropertyControl.SimpleControlManager.GetUIControlManager(dataItem);
        //    if (uiControl != null)
        //        typePropertyControl.SimpleControlManager.GetUIControlManager(dataItem).SetBinding(property);
        //}

        //public bool ShowTypePropertyControlValue(DP_FormDataRepository dataItem, SimpleColumnControlMultiple typePropertyControl, string value)
        //{

        //    //ColumnSetting columnSetting = new ColumnSetting();

        //    //if (typePropertyControl.Column.PrimaryKey == true && (dataItem != null && !dataItem.IsNewItem))
        //    //{
        //    //    columnSetting.IsReadOnly = true;
        //    //}
        //    //else
        //    //    columnSetting.IsReadOnly = typePropertyControl.ColumnSetting.IsReadOnly;
        //    //بهتره جور دیگه نوشته بشه
        //    //if (typePropertyControl.ControlPackage != null)
        //    return typePropertyControl.SimpleControlManager.GetUIControlManager(dataItem).SetValue(value);
        //    //else
        //    //    return typePropertyControl.SetValue(value);
        //    //AgentUICoreMediator.UIManager.ShowControlValue(typePropertyControl.ControlPackage, typePropertyControl.Column, value, columnSetting);

        //}



        //public object FetchTypePropertyControlValue(DP_FormDataRepository dataRepository, SimpleColumnControlMultiple SimpleColumnControlMultipleData)
        //{
        //    //if (AreaInitializer.DataMode == DataMode.Multiple)
        //    return SimpleColumnControlMultipleData.SimpleControlManager.GetUIControlManager(dataRepository).GetValue();
        //    //else
        //    //    return SimpleColumnControlMultipleData.GetValue();


        //    //////if (typePropertyControl.EditNdTypeArea != null)
        //    //////{
        //    //////    //var relationSourceControl = (typePropertyControl as RelationSourceControl);
        //    //////    if (typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl != null)
        //    //////    {
        //    //////        if (typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.ManyToOne
        //    //////            || typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.ExplicitOneToOne
        //    //////            || typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.SubToSuper
        //    //////            || (typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.UnionToSubUnion_UnionHoldsKeys)
        //    //////            || (typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.SubUnionToUnion_SubUnionHoldsKeys))
        //    //////        {
        //    //////            var data = typePropertyControl.EditNdTypeArea.AreaInitializer.Data.FirstOrDefault(x => x.SourceRelatedData == dataRepository);
        //    //////            //AreaInitializer.DataMode == DataMode.Multiple
        //    //////            //&& 
        //    //////            if (data == null)
        //    //////                return "";
        //    //////            else
        //    //////                return typePropertyControl.EditNdTypeArea.FetchTypePorpertyValue(data, typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipColumns.First().SecondSideColumn1);
        //    //////        }
        //    //////        else if (typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.OneToMany
        //    //////            || typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.ImplicitOneToOne
        //    //////              || typePropertyControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelationshipType == Enum_RelationshipType.SuperToSub)
        //    //////        {
        //    //////            //return FetchTypePorpertyValue(dataRepository, typePropertyControl.Column);
        //    //////            throw (new Exception("asfsdf"));
        //    //////        }
        //    //////    }
        //    //////    return "";


        //    //////}
        //    //////else
        //    //////{

        //    //////}


        //    //if (typePropertyControl is RelationSourceControl)
        //    //{
        //    //    var relationSourceControl = (typePropertyControl as RelationSourceControl);
        //    //    //////return relationSourceControl.EditNdTypeArea.FetchTypePorpertyValue(dataRepository, AgentHelper.GetRelationOperand(relationSourceControl.Relation, relationSourceControl.RelationSide == Enum_DP_RelationSide.FirstSide ? Enum_DP_RelationSide.SecondSide : Enum_DP_RelationSide.FirstSide));
        //    //    return "";
        //    //    // return FetchTypePropertyRelationSourceControl(typePropertyControl as RelationSourceControl);
        //    //}
        //    //else
        //    //    return SpecializedDataView.FetchTypePropertyControlValue(dataRepository, typePropertyControl);
        //}





    }



}
