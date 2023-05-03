﻿using DataAccess;
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
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return GetAllEnabledEntities(projectContext).First(x => x.ID == entityID).IndependentDataEntry == true;
            }
        }
        public IQueryable<TableDrivedEntity> GetAllEntitiesBase(MyIdeaEntities projectContext)
        {
            //fc78bf47-f1ed-4bee-b141-8b4f7f33ae0d
            IQueryable<TableDrivedEntity> listEntity;
            listEntity = projectContext.TableDrivedEntity.Include("Table.DBSchema.DatabaseInformation").Where(x =>
            x.Removed == false);
            return listEntity;
        }
        public IQueryable<TableDrivedEntity> GetAllEnabledEntities(MyIdeaEntities projectContext)
        {
            //53e163cd-40df-4694-8412-05df4ec41a23
            IQueryable<TableDrivedEntity> listEntity = GetAllEntitiesBase(projectContext).Where(x => x.IsDisabled == false);
            return listEntity;
        }
        public IQueryable<TableDrivedEntity> GetAllEntitiesExceptViews(MyIdeaEntities projectContext)
        {
            //BizTableDrivedEntity.GetAllEntitiesExceptViews: 1f144c7d-c4ba-4c79-a605-1ab97a637518
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == false);
        }
        public IQueryable<TableDrivedEntity> GetAllEnabledEntitiesExceptViews(MyIdeaEntities projectContext)
        {
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == false && x.IsDisabled == false);
        }
        public IQueryable<TableDrivedEntity> GetAllEnabledOrginalEntitiesExceptViews(MyIdeaEntities projectContext)
        {
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == false && x.IsDisabled == false && x.IsOrginal == true);
        }
        public IQueryable<TableDrivedEntity> GetAllOrginalEntitiesExceptViews(MyIdeaEntities projectContext)
        {
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == false && x.IsOrginal == true);
        }

        public IQueryable<TableDrivedEntity> GetAllEntitiesOnlyViewsBase(MyIdeaEntities projectContext)
        {
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == true);
        }
        public IQueryable<TableDrivedEntity> GetAllEnabledEntitiesOnlyViews(MyIdeaEntities projectContext)
        {
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == true && x.IsDisabled == false);
        }
        public IQueryable<TableDrivedEntity> GetAllEnabledOrginalEntitiesOnlyViews(MyIdeaEntities projectContext)
        {
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == true && x.IsDisabled == false && x.IsOrginal == true);
        }
        public IQueryable<TableDrivedEntity> GetAllOrginalEntitiesOnlyViews(MyIdeaEntities projectContext)
        {
            return GetAllEntitiesBase(projectContext).Where(x => x.IsView == true && x.IsOrginal == true);
        }

        //public IQueryable<TableDrivedEntity> GetAllEnabledViews(MyIdeaEntities projectContext)
        //{
        //    //fc78bf47-f1ed-4bee-b141-8b4f7f33ae0d
        //    return GetAllEnabledEntities(projectContext).Where(x => x.IsView == true);
        //}
        public List<TableDrivedEntityDTO> GetAllEntitiesDTO(int databaseID)
        {
            //بهتره خود انتیتی با دیتابیس رابطه داشته باشد
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntity = GetAllEntitiesBase(projectContext);
                listEntity = listEntity.Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
                foreach (var item in listEntity)
                    result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, false));
            }
            return result.OrderBy(x => x.RelatedSchema).ThenBy(x => x.Name).ToList();
        }
        public List<TableDrivedEntityDTO> GetAllEnabledEntitiesDTO(int databaseID)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntity = GetAllEnabledEntities(projectContext);
                listEntity = listEntity.Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
                foreach (var item in listEntity)
                    result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, false));
            }
            return result.OrderBy(x => x.RelatedSchema).ThenBy(x => x.Name).ToList();
        }

        //public List<TableDrivedEntityDTO> GetAllEnabledEntitiesExceptViewsDTO(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var listEntity = GetAllEnabledEntitiesExceptViews(projectContext).Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
        //        foreach (var item in listEntity)
        //            result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, false));
        //    }
        //    return result.OrderBy(x => x.RelatedSchema).ThenBy(x => x.Name).ToList();
        //}

        public List<TableDrivedEntityDTO> GetAllEnbaledEntitiesDTO(DR_Requester requester, string generalFilter, List<SecurityAction> specificActions = null)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntity = GetAllEnabledEntities(projectContext);
                if (generalFilter != "")
                    listEntity = listEntity.Where(x => x.ID.ToString() == generalFilter || x.Name.Contains(generalFilter) || x.Alias.Contains(generalFilter));

                foreach (var item in listEntity)
                {
                    if (DataIsAccessable(requester, item, specificActions))
                    {
                        var rItem = ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, false);
                        result.Add(rItem);
                    }
                }
            }

            return result;
        }
        public List<TableDrivedEntityDTO> GetAllEnabledEntitiesExceptViewsDTO(DR_Requester requester, string generalFilter, List<SecurityAction> specificActions = null)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntity = GetAllEnabledEntitiesExceptViews(projectContext);
                if (generalFilter != "")
                    listEntity = listEntity.Where(x => x.ID.ToString() == generalFilter || x.Name.Contains(generalFilter) || x.Alias.Contains(generalFilter));

                foreach (var item in listEntity)
                {
                    if (DataIsAccessable(requester, item, specificActions))
                    {
                        var rItem = ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships, false);
                        result.Add(rItem);
                    }
                }
            }

            return result;
        }
        //public List<TableDrivedEntityDTO> GetAllOrginalEntitiesDTO(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entities = GetAllEntitiesBase(projectContext)
        //            .Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true).ToList();
        //        foreach (var entity in entities)
        //        {
        //            if (ItemImportingStarted != null)
        //                ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = entities.Count(), CurrentProgress = entities.IndexOf(entity) + 1 });
        //            result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false));
        //        }
        //    }
        //    return result;
        //}
        public List<TableDrivedEntityDTO> GetAllEnbaledOrginalEntitiesExceptViewsDTO(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entities = GetAllEnabledOrginalEntitiesExceptViews(projectContext)
                    .Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID).ToList();
                foreach (var entity in entities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = entities.Count(), CurrentProgress = entities.IndexOf(entity) + 1 });
                    result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false));
                }
            }
            return result;
        }
        public List<TableDrivedEntityDTO> GetAllOrginalEntitiesExceptViewsDTO(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entities = GetAllOrginalEntitiesExceptViews(projectContext)
                    .Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID).ToList();
                foreach (var entity in entities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = entities.Count(), CurrentProgress = entities.IndexOf(entity) + 1 });
                    result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false));
                }
            }
            return result;
        }
        public List<TableDrivedEntityDTO> GetAllOrginalEntitiesOnlyViewsDTO(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        {
            List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entities = GetAllOrginalEntitiesOnlyViews(projectContext)
                    .Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true).ToList();
                foreach (var entity in entities)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Fetching entity" + " " + entity.Name, TotalProgressCount = entities.Count(), CurrentProgress = entities.IndexOf(entity) + 1 });
                    result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false));
                }
            }
            return result;
        }

        public List<int> GetOrginalEntitiesWithoutUIComposition(int databaseID)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return GetAllEnabledOrginalEntitiesExceptViews(projectContext)
                      .Where(x => !x.EntityUIComposition.Any() && x.Table.DBSchema.DatabaseInformationID == databaseID).Select(x => x.ID).ToList();
            }
        }
        public List<int> GetOrginalEntitiesWithoutEntitySearch(int databaseID)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return GetAllEnabledOrginalEntitiesExceptViews(projectContext)
                       .Where(x => !x.EntitySearch.Any() && x.Table.DBSchema.DatabaseInformationID == databaseID).Select(x => x.ID).ToList();
            }
        }


        public List<int> GetOrginalEntitiesWithoutEntityListView(int databaseID)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return GetAllEnabledOrginalEntitiesExceptViews(projectContext)
                       .Where(x => !x.EntityListView.Any() && x.Table.DBSchema.DatabaseInformationID == databaseID).Select(x => x.ID).ToList();

            }
        }
        public List<int> GetOrginalEntitiesWithoutInitiallySearch(int databaseID)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return GetAllEnabledOrginalEntitiesExceptViews(projectContext)
                     .Where(x => x.SearchInitially == null && x.Table.DBSchema.DatabaseInformationID == databaseID).Select(x => x.ID).ToList();
            }
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        returnGetAllEntities(projectContext, false).Where(x => checkEntityIDs.Contains(x.ID) && x.IsEnabled == false).Select(x => x.ID).ToList();
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
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);
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
        public bool EntityIsReadonly(DR_Requester requester, int entityID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);
                return EntityIsReadonly(requester, entity);
            }
        }
        public bool EntityIsReadonly(DR_Requester requester, TableDrivedEntity entity)
        {
            //** 224c5e70-1ba3-43d1-a47b-a8c7523b7591
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
        public bool DataHasNotDeleteAccess(DR_Requester requester, int entityID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);
                return DataHasNotDeleteAccess(requester, entity);
            }
        }
        public bool DataHasNotDeleteAccess(DR_Requester requester, TableDrivedEntity entity)
        {
            // a34e3c07-b7ca-4b78-9b00-f7b87aadf83b
            if (entity.IsReadonly)
                return true;
            else
            {
                if (requester.SkipSecurity)
                    return false;
                var permission = GetEntityAssignedPermissions(requester, entity.ID, false);
                if (permission.GrantedActions.Any(y => y == SecurityAction.EditAndDelete))
                    return true;
                else
                {
                    return false;
                }
            }
        }


        internal void UpdateEntityInitiallySearch(MyIdeaEntities projectContext, int entityID, bool item2)
        {
            var dbEntity = GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);
            dbEntity.SearchInitially = item2;
        }

        internal bool DecideEntityIsInitialySearched(int entityID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);
                return entity.IsDataReference == true;
            }
        }
        public DataEntryEntityDTO GetDataEntryEntity(DR_Requester requester, int entityID, EntityUICompositionDTO uiCompositionDTO)
        {
            return GetDataEntryEntity(requester, entityID, uiCompositionDTO, null);
        }
        public DataEntryEntityDTO GetDataEntryEntity(DR_Requester requester, int entityID, DataEntryRelationshipDTO parentRelationship = null)
        {
            return GetDataEntryEntity(requester, entityID, null, parentRelationship);
        }
        private DataEntryEntityDTO GetDataEntryEntity(DR_Requester requester, int entityID, EntityUICompositionDTO uiCompositionDTO, DataEntryRelationshipDTO parentRelationship = null)
        {
            //** BizTableDrivedEntity.GetDataEntryEntity: fedaef41c5c7

            var entity = GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships);
            var finalEntity = CheckDataEntryPermission(requester, entity, true, parentRelationship);

            DataEntryEntityDTO result = new DataEntryEntityDTO();

            result.ParentDataEntryRelationship = parentRelationship;
            result.IsReadonly = finalEntity.IsReadonly;
            result.HasNotDeleteAccess = finalEntity.HasNotDeleteAccess;
            foreach (var column in finalEntity.Columns)
                result.Columns.Add(column);
            foreach (var relationship in finalEntity.Relationships)
            {
                var skipMode = GetRelationshipSkipMode(parentRelationship, relationship);
                if (skipMode == RelationshipSkipMode.MustSkip)
                {
                    result.SkippedRelationships.Add(relationship);
                    continue;
                }
                //if (skipMode != RelationshipSkipMode.CanNotSkip && ShouldSkipRelationshipGeneral(parentRelationship, relationship))
                //{
                //    result.SkippedRelationships.Add(relationship);
                //    continue;
                //}
                DataEntryRelationshipDTO relItem = new DataEntryRelationshipDTO();
                relItem.Relationship = relationship;

                if (parentRelationship != null && parentRelationship.DataMode == DataMode.Multiple)
                    relItem.Relationship.IsOtherSideDirectlyCreatable = false;

                if (relItem.Relationship.IsOtherSideCreatable == true)
                {
                    if (relItem.Relationship.IsOtherSideDirectlyCreatable == true && !RelationHistoryContainsDirectEntityID(parentRelationship, relItem.Relationship.EntityID2))
                    {
                        //if (skipMode != RelationshipSkipMode.CanNotSkip && ShouldSkipRelationshipDirect(parentRelationship, relationship))
                        //{
                        //    result.SkippedRelationships.Add(relationship);
                        //    continue;
                        //}

                        if (relItem.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                            relItem.IntracionMode = IntracionMode.CreateDirect;
                        else
                            relItem.IntracionMode = IntracionMode.CreateSelectDirect;

                    }
                    else
                    {
                        if (relItem.Relationship.IsOtherSideDirectlyCreatable == true)
                            relItem.TempViewBecauseOfRelationHistory = true;

                        if (relItem.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                            relItem.IntracionMode = IntracionMode.CreateInDirect;
                        else
                            relItem.IntracionMode = IntracionMode.CreateSelectInDirect;

                    }
                }
                else
                {
                    if (relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                    {
                        result.SkippedRelationships.Add(relationship);
                        continue;
                    }
                    else
                        relItem.IntracionMode = IntracionMode.Select;
                }

                if (parentRelationship != null)
                {
                    if (ShouldSkipRelationshipDepended(parentRelationship, relationship, relItem.IntracionMode))
                    {
                        result.SkippedRelationships.Add(relationship);
                        continue;
                    }
                }

                if (relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                {
                    relItem.DataMode = DataMode.Multiple;
                }
                else
                    relItem.DataMode = DataMode.One;




                if (relItem.IntracionMode == IntracionMode.CreateDirect ||
                relItem.IntracionMode == IntracionMode.CreateSelectDirect)
                {
                    relItem.TargetDataEntryEntity = GetDataEntryEntity(requester, relationship.EntityID2, relItem);
                }
                result.Relationships.Add(relItem);



            }
            SetDataEntryEntityUIComposition(result, finalEntity, uiCompositionDTO);

            return result;


        }

        private void SetDataEntryEntityUIComposition(DataEntryEntityDTO dataEntryDTO, TableDrivedEntityDTO finalEntity, EntityUICompositionDTO currentUIComposition)
        {
            //** 5e0fbd74-a0e8-4c09-8904-59b9417736aa
            //اینجا باید بعدا تست بشه، موجودیت تغییر بکنه و انواع حالتهای اضافه شدن ستون و رابطه و یا رابطه شدن یک ستون قبلا موجود تست شود
            BizEntityUIComposition bizEntityUIComposition = new BizEntityUIComposition();
            if (currentUIComposition == null)
            {
                currentUIComposition = bizEntityUIComposition.GetOrCreateEntityUIComposition(finalEntity.ID, dataEntryDTO.Columns, dataEntryDTO.Relationships.Select(x => x.Relationship).ToList());
                //if (currentUIComposition == null)
                //{
                //    currentUIComposition = bizEntityUIComposition.GenerateUIComposition(finalEntity, finalEntity.Columns, finalEntity.Relationships);
                //}
            }

            //** 8400bf54-fed4-4226-8e61-0f2b5dd22bf3


            //foreach (var column in dataEntryDTO.Columns)
            //{
            //    if (dataEntryDTO.Relationships.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && x.Relationship.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID)))
            //        column.InvisibleInUI = true;
            //}
            SetColumnsAndRelationship(currentUIComposition, dataEntryDTO);
            dataEntryDTO.UICompositions = currentUIComposition;
        }

        private void SetColumnsAndRelationship(EntityUICompositionDTO currentUIComposition, DataEntryEntityDTO dataEntryDTO)
        {
            //** ed89f4f2-9a80-4cbf-be42-bb0b00630402
            if (currentUIComposition.ObjectCategory == DatabaseObjectCategory.Column)
            {
                //result.Add(currentUIComposition);
                var fColumn = dataEntryDTO.Columns.FirstOrDefault(x => x.ID == Convert.ToInt32(currentUIComposition.ObjectIdentity));
                if (fColumn != null)
                    currentUIComposition.Column = fColumn;
            }
            else if (currentUIComposition.ObjectCategory == DatabaseObjectCategory.Relationship)
            {
                //result.Add(currentUIComposition);
                var fRel = dataEntryDTO.Relationships.FirstOrDefault(x => x.Relationship.ID == Convert.ToInt32(currentUIComposition.ObjectIdentity));
                if (fRel != null)
                    currentUIComposition.Relationship = fRel;
                else
                {

                }

            }
            foreach (var cItem in currentUIComposition.ChildItems)
            {
                SetColumnsAndRelationship(cItem, dataEntryDTO);
            }
        }
        //private List<EntityUICompositionDTO> GetRelationships(EntityUICompositionDTO currentUIComposition, List<EntityUICompositionDTO> result = null)
        //{
        //    if (result == null)
        //        result = new List<EntityUICompositionDTO>();

        //    if (currentUIComposition.ObjectCategory == DatabaseObjectCategory.Relationship)
        //    {
        //        result.Add(currentUIComposition);
        //    }
        //    foreach (var cItem in currentUIComposition.ChildItems)
        //    {
        //        GetRelationships(cItem, result);
        //    }
        //    return result;
        //}
        //private DataEntryEntityDTO ToDataEntryEntityDTO(DR_Requester requester, TableDrivedEntityDTO finalEntity, DataEntryRelationshipDTO parentRelationship)
        //{

        //}

        private RelationshipSkipMode GetRelationshipSkipMode(DataEntryRelationshipDTO parentRelationship, RelationshipDTO relationship)
        {
            //** 2a1c0f40-2332-491a-a58d-20a74a0bbfe2
            if (parentRelationship != null)
            {
                if (IsReverseRelation(parentRelationship.Relationship, relationship))
                    return RelationshipSkipMode.MustSkip;

                if (relationship.TypeEnum == Enum_RelationshipType.SuperToSub
               || relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion
               || relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion)
                {
                    if (relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
                    {
                        var isaRelationship = (relationship as SuperToSubRelationshipDTO).ISARelationship;

                        if (parentRelationship.Relationship is SubToSuperRelationshipDTO &&
                            isaRelationship.ID == (parentRelationship.Relationship as SubToSuperRelationshipDTO).ISARelationship.ID)
                        {
                            // اگر پدر ساب به سوپر باشد و این رابطه از مابقی  سوپر به سابهای  همان رابطه ارث بری پدر باشد
                            //مثلا پدر شخص حقیقی به شخص باشد و این رابطه شخص به شخص حقوقی باشد
                            if (isaRelationship.IsDisjoint)
                                return RelationshipSkipMode.MustSkip;
                        }

                    }
                    else
                    {
                        var unionRelationship = (relationship as UnionToSubUnionRelationshipDTO).UnionRelationship;

                        if (parentRelationship.Relationship is SubUnionToSuperUnionRelationshipDTO
                            && unionRelationship.ID == (parentRelationship.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship.ID)
                        {
                            // اگر پدر زیر اتحاد به اتحاد باشد و این رابطه از مابقی  اتحاد به زیر اتحادهای  همان رابطه اتحاد پدر باشد
                            return RelationshipSkipMode.MustSkip;
                        }

                    }
                }
            }
            return RelationshipSkipMode.Depends;
        }

        private bool ShouldSkipRelationshipDepended(DataEntryRelationshipDTO parentRelationship, RelationshipDTO relationship, IntracionMode intracionMode)
        {
            //Entity2IsIndependent آیا این خصوصیت مناسبه یا وجود در منوها جایگزین بشه
            //BizTableDrivedEntity.ShouldSkipRelationshipDepended:  57991005621a
            if ((parentRelationship.IntracionMode == IntracionMode.CreateDirect || parentRelationship.IntracionMode == IntracionMode.CreateSelectDirect)
                && parentRelationship.Relationship.Entity2IsIndependent)
            {
                if (!relationship.IsNotSkippable && !relationship.IsOtherSideMandatory)
                    return true;
            }
            return false;
        }
        //private bool ShouldSkipRelationshipDirect(DataEntryRelationshipDTO parentRelationship, RelationshipDTO relationship)
        //{
        //    return relationship.Entity1IsIndependent || relationship.Entity2IsIndependent;
        //}

        private static bool IsReverseRelation(RelationshipDTO relationship1, RelationshipDTO relationship2)
        {
            if ((relationship1.PairRelationshipID == relationship2.ID) || (relationship2.PairRelationshipID == relationship1.ID))
                return true;
            return false;
        }
        private static bool RelationHistoryContainsDirectEntityID(DataEntryRelationshipDTO parentRelationship, int entityID)
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
                    return RelationHistoryContainsDirectEntityID(parentRelationship.TargetDataEntryEntity.ParentDataEntryRelationship, entityID);

                }

            }
            else
                return false;
        }
        public TableDrivedEntityDTO GetPermissionedEntity(DR_Requester requester, int entityID)
        {
            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.PermissionedEntity, entityID.ToString(), requester.Identity.ToString());
            if (cachedItem != null)
                return (cachedItem as TableDrivedEntityDTO);
            var entity = GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships);
            var result = CheckDataEntryPermission(requester, entity, false);
            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.PermissionedEntity, entityID.ToString(), requester.Identity.ToString());
            return result;
        }

        private TableDrivedEntityDTO CheckDataEntryPermission(DR_Requester requester, TableDrivedEntityDTO entity, bool dataEntry, DataEntryRelationshipDTO parentRelationship = null)
        {
            //** BizTableDrivedEntity.CheckDataEntryPermission: 8be394c313b9
            //باید روابط اجباری همه برای ورود اطلاعلات فعال باشند. بعداً این کنترلها چک شود
            BizColumn bizColumn = new MyModelManager.BizColumn();
            BizRelationship bizRelationship = new BizRelationship();
            List<ColumnDTO> InValidColumns = new List<ColumnDTO>();
            List<RelationshipDTO> InValidRelationships = new List<RelationshipDTO>();

            entity.IsReadonly = EntityIsReadonly(requester, entity.ID);
            entity.HasNotDeleteAccess = DataHasNotDeleteAccess(requester, entity.ID);

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


            //فقط خواندنی کردن ستونها وروابط
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



        public bool EntityWithoutSetting(int databaseID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                //foreach(var item in   bizTableDrivedEntity.GetAllEntities(projectContext, false).Where(x =>
                //x.IsOrginal == true && x.IsDisabled == false && x.Table.DBSchema.DatabaseInformationID == databaseID
                //&& ( !x.EntityListView.Any() || !x.EntityUIComposition.Any() || x.SearchInitially == null)))
                //{

                //}

                return GetAllEnabledOrginalEntitiesExceptViews(projectContext).Any(x =>
                 x.Table.DBSchema.DatabaseInformationID == databaseID
                && (!x.EntitySearch.Any() || !x.EntityListView.Any() || !x.EntityUIComposition.Any() || x.SearchInitially == null));
            }
        }

        //public List<TableDrivedEntityDTO> GetOrginalEntitiesWithoutDefaultListView(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool? isView)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entities = GetEntities(projectContext, columnInfoType, relationshipInfoType, isView)
        //            .Where(x => x.IsView == false && x.EntityListViewID == null && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        foreach (var entity in entities)
        //            result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false, true));
        //    }
        //    return result;
        //}
        //public List<TableDrivedEntityDTO> GetOrginalEntitiesWithoutDefaultSearchList(int databaseID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool? isView)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entities = GetEntities(projectContext, columnInfoType, relationshipInfoType, isView)
        //            .Where(x => x.IsView == false && x.EntitySearchID == null && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        foreach (var entity in entities)
        //            result.Add(ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, false, true));
        //    }
        //    return result;
        //}
        public List<int> GetEntityIDs(int databaseID)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entities = GetAllEnabledEntities(projectContext)
                    .Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID).ToList();
                foreach (var entity in entities)
                {
                    result.Add(entity.ID);
                }
            }
            return result;
        }

        //public List<TableDrivedEntityDTO> GetOrginalViews(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entity =GetAllEntities(projectContext, false).First(x => x.Name == entityName && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        return ToTableDrivedEntityDTO(entity, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithoutRelationships, false, true);
        //    }
        //}

        //public bool OrginalEntityExists(string entityName, int databaseID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        returnGetAllEntities(projectContext, false).Any(x => x.Name == entityName && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //    }
        //}

        //public List<TableDrivedEntityDTO> GetEnabledOrginalEntities(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var list =GetAllEntities(projectContext, false).Where(x => x.IsView == false && x.IsEnabled == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        foreach (var item in list)
        //        {
        //            result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships));
        //        }
        //    }
        //    return result;
        //}

        //public List<string> GetOriginalEntityNames(int databaseID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        returnGetAllEntities(projectContext, false).Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true).Select(x => x.Name).ToList();
        //    }
        //}
        public void UpdateTablesIsDataReferenceProperty(int databaseID, List<TableDrivedEntityDTO> listEntities)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var item in listEntities)
                {
                    var dbEntity = GetAllEnabledEntities(projectContext).First(x => x.ID == item.ID);
                    dbEntity.IsDataReference = item.IsDataReference;
                    //dbEntity.Reviewed = true;
                }
                if (ItemImportingStarted != null)
                    ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Saving changes..." });
                projectContext.SaveChanges();
            }
        }
        BizRelationship bizRelationship = new BizRelationship();
        public void UpdateTableIndependentDataEntryProperty(int databaseID, List<TableImportItem> listEntities)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var item in listEntities)
                {
                    var dbEntity = GetAllEnabledEntities(projectContext).First(x => x.ID == item.Entity.ID);
                    dbEntity.IndependentDataEntry = item.Entity.IndependentDataEntry;
                    //dbEntity.SearchInitially = item.Entity.SearchInitially;
                    if (dbEntity.IndependentDataEntry == false)
                    {
                        foreach (var rel in item.Relationships)
                        {
                            var dbRel = bizRelationship.GetAllEnabledRelationships(projectContext, true).First(x => x.ID == rel.ID);
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var list = GetAllEnabledEntities(projectContext).Where(x => x.IsOrginal == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsDataReference == null);
                return list.Any();
            }
            //return result;
        }
        public bool ExistsEnabledEntitiesWithNullIndependentProperty(int databaseID)
        {
            List<TableImportItem> result = new List<TableImportItem>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var list = GetAllEnabledEntities(projectContext).Where(x => x.IsOrginal == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IndependentDataEntry == null);
                return list.Any();
            }
            //return result;
        }
        //public List<TableImportItem> GetEnabledEntitiesWithNullDataReference(int databaseID)
        //{
        //    List<TableImportItem> result = new List<TableImportItem>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var list =GetAllEntities(projectContext, false).Where(x => x.IsView == true && x.IsEnabled == true && x.Table.DBSchema.DatabaseInformationID == databaseID && x.IsOrginal == true);
        //        foreach (var item in list)
        //        {
        //            result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships));
        //        }
        //    }
        //    return result;
        //}
        //public string GetCatalog(int entityID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        returnGetAllEntities(projectContext, false).First(x => x.ID == entityID).Table.Catalog;
        //    }
        //}
        //public List<TableDrivedEntityDTO> GetAllEntities(int databaseID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        //string catalogName = GeneralHelper.GetCatalogName(serverName, dbName);
        //        var listEntity =GetAllEntities(projectContext, false).Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
        //        foreach (var item in listEntity)
        //            result.Add(ToTableDrivedEntityDTO(item, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships));
        //    }
        //    return result;
        //}



        //public TableDrivedEntityDTO GetBaseEntity(int entityID)
        //{

        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entity =GetAllEntities(projectContext, false).First(x => x.ID == entityID);
        //        if (string.IsNullOrEmpty(entity.Criteria))
        //            return ToTableDrivedEntityDTO(entity, EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships);
        //        else
        //            return ToTableDrivedEntityDTO(projectContext.TableDrivedEntity.First(x => (x.Criteria == null || x.Criteria == "") && x.TableID == entity.TableID), EntityColumnInfoType.WithoutColumn, EntityRelationshipInfoType.WithoutRelationships);
        //    }
        //}

        //public bool IsEntityEnabled(int entityID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
        //        return entity.IsDisabled == false;
        //    }
        //}
        //public bool IsEntityReadonly(int entityID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
        //        return entity.IsEnabled == true;
        //    }
        //}
        public List<int> GetOtherDrivedEntityIDs(ISARelationshipDTO isaRelationship)
        {
            List<int> result = new List<int>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
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
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return projectContext.Table.First(x => x.ID == tableID).TableDrivedEntity.First().ID;

            }
        }
        //public string GetTableName(int entityID)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        return projectContext.TableDrivedEntity.First(x => x.ID == entityID).Table.Name;

        //    }
        //}




        //public TableDrivedEntityDTO GetTableDrivedEntity(int databaseID, string entityName, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool onlyEnabledColumns = true)
        //{
        //    List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
        public TableDrivedEntityDTO GetTableDrivedEntity(int entityID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, List<SecurityAction> specificActions = null)
        {
            DR_Requester requester = new DR_Requester();
            requester.SkipSecurity = true;
            return GetTableDrivedEntity(requester, entityID, columnInfoType, relationshipInfoType);
        }
        public TableDrivedEntityDTO GetTableDrivedEntity(DR_Requester requester, int entityID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, List<SecurityAction> specificActions = null)
        {
            //List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Entity, entityID.ToString(), columnInfoType.ToString(), relationshipInfoType.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as TableDrivedEntityDTO);

            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = GetAllEnabledEntities(projectContext).FirstOrDefault(x => x.ID == entityID);
                if (!DataIsAccessable(requester, entity, specificActions))
                {
                    return null;
                }
                else
                {
                    var result = ToTableDrivedEntityDTO(entity, columnInfoType, relationshipInfoType, true);
                    return result;
                }
            }
        }

        //public TableDrivedEntityDTO GetPermissionedEntityByName(DR_Requester requester, int databaseID, string entityName)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.FirstOrDefault(x => x.Name == entityName);
        //        if (entity != null)
        //            return GetPermissionedEntity(requester, entity.ID);
        //        else
        //            return null;
        //    }
        //}

        //این متود باید یواش یواش پرایوت شود یا حذف شود
        //public TableDrivedEntityDTO GetTableDrivedEntity(int entityID, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType)
        //{
        //    //List<TableDrivedEntityDTO> result = new List<TableDrivedEntityDTO>();
        //    //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Entity, entityID.ToString(), columnInfoType.ToString(), relationshipInfoType.ToString());
        //    //if (cachedItem != null)
        //    //    return (cachedItem as TableDrivedEntityDTO);

        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var table = GetEntities(projectContext, columnInfoType, relationshipInfoType, null).FirstOrDefault(x => x.ID == entityID);
        //        if (table == null)
        //            return null;
        //        else return
        //               ToTableDrivedEntityDTO(table, columnInfoType, relationshipInfoType, true, false);
        //    }
        //}
        private TableDrivedEntityDTO ToTableDrivedEntityDTO(DataAccess.TableDrivedEntity item, EntityColumnInfoType columnInfoType, EntityRelationshipInfoType relationshipInfoType, bool specializeRelationships)
        {
            // BizTableDrivedEntity.ToTableDrivedEntityDTO: 9ced8aaf7fb6
            // ستونهای disable از لیست ستونها حذف میشوند


            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Entity, item.ID.ToString(), columnInfoType.ToString(), relationshipInfoType.ToString());
            if (cachedItem != null)
                return (cachedItem as TableDrivedEntityDTO);


            TableDrivedEntityDTO result = new TableDrivedEntityDTO();
            result.Name = item.Name;
            result.ID = item.ID;
            result.TableID = item.TableID;
            result.TableName = item.Table.Name;
            result.IsView = item.IsView;
            //    result.SelectAsComboBox = item.SelectAsComboBox;
            result.Description = item.Description;
            //    result.Schema = item.Table.DBSchema.Name;
            //result.EntitySearchID = item.EntitySearchID ?? 0;
            //result.EntityListViewID = item.EntityListViewID ?? 0;
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
            //   result.ColumnsReviewed = item.ColumnsReviewed;
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

                var columns = bizColumn.GetAllEnabledColumns(item);
                //if (tableColumns)
                //{
                //    columns = item.Table.Column.ToList();
                //}
                //else
                //{
                //    if (item.TableDrivedEntity_Columns.Count > 0)
                //    {
                //        columns = item.TableDrivedEntity_Columns.Select(x => x.Column).ToList();
                //    }
                //    else
                //    {
                //        columns = item.Table.Column.ToList();
                //    }
                //}
                // ستونهای disable از لیست ستونها حذف میشوند
                columns = columns.OrderBy(x => x.Position).ToList();
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
                foreach (var relationship in bizRelationship.GetEnabledRelationships(item))
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
        public void UpdateModelFromTargetDB(int databaseID, List<TableDrivedEntityDTO> listNew, List<TableDrivedEntityDTO> listEdit, List<TableDrivedEntityDTO> listDeleted)
        {
            //** 1bcad93c-9349-4f31-a710-b2bc55f8b578
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                //bool showNullValue = false;
                //var database = projectContext.DatabaseInformation.First(x => x.ID == databaseID);
                //if(database.DatabaseUISetting!=null && database.DatabaseUISetting.ShowNullValue)


                var listSchema = new List<DBSchema>();
                foreach (var newEntity in listNew)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Creating" + " " + newEntity.Name, TotalProgressCount = listNew.Count, CurrentProgress = listNew.IndexOf(newEntity) + 1 });
                    UpdateEntityFromTargetDB(projectContext, databaseID, newEntity, listSchema);
                }
                foreach (var editEntity in listEdit)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Updating" + " " + editEntity.Name, TotalProgressCount = listEdit.Count, CurrentProgress = listEdit.IndexOf(editEntity) + 1 });
                    UpdateEntityFromTargetDB(projectContext, databaseID, editEntity, listSchema);
                }
                foreach (var deleteEntity in listDeleted)
                {
                    if (ItemImportingStarted != null)
                        ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Disabling" + " " + deleteEntity.Name, TotalProgressCount = listDeleted.Count, CurrentProgress = listDeleted.IndexOf(deleteEntity) + 1 });
                    var dbEntity = GetAllEnabledEntities(projectContext).FirstOrDefault(x => x.ID == deleteEntity.ID);
                    dbEntity.Removed = true;
                }
                if (ItemImportingStarted != null)
                    ItemImportingStarted(this, new ItemImportingStartedArg() { ItemName = "Saving changes..." });
                projectContext.SaveChanges();
            }
        }

        private void UpdateEntityFromTargetDB(MyIdeaEntities projectContext, int databaseID, TableDrivedEntityDTO entity, List<DBSchema> listAddedSchema)
        {
            // BizTableDrivedEntity.UpdateEntityFromTargetDB: 86b1c396-2004-4535-8de2-f4a43b5f5d62
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
                //tdEntity.IsReadonly = entity.IsView;
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

            BizColumn bizColumn = new BizColumn();
            bizColumn.UpdateColumnsFromTargetDB(entity, table, projectContext);

            //throw new Exception("asdasdasd");
        }



        private void RemoveColumnTypes(MyIdeaEntities projectContext, Column dbColumn, List<Enum_ColumnType> exceptionTypes)
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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


        public void UpdateFromUI(List<TableDrivedEntityDTO> entities)
        {
            //**03385f9c-7b5c-431f-b532-fd4a517eeb6e
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var entity in entities)
                {
                    var dbEntity = GetAllEntitiesBase(projectContext).First(x => x.ID == entity.ID);
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
                using (var projectContext = new DataAccess.MyIdeaEntities())
                {
                    var dbBaseEntity = GetAllEnabledEntities(projectContext).First(x => x.ID == message.BaseEntity.ID);
                    foreach (var item in message.BaseEntity.Relationships)
                    {
                        var dbRelationship = bizRelationship.GetAllEnabledRelationships(projectContext, false).First(x => x.ID == item.ID);
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
                            var dbDrivedEntity = GetAllEnabledEntities(projectContext).FirstOrDefault(x => x.InternalTableSuperToSubRelID == rel.ID);
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
                                dbDrived = GetAllEnabledEntities(projectContext).First(x => x.ID == drived.Item3.ID);
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
                                var dbRelationship = bizRelationship.GetAllEnabledRelationships(projectContext, false).First(x => x.ID == item.ID);
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

        private Tuple<Relationship, Relationship> AddISARelationship(MyIdeaEntities projectContext, ISARelationship isaRelationship
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

            BizColumn bizColumn = new BizColumn();
            var columns = bizColumn.GetAllEnabledColumns(dbBaseEntity);
            var pkColumns = columns.Where(x => x.PrimaryKey);
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

        private void RemoveDrivedEntity(MyIdeaEntities projectContext, TableDrivedEntity entity, ISARelationship isaRelationship)
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
        //public void ImposeEntityTableRule(int databaseID)
        //{
        //    //string catalogName = GeneralHelper.GetCatalogName(serverName, dbName);
        //    MyIdeaEntities context = new MyIdeaEntities();
        //    context.Configuration.LazyLoadingEnabled = true;
        //    var list = context.TableDrivedEntity.Where(x => x.Table.DBSchema.DatabaseInformationID == databaseID);
        //    var count = list.Count();
        //    int index = 0;
        //    foreach (var entity in list)
        //    {
        //        index++;
        //        Biz_RuleSet myruleSet = new Biz_RuleSet("TableDrivedEntity_IsReference");
        //        myruleSet.Execute(entity);
        //        //if (RuleImposedEvent != null)
        //        //    RuleImposedEvent(this, new SimpleGenerationInfoArg() { Title = entity.Name, CurrentProgress = index, TotalProgressCount = count });
        //    }
        //    //index = 0;
        //    //foreach (var entity in list)
        //    //{
        //    //    index++;
        //    //    View.SetInfo(count, index, entity.Name);
        //    //    Biz_RuleSet myruleSet = new Biz_RuleSet("TableDrivedEntity_IsAssociative");
        //    //    myruleSet.Execute(entity);
        //    //}
        //    //index = 0;
        //    //foreach (var entity in list)
        //    //{
        //    //    index++;
        //    //    View.SetInfo(count, index, entity.Name);
        //    //    Biz_RuleSet myruleSet = new Biz_RuleSet("TableDrivedEntity_IndependentDataEntry");
        //    //    myruleSet.Execute(entity);
        //    //}

        //    context.SaveChanges();
        //    //if (RuleImposedEvent != null)
        //    //    RuleImposedEvent(this, new SimpleGenerationInfoArg() { Title = "Operation is completed.", CurrentProgress = index, TotalProgressCount = count });
        //}


    }

}

