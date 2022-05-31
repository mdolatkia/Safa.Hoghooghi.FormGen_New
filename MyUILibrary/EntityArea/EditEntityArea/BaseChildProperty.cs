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
        //public I_UIControlManager LabelControlManager
        //{
        //    get
        //    {
        //        if (this is ChildRelationshipInfo)
        //        {

        //        }
        //        else if (this is ChildSimpleContorlProperty)
        //        {
        //            return (this as ChildSimpleContorlProperty).SimpleColumnControl.SimpleControlManager.LabelControlManager;
        //        }
        //        return null;
        //    }
        //}

        public List<ControlStateItem> ControlReadonlyStateItems = new List<ControlStateItem>();
        public List<ControlStateItem> ControlHiddenStateItems = new List<ControlStateItem>();

        public BaseChildProperty(DP_FormDataRepository sourceData, BaseColumnControl baseColumnControl)
        {
            SourceData = sourceData;
            BaseColumnControl = baseColumnControl;
      //      SetMessageAndColor();
        }

        public void AddReadonlyState(string key, string message, bool permanent)
        {
            if (ControlReadonlyStateItems.Any(x => x.Key == key))
                ControlReadonlyStateItems.Remove(ControlReadonlyStateItems.First(x => x.Key == key));
            ControlReadonlyStateItems.Add(new ControlStateItem(key, message, permanent));
          
            SetMessageAndColor();
        }



        public void RemoveReadonlyState(string key)
        {
            if (ControlReadonlyStateItems.Any(x => x.Key == key && x.Permanent == false))
                ControlReadonlyStateItems.RemoveAll(x => x.Key == key && x.Permanent == false);
           
            SetMessageAndColor();
        }

       
        public void AddHiddenState(string key, string message, bool permanent)
        {
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.Remove(ControlHiddenStateItems.First(x => x.Key == key));
            ControlHiddenStateItems.Add(new ControlStateItem(key, message, permanent));

       
            SetMessageAndColor();
        }
        public void RemoveHiddenState(string key)
        {
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.RemoveAll(x => x.Key == key);

            
            SetMessageAndColor();
        }

      
        public void SetMessageAndColor()
        {
            List<ColumnControlColorItem> columnControlColorItems = new List<ColumnControlColorItem>();
            List<ColumnControlMessageItem> columnControlMessageItems = new List<ColumnControlMessageItem>();

            if (this is ChildRelationshipInfo)
            {
                if ((BaseColumnControl as RelationshipColumnControlGeneral).Relationship.IsOtherSideMandatory)
                    columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Label, ControlColorTarget.Foreground, "mandatory", ControlItemPriority.Normal));

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


        //public bool IsHidden
        //{
        //    get
        //    {
        //        return IsHiddenOnState || IsHiddenOnSHow;
        //    }
        //}

        public bool IsHiddenOnState
        {
            get
            {
                return ControlHiddenStateItems.Any();
            }
        }
        // public bool IsHiddenOnSHow { set; get; }




        public bool IsReadonlyOnState
        {
            get
            {
                return ControlReadonlyStateItems.Any();
            }
        }
        // public bool IsReadonlyOnSHow { set; get; }



        //public void AddColumnControlColor(InfoColor infoColor, ControlOrLabelAsTarget controlOrLabelAsTarget, ControlColorTarget controlColorTarget, string key, ControlItemPriority priority)
        //{
        //    if (!ColumnControlColorItems.Any(x => x.ControlOrLabel == controlOrLabelAsTarget && x.Key == key && x.ColorTarget == controlColorTarget))
        //        ColumnControlColorItems.Add(new ColumnControlColorItem(infoColor, controlOrLabelAsTarget, controlColorTarget, key, priority));


        //}
        //public void RemoveColumnControlColor(ControlOrLabelAsTarget ControlOrLabel, string key)
        //{
        //    foreach (var item in ColumnControlColorItems.Where(x => x.ControlOrLabel == ControlOrLabel && x.Key == key).ToList())
        //    {
        //        ColumnControlColorItems.Remove(item);
        //    }
        //    SetItemColor(ControlOrLabel, ControlColorTarget.Background);
        //    SetItemColor(ControlOrLabel, ControlColorTarget.Border);
        //}


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

        //public void AddColumnControlMessage(string message, ControlOrLabelAsTarget controlOrLabelAsTarget, string key, ControlItemPriority priority)
        //{
        //    if (!ColumnControlMessageItems.Any(x => x.ControlOrLabel == controlOrLabelAsTarget && x.Key == key))
        //        ColumnControlMessageItems.Add(new ColumnControlMessageItem(message, controlOrLabelAsTarget, key, priority));
        //    SetItemMessage(controlOrLabelAsTarget);
        //}
        //public void RemoveColumnControlMessage(ControlOrLabelAsTarget ControlOrLabel, string key)
        //{
        //    foreach (var item in ColumnControlMessageItems.Where(x => x.ControlOrLabel == ControlOrLabel && x.Key == key).ToList())
        //    {
        //        ColumnControlMessageItems.Remove(item);
        //    }
        //    SetItemMessage(ControlOrLabel);
        //}
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
        //private List<I_UIElementManager> GetColumnControlDataManagers(ControlOrLabelAsTarget controlOrLabelAsTarget)
        //{
        //    List<I_UIElementManager> result = new List<I_UIElementManager>();

        //    if (this is ChildRelationshipInfo)
        //    {
        //        if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
        //        {
        //            var relationshipControl = (BaseColumnControl as RelationshipColumnControlGeneral);


        //            if (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect ||
        //           relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //                result.Add(relationshipControl.RelationshipControlManager.GetDateView(SourceData));
        //            else
        //            {
        //                result.Add(relationshipControl.RelationshipControlManager.GetTemporaryView(SourceData));

        //                if (relationshipControl.EditNdTypeArea.DataView != null && relationshipControl.EditNdTypeArea.DataView.IsOpenedTemporary)


        //            }
        //            if (hasTempView)
        //            {
        //                if (relationshipControl.EditNdTypeArea.DataView != null)
        //                    if (relationshipControl.EditNdTypeArea.DataView.IsOpenedTemporary)
        //                        result.Add(relationshipControl.EditNdTypeArea.DataView);
        //            }
        //            result.Add((BaseColumnControl as RelationshipColumnControl).RelationshipControlManager.GetDateView);

        //            //          bool hasTempView = (relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateInDirect ||
        //            //relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect ||
        //            // relationshipControl.EditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.Select);
        //            //          if (hasTempView)
        //            //          {
        //            //              if (relationshipControl.EditNdTypeArea.DataView != null)
        //            //                  if (relationshipControl.EditNdTypeArea.DataView.IsOpenedTemporary)
        //            //                      result.Add(relationshipControl.EditNdTypeArea.DataView);
        //            //          }


        //        }
        //        else
        //        {
        //            result.Add(LabelControlManager);
        //        }
        //    }
        //    else if (this is ChildSimpleContorlProperty)
        //    {
        //        if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
        //        {
        //            result.Add((this as ChildSimpleContorlProperty).SimpleColumnControl.SimpleControlManager);
        //        }
        //        else
        //        {
        //            result.Add(LabelControlManager);
        //        }
        //    }



        //    return result;
        //}
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
    public class ControlStateItem
    {
        //public ControlStateItem(string key, string message)
        //{
        //    Key = key;
        //    Message = message;
        //}
        public ControlStateItem(string key, string message, bool permanent)
        {
            Key = key;
            Message = message;
            Permanent = permanent;
        }
        public bool Permanent { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }

    }
}
