using MyUILibrary.DataMenuArea;
using MyUILibraryInterfaces.DataMenuArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using ModelEntites;

using MyModelManager;

namespace MyUILibrary.DataMenuArea
{
    public class DataMenuArea : I_DataMenuArea
    {
        DataMenuAreaInitializer AreaInitializer { set; get; }
        public DataMenuArea(DataMenuAreaInitializer initializer)
        {
            // DataMenuArea: 6f727758aef9
            AreaInitializer = initializer;
        }
        List<DataMenuUI> Menus = null;

        DataMenuResult DataMenuResult { set; get; }
        private List<DataMenuUI> GetDataMenus()
        {
            if (Menus == null)
            {
                var Menus = new List<DataMenuUI>();
                DataMenuResult = AgentUICoreMediator.GetAgentUICoreMediator.DataMenuManager.GetDataMenu(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), AreaInitializer.DataItem, AreaInitializer.DataMenuSettingID);
                AddUIMenu(Menus, DataMenuResult.DataMenus, DataMenuResult.NewData != null ? DataMenuResult.NewData : AreaInitializer.DataItem);
                return Menus;
            }
            else
                return Menus;
        }
        private void AddUIMenu(List<DataMenuUI> menus, List<DataMenuDTO> dataMenus, DP_DataView dataItem)
        {
            // DataMenuArea.AddUIMenu: 9b385d55d0be
            foreach (var item in dataMenus)
            {
                DataMenuUI menuUI = new DataMenuUI();
                menuUI.DataMenu = item;
                menuUI.Title = item.Title;
                menuUI.Tooltip = item.Tooltip;
                menuUI.DataItem = dataItem;
                menus.Add(menuUI);
                menuUI.MenuClicked += MenuUI_MenuClicked;
                AddUIMenu(menuUI.SubMenus, item.SubMenus, dataItem);
            }
        }
        internal void ShowMenu()
        {
            // DataMenuArea.ShowMenu: 40eb6254d138  
            if (AreaInitializer.DataItem == null)
                return;
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowDataViewItemMenus(GetDataMenus(), DataMenuResult.DataMenuSettingName, AreaInitializer.SourceView);
        }
        private void MenuUI_MenuClicked(object sender, EventArgs e)
        {
            //**DataMenuArea.MenuUI_MenuClicked: 28b138367084
            var dataMenuUI = sender as DataMenuUI;
            if (dataMenuUI != null)
            {
                var dataMenu = dataMenuUI.DataMenu;
                if (dataMenu.Type == DataMenuType.Archive)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowArchiveArea(dataMenuUI.DataItem.TargetEntityID, "آرشیو", true, dataMenuUI.DataItem);
                }
                else if (dataMenu.Type == DataMenuType.DataItemReport)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowDataItemReport(dataMenu.ReportID, true, dataMenuUI.DataItem);
                }
                //else if (dataMenu.Type == DataMenuType.ViewRel)
                //{
                //    var menuInitializer = new DataMenuAreaInitializer(0);
                //    menuInitializer.HostDataViewArea = AreaInitializer.HostDataViewArea;
                //    menuInitializer.HostDataViewItem = AreaInitializer.HostDataViewItem;
                //    menuInitializer.SourceView = AreaInitializer.SourceView;
                //    menuInitializer.DataItem = dataMenu.ViewRelTargetDataItem;
                //    AgentUICoreMediator.GetAgentUICoreMediator.ShowMenuArea(menuInitializer);
                //}
                else if (dataMenu.Type == DataMenuType.Form)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowEditEntityArea(dataMenuUI.DataItem.TargetEntityID, true, CommonDefinitions.UISettings.DataMode.None, new List<DP_BaseData>() { dataMenuUI.DataItem });
                }
                else if (dataMenu.Type == DataMenuType.Letter)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowLetterArea(dataMenuUI.DataItem.TargetEntityID, "نامه ها", true, dataMenuUI.DataItem);
                }
                else if (dataMenu.Type == DataMenuType.RelationshipTailSearchableReport)
                {
                    var searchItem = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(),
                        dataMenuUI.DataItem, dataMenu.RelationshipTailID);
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowSearchableReportArea(dataMenu.ReportID, true, searchItem, false, true, AreaInitializer.HostDataViewArea, AreaInitializer.HostDataViewItem);
                }
                else if (dataMenu.Type == DataMenuType.Workflow)
                {
                    var initializer = new WorkflowArea.WorkflowReportAreaInitializer();
                    initializer.DataItem = dataMenuUI.DataItem;
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowWorkflowReportArea(initializer, "گزارش جریان کار", true);
                }
            }
        }




        //private DataMenu GetOrCreateMenu1(List<DataMenu> collection, string key)
        //{
        //    var fITem = collection.FirstOrDefault(x => x.Title == key);
        //    if (fITem == null)
        //    {
        //        fITem = new DataMenu();
        //        fITem.Type = DataMenuType.Folder;
        //        fITem.Title = key;
        //        collection.Add(fITem);
        //        return fITem;
        //    }
        //    else
        //        return fITem;
        //}

    }
}
