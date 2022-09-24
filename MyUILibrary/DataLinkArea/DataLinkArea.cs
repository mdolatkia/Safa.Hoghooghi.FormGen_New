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
using MyUILibraryInterfaces.DataLinkArea;
using MyRelationshipDataManager;
using MyUILibraryInterfaces.DataMenuArea;

using MyCommonWPFControls;

namespace MyUILibrary.DataLinkArea
{
    public class DataLinkArea : I_DataLinkArea
    {
        public DataLinkAreaInitializer AreaInitializer
        {
            set; get;
        }

        //public DataLinkDTO DataLink
        //{
        //    set; get;
        //}

        public I_View_DataLinkArea View
        {
            set; get;
        }
        public bool SecurityNoAccess { set; get; }
        public bool SecurityReadonly { set; get; }
        public bool SecurityEdit { set; get; }
        I_EditEntityAreaOneData FirstSideEditEntityArea { set; get; }
        I_EditEntityAreaOneData SecondSideEditEntityArea { set; get; }
        public List<DataLinkDTO> DataLinks { get; set; }
        public DataLinkDTO SelectedDataLink { get; set; }
        DP_DataView FirstData;
        // چون جایی نداریم فعلا که دوتا داده انتخاب کنیم و ارتباطشون رو بخوایم, DP_DataView otherData)


        /// <summary>
        /// /////////////////////////////////////////////////////// داده های تکراری را می توان یکسان سازی کرد در نمودار
        /// </summary>
        DP_DataView OtherData;
        MySearchLookup dataLinkSearchLookup;
        I_View_Diagram Diagram;
        public DataLinkArea(DataLinkAreaInitializer initParam)
        {
            AreaInitializer = initParam;
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfDataLinkArea();
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
            View.DataLinkConfirmed += View_DataLinkConfirmed;
            //     View.DataLinkChanged += View_DataLinkChanged;

            dataLinkSearchLookup = new MySearchLookup();
            dataLinkSearchLookup.DisplayMember = "ReportTitle";
            dataLinkSearchLookup.SelectedValueMember = "ID";
            dataLinkSearchLookup.SearchFilterChanged += dataLinkSearchLookup_SearchFilterChanged;
            dataLinkSearchLookup.SelectionChanged += dataLinkSearchLookup_SelectionChanged;
            View.AddDataLinkSelector(dataLinkSearchLookup);
            FirstData = AreaInitializer.FirstDataItem;
            //OtherData = AreaInitializer.OtherDataItem;
            if (AreaInitializer.DataLinkID != 0)
            {
                dataLinkSearchLookup.SelectedValue = AreaInitializer.DataLinkID;
                dataLinkSearchLookup.IsEnabledLookup = false;
            }
            else if (AreaInitializer.EntityID != 0)
            {
                DataLinks = AgentUICoreMediator.GetAgentUICoreMediator.DataLinkManager.GetDataLinks(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.EntityID);
                dataLinkSearchLookup.ItemsSource = DataLinks;
                dataLinkSearchLookup.SearchIsEnabled = false;
                if (DataLinks.Count == 1)
                    dataLinkSearchLookup.SelectedItem = DataLinks[0];
            }


            //ManageSecurity();
        }

