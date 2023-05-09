using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelEntites;
using ProxyLibrary;
using MyUILibrary.EntityArea.Commands;
using CommonDefinitions.UISettings;
using MyUILibraryInterfaces.FormulaCalculationArea;
using MyUILibrary.FormulaArea;
using MyUILibrary.Temp;
using System.Text.RegularExpressions;

namespace MyUILibrary.EntityArea
{
    public class UIValidationManager : I_UIValidationManager
    {
        BaseEditEntityArea EditArea { set; get; }
        public UIValidationManager(BaseEditEntityArea editArea)
        {
            EditArea = editArea;
        }
        //public bool ValidateData(bool fromUpdate)
        //{
        //    //var datas = EditArea.GetDataList();
        //    //if (datas == null)
        //    //    return true;

        //    //اگر از دکمه آپدیت اومده باشه چک میشه حتی اگر داده مخفی باشد زیرا اونموقع یعنی برای یک تمپ هستش که در فرم پدرش ممکنه
        //    //وضعیتش تغییر کنه و از حالت هیدن خارج بشه
        //    //اما برای کنترل داده های چایلد مستقیم هم خود داده باید ولید باشه وهم دادهی های چایلد غیر فعال نباشند
        //    List<DP_FormDataRepository> dataList = null;
        //    if (fromUpdate)
        //        dataList = EditArea.GetDataList().ToList();
        //    else
        //        dataList = EditArea.GetDataList().Where(x => x.ShoudBeCounted).ToList();
        //    bool result = true;
        //    RemoveValidationMessages();
        //    foreach (var data in dataList)
        //    {
        //        bool resultData = ValidateData(data);
        //        if (!resultData)
        //            result = false;
        //        if (resultData)
        //        {
        //            foreach (var relationshipControl in data.ChildRelationshipDatas)
        //            {
        //                if (relationshipControl.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateDirect
        //                         || relationshipControl.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect)
        //                {
        //                  //  var childRelInfo = data.ChildRelationshipDatas.First(x => x.Relationship == relationshipControl.Relationship);
        //                    if (!relationshipControl.IsHidden)
        //                    {
        //                      //  relationshipControl.RelationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(childRelInfo);

        //                        if (fromUpdate && EditArea.AreaInitializer.SourceRelationColumnControl == null)
        //                        {
        //                            if (!relationshipControl.RelationshipControl.GenericEditNdTypeArea.AreaInitializer.UIValidationManager.ValidateData(false))
        //                                result = false;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return result;

        //}
        public void RemoveValidationMessages()
        {
            //اینجا
            //////foreach (var item in EditArea.SimpleColumnControls)
            //////{
            //////    EditArea.RemoveColumnControlColor(item, ControlOrLabelAsTarget.Control, "validation");
            //////    EditArea.RemoveColumnControlMessage(item, ControlOrLabelAsTarget.Control, "validation");

            //////}
            //////foreach (var item in EditArea.RelationshipColumnControls)
            //////{
            //////    EditArea.RemoveColumnControlColor(item, ControlOrLabelAsTarget.Control, "validation");
            //////    EditArea.RemoveColumnControlMessage(item, ControlOrLabelAsTarget.Control, "validation");
            //////}

            //////EditArea.RemoveDataItemMessageByKey("validation");
            //////EditArea.RemoveDataItemColorByKey("validation");

        }

        public bool ValidateData(DP_FormDataRepository data)
        {
            bool result = true;
            data.ISValid = true;

            //اگر داده جدید باشد ستونها و رابطی که اجباری باشند اما در فرم ورود اطلاعات وجود نداشته باشند را چک میکند
            CheckAccessValidation(data);

            //foreach (var simplePropertyControl in EditArea.SimpleColumnControls)
            //{
            //    var dataProperty = data.GetProperty(simplePropertyControl.Column.ID);
            //    if (dataProperty != null)
            foreach (var simpleProperty in data.ChildSimpleContorlProperties)
                ValidateSimpleColumn(data, simpleProperty);
            //}

            foreach (var childRel in data.ChildRelationshipDatas)
            {
                // relationshipControl.GenericEditNdTypeArea.SetChildRelationshipInfo(data.ChildRelationshipDatas.First(x => x.Relationship == relationshipControl.Relationship));

                ValidateRelationshipColumn(data, childRel, childRel.RelationshipControl);
            }
            ValidateRelationshipFilters(data);
            ValidateISARelationships(data);
            ValidateUnionRelationships(data);
            ValidateEntityValidations(data);
            if (EditArea.AreaInitializer.TailDataValidation != null)
            {
                ValidateTailDataValidation(data);
            }
            if (data.ISValid == false)
            {
                result = false;
            }

            return result;
        }

