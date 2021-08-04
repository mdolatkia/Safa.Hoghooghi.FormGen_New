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

namespace MyUIGenerator
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        DataItem aa;
        public Test()
        {
            InitializeComponent();


            //aa = new DataItem();
            //aa.Date = DateTime.Today.AddDays(3);
            txtDate1.SelectedPersianDate = "1400/05/05";
            //SetBinding();
            txtDate.SelectedDateTime = DateTime.Now;

        }


        private void SetBinding()
        {
            Binding binding = new Binding("Date");
            binding.Source = aa;
           // binding.Path = new PropertyPath("Date");
            binding.Mode = BindingMode.TwoWay;
            //    binding.Converter = new ConverterDate();
            // DateConverterParameter param = GetConverterParameter();
            //    binding.ConverterParameter = param;

            BindingOperations.ClearBinding(txtDate, UIControlHelper.Controls.MyDateTimePicker.SelectedDateTimeProperty);

            txtDate.SetBinding(UIControlHelper.Controls.MyDateTimePicker.SelectedDateTimeProperty, binding);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            aa.Date = DateTime.Today.AddDays(-3);
             SetBinding();
        }
    }
    public class DataItem
    {
       
        public DateTime Date { set; get; }
    }
}
