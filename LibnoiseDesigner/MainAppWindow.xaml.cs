using LibNoise.Operator;
using LibNoise;
using LibNoise.Generator;
using Microsoft.Win32;
using NetworkModel;
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
using System.Xml;
using System.Xml.Linq;
using WorldForge.LibnoiseDesigner;
using WorldForge.LibnoiseDesigner.Viewer;
using System.IO;
using System.Reflection;
using Utils;

namespace WorldForge
{
    /// <summary>
    /// Interaction logic for MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        public MainAppWindow()
        {
            InitializeComponent();
        }

        private void SaveLibnoise_Click(object sender, RoutedEventArgs e)
        {
            // validate the diagram first

            bool validNoise = ValidateDiagram.ValidateNoiseGeneration(LibnoiseDesigner.networkControl.Nodes.Cast<NodeViewModel>().ToList());
            bool validNames = ValidateDiagram.ValidateUniqueNames(LibnoiseDesigner.networkControl.Nodes.Cast<NodeViewModel>().ToList());

            if (!validNoise && validNames) MessageBox.Show("The diagram is invalid and cannot be saved at this time. Please verify links to the final module are valid and not circular.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (validNoise && !validNames) MessageBox.Show("The diagram is invalid and cannot be saved at this time. Please verify all modules have unique names.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (!validNoise && !validNames) MessageBox.Show("The diagram is invalid and cannot be saved at this time. Please verify links to the final module are valid and not circular. Please verify all modules have unique names.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                try
                {
                    XmlDocument doc = LibnoiseFileUtils.DiagramToXML(LibnoiseDesigner.networkControl.Nodes);

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "XML Document|*.xml";
                    saveFileDialog.Title = "Save libnoise XML File";
                    saveFileDialog.ShowDialog();

                    if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                    {
                        doc.Save(saveFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured attempting to save the Libnoise Document.");
                }
            }
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            LibnoiseDesigner.ViewModel.DeleteAllNodes();
            NodeViewModel finalNode = LibnoiseDesigner.ViewModel.CreateNode(new Final(), new System.Windows.Point(100, 60), false);
        }

        private void LoadLibnoiseItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Document|*.xml";
            openFileDialog.Title = "Save libnoise XML File";
            openFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(openFileDialog.FileName);

                LibnoiseFileUtils.LoadLibnoiseXml(doc, LibnoiseDesigner);

                LibnoiseDesigner.ResizeWorkarea();
            }
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LibnoiseDetailedItem_Click(object sender, RoutedEventArgs e)
        {
            LibnoiseFileUtils.LoadDetailedXml(LibnoiseDesigner);
        }

        private void LibnoiseSimpleItem_Click(object sender, RoutedEventArgs e)
        {
            LibnoiseFileUtils.LoadDefaultXml(LibnoiseDesigner);
        }

        private void ExportClassItem_Click(object sender, RoutedEventArgs e)
        {
             bool validNoise = ValidateDiagram.ValidateNoiseGeneration(LibnoiseDesigner.networkControl.Nodes.Cast<NodeViewModel>().ToList());
            bool validNames = ValidateDiagram.ValidateUniqueNames(LibnoiseDesigner.networkControl.Nodes.Cast<NodeViewModel>().ToList());

            if (!validNoise && validNames) MessageBox.Show("The diagram is invalid and cannot be saved at this time. Please verify links to the final module are valid and not circular.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (validNoise && !validNames) MessageBox.Show("The diagram is invalid and cannot be saved at this time. Please verify all modules have unique names.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (!validNoise && !validNames) MessageBox.Show("The diagram is invalid and cannot be saved at this time. Please verify links to the final module are valid and not circular. Please verify all modules have unique names.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "c# class Document|*.cs";
                    saveFileDialog.Title = "Save libnoise c# class File";
                    saveFileDialog.ShowDialog();

                    if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                    {
                        StringBuilder sb = LibnoiseFileUtils.ExportToClass(LibnoiseDesigner.networkControl.Nodes);

                        using (StreamWriter outfile = new StreamWriter(saveFileDialog.FileName))
                        {
                            outfile.Write(sb.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured attempting to save the Libnoise Document.");
                }
            }
        }

        private void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow pw = new PreviewWindow(LibnoiseDesigner.networkControl.Nodes);
            pw.ShowDialog();
        }
    }
}
