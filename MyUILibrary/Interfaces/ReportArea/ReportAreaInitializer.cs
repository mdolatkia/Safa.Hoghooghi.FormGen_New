
using CommonDefinitions.UISettings;
using ModelEntites;

using ProxyLibrary;
using System;
using System.Collections.Generic;
namespace MyUILibrary.EntityArea
{
    public class InternalReportAreaInitializer
    {
        //public int EntityID;
        //public DP_SearchRepositoryMain AdvanceSearchRepository { get; internal set; }
        //public PreDefinedSearchDTO PreDefinedSearch { get; internal set; }
        //   public DP_SearchRepositoryMain InitialSearchRepository { set; get; }
    //    public int ReportID { get; set; }
        //   public DP_SearchRepositoryMain PreDefinedSearch { get; set; }
        public bool SearchInitially { set; get; }
        public bool UserCanChangeSearch { get; set; }
     //   public string Title { get; set; }
      //  public SearchableReportType ReportType { set; get; }
        public EntityReportDTO Report { get; internal set; }
    }
    public class ExternalReportAreaInitializer
    {
     //   public int EntityID;
    //    public DP_SearchRepositoryMain InitialSearchRepository { set; get; }
       // public int ReportID { get; set; }
        public bool SearchInitially { set; get; }
        public bool UserCanChangeSearch { get; set; }

        //public DP_SearchRepositoryMain AdvanceSearchRepository { get; internal set; }
        //public PreDefinedSearchDTO PreDefinedSearch { get; internal set; }
        public EntityReportDTO Report { get; internal set; }
    }
}
