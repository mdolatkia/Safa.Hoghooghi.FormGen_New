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
using Telerik.Windows.Controls;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmCodeFunction.xaml
    /// </summary>
    public partial class frmCodeFunction_Entity : UserControl
    {
        public event EventHandler<CodeFunctionEntitySelectedArg> CodeFunctionEntityUpdated;

        BizCodeFunction bizCodeFunction = new BizCodeFunction();
        BizTableDrivedEntity bizEntity = new BizTableDrivedEntity();
        //  CodeFunctionEntityIntention CodeFunctionEntityIntention;
        CodeFunction_EntityDTO CodeFunctionEntity { set; get; }
        TableDrivedEntityDTO Entity { set; get; }
        public frmCodeFunction_Entity(int ID, int entityID)
        {
            InitializeComponent();
            //    CodeFunctionParamTypes = codeFunctionParamTypes;
            Entity = bizEntity.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            SetLookups();
            if (ID == 0)
            {
                CodeFunctionEntity = new CodeFunction_EntityDTO();
                //lokCodeFunction.ItemsSource = bizCodeFunction.GetCodeFunctionsByEntityID(MyProjectManager.GetMyProjectManager.GetRequester(), Entity.ID);
            }
            else
            {

                //lokCodeFunction.Visibility = Visibility.Collapsed;
                GetCodeFunctionEntity(ID);

            }
            SetDataGridColumns();
        }

        private void GetCodeFunctionEntity(int iD)
        {
            CodeFunctionEntity = bizCodeFunction.GetCodeFunctionEntity(MyProjectManager.GetMyProjectManager.GetRequester(), iD);
            ShowDataItem();
        }

        private void ShowDataItem()
        {
            lokCodeFunction.SelectedValue = CodeFunctionEntity.CodeFunctionID;
            //    SetCodeFunctionParametersToGrid(CodeFunctionEntity);
            txtTitle.Text = CodeFunctionEntity.Title;
            txtName.Text = CodeFunctionEntity.Name;
        }

        private void SetLookups()
        {
            lokCodeFunction.SelectionChanged += LokCodeFunction_SelectionChanged;
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
            frmCodeFunction view = new frmCodeFunction((selectedItem == null ? 0 : selectedItem.ID), Enum_CodeFunctionParamType.KeyColumns);
            view.CodeFunctionUpdated += View_CodeFunctionUpdated;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف کد", Enum_WindowSize.Big);
        }

        private void View_CodeFunctionUpdated(object sender, DataItemSelectedArg e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            lokCodeFunction.SelectedValue = e.ID;
        }

        private void LokCodeFunction_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    List<Enum_CodeFunctionParamType> paramTypes = null;
                    var paramType = Enum_CodeFunctionParamType.KeyColumns;
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
        private void LokCodeFunction_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            if (e.SelectedItem != null)
            {
                SetCodeFunctionParametersToGrid((int)lokCodeFunction.SelectedValue);
            }
            else
            {
                dtgCodeFunctionParams.ItemsSource = null;
            }
        }
        private void SetCodeFunctionParametersToGrid(int codeFunctionID)
        {
            //CodeFunctionEntity = new CodeFunction_EntityDTO();
            //CodeFunctionEntity.CodeFunctionID = codeFunctionID;
            //CodeFunctionEntity.EntityID = Entity.ID;
            //  SetCodeFunctionParametersToGrid(CodeFunctionEntity, codeFunctionID);

            var functionColumns = bizCodeFunction.GetCodeFunctionParameters(codeFunctionID);
            SetCodeFunctionParametersToGrid(functionColumns, CodeFunctionEntity);
        }
        //private void SetCodeFunctionParametersToGrid(CodeFunction_EntityDTO codeFunctionEntity,int codeFunctionID)
        //{

        //}

        private void SetCodeFunctionParametersToGrid(List<CodeFunctionColumnDTO> functionColumns, CodeFunction_EntityDTO codeFunctionEntity)
        {
            dtgCodeFunctionParams.ItemsSource = null;
            List<CodeFunction_Entity_ColumnDTO> gridColumns = new List<CodeFunction_Entity_ColumnDTO>();
            foreach (var item in functionColumns)
            {
                var row = new CodeFunction_Entity_ColumnDTO();
                row.CodeFunctionParameterID = item.ID;
                row.FunctionColumnParamName = item.ParameterName;
                row.FunctionDataType = item.DataType;
                row.FunctionColumnDotNetType = item.DotNetType;
                var existingRow = codeFunctionEntity.CodeFunctionEntityColumns.FirstOrDefault(x => x.FunctionColumnParamName == item.ParameterName);
                if (existingRow != null)
                {
                    row.ColumnID = existingRow.ColumnID;
                }
                gridColumns.Add(row);

            }


            dtgCodeFunctionParams.ItemsSource = gridColumns;



        }

        private void SetDataGridColumns()
        {
            colEntityColumn.ItemsSource = Entity.Columns;

            colEntityColumn.DisplayMemberPath = "Name";
            colEntityColumn.SelectedValueMemberPath = "ID";
        }





        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("نام نامشخص است");
                return;
            }
            if (CodeFunctionEntity == null)
            {
                MessageBox.Show("فانکشن نامشخص است");
                return;
            }
            //if (cmbParamTypes.SelectedItem == null)
            //{
            //    MessageBox.Show("نوع نامشخص است");
            //    return;
            //}
            //if (txtTitle.Text == "")
            //{
            //    MessageBox.Show("عنوان فانکشن نامشخص است");
            //    return;
            //}

            //if ((Enum_CodeFunctionParamType)cmbParamTypes.SelectedItem == Enum_CodeFunctionParamType.KeyColumns)
            //{
            if (CodeFunctionEntity.CodeFunctionEntityColumns.Any(x => x.ColumnID == 0))
            {
                MessageBox.Show("ستون معادل برای یکی از پارامترهای فانکشن مشخص نشده است");
                return;
            }
            //}


            CodeFunctionEntity.CodeFunctionEntityColumns = dtgCodeFunctionParams.ItemsSource as List<CodeFunction_Entity_ColumnDTO>;
            CodeFunctionEntity.EntityID = Entity.ID;
            CodeFunctionEntity.Title = txtTitle.Text;
            CodeFunctionEntity.Name = txtName.Text;
            CodeFunctionEntity.CodeFunctionID = (int)lokCodeFunction.SelectedValue;
            CodeFunctionEntity.ID = bizCodeFunction.UpdateCodeFunctionEntity(CodeFunctionEntity);
            MessageBox.Show("اطلاعات ثبت شد");
            if (CodeFunctionEntityUpdated != null)
                CodeFunctionEntityUpdated(this, new CodeFunctionEntitySelectedArg() { CodeFunctionEntityID = CodeFunctionEntity.ID });
            //    MyProjectManager.GetMyProjectManager().CloseDialog(this);
        }

        private void btnCodeList_Click(object sender, RoutedEventArgs e)
        {
            frmCodeFunction_EntitySelect view = new frmCodeFunction_EntitySelect(Entity.ID);
            view.CodeFunctionEntitySelected += View_CodeFunctionSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "انتخاب تابع");
        }
        private void View_CodeFunctionSelected(object sender, DataItemSelectedArg e)
        {
            if (e.ID != 0)
            {
                GetCodeFunctionEntity(e.ID);
            }
        }
        //private void cmbParamTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((Enum_CodeFunctionParamType)cmbParamTypes.SelectedItem != Enum_CodeFunctionParamType.KeyColumns)
        //    {
        //        dtgCodeFunctionParams.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //        dtgCodeFunctionParams.Visibility = Visibility.Visible;
        //    //GetCodeFunctions();

        //}
    }

    public class CodeFunctionEntitySelectedArg : EventArgs
    {
        public int CodeFunctionEntityID { set; get; }
    }


    //public class CodeFunctionEntityIntention
    //{
    //    public int CodeFunctionEntityID { set; get; }
    //    public int EntityID { set; get; }
    //    public Enum_CodeFunctionEntityIntention Type { set; get; }
    //}

    public enum Enum_CodeFunctionEntityIntention
    {
        CodeFunctionEntityDefinition,
        CodeFunctionEntityEdit
    }
}
