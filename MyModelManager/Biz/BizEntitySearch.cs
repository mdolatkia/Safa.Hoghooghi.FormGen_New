
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
    public class BizEntitySearch
    {
        SecurityHelper securityHelper = new SecurityHelper();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        public event EventHandler<ItemImportingStartedArg> ItemImportingStarted;
        BizEntityRelationshipTail bizEntityRelationshipTail = new MyModelManager.BizEntityRelationshipTail();
        public List<EntitySearchDTO> GetEntitySearchs(DR_Requester requester, int entityID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Validation, entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntitySearchDTO>);

            List<EntitySearchDTO> result = new List<EntitySearchDTO>();
            using (var projectContext = new MyIdeaEntities())
            {
                var listEntitySearch = projectContext.EntitySearch.Where(x => x.TableDrivedEntityID1 == entityID);
                foreach (var item in listEntitySearch)
                    result.Add(ToEntitySearchDTO(item, false));

            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Validation, entityID.ToString());
            return result;
        }
        public EntitySearchDTO GetOrCreateEntitySearchDTO(int entityID)
        {
            DR_Requester requester = new DR_Requester();
            requester.SkipSecurity = true;
            return GetOrCreateEntitySearchDTO(requester, entityID);
        }
        public EntitySearchDTO GetOrCreateEntitySearchDTO(DR_Requester requester, int entityID)
        {
            EntitySearchDTO result = null;
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var entitySearch = projectContext.EntitySearch.FirstOrDefault(x => x.TableDrivedEntityID1 == entityID && x.IsDefault == true);
                if (entitySearch != null)
                {
                    result = ToEntitySearchDTO(entitySearch, true);
                }
                else
                {
                    var entityDTO = bizTableDrivedEntity.GetTableDrivedEntity(entityID, EntityColumnInfoType.WithSimpleColumns, EntityRelationshipInfoType.WithRelationships);
                    result = new EntitySearchDTO();
                    result.TableDrivedEntityID = entityID;
                    result.Title = "لیست جستجوی پیش فرض";
                    result.IsDefault = true;
                    result.EntitySearchAllColumns = GenereateDefaultSearchColumns(entityDTO, null);
                }

            }
            return PermissionedResult(requester, result);
        }

        private EntitySearchDTO PermissionedResult(DR_Requester requester, EntitySearchDTO result)
        {
            BizColumn bizColumn = new BizColumn();
            if (!bizTableDrivedEntity.DataIsAccessable(requester, result.TableDrivedEntityID))
            {
                return null;
            }

            List<EntitySearchColumnsDTO> removeList = new List<ModelEntites.EntitySearchColumnsDTO>();
            foreach (var columnGroup in result.EntitySearchAllColumns.GroupBy(x => x.RelationshipPath))
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
                result.EntitySearchAllColumns.Remove(remove);
            }
            return result;
        }

        private List<EntitySearchColumnsDTO> GenereateDefaultSearchColumns(TableDrivedEntityDTO entity, List<EntitySearchColumnsDTO> list = null, string relationshipPath = "", List<RelationshipDTO> relationships = null)
        {
            //** 8ab306e9-0d52-4be6-95c0-9e5b4c36a21c
            if (list == null)
                list = new List<EntitySearchColumnsDTO>();
            //   AddSearchColumns(entity, list, relationshipPath, relationships);
            if (entity.IsView == false)
            {
                //کلا بر اساس ترتیب ستونها جلو نمیریم چون برای سرچ و نمایش اولویت با ستونها خود موجودیت است و بعد روابط
                //البته روابط هم اولویت با ارث بری فرزند به پدر است
                List<RelationshipDTO> reviewedRels = new List<RelationshipDTO>();
                foreach (var column in entity.Columns)
                {
                    if (column.PrimaryKey)
                    {
                        AddSearchColumns(entity, list, column, relationshipPath, relationships);
                    }
                    if (entity.Relationships.Any(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID) &&
                   (x.TypeEnum == Enum_RelationshipType.SubToSuper || x.TypeEnum == Enum_RelationshipType.ManyToOne || x.TypeEnum == Enum_RelationshipType.ExplicitOneToOne)))
                    {
                        var newrelationship = entity.Relationships.First(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == column.ID) &&
                          (x.TypeEnum == Enum_RelationshipType.SubToSuper || x.TypeEnum == Enum_RelationshipType.ManyToOne || x.TypeEnum == Enum_RelationshipType.ExplicitOneToOne));

                        بقیه موجودیت هارو ببینم خوب تولید شدن یا نه

                        int distanceFromNonSubToSuper = 0;
                        if (relationships != null)
                        {
                            distanceFromNonSubToSuper = relationships.Count(x => x.TypeEnum != Enum_RelationshipType.SubToSuper);
                        }
                        if (newrelationship.TypeEnum == Enum_RelationshipType.SubToSuper || distanceFromNonSubToSuper < 2)
                        {
                            if (!reviewedRels.Any(x => x.ID == newrelationship.ID))
                            {
                                if (relationships == null ||
                                newrelationship.TypeEnum != Enum_RelationshipType.SubToSuper)
                                {

                                    string entityAlias = "";
                                    if (relationships != null)
                                    {
                                        for (int i = relationships.Count - 1; i >= 0; i--)
                                        {
                                            entityAlias += (entityAlias == "" ? "" : ".") + relationships[i].Entity2Alias;
                                        }
                                    }

                                    var resultColumn = new EntitySearchColumnsDTO();
                                    resultColumn.CreateRelationshipTailPath = relationshipPath + (relationshipPath == "" ? "" : ",") + newrelationship.ID.ToString();
                                    resultColumn.Alias = newrelationship.Entity2Alias + (entityAlias == "" ? "" : "." + entityAlias);
                                    resultColumn.Tooltip = newrelationship.Entity2Alias + (entityAlias == "" ? "" : "." + entityAlias);
                                    list.Add(resultColumn);
                                }
                            }
                            AddSearchColumns(entity, list, column, relationshipPath, relationships);

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
                                GenereateDefaultSearchColumns(entityDTO, list, relationshipPath + (relationshipPath == "" ? "" : ",") + newrelationship.ID.ToString(), relationshipsTail);
                            }
                            reviewedRels.Add(newrelationship);

                        }
                    }

                    var key = string.IsNullOrEmpty(column.Alias) ? column.Name : column.Alias;
                    if (CheckColumnDetection(GetPriorityColumnNames(), key))
                        AddSearchColumns(entity, list, column, relationshipPath, relationships);

                }
            }
            else
            {
                foreach (var column in entity.Columns)
                {
                    AddSearchColumns(entity, list, column, relationshipPath, relationships);
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

        private void AddSearchColumns(TableDrivedEntityDTO entity, List<EntitySearchColumnsDTO> list, ColumnDTO column, string relationshipPath, List<RelationshipDTO> relationships)
        {
            if (list.Any(x => x.ColumnID == column.ID))
                return;
            if (relationships != null && relationships.Any(y => y.RelationshipColumns.Any(x => x.FirstSideColumnID == column.ID || x.SecondSideColumnID == column.ID)))
                return;

            var resultColumn = new EntitySearchColumnsDTO();
            resultColumn.ColumnID = column.ID;
            resultColumn.Column = column;
            resultColumn.CreateRelationshipTailPath = relationshipPath;
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

            list.Add(resultColumn);

        }


        List<PriorityColumnDetection> _GetPriorityColumnNames;
        List<PriorityColumnDetection> GetPriorityColumnNames()
        {
            if (_GetPriorityColumnNames == null)
            {
                _GetPriorityColumnNames = new List<PriorityColumnDetection>()
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
                    new PriorityColumnDetection("family",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("نوع",PriorityCompareType.Equals),
                      new PriorityColumnDetection("نوع"+" ",PriorityCompareType.ColumnAliasContainsKey),
                    new PriorityColumnDetection("type",PriorityCompareType.ColumnAliasContainsKey)
                };

            }
            return _GetPriorityColumnNames;
        }
        public EntitySearchDTO GetEntitySearch(DR_Requester requester, int EntitySearchsID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var EntitySearchs = projectContext.EntitySearch.First(x => x.ID == EntitySearchsID);
                var result = ToEntitySearchDTO(EntitySearchs, true);
                return PermissionedResult(requester, result);
            }
        }

        private EntitySearchDTO ToEntitySearchDTO(EntitySearch item, bool withDetails)
        {
            EntitySearchDTO result = new EntitySearchDTO();
            result.TableDrivedEntityID = item.TableDrivedEntityID1;
            result.ID = item.ID;
            result.Title = item.Title;
            result.IsDefault = item.IsDefault == true;

            BizEntityRelationshipTail bizEntityRelationshipTail = new MyModelManager.BizEntityRelationshipTail();
            if (withDetails)
            {
                BizColumn bizColumn = new MyModelManager.BizColumn();
                foreach (var column in item.EntitySearchColumns)
                {
                    EntitySearchColumnsDTO rColumn = new EntitySearchColumnsDTO();
                    rColumn.ID = column.ID;
                    rColumn.ColumnID = column.ColumnID ?? 0;
                    if (column.Column != null)
                        rColumn.Column = bizColumn.ToColumnDTO(column.Column, true);
                    if (column.ColumnID != null)
                        rColumn.Alias = column.Alias ?? column.Column.Alias ?? column.Column.Name;
                    else
                        rColumn.Alias = column.Alias ?? column.EntityRelationshipTail.TableDrivedEntity.Alias ?? column.EntityRelationshipTail.TableDrivedEntity.Name;
                    rColumn.OrderID = column.OrderID ?? 0;

                    //rColumn.WidthUnit = column.WidthUnit ?? 0;
                    if (column.EntityRelationshipTailID != null)
                    {
                        rColumn.RelationshipTailID = column.EntityRelationshipTailID.Value;
                        rColumn.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(column.EntityRelationshipTail);
                    }
                    if (!string.IsNullOrEmpty(column.Tooltip))
                        rColumn.Tooltip = column.Tooltip;
                    else
                    {
                        if (rColumn.RelationshipTail != null && rColumn.Column != null)
                            rColumn.Tooltip = rColumn.RelationshipTail.ReverseRelationshipTail.TargetEntityAlias + "." + rColumn.Column.Alias;
                    }
                    result.EntitySearchAllColumns.Add(rColumn);
                }

            }
            return result;
        }


        private bool CheckColumnDetection(List<PriorityColumnDetection> list, string columnAlias)
        {
            return list.Any(x =>
              (x.CompareType == PriorityCompareType.Equals && x.Key.ToLower() == columnAlias.ToLower())
              || (x.CompareType == PriorityCompareType.ColumnAliasContainsKey && columnAlias.ToLower().Contains(x.Key.ToLower()))
                );
        }


        public EntitySearch SaveItem(MyIdeaEntities projectContext, EntitySearchDTO message, List<EntityRelationshipTail> createdRelationshipTails = null)
        {
            var dbEntitySearch = projectContext.EntitySearch.FirstOrDefault(x => x.ID == message.ID);
            if (dbEntitySearch == null)
            {
                dbEntitySearch = new DataAccess.EntitySearch();
            }
            if (message.IsDefault)
            {
                foreach (var item in projectContext.EntityListView.Where(x => x.TableDrivedEntityID == message.TableDrivedEntityID && x.ID != dbEntitySearch.ID && x.IsDefault == true))
                {
                    item.IsDefault = false;
                }
            }
            dbEntitySearch.TableDrivedEntityID1 = message.TableDrivedEntityID;
            dbEntitySearch.Title = message.Title;
            dbEntitySearch.IsDefault = message.IsDefault;
            //تیلهای گزارش را از روی تیلهای ستونها میسازد
            //هر دفعه پاک نشن بهتره..اصلاح بشن
            while (dbEntitySearch.EntitySearchColumns.Any())
                projectContext.EntitySearchColumns.Remove(dbEntitySearch.EntitySearchColumns.First());
            //while (dbEntitySearch.EntitySearchRelationshipTails.Any())
            //    projectContext.EntitySearchRelationshipTails.Remove(dbEntitySearch.EntitySearchRelationshipTails.First());
            if (createdRelationshipTails == null)
                createdRelationshipTails = new List<EntityRelationshipTail>();
            BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
            foreach (var column in message.EntitySearchAllColumns)
            {
                EntitySearchColumns rColumn = new EntitySearchColumns();
                if (column.ColumnID != 0)
                    rColumn.ColumnID = column.ColumnID;
                else
                    rColumn.ColumnID = null;
                rColumn.Alias = column.Alias;
                rColumn.Tooltip = column.Tooltip;

                rColumn.OrderID = column.OrderID;
                // rColumn.WidthUnit = column.WidthUnit;

                if (string.IsNullOrEmpty(column.CreateRelationshipTailPath))
                    rColumn.EntityRelationshipTailID = column.RelationshipTailID == 0 ? (int?)null : column.RelationshipTailID;
                else
                {
                    if (createdRelationshipTails.Any(x => x.TableDrivedEntityID == message.TableDrivedEntityID && x.RelationshipPath == column.CreateRelationshipTailPath))
                        rColumn.EntityRelationshipTail = createdRelationshipTails.First(x => x.TableDrivedEntityID == message.TableDrivedEntityID && x.RelationshipPath == column.CreateRelationshipTailPath);
                    else
                    {
                        var relationshipTail = bizEntityRelationshipTail.GetOrCreateEntityRelationshipTail(projectContext, message.TableDrivedEntityID, column.CreateRelationshipTailPath);
                        createdRelationshipTails.Add(relationshipTail);
                        rColumn.EntityRelationshipTail = relationshipTail;
                    }
                }


                //if (column.RelationshipTailID != 0 || !string.IsNullOrEmpty(column.RelationshipPath))
                //{
                //    int tailID = 0;
                //    if (column.RelationshipTailID != 0)
                //        tailID = column.RelationshipTailID;
                //    else
                //        tailID = bizEntityRelationshipTail.GetOrCreateEntityRelationshipTail(message.TableDrivedEntityID, column.RelationshipPath);
                //    //var relatedListReportTail = dbEntitySearch.EntitySearchRelationshipTails.FirstOrDefault(x => x.EntityRelationshipTailID == tailID);
                //    //if (relatedListReportTail == null)
                //    //{
                //    //    relatedListReportTail = new EntitySearchRelationshipTails();
                //    //    relatedListReportTail.EntityRelationshipTailID = tailID;
                //    //    dbEntitySearch.EntitySearchRelationshipTails.Add(relatedListReportTail);
                //    //}

                //    //rColumn.EntitySearchRelationshipTails = relatedListReportTail;
                //}
                dbEntitySearch.EntitySearchColumns.Add(rColumn);
            }

            if (dbEntitySearch.ID == 0)
                projectContext.EntitySearch.Add(dbEntitySearch);
            return dbEntitySearch;
        }
        public int UpdateEntitySearchs(EntitySearchDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbItem = SaveItem(projectContext, message);
                projectContext.SaveChanges();
                return dbItem.ID;
            }
        }
    }

}
