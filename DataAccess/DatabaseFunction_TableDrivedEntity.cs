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
    
    public partial class DatabaseFunction_TableDrivedEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatabaseFunction_TableDrivedEntity()
        {
            this.BackendActionActivity = new HashSet<BackendActionActivity>();
            this.Formula = new HashSet<Formula>();
            this.FormulaItems = new HashSet<FormulaItems>();
            this.DatabaseFunction_TableDrivedEntity_Columns = new HashSet<DatabaseFunction_TableDrivedEntity_Columns>();
        }
    
        public int ID { get; set; }
        public int DatabaseFunctionID { get; set; }
        public int TableDrivedEntityID { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BackendActionActivity> BackendActionActivity { get; set; }
        public virtual DatabaseFunction DatabaseFunction { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Formula> Formula { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormulaItems> FormulaItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatabaseFunction_TableDrivedEntity_Columns> DatabaseFunction_TableDrivedEntity_Columns { get; set; }
        public virtual TableDrivedEntity TableDrivedEntity { get; set; }
    }
}
