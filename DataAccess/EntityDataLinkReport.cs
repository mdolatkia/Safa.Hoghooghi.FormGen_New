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
    
    public partial class EntityDataLinkReport
    {
        public int ID { get; set; }
        public int DataLinkDefinitionID { get; set; }
        public int tmp { get; set; }
    
        public virtual DataLinkDefinition DataLinkDefinition { get; set; }
    }
}
