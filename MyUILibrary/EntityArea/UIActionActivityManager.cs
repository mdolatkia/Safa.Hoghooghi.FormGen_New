using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelEntites;
using ProxyLibrary;
using MyUILibrary.EntityArea.Commands;
using CommonDefinitions.UISettings;
using System.Collections.ObjectModel;
namespace MyUILibrary.EntityArea
{
    public class UIActionActivityManager : I_UIActionActivityManager
    {
        //   public List<DataAndStates> ListDataAndStates = new List<DataAndStates>();
        I_EditEntityArea EditArea { set; get; }
        // List<Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>> firstLoadItems = new List<Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>>();
        public UIActionActivityManager(I_EditEntityArea editArea)
        {
            // UIActionActivityManager: ee8265997ef8
            EditArea = editArea;
            //EditArea.DataItemLoaded += EditArea_DataItemLoaded;
            //   EditArea.DataItemShown += EditArea_DataItemShown;
            EditArea.UIGenerated += EditArea_UIGenerated;
            //     EditArea.DataItemUnShown += EditArea_DataItemUnShown;

        }

        private void EditArea_UIGenerated(object sender, EventArgs e)
        {
            CheckISADeterminerStates();
            CheckUnionDeterminerStates();
            CheckDirectDataSecurityStates();
            CheckColumnValueRangeStates();
            //        SetEntityStates();
        }



        //void SetEntityStates()
        //{
        //    if (EditArea.EntityStates1.Any(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
        //    {
        //        foreach (var item in EditArea.EntityStates1.Where(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
        //        {
        //            var newActionActivity = GetReadonlyActionActivity();
        //            newActionActivity.Type = Enum_ActionActivityType.UIEnablity;
        //            item.ActionActivities.Add(newActionActivity);
        //        }
        //    }


        //    //if (fItems.Any())
        //    //    firstLoadItems.Add(new Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>(entityState, fItems));

        //}

        private void CheckDirectDataSecurityStates()
        {

            //اینجا این بدرد نمیخوره دیگه
            if (AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.EntityHasDirectSecurities(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID))
            {
                var dataDirectSecurity = AgentUICoreMediator.GetAgentUICoreMediator.DataSecurityManager.GetEntitySecurityDirects(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EditArea.AreaInitializer.EntityID);
                foreach (var item in dataDirectSecurity)
                {
                    //item.EntityState.ConditionOperator = AgentHelper.GetNotOperator(item.EntityState.ConditionOperator);
                    item.EntityState.ActionActivities.Add(new UIActionActivityDTO() { Type = Enum_ActionActivityType.EntityReadonly });

                    EditArea.EntityStates1.Add(item.EntityState);
                }
            }

        }

