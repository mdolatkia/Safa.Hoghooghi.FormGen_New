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
    
    public partial class TableDrivedEntity
    {
        public TableDrivedEntity()
        {
            this.ArchiveFolder = new HashSet<ArchiveFolder>();
            this.ArchiveTag = new HashSet<ArchiveTag>();
            this.EntityArchiveRelationshipTails = new HashSet<EntityArchiveRelationshipTails>();
            this.EntityLetterRelationshipTails = new HashSet<EntityLetterRelationshipTails>();
            this.ArcRelationshipGroup = new HashSet<ArcRelationshipGroup>();
            this.BackendActionActivity = new HashSet<BackendActionActivity>();
            this.CodeFunction_TableDrivedEntity = new HashSet<CodeFunction_TableDrivedEntity>();
            this.DatabaseFunction_TableDrivedEntity = new HashSet<DatabaseFunction_TableDrivedEntity>();
            this.DataLinkDefinition = new HashSet<DataLinkDefinition>();
            this.DataMenuSetting = new HashSet<DataMenuSetting>();
            this.EntityDeterminer = new HashSet<EntityDeterminer>();
            this.EntityListView = new HashSet<EntityListView>();
            this.EntityRelationshipTail = new HashSet<EntityRelationshipTail>();
            this.EntityRelationshipTail1 = new HashSet<EntityRelationshipTail>();
            this.EntityRelationshipTailDataMenuItems = new HashSet<EntityRelationshipTailDataMenuItems>();
            this.EntityReport = new HashSet<EntityReport>();
            this.EntitySearch1 = new HashSet<EntitySearch>();
            this.EntityUIComposition = new HashSet<EntityUIComposition>();
            this.EntityValidation = new HashSet<EntityValidation>();
            this.Formula = new HashSet<Formula>();
            this.LetterTemplate = new HashSet<LetterTemplate>();
            this.LetterType = new HashSet<LetterType>();
            this.NavigationTree = new HashSet<NavigationTree>();
            this.Relationship = new HashSet<Relationship>();
            this.Relationship1 = new HashSet<Relationship>();
            this.SearchRepository = new HashSet<SearchRepository>();
            this.Process = new HashSet<Process>();
            this.TableDrivedEntity_EntityCommand = new HashSet<TableDrivedEntity_EntityCommand>();
            this.UIActionActivity = new HashSet<UIActionActivity>();
            this.EntitySecurityDirect = new HashSet<EntitySecurityDirect>();
            this.TableDrivedEntity_Columns = new HashSet<TableDrivedEntity_Columns>();
            this.TableDrivedEntityState = new HashSet<TableDrivedEntityState>();
        }
    
        public int ID { get; set; }
        public int TableID { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public Nullable<bool> IndependentDataEntry { get; set; }
        public bool BatchDataEntry { get; set; }
        public Nullable<bool> IsDataReference { get; set; }
        public bool IsStructurReferencee { get; set; }
        public bool IsAssociative { get; set; }
        public Nullable<int> EntityListViewID { get; set; }
        public Nullable<int> EntitySearchID { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsReadonly { get; set; }
        public Nullable<bool> SearchInitially { get; set; }
        public bool LoadArchiveRelatedItems { get; set; }
        public string Color { get; set; }
        public bool LoadLetterRelatedItems { get; set; }
        public bool IsOrginal { get; set; }
        public bool IsView { get; set; }
        public bool Reviewed { get; set; }
        public string Description { get; set; }
        public bool ColumnsReviewed { get; set; }
        public bool SelectAsComboBox { get; set; }
        public Nullable<int> DeterminerColumnID { get; set; }
        public Nullable<int> DataMenuSettingID { get; set; }
        public Nullable<int> InternalTableSuperToSubRelID { get; set; }
        public bool Removed { get; set; }
    
        public virtual ICollection<ArchiveFolder> ArchiveFolder { get; set; }
        public virtual ICollection<ArchiveTag> ArchiveTag { get; set; }
        public virtual ICollection<EntityArchiveRelationshipTails> EntityArchiveRelationshipTails { get; set; }
        public virtual ICollection<EntityLetterRelationshipTails> EntityLetterRelationshipTails { get; set; }
        public virtual ICollection<ArcRelationshipGroup> ArcRelationshipGroup { get; set; }
        public virtual ICollection<BackendActionActivity> BackendActionActivity { get; set; }
        public virtual ICollection<CodeFunction_TableDrivedEntity> CodeFunction_TableDrivedEntity { get; set; }
        public virtual Column Column { get; set; }
        public virtual ICollection<DatabaseFunction_TableDrivedEntity> DatabaseFunction_TableDrivedEntity { get; set; }
        public virtual ICollection<DataLinkDefinition> DataLinkDefinition { get; set; }
        public virtual ICollection<DataMenuSetting> DataMenuSetting { get; set; }
        public virtual DataMenuSetting DataMenuSetting1 { get; set; }
        public virtual ICollection<EntityDeterminer> EntityDeterminer { get; set; }
        public virtual ICollection<EntityListView> EntityListView { get; set; }
        public virtual EntityListView EntityListView1 { get; set; }
        public virtual ICollection<EntityRelationshipTail> EntityRelationshipTail { get; set; }
        public virtual ICollection<EntityRelationshipTail> EntityRelationshipTail1 { get; set; }
        public virtual ICollection<EntityRelationshipTailDataMenuItems> EntityRelationshipTailDataMenuItems { get; set; }
        public virtual ICollection<EntityReport> EntityReport { get; set; }
        public virtual EntitySearch EntitySearch { get; set; }
        public virtual ICollection<EntitySearch> EntitySearch1 { get; set; }
        public virtual ICollection<EntityUIComposition> EntityUIComposition { get; set; }
        public virtual ICollection<EntityValidation> EntityValidation { get; set; }
        public virtual ICollection<Formula> Formula { get; set; }
        public virtual LetterEnabledEntities LetterEnabledEntities { get; set; }
        public virtual ICollection<LetterTemplate> LetterTemplate { get; set; }
        public virtual ICollection<LetterType> LetterType { get; set; }
        public virtual ICollection<NavigationTree> NavigationTree { get; set; }
        public virtual ICollection<Relationship> Relationship { get; set; }
        public virtual ICollection<Relationship> Relationship1 { get; set; }
        public virtual ICollection<SearchRepository> SearchRepository { get; set; }
        public virtual SuperToSubRelationshipType SuperToSubRelationshipType { get; set; }
        public virtual Table Table { get; set; }
        public virtual ICollection<Process> Process { get; set; }
        public virtual ICollection<TableDrivedEntity_EntityCommand> TableDrivedEntity_EntityCommand { get; set; }
        public virtual ICollection<UIActionActivity> UIActionActivity { get; set; }
        public virtual ICollection<EntitySecurityDirect> EntitySecurityDirect { get; set; }
        public virtual SecurityObject SecurityObject { get; set; }
        public virtual ICollection<TableDrivedEntity_Columns> TableDrivedEntity_Columns { get; set; }
        public virtual ICollection<TableDrivedEntityState> TableDrivedEntityState { get; set; }
    }
}
