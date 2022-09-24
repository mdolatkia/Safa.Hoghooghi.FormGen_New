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
    public class BizGraph
    {
        BizEntityReport bizEntityReport = new MyModelManager.BizEntityReport();
        SecurityHelper securityHelper = new SecurityHelper();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();

        public GraphDTO GetGraph(DR_Requester requester, int ID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbItem = projectContext.GraphDefinition.First(x => x.ID == ID);
                if (!DataIsAccessable(requester, dbItem))
                    return null;
                else
                    return ToGraphDTO(dbItem, true);
            }
        }
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        public bool DataIsAccessable(DR_Requester requester, GraphDefinition dataLink)
        {
            if (requester.SkipSecurity)
                return true;


            if (!bizEntityReport.DataIsAccessable(requester, dataLink.EntityDataItemReport.EntityReport))
                return false;

            //اینجا تیل چک نمیشه
            foreach (var tail in dataLink.GraphDefinition_EntityRelationshipTail)
            {
                if (!bizEntityRelationshipTail.DataIsAccessable(requester, tail.EntityRelationshipTail))
                    return false;
            }
            return true;
        }

        public List<GraphDTO> GetGraphByEntitiyID(DR_Requester requester, int entitiyID)
        {
            List<GraphDTO> result = new List<GraphDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var items = projectContext.GraphDefinition.Where(x => x.EntityDataItemReport.EntityReport.TableDrivedEntityID == entitiyID);
                foreach (var dbItem in items)
                {
                    if (DataIsAccessable(requester, dbItem))
                        result.Add(ToGraphDTO(dbItem, false));
                }
            }

            return result;
        }

        public List<GraphDTO> SearchGraphs(DR_Requester requester, string singleFilterValue)
        {
            List<GraphDTO> result = new List<GraphDTO>();
            using (var projectContext = new MyIdeaEntities())
            {
                var items = projectContext.GraphDefinition.Where(x => x.EntityDataItemReport.EntityReport.Title.Contains(singleFilterValue) || x.EntityDataItemReport.EntityReport.TableDrivedEntity.Alias.Contains(singleFilterValue)
                || x.EntityDataItemReport.EntityReport.TableDrivedEntity.Name.Contains(singleFilterValue));
                foreach (var dbItem in items)
                {
                    if (DataIsAccessable(requester, dbItem))
                        result.Add(ToGraphDTO(dbItem, false));
                }
            }
            return result;
        }



        private GraphDTO ToGraphDTO(GraphDefinition item, bool withDetails)
        {
            GraphDTO result = new GraphDTO();
            bizEntityReport.ToEntityReportDTO(item.EntityDataItemReport.EntityReport, result, withDetails);
            result.NotJointEntities = item.NotJointEntities == true;
            result.FirstSideDataMenuID = item.FirstSideDataMenuID ?? 0;
            if (withDetails)
            {
                BizEntityRelationshipTail bizEntityRelationshipTail = new MyModelManager.BizEntityRelationshipTail();
                BizEntityRelationshipTailDataMenu bizEntityRelationshipTailListView = new MyModelManager.BizEntityRelationshipTailDataMenu();

                foreach (var dbRel in item.GraphDefinition_EntityRelationshipTail)
                {
                    var rel = new GraphRelationshipTailDTO();
                    rel.RelationshipTailID = dbRel.EntityRelationshipTailID;
                    rel.EntityRelationshipTailDataMenuID = dbRel.EntityRelationshipTailDataMenuID ?? 0;
                    if (rel.EntityRelationshipTailDataMenuID != 0)
                        rel.EntityRelationshipTailDataMenu = bizEntityRelationshipTailListView.ToEntityRelationshipTailDataMenuDTO(dbRel.EntityRelationshipTailDataMenu, true);
                    rel.ID = dbRel.ID;
                    rel.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(dbRel.EntityRelationshipTail);
                    result.RelationshipsTails.Add(rel);
                }
            }



            return result;
        }
        public int UpdateGraph(GraphDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                BizEntityDataItemReport bizEntityDataItemReport = new BizEntityDataItemReport();

                var dbEntity = projectContext.GraphDefinition.FirstOrDefault(x => x.ID == message.ID);
                if (dbEntity == null)
                {
                    message.ReportType = ReportType.DataItemReport;
                    dbEntity = new GraphDefinition();
                    dbEntity.EntityDataItemReport = bizEntityDataItemReport.ToNewEntityDataItemReport(message);
                    dbEntity.EntityDataItemReport.DataItemReportType = (short)DataItemReportType.GraphReport;
                }
                else
                    bizEntityDataItemReport.ToUpdateEntityDataItemReport(dbEntity.EntityDataItemReport, message);

                if (message.FirstSideDataMenuID != 0)
                    dbEntity.FirstSideDataMenuID = message.FirstSideDataMenuID;
                else
                    dbEntity.FirstSideDataMenuID = null;

                dbEntity.NotJointEntities = message.NotJointEntities;
                while (dbEntity.GraphDefinition_EntityRelationshipTail.Any())
                    projectContext.GraphDefinition_EntityRelationshipTail.Remove(dbEntity.GraphDefinition_EntityRelationshipTail.First());
                foreach (var item in message.RelationshipsTails)
                {
                    var dbRel = new GraphDefinition_EntityRelationshipTail();
                    dbRel.EntityRelationshipTailID = item.RelationshipTailID;

                    if (item.EntityRelationshipTailDataMenuID != 0)
                        dbRel.EntityRelationshipTailDataMenuID = item.EntityRelationshipTailDataMenuID;

                    //   dbRel.FromFirstSideToSecondSide = item.FromFirstSideToSecondSide;
                    dbEntity.GraphDefinition_EntityRelationshipTail.Add(dbRel);
                }

                if (dbEntity.ID == 0)
                    projectContext.GraphDefinition.Add(dbEntity);
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
