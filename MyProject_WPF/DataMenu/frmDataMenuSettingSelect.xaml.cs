
using ModelEntites;
using MyFormulaFunctionStateFunctionLibrary;
using MyModelManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using Telerik.Windows.Controls;


namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmFormula.xaml
    /// </summary>
    public partial class frmDataMenuSettingSelect : UserControl
    {
        public event EventHandler<int> ItemSelected;
        public int EntityID { set; get; }
        BizDataMenuSetting bizDataMenuSetting = new BizDataMenuSetting();
        SelectorGrid SelectorGrid = null;

        public frmDataMenuSettingSelect(int entityID)
        {
            InitializeComponent();

            EntityID = entityID;

            var listColumns = new Dictionary<string, string>();
            listColumns.Add("ID", "شناسه");
            listColumns.Add("Name", "عنوان");

            SelectorGrid = ControlHelper.SetSelectorGrid(dtgItems, listColumns);
            SelectorGrid.DataItemSelected += SelectorGrid_DataItemSelected;
            this.Loaded += FrmEntityListViewSelect_Loaded;
        }

        private void FrmEntityListViewSelect_Loaded(object sender, RoutedEventArgs e)
        {
            GetDataMenuSettings();
        }

        private void SelectorGrid_DataItemSelected(object sender, object e)
        {
            CheckSelectedItem(e);
        }

        private void CheckSelectedItem(object item)
        {
            if (item != null)
            {
                var selected = dtgItems.SelectedItem as DataMenuSettingDTO;
                if (selected != null)
                {
                    if (ItemSelected != null)
                        ItemSelected(this, selected.ID);
                }
            }
        }

        private void GetDataMenuSettings()
        {
            var listEntityListViews = bizDataMenuSetting.GetDataMenuSettings(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
            dtgItems.ItemsSource = listEntityListViews;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            CheckSelectedItem(dtgItems.SelectedItem);
        }
    }



}
