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
    
    public partial class EntityChartReport
    {
        public EntityChartReport()
        {
            this.CharetReportCategories = new HashSet<CharetReportCategories>();
            this.CharetReportSeries = new HashSet<CharetReportSeries>();
            this.CharetReportValues = new HashSet<CharetReportValues>();
        }
    
        public int ID { get; set; }
        public short ChartType { get; set; }
        public int EntityListViewID { get; set; }
    
        public virtual ICollection<CharetReportCategories> CharetReportCategories { get; set; }
        public virtual ICollection<CharetReportSeries> CharetReportSeries { get; set; }
        public virtual ICollection<CharetReportValues> CharetReportValues { get; set; }
        public virtual EntityListView EntityListView { get; set; }
        public virtual EntitySearchableReport EntitySearchableReport { get; set; }
    }
}