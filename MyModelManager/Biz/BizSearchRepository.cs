using DataAccess;
using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizSearchRepository
    {
        public List<SavedSearchRepositoryDTO> GetSearchRepositories(int entityID)
        {
            List<SavedSearchRepositoryDTO> result = new List<SavedSearchRepositoryDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityReport = projectContext.SavedSearchRepository.Where(x => x.TableDrivedEntityID == entityID);
                foreach (var item in listEntityReport)
                    result.Add(ToSavedSearchRepositoryDTO(item));
            }
            return result;
        }
        public PreDefinedSearchDTO GetPreDefinedSearch(int ID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var item = projectContext.SavedPreDefinedSearch.First(x => x.ID == ID);

                return ToPreDefinedSearchDTO(item);
            }
        }
        public AdvancedSearchDTO GetAdvancedSearch(int ID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var item = projectContext.SaveAdvancedSearch.First(x => x.ID == ID);

                return ToAdvancedSearch(item);
            }
        }
        public SavedSearchRepositoryDTO GetSavedSearchRepository(int ID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var item = projectContext.SavedSearchRepository.First(x => x.ID == ID);

                return ToSavedSearchRepositoryDTO(item);
            }
        }
        public int UpdatePreDefinedSearch(PreDefinedSearchDTO preDefinedSearchDTO)
        {
            try
            {
                using (var projectContext = new DataAccess.MyIdeaEntities())
                {
                    SavedPreDefinedSearch dbItem = null;
                    if (preDefinedSearchDTO.ID != 0)
                        dbItem = projectContext.SavedPreDefinedSearch.FirstOrDefault(x => x.ID == preDefinedSearchDTO.ID);
                    else
                    {
                        dbItem = new DataAccess.SavedPreDefinedSearch();
                        dbItem.SavedSearchRepository = new SavedSearchRepository();
                        projectContext.SavedSearchRepository.Add(dbItem.SavedSearchRepository);

                    }
                    dbItem.SavedSearchRepository.IsPreDefinedOrAdvanced = true;
                    dbItem.SavedSearchRepository.Title = preDefinedSearchDTO.Title;
                    dbItem.SavedSearchRepository.TableDrivedEntityID = preDefinedSearchDTO.EntityID;



                    dbItem.EntitySearchID = preDefinedSearchDTO.EntitySearchID;
                    dbItem.QuickSearchValue = preDefinedSearchDTO.QuickSearchValue;
                    while (dbItem.SavedPreDefinedSearchSimpleColumn.Any())
                        projectContext.SavedPreDefinedSearchSimpleColumn.Remove(dbItem.SavedPreDefinedSearchSimpleColumn.First());
                    foreach (var dbCol in preDefinedSearchDTO.SimpleSearchProperties)
                    {
                        dbItem.SavedPreDefinedSearchSimpleColumn.Add(
                            new SavedPreDefinedSearchSimpleColumn() { EntitySearchColumnsID = dbCol.EntitySearchColumnsID, Value = dbCol.Value?.ToString(), Operator = (short)dbCol.Operator });
                    }

                    while (dbItem.SavedPreDefinedSearchRelationship.Any())
                    {
                        var dbrel = dbItem.SavedPreDefinedSearchRelationship.First();
                        while (dbrel.SavedPreDefinedSearchRelationshipData.Any())
                            projectContext.SavedPreDefinedSearchRelationshipData.Remove(dbrel.SavedPreDefinedSearchRelationshipData.First());
                        projectContext.SavedPreDefinedSearchRelationship.Remove(dbrel);
                    }
                    foreach (var rel in preDefinedSearchDTO.RelationshipSearchProperties)
                    {
                        var dbRel = new SavedPreDefinedSearchRelationship();
                        dbRel.EntitySearchColumnsID = rel.EntitySearchColumnsID;
                        int group = 0;
                        foreach (var data in rel.DataItems)
                        {
                            foreach (var column in data.KeyProperties)
                            {
                                dbRel.SavedPreDefinedSearchRelationshipData.Add(new SavedPreDefinedSearchRelationshipData() { DataGroup = group.ToString(), KeyColumnID = column.ColumnID, Value = column.Value?.ToString() });
                            }
                            group++;
                        }
                    }


                    projectContext.SaveChanges();
                    return dbItem.ID;
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
        }
        public int UpdateAdvancedSearch(AdvancedSearchDTO advancedSearchDTO)
        {
            try
            {
                using (var projectContext = new DataAccess.MyIdeaEntities())
                {
                    SaveAdvancedSearch dbItem = null;
                    if (advancedSearchDTO.ID != 0)
                        dbItem = projectContext.SaveAdvancedSearch.FirstOrDefault(x => x.ID == advancedSearchDTO.ID);
                    else
                    {
                        dbItem = new DataAccess.SaveAdvancedSearch();
                        dbItem.SavedSearchRepository = new SavedSearchRepository();
                        projectContext.SavedSearchRepository.Add(dbItem.SavedSearchRepository);

                    }
                    dbItem.SavedSearchRepository.IsPreDefinedOrAdvanced = false;
                    dbItem.SavedSearchRepository.Title = advancedSearchDTO.Title;
                    dbItem.SavedSearchRepository.TableDrivedEntityID = advancedSearchDTO.EntityID;

                    if (dbItem.PhraseLogic != null)
                        RemovePhraseLogic(projectContext, dbItem.PhraseLogic);

                    dbItem.PhraseLogic = CreatePhraseLogic(projectContext, advancedSearchDTO.SearchRepositoryMain);


                    projectContext.SaveChanges();
                    return dbItem.ID;
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
        }





        private PhraseLogic CreatePhraseLogic(MyIdeaEntities projectContext, LogicPhraseDTO logicPhraseDTO)
        {
            DataAccess.Phrase phrase = new DataAccess.Phrase();
            //  phrase.Type = 2;
            projectContext.Phrase.Add(phrase);
            phrase.PhraseLogic = new PhraseLogic();
            phrase.PhraseLogic.AndOrType = (short)logicPhraseDTO.AndOrType;
            foreach (var phraseDTO in logicPhraseDTO.Phrases)
            {
                if (phraseDTO is SearchProperty)
                    CreateColumnPhrase(projectContext, phraseDTO as SearchProperty);
                else if (phraseDTO is LogicPhraseDTO)
                    CreatePhraseLogic(projectContext, phraseDTO as LogicPhraseDTO);
            }

            if (logicPhraseDTO is DP_SearchRepositoryRelationship)
            {
                phrase.PhraseLogic.PhraseRelationship = new PhraseRelationship();
                phrase.PhraseLogic.PhraseRelationship.HasNotRelationshipCheck = (logicPhraseDTO as DP_SearchRepositoryRelationship).HasNotRelationshipCheck;
                //dbSearchRepository.HasRelationshipCheck = searchRepository.HasRelationshipCheck;
                phrase.PhraseLogic.PhraseRelationship.RelationshipFromCount = (logicPhraseDTO as DP_SearchRepositoryRelationship).RelationshipFromCount;
                phrase.PhraseLogic.PhraseRelationship.RelationshipToCount = (logicPhraseDTO as DP_SearchRepositoryRelationship).RelationshipToCount;
                if ((logicPhraseDTO as DP_SearchRepositoryRelationship).SourceRelationship != null)
                    phrase.PhraseLogic.PhraseRelationship.SourceRelationID = (logicPhraseDTO as DP_SearchRepositoryRelationship).SourceRelationship.ID;
                else
                    phrase.PhraseLogic.PhraseRelationship.SourceRelationID = null;
            }


            return phrase.PhraseLogic;
        }
        private PhraseColumn CreateColumnPhrase(MyIdeaEntities projectContext, SearchProperty searchProperty)
        {
            DataAccess.Phrase phrase = new DataAccess.Phrase();
            phrase.Type = 3;
            projectContext.Phrase.Add(phrase);
            phrase.PhraseColumn = new PhraseColumn();
            phrase.PhraseColumn.ColumnID = searchProperty.ColumnID;
            phrase.PhraseColumn.Operator = searchProperty.Operator.ToString();
            phrase.PhraseColumn.Value = searchProperty.Value == null ? null : searchProperty.Value.ToString();

            return phrase.PhraseColumn;
        }





        //private DataAccess.Phrase ToSearchRepositoryAsPhrase(DP_SearchRepository dP_SearchRepository)
        //{
        //    DataAccess.Phrase dbphrase = new DataAccess.Phrase();
        //    dbphrase.SearchRepository = ToSearchRepository(dP_SearchRepository);
        //    return dbphrase;
        //}
        //private DataAccess.LogicPhrase ToLogicPhrase(ProxyLibrary.LogicPhrase logicPhrase)
        //{
        //    DataAccess.LogicPhrase dbphrase = new DataAccess.LogicPhrase();
        //    dbphrase.Phrase.Add(new DataAccess.Phrase());
        //    dbphrase.LogicPhrase = ToLogicPhrase(logicPhrase);
        //    return dbphrase;
        //}
        //private Tuple<DataAccess.LogicPhrase, DataAccess.Phrase> ToLogicPhrase(MyIdeaEntities projectContext, ProxyLibrary.LogicPhraseDTO logicPhrase)
        //{
        //    //if (logicPhrase.Phrases.Any())
        //    //{

        //    //}
        //    //else
        //    //    return null;
        //}


        //private DataAccess.Phrase ToLogicPhraseAsPhrase(ProxyLibrary.LogicPhrase logicPhrase)
        //{
        //    DataAccess.Phrase dbphrase = new DataAccess.Phrase();
        //    dbphrase.LogicPhrase = ToLogicPhrase(logicPhrase);
        //    return dbphrase;
        //}


        //private void RemoveLogicPhrase(MyIdeaEntities projectContext, DataAccess.LogicPhrase logicPhrase)
        //{
        //    while (logicPhrase.Phrase1.Any())
        //    {
        //        foreach (var item in logicPhrase.Phrase1.ToList())
        //        {
        //            RemovePhrase(projectContext, item);
        //        }
        //    }
        //    projectContext.LogicPhrase.Remove(logicPhrase);
        //}
        private void RemovePhraseLogic(MyIdeaEntities projectContext, PhraseLogic phraseLogic)
        {
            if (phraseLogic.Phrase.Any())
                RemovePhrase(projectContext, phraseLogic.Phrase.First());

            if (phraseLogic.PhraseRelationship != null)
                projectContext.PhraseRelationship.Remove(phraseLogic.PhraseRelationship);

            projectContext.PhraseLogic.Remove(phraseLogic);
        }
        private void RemovePhrase(MyIdeaEntities projectContext, DataAccess.Phrase item)
        {
            if (item.PhraseColumn != null)
            {
                projectContext.PhraseColumn.Remove(item.PhraseColumn);
                //  item.ColumnPhrase = null;
            }
            //else if (item.SearchRepositoryID != null)
            //{
            //    RemovePhrase(projectContext, item.SearchRepository.LogicPhrase.Phrase.First());
            //    //    projectContext.LogicPhrase.Remove(item.SearchRepository.LogicPhrase);
            //    //projectContext.SearchRepository.Remove(item.SearchRepository);

            //    // item.SearchRepository = null;
            //}
            else if (item.PhraseLogic != null)
            {
                RemovePhraseLogic(projectContext, item.PhraseLogic);

                projectContext.PhraseLogic.Remove(item.PhraseLogic);
                //    item.LogicPhrase = null;
            }
            projectContext.Phrase.Remove(item);
        }


        //public DP_SearchRepository ToSearchRepository(SearchRepository entityPreDefinedSearch, bool withDetails)
        //{
        //    //فانکشن به یه کلاس مستقل دیگر منتقل شود
        //    DP_SearchRepository result = new DP_SearchRepository();
        //    result.TargetEntityID = entityPreDefinedSearch.TableDrivedEntityID??0;
        //    result.Title = entityPreDefinedSearch.Title;
        //    result.EntitySearchID = entityPreDefinedSearch.EntitySearchID ?? 0;
        //    //result.IsSimpleSearch = entityPreDefinedSearch.IsSimpleSearch;
        //    result.ID = entityPreDefinedSearch.ID;
        //    if (withDetails)
        //    {
        //        if (entityPreDefinedSearch.SearchRepository != null)
        //            result.SearchRepository = ToSearchRepositoryDTO(entityPreDefinedSearch.SearchRepository, entityPreDefinedSearch.TableDrivedEntityID, entityPreDefinedSearch.TableDrivedEntity.Name);
        //        //foreach (var item in entityPreDefinedSearch.PreDefinedSearchSimpleSearchColumns)
        //        //{
        //        //    PreDefinedSearchColumns newCol = new PreDefinedSearchColumns();
        //        //    newCol.ColumnID = item.ColumnID ?? 0;
        //        //    newCol.EntitySearchColumnsID = item.EntitySearchColumnsID ?? 0;
        //        //    newCol.Value = item.Value;
        //        //    newCol.Operator = (CommonOperator)Enum.Parse(typeof(CommonOperator), item.Operator);
        //        //    result.SimpleColumns.Add(newCol);
        //        //}
        //    }
        //    return result;
        //}



        public SavedSearchRepositoryDTO ToSavedSearchRepositoryDTO(SavedSearchRepository savedSearchRepository)
        {
            SavedSearchRepositoryDTO result = new SavedSearchRepositoryDTO();
            result.ID = savedSearchRepository.ID;
            result.EntityID = savedSearchRepository.TableDrivedEntityID;
            result.Title = savedSearchRepository.Title;
            result.IsPreDefinedOrAdvanced = savedSearchRepository.IsPreDefinedOrAdvanced;
          
            return result;
        }

        private AdvancedSearchDTO ToAdvancedSearch(SaveAdvancedSearch saveAdvancedSearch)
        {
            var result = new AdvancedSearchDTO();
            result.SearchRepositoryMain = new DP_SearchRepositoryMain(saveAdvancedSearch.SavedSearchRepository.TableDrivedEntityID);
            SetLogicPhraseDTO(saveAdvancedSearch.PhraseLogic, result.SearchRepositoryMain);
            return result;
        }
        private void SetLogicPhraseDTO(DataAccess.PhraseLogic logicPhrase, ProxyLibrary.LogicPhraseDTO logicPhraseDTO)
        {
            logicPhraseDTO.AndOrType = (AndOREqualType)logicPhrase.AndOrType;

            if (logicPhrase.PhraseRelationship != null)
            {
                var phraseRelationship = logicPhrase.PhraseRelationship;
                var relDTO = new BizRelationship().ToRelationshipDTO(phraseRelationship.Relationship);
                logicPhraseDTO = new DP_SearchRepositoryRelationship();
                (logicPhraseDTO as DP_SearchRepositoryRelationship).Title = relDTO.Name;
                (logicPhraseDTO as DP_SearchRepositoryRelationship).HasNotRelationshipCheck = phraseRelationship.HasNotRelationshipCheck;
                (logicPhraseDTO as DP_SearchRepositoryRelationship).RelationshipFromCount = phraseRelationship.RelationshipFromCount;
                (logicPhraseDTO as DP_SearchRepositoryRelationship).RelationshipToCount = phraseRelationship.RelationshipToCount;
                //searchRepository.SourceEntityID = relDTO.EntityID1;
                (logicPhraseDTO as DP_SearchRepositoryRelationship).SourceRelationship = relDTO;
                (logicPhraseDTO as DP_SearchRepositoryRelationship).ID = phraseRelationship.ID;

            }

            foreach (var dbPhrase in logicPhrase.Phrase)
            {
                if (dbPhrase.PhraseColumn != null)
                    logicPhraseDTO.Phrases.Add(ToColumnPhraseDTO(dbPhrase.PhraseColumn));
                else if (dbPhrase.PhraseLogic != null)
                {
                    var newLogicPhrase = new LogicPhraseDTO();
                    logicPhraseDTO.Phrases.Add(newLogicPhrase);
                    SetLogicPhraseDTO(dbPhrase.PhraseLogic, newLogicPhrase);
                }
            }

        }
        //private ProxyLibrary.LogicPhraseDTO ToLogicPhraseDTO(DataAccess.PhraseLogic logicPhrase)
        //{
        //    ProxyLibrary.LogicPhraseDTO result = null;
        //    if (logicPhrase.PhraseRelationship != null)
        //    {
        //        result = ToRelationshipSearch(logicPhrase.PhraseRelationship);
        //    }
        //    else
        //    {
        //        result = new ProxyLibrary.LogicPhraseDTO();

        //        result.AndOrType = (AndOREqualType)logicPhrase.AndOrType;

        //        foreach (var dbPhrase in logicPhrase.Phrase)
        //        {
        //            if (dbPhrase.PhraseColumn != null)
        //                result.Phrases.Add(ToColumnPhraseDTO(dbPhrase.PhraseColumn));
        //            else if (dbPhrase.PhraseLogic != null)
        //                result.Phrases.Add(ToLogicPhraseDTO(dbPhrase.PhraseLogic));
        //        }

        //    }

        //    return result;
        //}


        //private LogicPhraseDTO ToRelationshipSearch(PhraseRelationship phraseRelationship)
        //{
        //    //    var result = new LogicPhraseDTO();


        //    var searchRepository =



        //    searchRepository.ID = phraseRelationship.ID;
        //    searchRepository.Title = phraseRelationship.Title;
        //    //    searchRepository.EntitySearchID = phraseRelationship.EntitySearchID ?? 0;
        //    //searchRepository.IsSimpleSearch = phraseRelationship.IsSimpleSearch;

        //    //SetLogicPhraseDTO(savedSearchRepository.LogicPhrase, searchRepository);

        //    //return searchRepository;
        //    //  var result = new LogicPhraseDTO();
        //    searchRepository.AndOrType = AndOREqualType.And;

        //    foreach (var dbPhrase in saveAdvancedSearch.PhraseLogic.Phrase)
        //    {
        //        if (dbPhrase.PhraseColumn != null)
        //            result.Phrases.Add(ToColumnPhraseDTO(dbPhrase.PhraseColumn));
        //        else if (dbPhrase.PhraseLogic != null)
        //            result.Phrases.Add(ToLogicPhraseDTO(dbPhrase.PhraseLogic));
        //    }
        //    return result;
        //}
        private PreDefinedSearchDTO ToPreDefinedSearchDTO(SavedPreDefinedSearch savedPreDefinedSearch)
        {
            PreDefinedSearchDTO result = new PreDefinedSearchDTO();
            result.QuickSearchValue = savedPreDefinedSearch.QuickSearchValue;
            result.EntitySearchID = savedPreDefinedSearch.EntitySearchID ?? 0;
            foreach (var item in savedPreDefinedSearch.SavedPreDefinedSearchSimpleColumn)
            {
                List<object> values = new List<object>();
                foreach (var val in item.Value.Split('@'))
                    values.Add(val);
                result.SimpleSearchProperties.Add(new DP_PreDefinedSearchSimpleColumn() { EntitySearchColumnsID = item.EntitySearchColumnsID, Value = values, Operator = (CommonOperator)item.Operator });
            }
            foreach (var dbItem in savedPreDefinedSearch.SavedPreDefinedSearchRelationship)
            {
                var item = new DP_PreDefinedSearchRelationship();
                item.EntitySearchColumnsID = dbItem.EntitySearchColumnsID ?? 0;
                item.DataItems = new List<DP_PreDefinedSearchRelationshipData>();


                foreach (var data in dbItem.SavedPreDefinedSearchRelationshipData.GroupBy(x => x.DataGroup))
                {
                    DP_PreDefinedSearchRelationshipData dataItem = new DP_PreDefinedSearchRelationshipData();
                    dataItem.KeyProperties = new List<DP_PreDefinedSearchRelationshipColumns>();
                    foreach (var col in data)
                    {
                        dataItem.KeyProperties.Add(new DP_PreDefinedSearchRelationshipColumns() { ColumnID = col.KeyColumnID, Value = col.Value });
                    }
                    item.DataItems.Add(dataItem);
                }

            }
            return result;
        }

        //private DP_SearchRepository ToSearchRepositoryDTO(SearchRepository SearchRepository)
        //{
        //    var relDTO = new BizRelationship().GetRelationship(SearchRepository.SourceRelationID);
        //    DP_SearchRepository searchRepository = new DP_SearchRepository(relDTO.EntityID2);
        //    searchRepository.Name = relDTO.Name;
        //    searchRepository.HasNotRelationshipCheck = SearchRepository.HasNotRelationshipCheck;
        //    //searchRepository.HasRelationshipCheck = SearchRepository.HasRelationshipCheck;
        //    searchRepository.RelationshipFromCount = SearchRepository.RelationshipFromCount;
        //    searchRepository.RelationshipToCount = SearchRepository.RelationshipToCount;
        //    searchRepository.SourceEntityID = relDTO.EntityID1;
        //    searchRepository.SourceRelationID = relDTO.ID;
        //    searchRepository.SourceToTargetMasterRelationshipType = relDTO.MastertTypeEnum;
        //    searchRepository.SourceToTargetRelationshipType = relDTO.TypeEnum;
        //    if (SearchRepository.LogicPhrase != null)
        //        searchRepository.Phrases.Add(ToLogicPhraseDTO(SearchRepository.LogicPhrase));

        //    return searchRepository;
        //}

        private ProxyLibrary.Phrase ToColumnPhraseDTO(PhraseColumn columnPhrase)
        {
            SearchProperty property = new SearchProperty();
            property.ColumnID = columnPhrase.ColumnID;
            //property.SearchColumnID = columnPhrase.EntitySearchColumnsID ?? 0;
            property.Operator = (CommonOperator)Enum.Parse(typeof(CommonOperator), columnPhrase.Operator);
            property.Value = columnPhrase.Value;
            return property;
        }
    }
}
