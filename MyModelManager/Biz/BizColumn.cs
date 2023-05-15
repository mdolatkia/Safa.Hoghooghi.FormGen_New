using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLibrary;
using System.Data;

namespace MyModelManager
{
    public class BizColumn
    {
        //public Type GetColumnDotNetType(int columnID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var column = GetColumn(columnID);
        //        return GetColumnDotNetType(column.DataType, column.IsNull);
        //    }
        //}
        public Type GetColumnDotNetType(string type, bool isNullable)
        {
            switch (type)
            {
                case "bigint":
                    return isNullable ? typeof(long?) : typeof(long);

                case "binary":
                case "image":
                case "timestamp":
                case "varbinary":
                    return typeof(byte[]);

                case "bit":
                    return isNullable ? typeof(bool?) : typeof(bool);

                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                case "xml":
                    return typeof(string);

                case "datetime":
                case "smalldatetime":
                case "date":
                case "time":
                case "datetime2":
                    return isNullable ? typeof(DateTime?) : typeof(DateTime);

                case "decimal":
                case "money":
                case "smallmoney":
                    return isNullable ? typeof(decimal?) : typeof(decimal);

                case "float":
                    return isNullable ? typeof(double?) : typeof(double);

                case "int":
                    return isNullable ? typeof(int?) : typeof(int);

                case "real":
                    return isNullable ? typeof(float?) : typeof(float);

                case "uniqueidentifier":
                    return isNullable ? typeof(Guid?) : typeof(Guid);

                case "smallint":
                    return isNullable ? typeof(short?) : typeof(short);

                case "tinyint":
                    return isNullable ? typeof(byte?) : typeof(byte);

                case "variant":
                case "udt":
                    return typeof(object);

                case "structured":
                    return typeof(DataTable);

                case "datetimeoffset":
                    return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);

                default:
                    throw new Exception("sqltype");
            }

            //if (type == "char" || type == "nvarchar" || type == "varchar" || type == "text")
            //    return typeof(string);
            //else if (type == "bigint" || type == "numeric" || type == "smallint"
            //    || type == "smallmoney" || type == "int"
            //    || type == "tinyint" || type == "money")
            //    return typeof(int);
            //else if (type == "decimal")
            //    return typeof(decimal);
            //else if (type == "double")
            //    return typeof(Nullable<double>);
            //else if (type == "float")
            //    return typeof(Nullable<double>);
            //else if (type == "date" || type == "datetime")
            //    return typeof(string);
            //else if (type == "bit")
            //    return typeof(bool);
            //return null;
        }

