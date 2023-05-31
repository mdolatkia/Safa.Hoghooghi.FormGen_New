using ModelEntites;
using MyDataSearchManagerBusiness;
using MyRelationshipDataManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class DataitemRelatedColumnValueHandler
    {
        public object GetValueSomeHow(DR_Requester requester, DP_BaseData sentdata, EntityRelationshipTailDTO valueRelationshipTail, int valueColumnID, bool firstIfMultiple = false)
        {
            if (valueRelationshipTail == null)
            {
                var proprty = sentdata.GetProperty(valueColumnID);
                return proprty?.Value;
            }
            else
            {
                DP_BaseData relatedData = null;
                if (sentdata is DP_DataRepository)
                {
                  
                    if ((sentdata as DP_DataRepository).ParantChildRelationshipData != null && (sentdata as DP_DataRepository).ParantChildRelationshipData.ToParentRelationshipID == valueRelationshipTail.Relationship.ID)
                    {
                        if ((sentdata as DP_DataRepository).ParantChildRelationshipData.ToParentRelationshipID == valueRelationshipTail.Relationship.ID)
                            relatedData = (sentdata as DP_DataRepository).ParantChildRelationshipData.SourceData;
                    }
                    else if ((sentdata as DP_DataRepository).ChildRelationshipDatas.Any(x => x.Relationship.ID == valueRelationshipTail.Relationship.ID))
                    {
                        var childInfo = (sentdata as DP_DataRepository).ChildRelationshipDatas.First(x => x.Relationship.ID == valueRelationshipTail.Relationship.ID);
                        if (childInfo.RelatedData.Count > 1)
                        {
                            if (firstIfMultiple)
                                relatedData = childInfo.RelatedData.First();
                            else
                                throw new Exception("asav");
                        }
                        else if (childInfo.RelatedData.Count == 1)
                            relatedData = childInfo.RelatedData.First();
                        else if (childInfo.RelatedData.Count == 0)
                        {
                            //یعنی یا داده مرتبطی وجود نداشته یا حذف شده
                            return "";
                        }
                    }
                }
                if (relatedData != null)
                    return GetValueSomeHow(requester, relatedData, valueRelationshipTail.ChildTail, valueColumnID, firstIfMultiple);
                else
                {
                    //var columnValues = sentdata.KeyProperties;
                    //if (columnValues == null || columnValues.Count == 0)
                    //    throw new Exception("asasd");

                    //سکوریتی داده اعمال میشود
                    //  var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();

                    //این شرط منطقیه؟؟
                    if (!sentdata.IsNewItem)
                    {
                        var relationshipTailDataManager = new RelationshipTailDataManager();
                        var searchDataTuple = relationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(sentdata, valueRelationshipTail);
                        DR_SearchFullDataRequest request = new DR_SearchFullDataRequest(requester, searchDataTuple);
                        var searchResult = new SearchRequestManager().Process(request);
                        if (searchResult.ResultDataItems.Count > 1)
                        {
                            throw new Exception("asdasd");
                        }
                        else if (searchResult.ResultDataItems.Count == 1)
                        {
                            var foundDataItem = searchResult.ResultDataItems.First();
                            var prop = foundDataItem.GetProperty(valueColumnID);
                            if (prop != null)
                                return prop.Value;
                            else
                                return "";
                        }
                        else
                            return "";
                    }
                    else
                        return "";
                }

            }
            //return "";
        }
    }
}
