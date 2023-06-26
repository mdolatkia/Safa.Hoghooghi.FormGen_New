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

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityListReport.xaml
    /// </summary>
    /// 

    public partial class frmExternalReport : UserControl
    {
        EntityExternalReportDTO Message { set; get; }
        BizEntityExternalReport bizEntityExternalReport = new BizEntityExternalReport();
        int EntityID { set; get; }
        public frmExternalReport(int entityID, int entityListReportID)
        {
            InitializeComponent();
            EntityID = entityID;
            if (entityListReportID != 0)
            {
                GetEntityExternalReport(entityListReportID);
            }
            else
            {
                Message = new EntityExternalReportDTO();
                ShowMessage();
            }

        }
       
        private void ShowMessage()
        {
            frmSearchableReport.EntityListViewVisiblity(false);
            frmSearchableReport.ShowMessage(Message);
            txtURL.Text = Message.URL;
          
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (!frmSearchableReport.FillMessage(Message))
            {
                return;
            }
            if (txtURL.Text == "")
            {
                MessageBox.Show("آدرس مناسب تعریف نشده است");
                return;
            }
          
            Message.TableDrivedEntityID = EntityID;
            Message.URL = txtURL.Text;
            bizEntityExternalReport.UpdateEntityExternalReports(MyProjectManager.GetMyProjectManager.GetRequester(), Message);
            MessageBox.Show("اطلاعات ثبت شد");
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new EntityExternalReportDTO();
            ShowMessage();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            frmExternalReportSelect view = new MyProject_WPF.frmExternalReportSelect(EntityID);
            view.EntityExternalReportSelected += View_EntityListReportSelected1;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "frmExternalReportSelect");
        }

        private void View_EntityListReportSelected1(object sender, EntityExternalReportSelectedArg e)
        {
            if (e.EntityExternalReportID != 0)
            {
                GetEntityExternalReport(e.EntityExternalReportID);
            }
        }

        private void GetEntityExternalReport(int entityListReportID)
        {
            Message = bizEntityExternalReport.GetEntityExternalReport(MyProjectManager.GetMyProjectManager.GetRequester(), entityListReportID, true);
            ShowMessage();
        }
    }
}
