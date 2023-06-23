using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizEntityDataViewReport
    {
        public BizEntityDataViewReport()
        {

        }
        BizEntitySearchableReport bizEntitySearchableReport = new BizEntitySearchableReport();

        public List<EntityDataViewReportDTO> GetEntityDataViewReports(DR_Requester requester, int entityID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Validation, entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntityDataViewReportDTO>);

            List<EntityDataViewReportDTO> result = new List<EntityDataViewReportDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityDataViewReport = projectContext.EntityDataViewReport.Where(x => x.EntitySearchableReport.EntityReport.TableDrivedEntityID == entityID);
                foreach (var item in listEntityDataViewReport)
                    result.Add(ToEntityDataViewReportDTO( requester, item, false));

            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Validation, entityID.ToString());
            return result;
        }
        public EntityDataViewReportDTO GetEntityDataViewReport(DR_Requester requester, int EntityDataViewReportsID, bool withDetails)
        {
            List<EntityDataViewReportDTO> result = new List<EntityDataViewReportDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbItem = projectContext.EntityDataViewReport.First(x => x.ID == EntityDataViewReportsID);
                if (bizEntityReport.DataIsAccessable(requester, dbItem.EntitySearchableReport.EntityReport))
                {
                    return ToEntityDataViewReportDTO( requester, dbItem, withDetails);
                }
                else
                    return null;
            }
        }
        BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();

        public EntityDataViewReportDTO ToEntityDataViewReportDTO(DR_Requester requester, EntityDataViewReport item, bool withDetails)
        {
            EntityDataViewReportDTO result = new EntityDataViewReportDTO();
            bizEntitySearchableReport.ToEntitySearchableReportDTO( requester, item.EntitySearchableReport, result, withDetails);
            result.DataMenuSettingID = item.DataMenuSettingID;
            result.EntityListViewID = item.EntitySearchableReport.EntityReport.EntityListViewID ?? 0;
            //         result.SearchRepositoryID = item.EntitySearchableReport.SearchRepositoryID ?? 0;
            return result;
        }
        public void UpdateEntityDataViewReports(EntityDataViewReportDTO message)
        {
            BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {

                var dbEntitySpecifiedReport = projectContext.EntityDataViewReport.FirstOrDefault(x => x.ID == message.ID);
                if (dbEntitySpecifiedReport == null)
                {
                    message.ReportType = ReportType.SearchableReport;
                    message.SearchableReportType = SearchableReportType.DataView;
                    dbEntitySpecifiedReport = new EntityDataViewReport();
                    dbEntitySpecifiedReport.EntitySearchableReport = bizEntitySearchableReport.ToNewEntitySearchableReport(message);
                }
                else
                    bizEntitySearchableReport.ToUpdateEntitySearchableReport(dbEntitySpecifiedReport.EntitySearchableReport, message);

                dbEntitySpecifiedReport.DataMenuSettingID = message.DataMenuSettingID;
                dbEntitySpecifiedReport.EntitySearchableReport.EntityReport.EntityListViewID  = message.EntityListViewID;
                //    dbEntitySpecifiedReport.EntitySearchableReport.SearchRepositoryID = message.SearchRepositoryID;
                //while (dbEntityDataViewReport.EntityDataViewReportSubs1.Any())
                //    projectContext.EntityDataViewReportSubs.Remove(dbEntityDataViewReport.EntityDataViewReportSubs1.First());
                //foreach (var sub in message.EntityDataViewReportSubs)
                //{
                //    EntityDataViewReportSubs rColumn = new EntityDataViewReportSubs();
                //    rColumn.Title = sub.Title;
                //    rColumn.ChildEntityDataViewReportID = sub.EntityDataViewReportID;
                //    rColumn.OrderID = sub.OrderID;
                //    rColumn.RelationshipID = sub.RelationshipID;
                //    dbEntityDataViewReport.EntityDataViewReportSubs1.Add(rColumn);
                //}

                if (dbEntitySpecifiedReport.ID == 0)
                    projectContext.EntityDataViewReport.Add(dbEntitySpecifiedReport);
                projectContext.SaveChanges();
            }
        }
    }

}
