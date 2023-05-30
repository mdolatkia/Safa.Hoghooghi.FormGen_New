
using ModelEntites;
using MyUILibrary;
using MyUILibrary.EntityArea;
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
using ProxyLibrary;
using System.Windows.Threading;
using MyUILibrary.Temp;

namespace MyUIGenerator.View
{
    /// <summary>
    /// Interaction logic for UC_TemporaryViewLink.xaml
    /// </summary>
    public partial class UC_TemporaryDataSearchLink : UserControl, I_View_TemporaryView
    {
      //  DispatcherTimer timer = new DispatcherTimer();
        public Brush DefaultBorderBrush { get; }
        public Thickness DefaultBorderThickness { get; }
        public Brush DefaultBackground { get; }
        public Brush DefaultForeground { get; }

        public bool ButtonPopupVisible { get => btnPopup.Visibility == Visibility.Visible; set => btnPopup.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        //   public bool ButtonQuickSearchVisible { get => btnQuickSearch.Visibility == Visibility.Visible; set => btnQuickSearch.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        public bool InfoTextboxReadOnly { get => txtInfo.IsReadOnly; set => txtInfo.IsReadOnly = value; }
        public bool ButtonDataEditVisible { get => btnDataEdit.Visibility == Visibility.Visible; set => btnDataEdit.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        public bool ButtonSearchFormVisible { get => btnSearchForm.Visibility == Visibility.Visible; set => btnSearchForm.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        public bool ButtonClearVisible { get => btnLinkClear.Visibility == Visibility.Visible; set => btnLinkClear.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        public bool ButtonInfoVisible { get => btnLinkInfo.Visibility == Visibility.Visible; set => btnLinkInfo.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

        public bool ButtonPopupEnabled { get => btnPopup.IsEnabled; set => btnPopup.IsEnabled = value; }
        //    public bool ButtonQuickSearchEnabled { get => btnQuickSearch.IsEnabled; set => btnQuickSearch.IsEnabled = value; }
        public bool ButtonDataEditEnabled { get => btnDataEdit.IsEnabled; set => btnDataEdit.IsEnabled = value; }
        public bool ButtonSearchFormEnabled { get => btnSearchForm.IsEnabled; set => btnSearchForm.IsEnabled = value; }
        public bool ButtonClearEnabled { get => btnLinkClear.IsEnabled; set => btnLinkClear.IsEnabled = value; }


        public UC_TemporaryDataSearchLink()
        {
            InitializeComponent();
            //this.Margin = new Thickness(2);
            // TemporaryLinkState = temporaryLinkState;
            //LinkType = linkType;
            //btnLinkClear.Visibility = Visibility.Collapsed;
            //btnLinkInfo.Visibility = Visibility.Collapsed;


            this.LostFocus += UC_TemporaryDataSearchLink_LostFocus;

            //txtInfo.TextChanged += TxtSearch_TextChanged;
            txtInfo.KeyUp += TxtInfo_KeyUp;
          //  timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        //    timer.Tick += Timer_Tick;

            DefaultBorderBrush = this.BorderBrush;
            DefaultBorderThickness = this.BorderThickness;
            DefaultBackground = this.Background;
            DefaultForeground = this.Foreground;
        }

        private void TxtInfo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchTextChanged != null)
                    SearchTextChanged(this, new Arg_TemporaryDisplaySerachText() { Text = txtInfo.Text });
            }
        }

        public UC_TemporaryDataSearchLink(I_View_TemporaryView mainView)
        {
            InitializeComponent();
            //this.Margin = new Thickness(2);
            // TemporaryLinkState = temporaryLinkState;
            //LinkType = linkType;
            //btnLinkClear.Visibility = Visibility.Collapsed;
            //btnLinkInfo.Visibility = Visibility.Collapsed;
            ButtonPopupVisible = mainView.ButtonPopupVisible;
            //    ButtonQuickSearchVisible = mainView.ButtonQuickSearchVisible;
            InfoTextboxReadOnly = mainView.InfoTextboxReadOnly;
            ButtonDataEditVisible = mainView.ButtonDataEditVisible;
            ButtonSearchFormVisible = mainView.ButtonSearchFormVisible;
            ButtonClearVisible = mainView.ButtonClearVisible;
            ButtonInfoVisible = mainView.ButtonInfoVisible;

            this.LostFocus += UC_TemporaryDataSearchLink_LostFocus;

            //   txtInfo.TextChanged += TxtSearch_TextChanged;
            txtInfo.KeyUp += TxtInfo_KeyUp;
          //  timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
          //  timer.Tick += Timer_Tick;

            DefaultBorderBrush = this.BorderBrush;
            DefaultBorderThickness = this.BorderThickness;
            DefaultBackground = this.Background;
            DefaultForeground = this.Foreground;
        }
        public void Visiblity(bool visible)
        {
            this.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }


        private void UC_TemporaryDataSearchLink_LostFocus(object sender, RoutedEventArgs e)
        {
            var aa = Keyboard.FocusedElement;
            bool focusIsInside = true;
            if (aa != null)
            {
                if (aa is FrameworkElement)
                {
                    if (!ElementOrParentIsInside(aa as FrameworkElement))
                        focusIsInside = false;
                }
            }
            //   FrameworkElement focusedElement = FocusManager.GetFocusedElement(popup1) as FrameworkElement;
            if (!focusIsInside)
            {
                if (FocusLost != null)
                    FocusLost(this, null);
            }
        }
        private bool ElementOrParentIsInside(FrameworkElement uIElement)
        {
            if (uIElement == this)
                return true;
            else if (uIElement.Parent != null && uIElement.Parent is FrameworkElement)
                return ElementOrParentIsInside(uIElement.Parent as FrameworkElement);
            else
            {
                var parent = VisualTreeHelper.GetParent(uIElement);
                if (parent != null && parent is FrameworkElement)
                    return ElementOrParentIsInside(parent as FrameworkElement);
                else
                    return false;
            }
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    (sender as DispatcherTimer).Stop();
        //    if (SearchTextChanged != null)
        //        SearchTextChanged(this, new Arg_TemporaryDisplaySerachText() { Text = txtInfo.Text });


        //}
        //private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    timer.Start();
        //}
        public event EventHandler FocusLost;
        public event EventHandler<Arg_TemporaryDisplayViewRequested> TemporaryDisplayViewRequested;
        public event EventHandler<Arg_TemporaryDisplaySerachText> SearchTextChanged;

        //public event EventHandler<Arg_TemporaryDisplayViewRequested> TemporarySearchViewRequested;

        public void SetLinkText(string text)
        {
            //UC_TemporaryDataSearchLink.SetLinkText: 9168b4b9155f
            txtInfo.Text = text;
        }

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            if (TemporaryDisplayViewRequested != null)
            {
                TemporaryDisplayViewRequested(this, new Arg_TemporaryDisplayViewRequested() { LinkType = TemporaryLinkType.DataView });

            }
        }

        //public DP_DataRepository ParentDataItem
        //{
        //    set;
        //    get;
        //}

        private void btnLinkSearch_Click(object sender, RoutedEventArgs e)
        {
            if (TemporaryDisplayViewRequested != null)
            {
                TemporaryDisplayViewRequested(this, new Arg_TemporaryDisplayViewRequested() { LinkType = TemporaryLinkType.SerachView });

            }
        }
        public void SetTooltip(object dataItem, string tooltip)
        {
            if (!string.IsNullOrEmpty(tooltip))
                ToolTipService.SetToolTip(this, tooltip);
            else
                ToolTipService.SetToolTip(this, null);
        }

        public void SetColor(object dataItem, InfoColor color)
        {
            this.BorderBrush = UIManager.GetColorFromInfoColor(color);
            this.BorderThickness = new Thickness(1);
        }

        //public void SetBackgroundColor(object dataItem, InfoColor color)
        //{
        //    this.Background = UIManager.GetColorFromInfoColor(color);
        //}

        //public void SetForegroundColor(object dataItem, InfoColor color)
        //{
        //    this.Foreground = UIManager.GetColorFromInfoColor(color);
        //}
        //public object SearchDataItem
        //{
        //    set;
        //    get;
        //}

        //public void SetSearchLinkText(string text)
        //{
        //    btnLinkSearch.Content = text;
        //}

        //public ColumnDTO Column
        //{
        //    set;
        //    get;
        //}

        //   public TemporaryLinkType LinkType { get; }



        public List<DP_DataRepository> CurrentDataItems
        {
            set; get;
        }



        private void btnLinkInfo_Click(object sender, RoutedEventArgs e)
        {
            if (TemporaryDisplayViewRequested != null)
            {
                TemporaryDisplayViewRequested(this, new Arg_TemporaryDisplayViewRequested() { LinkType = TemporaryLinkType.Info });
            }
        }

        private void btnLinkClear_Click(object sender, RoutedEventArgs e)
        {
            if (TemporaryDisplayViewRequested != null)
            {
                TemporaryDisplayViewRequested(this, new Arg_TemporaryDisplayViewRequested() { LinkType = TemporaryLinkType.Clear });
            }
        }

        public void DisableEnable(bool enable)
        {
            this.IsEnabled = enable;
        }

        //public void DisableEnable(TemporaryLinkType link, bool enable)
        //{
        //    if (link == TemporaryLinkType.SerachView)
        //    {
        //        btnLinkSearch.IsEnabled = enable;
        //    }
        //    else if (link == TemporaryLinkType.DataView)
        //    {
        //        btnLink.IsEnabled = enable;
        //    }
        //    else if (link == TemporaryLinkType.Clear)
        //    {
        //        btnLinkClear.IsEnabled = enable;
        //    }
        //    else if (link == TemporaryLinkType.QuickSearch)
        //    {
        //        btnQuickSearch.IsEnabled = enable;
        //    }
        //    else if (link == TemporaryLinkType.Popup)
        //    {
        //        btnPopup.IsEnabled = enable;
        //    }
        //}

        //public void SetComboBoxVisibile(bool combo)
        //{
        //    if (combo)
        //    {
        //        cmbItems.Visibility = Visibility.Visible;
        //        txtInfo.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        cmbItems.Visibility = Visibility.Collapsed;
        //        txtInfo.Visibility = Visibility.Visible;
        //    }
        //}

        //public void SetTextReadonly(bool isreadonly)
        //{
        //    txtInfo.IsReadOnly = isreadonly;
        //}

        public string GetSearchText()
        {
            return txtInfo.Text;
        }

        public void RemovePopupView(I_View_ViewEntityArea viewView)
        {
            popup1.Child = null;
        }

        public bool PopupVisibility
        {
            get
            {
                return popup1.IsOpen;
            }
            set
            {
                popup1.IsOpen = value;
            }
        }

        public bool HasPopupView
        {
            get
            {
                return popup1.Child != null;
            }
        }

        //public bool QuickSearchVisibility
        //{
        //    get
        //    {
        //        return txtSearch.Visibility == Visibility.Visible;
        //    }

        //    set
        //    {
        //        txtSearch.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        //    }
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
        public void AddPopupView(I_View_ViewEntityArea viewView)
        {
            popup1.Child = viewView as UIElement;
        }

        //private void btnQuickSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    if (TemporaryDisplayViewRequested != null)
        //    {
        //        TemporaryDisplayViewRequested(this, new Arg_TemporaryDisplayViewRequested() { LinkType = TemporaryLinkType.QuickSearch });

        //    }
        //}

        private void btnPopup_Click(object sender, RoutedEventArgs e)
        {
            if (TemporaryDisplayViewRequested != null)
            {
                TemporaryDisplayViewRequested(this, new Arg_TemporaryDisplayViewRequested() { LinkType = TemporaryLinkType.Popup });
            }
        }

        public void QuickSearchSelectAll()
        {
            txtInfo.SelectAll();
            txtInfo.Focus();
        }

        public void ClearSearchText()
        {
            txtInfo.Text = "";
        }

        public void DeHighlightCommands()
        {

        }

        public void AddCommand(I_CommandManager command, bool indirect = false)
        {

        }
        public void ClearCommands()
        {

        }
        public void EnableDisable(bool enable)
        {
            this.IsEnabled = enable;
        }

        public void EnableDisableDataSection(bool enable)
        {
            throw new NotImplementedException();
        }
        //public void SetBackgroundColor(string color)
        //{
        //    throw new NotImplementedException();
        //}


    }
}
