using MyUILibraryInterfaces.EntityArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUILibrary.EntityArea;
using ProxyLibrary;
using MyCommonWPFControls;
using ModelEntites;
using MyUILibrary.EntityArea.Commands;
using CommonDefinitions.UISettings;

namespace MyUILibrary.EntitySelectArea
{
    public class GeneralEntityDataSelectArea : I_GeneralEntityDataSelectArea
    {
        // MySearchLookup entitySearchLookup;
        public GeneralEntityDataSelectArea()
        {
            // GeneralEntityDataSelectArea: 2f06dab7eaed
        }
        public EntityDataSelectAreaInitializer EntityDataSelectAreaInitializer { set; get; }
        public TableDrivedEntityDTO Entity { set; get; }

        public I_View_GeneralEntityDataSelectArea View
        {
            set; get;
        }
        public I_EditEntityAreaOneData DataArea
        {
            set; get;
        }

        public void SetAreaInitializer(EntityDataSelectAreaInitializer areaInitializer)
        {
            // GeneralEntityDataSelectArea.SetAreaInitializer: df37d2b675e8
            //تو این ویوی کاربردی مثلا آرشیو به این اصلی اضافه میشه ولی تو جنرال سرچ ویو اینجا به کاربردی مثلا دیتا ویو اضافه میشه. کدوم بهتره؟ اصلاح بشه
            EntityDataSelectAreaInitializer = areaInitializer;
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfGeneralEntityDataSelectArea();

            if (EntityDataSelectAreaInitializer.EntityID != 0)
            {
                Entity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetSimpleEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EntityDataSelectAreaInitializer.EntityID);


                View.SetSelectorTitle(Entity.Alias);
                EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
                editEntityAreaInitializer1.EntityID = Entity.ID;
                editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                editEntityAreaInitializer1.DataMode = (EntityDataSelectAreaInitializer.DataMode == DataMode.Multiple | EntityDataSelectAreaInitializer.DataMode == DataMode.Multiple) ? DataMode.Multiple : DataMode.One;
                editEntityAreaInitializer1.EntityListViewID = EntityDataSelectAreaInitializer.EntityListViewID;
                var FirstSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer1);
                if (FirstSideEditEntityAreaResult.Item1 != null && FirstSideEditEntityAreaResult.Item1 is I_EditEntityAreaOneData)
                {
                    DataArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
                    DataArea.DataItemSelected += FirstSideEditEntityArea_DataItemSelected;
                    DataArea.DataItemsCleared += SelectDataArea_DataItemsCleared;
                    View.AddSelector(DataArea.TemporaryDisplayView);
                    if (EntityDataSelectAreaInitializer.LockDataSelector)
                        DataArea.FirstView.EnableDisable(false);
                }
                if (EntityDataSelectAreaInitializer.DataItem != null && firstTime)
                {
                    DataArea.SelectData( EntityDataSelectAreaInitializer.DataItem );
                }
                firstTime = false;

            }

        }

        //public TableDrivedEntityDTO SelectedEntity
        //{
        //    get
        //    {
        //        return entitySearchLookup.SelectedItem as TableDrivedEntityDTO;
        //    }
        //}


        bool firstTime = true;


        public DP_FormDataRepository SelectedData
        {
            get
            {
                if (DataArea != null && DataArea.GetDataList().Any())
                    return DataArea.GetDataList().First();
                else
                    return null;
            }
        }



        private void SelectDataArea_DataItemsCleared(object sender, EventArgs e)
        {
            if (DataItemChanged != null)
                DataItemChanged(this, null);
        }
        public event EventHandler<List<DP_FormDataRepository>> DataItemChanged;


        private void FirstSideEditEntityArea_DataItemSelected(object sender, List<DP_FormDataRepository> e)
        {
            if (DataItemChanged != null)
                DataItemChanged(this, e);
        }





    }
}
