using CommonDefinitions.UISettings;
using MyUILibrary.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{

    public abstract class BaseChildProperty
    {
        public List<PropertyFormulaComment> PropertyFormulaCommentItems = new List<PropertyFormulaComment>();
        public DP_FormDataRepository SourceData { set; get; }
        public abstract List<I_UIElementManager> GetColumnControlDataManagers(ControlOrLabelAsTarget controlOrLabelAsTarget);
        public BaseColumnControl BaseColumnControl { set; get; }
        public bool LableIsShared { get { return SourceData.EditEntityArea is I_EditEntityAreaMultipleData; } }
      
        public List<ControlStateItem> ControlReadonlyStateItems = new List<ControlStateItem>();
        public List<ControlStateItem> ControlHiddenStateItems = new List<ControlStateItem>();

        public BaseChildProperty(DP_FormDataRepository sourceData, BaseColumnControl baseColumnControl)
        {
            SourceData = sourceData;
            BaseColumnControl = baseColumnControl;
            //      SetMessageAndColor();
        }

        public void SetMessageAndColor()
        {
            List<ColumnControlColorItem> columnControlColorItems = new List<ColumnControlColorItem>();
            List<ColumnControlMessageItem> columnControlMessageItems = new List<ColumnControlMessageItem>();

            if (this is ChildRelationshipInfo)
            {
                if ((BaseColumnControl as RelationshipColumnControlGeneral).Relationship.IsOtherSideMandatory)
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Label, ControlColorTarget.Foreground, "mandatory", ControlItemPriority.Normal));
                if ((BaseColumnControl as RelationshipColumnControlGeneral).Relationship.IsReadonly)
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "relationReadonly", ControlItemPriority.Normal));
                    columnControlMessageItems.Add(new ColumnControlMessageItem("این رابطه فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد", ControlOrLabelAsTarget.Control, "relationReadonly", ControlItemPriority.Normal));
                }
                foreach (var item in ControlHiddenStateItems)
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.Red, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, item.Key, ControlItemPriority.High));
                    columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.High));
                }
                foreach (var item in ControlReadonlyStateItems)
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, item.Key, ControlItemPriority.High));
                    columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message + Environment.NewLine + "این رابطه فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد", ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.High));
                }
                SetItemColor(columnControlColorItems);
                SetItemMessage(columnControlMessageItems);
            }
            else if (this is ChildSimpleContorlProperty)
            {
                if ((BaseColumnControl as SimpleColumnControlGenerel).Column.IsMandatory)
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Label, ControlColorTarget.Foreground, "mandatory", ControlItemPriority.Normal));
                if ((BaseColumnControl as SimpleColumnControlGenerel).Column.IsReadonly)
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, "columnReadonly", ControlItemPriority.Normal));
                    columnControlMessageItems.Add(new ColumnControlMessageItem("این فیلد فقط خواندنی می باشد و تغییرات رابطه اعمال نخواهد شد", ControlOrLabelAsTarget.Control, "columnReadonly", ControlItemPriority.Normal));
                }
                foreach (var item in ControlHiddenStateItems)
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.Red, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, item.Key, ControlItemPriority.High));
                    columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.High));
                }
                foreach (var item in ControlReadonlyStateItems)
                {
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, item.Key, ControlItemPriority.High));
                    columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message + Environment.NewLine + "این فیلد فقط خواندنی می باشد و تغییرات اعمال نخواهد شد", ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.High));
                }

                foreach (var item in PropertyFormulaCommentItems)
                {
                    columnControlMessageItems.Add(new ColumnControlMessageItem(item.Message, ControlOrLabelAsTarget.Control, item.Key, ControlItemPriority.Normal));
                }

                SetItemColor(columnControlColorItems);
                SetItemMessage(columnControlMessageItems);
            }
        }



        private void SetItemColor(List<ColumnControlColorItem> columnControlColorItems)
        {
            SetItemColor(ControlOrLabelAsTarget.Control, columnControlColorItems);
            SetItemColor(ControlOrLabelAsTarget.Label, columnControlColorItems);
        }
        public void SetItemColor(ControlOrLabelAsTarget controlOrLabel, List<ColumnControlColorItem> columnControlColorItems)
        {

            var colorBackground = GetColor(controlOrLabel, ControlColorTarget.Background, columnControlColorItems);
            var colorForeground = GetColor(controlOrLabel, ControlColorTarget.Foreground, columnControlColorItems);
            var colorBorder = GetColor(controlOrLabel, ControlColorTarget.Border, columnControlColorItems);


            var controlManagers = GetColumnControlDataManagers(controlOrLabel);


            foreach (var controlManager in controlManagers)
            {

                controlManager.SetBackgroundColor(colorBackground);
                controlManager.SetForegroundColor(colorForeground);
                controlManager.SetBorderColor(colorBorder);
            }
        }

        private InfoColor GetColor(ControlOrLabelAsTarget controlOrLabel, ControlColorTarget colorTarget, List<ColumnControlColorItem> columnControlColorItems)
        {
            var color = columnControlColorItems.OrderByDescending(x => x.Priority).FirstOrDefault(x => x.ControlOrLabel == controlOrLabel && x.ColorTarget == colorTarget);
            if (color != null)
                return color.Color;
            else
                return InfoColor.Default;
        }

        
        private void SetItemMessage(List<ColumnControlMessageItem> columnControlMessageItems)
        {
            SetItemMessage(ControlOrLabelAsTarget.Control, columnControlMessageItems);
            SetItemMessage(ControlOrLabelAsTarget.Label, columnControlMessageItems);
        }
        public void SetItemMessage(ControlOrLabelAsTarget controlOrLabel, List<ColumnControlMessageItem> columnControlMessageItems)
        {
            var tooltip = GetTooltip(controlOrLabel, columnControlMessageItems);
            var controlManagers = GetColumnControlDataManagers(controlOrLabel);
            foreach (var view in controlManagers)
            {
                view.SetTooltip(tooltip);
            }
        }
        private string GetTooltip(ControlOrLabelAsTarget controlOrLabel, List<ColumnControlMessageItem> columnControlMessageItems)
        {
            var tooltip = "";
            foreach (var item in columnControlMessageItems.Where(x => x.ControlOrLabel == controlOrLabel).OrderByDescending(x => x.Priority))
                tooltip += (tooltip == "" ? "" : Environment.NewLine) + item.Message;
            return tooltip;
        }
       
        public void AddPropertyFormulaComment(string key, string message)
        {
            if (!PropertyFormulaCommentItems.Any(x => x.Key == key))
                PropertyFormulaCommentItems.Add(new PropertyFormulaComment(key, message));

            SetMessageAndColor();
        }
        public void RemovePropertyFormulaComment(string key)
        {
            if (PropertyFormulaCommentItems.Any(x => x.Key == key))
                PropertyFormulaCommentItems.RemoveAll(x => x.Key == key);

            SetMessageAndColor();
        }


        private List<DataColorItem> DataItemColorItems = new List<DataColorItem>();
        private List<DataMessageItem> DataItemMessageItems = new List<DataMessageItem>();

    }
   
}
