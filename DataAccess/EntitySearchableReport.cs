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
    
    public partial class EntitySearchableReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EntitySearchableReport()
        {
            this.DataMenuSearchableReportRelationship = new HashSet<DataMenuSearchableReportRelationship>();
        }
    
        public int ID { get; set; }
        public Nullable<short> SearchableReportType { get; set; }
        public Nullable<int> SearchRepositoryID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataMenuSearchableReportRelationship> DataMenuSearchableReportRelationship { get; set; }
        public virtual EntityChartReport EntityChartReport { get; set; }
        public virtual EntityCrosstabReport EntityCrosstabReport { get; set; }
        public virtual EntityDataViewReport EntityDataViewReport { get; set; }
        public virtual EntityExternalReport EntityExternalReport { get; set; }
        public virtual EntityGridViewReport EntityGridViewReport { get; set; }
        public virtual EntityListReport EntityListReport { get; set; }
        public virtual EntityReport EntityReport { get; set; }
        public virtual SearchRepository SearchRepository { get; set; }
    }
}
