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
    
    public partial class EntityListViewColumns
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EntityListViewColumns()
        {
            this.CharetReportCategories = new HashSet<CharetReportCategories>();
            this.CharetReportSeries = new HashSet<CharetReportSeries>();
            this.CharetReportValues = new HashSet<CharetReportValues>();
            this.CrosstabReportColumns = new HashSet<CrosstabReportColumns>();
            this.CrosstabReportRows = new HashSet<CrosstabReportRows>();
            this.CrosstabReportValues = new HashSet<CrosstabReportValues>();
            this.EntityListReportSubsColumns = new HashSet<EntityListReportSubsColumns>();
            this.EntityListReportSubsColumns1 = new HashSet<EntityListReportSubsColumns>();
            this.LetterTemplatePlainField = new HashSet<LetterTemplatePlainField>();
            this.ReportGroups = new HashSet<ReportGroups>();
        }
    
        public int ID { get; set; }
        public int EntityListViewID { get; set; }
        public int ColumnID { get; set; }
        public Nullable<short> OrderID { get; set; }
        public Nullable<short> WidthUnit { get; set; }
        public string Alias { get; set; }
        public Nullable<int> EntityRelationshipTailID { get; set; }
        public bool IsDescriptive { get; set; }
        public string Tooltip { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CharetReportCategories> CharetReportCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CharetReportSeries> CharetReportSeries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CharetReportValues> CharetReportValues { get; set; }
        public virtual Column Column { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CrosstabReportColumns> CrosstabReportColumns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CrosstabReportRows> CrosstabReportRows { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CrosstabReportValues> CrosstabReportValues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntityListReportSubsColumns> EntityListReportSubsColumns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntityListReportSubsColumns> EntityListReportSubsColumns1 { get; set; }
        public virtual EntityListView EntityListView { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LetterTemplatePlainField> LetterTemplatePlainField { get; set; }
        public virtual EntityRelationshipTail EntityRelationshipTail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportGroups> ReportGroups { get; set; }
    }
}
