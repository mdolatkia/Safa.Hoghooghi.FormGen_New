﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelEntites;
using ProxyLibrary;
using MyUILibrary.EntityArea.Commands;
using CommonDefinitions.UISettings;
using System.Collections.ObjectModel;
using System.Linq;
namespace MyUILibrary.EntityArea
{
    public class UIActionActivityManager : I_UIActionActivityManager
    {
        public List<DataAndStates> ListDataAndStates = new List<DataAndStates>();
        I_EditEntityArea EditArea { set; get; }
        public UIActionActivityManager(I_EditEntityArea editArea)
        {
            EditArea = editArea;
            //EditArea.DataItemLoaded += EditArea_DataItemLoaded;
            EditArea.DataItemShown += EditArea_DataItemShown;
            EditArea.UIGenerated += EditArea_UIGenerated;
            //     EditArea.DataItemUnShown += EditArea_DataItemUnShown;

        }

        private void EditArea_UIGenerated(object sender, EventArgs e)
        {
            CheckISADeterminerStates();
            CheckUnionDeterminerStates();
            CheckDirectDataSecurityStates();
        }

        private void CheckDirectDataSecurityStates()
        {

            if (AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.EntityHasDirectSecurities(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID))
            {
                var dataDirectSecurity = AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.GetEntitySecurityDirects(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID);
                foreach (var item in dataDirectSecurity)
                {
                    item.EntityState.ConditionOperator = AgentHelper.GetNotOperator(item.EntityState.ConditionOperator);
                    item.EntityState.ActionActivities.Add(new UIActionActivityDTO() { Type = Enum_ActionActivityType.EntityReadonly });

                    EditArea.EntityStates1.Add(item.EntityState);
                }
            }
            //else if (AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.EntityHasInDirectSecurities(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID))
            //{
            //    var dataInDirectSecurity = AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.GetEntitySecurityInDirect(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID);
            //    var dataDirectSecurity = AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.GetEntitySecurityDirects(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), dataInDirectSecurity.RelationshipTail.TargetEntityID);

            //    foreach (var item in dataDirectSecurity)
            //    {
            //        item.EntityState.ConditionOperator = AgentHelper.GetNotOperator(item.EntityState.ConditionOperator);
            //        SetEntityStateForIndirectSecurity(dataInDirectSecurity, item);
            //        item.EntityState.ActionActivities.Add(new UIActionActivityDTO() { Type = Enum_ActionActivityType.EntityReadonly });
            //        EditArea.EntityStates1.Add(item.EntityState);
            //    }

            //}

            //if (AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.EntityHasDirectSecurities(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID, DataDirectSecurityMode.ReadonlyData))
            //{
            //    //همه اینها میتوانند یک فانکشن شوند

            //    var dataDirectSecurity = AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.GetEntitySecurityDirects(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID);
            //    if (dataDirectSecurity.Any(x => x.Mode == DataDirectSecurityMode.FetchData))
            //    {
            //        EntityStateGroupDTO group = new EntityStateGroupDTO();
            //        group.AndORType = AndOREqualType.OrNotTrue;
            //        foreach (var item in dataDirectSecurity.Where(x => x.Mode == DataDirectSecurityMode.FetchData))
            //        {
            //            group.EntityStates.Add(AddDirectSecurityState(item, null));
            //        }
            //        group.ActionActivities.Add(new UIActionActivityDTO() { Type = Enum_ActionActivityType.EntityReadonly });

            //        EditArea.EntityStateGroups.Add(group);
            //    }
            //    if (dataDirectSecurity.Any(x => x.Mode == DataDirectSecurityMode.ReadonlyData))
            //    {
            //        EntityStateGroupDTO group = new EntityStateGroupDTO();
            //        group.AndORType = AndOREqualType.OrNotTrue;
            //        foreach (var item in dataDirectSecurity.Where(x => x.Mode == DataDirectSecurityMode.ReadonlyData))
            //        {
            //            group.EntityStates.Add(AddDirectSecurityState(item, null));
            //        }
            //        group.ActionActivities.Add(new UIActionActivityDTO() { Type = Enum_ActionActivityType.EntityReadonly });

            //        EditArea.EntityStateGroups.Add(group);
            //    }


            //}
            //else if (AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.EntityHasInDirectSecurities(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID, DataDirectSecurityMode.ReadonlyData))
            //{
            //    var dataInDirectSecurity = AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.GetEntitySecurityInDirect(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID);
            //    if (dataInDirectSecurity != null)
            //    {
            //        var dataDirectSecurity = AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.GetEntitySecurityDirects(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), dataInDirectSecurity.RelationshipTail.TargetEntityID);
            //        if (dataDirectSecurity.Any(x => x.Mode == DataDirectSecurityMode.FetchData))
            //        {
            //            EntityStateGroupDTO group = new EntityStateGroupDTO();
            //            group.AndORType = AndOREqualType.OrNotTrue;
            //            foreach (var item in dataDirectSecurity.Where(x => x.Mode == DataDirectSecurityMode.FetchData))
            //            {
            //                group.EntityStates.Add(AddDirectSecurityState(item, dataInDirectSecurity.RelationshipTail));
            //            }
            //            group.ActionActivities.Add(new UIActionActivityDTO() { Type = Enum_ActionActivityType.EntityReadonly });

            //            EditArea.EntityStateGroups.Add(group);
            //        }
            //        if (dataDirectSecurity.Any(x => x.Mode == DataDirectSecurityMode.ReadonlyData))
            //        {
            //            EntityStateGroupDTO group = new EntityStateGroupDTO();
            //            group.AndORType = AndOREqualType.OrNotTrue;
            //            foreach (var item in dataDirectSecurity.Where(x => x.Mode == DataDirectSecurityMode.ReadonlyData))
            //            {
            //                group.EntityStates.Add(AddDirectSecurityState(item, dataInDirectSecurity.RelationshipTail));
            //            }
            //            group.ActionActivities.Add(new UIActionActivityDTO() { Type = Enum_ActionActivityType.EntityReadonly });

            //            EditArea.EntityStateGroups.Add(group);
            //        }
            //    }
            //}
        }

        //private void SetEntityStateForIndirectSecurity(EntitySecurityInDirectDTO dataInDirectSecurity, EntitySecurityDirectDTO item)
        //{
        //    foreach (var codition in item.EntityState.StateConditions)
        //    {
        //        codition.RelationshipTailID = dataInDirectSecurity.RelationshipTailID;
        //        codition.RelationshipTail = dataInDirectSecurity.RelationshipTail;
        //    }
        //}

