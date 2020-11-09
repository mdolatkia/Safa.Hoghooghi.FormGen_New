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
    
    public partial class RelationshipType
    {
        public int ID { get; set; }
        public bool IsOtherSideMandatory { get; set; }
        public bool IsOtherSideCreatable { get; set; }
        public bool IsOtherSideDirectlyCreatable { get; set; }
        public Nullable<short> DeleteOption { get; set; }
        public bool IsNotSkippable { get; set; }
        public Nullable<bool> PKToFKDataEntryEnabled { get; set; }
    
        public virtual ExplicitOneToOneRelationshipType ExplicitOneToOneRelationshipType { get; set; }
        public virtual ImplicitOneToOneRelationshipType ImplicitOneToOneRelationshipType { get; set; }
        public virtual ManyToOneRelationshipType ManyToOneRelationshipType { get; set; }
        public virtual OneToManyRelationshipType OneToManyRelationshipType { get; set; }
        public virtual Relationship Relationship { get; set; }
        public virtual SubToSuperRelationshipType SubToSuperRelationshipType { get; set; }
        public virtual SubUnionToUnionRelationshipType SubUnionToUnionRelationshipType { get; set; }
        public virtual SuperToSubRelationshipType SuperToSubRelationshipType { get; set; }
        public virtual UnionToSubUnionRelationshipType UnionToSubUnionRelationshipType { get; set; }
    }
}
