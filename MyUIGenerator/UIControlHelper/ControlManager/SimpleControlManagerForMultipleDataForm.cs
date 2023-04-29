﻿
using ModelEntites;
using MyUIGenerator;
using MyUIGenerator.UIControlHelper;
using MyUILibrary;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows;
using System.Windows.Threading;
using MyUILibrary.Temp;
using System.Windows.Media;

namespace MyUIGenerator.UIControlHelper
{
    public class SimpleControlManagerForMultipleDataForm : Telerik.Windows.Controls.GridViewColumn, I_SimpleControlManagerMultiple
    {
        public event EventHandler<ColumnValueChangeArg> ValueChanged;
        // DataMaster.EntityDefinition.ND_Type_Property TypeProperty { set; get; }
        public ColumnDTO Column { set; get; }
        public ColumnUISettingDTO ColumnSetting { set; get; }

        bool HasRangeOfValues;
        //   bool ValueIsTitleOrValue;

        ConrolPackageMenu CPMenu { set; get; }
        internal void GenerateMenu(ConrolPackageMenu cpMenu)
        {
            CPMenu = cpMenu;


        }


        public SimpleControlManagerForMultipleDataForm(ColumnDTO column, ColumnUISettingDTO columnSetting, bool hasRangeOfValues)
        {

            ColumnSetting = columnSetting;

            Column = column;
            HasRangeOfValues = hasRangeOfValues;
            //     ValueIsTitleOrValue = valueIsTitleOrValue;
            if (columnSetting != null)
            {
                if (columnSetting.UIColumnsType == Enum_UIColumnsType.Full)
                    Width = 200;
                else if (columnSetting.UIColumnsType == Enum_UIColumnsType.Half)
                    Width = 140;
                else
                    Width = 80;
            }

        }


        public override FrameworkElement CreateCellElement(GridViewCell cell, object dataItem)
        {
            //I_UIControlManager MyControlHelper = null;
            //if (HasRangeOfValues)
            //    MyControlHelper = ControlHelper.KeyValueControlHelper(Column);
            //else
            //    MyControlHelper = ControlHelper.GetControlHelper(Column, ColumnSetting, null);
            //MyControlHelper.SetReadonly(IsReadOnly);
            //if (MyControlHelper is I_ControlHelperValueRange)
            //{
            //    if (ColumnValueRange != null)
            //    {
            //        (MyControlHelper as I_ControlHelperValueRange).SetColumnValueRange(ColumnValueRange);
            //    }
            //}
            //cell.Tag = MyControlHelper;
            //if (ButtonMenus.Any())
            //{
            //    foreach (var item in ButtonMenus)
            //    {
            //        var newMenu = new ConrolPackageMenu();
            //        newMenu.Name = item.Name;
            //        newMenu.Title = item.Title;
            //        newMenu.MenuClicked += (sender, e) => Item_MenuClicked(sender, e, item, dataItem);
            //        MyControlHelper.AddButtonMenu(newMenu);
            //    }
            //}
            //return MyControlHelper.GetUIControl() as FrameworkElement;

            var ui = GetUIControlManager(dataItem);
            if (ui != null)
                return ui.GetUIControl() as FrameworkElement;
            else
                return null;
        }

        //internal object GetUIControl(object dataItem)
        //{
        //    var dataRow = this.DataControl.GetRowForItem(dataItem);

        //    if (dataRow != null)
        //    {
        //        var cell = dataRow.GetCell(this);

        //        if (cell != null)
        //        {
        //            return (cell.Tag as I_UIControlManager).MainControl;
        //        }
        //    }
        //    return null;
        //}

        public I_UIControlManager GetUIControlManager(object dataItem)
        {
            if (listControls.Any(x => x.Item1 == dataItem))
                return listControls.First(x => x.Item1 == dataItem).Item2;
            else
                return null;
        }
        public List<Tuple<object, I_UIControlManager>> listControls = new List<Tuple<object, I_UIControlManager>>();

