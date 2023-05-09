﻿using CommonDefinitions.UISettings;

using MyUILibrary;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea.Commands
{
    class DataViewCommand : BaseCommand
    {
        I_EditEntityArea EditArea { set; get; }
        public DataViewCommand(I_EditEntityArea editArea) : base()
        {
            // DataViewCommand: 939641817cea
            EditArea = editArea;
            CommandManager.SetTitle("نمایش داده");
            CommandManager.ImagePath = "Images//grid.png";
            if (!editArea.AreaInitializer.Preview)
                CommandManager.Clicked += CommandManager_Clicked;
        }

        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            if (EditArea != null)
            {
                var searchDP = EditArea.SearchEntityArea?.LastSearch;
                //if (searchDP != null)
                //{
                //if (packageArea.DataViewArea == null)
                //{

                //}
                //else
                //{
                AgentUICoreMediator.GetAgentUICoreMediator.ShowDataViewGridViewArea(EditArea.AreaInitializer.EntityID, EditArea.SimpleEntity.Alias, false, true, true, searchDP, true, 0, null, null);
                //}

                //}
            }
        }
    }
}
