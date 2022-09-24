using DataAccess;
using ModelEntites;
using MyDataManagerBusiness;
using MyGeneralLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLibrary;
using MyCacheManager;
using AutoMapper;

namespace MyModelManager
{
    public class BizFormula
    {
        //public CodeFunctionEntityFormulaDTO GetCodeFunctionEntityFormula(DR_Requester requester, int FormulaID, bool withDetails)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbFormula = projectContext.Formula.First(x => x.ID == FormulaID);
        //        CodeFunctionEntityFormulaDTO result = new CodeFunctionEntityFormulaDTO();
        //        ToFormulaDTO(dbFormula, result, withDetails);
        //        if (withDetails)
        //        {
        //            BizCodeFunction bizCodeFunction = new MyModelManager.BizCodeFunction();
        //            result.CodeFunctionEntity = bizCodeFunction.GetCodeFunctionEntity(requester, result.CodeFunctionEntityID);
        //        }
        //        return result;
        //    }
        //}
        public LinearFormulaDTO GetLinearFormula(DR_Requester requester, int FormulaID, bool withDetails)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbFormula = projectContext.Formula.First(x => x.ID == FormulaID);
                //     LinearFormulaDTO result = new LinearFormulaDTO();
                     Mapper.Initialize(cfg => cfg.CreateMap<FormulaDTO, LinearFormulaDTO>());
                var formula = ToFormulaDTO(dbFormula, withDetails);

                var result = AutoMapper.Mapper.Map<FormulaDTO, LinearFormulaDTO>(formula);

                result.FormulaText = dbFormula.LinearFormula.FormulaText;
                result.Version = dbFormula.LinearFormula.Version;

