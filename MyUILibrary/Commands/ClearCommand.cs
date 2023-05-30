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
            // ClearCommand: d587de1402da
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

            if (EditArea.SourceRelationColumnControl == null)
                EditArea.ClearData();
            else
                EditArea.ChildRelationshipInfoBinded.RemoveAllRelatedData();
            //if (EditArea is I_EditEntityAreaOneData)
            //{
            //    (EditArea as I_EditEntityAreaOneData).ClearData(true);
            //}
            //else if (EditArea is I_EditEntityAreaMultipleData)
            //{
            //    (EditArea as I_EditEntityAreaMultipleData).ClearData(false);
            //}

                //if (EditArea.SourceRelationColumnControl == null)
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
                //    if (EditArea.SourceRelationColumnControl == null)
                //        EditArea.AreaInitializer.Datas.Clear();
                //    else
                //    {
                //        EditArea.SourceRelationColumnControl.RelatedData.RemoveRelatedData(EditArea.SourceRelationColumnControl.Relationship.ID);
                //        if (EditArea.ParentTemporaryView != null)
                //            EditArea.ParentTemporaryView.SetLinkText("");
                //    }
                //    (EditArea as I_EditEntityAreaMultipleData).ClearDataContainsers();
                //}
        }





    }
}
