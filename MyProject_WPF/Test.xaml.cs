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

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();

            SetBinding();

         //   txtDate.setb
        }

        private void SetBinding()
        {
            var aa = new DataItem();
            aa.Date = DateTime.Today;

            Binding binding = new Binding("Value");
            binding.Source = aa;
            binding.Path = new PropertyPath("Date");
            binding.Mode = BindingMode.TwoWay;
            //    binding.Converter = new ConverterDate();
            // DateConverterParameter param = GetConverterParameter();
            //    binding.ConverterParameter = param;
            txtDate.SetBinding(DatePicker.SelectedDateProperty, binding);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SetBinding();
        }
    }
    public class DataItem
    {
        public object Date { set; get; }
    }
}
