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
        public List<EntityStateDTO> GetEntityStates(DR_Requester requester, int entityID)
        {
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityState = projectContext.EntityState.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in listEntityState)
                    result.Add(ToEntityStateDTO(requester, item, false));

            }
            return result;
        }

        public List<EntityStateDTO> GetEntityStatesForApply(DR_Requester requester, int entityID, Enum_ApplyState applyState, int toParentRelationshipID = 0)
        {
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityState = projectContext.EntityState.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in listEntityState)
                    result.Add(ToEntityStateDTO(requester, item, true, applyState, toParentRelationshipID));

            }
            return result;
        }
        //public List<EntityStateDTO> GetPreservableEntityStates(int entityID, bool withDetails)
        //{
        //    List<EntityStateDTO> result = new List<EntityStateDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var EntityStates = projectContext.EntityState.First(x => x.ID == entityStatesID);
                return ToEntityStateDTO(requester, EntityStates, withDetails);
            }
        }
        //public bool EntityHasState(DP_DataRepository dataItem, int stateID)
        //{
        //    if (dataItem != null)
        //        using (var projectContext = new DataAccess.MyIdeaEntities())
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

        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var foundDataItem = GetDataItem(projectContext, dataItem);
        //        if (foundDataItem != null)
        //        {
        //            return foundDataItem.DataItem_EntityState.Any(x => x.TableDrivedEntityState.Title == state);
        //        }
        //    }
        //    return false;
        //}

        //private DataItem GetDataItem(DataAccess.MyIdeaEntities projectContext, DP_DataRepository dataItem)
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

        public EntityStateDTO ToEntityStateDTO(DR_Requester requester, EntityState item, bool withDetails, Enum_ApplyState applyState = Enum_ApplyState.None, int toParentRelationshipID = 0)
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

            //if (item.ConditionOperator != null)
            //    result.ConditionOperator = (AndOREqualType)item.ConditionOperator;

            var bizActionActivity = new BizUIActionActivity();
            foreach (var actionActivity in item.EntityState_UIActionActivity)
            {
                result.ActionActivities.Add(bizActionActivity.ToActionActivityDTO(actionActivity.UIActionActivity, withDetails, applyState, toParentRelationshipID));
            }
            if (applyState == Enum_ApplyState.InUI)
            {
                if (result.ActionActivities.Any(x => x.UIEnablityDetails != null && x.UIEnablityDetails.Any(y => y.RelationshipID != 0 && y.RelationshipID == toParentRelationshipID)))
                    result.ApplyOnViewMode = true;
                if (result.ActionActivities.Any(x => x.Type==Enum_ActionActivityType. x.UIEnablityDetails != null && x.UIEnablityDetails.Any(y => y.RelationshipID != 0 && y.RelationshipID == toParentRelationshipID)))
                    result.ApplyOnViewMode = true;
            }

            result.ID = item.ID;
            result.Title = item.Title;

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
            result.RelationshipTailID = item.EntityRelationshipTailID ?? 0;
            if (item.EntityRelationshipTail != null)
            {
                BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
                result.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
            }
            if (item.ValueOpoerator != null)
                result.EntityStateOperator = (InORNotIn)item.ValueOpoerator;

            foreach (var valItem in item.EntityStateValues)
            {
                result.Values.Add(new ModelEntites.EntityStateValueDTO() { Value = valItem.Value, SecurityReservedValue = valItem.ReservedValue == null ? SecurityReservedValue.None : (SecurityReservedValue)valItem.ReservedValue });
            }

            foreach (var valItem in item.EntityStateSecuritySubject)
            {
                result.SecuritySubjects.Add(valItem.SecuritySubjectID);//, SecuritySubjectOperator = (Enum_SecuritySubjectOperator)valItem.SecuritySubjectOperator });
            }
            if (item.SecuritySubjectInOrNotIn == null)
                result.SecuritySubjectInORNotIn = InORNotIn.In;
            else
                result.SecuritySubjectInORNotIn = (InORNotIn)item.SecuritySubjectInOrNotIn;

            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityState, item.ID.ToString(), withDetails.ToString());


            return result;
        }

        //public EntityStateConditionDTO ToEntityStateConditionDTO(DR_Requester requester, EntityStateCondition item, bool withDetails)
        //{
        //    var result = new EntityStateConditionDTO();
        //    result.ID = item.ID;

        //    return result;
        //}

        //public void ClearAndSaveStates(DataItemState item)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
            using (var projectContext = new DataAccess.MyIdeaEntities())
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
                var dbEntityState = projectContext.EntityState.FirstOrDefault(x => x.ID == EntityState.ID);
                if (dbEntityState == null)
                    dbEntityState = new DataAccess.EntityState();




                //if (EntityState.ActionActivityID != 0)
                //    dbEntityState.ActionActivityID = EntityState.ActionActivityID;
                //else
                //    dbEntityState.ActionActivityID = null;
                dbEntityState.TableDrivedEntityID = EntityState.TableDrivedEntityID;
                dbEntityState.ID = EntityState.ID;
                //   dbEntityState.Preserve = EntityState.Preserve;
                dbEntityState.Title = EntityState.Title;
                //dbEntityState.ConditionOperator = (short)EntityState.ConditionOperator;

                while (dbEntityState.EntityState_UIActionActivity.Any())
                {
                    //dbEntityState.EntityState_ActionActivity.Remove(dbEntityState.EntityState_ActionActivity.First());
                    projectContext.EntityState_UIActionActivity.Remove(dbEntityState.EntityState_UIActionActivity.First());
                }
                foreach (var actionActivity in EntityState.ActionActivities)
                {
                    dbEntityState.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivityID = actionActivity.ID });
                }
                //List<EntityStateCondition> removeList = new List<EntityStateCondition>();
                //foreach (var item in dbEntityState.EntityStateCondition)
                //{
                //    if (!EntityState.StateConditions.Any(x => x.ID == item.ID))
                //        removeList.Add(item);
                //}


                while (dbEntityState.EntityStateValues.Any())
                    dbEntityState.EntityStateValues.Remove(dbEntityState.EntityStateValues.First());
                while (dbEntityState.EntityStateSecuritySubject.Any())
                    dbEntityState.EntityStateSecuritySubject.Remove(dbEntityState.EntityStateSecuritySubject.First());



                if (dbEntityState.FormulaID != 0)
                    dbEntityState.FormulaID = EntityState.FormulaID;
                else
                    dbEntityState.FormulaID = null;
                if (EntityState.ColumnID != 0)
                {
                    dbEntityState.ColumnID = EntityState.ColumnID;
                    if (EntityState.RelationshipTailID == 0)
                        dbEntityState.EntityRelationshipTailID = null;
                    else
                        dbEntityState.EntityRelationshipTailID = EntityState.RelationshipTailID;
                }
                else
                {
                    dbEntityState.ColumnID = null;
                    dbEntityState.EntityRelationshipTailID = null;
                }
                dbEntityState.Title = EntityState.Title;
                dbEntityState.ValueOpoerator = (short)EntityState.EntityStateOperator;
                dbEntityState.SecuritySubjectInOrNotIn = (short)EntityState.SecuritySubjectInORNotIn;

                while (dbEntityState.EntityStateValues.Any())
                    projectContext.EntityStateValues.Remove(dbEntityState.EntityStateValues.First());
                foreach (var nItem in EntityState.Values)
                {
                    dbEntityState.EntityStateValues.Add(new EntityStateValues() { Value = nItem.Value, ReservedValue = (short)nItem.SecurityReservedValue });
                }


                while (dbEntityState.EntityStateSecuritySubject.Any())
                    projectContext.EntityStateSecuritySubject.Remove(dbEntityState.EntityStateSecuritySubject.First());
                foreach (var nItem in EntityState.SecuritySubjects)
                {
                    dbEntityState.EntityStateSecuritySubject.Add(new EntityStateSecuritySubject() { SecuritySubjectID = nItem });//, SecuritySubjectOperator = (short)nItem.SecuritySubjectOperator });
                }



                if (dbEntityState.ID == 0)
                    projectContext.EntityState.Add(dbEntityState);
                projectContext.SaveChanges();
                return dbEntityState.ID;
            }
        }
        internal void DoDataBeforeLoadActionActivities(DR_Requester Requester, List<DP_DataView> resultDataItems, bool fullData, int toParentRelationshipID)
        {
            // BizEntityState.DoDataBeforeLoadUIActionActivities: 84133990d0c1
            var entityStates = GetEntityStatesForApply(Requester, resultDataItems.First().TargetEntityID,
                fullData ? Enum_ApplyState.InDataFetchFullData : Enum_ApplyState.InDataFetchDataView, toParentRelationshipID);
            List<Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>> listItems = new List<Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>>();
            foreach (var entityState in entityStates.Where(x => x.ActionActivities != null))
            {
                List<UIEnablityDetailsDTO> listEnablity = new List<UIEnablityDetailsDTO>();
                foreach (var action in entityState.ActionActivities.Where(x => x.UIEnablityDetails != null))
                {
                    foreach (var uiEnablity in action.UIEnablityDetails)
                    {
                        listEnablity.Add(uiEnablity);
                    }
                }
                if (listEnablity.Any())
                {
                    foreach (var dataItem in resultDataItems)
                    {
                        if (CheckEntityState(Requester, dataItem, entityState))
                        {
                            foreach (var uiEnablity in listEnablity)
                            {
                                if (uiEnablity.ColumnID != 0)
                                {
                                    var property = dataItem.Properties.FirstOrDefault(x => x.ColumnID == uiEnablity.ColumnID);
                                    if (property != null)
                                    {
                                        if (uiEnablity.Readonly == true)
                                        {
                                            property.OnLoadIsReadonlyOfState = true;
                                            property.OnLoadIsReadonlyStateTitle += (string.IsNullOrEmpty(property.OnLoadIsReadonlyStateTitle) ? "" : Environment.NewLine + entityState.Title);
                                        }
                                        //else if (uiEnablity.Hidden == true)
                                        //{
                                        //    property.OnLoadIsHiddenOfState = true;
                                        //    property.OnLoadIsHiddenStateTitle += (string.IsNullOrEmpty(property.OnLoadIsReadonlyStateTitle) ? "" : Environment.NewLine + entityState.Title);
                                        //}
                                    }
                                }
                                else if (uiEnablity.RelationshipID != 0)
                                {
                                    if (uiEnablity.RelationshipID == toParentRelationshipID)
                                    {
                                        if (uiEnablity.Readonly == true)
                                        {
                                            dataItem.ParentRelationshipIsReadonlyOnLoad = true;
                                            dataItem.ParentRelationshipIsReadonlyOnLoadText += (string.IsNullOrEmpty(dataItem.ParentRelationshipIsReadonlyOnLoadText) ? "" : ",") + entityState.Title;
                                        }
                                        else if (uiEnablity.Hidden == true)
                                        {
                                            dataItem.ParentRelationshipIsHidenOnLoad = true;
                                            dataItem.ParentRelationshipIsHidenOnLoadText += (string.IsNullOrEmpty(dataItem.ParentRelationshipIsHidenOnLoadText) ? "" : ",") + entityState.Title;
                                        }
                                    }
                                    else
                                    {
                                        if (uiEnablity.Readonly == true)
                                        {
                                            (dataItem as DP_DataRepository).ChildReadonlyRelationships.Add(new Tuple<int, string>(uiEnablity.RelationshipID, entityState.Title));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //internal void DoDataViewBeforeLoadUIActionActivities(DR_Requester Requester, List<DP_DataView> resultDataItems, int toParentRelationshipID)
        //{
        //    var entityStates = GetEntityStates(Requester, resultDataItems.First().TargetEntityID, true, toParentRelationshipID);
        //    List<Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>> listItems = new List<Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>>();
        //    foreach (var entityState in entityStates)
        //    {
        //        List<UIEnablityDetailsDTO> listEnablity = new List<UIEnablityDetailsDTO>();
        //        foreach (var action in entityState.ActionActivities)
        //        {

        //        }
        //        if (listEnablity.Any())
        //        {
        //            foreach (var dataItem in resultDataItems)
        //            {
        //                if (CheckEntityState(Requester, dataItem, entityState))
        //                {
        //                    foreach (var uiEnablity in listEnablity)
        //                    {
        //                        if (uiEnablity.Readonly == true)
        //                        {
        //                            dataItem.ParentRelationshipIsReadonlyOnLoad = true;
        //                            dataItem.ParentRelationshipIsReadonlyOnLoadText += (string.IsNullOrEmpty(dataItem.ParentRelationshipIsReadonlyOnLoadText) ? "" : ",") + entityState.Title;
        //                        }
        //                        else if (uiEnablity.Hidden == true)
        //                        {
        //                            dataItem.ParentRelationshipIsHidenOnLoad = true;
        //                            dataItem.ParentRelationshipIsHidenOnLoadText += (string.IsNullOrEmpty(dataItem.ParentRelationshipIsHidenOnLoadText) ? "" : ",") + entityState.Title;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private bool CheckEntityState(DR_Requester Requester, DP_BaseData dataItem, EntityStateDTO state)
        {
            bool stateIsValid = false;
            StateHandler handler = new StateHandler();
            var stateResult = handler.GetStateResult(state, dataItem, Requester);
            if (string.IsNullOrEmpty(stateResult.Message))
            {
                stateIsValid = stateResult.Result;
            }
            return stateIsValid;
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
