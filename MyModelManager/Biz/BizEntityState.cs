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
    public class BizEntityState
    {
        public List<EntityStateDTO> GetEntityStates(DR_Requester requester, int entityID, bool withDetails)
        {
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var listEntityState = projectContext.TableDrivedEntityState.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in listEntityState)
                    result.Add(ToEntityStateDTO(requester, item, withDetails));

            }
            return result;
        }
        //public List<EntityStateDTO> GetPreservableEntityStates(int entityID, bool withDetails)
        //{
        //    List<EntityStateDTO> result = new List<EntityStateDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var listEntityState = projectContext.TableDrivedEntityState.Where(x => x.TableDrivedEntityID == entityID && x.Preserve == true);
        //        foreach (var item in listEntityState)
        //            result.Add(ToEntityStateDTO(item, withDetails));

        //    }
        //    return result;
        //}

        //public List<EntityStateDTO> GetEntityStatesWithServerSideActionActivities(int entityID, bool withDetails)
        //{
        //    List<EntityStateDTO> result = new List<EntityStateDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var listEntityState = projectContext.TableDrivedEntityState.Where(x => x.TableDrivedEntityID == entityID);
        //        listEntityState = listEntityState.Where(x => x.EntityState_ActionActivity.Any(y => y.ActionActivity.Type == 0 || y.ActionActivity.Type == 1) || x.Preserve == true);
        //        foreach (var item in listEntityState)
        //            foreach (var actionActivity in item.EntityState_ActionActivity.Where(y => y.ActionActivity.Type != 0 && y.ActionActivity.Type != 1).ToList())
        //                item.EntityState_ActionActivity.Remove(actionActivity);
        //        foreach (var item in listEntityState)
        //            result.Add(ToEntityStateDTO(item, withDetails));

        //    }
        //    return result;
        //}
        public EntityStateDTO GetEntityState(DR_Requester requester, int entityStatesID, bool withDetails)
        {
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var EntityStates = projectContext.TableDrivedEntityState.First(x => x.ID == entityStatesID);
                return ToEntityStateDTO(requester, EntityStates, withDetails);
            }
        }
        //public bool EntityHasState(DP_DataRepository dataItem, int stateID)
        //{
        //    if (dataItem != null)
        //        using (var projectContext = new DataAccess.MyProjectEntities())
        //        {
        //            var foundDataItem = GetDataItem(projectContext, dataItem);
        //            if (foundDataItem != null)
        //            {
        //                return foundDataItem.DataItem_EntityState.Any(x => x.TableDrivedEntityStateID == stateID);
        //            }
        //        }
        //    return false;
        //}
        //public bool EntityHasState(DP_DataRepository dataItem, string state)
        //{

        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var foundDataItem = GetDataItem(projectContext, dataItem);
        //        if (foundDataItem != null)
        //        {
        //            return foundDataItem.DataItem_EntityState.Any(x => x.TableDrivedEntityState.Title == state);
        //        }
        //    }
        //    return false;
        //}

        //private DataItem GetDataItem(DataAccess.MyProjectEntities projectContext, DP_DataRepository dataItem)
        //{
        //    if (dataItem != null)
        //    {
        //        var keyColumns = dataItem.KeyProperties;
        //        if (keyColumns.Any())
        //        {
        //            var dataItems = projectContext.DataItem.Where(x => x.TableDrivedEntityID == dataItem.TargetEntityID);
        //            foreach (var keyColumn in keyColumns)
        //                dataItems = dataItems.Where(x => x.DataItemKeyColumns.Any(y => y.ColumnID == keyColumn.ColumnID && y.Value == keyColumn.Value));
        //            return dataItems.FirstOrDefault();
        //        }
        //    }
        //    return null;
        //}

        public EntityStateDTO ToEntityStateDTO(DR_Requester requester, TableDrivedEntityState item, bool withDetails)
        {
            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityState, item.ID.ToString(), withDetails.ToString());
            if (cachedItem != null)
                return (cachedItem as EntityStateDTO);

            EntityStateDTO result = new EntityStateDTO();

            result.TableDrivedEntityID = item.TableDrivedEntityID;
            //result.Preserve = item.Preserve;
            //result.ActionActivityID = item.ActionActivityID ?? 0;
            //if (result.ActionActivityID != 0 && withDetails)
            //{
            //    var bizActionActivity = new BizActionActivity();
            //    result.ActionActivity = bizActionActivity.GetActionActivity(item.ActionActivityID.Value);
            //}

            if (item.ConditionOperator != null)
                result.ConditionOperator = (AndOREqualType)item.ConditionOperator;

            var bizActionActivity = new BizUIActionActivity();
            foreach (var actionActivity in item.EntityState_UIActionActivity)
            {
                result.ActionActivities.Add(bizActionActivity.ToActionActivityDTO(actionActivity.UIActionActivity, withDetails));
            }
            result.ID = item.ID;
            result.Title = item.Title;
            foreach (var dbCondition in item.TableDrivedEntityStateCondition)
            {
                result.StateConditions.Add(ToEntityStateConditionDTO(requester, dbCondition, withDetails));
            }

            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityState, item.ID.ToString(), withDetails.ToString());


            return result;
        }

        public EntityStateConditionDTO ToEntityStateConditionDTO(DR_Requester requester, TableDrivedEntityStateCondition item, bool withDetails)
        {
            var result = new EntityStateConditionDTO();
            result.ID = item.ID;
            result.FormulaID = item.FormulaID ?? 0;
            if (result.FormulaID != 0 && withDetails)
            {  //??با جزئیات؟؟........................................................................ 
                var bizFormula = new BizFormula();
                result.Formula = bizFormula.GetFormula(requester, item.FormulaID.Value, withDetails);
            }
            result.ColumnID = item.ColumnID ?? 0;
            if (item.Column != null)
            {
                BizColumn bizColumn = new BizColumn();
                result.Column = bizColumn.ToColumnDTO(item.Column, true);

            }
            result.Title = item.Title;
            result.RelationshipTailID = item.EntityRelationshipTailID ?? 0;
            if (item.EntityRelationshipTail != null)
            {
                BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
                result.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
            }
            if (item.ValueOpoerator != null)
                result.EntityStateOperator = (Enum_EntityStateOperator)item.ValueOpoerator;

            foreach (var valItem in item.TableDrivedEntityStateConditionValues)
            {
                result.Values.Add(new ModelEntites.EntityStateValueDTO() { Value = valItem.Value, SecurityReservedValue = valItem.ReservedValue == null ? SecurityReservedValue.None : (SecurityReservedValue)valItem.ReservedValue });
            }

            foreach (var valItem in item.TableDrivedEntityStateConditionSecuritySubject)
            {
                result.SecuritySubjects.Add(new ChildSecuritySubjectDTO { SecuritySubjectID = valItem.SecuritySubjectID });//, SecuritySubjectOperator = (Enum_SecuritySubjectOperator)valItem.SecuritySubjectOperator });
            }
            if (item.SecuritySubjectInOrNotIn == null)
                result.SecuritySubjectInORNotIn = InORNotIn.In;
            else
                result.SecuritySubjectInORNotIn = (InORNotIn)item.SecuritySubjectInOrNotIn;
            return result;
        }

        //public void ClearAndSaveStates(DataItemState item)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var foundDataItem = GetDataItem(projectContext, item.DataItem);
        //        if (foundDataItem != null)
        //        {
        //            foundDataItem = new DataItem() { TableDrivedEntityID = item.DataItem.TargetEntityID };
        //            var keyColumns = item.DataItem.KeyProperties;
        //            foreach (var keyColumn in keyColumns)
        //                foundDataItem.DataItemKeyColumns.Add(new DataItemKeyColumns() { ColumnID = keyColumn.ColumnID, Value = keyColumn.Value });
        //            projectContext.DataItem.Add(foundDataItem);
        //        }
        //        while (foundDataItem.DataItem_EntityState.Any())
        //            projectContext.DataItem_EntityState.Remove(foundDataItem.DataItem_EntityState.First());
        //        foreach (var state in item.States.Where(x => x.Preserve))
        //            foundDataItem.DataItem_EntityState.Add(new DataItem_EntityState() { TableDrivedEntityStateID = state.ID });
        //        projectContext.SaveChanges();
        //    }
        //}

        public int UpdateEntityStates(EntityStateDTO EntityState)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                //if (EntityState.RelationshipTailID != 0)

                //{
                //    var stateTail = projectContext.EntityRelationshipTail.First(x => x.ID == EntityState.RelationshipTailID);
                //    foreach (var actionActivity in EntityState.ActionActivities)
                //    {
                //        var actionActivityEntity = projectContext.UIActionActivity.First(x => x.ID == actionActivity.ID);
                //        if (actionActivityEntity.UIEnablityDetails.Any(x => stateTail.RelationshipPath == x.RelationshipID.ToString() ||
                //             stateTail.RelationshipPath.StartsWith(x.RelationshipID.ToString())))
                //        {
                //            var actionItem = actionActivityEntity.UIEnablityDetails.First(x => stateTail.RelationshipPath == x.RelationshipID.ToString() ||
                //                 stateTail.RelationshipPath.StartsWith(x.RelationshipID.ToString()));
                //            var relationshipAlias = actionItem.Relationship.Alias;
                //            throw new Exception("امکان استفاده از رابطه" + " " + relationshipAlias + " " + "در اقدام" + " "
                //                + actionItem.UIActionActivity.Title + " " + "به علت وجود اشتراک با رشته رابطه وضعیت وجود ندارد");

                //            //زیرا تناقض است. از طرفی رشته رابطه جزو خصوصیات موجودیت برای تعیین وضعیت هست و از طرفی میخواهیم فقط خواندنی و یا مخفی کنیم
                //            //اگر در لود شدن فرمهای ورود اطلاعات فرض کنیم رشته رابطه پرنت و خود موجودیت فرم چایلد باشد منطقی است اما اگر 
                //            // فرض کنیم که رشته رابطه چایلد باشد و موجودیت اصلی خود این موجودیات باشد آنوقت برای تغییر رابطه باید به صورت مصنوعی رشته رابطه را تغییر دهیم تا رابطه قابل اصلاح شود
                //            // آنوقت با تغییر رابطه دیگر تغییراتی که در رشته رابطه یا وضعیت داده ایم هیچوقت ثبت نخواهد شد چون ریموو شده اند
                //            // پس بهتر است اشتراکی وجود نداشته باشد و وضعیت آزادانه تغییر کند

                //        }
                //    }
                //}


                //foreach (var condition in EntityState.StateConditions)
                //{
                //    if (condition.RelationshipTailID == 0 && condition.ColumnID != 0)
                //    {
                //        foreach (var actionActivity in EntityState.ActionActivities)
                //        {
                //            var actionActivityEntity = projectContext.UIActionActivity.First(x => x.ID == actionActivity.ID);
                //            if (actionActivityEntity.UIEnablityDetails.Any(x => x.ColumnID == condition.ColumnID))
                //            {
                //                var actionItem = actionActivityEntity.UIEnablityDetails.First(x => x.ColumnID == condition.ColumnID);
                //                var columnAlias = actionItem.Column.Alias;
                //                throw new Exception("امکان استفاده از ستون" + " " + columnAlias + " " + "در اقدام" + " "
                //                    + actionItem.UIActionActivity.Title + " " + "به علت همسان بودن با ستون تعیین وضعیت وجود ندارد");
                //                //ستون تعیین وضعیت نمیتواند خود مخفی یا فقط خواندنی باشد زیرا دیگر نمی تواند مقدار بگیرد
                //                // باید دسترسی مخصوص در صورت نیاز برای این ستون تعریف شود
                //            }
                //        }
                //    }
                //}
                var dbEntityState = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.ID == EntityState.ID);
                if (dbEntityState == null)
                    dbEntityState = new DataAccess.TableDrivedEntityState();




                //if (EntityState.ActionActivityID != 0)
                //    dbEntityState.ActionActivityID = EntityState.ActionActivityID;
                //else
                //    dbEntityState.ActionActivityID = null;
                dbEntityState.TableDrivedEntityID = EntityState.TableDrivedEntityID;
                dbEntityState.ID = EntityState.ID;
                //   dbEntityState.Preserve = EntityState.Preserve;
                dbEntityState.Title = EntityState.Title;
                dbEntityState.ConditionOperator = (short)EntityState.ConditionOperator;

                while (dbEntityState.EntityState_UIActionActivity.Any())
                {
                    //dbEntityState.EntityState_ActionActivity.Remove(dbEntityState.EntityState_ActionActivity.First());
                    projectContext.EntityState_UIActionActivity.Remove(dbEntityState.EntityState_UIActionActivity.First());
                }
                foreach (var actionActivity in EntityState.ActionActivities)
                {
                    dbEntityState.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivityID = actionActivity.ID });
                }
                List<TableDrivedEntityStateCondition> removeList = new List<TableDrivedEntityStateCondition>();
                foreach (var item in dbEntityState.TableDrivedEntityStateCondition)
                {
                    if (!EntityState.StateConditions.Any(x => x.ID == item.ID))
                        removeList.Add(item);
                }
                foreach (var item in removeList)
                {
                    while (item.TableDrivedEntityStateConditionValues.Any())
                        item.TableDrivedEntityStateConditionValues.Remove(item.TableDrivedEntityStateConditionValues.First());
                    while (item.TableDrivedEntityStateConditionSecuritySubject.Any())
                        item.TableDrivedEntityStateConditionSecuritySubject.Remove(item.TableDrivedEntityStateConditionSecuritySubject.First());
                    projectContext.TableDrivedEntityStateCondition.Remove(item);
                }
                foreach (var condition in EntityState.StateConditions)
                {
                    TableDrivedEntityStateCondition dbCondition = null;
                    if (condition.ID == 0)
                    {
                        dbCondition = new TableDrivedEntityStateCondition();
                        dbEntityState.TableDrivedEntityStateCondition.Add(dbCondition);
                    }
                    else
                        dbCondition = projectContext.TableDrivedEntityStateCondition.First(x => x.ID == condition.ID);

                    if (condition.FormulaID != 0)
                        dbCondition.FormulaID = condition.FormulaID;
                    else
                        dbCondition.FormulaID = null;
                    if (condition.ColumnID != 0)
                    {
                        dbCondition.ColumnID = condition.ColumnID;
                        if (condition.RelationshipTailID == 0)
                            dbCondition.EntityRelationshipTailID = null;
                        else
                            dbCondition.EntityRelationshipTailID = condition.RelationshipTailID;
                    }
                    else
                    {
                        dbCondition.ColumnID = null;
                        dbCondition.EntityRelationshipTailID = null;
                    }
                    dbCondition.Title = condition.Title;
                    dbCondition.ValueOpoerator = (short)condition.EntityStateOperator;
                    dbCondition.SecuritySubjectInOrNotIn = (short)condition.SecuritySubjectInORNotIn;

                    while (dbCondition.TableDrivedEntityStateConditionValues.Any())
                        projectContext.TableDrivedEntityStateConditionValues.Remove(dbCondition.TableDrivedEntityStateConditionValues.First());
                    foreach (var nItem in condition.Values)
                    {
                        dbCondition.TableDrivedEntityStateConditionValues.Add(new TableDrivedEntityStateConditionValues() { Value = nItem.Value, ReservedValue = (short)nItem.SecurityReservedValue });
                    }


                    while (dbCondition.TableDrivedEntityStateConditionSecuritySubject.Any())
                        projectContext.TableDrivedEntityStateConditionSecuritySubject.Remove(dbCondition.TableDrivedEntityStateConditionSecuritySubject.First());
                    foreach (var nItem in condition.SecuritySubjects)
                    {
                        dbCondition.TableDrivedEntityStateConditionSecuritySubject.Add(new TableDrivedEntityStateConditionSecuritySubject() { SecuritySubjectID = nItem.SecuritySubjectID });//, SecuritySubjectOperator = (short)nItem.SecuritySubjectOperator });
                    }

                }

                if (dbEntityState.ID == 0)
                    projectContext.TableDrivedEntityState.Add(dbEntityState);
                projectContext.SaveChanges();
                return dbEntityState.ID;
            }
        }
    }
    public class DataItemState
    {
        public DataItemState()
        {
            States = new List<EntityStateDTO>();
        }
        public DP_DataRepository DataItem { set; get; }
        public List<EntityStateDTO> States { set; get; }

    }
}
