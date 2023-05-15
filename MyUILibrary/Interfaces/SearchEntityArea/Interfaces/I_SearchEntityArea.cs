using ModelEntites;
using MyUILibrary.EntityArea.Commands;
using MyUILibrary.Temp;
using ProxyLibrary;
using System;
using System.Collections.Generic;
namespace MyUILibrary.EntityArea
{
    public interface I_BaseSearchEntityArea
    {
        SearchAreaInitializer AreaInitializer { set; get; }
        void ClearSearchData();
    }
    public interface I_SearchEntityArea : I_BaseSearchEntityArea
    {
        void OnSearchDataDefined(DP_SearchRepositoryMain logicPhrase);
        DP_SearchRepositoryMain LastSearch { set; get; }

        event EventHandler<DP_SearchRepositoryMain> SearchDataDefined;
        I_View_SearchEntityArea SearchView { get; }
        I_EntityDefinedSearchArea SimpleSearchEntityArea { set; get; }
        I_AdvancedSearchEntityArea AdvancedSearchEntityAre { set; get; }
        bool IsSimpleSearchActiveOrAdvancedSearch { get; }
        //     void ShowSearchRepository(DP_SearchRepositoryMain searchRepository);

    }
    public interface I_AdvancedAndRawSearchEntityArea : I_BaseSearchEntityArea
    {
        //باید از این ایونت استفاده شود بجای کار با متغیر های داخلی مانندRawSearchEntityArea 
        //بهتر است در اینشیالایزر رابطه گنجانده شود و به هنگام بازگرداندن 
        //DP_SearchRepositoryMain
        //در صورت وجود رابطه مقادیر سورس ریلیشن و .. ست وشود

        event EventHandler<DP_SearchRepositoryMain> SearchDataDefined;
        //event EventHandler<AdvanceOrRawArg> SearchDataDefined;
        I_View_SearchEntityArea SearchView { set; get; }
        I_RawSearchEntityArea RawSearchEntityArea { set; get; }
        I_AdvancedSearchEntityArea AdvancedSearchEntityAre { set; get; }

    }
    public interface I_EntityDefinedSearchArea : I_BaseSearchEntityArea
    {
        EntitySearchDTO EntitySearchDTO { get; }
        void OnSearchDataDefined(DP_SearchRepositoryMain logicPhrase);
        DP_SearchRepositoryMain GetSearchRepository();
        void ShowPreDefinedSearch(PreDefinedSearchDTO preDefinedSearch);

        event EventHandler<DP_SearchRepositoryMain> SearchDataDefined;

        PreDefinedSearchDTO GetSearchRepositoryForSave();
        //List<I_Command> SearchCommands
        //{
        //    get;
        //    set;
        //}

        I_View_SimpleSearchEntityArea SimpleSearchView { set; get; }

        List<RelationshipSearchColumnControl> RelationshipColumnControls
        {
            set;
            get;
        }
        List<SimpleSearchColumnControl> SimpleColumnControls
        {
            set;
            get;
        }

        //  bool PreDefinedSearchIsApplicable(EntityPreDefinedSearchDTO message);
        //      bool ApplyPreDefinedSearch(EntityPreDefinedSearchDTO message);
        //List<PreDefinedSearchColumns> GetSearchColumns();
    }
    public interface I_RawSearchEntityArea : I_BaseSearchEntityArea
    {
        void OnSearchDataDefined(List<SearchProperty> logicPhrase);
        List<SearchProperty> GetSearchRepository();

        event EventHandler<SearchPropertyArg> SearchDataDefined;
        List<I_Command> SearchCommands
        {
            get;
            set;
        }
        I_View_SimpleSearchEntityArea RawSearchView { set; get; }
        List<SimpleSearchColumnControl> SimpleColumnControls
        {
            set;
            get;
        }

    }
    public interface I_AdvancedSearchEntityArea : I_BaseSearchEntityArea
    {
        void OnSearchDataDefined(DP_SearchRepositoryMain logicPhrase);
        DP_SearchRepositoryMain GetSearchRepository();

