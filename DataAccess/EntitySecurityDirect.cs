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
    
    public partial class EntitySecurityDirect
    {
        public EntitySecurityDirect()
        {
            this.EntitySecurityCondition_Old = new HashSet<EntitySecurityCondition_Old>();
            this.EntitySecurityDirectStates = new HashSet<EntitySecurityDirectStates>();
        }
    
        public int ID { get; set; }
        public int TableDrivedEntityID { get; set; }
        public Nullable<int> SecuritySubjectID { get; set; }
        public bool IgnoreSecurity { get; set; }
    
        public virtual TableDrivedEntity TableDrivedEntity { get; set; }
        public virtual ICollection<EntitySecurityCondition_Old> EntitySecurityCondition_Old { get; set; }
        public virtual ICollection<EntitySecurityDirectStates> EntitySecurityDirectStates { get; set; }
        public virtual SecuritySubject SecuritySubject { get; set; }
    }
}
