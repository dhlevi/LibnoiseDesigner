using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that uses three source modules to displace each
    /// coordinate of the input value before returning the output value from
    /// a source module. [OPERATOR]
    /// </summary>
    public class Displace : ModuleBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of Displace.
        /// </summary>
        public Displace()
            : base(4)
        {
        }

        /// <summary>
        /// Initializes a new instance of Displace.
        /// </summary>
        /// <param name="input">The input module.</param>
        /// <param name="x">The displacement module of the x-axis.</param>
        /// <param name="y">The displacement module of the y-axis.</param>
        /// <param name="z">The displacement module of the z-axis.</param>
        public Displace(ModuleBase input, ModuleBase x, ModuleBase y, ModuleBase z)
            : base(4)
        {
            Modules[0] = input;
            Modules[1] = x;
            Modules[2] = y;
            Modules[3] = z;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the controlling module on the x-axis.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("X Displacement")]
        [Description("Sets the displacement amount to apply to the x coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public ModuleBase X
        {
            get 
            { 
                return Modules[1]; 
            }
            set
            {
                Modules[1] = value;
            }
        }

        /// <summary>
        /// Gets or sets the controlling module on the z-axis.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Y Displacement")]
        [Description("Sets the displacement amount to apply to the y coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public ModuleBase Y
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

        /// <summary>
        /// Gets or sets the controlling module on the z-axis.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Z Displacement")]
        [Description("Sets the displacement amount to apply to the z coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public ModuleBase Z
        {
            get 
            { 
                return Modules[3]; 
            }
            set
            {
                Modules[3] = value;
            }
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that uses three source modules to displace each coordinate of the input value before returning the output value from a source module.";
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
            double dx = x + Modules[1].GetValue(x, y, z, scale);
            double dy = y + Modules[2].GetValue(x, y, z, scale);
            double dz = z + Modules[3].GetValue(x, y, z, scale);

            return Modules[0].GetValue(dx, dy, dz, scale);
        }

        #endregion
    }
}