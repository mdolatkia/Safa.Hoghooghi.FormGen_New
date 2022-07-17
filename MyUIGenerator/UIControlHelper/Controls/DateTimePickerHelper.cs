
using ModelEntites;
using MyUIGenerator.UIControlHelper.Controls;
using MyUILibrary;
using MyUILibrary.EntityArea;
using MyUILibrary.Temp;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WpfPersianDatePicker.Views;

namespace MyUIGenerator.UIControlHelper
{
    public class DateTimePickerHelper : BaseControlHelper, I_UIControlManager
    {
        Control textBox;
        ComboBox cmbOperators;

        public override FrameworkElement MainControl { get { return textBox; } }

        bool stringDateIsMiladi;
        //bool stringTimeIsMiladi;
        bool valueIsString = false;
        //    bool stringTimeISAMPMFormat;
        bool hasnotTimePicker;
        // bool hideTimePicker;
        bool hideMiladiDatePicker;
        bool hideShamsiDatePicker;
        //  bool hasnotDatePicker;
        StringTimeFormat stringTimeFormat;
        //   bool showMiladiTime;
        //   bool showMiladiDate;
        public DateTimePickerHelper(ColumnDTO correspondingTypeProperty, ColumnUISettingDTO columnSetting, List<SimpleSearchOperator> operators = null)
        {
            theGrid = new Grid();
            theGrid.ColumnDefinitions.Add(new ColumnDefinition());
            theGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            if (correspondingTypeProperty.ColumnType == Enum_ColumnType.DateTime)
            {
                //  showMiladiTime = correspondingTypeProperty.DateTimeColumnType.ShowMiladiDateInUI;

                if (correspondingTypeProperty.DateTimeColumnType.ShowMiladiDateInUI == true)
                    hideShamsiDatePicker = true;
                else
                    hideMiladiDatePicker = true;

                valueIsString = correspondingTypeProperty.DateTimeColumnType.DBValueIsString;
                //   stringTimeFormat = correspondingTypeProperty.DateTimeColumnType.StringTimeISAMPMFormat;
                stringDateIsMiladi = correspondingTypeProperty.DateTimeColumnType.DBValueIsStringMiladi == true;
                stringTimeFormat = correspondingTypeProperty.DateTimeColumnType.DBValueStringTimeFormat;
                //   hideTimePicker = false;
            }
            else if (correspondingTypeProperty.ColumnType == Enum_ColumnType.Date)
            {
                //??نباید DateColumnType نال باشد. فعلا ایف گذاشته شد
                if (correspondingTypeProperty.DateColumnType != null)
                {
                    //**cbeb78f5-f3ac-41d8-b615-c2f50657509c
                    if (correspondingTypeProperty.DateColumnType.ShowMiladiDateInUI == true)
                        hideShamsiDatePicker = true;
                    else
                        hideMiladiDatePicker = true;

                    valueIsString = correspondingTypeProperty.DateColumnType.DBValueIsString;
                    stringDateIsMiladi = correspondingTypeProperty.DateColumnType.DBValueIsStringMiladi == true;

                }
                hasnotTimePicker = true;

            }
            else if (correspondingTypeProperty.ColumnType == Enum_ColumnType.Time)
            {
                //   showMiladiTime = correspondingTypeProperty.TimeColumnType.ShowMiladiTime;
                stringTimeFormat = correspondingTypeProperty.TimeColumnType.DBValueStringTimeFormat;
                valueIsString = correspondingTypeProperty.TimeColumnType.DBValueIsString;

                //      stringTimeIsMiladi = correspondingTypeProperty.TimeColumnType.StringTimeIsMiladi;
                //   stringTimeISAMPMFormat = correspondingTypeProperty.TimeColumnType.StringTimeISAMPMFormat;
                hideShamsiDatePicker = true;
                hideMiladiDatePicker = true;
            }



            textBox = new MyDateTimePicker();


            (textBox as MyDateTimePicker).TimePickerVisiblity = !hasnotTimePicker;
            (textBox as MyDateTimePicker).ShamsiDatePickerVisiblity = !hideShamsiDatePicker;
            (textBox as MyDateTimePicker).MiladiDatePickerVisiblity = !hideMiladiDatePicker;

            if ((textBox as MyDateTimePicker).TimePickerVisiblity)
            {
                if (stringTimeFormat == StringTimeFormat.AMPMMiladi ||
                   stringTimeFormat == StringTimeFormat.AMPMShamsi)
                {
                    CultureInfo cultureInfo = null;
                    if (stringTimeFormat == StringTimeFormat.AMPMMiladi)
                        cultureInfo = new CultureInfo("en-US");
                    else if (stringTimeFormat == StringTimeFormat.AMPMShamsi)
                        cultureInfo = new CultureInfo("fa-IR");
                    (textBox as MyDateTimePicker).TimePickeCulture = cultureInfo;
                }
                else
                {
                    //(textBox as RadTimePicker).DisplayFormat = DateTimePickerFormat.Long;
                    CultureInfo cultureInfo = new CultureInfo("en-US");
                    cultureInfo.DateTimeFormat.ShortTimePattern = "H:mm";
                    cultureInfo.DateTimeFormat.LongTimePattern = "H:mm";
                    //cultureInfo.DateTimeFormat.PMDesignator = " za";
                    //cultureInfo.DateTimeFormat.AMDesignator = " az";
                    (textBox as MyDateTimePicker).TimePickeCulture = cultureInfo;
                }
            }
            textBox.Name = "txtControl";
            //
            textBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            theGrid.Children.Add(textBox);

            if (operators != null && operators.Count > 0)
            {
                theGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70) });
                cmbOperators = new ComboBox();
                cmbOperators.Name = "cmbOperators";
                cmbOperators.ItemsSource = operators;
                cmbOperators.DisplayMemberPath = "Title";
                cmbOperators.SelectedValuePath = "Operator";
                Grid.SetColumn(cmbOperators, 1);
                theGrid.Children.Add(cmbOperators);
            }
            DefaultBorderBrush = textBox.BorderBrush;
            DefaultBorderThickness = textBox.BorderThickness;
            DefaultBackground = textBox.Background;
            DefaultForeground = textBox.Foreground;

            //}
            //}
            //return package;
        }

        //private void DatePickerHelper_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    var binding = textBox.GetBindingExpression(PDatePicker.SelectedDateProperty);
        //    var bindinga = textBox.GetBindingExpression(PDatePicker.SelectedPersianDateProperty);


        //}

        //private void DatePickerHelper_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var binding = textBox.GetBindingExpression(PDatePicker.SelectedDateProperty);
        //    var bindinga = textBox.GetBindingExpression(PDatePicker.SelectedPersianDateProperty);
        //}

        //void textBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var binding = textBox.GetBindingExpression(PDatePicker.SelectedDateProperty);
        //    var bindinga = textBox.GetBindingExpression(PDatePicker.SelectedPersianDateProperty);
        //}
        public bool SetValue(object value)
        {
            DateConverterParameter param = GetConverterParameter();
            DateTime? dateTime = ReadValueFromProperty(value, param);
            (textBox as MyDateTimePicker).SelectedDateTime = dateTime;
            return true;
        }

        public static DateTime? ReadValueFromProperty(object value, DateConverterParameter param)
        {
            DateTime? dateTime = null;
            if (value != null && value.ToString() != "")
            {
                if (param.valueIsString)
                {
                    if (!param.hasnotDatePicker)
                    {
                        string stringdate = null;
                        if (param.hasnotTimePicker)
                            stringdate = value.ToString();
                        else
                            stringdate = value.ToString().Split(' ')[0];
                        if (param.stringDateIsMiladi == false)
                            dateTime = AgentHelper.GetMiladiDateFromShamsi(stringdate);
                        else
                            dateTime = Convert.ToDateTime(stringdate);
                    }
                    if (!param.hasnotTimePicker)
                    {
                        string stringtime = null;
                        if (param.hasnotDatePicker)
                            stringtime = value.ToString();
                        else
                        {
                            stringtime = value.ToString().Split(" ".ToCharArray(), 2)[1];
                        }
                        if (param.stringTimeFormat == StringTimeFormat.AMPMMiladi ||
                              param.stringTimeFormat == StringTimeFormat.AMPMShamsi)
                            stringtime = stringtime.Replace("ق.ظ", "AM").Replace("ب.ظ", "PM");

                        DateTime time;
                        if (DateTime.TryParse(stringtime, out time))
                        {
                            if (dateTime == null)
                                dateTime = time;
                            else
                                dateTime = dateTime.Value.Add(time.TimeOfDay);
                        }
                    }
                }
                else
                {
                    //اگر نوع خصوصیت تایم باشه در دیتابیس مقدار تایم اسپن میاد
                    if (value is TimeSpan)
                        dateTime = DateTime.Today.Add((TimeSpan)value);
                    else
                        dateTime = Convert.ToDateTime(value);
                }
            }
            return dateTime;
        }

        public object GetValue()
        {
            return ReadValueFromControl((textBox as MyDateTimePicker).SelectedDateTime, GetConverterParameter());

        }

        public static object ReadValueFromControl(DateTime? selectedDateTime, DateConverterParameter param)
        {
            //اینجا درسته که اگر نوع خصوصیت تایم اسپن باشه ولی چون کوئری بصورت رشته اعمال میشه اگر تاریخ و زمان هم باشه در دیتابیس فقط زمان ثبت میشود
            //البته میشود بجای رشته انواع دیت تایم یا تایم اسپن هم برگرداند. بعدا بررسی شود آیا بهتر است؟ مثلا اگر قرار است فرمولی حساب بشود با انواعی که از پایگاه داده لود میشود یکسان باشند
            if (selectedDateTime != null && param.valueIsString)
            {
                var date = "";
                var time = "";
                if (!param.hasnotDatePicker)
                {
                    if (param.stringDateIsMiladi == false)
                        date = AgentHelper.GetShamsiDateFromMiladi(selectedDateTime.Value);
                    else
                        date = selectedDateTime.Value.ToShortDateString();
                }
                if (!param.hasnotTimePicker)
                {
                    if (param.stringTimeFormat == StringTimeFormat.AMPMMiladi ||
                            param.stringTimeFormat == StringTimeFormat.AMPMShamsi)
                    {
                        if (param.stringTimeFormat == StringTimeFormat.AMPMMiladi)
                        {
                            time = selectedDateTime.Value.ToString("hh:mm tt").Replace("ق.ظ", "AM").Replace("ب.ظ", "PM");
                        }
                        else if (param.stringTimeFormat == StringTimeFormat.AMPMShamsi)
                        {
                            time = selectedDateTime.Value.ToString("hh:mm tt").ToUpper().Replace("AM", "ق.ظ").Replace("PM", "ب.ظ"); ;
                        }
                    }
                    else
                        time = selectedDateTime.Value.ToString("hh:mm:ss");
                }
                return date + (!string.IsNullOrEmpty(time) ? " " : "") + time;
            }
            else
            {
                return selectedDateTime;
            }

        }

        public CommonOperator GetOperator()
        {
            if (cmbOperators != null)
            {
                return (cmbOperators.SelectedItem as SimpleSearchOperator).Operator;
            }
            else
                return CommonOperator.Equals;
        }
        public void SetOperator(CommonOperator searchOperator)
        {
            cmbOperators.SelectedValue = searchOperator;
            //  return false;
        }
        public bool HasOperator()
        {
            return cmbOperators != null;
        }
        public void EnableDisable(bool enable)
        {
            textBox.IsEnabled = enable;
        }

        public bool IsVisible()
        {
            return textBox.Visibility == Visibility.Visible;
        }
        public void SetReadonly(bool isreadonly)
        {
            textBox.IsEnabled = !isreadonly;
        }

        //public void SetTooltip( string tooltip)
        //{
        //    if (!string.IsNullOrEmpty(tooltip))
        //        ToolTipService.SetToolTip(textBox, tooltip);
        //    else
        //        ToolTipService.SetToolTip(textBox, null);
        //}

        //public void ClearTooltip()
        //{
        //    ToolTipService.SetToolTip(textBox, null);
        //}
        public void SetBorderColor(InfoColor color)
        {
            if (color != InfoColor.Default)
            {
                textBox.BorderBrush = UIManager.GetColorFromInfoColor(color);
                textBox.BorderThickness = new Thickness(1);
            }
            else
            {
                textBox.BorderBrush = DefaultBorderBrush;
                textBox.BorderThickness = DefaultBorderThickness;
            }
        }
        public void SetBackgroundColor(InfoColor color)
        {
            if (color != InfoColor.Default)
            {
                textBox.Background = UIManager.GetColorFromInfoColor(color);
            }
            else
            {
                textBox.Background = DefaultBackground;
            }
        }
        public void SetForegroundColor(InfoColor color)
        {
            if (color != InfoColor.Default)
            {
                textBox.Foreground = UIManager.GetColorFromInfoColor(color);
            }
            else
            {
                textBox.Foreground = DefaultForeground;
            }
        }

        public void SetBinding(EntityInstanceProperty property)
        {
            Binding binding = new Binding("Value");
            binding.Source = property;
            binding.Mode = BindingMode.TwoWay;
            binding.Converter = new ConverterDate();
            DateConverterParameter param = GetConverterParameter();
            binding.ConverterParameter = param;
            (textBox as MyDateTimePicker).SetBinding(MyDateTimePicker.SelectedDateTimeProperty, binding);
            //if(property.Value!=null)
            //(textBox as MyDateTimePicker).SelectedDateTime = Convert.ToDateTime(property.Value);
            //پارامتر بشن خصوصیات بالا و در کانورتر از همون گت و ست استفاده یشه


            if (valueIsString)
            {
                //if (!hasnotDatePicker && !hasnotTimePicker)
                //{

                //}
                //else if (!hasnotDatePicker)
                //{
                //    if (stringDateIsMiladi == false)
                //        binding.Converter = new ConverterDateOnlyShamsi();
                //    binding.con
                //}
                //else if (!hasnotTimePicker)
                //{
                //if (stringTimeISAMPMFormat == true)
                //{
                //    if (stringTimeIsMiladi == false)
                //        binding.Converter = new ConverterTimeOnlyAMPMShamsi();
                //    else
                //        binding.Converter = new ConverterTimeOnlyAMPM();
                //}
                //else
                //    binding.Converter = new ConverterTimeOnlyLong();

                //}
            }

        }

        private DateConverterParameter GetConverterParameter()
        {
            DateConverterParameter param = new UIControlHelper.DateConverterParameter();
            //param.hasnotDatePicker = hasnotDatePicker;
            param.hasnotTimePicker = hasnotTimePicker;
            param.hideMiladiDatePicker = hideMiladiDatePicker;
            param.hideShamsiDatePicker = hideShamsiDatePicker;
            // param.hideTimePicker = hasnotTimePicker;
            param.stringTimeFormat = stringTimeFormat;
            //param.showMiladiDate = showMiladiDate;
            //    param.showMiladiTime = showMiladiTime;
            param.stringDateIsMiladi = stringDateIsMiladi;
            //     param.stringTimeISAMPMFormat = stringTimeISAMPMFormat;
            //param.stringTimeIsMiladi = stringTimeIsMiladi;
            param.valueIsString = valueIsString;
            return param;
        }


        //public void SetBorderColor(InfoColor color)
        //{
        //    textBox.BorderBrush = UIManager.GetColorFromInfoColor(color);
        //    textBox.BorderThickness = new Thickness(1);
        //}
        //public void SetBackgroundColor(InfoColor color)
        //{
        //    textBox.Background = UIManager.GetColorFromInfoColor(color);
        //}
        //public void SetForegroundColor(InfoColor color)
        //{
        //    textBox.Foreground = UIManager.GetColorFromInfoColor(color);
        //}

        public void SetColumnValueRange(List<ColumnValueRangeDetailsDTO> details)
        {
            throw new NotImplementedException();
        }

        //public void AddButtonMenu(ConrolPackageMenu menu)
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
    public class ConverterDate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var cParam = parameter as DateConverterParameter;
            DateTime? dateTime = DateTimePickerHelper.ReadValueFromProperty(value, cParam);
            return dateTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var cParam = parameter as DateConverterParameter;
            return DateTimePickerHelper.ReadValueFromControl((DateTime?)value, cParam);
        }
    }

    public class ConverterDateOnlyShamsi : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.ToString() != "")
            {
                return AgentHelper.GetMiladiDateFromShamsi(value.ToString());
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return AgentHelper.GetShamsiDateFromMiladi((DateTime)value);
            }
            else
                return null;
        }
    }
    public class ConverterTimeOnlyLong : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var time = (DateTime)value;
                return time.ToString("hh:mm:ss");
            }
            else
                return null;
        }
    }
    public class ConverterTimeOnlyAMPM : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var time = (DateTime)value;
                return time.ToString("hh:mm tt");
            }
            else
                return null;
        }
    }
    public class ConverterTimeOnlyAMPMShamsi : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return value.ToString().Replace("ق.ظ", "AM").Replace("ب.ظ", "PM");
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var time = (DateTime)value;
                return time.ToString("hh:mm tt").ToUpper().Replace("AM", "ق.ظ").Replace("PM", "ب.ظ");
            }
            else
                return null;
        }
    }

    public class DateConverterParameter
    {
        internal bool hasnotDatePicker
        {
            get
            {
                return (hideMiladiDatePicker && hideShamsiDatePicker);
            }
        }
        public bool? stringDateIsMiladi;
        //      public bool? stringTimeIsMiladi;
        public bool valueIsString = false;
        //   public bool? stringTimeISAMPMFormat;
        public bool hasnotTimePicker;
        //  public bool hideTimePicker;
        public bool hideMiladiDatePicker;
        public bool hideShamsiDatePicker;
        //   public bool hasnotDatePicker;
        public StringTimeFormat stringTimeFormat;
        //      public bool showMiladiTime;
        //    public bool showMiladiDate;
    }
}
