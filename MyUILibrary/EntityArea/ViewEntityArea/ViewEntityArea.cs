using CommonDefinitions.UISettings;
using ModelEntites;
using MyUILibrary;
using MyUILibrary.EntityArea;
using MyUILibrary.EntityArea.Commands;

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyUILibrary.EntityArea
{
    public class ViewEntityArea : I_ViewEntityArea
    {
        public ViewEntityArea(ViewEntityAreaInitializer initParam)
        {

            ViewInitializer = initParam;


            //if (initParam.TempEntity != null)
            //    _FullEntity = initParam.TempEntity;


        }
        I_View_ViewEntityArea _ViewForViewEntityArea;
        public I_View_ViewEntityArea ViewForViewEntityArea
        {
            //** ViewEntityArea.ViewForViewEntityArea: 14238316d018
            get
            {
                if (_ViewForViewEntityArea == null)
                {
                    ViewColumnControls = new List<SimpleViewColumnControl>();
                    _ViewForViewEntityArea = AgentUICoreMediator.UIManager.GenerateViewOfViewEntityArea();
                    _ViewForViewEntityArea.MultipleSelection = ViewInitializer.MultipleSelection;

                    AddCommands();
                  
                    _ViewForViewEntityArea.DataContainerLoaded += ViewView_DataContainerLoaded;
                    _ViewForViewEntityArea.ItemDoubleClicked += ViewView_ItemDoubleClicked;

                    ManageViewEntityArea();
                }
                return _ViewForViewEntityArea;
            }
        }

        private void AddCommands()
        {
            // ViewEntityArea.AddCommands: 0062609d681f
            ViewCommands = new List<I_ViewAreaCommand>();
            var selectcommand = new SelectCommand(this);
            _ViewForViewEntityArea.AddCommand(selectcommand.CommandManager);
        }

        public ViewEntityAreaInitializer ViewInitializer { set; get; }
        //public I_EditEntityArea SourceEditEntityArea
        //{
        //    set;
        //    get;
        //}




        public List<I_ViewAreaCommand> ViewCommands
        {
            set;
            get;
        }
        public AgentUICoreMediator AgentUICoreMediator
        {
            get
            {
                return AgentUICoreMediator.GetAgentUICoreMediator;
            }
        }


        private void ViewView_ItemDoubleClicked(object sender, DataContainerLoadedArg e)
        {
            if (e.DataItem != null && e.DataItem is DP_DataView)
            {
                OnDataSelected(new List<DP_DataView>() { e.DataItem as DP_DataView });
            }
        }

        private void ViewView_DataContainerLoaded(object sender, DataContainerLoadedArg e)
        {
            //ShowTypePropertyData(e.DataItem as DP_DataView);
        }

        private void ManageViewEntityArea()
        {
            //** ViewEntityArea.ManageViewEntityArea: d2d99496f04e
            foreach (var column in EntityListView.EntityListViewAllColumns.OrderBy(x => x.OrderID))
            {
                var propertyControl = new SimpleViewColumnControl(AgentUICoreMediator.GetAgentUICoreMediator.UIManager, column) ;
                //propertyControl.RelativeColumnName = column.RelativeColumnName;

                //if (string.IsNullOrEmpty(column.Alias))
                //    propertyControl.Alias = column.Column.Alias;
                //else
                //    propertyControl.Alias = column.Alias;
                //   propertyControl.ControlPackage = new UIControlPackageForSimpleColumn();
                //     propertyControl.IsPermanentReadOnly = true;
            //    propertyControl.ControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateSimpleControlManagerForMultipleDataForm(column.Column, column.ColumnUISetting);
            //    propertyControl.LabelControlManager = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateLabelControlManager(propertyControl.Alias);

                //      if (propertyControl.IsPermanentReadOnly)
                if (!string.IsNullOrEmpty(column.Tooltip))
                {
                    propertyControl.LabelControlManager.SetTooltip(column.Tooltip);
                }
                //   propertyControl.ControlManager.GetUIControlManager().SetReadonly(true);

                ViewColumnControls.Add(propertyControl);


            }
            foreach (var columnControl in ViewColumnControls)
            {

                //    columnControl.Visited = true;
                var simpleColumn = (columnControl as SimpleViewColumnControl);
                ViewForViewEntityArea.AddUIControlPackage(simpleColumn.ControlManager, simpleColumn.LabelControlManager);
            }
        }

        public List<SimpleViewColumnControl> ViewColumnControls { set; get; }

        //public event EventHandler<Arg_PackageSelected> DataPackageSelected;
        public event EventHandler<DataViewDataSelectedEventArg> DataSelected;


        public List<DP_DataView> GetSelectedData()
        {
            // ShowTypePropertyData(ViewInitializer.ViewData);

            List<DP_DataView> selectedData = new List<DP_DataView>();
            var viewSelectedData = ViewForViewEntityArea.GetSelectedData();


            if (ViewInitializer.MultipleSelection == false)
            {
                if (viewSelectedData.Count > 0)
                {
                    selectedData.Add(viewSelectedData.First() as DP_DataView);
                }
            }
            else
            //if (packageArea.ViewTemplate.SelectCount > 1)
            {
                selectedData = viewSelectedData.Cast<DP_DataView>().ToList();
            }
            return selectedData;
        }
        public void ClearSelectedData()
        {
            ViewForViewEntityArea.SetSelectedData(null);
        }
        public void AddData(List<DP_DataView> data)
        {
            if (data == null || data.Count == 0)
            {
                //اینجا باید اگر کلید پرشده بود دیتا را بگیرد
                //throw new Exception("sdf");

            }
            ViewInitializer.ViewData.Clear();
            ViewForViewEntityArea.RemoveDataContainers();
            ViewInitializer.ViewData.AddRange(data);

            ShowData1(data);
        }

        public void ShowData1(List<DP_DataView> specificDatas)
        {
            if (specificDatas == null)
                throw new Exception("sdf");
            foreach (var item in specificDatas)
            {
                ViewForViewEntityArea.AddDataContainer(item);
                ShowTypePropertyData(item);
            }

        }
        private void ShowTypePropertyData(DP_DataView dataRepository)
        {

            //foreach (var viewItem in dataRepository.DataViewItems)
            //{
            foreach (var property in dataRepository.Properties)
            {
                var columnControl = ViewColumnControls.FirstOrDefault(x => x.RelativeColumnName == property.RelativeName);
                if (columnControl != null)
                {
                    var uiControl = columnControl.ControlManager.GetUIControlManager(dataRepository);
                    if (uiControl != null)
                        uiControl.SetValue(property.Value);
                }
            }


        }

        EntityListViewDTO _EntityListView;
        public EntityListViewDTO EntityListView
        {
            //** ViewEntityArea.EntityListView: 890dd76ff4e3
            get
            {
                if (_EntityListView == null)
                {
                    //if (ViewInitializer.EntityListViewID != 0)
                    //    _EntityListView = AgentUICoreMediator.GetAgentUICoreMediator.EntityListViewManager.GetEntityListView(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), ViewInitializer.EntityListViewID);
                    //else
                    _EntityListView = AgentUICoreMediator.GetAgentUICoreMediator.EntityListViewManager.GetOrCreateEntityListViewDTO(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), ViewInitializer.EntityID);
                }


                return _EntityListView;
            }
        }

        public I_View_TemporaryView LastTemporaryView { set; get; }
        public bool IsCalledFromDataView { set; get; }

        public void OnDataSelected(List<DP_DataView> dataItems)
        {
            // ViewEntityArea.OnDataSelected: e62db97ed241
            if (DataSelected != null)
                DataSelected(this, new DataViewDataSelectedEventArg(dataItems, IsCalledFromDataView));

        }
    }
}
