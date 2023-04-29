using Microsoft.Win32;
using ModelEntites;
using MyCommonWPFControls;


using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityCommands.xaml
    /// </summary>
    public partial class frmDataMenuSetting : UserControl
    {
        BizEntitySearchableReport bizEntitySearchableReport = new BizEntitySearchableReport();
        DataMenuSettingDTO Message { set; get; }
        BizDataMenuSetting bizDataMenuSetting = new BizDataMenuSetting();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        TableDrivedEntityDTO Entity { set; get; }
        public event EventHandler<int> DataUpdated;
        int EntityID { set; get; }

        public frmDataMenuSetting(int entityID, int dataMenuSettingID)
        {
            InitializeComponent();
            EntityID = entityID;
            SetRelationshipTails();
            SetDataViewList();
            SetDataItemReports();
            if (dataMenuSettingID == 0)
            {
                Message = new DataMenuSettingDTO();
                ShowMessage();
            }
            else
                GetDataMenuSetting(dataMenuSettingID);
            Entity = bizTableDrivedEntity.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
            SetTabVisiblities();
            SetBaseTables();

            ControlHelper.GenerateContextMenu(dtgDataGridRelationships);
            ControlHelper.GenerateContextMenu(dtgDataViewRelationships);
            ControlHelper.GenerateContextMenu(dtgReportRelationships);
            ControlHelper.GenerateContextMenu(dtgDataItemReport);

            //ControlHelper.GenerateContextMenu(dtgExternalReports);
            dtgReportRelationships.RowLoaded += DtgColumns_RowLoaded;
            dtgReportRelationships.CellEditEnded += DtgConditions_CellEditEnded;

            dtgDataViewRelationships.RowLoaded += DtgColumns_RowLoaded;
            dtgDataViewRelationships.CellEditEnded += DtgConditions_CellEditEnded;

            dtgDataGridRelationships.RowLoaded += DtgColumns_RowLoaded;
            dtgDataGridRelationships.CellEditEnded += DtgConditions_CellEditEnded;

            colReportRelationshipTail.EditItemClicked += ColRelationshipTail_EditItemClicked;
            colDataViewRelationshipTail.EditItemClicked += ColRelationshipTail_EditItemClicked;
            colDataGridRelationshipTail.EditItemClicked += ColRelationshipTail_EditItemClicked;

            colDataGridRelTargetDataMenuSetting.NewItemEnabled = true;
            colDataViewRelTargetDataMenuSetting.NewItemEnabled = true;
            colDataGridRelTargetDataMenuSetting.EditItemEnabled = true;
            colDataViewRelTargetDataMenuSetting.EditItemEnabled = true;
            colDataViewRelTargetDataMenuSetting.EditItemClicked += ColDataGridRelTargetDataMenuSetting_EditItemClicked;
            colDataGridRelTargetDataMenuSetting.EditItemClicked += ColDataGridRelTargetDataMenuSetting_EditItemClicked;

            lokRelationship.SelectionChanged += LokRelationship_SelectionChanged;
            lokDataMenuSetting.EditItemClicked += LokDataMenuSetting_EditItemClicked;
            lokDataMenuSetting.EditItemEnabled = true;
            lokDataMenuSetting.NewItemEnabled = true;
        }

        private void LokDataMenuSetting_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            var context = e.DataConext as DataMenuSettingDTO;
            if (lokRelationship.SelectedItem != null)
            {
                var relationship = lokRelationship.SelectedItem as RelationshipDTO;

                frmDataMenuSetting frm = new frmDataMenuSetting(relationship.EntityID2, lokDataMenuSetting.SelectedItem == null ? 0 : (int)lokDataMenuSetting.SelectedValue);
                MyProjectManager.GetMyProjectManager.ShowDialog(frm, "تنظیمات منو");
                frm.DataUpdated += (sender1, e1) => Frm_TailSelected(sender1, e1, sender as MyStaticLookup);
            }
        }
        private void Frm_TailSelected(object sender1, int e1, MyStaticLookup myStaticLookup)
        {
            var relationship = lokRelationship.SelectedItem as RelationshipDTO;
            lokDataMenuSetting.ItemsSource = null;
            SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), relationship.EntityID2);
            myStaticLookup.SelectedValue = e1;
        }

        private void SetTabVisiblities()
        {
            //** 71a75126-c643-4bbd-b001-8605f9c20cc9
            if (Entity.IsView)
            {
                tabDataGridRelationships.Visibility = Visibility.Collapsed;
                tabDataViewRelationships.Visibility = Visibility.Collapsed;
                tabDataItemReport.Visibility = Visibility.Collapsed;
                tabReportRelationships.Visibility = Visibility.Collapsed;
                tabView.IsSelected = true;
            }
            else
            {
                tabView.Visibility = Visibility.Collapsed;
            }
        }

        private void LokRelationship_SelectionChanged(object sender, SelectionChangedArg e)
        {
            if (e.SelectedItem != null)
            {
                var relationship = e.SelectedItem as RelationshipDTO;
                SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), relationship.EntityID2);
            }
            else
            {
                lokDataMenuSetting.ItemsSource = null;
            }
        }

        private void SetRelationshipDataMenus(DR_Requester dR_Requester, int entityID2)
        {
            var list = bizDataMenuSetting.GetDataMenuSettings(MyProjectManager.GetMyProjectManager.GetRequester(), entityID2);
            lokDataMenuSetting.DisplayMember = "Name";
            lokDataMenuSetting.SelectedValueMember = "ID";
            lokDataMenuSetting.ItemsSource = list;
            //if(Message.ID!=0)
            //{
            //    lokDataMenuSetting.SelectedValue = Message.TargetDataMenuSettingID;
            //}
        }

        private void SetBaseTables()
        {
            lokRelationship.SelectedValueMember = "ID";
            lokRelationship.DisplayMember = "Name";
            lokRelationship.ItemsSource = Entity.Relationships;
        }

        private void SetDataItemReports()
        {
            BizEntityDataItemReport bizEntityDataItemReport = new BizEntityDataItemReport();
            var reports = bizEntityDataItemReport.GetEntityDataItemReports(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
            colDataItemReport.SelectedValueMemberPath = "ID";
            colDataItemReport.DisplayMemberPath = "ReportTitle";
            colDataItemReport.ItemsSource = reports;
        }

        private void ColDataGridRelTargetDataMenuSetting_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            if (e.DataConext is DataMenuDataViewRelationshipDTO)
            {
                var context = e.DataConext as DataMenuDataViewRelationshipDTO;
                if (context.RelationshipTailID != 0)
                {
                    var tail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), context.RelationshipTailID);
                    frmDataMenuSetting frm = new frmDataMenuSetting(tail.TargetEntityID, context.TargetDataMenuSettingID);
                    MyProjectManager.GetMyProjectManager.ShowDialog(frm, "رابطه های مرتبط");
                    frm.DataUpdated += (sender1, e1) => Frm_TailSelected(sender1, e1, sender as MyStaticLookup, context);
                }
            }
            else if (e.DataConext is DataMenuGridViewRelationshipDTO)
            {
                var context = e.DataConext as DataMenuGridViewRelationshipDTO;
                var tail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), context.RelationshipTailID);
                frmDataMenuSetting frm = new frmDataMenuSetting(tail.TargetEntityID, context.TargetDataMenuSettingID);
                MyProjectManager.GetMyProjectManager.ShowDialog(frm, "رابطه های مرتبط");
                frm.DataUpdated += (sender1, e1) => Frm_TailSelected(sender1, e1, sender as MyStaticLookup, context);
            }
        }
        private void Frm_TailSelected(object sender1, int e1, MyStaticLookup myStaticLookup, DataMenuDataViewRelationshipDTO dataContext)
        {
            SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), dataContext);
            myStaticLookup.SelectedValue = e1;
        }
        private void Frm_TailSelected(object sender1, int e1, MyStaticLookup myStaticLookup, DataMenuGridViewRelationshipDTO dataContext)
        {
            SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), dataContext);
            myStaticLookup.SelectedValue = e1;
        }


        private void SetRelationshipTails()
        {
            BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
            var relationshipTails = bizEntityRelationshipTail.GetEntityRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);

            colDataGridRelationshipTail.DisplayMemberPath = "EntityPath";
            colDataGridRelationshipTail.SelectedValueMemberPath = "ID";
            colDataGridRelationshipTail.ItemsSource = relationshipTails;

            colDataViewRelationshipTail.DisplayMemberPath = "EntityPath";
            colDataViewRelationshipTail.SelectedValueMemberPath = "ID";
            colDataViewRelationshipTail.ItemsSource = relationshipTails;

            colReportRelationshipTail.DisplayMemberPath = "EntityPath";
            colReportRelationshipTail.SelectedValueMemberPath = "ID";
            colReportRelationshipTail.ItemsSource = relationshipTails;

        }
        private void SetDataViewList()
        {
            BizEntityListView bizEntityListView = new BizEntityListView();
            lokEntityDataView.DisplayMember = "Title";
            lokEntityDataView.SelectedValueMember = "ID";
            lokEntityDataView.ItemsSource = bizEntityListView.GetEntityListViews(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
        }
        private void DtgColumns_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.DataElement is DataMenuSearchableReportRelationshipDTO)
            {
                var data = (e.DataElement as DataMenuSearchableReportRelationshipDTO);
                if (data.vwReports == null || data.vwReports.Count == 0)
                    SetRelationshipReports(MyProjectManager.GetMyProjectManager.GetRequester(), data);
            }
            else if (e.DataElement is DataMenuDataViewRelationshipDTO)
            {
                var data = (e.DataElement as DataMenuDataViewRelationshipDTO);
                if (data.vwDataMenuSettings == null || data.vwDataMenuSettings.Count == 0)
                    SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), data);
            }
            else if (e.DataElement is DataMenuGridViewRelationshipDTO)
            {
                var data = (e.DataElement as DataMenuGridViewRelationshipDTO);
                if (data.vwDataMenuSettings == null || data.vwDataMenuSettings.Count == 0)
                    SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), data);
            }
        }

        private void DtgConditions_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell.Column == colReportRelationshipTail)
            {
                if (e.Cell.DataContext is DataMenuSearchableReportRelationshipDTO)
                {
                    var condition = (e.Cell.DataContext as DataMenuSearchableReportRelationshipDTO);
                    SetRelationshipReports(MyProjectManager.GetMyProjectManager.GetRequester(), condition);
                }
            }
            else if (e.Cell.Column == colDataViewRelationshipTail)
            {
                if (e.Cell.DataContext is DataMenuDataViewRelationshipDTO)
                {
                    var condition = (e.Cell.DataContext as DataMenuDataViewRelationshipDTO);
                    SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), condition);
                }
            }
            else if (e.Cell.Column == colDataGridRelationshipTail)
            {
                if (e.Cell.DataContext is DataMenuGridViewRelationshipDTO)
                {
                    var condition = (e.Cell.DataContext as DataMenuGridViewRelationshipDTO);
                    SetRelationshipDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), condition);
                }
            }
        }

        private void SetRelationshipReports(DR_Requester requester, DataMenuSearchableReportRelationshipDTO condition)
        {
            if (condition.RelationshipTailID == 0)
                return;
            colReports.DisplayMemberPath = "ReportTitle";
            colReports.SelectedValueMemberPath = "ID";
            //    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(condition.EntityRelationshipTailID);
            var reports = bizEntitySearchableReport.GetEntityReportsOfRelationshipTail(requester, condition.RelationshipTailID);
            condition.vwReports = reports;
        }

        private void SetRelationshipDataMenus(DR_Requester requester, DataMenuDataViewRelationshipDTO condition)
        {
            if (condition.RelationshipTailID == 0)
                return;
            colDataViewRelTargetDataMenuSetting.DisplayMemberPath = "Name";
            colDataViewRelTargetDataMenuSetting.SelectedValueMemberPath = "ID";
            //    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(condition.EntityRelationshipTailID);
            var items = bizDataMenuSetting.GetDataMenusOfRelationshipTail(requester, condition.RelationshipTailID);
            condition.vwDataMenuSettings = items;
        }
        private void SetRelationshipDataMenus(DR_Requester requester, DataMenuGridViewRelationshipDTO condition)
        {
            if (condition.RelationshipTailID == 0)
                return;
            colDataGridRelTargetDataMenuSetting.DisplayMemberPath = "Name";
            colDataGridRelTargetDataMenuSetting.SelectedValueMemberPath = "ID";
            //    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(condition.EntityRelationshipTailID);
            var items = bizDataMenuSetting.GetDataMenusOfRelationshipTail(requester, condition.RelationshipTailID);
            condition.vwDataMenuSettings = items;
        }
        private void ColRelationshipTail_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
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

        private void GetDataMenuSetting(int ID)
        {
            Message = bizDataMenuSetting.GetDataMenuSetting(MyProjectManager.GetMyProjectManager.GetRequester(), ID, true);
            ShowMessage();
        }
        private void ShowMessage()
        {
            if (Message.ID == 0)
                btnSetDataMenuSetting.IsEnabled = false;
            else
                btnSetDataMenuSetting.IsEnabled = true;
            dtgReportRelationships.ItemsSource = Message.SearchableReportRelationships;
            dtgDataViewRelationships.ItemsSource = Message.DataViewRelationships;
            dtgDataGridRelationships.ItemsSource = Message.GridViewRelationships;
            dtgDataItemReport.ItemsSource = Message.DataItemReports;
            txtName.Text = Message.Name;
            lokRelationship.SelectedValue = Message.RelationshipID;
            lokDataMenuSetting.SelectedValue = Message.TargetDataMenuSettingID;
            foreach (var item in Message.SearchableReportRelationships)
            {
                SetRelationshipReports(MyProjectManager.GetMyProjectManager.GetRequester(), item);
            }

            lokEntityDataView.SelectedValue = Message.EntityListViewID;
            if (Message.IconContent != null)
            {
                grdExisting.Visibility = Visibility.Visible;
                grdAddFile.Visibility = Visibility.Collapsed;
                txtExistingFile.Text = "دارای فایل";
            }
            else
            {
                grdExisting.Visibility = Visibility.Collapsed;
                grdAddFile.Visibility = Visibility.Visible;
            }
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Icons (*.png,*.ico)|*.png;*.ico|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }
        //private void AddFile_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "Icons (*.png,*.ico)|*.png;*.ico|All files (*.*)|*.*";
        //    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    //if (openFileDialog.ShowDialog() == true)
        //    //{
        //    //    txtFilePath.Text = openFileDialog.FileName;
        //    //}
        //}

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Message.DataViewRelationships)
            {
                if (item.RelationshipTailID != 0)
                {
                    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), item.RelationshipTailID);
                    BizTableDrivedEntity bizTableDrivedEntity = new MyModelManager.BizTableDrivedEntity();
                    var entity = bizTableDrivedEntity.GetSimpleEntity(MyProjectManager.GetMyProjectManager.GetRequester(), relationshipTail.TargetEntityID);
                    var linkedServerMessage = bizEntityRelationshipTail.CheckLinkedServers(entity, relationshipTail.ReverseRelationshipTail);
                    if (linkedServerMessage != "")
                    {
                        var message = "اشکال در تعریف نمایش داده و لینک سرور برای موجودیت" + " " + string.IsNullOrEmpty(relationshipTail.TargetEntityAlias);
                        message += Environment.NewLine + linkedServerMessage;
                        MessageBox.Show(message);
                        return;
                    }
                }
            }
            foreach (var item in Message.GridViewRelationships)
            {
                if (item.RelationshipTailID != 0)
                {
                    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), item.RelationshipTailID);
                    BizTableDrivedEntity bizTableDrivedEntity = new MyModelManager.BizTableDrivedEntity();
                    var entity = bizTableDrivedEntity.GetSimpleEntity(MyProjectManager.GetMyProjectManager.GetRequester(), relationshipTail.TargetEntityID);
                    var linkedServerMessage = bizEntityRelationshipTail.CheckLinkedServers(entity, relationshipTail.ReverseRelationshipTail);
                    if (linkedServerMessage != "")
                    {
                        var message = "اشکال در تعریف گرید داده و لینک سرور برای موجودیت" + " " + string.IsNullOrEmpty(relationshipTail.TargetEntityAlias);
                        message += Environment.NewLine + linkedServerMessage;
                        MessageBox.Show(message);
                        return;
                    }
                }
            }
            foreach (var item in Message.SearchableReportRelationships)
            {
                if (item.RelationshipTailID != 0)
                {
                    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), item.RelationshipTailID);
                    BizTableDrivedEntity bizTableDrivedEntity = new MyModelManager.BizTableDrivedEntity();
                    var entity = bizTableDrivedEntity.GetSimpleEntity(MyProjectManager.GetMyProjectManager.GetRequester(), relationshipTail.TargetEntityID);
                    var linkedServerMessage = bizEntityRelationshipTail.CheckLinkedServers(entity, relationshipTail.ReverseRelationshipTail);
                    if (linkedServerMessage != "")
                    {
                        var message = "اشکال در تعریف گزارشات مرتبط و لینک سرور برای موجودیت" + " " + string.IsNullOrEmpty(relationshipTail.TargetEntityAlias);
                        message += Environment.NewLine + linkedServerMessage;
                        MessageBox.Show(message);
                        return;
                    }
                }
                if (item.EntitySearchableReportID == 0)
                {
                    var message = "اشکال در تعریف گزارشات مرتبط";
                    message += Environment.NewLine + "گزارش مرتبط اجباری می باشد";
                    MessageBox.Show(message);
                    return;
                }
            }
            if (Entity.IsView)
            {
                if (lokRelationship.SelectedItem != null)
                {
                    if (lokDataMenuSetting.SelectedItem == null)
                    {
                        MessageBox.Show("تنظیمات منوی رابطه مشخص نشده است");
                        return;
                    }
                }

            }
            if (Message.SearchableReportRelationships.Any(x => x.RelationshipTailID == 0))
            {
                MessageBox.Show("انتخاب رابطه و گزارش برای لیست گزارشات اجباری می باشد");
                return;
            }
            if (txtFilePath.Text != "")
            {
                Message.IconContent = File.ReadAllBytes(txtFilePath.Text);
            }
            if (lokEntityDataView.SelectedItem != null)
                Message.EntityListViewID = (int)lokEntityDataView.SelectedValue;
            else
                Message.EntityListViewID = 0;
            Message.EntityID = EntityID;
            Message.Name = txtName.Text;

            if (lokRelationship.SelectedItem != null)
                Message.RelationshipID = (int)lokRelationship.SelectedValue;
            if (lokDataMenuSetting.SelectedItem != null)
                Message.TargetDataMenuSettingID = (int)lokDataMenuSetting.SelectedValue;


            bizDataMenuSetting.UpdateEntityReportDataMenuSettings(Message);
            MessageBox.Show("اطلاعات ثبت شد");
        }
        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            txtFilePath.Text = "";
            grdExisting.Visibility = Visibility.Collapsed;
            grdAddFile.Visibility = Visibility.Visible;
            Message.IconContent = null;
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new DataMenuSettingDTO();
            ShowMessage();
        }

        private void BtnSetDataMenuSetting_Click(object sender, RoutedEventArgs e)
        {
            if (bizDataMenuSetting.SetDefaultDataMenuSetting(EntityID, Message.ID))
                MessageBox.Show("تنظیمات منوی پیشفرض تعیین شد");
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            frmDataMenuSettingSelect view = new MyProject_WPF.frmDataMenuSettingSelect(EntityID);
            view.ItemSelected += View_ItemSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "frmEntityListViewSelect", Enum_WindowSize.Big);
        }

        private void View_ItemSelected(object sender, int e)
        {
            if (e != 0)
            {
                MyProjectManager.GetMyProjectManager.CloseDialog(sender);
                GetDataMenuSetting(e);
            }
        }


        //private void RemoveFile_Click(object sender, RoutedEventArgs e)
        //{
        //    txtFilePath.Text = "";
        //    grdExisting.Visibility = Visibility.Collapsed;
        //    grdAddFile.Visibility = Visibility.Visible;
        //    Message.IconContent = null;
        //}
    }

}
