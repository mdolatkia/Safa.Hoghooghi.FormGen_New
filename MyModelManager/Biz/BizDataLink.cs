using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizDataLink
    {
        BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();
        SecurityHelper securityHelper = new SecurityHelper();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();

        public DataLinkDTO GetDataLink(DR_Requester requester, int ID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbItem = projectContext.DataLinkDefinition.First(x => x.ID == ID);
                if (!DataIsAccessable(requester, dbItem))
                    return null;
                else
                    return ToDataLinkDTO( requester, dbItem, true);
            }
        }
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        public bool DataIsAccessable(DR_Requester requester, DataLinkDefinition dataLink)
        {
            if (requester.SkipSecurity)
                return true;

            if (!bizTableDrivedEntity.DataIsAccessable(requester, dataLink.TableDrivedEntity))
                return false;
            if (!bizEntityReport.DataIsAccessable(requester, dataLink.EntityDataItemReport.EntityReport))
                return false;

            //اینجا تیل چک نمیشه
            foreach (var tail in dataLink.DataLinkDefinition_EntityRelationshipTail)
            {
                if (!bizEntityRelationshipTail.DataIsAccessable(requester, tail.EntityRelationshipTail))
                    return false;
            }
            return true;
        }

        public List<DataLinkDTO> GetDataLinkByEntitiyID(DR_Requester requester, int entitiyID)
        {
            List<DataLinkDTO> result = new List<DataLinkDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var items = projectContext.DataLinkDefinition.Where(x => x.EntityDataItemReport.EntityReport.TableDrivedEntityID == entitiyID
                || x.SecondSideEntityID == entitiyID);
                foreach (var dbItem in items)
                {
                    if (DataIsAccessable(requester, dbItem))
                        result.Add(ToDataLinkDTO( requester, dbItem, false));
                }
            }

            return result;
        }

        public List<DataLinkDTO> SearchDatalinks(DR_Requester requester, string singleFilterValue)
        {
            List<DataLinkDTO> result = new List<DataLinkDTO>();
            using (var projectContext = new MyIdeaEntities())
            {
                var items = projectContext.DataLinkDefinition.Where(x => x.EntityDataItemReport.EntityReport.Title.Contains(singleFilterValue) || x.EntityDataItemReport.EntityReport.TableDrivedEntity.Alias.Contains(singleFilterValue)
                || x.EntityDataItemReport.EntityReport.TableDrivedEntity.Name.Contains(singleFilterValue) || x.TableDrivedEntity.Alias.Contains(singleFilterValue) || x.TableDrivedEntity.Name.Contains(singleFilterValue));
                foreach (var dbItem in items)
                {
                    if (DataIsAccessable(requester, dbItem))
                        result.Add(ToDataLinkDTO( requester, dbItem, false));
                }
            }
            return result;
        }



        private DataLinkDTO ToDataLinkDTO(DR_Requester requester, DataLinkDefinition item, bool withDetails)
        {
            DataLinkDTO result = new DataLinkDTO();
            bizEntityReport.ToEntityReportDTO( requester, item.EntityDataItemReport.EntityReport, result, withDetails);
            result.SecondSideEntityID = item.SecondSideEntityID;
            result.NotJointEntities = item.NotJointEntities == true;
            result.FirstSideDataMenuID = item.FirstSideDataMenuID ?? 0;
            result.SecondSideDataMenuID = item.SecondSideDataMenuID ?? 0;
            if (withDetails)
            {
                BizEntityRelationshipTail bizEntityRelationshipTail = new MyModelManager.BizEntityRelationshipTail();
                BizEntityRelationshipTailDataMenu bizEntityRelationshipTailListView = new MyModelManager.BizEntityRelationshipTailDataMenu();

                foreach (var dbRel in item.DataLinkDefinition_EntityRelationshipTail)
                {
                    var rel = new DataLinkRelationshipTailDTO();
                    rel.RelationshipTailID = dbRel.EntityRelationshipTailID;
                    rel.EntityRelationshipTailDataMenuID = dbRel.EntityRelationshipTailDataMenuID ?? 0;
                    if (rel.EntityRelationshipTailDataMenuID != 0)
                        rel.EntityRelationshipTailDataMenu = bizEntityRelationshipTailListView.ToEntityRelationshipTailDataMenuDTO(dbRel.EntityRelationshipTailDataMenu,true);
                    //   rel.FromFirstSideToSecondSide = dbRel.FromFirstSideToSecondSide;
                    rel.ID = dbRel.ID;
                    rel.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(dbRel.EntityRelationshipTail);
                    result.RelationshipsTails.Add(rel);
                }
            }



            return result;
        }
        public int UpdateDataLink(DataLinkDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                BizEntityDataItemReport bizEntityDataItemReport = new BizEntityDataItemReport();

                var dbEntity = projectContext.DataLinkDefinition.FirstOrDefault(x => x.ID == message.ID);
                if (dbEntity == null)
                {
                    message.ReportType = ReportType.DataItemReport;
                    dbEntity = new DataLinkDefinition();
                    dbEntity.EntityDataItemReport = bizEntityDataItemReport.ToNewEntityDataItemReport(message);
                    dbEntity.EntityDataItemReport.DataItemReportType = (short)DataItemReportType.DataLinkReport;
                }
                else
                    bizEntityDataItemReport.ToUpdateEntityDataItemReport(dbEntity.EntityDataItemReport, message);

                dbEntity.SecondSideEntityID = message.SecondSideEntityID;
                dbEntity.NotJointEntities = message.NotJointEntities;

                if (message.FirstSideDataMenuID != 0)
                    dbEntity.FirstSideDataMenuID = message.FirstSideDataMenuID;
                else
                    dbEntity.FirstSideDataMenuID = null;
                if (message.SecondSideDataMenuID != 0)
                    dbEntity.SecondSideDataMenuID = message.SecondSideDataMenuID;
                else
                    dbEntity.SecondSideDataMenuID = null;

                while (dbEntity.DataLinkDefinition_EntityRelationshipTail.Any())
                    projectContext.DataLinkDefinition_EntityRelationshipTail.Remove(dbEntity.DataLinkDefinition_EntityRelationshipTail.First());
                foreach (var item in message.RelationshipsTails)
                {
                    var dbRel = new DataLinkDefinition_EntityRelationshipTail();
                    dbRel.EntityRelationshipTailID = item.RelationshipTailID;
                    if (item.EntityRelationshipTailDataMenuID != 0)
                        dbRel.EntityRelationshipTailDataMenuID = item.EntityRelationshipTailDataMenuID;
                    //   dbRel.FromFirstSideToSecondSide = item.FromFirstSideToSecondSide;
                    dbEntity.DataLinkDefinition_EntityRelationshipTail.Add(dbRel);
                }

                if (dbEntity.ID == 0)
                    projectContext.DataLinkDefinition.Add(dbEntity);

                try
                {
                    projectContext.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    //result.Result = Enum_DR_ResultType.ExceptionThrown;
                    //result.Message = "خطا در ثبت" + Environment.NewLine + ex.Message;
                }
                return dbEntity.ID;
            }
        }
    }

}
