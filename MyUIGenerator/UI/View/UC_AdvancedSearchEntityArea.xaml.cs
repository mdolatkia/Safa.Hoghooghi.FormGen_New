
using ModelEntites;
using MyUIGenerator.UIControlHelper;
using MyUILibrary;
using MyUILibrary.EntityArea;
using MyUILibrary.EntityArea.Commands;
using MyUILibrary.Temp;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace MyUIGenerator.View
{
    /// <summary>
    /// Interaction logic for UC_ViewPackageArea.xaml
    /// </summary>
    public partial class UC_AdvancedSearchEntityArea : UserControl, I_View_AdvancedSearchEntityArea
    {

        public Brush DefaultBorderBrush { get; }
        public Thickness DefaultBorderThickness { get; }
        public Brush DefaultBackground { get; }
        public Brush DefaultForeground { get; }
        //View_Container View_Container { set; get; }
        public UC_AdvancedSearchEntityArea()

        {
            InitializeComponent();

            //View_Container = new View_Container(basicGridSetting);
            //if (AgentHelper.GetAppMode() != AppMode.Paper)
            FlowDirection = System.Windows.FlowDirection.RightToLeft;
            //grdArea.Children.Add(View_Container.Grid);


            DefaultBorderBrush = this.BorderBrush;
            DefaultBorderThickness = this.BorderThickness;
            DefaultBackground = this.Background;
            DefaultForeground = this.Foreground;

        }

        public event EventHandler<Arg_CommandExecuted> CommandExecuted;

        public void EnableDisable(bool enable)
        {
            this.IsEnabled = enable;
        }
        //public override Expander Expander
        //{
        //    get
        //    {
        //        return expander;
        //    }
        //}

        //public override Grid expanderHeader
        //{
        //    get
        //    {
        //        return grdExpanderHeader;
        //    }
        //}
        //private void InitializePackageArea()
        //{
        //    //foreach (var type in TemplatePackage.TypeConditions)
        //    //{
        //    //    //foreach (var property in type.NDType.Properties)
        //    //    //{
        //    //    //    var column = new DataGridViewTextBoxColumn();
        //    //    //    column.HeaderText = (property.Title == null ? property.Property.Title : property.Title);
        //    //    //    column.Tag = property;
        //    //    //    column.Name = property.Property.Name;
        //    //    //    //column.CellType=GetType(string);
        //    //    //    dataGridView1.Columns.Add(column);
        //    //    //}
        //    //}

        //}


        //public I_SearchViewEntityArea Controller
        //{
        //    set;
        //    get;
        //}
        public bool IsOpenedTemporary { set; get; }
        public EntityUICompositionDTO UICompositionDTO { set; get; }

        public void SetTooltip(string tooltip)
        {
            if (!string.IsNullOrEmpty(tooltip))
                ToolTipService.SetToolTip(this, tooltip);
            else
                ToolTipService.SetToolTip(this, null);
        }
        public void SetBorderColor(InfoColor color)
        {
            this.BorderBrush = UIManager.GetColorFromInfoColor(color);
            this.BorderThickness = new Thickness(1);
        }
        public void SetColor(InfoColor color)
        {
            this.BorderBrush = UIManager.GetColorFromInfoColor(color);
            this.BorderThickness = new Thickness(1);
        }
        public void SetBackgroundColor(InfoColor color)
        {
            this.Background = UIManager.GetColorFromInfoColor(color);
        }

        public void SetForegroundColor(InfoColor color)
        {
            this.Foreground = UIManager.GetColorFromInfoColor(color);
        }
        public void SetBorderColorDefault()
        {
            this.BorderBrush = DefaultBorderBrush;
            this.BorderThickness = DefaultBorderThickness;
        }

        public void SetBackgroundColorDefault()
        {
            this.Background = DefaultBackground;
        }

        public void SetForegroundColorDefault()
        {
            this.Background = DefaultForeground;
        }
        public void DeHighlightCommands()
        {
            toolbar.Background = null;
        }
        public void AddCommand(I_CommandManager item, bool indirect)
        {
            //Button btnCommand = UIHelper.GenerateCommand(item);
            //item.EnabledChanged += (sender, e) => item_EnabledChanged(sender, e, btnCommand);
            //btnCommand.Click += btnCommand_Click;
            toolbar.Items.Add((item as MyUIGenerator.UIControlHelper.CommandManager).Button);

        }
        public void ClearCommands()
        {
            toolbar.Items.Clear();
        }
        //private void item_EnabledChanged(object sender, EventArgs e, Button btnCommand)
        //{
        //    btnCommand.IsEnabled = (sender as I_Command).Enabled;
        //}
        //void btnCommand_Click(object sender, EventArgs e)
        //{
        //    if (CommandExecuted != null)
        //        CommandExecuted(this, new Arg_CommandExecuted() { Command = (sender as Button).Tag as I_Command });
        //    ////  Controller.CommandExecuted(((sender as Button).Tag as I_EntityAreaCommand));
        //}



        public void SetBackgroundColor(string color)
        {
            this.Background = new SolidColorBrush(UIHelper.getColorFromHexString(color));
        }

        public I_AdvanceSearchNodeManager AddTreeItem()
        {
            RadTreeViewItem node = new RadTreeViewItem();
            treeItems.Items.Add(node); ;
            var newAdvenveSearchNode = new AdvenveSearchNode(node);
            return newAdvenveSearchNode;
        }
        public void ClearTreeItems()
        {
            treeItems.Items.Clear();
        }

        public void DisableEnableDataSection(bool enable)
        {
            treeItems.IsEnabled = enable;
        }

        public void Visiblity(bool visible)
        {
            this.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }



        //public bool AddControlPackageToHeader(object uiControlPackage, string title, InfoColor titleColor, string tooltip = "")
        //{
        //    throw new NotImplementedException();
        //}
    }
    public class AdvenveSearchNode : I_AdvanceSearchNodeManager
    {
        public RadTreeViewItem Node;
        public AdvenveSearchNode(RadTreeViewItem node)
        {
            Node = node;
        }
        ContextMenu contextMenu = new ContextMenu();

        public event EventHandler<LogicComboBoxChangedEventArg> LogicComboBoxChanged;

        public I_AdvanceSearchMenu AddMenu(string header)
        {
            if (Node.ContextMenu == null)
                Node.ContextMenu = new ContextMenu();
            MenuItem menu = new MenuItem();
            menu.Header = header;
            AdvanceSearchMenu advanceSearchMenu = new AdvanceSearchMenu(menu);
            Node.ContextMenu.Items.Add(menu);
            menu.Click += (sender, e) => Menu_Click(sender, e, advanceSearchMenu);
            return advanceSearchMenu;
        }

        private void Menu_Click(object sender, RoutedEventArgs e, AdvanceSearchMenu advanceSearchMenu)
        {
            advanceSearchMenu.OnClicked();
        }

        public void SetHeader(string header, string tooltip)
        {
            var headerObj = new StackPanel();
            headerObj.Orientation = Orientation.Horizontal;
            headerObj.Children.Add(new TextBlock() { Text = header });
            var combobox = new ComboBox();
            combobox.Visibility = Visibility.Collapsed;
            headerObj.Children.Add(combobox);
            Node.Header = headerObj;

            if (!string.IsNullOrEmpty(tooltip))
                ToolTipService.SetToolTip(Node, tooltip);

        }
        public void RemoveItem(I_AdvanceSearchNodeManager nodeManager)
        {
            Node.Items.Remove((nodeManager as AdvenveSearchNode).Node);
        }
        public I_AdvanceSearchNodeManager AddChildItem()
        {
            RadTreeViewItem cnode = new RadTreeViewItem();
            Node.Items.Add(cnode);
            Node.IsExpanded = true;
            var newAdvenveSearchNode = new AdvenveSearchNode(cnode);
            return newAdvenveSearchNode;
        }

        public void AddExistingChildItem(I_AdvanceSearchNodeManager item)
        {
            Node.Items.Add((item as AdvenveSearchNode).Node);
        }

        public void ClearItems()
        {
            Node.Items.Clear();
        }

        public void AddLogicComboBox(List<AndORListItem> andORListItems)
        {
            var headerObj = Node.Header as StackPanel;
            var combobox = headerObj.Children[1] as ComboBox;
            combobox.Visibility = Visibility.Visible;
            combobox.ItemsSource = andORListItems;
            combobox.DisplayMemberPath = "Title";
            combobox.SelectedItem = andORListItems.FirstOrDefault(x => x.IsDefault) ?? andORListItems.First();
            combobox.SelectionChanged += Combobox_SelectionChanged;
        }

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                if (LogicComboBoxChanged != null)
                    LogicComboBoxChanged(this, new LogicComboBoxChangedEventArg() { Item = (sender as ComboBox).SelectedItem as AndORListItem });
            }
        }

        public void SetLogicComboBoxValue(AndORListItem andOrType)
        {
            var headerObj = Node.Header as StackPanel;
            var combobox = headerObj.Children[1] as ComboBox;
            combobox.SelectedItem = andOrType;
        }
    }
    public class AdvanceSearchMenu : I_AdvanceSearchMenu
    {
        MenuItem Menu { set; get; }
        public AdvanceSearchMenu(MenuItem menu)
        {
            Menu = menu;
        }

        public void OnClicked()
        {
            if (Clicked != null)
                Clicked(this, null);
        }
        public event EventHandler Clicked;
    }
}
