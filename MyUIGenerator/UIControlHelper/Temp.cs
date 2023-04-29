using MyUILibrary;
using MyUILibrary.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModelEntites;
using MyUILibrary.EntityArea;
using System.Windows.Controls;
using System.Windows.Media;
using ProxyLibrary;

namespace MyUIGenerator.UIControlHelper
{
    //public interface I_BaseControlHelper
    //{


    //}
   
    //public interface I_ControlHelperValueRange
    //{
    //    void SetColumnValueRange( List<ColumnValueRangeDetailsDTO> candidates, bool multiselect);
    //}
    //public interface I_SimpleControlHelper: I_ControlHelper
    //{
    //    UISingleControl GenerateControl(ColumnUISettingDTO columnSetting, List<SimpleSearchOperator> operators = null);
    //}
    //public interface I_KeyValueControlHelper: I_ControlHelper
    //{
    //    UISingleControl GenerateControl(List<ColumnKeyValueRangeDTO> keyValues, bool valueIsKeyOrTitle, ColumnUISettingDTO columnSetting, List<SimpleSearchOperator> operators = null);
    //}


    //public abstract class UIControl
    //{
    //    public UIControl()
    //    {

    //    }
    //    public FrameworkElement Control { set; get; }
    //    public List<Tuple<int, int>> ReservedCells;

    //    public abstract Enum_UIColumnsType UIColumnsType { get; }
    //    public abstract short UIRowsCount { get; }
    //}

    //public class UISingleControl : UIControl
    //{
    //    public UISingleControl()
    //    {

    //    }

    //    public ColumnUISettingDTO ColumnSetting { get; internal set; }

    //    public override Enum_UIColumnsType UIColumnsType
    //    {
    //        get
    //        {
    //            return ColumnSetting.UIColumnsType;
    //        }
    //    }

    //    public override short UIRowsCount
    //    {
    //        get
    //        {
    //            return ColumnSetting.UIRowsCount;
    //        }
    //    }
    //}
    //public class UIRelationshipControl : UIControl
    //{
    //    public UIRelationshipControl()
    //    {

    //    }
    //    public RelationshipUISettingDTO RelationshipSetting { get; internal set; }
    //    public override Enum_UIColumnsType UIColumnsType
    //    {
    //        get
    //        {
    //            return RelationshipSetting.UIColumnsType;
    //        }
    //    }


    //}

    //public class UIGroupControl : UIControl
    //{
    //    public UIGroupControl()
    //    {

    //    }
    //    public GroupUISettingDTO GroupSetting { get; internal set; }
    //    public override Enum_UIColumnsType UIColumnsType
    //    {
    //        get
    //        {
    //            return GroupSetting.UIColumnsType;
    //        }
    //    }

    //    public override short UIRowsCount
    //    {
    //        get
    //        {
    //            if (GroupSetting.UIRowsCount == Enum_UIRowsType.One)
    //                return 1;
    //            else if (GroupSetting.UIRowsCount == Enum_UIRowsType.Two)
    //                return 2;
    //            else if (GroupSetting.UIRowsCount == Enum_UIRowsType.Unlimited)
    //                return -1;
    //            return 1;
    //        }
    //    }
    //}


}