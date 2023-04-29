using MyUILibrary.DataViewArea;
using MyUILibraryInterfaces.DataViewArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using ModelEntites;
using MyUILibraryInterfaces.DataMenuArea;

using MyCommonWPFControls;
using MyUILibraryInterfaces.DataLinkArea;
using MyRelationshipDataManager;

namespace MyUILibrary.GraphArea
{
    public class GraphArea : I_GraphArea
    {
        public GraphAreaInitializer AreaInitializer
        {
            set; get;
        }

        //public GraphDTO Graph
        //{
        //    set; get;
        //}

        public I_View_GraphArea View
        {
            set; get;
        }
        public bool SecurityNoAccess { set; get; }
        public bool SecurityReadonly { set; get; }
        public bool SecurityEdit { set; get; }
        I_EditEntityAreaOneData FirstSideEditEntityArea { set; get; }
        public List<GraphDTO> Graphs { get; set; }
        public GraphDTO SelectedGraph { get; set; }
        DP_DataView FirstData;
        // چون جایی نداریم فعلا که دوتا داده انتخاب کنیم و ارتباطشون رو بخوایم, DP_DataView otherData)


        /// <summary>
        /// /////////////////////////////////////////////////////// داده های تکراری را می توان یکسان سازی کرد در نمودار
        /// </summary>
     //   DP_DataView OtherData;
        MySearchLookup GraphSearchLookup;
        I_View_Diagram Diagram;
        public GraphArea(GraphAreaInitializer initParam)
        {
            AreaInitializer = initParam;
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfGraphArea();
            Diagram = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfDiagram();

            List<DiagramTypes> diagramTypes = new List<DiagramTypes>();

            diagramTypes.Add(new DiagramTypes() { Title = "سلسله مراتبی", DiagramType = EnumDiagramTypes.Sugiyama });
            diagramTypes.Add(new DiagramTypes() { Title = "درخت", DiagramType = EnumDiagramTypes.TreeUndefined });
            diagramTypes.Add(new DiagramTypes() { Title = "درخت افقی", DiagramType = EnumDiagramTypes.TreeHorizontal });
            diagramTypes.Add(new DiagramTypes() { Title = "درخت عمودی", DiagramType = EnumDiagramTypes.TreeVertical });
            diagramTypes.Add(new DiagramTypes() { Title = "درخت گردشی", DiagramType = EnumDiagramTypes.TreeRadial });
            diagramTypes.Add(new DiagramTypes() { Title = "درخت Tip Over", DiagramType = EnumDiagramTypes.TreeTipOver });
            diagramTypes.Add(new DiagramTypes() { Title = "درخت MindmapHorizontal", DiagramType = EnumDiagramTypes.MindmapHorizontal });
            diagramTypes.Add(new DiagramTypes() { Title = "درخت MindmapVertical", DiagramType = EnumDiagramTypes.MindmapVertical });
            Diagram.SetDiagramTypes(diagramTypes);
            View.AddDiagramView(Diagram);
            View.GraphConfirmed += View_GraphConfirmed;
            //     View.GraphChanged += View_GraphChanged;

            GraphSearchLookup = new MySearchLookup();
            GraphSearchLookup.DisplayMember = "ReportTitle";
            GraphSearchLookup.SelectedValueMember = "ID";
            GraphSearchLookup.SearchFilterChanged += GraphSearchLookup_SearchFilterChanged;
            GraphSearchLookup.SelectionChanged += GraphSearchLookup_SelectionChanged;
            View.AddGraphSelector(GraphSearchLookup);
            FirstData = AreaInitializer.FirstDataItem;
            //OtherData = AreaInitializer.OtherDataItem;
            if (AreaInitializer.GraphID != 0)
            {
                GraphSearchLookup.SelectedValue = AreaInitializer.GraphID;
                GraphSearchLookup.IsEnabledLookup = false;
            }
            else if (AreaInitializer.EntityID != 0)
            {
                Graphs = AgentUICoreMediator.GetAgentUICoreMediator.GraphManager.GetGraphs(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                GraphSearchLookup.ItemsSource = Graphs;
                GraphSearchLookup.SearchIsEnabled = false;
                if (Graphs.Count == 1)
                    GraphSearchLookup.SelectedItem = Graphs[0];
            }


            //ManageSecurity();
        }

        private void GraphSearchLookup_SearchFilterChanged(object sender, SearchFilterArg e)
        {
            if (!string.IsNullOrEmpty(e.SingleFilterValue))
            {
                if (e.FilterBySelectedValue)
                {
                    var Graph = AgentUICoreMediator.GetAgentUICoreMediator.GraphManager.GetGraph(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), Convert.ToInt32(e.SingleFilterValue));
                    e.ResultItemsSource = new List<GraphDTO> { Graph };
                }
                else
                {
                    var Graphs = AgentUICoreMediator.GetAgentUICoreMediator.GraphManager.SearchGraphs(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), e.SingleFilterValue);
                    e.ResultItemsSource = Graphs;
                }
            }
        }
        private void GraphSearchLookup_SelectionChanged(object sender, SelectionChangedArg e)
        {
            Diagram.ClearItems();
            if (e.SelectedItem != null)
            {
                var Graph = e.SelectedItem as GraphDTO;
                View.EnabaleDisabeViewSection(true);
                SetGraph(Graph.ID);
            }
            else
            {
                View.EnabaleDisabeViewSection(false);
            }
        }