        private void dataLinkSearchLookup_SearchFilterChanged(object sender, SearchFilterArg e)
        {
            if (!string.IsNullOrEmpty(e.SingleFilterValue))
            {
                if (e.FilterBySelectedValue)
                {
                    var dataLink = AgentUICoreMediator.GetAgentUICoreMediator.DataLinkManager.GetDataLink(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), Convert.ToInt32(e.SingleFilterValue));
                    e.ResultItemsSource = new List<DataLinkDTO> { dataLink };
                }
                else
                {
                    var dataLinks = AgentUICoreMediator.GetAgentUICoreMediator.DataLinkManager.SearchDatalinks(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), e.SingleFilterValue);
                    e.ResultItemsSource = dataLinks;
                }
            }
        }
        private void dataLinkSearchLookup_SelectionChanged(object sender, SelectionChangedArg e)
        {
            Diagram.ClearItems();
            if (e.SelectedItem != null)
            {
                var dataLink = e.SelectedItem as DataLinkDTO;
                View.EnabaleDisabeViewSection(true);
                SetDataLink(dataLink.ID);
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
        //private void View_DataLinkChanged(object sender, EventArgs e)
        //{

        //}

        private void SetDataLink(int dataLinkID)
        {
            //** c23b278a-930f-45ef-92cb-cdff2b2126ea
            SelectedDataLink = AgentUICoreMediator.GetAgentUICoreMediator.DataLinkManager.GetDataLink(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), dataLinkID);
            View.ClearEntityViews();
            if (SelectedDataLink != null)
            {
                EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
                editEntityAreaInitializer1.EntityID = SelectedDataLink.TableDrivedEntityID;
                editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                editEntityAreaInitializer1.DataMode = CommonDefinitions.UISettings.DataMode.One;
                var FirstSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer1);
                if (FirstSideEditEntityAreaResult.Item1 != null)
                {
                    FirstSideEditEntityArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
                //    FirstSideEditEntityArea.SetAreaInitializer(editEntityAreaInitializer1);
                    View.SetFirstSideEntityView(FirstSideEditEntityArea.TemporaryDisplayView, FirstSideEditEntityArea.SimpleEntity.Alias);
                }
                else
                {
                    if (!string.IsNullOrEmpty(FirstSideEditEntityAreaResult.Item2))
                        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage(FirstSideEditEntityAreaResult.Item2);
                    return;

                }
                EditEntityAreaInitializer editEntityAreaInitializer2 = new EditEntityAreaInitializer();
                editEntityAreaInitializer2.EntityID = SelectedDataLink.SecondSideEntityID;
                editEntityAreaInitializer2.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                editEntityAreaInitializer2.DataMode = CommonDefinitions.UISettings.DataMode.One;
                var SecondSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer2);
                if (SecondSideEditEntityAreaResult.Item1 != null)
                {
                    SecondSideEditEntityArea = SecondSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
          //          SecondSideEditEntityArea.SetAreaInitializer(editEntityAreaInitializer2);
                    View.SetSecondSideEntityView(SecondSideEditEntityArea.TemporaryDisplayView, SecondSideEditEntityArea.SimpleEntity.Alias);
                }
                else
                    return;
                bool firstDataSetToFirst = false;
                bool firstDataSetToSecond = false;
                if (FirstData != null)
                {
                    if (SelectedDataLink.TableDrivedEntityID == FirstData.TargetEntityID)
                    {
                        FirstSideEditEntityArea.ClearData();
                        FirstSideEditEntityArea.ShowDataFromExternalSource(FirstData);
                        firstDataSetToFirst = true;
                    }
                    else if (SelectedDataLink.SecondSideEntityID == FirstData.TargetEntityID)
                    {
                        SecondSideEditEntityArea.ClearData();
                        SecondSideEditEntityArea.ShowDataFromExternalSource(FirstData);
                        firstDataSetToSecond = true;
                    }
                }
                if (OtherData != null)
                {
                    if (!firstDataSetToFirst && SelectedDataLink.TableDrivedEntityID == OtherData.TargetEntityID)
                    {
                        FirstSideEditEntityArea.ClearData();
                        FirstSideEditEntityArea.ShowDataFromExternalSource(OtherData);
                    }
                    else if (!firstDataSetToSecond && SelectedDataLink.SecondSideEntityID == OtherData.TargetEntityID)
                    {
                        SecondSideEditEntityArea.ClearData();
                        SecondSideEditEntityArea.ShowDataFromExternalSource(OtherData);
                    }
                }
            }
        }

        private void View_DataLinkConfirmed(object sender, EventArgs e)
        {
            if (SelectedDataLink == null ||
                FirstSideEditEntityArea.AreaInitializer.Datas.Count == 0
                || SecondSideEditEntityArea.AreaInitializer.Datas.Count == 0)
                return;
            FirstData = FirstSideEditEntityArea.AreaInitializer.Datas[0].DataView;
            OtherData = SecondSideEditEntityArea.AreaInitializer.Datas[0].DataView;
            List<DataLinkItem> dataLinkItems = new List<DataLinkItem>();

            var fItems = new List<DataLinkItemGroup>();
            //البته از هر دو طرف میشه به طرف دیگر رسید
            //بهتره همین طور باشه چون برای لینک سرورها کافیه سرورهای طرف اول به طرف دوم لینک داشته باشد
            //اینطوری مفهوم تر است
            foreach (var tail in SelectedDataLink.RelationshipsTails)
            {
                //     tail.EntityRelationshipTailDataMenu

                var searchDataTuple = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(OtherData, tail.RelationshipTail.ReverseRelationshipTail);
                foreach (var item in FirstData.KeyProperties)
                {
                    searchDataTuple.Phrases.Add(new SearchProperty() { ColumnID = item.ColumnID, Value = item.Value });
                }
                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                //سکوریتی داده اعمال میشود
                var existsRequest = new DR_SearchExistsRequest(requester);
                existsRequest.EntityID = SecondSideEditEntityArea.AreaInitializer.EntityID;
                existsRequest.SearchDataItems = searchDataTuple;
                var exists = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchExistsRequest(existsRequest);
                if (exists.Result == Enum_DR_ResultType.SeccessfullyDone)
                {
                    if (exists.ExistsResult)
                    {
                        //البته از هر دو طرف میشه به طرف دیگر رسید
                        //بهتره همین طور باشه چون برای لینک سرورها کافیه  در تیل سرورهای طرف اول هر رابطه به طرف دوم رابطه لینک داشته باشد
                        //اینطوری مفهوم تر است
                        fItems.Add(GetIncludedDataLinkItems(tail.RelationshipTail, SelectedDataLink, FirstData, OtherData, 0, tail.EntityRelationshipTailDataMenu));
                    }
                }
                else
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(exists.Message, exists.Details, MyUILibrary.Temp.InfoColor.Red);
                }
            }

            var tail1 = SelectedDataLink.RelationshipsTails.First();
            //    tail1.EntityRelationshipTailListView.FirstSideListViewID
            var firstSidelinkItem = new MyUILibrary.DataLinkArea.DataLinkItem();
            firstSidelinkItem.DataItem = FirstData;
            firstSidelinkItem.View = GetDataViewItem(firstSidelinkItem.DataItem, SelectedDataLink.FirstSideDataMenuID);
            firstSidelinkItem.View.IsRoot = true;
            firstSidelinkItem.IsFixed = true;


            var secondSidelinkItem = new MyUILibrary.DataLinkArea.DataLinkItem();
            secondSidelinkItem.DataItem = OtherData;
            secondSidelinkItem.View = GetDataViewItem(secondSidelinkItem.DataItem, SelectedDataLink.SecondSideDataMenuID);
            secondSidelinkItem.View.IsRoot = true;
            secondSidelinkItem.IsFixed = true;

            foreach (var pitem in fItems)
                foreach (var item in pitem.Items)
                {
                    item.View = GetDataViewItem(item.DataItem, item.DataMenuID);
                }

            //////List<DataLinkRelation> allRelations = new List<DataLinkRelation>();
            //////List<DataLinkItem> allItems = new List<DataLinkItem>();
            //////allItems.Add(firstSidelinkItem);
            //////allItems.Add(secondSidelinkItem);
            //////foreach (var pitem in fItems)
            //////{
            //////    allRelations.AddRange(pitem.Relations);
            //////    allItems.AddRange(pitem.Items);
            //////}
            //////foreach (var item in allItems.Where(x => x.IsFixed == false && x.Level == 0))
            //////{
            //////    allRelations.Add(new DataLinkRelation(firstSidelinkItem, item));
            //////}
            //////var maxLevel = allItems.Where(x => x.IsFixed == false).Max(x => x.Level);
            //////foreach (var item in allItems.Where(x => x.IsFixed == false && x.Level == maxLevel))
            //////{
            //////    allRelations.Add(new DataLinkRelation(item, secondSidelinkItem));
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
            Diagram.AddView(secondSidelinkItem.View);
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
                var maxLevel = pitem.Items.Max(x => x.Level);
                foreach (var item in pitem.Items.Where(x => x.Level == maxLevel))
                {
                    Diagram.AddRelation(item.View, secondSidelinkItem.View);
                }
            }

            Diagram.RefreshDiagram();



            //foreach (var item in fItems.Where(x => x != secondSidelinkItem && !fItems.Any(y => y.ParentDataLinkItem == x)))
            //{
            //    //   View.AddLink(item.View, secondSidelinkItem.View);
            //}

            //var groupedList = fItems.GroupBy(x => x.RelationshipTail);
            //foreach (var tail in groupedList)
            //{
            //    //object panel = View.GenerateTailPanel();
            //    foreach (var item in tail.Where(x => x.Level == 0))
            //    {
            //        View.AddFirstLevelDataLinkItem(item.View);
            //        CheckDependentItems(tail, item);
            //    }
            //}
            //List<DataLinkItemViewGroups> viewGroups = new List<DataLinkItemViewGroups>();
            ////var listViewGroups = fItems.Select(x => x.Items.Select(y => y.View).ToList()).ToList();
            ////var listViewRelations = fItems.Select(x => x.Relations.Select(y => new Tuple<I_DataViewItem, I_DataViewItem>(y.Item1.View, y.Item2.View)).ToList()).ToList();
            //foreach (var pitem in fItems)
            //{
            //    DataLinkItemViewGroups viewGroup = new DataLinkItemViewGroups();
            //    viewGroup.Views.AddRange(pitem.Items.Select(x => x.View).ToList());
            //    viewGroup.ViewRelations.AddRange(pitem.Relations.Select(x => new Tuple<I_DataViewItem, I_DataViewItem>(x.Item1.View, x.Item2.View)).ToList());
            //    viewGroups.Add(viewGroup);
            //}

            //   View.ShowDiagram(viewGroups, firstSidelinkItem.View, secondSidelinkItem.View);
        }

        //private Tuple<List<DataLinkItem>, List<DataLinkRelation>> RemoveRepeatedItems(List<DataLinkItem> items, List<DataLinkRelation> relations)
        //{
        //    List<DataLinkItem> allItems = new List<DataLinkItem>();
        //    foreach (var item in items.Where(x => x.IsFixed == true))
        //    {
        //        allItems.Add(item);
        //    }

        //    List<DataLinkItem> bodyItems = new List<DataLinkItem>();
        //    foreach (var item in items.Where(x => x.IsFixed == false))
        //    {
        //        bodyItems.Add(item);
        //    }
        //    List<Tuple<DataLinkItem, List<DataLinkItem>>> groupItems = new List<Tuple<DataLinkItem, List<DataLinkItem>>>();
        //    foreach (var item in bodyItems)
        //    {
        //        if (!groupItems.Any(x => x.Item2.Contains(item)))
        //        {

        //            var sameOtherItems = bodyItems.Where(x => x != item && AgentHelper.DataItemsAreEqual(item.DataItem, x.DataItem));
        //            List<DataLinkItem> removes = new List<DataLinkItem>();
        //            foreach (var otheritem in sameOtherItems)
        //            {
        //                removes.Add(otheritem);
        //            }
        //            if (removes.Any())
        //                groupItems.Add(new Tuple<DataLinkItem, List<DataLinkItem>>(item, removes));
        //        }
        //    }
        //    foreach (var item in groupItems)
        //    {
        //        allItems.Add(item.Item1);
        //        foreach (var removes in item.Item2)
        //        {
        //            foreach (var relation in relations)
        //            {
        //                if (relation.Item1 == item.Item1)
        //                    relation.Item1 = item.Item1;
        //                if (relation.Item2 == item.Item1)
        //                    relation.Item2 = item.Item1;
        //            }
        //        }
        //    }
        //    return new Tuple<List<DataLinkItem>, List<DataLinkRelation>>(allItems, relations);
        //}
        //private void CheckDependentItems(List<DataLinkItem> tail, DataLinkItem parentItem)
        //{
        //    //   return;
        //    foreach (var item in tail.Where(x => x.ParentDataLinkItem == parentItem))
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
            dataViewItem.InfoClicked += (sender, e) => DataViewItem_InfoClicked(sender, e, dataMenuID);
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

        private void DataViewItem_InfoClicked(object sender, EventArgs e, int dataMenuID)
        {
            //فانکشن شود در دیتا ویو هم هست
            var dataViewItem = (sender as I_DataViewItem);
            if (dataViewItem != null)
            {
                //var menus = GetDataViewItemMenus(dataViewItem);
                //dataViewItem.ShowDataViewItemMenus(menus);
                var menuInitializer = new DataMenuAreaInitializer(dataMenuID);
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

        private DataLinkItemGroup GetIncludedDataLinkItems(EntityRelationshipTailDTO relationshipTail, DataLinkDTO selectedDataLink, DP_DataView relationshipFirstData, DP_DataView targetData, int level
            , EntityRelationshipTailDataMenuDTO relationshipTailDataMenuDTO, DataLinkItem parentDataLinkItem = null, DataLinkItemGroup result = null, List<DataLinkItem> allItems = null)
        {

            if (result == null)
                result = new DataLinkItemGroup();
            if (relationshipTail != null && relationshipTail.ChildTail != null)
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
                var searchPhraseToOtherData = relationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(targetData, relationshipTail.ChildTail.ReverseRelationshipTail);
                searchDataTuple.Phrases.AddRange(searchPhraseToOtherData.Phrases);
                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                //سکوریتی داده اعمال میشود
                var searchRequest = new DR_SearchViewRequest(requester, searchDataTuple);
                //searchRequest.EntityID = parentTail.RelationshipTargetEntityID;
                var searchResult = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(searchRequest);

                if(parentDataLinkItem!=null)
                {
                    parentDataLinkItem.TailPath = relationshipTail.RelationshipIDPath;
                    if (relationshipTailDataMenuDTO != null)
                    {
                        if (relationshipTailDataMenuDTO.Items.Any(x => x.Path == parentDataLinkItem.TailPath))
                        {
                            parentDataLinkItem.DataMenuID = relationshipTailDataMenuDTO.Items.First(x => x.Path == parentDataLinkItem.TailPath).DataMenuSettingID;
                        }
                    }
                }
                if (searchResult.Result == Enum_DR_ResultType.SeccessfullyDone)
                {
                    foreach (var item in searchResult.ResultDataItems)
                    {
                        DataLinkItem found = null;
                        if (allItems == null)
                            allItems = new List<DataLinkItem>();
                        else
                        {
                            if (selectedDataLink.NotJointEntities == true)
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
                            found = new DataLinkItem();
                            found.DataItem = item;
                            found.Level = level;
                          
                            result.Items.Add(found);
                            allItems.Add(found);
                        }
                        if (parentDataLinkItem != null)
                            result.Relations.Add(new DataLinkRelation(parentDataLinkItem, found));

                        GetIncludedDataLinkItems(relationshipTail.ChildTail, selectedDataLink, item, targetData, level + 1, relationshipTailDataMenuDTO, found, result);
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
    public class DataLinkItemGroup
    {
        public DataLinkItemGroup()
        {
            Items = new List<DataLinkItem>();
            Relations = new List<DataLinkRelation>();
        }
        public List<DataLinkItem> Items { set; get; }
        public List<DataLinkRelation> Relations { set; get; }

    }
    public class DataLinkRelation
    {
        public DataLinkRelation(DataLinkItem item1, DataLinkItem item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        public DataLinkItem Item1 { set; get; }
        public DataLinkItem Item2 { set; get; }
    }
    public class DataLinkItem
    {
        //    public EntityRelationshipTailDTO RelationshipTail { set; get; }
        //     public int Level { set; get; }
        //public DataLinkItem ParentDataLinkItem { set; get; }
        public DP_DataView DataItem { set; get; }
        //      public bool IsLastLevel { set; get; }
        public I_DataViewItem View { set; get; }
        public int Level { get; internal set; }

        public bool IsFixed { set; get; }
        public string TailPath { get; internal set; }
        public int DataMenuID { get; internal set; }
    }
}
