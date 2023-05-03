using ModelEntites;
using MyModelManager;
using MyProject_WPF.Biz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for frmEditEntity.xaml
    /// </summary>
    public partial class frmEditBaseEntity : UserControl
    {
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        int EntityID { set; get; }
        EditBaseEntityDTO Message { set; get; }
        public frmEditBaseEntity(int entityID)
        {
            //frmEditBaseEntity: e6c6227212d5
            InitializeComponent();
            EntityID = entityID;
            //  Inheritance = inheritance;
            //if (Inheritance)
            //{
            //}
            //else
            //{
            //    pnlDisjoint.Visibility = System.Windows.Visibility.Collapsed;
            //    pnlParticipation.Visibility = System.Windows.Visibility.Collapsed;
            //}
            Message = new EditBaseEntityDTO();
            //   SetBaseEntity();
            SetEntities();

        }

        private void SetEntities()
        {
            Message.BaseEntity = bizTableDrivedEntity.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
            lblEntityName.Text = Message.BaseEntity.Name;

            BizISARelationship bizISARelationship = new BizISARelationship();
            Message.ISARelationship = bizISARelationship.GetInternalTableISARelationships(EntityID);
            if (Message.ISARelationship != null)
            {
                if (Message.ISARelationship.IsTolatParticipation)
                    optIsTolatParticipation.IsChecked = true;
                else
                    optIsPartialParticipation.IsChecked = true;
                if (Message.ISARelationship.IsDisjoint)
                    optIsDisjoint.IsChecked = true;
                else
                    optIsOverlap.IsChecked = true;
            }
            else
            {
                Message.ISARelationship = new ISARelationshipDTO();
            }

            if (Message.ISARelationship.ID != 0)
            {
                List<SuperToSubRelationshipDTO> listSuperToSubRels = new List<SuperToSubRelationshipDTO>();
                var drivedEntities = bizTableDrivedEntity.GetOtherDrivedEntityIDs(Message.ISARelationship);
                foreach (var id in drivedEntities)
                {
                    var entity = bizTableDrivedEntity.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), id, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
                    List<RelationshipDTO> removeRelationships = new List<RelationshipDTO>();
                    SubToSuperRelationshipDTO subToSuperRelationship = null;
                    SuperToSubRelationshipDTO superToSubRelationship = null;

                    foreach (var item in entity.Relationships.Where(x => x.TypeEnum == Enum_RelationshipType.SubToSuper))
                    {
                        if (item is SubToSuperRelationshipDTO)
                        {
                            if ((item as SubToSuperRelationshipDTO).ISARelationship.ID == Message.ISARelationship.ID)
                            {
                                subToSuperRelationship = (item as SubToSuperRelationshipDTO);
                                superToSubRelationship = Message.BaseEntity.Relationships.FirstOrDefault(x => x.ID == (item as SubToSuperRelationshipDTO).PairRelationshipID) as SuperToSubRelationshipDTO;
                            }
                        }
                    }

                    if (subToSuperRelationship != null && superToSubRelationship != null)
                    {
                        entity.Relationships.Remove(subToSuperRelationship);
                        Message.BaseEntity.Relationships.Remove(superToSubRelationship);

                        AddDrivedEntityTab(new Tuple<SuperToSubRelationshipDTO, SubToSuperRelationshipDTO, TableDrivedEntityDTO>(superToSubRelationship, subToSuperRelationship, entity));
                    }
                }


             

            }
            dtgRelationships.ItemsSource = Message.BaseEntity.Relationships;
            dtgColumns.ItemsSource = Message.BaseEntity.Columns;

        }

        private void AddDrivedEntityTab(Tuple<SuperToSubRelationshipDTO, SubToSuperRelationshipDTO, TableDrivedEntityDTO> tuple)
        {
            Message.DrivedEntities.Add(tuple);
            frmEditSubEntity view = new frmEditSubEntity(Message.BaseEntity, tuple, Message.BaseEntity.Columns);

            TabItem tab = new TabItem();
            tab.Header = tuple.Item3.Name;
            tab.Content = view;
            tabControl.Items.Add(tab);
            tab.IsSelected = true;
            view.NameChanged += (sender, e) => View_NameChanged(sender, e, tab);
        }

        private void View_NameChanged(object sender, string e, TabItem tab)
        {
            tab.Header = e;
        }



        //private List<TableDrivedEntityDTO> Message.DrivedEntities
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    foreach (var item in tabControl.Items)
        //    {
        //        if ((item as TabItem).Content is frmEditSubEntity)
        //        {
        //            TableDrivedEntityDTO subEntity = ((item as TabItem).Content as frmEditSubEntity).Message;
        //            result.Add(subEntity);
        //        }
        //    }
        //    return result;
        //}

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        void AddColumn(TableDrivedEntityDTO sourceEntity, ColumnDTO sourceColumn, TableDrivedEntityDTO targetEntity, bool move, RadGridView sourceDataGrid, RadGridView targetDataGrid, bool checkRelationship)
        {
            if (sourceEntity == Message.BaseEntity && sourceColumn.IsNull == false)
            {
                MessageBox.Show("ستون اجباری میباشد و امکان انتقال به موجودیت دیگر وجود ندارد");
                return;
            }
            if (sourceColumn.PrimaryKey)
            {
                MessageBox.Show("ستون کلید اصلی میباشد");
                return;
            }
            if (checkRelationship)
            {
                if (sourceEntity.Relationships.Any(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == sourceColumn.ID)))
                {
                    MessageBox.Show("ستون مربوط به یک رابطه میباشد");
                    return;
                }
            }
            if (!targetEntity.Columns.Any(x => x.ID == sourceColumn.ID))
            {
                targetEntity.Columns.Add(sourceColumn);
                targetDataGrid.ItemsSource = null;
                targetDataGrid.ItemsSource = targetEntity.Columns;
            }
            if (move)
            {
                sourceEntity.Columns.Remove(sourceColumn);
                sourceDataGrid.ItemsSource = null;
                sourceDataGrid.ItemsSource = sourceEntity.Columns;
            }
        }
        private void btnMoveColumnFromBaseToDrived_Click(object sender, RoutedEventArgs e)
        {
            frmEditSubEntity view = GetCurrentSubEntityForm();
            if (view != null)
            {
                if (dtgColumns.SelectedItem != null)
                {
                    var column = dtgColumns.SelectedItem as ColumnDTO;
                    AddColumn(Message.BaseEntity, column, view.Message.Item3, true, dtgColumns, view.dtgColumnsDrived, true);
                }
            }
        }
        private void btnMoveColumnFromDrivedToBase_Click(object sender, RoutedEventArgs e)
        {
            frmEditSubEntity view = GetCurrentSubEntityForm();
            if (view != null)
            {
                if (view.SelectedColumn != null)
                {
                    AddColumn(view.Message.Item3, view.SelectedColumn, Message.BaseEntity, true, view.dtgColumnsDrived, dtgColumns, true);
                }
            }
        }
        private void btnCopyColumnFromDrivedToAnother_Click(object sender, RoutedEventArgs e)
        {
            frmEditSubEntity view = GetCurrentSubEntityForm();
            if (view != null)
            {
                if (view.SelectedColumn != null)
                {

                    var otherSubEntities = GetAllSubEntityForm().Where(x => x != view).ToList(); ;
                    if (otherSubEntities.Any())
                    {
                        UpdateSubEntities();
                        SelectDrivedEntites viewSelect = new MyProject_WPF.SelectDrivedEntites(otherSubEntities);
                        viewSelect.EntitySelected += (sender1, e1) => ViewSelect_EntitySelected(sender1, e1, view, view.SelectedColumn);
                        MyProjectManager.GetMyProjectManager.ShowDialog(viewSelect, "Select", Enum_WindowSize.None);
                    }
                }
            }
        }
        private void ViewSelect_EntitySelected(object sender, frmEditSubEntity e, frmEditSubEntity sourceView, ColumnDTO column)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            AddColumn(sourceView.Message.Item3, column, e.Message.Item3, false, sourceView.dtgColumnsDrived, e.dtgColumnsDrived, true);

        }

        private void UpdateSubEntities()
        {
            foreach (var item in tabControl.Items)
            {
                if ((item as TabItem).Content is frmEditSubEntity)
                {
                    ((item as TabItem).Content as frmEditSubEntity).UpdateDrivedEntity();
                }
            }
        }


        //private frmEditSubEntity GetSubEntityForm(TableDrivedEntityDTO e)
        //{
        //    foreach (var item in tabControl.Items)
        //    {
        //        if ((item as TabItem).Content is frmEditSubEntity)
        //        {
        //            if (((item as TabItem).Content as frmEditSubEntity).Message == e)
        //                return (item as TabItem).Content as frmEditSubEntity;
        //        }
        //    }
        //    return null;
        //}

        //private void MoveColumnFromBaseToDrived(frmEditSubEntity view, ColumnDTO column, bool showMessage)
        //{


        //}


        //private void MoveColumnFromDrivedToBase(frmEditSubEntity view, ColumnDTO column, bool showMessage)
        //{

        //    if (column.PrimaryKey)
        //    {
        //        if (showMessage)
        //            MessageBox.Show("ستون کلید اصلی میباشد");
        //        return;
        //    }
        //    Message.BaseEntity.Columns.Add(column);
        //    view.RemoveColumn(column);
        //    dtgColumns.ItemsSource = null;
        //    dtgColumns.ItemsSource = Message.BaseEntity.Columns;
        //}

        void AddRelationship(TableDrivedEntityDTO sourceEntity, RelationshipDTO sourceRelationship, TableDrivedEntityDTO targetEntity, bool move
            , RadGridView sourceRelationshipDataGrid, RadGridView targetRelationshipDataGrid, RadGridView sourceColumnDataGrid, RadGridView targetColumnDataGrid)
        {
            BizRelationship bizRelationship = new BizRelationship();

            if (sourceEntity == Message.BaseEntity && bizRelationship.RelationshipIsNotNulable(sourceRelationship.ID))
            {
                var message = " این رابطه اجباری میباشند و امکان انتخاب شدن برای موجودیت های مشتق شده را ندارند";
                MessageBox.Show(message, "روابط اجباری");
                return;
            }

            if (sourceRelationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
            {
                if (!sourceRelationship.RelationshipColumns.Any(x => x.FirstSideColumn.PrimaryKey))
                {
                    foreach (var col in sourceRelationship.RelationshipColumns)
                    {
                        var fColumn = sourceEntity.Columns.FirstOrDefault(x => x.ID == col.FirstSideColumnID);
                        if (fColumn != null)
                            AddColumn(sourceEntity, fColumn, targetEntity, move, sourceColumnDataGrid, targetColumnDataGrid, false);
                    }
                }
            }
            if (!targetEntity.Relationships.Any(x => x.ID == sourceRelationship.ID))
            {
                targetEntity.Relationships.Add(sourceRelationship);
                targetRelationshipDataGrid.ItemsSource = null;
                targetRelationshipDataGrid.ItemsSource = targetEntity.Relationships;
            }
            if (move)
            {
                sourceEntity.Relationships.Remove(sourceRelationship);
                sourceRelationshipDataGrid.ItemsSource = null;
                sourceRelationshipDataGrid.ItemsSource = sourceEntity.Relationships;
            }

        }

        private void btnMoveRelationshipFromBaseToDerived_Click(object sender, RoutedEventArgs e)
        {
            if (dtgRelationships.SelectedItem != null)
            {
                var relationship = dtgRelationships.SelectedItem as RelationshipDTO;
                frmEditSubEntity view = GetCurrentSubEntityForm();
                if (view != null)
                {
                    AddRelationship(Message.BaseEntity, relationship, view.Message.Item3, true, dtgRelationships, view.dtgRelationshipsDrived, dtgColumns, view.dtgColumnsDrived);
                }
            }
        }

        private void btnMoveRelationshipFromDrivedToBase_Click(object sender, RoutedEventArgs e)
        {
            frmEditSubEntity view = GetCurrentSubEntityForm();
            if (view != null)
            {
                if (view.SelectedRelationship != null)
                {
                    AddRelationship(view.Message.Item3, view.SelectedRelationship, Message.BaseEntity, true, view.dtgRelationshipsDrived, dtgRelationships, view.dtgColumnsDrived, dtgColumns);
                }
            }
        }
        private void btnCopyRelationshipFromDrivedToAnother_Click(object sender, RoutedEventArgs e)
        {
            frmEditSubEntity view = GetCurrentSubEntityForm();
            if (view != null)
            {
                if (view.SelectedRelationship != null)
                {
                    var relationship = view.SelectedRelationship as RelationshipDTO;
                    var otherSubEntities = GetAllSubEntityForm().Where(x => x != view).ToList();
                    if (otherSubEntities.Any())
                    {
                        UpdateSubEntities();
                        SelectDrivedEntites viewSelect = new MyProject_WPF.SelectDrivedEntites(otherSubEntities);
                        viewSelect.EntitySelected += (sender1, e1) => ViewSelectRelationship_EntitySelected(sender1, e1, view, relationship);
                        MyProjectManager.GetMyProjectManager.ShowDialog(viewSelect, "Select", Enum_WindowSize.None);
                    }
                }
            }
        }
        private void ViewSelectRelationship_EntitySelected(object sender, frmEditSubEntity e, frmEditSubEntity sourceView, RelationshipDTO relationship)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            AddRelationship(sourceView.Message.Item3, relationship, e.Message.Item3, false, sourceView.dtgRelationshipsDrived, e.dtgRelationshipsDrived, sourceView.dtgColumnsDrived, e.dtgColumnsDrived);

        }

        private void btnNewEntity_Click_1(object sender, RoutedEventArgs e)
        {
            var newItem = new TableDrivedEntityDTO() { Name = "موجودیت جدید" };
            foreach (var item in Message.BaseEntity.Columns.Where(x => x.PrimaryKey))
                newItem.Columns.Add(item);

            SuperToSubRelationshipDTO superToSub = new SuperToSubRelationshipDTO();
            SubToSuperRelationshipDTO subToSuper = new SubToSuperRelationshipDTO();
            AddDrivedEntityTab(new Tuple<SuperToSubRelationshipDTO, SubToSuperRelationshipDTO, TableDrivedEntityDTO>(superToSub, subToSuper, newItem));
        }
        private frmEditSubEntity GetCurrentSubEntityForm()
        {
            if (tabControl.Items.Count > 0)
            {
                var tabItem = tabControl.SelectedItem as TabItem;
                if (tabItem != null)
                {
                    return tabItem.Content as frmEditSubEntity;
                }
            }
            return null;
        }
        private List<frmEditSubEntity> GetAllSubEntityForm()
        {
            List<frmEditSubEntity> result = new List<MyProject_WPF.frmEditSubEntity>();
            if (tabControl.Items.Count > 0)
            {
                foreach (TabItem tabItem in tabControl.Items)
                {
                    if (tabItem != null)
                    {
                        result.Add(tabItem.Content as frmEditSubEntity);
                    }
                }
            }
            return result;
        }
        private void btnDeleteEntity_Click(object sender, RoutedEventArgs e)
        {
            frmEditSubEntity view = GetCurrentSubEntityForm();
            if (view != null)
            {
                if (view.Message.Item3.Columns.Any(x=>!x.PrimaryKey))
                {
                    MessageBox.Show("لطفا ابتدا وضعیت ستونها موجودیت مشتق شده را تعیین نمایید");
                    return;
                }
                if (view.Message.Item3.Relationships.Any())
                {
                    MessageBox.Show("لطفا ابتدا وضعیت روابط موجودیت مشتق شده را تعیین نمایید");
                    return;
                }
                Message.DrivedEntities.Remove(view.Message);
                var tabItem = tabControl.SelectedItem as TabItem;
                if (tabItem != null)
                {
                    tabControl.Items.Remove(tabItem);
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //ستونهای رابطه برای مشتقها از موجودیت پایه حذف شوند
            //کد نوشته شود
            //if (txtName.Text == "")
            //{
            //    MessageBox.Show("نام موجودیت مشخص نشده است", "نام موجودیت");
            //    return;
            //}
            //if ((cmbDrivedEntities.SelectedValue == null ? 0 : (int)cmbDrivedEntities.SelectedValue) == BaseEntity.ID)
            //{
            //    if (txtCriteria.Text != "")
            //    {
            //        MessageBox.Show("برای موجودیت پایه شرط نمیتوان تعریف نمود", "شرط موجودیت");
            //        return;
            //    }
            //}
            //else
            //{
            //    if (txtCriteria.Text == "")
            //    {
            //        MessageBox.Show("شرط موجودیت مشخص نشده است", "شرط موجودیت");
            //        return;
            //    }
            //}
            //if (Inheritance)
            //{
            //if (!Message.DeterminerValues.Any())
            //{
            //    MessageBox.Show("مقادیر تعیین کننده نوع موجودیت مشخص نشده است");
            //    return;
            //}
            if (optIsDisjoint.IsChecked == false && optIsOverlap.IsChecked == false)
            {
                MessageBox.Show("نوع رابطه ارث بری مشخص نشده است", "Disjoint or Overlap");
                return;
            }
            if (optIsTolatParticipation.IsChecked == false && optIsPartialParticipation.IsChecked == false)
            {
                MessageBox.Show("نوع رابطه ارث بری مشخص نشده است", "TolatParticipation or PartialParticipatio");
                return;
            }
            foreach (var entity in Message.DrivedEntities)
            {
                foreach (var column in entity.Item3.Columns)
                {
                    if (!column.PrimaryKey)
                    {
                        if (Message.BaseEntity.Columns.Any(x => x.ID == column.ID))
                        {
                            MessageBox.Show("ستون" + " " + column.Name + " " + "همزمان در موجودیت پایه و همچنین زیر موجودیت" + " " + entity.Item3.Name + " " + "تعریف شده است");
                            return;
                        }
                    }
                }
                foreach (var rel in entity.Item3.Relationships)
                {
                    if (Message.BaseEntity.Relationships.Any(x => x.ID == rel.ID))
                    {
                        MessageBox.Show("رابطه" + " " + rel.Name + " " + "همزمان در موجودیت پایه و همچنین زیر موجودیت" + " " + entity.Item3.Name + " " + "تعریف شده است");
                        return;
                    }
                }
            }
            if (optIsOverlap.IsChecked == true)
            {
                foreach (var entity in Message.DrivedEntities)
                {
                    foreach (var column in entity.Item3.Columns)
                    {
                        if (Message.DrivedEntities.Any(x => x != entity && x.Item3.Columns.Any(y => !y.PrimaryKey && y.ID == column.ID)))
                        {
                            MessageBox.Show("ستون" + " " + column.Name + " " + "در بیش از یک زیر موجودیت تعریف شده و این برای روابط ارث بری" + " " + "Overlap" + " " + "امکان پذیر نمی باشد");
                            return;
                        }
                    }
                }
                foreach (var entity in Message.DrivedEntities)
                {
                    foreach (var relationship in entity.Item3.Relationships)
                    {
                        if (Message.DrivedEntities.Any(x => x != entity && x.Item3.Relationships.Any(y => y.ID == relationship.ID)))
                        {
                            MessageBox.Show("رابطه" + " " + relationship.Name + " " + "در بیش از یک زیر موجودیت تعریف شده و این برای روابط ارث بری" + " " + "Overlap" + " " + "امکان پذیر نمی باشد");
                            return;
                        }
                    }
                }
            }

            foreach (var entity in Message.DrivedEntities)
            {
                if (!entity.Item1.DeterminerColumnValues.Any())
                {
                    var message = "برای یک یا چند موجودیت مشتق ستون و یا مقدار تعیین کننده مشخص نشده است";
                    MessageBox.Show(message);
                    return;
                }
                if (optIsDisjoint.IsChecked == true)
                {
                    foreach (var values in entity.Item1.DeterminerColumnValues)
                    {
                        if (Message.DrivedEntities.Any(x => x != entity && x.Item1.DeterminerColumnValues.Any(y => y.Value == values.Value)))
                        {
                            var message = "مقدار تعیین کننده" + " '" + values + "' " + "برای بیش از یک زیر موجودیت تعریف شده است";
                            MessageBox.Show(message);
                            return;
                        }
                    }
                }
            }


            UpdateSubEntities();
            foreach (var DrivedEntity in Message.DrivedEntities)
            {
                if (DrivedEntity.Item1.SuperEntityDeterminerColumnID == 0)
                {
                    MessageBox.Show("برای زیر موجودیت" + " " + DrivedEntity.Item3.Name + " " + "ستون تعیین موجودیت مشخص نشده است");
                    return;
                }
                if (!DrivedEntity.Item1.DeterminerColumnValues.Any())
                {
                    MessageBox.Show("برای زیر موجودیت" + " " + DrivedEntity.Item3.Name + " " + "مقادیر تعیین موجودیت مشخص نشده است");
                    return;
                }
                if (!Message.BaseEntity.Columns.Any(x => x.ID == DrivedEntity.Item1.SuperEntityDeterminerColumnID))
                {
                    MessageBox.Show("ستون تعیین کننده برای زیر موجودیت" + " " + DrivedEntity.Item3.Name + " " + "در موجودیت پایه موجود نمی باشد");
                    return;
                }
            }
            foreach (var DrivedEntity in Message.DrivedEntities)
            {
                if (DrivedEntity.Item3.Name == "")
                {
                    var message = "برای یک یا چند موجودیت مشتق نام موجودیت تعریف نشده است";
                    MessageBox.Show(message);
                    return;
                }
            }

            Message.ISARelationship.IsDisjoint = optIsDisjoint.IsChecked == true;
            Message.ISARelationship.IsTolatParticipation = optIsTolatParticipation.IsChecked == true;

            BizTableDrivedEntity biz = new BizTableDrivedEntity();
            var id = biz.Save(MyProjectManager.GetMyProjectManager.GetRequester(), Message);
            MessageBox.Show("اطلاعات ثبت شد");
        }

    }
}
