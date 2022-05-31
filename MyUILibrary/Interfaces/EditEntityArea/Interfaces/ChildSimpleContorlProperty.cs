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
        public SimpleColumnControlGenerel SimpleColumnControl { get { return BaseColumnControl as SimpleColumnControlGenerel; } }
        public EntityInstanceProperty Property { set; get; }

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

        public ChildSimpleContorlProperty(SimpleColumnControlGenerel simpleColumnControl, DP_FormDataRepository sourceData, EntityInstanceProperty property) : base(sourceData, simpleColumnControl)
        {
            SourceData = sourceData;
            //    SimpleColumnControl = simpleColumnControl;
            Property = property;
            if (SimpleColumnControl.Column.ColumnValueRange != null && SimpleColumnControl.Column.ColumnValueRange.Details.Any())
            {
                GetUIControlManager.SetColumnValueRange(SimpleColumnControl.Column.ColumnValueRange.Details);
            }
            CheckColumnReadonly();
        }

        public void SetBinding()
        {
            CheckColumnReadonly();
            GetUIControlManager.SetBinding(this.Property);
        }

        public bool IsReadonly
        {
            get
            {

                return SimpleColumnControl.Column.IsReadonly || IsReadonlyOnState;

                //|| (Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary &&
                //              (SourceData.EditEntityArea.DataEntryEntity.IsReadonly || SourceData.IsReadonlyBecauseOfState));
            }
        }

        public void SetSimpleColumnHiddenFromState(string message, string key, bool permanent, bool checkInUI)//, ImposeControlState hiddenControlState)
        {
            if (checkInUI)
            {
                if (SourceData.DataIsInEditMode())
                {
                    //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده
                    AddHiddenState(key, message, permanent);
                    DecideVisiblity();
                    //if (permanent)
                    //{

                    //}

                }
            }
            else
            {
                AddHiddenState(key, message, permanent);
            }
        }
        private void DecideVisiblity()
        {
            if (ControlHiddenStateItems.Any())
            {
                GetUIControlManager.Visiblity(false);
                if (!LableIsShared)
                    SimpleColumnControl.LabelControlManager.Visiblity(false);
            }
            else
            {
                GetUIControlManager.Visiblity(true);
                if (!LableIsShared)
                    SimpleColumnControl.LabelControlManager.Visiblity(true);
            }
        }
        public void ResetSimpleColumnVisiblityFromState(string key)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده
                RemoveHiddenState(key);
                DecideVisiblity();
            }
        }
        public void SetSimpleColumnReadonlyFromState(string message, string key, bool permanent, bool checkInUI)//, ImposeControlState hiddenControlState)
        {
            if (checkInUI)
            {
                if (SourceData.DataIsInEditMode())
                {
                    AddReadonlyState(key, message, permanent);
                    //if (permanent)
                    //{
                    CheckColumnReadonly();
                    //}

                }
            }
            else
            {
                AddReadonlyState(key, message, permanent);
            }
        }
        private void CheckColumnReadonly()
        {
            if (SourceData.DataIsInEditMode())
                GetUIControlManager.SetReadonly(SimpleColumnControl.Column.IsReadonly);
        }

        public void ResetSimpleColumnReadonlyFromState(string key)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                RemoveReadonlyState(key);
            }
        }

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


        public override List<I_UIElementManager> GetColumnControlDataManagers(ControlOrLabelAsTarget controlOrLabelAsTarget)
        {
            List<I_UIElementManager> result = new List<I_UIElementManager>();
            if (controlOrLabelAsTarget == ControlOrLabelAsTarget.Control)
            {
                result.Add(GetUIControlManager);
            }
            else
            {
                result.Add(SimpleColumnControl.LabelControlManager);
            }
            return result;
        }


    }
}
