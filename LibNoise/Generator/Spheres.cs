using System;
using System.ComponentModel;

namespace LibNoise.Generator
{
    /// <summary>
    /// Provides a noise module that outputs concentric spheres. [GENERATOR]
    /// </summary>
    public class Spheres : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the frequency of the concentric spheres.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Frequency")]
        [Description("Sets the frequency of the concentric spheres. The frequency determines how many changes occur along a unit length. Increasing the frequency will increase the number of features (and also decrease the size of these features) in a noise map.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Frequency { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Spheres.
        /// </summary>
        public Spheres()
            : base(0)
        {
            this.Frequency = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Spheres.
        /// </summary>
        /// <param name="frequency">The frequency of the concentric spheres.</param>
        public Spheres(double frequency)
            : base(0)
        {
            this.Frequency = frequency;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "This noise module outputs concentric spheres centered on the origin like the concentric rings of an onion. The first sphere has a radius of 1.0. Each subsequent sphere has a radius that is 1.0 unit larger than the previous sphere. The output value from this noise module is determined by the distance between the input value and the the nearest spherical surface. The input values that are located on a spherical surface are given the output value 1.0 and the input values that are equidistant from two spherical surfaces are given the output value -1.0.";
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

            double dfc = Math.Sqrt(x * x + y * y + z * z);
            double dfss = dfc - Math.Floor(dfc);
            double dfls = 1.0 - dfss;
            double nd = Math.Min(dfss, dfls);

            return 1.0 - (nd * 4.0);
        }

        #endregion
    }
}