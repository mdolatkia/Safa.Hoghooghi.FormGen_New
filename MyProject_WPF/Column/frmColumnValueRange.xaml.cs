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
    public partial class frmColumnValueRange : UserControl
    {
        BizColumnValueRange bizColumnValueRange = new BizColumnValueRange();
        ColumnValueRangeDTO Message;
        int ColumnID { set; get; }
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();

        int EntityID { set; get; }
        //int EntityID { set; get; }
        public event EventHandler<SavedItemArg> ItemSaved;

        public frmColumnValueRange(int entityID, int columnID)
        {
            InitializeComponent();
            ColumnID = columnID;
            EntityID = entityID;
            ControlHelper.GenerateContextMenu(dtgColumnKeyValue);

            lokRelationshipTail.SelectionChanged += LokRelationshipTail_SelectionChanged;
            lokRelationshipTail.EditItemClicked += LokRelationshipTail_EditItemClicked;

            SetGridViewColumns();
            SetRelationshipTails();
            SetColumns();
            GetColumnValueRange(columnID);
            this.FlowDirection = FlowDirection.RightToLeft;
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
            columns.Insert(0, new ColumnDTO() { ID = 0, Alias = "عدم انتخاب" });

            cmbTagColumn.DisplayMemberPath = "Alias";
            cmbTagColumn.SelectedValuePath = "ID";
            cmbTagColumn.ItemsSource = columns;
            if (Message != null && Message.ID != 0)
            {
                if (Message.TagColumnID != 0)
                    cmbTagColumn.SelectedValue = Message.TagColumnID;
            }

            //cmbTag2Column.DisplayMemberPath = "Alias";
            //cmbTag2Column.SelectedValuePath = "ID";
            //cmbTag2Column.ItemsSource = columns;
            //if (Message != null && Message.ID != 0)
            //{
            //    if (Message.ColumnID != 0)
            //        cmbColumns.SelectedValue = Message.ColumnID;
            //}
        }
        private void GetColumnValueRange(int id)
        {
            Message = bizColumnValueRange.GetColumnValueRange(id);
            if (Message == null)
            {
                Message = new ColumnValueRangeDTO();
                Message.ID = ColumnID;
            }
            ShowMessage();
        }

        private void SetGridViewColumns()
        {
            dtgColumnKeyValue.Columns.Add(ControlHelper.GenerateGridviewColumn("Value", "مقدار", false, null, GridViewColumnType.Numeric));
            dtgColumnKeyValue.Columns.Add(ControlHelper.GenerateGridviewColumn("KeyTitle", "عنوان/کلید", false, null, GridViewColumnType.Text));
            dtgColumnKeyValue.Columns.Add(ControlHelper.GenerateGridviewColumn("Tag1", "مشخصه یک", false, null, GridViewColumnType.Text));
            dtgColumnKeyValue.Columns.Add(ControlHelper.GenerateGridviewColumn("Tag2", "مشخصه دو", false, null, GridViewColumnType.Text));
        }
        private void btnUpdateKeyValue_Click(object sender, RoutedEventArgs e)
        {
            //Message.ValueFromTitleOrValue = optValueComesFromTitle.IsChecked == true;
            Message.EntityRelationshipTailID = lokRelationshipTail.SelectedValue == null ? 0 : (int)lokRelationshipTail.SelectedValue;
            Message.TagColumnID = cmbTagColumn.SelectedValue == null ? 0 : (int)cmbTagColumn.SelectedValue;

            var id = bizColumnValueRange.UpdateColumnValueRange(Message);
            if (ItemSaved != null)
                ItemSaved(this, new MyProject_WPF.SavedItemArg() { ID = Message.ID });

            MessageBox.Show("اطلاعات ثبت شد");
        }
        private void ShowMessage()
        {
            // optValueComesFromTitle.IsChecked = Message.ValueFromTitleOrValue;
            //  optValueComesFromValue.IsChecked = !Message.ValueFromTitleOrValue;
            dtgColumnKeyValue.ItemsSource = Message.Details;

            lokRelationshipTail.SelectedValue = Message.EntityRelationshipTailID;
            cmbTagColumn.SelectedValue = Message.TagColumnID;
        }
        private void btnImportKeyValues_Click(object sender, RoutedEventArgs e)
        {
            //if (optValueComesFromTitle.IsChecked == false && optValueComesFromValue.IsChecked == false)
            //{
            //    MessageBox.Show("لطفا نوع جایگذاری مقادیر ستون در عنوان یا مقدار را مشخص نمایید");
            //    return;
            //}

            //**frmColumnValueRange.btnImportKeyValues_Click: 1489ef28f581
            BizDatabase bizDatabase = new BizDatabase();
            BizColumn bizColumn = new BizColumn();
            var column = bizColumn.GetColumnDTO(ColumnID, true);
            var database = bizDatabase.GetDatabaseByTableID(column.TableID);
            var dbHelper = MyModelGenerator.ModelGenerator.GetDatabaseImportHelper(database);
            var result = dbHelper.GetColumnValueRange(ColumnID);
            if (result != null)
            {
                Message.Details = result.Details;
                dtgColumnKeyValue.ItemsSource = null;
                dtgColumnKeyValue.ItemsSource = Message.Details;
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private void btnRemoveKeyValue_Click(object sender, RoutedEventArgs e)
        {
            bizColumnValueRange.RemoveColumnValueRangeID(ColumnID);
        }
    }
}
