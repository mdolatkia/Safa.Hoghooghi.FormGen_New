﻿using ModelEntites;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea
{
    public interface I_EditAreaDataManager
    {
      //  void SerachDataFromParentRelationForChildDataView(ChildRelationshipInfo ChildRelationshipInfo);
     //   void SerachDataFromParentRelationForChildTempView(ChildRelationshipInfo ChildRelationshipInfo);
        DP_DataView GetDataView(DP_DataRepository data);
      //  bool ConvertDataViewToFullData(int entityID, DP_FormDataRepository searchViewData, I_EditEntityArea editEntityArea);
   //     DP_FormDataRepository GetFullDataFromDataViewSearch(int entityID, DP_DataView searchViewData, I_EditEntityArea editEntityArea);
     //   DP_FormDataRepository SearchDataForEditFromExternalSource(int entityID, DP_BaseData searchViewData, I_EditEntityArea editEntityArea);
    //    DP_FormDataRepository SearchDataForViewFromExternalSource(int entityID, DP_BaseData searchViewData, I_EditEntityArea editEntityArea);
      //  DP_FormDataRepository ConvertDP_DataViewToDP_DataRepository(DP_DataView searechedData, I_EditEntityArea editEntityArea);
        //List<DP_DataRepository> SerachDataViewFromParentRelation(int relationshipID, DP_DataRepository parentRelationData);

        //void ConvertDataViewToFullData(int entityID, List<DP_DataRepository> searchViewData, I_EditEntityArea editEntityArea);
        //   List<DP_DataRepository> SearchDataForEditFromExternalSource(int entityID, List<DP_DataRepository> searchViewData, I_EditEntityArea editEntityArea);
        //List<DP_DataRepository> SearchDataForEditFromTempDataViewSearch(int entityID, List<DP_DataRepository> searchViewData, I_EditEntityArea editEntityArea);
        //List<DP_DataView> SearchDataForViewFromExternalSource(int entityID, List<DP_DataRepository> searchViewData, I_EditEntityArea editEntityArea);

        //List<DP_DataRepository> GetFullDataFromDataViewSearch(int entityID, List<DP_DataView> searchViewData, I_EditEntityArea editEntityArea);
        //List<DP_DataRepository> ConvertDP_DataViewToDP_DataRepository(List<DP_DataView> simpledata, I_EditEntityArea editEntityArea);
    }
}
