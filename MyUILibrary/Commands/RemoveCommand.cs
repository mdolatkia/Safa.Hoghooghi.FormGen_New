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
    class RemoveCommand : BaseCommand
    {
        I_EditEntityAreaMultipleData EditArea { set; get; }
        public RemoveCommand(I_EditEntityAreaMultipleData editArea) : base()
        {
            //RemoveCommand: 43c89dd5b4bf
            EditArea = editArea;
            //if (AgentHelper.GetAppMode() == AppMode.Paper)
            //    CommandManager.SetTitle("Remove");
            //else
            CommandManager.SetTitle("حذف");
            CommandManager.ImagePath = "Images//remove.png";
            if (!editArea.AreaInitializer.Preview)
                CommandManager.Clicked += CommandManager_Clicked;
        }
        List<DP_DataRepository> deleteDataList = new List<DP_DataRepository>();
        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            //if (EditArea.SourceRelationColumnControl == null)
            //    return;

            //if (EditArea.SourceRelationColumnControl != null)
            //{
            //    //EditArea.SourceRelationColumnControl.RelatedData.ValueChanged = true;
            //}

            //(EditArea as I_EditEntityAreaMultipleData).RemoveSelectedData();
            if (EditArea.SourceRelationColumnControl == null)
            {
                var datas = EditArea.GetSelectedData();
                EditArea.RemoveMultipleData(datas);
            }
            else
            {
                var datas = EditArea.GetSelectedData();
                EditArea.ChildRelationshipInfoBinded.RemoveRelatedMultipleData(datas);
            }

        }





    }
}
