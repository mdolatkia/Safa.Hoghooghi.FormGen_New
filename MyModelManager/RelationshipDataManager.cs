using ModelEntites;
using MyDataSearchManagerBusiness;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRelationshipDataManager
{
    public class RelationshipDataManager
    {
        BizRelationship bizRelationship = new BizRelationship();
        public DP_SearchRepositoryMain GetSecondSideSearchDataItemByRelationship(DP_BaseData firstSideDataItem, int relationshipID)
        {
            var relationship = bizRelationship.GetRelationship(relationshipID);
            return GetSecondSideSearchItemByRelationship(firstSideDataItem, relationship);
        }

        private DP_SearchRepositoryMain GetSecondSideSearchItemByRelationship(DP_BaseData firstSideDataItem, RelationshipDTO relationship)
        {
            if (RelationshipFirstSideColumnExist(firstSideDataItem, relationship))
            {
                return GetSecondSideSearchItemByFirstSideColumns(firstSideDataItem, relationship);
            }
            else if (relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign
                || relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
            {
                //کلیدهای طرف اول مشخصند اما کلید ها کلید خارجی رابطه نیستند
                DP_SearchRepositoryMain resultDataItem = new DP_SearchRepositoryMain(relationship.EntityID2);
                if (firstSideDataItem.KeyProperties.Any() && firstSideDataItem.KeyProperties.All(x => x.Value != null && !string.IsNullOrEmpty(x.Value.ToString())))
                {
                    DP_SearchRepositoryMain searchItem = new DP_SearchRepositoryMain(relationship.EntityID1);
                    foreach (var col in firstSideDataItem.KeyProperties)
                        searchItem.Phrases.Add(new SearchProperty(col.Column) { Value = col.Value });
                    var requester = new DR_Requester();
                    requester.SkipSecurity = true;
                    DR_SearchFullDataRequest request = new DR_SearchFullDataRequest(requester, searchItem);
                    SearchRequestManager searchProcessor = new SearchRequestManager();
                    var searchResult = searchProcessor.Process(request);
                    if (searchResult.Result == Enum_DR_ResultType.SeccessfullyDone)
                        firstSideDataItem = searchResult.ResultDataItems.First();
                    else if (searchResult.Result == Enum_DR_ResultType.ExceptionThrown)
                        throw (new Exception(searchResult.Message));

                    List<EntityInstanceProperty> properties = new List<EntityInstanceProperty>();

                    foreach (var col in relationship.RelationshipColumns)
                    {

                        var value = firstSideDataItem.GetProperty(col.FirstSideColumnID).Value;
                        if (value == null)
                            return null;
                        resultDataItem.Phrases.Add(new SearchProperty(col.SecondSideColumn) { Value = value });

                    }
                    return resultDataItem;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        public bool RelationshipFirstSideColumnExist(DP_BaseData firstSideDataItem, RelationshipDTO relationship)
        {
            var relationshipFirstSideColumnExist = true;

            foreach (var col in relationship.RelationshipColumns)
            {
                if (!firstSideDataItem.Properties.Any(x => x.Value != null && !string.IsNullOrEmpty(x.Value.ToString()) && x.ColumnID == col.FirstSideColumnID))
                    relationshipFirstSideColumnExist = false;

            }
            return relationshipFirstSideColumnExist;
        }

        public DP_SearchRepositoryMain GetSecondSideSearchItemByFirstSideColumns(DP_BaseData firstSideDataItem, RelationshipDTO relationship)
        {
            if (!RelationshipFirstSideColumnExist(firstSideDataItem, relationship))
                return null;
            List<EntityInstanceProperty> properties = new List<EntityInstanceProperty>();
            DP_SearchRepositoryMain resultDataItem = new DP_SearchRepositoryMain(relationship.EntityID2);
            foreach (var col in relationship.RelationshipColumns)
            {

                var value = firstSideDataItem.GetProperty(col.FirstSideColumnID).Value;
                if (value == null)
                    return null;
                resultDataItem.Phrases.Add(new SearchProperty(col.SecondSideColumn) { Value = value });
            }
            return resultDataItem;
        }
    }

    //public enum RelationshipSreachType
    //{
    //    FirstSideBasedOnSecondRelationshhipColumn,
    //    SecondSideBasedOnFirstRelationshhipColumn
    //}
}
