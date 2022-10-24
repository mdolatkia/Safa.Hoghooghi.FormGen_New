using ModelEntites;
using System;
using System.Collections.Generic;
namespace ProxyLibrary
{
    public class DR_SearchViewRequest : BaseRequest
    {
        public DR_SearchViewRequest(DR_Requester Requester, DP_SearchRepositoryMain searchDataItems) : base(Requester)
        {
            SearchDataItems = searchDataItems;
            //  
        }
      //  public bool CheckStates { set; get; }
        public DP_SearchRepositoryMain SearchDataItems;
        //public int EntityID;
        public int EntityViewID { set; get; }
        public int OrderByEntityViewColumnID { set; get; }
        public Enum_OrderBy SortType { set; get; }

        public int MaxDataItems { set; get; }
    }

    public class DR_SearchEditViewRequest : DR_SearchViewRequest
    {
        public DR_SearchEditViewRequest(DR_Requester Requester, DP_SearchRepositoryMain searchDataItems, int toParentRelationshipID) : base(Requester, searchDataItems)
        {
            ToParentRelationshipID = toParentRelationshipID;
            //  
        }
        public int ToParentRelationshipID { get; set; }
    }
    //public class DR_SearchViewByRelationshipTailRequest : BaseRequest
    //{
    //    public DR_SearchViewByRelationshipTailRequest(DR_Requester Requester) : base(Requester)
    //    {
    //        
    //    }
    //    public int EntityViewID { set; get; }
    //    public EntityRelationshipTailDTO RelationshipTail { set; get; }
    //    public List<EntityInstanceProperty> FirstRelationshipFirstSideKeyColumns { set; get; }
    //}

    public class DR_SearchExistsRequest : BaseRequest
    {
        public DR_SearchExistsRequest(DR_Requester Requester) : base(Requester)
        {
            //
        }
        public DP_SearchRepositoryMain SearchDataItems;
        public int EntityID;
    }
    public class DR_SearchCountRequest : BaseRequest
    {
        public DR_SearchCountRequest(DR_Requester Requester) : base(Requester)
        {

        }
        public DP_SearchRepositoryMain SearchDataItems;
        public int EntityID;
    }
    //public class DR_SearchByRelationViewRequest : BaseRequest
    //{
    //    public DR_SearchByRelationViewRequest(DR_Requester Requester) : base(Requester)
    //    {

    //    }
    //    public DP_SearchRepository SourceRelationData;
    //    public int SourceRelationshipID;
    //}

    public class DR_SearchEditRequest : BaseRequest
    {
        public DR_SearchEditRequest(DR_Requester Requester, DP_SearchRepositoryMain searchDataItem) : base(Requester)
        {
            SearchDataItem = searchDataItem;
            //CheckStates = checkStates;
            //ToParentRelationshipID = toParentRelationshipID;
            //if (viewMode)
            //    
            //else
            //    SecurityMode = SecurityMode.Edit;
            //     WithDataView = withDataView;
        }
        public bool WithDataView { set; get; }
        //public bool CheckStates { get; set; }
        //public int ToParentRelationshipID { get; set; }

        //public DR_SearchEditRequest()
        //{

        //    //Properties = new List<List<EntityInstanceProperty>>();
        //}
        public DP_SearchRepositoryMain SearchDataItem;

    }
    public class DR_SearchFullDataRequest : BaseRequest
    {
        public DR_SearchFullDataRequest(DR_Requester Requester, DP_SearchRepositoryMain searchDataItem) : base(Requester)
        {

            SearchDataItem = searchDataItem;
        }
        //public DR_SearchEditRequest()
        //{

        //    //Properties = new List<List<EntityInstanceProperty>>();
        //}
        public DP_SearchRepositoryMain SearchDataItem;

    }
    public class DR_SearchKeysOnlyRequest : BaseRequest
    {
        public DR_SearchKeysOnlyRequest(DR_Requester Requester, DP_SearchRepositoryMain searchDataItem) : base(Requester)
        {

            SearchDataItem = searchDataItem;
        }
        public DP_SearchRepositoryMain SearchDataItem;
    }
    //بهتره این حذف شه. یعنی ابتدا مثل خیلی از جاها سرچ آیتم ساخته شه و بعد بر حسب نیاز هر کدوم از جستجو ها صدا زده بشه
    //public class DR_SearchByRelationshipTailRequest : BaseRequest
    //{
    //    public DR_SearchByRelationshipTailRequest(DR_Requester Requester) : base(Requester)
    //    {
    //        
    //    }
    //    public EntityRelationshipTailDTO RelationshipTail { set; get; }
    //    public DP_BaseData FirstDataItem { set; get; }
    //}

    //public class DR_SearchViewByRelationshipTailRequest : BaseRequest
    //{
    //    public DR_SearchViewByRelationshipTailRequest(DR_Requester Requester) : base(Requester)
    //    {
    //        
    //    }
    //    public int EntityViewID { set; get; }

    //    public EntityRelationshipTailDTO RelationshipTail { set; get; }
    //    public DP_BaseData FirstDataItem { set; get; }
    //}
    public enum Enum_OrderBy
    {
        Ascending,
        Descending
    }
}