        internal void AddDataItem(object dataItem)
        {
            I_UIControlManager MyControlHelper = null;
            if (HasRangeOfValues)
            {
                MyControlHelper = ControlHelper.KeyValueControlHelper(Column);
                MyControlHelper.SetColumnValueRange(ColumnValueRange, false);
            }
            else
                MyControlHelper = ControlHelper.GetControlHelper(Column, ColumnSetting, null);
            MyControlHelper.SetReadonly(IsReadOnly);
            //if (MyControlHelper is I_ControlHelperValueRange)
            //{
            //    if (ColumnValueRange != null)
            //    {

            //    }
            //}
            // cell.Tag = MyControlHelper;
            if (ButtonMenus.Any())
            {
                foreach (var item in ButtonMenus)
                {
                    var newMenu = new ConrolPackageMenu();
                    newMenu.Name = item.Name;
                    newMenu.Title = item.Title;
                    newMenu.MenuClicked += (sender, e) => Item_MenuClicked(sender, e, item, dataItem);
                    MyControlHelper.AddButtonMenu(newMenu);
                }
            }
            //  return MyControlHelper.GetUIControl() as FrameworkElement;
            listControls.Add(new Tuple<object, I_UIControlManager>(dataItem, MyControlHelper));
        }

        private void Item_MenuClicked(object sender, ConrolPackageMenuArg e, ConrolPackageMenu mainMenu, object dataItem)
        {
            e.data = dataItem;
            mainMenu.OnMenuClicked(sender, e);

        }

        internal void RemoveDataItem(object dataItem)
        {
            if (listControls.Any(x => x.Item1 == dataItem))
                listControls.Remove(listControls.First(x => x.Item1 == dataItem));
        }

        internal void RemoveDataItems()
        {
            listControls.Clear();
        }

        //internal void SetReadonly(bool isreadonly)
        //{
        //    this.IsReadOnly = isreadonly;
        //}

        //internal void SetReadonly(object dataItem, bool isreadonly)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);

        //        if (dataRow != null)
        //        {
        //            var cell = dataRow.GetCell(this);

        //            if (cell != null)
        //            {
        //                (cell.Tag as I_ControlHelper).SetReadonly(isreadonly);
        //            }
        //        }
        //    }));
        //}
        //internal void EnableDisable(bool enable)
        //{
        //    this.IsEnabled = enable;
        //}
        //internal void EnableDisable(object dataItem, bool enable)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        if (dataRow != null)
        //        {
        //            var cell = dataRow.GetCell(this);

        //            if (cell != null)
        //            {
        //                (cell.Tag as I_ControlHelper).EnableDisable(enable);

        //            }
        //        }
        //    }));
        //}
        //internal void Visiblity(bool visible)
        //{
        //    this.IsVisible = visible;
        //}
        //internal void Visiblity(object dataItem, bool visible)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        if (dataRow != null)
        //        {
        //            var cell = dataRow.GetCell(this);

        //            if (cell != null)
        //            {
        //                (cell.Tag as I_ControlHelper).Visiblity(visible);

        //            }
        //        }
        //    }));
        //}
        private void MenuItem_Click(object sender, RadRoutedEventArgs e, ConrolPackageMenu cpMenu, Tuple<object, string> cellItem)
        {
            if (cpMenu != null)
            {
                ConrolPackageMenuArg arg = new ConrolPackageMenuArg();
                arg.data = cellItem;
                cpMenu.OnMenuClicked(sender, arg);
            }
        }


        void control_ValueChanged(object sender, ColumnValueChangeArg e, object dataItem)
        {
            e.DataItem = dataItem;
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }



        //void control_TemporaryDisplayViewRequested(object sender, Arg_TemporaryDisplayViewRequested e)
        //{
        //    TemporaryArg.OnTemporaryViewRequested(sender, e);
        //}


        //public override FrameworkElement CreateCellEditElement(GridViewCell cell, object dataItem)
        //{

        //    I_UIControlManager MyControlHelper = null;
        //    if (HasRangeOfValues)
        //        MyControlHelper = ControlHelper.KeyValueControlHelper(Column);
        //    else
        //        MyControlHelper = ControlHelper.GetControlHelper(Column, ColumnSetting, null);

