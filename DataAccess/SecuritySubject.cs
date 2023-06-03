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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SecuritySubject()
        {
            this.EntityStateConditionSecuritySubject = new HashSet<EntityStateConditionSecuritySubject>();
            this.Permission = new HashSet<Permission>();
        }
    
        public int ID { get; set; }
        public short Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EntityStateConditionSecuritySubject> EntityStateConditionSecuritySubject { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual OrganizationPost OrganizationPost { get; set; }
        public virtual OrganizationType OrganizationType { get; set; }
        public virtual OrganizationType_RoleType OrganizationType_RoleType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Permission> Permission { get; set; }
        public virtual RoleType RoleType { get; set; }
    }
}
