using ModelEntites;
using MyUILibrary.Temp;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{
    public class ChildSimpleContorlProperty
    {
        public SimpleColumnControlGenerel SimpleColumnControl { set; get; }
        public EntityInstanceProperty Property { set; get; }
        public List<PropertyFormulaComment> PropertyFormulaCommentItems = new List<PropertyFormulaComment>();
        public DP_FormDataRepository SourceData { set; get; }
        public bool LableIsShared { get { return SourceData.EditEntityArea is I_EditEntityAreaMultipleData; } }

        public List<ControlStateItem> ControlReadonlyStateItems = new List<ControlStateItem>();
        public List<ControlStateItem> ControlHiddenStateItems = new List<ControlStateItem>();


        public I_UIControlManager GetUIControlManager
        {
            get
            {
                if (SimpleColumnControl is SimpleColumnControlOne)
                    return (SimpleColumnControl as SimpleColumnControlOne).SimpleControlManager.GetUIControlManager();
                else if (SimpleColumnControl is SimpleColumnControlMultiple)
                    return (SimpleColumnControl as SimpleColumnControlMultiple).SimpleControlManager.GetUIControlManager(SourceData);
                return null;
            }

        }

        //public bool IsReadonlyFromState { set; get; }
        //public bool IsReadonly
        //{
        //    get
        //    {
        //        return SimpleColumnControl.Column.IsReadonly || IsReadonlyFromState;
        //    }
        //}

        public ChildSimpleContorlProperty(SimpleColumnControlGenerel simpleColumnControl, DP_FormDataRepository sourceData, EntityInstanceProperty property) 
        {
            SourceData = sourceData;
               SimpleColumnControl = simpleColumnControl;
            Property = property;
            if (SimpleColumnControl.Column.ColumnValueRange != null && SimpleColumnControl.Column.ColumnValueRange.Details.Any())
            {
                GetUIControlManager.SetColumnValueRange(SimpleColumnControl.Column.ColumnValueRange.Details);
            }
            //CheckColumnReadonly();
        }

        public void SetBinding()
        {
            GetUIControlManager.SetBinding(this.Property);

            //این اضافیه چون تو وضعیها بصورت داینامیک فقط تعیین میشه و اونجا هم ریست میشه اما جهت محکم کاری بد نیست
            DecideVisiblity();

            CheckColumnReadonly();
            SetMessageAndColor();
        }
        public bool IsHiddenOnState
        {
            get
            {
                return ControlHiddenStateItems.Any();
            }
        }
        public bool IsReadonlyOnState
        {
            get
            {
                return ControlReadonlyStateItems.Any();
            }
        }
        public bool IsReadonly
        {
            get
            {

                return SimpleColumnControl.Column.IsReadonly || ControlReadonlyStateItems.Any();

                //|| (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                //              (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
            }
        }

        //public void SetSimpleColumnHiddenFromState(string message, string key, bool permanent, bool checkInUI)//, ImposeControlState hiddenControlState)
        //{
        //    if (checkInUI)
        //    {
        //        if (SourceData.DataIsInEditMode())
        //        {
        //            AddHiddenState(key, message, permanent, checkInUI);
        //        }
        //    }
        //    else
        //    {
        //        AddHiddenState(key, message, permanent, checkInUI);
        //    }

        //}

        public void AddHiddenState(string key, string message, bool permanent, bool checkInUI)
        {
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.Remove(ControlHiddenStateItems.First(x => x.Key == key));
            ControlHiddenStateItems.Add(new ControlStateItem(key, message, permanent));


            if (checkInUI)
            {
                DecideVisiblity();
                SetMessageAndColor();
            }
        }
        public void RemoveHiddenState(string key, bool checkInUI)
        {
            if (ControlHiddenStateItems.Any(x => x.Key == key))
                ControlHiddenStateItems.RemoveAll(x => x.Key == key);

            if (checkInUI)
            {
                DecideVisiblity();
                SetMessageAndColor();
            }
        }

        private void DecideVisiblity()
        {
            GetUIControlManager.Visiblity(!IsHiddenOnState);
            if (!LableIsShared)
                SimpleColumnControl.LabelControlManager.Visiblity(!IsHiddenOnState);

        }
        //public void ResetSimpleColumnVisiblityFromState(string key)//, ImposeControlState hiddenControlState)
        //{
        //    if (SourceData.DataIsInEditMode())
        //    {
        //        //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده
        //        RemoveHiddenState(key);
        //        DecideVisiblity();
        //    }
        //}
        //public void SetSimpleColumnReadonlyFromState(string message, string key, bool permanent, bool checkInUI)//, ImposeControlState hiddenControlState)
        //{
        //    if (checkInUI)
        //    {
        //        if (SourceData.DataIsInEditMode())
        //        {
        //            AddReadonlyState(key, message, permanent, checkInUI);
        //        }
        //    }
        //    else
        //    {
        //        AddReadonlyState(key, message, permanent, checkInUI);
        //    }
        //}

        public void AddReadonlyState(string key, string message, bool permanent, bool checkInUI)
        {
            if (ControlReadonlyStateItems.Any(x => x.Key == key))
                ControlReadonlyStateItems.Remove(ControlReadonlyStateItems.First(x => x.Key == key));
            ControlReadonlyStateItems.Add(new ControlStateItem(key, message, permanent));

            if (checkInUI)
            {
                CheckColumnReadonly();
                SetMessageAndColor();
            }
        }



        public void RemoveReadonlyState(string key, bool checkInUI)
        {
            if (ControlReadonlyStateItems.Any(x => x.Key == key && x.Permanent == false))
                ControlReadonlyStateItems.RemoveAll(x => x.Key == key && x.Permanent == false);

            if (checkInUI)
            {
                CheckColumnReadonly();
                SetMessageAndColor();
            }
        }

        private void CheckColumnReadonly()
        {
            GetUIControlManager.SetReadonly(IsReadonly);
        }

        //public void ResetSimpleColumnReadonlyFromState(string key)//, ImposeControlState hiddenControlState)
        //{
        //    if (SourceData.DataIsInEditMode())
        //    {
        //        RemoveReadonlyState(key);
        //    }
        //}

        public void SetValue(object value)
        {
            if (!IsReadonly && IsHiddenOnState)
            {
                Property.Value = value;
            }
        }
        List<ColumnValueRangeDetailsDTO> ColumnValueRange { set; get; }
        public void SetColumnValueRangeFromState(List<ColumnValueRangeDetailsDTO> details)
        {
            if (SourceData.DataIsInEditMode())
            {
                if (!IsHiddenOnState && !IsReadonly)
                {
                    ColumnValueRange = details;
                    GetUIControlManager.SetColumnValueRange(details);
                }
            }
        }
        public void ResetColumnValueRangeFromState()
        {
            if (SourceData.DataIsInEditMode())
            {
                if (!IsHiddenOnState && !IsReadonly)
                {
                    ColumnValueRange = null;
                    GetUIControlManager.SetColumnValueRange(SimpleColumnControl.Column.ColumnValueRange.Details);
                }
            }
        }


        public List<I_UIElementManager> GetColumnControlDataManagers(ControlOrLabelAsTarget controlOrLabelAsTarget)
        {
            List<I_UIElementManager> result = new List<I_UIElementManager>();
            if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
            {
                var uiManager = GetUIControlManager;
                if (uiManager != null)
                    result.Add(uiManager);
            }
            else
            {
                result.Add(SimpleColumnControl.LabelControlManager);
            }
            return result;
        }
        public void SetMessageAndColor()
        {
            List<ColumnControlColorItem> columnControlColorItems = new List<ColumnControlColorItem>();
            List<ColumnControlMessageItem> columnControlMessageItems = new List<ColumnControlMessageItem>();


            if (SimpleColumnControl.Column.IsMandatory)
                columnControlColorItems.Add(new ColumnControlColorItem(InfoColor.DarkRed, ControlOrLabelAsTarget.Label, ControlColorTarget.Foreground, "mandatory", ControlItemPriority.Normal));
            if (SimpleColumnControl.Column.IsReadonly)
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



    }
}
