using ModelEntites;
using MyCommonWPFControls;
using MyModelManager;
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
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmEntityChartReport.xaml
    /// </summary>
    /// 

    public partial class frmEntitySearchableReport : UserControl
    {
        private EntitySearchableReportDTO Message { set; get; }
        public event EventHandler<EntityListViewDTO> EntityListViewChanged;
        int EntityID { set; get; }
        public frmEntitySearchableReport()
        {
            InitializeComponent();
        }
        public void SetEntity(int entityID)
        {
            EntityID = entityID;

            lokSearchRepository.EditItemClicked += LokEntityPreDefined_EditItemClicked;
            lokEntitySearch.EditItemClicked += LokEntitySearch_EditItemClicked;
            lokEntitySearch.SelectionChanged += LokEntitySearch_SelectionChanged;
            lokEntityListView.EditItemClicked += LokEntityListView_EditItemClicked;

            SetEntityListViews();
            SetEntitySearches();

        }

        internal void EntityListViewVisiblity(bool visible)
        {
            lokEntityListView.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            lblEntityListView.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetEntitySearches()
        {

            BizEntitySearch biz = new BizEntitySearch();
            lokEntitySearch.DisplayMember = "Title";
            lokEntitySearch.SelectedValueMember = "ID";
            lokEntitySearch.ItemsSource = biz.GetEntitySearchs(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
        }

        private void LokEntitySearch_SelectionChanged(object sender, SelectionChangedArg e)
        {
            SetSearchRepositoryList();
        }

        private void LokEntitySearch_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
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
            view.EntitySearchUpdated += (sender1, e1) => View_EntitySearchViewUpdated(sender1, e1, lookup);
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تنظیمات نامه", Enum_WindowSize.Big);
        }
        private void View_EntitySearchViewUpdated(object sender, EntitySearchUpdatedArg e, MyStaticLookup lookup)
        {
            SetEntitySearches();
            lookup.SelectedValue = e.ID;
        }
        private void SetSearchRepositoryList()
        {
            if (lokEntitySearch.SelectedItem != null && lokEntitySearch.SelectedItem is EntitySearchDTO)
            {
                var entitySearch = lokEntitySearch.SelectedItem as EntitySearchDTO;
                BizSearchRepository biz = new BizSearchRepository();
                lokSearchRepository.DisplayMember = "Title";
                lokSearchRepository.SelectedValueMember = "ID";
                lokSearchRepository.ItemsSource = biz.GetSearchRepositoriesBySearchID(entitySearch.ID);
            }
            else
            {
                lokSearchRepository.ItemsSource = null;
            }
        }
        private void LokEntityPreDefined_EditItemClicked(object sender, EditItemClickEventArg e)
        {
            var lookup = (sender as MyStaticLookup);
            frmSearchRepository view;

            view = new frmSearchRepository(EntityID, (int)lokEntitySearch.SelectedValue, (int)lookup.SelectedValue);
            view.EntityPreDefinedSearchUpdated += (sender1, e1) => View_EntityPreDefinedSearchUpdated(sender1, e1, lookup);
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تنظیمات نامه", Enum_WindowSize.Maximized);

        }

        private void View_EntityPreDefinedSearchUpdated(object sender, EntityPreDefinedSearchUpdatedArg e, MyStaticLookup lookup)
        {
            SetSearchRepositoryList();
            lookup.SelectedValue = e.ID;
        }
        private void SetEntityListViews()
        {

            BizEntityListView biz = new BizEntityListView();
            lokEntityListView.DisplayMember = "Title";
            lokEntityListView.SelectedValueMember = "ID";
            lokEntityListView.ItemsSource = biz.GetEntityListViews(MyProjectManager.GetMyProjectManager.GetRequester(), EntityID);
        }
        private void LokEntityListView_EditItemClicked(object sender, MyCommonWPFControls.EditItemClickEventArg e)
        {
            var lookup = (sender as MyStaticLookup);
            frmEntityListView view;
            if (lookup.SelectedItem == null)
            {
                view = new frmEntityListView(EntityID, 0);
            }
            else
            {
                view = new frmEntityListView(EntityID, (int)lookup.SelectedValue);
            }
            view.EntityListViewUpdated += (sender1, e1) => View_EntityListViewUpdated(sender1, e1, lookup);
            MyProjectManager.GetMyProjectManager.ShowDialog(view, "تنظیمات نامه", Enum_WindowSize.Big);
        }
        private void View_EntityListViewUpdated(object sender, EntityListViewUpdatedArg e, MyStaticLookup lookup)
        {
            SetEntityListViews();
            lookup.SelectedValue = e.ID;
        }
        private void LokEntityListView_SelectionChanged(object sender, SelectionChangedArg e)
        {
            if (lokEntityListView.SelectedItem != null)
            {
                if (EntityListViewChanged != null)
                    EntityListViewChanged(this, lokEntityListView.SelectedItem as EntityListViewDTO);
            }
        }


        public void ShowMessage(EntitySearchableReportDTO message)
        {
            txtTitle.Text = Message.ReportTitle;
            lokEntityListView.SelectedValue = message.EntityListViewID;
            lokEntitySearch.SelectedValue = message.EntitySearchID;
            lokSearchRepository.SelectedValue = message.SearchRepositoryID;
        }
        public bool FillMessage(EntitySearchableReportDTO message)
        {
            if (lokEntityListView.SelectedItem == null)
            {
                MessageBox.Show("لطفا ابتدا لیست ستونها را ثبت و انتخاب نمایید");
                return false;
            }
            if (txtTitle.Text == "")
            {
                MessageBox.Show("عنوان مناسب تعریف نشده است");
                return false;
            }

            Message.ReportTitle = txtTitle.Text;

            message.EntityListViewID = (int)lokEntityListView.SelectedValue;
            message.EntitySearchID = lokEntitySearch.SelectedValue == null ? 0 : (int)lokEntitySearch.SelectedValue;
            message.SearchRepositoryID = lokSearchRepository.SelectedValue == null ? 0 : (int)lokSearchRepository.SelectedValue;
            return true;
        }

    }
}
