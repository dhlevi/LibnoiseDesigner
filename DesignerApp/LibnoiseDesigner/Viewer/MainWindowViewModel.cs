using LibNoise;
using LibNoise.Operator;
using NetworkModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Utils;
using WorldForge.LibnoiseDesigner;
using WorldForge.LibnoiseDesigner.Viewer;

namespace NetworkView
{
    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class MainWindowViewModel : AbstractModelBase
    {
        #region Internal Data Members

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel network = null;

        ///
        /// The current scale at which the content is being viewed.
        /// 
        private double contentScale = 1;

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        private double contentWidth = 1000;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        private double contentHeight = 1000;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private double contentViewportWidth = 0;

        ///
        /// The height of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private double contentViewportHeight = 0;

        #endregion Internal Data Members

        public MainWindowViewModel()
        {
            this.Network = new NetworkViewModel();

            NodeViewModel finalNode = CreateNode(new Final(), new Point(100, 60), false);
        }

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel Network
        {
            get
            {
                return network;
            }
            set
            {
                network = value;

                OnPropertyChanged("Network");
            }
        }

        ///
        /// The current scale at which the content is being viewed.
        /// 
        public double ContentScale
        {
            get
            {
                return contentScale;
            }
            set
            {
                contentScale = value;

                OnPropertyChanged("ContentScale");
            }
        }

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetX
        {
            get
            {
                return contentOffsetX;
            }
            set
            {
                contentOffsetX = value;

                OnPropertyChanged("ContentOffsetX");
            }
        }

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetY
        {
            get
            {
                return contentOffsetY;
            }
            set
            {
                contentOffsetY = value;

                OnPropertyChanged("ContentOffsetY");
            }
        }

        ///
        /// The width of the content (in content coordinates).
        /// 
        public double ContentWidth
        {
            get
            {
                return contentWidth;
            }
            set
            {
                contentWidth = value;

                OnPropertyChanged("ContentWidth");
            }
        }

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        public double ContentHeight
        {
            get
            {
                return contentHeight;
            }
            set
            {
                contentHeight = value;

                OnPropertyChanged("ContentHeight");
            }
        }

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportWidth
        {
            get
            {
                return contentViewportWidth;
            }
            set
            {
                contentViewportWidth = value;

                OnPropertyChanged("ContentViewportWidth");
            }
        }

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportHeight
        {
            get
            {
                return contentViewportHeight;
            }
            set
            {
                contentViewportHeight = value;

                OnPropertyChanged("ContentViewportHeight");
            }
        }

        /// <summary>
        /// Called when the user has started to drag out a connector, thus creating a new connection.
        /// </summary>
        public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
        {
            //
            // Create a new connection to add to the view-model.
            //
            var connection = new ConnectionViewModel();

            if (draggedOutConnector.Type == ConnectorType.Output)
            {
                //
                // The user is dragging out a source connector (an output) and will connect it to a destination connector (an input).
                //
                connection.SourceConnector = draggedOutConnector;
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                //
                // The user is dragging out a destination connector (an input) and will connect it to a source connector (an output).
                //
                connection.DestConnector = draggedOutConnector;
                connection.SourceConnectorHotspot = curDragPoint;
            }

            //
            // Add the new connection to the view-model.
            //
            this.Network.Connections.Add(connection);

            return connection;
        }

        /// <summary>
        /// Called to query the application for feedback while the user is dragging the connection.
        /// </summary>
        public void QueryConnnectionFeedback(ConnectorViewModel draggedOutConnector, ConnectorViewModel draggedOverConnector, out object feedbackIndicator, out bool connectionOk)
        {
            if (draggedOutConnector == draggedOverConnector)
            {
                //
                // Can't connect to self!
                // Provide feedback to indicate that this connection is not valid!
                //
                feedbackIndicator = new ConnectionBadIndicator();
                connectionOk = false;
            }
            else
            {
                var sourceConnector = draggedOutConnector;
                var destConnector = draggedOverConnector;

                //
                // Only allow connections from output connector to input connector (ie each
                // connector must have a different type).
                // Also only allocation from one node to another, never one node back to the same node.
                //
                connectionOk = sourceConnector.ParentNode != destConnector.ParentNode &&
                                 sourceConnector.Type != destConnector.Type &&
                                 destConnector.AttachedConnections.Count == 0;

                if (connectionOk)
                {
                    // 
                    // Yay, this is a valid connection!
                    // Provide feedback to indicate that this connection is ok!
                    //
                    feedbackIndicator = new ConnectionOkIndicator();
                }
                else
                {
                    //
                    // Connectors with the same connector type (eg input & input, or output & output)
                    // can't be connected.
                    // Only connectors with separate connector type (eg input & output).
                    // Provide feedback to indicate that this connection is not valid!
                    //
                    feedbackIndicator = new ConnectionBadIndicator();
                }
            }
        }

        /// <summary>
        /// Called as the user continues to drag the connection.
        /// </summary>
        public void ConnectionDragging(Point curDragPoint, ConnectionViewModel connection)
        {
            if (connection.DestConnector == null)
            {
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                connection.SourceConnectorHotspot = curDragPoint;
            }
        }

        /// <summary>
        /// Called when the user has finished dragging out the new connection.
        /// </summary>
        public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
        {
            if (connectorDraggedOver == null)
            {
                //
                // The connection was unsuccessful.
                // Maybe the user dragged it out and dropped it in empty space.
                //
                this.Network.Connections.Remove(newConnection);
                return;
            }

            //
            // Only allow connections from output connector to input connector (ie each
            // connector must have a different type).
            // Also only allocation from one node to another, never one node back to the same node.
            //
            bool connectionOk = connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode &&
                                connectorDraggedOut.Type != connectorDraggedOver.Type
                                && connectorDraggedOver.AttachedConnections.Count == 0;


            if (!connectionOk)
            {
                //
                // Connections between connectors that have the same type,
                // eg input -> input or output -> output, are not allowed,
                // Remove the connection.
                //
                this.Network.Connections.Remove(newConnection);
                return;
            }

            //
            // The user has dragged the connection on top of another valid connector.
            //

            //
            // Remove any existing connection between the same two connectors.
            //
            var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
            if (existingConnection != null)
            {
                this.Network.Connections.Remove(existingConnection);
            }

            //
            // Finalize the connection by attaching it to the connector
            // that the user dragged the mouse over.
            //
            if (newConnection.DestConnector == null)
            {
                newConnection.DestConnector = connectorDraggedOver;
            }
            else
            {
                newConnection.SourceConnector = connectorDraggedOver;
            }

            if (newConnection.DestConnector != null && newConnection.SourceConnector != null)
            {
                LibnoiseNode sourceNode = newConnection.SourceConnector.ParentNode.Module;
                LibnoiseNode destinationNode = newConnection.DestConnector.ParentNode.Module;
                ModuleBase sourceModule = newConnection.SourceConnector.ParentNode.Module.LibnoiseModule;
                ModuleBase destinationModule = newConnection.DestConnector.ParentNode.Module.LibnoiseModule;

                // get destination type, connector name, assign to correct module
                if(newConnection.DestConnector.Name.Equals("Input"))
                {
                    destinationModule.Modules[0] = sourceModule;
                    destinationNode.Inputs[0] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Left"))
                {
                    destinationModule.Modules[0] = sourceModule;
                    destinationNode.Inputs[0] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Right"))
                {
                    destinationModule.Modules[1] = sourceModule;
                    destinationNode.Inputs[1] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Operator"))
                {
                    destinationModule.Modules[2] = sourceModule;
                    destinationNode.Inputs[2] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Primary"))
                {
                    destinationModule.Modules[0] = sourceModule;
                    destinationNode.Inputs[0] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Secondary"))
                {
                    destinationModule.Modules[1] = sourceModule;
                    destinationNode.Inputs[1] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Controller"))
                {
                    destinationModule.Modules[2] = sourceModule;
                    destinationNode.Inputs[2] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("X"))
                {
                    destinationModule.Modules[1] = sourceModule;
                    destinationNode.Inputs[1] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Y"))
                {
                    destinationModule.Modules[2] = sourceModule;
                    destinationNode.Inputs[2] = sourceNode;
                }
                else if(newConnection.DestConnector.Name.Equals("Z"))
                {
                    destinationModule.Modules[3] = sourceModule;
                    destinationNode.Inputs[3] = sourceNode;
                }
            }
        }

        /// <summary>
        /// Retrieve a connection between the two connectors.
        /// Returns null if there is no connection between the connectors.
        /// </summary>
        public ConnectionViewModel FindConnection(ConnectorViewModel connector1, ConnectorViewModel connector2)
        {
            Trace.Assert(connector1.Type != connector2.Type);

            //
            // Figure out which one is the source connector and which one is the
            // destination connector based on their connector types.
            //
            var sourceConnector = connector1.Type == ConnectorType.Output ? connector1 : connector2;
            var destConnector = connector1.Type == ConnectorType.Output ? connector2 : connector1;

            //
            // Now we can just iterate attached connections of the source
            // and see if it each one is attached to the destination connector.
            //

            foreach (var connection in sourceConnector.AttachedConnections)
            {
                if (connection.DestConnector == destConnector)
                {
                    //
                    // Found a connection that is outgoing from the source connector
                    // and incoming to the destination connector.
                    //
                    return connection;
                }
            }

            return null;
        }

        /// <summary>
        /// Delete the currently selected nodes from the view-model.
        /// </summary>
        public void DeleteSelectedNodes()
        {
            // Take a copy of the selected nodes list so we can delete nodes while iterating.
            var nodesCopy = this.Network.Nodes.ToArray();
            foreach (var node in nodesCopy)
            {
                if (node.IsSelected)
                {
                    DeleteNode(node, false);
                }
            }
        }

        public void Validate()
        {
            bool validNoise = ValidateDiagram.ValidateNoiseGeneration(Network.Nodes.Cast<NodeViewModel>().ToList());
            bool validNames = ValidateDiagram.ValidateUniqueNames(Network.Nodes.Cast<NodeViewModel>().ToList());

            if (!validNoise && validNames) MessageBox.Show("The diagram is invalid. Please verify links to the final module are valid and not circular.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (validNoise && !validNames) MessageBox.Show("The diagram is invalid. Please verify all modules have unique names.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (!validNoise && !validNames) MessageBox.Show("The diagram is invalid. Please verify links to the final module are valid and not circular. Please verify all modules have unique names.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else MessageBox.Show("The diagram is valid.", "Valid", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void DeleteAllNodes()
        {
            while (Network.Nodes.Count() > 0)
            {
                DeleteNode(Network.Nodes[0], true);
            }
        }

        /// <summary>
        /// Delete the node from the view-model.
        /// Also deletes any connections to or from the node.
        /// </summary>
        public void DeleteNode(NodeViewModel node, bool purge)
        {
            if (!node.Module.LibnoiseModule.GetType().Name.Equals("Final") || purge)
            {
                foreach (ConnectionViewModel cvm in node.AttachedConnections)
                {
                    DeleteConnection(cvm);
                }

                //
                // Remove all connections attached to the node. - not used because we need to ensure each node is also cleared of the dest/source modules
                //
                //this.Network.Connections.RemoveRange(node.AttachedConnections);

                //
                // Remove the node from the network.
                //
                this.Network.Nodes.Remove(node);
            }
            else
            {
                MessageBox.Show("You cannot delete the final node");
            }
        }

        /// <summary>
        /// Create a node and add it to the view-model.
        /// </summary>
        public NodeViewModel CreateNode(ModuleBase moduleBase, Point nodeLocation, bool centerNode)
        {
            System.Windows.Media.Brush generatorBrush = new LinearGradientBrush(Colors.White, Colors.Orange, 90.0);
            System.Windows.Media.Brush operatorBrush = new LinearGradientBrush(Colors.White, Colors.PowderBlue, 90.0);
            System.Windows.Media.Brush finalBrush = new LinearGradientBrush(Colors.White, Colors.YellowGreen, 90.0);

            NodeViewModel node = new NodeViewModel(new LibnoiseNode(moduleBase));
            node.Module.ID = moduleBase.GetType().Name + "_" + (Network.Nodes.Count(n => n.Module.ModuleType.Equals(moduleBase.GetType().Name)) + 1);
            node.X = nodeLocation.X;
            node.Y = nodeLocation.Y;

            node.StrokeBrush = System.Windows.Media.Brushes.Black;

            // this can be simplified, for sure, but I left in the giant ugly switch
            // in the event I want to do something unique by type, such as any additional
            // colouring or whatever. It's a little gross to look at, but it does the job
            switch (moduleBase.GetType().Name)
            {
                    // Generators should not have any input values
                case "Billow":
                    node.FillBrush = generatorBrush;
                    break;
                case "Checker":
                    node.FillBrush = generatorBrush;
                    break;
                case "Const":
                    node.FillBrush = generatorBrush;
                    break;
                case "Cylinders":
                    node.FillBrush = generatorBrush;
                    break;
                case "Perlin":
                    node.FillBrush = generatorBrush;
                    break;
                case "RidgedMultifractal":
                    node.FillBrush = generatorBrush;
                    break;
                case "Spheres":
                    node.FillBrush = generatorBrush;
                    break;
                case "Voronoi":
                    node.FillBrush = generatorBrush;
                    break;
                case "Abs":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Add":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Left"));
                    node.InputConnectors.Add(new ConnectorViewModel("Right"));
                    break;
                case "Blend":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Left"));
                    node.InputConnectors.Add(new ConnectorViewModel("Right"));
                    node.InputConnectors.Add(new ConnectorViewModel("Operator"));
                    break;
                case "Cache":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Clamp":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Curve":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Displace":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Primary"));
                    node.InputConnectors.Add(new ConnectorViewModel("X"));
                    node.InputConnectors.Add(new ConnectorViewModel("Y"));
                    node.InputConnectors.Add(new ConnectorViewModel("Z"));
                    break;
                case "Exponent":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Invert":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Max":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Left"));
                    node.InputConnectors.Add(new ConnectorViewModel("Right"));
                    break;
                case "Min":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Left"));
                    node.InputConnectors.Add(new ConnectorViewModel("Right"));
                    break;
                case "Multiply":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Left"));
                    node.InputConnectors.Add(new ConnectorViewModel("Right"));
                    break;
                case "Power":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Left"));
                    node.InputConnectors.Add(new ConnectorViewModel("Right"));
                    break;
                case "Rotate":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Scale":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "ScaleBias":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Select":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Primary"));
                    node.InputConnectors.Add(new ConnectorViewModel("Secondary"));
                    node.InputConnectors.Add(new ConnectorViewModel("Controller"));
                    break;
                case "Subtract":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Left"));
                    node.InputConnectors.Add(new ConnectorViewModel("Right"));
                    break;
                case "Terrace":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Translate":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Turbulence":
                    node.FillBrush = operatorBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                case "Final":
                    node.FillBrush = finalBrush;
                    node.InputConnectors.Add(new ConnectorViewModel("Input"));
                    break;
                default:
                    break;
            }

            // for the 'Final' node, don't add any outputs.
            if (!moduleBase.GetType().Name.Equals("Final"))
            {
                node.OutputConnectors.Add(new ConnectorViewModel("Output"));
            }

            if (centerNode)
            {
                // 
                // We want to center the node.
                //
                // For this to happen we need to wait until the UI has determined the 
                // size based on the node's data-template.
                //
                // So we define an anonymous method to handle the SizeChanged event for a node.
                //
                // Note: If you don't declare sizeChangedEventHandler before initializing it you will get
                //       an error when you try and unsubscribe the event from within the event handler.
                //
                EventHandler<EventArgs> sizeChangedEventHandler = null;
                sizeChangedEventHandler =
                    delegate(object sender, EventArgs e)
                    {
                        //
                        // This event handler will be called after the size of the node has been determined.
                        // So we can now use the size of the node to modify its position.
                        //
                        node.X -= node.Size.Width / 2;
                        node.Y -= node.Size.Height / 2;

                        //
                        // Don't forget to unhook the event, after the initial centering of the node
                        // we don't need to be notified again of any size changes.
                        //
                        node.SizeChanged -= sizeChangedEventHandler;
                    };

                //
                // Now we hook the SizeChanged event so the anonymous method is called later
                // when the size of the node has actually been determined.
                //
                node.SizeChanged += sizeChangedEventHandler;
            }

            //
            // Add the node to the view-model.
            //
            this.Network.Nodes.Add(node);

            return node;
        }

        /// <summary>
        /// Utility method to delete a connection from the view-model.
        /// </summary>
        public void DeleteConnection(ConnectionViewModel connection)
        {
            ModuleBase sourceModule = connection.SourceConnector.ParentNode.Module.LibnoiseModule;
            ModuleBase destinationModule = connection.DestConnector.ParentNode.Module.LibnoiseModule;

            // get destination type, connector name, assign to correct module
            if(connection.DestConnector.Name.Equals("Input"))
            {
                destinationModule.Modules[0] = null;
            }
            else if(connection.DestConnector.Name.Equals("Left"))
            {
                destinationModule.Modules[0] = null;
            }
            else if(connection.DestConnector.Name.Equals("Right"))
            {
                destinationModule.Modules[1] = null;
            }
            else if(connection.DestConnector.Name.Equals("Operator"))
            {
                destinationModule.Modules[2] = null;
            }
            else if(connection.DestConnector.Name.Equals("Primary"))
            {
                destinationModule.Modules[0] = null;
            }
            else if(connection.DestConnector.Name.Equals("Secondary"))
            {
                destinationModule.Modules[1] = null;
            }
            else if(connection.DestConnector.Name.Equals("Controller"))
            {
                destinationModule.Modules[2] = null;
            }
            else if(connection.DestConnector.Name.Equals("X"))
            {
                destinationModule.Modules[1] = null;
            }
            else if(connection.DestConnector.Name.Equals("Y"))
            {
                destinationModule.Modules[2] = null;
            }
            else if(connection.DestConnector.Name.Equals("Z"))
            {
                destinationModule.Modules[3] = null;
            }

            this.Network.Connections.Remove(connection);
        }
    }
}
