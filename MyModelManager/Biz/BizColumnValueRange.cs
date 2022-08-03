using DataAccess;
using ModelEntites;
using MyCacheManager;
using MyGeneralLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizColumnValueRange
    {
        //public ColumnValueRangeDTO GetColumnValueRange(column)
        //{
        //    ColumnDTO result = new ColumnDTO();
        //    using (var projectContext = new DataAccess.MyProjectEntities())
        //    {
        //        var column = projectContext.ColumnValueRange.First(x => x.ID == id);
        //        return ToColumnValueRangeDTO(column);
        //    }
        //}

        public void RemoveColumnValueRangeID(int columnID)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var dbColumnValueRange = projectContext.ColumnValueRange.FirstOrDefault(x => x.ID == columnID);
                if (dbColumnValueRange != null)
                {
                    while (dbColumnValueRange.ColumnValueRangeDetails.Any())
                        projectContext.ColumnValueRangeDetails.Remove(dbColumnValueRange.ColumnValueRangeDetails.First());
                    projectContext.ColumnValueRange.Remove(dbColumnValueRange);
                    projectContext.SaveChanges();
                }
            }
        }

        public ColumnValueRangeDTO GetColumnValueRange(int columnID)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var column = projectContext.ColumnValueRange.FirstOrDefault(x => x.ID == columnID);
                if (column != null)
                    return ToColumnValueRangeDTO(column, false, false);
                else
                    return null;
            }
        }
        public List<string> GetColumnValueRangeValues(int columnValueRangeID, EnumColumnValueRangeTag tag)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var columnValueRange = projectContext.ColumnValueRange.FirstOrDefault(x => x.ID == columnValueRangeID);
                if (tag == EnumColumnValueRangeTag.Title)
                {
                    return columnValueRange.ColumnValueRangeDetails.Select(x => x.KeyTitle).Distinct().ToList();
                }
                else if (tag == EnumColumnValueRangeTag.Value)
                {
                    return columnValueRange.ColumnValueRangeDetails.Select(x => x.Value).Distinct().ToList();
                }
                else if (tag == EnumColumnValueRangeTag.Tag1)
                {
                    return columnValueRange.ColumnValueRangeDetails.Select(x => x.Tag).Distinct().ToList();
                }
                //else if (tag == EnumColumnValueRangeTag.Tag2)
                //{
                //    return columnValueRange.ColumnValueRangeDetails.Select(x => x.Tag2).Distinct().ToList();
                //}

            }
            return null;
        }

        //public ColumnValueRangeDTO GetColumnKeyValue(int columnID)
        //{
        //    //////ColumnValueRangeDTO result = new ColumnValueRangeDTO();
        //    //////using (var projectContext = new DataAccess.MyProjectEntities())
        //    //////{
        //    //////    var column = projectContext.Column.FirstOrDefault(x => x.ID == columnID);
        //    //////    if (column.ColumnValueRange != null)
        //    //////        return ToColumnKeyValueDTO(column.ColumnValueRange);
        //    //////}
        //    return null;
        //}

        public ColumnValueRangeDTO ToColumnValueRangeDTO(DataAccess.ColumnValueRange item, bool titleIsValueIfEmpty, bool withDetails)
        {
            ColumnValueRangeDTO result = new ColumnValueRangeDTO();
            //       result.ValueFromTitleOrValue = item.ValueFromTitleOrValue;
            result.Details = new List<ColumnValueRangeDetailsDTO>();
            result.ID = item.ID;
            BizEntityRelationshipTail bizEntityRelationshipTail = new BizEntityRelationshipTail();
            result.EntityRelationshipTailID = item.EntityRelationshipTailID ?? 0;
            if (withDetails && result.EntityRelationshipTailID != 0)
                result.EntityRelationshipTail = bizEntityRelationshipTail.ToEntityRelationshipTailDTO(item.EntityRelationshipTail);
            result.TagColumnID = item.TagColumnID ?? 0;
            foreach (var rItem in item.ColumnValueRangeDetails)
            {
                var keyValueRange = new ColumnValueRangeDetailsDTO();
                keyValueRange.ColumnValueRangeID = rItem.ColumnValueRangeID;
                keyValueRange.ID = rItem.ID;
                keyValueRange.Value = rItem.Value;
                keyValueRange.KeyTitle = rItem.KeyTitle;
                if (titleIsValueIfEmpty && string.IsNullOrEmpty(keyValueRange.KeyTitle))
                    keyValueRange.KeyTitle = keyValueRange.Value;
                keyValueRange.Tag = rItem.Tag;
             //   keyValueRange.Tag2 = rItem.Tag2;


                result.Details.Add(keyValueRange);
            }
            return result;
        }
        public bool UpdateColumnValueRange(ColumnValueRangeDTO message)
        {
            using (var projectContext = new DataAccess.MyProjectEntities())
            {
                var dbColumn = projectContext.Column.First(x => x.ID == message.ID);

                if (dbColumn.ColumnValueRange == null)
                {
                    dbColumn.ColumnValueRange = new ColumnValueRange();
                    //  projectContext.ColumnValueRange.Add(dbColumnValueRange);
                }
                dbColumn.ColumnValueRange.EntityRelationshipTailID = message.EntityRelationshipTailID == 0 ? (int?)null : message.EntityRelationshipTailID;
                dbColumn.ColumnValueRange.TagColumnID = message.TagColumnID == 0 ? (int?)null : message.TagColumnID;

                //else
                //    dbColumnValueRange = projectContext.ColumnValueRange.First(x => x.ID == message.ID);
                //     dbColumnValueRange.ID = message.ID;
                //dbColumn.ColumnValueRange.ValueFromTitleOrValue = message.ValueFromTitleOrValue;
                while (dbColumn.ColumnValueRange.ColumnValueRangeDetails.Any())
                    projectContext.ColumnValueRangeDetails.Remove(dbColumn.ColumnValueRange.ColumnValueRangeDetails.First());
                foreach (var keyValueRange in message.Details)
                {
                    dbColumn.ColumnValueRange.ColumnValueRangeDetails.Add(new ColumnValueRangeDetails() { Value = keyValueRange.Value, KeyTitle = keyValueRange.KeyTitle, Tag = keyValueRange.Tag});
                }
                //dbColumnValueRange.Name = message.Name;
                projectContext.SaveChanges();
                return true;
            }
        }



    }

}
