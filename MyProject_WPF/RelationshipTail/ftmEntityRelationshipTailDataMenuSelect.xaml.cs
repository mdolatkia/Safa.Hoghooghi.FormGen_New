
using ModelEntites;
using MyFormulaFunctionStateFunctionLibrary;
using MyModelManager;
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


namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmFormula.xaml
    /// </summary>
    public partial class ftmEntityRelationshipTailDataMenuSelect : UserControl
    {
        public event EventHandler<EntityDataMenuSelectedArg> EntityDataMenuSelected;
        public int RelationshipTailID { set; get; }
        BizEntityRelationshipTailDataMenu bizEntityRelationshipTailDataMenu = new BizEntityRelationshipTailDataMenu();
        SelectorGrid SelectorGrid = null;

        public ftmEntityRelationshipTailDataMenuSelect(int relationshipTailID)
        {
            InitializeComponent();

            RelationshipTailID = relationshipTailID;

            var listColumns = new Dictionary<string, string>();
            listColumns.Add("ID", "شناسه");
            listColumns.Add("Name", "عنوان");

            SelectorGrid = ControlHelper.SetSelectorGrid(dtgItems, listColumns);
            SelectorGrid.DataItemSelected += SelectorGrid_DataItemSelected;
            this.Loaded += ftmEntityRelationshipTailDataMenuSelect_Loaded;
        }

        private void ftmEntityRelationshipTailDataMenuSelect_Loaded(object sender, RoutedEventArgs e)
        {
            GetEntityDataMenus();
        }

        private void SelectorGrid_DataItemSelected(object sender, object e)
        {
            CheckSelectedItem(e);
        }

        private void CheckSelectedItem(object item)
        {
            if (item != null)
            {
                var selected = dtgItems.SelectedItem as DataMenuSettingDTO;
                if (selected != null)
                {
                    if (EntityDataMenuSelected != null)
                        EntityDataMenuSelected(this, new EntityDataMenuSelectedArg() { ID = selected.ID });
                }
            }
        }
     
        private void GetEntityDataMenus()
        {
            var listEntityDataMenus = bizEntityRelationshipTailDataMenu.GetEntityRelationshipTailDataMenus(MyProjectManager.GetMyProjectManager.GetRequester(), RelationshipTailID);
            dtgItems.ItemsSource = listEntityDataMenus;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            CheckSelectedItem(dtgItems.SelectedItem);
        }
    }

    public class EntityDataMenuSelectedArg : EventArgs
    {
        public int ID { set; get; }
    }

}
