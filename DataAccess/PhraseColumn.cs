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
    
    public partial class PhraseColumn
    {
        public int ID { get; set; }
        public int ColumnID { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
        public Nullable<int> FormulaID { get; set; }
    
        public virtual Column Column { get; set; }
        public virtual Formula Formula { get; set; }
        public virtual Phrase Phrase { get; set; }
    }
}
