using Microsoft.Win32;
using ModelEntites;
using MyCommonWPFControls;



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
    public partial class frmGraph : UserControl
    {
        BizDataMenuSetting bizEntityDataMenu = new BizDataMenuSetting();
        GraphDTO Message { set; get; }
        BizGraph bizGraph = new BizGraph();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        BizEntityRelationshipTailDataMenu bizEntityRelationshipTailDataMenu = new BizEntityRelationshipTailDataMenu();

        int FirstEntityID { set; get; }
        public event EventHandler<UpdatedEventArg> Updated;
        public frmGraph(int entityID, int GraphID)
        {
            InitializeComponent();
            lokFirstSideEntity.SelectionChanged += LokFirstSideEntity_SelectionChanged;
            FirstEntityID = entityID;
            //   var entity = bizTableDrivedEntity.GetTableDrivedEntity(EntityID, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships);
            SetEntities();

            if (GraphID == 0)
            {
                NewItem();
            }
            else
            {
                GetGraph(GraphID);
            }
            //SetRelationshipTails();


            colRelationshipTail.NewItemEnabled = true;
            colRelationshipTail.EditItemEnabled = true;
            colRelationshipTail.EditItemClicked += ColRelationshipTail_EditItemClicked;

            colRelationshipTailDataMenu.SelectedValueMemberPath = "ID";
            colRelationshipTailDataMenu.DisplayMemberPath = "Name";
            colRelationshipTailDataMenu.NewItemEnabled = true;
            colRelationshipTailDataMenu.EditItemEnabled = true;
            colRelationshipTailDataMenu.EditItemClicked += colRelationshipTailDataMenu_EditItemClicked;

            dtgRelationships.RowLoaded += DtgRelationships_RowLoaded;
            dtgRelationships.CellEditEnded += DtgRelationships_CellEditEnded;
            ControlHelper.GenerateContextMenu(dtgRelationships);
        }
        private void LokFirstSideEntity_SelectionChanged(object sender, SelectionChangedArg e)
        {
            if (e.SelectedItem != null)
            {
                var entity = e.SelectedItem as TableDrivedEntityDTO;
                var listFirst = bizEntityDataMenu.GetDataMenuSettings(MyProjectManager.GetMyProjectManager.GetRequester(), entity.ID);
                lokFirstDataMenu.ItemsSource = listFirst;
                lokFirstDataMenu.SelectedValueMember = "ID";
                lokFirstDataMenu.DisplayMember = "Name";

                if (Message.ID != 0)
                    lokFirstDataMenu.SelectedValue = Message.TableDrivedEntityID;
            }
            else
            {
                lokFirstDataMenu.ItemsSource = null;
            }
        }
        private void colRelationshipTailDataMenu_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            if (e.DataConext is GraphRelationshipTailDTO)
            {
                var item = (e.DataConext as GraphRelationshipTailDTO);
                if (item.RelationshipTailID != 0)
                {
                    ftmEntityRelationshipTailDataMenu view = new ftmEntityRelationshipTailDataMenu(item.EntityRelationshipTailDataMenuID, item.RelationshipTailID);
                    MyProjectManager.GetMyProjectManager.ShowDialog(view, "لیست نمایش رابطه");
                    view.EntityDataMenuUpdated += (sender1, e1) => View_ItemSelected1(sender1, e1, item, (sender as MyStaticLookup));
                }
            }
        }
        private void View_ItemSelected1(object sender, EntityDataMenuUpdatedArg e, GraphRelationshipTailDTO item, MyStaticLookup lookup)
        {
            SetRelationshipTailDataMenuList(item);
            lookup.SelectedValue = e.ID;
        }
        private void DtgRelationships_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell.Column == colRelationshipTail)
            {
                if (e.Cell.DataContext is GraphRelationshipTailDTO)
                {
                    var condition = (e.Cell.DataContext as GraphRelationshipTailDTO);
                    SetRelationshipTailDataMenuList(condition);
                }
            }
        }

        private void DtgRelationships_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.DataElement is GraphRelationshipTailDTO)
            {
                SetRelationshipTailDataMenuList(e.DataElement as GraphRelationshipTailDTO);
            }
        }

        private void SetRelationshipTailDataMenuList(GraphRelationshipTailDTO GraphRelationshipTailDTO)
        {
            if (GraphRelationshipTailDTO.RelationshipTailID != 0)
            {
                var list = bizEntityRelationshipTailDataMenu.GetEntityRelationshipTailDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), GraphRelationshipTailDTO.RelationshipTailID);
                GraphRelationshipTailDTO.tmpEntityRelationshipTailDataMenus = list;
            }
            else
            {
                GraphRelationshipTailDTO.tmpEntityRelationshipTailDataMenus = null;
            }
        }
        private void ColRelationshipTail_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            if (lokFirstSideEntity.SelectedItem != null)
            {
                frmEntityRelationshipTail view = new frmEntityRelationshipTail((int)lokFirstSideEntity.SelectedValue);
                MyProjectManager.GetMyProjectManager.ShowDialog(view, "رابطه");
                view.ItemSelected += (sender1, e1) => View_ItemSelected(sender1, e1, (sender as MyStaticLookup));
            }
        }
        private void View_ItemSelected(object sender, EntityRelationshipTailSelectedArg e, MyStaticLookup lookup)
        {
            SetRelationshipTails();
            lookup.SelectedValue = e.EntityRelationshipTailID;
        }
        private void SetEntities()
        {
            lokFirstSideEntity.DisplayMember = "Name";
            lokFirstSideEntity.SelectedValueMember = "ID";
            lokFirstSideEntity.SearchFilterChanged += LokEntitiesFirst_SearchFilterChanged;
        }
        private void LokEntitiesFirst_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    var list = bizTableDrivedEntity.GetAllEnbaledEntitiesDTO(MyProjectManager.GetMyProjectManager.GetRequester(), e.SingleFilterValue);
                    e.ResultItemsSource = list;
                }
                else
                {
                    var id = Convert.ToInt32(e.SingleFilterValue);
                    if (id > 0)
                    {
                        //lokSecondSideEntity.ItemsSource = bizTableDrivedEntity.GetAllEntities();
                        var entity = bizTableDrivedEntity.GetSimpleEntity(MyProjectManager.GetMyProjectManager.GetRequester(), id);
                        e.ResultItemsSource = new List<TableDrivedEntityDTO> { entity };
                    }
                    else
                        e.ResultItemsSource = null;
                }
            }
        }


        private void GetGraph(int ID)
        {
            Message = bizGraph.GetGraph(MyProjectManager.GetMyProjectManager.GetRequester(), ID);
            ShowMessage();
        }
        private void SetRelationshipTails()
        {

            //چک شود فقط یکی ازین دو پر شوند
            if (lokFirstSideEntity.SelectedItem != null)
            {
                var relationshipTails = bizEntityRelationshipTail.GetEntityRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokFirstSideEntity.SelectedValue);
                colRelationshipTail.DisplayMemberPath = "EntityPath";
                colRelationshipTail.SelectedValueMemberPath = "ID";
                colRelationshipTail.ItemsSource = relationshipTails;
            }
        }
        private void ShowMessage()
        {

            //  var entity = bizTableDrivedEntity.GetTableDrivedEntity(EntityID, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships);
            lokFirstSideEntity.SelectedValue = Message.TableDrivedEntityID;
            chkNotJoint.IsChecked = Message.NotJointEntities;
            if (Message.ID == 0)
            {
                lokFirstSideEntity.IsEnabled = false;
            }
            else
            {
                lokFirstSideEntity.IsEnabled = false;
            }
            lokFirstDataMenu.SelectedValue = Message.FirstSideDataMenuID;
            txtName.Text = Message.ReportTitle;
            //}
            //else
            //{
            //    var entity = bizTableDrivedEntity.GetTableDrivedEntity(Message.FirstSideEntityID, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships);
            //    txtFirstSideEntity.Text = entity.Alias;
            //    lokSecondSideEntity.IsEnabled = false;
            //}
            SetRelationshipTails();
            dtgRelationships.ItemsSource = Message.RelationshipsTails;
            //txtName.Text = Message.Name;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("نام");
                return;
            }
            if (lokFirstSideEntity.SelectedItem == null)
            {
                MessageBox.Show("موجودیت سمت اول انتخاب نشده است");
                return;
            }


            foreach (var item in Message.RelationshipsTails)
            {
                if (item.RelationshipTailID != 0)
                {
                    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), item.RelationshipTailID);
                    BizTableDrivedEntity bizTableDrivedEntity = new MyModelManager.BizTableDrivedEntity();
                    var entity = bizTableDrivedEntity.GetSimpleEntity(MyProjectManager.GetMyProjectManager.GetRequester(), relationshipTail.TargetEntityID);
                    var viewMessage = bizEntityRelationshipTail.CheckTailHasRelationshipWithView(relationshipTail);
                    if (viewMessage != "")
                    {
                        var message = "اشکال در تعریف لینک داده با موجودیت" + " " + string.IsNullOrEmpty(relationshipTail.TargetEntityAlias);
                        message += Environment.NewLine + "در رشته رابطه ارتباط با نمای" + " " + viewMessage + " " + "امکان پذیر نمی باشد";
                        MessageBox.Show(message);
                        return;
                    }
                }
            }

            foreach (var item in Message.RelationshipsTails)
            {

                if (item.RelationshipTailID != 0)
                {
                    var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), item.RelationshipTailID);
                    var linkedServerMessage = bizEntityRelationshipTail.CheckRelationshipsLinkedServers(relationshipTail);
                    if (linkedServerMessage != "")
                    {
                        var message = "اشکال در رشته رابطه" + " " + string.IsNullOrEmpty(relationshipTail.EntityPath);
                        message += Environment.NewLine + linkedServerMessage;
                        MessageBox.Show(message);
                        return;
                    }
                }
            }

            Message.ReportTitle = txtName.Text;
            if (lokFirstDataMenu.SelectedItem != null)
                Message.FirstSideDataMenuID = (int)lokFirstDataMenu.SelectedValue;
            Message.TableDrivedEntityID = (int)lokFirstSideEntity.SelectedValue;
            Message.NotJointEntities = chkNotJoint.IsChecked == true;
            Message.ID = bizGraph.UpdateGraph(Message);
            MessageBox.Show("اطلاعات ثبت شد");
            if (Updated != null)
                Updated(this, new MyProject_WPF.UpdatedEventArg() { ID = Message.ID });
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewItem();
        }

        private void NewItem()
        {
            Message = new GraphDTO();
            Message.TableDrivedEntityID = FirstEntityID;
            ShowMessage();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            frmGraphSelect view = new MyProject_WPF.frmGraphSelect(FirstEntityID);
            view.LetterTemplateSelected += View_LetterTemplateSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form");
        }

        private void View_LetterTemplateSelected(object sender, GraphSelectedArg e)
        {
            if (e.ID != 0)
            {
                GetGraph(e.ID);
            }
        }
    }

}
