//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyRuleEngine.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Action
    {
        public Action()
        {
            this.Rule = new HashSet<Rule>();
        }
    
        public int ID { get; set; }
        public string ClassName { get; set; }
        public int AssemblyInfoID { get; set; }
    
        public virtual AssemblyInfo AssemblyInfo { get; set; }
        public virtual ICollection<Rule> Rule { get; set; }
    }
}
