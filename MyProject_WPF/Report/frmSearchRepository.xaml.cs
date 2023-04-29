using ModelEntites;
using MyCommonWPFControls;
using MyModelManager;
using MyUIGenerator;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmSearchEntityArea.xaml
    /// </summary>
    /// 

    public partial class frmSearchRepository : UserControl
    {
        PreDefinedSearchDTO PreDefinedSearchMessage { set; get; }
        AdvancedSearchDTO AdvancedSearchDTOMessage { set; get; }
        EntityDefinedSearchArea EntityDefinedSearchArea;
        AdvancedSearchEntityArea AdvancedSearchEntityArea;
        BizSearchRepository bizSearchRepository = new BizSearchRepository();
        public event EventHandler<EntityPreDefinedSearchUpdatedArg> EntityPreDefinedSearchUpdated;
        int EntityID { set; get; }
        //int SearchEntityID { set; get; }
        public frmSearchRepository(int entityID, int savedSearchRepositoryID)
        {
            InitializeComponent();
            EntityID = entityID;

            MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.SetUIManager(new UIManager());
            var userInfo = new MyUILibrary.UserInfo();
            userInfo.AdminSecurityInfo = new MyUILibrary.AdminSecurityInfo() { IsActive = true, ByPassSecurity = true };
            MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.UserInfo = userInfo;

            SetEntitySearchList();
            lokEntitySearch.SelectionChanged += LokEntitySearch_SelectionChanged;
            CreateAdvanceSearchArea();

            if (savedSearchRepositoryID != 0)
            {
                GetSearchRepositoryh(savedSearchRepositoryID);
            }
            else
            {
                SetNewItem();
            }

        }

        private void CreateAdvanceSearchArea()
        {
            var searchViewInitializer = new SearchAreaInitializer();
            searchViewInitializer.EntityID = EntityID;
            searchViewInitializer.ForSave = true;
            AdvancedSearchEntityArea = new AdvancedSearchEntityArea(searchViewInitializer);
            AdvancedSearchEntityArea.FormulaSelectionRequested += EntityDefinedSearchArea_FormulaSelectionRequested;
            grdAdvancedSearchView.Children.Add(AdvancedSearchEntityArea.AdvancedSearchView as UIElement);

        }

        private void LokEntitySearch_SelectionChanged(object sender, SelectionChangedArg e)
        {
            //** 2c7b2d28-f3af-4d68-9ca9-9b6218afd23c
            grdPreDefinedView.Children.Clear();
            if (lokEntitySearch.SelectedItem != null)
            {
                int entitySearchID = lokEntitySearch.SelectedItem == null ? 0 : (int)lokEntitySearch.SelectedValue;

                var searchViewInitializer = new SearchAreaInitializer();
                searchViewInitializer.EntityID = EntityID;
                searchViewInitializer.ForSave = true;
                searchViewInitializer.EntitySearchID = entitySearchID;
                EntityDefinedSearchArea = new EntityDefinedSearchArea(searchViewInitializer);
                EntityDefinedSearchArea.FormulaSelectionRequested += EntityDefinedSearchArea_FormulaSelectionRequested;
                grdPreDefinedView.Children.Add(EntityDefinedSearchArea.SimpleSearchView as UIElement);
            }

        }

        private void EntityDefinedSearchArea_FormulaSelectionRequested(object sender, SimpleSearchColumnControl e)
        {
            frmFormulaSelectGeneral view = new frmFormulaSelectGeneral();
            view.FormulaSelected += (sender1, e1) => View_FormulaSelected(sender1, e1, e);
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "Form", Enum_WindowSize.Maximized);
        }

        private void View_FormulaSelected(object sender, GeneralFormulaSelectedArg e, SimpleSearchColumnControl simpleSearchColumnControl)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            if (e.Formula != null)
            {
                simpleSearchColumnControl.AddSimpleColumnFormula(e.Formula);
            }
        }

        private void GetSearchRepositoryh(int id)
        {
            var message = bizSearchRepository.GetSavedSearchRepository(id);
            optPreDefined.IsEnabled = false;
            optAdvanced.IsEnabled = false;
            if (message.IsPreDefinedOrAdvanced)
            {
                PreDefinedSearchMessage = bizSearchRepository.GetPreDefinedSearch(MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), id, true);
                ShowPreDefinedSearchMessage();
            }
            else
            {
                AdvancedSearchDTOMessage = bizSearchRepository.GetAdvancedSearch(MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), id, true);
                ShowAdvancedSearchMessage();
            }

        }

        private void SetEntitySearchList()
        {
            if (lokEntitySearch.ItemsSource == null)
            {
                lokEntitySearch.EditItemClicked += LokEntitySearch_EditItemClicked; ;
            }
            BizEntitySearch biz = new BizEntitySearch();
            lokEntitySearch.DisplayMember = "Title";
            lokEntitySearch.SelectedValueMember = "ID";
            lokEntitySearch.ItemsSource = biz.GetEntitySearchs(MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EntityID);
        }

        private void LokEntitySearch_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            var lookup = (sender as MyStaticLookup);
            frmEntitySearch view;
            if (lookup.SelectedItem == null)
            {
                view = new frmEntitySearch(EntityID, 0);
            }
            else
            {
                view = new frmEntitySearch(EntityID, (int)lookup.SelectedValue);
            }
            view.EntitySearchUpdated += (sender1, e1) => View_EntitySearchUpdated(sender1, e1, lookup);
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تنظیمات نامه");
        }
        private void View_EntitySearchUpdated(object sender, EntitySearchUpdatedArg e, MyStaticLookup lookup)
        {
            SetEntitySearchList();
            lookup.SelectedValue = e.ID;
        }


        private void ShowPreDefinedSearchMessage()
        {


            txtTitle.Text = PreDefinedSearchMessage.Title;
            optPreDefined.IsChecked = true;
            lokEntitySearch.SelectedValue = PreDefinedSearchMessage.EntitySearchID;
            EntityDefinedSearchArea.ShowPreDefinedSearch(PreDefinedSearchMessage);
        }
        private void ShowAdvancedSearchMessage()
        {
            txtTitle.Text = AdvancedSearchDTOMessage.Title;
            optAdvanced.IsChecked = true;
            AdvancedSearchEntityArea.ShowSearchRepository(AdvancedSearchDTOMessage.SearchRepositoryMain);
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtTitle.Text == "")
            {
                MessageBox.Show("عنوان جستجو الزامی می باشد");
                return;
            }

            //if (SearchEntityArea.IsSimpleSearchActiveOrAdvancedSearch)
            //{
            //  Message.IsSimpleSearch = true;
            //   Message.SimpleColumns = SearchEntityArea.SimpleSearchEntityArea.GetSearchColumns();
            //  Message.SearchRepository = null;

            if (optPreDefined.IsChecked == true)
            {
                var message = EntityDefinedSearchArea.GetSearchRepositoryForSave();
                if (!message.SimpleSearchProperties.Any() && !message.RelationshipSearchProperties.Any())
                {
                    MessageBox.Show("مقدار پیش فرضی مشخص نشده است");
                    return;
                }
                if (lokEntitySearch.SelectedValue == null)
                {
                    MessageBox.Show("نوع جستجو مشخص نشده است");
                    return;
                }
                message.Title = txtTitle.Text;
                message.EntityID = EntityID;
                message.EntitySearchID = (int)lokEntitySearch.SelectedValue;
                message.ID = PreDefinedSearchMessage.ID;
                PreDefinedSearchMessage.ID = bizSearchRepository.UpdatePreDefinedSearch(message);
                MessageBox.Show("اطلاعات ثبت شد");

                if (EntityPreDefinedSearchUpdated != null)
                    EntityPreDefinedSearchUpdated(this, new MyProject_WPF.EntityPreDefinedSearchUpdatedArg() { ID = PreDefinedSearchMessage.ID });
            }
            else if (optAdvanced.IsChecked == true)
            {
                AdvancedSearchDTOMessage.Title = txtTitle.Text;
                AdvancedSearchDTOMessage.EntityID = EntityID;
                AdvancedSearchDTOMessage.SearchRepositoryMain = AdvancedSearchEntityArea.GetSearchRepository();
                AdvancedSearchDTOMessage.ID = bizSearchRepository.UpdateAdvancedSearch(AdvancedSearchDTOMessage);
                MessageBox.Show("اطلاعات ثبت شد");

                if (EntityPreDefinedSearchUpdated != null)
                    EntityPreDefinedSearchUpdated(this, new MyProject_WPF.EntityPreDefinedSearchUpdatedArg() { ID = AdvancedSearchDTOMessage.ID });
            }


            // }
            // else
            // {
            ////     Message.SimpleColumns = new List<PreDefinedSearchColumns>();
            //   //  Message.IsSimpleSearch = false;
            //     Message.SearchRepository = SearchEntityArea.AdvancedSearchEntityAre.GetSearchRepository();
            // }
            //if (lokEntitySearch.SelectedItem != null)
            //    Message.EntitySearchID = (int)lokEntitySearch.SelectedValue;
            //else
            //    Message.EntitySearchID = 0;


        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(this);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            SetNewItem();

        }

        private void SetNewItem()
        {
            //Message = new DP_SearchRepositoryMain(EntityID);
            //Message.Title = "جستجوی جدید";
            //ShowMessage();
            optPreDefined.IsEnabled = true;
            optAdvanced.IsEnabled = true;

            optPreDefined.IsChecked = true;
            txtTitle.Text = "";
            lokEntitySearch.SelectedValue = 0;
            PreDefinedSearchMessage = new PreDefinedSearchDTO();
            AdvancedSearchDTOMessage = new AdvancedSearchDTO();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            frmSearchRepositoryList view = new MyProject_WPF.frmSearchRepositoryList(EntityID);
            view.PreDefinedSearchSelected += View_PreDefinedSearchSelected;
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "frmEntityDataViewReportSelect");
        }

        private void View_PreDefinedSearchSelected(object sender, SearchRepositorySelectedArg e)
        {
            if (e.SearchRepositoryID != 0)
            {
                GetSearchRepositoryh(e.SearchRepositoryID);
            }
        }



        private void optPreDefined_Checked(object sender, RoutedEventArgs e)
        {
            if (optAdvanced.IsChecked == true)
            {
                tabAdvanced.IsSelected = true;
                tabPreDefined.IsEnabled = false;


            }
            else if (optPreDefined.IsChecked == true)
            {
                tabPreDefined.IsSelected = true;
                tabAdvanced.IsEnabled = false;


            }
        }
    }
    public class EntityPreDefinedSearchUpdatedArg : EventArgs
    {
        public int ID { set; get; }
    }
}