        private void CheckAccessValidation(DP_FormDataRepository dataITem)
        {

            //**UIValidationManager.CheckAccessValidation: 3b7eb162083e
            if (dataITem.IsNewItem)
            {
                if (EditArea.FullEntity.Columns.Any(x => x.DataEntryEnabled && x.IsMandatory &&
                (!EditArea.SimpleColumnControls.Any(c => c.Column.ID == x.ID) &&
                !EditArea.RelationshipColumnControls.Any(r => r.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && r.Relationship.RelationshipColumns.Any(rc => rc.FirstSideColumnID == x.ID)) &&
                (EditArea.AreaInitializer == null || EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum != Enum_MasterRelationshipType.FromPrimartyToForeign || !EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.RelationshipColumns.Any(r => r.SecondSideColumnID == x.ID))
                )
                ))
                {  //اگر غیر از این باشه و بخوایم ستونهایی که دسترسی دارند را از آنهایی که ندارند جدا کرده و بررسی کنیم قضیه خلی پیچیده میشود. 
                   //بهتر است نیاز بیزینسی با اختصاصی سازی موجودیت حل شود ونه با دسترسی امنیتی
                    var columnNames = "";
                    foreach (var col in EditArea.FullEntity.Columns.Where(x => x.DataEntryEnabled && x.IsMandatory &&
                (!EditArea.SimpleColumnControls.Any(c => c.Column.ID == x.ID) &&
                !EditArea.RelationshipColumnControls.Any(r => r.Relationship.MastertTypeEnum == Enum_MasterRelationshipType.FromForeignToPrimary && r.Relationship.RelationshipColumns.Any(rc => rc.FirstSideColumnID == x.ID)) &&
                (EditArea.AreaInitializer == null || EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.MastertTypeEnum != Enum_MasterRelationshipType.FromPrimartyToForeign || !EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.RelationshipColumns.Any(r => r.SecondSideColumnID == x.ID))
                )
                ))
                    {
                        columnNames += (columnNames == "" ? "" : ",") + col.Name;
                    }
                    var message = "دسترسی به" + " " + (columnNames.Contains(",") ? "خصوصیاتی به نامهای" : "خصوصیتی به نام") + " "
                        + columnNames + " " + "برای موجودیت" + " " + EditArea.FullEntity.Alias + " " + "وجود ندارد";
                    AddDataValidationMessage(message, dataITem);
                }

                if (EditArea.FullEntity.Relationships.Any(x => x.DataEntryEnabled == true && x.IsOtherSideMandatory == true &&
              (!EditArea.RelationshipColumnControls.Any(r => r.Relationship.ID == x.ID) && (EditArea.AreaInitializer.SourceRelationColumnControl == null || EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID != x.ID))))
                {  //اگر غیر از این باشه و بخوایم روابطی که دسترسی دارند را از آنهایی که ندارند جدا کرده و بررسی کنیم قضیه خلی پیچیده میشود. 
                   //بهتر است نیاز بیزینسی با اختصاصی سازی موجودیت حل شود ونه با دسترسی امنیتی
                    var relNames = "";
                    foreach (var rel in EditArea.FullEntity.Relationships.Where(x => x.DataEntryEnabled == true &&
              (!EditArea.RelationshipColumnControls.Any(r => r.Relationship.ID == x.ID) && (EditArea.AreaInitializer.SourceRelationColumnControl == null || EditArea.AreaInitializer.SourceRelationColumnControl.Relationship.PairRelationshipID != x.ID))))
                    {
                        relNames += (relNames == "" ? "" : Environment.NewLine) + rel.Name;
                    }
                    var message = "دسترسی به" + " " + (relNames.Contains(",") ? "روابطی به نامهای" : "رابطه به نام") + " "
                        + relNames + " " + "برای موجودیت" + " " + EditArea.FullEntity.Alias + " " + "وجود ندارد";
                    AddDataValidationMessage(message, dataITem);
                }
            }
        }


