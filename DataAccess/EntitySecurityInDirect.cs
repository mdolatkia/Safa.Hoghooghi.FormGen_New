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
    
    public partial class EntitySecurityInDirect
    {
        public int ID { get; set; }
        public int TableDrivedEntityID { get; set; }
        public int EntityRelationshipTailID { get; set; }
    
        public virtual EntityRelationshipTail EntityRelationshipTail { get; set; }
        public virtual TableDrivedEntity TableDrivedEntity { get; set; }
    }
}
