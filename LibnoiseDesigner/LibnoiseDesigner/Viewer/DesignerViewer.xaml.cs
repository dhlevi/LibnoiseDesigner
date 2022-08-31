using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using NetworkModel;
using NetworkUI;
using NetworkView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ZoomAndPan;

namespace WorldForge.LibnoiseDesigner.Viewer
{
    /// <summary>
    /// Interaction logic for DesignerViewer.xaml
    /// </summary>
    public partial class DesignerViewer : UserControl
    {
        private bool initialized = false;

        public DesignerViewer()
        {
            InitializeComponent();
            
            // adding this event handler will allow us to automatically resize the 
            // workarea when a node is dragged and dropped. Unfortunately, this 
            // seems to cause some weirdness with current selected node...
            //networkControl.NodeDragCompleted += networkControl_NodeDragCompleted;
        }

        private CurvedArrow _selectedArrow;
        public CurvedArrow SelectedArrow 
        {
            get
            {
                return _selectedArrow;
            }
            set
            {
                _selectedArrow = value;
            }
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get
            {
                return (MainWindowViewModel)DataContext;
            }
        }

        /// <summary>
        /// Event raised when the Window has loaded.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                networkControl.SelectionChanged += networkControl_SelectionChanged;

                LibnoiseFileUtils.LoadDefaultXml(this);

                ResizeWorkarea();

