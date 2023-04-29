
using CommonDefinitions.UISettings;
using ModelEntites;

using ProxyLibrary;
using System;
using System.Collections.Generic;
namespace MyUILibrary.EntityArea
{
    public class SearchAreaInitializer
    {
        public SearchAreaInitializer()
        {
            //   UISettings = new TemplateEntityUISettings();
            //UISettings.FlowDirection = FlowDirection.RightToLeft;
            //   UISettings.Language = "farsi";
        }

        //public int MainEntitySourceRelationshipID;
        //public SearchAreaRelationSource SourceRelation { set; get; }
        //   public TemplateEntityUISettings UISettings { set; get; }
        //    public DP_SearchRepositoryMain EditSearchRepository { set; get; }

        public int EntityID { get; set; }
        //public TableDrivedEntityDTO TempEntity { get; set; }

        public string Title { set; get; }
        public int EntitySearchID { get; set; }

        public bool ForSave { get; set; }
        //  public EntitySearchDTO EntitySearchDTO { get; set; }

        // public DP_SearchRepositoryMain PreDefinedSearch { get; set; }

        public PreDefinedSearchDTO PreDefinedSearchMessage { set; get; }
        public AdvancedSearchDTO AdvancedSearchDTOMessage { set; get; }


        //  public EntitySearchDTO SearchEntity { get; set; }

        //public int TargetEntityID;
        //  public RelationshipDTO SourceRelationship;
        //  public Enum_RelationshipType SourceToTargetRelationshipType;
        //  public Enum_MasterRelationshipType SourceToTargetMasterRelationshipType;

    }
    //public class RelationshipFilterInfo
    //{
    //    public I_EditEntityArea SourceEntityArea { set; get; }
    //    public List<RelationshipFilterDTO> RelationshipFilters { set; get; }
    //}
}
