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
    
    public partial class DateTimeColumnType
    {
        public int ColumnID { get; set; }
        public Nullable<bool> ShowMiladiDateInUI { get; set; }
        public bool DBValueIsString { get; set; }
        public Nullable<bool> DBValueIsStringMiladi { get; set; }
        public Nullable<short> DBValueStringTimeFormat { get; set; }
    
        public virtual Column Column { get; set; }
    }
}
