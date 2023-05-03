using CommonDefinitions.UISettings;
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
    public class BizEntityUIComposition
    {

        //** 0a8a4656-9ddf-4557-9823-9cbf2509d6f8
        public event EventHandler<ItemImportingStartedArg> ItemImportingStarted;
        //public EntityUICompositionDTO GetEntityUICompositionTree(int entityID)
        //{
        //    //////var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityUICompositionTree, entityID.ToString());
        //    //////if (cachedItem != null)
        //    //////    return (cachedItem as List<EntityUICompositionDTO>);
        //    //  List<EntityUICompositionDTO> result = new List<EntityUICompositionDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var rootdb = projectContext.EntityUIComposition.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.ParentID == null);
        //        if (rootdb != null)
        //        {
        //            var result = ToEntityUICompositionDTO(rootdb, true);
        //            return result;
        //        }
        //        else
        //            return null;
        //    }
        //    //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityUICompositionTree, entityID.ToString());

        //}
        public EntityUICompositionDTO GetOrCreateEntityUIComposition(int entityID)
        {
            // BizEntityUIComposition.GetOrCreateEntityUIComposition: 33f45647385a
            BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            DR_Requester requester = new DR_Requester();
            requester.SkipSecurity = true;
            var entity = bizTableDrivedEntity.GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships);
            return GetOrCreateEntityUIComposition(entity.ID, entity.Columns.Where(x => x.DataEntryEnabled).ToList(), entity.Relationships.Where(x => x.DataEntryEnabled).ToList());
        }
        public EntityUICompositionDTO GetOrCreateEntityUIComposition(int entityID, List<ColumnDTO> columns, List<RelationshipDTO> relationships)
        {
            //** BizEntityUIComposition.GetOrCreateEntityUIComposition: fdb08b6bb258

            //اینجا نیازی به requester
            // نیست چون ستونها و روابط از بیرون میان و چک شده هستن

            // EntityUICompositionCompositeDTO result = new EntityUICompositionCompositeDTO();
            //////var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.EntityUICompositionTree, entityID.ToString());
            //////if (cachedItem != null)
            //////    return (cachedItem as List<EntityUICompositionDTO>);

            //  result.ColumnItems = new List<ColumnUISettingDTO>();
            // result.RelationshipItems = new List<RelationshipUISettingDTO>();
            EntityUICompositionDTO result = null;
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbroot = projectContext.EntityUIComposition.FirstOrDefault(x => x.TableDrivedEntityID == entityID && x.ParentID == null);
                //foreach (var item in list)
                //{
                if (dbroot != null)
                {
                    //** 11de07a7-43cf-4465-ad1d-a5e3914fd5f5
                    result = ToEntityUICompositionDTO(dbroot, columns, relationships);
                    var columnsAndRelationships = GetColumnsAndRelationships(result);
                    var notInUICompositionColumns = columns.Where(c => !columnsAndRelationships.Any(x => x.ObjectCategory == DatabaseObjectCategory.Column &&
                         c.ID == Convert.ToInt32(x.ObjectIdentity))).ToList();
                    var notInUICompositionRelationships = relationships.Where(c => !columnsAndRelationships.Any(x => x.ObjectCategory == DatabaseObjectCategory.Relationship &&
                       c.ID == Convert.ToInt32(x.ObjectIdentity))).ToList();
                    var notInUIrelationshipUIs = notInUICompositionRelationships.ToList();

                    AddColumnsAndRelationships(result, notInUICompositionColumns, notInUIrelationshipUIs);
                }
                else
                    result = GenerateUIComposition(entityID, columns, relationships);
                //}
            }
            return result;
            //CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.EntityUICompositionTree, entityID.ToString());
            //return null;
        }
        private List<EntityUICompositionDTO> GetColumnsAndRelationships(EntityUICompositionDTO currentUIComposition, List<EntityUICompositionDTO> result = null)
        {
            if (result == null)
                result = new List<EntityUICompositionDTO>();
            if (currentUIComposition.ObjectCategory == DatabaseObjectCategory.Column)
            {
                result.Add(currentUIComposition);

            }
            else if (currentUIComposition.ObjectCategory == DatabaseObjectCategory.Relationship)
            {
                result.Add(currentUIComposition);
            }
            foreach (var cItem in currentUIComposition.ChildItems)
            {
                GetColumnsAndRelationships(cItem, result);
            }
            return result;
        }
        private EntityUICompositionDTO GenerateUIComposition(int entityID, List<ColumnDTO> columns, List<RelationshipDTO> relationships)
        {
            //** a301db67-64d0-4a8b-85cc-ed81338a7008
            //    List<EntityUICompositionDTO> result = new List<EntityUICompositionDTO>();
            var entityUiComposition = new EntityUICompositionDTO();
            entityUiComposition.ObjectCategory = DatabaseObjectCategory.Entity;
            entityUiComposition.EntityUISetting = new EntityUISettingDTO() { UIColumnsCount = 4 };
            entityUiComposition.ObjectIdentity = entityID.ToString();
            //  result.Add(entityUiComposition);

            AddColumnsAndRelationships(entityUiComposition, columns, relationships);
            return entityUiComposition;
        }

        public void AddColumnsAndRelationships(EntityUICompositionDTO entityUiComposition, List<ColumnDTO> columns, List<RelationshipDTO> relationships)
        {
            //** ddb369e2-26cc-4d8a-995c-52b9e6b626d0
            foreach (var relationshipUI in relationships)
            {
                if (relationshipUI.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
                {
                    FKRelationshipHasReservedPosition(relationships, relationshipUI);
                }

            }
            AddColumnsAndFKPositionedRelationships(entityUiComposition, columns, relationships.Where(x => x.FKReservedPosition == true).ToList());
            var otherRelationshipsGroups = GetOtherRelationshipsGroups(relationships.Where(x => x.FKReservedPosition != true).ToList());
            AddOtherRelationshipUICompositionItem(entityUiComposition, otherRelationshipsGroups);
        }



        private void FKRelationshipHasReservedPosition(List<RelationshipDTO> relationships, RelationshipDTO relationshipUI)
        {
            //** 1cf96481-17c4-4d0a-96e1-7387bc372bcb
            if (relationshipUI.FKReservedPosition == null)
            {
                if (relationshipUI.TypeEnum == Enum_RelationshipType.UnionToSubUnion)
                {
                    if (relationshipUI.IsOtherSideDirectlyCreatable)
                    {
                        relationshipUI.FKReservedPosition = false;
                        foreach (var otherRelationshipUIs in relationships.Where(x => x.TypeEnum == Enum_RelationshipType.UnionToSubUnion && x.ID != relationshipUI.ID))
                        {
                            if ((otherRelationshipUIs as UnionToSubUnionRelationshipDTO).UnionRelationship.ID == (relationshipUI as UnionToSubUnionRelationshipDTO).UnionRelationship.ID)
                            {

                                otherRelationshipUIs.FKReservedPosition = false;
                            }
                        }
                    }
                    else
                    {
                        //بالا اگر ست بشه چون دیگه نال نیست اینجا نمیاد
                        relationshipUI.FKReservedPosition = true;
                    }
                }
                else if (relationshipUI.TypeEnum == Enum_RelationshipType.SubToSuper)
                {
                    relationshipUI.FKReservedPosition = true;
                }
                else
                {
                    relationshipUI.FKReservedPosition = !relationshipUI.IsOtherSideDirectlyCreatable;
                }
            }

        }
        private void AddColumnsAndFKPositionedRelationships(EntityUICompositionDTO parentItem, List<ColumnDTO> columns, List<RelationshipDTO> relatinoshipUIs)
        {
            List<RelationshipDTO> listFRelationships = new List<RelationshipDTO>();
            var index = GetNextIndex(parentItem);
            foreach (var item in columns.OrderBy(x => x.Position))
            {
                if (!listFRelationships.Any(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == item.ID)))
                {
                    if (relatinoshipUIs.Any(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == item.ID)))
                    {
                        var relatinoshipUI = relatinoshipUIs.First(x => x.RelationshipColumns.Any(y => y.FirstSideColumnID == item.ID));
                        listFRelationships.Add(relatinoshipUI);
                        AddRelationshipUICompositionItem(parentItem, relatinoshipUI, index, relatinoshipUI.IsOtherSideDirectlyCreatable);
                        index++;
                    }
                    else
                    {
                        AddColumnUICompositionItem(parentItem, item, index);
                        index++;
                    }
                }
            }


            ////برای مثال روابطی که ستونهاش از قبل تعیین ظاهر شدن اما بعدا ستون تبدیل به رابطه شده بنابراین وقتی از 
            ////8400bf54-fed4-4226-8e61-0f2b5dd22bf3
            ////می آید ستون غایب است
            //foreach (var relatinoshipUI in relatinoshipUIs.Where(x => !listFRelationships.Contains(x)))
            //{
            //    AddRelationshipUICompositionItem(parentItem, relatinoshipUI, index, relatinoshipUI.IsOtherSideDirectlyCreatable);
            //    index++;
            //}

        }
        private List<Tuple<int, string, List<RelationshipDTO>>> GetOtherRelationshipsGroups(List<RelationshipDTO> relationships)
        {
            //** 1ae24738-9276-424d-922e-5d69f05cfdd5
            List<Tuple<int, string, List<RelationshipDTO>>> result = new List<Tuple<int, string, List<RelationshipDTO>>>();
            foreach (var relationship in relationships)
            {
                if (!result.Any(x => x.Item3.Any(y => y == relationship)))
                {
                    int id = 0;
                    string title = "";
                    List<RelationshipDTO> group = new List<RelationshipDTO>();
                    if (relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion)
                    {
                        title = (relationship as UnionToSubUnionRelationshipDTO).UnionRelationship.Name;
                        id = (relationship as UnionToSubUnionRelationshipDTO).UnionRelationship.ID;
                        foreach (var inGroupRelationshipUI in relationships.Where(x => x.TypeEnum == Enum_RelationshipType.UnionToSubUnion))
                        {
                            if ((inGroupRelationshipUI as UnionToSubUnionRelationshipDTO).UnionRelationship.ID == (relationship as UnionToSubUnionRelationshipDTO).UnionRelationship.ID)
                            {
                                group.Add(inGroupRelationshipUI);

                            }
                        }
                    }
                    else if (relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
                    {
                        title = (relationship as SuperToSubRelationshipDTO).ISARelationship.Name;
                        id = (relationship as SuperToSubRelationshipDTO).ISARelationship.ID;
                        foreach (var inGroupRelationshipUI in relationships.Where(x => x.TypeEnum == Enum_RelationshipType.SuperToSub))
                        {
                            if ((inGroupRelationshipUI as SuperToSubRelationshipDTO).ISARelationship.ID == (relationship as SuperToSubRelationshipDTO).ISARelationship.ID)
                            {
                                group.Add(inGroupRelationshipUI);

                            }
                        }
                    }
                    else
                    {
                        id = relationship.ID;
                        title = relationship.ID.ToString();
                        group.Add(relationship);
                    }
                    result.Add(new Tuple<int, string, List<RelationshipDTO>>(id, title, group));
                }
                else
                {
                }
            }
            return result;
        }



        private int GetNextIndex(EntityUICompositionDTO parentItem)
        {
            if (parentItem.ChildItems.Any())
                return parentItem.ChildItems.Max(x => x.Position) + 1;
            else
                return 0;
        }

        private void AddOtherRelationshipUICompositionItem(EntityUICompositionDTO parentItem, List<Tuple<int, string, List<RelationshipDTO>>> otherRelationshipsGroups)
        {
            //** 0a01c47e-c46f-414b-804c-ac47f85a7fd9
            var index = GetNextIndex(parentItem);
            foreach (var relationshipGroup in otherRelationshipsGroups.Where(x => !x.Item3.Any(y => y.IsOtherSideDirectlyCreatable)).OrderBy(x => x.Item2))
            {
                foreach (var relationshipUI in relationshipGroup.Item3)
                {
                    AddRelationshipUICompositionItem(parentItem, relationshipUI, index, false);
                    index++;
                }
            }
            if (otherRelationshipsGroups.Count(x => x.Item3.Any(y => y.IsOtherSideDirectlyCreatable)) > 1)
            {

                var tabConrol = GetOrCreateRelationshipTabControl(parentItem, index, "روابط", "TabControl-99");

                parentItem.ChildItems.Add(tabConrol);
              //  parentItem = tabPage;


                foreach (var relationshipGroup in otherRelationshipsGroups.Where(x => x.Item3.Any(y => y.IsOtherSideDirectlyCreatable)).OrderBy(x => x.Item2))
                {
                    if (relationshipGroup.Item3.Count() > 1)
                    {
                        var tabPageGroup = AddTabPage(tabConrol, relationshipGroup.Item2, "group_" + relationshipGroup.Item1);

                        var grouptabConrol = GetOrCreateRelationshipTabControl(tabPageGroup, 0, relationshipGroup.Item2, "group_" + relationshipGroup.Item1);
                        foreach (var relationshipUI in relationshipGroup.Item3)
                        {
                            var tabPage = AddTabPage(grouptabConrol, relationshipUI.Alias, "Rel_" + relationshipUI.ID);
                            AddRelationshipUICompositionItem(tabPage, relationshipUI, 0, false);

                        }
                    }
                    else
                    {
                        var tabPage = AddTabPage(tabConrol, relationshipGroup.Item2, "group_" + relationshipGroup.Item1);
                        AddRelationshipUICompositionItem(tabPage, relationshipGroup.Item3.First(), 0, false);
                    }
                }
            }
        }



        private void AddRelationshipUICompositionItem(EntityUICompositionDTO parentItem, RelationshipDTO relationship, int index, bool expander)
        {
            var childItem = new EntityUICompositionDTO();
            childItem.ObjectCategory = DatabaseObjectCategory.Relationship;
            childItem.ObjectIdentity = relationship.ID.ToString();
            childItem.Position = index;
            childItem.Title = relationship.Alias;
            childItem.ParentItem = parentItem;
            childItem.RelationshipUISetting = new RelationshipUISettingDTO();
            if (relationship.IsOtherSideDirectlyCreatable)
            {
                //if (relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
                //{
                if (expander)//parentItem.ObjectCategory == DatabaseObjectCategory.Entity)
                {
                    childItem.RelationshipUISetting.Expander = true;
                    childItem.RelationshipUISetting.IsExpanded = true;
                }
                childItem.RelationshipUISetting.UIColumnsType = Enum_UIColumnsType.Full;

                //}
                //else
                //{
                //if (relationship.TypeEnum == Enum_RelationshipType.OneToMany)
                //{
                //    AddTabPageUICompositionItem(result[0], result, relationship, index)
                //}
                //}
            }
            else
            {
                childItem.RelationshipUISetting.UIColumnsType = Enum_UIColumnsType.Half;
            }
            parentItem.ChildItems.Add(childItem);
        }

        private void AddColumnUICompositionItem(EntityUICompositionDTO parentItem, ColumnDTO column, int index)
        {
            //** BizEntityUIComposition.AddColumnUICompositionItem: ad8db1ea-07d5-481a-95fd-5c93e6562e87
            var childItem = new EntityUICompositionDTO();
            childItem.ObjectCategory = DatabaseObjectCategory.Column;
            childItem.ObjectIdentity = column.ID.ToString();
            childItem.Position = index;
            childItem.Title = column.Alias;
            childItem.ParentItem = parentItem;
            childItem.ColumnUISetting = new ColumnUISettingDTO();

            if (column.ColumnType != Enum_ColumnType.String)
            {
                childItem.ColumnUISetting.UIColumnsType = Enum_UIColumnsType.Normal;
            }
            else
            {
                if (column.StringColumnType.MaxLength == -1)
                {
                    childItem.ColumnUISetting.UIColumnsType = Enum_UIColumnsType.Full;
                    childItem.ColumnUISetting.UIRowsCount = 3;
                }
                else
                {
                    if (column.StringColumnType.MaxLength <= 64)
                        childItem.ColumnUISetting.UIColumnsType = Enum_UIColumnsType.Normal;
                    else if (column.StringColumnType.MaxLength > 64 && column.StringColumnType.MaxLength <= 256)
                        childItem.ColumnUISetting.UIColumnsType = Enum_UIColumnsType.Half;
                    else if (column.StringColumnType.MaxLength > 256 && column.StringColumnType.MaxLength <= 1024)
                        childItem.ColumnUISetting.UIColumnsType = Enum_UIColumnsType.Full;
                    else if (column.StringColumnType.MaxLength > 1024)
                    {
                        childItem.ColumnUISetting.UIColumnsType = Enum_UIColumnsType.Full;
                        childItem.ColumnUISetting.UIRowsCount = 2;
                    }
                }
            }
            parentItem.ChildItems.Add(childItem);
            //  result.Add(childItem);
        }
        private EntityUICompositionDTO ToEntityUICompositionDTO(DataAccess.EntityUIComposition item, List<ColumnDTO> columns, List<RelationshipDTO> relationships)
        {
            //**4d246d11-17b7-455f-86d9-6204e634719f
            var result = new EntityUICompositionDTO();
            result.ID = item.ID;
            result.ParentID = item.ParentID;
            result.ObjectIdentity = item.ItemIdentity;
            result.Title = item.ItemTitle;
            result.ObjectCategory = (DatabaseObjectCategory)Enum.Parse(typeof(DatabaseObjectCategory), item.Category);
            result.Position = item.Position;
            if (result.ObjectCategory == DatabaseObjectCategory.Entity)
            {
                if (item.EntityUISetting != null)
                {
                    result.EntityUISetting = new EntityUISettingDTO();
                    result.EntityUISetting.ID = item.EntityUISetting.ID;
                    result.EntityUISetting.UIColumnsCount = item.EntityUISetting.UIColumnsCount;
                }
            }
            else if (result.ObjectCategory == DatabaseObjectCategory.Column)
            {
                if (columns.Any(x => x.ID == Convert.ToInt32(item.ItemIdentity)))
                {
                    //   var column = columns.First(x => x.ID == Convert.ToInt32(item.ItemIdentity));
                    //    result.Column = column;
                    if (item.ColumnUISetting != null)
                    {
                        result.ColumnUISetting = new ColumnUISettingDTO();
                        result.ColumnUISetting.ID = item.ColumnUISetting.ID;
                        result.ColumnUISetting.ColumnID = Convert.ToInt32(item.ItemIdentity);
                        result.ColumnUISetting.UIColumnsType = (Enum_UIColumnsType)item.ColumnUISetting.UIColumnsType;
                        result.ColumnUISetting.UIRowsCount = item.ColumnUISetting.UIRowsCount;

                        //      if (columnItems != null)
                        //            columnItems.Add(result.ColumnUISetting);
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (result.ObjectCategory == DatabaseObjectCategory.Relationship)
            {
                if (relationships.Any(x => x.ID == Convert.ToInt32(item.ItemIdentity)))
                {
                    //  var relationship = relationships.First(x => x.ID == Convert.ToInt32(item.ItemIdentity));
                    //  result.Relationship = relationship;
                    if (item.RelationshipUISetting != null)
                    {
                        result.RelationshipUISetting = new RelationshipUISettingDTO();
                        result.RelationshipUISetting.ID = item.RelationshipUISetting.ID;
                        result.RelationshipUISetting.RelationshipID = Convert.ToInt32(item.ItemIdentity);
                        result.RelationshipUISetting.UIColumnsType = (Enum_UIColumnsType)item.RelationshipUISetting.UIColumnsType;
                        result.RelationshipUISetting.Expander = item.RelationshipUISetting.Expander;
                        result.RelationshipUISetting.IsExpanded = item.RelationshipUISetting.IsExpanded == true;
                        result.RelationshipUISetting.UIRowsCount = Convert.ToInt16(item.RelationshipUISetting.UIRowsCount);
                        //      if (relationshipItems != null)
                        //          relationshipItems.Add(result.RelationshipUISetting);
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (result.ObjectCategory == DatabaseObjectCategory.Group)
            {
                if (item.GroupUISetting != null)
                {
                    result.GroupUISetting = new GroupUISettingDTO();
                    result.GroupUISetting.ID = item.GroupUISetting.ID;
                    result.GroupUISetting.UIColumnsType = (Enum_UIColumnsType)item.GroupUISetting.UIColumnsType;
                    result.GroupUISetting.Expander = item.GroupUISetting.Expander;
                    result.GroupUISetting.IsExpanded = item.GroupUISetting.IsExpanded == true;

                    result.GroupUISetting.InternalColumnsCount = item.GroupUISetting.InternalColumnsCount;
                    result.GroupUISetting.UIRowsCount = Convert.ToInt16(item.GroupUISetting.UIRowsCount);

                }
            }
            else if (result.ObjectCategory == DatabaseObjectCategory.TabControl)
            {
                if (item.TabGroupUISetting != null)
                {
                    result.TabGroupUISetting = new TabGroupUISettingDTO();
                    result.TabGroupUISetting.ID = item.TabGroupUISetting.ID;
                    result.TabGroupUISetting.UIColumnsType = (Enum_UIColumnsType)item.TabGroupUISetting.UIColumnsType;
                    result.TabGroupUISetting.Expander = item.TabGroupUISetting.Expander;
                    result.TabGroupUISetting.IsExpanded = item.TabGroupUISetting.IsExpanded == true;

                    result.TabGroupUISetting.UIRowsCount = Convert.ToInt16(item.TabGroupUISetting.UIRowsCount);

                }
            }
            else if (result.ObjectCategory == DatabaseObjectCategory.TabPage)
            {
                if (item.TabPageUISetting != null)
                {
                    result.TabPageUISetting = new TabPageUISettingDTO();
                    result.TabPageUISetting.ID = item.TabPageUISetting.ID;
                    result.TabPageUISetting.InternalColumnsCount = item.TabPageUISetting.InternalColumnsCount;
                }
            }
            else if (result.ObjectCategory == DatabaseObjectCategory.EmptySpace)
            {
                if (item.EmptySpaceUISetting != null)
                {
                    result.EmptySpaceUISetting = new EmptySpaceUISettingDTO();
                    result.EmptySpaceUISetting.ID = item.EmptySpaceUISetting.ID;
                    result.EmptySpaceUISetting.UIColumnsType = (Enum_UIColumnsType)item.EmptySpaceUISetting.UIColumnsType;
                    result.EmptySpaceUISetting.ExpandToEnd = item.EmptySpaceUISetting.ExtendToEnd;
                }
            }
            result.ChildItems = new List<EntityUICompositionDTO>();

            foreach (var citem in item.EntityUIComposition1)
            {
                var child = ToEntityUICompositionDTO(citem, columns, relationships);
                if (child != null)
                    result.ChildItems.Add(child);
            }


            return result;
        }
        public void UpdateUIComposition(DR_Requester requester, int entityID)
        {
            //کلا کامنت شد به موقعش اصلاح شود

            //////BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
            //////var entity = bizTableDrivedEntity.GetTableDrivedEntity(requester, entityID, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships);
            ////////اینجا کش شدن روابط توجه شود
            //////EntityUICompositionDTO generetedUIComposition = null;

            //////if (!EntityHasUIComposition(entity))
            //////{
            //////    generetedUIComposition = GenerateUIComposition(entity);
            //////    Save(entityID, generetedUIComposition);
            //////}
            //////else
            //////{
            //////    //اینجا هم مثل بالا بهینه بشه
            //////    var entityUICompositions = GetEntityUICompositionTree(entityID);
            //////    //     generetedUIComposition = new List<EntityUICompositionDTO>();
            //////    //var rootItem = new List<EntityUICompositionDTO>();
            //////    //List<EntityUICompositionDTO> result = new List<EntityUICompositionDTO>();
            //////    List<RelationshipDTO> candidateRelationships = GetCandidUpdateRelationships(entityUICompositions, entity);
            //////    if (candidateRelationships.Any())
            //////    {
            //////        var index = GetNextIndex(entityUICompositions);
            //////        var tabControl = GetOrCreateRelationshipTabControl(entityUICompositions, index, "روابط", "-99");
            //////        //if (tabControl.ID == 0)
            //////        //    generetedUIComposition.Add(tabControl);
            //////        foreach (var relationship in candidateRelationships)
            //////        {
            //////            //اینجا کامنت شد
            //////            ///var tabPage = AddTabPage(tabControl, relationship.Alias, relationship.ID);
            //////            ///   AddRelationshipUICompositionItem(tabPage,)
            //////        }
            //////        using (var projectContext = new DataAccess.MyIdeaEntities())
            //////        {

            //////            var parentDBItem = projectContext.EntityUIComposition.First(x => x.ID == tabControl.ParentItem.ID);
            //////            CheckAddItems(entityID, generetedUIComposition, parentDBItem, projectContext);

            //////            projectContext.SaveChanges();
            //////        }
            //////    }
            //////    //if (generetedUIComposition.Any())
            //////    //{

            //////    //}
            //////    //AddPrimaryToForeignRelationshipUICompositionItem(result[0], result, entity.Relationships.Where(x => x.DataEntryEnabled == true && x.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign).ToList(), index);
            //////}


        }


        private EntityUICompositionDTO GetOrCreateRelationshipTabControl(EntityUICompositionDTO rootItem, int index, string title, string identity)
        {
            var foundTabControl = FindTabControl(rootItem, identity);
            if (foundTabControl == null)
            {
                foundTabControl = CreateRelationshipTabControl(rootItem, index, title, identity);
            }
            return foundTabControl;
        }

        private EntityUICompositionDTO CreateRelationshipTabControl(EntityUICompositionDTO rootItem, int index, string title, string identity)
        {
            var tabConrol = new EntityUICompositionDTO();
            tabConrol.ObjectCategory = DatabaseObjectCategory.TabControl;
            tabConrol.TabGroupUISetting = new TabGroupUISettingDTO() { UIColumnsType = Enum_UIColumnsType.Full };
            tabConrol.Title = title;
            tabConrol.ObjectIdentity = identity;
            tabConrol.ParentItem = rootItem;
            tabConrol.Position = index;
            return tabConrol;
        }

        private EntityUICompositionDTO FindTabControl(EntityUICompositionDTO rootItem, string identity)
        {
            if (rootItem.ObjectCategory == DatabaseObjectCategory.TabControl)
            {
                if (rootItem.ObjectIdentity == identity)
                {
                    return rootItem;
                }
            }
            foreach (var item in rootItem.ChildItems)
            {
                var findInChild = FindTabControl(item, identity);
                if (findInChild != null)
                    return findInChild;
            }
            return null;
        }
        private EntityUICompositionDTO AddTabPage(EntityUICompositionDTO tabConrol, string title, string identity)
        {
            var tabPage = new EntityUICompositionDTO();
            tabPage.ObjectCategory = DatabaseObjectCategory.TabPage;
            tabPage.ObjectIdentity = identity;
            tabPage.ParentItem = tabConrol;
            tabPage.Title = title;
            tabPage.TabPageUISetting = new TabPageUISettingDTO() { InternalColumnsCount = 4 };
            tabConrol.ChildItems.Add(tabPage);
            return tabPage;
            //AddRelationshipUICompositionItem(tabPage, relationship, 0);
        }

        //private List<RelationshipDTO> GetCandidUpdateRelationships(EntityUICompositionDTO entityUICompositions, TableDrivedEntityDTO entity)
        //{
        //    List<RelationshipDTO> rels = new List<RelationshipDTO>();
        //    foreach (var relationship in entity.Relationships.Where(x => x.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign &&
        //    x.DataEntryEnabled == true && x.IsOtherSideDirectlyCreatable))
        //    {
        //        if (!RelationshipExistsInTree(entityUICompositions, relationship))
        //        {
        //            rels.Add(relationship);
        //        }
        //    }
        //    return rels;
        //}

        private bool RelationshipExistsInTree(EntityUICompositionDTO rootItem, RelationshipDTO relationship)
        {
            if (rootItem.ObjectCategory == DatabaseObjectCategory.Relationship)
            {
                if (rootItem.ObjectIdentity == relationship.ID.ToString())
                {
                    return true;
                }
            }

            foreach (var item in rootItem.ChildItems)
            {
                if (RelationshipExistsInTree(item, relationship))
                    return true;
            }
            return false;
        }



        public bool EntityHasUIComposition(TableDrivedEntityDTO entity)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                return bizTableDrivedEntity.GetAllEnabledEntities(projectContext).First(x => x.ID == entity.ID).EntityUIComposition.Any();
            }
        }

        public void Save(int entityID, EntityUICompositionDTO items)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                SaveItem(projectContext, entityID, items);

                try
                {
                    projectContext.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
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
            }
        }

        public void SaveItem(MyIdeaEntities projectContext, int entityID, EntityUICompositionDTO item)
        {
            var existingIds = GetItemIds(item);// items.Where(x => x.ID != 0).Select(x => x.ID).ToList();
            if (existingIds.Count > 0)
            {
                var removeList = projectContext.EntityUIComposition.Where(x => x.TableDrivedEntityID == entityID && !existingIds.Contains(x.ID)).ToList();
                foreach (var rItem in removeList)
                    RemoveItem(rItem, projectContext);
            }

            CheckAddItems(entityID, item, null, projectContext);

        }

        private List<int> GetItemIds(EntityUICompositionDTO item, List<int> result = null)
        {
            if (result == null)
                result = new List<int>();
            if (item.ID != 0)
                result.Add(item.ID);

            if (item.ChildItems != null && item.ChildItems.Any())
            {
                foreach (var cItem in item.ChildItems)
                {
                    GetItemIds(cItem, result);
                }
            }
            return result;
        }

        private void CheckAddItems(int entityID, EntityUICompositionDTO item, EntityUIComposition parentDBItem, DataAccess.MyIdeaEntities projectContext)
        {
            EntityUIComposition dbItem = null;
            if (item.ID != 0)
            {
                dbItem = projectContext.EntityUIComposition.First(x => x.ID == item.ID);
            }
            else
            {
                dbItem = new EntityUIComposition();
                projectContext.EntityUIComposition.Add(dbItem);
            }
            if (string.IsNullOrEmpty(item.ObjectIdentity))
            {

            }
            dbItem.TableDrivedEntityID = entityID;
            dbItem.Category = item.ObjectCategory.ToString();
            if (item.ObjectCategory == DatabaseObjectCategory.Entity)
            {
                if (dbItem.EntityUISetting == null)
                    dbItem.EntityUISetting = new EntityUISetting();
                dbItem.EntityUISetting.UIColumnsCount = item.EntityUISetting.UIColumnsCount;
            }
            else if (item.ObjectCategory == DatabaseObjectCategory.Column)
            {
                if (dbItem.ColumnUISetting == null)
                    dbItem.ColumnUISetting = new ColumnUISetting();
                dbItem.ColumnUISetting.UIColumnsType = (short)item.ColumnUISetting.UIColumnsType;
                dbItem.ColumnUISetting.UIRowsCount = item.ColumnUISetting.UIRowsCount;
            }
            else if (item.ObjectCategory == DatabaseObjectCategory.Relationship)
            {
                if (dbItem.RelationshipUISetting == null)
                    dbItem.RelationshipUISetting = new RelationshipUISetting();
                dbItem.RelationshipUISetting.UIColumnsType = (short)item.RelationshipUISetting.UIColumnsType;
                dbItem.RelationshipUISetting.Expander = item.RelationshipUISetting.Expander;
                dbItem.RelationshipUISetting.IsExpanded = item.RelationshipUISetting.IsExpanded;
                dbItem.RelationshipUISetting.UIRowsCount = item.RelationshipUISetting.UIRowsCount;

            }
            else if (item.ObjectCategory == DatabaseObjectCategory.Group)
            {
                if (dbItem.GroupUISetting == null)
                    dbItem.GroupUISetting = new GroupUISetting();
                dbItem.GroupUISetting.UIColumnsType = (short)item.GroupUISetting.UIColumnsType;
                dbItem.GroupUISetting.Expander = item.GroupUISetting.Expander;
                dbItem.GroupUISetting.IsExpanded = item.GroupUISetting.IsExpanded;

                dbItem.GroupUISetting.InternalColumnsCount = item.GroupUISetting.InternalColumnsCount;
                dbItem.GroupUISetting.UIRowsCount = item.GroupUISetting.UIRowsCount;

            }
            else if (item.ObjectCategory == DatabaseObjectCategory.TabControl)
            {
                if (dbItem.TabGroupUISetting == null)
                    dbItem.TabGroupUISetting = new TabGroupUISetting();
                dbItem.TabGroupUISetting.UIColumnsType = (short)item.TabGroupUISetting.UIColumnsType;
                dbItem.TabGroupUISetting.Expander = item.TabGroupUISetting.Expander;
                dbItem.TabGroupUISetting.IsExpanded = item.TabGroupUISetting.IsExpanded;

                dbItem.TabGroupUISetting.UIRowsCount = item.TabGroupUISetting.UIRowsCount;

            }
            else if (item.ObjectCategory == DatabaseObjectCategory.TabPage)
            {
                if (dbItem.TabPageUISetting == null)
                    dbItem.TabPageUISetting = new TabPageUISetting();
                dbItem.TabPageUISetting.InternalColumnsCount = item.TabPageUISetting.InternalColumnsCount;

            }
            else if (item.ObjectCategory == DatabaseObjectCategory.EmptySpace)
            {
                if (dbItem.EmptySpaceUISetting == null)
                    dbItem.EmptySpaceUISetting = new EmptySpaceUISetting();
                dbItem.EmptySpaceUISetting.UIColumnsType = (short)item.EmptySpaceUISetting.UIColumnsType;
                dbItem.EmptySpaceUISetting.ExtendToEnd = item.EmptySpaceUISetting.ExpandToEnd;

            }
            dbItem.ItemTitle = item.Title;
            dbItem.Position = item.Position;
            dbItem.ItemIdentity = item.ObjectIdentity ?? item.Title;
            dbItem.EntityUIComposition2 = parentDBItem;

            if (item.ChildItems != null && item.ChildItems.Any())
            {
                foreach (var cItem in item.ChildItems)
                    CheckAddItems(entityID, cItem, dbItem, projectContext);
            }

        }

        private void RemoveItem(DataAccess.EntityUIComposition item, DataAccess.MyIdeaEntities projectContext)
        {
            foreach (var cItem in item.EntityUIComposition1)
            {
                RemoveItem(cItem, projectContext);
            }
            if (item.ColumnUISetting != null)
                projectContext.ColumnUISetting.Remove(item.ColumnUISetting);
            if (item.RelationshipUISetting != null)
                projectContext.RelationshipUISetting.Remove(item.RelationshipUISetting);
            if (item.TabGroupUISetting != null)
                projectContext.TabGroupUISetting.Remove(item.TabGroupUISetting);
            if (item.EmptySpaceUISetting != null)
                projectContext.EmptySpaceUISetting.Remove(item.EmptySpaceUISetting);
            if (item.GroupUISetting != null)
                projectContext.GroupUISetting.Remove(item.GroupUISetting);
            if (item.TabPageUISetting != null)
                projectContext.TabPageUISetting.Remove(item.TabPageUISetting);
            if (item.EntityUISetting != null)
                projectContext.EntityUISetting.Remove(item.EntityUISetting);
            projectContext.EntityUIComposition.Remove(item);
        }
    }
    //public class RelationshipUI
    //{
    //    public RelationshipUI(RelationshipDTO relationship)
    //    {
    //        Relationship = relationship;

    //    }

    //    public RelationshipDTO Relationship { set; get; }
    //    //public bool IsDirect
    //    //{
    //    //    get
    //    //    {
    //    //        if (Relationship.IsOtherSideDirectlyCreatable)
    //    //        {
    //    //            return true;
    //    //        }
    //    //        else
    //    //        {
    //    //            return false;
    //    //        }

    //    //    }
    //    //}

    //}
}
