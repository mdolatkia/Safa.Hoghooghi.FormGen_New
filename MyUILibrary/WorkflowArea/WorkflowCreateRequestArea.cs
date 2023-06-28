using MyDataManagerService;
using MyGeneralLibrary;

using MyUILibrary.EntityArea;
using MyUILibrary.EntitySelectArea;
using MyUILibrary.Temp;
using MyUILibraryInterfaces.EntityArea;
using MyWorkflowService;
using ProxyLibrary;
using ProxyLibrary.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.WorkflowArea
{
    public class WorkflowCreateRequestArea : WorkflowTransitionTargetSelectionArea, I_WorkflowCreateRequestArea
    {
        OrganizationManagerService organizationManager = new OrganizationManagerService();
        public I_View_WorkflowRequestCreator View { set; get; }
        List<ProcessDTO> ProcessList { set; get; }
        public WorkflowCreateRequestArea()
        {
            ProcessList = AgentUICoreMediator.GetAgentUICoreMediator.workflowService.GetProcessesForExecution(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester());
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetWorkflowReauestCreationForm(ProcessList);

            View.WorkflowRequestCreate += View_WorkflowRequestCreate;
            View.ProcessSelected += View_ProcessSelected;
            View.StateSelected += View_StateSelected;
            //View.CurrentUserOrganizationPostChanged += View_OrganizationPostChanged;
            View.AddTargetSelectionView(ViewTargetSelection);
            View.CloseRequested += View_CloseRequested;
            //View.WorkflowRequesterChanged += View_WorkflowRequesterChanged;
            //View.WorkflowAdminSearched += View_WorkflowAdminSearched;
            //View.WorkflowAdminSelected += View_WorkflowAdminSelected;
            //View.WorkflowStackholderSearched += View_WorkflowStackholderSearched;
            //View.WorkflowStackholderSelected += View_WorkflowStackholderSelected;
            View.Date = DateTime.Today;
            var posts = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester().Posts;
            View.CreatorOrganizationPosts = posts;
            if (posts.Count == 1)
                View.CurrentUserSelectedOrganizationPost = posts.First();

            CheckOutgoingTransitionActions(this);
        }

        private void View_CloseRequested(object sender, EventArgs e)
        {
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(sender);
        }

        //private void View_OrganizationPostChanged(object sender, OrganizationPostDTO e)
        //{

        //}

        I_EditEntityAreaOneData SearchEditEntityArea;
        private void View_ProcessSelected(object sender, ProcessSelectedArg e)
        {
            //** WorkflowCreateRequestArea.View_ProcessSelected: 016da5fcba02
            var states = AgentUICoreMediator.GetAgentUICoreMediator.workflowService.GetProcessInitializeStates(e.ProcessID);
            View.States = states;
            if (states.Count == 1)
            {
                View.SelectedStateID = states.First().ID;
            }
            var proccess = ProcessList.First(x => x.ID == e.ProcessID);
            if (proccess.EntityID != 0)
            {
                EntityDataSelectAreaInitializer selectAreaInitializer = new EntityDataSelectAreaInitializer();
                selectAreaInitializer.EntityID = proccess.EntityID;
                var editEntityArea = new GeneralEntityDataSelectArea();
                editEntityArea.DataItemChanged += EditEntityArea_DataItemChanged;
                editEntityArea.SetAreaInitializer(selectAreaInitializer);
                (sender as I_View_WorkflowRequestCreator).SetDataSelector(editEntityArea.View);

            }
            else
            {
                View.RemoveDataSelector();
            }
            CheckWorkflowTitle();
            //var adminRoles = workflowService.GetProcessAdminRoles(e.ProcessID);
            //(sender as I_View_WorkflowRequestCreator).AdminRoles = adminRoles;
            //(sender as I_View_WorkflowRequestCreator).RequesterRoles = GetRequester().Roles;

        }

        private void EditEntityArea_DataItemChanged(object sender, List<DP_FormDataRepository> e)
        {
            CheckWorkflowTitle();
        }

      

        private void CheckWorkflowTitle()
        {
            if (View.Title == "")
            {
                if (View.SelectedProcess != null && SearchEditEntityArea != null && SearchEditEntityArea.GetDataList().Any())
                {
                    View.Title = View.SelectedProcess.Name + " " + "برای" + " " + SearchEditEntityArea.GetDataList().First().ViewInfo;
                }
            }
        }

        private void View_WorkflowRequestCreate(object sender, EventArgs e)
        {
            CreateRequestDTO requestMessage = new CreateRequestDTO();
            if (string.IsNullOrEmpty(View.Title))
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("عنوان مشخص نشده است");
                return;
            }
            if (SearchEditEntityArea != null)
            {

                DP_DataView dataItem = SearchEditEntityArea.GetDataList().FirstOrDefault();
                if (dataItem == null)
                {
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("موجودیتی انتخاب نشده است");
                    return;
                }
                else
                {
                    requestMessage.DatItem = dataItem;
                }
            }
            if (!ValidateTargetSelection())
                return;
            string confirmMessage = "در مرحله بعد اقدامات توسط این اشخاص امکانپذیر است";
            foreach (var item in ViewTargetSelection.OutgoingTransitoinActions)
            {
                confirmMessage += (Environment.NewLine) + "اقدام" + " : " + item.Title;
                foreach (var post in item.OrganizationPosts.Where(x => x.Selected))
                {
                    confirmMessage += (Environment.NewLine) + "    " + post.OrganizationPost.Name;
                }
            }

            if (AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowConfirm("تایید مرحله بعد", confirmMessage, UserPromptMode.YesNo) == Temp.ConfirmResul.No)
            {
                return;
            }
            requestMessage.OutgoingTransitoinActions = ViewTargetSelection.OutgoingTransitoinActions;
            requestMessage.Title = View.Title;
            //requestMessage.Desc = View.Description;
            requestMessage.ProcessID = View.SelectedProcess.ID;
            requestMessage.CurrentStateID = View.SelectedStateID;
            //requestMessage.RequestFiles = View.RequestFiles;
            requestMessage.RequestNotes = View.RequestNotes;

            //if (view.ProcessAdmins != null)
            //    e.Request.AdminPostIDs = view.ProcessAdmins.Select(x => x.ID).ToList();
            //if (view.ProcessStackholders != null)
            //    e.Request.StackHolderPostIDs = view.ProcessStackholders.Select(x => x.ID).ToList();

            var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();

            var result = AgentUICoreMediator.GetAgentUICoreMediator.workflowService.CreateWorkflowRequest(requestMessage, requester);

            if (result.Result == Enum_DR_ResultType.SeccessfullyDone)
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("جریان کار ایجاد شد");
                if (SearchEditEntityArea != null)
                    SearchEditEntityArea.ClearData();
                View.Title = "";
            }
            else
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowMessage("جریان کار ایجاد نشد" + Environment.NewLine + result.Message);
          
         
        }



        //private void View_WorkflowStackholderSearched(object sender, WorkflowStackholderSearchArg e)
        //{
        //    var view = sender as I_View_WorkflowRequestCreator;
        //    view.StackholderSearchResult = organizationManager.GetOrganizationPosts(e.Search);
        //}
        private void View_StateSelected(object sender, StateSelectedArg e)
        {
            CheckOutgoingTransitionActions(this);
        }


        //private void View_WorkflowRequesterChanged(object sender, WorkflowRequesterChangedArg e)
        //{
        //    var view = (sender as I_View_WorkflowRequestCreator);
        //    CheckOutgoingTransitionActions(view);
        //}

        //private void View_WorkflowAdminSelected(object sender, WorkflowAdminSelectedArg e)
        //{
        //    var view = (sender as I_View_WorkflowRequestCreator);
        //    if (view.ProcessAdmins == null)
        //        view.ProcessAdmins = new List<OrganizationPostDTO>();
        //    view.ProcessAdmins.AddRange(e.Posts);
        //    CheckOutgoingTransitionActions(view);
        //}

        //private void View_WorkflowAdminSearched(object sender, WorkflowAdminSearchArg e)
        //{
        //    var view = sender as I_View_WorkflowRequestCreator;
        //    var process = view.SelectedProcess;
        //    if (process != null)
        //    {
        //        var fullProcess = AgentUICoreMediator.GetAgentUICoreMediator.workflowService.GetProcessAdmins(process.ID);
        //        view.AdminSearchResult = organizationManager.GetOrganizationPostsByRoleTypeIds(e.Search, fullProcess);
        //    }
        //}
        //private void View_WorkflowStackholderSelected(object sender, WorkflowStackholderSelectedArg e)
        //{
        //    var currentStackholder = (sender as I_View_WorkflowRequestCreator).ProcessStackholders;
        //    if (View.ProcessStackholders == null)
        //        View.ProcessStackholders = new List<OrganizationPostDTO>();
        //    View.ProcessStackholders.AddRange(e.Posts);
        //}

    }
}
