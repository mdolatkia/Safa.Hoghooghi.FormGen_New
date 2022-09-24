using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLibrary;


namespace MyModelManager
{
    public class BizDataMenuSetting
    {
        BizProcess bizProcess = new BizProcess();
        BizEntityDataItemReport bizEntityDataItemReport = new BizEntityDataItemReport();
        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        BizEntityReport bizEntityReport = new BizEntityReport();
        SecurityHelper securityHelper = new SecurityHelper();
        BizEntitySearchableReport bizEntitySearchableReport = new BizEntitySearchableReport();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        BizRelationship bizRelationship = new BizRelationship();
        public List<DataMenuSettingDTO> GetDataMenuSettings(DR_Requester requester, int entityID)
        {
            List<DataMenuSettingDTO> result = new List<DataMenuSettingDTO>();

            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var settings = projectContext.DataMenuSetting.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in settings)
                {
                    result.Add(ToDataMenuSettingDTO(requester, item, false));
                }
            }
            return result;
        }
        public DataMenuSettingDTO GetDataMenuSetting(DR_Requester requester, int ID, bool withDetails)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = projectContext.DataMenuSetting.FirstOrDefault(x => x.ID == ID);
                if (entity != null)
                {
                    return ToDataMenuSettingDTO(requester, entity, withDetails);
                }
            }
            return null;
        }
        public DataMenuSettingDTO GetDefaultDataMenuSetting(DR_Requester requester, int entityID, bool withDetails)
        {

            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = bizTableDrivedEntity.GetAllEntities(projectContext, false).FirstOrDefault(x => x.ID == entityID);
                if (entity != null && entity.DataMenuSetting1 != null)
                {
                    return ToDataMenuSettingDTO(requester, entity.DataMenuSetting1, withDetails);
                }
                else
                    return null;
            }
        }
        private DataMenuSettingDTO ToDataMenuSettingDTO(DR_Requester requester, DataMenuSetting entity, bool withDetails)
        {
            var result = new DataMenuSettingDTO();
            if (withDetails)
            {
                foreach (var item in entity.DataMenuSearchableReportRelationship)
                {
                    //دسترسی به خود گزارش هم کنترل شود
                    if (bizEntityRelationshipTail.DataIsAccessable(requester, item.EntityRelationshipTail))
                    {
                        if (bizEntityReport.DataIsAccessable(requester, item.EntitySearchableReport.EntityReport))
                        {
                            var tail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
                            var entityReportDTO = new EntitySearchableReportDTO();
                            bizEntitySearchableReport.ToEntitySearchableReportDTO(item.EntitySearchableReport, entityReportDTO,  withDetails);
                            result.SearchableReportRelationships.Add(ToDataMenuSearchableReportRelationshipDTO(item, tail, entityReportDTO));
                        }
                    }
                }
                foreach (var item in entity.DataMenuDataViewRelationship)
                {
                    if (bizEntityRelationshipTail.DataIsAccessable(requester, item.EntityRelationshipTail))
                    {
                        var tail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
                        result.DataViewRelationships.Add(ToDataMenuDataViewRelationshipDTO(item, tail));
                    }
                }
                foreach (var item in entity.DataMenuGridViewRelationship)
                {
                    if (bizEntityRelationshipTail.DataIsAccessable(requester, item.EntityRelationshipTail))
                    {
                        var tail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
                        result.GridViewRelationships.Add(ToDataMenuGridViewRelationshipDTO(item, tail));
                    }
                }
                foreach (var item in entity.DataMenuDataItemReportRelationship)
                {
                    if (bizEntityReport.DataIsAccessable(requester, item.EntityDataItemReportID))
                    {
                        var entityReportDTO = new EntityDataItemReportDTO();
                        bizEntityDataItemReport.ToEntityDataItemReportDTO(item.EntityDataItemReport, entityReportDTO,false);
                        result.DataItemReports.Add(ToDataMenuDataItemReportRelationshipDTO(item, entityReportDTO));
                    }
                }
            }
            result.EntityID = entity.TableDrivedEntityID;
            result.EntityListViewID = entity.EntityListViewID;
            result.Name = entity.Name;
            if (entity.DataMenuForViewEntity.Any())
            {
                result.RelationshipID = entity.DataMenuForViewEntity.First().RelationshipID;
                if (withDetails)
                {
                    if (bizRelationship.DataIsAccessable(requester, entity.DataMenuForViewEntity.First().Relationship, false, true))
                        result.Relationship = bizRelationship.ToRelationshipDTO(entity.DataMenuForViewEntity.First().Relationship);
                }
                result.TargetDataMenuSettingID = entity.DataMenuForViewEntity.First().TargetDataMenuSettingID;
                if (withDetails)
                    result.DataMenuSetting = ToDataMenuSettingDTO(requester, entity.DataMenuForViewEntity.First().DataMenuSetting1, false);
            }
            result.ID = entity.ID;
            if (withDetails)
                result.IconContent = entity.IconContent;
            return result;
        }

        private DataMenuDataItemReportDTO ToDataMenuDataItemReportRelationshipDTO(DataMenuDataItemReportRelationship dbRel, EntityDataItemReportDTO entityReport)
        {
            var rel = new DataMenuDataItemReportDTO();
            rel.EntityDataItemReportID = dbRel.EntityDataItemReportID;
            rel.EntityDataItemReport = entityReport;
            rel.Group1 = dbRel.Group1;

            return rel;
        }

        private DataMenuGridViewRelationshipDTO ToDataMenuGridViewRelationshipDTO(DataMenuGridViewRelationship dbRel, EntityRelationshipTailDTO tail)
        {
            var rel = new DataMenuGridViewRelationshipDTO();
            rel.ID = dbRel.ID;
            rel.RelationshipTailID = dbRel.EntityRelationshipTailID;
            rel.RelationshipTail = tail;
            rel.Group1 = dbRel.Group1;
            rel.TargetDataMenuSettingID = dbRel.TargetDataMenuSettingID ?? 0;
            return rel;
        }

        private DataMenuDataViewRelationshipDTO ToDataMenuDataViewRelationshipDTO(DataMenuDataViewRelationship dbRel, EntityRelationshipTailDTO tail)
        {
            var rel = new DataMenuDataViewRelationshipDTO();
            rel.ID = dbRel.ID;
            rel.RelationshipTailID = dbRel.EntityRelationshipTailID;
            rel.RelationshipTail = tail;
            rel.Group1 = dbRel.Group1;
            rel.TargetDataMenuSettingID = dbRel.TargetDataMenuSettingID ?? 0;
            return rel;
        }

        private DataMenuSearchableReportRelationshipDTO ToDataMenuSearchableReportRelationshipDTO(DataMenuSearchableReportRelationship dbRel, EntityRelationshipTailDTO tail, EntitySearchableReportDTO entityReport)
        {
            var result = new DataMenuSearchableReportRelationshipDTO();
            result.ID = dbRel.ID;
            result.RelationshipTailID = dbRel.EntityRelationshipTailID;
            result.RelationshipTail = tail;
            BizEntityReport bizEntityReport = new BizEntityReport();
            result.EntitySearchableReportID = dbRel.EntitySearchableReportID;
            result.SearchableReportReport = entityReport;
            result.Group1 = dbRel.Group1;
            return result;
        }

        public DataMenuResult GetDataMenu(DR_Requester requester, DP_DataView dataItem, int dataMenuSettingID)
        {
            DataMenuResult result = new DataMenuResult();
            List<DataMenu> resultMenus = new List<DataMenu>();
            result.DataMenus = resultMenus;
            var entityID = dataItem.TargetEntityID;
            var simpleEntity = bizTableDrivedEntity.GetSimpleEntity(requester, entityID);
            if (simpleEntity == null)
                return null;
            DataMenuSettingDTO dataMenuSetting = null;
            if (dataMenuSettingID != 0)
                dataMenuSetting = GetDataMenuSetting(requester, dataMenuSettingID, true);
            else
                dataMenuSetting = GetDefaultDataMenuSetting(requester, entityID, true);

            if (dataMenuSetting != null)
                result.DataMenuSettingName = dataMenuSetting.Name;

            //** 1677d2a6-f3cf-43f9-b61b-f6bf4c34c203
            if (simpleEntity.IsView)
            {
                if (dataMenuSetting != null)
                {
                    if (dataMenuSetting.RelationshipID != 0 && dataMenuSetting.TargetDataMenuSettingID != 0)
                    {
                        var relationship = bizRelationship.GetRelationship(dataMenuSetting.RelationshipID);
                        var dataView = CreateDataView(requester,dataItem, relationship);
                        return GetDataMenu(requester, dataView, dataMenuSetting.TargetDataMenuSettingID);
                    }
                }
                //var fullEntity = bizTableDrivedEntity.GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
                //if (fullEntity.Relationships.Any())
                //{
                //    foreach (var relationsip in fullEntity.Relationships)
                //    {
                //        var dataView = CreateDataView(dataItem, relationsip);
                //        var menu = AddMenu(result, relationsip.Alias, "", DataMenuType.ViewRel);
                //        menu.ViewRelTargetDataItem = dataView;
                //    }
                //}
            }
            else
            {
                var dataEntryRootMenu = AddMenu(resultMenus, "نمایش/اصلاح داده", "", DataMenuType.Form, dataItem);
                //آرشیو داده
                // BizArchive bizArchive = new BizArchive();
                if (bizTableDrivedEntity.DataIsAccessable(requester, entityID, new List<SecurityAction>() { SecurityAction.ArchiveView, SecurityAction.ArchiveEdit }))
                {
                    var archiveRootMenu = AddMenu(resultMenus, "آرشیو", "", DataMenuType.Archive, dataItem);
                }


                //نامه های داده
                //  BizLetterTemplate bizLetterTemplate = new BizLetterTemplate();
                if (bizTableDrivedEntity.DataIsAccessable(requester, entityID, new List<SecurityAction>() { SecurityAction.LetterView, SecurityAction.LetterEdit }))
                {
                    var letterRootMenu = AddMenu(resultMenus, "نامه ها", "", DataMenuType.Letter, dataItem);
                }
                //جریان کارهای مرتبط
                if (bizProcess.EntityHasAnyProcess(entityID))
                {
                    var workflowRootMenu = AddMenu(resultMenus, "جریان کار", "", DataMenuType.Workflow, dataItem);
                }




                ////لینک های داده
                //BizDataLink bizDataLink = new MyModelManager.BizDataLink();
                //var datalinks = bizDataLink.GetDataLinkByEntitiyID(requester, entityID);
                //if (datalinks.Any())
                //{
                //    var dataViewRootMenu = AddMenu(resultMenus, "لینک داده", "", DataMenuType.Folder,null);
                //    foreach (var datalink in datalinks)
                //    {
                //        var datalinkMenu = AddMenu(dataViewRootMenu.SubMenus, datalink.ReportTitle, "", DataMenuType.DataLink, dataItem);
                //        datalinkMenu.Datalink = datalink;
                //    }
                //}

                //گزارشهای داده های مرتبط

                if (dataMenuSetting != null)
                {
                    if (dataMenuSetting.SearchableReportRelationships.Any())
                    {
                        var relationshipReportRootMenu = AddMenu(resultMenus, "گزارش داده های مرتبط", "", DataMenuType.Folder,null);
                        foreach (var group in dataMenuSetting.SearchableReportRelationships.GroupBy(x => x.Group1 ?? ""))
                        {
                            DataMenu parentGroupMenu = GetGroupMenu(relationshipReportRootMenu, group.Key);
                            foreach (var rel in group)
                            {
                                var menu = AddMenu(parentGroupMenu.SubMenus, rel.SearchableReportReport.ReportTitle, "", DataMenuType.RelationshipTailSearchableReport, dataItem);
                                menu.SearchableReportRelationshipTail = rel;
                            }
                        }
                    }
                    //نمای داده های مرتبط
                    if (dataMenuSetting.DataViewRelationships.Any())
                    {
                        var dataViewRootMenu = AddMenu(resultMenus, "نمایش داده های مرتبط", "", DataMenuType.Folder,null);
                        foreach (var group in dataMenuSetting.DataViewRelationships.GroupBy(x => x.Group1 ?? ""))
                        {
                            DataMenu parentGroupMenu = GetGroupMenu(dataViewRootMenu, group.Key);
                            foreach (var rel in group)
                            {
                                var dataViewRelMenu = AddMenu(parentGroupMenu.SubMenus, rel.RelationshipTail.TargetEntityAlias, rel.RelationshipTail.EntityPath, DataMenuType.RelationshipTailDataView, dataItem);
                                dataViewRelMenu.DataviewRelationshipTail = rel.RelationshipTail;
                                dataViewRelMenu.TargetDataMenuSettingID = rel.TargetDataMenuSettingID;
                            }
                        }
                    }
                    if (dataMenuSetting.GridViewRelationships.Any())
                    {
                        var gridViewRootMenu = AddMenu(resultMenus, "گرید داده های مرتبط", "", DataMenuType.Folder,null);
                        foreach (var group in dataMenuSetting.GridViewRelationships.GroupBy(x => x.Group1 ?? ""))
                        {
                            DataMenu parentGroupMenu = GetGroupMenu(gridViewRootMenu, group.Key);
                            foreach (var rel in group)
                            {
                                var gridViewRelMenu = AddMenu(parentGroupMenu.SubMenus, rel.RelationshipTail.TargetEntityAlias, rel.RelationshipTail.EntityPath, DataMenuType.RelationshipTailDataGrid, dataItem);
                                gridViewRelMenu.GridviewRelationshipTail = rel.RelationshipTail;
                                gridViewRelMenu.TargetDataMenuSettingID = rel.TargetDataMenuSettingID;
                            }
                        }
                    }
                    if (dataMenuSetting.DataItemReports.Any())
                    {
                        var gridViewRootMenu = AddMenu(resultMenus, "گزارشات مورد داده", "", DataMenuType.Folder,null);
                        foreach (var group in dataMenuSetting.DataItemReports.GroupBy(x => x.Group1 ?? ""))
                        {
                            DataMenu parentGroupMenu = GetGroupMenu(gridViewRootMenu, group.Key);
                            foreach (var rel in group)
                            {
                                var gridViewRelMenu = AddMenu(parentGroupMenu.SubMenus, rel.EntityDataItemReport.ReportTitle, "", DataMenuType.DataItemReport, dataItem);
                                gridViewRelMenu.DataItemReport = rel.EntityDataItemReport;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<DataMenuSettingDTO> GetDataMenusOfRelationshipTail(DR_Requester requester, int relationshipTailID)
        {
            List<EntitySearchableReportDTO> result = new List<EntitySearchableReportDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
                var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(requester, relationshipTailID);
                return GetDataMenuSettings(requester, relationshipTail.TargetEntityID);
            }

        }
        //public List<DataMenuSettingDTO> GetDataMenusOfRelationship(DR_Requester requester, int relationshipID)
        //{
        //    List<EntitySearchableReportDTO> result = new List<EntitySearchableReportDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        BizRelationship bizRelationship = new BizRelationship();
        //        var relationship = bizRelationship.GetRelationship(requester, relationshipTailID);
        //        return GetDataMenuSettings(requester, relationshipTail.TargetEntityID);
        //    }

        //}

        private DataMenu GetGroupMenu(DataMenu parentMenu, string key)
        {
            if (string.IsNullOrEmpty(key))
                return parentMenu;
            else
            {
                if (parentMenu.SubMenus.Any(x => x.Title == key))
                    return parentMenu.SubMenus.First(x => x.Title == key);
                else
                {
                    var subMenu = AddMenu(parentMenu.SubMenus, key, "", DataMenuType.Folder,null);
                    return subMenu;
                }
            }
        }
        DP_DataView CreateDataView(DR_Requester requester, DP_DataView sourceDataItem, RelationshipDTO relationsip)
        {
            var entity = bizTableDrivedEntity.GetTableDrivedEntity(requester, relationsip.EntityID2, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            var dataView = new DP_DataView(relationsip.EntityID2, relationsip.Entity2);
            foreach (var relCol in relationsip.RelationshipColumns)
            {
                var property = sourceDataItem.GetProperty(relCol.FirstSideColumnID);
                if (property == null)
                    return null;
                dataView.Properties.Add(new EntityInstanceProperty(relCol.SecondSideColumn) { Value = sourceDataItem.GetProperty(relCol.FirstSideColumnID).Value });
            }
            return dataView;
        }

        public bool SetDefaultDataMenuSetting(int entityID, int iD)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbEntity = bizTableDrivedEntity.GetAllEntities(projectContext, false).FirstOrDefault(x => x.ID == entityID);
                dbEntity.DataMenuSettingID = iD;
                projectContext.SaveChanges();
            }
            return true;
        }

        private DataMenu AddMenu(List<DataMenu> result, string title, string tooltip, DataMenuType type,DP_DataView dataItem)
        {
            var dataMenu = new DataMenu();
            dataMenu.Title = title;
            dataMenu.Tooltip = tooltip;
            dataMenu.Type = type;
            dataMenu.DataItem = dataItem;
            result.Add(dataMenu);
            return dataMenu;
        }

        //public List<EntityReportDataMenuRelationshipsDTO> GetEntityRelationshipReports(int entityID, bool withDetails)
        //{
        //    List<EntityReportDataMenuRelationshipsDTO> result = new List<EntityReportDataMenuRelationshipsDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entityReportSetting = projectContext.EntityReportDataMenuSetting.FirstOrDefault(x => x.ID == entityID);
        //        if (entityReportSetting != null)
        //        {
        //            foreach (var item in entityReportSetting.EntityReportDataMenuRelationships)
        //            {
        //                result.Add(ToEntityReportDataMenuRelationshipsDTO(item, withDetails));
        //            }
        //        }
        //    }
        //    return result;
        //}

        //private EntityReportDataMenuRelationshipsDTO ToEntityReportDataMenuRelationshipsDTO(EntityReportDataMenuRelationships dbRel, bool withDetails)
        //{

        //}



        public void UpdateEntityReportDataMenuSettings(DataMenuSettingDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                DataMenuSetting dbEntity = null;
                if (message.ID != 0)
                    dbEntity = projectContext.DataMenuSetting.First(x => x.ID == message.ID);
                else
                {
                    dbEntity = new DataMenuSetting();
                    projectContext.DataMenuSetting.Add(dbEntity);
                }

                while (dbEntity.DataMenuSearchableReportRelationship.Any())
                    projectContext.DataMenuSearchableReportRelationship.Remove(dbEntity.DataMenuSearchableReportRelationship.First());
                foreach (var item in message.SearchableReportRelationships)
                {
                    DataMenuSearchableReportRelationship dbRel = new DataMenuSearchableReportRelationship();
                    //  dbRel.RelationshipID = (item.RelationshipID == 0 ? (int?)null : item.RelationshipID);
                    dbRel.EntityRelationshipTailID = item.RelationshipTailID;
                    dbRel.EntitySearchableReportID = item.EntitySearchableReportID;
                    dbRel.Group1 = item.Group1;
                    dbEntity.DataMenuSearchableReportRelationship.Add(dbRel);
                }

                while (dbEntity.DataMenuDataViewRelationship.Any())
                    projectContext.DataMenuDataViewRelationship.Remove(dbEntity.DataMenuDataViewRelationship.First());
                foreach (var item in message.DataViewRelationships)
                {
                    DataMenuDataViewRelationship dbRel = new DataMenuDataViewRelationship();
                    dbRel.EntityRelationshipTailID = item.RelationshipTailID;
                    dbRel.Group1 = item.Group1;
                    if (item.TargetDataMenuSettingID != 0)
                        dbRel.TargetDataMenuSettingID = item.TargetDataMenuSettingID;
                    else
                        dbRel.TargetDataMenuSettingID = null;
                    dbEntity.DataMenuDataViewRelationship.Add(dbRel);

                }

                while (dbEntity.DataMenuGridViewRelationship.Any())
                    projectContext.DataMenuGridViewRelationship.Remove(dbEntity.DataMenuGridViewRelationship.First());
                foreach (var item in message.GridViewRelationships)
                {
                    DataMenuGridViewRelationship dbRel = new DataMenuGridViewRelationship();
                    dbRel.EntityRelationshipTailID = item.RelationshipTailID;
                    dbRel.Group1 = item.Group1;
                    if (item.TargetDataMenuSettingID != 0)
                        dbRel.TargetDataMenuSettingID = item.TargetDataMenuSettingID;
                    else
                        dbRel.TargetDataMenuSettingID = null;
                    dbEntity.DataMenuGridViewRelationship.Add(dbRel);
                }

                while (dbEntity.DataMenuDataItemReportRelationship.Any())
                    projectContext.DataMenuDataItemReportRelationship.Remove(dbEntity.DataMenuDataItemReportRelationship.First());
                foreach (var item in message.DataItemReports)
                {
                    DataMenuDataItemReportRelationship dbRel = new DataMenuDataItemReportRelationship();
                    dbRel.EntityDataItemReportID = item.EntityDataItemReportID;
                    dbRel.Group1 = item.Group1;
                    dbEntity.DataMenuDataItemReportRelationship.Add(dbRel);
                }


                while (dbEntity.DataMenuForViewEntity.Any())
                    projectContext.DataMenuForViewEntity.Remove(dbEntity.DataMenuForViewEntity.First());
                if (message.RelationshipID != 0 && message.TargetDataMenuSettingID != 0)
                {
                    DataMenuForViewEntity dbRel = new DataMenuForViewEntity();
                    dbRel.RelationshipID = message.RelationshipID;
                    dbRel.TargetDataMenuSettingID = message.TargetDataMenuSettingID;
                    dbEntity.DataMenuForViewEntity.Add(dbRel);
                }




                dbEntity.TableDrivedEntityID = message.EntityID;
                dbEntity.Name = message.Name;
                dbEntity.EntityListViewID = message.EntityListViewID;
                dbEntity.IconContent = message.IconContent;

                //dbEntity.EntityReportDataMenuSetting.IconContent = message.IconContent;
                projectContext.SaveChanges();
            }
        }
    }

}
