using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLibrary;


namespace MyModelManager
{
    public class BizEntityDataItemReport
    {

        SecurityHelper securityHelper = new SecurityHelper();
        BizEntityReport bizEntityReport = new BizEntityReport();

        public List<EntityDataItemReportDTO> GetEntityDataItemReports(DR_Requester requester, int entityID)
        {
            List<EntityDataItemReportDTO> result = new List<EntityDataItemReportDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var list = projectContext.EntityDataItemReport.Where(x => x.EntityReport.TableDrivedEntityID == entityID);
                foreach (var item in list)
                {
                    if (bizEntityReport.DataIsAccessable(requester, item.EntityReport))
                    {
                        var nItem = new EntityDataItemReportDTO();
                        ToEntityDataItemReportDTO(item, nItem, false);
                        result.Add(nItem);
                    }
                }
            }
            return result;
        }

        internal void ToEntityDataItemReportDTO(EntityDataItemReport entityDataItemReport, EntityDataItemReportDTO entityDataItemReportDTO, bool withDetails)
        {
            bizEntityReport.ToEntityReportDTO(entityDataItemReport.EntityReport, entityDataItemReportDTO, withDetails);
            //entityDataItemReportDTO.SearchRepositoryID = entityDataItemReport.SearchRepositoryID ?? 0;
            //if (entityDataItemReport.SearchRepository != null)
            //    entityDataItemReportDTO.SearchRepository = bizSearchRepository.ToSearchRepositoryDTO(entityDataItemReport.SearchRepository);
            //entityDataItemReportDTO.DataItemReportType = (DataItemReportType)entityDataItemReport.DataItemReportType;
        }

        internal EntityDataItemReport ToNewEntityDataItemReport(EntityDataItemReportDTO message)
        {
            var result = new EntityDataItemReport();
            result.EntityReport = bizEntityReport.ToNewEntityReport(message);
            result.DataItemReportType = (short)message.DataItemReportType;
            return result;
        }
        internal void ToUpdateEntityDataItemReport(EntityDataItemReport entityDataItemReport, EntityDataItemReportDTO message)
        {
            bizEntityReport.ToUpdateEntityReport(entityDataItemReport.EntityReport, message);
        }
    }

}
