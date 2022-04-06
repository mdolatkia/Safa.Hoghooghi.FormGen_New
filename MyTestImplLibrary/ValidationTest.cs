using ModelEntites;
using MyDataSearchManagerBusiness;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary
{
    public class ValidationTest
    {
        public FunctionResult ValidateServiceRequest(CodeFunctionParamOneDataItem data)
        {
            FunctionResult result = new FunctionResult();
            var productItemRel = data.DataItem.ChildRelationshipDatas.FirstOrDefault(x => x.Relationship.Entity2Alias.Contains("کالا"));
            if (productItemRel != null)
            {
                if (productItemRel.RelatedData.Any())
                {
                    var dataItem = productItemRel.RelatedData.First();
                    if (!dataItem.IsFullData)
                    {
                        SearchRequestManager requestManager = new SearchRequestManager();

                        DP_SearchRepository SearchDataItem = new DP_SearchRepository(dataItem.TargetEntityID);
                        foreach (var col in dataItem.KeyProperties)
                        {
                            SearchDataItem.Phrases.Add(new SearchProperty() { ColumnID = col.ColumnID, Value = col.Value });
                        }
                        var requester = new DR_Requester() { SkipSecurity = true };
                        var request = new DR_SearchFullDataRequest(requester, SearchDataItem);
                        var resultSearch = requestManager.Process(request);
                        dataItem = resultSearch.ResultDataItems[0];
                    }
                    var property = dataItem.Properties.FirstOrDefault(x => x.Column.Name.ToLower().Contains("brand"));
                    if (property != null)
                        if (property.Value != null && property.Value.ToString().ToLower() == "Hisense".ToLower())
                        {
                            result.Result = false;
                            result.Exception = new Exception("امکان استفاده از برند انتخاب شده وجود ندارد");
                        }

                }

            }
            result.Result = true;
            return result;
        }
    }
}
