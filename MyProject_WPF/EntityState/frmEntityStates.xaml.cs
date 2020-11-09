﻿using ModelEntites;
using MyCommonWPFControls;
using MyFormulaFunctionStateFunctionLibrary;

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
using Telerik.Windows.Controls.GridView;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityStates.xaml
    /// </summary>
    public partial class frmEntityStates : UserControl
    {
        EntityStateDTO StateDTO { set; get; }
        BizEntityState bizEntityState = new BizEntityState();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        int EntityID { set; get; }
        int EntityStateID { set; get; }
        public frmEntityStates(int entityID, int entityStateID)
        {
            InitializeComponent();
            EntityID = entityID;
            EntityStateID = entityStateID;
            SetColumns();
            SetRelationshipTails();
            SetFromulas();
            SetActionActivities();
            if (EntityStateID == 0)
            {
                StateDTO = new EntityStateDTO();
                ShowStateDTO();
            }
            else
                GetEntityState(EntityStateID);
            ControlHelper.GenerateContextMenu(dtgColumnValue);
            ControlHelper.GenerateContextMenu(dtgFormulaValue);
            lokRelationshipTail.SelectionChanged += LokRelationshipTail_SelectionChanged;
            lokRelationshipTail.EditItemClicked += LokRelationshipTail_EditItemClicked;
            cmbOperator.ItemsSource = Enum.GetValues(typeof(Enum_EntityStateOperator)).Cast<Enum_EntityStateOperator>();
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

        private void SetActionActivities()
        {
            //if (optPersist.IsChecked == true || optNotPersist.IsChecked == true)
            //{
            var cmbActionActivity = dtgActionActivities.Columns[0] as GridViewComboBoxColumn;
            cmbActionActivity.DisplayMemberPath = "Title";
            cmbActionActivity.SelectedValueMemberPath = "ID";
            BizUIActionActivity bizActionActivity = new BizUIActionActivity();
            cmbActionActivity.ItemsSource = bizActionActivity.GetActionActivities(EntityID, false);
            //}
        }
        private void SetFromulas()
        {
            cmbFormula.DisplayMemberPath = "Name";
            cmbFormula.SelectedValuePath = "ID";
            BizFormula bizFormula = new BizFormula();
            cmbFormula.ItemsSource = bizFormula.GetFormulas(EntityID);
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
            var columns = entity.Columns.Where(x => x.ForeignKey == false).ToList();
            cmbColumns.DisplayMemberPath = "Alias";
            cmbColumns.SelectedValuePath = "ID";
            cmbColumns.ItemsSource = columns;
            if (StateDTO != null && StateDTO.ID != 0)
            {
                if (StateDTO.ColumnID != 0)
                    cmbColumns.SelectedValue = StateDTO.ColumnID;
            }

        }

        private void GetEntityState(int entityStateID)
        {
            StateDTO = bizEntityState.GetEntityState(MyProjectManager.GetMyProjectManager.GetRequester(), entityStateID, false);
            ShowStateDTO();
        }

        private void ShowStateDTO()
        {
            txtTitle.Text = StateDTO.Title;
            dtgActionActivities.ItemsSource = StateDTO.ActionActivities;
            lokRelationshipTail.SelectedValue = StateDTO.RelationshipTailID;
            //if (StateDTO.Preserve)
            //    optPersist.IsChecked = true;
            //else
            //    optNotPersist.IsChecked = true;
            if (StateDTO.FormulaID != 0)
            {
                cmbFormula.SelectedValue = StateDTO.FormulaID;
                dtgFormulaValue.ItemsSource = StateDTO.Values;
                optFormula.IsChecked = true;
            }
            else if (StateDTO.ColumnID != 0)
            {
                cmbOperator.SelectedItem = StateDTO.EntityStateOperator;
                cmbColumns.SelectedValue = StateDTO.ColumnID;
                dtgColumnValue.ItemsSource = StateDTO.Values;
                optColumn.IsChecked = true;
            }
            else
            {
                dtgFormulaValue.ItemsSource = StateDTO.Values;
                dtgColumnValue.ItemsSource = StateDTO.Values;
            }
        }
        private void optFormula_Checked(object sender, RoutedEventArgs e)
        {
            tabColumn.Visibility = Visibility.Collapsed;
            tabFormula.Visibility = Visibility.Visible;
            tabFormula.IsSelected = true;
        }


        private void optColumn_Checked(object sender, RoutedEventArgs e)
        {
            tabFormula.Visibility = Visibility.Collapsed;
            tabColumn.Visibility = Visibility.Visible;
            tabColumn.IsSelected = true;
        }

        private void btnAddFormula_Click(object sender, RoutedEventArgs e)
        {
            frmFormula view = new frmFormula(0, EntityID);
            view.FormulaUpdated += View_FormulaSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form",Enum_WindowSize.Maximized);
        }

        private void View_FormulaSelected(object sender, FormulaSelectedArg e)
        {
            SetFromulas();
            cmbFormula.SelectedValue = e.FormulaID;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtTitle.Text == "")
            {
                MessageBox.Show("عنوان مناسب تعریف نشده است");
                return;
            }
            //if (optPersist.IsChecked == false && optNotPersist.IsChecked == false)
            //{
            //    MessageBox.Show("یکی از حالات ذخیره و یا عدم ذخیره را انتخاب نمایید");
            //    return;
            //}
            if (optFormula.IsChecked == false && optColumn.IsChecked == false)
            {
                MessageBox.Show("یکی از حالات فرمول و یا ستون را انتخاب نمایید");
                return;
            }
            if (optFormula.IsChecked == true)
            {
                if (cmbFormula.SelectedItem == null)
                {
                    MessageBox.Show("فرمول مشخص نشده است");
                    return;
                }
            }
            else if (optColumn.IsChecked == true)
            {
                if (cmbColumns.SelectedItem == null)
                {
                    MessageBox.Show("ستون مشخص نشده است");
                    return;
                }
            }
            if (lokRelationshipTail.SelectedItem == null)
                StateDTO.RelationshipTailID = 0;
            else
                StateDTO.RelationshipTailID = (int)lokRelationshipTail.SelectedValue;

            StateDTO.TableDrivedEntityID = EntityID;
            //if (cmbActionActivity.SelectedItem != null)
            //    StateDTO.ActionActivityID = (int)cmbActionActivity.SelectedValue;
            //else
            //    StateDTO.ActionActivityID = 0;
            StateDTO.Title = txtTitle.Text;
            StateDTO.EntityStateOperator = (Enum_EntityStateOperator)cmbOperator.SelectedItem;
            //StateDTO.Preserve = optPersist.IsChecked == true;
            if (optFormula.IsChecked == true)
            {
                StateDTO.FormulaID = (int)cmbFormula.SelectedValue;
                StateDTO.ColumnID = 0;
            }
            else if (optColumn.IsChecked == true)
            {
                StateDTO.FormulaID = 0;
                StateDTO.ColumnID = (int)cmbColumns.SelectedValue;
            }
            try
            {
                bizEntityState.UpdateEntityStates(StateDTO);
                MessageBox.Show("اطلاعات ثبت شد");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            StateDTO = new EntityStateDTO();
            ShowStateDTO();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            frmEntityStateSelect view = new MyProject_WPF.frmEntityStateSelect(EntityID);
            view.EntityStateSelected += View_EntityStateSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "frmEntityStateSelect");
        }

        private void View_EntityStateSelected(object sender, EntityStateSelectedArg e)
        {
            if (e.EntityStateID != 0)
            {
                GetEntityState(e.EntityStateID);
            }
        }

        //private void chkPersist_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (optNotPersist.IsChecked == true)
        //    {
        //        //if(StateDTO.ID!=0)
        //        //{
        //        if (StateDTO.ActionActivities.Any(x => x.Type == Enum_ActionActivityType.CodeFunction || x.Type == Enum_ActionActivityType.DatabaseFunction))
        //        {
        //            optPersist.IsChecked = true;
        //            MessageBox.Show("بعلت وجود اقدام غیر ظاهری ذخیره وضعیت اجباری میباشد");
        //        }
        //        //}
        //    }
        //    SetActionActivities();
        //}

        private void mnuRemoveItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var menuItem = sender as RadMenuItem;
            var contextMenu = menuItem.Parent as RadContextMenu;
            var source = contextMenu.GetClickedElement<GridViewRow>();
            if (contextMenu != null && source != null)
            {
                StateDTO.ActionActivities.Remove(source.DataContext as UIActionActivityDTO);
            }
        }

        private void mnuAddNewActionActivity_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //if (optPersist.IsChecked == false && optNotPersist.IsChecked == false)
            //{
            //    MessageBox.Show("لطفا یکی از حالات ذخیره و یا عدم ذخیره وضعیت را انتخاب نمایید");
            //    return;
            //}
            frmUIActionActivity view = new frmUIActionActivity(0, EntityID);
            view.ItemSaved += View_ItemSaved;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف اقدامات", Enum_WindowSize.Big);
        }
        private void View_ItemSaved(object sender, SavedItemArg e)
        {
            SetActionActivities();
            StateDTO.ActionActivities.Add(new UIActionActivityDTO() { ID = e.ID });
        }
        //private Enum_CodeFunctionParamType GetAllowedCodeFunctionParamType()
        //{
        //    throw new NotImplementedException();
        //}

        //private List<Enum_EntityActionActivityStep> GetAllowedActionActivitySteps()
        //{
        //    return new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.None };
        //}

        private List<Enum_ActionActivityType> GetAllowedActionActivityTypes()
        {
            //if (optPersist.IsChecked == true)
            //    return new List<Enum_ActionActivityType>() { Enum_ActionActivityType.ColumnValue, Enum_ActionActivityType.RelationshipEnablity, Enum_ActionActivityType.UIEnablity, Enum_ActionActivityType.CodeFunction, Enum_ActionActivityType.DatabaseFunction };
            //else if (optNotPersist.IsChecked == true)
            return new List<Enum_ActionActivityType>() { Enum_ActionActivityType.ColumnValue, Enum_ActionActivityType.UIEnablity };
            //else return new List<Enum_ActionActivityType>();
        }


        private void mnuEditActionActivity_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //if (optPersist.IsChecked == false && optNotPersist.IsChecked == false)
            //{
            //    MessageBox.Show("لطفا یکی از حالات ذخیره و یا عدم ذخیره وضعیت را انتخاب نمایید");
            //    return;
            //}
            var menuItem = sender as RadMenuItem;
            var contextMenu = menuItem.Parent as RadContextMenu;
            var source = contextMenu.GetClickedElement<GridViewRow>();
            if (contextMenu != null && source != null)
            {
                frmUIActionActivity view = new frmUIActionActivity((source.DataContext as UIActionActivityDTO).ID, EntityID);
                view.ItemSaved += View_ItemSavedEdit;
                MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف اقدامات", Enum_WindowSize.Big);
            }
        }
        private void View_ItemSavedEdit(object sender, SavedItemArg e)
        {
            SetActionActivities();
        }

        private void mnuAddNewItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            StateDTO.ActionActivities.Add(new UIActionActivityDTO());
        }
    }

}
