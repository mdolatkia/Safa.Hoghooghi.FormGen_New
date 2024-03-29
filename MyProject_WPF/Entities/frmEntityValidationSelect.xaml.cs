﻿
using ModelEntites;

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
    public partial class frmEntityValidationSelect : UserControl
    {
        public event EventHandler<EntityValidationSelectedArg> EntityValidationSelected;
        public int EntityID { set; get; }
        BizEntityValidation bizEntityValidation = new BizEntityValidation();
        public frmEntityValidationSelect(int entityID)
        {
            InitializeComponent();

            EntityID = entityID;
            GetEntityValidations();
        }

        private void FrmFormula_Loaded(object sender, RoutedEventArgs e)
        {
            GetEntityValidations();

        }
     
        private void GetEntityValidations()
        {
            var listEntityValidations = bizEntityValidation.GetEntityValidations(EntityID,false);
            dtgItems.ItemsSource = listEntityValidations;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            var item = dtgItems.SelectedItem as EntityValidationDTO;
            if (item != null)
            {
                if (EntityValidationSelected != null)
                    EntityValidationSelected(this, new EntityValidationSelectedArg() { EntityValidationID = item.ID });
            }
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }
    }

    public class EntityValidationSelectedArg : EventArgs
    {
        public int EntityValidationID { set; get; }
    }

}
