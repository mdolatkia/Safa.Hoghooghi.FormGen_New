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
    
    public partial class EntityDataItemReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EntityDataItemReport()
        {
            this.DataMenuDataItemReport = new HashSet<DataMenuDataItemReport>();
        }
    
        public int ID { get; set; }
        public short DataItemReportType { get; set; }
    
        public virtual DataLinkDefinition DataLinkDefinition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataMenuDataItemReport> DataMenuDataItemReport { get; set; }
        public virtual EntityReport EntityReport { get; set; }
        public virtual EntityDirectlReport EntityDirectlReport { get; set; }
        public virtual GraphDefinition GraphDefinition { get; set; }
    }
}
