using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibNoise
{
    public class NoiseFactory
    {
        private const int UC_BORDER = 1;

        /// <summary>
        /// Generates a spherical projection of a point in the noise map.
        /// </summary>
        /// <param name="lat">The latitude of the point.</param>
        /// <param name="lon">The longitude of the point.</param>
        /// <returns>The corresponding noise map value.</returns>
        public static double GenerateSphericalPoint(ModuleBase module, double lat, double lon, int scale)
        {
            double r = System.Math.Cos(Utils.DegreesToRadians() * lat);
            return module.GetValue(r * System.Math.Cos(Utils.DegreesToRadians() * lon), System.Math.Sin(Utils.DegreesToRadians() * lat), r * System.Math.Sin(Utils.DegreesToRadians() * lon), scale);
        }

        /// <summary>
        /// Generates a spherical projection of the noise map.
        /// </summary>
        /// <param name="south">The clip region to the south.</param>
        /// <param name="north">The clip region to the north.</param>
        /// <param name="west">The clip region to the west.</param>
        /// <param name="east">The clip region to the east.</param>
        public static float[,] GenerateSpherical(ModuleBase module, int width, int height, double south, double north, double west, double east, bool isNormalized = true, int scale = 0, int threads = 1)
        {
            int ucWidth = width + UC_BORDER * 2;
            int ucHeight = height + UC_BORDER * 2;
            float[,] data = new float[ucWidth, ucHeight];

            if (east <= west || north <= south) throw new ArgumentException("Invalid east/west or north/south combination");
            if (module == null) throw new ArgumentNullException("Generator is null");

            double loe = east - west;
            double lae = north - south;
            double xd = loe / ((double)(width - UC_BORDER));
            double yd = lae / ((double)(height - UC_BORDER));
            double clo = west;

            List<ParallelProcessHelper> processValues = new List<ParallelProcessHelper>();

            // Create parallel process helpers with the values for each sample
            for (int x = 0; x < ucWidth; x++)
            {
                double cla = south;
                for (int y = 0; y < ucHeight; y++)
                {
                    processValues.Add(new ParallelProcessHelper(x, y, cla, clo));
                    cla += yd;
                }
                clo += xd;
            }

            // now that we have each sample calculated, run through and fetch the noise
            // Parallelism should be set somwhere from 1 to Processor Count / 2.
            var rangePartitioner = Partitioner.Create(0, processValues.Count);
            Parallel.ForEach(rangePartitioner, new ParallelOptions { MaxDegreeOfParallelism = threads }, range =>
            {
                for (int i = range.Item1; i < range .Item2; i++)
                {
                    ParallelProcessHelper processHelper = processValues[i];
                    double sample = GenerateSphericalPoint(module, processHelper.innerValue, processHelper.outerValue, scale);
                    if (isNormalized) sample = (sample + 1) / 2;

                    data[processHelper.x, processHelper.y] = (float)sample;
                }
            });

            return data;
        }

        /// <summary>
        /// Generates a cylindrical projection of a point in the noise map.
        /// </summary>
        /// <param name="angle">The angle of the point.</param>
        /// <param name="height">The height of the point.</param>
        /// <returns>The corresponding noise map value.</returns>
        public static double GenerateCylindricalPoint(ModuleBase module, double angle, double height, int scale)
        {
            double x = System.Math.Cos(angle * Utils.DegreesToRadians());
            double y = height;
            double z = System.Math.Sin(angle * Utils.DegreesToRadians());

            return module.GetValue(x, y, z, scale);
        }

        /// <summary>
        /// Generates a cylindrical projection of the noise map. Result will be buffered with extra data
        /// </summary>
        /// <param name="angleMin">The maximum angle of the clip region.</param>
        /// <param name="angleMax">The minimum angle of the clip region.</param>
        /// <param name="heightMin">The minimum height of the clip region.</param>
        /// <param name="heightMax">The maximum height of the clip region.</param>
        public static double[,] GenerateCylindrical(ModuleBase module, int width, int height, double angleMin, double angleMax, double heightMin, double heightMax, bool isNormalized = true, int scale = 0)
        {
            int ucWidth = width + UC_BORDER * 2;
            int ucHeight = height + UC_BORDER * 2;
            double[,] data = new double[ucWidth, ucHeight];

            if (angleMax <= angleMin || heightMax <= heightMin) throw new ArgumentException("Invalid angle or height parameters");
            if (module == null) throw new ArgumentNullException("Generator is null");

            double ae = angleMax - angleMin;
            double he = heightMax - heightMin;
            double xd = ae / ((double)(width - UC_BORDER));
            double yd = he / ((double)(height - UC_BORDER));
            double ca = angleMin;

            List<ParallelProcessHelper> processValues = new List<ParallelProcessHelper>();

            for (int x = 0; x < ucWidth; x++)
            {
                double ch = heightMin;
                for (int y = 0; y < ucHeight; y++)
                {
                    processValues.Add(new ParallelProcessHelper(x, y, ca, ch));
                    ch += yd;
                }
                ca += xd;
            }

            var rangePartitioner = Partitioner.Create(0, processValues.Count);
            Parallel.ForEach(rangePartitioner, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    ParallelProcessHelper processHelper = processValues[i];
                    double sample = GenerateCylindricalPoint(module, processHelper.innerValue, processHelper.outerValue, scale);
                    if (isNormalized) sample = (sample + 1) / 2;

                    data[processHelper.x, processHelper.y] = sample;
                }
            });

            return data;
        }

        /// <summary>
        /// Generates a planar projection of a point in the noise map.
        /// </summary>
        /// <param name="x">The position on the x-axis.</param>
        /// <param name="y">The position on the y-axis.</param>
        /// <returns>The corresponding noise map value.</returns>
        public static double GeneratePlanarPoint(ModuleBase module, double x, double y, int scale = 0)
        {
            return module.GetValue(x, 0.0, y, scale);
        }

        /// <summary>
        /// Generates a non-seamless planar projection of the noise map. Result will be buffered with extra data
        /// </summary>
        /// <param name="left">The clip region to the left.</param>
        /// <param name="right">The clip region to the right.</param>
        /// <param name="top">The clip region to the top.</param>
        /// <param name="bottom">The clip region to the bottom.</param>
        /// <param name="isSeamless">Indicates whether the resulting noise map should be seamless.</param>
        public static double[,] GeneratePlanar(ModuleBase module, int width, int height, double left, double right, double top, double bottom, bool isSeamless = true, bool isNormalized = true, int scale = 0)
        {
            int ucWidth = width + UC_BORDER * 2;
            int ucHeight = height + UC_BORDER * 2;
            double[,] data = new double[ucWidth, ucHeight];

            if (right <= left || bottom <= top) throw new ArgumentException("Invalid right/left or bottom/top combination");
            if (module == null) throw new ArgumentNullException("Base Module is null");

            double xe = right - left;
            double ze = bottom - top;
            double xd = xe / ((double)width - UC_BORDER);
            double zd = ze / ((double)height - UC_BORDER);
            double xc = left;

            for (var x = 0; x < ucWidth; x++)
            {
                double zc = top;
                for (var y = 0; y < ucHeight; y++)
                {
                    double fv;

                    if (isSeamless) fv = GeneratePlanarPoint(module, xc, zc, scale);
                    else
                    {
                        double swv = GeneratePlanarPoint(module, xc, zc, scale);
                        double sev = GeneratePlanarPoint(module, xc + xe, zc, scale);
                        double nwv = GeneratePlanarPoint(module, xc, zc + ze, scale);
                        double nev = GeneratePlanarPoint(module, xc + xe, zc + ze, scale);

                        double xb = 1.0 - ((xc - left) / xe);
                        double zb = 1.0 - ((zc - top) / ze);
                        
                        double z0 = Utils.InterpolateLinear(swv, sev, xb);
                        double z1 = Utils.InterpolateLinear(nwv, nev, xb);

                        fv = Utils.InterpolateLinear(z0, z1, zb);
                    }

                    if (isNormalized) fv = (fv + 1) / 2;

                    data[x, y] = fv;
                    zc += zd;
                }
                xc += xd;
            }

            return data;
        }
    }

    /// <summary>
    /// A helper class to create single objects from the double for loops
    /// so we can run the GetVales process in a parallel foreach
    /// </summary>
    public class ParallelProcessHelper
    {
        public int x { get; set; }
        public int y { get; set; }
        public double innerValue { get; set; }
        public double outerValue { get; set; }

        public ParallelProcessHelper() { }
        public ParallelProcessHelper(int x, int y, double innerValue, double outerValue)
        {
            this.x = x;
            this.y = y;
            this.innerValue = innerValue;
            this.outerValue = outerValue;
        }
    }
}
