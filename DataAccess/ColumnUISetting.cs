//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class ColumnUISetting
    {
        public int ID { get; set; }
        public int EntityUISettingID { get; set; }
        public short UIColumnsType { get; set; }
        public short UIRowsCount { get; set; }
    
        public virtual EntityUIComposition EntityUIComposition { get; set; }
    }
}
