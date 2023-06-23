using DataAccess;
using ModelEntites;
using MyGeneralLibrary;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizBackendActionActivity
    {
        public List<BackendActionActivityDTO> GetBackendActionActivities(int entityID, List<Enum_EntityActionActivityStep> BackendActionActivitySteps, DetailsDepth detailsDepth)
        {
            // BizBackendActionActivity.GetBackendActionActivities: 6a4380912411
            List<BackendActionActivityDTO> result = new List<BackendActionActivityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listBackendActionActivity = projectContext.BackendActionActivity as IQueryable<BackendActionActivity>;

                //اینجا درست بشه
                if (entityID != 0)
                    listBackendActionActivity = listBackendActionActivity.Where(x => x.TableDrivedEntityID == entityID);
                else
                    listBackendActionActivity = listBackendActionActivity.Where(x => x.TableDrivedEntityID == null);


                //if (BackendActionActivityTypes != null && BackendActionActivityTypes.Any())
                //{
                //    List<short> BackendActionActivityTypeIds = new List<short>();
                //    foreach (var BackendActionActivityType in BackendActionActivityTypes)
                //        BackendActionActivityTypeIds.Add((short)BackendActionActivityType);
                //    listBackendActionActivity = listBackendActionActivity.Where(x => BackendActionActivityTypeIds.Contains(x.Type));
                //}
                if (BackendActionActivitySteps != null && BackendActionActivitySteps.Any())
                {
                    List<short> BackendActionActivityStepIds = new List<short>();
                    foreach (var BackendActionActivityStep in BackendActionActivitySteps)
                        BackendActionActivityStepIds.Add((short)BackendActionActivityStep);
                    listBackendActionActivity = listBackendActionActivity.Where(x => BackendActionActivityStepIds.Contains(x.StepType));
                }

                foreach (var item in listBackendActionActivity)
                    result.Add(ToBackendActionActivityDTO(item, detailsDepth));

            }
            return result;
        }
        //public List<BackendActionActivityDTO> GetAllActionActivities(DR_Requester requester, string generalFilter)
        //{
        //    List<BackendActionActivityDTO> result = new List<BackendActionActivityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var listBackendActionActivity = projectContext.BackendActionActivity as IQueryable<BackendActionActivity>; ;

        //        if (generalFilter != "")
        //            listBackendActionActivity = listBackendActionActivity.Where(x => x.ID.ToString() == generalFilter || x.Title.Contains(generalFilter));
        //        foreach (var item in listBackendActionActivity)
        //            result.Add(ToBackendActionActivityDTO(item, DetailsDepth.SimpleInfo));

        //    }
        //    return result;
        //}
        public BackendActionActivityDTO GetBackendActionActivity(int BackendActionActivitysID)
        {
            List<BackendActionActivityDTO> result = new List<BackendActionActivityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var BackendActionActivitys = projectContext.BackendActionActivity.First(x => x.ID == BackendActionActivitysID);
                return ToBackendActionActivityDTO(BackendActionActivitys, DetailsDepth.WithDetails);
            }
        }
        public BackendActionActivityDTO ToBackendActionActivityDTO(BackendActionActivity item, DetailsDepth detailsDepth)
        {
            BackendActionActivityDTO result = new BackendActionActivityDTO();
            result.Type = (Enum_ActionActivityType)item.Type;


            if (detailsDepth != DetailsDepth.SimpleInfo && result.Type == Enum_ActionActivityType.DatabaseFunction)
            {
                result.DatabaseFunctionEntityID = item.DatabaseFunctionEntityID ?? 0;
                if (detailsDepth == DetailsDepth.WithDetailsAndObjects)
                {
                    BizDatabaseFunction bizDatabaseFunction = new MyModelManager.BizDatabaseFunction();
                    result.DatabaseFunctionEntity = bizDatabaseFunction.ToDatabaseFunction_EntityDTO(item.DatabaseFunction_TableDrivedEntity, withDetails);
                }
            }

            if (detailsDepth != DetailsDepth.SimpleInfo && result.Type == Enum_ActionActivityType.CodeFunction)
            {
                result.CodeFunctionID = item.CodeFunctionID ?? 0;
                if (detailsDepth == DetailsDepth.WithDetailsAndObjects)
                {
                    BizCodeFunction bizCodeFunction = new MyModelManager.BizCodeFunction();
                    if (item.CodeFunction != null)
                        result.CodeFunction = bizCodeFunction.ToCodeFunctionDTO(item.CodeFunction, true);
                }
            }


            result.ID = item.ID;
            result.EntityID = item.TableDrivedEntityID ?? 0;
            if (item.TableDrivedEntity != null)
            {
                result.EntityAlias = item.TableDrivedEntity.Alias;
            }
            result.Step = (Enum_EntityActionActivityStep)item.StepType;
            result.ResultSensetive = item.ResultSensetive == true;
            //result.BackendActionActivityType = (Enum_ActionActivityType)item.ActivityType;
            result.Title = item.Title;


            return result;
        }


        public int UpdateBackendActionActivitys(BackendActionActivityDTO BackendActionActivity)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbBackendActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.ID == BackendActionActivity.ID);
                if (dbBackendActionActivity == null)
                    dbBackendActionActivity = new DataAccess.BackendActionActivity();



                if (BackendActionActivity.CodeFunctionID != 0)
                    dbBackendActionActivity.CodeFunctionID = BackendActionActivity.CodeFunctionID;
                else
                    dbBackendActionActivity.CodeFunctionID = null;

                if (BackendActionActivity.DatabaseFunctionEntityID != 0)
                    dbBackendActionActivity.DatabaseFunctionEntityID = BackendActionActivity.DatabaseFunctionEntityID;
                else
                    dbBackendActionActivity.DatabaseFunctionEntityID = null;



                dbBackendActionActivity.ID = BackendActionActivity.ID;
                dbBackendActionActivity.Type = (short)BackendActionActivity.Type;
                //dbBackendActionActivity.ActivityType = (short)BackendActionActivity.BackendActionActivityType;
                dbBackendActionActivity.Title = BackendActionActivity.Title;
                if (BackendActionActivity.EntityID == 0)
                    dbBackendActionActivity.TableDrivedEntityID = null;
                else
                    dbBackendActionActivity.TableDrivedEntityID = BackendActionActivity.EntityID;
                dbBackendActionActivity.StepType = (short)BackendActionActivity.Step;
                dbBackendActionActivity.ResultSensetive = BackendActionActivity.ResultSensetive;
                if (dbBackendActionActivity.ID == 0)
                    projectContext.BackendActionActivity.Add(dbBackendActionActivity);
                projectContext.SaveChanges();
                return dbBackendActionActivity.ID;
            }
        }

        public void DeleteBackendActionActivity(int iD)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbBackendActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.ID == iD);
                projectContext.BackendActionActivity.Remove(dbBackendActionActivity);
                projectContext.SaveChanges();
            }
        }
    }
    public enum Enum_GetBackendActionActivityType
    {
        CodeFunctionOrDatabaseFunction,
        UIActions,
        All
    }
    public enum DetailsDepth
    {
        SimpleInfo,
        WithDetails,
        WithDetailsAndObjects
    }
}
