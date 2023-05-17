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
                AddUIMenu(Menus, DataMenuResult.DataMenus);
                return Menus;
            }
            else
                return Menus;
        }
        private void AddUIMenu(List<DataMenuUI> menus, List<DataMenu> dataMenus)
        {
            foreach (var item in dataMenus)
            {
                DataMenuUI menuUI = new DataMenuUI();
                menuUI.DataMenu = item;
                menuUI.Title = item.Title;
                menuUI.Tooltip = item.Tooltip;
                menus.Add(menuUI);
                menuUI.MenuClicked += MenuUI_MenuClicked;
                AddUIMenu(menuUI.SubMenus, item.SubMenus);
            }
        }
        internal void ShowMenu()
        {
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
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowArchiveArea(dataMenu.DataItem.TargetEntityID, "آرشیو", true, dataMenu.DataItem);
                else if (dataMenu.Type == DataMenuType.RelationshipTailDataGrid)
                {
                    var searchDataTuple = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(dataMenu.DataItem, dataMenu.GridviewRelationshipTail);
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowDataViewGridViewArea(dataMenu.GridviewRelationshipTail.TargetEntityID, dataMenu.GridviewRelationshipTail.TargetEntityAlias, true, false, false, searchDataTuple, true, dataMenu.TargetDataMenuSettingID, AreaInitializer.HostDataViewArea, AreaInitializer.HostDataViewItem);
                }
                //else if (dataMenu.Type == DataMenuType.DataLink)
                //{
                //    AgentUICoreMediator.GetAgentUICoreMediator.ShowDataLinkArea(dataMenu.DataItem.TargetEntityID, dataMenu.Datalink.ID, true, dataMenu.Datalink.ReportTitle, dataMenu.DataItem);
                //}
                //else if (dataMenu.Type == DataMenuType.Graph)
                //{
                //    AgentUICoreMediator.GetAgentUICoreMediator.ShowGraphArea(dataMenu.DataItem.TargetEntityID, dataMenu.Datalink.ID, true, dataMenu.Datalink.ReportTitle, dataMenu.DataItem);
                //}
                else if (dataMenu.Type == DataMenuType.RelationshipTailDataView)
                {
                    var searchDataTuple = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(dataMenu.DataItem, dataMenu.DataviewRelationshipTail);
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowDataViewGridViewArea(dataMenu.DataviewRelationshipTail.TargetEntityID, dataMenu.DataviewRelationshipTail.TargetEntityAlias, true, false, true, searchDataTuple, true, dataMenu.TargetDataMenuSettingID, AreaInitializer.HostDataViewArea, AreaInitializer.HostDataViewItem);
                }
                else if (dataMenu.Type == DataMenuType.DataItemReport)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowDataItemReport(dataMenu.DataItemReport.ID, true, dataMenu.DataItem);
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
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowEditEntityArea(dataMenu.DataItem.TargetEntityID, true, CommonDefinitions.UISettings.DataMode.None, new List<DP_BaseData>() { dataMenu.DataItem });
                }
                else if (dataMenu.Type == DataMenuType.Letter)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowLetterArea(dataMenu.DataItem.TargetEntityID, "نامه ها", true, dataMenu.DataItem);
                }
                else if (dataMenu.Type == DataMenuType.RelationshipTailSearchableReport)
                {
                    var searchItem = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(dataMenu.DataItem, dataMenu.SearchableReportRelationshipTail.RelationshipTail);
                    AgentUICoreMediator.GetAgentUICoreMediator.ShowSearchableReportArea(dataMenu.SearchableReportRelationshipTail.EntitySearchableReportID, true, searchItem, false, true, AreaInitializer.HostDataViewArea, AreaInitializer.HostDataViewItem);
                }
                else if (dataMenu.Type == DataMenuType.Workflow)
                {
                    var initializer = new WorkflowArea.WorkflowReportAreaInitializer();
                    initializer.DataItem = dataMenu.DataItem;
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
