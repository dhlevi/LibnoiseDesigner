namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that outputs the sum of the two output values from two
    /// source modules. [OPERATOR]
    /// </summary>
    public class Add : ModuleBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of Add.
        /// </summary>
        public Add()
            : base(2)
        {
        }

        /// <summary>
        /// Initializes a new instance of Add.
        /// </summary>
        /// <param name="lhs">The left hand input module.</param>
        /// <param name="rhs">The right hand input module.</param>
        public Add(ModuleBase lhs, ModuleBase rhs)
            : base(2)
        {
            Modules[0] = lhs;
            Modules[1] = rhs;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that outputs the sum of the two output values from two source modules.";
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
            return Modules[0].GetValue(x, y, z, scale) + Modules[1].GetValue(x, y, z, scale);
        }

        #endregion
    }
}