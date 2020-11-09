﻿using Microsoft.Win32;
using ModelEntites;
using MyCommonWPFControls;

using MyFormulaFunctionStateFunctionLibrary;

using MyModelManager;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class frmLetterRelationships : UserControl
    {
        BizLetterTemplate bizLetterTemplate = new BizLetterTemplate();
       // ArchiveRelationshipTailDTO Message { set; get; }
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        int EntityID { set; get; }

        public frmLetterRelationships(int entityID)
        {
            InitializeComponent();
            EntityID = entityID;
            dtgRelationships.ItemsSource = bizLetterTemplate.GetLetterRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID,false);
            SetRelationshipTails();

            colRelationshipTail.NewItemEnabled = true;
            colRelationshipTail.EditItemEnabled = true;
            colRelationshipTail.EditItemClicked += ColRelationshipTail_EditItemClicked;

            ControlHelper.GenerateContextMenu(dtgRelationships);
        }
        private void ColRelationshipTail_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            frmEntityRelationshipTail view = new frmEntityRelationshipTail(EntityID);
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "رابطه");
            view.ItemSelected += (sender1, e1) => View_ItemSelected(sender1, e1, (sender as MyStaticLookup));

        }
        private void View_ItemSelected(object sender, EntityRelationshipTailSelectedArg e, MyStaticLookup lookup)
        {
            SetRelationshipTails();
            lookup.SelectedValue = e.EntityRelationshipTailID;
        }

     

      

        private void SetRelationshipTails()
        {
            //چک شود فقط یکی ازین دو پر شوند
            var relationshipTails = bizEntityRelationshipTail.GetEntityRelationshipTails(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
            colRelationshipTail.DisplayMemberPath = "EntityPath";
            colRelationshipTail.SelectedValueMemberPath = "ID";
            colRelationshipTail.ItemsSource = relationshipTails;
        }




     

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bizLetterTemplate.UpdateLetterRelationshipTails(EntityID, dtgRelationships.ItemsSource as List<LetterRelationshipTailDTO>);
            MessageBox.Show("اطلاعات ثبت شد");
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {

        }

      
      
    
    }

}
