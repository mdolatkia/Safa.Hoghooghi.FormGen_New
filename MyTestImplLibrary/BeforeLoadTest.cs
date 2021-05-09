using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    public class BeforeLoadTest
    {
        public FunctionResult SetUserID(CodeFunctionParamManyDataItems datas)
        {
            FunctionResult result = new FunctionResult();
            foreach (var data in datas.DataItems)
            {
                if (data != null)
                {
                    var useridColumn = data.Properties.FirstOrDefault(x => x.Column != null && x.Column.Name.ToLower() == "userid");
                    if (useridColumn != null)
                    {
                        useridColumn.Value = datas.Requester.Identity;
                    }
                }
            }
            return result;
        }
        public FunctionResult EditPersianDateMonthDay(CodeFunctionParamManyDataItems datas)
        {
            FunctionResult result = new FunctionResult();
            foreach (var data in datas.DataItems)
            {
                if (data != null)
                {
                    foreach (var property in data.Properties.Where(x => x.Column != null && x.Column.Name.ToLower().Contains("date")
                    && x.Column.OriginalColumnType == Enum_ColumnType.String))
                    {

                        if (property.Value != null && property.Value.ToString().Length < 10 && property.Value.ToString().Contains("/"))
                        {
                            var splt = property.Value.ToString().Split('/');
                            if (splt.Count() == 3)
                            {
                                string month = splt[1];
                                string day = splt[2];
                                if (month.Length == 1)
                                    month = "0" + month;
                                if (day.Length == 1)
                                    day = "0" + day;
                                var date = splt[0] + "/" + month + "/" + day;
                                property.Value = date;
                            }
                        }
                    }
                }
            }
            return result;
        }
        public FunctionResult EditRealPersonNationalCode(CodeFunctionParamManyDataItems datas)
        {
            FunctionResult result = new FunctionResult();
            foreach (var data in datas.DataItems)
            {
                if (data != null)
                {
                    foreach (var property in data.Properties.Where(x => x.Column != null && x.Column.Name.ToLower() == "nationalcode"
                    && x.Column.OriginalColumnType == Enum_ColumnType.String))
                    {
                        if (property.Value != null && property.Value.ToString().Length < 10)
                        {
                            var len = 10 - property.Value.ToString().Length;
                            string add = "";
                            for (int i = 1; i <= len; i++)
                            {
                                add += "0";
                            }
                            property.Value = add + property.Value.ToString();
                        }
                    }
                }
            }
            return result;
        }
        //public FunctionResult EditNationalCode(CodeFunctionParamManyDataItems datas)
        //{
        //    FunctionResult result = new FunctionResult();
        //    foreach (var data in datas.DataItems)
        //    {
        //        if (data != null)
        //        {
        //            foreach (var property in data.Properties.Where(x => x.Column != null && x.Column.Name.ToLower().Contains("nationalcode")
        //            && x.Column.OriginalColumnType == Enum_ColumnType.String))
        //            {

        //                if (property.Value != null && property.Value.ToString().Length < 10 )
        //                {
        //                    property.Value = "00" + property.Value;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
}
