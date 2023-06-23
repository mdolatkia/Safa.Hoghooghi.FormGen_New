using ModelEntites;

using MyConnectionManager;
using MyLogManager;
using MyModelManager;
using ProxyLibrary;
using ProxyLibrary.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDataEditManagerBusiness
{
    public class EditDataActionActivityManager
    {
        //public void DoBeforeEditActionActivities(DR_Requester requester, List<EditQueryPreItem> items)
        //{
        //    //

        //}
        //public void DoBeforeDeleteActionActivities(DR_Requester requester, List<EditQueryPreItem> items)
        //{
        //    BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
        //    foreach (var editQuertyResult in items)
        //    {
        //        // var queryItem = editQuertyResult.DataItem;
        //        var actionActivities = bizActionActivity.GetBackendActionActivities(editQuertyResult.QueryItem.DataItem.TargetEntityID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.BeforeDelete }, true, true);
        //        CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
        //        foreach (var entityActionActivity in actionActivities)
        //        {
        //            if (entityActionActivity.CodeFunctionID != 0)
        //            {
        //                var resultFunction = codeFunctionHelper.GetCodeFunctionResultOneDataItem(requester, entityActionActivity.CodeFunction, editQuertyResult.QueryItem.DataItem);
        //                if (resultFunction.Exception != null)
        //                {

        //                    editQuertyResult.BeforeSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
        //                    editQuertyResult.BeforeSaveActionActivitiesMessage += resultFunction.Exception.Message;
        //                    return;
        //                }
        //            }
        //        }
        //    }
        //}

        internal bool DoBeforeEditActionActivities(DR_Requester requester, List<QueryItem> allQueryItems, BaseResult result)
        {
            bool doneWithoutException = true;
            // EditDataActionActivityManager.DoBeforeEditActionActivities: d451b5c0ef6e

            //var preEditQueryResults = new List<EditQueryPreItem>();
            //foreach (var item in allQueryItems)
            //    preEditQueryResults.Add(new EditQueryPreItem(item));
            CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
            BizLogManager bizLogManager = new BizLogManager();
            BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();

            foreach (var queryItem in allQueryItems)
            {
                var listActionType = new List<Enum_EntityActionActivityStep>();
                if (queryItem.QueryType == Enum_QueryItemType.Insert || queryItem.QueryType == Enum_QueryItemType.Update)
                    listActionType.Add(Enum_EntityActionActivityStep.BeforeSave);
                else if (queryItem.QueryType == Enum_QueryItemType.Delete)
                    listActionType.Add(Enum_EntityActionActivityStep.BeforeDelete);
                var actionActivities = bizActionActivity.GetBackendActionActivities(queryItem.DataItem.TargetEntityID, listActionType, true, DetailsDepth.WithDetailsAndObjects);
                if (actionActivities.Any())
                {
                    foreach (var entityActionActivity in actionActivities)
                    {
                        var resultFunction = codeFunctionHelper.GetCodeFunctionResultOneDataItem(requester, entityActionActivity.CodeFunction, queryItem.DataItem);
                        queryItem.BeforeSaveActionActivitiesResult.Add(new BackendActionActivityResult()
                        {
                            BackendActionActivityID = entityActionActivity.ID,
                            Exception = resultFunction.Exception
                        });
                        if (resultFunction.Exception != null)
                        {
                            doneWithoutException = false;
                            var logResult = bizLogManager.AddLog(GetActionExceptionLog(queryItem, resultFunction.Exception.Message), requester);
                        }
                    }
                }
                if (queryItem.BeforeSaveActionActivitiesResult.Any(x => x.Exception != null))
                {
                    var message = "فعالیتهای قبل از ذخیره شدن داده با خطا همراه بود";
                    var systemmessage = "";
                    foreach (var item in queryItem.BeforeSaveActionActivitiesResult.Where(x => x.Exception != null))
                    {
                        systemmessage += (systemmessage == "" ? "" : " , ") + item.Exception.Message;
                    }
                    result.Details.Add(ToResultDetail(queryItem.DataItem.ViewInfo, message, systemmessage));
                }
            }
            return doneWithoutException;
        }

        internal bool DoAfterEditActionActivities(DR_Requester requester, List<QueryItem> allQueryItems, BaseResult result)
        {
            // EditDataActionActivityManager.DoAfterEditActionActivities: 349e5b1092af
            bool doneWithoutException = true;

            CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
            BizLogManager bizLogManager = new BizLogManager();
            BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
            DatabaseFunctionHandler databaseFunctionHandler = new DatabaseFunctionHandler();
            foreach (var queryItem in allQueryItems)
            {
                var listActionType = new List<Enum_EntityActionActivityStep>();
                if (queryItem.QueryType == Enum_QueryItemType.Insert || queryItem.QueryType == Enum_QueryItemType.Update)
                    listActionType.Add(Enum_EntityActionActivityStep.AfterSave);
                else if (queryItem.QueryType == Enum_QueryItemType.Delete)
                    listActionType.Add(Enum_EntityActionActivityStep.AfterSave);
                var actionActivities = bizActionActivity.GetBackendActionActivities(queryItem.DataItem.TargetEntityID, listActionType, true, , DetailsDepth.WithDetailsAndObjects);
                if (actionActivities.Any())
                {
                    foreach (var entityActionActivity in actionActivities)
                    {
                        FunctionResult resultFunction = null;
                        if (entityActionActivity.CodeFunctionID != 0)
                        {
                            resultFunction = codeFunctionHelper.GetCodeFunctionResultOneDataItem(requester, entityActionActivity.CodeFunction, queryItem.DataItem);
                        }
                        else if (entityActionActivity.DatabaseFunctionEntityID != 0)
                        {
                            ////اصلاح شود و با خصوصیات صدا زده شود یا حداقل لیست خصوصیات ارسال شود چون بهتره ارتباط 
                            resultFunction = databaseFunctionHandler.GetDatabaseFunctionValue(requester, entityActionActivity.DatabaseFunctionEntityID, queryItem.DataItem);
                        }

                        queryItem.AfterSaveActionActivitiesResult.Add(new BackendActionActivityResult() { BackendActionActivityID = entityActionActivity.ID, Exception = resultFunction.Exception });

                        if (resultFunction.Exception != null)
                        {
                            doneWithoutException = true;
                            //var logResult = bizLogManager.AddLog(GetActionExceptionLog(queryItem, message), requester);
                            //if (!string.IsNullOrEmpty(logResult))
                            //    result.Details.Add(ToResultDetail("خطا در ثبت لاگ", "", logResult));
                        }
                    }
                }
                if (queryItem.AfterSaveActionActivitiesResult.Any(x => x.Exception != null))
                {
                    var message = "فعالیتهای بعد از ذخیره شدن داده با خطا همراه بود";
                    var systemmessage = "";
                    foreach (var item in queryItem.AfterSaveActionActivitiesResult.Where(x => x.Exception != null))
                    {
                        systemmessage += (systemmessage == "" ? "" : " , ") + item.Exception.Message;
                    }
                    result.Details.Add(ToResultDetail(queryItem.DataItem.ViewInfo, message, systemmessage));
                }
            }
            return doneWithoutException;

        }

        private ResultDetail ToResultDetail(string title, string description, string technicalDescription)
        {
            var detail = new ResultDetail();
            detail.Title = title;
            detail.Description = description;
            detail.TechnicalDescription = technicalDescription;
            return detail;
        }
        private DataLogDTO GetActionExceptionLog(QueryItem queryItem, string message)
        {
            var dataLog = GetBaseLog(queryItem.DataItem);
            dataLog.MajorException = true;
            dataLog.MajorFunctionExceptionMessage = message;
            //     dataLog.MainType=DataLogType.
            return dataLog;
        }
        private DataLogDTO GetBaseLog(DP_DataRepository dataItem)
        {
            var dataLog = new DataLogDTO();
            dataLog.LocationInfo = "";
            dataLog.Duration = 0;
            dataLog.DatItem = dataItem;
            return dataLog;
        }
        //public void DoAfterEditActionActivities(DR_Requester requester, List<EditQueryResultItem> items)
        //{
        //    BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
        //    foreach (var editQuertyResult in items.Where(x => x.QueryItem.QueryType == Enum_QueryItemType.Insert || x.QueryItem.QueryType == Enum_QueryItemType.Update))
        //    {
        //        var queryItem = editQuertyResult.QueryItem;
        //        if (queryItem.QueryType == Enum_QueryItemType.Insert || queryItem.QueryType == Enum_QueryItemType.Update)
        //        {
        //            var actionActivities = bizActionActivity.GetBackendActionActivities(queryItem.DataItem.TargetEntityID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.AfterSave }, false, true);
        //            CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
        //            DatabaseFunctionHandler databaseFunctionHandler = new DatabaseFunctionHandler();
        //            foreach (var entityActionActivity in actionActivities)
        //            {
        //                if (entityActionActivity.CodeFunctionID != 0)
        //                {
        //                    var resultFunction = codeFunctionHelper.GetCodeFunctionResultOneDataItem(requester, entityActionActivity.CodeFunction, queryItem.DataItem);
        //                    if (resultFunction.Exception != null)
        //                    {

        //                        editQuertyResult.AfterSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
        //                        editQuertyResult.AfterSaveActionActivitiesMessage += resultFunction.Exception.Message;
        //                    }
        //                }
        //                else if (entityActionActivity.DatabaseFunctionEntityID != 0)
        //                {
        //                    ////اصلاح شود و با خصوصیات صدا زده شود یا حداقل لیست خصوصیات ارسال شود چون بهتره ارتباط 
        //                    var resultFunction = databaseFunctionHandler.GetDatabaseFunctionValue(requester, entityActionActivity.DatabaseFunctionEntityID, queryItem.DataItem);
        //                    if (resultFunction.Exception != null)
        //                    {

        //                        editQuertyResult.AfterSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
        //                        editQuertyResult.AfterSaveActionActivitiesMessage += resultFunction.Exception.Message;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    //    var deleteResult = new DR_ResultDelete();
        //    //new DeleteRequestManager().DoAfterDeleteActionActivities(requester, queryItems.Where(x => x.QueryType == Enum_QueryItemType.Delete), deleteResult);
        //    //result.Message += deleteResult.Message;
        //}
        //public void DoAfterDeleteActionActivities(DR_Requester requester, List<EditQueryResultItem> items)
        //{
        //    BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
        //    foreach (var editQuertyResult in items.Where(x => x.QueryItem.QueryType == Enum_QueryItemType.Delete))
        //    {
        //        var queryItem = editQuertyResult.QueryItem;
        //        var actionActivities = bizActionActivity.GetBackendActionActivities(queryItem.TargetEntity.ID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.AfterDelete }, false, true);
        //        CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
        //        DatabaseFunctionHandler databaseFunctionHandler = new DatabaseFunctionHandler();
        //        foreach (var entityActionActivity in actionActivities)
        //        {
        //            if (entityActionActivity.CodeFunctionID != 0)
        //            {
        //                var resultFunction = codeFunctionHelper.GetCodeFunctionResultOneDataItem(requester, entityActionActivity.CodeFunction, queryItem.DataItem);
        //                if (resultFunction.Exception != null)
        //                {

        //                    editQuertyResult.AfterSaveActionActivitiesResult = Enum_DR_SimpleResultType.ExceptionThrown;
        //                    editQuertyResult.AfterSaveActionActivitiesMessage += resultFunction.Exception.Message;
        //                }
        //            }

        //        }
        //    }
        //}
    }
}
