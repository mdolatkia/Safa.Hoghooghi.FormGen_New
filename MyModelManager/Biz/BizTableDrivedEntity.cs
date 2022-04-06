using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;
using MyRuleEngine;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLibrary;
using System.Data.Entity.Infrastructure;
using CommonDefinitions.UISettings;

namespace MyModelManager
{
    public class BizTableDrivedEntity
    {



        public event EventHandler<ItemImportingStartedArg> ItemImportingStarted;

        public bool IndependentDataEntry(int entityID)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                return projectContext.TableDrivedEntity.First(x => x.ID == entityID).IndependentDataEntry == true;
            }
        }
        public List<TableDrivedEntityDTO> GetAllEntities(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool? isView)
        {
            //بهتره خود انتیتی با دیتابیس رابطه داشته باشد
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var listEntity = GetEntities(projectContext, columnInfoType, relationshipInfoType, isView);
                listEntity = listEntity.Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
                foreach (var item in listEntity)
                    result.Add(ToTableDrivedEntityDTO(item, columnInfoType, relationshipInfoType, false, false));
            }
            return result.OrderBy(x => x.RelatedSchema).ThenBy(x => x.Name).ToList();
        }
        public List<TableDrivedEntityDTO> GetAllEntities(DR_Requester requester, string generalFilter, bool? isView, List<SecurityAction> specificActions = null)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var listEntity = GetEntities(projectContext, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, isView);
                if (generalFilter != "")
                    listEntity = listEntity.Where(x => x.ID.ToString() == generalFilter || x.Name.Contains(generalFilter) || x.Alias.Contains(generalFilter));

                foreach (var item in listEntity)
                {
                    if (DataIsAccessable(requester, item, specificActions))
                    {
                        var rItem = ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, false, false);
                        result.Add(rItem);
                    }
                }
            }

            return result;
        }
        public AssignedPermissionDTO GetEntityAssignedPermissions(DR_Requester requester, int entityID, bool withChildObjects)
        {
            SecurityHelper securityHelper = new SecurityHelper();
            return securityHelper.GetAssignedPermissions(requester, entityID, withChildObjects);
        }
        //public List<int> EntitiesHasActionsPermissions(DR_Requester requester, List<int> entityIDs, List<SecurityAction> actionNames)
        //{
        //    var permissoinResult = securityHelper.ObjectsHaveSpecificPermissions(requester, entityIDs, actionNames);
        //    foreach (var permission in permissoinResult)
        //    {
        //        if (permission.Permitted)
        //        {
        //            foreach (var item in checkItems.Where(x => x.TableDrivedEntityID == permission.ObjectSecurityID))
        //            {
        //                RemoveTreeItem(treeItems, item);
        //            }
        //        }
        //    }
        //}

        //public List<int> GetDisabledEntityIDs(List<int> checkEntityIDs)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        return projectContext.TableDrivedEntity.Where(x => checkEntityIDs.Contains(x.ID) && x.IsEnabled == false).Select(x => x.ID).ToList();
        //    }
        //}




        //public bool DataIsAccessable(DR_Requester requester, int entityID, SecurityMode securityMode)
        //{
        //    List<SecurityAction> listActions = null;
        //    if (securityMode == SecurityMode.View)
        //        listActions = new List<SecurityAction>() { SecurityAction.ReadOnly, SecurityAction.Edit, SecurityAction.EditAndDelete };
        //    else if (securityMode == SecurityMode.Edit)
        //        listActions = new List<SecurityAction>() { SecurityAction.Edit, SecurityAction.EditAndDelete };

        //    return DataIsAccessable(requester, entityID, listActions);
        //}
        //public bool DataIsAccessable(DR_Requester requester, TableDrivedEntity entity, SecurityMode securityMode)
        //{
        //    List<SecurityAction> listActions = null;
        //    if (securityMode == SecurityMode.View)
        //        listActions = new List<SecurityAction>() { SecurityAction.ReadOnly, SecurityAction.Edit, SecurityAction.EditAndDelete };
        //    else if (securityMode == SecurityMode.Edit)
        //        listActions = new List<SecurityAction>() { SecurityAction.Edit, SecurityAction.EditAndDelete };

        //    return DataIsAccessable(requester, entity, listActions);
        //}

        public bool DataIsAccessable(DR_Requester requester, int entityID, List<SecurityAction> specificActions = null)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
                return DataIsAccessable(requester, entity, specificActions);
            }
        }
        public bool DataIsAccessable(DR_Requester requester, TableDrivedEntity entity, List<SecurityAction> specificActions = null)
        {
            if (entity.IsDisabled)
                return false;
            else
            {
                if (requester.SkipSecurity)
                    return true;
                var permission = GetEntityAssignedPermissions(requester, entity.ID, false);
                if (permission.GrantedActions.Any(y => y == SecurityAction.NoAccess))
                    return false;
                else if (specificActions != null)
                {
                    if (permission.GrantedActions.Any(y => specificActions.Contains(y)))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool DataIsReadonly(DR_Requester requester, int entityID)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
                return DataIsReadonly(requester, entity);
            }
        }
        public bool DataIsReadonly(DR_Requester requester, TableDrivedEntity entity)
        {
            if (entity.IsReadonly)
                return true;
            else
            {
                if (requester.SkipSecurity)
                    return false;
                var permission = GetEntityAssignedPermissions(requester, entity.ID, false);
                if (permission.GrantedActions.Any(y => y == SecurityAction.ReadOnly))
                    return true;
                else
                {
                    return false;
                }
            }
        }

        private IQueryable<TableDrivedEntity> GetEntities(MyProjectEntities projectContext, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool? isView, int ID = 0)
        {
            //بعدا بررسی شود که اینکلود رو میشه یکبار به کوئری اضافه کرد؟
            IQueryable<TableDrivedEntity> listEntity;
            //if (columnInfoType == EntityColumnInfoType.WithFullColumns)
            //{
            //    if (relationshipInfoType == EntityRelationshipInfoType.WithRelationships)
            //    {
            //        listEntity = projectContext.TableDrivedEntity.Include("Table.DBSchema.DatabaseInformation")


            //       .Include("TableDrivedEntity_Columns.Column.StringColumnType").Include("TableDrivedEntity_Columns.Column.NumericColumnType")
            //       .Include("TableDrivedEntity_Columns.Column.DateColumnType")
            //       .Include("Table.Column.StringColumnType").Include("Table.Column.NumericColumnType")
            //       .Include("Table.Column.DateColumnType")

            //       .Include("Relationship.TableDrivedEntity.Table.DBSchema.DatabaseInformation.DBServer.LinkedServer")
            //       .Include("Relationship.TableDrivedEntity1.Table.DBSchema.DatabaseInformation.DBServer.LinkedServer")
            //       .Include("Relationship.RelationshipColumns.Column")
            //       .Include("Relationship.RelationshipType");
            //    }
            //    else
            //    {
            //        listEntity = projectContext.TableDrivedEntity.Include("Table.DBSchema.DatabaseInformation")

            //        .Include("TableDrivedEntity_Columns.Column.StringColumnType").Include("TableDrivedEntity_Columns.Column.NumericColumnType")
            //        .Include("TableDrivedEntity_Columns.Column.DateColumnType")

            //        .Include("Table.Column.StringColumnType").Include("Table.Column.NumericColumnType")
            //        .Include("Table.Column.DateColumnType");
            //    }
            //}
            //else
            //{
            //    if (relationshipInfoType == EntityRelationshipInfoType.WithRelationships)
            //    {
            //        listEntity = projectContext.TableDrivedEntity.Include("Table.DBSchema.DatabaseInformation")

            //        .Include("Relationship.TableDrivedEntity.Table.DBSchema.DatabaseInformation.DBServer.LinkedServer")
            //        .Include("Relationship.TableDrivedEntity1.Table.DBSchema.DatabaseInformation.DBServer.LinkedServer")
            //        .Include("Relationship.RelationshipColumns.Column")
            //        .Include("Relationship.RelationshipType");
            //    }
            //    else
            listEntity = projectContext.TableDrivedEntity.Include("Table.DBSchema.DatabaseInformation");
            //}
            if (ID != 0)
                listEntity = listEntity.Where(x => x.ID == ID);

            if (isView != null)
                listEntity = listEntity.Where(x => x.IsView == isView.Value);
            return listEntity;
        }

        internal void UpdateEntityInitiallySearch(MyProjectEntities projectContext, int entityID, bool item2)
        {
            var dbEntity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
            dbEntity.SearchInitially = item2;
        }

        internal bool DecideEntityIsInitialySearched(TableDrivedEntityDTO entity, List<TableDrivedEntityDTO> allEntities)
        {
            return entity.IsDataReference == true;
        }

        public DataEntryEntityDTO GetDataEntryEntity(DR_Requester requester, int entityID, DataEntryRelationshipDTO parentRelationship = null)
        {
            var entity = GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships);
            var finalEntity = CheckDataEntryPermission(requester, entity, true, parentRelationship);
            return ToDataEntryEntityDTO(requester, finalEntity, parentRelationship);
        }

        private DataEntryEntityDTO ToDataEntryEntityDTO(DR_Requester requester, TableDrivedEntityDTO finalEntity, DataEntryRelationshipDTO parentRelationship)
        {
            DataEntryEntityDTO result = new DataEntryEntityDTO();
            result.ParentDataEntryRelationship = parentRelationship;
            result.IsReadonly = finalEntity.IsReadonly;
            foreach (var column in finalEntity.Columns)
                result.Columns.Add(column);
            foreach (var relationship in finalEntity.Relationships)
            {
                if (parentRelationship != null)
                    if (IsReverseRelation(parentRelationship.Relationship, relationship))
                        continue;


                DataEntryRelationshipDTO relItem = new DataEntryRelationshipDTO();
                relItem.Relationship = relationship;
                if (relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                {
                    relItem.DataMode = DataMode.Multiple;
                }
                else
                    relItem.DataMode = DataMode.One;


                if (parentRelationship != null && parentRelationship.DataMode == DataMode.Multiple)
                    relItem.Relationship.IsOtherSideDirectlyCreatable = false;

                if (relItem.Relationship.IsOtherSideCreatable == true)
                {
                    if (relItem.Relationship.IsOtherSideDirectlyCreatable == true)
                    {
                        if (relItem.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                            relItem.IntracionMode = IntracionMode.CreateDirect;
                        else
                            relItem.IntracionMode = IntracionMode.CreateSelectDirect;
                    }
                    else
                    {
                        if (relItem.Relationship.TypeEnum != Enum_RelationshipType.OneToMany)
                            relItem.IntracionMode = IntracionMode.CreateSelectInDirect;
                        else
                            relItem.IntracionMode = IntracionMode.CreateInDirect;
                    }
                }
                else
                {
                    if (relItem.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                    {
                        result.SkippedRelationships.Add(relationship);
                        continue;
                    }
                    else
                        relItem.IntracionMode = IntracionMode.Select;
                }

                //این پایین اومده چون ممکنه پارامترها که بالا ست شدند هم دخیل بشن. فعلا دخلی نشدند
                if (SkipableRelationship(parentRelationship, relationship))
                {
                    result.SkippedRelationships.Add(relationship);
                    continue;
                }

                if (relItem.IntracionMode == IntracionMode.CreateDirect ||
                  relItem.IntracionMode == IntracionMode.CreateSelectDirect)
                {
                    if (RelationHistoryContainsEntityID(parentRelationship, relItem.Relationship.EntityID2))
                    {
                        if (relItem.IntracionMode == IntracionMode.CreateDirect)
                        {
                            relItem.TempViewBecauseOfRelationHistory = true;
                            relItem.IntracionMode = IntracionMode.CreateInDirect;
                        }
                        else if (relItem.IntracionMode == IntracionMode.CreateSelectDirect)
                        {
                            relItem.TempViewBecauseOfRelationHistory = true;
                            relItem.IntracionMode = IntracionMode.CreateSelectInDirect;
                        }
                    }
                }
                if (relItem.IntracionMode == IntracionMode.CreateDirect ||
                relItem.IntracionMode == IntracionMode.CreateSelectDirect)
                {
                    relItem.TargetDataEntryEntity = GetDataEntryEntity(requester, relationship.EntityID2, relItem);
                }
                result.Relationships.Add(relItem);
            }
            return result;
        }
        private bool SkipableRelationship(DataEntryRelationshipDTO parentRelationship, RelationshipDTO relationship)
        {
            if (parentRelationship != null)
            {
                //bool logicallyMandatory = RelationshipColumnControlIsLogicallyMandatory(relationshipColumnControl);
                if (parentRelationship.IntracionMode == IntracionMode.CreateDirect ||
                   parentRelationship.IntracionMode == IntracionMode.CreateSelectDirect)
                {
                    //اگه تمپ ویو بود همه کنترلهای سطح اول نمایش داده میشوند

                    //این خصوصیت قابل مقداردهی توسط کاربر می باشد 
                    if (!relationship.IsNotSkippable)
                    {
                        if (relationship.TypeEnum == Enum_RelationshipType.SubToSuper
                            || relationship.TypeEnum == Enum_RelationshipType.SuperToSub
                            || relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion
                            || relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion)
                        {
                            if (relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
                                return true;
                            else if (relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
                            {
                                var isaRelationship = (relationship as SuperToSubRelationshipDTO).ISARelationship;

                                int parentIsaRelationshipID = 0;
                                if (parentRelationship.Relationship is SubToSuperRelationshipDTO)
                                    parentIsaRelationshipID = (parentRelationship.Relationship as SubToSuperRelationshipDTO).ISARelationship.ID;
                                if (isaRelationship.ID == parentIsaRelationshipID)
                                {
                                    if (isaRelationship.IsDisjoint)
                                        return true;
                                    else
                                        return relationship.Entity1IsIndependent || relationship.Entity2IsIndependent;
                                }
                                else
                                {
                                    if (isaRelationship.IsTolatParticipation)
                                    {
                                        //مثل شخص ورود اطلاعات حقیقی یا حقوقی واجب است
                                        return false;
                                    }
                                    else
                                        return relationship.Entity1IsIndependent || relationship.Entity2IsIndependent;
                                }
                            }
                            else if (relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
                            {
                                var unionRelationship = (relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
                                return unionRelationship.IsTolatParticipation;
                            }
                            else
                            {
                                //relationshipColumnControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub
                                var unionRelationship = (relationship as UnionToSubUnionRelationshipDTO).UnionRelationship;

                                int parentUnionRelationshipID = 0;
                                if (parentRelationship.Relationship is SubUnionToSuperUnionRelationshipDTO)
                                    parentUnionRelationshipID = (parentRelationship.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship.ID;
                                if (unionRelationship.ID == parentUnionRelationshipID)
                                {
                                    return true;
                                }
                                else
                                    return relationship.Entity1IsIndependent || relationship.Entity2IsIndependent;
                            }
                        }
                        else
                        {
                            return !relationship.IsOtherSideMandatory;

                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }
        private static bool IsReverseRelation(RelationshipDTO relationship1, RelationshipDTO relationship2)
        {
            if ((relationship1.PairRelationshipID == relationship2.ID) || (relationship2.PairRelationshipID == relationship1.ID))
                return true;
            return false;
        }
        private static bool RelationHistoryContainsEntityID(DataEntryRelationshipDTO parentRelationship, int entityID)
        {
            if (parentRelationship != null && parentRelationship.TargetDataEntryEntity != null)
            {
                if (parentRelationship.IntracionMode != IntracionMode.CreateDirect &&
                    parentRelationship.IntracionMode != IntracionMode.CreateSelectDirect)
                    return false;
                else if (parentRelationship.Relationship.EntityID2 == entityID)
                {
                    return true;
                }
                else
                {
                    return RelationHistoryContainsEntityID(parentRelationship.TargetDataEntryEntity.ParentDataEntryRelationship, entityID);

                }

            }
            else
                return false;
        }
        public TableDrivedEntityDTO GetPermissionedEntity(DR_Requester requester, int entityID)
        {
            var entity = GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships);
            return CheckDataEntryPermission(requester, entity, false);
        }

        private TableDrivedEntityDTO CheckDataEntryPermission(DR_Requester requester, TableDrivedEntityDTO entity, bool dataEntry, DataEntryRelationshipDTO parentRelationship = null)
        {
            //باید روابط اجباری همه برای ورود اطلاعلات فعال باشند. بعداً این کنترلها چک شود
            BizColumn bizColumn = new MyModelManager.BizColumn();
            BizRelationship bizRelationship = new BizRelationship();
            List<ColumnDTO> InValidColumns = new List<ColumnDTO>();
            List<RelationshipDTO> InValidRelationships = new List<RelationshipDTO>();

            entity.IsReadonly = DataIsReadonly(requester, entity.ID);

            foreach (var rel in entity.Relationships)
            {
                bool relAccess = bizRelationship.DataIsAccessable(requester, rel.ID, false, true);
                if (relAccess && dataEntry)
                {
                    relAccess = rel.DataEntryEnabled;
                }
                if (!relAccess)
                {
                    InValidRelationships.Add(rel);
                }
            }
            foreach (var removeRel in InValidRelationships)
            {
                entity.Relationships.Remove(removeRel);
                foreach (var column in removeRel.RelationshipColumns.Where(x => !x.FirstSideColumn.PrimaryKey))
                    entity.Columns.Remove(entity.Columns.First(x => x.ID == column.FirstSideColumnID));
            }
            foreach (var column in entity.Columns)
            {
                if (!column.PrimaryKey)
                {
                    bool colAccess = bizColumn.DataIsAccessable(requester, column.ID);
                    if (colAccess && dataEntry)
                    {
                        colAccess = column.DataEntryEnabled;
                    }
                    if (!colAccess)
                        InValidColumns.Add(column);
                }
            }
            foreach (var removeCol in InValidColumns)
            {
                entity.Columns.Remove(removeCol);
            }



            if (entity.IsReadonly)
            {
                foreach (var relationship in entity.Relationships.Where(x => x.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary))
                {
                    relationship.IsReadonly = true;
                }
                foreach (var column in entity.Columns)
                {
                    column.IsReadonly = true;
                }
            }
            else
            {
                foreach (var relationship in entity.Relationships)
                {
                    relationship.IsReadonly = bizRelationship.DataIsReadonly(requester, relationship.ID);
                }

                foreach (var column in entity.Columns)
                {
                    column.IsReadonly = bizColumn.DataIsReadonly(requester, column.ID);
                    if (!column.IsReadonly)
                    {
                        if (parentRelationship != null)
                        {
                            if (parentRelationship.Relationship.RelationshipColumns.Any(x => x.SecondSideColumnID == column.ID))
                            {
                                if (parentRelationship.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
                                    column.IsReadonly = true;
                            }
                        }
                    }
                }
            }
            return entity;
        }

        public List<TableDrivedEntityDTO> GetOrginalEntitiesWithoutUIComposition(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entities = GetEntities(projectContext, columnInfoType, relationshipInfoType, false)
                    .Where(x => !x.EntityUIComposition.Any(e => e.TableDrivedEntityID == x.ID) && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
                foreach (var entity in entities)
                    result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false, true));
            }
            return result;
        }
        public List<TableDrivedEntityDTO> GetOrginalEntitiesWithoutDefaultListView(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool? isView)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entities = GetEntities(projectContext, columnInfoType, relationshipInfoType, isView)
                    .Where(x => x.IsView == false && x.EntityListViewID == null && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
                foreach (var entity in entities)
                    result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false, true));
            }
            return result;
        }
        public List<TableDrivedEntityDTO> GetOrginalEntitiesWithoutDefaultSearchList(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool? isView)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entities = GetEntities(projectContext, columnInfoType, relationshipInfoType, isView)
                    .Where(x => x.IsView == false && x.EntitySearchID == null && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
                foreach (var entity in entities)
                    result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false, true));
            }
            return result;
        }
        public List<int> GetEntityIDs(int databaseID)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entities = GetEntities(projectContext, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, null)
                    .Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID).ToList();
                foreach (var entity in entities)
                {
                    result.Add(entity.ID);
                }
            }
            return result;
        }
        public List<TableDrivedEntityDTO> GetOrginalEntities(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool? isView)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entities = GetEntities(projectContext, columnInfoType, relationshipInfoType, isView)
                    .Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true).ToList();
                foreach (var entity in entities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = entities.Count(), CurrentProgress = entities.IndexOf(entity) + 1 });
                    result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false, true));
                }
            }
            return result;
        }
        //public List<TableDrivedEntityDTO> GetOrginalViews(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var entities = GetEntities(projectContext, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships)
        //            .Where(x => x.IsView == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        foreach (var entity in entities)
        //            result.Add(ToTableDrivedEntityDTO(entity, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships, false, true));
        //    }
        //    return result;
        //}
        //public TableDrivedEntityDTO GetOrginalEntity(string entityName, int databaseID)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.Name == entityName && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        return ToTableDrivedEntityDTO(entity, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithoutRelationships, false, true);
        //    }
        //}

        //public bool OrginalEntityExists(string entityName, int databaseID)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        return projectContext.TableDrivedEntity.Any(x => x.Name == entityName && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //    }
        //}

        //public List<TableDrivedEntityDTO> GetEnabledOrginalEntities(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var list = projectContext.TableDrivedEntity.Where(x => x.IsView == false && x.IsEnabled == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        foreach (var item in list)
        //        {
        //            result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships));
        //        }
        //    }
        //    return result;
        //}

        //public List<string> GetOriginalEntityNames(int databaseID)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        return projectContext.TableDrivedEntity.Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true).Select(x => x.Name).ToList();
        //    }
        //}
        public void UpdateTablesIsDataReferenceProperty(int databaseID, List<TableDrivedEntityDTO> listEntities)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                foreach (var item in listEntities)
                {
                    var dbEntity = projectContext.TableDrivedEntity.First(x => x.ID == item.ID);
                    dbEntity.IsDataReference = item.IsDataReference;
                    //dbEntity.Reviewed = true;
                }
                if (ItemImportingStarted != null)
                    ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Saving changes..." });
                projectContext.SaveChanges();
            }
        }
        public void UpdateTableIndependentDataEntryProperty(int databaseID, List<TableImportItem> listEntities)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                foreach (var item in listEntities)
                {
                    var dbEntity = projectContext.TableDrivedEntity.First(x => x.ID == item.Entity.ID);
                    dbEntity.IndependentDataEntry = item.Entity.IndependentDataEntry;
                    //dbEntity.SearchInitially = item.Entity.SearchInitially;
                    if (dbEntity.IndependentDataEntry == false)
                    {
                        foreach (var rel in item.Relationships)
                        {
                            var dbRel = projectContext.Relationship.First(x => x.ID == rel.ID);
                            if (rel.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
                                dbRel.RelationshipType.PKToFKDataEntryEnabled = rel.Select;
                            else
                            {
                                foreach (var relCol in dbRel.RelationshipColumns)
                                {
                                    relCol.Column.DataEntryEnabled = rel.Select;
                                }
                            }
                        }
                    }
                    //dbEntity.Reviewed = true;
                }
                if (ItemImportingStarted != null)
                    ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Saving changes..." });
                projectContext.SaveChanges();
            }
        }

        //public List<TableImportItem> GetEnabledWithNullIsIndependent(int databaseID)
        //{
        //    List<TableImportItem> result = new List<TableImportItem>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var list = GetEntities(projectContext, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships).Where(x => x.IsOrginal == true && x.IsView == false && x.IsEnabled == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IndependentDataEntry == null);
        //        foreach (var item in list)
        //        {
        //            result.Add(new TableImportItem(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships, false, true), false, ""));
        //        }
        //    }
        //    return result;
        //}


        //public List<TableImportItem> GetEnabledNotIndependentEntities(int databaseID)
        //{
        //    List<TableImportItem> result = new List<TableImportItem>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var list = GetEntities(projectContext, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships).Where(x => x.IsOrginal == true && x.Reviewed == false && x.IsView == false
        //        && x.IsEnabled == true && x.Table.DBSchema.DatabaseInformationID == databaseID &&
        //        x.IndependentDataEntry == false && x.Relationship1.Any() && x.Relationship1.Where(z => z.TypeEnum == 2).All(y => y.DataEntryEnabled == false));
        //        foreach (var item in list)
        //        {
        //            result.Add(new TableImportItem(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships, false, true), false, ""));
        //        }
        //    }
        //    return result;
        //}
        public bool ExistsEnabledEntitiesWithNullDataReference(int databaseID)
        {
            List<TableImportItem> result = new List<TableImportItem>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var list = GetEntities(projectContext, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships, false).Where(x => x.IsOrginal == true && x.IsDisabled == false && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsDataReference == null);
                return list.Any();
            }
            //return result;
        }
        public bool ExistsEnabledEntitiesWithNullIndependentProperty(int databaseID)
        {
            List<TableImportItem> result = new List<TableImportItem>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var list = GetEntities(projectContext, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships, false).Where(x => x.IsOrginal == true && x.IsDisabled == false && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IndependentDataEntry == null);
                return list.Any();
            }
            //return result;
        }
        //public List<TableImportItem> GetEnabledEntitiesWithNullDataReference(int databaseID)
        //{
        //    List<TableImportItem> result = new List<TableImportItem>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var list = GetEntities(projectContext, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships).Where(x => x.IsOrginal == true && x.IsView == false && x.IsEnabled == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsDataReference == null);
        //        foreach (var item in list)
        //        {
        //            result.Add(new TableImportItem(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships, false, true), false, ""));
        //        }
        //    }
        //    return result;
        //}

        //public List<TableDrivedEntityDTO> GetEnabledOrginalViews(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var list = projectContext.TableDrivedEntity.Where(x => x.IsView == true && x.IsEnabled == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        foreach (var item in list)
        //        {
        //            result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships));
        //        }
        //    }
        //    return result;
        //}
        //public string GetCatalog(int entityID)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        return projectContext.TableDrivedEntity.First(x => x.ID == entityID).Table.Catalog;
        //    }
        //}
        //public List<TableDrivedEntityDTO> GetAllEntities(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        //string catalogName = GeneralHelper.GetCatalogName(serverName, dbName);
        //        var listEntity = projectContext.TableDrivedEntity.Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
        //        foreach (var item in listEntity)
        //            result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships));
        //    }
        //    return result;
        //}



        //public TableDrivedEntityDTO GetBaseEntity(int entityID)
        //{

        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
        //        if (string.IsNullOrEmpty(entity.Criteria))
        //            return ToTableDrivedEntityDTO(entity, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships);
        //        else
        //            return ToTableDrivedEntityDTO(projectContext.TableDrivedEntity.First(x => (x.Criteria == null || x.Criteria == "") && x.TableID == entity.TableID), EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships);
        //    }
        //}

        public bool IsEntityEnabled(int entityID)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
                return entity.IsDisabled == false;
            }
        }
        //public bool IsEntityReadonly(int entityID)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
        //        return entity.IsEnabled == true;
        //    }
        //}
        public List<int> GetOtherDrivedEntityIDs(ISARelationshipDTO isaRelationship)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                //if (withInheritance)
                //{
                var isa = projectContext.ISARelationship.First(x => x.ID == isaRelationship.ID);
                var list = isa.SuperToSubRelationshipType.Select(x => x.RelationshipType.Relationship.TableDrivedEntity1);
                foreach (var item in list)
                {
                    result.Add(item.ID);
                }
                //}
                //else
                //{
                //    var list = projectContext.TableDrivedEntity.Where(x => x.ID != entityID && x.TableID == tableID);
                //    foreach (var item in list)
                //    {
                //        result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships));
                //    }
                //}
            }
            return result;
        }
        public int GetDefaultEntityID(int tableID)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                return projectContext.Table.First(x => x.ID == tableID).TableDrivedEntity.First().ID;

            }
        }
        public string GetTableName(int entityID)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                return projectContext.TableDrivedEntity.First(x => x.ID == entityID).Table.Name;

            }
        }




        //public TableDrivedEntityDTO GetTableDrivedEntity(int databaseID, string entityName, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool onlyEnabledColumns = true)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var table = GetEntities(projectContext, columnInfoType, relationshipInfoType).FirstOrDefault(x => x.Table.DBSchema.DatabaseInformationID == databaseID && x.Name == entityName);
        //        if (table == null)
        //            return null;
        //        else return
        //               ToTableDrivedEntityDTO(table, columnInfoType, relationshipInfoType, onlyEnabledColumns);
        //    }
        //}
        public TableDrivedEntityDTO GetSimpleEntityWithColumns(DR_Requester requester, int entityID, List<SecurityAction> specificActions = null)
        {
            return GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships, specificActions);
        }
        public TableDrivedEntityDTO GetSimpleEntity(DR_Requester requester, int entityID, List<SecurityAction> specificActions = null)
        {
            return GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, specificActions);
        }
        //public TableDrivedEntityDTO GetSimpleEntity(DR_Requester requester, int entityID)
        //{
        //    return GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, securityMode);
        //}
        public TableDrivedEntityDTO GetTableDrivedEntity(DR_Requester requester, int entityID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, List<SecurityAction> specificActions = null)
        {
            //List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Entity, entityID.ToString(), columnInfoType.ToString(), relationshipInfoType.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as TableDrivedEntityDTO);

            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entity = GetEntities(projectContext, columnInfoType, relationshipInfoType, null, entityID).FirstOrDefault();
                if (!DataIsAccessable(requester, entity, specificActions))
                {
                    return null;
                }
                else
                {
                    var result = ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, true, false);
                    return result;
                }
            }
        }

        public TableDrivedEntityDTO GetPermissionedEntityByName(DR_Requester requester, int databaseID, string entityName)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var entity = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == entityName);
                if (entity != null)
                    return GetPermissionedEntity(requester, entity.ID);
                else
                    return null;
            }
        }

        //این متود باید یواش یواش پرایوت شود یا حذف شود
        //public TableDrivedEntityDTO GetTableDrivedEntity(int entityID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        //{
        //    //List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Entity, entityID.ToString(), columnInfoType.ToString(), relationshipInfoType.ToString());
        //    //if (cachedItem != null)
        //    //    return (cachedItem as TableDrivedEntityDTO);

        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var table = GetEntities(projectContext, columnInfoType, relationshipInfoType, null).FirstOrDefault(x => x.ID == entityID);
        //        if (table == null)
        //            return null;
        //        else return
        //               ToTableDrivedEntityDTO(table, columnInfoType, relationshipInfoType, true, false);
        //    }
        //}
        private TableDrivedEntityDTO ToTableDrivedEntityDTO(DataAccess.TableDrivedEntity item, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool specializeRelationships, bool tableColumns)
        {

            // ستونهای disable از لیست ستونها حذف میشوند


            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Entity, item.ID.ToString(), columnInfoType.ToString(), relationshipInfoType.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as TableDrivedEntityDTO);


            TableDrivedEntityDTO result = new TableDrivedEntityDTO();
            result.Name = item.Name;
            result.ID = item.ID;
            result.TableID = item.TableID;
            result.TableName = item.Table.Name;
            result.IsView = item.IsView;
            //    result.SelectAsComboBox = item.SelectAsComboBox;
            result.Description = item.Description;
            //    result.Schema = item.Table.DBSchema.Name;
            result.EntitySearchID = item.EntitySearchID ?? 0;
            result.EntityListViewID = item.EntityListViewID ?? 0;
            result.DatabaseID = item.Table.DBSchema.DatabaseInformationID;
            result.DatabaseName = item.Table.DBSchema.DatabaseInformation.Name;
            result.RelatedSchemaID = item.Table.DBSchemaID;
            result.RelatedSchema = item.Table.DBSchema.Name;
            result.ServerID = item.Table.DBSchema.DatabaseInformation.DBServerID;
            if (item.SuperToSubRelationshipType != null)
            {
                BizISARelationship bizRelationship = new BizISARelationship();
                result.InternalSuperToSubRelationship = bizRelationship.ToSuperToSubRelationshipDTO(item.SuperToSubRelationshipType);
            }
            result.Alias = item.Alias;
            if (string.IsNullOrEmpty(result.Alias))
                result.Alias = item.Name;
            //   result.DeterminerColumnID = item.DeterminerColumnID ?? 0;

            BizColumn bizColumn = new BizColumn();

            //    if (item.DeterminerColumnID != null)
            //            result.DeterminerColumn = bizColumn.ToColumnDTO(item.Column, true);
            //result.DeterminerColumnValue = item.DeterminerColumnValue;
            //   result.Criteria = item.Criteria;
            //result.SecurityObjectID = item.SecurityObjectID.Value;
            //if (result.UnionTypeEntities == "")
            //    if (item.Relationship.Any(x => (x.RelationshipType == null && x.Relationship2 != null && x.TableDrivedEntity != x.TableDrivedEntity1 && !x.RelationshipColumns.All(y => y.Column.PrimaryKey == true))
            //        || (x.Relationship2 == null && x.TableDrivedEntity != x.TableDrivedEntity1 && !x.RelationshipColumns.All(y => y.Column1.PrimaryKey == true))))
            //        result.UnionTypeEntities = "Choose UnionType";
            result.Reviewed = item.Reviewed;
            result.ColumnsReviewed = item.ColumnsReviewed;
            result.Color = item.Color;
            result.IsDisabled = item.IsDisabled;
            result.IsReadonly = item.IsReadonly;
            result.SearchInitially = item.SearchInitially;
            result.LoadArchiveRelatedItems = item.LoadArchiveRelatedItems;
            result.LoadLetterRelatedItems = item.LoadLetterRelatedItems;
            result.BatchDataEntry = item.BatchDataEntry;
            result.IsAssociative = item.IsAssociative;
            result.IsDataReference = item.IsDataReference;
            result.IndependentDataEntry = item.IndependentDataEntry;
            result.IsStructurReferencee = item.IsStructurReferencee;
            if (columnInfoType != EntityColumnInfoType.WithoutColumn)
            {
                //foreach (var det in item.EntityDeterminer)
                //{
                //    result.EntityDeterminers.Add(new EntityDeterminerDTO() { ID = det.ID, Value = det.Value });
                //}

                List<Column> columns = null;
                if (tableColumns)
                {
                    columns = item.Table.Column.ToList();
                }
                else
                {
                    if (item.TableDrivedEntity_Columns.Count > 0)
                    {
                        columns = item.TableDrivedEntity_Columns.Select(x => x.Column).ToList();
                    }
                    else
                    {
                        columns = item.Table.Column.ToList();
                    }
                }
                // ستونهای disable از لیست ستونها حذف میشوند
                columns = columns.Where(x => x.IsDisabled == false).OrderBy(x => x.Position).ToList();
                foreach (var column in columns)
                {
                    var columnDTO = bizColumn.ToColumnDTO(column, columnInfoType == EntityColumnInfoType.WithSimpleColumns);

                    result.Columns.Add(columnDTO);
                }
                //if (columnInfoType == EntityColumnInfoType.WithSimpleColumns)
                //    result.Columns = bizColumn.GetColumns(item, true, true);
                //else if (columnInfoType == EntityColumnInfoType.WithFullColumns)
                //    result.Columns = bizColumn.GetColumns(item, false, true);
                //else if (columnInfoType == EntityColumnInfoType.WithSimpleColumnsEvenDisableds)
                //    result.Columns = bizColumn.GetColumns(item, false, false);
                //else if (columnInfoType == EntityColumnInfoType.WithFullColumnsEvenDisableds)
                //    result.Columns = bizColumn.GetColumns(item, false, false);
            }
            if (relationshipInfoType == EntityRelationshipInfoType.WithRelationships)
            {
                BizISARelationship bizISARelationship = new BizISARelationship();
                BizUnionRelationship bizUnionRelationship = new MyModelManager.BizUnionRelationship();
                BizRelationship bizRelationship = new BizRelationship();
                foreach (var relationship in item.Relationship.Where(x => x.Removed != true))
                {
                    var relationshipDTO = bizRelationship.ToRelationshipDTO(relationship, true);

                    if (specializeRelationships)
                    {
                        if (relationship.RelationshipType.OneToManyRelationshipType != null)
                            result.Relationships.Add(bizRelationship.ToOneToManyRelationship(relationship.RelationshipType.OneToManyRelationshipType, relationshipDTO));
                        else if (relationship.RelationshipType.ManyToOneRelationshipType != null)
                            result.Relationships.Add(bizRelationship.ToManyToOneRelationshipDTO(relationship.RelationshipType.ManyToOneRelationshipType, relationshipDTO));
                        else if (relationship.RelationshipType.ImplicitOneToOneRelationshipType != null)
                            result.Relationships.Add(bizRelationship.ToImplicitOneToOneRelationshipDTO(relationship.RelationshipType.ImplicitOneToOneRelationshipType, relationshipDTO));
                        else if (relationship.RelationshipType.ExplicitOneToOneRelationshipType != null)
                            result.Relationships.Add(bizRelationship.ToExplicitOneToOneRelationshipDTO(relationship.RelationshipType.ExplicitOneToOneRelationshipType, relationshipDTO));
                        else if (relationship.RelationshipType.SuperToSubRelationshipType != null)
                            result.Relationships.Add(bizISARelationship.ToSuperToSubRelationshipDTO(relationship.RelationshipType.SuperToSubRelationshipType, relationshipDTO));
                        else if (relationship.RelationshipType.SubToSuperRelationshipType != null)
                            result.Relationships.Add(bizISARelationship.ToSubToSuperRelationshipDTO(relationship.RelationshipType.SubToSuperRelationshipType, relationshipDTO));
                        else if (relationship.RelationshipType.UnionToSubUnionRelationshipType != null)
                            result.Relationships.Add(bizUnionRelationship.ToSuperUnionToSubUnionRelationshipDTO(relationship.RelationshipType.UnionToSubUnionRelationshipType, relationshipDTO));
                        else if (relationship.RelationshipType.SubUnionToUnionRelationshipType != null)
                            result.Relationships.Add(bizUnionRelationship.ToSubUnionToSuperUnionRelationshipDTO(relationship.RelationshipType.SubUnionToUnionRelationshipType, relationshipDTO));
                        else
                            result.Relationships.Add(relationshipDTO);
                    }
                    else
                        result.Relationships.Add(relationshipDTO);
                }
            }

            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Entity, item.ID.ToString(), columnInfoType.ToString(), relationshipInfoType.ToString());
            return result;
        }
        public void UpdateModel(int databaseID, List<TableDrivedEntityDTO> listNew, List<TableDrivedEntityDTO> listEdit, List<TableDrivedEntityDTO> listDeleted)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                //bool showNullValue = false;
                //var database = projectContext.DatabaseInformation.First(x => x.ID == databaseID);
                //if(database.DatabaseUISetting!=null && database.DatabaseUISetting.ShowNullValue)


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
                    var dbEntity = projectContext.TableDrivedEntity.FirstOrDefault(x => x.ID == deleteEntity.ID);
                    dbEntity.IsDisabled = true;
                }
                if (ItemImportingStarted != null)
                    ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Saving changes..." });
                projectContext.SaveChanges();
            }
        }

        private void UpdateEntityInModel(MyProjectEntities projectContext, int databaseID, TableDrivedEntityDTO entity, List<DBSchema> listAddedSchema)
        {

            DBSchema dbSchema = null;
            dbSchema = listAddedSchema.FirstOrDefault(x => x.DatabaseInformationID == databaseID && x.Name == entity.RelatedSchema);
            if (dbSchema == null)
                dbSchema = projectContext.DBSchema.FirstOrDefault(x => x.DatabaseInformationID == databaseID && x.Name == entity.RelatedSchema);
            if (dbSchema == null)
            {
                dbSchema = projectContext.DBSchema.Create();
                dbSchema.DatabaseInformationID = databaseID;
                dbSchema.Name = entity.RelatedSchema;
                dbSchema.SecurityObject = new SecurityObject();
                dbSchema.SecurityObject.Type = (int)DatabaseObjectCategory.Schema;
                projectContext.DBSchema.Add(dbSchema);
                listAddedSchema.Add(dbSchema);
            }

            var table = projectContext.Table.Include("TableDrivedEntity")
                    .Include("Column.StringColumnType").Include("Column.NumericColumnType")
                    .Include("Column.DateColumnType")
                   .FirstOrDefault(x => x.Name == entity.Name && x.DBSchema.DatabaseInformationID == databaseID);
            TableDrivedEntity tdEntity = null;
            if (table == null)
            {
                table = new Table();
                projectContext.Table.Add(table);
                table.Name = entity.Name;
                tdEntity = new TableDrivedEntity();
                tdEntity.Name = table.Name;
                tdEntity.SecurityObject = new SecurityObject();
                tdEntity.SecurityObject.Type = (int)DatabaseObjectCategory.Entity;
                tdEntity.IsOrginal = true;
                tdEntity.IsReadonly = entity.IsView;
                tdEntity.IsView = entity.IsView;
                table.TableDrivedEntity.Add(tdEntity);
            }
            else
                tdEntity = table.TableDrivedEntity.First(x => x.IsOrginal == true);

            //if (string.IsNullOrEmpty(table.Alias))
            //    table.Alias = entity.Alias;
            // if (string.IsNullOrEmpty(tdEntity.Alias))

            //if (tdEntity.Reviewed == false)
            //{
            if (tdEntity.ID == 0)
            {
                tdEntity.Alias = string.IsNullOrEmpty(entity.Alias) ? entity.Name : entity.Alias;
                tdEntity.Description = entity.Description;
            }
            //if (string.IsNullOrEmpty(entity.Alias))
            //    tdEntity.Alias = entity.Name;
            //else
            //    tdEntity.Alias = entity.Alias;

            //if (string.IsNullOrEmpty(entity.Description))

            //}
            //  else
            //  {

            // }

            //   table.TableDrivedEntity.First(x => x.IsOrginal == true).Alias = entity.Alias;



            table.DBSchema = dbSchema;
            foreach (var column in entity.Columns)
            {
                Column dbColumn = table.Column.FirstOrDefault(x => x.Name == column.Name);
                if (dbColumn == null)
                {
                    dbColumn = new Column();
                    dbColumn.SecurityObject = new SecurityObject();
                    dbColumn.SecurityObject.Type = (int)DatabaseObjectCategory.Column;
                    dbColumn.Name = column.Name;
                    dbColumn.DataEntryEnabled = true;

                    if (!string.IsNullOrEmpty(column.DBFormula) ||
                        (!string.IsNullOrEmpty(column.DefaultValue) && DefaultValueIsDBFunction(column)))
                    {
                        dbColumn.IsReadonly = true;
                        //چون اگه تو حالت اصلاح بود بتونه ببینه داده رو

                    }
                    else
                    {
                        dbColumn.IsReadonly = false;
                    }
                    table.Column.Add(dbColumn);
                }
                if (dbColumn.ID == 0)
                {
                    dbColumn.Alias = string.IsNullOrEmpty(column.Alias) ? column.Name : column.Alias;
                    dbColumn.Description = column.Description;
                }

                dbColumn.DataType = column.DataType;
                dbColumn.PrimaryKey = column.PrimaryKey;
                dbColumn.IsNull = column.IsNull;
                dbColumn.IsMandatory = !column.IsNull;
                dbColumn.IsIdentity = column.IsIdentity;
                dbColumn.Position = column.Position;
                dbColumn.DefaultValue = column.DefaultValue;
                //if (column.OriginalColumnType == Enum_ColumnType.None ||
                //   column.ColumnType == Enum_ColumnType.None)
                //{
                //    throw (new Exception("نوع ستون" + " " + column.Name + " " + "در جدول" + " " + entity.Name + " " + "مشخص نشده است"));
                //}
                dbColumn.TypeEnum = Convert.ToByte(column.ColumnType);
                dbColumn.OriginalTypeEnum = Convert.ToByte(column.OriginalColumnType);
                if (column.ColumnType == Enum_ColumnType.Date)
                {
                    if (dbColumn.DateColumnType == null)
                        dbColumn.DateColumnType = new DateColumnType();
                    dbColumn.DateColumnType.ShowMiladiDateInUI = column.DateColumnType.ShowMiladiDateInUI;
                    dbColumn.DateColumnType.StringDateIsMiladi = column.DateColumnType.StringDateIsMiladi;
                    if (column.OriginalColumnType == Enum_ColumnType.String)
                    {
                        if (dbColumn.StringColumnType == null)
                            dbColumn.StringColumnType = new StringColumnType();
                        dbColumn.StringColumnType.MaxLength = column.StringColumnType.MaxLength;
                        RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.Date, Enum_ColumnType.String });

                    }
                    else
                        RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.Date });
                }
                else if (column.ColumnType == Enum_ColumnType.String)
                {
                    if (dbColumn.StringColumnType == null)
                        dbColumn.StringColumnType = new StringColumnType();
                    dbColumn.StringColumnType.MaxLength = column.StringColumnType.MaxLength;
                    RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.String });
                }
                else if (column.ColumnType == Enum_ColumnType.Time)
                {
                    if (dbColumn.TimeColumnType == null)
                        dbColumn.TimeColumnType = new TimeColumnType();
                    dbColumn.TimeColumnType.ShowMiladiTime = column.TimeColumnType.ShowMiladiTime;
                    dbColumn.TimeColumnType.ShowAMPMFormat = column.TimeColumnType.ShowAMPMFormat;
                    dbColumn.TimeColumnType.StringTimeIsMiladi = column.TimeColumnType.StringTimeIsMiladi;
                    dbColumn.TimeColumnType.StringTimeISAMPMFormat = column.TimeColumnType.StringTimeISAMPMFormat;
                    if (column.OriginalColumnType == Enum_ColumnType.String)
                    {
                        if (dbColumn.StringColumnType == null)
                            dbColumn.StringColumnType = new StringColumnType();
                        dbColumn.StringColumnType.MaxLength = column.StringColumnType.MaxLength;
                        RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.Time, Enum_ColumnType.String });
                    }
                    else
                        RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.Time });
                }
                else if (column.ColumnType == Enum_ColumnType.DateTime)
                {
                    if (dbColumn.DateTimeColumnType == null)
                        dbColumn.DateTimeColumnType = new DateTimeColumnType();
                    dbColumn.DateTimeColumnType.ShowMiladiDateInUI = column.DateTimeColumnType.ShowMiladiDateInUI;
                    dbColumn.DateTimeColumnType.ShowAMPMFormat = column.DateTimeColumnType.ShowAMPMFormat;
                    dbColumn.DateTimeColumnType.HideTimePicker = column.DateTimeColumnType.HideTimePicker;
                    dbColumn.DateTimeColumnType.StringDateIsMiladi = column.DateTimeColumnType.StringDateIsMiladi;
                    dbColumn.DateTimeColumnType.StringTimeIsMiladi = column.DateTimeColumnType.StringTimeIsMiladi;
                    dbColumn.DateTimeColumnType.StringTimeISAMPMFormat = column.DateTimeColumnType.StringTimeISAMPMFormat;
                    if (column.OriginalColumnType == Enum_ColumnType.String)
                    {
                        if (dbColumn.StringColumnType == null)
                            dbColumn.StringColumnType = new StringColumnType();
                        dbColumn.StringColumnType.MaxLength = column.StringColumnType.MaxLength;
                        RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.DateTime, Enum_ColumnType.String });
                    }
                    else
                        RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.DateTime });
                }
                else if (column.ColumnType == Enum_ColumnType.Numeric)
                {
                    if (dbColumn.NumericColumnType == null)
                        dbColumn.NumericColumnType = new NumericColumnType();

                    dbColumn.NumericColumnType.Precision = column.NumericColumnType.Precision;
                    dbColumn.NumericColumnType.Scale = column.NumericColumnType.Scale;
                    RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.Numeric });
                }

                dbColumn.DBCalculateFormula = column.DBFormula;

                //dbColumn.ShowNullValue=
                //if (column.IsDBCalculatedColumn)
                //{
                //    dbColumn.IsDBCalculatedColumn = true;
                //    if (dbColumn.DBCalculatedColumn == null)
                //        dbColumn.DBCalculatedColumn = new DBCalculatedColumn();
                //    dbColumn.DBCalculatedColumn.Formula = column.DBFormula;
                //}
                //else
                //{
                //    dbColumn.IsDBCalculatedColumn = false;
                //    if (dbColumn.DBCalculatedColumn != null)
                //        projectContext.DBCalculatedColumn.Remove(dbColumn.DBCalculatedColumn);
                //}
            }
            var columnNames = entity.Columns.Select(x => x.Name).ToList();
            foreach (var dbColumn in table.Column.Where(x => !columnNames.Contains(x.Name)))
            {
                dbColumn.IsDisabled = true;
            }
            //throw new Exception("asdasdasd");
        }

        private void RemoveColumnTypes(MyProjectEntities projectContext, Column dbColumn, List<Enum_ColumnType> exceptionTypes)
        {
            if (!exceptionTypes.Contains(Enum_ColumnType.Numeric))
            {
                if (dbColumn.NumericColumnType != null)
                    projectContext.NumericColumnType.Remove(dbColumn.NumericColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.DateTime))
            {
                if (dbColumn.DateTimeColumnType != null)
                    projectContext.DateTimeColumnType.Remove(dbColumn.DateTimeColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.Date))
            {
                if (dbColumn.DateColumnType != null)
                    projectContext.DateColumnType.Remove(dbColumn.DateColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.Time))
            {
                if (dbColumn.TimeColumnType != null)
                    projectContext.TimeColumnType.Remove(dbColumn.TimeColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.String))
            {
                if (dbColumn.StringColumnType != null)
                    projectContext.StringColumnType.Remove(dbColumn.StringColumnType);
            }
        }

        private bool DefaultValueIsDBFunction(ColumnDTO column)
        {
            if (column.DefaultValue != null && column.DefaultValue.Contains("()"))
                return true;
            else
                return false;
        }




        //private string[] GetCacheKeys(EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        //{
        //    List<string> list = new List<string>();
        //    if(columnInfoType==EntityColumnInfoType.WithFullColumns)
        //    {
        //        list.Add(EntityColumnInfoType.WithoutColumn.ToString());
        //        list.Add(EntityColumnInfoType.WithSimpleColumns.ToString());
        //    }
        //    else if (columnInfoType == EntityColumnInfoType.WithSimpleColumns)
        //    {
        //        list.Add(EntityColumnInfoType.WithoutColumn.ToString());

        //    }
        //}



        //public List<ColumnDTO> GetColumnsSimple(TableDrivedEntity entity)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        if (entity.Column.Count > 0)
        //        {
        //            return GetColumnsSimple(entity.Column);
        //        }
        //        else
        //        {
        //            return GetColumnsSimple(entity.Table.Column);
        //        }
        //    }
        //}


        public void Save(List<TableDrivedEntityDTO> entities)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                foreach (var entity in entities)
                {
                    var dbEntity = projectContext.TableDrivedEntity.First(x => x.ID == entity.ID);
                    dbEntity.Alias = entity.Alias;
                    dbEntity.Name = entity.Name;
                    //   dbEntity.Criteria = entity.Criteria;
                    dbEntity.Reviewed = true;
                    dbEntity.IndependentDataEntry = entity.IndependentDataEntry;
                    dbEntity.BatchDataEntry = entity.BatchDataEntry;
                    dbEntity.IsAssociative = entity.IsAssociative;
                    dbEntity.IsDisabled = entity.IsDisabled;
                    dbEntity.Color = entity.Color;
                    dbEntity.Description = entity.Description;
                    dbEntity.IsReadonly = entity.IsReadonly;
                    // dbEntity.SelectAsComboBox = entity.SelectAsComboBox;
                    dbEntity.IsDataReference = entity.IsDataReference;
                    dbEntity.IsStructurReferencee = entity.IsStructurReferencee;
                    dbEntity.SearchInitially = entity.SearchInitially;
                    dbEntity.LoadArchiveRelatedItems = entity.LoadArchiveRelatedItems;
                    dbEntity.LoadLetterRelatedItems = entity.LoadLetterRelatedItems;
                }
                projectContext.SaveChanges();
            }
        }
        public bool Save(DR_Requester requester, EditBaseEntityDTO message)
        {
            try
            {
                using (var projectContext = new DataAccess.MyProjectEntities())
                {
                    var dbBaseEntity = projectContext.TableDrivedEntity.First(x => x.ID == message.BaseEntity.ID);
                    foreach (var item in message.BaseEntity.Relationships)
                    {
                        var dbRelationship = projectContext.Relationship.First(x => x.ID == item.ID);
                        Relationship dbReverseRelationship = dbRelationship.Relationship2;
                        dbRelationship.TableDrivedEntityID1 = message.BaseEntity.ID;
                        dbReverseRelationship.TableDrivedEntityID2 = message.BaseEntity.ID;
                        dbRelationship.Name = message.BaseEntity.Name + ">" + dbRelationship.TableDrivedEntity1.Name;
                        dbReverseRelationship.Name = dbReverseRelationship.TableDrivedEntity.Name + ">" + message.BaseEntity.Name;
                    }

                    if (message.DrivedEntities.Any())
                    {
                        var listRemoveColumn = dbBaseEntity.TableDrivedEntity_Columns.Where(x => !message.BaseEntity.Columns.Any(y => y.ID == x.ColumnID)).ToList();
                        foreach (var removeCol in listRemoveColumn)
                            projectContext.TableDrivedEntity_Columns.Remove(removeCol);
                        foreach (var item in message.BaseEntity.Columns)
                        {
                            var entityColumn = dbBaseEntity.TableDrivedEntity_Columns.FirstOrDefault(x => x.TableDrivedEntityID == message.BaseEntity.ID && x.ColumnID == item.ID);
                            if (entityColumn == null)
                            {
                                entityColumn = new TableDrivedEntity_Columns();
                                entityColumn.ColumnID = item.ID;
                                dbBaseEntity.TableDrivedEntity_Columns.Add(entityColumn);
                            }
                        }
                        DataAccess.ISARelationship isaRelationship = null;
                        if (message.ISARelationship.ID == 0)
                        {
                            isaRelationship = new ISARelationship();
                            isaRelationship.Name = "ISAinTable" + "_" + dbBaseEntity.Name;
                            projectContext.ISARelationship.Add(isaRelationship);
                        }
                        else
                            isaRelationship = projectContext.ISARelationship.First(x => x.ID == message.ISARelationship.ID);
                        isaRelationship.InternalTable = true;
                        isaRelationship.IsTolatParticipation = message.ISARelationship.IsTolatParticipation;
                        isaRelationship.IsDisjoint = message.ISARelationship.IsDisjoint;

                        List<TableDrivedEntity> listDbDrivedEntities = new List<TableDrivedEntity>();
                        foreach (var rel in isaRelationship.SuperToSubRelationshipType)
                        {
                            var dbDrivedEntity = projectContext.TableDrivedEntity.FirstOrDefault(x => x.InternalTableSuperToSubRelID == rel.ID);
                            if (dbDrivedEntity != null)
                                listDbDrivedEntities.Add(dbDrivedEntity);
                        }
                        var listRemoveEntity = listDbDrivedEntities.Where(x => !message.DrivedEntities.Any(y => y.Item3.ID == x.ID)).ToList();
                        foreach (var entity in listRemoveEntity)
                        {
                            RemoveDrivedEntity(projectContext, entity, isaRelationship);
                        }
                        List<Tuple<Relationship, Relationship>> listCreatedRelationships = new List<Tuple<Relationship, Relationship>>();
                        List<Tuple<TableDrivedEntity, SuperToSubRelationshipType>> createdEntities = new List<Tuple<TableDrivedEntity, SuperToSubRelationshipType>>();
                        List<int> reviedRelationshipIDs = new List<int>();

                        foreach (var drived in message.DrivedEntities)
                        {
                            TableDrivedEntity dbDrived = null;
                            if (drived.Item3.ID == 0)
                            {
                                dbDrived = new TableDrivedEntity();
                                dbDrived.IndependentDataEntry = true;
                                dbDrived.SecurityObject = new SecurityObject();
                                dbDrived.SecurityObject.Type = (int)DatabaseObjectCategory.Entity;
                                dbDrived = projectContext.TableDrivedEntity.Add(dbDrived);

                            }
                            else
                                dbDrived = projectContext.TableDrivedEntity.First(x => x.ID == drived.Item3.ID);
                            // dbDrived.DeterminerColumnID = drived.DeterminerColumnID;
                            //dbDrived.DeterminerColumnValue = drived.DeterminerColumnValue;
                            dbDrived.Name = drived.Item3.Name;
                            dbDrived.Alias = drived.Item3.Alias;
                            dbDrived.TableID = dbBaseEntity.TableID;


                            SuperToSubRelationshipType superToSubRelationshipType = null;
                            if (dbDrived.ID == 0)
                            {
                                var tuple = AddISARelationship(projectContext, isaRelationship, drived.Item1, drived.Item2, dbBaseEntity, dbDrived);
                                superToSubRelationshipType = tuple.Item1.RelationshipType.SuperToSubRelationshipType;
                                listCreatedRelationships.Add(tuple);
                                //      dbDrived.SuperToSubRelationshipType = superToSubRelationshipType;

                                createdEntities.Add(new Tuple<TableDrivedEntity, SuperToSubRelationshipType>(dbDrived, superToSubRelationshipType));
                            }
                            else
                            {
                                superToSubRelationshipType = dbDrived.SuperToSubRelationshipType;
                            }
                            if (superToSubRelationshipType != null)
                            {
                                superToSubRelationshipType.SuperEntityDeterminerColumnID = drived.Item1.SuperEntityDeterminerColumnID;
                                while (superToSubRelationshipType.SuperToSubDeterminerValue.Any())
                                    projectContext.SuperToSubDeterminerValue.Remove(superToSubRelationshipType.SuperToSubDeterminerValue.First());
                                foreach (var detRecord in drived.Item1.DeterminerColumnValues)
                                {
                                    superToSubRelationshipType.SuperToSubDeterminerValue.Add(new SuperToSubDeterminerValue() { DeterminerValue = detRecord.Value });
                                }
                            }
                            foreach (var item in drived.Item3.Relationships)
                            {
                                var dbRelationship = projectContext.Relationship.First(x => x.ID == item.ID);
                                Relationship dbReverseRelationship = dbRelationship.Relationship2;
                                if (reviedRelationshipIDs.Contains(item.ID))
                                {
                                    BizRelationship bizRelationship = new BizRelationship();
                                    var RelTuple = bizRelationship.CopyRelationshipTuple(projectContext, dbRelationship);
                                    dbRelationship = RelTuple.Item1;
                                    dbReverseRelationship = RelTuple.Item2;
                                    listCreatedRelationships.Add(RelTuple);
                                }

                                dbRelationship.TableDrivedEntity = dbDrived;
                                dbReverseRelationship.TableDrivedEntity1 = dbDrived;
                                dbReverseRelationship.Alias = dbDrived.Alias;

                                dbRelationship.Name = drived.Item3.Name + ">" + dbRelationship.TableDrivedEntity1.Name;
                                dbReverseRelationship.Name = dbReverseRelationship.TableDrivedEntity.Name + ">" + drived.Item3.Name;
                                reviedRelationshipIDs.Add(item.ID);

                            }
                            var listDrivedRemoveColumn = dbDrived.TableDrivedEntity_Columns.Where(x => !drived.Item3.Columns.Any(y => y.ID == x.ColumnID)).ToList();
                            foreach (var removeCol in listDrivedRemoveColumn)
                                projectContext.TableDrivedEntity_Columns.Remove(removeCol);
                            foreach (var item in drived.Item3.Columns)
                            {
                                var entityColumn = dbDrived.TableDrivedEntity_Columns.FirstOrDefault(x => x.TableDrivedEntityID == drived.Item3.ID && x.ColumnID == item.ID);
                                if (entityColumn == null)
                                {
                                    entityColumn = new TableDrivedEntity_Columns();
                                    entityColumn.ColumnID = item.ID;
                                    dbDrived.TableDrivedEntity_Columns.Add(entityColumn);
                                }
                            }

                        }

                        try
                        {
                            projectContext.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine(@"Entity of type ""{0}"" in state ""{1}"" 
                   has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name,
                                    eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine(@"- Property: ""{0}"", Error: ""{1}""",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                        catch (DbUpdateException e)
                        {
                            //Add your code to inspect the inner exception and/or
                            //e.Entries here.
                            //Or just use the debugger.
                            //Added this catch (after the comments below) to make it more obvious 
                            //how this code might help this specific problem
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            throw;
                        }
                        foreach (var item in listCreatedRelationships)
                        {
                            item.Item1.RelationshipID = item.Item2.ID;
                            item.Item2.RelationshipID = item.Item1.ID;
                        }
                        foreach (var entity in createdEntities)
                        {
                            entity.Item1.SuperToSubRelationshipType = entity.Item2;
                        }
                        try
                        {
                            projectContext.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine(@"Entity of type ""{0}"" in state ""{1}"" 
                   has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name,
                                    eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine(@"- Property: ""{0}"", Error: ""{1}""",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                        catch (DbUpdateException e)
                        {
                            //Add your code to inspect the inner exception and/or
                            //e.Entries here.
                            //Or just use the debugger.
                            //Added this catch (after the comments below) to make it more obvious 
                            //how this code might help this specific problem
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            throw;
                        }

                        BizEntityUIComposition bizEntityUIComposition = new BizEntityUIComposition();
                        if (createdEntities.Any())
                        {
                            bizEntityUIComposition.UpdateUIComposition(requester, message.BaseEntity.ID);

                            BizEntitySettings bizEntitySettings = new MyModelManager.BizEntitySettings();
                            bizEntitySettings.UpdateDefaultSettingsInModel(requester, createdEntities.Select(x => x.Item1.ID).ToList());
                        }

                    }
                    else
                    {
                        //foreach (var item in message.BaseEntity.Columns)
                        //{
                        if (dbBaseEntity.IsOrginal)
                        {
                            var listRemoveColumn = dbBaseEntity.TableDrivedEntity_Columns.ToList();
                            foreach (var removeCol in listRemoveColumn)
                                projectContext.TableDrivedEntity_Columns.Remove(removeCol);
                        }
                        //}
                        if (message.ISARelationship.ID != 0)
                        {
                            var isaRelationship = projectContext.ISARelationship.First(x => x.ID == message.ISARelationship.ID);
                            List<TableDrivedEntity> listEntities = new List<TableDrivedEntity>();
                            foreach (var rel in isaRelationship.SuperToSubRelationshipType)
                                listEntities.Add(rel.RelationshipType.Relationship.TableDrivedEntity1);
                            foreach (var entity in listEntities)
                                RemoveDrivedEntity(projectContext, entity, isaRelationship);
                            projectContext.ISARelationship.Remove(isaRelationship);
                        }
                        projectContext.SaveChanges();
                    }

                }

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
            catch (Exception ex)
            {

            }

            //if (!(message.ID == message.BaseEntity.ID))
            //{
            //    DataAccess.ISARelationship isaRelationship = null;

            //    var sampleSuperToSub = projectContext.Relationship.FirstOrDefault(x => x.TableDrivedEntityID1 == message.BaseEntity.ID && x.RelationshipType != null &&
            //        x.RelationshipType.SuperToSubRelationshipType != null);
            //    if (sampleSuperToSub != null)
            //        isaRelationship = sampleSuperToSub.RelationshipType.SuperToSubRelationshipType.ISARelationship;

            //    if (isaRelationship == null)
            //    {
            //        isaRelationship = new ISARelationship();
            //    }
            //    isaRelationship.IsTolatParticipation = message.IsTolatParticipation;
            //    isaRelationship.IsDisjoint = message.IsDisjoint;

            //    string subTypesStr = "";
            //    foreach (var relationship in isaRelationship.SuperToSubRelationshipType)
            //    {
            //        if (relationship.RelationshipType.Relationship.TableDrivedEntity1.ID != message.BaseEntity.ID)
            //            subTypesStr += (subTypesStr == "" ? "" : ",") + relationship.RelationshipType.Relationship.TableDrivedEntity1.Name;
            //    }
            //    if (message.ID == 0)
            //        subTypesStr += (subTypesStr == "" ? "" : ",") + message.Name;
            //    isaRelationship.Name = message.BaseEntity.Name + ">" + subTypesStr;

            //    if (message.ID == 0)
            //    {
            //        var relationship = new Relationship();
            //        relationship.RelationshipType = new RelationshipType();
            //        relationship.RelationshipType.SuperToSubRelationshipType = new SuperToSubRelationshipType();
            //        relationship.RelationshipType.IsOtherSideCreatable = true;
            //        relationship.TableDrivedEntityID1 = message.BaseEntity.ID;
            //        relationship.RelationshipType.SuperToSubRelationshipType.ISARelationship = isaRelationship;
            //        //drivedEntity.Relationship1.Add(relationship);

            //        var relationshipReverse = new Relationship();
            //        relationshipReverse.RelationshipType = new RelationshipType();
            //        relationshipReverse.RelationshipType.SubToSuperRelationshipType = new SubToSuperRelationshipType();
            //        relationshipReverse.RelationshipType.IsOtherSideCreatable = true;
            //        relationshipReverse.TableDrivedEntityID2 = message.BaseEntity.ID;
            //        relationshipReverse.RelationshipType.SubToSuperRelationshipType.ISARelationship = isaRelationship;
            //        relationshipReverse.Relationship2 = relationship;
            //        //drivedEntity.Relationship.Add(relationshipReverse);

            //        var dbBaseEntity = projectContext.TableDrivedEntity.First(x => x.ID == message.BaseEntity.ID);
            //        string PKColumns = "";
            //        string FKColumns = "";
            //        foreach (var primaryCol in dbBaseEntity.Table.Column.Where(x => x.PrimaryKey == true))
            //        {
            //            PKColumns += (PKColumns == "" ? "" : ",") + primaryCol.Name;
            //            FKColumns += (FKColumns == "" ? "" : ",") + primaryCol.Name;
            //            relationship.RelationshipColumns.Add(new RelationshipColumns() { Column = primaryCol, Column1 = primaryCol });
            //            relationshipReverse.RelationshipColumns.Add(new RelationshipColumns() { Column = primaryCol, Column1 = primaryCol });
            //        }
            //        relationship.Name = "(PK)" + message.BaseEntity.Name + "." + PKColumns + ">(FK)" + message.Name + "." + FKColumns;
            //        relationshipReverse.Name = "(FK)" + message.Name + "." + FKColumns + ">(PK)" + message.BaseEntity.Name + "." + PKColumns;

            //    }

            //}
            //if (drivedEntity.ID == 0)
            //    projectContext.TableDrivedEntity.Add(drivedEntity);
            //  projectContext.SaveChanges();
            return true;
        }

        private Tuple<Relationship, Relationship> AddISARelationship(MyProjectEntities projectContext, ISARelationship isaRelationship
            , SuperToSubRelationshipDTO superToSub, SubToSuperRelationshipDTO subToSuper, TableDrivedEntity dbBaseEntity, TableDrivedEntity dbDrivedEntity)
        {
            var relaitonship = new Relationship();
            //relaitonship.Enabled = true;

            relaitonship.SecurityObject = new SecurityObject();
            relaitonship.SecurityObject.Type = (int)DatabaseObjectCategory.Relationship;
            relaitonship.Name = superToSub.Name;
            relaitonship.Alias = superToSub.Alias;
            relaitonship.TableDrivedEntity = dbBaseEntity;
            relaitonship.TableDrivedEntity1 = dbDrivedEntity;
            var pkColumns = dbBaseEntity.Table.Column.Where(x => x.PrimaryKey);
            foreach (var col in pkColumns)
                relaitonship.RelationshipColumns.Add(new RelationshipColumns() { FirstSideColumnID = col.ID, SecondSideColumnID = col.ID });
            relaitonship.RelationshipType = new RelationshipType();
            relaitonship.RelationshipType.PKToFKDataEntryEnabled = true;
            relaitonship.RelationshipType.IsOtherSideCreatable = true;
            relaitonship.RelationshipType.IsOtherSideDirectlyCreatable = true;
            relaitonship.RelationshipType.SuperToSubRelationshipType = new SuperToSubRelationshipType() { ISARelationship = isaRelationship };
            relaitonship.TypeEnum = (int)Enum_RelationshipType.SuperToSub;
            relaitonship.MasterTypeEnum = (int)Enum_MasterRelationshipType.FromPrimartyToForeign;
            projectContext.Relationship.Add(relaitonship);

            var reverserelaitonship = new Relationship();
            //reverserelaitonship.Enabled = true;
            // reverserelaitonship.DataEntryEnabled = true;
            reverserelaitonship.Name = subToSuper.Name;
            reverserelaitonship.Alias = subToSuper.Alias;
            reverserelaitonship.SecurityObject = new SecurityObject();
            reverserelaitonship.SecurityObject.Type = (int)DatabaseObjectCategory.Relationship;
            reverserelaitonship.TableDrivedEntity = dbDrivedEntity;
            reverserelaitonship.TableDrivedEntity1 = dbBaseEntity;
            foreach (var col in pkColumns)
                reverserelaitonship.RelationshipColumns.Add(new RelationshipColumns() { FirstSideColumnID = col.ID, SecondSideColumnID = col.ID });
            reverserelaitonship.RelationshipType = new RelationshipType();
            reverserelaitonship.RelationshipType.IsOtherSideMandatory = true;
            reverserelaitonship.RelationshipType.IsOtherSideCreatable = true;
            reverserelaitonship.RelationshipType.IsOtherSideDirectlyCreatable = true;
            reverserelaitonship.RelationshipType.SubToSuperRelationshipType = new SubToSuperRelationshipType() { ISARelationship = isaRelationship };
            reverserelaitonship.TypeEnum = (int)Enum_RelationshipType.SubToSuper;
            reverserelaitonship.MasterTypeEnum = (int)Enum_MasterRelationshipType.FromForeignToPrimary;
            projectContext.Relationship.Add(reverserelaitonship);



            //var entityState = new TableDrivedEntityState();
            //entityState.ColumnID = dbDrived.DeteminerColumnID;
            //entityState.Value = dbDrived.DeterminerColumnValue;
            //entityState.Title = entityState.ColumnID + ">" + entityState.Value;
            //var stateAction = new EntityState_UIActionActivity();
            //stateAction.UIActionActivity = new UIActionActivity();
            //stateAction.UIActionActivity.Title = relaitonship.Name + ">Hidden";
            //stateAction.UIActionActivity.UIEnablityDetails.Add(new UIEnablityDetails() { RelationshipID = relaitonship.ID, Hidden = true });
            //entityState.EntityState_UIActionActivity.Add(stateAction);
            //dbBaseEntity.TableDrivedEntityState.Add(entityState);


            return new Tuple<Relationship, Relationship>(relaitonship, reverserelaitonship);
        }

        private void RemoveDrivedEntity(MyProjectEntities projectContext, TableDrivedEntity entity, ISARelationship isaRelationship)
        {
            //  projectContext.TableDrivedEntity.Remove(entity);
            entity.IsDisabled = true;
            var superRel = entity.SuperToSubRelationshipType;
            var subRel = superRel.RelationshipType.Relationship.Relationship2.RelationshipType.SubToSuperRelationshipType;
            superRel.RelationshipType.Relationship.Relationship2 = null;
            subRel.RelationshipType.Relationship.Relationship2 = null;
            projectContext.SaveChanges();
            //foreach (var item in rel.RelationshipType.Relationship.UIEnablityDetails.Where(X => X.UIActionActivity.EntityState_UIActionActivity.Any(y => y.TableDrivedEntityState.TableDrivedEntityID == rel.RelationshipType.Relationship.TableDrivedEntityID1)).ToList())
            //{
            //    projectContext.UIEnablityDetails.Remove(item);
            //}

            while (superRel.SuperToSubDeterminerValue.Any())
                projectContext.SuperToSubDeterminerValue.Remove(superRel.SuperToSubDeterminerValue.First());
            entity.SuperToSubRelationshipType = null;

            var relType = superRel.RelationshipType;
            var relType1 = subRel.RelationshipType;

            while (superRel.RelationshipType.Relationship.RelationshipColumns.Any())
                projectContext.RelationshipColumns.Remove(superRel.RelationshipType.Relationship.RelationshipColumns.First());
            projectContext.Relationship.Remove(superRel.RelationshipType.Relationship);
            projectContext.RelationshipType.Remove(relType);
            projectContext.SuperToSubRelationshipType.Remove(superRel);

            while (subRel.RelationshipType.Relationship.RelationshipColumns.Any())
                projectContext.RelationshipColumns.Remove(subRel.RelationshipType.Relationship.RelationshipColumns.First());
            projectContext.Relationship.Remove(subRel.RelationshipType.Relationship);
            projectContext.RelationshipType.Remove(relType1);
            projectContext.SubToSuperRelationshipType.Remove(subRel);




        }





        //private string GetRelationshipName(Relationship dbRelationship)
        //{
        //    string name = "";
        //    if (dbRelationship.Name.Contains("="))
        //    {
        //        name = dbRelationship.Name.Split('=')[0];
        //    }
        //    if (dbRelationship.MasterTypeEnum ==)
        //        return "(PK)" + dbRelationship.TableDrivedEntity.Name + ".", "(PK)" + message.Name + ".");
        //    else
        //        return dbRelationship.Name = dbRelationship.Name.Replace("(FK)" + dbRelationship.TableDrivedEntity.Name + ".", "(FK)" + message.Name + ".");
        //}



        //public event EventHandler<SimpleGenerationInfoArg> RuleImposedEvent;
        public void ImposeEntityTableRule(int databaseID)
        {
            //string catalogName = GeneralHelper.GetCatalogName(serverName, dbName);
            MyProjectEntities context = new MyProjectEntities();
            context.Configuration.LazyLoadingEnabled = true;
            var list = context.TableDrivedEntity.Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
            var count = list.Count();
            int index = 0;
            foreach (var entity in list)
            {
                index++;
                Biz_RuleSet myruleSet = new Biz_RuleSet("TableDrivedEntity_IsReference");
                myruleSet.Execute(entity);
                //if (RuleImposedEvent != null)
                //    RuleImposedEvent(this, new SimpleGenerationInfoArg() { Title = entity.Name, CurrentProgress = index, TotalProgressCount = count });
            }
            //index = 0;
            //foreach (var entity in list)
            //{
            //    index++;
            //    View.SetInfo(count, index, entity.Name);
            //    Biz_RuleSet myruleSet = new Biz_RuleSet("TableDrivedEntity_IsAssociative");
            //    myruleSet.Execute(entity);
            //}
            //index = 0;
            //foreach (var entity in list)
            //{
            //    index++;
            //    View.SetInfo(count, index, entity.Name);
            //    Biz_RuleSet myruleSet = new Biz_RuleSet("TableDrivedEntity_IndependentDataEntry");
            //    myruleSet.Execute(entity);
            //}

            context.SaveChanges();
            //if (RuleImposedEvent != null)
            //    RuleImposedEvent(this, new SimpleGenerationInfoArg() { Title = "Operation is completed.", CurrentProgress = index, TotalProgressCount = count });
        }


    }

}

