using Microsoft.Win32;
using ModelEntites;
using MyCommonWPFControls;
using MyFormulaFunctionStateFunctionLibrary;

using MyModelManager;
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
    public partial class frmEntityDataView : UserControl
    {
        DataViewSettingDTO Message { set; get; }
        BizEntityDataView bizEntityDataView = new BizEntityDataView();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        int EntityID { set; get; }

        public frmEntityDataView(int entityID)
        {
            InitializeComponent();
            EntityID = entityID;
            SetDataViewList();
            //SetRelationships();
            //SetRelationshipTails();
            GetEntityCommand(entityID);

            ControlHelper.GenerateContextMenu(dtgDataViewRelationships);
            colDataViewRelationshipTail.EditItemClicked += ColRelationshipTail_EditItemClicked;

            //ControlHelper.GenerateContextMenu(dtgRelationships);
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

        private void SetRelationshipTails()
        {
            BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
            var relationshipTails = bizEntityRelationshipTail.GetEntityRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);

            //colDataGridRelationshipTail.DisplayMemberPath = "EntityPath";
            //colDataGridRelationshipTail.SelectedValueMemberPath = "ID";
            //colDataGridRelationshipTail.ItemsSource = relationshipTails;

            colDataViewRelationshipTail.DisplayMemberPath = "EntityPath";
            colDataViewRelationshipTail.SelectedValueMemberPath = "ID";
            colDataViewRelationshipTail.ItemsSource = relationshipTails;

            //colReportRelationshipTail.DisplayMemberPath = "EntityPath";
            //colReportRelationshipTail.SelectedValueMemberPath = "ID";
            //colReportRelationshipTail.ItemsSource = relationshipTails;

        }
        //private void SetRelationshipTails()
        //{//چک شود فقط یکی ازین دو پر شوند
        //    var relationshipTails = bizEntityRelationshipTail.GetEntityRelationshipTails(EntityID);
        //    colRelationshipTail.DisplayMemberPath = "EntityPath";
        //    colRelationshipTail.SelectedValueMemberPath = "ID";
        //    colRelationshipTail.ItemsSource = relationshipTails;

        //}

        //private void SetRelationships()
        //{
        //    var entity = bizTableDrivedEntity.GetTableDrivedEntity(EntityID, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithRelationships);
        //    var relationships = entity.Relationships;
        //    colRelationship.DisplayMemberPath = "Alias";
        //    colRelationship.SelectedValueMemberPath = "ID";
        //    colRelationship.ItemsSource = relationships;
        //}

        private void SetDataViewList()
        {
            BizEntityListView bizEntityListView = new BizEntityListView();
            lokEntityDataView.DisplayMember = "Title";
            lokEntityDataView.SelectedValueMember = "ID";
            lokEntityDataView.ItemsSource = bizEntityListView.GetEntityListViews(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
        }
        private void GetEntityCommand(int entityID)
        {
            Message = bizEntityDataView.GetDataViewSetting(entityID, false);
            if (Message == null)
                Message = new DataViewSettingDTO();
            ShowMessage();
        }

        private void ShowMessage()
        {
            lokEntityDataView.SelectedValue = Message.EntityListViewID;
            dtgDataViewRelationships.ItemsSource = Message.DataViewRelationships;
            //dtgRelationships.ItemsSource = Message.EntityDataViewRelationships;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           

            if (txtFilePath.Text != "")
            {
                Message.IconContent = File.ReadAllBytes(txtFilePath.Text);
            }
            if (lokEntityDataView.SelectedItem != null)
                Message.EntityListViewID = (int)lokEntityDataView.SelectedValue;
            else
                Message.EntityListViewID = 0;
            bizEntityDataView.UpdateEntityDataViews(EntityID, Message);
            MessageBox.Show("اطلاعات ثبت شد");
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new DataViewSettingDTO();
            ShowMessage();
        }

        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            txtFilePath.Text = "";
            grdExisting.Visibility = Visibility.Collapsed;
            grdAddFile.Visibility = Visibility.Visible;
            Message.IconContent = null;
        }
    }

}
