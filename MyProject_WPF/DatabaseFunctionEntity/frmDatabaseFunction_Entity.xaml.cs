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
using Telerik.Windows.Controls;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmDatabaseFunction.xaml
    /// </summary>
    public partial class frmDatabaseFunction_Entity : UserControl
    {
        public event EventHandler<DatabaseFunctionEntitySelectedArg> DatabaseFunctionEntityUpdated;

        BizDatabaseFunction bizDatabaseFunction = new BizDatabaseFunction();
        BizTableDrivedEntity bizEntity = new BizTableDrivedEntity();
        //  DatabaseFunctionEntityIntention DatabaseFunctionEntityIntention;
        DatabaseFunction_EntityDTO DatabaseFunctionEntity { set; get; }
        TableDrivedEntityDTO Entity { set; get; }
        public frmDatabaseFunction_Entity(int ID, int entityID)
        {
            InitializeComponent();
            //    DatabaseFunctionParamTypes = DatabaseFunctionParamTypes;
            Entity = bizEntity.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            SetLookups();
            if (ID == 0)
            {
                DatabaseFunctionEntity = new DatabaseFunction_EntityDTO();
                lokDatabaseFunction.ItemsSource = bizDatabaseFunction.GetDatabaseFunctionEntityByEntityID(MyProjectManager.GetMyProjectManager.GetRequester(), Entity.ID);
            }
            else
            {

                lokDatabaseFunction.Visibility = Visibility.Collapsed;
                GetDatabaseFunctionEntity(ID);

            }
            SetDataGridColumns();
        }
        private void GetDatabaseFunctionEntity(int iD)
        {
            DatabaseFunctionEntity = bizDatabaseFunction.GetDatabaseFunctionEntity(MyProjectManager.GetMyProjectManager.GetRequester(), iD);
            ShowDataItem();
        }

        private void ShowDataItem()
        {
            lokDatabaseFunction.SelectedValue = DatabaseFunctionEntity.DatabaseFunctionID;
            //SetDatabaseFunctionParametersToGrid(DatabaseFunctionEntity);
            txtTitle.Text = DatabaseFunctionEntity.Title;
            txtName.Text = DatabaseFunctionEntity.Name;
        }

        private void SetLookups()
        {
            lokDatabaseFunction.SelectionChanged += LokDatabaseFunction_SelectionChanged;
            lokDatabaseFunction.DisplayMember = "Name";
            lokDatabaseFunction.SelectedValueMember = "ID";
            lokDatabaseFunction.SearchFilterChanged += LokDatabaseFunction_SearchFilterChanged;
        }


        private void LokDatabaseFunction_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    Enum_DatabaseFunctionType type = Enum_DatabaseFunctionType.None;

                    var list = bizDatabaseFunction.GetAllDatabaseFunctions(MyProjectManager.GetMyProjectManager.GetRequester(), e.SingleFilterValue, type);
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
        private void LokDatabaseFunction_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            if (e.SelectedItem != null)
            {
                SetDatabaseFunctionParametersToGrid((int)lokDatabaseFunction.SelectedValue);
            }
            else
            {
                dtgDatabaseFunctionParams.ItemsSource = null;
            }
        }

        private void SetDatabaseFunctionParametersToGrid(int databaseFunctionID)
        {
            //DatabaseFunctionEntity = new DatabaseFunction_EntityDTO();
            //DatabaseFunctionEntity.DatabaseFunctionID = DatabaseFunctionID;
            //DatabaseFunctionEntity.EntityID = Entity.ID;
            //  SetDatabaseFunctionParametersToGrid(DatabaseFunctionEntity, DatabaseFunctionID);

            var functionColumns = bizDatabaseFunction.GetDatabaseFunctionParameters(databaseFunctionID);
            SetDatabaseFunctionParametersToGrid(functionColumns, DatabaseFunctionEntity);
        }
        private void SetDatabaseFunctionParametersToGrid(List<DatabaseFunctionColumnDTO> functionColumns, DatabaseFunction_EntityDTO DatabaseFunctionEntity)
        {
            dtgDatabaseFunctionParams.ItemsSource = null;
            List<DatabaseFunction_Entity_ColumnDTO> gridColumns = new List<DatabaseFunction_Entity_ColumnDTO>();
            foreach (var item in functionColumns.Where(x=>x.InputOutput==Enum_DatabaseFunctionParameterType.Input|| x.InputOutput == Enum_DatabaseFunctionParameterType.InputOutput))
            {
                var row = new DatabaseFunction_Entity_ColumnDTO();
                row.DatabaseFunctionParameterID = item.ID;
                row.FunctionColumnParamName = item.ParameterName;
                row.FunctionDataType = item.DataType;
                row.FunctionColumnDotNetType = item.DotNetType;
                var existingRow = DatabaseFunctionEntity.DatabaseFunctionEntityColumns.FirstOrDefault(x => x.FunctionColumnParamName == item.ParameterName);
                if (existingRow != null)
                {
                    row.ColumnID = existingRow.ColumnID;
                }
                gridColumns.Add(row);
            }

            dtgDatabaseFunctionParams.ItemsSource = gridColumns;
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
            if (DatabaseFunctionEntity == null)
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

            //if ((Enum_DatabaseFunctionParamType)cmbParamTypes.SelectedItem == Enum_DatabaseFunctionParamType.KeyColumns)
            //{
            if (DatabaseFunctionEntity.DatabaseFunctionEntityColumns.Any(x => x.ColumnID == 0))
            {
                MessageBox.Show("ستون معادل برای یکی از پارامترهای فانکشن مشخص نشده است");
                return;
            }
            //}

            DatabaseFunctionEntity.DatabaseFunctionEntityColumns = dtgDatabaseFunctionParams.ItemsSource as List<DatabaseFunction_Entity_ColumnDTO>;
            DatabaseFunctionEntity.EntityID = Entity.ID;
            DatabaseFunctionEntity.Title = txtTitle.Text;
            DatabaseFunctionEntity.Name = txtName.Text;
            DatabaseFunctionEntity.DatabaseFunctionID = (int)lokDatabaseFunction.SelectedValue;
            DatabaseFunctionEntity.ID = bizDatabaseFunction.UpdateDatabaseFunctionEntity(DatabaseFunctionEntity);
            MessageBox.Show("اطلاعات ثبت شد");
            if (DatabaseFunctionEntityUpdated != null)
                DatabaseFunctionEntityUpdated(this, new DatabaseFunctionEntitySelectedArg() { DatabaseFunctionEntityID = DatabaseFunctionEntity.ID });
            //    MyProjectManager.GetMyProjectManager().CloseDialog(this);
        }

        private void btnCodeList_Click(object sender, RoutedEventArgs e)
        {
            frmDatabaseFunction_EntitySelect view = new frmDatabaseFunction_EntitySelect(Entity.ID);
            view.DatabaseFunctionEntitySelected += View_DatabaseFunctionEntitySelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "انتخاب تابع");
        }

        private void View_DatabaseFunctionEntitySelected(object sender, DataItemSelectedArg e)
        {
            throw new NotImplementedException();
        }

        private void View_DatabaseFunctionSelected(object sender, DataItemSelectedArg e)
        {
            if (e.ID != 0)
            {
                GetDatabaseFunctionEntity(e.ID);
            }
        }
        //private void cmbParamTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((Enum_DatabaseFunctionParamType)cmbParamTypes.SelectedItem != Enum_DatabaseFunctionParamType.KeyColumns)
        //    {
        //        dtgDatabaseFunctionParams.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //        dtgDatabaseFunctionParams.Visibility = Visibility.Visible;
        //    //GetDatabaseFunctions();

        //}
    }

    public class DatabaseFunctionEntitySelectedArg : EventArgs
    {
        public int DatabaseFunctionEntityID { set; get; }
    }


    //public class DatabaseFunctionEntityIntention
    //{
    //    public int DatabaseFunctionEntityID { set; get; }
    //    public int EntityID { set; get; }
    //    public Enum_DatabaseFunctionEntityIntention Type { set; get; }
    //}

    //public enum Enum_DatabaseFunctionEntityIntention
    //{
    //    DatabaseFunctionEntityDefinition,
    //    DatabaseFunctionEntityEdit
    //}
}
