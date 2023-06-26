

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
    public class EditEntityAreaOneData : BaseEditEntityArea, I_EditEntityAreaOneData
    {

        //StateHelper stateHelper = new StateHelper();
        public EditEntityAreaOneData(TableDrivedEntityDTO simpleEntity) : base(simpleEntity)
        {
            //     UIControlPackageTree = new List<UIControlComposition>();
            //SimpleColumnControls = new List<SimpleColumnControl>();
            //RelationshipColumnControls = new List<RelationshipColumnControl>();
        }

        //event EventHandler<List<DP_FormDataRepository>> I_EditEntityAreaOneData.DataItemSelected
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}


        //public void OnDataItemSelected(DP_FormDataRepository DP_FormDataRepository)
        //{

        //}

        public override void GenerateUIControlsByCompositionDTO(EntityUICompositionDTO UICompositions)
        {
            //** EditEntityAreaOneData.GenerateUIControlsByCompositionDTO: ad8b0609f380
            I_View_GridContainer container = DataView;
            //container.ClearControls();
            //SimpleColumnControls.Clear();
            //RelationshipColumnControls.Clear();
            GenerateUIComposition(UICompositions, container);
        }

        private void GenerateUIComposition(EntityUICompositionDTO UICompositions, I_View_GridContainer container)
        {
            //**49f4821e-b752-4cdd-87ca-45dffc84c044
            foreach (var uiCompositionItem in UICompositions.ChildItems.OrderBy(x => x.Position))
            {
                // UIControlComposition item = new UIControlComposition();
                //  item.ParentItem = parentUIControlComposition;
                uiCompositionItem.Container = container;

                if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Group)
                {
                    var groupItem = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateGroup(uiCompositionItem.GroupUISetting);
                    uiCompositionItem.Item = groupItem;
                    uiCompositionItem.UIItem = groupItem.UIMainControl;
                    groupItem.UICompositionDTO = uiCompositionItem;
                    GenerateUIComposition(uiCompositionItem, groupItem);
                    (container as I_View_GridContainer).AddGroup(groupItem, uiCompositionItem.Title, uiCompositionItem.GroupUISetting);
                    //if (item.ChildItems.Count != 0)
                    //{
                    //    
                    //    parentUIControlComposition.ChildItems.Add(item);
                    //}
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    var tabGroupContainer = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabGroup(uiCompositionItem.TabGroupUISetting);
                    uiCompositionItem.Item = tabGroupContainer;
                    uiCompositionItem.UIItem = tabGroupContainer.UIMainControl;
                    tabGroupContainer.UICompositionDTO = uiCompositionItem;

                    GenerateUIComposition(uiCompositionItem, null);
                    (container as I_View_GridContainer).AddTabGroup(tabGroupContainer, uiCompositionItem.Title, uiCompositionItem.TabGroupUISetting);
                    //if (item.ChildItems.Count != 0)
                    //{
                    //    
                    //    parentUIControlComposition.ChildItems.Add(item);
                    //}
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.TabPage)
                {
                    var tabPage = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabPage(uiCompositionItem.TabPageUISetting);
                    uiCompositionItem.Item = tabPage;
                    uiCompositionItem.UIItem = tabPage.UIMainControl;
                    tabPage.UICompositionDTO = uiCompositionItem;
                    GenerateUIComposition(uiCompositionItem, tabPage);

                    var parentTabGroupContainer = UICompositions.Item as I_TabGroupContainer;
                    parentTabGroupContainer.AddTabPage(tabPage, uiCompositionItem.Title, uiCompositionItem.TabPageUISetting, tabPage.HasHeader);


                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Column)
                {
                    var propertyControl = new SimpleColumnControlOne(UIManager, uiCompositionItem);

                    SimpleColumnControls.Add(propertyControl);

                    uiCompositionItem.Item = propertyControl.SimpleControlManager;
                    uiCompositionItem.UIItem = propertyControl.SimpleControlManager.GetUIControlManager().GetUIControl();
                    propertyControl.UICompositionDTO = uiCompositionItem;

                    (container as I_View_GridContainer).AddUIControlPackage(propertyControl.SimpleControlManager, propertyControl.LabelControlManager);

                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Relationship)
                {
                    var editArea = GenerateRelationshipControlEditArea(uiCompositionItem.Relationship);
                    //اینجا ادیت اریا رابطه و همچنین کنترل منیجر رابطه مشخص میشوند. اگر مثلا کاربر به موجودیت رابطه دسترسی نداشته باشد این مقادیر تولید نمی شوند و نال بر میگردد

                    if (editArea != null)
                    {

                        var relationshipColumnControl = new RelationshipColumnControlOne(UIManager, uiCompositionItem, this as I_EditEntityArea, editArea);
                        RelationshipColumnControls.Add(relationshipColumnControl);

                        if (UICompositions.ChildItems.Count == 1 && UICompositions.Item is I_TabPageContainer)
                        {
                            relationshipColumnControl.RelationshipControlManager.TabPageContainer = UICompositions.Item as I_TabPageContainer;
                        }
                        uiCompositionItem.Item = relationshipColumnControl.RelationshipControlManager;
                        uiCompositionItem.UIItem = relationshipColumnControl.RelationshipControlManager.GetView();
                        relationshipColumnControl.UICompositionDTO = uiCompositionItem;
                        (container as I_View_GridContainer).AddView(relationshipColumnControl.LabelControlManager, relationshipColumnControl.RelationshipControlManager);
                    }
                    //    parentUIControlComposition.ChildItems.Add(item);
                    //   columnControl.Visited = true;

                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                {
                    var setting = uiCompositionItem.EmptySpaceUISetting;
                    if (setting == null)
                    {
                        setting = new EmptySpaceUISettingDTO();
                        setting.ExpandToEnd = false;
                        setting.UIColumnsType = Enum_UIColumnsType.Normal;
                    }
                    //var emptyItem = SpecializedDataView.GenerateEmptySpace(setting);
                    //item.Item = emptyItem;
                     (container as I_View_GridContainer).AddEmptySpace(setting);
                    //parentUIControlComposition.ChildItems.Add(item);
                }


            }




        }

        public void CheckContainersVisiblity(List<BaseColumnControl> changedControls)
        {
            //EditEntityAreaOneData.CheckContainersVisiblity: 29dc4e65ce6f
            if (this is I_EditEntityAreaOneData)
            {
                List<EntityUICompositionDTO> parentAsContainers = new List<EntityUICompositionDTO>();
                foreach (var item in changedControls)
                {
                    if (item.UICompositionDTO != null
                        && item.UICompositionDTO.ParentItem != null)
                    {
                        if (!parentAsContainers.Any(x => x == item.UICompositionDTO.ParentItem))
                            parentAsContainers.Add(item.UICompositionDTO.ParentItem);
                    }
                }
                if (parentAsContainers.Any())
                {
                    foreach (var container in parentAsContainers)
                    {
                        CheckContainerVisiblity(container);
                    }
                }
            }
        }
        public void CheckContainerVisiblity(EntityUICompositionDTO container)
        {
            if (container.ChildItems.Any(x => x.UIItem != null && AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ControlIsVisible(x.UIItem)))
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.SetContaierVisiblity(container.UIItem, true);
            }
            else
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.SetContaierVisiblity(container.UIItem, false);
            }
            if (container.ParentItem != null)
                CheckContainerVisiblity(container.ParentItem);
        }
        //public override void DataItemVisiblity(object dataItem, bool visible)
        //{
        //    (DataViewGeneric as I_View_EditEntityAreaDataView).Visiblity(visible);
        //}
        //public override void DataItemEnablity(object dataItem, bool visible)
        //{
        //    (DataViewGeneric as I_View_EditEntityAreaDataView).Visiblity(visible);
        //}
        //public I_View_EditEntityAreaDataView SpecializedDataView
        //{
        //    get
        //    {
        //        return DataViewGeneric as I_View_EditEntityAreaDataView;
        //    }
        //}
        public I_View_EditEntityAreaDataView DataView { set; get; }

        //      List<UIControlComposition> I_EditEntityAreaOneData.UIControlPackageTree { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //   I_View_EditEntityAreaDataView I_EditEntityAreaOneData.SpecializedDataView => throw new NotImplementedException();


        //public override I_View_Area DataViewGeneric
        //{
        //    get { return DataView; }
        //}

        public void CreateDefaultData()
        {
            //EditEntityAreaOneData.CreateDefaultData: f089224255d6
            bool shouldCreatData = true;

            if (this.SourceRelationColumnControl != null)
                throw new Exception("asvvvb");

            //if (DataEntryEntity.IsReadonly)
            //{
            //    shouldCreatData = false;
            //}
            //if (SourceRelationColumnControl != null)
            //{
            //    if (SourceRelationColumnControl.Relationship.IsReadonly)
            //        shouldCreatData = false;
            //}
            if (shouldCreatData)
                shouldCreatData = GetDataList().Count == 0;

            if (shouldCreatData)
            {
                var newData = AgentHelper.CreateAreaInitializerNewData(this, true);


                //if (DataEntryEntity.IsReadonly)
                //{
                //    newData.IsUseLessBecauseNewAndReadonly = true;

                //    //////foreach (var property in newData.ChildSimpleContorlProperties)
                //    //////{
                //    //////    property.AddReadonlyState("", "DataNewAndReadonly", true);
                //    //////}
                //    //////foreach (var rel in newData.ChildRelationshipDatas)
                //    //////{
                //    //////    rel.AddReadonlyState("", "DataNewAndReadonly", true);
                //    //////}
                //}
                AddData(newData);

                ////var addResult = AddData(newData);
                ////if (!addResult)
                ////    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده پیش فرض و یا داده های وابسته", newData.ViewInfo, Temp.InfoColor.Red);


                //else
                //{

                //}
                //if (SourceRelationColumnControl != null)
                //{
                //    if (DataView != null)
                //    {
                //        //////SpecializedDataView.DisableEnableDataSection(true);
                //        //////if (AreaInitializer.BusinessReadOnlyByParent || AreaInitializer.ParentDataItemBusinessReadOnly.Any(x => x == ChildRelationshipInfo.SourceData) || AreaInitializer.SecurityReadOnlyByParent || AreaInitializer.SecurityReadOnly)
                //        //////{
                //        //////    SpecializedDataView.DisableEnableDataSection(false);
                //        //////}
                //    }
                //}
            }
        }

        //private void DecideDataSectionEnablity()
        //{
        //    var data = GetDataList();
        //    bool enablity = true;
        //    if (data.Any(x => x.IsUseLessBecauseNewAndReadonly))
        //        enablity = false;
        //    DataViewGeneric.DisableEnableDataSection(enablity);
        //}

        //صدا زده میشود RelationData تنها بوسیله ShowData برای نمایش داده های اضافه شده در آن صدا زده میشود.در مابقی موارد ShowData که AddData فقط یکجا مشخص میشود و آنهم در specificDate
        //public override bool ShowDataInDataView(DP_FormDataRepository specificDate)
        //{

        //    if (!specificDate.IsFullData)
        //        throw new Exception("asdasd");

        //    //اینجا میشه از خود ChildSimpleContorlProperty استفاده کرد
        //    foreach (var  propertyControl in specificDate.ChildSimpleContorlProperties)
        //    {
        //        propertyControl.SetBinding();
        //        //var property = specificDate.GetProperty(propertyControl.Column.ID);
        //        //if (property != null)
        //        //{

        //        //    //////ShowTypePropertyControlValue(specificDate, propertyControl, property.Value);
        //        //    //if (propertyControl.Column.ColumnValueRange != null && propertyControl.Column.ColumnValueRange.Details.Any())
        //        //    //{
        //        //    //    var columnKeyValue = propertyControl.Column.ColumnValueRange;
        //        //    //    CheckItemsSourceAndPropertyValue(propertyControl, specificDate);
        //        //    //}
        //        //    SetBinding(specificDate, propertyControl, property);

        //        //}
        //        //else
        //        //{
        //        //    //????
        //        //}
        //    }
        //    bool result = true;
        //    //جدید--دقت شود که اگر نمایش مستقیم نیست داخل فرم رابطه ای نباید همه کنترلها مقداردهی شوند
        //    foreach (var relationshipControl in specificDate.ChildRelationshipDatas)
        //    {
        //        relationshipControl.SetBinding();
        //    }
        //   //     foreach (var relationshipControl in RelationshipColumnControls)
        //   // {
        //   //     bool relationshipFirstSideHasValue = relationshipControl.Relationship.RelationshipColumns.Any()
        //   //         && relationshipControl.Relationship.RelationshipColumns.All(x => specificDate.GetProperties().Any(y => !AgentHelper.ValueIsEmpty(y) && y.ColumnID == x.FirstSideColumnID));

        //   //     //relationshipControl.EditNdTypeArea.SourceRelationColumnControl.RelatedData = specificDate;
        //   //     //اینجا یکارایی بشه دسترسی موقت

        //   ////     bool childLoadedBefore = specificDate.ChildRelationshipDatas.Any(x => x.Relationship.ID == relationshipControl.Relationship.ID);

        //   //     ChildRelationshipInfo childData = null;
        //   //     if (childLoadedBefore)
        //   //         childData = specificDate.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
        //   //     else
        //   //     {
        //   //         if (!relationshipFirstSideHasValue)
        //   //         {
        //   //             childData = specificDate.AddChildRelationshipInfo(relationshipControl);
        //   //         }
        //   //         else
        //   //         {
        //   //             bool childIsDataView = (relationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
        //   //                                    relationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect);
        //   //             if (childIsDataView)
        //   //                 childData = AreaInitializer.EditAreaDataManager.SerachDataFromParentRelationForChildDataView(relationshipControl.Relationship, this, relationshipControl.GenericEditNdTypeArea, relationshipControl, specificDate);
        //   //             else
        //   //                 childData = AreaInitializer.EditAreaDataManager.SerachDataFromParentRelationForChildTempView(relationshipControl.Relationship, this, relationshipControl.GenericEditNdTypeArea, relationshipControl, specificDate);
        //   //         }
        //   //     }
        //   //     if (childData.SecurityIssue == false)
        //   //     {

        //   //         //    if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
        //   //         //relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
        //   //         //    {
        //   //         //        if (relationshipControl.EditNdTypeArea.TemporaryDisplayView != null)
        //   //         //            relationshipControl.EditNdTypeArea.TemporaryDisplayView.DisableEnable(TemporaryLinkType.DataView, true);

        //   //         //        if (relationshipControl.EditNdTypeArea.TemporaryDisplayView != null)
        //   //         //        {
        //   //         //            if (relationshipControl.EditNdTypeArea.SecurityReadOnlyByParent|| relationshipControl.EditNdTypeArea.AreaInitializer.SecurityReadOnly )
        //   //         //            {
        //   //         //                if (!childData.RelatedData.Any())
        //   //         //                {

        //   //         //                    //اگر مستقیم بود چی..اصن نباید دیتای دیفالت تولید بشه .درست شود
        //   //         //                    relationshipControl.EditNdTypeArea.TemporaryDisplayView.DisableEnable(TemporaryLinkType.DataView, false);

        //   //         //                }
        //   //         //            }
        //   //         //        }
        //   //         //    }

        //   //         var childResult = relationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfoAndShow(childData);
        //   //         if (!childResult)
        //   //             result = false;
        //   //     }
        //   //     else
        //   //         result = false;
        //   // }
        //    if (result)
        //        OnDataItemShown(new EditAreaDataItemLoadedArg() { DataItem = specificDate, InEditMode = true });
        //    //    CheckRelationshipReadonlyEnablity();

        //    //DecideDataSectionEnablity();

        //    return result;


        //    //else

        //}
        //private void CheckRelationshipReadonlyEnablity()
        //{

        //}
        //اونی که کلید نداره قتی اول میشه موقع آپدیت اونی که کلید داره مقدار موجود اولیو نمیگیره؟؟

        //تنها یکبار و برای نمایش مقادیر در کنترلهای ساده و غیر فرمی صدا زده میشود
        //public bool ShowTypePropertyControlValue(DP_FormDataRepository dataItem, SimpleColumnControl typePropertyControl, string value)
        //{
        //    return typePropertyControl.SetValue(value);

        //}
        //public void SetBinding(DP_FormDataRepository dataItem, SimpleColumnControlOne simpleColumnControl, EntityInstanceProperty property)
        //{
        //    //اینجا 
        //    //بایندینگ های قبلی نمیمونن مثلا برای فرم های چندتایی که با یک تکی ارتباط دارد//
        //    //تکست را عوض کنیم بایند قبلی خصوصیت مقدارش تغییر کند
        //    //dataItem.AddChildSimpleContorlProperty(simpleColumnControl, property);
        //    simpleColumnControl.SimpleControlManager.GetUIControlManager().SetBinding(property);
        //}






    }



}
