
using ModelEntites;

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
    public partial class frmDatabaseFunction_EntitySelect : UserControl
    {
        SelectorGrid SelectorGrid = null;
        public event EventHandler<DataItemSelectedArg> DatabaseFunctionEntitySelected;
        //public string Catalog { set; get; }
        public int EntityID { set; get; }
        BizDatabaseFunction bizCode = new BizDatabaseFunction();
        public frmDatabaseFunction_EntitySelect(int entityID)
        {
            InitializeComponent();
            EntityID = entityID;
            this.Loaded += FrmFormula_Loaded;
            var listColumns = new Dictionary<string, string>();
            listColumns.Add("ID", "شناسه");
            listColumns.Add("Title", "عنوان");
            SelectorGrid = ControlHelper.SetSelectorGrid(dtgItems, listColumns);
            SelectorGrid.DataItemSelected += SelectorGrid_DataItemSelected;
        }
        private void SelectorGrid_DataItemSelected(object sender, object e)
        {
            CheckSelectedItem(e);
        }
        private void CheckSelectedItem(object item)
        {
            if (item != null)
            {
                var selected = dtgItems.SelectedItem as DatabaseFunction_EntityDTO;
                if (selected != null)
                {
                    if (DatabaseFunctionEntitySelected != null)
                        DatabaseFunctionEntitySelected(this, new DataItemSelectedArg() { ID = selected.ID });
                }
            }
        }
        private void FrmFormula_Loaded(object sender, RoutedEventArgs e)
        {
            GetCodes();
        }

        private void GetCodes()
        {
            var listCodes = bizCode.GetDatabaseFunctionEntityByEntityID(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
            dtgItems.ItemsSource = listCodes;
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