                initialized = true;
            }
        }

        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void networkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            ConnectorViewModel draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            System.Windows.Point curDragPoint = Mouse.GetPosition(networkControl);

            // Delegate the real work to the view model.
            ConnectionViewModel connection = ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            // Must return the view-model object that represents the connection via the event args.
            // This is so that NetworkView can keep track of the object while it is being dragged.
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised, to query for feedback, while the user is dragging a connection.
        /// </summary>
        private void networkControl_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e)
        {
            ConnectorViewModel draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            ConnectorViewModel draggedOverConnector = (ConnectorViewModel)e.DraggedOverConnector;

            object feedbackIndicator = null;
            bool connectionOk = true;

            ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);

            // Return the feedback object to NetworkView.
            // The object combined with the data-template for it will be used to create a 'feedback icon' to
            // display (in an adorner) to the user.
            e.FeedbackIndicator = feedbackIndicator;

            // Let NetworkView know if the connection is ok or not ok.
            e.ConnectionOk = connectionOk;
        }

        // Event raised while the user is dragging a connection.
        private void networkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            System.Windows.Point curDragPoint = Mouse.GetPosition(networkControl);
            ConnectionViewModel connection = (ConnectionViewModel)e.Connection;

            ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        // Event raised when the user has finished dragging out a connection.
        private void networkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            ConnectorViewModel connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            ConnectorViewModel connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            ConnectionViewModel newConnection = (ConnectionViewModel)e.Connection;

            ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        // Event raised to delete the selected node.
        private void DeleteSelectedNodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.DeleteSelectedNodes();
        }

        // Event raised to create a new node.
        private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNode(new Cache());
        }

        // Event raised to delete a node.
        private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.DeleteNode((NodeViewModel)e.Parameter, false);
        }

        // Event raised to delete a connection.
        private void DeleteConnection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.DeleteConnection((ConnectionViewModel)e.Parameter);
        }

        // Creates a new node in the network at the current mouse location.
        private void CreateNode(ModuleBase module)
        {
            //var newNodePosition = Mouse.GetPosition(networkControl);
            NodeViewModel nvw = this.ViewModel.CreateNode(module, contextMenuPosition, true);
        }

        // trigger a resize of the workspace when a node has been dragged
        void networkControl_NodeDragCompleted(object sender, NodeDragCompletedEventArgs e)
        {
            ResizeWorkarea();
        }

        // Event raised when the size of a node has changed.
        private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            FrameworkElement element = (FrameworkElement)sender;
            NodeViewModel node = (NodeViewModel)element.DataContext;

            node.Size = new System.Windows.Size(element.ActualWidth, element.ActualHeight);
        }

        // Specifies the current state of the mouse handling logic.
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        // The point that was clicked relative to the ZoomAndPanControl.
        private System.Windows.Point origZoomAndPanControlMouseDownPoint;

        // The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        private System.Windows.Point origContentMouseDownPoint;

        // Records which mouse button clicked during mouse dragging.
        private MouseButton mouseButtonDown;

        // Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        private Rect prevZoomRect;

        // Save the previous content scale, pressing the backspace key jumps back to this scale.
        private double prevZoomScale;

        // Set to 'true' when the previous zoom rect is saved.
        private bool prevZoomRectSet = false;

        // Event raised on mouse down in the NetworkView.
        private void networkControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            networkControl.Focus();
            Keyboard.Focus(networkControl);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(networkControl);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 && (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left && (Keyboard.Modifiers & ModifierKeys.Control) == 0)
            {
                // Initiate panning, when control is not held down.
                // When control is held down left dragging is used for drag selection.
                // After panning has been initiated the user must drag further than the threshold value to actually start drag panning.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                networkControl.CaptureMouse();
                e.Handled = true;
            }
        }

        // Event raised on mouse up in the NetworkView.
        private void networkControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            NodePropertyGrid.SelectedObject = networkControl.SelectedNode != null ? ((NodeViewModel)networkControl.SelectedNode).Module.LibnoiseModule : null;

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Panning)
                {
                    // Panning was initiated but dragging was abandoned before the mouse
                    // cursor was dragged further than the threshold distance.
                    // This means that this basically just a regular left mouse click.
                    // Because it was a mouse click in empty space we need to clear the current selection.
                }
                else if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(origContentMouseDownPoint);
                    }
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragZoomRect();
                }

                // Reenable clearing of selection when empty space is clicked.
                // This is disabled when drag panning is in progress.
                networkControl.IsClearSelectionOnEmptySpaceClickEnabled = true;

                // Reset the override cursor.
                // This is set to a special cursor while drag panning is in progress.
                Mouse.OverrideCursor = null;

                networkControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        // Event raised on mouse move in the NetworkView.
        private void networkControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                System.Windows.Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;

                if (Math.Abs(dragOffset.X) > dragThreshold || Math.Abs(dragOffset.Y) > dragThreshold)
                {
                    // The user has dragged the cursor further than the threshold distance, initiate
                    // drag panning.
                    mouseHandlingMode = MouseHandlingMode.DragPanning;
                    networkControl.IsClearSelectionOnEmptySpaceClickEnabled = false;
                    Mouse.OverrideCursor = Cursors.ScrollAll;
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragPanning)
            {
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                System.Windows.Point curContentMousePoint = e.GetPosition(networkControl);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomAndPanControl.ContentOffsetX -= dragOffset.X;
                zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                System.Windows.Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (mouseButtonDown == MouseButton.Left && (Math.Abs(dragOffset.X) > dragThreshold || Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    mouseHandlingMode = MouseHandlingMode.DragZooming;
                    System.Windows.Point curContentMousePoint = e.GetPosition(networkControl);
                    InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                // When in drag zooming mode continously update the position of the rectangle
                // that the user is dragging out.
                System.Windows.Point curContentMousePoint = e.GetPosition(networkControl);
                SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
        }

        // Event raised by rotating the mouse wheel.
        private void networkControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                System.Windows.Point curContentMousePoint = e.GetPosition(networkControl);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                System.Windows.Point curContentMousePoint = e.GetPosition(networkControl);
                ZoomOut(curContentMousePoint);
            }
        }

        // Event raised when the user has double clicked in the zoom and pan control.
        private void networkControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                System.Windows.Point doubleClickPoint = e.GetPosition(networkControl);
                zoomAndPanControl.AnimatedSnapTo(doubleClickPoint);
            }
        }

        // The 'ZoomIn' command (bound to the plus key) was executed.
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //var o = networkControl.SelectedNode;
            ZoomIn(new System.Windows.Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        // The 'ZoomOut' command (bound to the minus key) was executed.
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut(new System.Windows.Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        // The 'JumpBackToPrevZoom' command was executed.
        private void JumpBackToPrevZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JumpBackToPrevZoom();
        }

        // Determines whether the 'JumpBackToPrevZoom' command can be executed.
        private void JumpBackToPrevZoom_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = prevZoomRectSet;
        }

        // The 'Fill' command was executed.
        private void FitContent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IList nodes = null;

            if (networkControl.SelectedNodes.Count > 0)
            {
                nodes = networkControl.SelectedNodes;
            }
            else
            {
                nodes = this.ViewModel.Network.Nodes;
                if (nodes.Count == 0)
                {
                    return;
                }
            }

            SavePrevZoomRect();

            Rect actualContentRect = DetermineAreaOfNodes(nodes);

            // Inflate the content rect by a fraction of the actual size of the total content area.
            // This puts a nice border around the content we are fitting to the viewport.
            actualContentRect.Inflate(networkControl.ActualWidth / 40, networkControl.ActualHeight / 40);

            zoomAndPanControl.AnimatedZoomTo(actualContentRect);
        }

        public void ResizeWorkarea()
        {
            //NodeViewModel firstNode = this.ViewModel.Network.Nodes.Where(n => !n.Size.IsEmpty).FirstOrDefault();

            double widthModifier = 300;
            double heightModifier = 100;

            double minX = this.ViewModel.Network.Nodes.Min(n => n.X);
            double maxX = this.ViewModel.Network.Nodes.Max(n => n.X) + widthModifier;
            double minY = this.ViewModel.Network.Nodes.Min(n => n.Y);
            double maxY = this.ViewModel.Network.Nodes.Max(n => n.Y) + heightModifier;

            double height = maxY - minY;
            double width = maxX - minX;

            if (!Double.IsNegativeInfinity(width) && !Double.IsNegativeInfinity(height))
            {
                NodePanel.Width = width;
                NodePanel.Height = height;
            }
        }

        // Determine the area covered by the specified list of nodes.
        private Rect DetermineAreaOfNodes(IList nodes)
        {
            NodeViewModel firstNode = (NodeViewModel)nodes[0];
            Rect actualContentRect = new Rect(firstNode.X, firstNode.Y, firstNode.Size.Width, firstNode.Size.Height);

            for (int i = 1; i < nodes.Count; ++i)
            {
                NodeViewModel node = (NodeViewModel)nodes[i];
                Rect nodeRect = new Rect(node.X, node.Y, node.Size.Width, node.Size.Height);
                actualContentRect = Rect.Union(actualContentRect, nodeRect);
            }

            return actualContentRect;
        }

        // The 'Fill' command was executed.
        private void Fill_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            zoomAndPanControl.AnimatedScaleToFit();
        }

        // The 'OneHundredPercent' command was executed.
        private void OneHundredPercent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            zoomAndPanControl.AnimatedZoomTo(1.0);
        }

        // Jump back to the previous zoom level.
        private void JumpBackToPrevZoom()
        {
            zoomAndPanControl.AnimatedZoomTo(prevZoomScale, prevZoomRect);

            ClearPrevZoomRect();
        }

        // Zoom the viewport out, centering on the specified System.Windows.Point (in content coordinates).
        private void ZoomOut(System.Windows.Point contentZoomCenter)
        {
            zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale - 0.1, contentZoomCenter);
        }

        // Zoom the viewport in, centering on the specified System.Windows.Point (in content coordinates).
        private void ZoomIn(System.Windows.Point contentZoomCenter)
        {
            zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale + 0.1, contentZoomCenter);
        }

        // Initialize the rectangle that the use is dragging out.
        private void InitDragZoomRect(System.Windows.Point pt1, System.Windows.Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        // Update the position and size of the rectangle that user is dragging out.
        private void SetDragZoomRect(System.Windows.Point pt1, System.Windows.Point pt2)
        {
            double x, y, width, height;

            // Deterine x,y,width and height of the rect inverting the points if necessary.

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }

        // When the user has finished dragging out the rectangle the zoom operation is applied.
        private void ApplyDragZoomRect()
        {
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            SavePrevZoomRect();

            // Retreive the rectangle that the user draggged out and zoom in on it.
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            zoomAndPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            FadeOutDragZoomRect();
        }

        // Fade out the drag zoom rectangle.
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(dragZoomBorder, Border.OpacityProperty, 0.0, 0.1,
            delegate(object sender, EventArgs e)
            {
                dragZoomCanvas.Visibility = Visibility.Collapsed;
            });
        }

        // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        private void SavePrevZoomRect()
        {
            prevZoomRect = new Rect(zoomAndPanControl.ContentOffsetX, zoomAndPanControl.ContentOffsetY, zoomAndPanControl.ContentViewportWidth, zoomAndPanControl.ContentViewportHeight);
            prevZoomScale = zoomAndPanControl.ContentScale;
            prevZoomRectSet = true;
        }

        // Clear the memory of the previous zoom level.
        private void ClearPrevZoomRect()
        {
            prevZoomRectSet = false;
        }

        private void arrow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurvedArrow clickedArrow = (CurvedArrow)sender;

            clickedArrow.Animate = !clickedArrow.Animate;

            if (clickedArrow == SelectedArrow) SelectedArrow = null;
            else SelectedArrow = clickedArrow;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedMenu = (MenuItem)sender;

            switch(clickedMenu.Name)
            {
                case "createBillow":
                    CreateNode(new Billow());
                    break;
                case"createChecker":
                    CreateNode(new Checker());
                    break;
                case "createConst":
                    CreateNode(new Const());
                    break;
                case "createCylinder":
                    CreateNode(new Cylinders());
                    break;
                case "createPerlin":
                    CreateNode(new Perlin());
                    break;
                case "createRidgedMF":
                    CreateNode(new RidgedMultifractal());
                    break;
                case "createSpheres":
                    CreateNode(new Spheres());
                    break;
                case "createVoronoi":
                    CreateNode(new Voronoi());
                    break;
                case "createAbs":
                    CreateNode(new Abs());
                    break;
                case "createAdd":
                    CreateNode(new Add());
                    break;
                case "createBlend":
                    CreateNode(new Blend());
                    break;
                case "createCache":
                    CreateNode(new Cache());
                    break;
                case "createClamp":
                    CreateNode(new Clamp());
                    break;
                case "createCurve":
                    CreateNode(new Curve());
                    break;
                case "createDisplace":
                    CreateNode(new Displace());
                    break;
                case "createExponent":
                    CreateNode(new Exponent());
                    break;
                case "createInvert":
                    CreateNode(new Invert());
                    break;
                case "createMax":
                    CreateNode(new Max());
                    break;
                case "createMin":
                    CreateNode(new Min());
                    break;
                case "createMultiply":
                    CreateNode(new Multiply());
                    break;
                case "createPower":
                    CreateNode(new Power());
                    break;
                case "createRotate":
                    CreateNode(new Rotate());
                    break;
                case "createScale":
                    CreateNode(new Scale());
                    break;
                case "createScaleBias":
                    CreateNode(new ScaleBias());
                    break;
                case "createSelect":
                    CreateNode(new Select());
                    break;
                case "createSubtract":
                    CreateNode(new Subtract());
                    break;
                case "createTerrace":
                    CreateNode(new Terrace());
                    break;
                case "createTranslate":
                    CreateNode(new Translate());
                    break;
                case "createTurb":
                    CreateNode(new Turbulence());
                    break;
                default:
                    break;
            }
        }

        void networkControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Worker != null) Worker.CancelAsync();
            CreatePreviewImage();

            if ((NodeViewModel)networkControl.SelectedNode != null)
            {
                ModuleBase module = ((NodeViewModel)networkControl.SelectedNode).Module.LibnoiseModule;

                NodeHeaderLabel.Content = module.GetType().Name;
                NodeDescription.Text = module.GetDescription();
            }
            else
            {
                NodeHeaderLabel.Content = "LibNoise Designer";
                NodeDescription.Text = "Select a node in the designer to view a description of what it does, and edit the node properties.";
            }
        }

        private void NodePropertyGrid_PropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {
            if (Worker != null) Worker.CancelAsync();
            CreatePreviewImage();
        }

        private Bitmap PreviewImageBitmap { get; set; }
        private BackgroundWorker Worker { get; set; }

        private int previewWidth = 200;
        private int previewHeight = 200;

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (PreviewImageBitmap != null && !e.Cancelled)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    PreviewImageBitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    PreviewImage.Source = bitmapImage;
                }

                PreviewImage.Visibility = System.Windows.Visibility.Visible;
                LoadingBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                double[,] data = NoiseFactory.GeneratePlanar((ModuleBase)e.Argument, previewWidth, previewHeight, -1, 1, -1, 1, true, true, 0);

                Bitmap bm = new Bitmap(previewWidth, previewHeight);

                for (int x = 0; x < previewWidth; x++)
                {
                    for (int y = 0; y < previewHeight; y++)
                    {
                        double val = data[x, y];

                        val = val > 1.0 ? 1.0 : val;
                        val = val < 0 ? 0 : val;

                        // typically, I'd rather do image work with lockbits, but for something so small the
                        // slow setpixels method should work fine.
                        int colorValue = Convert.ToInt32(Math.Round((255.0f * val), 0, MidpointRounding.ToEven));
                        bm.SetPixel(x, y, System.Drawing.Color.FromArgb(colorValue, colorValue, colorValue));
                    }
                }

                PreviewImageBitmap = bm;
            }
            catch (Exception ex)
            {
                // can't prepare the preview image. just toss back an empty one for now
                // it's most likely that the diagram is invalid
                PreviewImageBitmap = new Bitmap(previewWidth, previewHeight);
            }
        }

        // Generate the preview bmp of the selected nodes noise.getvalue result
        private void CreatePreviewImage()
        {
            if (networkControl.SelectedNode != null)
            {
                ModuleBase module = ((NodeViewModel)networkControl.SelectedNode).Module.LibnoiseModule;
                NodePropertyGrid.SelectedObject = module;

                if (Worker == null)
                {
                    Worker = new BackgroundWorker();
                    Worker.WorkerSupportsCancellation = true;
                    Worker.DoWork += Worker_DoWork;
                    Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                }

                if (!Worker.IsBusy)
                {
                    previewWidth = (int)PreviewImage.Width;
                    previewHeight = (int)PreviewImage.Height;

                    PreviewImage.Visibility = System.Windows.Visibility.Collapsed;
                    LoadingBar.Visibility = System.Windows.Visibility.Visible;

                    Worker.RunWorkerAsync(module);
                }
            }
            else
            {
                NodePropertyGrid.SelectedObject = null;
            }
        }

        private void highlightPath_Click(object sender, RoutedEventArgs e)
        {
            //get the data context of the adorner to get the NodeView
            //this is the single ugliest line of code I have ever written, but hey, it worked for a quick test. Thought I'd leave it here for the memories
            //NodeViewModel nvm = (NodeViewModel)((AdornedControl.AdornedControl)((Grid)((Grid)((StackPanel)((Button)sender).Parent).Parent).Parent).Parent).DataContext;

            NodeViewModel nvm = FindParentNodeViewModel((Button)sender);

            if (nvm != null)
            {
                this.networkControl.SelectedNodes.Clear();
                TraceConnectionPath(nvm);
            }
        }

        private NodeViewModel FindParentNodeViewModel(FrameworkElement obj)
        {
            try
            {
                if (obj.GetType() == typeof(AdornedControl.AdornedControl))
                {
                    AdornedControl.AdornedControl adornerParent = (AdornedControl.AdornedControl)obj;
                    if (adornerParent.DataContext.GetType() == typeof(NodeViewModel)) return ((AdornedControl.AdornedControl)obj).DataContext as NodeViewModel;
                    else return null; // we found the adorner parent object, but the context wasn't right... why loop forever when we can just return nothing now
                }
                else return FindParentNodeViewModel((FrameworkElement)obj.Parent);
            }
            catch (Exception e)
            {
                //something (understandably) went horribly wrong...
                //Likely a child wasnt a framework element or something
                return null;
            }
        }

        // simple recursive method to trace the path from the selected node
        // to the final node. Each node on the trace will be selected.
        // eventually, it would be ideal to highlight the connectors as well...
        private void TraceConnectionPath(NodeViewModel nvm)
        {
            foreach (ConnectorViewModel cvm in nvm.OutputConnectors)
            {
                ConnectorViewModel destConnector = cvm.AttachedConnections.First().DestConnector;
                NodeViewModel destNode = destConnector.ParentNode;

                cvm.AttachedConnections.First().IsSelected = true;
                destConnector.IsSelected = true;
                
                TraceConnectionPath(destNode);
            }
        }

        //    private DispatcherTimer t;
        //    if(t != null) t.Stop();

        //    if (exp.Visibility != Visibility.Visible)
        //    {
        //        // wait a couple seconds, then expand.
        //        t = new DispatcherTimer();
        //        t.Interval = new TimeSpan(0, 0, 2);
        //        t.Tick += (a, args) =>
        //        {
        //            exp.Visibility = Visibility.Visible;
        //        };
        //        t.Start();
        //    }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // on double click, expand/hide the node and show the extra content
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                // we need to grab the stack panel holder to set visibility on
                // this isn't the prettiest way to go around it, but it will work
                // for now
                Grid nodeGrid = (Grid)((Grid)sender).Children[1];
                StackPanel exp = (StackPanel)nodeGrid.Children[1]; // get the containing stack panel

                exp.Visibility = exp.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Poor mans validation on text input.
        private void moduleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (networkControl.Nodes.Cast<NodeViewModel>().Count(n => n.Module.ID.Equals(((TextBox)sender).Text)) != 0)
            {
                ((TextBox)sender).Background = System.Windows.Media.Brushes.LightPink;
                ((TextBox)sender).BorderBrush = System.Windows.Media.Brushes.Red;
                ((TextBox)sender).SelectionBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                ((TextBox)sender).Background = System.Windows.Media.Brushes.White;
                ((TextBox)sender).SelectionBrush = System.Windows.Media.Brushes.CornflowerBlue;
                ((TextBox)sender).BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;
            }
        }

        private void Resize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResizeWorkarea();
        }

        private void Validate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.Validate();
        }

        // get the mouse position where the mouse was right clicked to open the context menu
        // if the user creates a new node, this right click location is where we want to place it
        private System.Windows.Point contextMenuPosition;
        private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            contextMenuPosition = Mouse.GetPosition(networkControl);
        }
    }
}
