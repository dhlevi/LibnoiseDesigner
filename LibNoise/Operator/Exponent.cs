using System;
using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that maps the output value from a source module onto an
    /// exponential curve. [OPERATOR]
    /// </summary>
    public class Exponent : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the exponent.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Value")]
        [Description("Sets the exponent value to apply to the output value from the source module.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Value { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Exponent.
        /// </summary>
        public Exponent()
            : base(1)
        {
            Value = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Exponent.
        /// </summary>
        /// <param name="input">The input module.</param>
        public Exponent(ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            Value = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Exponent.
        /// </summary>
        /// <param name="exponent">The exponent to use.</param>
        /// <param name="input">The input module.</param>
        public Exponent(double exponent, ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            Value = exponent;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that maps the output value from a source module onto an exponential curve.";
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
            double v = Modules[0].GetValue(x, y, z, scale);

            return (Math.Pow(Math.Abs((v + 1.0) / 2.0), Value) * 2.0 - 1.0);
        }

        #endregion
    }
}