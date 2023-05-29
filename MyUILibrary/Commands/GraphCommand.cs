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
    class GraphCommand : BaseCommand
    {
        I_EditEntityArea EditArea { set; get; }
        public GraphCommand(I_EditEntityArea editArea) : base()
        {
            // GraphCommand: 6ef5fd4bbae5
            EditArea = editArea;
            CommandManager.SetTitle("گراف داده");
            CommandManager.ImagePath = "Images//Close.png";
            if (!editArea.AreaInitializer.Preview)
                CommandManager.Clicked += CommandManager_Clicked;
        }

        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            if (EditArea != null)
            {
           //     var searchDP = EditArea.SearchEntityArea.LastSearch;
                DP_DataRepository dataInstance = null;
                if (EditArea.AreaInitializer.DataMode != DataMode.Multiple)
                {
                    dataInstance = EditArea.GetDataList().FirstOrDefault();
                }
                else
                    dataInstance = (EditArea as EditEntityAreaMultipleData).GetSelectedData().FirstOrDefault();
                DP_DataView dataView = null;
                if (dataInstance != null)
                {
                    if ( !dataInstance.IsNewItem)
                    {
                        dataView = dataInstance;
                    }
                }
                //if (searchDP != null)
                //{
                //if (packageArea.GraphArea == null)
                //{



                //}
                //else
                //{

                //}
                AgentUICoreMediator.GetAgentUICoreMediator.ShowGraphArea(EditArea.AreaInitializer.EntityID, 0, false, "لینک داده ", dataView);
                //}
            }
        }






    }
}
