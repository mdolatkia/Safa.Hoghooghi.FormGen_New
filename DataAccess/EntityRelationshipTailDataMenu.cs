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
    
    public partial class EntityRelationshipTailDataMenu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EntityRelationshipTailDataMenu()
        {
            this.DataLinkDefinition_EntityRelationshipTail = new HashSet<DataLinkDefinition_EntityRelationshipTail>();
            this.EntityRelationshipTailDataMenuItems = new HashSet<EntityRelationshipTailDataMenuItems>();
            this.GraphDefinition_EntityRelationshipTail = new HashSet<GraphDefinition_EntityRelationshipTail>();
        }
    
        public int ID { get; set; }
        public int EntityRelationshipTailID { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataLinkDefinition_EntityRelationshipTail> DataLinkDefinition_EntityRelationshipTail { get; set; }
        public virtual EntityRelationshipTail EntityRelationshipTail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntityRelationshipTailDataMenuItems> EntityRelationshipTailDataMenuItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GraphDefinition_EntityRelationshipTail> GraphDefinition_EntityRelationshipTail { get; set; }
    }
}