        //AssignedPermissionDTO _Permission;
        //public AssignedPermissionDTO Permission
        //{
        //    get
        //    {
        //        if (_Permission == null)
        //            _Permission = AgentUICoreMediator.GetAgentUICoreMediator.SecurityHelper.GetAssignedPermissions(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID, false);
        //        return _Permission;
        //    }
        //}
        //private void ManageSecurity()
        //{
        //    if (Permission.GrantedActions.Any(x => x == SecurityAction.NoAccess))
        //    {
        //        SecurityNoAccess = true;
        //    }
        //    else
        //    {
        //        if (Permission.GrantedActions.Any(x => x == SecurityAction.EditAndDelete || x == SecurityAction.Edit))
        //        {
        //            SecurityEdit = true;
        //        }
        //        else if (Permission.GrantedActions.Any(x => x == SecurityAction.ReadOnly))
        //        {
        //            SecurityReadonly = true;
        //        }
        //        else
        //            SecurityNoAccess = true;
        //    }
        //    ImposeSecurity();
        //}

        //private void ImposeSecurity()
        //{
        //    if (SecurityNoAccess)
        //    {
        //        View = null;
        //        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به آرشیو");
        //    }
        //    else
        //    {
        //        if (!SecurityReadonly && !SecurityEdit)
        //        {
        //            View = null;
        //            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به آرشیو");

        //        }
        //    }
        //}
        //private void View_GraphChanged(object sender, EventArgs e)
        //{

        //}

        private void SetGraph(int GraphID)
        {
            //** 57f30aa8-478c-419d-9f90-d3b96a0f7c64
            SelectedGraph = AgentUICoreMediator.GetAgentUICoreMediator.GraphManager.GetGraph(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), GraphID);
            View.ClearEntityViews();
            if (SelectedGraph != null)
            {
                EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
                editEntityAreaInitializer1.EntityID = SelectedGraph.TableDrivedEntityID;
                editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                editEntityAreaInitializer1.DataMode = CommonDefinitions.UISettings.DataMode.One;
                var FirstSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer1);
                if (FirstSideEditEntityAreaResult.Item1 != null)
                {
                    FirstSideEditEntityArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
                 //   FirstSideEditEntityArea.SetAreaInitializer(editEntityAreaInitializer1);
                    View.SetFirstSideEntityView(FirstSideEditEntityArea.TemporaryDisplayView, FirstSideEditEntityArea.SimpleEntity.Alias);
                    FirstSideEditEntityArea.DataItemSelected += FirstSideEditEntityArea_DataItemSelected;
                }
                else
                {
                    if (!string.IsNullOrEmpty(FirstSideEditEntityAreaResult.Item2))
                        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage(FirstSideEditEntityAreaResult.Item2);
                    return;

                }

