using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using Utils;
using WorldForge.LibnoiseDesigner;

namespace WorldForge
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private ModuleBase module;
        private Bitmap previewBmp;
        private ImpObservableCollection<object> nodes;

        private int seed;
        private int imageWidth;
        private int imageHeight;
        private NoiseStyles selectedNoiseStyle;
        private ColourStyles selectedColorStyle;

        private enum NoiseStyles { Planar, Cylindrical, Spherical };
        private enum ColourStyles { Greyscale, RedBlue, World };

        // spherical constants
        private const float worldSouth = -90.0f;
        private const float worldNorth = 90.0f;
        private const float worldWest = -180.0f;
        private const float worldEast = 180.0f;

        public PreviewWindow(ImpObservableCollection<object> nodes)
        {
            InitializeComponent();

            // clone the module by generating a new module via serialization
            // this is not the nicest way of doing this, so we'll improve it later with a proper clone mechanism
            // for now though, it works quite well
            XmlDocument doc = LibnoiseFileUtils.DiagramToXML(nodes);
            ModuleBase module = LibnoiseFileUtils.LoadLibnoiseXml(doc);
            SetSeeds(module, 0);

            this.nodes = nodes;
            this.module = module;

            imageWidth = 512;
            imageHeight = 512;

            selectedNoiseStyle = NoiseStyles.Planar;
            selectedColorStyle = ColourStyles.Greyscale;

            GeneratePreview();
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // validate the height/width first
            bool widthValid = Int32.TryParse(WidthBox.Text, out imageWidth);
            bool heightValid = Int32.TryParse(HeightBox.Text, out imageHeight);
            bool seedValid = Int32.TryParse(SeedBox.Text, out seed);

            if (!widthValid) WidthBox.BorderBrush = System.Windows.Media.Brushes.Red;
            else WidthBox.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;

            if (!heightValid) HeightBox.BorderBrush = System.Windows.Media.Brushes.Red;
            else HeightBox.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;

            if (!seedValid) SeedBox.BorderBrush = System.Windows.Media.Brushes.Red;
            else SeedBox.BorderBrush = System.Windows.Media.Brushes.CornflowerBlue;

            string noiseStyle = ((ComboBoxItem)NoiseStyle.SelectedItem).Content.ToString();
            string colorStyle = ((ComboBoxItem)ColourStyle.SelectedItem).Content.ToString();

            if (noiseStyle.Equals("Planar")) selectedNoiseStyle = NoiseStyles.Planar;
            else if (noiseStyle.Equals("Cylindrical")) selectedNoiseStyle = NoiseStyles.Cylindrical;
            else selectedNoiseStyle = NoiseStyles.Spherical;

            if (colorStyle.Equals("Grayscale")) selectedColorStyle = ColourStyles.Greyscale;
            else if (colorStyle.Equals("Blue/Red")) selectedColorStyle = ColourStyles.RedBlue;
            else selectedColorStyle = ColourStyles.World;

            if (seedValid)
            {
                // clone the module by generating a new module via serialization
                // this is not the nicest way of doing this, so we'll improve it later with a proper clone mechanism
                // for now though, it works quite well
                XmlDocument doc = LibnoiseFileUtils.DiagramToXML(nodes);
                ModuleBase module = LibnoiseFileUtils.LoadLibnoiseXml(doc);
                SetSeeds(module, seed);
                this.module = module;
            }

            if (widthValid && heightValid && seedValid) GeneratePreview();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Png Image|*.png";
            sfd.Title = "Save an Png File";
            sfd.ShowDialog();

            if(sfd.FileName != "")
            {
                previewBmp.Save(sfd.FileName, ImageFormat.Png);
            }
        }

        private void GeneratePreview()
        {
            busyIndicator.IsBusy = true;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            previewBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.StreamSource = ms;
            bmpImage.EndInit();
            bmpImage.Freeze();

            PreviewImage.Source = bmpImage;

            busyIndicator.IsBusy = false;
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                double[,] noise = null;

                //depending on the style selected, create some default noise values
                if (selectedNoiseStyle.Equals(NoiseStyles.Planar))
                {
                    noise = NoiseFactory.GeneratePlanar(module, imageWidth, imageHeight, -1, 1, -1, 1, true, true, 0);
                }
                if (selectedNoiseStyle.Equals(NoiseStyles.Cylindrical))
                {
                    noise = NoiseFactory.GenerateCylindrical(module, imageWidth, imageHeight, -1, 180, -1, 1, true, 0);
                }
                else
                {
                    noise = NoiseFactory.GenerateSpherical(module, imageWidth, imageHeight, worldSouth, worldNorth, worldWest, worldEast, true, 0);
                }

                // Generate the image from the nosie values

                System.Drawing.Rectangle areaToPaint = new System.Drawing.Rectangle(0, 0, imageWidth, imageHeight);
                Bitmap bmp = new Bitmap(imageWidth, imageHeight);
                BitmapData bmData = bmp.LockBits(areaToPaint, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                
                //Generate a second image to overlay "Hillshading" for the world style
                Bitmap hillshadeBmp = new Bitmap(imageWidth, imageHeight);
                BitmapData hsbmData = hillshadeBmp.LockBits(areaToPaint, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int stride = imageWidth * 4;

                unsafe
                {
                    byte* ptr = (byte*)bmData.Scan0.ToPointer();
                    byte* hsPtr = (byte*)hsbmData.Scan0.ToPointer();

                    for (int y = areaToPaint.Top; y < areaToPaint.Height; y++)
                    {
                        for (int x = areaToPaint.Left; x < areaToPaint.Width; x++)
                        {
                            double heightValue = noise[x, y];

                            System.Drawing.Color finalColor = System.Drawing.Color.Black;

                            if (selectedColorStyle.Equals(ColourStyles.Greyscale)) finalColor = GetGradientColor(System.Drawing.Color.Black, System.Drawing.Color.White, (float)heightValue);
                            else if (selectedColorStyle.Equals(ColourStyles.RedBlue)) finalColor = GetGradientColor(System.Drawing.Color.Blue, System.Drawing.Color.Red, (float)heightValue);
                            else
                            {
                                // "World" style
                                if (heightValue <= 0.5) finalColor = System.Drawing.Color.PowderBlue;
                                else if (heightValue >= 0.9) finalColor = GetGradientColor(System.Drawing.Color.LightGray, System.Drawing.Color.Snow, (float)heightValue);
                                else finalColor = GetGradientColor(System.Drawing.Color.LightGreen, System.Drawing.Color.ForestGreen, (float)heightValue);

                                System.Drawing.Color hillshading = Hillshade(noise, imageWidth, imageHeight, x, y, 10, 45, 315, 1);

                                hsPtr[(x * 4) + y * stride] = ((Color)hillshading).B;
                                hsPtr[(x * 4) + y * stride + 1] = ((Color)hillshading).G;
                                hsPtr[(x * 4) + y * stride + 2] = ((Color)hillshading).R;

                                byte trans = 255;

                                if (heightValue > 0.5) trans = (byte)(((Color)hillshading).A * 0.3); // over land set transparency to 30%
                                else trans = (byte)(((Color)hillshading).A * 0.1); // over water transparency to 10%

                                hsPtr[(x * 4) + y * stride + 3] = trans;
                            }

                            ptr[(x * 4) + y * stride] = finalColor.B;
                            ptr[(x * 4) + y * stride + 1] = finalColor.G;
                            ptr[(x * 4) + y * stride + 2] = finalColor.R;
                            ptr[(x * 4) + y * stride + 3] = finalColor.A;
                        }
                    }
                }
                bmp.UnlockBits(bmData);
                hillshadeBmp.UnlockBits(hsbmData);

                if (selectedColorStyle.Equals(ColourStyles.World))
                {
                    Bitmap mergedResult = new Bitmap(imageWidth, imageHeight);

                    Graphics canvas = Graphics.FromImage(mergedResult);

                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    canvas.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                    canvas.DrawImage(hillshadeBmp, new Rectangle(0, 0, hillshadeBmp.Width, hillshadeBmp.Height), new Rectangle(0, 0, hillshadeBmp.Width, hillshadeBmp.Height), GraphicsUnit.Pixel);
                    canvas.Save();

                    previewBmp = mergedResult;
                }
                else previewBmp = bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured while generating the image preview. " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static System.Drawing.Color GetGradientColor(System.Drawing.Color gradientStart, System.Drawing.Color gradientEnd, float t)
        {
            t = t < 0.0f ? 0.0f : t;
            t = t > 1.0f ? 1.0f : t;

            double u = 1 - t;

            System.Drawing.Color color = System.Drawing.Color.FromArgb(
               (int)(gradientStart.A * u + gradientEnd.A * t),
               (int)(gradientStart.R * u + gradientEnd.R * t),
               (int)(gradientStart.G * u + gradientEnd.G * t),
               (int)(gradientStart.B * u + gradientEnd.B * t));

            return color;
        }

        public static System.Drawing.Color Hillshade(double[,] noise, int width, int height, int x, int y, double zFactor, double Altitude, double Azimuth, double cellSize, int buffer = 1)
        {
            double zenithDeg = 90.0 - Altitude;
            double zenithRad = zenithDeg * System.Math.PI / 180.0;

            double azimuthMath = 360.0 - Azimuth + 90.0;
            if (azimuthMath >= 360.0) azimuthMath = azimuthMath - 360.0;
            double azimuthRad = azimuthMath * System.Math.PI / 180.0;

            int bx = x + buffer;
            int by = y + buffer;

            // This will break if you don't use a buffer, or have a negative buffer, obviously
            double nw = noise[bx - 1, by - 1];
            double n = noise[bx, by - 1];
            double ne = noise[bx + 1, by - 1];
            double w = noise[bx - 1, by];
            double e = noise[bx + 1, by];
            double sw = noise[bx - 1, by + 1];
            double s = noise[bx, by + 1];
            double se = noise[bx + 1, by + 1];

            double xRateOfChange = ((ne + (2 * e) + se) - (nw + (2 * w) + sw)) / (8.0 * cellSize);
            double yRateOfChange = ((sw + (2 * s) + se) - (nw + (2 * n) + ne)) / (8.0 * cellSize);

            double slopeRad = System.Math.Atan(zFactor * System.Math.Sqrt((System.Math.Pow(xRateOfChange, 2) + System.Math.Pow(yRateOfChange, 2))));

            double aspectRad = 0.0f;

            if (xRateOfChange != 0) aspectRad = System.Math.Atan2(yRateOfChange, -xRateOfChange);
            if (aspectRad < 0) aspectRad = (2 * System.Math.PI) + aspectRad;

            if (xRateOfChange == 0)
            {
                if (yRateOfChange > 0) aspectRad = System.Math.PI / 2.0;
                else if (yRateOfChange < 0) aspectRad = (2.0 * System.Math.PI) - (System.Math.PI / 2.0);
            }

            int hillshade = Convert.ToInt32(System.Math.Round((255.0 * ((System.Math.Cos(zenithRad) * System.Math.Cos(slopeRad)) + (System.Math.Sin(zenithRad) * System.Math.Sin(slopeRad) * System.Math.Cos(azimuthRad - aspectRad)))), 0, MidpointRounding.ToEven));

            if (hillshade < 0) hillshade = 0;
            else if (hillshade > 255) hillshade = 255;

            // get pixel color
            Color col = Color.FromArgb(255, hillshade, hillshade, hillshade);

            return col;
        }

        // simple recursive call to cycle through a selected module
        // and their connected children and set the seed value to the 
        // seed selected by the user. If a seed value already exists,
        // then leave it alone... or maybe add it together?
        private void SetSeeds(ModuleBase module, int seed)
        {
            switch (module.GetType().Name)
            {
                case "Billow":
                    if (((Billow)module).Seed == 0) ((Billow)module).Seed = seed;
                    break;
                case "Perlin":
                    if (((Perlin)module).Seed == 0) ((Perlin)module).Seed = seed;
                    break;
                case "RidgedMultifractal":
                    if (((RidgedMultifractal)module).Seed == 0) ((RidgedMultifractal)module).Seed = seed;
                    break;
                case "Voronoi":
                    if (((Voronoi)module).Seed == 0) ((Voronoi)module).Seed = seed;
                    break;
                case "Turbulence":
                    if (((Turbulence)module).Seed == 0) ((Turbulence)module).Seed = seed;
                    break;
                default:
                    break;
            }

            if (module.Modules != null && module.Modules.Count() > 0)
            {
                foreach (ModuleBase mb in module.Modules)
                {
                    SetSeeds(mb, seed);
                }
            }
        }
    }
}
