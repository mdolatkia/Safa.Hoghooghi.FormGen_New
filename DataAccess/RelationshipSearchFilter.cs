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
    
    public partial class RelationshipSearchFilter
    {
        public int ID { get; set; }
        public int RelationshipID { get; set; }
        public Nullable<int> ValueRelationshipTailID { get; set; }
        public int ValueColumnID { get; set; }
        public Nullable<int> SearchRelationshipTailID { get; set; }
        public int SearchColumnID { get; set; }
    
        public virtual Column Column { get; set; }
        public virtual Column Column1 { get; set; }
        public virtual EntityRelationshipTail EntityRelationshipTail { get; set; }
        public virtual EntityRelationshipTail EntityRelationshipTail1 { get; set; }
        public virtual Relationship Relationship { get; set; }
    }
}