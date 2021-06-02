﻿using DataAccess;
using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
                var customer = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "Customer" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                var productItem = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ProductItem" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                var realPerson = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "RealPerson" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (realPerson != null)
                {

                }
                var legalPerson = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "LegalPerson" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (legalPerson != null)
                {
                    var legalPersonName = legalPerson.Table.Column.FirstOrDefault(x => x.Name == "Name");
                    if (legalPersonName != null)
                    {
                        legalPersonName.IsMandatory = true;
                    }
                }
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




                    var confirmedColumn = serviceConclusionItem.Table.Column.FirstOrDefault(x => x.Name == "Confirmed");
                    if (confirmedColumn != null)
                    {
                        var readonlyTypeOfConclusionItem = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == serviceConclusionItem.ID && x.Title == "فقط خواندنی سازی نوع مورد خلاصه سرویس");
                        if (readonlyTypeOfConclusionItem == null)
                        {
                            readonlyTypeOfConclusionItem = new TableDrivedEntityState();
                            readonlyTypeOfConclusionItem.Title = "فقط خواندنی سازی نوع مورد خلاصه سرویس";
                            readonlyTypeOfConclusionItem.TableDrivedEntityID = serviceConclusionItem.ID;
                            readonlyTypeOfConclusionItem.ColumnID = confirmedColumn.ID;
                            readonlyTypeOfConclusionItem.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "true" });
                            var uiActionActivity = new UIActionActivity();
                            uiActionActivity.Title = "فقط خواندنی سازی نوع";
                            readonlyTypeOfConclusionItem.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                            uiActionActivity.Type = (short)Enum_ActionActivityType.UIEnablity;
                            uiActionActivity.TableDrivedEntityID = serviceConclusionItem.ID;
                            uiActionActivity.UIEnablityDetails.Add(new UIEnablityDetails() { ColumnID = typeColumn.ID, Readonly = true });
                            projectContext.TableDrivedEntityState.Add(readonlyTypeOfConclusionItem);
                        }


                        if (serviceItem != null && serviceAdditionalItem != null)
                        {
                            var serviceItemRelationship = serviceConclusionItem.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusionItem.ID && x.TableDrivedEntityID2 == serviceItem.ID);
                            var serviceAdditionalItemRelationship = serviceConclusionItem.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusionItem.ID && x.TableDrivedEntityID2 == serviceAdditionalItem.ID);
                            if (serviceItemRelationship != null && serviceAdditionalItemRelationship != null)
                            {
                                var readonlyRelationships = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == serviceConclusionItem.ID && x.Title == "فقط خواندنی سازی روابط");
                                if (readonlyRelationships == null)
                                {
                                    readonlyRelationships = new TableDrivedEntityState();
                                    readonlyRelationships.Title = "فقط خواندنی سازی روابط";
                                    readonlyRelationships.TableDrivedEntityID = serviceConclusionItem.ID;
                                    readonlyRelationships.ColumnID = confirmedColumn.ID;
                                    readonlyRelationships.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "true" });
                                    var uiActionActivity = new UIActionActivity();
                                    uiActionActivity.Title = "فقط خواندنی سازی روابط";
                                    readonlyRelationships.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                                    uiActionActivity.Type = (short)Enum_ActionActivityType.UIEnablity;
                                    uiActionActivity.TableDrivedEntityID = serviceConclusionItem.ID;
                                    uiActionActivity.UIEnablityDetails.Add(new UIEnablityDetails() { RelationshipID = serviceItemRelationship.ID, Readonly = true });
                                    uiActionActivity.UIEnablityDetails.Add(new UIEnablityDetails() { RelationshipID = serviceAdditionalItemRelationship.ID, Readonly = true });
                                    projectContext.TableDrivedEntityState.Add(readonlyRelationships);
                                }
                            }
                        }
                    }
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
                    {
                        persianDateColumn.IsReadonly = true;
                        if (persianDateColumn.DateColumnType != null)
                            persianDateColumn.DateColumnType.ShowMiladiDateInUI = true;


                    }
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

                        var employeeRole2Column = employee.Table.Column.FirstOrDefault(x => x.Name == "EmployeeRole2");
                        if (employeeRole2Column != null)
                        {
                            if (employeeRole2Column.ColumnValueRange == null)
                            {
                                employeeRole2Column.ColumnValueRange = new ColumnValueRange();

                                employeeRole2Column.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "1", Tag1 = "1", KeyTitle = "مدیر سطح 1" });
                                employeeRole2Column.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "2", Tag1 = "1", KeyTitle = "مدیر سطح 2" });
                                employeeRole2Column.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "3", Tag1 = "2", KeyTitle = "کارشناس سطح 1" });
                                employeeRole2Column.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "4", Tag1 = "2", KeyTitle = "کارشناس سطح 2" });
                            }

                            var employeeRole2ModirState = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == employee.ID && x.Title == "فیلتر نقش 2 کارمند برای مدیر");
                            if (employeeRole2ModirState == null)
                            {
                                employeeRole2ModirState = new TableDrivedEntityState();
                                employeeRole2ModirState.Title = "فیلتر نقش 2 کارمند برای مدیر";
                                employeeRole2ModirState.TableDrivedEntityID = employee.ID;
                                employeeRole2ModirState.ColumnID = employeeRoleColumn.ID;
                                employeeRole2ModirState.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "1" });
                                var uiActionActivity = new UIActionActivity();
                                uiActionActivity.Title = "نقش 2 برای مدیر";
                                employeeRole2ModirState.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                                uiActionActivity.Type = (short)Enum_ActionActivityType.ColumnValueRange;
                                uiActionActivity.TableDrivedEntityID = employee.ID;
                                uiActionActivity.UIColumnValueRange.Add(new UIColumnValueRange() { ColumnValueRange = employeeRole2Column.ColumnValueRange, EnumTag = (short)EnumColumnValueRangeTag.Tag1, Value = "1" });
                                projectContext.TableDrivedEntityState.Add(employeeRole2ModirState);
                            }
                            var employeeRole2KarshenasState = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == employee.ID && x.Title == "فیلتر نقش 2 کارمند برای کارشناس");
                            if (employeeRole2KarshenasState == null)
                            {
                                employeeRole2KarshenasState = new TableDrivedEntityState();
                                employeeRole2KarshenasState.Title = "فیلتر نقش 2 کارمند برای کارشناس";
                                employeeRole2KarshenasState.TableDrivedEntityID = employee.ID;
                                employeeRole2KarshenasState.ColumnID = employeeRoleColumn.ID;
                                employeeRole2KarshenasState.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "2" });
                                var uiActionActivity = new UIActionActivity();
                                uiActionActivity.Title = "نقش 2 برای کارشناس";
                                employeeRole2KarshenasState.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                                uiActionActivity.Type = (short)Enum_ActionActivityType.ColumnValueRange;
                                uiActionActivity.TableDrivedEntityID = employee.ID;
                                uiActionActivity.UIColumnValueRange.Add(new UIColumnValueRange() { ColumnValueRange = employeeRole2Column.ColumnValueRange, EnumTag = (short)EnumColumnValueRangeTag.Tag1, Value = "2" });
                                projectContext.TableDrivedEntityState.Add(employeeRole2KarshenasState);
                            }
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
                    var isAgencyColumn = office.Table.Column.FirstOrDefault(x => x.Name == "IsAgency");
                    var isWorkshopColumn = office.Table.Column.FirstOrDefault(x => x.Name == "IsWorkshop");
                    if (isAgencyColumn != null && isWorkshopColumn != null && codeColumn != null && workshopLevelColumn != null)
                    {
                        var agencyDefaultCodeState = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == office.ID && x.Title == "کد پیش فرض سازمان برای آژانس");
                        if (agencyDefaultCodeState == null)
                        {
                            agencyDefaultCodeState = new TableDrivedEntityState();
                            agencyDefaultCodeState.Title = "کد پیش فرض سازمان برای آژانس";
                            agencyDefaultCodeState.TableDrivedEntityID = office.ID;
                            agencyDefaultCodeState.ColumnID = isAgencyColumn.ID;
                            agencyDefaultCodeState.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "true" });
                            var uiActionActivity = new UIActionActivity();
                            uiActionActivity.Title = "مقدار پیش فرض کد 10";
                            agencyDefaultCodeState.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                            uiActionActivity.Type = (short)Enum_ActionActivityType.ColumnValue;
                            uiActionActivity.TableDrivedEntityID = office.ID;
                            uiActionActivity.UIColumnValue.Add(new UIColumnValue() { ColumnID = codeColumn.ID, ExactValue = "10" });
                            projectContext.TableDrivedEntityState.Add(agencyDefaultCodeState);
                        }
                        var workshopDefaultCodeState = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == office.ID && x.Title == "کد پیش فرض سازمان برای ورکشاپ");
                        if (workshopDefaultCodeState == null)
                        {
                            workshopDefaultCodeState = new TableDrivedEntityState();
                            workshopDefaultCodeState.Title = "کد پیش فرض سازمان برای ورکشاپ";
                            workshopDefaultCodeState.TableDrivedEntityID = office.ID;
                            workshopDefaultCodeState.ColumnID = isWorkshopColumn.ID;
                            workshopDefaultCodeState.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "true" });
                            var uiActionActivity = new UIActionActivity();
                            uiActionActivity.Title = "مقدار پیش فرض کد 20";
                            workshopDefaultCodeState.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                            uiActionActivity.Type = (short)Enum_ActionActivityType.ColumnValue;
                            uiActionActivity.TableDrivedEntityID = office.ID;
                            uiActionActivity.UIColumnValue.Add(new UIColumnValue() { ColumnID = codeColumn.ID, ExactValue = "20" });
                            projectContext.TableDrivedEntityState.Add(workshopDefaultCodeState);
                        }

                        var hideWorkshopLevelState = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == office.ID && x.Title == "مخفی کردن سطح ورکشاپ برای آژانس");
                        if (hideWorkshopLevelState == null)
                        {
                            hideWorkshopLevelState = new TableDrivedEntityState();
                            hideWorkshopLevelState.Title = "مخفی کردن سطح ورکشاپ برای آژانس";
                            hideWorkshopLevelState.TableDrivedEntityID = office.ID;
                            hideWorkshopLevelState.ColumnID = isAgencyColumn.ID;
                            hideWorkshopLevelState.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "true" });
                            var uiActionActivity = new UIActionActivity();
                            uiActionActivity.Title = "مخفی کردن سطح ورکشاپ";
                            hideWorkshopLevelState.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                            uiActionActivity.Type = (short)Enum_ActionActivityType.UIEnablity;
                            uiActionActivity.TableDrivedEntityID = office.ID;
                            uiActionActivity.UIEnablityDetails.Add(new UIEnablityDetails() { ColumnID = workshopLevelColumn.ID, Hidden = true });
                            projectContext.TableDrivedEntityState.Add(hideWorkshopLevelState);
                        }
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

                        var regionCountryReadonly = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == region.ID && x.Title == "فقط خواندنی سازی کشورها");
                        if (regionCountryReadonly == null)
                        {
                            regionCountryReadonly = new TableDrivedEntityState();
                            regionCountryReadonly.Title = "فقط خواندنی سازی کشورها";
                            regionCountryReadonly.TableDrivedEntityID = region.ID;
                            regionCountryReadonly.ColumnID = typeColumn.ID;
                            regionCountryReadonly.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "کشور" });
                            var uiActionActivity = new UIActionActivity();
                            uiActionActivity.Title = "فقط خواندنی سازی کشورها";
                            regionCountryReadonly.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                            uiActionActivity.Type = (short)Enum_ActionActivityType.EntityReadonly;
                            uiActionActivity.TableDrivedEntityID = region.ID;
                            projectContext.TableDrivedEntityState.Add(regionCountryReadonly);
                        }
                        var regionParentRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == region.ID && x.TableDrivedEntityID2 == region.ID && x.MasterTypeEnum == 1);
                        if (regionParentRelationship != null)
                        {
                            var regionCountryHideParent = projectContext.TableDrivedEntityState.FirstOrDefault(x => x.TableDrivedEntityID == region.ID && x.Title == "مخفی سازی رابطه پدر برای کشورها");
                            if (regionCountryHideParent == null)
                            {
                                regionCountryHideParent = new TableDrivedEntityState();
                                regionCountryHideParent.Title = "مخفی سازی رابطه پدر برای کشورها";
                                regionCountryHideParent.TableDrivedEntityID = region.ID;
                                regionCountryHideParent.ColumnID = typeColumn.ID;
                                regionCountryHideParent.TableDrivedEntityStateValues.Add(new TableDrivedEntityStateValues() { Value = "کشور" });
                                var uiActionActivity = new UIActionActivity();
                                uiActionActivity.Title = "مخفی سازی رابطه پدر";
                                regionCountryHideParent.EntityState_UIActionActivity.Add(new EntityState_UIActionActivity() { UIActionActivity = uiActionActivity });
                                uiActionActivity.Type = (short)Enum_ActionActivityType.UIEnablity;
                                uiActionActivity.TableDrivedEntityID = region.ID;
                                uiActionActivity.UIEnablityDetails.Add(new UIEnablityDetails() { RelationshipID = regionParentRelationship.ID, Hidden = true });
                                projectContext.TableDrivedEntityState.Add(regionCountryHideParent);
                            }
                        }

                    }
                }
                var genericPersonAddress = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "GenericPersonAddress" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (genericPersonAddress != null && region != null)
                {
                    var relaitonship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == genericPersonAddress.ID && x.TableDrivedEntityID2 == region.ID);
                    if (relaitonship != null)
                        relaitonship.RelationshipType.IsOtherSideMandatory = true;
                }

                var isaGenereicPerson = projectContext.ISARelationship.FirstOrDefault(x => x.Name == "ISA_OnPK_GenericPerson");
                if (isaGenereicPerson != null)
                {
                    isaGenereicPerson.IsDisjoint = true;
                    isaGenereicPerson.IsTolatParticipation = true;
                }
                var isaGenereicPersonCustomer = projectContext.ISARelationship.FirstOrDefault(x => x.Name == "GenericPerson_Customer");
                if (isaGenereicPersonCustomer != null)
                {
                    isaGenereicPersonCustomer.IsTolatParticipation = false;
                }

                var unionServiceConclusionItem = projectContext.UnionRelationshipType.FirstOrDefault(x => x.Name == "Union_ServiceConclusionItem");
                if (unionServiceConclusionItem != null)
                {
                    unionServiceConclusionItem.IsTolatParticipation = false;
                }

                var genericBeforeLoadBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.BeforeLoad && x.TableDrivedEntityID == null && x.Title == "اصلاح تاريخ تک رقمي");
                if (genericBeforeLoadBackenActionActivity == null)
                {
                    genericBeforeLoadBackenActionActivity = new BackendActionActivity();
                    genericBeforeLoadBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.BeforeLoad;
                    genericBeforeLoadBackenActionActivity.Title = "اصلاح تاريخ تک رقمي";
                    genericBeforeLoadBackenActionActivity.Type = 0;
                    genericBeforeLoadBackenActionActivity.ResultSensetive = false;
                    genericBeforeLoadBackenActionActivity.CodeFunction = new CodeFunction();
                    genericBeforeLoadBackenActionActivity.CodeFunction.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                    genericBeforeLoadBackenActionActivity.CodeFunction.ClassName = "MyTestImplLibrary.BeforeLoadTest";
                    genericBeforeLoadBackenActionActivity.CodeFunction.FunctionName = "EditPersianDateMonthDay";
                    genericBeforeLoadBackenActionActivity.CodeFunction.Type = 0;
                    genericBeforeLoadBackenActionActivity.CodeFunction.ReturnType = "ModelEntites.FunctionResult";
                    genericBeforeLoadBackenActionActivity.CodeFunction.Name = "اصلاح تاريخ تک رقمي";
                    projectContext.BackendActionActivity.Add(genericBeforeLoadBackenActionActivity);
                }
                if (realPerson != null)
                {
                    var beforeLoadBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.BeforeLoad && x.TableDrivedEntityID == realPerson.ID && x.Title == "اصلاح شماره ملی شخص حقیقی");
                    if (beforeLoadBackenActionActivity == null)
                    {
                        beforeLoadBackenActionActivity = new BackendActionActivity();
                        beforeLoadBackenActionActivity.TableDrivedEntityID = realPerson.ID;
                        beforeLoadBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.BeforeLoad;
                        beforeLoadBackenActionActivity.Title = "اصلاح شماره ملی شخص حقیقی";
                        beforeLoadBackenActionActivity.Type = 0;
                        beforeLoadBackenActionActivity.ResultSensetive = false;
                        beforeLoadBackenActionActivity.CodeFunction = new CodeFunction();
                        beforeLoadBackenActionActivity.CodeFunction.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        beforeLoadBackenActionActivity.CodeFunction.ClassName = "MyTestImplLibrary.BeforeLoadTest";
                        beforeLoadBackenActionActivity.CodeFunction.FunctionName = "EditRealPersonNationalCode";
                        beforeLoadBackenActionActivity.CodeFunction.Type = 0;
                        beforeLoadBackenActionActivity.CodeFunction.ReturnType = "ModelEntites.FunctionResult";
                        beforeLoadBackenActionActivity.CodeFunction.Name = "اصلاح عنوان شرکت";
                        projectContext.BackendActionActivity.Add(beforeLoadBackenActionActivity);
                    }
                }

                var genericBeforeSaveBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.BeforeSave && x.TableDrivedEntityID == null && x.Title == "اعتبارسنجی درخواست");
                if (genericBeforeSaveBackenActionActivity == null)
                {
                    genericBeforeSaveBackenActionActivity = new BackendActionActivity();
                    genericBeforeSaveBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.BeforeSave;
                    genericBeforeSaveBackenActionActivity.Title = "اعتبارسنجی درخواست";
                    genericBeforeSaveBackenActionActivity.Type = 0;
                    genericBeforeSaveBackenActionActivity.ResultSensetive = false;
                    genericBeforeSaveBackenActionActivity.CodeFunction = new CodeFunction();
                    genericBeforeSaveBackenActionActivity.CodeFunction.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                    genericBeforeSaveBackenActionActivity.CodeFunction.ClassName = "MyTestImplLibrary.BeforeSaveTest";
                    genericBeforeSaveBackenActionActivity.CodeFunction.FunctionName = "CheckRequestIsValid";
                    genericBeforeSaveBackenActionActivity.CodeFunction.Type = 0;
                    genericBeforeSaveBackenActionActivity.CodeFunction.ReturnType = "ModelEntites.FunctionResult";
                    genericBeforeSaveBackenActionActivity.CodeFunction.Name = "اعتبارسنجی درخواست";
                    projectContext.BackendActionActivity.Add(genericBeforeSaveBackenActionActivity);

                }
                if (legalPerson != null)
                {
                    var beforeSaveBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.BeforeSave && x.TableDrivedEntityID == legalPerson.ID && x.Title == "اصلاح عبارت شرکت در عنوان شخص حقوقی");
                    if (beforeSaveBackenActionActivity == null)
                    {
                        beforeSaveBackenActionActivity = new BackendActionActivity();
                        beforeSaveBackenActionActivity.TableDrivedEntityID = legalPerson.ID;
                        beforeSaveBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.BeforeSave;
                        beforeSaveBackenActionActivity.Title = "اصلاح عبارت شرکت در عنوان شخص حقوقی";
                        beforeSaveBackenActionActivity.Type = 0;
                        beforeSaveBackenActionActivity.ResultSensetive = false;
                        beforeSaveBackenActionActivity.CodeFunction = new CodeFunction();
                        beforeSaveBackenActionActivity.CodeFunction.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        beforeSaveBackenActionActivity.CodeFunction.ClassName = "MyTestImplLibrary.BeforeSaveTest";
                        beforeSaveBackenActionActivity.CodeFunction.FunctionName = "EditLegalPersonName";
                        beforeSaveBackenActionActivity.CodeFunction.Type = 0;
                        beforeSaveBackenActionActivity.CodeFunction.ReturnType = "ModelEntites.FunctionResult";
                        beforeSaveBackenActionActivity.CodeFunction.Name = "اصلاح عبارت شرکت در عنوان شخص حقوقی";
                        projectContext.BackendActionActivity.Add(beforeSaveBackenActionActivity);
                    }
                }


                var genericBeforeDeleteBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.BeforeDelete && x.TableDrivedEntityID == null && x.Title == "اعتبارسنجی درخواست حذف");
                if (genericBeforeDeleteBackenActionActivity == null)
                {
                    genericBeforeDeleteBackenActionActivity = new BackendActionActivity();
                    genericBeforeDeleteBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.BeforeDelete;
                    genericBeforeDeleteBackenActionActivity.Title = "اعتبارسنجی درخواست حذف";
                    genericBeforeDeleteBackenActionActivity.Type = 0;
                    genericBeforeDeleteBackenActionActivity.ResultSensetive = false;
                    genericBeforeDeleteBackenActionActivity.CodeFunction = new CodeFunction();
                    genericBeforeDeleteBackenActionActivity.CodeFunction.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                    genericBeforeDeleteBackenActionActivity.CodeFunction.ClassName = "MyTestImplLibrary.BeforeDeleteTest";
                    genericBeforeDeleteBackenActionActivity.CodeFunction.FunctionName = "CheckRequestIsValid";
                    genericBeforeDeleteBackenActionActivity.CodeFunction.Type = 0;
                    genericBeforeDeleteBackenActionActivity.CodeFunction.ReturnType = "ModelEntites.FunctionResult";
                    genericBeforeDeleteBackenActionActivity.CodeFunction.Name = "اعتبارسنجی درخواست حذف";
                    projectContext.BackendActionActivity.Add(genericBeforeDeleteBackenActionActivity);

                }

                if (serviceRequest != null)
                {
                    var beforeDeleteBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.BeforeDelete && x.TableDrivedEntityID == serviceRequest.ID && x.Title == "اعتبارسنجی حذف درخواست سرویس");
                    if (beforeDeleteBackenActionActivity == null)
                    {
                        beforeDeleteBackenActionActivity = new BackendActionActivity();
                        beforeDeleteBackenActionActivity.TableDrivedEntityID = serviceRequest.ID;
                        beforeDeleteBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.BeforeDelete;
                        beforeDeleteBackenActionActivity.Title = "اعتبارسنجی حذف درخواست سرویس";
                        beforeDeleteBackenActionActivity.Type = 0;
                        beforeDeleteBackenActionActivity.ResultSensetive = false;
                        beforeDeleteBackenActionActivity.CodeFunction = new CodeFunction();
                        beforeDeleteBackenActionActivity.CodeFunction.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        beforeDeleteBackenActionActivity.CodeFunction.ClassName = "MyTestImplLibrary.BeforeDeleteTest";
                        beforeDeleteBackenActionActivity.CodeFunction.FunctionName = "ValidateServiceRequest";
                        beforeDeleteBackenActionActivity.CodeFunction.Type = 0;
                        beforeDeleteBackenActionActivity.CodeFunction.ReturnType = "ModelEntites.FunctionResult";
                        beforeDeleteBackenActionActivity.CodeFunction.Name = "اعتبارسنجی حذف درخواست سرویس";
                        projectContext.BackendActionActivity.Add(beforeDeleteBackenActionActivity);
                    }
                }




                if (serviceConclusion != null)
                {
                    var sp_CalculateCustomerValue = projectContext.DatabaseFunction.FirstOrDefault(x => x.FunctionName == "sp_CalculateCustomerValue");
                    if (sp_CalculateCustomerValue != null)
                    {
                        DatabaseFunction_TableDrivedEntity serviceConclusionDatabaseFunctionEntity = projectContext.DatabaseFunction_TableDrivedEntity.FirstOrDefault(x => x.TableDrivedEntityID == serviceConclusion.ID && x.DatabaseFunctionID == sp_CalculateCustomerValue.ID);
                        if (serviceConclusionDatabaseFunctionEntity == null)
                        {
                            serviceConclusionDatabaseFunctionEntity = new DatabaseFunction_TableDrivedEntity();
                            serviceConclusionDatabaseFunctionEntity.TableDrivedEntityID = serviceConclusion.ID;
                            serviceConclusionDatabaseFunctionEntity.DatabaseFunctionID = sp_CalculateCustomerValue.ID;
                            serviceConclusionDatabaseFunctionEntity.Title = "محاسبه ارزش مشتری";
                            serviceConclusionDatabaseFunctionEntity.Name = "CustomerValue";
                            projectContext.DatabaseFunction_TableDrivedEntity.Add(serviceConclusionDatabaseFunctionEntity);
                        }

                        var conclusionIDColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "ID");

                        if (conclusionIDColumn != null && serviceConclusionDatabaseFunctionEntity.ID == 0)
                        {
                            if (sp_CalculateCustomerValue.DatabaseFunctionParameter.Any(x => x.ParamName == "@serviceConclusionID"))
                                serviceConclusionDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(new DatabaseFunction_TableDrivedEntity_Columns()
                                { ColumnID = conclusionIDColumn.ID, DatabaseFunctionParameter = sp_CalculateCustomerValue.DatabaseFunctionParameter.First(x => x.ParamName == "@serviceConclusionID") });

                        }
                        var afterSaveBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.AfterSave && x.TableDrivedEntityID == serviceConclusion.ID && x.Title == "محاسبه ارزش مشتری");
                        if (afterSaveBackenActionActivity == null)
                        {
                            afterSaveBackenActionActivity = new BackendActionActivity();
                            afterSaveBackenActionActivity.TableDrivedEntityID = serviceConclusion.ID;
                            afterSaveBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.AfterSave;
                            afterSaveBackenActionActivity.Title = "محاسبه ارزش مشتری";
                            afterSaveBackenActionActivity.Type = 1;
                            afterSaveBackenActionActivity.ResultSensetive = false;

                            afterSaveBackenActionActivity.DatabaseFunction_TableDrivedEntity = serviceConclusionDatabaseFunctionEntity;
                            projectContext.BackendActionActivity.Add(afterSaveBackenActionActivity);
                        }

                        if (!serviceConclusion.EntityListView.Any(x => x.Title == "لیست پیش فرض خلاصه سرویس"))
                        {
                            var serviceConclusaionListView = new EntityListView();
                            serviceConclusaionListView.TableDrivedEntityID = serviceConclusion.ID;
                            //    projectContext.EntityListView.Add(serviceConclusaionListView);
                            serviceConclusaionListView.Title = "لیست پیش فرض خلاصه سرویس";
                            serviceConclusion.EntityListView1 = serviceConclusaionListView;
                            if (conclusionIDColumn != null)
                            {
                                var idColumn = new EntityListViewColumns() { Column = conclusionIDColumn, Alias = "شناسه", WidthUnit = 1, IsDescriptive = true, OrderID = 1 };
                                serviceConclusaionListView.EntityListViewColumns.Add(idColumn);
                            }
                            var serviceRequestIDColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "RequestServiceID");
                            if (serviceRequestIDColumn != null)
                            {
                                var serviceRequestIDListViewColumn = new EntityListViewColumns() { Column = serviceRequestIDColumn, Alias = "شناسه درخواست سرویس", WidthUnit = 1, IsDescriptive = true, OrderID = 2 };
                                serviceConclusaionListView.EntityListViewColumns.Add(serviceRequestIDListViewColumn);
                            }
                            if (serviceRequest != null)
                            {
                                var serviceRequestRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusion.ID && x.TableDrivedEntityID2 == serviceRequest.ID);
                                if (serviceRequestRelationship != null)
                                {
                                    var serviceRequestRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, serviceRequest, serviceRequestRelationship.ID.ToString());
                                    if (serviceRequestRelationshipTail != null)
                                    {
                                        if (customer != null)
                                        {
                                            var serviceRequestCustomerRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == customer.ID);
                                            if (serviceRequestCustomerRelationship != null)
                                            {
                                                var serviceRequestCustomerRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, customer, serviceRequestRelationship.ID.ToString() + "," + serviceRequestCustomerRelationship.ID.ToString());
                                                if (serviceRequestCustomerRelationshipTail != null)
                                                {
                                                    var customerCodeColumn = customer.Table.Column.FirstOrDefault(x => x.Name == "Code");
                                                    if (customerCodeColumn != null)
                                                    {
                                                        var serviceResuestCustomerCode = new EntityListViewColumns() { EntityRelationshipTail = serviceRequestCustomerRelationshipTail, Column = customerCodeColumn, Alias = "کد مشتری", WidthUnit = 1, IsDescriptive = false, OrderID = 3 };
                                                        serviceConclusaionListView.EntityListViewColumns.Add(serviceResuestCustomerCode);
                                                    }
                                                    if (genericPerson != null)
                                                    {
                                                        var genericPersonNameColumn = genericPerson.Table.Column.FirstOrDefault(x => x.Name == "Name");

                                                        var serviceRequestCustomerGenericPersonRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == customer.ID && x.TableDrivedEntityID2 == genericPerson.ID);
                                                        if (serviceRequestCustomerGenericPersonRelationship != null)
                                                        {
                                                            var serviceRequestCustomerGenericPersonRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, genericPerson, serviceRequestRelationship.ID.ToString() + "," + serviceRequestCustomerRelationship.ID.ToString()
                                                                + "," + serviceRequestCustomerGenericPersonRelationship.ID.ToString());
                                                            if (serviceRequestCustomerGenericPersonRelationshipTail != null)
                                                            {
                                                                var serviceResuestCustomerName = new EntityListViewColumns() { EntityRelationshipTail = serviceRequestCustomerGenericPersonRelationshipTail, Column = genericPersonNameColumn, Alias = "نام مشتری", WidthUnit = 2, IsDescriptive = false, OrderID = 4 };
                                                                serviceConclusaionListView.EntityListViewColumns.Add(serviceResuestCustomerName);


                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        if (productItem != null)
                                        {
                                            var serviceRequestProductItemRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == productItem.ID);
                                            if (serviceRequestProductItemRelationship != null)
                                            {
                                                var serviceRequestProductItemRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, productItem, serviceRequestRelationship.ID.ToString() + "," + serviceRequestProductItemRelationship.ID.ToString());
                                                if (serviceRequestProductItemRelationshipTail != null)
                                                {
                                                    var productItemModelColumn = productItem.Table.Column.FirstOrDefault(x => x.Name == "ProductModel");
                                                    if (productItemModelColumn != null)
                                                    {
                                                        var serviceResuestProductItemModel = new EntityListViewColumns() { EntityRelationshipTail = serviceRequestProductItemRelationshipTail, Column = productItemModelColumn, Alias = "مدل کالا", WidthUnit = 2, IsDescriptive = false, OrderID = 5 };
                                                        serviceConclusaionListView.EntityListViewColumns.Add(serviceResuestProductItemModel);
                                                    }
                                                    var productItemBrandColumn = productItem.Table.Column.FirstOrDefault(x => x.Name == "BrandTitle");
                                                    if (productItemBrandColumn != null)
                                                    {
                                                        var serviceResuestProductItemModel = new EntityListViewColumns() { EntityRelationshipTail = serviceRequestProductItemRelationshipTail, Column = productItemBrandColumn, Alias = "برند کالا", WidthUnit = 2, IsDescriptive = false, OrderID = 6 };
                                                        serviceConclusaionListView.EntityListViewColumns.Add(serviceResuestProductItemModel);
                                                    }
                                                    var productItemSerialColumn = productItem.Table.Column.FirstOrDefault(x => x.Name == "Serial");
                                                    if (productItemSerialColumn != null)
                                                    {
                                                        var serviceResuestProductItemModel = new EntityListViewColumns() { EntityRelationshipTail = serviceRequestProductItemRelationshipTail, Column = productItemSerialColumn, Alias = "سریال کالا", WidthUnit = 2, IsDescriptive = false, OrderID = 7 };
                                                        serviceConclusaionListView.EntityListViewColumns.Add(serviceResuestProductItemModel);
                                                    }
                                                }
                                            }
                                        }
                                        if (office != null)
                                        {
                                            var serviceRequestOfficeRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == office.ID);
                                            if (serviceRequestOfficeRelationship != null)
                                            {
                                                var serviceRequestOfficeRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, office, serviceRequestRelationship.ID.ToString() + "," + serviceRequestOfficeRelationship.ID.ToString());
                                                if (serviceRequestOfficeRelationshipTail != null)
                                                {
                                                    var officeCodeColumn = office.Table.Column.FirstOrDefault(x => x.Name == "Code");
                                                    if (officeCodeColumn != null)
                                                    {
                                                        var serviceResuestOfficeCode = new EntityListViewColumns() { EntityRelationshipTail = serviceRequestOfficeRelationshipTail, Column = officeCodeColumn, Alias = "کد دفتر", WidthUnit = 1, IsDescriptive = false, OrderID = 8 };
                                                        serviceConclusaionListView.EntityListViewColumns.Add(serviceResuestOfficeCode);
                                                    }
                                                    if (legalPerson != null)
                                                    {
                                                        var legalPersonNameColumn = legalPerson.Table.Column.FirstOrDefault(x => x.Name == "Name");

                                                        var serviceRequestOfficelegalPersonRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == office.ID && x.TableDrivedEntityID2 == legalPerson.ID);
                                                        if (serviceRequestOfficelegalPersonRelationship != null)
                                                        {
                                                            var serviceRequestOfficelegalPersonRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion,legalPerson, serviceRequestRelationship.ID.ToString() + "," + serviceRequestOfficeRelationship.ID.ToString()
                                                                + "," + serviceRequestOfficelegalPersonRelationship.ID.ToString());
                                                            if (serviceRequestOfficelegalPersonRelationshipTail != null)
                                                            {
                                                                var serviceResuestOfficeName = new EntityListViewColumns() { EntityRelationshipTail = serviceRequestOfficelegalPersonRelationshipTail, Column = legalPersonNameColumn, Alias = "نام دفتر", WidthUnit = 2, IsDescriptive = false, OrderID = 9 };
                                                                serviceConclusaionListView.EntityListViewColumns.Add(serviceResuestOfficeName);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }













                        if (!serviceConclusion.EntitySearch1.Any(x => x.Title == "جستجوی پیش فرض خلاصه سرویس"))
                        {
                            var serviceConclusaionSearch = new EntitySearch();
                            serviceConclusaionSearch.TableDrivedEntityID = serviceConclusion.ID;
                            //    projectContext.EntitySearch.Add(serviceConclusaionSearch);
                            serviceConclusaionSearch.Title = "جستجوی پیش فرض خلاصه سرویس";
                            serviceConclusion.EntitySearch = serviceConclusaionSearch;
                            if (conclusionIDColumn != null)
                            {
                                var idColumn = new EntitySearchColumns() { Column = conclusionIDColumn, Alias = "شناسه", OrderID = 1 };
                                serviceConclusaionSearch.EntitySearchColumns.Add(idColumn);
                            }
                            var serviceRequestIDColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "RequestServiceID");
                            if (serviceRequestIDColumn != null)
                            {
                                var serviceRequestIDSearchColumn = new EntitySearchColumns() { Column = serviceRequestIDColumn, Alias = "شناسه درخواست سرویس", OrderID = 2 };
                                serviceConclusaionSearch.EntitySearchColumns.Add(serviceRequestIDSearchColumn);
                            }
                            if (serviceRequest != null)
                            {
                                var serviceRequestRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusion.ID && x.TableDrivedEntityID2 == serviceRequest.ID);
                                if (serviceRequestRelationship != null)
                                {
                                    var serviceRequestRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, serviceRequest, serviceRequestRelationship.ID.ToString());
                                    if (serviceRequestRelationshipTail != null)
                                    {
                                        if (customer != null)
                                        {
                                            var serviceRequestCustomerRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == customer.ID);
                                            if (serviceRequestCustomerRelationship != null)
                                            {
                                                var serviceRequestCustomerRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, customer, serviceRequestRelationship.ID.ToString() + "," + serviceRequestCustomerRelationship.ID.ToString());
                                                if (serviceRequestCustomerRelationshipTail != null)
                                                {
                                                    var customerCodeColumn = customer.Table.Column.FirstOrDefault(x => x.Name == "Code");
                                                    if (customerCodeColumn != null)
                                                    {
                                                        var serviceResuestCustomerCode = new EntitySearchColumns() { EntityRelationshipTail = serviceRequestCustomerRelationshipTail, Column = customerCodeColumn, Alias = "کد مشتری", OrderID = 3 };
                                                        serviceConclusaionSearch.EntitySearchColumns.Add(serviceResuestCustomerCode);
                                                    }
                                                    if (genericPerson != null)
                                                    {
                                                        var genericPersonNameColumn = genericPerson.Table.Column.FirstOrDefault(x => x.Name == "Name");

                                                        var serviceRequestCustomerGenericPersonRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == customer.ID && x.TableDrivedEntityID2 == genericPerson.ID);
                                                        if (serviceRequestCustomerGenericPersonRelationship != null)
                                                        {
                                                            var serviceRequestCustomerGenericPersonRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, genericPerson, serviceRequestRelationship.ID.ToString() + "," + serviceRequestCustomerRelationship.ID.ToString()
                                                                + "," + serviceRequestCustomerGenericPersonRelationship.ID.ToString());
                                                            if (serviceRequestCustomerGenericPersonRelationshipTail != null)
                                                            {
                                                                var serviceResuestCustomerName = new EntitySearchColumns() { EntityRelationshipTail = serviceRequestCustomerGenericPersonRelationshipTail, Column = genericPersonNameColumn, Alias = "نام مشتری", OrderID = 4 };
                                                                serviceConclusaionSearch.EntitySearchColumns.Add(serviceResuestCustomerName);


                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        if (productItem != null)
                                        {
                                            var serviceRequestProductItemRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == productItem.ID);
                                            if (serviceRequestProductItemRelationship != null)
                                            {
                                                var serviceRequestProductItemRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, productItem, serviceRequestRelationship.ID.ToString() + "," + serviceRequestProductItemRelationship.ID.ToString());
                                                if (serviceRequestProductItemRelationshipTail != null)
                                                {
                                                    var productItemModelColumn = productItem.Table.Column.FirstOrDefault(x => x.Name == "ProductModel");
                                                    if (productItemModelColumn != null)
                                                    {
                                                        var serviceResuestProductItemModel = new EntitySearchColumns() { EntityRelationshipTail = serviceRequestProductItemRelationshipTail, Column = productItemModelColumn, Alias = "مدل کالا", OrderID = 5 };
                                                        serviceConclusaionSearch.EntitySearchColumns.Add(serviceResuestProductItemModel);
                                                    }
                                                    var productItemBrandColumn = productItem.Table.Column.FirstOrDefault(x => x.Name == "BrandTitle");
                                                    if (productItemBrandColumn != null)
                                                    {
                                                        var serviceResuestProductItemModel = new EntitySearchColumns() { EntityRelationshipTail = serviceRequestProductItemRelationshipTail, Column = productItemBrandColumn, Alias = "برند کالا", OrderID = 6 };
                                                        serviceConclusaionSearch.EntitySearchColumns.Add(serviceResuestProductItemModel);
                                                    }
                                                    var productItemSerialColumn = productItem.Table.Column.FirstOrDefault(x => x.Name == "Serial");
                                                    if (productItemSerialColumn != null)
                                                    {
                                                        var serviceResuestProductItemModel = new EntitySearchColumns() { EntityRelationshipTail = serviceRequestProductItemRelationshipTail, Column = productItemSerialColumn, Alias = "سریال کالا", OrderID = 7 };
                                                        serviceConclusaionSearch.EntitySearchColumns.Add(serviceResuestProductItemModel);
                                                    }
                                                }
                                            }
                                        }
                                        if (office != null)
                                        {
                                            var serviceRequestOfficeRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == office.ID);
                                            if (serviceRequestOfficeRelationship != null)
                                            {
                                                var serviceRequestOfficeRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, office, serviceRequestRelationship.ID.ToString() + "," + serviceRequestOfficeRelationship.ID.ToString());
                                                if (serviceRequestOfficeRelationshipTail != null)
                                                {
                                                    var officeCodeColumn = office.Table.Column.FirstOrDefault(x => x.Name == "Code");
                                                    if (officeCodeColumn != null)
                                                    {
                                                        var serviceResuestOfficeCode = new EntitySearchColumns() { EntityRelationshipTail = serviceRequestOfficeRelationshipTail, Column = officeCodeColumn, Alias = "کد دفتر", OrderID = 8 };
                                                        serviceConclusaionSearch.EntitySearchColumns.Add(serviceResuestOfficeCode);
                                                    }
                                                    if (legalPerson != null)
                                                    {
                                                        var legalPersonNameColumn = legalPerson.Table.Column.FirstOrDefault(x => x.Name == "Name");

                                                        var serviceRequestOfficelegalPersonRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == office.ID && x.TableDrivedEntityID2 == legalPerson.ID);
                                                        if (serviceRequestOfficelegalPersonRelationship != null)
                                                        {
                                                            var serviceRequestOfficelegalPersonRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, legalPerson, serviceRequestRelationship.ID.ToString() + "," + serviceRequestOfficeRelationship.ID.ToString()
                                                                + "," + serviceRequestOfficelegalPersonRelationship.ID.ToString());
                                                            if (serviceRequestOfficelegalPersonRelationshipTail != null)
                                                            {
                                                                var serviceResuestOfficeName = new EntitySearchColumns() { EntityRelationshipTail = serviceRequestOfficelegalPersonRelationshipTail, Column = legalPersonNameColumn, Alias = "نام دفتر", OrderID = 9 };
                                                                serviceConclusaionSearch.EntitySearchColumns.Add(serviceResuestOfficeName);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var sp_CalculateCustomerValueByServiceRequestID = projectContext.DatabaseFunction.FirstOrDefault(x => x.FunctionName == "sp_CalculateCustomerValueByServiceRequestID");
                    if (sp_CalculateCustomerValueByServiceRequestID != null)
                    {
                        DatabaseFunction_TableDrivedEntity serviceConclusionDatabaseFunctionEntity = projectContext.DatabaseFunction_TableDrivedEntity.FirstOrDefault(x => x.TableDrivedEntityID == serviceConclusion.ID && x.DatabaseFunctionID == sp_CalculateCustomerValueByServiceRequestID.ID);
                        if (serviceConclusionDatabaseFunctionEntity == null)
                        {
                            serviceConclusionDatabaseFunctionEntity = new DatabaseFunction_TableDrivedEntity();
                            serviceConclusionDatabaseFunctionEntity.TableDrivedEntityID = serviceConclusion.ID;
                            serviceConclusionDatabaseFunctionEntity.DatabaseFunctionID = sp_CalculateCustomerValueByServiceRequestID.ID;
                            serviceConclusionDatabaseFunctionEntity.Title = "محاسبه ارزش مشتری بعد از حذف";
                            serviceConclusionDatabaseFunctionEntity.Name = "CustomerValueAfterDelete";
                            projectContext.DatabaseFunction_TableDrivedEntity.Add(serviceConclusionDatabaseFunctionEntity);
                        }

                        var serviceRequestIDColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "RequestServiceID");

                        if (serviceRequestIDColumn != null && serviceConclusionDatabaseFunctionEntity.ID == 0)
                        {
                            if (sp_CalculateCustomerValueByServiceRequestID.DatabaseFunctionParameter.Any(x => x.ParamName == "@serviceRequestID"))
                                serviceConclusionDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(new DatabaseFunction_TableDrivedEntity_Columns()
                                { ColumnID = serviceRequestIDColumn.ID, DatabaseFunctionParameter = sp_CalculateCustomerValueByServiceRequestID.DatabaseFunctionParameter.First(x => x.ParamName == "@serviceRequestID") });

                        }

                        var afterDeleteBackenActionActivity = projectContext.BackendActionActivity.FirstOrDefault(x => x.StepType == (short)Enum_EntityActionActivityStep.AfterDelete && x.TableDrivedEntityID == serviceConclusion.ID && x.Title == "محاسبه ارزش مشتری بعد از حذف");
                        if (afterDeleteBackenActionActivity == null)
                        {
                            afterDeleteBackenActionActivity = new BackendActionActivity();
                            afterDeleteBackenActionActivity.TableDrivedEntityID = serviceConclusion.ID;
                            afterDeleteBackenActionActivity.StepType = (short)Enum_EntityActionActivityStep.AfterDelete;
                            afterDeleteBackenActionActivity.Title = "محاسبه ارزش مشتری بعد از حذف";
                            afterDeleteBackenActionActivity.Type = 1;
                            afterDeleteBackenActionActivity.ResultSensetive = false;

                            afterDeleteBackenActionActivity.DatabaseFunction_TableDrivedEntity = serviceConclusionDatabaseFunctionEntity;
                            projectContext.BackendActionActivity.Add(afterDeleteBackenActionActivity);
                        }
                    }
                }
                try
                {
                    projectContext.SaveChanges();

                }
                catch (DbUpdateException e)
                {
                    throw e;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private EntityRelationshipTail GetRelationshipTail(MyProjectEntities projectContext, TableDrivedEntity entity, TableDrivedEntity targetEntity, string path)
        {
            if (projectContext.EntityRelationshipTail.Any(x => x.TableDrivedEntityID == entity.ID && x.RelationshipPath == path))
                return projectContext.EntityRelationshipTail.First(x => x.TableDrivedEntityID == entity.ID && x.RelationshipPath == path);
            else
            {
                var result = new EntityRelationshipTail() { TableDrivedEntityID = entity.ID, RelationshipPath = path, TargetEntityID = targetEntity.ID };
                projectContext.EntityRelationshipTail.Add(result);
                return result;
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