using ModelEntites;
using MyCommonWPFControls;
using MyModelManager;
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

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmColumnValueRange1.xaml
    /// </summary>
    public partial class frmColumnCustomFormula : UserControl
    {
        BizColumn bizColumn = new BizColumn();
        BizFormula bizFormula = new BizFormula();
        ColumnCustomFormulaDTO Message;
        int ColumnID { set; get; }
        int EntityID { set; get; }
        public event EventHandler<string> ItemUpdated;
        public frmColumnCustomFormula(int entityID, int columnID)
        {
            InitializeComponent();
            EntityID = entityID;
            ColumnID = columnID;
            lokFormula.EditItemEnabled = true;
            lokFormula.NewItemEnabled = true;
            lokFormula.EditItemClicked += LokFormula_EditItemClicked;
            SetFromulas();

            Message = bizColumn.GetCustomFormula(ColumnID);
            if (Message == null)
                Message = new ColumnCustomFormulaDTO();
            ShowMessage();
        }

        private void LokFormula_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            int formulaID = 0;
            if (lokFormula.SelectedItem != null)
                formulaID = (int)lokFormula.SelectedValue;
            frmFormula view = new frmFormula(formulaID, EntityID);
            view.FormulaUpdated += View_FormulaSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form", Enum_WindowSize.Maximized);
        }

        private void View_FormulaSelected(object sender, FormulaSelectedArg e)
        {
            SetFromulas();
            lokFormula.SelectedValue = e.FormulaID;
        }
        private void SetFromulas()
        {
            lokFormula.DisplayMember = "Name";
            lokFormula.SelectedValueMember = "ID";
            BizFormula bizFormula = new BizFormula();
            lokFormula.ItemsSource = bizFormula.GetFormulas(EntityID, true);
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lokFormula.SelectedItem == null)
            {
                MessageBox.Show("فرمول نامشخص است");
                return;
            }
            Message.FormulaID = (int)lokFormula.SelectedValue;
            Message.CalculateFormulaAsDefault = chkCalculateFormulaAsDefault.IsChecked == true;
            Message.OnlyOnEmptyValue = chkOnlyOnEmptyValue.IsChecked == true;
            Message.OnlyOnNewData = chkOnlyOnNewData.IsChecked == true;
            bizColumn.SaveColumnCustomFormula(ColumnID, Message);
            if (ItemUpdated != null)
                ItemUpdated(this, (lokFormula.SelectedItem as FormulaDTO).Name);
            MessageBox.Show("اطلاعات ثبت شد");
        }
        private void ShowMessage()
        {
            lokFormula.SelectedValue = Message.FormulaID;
            chkCalculateFormulaAsDefault.IsChecked = Message.CalculateFormulaAsDefault;
            chkOnlyOnEmptyValue.IsChecked = Message.OnlyOnEmptyValue;
            chkOnlyOnNewData.IsChecked = Message.OnlyOnNewData;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            bizColumn.DeleteColumnCustomFormula(ColumnID);
        }
    }
}
