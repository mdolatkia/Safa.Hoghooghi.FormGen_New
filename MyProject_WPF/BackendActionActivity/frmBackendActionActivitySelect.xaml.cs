using ModelEntites;
using MyFormulaFunctionStateFunctionLibrary;

using MyModelManager;
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
using Telerik.Windows.Controls.GridView;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityActionActivitys.xaml
    /// </summary>
    public partial class frmBackendActionActivitySelect : UserControl
    {
        //   EntityActionActivityDTO ValidationMessage { set; get; }
        //   BizEntityActionActivity bizEntityActionActivity = new BizEntityActionActivity();
        public event EventHandler<int> BackendActionActivitySelected;

        BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
        ObservableCollection<BackendActionActivityDTO> Message { set; get; }
        int EntityID { set; get; }
        //   string Catalog { set; get; }
        int EntityActionActivityID { set; get; }
        SelectorGrid SelectorGrid = null;

        public frmBackendActionActivitySelect(int entityID)
        {
            InitializeComponent();
            EntityID = entityID;
            // GetCatalog();
            //SetStepTypes();
            //SetActionActivities();

            var listColumns = new Dictionary<string, string>();
            listColumns.Add("Title", "اقدام مرتبط");
            listColumns.Add("Step", "مرحله مرتبط");


            SelectorGrid = ControlHelper.SetSelectorGrid(dtgActionActivities, listColumns);
            SelectorGrid.DataItemSelected += SelectorGrid_DataItemSelected;

            GetEntityActionActivities(EntityID);
        }
        private void SelectorGrid_DataItemSelected(object sender, object e)
        {
            CheckSelectedItem(e);
        }
        private void CheckSelectedItem(object item)
        {
            if (item != null)
            {
                var selected = dtgActionActivities.SelectedItem as BackendActionActivityDTO;
                if (selected != null)
                {
                    if (BackendActionActivitySelected != null)
                        BackendActionActivitySelected(this, selected.ID);
                }
            }
        }
        //private void GetCatalog()
        //{
        //    BizTableDrivedEntity bizTableDrivedActivity = new BizTableDrivedEntity();
        //    Catalog = bizTableDrivedActivity.GetCatalog(EntityID);
        //}

        //private void SetStepTypes()
        //{
        //    var rel = dtgActionActivities.Columns[1] as GridViewComboBoxColumn;
        //    rel.ItemsSource = Enum.GetValues(typeof(Enum_EntityActionActivityStep));
        //}

        //private void SetActionActivities()
        //{

        //    var list = bizActionActivity.GetActionActivities();
        //    var rel = dtgActionActivities.Columns[0] as GridViewComboBoxColumn;
        //    rel.ItemsSource = list;
        //    rel.DisplayMemberPath = "Title";
        //    rel.SelectedValueMemberPath = "ID";
        //}


        private void GetEntityActionActivities(int entityID)
        {
            Message = new ObservableCollection<BackendActionActivityDTO>();
            if (entityID == 0)
            {
                foreach (var item in bizActionActivity.GetAllActionActivities(MyProjectManager.GetMyProjectManager.GetRequester(), ""))
                    Message.Add(item);
            }
            else
            {
                foreach (var item in bizActionActivity.GetActionActivities(entityID, null, false, false))
                    Message.Add(item);
            }
            dtgActionActivities.ItemsSource = Message;
        }


        //private void ShowValidationMessage()
        //{
        //    txtMessage.Text = ValidationMessage.Message;
        //    chkReslutSensetive.IsChecked = ValidationMessage.ResultSensetive;
        //    if (ValidationMessage.FormulaID != 0)
        //    {
        //        GetFormulaName(ValidationMessage.FormulaID);
        //        txtFormulaValue.Text = ValidationMessage.Value;
        //        optFormula.IsChecked = true;
        //    }
        //    else if (ValidationMessage.CodeFunctionID != 0)
        //    {
        //        GetCodePath(ValidationMessage.CodeFunctionID);
        //        txtCodeValue.Text = ValidationMessage.Value;
        //        optCode.IsChecked = true;
        //    }
        //}


        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Message.Any(x => x.ActionActivityID == 0))
        //    {
        //        MessageBox.Show("برای یک یا چند مورد اقدام مرتبط انتخاب نشده است");
        //    }
        //    bizEntityActionActivity.UpdateEntityActionActivities(EntityID, Message.ToList());
        //    MessageBox.Show("اطلاعات ثبت شد");
        //}

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        //private void mnuAddNewActionActivity_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        //{
        //    //var menuItem = sender as RadMenuItem;
        //    //var contextMenu = menuItem.Parent as RadContextMenu;
        //    //var source = contextMenu.GetClickedElement<GridViewRow>();
        //    //if (contextMenu != null && source != null)
        //    //{//فرم جدا هم ساختاه شود
        //    frmBackendActionActivity view = new frmBackendActionActivity(0, EntityID);
        //    view.ItemSaved += View_ItemSaved;
        //    MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف اقدامات", Enum_WindowSize.Big);
        //    //}
        //}
        //private List<Enum_EntityActionActivityStep> GetAllowedActionActivitySteps()
        //{
        //    return new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.BeforeLoad,Enum_EntityActionActivityStep.BeforeSave,Enum_EntityActionActivityStep.AfterSave
        //    ,Enum_EntityActionActivityStep.BeforeDelete,Enum_EntityActionActivityStep.AfterDelete};
        //}

        //private List<Enum_ActionActivityType> GetAllowedActionActivityTypes()
        //{
        //    return new List<Enum_ActionActivityType>() { Enum_ActionActivityType.CodeFunction, Enum_ActionActivityType.DatabaseFunction };
        //}
        private void View_ItemSaved(object sender, SavedItemArg e)
        {
            GetEntityActionActivities(EntityID);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            CheckSelectedItem(dtgActionActivities.SelectedItem);
        }

        //private void mnuAddNewItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        //{
        //    Message.Add(new EntityActionActivityDTO());
        //}


        //private void mnuRemoveItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        //{
        //    var menuItem = sender as RadMenuItem;
        //    var contextMenu = menuItem.Parent as RadContextMenu;
        //    var source = contextMenu.GetClickedElement<GridViewRow>();
        //    if (contextMenu != null && source != null)
        //    {
        //        bizActionActivity.DeleteBackendActionActivity((source.DataContext as UIActionActivityDTO).ID);
        //    }
        //}

        //private void mnuEditActionActivity_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        //{
        //    var menuItem = sender as RadMenuItem;
        //    var contextMenu = menuItem.Parent as RadContextMenu;
        //    var source = contextMenu.GetClickedElement<GridViewRow>();
        //    if (contextMenu != null && source != null)
        //    {
        //        var actionActivity = (source.DataContext as BackendActionActivityDTO);
        //        if (actionActivity.Step != Enum_EntityActionActivityStep.None)

        //        {
        //            frmBackendActionActivity view = new frmBackendActionActivity((source.DataContext as BackendActionActivityDTO).ID, EntityID);
        //            view.ItemSaved += View_ItemSaved;
        //            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف اقدامات");
        //        }
        //        else
        //            MessageBox.Show("امکان اصلاح اقدام انتخاب شده وجود ندارد");
        //    }
        //}
    }

}