                return result;
            }
        }
        public List<FormulaDTO> GetAllFormulas(DR_Requester requester, string generalFilter)
        {
            List<FormulaDTO> result = new List<FormulaDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var list = projectContext.Formula as IQueryable<Formula>;
                if (generalFilter != "")
                    list = list.Where(x => x.ID.ToString() == generalFilter || x.Name.Contains(generalFilter) || x.Title.Contains(generalFilter));

                foreach (var item in list)
                {

                    var rItem = ToFormulaDTO(item, false);
                    result.Add(rItem);

                }
            }

            return result;
        }
        //public CodeFunctionFormulaDTO GetCodeFunctionFormula(DR_Requester requester, int FormulaID, bool withDetails)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbFormula = projectContext.Formula.First(x => x.ID == FormulaID);
        //        CodeFunctionFormulaDTO result = new CodeFunctionFormulaDTO();
        //        ToFormulaDTO(dbFormula, result, withDetails);
        //        if (withDetails)
        //        {
        //            BizCodeFunction bizCodeFunction = new MyModelManager.BizCodeFunction();
        //            result.CodeFunction = bizCodeFunction.GetCodeFunction(requester, result.CodeFunctionID);
        //        }
        //        return result;
        //    }
        //}
        //public DatabaseFunctionEntityFormulaDTO GetDatabaseFunctionEntityFormula(DR_Requester requester, int FormulaID, bool withDetails)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbFormula = projectContext.Formula.First(x => x.ID == FormulaID);
        //        DatabaseFunctionEntityFormulaDTO result = new DatabaseFunctionEntityFormulaDTO();
        //        ToFormulaDTO(dbFormula, result, withDetails);
        //        if (withDetails)
        //        {
        //            BizDatabaseFunction bizDatabaseFunction = new MyModelManager.BizDatabaseFunction();
        //            result.DatabaseFunctionEntity = bizDatabaseFunction.GetDatabaseFunctionEntity(requester, result.DatabaseFunctionEntityID);
        //        }
        //        return result;
        //    }
        //}


        public FormulaDTO GetFormula(DR_Requester requester, int FormulaID, bool withDetails)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var Formula = projectContext.Formula.First(x => x.ID == FormulaID);

                FormulaDTO result = new FormulaDTO();
                return ToFormulaDTO(Formula, withDetails);


            }
        }
        //public FormulaDTO ToFormulaDTO(DataAccess.Formula item, bool withDetails)
        //{
        //    FormulaDTO result = new FormulaDTO();
        //    ToFormulaDTO(item, result, withDetails);
        //    return result;
        //}
        public FormulaDTO ToFormulaDTO(DataAccess.Formula item, bool withDetails)
        {
            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Formula, item.ID.ToString(), withDetails.ToString());
            if (cachedItem != null)
                return (cachedItem as FormulaDTO);
            var result = new FormulaDTO();
            result.Name = item.Name;
            result.ID = item.ID;
            result.EntityID = item.TableDrivedEntityID ?? 0;
            if (item.LinearFormulaID != null)
            {
                result.FormulaType = FormulaType.Linear;
                result.LinearFormulaID = item.LinearFormulaID.Value;
            }
            else if (item.CodeFunction_TableDrivedEntityID != null)
            {
                result.FormulaType = FormulaType.CodeFunctionEntity;
                result.CodeFunctionEntityID = item.CodeFunction_TableDrivedEntityID.Value;
            }
            else if (item.DatabaseFunction_TableDrivedEntityID != null)
            {
                result.FormulaType = FormulaType.DatabaseFunctionEntity;
                result.DatabaseFunctionEntityID = item.DatabaseFunction_TableDrivedEntityID.Value;
            }
            else if (item.CodeFunctionID != null)
            {
                result.FormulaType = FormulaType.CodeFunction;
                result.CodeFunctionID = item.CodeFunctionID.Value;
            }
            if (item.LinearFormula != null)
                result.Tooltip = item.LinearFormula.FormulaText;
            result.Title = item.Title;
            //result.ValueCustomType = (ValueCustomType)item.ValueCustomType;
            result.ResultDotNetType = GetFormulaDotNetType(item.ResultType);
            result.ResultType = item.ResultType;
            //////BizFormulaUsage
            //////result.FormulaUsed = item.FormulaUsage.Any();
            if (withDetails)
            {
                foreach (var dbFormulaItem in item.FormulaItems1)
                {
                    result.FormulaItems.Add(ToFormualaItemDTO(item.TableDrivedEntityID ?? 0, dbFormulaItem));
                }
            }

            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Formula, item.ID.ToString(), withDetails.ToString());
            return result;
        }
        public List<FormulaDTO> GetFormulas(int entityID, bool generalFormulas)
        {
            List<FormulaDTO> result = new List<FormulaDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                IQueryable<Formula> formulas = null;
                if (generalFormulas)
                    formulas = projectContext.Formula.Where(x => x.TableDrivedEntityID == null || x.TableDrivedEntityID == entityID);
                else
                {
                    formulas = projectContext.Formula.Where(x => x.TableDrivedEntityID == entityID);
                }
                foreach (var item in formulas)
                {
                    //  FormulaDTO rItem = new FormulaDTO();
                    var rItem = ToFormulaDTO(item, false);
                    result.Add(rItem);
                }
                return result;
            }
        }







        private FormulaItemDTO ToFormualaItemDTO(int entityID, FormulaItems dbFormulaItem)
        {
            FormulaItemDTO formulaItem = new FormulaItemDTO();
            formulaItem.ID = dbFormulaItem.ID;
            formulaItem.RelationshipIDTail = dbFormulaItem.RelationshipIDTail;
            if (!string.IsNullOrEmpty(formulaItem.RelationshipIDTail))
            {
                var bizEntityRelationshipTail = new BizEntityRelationshipTail();
                formulaItem.RelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(entityID, formulaItem.RelationshipIDTail);
            }
            formulaItem.RelationshipNameTail = dbFormulaItem.RelationshipNameTail;
            formulaItem.FormulaID = dbFormulaItem.FormulaID ?? 0;
            formulaItem.ItemTitle = dbFormulaItem.ItemTitle;
            if (dbFormulaItem.ColumnID != null)
            {
                formulaItem.ItemID = dbFormulaItem.ColumnID.Value;
                formulaItem.ItemType = FormuaItemType.Column;
            }
            else if (dbFormulaItem.FormulaParameterID != null)
            {
                formulaItem.ItemID = dbFormulaItem.FormulaParameterID.Value;
                formulaItem.ItemType = FormuaItemType.FormulaParameter;
            }
            else if (dbFormulaItem.RelationshipID != null)
            {
                formulaItem.ItemID = dbFormulaItem.RelationshipID.Value;
                formulaItem.ItemType = FormuaItemType.Relationship;
            }

            //foreach (var dbcItem in dbFormulaItem.FormulaItems1)
            //{
            //    formulaItem.ChildFormulaItems.Add(ToFormualaItemDTO(dbcItem));
            //}

            return formulaItem;
        }

        public Type GetFormulaDotNetType(int forumlaID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var Formula = projectContext.Formula.First(x => x.ID == forumlaID);
                return GetFormulaDotNetType(Formula.ResultType);
            }
        }

        private Type GetFormulaDotNetType(string resultType)
        {
            return Type.GetType(resultType);
        }



        //public FormulaParameterDTO GetFormulaParameter(int formulaParameterID)
        //{
        //    List<FormulaParameterDTO> result = new List<FormulaParameterDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        return ToFormulaParameterDTO(projectContext.FormulaParameter.First(x => x.ID == formulaParameterID));
        //    }
        //}
        //public List<FormulaParameterDTO> GetFormulaParameters(int entityID)
        //{
        //    List<FormulaParameterDTO> result = new List<FormulaParameterDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var list = projectContext.FormulaParameter.Where(x => x.TableDrivedEntityID == entityID);
        //        foreach (var item in list)
        //            result.Add(ToFormulaParameterDTO(item));
        //        return result;
        //    }
        //}

        //private FormulaParameterDTO ToFormulaParameterDTO(FormulaParameter cItem)
        //{
        //    var result = new FormulaParameterDTO();
        //    result.ID = cItem.ID;
        //    result.FormulaID = cItem.FormulaID;
        //    result.EntityID = cItem.TableDrivedEntityID;
        //    result.Name = cItem.Name;
        //    result.Title = cItem.Title;
        //    result.ResultType = GetFormulaDotNetType(cItem.Formula.ResultType);
        //    return result;
        //}







        //public int UpdateDatabaseFunction(DatabaseFunctionDTO DatabaseFunction)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbDatabaseFunction = projectContext.DatabaseFunction.FirstOrDefault(x => x.ID == DatabaseFunction.ID);
        //        if (dbDatabaseFunction == null)
        //            dbDatabaseFunction = new DataAccess.DatabaseFunction();
        //        dbDatabaseFunction.FunctionName = DatabaseFunction.FunctionName;
        //        dbDatabaseFunction.ID = DatabaseFunction.ID;
        //        if (DatabaseFunction.EntityID != 0)
        //            dbDatabaseFunction.TableDrivedEntityID = DatabaseFunction.EntityID;
        //        dbDatabaseFunction.Title = DatabaseFunction.Title;
        //        dbDatabaseFunction.ReturnType = DatabaseFunction.ReturnType;
        //        dbDatabaseFunction.RelatedSchema = DatabaseFunction.Schema;
        //        while (dbDatabaseFunction.DatabaseFunctionParameter.Any())
        //        {
        //            dbDatabaseFunction.DatabaseFunctionParameter.Remove(dbDatabaseFunction.DatabaseFunctionParameter.First());
        //        }
        //        foreach (var column in DatabaseFunction.DatabaseFunctionParameter)
        //        {
        //            DatabaseFunctionParameter dbColumn = new DataAccess.DatabaseFunctionParameter();
        //            dbColumn.ColumnID = column.ColumnID;
        //            dbColumn.DataType = column.DataType;
        //            dbColumn.ParamName = column.ParameterName;
        //            dbDatabaseFunction.DatabaseFunctionParameter.Add(dbColumn);
        //        }
        //        if (dbDatabaseFunction.ID == 0)
        //            projectContext.DatabaseFunction.Add(dbDatabaseFunction);
        //        projectContext.SaveChanges();
        //        return dbDatabaseFunction.ID;
        //    }
        //}


        //private Enum_ColumnType ConvertParameterResultType(string resultType)
        //{
        //    if (string.IsNullOrEmpty(resultType))
        //        return Enum_ColumnType.None;
        //    else if (resultType == "String")
        //        return Enum_ColumnType.String;
        //    else if (resultType == "Numeric")
        //        return Enum_ColumnType.Numeric;
        //    else if (resultType == "Boolean")
        //        return Enum_ColumnType.Boolean;
        //    else if (resultType == "Date")
        //        return Enum_ColumnType.Date;
        //    return Enum_ColumnType.None;
        //}
        //private string ConvertParameterResultType(Enum_ColumnType resultType)
        //{
        //    return resultType.ToString();
        //}

        public int UpdateFormula(FormulaDTO formula, LinearFormulaDTO linearFormula)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                DataAccess.Formula dbFormula = null;
                if (formula.ID == 0)
                {
                    dbFormula = new Formula();
                }
                else
                    dbFormula = projectContext.Formula.FirstOrDefault(x => x.ID == formula.ID);

                dbFormula.Name = formula.Name;
                dbFormula.ID = formula.ID;
                if (formula.EntityID != 0)
                    dbFormula.TableDrivedEntityID = formula.EntityID;
                else
                    dbFormula.TableDrivedEntityID = null;
                dbFormula.ResultType = formula.ResultType;

                //dbFormula.ValueCustomType = (Int16)Formula.ValueCustomType;
                dbFormula.Title = formula.Title;
                while (dbFormula.FormulaItems1.Any())
                    projectContext.FormulaItems.Remove(dbFormula.FormulaItems1.First());
                foreach (var formulaItem in formula.FormulaItems)
                {
                    var dbFormulaItem = ToFormualaItem(formulaItem, dbFormula);
                    dbFormula.FormulaItems1.Add(dbFormulaItem);
                }

                if (formula.FormulaType == FormulaType.Linear)
                {
                    if (dbFormula.LinearFormula == null)
                        dbFormula.LinearFormula = new LinearFormula();
                    dbFormula.LinearFormula.FormulaText = linearFormula.FormulaText;
                    dbFormula.LinearFormula.Version = linearFormula.Version;

                }
                else
                {
                    if (dbFormula.LinearFormula != null)
                        projectContext.LinearFormula.Remove(dbFormula.LinearFormula);
                }
                if (formula.FormulaType == FormulaType.CodeFunctionEntity)
                {
                    dbFormula.CodeFunction_TableDrivedEntityID = formula.CodeFunctionEntityID;
                }
                else
                {
                    dbFormula.CodeFunction_TableDrivedEntityID = null;
                }
                if (formula.FormulaType == FormulaType.CodeFunction)
                {
                    dbFormula.CodeFunctionID = formula.CodeFunctionID;
                }
                else
                {
                    dbFormula.CodeFunctionID = null;
                }
                if (formula.FormulaType == FormulaType.DatabaseFunctionEntity)
                {
                    dbFormula.DatabaseFunction_TableDrivedEntityID = formula.DatabaseFunctionEntityID;
                }
                else
                {
                    dbFormula.DatabaseFunction_TableDrivedEntityID = null;
                }
                if (dbFormula.ID == 0)
                    projectContext.Formula.Add(dbFormula);
                projectContext.SaveChanges();
                return dbFormula.ID;
            }
        }

        //private void RemoveFormulaItem(MyIdeaEntities projectContext, FormulaItems formulaItems)
        //{
        //    while (formulaItems.Any())
        //    {
        //        projectContext.FormulaItems.Remove(formulaItems);
        //    }
        //}


        private FormulaItems ToFormualaItem(FormulaItemDTO formulaItem, Formula dbFormula)
        {
            FormulaItems dbItem = new FormulaItems();
            dbItem.ID = formulaItem.ID;
            dbItem.RelationshipIDTail = formulaItem.RelationshipIDTail;
            dbItem.RelationshipNameTail = formulaItem.RelationshipNameTail;
            dbItem.ItemTitle = formulaItem.ItemTitle;
            if (formulaItem.ItemType == FormuaItemType.Column)
                dbItem.ColumnID = formulaItem.ItemID;
            else if (formulaItem.ItemType == FormuaItemType.FormulaParameter)
                dbItem.FormulaParameterID = formulaItem.ItemID;
            else if (formulaItem.ItemType == FormuaItemType.Relationship)
                dbItem.RelationshipID = formulaItem.ItemID;
            else if (formulaItem.ItemType == FormuaItemType.DatabaseFunction)
                dbItem.DatabaseFunction_TableDrivedEntityID = formulaItem.ItemID;
            else if (formulaItem.ItemType == FormuaItemType.Code)
                dbItem.CodeFunction_TableDrivedEntityID = formulaItem.ItemID;
            else if (formulaItem.ItemType == FormuaItemType.State)
                dbItem.TableDrivedEntityStateID = formulaItem.ItemID;
            //foreach (var citem in formulaItem.ChildFormulaItems)
            //{
            //    var sendRelationshipTail = relationshipTail;
            //    if (dbItem.RelationshipID != null)
            //        sendRelationshipTail = (sendRelationshipTail == "" ? dbItem.RelationshipID.ToString() : sendRelationshipTail + "," + dbItem.RelationshipID);

            //    var childItem = ToFormualaItem(citem, dbFormula, sendRelationshipTail);
            //    dbItem.FormulaItems1.Add(childItem);
            //}

            return dbItem;
        }

        //private void AddDBFormulaItem(Formula dbFormula, ICollection<FormulaItems> formulaItems, FormulaItemDTO formulaItem)
        //{

        //    dbItem.Formula = dbFormula;
        //    formulaItems.Add(dbItem);



        //}

        //public int UpdateFormulaParameterss(FormulaParameterDTO formulaParameter)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbFormulaParameter = projectContext.FormulaParameter.FirstOrDefault(x => x.ID == formulaParameter.ID);
        //        if (dbFormulaParameter == null)
        //            dbFormulaParameter = new DataAccess.FormulaParameter();
        //        dbFormulaParameter.Name = formulaParameter.Name;
        //        dbFormulaParameter.ID = formulaParameter.ID;
        //        if (formulaParameter.EntityID != 0)
        //            dbFormulaParameter.TableDrivedEntityID = formulaParameter.EntityID;
        //        dbFormulaParameter.FormulaID = formulaParameter.FormulaID;
        //        dbFormulaParameter.Title = formulaParameter.Title;

        //        if (dbFormulaParameter.ID == 0)
        //            projectContext.FormulaParameter.Add(dbFormulaParameter);
        //        projectContext.SaveChanges();
        //        return dbFormulaParameter.ID;
        //    }
        //}



    }

}
