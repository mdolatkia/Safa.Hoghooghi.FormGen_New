using CommonDefinitions.UISettings;
using MyUILibrary.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{

    public class BaseChildProperty
    {
        public DP_FormDataRepository SourceData { set; get; }
        public BaseColumnControl BaseColumnControl { set; get; }
        private List<ColumnControlColorItem> ColumnControlColorItems = new List<ColumnControlColorItem>();
        private List<ColumnControlMessageItem> ColumnControlMessageItems = new List<ColumnControlMessageItem>();

        public BaseChildProperty(DP_FormDataRepository sourceData, BaseColumnControl baseColumnControl)
        {
            SourceData = sourceData;
            BaseColumnControl = baseColumnControl;
        }

        public void AddColumnControlColor(InfoColor infoColor, ControlOrLabelAsTarget controlOrLabelAsTarget, ControlColorTarget controlColorTarget, string key, ControlItemPriority priority)
        {
            if (!ColumnControlColorItems.Any(x => x.ControlOrLabel == controlOrLabelAsTarget && x.Key == key && x.ColorTarget == controlColorTarget))
                ColumnControlColorItems.Add(new ColumnControlColorItem(infoColor, controlOrLabelAsTarget, controlColorTarget, key, priority));
            SetItemColor(controlOrLabelAsTarget, controlColorTarget);

        }
        public void RemoveColumnControlColor(ControlOrLabelAsTarget ControlOrLabel, string key)
        {
            foreach (var item in ColumnControlColorItems.Where(x => x.ControlOrLabel == ControlOrLabel && x.Key == key).ToList())
            {
                ColumnControlColorItems.Remove(item);
            }
            SetItemColor(ControlOrLabel, ControlColorTarget.Background);
            SetItemColor(ControlOrLabel, ControlColorTarget.Border);
        }

        private void SetItemColor(ControlOrLabelAsTarget controlOrLabelAsTarget, ControlColorTarget controlColorTarget)
        {

            InfoColor color = InfoColor.Null;

            var list = ColumnControlColorItems.Where(x => x.ControlOrLabel == controlOrLabelAsTarget
            && x.ColorTarget == controlColorTarget).ToList<BaseColorItem>();
            color = GetColor(list);
            var controlManagers = GetColumnControlDataManagers(controlOrLabelAsTarget);

            //     var list = ControlManagerColorItems.Where(x => x.CausingDataItem == baseColorItem.CausingDataItem && x.ColorTarget == baseColorItem.ColorTarget).ToList<BaseColorItem>();

            foreach (var view in controlManagers)
            {
                if (controlColorTarget == ControlColorTarget.Background)
                {
                    view.SetBackgroundColor(SourceData, color);
                }
                else if (controlColorTarget == ControlColorTarget.Foreground)
                {
                    view.SetForegroundColor(SourceData, color);
                }
                if (controlColorTarget == ControlColorTarget.Border)
                {
                    view.SetBorderColor(SourceData, color);
                }
            }
        }

        public void AddColumnControlMessage(string message, ControlOrLabelAsTarget controlOrLabelAsTarget, string key, ControlItemPriority priority)
        {
            if (!ColumnControlMessageItems.Any(x => x.ControlOrLabel == controlOrLabelAsTarget && x.Key == key))
                ColumnControlMessageItems.Add(new ColumnControlMessageItem(message, controlOrLabelAsTarget, key, priority));
            SetItemMessage(controlOrLabelAsTarget);
        }
        public void RemoveColumnControlMessage(ControlOrLabelAsTarget ControlOrLabel, string key)
        {
            foreach (var item in ColumnControlMessageItems.Where(x => x.ControlOrLabel == ControlOrLabel && x.Key == key).ToList())
            {
                ColumnControlMessageItems.Remove(item);
            }
            SetItemMessage(ControlOrLabel);
        }
        private void SetItemMessage(ControlOrLabelAsTarget controlOrLabel)
        {
            string tooltip = "";
            var list = ColumnControlMessageItems.Where(x => x.ControlOrLabel == controlOrLabel).ToList<BaseMessageItem>();
            tooltip = GetTooltip(list);

            var controlManagers = GetColumnControlDataManagers(controlOrLabel);

            foreach (var view in controlManagers)
            {
                view.SetTooltip(SourceData, tooltip);
            }

        }
        private string GetTooltip(List<BaseMessageItem> MessageItems)
        {
            var tooltip = "";
            foreach (var item in MessageItems.OrderBy(x => x.Priority))
                tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
            return tooltip;
        }
        private List<I_DataControlManager> GetColumnControlDataManagers(ControlOrLabelAsTarget controlOrLabelAsTarget)
        {
            List<I_DataControlManager> result = new List<I_DataControlManager>();
            if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
            {
                if (BaseColumnControl is SimpleColumnControl)
                {
                    result.Add((BaseColumnControl as SimpleColumnControl).ControlManager);
                }
                else if (BaseColumnControl is RelationshipColumnControl)
                {
                    var relationshipControl = (BaseColumnControl as RelationshipColumnControl);
                    result.Add((BaseColumnControl as RelationshipColumnControl).ControlManager);

                    bool hasTempView = (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
          relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
           relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.Select);
                    if (hasTempView)
                    {
                        if (relationshipControl.EditNdTypeArea.DataView != null)
                            if (relationshipControl.EditNdTypeArea.DataView.IsOpenedTemporary)
                                result.Add(relationshipControl.EditNdTypeArea.DataView);
                    }

                }
            }
            else
            {
                if (BaseColumnControl is SimpleColumnControl)
                {
                    result.Add((BaseColumnControl as SimpleColumnControl).ControlManager.LabelControlManager);
                }
                else if (BaseColumnControl is RelationshipColumnControl)
                {
                    result.Add((BaseColumnControl as RelationshipColumnControl).ControlManager.LabelControlManager);
                }
            }
            return result;
        }
        private InfoColor GetColor(List<BaseColorItem> list)
        {
            var color = InfoColor.Null;
            foreach (var item in list.Where(x => x.Color != InfoColor.Null).OrderByDescending(x => x.Priority))
                color = item.Color;
            return color;
        }
        private List<DataColorItem> DataItemColorItems = new List<DataColorItem>();
        private List<DataMessageItem> DataItemMessageItems = new List<DataMessageItem>();

    }
}
