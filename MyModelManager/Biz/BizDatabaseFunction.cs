﻿using DataAccess;
using ModelEntites;
using MyGeneralLibrary;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizDatabaseFunction
    {
        public event EventHandler<ItemImportingStartedArg> ItemImportingStarted;

        //public List<DatabaseFunctionDTO> GetDatabaseFunctions(string serverName, string dbName)
        //{
        //    return GetDatabaseFunctions(GeneralHelper.GetCatalogName(serverName, dbName));
        //}
        //public List<DatabaseFunctionDTO> GetDatabaseFunctionsByEntityID(int entityID)
        //{
        //    List<DatabaseFunctionDTO> result = new List<DatabaseFunctionDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        //projectContext.Configuration.LazyLoadingEnabled = false;


        //        var listDatabaseFunction = projectContext.DatabaseFunction.Where(x => x.DatabaseFunction_TableDrivedEntity.Any(y => y.TableDrivedEntityID == entityID));

        //        foreach (var item in listDatabaseFunction)
        //            result.Add(ToDatabaseFunctionDTO(item, false));

        //    }
        //    return result;
        //}
        public DatabaseFunctionDTO GetDatabaseFunctionByName(int databaseID, string name)
        {
            List<DatabaseFunctionDTO> result = new List<DatabaseFunctionDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbFunction = projectContext.DatabaseFunction.FirstOrDefault(x => x.FunctionName == name && x.DBSchema.DatabaseInformationID == databaseID);

                if (dbFunction != null)
                    return ToDatabaseFunctionDTO(dbFunction, true);
                else
                    return null;

            }
        }
        public List<DatabaseFunctionDTO> GetAllDatabaseFunctions(DR_Requester dR_Requester, string generalFilter, Enum_DatabaseFunctionType type)
        {
            List<DatabaseFunctionDTO> result = new List<DatabaseFunctionDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                IQueryable<DatabaseFunction> listDatabaseFunction;
                listDatabaseFunction = projectContext.DatabaseFunction;
                if (!string.IsNullOrEmpty(generalFilter))
                {
                    listDatabaseFunction = listDatabaseFunction.Where(x => x.FunctionName.Contains(generalFilter));
                }

                if (type == Enum_DatabaseFunctionType.Function)
                    listDatabaseFunction = listDatabaseFunction.Where(x => x.Type == (short)Enum_DatabaseFunctionType.Function);
                else if (type == Enum_DatabaseFunctionType.StoredProcedure)
                    listDatabaseFunction = listDatabaseFunction.Where(x => x.Type == (short)Enum_DatabaseFunctionType.StoredProcedure);
                foreach (var item in listDatabaseFunction)
                    result.Add(ToDatabaseFunctionDTO(item, false));

            }
            return result;
        }
        public List<DatabaseFunctionDTO> GetDatabaseFunctions(DR_Requester requester, Enum_DatabaseFunctionType type, int databaseID = 0)
        {
            List<DatabaseFunctionDTO> result = new List<DatabaseFunctionDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                //projectContext.Configuration.LazyLoadingEnabled = false;
                IQueryable<DatabaseFunction> listDatabaseFunction;

                if (databaseID == 0)
                    listDatabaseFunction = projectContext.DatabaseFunction;
                else
                    listDatabaseFunction = projectContext.DatabaseFunction.Where(x => x.DBSchema.DatabaseInformationID == databaseID); 
                if (type == Enum_DatabaseFunctionType.Function)
                    listDatabaseFunction = listDatabaseFunction.Where(x => x.Type == (short)Enum_DatabaseFunctionType.Function);
                else if (type == Enum_DatabaseFunctionType.StoredProcedure)
                    listDatabaseFunction = listDatabaseFunction.Where(x => x.Type == (short)Enum_DatabaseFunctionType.StoredProcedure);
                foreach (var item in listDatabaseFunction)
                    result.Add(ToDatabaseFunctionDTO(item, false));

            }
            return result;
        }



        public DatabaseFunctionDTO GetDatabaseFunction(DR_Requester requester, int DatabaseFunctionsID)
        {
            List<DatabaseFunctionDTO> result = new List<DatabaseFunctionDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var DatabaseFunctions = projectContext.DatabaseFunction.First(x => x.ID == DatabaseFunctionsID);
                return ToDatabaseFunctionDTO(DatabaseFunctions, true);
            }
        }
        public List<DatabaseFunctionDTO> GetOrginalFunctions(int databaseID)
        {
            List<DatabaseFunctionDTO> result = new List<DatabaseFunctionDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var functions = projectContext.DatabaseFunction.Include("DatabaseFunctionParameter")
                    .Where(x => x.DBSchema.DatabaseInformationID == databaseID);
                foreach (var function in functions)
                    result.Add(ToDatabaseFunctionDTO(function, true));
            }
            return result;
        }
        public bool OrginalEntityExists(string functionName, int databaseID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return projectContext.DatabaseFunction.Any(x => x.FunctionName == functionName && x.DBSchema.DatabaseInformationID == databaseID);
            }
        }
        public DatabaseFunctionDTO GetOrginalDatabaseFunction(string name, int databaseID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return ToDatabaseFunctionDTO(projectContext.DatabaseFunction.First(x => x.FunctionName == name && x.DBSchema.DatabaseInformationID == databaseID), true);

            }
        }
        public List<DatabaseFunctionDTO> GetEnalbedDatabaseFunctions(int databaseID)
        {
            List<DatabaseFunctionDTO> result = new List<DatabaseFunctionDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var list = projectContext.DatabaseFunction.Where(x => x.Enable == true && x.DBSchema.DatabaseInformationID == databaseID);
                foreach (var item in list)
                    result.Add(ToDatabaseFunctionDTO(item, true));
            }
            return result;
        }
        public DatabaseFunctionDTO ToDatabaseFunctionDTO(DatabaseFunction cItem, bool withColumns)
        {
            BizColumn bizColumn = new BizColumn();
            var result = new DatabaseFunctionDTO();
            result.ID = cItem.ID;
            result.Name = cItem.FunctionName;
            //result.Catalog = cItem.Catalog;
            result.SchemaID = cItem.DBSchemaID;
            result.DatabaseID = cItem.DBSchema.DatabaseInformationID;
            result.RelatedSchema = cItem.DBSchema.Name;

            //result.ReturnType = cItem.ReturnType;
            //if (cItem.ValueCustomType != null)
            //    result.ValueCustomType = (ValueCustomType)cItem.ValueCustomType;
            result.Type = (Enum_DatabaseFunctionType)cItem.Type;

            result.Title = cItem.Title;
            result.Enable = cItem.Enable;
            //if (result.Title == null)
            //{
            //    result.Title = cItem.FunctionName;
            //}
            if (withColumns)
            {
                result.DatabaseFunctionParameter = ToDatabaseFunctionParameterDTO(cItem);
            }
            //    if (result.Type == Enum_DatabaseFunctionType.Function)

            result.ReturnDotNetType = GetReturnType(cItem);

            return result;
        }

        private Type GetReturnType(DatabaseFunction cItem)
        {
            //اینجا میتوان برای استورد پروسیجر ها با یک خصوصیت تعیین کرد که کدوم پارامتر خروجی اصلی است
            BizColumn bizColumn = new BizColumn();
            DatabaseFunctionParameter parameter = null;
            var output = (short)Enum_DatabaseFunctionParameterType.Output;
            parameter = cItem.DatabaseFunctionParameter.FirstOrDefault(x => x.InputOutput == output);
            if (parameter != null)
                return bizColumn.GetColumnDotNetType(parameter.DataType, false);
            else
            {
                output = (short)Enum_DatabaseFunctionParameterType.InputOutput;
                parameter = cItem.DatabaseFunctionParameter.FirstOrDefault(x => x.InputOutput == output);
                if (parameter != null)
                    return bizColumn.GetColumnDotNetType(parameter.DataType, false);
                else
                {
                    output = (short)Enum_DatabaseFunctionParameterType.ReturnValue;
                    parameter = cItem.DatabaseFunctionParameter.FirstOrDefault(x => x.InputOutput == output);
                    if (parameter != null)
                        return bizColumn.GetColumnDotNetType(parameter.DataType, false);
                }
            }
            return null;
        }

        private List<DatabaseFunctionParameterDTO> ToDatabaseFunctionParameterDTO(DatabaseFunction cItem)
        {
            BizColumn bizColumn = new BizColumn();
            List<DatabaseFunctionParameterDTO> result = new List<DatabaseFunctionParameterDTO>();
            foreach (var column in cItem.DatabaseFunctionParameter)
            {
                var item = new DatabaseFunctionParameterDTO()
                {
                    ID = column.ID,
                    DataType = column.DataType,
                    Enable = column.Enable,
                    ParameterName = column.ParamName,
                    DotNetType = bizColumn.GetColumnDotNetType(column.DataType, false),
                    Order = column.Order ?? 0,
                    InputOutput = (Enum_DatabaseFunctionParameterType)column.InputOutput

                };

                result.Add(item);
            }
            return result;
        }


        public List<DatabaseFunctionParameterDTO> GetDatabaseFunctionParameters(int functionID)
        {



            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var DatabaseFunction = projectContext.DatabaseFunction.First(x => x.ID == functionID);
                return ToDatabaseFunctionParameterDTO(DatabaseFunction);
            }
        }

        public DatabaseFunction_EntityDTO GetDatabaseFunctionEntity(DR_Requester requester, int functionEntityID)
        {

            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var DatabaseFunctions = projectContext.DatabaseFunction_TableDrivedEntity.First(x => x.ID == functionEntityID);
                return ToDatabaseFunction_EntityDTO(DatabaseFunctions, true);
            }
        }
        public DatabaseFunction_EntityDTO GetDatabaseFunctionEntity(int functionID, int entityID)
        {

            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var DatabaseFunctions = projectContext.DatabaseFunction_TableDrivedEntity.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.DatabaseFunctionID == functionID);
                if (DatabaseFunctions != null)
                    return ToDatabaseFunction_EntityDTO(DatabaseFunctions, true);
                else
                    return null;
            }
        }
        public List<DatabaseFunction_EntityDTO> GetDatabaseFunctionEntities(DR_Requester requester, int entityID)
        {
            // BizDatabaseFunction:GetDatabaseFunctionEntities 5c860f83a5fd
            List<DatabaseFunction_EntityDTO> result = new List<DatabaseFunction_EntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                //projectContext.Configuration.LazyLoadingEnabled = false;

                var listDatabaseFunction = projectContext.DatabaseFunction_TableDrivedEntity.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in listDatabaseFunction)
                    result.Add(ToDatabaseFunction_EntityDTO(item, false));

            }
            return result;
        }
        //public List<DatabaseFunction_EntityDTO> GetDatabaseFunctionEntityByEntityID(DR_Requester requester, int entityID)
        //{
        //    List<DatabaseFunction_EntityDTO> result = new List<DatabaseFunction_EntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var DatabaseFunctions = projectContext.DatabaseFunction_TableDrivedEntity.Where(x => x.TableDrivedEntityID == entityID);
        //        foreach (var item in DatabaseFunctions)
        //            result.Add(ToDatabaseFunction_EntityDTO(item, false));
        //    }
        //    return result;
        //}
        public void UpdateModel(int databaseID, List<DatabaseFunctionDTO> listNew, List<DatabaseFunctionDTO> listEdit, List<DatabaseFunctionDTO> listDeleted)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listSchema = new List<DBSchema>();
                foreach (var newEntity in listNew)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Creating" + " " + newEntity.Name, TotalProgressCount = listNew.Count, CurrentProgress = listNew.IndexOf(newEntity) + 1 });
                    UpdateEntityInModel(projectContext, databaseID, newEntity, listSchema);
                }
                foreach (var editEntity in listEdit)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Updating" + " " + editEntity.Name, TotalProgressCount = listEdit.Count, CurrentProgress = listEdit.IndexOf(editEntity) + 1 });
                    UpdateEntityInModel(projectContext, databaseID, editEntity, listSchema);
                }
                foreach (var deleteEntity in listDeleted)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Disabling" + " " + deleteEntity.Name, TotalProgressCount = listDeleted.Count, CurrentProgress = listDeleted.IndexOf(deleteEntity) + 1 });
                    var dbEntity = projectContext.DatabaseFunction.FirstOrDefault(x => x.ID == deleteEntity.ID);
                    dbEntity.Enable = false;
                }
                projectContext.SaveChanges();
            }
        }
        private void UpdateEntityInModel(MyIdeaEntities projectContext, int databaseID, DatabaseFunctionDTO function, List<DBSchema> listAddedSchema)
        {
            DBSchema dbSchema = null;
            var schema = function.RelatedSchema;
            dbSchema = listAddedSchema.FirstOrDefault(x => x.DatabaseInformationID == databaseID && x.Name == function.RelatedSchema);
            if (dbSchema == null)
            {
                dbSchema = new DBSchema() { DatabaseInformationID = databaseID, Name = schema };
                dbSchema.SecurityObject = new SecurityObject();
                dbSchema.SecurityObject.Type = (int)DatabaseObjectCategory.Schema;
                projectContext.DBSchema.Add(dbSchema);
                listAddedSchema.Add(dbSchema);
            }

            var dbFunction = projectContext.DatabaseFunction.Include("DatabaseFunctionParameter").FirstOrDefault(x => x.FunctionName == function.Name && x.DBSchema.DatabaseInformationID == databaseID);
            if (dbFunction == null)
            {
                dbFunction = new DatabaseFunction();
                projectContext.DatabaseFunction.Add(dbFunction);
                dbFunction.FunctionName = function.Name;
                dbFunction.Enable = true;
            }
            dbFunction.DBSchema = dbSchema;

            dbFunction.Type = (short)function.Type;
            foreach (var column in function.DatabaseFunctionParameter)
            {
                var dbColumn = dbFunction.DatabaseFunctionParameter.Where(x => x.ParamName == column.ParameterName).FirstOrDefault();
                if (dbColumn == null)
                {
                    dbColumn = new DatabaseFunctionParameter();
                    dbColumn.ParamName = column.ParameterName;
                    dbColumn.Enable = true;
                    dbFunction.DatabaseFunctionParameter.Add(dbColumn);
                }
                dbColumn.InputOutput = (short)column.InputOutput;
                dbColumn.DataType = column.DataType;
                dbColumn.ParamName = column.ParameterName;
            }
            var columnNames = function.DatabaseFunctionParameter.Select(x => x.ParameterName).ToList();
            foreach (var dbColumn in dbFunction.DatabaseFunctionParameter.Where(x => !columnNames.Contains(x.ParamName)))
            {
                dbColumn.Enable = false;
            }
        }
        public DatabaseFunction_EntityDTO ToDatabaseFunction_EntityDTO(DatabaseFunction_TableDrivedEntity cItem, bool withColumns)
        {
            BizColumn bizColumn = new BizColumn();
            var result = new DatabaseFunction_EntityDTO();
            result.ID = cItem.ID;
            result.Title = cItem.Title;
            result.Name = cItem.Name;
            result.EntityID = cItem.TableDrivedEntityID;
            result.DatabaseFunctionID = cItem.DatabaseFunctionID;
            result.DatabaseFunction = ToDatabaseFunctionDTO(cItem.DatabaseFunction, withColumns);
            if (withColumns)
            {
                result.DatabaseFunctionEntityColumns = ToDatabaseFunctionEntityColumnsDTO(cItem);
            }

            return result;
        }

        public int GetDatabaseFunctionEntityID(int entityID, int DatabaseFunctionID)
        {
            List<DatabaseFunction_EntityDTO> result = new List<DatabaseFunction_EntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                //projectContext.Configuration.LazyLoadingEnabled = false;

                var item = projectContext.DatabaseFunction_TableDrivedEntity.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.DatabaseFunctionID == DatabaseFunctionID);
                if (item != null)
                    return item.ID;
                else
                    return 0;

            }
        }



        private List<DatabaseFunction_Entity_ColumnDTO> ToDatabaseFunctionEntityColumnsDTO(DatabaseFunction_TableDrivedEntity cItem)
        {
            BizColumn bizColumn = new BizColumn();
            List<DatabaseFunction_Entity_ColumnDTO> result = new List<DatabaseFunction_Entity_ColumnDTO>();
            foreach (var column in cItem.DatabaseFunction_TableDrivedEntity_Columns)
            {
                var item = new DatabaseFunction_Entity_ColumnDTO();
                item.ID = column.ID;
                item.DBFunctionParameterID = column.DatabaseFunctionParameterID;
                item.DatabaseFunction_EntityID = column.DatabaseFunction_TableDrivedEntityID;
                if (column.ColumnID != null)
                {
                    item.ColumnID = column.ColumnID.Value;
                    item.ColumnName = column.Column.Name;
                }
                if (column.FixedParamID != null)
                    item.FixedParam = (Enum_FixedParam)column.FixedParamID;
                item.DBFunctionParamDotNetType = bizColumn.GetColumnDotNetType(column.DatabaseFunctionParameter.DataType, false);
                item.DBFunctionParamName = column.DatabaseFunctionParameter.ParamName;
                result.Add(item);
            }
            return result;
        }
        public void Save(List<DatabaseFunctionDTO> list)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var dbFunction in list)
                {
                    var dbEntity = projectContext.DatabaseFunction.First(x => x.ID == dbFunction.ID);
                    dbEntity.Title = dbFunction.Title;
                    //dbEntity.ValueCustomType = (short)dbFunction.ValueCustomType;
                }
                projectContext.SaveChanges();
            }
        }
        public int UpdateDatabaseFunctionEntity(DatabaseFunction_EntityDTO DatabaseFunctionEntity)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbDatabaseFunctionEntity = projectContext.DatabaseFunction_TableDrivedEntity.FirstOrDefault(x => x.ID == DatabaseFunctionEntity.ID);
                if (dbDatabaseFunctionEntity == null)
                    dbDatabaseFunctionEntity = new DatabaseFunction_TableDrivedEntity();
                dbDatabaseFunctionEntity.DatabaseFunctionID = DatabaseFunctionEntity.DatabaseFunctionID;
                dbDatabaseFunctionEntity.TableDrivedEntityID = DatabaseFunctionEntity.EntityID;
                dbDatabaseFunctionEntity.Title = DatabaseFunctionEntity.Title;
                dbDatabaseFunctionEntity.Name = DatabaseFunctionEntity.Name;
                while (dbDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Any())
                {
                    dbDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Remove(dbDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.First());
                }
                foreach (var column in DatabaseFunctionEntity.DatabaseFunctionEntityColumns)
                {
                    DatabaseFunction_TableDrivedEntity_Columns dbColumn = new DataAccess.DatabaseFunction_TableDrivedEntity_Columns();
                    if (column.ColumnID != 0)
                    {
                        dbColumn.ColumnID = column.ColumnID;
                        dbColumn.FixedParamID = null;
                    }
                    else
                    {
                        dbColumn.ColumnID = null;
                        dbColumn.FixedParamID = (short)column.FixedParam;
                    }
                    dbColumn.DatabaseFunctionParameterID = column.DBFunctionParameterID;
                    dbDatabaseFunctionEntity.DatabaseFunction_TableDrivedEntity_Columns.Add(dbColumn);
                }
                if (dbDatabaseFunctionEntity.ID == 0)
                    projectContext.DatabaseFunction_TableDrivedEntity.Add(dbDatabaseFunctionEntity);
                projectContext.SaveChanges();
                return dbDatabaseFunctionEntity.ID;
            }
        }

        //public void UpdateDatabaseFunctions(List<DatabaseFunctionDTO> DatabaseFunctions)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        foreach (var item in DatabaseFunctions)
        //        {
        //            var dbDatabaseFunctions = projectContext.DatabaseFunction.First(x => x.ID == DatabaseFunctions.ID);
        //            dbDatabaseFunctions.Name = item.Name;

        //            //  dbDatabaseFunctions.BatchDataEntry = DatabaseFunctions.BatchDataEntry;
        //            //  dbDatabaseFunctions.IsAssociative = DatabaseFunctions.IsAssociative;

        //            //   dbDatabaseFunctions.IsDataReference = DatabaseFunctions.IsDataReference;
        //            //   dbDatabaseFunctions.IsStructurReferencee = DatabaseFunctions.IsStructurReferencee;
        //        }
        //        projectContext.SaveChanges();
        //    }
        //}
    }

}
