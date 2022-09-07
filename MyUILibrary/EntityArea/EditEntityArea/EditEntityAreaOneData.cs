

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

        //event EventHandler<EditAreaDataItemArg> I_EditEntityAreaOneData.DataItemSelected
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
            I_View_GridContainer container = DataView;

            GenerateUIComposition(UICompositions, container, UIControlComposition);
        }
       
        private void GenerateUIComposition(EntityUICompositionDTO UICompositions, I_View_GridContainer container, UIControlComposition parentUIControlComposition)
        {
            //**49f4821e-b752-4cdd-87ca-45dffc84c044
            foreach (var uiCompositionItem in UICompositions.ChildItems.OrderBy(x => x.Position))
            {
                UIControlComposition item = new UIControlComposition();
                item.ParentItem = parentUIControlComposition;
                item.Container = container;

                if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Group)
                {
                    var groupItem = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateGroup(uiCompositionItem.GroupUISetting);
                    item.Item = groupItem;
                    item.UIItem = groupItem.UIMainControl;
                    groupItem.UIControlPackageTreeItem = item;
                    GenerateUIComposition(uiCompositionItem, groupItem, item);
                    if (item.ChildItems.Count != 0)
                    {
                        (container as I_View_GridContainer).AddGroup(groupItem, uiCompositionItem.Title, uiCompositionItem.GroupUISetting);
                        parentUIControlComposition.ChildItems.Add(item);
                    }
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    var tabGroupContainer = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabGroup(uiCompositionItem.TabGroupUISetting);
                    tabGroupContainer.UIControlPackageTreeItem = item;
                    item.Item = tabGroupContainer;
                    item.UIItem = tabGroupContainer.UIMainControl;

                    GenerateUIComposition(uiCompositionItem, null, item);
                    if (item.ChildItems.Count != 0)
                    {
                        (container as I_View_GridContainer).AddTabGroup(tabGroupContainer, uiCompositionItem.Title, uiCompositionItem.TabGroupUISetting);
                        parentUIControlComposition.ChildItems.Add(item);
                    }
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.TabPage)
                {
                    var tabPage = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateTabPage(uiCompositionItem.TabPageUISetting);
                    item.Item = tabPage;
                    item.UIItem = tabPage.UIMainControl;
                    tabPage.UIControlPackageTreeItem = item;
                    GenerateUIComposition(uiCompositionItem, tabPage, item);
                    if (item.ChildItems.Count != 0)
                    {
                        var parentTabGroupContainer = parentUIControlComposition.Item as I_TabGroupContainer;
                        parentTabGroupContainer.AddTabPage(tabPage, uiCompositionItem.Title, uiCompositionItem.TabPageUISetting, tabPage.HasHeader);
                        parentUIControlComposition.ChildItems.Add(item);
                    }
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Column)
                {
                    var columnControl = SimpleColumnControls.FirstOrDefault(x => x.Column.ID == Convert.ToInt32(uiCompositionItem.ObjectIdentity)) as SimpleColumnControlOne;
                    if (columnControl != null)
                    {
                        columnControl.UIControlPackageTreeItem = item;
                        item.Item = columnControl.SimpleControlManager;
                        item.UIItem = columnControl.SimpleControlManager.GetUIControlManager().GetUIControl();
                        (container as I_View_GridContainer).AddUIControlPackage(columnControl.SimpleControlManager, columnControl.LabelControlManager);
                        parentUIControlComposition.ChildItems.Add(item);
                        columnControl.Visited = true;
                    }
                }
                else if (uiCompositionItem.ObjectCategory == DatabaseObjectCategory.Relationship)
                {
                    var columnControl = RelationshipColumnControls.FirstOrDefault(x => x.Relationship != null && x.Relationship.ID == Convert.ToInt32(uiCompositionItem.ObjectIdentity)) as RelationshipColumnControlOne;
                    if (columnControl != null)
                    {
                        columnControl.UIControlPackageTreeItem = item;
                        if (UICompositions.ChildItems.Count == 1 && parentUIControlComposition != null && parentUIControlComposition.Item is I_TabPageContainer)
                        {
                            columnControl.RelationshipControlManager.TabPageContainer = parentUIControlComposition.Item as I_TabPageContainer;
                        }
                        item.Item = columnControl.RelationshipControlManager;
                        item.UIItem = columnControl.RelationshipControlManager.GetView();
                        (container as I_View_GridContainer).AddView(columnControl.LabelControlManager, columnControl.RelationshipControlManager);
                        parentUIControlComposition.ChildItems.Add(item);
                        columnControl.Visited = true;
                    }
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
                    parentUIControlComposition.ChildItems.Add(item);
                }
               

            }




        }
        public void CheckContainersVisiblity(List<BaseColumnControl> changedControls)
        {
            if (this is I_EditEntityAreaOneData)
            {
                List<UIControlComposition> parentAsContainers = new List<UIControlComposition>();
                foreach (var item in changedControls)
                {
                    if (item.UIControlPackageTreeItem != null && item.UIControlPackageTreeItem.Container != null
                        && item.UIControlPackageTreeItem.ParentItem != null)
                    {
                        if (!parentAsContainers.Any(x => x == item.UIControlPackageTreeItem.ParentItem))
                            parentAsContainers.Add(item.UIControlPackageTreeItem.ParentItem);
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
        public void CheckContainerVisiblity(UIControlComposition container)
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

        List<UIControlComposition> I_EditEntityAreaOneData.UIControlPackageTree { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        I_View_EditEntityAreaDataView I_EditEntityAreaOneData.SpecializedDataView => throw new NotImplementedException();


        public override I_View_Area DataViewGeneric
        {
            get { return DataView; }
        }

        public void CreateDefaultData()
        {
            bool shouldCreatData = true;

            if (this.AreaInitializer.SourceRelationColumnControl != null)
                throw new Exception("asvvvb");

            //if (DataEntryEntity.IsReadonly)
            //{
            //    shouldCreatData = false;
            //}
            //if (AreaInitializer.SourceRelationColumnControl != null)
            //{
            //    if (AreaInitializer.SourceRelationColumnControl.Relationship.IsReadonly)
            //        shouldCreatData = false;
            //}
            if (shouldCreatData)
                shouldCreatData = GetDataList().Count == 0;

            if (shouldCreatData)
            {
                var newData = AgentHelper.CreateAreaInitializerNewData(this);
                newData.IsDefaultData = true;

                if (DataEntryEntity.IsReadonly)
                {
                    newData.IsUseLessBecauseNewAndReadonly = true;

                    foreach (var property in newData.ChildSimpleContorlProperties)
                    {
                        property.AddReadonlyState("", "DataNewAndReadonly", true);
                    }
                    foreach (var rel in newData.ChildRelationshipDatas)
                    {
                        rel.AddReadonlyState("", "DataNewAndReadonly", true);
                    }
                }

                var addResult = AddData(newData);
                if (!addResult)
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده پیش فرض و یا داده های وابسته", newData.ViewInfo, Temp.InfoColor.Red);
                //else
                //{

                //}
                //if (AreaInitializer.SourceRelationColumnControl != null)
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

        //   //     //relationshipControl.EditNdTypeArea.AreaInitializer.SourceRelationColumnControl.RelatedData = specificDate;
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
