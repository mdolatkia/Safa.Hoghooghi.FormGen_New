using CommonDefinitions.UISettings;

using MyUILibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea.Commands
{
    class SearchCommand : BaseCommand
    {
        I_EditEntityArea EditArea { set; get; }
        public SearchCommand(I_EditEntityArea editArea) : base()
        {
            //SearchCommand: cda8ae466b21
            EditArea = editArea;
            //if (AgentHelper.GetAppMode() == AppMode.Paper)
            //    CommandManager.SetTitle("Search");
            //else
            CommandManager.SetTitle("جستجو");
            CommandManager.ImagePath = "Images//search.png";
            if (!editArea.AreaInitializer.Preview)
                CommandManager.Clicked += CommandManager_Clicked;
        }

        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            //** b5e691ad-29ae-4b2c-a2a1-c971552116e1
            //if (EditArea.SearchViewEntityArea != null)
            //{
            //AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(packageArea.SearchViewEntityArea.View, packageArea.AreaInitializer.Title);

            I_EditEntityArea editArea = null;

            if (EditArea.AreaInitializer.SourceRelationColumnControl == null)
                editArea = EditArea;
            else
                editArea = EditArea.ChildRelationshipInfoBinded.RelationshipControl.GenericEditNdTypeArea;

            editArea.ShowSearchView(true);

            // }
        }

    }
}
