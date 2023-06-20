using ModelEntites;



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
    /// Interaction logic for frmEntityCommands.xaml
    /// </summary>
    public partial class frmEntityCommand : UserControl
    {
        BizCodeFunction bizCodeFunction = new BizCodeFunction();
           EntityCommandDTO CommandMessage { set; get; }
        BizEntityCommand bizEntityCommand = new BizEntityCommand();
        //   int EntityID { set; get; }
        //string Catalog { set; get; }
        //int EntityCommandID { set; get; }
        public frmEntityCommand(int entityID, int entityCommandID)
        {
            // frmEntityCommand: f7d70c177b8c
            InitializeComponent();
            //    EntityID = entityID;
            // EntityCommandID = entityCommandID;
            //GetCatalog();
            SetEnumType();
            SetLookups();
            //SetFromulas();
            if (entityCommandID == 0)
                CommandMessage = new EntityCommandDTO();
            else
                GetEntityCommand(entityCommandID);
            dtgEntities.ItemsSource = CommandMessage.Entities;
            ControlHelper.GenerateContextMenu(dtgEntities);
            SetEntities();
        }
        private void SetLookups()
        {
            lokCodeFunction.DisplayMember = "Name";
            lokCodeFunction.SelectedValueMember = "ID";
            lokCodeFunction.SearchFilterChanged += LokCodeFunction_SearchFilterChanged;

            lokCodeFunction.NewItemEnabled = true;
            lokCodeFunction.EditItemEnabled = true;
            lokCodeFunction.EditItemClicked += LokCodeFunction_EditItemClicked;

        }

        private void LokCodeFunction_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var selectedItem = lokCodeFunction.SelectedItem as CodeFunctionDTO;
            frmCodeFunction view = new frmCodeFunction((selectedItem == null ? 0 : selectedItem.ID), Enum_CodeFunctionParamType.OneDataItem);
            view.CodeFunctionUpdated += View_CodeFunctionUpdated;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف کد",Enum_WindowSize.Big);
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
                    var paramType = Enum_CodeFunctionParamType.OneDataItem;
                    paramTypes = new List<Enum_CodeFunctionParamType>() { paramType };

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
        private void SetEnumType()
        {
            cmbType.ItemsSource = Enum.GetValues(typeof(EntityCommandType));
        }

        //private void GetCatalog()
        //{
        //    BizTableDrivedEntity bizTableDrivedActivity = new BizTableDrivedEntity();
        //    Catalog = bizTableDrivedActivity.GetCatalog(EntityID);
        //}
        //private void SetFromulas()
        //{
        //    cmbFormula.DisplayMemberPath = "Name";
        //    cmbFormula.SelectedValuePath = "ID";
        //    BizFormula bizFormula = new BizFormula();
        //    cmbFormula.ItemsSource = bizFormula.GetFormulas(EntityID);
        //}
        private void SetEntities()
        {
            colEntity.DisplayMemberPath = "Alias";
            colEntity.SelectedValueMemberPath = "ID";
            colEntity.SearchFilterChanged += ColUser_SearchFilterChanged;
        }
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        private void ColUser_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (!string.IsNullOrEmpty(e.SingleFilterValue))
            {
                if (e.FilterBySelectedValue)
                {
                    var id = Convert.ToInt32(e.SingleFilterValue);
                    if (id > 0)
                    {
                        var entity = bizTableDrivedEntity.GetSimpleEntity(MyProjectManager.GetMyProjectManager.GetRequester(), id); ;
                        e.ResultItemsSource = new List<TableDrivedEntityDTO> { entity };
                    }
                    else
                        e.ResultItemsSource = null;
                }
                else
                {
                    e.ResultItemsSource = bizTableDrivedEntity.GetAllEnabledEntitiesExceptViewsDTO(MyProjectManager.GetMyProjectManager.GetRequester(), e.SingleFilterValue);
                }
            }
            //else if (e.Filters.Count > 0)
            //{

            //}
        }

        private void GetEntityCommand(int EntityCommandID)
        {
            CommandMessage = bizEntityCommand.GetEntityCommand(MyProjectManager.GetMyProjectManager.GetRequester(), EntityCommandID, false);
            ShowCommandMessage();
        }

        private void ShowCommandMessage()
        {
            txtTitle.Text = CommandMessage.Title;
            //if (CommandMessage.FormulaID != 0)
            //{
            //    cmbFormula.SelectedValue = CommandMessage.FormulaID;
            //    optFormula.IsChecked = true;
            //}
            if (CommandMessage.CodeFunctionID != 0)
            {
                lokCodeFunction.SelectedValue = CommandMessage.CodeFunctionID;
            }
            cmbType.SelectedItem = CommandMessage.Type;
            dtgEntities.ItemsSource = CommandMessage.Entities;

        }
        //private void optFormula_Checked(object sender, RoutedEventArgs e)
        //{
        //    tabCode.Visibility = Visibility.Collapsed;
        //    tabFormula.Visibility = Visibility.Visible;
        //    tabFormula.IsSelected = true;
        //}


        //private void GetCodePath(int codeFunctionID)
        //{
        //    BizCodeFunction bizCodeFunction = new BizCodeFunction();
        //    var codeFunction = bizCodeFunction.GetCodeFunction(codeFunctionID);
        //    txtCodePath.Text = codeFunction.Path;
        //}

        //private void optCode_Checked(object sender, RoutedEventArgs e)
        //{
        //    tabFormula.Visibility = Visibility.Collapsed;
        //    tabCode.Visibility = Visibility.Visible;
        //    tabCode.IsSelected = true;
        //}

        //private void btnAddFormula_Click(object sender, RoutedEventArgs e)
        //{
        //    FormulaIntention formulaIntention = new FormulaIntention();
        //    formulaIntention.EntityID = EntityID;
        //    formulaIntention.Type = Enum_FormulaIntention.FormulaForParameter;
        //    frmFormula view = new frmFormula(formulaIntention);
        //    view.FormulaSelected += View_FormulaSelected;
        //       MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form");
        //}

        //private void View_FormulaSelected(object sender, FormulaSelectedArg e)
        //{
        //    SetFromulas();
        //    cmbFormula.SelectedValue = e.FormulaID;

        //}

        //private void GetFormulaName(int formulaID)
        //{
        //    BizFormula bizFormula = new BizFormula();
        //    var formula = bizFormula.GetFormula(formulaID);
        //    txtFormulaName.Text = formula.Name;

        //}
        //private void btnAddCodeFunction_Click(object sender, RoutedEventArgs e)
        //{
        //    var selectedItem = cmbCodeFunction.SelectedItem as CodeFunctionDTO;
        //    frmCodeFunction view = new frmCodeFunction((selectedItem == null ? 0 : selectedItem.ID), Enum_CodeFunctionParamType.OneDataItem);
        //    view.CodeFunctionUpdated += View_CodeSelected;
        //    MyProjectManager.GetMyProjectManager.ShowDialog(view, "تنظیمات نامه");
        //}


        //private void View_CodeSelected(object sender, DataItemSelectedArg e)
        //{
        //    SetCodeFunctions();
        //    cmbCodeFunction.SelectedValue = e.ID;
        //}

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtTitle.Text == "")
            {
                MessageBox.Show("عنوان مناسب تعریف نشده است");
                return;
            }
            if (lokCodeFunction.SelectedItem == null)
            {
                MessageBox.Show("کد مشخص نشده است");
                return;
            }

            //if (txtMessage.Text == "")
            //{
            //    MessageBox.Show("پیغام مناسب تعریف نشده است");
            //    return;
            //}
            //if (optFormula.IsChecked == false && optCode.IsChecked == false)
            //{
            //    MessageBox.Show("یکی از حالات فرمول و یا کد را انتخاب نمایید");
            //    return;
            //}
            //if (optFormula.IsChecked == true)
            //{
            //    if (cmbFormula.SelectedItem == null)
            //    {
            //        MessageBox.Show("فرمول مشخص نشده است");
            //        return;
            //    }
            //}
            //else if (optCode.IsChecked == true)
            //{
            //   
            //}

            //   CommandMessage.TableDrivedEntityID = EntityID;

            CommandMessage.Title = txtTitle.Text;
            CommandMessage.Type = (EntityCommandType)cmbType.SelectedItem;


            CommandMessage.CodeFunctionID = (int)lokCodeFunction.SelectedValue;
            //CommandMessage.FormulaID = (int)cmbFormula.SelectedValue;
            //CommandMessage.Value = txtFormulaValue.Text;

            //}
            //else if (optCode.IsChecked == true)
            //{
            //    CommandMessage.FormulaID = 0;
            //    CommandMessage.CodeFunctionID = (int)cmbCodeFunction.SelectedValue;
            //    CommandMessage.Value = txtCodeValue.Text;
            //}
            bizEntityCommand.UpdateEntityCommands(CommandMessage);
            MessageBox.Show("اطلاعات ثبت شد");
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            CommandMessage = new EntityCommandDTO();
            ShowCommandMessage();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            frmEntityCommandSelect view = new MyProject_WPF.frmEntityCommandSelect();
            view.EntityCommandSelected += View_EntityCommandSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "frmEntityCommandSelect");
        }

        private void View_EntityCommandSelected(object sender, EntityCommandSelectedArg e)
        {
            if (e.EntityCommandID != 0)
            {
                GetEntityCommand(e.EntityCommandID);
            }
        }


    }

}
