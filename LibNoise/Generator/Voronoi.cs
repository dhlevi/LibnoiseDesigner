using System;
using System.ComponentModel;

namespace LibNoise.Generator
{
    /// <summary>
    /// Provides a noise module that outputs Voronoi cells. [GENERATOR]
    /// </summary>
    public class Voronoi : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the displacement value of the Voronoi cells.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Displacement")]
        [Description("Sets the displacement value of the Voronoi cells. The displacement value controls the range of random values to assign to each cell. The range of random values is +/- the displacement value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Displacement { get; set; }

        /// <summary>
        /// Gets or sets the frequency of the seed points.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Frequency")]
        [Description("Sets the frequency of the first octave. The frequency determines how many changes occur along a unit length. Increasing the frequency will increase the number of features (and also decrease the size of these features) in a noise map.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the seed value used by the Voronoi cells.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Seed")]
        [Description("This property defines the random seed used to generate coherent noise values.")]
        [Editor("IntegerUpDownEditor", "IntegerUpDownEditor")]
        public int Seed { get; set; }

        /// <summary>
        /// Gets or sets a value whether the distance from the nearest seed point is applied to the output value.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Enable Distance")]
        [Description("This property activates the distance function for Voronoi cells. This will add the distance from the nearest seed to the output value.")]
        public bool UseDistance { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Voronoi.
        /// </summary>
        public Voronoi()
            : base(0)
        {
            Frequency = 1.0;
            Displacement = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Voronoi.
        /// </summary>
        /// <param name="frequency">The frequency of the first octave.</param>
        /// <param name="displacement">The displacement of the ridged-multifractal noise.</param>
        /// <param name="seed">The seed of the ridged-multifractal noise.</param>
        /// <param name="distance">Indicates whether the distance from the nearest seed point is applied to the output value.</param>
        public Voronoi(double frequency, double displacement, int seed, bool distance)
            : base(0)
        {
            Frequency = frequency;
            Displacement = displacement;
            Seed = seed;
            UseDistance = distance;
            Seed = seed;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "In mathematics, a Voronoi cell is a region containing all the points that are closer to a specific seed point than to any other seed point. These cells mesh with one another, producing polygon-like formations. By default, this noise module randomly places a seed point within each unit cube. By modifying the frequency of the seed points, an application can change the distance between seed points. The higher the frequency, the closer together this noise module places the seed points, which reduces the size of the cells. This noise module assigns each Voronoi cell with a random constant value from a coherent-noise function. The displacement value controls the range of random values to assign to each cell. The range of random values is +/- the displacement value. This noise module can optionally add the distance from the nearest seed to the output value (enable distance). This causes the points in the Voronoi cells to increase in value the further away that point is from the nearest seed point.";
        }

        /// <summary>
        /// Returns the output value for the given input coordinates.
        /// </summary>
        /// <param name="x">The input coordinate on the x-axis.</param>
        /// <param name="y">The input coordinate on the y-axis.</param>
        /// <param name="z">The input coordinate on the z-axis.</param>
        /// <returns>The resulting output value.</returns>
        public override double GetValue(double x, double y, double z, int scale)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            int xi = (x > 0.0 ? (int)x : (int)x - 1);
            int iy = (y > 0.0 ? (int)y : (int)y - 1);
            int iz = (z > 0.0 ? (int)z : (int)z - 1);

            double md = 2147483647.0;
            
            double xc = 0;
            double yc = 0;
            double zc = 0;
            
            for (int zcu = iz - 2; zcu <= iz + 2; zcu++)
            {
                for (int ycu = iy - 2; ycu <= iy + 2; ycu++)
                {
                    for (int xcu = xi - 2; xcu <= xi + 2; xcu++)
                    {
                        double xp = xcu + Utils.ValueNoise3D(xcu, ycu, zcu, Seed);
                        double yp = ycu + Utils.ValueNoise3D(xcu, ycu, zcu, Seed + 1);
                        double zp = zcu + Utils.ValueNoise3D(xcu, ycu, zcu, Seed + 2);
                        double xd = xp - x;
                        double yd = yp - y;
                        double zd = zp - z;
                        double d = xd * xd + yd * yd + zd * zd;

                        if (d < md)
                        {
                            md = d;
                            xc = xp;
                            yc = yp;
                            zc = zp;
                        }
                    }
                }
            }

            double v;
            
            if (UseDistance)
            {
                double xd = xc - x;
                double yd = yc - y;
                double zd = zc - z;

                v = (Math.Sqrt(xd * xd + yd * yd + zd * zd)) * Utils.Sqrt3 - 1.0;
            }
            else
            {
                v = 0.0;
            }

            return v + (Displacement * Utils.ValueNoise3D((int)(Math.Floor(xc)), (int)(Math.Floor(yc)), (int)(Math.Floor(zc)), 0));
        }

        #endregion
    }
}