        //    if (MyControlHelper is I_ControlHelperValueRange)
        //    {
        //        if (ColumnValueRange != null)
        //        {
        //            (MyControlHelper as I_ControlHelperValueRange).SetColumnValueRange(ColumnValueRange);
        //        }
        //    }

        //    return MyControlHelper.GetUIControl() as FrameworkElement;
        //}

        //internal void RemoveButtonMenu(string name, object dataItem)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null)
        //        {
        //            if (cell.Content != null)
        //            {
        //                (cell.Tag as I_ControlHelper).RemoveButtonMenu(name);
        //            }
        //        }
        //    }));
        //}

        //internal void RemoveButtonMenu(string name)
        //{
        //    ButtonMenus.Remove(ButtonMenus.FirstOrDefault(x => x.Name == "name"));
        //}
        //internal void AddButtonMenu(ConrolPackageMenu menu)
        //{
        //    ButtonMenus.Add(menu);
        //}
        //internal void AddButtonMenu(ConrolPackageMenu menu, object dataItem)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null)
        //        {
        //            if (cell.Content != null)
        //            {
        //                (cell.Tag as I_UIControlManager).AddButtonMenu(menu);
        //            }
        //        }
        //    }));
        //}

        //internal bool SetValue(object dataItem, object value)
        //{

        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null && cell.Content != null)
        //            (cell.Tag as I_UIControlManager).SetValue(value);
        //    }));


        //    return true;

        //}



        //internal void SetBinding(object dataItem, EntityInstanceProperty property)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null)
        //        {
        //            if (cell.Content != null)
        //            {
        //                (cell.Tag as I_UIControlManager).SetBinding(property);
        //                if (property.Name == "Name")
        //                    (cell.Tag as I_UIControlManager).SetBackgroundColor(InfoColor.Blue);

        //            }
        //        }
        //    }));
        //}

