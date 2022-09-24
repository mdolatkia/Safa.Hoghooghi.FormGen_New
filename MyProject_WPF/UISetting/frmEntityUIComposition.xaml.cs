using ModelEntites;
using MyModelManager;
using MyUIGenerator;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityUIComposition.xaml
    /// </summary>
    public partial class frmEntityUIComposition : UserControl
    {
        //** d4503365-cbb8-45c8-a108-3292f851d67b
        BizEntityUIComposition bizEntityUIComposition = new BizEntityUIComposition();
        int EntityID { set; get; }
        //  List<ColumnOrRelationship> ColumnOrRelationships = new List<ColumnOrRelationship>();
        //List<relationship> Relationships = new List<relationship>();

        public frmEntityUIComposition(int entityID)
        {
            InitializeComponent();
            EntityID = entityID;
            SetColumnTypes();
            HideAllPanels();
            PopulateTree();
            this.treeEntityUIComposition.MouseMove += treeDBObjects_MouseMove;
            //this.ucEntityTree.treeDBObjects.startdr
            //this.ucEntityTree.treeDBObjects.MouseLeftButtonDown += frmDBTree_MouseLeftButtonDown;
            this.treeEntityUIComposition.DragOver += treeEntityUIComposition_DragOver;
            this.treeEntityUIComposition.Drop += treeEntityUIComposition_Drop;

            //this.Loaded += frmEntityUIComposition_Loaded;

        }

        private void SetColumnTypes()
        {
            var listEnum = Enum.GetValues(typeof(Enum_UIColumnsType));
            cmbColumnColumnsCount.ItemsSource = listEnum;
            cmbGroupColumnsCount.ItemsSource = listEnum;
            cmbRelationshipColumnsCount.ItemsSource = listEnum;
            cmbTabGroupColumnsCount.ItemsSource = listEnum;
            cmbEmptySpaceColumnsCount.ItemsSource = listEnum;
        }



        private void PopulateTree()
        {
            //** c6287638-6961-48c2-ba47-35292cf1d9a1
            treeEntityUIComposition.Items.Clear();

            var UICompositionTree = bizEntityUIComposition.GetOrCreateEntityUIComposition(EntityID);

            var parentNode = AddNavigationNode(treeEntityUIComposition.Items, UICompositionTree);
            ShowTreeItem(parentNode.Items, UICompositionTree.ChildItems.OrderBy(x => x.Position).ToList());

            treeEntityUIComposition.ExpandAll();
        }
        private void ShowTreeItem(ItemCollection itemCollection, List<EntityUICompositionDTO> items)
        {
            foreach (var item in items)
            {
                var node = AddNavigationNode(itemCollection, item);
                ShowTreeItem(node.Items, item.ChildItems.OrderBy(x => x.Position).ToList());
            }
            // return node;
        }


        private RadTreeViewItem AddNode(ItemCollection collection, string title, DatabaseObjectCategory type)
        {
            var node = new RadTreeViewItem();
            node.Header = GetNodeHeader(title, type);
            EntityUICompositionDTO context = new EntityUICompositionDTO();
            context.ObjectCategory = type;
            if (context.ObjectCategory == DatabaseObjectCategory.Entity)
                context.EntityUISetting = new EntityUISettingDTO();
            else if (context.ObjectCategory == DatabaseObjectCategory.Column)
                context.ColumnUISetting = new ColumnUISettingDTO();
            else if (context.ObjectCategory == DatabaseObjectCategory.Relationship)
                context.RelationshipUISetting = new RelationshipUISettingDTO();
            else if (context.ObjectCategory == DatabaseObjectCategory.Group)
                context.GroupUISetting = new GroupUISettingDTO();
            else if (context.ObjectCategory == DatabaseObjectCategory.TabControl)
                context.TabGroupUISetting = new TabGroupUISettingDTO();
            else if (context.ObjectCategory == DatabaseObjectCategory.TabPage)
                context.TabPageUISetting = new TabPageUISettingDTO();
            else if (context.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                context.EmptySpaceUISetting = new EmptySpaceUISettingDTO();
            context.Title = title;
            node.DataContext = context;
            collection.Add(node);
            return node;
        }
        private RadTreeViewItem AddNavigationNode(ItemCollection collection, EntityUICompositionDTO uiComposition)
        {
            var node = new RadTreeViewItem();
            node.DataContext = uiComposition;
            node.Header = GetNodeHeader(uiComposition.Title, uiComposition.ObjectCategory);
            //   node.Selected += node_Selected;
            collection.Add(node);
            return node;
        }


        private FrameworkElement GetNodeHeader(string title, DatabaseObjectCategory type)
        {
            StackPanel pnlHeader = new StackPanel();
            System.Windows.Controls.TextBlock label = new System.Windows.Controls.TextBlock();
            label.Text = title;
            Image img = new Image();
            img.Width = 15;
            Uri uriSource = null;
            if (type == DatabaseObjectCategory.Entity)
            {
                uriSource = new Uri("../Images/folder.png", UriKind.Relative);
            }
            else if (type == DatabaseObjectCategory.Entity)
            {
                uriSource = new Uri("../Images/form.png", UriKind.Relative);
            }
            else if (type == DatabaseObjectCategory.Column)
            {
                uriSource = new Uri("../Images/column.png", UriKind.Relative);
            }
            else if (type == DatabaseObjectCategory.Relationship)
            {
                uriSource = new Uri("../Images/relationship.png", UriKind.Relative);
            }
            else if (type == DatabaseObjectCategory.Group)
            {
                uriSource = new Uri("../Images/group.png", UriKind.Relative);
            }
            else if (type == DatabaseObjectCategory.TabControl)
            {
                uriSource = new Uri("../Images/tabcontrol.png", UriKind.Relative);
            }
            else if (type == DatabaseObjectCategory.TabPage)
            {
                uriSource = new Uri("../Images/tabpage.png", UriKind.Relative);
            }
            if (uriSource != null)
                img.Source = new BitmapImage(uriSource);
            pnlHeader.Orientation = Orientation.Horizontal;
            pnlHeader.Children.Add(img);
            pnlHeader.Children.Add(label);
            return pnlHeader;
        }


        private void treeEntityUIComposition_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is RadTreeViewItem))
                source = VisualTreeHelper.GetParent(source);
            if (source != null)
            {
                treeEntityUIComposition.SelectedItem = (source as RadTreeViewItem);
                (source as RadTreeViewItem).Focus();
                e.Handled = true;
            }
        }

        bool _isDragging = false;
        void treeDBObjects_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton != MouseButtonState.Pressed)
                _isDragging = false;
            if (!_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                if (menuNavigation.IsVisible)
                    return;
                if (treeEntityUIComposition.SelectedItem != null)
                {
                    try
                    {
                        _isDragging = true;
                        DragDrop.DoDragDrop(treeEntityUIComposition, treeEntityUIComposition.SelectedItem,
                            DragDropEffects.Move);
                    }
                    catch { }
                }
            }
        }

        void treeEntityUIComposition_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(RadTreeViewItem)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        void treeEntityUIComposition_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(RadTreeViewItem)))
            {
                var sourceNode = (RadTreeViewItem)e.Data.GetData(typeof(RadTreeViewItem));
                var target = e.Source as DependencyObject;
                while (target != null && !(target is RadTreeViewItem))
                    target = VisualTreeHelper.GetParent(target);

                var targetNode = target as RadTreeViewItem;
                if (sourceNode != null && targetNode != null)
                {
                    var sourceContext = sourceNode.DataContext as EntityUICompositionDTO;
                    CloneTreeNode(sourceNode, targetNode);
                }
            }
        }
        private void CloneTreeNode(RadTreeViewItem sourceNode, RadTreeViewItem targetNode)
        {
            if (sourceNode == null || targetNode == null)
                return;
            if (IsNodeInChilds(sourceNode.Items, targetNode))
                return;
            if (sourceNode == targetNode)
                return;
            var sourceContext = sourceNode.DataContext as EntityUICompositionDTO;
            var targetContext = targetNode.DataContext as EntityUICompositionDTO;
            if (sourceContext == null || targetContext == null)
                return;
            if (sourceContext.ObjectCategory == DatabaseObjectCategory.Entity)
                return;

            try
            {
                if (sourceContext.ObjectCategory == DatabaseObjectCategory.Group
                  || sourceContext.ObjectCategory == DatabaseObjectCategory.TabControl
                  || sourceContext.ObjectCategory == DatabaseObjectCategory.Column
                    || sourceContext.ObjectCategory == DatabaseObjectCategory.Relationship
                       || sourceContext.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                {
                    if (targetContext.ObjectCategory == DatabaseObjectCategory.Entity)
                    {
                        (sourceNode.Parent as RadTreeViewItem).Items.Remove(sourceNode);
                        targetNode.Items.Insert(0, sourceNode);
                    }
                    else if (targetContext.ObjectCategory == DatabaseObjectCategory.Group
                       || targetContext.ObjectCategory == DatabaseObjectCategory.TabPage)
                    {
                        (sourceNode.Parent as RadTreeViewItem).Items.Remove(sourceNode);
                        targetNode.Items.Add(sourceNode);
                    }
                    else if (targetContext.ObjectCategory == DatabaseObjectCategory.Column
                  || targetContext.ObjectCategory == DatabaseObjectCategory.Relationship
                   || targetContext.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                    {
                        (sourceNode.Parent as RadTreeViewItem).Items.Remove(sourceNode);
                        (targetNode.Parent as RadTreeViewItem).Items.Insert(GetTreeViewItemIndex((targetNode.Parent as RadTreeViewItem).Items, targetNode) + 1, sourceNode);
                    }
                }
                else if (sourceContext.ObjectCategory == DatabaseObjectCategory.TabPage)
                {
                    if (targetContext.ObjectCategory == DatabaseObjectCategory.TabControl)
                    {
                        (sourceNode.Parent as RadTreeViewItem).Items.Remove(sourceNode);
                        targetNode.Items.Add(sourceNode);
                    }
                    else if (targetContext.ObjectCategory == DatabaseObjectCategory.TabPage)
                    {
                        (sourceNode.Parent as RadTreeViewItem).Items.Remove(sourceNode);
                        (targetNode.Parent as RadTreeViewItem).Items.Insert(GetTreeViewItemIndex((targetNode.Parent as RadTreeViewItem).Items, targetNode) + 1, sourceNode);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private bool IsNodeInChilds(ItemCollection items, RadTreeViewItem targetNode)
        {
            foreach (RadTreeViewItem item in items)
            {
                if (item == targetNode)
                    return true;
                else
                    return IsNodeInChilds(item.Items, targetNode);
            }
            return false;
        }
        //private void SetEntityUICompositionIcons(ItemCollection treeNodeCollection)
        //{
        //    foreach (RadTreeViewItem node in treeNodeCollection)
        //    {
        //        SetNodeImage(node);
        //        SetEntityUICompositionIcons(node.Nodes);
        //    }
        //}
        private void menuNavigation_Opened(object sender, RoutedEventArgs e)
        {
            mnuDelete.Visibility = Visibility.Collapsed;
            mnuGroup.Visibility = Visibility.Collapsed;
            mnuEmptySpace.Visibility = Visibility.Collapsed;
            mnuTabControl.Visibility = Visibility.Collapsed;
            mnuTabPage.Visibility = Visibility.Collapsed;
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null && node.DataContext is EntityUICompositionDTO)
            {
                var context = node.DataContext as EntityUICompositionDTO;

                if (context.ObjectCategory == DatabaseObjectCategory.Entity
                    || context.ObjectCategory == DatabaseObjectCategory.Group
                    || context.ObjectCategory == DatabaseObjectCategory.TabPage
                     || context.ObjectCategory == DatabaseObjectCategory.Column
                      || context.ObjectCategory == DatabaseObjectCategory.Relationship
                      || context.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                {
                    mnuGroup.Visibility = Visibility.Visible;
                    mnuEmptySpace.Visibility = Visibility.Visible;
                    mnuTabControl.Visibility = Visibility.Visible;
                }
                else if (context.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    mnuTabPage.Visibility = Visibility.Visible;
                }
                if (context.ObjectCategory == DatabaseObjectCategory.Group
                    || context.ObjectCategory == DatabaseObjectCategory.EmptySpace
                    || context.ObjectCategory == DatabaseObjectCategory.TabPage
                    || context.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    if (!HasColumnOrRelationshipChild(node.Items))
                        mnuDelete.Visibility = Visibility.Visible;
                }

            }
        }

        private bool HasColumnOrRelationshipChild(ItemCollection items)
        {
            foreach (RadTreeViewItem node in items)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                if (context != null)
                {
                    if (context.ObjectCategory == DatabaseObjectCategory.Column
                         || context.ObjectCategory == DatabaseObjectCategory.Relationship)
                        return true;
                }
                return HasColumnOrRelationshipChild(node.Items);
            }
            return false;
        }



        private void MenuItemAddGroup_Click(object sender, RoutedEventArgs e)
        {
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null && node.DataContext is EntityUICompositionDTO)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                if (context.ObjectCategory == DatabaseObjectCategory.Entity
                    || context.ObjectCategory == DatabaseObjectCategory.Group
                    || context.ObjectCategory == DatabaseObjectCategory.TabPage)
                {
                    var newNode = AddNode(node.Items, "گروه جدید", DatabaseObjectCategory.Group);
                    node.Items.Remove(newNode);
                    node.Items.Insert(0, newNode);
                    newNode.IsSelected = true;
                }
                else if (context.ObjectCategory == DatabaseObjectCategory.Column
                          || context.ObjectCategory == DatabaseObjectCategory.Relationship
                           || context.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                {
                    var newNode = AddNode((node.Parent as RadTreeViewItem).Items, "گروه جدید", DatabaseObjectCategory.Group);
                    (node.Parent as RadTreeViewItem).Items.Remove(newNode);
                    (node.Parent as RadTreeViewItem).Items.Insert(GetTreeViewItemIndex((node.Parent as RadTreeViewItem).Items, node) + 1, newNode);

                    newNode.IsSelected = true;
                }
            }
        }
        private int GetTreeViewItemIndex(ItemCollection items, RadTreeViewItem item)
        {
            int index = 0;
            foreach (var _item in items)
            {
                if (_item == item)
                {
                    return index;
                }
                index++;
            }
            return 0;
        }
        private void mnuTabPage_Click(object sender, RoutedEventArgs e)
        {
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null && node.DataContext is EntityUICompositionDTO)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                if (context.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    var newNode = AddNode(node.Items, "تب جدید", DatabaseObjectCategory.TabPage);
                    newNode.IsSelected = true;
                }
            }
        }
        private void MenuItemAddTabControl_Click(object sender, RoutedEventArgs e)
        {
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null && node.DataContext is EntityUICompositionDTO)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                if (context.ObjectCategory == DatabaseObjectCategory.Entity
                    || context.ObjectCategory == DatabaseObjectCategory.Group
                    || context.ObjectCategory == DatabaseObjectCategory.TabPage)
                {
                    var newNode = AddNode(node.Items, "گروه تب جدید", DatabaseObjectCategory.TabControl);
                    node.Items.Remove(newNode);
                    node.Items.Insert(0, newNode);
                    newNode.IsSelected = true;
                }
                else if (context.ObjectCategory == DatabaseObjectCategory.Column
                          || context.ObjectCategory == DatabaseObjectCategory.Relationship
                           || context.ObjectCategory == DatabaseObjectCategory.Relationship)
                {
                    var newNode = AddNode((node.Parent as RadTreeViewItem).Items, "گروه تب جدید", DatabaseObjectCategory.TabControl);
                    (node.Parent as RadTreeViewItem).Items.Remove(newNode);
                    (node.Parent as RadTreeViewItem).Items.Insert(GetTreeViewItemIndex((node.Parent as RadTreeViewItem).Items, node) + 1, newNode);

                    newNode.IsSelected = true;
                }
            }
        }
        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null && node.DataContext is EntityUICompositionDTO)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                if (node != treeEntityUIComposition.Items[0])
                {
                    if (context.ObjectCategory == DatabaseObjectCategory.Group
                     || context.ObjectCategory == DatabaseObjectCategory.TabPage
                     || context.ObjectCategory == DatabaseObjectCategory.TabControl
                        || context.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                    {
                        if (!HasColumnOrRelationshipChild(node.Items))
                            (node.Parent as RadTreeViewItem).Items.Remove(node);
                    }
                }
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.IsEnabled == true)
            {
                var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
                var context = node.DataContext as EntityUICompositionDTO;

                if (context.ObjectCategory == DatabaseObjectCategory.Group
                     || context.ObjectCategory == DatabaseObjectCategory.TabPage
                     || context.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    context.Title = txtName.Text;
                    node.Header = GetNodeHeader(context.Title, context.ObjectCategory);
                }
            }

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            var rootNode = (treeEntityUIComposition.Items[0] as RadTreeViewItem);

            CollectEntityUICompositionItems(rootNode);
            EntityUICompositionDTO item = rootNode.DataContext as EntityUICompositionDTO;

            bizEntityUIComposition.Save(EntityID, item);
            PopulateTree();
        }
        private void CollectEntityUICompositionItems(RadTreeViewItem parentNode)
        {
            var parentContext = parentNode.DataContext as EntityUICompositionDTO;
            int index = 0;
            parentContext.ChildItems.Clear();
            foreach (RadTreeViewItem node in parentNode.Items)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                context.ParentItem = parentContext;
                context.Position = index;
               
                
                parentContext.ChildItems.Add(context);
                index++;
                CollectEntityUICompositionItems(node);
            }

        }
        public EntityUICompositionDTO GetEntityUICompositionComposite()
        {
            EntityUICompositionDTO result = new EntityUICompositionDTO();
            var rootNode = (treeEntityUIComposition.Items[0] as RadTreeViewItem);
            var context = rootNode.DataContext as EntityUICompositionDTO;
            SetEntityUICompositionDTO(rootNode);
            return context;
        }

        private void SetEntityUICompositionDTO(RadTreeViewItem parentNode)
        {
            var parentContext = parentNode.DataContext as EntityUICompositionDTO;
            int index = 0;
            parentContext.ChildItems.Clear();
            foreach (RadTreeViewItem node in parentNode.Items)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                context.ParentItem = parentContext;
                parentContext.ChildItems.Add(context);
                context.Position = index;
                index++;

                //if (context.ObjectCategory == DatabaseObjectCategory.Column)
                //{
                //    columnItems.Add(context.ColumnUISetting);
                //}
                //else if (context.ObjectCategory == DatabaseObjectCategory.Relationship)
                //{
                //    relationshipItems.Add(context.RelationshipUISetting);
                //}

                SetEntityUICompositionDTO(node);
            }
        }

        private void btmMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null)
            {
                var context = node.DataContext as EntityUICompositionDTO;

                if (context.ObjectCategory != DatabaseObjectCategory.Entity)
                {
                    var parentNode = (node.Parent as RadTreeViewItem);
                    var nodeIndex = GetTreeViewItemIndex(parentNode.Items, node);
                    if (nodeIndex != 0)
                    {
                        parentNode.Items.Remove(node);
                        parentNode.Items.Insert(nodeIndex - 1, node);
                    }
                    node.IsSelected = true;
                }
            }
        }

        private void btmMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null)
            {
                var context = node.DataContext as EntityUICompositionDTO;

                if (context.ObjectCategory != DatabaseObjectCategory.Entity)
                {
                    var nodeIndex = GetTreeViewItemIndex((node.Parent as RadTreeViewItem).Items, node);
                    var parentNode = (node.Parent as RadTreeViewItem);
                    if (nodeIndex != parentNode.Items.Count - 1)
                    {
                        parentNode.Items.Remove(node);
                        parentNode.Items.Insert(nodeIndex + 1, node);
                    }
                    node.IsSelected = true;
                }
            }
        }
        EntityUICompositionDTO SelectedTreeItem { set; get; }
        private void treeEntityUIComposition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTreeItem = null;
            txtName.IsEnabled = false;
            txtName.Text = "";
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null && node.DataContext is EntityUICompositionDTO)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                SelectedTreeItem = context;
                if (context.ObjectCategory == DatabaseObjectCategory.Group
                     || context.ObjectCategory == DatabaseObjectCategory.TabPage
                     || context.ObjectCategory == DatabaseObjectCategory.TabControl)
                {
                    txtName.IsEnabled = true;
                    txtName.Text = context.Title;
                }

                SetUISettingPanels(context);
            }



        }
        bool change = true;
        private void SetUISettingPanels(EntityUICompositionDTO context)
        {

            HideAllPanels();
            lblSelectedItem.Text = context.Title;
            //grdInfo.Visibility = Visibility.Visible;
            change = false;
            if (context.ObjectCategory == DatabaseObjectCategory.Entity && context.EntityUISetting != null)
            {
                grdEntity.Visibility = Visibility.Visible;
                txtEntityColumnsCount.Text = context.EntityUISetting.UIColumnsCount.ToString();

            }
            else if (context.ObjectCategory == DatabaseObjectCategory.Column && context.ColumnUISetting != null)
            {
                grdColumns.Visibility = Visibility.Visible;
                cmbColumnColumnsCount.SelectedItem = context.ColumnUISetting.UIColumnsType;
                txtColumnRowsCount.Text = context.ColumnUISetting.UIRowsCount.ToString();
            }
            else if (context.ObjectCategory == DatabaseObjectCategory.Relationship && context.RelationshipUISetting != null)
            {
                grdRelationships.Visibility = Visibility.Visible;
                cmbRelationshipColumnsCount.SelectedItem = context.RelationshipUISetting.UIColumnsType;
                chkRelationshipExpander.IsChecked = context.RelationshipUISetting.Expander;
                chkRelationshipIsExpanded.IsChecked = context.RelationshipUISetting.IsExpanded;
            }
            else if (context.ObjectCategory == DatabaseObjectCategory.Group && context.GroupUISetting != null)
            {
                grdGroups.Visibility = Visibility.Visible;
                txtGroupInternalColumnsCount.Text = context.GroupUISetting.InternalColumnsCount.ToString();
                cmbGroupColumnsCount.SelectedItem = context.GroupUISetting.UIColumnsType;
                chkGroupExpander.IsChecked = context.GroupUISetting.Expander;
                chkGroupIsExpanded.IsChecked = context.GroupUISetting.IsExpanded;
            }
            else if (context.ObjectCategory == DatabaseObjectCategory.TabControl && context.TabGroupUISetting != null)
            {
                grdTabGroups.Visibility = Visibility.Visible;
                cmbTabGroupColumnsCount.SelectedItem = context.TabGroupUISetting.UIColumnsType;
                chkTabGroupExpander.IsChecked = context.TabGroupUISetting.Expander;
                chkTabGroupIsExpanded.IsChecked = context.TabGroupUISetting.IsExpanded;
            }
            else if (context.ObjectCategory == DatabaseObjectCategory.TabPage && context.TabPageUISetting != null)
            {
                grdTabPages.Visibility = Visibility.Visible;
                txtTabPageColumnsCount.Text = context.TabPageUISetting.InternalColumnsCount.ToString();
            }
            else if (context.ObjectCategory == DatabaseObjectCategory.EmptySpace && context.EmptySpaceUISetting != null)
            {
                grdEmptySpace.Visibility = Visibility.Visible;
                cmbEmptySpaceColumnsCount.SelectedItem = context.EmptySpaceUISetting.UIColumnsType;
                chkExtendToEnd.IsChecked = context.EmptySpaceUISetting.ExpandToEnd;
            }
            change = true;
        }

        private void HideAllPanels()
        {
            //grdInfo.Visibility = Visibility.Collapsed;
            grdEntity.Visibility = Visibility.Collapsed;
            grdColumns.Visibility = Visibility.Collapsed;
            grdRelationships.Visibility = Visibility.Collapsed;
            grdGroups.Visibility = Visibility.Collapsed;
            grdTabGroups.Visibility = Visibility.Collapsed;
            grdTabPages.Visibility = Visibility.Collapsed;
            grdEmptySpace.Visibility = Visibility.Collapsed;
        }

        private void txtEntityColumnsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.EntityUISetting.UIColumnsCount = (txtEntityColumnsCount.Text == "" ? (Int16)0 : Convert.ToInt16(txtEntityColumnsCount.Text));
        }
        private void cmbColumnColumnsCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.ColumnUISetting.UIColumnsType = (Enum_UIColumnsType)cmbColumnColumnsCount.SelectedItem;
        }
        private void txtColumnRowsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.ColumnUISetting.UIRowsCount = (txtColumnRowsCount.Text == "" ? (Int16)0 : Convert.ToInt16(txtColumnRowsCount.Text));
        }
        private void cmbRelationshipColumnsCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.RelationshipUISetting.UIColumnsType = (Enum_UIColumnsType)cmbRelationshipColumnsCount.SelectedItem;
        }
        private void chkRelationshipExpander_Checked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.RelationshipUISetting.Expander = chkRelationshipExpander.IsChecked == true;
        }
        private void cmbGroupColumnsCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.GroupUISetting.UIColumnsType = (Enum_UIColumnsType)cmbGroupColumnsCount.SelectedItem;
        }
        private void txtGroupInternalColumnsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.GroupUISetting.InternalColumnsCount = (txtGroupInternalColumnsCount.Text == "" ? (Int16)0 : Convert.ToInt16(txtGroupInternalColumnsCount.Text));
        }
        private void chkGroupExpander_Checked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.GroupUISetting.Expander = chkGroupExpander.IsChecked == true;
        }
        private void cmbTabGroupColumnsCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.TabGroupUISetting.UIColumnsType = (Enum_UIColumnsType)cmbTabGroupColumnsCount.SelectedItem;
        }
        private void chkTabGroupExpander_Checked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.TabGroupUISetting.Expander = chkTabGroupExpander.IsChecked == true;
        }
        private void txtTabPageColumnsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.TabPageUISetting.InternalColumnsCount = (txtTabPageColumnsCount.Text == "" ? (Int16)0 : Convert.ToInt16(txtTabPageColumnsCount.Text));
        }
        //   I_EditEntityArea EditEntityArea { set; get; }
        //   I_EditEntityArea MultipleEditEntityArea { set; get; }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.SetUIManager(new UIManager());
            var userInfo = new MyUILibrary.UserInfo();
            userInfo.AdminSecurityInfo = new MyUILibrary.AdminSecurityInfo() { IsActive = true, ByPassSecurity = true };
            MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.UserInfo = userInfo;

            brdView.Child = null;

            //I_EditEntityArea currentEditArea = null;
            //if (chkMultiple.IsChecked == false)
            //    currentEditArea = EditEntityArea;
            //else
            //    currentEditArea = MultipleEditEntityArea;

            //if (currentEditArea == null)
            //{

            DR_Requester requester = new DR_Requester();
            requester.SkipSecurity = true;
            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            var dataEntryEntity = bizTableDrivedEntity.GetDataEntryEntity(requester, EntityID, GetEntityUICompositionComposite());

            var initializer = new EditEntityAreaInitializer();
            initializer.Preview = true;
            initializer.EntityID = EntityID;
            if (chkMultiple.IsChecked == false)
                initializer.DataMode = CommonDefinitions.UISettings.DataMode.One;
            else
                initializer.DataMode = CommonDefinitions.UISettings.DataMode.Multiple;
            initializer.PreviewDataEntryEntity = dataEntryEntity;
            var currentEditArea = BaseEditEntityArea.GetEditEntityArea(initializer).Item1;


            //if (chkMultiple.IsChecked == false)
            //    EditEntityArea = currentEditArea;
            //else
            //    MultipleEditEntityArea = currentEditArea;
            //}


            //    currentEditArea.GenerateDataViewPreview(dataEntryEntity);
            brdView.Child = currentEditArea.DataViewGeneric as UIElement;

        }


        private void mnuEmptySpace_Click(object sender, RoutedEventArgs e)
        {
            var node = treeEntityUIComposition.SelectedItem as RadTreeViewItem;
            if (node != null && node.DataContext is EntityUICompositionDTO)
            {
                var context = node.DataContext as EntityUICompositionDTO;
                if (context.ObjectCategory == DatabaseObjectCategory.Entity
                    || context.ObjectCategory == DatabaseObjectCategory.Group
                    || context.ObjectCategory == DatabaseObjectCategory.TabPage)
                {
                    var newNode = AddNode(node.Items, "فضای خالی", DatabaseObjectCategory.EmptySpace);
                    node.Items.Remove(newNode);
                    node.Items.Insert(0, newNode);
                    newNode.IsSelected = true;
                }
                else if (context.ObjectCategory == DatabaseObjectCategory.Column
                          || context.ObjectCategory == DatabaseObjectCategory.Relationship
                            || context.ObjectCategory == DatabaseObjectCategory.EmptySpace)
                {
                    var newNode = AddNode((node.Parent as RadTreeViewItem).Items, "فضای خالی", DatabaseObjectCategory.EmptySpace);
                    (node.Parent as RadTreeViewItem).Items.Remove(newNode);
                    (node.Parent as RadTreeViewItem).Items.Insert(GetTreeViewItemIndex((node.Parent as RadTreeViewItem).Items, node) + 1, newNode);

                    newNode.IsSelected = true;
                }
            }
        }
        private void chkExtendToEnd_Checked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
            {
                SelectedTreeItem.EmptySpaceUISetting.ExpandToEnd = chkExtendToEnd.IsChecked == true;
                cmbEmptySpaceColumnsCount.IsEnabled = false;
            }
        }
        private void chkExtendToEnd_Unchecked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
            {
                SelectedTreeItem.EmptySpaceUISetting.ExpandToEnd = chkExtendToEnd.IsChecked == true;
                cmbEmptySpaceColumnsCount.IsEnabled = true;
            }
        }
        private void cmbEmptySpaceColumnsCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.EmptySpaceUISetting.UIColumnsType = (Enum_UIColumnsType)cmbEmptySpaceColumnsCount.SelectedItem;
        }
        private void txtRelationshipRowsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.RelationshipUISetting.UIRowsCount = (txtRelationshipRowsCount.Text == "" ? (Int16)0 : Convert.ToInt16(txtRelationshipRowsCount.Text));
        }
        private void txtGroupRowsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.GroupUISetting.UIRowsCount = (txtGroupRowsCount.Text == "" ? (Int16)0 : Convert.ToInt16(txtGroupRowsCount.Text));
        }
        private void txtTabGroupRowsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.TabGroupUISetting.UIRowsCount = (txtTabGroupRowsCount.Text == "" ? (Int16)0 : Convert.ToInt16(txtTabGroupRowsCount.Text));
        }
        private void chkRelationshipIsExpanded_Checked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.RelationshipUISetting.IsExpanded = chkRelationshipIsExpanded.IsChecked == true;
        }
        private void chkGroupIsExpanded_Checked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.GroupUISetting.IsExpanded = chkGroupIsExpanded.IsChecked == true;
        }
        private void chkTabGroupIsExpanded_Checked(object sender, RoutedEventArgs e)
        {
            if (change && SelectedTreeItem != null)
                SelectedTreeItem.TabGroupUISetting.IsExpanded = chkTabGroupIsExpanded.IsChecked == true;
        }
    }
    class ColumnOrRelationship
    {
        public ColumnOrRelationship()
        {
            RelationshipColumn = new List<int>();
        }
        public int ColumnID { set; get; }
        public int RelationshipID { set; get; }
        public List<int> RelationshipColumn { set; get; }
        public bool visited { set; get; }
    }

}
