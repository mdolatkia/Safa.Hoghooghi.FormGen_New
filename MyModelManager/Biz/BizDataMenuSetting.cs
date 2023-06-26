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
        public List<DataMenuSettingDTO> GetDataMenuSettings(DR_Requester requester, int entityID, DetailsDepth detailsDepth)
        {
            List<DataMenuSettingDTO> result = new List<DataMenuSettingDTO>();

            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var settings = projectContext.DataMenuSetting.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in settings)
                {
                    result.Add(ToDataMenuSettingDTO(requester, item, detailsDepth));
                }
            }
            return result;
        }
        public DataMenuSettingDTO GetDataMenuSetting(DR_Requester requester, int ID, DetailsDepth detailsDepth)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = projectContext.DataMenuSetting.FirstOrDefault(x => x.ID == ID);
                if (entity != null)
                {
                    return ToDataMenuSettingDTO(requester, entity, detailsDepth);
                }
            }
            return null;
        }
        public DataMenuSettingDTO GetOrCreateDataMenuSetting(DR_Requester requester, int entityID, DetailsDepth detailsDepth)
        {

            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entity = bizTableDrivedEntity.GetAllEnabledEntities(projectContext).FirstOrDefault(x => x.ID == entityID);
                if (entity != null && entity.DataMenuSetting1 != null)
                {
                    return ToDataMenuSettingDTO(requester, entity.DataMenuSetting1, detailsDepth);
                }
                else
                {
                    DataMenuSettingDTO menySetting = new DataMenuSettingDTO();
                    menySetting.AllowArchive = true;
                    menySetting.AllowDataEntry = true;
                    menySetting.AllowLetter = true;
                    menySetting.AllowWorkflowReport = true;
                    return menySetting;
                }
            }
        }

        private DataMenuSettingDTO ToDataMenuSettingDTO(DR_Requester requester, DataMenuSetting entity
            , DetailsDepth detailsDepth)
        {
            var result = new DataMenuSettingDTO();

            result.EntityID = entity.TableDrivedEntityID;
            result.Name = entity.Name;


            //if (detailsDepth == DetailsDepth.WithDetailsAndObjects && !bizTableDrivedEntity.DataIsAccessable(requester, result.EntityID, new List<SecurityAction>() { SecurityAction.ArchiveView, SecurityAction.ArchiveEdit }))
            //    result.AllowDataEntry = false;
            //else
            result.AllowDataEntry = entity.AllowDataEntry == true;

            if (detailsDepth == DetailsDepth.WithDetailsAndObjects && !bizTableDrivedEntity.DataIsAccessable(requester, result.EntityID, new List<SecurityAction>() { SecurityAction.ArchiveView, SecurityAction.ArchiveEdit }))
                result.AllowArchive = false;
            else
                result.AllowArchive = entity.AllowArchive == true;




            if (detailsDepth == DetailsDepth.WithDetailsAndObjects && !bizTableDrivedEntity.DataIsAccessable(requester, result.EntityID, new List<SecurityAction>() { SecurityAction.LetterView, SecurityAction.LetterEdit }))
                result.AllowLetter = false;
            else
                result.AllowLetter = entity.AllowLetter == true;
            result.AllowWorkflowReport = entity.AllowWorkflowReport == true;
            if (detailsDepth != DetailsDepth.SimpleInfo)
            {
                foreach (var item in entity.DataMenuRelationshipTail)
                {
                    //دسترسی به خود گزارش هم کنترل شود
                    if (detailsDepth == DetailsDepth.WithDetailsAndObjects && !bizEntityRelationshipTail.DataIsAccessable(requester, item.EntityRelationshipTail))
                        continue;

                    //    if (bizEntityReport.DataIsAccessable(requester, item.EntitySearchableReport.EntityReport))
                    //    {
                    //  var tail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
                    // var entityReportDTO = new EntitySearchableReportDTO();
                    //   bizEntitySearchableReport.ToEntitySearchableReportDTO(requester, item.EntitySearchableReport, entityReportDTO, withDetails);\
                    var cItem = new DataMenuReportRelationshipTailDTO();
                    cItem.ID = item.ID;
                    cItem.DataMenuSettingID = item.DataMenuSettingID;
                    cItem.RelationshipTailID = item.EntityRelationshipTailID;
                    if (detailsDepth == DetailsDepth.WithDetailsAndObjects)
                    {
                        //بهتره برای Tail یه عنوان بزاریم
                        cItem.RelationshipTailTitle = item.EntityRelationshipTail.TableDrivedEntity1.Alias;
                    }
                    //if (detailsDepth == DetailsDepth.WithDetailsAndObjects)
                    //    cItem.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
                    foreach (var report in item.DataMenuRelTailSearchableReports)
                    {
                        if (detailsDepth == DetailsDepth.WithDetailsAndObjects && !bizEntityReport.DataIsAccessable(requester, report.EntitySearchableReport.EntityReport))
                            continue;

                        DataMenuRelTailSearchableReportsDTO reportItem = new DataMenuRelTailSearchableReportsDTO();
                        reportItem.ID = report.ID;
                        reportItem.Group1 = report.Group1;
                        reportItem.EntitySearchableReportID = report.EntitySearchableReportID;
                        reportItem.DataMenuRelationshipTailID = report.DataMenuRelationshipTailID;
                        if (detailsDepth == DetailsDepth.WithDetailsAndObjects)
                        {
                            reportItem.ReportTitle = report.EntitySearchableReport.EntityReport.Title;
                        }
                        cItem.SearchableReports.Add(reportItem);
                    }
                    result.ReportRelationshipTails.Add(cItem);
                    //    }
                    //}
                }

                foreach (var item in entity.DataMenuDataItemReport)
                {
                    //if (bizEntityReport.DataIsAccessable(requester, item.EntityDataItemReportID))
                    //{
                    // var entityReportDTO = new EntityDataItemReportDTO();
                    // bizEntityDataItemReport.ToEntityDataItemReportDTO(requester, item.EntityDataItemReport, entityReportDTO, false);
                    if (detailsDepth == DetailsDepth.WithDetailsAndObjects && !bizEntityReport.DataIsAccessable(requester, item.EntityDataItemReport.EntityReport))
                        continue;
                    var rel = new DataMenuDataItemReportDTO();
                    rel.EntityDataItemReportID = item.EntityDataItemReportID;
                    //   rel.EntityDataItemReport = entityReport;
                    rel.Group1 = item.Group1;
                    if (detailsDepth == DetailsDepth.WithDetailsAndObjects)
                    {
                        rel.ReportTitle = item.EntityDataItemReport.EntityReport.Title;
                    }
                    result.DataItemReports.Add(rel);
                    // }
                }
                if (entity.DataMenuForViewEntity.Any())
                {
                    var item = entity.DataMenuForViewEntity.First();
                    if (bizRelationship.DataIsAccessable(requester, item.Relationship, false, true, false))
                    {
                        result.RelationshipID = item.RelationshipID;
                        //if (withDetails)
                        //{


                        ///  result.Relationship = bizRelationship.ToRelationshipDTO(entity.DataMenuForViewEntity.First().Relationship);
                        //      }
                        result.TargetDataMenuSettingID = item.TargetDataMenuSettingID;
                    }
                    //   result.DataMenuSetting = ToDataMenuSettingDTO(requester, entity.DataMenuForViewEntity.First().DataMenuSetting1, false);
                }
                result.IconContent = entity.IconContent;
            }
            result.ID = entity.ID;


            return result;
        }



        //private DataMenuGridViewRelationshipDTO ToDataMenuGridViewRelationshipDTO(DataMenuGridViewRelationship dbRel, EntityRelationshipTailDTO tail)
        //{
        //    var rel = new DataMenuGridViewRelationshipDTO();
        //    rel.ID = dbRel.ID;
        //    rel.RelationshipTailID = dbRel.EntityRelationshipTailID;
        //    rel.RelationshipTail = tail;
        //    rel.Group1 = dbRel.Group1;
        //    rel.TargetDataMenuSettingID = dbRel.TargetDataMenuSettingID ?? 0;
        //    return rel;
        //}

        //private DataMenuDataViewRelationshipDTO ToDataMenuDataViewRelationshipDTO(DataMenuDataViewRelationship dbRel, EntityRelationshipTailDTO tail)
        //{
        //    var rel = new DataMenuDataViewRelationshipDTO();
        //    rel.ID = dbRel.ID;
        //    rel.RelationshipTailID = dbRel.EntityRelationshipTailID;
        //    rel.RelationshipTail = tail;
        //    rel.Group1 = dbRel.Group1;
        //    rel.TargetDataMenuSettingID = dbRel.TargetDataMenuSettingID ?? 0;
        //    return rel;
        //}

        //private DataMenuSearchableReportRelationshipDTO ToDataMenuSearchableReportRelationshipDTO(DataMenuSearchableReportRelationship dbRel, EntityRelationshipTailDTO tail, EntitySearchableReportDTO entityReport)
        //{
        //    var result = new DataMenuSearchableReportRelationshipDTO();
        //    result.ID = dbRel.ID;
        //    result.RelationshipTailID = dbRel.EntityRelationshipTailID;
        //    result.RelationshipTail = tail;
        //    BizEntityReport bizEntityReport = new BizEntityReport();
        //    result.EntitySearchableReportID = dbRel.EntitySearchableReportID;
        //    result.SearchableReportReport = entityReport;
        //    result.Group1 = dbRel.Group1;

        //    return result;
        //}

        public DataMenuResult GetDataMenu(DR_Requester requester, DP_DataView dataItem, int dataMenuSettingID)
        {
            // BizDataMenuSetting.GetDataMenu: dad2f7cbb995
            DataMenuResult result = new DataMenuResult();
            List<DataMenuDTO> resultMenus = new List<DataMenuDTO>();
            result.DataMenus = resultMenus;
            var entityID = dataItem.TargetEntityID;
            var simpleEntity = bizTableDrivedEntity.GetSimpleEntity(requester, entityID);
            if (simpleEntity == null)
                return null;
            DataMenuSettingDTO dataMenuSetting = null;
            if (dataMenuSettingID != 0)
                dataMenuSetting = GetDataMenuSetting(requester, dataMenuSettingID, DetailsDepth.WithDetailsAndObjects);
            else
                dataMenuSetting = GetOrCreateDataMenuSetting(requester, entityID, DetailsDepth.WithDetailsAndObjects);

            if (dataMenuSetting != null)
                result.DataMenuSettingName = dataMenuSetting.Name;

            //** BizDataMenuSetting.GetDataMenu: 1677d2a6-f3cf-43f9-b61b-f6bf4c34c203
            if (simpleEntity.IsView)
            {
                if (dataMenuSetting != null)
                {
                    if (dataMenuSetting.RelationshipID != 0 && dataMenuSetting.TargetDataMenuSettingID != 0)
                    {
                        var relationship = bizRelationship.GetRelationship(dataMenuSetting.RelationshipID);
                        var dataView = CreateDataView(requester, dataItem, relationship);
                        var newresult = GetDataMenu(requester, dataView, dataMenuSetting.TargetDataMenuSettingID);
                        newresult.NewData = dataView;
                        return newresult;
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
                if (dataMenuSetting.AllowDataEntry)
                    AddMenu(resultMenus, "نمایش/اصلاح داده", "", DataMenuType.Form);
                //آرشیو داده
                // BizArchive bizArchive = new BizArchive();
                if (dataMenuSetting.AllowArchive)
                {

                    var archiveRootMenu = AddMenu(resultMenus, "آرشیو", "", DataMenuType.Archive);
                }

                //نامه های داده
                //  BizLetterTemplate bizLetterTemplate = new BizLetterTemplate();
                if (dataMenuSetting.AllowLetter)
                {
                    var letterRootMenu = AddMenu(resultMenus, "نامه ها", "", DataMenuType.Letter);
                }
                if (dataMenuSetting.AllowWorkflowReport)
                {
                    //جریان کارهای مرتبط
                    if (bizProcess.EntityHasAnyProcess(entityID))
                    {
                        var workflowRootMenu = AddMenu(resultMenus, "جریان کار", "", DataMenuType.Workflow);
                    }
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
                if (dataMenuSetting.ReportRelationshipTails.Any())
                {

                    //دسترسی به خود گزارش هم کنترل شود
                    //if (bizEntityRelationshipTail.DataIsAccessable(requester, item.EntityRelationshipTail))
                    //{
                    //    if (bizEntityReport.DataIsAccessable(requester, item.EntitySearchableReport.EntityReport))
                    //    {
                    List<DataMenuDTO> source = null;
                    if (dataMenuSetting.ReportRelationshipTails.Count > 3)
                    {
                        var relationshipReportRootMenu = AddMenu(resultMenus, "گزارش داده های مرتبط", "", DataMenuType.Folder);
                        source = relationshipReportRootMenu.SubMenus;
                    }
                    else
                        source = resultMenus;
                    foreach (var tail in dataMenuSetting.ReportRelationshipTails)
                    {
                        var tailMenu = AddMenu(source, tail.RelationshipTailTitle, "", DataMenuType.Folder);
                        foreach (var group in tail.SearchableReports.GroupBy(x => x.Group1 ?? ""))
                        {
                            List<DataMenuDTO> groupSource = null;
                            if (!string.IsNullOrEmpty(group.Key))
                                groupSource = source;
                            else
                                groupSource = GetGroupMenu(source, group.Key).SubMenus;
                            foreach (var rel in group)
                            {
                                var menu = AddMenu(groupSource, rel.ReportTitle, "", DataMenuType.RelationshipTailSearchableReport);
                                menu.ReportID = rel.EntitySearchableReportID;
                                menu.RelationshipTailID = tail.RelationshipTailID;
                                //  menu.SearchableReportRelationshipTail = rel;
                            }
                        }
                    }
                }


                if (dataMenuSetting.DataItemReports.Any())
                {
                    List<DataMenuDTO> source = null;
                    if (dataMenuSetting.ReportRelationshipTails.Count > 3)
                    {
                        var gridViewRootMenu = AddMenu(resultMenus, "گزارشات مورد داده", "", DataMenuType.Folder);
                        source = gridViewRootMenu.SubMenus;
                    }
                    else
                        source = resultMenus;
                    foreach (var group in dataMenuSetting.DataItemReports.GroupBy(x => x.Group1 ?? ""))
                    {
                        List<DataMenuDTO> groupSource = null;
                        if (!string.IsNullOrEmpty(group.Key))
                            groupSource = resultMenus;
                        else
                            groupSource = GetGroupMenu(source, group.Key).SubMenus;

                        foreach (var rel in group)
                        {
                            var menu = AddMenu(groupSource, rel.ReportTitle, "", DataMenuType.DataItemReport);
                            menu.ReportID = rel.EntityDataItemReportID;
                            // gridViewRelMenu.DataItemReport = rel.EntityDataItemReport;
                        }
                    }

                }
            }
            return result;
        }

        //public List<DataMenuSettingDTO> GetDataMenusOfRelationshipTail(DR_Requester requester, int relationshipTailID)
        //{
        //    List<EntitySearchableReportDTO> result = new List<EntitySearchableReportDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        //        var relationshipTail = bizEntityRelationshipTail.GetEntityRelationshipTail(requester, relationshipTailID);
        //        return GetDataMenuSettings(requester, relationshipTail.TargetEntityID);
        //    }

        //}
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

        private DataMenuDTO GetGroupMenu(List<DataMenuDTO> menus, string key)
        {
            if (menus.Any(x => x.Title == key))
                return menus.First(x => x.Title == key);
            else
            {
                var subMenu = AddMenu(menus, key, "", DataMenuType.Folder);
                return subMenu;
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
                var dbEntity = bizTableDrivedEntity.GetAllEnabledEntities(projectContext).FirstOrDefault(x => x.ID == entityID);
                dbEntity.DataMenuSettingID = iD;
                projectContext.SaveChanges();
            }
            return true;
        }

        private DataMenuDTO AddMenu(List<DataMenuDTO> result, string title, string tooltip, DataMenuType type)
        {
            var dataMenu = new DataMenuDTO();
            dataMenu.Title = title;
            dataMenu.Tooltip = tooltip;
            dataMenu.Type = type;
            
            //dataMenu.DataItem = dataItem;
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
            // BizDataMenuSetting.UpdateEntityReportDataMenuSettings: a0894cb4830e
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
                dbEntity.AllowArchive = message.AllowArchive;
                dbEntity.AllowDataEntry = message.AllowDataEntry;
                dbEntity.AllowLetter = message.AllowLetter;
                dbEntity.AllowWorkflowReport = message.AllowWorkflowReport;

                while (dbEntity.DataMenuRelationshipTail.Any())
                {
                    var item = dbEntity.DataMenuRelationshipTail.First();
                    while (item.DataMenuRelTailSearchableReports.Any())
                        projectContext.DataMenuRelTailSearchableReports.Remove(item.DataMenuRelTailSearchableReports.First());
                    projectContext.DataMenuRelationshipTail.Remove(item);
                }
                foreach (var item in message.ReportRelationshipTails)
                {
                    DataMenuRelationshipTail dbRel = new DataMenuRelationshipTail();
                    //  dbRel.RelationshipID = (item.RelationshipID == 0 ? (int?)null : item.RelationshipID);
                    dbRel.EntityRelationshipTailID = item.RelationshipTailID;
                    //   dbRel.EntitySearchableReportID = item.EntitySearchableReportID;
                    //   dbRel.Group1 = item.Group1;
                    foreach (var citem in item.SearchableReports)
                    {
                        var sREport = new DataMenuRelTailSearchableReports();
                        sREport.EntitySearchableReportID = citem.EntitySearchableReportID;
                        sREport.Group1 = citem.Group1;
                        dbRel.DataMenuRelTailSearchableReports.Add(sREport);
                    }
                    dbEntity.DataMenuRelationshipTail.Add(dbRel);
                }

                //while (dbEntity.DataMenuDataViewRelationship.Any())
                //    projectContext.DataMenuDataViewRelationship.Remove(dbEntity.DataMenuDataViewRelationship.First());
                //foreach (var item in message.DataViewRelationships)
                //{
                //    DataMenuDataViewRelationship dbRel = new DataMenuDataViewRelationship();
                //    dbRel.EntityRelationshipTailID = item.RelationshipTailID;
                //    dbRel.Group1 = item.Group1;
                //    if (item.TargetDataMenuSettingID != 0)
                //        dbRel.TargetDataMenuSettingID = item.TargetDataMenuSettingID;
                //    else
                //        dbRel.TargetDataMenuSettingID = null;
                //    dbEntity.DataMenuDataViewRelationship.Add(dbRel);

                //}

                //while (dbEntity.DataMenuGridViewRelationship.Any())
                //    projectContext.DataMenuGridViewRelationship.Remove(dbEntity.DataMenuGridViewRelationship.First());
                //foreach (var item in message.GridViewRelationships)
                //{
                //    DataMenuGridViewRelationship dbRel = new DataMenuGridViewRelationship();
                //    dbRel.EntityRelationshipTailID = item.RelationshipTailID;
                //    dbRel.Group1 = item.Group1;
                //    if (item.TargetDataMenuSettingID != 0)
                //        dbRel.TargetDataMenuSettingID = item.TargetDataMenuSettingID;
                //    else
                //        dbRel.TargetDataMenuSettingID = null;
                //    dbEntity.DataMenuGridViewRelationship.Add(dbRel);
                //}

                while (dbEntity.DataMenuDataItemReport.Any())
                    projectContext.DataMenuDataItemReport.Remove(dbEntity.DataMenuDataItemReport.First());
                foreach (var item in message.DataItemReports)
                {
                    DataMenuDataItemReport dbRel = new DataMenuDataItemReport();
                    dbRel.EntityDataItemReportID = item.EntityDataItemReportID;
                    dbRel.Group1 = item.Group1;
                    dbEntity.DataMenuDataItemReport.Add(dbRel);
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

                dbEntity.IconContent = message.IconContent;

                //dbEntity.EntityReportDataMenuSetting.IconContent = message.IconContent;
                projectContext.SaveChanges();
            }
        }
    }

}