        //internal void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details, object dataItem)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //   {
        //       var dataRow = this.DataControl.GetRowForItem(dataItem);
        //       var cell = dataRow.GetCell(this);
        //       if (cell != null)
        //       {
        //           if ((cell.Tag as I_UIControlManager) is I_ControlHelperValueRange)
        //           {
        //               ((cell.Tag as I_UIControlManager) as I_ControlHelperValueRange).SetColumnValueRange(details);
        //           }
        //       }
        //   }));
        //}
        List<ColumnValueRangeDetailsDTO> ColumnValueRange { set; get; }
        public void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details)
        {
            ColumnValueRange = details;
        }


        List<ConrolPackageMenu> ButtonMenus = new List<ConrolPackageMenu>();

        //internal void SetTooltip(string tooltip)
        //{
        //    ToolTipService.SetToolTip(this, tooltip);
        //}
        //internal void ClearTooltip()
        //{
        //    ToolTipService.SetToolTip(this, null);
        //}


        //internal void SetTooltip(object dataItem, string tooltip)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null)
        //        {
        //            if (cell.Content != null)
        //            {
        //                if (!string.IsNullOrEmpty(tooltip))
        //                    ToolTipService.SetToolTip(cell.Content as FrameworkElement, tooltip);
        //                else
        //                    ToolTipService.SetToolTip(cell.Content as FrameworkElement, null);
        //            }
        //        }
        //    }));
        //}
        //internal void ClearTooltip(object dataItem)
        //{
        //    var dataRow = this.DataControl.GetRowForItem(dataItem);
        //    var cell = dataRow.GetCell(this);
        //    if (cell != null)
        //    {
        //        if (cell.Content != null)
        //            ToolTipService.SetToolTip(cell.Content as FrameworkElement, null);
        //    }
        //}

        //internal void SetColor(InfoColor color)
        //{
        //    this.Background = UIManager.GetColorFromInfoColor(color);
        //}
        //internal void ClearColor()
        //{
        //    this.Background = null;
        //}
        //internal void SetBorderColor(object dataItem, InfoColor color)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null)
        //        {
        //            if (cell.Content != null)
        //            {
        //                cell.BorderBrush = UIManager.GetColorFromInfoColor(color);
        //                cell.BorderThickness = new Thickness(1);
        //            }
        //        }
        //    }));
        //}
        //internal void SetBackgroundColor(object dataItem, InfoColor color)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null)
        //        {
        //            if (cell.Content != null)
        //            {
        //                cell.Background = UIManager.GetColorFromInfoColor(color);
        //            }
        //        }
        //    }));
        //}
        //internal void SetForegroundColor(object dataItem, InfoColor color)
        //{
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        //    {
        //        var dataRow = this.DataControl.GetRowForItem(dataItem);
        //        var cell = dataRow.GetCell(this);
        //        if (cell != null)
        //        {
        //            if (cell.Content != null)
        //            {
        //                cell.Foreground = UIManager.GetColorFromInfoColor(color);
        //            }
        //        }
        //    }));
        //}
        //internal void ClearColor(object dataItem)
        //{
        //    var dataRow = this.DataControl.GetRowForItem(dataItem);
        //    var cell = dataRow.GetCell(this);
        //    if (cell != null)
        //    {
        //        if (cell.Content != null)
        //        {
        //            (cell.Content as Grid).Background = null;// new SolidColorBrush(UIManager.GetColorFromInfoColor(InfoColor.Black));
        //                                                     // (cell.Content as Control).BorderThickness = new Thickness(1);
        //        }
        //    }
        //}
        //internal object GetValue(object dataItem)
        //{
        //    //var dataRow = GetDataRow(dataGrid, dataItem);
        //    //var dataRow = this.DataControl.GetRowForItem(dataItem);

        //    //if (dataRow != null)
        //    //{
        //    //    var cell = dataRow.GetCell(this);

        //    //    if (cell != null)
        //    //    {
        //    //        //////return (cell.Tag as UIControlPackage).BaseControlHelper.GetValue( cell.Tag as UIControlPackage);

        //    //    }
        //    //}
        //    //return "";


        //    var dataRow = this.DataControl.GetRowForItem(dataItem);
        //    var cell = dataRow.GetCell(this);
        //    if (cell.Content != null)
        //        return (cell.Tag as I_UIControlManager).GetValue();
        //    return "";

        //}

        //internal void DisableEnableCell(object dataItem, bool enable)
        //{
        //    var dataRow = this.DataControl.GetRowForItem(dataItem);

        //    if (dataRow != null)
        //    {
        //        var cell = dataRow.GetCell(this);

        //        if (cell != null)
        //        {
        //            cell.IsEnabled = enable;

        //        }
        //    }

        //}
        //internal void SetReadonlyCell(object dataItem, bool isreadonly)
        //{
        //    var dataRow = this.DataControl.GetRowForItem(dataItem);

        //    if (dataRow != null)
        //    {
        //        var cell = dataRow.GetCell(this);

        //        if (cell != null)
        //        {//درست شود
        //            cell.IsEnabled = !isreadonly;

        //        }
        //    }

        //}



        //public IAG_View_TemporaryView GenerateTemporaryView()
        //{
        //    throw new NotImplementedException();
        //}


        //public TemporaryLinkType LinkType
        //{
        //    set;
        //    get;
        //}

        //public void OnTemporaryViewRequested(object sender, Arg_TemporaryDisplayViewRequested arg)
        //{
        //    throw new NotImplementedException();
        //}
    }

    //public class tempMutipleUIManager : I_UIControlManager
    //{
    //    public void AddButtonMenu(ConrolPackageMenu menu)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ClearMenuColor()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ClearValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public List<ConrolPackageMenu> GetButtonMenus()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public CommonOperator GetOperator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object GetUIControl()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void RemoveButtonMenu(string name)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public void RemoveButtonMenus()
    //    {
    //    }
    //        public void SetBackgroundColor(InfoColor color)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetBinding(EntityInstanceProperty property)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetBorderColor(InfoColor color)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details, bool multiselect)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetForegroundColor(InfoColor color)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetMenuColor(InfoColor green)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetOperator(CommonOperator operatorValue)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetReadonly(bool isreadonly)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetTooltip(string tooltip)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool SetValue(object value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Visiblity(bool visible)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
