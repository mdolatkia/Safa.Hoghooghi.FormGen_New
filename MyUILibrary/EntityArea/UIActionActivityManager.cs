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
            SetEntityStates();
        }
        void SetEntityStates()
        {
            if (EditArea.EntityStates1.Any(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
            {
                foreach (var item in EditArea.EntityStates1.Where(x => x.ActionActivities.Any(y => y.Type == Enum_ActionActivityType.EntityReadonly)))
                {
                    var newActionActivity = GetReadonlyActionActivity();
                    newActionActivity.Type = Enum_ActionActivityType.UIEnablity;
                    item.ActionActivities.Add(newActionActivity);
                }
            }


            //if (fItems.Any())
            //    firstLoadItems.Add(new Tuple<EntityStateDTO, List<UIEnablityDetailsDTO>>(entityState, fItems));

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

        }

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

        //private void EditArea_DataItemShown(object sender, EditAreaDataItemLoadedArg e)
        //{


        //}
        public void DataToShowInDataview(DP_FormDataRepository dataItem)
        {
            if ((EditArea.EntityStates1 == null || EditArea.EntityStates1.Count == 0))
                return;
            foreach (var state in GetChangeMoniStates(EditArea))
            {
                CheckDataItemChangeMonitors(dataItem, state);
            }
            var actionActivitySource = ActionActivitySource.OnShowData;
         
            CheckAndImposeEntityStates(dataItem, actionActivitySource);
        }
        private UIActionActivityDTO GetReadonlyActionActivity()
        {
            var newActionActivity = new UIActionActivityDTO();
            foreach (var column in EditArea.DataEntryEntity.Columns)
            {
                if (!column.PrimaryKey)
                {
                    //اگر با فرمول غیر فعال شود چی خود ستونی که باعث ریدونلی شدن می شود
                    if (!EditArea.DataEntryEntity.Relationships.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.SecondSideColumnID == column.ID)))
                    {
                        //bool skip = false;
                        //foreach (var item in states)
                        //{
                        //    if (item.RelationshipTailID == 0 && item.ColumnID != 0)
                        //    {
                        //        if (item.ColumnID == column.ID)
                        //            skip = true;
                        //    }
                        //}
                        //if (!skip)
                        //{
                        var detail = new UIEnablityDetailsDTO();
                        detail.ColumnID = column.ID;
                        detail.Readonly = true;
                        detail.ID = column.ID;
                        newActionActivity.UIEnablityDetails.Add(detail);
                        //}
                    }
                }
            }
            foreach (var relationship in EditArea.DataEntryEntity.Relationships.Where(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary))
            {
                //////if (item.RelationshipTail != null)
                //////{
                //همون قضیه که خود رابطه ک=ه با وضعیت مشترکه نمیتونه اعمال بشه
                //if (item.RelationshipTail.Relationship.ID == relationship.ID)
                //    continue;
                //////}
                var detail = new UIEnablityDetailsDTO();
                detail.RelationshipID = relationship.Relationship.ID;
                detail.Readonly = true;
                detail.ID = relationship.Relationship.ID;
                newActionActivity.UIEnablityDetails.Add(detail);
            }
            return newActionActivity;
        }
        private List<EntityStateDTO> GetChangeMoniStates(I_EditEntityArea editArea)
        {
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            foreach (var state in EditArea.EntityStates1)
            {
                if (StateHasDynamicAction(state))
                    result.Add(state);
            }
            return result;
        }


        public void CheckAndImposeEntityStates(DP_FormDataRepository dataItem, ActionActivitySource actionActivitySource)
        {
            if (EditArea.EntityStates1 == null || EditArea.EntityStates1.Count == 0)
                return;
            //   DataAndStates item = null;
            //  if (ListDataAndStates.Any(x => x.DataItem == dataItem))
            //      item = ListDataAndStates.First(x => x.DataItem == dataItem);
            //  else
            //  {
            //     item = new EntityArea.DataAndStates(dataItem);
            //      //  item.EntityStates.CollectionChanged += (sender1, e1) => EntityStates_CollectionChanged(sender1, e1, item);
            //      ListDataAndStates.Add(item);
            //   }
            foreach (var state in EditArea.EntityStates1)
            {
                ResetActionActivities(state, dataItem);
            }
            //   item.EntityStates1.Clear();
            //List<EntityStateDTO> trueStates = new List<EntityStateDTO>();
            //   var appliableStates = GetAppliableStates(dataItem);
            foreach (var state in EditArea.EntityStates1)
            {
                if (StateHasDynamicAction(state))
                    if (CheckEntityState(dataItem, state))
                        DoStateActionActivity(state, dataItem, actionActivitySource);
            }
            //item.EntityStateGroups.Clear();
            //foreach (var group in appliableStates.Item2)
            //{
            //    if (CheckEntityState(dataItem, group))
            //        item.EntityStateGroups.Add(group);
            //}
            //       if (item.EntityStates1.Any())

        }

        private bool StateHasDynamicAction(EntityStateDTO state)
        {
            return (state.ActionActivities.Any(x => x.Type == Enum_ActionActivityType.ColumnValueRange || x.Type == Enum_ActionActivityType.ColumnValue
                || (x.Type == Enum_ActionActivityType.UIEnablity && x.UIEnablityDetails.Any(y => IsOnLoadOnlyAction(y) == ActionType.DynamicChildVisiblity))));
        }
        private bool StateHasOnLoadAction(EntityStateDTO state)
        {
            return (state.ActionActivities.Any(x => x.Type == Enum_ActionActivityType.UIEnablity && x.UIEnablityDetails.Any(y => IsOnLoadOnlyAction(y) == ActionType.OnLoadChildReadonly
            || IsOnLoadOnlyAction(y) == ActionType.OnLoadParentRelationship)));
        }


        //private void EditArea_DataItemLoaded(object sender, EditAreaDataItemLoadedArg e)
        //{
        //private List<EntityStateDTO> GetAppliableStates(DP_FormDataRepository dataItem)
        //{
        //    List<EntityStateDTO> result = new List<EntityStateDTO>();
        //    foreach (var state in EditArea.EntityStates1.Where(x => x.ActionActivities.Any()))
        //    {
        //        //if (skipUICheck)
        //        //    result.Add(state);
        //        //else
        //        //{
        //        if (state.ActionActivities.Any(x => x.UIEnablityDetails.Any(y => EditArea.AreaInitializer.SourceRelationColumnControl != null && y.RelationshipID == EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID)))
        //        {
        //            //bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || (EditArea is I_EditEntityAreaOneData && EditArea.DataItemIsInTempViewMode(dataItem));
        //            //چرا اینجا تمپ ویو هم باشه وضعیت حساب میشه؟
        //            bool dataIsInValidMode = dataItem.DataIsInEditMode() || dataItem.DataItemIsInTempViewMode();
        //            if (dataIsInValidMode)
        //                result.Add(state);
        //        }
        //        else
        //        {
        //            if (dataItem.DataIsInEditMode())
        //                result.Add(state);
        //        }
        //        //}
        //    }
        //    //List<EntityStateGroupDTO> resultGroup = new List<EntityStateGroupDTO>();
        //    //foreach (var group in EditArea.EntityStateGroups.Where(x => x.ActionActivities.Any()))
        //    //{

        //    //    if (skipUICheck)
        //    //        resultGroup.Add(group);
        //    //    else
        //    //    {
        //    //        if (group.EntityStates.Any(z => z.ActionActivities.Any(x => x.UIEnablityDetails.Any(y => EditArea.AreaInitializer.SourceRelationColumnControl != null && y.RelationshipID == EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID))))
        //    //        {
        //    //            //bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || (EditArea is I_EditEntityAreaOneData && EditArea.DataItemIsInTempViewMode(dataItem));
        //    //            bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || EditArea.DataItemIsInTempViewMode(dataItem);
        //    //            if (dataIsInValidMode)
        //    //                resultGroup.Add(group);
        //    //        }
        //    //        else
        //    //        {
        //    //            if (EditArea.DataItemIsInEditMode(dataItem))
        //    //                resultGroup.Add(group);
        //    //        }
        //    //    }

        //    //}
        //    return result;
        //}


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
                dataItem.AddChangeMonitorIfNotExists(generalKey, usageKey, item.Item1, item.Item2);
            foreach (var item in rels)
                dataItem.AddChangeMonitorIfNotExists(generalKey, usageKey, item.Item1, 0);

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
                foreach (var entityState in EditArea.EntityStates1.Where(x => x.ID.ToString() == e.UsageKey))
                {
                    if (e.UsageKey == entityState.ID.ToString())
                    {
                        CheckAndImposeEntityStates(e.DataToCall, ActionActivitySource.TailOrPropertyChange);

                        //bool changed = false;
                        //var listItem = ListDataAndStates.FirstOrDefault(x => x.DataItem == e.DataToCall);
                        //if (listItem != null)
                        //{
                        //    bool entityStateValue = CheckEntityState(e.DataToCall, entityState);
                        //    if (entityStateValue)
                        //    {
                        //        if (!listItem.EntityStates1.Any(x => x.ID == entityState.ID))
                        //        {
                        //            listItem.EntityStates1.Add(entityState);
                        //            changed = true;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (listItem.EntityStates1.Any(x => x.ID == entityState.ID))
                        //        {
                        //            listItem.EntityStates1.Remove(listItem.EntityStates1.First(x => x.ID == entityState.ID));
                        //            changed = true;
                        //        }
                        //    }
                        //}
                        //if (changed)
                        //{
                        //    CheckAndImposeEntityStates(listItem.DataItem, ActionActivitySource.TailOrPropertyChange);
                        //    //ResetActionActivities(listItem.DataItem);
                        //    //DoStateActionActivity(listItem, ActionActivitySource.TailOrPropertyChange);
                        //}

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

        public void SetExistingDataFirstLoadStates(DP_FormDataRepository dataItem)
        {
            //DataAddedResult result = new DataAddedResult();

            if (dataItem.IsNewItem)
                return;

            foreach (var state in EditArea.EntityStates1.Where(x => StateHasOnLoadAction(x)))
            {
                if (CheckEntityState(dataItem, state))
                {
                    foreach (var actionActivity in state.ActionActivities)
                    {
                        if (actionActivity.Type == Enum_ActionActivityType.UIEnablity)
                        {
                            foreach (var detail in actionActivity.UIEnablityDetails)
                            {
                                var type = IsOnLoadOnlyAction(detail);
                                if (type == ActionType.OnLoadParentRelationship || type == ActionType.OnLoadChildReadonly)
                                {
                                    if (type == ActionType.OnLoadParentRelationship)
                                    {
                                        if (detail.Hidden == true)
                                        {
                                            dataItem.AddParentRelationshipHiddenState(detail.ID.ToString(), state.Title, true);//, actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);
                                                                                                                               //    result.parentRelationshipIsHidden = true;
                                        }
                                        else if (detail.Readonly == true)
                                        {
                                            dataItem.AddParentRelationshipReadonlyState(detail.ID.ToString(), state.Title, true);//, actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);
                                                                                                                                 //  result.parentRelationshipIsReadonly = true;
                                        }
                                    }
                                    else if (type == ActionType.OnLoadChildReadonly)
                                    {
                                        if (detail.RelationshipID != 0)
                                        {
                                            if (dataItem.ChildRelationshipDatas.Any(x => x.Relationship.ID == detail.RelationshipID))
                                            {
                                                var childRelationshipInfo = dataItem.ChildRelationshipDatas.First(x => x.Relationship.ID == detail.RelationshipID);
                                                childRelationshipInfo.AddReadonlyState(detail.ID.ToString(), state.Title, true);//, actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);
                                            }
                                            else
                                                dataItem.AddTempRelationshipPropertyReadonly(detail.RelationshipID,detail.ID.ToString(), state.Title, true);
                                        }
                                        else if (detail.ColumnID != 0)
                                        {
                                            var simpleColumn = GetChildSimpleContorlProperty(dataItem, detail.ColumnID);
                                            if (simpleColumn != null)
                                            {
                                                simpleColumn.AddReadonlyState(detail.ID.ToString(), state.Title, true);//, actionActivitySource == ActionActivitySource.OnShowData && detail.ID != 0);//, ImposeControlState.Impose);// GetColumnReadonlyControlState(state, dataItem, detail.ColumnID, actionActivitySource));
                                            }
                                            else
                                                dataItem.AddTempSimplePropertyReadonly(detail.ColumnID,detail.ID.ToString(), state.Title, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //   return result;
            //if (EditArea is I_EditEntityAreaOneData && hiddenControls.Any())
            //{
            //    (EditArea as I_EditEntityAreaOneData).CheckContainersVisiblity(hiddenControls);
            //}
        }
        public void DataShown(DP_FormDataRepository dataItem)
        {

        }
        private ActionType IsOnLoadOnlyAction(UIEnablityDetailsDTO uiEnablity)
        {
            if (uiEnablity.RelationshipID != 0 && EditArea.AreaInitializer.SourceRelationColumnControl != null &&
             uiEnablity.RelationshipID == EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID)
                return ActionType.OnLoadParentRelationship;
            else if (uiEnablity.Hidden == true)
                return ActionType.DynamicChildVisiblity;
            else if (uiEnablity.Readonly == true)
            {
                return ActionType.OnLoadChildReadonly;
            }
            else
                return ActionType.Unknown;
            //   fItems.Add(uiEnablity);

            //  uiEnablity.Permanent = true;

        }


        private BaseColumnControl ChildItemVisiblity(EntityStateDTO state, UIEnablityDetailsDTO detail, DP_FormDataRepository dataItem, bool hidden)
        {
            if (dataItem.DataIsInEditMode())
            {
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
               //         if (dataItem.DataIsInEditMode())

                            if (hidden)
                                simpleColumn.AddHiddenState(detail.ID.ToString(), "غیر فعال سازی ستون" + " " + "بر اساس وضعیت" + " " + state.Title, false);
                            else
                                simpleColumn.RemoveHiddenState(detail.ID.ToString());
                        return (simpleColumn.SimpleColumnControl);
                    }
                }
            }

            //دوتا کار باید انجام شه

            //    پراپرتی ستونها هم مثل روابط یکی شود پیام و مخفی شدن

            //    دوم اینکه اگر رابطه مخفی شد دیگه دیتا و بایندینگش انجام نشه.
            return null;
        }

        //private bool ParentRelationshipAction(EntityStateDTO state, UIEnablityDetailsDTO detail, DP_FormDataRepository dataItem)
        //{
        //    if (detail.RelationshipID != 0)
        //    {


        //    }
        //    return false;
        //}
        //private BaseColumnControl ChildItemReadonly(EntityStateDTO state, UIEnablityDetailsDTO detail, DP_FormDataRepository dataItem)
        //{

        //    return null;
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
        public void DoStateActionActivity(EntityStateDTO state, DP_FormDataRepository dataItem, ActionActivitySource actionActivitySource)
        {
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
                            var type = IsOnLoadOnlyAction(detail);
                            if (type == ActionType.DynamicChildVisiblity)
                            {
                                var isHidden = ChildItemVisiblity(state, detail, dataItem, true);
                                if (isHidden != null)
                                {
                                    hiddenControls.Add(isHidden);
                                }
                            }

                        }
                    }
                }
                else if (actionActivity.Type == Enum_ActionActivityType.ColumnValueRange)
                {
                    if (ActionActivityIsApliable(dataItem, actionActivity))
                    {
                        if (actionActivity.UIColumnValueRange.Any())
                        {
                            foreach (var columnValueRange in actionActivity.UIColumnValueRange.GroupBy(x => x.ColumnValueRangeID))
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
                }
                else if (actionActivity.Type == Enum_ActionActivityType.ColumnValue)
                {
                    if (ActionActivityIsApliable(dataItem, actionActivity))
                    {
                        if (actionActivity.UIColumnValue.Any())
                        {
                            //در واقع مقادیر پیش فرض را ست میکند
                            dataItem.SetColumnValue(actionActivity.UIColumnValue, state, null, false);
                        }
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

        public void ResetActionActivities(EntityStateDTO state, DP_FormDataRepository dataItem)
        {
            List<BaseColumnControl> hiddenControls = new List<BaseColumnControl>();
            //foreach (var state in EditArea.EntityStates1)
            //{
            foreach (var actionActivity in state.ActionActivities)
            {
                if (actionActivity.Type == Enum_ActionActivityType.UIEnablity)
                {
                    foreach (var detail in actionActivity.UIEnablityDetails)
                    {

                        var type = IsOnLoadOnlyAction(detail);
                        if (type == ActionType.DynamicChildVisiblity)
                        {
                            var isHidden = ChildItemVisiblity(state, detail, dataItem, false);
                            if (isHidden != null)
                            {
                                hiddenControls.Add(isHidden);
                            }
                        }
                        //if (UIEnablityIsApliable(dataItem, detail))
                        //{
                        //////if (detail.ColumnID != 0)
                        //////{
                        //////    var simpleColumn = GetChildSimpleContorlProperty(dataItem, detail.ColumnID);
                        //////    if (simpleColumn != null)
                        //////    {
                        //////        if (detail.Hidden == true)
                        //////        {
                        //////            simpleColumn.RemoveHiddenState(detail.ID.ToString(), true);//, ImposeControlState.Both);
                        //////            hiddenControls.Add(simpleColumn.SimpleColumnControl);
                        //////        }
                        //////        else if (detail.Readonly == true)
                        //////            simpleColumn.RemoveReadonlyState(detail.ID.ToString(), true);//, ImposeControlState.Both);
                        //////    }
                        //////}
                        //////else if (detail.RelationshipID != 0)
                        //////{
                        //////    if (detail.Hidden == true)
                        //////    {
                        //////        if (dataItem.ChildRelationshipDatas.Any(x => x.Relationship.ID == detail.RelationshipID))
                        //////        {
                        //////            var childRelationshipInfo = dataItem.ChildRelationshipDatas.First(x => x.Relationship.ID == detail.RelationshipID);
                        //////            childRelationshipInfo.ResetRelatoinsipColumnVisiblityFromState(detail.ID.ToString());
                        //////            hiddenControls.Add(childRelationshipInfo.RelationshipControl);

                        //////            //حتما داده باید در یو آی حاضر باشد
                        //////        }
                        //////        else if (dataItem.ToParantChildRelationshipData != null && dataItem.ToParantChildRelationshipData.RelationshipID == detail.RelationshipID)
                        //////        {
                        //////            //   var childRelationshipInfo = dataItem.ParantChildRelationshipInfo;
                        //////            dataItem.ResetDataItemParentRelationshipVisiblity(detail.ID.ToString());
                        //////        }

                        //////    }
                        //////    else if (detail.Readonly == true)
                        //////    {
                        //////        if (dataItem.ChildRelationshipDatas.Any(x => x.Relationship.ID == detail.RelationshipID))
                        //////        {
                        //////            var childRelationshipInfo = dataItem.ChildRelationshipDatas.First(x => x.Relationship.ID == detail.RelationshipID);
                        //////            childRelationshipInfo.ResetColumnReadonlyFromState(detail.ID.ToString());
                        //////            //حتما داده باید در یو آی حاضر باشد
                        //////        }
                        //////        else if (dataItem.ToParantChildRelationshipData != null && dataItem.ToParantChildRelationshipData.RelationshipID == detail.RelationshipID)
                        //////        {
                        //////            //  var childRelationshipInfo = dataItem.ParantChildRelationshipInfo;
                        //////            dataItem.ResetDataItemParentRelationshipReadonly(detail.ID.ToString());
                        //////        }
                        //////    }

                        //////}
                        //}
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
                }
                else if (actionActivity.Type == Enum_ActionActivityType.ColumnValueRange)
                {
                    if (ActionActivityIsApliable(dataItem, actionActivity))
                    {
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
                    }
                }
                ////if (actionActivity.Type == Enum_ActionActivityType.EntityReadonly)
                ////{
                ////    //موقتی گذاشته شد
                ////    //بهتره از ClearDataItemReadonlyFromState استفاده بشه
                ////    dataItem.ResetDataItemReadonlyFromState(actionActivity.ID.ToString());
                ////}
                //}
            }
            if (EditArea is I_EditEntityAreaOneData && hiddenControls.Any())
            {
                (EditArea as I_EditEntityAreaOneData).CheckContainersVisiblity(hiddenControls);
            }
            //////item.EntityStates.Clear();
        }

        private bool ActionActivityIsApliable(DP_FormDataRepository dataItem, UIActionActivityDTO actionActivity)
        {
            if (dataItem.DataIsInEditMode())
                return true;
            else
                return false;
        }

        //private bool UIEnablityIsApliable(DP_FormDataRepository dataItem, UIEnablityDetailsDTO detail)
        //{
        //    if (dataItem.DataIsInEditMode() || (detail.EvenInTempView && dataItem.DataItemIsInTempViewMode()))
        //        return true;
        //    else
        //        return false;
        //}




        //private void ApplyState(I_EditEntityArea editArea, DP_FormDataRepository dataItem, EntityStateDTO state)
        //{

        //}










        //private bool AllChildItemsAreDBRel(DP_FormDataRepository dataItem, EntityRelationshipTailDTO relationshipTail)
        //{
        //    if (relationshipTail == null || !dataItem.ChildRelationshipDatas.Any(x => x.Relationship.ID == relationshipTail.Relationship.ID))
        //        return true;
        //    else
        //    {
        //        var childRelInfo = dataItem.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipTail.Relationship.ID);
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
            RelationshipColumnControlGeneral relatedEntityArea = null;
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
                    result.FoundEditEntityArea = relatedEntityArea.GenericEditNdTypeArea;
                    return result;
                }
                else
                {
                    return GetEditEntityAreaByRelationshipTail(relatedEntityArea.GenericEditNdTypeArea, entityRelationshipTail.ChildTail);
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
