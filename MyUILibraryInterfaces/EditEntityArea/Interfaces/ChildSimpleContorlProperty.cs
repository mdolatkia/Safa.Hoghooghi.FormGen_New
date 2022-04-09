using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{
    public class ChildSimpleContorlProperty : BaseChildProperty
    {
        public SimpleColumnControl SimpleColumnControl { set; get; }
        public EntityInstanceProperty Property { set; get; }

        public bool IsHidden { get; set; }
        public bool IsReadonlyFromState { set; get; }
        public bool IsReadonly
        {
            get
            {
                return SimpleColumnControl.Column.IsReadonly || IsReadonlyFromState;
            }
        }

        public ChildSimpleContorlProperty(SimpleColumnControl simpleColumnControl, DP_FormDataRepository sourceData, EntityInstanceProperty property) : base(sourceData, simpleColumnControl)
        {
            SourceData = sourceData;
            SimpleColumnControl = simpleColumnControl;
            Property = property;
        }


        public void ChangeSimpleColumnVisiblityFromState(bool hidden, string message, string key)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده

                IsHidden = hidden;
                //if (hiddenControlState == ImposeControlState.Impose || hiddenControlState == ImposeControlState.Both)
                //{
                SimpleColumnControl.ControlManager.Visiblity(SourceData, !hidden);
                //}

                //if (hiddenControlState == ImposeControlState.AddMessageColor || hiddenControlState == ImposeControlState.Both)
                //{
                //if (!hidden)
                //{
                //    RemoveColumnControlColor(simpleColumn, ControlOrLabelAsTarget.Label, dataItem, key);
                //    RemoveColumnControlMessage(simpleColumn, ControlOrLabelAsTarget.Label, dataItem, key);
                //    RemoveColumnControlColor(simpleColumn, ControlOrLabelAsTarget.Control, dataItem, key);
                //    RemoveColumnControlMessage(simpleColumn, ControlOrLabelAsTarget.Control, dataItem, key);
                //}
                //else
                //{
                //    AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Control) { CausingDataItem = dataItem, Color = InfoColor.Red, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                //    AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Control) { CausingDataItem = dataItem, Color = InfoColor.Red, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });
                //    AddColumnControlMessage(new ColumnControlMessageItem(simpleColumn, ControlOrLabelAsTarget.Control) { CausingDataItem = dataItem, Message = message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", Key = key, Priority = ControlItemPriority.High });
                //    AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Label) { CausingDataItem = dataItem, Color = InfoColor.Red, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                //    AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Label) { CausingDataItem = dataItem, Color = InfoColor.Red, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });
                //    AddColumnControlMessage(new ColumnControlMessageItem(simpleColumn, ControlOrLabelAsTarget.Label) { CausingDataItem = dataItem, Message = message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", Key = key, Priority = ControlItemPriority.High });
                //}
                //}
            }
        }

        public void ChangeSimpleColumnReadonlyFromState(bool isReadonly, string message, string key)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                IsReadonlyFromState = isReadonly;
                if (!SimpleColumnControl.Column.IsReadonly)
                {
                    //if (hiddenControlState == ImposeControlState.Impose || hiddenControlState == ImposeControlState.Both)
                    //{
                    SimpleColumnControl.SimpleControlManager.SetReadonly(SourceData, isReadonly);
                    //}

                    //if (hiddenControlState == ImposeControlState.AddMessageColor || hiddenControlState == ImposeControlState.Both)
                    //{
                    if (isReadonly)
                    {
                        AddColumnControlMessage(message + Environment.NewLine + "این فیلد فقط خواندنی می باشد و تغییرات فیلد اعمال نخواهد شد", ControlOrLabelAsTarget.Control, key, ControlItemPriority.High);

                        // AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Control) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                        //   AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Control) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });
                        if (this is I_EditEntityAreaOneData)
                        {
                            //     AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Label) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                            //     AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Label) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });
                            AddColumnControlMessage(message + Environment.NewLine + "این فیلد فقط خواندنی می باشد و تغییرات فیلد اعمال نخواهد شد", ControlOrLabelAsTarget.Label, key, ControlItemPriority.High);
                        }
                    }
                    else
                    {
                        RemoveColumnControlMessage(ControlOrLabelAsTarget.Control, key);
                        RemoveColumnControlMessage(ControlOrLabelAsTarget.Label, key);
                        // RemoveColumnControlColor(simpleColumn, ControlOrLabelAsTarget.Control, dataItem, key);
                        //  RemoveColumnControlColor(simpleColumn, ControlOrLabelAsTarget.Label, dataItem, key);
                    }
                    // }
                }
            }
        }


        public void SetValue(object value)
        {
            if (!IsReadonly && IsHidden)
            {
                Property.Value = value;
            }
        }
        List<ColumnValueRangeDetailsDTO> ColumnValueRange { set; get; }
        public void SetColumnValueRangeFromState(List<ColumnValueRangeDetailsDTO> details)
        {
            if (SourceData.DataIsInEditMode())
            {
                if (!IsHidden && !IsReadonly)
                {
                    ColumnValueRange = details;
                    SimpleColumnControl.SimpleControlManager.SetColumnValueRange(details, SourceData);
                }
            }
        }
        public void ResetColumnValueRangeFromState()
        {
            if (SourceData.DataIsInEditMode())
            {
                if (!IsHidden && !IsReadonly)
                {
                    ColumnValueRange = null;
                    SimpleColumnControl.SimpleControlManager.SetColumnValueRange(SimpleColumnControl.Column.ColumnValueRange.Details, SourceData);
                }
            }
        }
    }
}