                if (FirstData != null)
                {
                    if (SelectedGraph.TableDrivedEntityID == FirstData.TargetEntityID)
                    {
                        FirstSideEditEntityArea.ClearData();
                        FirstSideEditEntityArea.ShowDataFromExternalSource(FirstData);
                    }
                }

            }
        }

        private void FirstSideEditEntityArea_DataItemSelected(object sender, EditAreaDataItemArg e)
        {
            if (e.DataItem != null)
            {
                View_GraphConfirmed(null, null);
            }
        }

        private void View_GraphConfirmed(object sender, EventArgs e)
        {
            if (SelectedGraph == null ||
                FirstSideEditEntityArea.AreaInitializer.Datas.Count == 0
             )
                return;
            FirstData = FirstSideEditEntityArea.AreaInitializer.Datas[0].DataView;
            List<GraphItem> GraphItems = new List<GraphItem>();

            var fItems = new List<GraphItemGroups>();
            //البته از هر دو طرف میشه به طرف دیگر رسید
            //بهتره همین طور باشه چون برای لینک سرورها کافیه سرورهای طرف اول به طرف دوم لینک داشته باشد
            //اینطوری مفهوم تر است
            foreach (var tail in SelectedGraph.RelationshipsTails)
            {
                //var searchDataTuple = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(OtherData, tail.RelationshipTail.ReverseRelationshipTail);
                //foreach (var item in FirstData.KeyProperties)
                //{
                //    searchDataTuple.Phrases.Add(new SearchProperty() { ColumnID = item.ColumnID, Value = item.Value });
                //}
                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                //سکوریتی داده اعمال میشود

                //البته از هر دو طرف میشه به طرف دیگر رسید
                //بهتره همین طور باشه چون برای لینک سرورها کافیه  در تیل سرورهای طرف اول هر رابطه به طرف دوم رابطه لینک داشته باشد
                //اینطوری مفهوم تر است
                fItems.Add(GetIncludedGraphItems(tail.RelationshipTail, SelectedGraph, FirstData, 0, tail.EntityRelationshipTailDataMenu));
            }

            var firstSidelinkItem = new MyUILibrary.GraphArea.GraphItem();
            firstSidelinkItem.DataItem = FirstData;
            firstSidelinkItem.View = GetDataViewItem(firstSidelinkItem.DataItem, SelectedGraph.FirstSideDataMenuID);
            firstSidelinkItem.View.IsRoot = true;
            firstSidelinkItem.IsFixed = true;



            foreach (var pitem in fItems)
                foreach (var item in pitem.Items)
                    item.View = GetDataViewItem(item.DataItem, item.DataMenuID);

            //////List<GraphRelation> allRelations = new List<GraphRelation>();
            //////List<GraphItem> allItems = new List<GraphItem>();
            //////allItems.Add(firstSidelinkItem);
            //////allItems.Add(secondSidelinkItem);
            //////foreach (var pitem in fItems)
            //////{
            //////    allRelations.AddRange(pitem.Relations);
            //////    allItems.AddRange(pitem.Items);
            //////}
            //////foreach (var item in allItems.Where(x => x.IsFixed == false && x.Level == 0))
            //////{
            //////    allRelations.Add(new GraphRelation(firstSidelinkItem, item));
            //////}
            //////var maxLevel = allItems.Where(x => x.IsFixed == false).Max(x => x.Level);
            //////foreach (var item in allItems.Where(x => x.IsFixed == false && x.Level == maxLevel))
            //////{
            //////    allRelations.Add(new GraphRelation(item, secondSidelinkItem));
            //////}
            /////////
            //////var result = RemoveRepeatedItems(allItems, allRelations);
            //بعدا تست شود ظاهر فرم
            Diagram.ClearItems();
            //////foreach (var item in result.Item1)
            //////{
            //////    Diagram.AddView(item.View);
            //////}

            //////foreach (var item in result.Item2)
            //////{
            //////    if (!result.Item1.Any(x => x == item.Item1))
            //////    {

            //////    }
            //////    if (!result.Item1.Any(x => x == item.Item2))
            //////    {

            //////    }
            //////    Diagram.AddRelation(item.Item1.View, item.Item2.View);
            //////}


            Diagram.AddView(firstSidelinkItem.View);
            foreach (var pitem in fItems)
            {
                foreach (var item in pitem.Items)
                {
                    Diagram.AddView(item.View);
                }
                foreach (var item in pitem.Relations)
                {
                    Diagram.AddRelation(item.Item1.View, item.Item2.View);
                }

                foreach (var item in pitem.Items.Where(x => x.Level == 0))
                {
                    Diagram.AddRelation(firstSidelinkItem.View, item.View);
                }
            }

            Diagram.RefreshDiagram();



            //foreach (var item in fItems.Where(x => x != secondSidelinkItem && !fItems.Any(y => y.ParentGraphItem == x)))
            //{
            //    //   View.AddLink(item.View, secondSidelinkItem.View);
            //}

            //var groupedList = fItems.GroupBy(x => x.RelationshipTail);
            //foreach (var tail in groupedList)
            //{
            //    //object panel = View.GenerateTailPanel();
            //    foreach (var item in tail.Where(x => x.Level == 0))
            //    {
            //        View.AddFirstLevelGraphItem(item.View);
            //        CheckDependentItems(tail, item);
            //    }
            //}
            //List<GraphItemViewGroups> viewGroups = new List<GraphItemViewGroups>();
            ////var listViewGroups = fItems.Select(x => x.Items.Select(y => y.View).ToList()).ToList();
            ////var listViewRelations = fItems.Select(x => x.Relations.Select(y => new Tuple<I_DataViewItem, I_DataViewItem>(y.Item1.View, y.Item2.View)).ToList()).ToList();
            //foreach (var pitem in fItems)
            //{
            //    GraphItemViewGroups viewGroup = new GraphItemViewGroups();
            //    viewGroup.Views.AddRange(pitem.Items.Select(x => x.View).ToList());
            //    viewGroup.ViewRelations.AddRange(pitem.Relations.Select(x => new Tuple<I_DataViewItem, I_DataViewItem>(x.Item1.View, x.Item2.View)).ToList());
            //    viewGroups.Add(viewGroup);
            //}

            //   View.ShowDiagram(viewGroups, firstSidelinkItem.View, secondSidelinkItem.View);
        }


        //private void CheckDependentItems(List<GraphItem> tail, GraphItem parentItem)
        //{
        //    //   return;
        //    foreach (var item in tail.Where(x => x.ParentGraphItem == parentItem))
        //    {
        //        View.AddLink(parentItem.View, item.View);
        //        CheckDependentItems(tail, item);
        //    }
        //}

        private I_DataViewItem GetDataViewItem(DP_DataView item, int dataMenuID)
        {
            I_DataViewItem dataViewItem = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDataViewItem();
            //     درست شود
            dataViewItem.DataView = item;
            dataViewItem.InfoClicked += DataViewItem_InfoClicked;
            //dataViewItem.Selected += DataViewItem_Selected;
            //foreach (var column in item.Properties.Take(3))
            //{
            //    dataViewItem.AddTitleRow(column.Name, column.Value);
            //}
            if (!string.IsNullOrEmpty(item.TargetEntityAlias))
                dataViewItem.Title = item.TargetEntityAlias;
            dataViewItem.Body = item.ViewInfo;
            return dataViewItem;
        }

        //private void DataViewItem_Selected(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private void DataViewItem_InfoClicked(object sender, EventArgs e)
        {
            //فانکشن شود در دیتا ویو هم هست
            var dataViewItem = (sender as I_DataViewItem);
            if (dataViewItem != null)
            {
                //var menus = GetDataViewItemMenus(dataViewItem);
                //dataViewItem.ShowDataViewItemMenus(menus);
                var menuInitializer = new DataMenuAreaInitializer(0);
                //menuInitializer.HostDataViewArea = this;
                //menuInitializer.HostDataViewItem = dataViewItem;
                menuInitializer.SourceView = sender;

                //DP_DataView dataRepository = new DP_DataView();
                //dataRepository.TargetEntityID = dataViewItem.DataView.TargetEntityID;
                var dataView = dataViewItem.DataView;
                //foreach (var key in dataViewItem.DataView.Properties.Where(x => x.IsKey))
                //{
                //    dataRepository.AddProperty(new ColumnDTO() { ID = key.ColumnID }, key.Value);
                //}
                menuInitializer.DataItem = dataView;


                AgentUICoreMediator.GetAgentUICoreMediator.ShowMenuArea(menuInitializer);

            }
        }

        private GraphItemGroups GetIncludedGraphItems(EntityRelationshipTailDTO relationshipTail, GraphDTO selectedGraph, DP_DataView relationshipFirstData, int level
          , EntityRelationshipTailDataMenuDTO relationshipTailDataMenuDTO, GraphItem parentGraphItem = null, GraphItemGroups result = null, List<GraphItem> allItems = null)
        {
            if (result == null)
                result = new GraphItemGroups();
            if (relationshipTail != null)
            {
                RelationshipTailDataManager relationshipTailDataManager = new RelationshipTailDataManager();

                //var firstData = new DP_DataRepository();
                //firstData.TargetEntityID = FirstData.TargetEntityID;
                //firstData.DataView = FirstData;
                //foreach (var key in FirstData.KeyProperties)
                //{
                //    firstData.AddProperty(new ColumnDTO() { ID = key.ColumnID }, key.Value);
                //}
                var searchDataTuple = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipDataManager.GetSecondSideSearchDataItemByRelationship(relationshipFirstData, relationshipTail.Relationship.ID);
                //var searchPhraseToOtherData = relationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(targetData, relationshipTail.ChildTail.ReverseRelationshipTail);
                //searchDataTuple.Phrases.AddRange(searchPhraseToOtherData.Phrases);
                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                //سکوریتی داده اعمال میشود
                var searchRequest = new DR_SearchViewRequest(requester, searchDataTuple);
                //searchRequest.EntityID = parentTail.RelationshipTargetEntityID;
                var searchResult = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(searchRequest);
                if (parentGraphItem != null)
                {
                    parentGraphItem.TailPath = relationshipTail.RelationshipIDPath;
                    if (relationshipTailDataMenuDTO != null)
                    {
                        if (relationshipTailDataMenuDTO.Items.Any(x => x.Path == parentGraphItem.TailPath))
                        {
                            parentGraphItem.DataMenuID = relationshipTailDataMenuDTO.Items.First(x => x.Path == parentGraphItem.TailPath).DataMenuSettingID;
                        }
                    }
                }
                if (searchResult.Result == Enum_DR_ResultType.SeccessfullyDone)
                {
                    foreach (var item in searchResult.ResultDataItems)
                    {
                        GraphItem found = null;
                        if (allItems == null)
                            allItems = new List<GraphItem>();
                        else
                        {
                            if (selectedGraph.NotJointEntities == true)
                            {
                                if (allItems.Any(x => AgentHelper.DataItemsAreEqual(item, x.DataItem)))
                                {
                                    var fItem = allItems.First(x => AgentHelper.DataItemsAreEqual(item, x.DataItem));
                                    found = fItem;
                                }
                            }
                        }
                        if (found == null)
                        {
                            found = new GraphItem();
                            found.DataItem = item;
                            found.Level = level;
                            result.Items.Add(found);
                            allItems.Add(found);
                        }
                        if (parentGraphItem != null)
                            result.Relations.Add(new GraphRelation(parentGraphItem, found));

                        GetIncludedGraphItems(relationshipTail.ChildTail, selectedGraph, item, level + 1, relationshipTailDataMenuDTO, found, result);
                    }
                }
                else
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(searchResult.Message, searchResult.Details, MyUILibrary.Temp.InfoColor.Red);
                }

            }
            return result;
        }
    }
    public class GraphItemGroups
    {
        public GraphItemGroups()
        {
            Items = new List<GraphItem>();
            Relations = new List<GraphRelation>();
        }
        public List<GraphItem> Items { set; get; }
        public List<GraphRelation> Relations { set; get; }

    }
    public class GraphRelation
    {
        public GraphRelation(GraphItem item1, GraphItem item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        public GraphItem Item1 { set; get; }
        public GraphItem Item2 { set; get; }
    }
    public class GraphItem
    {
        //    public EntityRelationshipTailDTO RelationshipTail { set; get; }
        //     public int Level { set; get; }
        //public GraphItem ParentGraphItem { set; get; }
        public DP_DataView DataItem { set; get; }
        //      public bool IsLastLevel { set; get; }
        public I_DataViewItem View { set; get; }
        public int Level { get; internal set; }

        public bool IsFixed { set; get; }
        public string TailPath { get; internal set; }
        public int DataMenuID { get; internal set; }
    }
}