        private void ValidateEntityValidations(DP_FormDataRepository data)
        {
            foreach (var validation in EditArea.EntityValidations)
            {
                //  bool booleanMode = string.IsNullOrEmpty(validation.Value);
                var calculatedValue = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.CalculateFormula(validation.FormulaID, data, AgentUICoreMediator.GetAgentUICoreMediator.GetRequester());

                //else if (validation.CodeFunctionID != 0)
                //{
                //    calculatedValue = AgentUICoreMediator.GetAgentUICoreMediator.CalculateCodeFunction(this, validation.CodeFunction, dataRepository).Result;
                //}

                if (calculatedValue.Exception == null)
                {
                    if (!Convert.ToBoolean(calculatedValue.Result))
                    {
                        data.ISValid = false;
                        AddDataValidationMessage(validation.Message, data);
                    }
                }
                else
                {
                    data.ISValid = false;
                    AddDataValidationMessage(calculatedValue.Exception.Message, data);
                }
            }

        }
        private void ValidateISARelationships(DP_FormDataRepository data)
        {
            //UIValidationManager.ValidateISARelationships: 3f64c5907ac5
            //ایزه ریلیشنهای ساب به سوپر
            foreach (var relationshipControl in EditArea.RelationshipColumnControls)
            {
                var childRel = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                if (!childRel.IsHidden)
                {
                    if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.SubToSuper)
                    {
                        var isaRelationship = (relationshipControl.Relationship as SubToSuperRelationshipDTO).ISARelationship;

                        if (isaRelationship.IsDisjoint)
                        {
                            var superDataItem = childRel.RealData.FirstOrDefault();
                            if (superDataItem != null)
                            {//اگر فول باشد در خود فرمش بررسی خواهد شد
                                if (!superDataItem.IsFullData)
                                {
                                    var superEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetFullEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), relationshipControl.GenericEditNdTypeArea.AreaInitializer.EntityID);
                                    var otherSuperToSubRelationships = superEntity.Relationships.Where(x => x is SuperToSubRelationshipDTO && (x as SuperToSubRelationshipDTO).ISARelationship.ID == isaRelationship.ID
                                    && (x as SuperToSubRelationshipDTO).ID != relationshipControl.Relationship.PairRelationshipID);
                                    bool moreThanOneData = false;
                                    foreach (var otherSuperToSubRelationship in otherSuperToSubRelationships)
                                    {
                                        var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                                        requester.SkipSecurity = true;
                                        DR_SearchViewRequest request = new DR_SearchViewRequest(requester, AgentUICoreMediator.GetAgentUICoreMediator.RelationshipDataManager.GetSecondSideSearchDataItemByRelationship(superDataItem, otherSuperToSubRelationship.ID));

                                        var otherSideData = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(request).ResultDataItems;
                                        if (otherSideData.Any())
                                        {
                                            moreThanOneData = true;
                                            break;
                                        }
                                    }
                                    if (moreThanOneData)
                                    {
                                        string relationshipNames = relationshipControl.Relationship.Entity1;
                                        foreach (var otherSuperToSubRelationship in otherSuperToSubRelationships)
                                        {
                                            relationshipNames += (relationshipNames == "" ? "" : ",") + otherSuperToSubRelationship.Entity2;
                                        }
                                        string message = "تنها یکی از";
                                        message += " " + (otherSuperToSubRelationships.Count() + 1) + " " + "رابطه" + " " + relationshipNames + " " + "با" + " " + relationshipControl.Relationship.Entity2 + " " + "ورود اطلاعات شود";

                                        AddColumnControlValidationMessage(relationshipControl, message, data);

                                    }

                                }
                            }
                        }

                    }
                }
            }

            //ایزه ریلیشنهای سوپر به ساب

            int parentISARelationshipID = 0;
            int parentSubToSuperRelationshipID = 0;
            string parentSubToSuperName = "";

            if (EditArea.AreaInitializer.SourceRelationColumnControl != null && EditArea.AreaInitializer.SourceRelationColumnControl.Relationship is SubToSuperRelationshipDTO)
            {
                parentSubToSuperRelationshipID = (EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ID;
                parentSubToSuperName = (EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).Entity1;
                parentISARelationshipID = (EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubToSuperRelationshipDTO).ISARelationship.ID;
            }
            List<Tuple<ISARelationshipDTO, List<RelationshipColumnControlGeneral>>> isaRelationships = new List<Tuple<ISARelationshipDTO, List<RelationshipColumnControlGeneral>>>();
            foreach (var relationshipControl in EditArea.RelationshipColumnControls)
            {
                var childRel = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                if (!childRel.IsHidden)
                {
                    if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.SuperToSub)
                    {
                        var isaID = (relationshipControl.Relationship as SuperToSubRelationshipDTO).ISARelationship.ID;
                        Tuple<ISARelationshipDTO, List<RelationshipColumnControlGeneral>> isaRelationship = null;
                        if (!isaRelationships.Any(x => x.Item1.ID == isaID))
                        {
                            isaRelationship = new Tuple<ISARelationshipDTO, List<RelationshipColumnControlGeneral>>((relationshipControl.Relationship as SuperToSubRelationshipDTO).ISARelationship, new List<RelationshipColumnControlGeneral>());
                            isaRelationships.Add(isaRelationship);
                        }
                        else
                            isaRelationship = isaRelationships.First(x => x.Item1.ID == isaID);
                        isaRelationship.Item2.Add(relationshipControl);
                        //}
                    }
                }
            }

            foreach (var isaRelationship in isaRelationships)
            {
                if (isaRelationship.Item1.IsTolatParticipation)
                {
                    //اگه از پرنت ساب باشه بنابراین خودش یک رابطه با داده فرض میشود
                    if (isaRelationship.Item1.ID != parentISARelationshipID)
                    {
                        bool hasData = false;
                        foreach (var relationshipControl in isaRelationship.Item2)
                        {
                            var childRelationshipInfo = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                            //این بود بررسی شود  if (childRelationshipInfo.RelatedData.Any(x => x.HasDirectData))
                            if (childRelationshipInfo.RealData.Any())
                                hasData = true;
                        }
                        if (!hasData)
                        {
                            string relationshipNames = "";
                            foreach (var relationshipControl in isaRelationship.Item2)
                            {
                                relationshipNames += (relationshipNames == "" ? "" : ",") + relationshipControl.Relationship.Entity2;
                            }
                            string message = "";
                            if (isaRelationship.Item1.IsDisjoint)
                                message = "یکی از";
                            else
                                message = "حداقل یکی از";
                            message += " " + isaRelationship.Item2.Count + " " + "رابطه" + " " + relationshipNames + " " + "می بایست ورود اطلاعات شود";
                            foreach (var relationshipControl in isaRelationship.Item2)
                            {
                                AddColumnControlValidationMessage(relationshipControl, message, data);
                            }
                        }
                    }
                }

                if (isaRelationship.Item1.IsDisjoint)
                {
                    bool moreThanOneData = false;
                    bool hasData = false;
                    if (isaRelationship.Item1.ID == parentISARelationshipID)
                        hasData = true;
                    foreach (var relationshipControl in isaRelationship.Item2)
                    {
                        var childRelationshipInfo = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                        if (childRelationshipInfo.RealData.Any())
                        {
                            if (!hasData)
                                hasData = true;
                            else
                                moreThanOneData = true;
                        }
                    }
                    if (moreThanOneData)
                    {
                        string relationshipNames = "";
                        if (parentSubToSuperName != "")
                            relationshipNames = parentSubToSuperName;
                        foreach (var relationshipControl in isaRelationship.Item2)
                        {
                            relationshipNames += (relationshipNames == "" ? "" : ",") + relationshipControl.Relationship.Entity2;
                        }
                        string message = "تنها یکی از";
                        message += " " + isaRelationship.Item2.Count + " " + "رابطه" + " " + relationshipNames + " " + "با" + " " + EditArea.FullEntity.Name + " " + " " + "ورود اطلاعات شود";
                        foreach (var relationshipControl in isaRelationship.Item2)
                        {
                            AddColumnControlValidationMessage(relationshipControl, message, data);
                        }
                    }
                }
            }
        }

        private void ValidateUnionRelationships(DP_FormDataRepository data)
        {
        //UIValidationManager.ValidateUnionRelationships: e50167077e44
            foreach (var relationshipControl in EditArea.RelationshipColumnControls)
            {
                if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.SubUnionToUnion)
                {
                    var childRel = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                    if (!childRel.IsHidden)
                    {
                        var unionRelationship = (relationshipControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship;
                        //چک کردن دیسجوینت بودن
                        var superDataItem = childRel.RealData.FirstOrDefault();
                        if (superDataItem != null)
                        {
                            if (!superDataItem.IsFullData)
                            {
                                var superEntity = AgentUICoreMediator.GetAgentUICoreMediator.tableDrivedEntityManagerService.GetFullEntity(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), relationshipControl.GenericEditNdTypeArea.AreaInitializer.EntityID);
                                var otherSuperToSubRelationships = superEntity.Relationships.Where(x => x is SuperToSubRelationshipDTO && (x as SuperToSubRelationshipDTO).ISARelationship.ID == unionRelationship.ID
                                && (x as SuperToSubRelationshipDTO).ID != relationshipControl.Relationship.PairRelationshipID);
                                bool moreThanOneData = false;
                                foreach (var otherSuperToSubRelationship in otherSuperToSubRelationships)
                                {
                                    var requester = AgentUICoreMediator.GetAgentUICoreMediator.GetRequester();
                                    requester.SkipSecurity = true;
                                    DR_SearchViewRequest request = new DR_SearchViewRequest(requester, AgentUICoreMediator.GetAgentUICoreMediator.RelationshipDataManager.GetSecondSideSearchDataItemByRelationship(superDataItem, otherSuperToSubRelationship.ID));

                                    var otherSideData = AgentUICoreMediator.GetAgentUICoreMediator.requestRegistration.SendSearchViewRequest(request).ResultDataItems;
                                    if (otherSideData.Any())
                                    {
                                        moreThanOneData = true;
                                        break;
                                    }
                                }

                                if (moreThanOneData)
                                {
                                    string relationshipNames = relationshipControl.Relationship.Entity1;
                                    foreach (var otherSuperToSubRelationship in otherSuperToSubRelationships)
                                    {
                                        relationshipNames += (relationshipNames == "" ? "" : ",") + otherSuperToSubRelationship.Entity2;
                                    }
                                    string message = "تنها یکی از";
                                    message += " " + (otherSuperToSubRelationships.Count() + 1) + " " + "رابطه" + " " + relationshipNames + " " + "با" + " " + relationshipControl.Relationship.Entity2 + " " + "ورود اطلاعات شود";

                                    AddColumnControlValidationMessage(relationshipControl, message, data);
                                }

                            }
                        }
                    }
                }
            }

            int parentUnionRelationshipID = 0;
            int parentSubToSuperRelationshipID = 0;
            string parentSubToSuperName = "";

            if (EditArea.AreaInitializer.SourceRelationColumnControl != null && EditArea.AreaInitializer.SourceRelationColumnControl.Relationship is SubUnionToSuperUnionRelationshipDTO)
            {
                parentSubToSuperRelationshipID = (EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).ID;
                parentSubToSuperName = (EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).Entity1;
                parentUnionRelationshipID = (EditArea.AreaInitializer.SourceRelationColumnControl.Relationship as SubUnionToSuperUnionRelationshipDTO).UnionRelationship.ID;
            }
            List<Tuple<UnionRelationshipDTO, List<RelationshipColumnControlGeneral>>> unionRelationships = new List<Tuple<UnionRelationshipDTO, List<RelationshipColumnControlGeneral>>>();
            foreach (var relationshipControl in EditArea.RelationshipColumnControls)
            {
                var childRel = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                if (!childRel.IsHidden)
                {
                    if (relationshipControl.Relationship.TypeEnum == Enum_RelationshipType.UnionToSubUnion)
                    {
                        var unionID = (relationshipControl.Relationship as UnionToSubUnionRelationshipDTO).UnionRelationship.ID;
                        Tuple<UnionRelationshipDTO, List<RelationshipColumnControlGeneral>> unionRelationship = null;
                        if (!unionRelationships.Any(x => x.Item1.ID == unionID))
                        {
                            unionRelationship = new Tuple<UnionRelationshipDTO, List<RelationshipColumnControlGeneral>>((relationshipControl.Relationship as UnionToSubUnionRelationshipDTO).UnionRelationship, new List<RelationshipColumnControlGeneral>());
                            unionRelationships.Add(unionRelationship);
                        }
                        else
                            unionRelationship = unionRelationships.First(x => x.Item1.ID == unionID);
                        unionRelationship.Item2.Add(relationshipControl);
                        //}
                    }
                }
            }

            foreach (var unionRelationship in unionRelationships)
            {
                //توتال پارتیشپنت در یونیون برعکسه و یعنی هر ساب با یک سوپر در ارتباط باشد.این را میتوان از طریق اجباری نمودن رابطه چک کرد


                bool moreThanOneData = false;
                bool hasData = false;
                if (unionRelationship.Item1.ID == parentUnionRelationshipID)
                    hasData = true;
                foreach (var relationshipControl in unionRelationship.Item2)
                {
                    var childRelationshipInfo = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);

                    //if (childRelationshipInfo.RelatedData.Any(x => x.HasDirectData))
                    if (childRelationshipInfo.RealData.Any())
                    {
                        if (!hasData)
                            hasData = true;
                        else
                            moreThanOneData = true;
                    }
                }
                if (moreThanOneData)
                {
                    string relationshipNames = "";
                    if (parentSubToSuperName != "")
                        relationshipNames = parentSubToSuperName;
                    foreach (var relationshipControl in unionRelationship.Item2)
                    {
                        relationshipNames += (relationshipNames == "" ? "" : ",") + relationshipControl.Relationship.Entity2;
                    }
                    string message = "تنها یکی از";
                    message += " " + unionRelationship.Item2.Count + " " + "رابطه" + " " + relationshipNames + " " + "با" + " " + EditArea.FullEntity.Name + " " + " " + "ورود اطلاعات شود";
                    foreach (var relationshipControl in unionRelationship.Item2)
                    {
                        AddColumnControlValidationMessage(relationshipControl, message, data);
                    }
                }


            }






        }
        private void ValidateRelationshipColumn(DP_FormDataRepository dataItem, ChildRelationshipInfo childRelationshipInfo, RelationshipColumnControlGeneral relationshipControl)
        {
            //** UIValidationManager.ValidateRelationshipColumn: dec468052171
            if (!childRelationshipInfo.IsHidden)
            {
                if (relationshipControl.Relationship.IsOtherSideMandatory == true)
                {
                    if (!childRelationshipInfo.RealData.Any())
                    {
                        AddColumnControlValidationMessage(relationshipControl, "مقدار دهی این رابطه اجباری می باشد", dataItem);
                    }
                }
            }
            //یک ولیدشن دیگه انجام شود.اینکه اگر رابطه فیلتر داشت و یک بار جستجو با داده انتخاب شده وفیلتر انجام شود تا جلوی رکوردهای غیر مجاز گرفته شود.مثل اداره مربوطه در تشکیل پرونده که آیتم سورس آنها تغییر میکند
        }
        private void ValidateRelationshipFilters(DP_FormDataRepository data)
        {
            //اینجا هم بحث realdata یا relateddata چک بشه
            //
            foreach (var relationshipControl in EditArea.RelationshipColumnControls)
            {
                var childRel = data.ChildRelationshipDatas.First(x => x.Relationship.ID == relationshipControl.Relationship.ID);
                if (!childRel.IsHidden)
                {
                    if (relationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectDirect
                    || relationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.CreateSelectInDirect
                    || relationshipControl.GenericEditNdTypeArea.AreaInitializer.IntracionMode == IntracionMode.Select)
                    {
                        if (relationshipControl.GenericEditNdTypeArea!=null && relationshipControl.GenericEditNdTypeArea.RelationshipFilters != null)
                        {
                            if (relationshipControl.GenericEditNdTypeArea.RelationshipFilters.Any())
                            {
                                foreach (var filter in relationshipControl.GenericEditNdTypeArea.RelationshipFilters)
                                {
                                    if (data.ChildRelationshipDatas.Any(x => x.Relationship.ID == filter.RelationshipID && x.RelatedData.Any()))
                                    {
                                        bool searchAndValueColumnsareEqual = false;

                                        var value = AgentUICoreMediator.GetAgentUICoreMediator.formulaManager.GetValueSomeHow(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), data, filter.ValueRelationshipTail, filter.ValueColumnID);
                                        foreach (var searchData in data.ChildRelationshipDatas.First(x => x.Relationship.ID == filter.RelationshipID && x.RelatedData.Any()).RelatedData)
                                        {
                                            var searchValue = searchData.GetProperty(filter.SearchColumnID);
                                            if (searchValue != null)
                                                if (searchValue.Value == value)
                                                    searchAndValueColumnsareEqual = true;
                                        }
                                        if (!searchAndValueColumnsareEqual)
                                        {
                                            var message = "فیلتر رابطه برای رابطه به نام" + " " + relationshipControl.Relationship.Alias + " " + "رعایت نشده است";
                                            AddColumnControlValidationMessage(relationshipControl, message, data);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ValidateTailDataValidation(DP_FormDataRepository data)
        {
            //برای فرمهایی که از کارتابل باز میشود اینکه داده ها حتما با داده مرجع در ارتباط باشند
            //تست شود
            var foundItems = AgentHelper.GetRelatedDataItemsSomeHow(data, EditArea.AreaInitializer.TailDataValidation.Item2);
            bool found = false;
            foreach (var item in foundItems)
            {
                if (AgentHelper.DataItemsAreEqual(item, EditArea.AreaInitializer.TailDataValidation.Item1))
                {
                    found = true;
                    break;
                }
            }
            //پیغام مرتبط شود برای فرم یکی هم اضافه شود
            if (!found)
            {
                AddDataValidationMessage("ارتباط با داده مرجع برقرار نمی باشد", data);
            }
        }


        private void ValidateSimpleColumn(DP_FormDataRepository dataItem, ChildSimpleContorlProperty childSimpleContorlProperty)
        {
            //**UIValidationManager.ValidateSimpleColumn: 2d60cd226e9e
            var simplePropertyControl = childSimpleContorlProperty.SimpleColumnControl;
            var dataColumn = childSimpleContorlProperty.Property;
            if (simplePropertyControl.Column.IsMandatory == true)
            {
                if (dataColumn.Value == null || dataColumn.Value.ToString() == "")
                {
                    if (dataItem.IsNewItem == false || simplePropertyControl.Column.IsIdentity == false)
                        AddColumnControlValidationMessage(simplePropertyControl, "مقدار دهی این خصوصیت اجباری می باشد", dataItem);

                }
            }
            if (dataColumn.Value != null && dataColumn.Value.ToString() != "")
            {
                if (simplePropertyControl.Column.StringColumnType != null)
                {
                    if (simplePropertyControl.Column.StringColumnType.MaxLength != 0
                        && simplePropertyControl.Column.StringColumnType.MaxLength != -1)
                    {
                        if (dataColumn.Value.ToString().Length > simplePropertyControl.Column.StringColumnType.MaxLength)
                        {
                            string message = "حداکثر طول این خصوصیت" + " " + simplePropertyControl.Column.StringColumnType.MaxLength + " " + "کاراکتر می باشد";
                            AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                        }
                    }
                    if (simplePropertyControl.Column.StringColumnType.MinLength != 0
                       && simplePropertyControl.Column.StringColumnType.MinLength != null)
                    {
                        if (dataColumn.Value.ToString().Length < simplePropertyControl.Column.StringColumnType.MinLength)
                        {
                            string message = "حداقل طول این خصوصیت" + " " + simplePropertyControl.Column.StringColumnType.MinLength + " " + "کاراکتر می باشد";
                            AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                        }
                    }
                }

                if (simplePropertyControl.Column.NumericColumnType != null)
                {
                    if (simplePropertyControl.Column.NumericColumnType.MinValue != null)
                    {
                        var value = Convert.ToDouble(dataColumn.Value);
                        if (value < simplePropertyControl.Column.NumericColumnType.MinValue.Value)
                        {
                            string message = "حداقل مقدار این خصوصیت" + " " + simplePropertyControl.Column.NumericColumnType.MinValue.Value + " " + "می باشد";
                            AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                        }
                    }
                    if (simplePropertyControl.Column.NumericColumnType.MaxValue != null)
                    {
                        var value = Convert.ToDouble(dataColumn.Value);
                        if (value > simplePropertyControl.Column.NumericColumnType.MaxValue.Value)
                        {
                            string message = "حداکثر مقدار این خصوصیت" + " " + simplePropertyControl.Column.NumericColumnType.MaxValue.Value + " " + "می باشد";
                            AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                        }
                    }
                    if (simplePropertyControl.Column.NumericColumnType.Precision != 0)
                    {
                        var value = Convert.ToDouble(dataColumn.Value);
                        if (value.ToString().Replace(".", "").Length > simplePropertyControl.Column.NumericColumnType.Precision)
                        {
                            string message = "تعداد اعداد این خصوصیت از مقدار تعیین شده" + " " + simplePropertyControl.Column.NumericColumnType.Precision + " " + "بیشتر می باشد";
                            AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                        }
                    }
                    if (simplePropertyControl.Column.NumericColumnType.Scale != 0)
                    {
                        var value = Convert.ToDouble(dataColumn.Value);
                        if (value.ToString().Contains("."))
                        {
                            var splt = value.ToString().Split('.')[1];
                            if (splt.Length > simplePropertyControl.Column.NumericColumnType.Scale)
                            {
                                string message = "تعداد اعشار این خصوصیت از مقدار تعیین شده" + " " + simplePropertyControl.Column.NumericColumnType.Scale + " " + "بیشتر می باشد";
                                AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                            }
                        }
                    }
                }
                if (simplePropertyControl.Column.StringColumnType != null)
                {
                    if (!string.IsNullOrEmpty(simplePropertyControl.Column.StringColumnType.Format))
                    {
                        Regex regex = new Regex(simplePropertyControl.Column.StringColumnType.Format);
                        if (!regex.IsMatch(dataColumn.Value.ToString()))
                        {
                            string message = "فرمت این خصوصیت صحیح نمی باشد";
                            AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                        }
                    }
                }



                if (simplePropertyControl.Column.HasValueRange)
                {

                    List<ColumnValueRangeDetailsDTO> validValueRange = childSimpleContorlProperty.ColumnValueRange;
                    //  اینجا
                    //if (dataItem.ColumnKeyValueRanges.Any(x => x.Key == simplePropertyControl.Column.ID && x.Value.Any()))
                    //{
                    //    validValueRange = dataItem.ColumnKeyValueRanges.First(x => x.Key == simplePropertyControl.Column.ID && x.Value.Any()).Value;

                    //}
                    //else
                    //    validValueRange = simplePropertyControl.Column.ColumnValueRange.Details;

                    if (!validValueRange.Any(x => x.Value == dataColumn.Value.ToString()))
                    {
                        string message = "مقدار این خصوصیت در لیست مقادیر مجاز نمی باشد";
                        AddColumnControlValidationMessage(simplePropertyControl, message, dataItem);
                    }
                }
            }
        }
        //public void AddRelationshipColumnMessageItem(RelationshipColumnControl relationshipControl, string message, InfoColor infoColor, string key, DP_FormDataRepository causingData, bool isPermanent)
        //{
        //    BaseMessageItem baseMessageItem = new BaseMessageItem();
        //    baseMessageItem.ColumnControl = relationshipControl;
        //    baseMessageItem.Message = message;
        //    baseMessageItem.Key = key;
        //    baseMessageItem.IsPermanentMessage = isPermanent;
        //    baseMessageItem.CausingDataItem = causingData;
        //    baseMessageItem.Color = infoColor;
        //    MessageItems.Add(baseMessageItem);
        //    relationshipControl.AddControlManagerMessage(baseMessageItem);
        //}


        public void AddColumnControlValidationMessage(BaseColumnControl baseColumnControl, string message, DP_FormDataRepository causingData)
        {
            causingData.ISValid = false;
            //اینجا
            //////ColumnControlMessageItem baseMessageItem = new ColumnControlMessageItem(baseColumnControl, ControlOrLabelAsTarget.Control);
            //////baseMessageItem.CausingDataItem = causingData;
            //////baseMessageItem.Key = "validation";
            //////baseMessageItem.Message = message;
            //////baseMessageItem.Priority = ControlItemPriority.High;
            //////EditArea.AddColumnControlMessage(baseMessageItem);

            //////ColumnControlColorItem baseColorItem = new ColumnControlColorItem(baseColumnControl, ControlOrLabelAsTarget.Control);
            //////baseColorItem.Key = "validation";
            //////baseColorItem.Color = InfoColor.Red;
            //////baseColorItem.ColorTarget = ControlColorTarget.Border;
            //////baseColorItem.Priority = ControlItemPriority.High;
            //////baseColorItem.CausingDataItem = causingData;
            //////EditArea.AddColumnControlColor(baseColorItem); ;

        }
        public void AddDataValidationMessage(string message, DP_FormDataRepository causingData)
        {
            //BaseMessageItem baseMessageItem = new BaseMessageItem();
            //baseMessageItem.CausingDataItem = causingData;
            //baseMessageItem.Key = key;
            //causingData.ISValid = false;
            //بعدا بررسی شود
            //اینجا بهتره برای همه کنترل ها تولتیپ اضافه شود
            //baseMessageItem.Message = message;
            //baseMessageItem.Color = Temp.InfoColor.Red;
            //  MessageItems.Add(baseMessageItem);

            //اینکه کدام داده هست به مسیج اضافه شود

            causingData.ISValid = false;
            //اینجا
            //////DataMessageItem baseMessageItem = new DataMessageItem();
            //////baseMessageItem.CausingDataItem = causingData;
            //////baseMessageItem.Key = "validation";
            //////baseMessageItem.Message = message;
            //////baseMessageItem.Priority = ControlItemPriority.High;
            //////EditArea.AddDataItemMessage(baseMessageItem);

            //////DataColorItem baseColorItem = new DataColorItem();
            //////baseColorItem.Key = "validation";
            //////baseColorItem.Color = InfoColor.Red;
            //////baseColorItem.ColorTarget = ControlColorTarget.Border;
            //////baseColorItem.Priority = ControlItemPriority.High;
            //////baseColorItem.CausingDataItem = causingData;
            //////EditArea.AddDataItemColor(baseColorItem); ;
            //////AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("اعتبارسنجی", message, InfoColor.Red);
        }

    }

}
