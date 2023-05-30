

using ModelEntites;

using MyConnectionManager;

using MyDataManagerBusiness;
using MyModelManager;
using MyRelationshipDataManager;

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyDataSearchManagerBusiness
{
    public class SearchRequestManager
    {
        BizFormula bizFormula = new BizFormula();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizRelationship bizRElationship = new BizRelationship();
        BizColumn bizColumn = new BizColumn();
        //BizOrganizationSecurity bizOrganizationSecurity = new BizOrganizationSecurity();
        ModelDataHelper dataHelper = new ModelDataHelper();
        BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();
        //TableDrivedEntityDTO mainEntity { set; get; }
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        BizEntityListView bizEntityListView = new BizEntityListView();
        BizEntitySearch bizEntitySearch = new BizEntitySearch();
        //TableDrivedEntityDTO connectionEntity { set; get; }

        public DR_ResultSearchExists Process(DR_SearchExistsRequest request)
        {
            DR_ResultSearchExists result = new DR_ResultSearchExists();
            try
            {
                // connectionEntity = mainEntity;
                var fromClause = GetFromQuery(request.Requester, request.SearchDataItems.TargetEntityID, request.SearchDataItems);

                //بخش سلکت را طبق خصویات و روابط نمایشی میسازد
                var select = "Top 1 1";

                var commandStr = "select " + select + fromClause.Item2;
                var executeResult = ConnectionManager.GetDBHelperByEntityID(request.SearchDataItems.TargetEntityID).ExecuteScalar(commandStr);
                if (executeResult != null && executeResult.ToString() == "1")
                    result.ExistsResult = true;
                else
                    result.ExistsResult = false;
            }
            catch (Exception ex)
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                result.Message = "خطا در جستجو" + Environment.NewLine + ex.Message;
            }
            return result;
        }

        public DR_ResultSearchCount Process(DR_SearchCountRequest request)
        {
            DR_ResultSearchCount result = new DR_ResultSearchCount();

            try
            {

                // connectionEntity = mainEntity;
                var fromClause = GetFromQuery(request.Requester, request.SearchDataItems.TargetEntityID, request.SearchDataItems);
                //بخش سلکت را طبق خصویات و روابط نمایشی میسازد
                var select = "Count(*)";
                var commandStr = "select " + select + fromClause.Item2;

                var executeResult = ConnectionManager.GetDBHelperByEntityID(request.SearchDataItems.TargetEntityID).ExecuteScalar(commandStr);
                if (executeResult != null)
                    result.ResultCount = (int)executeResult;
                else
                    result.ResultCount = 0;
            }
            catch (Exception ex)
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                result.Message = "خطا در جستجو" + Environment.NewLine + ex.Message;
            }
            return result;
        }


        //private int GetSearchCountResult(DR_Requester requester, TableDrivedEntityDTO entity, DP_SearchRepository searchDataItem)
        //{
        //    //TableDrivedEntityDTO entity = bizTableDrivedEntity.GetTableDrivedEntity(entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
        //}

        //public DR_ResultSearchView Process(DR_SearchEditViewRequest request)
        //{
        //    var result = Process(request as DR_SearchViewRequest);
        //    DataSearchStateManager dataSearchStateManager = new DataSearchStateManager();
        //    if (request.ToParentRelationshipID != 0)
        //    {
        //        foreach (var item in result.ResultDataItems)
        //        {
        //            item.
        //            }
        //    }
        //    return result;
        //}
        //catch (Exception ex)
        //{
        //    result.Result = Enum_DR_ResultType.ExceptionThrown;
        //    result.Message = "خطا در جستجو" + Environment.NewLine + ex.Message;
        //}



        public DR_ResultSearchView ProcessSearchViewRequest(DR_SearchViewRequest request)
        {
            //SearchRequestProcessor.ProcessSearchViewRequest: 24931a627c37
            DR_ResultSearchView result = new DR_ResultSearchView();
            //var mainEntity = GetTableDrivedEntity1(request.Requester, request.SearchDataItems.TargetEntityID);
            //if (mainEntity == null)
            //{
            //    result.Result = Enum_DR_ResultType.ExceptionThrown;
            //    result.Message = "عدم دسترسی به موجودیت به شناسه" + " " + request.SearchDataItems.TargetEntityID + " ";
            //    return result;
            //}


            //try
            //{

            EntityListViewDTO listView = null;
            if (request.EntityViewID != 0)
                listView = bizEntityListView.GetEntityListView(request.Requester, request.EntityViewID);
            else
                listView = bizEntityListView.GetOrCreateEntityListViewDTO(request.Requester, request.SearchDataItems.TargetEntityID);
            var dataTable = GetDataTableBySearchDataItems(request.Requester, request.SearchDataItems.TargetEntityID, request.SearchDataItems, listView.EntityListViewAllColumns, request.MaxDataItems, request.OrderByEntityViewColumnID, request.SortType);
            result.ResultDataItems = DataTableToDP_ViewRepository(dataTable.Item1, dataTable.Item2, listView);
            result.Result = Enum_DR_ResultType.SeccessfullyDone;

            //if(request.CheckStates)
            //{
            //    DataSearchStateManager dataSearchStateManager = new DataSearchStateManager(request.SearchDataItems.TargetEntityID);
            //    foreach (var item in result.ResultDataItems)
            //    {
            //        dataSearchStateManager.
            //    }
            //}
            //}
            //catch (Exception ex)
            //{
            //    result.Result = Enum_DR_ResultType.ExceptionThrown;
            //    result.Message = "خطا در جستجو" + Environment.NewLine + ex.Message;
            //}
            return result;
        }



        SecurityHelper securityHelper = new SecurityHelper();
        //private TableDrivedEntityDTO GetTableDrivedEntity1(DR_Requester requester, int targetEntityID, SecurityMode securityMode)
        //{
        //    return ;

        //}
        //private TableDrivedEntityDTO GetTableDrivedEntity2(int targetEntityID, EntityColumnInfoType withSimpleColumns, EntityRelationshipInfoType withRelationships)
        //{
        //    //DP_EntityPermissionResult result = new DP_EntityPermissionResult();
        //    //var permissions = securityHelper.GetAssignedPermissions(requester, targetEntityID, DatabaseObjectCategory.Entity, false);
        //    //if (permissions.GrantedActions.Any(x => x == SecurityAction.NoAccess))
        //    //    return null;
        //    //else
        //    return bizTableDrivedEntity.GetTableDrivedEntity(targetEntityID, withSimpleColumns, withRelationships);
        //}
        public Tuple<TableDrivedEntityDTO, DataTable> GetDataTableBySearchDataItems(DR_Requester requester, int entityID, DP_SearchRepositoryMain searchDataItem
            , List<EntityListViewColumnsDTO> listColumns, int maxItems = 0, int orderColumnID = 0
            , Enum_OrderBy sortType = Enum_OrderBy.Ascending)
        {
            var queryParts = GetQueryParts(requester, entityID, searchDataItem, listColumns, maxItems, orderColumnID, sortType);
            var commandStr = "select " + queryParts.Item2 + queryParts.Item3 + queryParts.Item4;
            return new Tuple<TableDrivedEntityDTO, DataTable>(queryParts.Item1, ConnectionManager.GetDBHelperByEntityID(entityID).ExecuteQuery(commandStr));

        }

        private Tuple<TableDrivedEntityDTO, string, string, string> GetQueryParts(DR_Requester requester, int entityID, DP_SearchRepositoryMain searchDataItem
            , List<EntityListViewColumnsDTO> listColumns
            , int maxItems = 0, int orderColumnID = 0, Enum_OrderBy sortType = Enum_OrderBy.Ascending)
        {
            var fromClause = GetFromQuery(requester, entityID, searchDataItem);
            string orderBy = "";// GetOrderByQuery(fromClause.Item1, listView, orderColumnID, sortType);
            var select = GetSelectQuery(requester, fromClause.Item1, listColumns);
            return new Tuple<TableDrivedEntityDTO, string, string, string>(fromClause.Item1, select, fromClause.Item2, orderBy);
        }







        public Tuple<string, string> GetSelectFromExternal(DR_Requester requester, int entityID, DP_SearchRepositoryMain searchDataItem, bool primaryKeys)
        {


            //TableDrivedEntityDTO entity = bizTableDrivedEntity.GetTableDrivedEntity(entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
            //var entity = GetTableDrivedEntity1(requester, entityID, securityMode);
            //if (entity == null)
            //{
            //    throw new Exception("عدم دسترسی به موجودیت به شناسه" + " " + entityID + " ");
            //}
            //  EntityListViewDTO listView = null;
            List<ColumnDTO> listColumns = new List<ColumnDTO>();

            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);

            if (primaryKeys)
            {
                listColumns = entityDTO.Columns.Where(x => x.PrimaryKey).ToList(); ;
            }
            else
            {
                listColumns = entityDTO.Columns;
            }
            var listView = GetListViewFromColumns(listColumns);
            var queryParts = GetQueryParts(requester, entityID, searchDataItem, listView.EntityListViewAllColumns);
            return new Tuple<string, string>(queryParts.Item3, queryParts.Item4);
            //return new Tuple<string, string>("", "");
        }

        private EntityListViewDTO GetListViewFromColumns(List<ColumnDTO> listColumns)
        {
            EntityListViewDTO result = new EntityListViewDTO();
            foreach (var column in listColumns)
            {
                EntityListViewColumnsDTO rColumn = new EntityListViewColumnsDTO();
                rColumn.ID = 0;
                rColumn.ColumnID = column.ID;
                rColumn.Column = column;
                rColumn.Alias = column.Alias;
                rColumn.OrderID = (short)column.Position;
                result.EntityListViewAllColumns.Add(rColumn);
            }

            return result;
        }

        //private string GetQuery(DR_Requester requester, TableDrivedEntityDTO mainEntity, DP_SearchRepository searchDataItem, int maxDataItems, EntityListViewDTO entityListView, int orderColumnID, Enum_OrderBy sortType, SecurityMode securityMode)
        //{

        //}


        private string GetSelectQuery(DR_Requester requester, TableDrivedEntityDTO entity, List<EntityListViewColumnsDTO> listColumns)
        {

            var select = "";
            var searchTableName = GetSearchTableAlias(entity, 0);
            foreach (var column in listColumns)
            {
                if (column.RelationshipTail == null)
                {
                    var relativeColumnName = "'" + column.RelativeColumnName + "'";// "'" + column.Column.Name + "0'";// "'0>" + column.Column.Name + "'";
                    select += (select == "" ? "" : ",") + "[" + searchTableName + "]" + "." + "[" + column.Column.Name + "]" + " as " + relativeColumnName;
                }
                else
                {
                    //سلکت باید درست شود و هر فیلد خودش یک سلکت باشد
                    var aliasColumn = "'" + column.Column.Name + column.RelationshipTailID + "'";
                    var relatedSelectParam = GetSelectColumn(requester, searchTableName, 0, column.RelationshipTail, entity);
                    var from = relatedSelectParam[0].Item3 + " as " + relatedSelectParam[0].Item2;
                    for (int i = 1; i <= relatedSelectParam.Count - 1; i++)
                    {
                        from += (" inner join " + relatedSelectParam[i].Item3 + " as " + relatedSelectParam[i].Item2 + " on " + relatedSelectParam[i].Item4);
                    }
                    from += " where " + relatedSelectParam[0].Item4;
                    var lastTableName = relatedSelectParam.Last().Item2;
                    select += (select == "" ? "" : ",") + "(" + "select" + " " + "[" + lastTableName + "]" + "." + "[" + column.Column.Name + "]" + " " + "from" + " " + from + ")"
                        + " as " + aliasColumn;
                }
            }
            return select;
        }

        private string GetOrderByQuery(EntityListViewDTO entityListView, int orderColumnID, Enum_OrderBy sortType)
        {
            var orderBy = "";
            //if (orderColumnID != 0)
            //{
            //    string columnName = "";

            //        var column = entityListView.EntityListViewAllColumns.FirstOrDefault(x => x.ColumnID == orderColumnID);
            //        if (column != null)
            //        {
            //            columnName = column.Column.Name;
            //        }


            //    var searchTableName = GetSearchTableAlias(entity, 0);

            //    if (columnName != "")
            //        orderBy = " order by " + (string.IsNullOrEmpty(mainEntity.RelatedSchema) ? "" : mainEntity.RelatedSchema + ".") + searchTableName + "." + columnName + " " + (sortType == Enum_OrderBy.Ascending ? "Asc" : "Desc");
            //}
            return orderBy;
        }

        private Tuple<TableDrivedEntityDTO, string> GetFromQuery(DR_Requester requester, int entityID, DP_SearchRepositoryMain searchDataItem)
        {
            var entity = bizTableDrivedEntity.GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
            if (entity == null)
            {
                throw new Exception("عدم دسترسی به موجودیت به شناسه" + " " + entityID + " ");
            }
            if (requester.SkipSecurity == false)
            {
                var pathPermission = CheckSearchRepositoryPermission(requester, searchDataItem);
                if (!string.IsNullOrEmpty(pathPermission))
                {
                    throw new Exception(pathPermission);
                }
            }
            if (entity.InternalSuperToSubRelationship != null)
            {
                if (entity.InternalSuperToSubRelationship.SuperEntityDeterminerColumn != null && entity.InternalSuperToSubRelationship.DeterminerColumnValues.Any())
                {
                    searchDataItem.Phrases.Add(new SearchProperty(new ColumnDTO() { ID = entity.InternalSuperToSubRelationship.SuperEntityDeterminerColumnID })
                    {
                        Operator = CommonOperator.InValues,
                        Name = entity.InternalSuperToSubRelationship.SuperEntityDeterminerColumn.Name,
                        Value = GetDeterminerValues(entity)
                    });
                }
            }

            string securityQuery = "";
            if (requester != null && !requester.SkipSecurity)
            {
                securityQuery = GetSecurityQuery(requester, entity);
            }

            var searchQueryResult = GetSearchQuery(requester, entity, searchDataItem);
            var searchQuery = searchQueryResult.Item1;
            //جوین خارجی خودش با خودش
            var innerjoin = "";
            var searchTableName = GetSearchTableAlias(entity, 0);
            var searchTableNameWithSchema = (string.IsNullOrEmpty(entity.RelatedSchema) ? "" : entity.RelatedSchema + ".") + searchTableName;
            //** SearchRequestProcessor.GetFromQuery: 72ca04ba-004f-45aa-afb4-49a7ec0e684b
            if (!entity.IsView)
            {
                var onClause = "";
                foreach (var rCol in entity.Columns.Where(x => x.PrimaryKey))
                {
                    var ffield = "";
                    var sfield = "";
                    ffield = searchTableName + "." + rCol.Name;
                    sfield = "innerSearch" + "." + rCol.Name;
                    onClause += (onClause == "" ? "" : " and ") + ffield + "=" + sfield;
                }
                innerjoin = " inner join " + "(" + searchQuery + ") as innerSearch" + " on " + onClause;

                if (securityQuery != "")
                {
                    var securityOnClause = "";
                    foreach (var rCol in entity.Columns.Where(x => x.PrimaryKey))
                    {
                        var ffield = "";
                        var sfield = "";
                        ffield = searchTableName + "." + rCol.Name;
                        sfield = "securitySearch" + "." + rCol.Name;
                        securityOnClause += (securityOnClause == "" ? "" : " and ") + ffield + "=" + sfield;
                    }
                    innerjoin += " inner join " + "(" + securityQuery + ") as securitySearch" + " on " + securityOnClause;
                }
            }
            else
            {
                var firstSearchTableName = searchQueryResult.Item2;
                var index = searchQuery.IndexOf(firstSearchTableName);
                innerjoin = searchQuery.Substring(index + firstSearchTableName.Length, searchQuery.Length - (index + firstSearchTableName.Length));
                innerjoin = innerjoin.Replace(firstSearchTableName, searchTableNameWithSchema);

            }
            return new Tuple<TableDrivedEntityDTO, string>(entity, " from " + searchTableNameWithSchema + innerjoin);

        }
        private string CheckSearchRepositoryPermission(DR_Requester requester, DP_SearchRepositoryMain searchDataItem)
        {

            if (!bizTableDrivedEntity.DataIsAccessable(requester, searchDataItem.TargetEntityID))
                return "عدم دسترسی به موجودیت به شناسه" + " " + searchDataItem.TargetEntityID;

            return CheckLogicPhrasePermission(requester, searchDataItem);


        }
        private string CheckLogicPhrasePermission(DR_Requester requester, LogicPhraseDTO logicPhraseDTO)
        {
            var result = "";
            foreach (var phrase in logicPhraseDTO.Phrases)
            {
                if (phrase is SearchProperty)
                {
                    if (!bizColumn.DataIsAccessable(requester, (phrase as SearchProperty).ColumnID, false))
                        result += (result == "" ? "" : Environment.NewLine) + "عدم دسترسی به خصوصیت به شناسه" + " " + (phrase as SearchProperty).ColumnID;
                }
                else if (phrase is DP_SearchRepositoryRelationship)
                {
                    if (!bizRElationship.DataIsAccessable(requester, (phrase as DP_SearchRepositoryRelationship).SourceRelationship.ID, false, true, false))
                        result += (result == "" ? "" : Environment.NewLine) + "عدم دسترسی به رابطه به شناسه" + " " + (phrase as DP_SearchRepositoryRelationship).SourceRelationship.ID;
                }

                if (phrase is LogicPhraseDTO)
                {
                    var resultInner = CheckLogicPhrasePermission(requester, phrase as LogicPhraseDTO);
                    if (!string.IsNullOrEmpty(resultInner))
                        result += (result == "" ? "" : Environment.NewLine) + resultInner;
                }
            }
            return result;
        }


        private string GetDeterminerValues(TableDrivedEntityDTO mainEntity)
        {
            string result = "";
            foreach (var val in mainEntity.InternalSuperToSubRelationship.DeterminerColumnValues)
            {
                result += (result == "" ? "" : ",") + val.Value;
            }
            return result;
        }

        private string GetSecurityQuery(DR_Requester requester, TableDrivedEntityDTO mainEntity)
        {

            //   SecurityHelper securityHelper = new SecurityHelper();
            //  EntityDataSecurityItems securityItem = null;
            //if (securityMode == SecurityMode.ViewOnly)
            //{
            //bool entityHasViewSecurity = bizRoleSecurity.EntityHasDirectSecurityForDirectOrIndirect(requester, mainEntity.ID, DataDirectSecurityMode.FetchData);
            //if (entityHasViewSecurity)
            //{
            var securityEntityState = bizRoleSecurity.GetAppliableConditionsBySecuritySubject(requester, mainEntity.ID, DataDirectSecurityMode.FetchData);
            if (securityEntityState != null)
            {
                if (securityEntityState.StateConditions.Count == 0)
                {//چون یعنی دایرکت سکوریتی دارد اما هیچکدام سکوریتی سابجکتشان صدق نمی کند
                    return GetNowRowSearchQuery(requester, mainEntity);
                }
                else if (securityEntityState.StateConditions.Any(x => !x.Values.Any()))
                {
                    //چون یا هست و حداقل یکی فقط سکوریتی سابجکت دارد که صدق میکند
                    return "";
                }
                //   var editSecurityItems = bizRoleSecurity.GetPostEntitySecurityItems(requester, mainEntity.ID, SecurityMode.EditAndView);
                //if (editSecurityItems != null)
                //       viewSecurityItems.PostEntityDataSecurityItems.AddRange(editSecurityItems.PostEntityDataSecurityItems);
                //securityItem = viewSecurityItems;
            }
            else
                return "";
            //}
            //else
            //{
            //    //bool entityHasEditSecurity = securityHelper.EntityHasSecurity(mainEntity.ID, securityMode);
            //    //if (entityHasEditSecurity)
            //    //    securityItems = bizRoleSecurity.GetPostEntitySecurityItems(requester, mainEntity.ID, securityMode);
            //    //else
            //    //{
            //    //    bool entityHasViewSecurity = securityHelper.EntityHasSecurity(mainEntity.ID, SecurityMode.ViewOnly);
            //    //    if (entityHasViewSecurity)
            //    //    {
            //    //        return GetNowRowSearchQuery(requester, mainEntity);
            //    //    }
            //    //    else
            //    //        return "";
            //    //}
            //}



            //////if (securityItems.PostEntityDataSecurityItems.Any(x => x.DirectSecurities.Any(y => y.IgnoreSecurity)))
            //////    return "";


            //mainDataItem.TargetEntityID = ;



            DP_SearchRepositoryMain mainSearchDataItem = null;
            //if (securityItem.InDirectDataSecurity != null)
            //{
            //    //چرا همین که فرستاده میشود در فانکشن پر نمیشود؟   mainDataItem.Phrase
            //    DP_SearchRepository mainDataItem = new DP_SearchRepository(mainEntity.ID);
            //    //mainDataItem.Phrase = new LogicPhrase();
            //    mainSearchDataItem = CreateChildSearchRepository(mainDataItem, securityItem.InDirectDataSecurity.RelationshipTail);

            //    //اینجا تازه کامنت شد
            //    //////if (!mainDataItem.Phrases.Contains(currentSearchRepository.Item1))
            //    //////    mainDataItem.Phrases.Add(currentSearchRepository.Item1);
            //    //////mainSearchDataItem = currentSearchRepository.Item2;
            //}
            //else
            //{
            mainSearchDataItem = new DP_SearchRepositoryMain(mainEntity.ID);
            //}


            mainSearchDataItem.AndOrType = securityEntityState.ConditionOperator;

            LogicPhraseDTO logicPhrase = new LogicPhraseDTO();
            foreach (var condition in securityEntityState.StateConditions)
            {
                if (condition.RelationshipTailID == 0)
                {
                    AddConditionPhrase(requester, condition, logicPhrase);
                }
                else
                {
                    var currentSearchRepository = CreateChildSearchRepository(logicPhrase, condition.RelationshipTail);
                    AddConditionPhrase(requester, condition, currentSearchRepository);
                }
            }
            mainSearchDataItem.Phrases.Add(logicPhrase);

            if (!mainSearchDataItem.Phrases.Any())
            {
                return GetNowRowSearchQuery(requester, mainEntity);
            }
            else
                return GetSearchQuery(requester, mainEntity, mainSearchDataItem).Item1;




        }

        private void AddConditionPhrase(DR_Requester requester, EntityStateConditionDTO conditionDTO, LogicPhraseDTO logicPhrase)
        {
            var searchProperty = new SearchProperty(conditionDTO.Column);
            //     searchProperty.ColumnID = conditionDTO.ColumnID;
            searchProperty.IsKey = conditionDTO.Column.PrimaryKey;
            searchProperty.Name = conditionDTO.Column.Name;
            string value = "";
            //بهینه بشه میشه اصلا اینکه این یا مساوی باشه رو به ساختن کوئری منتقل کرد و اونجا خودش تصمیم بگیره
            if (conditionDTO.Values.Count > 1)
            {
                if (conditionDTO.EntityStateOperator == Enum_EntityStateOperator.Equals)
                    searchProperty.Operator = CommonOperator.InValues;
                else if (conditionDTO.EntityStateOperator == Enum_EntityStateOperator.NotEquals)
                    searchProperty.Operator = CommonOperator.NotInValues;

                foreach (var val in conditionDTO.Values)
                {
                    if (!string.IsNullOrEmpty(val.Value))
                        value += (value == "" ? "" : ",") + val.Value;
                }
            }
            else
            {
                if (conditionDTO.EntityStateOperator == Enum_EntityStateOperator.Equals)
                    searchProperty.Operator = CommonOperator.Equals;
                else if (conditionDTO.EntityStateOperator == Enum_EntityStateOperator.NotEquals)
                    searchProperty.Operator = CommonOperator.Equals;

                foreach (var val in conditionDTO.Values)
                {
                    if (!string.IsNullOrEmpty(val.Value))
                        value = val.Value;
                }
            }


            if (!string.IsNullOrEmpty(value))
            {
                //if (condition.DBFunctionID != 0)
                //{
                //    DatabaseFunctionHandler dbFunctionHandler = new DatabaseFunctionHandler();
                //    var dbFunctionResult = dbFunctionHandler.GetDatabaseFunctionValue(requester, condition.DBFunctionID, value);
                //    value = dbFunctionResult.Result.ToString();
                //}
                searchProperty.Value = value;
                searchProperty.Name = conditionDTO.Column.Name;
                logicPhrase.Phrases.Add(searchProperty);
            }

        }

        private string GetNowRowSearchQuery(DR_Requester requester, TableDrivedEntityDTO entity)
        {
            List<DP_DataRepository> result = new List<DP_DataRepository>();

            var firstTableIndex = 1;
            var firstTableAlias = GetSearchTableAlias(entity, firstTableIndex);
            var keyColumns = dataHelper.GetKeyColumns(entity);
            var entityBaseSelectFromQuery = dataHelper.GetSingleEntityBaseSelectFromQuery(entity, firstTableAlias, keyColumns);
            var split = dataHelper.splitQuery(entityBaseSelectFromQuery);
            var selectfromClause = split.Item1;
            string criteriaClause = split.Item2;
            string searchWhereClause = "1=2";

            string whereClause = "";
            if (criteriaClause != "")
                whereClause += "(" + criteriaClause + ")";
            if (searchWhereClause != "")
                whereClause += (whereClause == "" ? "" : " and ") + "(" + searchWhereClause + ")";
            //فانکشن شود
            var commandStr = selectfromClause + (whereClause == "" ? "" : " where ") + whereClause;

            return commandStr;
        }

        private DP_SearchRepositoryRelationship CreateChildSearchRepository(LogicPhraseDTO parentRepository, EntityRelationshipTailDTO relationshipTail)
        {
            if (relationshipTail != null)
            {
                //LogicPhrase phrase = parentRepository.Phrase as LogicPhrase;
                //if (phrase == null)
                //{
                //    phrase = new LogicPhrase();
                //    parentRepository.Phrase = phrase;
                //}
                var newRepository = parentRepository.Phrases.FirstOrDefault(x => x is DP_SearchRepositoryRelationship && (x as DP_SearchRepositoryRelationship).SourceRelationship.ID == relationshipTail.Relationship.ID) as DP_SearchRepositoryRelationship;
                if (newRepository == null)
                {
                    newRepository = new DP_SearchRepositoryRelationship();
                    //newRepository.TargetEntityID = ;
                    newRepository.SourceRelationship = relationshipTail.Relationship;

                    parentRepository.Phrases.Add(newRepository);
                    //newRepository.SourceToTargetRelationshipType = relationshipTail.Relationship.TypeEnum;
                    //newRepository.SourceToTargetMasterRelationshipType = relationshipTail.Relationship.MastertTypeEnum;
                    // newRepository.Phrase = new LogicPhrase();
                }
                //  if (firstRepository == null)
                //      firstRepository = newRepository;

                if (relationshipTail.ChildTail != null)
                {
                    return CreateChildSearchRepository(newRepository, relationshipTail.ChildTail);
                }
                else
                    return newRepository;
            }
            else
                return null;
        }






        ////private Tuple<bool, bool> HasGeneralAccess(List<OrganizationPostDTO> posts, int entityID, List<DP_SearchRepository> searchDataItems, DR_Requester requester, EntityRelationshipTailDTO sentRelationshipTail = null)
        ////{
        ////    //دسترسی مثل حقوقی چی؟ اگر اپراتور بود اداره پرونده درست باشد اگر کارشناس بود نامش در لیست جدول کارشناسان؟؟؟؟؟
        ////    //bool shouldImposeRoleSecurity = false;
        ////    //bool roleSecurityImposed = false;

        ////    var roleSecurityDirects = bizRoleSecurity.GetEntitySecurityDirects(entityID, true);
        ////    //if (directSecurity.Any())
        ////    //{
        ////    //    shouldImposeRoleSecurity = true;
        ////    //}

        ////    //if (roleSecurities.Any(x => roleIds.Contains(x.RoleID)))
        ////    //{//دسترسی همیشگی
        ////    //    roleSecurityImposed = true;
        ////    //}
        ////    //else
        ////    //{
        ////    DP_SearchRepository finalSearchDataItem = null;
        ////    //var roleSecurityDirects = bizRoleSecurity.GetEntityRoleSecurityDirects(entityID, true);
        ////    if (roleSecurityDirects.Any())
        ////    {
        ////        //shouldImposeRoleSecurity = true;
        ////        if (sentRelationshipTail == null)
        ////        {
        ////            finalSearchDataItem = searchDataItems.FirstOrDefault(x => x.SourceRelatedData == null);
        ////            if (finalSearchDataItem == null)
        ////            {
        ////                finalSearchDataItem = new DP_SearchRepository() { TargetEntityID = entityID };
        ////                searchDataItems.Add(finalSearchDataItem);
        ////            }
        ////        }
        ////        else
        ////        {
        ////            var firstSearchDataItem = searchDataItems.FirstOrDefault(x => x.SourceRelatedData == null);
        ////            if (firstSearchDataItem == null)
        ////            {
        ////                firstSearchDataItem = new DP_SearchRepository() { TargetEntityID = entityID };
        ////                searchDataItems.Add(firstSearchDataItem);
        ////            }


        ////            finalSearchDataItem = SetOrCreateSearchDataItems(searchDataItems, null, sentRelationshipTail, firstSearchDataItem);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        if (sentRelationshipTail != null)
        ////        {
        ////            return new Tuple<bool, bool>(true, false);
        ////        }
        ////        else
        ////        {
        ////            var roleSecurityInDirect = bizRoleSecurity.GetEntitySecurityInDirect(entityID, true);
        ////            if (roleSecurityInDirect != null)
        ////            {
        ////                shouldImposeRoleSecurity = true;
        ////                //var searchDataItem = SetOrCreateSearchDataItems(searchDataItems, searchDataItems.First(x => x.SourceRelatedData == null), roleSecurityInDirect.RelationshipTail);
        ////                var redirectResult = HasGeneralAccess(posts, roleSecurityInDirect.RelationshipTail.TargetEntityID, searchDataItems, requester, roleSecurityInDirect.RelationshipTail);
        ////                roleSecurityImposed = redirectResult.Item2;
        ////            }
        ////        }
        ////    }
        ////    if (roleSecurityDirects.Any() && finalSearchDataItem != null)
        ////    {

        ////        List<SearchProperty> searchProperties = new List<SearchProperty>();
        ////        foreach (var roleSecurityDirect in roleSecurityDirects)
        ////        {
        ////            roleSecurityDirect.SecuritySubject.
        ////            foreach (var roldId in roleIds)
        ////            {
        ////                var securityDirects = roleSecurityDirects.Where(x => x.RoleID == roldId || (x.RoleGroup != null && x.RoleGroup.Roles.Any(y => y.ID == roldId)));

        ////                //if (securityDirects.Any())
        ////                //{
        ////                //    roleSecurityImposed = true;
        ////                //}

        ////                string value = "";
        ////                SearchOperator searchOperator;

        ////                if (roleSecurityDirect.DBFunctionID != 0)
        ////                {
        ////                    DatabaseFunctionHandler dbFunctionHandler = new DatabaseFunctionHandler();
        ////                    var dbFunctionResult = dbFunctionHandler.GetDatabaseFunctionValue(roleSecurityDirect.DBFunctionID, roldId.ToString());
        ////                    value = dbFunctionResult.Result.ToString();
        ////                    if (roleSecurityDirect.Operator == EntitySecurityOperator.Equals)
        ////                        searchOperator = new SearchOperator() { Name = StringOperator.Equals.ToString() };
        ////                    else
        ////                        searchOperator = new SearchOperator() { Name = StringOperator.In.ToString() };
        ////                }
        ////                else
        ////                {
        ////                    searchOperator = new SearchOperator() { Name = StringOperator.Equals.ToString() };
        ////                    value = roleSecurityDirect.Value;
        ////                }

        ////                //var property = finalSearchDataItem.Properties.FirstOrDefault(x => x.ColumnID == roleSecurityDirect.ColumnID);
        ////                //if (property == null)
        ////                //{
        ////                var property = new SearchProperty() { ColumnID = roleSecurityDirect.ColumnID };
        ////                property.Value = value;
        ////                property.Operator = searchOperator;
        ////                property.AndORType = AndORType.Or;
        ////                searchProperties.Add(property);


        ////                //}


        ////            }

        ////        }
        ////        if (searchProperties.Any())
        ////        {
        ////            var searchProperty = new SearchProperty();
        ////            searchProperty.ChildProperties = new Tuple<AndORType, List<SearchProperty>>(AndORType.And, searchProperties);
        ////            finalSearchDataItem.Properties.Add(searchProperty);
        ////            roleSecurityImposed = true;
        ////        }
        ////    }

        ////    //}
        ////    return new Tuple<bool, bool>(shouldImposeRoleSecurity, roleSecurityImposed);



        ////}


        //private bool HasGeneralAccess(List<int> roleIds, int entityID)
        //{

        //}

        //private DP_SearchRepository SetOrCreateSearchDataItems(List<DP_SearchRepository> searchDataItems, DP_SearchRepository currentSearchDataItem, EntityRelationshipTailDTO relationshipTail, DP_SearchRepository parentSearchDataItem = null)
        //{
        //    if (relationshipTail == null)
        //    {
        //        return currentSearchDataItem;
        //    }
        //    else
        //    {
        //        var relatedSearchDataItem = searchDataItems.FirstOrDefault(x => x.SourceRelatedData == currentSearchDataItem && x.SourceRelationID == relationshipTail.RelationshipID);
        //        if (relatedSearchDataItem == null)
        //        {
        //            relatedSearchDataItem = new DP_SearchRepository();
        //            relatedSearchDataItem.SourceRelationID = relationshipTail.RelationshipID;
        //            //relatedSearchDataItem.RelationshipColumns = relationshipTail.RelationshipColumns;
        //            relatedSearchDataItem.SourceRelatedData = parentSearchDataItem;
        //            relatedSearchDataItem.TargetEntityID = relationshipTail.RelationshipTargetEntityID;
        //            relatedSearchDataItem.SourceToTargetRelationshipType = relationshipTail.SourceToTargetRelationshipType;
        //            relatedSearchDataItem.SourceToTargetMasterRelationshipType = relationshipTail.SourceToTargetMasterRelationshipType;
        //            searchDataItems.Add(relatedSearchDataItem);
        //        }
        //        return SetOrCreateSearchDataItems(searchDataItems, relatedSearchDataItem, relationshipTail.ChildTail, currentSearchDataItem);
        //    }

        //}

        private Tuple<string, string> GetSearchQuery(DR_Requester requester, TableDrivedEntityDTO entity, DP_SearchRepositoryMain searchDataItem)
        {
            List<DP_DataRepository> result = new List<DP_DataRepository>();

            //////سلکت جدول اصلی فقط ساخته میشود
            var firstTanleIndex = 1;
            var firstTableAlias = GetSearchTableAlias(entity, firstTanleIndex);
            var keyColumns = dataHelper.GetKeyColumns(entity);
            var entityBaseSelectFromQuery = dataHelper.GetSingleEntityBaseSelectFromQuery(entity, firstTableAlias, keyColumns);
            var split = dataHelper.splitQuery(entityBaseSelectFromQuery);
            var selectfromClause = split.Item1;
            string criteriaClause = split.Item2;
            string searchWhereClause = GetWhereClause(requester, entity, entity, searchDataItem, firstTableAlias, firstTanleIndex);

            string whereClause = "";
            if (criteriaClause != "")
                whereClause += "(" + criteriaClause + ")";
            if (searchWhereClause != "")
                whereClause += (whereClause == "" ? "" : " and ") + "(" + searchWhereClause + ")";
            //فانکشن شود
            var commandStr = selectfromClause + (whereClause == "" ? "" : " where ") + whereClause;

            return new Tuple<string, string>(commandStr, firstTableAlias);

        }

        private string GetWhereClause(DR_Requester requester, TableDrivedEntityDTO entity, TableDrivedEntityDTO connectionEntity, Phrase phrase, string tableAlias, int tableIndex)
        {
            //string where = "";

            if (phrase is SearchProperty)
            {
                return GetPartialWhere((phrase as SearchProperty), entity, tableAlias);
            }
            else if (phrase is DP_SearchRepositoryRelationship)
            {
                var searchRepository = (phrase as DP_SearchRepositoryRelationship);

                //return GetWhereClause(entity, searchRepository, tableAlias, tableIndex);
                //else
                return GetRelatedSearchParams(requester, searchRepository, tableAlias, tableIndex, connectionEntity);

            }
            else if (phrase is LogicPhraseDTO)
            {
                var logicPhrase = (phrase as LogicPhraseDTO);
                var result = "";
                foreach (var internalPhrase in logicPhrase.Phrases)
                {
                    var where = GetWhereClause(requester, entity, connectionEntity, internalPhrase, tableAlias, tableIndex);
                    if (where != "")
                    {
                        result += (result == "" ? "" : (logicPhrase.AndOrType == AndOREqualType.And || logicPhrase.AndOrType == AndOREqualType.NotAnd) ? " and " : " or ") + where;
                    }
                }
                if (result != "")
                {
                    if (logicPhrase.Phrases.Count > 1)
                        result = "(" + result + ")";

                    if (logicPhrase.AndOrType == AndOREqualType.NotAnd ||
                        logicPhrase.AndOrType == AndOREqualType.NotOr)
                        result = "not" + " " + result;

                    return result;
                }
            }
            return "";
        }


        private string GetPartialWhere(SearchProperty property, TableDrivedEntityDTO entity, string searchTableName)
        {
            string where = "";
            if (PropertyHasValue(property))
            {

                if (property.ColumnID != 0)
                {
                    string columnName = property.Name;
                    if (string.IsNullOrEmpty(property.Name))
                        //میشه نام ستون رو در ایتم قرار داد.نه اینجا همه جا
                        columnName = entity.Columns.First(x => x.ID == property.ColumnID).Name;
                    where = GetEquation(searchTableName + "." + columnName, property, property.Value);
                }
                else
                {
                    throw new Exception("asdad");
                }
            }
            return where;
        }

        private string GetEquation(string columnName, SearchProperty property, object value)
        {
            if (property.Operator == CommonOperator.Equals ||
                property.Operator == CommonOperator.NotEquals ||
                property.Operator == CommonOperator.BiggerThan ||
                property.Operator == CommonOperator.SmallerThan)
            {
                return columnName + GetStringOperaor(property) + "'" + property.Value + "'";
            }
            else if (property.Operator == CommonOperator.Contains ||
                    property.Operator == CommonOperator.StartsWith ||
                      property.Operator == CommonOperator.EndsWith)
            {
                if (property.Operator == CommonOperator.Contains)
                    return columnName + " like " + "N'%" + property.Value + "%'";
                else if (property.Operator == CommonOperator.StartsWith)
                    return columnName + " like " + "N'" + property.Value + "%'";
                else if (property.Operator == CommonOperator.EndsWith)
                    return columnName + " like " + "N'%" + property.Value + "'";
            }
            else if (property.Operator == CommonOperator.InValues || property.Operator == CommonOperator.NotInValues)
            {
                if (property.Operator == CommonOperator.InValues)
                    return columnName + " in " + GetInValuseRange(property);
                else if (property.Operator == CommonOperator.NotInValues)
                    return columnName + " not in " + GetInValuseRange(property);
            }
            return columnName + "=" + "'" + property.Value + "'";

        }

        private string GetInValuseRange(SearchProperty property)
        {
            string result = "";
            if (property.Value != null && property.Value.ToString().Contains(","))
            {
                var split = property.Value.ToString().Split(',');
                foreach (var splt in split)
                {
                    result += (result == "" ? "" : ",") + "'" + splt + "'";
                }
            }
            else
            {
                result = "'" + property.Value + "'";
            }
            if (result != "")
                result = "(" + result + ")";
            return result;
        }

        private string GetStringOperaor(SearchProperty property)
        {
            if (property.Operator == CommonOperator.Equals)
                return "=";
            else if (property.Operator == CommonOperator.BiggerThan)
                return ">";
            else if (property.Operator == CommonOperator.SmallerThan)
                return "<";
            else if (property.Operator == CommonOperator.NotEquals)
                return "<>";
            return "=";

        }

        private string GetRelatedSearchParams(DR_Requester requester, DP_SearchRepositoryRelationship dataItem, string parentSearchTableAlias, int parentSearchTableIndex, TableDrivedEntityDTO connectionEntity)
        {
            if (dataItem.SourceRelationship == null)
                return "";
            var entity = bizTableDrivedEntity.GetSimpleEntityWithColumns(requester, dataItem.SourceRelationship.EntityID2);


            var currentTableIndex = parentSearchTableIndex + 1;
            var whereClause = "";
            var currentTableAlias = GetSearchTableAlias(entity, currentTableIndex);

            var relationship = dataItem.SourceRelationship;

            var select = "";
            var selectColumn = "";
            if ((dataItem.RelationshipFromCount != 0 && dataItem.RelationshipFromCount != null) || (dataItem.RelationshipToCount != 0 && dataItem.RelationshipToCount != null))
                selectColumn = "count(1)";
            else
                selectColumn = "1";
            select = dataHelper.GetRelationEntityBaseSelectFromQuery(connectionEntity, entity, currentTableAlias, selectColumn);

            var split = dataHelper.splitQuery(select);
            var selectfromClause = split.Item1;
            string criteriawhereClause = split.Item2;

            var joinWhereClause = dataHelper.GetOnClause(parentSearchTableAlias, currentTableAlias, relationship);

            //joinWhereClause += (partialWhere == "" ? "" : (" and " + partialWhere));

            var finalInnerWhereCluase = "";
            if (criteriawhereClause != "")
                finalInnerWhereCluase = "(" + criteriawhereClause + ")";
            if (joinWhereClause != "")
                finalInnerWhereCluase += (finalInnerWhereCluase == "" ? "" : " and ") + "(" + joinWhereClause + ")";

            var childPhrasesResult = "";
            foreach (var childPhrase in dataItem.Phrases)
            {
                var phraseWhere = GetWhereClause(requester, entity, connectionEntity, childPhrase, currentTableAlias, currentTableIndex);
                if (phraseWhere != "")
                    childPhrasesResult += (childPhrasesResult == "" ? "" : ((dataItem.AndOrType == AndOREqualType.And || dataItem.AndOrType == AndOREqualType.NotAnd) ? " and " : " or ")) + phraseWhere;
            }
            if (childPhrasesResult != "")
            {
                if (dataItem.AndOrType == AndOREqualType.NotAnd ||
                     dataItem.AndOrType == AndOREqualType.NotOr)
                    childPhrasesResult = "not" + " (" + childPhrasesResult + ")";

                finalInnerWhereCluase += (finalInnerWhereCluase == "" ? "" : " and ") + "(" + childPhrasesResult + ")";
            }


            var selectfull = "(" + selectfromClause + (finalInnerWhereCluase == "" ? "" : " where ") + finalInnerWhereCluase + ")";

            if ((dataItem.RelationshipFromCount != 0 && dataItem.RelationshipFromCount != null) || (dataItem.RelationshipToCount != 0 && dataItem.RelationshipToCount != null))
            {


                //var finalCountQuery = 
                if (dataItem.RelationshipFromCount != 0 && dataItem.RelationshipToCount != 0)
                    whereClause = selectfull + " between " + dataItem.RelationshipFromCount + " and " + dataItem.RelationshipToCount;
                else if (dataItem.RelationshipFromCount != 0)
                    whereClause = selectfull + ">=" + dataItem.RelationshipFromCount;
                else if (dataItem.RelationshipToCount != 0)
                    whereClause = selectfull + "<=" + dataItem.RelationshipToCount;
            }
            else
            {

                //whereClause = dataItem.HasNotRelationshipCheck ? "not " : "" + "exists (" + selectfromClause + childInnerjoin + (joinWhereClause == "" ? "" : " where ") + joinWhereClause + ")";
                whereClause = dataItem.HasNotRelationshipCheck == true ? "not" : "" + " exists " + selectfull;
            }




            return whereClause;



        }





        private List<Tuple<string, string, string, string>> GetSelectColumn(DR_Requester requester, string parentTableName, int parentSearchTableIndex, EntityRelationshipTailDTO relationshipTail, TableDrivedEntityDTO connectionEntity, List<Tuple<string, string, string, string>> result = null)
        {
            if (result == null)
                result = new List<Tuple<string, string, string, string>>();
            //string resultInnerjoin = "";

            var entity = bizTableDrivedEntity.GetSimpleEntity(requester, relationshipTail.Relationship.EntityID2);


            int currentTableIndex = parentSearchTableIndex + 1;
            var currentTableName = GetSearchTableAlias(entity, currentTableIndex);
            //string innerjoin = "";
            //var select = "";

            //////if (parentRelationship.TypeEnum != Enum_RelationshipType.OneToMany)
            //////{

            var innerjoin = dataHelper.GetInnerjoinOnClause(parentTableName, currentTableName, relationshipTail.Relationship, connectionEntity, entity);
            result.Add(new Tuple<string, string, string, string>(parentTableName, currentTableName, innerjoin.Item1, innerjoin.Item2));
            if (relationshipTail.ChildTail == null)
                return result;
            else
            {
                return GetSelectColumn(requester, currentTableName, currentTableIndex, relationshipTail.ChildTail, connectionEntity, result);
                //var childResult = GetSelectColumn(requester, currentTableName, currentTableIndex, relationshipTail.ChildTail, entity);
                ////  if(childResult.Item2=="")
                //return new Tuple<string, string>(innerjoin + childResult.Item1, childResult.Item2);
            }

            //foreach (var column in entity.Columns)//.Where(x => x.ViewEnabled))
            //{
            //    //if (hasOneToManyParents)
            //    //    select += (select == "" ? "" : "+") + "'" + column.Alias + "'" + "+" + "' '" + "+" + "GROUP_CONCAT(" + currentTableName + "." + column.Name + ")";
            //    ////select += (select == "" ? "" : ",") + searchTableName + "." + column.Name + " as " + column.Alias;
            //    //else
            //    select += (select == "" ? "" : ",") + currentTableName + "." + column.Name + " as " + parentRelatoinshipIds + "_" + column.ID;
            //}

            //////foreach (var relationship in entity.Relationships.Where(x => x.ViewEnabled == true))
            //////{

            //////    var childResult = GetViewRelatedSelectParams(currentTableName, currentTableIndex, relationship, entity, parentRelatoinshipIds + "_" + relationship.ID, hasOneToManyParents);
            //////    select += (select == "" ? "" : ",") + childResult.Item2;
            //////    innerjoin += " " + childResult.Item1;

            //////}

            //////    return new Tuple<string, string>(innerjoin, select);
            //////}
            //////else
            //////{
            //////    var childInnerjoin = "";
            //////    var childSelect = "";

            //////    //////foreach (var relationship in entity.Relationships.Where(x => x.ViewEnabled == true))
            //////    //////{
            //////    //////    var childResult = GetViewRelatedSelectParams(currentTableName, currentTableIndex, relationship, entity, parentRelatoinshipIds + "_" + relationship.ID, true);
            //////    //////    childSelect += (childSelect == "" ? "" : ",") + childResult.Item2;
            //////    //////    childInnerjoin += " " + childResult.Item1;
            //////    //////}
            //////    var joinWhereClause = dataHelper.GetOnClause(parentTableName, currentTableName, parentRelationship);

            //////    var selectCount = "'تعداد' + ' : '+ " + "ltrim(str(count(*)))";
            //////    var selectMany = "";
            //////    foreach (var column in entity.Columns)//.Where(x => x.ViewEnabled))
            //////    {
            //////        selectMany += (selectMany == "" ? "'" : "+' / ") + column.Alias + "'" + "+ ' : ' +" + "dbo.GROUP_CONCAT(" + currentTableName + "." + column.Name + ")";// + " as " + parentRelatoinshipIds + "#" + entity.ID + "_" + column.ID;
            //////    }
            //////    selectMany = selectCount + (selectMany == "" ? "" : "+ ' / '+") + selectMany;
            //////    selectMany += (childSelect == "" ? "" : "+" + childSelect);

            //////    var entityBaseSelectFromQuery = dataHelper.GetRelationEntityBaseSelectFromQuery(connectionEntity, entity, currentTableName, selectMany);
            //////    var split = dataHelper.splitQuery(entityBaseSelectFromQuery);
            //////    var selectfromClause = split.Item1;
            //////    string criteriawhereClause = split.Item2;
            //////    joinWhereClause += (criteriawhereClause == "" ? "" : " and " + criteriawhereClause);

            //////    select += "(" + selectfromClause + " where " + joinWhereClause + ")" + " as " + parentRelatoinshipIds;
            //////    //var relatedSeachParam = GetRelatedSearchParams(searchDataItems, innerjoin, whereClause, searchTableName, 0, entity, mainSearchDataItem, dataItem);

            //////    //innerjoin خالیه تو این حالت
            //////}



            //return innerjoin;
        }
        private string GetSearchTableAlias(TableDrivedEntityDTO entity, int v)
        {
            if (v != 0)
                return entity.TableName + "_" + v;
            else
                return entity.TableName;
        }
        private bool PropertyHasValue(SearchProperty item)
        {
            return (item.Value != null && !string.IsNullOrEmpty(item.Value.ToString()) && (item.NotIgnoreZeroValue || item.Value.ToString() != "0"));
        }
        private List<DP_DataRepository> DataTableToDP_DataRepository(TableDrivedEntityDTO entity
            , DataTable dataTable, EntityListViewDTO listView
            , DataTable dataTableForDataView, EntityListViewDTO listViewForDataView)
        {
            //var mainEntity = GetTableDrivedEntity2( entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            List<Tuple<short, ColumnDTO>> listIndexColumns = new List<Tuple<short, ColumnDTO>>();

            for (short i = 0; i < dataTable.Columns.Count; i++)
            {
                var tailWithColumnName = dataTable.Columns[i].ColumnName;
                //var splt = tailWithColumnName.Split('>');
                //int tailID = Convert.ToInt32(splt[0]);
                //var columnName = splt[1];

                var column = listView.EntityListViewAllColumns.FirstOrDefault(x => x.RelativeColumnName == tailWithColumnName);
                if (column != null)
                {
                    listIndexColumns.Add(new Tuple<short, ColumnDTO>(i, column.Column));
                }
            }

            List<DP_DataView> listDataView = null;
            if (dataTableForDataView != null)
                listDataView = DataTableToDP_ViewRepository(entity, dataTableForDataView, listViewForDataView);

            List<DP_DataRepository> result = new List<DP_DataRepository>();
            //  BizFormulaUsage bizFormulaUsage = new BizFormulaUsage();
            foreach (DataRow row in dataTable.Rows)
            {
                List<EntityInstanceProperty> properties = new List<EntityInstanceProperty>();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    if (listIndexColumns.Any(x => x.Item1 == i))
                    {
                        var column = listIndexColumns.First(x => x.Item1 == i).Item2;
                        object value = null;
                        if (row[i] != DBNull.Value)
                            value = row[i];//.ToString();
                        else
                            value = null;
                        var property = new EntityInstanceProperty(column, value);
                        //property.Name = column.Name;
                        properties.Add(property);
                    }
                }


                DP_DataView dataView = null;
                if (listDataView != null)
                {
                    dataView = listDataView.FirstOrDefault(x => DataItemsAreEqual(x.KeyProperties, properties.Where(y => y.IsKey).ToList()));
                }
                var data = new DP_DataRepository(dataView);
                data.SetProperties(properties);
                data.IsFullData = true;
                //    bizFormulaUsage.CheckFormulaUsages(data);
                result.Add(data);
            }
            return result;
        }

        internal static bool DataItemsAreEqual(List<EntityInstanceProperty> keyProperties1, List<EntityInstanceProperty> keyProperties2)
        {
            if (keyProperties1.Any() && keyProperties2.Any())
            {

                if (keyProperties1.All(x => keyProperties2.Any(y => x.ColumnID == y.ColumnID && x.Value.Equals(y.Value)))
                    && keyProperties2.All(x => keyProperties1.Any(y => x.ColumnID == y.ColumnID && x.Value.Equals(y.Value))))
                    return true;

            }
            return false;
        }

        //private DP_DataRepository ToDataRepository(TableDrivedEntityDTO entity, DataRow reader
        //    , List<Tuple<short, ColumnDTO>> listColumns, int listViewID, List<DataViewProperty> dataViewProperties)
        //{


        //    return result;
        //}

        private List<DP_DataView> DataTableToDP_ViewRepository(TableDrivedEntityDTO entity, DataTable dataTable, EntityListViewDTO listView)
        {
            List<DP_DataView> result = new List<DP_DataView>();
            List<Tuple<short, EntityListViewColumnsDTO>> listIndexColumns = new List<Tuple<short, EntityListViewColumnsDTO>>();
            for (short i = 0; i < dataTable.Columns.Count; i++)
            {

                var tailWithColumnName = dataTable.Columns[i].ColumnName;

                var column = listView.EntityListViewAllColumns.FirstOrDefault(x => x.RelativeColumnName == tailWithColumnName);
                if (column != null)
                    listIndexColumns.Add(new Tuple<short, EntityListViewColumnsDTO>(i, column));

            }
            foreach (DataRow row in dataTable.Rows)
            {
                List<DataViewProperty> dataViewProperties = new List<DataViewProperty>();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    if (listIndexColumns.Any(x => x.Item1 == i))
                    {
                        var listColumnsItem = listIndexColumns.First(x => x.Item1 == i);
                        var column = listColumnsItem.Item2;
                        object value;
                        if (row[i] != DBNull.Value)
                            value = row[i];//.ToString();
                        else
                            value = null;
                        var property = new DataViewProperty(column);
                        property.Value = value;
                        dataViewProperties.Add(property);
                    }
                }
                var item = new DP_DataView(entity.ID, entity.Alias, listView.ID, dataViewProperties);
                foreach (var dvColumn in item.DataViewProperties)
                {
                    if (dvColumn.Column.PrimaryKey && string.IsNullOrEmpty(dvColumn.RelationshipIDTailPath))
                        item.Properties.Add(new EntityInstanceProperty(dvColumn.Column, dvColumn.Value));
                }
                result.Add(item);
            }
            return result;
        }



        //private DP_DataView ToViewRepository(TableDrivedEntityDTO entity, DataRow reader, List<Tuple<short, EntityListViewColumnsDTO>> listColumns)
        //{


        //    return result;
        //}

        //private DP_DataViewItem GetViewItem(DP_DataView result, int tailID)
        //{
        //    var viewItem = result.DataViewItems.FirstOrDefault(x => x.RelationshipTailID == tailID);
        //    if (viewItem == null)
        //    {
        //        viewItem = new DP_DataViewItem() { RelationshipTailID = tailID };
        //        result.DataViewItems.Add(viewItem);
        //    }
        //    return viewItem;
        //}
        public DR_ResultSearchKeysOnly Process(DR_SearchKeysOnlyRequest request)
        {
            DR_ResultSearchKeysOnly result = new DR_ResultSearchKeysOnly();
            try
            {
                //  BizEntityListView bizEntityListView = new BizEntityListView();
                //   var listView = bizEntityListView.GetEntityKeysListView(request.Requester, request.SearchDataItem.TargetEntityID);
                BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(request.Requester, request.SearchDataItem.TargetEntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
                var listColumns = entityDTO.Columns.Where(x => x.PrimaryKey).ToList(); ;
                var listView = GetListViewFromColumns(listColumns);
                var dataTable = GetDataTableBySearchDataItems(request.Requester, request.SearchDataItem.TargetEntityID, request.SearchDataItem, listView.EntityListViewAllColumns);
                result.ResultDataItems = DataTableToDP_ViewRepository(dataTable.Item1, dataTable.Item2, listView);
                result.Result = Enum_DR_ResultType.SeccessfullyDone;
            }
            catch (Exception ex)
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                result.Message = "خطا در جستجو" + Environment.NewLine + ex.Message;
            }
            return result;
        }
        public DR_ResultSearchFullData Process(DR_SearchFullDataRequest request)
        {
            DR_ResultSearchFullData result = new DR_ResultSearchFullData();
            try
            {
                result.ResultDataItems = GetFullDataResult(request.Requester, request.SearchDataItem);
                result.Result = Enum_DR_ResultType.SeccessfullyDone;
            }
            catch (Exception ex)
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                result.Message = "خطا در جستجو" + Environment.NewLine + ex.Message;
            }
            return result;
        }
        public DR_ResultSearchFullData ProcessSearchEditRequest(DR_SearchEditRequest request)
        {
            // SearchRequestProcessor.ProcessSearchEditRequest: f5cf8a2212bd
            DR_ResultSearchFullData result = new DR_ResultSearchFullData();
            try
            {
                result.ResultDataItems = GetFullDataResult(request.Requester, request.SearchDataItem);
                DoBeforeLoadBackendActionActivities(request, result);
                DoBeforeLoadUIActionActivities(request, result);
            }
            catch (Exception ex)
            {
                result.Result = Enum_DR_ResultType.ExceptionThrown;
                result.Message = "خطا در جستجو" + Environment.NewLine + ex.Message;
            }
            return result;
        }

        private void DoBeforeLoadUIActionActivities(DR_SearchEditRequest request, DR_ResultSearchFullData result)
        {
            if (result.ResultDataItems.Any())
            {
                BizEntityState bizEntityState = new BizEntityState();
                bizEntityState.DoBeforeLoadUIActionActivities(request.Requester, result.ResultDataItems);
            }
        }

        private List<DP_DataRepository> GetFullDataResult(DR_Requester requester, DP_SearchRepositoryMain searchDataItem)
        {
            BizEntityListView bizEntityListView = new BizEntityListView();

            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(requester, searchDataItem.TargetEntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            var listView = GetListViewFromColumns(entityDTO.Columns);
            //  var editListView = bizEntityListView.GetEntityListViewWithAllColumns(requester, searchDataItem.TargetEntityID);
            var dataTable = GetDataTableBySearchDataItems(requester, searchDataItem.TargetEntityID, searchDataItem, listView.EntityListViewAllColumns);

            DataTable dataviewDataTable = null;
            EntityListViewDTO dataviewListView = null;
            //if (withDataView)
            //{
            //فعلا همیشه بیاد
            dataviewListView = bizEntityListView.GetOrCreateEntityListViewDTO(requester, searchDataItem.TargetEntityID);
            dataviewDataTable = GetDataTableBySearchDataItems(requester, searchDataItem.TargetEntityID, searchDataItem, dataviewListView.EntityListViewAllColumns).Item2;
            // }
            return DataTableToDP_DataRepository(dataTable.Item1, dataTable.Item2, listView, dataviewDataTable, dataviewListView);
        }

        public void DoBeforeLoadBackendActionActivities(DR_SearchEditRequest request, DR_ResultSearchFullData result)
        {
            //بازدارنده بودن اقدام کنترل شود
            BizBackendActionActivity bizActionActivity = new BizBackendActionActivity();
            var actionActivities = bizActionActivity.GetBackendActionActivities(request.SearchDataItem.TargetEntityID, new List<Enum_EntityActionActivityStep>() { Enum_EntityActionActivityStep.BeforeLoad }, true, true);
            CodeFunctionHandler codeFunctionHelper = new CodeFunctionHandler();
            foreach (var entityActionActivity in actionActivities)
            {
                if (entityActionActivity.CodeFunctionID != 0)
                {
                    var resultFunction = codeFunctionHelper.GetCodeFunctionResult(request.Requester, entityActionActivity.CodeFunctionID, result.ResultDataItems);
                    if (resultFunction.Exception == null)
                    {

                    }
                    else
                    {
                        result.Result = Enum_DR_ResultType.ExceptionThrown;
                        result.Message += (string.IsNullOrEmpty(result.Message) ? "" : Environment.NewLine) + resultFunction.Exception.Message;
                    }
                }
            }
        }

        //private List<DP_DataRepository> GetDataItemsByListOFSearchProperties(DR_Requester requester, DP_SearchRepository searchDataItem)
        //{


        //    return 
        //}

        //private DP_SearchRepository GetSearchDataItem(int entityID, List<EntityInstanceProperty> properties)
        //{
        //    DP_SearchRepository result = new DP_SearchRepository();
        //    result.TargetEntityID = entityID;
        //    result.Properties = properties;
        //    return result;
        //}


        //private DataTable GetDataTableByListOFSearchProperties(TableDrivedEntityDTO entity, List<List<EntityInstanceProperty>> listProperties)
        //{



        //    //string tableName = bizTableDrivedEntity.GetTableName(entityID);
        //    //var ConnectionString = ConnectoinHelper.GetConnectionString(entityID);
        //    //using (SqlConnection testConn = new SqlConnection(ConnectionString))
        //    //{
        //    //    testConn.Open();
        //    string commandStr = "select top 500 * from " + entity.TableName;
        //    string criteria = entity.Criteria;
        //    string whereClause = "";
        //    foreach (var listItem in listProperties)
        //    {
        //        string where = "";
        //        foreach (var item in listItem)
        //        {
        //            var column = entity.Columns.Where(x => x.ID == item.ColumnID).FirstOrDefault();
        //            if (column != null)
        //                if (!string.IsNullOrEmpty(item.Value) && item.Value.ToLower() != "<null>" && item.Value.ToLower() != "0")
        //                {
        //                    where += (where == "" ? "" : " and ") + column.Name + "='" + item.Value + "'";
        //                }
        //        }
        //        if (where != "")
        //            where = "(" + where + ")";
        //        whereClause += (whereClause == "" ? "" : " or ") + where;
        //    }
        //    if (whereClause != "")
        //        whereClause = "(" + whereClause + ")";
        //    string finalWhereClause = criteria + (string.IsNullOrEmpty(criteria) ? "" : " and ") + whereClause;
        //    commandStr += (finalWhereClause == "" ? "" : " where ") + finalWhereClause;

        //    //List<ColumnDTO> columns = null;
        //    //if (targetColumnIds != null && targetColumnIds.Count > 0)
        //    //    columns = entity.Columns.Where(x => targetColumnIds.Contains(x.ID)).ToList();
        //    //else
        //    //    columns = entity.Columns;


        //    //using (SqlCommand command = new SqlCommand(commandStr, testConn))
        //    //{
        //    //    using (SqlDataReader reader = command.ExecuteReader())
        //    //    {
        //    //        while (reader.Read())
        //    //        {
        //    return ConnectionManager.GetDBHelperByEntityID(entity.ID).ExecuteQuery(commandStr);

        //    //        }
        //    //    }
        //    //}







        //    //}
        //    //return result;
        //}









        //public DR_ResultSearchViewByRelatinoshipTail Process(DR_SearchViewByRelationshipTailRequest request)
        //{
        //    DR_ResultSearchViewByRelatinoshipTail result = new DR_ResultSearchViewByRelatinoshipTail();
        //    RelationshipTailDataManager relationshipTailDataManager = new RelationshipTailDataManager();
        //    var searchDataItem = relationshipTailDataManager.GetTargetSearchItemFromRelationshipTail(request.FirstDataItem, request.RelationshipTail);
        //    //یه نکته بعدا چک شود..باید در طول مسیر مقادیر کلید نیز در سرچ دیتا آیتمها قرار بگیرند
        //    //بدون آنها ممکن است اطلاعات مشابه نیز وجود داشته باشند

        //    var dataTable = GetDataTableBySearchDataItems(request.Requester, searchDataItem.TargetEntityID, searchDataItem, null, request.EntityViewID);
        //    result.ResultDataItems = DataTableToDP_ViewRepository(dataTable.Item1, dataTable.Item2, dataTable.Item3);
        //    result.Result = Enum_DR_ResultType.SeccessfullyDone;
        //    return result;
        //}



        //public DR_ResultSearchEdit Process(DR_SearchByRelationViewRequest request)
        //{
        //    DR_ResultSearchEdit result = new DR_ResultSearchEdit();
        //    var entity = bizTableDrivedEntity.GetTableDrivedEntity(request.EntityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
        //    var dataTable = GetDataItems(entity, request.Properties);
        //    result.ResultDataItems = DataTableToDP_DataRepository(entity, dataTable);
        //    result.Result = Enum_DR_ResultType.SeccessfullyDone;
        //    return result;
        //}







        //private List<DP_DataRepository> DataTableToDP_DataRepository(TableDrivedEntityDTO entity, DataTable dataTable)
        //{
        //    List<DP_DataRepository> result = new List<DP_DataRepository>();
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        result.Add(ToBaseDataRepository(entity, row));
        //    }
        //    return result;
        //}




        //private string GetTableName(TableDrivedEntity entity)
        //{
        //    if (entity != null)
        //        return (string.IsNullOrEmpty(entity.Table.RelatedSchema) ? "" : entity.Table.RelatedSchema + ".") + entity.Table.Name;
        //    else
        //        return entity.Name;
        //}




        //private DP_DataRepository ToBaseDataRepository(TableDrivedEntityDTO entity, DataRow reader)
        //{

        //    var result = new DP_DataRepository();
        //    result.TargetEntityID = entity.ID;
        //    for (int i = 0; i < reader.Table.Columns.Count; i++)
        //    {
        //        string columnName = reader.Table.Columns[i].ColumnName;
        //        //foreach (var item in result.TypeOrTypeConditions.Select(x => x.Type))
        //        //{
        //        foreach (var itemProperty in entity.Columns)
        //        {
        //            if (itemProperty.Name.ToLower() == columnName.ToLower())
        //            {

        //                var property = new EntityInstanceProperty();
        //                property.IsKey = itemProperty.PrimaryKey;
        //                property.ColumnID = itemProperty.ID;
        //                if (reader[i] != DBNull.Value)
        //                    property.Value = reader[i].ToString();
        //                else
        //                    property.Value = "<Null>";
        //                result.Properties.Add(property);
        //            }
        //        }
        //        //}
        //    }
        //    return result;
        //}

        //public static List<DataAccess.Column> GetColumnList(TableDrivedEntity template)
        //{
        //    if (template.Column == null || template.Column.Count == 0)
        //    {
        //        return template.Table.Column.ToList();
        //    }
        //    else
        //        return template.Column.ToList();
        //}
        //private DP_DataRepository ToDataRepository(SqlDataReader reader)
        //{
        //    //    DataManager.DataPackage.DP_Package result = new DataManager.DataPackage.DP_Package();
        //    //var result = CommonBusiness.BizHelper.Clone<DataManager.DataPackage.DP_Package>(dP_Package);var result

        //}
    }

}

