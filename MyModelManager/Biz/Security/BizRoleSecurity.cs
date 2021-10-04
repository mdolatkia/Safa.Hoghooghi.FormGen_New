using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyModelManager;

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizRoleSecurity
    {

        //public List<EntityRoleSecurityDTO> GetEntityRoleSecurities(int entityID, bool withDetails)
        //{

        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var list = projectContext.EntityRoleSecurity.Where(x => x.TableDrivedEntityID == entityID);
        //        return ToEntityRoleSecuritiesDTO(list.ToList());
        //    }
        //}
        //private List<EntityRoleSecurityDTO> ToEntityRoleSecuritiesDTO(List<EntityRoleSecurity> list)
        //{
        //    List<EntityRoleSecurityDTO> result = new List<EntityRoleSecurityDTO>();
        //    foreach (var dbitem in list)
        //    {
        //        EntityRoleSecurityDTO item = new EntityRoleSecurityDTO();
        //        item.ID = dbitem.ID;
        //        item.RoleID = dbitem.RoleID;
        //        item.TableDrivedEntityID = dbitem.TableDrivedEntityID;
        //        result.Add(item);
        //    }
        //    return result;
        //}
        //public void UpdateRoleSecurity(int entityID, List<EntityRoleSecurityDTO> message)
        //{
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
        //        while (entity.EntityRoleSecurity.Any())
        //            projectContext.EntityRoleSecurity.Remove(entity.EntityRoleSecurity.First());
        //        foreach (var item in message)
        //        {
        //            var dbItem = new EntityRoleSecurity();
        //            dbItem.RoleID = item.RoleID;
        //            entity.EntityRoleSecurity.Add(dbItem);
        //        }
        //        projectContext.SaveChanges();
        //    }
        //}


        public EntitySecurityDirectDTO GetEntitySecurityDirect(DR_Requester requester, int id, bool withDetails)
        {
            EntitySecurityDirectDTO result = new EntitySecurityDirectDTO();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var item = projectContext.EntitySecurityDirect.FirstOrDefault(x => x.ID == id);
                if (item != null)
                    return ToEntitySecurityDirectDTO(requester,item, withDetails);
                else
                    return null;

            }
        }

        public bool DeleteEntitySecurityDirect(int id)
        {
            EntitySecurityDirectDTO result = new EntitySecurityDirectDTO();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var item = projectContext.EntitySecurityDirect.FirstOrDefault(x => x.ID == id);
                foreach (var child in item.EntitySecurityDirectStates.ToList())
                    projectContext.EntitySecurityDirectStates.Remove(child);
                projectContext.EntitySecurityDirect.Remove(item);
                projectContext.SaveChanges();
                return true;
            }
        }
        public List<EntitySecurityDirectDTO> GetEntitySecurityDirects(DR_Requester requester, string search)
        {
            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var list = projectContext.EntitySecurityDirect;
                foreach (var item in list)
                {
                    result.Add(ToEntitySecurityDirectDTO(requester, item, false));
                }
            }
            return result;
        }

        public List<EntitySecurityDirectDTO> GetEntitySecurityDirects(DR_Requester requester, int entityID, bool withDetails)
        {
            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var list = projectContext.EntitySecurityDirect.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in list)
                {
                    result.Add(ToEntitySecurityDirectDTO(requester, item, withDetails));
                }
            }
            return result;
        }

        public EntitySecurityDirectDTO ToEntitySecurityDirectDTO(DR_Requester requester, EntitySecurityDirect item, bool withDetails)
        {
            EntitySecurityDirectDTO result = new EntitySecurityDirectDTO();
            result.ID = item.ID;
            if (item.SecuritySubjectID != null)
                result.SecuritySubjectID = item.SecuritySubjectID.Value;
            else
                result.SecuritySubjectID = 0;

            // result.Mode = (SecurityMode)item.Mode;
            result.IgnoreSecurity = item.IgnoreSecurity;
            result.TableDrivedEntityID = item.TableDrivedEntityID;

            foreach (var state in item.EntitySecurityDirectStates)
            {
                EntitySecurityDirectStatesDTO securityState = new EntitySecurityDirectStatesDTO();
                securityState.EntityStateID = state.TableDrivedEntityStateID;
                if (withDetails)
                {
                    BizEntityState bizEntityState = new BizEntityState();
                    securityState.EntityState = bizEntityState.ToEntityStateDTO(requester, state.TableDrivedEntityState, withDetails);
                }
                result.EntityStates.Add(securityState);
            }
            return result;
        }

        //private EntitySecurityConditionDTO ToEntitySecurityConditionDTO(EntitySecurityCondition item, bool withDetails)
        //{
        //    var result = new EntitySecurityConditionDTO();
        //    result.ColumnID = item.ColumnID;
        //    result.Value = item.Value;
        //    if (item.DatabaseFunctionID != null)
        //        result.DBFunctionID = item.DatabaseFunctionID.Value;

        //    if (item.EntityRelationshipTailID != null)
        //    {

        //        result.RelationshipTailID = item.EntityRelationshipTailID.Value;

        //    }
        //    if (withDetails)
        //    {
        //        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        //        if (item.EntityRelationshipTailID != null)
        //            result.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
        //        BizColumn bizColumn = new BizColumn();
        //        result.Column = bizColumn.ToColumnDTO(item.Column, true);
        //    }
        //    result.ReservedValue = (SecurityReservedValue)item.ReservedValue;
        //    result.Operator = (EntitySecurityOperator)item.Operator;
        //    return result;
        //}

        public EntitySecurityInDirectDTO GetEntitySecurityInDirect(DR_Requester requester, int entityID, bool withDetails)
        {
            EntitySecurityInDirectDTO result = new EntitySecurityInDirectDTO();
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var item = projectContext.EntitySecurityInDirect.FirstOrDefault(x => x.TableDrivedEntityID == entityID);
                if (item != null)
                    return ToEntitySecurityInDirectDTO(item, withDetails);
                else
                    return null;

            }
        }

        public EntitySecurityInDirectDTO ToEntitySecurityInDirectDTO(EntitySecurityInDirect item, bool withDetails)
        {
            EntitySecurityInDirectDTO result = new EntitySecurityInDirectDTO();
            result.ID = item.ID;
            //result.DirectRoleSecurityID = item.EntitySecurityDirectID;
            result.RelationshipTailID = item.EntityRelationshipTailID;
            result.TableDrivedEntityID = item.TableDrivedEntityID;
            if (withDetails)
            {
                BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
                result.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
                //result.DirectRoleSecurity = ToEntitySecurityDirectDTO(item.EntitySecurityDirect, withDetails);
            }

            return result;
        }

        public int UpdateEntitySecurityDirect(EntitySecurityDirectDTO message)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var dbItem = projectContext.EntitySecurityDirect.FirstOrDefault(x => x.ID == message.ID);
                if (dbItem == null)
                {
                    dbItem = new DataAccess.EntitySecurityDirect();
                    projectContext.EntitySecurityDirect.Add(dbItem);
                }
                dbItem.TableDrivedEntityID = message.TableDrivedEntityID;
                if (message.SecuritySubjectID != 0)
                    dbItem.SecuritySubjectID = message.SecuritySubjectID;
                else
                    dbItem.SecuritySubjectID = null;
                //dbItem.Mode = (short)message.Mode;
                dbItem.IgnoreSecurity = message.IgnoreSecurity;
                //dbItem.ConditionsAndOrType = (short)message.ConditionAndORType;

                while (dbItem.EntitySecurityDirectStates.Any())
                    projectContext.EntitySecurityDirectStates.Remove(dbItem.EntitySecurityDirectStates.First());
                foreach (var securityState in message.EntityStates)
                {
                    var dbCondition = new EntitySecurityDirectStates();
                    dbCondition.TableDrivedEntityStateID = securityState.EntityStateID;
                    dbItem.EntitySecurityDirectStates.Add(dbCondition);
                }

                //while (dbItem.EntitySecurityCondition.Any())
                //    projectContext.EntitySecurityCondition.Remove(dbItem.EntitySecurityCondition.First());
                //foreach (var conditoin in message.Conditions)
                //{
                //    var dbCondition = new EntitySecurityCondition();
                //    dbCondition.ColumnID = conditoin.ColumnID;
                //    dbCondition.Value = conditoin.Value;
                //    if (conditoin.DBFunctionID != 0)
                //        dbCondition.DatabaseFunctionID = conditoin.DBFunctionID;
                //    else
                //        dbCondition.DatabaseFunctionID = null;
                //    if (conditoin.RelationshipTailID != 0)
                //        dbCondition.EntityRelationshipTailID = conditoin.RelationshipTailID;
                //    else
                //        dbCondition.EntityRelationshipTailID = null;
                //    dbCondition.ReservedValue = (short)conditoin.ReservedValue;
                //    dbCondition.Operator = (short)conditoin.Operator;
                //    dbItem.EntitySecurityCondition.Add(dbCondition);
                //}

                projectContext.SaveChanges();
                return dbItem.ID;
            }
        }
        public void UpdateEntitySecurityInDirect(EntitySecurityInDirectDTO message)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var dbItem = projectContext.EntitySecurityInDirect.FirstOrDefault(x => x.ID == message.ID);
                if (dbItem == null)
                {
                    dbItem = new DataAccess.EntitySecurityInDirect();
                    projectContext.EntitySecurityInDirect.Add(dbItem);
                }
                dbItem.TableDrivedEntityID = message.TableDrivedEntityID;
                dbItem.EntityRelationshipTailID = message.RelationshipTailID;
                projectContext.SaveChanges();
            }
        }
        public EntityDataSecurityItems GetPostEntitySecurityItems(DR_Requester requester, int entityID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.ConditionalPermission, securitySubjectID.ToString(), entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<ConditionalPermissionDTO>);
            //List<int> organizationTypeIDs = new List<int>();
            //List<int> organizationIDs = new List<int>();
            //List<int> roleTypeIDs = new List<int>();
            //List<int> orgTypeRoleTypeIDs = new List<int>();
            //Tuple<EntitySecurityInDirectDTO, List<EntitySecurityDirectDTO>> result;= new Tuple<EntitySecurityInDirectDTO, List<EntitySecurityDirectDTO>>();

            BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();
            List<PostEntityDataSecurityItems> allPostsDirectSecurities = new List<PostEntityDataSecurityItems>();
            EntitySecurityInDirectDTO indisrectSecurityDTO = null;
            using (var context = new MyProjectEntities())
            {
                var directSecurityEntityID = entityID;
                var disrectSecurities = context.EntitySecurityDirect.Where(x => x.TableDrivedEntityID == entityID);// && x.Mode == (short)securityMode);
                if (!disrectSecurities.Any())
                {
                    var indisrectSecurity = context.EntitySecurityInDirect.FirstOrDefault(x => x.TableDrivedEntityID == entityID);
                    if (indisrectSecurity == null)
                        return null;
                    else
                    {
                        indisrectSecurityDTO = bizRoleSecurity.ToEntitySecurityInDirectDTO(indisrectSecurity, true);
                        var targetEntity = indisrectSecurity.EntityRelationshipTail.TableDrivedEntity;
                        directSecurityEntityID = targetEntity.ID;
                        disrectSecurities = targetEntity.EntitySecurityDirect.AsQueryable();
                    }
                }
                var organizationPosts = GetDBOrganizationPosts(context, requester);
                BizOrganization bizOrganization = new BizOrganization();
                foreach (var post in organizationPosts)
                {
                    //  List<EntitySecurityDirectDTO> listDirectSecuritiesForPost = new List<EntitySecurityDirectDTO>();
                    var postDto = requester.Posts.FirstOrDefault(x => x.ID == post.ID);
                    if (postDto == null)
                        postDto = bizOrganization.GetOrganizationPost(post.ID);
                    var postDisrectSecurities = GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.SecuritySubject.ID);
                    postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.Organization.SecuritySubject.ID));
                    postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.SecuritySubject.ID));
                    postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.OrganizationType.SecuritySubject.ID));
                    postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.RoleType.SecuritySubject.ID));


                    //منطق اینجا رو نفهمیدم غیر فعال شد. بجاش بالا همه دسترسی ها تجمیع می شوند
                    //////if (postDisrectSecurities.Any())
                    //////    listDirectSecuritiesForPost.AddRange(postDisrectSecurities);
                    //////else
                    //////{
                    //////    var orgTypeRoleTypeDisrectSecurities = GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.SecuritySubject.ID);
                    //////    var organizationDisrectSecurities = GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.Organization.SecuritySubject.ID);
                    //////    if (orgTypeRoleTypeDisrectSecurities.Any())
                    //////    {
                    //////        //اینجا دسترسی های موازی با هم جمع میشوند زیرا معلوم نیست بروی کدام آبجکت دارند اعمال میشوند و تصمیم گیری در مورد تداخل دسترسی بروی یک آبجکت به کلاینت واگذار میشود
                    //////        listDirectSecuritiesForPost.AddRange(orgTypeRoleTypeDisrectSecurities);
                    //////        listDirectSecuritiesForPost.AddRange(organizationDisrectSecurities);
                    //////    }
                    //////    else
                    //////    {
                    //////        var roleTypeDisrectSecurities = GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.RoleType.SecuritySubject.ID);
                    //////        if (organizationDisrectSecurities.Any())
                    //////        {
                    //////            listDirectSecuritiesForPost.AddRange(organizationDisrectSecurities);
                    //////            listDirectSecuritiesForPost.AddRange(roleTypeDisrectSecurities);
                    //////        }
                    //////        else
                    //////        {
                    //////            var organizationTypeDisrectSecurities = GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.OrganizationType.SecuritySubject.ID);
                    //////            listDirectSecuritiesForPost.AddRange(organizationTypeDisrectSecurities);
                    //////            listDirectSecuritiesForPost.AddRange(roleTypeDisrectSecurities);
                    //////        }
                    //////    }
                    //////}





                    //if (listDirectSecuritiesForPost.Any())
                    //{
                    //اونهای که سابجکت نال دارند و عمومی هستند
                    var generalSecurityItems = GetGeneralEntitySecurityItems(requester, directSecurityEntityID);
                    if (generalSecurityItems.Any())
                        postDisrectSecurities.AddRange(generalSecurityItems);
                    //foreach (var generalSecurityItem in generalSecurityItems)
                    //{
                    //    foreach (var directSecurityItem in listDirectSecuritiesForPost)
                    //    {
                    //        directSecurityItem.Conditions.AddRange(generalSecurityItem.Conditions);
                    //    }
                    //}
                    //}
                    //listDirectSecuritiesForPost.AddRange(generalSecurityItems);
                    allPostsDirectSecurities.Add(new PostEntityDataSecurityItems(postDto, postDisrectSecurities));
                }
            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.ConditionalPermission, securitySubjectID.ToString(), entityID.ToString());
            return new EntityDataSecurityItems(indisrectSecurityDTO, allPostsDirectSecurities);
        }
        public IQueryable<OrganizationPost> GetDBOrganizationPosts(MyProjectEntities context, DR_Requester requester)
        {
            if (requester.PostIds.Any())
            {
                return context.OrganizationPost.Where(x => requester.PostIds.Contains(x.ID));
            }
            else
            {
                //BizOrganization bizOrganization = new BizOrganization();
                return context.OrganizationPost.Where(x => x.UserID == requester.Identity);
            }
        }

        private List<EntitySecurityDirectDTO> GetDirectSecurities(DR_Requester requester, IQueryable<EntitySecurityDirect> directSecurities, int entityID, int subjectID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityDirectSecurity, subjectID.ToString(), entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntitySecurityDirectDTO>);

            var subjectDisrectSecurities = directSecurities.Where(x => x.SecuritySubjectID == subjectID);// && x.Mode == (short)securityMode);
            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();
            foreach (var item in subjectDisrectSecurities)
            {
                result.Add(bizRoleSecurity.ToEntitySecurityDirectDTO(requester,item, true));
            }
            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityDirectSecurity, subjectID.ToString(), entityID.ToString());
            return result;

        }

        public List<EntitySecurityDirectDTO> GetGeneralEntitySecurityItems(DR_Requester requester, int entityID)
        {
            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityGeneralDirectSecurity, entityID.ToString());
            if (cachedItem != null)
                return (cachedItem as List<EntitySecurityDirectDTO>);
            BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();
            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            using (var context = new MyProjectEntities())
            {
                var disrectSecurities = context.EntitySecurityDirect.Where(x => x.TableDrivedEntityID == entityID);// && x.Mode == (short)securityMode);

                var subjectDisrectSecurities = disrectSecurities.Where(x => x.SecuritySubjectID == null);
                foreach (var item in subjectDisrectSecurities)
                {
                    result.Add(bizRoleSecurity.ToEntitySecurityDirectDTO(requester,item, true));
                }
            }
            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityGeneralDirectSecurity, entityID.ToString());
            return result;
        }
    }
    public class EntityDataSecurityItems
    {
        public EntityDataSecurityItems(EntitySecurityInDirectDTO indisrectSecurityDTO, List<PostEntityDataSecurityItems> allPostsDirectSecurities)
        {
            InDirectDataSecurity = indisrectSecurityDTO;
            PostEntityDataSecurityItems = allPostsDirectSecurities;
        }

        public EntitySecurityInDirectDTO InDirectDataSecurity { set; get; }
        public List<PostEntityDataSecurityItems> PostEntityDataSecurityItems { set; get; }
    }
    public class PostEntityDataSecurityItems
    {
        public PostEntityDataSecurityItems(OrganizationPostDTO postDto, List<EntitySecurityDirectDTO> postDisrectSecurities)
        {
            OrganizationPost = postDto;
            DirectSecurities = postDisrectSecurities;
        }

        public OrganizationPostDTO OrganizationPost { set; get; }
        public List<EntitySecurityDirectDTO> DirectSecurities { set; get; }
    }
}