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
    public class BizEntityRelationshipTail
    {
        SecurityHelper securityHelper = new SecurityHelper();
        public List<EntityRelationshipTailDTO> GetEntityRelationshipTails(DR_Requester requester, int entityID)
        {
            List<EntityRelationshipTailDTO> result = new List<EntityRelationshipTailDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                IQueryable<EntityRelationshipTail> listEntityRelationshipTail;
                listEntityRelationshipTail = projectContext.EntityRelationshipTail.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in listEntityRelationshipTail)
                    if (DataIsAccessable(requester, item))
                        result.Add(ToEntityRelationshipTailDTO(item));

            }
            return result;
        }
        public EntityRelationshipTailDTO GetEntityRelationshipTail(DR_Requester requester, int EntityRelationshipTailsID)
        {
            List<EntityRelationshipTailDTO> result = new List<EntityRelationshipTailDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var EntityRelationshipTail = projectContext.EntityRelationshipTail.First(x => x.ID == EntityRelationshipTailsID);
                if (DataIsAccessable(requester, EntityRelationshipTail))
                    return ToEntityRelationshipTailDTO(EntityRelationshipTail);
                else
                    return null;
            }
        }
        public bool DataIsAccessable(DR_Requester requester, EntityRelationshipTail relationshipTail)
        {
            if (requester.SkipSecurity)
                return true;
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                return DataIsAccessable(requester, relationshipTail.TableDrivedEntityID, relationshipTail.RelationshipPath);
            }

            //return CheckRelationshipTailPermission(requester, dto);
        }
        public bool DataIsAccessable(DR_Requester requester, int initialiEntityID, string relationshipPath)
        {
            if (requester.SkipSecurity)
                return true;
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var ittem = GetRelationshipTailEntitiesAndRelationsips(projectContext, initialiEntityID, relationshipPath);
                var entities = ittem.Item2;
                return DataIsAccessable(requester, ittem.Item1, ittem.Item2);
            }

            //return CheckRelationshipTailPermission(requester, dto);
        }
        public bool DataIsAccessable(DR_Requester requester, List<TableDrivedEntity> entities, List<Relationship> relationships)
        {
            if (requester.SkipSecurity)
                return true;
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                if (entities.Any(x => !bizTableDrivedEntity.DataIsAccessable(requester, x)))
                    return false;
                if (relationships.Any(x => !bizRelationship.DataIsAccessable(requester, x, false, false)))
                    return false;
                return true;
            }
        }

        public EntityRelationshipTailDTO ToEntityRelationshipTailDTO(EntityRelationshipTail tail)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var ittem = ToEntityRelationshipTailDTO(tail.RelationshipPath, tail.TableDrivedEntityID, tail.TableDrivedEntity1.Alias, tail.TargetEntityID, tail.TableDrivedEntity.Alias, null);
                ittem.ID = tail.ID;
                return ittem;
            }
        }
        //public EntityRelationshipTailDTO JoinRelationshipTail(EntityRelationshipTailDTO firstTail, EntityRelationshipTailDTO secondTail)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var RelationshipIDPath = firstTail.RelationshipIDPath + "," + secondTail.RelationshipIDPath;
        //        return ToEntityRelationshipTailDTO(projectContext, RelationshipIDPath, firstTail.InitialEntityID, firstTail.InitialiEntityAlias, secondTail.TargetEntityID, secondTail.TargetEntityAlias, null, false).Item1;
        //    }
        //}
        BizRelationship bizRelationship = new BizRelationship();
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();

        Tuple<List<TableDrivedEntity>, List<Relationship>> GetRelationshipTailEntitiesAndRelationsips(MyIdeaEntities projectContext, int initialiEntityID, string relationshipPath
            , List<TableDrivedEntity> entities = null, List<Relationship> relationships = null)
        {
            if (entities == null)
            {
                entities = new List<TableDrivedEntity>();
                relationships = new List<Relationship>();
            }
            if (!string.IsNullOrEmpty(relationshipPath))
            {
                int relationshipID = 0;
                string rest = "";
                if (relationshipPath.Contains(","))
                {
                    var splt = relationshipPath.Split(',');
                    relationshipID = Convert.ToInt32(splt[0]);
                    for (int i = 1; i <= splt.Count() - 1; i++)
                    {
                        rest += (rest == "" ? "" : ",") + splt[i];
                    }
                }
                else
                {
                    relationshipID = Convert.ToInt32(relationshipPath);
                }
                var relationship = bizRelationship.GetAllEnabledRelationships(projectContext, false).First(x => x.ID == relationshipID);
                if (!entities.Any(x => x.ID == relationship.TableDrivedEntity.ID))
                {
                    entities.Add(relationship.TableDrivedEntity);
                }
                if (!entities.Any(x => x.ID == relationship.TableDrivedEntity1.ID))
                {
                    entities.Add(relationship.TableDrivedEntity1);
                }
                if (!relationships.Any(x => x.ID == relationshipID))
                {
                    relationships.Add(relationship);
                }
                if (rest != "")
                {
                    GetRelationshipTailEntitiesAndRelationsips(projectContext, initialiEntityID, rest, entities, relationships);
                }
            }
            return new Tuple<List<TableDrivedEntity>, List<Relationship>>(entities, relationships);
        }
        private EntityRelationshipTailDTO ToEntityRelationshipTailDTO
            (string relationshipPath, int initialiEntityID, string initialiEntityAlias, int targetEntityID, string targetEntityAlias
            , EntityRelationshipTailDTO reverseRelationshipTail)
        {

            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.RelationshipTail, initialiEntityID.ToString(), relationshipPath.ToString());
            if (cachedItem != null)
                return cachedItem as EntityRelationshipTailDTO;



            //    اینجا درست شه. بعد چک شه همه جا در درسترس بودن انتیتی از کلاس بیز انتیتی چک بشه و نه از گرقتن اسیند پرمیشن ها

            EntityRelationshipTailDTO result = new EntityRelationshipTailDTO();
            result.InitialEntityID = initialiEntityID;
            result.InitialiEntityAlias = initialiEntityAlias;
            result.TargetEntityID = targetEntityID;
            result.TargetEntityAlias = targetEntityAlias;
            //var targetEntity = bizTableDrivedEntity.GetSimpleEntity(requester, result.TargetEntityID);
            //if (targetEntity != null)
            //    result.TargetEntityAlias = targetEntity.Alias;
            //else
            //    result.TargetEntityAlias = result.TargetEntityID.ToString();
            result.RelationshipIDPath = relationshipPath;
            if (!string.IsNullOrEmpty(relationshipPath))
            {
                int relationshipID = 0;
                string rest = "";
                if (relationshipPath.Contains(","))
                {
                    var splt = relationshipPath.Split(',');
                    relationshipID = Convert.ToInt32(splt[0]);
                    for (int i = 1; i <= splt.Count() - 1; i++)
                    {
                        rest += (rest == "" ? "" : ",") + splt[i];
                    }
                }
                else
                {
                    relationshipID = Convert.ToInt32(relationshipPath);
                }

                result.Relationship = bizRelationship.GetRelationship(relationshipID);
                if (rest != "")
                {
                    result.ChildTail = ToEntityRelationshipTailDTO(rest, initialiEntityID, initialiEntityAlias, targetEntityID, targetEntityAlias, null);
                    //result.LastRelationship = result.ChildTail.LastRelationship;
                }
            }
            //else
            //    result.LastRelationship = result.Relationship;
            result.IsOneToManyTail = ((result.Relationship != null && result.Relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                || (result.ChildTail != null && result.ChildTail.IsOneToManyTail));

            result.EntityPath = result.Relationship.Entity1 + "==>" + GetEntityPath(result);
            if (reverseRelationshipTail == null)
            {
                result.ReverseRelationshipTail = ToEntityRelationshipTailDTO(GetReveseRaletionshipPath(result), result.TargetEntityID, targetEntityAlias, result.InitialEntityID, initialiEntityAlias, result);
            }
            else
                result.ReverseRelationshipTail = reverseRelationshipTail;
            //if (result.ReverseRelationshipTail.ChildTail != null)
            //    result.ReverseRelationshipTail.LastRelationship = result.ReverseRelationshipTail.ChildTail.LastRelationship;

            //var resultTuple = new Tuple<EntityRelationshipTailDTO, List<TableDrivedEntity>, List<Relationship>>(result, entities, relationships);
            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.RelationshipTail, initialiEntityID.ToString(), relationshipPath.ToString());

            return result;
            //if (item.ReverseRelationshipTailID != null)
            //    result.ReverseRelationshipTailID = item.ReverseRelationshipTailID.Value;
            //result.TargetEntityAlias = item.TableDrivedEntity1.Alias;
            //result.RelationshipSourceEntityID = item.Relationship.TableDrivedEntityID1;
            //result.RelationshipTargetEntityID = item.Relationship.TableDrivedEntityID2;
            //BizRelationship bizRElationship = new BizRelationship();
            //if (item.Relationship.TypeEnum != null)
            //    result.SourceToTargetRelationshipType = (Enum_RelationshipType)item.Relationship.TypeEnum.Value;
            //else
            //    result.SourceToTargetRelationshipType = Enum_RelationshipType.None;
            //result.SourceToTargetMasterRelationshipType = (Enum_MasterRelationshipType)item.Relationship.MasterTypeEnum;
            //foreach (var relcolumn in item.Relationship.RelationshipColumns)
            //{
            //    result.RelationshipColumns.Add(bizRElationship.ToRelationshipColumn(result.SourceToTargetMasterRelationshipType, relcolumn));
            //}
            //result.RelationshipID = item.RelationshipID;
            //result.Relationship = bizRElationship.ToRelationshipDTO(item.Relationship);
            //result.RelationshipIDPath = GetRelationshipPath(item);
            //result.RelationshipPath = GetEntityPath(item);
            //foreach (var child in item.EntityRelationshipTail11)
            //{
            //    result.ChildTail = ToEntityRelationshipTailDTO(child);
            //}

        }

        internal EntityRelationshipTailDTO ToEntityRelationshipTailDTO(int entityID, string relationshipIDTail)
        {
            return ToEntityRelationshipTailDTO(relationshipIDTail, entityID, "", 0, "", null);
        }

        public string CheckLinkedServers(TableDrivedEntityDTO firstEntity, EntityRelationshipTailDTO relationshipTail)
        {
            BizDatabase bizDatabase = new BizDatabase();

            if (firstEntity.ServerID != relationshipTail.Relationship.ServerID2)
            {
                if (!bizDatabase.LinkedServerExists(firstEntity.ServerID, relationshipTail.Relationship.ServerID2))
                {
                    return "ارتباط لینک سرور بین موجودیت های" + " " + firstEntity.Alias + " " + "و موجودیت" + " " + relationshipTail.Relationship.Entity2Alias + " " + "تعریف نشده است";
                }
            }
            if (relationshipTail.ChildTail != null)
            {
                return CheckLinkedServers(firstEntity, relationshipTail.ChildTail);
            }
            return "";
        }
        public string CheckRelationshipsLinkedServers(EntityRelationshipTailDTO relationshipTail)
        {
            if (relationshipTail.Relationship.ServerID1 != relationshipTail.Relationship.ServerID2 && string.IsNullOrEmpty(relationshipTail.Relationship.LinkedServer))
                return "ارتباط لینک سرور برای رابطه" + " " + relationshipTail.Relationship.Alias + " " + "بین موجودیت های" + " " + relationshipTail.Relationship.Entity1Alias + " " + "و موجودیت" + " " + relationshipTail.Relationship.Entity2Alias + " " + "تعریف نشده است";
            if (relationshipTail.ChildTail != null)
            {
                return CheckRelationshipsLinkedServers(relationshipTail.ChildTail);
            }
            return "";
        }

        public string CheckTailHasRelationshipWithView(EntityRelationshipTailDTO relationshipTail)
        {
            if (relationshipTail.Relationship.OtherSideIsView)
                return relationshipTail.Relationship.Entity2Alias;
            else if (relationshipTail.ChildTail != null)
            {
                return CheckTailHasRelationshipWithView(relationshipTail.ChildTail);
            }
            return "";
        }
        //public string CheckLinkedServers(int relationshipTailID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        List<string> messages = new List<string>();
        //        var tail = projectContext.EntityRelationshipTail.First(x => x.ID == relationshipTailID);
        //        var message = CheckRelationshipTailLinkedServer(messages,tail);
        //        if (!string.IsNullOrEmpty(message))
        //            messages.Add(message);
        //    }
        //}

        //private string CheckRelationshipTailLinkedServer(List<string> messages, EntityRelationshipTail tail)
        //{
        //   if(tail.TableDrivedEntity.Table.DBSchema.DatabaseInformation.DBServerID!= tail.TableDrivedEntity.Table.DBSchema.DatabaseInformation.DBServerID !=)
        //}

        private string GetReveseRaletionshipPath(EntityRelationshipTailDTO result)
        {
            if (result.ChildTail == null)
                return result.Relationship.PairRelationshipID.ToString();
            else
                return GetReveseRaletionshipPath(result.ChildTail) + "," + result.Relationship.PairRelationshipID.ToString();
        }

        private string GetEntityPath(EntityRelationshipTailDTO item)
        {
            if (item.ChildTail != null)
            {
                return item.Relationship.Entity2 + "," + GetEntityPath(item.ChildTail);
            }
            else
                return item.Relationship.Entity2;
        }



        //private string GetRelationshipPath(EntityRelationshipTail item)
        //{
        //    if (item.EntityRelationshipTail11.Any())
        //    {
        //        return item.Relationship.ID + "," + GetRelationshipPath(item.EntityRelationshipTail11.First());
        //    }
        //    else
        //        return item.Relationship.ID.ToString();
        //}
        public EntityRelationshipTail GetOrCreateEntityRelationshipTail(MyIdeaEntities projectContext, int entityID, string path)
        {
            var dbItem = projectContext.EntityRelationshipTail.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.RelationshipPath == path);
            if (dbItem == null)
            {
                dbItem = new EntityRelationshipTail();
                dbItem.TableDrivedEntityID = entityID;

                int relationshipID = 0;
                if (path.Contains(","))
                {
                    var splt = path.Split(',');
                    relationshipID = Convert.ToInt32(splt[splt.Count() - 1]);

                }
                else
                {
                    relationshipID = Convert.ToInt32(path);
                }
                dbItem.TargetEntityID = bizRelationship.GetAllEnabledRelationships(projectContext, false).First(x => x.ID == relationshipID).TableDrivedEntityID2;
                dbItem.RelationshipPath = path;
                projectContext.EntityRelationshipTail.Add(dbItem);
            }
            return dbItem;
        }



        public int GetOrCreateEntityRelationshipTailID(int entityID, string path)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                //var dbEntityRelationshipTail = ToEntityRelationshipTail(entityID, targetEntityID, EntityRelationshipTail, "");
                //projectContext.EntityRelationshipTail.Add(dbEntityRelationshipTail);
                var dbItem = projectContext.EntityRelationshipTail.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.RelationshipPath == path);
                if (dbItem == null)
                {
                    dbItem = GetOrCreateEntityRelationshipTail(projectContext, entityID, path);

                    projectContext.SaveChanges();
                }
                return dbItem.ID;
            }
        }

        //private EntityRelationshipTailDTO ToReverseRelationshipTail(EntityRelationshipTailDTO entityRelationshipTail)
        //{
        //    BizRelationship bizRelationship = new BizRelationship();

        //    var reveseLinkedList = GetReverseLinkedRelationshipTails(entityRelationshipTail);
        //    var firstTail = reveseLinkedList.First;
        //    EntityRelationshipTailDTO result = new EntityRelationshipTailDTO();
        //    result.RelationshipID = bizRelationship.GetReverseRelationship(firstTail.Value.RelationshipID).ID;
        //    if (firstTail.Next != null)
        //        result.ChildTail = ToReverseRelationshipTail(firstTail.Next);
        //    return result;
        //}

        //private EntityRelationshipTailDTO ToReverseRelationshipTail(LinkedListNode<EntityRelationshipTailDTO> next)
        //{
        //    BizRelationship bizRelationship = new BizRelationship();
        //    EntityRelationshipTailDTO result = new EntityRelationshipTailDTO();
        //    result.RelationshipID = bizRelationship.GetReverseRelationship(next.Value.RelationshipID).ID;
        //    if (next.Next != null)
        //        result.ChildTail = ToReverseRelationshipTail(next.Next);
        //    return result;
        //}

        //private LinkedList<EntityRelationshipTailDTO> GetReverseLinkedRelationshipTails(EntityRelationshipTailDTO relationshipTail)
        //{
        //    LinkedList<EntityRelationshipTailDTO> result = new LinkedList<EntityRelationshipTailDTO>();

        //    List<EntityRelationshipTailDTO> list = GetListOfRelationshipTails(relationshipTail);
        //    list.Reverse();
        //    foreach (var item in list)
        //    {
        //        result.AddLast(item);
        //    }
        //    return result;
        //}

        //private List<EntityRelationshipTailDTO> GetListOfRelationshipTails(EntityRelationshipTailDTO relationshipTail, List<EntityRelationshipTailDTO> items = null)
        //{
        //    if (items == null)
        //        items = new List<EntityRelationshipTailDTO>();
        //    if (relationshipTail != null)
        //    {
        //        items.Add(relationshipTail);
        //        return GetListOfRelationshipTails(relationshipTail.ChildTail, items);
        //    }
        //    else
        //        return items;
        //}

        //private EntityRelationshipTail ToEntityRelationshipTail(int entityID, int targetEntityID, EntityRelationshipTailDTO entityRelationshipTail, string relationshipPath)
        //{
        //    var dbEntityRelationshipTail = new EntityRelationshipTail();
        //    dbEntityRelationshipTail.ID = entityRelationshipTail.ID;
        //    dbEntityRelationshipTail.TableDrivedEntityID = entityID;
        //    dbEntityRelationshipTail.TargetEntityID = targetEntityID;
        //    dbEntityRelationshipTail.RelationshipID = entityRelationshipTail.RelationshipID;
        //    relationshipPath += (relationshipPath == "" ? "" : ",") + dbEntityRelationshipTail.RelationshipID;
        //    dbEntityRelationshipTail.RelationshipPath = relationshipPath;
        //    if (entityRelationshipTail.ChildTail != null)
        //    {
        //        dbEntityRelationshipTail.EntityRelationshipTail11.Add(ToEntityRelationshipTail(entityID, targetEntityID, entityRelationshipTail.ChildTail, relationshipPath));
        //    }
        //    return dbEntityRelationshipTail;
        //}

        //internal int GetOrCreateEntityRelationshipTail(int tableDrivedEntityID, string relationshipPath)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var fItem = GetEntityRelationshipTail(relationshipPath, projectContext, tableDrivedEntityID);
        //        if (fItem != null)
        //            return fItem.ID;
        //        else
        //        {
        //            var paths = relationshipPath.Split(',').ToList();
        //            var lastRelationshipID = Convert.ToInt32(paths.Last());
        //            var dbRelationship = projectContext.Relationship.First(x => x.ID == lastRelationshipID);
        //            var targetEntityID = dbRelationship.TableDrivedEntityID2;
        //            var createpaths = relationshipPath.Split(',').ToList();
        //            var createrelationship = Convert.ToInt32(paths.First());
        //            createpaths.Remove(createpaths.First());
        //            EntityRelationshipTailDTO dto = CreateRelationshipDTOFromPath(createpaths, createrelationship);
        //            return SaveNewRelationshipTails(tableDrivedEntityID, targetEntityID, dto);
        //        }
        //    }
        //}
        //private EntityRelationshipTail GetEntityRelationshipTail(string relationshipPath, MyIdeaEntities projectContext, int entityID)
        //{
        //    var lastTail = projectContext.EntityRelationshipTail.FirstOrDefault(x => x.RelationshipPath == relationshipPath && x.TableDrivedEntityID == entityID && !x.EntityRelationshipTail11.Any());
        //    if (lastTail != null)
        //        return GetFirstRelationshipTailOf(lastTail);
        //    else return null;
        //}
        //private EntityRelationshipTail GetFirstRelationshipTailOf(EntityRelationshipTail lastFoundTail)
        //{
        //    if (lastFoundTail.ParentID == null)
        //        return lastFoundTail;
        //    else
        //        return GetFirstRelationshipTailOf(lastFoundTail.EntityRelationshipTail2);
        //}
        //private EntityRelationshipTailDTO CreateRelationshipDTOFromPath(List<string> createpaths, int createrelationship)
        //{
        //    EntityRelationshipTailDTO dto = new EntityRelationshipTailDTO();
        //    dto.RelationshipID = createrelationship;
        //    if (createpaths.Any())
        //    {
        //        var nextRelationshipID = Convert.ToInt32(createpaths.First());
        //        createpaths.Remove(createpaths.First());
        //        dto.ChildTail = CreateRelationshipDTOFromPath(createpaths, nextRelationshipID);
        //    }
        //    return dto;
        //}



        //private EntityRelationshipTail GetEntityRelationshipTail(List<string> paths, MyIdeaEntities projectContext, IQueryable<EntityRelationshipTail> childTails)
        //{
        //    //تکنیک قشنگی بود که ار پت ریلیشن شیپ تیل رو پیدا میکرد.اما مسیر در خود جدول قرار گرفت و این بلااستفاده شد.
        //    if (paths.Count == 0)
        //    {
        //        if (childTails.Any(x => !x.EntityRelationshipTail11.Any()))
        //        {
        //            var lastFoundTail = childTails.First(x => !x.EntityRelationshipTail11.Any());
        //            return GetFirstRelationshipTailOf(lastFoundTail);
        //        }
        //        else
        //            return null;
        //    }
        //    else
        //    {
        //        if (childTails.Count() == 0)
        //            return null;
        //        else
        //        {
        //            var relationship = Convert.ToInt32(paths.First());
        //            paths.Remove(paths.First());
        //            foreach (var childTail in childTails)
        //            {
        //                var newchildTails = projectContext.EntityRelationshipTail.Where(x => x.ParentID == childTail.ID && x.RelationshipID == relationship) as IQueryable<EntityRelationshipTail>;
        //                var fChildTail = GetEntityRelationshipTail(paths, projectContext, newchildTails);
        //                if (fChildTail != null)
        //                    return fChildTail;
        //            }
        //            return null;
        //        }
        //    }

        //}


    }

}