        public ModelEntites.ColumnCustomFormulaDTO GetCustomFormula(int columnID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbColumn = GetEnabledColumn(projectContext, columnID);
                if (dbColumn.ColumnCustomFormula == null)
                    return null;
                else
                    return ToColumnCustomFormulaDTO(dbColumn.ColumnCustomFormula);
            }
        }

        public void DeleteColumnCustomFormula(int columnID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbColumn = GetEnabledColumn(projectContext, columnID);


                if (dbColumn.ColumnCustomFormula == null)
                    projectContext.ColumnCustomFormula.Remove(dbColumn.ColumnCustomFormula);
                projectContext.SaveChanges();
            }
        }

        public void SaveColumnCustomFormula(int columnID, ModelEntites.ColumnCustomFormulaDTO message)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbColumn = GetEnabledColumn(projectContext, columnID);


                if (dbColumn.ColumnCustomFormula == null)
                    dbColumn.ColumnCustomFormula = new DataAccess.ColumnCustomFormula();

                dbColumn.ColumnCustomFormula.FormulaID = message.FormulaID;
                dbColumn.ColumnCustomFormula.CalculateFormulaAsDefault = message.CalculateFormulaAsDefault;
                dbColumn.ColumnCustomFormula.OnlyOnEmptyValue = message.OnlyOnEmptyValue;
                dbColumn.ColumnCustomFormula.OnlyOnNewData = message.OnlyOnNewData;
                projectContext.SaveChanges();
            }



        }


        //public Type GetColumnDotNetType(string type)
        //{

        //}
        //public List<ColumnDTO> GetTableColumns(int tableID, bool simple)
        //{
        //    List<ColumnDTO> result = new List<ColumnDTO>();

        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var table = projectContext.Table.First(x => x.ID == tableID);
        //        foreach (var column in table.Column)
        //            result.Add(ToColumnDTO(column, simple));
        //    }
        //    return result;
        //}

        //public ICollection<Column> GetColumns(TableDrivedEntity entity)
        //{
        //    if (entity.Column.Any())
        //        return entity.Column;
        //    else
        //        return entity.Table.Column;
        //}

        public ColumnDTO GetColumnDTO(int columnID, bool simple)
        {
            ColumnDTO result = new ColumnDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var column = GetEnabledColumn(projectContext, columnID);
                return ToColumnDTO(column, simple);
            }
        }

        public StringColumnTypeDTO GetStringColumType(int columnID)
        {
            ColumnDTO result = new ColumnDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var column = GetEnabledColumn(projectContext, columnID);
                if (column.StringColumnType != null)
                    return ToStringColumTypeDTO(column.StringColumnType);
            }
            return null;
        }

        //public void UpdateCustomCalculation(int columnID, int formulaID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var column = GetColumn(columnID);
        //        if (column.CustomCalculatedColumn == null)
        //            column.CustomCalculatedColumn = new CustomCalculatedColumn();
        //        column.CustomCalculatedColumn.FormulaID = formulaID;
        //        projectContext.SaveChanges();
        //    }
        //}

        private StringColumnTypeDTO ToStringColumTypeDTO(DataAccess.StringColumnType item)
        {
            StringColumnTypeDTO result = new StringColumnTypeDTO();
            result.ColumnID = item.ColumnID;
            result.Format = item.Format;
            result.MaxLength = item.MaxLength;
            result.MinLength = item.MinLength;
            return result;
        }
        public NumericColumnTypeDTO GetNumericColumType(int columnID)
        {
            ColumnDTO result = new ColumnDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var column = GetEnabledColumn(projectContext, columnID);
                if (column.NumericColumnType != null)
                    return ToNumericColumTypeDTO(column.NumericColumnType);
            }
            return null;
        }
        private NumericColumnTypeDTO ToNumericColumTypeDTO(DataAccess.NumericColumnType item)
        {
            NumericColumnTypeDTO result = new NumericColumnTypeDTO();
            result.ColumnID = item.ColumnID;
            result.MaxValue = item.MaxValue;
            result.MinValue = item.MinValue;
            result.Precision = (item.Precision == null ? 0 : item.Precision.Value);
            result.Scale = (item.Scale == null ? 0 : item.Scale.Value);
            result.Delimiter = item.Delimiter;

            return result;
        }



        public DateColumnTypeDTO GetDateColumType(int columnID)
        {
            ColumnDTO result = new ColumnDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var column = GetEnabledColumn(projectContext, columnID);
                if (column.DateColumnType != null)
                    return ToDateColumTypeDTO(column.DateColumnType, true);
            }
            return null;
        }

        private DateColumnTypeDTO ToDateColumTypeDTO(DataAccess.DateColumnType item, bool withNullValues)
        {
            //**BizColumn.ToDateColumTypeDTO: 5bd4e7ec-d66f-4476-a4e7-0c4c410b98b1
            DateColumnTypeDTO result = new DateColumnTypeDTO();
            result.ColumnID = item.ColumnID;
            result.DBValueIsString = item.DBValueIsString;
            if (withNullValues)
            {
                //اینجا برای برنامه مدیریت فراداده است
                result.ShowMiladiDateInUI = item.ShowMiladiDateInUI;
                result.DBStringValueIsMiladi = item.DBValueIsStringMiladi;
            }
            else
            {
                if (item.ShowMiladiDateInUI != null)
                    result.ShowMiladiDateInUI = item.ShowMiladiDateInUI.Value;
                else
                {
                    if (item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting != null)
                    {
                        result.ShowMiladiDateInUI = item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.ShowMiladiDateInUI;
                    }
                    else
                        result.ShowMiladiDateInUI = false;
                }
                if (item.DBValueIsStringMiladi != null)
                    result.DBStringValueIsMiladi = item.DBValueIsStringMiladi.Value;
                else
                {
                    if (item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting != null)
                    {
                        result.DBStringValueIsMiladi = item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.StringDateColumnIsMiladi;
                    }
                    else
                        result.DBStringValueIsMiladi = false;
                }
            }


            return result;
        }



        public TimeColumnTypeDTO GetTimeColumType(int columnID)
        {
            ColumnDTO result = new ColumnDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var column = GetEnabledColumn(projectContext, columnID);
                if (column.TimeColumnType != null)
                    return ToTimeColumTypeDTO(column.TimeColumnType, true);
            }
            return null;
        }


        private TimeColumnTypeDTO ToTimeColumTypeDTO(DataAccess.TimeColumnType item, bool withNullValues)
        {
            //**BizColumn.ToTimeColumTypeDTO: 8f3baefaf4fd
            TimeColumnTypeDTO result = new TimeColumnTypeDTO();
            result.ColumnID = item.ColumnID;
            result.DBValueIsString = item.DBValueIsString;
            if (withNullValues)
            {
                if (item.DBValueStringTimeFormat != null)
                    result.DBStringValueTimeFormat = (StringTimeFormat)item.DBValueStringTimeFormat;
                else
                    result.DBStringValueTimeFormat = StringTimeFormat.Unknown;

            }
            else
            {
                if (item.DBValueStringTimeFormat != null && (StringTimeFormat)item.DBValueStringTimeFormat != StringTimeFormat.Unknown)
                    result.DBStringValueTimeFormat = (StringTimeFormat)item.DBValueStringTimeFormat;
                else
                {
                    if (item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting != null
                        && (StringTimeFormat)item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.StringTimeFormat != StringTimeFormat.Unknown)
                    {
                        result.DBStringValueTimeFormat = (StringTimeFormat)item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.StringTimeFormat;
                    }
                    else
                        result.DBStringValueTimeFormat = StringTimeFormat.Hours24;
                }
            }

            //result.StringTimeISAMPMFormat = item.StringTimeISAMPMFormat == true;
            //result.ShowAMPMFormat = item.ShowAMPMFormat;
            //     result.ValueIsPersianDate = item.ValueIsPersianDate;
            return result;
        }
        public DateTimeColumnTypeDTO GetDateTimeColumType(int columnID)
        {
            ColumnDTO result = new ColumnDTO();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var column = GetEnabledColumn(projectContext, columnID);
                if (column.DateTimeColumnType != null)
                    return ToDateTimeColumTypeDTO(column.DateTimeColumnType, true);
            }
            return null;
        }
        private DateTimeColumnTypeDTO ToDateTimeColumTypeDTO(DataAccess.DateTimeColumnType item, bool withNullValues)
        {
            //**BizColumn.ToDateTimeColumTypeDTO: 113afb35-1b98-401a-82dd-4bf13b0175a4
            DateTimeColumnTypeDTO result = new DateTimeColumnTypeDTO();
            result.ColumnID = item.ColumnID;
            result.DBValueIsString = item.DBValueIsString;


            if (withNullValues)
            {
                if (item.DBValueStringTimeFormat != null)
                    result.DBStringValueTimeFormat = (StringTimeFormat)item.DBValueStringTimeFormat;
                else
                    result.DBStringValueTimeFormat = StringTimeFormat.Unknown;

                result.ShowMiladiDateInUI = item.ShowMiladiDateInUI;
                result.DBStringValueIsMiladi = item.DBValueIsStringMiladi;
            }
            else
            {
                if (item.DBValueStringTimeFormat != null && (StringTimeFormat)item.DBValueStringTimeFormat != StringTimeFormat.Unknown)
                    result.DBStringValueTimeFormat = (StringTimeFormat)item.DBValueStringTimeFormat;
                else
                {
                    if (item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting != null
                        && (StringTimeFormat)item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.StringTimeFormat != StringTimeFormat.Unknown)
                    {
                        result.DBStringValueTimeFormat = (StringTimeFormat)item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.StringTimeFormat;
                    }
                    else
                        result.DBStringValueTimeFormat = StringTimeFormat.Hours24;
                }

                if (item.ShowMiladiDateInUI != null)
                    result.ShowMiladiDateInUI = item.ShowMiladiDateInUI.Value;
                else
                {
                    if (item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting != null)
                    {
                        result.ShowMiladiDateInUI = item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.ShowMiladiDateInUI;
                    }
                    else
                        result.ShowMiladiDateInUI = false;
                }
                if (item.DBValueIsStringMiladi != null)
                    result.DBStringValueIsMiladi = item.DBValueIsStringMiladi.Value;
                else
                {
                    if (item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting != null)
                    {
                        result.DBStringValueIsMiladi = item.Column.Table.DBSchema.DatabaseInformation.DatabaseUISetting.StringDateColumnIsMiladi;
                    }
                    else
                        result.DBStringValueIsMiladi = false;
                }
            }

            return result;
        }




        //public void ConvertStringTimeColumnToStringColumnType(int columnID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbColumn = projectContext.TimeColumnType.First(x => x.ColumnID == columnID);

        //        if ((Enum_ColumnType)dbColumn.Column.OriginalTypeEnum != Enum_ColumnType.String)
        //        {
        //            throw new Exception("ستون امکان تبدیل به نوع رشته ای ندارد");
        //        }

        //        if (dbColumn.Column.TimeColumnType != null)
        //        {
        //            projectContext.TimeColumnType.Remove(dbColumn.Column.TimeColumnType);
        //            dbColumn.Column.TimeColumnType = null;
        //        }
        //        dbColumn.Column.TypeEnum = Convert.ToByte(Enum_ColumnType.String);
        //        projectContext.SaveChanges();
        //    }
        //}
        //public void ConvertStringDateTimeColumnToStringColumnType(int columnID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbColumn = projectContext.TimeColumnType.First(x => x.ColumnID == columnID);

        //        if ((Enum_ColumnType)dbColumn.Column.OriginalTypeEnum != Enum_ColumnType.String)
        //        {
        //            throw new Exception("ستون امکان تبدیل به نوع رشته ای ندارد");
        //        }

        //        if (dbColumn.Column.TimeColumnType != null)
        //        {
        //            projectContext.TimeColumnType.Remove(dbColumn.Column.TimeColumnType);
        //            dbColumn.Column.TimeColumnType = null;
        //        }
        //        dbColumn.Column.TypeEnum = Convert.ToByte(Enum_ColumnType.String);
        //        projectContext.SaveChanges();
        //    }
        //}


        public void ConvertColumnToStringColumnType(TableDrivedEntityDTO entity, ColumnDTO column)
        {
            //** BizColumn.ConvertColumnToStringColumnType: 8351e66f-a105-44ba-8a5c-7715aa287708
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {

                var dbColumn = projectContext.Column.First(x => x.ID == column.ID);
                RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>() { Enum_ColumnType.String });

                //CreateNewStringColumn(dbColumn, column);

                dbColumn.TypeEnum = Convert.ToByte(Enum_ColumnType.String);

                projectContext.SaveChanges();
            }
        }

        private void RemoveColumnTypes(MyIdeaEntities projectContext, Column dbColumn, List<Enum_ColumnType> exceptionTypes)
        {
            if (!exceptionTypes.Contains(Enum_ColumnType.Numeric))
            {
                if (dbColumn.NumericColumnType != null)
                    projectContext.NumericColumnType.Remove(dbColumn.NumericColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.DateTime))
            {
                if (dbColumn.DateTimeColumnType != null)
                    projectContext.DateTimeColumnType.Remove(dbColumn.DateTimeColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.Date))
            {
                if (dbColumn.DateColumnType != null)
                    projectContext.DateColumnType.Remove(dbColumn.DateColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.Time))
            {
                if (dbColumn.TimeColumnType != null)
                    projectContext.TimeColumnType.Remove(dbColumn.TimeColumnType);
            }
            if (!exceptionTypes.Contains(Enum_ColumnType.String))
            {
                if (dbColumn.StringColumnType != null)
                    projectContext.StringColumnType.Remove(dbColumn.StringColumnType);
            }
        }

        public IEnumerable<Column> GetAllColumns(TableDrivedEntity dbEntity)
        {
            //**835426f1-378b-41df-af1a-76435880babc
            IEnumerable<Column> columns = null;
            if (dbEntity.TableDrivedEntity_Columns.Count > 0)
            {
                //اینجا باید لیست مقادیر هم ست شود
                columns = dbEntity.TableDrivedEntity_Columns.Select(x => x.Column);
            }
            else
            {
                columns = dbEntity.Table.Column;
            }

            columns = columns.Where(x => x.Removed == false);
            return columns;
        }

        public IEnumerable<Column> GetAllEnabledColumns(TableDrivedEntity dbEntity)
        {
            // dc04b084-bb2d-4e9a-9710-b0943ebf3267
            IEnumerable<Column> columns = GetAllColumns(dbEntity).Where(x => x.IsDisabled == false);
            return columns;
        }
        private Column GetEnabledColumn(MyIdeaEntities projectContext, int columnID)
        {
            return projectContext.Column.FirstOrDefault(x => x.ID == columnID && x.Removed == false && x.IsDisabled == false);
        }
        public List<ColumnDTO> GetAllColumnsDTO(int entityID, bool simple)
        {
            List<ColumnDTO> result = new List<ColumnDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                var dbEntity = bizTableDrivedEntity.GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);
                var columns = GetAllColumns(dbEntity);
                foreach (var column in columns)
                    result.Add(ToColumnDTO(column, simple));
            }
            return result;
        }
        public List<ColumnDTO> GetAllEnabledColumnsDTO(int entityID, bool simple)
        {
            List<ColumnDTO> result = new List<ColumnDTO>();
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                var dbEntity = bizTableDrivedEntity.GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);

                var columns = GetAllEnabledColumns(dbEntity);

                foreach (var column in columns)
                    result.Add(ToColumnDTO(column, simple));

            }
            return result;
        }

        //private List<ColumnDTO> GetColumns(TableDrivedEntity item, bool simple)
        //{


        //}

        //public List<ColumnDTO> GetColumnsForEdit(int entityID)
        //{
        //    List<ColumnDTO> result = new List<ColumnDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
        //        ICollection<Column> columns = null;
        //        if (entity.Column.Count > 0)
        //        {
        //            columns = entity.Column;
        //        }
        //        else
        //        {
        //            columns = entity.Table.Column;
        //        }
        //        columns = columns.Where(x => x.IsDBCalculatedColumn == false).ToList();
        //        foreach (var column in columns)
        //            result.Add(ToColumnDTO(column, true));
        //        return result;
        //    }
        //}

        //public List<ColumnDTO> GetColumnsSimple(int entityID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var entity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
        //        if (entity.Column.Count > 0)
        //        {
        //            return GetColumnsSimple(entity.Column);
        //        }
        //        else
        //        {
        //            return GetColumnsSimple(entity.Table.Column);
        //        }
        //    }
        //}

        //private List<ColumnDTO> GetColumnsSimple(ICollection<Column> columns)
        //{
        //    BizColumn bizColumn = new BizColumn();
        //    List<ColumnDTO> result = new List<ColumnDTO>();
        //    foreach (var column in columns)
        //        result.Add(bizColumn.ToColumnDTO(column, true));
        //    return result;
        //}


        //public List<ColumnDTO> GetColums(int tableID, bool simple)
        //{
        //    List<ColumnDTO> result = new List<ColumnDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var list = projectContext.Column.Where(x => x.TableID == tableID);
        //        foreach (var item in list)
        //        {
        //            result.Add(ToColumnDTO(item, simple));
        //        }
        //    }
        //    return result;
        //}
        //public List<ColumnDTO> GetOtherColums(int columnID)
        //{
        //    List<ColumnDTO> result = new List<ColumnDTO>();
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var column = GetColumn(columnID);
        //        var list = column.Table.Column.ToList();
        //        foreach (var item in list)
        //        {
        //            result.Add(ToColumnDTO(item, true));
        //        }
        //    }
        //    return result;
        //}
        public ColumnDTO ToColumnDTO(DataAccess.Column item, bool simple)
        {
            // BizColumn.ToColumnDTO: 3fad23169aeb
            var cachedItem = CacheManager.GetCacheManager().GetCachedItem(CacheItemType.Column, item.ID.ToString(), simple.ToString());
            if (cachedItem != null)
                return (cachedItem as ColumnDTO);

            ColumnDTO result = new ColumnDTO();
            result.Name = item.Name;
            result.DataType = item.DataType;

            result.ID = item.ID;
            result.TableID = item.TableID;
            result.IsNull = item.IsNull;

            result.PrimaryKey = item.PrimaryKey;


            result.ColumnType = (Enum_ColumnType)item.TypeEnum;
            result.OriginalColumnType = (Enum_ColumnType)item.OriginalTypeEnum;

            if (!string.IsNullOrEmpty(item.Alias))
                result.Alias = item.Alias;
            else
                result.Alias = item.Name;
            if (item.RelationshipColumns.Any())
                result.ForeignKey = item.RelationshipColumns.Any(x => x.Relationship.Removed != true && x.Relationship.MasterTypeEnum == (int)Enum_MasterRelationshipType.FromForeignToPrimary);
            result.DataEntryEnabled = item.DataEntryEnabled;
            result.DefaultValue = item.DefaultValue;
            result.IsMandatory = item.IsMandatory;
            result.IsIdentity = item.IsIdentity;
            result.Position = (item.Position == null ? 0 : item.Position.Value);
            result.IsDisabled = item.IsDisabled;
            result.IsNotTransferable = item.IsNotTransferable;

            result.DBCalculateFormula = item.DBCalculateFormula;
            // result.IsDBCalculatedColumn = !string.IsNullOrEmpty(result.DBFormula);
            result.IsReadonly = item.IsReadonly;
            result.DotNetType = GetColumnDotNetType(item.DataType, item.IsNull);

            if (item.ColumnCustomFormula != null)
            {
                result.CustomFormulaName = item.ColumnCustomFormula.Formula.Name;
            }
            if (!simple)
            {
                if (item.StringColumnType != null)
                    result.StringColumnType = ToStringColumTypeDTO(item.StringColumnType);
                if (item.NumericColumnType != null)
                    result.NumericColumnType = ToNumericColumTypeDTO(item.NumericColumnType);
                if (item.DateColumnType != null)
                    result.DateColumnType = ToDateColumTypeDTO(item.DateColumnType, false);
                if (item.TimeColumnType != null)
                    result.TimeColumnType = ToTimeColumTypeDTO(item.TimeColumnType, false);
                if (item.DateTimeColumnType != null)
                    result.DateTimeColumnType = ToDateTimeColumTypeDTO(item.DateTimeColumnType, false);
                BizColumnValueRange bizColumnValueRange = new MyModelManager.BizColumnValueRange();
                if (item.ColumnValueRange != null)
                    result.ColumnValueRange = bizColumnValueRange.ToColumnValueRangeDTO(item.ColumnValueRange, true, true);

                if (item.ColumnCustomFormula != null)
                {
                    result.ColumnCustomFormula = ToColumnCustomFormulaDTO(item.ColumnCustomFormula);
                }

            }
            CacheManager.GetCacheManager().AddCacheItem(result, CacheItemType.Column, item.ID.ToString(), simple.ToString());
            return result;
        }

        private ModelEntites.ColumnCustomFormulaDTO ToColumnCustomFormulaDTO(ColumnCustomFormula item)
        {
            var result = new ModelEntites.ColumnCustomFormulaDTO();
            result.ID = item.ID;
            result.FormulaID = item.FormulaID ?? 0;
            result.CalculateFormulaAsDefault = item.CalculateFormulaAsDefault;
            result.OnlyOnEmptyValue = item.OnlyOnEmptyValue;
            result.OnlyOnNewData = item.OnlyOnNewData;
            result.Formula = new BizFormula().ToFormulaDTO(item.Formula, false);
            return result;
        }

        private bool IsStringType(string datatype)
        {
            return (datatype.Contains("char") || datatype.Contains("text"));
        }
        //public FormulaDTO GetCustomCalculationFormula(int columnID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var column = projectContext.Column.FirstOrDefault(x => x.ID == columnID);
        //        if (column.CustomCalculatedColumn != null)
        //            return new BizFormula().GetFormula(column.CustomCalculatedColumn.FormulaID, false);
        //    }
        //    return null;
        //}



        public void UpdateColumnsFromUI(int entityID, List<ColumnDTO> columns)
        {
            //** BizColumn.UpdateColumnsFromUI: 8ee14cf5-30d9-4af4-afc3-14ecf5010471
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                //   var dbEntity = projectContext.TableDrivedEntity.First(x => x.ID == entityID);
                //  dbEntity.ColumnsReviewed = true;
                foreach (var column in columns)
                {
                    var dbColumn = projectContext.Column.First(x => x.ID == column.ID);
                    dbColumn.Alias = column.Alias;
                    //   dbColumn.DefaultValue = column.DefaultValue;
                    dbColumn.IsMandatory = column.IsMandatory;
                    dbColumn.Position = column.Position;
                    dbColumn.Description = column.Description;
                    if (column.PrimaryKey || column.ForeignKey)
                    {

                    }
                    else
                    {
                        //اینها توسط رابطه انجام می شود
                        dbColumn.IsDisabled = column.IsDisabled;
                        dbColumn.IsReadonly = column.IsReadonly;
                        dbColumn.DataEntryEnabled = column.DataEntryEnabled;
                        dbColumn.IsNotTransferable = column.IsNotTransferable;
                    }

                    //if(dbEntity.IsView)
                    //{
                    //    dbColumn.PrimaryKey = column.PrimaryKey;
                    //}




                    // if (column.CustomFormulaID != 0)
                    // {
                    //     //if (dbColumn.CustomCalculatedColumn == null)
                    //     //    dbColumn.CustomCalculatedColumn = new CustomCalculatedColumn();
                    //     //dbColumn.CustomCalculatedColumn.FormulaID = column.CustomFormulaID;
                    //     //dbColumn.IsCustomColumn = true;
                    // }
                    // else
                    // {
                    ////     dbColumn.IsCustomColumn = false;
                    //     //if (dbColumn.CustomCalculatedColumn != null)
                    //     //{
                    //     //    projectContext.CustomCalculatedColumn.Remove(dbColumn.CustomCalculatedColumn);
                    //     //}
                    // }
                }
                projectContext.SaveChanges();
            }
        }
        public void UpdateStringColumnType(List<StringColumnTypeDTO> columnTypes)
        {
            //**BizColumnUpdateStringColumnType: bf58c975-a36d-4a12-b7a3-1cbf252eff52
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var column in columnTypes)
                {
                    var dbColumn = projectContext.StringColumnType.First(x => x.ColumnID == column.ColumnID);
                    //  dbColumn.MaxLength = column.MaxLength;
                    dbColumn.MinLength = column.MinLength;
                    dbColumn.Format = column.Format;
                }
                projectContext.SaveChanges();
            }
        }

        public void UpdateNumericColumnType(List<NumericColumnTypeDTO> columnTypes)
        {
            //** BizColumn.UpdateNumericColumnType: c99a93a8-756a-42a2-a9f2-d346c3f0299b
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var column in columnTypes)
                {
                    var dbColumn = projectContext.NumericColumnType.First(x => x.ColumnID == column.ColumnID);
                    dbColumn.MaxValue = column.MaxValue;
                    dbColumn.MinValue = column.MinValue;
                    dbColumn.Delimiter = column.Delimiter;
                    //dbColumn.Precision = column.Precision;
                    //dbColumn.Scale = column.Scale;
                }
                projectContext.SaveChanges();
            }
        }

        public bool DataIsAccessable(DR_Requester requester, int columnID, bool checkDataEntry)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbColumn = GetEnabledColumn(projectContext, columnID);
                return DataIsAccessable(requester, dbColumn, checkDataEntry);
            }
        }
        public bool DataIsAccessable(DR_Requester requester, Column column, bool checkDataEntry)
        {
            //** BizColumn.DataIsAccessable: 0c6adb5b0bdc
            SecurityHelper securityHelper = new SecurityHelper();
            if (column.IsDisabled == true)
                return false;
            else
            {
                if (checkDataEntry && !column.DataEntryEnabled)
                {
                    return false;
                }
                else
                {
                    if (requester.SkipSecurity)
                        return true;
                    var permission = securityHelper.GetAssignedPermissions(requester, column.ID, false);
                    if (permission.GrantedActions.Any(y => y == SecurityAction.NoAccess))
                        return false;
                    //else if (isNotReadonlyCheck && permission.GrantedActions.Any(y => y == SecurityAction.ReadOnly))
                    //    return false;
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public bool DataIsReadonly(DR_Requester requester, int columnID, bool? entityIsReadonly, int entityID)
        {
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbColumn = GetEnabledColumn(projectContext, columnID);
                BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                var dbEntity = bizTableDrivedEntity.GetAllEnabledEntities(projectContext).First(x => x.ID == entityID);
                return DataIsReadonly(requester, dbColumn, entityIsReadonly, dbEntity);
            }
        }
        public bool DataIsReadonly(DR_Requester requester, Column column, bool? entityIsReadonly, TableDrivedEntity entity)
        {
            //** BizColumn.DataIsReadonly: 46ee61f89778
            SecurityHelper securityHelper = new SecurityHelper();
            if (entityIsReadonly == null)
            {
                BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
                entityIsReadonly = bizTableDrivedEntity.EntityIsReadonly(requester, entity);
            }
            if (entityIsReadonly.Value)
                return true;
            else
            {
                if (column.IsReadonly == true)
                    return true;
                else
                {
                    if (column.IsIdentity == true)
                        return true;
                    else
                    {
                        if (requester.SkipSecurity)
                            return false;
                        var permission = securityHelper.GetAssignedPermissions(requester, column.ID, false);
                        if (permission.GrantedActions.Any(y => y == SecurityAction.ReadOnly))
                            return true;
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }


        public void UpdateDateColumnType(List<DateColumnTypeDTO> columnTypes)
        {
            //BizColumn.UpdateDateColumnType: 1589bf82d5ee
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var column in columnTypes)
                {
                    var dbColumn = projectContext.DateColumnType.First(x => x.ColumnID == column.ColumnID);
                    dbColumn.ShowMiladiDateInUI = column.ShowMiladiDateInUI;
                    dbColumn.DBValueIsStringMiladi = column.DBStringValueIsMiladi;
                    //if ((Enum_ColumnType)dbColumn.Column.OriginalTypeEnum == Enum_ColumnType.String)
                    //{


                    //}
                    //else
                    //{
                    //    dbColumn.StringDateIsMiladi = null;
                    //    dbColumn.ValueIsString = false;
                    //}
                }
                projectContext.SaveChanges();
            }
        }

        public void UpdateDateTimeColumnType(List<DateTimeColumnTypeDTO> columnTypes)
        {
            // BizColumn.UpdateDateTimeColumnType: 1ac95507-c845-4f42-b8c5-c28d005177f0
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var column in columnTypes)
                {
                    var dbColumn = projectContext.DateTimeColumnType.First(x => x.ColumnID == column.ColumnID);

                    dbColumn.ShowMiladiDateInUI = column.ShowMiladiDateInUI;
                    dbColumn.DBValueIsStringMiladi = column.DBStringValueIsMiladi;
                    dbColumn.DBValueStringTimeFormat = (short)column.DBStringValueTimeFormat;

                    //dbColumn.ShowMiladiDateInUI = column.ShowMiladiDateInUI;
                    //dbColumn.ShowAMPMFormat = column.ShowAMPMFormat;
                    //dbColumn.HideTimePicker = column.HideTimePicker;
                    //if ((Enum_ColumnType)dbColumn.Column.OriginalTypeEnum == Enum_ColumnType.String)
                    //{
                    //    dbColumn.StringDateIsMiladi = column.StringDateIsMiladi;
                    //    dbColumn.StringTimeISAMPMFormat = column.StringTimeISAMPMFormat;
                    //    dbColumn.StringTimeIsMiladi = column.StringTimeIsMiladi;
                    //}
                    //else
                    //{
                    //    dbColumn.StringDateIsMiladi = null;
                    //    dbColumn.StringTimeISAMPMFormat = null;
                    //    dbColumn.StringTimeIsMiladi = null;
                    //}

                }
                projectContext.SaveChanges();
            }
        }

        public void UpdateTimeColumnType(List<TimeColumnTypeDTO> columnTypes)
        {
            //BizColumn.UpdateTimeColumnType: 247a404e04f7
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                foreach (var column in columnTypes)
                {
                    var dbColumn = projectContext.TimeColumnType.First(x => x.ColumnID == column.ColumnID);
                    //if (!column.StringValueIsMiladi)
                    //{
                    //    if ((Enum_ColumnType)dbColumn.Column.OriginalTypeEnum == Enum_ColumnType.Time)
                    //    {
                    //        throw new Exception("ستون از نوع زمان می باشد و گزینه مقدار شمسی به اشتباه انتخاب شده است");
                    //    }
                    //}
                    dbColumn.DBValueStringTimeFormat = (short)column.DBStringValueTimeFormat;
                    //dbColumn.ShowAMPMFormat = column.ShowAMPMFormat;
                    //if ((Enum_ColumnType)dbColumn.Column.OriginalTypeEnum == Enum_ColumnType.String)
                    //{
                    //    dbColumn.StringTimeIsMiladi = column.StringTimeIsMiladi;
                    //    dbColumn.StringTimeISAMPMFormat = column.StringTimeISAMPMFormat;
                    //}
                    //else
                    //{
                    //    dbColumn.StringTimeIsMiladi = null;
                    //    dbColumn.StringTimeISAMPMFormat = null;
                    //}
                }
                projectContext.SaveChanges();
            }
        }

        internal void UpdateColumnsFromTargetDB(TableDrivedEntityDTO entity, Table table, MyIdeaEntities projectContext)
        {
            //**UpdateColumnsFromTargetDB.UpdateColumnsFromTargetDB:  6c99db26-bb1b-40df-ba63-1e9a8bbe5eff
            foreach (var column in entity.Columns)
            {
                Column dbColumn = table.Column.FirstOrDefault(x => x.Removed == false && x.Name == column.Name);
                if (dbColumn == null)
                {
                    dbColumn = new Column();
                    dbColumn.SecurityObject = new SecurityObject();
                    dbColumn.SecurityObject.Type = (int)DatabaseObjectCategory.Column;
                    dbColumn.Name = column.Name;
                    dbColumn.DataEntryEnabled = true;

                    //if (!string.IsNullOrEmpty(column.DBFormula) ||
                    //    (!string.IsNullOrEmpty(column.DefaultValue) && DefaultValueIsDBFunction(column)))
                    //{
                    //    dbColumn.IsReadonly = true;
                    //    //چون اگه تو حالت اصلاح بود بتونه ببینه داده رو

                    //}
                    //else
                    //{
                    //    dbColumn.IsReadonly = false;
                    //}
                    table.Column.Add(dbColumn);
                }
                if (dbColumn.ID == 0)
                {
                    dbColumn.Alias = string.IsNullOrEmpty(column.Alias) ? column.Name : column.Alias;
                    dbColumn.Description = column.Description;
                    dbColumn.IsMandatory = !column.IsNull;
                }

                dbColumn.DataType = column.DataType;
                dbColumn.PrimaryKey = column.PrimaryKey;
                dbColumn.IsNull = column.IsNull;

                dbColumn.IsIdentity = column.IsIdentity;
                dbColumn.Position = column.Position;
                dbColumn.DefaultValue = column.DefaultValue;
                dbColumn.IsDisabled = column.IsDisabled;
                //dbColumn.IsDisabled = false;
                //if (column.OriginalColumnType == Enum_ColumnType.None ||
                //   column.ColumnType == Enum_ColumnType.None)
                //{
                //    throw (new Exception("نوع ستون" + " " + column.Name + " " + "در جدول" + " " + entity.Name + " " + "مشخص نشده است"));
                //}
                if (dbColumn.ID == 0)
                {
                    SetColumnDataTypeFromTargetDB(entity, dbColumn, column);
                }
                else
                {
                    if ((Enum_ColumnType)dbColumn.OriginalTypeEnum != column.OriginalColumnType)
                    {
                        RemoveColumnTypes(projectContext, dbColumn, new List<Enum_ColumnType>());
                        SetColumnDataTypeFromTargetDB(entity, dbColumn, column);
                    }
                    else
                    {
                        SyncColumnTypesDBProperties(dbColumn, column);
                    }
                }





                dbColumn.DBCalculateFormula = column.DBCalculateFormula;

                //dbColumn.ShowNullValue=
                //if (column.IsDBCalculatedColumn)
                //{
                //    dbColumn.IsDBCalculatedColumn = true;
                //    if (dbColumn.DBCalculatedColumn == null)
                //        dbColumn.DBCalculatedColumn = new DBCalculatedColumn();
                //    dbColumn.DBCalculatedColumn.Formula = column.DBFormula;
                //}
                //else
                //{
                //    dbColumn.IsDBCalculatedColumn = false;
                //    if (dbColumn.DBCalculatedColumn != null)
                //        projectContext.DBCalculatedColumn.Remove(dbColumn.DBCalculatedColumn);
                //}
            }
            var columnNames = entity.Columns.Select(x => x.Name).ToList();
            foreach (var dbColumn in table.Column.Where(x => !columnNames.Contains(x.Name)))
            {
                dbColumn.Removed = true;
            }
        }

        private void SyncColumnTypesDBProperties(Column dbColumn, ColumnDTO column)
        {
            //**BizColumn.SyncColumnTypesDBProperties: 608254b4-72f0-419f-a038-ad4ff657a6ff 
            if ((Enum_ColumnType)dbColumn.OriginalTypeEnum == Enum_ColumnType.String)
            {
                dbColumn.StringColumnType.MaxLength = column.StringColumnType.MaxLength;
            }
            else if ((Enum_ColumnType)dbColumn.OriginalTypeEnum == Enum_ColumnType.Numeric)
            {
                dbColumn.NumericColumnType.Precision = column.NumericColumnType.Precision;
                dbColumn.NumericColumnType.Scale = column.NumericColumnType.Scale;
            }
        }

        private void SetColumnDataTypeFromTargetDB(TableDrivedEntityDTO entity, Column dbColumn, ColumnDTO column)
        {
            //BizColumn.CreateNewColumnDataType: fea6234f-fc47-4548-8d93-5e5127e74877
            if (column.ColumnType == Enum_ColumnType.String)
            {
                var dataHelper = new ModelDataHelper();
                if (dbColumn.ID == 0)
                {

                    if (column.StringColumnType.MaxLength <= 50 &&
                                  (column.Name.ToLower().StartsWith("datetime") ||
                                  column.Name.ToLower().EndsWith("datetime"))
                                    )
                    {
                        CreateNewStringDateTimeColumn(entity, dbColumn, column, Enum_ColumnType.DateTime);
                    }
                    else if (column.StringColumnType.MaxLength <= 50 &&
                        (column.Name.ToLower().StartsWith("date") ||
                        column.Name.ToLower().EndsWith("date") ||
                         column.Name.ToLower().StartsWith("tarikh") ||
                          column.Name.ToLower().EndsWith("tarikh"))
                          )
                    {
                        CreateNewStringDateTimeColumn(entity, dbColumn, column, Enum_ColumnType.Date);
                    }
                    else if (column.StringColumnType.MaxLength <= 20 &&
                       (column.Name.ToLower().StartsWith("time") ||
                       column.Name.ToLower().EndsWith("time") ||
                       column.Name.ToLower().StartsWith("time") ||
                       column.Name.ToLower().EndsWith("time") ||
                        column.Name.ToLower().StartsWith("zaman") ||
                         column.Name.ToLower().EndsWith("zaman"))
                         )
                    {
                        CreateNewStringDateTimeColumn(entity, dbColumn, column, Enum_ColumnType.Time);
                    }
                    else
                    {
                        CreateNewStringColumn(dbColumn, column);
                    }
                }
                else
                {
                    CreateNewStringColumn(dbColumn, column);
                }
            }
            else if (column.ColumnType == Enum_ColumnType.Date)
            {
                CreateNewDateTimeColumn(dbColumn, column, Enum_ColumnType.Date);
            }
            else if (column.ColumnType == Enum_ColumnType.Time)
            {
                CreateNewDateTimeColumn(dbColumn, column, Enum_ColumnType.Time);

            }
            else if (column.ColumnType == Enum_ColumnType.DateTime)
            {
                CreateNewDateTimeColumn(dbColumn, column, Enum_ColumnType.DateTime);

            }
            else if (column.ColumnType == Enum_ColumnType.Numeric)
            {
                CreateNewNumericColumn(dbColumn, column);
            }
        }

        private void CreateNewStringColumn(Column dbColumn, ColumnDTO column)
        {
            //**BizColumn.CreateNewStringColumn: c3583c1c-dcc2-42c1-979e-aa893da6b6e7
            dbColumn.OriginalTypeEnum = Convert.ToByte(Enum_ColumnType.String);
            dbColumn.TypeEnum = Convert.ToByte(Enum_ColumnType.String);

            dbColumn.StringColumnType = new StringColumnType();
            SyncColumnTypesDBProperties(dbColumn, column);
        }

        private void CreateNewStringDateTimeColumn(TableDrivedEntityDTO entity, Column dbColumn, ColumnDTO column, Enum_ColumnType columnType)
        {
            //BizColumn.CreateNewStringDateTimeColumn: e740a7d8-1456-422f-b766-2b8bb6df4790
            CreateNewStringColumn(dbColumn, column);
            CreateNewDateTimeColumnOriginallyString(entity, dbColumn, column, columnType);
        }


        private void CreateNewDateTimeColumn(Column dbColumn, ColumnDTO column, Enum_ColumnType columnType)
        {
            //**BizColumn.CreateNewDateTimeColumn: 5a4645cf-0487-48d5-89bb-65590717ebfd
            ModelDataHelper dataHelper = new ModelDataHelper();
            dbColumn.OriginalTypeEnum = Convert.ToByte(columnType);
            dbColumn.TypeEnum = Convert.ToByte(columnType);

            if (columnType == Enum_ColumnType.DateTime)
            {
                dbColumn.DateTimeColumnType = new DateTimeColumnType();
                dbColumn.DateTimeColumnType.DBValueIsString = false;
            }
            else if (columnType == Enum_ColumnType.Date)
            {
                dbColumn.DateColumnType = new DateColumnType();
                dbColumn.DateColumnType.DBValueIsString = false;
            }
            else if (columnType == Enum_ColumnType.Time)
            {
                dbColumn.TimeColumnType = new TimeColumnType();
                dbColumn.TimeColumnType.DBValueIsString = false;
            }
        }
        private void CreateNewDateTimeColumnOriginallyString(TableDrivedEntityDTO entity, Column dbColumn, ColumnDTO column, Enum_ColumnType columnType)
        {
            //**BizColumn.CreateNewDateTimeColumnOriginallyString: 9e272e60f7cb
            ModelDataHelper dataHelper = new ModelDataHelper();
            dbColumn.TypeEnum = Convert.ToByte(columnType);

            if (columnType == Enum_ColumnType.DateTime)
            {
                dbColumn.DateTimeColumnType = new DateTimeColumnType();
                dbColumn.DateTimeColumnType.DBValueIsString = true;
                var value = dataHelper.GetColumnNotNullData(entity, column);
                dbColumn.DateTimeColumnType.DBValueIsStringMiladi = DBValueIsStringMiladi(value);
                dbColumn.DateTimeColumnType.DBValueStringTimeFormat = (short)DBValueStringTimeFormat(value);
            }
            else if (columnType == Enum_ColumnType.Date)
            {
                dbColumn.DateColumnType = new DateColumnType();
                dbColumn.DateColumnType.DBValueIsString = true;
                var value = dataHelper.GetColumnNotNullData(entity, column);
                dbColumn.DateColumnType.DBValueIsStringMiladi = DBValueIsStringMiladi(value);
            }
            else if (columnType == Enum_ColumnType.Time)
            {
                dbColumn.TimeColumnType = new TimeColumnType();
                dbColumn.TimeColumnType.DBValueIsString = true;
                var value = dataHelper.GetColumnNotNullData(entity, column);
                dbColumn.TimeColumnType.DBValueStringTimeFormat = (short)DBValueStringTimeFormat(value);
            }
        }
        public void ConvertStringColumnToDateTimeColumn(TableDrivedEntityDTO entity, ColumnDTO column, Enum_ColumnType columnType)
        {
            //**BizColumn.ConvertStringColumnToDateTimeColumn: 559a48c0-417b-4813-8b9e-b6ad00bdd936
            using (var projectContext = new DataAccess.MyIdeaEntities())
            {
                var dbColumn = projectContext.Column.First(x => x.ID == column.ID);
                CreateNewDateTimeColumnOriginallyString(entity, dbColumn, column, columnType);
                projectContext.SaveChanges();
            }
        }
        private void CreateNewNumericColumn(Column dbColumn, ColumnDTO column)
        {
            //**BizColumn.CreateNewNumericColumn: e64b16d0-bfc0-46fd-9116-67b59e86132f
            ModelDataHelper dataHelper = new ModelDataHelper();
            dbColumn.OriginalTypeEnum = Convert.ToByte(Enum_ColumnType.Numeric);
            dbColumn.TypeEnum = Convert.ToByte(Enum_ColumnType.Numeric);

            dbColumn.NumericColumnType = new NumericColumnType();
            SyncColumnTypesDBProperties(dbColumn, column);
            dbColumn.NumericColumnType.Delimiter = CheckDelimiter(column);

        }
        private bool CheckDelimiter(ColumnDTO column)
        {
            //**9d5217f1-8a54-49a1-a1d2-0ff4dad8df1a
            if (column.Name.ToLower().Contains("price"))
                return true;
            if (column.Alias != null)
            {
                if (column.Alias.ToLower().Contains("price") ||
                         column.Alias.ToLower().Contains("مبلغ"))
                    return true;
            }
            return false;
        }

        //private bool DefaultValueIsDBFunction(ColumnDTO column)
        //{
        //    if (column.DefaultValue != null && column.DefaultValue.Contains("()"))
        //        return true;
        //    else
        //        return false;
        //}
        //private void CheckStringDateColumn(TableDrivedEntityDTO entity, Column dbColumn, ColumnDTO column)
        //{

        //    //  else
        //    {
        //        if (column.ColumnType == Enum_ColumnType.String)
        //        {
        //            if (dbColumn.DateTimeColumnType != null)
        //            {
        //                column.ColumnType = Enum_ColumnType.DateTime;
        //            }
        //            else if (dbColumn.DateColumnType != null)
        //            {
        //                column.ColumnType = Enum_ColumnType.Date;
        //            }
        //            else if (dbColumn.TimeColumnType != null)
        //            {
        //                column.ColumnType = Enum_ColumnType.Time;
        //            }
        //        }
        //    }
        //}

        private StringTimeFormat DBValueStringTimeFormat(object value)
        {
            if (value != null)
            {
                var strValue = value.ToString();
                if (strValue.ToLower().Contains("am") || strValue.ToLower().Contains("pm"))
                    return StringTimeFormat.AMPMMiladi;
                else if (strValue.ToLower().Contains("ق") || strValue.ToLower().Contains("ب"))
                    return StringTimeFormat.AMPMShamsi;
                else
                    return StringTimeFormat.Hours24;
            }
            return StringTimeFormat.Unknown;
        }

        private bool? DBValueIsStringMiladi(object value)
        {
            //4034e423-2440-4466-ac7e-922ad4df7105
            if (value != null)
            {
                var splt = value.ToString().Split('/');
                var strValue = "";
                if (value.ToString().Contains("/"))
                {
                    if (splt.Any(x => x.Length > 2))
                    {
                        strValue = splt.First(x => x.Length > 2);
                    }
                    else
                        strValue = value.ToString();
                }
                else
                    strValue = value.ToString();

                if (strValue.StartsWith("19") || strValue.StartsWith("20"))
                    return true;
                else if (strValue.StartsWith("13") || strValue.StartsWith("14"))
                    return false;
            }
            return null;
        }

        //public void UpdateColumnValueRangeID(int columnID, int columnValueRangeID)
        //{
        //    using (var projectContext = new DataAccess.MyIdeaEntities())
        //    {
        //        var dbColumn = GetColumn(columnID);
        //        dbColumn.ColumnValueRangeID = columnValueRangeID;
        //        projectContext.SaveChanges();
        //    }
        //}



        //private Enum_ColumnType GetColumnType(DataAccess.Column column)
        //{

        //    if (column.StringColumnType != null)
        //    {
        //        return Enum_ColumnType.String;
        //    }
        //    else if (column.NumericColumnType != null)
        //    {
        //        return Enum_ColumnType.Numeric;
        //    }
        //    else if (column.DateColumnType != null)
        //    {
        //        return Enum_ColumnType.Date;
        //    }

        //    return Enum_ColumnType.None;
        //}



    }

}