        private void CheckUnionDeterminerStates()
        {
            //UIActionActivityManager.CheckUnionDeterminerStates: 0199f3874e7c
            if (EditArea.RelationshipColumnControls.Any(x => x.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion))
            {
                foreach (var item in EditArea.RelationshipColumnControls.Where(x => x.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion))
                {
                    var superToSubRel = item.Relationship as UnionToSubUnionRelationshipDTO;
                    if (superToSubRel.DeterminerColumnID != 0 && !string.IsNullOrEmpty(superToSubRel.DeterminerColumnValue))
                    {
                        var state = new EntityStateDTO();
                        state.ID = -1 * superToSubRel.ID;
                        state.ColumnID = superToSubRel.DeterminerColumnID;
                        state.Values.Add(new EntityStateValueDTO() { Value = superToSubRel.DeterminerColumnValue });
                        state.EntityStateOperator = InORNotIn.NotIn;
                        var actionActivity = new UIActionActivityDTO();
                        actionActivity.ID = -1 * superToSubRel.ID;
                        actionActivity.Type = Enum_ActionActivityType.UIEnablity;
                        actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = superToSubRel.ID });
                        state.ActionActivities.Add(actionActivity);
                        EditArea.EntityStates1.Add(state);
                    }
                }
            }
            if (EditArea.SourceRelationColumnControl != null && EditArea.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
            {

                var subToSuperRel = EditArea.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO;
                if (subToSuperRel.DeterminerColumnID != 0 && !string.IsNullOrEmpty(subToSuperRel.DeterminerColumnValue))
                {
                    var state = new EntityStateDTO();
                    state.ID = -1 * subToSuperRel.ID;
                    state.ColumnID = subToSuperRel.DeterminerColumnID;
                    state.Values.Add(new EntityStateValueDTO() { Value = subToSuperRel.DeterminerColumnValue });
                    state.EntityStateOperator = InORNotIn.NotIn;
                    var actionActivity = new UIActionActivityDTO();
                    actionActivity.ID = -1 * subToSuperRel.ID;
                    actionActivity.Type = Enum_ActionActivityType.UIEnablity;
                    actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = subToSuperRel.PairRelationshipID });
                    state.ActionActivities.Add(actionActivity);
                    EditArea.EntityStates1.Add(state);


                    //شرط اگر داده جدید باشد
                    var setDeterminerState = new EntityStateDTO();
                    setDeterminerState.ID = -2 * subToSuperRel.ID;

                    setDeterminerState.ColumnID = subToSuperRel.DeterminerColumnID;
                    setDeterminerState.Values.Add(new EntityStateValueDTO() { Value = "!@#$#" });
                    setDeterminerState.EntityStateOperator = InORNotIn.NotIn;
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
            //UIActionActivityManager.CheckISADeterminerStates: 56344e195b36
            if (EditArea.RelationshipColumnControls.Any(x => x.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub))
            {
                foreach (var item in EditArea.RelationshipColumnControls.Where(x => x.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub))
                {
                    var superToSubRel = item.Relationship as SuperToSubRelationshipDTO;
                    if (superToSubRel.SuperEntityDeterminerColumn != null && superToSubRel.DeterminerColumnValues.Any())
                    {
                        var state = new EntityStateDTO();
                        state.ID = -1 * superToSubRel.ID;

                        state.ColumnID = superToSubRel.SuperEntityDeterminerColumn.ID;
                        foreach (var val in superToSubRel.DeterminerColumnValues)
                            state.Values.Add(new EntityStateValueDTO() { Value = val.Value });
                        state.EntityStateOperator = InORNotIn.NotIn;
                        var actionActivity = new UIActionActivityDTO();
                        actionActivity.ID = -1 * superToSubRel.ID;
                        actionActivity.Type = Enum_ActionActivityType.UIEnablity;
                        actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = superToSubRel.ID });
                        state.ActionActivities.Add(actionActivity);
                        EditArea.EntityStates1.Add(state);
                    }
                }
            }
            if (EditArea.SourceRelationColumnControl != null && EditArea.SourceRelationColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
            {

                var subToSuperRel = EditArea.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO;
                if (subToSuperRel.SuperEntityDeterminerColumn != null && subToSuperRel.DeterminerColumnValues.Any())
                {

                    var state = new EntityStateDTO();
                    state.ID = -1 * subToSuperRel.ID;
                    state.ColumnID = subToSuperRel.SuperEntityDeterminerColumn.ID;
                    foreach (var val in subToSuperRel.DeterminerColumnValues)
                        state.Values.Add(new EntityStateValueDTO() { Value = val.Value });
                    state.EntityStateOperator = InORNotIn.NotIn;
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
                    setDeterminerState.ColumnID = subToSuperRel.SuperEntityDeterminerColumn.ID;
                    setDeterminerState.Values.Add(new EntityStateValueDTO() { Value = "!@#$#" });
                    setDeterminerState.EntityStateOperator = InORNotIn.NotIn;
                    var setDeterminerActionActivity = new UIActionActivityDTO();
                    setDeterminerActionActivity.ID = -2 * subToSuperRel.ID;
                    setDeterminerActionActivity.Type = Enum_ActionActivityType.ColumnValue;
                    setDeterminerActionActivity.UIColumnValue.Add(new UIColumnValueDTO() { ColumnID = subToSuperRel.SuperEntityDeterminerColumn.ID, ExactValue = subToSuperRel.DeterminerColumnValues.First().Value });//, EvenHasValue = true });
                    setDeterminerState.ActionActivities.Add(setDeterminerActionActivity);
                    EditArea.EntityStates1.Add(setDeterminerState);

                }

            }

        }
        private void CheckColumnValueRangeStates()
        {
            //** UIActionActivityManager.CheckColumnValueRangeStates: 403d341eba76
            if (EditArea.SimpleColumnControls.Any(x => x.DataEntryColumn.ColumnValueRange != null && x.DataEntryColumn.ColumnValueRange.TagColumnID != 0))
            {
                foreach (var item in EditArea.SimpleColumnControls.Where(x => x.DataEntryColumn.ColumnValueRange != null && x.DataEntryColumn.ColumnValueRange.TagColumnID != 0))
                {

                    var state = new EntityStateDTO();
                    state.ID = -1 * item.DataEntryColumn.ID;

                    state.ColumnID = item.DataEntryColumn.ColumnValueRange.TagColumnID;
                    state.RelationshipTailID = item.DataEntryColumn.ColumnValueRange.EntityRelationshipTailID;
                    state.RelationshipTail = item.DataEntryColumn.ColumnValueRange.EntityRelationshipTail;
                    state.Values.Add(new EntityStateValueDTO() { Value = "###" });
                    state.EntityStateOperator = InORNotIn.NotIn;
                    var actionActivity = new UIActionActivityDTO();
                    actionActivity.ID = -1 * item.DataEntryColumn.ID;
                    actionActivity.Type = Enum_ActionActivityType.ColumnValueRange;
                    actionActivity.UIColumnValueRange = new UIColumnValueRangeDTO();
                    actionActivity.UIColumnValueRange.ColumnID = item.DataEntryColumn.ID;
                    actionActivity.UIColumnValueRange.FilterValueRelationshipTailID = item.DataEntryColumn.ColumnValueRange.EntityRelationshipTailID;
                    actionActivity.UIColumnValueRange.FilterValueRelationshipTail = item.DataEntryColumn.ColumnValueRange.EntityRelationshipTail;
                    actionActivity.UIColumnValueRange.FilterValueColumnID = item.DataEntryColumn.ColumnValueRange.TagColumnID;
                    //actionActivity.UIEnablityDetails.Add(new UIEnablityDetailsDTO() { Hidden = true, RelationshipID = superToSubRel.ID });
                    state.ActionActivities.Add(actionActivity);
                    EditArea.EntityStates1.Add(state);

                }
            }
        }
        //private void EditArea_DataItemShown(object sender, EditAreaDataItemLoadedArg e)
        //{


        //}
        private List<EntityStateDTO> GetUIEntityStates()
        {
            return EditArea.EntityStates1;

        }
        private List<EntityStateDTO> GetEditModeUIEntityStates()
        {
            return EditArea.EntityStates1.Where(x => x.UIApplyState == Enum_UIApplyState.OnlyEditMode).ToList();

        }
        private List<EntityStateDTO> GetEditViewEntityStates()
        {
            return EditArea.EntityStates1.Where(x => x.UIApplyState == Enum_UIApplyState.EditOrViewMode).ToList();

        }
        public void DataToShowInDataview(DP_FormDataRepository dataItem)
        {
            // UIActionActivityManager.DataToShowInDataview: f3c425b1720a

            if (GetEditModeUIEntityStates().Any() && dataItem.DataIsInEditMode())
            {
                foreach (var state in GetEditModeUIEntityStates())
                {
                    SetDataItemChangeMonitors(dataItem, state);
                }
            }
            else if (GetEditViewEntityStates().Any() && !dataItem.IsDBRelationship && (dataItem.DataIsInEditMode() || dataItem.DataIsInViewMode()))
            {
                foreach (var state in GetEditViewEntityStates())
                {
                    SetDataItemChangeMonitors(dataItem, state);
                }
            }

            CheckAndImposeEntityStates(dataItem);
        }
        //private UIActionActivityDTO GetReadonlyActionActivity()
        //{
        //    var newActionActivity = new UIActionActivityDTO();
        //    foreach (var column in EditArea.DataEntryEntity.Columns)
        //    {
        //        if (!column.PrimaryKey)
        //        {
        //            //اگر با فرمول غیر فعال شود چی خود ستونی که باعث ریدونلی شدن می شود
        //            if (!EditArea.DataEntryEntity.Relationships.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.SecondSideColumnID == column.ID)))
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
        //    foreach (var relationship in EditArea.DataEntryEntity.Relationships.Where(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary))
        //    {
        //        //////if (item.RelationshipTail != null)
        //        //////{
        //        //همون قضیه که خود رابطه ک=ه با وضعیت مشترکه نمیتونه اعمال بشه
        //        //if (item.RelationshipTail.Relationship.ID == relationship.ID)
        //        //    continue;
        //        //////}
        //        var detail = new UIEnablityDetailsDTO();
        //        detail.RelationshipID = relationship.Relationship.ID;
        //        detail.Readonly = true;
        //        detail.ID = relationship.Relationship.ID;
        //        newActionActivity.UIEnablityDetails.Add(detail);
        //    }
        //    return newActionActivity;
        //}


        public void CheckAndImposeEntityStates(DP_FormDataRepository dataItem)
        {
            //UIActionActivityManager.CheckAndImposeEntityStates: 5a9f918cdb55

            if (GetEditModeUIEntityStates().Any() && dataItem.DataIsInEditMode())
            {
                ResetActionActivities(dataItem, true);
                foreach (var state in GetEditModeUIEntityStates())
                {
                    if (CheckEntityState(dataItem, state))
                        DoStateActionActivity(state, dataItem, true);
                }
            }
            if (GetEditViewEntityStates().Any() && !dataItem.IsDBRelationship && (dataItem.DataIsInEditMode() || dataItem.DataIsInViewMode()))
            {
                ResetActionActivities(dataItem, false);
                foreach (var state in GetEditViewEntityStates())
                {
                    if (CheckEntityState(dataItem, state))
                        DoStateActionActivity(state, dataItem, false);
                }
            }

        }
        public void DoStateActionActivity(EntityStateDTO state, DP_FormDataRepository dataItem, bool inEditMode)
        {
            // UIActionActivityManager.DoStateActionActivity: 9d894434cc60
            //    var dataItem = dataAndState.DataItem;
            List<BaseColumnControl> hiddenControls = new List<BaseColumnControl>();

            foreach (var actionActivity in state.ActionActivities)
            {
                if (EditArea.RunningActionActivities.Any(x => x.ID == actionActivity.ID))
                    continue;
                EditArea.RunningActionActivities.Add(actionActivity);
                if (actionActivity.Type == Enum_ActionActivityType.UIEnablity)
                {
                    if (actionActivity.UIEnablityDetails.Any())
                    {
                        foreach (var detail in actionActivity.UIEnablityDetails)
                        {
                            //var type = IsOnLoadOnlyAction(detail);
                            //if (type == ActionType.DynamicChildVisiblity)
                            //{
                            if (detail.Hidden == true)
                            {
                                if (detail.RelationshipID != 0 && dataItem.ParantChildRelationshipData != null && detail.RelationshipID == dataItem.ParantChildRelationshipData.ToParentRelationshipID)
                                {
                                    dataItem.ParentRelationshipIsHidenInUI = true;
                                    dataItem.ParentRelationshipIsHidenInUIText += (string.IsNullOrEmpty(dataItem.ParentRelationshipIsHidenInUIText) ? "" : ",") + state.Title;
                                }
                                else if (inEditMode)
                                {
                                    var isHidden = ChildItemVisiblity(state, detail, dataItem, true);
                                    if (isHidden != null)
                                    {
                                        hiddenControls.Add(isHidden);
                                    }
                                }
                            }
                            //}

                        }
                    }
                }
                else if (inEditMode && actionActivity.Type == Enum_ActionActivityType.ColumnValueRange)
                {
                    //**29ec5b10-3bf3-49b4-8dfe-a4dc7f6869ad
                    //if (ActionActivityIsApliable(dataItem, actionActivity))
                    //{
                    //if (actionActivity.UIColumnValueRange.Any())
                    //{
                    //foreach (var columnValueRange in actionActivity.UIColumnValueRange.GroupBy(x => x.ColumnValueRangeID))
                    //{
                    if (EditArea.SimpleColumnControls.Any(x => actionActivity.UIColumnValueRange != null && x.DataEntryColumn.ID == actionActivity.UIColumnValueRange.ColumnID))
                    {
                        var simpleColumn = GetChildSimpleContorlProperty(dataItem, actionActivity.UIColumnValueRange.ColumnID);

                        if (simpleColumn != null)
                        {
                            List<ColumnValueRangeDetailsDTO> candidates = GetFilteredRange(simpleColumn, actionActivity.UIColumnValueRange);
                            //if (simpleColumn.ColumnKeyValueRanges.Any(x => x.Key == simpleColumn.Column.ID))
                            //    dataItem.ColumnKeyValueRanges[simpleColumn.Column.ID] = candidates;
                            //else
                            //    dataItem.ColumnKeyValueRanges.Add(simpleColumn.Column.ID, candidates);
                            simpleColumn.SetColumnValueRangeFromState(candidates);
                        }
                    }
                    //}
                    //}
                    //}
                }
                else if (inEditMode && actionActivity.Type == Enum_ActionActivityType.ColumnValue)
                {
                    //if (ActionActivityIsApliable(dataItem, actionActivity))
                    //{
                    if (actionActivity.UIColumnValue.Any())
                    {
                        //در واقع مقادیر پیش فرض را ست میکند
                        dataItem.SetColumnValue(actionActivity.UIColumnValue, state, null, false);
                        //}
                    }
                }
                ////else if (item.Type == Enum_ActionActivityType.EntityReadonly)
                ////{
                ////    dataItem.SetDataItemReadonlyFromState("داده به علت وضعیت" + " " + state.Title + " " + "فقط خواندنی شد", item.ID.ToString());//, actionActivitySource == ActionActivitySource.OnShowData);
                ////}
                EditArea.RunningActionActivities.Remove(actionActivity);
            }

            if (EditArea is I_EditEntityAreaOneData && hiddenControls.Any())
            {
                (EditArea as I_EditEntityAreaOneData).CheckContainersVisiblity(hiddenControls);
            }
        }


        public void ResetActionActivities(DP_FormDataRepository dataItem, bool inEditMode)
        {
            // UIActionActivityManager.ResetActionActivities: 42b13c5915e5
            List<BaseColumnControl> hiddenControls = new List<BaseColumnControl>();
            foreach (var state in GetUIEntityStates())
            {
                foreach (var actionActivity in state.ActionActivities)
                {
                    if (actionActivity.Type == Enum_ActionActivityType.UIEnablity)
                    {
                        foreach (var detail in actionActivity.UIEnablityDetails)
                        {
                            if (detail.Hidden == true)
                            {
                                if (detail.RelationshipID != 0 && dataItem.ParantChildRelationshipData != null && detail.RelationshipID == dataItem.ParantChildRelationshipData.ToParentRelationshipID)
                                {
                                    if (!dataItem.IsDBRelationship)
                                    {
                                        dataItem.ParentRelationshipIsHidenInUI = false;
                                        dataItem.ParentRelationshipIsHidenInUIText = "";
                                    }
                                }
                                else if (inEditMode)
                                {
                                    var isHidden = ChildItemVisiblity(state, detail, dataItem, false);
                                    if (isHidden != null)
                                    {
                                        hiddenControls.Add(isHidden);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            if (EditArea is I_EditEntityAreaOneData && hiddenControls.Any())
            {
                (EditArea as I_EditEntityAreaOneData).CheckContainersVisiblity(hiddenControls);
            }
        }

        //private void DoStateEditViewActionActivity(EntityStateDTO state, DP_FormDataRepository dataItem)
        //{
        //    foreach (var actionActivity in state.ActionActivities)
        //    {
        //        if (actionActivity.Type == Enum_ActionActivityType.UIEnablity)
        //        {
        //            foreach (var detail in actionActivity.UIEnablityDetails)
        //            {
        //                if (detail.Hidden == true)
        //                {

        //                }
        //            }
        //        }
        //    }
        //}

        //private void ResetEditViewModeActionActivities(DP_FormDataRepository dataItem)
        //{
        //    foreach (var state in GetEditViewEntityStates())
        //    {
        //        foreach (var actionActivity in state.ActionActivities)
        //        {
        //            if (actionActivity.Type == Enum_ActionActivityType.UIEnablity)
        //            {
        //                foreach (var detail in actionActivity.UIEnablityDetails)
        //                {
        //                    if (detail.Hidden == true)
        //                    {

        //                    }

        //                }
        //            }

        //        }
        //    }

        //}

        private bool SetDataItemChangeMonitors(DP_FormDataRepository dataItem, EntityStateDTO entityState)
        {
            // UIActionActivityManager.SetDataItemChangeMonitors: 96259224044f
            var generalKey = "stateWatch" + AgentHelper.GetUniqueDataPostfix(dataItem);
            var usageKey = "stateWatch" + entityState.ID.ToString();

            if (dataItem.ChangeMonitorExists(usageKey))
                return false;

            List<Tuple<string, int>> relTailsAndColumns = new List<Tuple<string, int>>();

            if (entityState.Formula != null)
            {
                foreach (var fItem in entityState.Formula.FormulaItems)
                {
                    relTailsAndColumns.Add(new Tuple<string, int>(fItem.RelationshipIDTail, fItem.ItemType == FormuaItemType.Column ? fItem.ItemID : 0));
                }
            }
            else if (entityState.ColumnID != 0)
            {
                relTailsAndColumns.Add(new Tuple<string, int>(entityState.RelationshipTail?.RelationshipIDPath, entityState.ColumnID));
            }

            List<Tuple<string, List<int>>> relTailAndColumns = new List<Tuple<string, List<int>>>();
            foreach (var item in relTailsAndColumns.GroupBy(x => x.Item1))
            {
                if (item.Any(x => x.Item2 != 0))
                {
                    List<int> columns = new List<int>();
                    foreach (var tuple in item.Where(x => x.Item2 != 0))
                    {
                        if (!columns.Any(x => x == tuple.Item2))
                            columns.Add(tuple.Item2);
                    }
                }
                else
                    relTailAndColumns.Add(new Tuple<string, List<int>>(item.Key, null));
            }
            foreach (var item in relTailAndColumns)
                dataItem.AddChangeMonitorIfNotExists(new ChangeMonitorItem(this, usageKey, item, dataItem));

            return relTailsAndColumns.Any();
        }





        public void DataPropertyRelationshipChanged(DP_FormDataRepository data, string usageKey)
        {
            // UIActionActivityManager.DataPropertyRelationshipChanged: b60b5cf2effa
            if (GetUIEntityStates().Any(x => x.ID.ToString() == usageKey))
            {
                CheckAndImposeEntityStates(data);
            }
        }




        private BaseColumnControl ChildItemVisiblity(EntityStateDTO state, UIEnablityDetailsDTO detail, DP_FormDataRepository dataItem, bool hidden)
        {
            // UIActionActivityManager.ChildItemVisiblity: 438a981b8c49
            //if (dataItem.DataIsInEditMode())
            //{
            if (detail.RelationshipID != 0)
            {
                if (dataItem.ChildRelationshipDatas.Any(x => x.Relationship.ID == detail.RelationshipID))
                {
                    var childRelationshipInfo = dataItem.ChildRelationshipDatas.First(x => x.Relationship.ID == detail.RelationshipID);

                    if (hidden)
                        childRelationshipInfo.AddHiddenState(detail.ID.ToString(), "غیر فعال سازی رابطه" + " " + "بر اساس وضعیت" + " " + state.Title, false);
                    else
                        childRelationshipInfo.RemoveHiddenState(detail.ID.ToString());
                    return childRelationshipInfo.RelationshipControl;
                }
            }
            else if (detail.ColumnID != 0)
            {
                var simpleColumn = GetChildSimpleContorlProperty(dataItem, detail.ColumnID);
                if (simpleColumn != null)
                {
                    if (hidden)
                        simpleColumn.AddHiddenState(detail.ID.ToString(), "غیر فعال سازی ستون" + " " + "بر اساس وضعیت" + " " + state.Title, false);
                    else
                        simpleColumn.RemoveHiddenState(detail.ID.ToString());
                    return (simpleColumn.SimpleColumnControl);
                }
            }
            //}

            //دوتا کار باید انجام شه

            //    پراپرتی ستونها هم مثل روابط یکی شود پیام و مخفی شدن

            //    دوم اینکه اگر رابطه مخفی شد دیگه دیتا و بایندینگش انجام نشه.
            return null;
        }


        private bool CheckEntityState(DP_FormDataRepository dataItem, EntityStateDTO state)
        {
            // UIActionActivityManager.CheckEntityState: 79bffbfb2ad2
            bool stateIsValid = false;
            var stateResult = AgentUICoreMediator.GetAgentUICoreMediator.StateManager.CalculateState(state, dataItem, AgentUICoreMediator.GetAgentUICoreMediator.GetRequester());
            if (string.IsNullOrEmpty(stateResult.Message))
            {
                stateIsValid = stateResult.Result;
            }
            return stateIsValid;

        }

        //private bool ActionActivityIsApliable(DP_FormDataRepository dataItem, UIActionActivityDTO actionActivity)
        //{
        //    if (dataItem.DataIsInEditMode())
        //        return true;
        //    else
        //        return false;
        //}


        private List<ColumnValueRangeDetailsDTO> GetFilteredRange(ChildSimpleContorlProperty childSimpleContorlProperty, UIColumnValueRangeDTO uIColumnValueRange)
        {
            // UIActionActivityManager.GetFilteredRange: 339cd0b07965
            var result = new List<ColumnValueRangeDetailsDTO>();
            var filtervalue = AgentUICoreMediator.GetAgentUICoreMediator.StateManager.GetValueSomeHow(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), childSimpleContorlProperty.SourceData, uIColumnValueRange.FilterValueRelationshipTail, uIColumnValueRange.FilterValueColumnID);

            foreach (var detail in childSimpleContorlProperty.SimpleColumnControl.DataEntryColumn.ColumnValueRange.Details)
            {
                var value = "";
                //if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Value)
                //    value = detail.Value;
                //else if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Title)
                //    value = detail.KeyTitle;
                //   if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Tag1)
                value = detail.Tag;
                //else if (columnValueItem.EnumTag == EnumColumnValueRangeTag.Tag2)
                //    value = detail.Tag2;
                if (filtervalue != null && filtervalue.ToString() == value)
                {
                    result.Add(detail);
                }
            }

            return result;
        }


        private ChildSimpleContorlProperty GetChildSimpleContorlProperty(DP_FormDataRepository dataItem, int columnID)
        {
            return dataItem.ChildSimpleContorlProperties.FirstOrDefault(x => x.SimpleColumnControl.DataEntryColumn.ID == columnID);
        }


    }
    public enum ActionType
    {
        OnLoadChildReadonly,
        OnLoadParentRelationship,
        DynamicChildVisiblity,
        Unknown
    }
    public class DataAddedResult
    {
        public bool parentRelationshipIsHidden { set; get; }
        public bool parentRelationshipIsReadonly { set; get; }
    }
    //private void DoUIEnablity(EntityStateDTO state, UIEnablityDetailsDTO detail, bool v)
    //{
    //    throw new NotImplementedException();
    //}

    //public class DataAndStates
    //{
    //    //////   public event EventHandler<ObserverDataChangedArg> RelatedDataChanged;

    //    public DataAndStates(DP_FormDataRepository dataItem)
    //    {
    //        DataItem = dataItem;
    //        EntityStates1 = new ObservableCollection<EntityStateDTO>();
    //        //EntityStateGroups = new ObservableCollection<EntityStateGroupDTO>();
    //    }


    //    //  public bool IsPresent { set; get; }
    //    //   public I_EditEntityArea SourceEditArea { set; get; }
    //    public DP_FormDataRepository DataItem { set; get; }
    //    // public UIActionActivityDTO ActionActivity { set; get; }
    //    public ObservableCollection<EntityStateDTO> EntityStates1 { set; get; }
    //    //public ObservableCollection<EntityStateGroupDTO> EntityStateGroups { set; get; }
    //    public bool OnShow { get; internal set; }
    //    //public bool InAction { set; get; }
    //    //public void RegisterEvent()
    //    //{
    //    //    ///////      DataItem.RelatedDataChanged += DataItem_RelatedDataChanged;
    //    //}
    //    //public void UnRegisterEvent()
    //    //{
    //    //    //////     DataItem.RelatedDataChanged -= DataItem_RelatedDataChanged;
    //    //}
    //    //////private void DataItem_RelatedDataChanged(object sender, ObserverDataChangedArg e)
    //    //////{
    //    //////    if (RelatedDataChanged != null)
    //    //////        RelatedDataChanged(this, e);
    //    //////}
    //}
}
