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
    
    public partial class MyDataItemKeyColumns
    {
        public int ID { get; set; }
        public int MyDataItemID { get; set; }
        public int ColumnID { get; set; }
        public string Value { get; set; }
    
        public virtual MyDataItem MyDataItem { get; set; }
    }
}