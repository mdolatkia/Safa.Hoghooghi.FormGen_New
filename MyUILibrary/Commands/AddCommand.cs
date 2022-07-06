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
    public class AddCommand : BaseCommand
    {
        I_EditEntityAreaMultipleData EditArea { set; get; }
        public AddCommand(I_EditEntityAreaMultipleData editArea) : base()
        {
            EditArea = editArea;
            //if (AgentHelper.GetAppMode() == AppMode.Paper)
            //    CommandManager.SetTitle("Add");
            //else
            CommandManager.SetTitle("افزودن");
            CommandManager.ImagePath = "Images//add.png";
            CommandManager.Clicked += CommandManager_Clicked;
        }

        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            //int relationID = 0;
            //DP_DataRepository RelationData = null;
            //if (EditArea.AreaInitializer.SourceRelationColumnControl != null)
            //{
            //    relationID = EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.ID;
            //    RelationData = EditArea.AreaInitializer.SourceRelationColumnControl.RelatedData;
            //    //if (RelationData != null)
            //    //    RelationData.ValueChanged = true;
            //}

            var newData = AgentHelper.CreateAreaInitializerNewData(EditArea);
            //var list = AgentHelper.CreateListFromSingleObject<DP_DataRepository>(newData);
            //if (EditArea.AreaInitializer.SourceRelationColumnControl == null)
            //{
            //    EditArea.AreaInitializer.Datas.Add(newData);
            //}
            //else
            //{
            //    EditArea.ChildRelationshipInfo.RelatedData.Add(newData);
            //}
            if (EditArea.AreaInitializer.SourceRelationColumnControl == null)
            {
                var addResult = EditArea.AddData(newData);
                if (!addResult)
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", newData.ViewInfo, Temp.InfoColor.Red);
               

            }
            else
            {
                EditArea.ChildRelationshipInfoBinded.AddDataToChildRelationshipInfo(newData);
            }
            //EditArea.ShowDataInDataView(list, false);
        }

        public void Execute(I_EditEntityArea editArea)
        {


            //     editArea.ShowData(AgentHelper.CreateListFromSingleObject<DP_DataRepository>(newData));
        }
        //bool _Enabled;
        //public bool Enabled
        //{
        //    get
        //    {
        //        return _Enabled;
        //    }
        //    set
        //    {
        //        _Enabled = value;
        //        OnEnabledChanged();
        //    }
        //}

        //public event EventHandler EnabledChanged;


        //public void OnEnabledChanged()
        //{
        //    if (EnabledChanged != null)
        //        EnabledChanged(this, null);
        //}



    }
}
