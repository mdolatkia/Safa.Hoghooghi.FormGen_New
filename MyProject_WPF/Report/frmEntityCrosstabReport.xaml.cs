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
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityCrosstabReport.xaml
    /// </summary>
    /// 

    public partial class frmEntityCrosstabReport : UserControl
    {
        private EntityCrosstabReportDTO Message { set; get; }
        BizEntityCrosstabReport bizEntityCrosstabReport = new BizEntityCrosstabReport();
        int EntityID { set; get; }
        public int SelectedEntityCrosstabReportID
        {
            get
            {
                if (Message == null)
                    return 0;
                else
                    return Message.ID;
            }
        }
        public frmEntityCrosstabReport(int entityID, int CrosstabReportID)
        {
            InitializeComponent();
            Message = new ModelEntites.EntityCrosstabReportDTO();
            EntityID = entityID;
            frmSearchableReport.EntityListViewChanged += FrmSearchableReport_EntityListViewChanged;

       
            SetFunctoinTypes();
            if (CrosstabReportID != 0)
            {
                GetEntityCrosstabReport(CrosstabReportID);
            }
            else
            {
                Message = new EntityCrosstabReportDTO();
                ShowMessage();
            }

            ControlHelper.GenerateContextMenu(dtgColumns);
            ControlHelper.GenerateContextMenu(dtgRows);
            ControlHelper.GenerateContextMenu(dtgValues);
        }

        private void FrmSearchableReport_EntityListViewChanged(object sender, EntityListViewDTO e)
        {
            if (e != null)
            {
                BizEntityListView biz = new BizEntityListView();
                var listView = biz.GetEntityListView(MyProjectManager.GetMyProjectManager.GetRequester(), e.ID);
                SetRowColumns(listView.EntityListViewAllColumns);
                SetColumnColumns(listView.EntityListViewAllColumns);
                SetValuesColumns(listView.EntityListViewAllColumns);
            }
        }


        private void SetColumnColumns(List<EntityListViewColumnsDTO> columns)
        {
            colColumnColumns.DisplayMemberPath = "Alias";
            colColumnColumns.SelectedValueMemberPath = "ID";
            colColumnColumns.ItemsSource = columns;
        }

        private void SetValuesColumns(List<EntityListViewColumnsDTO> columns)
        {
            colValueColumns.DisplayMemberPath = "Alias";
            colValueColumns.SelectedValueMemberPath = "ID";
            colValueColumns.ItemsSource = columns;
        }


        private void SetRowColumns(List<EntityListViewColumnsDTO> columns)
        {
            colRowColumns.DisplayMemberPath = "Alias";
            colRowColumns.SelectedValueMemberPath = "ID";
            colRowColumns.ItemsSource = columns;
        }


        private void SetFunctoinTypes()
        {
            var listFunctoins = bizEntityCrosstabReport.GetFunctionTypes();

            var rel = dtgValues.Columns[1] as GridViewComboBoxColumn;
            rel.ItemsSource = listFunctoins;
            //rel.DisplayMemberPath = "Name";
            //rel.SelectedValueMemberPath = "Value";
        }


        private void GetEntityCrosstabReport(int EntityCrosstabReportID)
        {
            Message = bizEntityCrosstabReport.GetEntityCrosstabReport(MyProjectManager.GetMyProjectManager.GetRequester(), EntityCrosstabReportID, true);
            ShowMessage();
        }
        private void ShowMessage()
        {
            frmSearchableReport.ShowMessage(Message);
            
            dtgColumns.ItemsSource = Message.Columns;
            dtgRows.ItemsSource = Message.Rows;
            dtgValues.ItemsSource = Message.Values;

        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!frmSearchableReport.FillMessage(Message))
            {
                return;
            }

            if (Message.Columns.Count == 0)
            {
                MessageBox.Show("ستون تعریف نشده است");
                return;
            }
            if (Message.Rows.Count == 0)
            {
                MessageBox.Show("ردیف تعریف نشده است");
                return;
            }
            if (Message.Values.Count == 0)
            {
                MessageBox.Show("مقدار تعریف نشده است");
                return;
            }
            Message.TableDrivedEntityID = EntityID;
            Message.ID = bizEntityCrosstabReport.UpdateEntityCrosstabReports(Message);
            MessageBox.Show("اطلاعات ثبت شد");
        }



        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new EntityCrosstabReportDTO();
            ShowMessage();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            frmEntityCrosstabReportSelect view = new MyProject_WPF.frmEntityCrosstabReportSelect(EntityID);
            view.EntityCrosstabReportSelected += View_EntityCrosstabReportSelected1;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "frmEntityCrosstabReportSelect");
        }

        private void View_EntityCrosstabReportSelected1(object sender, EntityCrosstabReportSelectedArg e)
        {
            if (e.EntityCrosstabReportID != 0)
            {
                GetEntityCrosstabReport(e.EntityCrosstabReportID);
            }
        }


    }
}
