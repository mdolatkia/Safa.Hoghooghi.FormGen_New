using ModelEntites;

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

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmActionActivitys.xaml
    /// </summary>
    public partial class frmBackendActionActivity : UserControl
    {
        BizCodeFunction bizCodeFunction = new BizCodeFunction();
        BackendActionActivityDTO Message { set; get; }
        BizDatabaseFunction bizDatabaseFunction = new BizDatabaseFunction();
        BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
        public event EventHandler<SavedItemArg> ItemSaved;
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        int ActionActivityID { set; get; }
        //string Catalog { set; get; }
        int EntityID
        {
            get
            {
                if (lokEntity.SelectedItem == null)
                    return 0;
                else
                    return (int)lokEntity.SelectedValue;
            }
        }
        //Enum_CodeFunctionParamType CodeFunctionParamType { set; get; }
        List<ColumnValueValidValuesDTO> ValidValues = new List<ColumnValueValidValuesDTO>();
        //bool VisualActions { set; get; }
        //  bool IncludeSteps { set; get; }
        //فرم جدا هم صدا زده شود 
        public frmBackendActionActivity(int actionActivityID, int entityID)
        {
            InitializeComponent();
            //EntityID = entityID;
            //Catalog = catalog;


            //     IncludeSteps = includeSteps;
            //   SetAllowedActivityTypes();
            ActionActivityID = actionActivityID;

            //CodeFunctionParamType = codeFunctionParamType;
            //if(EntityID==0)
            //{
            //    optRelationshipEnablity.Visibility = Visibility.Collapsed;
            //    tabRelationshipEnablity.Visibility = Visibility.Collapsed;
            //    optColumnValue.Visibility = Visibility.Collapsed;
            //    tabColumnValue.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            SetStepTypes();
            SetLookups();
            //if (ActionActivityTypes.Contains(Enum_ActionActivityType.CodeFunction))
            //    SetCodeFunctions();
            if (ActionActivityID == 0)
            {
                Message = new BackendActionActivityDTO();
                Message.EntityID = entityID;
                ShowMessage();
            }
            else
                GetActionActivity(ActionActivityID);

            CheckTabVisibilities();
        }

        private void SetLookups()
        {
            lokCodeFunction.DisplayMember = "Name";
            lokCodeFunction.SelectedValueMember = "ID";
            lokCodeFunction.SearchFilterChanged += LokCodeFunction_SearchFilterChanged;

            lokCodeFunction.NewItemEnabled = true;
            lokCodeFunction.EditItemEnabled = true;
            lokCodeFunction.EditItemClicked += LokCodeFunction_EditItemClicked;

            lokEntityDatabaseFunction.DisplayMember = "Name";
            lokEntityDatabaseFunction.SelectedValueMember = "ID";
            lokEntityDatabaseFunction.SearchFilterChanged += LokDBFunction_SearchFilterChanged;

            lokEntityDatabaseFunction.NewItemEnabled = true;
            lokEntityDatabaseFunction.EditItemEnabled = true;
            lokEntityDatabaseFunction.EditItemClicked += LokEntityDatabaseFunction_EditItemClicked;

            lokEntity.DisplayMember = "Name";
            lokEntity.SelectedValueMember = "ID";
            lokEntity.SearchFilterChanged += LokEntitiesFirst_SearchFilterChanged;
            lokEntity.SelectionChanged += LokEntity_SelectionChanged;
        }

        private void LokEntity_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            CheckTabVisibilities();
        }

        private void CheckTabVisibilities()
        {
            bool databaseFunctionVisiblity = true;
            if (cmbStep.SelectedItem != null)
            {
                if ((Enum_EntityActionActivityStep)cmbStep.SelectedItem == Enum_EntityActionActivityStep.BeforeLoad
                || (Enum_EntityActionActivityStep)cmbStep.SelectedItem == Enum_EntityActionActivityStep.BeforeSave
                  || (Enum_EntityActionActivityStep)cmbStep.SelectedItem == Enum_EntityActionActivityStep.BeforeDelete)
                {
                    databaseFunctionVisiblity = false;
                }
            }

            if (lokEntity.SelectedItem == null)
            {
                databaseFunctionVisiblity = false;
            }


            optDatabaseFunctionEntity.Visibility = databaseFunctionVisiblity ? Visibility.Visible : Visibility.Collapsed;
            tabDatabaseFunctionEntity.Visibility = databaseFunctionVisiblity ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LokEntitiesFirst_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    var list = bizTableDrivedEntity.GetAllEntities(MyProjectManager.GetMyProjectManager.GetRequester(), e.SingleFilterValue, false);
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

        private void LokEntityDatabaseFunction_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var selectedItem = lokEntityDatabaseFunction.SelectedItem as CodeFunctionDTO;
            var view = new frmDatabaseFunction_Entity((selectedItem == null ? 0 : selectedItem.ID), EntityID);
            view.DatabaseFunctionEntityUpdated += View_DatabaseFunctionEntityUpdated;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف ارتباط دیتابیس فانکشن و موجودیت", Enum_WindowSize.Big);
        }

        private void View_DatabaseFunctionEntityUpdated(object sender, DatabaseFunctionEntitySelectedArg e)
        {
            lokEntityDatabaseFunction.SelectedValue = e.DatabaseFunctionEntityID;
        }

        private void LokCodeFunction_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var codeFunctionParamType = GetCodeFunctionParamType();
            if (codeFunctionParamType != null)
            {
                var selectedItem = lokCodeFunction.SelectedItem as CodeFunctionDTO;
                frmCodeFunction view = new frmCodeFunction((selectedItem == null ? 0 : selectedItem.ID), codeFunctionParamType.Value);
                view.CodeFunctionUpdated += View_CodeFunctionUpdated;
                MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف کد", Enum_WindowSize.Big);
            }
            else
                MessageBox.Show("نوع کد مشخص نمیباشد");
        }

        private void View_CodeFunctionUpdated(object sender, DataItemSelectedArg e)
        {
            lokCodeFunction.SelectedValue = e.ID;
        }

        private void LokCodeFunction_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    List<Enum_CodeFunctionParamType> paramTypes = null;
                    var paramType = GetCodeFunctionParamType();
                    if (paramType != null)
                        paramTypes = new List<Enum_CodeFunctionParamType>() { paramType.Value };

                    var list = bizCodeFunction.GetAllCodeFunctions(MyProjectManager.GetMyProjectManager.GetRequester(), e.SingleFilterValue, paramTypes);
                    e.ResultItemsSource = list;
                }
                else
                {
                    var id = Convert.ToInt32(e.SingleFilterValue);
                    if (id > 0)
                    {
                        //lokSecondSideEntity.ItemsSource = bizTableDrivedEntity.GetAllEntities();
                        var item = bizCodeFunction.GetCodeFunction(MyProjectManager.GetMyProjectManager.GetRequester(), id);
                        e.ResultItemsSource = new List<CodeFunctionDTO> { item };
                    }
                    else
                        e.ResultItemsSource = null;
                }
            }
        }
        private void LokDBFunction_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    var list = bizDatabaseFunction.GetDatabaseFunctionEntities(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
                    list = list.Where(x => x.DatabaseFunction.Type == Enum_DatabaseFunctionType.StoredProcedure).ToList();
                    e.ResultItemsSource = list;
                }
                else
                {
                    var id = Convert.ToInt32(e.SingleFilterValue);
                    if (id > 0)
                    {
                        //lokSecondSideEntity.ItemsSource = bizTableDrivedEntity.GetAllEntities();
                        var item = bizDatabaseFunction.GetDatabaseFunction(MyProjectManager.GetMyProjectManager.GetRequester(), id);
                        e.ResultItemsSource = new List<DatabaseFunctionDTO> { item };
                    }
                    else
                        e.ResultItemsSource = null;
                }
            }
        }
        //private void SetAllowedActivityTypes()
        //{

        //}

        private void SetStepTypes()
        {
            //SetDatabaseFunctions();

            cmbStep.SelectionChanged += CmbStep_SelectionChanged;
            cmbStep.ItemsSource = Enum.GetValues(typeof(Enum_EntityActionActivityStep));
        }



        private void CmbStep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            CheckTabVisibilities();

        }


        private Enum_CodeFunctionParamType? GetCodeFunctionParamType()
        {

            if (cmbStep.SelectedItem != null)
            {

                var selectedItem = (Enum_EntityActionActivityStep)cmbStep.SelectedItem;
                if (selectedItem == Enum_EntityActionActivityStep.BeforeLoad)
                    return Enum_CodeFunctionParamType.ManyDataItems;
                else if (selectedItem == Enum_EntityActionActivityStep.BeforeSave ||
                        selectedItem == Enum_EntityActionActivityStep.AfterSave ||
                        selectedItem == Enum_EntityActionActivityStep.BeforeDelete ||
                        selectedItem == Enum_EntityActionActivityStep.AfterDelete)
                    return Enum_CodeFunctionParamType.OneDataItem;

            }
            return null;

        }


        //private void SetActionActivityType()
        //{
        //    cmbActionActivityType.ItemsSource = Enum.GetValues(typeof(Enum_ActionActivityType));
        //}

        //private void SetDatabaseFunctions()
        //{
        //    cmbDatabaseFunction.DisplayMemberPath = "FunctionName";
        //    cmbDatabaseFunction.SelectedValuePath = "ID";
        //    cmbDatabaseFunction.ItemsSource 
        //}

        private void GetActionActivity(int actionActivityID)
        {
            Message = bizActionActivity.GetBackendActionActivity(actionActivityID);
            ShowMessage();
        }

        private void ShowMessage()
        {
            txtTitle.Text = Message.Title;
            lokEntity.SelectedValue = Message.EntityID;
            //chkReslutSensetive.IsChecked = Message.ResultSensetive;
            cmbStep.SelectedItem = Message.Step;
            if (Message.Type == Enum_ActionActivityType.DatabaseFunction)
            {
                lokEntityDatabaseFunction.SelectedValue = Message.DatabaseFunctionEntityID;
                optDatabaseFunctionEntity.IsChecked = true;
            }
            if (Message.Type == Enum_ActionActivityType.CodeFunction)
            {
                lokCodeFunction.SelectedValue = Message.CodeFunctionID;
                optCode.IsChecked = true;
            }

        }
        private void optFormula_Checked(object sender, RoutedEventArgs e)
        {
            HideTabs();
            tabDatabaseFunctionEntity.Visibility = Visibility.Visible;
            tabDatabaseFunctionEntity.IsSelected = true;
        }

        private void HideTabs()
        {
            tabCode.Visibility = Visibility.Collapsed;
            tabDatabaseFunctionEntity.Visibility = Visibility.Collapsed;
        }


        //private void GetCodePath(int codeFunctionID)
        //{
        //    BizCodeFunction bizCodeFunction = new BizCodeFunction();
        //    var codeFunction = bizCodeFunction.GetCodeFunction(codeFunctionID);
        //    txtCodePath.Text = codeFunction.Path;
        //}

        private void optCode_Checked(object sender, RoutedEventArgs e)
        {
            HideTabs();
            tabCode.Visibility = Visibility.Visible;
            tabCode.IsSelected = true;
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //چک بشه که نوع کد و مورد انتخاب شده هماهنگی باشن



            if (txtTitle.Text == "")
            {
                MessageBox.Show("عنوان مناسب تعریف نشده است");
                return;
            }

            if (cmbStep.SelectedItem == null)
            {
                MessageBox.Show("مرحله مناسب تعریف نشده است");
                return;
            }

            if (optDatabaseFunctionEntity.IsChecked == false && optCode.IsChecked == false)
            {
                MessageBox.Show("یکی از حالات شروط را انتخاب نمایید");
                return;
            }
            if (optDatabaseFunctionEntity.IsChecked == true)
            {
                if (lokEntityDatabaseFunction.SelectedItem == null)
                {
                    MessageBox.Show("تابع مشخص نشده است");
                    return;
                }
            }
            else if (optCode.IsChecked == true)
            {
                if (lokCodeFunction.SelectedItem == null)
                {
                    MessageBox.Show("کد مشخص نشده است");
                    return;
                }
            }

            Message.EntityID = EntityID;
            Message.Title = txtTitle.Text;
            Message.Step = (Enum_EntityActionActivityStep)cmbStep.SelectedItem;
            //   Message.ResultSensetive = chkReslutSensetive.IsChecked == true;
            Message.DatabaseFunctionEntityID = 0;
            Message.CodeFunctionID = 0;

            if (optDatabaseFunctionEntity.IsChecked == true)
            {
                Message.DatabaseFunctionEntityID = (int)lokEntityDatabaseFunction.SelectedValue;
                Message.Type = Enum_ActionActivityType.DatabaseFunction;
            }
            else if (optCode.IsChecked == true)
            {
                Message.CodeFunctionID = (int)lokCodeFunction.SelectedValue;
                Message.Type = Enum_ActionActivityType.CodeFunction;
            }
            Message.ID = bizActionActivity.UpdateBackendActionActivitys(Message);
            if (ItemSaved != null)
                ItemSaved(this, new SavedItemArg() { ID = Message.ID });
            MessageBox.Show("اطلاعات ثبت شد");


        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new BackendActionActivityDTO();
            ShowMessage();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            frmBackendActionActivitySelect view = new MyProject_WPF.frmBackendActionActivitySelect(EntityID);
            view.BackendActionActivitySelected += View_BackendActionActivitySelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "", Enum_WindowSize.Big);

        }

        private void View_BackendActionActivitySelected(object sender, int e)
        {
            GetActionActivity(e);
        }

        //private void btnSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    frmActionActivitySelect view = new MyProject_WPF.frmActionActivitySelect(Catalog);
        //    view.ActionActivitySelected += View_ActionActivitySelected;
        //       MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form");
        //}

        //private void View_ActionActivitySelected(object sender, ActionActivitySelectedArg e)
        //{
        //    if (e.ActionActivityID != 0)
        //    {
        //        GetActionActivity(e.ActionActivityID);
        //    }
        //}

        //private void btnDatabaseFunctionEntity_Click(object sender, RoutedEventArgs e)
        //{
        //    if (cmbDatabaseFunction.SelectedItem != null)
        //    {
        //        var selected = cmbDatabaseFunction.SelectedItem as DatabaseFunctionDTO;
        //        var DatabaseFunctionEntityID = bizDatabaseFunction.GetDatabaseFunctionEntityID(EntityID, selected.ID);
        //        var DatabaseFunctionIntention = new DatabaseFunctionEntityIntention();
        //        DatabaseFunctionIntention.EntityID = EntityID;
        //        if (DatabaseFunctionEntityID == 0)
        //        {
        //            DatabaseFunctionIntention.DatabaseFunctionID = selected.ID;
        //            DatabaseFunctionIntention.Type = Enum_DatabaseFunctionEntityIntention.DatabaseFunctionEntityDefinition;
        //        }
        //        else
        //        {
        //            DatabaseFunctionIntention.DatabaseFunctionEntityID = DatabaseFunctionEntityID;
        //            DatabaseFunctionIntention.Type = Enum_DatabaseFunctionEntityIntention.DatabaseFunctionEntityEdit;
        //        }
        //        frmDatabaseFunction_Entity view = new frmDatabaseFunction_Entity(DatabaseFunctionIntention);
        //        MyProjectManager.GetMyProjectManager.ShowDialog(view, "DatabaseFunction_Entity");

        //    }
        //}




    }
    //public class SavedItemArg : EventArgs
    //{
    //    public int ID { set; get; }
    //}

}
