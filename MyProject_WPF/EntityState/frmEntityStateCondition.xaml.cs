using ModelEntites;
using MyCommonWPFControls;


using MyModelManager;
using ProxyLibrary;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityStates.xaml
    /// </summary>
    public partial class frmEntityStateCondition : UserControl
    {
        public event EventHandler<EntityStateConditionDTO> DeleteConditionRequest;
        BizEntityState bizEntityState = new BizEntityState();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        int EntityID { set; get; }
        int EntityStateID { set; get; }
        BizSecuritySubject bizSecuritySubject = new BizSecuritySubject();
        public EntityStateConditionDTO Message = null;

        public frmEntityStateCondition(int entityID,EntityStateConditionDTO entityStateConditionDTO)
        {
            InitializeComponent();
            Message = entityStateConditionDTO;
            EntityID = entityID;
            SetColumns();
            SetRelationshipTails();
            SetFromulas();
            SetSecuritySubjects();

            ControlHelper.GenerateContextMenu(dtgColumnValue);
            ControlHelper.GenerateContextMenu(dtgFormulaValue);
            ControlHelper.GenerateContextMenu(dtgSecuritySubjects);
            lokRelationshipTail.SelectionChanged += LokRelationshipTail_SelectionChanged;
            lokRelationshipTail.EditItemClicked += LokRelationshipTail_EditItemClicked;
            cmbOperator.ItemsSource = Enum.GetValues(typeof(Enum_EntityStateOperator)).Cast<Enum_EntityStateOperator>();
            cmbInOrNotIn.ItemsSource = Enum.GetValues(typeof(InORNotIn)).Cast<InORNotIn>();
            colReservedValue.ItemsSource = Enum.GetValues(typeof(SecurityReservedValue));
            //colHasOrHasNot.ItemsSource = Enum.GetValues(typeof(Enum_SecuritySubjectOperator));

            lokFormula.EditItemEnabled = true;
            lokFormula.NewItemEnabled = true;
            lokFormula.EditItemClicked += LokFormula_EditItemClicked;


            ShowMessage();

        }


        private void LokRelationshipTail_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            frmEntityRelationshipTail frm = null;
            frm = new frmEntityRelationshipTail(EntityID);
            MyProjectManager.GetMyProjectManager.ShowDialog(frm, "رابطه های مرتبط");
            frm.ItemSelected += (sender1, e1) => Frm_TailSelected(sender1, e1, (sender as MyStaticLookup));
        }
        private void Frm_TailSelected(object sender1, EntityRelationshipTailSelectedArg e1, MyStaticLookup myStaticLookup)
        {
            SetRelationshipTails();
            myStaticLookup.SelectedValue = e1.EntityRelationshipTailID;
        }
        private void LokRelationshipTail_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            SetColumns();
        }

        private void SetRelationshipTails()
        {
            var list = bizEntityRelationshipTail.GetEntityRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
            var tails = list.Where(x => x.IsOneToManyTail == false).ToList();
            lokRelationshipTail.DisplayMember = "EntityPath";
            lokRelationshipTail.SelectedValueMember = "ID";
            lokRelationshipTail.ItemsSource = tails;
        }
        private void SetSecuritySubjects()
        {
            colSecuritySubject.DisplayMemberPath = "Name";
            colSecuritySubject.SelectedValueMemberPath = "ID";
            colSecuritySubject.SearchFilterChanged += LokSubject_SearchFilterChanged;
        }
        private void LokSubject_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    var subjects = bizSecuritySubject.GetSecuritySubjects(e.SingleFilterValue);
                    e.ResultItemsSource = subjects;
                }
                else
                {
                    var id = Convert.ToInt32(e.SingleFilterValue);
                    if (id > 0)
                    {
                        var subject = bizSecuritySubject.GetSecuritySubject(id);
                        e.ResultItemsSource = new List<SecuritySubjectDTO> { subject };
                    }
                    else
                        e.ResultItemsSource = null;
                }
            }
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
            lokFormula.ItemsSource = bizFormula.GetFormulas(EntityID, false);
        }
        private void SetColumns()
        {
            BizColumn bizColumn = new BizColumn();
            BizTableDrivedEntity biz = new BizTableDrivedEntity();
            var entityID = 0;
            if (lokRelationshipTail.SelectedItem == null)
                entityID = EntityID;
            else
            {
                EntityRelationshipTailDTO item = lokRelationshipTail.SelectedItem as EntityRelationshipTailDTO;
                entityID = item.TargetEntityID;
            }
            var entity = biz.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            var columns = entity.Columns;  //  .Where(x => x.ForeignKey == false).ToList();
            //  برای وضعیتهایی که به دسترسی داده وصل میشن همه ستونها لازمند چون مثلا برای درخواست سرویس شناسه دفتر با شناسه خاری سازمان کاربر چک میشود. اما برای وضعیتهای فرم کلید خارجی ها کنترل نمی شوند که باعث فعال شدن اقدامات بشوند. چون داینامیک تغییر نمی کنند. البته بعهتر است برنامه تغییر کند که کلید خارجی ها با تغییر رابطه تغییر کنند.

            cmbColumns.DisplayMemberPath = "Alias";
            cmbColumns.SelectedValuePath = "ID";
            cmbColumns.ItemsSource = columns;
            if (Message != null && Message.ID != 0)
            {
                if (Message.ColumnID != 0)
                    cmbColumns.SelectedValue = Message.ColumnID;
            }
        }



        private void ShowMessage()
        {
            txtTitle.Text = Message.Title;

            dtgSecuritySubjects.ItemsSource = Message.SecuritySubjects;
            //if (EntityStateConditionDTO.Preserve)
            //    optPersist.IsChecked = true;
            //else
            //    optNotPersist.IsChecked = true;
            lokRelationshipTail.SelectedValue = Message.RelationshipTailID;
            cmbOperator.SelectedItem = Message.EntityStateOperator;
            if (Message.FormulaID != 0)
            {
                lokFormula.SelectedValue = Message.FormulaID;
                dtgFormulaValue.ItemsSource = Message.Values;
                optFormula.IsChecked = true;
            }
            else if (Message.ColumnID != 0)
            {

                cmbColumns.SelectedValue = Message.ColumnID;
                dtgColumnValue.ItemsSource = Message.Values;
                optColumn.IsChecked = true;
            }
            else
            {
                dtgFormulaValue.ItemsSource = Message.Values;
                dtgColumnValue.ItemsSource = Message.Values;
            }
            cmbInOrNotIn.SelectedItem = Message.SecuritySubjectInORNotIn;
        }
        private void optFormula_Checked(object sender, RoutedEventArgs e)
        {
            tabColumn.Visibility = Visibility.Collapsed;
            tabFormula.Visibility = Visibility.Visible;
            tabFormula.IsSelected = true;
        }


        private void optColumn_Checked(object sender, RoutedEventArgs e)
        {
            tabFormula.Visibility = Visibility.Collapsed;
            tabColumn.Visibility = Visibility.Visible;
            tabColumn.IsSelected = true;
        }

        public string UpdateMessage()
        {
            //if (txtTitle.Text == "")
            //{
            //    MessageBox.Show("عنوان مناسب تعریف نشده است");
            //    return;
            //}
            //if (optPersist.IsChecked == false && optNotPersist.IsChecked == false)
            //{
            //    MessageBox.Show("یکی از حالات ذخیره و یا عدم ذخیره را انتخاب نمایید");
            //    return;
            //}


            //if (optFormula.IsChecked == false && optColumn.IsChecked == false)
            //{
            //    return "یکی از حالات فرمول و یا ستون را انتخاب نمایید";
            //}
            //if (optFormula.IsChecked == true)
            //{
            //    if (lokFormula.SelectedItem == null)
            //    {
            //        return "فرمول مشخص نشده است";
            //    }
            //}
            //else if (optColumn.IsChecked == true)
            //{
            //    if (cmbColumns.SelectedItem == null)
            //    {
            //        return "ستون مشخص نشده است";
            //    }
            //}
            Message.Title = txtTitle.Text;
            if (lokRelationshipTail.SelectedItem == null)
                Message.RelationshipTailID = 0;
            else
                Message.RelationshipTailID = (int)lokRelationshipTail.SelectedValue;

            if (cmbOperator.SelectedItem != null)
                Message.EntityStateOperator = (Enum_EntityStateOperator)cmbOperator.SelectedItem;
            Message.SecuritySubjectInORNotIn = (InORNotIn)cmbInOrNotIn.SelectedItem;
            //EntityStateConditionDTO.Preserve = optPersist.IsChecked == true;
            if (optFormula.IsChecked == true)
            {
                Message.FormulaID = (int)lokFormula.SelectedValue;
                Message.ColumnID = 0;
            }
            else if (optColumn.IsChecked == true)
            {
                Message.FormulaID = 0;
                Message.ColumnID = (int)cmbColumns.SelectedValue;
            }
            return "";
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new EntityStateConditionDTO();
            ShowMessage();
        }






        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteConditionRequest != null)
                DeleteConditionRequest(this, Message);
        }
    }

}
