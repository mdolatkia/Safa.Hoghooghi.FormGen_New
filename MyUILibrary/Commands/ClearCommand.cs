using CommonDefinitions.UISettings;
using MyUILibrary;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea.Commands
{
    class ClearCommand : BaseCommand
    {
        I_EditEntityArea EditArea { set; get; }
        public ClearCommand(I_EditEntityArea editArea) : base()
        {
            EditArea = editArea;
            CommandManager.SetTitle("پاک کردن");
            CommandManager.ImagePath = "Images//clear.png";
            if (!editArea.AreaInitializer.Preview)
                CommandManager.Clicked += CommandManager_Clicked;
        }

        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            //if (EditArea is I_EditEntityArea)
            //{

            if (EditArea.AreaInitializer.SourceRelationColumnControl == null)
                EditArea.ClearData();
            else
                EditArea.ChildRelationshipInfoBinded.RemoveRelatedData();
            //if (EditArea is I_EditEntityAreaOneData)
            //{
            //    (EditArea as I_EditEntityAreaOneData).ClearData(true);
            //}
            //else if (EditArea is I_EditEntityAreaMultipleData)
            //{
            //    (EditArea as I_EditEntityAreaMultipleData).ClearData(false);
            //}

                //if (EditArea.AreaInitializer.SourceRelationColumnControl == null)
                //    EditArea.AreaInitializer.Datas.Clear();
                //else
                //{
                //    EditArea.ChildRelationshipInfo.RelatedData.Clear();
                //    //if (EditArea.ParentTemporaryView != null)
                //    //    EditArea.ParentTemporaryView.SetLinkText("");
                //}
                //(EditArea as I_EditEntityAreaOneData).CreateDefaultData();
                //}
                //else if (EditArea is I_EditEntityAreaMultipleData)
                //{
                //    if (EditArea.AreaInitializer.SourceRelationColumnControl == null)
                //        EditArea.AreaInitializer.Datas.Clear();
                //    else
                //    {
                //        EditArea.AreaInitializer.SourceRelationColumnControl.RelatedData.RemoveRelatedData(EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.ID);
                //        if (EditArea.ParentTemporaryView != null)
                //            EditArea.ParentTemporaryView.SetLinkText("");
                //    }
                //    (EditArea as I_EditEntityAreaMultipleData).ClearDataContainsers();
                //}
        }





    }
}
