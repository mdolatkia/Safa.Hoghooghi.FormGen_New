﻿using ModelEntites;
using MyCodeFunctionLibrary;
using MyConnectionManager;
using MyDatabaseFunctionLibrary;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDataEditManagerBusiness
{
    public class EditDataActionActivityManager
    {
        public void DoBeforeEditActionActivities(DR_Requester requester, List<EditQueryPreItem> items)
        {
            BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
            foreach (var editQuertyResult in items)
            {
                var actionActivities = bizActionActivity.GetActionActivities(editQuertyResult.DataItem.TargetEntityID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.BeforeSave },true, true);
                CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
                foreach (var entityActionActivity in actionActivities)
                {
                    if (entityActionActivity.CodeFunctionID != 0)
                    {
                        var resultFunction = codeFunctionHelper.GetCodeFunctionResult(requester, entityActionActivity.CodeFunctionID, editQuertyResult.DataItem);
                        if (resultFunction.Exception != null)
                        {
                            editQuertyResult.BeforeSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
                            editQuertyResult.BeforeSaveActionActivitiesMessage += resultFunction.Exception.Message;
                            return;
                        }
                    }
                }
            }
        }
        public void DoBeforeDeleteActionActivities(DR_Requester requester, List<EditQueryResultItem> items)
        {
            BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
            foreach (var editQuertyResult in items.Where(x => x.QueryItem.QueryType == Enum_QueryItemType.Delete))
            {
                var queryItem = editQuertyResult.QueryItem;
                var actionActivities = bizActionActivity.GetActionActivities(queryItem.TargetEntity.ID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.BeforeDelete }, true, true);
                CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
                foreach (var entityActionActivity in actionActivities)
                {
                    if (entityActionActivity.CodeFunctionID != 0)
                    {
                        var resultFunction = codeFunctionHelper.GetCodeFunctionResult(requester, entityActionActivity.CodeFunctionID, queryItem.DataItem);
                        if (resultFunction.Exception != null)
                        {

                            editQuertyResult.BeforeSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
                            editQuertyResult.BeforeSaveActionActivitiesMessage += resultFunction.Exception.Message;
                            return;
                        }
                    }
                }

            }
        }

        public void DoAfterEditActionActivities(DR_Requester requester, List<EditQueryResultItem> items)
        {
            BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
            foreach (var editQuertyResult in items.Where(x => x.QueryItem.QueryType == Enum_QueryItemType.Insert || x.QueryItem.QueryType == Enum_QueryItemType.Update))
            {
                var queryItem = editQuertyResult.QueryItem;
                if (queryItem.QueryType == Enum_QueryItemType.Insert || queryItem.QueryType == Enum_QueryItemType.Update)
                {
                    var actionActivities = bizActionActivity.GetActionActivities(queryItem.DataItem.TargetEntityID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.AfterSave },false, true);
                    CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
                    DatabaseFunctionHandler databaseFunctionHandler = new DatabaseFunctionHandler();
                    foreach (var entityActionActivity in actionActivities)
                    {
                        if (entityActionActivity.CodeFunctionID != 0)
                        {
                            var resultFunction = codeFunctionHelper.GetCodeFunctionResult(requester, entityActionActivity.CodeFunctionID, queryItem.DataItem);
                            if (resultFunction.Exception != null)
                            {

                                editQuertyResult.AfterSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
                                editQuertyResult.AfterSaveActionActivitiesMessage += resultFunction.Exception.Message;
                            }
                        }
                        else if (entityActionActivity.DatabaseFunctionEntityID != 0)
                        {
                            ////اصلاح شود و با خصوصیات صدا زده شود یا حداقل لیست خصوصیات ارسال شود چون بهتره ارتباط 
                            var resultFunction = databaseFunctionHandler.GetDatabaseFunctionValue(requester, entityActionActivity.DatabaseFunctionEntityID, queryItem.DataItem);
                            if (resultFunction.Exception != null)
                            {

                                editQuertyResult.AfterSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
                                editQuertyResult.AfterSaveActionActivitiesMessage += resultFunction.Exception.Message;
                            }
                        }
                    }
                }
            }
            foreach (var editQuertyResult in items.Where(x => x.QueryItem.QueryType == Enum_QueryItemType.Delete))
            {
                var queryItem = editQuertyResult.QueryItem;
                var actionActivities = bizActionActivity.GetActionActivities(queryItem.TargetEntity.ID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.AfterDelete },false, true);
                CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
                DatabaseFunctionHandler databaseFunctionHandler = new DatabaseFunctionHandler();
                foreach (var entityActionActivity in actionActivities)
                {
                    if (entityActionActivity.CodeFunctionID != 0)
                    {
                        var resultFunction = codeFunctionHelper.GetCodeFunctionResult(requester, entityActionActivity.CodeFunctionID, queryItem.DataItem);
                        if (resultFunction.Exception != null)
                        {

                            editQuertyResult.AfterSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
                            editQuertyResult.AfterSaveActionActivitiesMessage += resultFunction.Exception.Message;
                        }
                    }
                    else if (entityActionActivity.DatabaseFunctionEntityID != 0)
                    {
                        var resultFunction = databaseFunctionHandler.GetDatabaseFunctionValue(requester, entityActionActivity.DatabaseFunctionEntityID, queryItem.DataItem);
                        if (resultFunction.Exception != null)
                        {

                            editQuertyResult.AfterSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
                            editQuertyResult.AfterSaveActionActivitiesMessage += resultFunction.Exception.Message;
                        }
                    }
                }
            }
            //    var deleteResult = new DR_ResultDelete();
            //new DeleteRequestManager().DoAfterDeleteActionActivities(requester, queryItems.Where(x => x.QueryType == Enum_QueryItemType.Delete), deleteResult);
            //result.Message += deleteResult.Message;
        }
    }
}
