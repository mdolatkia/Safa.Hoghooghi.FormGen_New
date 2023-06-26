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

namespace MyUILibrary.EntitySelectArea
{
    public class GeneralEntityDataSelectArea : I_GeneralEntityDataSelectArea
    {
        MySearchLookup entitySearchLookup;
        public EntityDataSelectAreaInitializer EntityDataSelectAreaInitializer { set; get; }
        public TableDrivedEntityDTO Entity { set; get; }

        public I_View_GeneralEntityDataSelectArea View
        {
            set; get;
        }
        I_EditEntityAreaOneData SelectDataArea
        {
            set; get;
        }

        public void SetAreaInitializer(EntityDataSelectAreaInitializer entityDataSelectAreaInitializer)
        {
            //تو این ویوی کاربردی مثلا آرشیو به این اصلی اضافه میشه ولی تو جنرال سرچ ویو اینجا به کاربردی مثلا دیتا ویو اضافه میشه. کدوم بهتره؟ اصلاح بشه
            EntityDataSelectAreaInitializer = entityDataSelectAreaInitializer;
            View = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GenerateViewOfGeneralEntityDataSelectArea();

            if (EntityDataSelectAreaInitializer.EntityID != 0)
            {
                Entity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetSimpleEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), EntityDataSelectAreaInitializer.EntityID);


                View.SetSelectorTitle(Entity.Alias);
                EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
                editEntityAreaInitializer1.EntityID = Entity.ID;
                editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
                editEntityAreaInitializer1.DataMode = CommonDefinitions.UISettings.DataMode.One;
                editEntityAreaInitializer1.EntityListViewID = EntityDataSelectAreaInitializer.EntityListViewID;
                var FirstSideEditEntityAreaResult = BaseEditEntityArea.GetEditEntityArea(editEntityAreaInitializer1);
                if (FirstSideEditEntityAreaResult.Item1 != null && FirstSideEditEntityAreaResult.Item1 is I_EditEntityAreaOneData)
                {
                    SelectDataArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
                    SelectDataArea.DataItemSelected += FirstSideEditEntityArea_DataItemSelected;
                    SelectDataArea.DataItemsCleared += SelectDataArea_DataItemsCleared;
                    View.AddSelector(SelectDataArea.TemporaryDisplayView);
                    if (EntityDataSelectAreaInitializer.LockDataSelector)
                        SelectDataArea.FirstView.EnableDisable(false);
                }
                if (EntityDataSelectAreaInitializer.DataItem != null && firstTime)
                {
                    SelectDataArea.SelectData(new List<DP_BaseData>() { EntityDataSelectAreaInitializer.DataItem });
                }
                firstTime = false;

            }

        }

        public TableDrivedEntityDTO SelectedEntity
        {
            get
            {
                return entitySearchLookup.SelectedItem as TableDrivedEntityDTO;
            }
        }


        bool firstTime = true;


        public DP_FormDataRepository SelectedData
        {
            get
            {
                if (SelectDataArea != null && SelectDataArea.GetDataList().Any())
                    return SelectDataArea.GetDataList().First();
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
