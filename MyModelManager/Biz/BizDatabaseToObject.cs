﻿using DataAccess;
using ModelEntites;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizDatabaseToObject
    {
        public bool IgnoreColumns { set; get; }
        public bool IgnoreCommands { set; get; }
        public bool IgnoreEntityArchive { set; get; }
        public bool IgnoreDataView { set; get; }
        public bool IgnoreGridView { set; get; }
        public bool IgnoreRelationships { set; get; }
        public bool HideFKRelationshipColumns { set; get; }
        public bool IgnoreReports { set; get; }
        public bool IgnoreEntityLetter { get; set; }
        public bool IgnoreViews { get; set; }
        public bool IgnoreNotIndependentOrAlreadyInNavigationTree { get; set; }
        public bool HidePKColumns { get; set; }

        //public  DatabaseDTO GetDatabaseDTO(int entityID)
        //{
        //    using (var context = new MyIdeaEntities())
        //    {
        //        var entity = context.TableDrivedEntity.FirstOrDefault(x => x.ID == entityID);
        //        if (entity != null)
        //        {
        //            var dbInfo = context.DatabaseInformation.FirstOrDefault(x => x.Name == entity.Table.Catalog);
        //            if (dbInfo != null)
        //                return ToDatabaseDTO(dbInfo);
        //        }
        //        return null;
        //    }
        //}
        //public List<DatabaseDTO> GetDatabases()
        //{
        //    List<DatabaseDTO> result = new List<DatabaseDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var list = projectContext.DatabaseInformation;
        //        foreach (var item in list)
        //        {
        //            result.Add(ToDatabaseDTO(item));
        //        }
        //    }
        //    return result;
        //}

        //private DatabaseDTO ToDatabaseDTO(DataAccess.DatabaseInformation item)
        //{
        //    DatabaseDTO result = new DatabaseDTO();
        //    result.Name = item.Name;
        //    result.ConnectionString = item.ConnectionString;
        //    return result;
        //}


        //public List<SchemaDTO> GetSchemaDTO(string databaseName)
        //{
        //    List<SchemaDTO> result = new List<SchemaDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        foreach (var schema in projectContext.TableDrivedEntity.Where(x => x.Table.Catalog == databaseName).GroupBy(x => x.Table.RelatedSchema))
        //        {
        //            result.Add(ToSchemaDTO(schema));
        //        }
        //    }
        //    return result;
        //}

        //private SchemaDTO ToSchemaDTO(IGrouping<string, DataAccess.TableDrivedEntity> schema)
        //{
        //    SchemaDTO result = new SchemaDTO();
        //    result.Name = schema.Key;
        //    return result;
        //}


        BizRelationship bizRelationship = new BizRelationship();
        BizColumn bizColumn = new BizColumn();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        public List<ObjectDTO> GetDatabaseChildObjects(DatabaseObjectCategory parentCategory, string parentTitle, int parentIdentity)
        {

            // BizDatabaseToObject.GetDatabaseChildObjects: c1e9819478c9
            List<ObjectDTO> result = new List<ObjectDTO>();
            using (var myProjectContext = new MyIdeaEntities())
            {

                //foreach (var database in myProjectContext.DatabaseInformation)
                //{
                //    ObjectDTO dbObject = ToObjectDTO(DatabaseObjectCategory.Database, database.Name, database.Name);
                //    result.Add(dbObject);
                //}

                if (parentCategory == DatabaseObjectCategory.Database)
                {
                    foreach (var schema in myProjectContext.DBSchema.Where(x => x.DatabaseInformationID == parentIdentity))
                    {
                        //var schemaName = "";
                        //if (string.IsNullOrEmpty(schema.Key))
                        //    schemaName = "Default Schema";
                        //else
                        //schemaName = schema.Name;

                        ObjectDTO schemaObject = ToObjectDTO(DatabaseObjectCategory.Schema, schema.ID, schema.Name, "", 0);
                        result.Add(schemaObject);
                    }
                }
                else if (parentCategory == DatabaseObjectCategory.Schema)
                {
                    //var res = GetDBNameSchemaName(parentIdentity);
                    foreach (var entity in bizTableDrivedEntity.GetAllEnabledEntities(myProjectContext).Where(x =>  x.Table.DBSchemaID == parentIdentity))
                    {
                        if (IgnoreNotIndependentOrAlreadyInNavigationTree)
                            if (entity.IndependentDataEntry != true || myProjectContext.NavigationTree.Any(x => x.Category == "Entity" && x.ItemIdentity == entity.ID))
                                continue;
                        if (IgnoreViews)
                            if (entity.IsView)
                                continue;
                        ObjectDTO entityObject = ToObjectDTO(DatabaseObjectCategory.Entity, entity.ID, string.IsNullOrEmpty(entity.Alias) ? entity.Name : entity.Alias, entity.Name, entity.ID, entity.IsView ? EntityObjectType.View : EntityObjectType.Entity);
                        result.Add(entityObject);
                    }
                }
                else if (parentCategory == DatabaseObjectCategory.Entity)
                {
                    int id = Convert.ToInt32(parentIdentity);
                    var dbEntity = bizTableDrivedEntity.GetAllEnabledEntities(myProjectContext).First(x => x.ID == id);
                    if (!IgnoreColumns)
                    {
                        var columns = bizColumn.GetAllEnabledColumns(dbEntity);
                        foreach (var column in columns)
                        {
                            bool skipColumn = false;
                            if (HidePKColumns)
                            {
                                if (column.PrimaryKey)
                                    skipColumn = true;
                            }
                            if (HideFKRelationshipColumns)
                            {
                                var type = (int)Enum_MasterRelationshipType.FromForeignToPrimary;
                                if (bizRelationship.GetAllEnabledRelationships(myProjectContext, false).Any(x => x.MasterTypeEnum == type && x.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID && y.Column.PrimaryKey == false)))
                                    skipColumn = true;
                            }
                            if (!skipColumn)
                            {
                                ObjectDTO columnObject = ToObjectDTO(DatabaseObjectCategory.Column, column.ID, string.IsNullOrEmpty(column.Alias) ? column.Name : column.Alias, column.Name, id);
                                result.Add(columnObject);
                            }
                        }
                    }
                    if (!IgnoreRelationships)
                    {
                        foreach (var relationship in bizRelationship.GetEnabledRelationships(dbEntity)) 
                        {
                            ObjectDTO relationshipObject = ToObjectDTO(DatabaseObjectCategory.Relationship, relationship.ID, string.IsNullOrEmpty(relationship.Alias) ? relationship.Name : relationship.Alias, relationship.Name, id);
                            result.Add(relationshipObject);
                        }
                    }
                    if (!IgnoreCommands)
                    {
                        foreach (var command in dbEntity.TableDrivedEntity_EntityCommand)
                        {
                            ObjectDTO commandObject = ToObjectDTO(DatabaseObjectCategory.Command, command.EntityCommandID, command.EntityCommand.Title, "", id);
                            result.Add(commandObject);
                        }
                    }
                    if (!IgnoreReports)
                    {
                        foreach (var report in dbEntity.EntityReport)
                        {
                            ObjectDTO commandObject = ToObjectDTO(DatabaseObjectCategory.Report, report.ID, report.Title, "", id);
                            result.Add(commandObject);
                        }
                    }
                    if (dbEntity.IsView == false && !IgnoreEntityArchive)
                    {
                        ObjectDTO commandObject = ToObjectDTO(DatabaseObjectCategory.Archive, parentIdentity, "آرشیو" + " " + parentTitle, "", id);
                        result.Add(commandObject);
                    }
                    if (dbEntity.IsView == false && !IgnoreEntityLetter)
                    {
                        ObjectDTO commandObject = ToObjectDTO(DatabaseObjectCategory.Letter, parentIdentity, "نامه ها" + " " + parentTitle, "", id);
                        result.Add(commandObject);
                    }
                    if (!IgnoreDataView)
                    {
                        ObjectDTO commandObject = ToObjectDTO(DatabaseObjectCategory.DataView, parentIdentity, "نمای داده" + " " + parentTitle, "", id);
                        result.Add(commandObject);
                    }
                    if (!IgnoreGridView)
                    {
                        ObjectDTO commandObject = ToObjectDTO(DatabaseObjectCategory.GridView, parentIdentity, "گرید داده" + " " + parentTitle, "", id);
                        result.Add(commandObject);
                    }
                }
            }
            return result;
        }

        //private Tuple<string, string> GetDBNameSchemaName(string objectIdentity)
        //{
        //    return new Tuple<string, string>(objectIdentity.Split('>')[0], objectIdentity.Split('>')[1]);
        //}
        private ObjectDTO ToObjectDTO(DatabaseObjectCategory objectCategory, int objectIdentity, string title, string name, int tableDrivedEntityID, EntityObjectType entityType = EntityObjectType.None)
        {
            ObjectDTO result = new ObjectDTO();
            result.ObjectCategory = objectCategory;
            result.ObjectIdentity = objectIdentity;
            result.Title = title;
            result.TableDrivedEntityID = tableDrivedEntityID;
            result.EntityType = entityType;
            result.Name = name;
            //result.SecurityObjectID = securityObjectID;
            result.NeedsExplicitPermission = (objectCategory == DatabaseObjectCategory.Entity);
            return result;
        }
        //private string GetSchemaObjectName(string dbName, string schemaName)
        //{
        //    return dbName + ">" + schemaName;
        //}

        public ObjectDTO GetParentObject(DatabaseObjectCategory objectCategory, int objectIdentity)
        {
            if (objectCategory == DatabaseObjectCategory.Database)
                return null;
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                ObjectDTO result = null;
                if (objectCategory == DatabaseObjectCategory.Schema)
                {

                    //var dbName = objectIdentity.Split('>')[0];
                    var schema = projectContext.DBSchema.First(x => x.ID == objectIdentity);
                    result = ToObjectDTO(DatabaseObjectCategory.Database, schema.DatabaseInformationID, schema.DatabaseInformation.Name, "", 0);
                }
                else if (objectCategory == DatabaseObjectCategory.Entity)
                {
                    //int id = Convert.ToInt32(objectIdentity);
                    var entity = bizTableDrivedEntity.GetAllEnabledEntities(projectContext).First(x => x.ID == objectIdentity);
                    result = ToObjectDTO(DatabaseObjectCategory.Schema, entity.Table.DBSchemaID, entity.Table.DBSchema.Name, "", 0);
                }
                else if (objectCategory == DatabaseObjectCategory.Column)
                {
                    //var column = projectContext.TableDrivedEntity.First(x => x.ID == objectIdentity);
                    //result = ToObjectDTO("Schema", GetSchemaObjectName(entity.Table.Catalog, entity.Table.RelatedSchema), entity.Table.RelatedSchema);
                }
                return result;
            }
        }

        //public List<ObjectDTO> GetChildObjects(string objectIdentity, string objectCategory)
        //{
        //    if (objectCategory == "Column")
        //        return null;
        //    var result = new List<ObjectDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        if (objectCategory == "Database")
        //        {
        //            var 
        //        }
        //        if (objectCategory == "Schema")
        //        {
        //            var dbName = objectIdentity.Split('>')[0];
        //            result = new ObjectDTO();
        //            result.ObjectIdentity = dbName;
        //            result.Category = "Database";
        //            result.Title = dbName;
        //        }
        //        if (objectCategory == "Schema")
        //        {
        //            var dbName = objectIdentity.Split('>')[0];
        //            result = new ObjectDTO();
        //            result.ObjectIdentity = dbName;
        //            result.Category = "Database";
        //            result.Title = dbName;
        //        }
        //        else if (objectCategory == "Entity")
        //        {
        //            int id = Convert.ToInt32(objectIdentity);
        //            var entity = projectContext.TableDrivedEntity.First(x => x.ID == id);
        //            result = new ObjectDTO();
        //            result.ObjectIdentity = GetSchemaObjectName(entity.Table.Catalog, entity.Table.RelatedSchema);
        //            result.Category = "Schema";
        //            result.Title = entity.Table.RelatedSchema;
        //        }
        //        return result;
        //    }
        //}


        public bool GetUpwardCondition(DatabaseObjectCategory objectCategory)
        {
            return objectCategory == DatabaseObjectCategory.Entity;
        }
    }

}
