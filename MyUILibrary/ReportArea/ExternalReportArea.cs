﻿using AutoMapper;
using CommonDefinitions.UISettings;
using ModelEntites;
using MyReportManager;

using MyUILibrary;
using MyUILibrary.EntityArea;
using MyUILibrary.EntityArea.Commands;
using MyUILibrary.EntitySearchArea;
using MyUILibraryInterfaces.EntityArea;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Telerik.Reporting;

namespace MyUILibrary.EntityArea
{
    public class ExternalReportArea : I_ExternalReportArea
    {
        //   ReportResolver reportResolver = new ReportResolver();
        //  EntityReportDTO Report { set; get; }
        //    DP_SearchRepositoryMain InitialSearchRepository { set; get; }
        public bool SecurityNoAccess { set; get; }
        public bool SecurityReadonly { set; get; }
        public bool SecurityEdit { set; get; }
        public I_GeneralEntitySearchArea GeneralEntitySearchArea { set; get; }
        public object MainView { set; get; }
        public ExternalReportArea(ExternalReportAreaInitializer initParam)
        {
            AreaInitializer = initParam;

         

            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfExternalReportArea();
            View.Title = AreaInitializer.Title;


            GeneralEntitySearchAreaInitializer selectAreaInitializer = new GeneralEntitySearchAreaInitializer();
            selectAreaInitializer.ExternalView = View;
            selectAreaInitializer.EntityID = AreaInitializer.EntityID;
            if (AreaInitializer.EntityID != 0)
                selectAreaInitializer.LockEntitySelector = true;
            if (initParam.InitialSearchRepository != null && !initParam.ShowInitializeSearchRepository)
                selectAreaInitializer.PreDefinedSearch = AreaInitializer.InitialSearchRepository;
            GeneralEntitySearchArea = new GeneralEntitySearchArea();
            GeneralEntitySearchArea.SearchDataDefined += GeneralEntitySearchArea_SearchDataDefined;
            GeneralEntitySearchArea.SetInitializer(selectAreaInitializer);

            GeneralEntitySearchArea.EnableDisableSearchArea(AreaInitializer.UserCanChangeSearch);
            MainView = GeneralEntitySearchArea.View;
            //View.AddGenerealSearchAreaView(GeneralEntitySearchArea.View);

            //ManageSecurity();
            //if (!SecurityNoAccess && (SecurityReadonly || SecurityEdit))
            //{
            if (AreaInitializer.InitialSearchRepository != null && initParam.ShowInitializeSearchRepository)
            {
                SetReport(AreaInitializer.InitialSearchRepository);
            }
            //}
        }



        private void GeneralEntitySearchArea_SearchDataDefined(object sender, SearchDataArg e)
        {
            SetReport(e.SearchItems);
        }

        //AssignedPermissionDTO _Permission;
        //public AssignedPermissionDTO Permission
        //{
        //    get
        //    {
        //        if (_Permission == null)
        //            _Permission = AgentUICoreMediator.GetAgentUICoreMediator.SecurityHelper.GetAssignedPermissions(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(),AreaInitializer.EntityID, false);
        //        return _Permission;
        //    }
        //}
        //private void ManageSecurity()
        //{
        //    if (Permission.GrantedActions.Any(x => x == SecurityAction.NoAccess))
        //    {
        //        SecurityNoAccess = true;
        //    }
        //    else
        //    {
        //        if (Permission.GrantedActions.Any(x => x == SecurityAction.EditAndDelete || x == SecurityAction.Edit))
        //        {
        //            SecurityEdit = true;
        //        }
        //        else if (Permission.GrantedActions.Any(x => x == SecurityAction.ReadOnly))
        //        {
        //            SecurityReadonly = true;
        //        }
        //        else
        //            SecurityNoAccess = true;
        //    }
        //    ImposeSecurity();
        //}

        //private void ImposeSecurity()
        //{
        //    if (SecurityNoAccess)
        //    {
        //        View = null;
        //        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به آرشیو");
        //    }
        //    else
        //    {
        //        if (!SecurityReadonly && !SecurityEdit)
        //        {
        //            View = null;
        //            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به آرشیو");

