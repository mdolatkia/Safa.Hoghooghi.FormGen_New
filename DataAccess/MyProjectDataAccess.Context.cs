﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MyIdeaEntities : DbContext
    {
        public MyIdeaEntities()
            : base("name=MyIdeaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ArchiveFolder> ArchiveFolder { get; set; }
        public virtual DbSet<ArchiveTag> ArchiveTag { get; set; }
        public virtual DbSet<EntityArchiveRelationshipTails> EntityArchiveRelationshipTails { get; set; }
        public virtual DbSet<EntityLetterRelationshipTails> EntityLetterRelationshipTails { get; set; }
        public virtual DbSet<ArcRelationshipGroup> ArcRelationshipGroup { get; set; }
        public virtual DbSet<ArcRelationshipGroup_Relationship> ArcRelationshipGroup_Relationship { get; set; }
        public virtual DbSet<BackendActionActivity> BackendActionActivity { get; set; }
        public virtual DbSet<CharetReportCategories> CharetReportCategories { get; set; }
        public virtual DbSet<CharetReportSeries> CharetReportSeries { get; set; }
        public virtual DbSet<CharetReportValues> CharetReportValues { get; set; }
        public virtual DbSet<CodeFunction> CodeFunction { get; set; }
        public virtual DbSet<CodeFunction_TableDrivedEntity> CodeFunction_TableDrivedEntity { get; set; }
        public virtual DbSet<CodeFunction_TableDrivedEntity_Parameters> CodeFunction_TableDrivedEntity_Parameters { get; set; }
        public virtual DbSet<CodeFunctionParameter> CodeFunctionParameter { get; set; }
        public virtual DbSet<Column> Column { get; set; }
        public virtual DbSet<ColumnCustomFormula> ColumnCustomFormula { get; set; }
        public virtual DbSet<ColumnUISetting> ColumnUISetting { get; set; }
        public virtual DbSet<ColumnValueRange> ColumnValueRange { get; set; }
        public virtual DbSet<ColumnValueRangeDetails> ColumnValueRangeDetails { get; set; }
        public virtual DbSet<CrosstabReportColumns> CrosstabReportColumns { get; set; }
        public virtual DbSet<CrosstabReportRows> CrosstabReportRows { get; set; }
        public virtual DbSet<CrosstabReportValues> CrosstabReportValues { get; set; }
        public virtual DbSet<DatabaseFunction> DatabaseFunction { get; set; }
        public virtual DbSet<DatabaseFunction_TableDrivedEntity> DatabaseFunction_TableDrivedEntity { get; set; }
        public virtual DbSet<DatabaseFunction_TableDrivedEntity_Columns> DatabaseFunction_TableDrivedEntity_Columns { get; set; }
        public virtual DbSet<DatabaseFunctionParameter> DatabaseFunctionParameter { get; set; }
        public virtual DbSet<DatabaseInformation> DatabaseInformation { get; set; }
        public virtual DbSet<DatabaseUISetting> DatabaseUISetting { get; set; }
        public virtual DbSet<DataLinkDefinition> DataLinkDefinition { get; set; }
        public virtual DbSet<DataLinkDefinition_EntityRelationshipTail> DataLinkDefinition_EntityRelationshipTail { get; set; }
        public virtual DbSet<DataMenuDataItemReport> DataMenuDataItemReport { get; set; }
        public virtual DbSet<DataMenuForViewEntity> DataMenuForViewEntity { get; set; }
        public virtual DbSet<DataMenuRelationshipTail> DataMenuRelationshipTail { get; set; }
        public virtual DbSet<DataMenuRelTailSearchableReports> DataMenuRelTailSearchableReports { get; set; }
        public virtual DbSet<DataMenuSetting> DataMenuSetting { get; set; }
        public virtual DbSet<DateColumnType> DateColumnType { get; set; }
        public virtual DbSet<DateTimeColumnType> DateTimeColumnType { get; set; }
        public virtual DbSet<DBSchema> DBSchema { get; set; }
        public virtual DbSet<DBServer> DBServer { get; set; }
        public virtual DbSet<EmptySpaceUISetting> EmptySpaceUISetting { get; set; }
        public virtual DbSet<EntityChartReport> EntityChartReport { get; set; }
        public virtual DbSet<EntityCommand> EntityCommand { get; set; }
        public virtual DbSet<EntityCrosstabReport> EntityCrosstabReport { get; set; }
        public virtual DbSet<EntityDataItemReport> EntityDataItemReport { get; set; }
        public virtual DbSet<EntityDataViewReport> EntityDataViewReport { get; set; }
        public virtual DbSet<EntityDeterminer> EntityDeterminer { get; set; }
        public virtual DbSet<EntityDirectlReport> EntityDirectlReport { get; set; }
        public virtual DbSet<EntityDirectlReportParameters> EntityDirectlReportParameters { get; set; }
        public virtual DbSet<EntityExternalReport> EntityExternalReport { get; set; }
        public virtual DbSet<EntityGridViewReport> EntityGridViewReport { get; set; }
        public virtual DbSet<EntityListReport> EntityListReport { get; set; }
        public virtual DbSet<EntityListReportSubs> EntityListReportSubs { get; set; }
        public virtual DbSet<EntityListReportSubsColumns> EntityListReportSubsColumns { get; set; }
        public virtual DbSet<EntityListView> EntityListView { get; set; }
        public virtual DbSet<EntityListViewColumns> EntityListViewColumns { get; set; }
        public virtual DbSet<EntityRelationshipTail> EntityRelationshipTail { get; set; }
        public virtual DbSet<EntityRelationshipTailDataMenu> EntityRelationshipTailDataMenu { get; set; }
        public virtual DbSet<EntityRelationshipTailDataMenuItems> EntityRelationshipTailDataMenuItems { get; set; }
        public virtual DbSet<EntityReport> EntityReport { get; set; }
        public virtual DbSet<EntitySearch> EntitySearch { get; set; }
        public virtual DbSet<EntitySearchableReport> EntitySearchableReport { get; set; }
        public virtual DbSet<EntitySearchColumns> EntitySearchColumns { get; set; }
        public virtual DbSet<EntityState> EntityState { get; set; }
        public virtual DbSet<EntityState_UIActionActivity> EntityState_UIActionActivity { get; set; }
        public virtual DbSet<EntityStateSecuritySubject> EntityStateSecuritySubject { get; set; }
        public virtual DbSet<EntityStateValues> EntityStateValues { get; set; }
        public virtual DbSet<EntityUIComposition> EntityUIComposition { get; set; }
        public virtual DbSet<EntityUISetting> EntityUISetting { get; set; }
        public virtual DbSet<EntityValidation> EntityValidation { get; set; }
        public virtual DbSet<ExplicitOneToOneRelationshipType> ExplicitOneToOneRelationshipType { get; set; }
        public virtual DbSet<Formula> Formula { get; set; }
        public virtual DbSet<FormulaItems> FormulaItems { get; set; }
        public virtual DbSet<GraphDefinition> GraphDefinition { get; set; }
        public virtual DbSet<GraphDefinition_EntityRelationshipTail> GraphDefinition_EntityRelationshipTail { get; set; }
        public virtual DbSet<GroupUISetting> GroupUISetting { get; set; }
        public virtual DbSet<ImplicitOneToOneRelationshipType> ImplicitOneToOneRelationshipType { get; set; }
        public virtual DbSet<ISARelationship> ISARelationship { get; set; }
        public virtual DbSet<LetterEnabledEntities> LetterEnabledEntities { get; set; }
        public virtual DbSet<LetterSetting> LetterSetting { get; set; }
        public virtual DbSet<LetterTemplate> LetterTemplate { get; set; }
        public virtual DbSet<LetterTemplatePlainField> LetterTemplatePlainField { get; set; }
        public virtual DbSet<LetterTemplateRelationshipField> LetterTemplateRelationshipField { get; set; }
        public virtual DbSet<LetterType> LetterType { get; set; }
        public virtual DbSet<LinearFormula> LinearFormula { get; set; }
        public virtual DbSet<LinkedServer> LinkedServer { get; set; }
        public virtual DbSet<MainLetterTemplate> MainLetterTemplate { get; set; }
        public virtual DbSet<ManyToManyRelationshipType> ManyToManyRelationshipType { get; set; }
        public virtual DbSet<ManyToOneRelationshipType> ManyToOneRelationshipType { get; set; }
        public virtual DbSet<NavigationTree> NavigationTree { get; set; }
        public virtual DbSet<NumericColumnType> NumericColumnType { get; set; }
        public virtual DbSet<OneToManyRelationshipType> OneToManyRelationshipType { get; set; }
        public virtual DbSet<PartialLetterTemplate> PartialLetterTemplate { get; set; }
        public virtual DbSet<Phrase> Phrase { get; set; }
        public virtual DbSet<PhraseColumn> PhraseColumn { get; set; }
        public virtual DbSet<PhraseLogic> PhraseLogic { get; set; }
        public virtual DbSet<PhraseRelationship> PhraseRelationship { get; set; }
        public virtual DbSet<Relationship> Relationship { get; set; }
        public virtual DbSet<RelationshipColumns> RelationshipColumns { get; set; }
        public virtual DbSet<RelationshipSearchFilter> RelationshipSearchFilter { get; set; }
        public virtual DbSet<RelationshipType> RelationshipType { get; set; }
        public virtual DbSet<RelationshipUISetting> RelationshipUISetting { get; set; }
        public virtual DbSet<ReportGroups> ReportGroups { get; set; }
        public virtual DbSet<SaveAdvancedSearch> SaveAdvancedSearch { get; set; }
        public virtual DbSet<SavedPreDefinedSearch> SavedPreDefinedSearch { get; set; }
        public virtual DbSet<SavedPreDefinedSearchRelationship> SavedPreDefinedSearchRelationship { get; set; }
        public virtual DbSet<SavedPreDefinedSearchRelationshipData> SavedPreDefinedSearchRelationshipData { get; set; }
        public virtual DbSet<SavedPreDefinedSearchSimpleColumn> SavedPreDefinedSearchSimpleColumn { get; set; }
        public virtual DbSet<SavedSearchRepository> SavedSearchRepository { get; set; }
        public virtual DbSet<StringColumnType> StringColumnType { get; set; }
        public virtual DbSet<SubSystems> SubSystems { get; set; }
        public virtual DbSet<SubToSuperRelationshipType> SubToSuperRelationshipType { get; set; }
        public virtual DbSet<SubUnionToUnionRelationshipType> SubUnionToUnionRelationshipType { get; set; }
        public virtual DbSet<SuperToSubDeterminerValue> SuperToSubDeterminerValue { get; set; }
        public virtual DbSet<SuperToSubRelationshipType> SuperToSubRelationshipType { get; set; }
        public virtual DbSet<TabGroupUISetting> TabGroupUISetting { get; set; }
        public virtual DbSet<Table> Table { get; set; }
        public virtual DbSet<TableDrivedEntity> TableDrivedEntity { get; set; }
        public virtual DbSet<TableDrivedEntity_Columns> TableDrivedEntity_Columns { get; set; }
        public virtual DbSet<TableDrivedEntity_EntityCommand> TableDrivedEntity_EntityCommand { get; set; }
        public virtual DbSet<TabPageUISetting> TabPageUISetting { get; set; }
        public virtual DbSet<TimeColumnType> TimeColumnType { get; set; }
        public virtual DbSet<UIActionActivity> UIActionActivity { get; set; }
        public virtual DbSet<UIColumnValue> UIColumnValue { get; set; }
        public virtual DbSet<UIEnablityDetails> UIEnablityDetails { get; set; }
        public virtual DbSet<UnionRelationshipType> UnionRelationshipType { get; set; }
        public virtual DbSet<UnionToSubUnionRelationshipType> UnionToSubUnionRelationshipType { get; set; }
        public virtual DbSet<UniqueConstraint> UniqueConstraint { get; set; }
        public virtual DbSet<EntitySecurityDirect> EntitySecurityDirect { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<OrganizationPost> OrganizationPost { get; set; }
        public virtual DbSet<OrganizationType> OrganizationType { get; set; }
        public virtual DbSet<OrganizationType_RoleType> OrganizationType_RoleType { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Permission_Action> Permission_Action { get; set; }
        public virtual DbSet<RoleType> RoleType { get; set; }
        public virtual DbSet<SecurityObject> SecurityObject { get; set; }
        public virtual DbSet<SecuritySubject> SecuritySubject { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<ActivityTarget> ActivityTarget { get; set; }
        public virtual DbSet<ActivityTarget_RoleType> ActivityTarget_RoleType { get; set; }
        public virtual DbSet<EntityGroup> EntityGroup { get; set; }
        public virtual DbSet<EntityGroup_Relationship> EntityGroup_Relationship { get; set; }
        public virtual DbSet<Process> Process { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<State_Formula> State_Formula { get; set; }
        public virtual DbSet<StateActivity> StateActivity { get; set; }
        public virtual DbSet<Transition> Transition { get; set; }
        public virtual DbSet<TransitionAction> TransitionAction { get; set; }
        public virtual DbSet<TransitionAction_EntityGroup> TransitionAction_EntityGroup { get; set; }
        public virtual DbSet<TransitionAction_Formula> TransitionAction_Formula { get; set; }
        public virtual DbSet<TransitionActionTarget> TransitionActionTarget { get; set; }
        public virtual DbSet<TransitionActivity> TransitionActivity { get; set; }
    }
}
