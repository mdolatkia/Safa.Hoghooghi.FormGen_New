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
    
    public partial class DBServer
    {
        public DBServer()
        {
            this.DatabaseInformation = new HashSet<DatabaseInformation>();
            this.LinkedServer = new HashSet<LinkedServer>();
            this.LinkedServer1 = new HashSet<LinkedServer>();
        }
    
        public int ID { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
    
        public virtual ICollection<DatabaseInformation> DatabaseInformation { get; set; }
        public virtual ICollection<LinkedServer> LinkedServer { get; set; }
        public virtual ICollection<LinkedServer> LinkedServer1 { get; set; }
    }
}