﻿
using ModelEntites;
using MyFormulaFunctionStateFunctionLibrary;
using MyModelManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using System.Collections.ObjectModel;
using Telerik.Windows.Documents.FormatProviders.Txt;
using System.Collections;
using ProxyLibrary;
using Telerik.Windows.Documents.Model.Styles;
using System.Timers;
using System.Windows.Threading;

using MyUILibrary.EntityArea;
using MyUIGenerator;
using System.Windows.Documents;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmFormula.xaml
    /// </summary>
    /// 
    public partial class frmNewFormulaDefinition : UserControl
    {
        FormulaFunctionHandler formulaFunctionHandler = new FormulaFunctionHandler();
        public event EventHandler<FormulaDefinedArg> FormulaDefined;
        FormulaAutoComplete formulaAutoComplete;
        //  FormulaDefinitionInstance FormulaInstance { set; get; }
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        int EntityID { set; get; }
        string FormulaString { set; get; }
        List<EntityAndProperties> EntityAndProperties = new List<EntityAndProperties>();

        TableDrivedEntityDTO Entity { set; get; }
        private I_ExpressionEvaluator ExpressionEvaluator;

        DP_DataRepository DataItem { set; get; }
        DispatcherTimer textChangedCalculationTimer = new DispatcherTimer();
        DispatcherTimer textChangedTimer = new DispatcherTimer();
        DispatcherTimer statesTimer = new DispatcherTimer();
        // DispatcherTimer selectionTimer = new DispatcherTimer();
        //List<NodeContext> nodeDictionary = new List<NodeContext>();
        List<FormulaTextBlock> AllTextStates = new List<FormulaTextBlock>();
        //   List<FormulaTextBlock> LastTextStates = new List<FormulaTextBlock>();
        public frmNewFormulaDefinition(string formula, int entityID)
        {
            InitializeComponent();
            EntityID = entityID;
            Entity = bizTableDrivedEntity.GetPermissionedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID);
            formulaAutoComplete = new FormulaAutoComplete();
            formulaAutoComplete.NodeSelected += FormulaAutoComplete_NodeSelected;
            FormulaString = formula;
            SetTimers();
            txtFormula.SelectionChanged += TxtFormula_SelectionChanged;
            DataItem = new ProxyLibrary.DP_DataRepository(EntityID, "");
            DataItem.IsFullData = true;
            ExpressionEvaluator = formulaFunctionHandler.GetExpressionEvaluator(DataItem, MyProjectManager.GetMyProjectManager.GetRequester(), true);
            ExpressionEvaluator.PropertyCalled += ExpressionEvaluator_PropertyCalled;
            txtFormula.KeyUp += TxtFormula_KeyUp;
            txtFormula.TextChanged += TxtFormula_TextChanged;
            SetParametersTree();
            SetEditEntityArea();
            txtFormula.FontSize = 18;
            FormulaItems = new List<FormulaItemDTO>();
            if (!string.IsNullOrEmpty(formula))
                txtFormula.CaretPosition.InsertTextInRun(formula);
            var contextMenu = GenerateContextMenu();
            RadContextMenu.SetContextMenu(treeProperties, contextMenu);
            contextMenu.Opening += ContextMenu_Opening;
            //treeProperties.ContextMenu += MnuTree_ContextMenuOpening;
        }


        private void SetParametersTree()
        {
            var fNodeTitle = FormulaInstanceInternalHelper.GetObjectPrefrix();
            ItemCollection treeitems = null;
            NodeContext rootNodeContext = null;
            if (!string.IsNullOrEmpty(fNodeTitle))
            {
                rootNodeContext = new NodeContext(EntityID, fNodeTitle, Entity.Name)
                {
                    Name = fNodeTitle,
                    Title = fNodeTitle,
                    Order1 = 0,
                    NodeType = NodeType.MainVariable
                };
                var rootItem = AddNodeToTree(treeProperties.Items, rootNodeContext, false);
                rootItem.IsExpanded = true;
                treeitems = rootItem.Items;
            }
            else
            {
                rootNodeContext = new NodeContext(EntityID, fNodeTitle, Entity.Name);
                rootNodeContext.NodeType = NodeType.MainVariable;
                //     rootNodeContext.Context = Entity;
                treeitems = treeProperties.Items;
            }
            foreach (var nodeContext in GetNodes(rootNodeContext))
            {
                AddNodeToTree(treeitems, nodeContext);
            }
            int i = 0;
            foreach (var item in ExpressionEvaluator.GetExpressionBuiltinVariables().OrderBy(x => x.Name))
            {
                i++;
                var nodeContext = new NodeContext(item, item.Name, "") { Order1 = i, Name = item.Name, NodeType = NodeType.HelperProperty };
                AddNodeToTree(treeProperties.Items, nodeContext);

            }

            //foreach (var item in FormulaInstanceInternalHelper.GetExpressionBuiltinVariables().OrderBy(x => x.Key))
            //{
            //    var nodeContext = new NodeContext() { Order1 = 2, ParentPath = "", Context = item.Value, Name = item.Key, Title = item.Key, NodeType = NodeType.HelperProperty };
            //    AddNodeContext(null, nodeContext, false);
            //    SetNodes(nodeContext, item.Value);
            //}

        }
        private void SetTimers()
        {
            //کل متن را انتخاب میکند
            //selectionTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            //selectionTimer.Tick += SelectionTimer_Tick;

            textChangedCalculationTimer.Interval = new TimeSpan(0, 0, 0, 0, 2000);
            textChangedCalculationTimer.Tick += textChangedCalculationTimer_Tick;

            textChangedTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            textChangedTimer.Tick += TextChangedTimer_Tick;

            statesTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            statesTimer.Tick += StatesTimer_Tick;
        }



        private void TxtFormula_SelectionChanged(object sender, RoutedEventArgs e)
        {
            lblPointer.Text = txtFormula.Document.ContentStart.GetOffsetToPosition(txtFormula.CaretPosition).ToString();
        }

        private void txtClear_Click(object sender, RoutedEventArgs e)
        {
            txtFormula.SelectAll();
            txtFormula.Selection.Text = "";
        }

        bool skipTextChanged = false;
        private void TxtFormula_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!skipTextChanged)
            {
                textChangedCalculationTimer.Stop();
                textChangedCalculationTimer.Start();

                //           LastTextStates = AllTextStates;
                textChangedTimer.Stop();
                textChangedTimer.Start();
            }

        }
        Type calculatedType = null;
        private void textChangedCalculationTimer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();
            CalculateFormula();

        }

        private void CalculateFormula()
        {
            var output = GetFormulaText();
            // var text = new TextRange(new DocumentPosition()., txtFormula.Document.ContentEnd).Text;
            try
            {
                var res = ExpressionEvaluator.Calculate(output);
                if (res != null)
                {
                    lblResult.Text = res.ToString();
                    calculatedType = res.GetType();
                }
                else
                    lblResult.Text = "Null";
            }
            catch (Exception ex)
            {
                lblResult.Text = "Exception:" + " " + ex.Message;
                calculatedType = null;
            }
        }

        private void TextChangedTimer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();
            ClearColor();

            //var list = GetListOfBlocks(txtFormula.Document.ContentEnd);
            //list = list.Where(x => x.Item3 != "").ToList();
            //list.Reverse();

            var chains = GetTextStateChains(txtFormula.Document.ContentEnd);
            SetColor(chains.Item2, true);

            statesTimer.Stop();
            statesTimer.Start();
        }
        private void StatesTimer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            var chains = GetTextStateChains(txtFormula.Document.ContentEnd);
            AllTextStates = chains.Item2;
            lstStates.Items.Clear();
            foreach (var item in AllTextStates)
            {
                lstStates.Items.Add(item.Text + " : " + item.StartOffset + " , " + item.ActualEndOffset + " , " + item.EmptySpaceAfter + " , " + txtFormula.Document.ContentStart.GetOffsetToPosition(item.StartPointer) + " , " + txtFormula.Document.ContentStart.GetOffsetToPosition(item.EndPointer));
            }
        }
        private void ClearColor()
        {
            skipTextChanged = true;
            var alltextRange = new TextRange(txtFormula.Document.ContentStart, txtFormula.Document.ContentEnd);
            alltextRange.ApplyPropertyValue(TextElement.ForegroundProperty­, Brushes.Black);
            alltextRange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            skipTextChanged = false;
        }

        private void SetEditEntityArea()
        {
            MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.SetUIManager(new UIManager());
            var userInfo = new MyUILibrary.UserInfo();
            userInfo.AdminSecurityInfo = new MyUILibrary.AdminSecurityInfo() { IsActive = true, ByPassSecurity = true };
            MyUILibrary.AgentUICoreMediator.GetAgentUICoreMediator.UserInfo = userInfo;


            EditEntityAreaInitializer editEntityAreaInitializer1 = new EditEntityAreaInitializer();
            editEntityAreaInitializer1.EntityID = EntityID;
            editEntityAreaInitializer1.IntracionMode = CommonDefinitions.UISettings.IntracionMode.Select;
            editEntityAreaInitializer1.DataMode = CommonDefinitions.UISettings.DataMode.One;
            var FirstSideEditEntityAreaResult = EditEntityAreaConstructor.GetEditEntityArea(editEntityAreaInitializer1);
            if (FirstSideEditEntityAreaResult.Item1 != null && FirstSideEditEntityAreaResult.Item1 is I_EditEntityAreaOneData)
            {
                EditEntityArea = FirstSideEditEntityAreaResult.Item1 as I_EditEntityAreaOneData;
                EditEntityArea.SetAreaInitializer(editEntityAreaInitializer1);
                grdSelectData.Children.Add(EditEntityArea.TemporaryDisplayView as UIElement);
            }
        }
        private void TxtFormula_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            bool ctrlSpace = false;
            bool dot = false;
            if (e.Key == Key.Space && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                ctrlSpace = true;
            if (e.Key == Key.OemPeriod && !Keyboard.IsKeyDown(Key.RightShift) && !Keyboard.IsKeyDown(Key.LeftShift))//&& (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift)))
                dot = true;
            if (dot || ctrlSpace)
            {
                //if (dot)
                //{
                //    var prev = GetPreviousText(txtFormula.CaretPosition, true, true);
                //    if (prev != null )
                //    {
                //        CheckAutoComplete(prev.Item2);
                //    }

                //}
                //else
                //{

                var prev = GetPreviousText(txtFormula.CaretPosition, true, true);
                if (prev != null && prev.Item3 == ".")
                {
                    var prevText = GetPreviousText(prev.Item1, true, true);
                    if (prevText != null)
                    {
                        //var state = LastTextStates.Any(x => IsSameOffset(x, prevText.Item5));
                        CheckAutoComplete(prevText.Item5);


                    }
                }
                else if (ctrlSpace)
                    ShowAutoComplete(GetRootNodes());
                //}
            }
        }


        private void CheckAutoComplete(int offset)
        {
            if (AllTextStates != null)
            {
                var states = AllTextStates.Where(x => x.NextPossibleContexts != null && x.NextPossibleContexts.Any()).ToList();
                var state = states.FirstOrDefault(x => IsSameOffset(x, offset));
                if (state != null)
                {
                    ShowAutoComplete(state.NextPossibleContexts);
                }
            }
        }

        private Tuple<List<TextStateChain>, List<FormulaTextBlock>> GetTextStateChains(TextPointer point)
        {
            var list = GetListOfBlocks(point);
            list.Reverse();
            //if(setStateRanges)
            //{

            //}
            //list = list.Where(x => x.Item3 != "").ToList();

            var statementTree = GetBlocks(list);
            var chains = SetChainLevels(statementTree, GetOffset(txtFormula.Document.ContentStart), GetOffset(txtFormula.Document.ContentEnd));
            SetChainContext(chains.Item1, GetRootNodes());
            return chains;
        }

        private int GetOffset(TextPointer contentStart)
        {
            return txtFormula.Document.ContentStart.GetOffsetToPosition(contentStart);
        }

        private void SetChainContext(List<TextStateChain> chains, List<NodeContext> nodeDictionary)
        {
            foreach (var chain in chains)
            {
                if (!chain.TextStates.Any(x => x.IsLambdaSign))
                    SetTextBlocksContext(chain.TextStates, nodeDictionary);
            }
        }

        private void SetTextBlocksContext(List<FormulaTextBlock> textBlocks, List<NodeContext> parentNodes, int i = 0)
        {
            var sItem = textBlocks[i];


            sItem.ParentNodeContexts = parentNodes;
            List<NodeContext> foundContext = null;
            if (sItem.IsFunction)
            {
                foundContext = parentNodes.Where(x => x.NodeType == NodeType.DotNetMethod && x.Name == sItem.Text).ToList();
            }
            else
            {
                foundContext = parentNodes.Where(x => x.NodeType != NodeType.DotNetMethod && x.Name.Replace("?", "") == sItem.Text.Replace("?", "")).ToList();
            }
            sItem.PossibleContexts = foundContext;
            List<NodeContext> nextContext = new List<NodeContext>();
            foreach (var node in foundContext)
            {
                var nodes = GetNodes(node);
                if (nodes != null)
                    nextContext.AddRange(nodes);
            }
            //اینجا چون فعلا تشخیص نداریم که مثلا فانکشن چند پارامتری هست نمیتونیم خروجی دقیق رو تشخیص بدیم بنابراین همه رو میاریم
            sItem.NextPossibleContexts = nextContext;
            if (textBlocks.Count - 1 > i)
                if (nextContext != null && nextContext.Any())
                    SetTextBlocksContext(textBlocks, nextContext, i + 1);

            if (sItem.TextStateChains != null && sItem.TextStateChains.Any())
            {
                SetChainContext(sItem.TextStateChains, GetFunctionNodeContext(sItem));
            }
        }
        private List<NodeContext> GetFunctionNodeContext(FormulaTextBlock parentFunction)
        {
            List<NodeContext> result = null;
            //اینجااونجاست که لامبدا اکپرشن ها میتونن اضافه بشن y=> y.
            Tuple<string, int> tuple = null;
            if (!string.IsNullOrEmpty(parentFunction.LambdaText))
            {
                if (parentFunction.PossibleContexts != null && parentFunction.PossibleContexts.Any())
                {
                    foreach (var nodeContext in parentFunction.PossibleContexts)
                    {

                        if (nodeContext.ParentNode.Context is MyPropertyInfo)
                        {
                            if ((nodeContext.ParentNode.Context as MyPropertyInfo).PropertyType == ProxyLibrary.PropertyType.Relationship)
                            {
                                //        var entityAndProperties = GetEntityAndProperties((nodeContext.ParentNode.Context as MyPropertyInfo).PropertyRelationship.EntityID2);
                                //        if (entityAndProperties != null)
                                //        {
                                //            tuple = new Tuple<string, EntityAndProperties>(parentFunction.LambdaText, entityAndProperties);
                                //        }
                                //    }

                                //}
                                tuple = new Tuple<string, int>(parentFunction.LambdaText, (nodeContext.ParentNode.Context as MyPropertyInfo).PropertyRelationship.EntityID2);
                            }
                        }

                    }
                }
            }
            result = GetRootNodes();
            if (tuple == null || tuple.Item2 == null)
            {

            }
            else
            {
                result.Add(new NodeContext(tuple.Item2, tuple.Item1, "") { Name = tuple.Item1, NodeType = NodeType.Lambda });
            }
            return result;
        }

        private Tuple<List<TextStateChain>, List<FormulaTextBlock>> SetChainLevels(List<CodeBlock> statementTree, int start, int end, int level = 0, List<FormulaTextBlock> alltextBlocks = null, FormulaTextBlock parentFunction = null)
        {
            List<TextStateChain> chains = new List<MyProject_WPF.TextStateChain>();
            if (alltextBlocks == null)
                alltextBlocks = new List<FormulaTextBlock>();
            //var result = new List<MyProject_WPF.TextStateChain>();

            foreach (var item in statementTree.Where(x => x.IsText))
            {
                if (RangeContainsBlock(start, end, item))
                {
                    if (!chains.Any(x => RangeContainsBlock(x.StartOffset, x.EndOffset, item)))
                    {
                        TextStateChain rItem = new TextStateChain();
                        rItem.ParentFunction = parentFunction;
                        rItem.TextStates = CheckCallChain(statementTree, item, null, null, parentFunction);
                        alltextBlocks.AddRange(rItem.TextStates);
                        //if (rItem.TextStates.Any())
                        //{
                        //    rItem.StartOffset = rItem.TextStates[0].StartOffset;
                        //    rItem.EndOffset = rItem.TextStates[rItem.TextStates.Count - 1].ActualEndOffset;
                        //}
                        chains.Add(rItem);
                    }
                }
            }
            foreach (var item in chains)
            {
                foreach (var fItem in item.TextStates.Where(x => x.IsFunction))
                {
                    var fResult = SetChainLevels(statementTree, fItem.FunctionParanteseStart.EndOffset, fItem.FunctionParanteseEnd.StartOffset, level + 1, alltextBlocks, fItem);
                    fItem.TextStateChains = fResult.Item1;
                }
            }
            return new Tuple<List<MyProject_WPF.TextStateChain>, List<MyProject_WPF.FormulaTextBlock>>(chains, alltextBlocks);
        }

        private bool RangeContainsBlock(int start, int end, CodeBlock item)
        {
            return start <= item.StartOffset
                  && end >= item.EndOffset;
        }

        private List<FormulaTextBlock> CheckCallChain(List<CodeBlock> statementTree, CodeBlock item, CodeBlock lastItem = null, List<FormulaTextBlock> result = null, FormulaTextBlock parentFunction = null)
        {
            if (result == null)
                result = new List<FormulaTextBlock>();

            if (item is FormulaTextBlock)
            {
                if (result.Any())
                    (item as FormulaTextBlock).LastItem = result.Last();
                result.Add(item as FormulaTextBlock);
            }

            if (statementTree.IndexOf(item) != statementTree.Count - 1)
            {
                var nextItem = statementTree[statementTree.IndexOf(item) + 1];
                if ((item is FormulaTextBlock) && lastItem == null)
                {
                    if (nextItem.Text == "=")
                    {
                        if (statementTree.IndexOf(nextItem) != statementTree.Count - 1)
                        {
                            var nextnextItem = statementTree[statementTree.IndexOf(nextItem) + 1];
                            if (nextnextItem.Text == ">")
                            {
                                if (parentFunction != null)
                                {
                                    parentFunction.LambdaText = item.Text;
                                    (item as FormulaTextBlock).IsLambdaSign = true;
                                }
                            }
                        }
                    }
                }
                bool canContinue = false;
                //bool nextCanAcceptOnlyDot = false;
                if (item is FormulaTextBlock)
                {
                    canContinue = true;
                }
                else if ((item is NonTextBlock) && (item as NonTextBlock).IsDot)
                {
                    if (nextItem is FormulaTextBlock)
                        CheckCallChain(statementTree, nextItem, item, result, parentFunction);
                }
                else if ((item is NonTextBlock) && (item as NonTextBlock).IsStartingParantese && (item as NonTextBlock).PairBlock != null)
                {
                    if (lastItem is FormulaTextBlock)
                    {
                        (lastItem as FormulaTextBlock).IsFunction = true;
                        (lastItem as FormulaTextBlock).FunctionParanteseStart = item as NonTextBlock;
                        (lastItem as FormulaTextBlock).FunctionParanteseEnd = (item as NonTextBlock).PairBlock as NonTextBlock;
                    }
                    item = (item as NonTextBlock).PairBlock;
                    if (statementTree.IndexOf(item) != statementTree.Count - 1)
                    {
                        nextItem = statementTree[statementTree.IndexOf(item) + 1];
                        if (nextItem is NonTextBlock)
                            if ((nextItem as NonTextBlock).IsDot)
                                canContinue = true;
                    }
                }
                if (canContinue)
                    CheckCallChain(statementTree, statementTree[statementTree.IndexOf(item) + 1], item, result, parentFunction);
                //}
            }



            return result;
        }

        private List<CodeBlock> GetBlocks(List<Tuple<TextPointer, TextPointer, string, int, int>> list, int j = 0)
        {
            List<CodeBlock> result = new List<CodeBlock>();
            for (int i = j; i <= list.Count - 1; i++)
            {
                //if (txtFormula.Document.ContentEnd.GetOffsetToPosition(list[i].Item2) >= 0)
                //{
                //    if (result.Any())
                //    {
                //        result[i - 1].IsEndOfDocument = true;
                //        continue;
                //    }
                //}
                CodeBlock rItem = null;
                if (list[i].Item3 == "" || list[i].Item3 == "\r\n")
                {
                    if (result.Any())
                        result[result.Count - 1].EmptySpaceAfter += 1;

                    continue;
                }
                bool isText = IsText(list[i].Item3);
                if (isText)
                {
                    rItem = new FormulaTextBlock(txtFormula.Document.ContentStart);
                }
                else
                {
                    rItem = new NonTextBlock(txtFormula.Document.ContentStart);
                }
                rItem.IsText = isText;

                rItem.StartPointer = list[i].Item1;
                //rItem.Offset = txtFormula.Document.ContentStart.GetOffsetToPosition(rItem.StartPointer);
                rItem.EndPointer = list[i].Item2;
                rItem.Text = list[i].Item3;

                result.Add(rItem);

            }

            SetNonTextBlockProperties(result.Where(x => x is NonTextBlock).Cast<NonTextBlock>().ToList());
            return result;
        }

        //private bool IsEndOfDocument(CodeBlock rItem)
        //{
        //    if (GetNextText(rItem.EndPointer, true, false) == null)
        //        return true;
        //    else
        //        return false;
        //}

        private bool IsText(string item3)
        {
            return item3.Any() && IsValidChar(item3[0]);
        }

        private void SetNonTextBlockProperties(List<NonTextBlock> result)
        {
            foreach (var item in result)
            {
                var block = item as NonTextBlock;
                if (block.Text == "(")
                {
                    block.IsStartingParantese = true;
                }
                else if (block.Text == ")")
                {
                    block.IsEndingParantese = true;
                }
                else if (item.Text == ".")
                {
                    block.IsDot = true;
                }
            }
            foreach (var item in result.Where(x => x.IsStartingParantese))
            {
                SetPairParantese(result, item);
            }
        }

        private void SetPairParantese(List<NonTextBlock> result, NonTextBlock item, int debth = 0)
        {
            for (int i = result.IndexOf(item) + 1; i <= result.Count - 1; i++)
            {

                if (debth == 0 && result[i].IsEndingParantese)
                {
                    item.PairBlock = result[i];
                    result[i].PairBlock = item;
                }
                else if (result[i].IsStartingParantese)
                    debth--;
                else if (result[i].IsEndingParantese)
                {
                    debth++;
                    if (debth > 0)
                        break;
                }
            }
        }
        private List<Tuple<TextPointer, TextPointer, string, int, int>> GetListOfBlocks(TextPointer item1, List<Tuple<TextPointer, TextPointer, string, int, int>> result = null)
        {
            if (result == null)
                result = new List<Tuple<TextPointer, TextPointer, string, int, int>>();
            var previousWord = GetPreviousText(item1, true, false);
            if (previousWord != null)
            {
                result.Add(new Tuple<TextPointer, TextPointer, string, int, int>(previousWord.Item1, previousWord.Item2, previousWord.Item3, txtFormula.Document.ContentStart.GetOffsetToPosition(previousWord.Item1), txtFormula.Document.ContentStart.GetOffsetToPosition(previousWord.Item2)));
                GetListOfBlocks(previousWord.Item1, result);
            }
            return result;
        }
        //private Tuple<TextPointer, TextPointer, string> GetCurrentWord(TextPointer item1)
        //{
        //    Tuple<TextPointer, TextPointer, string> prevText;
        //    Tuple<TextPointer, TextPointer, string> nextText;
        //    prevText = GetPreviousText(item1, false);
        //    nextText = GetNextText(item1, false);
        //    TextPointer start;
        //    TextPointer end;

        //    string prevContent = "";
        //    string nextContent = "";
        //    if (prevText == null)
        //        start = item1;
        //    else
        //    {
        //        prevContent = prevText.Item3;
        //        start = prevText.Item1;
        //    }
        //    if (nextText == null)
        //        end = item1;
        //    else
        //    {
        //        nextContent = nextText.Item3;
        //        end = nextText.Item2;
        //    }
        //    return new Tuple<TextPointer, TextPointer, string>(start, end, prevContent + nextContent);
        //}
        private Tuple<TextPointer, TextPointer, string, int, int> GetPreviousText(TextPointer start, bool includeSingleNonText, bool ignoreEmptySpace)
        {
            var len = GetPreviousTextLen(start, includeSingleNonText, ignoreEmptySpace);
            if (len == 0)
                return null;
            else
                return GetTextRangeTuple(start, len);
        }
        private int GetPreviousTextLen(TextPointer cStart, bool includeSingleNonText, bool ignoreEmptySpace, int result = 0)
        {
            if (txtFormula.Document.ContentStart.GetOffsetToPosition(cStart) <= 0)
                return result;

            var prevChar = GetTextRangeTuple(cStart, -1);
            if (ignoreEmptySpace && prevChar.Item3 == "")
            {
                result--;
                return GetPreviousTextLen(prevChar.Item1, includeSingleNonText, ignoreEmptySpace, result);
            }
            else
            {
                if (prevChar.Item3 != "" && IsValidChar(prevChar.Item3[0]))
                {
                    result--;
                    return GetPreviousTextLen(prevChar.Item1, includeSingleNonText, ignoreEmptySpace, result);
                }
                else
                {
                    if (includeSingleNonText && result == 0)
                        result--;
                }
            }
            return result;
        }
        private Tuple<TextPointer, TextPointer, string, int, int> GetNextText(TextPointer start, bool includeSingleNonText, bool ignoreEmptySpace)
        {
            var len = GetNextTextLen(start, includeSingleNonText, ignoreEmptySpace);
            if (len == 0)
                return null;
            else
                return GetTextRangeTuple(start, len);
        }
        private Tuple<TextPointer, TextPointer, string, int, int> GetTextRangeTuple(TextPointer position, int len)
        {
            TextPointer start;
            TextPointer end;
            if (len >= 0)
            {
                start = position;
                end = start.GetPositionAtOffset(len);
            }
            else
            {

                start = position.GetPositionAtOffset(len);
                end = position;
            }
            TextRange r = new TextRange(start, end);
            String text = r.Text;
            return new Tuple<TextPointer, TextPointer, string, int, int>(start, end, text, txtFormula.Document.ContentStart.GetOffsetToPosition(start), txtFormula.Document.ContentStart.GetOffsetToPosition(end));
        }
        private int GetNextTextLen(TextPointer cStart, bool includeSingleNonText, bool ignoreEmptySpace, int result = 0)
        {
            if (txtFormula.Document.ContentEnd.GetOffsetToPosition(cStart) >= 0)
                return result;
            var nextChar = GetTextRangeTuple(cStart, 1);
            if (ignoreEmptySpace && nextChar.Item3 == "")
            {
                result++;
                return GetPreviousTextLen(nextChar.Item2, includeSingleNonText, ignoreEmptySpace, result);
            }
            else
            {
                if (nextChar.Item3 != "" && IsValidChar(nextChar.Item3[0]))
                {
                    result++;
                    return GetNextTextLen(nextChar.Item2, includeSingleNonText, ignoreEmptySpace, result);
                }
                else
                {
                    if (includeSingleNonText && result == 0)
                        result++;

                }
            }
            return result;
        }
        private TextRange GetTextRange(TextPointer start, int len)
        {
            var end = start.GetPositionAtOffset(len);
            return new TextRange(start, end);
        }

        private bool IsValidChar(char ch)
        {
            return Char.IsLetterOrDigit(ch) || new List<char>() { '_', '?' }.Contains(ch);
        }
        private bool IsValidChar(char ch, List<char> validChars)
        {
            return Char.IsLetterOrDigit(ch) || (validChars != null && validChars.Contains(ch));
        }
        private bool IsSameOffset(FormulaTextBlock state, int offset)
        {
            //var res = actualEndPointer.ActualEndOffset - offset;
            bool aa = offset >= state.ActualEndOffset - 1 && offset <= state.ActualEndOffset + state.EmptySpaceAfter;
            return aa;
        }
        private void SetColor(List<FormulaTextBlock> textStates, bool first)
        {
            //return;
            skipTextChanged = true;
            List<Tuple<TextRange, int, SolidColorBrush, bool>> colors = new List<Tuple<TextRange, int, SolidColorBrush, bool>>();
            foreach (var textstate in textStates)
            {
                var textRange = new TextRange(textstate.StartPointer, textstate.EndPointer);
                if (textstate.PossibleContexts != null && textstate.PossibleContexts.Any())
                {
                    if (textstate.IsFunction)
                    {
                        var parEnd = new TextRange(textstate.FunctionParanteseEnd.StartPointer, textstate.FunctionParanteseEnd.EndPointer);
                        colors.Add(new Tuple<TextRange, int, SolidColorBrush, bool>(parEnd, txtFormula.Document.ContentStart.GetOffsetToPosition(parEnd.Start), Brushes.Red, false));
                        var parStart = new TextRange(textstate.FunctionParanteseStart.StartPointer, textstate.FunctionParanteseStart.EndPointer);
                        colors.Add(new Tuple<TextRange, int, SolidColorBrush, bool>(parStart, txtFormula.Document.ContentStart.GetOffsetToPosition(parStart.Start), Brushes.Red, false));
                        colors.Add(new Tuple<TextRange, int, SolidColorBrush, bool>(textRange, textstate.StartOffset, Brushes.Red, false));
                    }
                    else
                        colors.Add(new Tuple<TextRange, int, SolidColorBrush, bool>(textRange, textstate.StartOffset, Brushes.Blue, false));
                }
                else
                {
                    colors.Add(new Tuple<TextRange, int, SolidColorBrush, bool>(textRange, textstate.StartOffset, null, true));
                }
            }
            //اینجا لیست رو میسازیم و از ته شروع میکنیم. چون اگه رنج های قبل رو مثلا رنگی کنیم
            // رنج های بعدی جابجا میشن. زیرا خود رنگ یک رنج حساب میشه
            foreach (var color in colors.OrderByDescending(x => x.Item2))
            {
                if (color.Item4)
                {
                    color.Item1.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                }
                else
                {
                    color.Item1.ApplyPropertyValue(TextElement.ForegroundProperty­, color.Item3);
                }
            }
            skipTextChanged = false;
        }


        private string GetFormulaText()
        {
            TextRange textRange = new TextRange(
                 txtFormula.Document.ContentStart,
                txtFormula.Document.ContentEnd
                   );
            return textRange.Text;
        }

        private void FormulaAutoComplete_NodeSelected(object sender, NodeSelectedArg e)
        {
            NodeSelected(e.Title);
        }
        //string selectedNodeText;
        TextPointer selectedNodePosition;

        private void NodeSelected(string text)
        {
            int lenth = text.Length;
            if (text.ToLower().Contains("string criteria"))
            {
                text = text.Replace("string criteria", "\"z=>z.\"").Replace("String Criteria", "\"z=>z.\"").Replace("String criteria", "\"z=>z.\"");
                lenth = text.Length - 2;
            }
            selectedNodePosition = txtFormula.CaretPosition;
            txtFormula.CaretPosition.InsertTextInRun(text);

            var newPos = selectedNodePosition.GetPositionAtOffset(lenth);
            if (newPos != null)
                txtFormula.CaretPosition = newPos;
            //return;
            //   selectionTimer.Start();
            if (autoCompleteWindow != null && autoCompleteWindow.IsOpen)
                autoCompleteWindow.Close();
            txtFormula.Focus();
        }

        //private void SelectionTimer_Tick(object sender, EventArgs e)
        //{
        //    (sender as DispatcherTimer).Stop();
        //    if (selectedNodeText.Contains("(") && selectedNodeText.Contains(")"))
        //    {
        //        var fIndex = selectedNodeText.IndexOf("(");
        //        var lIndex = selectedNodeText.IndexOf(")");
        //        var lenght = lIndex - fIndex;
        //        var paramStr = selectedNodeText.Substring((fIndex + 1), lenght - 1);

        //        if (!string.IsNullOrEmpty(paramStr))
        //        {
        //            if (!paramStr.Contains("[]"))
        //            {
        //                try
        //                {
        //                    var paramRange = GetTextRange(selectedNodePosition.GetPositionAtOffset(fIndex + 1), paramStr.Length);
        //                    // paramRange.Select(paramRange.Start, paramRange.End);
        //                    txtFormula.Selection.Select(paramRange.Start, paramRange.End);
        //                    //         paramRange.ApplyPropertyValue(TextElement.ForegroundProperty­, Brushes.Green);

        //                }
        //                catch
        //                {

        //                }
        //            }
        //        }
        //    }


        //}

        RadWindow autoCompleteWindow;
        private void ShowAutoComplete(List<NodeContext> nodes)
        {

            formulaAutoComplete.SetTree(nodes);
            if (autoCompleteWindow == null)
            {
                autoCompleteWindow = new RadWindow();
                autoCompleteWindow.Header = "";
                autoCompleteWindow.Content = formulaAutoComplete;
                autoCompleteWindow.HideMinimizeButton = true;
                autoCompleteWindow.HideMaximizeButton = true;
                //   autoCompleteWindow.SizeToContent = true;
                autoCompleteWindow.ResizeMode = ResizeMode.NoResize;
            }
            autoCompleteWindow.ShowDialog();
        }
        private List<NodeContext> GetRootNodes()
        {
            List<NodeContext> list = new List<NodeContext>();
            foreach (RadTreeViewItem item in treeProperties.Items)
            {
                if ((item.DataContext is NodeContext))
                {
                    list.Add(item.DataContext as NodeContext);
                }
            }
            return list;
        }



        //private RadTreeViewItem AddNodeContext(NodeContext parentNodeContext, NodeContext nodeContext, bool lateExpand)
        //{
        //    string parentPath = "";
        //    if (parentNodeContext != null)
        //        parentPath = (string.IsNullOrEmpty(parentNodeContext.ParentPath) ? "" : parentNodeContext.ParentPath + ".") + parentNodeContext.Name;
        //    nodeContext.ParentPath = parentPath;
        //    nodeContext.ParentNode = parentNodeContext;

        //    if (parentNodeContext != null)
        //    {
        //        if (parentNodeContext.ChildNodes == null)
        //            parentNodeContext.ChildNodes = new List<NodeContext>();
        //        parentNodeContext.ChildNodes.Add(nodeContext);
        //    }
        //    else
        //        nodeDictionary.Add(nodeContext);
        //    if (parentNodeContext != null && parentNodeContext.UIItem == null)
        //        return null;

        //    var fnode = AddNodeToTree(nodeContext, lateExpand);
        //    nodeContext.UIItem = fnode;
        //    return fnode;


        //}

        private RadTreeViewItem AddNodeToTree(ItemCollection items, NodeContext nodeContext, bool lateExpand = true)
        {
            RadTreeViewItem node = new RadTreeViewItem();
            node.DataContext = nodeContext;
            node.Header = GetHeader(node);
            if (!string.IsNullOrEmpty(nodeContext.Tooltip))
                node.ToolTip = nodeContext.Tooltip;
            // node.DoubleClick += Node_DoubleClick;

            if (lateExpand)
            {
                node.Expanded += Node_Expanded;
                node.Items.Add("aaa");
            }
            items.Add(node);
            return node;
        }
        private void Node_Expanded(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (sender is RadTreeViewItem)
            {
                var node = (sender as RadTreeViewItem);

                if (node.Items.Count == 1 && node.Items[0].ToString() == "aaa")
                {
                    SetNodeChilds(node);
                }
            }
        }

        private void SetNodeChilds(RadTreeViewItem node)
        {
            node.Items.Clear();
            var nodeContext = (node.DataContext as NodeContext);
            var cnodes = GetNodes(nodeContext);
            if (cnodes != null)
            {
                foreach (var cnode in cnodes)
                {
                    AddNodeToTree(node.Items, cnode);
                }
            }
        }




        //private List<NodeContext> GetNodes(TableDrivedEntityDTO tableDrivedEntityDTO)
        //{
        //    var entityAndProperties = GetEntityAndProperties(tableDrivedEntityDTO);
        //    return GetNodes(entityAndProperties);
        //}
        private List<NodeContext> GetNodes(NodeContext nodeContext)
        {
            List<NodeContext> result = null;
            if (nodeContext.Context is Int32 ||
                (nodeContext.Context is MyPropertyInfo && (nodeContext.Context as MyPropertyInfo).Type == typeof(MyCustomSingleData)))
            {
                result = GetNodes(GetEntityID(nodeContext));
            }
            else if (nodeContext.Context is MethodInfo && (nodeContext.Context as MethodInfo).ReturnType == typeof(MyCustomSingleData)
                && nodeContext.ParentNode != null && nodeContext.ParentNode.Context is MyPropertyInfo && (nodeContext.ParentNode.Context as MyPropertyInfo).Type == typeof(MyCustomMultipleData))
                result = GetNodes(GetEntityID(nodeContext.ParentNode));
            else
            {
                var tuple = GetReturnType(nodeContext);
                result = GetNodes(tuple.Item1, tuple.Item2);
            }

            //if (nodeContext.Context is MyPropertyInfo
            //   || ((nodeContext.Context is MethodInfo) && (nodeContext.Context as MethodInfo).ReturnType == typeof(MyCustomSingleData)))
            //{
            //    if (nodeContext.Context is MyPropertyInfo)
            //    {
            //        if ((nodeContext.Context as MyPropertyInfo).PropertyType == ProxyLibrary.PropertyType.Relationship)
            //        {
            //            if ((nodeContext.Context as MyPropertyInfo).PropertyRelationship.TypeEnum == Enum_RelationshipType.OneToMany)
            //            {
            //                result = GetNodes(typeof(MyCustomMultipleData));
            //            }
            //            else
            //            {
            //                var entityAndProperties = GetEntityAndProperties((nodeContext.Context as MyPropertyInfo).PropertyRelationship.EntityID2);
            //                if (entityAndProperties != null)
            //                {
            //                    result = GetNodes(entityAndProperties);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            result = GetNodes((nodeContext.Context as MyPropertyInfo).Type);
            //        }
            //    }
            //    else
            //    {
            //        var parent = nodeContext.ParentNode;
            //        if (parent.Context is MyPropertyInfo)
            //        {
            //            if ((parent.Context as MyPropertyInfo).PropertyType == ProxyLibrary.PropertyType.Relationship)
            //            {
            //                var entityAndProperties = GetEntityAndProperties((parent.Context as MyPropertyInfo).PropertyRelationship.EntityID2);
            //                if (entityAndProperties != null)
            //                {
            //                    result = GetNodes(entityAndProperties);
            //                }
            //            }

            //        }
            //    }
            //}
            //else if (nodeContext.NodeType == NodeType.DotNetProperty)
            //{
            //    result = GetNodes((nodeContext.Context as MyProp).Type);
            //}
            //else if (nodeContext.NodeType == NodeType.DotNetMethod)
            //{
            //    result = GetNodes((nodeContext.Context as MethodInfo).ReturnType);
            //}
            //else if (nodeContext.NodeType == NodeType.Lambda)
            //{
            //    result = GetNodes(nodeContext.Context as EntityAndProperties);
            //}
            //else if (nodeContext.NodeType == NodeType.MainVariable)
            //{
            //    result = GetNodes(nodeContext.Context as TableDrivedEntityDTO);
            //}
            //else if (nodeContext.NodeType == NodeType.HelperProperty)
            //{
            //    result = GetNodes(nodeContext.Context as BuiltinRefClass);
            //}
            if (result != null)
                foreach (var item in result)
                    item.ParentNode = nodeContext;
            return result;
        }

        private int GetEntityID(NodeContext nodeContext)
        {
            if (nodeContext.Context is Int32)
                return (int)nodeContext.Context;
            //else if (nodeContext.Context is MyCustomSingleData)
            //    return (nodeContext.Context as MyCustomSingleData).DataItem.TargetEntityID;
            else if (nodeContext.Context != null && nodeContext.Context is MyPropertyInfo)
            {
                if ((nodeContext.Context as MyPropertyInfo).PropertyRelationship != null)
                {
                    return (nodeContext.Context as MyPropertyInfo).PropertyRelationship.EntityID2;
                }
            }
            return 0;
        }

        private Tuple<Type, bool> GetReturnType(NodeContext context)
        {
            if (context.Context is MyPropertyInfo)
            {
                return new Tuple<Type, bool>((context.Context as MyPropertyInfo).Type, false);
            }
            else if (context.Context is MethodInfo)
            {
                return new Tuple<Type, bool>((context.Context as MethodInfo).ReturnType, false);
            }
            else if (context.Context is MyProp)
            {
                return new Tuple<Type, bool>((context.Context as MyProp).Type, false);
            }
            else if (context.Context is BuiltinRefClass)
            {
                if ((context.Context as BuiltinRefClass).IsType)
                    return new Tuple<Type, bool>((context.Context as BuiltinRefClass).Type, true);
                else if ((context.Context as BuiltinRefClass).IsObject)
                    return new Tuple<Type, bool>((context.Context as BuiltinRefClass).Type, false);
            }
            return null;
        }

        private List<NodeContext> GetNodes(int entityID)
        {
            var result = new List<NodeContext>();
            EntityAndProperties entityAndProperties = null;
            if (!EntityAndProperties.Any(x => x.EntityID == entityID))
            {
                var entity = bizTableDrivedEntity.GetPermissionedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID);
                if (entity != null)
                {
                    var properties = FormulaInstanceInternalHelper.GetProperties(MyProjectManager.GetMyProjectManager.GetRequester(), entity, null, true).Select(x => x.Value).ToList();
                    entityAndProperties = new EntityAndProperties() { EntityID = entityID, Properties = properties };
                    EntityAndProperties.Add(entityAndProperties);
                }
            }
            else
            {
                entityAndProperties = EntityAndProperties.First(x => x.EntityID == entityID);
            }
            if (entityAndProperties != null)
            {

                int i = 0;
                foreach (var property in entityAndProperties.Properties.OrderBy(x => x.Name))
                {
                    NodeContext nodeContext = new NodeContext(property, property.Name, property.Tooltip);
                    nodeContext.Name = property.Name;
                    //nodeContext.ReturnType = property.Type;
                    //  nodeContext.Context = property;
                    if (property.PropertyType == PropertyType.Relationship)
                        nodeContext.NodeType = NodeType.RelationshipProperty;
                    else
                        nodeContext.NodeType = NodeType.CustomProperty;
                    nodeContext.Order1 = i;
                    i++;
                    result.Add(nodeContext);
                }
            }
            return result;
        }
        private List<NodeContext> GetNodes(Type type, bool isStatic)
        {
            return GetPropertyAndMethods(type, isStatic);
        }
        //private List<NodeContext> GetNodes(BuiltinRefClass item)
        //{
        //    List<NodeContext> result = new List<NodeContext>();
        //    if (item.IsType)
        //    {
        //        result.AddRange(GetPropertyAndMethods(item.Type, true));
        //    }
        //    if (item.IsObject)
        //    {
        //        result.AddRange(GetPropertyAndMethods(item.Type, false));
        //    }
        //    return result;
        //}

        private List<NodeContext> GetPropertyAndMethods(Type type, bool isStatic)
        {
            List<NodeContext> result = new List<NodeContext>();
            int i = 0;

            List<MyProp> list = new List<MyProp>();

            List<PropertyInfo> properties = null;
            var staticproperties = type.GetProperties(BindingFlags.Static).ToList();
            if (isStatic)
                properties = staticproperties;
            else
                properties = type.GetProperties().Where(x => !staticproperties.Any(y => y.Name == x.Name)).ToList();
            foreach (var prop in properties)
                list.Add(new MyProp(prop.Name, prop.PropertyType));

            foreach (var field in type.GetFields().Where(x => x.IsPublic && x.IsStatic == isStatic))
            {
                list.Add(new MyProp(field.Name, field.FieldType));
            }

            foreach (var prop in list.OrderBy(x => x.Name))
            {
                NodeContext nodeContext = new NodeContext(prop, prop.Name, "ReturnType : " + prop.Type);
                nodeContext.Name = prop.Name;
                nodeContext.NodeType = NodeType.DotNetProperty;
                nodeContext.Order1 = i;
                i++;
                result.Add(nodeContext);
            }
            List<MethodInfo> methods = new List<MethodInfo>();
            //List<Type> AssTypes = new List<Type>();

            //foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    AssTypes.AddRange(item.GetTypes());
            //}
            //var query = from AssType in AssTypes
            //            where AssType.IsSealed && !AssType.IsGenericType && !AssType.IsNested
            //            from method in AssType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            //            where method.IsDefined(typeof(ExtensionAttribute), false)
            //            where method.GetParameters()[0].ParameterType == type
            //            select method;
            //var mets = query.ToArray<MethodInfo>();
            //methods.AddRange(mets.Where(x => x.IsPublic ));
            methods.AddRange(type.GetMethods().Where(x => x.IsPublic && x.IsStatic == isStatic));
            foreach (var method in methods.OrderBy(x => x.Name))
            {
                if (method.CustomAttributes.Any(x => x.AttributeType == typeof(CompilerGeneratedAttribute)))
                {
                    continue;
                }
                var methodParamStr = "";
                var paramList = method.GetParameters();
                var methodName = method.Name;
                var paramsStr = "";
                if (paramList.Count() > 0)
                {
                    foreach (var param in paramList)
                    {
                        //if (item.Type == typeof(Enumerable) && param.Name.ToLower() == "source")
                        //    continue;
                        paramsStr += (paramsStr == "" ? "" : ",") + param.ParameterType.Name + " " + param.Name;
                    }
                    methodParamStr += "(" + paramsStr + ")";
                }
                else
                    methodParamStr += "()";


                NodeContext nodeContext = new NodeContext(method, methodName + methodParamStr, "ReturnType : " + method.ReturnType);
                nodeContext.Name = methodName;
                nodeContext.NodeType = NodeType.DotNetMethod;
                nodeContext.Order1 = i;
                i++;
                result.Add(nodeContext);
            }
            return result;
        }

        //EntityAndProperties GetEntityAndProperties(string entityName)
        //{
        //    if (!EntityAndProperties.Any(x => x.Entity.Name.ToLower() == entityName.ToLower()))
        //    {
        //        var entity = bizTableDrivedEntity.GetPermissionedEntityByName(MyProjectManager.GetMyProjectManager.GetRequester(), Entity.DatabaseID, entityName);
        //        if (entity != null)
        //        {
        //            return GetEntityAndProperties(entity);
        //        }
        //        else
        //            return null;
        //    }
        //    else
        //    {
        //        return EntityAndProperties.First(x => x.Entity.Name == entityName);
        //    }
        //}
        //EntityAndProperties GetEntityAndProperties(int entityID)
        //{

        //}
        //EntityAndProperties GetEntityAndProperties(TableDrivedEntityDTO entity)
        //{

        //}
        private object GetHeader(RadTreeViewItem node)
        {
            var context = node.DataContext as NodeContext;
            StackPanel pnl = new StackPanel();
            pnl.Orientation = Orientation.Horizontal;
            System.Windows.Controls.TextBlock lbl = new System.Windows.Controls.TextBlock();
            lbl.Text = context.Title;
            Image img = new Image();
            img.Source = GetPropertyImage(context);
            img.Width = 15;
            pnl.Children.Add(img);
            pnl.Children.Add(lbl);

            //if (context.Context is MyPropertyInfo)
            //{
            Image imgAdd = new Image();
            imgAdd.Source = new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/addnew.png", UriKind.Relative));
            imgAdd.Width = 15;
            imgAdd.Cursor = Cursors.Hand;
            imgAdd.MouseLeftButtonUp += (sender, e) => ImgAdd_MouseLeftButtonUp(sender, e, node);
            pnl.Children.Add(imgAdd);

            Image imgAddWithPath = new Image();
            imgAddWithPath.Source = new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/datalink.png", UriKind.Relative));
            imgAddWithPath.Width = 15;
            imgAddWithPath.Cursor = Cursors.Hand;
            imgAddWithPath.MouseLeftButtonUp += (sender, e) => ImgAddWithPath_MouseLeftButtonUp(sender, e, node);

            pnl.Children.Add(imgAddWithPath);
            //}
            return pnl;
        }

        private void ImgAdd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e, RadTreeViewItem node)
        {
            var context = node.DataContext as NodeContext;
            NodeSelected(context.Title);
        }
        private void ImgAddWithPath_MouseLeftButtonUp(object sender, MouseButtonEventArgs e, RadTreeViewItem node)
        {
            var context = node.DataContext as NodeContext;
            var text = GetNodePath(context, context.Title);
            NodeSelected(text);
        }
        private string GetNodePath(NodeContext node, string currentPath = "")
        {
            if (currentPath == "")
                currentPath = node.Title;
            if (node.ParentNode != null)
            {
                currentPath = node.ParentNode.Title + "." + currentPath;
                return GetNodePath(node.ParentNode, currentPath);
            }
            else
            {
                return currentPath;
            }
        }
        private ImageSource GetPropertyImage(NodeContext nodeContext)
        {
            NodeType propertyType = nodeContext.NodeType;

            if (nodeContext.Context != null && (nodeContext.Context as MyPropertyInfo) != null)
            {

                if ((nodeContext.Context as MyPropertyInfo).PropertyType == PropertyType.Column)
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/column.png", UriKind.Relative));
                else if ((nodeContext.Context as MyPropertyInfo).PropertyType == PropertyType.DBFunction)
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/dbformula.png", UriKind.Relative));
                else if ((nodeContext.Context as MyPropertyInfo).PropertyType == PropertyType.Code)
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/function.png", UriKind.Relative));
                else if ((nodeContext.Context as MyPropertyInfo).PropertyType == PropertyType.FormulaParameter)
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/formula.png", UriKind.Relative));
                else if ((nodeContext.Context as MyPropertyInfo).PropertyType == PropertyType.State)
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/state.png", UriKind.Relative));
                else if ((nodeContext.Context as MyPropertyInfo).PropertyType == PropertyType.Relationship)
                {
                    if ((nodeContext.Context as MyPropertyInfo).PropertyRelationship.TypeEnum == Enum_RelationshipType.OneToMany)
                        return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/type.png", UriKind.Relative));
                    else
                        return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/relationship.png", UriKind.Relative));
                }
                else
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/property.png", UriKind.Relative));
            }
            else
            {
                if (propertyType == NodeType.DotNetMethod)
                {
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/method.png", UriKind.Relative));
                }
                else if (propertyType == NodeType.HelperProperty)
                {
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/validate.png", UriKind.Relative));
                }
                else
                    return new BitmapImage(new Uri(@"/MyProject_WPF;component/Images/property.png", UriKind.Relative));
            }
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FormulaDefined != null)
                {
                    var arg = new FormulaDefinedArg();
                    // (ExpressionEditor.Expression as LambdaExpression).ReturnType;
                    //if (!arg.ExpressionResultType.IsPrimitive && arg.ExpressionResultType != typeof(string))
                    //{
                    //    MessageBox.Show("فرمول باید یک مقدار را برگداند");
                    //    return;
                    //}
                    FormulaItems.Clear();
                    CalculateFormula();
                    arg.Expression = GetFormulaText();
                    arg.ExpressionResultType = calculatedType;
                    //اینجا بهتره از chain 
                    //ها هم استفاده بشه تا اون خصوصیاتی که محاسبه در حالت تعریف محاسبه نمیشوند هم اضافه شوند
                    arg.FormulaItems = FormulaItems;
                    FormulaDefined(this, arg);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public List<FormulaItemDTO> FormulaItems { get; private set; }
        private void ExpressionEvaluator_PropertyCalled(object sender, PropertyCalledArg e)
        {
            if (FormulaItems == null)
                FormulaItems = new List<FormulaItemDTO>();

            if (FormulaItemExists(e.CalledProperty))
            {
                return;
            }
            FormulaItemDTO newItem = FormulaHelper.ToFormulaItem(e.CalledProperty);

            FormulaItems.Add(newItem);
        }
        private bool FormulaItemExists(MyPropertyInfo propertyInfo)
        {
            foreach (var item in FormulaItems)
            {
                if (item.RelationshipIDTail == propertyInfo.RelationshipTail && item.ItemType == FormulaHelper.GetFormulaItemType(propertyInfo.PropertyType) && item.ItemID == propertyInfo.ID)
                    return true;
            }
            return false;
        }

        I_EditEntityArea EditEntityArea { set; get; }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            if (EditEntityArea.GetDataList().Any())
            {
                TestData(EditEntityArea.GetDataList().First());
            }

            //if (lastSelectedData == null)
            //{
            //    btnSelectData_Click(null, null);
            //}
            //else
            //    TestData(lastSelectedData);

        }
        DP_DataRepository lastSelectedData;


        private void TestData(DP_DataRepository data)
        {
            try
            {
                FormulaFunctionHandler handler = new MyFormulaFunctionStateFunctionLibrary.FormulaFunctionHandler();

                //var child = new ChildRelationshipInfo();
                //child.Relationship = new BizRelationship().GetRelationship(33);

                //SearchRequestManager searchProcessor = new SearchRequestManager();
                //DP_SearchRepository searchItem = new DP_SearchRepository(75);
                //searchItem.Phrases.Add(new SearchProperty() { ColumnID = 68, Value = "بانک تجارت" });

                //DP_DataRepository childdata = null;
                //var requester = GetRequester();
                //سکوریتی داده اعمال میشود
                //DR_SearchFullDataRequest request = new DR_SearchFullDataRequest(requester, searchItem);
                //var searchResult = searchProcessor.Process(request);
                //if (searchResult.Result == Enum_DR_ResultType.SeccessfullyDone)
                //    childdata = searchResult.ResultDataItems.First();
                //else if (searchResult.Result == Enum_DR_ResultType.ExceptionThrown)
                //    throw (new Exception(searchResult.Message));

                //var childdata = searchProcessor.GetDataItemsByListOFSearchProperties(GetRequester(), searchItem).First();

                //child.ParentData = data;

                //childdata.ParantChildRelationshipInfo = child;
                //childdata.SetProperties(newProp);
                //child.RelatedData.Add(childdata);
                //data.ChildRelationshipInfos.Add(child);





                var result = handler.CalculateFormulaTest(GetFormulaText(), data, MyProjectManager.GetMyProjectManager.GetRequester());
                if (result.Exception == null)
                    MessageBox.Show(result.Result == null ? "null" : result.Result.ToString());
                else
                    MessageBox.Show(result.Exception.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //برنگشتن آبجکت
            //                آیتمهای فرمول
        }


        RadMenuItem codeMenuItem;
        RadMenuItem dbFunctionMenuItem;
        private RadContextMenu GenerateContextMenu()
        {
            var contextMenu = new RadContextMenu();

            codeMenuItem = new RadMenuItem();
            codeMenuItem.Header = "افزودن کد تابع";
            codeMenuItem.Click += mnuAddCodeFunction_Click;
            contextMenu.Items.Add(codeMenuItem);

            dbFunctionMenuItem = new RadMenuItem();
            dbFunctionMenuItem.Header = "افزودن تابع پایگاه داده";
            dbFunctionMenuItem.Click += mnuAddDBFunction_Click;
            contextMenu.Items.Add(dbFunctionMenuItem);

            return contextMenu;
        }

        int menuEntityID = 0;
        RadTreeViewItem mnuTreeItem = null;

        private void ContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            menuEntityID = 0;
            mnuTreeItem = null;

            var contextMenu = RadContextMenu.GetContextMenu(treeProperties);
            if (contextMenu == null)
                return;
            contextMenu.Visibility = Visibility.Collapsed;
            mnuTreeItem = contextMenu.GetClickedElement<RadTreeViewItem>();
            if (contextMenu != null && mnuTreeItem != null)
            {
                if (mnuTreeItem.DataContext is NodeContext)
                {
                    var nodeContext = (mnuTreeItem.DataContext as NodeContext);
                    if (GetEntityID(nodeContext) != 0)
                        menuEntityID = GetEntityID(nodeContext);
                    //else if (nodeContext.Context is MyPropertyInfo
                    //    && (nodeContext.Context as MyPropertyInfo).PropertyRelationship != null)
                    //{
                    //    menuEntityID = (nodeContext.Context as MyPropertyInfo).PropertyRelationship.EntityID2;
                    //}
                    //else if (nodeContext.Context is MethodInfo
                    //    && (nodeContext.Context as MethodInfo).ReturnType == typeof(MyCustomSingleData))
                    //{
                    //    var parent = nodeContext.ParentNode;
                    //    if (parent.Context is MyPropertyInfo)
                    //    {
                    //        if ((parent.Context as MyPropertyInfo).PropertyType == ProxyLibrary.PropertyType.Relationship)
                    //        {
                    //            menuEntityID = (parent.Context as MyPropertyInfo).PropertyRelationship.EntityID2;
                    //        }

                    //    }
                    //}
                    if (menuEntityID != 0)
                    {
                        contextMenu.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void mnuAddCodeFunction_Click(object sender, RoutedEventArgs e)
        {
            if (menuEntityID != 0)
            {
                frmCodeFunction_Entity frm = new MyProject_WPF.frmCodeFunction_Entity(0, menuEntityID);
                frm.CodeFunctionEntityUpdated += Frm_CodeFunctionEntityUpdated;
                MyProjectManager.GetMyProjectManager.ShowDialog(frm, "");
            }
        }

        private void Frm_CodeFunctionEntityUpdated(object sender, CodeFunctionEntitySelectedArg e)
        {
            MyProjectManager.GetMyProjectManager.CloseDialog(sender);
            if (mnuTreeItem != null)
            {
                RefreshEntityNode(mnuTreeItem, menuEntityID);
            }
        }

        private void RefreshEntityNode(RadTreeViewItem mnuTreeItem, int entityID)
        {
            if (EntityAndProperties.Any(x => x.EntityID == entityID))
                EntityAndProperties.Remove(EntityAndProperties.First(x => x.EntityID == entityID));
            SetNodeChilds(mnuTreeItem);
            mnuTreeItem.IsExpanded = true;
        }

        private void mnuAddDBFunction_Click(object sender, RoutedEventArgs e)
        {
            frmDatabaseFunction_Entity frm = new MyProject_WPF.frmDatabaseFunction_Entity(0, menuEntityID);
            frm.DatabaseFunctionEntityUpdated += Frm_DatabaseFunctionEntityUpdated;
            MyProjectManager.GetMyProjectManager.ShowDialog(frm, "");
        }

        private void Frm_DatabaseFunctionEntityUpdated(object sender, DatabaseFunctionEntitySelectedArg e)
        {
            if (mnuTreeItem != null)
            {
                RefreshEntityNode(mnuTreeItem, menuEntityID);
            }
        }


        //frmDataSelect frmDataSelect;
        //private void btnSelectData_Click(object sender, RoutedEventArgs e)
        //{
        //    if (frmDataSelect == null)
        //    {
        //        frmDataSelect = new MyProject_WPF.frmDataSelect(EntityID);
        //        frmDataSelect.DataSelected += View_DataSelected;
        //    }
        //    MyProjectManager.GetMyProjectManager.ShowDialog(frmDataSelect, "انتخاب داده", Enum_WindowSize.Big);
        //}
        //private void View_DataSelected(object sender, DataSelectedArg e)
        //{
        //    SearchRequestManager searchProcessor = new SearchRequestManager();
        //    DP_SearchRepository searchDataItem = new DP_SearchRepository(Entity.ID);
        //    foreach (var property in e.Columns)
        //        searchDataItem.Phrases.Add(new SearchProperty() { ColumnID = property.ColumnID, Value = property.Value });
        //    //سکوریتی داده اعمال میشود
        //    DP_DataRepository foundDataItem = null;
        //    var requester = GetRequester();
        //    DR_SearchFullDataRequest request = new DR_SearchFullDataRequest(requester, searchDataItem);

        //    var searchResult = searchProcessor.Process(request);
        //    if (searchResult.Result == Enum_DR_ResultType.SeccessfullyDone)
        //        foundDataItem = searchResult.ResultDataItems.FirstOrDefault();
        //    else if (searchResult.Result == Enum_DR_ResultType.ExceptionThrown)
        //        throw (new Exception(searchResult.Message));


        //    //var foundDataItem = searchProcessor.GetDataItemsByListOFSearchProperties(GetRequester(), searchDataItem).FirstOrDefault();
        //    if (foundDataItem != null)
        //    {
        //        lastSelectedData = foundDataItem;
        //        TestData(lastSelectedData);
        //    }
        //    else
        //        MessageBox.Show("چنین داده ای یافت نشد");
        //}

    }



}
