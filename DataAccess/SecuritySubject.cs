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
    
    public partial class SecuritySubject
    {
        public SecuritySubject()
        {
            this.TableDrivedEntityStateConditionSecuritySubject = new HashSet<TableDrivedEntityStateConditionSecuritySubject>();
            this.Permission = new HashSet<Permission>();
        }
    
        public int ID { get; set; }
        public short Type { get; set; }
    
        public virtual ICollection<TableDrivedEntityStateConditionSecuritySubject> TableDrivedEntityStateConditionSecuritySubject { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual OrganizationPost OrganizationPost { get; set; }
        public virtual OrganizationType OrganizationType { get; set; }
        public virtual OrganizationType_RoleType OrganizationType_RoleType { get; set; }
        public virtual ICollection<Permission> Permission { get; set; }
        public virtual RoleType RoleType { get; set; }
    }
}
