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
    public class SimpleSearchConfirmCommand : BaseCommand
    {
        I_EntityDefinedSearchArea SearchArea { set; get; }
        public SimpleSearchConfirmCommand(I_EntityDefinedSearchArea searchArea) : base()
        {
            //SimpleSearchConfirmCommand: 72429c533aa4
            SearchArea = searchArea;
            //if (AgentHelper.GetAppMode() == AppMode.Paper)
            //    CommandManager.SetTitle("Search");
            //else
                CommandManager.SetTitle("جستجو");
            CommandManager.ImagePath = "Images//search.png";
            CommandManager.Clicked += CommandManager_Clicked;
        }
        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            //Enabled = false;
            //try
            //{
            SearchArea.ConfirmSearch();
      
            //Enabled = true;
            //var command = new AG_CommandExecutionRe
        }

    }
}
