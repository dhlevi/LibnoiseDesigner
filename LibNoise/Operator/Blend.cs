using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that outputs a weighted blend of the output values from
    /// two source modules given the output value supplied by a control module. [OPERATOR]
    /// </summary>
    public class Blend : ModuleBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of Blend.
        /// </summary>
        public Blend()
            : base(3)
        {
        }

        /// <summary>
        /// Initializes a new instance of Blend.
        /// </summary>
        /// <param name="lhs">The left hand input module.</param>
        /// <param name="rhs">The right hand input module.</param>
        /// <param name="controller">The controller of the operator.</param>
        public Blend(ModuleBase lhs, ModuleBase rhs, ModuleBase controller)
            : base(3)
        {
            Modules[0] = lhs;
            Modules[1] = rhs;
            Modules[2] = controller;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the controlling module.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Controller")]
        [Description("The control value for the blend operation")]
        [Browsable(false)]
        public ModuleBase Controller
        {
            get 
            { 
                return Modules[2]; 
            }
            set
            {
                Modules[2] = value;
            }
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that outputs a weighted blend of the output values from two source modules given the output value supplied by a control module.";
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
            double a = Modules[0].GetValue(x, y, z, scale);
            double b = Modules[1].GetValue(x, y, z, scale);
            double c = (Modules[2].GetValue(x, y, z, scale) + 1.0) / 2.0;

            return Utils.InterpolateLinear(a, b, c);
        }

        #endregion
    }
}