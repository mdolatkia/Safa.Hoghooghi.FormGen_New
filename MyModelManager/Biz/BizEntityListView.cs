using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizEntityListView
    {
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();

        SecurityHelper securityHelper = new SecurityHelper();
        public event EventHandler<ItemImportingStartedArg> ItemImportingStarted;
        BizEntityRelationshipTail bizEntityRelationshipTail = new MyModelManager.BizEntityRelationshipTail();
        public List<EntityListViewDTO> GetEntityListViews(DR_Requester requester, int entityID)
        {

            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Validation, entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntityListViewDTO>);

            List<EntityListViewDTO> result = new List<EntityListViewDTO>();
            using (var projectContext = new MyIdeaEntities())
            {
                var listEntityListView = projectContext.EntityListView.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in listEntityListView)
                    result.Add(ToEntityListViewDTO(item, false));

            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Validation, entityID.ToString());
            return result;
        }
        public EntityListViewDTO GetOrCreateEntityListViewDTO(int entityID)
        {
            DR_Requester requester = new DR_Requester();
            requester.SkipSecurity = true;
            return GetOrCreateEntityListViewDTO(requester, entityID);
        }


        public EntityListViewDTO GetOrCreateEntityListViewDTO(DR_Requester requester, int entityID)
        {
            //** 4aecc037-4825-4e44-926a-3462d35bf2fc
            EntityListViewDTO result = null;
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entitySearch = projectContext.EntityListView.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.IsDefault == true);
                if (entitySearch != null)
                {
                    result = ToEntityListViewDTO(entitySearch, true);
                }
                else
                {
                    //BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                    var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
                    result = new EntityListViewDTO();
                    result.TableDrivedEntityID = entityID;
                    result.Title = "لیست نمایش پیش فرض";
                    result.IsDefault = true;
                    result.EntityListViewAllColumns = GenereateDefaultListViewColumns(entityDTO, null);
                }
                //var entity = bizTableDrivedEntity.GetAllEntities(projectContext, false).First(x => x.ID == entityID);



                //باید یک دیفالت ساخته و فرستاده شه
                //BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                //var entityDTO = bizTableDrivedEntity.GetPermissionedEntity(requester, entityID);
                //result = ToEntitySearchDTO(entityDTO);


            }
            return PermissionedResult(requester, result);
        }

        private EntityListViewDTO PermissionedResult(DR_Requester requester, EntityListViewDTO result)
        {
            BizColumn bizColumn = new BizColumn();
            if (!bizTableDrivedEntity.DataIsAccessable(requester, result.TableDrivedEntityID))
            {
                return null;
            }

            List<EntityListViewColumnsDTO> removeList = new List<ModelEntites.EntityListViewColumnsDTO>();
            foreach (var columnGroup in result.EntityListViewAllColumns.GroupBy(x => x.RelationshipPath))
            {
                bool pathPermission = true;
                if (string.IsNullOrEmpty(columnGroup.Key))
                {
                    foreach (var column in columnGroup)
                    {
                        if (column.ColumnID != 0 && !bizColumn.DataIsAccessable(requester, column.ColumnID))
                            removeList.Add(column);
                    }
                }
                else
                {
                    //   var relationshipTail = columnGroup.First(x => x.RelationshipTailID == columnGroup.Key).RelationshipTail;
                    pathPermission = bizEntityRelationshipTail.DataIsAccessable(requester, result.TableDrivedEntityID, columnGroup.Key);

                    if (!pathPermission)
                    {
                        foreach (var column in columnGroup)
                        {
                            removeList.Add(column);
                        }
                    }
                    else
                    {
                        foreach (var column in columnGroup)
                        {
                            if (column.ColumnID != 0 && !bizColumn.DataIsAccessable(requester, column.ColumnID))
                                removeList.Add(column);
                        }
                    }
                }

            }
            foreach (var remove in removeList)
            {
                result.EntityListViewAllColumns.Remove(remove);
            }
            return result;
        }


        private List<EntityListViewColumnsDTO> GenereateDefaultListViewColumns(TableDrivedEntityDTO firstEntity, TableDrivedEntityDTO entity = null, List<EntityListViewColumnsDTO> list = null, string relationshipPath = "", List<RelationshipDTO> relationships = null)
        {
            //** BizEntityListView.GenereateDefaultListViewColumns: 35838f12-76ec-4a3a-8f4d-e0b50248b309
            if (list == null)
                list = new List<EntityListViewColumnsDTO>();
            if (entity == null)
                entity = firstEntity;
            //AddListViewColumns(entity, list, relationshipPath, relationships);
            if (entity.IsView == false)
            {
                List<RelationshipDTO> reviewedRels = new List<RelationshipDTO>();
                foreach (var column in entity.Columns)
                {
                    if (column.PrimaryKey)
                    {
                        AddListViewColumns(firstEntity, entity, list, column, relationshipPath, relationships);
                    }

                    if (entity.Relationships.Any(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID) &&
                   (x.TypeEnum == Enum_RelationshipType.SubToSuper || x.TypeEnum == Enum_RelationshipType.ManyToOne || x.TypeEnum == Enum_RelationshipType.ExplicitOneToOne)))
                    {
                        var newrelationship = entity.Relationships.First(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID) &&
                     (x.TypeEnum == Enum_RelationshipType.SubToSuper || x.TypeEnum == Enum_RelationshipType.ManyToOne || x.TypeEnum == Enum_RelationshipType.ExplicitOneToOne));



                        int distanceFromNonSubToSuper = 0;
                        if (relationships != null)
                        {
                            distanceFromNonSubToSuper = relationships.Count(x => x.TypeEnum != Enum_RelationshipType.SubToSuper);
                        }
                        if (newrelationship.TypeEnum == Enum_RelationshipType.SubToSuper || distanceFromNonSubToSuper < 2)
                        {

                            AddListViewColumns(firstEntity, entity, list, column, relationshipPath, relationships);
                            //جلوگیری از لوپ و همچنین رابطه چند ستونی
                            if (!reviewedRels.Any(x => x.ID == newrelationship.ID)
                                && (relationships == null || !relationships.Any(x => x.ID == newrelationship.ID)))
                            {

                                //به لیست جدید از روابط میسازیم چون ممکن از روابط چند شاخه شوند
                                List<RelationshipDTO> relationshipsTail = new List<RelationshipDTO>();
                                if (relationships != null)
                                {
                                    foreach (var relItem in relationships)
                                        relationshipsTail.Add(relItem);
                                }
                                relationshipsTail.Add(newrelationship);
                                var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(newrelationship.EntityID2, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
                                GenereateDefaultListViewColumns(firstEntity, entityDTO, list, relationshipPath + (relationshipPath == "" ? "" : ",") + newrelationship.ID.ToString(), relationshipsTail);
                            }
                        }
                        reviewedRels.Add(newrelationship);
                    }

                    var key = string.IsNullOrEmpty(column.Alias) ? column.Name : column.Alias;
                    if (CheckColumnDetection(GetPriorityColumnNames(), key))
                        AddListViewColumns(firstEntity, entity, list, column, relationshipPath, relationships);

                }
            }
            else
            {
                foreach (var column in entity.Columns)
                {
                    AddListViewColumns(firstEntity, entity, list, column, relationshipPath, relationships);
                }
            }


            short index = 0;
            foreach (var item in list)
            {
                item.OrderID = index;
                index++;
            }

            return list;
        }

        private void AddListViewColumns(TableDrivedEntityDTO firstEntity, TableDrivedEntityDTO entity, List<EntityListViewColumnsDTO> list, ColumnDTO column, string relationshipPath, List<RelationshipDTO> relationships)
        {
            if (list.Any(x => x.ColumnID == column.ID))
                return;
            if (relationships != null && relationships.Any(y => y.RelationshipColumns.Any(x => x.FirstSideColumnID == column.ID || x.SecondSideColumnID == column.ID)))
                return;

            var resultColumn = new EntityListViewColumnsDTO();
            resultColumn.ColumnID = column.ID;
            resultColumn.Column = column;
            if (!string.IsNullOrEmpty(relationshipPath))
            {
                var bizEntityRelationshipTail = new BizEntityRelationshipTail();
                resultColumn.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(firstEntity.ID, relationshipPath);
            }

            string entityAlias = "";
            string entityTooltip = "";
            if (relationships != null)
            {
                for (int i = relationships.Count - 1; i >= 0; i--)
                {
                    if (relationships[i].TypeEnum != Enum_RelationshipType.SubToSuper)
                        entityAlias += (entityAlias == "" ? "" : ".") + relationships[i].Entity2Alias;

                    entityTooltip += (entityTooltip == "" ? "" : ".") + relationships[i].Entity2Alias;
                }
            }
            resultColumn.Alias = column.Alias + (entityAlias == "" ? "" : "." + entityAlias);
            resultColumn.Tooltip = column.Alias + (entityTooltip == "" ? "" : "." + entityTooltip);
            resultColumn.IsDescriptive = CheckIsDescriptive(column, relationshipPath);
            list.Add(resultColumn);
        }

        //private EntityRelationshipTailDTO CreateRelationshipTail(int firstEntityID, string relationshipPath, int lastEntityID, List<RelationshipDTO> relationships)
        //{
        //    if (!string.IsNullOrEmpty(relationshipPath))
        //    {
        //        var result = new EntityRelationshipTailDTO();
        //        result.InitialEntityID = initialiEntityID;
        //        result.InitialiEntityAlias = initialiEntityAlias;
        //        result.TargetEntityID = targetEntityID;
        //        result.TargetEntityAlias = targetEntityAlias;
        //        result.RelationshipIDPath = relationshipPath;

        //        int relationshipID = 0;
        //        string rest = "";
        //        if (relationshipPath.Contains(","))
        //        {
        //            var splt = relationshipPath.Split(',');
        //            relationshipID = Convert.ToInt32(splt[0]);
        //            for (int i = 1; i <= splt.Count() - 1; i++)
        //            {
        //                rest += (rest == "" ? "" : ",") + splt[i];
        //            }
        //        }
        //        else
        //        {
        //            relationshipID = Convert.ToInt32(relationshipPath);
        //        }

        //        result.Relationship = bizRelationship.GetRelationship(relationshipID);
        //        if (rest != "")
        //        {
        //            result.ChildTail = ToEntityRelationshipTailDTO(projectContext, rest, initialiEntityID, initialiEntityAlias, targetEntityID, targetEntityAlias, null);
        //            //result.LastRelationship = result.ChildTail.LastRelationship;
        //        }
        //    }
        //    else
        //        return null;
        //}

        List<PriorityColumnDetection> _GetPriorityColumnNames;
        List<PriorityColumnDetection> GetPriorityColumnNames()
        {
            if (_GetPriorityColumnNames == null)
            {
                _GetPriorityColumnNames = GetDescriptiveColumnNames().ToList();
                _GetPriorityColumnNames.Add(new PriorityColumnDetection("type", PriorityCompareType.ColumnAliasContainsKey));
                _GetPriorityColumnNames.Add(new PriorityColumnDetection("نوع", PriorityCompareType.Equals));
                _GetPriorityColumnNames.Add(new PriorityColumnDetection("نوع" + " ", PriorityCompareType.ColumnAliasContainsKey));

            }
            return _GetPriorityColumnNames;
        }

        private bool CheckIsDescriptive(ColumnDTO column, string relationshipTailPath)
        {
            if (column.ColumnType != Enum_ColumnType.String
                && column.ColumnType != Enum_ColumnType.Numeric
                && column.ColumnType != Enum_ColumnType.Date)
            {
                return false;
            }
            var key = string.IsNullOrEmpty(column.Alias) ? column.Name : column.Alias;
            if (string.IsNullOrEmpty(relationshipTailPath))
            {
                if (column.PrimaryKey)
                    return true;
                else
                    return CheckColumnDetection(GetDescriptiveColumnNames(), key);
            }
            else
            {
                return CheckColumnDetection(GetDescriptiveColumnNames(), key);
            }
        }

        public EntityListViewDTO GetEntityListViewWithAllColumns(DR_Requester requester, int entityID)
        {
            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);

            EntityListViewDTO result = new EntityListViewDTO();
            result.TableDrivedEntityID = entityDTO.ID;
            result.ID = 0;
            result.Title = "ستونهای ساخته شده";
            foreach (var column in entityDTO.Columns)
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

        public EntityListViewDTO GetEntityKeysListView(DR_Requester requester, int entityID)
        {
            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithoutRelationships);
            EntityListViewDTO result = new EntityListViewDTO();
            result.TableDrivedEntityID = entityDTO.ID;
            result.ID = 0;
            result.Title = "ستونهای کلید";
            foreach (var column in entityDTO.Columns.Where(x => x.PrimaryKey))
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


        public EntityListViewDTO GetEntityListView(DR_Requester requester, int EntityListViewsID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var EntityListViews = projectContext.EntityListView.First(x => x.ID == EntityListViewsID);
                var result = ToEntityListViewDTO(EntityListViews, true);
                return PermissionedResult(requester, result);
            }
        }


        private EntityListViewDTO ToEntityListViewDTO(EntityListView item, bool withDetails)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityListView, item.ID.ToString(), withDetails.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as EntityListViewDTO);

            EntityListViewDTO result = new EntityListViewDTO();
            result.TableDrivedEntityID = item.TableDrivedEntityID;
            result.ID = item.ID;
            result.IsDefault = item.IsDefault == true;

            result.Title = item.Title;
            if (withDetails)
            {
                BizColumn bizColumn = new MyModelManager.BizColumn();
                foreach (var column in item.EntityListViewColumns)
                {
                    EntityListViewColumnsDTO rColumn = new EntityListViewColumnsDTO();
                    rColumn.ID = column.ID;
                    rColumn.ColumnID = column.ColumnID;
                    rColumn.Column = bizColumn.ToColumnDTO(column.Column, true);
                    rColumn.IsDescriptive = column.IsDescriptive;
                    rColumn.Alias = column.Alias ?? rColumn.Column.Alias ?? rColumn.Column.Name;
                    rColumn.OrderID = column.OrderID ?? 0;
                    if (column.EntityRelationshipTailID != null)
                    {
                        rColumn.RelationshipTailID = column.EntityRelationshipTailID.Value;
                        rColumn.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(column.EntityRelationshipTail);
                    }
                    if (!string.IsNullOrEmpty(column.Tooltip))
                        rColumn.Tooltip = column.Tooltip;
                    else
                    {

                        if (rColumn.RelationshipTail != null)
                            rColumn.Tooltip = rColumn.RelationshipTail.TargetEntityAlias + "." + rColumn.Column.Alias;
                    }
                    rColumn.WidthUnit = column.WidthUnit ?? 0;

                    //rColumn.RelativeColumnName = rColumn.Column.Name + rColumn.RelationshipTailID.ToString();
                    result.EntityListViewAllColumns.Add(rColumn);
                }

            }

            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityListView, item.ID.ToString(), withDetails.ToString());

            return result;
        }



        private bool CheckColumnDetection(List<PriorityColumnDetection> list, string columnAlias)
        {
            return list.Any(x =>
              (x.CompareType == PriorityCompareType.Equals && x.Key.ToLower() == columnAlias.ToLower())
              || (x.CompareType == PriorityCompareType.ColumnAliasContainsKey && columnAlias.ToLower().Contains(x.Key.ToLower()))
                );
        }

        List<PriorityColumnDetection> _DescriptiveColumnNames;
        List<PriorityColumnDetection> GetDescriptiveColumnNames()
        {
            if (_DescriptiveColumnNames == null)
            {
                _DescriptiveColumnNames = new List<PriorityColumnDetection>()
                {
                    new PriorityColumnDetection("code",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("کد",PriorityCompareType.Equals),
                    new PriorityColumnDetection("کد"+" ",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("name",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("نام",PriorityCompareType.Equals),
                    new PriorityColumnDetection("نام"+" ",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("title",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("عنوان",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("number",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("شماره",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("family",PriorityCompareType.ColumnAliasContainsKey)
                };
            }
            return _DescriptiveColumnNames;
        }


        public EntityListView SaveItem(MyIdeaEntities projectContext, EntityListViewDTO message, List<EntityRelationshipTail> createdRelationshipTails = null)
        {
            var dbEntityListView = projectContext.EntityListView.FirstOrDefault(x => x.ID == message.ID);
            if (dbEntityListView == null)
            {
                dbEntityListView = new DataAccess.EntityListView();
            }

            if (message.IsDefault)
            {
                foreach (var item in projectContext.EntityListView.Where(x => x.TableDrivedEntityID == message.TableDrivedEntityID && x.ID != dbEntityListView.ID && x.IsDefault == true))
                {
                    item.IsDefault = false;
                }
            }
            dbEntityListView.TableDrivedEntityID = message.TableDrivedEntityID;
            dbEntityListView.Title = message.Title;
            dbEntityListView.IsDefault = message.IsDefault;
            //تیلهای گزارش را از روی تیلهای ستونها میسازد
            //هر دفعه پاک نشن بهتره..اصلاح بشن

            List<EntityListViewColumns> listRemove = new List<EntityListViewColumns>();
            foreach (var dbColumn in dbEntityListView.EntityListViewColumns)
            {
                if (!message.EntityListViewAllColumns.Any(x => x.RelationshipTailID == (dbColumn.EntityRelationshipTailID ?? 0) && x.ColumnID == dbColumn.ColumnID))
                    listRemove.Add(dbColumn);

            }
            foreach (var item in listRemove)
            {
                projectContext.EntityListViewColumns.Remove(item);
            }
            //while (dbEntityListView.EntityListViewRelationshipTails.Any())
            //    projectContext.EntityListViewRelationshipTails.Remove(dbEntityListView.EntityListViewRelationshipTails.First());
            if (createdRelationshipTails == null)
                createdRelationshipTails = new List<EntityRelationshipTail>();

            BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
            foreach (var column in message.EntityListViewAllColumns)
            {
                EntityListViewColumns rColumn = null;
                if (column.ID == 0)
                    rColumn = new EntityListViewColumns();
                else
                    rColumn = projectContext.EntityListViewColumns.First(x => x.ID == column.ID);

                rColumn.ColumnID = column.ColumnID;
                rColumn.Alias = column.Alias;
                rColumn.OrderID = column.OrderID;
                rColumn.Tooltip = column.Tooltip;
                rColumn.IsDescriptive = column.IsDescriptive;
                rColumn.WidthUnit = column.WidthUnit;

                rColumn.EntityRelationshipTailID = column.RelationshipTailID == 0 ? (int?)null : column.RelationshipTailID;
                if (column.RelationshipTail != null && column.RelationshipTail.ID == 0)
                {
                    if (createdRelationshipTails.Any(x => x.TableDrivedEntityID == message.TableDrivedEntityID && x.RelationshipPath == column.RelationshipTail.RelationshipIDPath))
                        rColumn.EntityRelationshipTail = createdRelationshipTails.First(x => x.TableDrivedEntityID == message.TableDrivedEntityID && x.RelationshipPath == column.RelationshipTail.RelationshipIDPath);
                    else
                    {
                        var relationshipTail = bizEntityRelationshipTail.GetOrCreateEntityRelationshipTail(projectContext, message.TableDrivedEntityID, column.RelationshipTail.RelationshipIDPath);
                        createdRelationshipTails.Add(relationshipTail);
                        rColumn.EntityRelationshipTail = relationshipTail;
                    }
                }

                dbEntityListView.EntityListViewColumns.Add(rColumn);
            }
            if (dbEntityListView.ID == 0)
                projectContext.EntityListView.Add(dbEntityListView);
            return dbEntityListView;


        }

        //public void CheckListViewPermissions(DR_Requester requester, EntityListViewDTO listView, SecurityMode securityMode)
        //{
        //    SecurityHelper securityHelper = new SecurityHelper();
        //    BizEntityRelationshipTail bizEntityRelationshipTail = new MyModelManager.BizEntityRelationshipTail();
        //    List<EntityListViewColumnsDTO> removeList = new List<ModelEntites.EntityListViewColumnsDTO>();
        //    foreach (var columnGroup in listView.EntityListViewAllColumns.GroupBy(x => x.RelationshipTailID))
        //    {

        //        if (columnGroup.Key == 0)
        //        {

        //            var permissions = securityHelper.GetAssignedPermissions(requester, listView.TableDrivedEntityID, true);
        //            foreach (var column in columnGroup)
        //            {
        //                if (!column.Column.Enabled)
        //                    removeList.Add(column);
        //                else if (securityMode == SecurityMode.View)
        //                {
        //                    if (permissions.ChildsPermissions.Any(x => x.SecurityObjectID == column.ColumnID && x.GrantedActions.Any(y => y == SecurityAction.NoAccess)))
        //                        removeList.Add(column);
        //                }
        //                else if (securityMode == SecurityMode.Edit)
        //                {
        //                    //دسترسی فیلدهایی که ریدونلی اند اما ادیت نباید بشوند باید در کلاس ادیت بررسی شود
        //                    //اینجا ریدولی ها هم میروند چون در فرم نیاز است که نمایش داده شوند
        //                    if (permissions.ChildsPermissions.Any(x => x.SecurityObjectID == column.ColumnID && x.GrantedActions.Any(y => y == SecurityAction.NoAccess)))
        //                        removeList.Add(column);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (securityMode == SecurityMode.Edit)
        //                throw new Exception("asdasdff");

        //            var relationshipTail = columnGroup.First(x => x.RelationshipTailID == columnGroup.Key).RelationshipTail;
        //            bool pathPermission = bizEntityRelationshipTail.CheckRelationshipTailPermission(requester, relationshipTail);
        //            if (!pathPermission)
        //            {
        //                foreach (var column in columnGroup)
        //                {
        //                    removeList.Add(column);
        //                }
        //            }
        //            else
        //            {
        //                var entityPermissions = securityHelper.GetAssignedPermissions(requester, relationshipTail.Relationship.ID, true);
        //                foreach (var column in columnGroup)
        //                {
        //                    if (!column.Column.Enabled)
        //                        removeList.Add(column);
        //                    else if (entityPermissions.ChildsPermissions.Any(x => x.SecurityObjectID == column.ColumnID && x.GrantedActions.Any(y => y == SecurityAction.NoAccess)))
        //                        removeList.Add(column);
        //                }
        //            }
        //        }
        //    }
        //    foreach (var remove in removeList)
        //    {
        //        listView.EntityListViewAllColumns.Remove(remove);
        //    }
        //}

        //public bool SetDefaultListView(int entityID, int iD)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbEntity = bizTableDrivedEntity.GetAllEntities(projectContext, false).FirstOrDefault(x => x.ID == entityID);
        //        dbEntity.EntityListViewID = iD;
        //        projectContext.SaveChanges();
        //    }
        //    return true;
        //}

        public int UpdateEntityListViews(EntityListViewDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbEntityListView = SaveItem(projectContext, message);
                try
                {
                    projectContext.SaveChanges();
                }
                catch (Exception ex)
                {

                }
                return dbEntityListView.ID;
            }
        }
    }
    public class PriorityColumnDetection
    {
        public PriorityColumnDetection(string key, PriorityCompareType compareType)
        {
            Key = key;
            CompareType = compareType;
        }
        public string Key { set; get; }
        public PriorityCompareType CompareType { set; get; }
    }
    public enum PriorityCompareType
    {
        Equals,
        ColumnAliasContainsKey,
        //   KeyContainsColumnAlias,
    }
}
