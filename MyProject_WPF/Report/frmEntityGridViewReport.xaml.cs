using ModelEntites;
using MyCommonWPFControls;

using MyModelManager;
using MyUILibrary.EntityArea;
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
    /// Interaction logic for frmEntityGridViewReport.xaml
    /// </summary>
    /// 

    public partial class frmEntityGridViewReport : UserControl
    {
        EntityGridViewReportDTO Message { set; get; }
        BizEntityGridViewReport bizEntityGridViewReport = new BizEntityGridViewReport();
        int EntityID { set; get; }
        public frmEntityGridViewReport(int entityID, int entityViewReportID)
        {
            InitializeComponent();
            EntityID = entityID;
          
            //  SetSubReports();
            //    SetSubReportRelationships();
            SetDataMenuSetting();
            if (entityViewReportID != 0)
            {
                GetEntityGridViewReport(entityViewReportID);
            }
            else
            {
                Message = new EntityGridViewReportDTO();
                ShowMessage();
            }

        }
     

        private void SetDataMenuSetting()
        {
            if (lokDataMenuSetting.ItemsSource == null)
            {
                lokDataMenuSetting.EditItemClicked += lokDataMenuSetting_EditItemClicked;
            }
            BizDataMenuSetting biz = new BizDataMenuSetting();
            lokDataMenuSetting.DisplayMember = "Name";
            lokDataMenuSetting.SelectedValueMember = "ID";
            lokDataMenuSetting.ItemsSource = biz.GetDataMenuSettings(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID,DetailsDepth.SimpleInfo);
        }

        private void lokDataMenuSetting_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var lookup = (sender as MyStaticLookup);
            frmDataMenuSetting view;
            if (lookup.SelectedItem == null)
            {
                view = new frmDataMenuSetting(EntityID, 0);
            }
            else
            {
                view = new frmDataMenuSetting(EntityID, (int)lookup.SelectedValue);
            }
            view.DataUpdated += (sender1, e1) => View_EntityListViewUpdated(sender1, e1, lookup);
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تنظیمات نامه");
        }
        private void View_EntityListViewUpdated(object sender, int e, MyStaticLookup lookup)
        {
            SetDataMenuSetting();
            lookup.SelectedValue = e;
        }
        //private void SetSubReportRelationships()
        //{
        //    BizRelationship bizRelationship = new BizRelationship();
        //    BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        //    var entity = bizTableDrivedEntity.GetTableDrivedEntity(EntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
        //    var relatinships = entity.Relationships;
        //    var rel = dtgSubReports.Columns[2] as GridViewComboBoxColumn;
        //    rel.ItemsSource = relatinships;
        //    rel.DisplayMemberPath = "Alias";
        //    rel.SelectedValueMemberPath = "ID";
        //}

        //private void SetSubReports()
        //{

        //    var listGridViewReports = bizEntityGridViewReport.GetEntityGridViewReports();

        //    var rel = dtgSubReports.Columns[3] as GridViewComboBoxColumn;
        //    rel.ItemsSource = listGridViewReports;
        //    rel.DisplayMemberPath = "ReportTitle";
        //    rel.SelectedValueMemberPath = "ID";
        //}

        private void ShowMessage()
        {
            frmSearchableReport.ShowMessage(Message);
            lokDataMenuSetting.SelectedValue = Message.DataMenuSettingID;
        
            //  dtgSubReports.ItemsSource = Message.EntityGridViewReportSubs;
            //SetGridSearch();
        }

        //private void SetGridSearch()
        //{
        //    //grdSearch.Children.Clear();
        //    //grdSearch.Children.Add(new frmSearchEntityArea(EntityID, Message.EntitySearchID)
        //}

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!frmSearchableReport.FillMessage(Message))
            {
                return;
            }
            if (lokDataMenuSetting.SelectedItem == null)
            {
                MessageBox.Show("لطفا ابتدا لیست ستونها را ثبت و انتخاب نمایید");
                return;
            }
           
            Message.TableDrivedEntityID = EntityID;
           
            Message.DataMenuSettingID = (int)lokDataMenuSetting.SelectedValue;
        

       
            bizEntityGridViewReport.UpdateEntityGridViewReports(Message);
            MessageBox.Show("اطلاعات ثبت شد");
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new EntityGridViewReportDTO();
            ShowMessage();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            frmEntityGridViewReportSelect view = new MyProject_WPF.frmEntityGridViewReportSelect(EntityID);
            view.EntityGridViewReportSelected += View_EntityGridViewReportSelected1;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "frmEntityGridViewReportSelect");
        }

        private void View_EntityGridViewReportSelected1(object sender, EntityGridViewReportSelectedArg e)
        {
            if (e.EntityGridViewReportID != 0)
            {
                GetEntityGridViewReport(e.EntityGridViewReportID);
            }
        }

        private void GetEntityGridViewReport(int entityGridViewReportID)
        {
            Message = bizEntityGridViewReport.GetEntityGridViewReport(MyProjectManager.GetMyProjectManager.GetRequester(), entityGridViewReportID, true);
            ShowMessage();
        }
        //SearchEntityArea SearchEntityArea;
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SearchEntityArea == null)
        //    {
        //        SearchEntityArea = new SearchEntityArea();
        //        var searchViewInitializer = new SearchEntityAreaInitializer();
        //        searchViewInitializer.EntityID = EntityID;
        //        SearchEntityArea.SetAreaInitializer(searchViewInitializer);
        //        SearchEntityArea.SearchDataDefined += SearchEntityArea_SearchDataDefined;
        //    }
        //    MyProjectManager.GetMyProjectManager.ShowDialog(SearchEntityArea.SearchView, "جستجو");
        //}

        //private void SearchEntityArea_SearchDataDefined(object sender, SearchDataArg e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
