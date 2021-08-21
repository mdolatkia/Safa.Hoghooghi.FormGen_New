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
    public partial class frmEditSubEntity : UserControl
    {
        public event EventHandler<string> NameChanged;
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        List<TableDrivedEntityDTO> DrivedEntities { set; get; }
        int EntityID { set; get; }
        TableDrivedEntityDTO BaseEntity { set; get; }
        public Tuple<SuperToSubRelationshipDTO, SubToSuperRelationshipDTO, TableDrivedEntityDTO> Message { set; get; }
        public ColumnDTO SelectedColumn
        {

            get
            {
                return dtgColumnsDrived.SelectedItem as ColumnDTO;
            }
        }

        public RelationshipDTO SelectedRelationship
        {
            get
            {
                return dtgRelationshipsDrived.SelectedItem as RelationshipDTO;
            }
        }

        public frmEditSubEntity(TableDrivedEntityDTO baseEntity, Tuple<SuperToSubRelationshipDTO, SubToSuperRelationshipDTO, TableDrivedEntityDTO> tuple, List<ColumnDTO> columns)
        {
            InitializeComponent();
            BaseEntity = baseEntity;
            Message = tuple;
            cmbValue.DisplayMemberPath = "Alias";
            cmbValue.SelectedValuePath = "ID";
            cmbValue.ItemsSource = columns;
            cmbValue.SelectedValue = tuple.Item1.SuperEntityDeterminerColumnID;
            txtName.Text = tuple.Item3.Name;
            txtAlias.Text = tuple.Item3.Alias;
            //cmbValue.SelectedValue = TableDrivedEntityDTO.DeterminerColumnID;
            //txtValue.Text = TableDrivedEntityDTO.DeterminerColumnValue;
            dtgColumnsDrived.ItemsSource = Message.Item3.Columns;
            dtgRelationshipsDrived.ItemsSource = Message.Item3.Relationships;
            dtgValues.ItemsSource = Message.Item1.DeterminerColumnValues;

            ControlHelper.GenerateContextMenu(dtgValues);

            txtName.TextChanged += TxtName_TextChanged;

        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameChanged != null)
                NameChanged(this, txtName.Text);
        }

        public void UpdateDrivedEntity()
        {
            Message.Item3.Name = txtName.Text;
            Message.Item3.Alias = txtAlias.Text;
            if (cmbValue.SelectedItem != null)
                Message.Item1.SuperEntityDeterminerColumnID = (int)cmbValue.SelectedValue;
            else
                Message.Item1.SuperEntityDeterminerColumnID = 0;

            Message.Item1.Name = BaseEntity.Name + ">" + Message.Item3.Name;
            Message.Item1.Alias = Message.Item3.Alias;


            Message.Item2.Name = Message.Item3.Name + ">" + BaseEntity.Name;
            Message.Item2.Alias = BaseEntity.Alias;

            //Message.DeterminerColumnValue = txtValue.Text;
        }

        //internal void AddColumn(ColumnDTO column)
        //{

        //}

        internal void RemoveColumn(ColumnDTO column)
        {
            Message.Item3.Columns.Remove(column);
            dtgColumnsDrived.ItemsSource = null;
            dtgColumnsDrived.ItemsSource = Message.Item3.Columns;
        }

        //internal void AddRelationship(RelationshipDTO relationship)
        //{
        //    Message.Relationships.Add(relationship);
        //    dtgRelationshipsDrived.ItemsSource = null;
        //    dtgRelationshipsDrived.ItemsSource = Message.Relationships;
        //}

        internal void RemoveRelationship(RelationshipDTO relationship)
        {
            Message.Item3.Relationships.Remove(relationship);
            dtgRelationshipsDrived.ItemsSource = null;
            dtgRelationshipsDrived.ItemsSource = Message.Item3.Relationships;
        }
    }
}
