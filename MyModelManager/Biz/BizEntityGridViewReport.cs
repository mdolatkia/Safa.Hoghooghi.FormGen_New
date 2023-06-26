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
    public class BizEntityGridViewReport
    {
        public BizEntityGridViewReport()
        {

        }
        BizEntitySearchableReport bizEntitySearchableReport = new BizEntitySearchableReport();

        public List<EntityGridViewReportDTO> GetEntityGridViewReports(DR_Requester requester)
        {
            List<EntityGridViewReportDTO> result = new List<EntityGridViewReportDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityGridViewReport = projectContext.EntityGridViewReport;
                foreach (var item in listEntityGridViewReport)
                    result.Add(ToEntityGridViewReportDTO( requester, item, false));

            }
            return result;
        }
        public List<EntityGridViewReportDTO> GetEntityGridViewReports(DR_Requester requester, int entityID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Validation, entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntityGridViewReportDTO>);

            List<EntityGridViewReportDTO> result = new List<EntityGridViewReportDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityGridViewReport = projectContext.EntityGridViewReport.Where(x => x.EntitySearchableReport.EntityReport.TableDrivedEntityID == entityID);
                foreach (var item in listEntityGridViewReport)
                    result.Add(ToEntityGridViewReportDTO( requester, item, false));

            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Validation, entityID.ToString());
            return result;
        }
        public EntityGridViewReportDTO GetEntityGridViewReport(DR_Requester requester, int EntityGridViewReportsID, bool withDetails)
        {
            List<EntityGridViewReportDTO> result = new List<EntityGridViewReportDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbItem = projectContext.EntityGridViewReport.First(x => x.ID == EntityGridViewReportsID);
                if (bizEntityReport.DataIsAccessable(requester, dbItem.EntitySearchableReport.EntityReport))
                {
                    return ToEntityGridViewReportDTO( requester, dbItem, withDetails);
                }
                else
                    return null;
            }
        }
        BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();

        public EntityGridViewReportDTO ToEntityGridViewReportDTO(DR_Requester requester, EntityGridViewReport item, bool withDetails)
        {
            EntityGridViewReportDTO result = new EntityGridViewReportDTO();
            bizEntitySearchableReport.ToEntitySearchableReportDTO( requester, item.EntitySearchableReport, result, withDetails);
            return result;
        }
        public void UpdateEntityGridViewReports(EntityGridViewReportDTO message)
        {
            BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbEntitySpecifiedReport = projectContext.EntityGridViewReport.FirstOrDefault(x => x.ID == message.ID);
                if (dbEntitySpecifiedReport == null)
                {
                    message.ReportType = ReportType.SearchableReport;
                    message.SearchableReportType = SearchableReportType.GridView;
                    dbEntitySpecifiedReport = new EntityGridViewReport();
                    dbEntitySpecifiedReport.EntitySearchableReport = bizEntitySearchableReport.ToNewEntitySearchableReport(message);
                }
                else
                    bizEntitySearchableReport.ToUpdateEntitySearchableReport(dbEntitySpecifiedReport.EntitySearchableReport, message);


                if (message.DataMenuSettingID != 0)
                    dbEntitySpecifiedReport.EntitySearchableReport.EntityReport.DataMenuSettingID = message.DataMenuSettingID;
                else
                    dbEntitySpecifiedReport.EntitySearchableReport.EntityReport.DataMenuSettingID = null;
                dbEntitySpecifiedReport.EntitySearchableReport.EntityReport.EntityListViewID  = message.EntityListViewID;
                //while (dbEntityGridViewReport.EntityGridViewReportSubs1.Any())
                //    projectContext.EntityGridViewReportSubs.Remove(dbEntityGridViewReport.EntityGridViewReportSubs1.First());
                //foreach (var sub in message.EntityGridViewReportSubs)
                //{
                //    EntityGridViewReportSubs rColumn = new EntityGridViewReportSubs();
                //    rColumn.Title = sub.Title;
                //    rColumn.ChildEntityGridViewReportID = sub.EntityGridViewReportID;
                //    rColumn.OrderID = sub.OrderID;
                //    rColumn.RelationshipID = sub.RelationshipID;
                //    dbEntityGridViewReport.EntityGridViewReportSubs1.Add(rColumn);
                //}

                if (dbEntitySpecifiedReport.ID == 0)
                    projectContext.EntityGridViewReport.Add(dbEntitySpecifiedReport);
                projectContext.SaveChanges();
            }
        }
    }

}
