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
    
    public partial class SavedPreDefinedSearch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SavedPreDefinedSearch()
        {
            this.SavedPreDefinedSearchRelationship = new HashSet<SavedPreDefinedSearchRelationship>();
            this.SavedPreDefinedSearchSimpleColumn = new HashSet<SavedPreDefinedSearchSimpleColumn>();
        }
    
        public int ID { get; set; }
        public int EntitySearchID { get; set; }
        public string QuickSearchValue { get; set; }
    
        public virtual EntitySearch EntitySearch { get; set; }
        public virtual SavedSearchRepository SavedSearchRepository { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SavedPreDefinedSearchRelationship> SavedPreDefinedSearchRelationship { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SavedPreDefinedSearchSimpleColumn> SavedPreDefinedSearchSimpleColumn { get; set; }
    }
}
