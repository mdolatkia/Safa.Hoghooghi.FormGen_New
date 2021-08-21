using DataAccess;
using ModelEntites;
using MyDataItemManager;
using MyDataSearchManagerBusiness;
using MyLetterGenerator;
using MyModelManager;
using ProxyLibrary;
using ProxyLibrary.Workflow;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelCustomSetting
{
    public class BizCustomSettings
    {
        public void SetCustomSettings(int databaseID, DR_Requester requester)
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
                            if (relaitonship.RelationshipType.SuperToSubRelationshipType.SuperEntityDeterminerColumnID == null)
                            {
                                relaitonship.RelationshipType.SuperToSubRelationshipType.SuperEntityDeterminerColumnID = typeColumn.ID;
                                relaitonship.RelationshipType.SuperToSubRelationshipType.SuperToSubDeterminerValue.Add(new SuperToSubDeterminerValue() { DeterminerValue = "1" });
                            }
                        }
                    }
                    if (legalPerson != null)
                    {
                        var relaitonship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == genericPerson.ID && x.TableDrivedEntityID2 == legalPerson.ID);
                        if (typeColumn != null && relaitonship != null && relaitonship.RelationshipType != null && relaitonship.RelationshipType.SuperToSubRelationshipType != null)
                        {
                            if (relaitonship.RelationshipType.SuperToSubRelationshipType.SuperEntityDeterminerColumnID == null)
                            {
                                relaitonship.RelationshipType.SuperToSubRelationshipType.SuperEntityDeterminerColumnID = typeColumn.ID;
                                relaitonship.RelationshipType.SuperToSubRelationshipType.SuperToSubDeterminerValue.Add(new SuperToSubDeterminerValue() { DeterminerValue = "2" });
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

                var serviceActionType = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceActionType" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (serviceActionType != null)
                {
                    //امکان کپی کردن لیست مقادیر اضافه شود
                    var serviceActionTypeServiceTypeEnumColumn = serviceActionType.Table.Column.FirstOrDefault(x => x.Name == "ServiceItemType");
                    if (serviceActionTypeServiceTypeEnumColumn != null)
                    {
                        if (serviceActionTypeServiceTypeEnumColumn.ColumnValueRange == null)
                        {
                            serviceActionTypeServiceTypeEnumColumn.ColumnValueRange = new ColumnValueRange();
                            serviceActionTypeServiceTypeEnumColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "1", KeyTitle = "تعمیر" });
                            serviceActionTypeServiceTypeEnumColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "2", KeyTitle = "تست" });
                            serviceActionTypeServiceTypeEnumColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "3", KeyTitle = "تعمیر و تست" });

                        }
                    }
                }
                Formula serviceItemPriceFormula = null;
                var serviceItemRepair = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceItemRepair" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                var serviceItemTest = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceItemTest" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                var serviceItemPartImage = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceItemPartImage" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                var serviceItem = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceItem" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                Column ServiceTypeEnumColumn = null;
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
                        getNowCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
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
                        getHoursSpentCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
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
                    ServiceTypeEnumColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "ServiceTypeEnum");
                    if (ServiceTypeEnumColumn != null)
                    {
                        if (ServiceTypeEnumColumn.ColumnValueRange == null)
                        {
                            ServiceTypeEnumColumn.ColumnValueRange = new ColumnValueRange();
                            ServiceTypeEnumColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "1", KeyTitle = "تعمیر" });
                            ServiceTypeEnumColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "2", KeyTitle = "تست" });
                            ServiceTypeEnumColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = "3", KeyTitle = "تعمیر و تست" });

                        }
                    }
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
                var serviceRequest_RequestType = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceRequest_RequestType" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                var serviceRequestType = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceRequestType" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                Column persianDateColumn = null;
                if (serviceRequest != null)
                {
                    persianDateColumn = serviceRequest.Table.Column.FirstOrDefault(x => x.Name == "PersianDate");
                    persianDateColumn = serviceRequest.Table.Column.FirstOrDefault(x => x.Name == "PersianDate");
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
                var serviceRequestReviewItems = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceRequestReviewItems" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                var serviceRequestReview = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceRequestReview" && x.Table.DBSchema.DatabaseInformationID == databaseID);
                if (serviceRequestReview != null)
                {
                    var autoDateColumn = serviceRequestReview.Table.Column.FirstOrDefault(x => x.Name == "AutoDate");
                    if (autoDateColumn != null)
                        autoDateColumn.DateColumnType.StringDateIsMiladi = true;
                    var autoTimeColumn = serviceRequestReview.Table.Column.FirstOrDefault(x => x.Name == "AutoTime");
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


                var conclusionIDColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "ID");
                EntityListView serviceConclusaionListView = null;
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
                    }
                    //نکته جالب
                    //اینجا اگر از projectContext.EntityListView.FirstOrDefault(x => x.Title == "لیست پیش فرض خلاصه سرویس");
                    //استفاده بشه مورد تکراری رو پیدا میکنه چون تشابه حرف ی در پایگاه داده اعمال میشه
                    //اما اگر از serviceConclusion.EntityListView.FirstOrDefault(x => x.Title == "لیست پیش فرض خلاصه سرویس");
                    //استفاده بشه اگرچه مورد موجوده اما پیدا نمیکنه چون مقایسه در کد انجام می شود و تشابه ی عربی و فارسی در نظر گرفته نمی شود
                    serviceConclusaionListView = projectContext.EntityListView.FirstOrDefault(x => x.Title == "لیست پیش فرض خلاصه سرویس");
                    if (serviceConclusaionListView == null)
                    {
                        serviceConclusaionListView = new EntityListView();
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
                            var serviceConclusionRequestRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusion.ID && x.TableDrivedEntityID2 == serviceRequest.ID);
                            if (serviceConclusionRequestRelationship != null)
                            {
                                var serviceConclusionRequestRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, serviceRequest, serviceConclusionRequestRelationship.ID.ToString());
                                if (serviceConclusionRequestRelationshipTail != null)
                                {
                                    if (customer != null)
                                    {
                                        var serviceRequestCustomerRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == customer.ID);
                                        if (serviceRequestCustomerRelationship != null)
                                        {
                                            var serviceRequestCustomerRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, customer, serviceConclusionRequestRelationship.ID.ToString() + "," + serviceRequestCustomerRelationship.ID.ToString());
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
                                                        var serviceRequestCustomerGenericPersonRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, genericPerson, serviceConclusionRequestRelationship.ID.ToString() + "," + serviceRequestCustomerRelationship.ID.ToString()
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
                                            var serviceRequestProductItemRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, productItem, serviceConclusionRequestRelationship.ID.ToString() + "," + serviceRequestProductItemRelationship.ID.ToString());
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
                                            var serviceRequestOfficeRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, office, serviceConclusionRequestRelationship.ID.ToString() + "," + serviceRequestOfficeRelationship.ID.ToString());
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
                                                        var serviceRequestOfficelegalPersonRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, legalPerson, serviceConclusionRequestRelationship.ID.ToString() + "," + serviceRequestOfficeRelationship.ID.ToString()
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
                        var finalPriceColumn = serviceConclusion.Table.Column.FirstOrDefault(x => x.Name == "FinalPrice");
                        var finalPriceListViewColumn = new EntityListViewColumns() { Column = finalPriceColumn, Alias = "مبلغ نهایی", WidthUnit = 1, IsDescriptive = false, OrderID = 10 };
                        serviceConclusaionListView.EntityListViewColumns.Add(finalPriceListViewColumn);

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
                var serviceRequestReviewItemsToRequestType = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequestReviewItems.ID && x.TableDrivedEntityID2 == serviceRequestType.ID);
                if (serviceRequestReviewItemsToRequestType != null)
                {
                    var searchFilterserviceReviewItemsToserviceRequestType = projectContext.RelationshipSearchFilter.FirstOrDefault(x => x.RelationshipID == serviceRequestReviewItemsToRequestType.ID);
                    if (searchFilterserviceReviewItemsToserviceRequestType == null)
                    {

                        var serviceReviewItemsToReviewRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequestReviewItems.ID && x.TableDrivedEntityID2 == serviceRequestReview.ID);
                        if (serviceReviewItemsToReviewRelationship != null)
                        {
                            var serviceReviewToServiceRequestRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequestReview.ID && x.TableDrivedEntityID2 == serviceRequest.ID);
                            if (serviceReviewToServiceRequestRelationship != null)
                            {
                                var serviceRequestToserviceRequest_RequestType = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == serviceRequest_RequestType.ID);
                                var serviceReviewItemsToserviceRequest_RequestTypeTail = GetRelationshipTail(projectContext, serviceRequestReviewItems, serviceRequest_RequestType, serviceReviewItemsToReviewRelationship.ID + "," +
                                  serviceReviewToServiceRequestRelationship.ID + "," + serviceRequestToserviceRequest_RequestType.ID);
                                var ServiceRequestTypeIDColumn = serviceRequest_RequestType.Table.Column.FirstOrDefault(x => x.Name == "ServiceRequestTypeID");
                                var searchColumn = serviceRequestType.Table.Column.FirstOrDefault(x => x.Name == "ID");

                                searchFilterserviceReviewItemsToserviceRequestType = new RelationshipSearchFilter();
                                projectContext.RelationshipSearchFilter.Add(searchFilterserviceReviewItemsToserviceRequestType);
                                searchFilterserviceReviewItemsToserviceRequestType.RelationshipID = serviceRequestReviewItemsToRequestType.ID;
                                searchFilterserviceReviewItemsToserviceRequestType.SearchColumnID = searchColumn.ID;
                                searchFilterserviceReviewItemsToserviceRequestType.ValueColumnID = ServiceRequestTypeIDColumn.ID;
                                searchFilterserviceReviewItemsToserviceRequestType.EntityRelationshipTail = serviceReviewItemsToserviceRequest_RequestTypeTail;
                            }
                        }
                    }
                }

                var organizationTypeTamirgah = projectContext.OrganizationType.FirstOrDefault(x => x.Name == "تعمیرگاه");
                if (organizationTypeTamirgah == null)
                {
                    organizationTypeTamirgah = new OrganizationType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationType }, Name = "تعمیرگاه" };
                    projectContext.OrganizationType.Add(organizationTypeTamirgah);
                }
                var organizationTypeDaftarKhadamat = projectContext.OrganizationType.FirstOrDefault(x => x.Name == "دفتر خدمات");
                if (organizationTypeDaftarKhadamat == null)
                {
                    organizationTypeDaftarKhadamat = new OrganizationType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationType }, Name = "دفتر خدمات" };
                    projectContext.OrganizationType.Add(organizationTypeDaftarKhadamat);
                }
                var organizationTamirgahTajrish = projectContext.Organization.FirstOrDefault(x => x.Name == "تعمیرگاه تجریش");
                if (organizationTamirgahTajrish == null)
                {
                    organizationTamirgahTajrish = new Organization() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.Organization }, Name = "تعمیرگاه تجریش", OrganizationType = organizationTypeTamirgah };
                    projectContext.Organization.Add(organizationTamirgahTajrish);
                }
                var organizationDaftarVanak = projectContext.Organization.FirstOrDefault(x => x.Name == "دفتر ونک");
                if (organizationDaftarVanak == null)
                {
                    organizationDaftarVanak = new Organization() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.Organization }, Name = "دفتر ونک", OrganizationType = organizationTypeDaftarKhadamat };
                    projectContext.Organization.Add(organizationDaftarVanak);
                }

                var roleTamirkar = projectContext.RoleType.FirstOrDefault(x => x.Name == "تعمیرکار");
                if (roleTamirkar == null)
                {
                    roleTamirkar = new RoleType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.RoleType }, Name = "تعمیرکار" };
                    projectContext.RoleType.Add(roleTamirkar);
                }
                var roleOperatorService = projectContext.RoleType.FirstOrDefault(x => x.Name == "اپراتور ثبت درخواست");
                if (roleOperatorService == null)
                {
                    roleOperatorService = new RoleType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.RoleType }, Name = "اپراتور ثبت درخواست" };
                    projectContext.RoleType.Add(roleOperatorService);
                }
                var roleRahbarEdare = projectContext.RoleType.FirstOrDefault(x => x.Name == "راهبر اداره");
                if (roleRahbarEdare == null)
                {
                    roleRahbarEdare = new RoleType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.RoleType }, Name = "راهبر اداره" };
                    projectContext.RoleType.Add(roleRahbarEdare);
                }
                var roleReviewerService = projectContext.RoleType.FirstOrDefault(x => x.Name == "بررسی کننده درخواست");
                if (roleReviewerService == null)
                {
                    roleReviewerService = new RoleType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.RoleType }, Name = "بررسی کننده درخواست" };
                    projectContext.RoleType.Add(roleReviewerService);
                }
                var roleMaaliService = projectContext.RoleType.FirstOrDefault(x => x.Name == "مسئول مالی سرویس");
                if (roleMaaliService == null)
                {
                    roleMaaliService = new RoleType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.RoleType }, Name = "مسئول مالی سرویس" };
                    projectContext.RoleType.Add(roleMaaliService);
                }
                var roleRahbarKol = projectContext.RoleType.FirstOrDefault(x => x.Name == "راهبر کل");
                if (roleRahbarKol == null)
                {
                    roleRahbarKol = new RoleType() { SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.RoleType }, Name = "راهبر کل" };
                    projectContext.RoleType.Add(roleRahbarKol);
                }

                var organizationTypeTamirgahRoleTamirkar = projectContext.OrganizationType_RoleType.FirstOrDefault(x => x.OrganizationTypeID == organizationTypeTamirgah.ID
                && x.RoleTypeID == roleTamirkar.ID);
                if (organizationTypeTamirgahRoleTamirkar == null)
                {
                    organizationTypeTamirgahRoleTamirkar = new OrganizationType_RoleType()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationTypeRoleType },
                        OrganizationType = organizationTypeTamirgah,
                        RoleType = roleTamirkar
                    };
                    projectContext.OrganizationType_RoleType.Add(organizationTypeTamirgahRoleTamirkar);
                }
                var organizationTypeTamirgahRoleRahbarEdare = projectContext.OrganizationType_RoleType.FirstOrDefault(x => x.OrganizationTypeID == organizationTypeTamirgah.ID
               && x.RoleTypeID == roleRahbarEdare.ID);
                if (organizationTypeTamirgahRoleRahbarEdare == null)
                {
                    organizationTypeTamirgahRoleRahbarEdare = new OrganizationType_RoleType()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationTypeRoleType },
                        OrganizationType = organizationTypeTamirgah,
                        RoleType = roleRahbarEdare
                    };
                    projectContext.OrganizationType_RoleType.Add(organizationTypeTamirgahRoleRahbarEdare);
                }
                var organizationTypeDaftarRoleRahbarEdare = projectContext.OrganizationType_RoleType.FirstOrDefault(x => x.OrganizationTypeID == organizationTypeDaftarKhadamat.ID
              && x.RoleTypeID == roleRahbarEdare.ID);
                if (organizationTypeDaftarRoleRahbarEdare == null)
                {
                    organizationTypeDaftarRoleRahbarEdare = new OrganizationType_RoleType()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationTypeRoleType },
                        OrganizationType = organizationTypeDaftarKhadamat,
                        RoleType = roleRahbarEdare
                    };
                    projectContext.OrganizationType_RoleType.Add(organizationTypeDaftarRoleRahbarEdare);
                }
                var organizationTypeDaftarRoleOperatorService = projectContext.OrganizationType_RoleType.FirstOrDefault(x => x.OrganizationTypeID == organizationTypeDaftarKhadamat.ID
              && x.RoleTypeID == roleOperatorService.ID);
                if (organizationTypeDaftarRoleOperatorService == null)
                {
                    organizationTypeDaftarRoleOperatorService = new OrganizationType_RoleType()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationTypeRoleType },
                        OrganizationType = organizationTypeDaftarKhadamat,
                        RoleType = roleOperatorService
                    };
                    projectContext.OrganizationType_RoleType.Add(organizationTypeDaftarRoleOperatorService);
                }
                var organizationTypeDaftarRoleMaaliService = projectContext.OrganizationType_RoleType.FirstOrDefault(x => x.OrganizationTypeID == organizationTypeDaftarKhadamat.ID
            && x.RoleTypeID == roleMaaliService.ID);
                if (organizationTypeDaftarRoleMaaliService == null)
                {
                    organizationTypeDaftarRoleMaaliService = new OrganizationType_RoleType()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationTypeRoleType },
                        OrganizationType = organizationTypeDaftarKhadamat,
                        RoleType = roleMaaliService
                    };
                    projectContext.OrganizationType_RoleType.Add(organizationTypeDaftarRoleMaaliService);
                }
                var organizationTypeDaftarRoleReviewer = projectContext.OrganizationType_RoleType.FirstOrDefault(x => x.OrganizationTypeID == organizationTypeDaftarKhadamat.ID
              && x.RoleTypeID == roleReviewerService.ID);
                if (organizationTypeDaftarRoleReviewer == null)
                {
                    organizationTypeDaftarRoleReviewer = new OrganizationType_RoleType()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationTypeRoleType },
                        OrganizationType = organizationTypeDaftarKhadamat,
                        RoleType = roleReviewerService
                    };
                    projectContext.OrganizationType_RoleType.Add(organizationTypeDaftarRoleReviewer);
                }
                var organizationTypeDaftarRoleRahbarKol = projectContext.OrganizationType_RoleType.FirstOrDefault(x => x.OrganizationTypeID == organizationTypeDaftarKhadamat.ID
             && x.RoleTypeID == roleRahbarKol.ID);
                if (organizationTypeDaftarRoleRahbarKol == null)
                {
                    organizationTypeDaftarRoleRahbarKol = new OrganizationType_RoleType()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationTypeRoleType },
                        OrganizationType = organizationTypeDaftarKhadamat,
                        RoleType = roleRahbarKol
                    };
                    projectContext.OrganizationType_RoleType.Add(organizationTypeDaftarRoleRahbarKol);
                }

                var userRezayi = projectContext.User.FirstOrDefault(x => x.UserName == "Rezayi");
                if (userRezayi == null)
                {
                    userRezayi = new User()
                    {
                        UserName = "Rezayi",
                        FirstName = "مجید",
                        LastName = "رضایی",
                        Password = "123"
                    };
                    projectContext.User.Add(userRezayi);
                }
                var userZahed = projectContext.User.FirstOrDefault(x => x.UserName == "Zahed");
                if (userZahed == null)
                {
                    userZahed = new User()
                    {
                        UserName = "Zahed",
                        FirstName = "وحید",
                        LastName = "زاهد",
                        Password = "123"
                    };
                    projectContext.User.Add(userZahed);
                }
                var userPirazad = projectContext.User.FirstOrDefault(x => x.UserName == "Pirazad");
                if (userPirazad == null)
                {
                    userPirazad = new User()
                    {
                        UserName = "Pirazad",
                        FirstName = "نرگش",
                        LastName = "پیرآزاد",
                        Password = "123"
                    };
                    projectContext.User.Add(userPirazad);
                }
                var userDolatkhah = projectContext.User.FirstOrDefault(x => x.UserName == "Dolatkhah");
                if (userDolatkhah == null)
                {
                    userDolatkhah = new User()
                    {
                        UserName = "Dolatkhah",
                        FirstName = "مریم",
                        LastName = "دولتخواه",
                        Password = "123"
                    };
                    projectContext.User.Add(userDolatkhah);
                }
                var userMovaseghi = projectContext.User.FirstOrDefault(x => x.UserName == "Movaseghi");
                if (userMovaseghi == null)
                {
                    userMovaseghi = new User()
                    {
                        UserName = "Movaseghi",
                        FirstName = "فاطمه",
                        LastName = "موثقی",
                        Password = "123"
                    };
                    projectContext.User.Add(userMovaseghi);
                }
                var userKosari = projectContext.User.FirstOrDefault(x => x.UserName == "Kosari");
                if (userKosari == null)
                {
                    userKosari = new User()
                    {
                        UserName = "Kosari",
                        FirstName = "سعید",
                        LastName = "کوثری",
                        Password = "123"
                    };
                    projectContext.User.Add(userKosari);
                }
                var userAfshari = projectContext.User.FirstOrDefault(x => x.UserName == "Afshari");
                if (userAfshari == null)
                {
                    userAfshari = new User()
                    {
                        UserName = "Afshari",
                        FirstName = "صالح",
                        LastName = "افشاری",
                        Password = "123"
                    };
                    projectContext.User.Add(userAfshari);
                }
                var userKarimi = projectContext.User.FirstOrDefault(x => x.UserName == "Karimi");
                if (userKarimi == null)
                {
                    userKarimi = new User()
                    {
                        UserName = "Karimi",
                        FirstName = "سمانه",
                        LastName = "کریمی",
                        Password = "123"
                    };
                    projectContext.User.Add(userKarimi);
                }

                var userDolatkia = projectContext.User.FirstOrDefault(x => x.UserName == "Dolatkia");
                if (userDolatkia == null)
                {
                    userDolatkia = new User()
                    {
                        UserName = "Dolatkia",
                        FirstName = "میثم",
                        LastName = "دولت کیا",
                        Password = "123"
                    };
                    projectContext.User.Add(userDolatkia);
                }

                var organizationTamirgahTajrishTamirkarPost1 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userRezayi.ID
                 && x.Name == "تعمیرکار 1");
                if (organizationTamirgahTajrishTamirkarPost1 == null)
                {
                    organizationTamirgahTajrishTamirkarPost1 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "تعمیرکار 1",
                        OrganizationType_RoleType = organizationTypeTamirgahRoleTamirkar,
                        Organization = organizationTamirgahTajrish,
                        User = userRezayi
                    };
                    projectContext.OrganizationPost.Add(organizationTamirgahTajrishTamirkarPost1);
                }
                var organizationTamirgahTajrishTamirkarPost2 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userZahed.ID
                && x.Name == "تعمیرکار 2");
                if (organizationTamirgahTajrishTamirkarPost2 == null)
                {
                    organizationTamirgahTajrishTamirkarPost2 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "تعمیرکار 2",
                        OrganizationType_RoleType = organizationTypeTamirgahRoleTamirkar,
                        Organization = organizationTamirgahTajrish,
                        User = userZahed
                    };
                    projectContext.OrganizationPost.Add(organizationTamirgahTajrishTamirkarPost2);
                }
                var organizationTamirgahTajrishTamirkarPost3 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userPirazad.ID
                && x.Name == "تعمیرکار 3");
                if (organizationTamirgahTajrishTamirkarPost3 == null)
                {
                    organizationTamirgahTajrishTamirkarPost3 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "تعمیرکار 3",
                        OrganizationType_RoleType = organizationTypeTamirgahRoleTamirkar,
                        Organization = organizationTamirgahTajrish,
                        User = userPirazad
                    };
                    projectContext.OrganizationPost.Add(organizationTamirgahTajrishTamirkarPost3);
                }
                var organizationTamirgahTajrishRahbar1 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userRezayi.ID
                && x.Name == "راهبر اداره 1");
                if (organizationTamirgahTajrishRahbar1 == null)
                {
                    organizationTamirgahTajrishRahbar1 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "راهبر اداره 1",
                        OrganizationType_RoleType = organizationTypeTamirgahRoleRahbarEdare,
                        Organization = organizationTamirgahTajrish,
                        User = userRezayi
                    };
                    projectContext.OrganizationPost.Add(organizationTamirgahTajrishRahbar1);
                }

                var organizationDaftarVanakOperator1 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userDolatkhah.ID
             && x.Name == "اپراتور 1");
                if (organizationDaftarVanakOperator1 == null)
                {
                    organizationDaftarVanakOperator1 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "اپراتور 1",
                        OrganizationType_RoleType = organizationTypeDaftarRoleOperatorService,
                        Organization = organizationDaftarVanak,
                        User = userDolatkhah
                    };
                    projectContext.OrganizationPost.Add(organizationDaftarVanakOperator1);
                }
                var organizationDaftarVanakOperator2 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userMovaseghi.ID
            && x.Name == "اپراتور 2");
                if (organizationDaftarVanakOperator2 == null)
                {
                    organizationDaftarVanakOperator2 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "اپراتور 2",
                        OrganizationType_RoleType = organizationTypeDaftarRoleOperatorService,
                        Organization = organizationDaftarVanak,
                        User = userMovaseghi
                    };
                    projectContext.OrganizationPost.Add(organizationDaftarVanakOperator2);
                }
                var organizationDaftarVanakReviewer1 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userKosari.ID
            && x.Name == "بررسی کننده درخواست 1");
                if (organizationDaftarVanakReviewer1 == null)
                {
                    organizationDaftarVanakReviewer1 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "بررسی کننده درخواست 1",
                        OrganizationType_RoleType = organizationTypeDaftarRoleReviewer,
                        Organization = organizationDaftarVanak,
                        User = userKosari
                    };
                    projectContext.OrganizationPost.Add(organizationDaftarVanakReviewer1);
                }
                var organizationDaftarVanakRahbar1 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userAfshari.ID
          && x.Name == "راهبر اداره 1");
                if (organizationDaftarVanakRahbar1 == null)
                {
                    organizationDaftarVanakRahbar1 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "راهبر اداره 1",
                        OrganizationType_RoleType = organizationTypeDaftarRoleRahbarEdare,
                        Organization = organizationDaftarVanak,
                        User = userAfshari
                    };
                    projectContext.OrganizationPost.Add(organizationDaftarVanakRahbar1);
                }
                var organizationDaftarVanakMaali1 = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userKarimi.ID
        && x.Name == "مسئول مالی سرویس 1");
                if (organizationDaftarVanakMaali1 == null)
                {
                    organizationDaftarVanakMaali1 = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "مسئول مالی سرویس 1",
                        OrganizationType_RoleType = organizationTypeDaftarRoleMaaliService,
                        Organization = organizationDaftarVanak,
                        User = userKarimi
                    };
                    projectContext.OrganizationPost.Add(organizationDaftarVanakMaali1);
                }
                var organizationDaftarVanakRahbarKol = projectContext.OrganizationPost.FirstOrDefault(x => x.UserID == userDolatkia.ID
       && x.Name == "راهبر کل 1");
                if (organizationDaftarVanakRahbarKol == null)
                {
                    organizationDaftarVanakRahbarKol = new OrganizationPost()
                    {
                        SecuritySubject = new SecuritySubject() { Type = (short)SecuritySubjectType.OrganizationPost },
                        Name = "راهبر کل 1",
                        OrganizationType_RoleType = organizationTypeDaftarRoleRahbarKol,
                        Organization = organizationDaftarVanak,
                        User = userDolatkia
                    };
                    projectContext.OrganizationPost.Add(organizationDaftarVanakRahbarKol);
                }

                if (organizationTypeTamirgah != null)
                {
                    var organizationTypeTamirgahPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == databaseID && x.SecuritySubjectID == organizationTypeTamirgah.ID);
                    if (organizationTypeTamirgahPermission == null)
                    {
                        organizationTypeTamirgahPermission = new Permission() { SecurityObjectID = databaseID, SecuritySubject = organizationTypeTamirgah.SecuritySubject };
                        organizationTypeTamirgahPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ReadOnly.ToString() });
                        organizationTypeTamirgahPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterView.ToString() });
                        organizationTypeTamirgahPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveView.ToString() });
                        projectContext.Permission.Add(organizationTypeTamirgahPermission);

                    }
                }
                if (organizationTypeDaftarKhadamat != null)
                {
                    var organizationTypeDaftarKhadamatPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == databaseID && x.SecuritySubjectID == organizationTypeDaftarKhadamat.ID);
                    if (organizationTypeDaftarKhadamatPermission == null)
                    {
                        organizationTypeDaftarKhadamatPermission = new Permission() { SecurityObjectID = databaseID, SecuritySubject = organizationTypeDaftarKhadamat.SecuritySubject };
                        organizationTypeDaftarKhadamatPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ReadOnly.ToString() });
                        organizationTypeDaftarKhadamatPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterView.ToString() });
                        organizationTypeDaftarKhadamatPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveView.ToString() });
                        projectContext.Permission.Add(organizationTypeDaftarKhadamatPermission);

                    }
                }
                var workshopSchema = projectContext.DBSchema.FirstOrDefault(x => x.Name == "Workshop");
                if (roleTamirkar != null && workshopSchema != null)
                {
                    var roleTamirkarPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == workshopSchema.ID && x.SecuritySubjectID == roleTamirkar.ID);
                    if (roleTamirkarPermission == null)
                    {
                        roleTamirkarPermission = new Permission() { SecurityObjectID = workshopSchema.ID, SecuritySubject = roleTamirkar.SecuritySubject };
                        roleTamirkarPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                        roleTamirkarPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                        roleTamirkarPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                        projectContext.Permission.Add(roleTamirkarPermission);

                    }
                }
                if (roleOperatorService != null)
                {
                    if (serviceRequest != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == serviceRequest.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = serviceRequest.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (serviceRequest_RequestType != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == serviceRequest_RequestType.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = serviceRequest_RequestType.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (customer != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == customer.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = customer.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (genericPerson != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == genericPerson.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = genericPerson.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (realPerson != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == realPerson.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = realPerson.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (legalPerson != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == legalPerson.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = legalPerson.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (genericPersonAddress != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == genericPersonAddress.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = genericPersonAddress.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (productItem != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == productItem.ID && x.SecuritySubjectID == roleOperatorService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = productItem.ID, SecuritySubject = roleOperatorService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                }
                if (roleReviewerService != null)
                {
                    if (serviceRequestReview != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == serviceRequestReview.ID && x.SecuritySubjectID == roleReviewerService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = serviceRequestReview.ID, SecuritySubject = roleReviewerService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (serviceRequestReviewItems != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == serviceRequestReviewItems.ID && x.SecuritySubjectID == roleReviewerService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = serviceRequestReviewItems.ID, SecuritySubject = roleReviewerService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                }
                var accountingSchema = projectContext.DBSchema.FirstOrDefault(x => x.Name == "Accounting");
                if (roleMaaliService != null && accountingSchema != null)
                {
                    if (serviceConclusion != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == accountingSchema.ID && x.SecuritySubjectID == roleMaaliService.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = accountingSchema.ID, SecuritySubject = roleMaaliService.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                }
                var commonSchema = projectContext.DBSchema.FirstOrDefault(x => x.Name == "Common");

                if (roleRahbarEdare != null && commonSchema != null && region != null)
                {
                    var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == commonSchema.ID && x.SecuritySubjectID == roleRahbarEdare.ID);
                    if (roleOperatorPermission == null)
                    {
                        roleOperatorPermission = new Permission() { SecurityObjectID = commonSchema.ID, SecuritySubject = roleRahbarEdare.SecuritySubject };
                        roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                        roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                        roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                        projectContext.Permission.Add(roleOperatorPermission);

                    }
                    var roleOperatorNoPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == region.ID && x.SecuritySubjectID == roleRahbarEdare.ID);
                    if (roleOperatorNoPermission == null)
                    {
                        roleOperatorNoPermission = new Permission() { SecurityObjectID = region.ID, SecuritySubject = roleRahbarEdare.SecuritySubject };
                        roleOperatorNoPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ReadOnly.ToString() });
                        projectContext.Permission.Add(roleOperatorNoPermission);

                    }
                }
                if (organizationTypeTamirgahRoleRahbarEdare != null)
                {
                    if (serviceRequestType != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == serviceRequestType.ID && x.SecuritySubjectID == organizationTypeTamirgahRoleRahbarEdare.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = serviceRequestType.ID, SecuritySubject = organizationTypeTamirgahRoleRahbarEdare.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ReadOnly.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (productItem != null)
                    {
                        var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == productItem.ID && x.SecuritySubjectID == organizationTypeTamirgahRoleRahbarEdare.ID);
                        if (roleOperatorPermission == null)
                        {
                            roleOperatorPermission = new Permission() { SecurityObjectID = productItem.ID, SecuritySubject = organizationTypeTamirgahRoleRahbarEdare.SecuritySubject };
                            roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ReadOnly.ToString() });
                            projectContext.Permission.Add(roleOperatorPermission);

                        }
                    }
                    if (customer != null)
                    {
                        var customerCodeColumn = customer.Table.Column.FirstOrDefault(x => x.Name == "Code");
                        if (customerCodeColumn != null)
                        {
                            var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == customerCodeColumn.ID && x.SecuritySubjectID == organizationTypeTamirgahRoleRahbarEdare.ID);
                            if (roleOperatorPermission == null)
                            {
                                roleOperatorPermission = new Permission() { SecurityObjectID = customerCodeColumn.ID, SecuritySubject = organizationTypeTamirgahRoleRahbarEdare.SecuritySubject };
                                roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ReadOnly.ToString() });
                                projectContext.Permission.Add(roleOperatorPermission);

                            }
                        }
                    }
                }

                if (roleRahbarKol != null)
                {
                    var roleOperatorPermission = projectContext.Permission.FirstOrDefault(x => x.SecurityObjectID == databaseID && x.SecuritySubjectID == roleRahbarKol.ID);
                    if (roleOperatorPermission == null)
                    {
                        roleOperatorPermission = new Permission() { SecurityObjectID = databaseID, SecuritySubject = roleRahbarKol.SecuritySubject };
                        roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.EditAndDelete.ToString() });
                        roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.ArchiveEdit.ToString() });
                        roleOperatorPermission.Permission_Action.Add(new Permission_Action() { Action = SecurityAction.LetterEdit.ToString() });
                        projectContext.Permission.Add(roleOperatorPermission);

                    }
                }

                var archivefolderGeneral = projectContext.ArchiveFolder.FirstOrDefault(x => x.Name == "مدارک عمومي");
                if (archivefolderGeneral == null)
                    projectContext.ArchiveFolder.Add(new ArchiveFolder() { Name = "مدارک عمومي" });
                if (serviceRequest != null)
                {
                    var archivefolderSpecific = projectContext.ArchiveFolder.FirstOrDefault(x => x.Name == "ضمانتنامه");
                    if (archivefolderSpecific == null)
                        projectContext.ArchiveFolder.Add(new ArchiveFolder() { Name = "ضمانتنامه", TableDrivedEntity = serviceRequest });
                }

                var archiveTagGeneralAsl = projectContext.ArchiveTag.FirstOrDefault(x => x.Name == "اصل");
                if (archiveTagGeneralAsl == null)
                    projectContext.ArchiveTag.Add(new ArchiveTag() { Name = "اصل" });
                var archiveTagGeneralCopy = projectContext.ArchiveTag.FirstOrDefault(x => x.Name == "کپی");
                if (archiveTagGeneralCopy == null)
                    projectContext.ArchiveTag.Add(new ArchiveTag() { Name = "کپی" });

                if (serviceRequest != null)
                {
                    var archiveTagSpecific = projectContext.ArchiveTag.FirstOrDefault(x => x.Name == "مدرک اصلي");
                    if (archiveTagSpecific == null)
                        projectContext.ArchiveTag.Add(new ArchiveTag() { Name = "مدرک اصلي", TableDrivedEntity = serviceRequest });
                }
                var serviceRequestToReviewRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == serviceRequestReview.ID);
                var serviceRequestToReviewRelationshipTail = GetRelationshipTail(projectContext, serviceRequest, serviceRequestReview, serviceRequestToReviewRelationship.ID.ToString());

                var serviceRequestServiceItemRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == serviceItem.ID);
                var serviceRequestServiceItemRelationshipTail = GetRelationshipTail(projectContext, serviceRequest, serviceItem, serviceRequestServiceItemRelationship.ID.ToString());

                var serviceRequestConclusionRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceRequest.ID && x.TableDrivedEntityID2 == serviceConclusion.ID);
                var serviceRequestConclusionRelationshipTail = GetRelationshipTail(projectContext, serviceRequest, serviceConclusion, serviceRequestConclusionRelationship.ID.ToString());

                LetterType letterTypeService = null;
                if (serviceRequest != null && serviceRequestReview != null)
                {
                    serviceRequestReview.LoadArchiveRelatedItems = true;
                    if (serviceRequestToReviewRelationship != null)
                    {

                        if (serviceRequestToReviewRelationshipTail != null)
                        {
                            var entityArchiveRelationshipTails = projectContext.EntityArchiveRelationshipTails.FirstOrDefault(x => x.TableDrivedEntityID == serviceRequest.ID && x.EntityRelationshipTailID == serviceRequestToReviewRelationshipTail.ID);
                            if (entityArchiveRelationshipTails == null)
                            {
                                projectContext.EntityArchiveRelationshipTails.Add(new EntityArchiveRelationshipTails() { TableDrivedEntityID = serviceRequest.ID, EntityRelationshipTail = serviceRequestToReviewRelationshipTail });
                            }
                            var entityLetterRelationshipTails = projectContext.EntityLetterRelationshipTails.FirstOrDefault(x => x.TableDrivedEntityID == serviceRequest.ID && x.EntityRelationshipTailID == serviceRequestToReviewRelationshipTail.ID);
                            if (entityLetterRelationshipTails == null)
                            {
                                projectContext.EntityLetterRelationshipTails.Add(new EntityLetterRelationshipTails() { TableDrivedEntityID = serviceRequest.ID, EntityRelationshipTail = serviceRequestToReviewRelationshipTail });
                            }
                        }
                        var serviceRequestReviewRelationshipTail = GetRelationshipTail(projectContext, serviceRequestReview, serviceRequest, serviceRequestToReviewRelationship.RelationshipID.ToString());
                        if (serviceRequestReviewRelationshipTail != null)
                        {
                            var entityArchiveRelationshipTails = projectContext.EntityArchiveRelationshipTails.FirstOrDefault(x => x.TableDrivedEntityID == serviceRequestReview.ID && x.EntityRelationshipTailID == serviceRequestReviewRelationshipTail.ID);
                            if (entityArchiveRelationshipTails == null)
                            {
                                projectContext.EntityArchiveRelationshipTails.Add(new EntityArchiveRelationshipTails() { TableDrivedEntityID = serviceRequestReview.ID, EntityRelationshipTail = serviceRequestReviewRelationshipTail });
                            }
                            var entityLetterRelationshipTails = projectContext.EntityLetterRelationshipTails.FirstOrDefault(x => x.TableDrivedEntityID == serviceRequestReview.ID && x.EntityRelationshipTailID == serviceRequestReviewRelationshipTail.ID);
                            if (entityLetterRelationshipTails == null)
                            {
                                projectContext.EntityLetterRelationshipTails.Add(new EntityLetterRelationshipTails() { TableDrivedEntityID = serviceRequestReview.ID, EntityRelationshipTail = serviceRequestReviewRelationshipTail });
                            }
                        }


                    }

                    letterTypeService = projectContext.LetterType.FirstOrDefault(x => x.Name == "درخواست سرویس");
                    if (letterTypeService == null)
                    {
                        projectContext.LetterType.Add(new LetterType() { Name = "درخواست سرویس", TableDrivedEntityID = serviceRequest.ID });
                    }
                }
                var letterType = projectContext.LetterType.FirstOrDefault(x => x.Name == "نوع نامشخص");
                if (letterType == null)
                {
                    projectContext.LetterType.Add(new LetterType() { Name = "نوع نامشخص" });
                }


                MainLetterTemplate simpleLetterTemplate = null;
                if (serviceRequest != null && serviceRequest.EntityListViewID != null)
                {
                    simpleLetterTemplate = projectContext.MainLetterTemplate.FirstOrDefault(x => x.LetterTemplate != null && x.LetterTemplate.Name == "قالب ساده" && x.LetterTemplate.TableDrivedEntityID == serviceRequest.ID);
                    if (simpleLetterTemplate == null)
                    {
                        string path = @"d:\files\ServiceRequestLetterTemplate.docx";
                        if (File.Exists(path))
                        {
                            simpleLetterTemplate = new MainLetterTemplate();
                            simpleLetterTemplate.LetterTemplate = new LetterTemplate();
                            simpleLetterTemplate.LetterTemplate.Name = "قالب ساده";
                            simpleLetterTemplate.LetterTemplate.TableDrivedEntityID = serviceRequest.ID;
                            simpleLetterTemplate.LetterTemplate.EntityListViewID = serviceRequest.EntityListViewID.Value;
                            simpleLetterTemplate.Content = File.ReadAllBytes(path);
                            simpleLetterTemplate.FileExtension = Path.GetExtension(path);
                            simpleLetterTemplate.Type = (short)LetterTemplateType.None;

                            var listViewColumnID = GetOrCreateListViewColumn(serviceRequest, "ID", null);
                            simpleLetterTemplate.LetterTemplate.LetterTemplatePlainField.Add(new LetterTemplatePlainField() { FieldName = "ID", EntityListViewColumns = listViewColumnID });

                            var listViewColumnPersianDate = GetOrCreateListViewColumn(serviceRequest, "PersianDate", null);

                            simpleLetterTemplate.LetterTemplate.LetterTemplatePlainField.Add(new LetterTemplatePlainField() { FieldName = "PersianDate", EntityListViewColumns = listViewColumnPersianDate });

                            projectContext.MainLetterTemplate.Add(simpleLetterTemplate);
                        }
                    }
                }
                MainLetterTemplate complexLetterTemplate = null;
                if (serviceRequestReview != null && serviceRequestReview.EntityListViewID != null)
                {
                    complexLetterTemplate = projectContext.MainLetterTemplate.FirstOrDefault(x => x.LetterTemplate != null && x.LetterTemplate.Name == "قالب بررسی درخواست" && x.LetterTemplate.TableDrivedEntityID == serviceRequestReview.ID);
                    if (complexLetterTemplate == null)
                    {
                        string path = @"d:\files\ServiceRequestReviewLetterTemplate.docx";
                        if (File.Exists(path))
                        {
                            complexLetterTemplate = new MainLetterTemplate();
                            complexLetterTemplate.LetterTemplate = new LetterTemplate();
                            complexLetterTemplate.LetterTemplate.Name = "قالب بررسی درخواست";
                            complexLetterTemplate.LetterTemplate.TableDrivedEntityID = serviceRequestReview.ID;
                            complexLetterTemplate.LetterTemplate.EntityListViewID = serviceRequestReview.EntityListViewID.Value;
                            complexLetterTemplate.Content = File.ReadAllBytes(path);
                            complexLetterTemplate.FileExtension = Path.GetExtension(path);
                            complexLetterTemplate.Type = (short)LetterTemplateType.None;

                            var listViewColumnID = GetOrCreateListViewColumn(serviceRequestReview, "RequestServiceID", null);
                            if (listViewColumnID != null)
                                complexLetterTemplate.LetterTemplate.LetterTemplatePlainField.Add(new LetterTemplatePlainField() { FieldName = "RequestID", EntityListViewColumns = listViewColumnID });

                            var reviewRel = serviceRequestReview.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceRequest.ID);
                            var servicveReqRel = serviceRequest.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceRequest_RequestType.ID);
                            var servicveReqTypeRel = serviceRequest_RequestType.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceRequestType.ID);
                            if (reviewRel != null && servicveReqRel != null && servicveReqTypeRel != null)
                            {
                                //هر LetterTemplateRelationshipField به یک پارشیال وصل میشود که پارشیال خود یک لتر تمپلیت است
                                var tail = GetRelationshipTail(projectContext, serviceRequestReview, serviceRequestType, reviewRel.ID + "," + servicveReqRel.ID + "," + servicveReqTypeRel.ID);
                                var letterTemplateRelationshipFieldWithTail = new LetterTemplateRelationshipField();
                                letterTemplateRelationshipFieldWithTail.EntityRelationshipTail = tail;
                                letterTemplateRelationshipFieldWithTail.FieldName = "Rel_SrvReqType_Start";
                                letterTemplateRelationshipFieldWithTail.PartialLetterTemplate = new PartialLetterTemplate();
                                letterTemplateRelationshipFieldWithTail.PartialLetterTemplate.LetterTemplate = new LetterTemplate();
                                letterTemplateRelationshipFieldWithTail.PartialLetterTemplate.LetterTemplate.Name = "نوع درخواست";
                                letterTemplateRelationshipFieldWithTail.PartialLetterTemplate.LetterTemplate.EntityListViewID = serviceRequestType.EntityListViewID.Value;
                                letterTemplateRelationshipFieldWithTail.PartialLetterTemplate.LetterTemplate.TableDrivedEntityID = serviceRequestType.ID;
                                var listViewColumnTitle = GetOrCreateListViewColumn(serviceRequestType, "Title", null);
                                letterTemplateRelationshipFieldWithTail.PartialLetterTemplate.LetterTemplate.LetterTemplatePlainField.Add(new LetterTemplatePlainField() { FieldName = "Title", EntityListViewColumns = listViewColumnTitle });
                                complexLetterTemplate.LetterTemplate.LetterTemplateRelationshipField.Add(letterTemplateRelationshipFieldWithTail);




                            }
                            var reviewRelToItem = serviceRequestReview.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceRequestReviewItems.ID);
                            if (reviewRelToItem != null)
                            {
                                var reviewRelToItemTail = GetRelationshipTail(projectContext, serviceRequestReview, serviceRequestReviewItems, reviewRelToItem.ID.ToString());
                                var letterTemplateRelationshipFieldDirect = new LetterTemplateRelationshipField();
                                complexLetterTemplate.LetterTemplate.LetterTemplateRelationshipField.Add(letterTemplateRelationshipFieldDirect);
                                letterTemplateRelationshipFieldDirect.EntityRelationshipTail = reviewRelToItemTail;
                                letterTemplateRelationshipFieldDirect.FieldName = "Rel_SrvReqRevItem_Start";
                                letterTemplateRelationshipFieldDirect.PartialLetterTemplate = new PartialLetterTemplate();
                                letterTemplateRelationshipFieldDirect.PartialLetterTemplate.LetterTemplate = new LetterTemplate();
                                letterTemplateRelationshipFieldDirect.PartialLetterTemplate.LetterTemplate.TableDrivedEntityID = serviceRequestReviewItems.ID;
                                letterTemplateRelationshipFieldDirect.PartialLetterTemplate.LetterTemplate.EntityListViewID = serviceRequestReviewItems.EntityListViewID.Value;
                                letterTemplateRelationshipFieldDirect.PartialLetterTemplate.LetterTemplate.Name = "قالب موارد بررسی";

                                var listViewColumnDescription = GetOrCreateListViewColumn(serviceRequestReviewItems, "Description", null);
                                letterTemplateRelationshipFieldDirect.PartialLetterTemplate.LetterTemplate.LetterTemplatePlainField.Add(new LetterTemplatePlainField() { FieldName = "Description", EntityListViewColumns = listViewColumnDescription });


                                var revItemToEmployeeRel = serviceRequestReviewItems.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == employee.ID);
                                var employeeToRealPerson = employee.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == realPerson.ID);
                                var realPersonToGenericPerson = realPerson.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == genericPerson.ID);
                                if (revItemToEmployeeRel != null && employeeToRealPerson != null)
                                {
                                    var employeeTail = GetRelationshipTail(projectContext, serviceRequestReviewItems, realPerson, revItemToEmployeeRel.ID + "," + employeeToRealPerson.ID + "," + realPersonToGenericPerson.ID);
                                    var listViewColumnEmployeeName = GetOrCreateListViewColumn(serviceRequestReviewItems, "Name", employeeTail);
                                    //    listViewColumnEmployeeName.EntityRelationshipTail = employeeTail;
                                    letterTemplateRelationshipFieldDirect.PartialLetterTemplate.LetterTemplate.LetterTemplatePlainField.Add(new LetterTemplatePlainField() { FieldName = "EmployeeName", EntityListViewColumns = listViewColumnEmployeeName });
                                }


                                var revItemToReqTypeRel = serviceRequestReviewItems.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceRequestType.ID);
                                if (revItemToReqTypeRel != null)
                                {
                                    var tail = GetRelationshipTail(projectContext, serviceRequestReviewItems, serviceRequestType, revItemToReqTypeRel.ID.ToString());
                                    var letterTemplateRelationshipField2 = new LetterTemplateRelationshipField();
                                    letterTemplateRelationshipField2.EntityRelationshipTail = tail;
                                    letterTemplateRelationshipField2.FieldName = "Rel_ReqType_Start";
                                    letterTemplateRelationshipField2.PartialLetterTemplate = new PartialLetterTemplate();
                                    letterTemplateRelationshipField2.PartialLetterTemplate.LetterTemplate = new LetterTemplate();
                                    letterTemplateRelationshipField2.PartialLetterTemplate.LetterTemplate.TableDrivedEntityID = serviceRequestType.ID;
                                    letterTemplateRelationshipField2.PartialLetterTemplate.LetterTemplate.Name = "قالب نوع درخواست";
                                    letterTemplateRelationshipField2.PartialLetterTemplate.LetterTemplate.EntityListViewID = serviceRequestType.EntityListViewID.Value;
                                    var listViewColumnTitle = GetOrCreateListViewColumn(serviceRequestType, "Title", null);
                                    letterTemplateRelationshipField2.PartialLetterTemplate.LetterTemplate.LetterTemplatePlainField.Add(new LetterTemplatePlainField() { FieldName = "ReqTypeTitle", EntityListViewColumns = listViewColumnTitle });
                                    letterTemplateRelationshipFieldDirect.PartialLetterTemplate.LetterTemplate.LetterTemplateRelationshipField.Add(letterTemplateRelationshipField2);
                                }



                            }



                            projectContext.MainLetterTemplate.Add(complexLetterTemplate);
                        }
                    }
                }

                var letterSetting = projectContext.LetterSetting.FirstOrDefault();
                if (letterSetting == null)
                {
                    letterSetting = new LetterSetting();
                    projectContext.LetterSetting.Add(letterSetting);
                }
                if (letterSetting.CodeFunction3 == null)
                {
                    var letterExternalInfoCodeFunctoin = projectContext.CodeFunction.FirstOrDefault(x => x.FunctionName == "ExternalCode");
                    if (letterExternalInfoCodeFunctoin == null)
                    {
                        letterExternalInfoCodeFunctoin = new CodeFunction();
                        letterExternalInfoCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        letterExternalInfoCodeFunctoin.ClassName = "MyTestImplLibrary.LetterCodes";
                        letterExternalInfoCodeFunctoin.FunctionName = "ExternalCode";
                        letterExternalInfoCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.LetterFunction;
                        letterExternalInfoCodeFunctoin.ReturnType = "ModelEntites.FunctionResult";
                        letterExternalInfoCodeFunctoin.Name = "اطلاعات نامه از منبع خارجی";
                    }
                    letterSetting.CodeFunction3 = letterExternalInfoCodeFunctoin;
                }
                if (letterSetting.CodeFunction1 == null)
                {
                    var letterCodeFunctoin = projectContext.CodeFunction.FirstOrDefault(x => x.FunctionName == "BeforeSave");
                    if (letterCodeFunctoin == null)
                    {
                        letterCodeFunctoin = new CodeFunction();
                        letterCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        letterCodeFunctoin.ClassName = "MyTestImplLibrary.LetterCodes";
                        letterCodeFunctoin.FunctionName = "BeforeSave";
                        letterCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.LetterFunction;
                        letterCodeFunctoin.ReturnType = "ModelEntites.FunctionResult";
                        letterCodeFunctoin.Name = "قبل از ذخیره نامه";
                    }
                    letterSetting.CodeFunction1 = letterCodeFunctoin;
                }
                if (letterSetting.CodeFunction2 == null)
                {
                    var letterCodeFunctoin = projectContext.CodeFunction.FirstOrDefault(x => x.FunctionName == "AfterSave");
                    if (letterCodeFunctoin == null)
                    {
                        letterCodeFunctoin = new CodeFunction();
                        letterCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        letterCodeFunctoin.ClassName = "MyTestImplLibrary.LetterCodes";
                        letterCodeFunctoin.FunctionName = "AfterSave";
                        letterCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.LetterFunction;
                        letterCodeFunctoin.ReturnType = "ModelEntites.FunctionResult";
                        letterCodeFunctoin.Name = "بعد از ذخیره نامه";
                    }
                    letterSetting.CodeFunction2 = letterCodeFunctoin;
                }
                if (letterSetting.CodeFunction4 == null)
                {
                    var letterCodeFunctoin = projectContext.CodeFunction.FirstOrDefault(x => x.FunctionName == "BeforeLoad");
                    if (letterCodeFunctoin == null)
                    {
                        letterCodeFunctoin = new CodeFunction();
                        letterCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        letterCodeFunctoin.ClassName = "MyTestImplLibrary.LetterCodes";
                        letterCodeFunctoin.FunctionName = "BeforeLoad";
                        letterCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.LetterFunction;
                        letterCodeFunctoin.ReturnType = "ModelEntites.FunctionResult";
                        letterCodeFunctoin.Name = "قبل از لود نامه";

                    }
                    letterSetting.CodeFunction4 = letterCodeFunctoin;
                }

                if (letterSetting.CodeFunction == null)
                {
                    var letterCodeFunctoin = projectContext.CodeFunction.FirstOrDefault(x => x.FunctionName == "ConvertToExternal");
                    if (letterCodeFunctoin == null)
                    {
                        letterCodeFunctoin = new CodeFunction();
                        letterCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                        letterCodeFunctoin.ClassName = "MyTestImplLibrary.LetterCodes";
                        letterCodeFunctoin.FunctionName = "ConvertToExternal";
                        letterCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.LetterConvert;
                        letterCodeFunctoin.ReturnType = "ProxyLibrary.LetterConvertToExternalResult";
                        letterCodeFunctoin.Name = "تبدیل به خارجی";
                    }
                    letterSetting.CodeFunction = letterCodeFunctoin;
                }


                DataMenuSetting conclusionDataMenuSetting = null;
                if (serviceConclusion != null)
                {
                    conclusionDataMenuSetting = new DataMenuSetting();
                    serviceConclusion.DataMenuSetting1 = conclusionDataMenuSetting;
                    conclusionDataMenuSetting.Name = "تنظیمات منوی صورتحساب و جزئیات";
                    conclusionDataMenuSetting.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionDataMenuSetting.EntityListViewID = serviceConclusion.EntityListViewID.Value;

                    var conclusionToItemsRelMenu = new DataMenuGridViewRelationship();
                    var conclusionToItems = serviceConclusion.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceConclusionItem.ID);
                    var tailconclusionToItemsRel = GetRelationshipTail(projectContext, serviceConclusion, serviceConclusionItem, conclusionToItems.ID.ToString());
                    conclusionToItemsRelMenu.EntityRelationshipTail = tailconclusionToItemsRel;
                    conclusionToItemsRelMenu.TargetDataMenuSettingID = null;
                    conclusionDataMenuSetting.DataMenuGridViewRelationship.Add(conclusionToItemsRelMenu);
                }
                NavigationTree mnuReportFolder = null;
                var mnuMain = projectContext.NavigationTree.FirstOrDefault(x => x.ItemIdentity == databaseID);
                if (mnuMain != null)
                {
                    var mnuDataViewFolder = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "نمای داده");
                    if (mnuDataViewFolder == null)
                    {
                        mnuDataViewFolder = new NavigationTree();
                        mnuDataViewFolder.ItemTitle = "نمای داده";
                        mnuDataViewFolder.ParentID = mnuMain.ID;
                        mnuDataViewFolder.Category = DatabaseObjectCategory.Folder.ToString();
                        projectContext.NavigationTree.Add(mnuDataViewFolder);
                    }

                    var mnuCustomerDataView = projectContext.NavigationTree.FirstOrDefault(x => x.Category == DatabaseObjectCategory.DataView.ToString() && x.ItemIdentity == customer.ID);
                    if (mnuCustomerDataView == null)
                    {
                        mnuCustomerDataView = new NavigationTree();
                        mnuCustomerDataView.ItemTitle = "نمای داده مشتری";
                        mnuCustomerDataView.NavigationTree2 = mnuDataViewFolder;
                        mnuCustomerDataView.Category = DatabaseObjectCategory.DataView.ToString();
                        mnuCustomerDataView.ItemIdentity = customer.ID;
                        mnuCustomerDataView.TableDrivedEntityID = customer.ID;
                        projectContext.NavigationTree.Add(mnuCustomerDataView);
                    }
                    var mnuCustomerDataGrid = projectContext.NavigationTree.FirstOrDefault(x => x.Category == DatabaseObjectCategory.GridView.ToString() && x.ItemIdentity == customer.ID);
                    if (mnuCustomerDataGrid == null)
                    {
                        mnuCustomerDataGrid = new NavigationTree();
                        mnuCustomerDataGrid.ItemTitle = "گرید داده مشتری";
                        mnuCustomerDataGrid.NavigationTree2 = mnuDataViewFolder;
                        mnuCustomerDataGrid.Category = DatabaseObjectCategory.GridView.ToString();
                        mnuCustomerDataGrid.ItemIdentity = customer.ID;
                        mnuCustomerDataGrid.TableDrivedEntityID = customer.ID;
                        projectContext.NavigationTree.Add(mnuCustomerDataGrid);
                    }


                    mnuReportFolder = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارشات");
                    if (mnuReportFolder == null)
                    {
                        mnuReportFolder = new NavigationTree();
                        mnuReportFolder.ItemTitle = "گزارشات";
                        mnuReportFolder.ParentID = mnuMain.ID;
                        mnuReportFolder.Category = DatabaseObjectCategory.Folder.ToString();
                        projectContext.NavigationTree.Add(mnuReportFolder);
                    }

                }



                EntitySearchColumns searchColumnAzDate = null;
                EntitySearchColumns searchColumnTaDate = null;

                var conclusionSearch = projectContext.EntitySearch.FirstOrDefault(x => x.TableDrivedEntityID == serviceConclusion.ID && x.Title == "جستجوی صورتحساب بر اساس تاریخ");
                if (conclusionSearch == null)
                {
                    conclusionSearch = new EntitySearch();
                    conclusionSearch.Title = "جستجوی صورتحساب بر اساس تاریخ";
                    conclusionSearch.TableDrivedEntityID = serviceConclusion.ID;

                    var conclusionServiceRequestRelationship = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == serviceConclusion.ID && x.TableDrivedEntityID2 == serviceRequest.ID);
                    if (conclusionServiceRequestRelationship != null)
                    {
                        var conclusionServiceRequestRelationshipTail = GetRelationshipTail(projectContext, serviceConclusion, serviceRequest, conclusionServiceRequestRelationship.ID.ToString());
                        if (conclusionServiceRequestRelationshipTail != null)
                        {

                            if (persianDateColumn != null)
                            {
                                searchColumnAzDate = new EntitySearchColumns();
                                searchColumnAzDate.EntityRelationshipTail = conclusionServiceRequestRelationshipTail;
                                searchColumnAzDate.ColumnID = persianDateColumn.ID;
                                searchColumnAzDate.Alias = "تاریخ درخواست سرویس از";
                                conclusionSearch.EntitySearchColumns.Add(searchColumnAzDate);

                                searchColumnTaDate = new EntitySearchColumns();
                                searchColumnTaDate.EntityRelationshipTail = conclusionServiceRequestRelationshipTail;
                                searchColumnTaDate.ColumnID = persianDateColumn.ID;
                                searchColumnTaDate.Alias = "تاریخ درخواست سرویس تا";
                                conclusionSearch.EntitySearchColumns.Add(searchColumnTaDate);
                            }
                        }
                    }

                    projectContext.EntitySearch.Add(conclusionSearch);
                }
                var serviceConclusionSearchRepository = projectContext.SearchRepository.FirstOrDefault(x => x.Title == "جستجوی صورتحساب از ابتدای سال");
                if (serviceConclusionSearchRepository == null)
                {
                    serviceConclusionSearchRepository = new SearchRepository();
                    serviceConclusionSearchRepository.EntitySearch = conclusionSearch;
                    serviceConclusionSearchRepository.TableDrivedEntityID = serviceConclusion.ID;
                    serviceConclusionSearchRepository.Title = "جستجوی صورتحساب از ابتدای سال";
                    serviceConclusionSearchRepository.IsSimpleSearch = true;

                    serviceConclusionSearchRepository.LogicPhrase = new LogicPhrase();

                    var phraseAz = new DataAccess.Phrase();
                    phraseAz.ColumnPhrase = new ColumnPhrase();
                    phraseAz.ColumnPhrase.ColumnID = persianDateColumn.ID;
                    if (searchColumnAzDate != null)
                        phraseAz.ColumnPhrase.EntitySearchColumns = searchColumnAzDate;
                    phraseAz.ColumnPhrase.Value = "1400/01/01";
                    phraseAz.ColumnPhrase.Operator = CommonOperator.BiggerThan.ToString();
                    serviceConclusionSearchRepository.LogicPhrase.Phrase1.Add(phraseAz);

                    var phraseTa = new DataAccess.Phrase();
                    phraseTa.ColumnPhrase = new ColumnPhrase();
                    phraseTa.ColumnPhrase.ColumnID = persianDateColumn.ID;
                    if (searchColumnTaDate != null)
                        phraseTa.ColumnPhrase.EntitySearchColumns = searchColumnTaDate;
                    phraseTa.ColumnPhrase.Value = "1400/12/01";
                    phraseTa.ColumnPhrase.Operator = CommonOperator.SmallerThan.ToString();
                    serviceConclusionSearchRepository.LogicPhrase.Phrase1.Add(phraseTa);

                    projectContext.SearchRepository.Add(serviceConclusionSearchRepository);
                }
                var dataViewReport = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش نمای داده صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (dataViewReport == null)
                {
                    dataViewReport = new EntityReport();
                    dataViewReport.Title = "گزارش نمای داده صورتحساب";
                    dataViewReport.ReportType = (short)ReportType.SearchableReport;
                    dataViewReport.SecurityObject = new SecurityObject();
                    dataViewReport.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    dataViewReport.TableDrivedEntityID = serviceConclusion.ID;
                    dataViewReport.EntitySearchableReport = new EntitySearchableReport();
                    dataViewReport.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.DataView;
                    dataViewReport.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;
                    dataViewReport.EntitySearchableReport.EntityDataViewReport = new EntityDataViewReport();
                    dataViewReport.EntitySearchableReport.EntityDataViewReport.DataMenuSetting = conclusionDataMenuSetting;



                    projectContext.EntityReport.Add(dataViewReport);

                }




                var gridViewReport = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش گرید داده صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (gridViewReport == null)
                {
                    gridViewReport = new EntityReport();
                    gridViewReport.Title = "گزارش گرید داده صورتحساب";
                    gridViewReport.ReportType = (short)ReportType.SearchableReport;
                    gridViewReport.SecurityObject = new SecurityObject();
                    gridViewReport.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    gridViewReport.TableDrivedEntityID = serviceConclusion.ID;
                    gridViewReport.EntitySearchableReport = new EntitySearchableReport();
                    gridViewReport.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.GridView;
                    gridViewReport.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    gridViewReport.EntitySearchableReport.EntityGridViewReport = new EntityGridViewReport();
                    gridViewReport.EntitySearchableReport.EntityGridViewReport.DataMenuSetting = conclusionDataMenuSetting;



                    projectContext.EntityReport.Add(gridViewReport);

                }

                var conclusionListReport = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چاپی لیست صورتحساب ساده" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (conclusionListReport == null)
                {
                    conclusionListReport = new EntityReport();
                    conclusionListReport.Title = "گزارش چاپی لیست صورتحساب ساده";
                    conclusionListReport.ReportType = (short)ReportType.SearchableReport;
                    conclusionListReport.SecurityObject = new SecurityObject();
                    conclusionListReport.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    conclusionListReport.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionListReport.EntitySearchableReport = new EntitySearchableReport();
                    conclusionListReport.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ListReport;
                    conclusionListReport.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    conclusionListReport.EntitySearchableReport.EntityListReport = new EntityListReport();
                    conclusionListReport.EntitySearchableReport.EntityListReport.EntityListView = serviceConclusaionListView;

                    projectContext.EntityReport.Add(conclusionListReport);

                }
                EntityListViewColumns listColumnBrandTitle = serviceConclusaionListView.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == "BrandTitle");
                EntityListViewColumns listColumnModel = serviceConclusaionListView.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == "ProductModel");
                var listReportGrouped = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چاپی لیست صورتحساب گروهبندی" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (listReportGrouped == null)
                {
                    listReportGrouped = new EntityReport();
                    listReportGrouped.Title = "گزارش چاپی لیست صورتحساب گروهبندی";
                    listReportGrouped.ReportType = (short)ReportType.SearchableReport;
                    listReportGrouped.SecurityObject = new SecurityObject();
                    listReportGrouped.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    listReportGrouped.TableDrivedEntityID = serviceConclusion.ID;
                    listReportGrouped.EntitySearchableReport = new EntitySearchableReport();
                    listReportGrouped.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ListReport;
                    listReportGrouped.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    listReportGrouped.EntitySearchableReport.EntityListReport = new EntityListReport();
                    listReportGrouped.EntitySearchableReport.EntityListReport.EntityListView = serviceConclusaionListView;
                    if (listColumnBrandTitle != null)
                    {
                        listReportGrouped.EntitySearchableReport.EntityListReport.ReportGroups.Add(new ReportGroups() { EntityListViewColumns = listColumnBrandTitle });
                    }
                    projectContext.EntityReport.Add(listReportGrouped);

                }


                EntityReport conclusionItemlistReport = null;
                if (serviceConclusionItem.EntityListViewID != null)
                {
                    conclusionItemlistReport = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چاپی لیست موارد صورتحساب" && x.TableDrivedEntityID == serviceConclusionItem.ID);
                    if (conclusionItemlistReport == null)
                    {
                        conclusionItemlistReport = new EntityReport();
                        conclusionItemlistReport.Title = "گزارش چاپی لیست موارد صورتحساب";
                        conclusionItemlistReport.ReportType = (short)ReportType.SearchableReport;
                        conclusionItemlistReport.SecurityObject = new SecurityObject();
                        conclusionItemlistReport.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                        conclusionItemlistReport.TableDrivedEntityID = serviceConclusionItem.ID;
                        conclusionItemlistReport.EntitySearchableReport = new EntitySearchableReport();
                        conclusionItemlistReport.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ListReport;
                        //conclusionItemlistReport.EntitySearchableReport.SearchRepositoryID=

                        conclusionItemlistReport.EntitySearchableReport.EntityListReport = new EntityListReport();
                        conclusionItemlistReport.EntitySearchableReport.EntityListReport.EntityListViewID = serviceConclusionItem.EntityListViewID.Value;

                        projectContext.EntityReport.Add(conclusionItemlistReport);

                    }
                }
                var listReportWithSubs = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چاپی لیست صورتحساب زیر گزارش" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (listReportWithSubs == null)
                {
                    listReportWithSubs = new EntityReport();
                    listReportWithSubs.Title = "گزارش چاپی لیست صورتحساب زیر گزارش";
                    listReportWithSubs.ReportType = (short)ReportType.SearchableReport;
                    listReportWithSubs.SecurityObject = new SecurityObject();
                    listReportWithSubs.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    listReportWithSubs.TableDrivedEntityID = serviceConclusion.ID;
                    listReportWithSubs.EntitySearchableReport = new EntitySearchableReport();
                    listReportWithSubs.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ListReport;
                    listReportWithSubs.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    listReportWithSubs.EntitySearchableReport.EntityListReport = new EntityListReport();
                    listReportWithSubs.EntitySearchableReport.EntityListReport.EntityListView = serviceConclusaionListView;

                    var listViewColumnID = serviceConclusaionListView.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == "ID");
                    var listViewColumnFKID = serviceConclusionItem.EntityListView1.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == "ServiceConclusionID");

                    var conclusionToItems = serviceConclusion.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceConclusionItem.ID);
                    var tailconclusionToItemsRel = GetRelationshipTail(projectContext, serviceConclusion, serviceConclusionItem, conclusionToItems.ID.ToString());


                    if (listViewColumnID != null && listViewColumnFKID != null && tailconclusionToItemsRel != null)
                    {
                        var subItem = new EntityListReportSubs();
                        subItem.EntityRelationshipTail = tailconclusionToItemsRel;
                        subItem.EntityListReportSubsColumns.Add(new EntityListReportSubsColumns() { EntityListViewColumns1 = listViewColumnID, EntityListViewColumns = listViewColumnFKID });
                        subItem.EntityListReport = conclusionItemlistReport.EntitySearchableReport.EntityListReport;
                        subItem.Title = "رابطه بر اساس شناسه صورتحساب";
                        listReportWithSubs.EntitySearchableReport.EntityListReport.EntityListReportSubs1.Add(subItem);
                    }

                    projectContext.EntityReport.Add(listReportWithSubs);

                }

                var listColumnOffice = serviceConclusaionListView.EntityListViewColumns.FirstOrDefault(x => x.Alias == "نام دفتر");
                var listColumnFinalPrice = serviceConclusaionListView.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == "FinalPrice");
                var listColumnID = serviceConclusaionListView.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == "ID");
                var conclusionChartColumn = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چارت ستونی صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (conclusionChartColumn == null)
                {
                    conclusionChartColumn = new EntityReport();
                    conclusionChartColumn.Title = "گزارش چارت ستونی صورتحساب";
                    conclusionChartColumn.ReportType = (short)ReportType.SearchableReport;
                    conclusionChartColumn.SecurityObject = new SecurityObject();
                    conclusionChartColumn.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    conclusionChartColumn.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionChartColumn.EntitySearchableReport = new EntitySearchableReport();
                    conclusionChartColumn.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ChartReport;
                    conclusionChartColumn.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    conclusionChartColumn.EntitySearchableReport.EntityChartReport = new EntityChartReport();
                    conclusionChartColumn.EntitySearchableReport.EntityChartReport.ChartType = (short)ChartType.Column;
                    conclusionChartColumn.EntitySearchableReport.EntityChartReport.EntityListView = serviceConclusaionListView;

                    conclusionChartColumn.EntitySearchableReport.EntityChartReport.CharetReportCategories.Add(new CharetReportCategories() { EntityListViewColumns = listColumnBrandTitle });
                    conclusionChartColumn.EntitySearchableReport.EntityChartReport.CharetReportSeries.Add(new CharetReportSeries() { EntityListViewColumns = listColumnOffice });
                    conclusionChartColumn.EntitySearchableReport.EntityChartReport.CharetReportValues.Add(new CharetReportValues() { FunctoinType = (short)ChartReportValueFunction.Count, EntityListViewColumns = listColumnID });

                    projectContext.EntityReport.Add(conclusionChartColumn);
                }
                var conclusionChartPie = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چارت دایره ای صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (conclusionChartPie == null)
                {
                    conclusionChartPie = new EntityReport();
                    conclusionChartPie.Title = "گزارش چارت دایره ای صورتحساب";
                    conclusionChartPie.ReportType = (short)ReportType.SearchableReport;
                    conclusionChartPie.SecurityObject = new SecurityObject();
                    conclusionChartPie.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    conclusionChartPie.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionChartPie.EntitySearchableReport = new EntitySearchableReport();
                    conclusionChartPie.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ChartReport;
                    conclusionChartPie.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    conclusionChartPie.EntitySearchableReport.EntityChartReport = new EntityChartReport();
                    conclusionChartPie.EntitySearchableReport.EntityChartReport.ChartType = (short)ChartType.Pie;
                    conclusionChartPie.EntitySearchableReport.EntityChartReport.EntityListView = serviceConclusaionListView;

                    // conclusionChartPie.EntitySearchableReport.EntityChartReport.CharetReportCategories.Add(new CharetReportCategories() { EntityListViewColumns = listColumnBrandTitle });
                    conclusionChartPie.EntitySearchableReport.EntityChartReport.CharetReportSeries.Add(new CharetReportSeries() { EntityListViewColumns = listColumnBrandTitle });
                    conclusionChartPie.EntitySearchableReport.EntityChartReport.CharetReportValues.Add(new CharetReportValues() { FunctoinType = (short)ChartReportValueFunction.Count, EntityListViewColumns = listColumnID });

                    projectContext.EntityReport.Add(conclusionChartPie);
                }

                var conclusionChartLine = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چارت خطی صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (conclusionChartLine == null)
                {
                    conclusionChartLine = new EntityReport();
                    conclusionChartLine.Title = "گزارش چارت خطی صورتحساب";
                    conclusionChartLine.ReportType = (short)ReportType.SearchableReport;
                    conclusionChartLine.SecurityObject = new SecurityObject();
                    conclusionChartLine.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    conclusionChartLine.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionChartLine.EntitySearchableReport = new EntitySearchableReport();
                    conclusionChartLine.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ChartReport;
                    conclusionChartLine.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    conclusionChartLine.EntitySearchableReport.EntityChartReport = new EntityChartReport();
                    conclusionChartLine.EntitySearchableReport.EntityChartReport.ChartType = (short)ChartType.Line;
                    conclusionChartLine.EntitySearchableReport.EntityChartReport.EntityListView = serviceConclusaionListView;

                    conclusionChartLine.EntitySearchableReport.EntityChartReport.CharetReportCategories.Add(new CharetReportCategories() { EntityListViewColumns = listColumnBrandTitle });
                    conclusionChartLine.EntitySearchableReport.EntityChartReport.CharetReportSeries.Add(new CharetReportSeries() { EntityListViewColumns = listColumnOffice });
                    conclusionChartLine.EntitySearchableReport.EntityChartReport.CharetReportValues.Add(new CharetReportValues() { FunctoinType = (short)ChartReportValueFunction.Count, EntityListViewColumns = listColumnID });

                    projectContext.EntityReport.Add(conclusionChartLine);
                }

                var conclusionChartRadar = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش چارت رادار صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (conclusionChartRadar == null)
                {
                    conclusionChartRadar = new EntityReport();
                    conclusionChartRadar.Title = "گزارش چارت رادار صورتحساب";
                    conclusionChartRadar.ReportType = (short)ReportType.SearchableReport;
                    conclusionChartRadar.SecurityObject = new SecurityObject();
                    conclusionChartRadar.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    conclusionChartRadar.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionChartRadar.EntitySearchableReport = new EntitySearchableReport();
                    conclusionChartRadar.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ChartReport;
                    conclusionChartRadar.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    conclusionChartRadar.EntitySearchableReport.EntityChartReport = new EntityChartReport();
                    conclusionChartRadar.EntitySearchableReport.EntityChartReport.ChartType = (short)ChartType.Radar;
                    conclusionChartRadar.EntitySearchableReport.EntityChartReport.EntityListView = serviceConclusaionListView;

                    conclusionChartRadar.EntitySearchableReport.EntityChartReport.CharetReportCategories.Add(new CharetReportCategories() { EntityListViewColumns = listColumnBrandTitle });
                    conclusionChartRadar.EntitySearchableReport.EntityChartReport.CharetReportSeries.Add(new CharetReportSeries() { EntityListViewColumns = listColumnOffice });
                    conclusionChartRadar.EntitySearchableReport.EntityChartReport.CharetReportValues.Add(new CharetReportValues() { FunctoinType = (short)ChartReportValueFunction.Count, EntityListViewColumns = listColumnID });

                    projectContext.EntityReport.Add(conclusionChartRadar);
                }

                var conclusionCrossTab = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش کراس تب صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (conclusionCrossTab == null)
                {
                    conclusionCrossTab = new EntityReport();
                    conclusionCrossTab.Title = "گزارش کراس تب صورتحساب";
                    conclusionCrossTab.ReportType = (short)ReportType.SearchableReport;
                    conclusionCrossTab.SecurityObject = new SecurityObject();
                    conclusionCrossTab.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    conclusionCrossTab.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionCrossTab.EntitySearchableReport = new EntitySearchableReport();
                    conclusionCrossTab.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.CrosstabReport;
                    conclusionCrossTab.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;

                    conclusionCrossTab.EntitySearchableReport.EntityCrosstabReport = new EntityCrosstabReport();
                    conclusionCrossTab.EntitySearchableReport.EntityCrosstabReport.EntityListView = serviceConclusaionListView;

                    conclusionCrossTab.EntitySearchableReport.EntityCrosstabReport.CrosstabReportColumns.Add(new CrosstabReportColumns() { EntityListViewColumns = listColumnBrandTitle });
                    conclusionCrossTab.EntitySearchableReport.EntityCrosstabReport.CrosstabReportColumns.Add(new CrosstabReportColumns() { EntityListViewColumns = listColumnModel });
                    conclusionCrossTab.EntitySearchableReport.EntityCrosstabReport.CrosstabReportRows.Add(new CrosstabReportRows() { EntityListViewColumns = listColumnOffice });
                    conclusionCrossTab.EntitySearchableReport.EntityCrosstabReport.CrosstabReportValues.Add(new CrosstabReportValues() { EntityListViewColumns = listColumnID });

                    projectContext.EntityReport.Add(conclusionCrossTab);
                }


                var customerDirectReport = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش خارجی مستقیم مشتری" && x.TableDrivedEntityID == customer.ID);
                if (customerDirectReport == null)
                {
                    customerDirectReport = new EntityReport();
                    customerDirectReport.Title = "گزارش خارجی مستقیم مشتری";
                    customerDirectReport.ReportType = (short)ReportType.DirectReport;
                    customerDirectReport.SecurityObject = new SecurityObject();
                    customerDirectReport.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    customerDirectReport.TableDrivedEntityID = customer.ID;
                    customerDirectReport.EntityDirectlReport = new EntityDirectlReport();
                    customerDirectReport.EntityDirectlReport.URL = "http://dolatkiam/ReportServer/Pages/ReportViewer.aspx?%2fRequestConclusionsByCustomerID";
                    customerDirectReport.EntityDirectlReport.EntityDirectlReportParameters.Add(
                        new EntityDirectlReportParameters()
                        {
                            ColumnID = customer.Table.Column.FirstOrDefault(x => x.PrimaryKey).ID,
                            ParameterName = "CustomerID"
                        });
                    projectContext.EntityReport.Add(customerDirectReport);
                }

                var conclusionExternalReport = projectContext.EntityReport.FirstOrDefault(x => x.Title == "گزارش خارجی صورتحساب" && x.TableDrivedEntityID == serviceConclusion.ID);
                if (conclusionExternalReport == null)
                {
                    conclusionExternalReport = new EntityReport();
                    conclusionExternalReport.Title = "گزارش خارجی صورتحساب";
                    conclusionExternalReport.ReportType = (short)ReportType.SearchableReport;
                    conclusionExternalReport.SecurityObject = new SecurityObject();
                    conclusionExternalReport.SecurityObject.Type = (short)DatabaseObjectCategory.Report;
                    conclusionExternalReport.TableDrivedEntityID = serviceConclusion.ID;
                    conclusionExternalReport.EntitySearchableReport = new EntitySearchableReport();
                    conclusionExternalReport.EntitySearchableReport.SearchableReportType = (short)SearchableReportType.ExternalReport;
                    conclusionExternalReport.EntitySearchableReport.SearchRepository = serviceConclusionSearchRepository;
                    conclusionExternalReport.EntitySearchableReport.EntityExternalReport = new EntityExternalReport();
                    conclusionExternalReport.EntitySearchableReport.EntityExternalReport.URL = "http://dolatkiam/ReportServer/Pages/ReportViewer.aspx?%2fRequestConclusionsByReportKey";

                    projectContext.EntityReport.Add(conclusionExternalReport);

                    BizEntityExternalReport bizEntityExternalReport = new BizEntityExternalReport();
                    bizEntityExternalReport.CreateReportTable(requester, serviceConclusion.ID);
                }


                EntityRelationshipTail tailCustomerToConclusion = null;
                DataMenuSetting customerDataMenuSetting = null;
                if (customer != null && customer.EntityListViewID != null)
                {
                    customerDataMenuSetting = projectContext.DataMenuSetting.FirstOrDefault(x => x.TableDrivedEntityID == customer.ID && x.Name == "تنظیمات منوی مشتری و صورتحساب");
                    if (customerDataMenuSetting == null)
                    {
                        customerDataMenuSetting = new DataMenuSetting();
                        customerDataMenuSetting.Name = "تنظیمات منوی مشتری و صورتحساب";
                        customerDataMenuSetting.TableDrivedEntityID = customer.ID;
                        customerDataMenuSetting.EntityListViewID = customer.EntityListViewID.Value;
                        projectContext.DataMenuSetting.Add(customerDataMenuSetting);
                        var customerToServiceRequest = customer.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceRequest.ID);
                        var serviceRequestToServiceConclusion = serviceRequest.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceConclusion.ID);
                        tailCustomerToConclusion = GetRelationshipTail(projectContext, customer, serviceConclusion, customerToServiceRequest.ID + "," + serviceRequestToServiceConclusion.ID);

                        var dataViewRel = new DataMenuDataViewRelationship();
                        dataViewRel.EntityRelationshipTail = tailCustomerToConclusion;
                        dataViewRel.DataMenuSetting1 = conclusionDataMenuSetting;
                        customerDataMenuSetting.DataMenuDataViewRelationship.Add(dataViewRel);

                        var dataGridRel = new DataMenuGridViewRelationship();
                        dataGridRel.EntityRelationshipTail = tailCustomerToConclusion;
                        dataGridRel.DataMenuSetting1 = conclusionDataMenuSetting;
                        customerDataMenuSetting.DataMenuGridViewRelationship.Add(dataGridRel);

                        var conclusionDataViewReportRel = new DataMenuReportRelationship();
                        conclusionDataViewReportRel.EntityRelationshipTail = tailCustomerToConclusion;
                        conclusionDataViewReportRel.EntitySearchableReport = dataViewReport.EntitySearchableReport;
                        customerDataMenuSetting.DataMenuReportRelationship.Add(conclusionDataViewReportRel);

                        var conclusionListReportRel = new DataMenuReportRelationship();
                        conclusionListReportRel.EntityRelationshipTail = tailCustomerToConclusion;
                        conclusionListReportRel.EntitySearchableReport = conclusionListReport.EntitySearchableReport;
                        customerDataMenuSetting.DataMenuReportRelationship.Add(conclusionListReportRel);

                        var conclusionChartReportRel = new DataMenuReportRelationship();
                        conclusionChartReportRel.EntityRelationshipTail = tailCustomerToConclusion;
                        conclusionChartReportRel.EntitySearchableReport = conclusionChartPie.EntitySearchableReport;
                        customerDataMenuSetting.DataMenuReportRelationship.Add(conclusionChartReportRel);


                        var directReportSetting = new DataMenuDirectReportRelationship();
                        directReportSetting.EntityDirectlReport = customerDirectReport.EntityDirectlReport;
                        customerDataMenuSetting.DataMenuDirectReportRelationship.Add(directReportSetting);
                    }


                    customer.DataMenuSetting1 = customerDataMenuSetting;

                }


                if (!projectContext.EntityValidation.Any(x => x.TableDrivedEntityID == serviceRequest.ID && x.Title == "اعتبارسنجی درخواست سرویس"))
                {
                    EntityValidation validation = new EntityValidation();
                    validation.Title = "اعتبارسنجی درخواست سرویس";
                    validation.TableDrivedEntityID = serviceRequest.ID;



                    var validationCodeFunctoin = new CodeFunction();
                    validationCodeFunctoin.Path = @"E:\Safa.Hoghooghi.FormGen_New\MyTestImplLibrary\bin\Debug\MyTestImplLibrary.dll";
                    validationCodeFunctoin.ClassName = "MyTestImplLibrary.ValidationTest";
                    validationCodeFunctoin.FunctionName = "ValidateServiceRequest";
                    validationCodeFunctoin.Type = (short)Enum_CodeFunctionParamType.OneDataItem;
                    validationCodeFunctoin.ReturnType = "FunctionResult";
                    validationCodeFunctoin.Name = "اعتبارسنجی درخواست سرویس";
                    projectContext.CodeFunction.Add(validationCodeFunctoin);

                    var validationFormula = new Formula();
                    validationFormula.CodeFunction = validationCodeFunctoin;
                    validationFormula.TableDrivedEntityID = serviceRequest.ID;
                    validationFormula.Name = "اعتبارسنجی درخواست سرویس";
                    validationFormula.ResultType = "FunctionResult";
                    validationFormula.Title = "اعتبارسنجی درخواست سرویس";
                    projectContext.Formula.Add(validationFormula);

                    validation.Formula = validationFormula;
                    projectContext.EntityValidation.Add(validation);
                }
                TableDrivedEntity serviceRepair = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceRepair");
                TableDrivedEntity serviceTest = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == "ServiceTest");
                Tuple<Relationship, Relationship> serviceRepairRelationship = null;
                Tuple<Relationship, Relationship> serviceTestRelationship = null;

                if (serviceRepair == null)
                {
                    serviceRepair = new TableDrivedEntity();
                    serviceRepair.Name = "ServiceRepair";
                    serviceRepair.TableID = serviceItem.TableID;
                    serviceRepair.Alias = "سرويس تعمير";
                    serviceRepair.IndependentDataEntry = true;
                    serviceRepair.SecurityObject = new SecurityObject();
                    serviceRepair.SecurityObject.Type = (int)DatabaseObjectCategory.Entity;
                    projectContext.TableDrivedEntity.Add(serviceRepair);

                    serviceTest = new TableDrivedEntity();
                    serviceTest.Name = "ServiceTest";
                    serviceTest.TableID = serviceItem.TableID;
                    serviceTest.Alias = "سرويس تست";
                    serviceTest.IndependentDataEntry = true;
                    serviceTest.SecurityObject = new SecurityObject();
                    serviceTest.SecurityObject.Type = (int)DatabaseObjectCategory.Entity;
                    projectContext.TableDrivedEntity.Add(serviceTest);

                    var serviceItemToRequestPart = serviceItem.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == requestProductPart.ID);
                    if (serviceItemToRequestPart != null)
                    {
                        serviceItemToRequestPart.TableDrivedEntity = serviceRepair;
                        serviceItemToRequestPart.Relationship2.TableDrivedEntity1 = serviceRepair;
                        serviceItemToRequestPart.Relationship2.Alias = serviceRepair.Name;

                        serviceItemToRequestPart.Name = serviceRepair.Name + ">" + requestProductPart.Name;
                        serviceItemToRequestPart.Relationship2.Name = requestProductPart.Name + ">" + serviceRepair.Name;
                    }

                    var serviceItemToServiceItemRepair = serviceItem.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceItemRepair.ID);
                    if (serviceItemToServiceItemRepair != null)
                    {
                        serviceItemToServiceItemRepair.TableDrivedEntity = serviceRepair;
                        serviceItemToServiceItemRepair.Relationship2.TableDrivedEntity1 = serviceRepair;
                        serviceItemToServiceItemRepair.Relationship2.Alias = serviceRepair.Name;

                        serviceItemToServiceItemRepair.Name = serviceRepair.Name + ">" + serviceItemRepair.Name;
                        serviceItemToServiceItemRepair.Relationship2.Name = serviceItemRepair.Name + ">" + serviceRepair.Name;
                    }

                    var serviceItemToServiceItemPartImage = serviceItem.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceItemPartImage.ID);
                    if (serviceItemToServiceItemPartImage != null)
                    {
                        serviceItemToServiceItemPartImage.TableDrivedEntity = serviceRepair;
                        serviceItemToServiceItemPartImage.Relationship2.TableDrivedEntity1 = serviceRepair;
                        serviceItemToServiceItemPartImage.Relationship2.Alias = serviceRepair.Name;

                        serviceItemToServiceItemPartImage.Name = serviceRepair.Name + ">" + serviceItemPartImage.Name;
                        serviceItemToServiceItemPartImage.Relationship2.Name = serviceItemPartImage.Name + ">" + serviceRepair.Name;
                    }
                    var serviceItemToServiceItemTest = serviceItem.Relationship.FirstOrDefault(x => x.TableDrivedEntityID2 == serviceItemTest.ID);
                    if (serviceItemToServiceItemTest != null)
                    {
                        serviceItemToServiceItemTest.TableDrivedEntity = serviceTest;
                        serviceItemToServiceItemTest.Relationship2.TableDrivedEntity1 = serviceTest;
                        serviceItemToServiceItemTest.Relationship2.Alias = serviceTest.Name;

                        serviceItemToServiceItemTest.Name = serviceTest.Name + ">" + serviceItemTest.Name;
                        serviceItemToServiceItemTest.Relationship2.Name = serviceItemTest.Name + ">" + serviceTest.Name;
                    }

                    var serviceItemRepairPartTotalPriceColumn = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "RepairPartTotalPrice");
                    if (serviceItemRepairPartTotalPriceColumn != null)
                    {
                        serviceRepair.TableDrivedEntity_Columns.Add(new TableDrivedEntity_Columns() { ColumnID = serviceItemRepairPartTotalPriceColumn.ID });
                    }
                    var serviceItemID = serviceItem.Table.Column.FirstOrDefault(x => x.Name == "ID");
                    if (serviceItemID != null)
                    {
                        serviceRepair.TableDrivedEntity_Columns.Add(new TableDrivedEntity_Columns() { ColumnID = serviceItemID.ID });
                        serviceTest.TableDrivedEntity_Columns.Add(new TableDrivedEntity_Columns() { ColumnID = serviceItemID.ID });
                    }
                    foreach (var column in serviceItem.Table.Column)
                    {
                        if (column.ID != serviceItemRepairPartTotalPriceColumn.ID)
                            serviceItem.TableDrivedEntity_Columns.Add(new TableDrivedEntity_Columns() { ColumnID = column.ID });

                    }

                    var isaRelationship = new ISARelationship();
                    isaRelationship.Name = "ISAinTable" + "_" + serviceItem.Name;
                    isaRelationship.InternalTable = true;
                    isaRelationship.IsTolatParticipation = true;
                    isaRelationship.IsDisjoint = false;
                    projectContext.ISARelationship.Add(isaRelationship);

                    var relaitonship = new Relationship();
                    relaitonship.SecurityObject = new SecurityObject();
                    relaitonship.SecurityObject.Type = (int)DatabaseObjectCategory.Relationship;
                    relaitonship.Name = serviceItem.Name + ">" + serviceRepair.Name;
                    relaitonship.Alias = serviceRepair.Name;
                    relaitonship.TableDrivedEntity = serviceItem;
                    relaitonship.TableDrivedEntity1 = serviceRepair;
                    relaitonship.RelationshipColumns.Add(new RelationshipColumns() { FirstSideColumnID = serviceItemID.ID, SecondSideColumnID = serviceItemID.ID });
                    relaitonship.RelationshipType = new RelationshipType();
                    relaitonship.RelationshipType.PKToFKDataEntryEnabled = true;
                    relaitonship.RelationshipType.IsOtherSideCreatable = true;
                    relaitonship.RelationshipType.IsOtherSideDirectlyCreatable = true;
                    relaitonship.RelationshipType.SuperToSubRelationshipType = new SuperToSubRelationshipType() { ISARelationship = isaRelationship };
                    relaitonship.RelationshipType.SuperToSubRelationshipType.SuperEntityDeterminerColumnID = ServiceTypeEnumColumn.ID;
                    relaitonship.RelationshipType.SuperToSubRelationshipType.SuperToSubDeterminerValue.Add(new SuperToSubDeterminerValue() { DeterminerValue = "1" });
                    relaitonship.RelationshipType.SuperToSubRelationshipType.SuperToSubDeterminerValue.Add(new SuperToSubDeterminerValue() { DeterminerValue = "3" });

                    relaitonship.TypeEnum = (int)Enum_RelationshipType.SuperToSub;
                    relaitonship.MasterTypeEnum = (int)Enum_MasterRelationshipType.FromPrimartyToForeign;
                    projectContext.Relationship.Add(relaitonship);

                    var reverserelaitonship = new Relationship();
                    //reverserelaitonship.Enabled = true;
                    // reverserelaitonship.DataEntryEnabled = true;
                    reverserelaitonship.Name = serviceRepair.Name + ">" + serviceItem.Name;
                    reverserelaitonship.Alias = serviceItem.Alias;
                    reverserelaitonship.SecurityObject = new SecurityObject();
                    reverserelaitonship.SecurityObject.Type = (int)DatabaseObjectCategory.Relationship;
                    reverserelaitonship.TableDrivedEntity = serviceRepair;
                    reverserelaitonship.TableDrivedEntity1 = serviceItem;
                    reverserelaitonship.RelationshipColumns.Add(new RelationshipColumns() { FirstSideColumnID = serviceItemID.ID, SecondSideColumnID = serviceItemID.ID });
                    reverserelaitonship.RelationshipType = new RelationshipType();
                    reverserelaitonship.RelationshipType.IsOtherSideMandatory = true;
                    reverserelaitonship.RelationshipType.IsOtherSideCreatable = true;
                    reverserelaitonship.RelationshipType.IsOtherSideDirectlyCreatable = true;
                    reverserelaitonship.RelationshipType.SubToSuperRelationshipType = new SubToSuperRelationshipType() { ISARelationship = isaRelationship };
                    reverserelaitonship.TypeEnum = (int)Enum_RelationshipType.SubToSuper;
                    reverserelaitonship.MasterTypeEnum = (int)Enum_MasterRelationshipType.FromForeignToPrimary;
                    projectContext.Relationship.Add(reverserelaitonship);

                    serviceRepairRelationship = new Tuple<Relationship, Relationship>(relaitonship, reverserelaitonship);


                    var relaitonshipTest = new Relationship();
                    relaitonshipTest.SecurityObject = new SecurityObject();
                    relaitonshipTest.SecurityObject.Type = (int)DatabaseObjectCategory.Relationship;
                    relaitonshipTest.Name = serviceItem.Name + ">" + serviceTest.Name;
                    relaitonshipTest.Alias = serviceTest.Name;
                    relaitonshipTest.TableDrivedEntity = serviceItem;
                    relaitonshipTest.TableDrivedEntity1 = serviceTest;
                    relaitonshipTest.RelationshipColumns.Add(new RelationshipColumns() { FirstSideColumnID = serviceItemID.ID, SecondSideColumnID = serviceItemID.ID });
                    relaitonshipTest.RelationshipType = new RelationshipType();
                    relaitonshipTest.RelationshipType.PKToFKDataEntryEnabled = true;
                    relaitonshipTest.RelationshipType.IsOtherSideCreatable = true;
                    relaitonshipTest.RelationshipType.IsOtherSideDirectlyCreatable = true;
                    relaitonshipTest.RelationshipType.SuperToSubRelationshipType = new SuperToSubRelationshipType() { ISARelationship = isaRelationship };
                    relaitonshipTest.RelationshipType.SuperToSubRelationshipType.SuperEntityDeterminerColumnID = ServiceTypeEnumColumn.ID;
                    relaitonshipTest.RelationshipType.SuperToSubRelationshipType.SuperToSubDeterminerValue.Add(new SuperToSubDeterminerValue() { DeterminerValue = "2" });
                    relaitonshipTest.RelationshipType.SuperToSubRelationshipType.SuperToSubDeterminerValue.Add(new SuperToSubDeterminerValue() { DeterminerValue = "3" });

                    relaitonshipTest.TypeEnum = (int)Enum_RelationshipType.SuperToSub;
                    relaitonshipTest.MasterTypeEnum = (int)Enum_MasterRelationshipType.FromPrimartyToForeign;
                    projectContext.Relationship.Add(relaitonshipTest);

                    var reverserelaitonshipTest = new Relationship();
                    //reverserelaitonshipTest.Enabled = true;
                    // reverserelaitonshipTest.DataEntryEnabled = true;
                    reverserelaitonshipTest.Name = serviceTest.Name + ">" + serviceItem.Name;
                    reverserelaitonshipTest.Alias = serviceItem.Alias;
                    reverserelaitonshipTest.SecurityObject = new SecurityObject();
                    reverserelaitonshipTest.SecurityObject.Type = (int)DatabaseObjectCategory.Relationship;
                    reverserelaitonshipTest.TableDrivedEntity = serviceTest;
                    reverserelaitonshipTest.TableDrivedEntity1 = serviceItem;
                    reverserelaitonshipTest.RelationshipColumns.Add(new RelationshipColumns() { FirstSideColumnID = serviceItemID.ID, SecondSideColumnID = serviceItemID.ID });
                    reverserelaitonshipTest.RelationshipType = new RelationshipType();
                    reverserelaitonshipTest.RelationshipType.IsOtherSideMandatory = true;
                    reverserelaitonshipTest.RelationshipType.IsOtherSideCreatable = true;
                    reverserelaitonshipTest.RelationshipType.IsOtherSideDirectlyCreatable = true;
                    reverserelaitonshipTest.RelationshipType.SubToSuperRelationshipType = new SubToSuperRelationshipType() { ISARelationship = isaRelationship };
                    reverserelaitonshipTest.TypeEnum = (int)Enum_RelationshipType.SubToSuper;
                    reverserelaitonshipTest.MasterTypeEnum = (int)Enum_MasterRelationshipType.FromForeignToPrimary;
                    projectContext.Relationship.Add(reverserelaitonshipTest);

                    serviceTestRelationship = new Tuple<Relationship, Relationship>(relaitonshipTest, reverserelaitonshipTest);
                }

                var process = projectContext.Process.FirstOrDefault(x => x.Name == "جریان کار درخواست سرویس" && x.TableDrivedEntityID == serviceRequest.ID);
                State stateFirst = null;
                State stateReview = null;
                State stateService = null;
                State stateConclusion = null;
                State stateEnd = null;
                State stateCanceled = null;
                //State stateNotServicable = null;

                bool processIsNew = false;
                if (process == null)
                {
                    processIsNew = true;
                    process = new Process();
                    process.Name = "جریان کار درخواست سرویس";
                    process.TableDrivedEntityID = serviceRequest.ID;
                    projectContext.Process.Add(process);

                    var entityGroupRequest = new EntityGroup();
                    entityGroupRequest.Name = "ثبت درخواست";
                    process.EntityGroup.Add(entityGroupRequest);
                    entityGroupRequest.EntityGroup_Relationship.Add(new EntityGroup_Relationship() { EntityRelationshipTail = null });

                    var entityGroupReview = new EntityGroup();
                    entityGroupReview.Name = "بررسی درخواست";
                    process.EntityGroup.Add(entityGroupReview);
                    entityGroupReview.EntityGroup_Relationship.Add(new EntityGroup_Relationship() { EntityRelationshipTail = serviceRequestToReviewRelationshipTail });


                    var entityGroupService = new EntityGroup();
                    entityGroupService.Name = "ثبت تعمیرات";
                    process.EntityGroup.Add(entityGroupService);
                    entityGroupService.EntityGroup_Relationship.Add(new EntityGroup_Relationship() { EntityRelationshipTail = serviceRequestServiceItemRelationshipTail });


                    var entityGroupConclusion = new EntityGroup();
                    entityGroupConclusion.Name = "ثبت صورتحساب";
                    process.EntityGroup.Add(entityGroupConclusion);
                    entityGroupService.EntityGroup_Relationship.Add(new EntityGroup_Relationship() { EntityRelationshipTail = serviceRequestConclusionRelationshipTail });


                    stateFirst = new State();
                    stateFirst.Name = "ثبت درخواست";
                    stateFirst.StateTypeID = (short)StateType.Start;
                    process.State.Add(stateFirst);
                    var firstStateFormula = new State_Formula();
                    var hasReviewFormula = projectContext.Formula.FirstOrDefault(x => x.Name == "HasReview");
                    if (hasReviewFormula == null)
                    {
                        hasReviewFormula = new Formula();
                        hasReviewFormula.ResultType = "System.Boolean";
                        hasReviewFormula.Name = "HasReview";
                        hasReviewFormula.Title = "بررسي سرويس ندارد";
                        hasReviewFormula.TableDrivedEntityID = serviceRequest.ID;
                        hasReviewFormula.LinearFormula = new LinearFormula();
                        hasReviewFormula.LinearFormula.FormulaText = "x.OTO_ServiceRequestReview!=null";
                        projectContext.Formula.Add(hasReviewFormula);
                    }
                    firstStateFormula.Formula = hasReviewFormula;
                    firstStateFormula.TrueFalse = false;
                    firstStateFormula.Message = "این درخواست قبلا بررسی شده است";
                    stateFirst.State_Formula.Add(firstStateFormula);


                    stateReview = new State();
                    stateReview.Name = "بررسي درخواست";
                    stateReview.StateTypeID = (short)StateType.Normal;
                    process.State.Add(stateReview);

                    stateService = new State();
                    stateService.Name = "در حال سرويس مي باشد";
                    stateService.StateTypeID = (short)StateType.Normal;
                    process.State.Add(stateService);

                    stateConclusion = new State();
                    stateConclusion.Name = "در حال صدور صورتحساب مي باشد";
                    stateConclusion.StateTypeID = (short)StateType.Normal;
                    process.State.Add(stateConclusion);

                    stateEnd = new State();
                    stateEnd.Name = "صورتحساب تسويه شد و اتمام کار";
                    stateEnd.StateTypeID = (short)StateType.End;
                    process.State.Add(stateEnd);

                    var clearedDatabaseFunction = projectContext.DatabaseFunction.FirstOrDefault(x => x.FunctionName == "ServiceRequestIsValidToClosed");
                    if (clearedDatabaseFunction != null)
                    {
                        var endStateFormula = new State_Formula();
                        var clearedFormula = projectContext.Formula.FirstOrDefault(x => x.Name == "ServiceRequestCleared");
                        if (clearedFormula == null)
                        {
                            clearedFormula = new Formula();
                            clearedFormula.ResultType = "System.Boolean";
                            clearedFormula.Title = "سرویس تسویه شده است";
                            clearedFormula.Name = "Cleared";
                            clearedFormula.TableDrivedEntityID = serviceRequest.ID;
                            clearedFormula.DatabaseFunction_TableDrivedEntity = new DatabaseFunction_TableDrivedEntity();
                            clearedFormula.DatabaseFunction_TableDrivedEntity.DatabaseFunction = clearedDatabaseFunction;
                            clearedFormula.DatabaseFunction_TableDrivedEntity.Name = "ServiceRequestCleared";
                            clearedFormula.DatabaseFunction_TableDrivedEntity.TableDrivedEntityID = serviceRequest.ID;
                            var idColumn = serviceRequest.Table.Column.FirstOrDefault(x => x.Name == "ID");
                            var parameterColumn = clearedDatabaseFunction.DatabaseFunctionParameter.FirstOrDefault(x => x.ParamName == "@ServiceRequestID");
                            clearedFormula.DatabaseFunction_TableDrivedEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(
                                new DatabaseFunction_TableDrivedEntity_Columns() { ColumnID = idColumn.ID, DatabaseFunctionParameterID = parameterColumn.ID });
                            projectContext.Formula.Add(clearedFormula);
                        }
                        endStateFormula.Formula = clearedFormula;
                        endStateFormula.TrueFalse = true;
                        endStateFormula.Message = "این درخواست تسویه شده است";
                        stateEnd.State_Formula.Add(endStateFormula);
                    }

                    stateCanceled = new State();
                    stateCanceled.Name = "درخواست لغو شد و اتمام کار";
                    stateCanceled.StateTypeID = (short)StateType.End;
                    process.State.Add(stateCanceled);

                    //stateNotServicable = new State();
                    //stateNotServicable.Name = "قابل تعمير نمي باشد و اتمام";
                    //stateNotServicable.StateTypeID = (short)StateType.End;
                    //process.State.Add(stateNotServicable);




                    var stateFirstTostateReview = new Transition();
                    stateFirstTostateReview.Name = "ارسال به بررسي درخواست";
                    stateFirstTostateReview.State = stateFirst;
                    stateFirstTostateReview.State1 = stateReview;
                    process.Transition.Add(stateFirstTostateReview);
                    stateFirstTostateReview.TransitionAction.Add(new TransitionAction() { Name = "تاييد و ارسال به بررسي درخواست", ActionTypeID = (short)ActionType.Approve });
                    stateFirstTostateReview.TransitionAction.First().TransitionAction_EntityGroup.Add(new TransitionAction_EntityGroup() { EntityGroup = entityGroupRequest });
                    stateFirstTostateReview.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarEdare });
                    stateFirstTostateReview.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarKol });
                    stateFirstTostateReview.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleOperatorService });



                    var stateReviewTostateService = new Transition();
                    stateReviewTostateService.Name = "ارسال به تعميرات";
                    stateReviewTostateService.State = stateReview;
                    stateReviewTostateService.State1 = stateService;
                    process.Transition.Add(stateReviewTostateService);
                    stateReviewTostateService.TransitionAction.Add(new TransitionAction() { Name = "تاييد و ارسال به تعميرات", ActionTypeID = (short)ActionType.Approve });
                    stateReviewTostateService.TransitionAction.First().TransitionAction_EntityGroup.Add(new TransitionAction_EntityGroup() { EntityGroup = entityGroupReview });
                    stateReviewTostateService.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarEdare });
                    stateReviewTostateService.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarKol });
                    stateReviewTostateService.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleReviewerService });

                    stateReviewTostateService.TransitionAction.First().TransitionAction_Formula.Add(new TransitionAction_Formula()
                    {
                        Formula = hasReviewFormula,
                        Message = "بررسي درخواست ثبت نشده است",
                        TrueFalse = true
                    });


                    var stateServiceTostateConclusion = new Transition();
                    stateServiceTostateConclusion.Name = "ارسال به صورتحساب";
                    stateServiceTostateConclusion.State = stateService;
                    stateServiceTostateConclusion.State1 = stateConclusion;
                    process.Transition.Add(stateServiceTostateConclusion);
                    stateServiceTostateConclusion.TransitionAction.Add(new TransitionAction() { Name = "تاييد و ارسال به صورتحساب", MultipleUserEnabled = true, ActionTypeID = (short)ActionType.Approve });
                    stateServiceTostateConclusion.TransitionAction.First().TransitionAction_EntityGroup.Add(new TransitionAction_EntityGroup() { EntityGroup = entityGroupService });
                    stateServiceTostateConclusion.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarEdare });
                    stateServiceTostateConclusion.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarKol });
                    stateServiceTostateConclusion.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleTamirkar });


                    var stateConclusionTostateEnd = new Transition();
                    stateConclusionTostateEnd.Name = "اتمام جريان کار";
                    stateConclusionTostateEnd.State = stateConclusion;
                    stateConclusionTostateEnd.State1 = stateEnd;
                    process.Transition.Add(stateConclusionTostateEnd);
                    stateConclusionTostateEnd.TransitionAction.Add(new TransitionAction() { Name = "اتمام جريان کار", ActionTypeID = (short)ActionType.Approve });
                    stateConclusionTostateEnd.TransitionAction.First().TransitionAction_EntityGroup.Add(new TransitionAction_EntityGroup() { EntityGroup = entityGroupConclusion });
                    stateConclusionTostateEnd.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarEdare });
                    stateConclusionTostateEnd.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarKol });
                    stateConclusionTostateEnd.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleMaaliService });



                    var stateReviewTostateCanceled = new Transition();
                    stateReviewTostateCanceled.Name = "درخواست قابل اجرا نمي باشد";
                    stateReviewTostateCanceled.State = stateReview;
                    stateReviewTostateCanceled.State1 = stateCanceled;
                    process.Transition.Add(stateReviewTostateCanceled);
                    stateReviewTostateCanceled.TransitionAction.Add(new TransitionAction() { Name = "درخواست قابل اجرا نمي باشد", ActionTypeID = (short)ActionType.Cancel });
                    stateReviewTostateCanceled.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarEdare });
                    stateReviewTostateCanceled.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarKol });
                    stateReviewTostateCanceled.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleReviewerService });


                    var stateServiceTostateNotServicable = new Transition();
                    stateServiceTostateNotServicable.Name = "قابل تعمیر نمی باشد";
                    stateServiceTostateNotServicable.State = stateService;
                    stateServiceTostateNotServicable.State1 = stateReview;
                    process.Transition.Add(stateServiceTostateNotServicable);
                    stateServiceTostateNotServicable.TransitionAction.Add(new TransitionAction() { Name = "قابل تعمیر نمی باشد", ActionTypeID = (short)ActionType.Cancel });
                    stateServiceTostateNotServicable.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarEdare });
                    stateServiceTostateNotServicable.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarKol });
                    stateServiceTostateNotServicable.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleTamirkar });



                    var stateReviewTostateFirst = new Transition();
                    stateReviewTostateFirst.Name = "بازگشت به ثبت سرویس";
                    stateReviewTostateFirst.State = stateReview;
                    stateReviewTostateFirst.State1 = stateFirst;
                    process.Transition.Add(stateReviewTostateFirst);
                    stateReviewTostateFirst.TransitionAction.Add(new TransitionAction() { Name = "بازگشت به ثبت سرویس", ActionTypeID = (short)ActionType.Deny });
                    stateReviewTostateFirst.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarEdare });
                    stateReviewTostateFirst.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleRahbarKol });
                    stateReviewTostateFirst.TransitionAction.First().TransitionActionTarget.Add(new TransitionActionTarget() { TargetType = (short)TargetType.RoleMembers, RoleType = roleReviewerService });


                }



                try
                {
                    projectContext.SaveChanges();


                    if (serviceRepairRelationship != null)
                    {
                        serviceRepairRelationship.Item1.RelationshipID = serviceRepairRelationship.Item2.ID;
                        serviceRepairRelationship.Item2.RelationshipID = serviceRepairRelationship.Item1.ID;
                        serviceRepair.SuperToSubRelationshipType = serviceRepairRelationship.Item1.RelationshipType.SuperToSubRelationshipType;


                        filter
                    }
                    if (serviceTestRelationship != null)
                    {
                        serviceTestRelationship.Item1.RelationshipID = serviceTestRelationship.Item2.ID;
                        serviceTestRelationship.Item2.RelationshipID = serviceTestRelationship.Item1.ID;
                        serviceTest.SuperToSubRelationshipType = serviceTestRelationship.Item1.RelationshipType.SuperToSubRelationshipType;
                    }



                    if (processIsNew)
                    {

                        process.TransitionFlowSTR = @"<?xml version=""1.0"" encoding=""utf - 8""?><RadDiagram Version=""2020.2""><Metadata Type=""Telerik.Windows.Controls.RadDiagram"" Id=""dfd25c17 - f7e3 - 4e86 - 8fd6 - 8b4605cd6308"" Zoom=""1"" Position=""50; 279""><Title>Diagram [7/31/2021 11:49:53 AM]</Title><Description></Description></Metadata><Groups /><Shapes QNs=""MyProject_WPF, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null; ""><StateShape Type=""MyProject_WPF.Diagram.StateShape"" Id=""23b3ba9b - 903a - 43eb - 91b4 - ea16019a9db8"" ZIndex=""2"" Position=""1100; -100"" Background=""#FFFF0000"" Size=""100;60"" RotationAngle=""0"" MinWidth=""0"" MinHeight=""0"" MaxWidth=""Infinity"" MaxHeight=""Infinity"" UseDefaultConnectors=""True"" QN=""0"" StateID=""stateFirst"" Title=""ثبت درخواست""><Connectors QNs=""Telerik.Windows.Controls.Diagrams, Version=2020.2.513.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7;""><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Auto"" Offset=""0.5;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Left"" Offset=""0;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Top"" Offset=""0.5;0"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Right"" Offset=""1;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Bottom"" Offset=""0.5;1"" QN=""0"" /></Connectors></StateShape><StateShape Type=""MyProject_WPF.Diagram.StateShape"" Id=""a0caccdc-1424-44ed-98cd-6f825abca3ab"" ZIndex=""2"" Position=""840;-100"" Size=""100;60"" RotationAngle=""0"" MinWidth=""0"" MinHeight=""0"" MaxWidth=""Infinity"" MaxHeight=""Infinity"" UseDefaultConnectors=""True"" QN=""0"" StateID=""stateReview"" Title=""بررسي درخواست""><Connectors QNs=""Telerik.Windows.Controls.Diagrams, Version=2020.2.513.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7;""><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Auto"" Offset=""0.5;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Left"" Offset=""0;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Top"" Offset=""0.5;0"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Right"" Offset=""1;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Bottom"" Offset=""0.5;1"" QN=""0"" /></Connectors></StateShape><StateShape Type=""MyProject_WPF.Diagram.StateShape"" Id=""302637ea-08f9-4e91-b8ba-b4247b87d129"" ZIndex=""2"" Position=""580;-100"" Size=""100;60"" RotationAngle=""0"" MinWidth=""0"" MinHeight=""0"" MaxWidth=""Infinity"" MaxHeight=""Infinity"" UseDefaultConnectors=""True"" QN=""0"" StateID=""stateService"" Title=""در حال سرويس مي باشد""><Connectors QNs=""Telerik.Windows.Controls.Diagrams, Version=2020.2.513.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7;""><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Auto"" Offset=""0.5;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Left"" Offset=""0;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Top"" Offset=""0.5;0"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Right"" Offset=""1;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Bottom"" Offset=""0.5;1"" QN=""0"" /></Connectors></StateShape><StateShape Type=""MyProject_WPF.Diagram.StateShape"" Id=""9b6c3168-da3a-4255-a0a5-b51bb7152070"" ZIndex=""2"" Position=""340;-100"" Size=""100;60"" RotationAngle=""0"" MinWidth=""0"" MinHeight=""0"" MaxWidth=""Infinity"" MaxHeight=""Infinity"" UseDefaultConnectors=""True"" QN=""0"" StateID=""stateConclusion"" Title=""در حال صدور صورتحساب مي باشد""><Connectors QNs=""Telerik.Windows.Controls.Diagrams, Version=2020.2.513.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7;""><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Auto"" Offset=""0.5;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Left"" Offset=""0;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Top"" Offset=""0.5;0"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Right"" Offset=""1;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Bottom"" Offset=""0.5;1"" QN=""0"" /></Connectors></StateShape><StateShape Type=""MyProject_WPF.Diagram.StateShape"" Id=""76c2cb54-6d47-49b1-a52c-f46d851c9545"" ZIndex=""2"" Position=""100;-100"" Background=""#FFFF0000"" Size=""100;60"" RotationAngle=""0"" MinWidth=""0"" MinHeight=""0"" MaxWidth=""Infinity"" MaxHeight=""Infinity"" UseDefaultConnectors=""True"" QN=""0"" StateID=""stateEnd"" Title=""صورتحساب تسويه شد و اتمام کار""><Connectors QNs=""Telerik.Windows.Controls.Diagrams, Version=2020.2.513.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7;""><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Auto"" Offset=""0.5;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Left"" Offset=""0;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Top"" Offset=""0.5;0"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Right"" Offset=""1;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Bottom"" Offset=""0.5;1"" QN=""0"" /></Connectors></StateShape><StateShape Type=""MyProject_WPF.Diagram.StateShape"" Id=""21bf6a45-0c4d-41b2-9d01-dd8779ecf292"" ZIndex=""2"" Position=""840;120"" Background=""#FFFF0000"" Size=""100;60"" RotationAngle=""0"" MinWidth=""0"" MinHeight=""0"" MaxWidth=""Infinity"" MaxHeight=""Infinity"" UseDefaultConnectors=""True"" QN=""0"" StateID=""stateCanceled"" Title=""درخواست لغو شد و اتمام کار""><Connectors QNs=""Telerik.Windows.Controls.Diagrams, Version=2020.2.513.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7;""><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Auto"" Offset=""0.5;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Left"" Offset=""0;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Top"" Offset=""0.5;0"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Right"" Offset=""1;0.5"" QN=""0"" /><RadDiagramConnector Type=""Telerik.Windows.Controls.Diagrams.RadDiagramConnector"" Name=""Bottom"" Offset=""0.5;1"" QN=""0"" /></Connectors></StateShape></Shapes><Connections QNs=""Telerik.Windows.Controls.Diagrams, Version=2020.2.513.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7;""><RadDiagramConnection Type=""Telerik.Windows.Controls.RadDiagramConnection"" Id=""ca28dc0a-c62d-40f6-9f56-d6651b40246e"" ZIndex=""1"" Position=""940;-70"" IsRotationEnabled=""False"" IsResizingEnabled=""False"" SourceConnectorPosition=""Auto"" TargetConnectorPosition=""Auto"" SourceCapType=""None"" TargetCapType=""Arrow1Filled"" SourceCapSize=""7;7"" TargetCapSize=""7;7"" StartPoint=""1100;-70"" Source=""23b3ba9b-903a-43eb-91b4-ea16019a9db8"" EndPoint=""940;-70"" Target=""a0caccdc-1424-44ed-98cd-6f825abca3ab"" ConnectionType=""Polyline"" UseFreeConnectors=""False"" ConnectionPoints="""" IsModified=""False"" QN=""0"" /><RadDiagramConnection Type=""Telerik.Windows.Controls.RadDiagramConnection"" Id=""8a637286-d20b-4d6b-8509-3f54b7672a23"" ZIndex=""1"" Position=""680;-70"" IsRotationEnabled=""False"" IsResizingEnabled=""False"" SourceConnectorPosition=""Auto"" TargetConnectorPosition=""Auto"" SourceCapType=""None"" TargetCapType=""Arrow1Filled"" SourceCapSize=""7;7"" TargetCapSize=""7;7"" StartPoint=""840;-70"" Source=""a0caccdc-1424-44ed-98cd-6f825abca3ab"" EndPoint=""680;-70"" Target=""302637ea-08f9-4e91-b8ba-b4247b87d129"" ConnectionType=""Polyline"" UseFreeConnectors=""False"" ConnectionPoints="""" IsModified=""False"" QN=""0"" /><RadDiagramConnection Type=""Telerik.Windows.Controls.RadDiagramConnection"" Id=""8f17ed01-5485-4e05-95b2-0a161801997a"" ZIndex=""1"" Position=""440;-70"" IsRotationEnabled=""False"" IsResizingEnabled=""False"" SourceConnectorPosition=""Auto"" TargetConnectorPosition=""Auto"" SourceCapType=""None"" TargetCapType=""Arrow1Filled"" SourceCapSize=""7;7"" TargetCapSize=""7;7"" StartPoint=""580;-70"" Source=""302637ea-08f9-4e91-b8ba-b4247b87d129"" EndPoint=""440;-70"" Target=""9b6c3168-da3a-4255-a0a5-b51bb7152070"" ConnectionType=""Polyline"" UseFreeConnectors=""False"" ConnectionPoints="""" IsModified=""False"" QN=""0"" /><RadDiagramConnection Type=""Telerik.Windows.Controls.RadDiagramConnection"" Id=""2d97676e-ba58-4580-bbea-e56af9b78fbc"" ZIndex=""1"" Position=""200;-70"" IsRotationEnabled=""False"" IsResizingEnabled=""False"" SourceConnectorPosition=""Auto"" TargetConnectorPosition=""Auto"" SourceCapType=""None"" TargetCapType=""Arrow1Filled"" SourceCapSize=""7;7"" TargetCapSize=""7;7"" StartPoint=""340;-70"" Source=""9b6c3168-da3a-4255-a0a5-b51bb7152070"" EndPoint=""200;-70"" Target=""76c2cb54-6d47-49b1-a52c-f46d851c9545"" ConnectionType=""Polyline"" UseFreeConnectors=""False"" ConnectionPoints="""" IsModified=""False"" QN=""0"" /><RadDiagramConnection Type=""Telerik.Windows.Controls.RadDiagramConnection"" Id=""0e7b550f-3064-4d7a-a5a2-6cfe478304e2"" ZIndex=""1"" Position=""890;-40"" IsRotationEnabled=""False"" IsResizingEnabled=""False"" SourceConnectorPosition=""Auto"" TargetConnectorPosition=""Auto"" SourceCapType=""None"" TargetCapType=""Arrow1Filled"" SourceCapSize=""7;7"" TargetCapSize=""7;7"" StartPoint=""890;-40"" Source=""a0caccdc-1424-44ed-98cd-6f825abca3ab"" EndPoint=""890;120"" Target=""21bf6a45-0c4d-41b2-9d01-dd8779ecf292"" ConnectionType=""Polyline"" UseFreeConnectors=""False"" ConnectionPoints="""" IsModified=""False"" QN=""0"" /><RadDiagramConnection Type=""Telerik.Windows.Controls.RadDiagramConnection"" Id=""e7d8e6f7-ede1-4db7-80a7-9a4c1a156f9c"" ZIndex=""1"" Position=""940;-70"" IsRotationEnabled=""False"" IsResizingEnabled=""False"" SourceConnectorPosition=""Auto"" TargetConnectorPosition=""Auto"" SourceCapType=""None"" TargetCapType=""Arrow1Filled"" SourceCapSize=""7;7"" TargetCapSize=""7;7"" StartPoint=""940;-70"" Source=""a0caccdc-1424-44ed-98cd-6f825abca3ab"" EndPoint=""1100;-70"" Target=""23b3ba9b-903a-43eb-91b4-ea16019a9db8"" ConnectionType=""Spline"" UseFreeConnectors=""False"" ConnectionPoints=""1020;0"" IsModified=""False"" QN=""0"" /><RadDiagramConnection Type=""Telerik.Windows.Controls.RadDiagramConnection"" Id=""2c9449af-44e0-4b31-9a2b-ee696b010258"" ZIndex=""1"" Position=""680;-70"" IsRotationEnabled=""False"" IsResizingEnabled=""False"" SourceConnectorPosition=""Auto"" TargetConnectorPosition=""Auto"" SourceCapType=""None"" TargetCapType=""Arrow1Filled"" SourceCapSize=""7;7"" TargetCapSize=""7;7"" StartPoint=""680;-70"" Source=""302637ea-08f9-4e91-b8ba-b4247b87d129"" EndPoint=""840;-70"" Target=""a0caccdc-1424-44ed-98cd-6f825abca3ab"" ConnectionType=""Spline"" UseFreeConnectors=""False"" ConnectionPoints=""760;0"" IsModified=""False"" QN=""0"" /></Connections></RadDiagram>";
                        process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateFirst", stateFirst.ID.ToString());
                        process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateReview", stateReview.ID.ToString());
                        process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateService", stateService.ID.ToString());
                        process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateConclusion", stateConclusion.ID.ToString());
                        process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateEnd", stateEnd.ID.ToString());
                        process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateCanceled", stateCanceled.ID.ToString());
                        //  process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateNotServicable", stateNotServicable.ID.ToString());
                        //process.TransitionFlowSTR = process.TransitionFlowSTR.Replace("stateFirst", stateFirst.ID.ToString());
                    }

                    var mnuConclustionDataViewReport = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش نمای داده صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == dataViewReport.ID);
                    if (mnuConclustionDataViewReport == null)
                    {
                        mnuConclustionDataViewReport = new NavigationTree();
                        mnuConclustionDataViewReport.ItemTitle = "گزارش نمای داده صورتحساب";
                        mnuConclustionDataViewReport.NavigationTree2 = mnuReportFolder;
                        mnuConclustionDataViewReport.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionDataViewReport.ItemIdentity = dataViewReport.ID;
                        mnuConclustionDataViewReport.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionDataViewReport);
                    }

                    var mnuConclustiongridViewReport = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش گرید داده صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == gridViewReport.ID);
                    if (mnuConclustiongridViewReport == null)
                    {
                        mnuConclustiongridViewReport = new NavigationTree();
                        mnuConclustiongridViewReport.ItemTitle = "گزارش گرید داده صورتحساب";
                        mnuConclustiongridViewReport.NavigationTree2 = mnuReportFolder;
                        mnuConclustiongridViewReport.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustiongridViewReport.ItemIdentity = gridViewReport.ID;
                        mnuConclustiongridViewReport.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustiongridViewReport);
                    }

                    var mnuConclustionListReport = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش چاپی صورتحساب ساده" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == conclusionListReport.ID);
                    if (mnuConclustionListReport == null)
                    {
                        mnuConclustionListReport = new NavigationTree();
                        mnuConclustionListReport.ItemTitle = "گزارش چاپی صورتحساب ساده";
                        mnuConclustionListReport.NavigationTree2 = mnuReportFolder;
                        mnuConclustionListReport.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionListReport.ItemIdentity = conclusionListReport.ID;
                        mnuConclustionListReport.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionListReport);
                    }

                    var mnuConclustionListReportGrouped = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش چاپی صورتحساب گروهبندی" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == listReportGrouped.ID);
                    if (mnuConclustionListReportGrouped == null)
                    {
                        mnuConclustionListReportGrouped = new NavigationTree();
                        mnuConclustionListReportGrouped.ItemTitle = "گزارش چاپی صورتحساب گروهبندی";
                        mnuConclustionListReportGrouped.NavigationTree2 = mnuReportFolder;
                        mnuConclustionListReportGrouped.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionListReportGrouped.ItemIdentity = listReportGrouped.ID;
                        mnuConclustionListReportGrouped.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionListReportGrouped);
                    }

                    var mnuConclustionListReportWithSub = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش چاپی صورتحساب زیر گزارش" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == listReportWithSubs.ID);
                    if (mnuConclustionListReportWithSub == null)
                    {
                        mnuConclustionListReportWithSub = new NavigationTree();
                        mnuConclustionListReportWithSub.ItemTitle = "گزارش چاپی صورتحساب زیر گزارش";
                        mnuConclustionListReportWithSub.NavigationTree2 = mnuReportFolder;
                        mnuConclustionListReportWithSub.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionListReportWithSub.ItemIdentity = listReportWithSubs.ID;
                        mnuConclustionListReportWithSub.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionListReportWithSub);
                    }

                    var mnuConclustionChartColumn = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش چارت ستونی صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == conclusionChartColumn.ID);
                    if (mnuConclustionChartColumn == null)
                    {
                        mnuConclustionChartColumn = new NavigationTree();
                        mnuConclustionChartColumn.ItemTitle = "گزارش چارت ستونی صورتحساب";
                        mnuConclustionChartColumn.NavigationTree2 = mnuReportFolder;
                        mnuConclustionChartColumn.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionChartColumn.ItemIdentity = conclusionChartColumn.ID;
                        mnuConclustionChartColumn.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionChartColumn);
                    }

                    var mnuConclustionChartPie = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش چارت دایره ای صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == conclusionChartPie.ID);
                    if (mnuConclustionChartPie == null)
                    {
                        mnuConclustionChartPie = new NavigationTree();
                        mnuConclustionChartPie.ItemTitle = "گزارش چارت دایره ای صورتحساب";
                        mnuConclustionChartPie.NavigationTree2 = mnuReportFolder;
                        mnuConclustionChartPie.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionChartPie.ItemIdentity = conclusionChartPie.ID;
                        mnuConclustionChartPie.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionChartPie);
                    }

                    var mnuConclustionChartLine = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش چارت خطی صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == conclusionChartLine.ID);
                    if (mnuConclustionChartLine == null)
                    {
                        mnuConclustionChartLine = new NavigationTree();
                        mnuConclustionChartLine.ItemTitle = "گزارش چارت خطی صورتحساب";
                        mnuConclustionChartLine.NavigationTree2 = mnuReportFolder;
                        mnuConclustionChartLine.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionChartLine.ItemIdentity = conclusionChartLine.ID;
                        mnuConclustionChartLine.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionChartLine);
                    }

                    var mnuConclustionChartRadar = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش چارت رادار صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == conclusionChartRadar.ID);
                    if (mnuConclustionChartRadar == null)
                    {
                        mnuConclustionChartRadar = new NavigationTree();
                        mnuConclustionChartRadar.ItemTitle = "گزارش چارت رادار صورتحساب";
                        mnuConclustionChartRadar.NavigationTree2 = mnuReportFolder;
                        mnuConclustionChartRadar.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionChartRadar.ItemIdentity = conclusionChartRadar.ID;
                        mnuConclustionChartRadar.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionChartRadar);
                    }

                    var mnuConclustionCrossTab = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش کراس تب صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == conclusionCrossTab.ID);
                    if (mnuConclustionCrossTab == null)
                    {
                        mnuConclustionCrossTab = new NavigationTree();
                        mnuConclustionCrossTab.ItemTitle = "گزارش کراس تب صورتحساب";
                        mnuConclustionCrossTab.NavigationTree2 = mnuReportFolder;
                        mnuConclustionCrossTab.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionCrossTab.ItemIdentity = conclusionCrossTab.ID;
                        mnuConclustionCrossTab.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionCrossTab);
                    }
                    var mnuConclustionExternalReport = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش خارجی صورتحساب" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == conclusionExternalReport.ID);
                    if (mnuConclustionExternalReport == null)
                    {
                        mnuConclustionExternalReport = new NavigationTree();
                        mnuConclustionExternalReport.ItemTitle = "گزارش خارجی صورتحساب";
                        mnuConclustionExternalReport.NavigationTree2 = mnuReportFolder;
                        mnuConclustionExternalReport.Category = DatabaseObjectCategory.Report.ToString();
                        mnuConclustionExternalReport.ItemIdentity = conclusionExternalReport.ID;
                        mnuConclustionExternalReport.TableDrivedEntityID = serviceConclusion.ID;
                        projectContext.NavigationTree.Add(mnuConclustionExternalReport);
                    }

                    var mnuCustomerDirectReport = projectContext.NavigationTree.FirstOrDefault(x => x.ItemTitle == "گزارش خارجی مستقیم مشتری" && x.Category == DatabaseObjectCategory.Report.ToString() && x.ItemIdentity == customerDirectReport.ID);
                    if (mnuCustomerDirectReport == null)
                    {
                        mnuCustomerDirectReport = new NavigationTree();
                        mnuCustomerDirectReport.ItemTitle = "گزارش خارجی مستقیم مشتری";
                        mnuCustomerDirectReport.NavigationTree2 = mnuReportFolder;
                        mnuCustomerDirectReport.Category = DatabaseObjectCategory.Report.ToString();
                        mnuCustomerDirectReport.ItemIdentity = customerDirectReport.ID;
                        mnuCustomerDirectReport.TableDrivedEntityID = customer.ID;
                        projectContext.NavigationTree.Add(mnuCustomerDirectReport);
                    }



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
                using (var dataContext = new MyIdeaDataDBEntities())
                {
                    var serviceRequestDataItemID = GetDataItemID(requester, serviceRequest);
                    if (serviceRequest != null && serviceRequestDataItemID != 0 && letterTypeService != null)
                    {

                        var letter = dataContext.Letter.FirstOrDefault(x => x.MyDataItemID == serviceRequestDataItemID && x.Title == "نامه داخلی نمونه");
                        if (letter == null)
                        {
                            letter = new Letter() { MyDataItemID = serviceRequestDataItemID, Title = "نامه داخلی نمونه" };

                            letter.CreationDate = DateTime.Now;
                            letter.FromExternalSource = false;
                            letter.UserID = requester.Identity;
                            letter.LetterNumber = "099211";
                            letter.IsGeneratedOrSelected = false;
                            letter.LetterTypeID = letterTypeService.ID;
                            letter.LetterDate = DateTime.Now;
                            string path = @"d:\files\ServiceRequestInternalLetter.docx";
                            if (File.Exists(path))
                            {
                                letter.FileRepository = new FileRepository()
                                {
                                    Content = File.ReadAllBytes(path),
                                    FileName = Path.GetFileName(path)
                                    ,
                                    FileExtention = Path.GetExtension(path)
                                };
                            }
                        }
                    }
                    if (serviceRequest != null && serviceRequestDataItemID != 0 && letterTypeService != null && simpleLetterTemplate != null)
                    {

                        BizDataItem biz = new BizDataItem();
                        var dataItem = biz.GetDataItem(requester, serviceRequestDataItemID, false);

                        var letter = dataContext.Letter.FirstOrDefault(x => x.MyDataItemID == serviceRequestDataItemID && x.Title == "نامه تولید شده نمونه");
                        if (letter == null)
                        {
                            letter = new Letter() { MyDataItemID = serviceRequestDataItemID, Title = "نامه تولید شده نمونه" };
                            letter.CreationDate = DateTime.Now;
                            letter.FromExternalSource = false;
                            letter.UserID = requester.Identity;
                            letter.LetterNumber = "099222";
                            letter.IsGeneratedOrSelected = true;
                            letter.LetterTypeID = letterTypeService.ID;
                            letter.LetterDate = DateTime.Now;
                            letter.FileRepository = new FileRepository()
                            {
                                Content = new LetterGenerator().GenerateLetter(simpleLetterTemplate.ID, dataItem.KeyProperties, requester),
                                FileName = Path.GetFileName(simpleLetterTemplate.LetterTemplate.Name),
                                FileExtention = Path.GetExtension(simpleLetterTemplate.FileExtension)
                            };

                            dataContext.Letter.Add(letter);

                        }
                    }

                    var serviceRequestReviewDataItemID = GetDataItemID(requester, serviceRequestReview);

                    if (serviceRequestReview != null && serviceRequestReviewDataItemID != 0 && letterTypeService != null && complexLetterTemplate != null)
                    {

                        BizDataItem biz = new BizDataItem();
                        var dataItem = biz.GetDataItem(requester, serviceRequestReviewDataItemID, false);

                        var letter = dataContext.Letter.FirstOrDefault(x => x.MyDataItemID == serviceRequestReviewDataItemID && x.Title == "نامه تولید شده پیچیده");
                        if (letter == null)
                        {
                            letter = new Letter() { MyDataItemID = serviceRequestReviewDataItemID, Title = "نامه تولید شده پیچیده" };
                            letter.CreationDate = DateTime.Now;
                            letter.FromExternalSource = false;
                            letter.UserID = requester.Identity;
                            letter.LetterNumber = "099282";
                            letter.IsGeneratedOrSelected = true;
                            letter.LetterTypeID = letterTypeService.ID;
                            letter.LetterDate = DateTime.Now;
                            letter.FileRepository = new FileRepository()
                            {
                                Content = new LetterGenerator().GenerateLetter(complexLetterTemplate.ID, dataItem.KeyProperties, requester),
                                FileName = Path.GetFileName(complexLetterTemplate.LetterTemplate.Name),
                                FileExtention = Path.GetExtension(complexLetterTemplate.FileExtension)
                            };

                            dataContext.Letter.Add(letter);

                        }
                    }

                    if (serviceRequest != null && serviceRequestDataItemID != 0 && letterTypeService != null)
                    {

                        var letter = dataContext.Letter.FirstOrDefault(x => x.MyDataItemID == serviceRequestDataItemID && x.Title == "اطلاعات نامه خارجی");
                        if (letter == null)
                        {
                            letter = new Letter() { MyDataItemID = serviceRequestDataItemID, Title = "اطلاعات نامه خارجی" };

                            letter.CreationDate = DateTime.Now;
                            letter.FromExternalSource = true;
                            letter.Title = "نامه از منبع خارجی";
                            letter.UserID = requester.Identity;
                            letter.LetterTypeID = letterTypeService.ID;
                            letter.ExternalSourceKey = "2234453";
                            dataContext.Letter.Add(letter);
                        }
                    }

                    try
                    {
                        dataContext.SaveChanges();
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
        }

        private EntityListViewColumns GetOrCreateListViewColumn(TableDrivedEntity entity, string columnName, EntityRelationshipTail relationshipTail)
        {
            EntityListViewColumns listViewColumn = null;
            if (relationshipTail == null)
                listViewColumn = entity.EntityListView1.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == columnName && x.EntityRelationshipTail == null);
            else
                listViewColumn = entity.EntityListView1.EntityListViewColumns.FirstOrDefault(x => x.Column.Name == columnName && x.EntityRelationshipTail != null && x.EntityRelationshipTail.RelationshipPath == relationshipTail.RelationshipPath);
            if (listViewColumn == null)
            {
                Column dateColumn = null;
                if (relationshipTail == null)
                    dateColumn = entity.Table.Column.FirstOrDefault(x => x.Name == columnName);
                else
                    dateColumn = relationshipTail.TableDrivedEntity1.Table.Column.FirstOrDefault(x => x.Name == columnName);
                if (dateColumn != null)
                {
                    listViewColumn = new EntityListViewColumns() { Column = dateColumn, IsDescriptive = true, EntityRelationshipTail = relationshipTail };
                    entity.EntityListView1.EntityListViewColumns.Add(listViewColumn);
                }
            }
            return listViewColumn;
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



        private int GetDataItemID(DR_Requester requester, TableDrivedEntity entity)
        {
            try
            {

                SearchRequestManager searchRequestManager = new SearchRequestManager();
                DR_SearchViewRequest item = new DR_SearchViewRequest(requester,
                     new DP_SearchRepository() { TargetEntityID = entity.ID });
                item.MaxDataItems = 1;
                item.SecurityMode = SecurityMode.View;

                var res = searchRequestManager.Process(item);
                if (res != null && res.ResultDataItems.Any())
                {
                    BizDataItem biz = new BizDataItem();
                    return biz.GetOrCreateDataItem(res.ResultDataItems[0]);
                }


                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }

}
