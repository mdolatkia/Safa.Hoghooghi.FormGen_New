using MyUILibraryInterfaces.DataViewArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams.Primitives;
using ModelEntites;
using System.Windows.Threading;
using MyUILibraryInterfaces.DataLinkArea;
using MyUILibrary.EntityArea;
using Telerik.Windows.Diagrams.Core;
using MyCommonWPFControls;

namespace MyUIGenerator.View
{
    /// <summary>
    /// Interaction logic for frmDataView.xaml
    /// </summary>
    public partial class frmDiagram : UserControl, I_View_Diagram
    {
        DispatcherTimer timer = new DispatcherTimer();
        public frmDiagram()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += Timer_Tick;
            cmdDiagramType.SelectionChanged += CmdDiagramType_SelectionChanged;
            //  cmbDataLinks.SelectionChanged += CmbDataLinks_SelectionChanged;
        }

        private void CmdDiagramType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDiagram();
        }

        public void AddRelation(object view1, object view2)
        {
            var shape1 = (view1 as UserControl).Parent as RadDiagramShape;
            var shape2 = (view2 as UserControl).Parent as RadDiagramShape;
            diagram.AddConnection(shape1, shape2);
            //      diagram.AddConnection(shape1, shape2);
        }

        List<IShape> Roots = new List<IShape>();
        public void AddView(I_DataViewItem view)
        {
            diagram.Items.Add(view);
            var shape = (diagram.Items[diagram.Items.Count - 1] as UserControl).Parent as RadDiagramShape;
            shape.IsConnectorsManipulationEnabled = false;
            shape.IsRotationEnabled = false;
            shape.IsResizingEnabled = false;
            if (view.IsRoot)
                Roots.Add(shape);
        }

        public void ClearItems()
        {
            diagram.Items.Clear();
            Roots.Clear();
        }

        public void SetDiagramTypes(List<DiagramTypes> diagramTypes)
        {
            cmdDiagramType.ItemsSource = diagramTypes;
            cmdDiagramType.DisplayMemberPath = "Title";
            if (diagramTypes.Any())
                cmdDiagramType.SelectedItem = diagramTypes.First();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();
            if (cmdDiagramType.SelectedItem != null)
            {
                var type = ((DiagramTypes)cmdDiagramType.SelectedItem).DiagramType;
                if (type == EnumDiagramTypes.Sugiyama)
                {
                    SugiyamaSettings settings = new SugiyamaSettings();
                    diagram.Layout(LayoutType.Sugiyama, settings);

                }
                else
                {
                    TreeLayoutSettings settings = new TreeLayoutSettings();
                    if (type == EnumDiagramTypes.TreeHorizontal)
                        settings.TreeLayoutType = TreeLayoutType.TreeRight;
                    else if (type == EnumDiagramTypes.TreeVertical)
                        settings.TreeLayoutType = TreeLayoutType.TreeDown;
                    else if (type == EnumDiagramTypes.TreeRadial)
                        settings.TreeLayoutType = TreeLayoutType.RadialTree;
                    else if (type == EnumDiagramTypes.TreeTipOver)
                        settings.TreeLayoutType = TreeLayoutType.TipOverTree;
                    else if (type == EnumDiagramTypes.MindmapHorizontal)
                        settings.TreeLayoutType = TreeLayoutType.MindmapHorizontal;
                    else if (type == EnumDiagramTypes.MindmapVertical)
                        settings.TreeLayoutType = TreeLayoutType.MindmapVertical;
                    else if (type == EnumDiagramTypes.TreeUndefined)
                        settings.TreeLayoutType = TreeLayoutType.Undefined;

                    if (Roots.Any())
                        settings.Roots.AddRange(Roots);
                    diagram.Layout(LayoutType.Tree, settings);
                }



                diagram.AutoFit();

                //if (diagram.Items.Any())
                //    settings.Roots.Add((diagram.Items[0] as UserControl).Parent as RadDiagramShape);
                //  
                //    diagram.Layout(LayoutType.Tree, settings);
                //    diagram.AutoFit();
                //    shapeS.Position = new Point(shapeS.Position.X, shapeF.Position.Y);
            }
           
        }

        public void RefreshDiagram()
        {
            timer.Start();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshDiagram();
        }

        //public void SetDataLinks(List<DataLinkDTO> dataLinks, int dataLinkID)
        //{
        //    cmbDataLinks.SelectedValuePath = "ID";
        //    cmbDataLinks.DisplayMemberPath = "Name";
        //    cmbDataLinks.ItemsSource = dataLinks;
        //    cmbDataLinks.SelectedValue = dataLinkID;
        //}



    }
}
