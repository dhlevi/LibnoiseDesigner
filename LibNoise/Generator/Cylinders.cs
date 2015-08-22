using System;
using System.ComponentModel;

namespace LibNoise.Generator
{
    /// <summary>
    /// Provides a noise module that outputs concentric cylinders. [GENERATOR]
    /// </summary>
    public class Cylinders : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the frequency of the concentric cylinders.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Frequency")]
        [Description("Sets the frequency of the concentric cylinders. The frequency determines how many changes occur along a unit length. Increasing the frequency will increase the number of features (and also decrease the size of these features) in a noise map.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Frequency { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Cylinders.
        /// </summary>
        public Cylinders()
            : base(0)
        {
            Frequency = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Cylinders.
        /// </summary>
        /// <param name="frequency">The frequency of the concentric cylinders.</param>
        public Cylinders(double frequency)
            : base(0)
        {
            Frequency = frequency;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that outputs concentric cylinders. This noise module outputs concentric cylinders centered on the origin. These cylinders are oriented along the y axis similar to the concentric rings of a tree. Each cylinder extends infinitely along the y axis. The first cylinder has a radius of 1.0. Each subsequent cylinder has a radius that is 1.0 unit larger than the previous cylinder. The output value from this noise module is determined by the distance between the input value and the the nearest cylinder surface. The input values that are located on a cylinder surface are given the output value 1.0 and the input values that are equidistant from two cylinder surfaces are given the output value -1.0.";
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
            z *= Frequency;

            double dfc = Math.Sqrt(x * x + z * z);
            double dfss = dfc - Math.Floor(dfc);
            double dfls = 1.0 - dfss;
            double nd = Math.Min(dfss, dfls);
            
            return 1.0 - (nd * 4.0);
        }

        #endregion
    }
}