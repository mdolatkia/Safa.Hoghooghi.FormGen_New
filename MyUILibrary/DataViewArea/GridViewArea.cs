﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using ModelEntites;

using MyUILibraryInterfaces.DataMenuArea;
using MyUILibraryInterfaces.EntityArea;

using MyUILibraryInterfaces.DataViewArea;
using MyUILibrary.DataViewArea;

namespace MyUILibrary.DataViewArea
{
    public class GridViewArea : DataArea, I_GridViewArea
    {
        public object MainView { set; get; }
        public void SetAreaInitializerSpecialized(DataViewAreaInitializer initParam)
        {
            // GridViewArea.SetAreaInitializerSpecialized: dbedc44acc58
            //GridViewSetting = AgentUICoreMediator.GetAgentUICoreMediator.GridViewManager.GetGridViewSetting(initParam.EntityID);
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetViewOfGridViewArea();
            View.InfoClicked += View_InfoClicked;
        }
        public new I_View_GridViewArea View
        {
            set; get;
        }
        //public GridViewSettingDTO GridViewSetting
        //{
        //    set; get;
        //}
        //bool UICompositionsCalled;
        //EntityUICompositionCompositeDTO _UICompositions;
        //public EntityUICompositionCompositeDTO UICompositions
        //{
        //    get
        //    {
        //        if (_UICompositions == null && !UICompositionsCalled)
        //        {
        //            UICompositionsCalled = true;
        //            _UICompositions = AgentUICoreMediator.GetAgentUICoreMediator.entityUICompositionService.GetEntityUICompositionTree(AreaInitializer.EntityID);
        //        }
        //        return _UICompositions;
        //    }
        //}
        //private ColumnUISettingDTO GetColumnUISetting(ColumnDTO column)
        //{
        //    if (UICompositions != null && UICompositions.ColumnItems != null
        //        && UICompositions.ColumnItems.Any(x => x.ColumnID == column.ID))
        //    {
        //        var setting = UICompositions.ColumnItems.First(x => x.ColumnID == column.ID);
        //        if (setting == null)
        //        {
        //            setting = new ColumnUISettingDTO();
        //            setting.UIColumnsType = Enum_UIColumnsType.Normal;
        //            setting.UIRowsCount = 1;
        //            setting.ColumnID = column.ID;
        //            UICompositions.ColumnItems.Add(setting);
        //        }
        //        return setting;
        //    }
        //    return null;
        //}
        public List<SimpleViewColumnControl> ViewColumnControls = new List<SimpleViewColumnControl>();

        int lastListView = 0;
        private void ManageView()
        {
            if (SelectedListView != null && (SelectedListView.ID != lastListView || lastListView == 0))
            {
                lastListView = SelectedListView.ID;
                ViewColumnControls.Clear();
                View.ClearControls();
                foreach (var column in SelectedListView.EntityListViewAllColumns.OrderBy(x => x.OrderID))
                {
                    var propertyControl = new SimpleViewColumnControl(AgentUICoreMediator.GetAgentUICoreMediator.UIManager, column);
                 //   propertyControl.RelativeColumnName = column.RelativeColumnName;

                    //if (string.IsNullOrEmpty(column.Alias))
                    //    propertyControl.Alias = column.Column.Alias;
                    //else
                    //    propertyControl.Alias = column.Alias;
                    //     propertyControl.ControlPackage = new UIControlPackageForSimpleColumn();


                    //      if (propertyControl.IsPermanentReadOnly)
                    if (!string.IsNullOrEmpty(column.Tooltip))
                    {
                        propertyControl.LabelControlManager.SetTooltip(column.Tooltip);
                    }
                    //    propertyControl.ControlManager.GetUIControlManager.SetReadonly(true);
                    ViewColumnControls.Add(propertyControl);
                }
                foreach (var columnControl in ViewColumnControls)
                {
                    //    columnControl.Visited = true;
                    View.AddUIControlPackage(columnControl.ControlManager, columnControl.LabelControlManager);
                }
            }
        }
        private void ShowTypePropertyData(DP_DataView dataRepository)
        {
            foreach (var property in dataRepository.Properties)
            {
                var columnControl = ViewColumnControls.FirstOrDefault(x => x.RelativeColumnName == property.RelativeName);
                if (columnControl != null)
                {
                    columnControl.ControlManager.GetUIControlManager(dataRepository).SetValue(property.Value);
                }
            }
        }
        private void View_InfoClicked(object sender, DataGridSelectedArg e)
        {

            if (e.DataView != null && e.DataView is DP_DataView)
            {
                var dataView = e.DataView as DP_DataView;
                //var menus = GetGridViewItemMenus(GridViewItem);
                //GridViewItem.ShowGridViewItemMenus(menus);
                var menuInitializer = new DataMenuAreaInitializer(AreaInitializer.DataMenuSettingID);
                menuInitializer.SourceView = e.UIElement;
                menuInitializer.HostDataViewArea = this;

                menuInitializer.DataItem = dataView;

                //if (EntityGridView != null)
                //{
                //    var list = new Dictionary<string, EntityRelationshipTailDTO>();
                //    foreach (var item in EntityGridView.EntityGridViewRelationships)
                //        list.Add(item.Group1 ?? "", item.RelationshipTail);
                //    menuInitializer.GridViewRelationshipTails = list;
                //}
                AgentUICoreMediator.GetAgentUICoreMediator.ShowMenuArea(menuInitializer);

            }
        }
        public void SetItems(List<DP_DataView> resultDataItems)
        {
            // GridViewArea.SetItems: 26be2f98a31d
            ManageView();
            View.RemoveDataContainers();
            foreach (var item in resultDataItems)
            {
                View.AddDataContainer(item);
                ShowTypePropertyData(item);
            }
        }
    }
}
