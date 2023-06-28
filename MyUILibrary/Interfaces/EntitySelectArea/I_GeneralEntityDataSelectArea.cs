using MyUILibrary.EntityArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyLibrary;
using ModelEntites;
using CommonDefinitions.UISettings;

namespace MyUILibraryInterfaces.EntityArea
{
    public interface I_GeneralEntityDataSelectArea
    {
        event EventHandler<List<DP_FormDataRepository>> DataItemChanged;
        void SetAreaInitializer(EntityDataSelectAreaInitializer entityDataSelectAreaInitializer);
        TableDrivedEntityDTO Entity { set; get; }
        EntityDataSelectAreaInitializer EntityDataSelectAreaInitializer { set; get; }
        I_View_GeneralEntityDataSelectArea View { set; get; }
        DP_FormDataRepository SelectedData { get; }
        I_EditEntityAreaOneData DataArea { set; get; }


    }
    public interface I_GeneralEntityDataSearchArea
    {
        void SetAreaInitializer(EntityDataSearchAreaInitializer entityDataSelectAreaInitializer);
        event EventHandler<DP_SearchRepositoryMain> SearchRepositoryChanged;
        TableDrivedEntityDTO Entity { set; get; }
        EntityDataSearchAreaInitializer EntityDataSearchAreaInitializer { set; get; }
        I_View_GeneralEntityDataSelectArea View { set; get; }

    }
    public interface I_View_GeneralEntityDataSelectArea
    {
        void AddSelector(object view);
        void SetSelectorTitle(string title);
    }

    public class EntityDataSelectAreaInitializer
    {
        public EntityDataSelectAreaInitializer()
        {

        }

        public bool HideSearchRepository { set; get; }
        public DataMode DataMode { set; get; }
        public int EntityID { set; get; }

        public bool LockDataSelector { set; get; }
        public DP_DataView DataItem { set; get; }
        // public Enum_EntityDataPurpose EntityDataPurpose { set; get; }
        public int EntityListViewID { set; get; }
        public int EntitySearchID { get; internal set; }
    }
    public class EntityDataSearchAreaInitializer
    {
        public bool UserCanChangeSearchRepository { get; internal set; }
        public bool SearchInitially { get; internal set; }

        public EntityDataSearchAreaInitializer()
        {

        }

        public bool HideSearchRepository { set; get; }
        public int EntityID { set; get; }
        //public int EntityListViewID { set; get; }
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
