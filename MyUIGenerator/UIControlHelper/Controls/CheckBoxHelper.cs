
using ModelEntites;
using MyUILibrary;
using MyUILibrary.EntityArea;
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
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace MyUIGenerator.UIControlHelper
{
    public class CheckBoxHelper : BaseControlHelper, I_UIControlManager
    {


        CheckBox checkbox;
        public override FrameworkElement MainControl { get { return checkbox; } }

        public Brush DefaultBackground { get; private set; }

        public CheckBoxHelper(ColumnDTO correspondingTypeProperty, ColumnUISettingDTO columnSetting, List<SimpleSearchOperator> operators = null)
        {
            //** 01264950-cc64-4088-90ec-c5150319d9e8
            //UIControlPackage package = new UIControlPackage();
            //  package.UIControls = new List<FrameworkElement>();
            //UIControlSetting controlUISetting = new UIControlSetting();
            //controlUISetting.DesieredColumns = 1;
            //controlUISetting.DesieredRows = 1;
            theGrid = new Grid();
            theGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            theGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkbox = new CheckBox();
            checkbox.Name = "txtControl";
            checkbox.Checked += (sender, e) => control_Checked(sender, e);
            if (correspondingTypeProperty.IsNull)
                checkbox.IsThreeState = true;
            theGrid.Children.Add(checkbox);


            //textBox.Mask = "###";

            //  textBox.FormatString = "";
            //  textBox.EmptyContent = "";
            //textBox.TextMode = Telerik.Windows.Controls.MaskedInput.TextMode.PlainText;
            //textBox.sho
            //   textBox.InputBehavior = Telerik.Windows.Controls.MaskedInput.InputBehavior.Replace;

            //            if(correspondingTypeProperty.ColumnType.NumericColumnType.Precision)
            //textBox.ty
            //control.IsEnabled = !columnSetting.IsReadOnly;
            //control.Margin = new System.Windows.Thickness(5);
            checkbox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //     checkbox.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            DefaultBackground = checkbox.Background;
            //return package;

            DefaultBorderBrush = checkbox.BorderBrush;
            DefaultBorderThickness = checkbox.BorderThickness;
            DefaultBackground = checkbox.Background;
            DefaultForeground = checkbox.Foreground;
        }


        void control_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //////ColumnValueChangeArg arg = new ColumnValueChangeArg();
            //////arg.NewValue = GetValue(uiControlPackage);
            //////uiControlPackage.OnValueChanged(sender, arg);
        }
        //internal static UIControlSetting GenerateUISetting(DataMaster.EntityDefinition.ND_Type_Property nD_Type_Property, UISetting.DataPackageUISetting.UI_PackagePropertySetting uI_PackagePropertySetting)
        //{
        //    throw new NotImplementedException();
        //}


        public bool SetValue(object value)
        {
            if (value == null)
                checkbox.IsChecked = null;
            else
            {
                if (value == "" || value == "0")
                    value = "false";
                else if (value == "1")
                    value = "true";
                checkbox.IsChecked = Convert.ToBoolean(value);
            }

            //if (columnSetting != null)
            //    (control as CheckBox).IsEnabled = !columnSetting.IsReadOnly;


            return true;
        }

        public object GetValue()
        {
            if (checkbox.IsChecked == null)
                return null;
            else
            {
                return (checkbox.IsChecked == true ? "1" : "0");
            }
        }

        public void EnableDisable(bool enable)
        {
            checkbox.IsEnabled = enable;
        }
        public bool IsVisible()
        {
            return checkbox.Visibility == Visibility.Visible;
        }

        public void SetReadonly(bool isreadonly)
        {
            checkbox.IsEnabled = !isreadonly;
        }
        public CommonOperator GetOperator()
        {
            return CommonOperator.Equals;
        }
        public void SetOperator(CommonOperator searchOperator)
        {
            //  return false;
        }
        public bool HasOperator()
        {
            return false;
        }


        public void SetBorderColor(InfoColor color)
        {
            if (color != InfoColor.Default)
            {
                checkbox.BorderBrush = UIManager.GetColorFromInfoColor(color);
                checkbox.BorderThickness = new Thickness(1);
            }
            else
            {
                checkbox.BorderBrush = DefaultBorderBrush;
                checkbox.BorderThickness = DefaultBorderThickness;
            }
        }
        public void SetBackgroundColor(InfoColor color)
        {
            if (color != InfoColor.Default)
            {
                checkbox.Background = UIManager.GetColorFromInfoColor(color);
            }
            else
            {
                checkbox.Background = DefaultBackground;
            }
        }
        public void SetForegroundColor(InfoColor color)
        {
            if (color != InfoColor.Default)
            {
                checkbox.Foreground = UIManager.GetColorFromInfoColor(color);
            }
            else
            {
                checkbox.Foreground = DefaultForeground;
            }
        }
        public void SetBinding(EntityInstanceProperty property)
        {
            Binding binding = new Binding("Value");
            binding.Source = property;
            checkbox.SetBinding(CheckBox.IsCheckedProperty, binding);
        }

        public void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details, bool multiselect)
        {
            throw new NotImplementedException();
        }

        public void ClearValue()
        {
            checkbox.IsChecked = null;
        }

     




        //public void AddButtonMenu( ConrolPackageMenu menu)
        //{
        //    theGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
        //    var menuButton = new Button();
        //    menuButton.Name = menu.Name;
        //    menuButton.Content = menu.Title;
        //    menuButton.Click += (sender, e) => MenuButton_Click(sender, e, menu);
        //    Grid.SetColumn(menuButton, theGrid.ColumnDefinitions.Count);
        //    theGrid.Children.Add(menuButton);
        //}

        //private void MenuButton_Click(object sender, RoutedEventArgs e, ConrolPackageMenu menu)
        //{
        //    ConrolPackageMenuArg arg = new ConrolPackageMenuArg();
        //    menu.OnMenuClicked(sender, arg);
        //}
    }
}
