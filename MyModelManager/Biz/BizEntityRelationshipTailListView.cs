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
    public class BizEntityRelationshipTailDataMenu
    {
        SecurityHelper securityHelper = new SecurityHelper();
        public event EventHandler<ItemImportingStartedArg> ItemImportingStarted;
        BizEntityRelationshipTail bizEntityRelationshipTail = new MyModelManager.BizEntityRelationshipTail();
        public List<EntityRelationshipTailDataMenuDTO> GetEntityRelationshipTailDataMenus(DR_Requester requester, int relationshipTailID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Validation, entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntityRelationshipTailDataMenuDTO>);

            List<EntityRelationshipTailDataMenuDTO> result = new List<EntityRelationshipTailDataMenuDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityRelationshipTailDataMenu = projectContext.EntityRelationshipTailDataMenu.Where(x => x.EntityRelationshipTailID == relationshipTailID);
                foreach (var item in listEntityRelationshipTailDataMenu)
                    result.Add(ToEntityRelationshipTailDataMenuDTO(item, true));

            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Validation, entityID.ToString());
            return result;
        }
        public EntityRelationshipTailDataMenuDTO GetEntityRelationshipTailDataMenu(DR_Requester requester, int id)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Validation, entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntityRelationshipTailDataMenuDTO>);


            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbItem = projectContext.EntityRelationshipTailDataMenu.First(x => x.ID == id);
                return ToEntityRelationshipTailDataMenuDTO(dbItem, true);

            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Validation, entityID.ToString());

        }
        public EntityRelationshipTailDataMenuDTO ToEntityRelationshipTailDataMenuDTO(EntityRelationshipTailDataMenu item, bool withDetails)
        {
            EntityRelationshipTailDataMenuDTO result = new EntityRelationshipTailDataMenuDTO();
            result.ID = item.ID;

            result.EntityRelationshipTailID = item.EntityRelationshipTailID;
            result.Name = item.Name;
            foreach (var fitem in item.EntityRelationshipTailDataMenuItems)
            {
                result.Items.Add(new EntityRelationshipTailDataMenuItemsDTO()
                {
                    ID = fitem.ID,
                    DataMenuSettingID = fitem.DataMenuSettingID ?? 0,
                    EntityListViewID = fitem.EntityListViewID ?? 0,
                    Path = fitem.Path,
                    TableDrivedEntityID = fitem.TableDrivedEntityID
                });
            }
            return result;
        }

        public int UpdateEntityRelationshipTailDataMenu(EntityRelationshipTailDataMenuDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                EntityRelationshipTailDataMenu dbItem = null;
                if (message.ID == 0)
                {
                    dbItem = new EntityRelationshipTailDataMenu();
                    projectContext.EntityRelationshipTailDataMenu.Add(dbItem);
                }
                else
                    dbItem = projectContext.EntityRelationshipTailDataMenu.First(x => x.ID == message.ID);

                dbItem.Name = message.Name;
                dbItem.EntityRelationshipTailID = message.EntityRelationshipTailID;
                while (dbItem.EntityRelationshipTailDataMenuItems.Any())
                    projectContext.EntityRelationshipTailDataMenuItems.Remove(dbItem.EntityRelationshipTailDataMenuItems.First());
                foreach (var fitem in message.Items.Where(x => x.DataMenuSettingID != 0 || x.EntityListViewID != 0))
                {
                    dbItem.EntityRelationshipTailDataMenuItems.Add(new EntityRelationshipTailDataMenuItems()
                    {
                        DataMenuSettingID = fitem.DataMenuSettingID == 0 ? null : (int?)fitem.DataMenuSettingID,
                        EntityListViewID = fitem.EntityListViewID == 0 ? null : (int?)fitem.EntityListViewID,
                        Path = fitem.Path,
                        TableDrivedEntityID = fitem.TableDrivedEntityID
                    });
                }
                projectContext.SaveChanges();
                return dbItem.ID;
            }
        }
    }
}