        //        }
        //    }
        //}
        //public bool InitialSearchShouldBeIncluded
        //{
        //    set; get;
        //}
        DP_SearchRepositoryMain SearchRepository { set; get; }
        private void SetReport(DP_SearchRepositoryMain searchRepository)
        {
            SearchRepository = searchRepository;


            if (searchRepository != null)
            {
                //کلید رو میفرسته به عنوان پارامتر گزارش. این کلید در دیتابیس به نام MyExternalReport ذخیره میشود که کوئری را نگه میدارد
                //بعد در ریپورت که پارامتر کلید را دارد این کلید را به یک استورد پروسیجر میتوان فرستاد.این استورد پروسیجر برای هر گزارش باید نوسته شود زیرا باید فیلدهای مخصوصی آن گزارش
                //را برگرداند اما داخل آن از یک استورد پروسیجر دیگر استفاده میشود که عمومی است و کلیدهای اصلی آن کوئری مرتبط با کلید پارامتر ارسال شده را در یک جدول تمپ ذخیره میکند

                //داستان عوض شد.همین برنامه کلیدها رو در پایگاه داده هدف در یک جدول که با حروف
                //xr_
                //شروع میشه اینزرت میکنه.رو دیتای سنگین تست شود که روش خوبی هست یا نه
                var reportKey = AgentUICoreMediator.GetAgentUICoreMediator.ReportManager.GetExternalReportKey(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(),
                    AreaInitializer.ReportID, AreaInitializer.EntityID, searchRepository);
                EntityExternalReportDTO report = AgentUICoreMediator.GetAgentUICoreMediator.ReportManager.GetExternalReport(AreaInitializer.ReportID);

                var url = report.URL;
                if (url.Contains("?"))
                    url += "&ReportKey=" + reportKey;
                else
                    url += "?ReportKey=" + reportKey;

                System.Diagnostics.Process.Start(url);

                //(View as I_View_ExternalReportArea).SetReportSource(url);

            }
            //var rpSource = reportResolver.GetReportSource(request);
            //View.SetReportSource(rpSource);
        }
        public string SerializeObject<T>(object toSerialize, List<Type> types)
        {
            XmlSerializer xmlSerializer = null;
            if (types == null)
                xmlSerializer = new XmlSerializer(toSerialize.GetType());
            else
                xmlSerializer = new XmlSerializer(toSerialize.GetType(), types.ToArray());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
        //private void View_SearchCommandRequested(object sender, EventArgs e)
        //{
        //    if (SearchEntityArea == null)
        //    {
        //        SearchEntityArea = new SearchEntityArea();
        //        var searchViewInitializer = new SearchEntityAreaInitializer();

        //        searchViewInitializer.EntityID = AreaInitializer.EntityID;
        //        SearchEntityArea.SetAreaInitializer(searchViewInitializer);
        //        //SearchEntityArea.GenerateSearchView();
        //        SearchEntityArea.SearchDataDefined += SearchEntityArea_SearchDataDefined;

        //    }
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(SearchEntityArea.SearchView, "جستجو");
        //}
        //private void SearchEntityArea_SearchDataDefined(object sender, SearchDataArg e)
        //{
        //    SetReport(e.SearchItems);

        //}
        //private void SearchEntityArea_SearchDataDefined(object sender, SearchDataArg e)
        //{
        //    //RR_ReportSourceRequest request = new RR_ReportSourceRequest();
        //    //request.ReportID = AreaInitializer.TemplateReport.ID;
        //    //request.SearchDataItems = e.SearchItems;
        //    //var resportSource=  AgentUICoreMediator.GetAgentUICoreMediator.SendReportSourceRequest(request);
        //    //string reportEngine = "engine=RestService;uri=http://localhost:25667/api/myreport;useDefaultCredentials=False";
        //    //var rs = new UriReportSource() { Uri = "aa" };
        //    //AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowTestReport(reportEngine, rs);

        //    ////// DR_SearchViewRequest request = new DR_SearchViewRequest();
        //    ////// request.SearchDataItems = e.SearchItems;
        //    ////// request.EntityID = AreaInitializer.TemplateReport.TableDrivedEntityID;
        //    ////// //اینج کوئری
        //    ////// var reuslt = AgentUICoreMediator.GetAgentUICoreMediator.SendSearchViewRequest(request);

        //    //////ViewEntityArea.ShowReport(reuslt.ResultDataItems, true);
        //}
        //private DP_SearchRepositoryMain FindOrCreateSearchItem(List<DP_SearchRepositoryMain> dataList, DP_SearchRepositoryMain mainItem, EntityRelationshipTailDTO searchRelationshipTail)
        //{
        //    if (searchRelationshipTail == null)
        //        return mainItem;
        //    else
        //    {  //بازنویسی شود
        //       ////var foundItem = dataList.FirstOrDefault(x => x.SourceRelatedData == mainItem && x.SourceRelationID == searchRelationshipTail.RelationshipID);
        //       ////if (foundItem == null)
        //       ////{
        //       ////    //sourcecolumn , target کجا ست میشوند؟؟
        //       ////    //فهمیدم اول کار  CreateDefaultData(); برای هر فرم صدا زده میشود
        //       ////    //بهتر شود که دوباره کاری نشود
        //       ////    foundItem = new DP_SearchRepositoryMain();
        //       ////    foundItem.SourceRelatedData = mainItem;
        //       ////    foundItem.TargetEntityID = searchRelationshipTail.RelationshipTargetEntityID;
        //       ////    foundItem.SourceToTargetRelationshipType = searchRelationshipTail.SourceToTargetRelationshipType;
        //       ////    foundItem.SourceToTargetMasterRelationshipType = searchRelationshipTail.SourceToTargetMasterRelationshipType;
        //       ////    dataList.Add(foundItem);
        //       ////    return FindOrCreateSearchItem(dataList, foundItem, searchRelationshipTail.ChildTail);
        //       ////}
        //       ////else
        //       ////    return foundItem;
        //        return null;

        //    }
        //}

        //private DP_EntityResult GetFullEntity(int entityID2)
        //{
        //    var request = new DP_EntityRequest();
        //    request.EntityID = entityID2;
        //    return AgentUICoreMediator.GetAgentUICoreMediator.GetEntity(request, EntityColumnInfoType.WithFullColumns, EntityRelationshipInfoType.WithRelationships, true, true, true, true, true, true);
        //}
        //private SearchEntityArea GenereateSearchArea()
        //{
        //    var searchEntityArea = new SearchEntityArea();
        //    var searchViewInitializer = new SearchEntityAreaInitializer();
        //    searchViewInitializer.SearchEntity = GetFullEntity(AreaInitializer.TemplateReport.TableDrivedEntityID).Entity;
        //    searchEntityArea.SetAreaInitializer(searchViewInitializer);
        //    searchEntityArea.GenerateSearchView();
        //    return searchEntityArea;
        //}




        public I_View_ExternalReportArea View
        {
            set; get;
        }

        //public List<I_Command> SearchViewCommands
        //{
        //    set; get;
        //}

        //public I_SearchEntityArea SearchEntityArea
        //{
        //    set; get;
        //}



        public ExternalReportAreaInitializer AreaInitializer
        {
            set; get;
        }


        //public void ShowTemporarySearchView()
        //{
        //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(View, AreaInitializer.TemplateReport.ReportTitle);
        //}


    }
}
