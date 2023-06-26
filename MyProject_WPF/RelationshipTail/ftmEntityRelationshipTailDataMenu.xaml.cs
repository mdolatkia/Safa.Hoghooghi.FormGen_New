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
    /// Interaction logic for ftmEntityRelationshipTailDataMenu.xaml
    /// </summary>
    /// 

    public partial class ftmEntityRelationshipTailDataMenu : UserControl
    {
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();

        BizEntityListView bizEntityListView = new BizEntityListView();
        BizDataMenuSetting bizEntityDataMenu = new BizDataMenuSetting();
        BizEntityRelationshipTailDataMenu bizEntityRelationshipTailDataMenu = new BizEntityRelationshipTailDataMenu();

        //    int EntityID { set; get; }
        //  int EntityDataMenuID { set; get; }
        public event EventHandler<EntityDataMenuUpdatedArg> EntityDataMenuUpdated;
        EntityRelationshipTailDTO EntityRelationshipTail { set; get; }
        EntityRelationshipTailDataMenuDTO Message { set; get; }
        public ftmEntityRelationshipTailDataMenu(int EntityRelationshipTailDataMenu, int relationshipTailID)
        {
            InitializeComponent();

            EntityRelationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), relationshipTailID);
            //    BizTableDrivedEntity biz = new BizTableDrivedEntity();
            //    var entity = biz.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            //    if (entity.IsView)
            //       colRelationshipTail.IsVisible = false;
            // EntityDataMenuID = entityDataMenuID;
            SetItems();
            if (EntityRelationshipTailDataMenu == 0)
            {
                Message = new EntityRelationshipTailDataMenuDTO();
                ShowMessage();
            }
            else
                GetEntityRelationshipTailDataMenu(EntityRelationshipTailDataMenu);

            colDataMenu.DisplayMemberPath = "Name";
            colDataMenu.SelectedValueMemberPath = "ID";

            ControlHelper.GenerateContextMenu(dtgColumns);
        }

        //private void DtgColumns_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        //{
        //    if (e.DataElement is EntityDataMenuColumnsDTO)
        //    {
        //        var data = (e.DataElement as EntityDataMenuColumnsDTO);
        //        if (data.vwValueColumns == null || data.vwValueColumns.Count == 0)
        //            SetColumns(data);
        //    }
        //}

        //private void DtgConditions_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        //{
        //    if (e.Cell.Column == colRelationshipTail)
        //    {
        //        if (e.Cell.DataContext is EntityDataMenuColumnsDTO)
        //        {
        //            var condition = (e.Cell.DataContext as EntityDataMenuColumnsDTO);
        //            SetColumns(condition);
        //        }
        //    }
        //}

        //private void SetColumns(EntityDataMenuColumnsDTO condition)
        //{
        //    colColumns.DisplayMemberPath = "Name";
        //    colColumns.SelectedValueMemberPath = "ID";
        //    BizTableDrivedEntity biz = new BizTableDrivedEntity();
        //    if (condition.RelationshipTailID == 0)
        //    {
        //        var entity = biz.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
        //        condition.vwValueColumns = entity.Columns; ;
        //    }
        //    else
        //    {
        //        var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), condition.RelationshipTailID);
        //        var entity = biz.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), relationshipTail.TargetEntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
        //        condition.vwValueColumns = entity.Columns;
        //    }
        //}


        private void SetItems()
        {
           

            List<EntityRelationshipTailDataMenuItemsDTO> items = new List<EntityRelationshipTailDataMenuItemsDTO>();
            items.Add(new EntityRelationshipTailDataMenuItemsDTO() { Path = EntityRelationshipTail.RelationshipIDPath, EntityName = EntityRelationshipTail.InitialiEntityAlias, TableDrivedEntityID = EntityRelationshipTail.InitialEntityID });


            if (EntityRelationshipTail.ChildTail != null)
                SetItems(items, EntityRelationshipTail.ChildTail);

            foreach (var item in items)
            {
                if (item.TableDrivedEntityID != 0)
                {
                    item.tmpDataMenus = bizEntityDataMenu.GetDataMenuSettings(MyProjectManager.GetMyProjectManager.GetRequester(), item.TableDrivedEntityID, DetailsDepth.SimpleInfo);
                    item.tmpListView = bizEntityListView.GetEntityListViews(MyProjectManager.GetMyProjectManager.GetRequester(), item.TableDrivedEntityID);
                }
            }


            dtgColumns.ItemsSource = items;
        }

        private void SetItems(List<EntityRelationshipTailDataMenuItemsDTO> items, EntityRelationshipTailDTO childTail)
        {
            items.Add(new EntityRelationshipTailDataMenuItemsDTO() { Path = childTail.RelationshipIDPath, EntityName = childTail.Relationship.Entity1Alias, TableDrivedEntityID = childTail.Relationship.EntityID1 });
            if (childTail.ChildTail != null)
                SetItems(items, childTail.ChildTail);
            else
                items.Add(new EntityRelationshipTailDataMenuItemsDTO() { Path = "", EntityName = EntityRelationshipTail.TargetEntityAlias, TableDrivedEntityID = EntityRelationshipTail.TargetEntityID });

        }

        private void GetEntityRelationshipTailDataMenu(int entityDataMenuID)
        {
            Message = bizEntityRelationshipTailDataMenu.GetEntityRelationshipTailDataMenu(MyProjectManager.GetMyProjectManager.GetRequester(), entityDataMenuID);
            ShowMessage();
        }
        private void ShowMessage()
        {
            //if (Message.ID == 0)
            //    btnSetDefaultDataMenu.IsEnabled = false;
            //else
            //    btnSetDefaultDataMenu.IsEnabled = true;
            txtViewName.Text = Message.Name;

        
            List<EntityRelationshipTailDataMenuItemsDTO> source = dtgColumns.ItemsSource as List<EntityRelationshipTailDataMenuItemsDTO>;
            foreach (var item in Message.Items)
            {
                var fitem = source.FirstOrDefault(x => x.Path == item.Path);
                if (fitem != null)
                {
                    fitem.DataMenuSettingID = item.DataMenuSettingID;
                    fitem.EntityListViewID = item.EntityListViewID;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtViewName.Text == "")
            {
                MessageBox.Show("عنوان مناسب تعریف نشده است");
                return;
            }

            Message.EntityRelationshipTailID = EntityRelationshipTail.ID;
            Message.Items = (dtgColumns.ItemsSource as List<EntityRelationshipTailDataMenuItemsDTO>);
            Message.Name = txtViewName.Text;
         
            Message.ID = bizEntityRelationshipTailDataMenu.UpdateEntityRelationshipTailDataMenu(Message);

            if (EntityDataMenuUpdated != null)
                EntityDataMenuUpdated(this, new MyProject_WPF.EntityDataMenuUpdatedArg() { ID = Message.ID });
            MessageBox.Show("اطلاعات ثبت شد");
        }



        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new EntityRelationshipTailDataMenuDTO();
            ShowMessage();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            ftmEntityRelationshipTailDataMenuSelect view = new MyProject_WPF.ftmEntityRelationshipTailDataMenuSelect(EntityRelationshipTail.ID);
            view.EntityDataMenuSelected += View_EntityDataMenuSelected1;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "ftmEntityRelationshipTailDataMenuSelect", Enum_WindowSize.Big);
        }

        private void View_EntityDataMenuSelected1(object sender, EntityDataMenuSelectedArg e)
        {
            if (e.ID != 0)
            {
                MyProjectManager.GetMyProjectManager.CloseDialog(sender);
                GetEntityRelationshipTailDataMenu(e.ID);
            }
        }


    }
    public class EntityDataMenuUpdatedArg : EventArgs
    {
        public int ID { set; get; }
    }
}
