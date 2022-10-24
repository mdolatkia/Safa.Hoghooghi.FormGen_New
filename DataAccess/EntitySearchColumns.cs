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
    
    public partial class EntitySearchColumns
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EntitySearchColumns()
        {
            this.SavedPreDefinedSearchRelationship = new HashSet<SavedPreDefinedSearchRelationship>();
            this.SavedPreDefinedSearchSimpleColumn = new HashSet<SavedPreDefinedSearchSimpleColumn>();
        }
    
        public int ID { get; set; }
        public int EntitySearchID { get; set; }
        public Nullable<int> ColumnID { get; set; }
        public Nullable<int> EntityRelationshipTailID { get; set; }
        public Nullable<short> OrderID { get; set; }
        public string Alias { get; set; }
        public bool RelationshipTailSelectable { get; set; }
        public string Tooltip { get; set; }
        public Nullable<bool> ExcludeInGeneralSearch { get; set; }
    
        public virtual Column Column { get; set; }
        public virtual EntityRelationshipTail EntityRelationshipTail { get; set; }
        public virtual EntitySearch EntitySearch { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SavedPreDefinedSearchRelationship> SavedPreDefinedSearchRelationship { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SavedPreDefinedSearchSimpleColumn> SavedPreDefinedSearchSimpleColumn { get; set; }
    }
}
