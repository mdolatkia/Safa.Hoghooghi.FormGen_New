
using ModelEntites;

using MyConnectionManager;

using MyDataItemManager;
using MyGeneralLibrary;
using MyLogManager;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDataEditManagerBusiness
{
    public class EditRequestManager
    {
        EditDataActionActivityManager actionActivityManager = new EditDataActionActivityManager();
        //EditDataFormulaUsageManager editDataFormulaUsageManager = new EditDataFormulaUsageManager();
        EditQueryItemManager editQueryItemManager = new EditQueryItemManager();
        BizLogManager bizLogManager = new BizLogManager();
        public DR_ResultEdit ProcessEditRequest(DR_EditRequest request)
        {
            //**EditRequestManager.ProcessEditRequest: 0cbaba268135

            DR_ResultEdit result = new DR_ResultEdit();



            //کوئری ها نباید ساخته بشن فقط دیتا آیتم نهایی آماده بشه
            var allUpdateQueryItems = editQueryItemManager.GetQueryItems(request.Requester, request.EditPackages);
            var allDeleteQueryItems = deleteQueryItemManager.GetDeleteQueryItems(request.Requester, request.DeletePackages).Select(x => x.Item2).ToList();
            var allQueryItems = allDeleteQueryItems.Union(allUpdateQueryItems).ToList();
            //
            if (allUpdateQueryItems.Any(x => string.IsNullOrEmpty(x.Query)))
            {
                throw new Exception("sdfsdf");
            }
            if (allDeleteQueryItems.Any(x => string.IsNullOrEmpty(x.Query)))
            {
                throw new Exception("sdfsdf");
            }

            //اینجا دیلیت ها چک نمیشه
            CheckPermissoinToEdit(request.Requester, result, allQueryItems);
            if (result.Result == Enum_DR_ResultType.ExceptionThrown)
                return result;



            //var preDeleteueryResults = new List<EditQueryPreItem>();
            //foreach (var item in allDeleteQueryItems)
            //    preDeleteueryResults.Add(new EditQueryPreItem(item));


            //   actionActivityManager.DoBeforeDeleteActionActivities(request.Requester, preDeleteueryResults);


            //اینجا باید کوئری ها تازه ساخته بشن از دیتاآیتم های آماده

            if (!actionActivityManager.DoBeforeEditActionActivities(request.Requester, allUpdateQueryItems, result))
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                return result;
            }
            else
            {

                var transactionresult = ConnectionManager.ExecuteTransactionalQueryItems(allQueryItems);
                if (transactionresult.Successful)
                {
                    if (actionActivityManager.DoAfterEditActionActivities(request.Requester, allQueryItems, result))
                        result.Result = Enum_DR_ResultType.SeccessfullyDone;
                    else
                        result.Result = Enum_DR_ResultType.JustMajorFunctionDone;

                    var logResult = bizLogManager.AddLogs(GetUpdateDataSuccessfulLogs(allQueryItems), request.Requester);
                    if (!string.IsNullOrEmpty(logResult))
                        result.Details.Add(ToResultDetail("خطا در ثبت لاگ", "", logResult));

                    foreach (var item in request.EditPackages)
                    {
                        var baseData = new DP_BaseData(item.TargetEntityID, item.TargetEntityAlias);
                        var listKeyProperties = new List<EntityInstanceProperty>();
                        if (item.IsNewItem && item.KeyProperties.Any(x => x.Column.IsIdentity))
                        {
                            var dataItem = allQueryItems.First(x => x.DataItem == item).DataItem;
                            foreach (var key in dataItem.KeyProperties)
                                baseData.Properties.Add(key);
                        }
                        else
                        {
                            foreach (var key in item.KeyProperties)
                                baseData.Properties.Add(key);
                        }
                        result.UpdatedItems.Add(baseData);
                    }
                }
                else
                {
                    result.Result = Enum_DR_ResultType.ExceptionThrown;
                    if (allQueryItems.Any(x => x.DataUpdateResult == Enum_DR_SimpleResultType.ExceptionThrown))
                    {
                        foreach (var item in allQueryItems.Where(x => x.DataUpdateResult == Enum_DR_SimpleResultType.ExceptionThrown))
                        {
                            var logResult = bizLogManager.AddLog(ToUpdateUnSuccessfulLog(item), request.Requester);
                            result.Details.Add(ToResultDetail(item.DataItem.ViewInfo, "ذخیره شدن داده با خطا همراه بود", item.DataUpdateMessage));
                        }

                    }
                    else
                    {
                        result.Details.Add(ToResultDetail("خطای عمومی ثبت", "ذخیره شدن داده با خطا همراه بود", transactionresult.Message));
                    }
                }
            }



            return result;
        }

        private void CheckPermissoinToEdit(DR_Requester requester, BaseResult result, List<QueryItem> allQueryItems)
        {

            //درست بشه دیلیت جدا بشه
            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            bool permission = true;
            foreach (var entityGroup in allQueryItems.GroupBy(x => x.TargetEntity.ID))
            {
                if (!bizTableDrivedEntity.DataIsAccessable(requester, entityGroup.Key, new List<SecurityAction>() { SecurityAction.EditAndDelete }))
                {
                    permission = false;
                    var entity = allQueryItems.First(x => x.TargetEntity.ID == entityGroup.Key).TargetEntity;
                    result.Details.Add(ToResultDetail("عدم دسترسی", "عدم دسترسی ثبت به موجودیت" + " " + entity.Alias, ""));
                }
                else if (bizTableDrivedEntity.EntityIsReadonly(requester, entityGroup.Key))
                {
                    permission = false;
                    var entity = allQueryItems.First(x => x.TargetEntity.ID == entityGroup.Key).TargetEntity;
                    result.Details.Add(ToResultDetail("عدم دسترسی", "عدم دسترسی ثبت به موجودیت" + " " + entity.Alias, ""));
                }
            }
            if (permission)
            {
                BizColumn bizColumn = new BizColumn();
                foreach (var query in allQueryItems)
                {
                    foreach (var column in query.EditingProperties)
                    {
                        if (!bizColumn.DataIsAccessable(requester, column.ColumnID, true))
                        {
                            permission = false;
                            result.Details.Add(ToResultDetail("عدم دسترسی", "عدم دسترسی به ستون" + " " + column.Column.Alias, ""));
                        }
                        else if (column.ValueIsChanged && (bizColumn.DataIsReadonly(requester, column.ColumnID, null, query.DataItem.TargetEntityID)))
                        {
                            permission = false;
                            result.Details.Add(ToResultDetail("عدم دسترسی", "عدم دسترسی ثبت به ستون" + " " + column.Column.Alias, ""));
                        }
                    }
                }
            }

            if (permission == false)
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                result.Message = "خطا در ثبت";
            }
        }

        private List<DataLogDTO> GetUpdateDataSuccessfulLogs(List<QueryItem> queryItems)
        {
            List<DataLogDTO> result = new List<DataLogDTO>();
            var packageGUID = Guid.NewGuid();
            foreach (var item in queryItems)
            {
                DataLogDTO dataLog = ToUpdateSuccessfulLog(item, packageGUID);
                result.Add(dataLog);
            }
            return result;
        }

        //private void SetResultDetails(DR_ResultEdit result, string logResult, List<EditQueryResultItem> editItems)
        //{
        //    if (!string.IsNullOrEmpty(logResult))
        //        result.Details.Add(ToResultDetail("خطا در ثبت لاگ", "", logResult));
        //    foreach (var item in editItems)
        //    {
        //        if (item.AfterSaveActionActivitiesResult == Enum_DR_SimpleResultType.ExceptionThrown)
        //            result.Details.Add(ToResultDetail(item.QueryItem.DataItem.ViewInfo, "فعالیتهای بعد از ذخیره شدن داده با خطا همراه بود", item.AfterSaveActionActivitiesMessage));
        //        if (item.BeforeSaveActionActivitiesResult == Enum_DR_SimpleResultType.ExceptionThrown)
        //            result.Details.Add(ToResultDetail(item.QueryItem.DataItem.ViewInfo, "فعالیتهای قبل از ذخیره شدن داده با خطا همراه بود", item.BeforeSaveActionActivitiesMessage));
        //        if (item.DataUpdateResult == Enum_DR_SimpleResultType.ExceptionThrown)
        //            result.Details.Add(ToResultDetail(item.QueryItem.DataItem.ViewInfo, "ذخیره شدن داده با خطا همراه بود", item.DataUpdateMessage));
        //        //if (item.ForumulaUsageResult == Enum_DR_ResultType.ExceptionThrown)
        //        //    result.Details.Add(ToResultDetail(item.QueryItem.DataItem.ViewInfo, "ذخیره شدن سابقه فرمول برای داده با خطا همراه بود", item.ForumulaUsageMessage));
        //    }
        //}

        private ResultDetail ToResultDetail(string title, string description, string technicalDescription)
        {
            var detail = new ResultDetail();
            detail.Title = title;
            detail.Description = description;
            detail.TechnicalDescription = technicalDescription;
            return detail;
        }

        //private List<DataLogDTO> GetDataLogs(InternaleEditResult internalResult, DR_Requester requester)
        //{
        //    List<DataLogDTO> result = new List<DataLogDTO>();
        //    var packageGUID = Guid.NewGuid();
        //    if (internalResult.Result == Enum_DR_ResultType.SeccessfullyDone)
        //    {
        //        foreach (var item in internalResult.EditQueryResults)
        //        {
        //            DataLogDTO dataLog = ToDataLog(item, packageGUID, requester, false, false);
        //            result.Add(dataLog);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var item in internalResult.EditQueryResults.Where(x => x.BeforeSaveActionActivitiesResult == Enum_DR_SimpleResultType.ExceptionThrown
        //        || x.DataUpdateResult == Enum_DR_SimpleResultType.ExceptionThrown || x.AfterSaveActionActivitiesResult == Enum_DR_SimpleResultType.ExceptionThrown))
        //        {
        //            DataLogDTO dataLog = ToDataLog(item, packageGUID, requester, internalResult.Result == Enum_DR_ResultType.ExceptionThrown, internalResult.Result == Enum_DR_ResultType.JustMajorFunctionDone);
        //            result.Add(dataLog);
        //        }
        //    }
        //    return result;
        //}




        private DataLogDTO GetBaseLog(QueryItem queryItem)
        {
            var dataLog = new DataLogDTO();
            dataLog.LocationInfo = "";
            dataLog.Duration = 0;
            dataLog.DatItem = queryItem.DataItem;
            dataLog.MainType = GetLogType(queryItem);
            return dataLog;
        }

        DeleteQueryItemManager deleteQueryItemManager = new DeleteQueryItemManager();
        public DR_ResultDelete ProcessDeleteRequest(DR_DeleteRequest request)
        {

            DR_ResultDelete result = new DR_ResultDelete();
            var itemsAndQueries = deleteQueryItemManager.GetDeleteQueryItems(request.Requester, request.DataItems);
            var allQueryItems = itemsAndQueries.Select(x => x.Item2).ToList();
            //var deleteQueryResults = new List<EditQueryPreItem>();
            //foreach (var item in itemsAndQueries)
            //{
            //    // foreach (var childitem in item.Item2)
            //    deleteQueryResults.Add(new EditQueryPreItem(item.Item2));
            //}
            //کاملا مثل ادیت .یکی شوند
            //  var allQueryItems = allUpdateQueryItems.Select(x => x.QueryItem).ToList();
            if (allQueryItems.Any(x => string.IsNullOrEmpty(x.Query)))
            {
                throw new Exception("sdfsdf");
            }

            CheckPermissoinToEdit(request.Requester, result, allQueryItems);
            if (result.Result == Enum_DR_ResultType.ExceptionThrown)
                return result;

            if (!actionActivityManager.DoBeforeEditActionActivities(request.Requester, allQueryItems, result))
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                return result;
            }
            else
            {

                var transactionresult = ConnectionManager.ExecuteTransactionalQueryItems(allQueryItems);




                if (transactionresult.Successful)
                {
                    actionActivityManager.DoAfterEditActionActivities(request.Requester, allQueryItems, result);

                    var logResult = bizLogManager.AddLogs(GetUpdateDataSuccessfulLogs(allQueryItems), request.Requester);
                    if (!string.IsNullOrEmpty(logResult))
                        result.Details.Add(ToResultDetail("خطا در ثبت لاگ", "", logResult));
                }
                else
                {
                    result.Result = Enum_DR_ResultType.ExceptionThrown;
                    if (allQueryItems.Any(x => x.DataUpdateResult == Enum_DR_SimpleResultType.ExceptionThrown))
                    {
                        foreach (var item in allQueryItems.Where(x => x.DataUpdateResult == Enum_DR_SimpleResultType.ExceptionThrown))
                        {
                            var logResult = bizLogManager.AddLog(ToUpdateUnSuccessfulLog(item), request.Requester);
                            result.Details.Add(ToResultDetail(item.DataItem.ViewInfo, "ذخیره شدن داده با خطا همراه بود", item.DataUpdateMessage));
                        }

                    }
                    else
                    {
                        result.Details.Add(ToResultDetail("خطای عمومی ثبت", "ذخیره شدن داده با خطا همراه بود", transactionresult.Message));
                    }
                }
            }

            return result;
        }



        private DataLogType GetLogType(QueryItem queryItem)
        {
            if (queryItem.QueryType == Enum_QueryItemType.Insert)
                return DataLogType.DataInsert;
            else if (queryItem.QueryType == Enum_QueryItemType.Update)
                return DataLogType.DataEdit;
            else if (queryItem.QueryType == Enum_QueryItemType.Delete)
                return DataLogType.DataDelete;
            return DataLogType.Unknown;
        }
        private DataLogDTO ToUpdateUnSuccessfulLog(QueryItem item)
        {
            var dataLog = GetBaseLog(item);
            dataLog.MajorException = true;
            dataLog.MajorFunctionExceptionMessage = item.DataUpdateMessage;
            dataLog.EditDataItemExtraLog = new EditDataItemExtraLogDTO();
            dataLog.EditDataItemExtraLog.DataUpdateQuery = item.Query;
            string extraMessage = "";
            if (item.BeforeSaveActionActivitiesResult.Any())
            {
                foreach (var before in item.BeforeSaveActionActivitiesResult)
                {
                    extraMessage += (extraMessage == "" ? "" : " , ") + "BAC:" + before.BackendActionActivityID + (before.Exception == null ? "" : ("#" + before.Exception));
                }
            }
            dataLog.EditDataItemExtraLog.Message = extraMessage;
            return dataLog;
        }

        private DataLogDTO ToUpdateSuccessfulLog(QueryItem item, Guid packageGUID)
        {
            var dataLog = GetBaseLog(item);

            dataLog.EditDataItemExtraLog = new EditDataItemExtraLogDTO();
            dataLog.EditDataItemExtraLog.DataUpdateQuery = item.Query;
            string extraMessage = "";
            if (item.BeforeSaveActionActivitiesResult.Any())
            {
                foreach (var before in item.BeforeSaveActionActivitiesResult)
                {
                    extraMessage += (extraMessage == "" ? "" : " , ") + "BAC:" + before.BackendActionActivityID + (before.Exception == null ? "" : ("#" + before.Exception));
                }
            }
            if (item.AfterSaveActionActivitiesResult.Any())
            {
                dataLog.MinorException = item.AfterSaveActionActivitiesResult.Any(x => x.Exception != null);
                foreach (var after in item.AfterSaveActionActivitiesResult)
                {
                    extraMessage += (extraMessage == "" ? "" : " , ") + "AAC:" + after.BackendActionActivityID + (after.Exception == null ? "" : ("#" + after.Exception));
                }
            }
            dataLog.EditDataItemExtraLog.Message = extraMessage;
            foreach (var column in item.EditingProperties)
            {
                var logColumn = new EditDataItemColumnDetailsDTO();
                logColumn.ColumnID = column.ColumnID;
                logColumn.NewValue = column.Value;
                if (item.DataItem.OriginalProperties.Any(x => x.ColumnID == column.ColumnID))
                    logColumn.OldValue = item.DataItem.OriginalProperties.First(x => x.ColumnID == column.ColumnID).Value;
                if (column.FormulaID != 0)
                {
                    logColumn.FormulaID = column.FormulaID;
                    logColumn.FormulaException = column.FormulaException;
                    foreach (var fparam in column.FormulaUsageParemeters)
                    {
                        logColumn.FormulaUsageParemeters.Add(new FormulaUsageParemetersDTO()
                        {
                            ParameterName = fparam.ParameterName,
                            ParameterValue = fparam.ParameterValue,
                            RelationshipPropertyTail = fparam.RelationshipPropertyTail,
                        });
                    }

                }
                dataLog.EditDataItemColumnDetails.Add(logColumn);
            }
            return dataLog;
        }

        //private DataLogDTO GetBeforeDeleteExceptionLog(EditQueryPreItem item)
        //{
        //    var dataLog = GetBaseLog(item.QueryItem);
        //    dataLog.MajorException = true;
        //    dataLog.MajorFunctionExceptionMessage = item.BeforeSaveActionActivitiesMessage;
        //    return dataLog;
        //}
        //public InternaleEditResult GetInternalResult(DR_DeleteRequest request)
        //{
        //    InternaleEditResult result = new InternaleEditResult();




        //    var transactionresult = ConnectionManager.ExecuteTransactionalQueryItems(result.EditQueryResults.Select(x => x.QueryItem).ToList());
        //    if (transactionresult.Successful)
        //    {
        //        result.Result = Enum_DR_ResultType.SeccessfullyDone;
        //        actionActivityManager.DoAfterEditActionActivities(request.Requester, result);
        //    }
        //    else
        //    {
        //        result.Result = Enum_DR_ResultType.ExceptionThrown;
        //        foreach (var item in transactionresult.QueryItems)
        //        {
        //            result.EditQueryResults.First(x => x.QueryItem == item.QueryItem).DataUpdateMessage = item.Exception.Message;
        //            result.EditQueryResults.First(x => x.QueryItem == item.QueryItem).DataUpdateResult = Enum_DR_ResultType.ExceptionThrown;

        //        }
        //    }

        //    return result;
        //}


        public DR_DeleteInquiryResult Process(DR_DeleteInquiryRequest request)
        {
            DR_DeleteInquiryResult result = new DR_DeleteInquiryResult();
            foreach (var item in request.DataItems)
            {
                if (!result.Loop)
                {
                    DP_DataRepository rootDeleteITem = new DP_DataRepository(deleteQueryItemManager.GetDataView(item));
                    // rootDeleteITem.DataView = ;
                    var treeResult = deleteQueryItemManager.GetTreeItems(request.Requester, rootDeleteITem, rootDeleteITem);
                    result.DataTreeItems.Add(rootDeleteITem);
                    if (treeResult)
                    {
                        result.Loop = true;
                        break;
                    }
                }

            }

            return result;
        }

        //private InternaleEditResult GetInternalResult(DR_EditRequest request)
        //{
        //    InternaleEditResult result = new InternaleEditResult();
        //    var allQueryItems = editQueryItemManager.GetQueryItems(request.EditPackages);
        //    foreach (var item in allQueryItems)
        //        result.EditQueryResults.Add(new MyDataEditManagerBusiness.EditQueryResultItem(item));

        //    if (allQueryItems.Any(x => string.IsNullOrEmpty(x.Query)))
        //    {
        //        throw new Exception("sdfsdf");
        //    }

        //    actionActivityManager.DoBeforeEditActionActivities(request.Requester, result);

        //    var transactionresult = ConnectionManager.ExecuteTransactionalQueryItems(allQueryItems);
        //    if (transactionresult.Successful)
        //    {
        //        actionActivityManager.DoAfterEditActionActivities(request.Requester, result);
        //        //editDataFormulaUsageManager.UpdateFormulaUsage(allQueryItems, result);
        //        //if (result.EditQueryResults.Any(x => x.AfterSaveActionActivitiesResult == Enum_DR_SimpleResultType.ExceptionThrown ))
        //        //    result.Result = Enum_DR_ResultType.JustMajorFunctionDone;
        //        //else
        //        //    result.Result = Enum_DR_ResultType.SeccessfullyDone;
        //    }
        //    else
        //    {
        //        //result.Result = Enum_DR_ResultType.ExceptionThrown;
        //        foreach (var item in transactionresult.QueryItems.Where(x=>x.Exception!=null))
        //        {
        //            result.EditQueryResults.First(x => x.QueryItem == item.QueryItem).DataUpdateMessage = item.Exception.Message;
        //            result.EditQueryResults.First(x => x.QueryItem == item.QueryItem).DataUpdateResult = Enum_DR_SimpleResultType.ExceptionThrown;

        //        }
        //    }
        //    return result;
        //}


    }
    //public class InternaleEditResult
    //{
    //    public InternaleEditResult()
    //    {
    //        EditQueryResults = new List<EditQueryResultItem>();

    //    }
    //    public List<EditQueryResultItem> EditQueryResults { set; get; }
    //    //    public Enum_DR_ResultType Result { set; get; }
    //}
    //public class EditQueryPreItem
    //{
    //    public EditQueryPreItem(QueryItem queryItem)
    //    {
    //        QueryItem = queryItem;
    //    }
    //    public QueryItem QueryItem { set; get; }
    //    public string BeforeSaveActionActivitiesMessage { set; get; }
    //    public Enum_DR_SimpleResultType BeforeSaveActionActivitiesResult { set; get; }

    //}

    //public class EditQueryResultItem
    //{
    //    public EditQueryResultItem(QueryItem queryItem)
    //    {
    //        QueryItem = queryItem;
    //    }

    //    public QueryItem QueryItem { set; get; }
    //    public string DataUpdateMessage { set; get; }
    //    public Enum_DR_SimpleResultType DataUpdateResult { set; get; }
    //    public Enum_DR_SimpleResultType AfterSaveActionActivitiesResult { set; get; }


    //    public string AfterSaveActionActivitiesMessage { set; get; }
    //    public string BeforeSaveActionActivitiesMessage { set; get; }
    //    public Enum_DR_SimpleResultType BeforeSaveActionActivitiesResult { set; get; }
    //    //public Enum_DR_ResultType ForumulaUsageResult { set; get; }
    //    public string ForumulaUsageMessage { set; get; }
    //}
    public enum QueryItemType
    {
        none,
        insert,
        update
    }
}
