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
    class GridViewCommand : BaseCommand
    {
        I_EditEntityArea EditArea { set; get; }
        public GridViewCommand(I_EditEntityArea editArea) : base()
        {
            // GridViewCommand: a7111982b91c
            EditArea = editArea;
            CommandManager.SetTitle("گرید داده");
            CommandManager.ImagePath = "Images//grid.png";
            if (!editArea.AreaInitializer.Preview)
                CommandManager.Clicked += CommandManager_Clicked;
        }

        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            if (EditArea != null)
            {
                DP_SearchRepositoryMain searchDP = null;
                if (EditArea.SearchEntityArea != null)
                {
                    searchDP = EditArea.SearchEntityArea.LastSearchRepository;
                }
                //if (searchDP != null)
                //{
                //if (packageArea.GridViewArea == null)
                //{

                //}
                //else
                //{
                AgentUICoreMediator.GetAgentUICoreMediator.ShowDataViewGridViewArea(EditArea.AreaInitializer.EntitySearchID, EditArea.SimpleEntity.Alias, false, true, false, searchDP,null, true, 0, EditArea.AreaInitializer.EntityListViewID, EditArea.AreaInitializer.EntityListViewID, null, null);
                //}

                //}
            }
        }
    }
}
