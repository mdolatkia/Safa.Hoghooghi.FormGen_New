using CommonDefinitions.UISettings;
using ModelEntites;
using MyUILibrary;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUILibrary.EntityArea.Commands
{

    class SaveCommand : BaseCommand
    {
        I_ConfirmUpdate ConfirmUpdateForm = null;
        I_EditEntityArea EditArea { set; get; }
        List<DP_DataRepository> Datas { set; get; }
        public SaveCommand(I_EditEntityArea editArea) : base()
        {
            EditArea = editArea;
            //if (AgentHelper.GetAppMode() == AppMode.Paper)
            //    CommandManager.SetTitle("Save");
            //else
            CommandManager.SetTitle("ذخیره");
            CommandManager.ImagePath = "Images//save.png";
            CommandManager.Clicked += CommandManager_Clicked;
        }
        private void CommandManager_Clicked(object sender, EventArgs e)
        {
            //Enabled = false;
            //try
            //{
            var updateresult = EditArea.UpdateDataAndValidate(EditArea.GetDataList());
            if (!updateresult.IsValid)
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(updateresult.Message, "", MyUILibrary.Temp.InfoColor.Red);
                return;
            }
            //SetFKProperties();
            Datas = GetDatas().ToList();
            if (Datas.Count > 0)
            {
                if (ConfirmUpdateForm == null)
                {
                    ConfirmUpdateForm = AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetConfirmUpdateForm();
                    ConfirmUpdateForm.Decided += ConfirmUpdateForm_Decided;
                    ConfirmUpdateForm.DateTreeRequested += ConfirmUpdateForm_DateTreeRequested;
                }
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(ConfirmUpdateForm, "تایید", Enum_WindowSize.None, true);
            }
            else
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(EditArea.SimpleEntity.Alias + " : " + "داده ای جهت ورود اطلاعات موجود نمیباشد", "", MyUILibrary.Temp.InfoColor.Red);
            }
            //////}
            //////else
            //////{
            //////    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(EditArea.AreaInitializer.Title + " : " + "داده های ورودی معتبر نمیباشند", "", MyUILibrary.Temp.InfoColor.Red);
            //////}
            //}
            //catch (Exception ex)
            //{
            //    var mesage = ex.Message;
            //    mesage += (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : "");
            //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(EditArea.AreaInitializer.Title + " : " + "خطا در عملیات", mesage, MyUILibrary.Temp.InfoColor.Red);
            //}
            //Enabled = true;
        }



        private void ConfirmUpdateForm_DateTreeRequested(object sender, EventArgs e)
        {

            //فانکشنه جدا شود و کامل شود
            var datatree = EditArea.AreaInitializer.EntityAreaLogManager.GetLogDataTree(Datas);
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.GetDialogWindow().ShowDialog(datatree, "درخت داده", Enum_WindowSize.Big);
        }



        private void ConfirmUpdateForm_Decided(object sender, ConfirmUpdateDecision e)
        {
            if (e.Confirm)
            {
                var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                DR_EditRequest request = new DR_EditRequest(requester);
                //اینجا
                //request.EditPackages = Datas;

                var reuslt = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendEditRequest(request);
                if (reuslt.Result == Enum_DR_ResultType.SeccessfullyDone)
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(EditArea.SimpleEntity.Alias + " : " + "عملیات ثبت با موفقیت انجام شد", reuslt.Details, MyUILibrary.Temp.InfoColor.Green);
                else if (reuslt.Result == Enum_DR_ResultType.JustMajorFunctionDone)
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(EditArea.SimpleEntity.Alias + " : " + "عملیات ثبت با موفقیت انجام شد اما برخی عملیات جانبی کامل انجام نشد", reuslt.Details, MyUILibrary.Temp.InfoColor.Blue);
                else if (reuslt.Result == Enum_DR_ResultType.ExceptionThrown)
                    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo(EditArea.SimpleEntity.Alias + " : " + "عملیات ثبت با خطا همراه بود", reuslt.Details, MyUILibrary.Temp.InfoColor.Red);

                if (reuslt.Result == Enum_DR_ResultType.SeccessfullyDone
                    || reuslt.Result == Enum_DR_ResultType.JustMajorFunctionDone)
                {
                    //List<DP_DataView> list = new List<DP_DataView>();
                    //foreach (var item in reuslt.UpdatedItems)
                    //{
                    //    list.Add(new DP_DataView() { })
                    //}
                    EditArea.ClearData();
                    EditArea.ShowDataFromExternalSource(reuslt.UpdatedItems);

                    //DP_SearchRepository searchDataItem = new DP_SearchRepository(EditArea.AreaInitializer.EntityID);
                    //foreach (var item in reuslt.UpdatedItems)
                    //{
                    //    var listProperties = new List<EntityInstanceProperty>();
                    //    LogicPhraseDTO logicPhrase = new LogicPhraseDTO();
                    //    foreach (var keyProperty in item.KeyProperties)
                    //        logicPhrase.Phrases.Add(new SearchProperty() { ColumnID = keyProperty.ColumnID, Value = keyProperty.Value });
                    //    searchDataItem.AndOrType = AndOREqualType.Or;
                    //    searchDataItem.Phrases.Add(logicPhrase);
                    //}
                    /////   var requestSearchEdit = new DR_SearchEditRequest(requester, searchDataItem, EditArea.AreaInitializer.SecurityReadOnly, true);
                    //var requestSearchEdit = new DR_SearchEditRequest(requester, searchDataItem);
                    //var results = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchEditRequest(requestSearchEdit);
                    //if (results.ResultDataItems.Count > 0)
                    //{
                    //    //if (EditArea is I_EditEntityAreaOneData)
                    //    //{
                    //    //    (EditArea as I_EditEntityAreaOneData).ClearData(false);

                    //    //    var data = results.ResultDataItems[0];
                    //    //    data.DataView = EditArea.AreaInitializer.EditAreaDataManager.GetDataView(data);
                    //    //    DP_FormDataRepository orgData = new DP_FormDataRepository(data, EditArea);
                    //    //    var addResult = (EditArea as I_EditEntityAreaOneData).AddData(orgData, true);
                    //    //    if (!addResult)
                    //    //        AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", results.ResultDataItems[0].ViewInfo, Temp.InfoColor.Red);
                    //    //}
                    //    //else if (EditArea is I_EditEntityAreaMultipleData)
                    //    //{
                    //    EditArea.ClearData();
                    //    foreach (var data in results.ResultDataItems)
                    //    {
                    //        data.DataView = EditArea.AreaInitializer.EditAreaDataManager.GetDataView(data);
                    //        DP_FormDataRepository orgData = new DP_FormDataRepository(data, EditArea, false, false);
                    //        var addResult = (EditArea as I_EditEntityAreaMultipleData).AddData(orgData);
                    //        if (!addResult)
                    //            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده و یا داده های وابسته", results.ResultDataItems[0].ViewInfo, Temp.InfoColor.Red);
                    //    }
                    //}
                    //}
                    //else
                    //{
                    //    AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("عدم دسترسی به داده", "", Temp.InfoColor.Red);
                    //    EditArea.ClearData();
                    //}
                }
            }
            AgentUICoreMediator.GetAgentUICoreMediator.UIManager.CloseDialog(sender);
        }

        //private void SetFKProperties(ChildRelationshipInfo parentChildRelationshipInfo = null)
        //{
        //    List<DP_FormDataRepository> sourceList = null;

        //    if (parentChildRelationshipInfo == null)
        //        sourceList = EditArea.GetDataList().ToList();
        //    else
        //        sourceList = parentChildRelationshipInfo.RelatedData.ToList();
        //    foreach (var dataItem in sourceList)
        //    {
        //        foreach (var childRel in dataItem.ChildRelationshipDatas.Where(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary))
        //        {
        //            foreach (var fkprop in childRel.Relationship.RelationshipColumns.Select(x => x.FirstSideColumnID))
        //            {
        //                var prop = dataItem.GetProperty(fkprop);
        //                if (prop != null)
        //                    prop.ISFK = true;
        //            }
        //        }
        //        if (parentChildRelationshipInfo != null && parentChildRelationshipInfo.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
        //        {
        //            foreach (var fkprop in parentChildRelationshipInfo.Relationship.RelationshipColumns.Select(x => x.SecondSideColumnID))
        //            {
        //                var prop = dataItem.GetProperty(fkprop);
        //                if (prop != null)
        //                    prop.ISFK = true;
        //            }
        //        }
        //        foreach (var child in dataItem.ChildRelationshipDatas)
        //            SetFKProperties(child);
        //    }
        //}

        private ObservableCollection<DP_DataRepository> GetDatas()
        {
            List<DP_FormDataRepository> sourceList = EditArea.GetDataList().ToList();
            ObservableCollection<DP_DataRepository> result = new ObservableCollection<DP_DataRepository>();
            foreach (var item in sourceList)
            {
                if (item.ShoudBeCounted)
                    result.Add(GetData(item));
            }

            //انیجا
            //  RemoveUnwantedItems(result);
            return result;

        }
        private DP_DataRepository GetData(DP_FormDataRepository item)
        {
            DP_DataRepository newItem = new DP_DataRepository(item.TargetEntityID, item.TargetEntityAlias);
            //////foreach (var property in item.GetProperties())
            //////{
            //////    if (!property.IsHidden && (property.IsKey || !property.ISFK))
            //////    {
            //////        var originalProperty = item.OriginalProperties.First(x => x.ColumnID == property.ColumnID);
            //////        var newProperty = newItem.AddCopyProperty(property);
            //////        newItem.OriginalProperties.Add(originalProperty);
            //////        if (property.IsReadonly)
            //////            newProperty.Value = originalProperty.Value;
            //////    }
            //////}
            newItem.IsFullData = item.IsFullData;
            newItem.DataView = item.DataView;
            //   newItem.HasDirectData = item.HasDirectData;
            newItem.IsDBRelationship = item.IsDBRelationship;

            //newItem.IsHiddenBecauseOfCreatorRelationshipOnShow = item.IsHiddenBecauseOfCreatorRelationshipOnShow;
            //  newItem.IsHiddenBecauseOfCreatorRelationshipOnState = item.IsHiddenBecauseOfCreatorRelationshipOnState;
            //newItem.IsReadonlyBecauseOfCreatorRelationshipOnShow = item.IsReadonlyBecauseOfCreatorRelationshipOnShow;
            //newItem.IsReadonlyBecauseOfCreatorRelationshipOnState = item.IsReadonlyBecauseOfCreatorRelationshipOnState;
            //newItem.IsReadonlyBecauseOfState = item.IsReadonlyBecauseOfState;
            newItem.EntityListView = item.EntityListView;
            newItem.IsNewItem = item.IsNewItem;

            // newItem.ParantChildRelationshipInfo = newParentChildRelationshipInfo;

            foreach (var childItem in item.ChildRelationshipDatas)
            {
                //مطمئنن رابطه فارن به پرایمری هست وقتی که هیدن یا ریدونلی ترو باشه
                // bool skipChildRel = false;
                //if (childItem.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
                //{
                //   if (childItem.IsHidden)
                //    skipChildRel = true;
                //////else if ((childItem.Relationship.IsReadonly || childItem.IsReadonly) && childItem.CheckRelationshipIsChanged())
                //////    skipChildRel = true;
                //}
                if (!childItem.IsHidden)
                {
                    var newChildItems = new ChildRelationshipData(childItem.SourceData, childItem.Relationship);

                    newChildItems.RelationshipDeleteOption = childItem.RelationshipDeleteOption;
                    foreach (var orginalData in childItem.OriginalRelatedData)
                    {
                        newChildItems.OriginalRelatedData.Add(orginalData);
                    }
                    //foreach (var orginalData in childItem.HiddenData)
                    //{
                    //    //if (childItem.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromPrimartyToForeign)
                    //    //{
                    //    newChildItems.HiddenData.Add(orginalData);
                    //}
                    foreach (var orginalData in childItem.RemovedOriginalDatas)
                    {
                        //bool skipOriginalData = false;

                        //برای وقتی که شرط داده اجازه حذف میداده و داده حذف شده اما قبل از آپد یت دیگه شرط اجازه حذف را به علت هیدن یا ریدونلی بودن نمیده
                        //if ((orginalData.ParantChildRelationshipData != null /*&& orginalData.ToParantChildRelationshipData.IsHidden*/) || childItem.IsReadonlyOnState)// || orginalData.IsReadonlyOnState)
                        //{
                        //}
                        //else
                        newChildItems.RemovedDataForUpdate.Add(orginalData);
                        //اد و ریمو به این اصلی ها کنترل شود   از کلاسه پایه dp_data.. ریمو بشن

                        //else
                        //{
                        //    var relatedDataToOriginalData = childItem.GetRelatedDataOfOriginalData(orginalData);
                        //    if (relatedDataToOriginalData.IsHiddenBecauseOfCreatorRelationshipOnState)
                        //        skipOriginalData = true;
                        //}
                        //if (!skipOriginalData)
                        //    newChildItems.RemovedDataForUpdate.Add(orginalData);
                        //}
                        //else
                        //{
                        //    newChildItems.OriginalRelatedData.Add(orginalData);
                        //}
                    }

                    var relatedData = childItem.RelatedData.ToList();
                    foreach (var ritem in relatedData)
                    {
                        if (ritem.ShoudBeCounted)
                        {
                            newChildItems.RelatedData.Add(GetData(ritem));
                        }
                    }
                    //var childDataItems = GetData(childItem, newChildItems);
                    //foreach (var cItem in childDataItems)
                    //    newChildItems.RelatedData.Add(cItem);

                    //  newChildItems.CheckAddedRemovedRelationships();
                    newItem.ChildRelationshipDatas.Add(newChildItems);

                }

                //  }
            }

            return newItem;
        }
        //private void SetData(DP_DataRepository ritem)
        //{
        //    throw new NotImplementedException();
        //}

        private void RemoveUnwantedItems(List<DP_FormDataRepository> result)
        {

            //  SetDataOrRelatedDataIsChangedToNull(result);
            foreach (var data in result)
            {
                //if (data.IsHiddenBecauseOfCreatorRelationshipOnState)
                //{
                //    throw (new Exception("داده غیر فعال امکان حذف شدن را ندارد"));
                //}
            }
            SetChangedProperties(result);
            SetDataOrRelatedDataIsChanged(result);
            RemoveUnchangedProperties(result);
            RemoveAllUnchangedDatas(result, null);
            RemoveAllUnChangedChildRelInfos(result);
        }



        private void SetChangedProperties(List<DP_FormDataRepository> result)
        {
            //foreach (var data in result)
            //{
            //    List<EntityInstanceProperty> removeProperties = new List<EntityInstanceProperty>();
            //    foreach (var property in data.GetProperties().Where(x => !x.ISFK))
            //    {
            //        property.ValueIsChanged = data.PropertyValueIsChanged(property);
            //    }
            //    foreach (var child in data.ChildRelationshipDatas)
            //    {
            //        SetChangedProperties(child.RelatedData);
            //    }
            //}
        }
        private void SetDataOrRelatedDataIsChanged(List<DP_FormDataRepository> result)
        {
            foreach (var data in result)
            {
                DataOrRelatedDataHasChanged(data);
            }
        }
        void DataOrRelatedDataHasChanged(DP_FormDataRepository data)
        {

            if (data.DataOrRelatedDataIsChanged == null)
            {
                if (data.IsNewItem)
                {
                    data.IsEdited = true;
                }
                else
                {
                    if (data.GetProperties().Any(x => x.ValueIsChanged))
                        data.IsEdited = true;
                    if (data.ChildRelationshipDatas.Any(x => x.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary
                     && x.RelationshipIsChangedForUpdate))
                    {
                        data.IsEdited = true;
                    }
                    if (data.ParantChildRelationshipData != null && data.ParantChildRelationshipData.ToParentRelationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary
                   && !data.IsDBRelationship)
                    {
                        data.IsEdited = true;
                    }
                }
                if (data.IsEdited || data.ChildRelationshipDatas.Any(x => x.RelationshipIsChangedForUpdate))
                    data.DataOrRelatedDataIsChanged = true;
                foreach (var child in data.ChildRelationshipDatas)
                {
                    foreach (var childData in child.RelatedData)
                    {
                        if (childData.DataOrRelatedDataIsChanged == null)
                            DataOrRelatedDataHasChanged(childData);
                        if (childData.DataOrRelatedDataIsChanged == true)
                            data.DataOrRelatedDataIsChanged = true;
                    }
                }
                if (data.DataOrRelatedDataIsChanged == null)
                    data.DataOrRelatedDataIsChanged = false;
            }
        }

        private void RemoveUnchangedProperties(List<DP_FormDataRepository> result)
        {
            foreach (var data in result)
            {
                List<EntityInstanceProperty> removeItems = new List<EntityInstanceProperty>();
                if (!data.IsNewItem)
                {
                    foreach (var property in data.Properties)
                    {
                        if (!property.IsKey)
                        {
                            if (!property.ValueIsChanged)
                                removeItems.Add(property);
                        }
                    }
                }
                foreach (var child in data.ChildRelationshipDatas)
                {
                    RemoveUnchangedProperties(child.RelatedData);
                }
                foreach (var item in removeItems)
                {
                    data.Properties.Remove(item);
                }
            }
        }

        private void RemoveAllUnchangedDatas(List<DP_FormDataRepository> result, ChildRelationshipInfo childRelationshipInfo)
        {
            List<DP_FormDataRepository> removeItems = new List<DP_FormDataRepository>();

            foreach (var data in result)
            {

                //بعدا بررسی شود
                ////if (!item.HasDirectData)
                ////    removeItems.Add(item);
                if (data.DataOrRelatedDataIsChanged == null)
                    throw new Exception("xbxcvxcv");
                if (data.DataOrRelatedDataIsChanged == false)
                {
                    if (childRelationshipInfo == null || data.IsDBRelationship)
                    {
                        removeItems.Add(data);

                        //این بیخوده فکر کنم
                        if (childRelationshipInfo != null)
                            childRelationshipInfo.RemovedDataForUpdate.Remove(childRelationshipInfo.GetOroginalDataOfOriginalData(data));
                    }

                }
                foreach (var child in data.ChildRelationshipDatas)
                {
                    RemoveAllUnchangedDatas(child.RelatedData, child);
                }

            }
            foreach (var item in removeItems)
            {
                result.Remove(item);
            }

        }
        private void RemoveAllUnChangedChildRelInfos(List<DP_FormDataRepository> result)
        {

            foreach (var data in result)
            {
                List<ChildRelationshipInfo> removeRels = new List<ChildRelationshipInfo>();
                foreach (var child in data.ChildRelationshipDatas)
                {
                    if (!child.RelatedData.Any() && !child.RelationshipIsChangedForUpdate)
                        removeRels.Add(child);
                    RemoveAllUnChangedChildRelInfos(child.RelatedData);
                }
                foreach (var child in removeRels)
                {
                    data.ChildRelationshipDatas.Remove(child);
                    //این تیکه جدید اضافه شد
                    if (child.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary)
                    {
                        foreach (var relColumn in child.Relationship.RelationshipColumns)
                        {
                            var fkProp = data.GetProperty(relColumn.FirstSideColumnID);
                            if (fkProp != null)
                                if (!fkProp.Column.PrimaryKey)
                                    data.Properties.Remove(fkProp);
                        }
                    }
                }
            }
        }

        //private void SetDataOrRelatedDataIsChangedToNull(ObservableCollection<DP_DataRepository> result)
        //{
        //    foreach (var data in result)
        //    {
        //        data.DataOrRelatedDataIsChanged = null;
        //        foreach (var child in data.ChildRelationshipDatas)
        //        {
        //            SetDataOrRelatedDataIsChangedToNull(child.RelatedData);
        //        }
        //    }
        //}


        //private bool DataItemIsRedundant(DP_DataRepository item)
        //{
        //    bool result = true;
        //    if (item.RemovedItems.Any())
        //        return false;
        //    if (AgentHelper.DataHasValue(item))
        //        return false;
        //    bool childHasData = false;
        //    foreach (var child in item.ChildRelationshipDatas)
        //    {
        //        foreach (var childData in child.RelatedData)
        //        {
        //            if (DataItemIsRedundant(childData))
        //                childHasData = true;
        //        }
        //    }
        //    if (childHasData)
        //        return true;
        //    return result;
        //}
    }
}
