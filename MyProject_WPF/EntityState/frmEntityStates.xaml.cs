using ModelEntites;
using MyCommonWPFControls;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityStates.xaml
    /// </summary>
    public partial class frmEntityStates : UserControl
    {
        public event EventHandler<SavedItemArg> ItemSaved;
        EntityStateDTO StateDTO { set; get; }
        BizEntityState bizEntityState = new BizEntityState();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        int EntityID { set; get; }
        int EntityStateID { set; get; }
        BizSecuritySubject bizSecuritySubject = new BizSecuritySubject();

        public frmEntityStates(int entityID, int entityStateID)
        {
            // frmEntityStates: 1082873315d1
            InitializeComponent();
            EntityID = entityID;
            EntityStateID = entityStateID;


            SetActionActivities();


            //cmbConditionOperator.ItemsSource = Enum.GetValues(typeof(AndOREqualType)).Cast<AndOREqualType>();

            if (EntityStateID == 0)
            {
                StateDTO = new EntityStateDTO();
                ShowStateDTO();
            }
            else
                GetEntityState(EntityStateID);

            colActionActivity.EditItemEnabled = true;
            colActionActivity.NewItemEnabled = true;
            colActionActivity.EditItemClicked += ColActionActivity_EditItemClicked;
        }

        private void ColActionActivity_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            frmUIActionActivity view;
            if ((sender as MyStaticLookup).SelectedItem == null)
            {
                view = new frmUIActionActivity(0, EntityID);
            }
            else
            {
                var id = ((sender as MyStaticLookup).SelectedItem as UIActionActivityDTO).ID;
                view = new frmUIActionActivity(id, EntityID);
            }
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف اقدامات", Enum_WindowSize.Big);

            view.ItemSaved += (sender1, e1) => View_ItemSavedEntityGroup(sender1, e1, (sender as MyStaticLookup));
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form", Enum_WindowSize.Big);
        }

        private void View_ItemSavedEntityGroup(object sender1, SavedItemArg e1, MyStaticLookup myStaticLookup)
        {
            SetActionActivities();
            myStaticLookup.SelectedValue = e1.ID;
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


        private void GetEntityState(int entityStateID)
        {
            StateDTO = bizEntityState.GetEntityState(MyProjectManager.GetMyProjectManager.GetRequester(), entityStateID,false);
            ShowStateDTO();
        }
        //  frmEntityStateCondition conditionView;
        private void ShowStateDTO()
        {
            txtTitle.Text = StateDTO.Title;
            dtgActionActivities.ItemsSource = StateDTO.ActionActivities;
            //       cmbConditionOperator.SelectedItem = StateDTO.ConditionOperator;

            //   conditionView = new frmEntityStateCondition(EntityID, condition);
            // view.VerticalAlignment = VerticalAlignment.Stretch;
            //TabItem tab = new TabItem();
            //tab.VerticalAlignment = VerticalAlignment.Stretch;
            //tab.Header = condition.Title;
            //tab.Content = view;
            //view.DeleteConditionRequest += View_DeleteConditionRequest;
            //    grdCondition.Children.Clear();
            //  grdCondition.Children.Add(conditionView);


            dtgSecuritySubjects.ItemsSource = StateDTO.SecuritySubjects;
            lokRelationshipTail.SelectedValue = StateDTO.RelationshipTailID;
            cmbOperator.SelectedItem = StateDTO.EntityStateOperator;
            if (StateDTO.FormulaID != 0)
            {
                lokFormula.SelectedValue = StateDTO.FormulaID;
                optFormula.IsChecked = true;
            }
            else if (StateDTO.ColumnID != 0)
            {
                cmbColumns.SelectedValue = StateDTO.ColumnID;
                optColumn.IsChecked = true;
            }

            dtgValues.ItemsSource = StateDTO.Values;
            cmbInOrNotIn.SelectedItem = StateDTO.SecuritySubjectInORNotIn;


            //   tab.IsSelected = true;

            //foreach (var item in StateDTO.StateConditions)
            //{
            // AddConditin(item);
            //}
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
            //StateDTO.ConditionOperator = (AndOREqualType)cmbConditionOperator.SelectedItem;
            StateDTO.TableDrivedEntityID = EntityID;
            //if (cmbActionActivity.SelectedItem != null)
            //    StateDTO.ActionActivityID = (int)cmbActionActivity.SelectedValue;
            //else
            //    StateDTO.ActionActivityID = 0;
            StateDTO.Title = txtTitle.Text;
            StateDTO.Title = txtTitle.Text;
            if (lokRelationshipTail.SelectedItem == null)
                StateDTO.RelationshipTailID = 0;
            else
                StateDTO.RelationshipTailID = (int)lokRelationshipTail.SelectedValue;

            if (cmbOperator.SelectedItem != null)
                StateDTO.EntityStateOperator = (InORNotIn)cmbOperator.SelectedItem;
            StateDTO.SecuritySubjectInORNotIn = (InORNotIn)cmbInOrNotIn.SelectedItem;
            //EntityStateConditionDTO.Preserve = optPersist.IsChecked == true;
            if (optFormula.IsChecked == true)
            {
                StateDTO.FormulaID = (int)lokFormula.SelectedValue;
                StateDTO.ColumnID = 0;
            }
            else if (optColumn.IsChecked == true)
            {
                StateDTO.FormulaID = 0;
                StateDTO.ColumnID = (int)cmbColumns.SelectedValue;
            }

            //foreach (TabItem item in tabMain.Items)
            //{
            //    var result = (item.Content as frmEntityStateCondition).UpdateMessage();
            //    if (!string.IsNullOrEmpty(result))
            //    {
            //        item.IsSelected = true;
            //        MessageBox.Show(result);
            //        return;
            //    }
            //    StateDTO.StateConditions.Add((item.Content as frmEntityStateCondition).Message);
            //}

            try
            {
                StateDTO.ID = bizEntityState.UpdateEntityStates(StateDTO);
                if (ItemSaved != null)
                    ItemSaved(this, new SavedItemArg() { ID = StateDTO.ID });
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

        //private void mnuRemoveItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        //{
        //    var menuItem = sender as RadMenuItem;
        //    var contextMenu = menuItem.Parent as RadContextMenu;
        //    var source = contextMenu.GetClickedElement<GridViewRow>();
        //    if (contextMenu != null && source != null)
        //    {
        //        StateDTO.ActionActivities.Remove(source.DataContext as UIActionActivityDTO);
        //    }
        //}

        //private void mnuAddNewActionActivity_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        //{
        //    //if (optPersist.IsChecked == false && optNotPersist.IsChecked == false)
        //    //{
        //    //    MessageBox.Show("لطفا یکی از حالات ذخیره و یا عدم ذخیره وضعیت را انتخاب نمایید");
        //    //    return;
        //    //}

        //}
        //private void View_ItemSaved(object sender, SavedItemArg e)
        //{
        //    SetActionActivities();
        //    StateDTO.ActionActivities.Add(new UIActionActivityDTO() { ID = e.ID });
        //}
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


        //private void mnuEditActionActivity_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        //{
        //    //if (optPersist.IsChecked == false && optNotPersist.IsChecked == false)
        //    //{
        //    //    MessageBox.Show("لطفا یکی از حالات ذخیره و یا عدم ذخیره وضعیت را انتخاب نمایید");
        //    //    return;
        //    //}
        //    var menuItem = sender as RadMenuItem;
        //    var contextMenu = menuItem.Parent as RadContextMenu;
        //    var source = contextMenu.GetClickedElement<GridViewRow>();
        //    if (contextMenu != null && source != null)
        //    {
        //        frmUIActionActivity view = new frmUIActionActivity((source.DataContext as UIActionActivityDTO).ID, EntityID);
        //        view.ItemSaved += View_ItemSavedEdit;
        //        MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف اقدامات", Enum_WindowSize.Big);
        //    }
        //}
        //private void View_ItemSavedEdit(object sender, SavedItemArg e)
        //{
        //    SetActionActivities();
        //}

        private void mnuAddNewItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            StateDTO.ActionActivities.Add(new UIActionActivityDTO());
        }

        private void optFormula_Checked(object sender, RoutedEventArgs e)
        {
            lblRelationshipTail.Visibility = Visibility.Collapsed;
            lokRelationshipTail.Visibility = Visibility.Collapsed;
            lblColumns.Visibility = Visibility.Collapsed;
            cmbColumns.Visibility = Visibility.Collapsed;
            lblFormula.Visibility = Visibility.Visible;
            lokFormula.Visibility = Visibility.Visible;
        }


        private void optColumn_Checked(object sender, RoutedEventArgs e)
        {
            lblRelationshipTail.Visibility = Visibility.Visible;
            lokRelationshipTail.Visibility = Visibility.Visible;
            lblColumns.Visibility = Visibility.Visible;
            cmbColumns.Visibility = Visibility.Visible;
            lblFormula.Visibility = Visibility.Collapsed;
            lokFormula.Visibility = Visibility.Collapsed;
        }

        //private void btnAddCondition_Click(object sender, RoutedEventArgs e)
        //{
        //    EntityStateConditionDTO condition = new EntityStateConditionDTO();
        //    AddConditin(condition);
        //}

        //private void AddConditin(EntityStateConditionDTO condition)
        //{

        //}

        //private void View_DeleteConditionRequest(object sender, EntityStateConditionDTO e)
        //{
        //    TabItem foundTab = null;
        //    foreach (TabItem item in tabMain.Items)
        //    {
        //        if ((item.Content as frmEntityStateCondition).Message == e)
        //        {
        //            foundTab = item;
        //        }
        //    }
        //    if (foundTab != null)
        //    {
        //        tabMain.Items.Remove(foundTab);
        //    }

        //}
    }

}
