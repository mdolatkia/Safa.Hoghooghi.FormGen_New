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
    
    public partial class ColumnValueRangeDetails
    {
        public int ID { get; set; }
        public int ColumnValueRangeID { get; set; }
        public string KeyTitle { get; set; }
        public string Value { get; set; }
        public string Tag { get; set; }
    
        public virtual ColumnValueRange ColumnValueRange { get; set; }
    }
}
