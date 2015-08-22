using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that clamps the output value from a source module to a
    /// range of values. [OPERATOR]
    /// </summary>
    public class Clamp : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the maximum to clamp to.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Maximum Bound")]
        [Description("Sets the upper boundary to clamp values to.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Maximum { get; set; }

        /// <summary>
        /// Gets or sets the minimum to clamp to.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Minimum Bound")]
        [Description("Sets the lower boundary to clamp values to.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Minimum { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Clamp.
        /// </summary>
        public Clamp()
            : base(1)
        {
            Minimum = -1.0;
            Maximum = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Clamp.
        /// </summary>
        /// <param name="input">The input module.</param>
        public Clamp(ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            Minimum = -1.0;
            Maximum = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Clamp.
        /// </summary>
        /// <param name="input">The input module.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public Clamp(double min, double max, ModuleBase input)
            : base(1)
        {
            Minimum = min;
            Maximum = max;
            Modules[0] = input;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the bounds.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public void SetBounds(double min, double max)
        {
            Minimum = min;
            Maximum = max;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that clamps the output value from a source module to a range of values. The range of values in which to clamp the output value is called the clamping range. If the output value from the source module is less than the minimum bound of the clamping range, this noise module clamps that value to the minimum bound. If the output value from the source module is greater than the maximum bound of the clamping range, this noise module clamps that value to the maximum bound.";
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
            if (Minimum > Maximum)
            {
                double t = Minimum;
                Minimum = Maximum;
                Maximum = t;
            }

            double v = Modules[0].GetValue(x, y, z, scale);
            
            if (v < Minimum) return Minimum;
            if (v > Maximum) return Maximum;

            return v;
        }

        #endregion
    }
}