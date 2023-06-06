﻿using DataAccess;
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

        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
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

        BizEntityState bizEntityState = new BizEntityState();
        //public FinalEntitySecurityDirects GetFinalEntitySecurityDirects(DR_Requester requester, int entityID, DataDirectSecurityFinalMode mode, bool withDetails)
        //{
        //    FinalEntitySecurityDirects result = new FinalEntitySecurityDirects();
        //    using (var context = new MyIdeaEntities())
        //    {
        //        IQueryable<EntitySecurityDirect> directs = context.EntitySecurityDirect.Where(x => x.TableDrivedEntityID == entityID);
        //        if (mode == DataDirectSecurityFinalMode.FetchData)
        //            directs = directs.Where(x => x.Mode == (short)DataDirectSecurityMode.FetchData);
        //        else if (mode == DataDirectSecurityFinalMode.ReadonlyData)
        //            directs = directs.Where(x => x.Mode == (short)DataDirectSecurityMode.ReadonlyData);
        //        if (directs.Any())
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //}

        public bool EntityHasDirectSecurities(DR_Requester requester, int entityID, DataDirectSecurityMode mode)
        {
            using (var context = new MyIdeaEntities())
            {
                return context.EntitySecurityDirect.Any(x => x.TableDrivedEntityID == entityID && x.Mode == (short)mode);
            }
        }
        public bool EntityHasDirectSecurities(DR_Requester requester, int entityID)
        {
            using (var context = new MyIdeaEntities())
            {
                return context.EntitySecurityDirect.Any(x => x.TableDrivedEntityID == entityID);
            }
        }
        //public bool EntityHasInDirectSecurityWithDirectSecurity(DR_Requester requester, int entityID, DataDirectSecurityMode mode)
        //{
        //    using (var context = new MyIdeaEntities())
        //    {
        //        if (mode == DataDirectSecurityMode.FetchData)
        //        {
        //            if (context.EntitySecurityInDirect.Any(x => x.TableDrivedEntityID == entityID &&
        //            (x.Mode == (short)DataInDirectSecurityMode.OnlyFetchData || x.Mode == (short)DataInDirectSecurityMode.Full)))
        //            {
        //                var indirect = context.EntitySecurityInDirect.First(x => x.TableDrivedEntityID == entityID &&
        //            (x.Mode == (short)DataInDirectSecurityMode.OnlyFetchData || x.Mode == (short)DataInDirectSecurityMode.Full));

        //                return context.EntitySecurityDirect.Any(x => x.TableDrivedEntityID == indirect.EntityRelationshipTail.TargetEntityID && x.Mode == (short)mode);
        //            }
        //            else
        //                return false;
        //        }
        //        else if (mode == DataDirectSecurityMode.ReadonlyData)
        //        {
        //            if (context.EntitySecurityInDirect.Any(x => x.TableDrivedEntityID == entityID && x.Mode == (short)DataInDirectSecurityMode.Full))
        //            {
        //                var indirect = context.EntitySecurityInDirect.First(x => x.TableDrivedEntityID == entityID && x.Mode == (short)DataInDirectSecurityMode.Full);
        //                return context.EntitySecurityDirect.Any(x => x.TableDrivedEntityID == indirect.EntityRelationshipTail.TargetEntityID && x.Mode == (short)mode);
        //            }
        //            else
        //                return false;
        //        }
        //    }
        //    return false;
        //}
        //public bool EntityHasInDirectSecurities(DR_Requester requester, int entityID)
        //{
        //    using (var context = new MyIdeaEntities())
        //    {
        //        if (context.EntitySecurityInDirect.Any(x => x.TableDrivedEntityID == entityID))
        //        {
        //            var indirect = context.EntitySecurityInDirect.First(x => x.TableDrivedEntityID == entityID);

        //            return context.EntitySecurityDirect.Any(x => x.TableDrivedEntityID == indirect.EntityRelationshipTail.TargetEntityID);
        //        }
        //        else
        //            return false;
        //    }
        //}

        public EntitySecurityDirectDTO GetEntitySecurityDirectByEntityID(DR_Requester requester, int entityID, DataDirectSecurityMode mode, bool withDetails)
        {
            EntitySecurityDirectDTO result = new EntitySecurityDirectDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var item = projectContext.EntitySecurityDirect.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.Mode == (short)mode);
                if (item != null)
                    return ToEntitySecurityDirectDTO(requester, item, withDetails);
                else
                    return null;

            }
        }

        public bool DeleteEntitySecurityDirect(int id)
        {
            EntitySecurityDirectDTO result = new EntitySecurityDirectDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var item = projectContext.EntitySecurityDirect.FirstOrDefault(x => x.ID == id);
                //foreach (var child in item.EntitySecurityDirectStates.ToList())
                //    projectContext.EntitySecurityDirectStates.Remove(child);
                projectContext.EntitySecurityDirect.Remove(item);
                projectContext.SaveChanges();
                return true;
            }
        }
        public List<EntitySecurityDirectDTO> GetEntitySecurityDirects(DR_Requester requester, bool withDetails, string search)
        {
            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var list = projectContext.EntitySecurityDirect;
                foreach (var item in list)
                {
                    result.Add(ToEntitySecurityDirectDTO(requester, item, withDetails));
                }
            }
            return result;
        }

        public List<EntitySecurityDirectDTO> GetEntitySecurityDirects(DR_Requester requester, int entityID, bool withDetails)
        {
            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
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
            //if (item.SecuritySubjectID != null)
            //    result.SecuritySubjectID = item.SecuritySubjectID.Value;
            //else
            //    result.SecuritySubjectID = 0;
            if (item.Mode != null)
                result.Mode = (DataDirectSecurityMode)item.Mode;
            //result.IgnoreSecurity = item.IgnoreSecurity;
            result.Description = item.Description;
            result.TableDrivedEntityID = item.TableDrivedEntityID;
            //if (item.SecuritySubjectOperator != null)
            //    result.SecuritySubjectInORNotIn = (InORNotIn)item.SecuritySubjectOperator;
            //foreach (var valItem in item.EntitySecurityDirectSecuritySubject)
            //{
            //    result.SecuritySubjects.Add(new ChildSecuritySubjectDTO { SecuritySubjectID = valItem.SecuritySubjectID });//, SecuritySubjectOperator = (Enum_SecuritySubjectOperator)valItem.SecuritySubjectOperator });
            //}

            //   EntitySecurityDirectStatesDTO securityState = new EntitySecurityDirectStatesDTO();
            result.EntityStateID = item.TableDrivedEntityStateID;
            if (withDetails && result.EntityStateID != 0)
            {
                BizEntityState bizEntityState = new BizEntityState();
                result.EntityState = bizEntityState.ToEntityStateDTO(requester, item.EntityState, withDetails);
            }

            //foreach (var valItem in item.EntitySecurityDirectValues)
            //{
            //    result.Values.Add(new ModelEntites.EntityStateValueDTO() { Value = valItem.Value, SecurityReservedValue = valItem.ReservedValue == null ? SecurityReservedValue.None : (SecurityReservedValue)valItem.ReservedValue });
            //}

            //result.FormulaID = item.FormulaID ?? 0;
            //if (result.FormulaID != 0 && withDetails)
            //{  //??با جزئیات؟؟........................................................................ 
            //    var bizFormula = new BizFormula();
            //    result.Formula = bizFormula.GetFormula(requester, item.FormulaID.Value, withDetails);
            //}
            //result.ColumnID = item.ColumnID ?? 0;
            //if (item.Column != null)
            //{
            //    BizColumn bizColumn = new BizColumn();
            //    result.Column = bizColumn.ToColumnDTO(item.Column, true);

            //}
            //result.RelationshipTailID = item.EntityRelationshipTailID ?? 0;
            //if (item.EntityRelationshipTail != null)
            //{
            //    BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
            //    result.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
            //}
            //if (item.ValueOperator != null)
            //    result.ValueOperator = (Enum_EntityStateOperator)item.ValueOperator;

            //result.EntityStates.Add(securityState);

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

        //public EntitySecurityInDirectDTO GetEntitySecurityInDirect(DR_Requester requester, int entityID, bool withDetails)
        //{
        //    EntitySecurityInDirectDTO result = new EntitySecurityInDirectDTO();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var item = projectContext.EntitySecurityInDirect.FirstOrDefault(x => x.TableDrivedEntityID == entityID);
        //        if (item != null)
        //            return ToEntitySecurityInDirectDTO(item, withDetails);
        //        else
        //            return null;

        //    }
        //}
        //public EntitySecurityInDirectDTO GetEntitySecurityInDirect(DR_Requester requester, int entityID, bool withDetails)
        //{
        //    EntitySecurityInDirectDTO result = new EntitySecurityInDirectDTO();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var item = projectContext.EntitySecurityInDirect.FirstOrDefault(x => x.TableDrivedEntityID == entityID);
        //        if (item != null)
        //            return ToEntitySecurityInDirectDTO(item, withDetails);
        //        else
        //            return null;
        //    }
        //}

        //public EntitySecurityInDirectDTO ToEntitySecurityInDirectDTO(EntitySecurityInDirect item, bool withDetails)
        //{
        //    EntitySecurityInDirectDTO result = new EntitySecurityInDirectDTO();
        //    result.ID = item.ID;
        //    //result.DirectRoleSecurityID = item.EntitySecurityDirectID;
        //    result.RelationshipTailID = item.EntityRelationshipTailID;
        //    result.TableDrivedEntityID = item.TableDrivedEntityID;
        //    if (item.Mode != null)
        //        result.Mode = (DataInDirectSecurityMode)item.Mode;

        //    if (withDetails)
        //    {
        //        BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
        //        result.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
        //        //result.DirectRoleSecurity = ToEntitySecurityDirectDTO(item.EntitySecurityDirect, withDetails);
        //    }

        //    return result;
        //}

        public int UpdateEntitySecurityDirect(EntitySecurityDirectDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbItem = projectContext.EntitySecurityDirect.FirstOrDefault(x => x.ID == message.TableDrivedEntityID && x.Mode == (short)message.Mode);
                if (dbItem == null)
                {
                    dbItem = new DataAccess.EntitySecurityDirect();
                    projectContext.EntitySecurityDirect.Add(dbItem);
                }
                dbItem.TableDrivedEntityID = message.TableDrivedEntityID;
                //if (message.SecuritySubjectID != 0)
                //    dbItem.SecuritySubjectID = message.SecuritySubjectID;
                //else
                //    dbItem.SecuritySubjectID = null;


                //dbItem.SecuritySubjectOperator = (short)message.SecuritySubjectInORNotIn;

                //while (dbItem.EntitySecurityDirectSecuritySubject.Any())
                //    projectContext.EntitySecurityDirectSecuritySubject.Remove(dbItem.EntitySecurityDirectSecuritySubject.First());
                //foreach (var nItem in message.SecuritySubjects)
                //{
                //    dbItem.EntitySecurityDirectSecuritySubject.Add(new EntitySecurityDirectSecuritySubject() { SecuritySubjectID = nItem.SecuritySubjectID });//, SecuritySubjectOperator = (short)nItem.SecuritySubjectOperator });
                //}
                dbItem.Description = message.Description;
                dbItem.Mode = (short)message.Mode;
                // dbItem.IgnoreSecurity = message.IgnoreSecurity;
                //dbItem.ConditionsAndOrType = (short)message.ConditionAndORType;
                dbItem.TableDrivedEntityStateID = message.EntityStateID;


                //if (message.FormulaID != 0)
                //    dbItem.FormulaID = message.FormulaID;
                //else
                //    dbItem.FormulaID = null;
                //if (message.ColumnID != 0)
                //{
                //    dbItem.ColumnID = message.ColumnID;
                //    if (message.RelationshipTailID == 0)
                //        dbItem.EntityRelationshipTailID = null;
                //    else
                //        dbItem.EntityRelationshipTailID = message.RelationshipTailID;
                //}
                //else
                //{
                //    dbItem.ColumnID = null;
                //    dbItem.EntityRelationshipTailID = null;
                //}
                //dbItem.ValueOperator = (short)message.ValueOperator;

                //while (dbItem.EntitySecurityDirectValues.Any())
                //    projectContext.EntitySecurityDirectValues.Remove(dbItem.EntitySecurityDirectValues.First());
                //foreach (var nItem in message.Values)
                //{
                //    dbItem.EntitySecurityDirectValues.Add(new EntitySecurityDirectValues() { Value = nItem.Value, ReservedValue = (short)nItem.SecurityReservedValue });
                //}

                //while (dbItem.EntitySecurityDirectStates.Any())
                //    projectContext.EntitySecurityDirectStates.Remove(dbItem.EntitySecurityDirectStates.First());
                //foreach (var securityState in message.EntityStates)
                //{
                //    var dbCondition = new EntitySecurityDirectStates();
                //    dbCondition.TableDrivedEntityStateID = securityState.EntityStateID;
                //    dbItem.EntitySecurityDirectStates.Add(dbCondition);
                //}

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
        //public void UpdateEntitySecurityInDirect(EntitySecurityInDirectDTO message)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbItem = projectContext.EntitySecurityInDirect.FirstOrDefault(x => x.ID == message.ID);
        //        if (dbItem == null)
        //        {
        //            dbItem = new DataAccess.EntitySecurityInDirect();
        //            projectContext.EntitySecurityInDirect.Add(dbItem);
        //        }
        //        dbItem.Mode = (short)message.Mode;
        //        dbItem.TableDrivedEntityID = message.TableDrivedEntityID;
        //        dbItem.EntityRelationshipTailID = message.RelationshipTailID;
        //        projectContext.SaveChanges();
        //    }
        //}
        public EntityStateDTO GetAppliableConditionsBySecuritySubject(DR_Requester requester, int entityID, DataDirectSecurityMode mode)
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
          //  List<EntityStateConditionDTO> entityStateConditions = new List<EntityStateConditionDTO>();
            //  EntitySecurityInDirectDTO indisrectSecurityDTO = null;
            //   EntityStateDTO entityState = null;
            using (var context = new MyIdeaEntities())
            {
                var directSecurityEntityID = entityID;
                var targetEntityDisrectSecurity = GetEntitySecurityDirectByEntityID(requester, entityID, mode, true);
                if (targetEntityDisrectSecurity == null)
                {
                    //var indisrectSecurity = GetEntitySecurityInDirect(context.EntitySecurityInDirect.FirstOrDefault(x => x.TableDrivedEntityID == entityID);
                    //if (indisrectSecurity == null)
                    //    return null;
                    //else
                    //{
                    //    indisrectSecurityDTO = bizRoleSecurity.ToEntitySecurityInDirectDTO(indisrectSecurity, true);
                    //    var targetEntity = indisrectSecurity.EntityRelationshipTail.TableDrivedEntity;
                    //    directSecurityEntityID = targetEntity.ID;
                    //    targetEntityDisrectSecurity = context.EntitySecurityDirect.FirstOrDefault(x => x.TableDrivedEntityID == targetEntity.ID && x.Mode == (short)DataDirectSecurityMode.FetchData);
                    //}
                    return null;
                }


                //   entityState = bizEntityState.ToEntityStateDTO(requester, targetEntityDisrectSecurity.TableDrivedEntityState, true);
              //  var condition = targetEntityDisrectSecurity.EntityState.StateCondition;

                if (ConditionSecuritySubjectIsValid(requester, targetEntityDisrectSecurity.EntityState))
                {
                     GetConditionDTOWithValues(requester, targetEntityDisrectSecurity.EntityState);
                }
                else
                {
                    targetEntityDisrectSecurity.EntityState.Values = null;
                  //  targetEntityDisrectSecurity.EntityState.StateCondition = null;
                }


                return targetEntityDisrectSecurity.EntityState;
                //var organizationPosts = GetDBOrganizationPosts(context, requester);
                //BizOrganization bizOrganization = new BizOrganization();
                //foreach (var post in organizationPosts)
                //{
                //    //  List<EntitySecurityDirectDTO> listDirectSecuritiesForPost = new List<EntitySecurityDirectDTO>();
                //    var postDto = requester.Posts.FirstOrDefault(x => x.ID == post.ID);
                //    if (postDto == null)
                //        postDto = bizOrganization.GetOrganizationPost(post.ID);
                //    var postDisrectSecurities = GetDirectSecurities(requester, postDto, targetEntityDisrectSecurities, directSecurityEntityID);
                //postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.Organization.SecuritySubject.ID));
                //postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.SecuritySubject.ID));
                //postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.OrganizationType.SecuritySubject.ID));
                //postDisrectSecurities.AddRange(GetDirectSecurities(requester, disrectSecurities, directSecurityEntityID, post.OrganizationType_RoleType.RoleType.SecuritySubject.ID));


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
                //var generalSecurityItems = GetGeneralEntitySecurityItems(requester, directSecurityEntityID);
                //if (generalSecurityItems.Any())
                //    postDisrectSecurities.AddRange(generalSecurityItems);
                //foreach (var generalSecurityItem in generalSecurityItems)
                //{
                //    foreach (var directSecurityItem in listDirectSecuritiesForPost)
                //    {
                //        directSecurityItem.Conditions.AddRange(generalSecurityItem.Conditions);
                //    }
                //}
                //}
                //listDirectSecuritiesForPost.AddRange(generalSecurityItems);
                //    allPostsDirectSecurities.Add(new PostEntityDataSecurityItems(postDto, postDisrectSecurities));
                //}
            }
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.ConditionalPermission, securitySubjectID.ToString(), entityID.ToString());
            //   return new EntityDataSecurityItems(entityState, indisrectSecurityDTO, entityStateConditions);
        }

        private void GetConditionDTOWithValues(DR_Requester requester, EntityStateDTO conditionDTO)
        {
            if (conditionDTO.Values.Any(x => x.SecurityReservedValue != SecurityReservedValue.None))
            {
                if (conditionDTO.Values.Any(x => x.SecurityReservedValue != SecurityReservedValue.None))
                {
                    List<string> addedValues = new List<string>();
                    foreach (var value in conditionDTO.Values.Where(x => x.SecurityReservedValue != SecurityReservedValue.None))
                    {
                        foreach (var post in requester.Posts)
                        {
                            var exactValue = GerReserveValueFromPost(post, value.SecurityReservedValue);
                            if (!addedValues.Any(x => x == exactValue))
                                addedValues.Add(exactValue);
                        }
                    }
                    foreach (var value in conditionDTO.Values.Where(x => x.SecurityReservedValue != SecurityReservedValue.None).ToList())
                        conditionDTO.Values.Remove(value);
                    foreach (var value in addedValues)
                    {
                        if (!conditionDTO.Values.Any(x => x.Value == value))
                            conditionDTO.Values.Add(new EntityStateValueDTO() { Value = value });
                    }
                }
            }
          //  return conditionDTO;
        }


        private string GerReserveValueFromPost(OrganizationPostDTO post, SecurityReservedValue reservedValue)
        {
            if (reservedValue == SecurityReservedValue.OrganizationID)
                return post.OrganizationID.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationPostID)
                return post.ID.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeID)
                return post.OrganizationTypeID.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeRoleTypeID)
                return post.OrganizationTypeRoleTypeID.ToString();
            else if (reservedValue == SecurityReservedValue.RoleTypeID)
                return post.RoleTypeID.ToString();
            else if (reservedValue == SecurityReservedValue.UserID)
                return post.CurrentUserID.ToString();

            else if (reservedValue == SecurityReservedValue.OrganizationExternalKey)
                return post.OrganizationExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationPostExternalKey)
                return post.ExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeExternalKey)
                return post.OrganizationTypeExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.OrganizationTypeRoleTypeExternalKey)
                return post.OrganizationTypeRoleTypeExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.RoleTypeExternalKey)
                return post.RoleTypeExternalKey.ToString();
            else if (reservedValue == SecurityReservedValue.UserExternalKey)
                return post.CurrentUserExternalKey.ToString();
            return "";
        }

        private bool ConditionSecuritySubjectIsValid(DR_Requester requester, EntityStateDTO condition)
        {

            if (condition.SecuritySubjects.Any())
            {
                bool hasAnyOfSubjects = false;
                foreach (var subject in condition.SecuritySubjects)
                {
                    if (requester.Posts.Any(x => x.CurrentUserID == subject)
                        || requester.Posts.Any(x => x.ID == subject)
                         || requester.Posts.Any(x => x.OrganizationID == subject)
                          || requester.Posts.Any(x => x.OrganizationTypeID == subject)
                           || requester.Posts.Any(x => x.OrganizationTypeRoleTypeID == subject)
                            || requester.Posts.Any(x => x.RoleTypeID == subject)
                            )
                        hasAnyOfSubjects = true;
                }

                if (condition.SecuritySubjectInORNotIn == InORNotIn.In)
                {
                    return hasAnyOfSubjects;
                }
                else
                {
                    return !hasAnyOfSubjects;
                }
            }
            else
                return true;
        }

        public IQueryable<OrganizationPost> GetDBOrganizationPosts(MyIdeaEntities context, DR_Requester requester)
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

        private List<EntitySecurityDirectDTO> GetDirectSecurities(DR_Requester requester, OrganizationPostDTO post, IQueryable<EntitySecurityDirect> directSecurities, int entityID)
        {
            //var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityDirectSecurity, subjectID.ToString(), entityID.ToString());
            //if (cachedItem != null)
            //    return (cachedItem as List<EntitySecurityDirectDTO>);
            BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();

            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            //////foreach (var directSecurity in directSecurities)
            //////{
            //////    bool hasAnyOfSubjects = false;
            //////    foreach (var subject in directSecurity.TableDrivedEntityState.)
            //////    {
            //////        if (post.CurrentUserID == subject.SecuritySubjectID
            //////            || post.ID == subject.SecuritySubjectID
            //////             || post.OrganizationID == subject.SecuritySubjectID
            //////              || post.OrganizationTypeID == subject.SecuritySubjectID
            //////               || post.OrganizationTypeRoleTypeID == subject.SecuritySubjectID
            //////                || post.RoleTypeID == subject.SecuritySubjectID
            //////                )
            //////            hasAnyOfSubjects = true;
            //////    }

            //////    if (directSecurity.SecuritySubjectOperator == null || (InORNotIn)directSecurity.SecuritySubjectOperator == InORNotIn.In)
            //////    {
            //////        if (hasAnyOfSubjects == true)
            //////            result.Add(bizRoleSecurity.ToEntitySecurityDirectDTO(requester, directSecurity, true));
            //////    }
            //////    else
            //////    {
            //////        if (hasAnyOfSubjects == false)
            //////            result.Add(bizRoleSecurity.ToEntitySecurityDirectDTO(requester, directSecurity, true));
            //////    }
            //////}
            ////////var subjectDisrectSecurities = directSecurities.Where(x => x.SecuritySubjectID == subjectID);// && x.Mode == (short)securityMode);
            ////////List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            ////////foreach (var item in subjectDisrectSecurities)
            ////////{
            ////////    result.Add(bizRoleSecurity.ToEntitySecurityDirectDTO(requester, item, true));
            ////////}
            ////////     CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityDirectSecurity, subjectID.ToString(), entityID.ToString());
            return result;

        }

        public List<EntitySecurityDirectDTO> GetGeneralEntitySecurityItems(DR_Requester requester, int entityID)
        {
            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityGeneralDirectSecurity, entityID.ToString());
            //   if (cachedItem != null)
            //       return (cachedItem as List<EntitySecurityDirectDTO>);
            BizRoleSecurity bizRoleSecurity = new BizRoleSecurity();
            List<EntitySecurityDirectDTO> result = new List<EntitySecurityDirectDTO>();
            //////using (var context = new MyIdeaEntities())
            //////{
            //////    var disrectSecurities = context.EntitySecurityDirect.Where(x => x.TableDrivedEntityID == entityID && x.Mode == (short)DataDirectSecurityMode.FetchData);

            //////    var subjectDisrectSecurities = disrectSecurities.Where(x => !x.EntitySecurityDirectSecuritySubject.Any());
            //////    foreach (var item in subjectDisrectSecurities)
            //////    {
            //////        result.Add(bizRoleSecurity.ToEntitySecurityDirectDTO(requester, item, true));
            //////    }
            //////}
            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityGeneralDirectSecurity, entityID.ToString());
            return result;
        }
        //public bool EntityHasDirectSecurityForDirectOrIndirect(DR_Requester requester, int entityID, DataDirectSecurityMode mode)
        //{
        //    return EntityHasDirectSecurities(requester, entityID, mode)
        //          || EntityHasInDirectSecurityWithDirectSecurity(requester, entityID, mode);
        //}


    }
    //public class EntityDataSecurityItems
    //{
    //    public EntityDataSecurityItems(EntityStateDTO state)
    //    {
    //        //      InDirectDataSecurity = indisrectSecurityDTO;
    //        EntityStateConditions = entityStateConditions;
    //        EntityState = state;
    //    }
    //    public EntityStateDTO EntityState { set; get; }
    //    public EntitySecurityInDirectDTO InDirectDataSecurity { set; get; }
    //    public List<EntityStateConditionDTO> EntityStateConditions { set; get; }
    //}
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