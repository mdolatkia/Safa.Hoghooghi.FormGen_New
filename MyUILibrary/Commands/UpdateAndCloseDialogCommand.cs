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
    class UpdateAndCloseDialogCommand : BaseCommand
    {
        I_EditEntityArea EditArea { set; get; }
        public UpdateAndCloseDialogCommand(I_EditEntityArea editArea) : base()
        {
            // UpdateAndCloseDialogCommand: dc6ea404ac9d
            EditArea = editArea;
            //if (AgentHelper.GetAppMode() == AppMode.Paper)
            //    CommandManager.SetTitle("Save and close");
            //else
            CommandManager.SetTitle("تایید و خروج");
            CommandManager.ImagePath = "Images//letter.png";
            if (!editArea.AreaInitializer.Preview)
                CommandManager.Clicked += CommandManager_Clicked;
        }

        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            var result = EditArea.UpdateDataAndValidate(EditArea.ChildRelationshipInfoBinded.RelatedData);
            if (!result.IsValid)
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("پیام", result.Message);
                return;
            }
            //if (EditArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect
            //    || EditArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect)
            //{
            //    if (EditArea is I_EditEntityAreaOneData)
            //        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog((EditArea as I_EditEntityAreaOneData).DataViewGeneric);
            //    else if (EditArea is I_EditEntityAreaMultipleData)
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(EditArea.DataViewGeneric);
            //}

        }





    }
}
