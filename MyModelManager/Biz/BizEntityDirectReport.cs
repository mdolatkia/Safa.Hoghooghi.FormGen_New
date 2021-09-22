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
    public class BizEntityDirectReport
    {
        BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();
        public List<EntityDirectReportDTO> GetEntityDirectReports(DR_Requester requester, int entityID)
        {
            List<EntityDirectReportDTO> result = new List<EntityDirectReportDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var list = projectContext.EntityDirectlReport.Where(x => x.EntityDataItemReport.EntityReport.TableDrivedEntityID == entityID);
                foreach (var item in list)
                {
                    if (bizEntityReport.DataIsAccessable(requester, item.EntityDataItemReport.EntityReport))
                        result.Add(ToEntityDirectReportDTO(requester, item, false));
                }
            }
            return result;
        }

        public EntityDirectReportDTO GetEntityDirectReport(DR_Requester requester, int iD)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var item = projectContext.EntityDirectlReport.First(x => x.ID == iD);
                if (bizEntityReport.DataIsAccessable(requester, item.EntityDataItemReport.EntityReport))
                {
                    var rItem = ToEntityDirectReportDTO(requester, item, true);
                    return rItem;
                }
                else
                    return null;
            }
        }




        public EntityDirectReportDTO ToEntityDirectReportDTO(DR_Requester requester, EntityDirectlReport dbRel, bool withDetails)
        {
            var result = new EntityDirectReportDTO();
            bizEntityReport.ToEntityReportDTO(dbRel.EntityDataItemReport.EntityReport, result, withDetails);
            result.URL = dbRel.URL;
            foreach (var item in dbRel.EntityDirectlReportParameters)
            {
                result.EntityDirectlReportParameters.Add(new EntityDirectlReportParameterDTO()
                {
                    ColumnID = item.ColumnID,
                    ID = item.ID,
                    ParameterName = item.ParameterName
                });
            }
            return result;
        }

        public int UpdateEntityDirectReport(EntityDirectReportDTO message)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                //BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();
                BizEntityDataItemReport bizEntityDataItemReport = new BizEntityDataItemReport();
                var dbEntitySpecifiedReport = projectContext.EntityDirectlReport.FirstOrDefault(x => x.ID == message.ID);
                if (dbEntitySpecifiedReport == null)
                {
                    message.ReportType = ReportType.DataItemReport;
                    dbEntitySpecifiedReport = new EntityDirectlReport();
                    dbEntitySpecifiedReport.EntityDataItemReport = bizEntityDataItemReport.ToNewEntityDataItemReport(message);
                    dbEntitySpecifiedReport.EntityDataItemReport.DataItemReportType = (short)DataItemReportType.DirectReport;
                }
                else
                    bizEntityDataItemReport.ToUpdateEntityDataItemReport(dbEntitySpecifiedReport.EntityDataItemReport, message);

                dbEntitySpecifiedReport.URL = message.URL;
                while (dbEntitySpecifiedReport.EntityDirectlReportParameters.Any())
                    projectContext.EntityDirectlReportParameters.Remove(dbEntitySpecifiedReport.EntityDirectlReportParameters.First());
                foreach (var item in message.EntityDirectlReportParameters)
                {
                    dbEntitySpecifiedReport.EntityDirectlReportParameters.Add(new EntityDirectlReportParameters()
                    {
                        ColumnID = item.ColumnID,
                        ParameterName = item.ParameterName
                    });
                }
                if (dbEntitySpecifiedReport.ID == 0)
                    projectContext.EntityDirectlReport.Add(dbEntitySpecifiedReport);
                projectContext.SaveChanges();
                return dbEntitySpecifiedReport.ID;
            }
        }
    }

}
