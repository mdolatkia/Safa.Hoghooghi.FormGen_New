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
    
    public partial class SavedPreDefinedSearchRelationshipData
    {
        public int ID { get; set; }
        public int SavedPreDefinedSearchRelationshipID { get; set; }
        public string DataGroup { get; set; }
        public int KeyColumnID { get; set; }
        public string Value { get; set; }
    
        public virtual Column Column { get; set; }
        public virtual SavedPreDefinedSearchRelationship SavedPreDefinedSearchRelationship { get; set; }
    }
}