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
        BizColumn bizColumn = new BizColumn();
        public List<SavedSearchRepositoryDTO> GetSearchRepositoriesBySearchID(int entitySearchID)
        {
            List<SavedSearchRepositoryDTO> result = new List<SavedSearchRepositoryDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var listEntityReport = projectContext.SavedSearchRepository.Where(x =>x.SavedPreDefinedSearch!=null && x.SavedPreDefinedSearch.EntitySearchID == entitySearchID);
                foreach (var item in listEntityReport)
                    result.Add(ToSavedSearchRepositoryDTO(item));
            }
            return result;
        }
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
        public PreDefinedSearchDTO GetPreDefinedSearch(DR_Requester requester, int ID, bool forDefinition)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var item = projectContext.SavedPreDefinedSearch.First(x => x.ID == ID);

                return ToPreDefinedSearchDTO(requester, item, forDefinition);
            }
        }
        public AdvancedSearchDTO GetAdvancedSearch(DR_Requester requester, int ID, bool forDefinition)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var item = projectContext.SaveAdvancedSearch.First(x => x.ID == ID);

                return ToAdvancedSearchDTO(requester, item, forDefinition);
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
            //BizSearchRepository.UpdatePreDefinedSearch: de19b52466fc
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
                        projectContext.SavedPreDefinedSearch.Add(dbItem);

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
                        //  string value = "";
                        //  foreach (var val in dbCol.Value)
                        //      value += (value == "" ? "" : "@") + val.ToString();
                        dbItem.SavedPreDefinedSearchSimpleColumn.Add(
                            new SavedPreDefinedSearchSimpleColumn() { EntitySearchColumnsID = dbCol.EntitySearchColumnsID, Value = dbCol.Value?.ToString(), FormulaID = dbCol.FormulaID == 0 ? null : (int?)dbCol.FormulaID, Operator = (short)dbCol.Operator });
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
                                dbRel.SavedPreDefinedSearchRelationshipData.Add(new SavedPreDefinedSearchRelationshipData() { DataGroup = group.ToString(), KeyColumnID = column.Column.ID, Value = column.Value?.ToString() });
                            }
                            group++;
                        }
                        dbItem.SavedPreDefinedSearchRelationship.Add(dbRel);
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
            // BizSearchRepository.UpdateAdvancedSearch: 6221f0d749e7
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
                        projectContext.SaveAdvancedSearch.Add(dbItem);

                    }
                    dbItem.SavedSearchRepository.IsPreDefinedOrAdvanced = false;
                    dbItem.SavedSearchRepository.Title = advancedSearchDTO.Title;
                    dbItem.SavedSearchRepository.TableDrivedEntityID = advancedSearchDTO.EntityID;

                    if (dbItem.PhraseLogic != null)
                        RemovePhraseLogic(projectContext, dbItem.PhraseLogic);

                    dbItem.PhraseLogic = CreatePhraseLogic(projectContext, advancedSearchDTO.SearchRepositoryMain).PhraseLogic1;


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

        private DataAccess.Phrase CreatePhraseLogic(MyIdeaEntities projectContext, LogicPhraseDTO logicPhraseDTO)
        {
            DataAccess.Phrase phrase = new DataAccess.Phrase();
            //  phrase.Type = 2;
            projectContext.Phrase.Add(phrase);
            phrase.PhraseLogic1 = new PhraseLogic();
            phrase.PhraseLogic1.AndOrType = (short)logicPhraseDTO.AndOrType;
            foreach (var phraseDTO in logicPhraseDTO.Phrases)
            {

                if (phraseDTO is SearchProperty)
                {
                    var created = CreateColumnPhrase(projectContext, phraseDTO as SearchProperty);
                    phrase.PhraseLogic1.Phrase.Add(created);
                }
                else if (phraseDTO is LogicPhraseDTO)
                {
                    var created = CreatePhraseLogic(projectContext, phraseDTO as LogicPhraseDTO);
                    phrase.PhraseLogic1.Phrase.Add(created);
                }
            }

            if (logicPhraseDTO is DP_SearchRepositoryRelationship)
            {
                phrase.PhraseLogic1.PhraseRelationship = new PhraseRelationship();
                phrase.PhraseLogic1.PhraseRelationship.HasNotRelationshipCheck = (logicPhraseDTO as DP_SearchRepositoryRelationship).HasNotRelationshipCheck;
                //dbSearchRepository.HasRelationshipCheck = searchRepository.HasRelationshipCheck;
                phrase.PhraseLogic1.PhraseRelationship.RelationshipFromCount = (logicPhraseDTO as DP_SearchRepositoryRelationship).RelationshipFromCount;
                phrase.PhraseLogic1.PhraseRelationship.RelationshipToCount = (logicPhraseDTO as DP_SearchRepositoryRelationship).RelationshipToCount;
                if ((logicPhraseDTO as DP_SearchRepositoryRelationship).SourceRelationship != null)
                    phrase.PhraseLogic1.PhraseRelationship.SourceRelationID = (logicPhraseDTO as DP_SearchRepositoryRelationship).SourceRelationship.ID;
                else
                    phrase.PhraseLogic1.PhraseRelationship.SourceRelationID = null;
            }


            return phrase;
        }
        private DataAccess.Phrase CreateColumnPhrase(MyIdeaEntities projectContext, SearchProperty searchProperty)
        {
            DataAccess.Phrase phrase = new DataAccess.Phrase();
            phrase.Type = 3;
            projectContext.Phrase.Add(phrase);
            phrase.PhraseColumn = new PhraseColumn();
            phrase.PhraseColumn.ColumnID = searchProperty.ColumnID;
            phrase.PhraseColumn.Operator = searchProperty.Operator.ToString();
            phrase.PhraseColumn.Value = searchProperty.Value?.ToString();
            phrase.PhraseColumn.FormulaID = searchProperty.FormulaID == 0 ? null : (int?)searchProperty.FormulaID;
            return phrase;
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
            foreach (var phrase in phraseLogic.Phrase.ToList())
                RemovePhrase(projectContext, phrase);

            if (phraseLogic.PhraseRelationship != null)
                projectContext.PhraseRelationship.Remove(phraseLogic.PhraseRelationship);

            projectContext.Phrase.Remove(phraseLogic.Phrase1);
            projectContext.PhraseLogic.Remove(phraseLogic);
        }
        private void RemovePhrase(MyIdeaEntities projectContext, DataAccess.Phrase item)
        {
            if (item.PhraseColumn != null)
            {
                projectContext.PhraseColumn.Remove(item.PhraseColumn);
                projectContext.Phrase.Remove(item);
                //  item.ColumnPhrase = null;
            }
            //else if (item.SearchRepositoryID != null)
            //{
            //    RemovePhrase(projectContext, item.SearchRepository.LogicPhrase.Phrase.First());
            //    //    projectContext.LogicPhrase.Remove(item.SearchRepository.LogicPhrase);
            //    //projectContext.SearchRepository.Remove(item.SearchRepository);

            //    // item.SearchRepository = null;
            //}
            else if (item.PhraseLogic1 != null)
            {
                RemovePhraseLogic(projectContext, item.PhraseLogic1);

                //   projectContext.PhraseLogic.Remove(item.PhraseLogic);
                //    item.LogicPhrase = null;
            }

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

        public AdvancedSearchDTO ToAdvancedSearchDTO(DR_Requester requester, SaveAdvancedSearch saveAdvancedSearch, bool forDefinition)
        {
            var result = new AdvancedSearchDTO();
            result.SearchRepositoryMain = new DP_SearchRepositoryMain(saveAdvancedSearch.SavedSearchRepository.TableDrivedEntityID);
            result.SearchRepositoryMain.Title = "عبارت جستجو";
            result.IsPreDefinedOrAdvanced = false;
            result.ID = saveAdvancedSearch.ID;
            result.Title = saveAdvancedSearch.SavedSearchRepository.Title;
            result.EntityID = saveAdvancedSearch.SavedSearchRepository.TableDrivedEntityID;

            SetLogicPhraseDTO(requester, saveAdvancedSearch.PhraseLogic, result.SearchRepositoryMain, forDefinition);
            return result;
        }
        private void SetLogicPhraseDTO(DR_Requester requester, DataAccess.PhraseLogic logicPhrase, ProxyLibrary.LogicPhraseDTO logicPhraseDTO, bool forDefinition)
        {
            logicPhraseDTO.AndOrType = (AndOREqualType)logicPhrase.AndOrType;


            foreach (var dbPhrase in logicPhrase.Phrase)
            {
                if (dbPhrase.PhraseColumn != null)
                    logicPhraseDTO.Phrases.Add(ToColumnPhraseDTO(requester, dbPhrase.PhraseColumn, forDefinition));
                else if (dbPhrase.PhraseLogic1 != null)
                {
                    if (dbPhrase.PhraseLogic1.PhraseRelationship != null)
                    {
                        var newLogicPhrase = new DP_SearchRepositoryRelationship();
                        var phraseRelationship = dbPhrase.PhraseLogic1.PhraseRelationship;
                        var relDTO = new BizRelationship().ToRelationshipDTO(phraseRelationship.Relationship);
                        newLogicPhrase.Title = relDTO.Alias;
                        newLogicPhrase.HasNotRelationshipCheck = phraseRelationship.HasNotRelationshipCheck;
                        newLogicPhrase.RelationshipFromCount = phraseRelationship.RelationshipFromCount;
                        newLogicPhrase.RelationshipToCount = phraseRelationship.RelationshipToCount;
                        //searchRepository.SourceEntityID = relDTO.EntityID1;
                        newLogicPhrase.SourceRelationship = relDTO;
                        newLogicPhrase.ID = phraseRelationship.ID;
                        logicPhraseDTO.Phrases.Add(newLogicPhrase);
                        SetLogicPhraseDTO(requester, dbPhrase.PhraseLogic1, newLogicPhrase, forDefinition);
                    }
                    else
                    {
                        var newLogicPhrase = new LogicPhraseDTO();
                        logicPhraseDTO.Phrases.Add(newLogicPhrase);
                        SetLogicPhraseDTO(requester, dbPhrase.PhraseLogic1, newLogicPhrase, forDefinition);
                    }
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
        public PreDefinedSearchDTO ToPreDefinedSearchDTO(DR_Requester requester, SavedPreDefinedSearch savedPreDefinedSearch, bool forDefinition)
        {
            PreDefinedSearchDTO result = new PreDefinedSearchDTO();
            result.QuickSearchValue = savedPreDefinedSearch.QuickSearchValue;
            result.EntitySearchID = savedPreDefinedSearch.EntitySearchID;
            result.IsPreDefinedOrAdvanced = true;
            result.ID = savedPreDefinedSearch.ID;
            result.Title = savedPreDefinedSearch.SavedSearchRepository.Title;
            result.EntityID = savedPreDefinedSearch.SavedSearchRepository.TableDrivedEntityID;
            BizFormula bizFormula = new BizFormula();
            foreach (var item in savedPreDefinedSearch.SavedPreDefinedSearchSimpleColumn)
            {
                //List<object> values = new List<object>();
                //foreach (var val in item.Value.Split('@'))
                //    values.Add(val);

                var nItem = new DP_PreDefinedSearchSimpleColumn();
                nItem.EntitySearchColumnsID = item.EntitySearchColumnsID;
                nItem.Value = item.Value;
                if (item.FormulaID != null)
                {
                    nItem.FormulaID = item.FormulaID.Value;
                    nItem.Formula = bizFormula.ToFormulaDTO(item.Formula, true);
                    if (!forDefinition)
                    {
                        FormulaFunctionHandler formulaFunctionHandler = new FormulaFunctionHandler();
                        var resultF = formulaFunctionHandler.CalculateFormula(nItem.FormulaID, null, requester);
                        if (resultF.Exception == null)
                            nItem.Value = resultF.Result;
                        else
                        {
                            nItem.Tooltip = resultF.Exception.Message;
                        }
                    }
                }
                nItem.Operator = (CommonOperator)item.Operator;

                result.SimpleSearchProperties.Add(nItem);
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
                        dataItem.KeyProperties.Add(new DP_PreDefinedSearchRelationshipColumns() { Column = bizColumn.ToColumnDTO(col.Column, true), Value = col.Value });
                    }
                    item.DataItems.Add(dataItem);
                }
                result.RelationshipSearchProperties.Add(item);
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

        private ProxyLibrary.Phrase ToColumnPhraseDTO(DR_Requester requester, PhraseColumn columnPhrase, bool forDefinition)
        {
            var columnDTO = bizColumn.ToColumnDTO(columnPhrase.Column, true);
            SearchProperty property = new SearchProperty(columnDTO);
            //property.SearchColumnID = columnPhrase.EntitySearchColumnsID ?? 0;
            property.Operator = (CommonOperator)Enum.Parse(typeof(CommonOperator), columnPhrase.Operator);
            property.Value = columnPhrase.Value;
            BizFormula bizFormula = new BizFormula();
            if (columnPhrase.FormulaID != null)
            {
                property.FormulaID = columnPhrase.FormulaID.Value;
                property.Formula = bizFormula.ToFormulaDTO(columnPhrase.Formula, true);
                if (!forDefinition)
                {
                    FormulaFunctionHandler formulaFunctionHandler = new FormulaFunctionHandler();
                    var resultF = formulaFunctionHandler.CalculateFormula(property.FormulaID, null, requester);
                    if (resultF.Exception == null)
                        property.Value = resultF.Result;
                    else
                    {
                        property.Tooltip = resultF.Exception.Message;
                    }
                }

            }
            return property;
        }
    }
}
