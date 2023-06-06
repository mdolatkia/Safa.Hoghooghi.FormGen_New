using ModelEntites;
using MyCommonWPFControls;

using MyModelManager;

using ProxyLibrary;
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
    /// Interaction logic for frmEntityRoleSecurity.xaml
    /// </summary>
    public partial class frmEntitySecurityDirect : UserControl
    {
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();

        BizColumn bizColumn = new BizColumn();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizDatabaseFunction bizDatabaseFunction = new BizDatabaseFunction();
        BizEntityState bizEntityState = new BizEntityState();
        EntitySecurityDirectDTO Message;
        BizSecuritySubject bizSecuritySubject = new BizSecuritySubject();
        BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();
        int EntitySecurityDirectID { set; get; }
        public int EntityID
        {
            get
            {
                if (lokEntities.SelectedItem != null)
                {
                    return (int)lokEntities.SelectedValue;
                }
                else
                    return 0;
            }
        }

        public frmEntitySecurityDirect(int entityID)
        {
            InitializeComponent();
            //   cmbInOrNotIn.ItemsSource = Enum.GetValues(typeof(InORNotIn)).Cast<InORNotIn>();

            SetEntites();
            //  SetSecuritySubjects();
            //  SetAndOrType();
            //     SetDatabaseFunctions();
            //  SetOperators();
            //  SetReservedValues();
            SetMode();
            cmbMode.SelectionChanged += CmbMode_SelectionChanged;
            //lokFormula.EditItemEnabled = true;
            //lokFormula.NewItemEnabled = true;
            //lokFormula.EditItemClicked += LokFormula_EditItemClicked;

            //   dtgConditions.RowLoaded += DtgConditions_RowLoaded;
            //   dtgConditions.CellEditEnded += DtgConditions_CellEditEnded;
            lokState.EditItemClicked += colEntityState_EditItemClicked;
            lokState.EditItemEnabled = true;
            lokState.NewItemEnabled = true;
            //   ControlHelper.GenerateContextMenu(dtgStates);
            //  ControlHelper.GenerateContextMenu(dtgSecuritySubjects);
            if (entityID != 0)
            {
                Message = new EntitySecurityDirectDTO();
                Message.TableDrivedEntityID = entityID;
                ShowMessage();

            }




            //ControlHelper.GenerateContextMenu(dtgColumnValue);
            //ControlHelper.GenerateContextMenu(dtgFormulaValue);
            //lokRelationshipTail.SelectionChanged += LokRelationshipTail_SelectionChanged;
            //lokRelationshipTail.EditItemClicked += LokRelationshipTail_EditItemClicked;
            //cmbOperator.ItemsSource = Enum.GetValues(typeof(Enum_EntityStateOperator)).Cast<Enum_EntityStateOperator>();
            //colReservedValue.ItemsSource = Enum.GetValues(typeof(SecurityReservedValue));


        }
        //private void LokRelationshipTail_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        //{
        //    if (EntityID != 0)
        //    {
        //        frmEntityRelationshipTail frm = null;
        //        frm = new frmEntityRelationshipTail(EntityID);
        //        MyProjectManager.GetMyProjectManager.ShowDialog(frm, "رابطه های مرتبط");
        //        frm.ItemSelected += (sender1, e1) => Frm_TailSelected(sender1, e1, (sender as MyStaticLookup));
        //    }
        //}
        //private void Frm_TailSelected(object sender1, EntityRelationshipTailSelectedArg e1, MyStaticLookup myStaticLookup)
        //{
        //    SetRelationshipTails();
        //    myStaticLookup.SelectedValue = e1.EntityRelationshipTailID;
        //}
        //private void LokRelationshipTail_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        //{
        //    SetColumns();
        //}
        //private void SetColumns()
        //{
        //    BizColumn bizColumn = new BizColumn();
        //    BizTableDrivedEntity biz = new BizTableDrivedEntity();
        //    var entityID = 0;
        //    //if (lokRelationshipTail.SelectedItem == null)
        //    //    entityID = EntityID;
        //    //else
        //    //{
        //    //    EntityRelationshipTailDTO item = lokRelationshipTail.SelectedItem as EntityRelationshipTailDTO;
        //    //    entityID = item.TargetEntityID;
        //    //}
        //    var entity = biz.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
        //    var columns = entity.Columns;  //  .Where(x => x.ForeignKey == false).ToList();
        //    //  برای وضعیتهایی که به دسترسی داده وصل میشن همه ستونها لازمند چون مثلا برای درخواست سرویس شناسه دفتر با شناسه خاری سازمان کاربر چک میشود. اما برای وضعیتهای فرم کلید خارجی ها کنترل نمی شوند که باعث فعال شدن اقدامات بشوند. چون داینامیک تغییر نمی کنند. البته بعهتر است برنامه تغییر کند که کلید خارجی ها با تغییر رابطه تغییر کنند.

        //    cmbColumns.DisplayMemberPath = "Alias";
        //    cmbColumns.SelectedValuePath = "ID";
        //    cmbColumns.ItemsSource = columns;
        //    if (Message != null && Message.ID != 0)
        //    {
        //        if (Message.ColumnID != 0)
        //            cmbColumns.SelectedValue = Message.ColumnID;
        //    }
        //}
        //private void SetRelationshipTails()
        //{
        //    var list = bizEntityRelationshipTail.GetEntityRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
        //    var tails = list.Where(x => x.IsOneToManyTail == false).ToList();
        //    lokRelationshipTail.DisplayMember = "EntityPath";
        //    lokRelationshipTail.SelectedValueMember = "ID";
        //    lokRelationshipTail.ItemsSource = tails;
        //}
        private void CmbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMode.SelectedItem != null)
            {
                GetDirectSecurity();
                if ((DataDirectSecurityMode)cmbMode.SelectedItem == DataDirectSecurityMode.FetchData)
                {
                    //lblIgnoreSecurity.Visibility = Visibility.Visible;
                    //chkIgnoreSecurity.Visibility = Visibility.Visible;

                    //optFormula.Visibility = Visibility.Collapsed;
                    //tabFormula.Visibility = Visibility.Collapsed;
                    //optColumn.IsChecked = true;
                    //tabColumn.IsSelected = true;

                }
                else
                {
                    //lblIgnoreSecurity.Visibility = Visibility.Collapsed;
                    //chkIgnoreSecurity.Visibility = Visibility.Collapsed;
                    //optFormula.Visibility = Visibility.Visible;
                    //tabFormula.Visibility = Visibility.Visible;
                }
                //SetEnityStates();
            }
        }

        private void SetMode()
        {
            cmbMode.ItemsSource = Enum.GetValues(typeof(DataDirectSecurityMode));
        }

        private void colEntityState_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            if (lokEntities.SelectedItem != null && cmbMode.SelectedItem != null)
            {
                int id = 0;
                if ((sender as MyStaticLookup).SelectedItem != null)
                {
                    var item = (sender as MyStaticLookup).SelectedItem as EntityStateDTO;
                    id = item.ID;
                }

                var mode = (DataDirectSecurityMode)cmbMode.SelectedItem;


                frmEntityStates frm = new frmEntityStates((int)lokEntities.SelectedValue, id);
                frm.ItemSaved += (sender1, e1) => Frm_EntityStateSelected(sender1, e1, (sender as MyStaticLookup));
                MyProjectManager.GetMyProjectManager.ShowDialog(frm, "وضعیتها", Enum_WindowSize.Maximized);
            }

        }

        //private void LokFormula_EditItemClicked(object sender, EditItemClickEventArg e)
        //{
        //    int formulaID = 0;
        //    if (lokFormula.SelectedItem != null)
        //        formulaID = (int)lokFormula.SelectedValue;
        //    frmFormula view = new frmFormula(formulaID, EntityID);
        //    view.FormulaUpdated += View_FormulaSelected;
        //    MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form", Enum_WindowSize.Maximized);
        //}

        //private void View_FormulaSelected(object sender, FormulaSelectedArg e)
        //{
        //    SetFromulas();
        //    lokFormula.SelectedValue = e.FormulaID;
        //}
        //private void SetFromulas()
        //{
        //    lokFormula.DisplayMember = "Name";
        //    lokFormula.SelectedValueMember = "ID";
        //    BizFormula bizFormula = new BizFormula();
        //    lokFormula.ItemsSource = bizFormula.GetFormulas(EntityID,false);
        //}

        private void Frm_EntityStateSelected(object sender1, SavedItemArg e1, MyStaticLookup myStaticLookup)
        {
            SetEnityStates();
            myStaticLookup.SelectedValue = e1.ID;
        }

        private void SetEntites()
        {
            lokEntities.DisplayMember = "Name";
            lokEntities.SelectedValueMember = "ID";
            lokEntities.SearchFilterChanged += LokEntities_SearchFilterChanged;
            lokEntities.SelectionChanged += LokEntities_SelectionChanged;
        }

        private void LokEntities_SelectionChanged(object sender, MyCommonWPFControls.SelectionChangedArg e)
        {
            SetEnityStates();
            if (lokEntities.SelectedItem != null)
            {
                //SetRelationshipTails();
                //SetColumns();
                //SetFromulas();

                GetDirectSecurity();
            }
            else
            {
                lokState.ItemsSource = null;
                txtDescription.Text = "";
                //cmbColumns.ItemsSource = null;
                //lokRelationshipTail.ItemsSource = null;
                //lokFormula.ItemsSource = null;
            }
        }

        private void GetDirectSecurity()
        {
            if (lokEntities.SelectedItem != null && cmbMode.SelectedItem != null)
            {

                Message = bizRoleSecurity.GetEntitySecurityDirectByEntityID(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokEntities.SelectedValue,
                    (DataDirectSecurityMode)cmbMode.SelectedItem, false);
                if (Message == null)
                {
                    Message = new EntitySecurityDirectDTO();
                    Message.TableDrivedEntityID = (int)lokEntities.SelectedValue;
                    Message.Mode = (DataDirectSecurityMode)cmbMode.SelectedItem;
                }
                ShowMessage();
            }
        }

        private void LokEntities_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
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
                        //  lokEntities.ItemsSource = bizTableDrivedEntity.GetAllEntities();
                        var entity = bizTableDrivedEntity.GetSimpleEntity(MyProjectManager.GetMyProjectManager.GetRequester(), id); ;
                        e.ResultItemsSource = new List<TableDrivedEntityDTO> { entity };
                    }
                    else
                        e.ResultItemsSource = null;
                }
            }
        }

        //private void SetReservedValues()
        //{
        //    colReservedValue.ItemsSource = Enum.GetValues(typeof(SecurityReservedValue));
        //}

        //private void DtgConditions_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        //{
        //    if (e.Cell.Column == colRelationshipTail)
        //    {
        //        if (e.Cell.DataContext is EntitySecurityConditionDTO)
        //        {
        //            var condition = (e.Cell.DataContext as EntitySecurityConditionDTO);
        //            SetConditionColumns(condition);
        //        }
        //    }
        //}

        //private void DtgConditions_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        //{
        //    if (e.DataElement is EntitySecurityConditionDTO)
        //    {
        //        var condition = (e.DataElement as EntitySecurityConditionDTO);
        //        SetConditionColumns(condition);
        //    }
        //}

        //private void SetConditionColumns(EntitySecurityConditionDTO condition)
        //{
        //    colColumns.DisplayMemberPath = "Name";
        //    colColumns.SelectedValueMemberPath = "ID";
        //    BizTableDrivedEntity biz = new BizTableDrivedEntity();

        //    if (condition.RelationshipTailID == 0)
        //    {
        //        if (lokEntities.SelectedItem != null)
        //        {
        //            var entity = biz.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokEntities.SelectedValue, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
        //            condition.Columns = entity.Columns; ;
        //        }
        //        else
        //            condition.Columns = new List<ColumnDTO>();
        //    }
        //    else
        //    {
        //        var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(MyProjectManager.GetMyProjectManager.GetRequester(), condition.RelationshipTailID);
        //        var entity = biz.GetTableDrivedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), relationshipTail.TargetEntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
        //        condition.Columns = entity.Columns;
        //    }
        //}

        private void SetEnityStates()
        {
            if (lokEntities.SelectedItem != null)
            {

                List<EntityStateDTO> entityStates = null;

                //////if (mode == DataDirectSecurityMode.FetchData)
                //////{
                //////    entityStates = bizEntityState.GetEntityStates(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokEntities.SelectedValue, false).Where(x => x.ColumnID != 0).ToList();
                //////}
                //////else
                //////{
                //////    entityStates = bizEntityState.GetEntityStates(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokEntities.SelectedValue, false);
                //////}
                ///
                entityStates = bizEntityState.GetEntityStates(MyProjectManager.GetMyProjectManager.GetRequester(), (int)lokEntities.SelectedValue, false,false).Where(x => x.FormulaID == 0).ToList();
                lokState.DisplayMember = "Title";
                lokState.SelectedValueMember = "ID";
                lokState.ItemsSource = entityStates;
            }
            else
            {
                lokState.ItemsSource = null;
            }
        }

        //private void SetAndOrType()
        //{
        //    cmbAndOR.ItemsSource = Enum.GetValues(typeof(AndORType));
        //}

        //private void GetEntitySecurityDirect(DR_Requester requester, int id)
        //{
        //    Message = bizRoleSecurity.GetEntitySecurityDirect(requester, id, true);
        //    ShowMessage();
        //}
        //private void SetOperators()
        //{
        //    colOperator.ItemsSource = Enum.GetValues(typeof(EntitySecurityOperator));
        //}


        //private void SetDatabaseFunctions()
        //{
        //    colDatabaseFunction.DisplayMemberPath = "Name";
        //    colDatabaseFunction.SelectedValueMemberPath = "ID";
        //    colDatabaseFunction.ItemsSource = bizDatabaseFunction.GetDatabaseFunctions(MyProjectManager.GetMyProjectManager.GetRequester(), Enum_DatabaseFunctionType.None);
        //}


        //private void SetSecuritySubjects()
        //{
        //    //lokSubject.DisplayMember = "Name";
        //    //lokSubject.SelectedValueMember = "ID";
        //    //lokSubject.SearchFilterChanged += LokSubject_SearchFilterChanged;
        //    colSecuritySubject.DisplayMemberPath = "Name";
        //    colSecuritySubject.SelectedValueMemberPath = "ID";
        //    colSecuritySubject.SearchFilterChanged += LokSubject_SearchFilterChanged;
        //}
        //private void LokSubject_SearchFilterChanged(object sender, MyCommonWPFControls.SearchFilterArg e)
        //{
        //    if (e.SingleFilterValue != null)
        //    {
        //        if (!e.FilterBySelectedValue)
        //        {
        //            var subjects = bizSecuritySubject.GetSecuritySubjects(e.SingleFilterValue);
        //            e.ResultItemsSource = subjects;
        //        }
        //        else
        //        {
        //            var id = Convert.ToInt32(e.SingleFilterValue);
        //            if (id > 0)
        //            {
        //                var subject = bizSecuritySubject.GetSecuritySubject(id);
        //                e.ResultItemsSource = new List<SecuritySubjectDTO> { subject };
        //            }
        //            else
        //                e.ResultItemsSource = null;
        //        }
        //    }
        //}

        ////private void btnAddRoleGroup_Click(object sender, RoutedEventArgs e)
        //{
        //    frmRoleGroup view = new frmRoleGroup(0);
        //    view.GroupSaved += View_GroupSaved;
        //       MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form");
        //}

        //private void View_GroupSaved(object sender, EventArgs e)
        //{
        //    SetSecuritySubjects();
        //}

        //private void btnEditRoleGroup_Click(object sender, RoutedEventArgs e)
        //{
        //    var selected = cmbRole.SelectedItem as RoleOrRoleGroupDTO;
        //    if (selected != null)
        //    {
        //        frmRoleGroup view = new frmRoleGroup(selected.ID);
        //        view.GroupSaved += View_GroupSaved;
        //           MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form");
        //    }
        //}
        //private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    btnEditRoleGroup.Visibility = Visibility.Collapsed;
        //    var selected = cmbRole.SelectedItem as RoleOrRoleGroupDTO;
        //    if (selected != null)
        //    {
        //        if (selected.Type == RoleOrRoleGroupType.RoleGroup)
        //        {
        //            btnEditRoleGroup.Visibility = Visibility.Visible;
        //        }
        //    }
        //}
        private void ShowMessage()
        {
            //  dtgSecuritySubjects.ItemsSource = Message.SecuritySubjects;
            //cmbAndOR.SelectedItem = Message.ConditionAndORType;
            lokEntities.SelectedValue = Message.TableDrivedEntityID;
            cmbMode.SelectedItem = Message.Mode;
            lokState.SelectedValue = Message.EntityStateID;
            txtDescription.Text = Message.Description;
            //lokRelationshipTail.SelectedValue = Message.RelationshipTailID;
            //if (Message.FormulaID != 0)
            //{
            //    lokFormula.SelectedValue = Message.FormulaID;
            //    dtgFormulaValue.ItemsSource = Message.Values;
            //    optFormula.IsChecked = true;
            //}
            //else if (Message.ColumnID != 0)
            //{
            //    cmbOperator.SelectedItem = Message.ValueOperator;
            //    cmbColumns.SelectedValue = Message.ColumnID;
            //    dtgColumnValue.ItemsSource = Message.Values;
            //    optColumn.IsChecked = true;
            //}
            //else
            //{
            //    dtgFormulaValue.ItemsSource = Message.Values;
            //    dtgColumnValue.ItemsSource = Message.Values;
            //}

            //chkIgnoreSecurity.IsChecked = Message.IgnoreSecurity;
            //cmbInOrNotIn.SelectedItem = Message.SecuritySubjectInORNotIn;
            //cmbColumns.SelectedValue = Message.ColumnID;
            //if (Message.RoleID != 0)
            //    cmbRole.SelectedValue = Message.RoleID;
            //else if (Message.RoleGroupID != 0)
            //    cmbRole.SelectedValue = Message.RoleGroupID;
            //if (!string.IsNullOrEmpty(Message.Value))
            //{
            //    txtValue.Text = Message.Value;
            //    optValue.IsChecked = true;
            //}
            //else if (Message.DBFunctionID != 0)
            //{
            //    cmbDatabaseFunction.SelectedValue = Message.DBFunctionID;
            //    optDBFunction.IsChecked = true;
            //}
            //cmbOperator.SelectedItem = Message.Operator;
            //cmbRole.SelectedItem= Message.Operator ;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (lokEntities.SelectedItem == null)
            {
                MessageBox.Show("موجودیت انتخاب نشده است");
                return;
            }
            //if (cmbColumns.SelectedItem == null)
            //{
            //    MessageBox.Show("ستون معادل انتخاب نشده است");
            //    return;
            //}
            //if (cmbRole.SelectedItem == null)
            //{
            //    MessageBox.Show("نفش معادل انتخاب نشده است");
            //    return;
            //}
            //if (cmbOperator.SelectedItem == null)
            //{
            //    MessageBox.Show("عملگر معادل انتخاب نشده است");
            //    return;
            //}

            //if (optFormula.IsChecked == false && optColumn.IsChecked == false)
            //{
            //    MessageBox.Show("یکی از حالات فرمول و یا ستون را انتخاب نمایید");
            //    return;
            //}
            //if (optFormula.IsChecked == true)
            //{
            //    if (lokFormula.SelectedItem == null)
            //    {
            //        MessageBox.Show("فرمول مشخص نشده است");
            //        return;
            //    }
            //}
            //else if (optColumn.IsChecked == true)
            //{
            //    if (cmbColumns.SelectedItem == null)
            //    {
            //        MessageBox.Show("ستون مشخص نشده است");
            //        return;
            //    }
            //}
            //if (lokRelationshipTail.SelectedItem == null)
            //    Message.RelationshipTailID = 0;
            //else
            //    Message.RelationshipTailID = (int)lokRelationshipTail.SelectedValue;
            //if (optFormula.IsChecked == true)
            //{
            //    Message.FormulaID = (int)lokFormula.SelectedValue;
            //    Message.ColumnID = 0;
            //}
            //else if (optColumn.IsChecked == true)
            //{
            //    Message.FormulaID = 0;
            //    Message.ColumnID = (int)cmbColumns.SelectedValue;
            //}
            // Message.ValueOperator = (Enum_EntityStateOperator)cmbOperator.SelectedItem;
            Message.Description = txtDescription.Text;
            //   Message.IgnoreSecurity = chkIgnoreSecurity.IsChecked == true;
            Message.Mode = (DataDirectSecurityMode)cmbMode.SelectedItem;
            Message.TableDrivedEntityID = (int)lokEntities.SelectedValue;
            Message.EntityStateID = (int)lokState.SelectedValue;
            //    Message.SecuritySubjectInORNotIn = (InORNotIn)cmbInOrNotIn.SelectedItem;
            //Message.ConditionAndORType = (AndORType)cmbAndOR.SelectedItem;
            //if (lokSubject.SelectedItem == null)
            //    Message.SecuritySubjectID = 0;
            //else
            //    Message.SecuritySubjectID = (int)lokSubject.SelectedValue;
            //var selected = cmbRole.SelectedItem as RoleOrRoleGroupDTO;
            //if (selected.Type == RoleOrRoleGroupType.Role)
            //{
            //    Message.RoleID = (int)cmbRole.SelectedValue;
            //    Message.RoleGroupID = 0;
            //}
            //else
            //{
            //    Message.RoleID = 0;
            //    Message.RoleGroupID = (int)cmbRole.SelectedValue;
            //}

            //if (optDBFunction.IsChecked == true)
            //{
            //    Message.Value = "";
            //    Message.DBFunctionID = (int)cmbDatabaseFunction.SelectedValue;
            //}
            //else
            //{
            //    Message.DBFunctionID = 0;
            //    Message.Value = txtValue.Text;
            //}
            //Message.Operator = (EntitySecurityOperator)cmbOperator.SelectedItem;
            Message.ID = bizRoleSecurity.UpdateEntitySecurityDirect(Message);
            MessageBox.Show("اطلاعات ثبت شد");
        }
        //private void optFormula_Checked(object sender, RoutedEventArgs e)
        //{
        //    tabColumn.Visibility = Visibility.Collapsed;
        //    tabFormula.Visibility = Visibility.Visible;
        //    tabFormula.IsSelected = true;
        //}


        //private void optColumn_Checked(object sender, RoutedEventArgs e)
        //{
        //    tabFormula.Visibility = Visibility.Collapsed;
        //    tabColumn.Visibility = Visibility.Visible;
        //    tabColumn.IsSelected = true;
        //}
        //private void btnReturn_Click(object sender, RoutedEventArgs e)
        //{
        //    MyProjectManager.GetMyProjectManager.CloseDialog(this);
        //}

        //private void optValue_Checked(object sender, RoutedEventArgs e)
        //{
        //    cmbDatabaseFunction.IsEnabled = false;
        //    txtValue.IsEnabled = true;
        //}

        //private void optDBFunction_Checked(object sender, RoutedEventArgs e)
        //{
        //    txtValue.IsEnabled = false;
        //    cmbDatabaseFunction.IsEnabled = true;

        //}
        //frmEntitySecurityDirectSelect searchView = null;

        //private void btnSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    if (searchView == null)
        //    {
        //        searchView = new frmEntitySecurityDirectSelect();
        //        searchView.EntitySecurityDirectSelected += View_EntitySecurityDirectSelected;
        //    }
        //    MyProjectManager.GetMyProjectManager.ShowDialog(searchView, "جستجوی دسترسی داده");
        //}

        //private void View_EntitySecurityDirectSelected(object sender, EntitySecurityDirectSelectArg e)
        //{
        //    if (e.ID != 0)
        //    {
        //        MyProjectManager.GetMyProjectManager.CloseDialog(sender);
        //        GetEntitySecurityDirect(MyProjectManager.GetMyProjectManager.GetRequester(), e.ID);
        //    }
        //}

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Message = new EntitySecurityDirectDTO();
            ShowMessage();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Message.ID != 0)
            {
                bizRoleSecurity.DeleteEntitySecurityDirect(Message.ID);
                btnNew_Click(null, null);
            }
        }






        //private void btnDatabaseFunctionEntity_Click(object sender, RoutedEventArgs e)
        //{
        //    if (cmbDatabaseFunction.SelectedItem != null)
        //    {
        //        var selected = cmbDatabaseFunction.SelectedItem as DatabaseFunctionDTO;
        //        var DatabaseFunctionEntityID = bizDatabaseFunction.GetDatabaseFunctionEntityID(EntityID, selected.ID);
        //        var DatabaseFunctionIntention = new DatabaseFunctionEntityIntention();
        //        DatabaseFunctionIntention.EntityID = EntityID;
        //        if (DatabaseFunctionEntityID == 0)
        //            DatabaseFunctionIntention.Type = Enum_DatabaseFunctionEntityIntention.DatabaseFunctionEntityDefinition;
        //        else
        //        {
        //            DatabaseFunctionIntention.DatabaseFunctionEntityID = DatabaseFunctionEntityID;
        //            DatabaseFunctionIntention.Type = Enum_DatabaseFunctionEntityIntention.DatabaseFunctionEntityEdit;
        //        }
        //        frmDatabaseFunction_Entity view = new frmDatabaseFunction_Entity(DatabaseFunctionIntention);
        //           MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form");

        //    }
        //}
    }


}
