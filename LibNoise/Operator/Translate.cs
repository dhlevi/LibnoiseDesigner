using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that moves the coordinates of the input value before
    /// returning the output value from a source module. [OPERATOR]
    /// </summary>
    public class Translate : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the translation on the x-axis.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("X Translation")]
        [Description("Sets the translation amount to apply to the x coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the translation on the y-axis.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Y Translation")]
        [Description("Sets the translation amount to apply to the y coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the translation on the z-axis.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Z Translation")]
        [Description("Sets the translation amount to apply to the z coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Z { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Translate.
        /// </summary>
        public Translate()
            : base(1)
        {
            X = 1.0;
            Z = 1.0;
            Y = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Translate.
        /// </summary>
        /// <param name="input">The input module.</param>
        public Translate(ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            X = 1.0;
            Z = 1.0;
            Y = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of Translate.
        /// </summary>
        /// <param name="x">The translation on the x-axis.</param>
        /// <param name="y">The translation on the y-axis.</param>
        /// <param name="z">The translation on the z-axis.</param>
        /// <param name="input">The input module.</param>
        public Translate(double x, double y, double z, ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that moves the coordinates of the input value before returning the output value from a source module.";
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
            return Modules[0].GetValue(x + this.X, y + this.Y, z + this.Z, scale);
        }

        #endregion
    }
}