        //private EntityStateDTO AddDirectSecurityState(EntitySecurityDirectDTO item, EntityRelationshipTailDTO relationshipTail)
        //{
        //    var state = new EntityStateDTO();
        //    state.ID = -1 * item.ID;
        //    state.Title = "فقط خواندنی سازی" + (!string.IsNullOrEmpty(item.Description) ? Environment.NewLine + item.Description : "");
        //    state.ColumnID = item.ColumnID;
        //    state.Column = item.Column;
        //    state.FormulaID = item.FormulaID;
        //    state.Formula = item.Formula;
        //    state.RelationshipTailID = item.RelationshipTailID;
        //    state.RelationshipTail = item.RelationshipTail;
        //    if (relationshipTail != null)
        //    {
        //        if (state.RelationshipTail == null)
        //            state.RelationshipTail = relationshipTail;
        //        else
        //        {
        //            state.RelationshipTail = AgentUICoreMediator.GetAgentUICoreMediator.RelationshipTailDataManager.JoinRelationshipTail(relationshipTail, state.RelationshipTail);
        //        }
        //    }
        //    foreach (var value in item.Values)
        //        state.Values.Add(value);
        //    state.EntityStateOperator = item.ValueOperator;
        //    foreach (var subject in item.SecuritySubjects)
        //        state.SecuritySubjects.Add(subject);
        //    state.SecuritySubjectInORNotIn = item.SecuritySubjectInORNotIn;
        //    return state;
        //}

