using ModelEntites;

using MyDataEditManagerBusiness;
using MyDataManagerBusiness;
using MyDataSearchManagerBusiness;

using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDataManager
{
    public class RequestRegistrationService : IAG_RequestManager
    {
        //     RequestManager requestManager = new RequestManager();
        //public DR_ResultRequest RecieveRequest(DR_Request request)
        //{
        //    var requestManager = new RequestProcessManager();
        //    return requestManager.ProcessRequest(request);
        //}

        public DR_ResultEdit SendEditRequest(DR_EditRequest request)
        {
            EditRequestManager requestManager = new MyDataEditManagerBusiness.EditRequestManager();
            return requestManager.ProcessEditRequest(request);
        }

        public DR_ResultDelete SendDeleteRequest(DR_DeleteRequest request)
        {
            EditRequestManager requestManager = new EditRequestManager();
            return requestManager.ProcessDeleteRequest(request as DR_DeleteRequest);
        }
        public DR_DeleteInquiryResult SendDeleteInquiryRequest(DR_DeleteInquiryRequest request)
        {
            EditRequestManager requestManager = new EditRequestManager();
            return requestManager.Process(request as DR_DeleteInquiryRequest);
        }
        public DR_ResultSearchView SendSearchViewRequest(DR_SearchViewRequest request)
        {
            // RequestRegistrationService.SendSearchViewRequest: 29b1b425fff4
            SearchRequestManager editProcessor = new SearchRequestManager();
            return editProcessor.ProcessSearchViewRequest(request as DR_SearchViewRequest);
        }
     
        public DR_ResultSearchExists SendSearchExistsRequest(DR_SearchExistsRequest request)
        {
            SearchRequestManager editProcessor = new SearchRequestManager();
            return editProcessor.Process(request as DR_SearchExistsRequest);
        }
        public DR_ResultSearchCount SendSearchCountRequest(DR_SearchCountRequest request)
        {
            SearchRequestManager editProcessor = new SearchRequestManager();
            return editProcessor.Process(request as DR_SearchCountRequest);
        }

        public DR_ResultSearchView SendSearchEditViewRequest(DR_SearchEditViewRequest request)
        {
            SearchRequestManager editProcessor = new SearchRequestManager();
            return editProcessor.ProcessSearchViewRequest(request);
        }

        public DR_ResultSearchFullData SendSearchEditRequest(DR_SearchEditRequest request)
        {
            // RequestManagerService.SendSearchEditRequest: a0a0ef868831
            SearchRequestManager editProcessor = new SearchRequestManager();
            return editProcessor.ProcessSearchEditRequest(request as DR_SearchEditRequest);
        }
        public DR_ResultSearchFullData SendSearchFullDataRequest(DR_SearchFullDataRequest request)
        {
            SearchRequestManager editProcessor = new SearchRequestManager();
            return editProcessor.Process(request as DR_SearchFullDataRequest);
        }
        public DR_ResultSearchKeysOnly SendSearchKeysOnlyRequest(DR_SearchKeysOnlyRequest request)
        {
            SearchRequestManager editProcessor = new SearchRequestManager();
            return editProcessor.Process(request as DR_SearchKeysOnlyRequest);
        }
        //public DR_ResultSearchByRelatinoshipTail SendSearchByRelationshipTail(DR_SearchByRelationshipTailRequest request)
        //{
        //    SearchRequestManager editProcessor = new SearchRequestManager();
        //    return editProcessor.Process(request as DR_SearchByRelationshipTailRequest);
        //}

        //public DR_ResultSearchViewByRelatinoshipTail SendSearchViewByRelationshipTail(DR_SearchViewByRelationshipTailRequest request)
        //{
        //    SearchRequestManager editProcessor = new SearchRequestManager();
        //    return editProcessor.Process(request as DR_SearchViewByRelationshipTailRequest);
        //}
        //public RR_ReportSourceResult SendReportSourceRequest(RR_ReportSourceRequest request)
        //{
        //    return requestManager.ProcessRequest(request) as RR_ReportSourceResult;
        //}


        //DR_ResultByRelationView SendSearchByRelationRequest(DR_SearchByRelationViewRequest request)
        //{
        //}



    }
}
