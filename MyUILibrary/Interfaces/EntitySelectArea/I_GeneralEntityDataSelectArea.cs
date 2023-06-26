using MyUILibrary.EntityArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLibrary;
using ModelEntites;


namespace MyUILibraryInterfaces.EntityArea
{
    public interface I_GeneralEntityDataSelectArea
    {
        event EventHandler<List<DP_FormDataRepository>> DataItemChanged;
        void SetAreaInitializer(EntityDataSelectAreaInitializer entityDataSelectAreaInitializer);
        event EventHandler<DP_SearchRepositoryMain> SearchRepositoryChanged;
        event EventHandler<int?> EntityChanged;

        EntityDataSelectAreaInitializer EntityDataSelectAreaInitializer { set; get; }
        I_View_GeneralEntityDataSelectArea View { set; get; }
        TableDrivedEntityDTO SelectedEntity { get; }
        DP_FormDataRepository SelectedData { get; }
        //    void EnableDisableSelectArea(bool v);
        void SelectData(DP_DataView dataInstance);
    }

    public interface I_View_GeneralEntityDataSelectArea
    {
        void AddEntitySelectorArea(object view);
        void SetEntitySelectorTitle(string title);
        void AddDataSelector(object view);
        void SetDataSelectorTitle(string title);

        void RemoveDataSelector();
        void RemoveEntitySelector();
        void SetSearchRepositoyTitle(string title);
        void RemoveSearchRepositoy();
        void AddSearchRepository(I_View_SearchEntityArea searchView);
    }

    public class EntityDataSelectAreaInitializer
    {
        public bool UserCanChangeSearchRepository { get; internal set; }
        public bool SearchInitially { get; internal set; }

        public EntityDataSelectAreaInitializer(Enum_EntityDataPurpose entityDataPurpose)
        {
            EntityDataPurpose = entityDataPurpose;
        }
        public bool HideEntitySelector
        {
            set; get;
        }
        public bool HideSearchRepository { set; get; }
        public bool LockEntitySelector { set; get; }
        public int EntityID { set; get; }
        //public bool HideDataSelector { set; get; }
        public bool LockDataSelector { set; get; }
        public DP_DataView DataItem { set; get; }
        public Enum_EntityDataPurpose EntityDataPurpose { set; get; }
        public int EntityListViewID { set; get; }
        public DP_SearchRepositoryMain AdvancedSearchDTOMessage { get; internal set; }
        public PreDefinedSearchDTO PreDefinedSearchMessage { get; internal set; }
        public int EntitySearchID { get; internal set; }
    }
    public enum Enum_EntityDataPurpose
    {
        SelectEntity,
        SelectData,
        SearchRepository
    }
}
