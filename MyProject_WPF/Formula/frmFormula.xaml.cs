
using ModelEntites;

using MyFormulaFunctionStateFunctionLibrary;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using System.Collections;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmFormula.xaml
    /// </summary>
    public partial class frmFormula : UserControl
    {
        public event EventHandler<FormulaSelectedArg> FormulaUpdated;
        //FormulaIntention FormulaIntention { set; get; }
        BizCodeFunction bizCodeFunction = new BizCodeFunction();
        BizDatabaseFunction bizDatabaseFunction = new BizDatabaseFunction();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        //int EntityID { set; get; }
        FormulaDTO Formula { set; get; }
        //public frmFormula()
        //{
        //    InitializeComponent();
        //    //FormulaIntention = new FormulaIntention() { DefaultFormulaID = 3006, Type = Enum_FormulaIntention.FormulaForParameter, EntityID = 11 };
        //}
        public frmFormula(int formulaID, int entityID)
        {
            InitializeComponent();
            //FormulaIntention = formulaIntention;
            //EntityID = entityID;
          
            SetTypeCombo();
            treeParameters.ContextMenuOpening += TreeParameters_ContextMenuOpening;
            RadContextMenu.SetContextMenu(treeParameters, GenerateContextMenu());
            //cmbCustomType.ItemsSource = Enum.GetValues(typeof(ValueCustomType));
            SetLookups();
            if (formulaID != 0)
                GetFormula(formulaID);
            else
            {
                Formula = new FormulaDTO();
                Formula.EntityID = entityID;
                ShowFormula();
            }
            CheckTabVisibilities();
        }

        private void SetLookups()
        {
            lokCodeFunctionEntity.SelectionChanged += LokCodeFunctionEntity_SelectionChanged;
            lokCodeFunctionEntity.DisplayMember = "Title";
            lokCodeFunctionEntity.SelectedValueMember = "ID";
            lokCodeFunctionEntity.SearchFilterChanged += LokCodeFunctionEntity_SearchFilterChanged;

            lokCodeFunctionEntity.NewItemEnabled = true;
            lokCodeFunctionEntity.EditItemEnabled = true;
            lokCodeFunctionEntity.EditItemClicked += LokCodeFunctionEntity_EditItemClicked;

            lokCodeFunction.SelectionChanged += LokCodeFunction_SelectionChanged;
            lokCodeFunction.DisplayMember = "Name";
            lokCodeFunction.SelectedValueMember = "ID";
            lokCodeFunction.SearchFilterChanged += LokCodeFunction_SearchFilterChanged;

            lokCodeFunction.NewItemEnabled = true;
            lokCodeFunction.EditItemEnabled = true;
            lokCodeFunction.EditItemClicked += LokCodeFunction_EditItemClicked;



            lokDatabaseFunctionEntity.SelectionChanged += LokDatabaseFunctionEntity_SelectionChanged;
            lokDatabaseFunctionEntity.DisplayMember = "Title";
            lokDatabaseFunctionEntity.SelectedValueMember = "ID";
            lokDatabaseFunctionEntity.SearchFilterChanged += LokDatabaseFunctionEntity_SearchFilterChanged;

            lokDatabaseFunctionEntity.NewItemEnabled = true;
            lokDatabaseFunctionEntity.EditItemEnabled = true;
            lokDatabaseFunctionEntity.EditItemClicked += LokDatabaseFunctionEntity_EditItemClicked;


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
            if (lokEntity.SelectedItem == null)
            {

                optCodeFunctionEntity.Visibility = Visibility.Collapsed;
                tabCodeFunctionEntity.Visibility = Visibility.Collapsed;
                optDatabaseFunctionEntity.Visibility = Visibility.Collapsed;
                tabDatabaseFunctionEntity.Visibility = Visibility.Collapsed;
            }
            else
            {
                optCodeFunctionEntity.Visibility = Visibility.Visible;
                tabCodeFunctionEntity.Visibility = Visibility.Visible;
                optDatabaseFunctionEntity.Visibility = Visibility.Visible;
                tabDatabaseFunctionEntity.Visibility = Visibility.Visible;
            }
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
        private void LokCodeFunctionEntity_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var selectedItem = lokCodeFunctionEntity.SelectedItem as CodeFunction_EntityDTO;
            frmCodeFunction_Entity view = new frmCodeFunction_Entity((selectedItem == null ? 0 : selectedItem.ID), (int)lokEntity.SelectedValue);
            view.CodeFunctionEntityUpdated += View_CodeFunctionEntityUpdated;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف کد", Enum_WindowSize.Big);
        }

        private void View_CodeFunctionEntityUpdated(object sender, CodeFunctionEntitySelectedArg e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            lokCodeFunctionEntity.SelectedValue = e.CodeFunctionEntityID;
        }

        private void LokCodeFunctionEntity_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    var list = bizCodeFunction.GetCodeFunctionEntityByEntityID(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokEntity.SelectedValue);
                    e.ResultItemsSource = list;
                }
                else
                {
                    var id = Convert.ToInt32(e.SingleFilterValue);
                    if (id > 0)
                    {
                        //lokSecondSideEntity.ItemsSource = bizTableDrivedEntity.GetAllEntities();
                        var item = bizCodeFunction.GetCodeFunctionEntity(MyProjectManager.GetMyProjectManager.GetRequester(), id);
                        e.ResultItemsSource = new List<CodeFunction_EntityDTO> { item };
                    }
                    else
                        e.ResultItemsSource = null;
                }
            }
        }

        private void LokCodeFunctionEntity_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            if (lokCodeFunctionEntity.SelectedItem != null)
            {
                //Formula.FormulaItems.Clear();
                //Formula.FormulaItems = e.FormulaItems;
                var codeEntity = bizCodeFunction.GetCodeFunctionEntity(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokCodeFunctionEntity.SelectedValue);
                cmbTypes.SelectedItem = codeEntity.CodeFunction.RetrunDotNetType;
                List<FormulaItemDTO> list = new List<FormulaItemDTO>();
                foreach (var item in codeEntity.CodeFunctionEntityColumns.Where(x => x.ColumnID != 0))
                {

                    var rItem = new FormulaItemDTO();
                    rItem.ItemID = item.ColumnID;
                    rItem.ItemType = FormuaItemType.Column;
                    rItem.ItemTitle = item.ColumnName;
                    list.Add(rItem);
                }
                ShowTreeParameters(treeParameters.Items, list);
            }
        }


        private void LokCodeFunction_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var selectedItem = lokCodeFunction.SelectedItem as CodeFunctionDTO;
            frmCodeFunction view = new frmCodeFunction((selectedItem == null ? 0 : selectedItem.ID), Enum_CodeFunctionParamType.OneDataItem);
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
        private void LokCodeFunction_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            if (lokCodeFunction.SelectedItem != null)
            {
                //Formula.FormulaItems.Clear();
                //Formula.FormulaItems = e.FormulaItems;
                var codeEntity = lokCodeFunction.SelectedItem as CodeFunctionDTO;
                cmbTypes.SelectedItem = codeEntity.RetrunDotNetType;
            }
        }

        private void LokDatabaseFunctionEntity_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var selectedItem = lokDatabaseFunctionEntity.SelectedItem as DatabaseFunction_EntityDTO;
            frmDatabaseFunction_Entity view = new frmDatabaseFunction_Entity((selectedItem == null ? 0 : selectedItem.ID), (int)lokEntity.SelectedValue);
            view.DatabaseFunctionEntityUpdated += View_DatabaseFunctionEntityUpdated;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تعریف کد", Enum_WindowSize.Big);
        }

        private void View_DatabaseFunctionEntityUpdated(object sender, DatabaseFunctionEntitySelectedArg e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            lokDatabaseFunctionEntity.SelectedValue = e.DatabaseFunctionEntityID;
        }

        private void LokDatabaseFunctionEntity_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        {
            if (e.SingleFilterValue != null)
            {
                if (!e.FilterBySelectedValue)
                {
                    var list = bizDatabaseFunction.GetDatabaseFunctionEntityByEntityID(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokEntity.SelectedValue);
                    e.ResultItemsSource = list;
                }
                else
                {
                    var id = Convert.ToInt32(e.SingleFilterValue);
                    if (id > 0)
                    {
                        //lokSecondSideEntity.ItemsSource = bizTableDrivedEntity.GetAllEntities();
                        var item = bizDatabaseFunction.GetDatabaseFunctionEntity(MyProjectManager.GetMyProjectManager.GetRequester(), id);
                        e.ResultItemsSource = new List<DatabaseFunction_EntityDTO> { item };
                    }
                    else
                        e.ResultItemsSource = null;
                }
            }
        }

        private void LokDatabaseFunctionEntity_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            if (lokDatabaseFunctionEntity.SelectedItem != null)
            {
                //Formula.FormulaItems.Clear();
                //Formula.FormulaItems = e.FormulaItems;

                var dbEntity = bizDatabaseFunction.GetDatabaseFunctionEntity(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokDatabaseFunctionEntity.SelectedValue);
                cmbTypes.SelectedItem = dbEntity.DatabaseFunction.ReturnDotNetType;
                List<FormulaItemDTO> list = new List<FormulaItemDTO>();
                foreach (var item in dbEntity.DatabaseFunctionEntityColumns.Where(x => x.ColumnID != 0))
                {
                    var rItem = new FormulaItemDTO();
                    rItem.ItemID = item.ColumnID;
                    rItem.ItemType = FormuaItemType.Column;
                    rItem.ItemTitle = item.ColumnName;
                    list.Add(rItem);
                }
                ShowTreeParameters(treeParameters.Items, list);
            }
        }

        private void SetTypeCombo()
        {
            cmbTypes.ItemsSource = GetTypes();
        }

        private IEnumerable GetTypes()
        {
            return new List<Type>() {
            typeof(bool),
            typeof(string),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(ushort),
            typeof(short),
            typeof(uint),
            typeof(int),
            typeof(ulong),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
         };
        }

        //FormulaHelper formulaHelper = new FormulaHelper();
        BizFormula bizFormula = new BizFormula();
        BizColumn bizColumn = new BizColumn();

        private void GetFormula(int formulaID)
        {
            Formula = bizFormula.GetFormula(MyProjectManager.GetMyProjectManager.GetRequester(), formulaID, true);
            ShowFormula();
        }
        private void ShowFormula()
        {
            txtTitle.Text = Formula.Title;
            txtName.Text = Formula.Name;
            if (Formula.FormulaType == FormulaType.Linear)
            {
                optLinear.IsChecked = true;
                if (Formula.ID == 0)
                {
                    txtVersion.Text = "";
                    txtFormula.Text = "";
                }
                else
                {
                    var linearFormula = bizFormula.GetLinearFormula(MyProjectManager.GetMyProjectManager.GetRequester(), Formula.ID, true);
                    txtVersion.Text = linearFormula.Version.ToString();
                    txtFormula.Text = linearFormula.FormulaText;
                }
            }
            else if (Formula.FormulaType == FormulaType.CodeFunctionEntity)
            {
                optCodeFunctionEntity.IsChecked = true;
                lokCodeFunctionEntity.SelectedValue = Formula.CodeFunctionEntityID;
            }
            else if (Formula.FormulaType == FormulaType.DatabaseFunctionEntity)
            {
                optDatabaseFunctionEntity.IsChecked = true;
                lokDatabaseFunctionEntity.SelectedValue = Formula.DatabaseFunctionEntityID;
            }
            else if (Formula.FormulaType == FormulaType.CodeFunction)
            {
                optCodeFunction.IsChecked = true;
                lokCodeFunction.SelectedValue = Formula.CodeFunctionID;
            }
            lokEntity.SelectedValue = Formula.EntityID;

            //cmbCustomType.SelectedItem = Formula.ValueCustomType;
            cmbTypes.SelectedItem = Formula.ResultDotNetType;
            ShowTreeParameters(treeParameters.Items, Formula.FormulaItems);

            btnFormula.IsEnabled = !Formula.FormulaUsed;
        }
        private void ShowTreeParameters(ItemCollection items, List<FormulaItemDTO> allFormulaItems, List<FormulaItemDTO> formulaItems = null)
        {
            if (items == treeParameters.Items)
                treeParameters.Items.Clear();
            if (formulaItems == null)
                formulaItems = allFormulaItems.Where(x => string.IsNullOrEmpty(x.RelationshipIDTail)).ToList();
            foreach (var item in formulaItems)
            {
                RadTreeViewItem node = AddNode(items, item);
                if (item.ItemType == FormuaItemType.Relationship)
                {
                    var tail = (item.RelationshipIDTail ?? "");
                    if (tail != "")
                        tail += "," + item.ItemID;
                    else
                        tail = item.ItemID.ToString();
                    if (allFormulaItems.Any(x => x.RelationshipIDTail == tail))
                    {
                        var childs = allFormulaItems.Where(x => x.RelationshipIDTail == tail).ToList();
                        ShowTreeParameters(node.Items, allFormulaItems, childs);
                    }
                }
                node.IsExpanded = true;
            }
        }

        private RadTreeViewItem AddNode(ItemCollection items, FormulaItemDTO item)
        {
            RadTreeViewItem node = new RadTreeViewItem();
            node.Header = item.ItemTitle;
            node.DataContext = item;
            items.Add(node);
            return node;
        }

        RadMenuItem insertMenuItem;
        RadMenuItem removeMenuItem;
        public RadContextMenu GenerateContextMenu()
        {
            var contextMenu = new RadContextMenu();

            insertMenuItem = new RadMenuItem();
            insertMenuItem.Header = "افزودن";
            insertMenuItem.Click += InsertMenuItem_Click;
            contextMenu.Items.Add(insertMenuItem);

            removeMenuItem = new RadMenuItem();
            removeMenuItem.Header = "حذف";
            removeMenuItem.Click += RemoveMenuItem_Click;
            contextMenu.Items.Add(removeMenuItem);

            //RadContextMenu.SetContextMenu(gridView, contextMenu);
            return contextMenu;
        }
        private void TreeParameters_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

            var contextMenu = RadContextMenu.GetContextMenu(treeParameters);
            if (contextMenu == null)
                return;
            RadTreeViewItem item = e.Source as RadTreeViewItem;
            if (item != null)
            {
                contextMenu.Visibility = Visibility.Visible;
                removeMenuItem.Visibility = Visibility.Visible;
                var fItem = item.DataContext as FormulaItemDTO;
                if (fItem.ItemType != FormuaItemType.Relationship)
                {
                    insertMenuItem.Visibility = Visibility.Collapsed;
                }
                else
                    insertMenuItem.Visibility = Visibility.Visible;


            }
            else
            {
                contextMenu.Visibility = Visibility.Visible;
                insertMenuItem.Visibility = Visibility.Visible;
                removeMenuItem.Visibility = Visibility.Collapsed;
            }
        }
        private void RemoveMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var menu = sender as RadMenuItem;
            var contextMenu = menu.Parent as RadContextMenu;
            RadTreeViewItem item = contextMenu.GetClickedElement<RadTreeViewItem>();
            if (item != null)
            {
                if (item.ParentItem != null)
                    item.ParentItem.Items.Remove(item);
                else
                    treeParameters.Items.Remove(item);
            }
        }

        private void InsertMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var menu = sender as RadMenuItem;
            var contextMenu = menu.Parent as RadContextMenu;
            RadTreeViewItem item = contextMenu.GetClickedElement<RadTreeViewItem>();
            int entityID = 0;
            if (item != null)
            {
                var fItem = item.DataContext as FormulaItemDTO;
                if (fItem.ItemType == FormuaItemType.Relationship)
                {
                    BizRelationship bizRelationship = new BizRelationship();
                    var rel = bizRelationship.GetRelationship(fItem.ItemID);
                    entityID = rel.EntityID2;

                }
            }
            else
                entityID = (int)lokEntity.SelectedValue;
            if (entityID != 0)
            {
                frmPropertySelector frm = new frmPropertySelector(entityID);
                frm.PropertySelected += (sender1, e1) => Frm_PropertySelected(sender1, e1, item);
                MyProjectManager.GetMyProjectManager.ShowDialog(frm, "انتخاب خصوصیت", Enum_WindowSize.Vertical);
            }
        }

        private void Frm_PropertySelected(object sender, MyPropertyInfo e, RadTreeViewItem parentItem)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            FormulaItemDTO fItem = FormulaHelper.ToFormulaItem(e);
            AddNode(parentItem == null ? treeParameters.Items : parentItem.Items, fItem);
        }

        //ParametersForFormula ParametersForFormula { set; get; }
        private void btnFormula_Click(object sender, RoutedEventArgs e)
        {
            //BindableTypeDescriptor CustomType = null;
            //ParametersForFormula = formulaHelper.GetFormulaParameters(FormulaIntention.Type, FormulaIntention.TableID);
            frmNewFormulaDefinition view = new frmNewFormulaDefinition(txtFormula.Text, (int)lokEntity.SelectedValue);
            view.FormulaDefined += View_FormulaDefined;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form", Enum_WindowSize.Maximized);
        }
        private void View_FormulaDefined(object sender, FormulaDefinedArg e)
        {
            txtFormula.Text = e.Expression;
            cmbTypes.SelectedItem = e.ExpressionResultType;

            //Formula.FormulaItems.Clear();
            //Formula.FormulaItems = e.FormulaItems;
            ShowTreeParameters(treeParameters.Items, e.FormulaItems);


            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            //if (e.FormulaItems != null)
            //{
            //    Formula.FormulaItems.Clear();
            //    foreach (var item in e.FormulaItems)
            //    {
            //        Formula_FormulaParameterDTO param = new Formula_FormulaParameterDTO();
            //        if (item is ExistingFormulaParameter)
            //        {
            //            param.FormulaParameterID = (item as ExistingFormulaParameter).FormulaParameterID;
            //        }
            //        else if (item is ColumnFormulaParameter)
            //        {
            //            param.ColumnID = (item as ColumnFormulaParameter).ColumnID;
            //        }
            //        param.FormulaParameterPath = item.FormulaParameterFullPath;
            //        Formula.Parameters.Add(param);
            //    }

            //}
        }
        private void btnSaveAndSelect_Click(object sender, RoutedEventArgs e)
        {
            if (optLinear.IsChecked == true)
            {
                if (txtFormula.Text == "")
                {
                    MessageBox.Show("فرمول نامشخص است");
                    return;
                }
            }
            if (txtName.Text == "")
            {
                MessageBox.Show("نام نامشخص است");
                return;
            }
            if (txtTitle.Text == "")
            {
                MessageBox.Show("عنوان نامشخص است");
                return;
            }
            if (cmbTypes.SelectedItem == null)
            {
                MessageBox.Show("نوع نتیجه مشخص نشده است");
                return;
            }
            Formula.Name = txtName.Text;
            Formula.EntityID = (int)lokEntity.SelectedValue;
            Formula.Title = txtTitle.Text;
            Formula.ResultType = (cmbTypes.SelectedItem as Type).ToString();
            Formula.FormulaItems = GetFormulaItemsFromTree(treeParameters.Items);
            if (optCodeFunction.IsChecked == true)
            {
                Formula.FormulaType = FormulaType.CodeFunction;
                Formula.CodeFunctionID = (int)lokCodeFunction.SelectedValue;
            }
            else if (optCodeFunctionEntity.IsChecked == true)
            {
                Formula.FormulaType = FormulaType.CodeFunctionEntity;
                Formula.CodeFunctionEntityID = (int)lokCodeFunctionEntity.SelectedValue;

            }
            else if (optDatabaseFunctionEntity.IsChecked == true)
            {
                Formula.FormulaType = FormulaType.DatabaseFunctionEntity;
                Formula.DatabaseFunctionEntityID = (int)lokDatabaseFunctionEntity.SelectedValue;

            }
            else if (optLinear.IsChecked == true)
            {
                Formula.FormulaType = FormulaType.Linear;
            }

            Formula.ID = bizFormula.UpdateFormula(Formula, GetLinearFormulaDTO());
            MessageBox.Show("فرمول ثبت شد");
            //اگر برای ستون بود به ستون ارتباط میدهد
            //if (FormulaIntention.Type == Enum_FormulaIntention.FormulaForColumn)
            //{
            //bizColumn.UpdateCustomCalculation(FormulaIntention.ColumnID, Formula.ID);
            //}
            if (FormulaUpdated != null)
                FormulaUpdated(this, new FormulaSelectedArg() { FormulaID = Formula.ID });
            //MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private LinearFormulaDTO GetLinearFormulaDTO()
        {
            if (optLinear.IsChecked == true)
            {
                var result = new LinearFormulaDTO();
                result.FormulaText = txtFormula.Text;
                return result;
                // result.Version = (short)txtVersion.Text;
            }
            else
                return null;
        }

        private List<FormulaItemDTO> GetFormulaItemsFromTree(ItemCollection items, List<FormulaItemDTO> result = null)
        {
            if (result == null)
                result = new List<FormulaItemDTO>();
            foreach (RadTreeViewItem item in items)
            {
                var fItem = item.DataContext as FormulaItemDTO;
                result.Add(fItem);
                GetFormulaItemsFromTree(item.Items, result);
            }
            return result;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Formula = new FormulaDTO();
            ShowFormula();
        }
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
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            frmFormulaSelect view = new frmFormulaSelect(EntityID);
            view.FormulaSelected += View_FormulaSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form");
        }

        private void View_FormulaSelected(object sender, FormulaSelectedArg e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            if (e.FormulaID != 0)
            {
                GetFormula(e.FormulaID);
            }
        }

        private void optLinear_Checked(object sender, RoutedEventArgs e)
        {
            tabLinear.Visibility = Visibility.Collapsed;
            tabCodeFunctionEntity.Visibility = Visibility.Collapsed;
            tabCodeFunction.Visibility = Visibility.Collapsed;
            tabDatabaseFunctionEntity.Visibility = Visibility.Collapsed;

            if (optLinear.IsChecked == true)
            {
                tabLinear.Visibility = Visibility.Visible;
                tabLinear.IsSelected = true;
            }
            else if (optCodeFunctionEntity.IsChecked == true)
            {
                tabCodeFunctionEntity.Visibility = Visibility.Visible;
                tabCodeFunctionEntity.IsSelected = true;
            }
            else if (optCodeFunction.IsChecked == true)
            {
                tabCodeFunction.Visibility = Visibility.Visible;
                tabCodeFunction.IsSelected = true;
            }
            else if (optDatabaseFunctionEntity.IsChecked == true)
            {
                tabDatabaseFunctionEntity.Visibility = Visibility.Visible;
                tabDatabaseFunctionEntity.IsSelected = true;
            }
        }
    }







}
