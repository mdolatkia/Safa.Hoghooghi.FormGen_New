
//using MyUIGenerator.UIContainerHelper;
using ModelEntites;
using MyUIGenerator.UIControlHelper;
using MyUILibrary;
using MyUILibrary.EntityArea;
using MyUILibrary.Temp;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace MyUIGenerator.UIControlHelper
{
    public class View_MultipleDataContainer : View_Container, I_View_MultipleDataContainer
    {
        //  public DataGridHelper dataGridHelper;
        public RadGridView dataGrid;
        public event EventHandler<DataContainerLoadedArg> DataContainerLoaded;

        bool _MultipleSelection;
        public bool MultipleSelection
        {
            get
            {
                return _MultipleSelection;
            }

            set
            {
                _MultipleSelection = value;
                if (_MultipleSelection)
                    dataGrid.SelectionMode = SelectionMode.Extended;
                else
                    dataGrid.SelectionMode = SelectionMode.Single;
            }
        }

        public View_MultipleDataContainer()
        {
            dataGrid = new RadGridView();
            dataGrid.EnableColumnVirtualization = false;
            dataGrid.EnableRowVirtualization = false;
            dataGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            dataGrid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.ShowGroupPanel = false;
            //dataGrid.AddingNewDataItem += dataGrid_AddingNewDataItem;
            //     dataGrid.
            dataGrid.RowLoaded += dataGrid_RowLoaded;

            //dataGridHelper = new DataGridHelper();
            //dataGridHelper.DataContainerLoaded += DataGridHelper_DataContainerLoaded;
            dataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
        }
        void dataGrid_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.DataElement != null)
                if (DataContainerLoaded != null)
                    DataContainerLoaded(this, new DataContainerLoadedArg() { DataItem = e.DataElement });
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                if (ItemDoubleClicked != null)
                    ItemDoubleClicked(this, new DataContainerLoadedArg() { DataItem = dataGrid.SelectedItem });
            }
        }


        //public CommonDefinitions.BasicUISettings.GridSetting GridSetting
        //{
        //    get;
        //    set;
        //}

        //public void AddUIControlPackage(object controlPackage, string title, InfoColor titleColor, string tooltip = "")
        //{
        //    //////dataGridHelper.AddControlToLayout(controlPackage as UIControlPackageMultipleData, title, titleColor, tooltip);
        //}




        //private List<RadTabControl> TabControls = new List<RadTabControl>();








        //public bool ShowMultipleDateItemControlValue(DP_DataRepository dataItem, UIControlPackageMultipleData controlPackage, string value)
        //{
        //    return false;
        //    //////return dataGridHelper.SetValue(dataItem, controlPackage, value, columnSetting);
        //}
        //public string FetchMultipleDateItemControlValue(DP_DataRepository dataItem, UIControlPackageMultipleData controlPackage)
        //{
        //    return dataGridHelper.GetValue(dataItem, controlPackage);
        //}











        public event EventHandler<Arg_DataContainer> DataCotainerIsReady;
        public event EventHandler<DataContainerLoadedArg> ItemDoubleClicked;


        //public void RemoveDataContainers(object data)
        //{
        //    //if (Controller.EditTemplate.Template.TableDrivedEntity.Table.BatchDataEntry == true)
        //    //{
        //    dataGridHelper.RemoveDataContainers(data);
        //    //}
        //}

        public List<object> GetSelectedData()
        {
            //if (AgentHelper.GetDataEntryMode(EditTemplate) == DataMode.Multiple)
            //{
            List<object> result = new List<object>();
            foreach (var item in dataGrid.SelectedItems)
                result.Add(item as object);
            return result;
            //}
            //else
            //    return null;
        }
        public void SetSelectedData(List<object> dataItems)
        {
            if (dataItems != null)
            {
                ObservableCollection<object> items = new ObservableCollection<object>();
                dataItems.ForEach(x => items.Add(x));
                dataGrid.SelectedItem = items.FirstOrDefault();
            }
            else
                dataGrid.SelectedItem = null;

        }

        public void AddDataContainer(object dataItem)
        {
            if (!dataGrid.Items.Contains(dataItem))
            {
                dataGrid.Items.Add(dataItem);

                foreach (var item in dataGrid.Columns)
                {
                    if (item is SimpleControlManagerForMultipleDataForm)
                        (item as SimpleControlManagerForMultipleDataForm).AddDataItem(dataItem);
                    else if (item is RelationshipControlManagerForMultipleDataForm)
                        (item as RelationshipControlManagerForMultipleDataForm).AddDataItem(dataItem);
                }

            }
            else
                throw (new Exception("zzdfdxf"));
        }


        public List<object> RemoveDataContainers()
        {
            //if (Controller.EditTemplate.Template.TableDrivedEntity.Table.BatchDataEntry == true)
            //{
            List<object> result = new List<object>();
            foreach (var item in dataGrid.Items)
                result.Add(item);
            dataGrid.Items.Clear();
            foreach (var item in dataGrid.Columns)
            {
                if (item is SimpleControlManagerForMultipleDataForm)
                    (item as SimpleControlManagerForMultipleDataForm).RemoveDataItems();
                else if (item is RelationshipControlManagerForMultipleDataForm)
                    (item as RelationshipControlManagerForMultipleDataForm).RemoveDataItems();


            }
            return result;
            //}
        }
        public void RemoveDataContainer(object dataItem)
        {
            dataGrid.Items.Remove(dataItem);
            foreach (var item in dataGrid.Columns)
            {
                if (item is SimpleControlManagerForMultipleDataForm)
                    (item as SimpleControlManagerForMultipleDataForm).RemoveDataItem(dataItem);
                else if (item is RelationshipControlManagerForMultipleDataForm)
                    (item as RelationshipControlManagerForMultipleDataForm).RemoveDataItem(dataItem);

            }
        }


        public void AddUIControlPackage(I_SimpleControlManagerMultiple control, I_UIControlManager labelControlManager)
        {
            (control as SimpleControlManagerForMultipleDataForm).Header = (labelControlManager as LabelHelper).WholeControl;
            dataGrid.Columns.Add((control as SimpleControlManagerForMultipleDataForm));
        }
        public void CleraUIControlPackages()
        {
            dataGrid.Columns.Clear();
        }

        public void AddView(I_UIControlManager labelControlManager, I_RelationshipControlManagerMultiple view)
        {
            //if (!(view as LocalDataGridRelationshipControlManager).RelatedControl.Any())
            //{
            //   DataGridUIControl labelControl = new DataGridUIControl();
            //if (!string.IsNullOrEmpty(title))
            //{

            //     var labelControl = LabelHelper.GenerateLabelControl(title, new ColumnUISettingDTO());


            //////(view as RelationshipControlManagerForMultipleDataForm).RelatedControl.Add(labelControl);
            (view as RelationshipControlManagerForMultipleDataForm).Header = (labelControlManager as LabelHelper).WholeControl;
            //}
            dataGrid.Columns.Add((view as RelationshipControlManagerForMultipleDataForm));
            //if (controlPackage.UIControl.Control is I_View_Container)
            //    (controlPackage.UIControl.Control as I_View_Container).SetExpanderInfo(labelControl.Control);
            //else
            //AddControlToGrid(labelControl);
            //}
            //var uiControl = controlPackage.UIControl;

        }

        public void RemoveUIControlPackage(I_SimpleControlManagerMultiple controlManager)
        {
            dataGrid.Columns.Add((controlManager as SimpleControlManagerForMultipleDataForm));
        }

        public void RemoveView(I_RelationshipControlManagerMultiple controlManager)
        {
            dataGrid.Columns.Add((controlManager as RelationshipControlManagerForMultipleDataForm));
        }

        public override void ClearControls()
        {
            dataGrid.Columns.Clear();
        }
        public int ControlsCount { get { return dataGrid.Columns.Count; } }


        public void SetTooltip(object dataItem, string tooltip)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
             {
                 GridViewRow row = (GridViewRow)dataGrid.ItemContainerGenerator
                                                     .ContainerFromItem(dataItem);
                 if (row != null)
                 {
                     if (!string.IsNullOrEmpty(tooltip))
                         ToolTipService.SetToolTip(row, tooltip);
                     else
                         ToolTipService.SetToolTip(row, null);
                 }
             }));

            //dataGrid.SetTooltip(dataItem, tooltip);
        }
        public void EnableDisable(object dataItem, bool enable)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                GridViewRow row = (GridViewRow)dataGrid.ItemContainerGenerator
                                                    .ContainerFromItem(dataItem);
                if (row != null)
                {
                    row.IsEnabled = enable;
                }
            }));
            //   dataGrid.EnableDisable(dataItem, enable);
        }

        //public void RemoveSelectedDataContainers()
        //{
        //    //if (AgentHelper.GetDataEntryMode( EditTemplate) == DataMode.Multiple)
        //    //{
        //    dataGridHelper.RemoveSelectedDataContainers();
        //    //}
        //}

        //private List<DataMessageItem> rowValidationMessages = new List<DataMessageItem>();
        //public void AddValidation(DataMessageItem item)
        //{
        //    rowValidationMessages.Add(item);
        //    SetTooltip(item.CausingDataItem);
        //    SetColor(item.CausingDataItem);
        //}
        //public void ClearValidation(DP_DataRepository dataitem)
        //{
        //    var validationMessage = rowValidationMessages.Where(x => x.CausingDataItem == dataitem).ToList();
        //    foreach (var item in validationMessage)
        //        validationMessage.Remove(item);

        //    SetTooltip(dataitem);
        //    SetColor(dataitem);
        //}
        //public void ClearValidation()
        //{
        //    List<DP_DataRepository> dataItems = new List<DP_DataRepository>();
        //    foreach (var item in rowValidationMessages)
        //    {
        //        dataItems.Add(item.CausingDataItem);
        //    }
        //    rowValidationMessages.Clear();
        //    foreach (var item in dataItems)
        //    {
        //        SetTooltip(item);
        //        SetColor(item);
        //    }
        //}
        //private void SetColor(object dataItem)
        //{
        //    var color = InfoColor.Black;
        //    var validationMessage = rowValidationMessages.Where(x => x.CausingDataItem == dataItem);
        //    if (validationMessage.Any())
        //        color = validationMessage.Last().Color;
        //    if (color != InfoColor.Black)
        //        dataGridHelper.SetColor(dataItem, color);
        //    else
        //        dataGridHelper.ClearColor(dataItem);
        //}

        //private void SetTooltip(object dataItem)
        //{
        //    var tooltip = "";
        //    var validationMessage = rowValidationMessages.Where(x => x.CausingDataItem == dataItem);
        //    foreach (var item in validationMessage)
        //        tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
        //    if (tooltip != "")
        //        dataGridHelper.SetTooltip(dataItem, tooltip);
        //    else
        //        dataGridHelper.ClearTooltip(dataItem);
        //}




        //public void SetColor(object dataItem, InfoColor color)
        //{
        //    dataGridHelper.SetBorderColor(dataItem, color);
        //}

        //public void SetBackgroundColor(object dataItem, InfoColor color)
        //{
        //    dataGridHelper.SetBackgroundColor(dataItem, color);
        //}

        //public void SetForegroundColor(object dataItem, InfoColor color)
        //{
        //    dataGridHelper.SetForegroundColor(dataItem, color);
        //}

        //public void Visiblity(object dataItem, bool visible)
        //{
        //    dataGridHelper.Visiblity(dataItem, visible);
        //}

    }

    //public override void SetTooltip(object dataItem, string tooltip)
    //{
    //    throw new NotImplementedException();
    //}

    //public override void SetColor(object dataItem, InfoColor color)
    //{
    //    throw new NotImplementedException();
    //}



    public class DataGridUIControl
    {
        public DataGridUIControl()
        {

        }
        public ColumnUISettingDTO ColumnSetting { get; internal set; }
        public FrameworkElement Control { set; get; }
    }

    public class DataGridUIrelationshipControl
    {
        public DataGridUIrelationshipControl()
        {

        }
        public RelationshipUISettingDTO RelationshipSetting { get; internal set; }
        public FrameworkElement Control { set; get; }
    }
}