        bool ShowSearchRepository(DP_SearchRepositoryMain item);
        //  void ApplyPreDefinedSearch(EntityPreDefinedSearchDTO message);

        event EventHandler<DP_SearchRepositoryMain> SearchDataDefined;
        List<I_Command> SearchCommands
        {
            get;
            set;
        }
        I_View_AdvancedSearchEntityArea AdvancedSearchView { set; get; }

    }
    public class SimpleSearchColumnControl : BaseColumnControl
    {
        public event EventHandler<SimpleSearchColumnControl> FormulaSelectionRequested;
        public SimpleSearchColumnControl(IAgentUIManager uiManager, EntitySearchColumnsDTO entitySearchColumn)
        {
            EntitySearchColumn = entitySearchColumn;
            ControlManager = uiManager.GenerateSimpleControlManagerForOneDataForm(Column, EntitySearchColumn.ColumnUISetting, Operators);
            _LabelControlManager = uiManager.GenerateLabelControlManager(EntitySearchColumn.Alias);
        }
        public ColumnDTO Column { get { return EntitySearchColumn.Column; } }
        public EntitySearchColumnsDTO EntitySearchColumn { get; set; }
        public I_SimpleControlManagerOne ControlManager { get; set; }
        public List<SimpleSearchOperator> Operators { get { return EntitySearchColumn.Operators; } }
        public FormulaDTO Formula { get; internal set; }



        public void SetSimpleColumnFormulaSelection()
        {
            var cpMenuFormulaCalculation = new ConrolPackageMenu();
            cpMenuFormulaCalculation.Name = "mnuFormulaCalculation";
            cpMenuFormulaCalculation.Title = "انتخاب فرمول";
            // cpMenuFormulaCalculation.Tooltip = columnControl.Column.ColumnCustomFormula.Formula.Tooltip;
            ControlManager.GetUIControlManager().AddButtonMenu(cpMenuFormulaCalculation);
            cpMenuFormulaCalculation.MenuClicked += (sender1, e1) => CpMenuFormulaCalculation_MenuClicked(sender1, e1, cpMenuFormulaCalculation);

        }
        public void AddSimpleColumnFormula(FormulaDTO formula)
        {
            ControlManager.GetUIControlManager().RemoveButtonMenus();

            Formula = formula;
            var cpMenuFormulaCalculation = new ConrolPackageMenu();
            cpMenuFormulaCalculation.Name = "mnuFormulaCalculation";
            cpMenuFormulaCalculation.Title = formula.Title;
            cpMenuFormulaCalculation.Tooltip = formula.Tooltip;
            // cpMenuFormulaCalculation.Tooltip = columnControl.Column.ColumnCustomFormula.Formula.Tooltip;
            ControlManager.GetUIControlManager().AddButtonMenu(cpMenuFormulaCalculation);
            ControlManager.GetUIControlManager().SetMenuColor(InfoColor.Green);

            ControlManager.GetUIControlManager().ClearValue();
            ControlManager.GetUIControlManager().EnableDisable(false);

            ControlManager.GetUIControlManager().SetTooltip(formula.Tooltip);


            var cpMenuFormulaCalculationDelete = new ConrolPackageMenu();
            cpMenuFormulaCalculationDelete.Name = "mnuFormulaCalculationDelete";
            cpMenuFormulaCalculationDelete.Title = "حذف فرمول";
            // cpMenuFormulaCalculation.Tooltip = columnControl.Column.ColumnCustomFormula.Formula.Tooltip;
            ControlManager.GetUIControlManager().AddButtonMenu(cpMenuFormulaCalculationDelete);



            cpMenuFormulaCalculationDelete.MenuClicked += (sender1, e1) => CpMenuFormulaCalculation_MenuClickedDelete(sender1, e1, cpMenuFormulaCalculation);


        }
        private void CpMenuFormulaCalculation_MenuClickedDelete(object sender1, ConrolPackageMenuArg e1, ConrolPackageMenu conrolPackageMenu)
        {
            RemoveSimpleColumnFormula();
        }