        private void CheckUnionDeterminerStates()
        {
            if (EditArea.RelationshipColumnControls.Any(x => x.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion))
            {
                foreach (var item in EditArea.RelationshipColumnControls.Where(x => x.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion))
                {
                    var superToSubRel = item.Relationship as UnionToSubUnionRelationshipDTO;
                    if (superToSubRel.DeterminerColumnID != 0 && !string.IsNullOrEmpty(superToSubRel.DeterminerColumnValue))
                    {
                        var state = new EntityStateDTO();
                        state.ID = -1 * superToSubRel.ID;
                        EntityStateConditionDTO condition = new EntityStateConditionDTO();
                        state.StateConditions.Add(condition);
                        condition.ColumnID = superToSubRel.DeterminerColumnID;
                        condition.Values.Add(new EntityStateValueDTO() { Value = superToSubRel.DeterminerColumnValue });
                        condition.EntityStateOperator = Enum_EntityStateOperator.NotEquals;
                        var actionActivity = new UIActionActivityDTO();
                        actionActivity.ID = -1 * superToSubRel.ID;
                        actionActivity.Type = Enum_ActionActivityType.UIEnablity;
                        actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = superToSubRel.ID });
                        state.ActionActivities.Add(actionActivity);
                        EditArea.EntityStates1.Add(state);
                    }
                }
            }
            if (EditArea.AreaInitializer.SourceRelationColumnControl != null && EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
            {

                var subToSuperRel = EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO;
                if (subToSuperRel.DeterminerColumnID != 0 && !string.IsNullOrEmpty(subToSuperRel.DeterminerColumnValue))
                {
                    var state = new EntityStateDTO();
                    state.ID = -1 * subToSuperRel.ID;
                    EntityStateConditionDTO condition = new EntityStateConditionDTO();
                    state.StateConditions.Add(condition);
                    condition.ColumnID = subToSuperRel.DeterminerColumnID;
                    condition.Values.Add(new EntityStateValueDTO() { Value = subToSuperRel.DeterminerColumnValue });
                    condition.EntityStateOperator = Enum_EntityStateOperator.NotEquals;
                    var actionActivity = new UIActionActivityDTO();
                    actionActivity.ID = -1 * subToSuperRel.ID;
                    actionActivity.Type = Enum_ActionActivityType.UIEnablity;
                    actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = subToSuperRel.PairRelationshipID });
                    state.ActionActivities.Add(actionActivity);
                    EditArea.EntityStates1.Add(state);


                    //شرط اگر داده جدید باشد
                    var setDeterminerState = new EntityStateDTO();
                    setDeterminerState.ID = -2 * subToSuperRel.ID;
                    EntityStateConditionDTO condition1 = new EntityStateConditionDTO();
                    setDeterminerState.StateConditions.Add(condition1);
                    condition1.ColumnID = subToSuperRel.DeterminerColumnID;
                    condition1.Values.Add(new EntityStateValueDTO() { Value = "!@#$#" });
                    condition1.EntityStateOperator = Enum_EntityStateOperator.NotEquals;
                    var setDeterminerActionActivity = new UIActionActivityDTO();
                    setDeterminerActionActivity.ID = -2 * subToSuperRel.ID;
                    setDeterminerActionActivity.Type = Enum_ActionActivityType.ColumnValue;
                    setDeterminerActionActivity.UIColumnValue.Add(new UIColumnValueDTO() { ColumnID = subToSuperRel.DeterminerColumnID, ExactValue = subToSuperRel.DeterminerColumnValue, EvenHasValue = true });
                    setDeterminerState.ActionActivities.Add(setDeterminerActionActivity);
                    EditArea.EntityStates1.Add(setDeterminerState);

                }

            }
        }

        private void CheckISADeterminerStates()
        {
            if (EditArea.RelationshipColumnControls.Any(x => x.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub))
            {
                foreach (var item in EditArea.RelationshipColumnControls.Where(x => x.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub))
                {
                    var superToSubRel = item.Relationship as SuperToSubRelationshipDTO;
                    if (superToSubRel.SuperEntityDeterminerColumn != null && superToSubRel.DeterminerColumnValues.Any())
                    {
                        var state = new EntityStateDTO();
                        state.ID = -1 * superToSubRel.ID;
                        EntityStateConditionDTO condition = new EntityStateConditionDTO();
                        state.StateConditions.Add(condition);
                        condition.ColumnID = superToSubRel.SuperEntityDeterminerColumn.ID;
                        foreach (var val in superToSubRel.DeterminerColumnValues)
                            condition.Values.Add(new EntityStateValueDTO() { Value = val.Value });
                        condition.EntityStateOperator = Enum_EntityStateOperator.NotEquals;
                        var actionActivity = new UIActionActivityDTO();
                        actionActivity.ID = -1 * superToSubRel.ID;
                        actionActivity.Type = Enum_ActionActivityType.UIEnablity;
                        actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = superToSubRel.ID });
                        state.ActionActivities.Add(actionActivity);
                        EditArea.EntityStates1.Add(state);
                    }
                }
            }
            if (EditArea.AreaInitializer.SourceRelationColumnControl != null && EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
            {

                var subToSuperRel = EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO;
                if (subToSuperRel.SuperEntityDeterminerColumn != null && subToSuperRel.DeterminerColumnValues.Any())
                {

                    var state = new EntityStateDTO();
                    state.ID = -1 * subToSuperRel.ID;
                    EntityStateConditionDTO condition = new EntityStateConditionDTO();
                    state.StateConditions.Add(condition);
                    condition.ColumnID = subToSuperRel.SuperEntityDeterminerColumn.ID;
                    foreach (var val in subToSuperRel.DeterminerColumnValues)
                        condition.Values.Add(new EntityStateValueDTO() { Value = val.Value });
                    condition.EntityStateOperator = Enum_EntityStateOperator.NotEquals;
                    var actionActivity = new UIActionActivityDTO();
                    actionActivity.ID = -1 * subToSuperRel.ID;
                    actionActivity.Type = Enum_ActionActivityType.UIEnablity;
                    actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = subToSuperRel.PairRelationshipID });
                    state.ActionActivities.Add(actionActivity);
                    EditArea.EntityStates1.Add(state);


                    //شرط اگر داده جدید باشد
                    // این برای اینکه مقدار تعیین کننده با توجه به فرزند خودکار پر شود
                    //منها در صورت چند مقداری بودن اولی انتخاب میشود. تست شود که فقط یکبار ست شود و اگر کاربر چیز دیگه ای انتخاب کرد مقدار با اولی مجددا ست نشود
                    var setDeterminerState = new EntityStateDTO();
                    setDeterminerState.ID = -2 * subToSuperRel.ID;
                    EntityStateConditionDTO condition1 = new EntityStateConditionDTO();
                    setDeterminerState.StateConditions.Add(condition1);
                    condition1.ColumnID = subToSuperRel.SuperEntityDeterminerColumn.ID;
                    condition1.Values.Add(new EntityStateValueDTO() { Value = "!@#$#" });
                    condition1.EntityStateOperator = Enum_EntityStateOperator.NotEquals;
                    var setDeterminerActionActivity = new UIActionActivityDTO();
                    setDeterminerActionActivity.ID = -2 * subToSuperRel.ID;
                    setDeterminerActionActivity.Type = Enum_ActionActivityType.ColumnValue;
                    setDeterminerActionActivity.UIColumnValue.Add(new UIColumnValueDTO() { ColumnID = subToSuperRel.SuperEntityDeterminerColumn.ID, ExactValue = subToSuperRel.DeterminerColumnValues.First().Value });//, EvenHasValue = true });
                    setDeterminerState.ActionActivities.Add(setDeterminerActionActivity);
                    EditArea.EntityStates1.Add(setDeterminerState);

                }

            }

        }

        bool statesSet = false;
        private void SetEntityStates()
        {
            if (statesSet)
                return;
            statesSet = true;

            //افزودن تمام کنترلها و روابط برای حالت ریدونلی کردن داده
            //بیخیال شدم چون غیر فعال میکرد حتی کنترلی که منشا وضعیت بود رو و فرم قفل میشد
            //if (EditArea.EntityStates1.Any(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
            //{
            //    foreach (var item in EditArea.EntityStates1.Where(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
            //    {
            //        var newActionActivity = GetReadonlyActionActivity();
            //        newActionActivity.Type = Enum_ActionActivityType.UIEnablity;
            //        item.ActionActivities.Add(newActionActivity);
            //    }
            //}


            //if (EditArea.EntityStateGroups.Any(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
            //{
            //    foreach (var item in EditArea.EntityStateGroups.Where(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
            //    {
            //        var newActionActivity = GetReadonlyActionActivity();
            //        newActionActivity.Type = Enum_ActionActivityType.UIEnablity;
            //        item.ActionActivities.Add(newActionActivity);
            //    }
            //}
        }

        //private UIActionActivityDTO GetReadonlyActionActivity()
        //{
        //    var newActionActivity = new UIActionActivityDTO();
        //    foreach (var column in EditArea.DataEntryEntity.Columns)
        //    {
        //        if (!column.PrimaryKey)
        //        {
        //            //اگر با فرمول غیر فعال شود چی خود ستونی که باعث ریدونلی شدن می شود
        //            if (!EditArea.DataEntryEntity.Relationships.Any(x => x.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.RelationshipColumns.Any(y => y.SecondSideColumnID == column.ID)))
        //            {
        //                //bool skip = false;
        //                //foreach (var item in states)
        //                //{
        //                //    if (item.RelationshipTailID == 0 && item.ColumnID != 0)
        //                //    {
        //                //        if (item.ColumnID == column.ID)
        //                //            skip = true;
        //                //    }
        //                //}
        //                //if (!skip)
        //                //{
        //                var detail = new UIEnablityDetailsDTO();
        //                detail.ColumnID = column.ID;
        //                detail.Readonly = true;
        //                detail.ID = column.ID;
        //                newActionActivity.UIEnablityDetails.Add(detail);
        //                //}
        //            }
        //        }
        //    }
        //    foreach (var relationship in EditArea.DataEntryEntity.Relationships.Where(x => x.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary))
        //    {
        //        //////if (item.RelationshipTail != null)
        //        //////{
        //        //همون قضیه که خود رابطه ک=ه با وضعیت مشترکه نمیتونه اعمال بشه
        //        //if (item.RelationshipTail.Relationship.ID == relationship.ID)
        //        //    continue;
        //        //////}
        //        var detail = new UIEnablityDetailsDTO();
        //        detail.RelationshipID = relationship.ID;
        //        detail.Readonly = true;
        //        detail.ID = relationship.ID;
        //        newActionActivity.UIEnablityDetails.Add(detail);
        //    }
        //    return newActionActivity;
        //}

        private void EditArea_DataItemShown(object sender, EditAreaDataItemLoadedArg e)
        {
            //////if (EditArea.AreaInitializer.SourceRelationColumnControl != null && e.InEditMode)
            //////    CheckParentReadonlyTail(e.DataItem);
            //حتمکا اینجا چک شود مخصوصا برای تمپ ویو ها که تک داده ای نیز باشند
            if ((EditArea.EntityStates1 == null || EditArea.EntityStates1.Count == 0))
                //&&
                //(EditArea.EntityStateGroups == null || EditArea.EntityStateGroups.Count == 0))
                return;
            SetEntityStates();

            foreach (var state in GetAllStates(EditArea))
            {
                CheckDataItemChangeMonitors(e.DataItem, state);
            }
            CheckAndImposeEntityStates(e.DataItem, ActionActivitySource.OnShowData);

        }

        private List<EntityStateDTO> GetAllStates(I_EditEntityArea editArea)
        {
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            foreach (var state in EditArea.EntityStates1)
            {
                result.Add(state);
            }
            //if (EditArea.EntityStateGroups != null)
            //{
            //    foreach (var stateGroup in EditArea.EntityStateGroups)
            //    {
            //        foreach (var state in stateGroup.EntityStates)
            //        {
            //            result.Add(state);
            //        }
            //    }
            //}
            return result;
        }

        //private void CheckParentReadonlyTail(DP_FormDataRepository data)
        //{
        //    var parentReadonlyTail = GetParentReadonlyTails(data.ParantChildRelationshipInfo);

        //    foreach (var item in EditArea.RelationshipColumnControls)
        //    {
        //        var childRel = data.ChildRelationshipInfos.FirstOrDefault(x => x.Relationship.ID == item.Relationship.ID);
        //        if (childRel != null)
        //        {
        //            if (parentReadonlyTail.Any(x => x.Item2.Contains(x.Item1 + "," + item.Relationship.ID)))
        //            {
        //                childRel.IsReadonlyBecauseOfParentTail = true;
        //                EditArea.MakeRelatoinsipColumnReadonlyFromState(childRel, data, item, "فقط خواندنی به واسطه رشته رابطه");

        //            }
        //            else
        //            {
        //                EditArea.MakeRelatoinsipColumnUnReadonlyFromState(childRel, data, item, "فقط خواندنی به واسطه رشته رابطه");

        //            }

        //        }
        //    }
        //}

        //private List<Tuple<string, string>> GetParentReadonlyTails(ChildRelationshipInfo ParantChildRelationshipInfo, string tail = "", List<Tuple<string, string>> result = null)
        //{
        //    if (result == null)
        //        result = new List<Tuple<string, string>>();
        //    if (tail == "")
        //        tail = ParantChildRelationshipInfo.Relationship.ID.ToString();
        //    else
        //        tail = ParantChildRelationshipInfo.Relationship.ID + "," + tail;

        //    if (ParantChildRelationshipInfo.ReadonlyStateFromTails.Any(x => x.Contains(tail)))
        //    {
        //        foreach (var item in ParantChildRelationshipInfo.ReadonlyStateFromTails.Where(x => x.Contains(tail)))
        //        {
        //            result.Add(new Tuple<string, string>(tail, item));
        //        }
        //    }
        //    else
        //    {
        //        if (ParantChildRelationshipInfo.SourceData.ParantChildRelationshipInfo == null)
        //            return result;
        //        else
        //            GetParentReadonlyTails(ParantChildRelationshipInfo.SourceData.ParantChildRelationshipInfo, tail, result);
        //    }
        //    return result;
        //}

        public void CheckAndImposeEntityStates(DP_FormDataRepository dataItem, ActionActivitySource actionActivitySource)
        {
            if (GetAllStates(EditArea).Count == 0)
                return;
            DataAndStates item = null;
            if (ListDataAndStates.Any(x => x.DataItem == dataItem))
                item = ListDataAndStates.First(x => x.DataItem == dataItem);
            else
            {
                item = new EntityArea.DataAndStates(dataItem);
                //  item.EntityStates.CollectionChanged += (sender1, e1) => EntityStates_CollectionChanged(sender1, e1, item);
                ListDataAndStates.Add(item);
            }

            ResetActionActivities(dataItem);
            item.EntityStates1.Clear();
            //List<EntityStateDTO> trueStates = new List<EntityStateDTO>();
            var appliableStates = GetAppliableStates(dataItem);
            foreach (var state in appliableStates)
            {
                if (CheckEntityState(dataItem, state))
                    item.EntityStates1.Add(state);
            }
            //item.EntityStateGroups.Clear();
            //foreach (var group in appliableStates.Item2)
            //{
            //    if (CheckEntityState(dataItem, group))
            //        item.EntityStateGroups.Add(group);
            //}
            if (item.EntityStates1.Any())
                DoStateActionActivity(item, actionActivitySource);
        }
        //private void EditArea_DataItemLoaded(object sender, EditAreaDataItemLoadedArg e)
        //{
        private List<EntityStateDTO> GetAppliableStates(DP_FormDataRepository dataItem)
        {
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            foreach (var state in EditArea.EntityStates1.Where(x => x.ActionActivities.Any()))
            {
                //if (skipUICheck)
                //    result.Add(state);
                //else
                //{
                if (state.ActionActivities.Any(x => x.UIEnablityDetails.Any(y => EditArea.AreaInitializer.SourceRelationColumnControl != null && y.RelationshipID == EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID)))
                {
                    //bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || (EditArea is I_EditEntityAreaOneData && EditArea.DataItemIsInTempViewMode(dataItem));
                    //چرا اینجا تمپ ویو هم باشه وضعیت حساب میشه؟
                    bool dataIsInValidMode = dataItem.DataIsInEditMode() || dataItem.DataItemIsInTempViewMode();
                    if (dataIsInValidMode)
                        result.Add(state);
                }
                else
                {
                    if (dataItem.DataIsInEditMode())
                        result.Add(state);
                }
                //}
            }
            //List<EntityStateGroupDTO> resultGroup = new List<EntityStateGroupDTO>();
            //foreach (var group in EditArea.EntityStateGroups.Where(x => x.ActionActivities.Any()))
            //{

            //    if (skipUICheck)
            //        resultGroup.Add(group);
            //    else
            //    {
            //        if (group.EntityStates.Any(z => z.ActionActivities.Any(x => x.UIEnablityDetails.Any(y => EditArea.AreaInitializer.SourceRelationColumnControl != null && y.RelationshipID == EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID))))
            //        {
            //            //bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || (EditArea is I_EditEntityAreaOneData && EditArea.DataItemIsInTempViewMode(dataItem));
            //            bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || EditArea.DataItemIsInTempViewMode(dataItem);
            //            if (dataIsInValidMode)
            //                resultGroup.Add(group);
            //        }
            //        else
            //        {
            //            if (EditArea.DataItemIsInEditMode(dataItem))
            //                resultGroup.Add(group);
            //        }
            //    }

            //}
            return result;
        }


        //    //   item.OnShow = true;
        //    //    if (item.EntityStates.Any())
        //    //  EntityStates_CollectionChanged(null, null, item);
        //    //     item.OnShow = false;
        //}

        private void CheckDataItemChangeMonitors(DP_FormDataRepository dataItem, EntityStateDTO entityState)
        {
            var generalKey = "stateWatch" + AgentHelper.GetUniqueDataPostfix(dataItem);
            var usageKey = entityState.ID.ToString();

            if (dataItem.ChangeMonitorExists(generalKey, usageKey))
                return;

            List<Tuple<string, int>> columns = new List<Tuple<string, int>>();
            List<Tuple<string, int>> rels = new List<Tuple<string, int>>();
            foreach (var condition in entityState.StateConditions)
            {
                if (condition.Formula != null)
                {
                    foreach (var fItem in condition.Formula.FormulaItems)
                    {
                        if (fItem.ItemType == FormuaItemType.Column)
                            columns.Add(new Tuple<string, int>(fItem.RelationshipIDTail, fItem.ItemID));
                        else if (!string.IsNullOrEmpty(fItem.RelationshipIDTail))
                            rels.Add(new Tuple<string, int>(fItem.RelationshipIDTail, 0));
                    }
                }
                else if (condition.ColumnID != 0)
                {
                    columns.Add(new Tuple<string, int>(condition.RelationshipTail?.RelationshipIDPath, condition.ColumnID));
                }
            }
            if (columns.Any() || rels.Any())
            {
                dataItem.RelatedDataTailOrColumnChanged += DataItem_RelatedDataTailOrColumnChanged;
            }
            foreach (var item in columns)
                dataItem.AddChangeMonitor(generalKey, usageKey, item.Item1, item.Item2);
            foreach (var item in rels)
                dataItem.AddChangeMonitor(generalKey, usageKey, item.Item1, 0);

        }




        //private void EditArea_DataItemUnShown(object sender, EditAreaDataItemArg e)
        //{
        //    e.DataItem.RemoveChangeMonitorByGenaralKey("stateWatch" + AgentHelper.GetUniqueDataPostfix(e.DataItem));
        //    foreach (var item in ListDataAndStates.Where(x => x.DataItem == e.DataItem).ToList())
        //        ListDataAndStates.Remove(item);
        //}


        //private void SetDataChangeEventsForDataItem(DP_FormDataRepository dataItem)
        //{

        //}

        private void DataItem_RelatedDataTailOrColumnChanged(object sender, ChangeMonitor e)
        {
            if (e.GeneralKey.StartsWith("stateWatch"))
            {
                foreach (var entityState in GetAppliableStates(e.DataToCall).Where(x => x.ID.ToString() == e.UsageKey))
                {
                    if (e.UsageKey == entityState.ID.ToString())
                    {
                        bool changed = false;
                        var listItem = ListDataAndStates.FirstOrDefault(x => x.DataItem == e.DataToCall);
                        if (listItem != null)
                        {
                            bool entityStateValue = CheckEntityState(e.DataToCall, entityState);
                            if (entityStateValue)
                            {
                                if (!listItem.EntityStates1.Any(x => x.ID == entityState.ID))
                                {
                                    listItem.EntityStates1.Add(entityState);
                                    changed = true;
                                }
                            }
                            else
                            {
                                if (listItem.EntityStates1.Any(x => x.ID == entityState.ID))
                                {
                                    listItem.EntityStates1.Remove(listItem.EntityStates1.First(x => x.ID == entityState.ID));
                                    changed = true;
                                }
                            }
                        }
                        if (changed)
                        {
                            ResetActionActivities(listItem.DataItem);
                            DoStateActionActivity(listItem, ActionActivitySource.TailOrPropertyChange);
                        }

                    }
                }
            }
        }
        //private void ImposetStates(DataAndStates dataAndState, bool reset)
        //{
        //    if (reset)
        //        ResetActionActivities(dataAndState.DataItem);
        //    DoStateActionActivity(dataAndState, dataAndState.EntityStates);
        //}
        //private void EntityStates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e, DataAndStates dataAndState)
        //{
        //    //var allActionActivities = dataAndState.EntityStates.SelectMany(x => x.ActionActivities);
        //    ResetActionActivities(dataAndState.DataItem);
        //}

        //private List<EntityStateDTO> GetEntityStates(DP_FormDataRepository dataItem)
        //{
        //    //  ResetActionActivities(dataItem);
        //    List<EntityStateDTO> result = new List<EntityStateDTO>();

        //    return result;
        //}

        private bool CheckEntityState(DP_FormDataRepository dataItem, EntityStateDTO state)
        {
            bool stateIsValid = false;
            var stateResult = AgentUICoreMediator.GetAgentUICoreMediator.StateManager.CalculateState(state, dataItem, AgentUICoreMediator.GetAgentUICoreMediator.GetRequester());
            if (string.IsNullOrEmpty(stateResult.Message))
            {
                stateIsValid = stateResult.Result;
            }
            return stateIsValid;

        }

        public void ResetActionActivities(DP_FormDataRepository dataItem)
        {
            List<BaseColumnControl> hiddenControls = new List<BaseColumnControl>();

            foreach (var state in GetAppliableStates(dataItem))
            {
                foreach (var actionActivity in state.ActionActivities)
                {
                    foreach (var detail in actionActivity.UIEnablityDetails)
                    {
                        if (detail.ColumnID != 0)
                        {
                            var simpleColumn = GetChildSimpleContorlProperty(dataItem, detail.ColumnID);
                            if (simpleColumn != null)
                            {
                                if (detail.Hidden == true)
                                {
                                    simpleColumn.ResetSimpleColumnVisiblityFromState(detail.ID.ToString());//, ImposeControlState.Both);
                                    hiddenControls.Add(simpleColumn.SimpleColumnControl);
                                }
                                else if (detail.Readonly == true)
                                    simpleColumn.ResetSimpleColumnReadonlyFromState(detail.ID.ToString());//, ImposeControlState.Both);
                            }
                        }
                        else if (detail.RelationshipID != 0)
                        {
                            if (detail.Hidden == true)
                            {
                                if (dataItem.ChildRelationshipInfos.Any(x => x.Relationship.ID == detail.RelationshipID))
                                {
                                    var childRelationshipInfo = dataItem.ChildRelationshipInfos.First(x => x.Relationship.ID == detail.RelationshipID);
                                    childRelationshipInfo.ResetRelatoinsipColumnVisiblityFromState(detail.ID.ToString());
                                    hiddenControls.Add(childRelationshipInfo.RelationshipControl);

                                    //حتما داده باید در یو آی حاضر باشد
                                }
                                else if (dataItem.ParantChildRelationshipInfo != null && dataItem.ParantChildRelationshipInfo.RelationshipID == detail.RelationshipID)
                                {
                                    //   var childRelationshipInfo = dataItem.ParantChildRelationshipInfo;
                                    dataItem.ResetDataItemParentRelationshipVisiblity(detail.ID.ToString());
                                }

                            }
                            else if (detail.Readonly == true)
                            {
                                if (dataItem.ChildRelationshipInfos.Any(x => x.Relationship.ID == detail.RelationshipID))
                                {
                                    var childRelationshipInfo = dataItem.ChildRelationshipInfos.First(x => x.Relationship.ID == detail.RelationshipID);
                                    childRelationshipInfo.ResetColumnReadonlyFromState(detail.ID.ToString());
                                    //حتما داده باید در یو آی حاضر باشد
                                }
                                else if (dataItem.ParantChildRelationshipInfo != null && dataItem.ParantChildRelationshipInfo.RelationshipID == detail.RelationshipID)
                                {
                                    //  var childRelationshipInfo = dataItem.ParantChildRelationshipInfo;
                                    dataItem.ResetDataItemParentRelationshipReadonly(detail.ID.ToString());
                                }
                            }

                        }
                        //else if (detail.UICompositionID != 0)
                        //{
                        //    if (detail.Hidden == true)
                        //    {
                        //        var uiComposition = GetUIComposition(EditArea, detail.UICompositionID);
                        //        if (uiComposition != null)
                        //            uiComposition.Container.Visiblity(true);
                        //    }
                        //}
                    }
                    if (actionActivity.UIColumnValueRange.Any())
                    {
                        foreach (var columnValueRange in actionActivity.UIColumnValueRange.GroupBy(x => x.ColumnValueRangeID))
                        {
                            if (EditArea.SimpleColumnControls.Any(x => x.Column.ID == columnValueRange.Key))
                            {
                                var simpleColumn = GetChildSimpleContorlProperty(dataItem, columnValueRange.Key);
                                //if (dataItem.ColumnKeyValueRanges.Any(x => x.Key == simpleColumn.Column.ID))
                                //dataItem.ColumnKeyValueRanges.Remove(simpleColumn.Column.ID);
                                simpleColumn.ResetColumnValueRangeFromState();
                            }
                        }
                    }
                    if (actionActivity.Type == Enum_ActionActivityType.EntityReadonly)
                    {
                        //موقتی گذاشته شد
                        //بهتره از ClearDataItemReadonlyFromState استفاده بشه
                        dataItem.ResetDataItemReadonlyFromState(actionActivity.ID.ToString());
                    }
                }
            }
            if (EditArea is I_EditEntityAreaOneData && hiddenControls.Any())
            {
                (EditArea as I_EditEntityAreaOneData).CheckContainersVisiblity(hiddenControls);
            }
            //////item.EntityStates.Clear();
        }




        //private void ApplyState(I_EditEntityArea editArea, DP_FormDataRepository dataItem, EntityStateDTO state)
        //{

        //}





        public void DoStateActionActivity(DataAndStates dataAndState, ActionActivitySource actionActivitySource)
        {
            var dataItem = dataAndState.DataItem;
            List<BaseColumnControl> hiddenControls = new List<BaseColumnControl>();
            foreach (var state in dataAndState.EntityStates1.ToList())
            {
                foreach (var item in state.ActionActivities)
                {
                    if (EditArea.RunningActionActivities.Any(x => x.ID == item.ID))
                        continue;
                    EditArea.RunningActionActivities.Add(item);
                    if (item.Type == Enum_ActionActivityType.UIEnablity)
                    {
                        if (item.UIEnablityDetails.Any())
                        {
                            foreach (var detail in item.UIEnablityDetails)
                            {
                                if (detail.RelationshipID != 0)
                                {
                                    if (detail.Hidden == true)
                                    {
                                        if (dataItem.ChildRelationshipInfos.Any(x => x.Relationship.ID == detail.RelationshipID))
                                        {
                                            var childRelationshipInfo = dataItem.ChildRelationshipInfos.First(x => x.Relationship.ID == detail.RelationshipID);

                                            childRelationshipInfo.SetRelatoinsipColumnHiddenFromState("غیر فعال سازی رابطه" + " " + "بر اساس وضعیت" + " " + state.Title, detail.ID.ToString(), actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);
                                            if (actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0)
                                                hiddenControls.Add(childRelationshipInfo.RelationshipControl);
                                            //حتما داده باید در یو آی حاضر باشد
                                        }
                                        else if (dataItem.ParantChildRelationshipInfo != null && dataItem.ParantChildRelationshipInfo.RelationshipID == detail.RelationshipID)
                                        {
                                            dataItem.SetDataItemParentRelationshipVisiblity("غیر فعال سازی رابطه" + " " + "بر اساس وضعیت" + " " + state.Title, detail.ID.ToString(), actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);
                                        }
                                    }
                                    else if (detail.Readonly == true)
                                    {
                                        if (dataItem.ChildRelationshipInfos.Any(x => x.Relationship.ID == detail.RelationshipID))
                                        {
                                            var childRelationshipInfo = dataItem.ChildRelationshipInfos.First(x => x.Relationship.ID == detail.RelationshipID);
                                            childRelationshipInfo.SetRelatoinsipColumnReadonlyFromState("فقط خواندنی سازی رابطه" + " " + "بر اساس وضعیت" + " " + state.Title, detail.ID.ToString(), actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);
                                            //حتما داده باید در یو آی حاضر باشد
                                        }
                                        else if (dataItem.ParantChildRelationshipInfo != null && dataItem.ParantChildRelationshipInfo.RelationshipID == detail.RelationshipID)
                                        {
                                            if (actionActivitySource == ActionActivitySource.OnShowData)
                                            {
                                                if (detail.ID != 0)
                                                    dataItem.ParantChildRelationshipInfo.IsReadonlyOnShow = true;
                                            }
                                            dataItem.SetDataItemParentRelationshipReadonly("فقط خواندنی سازی رابطه" + " " + "بر اساس وضعیت" + " " + state.Title, detail.ID.ToString(), actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);
                                        }
                                    }
                                }
                                else if (detail.ColumnID != 0)
                                {
                                    var simpleColumn = GetChildSimpleContorlProperty(dataItem, detail.ColumnID);
                                    if (simpleColumn != null)
                                    {
                                        if (detail.Hidden == true)
                                        {
                                            simpleColumn.SetSimpleColumnHiddenFromState("غیر فعال سازی ستون" + " " + "بر اساس وضعیت" + " " + state.Title, detail.ID.ToString(), actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);//, ImposeControlState.Impose);// GetColumnHiddenControlState(state, dataItem, detail.ColumnID, actionActivitySource));
                                            hiddenControls.Add(simpleColumn.SimpleColumnControl);
                                        }
                                        else if (detail.Readonly == true)
                                            simpleColumn.SetSimpleColumnReadonlyFromState("فقط خواندنی سازی ستون" + " " + "بر اساس وضعیت" + " " + state.Title, detail.ID.ToString(), actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);//, ImposeControlState.Impose);// GetColumnReadonlyControlState(state, dataItem, detail.ColumnID, actionActivitySource));
                                    }
                                }
                            }
                        }
                    }
                    else if (item.Type == Enum_ActionActivityType.ColumnValueRange)
                    {
                        if (item.UIColumnValueRange.Any())
                        {
                            foreach (var columnValueRange in item.UIColumnValueRange.GroupBy(x => x.ColumnValueRangeID))
                            {
                                if (EditArea.SimpleColumnControls.Any(x => x.Column.ID == columnValueRange.Key))
                                {
                                    var simpleColumn = GetChildSimpleContorlProperty(dataItem, columnValueRange.Key);
                                    List<ColumnValueRangeDetailsDTO> candidates = GetFilteredRange(simpleColumn.SimpleColumnControl, columnValueRange);
                                    //if (simpleColumn.ColumnKeyValueRanges.Any(x => x.Key == simpleColumn.Column.ID))
                                    //    dataItem.ColumnKeyValueRanges[simpleColumn.Column.ID] = candidates;
                                    //else
                                    //    dataItem.ColumnKeyValueRanges.Add(simpleColumn.Column.ID, candidates);
                                    simpleColumn.SetColumnValueRangeFromState(candidates);
                                }
                            }
                        }
                    }
                    else if (item.Type == Enum_ActionActivityType.ColumnValue)
                    {
                        if (item.UIColumnValue.Any())
                        {
                            //در واقع مقادیر پیش فرض را ست میکند
                            EditArea.SetColumnValueFromState(dataItem, item.UIColumnValue, state, null, false);
                        }
                    }
                    else if (item.Type == Enum_ActionActivityType.EntityReadonly)
                    {
                        dataItem.SetDataItemReadonlyFromState("داده به علت وضعیت" + " " + state.Title + " " + "فقط خواندنی شد", item.ID.ToString(), actionActivitySource == ActionActivitySource.OnShowData);
                    }
                    EditArea.RunningActionActivities.Remove(item);
                }
            }
            if (EditArea is I_EditEntityAreaOneData && hiddenControls.Any())
            {
                (EditArea as I_EditEntityAreaOneData).CheckContainersVisiblity(hiddenControls);
            }
        }





        //private bool AllChildItemsAreDBRel(DP_FormDataRepository dataItem, EntityRelationshipTailDTO relationshipTail)
        //{
        //    if (relationshipTail == null || !dataItem.ChildRelationshipInfos.Any(x => x.Relationship.ID == relationshipTail.Relationship.ID))
        //        return true;
        //    else
        //    {
        //        var childRelInfo = dataItem.ChildRelationshipInfos.First(x => x.Relationship.ID == relationshipTail.Relationship.ID);
        //        var childData = childRelInfo.RelatedData.First();
        //        if (childData.IsDBRelationship)
        //            return AllChildItemsAreDBRel(childData, relationshipTail.ChildTail);
        //        else
        //            return false;
        //    }
        //}
        //private ImposeControlState GetRelationshipReadonlyControlState(ChildRelationshipInfo childRelationshipInfo, EntityStateDTO state, int relationshipID, ActionActivitySource actionActivitySource)
        //{
        //    if (state.StateConditions.Any(x => x.RelationshipTailID != 0))
        //    {
        //        if (state.StateConditions.Any(x => x.RelationshipTail.RelationshipIDPath == relationshipID.ToString() || x.RelationshipTail.RelationshipIDPath.StartsWith(relationshipID.ToString() + ",")))
        //        {
        //            if (actionActivitySource == ActionActivitySource.TailOrPropertyChange)
        //                return ImposeControlState.AddMessageColor;
        //            else if (actionActivitySource == ActionActivitySource.BeforeUpdate)
        //            {
        //                if (childRelationshipInfo.CheckRelationshipIsChanged())
        //                {
        //                    return ImposeControlState.AddMessageColor;
        //                }
        //            }
        //            else if (actionActivitySource == ActionActivitySource.OnShowData)
        //            {
        //                return ImposeControlState.AddMessageColor;
        //            }
        //        }
        //    }
        //    return ImposeControlState.Impose;
        //}
        //private ImposeControlState GetRelationshipHiddenControlState(ChildRelationshipInfo childRelationshipInfo, EntityStateDTO state, int relationshipID, ActionActivitySource actionActivitySource)
        //{
        //    if (state.StateConditions.Any(x => x.RelationshipTailID != 0))
        //    {
        //        if (state.StateConditions.Any(x => x.RelationshipTail.RelationshipIDPath == relationshipID.ToString() || x.RelationshipTail.RelationshipIDPath.StartsWith(relationshipID.ToString() + ",")))
        //        {
        //            if (actionActivitySource == ActionActivitySource.TailOrPropertyChange)
        //                return ImposeControlState.AddMessageColor;
        //            else if (actionActivitySource == ActionActivitySource.BeforeUpdate)
        //            {
        //                if (childRelationshipInfo.CheckRelationshipIsChanged())
        //                {
        //                    return ImposeControlState.AddMessageColor;
        //                }
        //            }
        //            else if (actionActivitySource == ActionActivitySource.OnShowData)
        //            {
        //                return ImposeControlState.AddMessageColor;
        //            }
        //        }
        //    }
        //    return ImposeControlState.Impose;
        //}
        //private ImposeControlState GetColumnReadonlyControlState(EntityStateDTO state, DP_FormDataRepository dataItem, int columnID, ActionActivitySource actionActivitySource)
        //{
        //    if (state.RelationshipTailID == 0 && state.ColumnID != 0)
        //    {
        //        if (state.ColumnID == columnID)
        //        {
        //            if (actionActivitySource == ActionActivitySource.TailOrPropertyChange)
        //            {
        //                //چون همونیه که تغییر کرده
        //                return ImposeControlState.AddMessageColor;
        //            }
        //            else if (actionActivitySource == ActionActivitySource.BeforeUpdate)
        //            {
        //                if (dataItem.PropertyValueIsChanged(dataItem.GetProperty(columnID)))
        //                    return ImposeControlState.AddMessageColor;
        //            }
        //        }
        //    }

        //    return ImposeControlState.Impose;
        //}

        //private ImposeControlState GetColumnHiddenControlState(EntityStateDTO state, DP_FormDataRepository dataItem, int columnID, ActionActivitySource actionActivitySource)
        //{
        //    if (state.RelationshipTailID == 0 && state.ColumnID != 0)
        //    {
        //        if (state.ColumnID == columnID)
        //        {
        //            if (actionActivitySource == ActionActivitySource.TailOrPropertyChange)
        //            {
        //                //چون همونیه که تغییر کرده
        //                return ImposeControlState.AddMessageColor;
        //            }
        //            else if (actionActivitySource == ActionActivitySource.BeforeUpdate)
        //            {
        //                if (dataItem.PropertyValueIsChanged(dataItem.GetProperty(columnID)))
        //                    return ImposeControlState.AddMessageColor;
        //            }
        //        }
        //    }
        //    return ImposeControlState.Impose;
        //}
        //private bool ShouldImposeRelationshipControlInUI(EntityStateDTO state, int relationshipID)
        //{
        //    if (state.FormulaID != 0)
        //        return false;
        //    else if (state.RelationshipTailID != 0)
        //    {
        //        if (state.RelationshipTail.RelationshipIDPath == relationshipID.ToString() || state.RelationshipTail.RelationshipIDPath.StartsWith(relationshipID.ToString() + ","))
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
        private List<ColumnValueRangeDetailsDTO> GetFilteredRange(SimpleColumnControlGenerel simpleColumn, IGrouping<int, UIColumnValueRangeDTO> columnValueRange)
        {
            var result = new List<ColumnValueRangeDetailsDTO>();
            foreach (var columnValueItem in columnValueRange)
            {
                foreach (var detail in simpleColumn.Column.ColumnValueRange.Details)
                {
                    var value = "";
                    if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Value)
                        value = detail.Value;
                    else if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Title)
                        value = detail.KeyTitle;
                    else if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Tag1)
                        value = detail.Tag1;
                    else if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Tag2)
                        value = detail.Tag2;
                    if (columnValueItem.Value == value)
                    {
                        result.Add(detail);
                    }
                }
            }
            return result;
        }

        private static EditEntityAreaByRelationshipTail GetEditEntityAreaByRelationshipTail(I_EditEntityArea editEntityArea, EntityRelationshipTailDTO entityRelationshipTail)
        {
            //اگر مالتی پل بود و ...
            RelationshipColumnControl relatedEntityArea = null;
            if (editEntityArea is I_EditEntityAreaOneData)
                relatedEntityArea = (editEntityArea as I_EditEntityAreaOneData).RelationshipColumnControls.FirstOrDefault(x => x.Relationship.ID == entityRelationshipTail.Relationship.ID);
            else if (editEntityArea is I_EditEntityAreaMultipleData)
                relatedEntityArea = (editEntityArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.FirstOrDefault(x => x.Relationship.ID == entityRelationshipTail.Relationship.ID);

            if (relatedEntityArea != null)
            {
                if (entityRelationshipTail.ChildTail == null)
                {
                    EditEntityAreaByRelationshipTail result = new MyUILibrary.EditEntityAreaByRelationshipTail();
                    result.EditEntityAreaFound = true;
                    result.FoundEditEntityArea = relatedEntityArea.EditNdTypeArea;
                    return result;
                }
                else
                {
                    return GetEditEntityAreaByRelationshipTail(relatedEntityArea.EditNdTypeArea, entityRelationshipTail.ChildTail);
                }
            }
            else
            {
                EditEntityAreaByRelationshipTail result = new MyUILibrary.EditEntityAreaByRelationshipTail();
                result.EditEntityAreaFound = false;
                result.LastFoundEntityArea = new Tuple<I_EditEntityArea, EntityRelationshipTailDTO>(editEntityArea, entityRelationshipTail);
                return result;
            }

        }
        //private UIControlPackageTree GetUIComposition(I_EditEntityArea editEntityArea, int uiCompositionID)
        //{
        //    if (editEntityArea is I_EditEntityAreaOneData)
        //        return (editEntityArea as I_EditEntityAreaOneData).UICompositionContainers.FirstOrDefault(x => x.UIComposition.ID == uiCompositionID);
        //    return null;
        //}
        //private RelationshipColumnControl GetChildRelationshipInfo(DP_FormDataRepository dataItem, int relationshipID)
        //{
        //    RelationshipColumnControl control = null;

        //    if (editEntityArea is I_EditEntityAreaOneData)
        //    {
        //        if ((editEntityArea as I_EditEntityAreaOneData).RelationshipColumnControls.Any(x => x.Relationship.ID == relationshipID))
        //        {
        //            control = (editEntityArea as I_EditEntityAreaOneData).RelationshipColumnControls.First(x => x.Relationship.ID == relationshipID);

        //        }
        //        else if (editEntityArea.AreaInitializer.SourceRelationColumnControl != null && editEntityArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID == relationshipID)
        //        {
        //            control = editEntityArea.AreaInitializer.SourceRelationColumnControl;
        //        }
        //    }
        //    else if (editEntityArea is I_EditEntityAreaMultipleData)
        //    {
        //        if ((editEntityArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.Any(x => x.Relationship.ID == relationshipID))
        //        {
        //            control = (editEntityArea as I_EditEntityAreaMultipleData).RelationshipColumnControls.First(x => x.Relationship.ID == relationshipID);
        //        }
        //        else if (editEntityArea.AreaInitializer.SourceRelationColumnControl != null && editEntityArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID == relationshipID)
        //        {
        //            control = editEntityArea.AreaInitializer.SourceRelationColumnControl;
        //        }
        //    }
        //    return control;
        //}



        private ChildSimpleContorlProperty GetChildSimpleContorlProperty(DP_FormDataRepository dataItem, int columnID)
        {
            return dataItem.ChildSimpleContorlProperties.FirstOrDefault(x => x.SimpleColumnControl.Column.ID == columnID);
        }


    }

    public class DataAndStates
    {
        //////   public event EventHandler<ObserverDataChangedArg> RelatedDataChanged;

        public DataAndStates(DP_FormDataRepository dataItem)
        {
            DataItem = dataItem;
            EntityStates1 = new ObservableCollection<EntityStateDTO>();
            //EntityStateGroups = new ObservableCollection<EntityStateGroupDTO>();
        }


        //  public bool IsPresent { set; get; }
        //   public I_EditEntityArea SourceEditArea { set; get; }
        public DP_FormDataRepository DataItem { set; get; }
        // public UIActionActivityDTO ActionActivity { set; get; }
        public ObservableCollection<EntityStateDTO> EntityStates1 { set; get; }
        //public ObservableCollection<EntityStateGroupDTO> EntityStateGroups { set; get; }
        public bool OnShow { get; internal set; }
        //public bool InAction { set; get; }
        //public void RegisterEvent()
        //{
        //    ///////      DataItem.RelatedDataChanged += DataItem_RelatedDataChanged;
        //}
        //public void UnRegisterEvent()
        //{
        //    //////     DataItem.RelatedDataChanged -= DataItem_RelatedDataChanged;
        //}
        //////private void DataItem_RelatedDataChanged(object sender, ObserverDataChangedArg e)
        //////{
        //////    if (RelatedDataChanged != null)
        //////        RelatedDataChanged(this, e);
        //////}
    }
}
