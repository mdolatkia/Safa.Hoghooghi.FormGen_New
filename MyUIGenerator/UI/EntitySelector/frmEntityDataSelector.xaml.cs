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

        public void AddEntitySelectorArea(object view)
        {
            grdEntitySelector.Children.Clear();
            grdEntitySelector.Children.Add(view as UIElement);
        }
        public void SetEntitySelectorTitle(string title)
        {
            lblEntitySelector.Text = title;
        }
        public void AddDataSelector(object view)
        {
            grdDataSelector.Children.Clear();
            grdDataSelector.Children.Add(view as UIElement);
        }
        public void SetDataSelectorTitle(string title)
        {
            lblDataSelector.Text = title;
        }

        public void RemoveDataSelector()
        {
            grdDataSelector.Children.Clear();
        }

        public void RemoveEntitySelector()
        {
            lblEntitySelector.Visibility = Visibility.Collapsed;
            grdEntitySelector.Children.Clear();
        }

        public void SetSearchRepositoyTitle(string title)
        {
            lblSearchRepository.Text = title;
        }

        public void RemoveSearchRepositoy()
        {
            lblSearchRepository.Visibility = Visibility.Collapsed;
            grdSearchRepository.Children.Clear();
        }

        public void AddSearchRepository(I_View_SearchEntityArea searchView)
        {
            grdSearchRepository.Children.Clear();
            grdSearchRepository.Children.Add(searchView as UIElement);
        }
    }
}
