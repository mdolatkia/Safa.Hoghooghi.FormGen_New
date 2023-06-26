using MyUILibrary.EntityArea;
using MyUILibraryInterfaces.EntityArea;
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

namespace MyUIGenerator.View
{
    /// <summary>
    /// Interaction logic for frmEntitySelector.xaml
    /// </summary>
    public partial class frmEntityDataSelector : UserControl, I_View_GeneralEntityDataSelectArea
    {
        public frmEntityDataSelector()
        {
            InitializeComponent();
        }

      
        public void AddSelector(object view)
        {
            grdSelector.Children.Clear();
            grdSelector.Children.Add(view as UIElement);
        }
        public void SetSelectorTitle(string title)
        {
            lblSelector.Text = title;
        }

       

      
    }
}
