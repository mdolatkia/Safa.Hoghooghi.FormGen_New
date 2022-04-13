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
        public SimpleColumnControlGenerel SimpleColumnControl { set; get; }
        public EntityInstanceProperty Property { set; get; }

        I_UIControlManager GetUIControlManager
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
            SimpleColumnControl = simpleColumnControl;
            Property = property;


            CheckColumnReadonly();
        }
        public void Binded()
        {
            CheckColumnReadonly();
        }
        private void CheckColumnReadonly()
        {
            if (SourceData.DataIsInEditMode())
                GetUIControlManager.SetReadonly(SimpleColumnControl.Column.IsReadonly);
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

        public void SetSimpleColumnHiddenFromState(string message, string key, bool permanent)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده
                AddHiddenState(key, message, permanent);

                if (permanent)
                {
                    GetUIControlManager.Visiblity(false);
                    if (!LableIsShared)
                        SimpleColumnControl.LabelControlManager.Visiblity(true);
                }

            }
        }
        public void ResetSimpleColumnVisiblityFromState(string key)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                //اگر کلیذد اصلی باشه چک میشه؟ خطا باید بده
                GetUIControlManager.Visiblity(true);
                if (!LableIsShared)
                    SimpleColumnControl.LabelControlManager.Visiblity(true);

                RemoveHiddenState(key);
            }
        }
        public void SetSimpleColumnReadonlyFromState(string message, string key, bool permanent)//, ImposeControlState hiddenControlState)
        {
            if (SourceData.DataIsInEditMode())
            {
                AddReadonlyState(key, message, permanent);
                if (permanent)
                {
                    CheckColumnReadonly();
                }

            }
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
