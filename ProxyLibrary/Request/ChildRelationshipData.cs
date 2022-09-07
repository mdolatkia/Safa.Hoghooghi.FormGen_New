using ModelEntites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
namespace ProxyLibrary
{


    public class ReadonlyStateFromTail
    {
        public ReadonlyStateFromTail(string relationshipTail)
        {
            RelationshipTail = relationshipTail;
        }
        public string RelationshipTail { set; get; }

    }


    public class ChildRelationshipData
    {
        public DP_DataRepository SourceData { set; get; }
        public RelationshipDTO Relationship { set; get; }
        public ChildRelationshipData(DP_DataRepository sourceData, RelationshipDTO relationship)
        {
            SourceData = sourceData;
            Relationship = relationship;
            RelatedData = new List<DP_DataRepository>();
            OriginalRelatedData = new List<DP_DataRepository>();
            RemovedDataForUpdate = new List<DP_DataRepository>();
            //RemovedItems = new List<ProxyLibrary.DP_DataRepository>();
            // ReadonlyStateFromTails = new List<string>();
        }


        //    RelationshipDeleteOption RelationshipDeleteOption { set; get; }
        public RelationshipDeleteUpdateRule DBDeleteRule { get {return Relationship.DBDeleteRule; } }
        public RelationshipDeleteUpdateRule DBUpdateRule { get { return Relationship.DBUpdateRule; } }


        public bool RelationshipIsChangedForUpdate
        {
            set; get;
        }
        public  List<DP_DataRepository> OriginalRelatedData { get; set; }

        public List<DP_DataRepository> RemovedDataForUpdate { set; get; }
        public List<DP_DataRepository> RelatedData { set; get; }


    }

    public class ParentRelationshipData

    {
        public ParentRelationshipData(ChildRelationshipData parantChildRelationshipdata)
        {
            ParantChildRelationshipData = parantChildRelationshipdata;
        }
        public ChildRelationshipData ParantChildRelationshipData { set; get; }
        public int ToParentRelationshipID { get { return ParantChildRelationshipData.Relationship.PairRelationshipID; } }
        public RelationshipDTO ToParentRelationship { get { return ParantChildRelationshipData.Relationship.PairRelationship; } }
        public DP_DataRepository SourceData { get { return ParantChildRelationshipData.SourceData; } }
        //   public bool IsHidden { get; set; }
        //   public bool IsReadonly { get; set; }
        public bool IsAdded { get; set; }
    }



}