        public void RemoveSimpleColumnFormula()
        {
            ControlManager.GetUIControlManager().SetTooltip(null);
            ControlManager.GetUIControlManager().ClearMenuColor();
            ControlManager.GetUIControlManager().EnableDisable(true);
            Formula = null;
            ControlManager.GetUIControlManager().RemoveButtonMenus();
            SetSimpleColumnFormulaSelection();
        }

        private void CpMenuFormulaCalculation_MenuClicked(object sender1, ConrolPackageMenuArg e1, ConrolPackageMenu conrolPackageMenu)
        {
            if (FormulaSelectionRequested != null)
            {
                FormulaSelectionRequested(sender1, this);
            }
        }
        //public SearchEnumerableType SearchEnumerableType { get; set; }
    }

    //public class NullColumnControl : SimpleSearchColumnControl
    //{
    //    public ColumnDTO FakeColumn { set; get; }

    //}
    //public class RelationCheckColumnControl : SimpleSearchColumnControl
    //{
    //    public ColumnDTO FakeColumn { set; get; }

    //}
    //public class RelationCountCheckColumnControl : SimpleSearchColumnControl
    //{
    //    public RelationCountCheckColumnControl()
    //    {

    //    }
    //    //   public SearchEnumerableType EnumerableType { get; set;}
    //    public ColumnDTO FakeColumn { set; get; }

    //}


    //public class UISearchNullControlPackage : UIControlPackageForSimpleColumn
    //{
    //    public event EventHandler NullSelected;
    //}
    //public enum NumericOperator
    //{
    //    Equals,
    //    BiggerThan,
    //    SmallerThan,
    //    EqualAndBiggerThan,
    //    EqualAndSmallerThan,
    //    NotEquals,
    //    NullValue
    //}
    public enum BooleanOperator
    {
        Equals,
        NullValue
    }
    //public class SearchAreaRelationSource
    //{
    //    public SearchAreaRelationSource()
    //    {
    //        //RelationData = new List<DP_SearchRepositoryMain>();
    //        //SourceRelationColumns = new List<ColumnDTO>();
    //        //RelationshipColumns = new List<RelationshipColumnDTO>();
    //    }
    //    //public Enum_DP_RelationSide SourceRelationSide { set; get; }
    //    public int SourceEntityID { set; get; }
    //    public int SourceTableID { set; get; }
    //    //public List<RelationshipColumnDTO> RelationshipColumns { set; get; }
    //    //public RelationshipDTO Relationship { set; get; }
    //    public int TargetEntityID { set; get; }
    //    public int TargetTableID { set; get; }
    //    public int RelationshipID { set; get; }
    //    //public List<ColumnDTO> TargetRelationColumns { set; get; }

    //    public Enum_RelationshipType RelationshipType
    //    {
    //        set;get;
    //    }
    //    public Enum_MasterRelationshipType MasterRelationshipType
    //    {
    //        get
    //        {
    //            return Relationship.MastertTypeEnum;
    //        }
    //    }
    //    public I_SearchEntityArea SourceSearchArea { set; get; }
    //    public DP_SearchRepositoryMain RelatedData { set; get; }

    //    public bool TargetSideIsMandatory { set; get; }

    //    //public bool SourceHoldsKeys { set; get; }
    //}


    //public class DP_SearchRepositoryMain : EventArgs
    //{
    //    public DP_SearchRepositoryMain SearchItems
    //    {
    //        get;
    //        set;
    //    }



    //}
    public class SearchLogicArg : EventArgs
    {
        public LogicPhraseDTO SearchItems
        {
            get;
            set;
        }

        //public AdvanceSearchNode AdvanceSearchNode
        //{
        //    get;
        //    set;
        //}


    }
    public class SearchPropertyArg
    {
        public List<SearchProperty> SearchItems
        {
            get;
            set;
        }
    }
    public class AdvanceOrRawArg
    {
        public AdvanceOrRaw SearchItems
        {
            get;
            set;
        }
    }
    public class AdvanceOrRaw
    {
        public LogicPhraseDTO LogicPhrase { set; get; }
        public List<SearchProperty> SearchProperties { set; get; }
    }
}
