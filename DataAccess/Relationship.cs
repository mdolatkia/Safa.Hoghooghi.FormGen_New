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
    
    public partial class Relationship
    {
        public Relationship()
        {
            this.ArcRelationshipGroup_Relationship = new HashSet<ArcRelationshipGroup_Relationship>();
            this.DataMenuForViewEntity = new HashSet<DataMenuForViewEntity>();
            this.FormulaItems = new HashSet<FormulaItems>();
            this.Relationship1 = new HashSet<Relationship>();
            this.SearchRepository = new HashSet<SearchRepository>();
            this.UIEnablityDetails = new HashSet<UIEnablityDetails>();
            this.RelationshipSearchFilter = new HashSet<RelationshipSearchFilter>();
            this.RelationshipColumns = new HashSet<RelationshipColumns>();
        }
    
        public int ID { get; set; }
        public Nullable<int> RelationshipID { get; set; }
        public bool Removed { get; set; }
        public Nullable<byte> TypeEnum { get; set; }
        public int TableDrivedEntityID1 { get; set; }
        public int TableDrivedEntityID2 { get; set; }
        public bool Created { get; set; }
        public int FirstSideTableID { get; set; }
        public int SecondSideTableID { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Alias { get; set; }
        public Nullable<byte> MasterTypeEnum { get; set; }
        public bool IsOrginal { get; set; }
        public bool Reviewed { get; set; }
        public bool SearchInitially { get; set; }
        public Nullable<short> DBDeleteRule { get; set; }
        public Nullable<short> DBUpdateRule { get; set; }
    
        public virtual ICollection<ArcRelationshipGroup_Relationship> ArcRelationshipGroup_Relationship { get; set; }
        public virtual ICollection<DataMenuForViewEntity> DataMenuForViewEntity { get; set; }
        public virtual ICollection<FormulaItems> FormulaItems { get; set; }
        public virtual ICollection<Relationship> Relationship1 { get; set; }
        public virtual Relationship Relationship2 { get; set; }
        public virtual SecurityObject SecurityObject { get; set; }
        public virtual ICollection<SearchRepository> SearchRepository { get; set; }
        public virtual ICollection<UIEnablityDetails> UIEnablityDetails { get; set; }
        public virtual TableDrivedEntity TableDrivedEntity { get; set; }
        public virtual TableDrivedEntity TableDrivedEntity1 { get; set; }
        public virtual ICollection<RelationshipSearchFilter> RelationshipSearchFilter { get; set; }
        public virtual ICollection<RelationshipColumns> RelationshipColumns { get; set; }
        public virtual RelationshipType RelationshipType { get; set; }
    }
}
