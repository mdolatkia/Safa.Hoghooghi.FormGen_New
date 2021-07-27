using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyConnectionManager;
using MyGeneralLibrary;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizEntityExternalReport
    {
        BizEntitySearchableReport bizEntitySearchableReport = new BizEntitySearchableReport();

        //public List<EntityExternalReportDTO> GetEntityExternalReports()
        //{
        //    List<EntityExternalReportDTO> result = new List<EntityExternalReportDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var listEntityListReport = projectContext.EntityExternalReport;
        //        foreach (var item in listEntityListReport)
        //            result.Add(ToEntityExternalReportDTO(item, false));

        //    }
        //    return result;
        //}
        public List<EntityExternalReportDTO> GetEntityExternalReports(int entityID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Validation, entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntityExternalReportDTO>);

            List<EntityExternalReportDTO> result = new List<EntityExternalReportDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var listEntityListReport = projectContext.EntityExternalReport.Where(x => x.EntitySearchableReport.EntityReport.TableDrivedEntityID == entityID);
                foreach (var item in listEntityListReport)
                    result.Add(ToEntityExternalReportDTO(item, false));

            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Validation, entityID.ToString());
            return result;
        }
        public EntityExternalReportDTO GetEntityExternalReport(int EntityListReportsID, bool withDetails)
        {
            List<EntityExternalReportDTO> result = new List<EntityExternalReportDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var EntityListReports = projectContext.EntityExternalReport.First(x => x.ID == EntityListReportsID);
                return ToEntityExternalReportDTO(EntityListReports, withDetails);
            }
        }
        BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();

        public EntityExternalReportDTO ToEntityExternalReportDTO(EntityExternalReport item, bool withDetails)
        {
            EntityExternalReportDTO result = new EntityExternalReportDTO();
            result.ID = item.ID;
            result.URL = item.URL;
            bizEntitySearchableReport.ToEntitySearchableReportDTO(item.EntitySearchableReport, result, withDetails);


            return result;
        }
        public void UpdateEntityExternalReports(DR_Requester requester, EntityExternalReportDTO message)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();

                var dbEntitySpecifiedReport = projectContext.EntityExternalReport.FirstOrDefault(x => x.ID == message.ID);
                if (dbEntitySpecifiedReport == null)
                {
                    message.ReportType = ReportType.SearchableReport;
                    message.SearchableReportType = SearchableReportType.ExternalReport;
                    dbEntitySpecifiedReport = new EntityExternalReport();
                    dbEntitySpecifiedReport.EntitySearchableReport = bizEntitySearchableReport.ToNewEntitySearchableReport(message);
                }
                else
                    bizEntitySearchableReport.ToUpdateEntitySearchableReport(dbEntitySpecifiedReport.EntitySearchableReport, message);

                dbEntitySpecifiedReport.URL = message.URL;
                if (dbEntitySpecifiedReport.ID == 0)
                    projectContext.EntityExternalReport.Add(dbEntitySpecifiedReport);

                var entity = new BizTableDrivedEntity().GetSimpleEntityWithColumns(requester, message.TableDrivedEntityID);
                CreateReportTable(entity);
                //ساختن جدول معادل

                projectContext.SaveChanges();
            }
        }
        public bool CreateReportTable(DR_Requester requester, int entityID)
        {
            var entity = new BizTableDrivedEntity().GetSimpleEntityWithColumns(requester, entityID);
            return CreateReportTable(entity);
        }
        private bool CreateReportTable(TableDrivedEntityDTO entity)
        {
            var dbHelper = ConnectionManager.GetDBHelper(entity.DatabaseID);
            var tmpTableName = "xr_" + entity.TableName;
            try
            {
                var drop = "drop table " + tmpTableName;
                var dropRes = dbHelper.ExecuteNonQuery(drop);
            }
            catch
            {

            }

            var res = dbHelper.CreateTable(tmpTableName, GetExternalReportTableColumns(entity));
            return res != 0;
            //SearchRequestManager searchRequestManager = new SearchRequestManager();
            //var query = searchRequestManager.GetSelectFromExternal(requester, entityID, searchItem, true, ModelEntites.SecurityMode.View);
            //var entity = bizTableDrivedEntity.GetSimpleEntity(requester, entityID);


            //var select = "select " + query.Item1 + "," + id + " as ReportKey" + " " + query.Item2;

            //var inserted = TargetDatabaseManager.InsertFromQueryToTargetDB(entityID, tmpTableName, select);
            //if (inserted)
            //    return id;
        }
        List<ColumnDTO> GetExternalReportTableColumns(TableDrivedEntityDTO entity)
        {
            var columns = entity.Columns.Where(x => x.PrimaryKey).ToList();
            columns.Add(new ColumnDTO() { Name = "ReportKey", ColumnType = Enum_ColumnType.Numeric, DataType = "int" });
            return columns;
        }
        public bool InsertDataIntoExternalReportTable(DR_Requester requester, int reportID, int entityID, int id, string fromquery)
        {
            var entity = new BizTableDrivedEntity().GetSimpleEntityWithColumns(requester, entityID);
            var tmpTableName = "xr_" + entity.TableName;
            var columns = GetExternalReportTableColumns(entity);
            var selectColumns = "";
            foreach (var item in columns)
            {
                if (item.Name != "ReportKey")
                {
                    selectColumns += (selectColumns == "" ? "" : ",") + entity.TableName + "." + item.Name;
                }
                else
                {
                    selectColumns += (selectColumns == "" ? "" : ",") + id + " as ReportKey";
                }
            }
            var query = "select " + selectColumns + " " + fromquery;


            var dbHelper = ConnectionManager.GetDBHelper(entity.DatabaseID);

            string result = "";

            result = "Insert into " + tmpTableName + " " + query;

            dbHelper.ExecuteNonQuery(result);

            return true;


        }
    }

}
