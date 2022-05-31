using ModelEntites;
using MyModelManager;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDataSearchManagerBusiness
{
    public class DataSearchStateManager
    {
        private int TargetEntityID;
        public List<EntityStateDTO> EntityStates { get; private set; }
        public DataSearchStateManager()
        {
        }
            public DataSearchStateManager(DR_Requester requester, int targetEntityID)
        {
            TargetEntityID = targetEntityID;
            BizEntityState bizEntityState = new BizEntityState();
            var allEntityStates = bizEntityState.GetEntityStates(requester, targetEntityID, true);
            List<EntityStateDTO> result = new List<EntityStateDTO>();
            foreach (var state in allEntityStates.Where(x => x.ActionActivities.Any()))
            {
                if (state.ActionActivities.Any(x => x.UIEnablityDetails.Any(y => EditArea.AreaInitializer.SourceRelationColumnControl != null && y.RelationshipID == EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID)))
                {
                    //bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || (EditArea is I_EditEntityAreaOneData && EditArea.DataItemIsInTempViewMode(dataItem));
                    //چرا اینجا تمپ ویو هم باشه وضعیت حساب میشه؟
                    bool dataIsInValidMode = dataItem.DataIsInEditMode() || dataItem.DataItemIsInTempViewMode();
                    if (dataIsInValidMode)
                        result.Add(state);
                }
                else
                {
                    if (dataItem.DataIsInEditMode())
                        result.Add(state);
                }
                //}
            }
        }
        private List<EntityStateDTO> GetAppliableStates()
        {

            //List<EntityStateGroupDTO> resultGroup = new List<EntityStateGroupDTO>();
            //foreach (var group in EditArea.EntityStateGroups.Where(x => x.ActionActivities.Any()))
            //{

            //    if (skipUICheck)
            //        resultGroup.Add(group);
            //    else
            //    {
            //        if (group.EntityStates.Any(z => z.ActionActivities.Any(x => x.UIEnablityDetails.Any(y => EditArea.AreaInitializer.SourceRelationColumnControl != null && y.RelationshipID == EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID))))
            //        {
            //            //bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || (EditArea is I_EditEntityAreaOneData && EditArea.DataItemIsInTempViewMode(dataItem));
            //            bool dataIsInValidMode = EditArea.DataItemIsInEditMode(dataItem) || EditArea.DataItemIsInTempViewMode(dataItem);
            //            if (dataIsInValidMode)
            //                resultGroup.Add(group);
            //        }
            //        else
            //        {
            //            if (EditArea.DataItemIsInEditMode(dataItem))
            //                resultGroup.Add(group);
            //        }
            //    }

            //}
            return result;
        }


        var appliableStates = GetAppliableStates(dataItem);
            foreach (var state in appliableStates)
            {
                if (CheckEntityState(dataItem, state))
                    item.EntityStates1.Add(state);
            }
}
}
