
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
    public partial class frmFormulaSelectGeneral : UserControl
    {
        public event EventHandler<GeneralFormulaSelectedArg> FormulaSelected;
        //public int EntityID { set; get; }
        BizFormula bizFormula = new BizFormula();
        SelectorGrid SelectorGrid = null;
        public frmFormulaSelectGeneral()
        {
            InitializeComponent();



            var listColumns = new Dictionary<string, string>();
            listColumns.Add("Name", "نام فرمول");
            listColumns.Add("Title", "عنوان");
            listColumns.Add("ResultType", "نوع نتیجه");
            listColumns.Add("ResultDotNetType", "نوع در برنامه");

            SelectorGrid = ControlHelper.SetSelectorGrid(dtgFormulas, listColumns);
            SelectorGrid.DataItemSelected += SelectorGrid_DataItemSelected;


            this.Loaded += FrmFormula_Loaded;
        }
        private void SelectorGrid_DataItemSelected(object sender, object e)
        {
            CheckSelectedItem(e);
        }
        private void CheckSelectedItem(object item)
        {
            if (item != null)
            {
                var selected = dtgFormulas.SelectedItem as FormulaDTO;
                if (selected != null)
                {
                    if (FormulaSelected != null)
                        FormulaSelected(this, new GeneralFormulaSelectedArg() { Formula = selected });
                }
            }
        }


        private void FrmFormula_Loaded(object sender, RoutedEventArgs e)
        {
            GetFormulas();

        }

        private void GetFormulas()
        {
            var listFomula = bizFormula.GetAllFormulas(MyProjectManager.GetMyProjectManager.GetRequester(), txtSearch.Text);
            dtgFormulas.ItemsSource = listFomula;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            CheckSelectedItem(dtgFormulas.SelectedItem);
        }



        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            GetFormulas();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int formulaID = 0;
            var selected = dtgFormulas.SelectedItem as FormulaDTO;
            if (selected != null)
                formulaID = selected.ID;
            frmFormula view = new frmFormula(formulaID, 0);
            view.FormulaUpdated += View_FormulaSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form", Enum_WindowSize.Maximized);
        }
        private void View_FormulaSelected(object sender, FormulaSelectedArg e)
        {
            GetFormulas();
        }
    }
    public class GeneralFormulaSelectedArg : EventArgs
    {
        public FormulaDTO Formula { set; get; }
    }


}
