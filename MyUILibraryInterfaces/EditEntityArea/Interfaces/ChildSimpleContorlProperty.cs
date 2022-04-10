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
    public class ChildSimpleContorlProperty : BaseChildProperty
    {
        public SimpleColumnControl SimpleColumnControl { set; get; }
        public EntityInstanceProperty Property { set; get; }


        //public bool IsReadonlyFromState { set; get; }
        //public bool IsReadonly
        //{
        //    get
        //    {
        //        return SimpleColumnControl.Column.IsReadonly || IsReadonlyFromState;
        //    }
        //}

        public ChildSimpleContorlProperty(SimpleColumnControl simpleColumnControl, DP_FormDataRepository sourceData, EntityInstanceProperty property) : base(sourceData, simpleColumnControl)
        {
            SourceData = sourceData;
            SimpleColumnControl = simpleColumnControl;
            Property = property;

            if (SimpleColumnControl.Column.IsMandatory)
                AddColumnControlColor(InfoColor.DarkRed, ControlOrLabelAsTarget.Label, ControlColorTarget.Foreground, "mandatory", ControlItemPriority.Normal);

            CheckColumnReadonly();
        }
        public void Binded()
        {
            CheckColumnReadonly();
        }
        private void CheckColumnReadonly()
        {
            if (SourceData.DataIsInEditMode())
                SimpleColumnControl.SimpleControlManager.SetReadonly(SourceData, SimpleColumnControl.Column.IsReadonly || IsReadonlyOnSHow);
        }

        public bool IsReadonly
        {
            get
            {

                return SimpleColumnControl.Column.IsReadonly || IsReadonlyOnState || IsReadonlyOnSHow;

                //|| (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                //              (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
            }
        }

        public void SetSimpleColumnHiddenFromState(string message, string key, bool OnShow)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده

                IsHiddenOnState = true;

                if (OnShow)
                {
                    IsHiddenOnSHow = true;
                    SimpleColumnControl.ControlManager.Visiblity(SourceData, false);
                }
                else
                {

                   // AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Control, ControlColorTarget.Background, key, ControlItemPriority.High);
                    AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, key, ControlItemPriority.High);
                    AddColumnControlMessage(message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", ControlOrLabelAsTarget.Control, key, ControlItemPriority.High);
                    //if (SourceData.EditEntityArea is I_EditEntityAreaOneData)
                    //{
                    //    AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Label, ControlColorTarget.Background, key, ControlItemPriority.High);
                    //    AddColumnControlColor(InfoColor.Red, ControlOrLabelAsTarget.Label, ControlColorTarget.Border, key, ControlItemPriority.High);
                    //    AddColumnControlMessage(message + Environment.NewLine + "ترتیب اثری به داده نخواهد شد", ControlOrLabelAsTarget.Label, key, ControlItemPriority.High);
                    //}

                }
            }
        }
        public void ResetSimpleColumnVisiblityFromState(string key)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده

                IsHiddenOnState = false;


                SimpleColumnControl.ControlManager.Visiblity(SourceData, true);

                RemoveColumnControlColor(ControlOrLabelAsTarget.Label, key);
                RemoveColumnControlMessage(ControlOrLabelAsTarget.Label, key);
                RemoveColumnControlColor(ControlOrLabelAsTarget.Control, key);
                RemoveColumnControlMessage(ControlOrLabelAsTarget.Control, key);

            }
        }
        public void SetSimpleColumnReadonlyFromState(string message, string key, bool OnShow)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                IsReadonlyOnState = true;
                if (OnShow)
                {
                    IsReadonlyOnSHow = IsReadonlyOnState;
                    if (IsReadonlyOnSHow)
                        CheckColumnReadonly();
                }
                AddColumnControlColor(InfoColor.DarkRed, ControlOrLabelAsTarget.Control, ControlColorTarget.Border, key, ControlItemPriority.High);
                AddColumnControlMessage(message + Environment.NewLine + "این فیلد فقط خواندنی می باشد و تغییرات فیلد اعمال نخواهد شد", ControlOrLabelAsTarget.Control, key, ControlItemPriority.High);

                // AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Control) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                //   AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Control) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });
                //if (this is I_EditEntityAreaOneData)
                //{
                //    //     AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Label) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Background, Key = key, Priority = ControlItemPriority.High });
                //    //     AddColumnControlColor(new ColumnControlColorItem(simpleColumn, ControlOrLabelAsTarget.Label) { CausingDataItem = dataItem, Color = InfoColor.DarkRed, ColorTarget = ControlColorTarget.Border, Key = key, Priority = ControlItemPriority.High });
                //    AddColumnControlMessage(message + Environment.NewLine + "این فیلد فقط خواندنی می باشد و تغییرات فیلد اعمال نخواهد شد", ControlOrLabelAsTarget.Label, key, ControlItemPriority.High);

                // }

            }
        }
        public void ResetSimpleColumnReadonlyFromState(string key)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                IsReadonlyOnState = false;

                RemoveColumnControlMessage(ControlOrLabelAsTarget.Control, key);
                //   RemoveColumnControlMessage(ControlOrLabelAsTarget.Label, key);
                 RemoveColumnControlColor( ControlOrLabelAsTarget.Control,  key);
                //  RemoveColumnControlColor(simpleColumn, ControlOrLabelAsTarget.Label, dataItem, key);

                // }

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
