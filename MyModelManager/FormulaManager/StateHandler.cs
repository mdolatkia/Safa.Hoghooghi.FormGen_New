using ModelEntites;


using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class StateHandler
    {
        BizEntityState bizStateFunction = new BizEntityState();

        public StateResult GetStateResult(int StateFunctionID, DP_DataRepository mainDataItem, DR_Requester requester)
        {
            var state = bizStateFunction.GetEntityState(requester, StateFunctionID, true, false);
            return GetStateResult(state, mainDataItem, requester);
            //////    var parameters = new List<object>();
            //////    var StateFunction = bizStateFunction.GetStateFunction(StateFunctionID);
            //////    if (StateFunction.ParamType == ModelEntites.Enum_StateFunctionParamType.OneDataItem)
            //////    {
            //////        parameters.Add(new StateFunctionParamOneDataItem(dataItem));
            //////    }
            //////    else if (StateFunction.ParamType == ModelEntites.Enum_StateFunctionParamType.KeyColumns)
            //////    {
            //////        var StateFunctionEntity = bizStateFunction.GetStateFunctionEntity(StateFunctionID, dataItem.TargetEntityID);

            //////        foreach (var column in StateFunctionEntity.StateFunctionEntityColumns)
            //////        {
            //////            EntityInstanceProperty property = dataItem.Properties.FirstOrDefault(x => x.ColumnID == column.ColumnID);
            //////            if (property != null)
            //////            {
            //////                //stringParamList += (stringParamList == "" ? "" : ",") + column.FunctionColumnParamName;
            //////                //paramList.Add(column.FunctionColumnParamName);
            //////                parameters.Add(Convert.ChangeType(property.Value, column.FunctionColumnDotNetType));
            //////            }
            //////        }
            //////    }
            //////    return GetStateFunctionResult(StateFunction, parameters);
        }
        public StateResult GetStateResult(EntityStateDTO state, DP_BaseData mainDataItem, DR_Requester requester)
        {
            StateResult result = new StateResult();
            bool hasAnyOfConditions = false;
            bool hasAllOfConditions = true;
            // var condition = state.StateCondition;

            var conditionResult = GetConditionResult(state, mainDataItem, requester);
            //if (conditionResult)
            //    hasAnyOfConditions = true;
            //else
            //    hasAllOfConditions = false;

            //if (state.ConditionOperator == AndOREqualType.And)
            //    result.Result = hasAllOfConditions;
            //else if (state.ConditionOperator == AndOREqualType.Or)
            //    result.Result = hasAnyOfConditions;
            //else if (state.ConditionOperator == AndOREqualType.NotAnd)
            //    result.Result = !hasAllOfConditions;
            //else if (state.ConditionOperator == AndOREqualType.NotOr)
            //    result.Result = !hasAnyOfConditions;
            return result;
        }

        private bool GetConditionResult(EntityStateDTO condition, DP_BaseData mainDataItem, DR_Requester requester)
        {
            bool result = false;
            bool securitySubjectIsOk = false;
            if (condition.SecuritySubjects.Any())
            {
                bool hasAnyOfSubjects = false;
                foreach (var subject in condition.SecuritySubjects)
                {
                    foreach (var post in requester.Posts)
                    {
                        if (post.CurrentUserID == subject
                            || post.ID == subject
                             || post.OrganizationID == subject
                              || post.OrganizationTypeID == subject
                               || post.OrganizationTypeRoleTypeID == subject
                                || post.RoleTypeID == subject
                                )
                            hasAnyOfSubjects = true;
                    }
                }

                if (condition.SecuritySubjectInORNotIn == InORNotIn.In)
                {
                    securitySubjectIsOk = hasAnyOfSubjects == true;
                }
                else
                {
                    securitySubjectIsOk = hasAnyOfSubjects == false;
                }
            }
            else
                securitySubjectIsOk = true;

            if (securitySubjectIsOk)
            {
                if (condition.ColumnID != 0)
                {
                    DataitemRelatedColumnValueHandler dataitemRelatedColumnValueHandler = new DataitemRelatedColumnValueHandler();
                    var value = dataitemRelatedColumnValueHandler.GetValueSomeHow(requester, mainDataItem, condition.RelationshipTail, condition.ColumnID);

                    result = StateHasValue(requester, condition, value);
                }
                else if (condition.FormulaID != 0)
                {
                    FormulaFunctionHandler FormulaFunctionHandler = new FormulaFunctionHandler();
                    var value = FormulaFunctionHandler.CalculateFormula(condition.FormulaID, mainDataItem, requester);
                    result = StateHasValue(requester, condition, value.Result);
                }
                else
                {
                    if (condition.SecuritySubjects.Any())
                        result = securitySubjectIsOk;
                    else
                        result = false;
                }
            }
            return result;
        }

        private bool StateHasValue(DR_Requester requester, EntityStateDTO condition, object columnValue)
        {
            bool hasAnyOfValues = false;
            if (columnValue == null)
            {
                columnValue = "<Null>";
            }
            foreach (var stateValue in condition.Values)
            {
                if (!string.IsNullOrEmpty(stateValue.Value))
                {
                    if (stateValue.Value.ToLower().Equals(columnValue.ToString().ToLower()))
                        hasAnyOfValues = true;
                }
                else if (stateValue.SecurityReservedValue != SecurityReservedValue.None)
                {
                    foreach (var post in requester.Posts)
                    {
                        var reservedPostValue = GerReserveValueFromPost(post, stateValue.SecurityReservedValue);
                        if (reservedPostValue.ToLower().Equals(columnValue.ToString().ToLower()))
                            hasAnyOfValues = true;
                    }
                }
            }
            if (condition.EntityStateOperator == InORNotIn.In)
            {
                return hasAnyOfValues == true;
            }
            else
            {
                return hasAnyOfValues == false;
            }
        }
        private string GerReserveValueFromPost(OrganizationPostDTO post, SecurityReservedValue reservedValue)
        {
            if (reservedValue == SecurityReservedValue.OrganizationID)
                return post.OrganizationID.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationPostID)
                return post.ID.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeID)
                return post.OrganizationTypeID.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeRoleTypeID)
                return post.OrganizationTypeRoleTypeID.ToString();
            else if (reservedValue == SecurityReservedValue.RoleTypeID)
                return post.RoleTypeID.ToString();
            else if (reservedValue == SecurityReservedValue.UserID)
                return post.CurrentUserID.ToString();

            else if (reservedValue == SecurityReservedValue.OrganizationExternalKey)
                return post.OrganizationExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationPostExternalKey)
                return post.ExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeExternalKey)
                return post.OrganizationTypeExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeRoleTypeExternalKey)
                return post.OrganizationTypeRoleTypeExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.RoleTypeExternalKey)
                return post.RoleTypeExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.UserExternalKey)
                return post.CurrentUserExternalKey.ToString();
            return "";
        }

        //private StateFunctionResult GetStateFunctionResult(StateFunctionDTO StateFunction, List<object> parameters)
        //{
        //    var result = ReflectionHelper.CallMethod(StateFunction.Path, StateFunction.ClassName, StateFunction.FunctionName, parameters.ToArray());
        //    return result as StateFunctionResult;
        //}



    }
}
