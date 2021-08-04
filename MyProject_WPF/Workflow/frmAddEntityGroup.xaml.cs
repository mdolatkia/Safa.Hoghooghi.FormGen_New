using ModelEntites;
using MyCommonWPFControls;
using MyModelManager;

using ProxyLibrary.Workflow;
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
    /// Interaction logic for frmAddSelectAction.xaml
    /// </summary>
    public partial class frmAddEntityGroup : UserControl
    {
        int ProcessID { set; get; }

        EntityGroupDTO Message = new EntityGroupDTO();
        BizEntityGroup bizEntityGroup = new BizEntityGroup();
        public event EventHandler<SavedItemArg> ItemSaved;
        int EntityID { set; get; }
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();

        public frmAddEntityGroup(int processID, int entityID, int entityGroupID)
        {
            InitializeComponent();
            ProcessID = processID;
            EntityID = entityID;
            SetRelationshipTails();
      

            ControlHelper.GenerateContextMenu(dtgRelationshipTails);

            colRelationshipTail.NewItemEnabled = true;
            colRelationshipTail.EditItemEnabled = true;
            colRelationshipTail.EditItemClicked += ColRelationshipTail_EditItemClicked;

            if (entityGroupID != 0)
                GetEntityGroup(entityGroupID);
            else
                SetNewItem();
        }
        private void ColRelationshipTail_EditItemClicked(object sender, EditItemClickEventArg e)
        {
          
                frmEntityRelationshipTail view = new frmEntityRelationshipTail(EntityID);
                MyProjectManager.GetMyProjectManager.ShowDialog(view, "رابطه");
                view.ItemSelected += (sender1, e1) => View_ItemSelected(sender1, e1, (sender as MyStaticLookup));
           
        }
        private void View_ItemSelected(object sender, EntityRelationshipTailSelectedArg e, MyStaticLookup lookup)
        {
            SetRelationshipTails();
            lookup.SelectedValue = e.EntityRelationshipTailID;
        }
        private void SetRelationshipTails()
        {
            //BizRelationship bizRelationship = new BizRelationship();
            //var col7 = dtgEntities.Columns[0] as MyStaticLookupColumn;
            //BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            //var entity = bizTableDrivedEntity.GetTableDrivedEntity(EntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
            //col7.ItemsSource = entity.Relationships;
            //col7.DisplayMemberPath = "Name";
            //col7.SelectedValueMemberPath = "ID";


    
                var relationshipTails = bizEntityRelationshipTail.GetEntityRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
                relationshipTails.Add(new ModelEntites.EntityRelationshipTailDTO() { ID = 0, EntityPath = "موجودیت اصلی" });
                colRelationshipTail.DisplayMemberPath = "EntityPath";
                colRelationshipTail.SelectedValueMemberPath = "ID";
                colRelationshipTail.ItemsSource = relationshipTails;
      

        }

        //private void Col7_SearchFilterChanged(object sender, SearchFilterArg e)
        //{


        //    if (!string.IsNullOrEmpty(e.SingleFilterValue))
        //    {
        //        if (e.FilerBySelectedValue)
        //        {
        //            var id = Convert.ToInt32(e.SingleFilterValue);
        //            if (id > 0)
        //            {
        //                var relatoinship = bizRelationship.GetRelationship(id);
        //                e.ResultItemsSource = new List<RelationshipDTO> { relatoinship };
        //            }
        //            else
        //                e.ResultItemsSource = null;
        //        }
        //        else
        //        {
        //            e.ResultItemsSource = bizRelationship.GetRelationships1(EntityID);
        //        }
        //    }
        //    else if (e.Filters.Count > 0)
        //    {

        //    }
        //}

        private void GetEntityGroup(int entityGroupID)
        {
            Message = bizEntityGroup.GetEntityGroup(MyProjectManager.GetMyProjectManager.GetRequester(), entityGroupID, true);
            ShowMessage();
        }

        private void ShowMessage()
        {
            //if (Message.ID != 0)
            //    ProcessID = Message.ProcessID;
            txtName.Text = Message.Name;
            //txtDescription.Text = Message.Description;
            dtgRelationshipTails.ItemsSource = Message.Relationships;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("نام وارد نشده است");
                return;
            }

            Message.ProcessID = ProcessID;
            Message.Name = txtName.Text;
            //Message.Description = txtDescription.Text;

            Message.ID = bizEntityGroup.UpdateEntityGroups(Message);
            if (ItemSaved != null)
                ItemSaved(this, new SavedItemArg() { ID = Message.ID });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            SetNewItem();

        }

        private void SetNewItem()
        {
            Message = new EntityGroupDTO();
            ShowMessage();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }
    }

}
