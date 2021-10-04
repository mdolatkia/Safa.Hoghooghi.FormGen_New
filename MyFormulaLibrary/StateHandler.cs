using ModelEntites;


using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFormulaFunctionStateFunctionLibrary
{
    public class StateHandler
    {
        BizEntityState bizStateFunction = new BizEntityState();

        public StateResult GetStateResult(int StateFunctionID, DP_DataRepository mainDataItem, DR_Requester requester)
        {
            var state = bizStateFunction.GetEntityState(requester, StateFunctionID, true);
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
        public StateResult GetStateResult(EntityStateDTO state, DP_DataRepository mainDataItem, DR_Requester requester)
        {
            StateResult result = new StateResult();

            if (state.ColumnID != 0)
            {
                DataitemRelatedColumnValueHandler dataitemRelatedColumnValueHandler = new MyFormulaFunctionStateFunctionLibrary.DataitemRelatedColumnValueHandler();
                var value = dataitemRelatedColumnValueHandler.GetValueSomeHow(requester, mainDataItem, state.RelationshipTail, state.ColumnID);
                if (value == null)
                {
                    value = "<Null>";
                }
                if (state.EntityStateOperator == Enum_EntityStateOperator.Equals)
                    result.Result = state.Values.Any(x => x.Value.Equals(value.ToString().ToLower()));
                else
                    result.Result = !state.Values.Any(x => x.Value.Equals(value.ToString().ToLower()));
            }
            else if (state.FormulaID != 0)
            {
                FormulaFunctionHandler FormulaFunctionHandler = new FormulaFunctionHandler();
                var value = FormulaFunctionHandler.CalculateFormula(state.FormulaID, mainDataItem, requester);
                if (value.Result == null)
                {
                    value.Result = "<Null>";
                }
                if (state.EntityStateOperator == Enum_EntityStateOperator.Equals)
                    result.Result = state.Values.Any(x => x.Value.ToLower().Equals(value.Result.ToString().ToLower()));
                else
                    result.Result = !state.Values.Any(x => x.Value.ToLower().Equals(value.Result.ToString().ToLower()));
            }
            if (result.Result)
            {
                if (state.SecuritySubjects.Any())
                {
                    bool hasNotAllSubject = true;
                    foreach (var subject in state.SecuritySubjects)
                    {
                        bool hasSubject = false;
                        foreach (var post in requester.Posts)
                        {
                            if (post.CurrentUserID == subject.SecuritySubjectID
                                || post.ID == subject.SecuritySubjectID
                                 || post.OrganizationID == subject.SecuritySubjectID
                                  || post.OrganizationTypeID == subject.SecuritySubjectID
                                   || post.OrganizationTypeRoleTypeID == subject.SecuritySubjectID
                                    || post.RoleTypeID == subject.SecuritySubjectID
                                    )
                                hasSubject = true;
                        }
                        if (hasSubject)
                            hasNotAllSubject = false;
                    }
                    result.Result = hasNotAllSubject;
                }
            }
            return result;

        }

        //private StateFunctionResult GetStateFunctionResult(StateFunctionDTO StateFunction, List<object> parameters)
        //{
        //    var result = ReflectionHelper.CallMethod(StateFunction.Path, StateFunction.ClassName, StateFunction.FunctionName, parameters.ToArray());
        //    return result as StateFunctionResult;
        //}



    }
}
