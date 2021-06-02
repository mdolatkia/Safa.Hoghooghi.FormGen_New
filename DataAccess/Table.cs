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
    
    public partial class Table
    {
        public Table()
        {
            this.Column = new HashSet<Column>();
            this.TableDrivedEntity = new HashSet<TableDrivedEntity>();
            this.UniqueConstraint = new HashSet<UniqueConstraint>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public int DBSchemaID { get; set; }
    
        public virtual ICollection<Column> Column { get; set; }
        public virtual DBSchema DBSchema { get; set; }
        public virtual ICollection<TableDrivedEntity> TableDrivedEntity { get; set; }
        public virtual ICollection<UniqueConstraint> UniqueConstraint { get; set; }
    }
}