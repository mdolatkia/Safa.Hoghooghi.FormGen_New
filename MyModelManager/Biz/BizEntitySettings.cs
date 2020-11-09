using DataAccess;
using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizEntitySettings
    {
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizEntityListView bizEntityListView = new BizEntityListView();
        BizEntitySearch bizEntitySearch = new BizEntitySearch();
        BizEntityUIComposition bizEntityUIComposition = new BizEntityUIComposition();
        public event EventHandler<ItemImportingStartedArg> ItemImportingStarted;
        private void BizTableDrivedEntity_ItemImportingStarted(object sender, ItemImportingStartedArg e)
        {
            if (ItemImportingStarted != null)
                ItemImportingStarted(this, e);
        }
        public void UpdateDefaultSettingsInModel(DR_Requester requester, List<int> entityIDs)
        {
            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            bizTableDrivedEntity.ItemImportingStarted += BizTableDrivedEntity_ItemImportingStarted;
            List<TableDrivedEntityDTO> listEntities = new List<TableDrivedEntityDTO>();
            foreach (var id in entityIDs)
            {
                listEntities.Add(bizTableDrivedEntity.GetTableDrivedEntity(requester, id, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships));
            }
            var allEntities = bizTableDrivedEntity.GetOrginalEntities(listEntities.First().DatabaseID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships, false);

            UpdateDefaultSettingsInModel(listEntities, listEntities, listEntities, listEntities, allEntities);
        }
        public void UpdateDefaultSettingsInModel(DR_Requester requester, int databaseID)
        {
            //try
            //{
            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            bizTableDrivedEntity.ItemImportingStarted += BizTableDrivedEntity_ItemImportingStarted;

            List<TableDrivedEntityDTO> allEntities = new List<TableDrivedEntityDTO>();
            var listEntityIds = bizTableDrivedEntity.GetEntityIDs(databaseID, false);
            foreach (var id in listEntityIds)
            {
                allEntities.Add(bizTableDrivedEntity.GetTableDrivedEntity(requester, id, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships));
            }

            var uiCompositionEntities = bizTableDrivedEntity.GetOrginalEntitiesWithoutUIComposition(databaseID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships, false);
            var listViewEntities = allEntities.Where(x => x.EntityListViewID == 0).ToList();

            var searchEntities = allEntities.Where(x => x.EntitySearchID == 0).ToList();

            var initialSearchEntities = allEntities.Where(x => x.SearchInitially == null).ToList();

            UpdateDefaultSettingsInModel(uiCompositionEntities, listViewEntities, searchEntities, initialSearchEntities, allEntities);
        }
        public void UpdateDefaultSettingsInModel(List<TableDrivedEntityDTO> uiCompositionEntities, List<TableDrivedEntityDTO> listViewEntities, List<TableDrivedEntityDTO> searchEntities, List<TableDrivedEntityDTO> initialSearchEntities, List<TableDrivedEntityDTO> allEntities)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                List<Tuple<TableDrivedEntityDTO, List<EntityUICompositionDTO>>> listEntityUIComposition = new List<Tuple<TableDrivedEntityDTO, List<EntityUICompositionDTO>>>();

                foreach (var entity in uiCompositionEntities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = uiCompositionEntities.Count(), CurrentProgress = uiCompositionEntities.IndexOf(entity) + 1 });
                    var uiComposition = bizEntityUIComposition.GenerateUIComposition(entity);
                    listEntityUIComposition.Add(new Tuple<TableDrivedEntityDTO, List<EntityUICompositionDTO>>(entity, uiComposition));

                }
                foreach (var item in listEntityUIComposition)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Setting UI for entity" + " " + item.Item1.Name, TotalProgressCount = listEntityUIComposition.Count(), CurrentProgress = listEntityUIComposition.IndexOf(item) + 1 });
                    bizEntityUIComposition.SaveItem(projectContext, item.Item1.ID, item.Item2);
                }

                List<Tuple<TableDrivedEntityDTO, EntityListViewDTO>> listEntityAndView = new List<Tuple<TableDrivedEntityDTO, EntityListViewDTO>>();
                foreach (var entity in listViewEntities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = listViewEntities.Count(), CurrentProgress = listViewEntities.IndexOf(entity) + 1 });
                    var viewItem = bizEntityListView.GenerateDefaultListView(entity, allEntities);
                    listEntityAndView.Add(new Tuple<TableDrivedEntityDTO, EntityListViewDTO>(entity, viewItem));
                }


                List<EntityRelationshipTail> createdRelationshipTails = new List<EntityRelationshipTail>();
                foreach (var item in listEntityAndView)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Setting default view for entity" + " " + item.Item1.Name, TotalProgressCount = listEntityAndView.Count(), CurrentProgress = listEntityAndView.IndexOf(item) + 1 });
                    var dbListView = bizEntityListView.SaveItem(projectContext, item.Item2, createdRelationshipTails);
                    var dbentity = projectContext.TableDrivedEntity.First(x => x.ID == item.Item1.ID);
                    dbentity.EntityListView1 = dbListView;
                }


                List<Tuple<TableDrivedEntityDTO, EntitySearchDTO>> listEntityAndSearch = new List<Tuple<TableDrivedEntityDTO, EntitySearchDTO>>();
                foreach (var entity in searchEntities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = searchEntities.Count(), CurrentProgress = searchEntities.IndexOf(entity) + 1 });
                    var searchItem = bizEntitySearch.GenerateDefaultSearchList(entity, allEntities);
                    listEntityAndSearch.Add(new Tuple<TableDrivedEntityDTO, EntitySearchDTO>(entity, searchItem));
                }

                foreach (var item in listEntityAndSearch)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Setting default search for entity" + " " + item.Item1.Name, TotalProgressCount = listEntityAndSearch.Count(), CurrentProgress = listEntityAndSearch.IndexOf(item) + 1 });
                    var dbSearch = bizEntitySearch.SaveItem(projectContext, item.Item2, createdRelationshipTails);
                    var dbentity = projectContext.TableDrivedEntity.First(x => x.ID == item.Item1.ID);
                    dbentity.EntitySearch = dbSearch;
                }


                List<Tuple<TableDrivedEntityDTO, bool>> listEntityAndInitialSearch = new List<Tuple<TableDrivedEntityDTO, bool>>();
                foreach (var entity in initialSearchEntities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = searchEntities.Count(), CurrentProgress = initialSearchEntities.IndexOf(entity) + 1 });
                    bool initiallySearched = bizTableDrivedEntity.DecideEntityIsInitialySearched(entity, allEntities);
                    listEntityAndInitialSearch.Add(new Tuple<TableDrivedEntityDTO, bool>(entity, initiallySearched));
                }
                foreach (var item in listEntityAndInitialSearch)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Setting initially searched entity" + " " + item.Item1.Name, TotalProgressCount = listEntityAndInitialSearch.Count(), CurrentProgress = listEntityAndInitialSearch.IndexOf(item) + 1 });
                    bizTableDrivedEntity.UpdateEntityInitiallySearch(projectContext, item.Item1.ID, item.Item2);
                }


                if (ItemImportingStarted != null)
                    ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Saving changes" });
                projectContext.SaveChanges();

            }
        }

        public void SetCustomSettings(int databaseID)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var dbDatabase = projectContext.DatabaseInformation.FirstOrDefault(x => x.ID == databaseID);
                var realPerson = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "RealPerson" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (realPerson != null)
                {

                }
                var legalPerson = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "LegalPerson" && x.Table.DBSchema.DatabaseInformationID == databaseID);

                var genericPerson = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "GenericPerson" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (genericPerson != null)
                {
                    var emailColumn = genericPerson.Table.Column.FirstOrDefault(x => x.Name == "EmailAddress");
                    if (emailColumn != null)
                        emailColumn.StringColumnType.Format = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

                    var typeColumn = genericPerson.Table.Column.FirstOrDefault(x => x.Name == "Type");
                    if (typeColumn != null)
                    {
                        if (typeColumn.ColumnValueRange == null)
                        {
                            typeColumn.ColumnValueRange = new ColumnValueRange();
                            typeColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "1", KeyTitle = "حقیقی" });
                            typeColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "2", KeyTitle = "حقوقی" });
                        }
                    }
                    if (realPerson != null)
                    {
                        var relaitonship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == genericPerson.ID && x.TableDrivedEntityID2 == realPerson.ID);
                        if (typeColumn != null && relaitonship != null && relaitonship.RelationshipType != null && relaitonship.RelationshipType.SuperToSubRelationshipType != null)
                        {
                            if (relaitonship.RelationshipType.SuperToSubRelationshipType.DeterminerColumnID == null)
                            {
                                relaitonship.RelationshipType.SuperToSubRelationshipType.DeterminerColumnID = typeColumn.ID;
                                relaitonship.RelationshipType.SuperToSubRelationshipType.DeterminerColumnValue = "1";
                            }
                        }
                    }
                    if (legalPerson != null)
                    {
                        var relaitonship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == genericPerson.ID && x.TableDrivedEntityID2 == legalPerson.ID);
                        if (typeColumn != null && relaitonship != null && relaitonship.RelationshipType != null && relaitonship.RelationshipType.SuperToSubRelationshipType != null)
                        {
                            if (relaitonship.RelationshipType.SuperToSubRelationshipType.DeterminerColumnID == null)
                            {
                                relaitonship.RelationshipType.SuperToSubRelationshipType.DeterminerColumnID = typeColumn.ID;
                                relaitonship.RelationshipType.SuperToSubRelationshipType.DeterminerColumnValue = "2";
                            }
                        }
                    }
                }


                Formula todayFormula = projectContext.Formula.FirstOrDefault(x => x.Name == "Today");
                if (todayFormula == null)
                {
                    todayFormula = new Formula();
                    projectContext.Formula.Add(todayFormula);
                    todayFormula.TableDrivedEntityID = null;
                    todayFormula.Name = "Today";
                    todayFormula.ResultType = "System.String";
                    todayFormula.Title = "تاريخ روز";
                    todayFormula.LinearFormula = new LinearFormula();
                    todayFormula.LinearFormula.FormulaText = "PersianDateHelper.get_Today()";
                    todayFormula.LinearFormula.Version = 0;

                }


                Formula serviceItemPriceFormula = null;
                var serviceItem = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceItem" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (serviceItem != null)
                {
                    var StartDateTimeColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "StartDateTime");
                    var EndDateTimeColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "EndDateTime");

                    var updateDateColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "UpdateDate");
                    if (updateDateColumn != null)
                        updateDateColumn.DateColumnType.ShowMiladiDateInUI = true;
                    var updateTimeColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "UpdateTime");
                    if (updateTimeColumn != null)
                    {
                    }
                    var getNowCodeFunctoin = projectContext.CodeFunction.FirstOrDefault(x => x.FunctionName == "GetNow");
                    if (getNowCodeFunctoin == null)
                    {
                        getNowCodeFunctoin = new CodeFunction();
                        getNowCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        getNowCodeFunctoin.ClassName = "MyTestImplLibrary.DBProductService.DateTimeHelper";
                        getNowCodeFunctoin.FunctionName = "GetNow";
                        getNowCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.OneDataItem;
                        getNowCodeFunctoin.ReturnType = "System.DateTime";
                        getNowCodeFunctoin.Name = "تاريخ روز";
                        projectContext.CodeFunction.Add(getNowCodeFunctoin);

                        var serviceItemGetNowFormula = new Formula();
                        serviceItemGetNowFormula.CodeFunction = getNowCodeFunctoin;
                        serviceItemGetNowFormula.TableDrivedEntityID = serviceItem.ID;
                        serviceItemGetNowFormula.Name = "GetNow";
                        serviceItemGetNowFormula.ResultType = "System.DateTime";
                        serviceItemGetNowFormula.Title = "تاريخ روز";

                        projectContext.Formula.Add(serviceItemGetNowFormula);

                        if (updateDateColumn != null)
                            updateDateColumn.Formula = serviceItemGetNowFormula;
                        if (updateTimeColumn != null)
                            updateTimeColumn.Formula = serviceItemGetNowFormula;
                    }

                    CodeFunction_TableDrivedEntity serviceItemCodeFunctionEntity = null;
                    var getHoursSpentCodeFunctoin = projectContext.CodeFunction.FirstOrDefault(x => x.FunctionName == "GetHoursSpent");
                    if (getHoursSpentCodeFunctoin == null)
                    {
                        getHoursSpentCodeFunctoin = new CodeFunction();
                        getHoursSpentCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        getHoursSpentCodeFunctoin.ClassName = "MyTestImplLibrary.DBProductService.ServiceItemHelper";
                        getHoursSpentCodeFunctoin.FunctionName = "GetHoursSpent";
                        getHoursSpentCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.KeyColumns;
                        getHoursSpentCodeFunctoin.ReturnType = "System.Double";
                        getHoursSpentCodeFunctoin.Name = "محاسبه ساعات کار";
                        getHoursSpentCodeFunctoin.CodeFunctionParameter.Add(new CodeFunctionParameter() { DataType = "System.DateTime", ParamName = "start" });
                        getHoursSpentCodeFunctoin.CodeFunctionParameter.Add(new CodeFunctionParameter() { DataType = "System.DateTime", ParamName = "end" });
                        projectContext.CodeFunction.Add(getHoursSpentCodeFunctoin);
                        serviceItemCodeFunctionEntity = new CodeFunction_TableDrivedEntity();

                    }
                    else
                    {
                        serviceItemCodeFunctionEntity = projectContext.CodeFunction_TableDrivedEntity.FirstOrDefault(x => x.TableDrivedEntityID == serviceItem.ID && x.CodeFunctionID == getHoursSpentCodeFunctoin.ID);
                        if (serviceItemCodeFunctionEntity == null)
                            serviceItemCodeFunctionEntity = new CodeFunction_TableDrivedEntity();
                    }
                    Formula serviceItemHoursSpentFormula = null;
                    if (serviceItemCodeFunctionEntity.ID == 0)
                    {
                        serviceItemCodeFunctionEntity.TableDrivedEntityID = serviceItem.ID;
                        serviceItemCodeFunctionEntity.CodeFunction = getHoursSpentCodeFunctoin;
                        serviceItemCodeFunctionEntity.Title = "محاسبه ساعات کار";
                        serviceItemCodeFunctionEntity.Name = "GetHoursSpent";
                        if (StartDateTimeColumn != null && getHoursSpentCodeFunctoin.CodeFunctionParameter.Any(x => x.ParamName == "start"))
                            serviceItemCodeFunctionEntity.CodeFunction_TableDrivedEntity_Parameters.Add(new CodeFunction_TableDrivedEntity_Parameters()
                            { ColumnID = StartDateTimeColumn.ID, CodeFunctionParameter = getHoursSpentCodeFunctoin.CodeFunctionParameter.First(x => x.ParamName == "start") });
                        if (EndDateTimeColumn != null && getHoursSpentCodeFunctoin.CodeFunctionParameter.Any(x => x.ParamName == "end"))
                            serviceItemCodeFunctionEntity.CodeFunction_TableDrivedEntity_Parameters.Add(new CodeFunction_TableDrivedEntity_Parameters()
                            { ColumnID = EndDateTimeColumn.ID, CodeFunctionParameter = getHoursSpentCodeFunctoin.CodeFunctionParameter.First(x => x.ParamName == "end") });
                        projectContext.CodeFunction_TableDrivedEntity.Add(serviceItemCodeFunctionEntity);
                        serviceItemHoursSpentFormula = new Formula();
                    }
                    else
                    {
                        serviceItemHoursSpentFormula = projectContext.Formula.FirstOrDefault(x => x.TableDrivedEntityID == serviceItem.ID && x.CodeFunction_TableDrivedEntityID == serviceItemCodeFunctionEntity.ID);
                        if (serviceItemHoursSpentFormula == null)
                            serviceItemHoursSpentFormula = new Formula();

                    }
                    if (serviceItemHoursSpentFormula.ID == 0)
                    {
                        projectContext.Formula.Add(serviceItemHoursSpentFormula);
                        serviceItemHoursSpentFormula.CodeFunction_TableDrivedEntity = serviceItemCodeFunctionEntity;
                        serviceItemHoursSpentFormula.TableDrivedEntityID = serviceItem.ID;
                        serviceItemHoursSpentFormula.Name = "HoursSpent";
                        serviceItemHoursSpentFormula.ResultType = "System.Double";
                        serviceItemHoursSpentFormula.Title = "محاسبه ساعات";


                        if (StartDateTimeColumn != null)
                            serviceItemHoursSpentFormula.FormulaItems1.Add(new FormulaItems() { Column = StartDateTimeColumn, ItemTitle = "StartDateTime" });

                        if (EndDateTimeColumn != null)
                            serviceItemHoursSpentFormula.FormulaItems1.Add(new FormulaItems() { Column = EndDateTimeColumn, ItemTitle = "EndDateTime" });
                    }
                    var HoursSpentColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "HoursSpent");
                    if (HoursSpentColumn != null)
                        HoursSpentColumn.Formula = serviceItemHoursSpentFormula;
                    var ServiceTypeEnumColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "ServiceTypeEnum");

                    var sp_CalculateServiceItemPrice = projectContext.DatabaseFunction.FirstOrDefault(x => x.FunctionName == "sp_CalculateServiceItemPrice");
                    if (sp_CalculateServiceItemPrice != null)
                    {
                        DatabaseFunction_TableDrivedEntity serviceItemDatabaseFunctionEntity = projectContext.DatabaseFunction_TableDrivedEntity.FirstOrDefault(x => x.TableDrivedEntityID == serviceItem.ID && x.DatabaseFunctionID == sp_CalculateServiceItemPrice.ID); ;
                        if (serviceItemDatabaseFunctionEntity == null)
                        {
                            serviceItemDatabaseFunctionEntity = new DatabaseFunction_TableDrivedEntity();
                            serviceItemDatabaseFunctionEntity.TableDrivedEntityID = serviceItem.ID;
                            serviceItemDatabaseFunctionEntity.DatabaseFunctionID = sp_CalculateServiceItemPrice.ID;
                            serviceItemDatabaseFunctionEntity.Title = "محاسبه مبلغ";
                            serviceItemDatabaseFunctionEntity.Name = "ServiceItemPrice";
                            projectContext.DatabaseFunction_TableDrivedEntity.Add(serviceItemDatabaseFunctionEntity);
                            serviceItemPriceFormula = new Formula();
                        }
                        else
                        {
                            serviceItemPriceFormula = projectContext.Formula.FirstOrDefault(x => x.TableDrivedEntityID == serviceItem.ID && x.DatabaseFunction_TableDrivedEntityID == serviceItemDatabaseFunctionEntity.ID);
                            if (serviceItemPriceFormula == null)
                                serviceItemPriceFormula = new Formula();

                        }
                        if (serviceItemDatabaseFunctionEntity.ID == 0)
                        {
                            if (sp_CalculateServiceItemPrice.DatabaseFunctionParameter.Any(x => x.ParamName == "ReturnValue"))
                                serviceItemDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(new DatabaseFunction_TableDrivedEntity_Columns()
                                { ColumnID = null, DatabaseFunctionParameter = sp_CalculateServiceItemPrice.DatabaseFunctionParameter.First(x => x.ParamName == "ReturnValue") });
                            if (sp_CalculateServiceItemPrice.DatabaseFunctionParameter.Any(x => x.ParamName == "@price"))
                                serviceItemDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(new DatabaseFunction_TableDrivedEntity_Columns()
                                { ColumnID = null, DatabaseFunctionParameter = sp_CalculateServiceItemPrice.DatabaseFunctionParameter.First(x => x.ParamName == "@price") });

                            if (HoursSpentColumn != null && sp_CalculateServiceItemPrice.DatabaseFunctionParameter.Any(x => x.ParamName == "@hours"))
                                serviceItemDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(new DatabaseFunction_TableDrivedEntity_Columns()
                                { ColumnID = HoursSpentColumn.ID, DatabaseFunctionParameter = sp_CalculateServiceItemPrice.DatabaseFunctionParameter.First(x => x.ParamName == "@hours") });
                            if (ServiceTypeEnumColumn != null && sp_CalculateServiceItemPrice.DatabaseFunctionParameter.Any(x => x.ParamName == "@type"))
                                serviceItemDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(new DatabaseFunction_TableDrivedEntity_Columns()
                                { ColumnID = ServiceTypeEnumColumn.ID, DatabaseFunctionParameter = sp_CalculateServiceItemPrice.DatabaseFunctionParameter.First(x => x.ParamName == "@type") });
                        }

                        if (serviceItemPriceFormula.ID == 0)
                        {
                            projectContext.Formula.Add(serviceItemPriceFormula);
                            serviceItemPriceFormula.DatabaseFunction_TableDrivedEntity = serviceItemDatabaseFunctionEntity;
                            serviceItemPriceFormula.TableDrivedEntityID = serviceItem.ID;
                            serviceItemPriceFormula.Name = "Price";
                            serviceItemPriceFormula.ResultType = "System.Double";
                            serviceItemPriceFormula.Title = "محاسبه مبلغ";

                            if (HoursSpentColumn != null)
                                serviceItemPriceFormula.FormulaItems1.Add(new FormulaItems() { Column = HoursSpentColumn, ItemTitle = "HoursSpent" });

                            if (ServiceTypeEnumColumn != null)
                                serviceItemPriceFormula.FormulaItems1.Add(new FormulaItems() { Column = ServiceTypeEnumColumn, ItemTitle = "ServiceTypeEnum" });
                        }
                    }
                }
                var serviceAdditionalItem = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceAdditionalItem" && x.Table.DBSchema.DatabaseInformationID == databaseID);

                Column ConclusionItemPriceColumn = null;
                var serviceConclusionItem = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceConclusionItem" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (serviceConclusionItem != null)
                {
                    var typeColumn = serviceConclusionItem.Table.Column.FirstOrDefault(x => x.Name == "Type");
                    if (typeColumn != null)
                    {
                        if (typeColumn.ColumnValueRange == null)
                        {
                            typeColumn.ColumnValueRange = new ColumnValueRange();
                            typeColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "1", KeyTitle = "سرویس" });
                            typeColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "2", KeyTitle = "خدمات اضافی" });
                        }
                    }
                    if (serviceItem != null)
                    {
                        var relaitonship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusionItem.ID && x.TableDrivedEntityID2 == serviceItem.ID);
                        if (typeColumn != null && relaitonship != null && relaitonship.RelationshipType != null && relaitonship.RelationshipType.UnionToSubUnionRelationshipType != null)
                        {
                            if (relaitonship.RelationshipType.UnionToSubUnionRelationshipType.DeterminerColumnID == null)
                            {
                                relaitonship.RelationshipType.UnionToSubUnionRelationshipType.DeterminerColumnID = typeColumn.ID;
                                relaitonship.RelationshipType.UnionToSubUnionRelationshipType.DeterminerColumnValue = "1";
                            }
                        }
                    }
                    if (serviceAdditionalItem != null)
                    {
                        var relaitonship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusionItem.ID && x.TableDrivedEntityID2 == serviceAdditionalItem.ID);
                        if (typeColumn != null && relaitonship != null && relaitonship.RelationshipType != null && relaitonship.RelationshipType.UnionToSubUnionRelationshipType != null)
                        {
                            if (relaitonship.RelationshipType.UnionToSubUnionRelationshipType.DeterminerColumnID == null)
                            {
                                relaitonship.RelationshipType.UnionToSubUnionRelationshipType.DeterminerColumnID = typeColumn.ID;
                                relaitonship.RelationshipType.UnionToSubUnionRelationshipType.DeterminerColumnValue = "2";
                            }
                        }
                    }

                    Formula ConclusionItemPriceFormula = projectContext.Formula.FirstOrDefault(x => x.TableDrivedEntityID == serviceConclusionItem.ID && x.Name == "CalculatePrice");
                    if (ConclusionItemPriceFormula == null)
                    {
                        ConclusionItemPriceFormula = new Formula();
                        projectContext.Formula.Add(ConclusionItemPriceFormula);
                        ConclusionItemPriceFormula.TableDrivedEntityID = serviceConclusionItem.ID;
                        ConclusionItemPriceFormula.Name = "CalculatePrice";
                        ConclusionItemPriceFormula.ResultType = "System.Double";
                        ConclusionItemPriceFormula.Title = "محاسبه هزينه";
                        ConclusionItemPriceFormula.LinearFormula = new LinearFormula();
                        ConclusionItemPriceFormula.LinearFormula.FormulaText = "x.OTO_ServiceItem==null ? x.OTO_ServiceAdditionalItem.cl_Price : x.OTO_ServiceItem.p_Price";
                        ConclusionItemPriceFormula.LinearFormula.Version = 0;
                        if (serviceItem != null && serviceAdditionalItem != null)
                        {
                            var relationshipToServiceItem = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusionItem.ID && x.TableDrivedEntityID2 == serviceItem.ID);
                            if (relationshipToServiceItem != null)
                            {
                                ConclusionItemPriceFormula.FormulaItems1.Add(new FormulaItems() { Relationship = relationshipToServiceItem, ItemTitle = "OTO_ServiceItem" });
                                ConclusionItemPriceFormula.FormulaItems1.Add(new FormulaItems() { Formula = serviceItemPriceFormula, ItemTitle = "p_Price", RelationshipIDTail = relationshipToServiceItem.ID.ToString(), RelationshipNameTail = "OTO_ServiceItem" });
                            }

                            //برای OTO_ServiceAdditionalItem
                            //هم باید ستونهای فرمول آیتم اضافه شوند اما فعلا چون در برنامه هم لود نمیشه بهتره اول برنامه رو درست کنم بعد اینجا
                        }
                    }
                    ConclusionItemPriceColumn = serviceConclusionItem.Table.Column.FirstOrDefault(x => x.Name == "Price");
                    if (ConclusionItemPriceColumn != null)
                        ConclusionItemPriceColumn.Formula = ConclusionItemPriceFormula;

                }
                Column ConclusionTotalPriceColumn = null;
                var serviceConclusion = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceConclusion" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (serviceConclusion != null)
                {
                    var userRateColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "UserRate");
                    if (userRateColumn != null)
                    {
                        userRateColumn.NumericColumnType.MinValue = 1;
                        userRateColumn.NumericColumnType.MaxValue = 5;
                    }
                    var stringUpdateDateTime = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "UpdateDateTime");
                    if (stringUpdateDateTime != null)
                    {
                        if (stringUpdateDateTime.DateTimeColumnType == null)
                            stringUpdateDateTime.DateTimeColumnType = new DateTimeColumnType();
                        stringUpdateDateTime.DateTimeColumnType.ShowMiladiDateInUI = true;
                        stringUpdateDateTime.DateTimeColumnType.HideTimePicker = true;
                        stringUpdateDateTime.DateTimeColumnType.StringDateIsMiladi = true;
                        stringUpdateDateTime.DateTimeColumnType.StringTimeIsMiladi = false;
                        stringUpdateDateTime.DateTimeColumnType.StringTimeISAMPMFormat = true;
                        stringUpdateDateTime.DateTimeColumnType.ShowAMPMFormat = true;
                    }
                    if (serviceConclusionItem != null)
                    {
                        Formula serviceConclusionPriceFormula = projectContext.Formula.FirstOrDefault(x => x.TableDrivedEntityID == serviceConclusion.ID && x.Name == "SumItems");
                        if (serviceConclusionPriceFormula == null)
                        {
                            serviceConclusionPriceFormula = new Formula();
                            serviceConclusionPriceFormula.TableDrivedEntityID = serviceConclusion.ID;
                            serviceConclusionPriceFormula.Name = "SumItems";
                            serviceConclusionPriceFormula.ResultType = "System.Double";
                            serviceConclusionPriceFormula.Title = "جمع موارد";
                            serviceConclusionPriceFormula.LinearFormula = new LinearFormula();
                            serviceConclusionPriceFormula.LinearFormula.FormulaText = "x.OTM_ServiceConclusionItem.Sum(\"z => z.cl_Price\")";
                            serviceConclusionPriceFormula.LinearFormula.Version = 0;
                            projectContext.Formula.Add(serviceConclusionPriceFormula);

                            var relationshipToConclusionItem = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusion.ID && x.TableDrivedEntityID2 == serviceConclusionItem.ID);
                            if (relationshipToConclusionItem != null)
                            {
                                serviceConclusionPriceFormula.FormulaItems1.Add(new FormulaItems() { Relationship = relationshipToConclusionItem, ItemTitle = "OTM_ServiceConclusionItem" });
                                if (ConclusionItemPriceColumn != null)
                                    serviceConclusionPriceFormula.FormulaItems1.Add(new FormulaItems() { ColumnID = ConclusionItemPriceColumn.ID, ItemTitle = "cl_Price", RelationshipIDTail = relationshipToConclusionItem.ID.ToString(), RelationshipNameTail = "OTM_ServiceConclusionItem" });
                            }
                        }
                        ConclusionTotalPriceColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "TotalPrice");
                        if (ConclusionTotalPriceColumn != null)
                            ConclusionTotalPriceColumn.Formula = serviceConclusionPriceFormula;
                    }

                }
                var requestProductPart = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "RequestProductPart" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (requestProductPart != null)
                {
                    var dateTimeColumn = requestProductPart.Table.Column.FirstOrDefault(x => x.Name == "DateTime");
                    if (dateTimeColumn != null)
                        dateTimeColumn.DateTimeColumnType.ShowMiladiDateInUI = true;
                    var stringDateTimeColumn = requestProductPart.Table.Column.FirstOrDefault(x => x.Name == "StringDateTime");
                    if (stringDateTimeColumn != null)
                    {
                        stringDateTimeColumn.DateTimeColumnType.StringDateIsMiladi = true;
                        stringDateTimeColumn.DateTimeColumnType.StringTimeIsMiladi = true;
                        stringDateTimeColumn.DateTimeColumnType.StringTimeISAMPMFormat = true;
                        stringDateTimeColumn.DateTimeColumnType.ShowAMPMFormat = true;
                    }
                }

                var serviceRequest = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceRequest" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (serviceRequest != null)
                {
                    var persianDateColumn = serviceRequest.Table.Column.FirstOrDefault(x => x.Name == "PersianDate");
                    if (persianDateColumn != null)
                        persianDateColumn.DateColumnType.ShowMiladiDateInUI = true;
                    var timeColumn = serviceRequest.Table.Column.FirstOrDefault(x => x.Name == "Time");
                    if (timeColumn != null)
                    {
                        timeColumn.TimeColumnType.ShowAMPMFormat = true;
                        timeColumn.TimeColumnType.StringTimeISAMPMFormat = true;
                        timeColumn.TimeColumnType.StringTimeIsMiladi = false;
                        timeColumn.TimeColumnType.ShowMiladiTime = true;
                    }



                    if (persianDateColumn != null)
                    {
                        persianDateColumn.Formula = todayFormula;
                        persianDateColumn.CalculateFormulaAsDefault = true;
                    }

                }
                var srviceRequestReview = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceRequestReview" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (srviceRequestReview != null)
                {
                    var autoDateColumn = srviceRequestReview.Table.Column.FirstOrDefault(x => x.Name == "AutoDate");
                    if (autoDateColumn != null)
                        autoDateColumn.DateColumnType.StringDateIsMiladi = true;
                    var autoTimeColumn = srviceRequestReview.Table.Column.FirstOrDefault(x => x.Name == "AutoTime");
                    if (autoTimeColumn != null)
                    {
                        autoTimeColumn.TimeColumnType.ShowAMPMFormat = true;
                        autoTimeColumn.TimeColumnType.StringTimeIsMiladi = true;
                        autoTimeColumn.TimeColumnType.StringTimeISAMPMFormat = true;
                    }
                }

                var employee = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "Employee" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (employee != null)
                {
                    var employeeRoleColumn = employee.Table.Column.FirstOrDefault(x => x.Name == "EmployeeRole");
                    if (employeeRoleColumn != null)
                    {
                        if (employeeRoleColumn.ColumnValueRange == null)
                        {
                            employeeRoleColumn.ColumnValueRange = new ColumnValueRange();

                            employeeRoleColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "1", KeyTitle = "مدیر" });
                            employeeRoleColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "2", KeyTitle = "کارشناس" });
                        }
                    }
                }
                var office = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "Office" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (office != null)
                {
                    var workshopLevelColumn = office.Table.Column.FirstOrDefault(x => x.Name == "WorkshopLevel");
                    if (workshopLevelColumn != null)
                    {
                        if (workshopLevelColumn.ColumnValueRange == null)
                        {
                            workshopLevelColumn.ColumnValueRange = new ColumnValueRange();

                            workshopLevelColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "سطح1" });
                            workshopLevelColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "سطح2" });
                        }
                    }
                    var codeColumn = office.Table.Column.FirstOrDefault(x => x.Name == "Code");
                    if (codeColumn != null)
                    {
                        codeColumn.StringColumnType.MaxLength = 3;
                        codeColumn.StringColumnType.MinLength = 3;
                    }
                }

                var region = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "Region" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (region != null)
                {
                    var typeColumn = region.Table.Column.FirstOrDefault(x => x.Name == "Type");
                    if (typeColumn != null)
                    {
                        if (typeColumn.ColumnValueRange == null)
                        {
                            typeColumn.ColumnValueRange = new ColumnValueRange();

                            typeColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "کشور" });
                            typeColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "استان" });
                            typeColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "شهر" });

                        }
                    }
                }
                projectContext.SaveChanges();
            }
        }

        public bool EntityWithoutSetting(int databaseID)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                return projectContext.TableDrivedEntity.Any(x =>
                x.IsOrginal == true && x.IsView == false && x.IsDisabled == false && x.Table.DBSchema.DatabaseInformationID == databaseID
                && (x.EntitySearchID == null || x.EntityListViewID == null || !x.EntityUIComposition.Any() || x.SearchInitially == null));
            }
        }


    }
}
