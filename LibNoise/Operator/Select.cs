using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that outputs the value selected from one of two source
    /// modules chosen by the output value from a control module. [OPERATOR]
    /// </summary>
    public class Select : ModuleBase
    {
        #region Fields

        private double _fallOff;
        private double _raw;
        private double _min = -1.0;
        private double _max = 1.0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Select.
        /// </summary>
        public Select()
            : base(3)
        {
        }

        /// <summary>
        /// Initializes a new instance of Select.
        /// </summary>
        /// <param name="inputA">The first input module.</param>
        /// <param name="inputB">The second input module.</param>
        /// <param name="controller">The controller module.</param>
        public Select(ModuleBase inputA, ModuleBase inputB, ModuleBase controller)
            : base(3)
        {
            Modules[0] = inputA;
            Modules[1] = inputB;
            Modules[2] = controller;
        }

        /// <summary>
        /// Initializes a new instance of Select.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="fallOff">The falloff value at the edge transition.</param>
        /// <param name="inputA">The first input module.</param>
        /// <param name="inputB">The second input module.</param>
        public Select(double min, double max, double fallOff, ModuleBase inputA, ModuleBase inputB, ModuleBase controller)
            : this(inputA, inputB, controller)
        {
            _min = min;
            _max = max;
            FallOff = fallOff;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the controlling module.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Controller")]
        [Description("The controller")]
        [Browsable(false)]
        public ModuleBase Controller
        {
            get { return Modules[2]; }
            set
            {
                Modules[2] = value;
            }
        }

        /// <summary>
        /// Gets or sets the falloff value at the edge transition.
        /// </summary>
		/// <remarks>
		/// Called SetEdgeFallOff() on the original LibNoise.
		/// </remarks>
        [Category("Noise Settings")]
        [DisplayName("Edge Falloff")]
        [Description("Sets the edge falloff value of the select operation. The falloff value is the width of the edge transition at either edge of the selection range. By default, there is an abrupt transition between the values from the two source modules at the boundaries of the selection range.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double FallOff
        {
            get { return _fallOff; }
            set
            {
                var bs = _max - _min;
                _raw = value;
                _fallOff = (value > bs / 2) ? bs / 2 : value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum, and re-calculated the fall-off accordingly.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Maximum Bounds")]
        [Description("Sets the maximum bounds of the select operation.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Maximum
        {
            get { return _max; }
            set
            {
                _max = value;
                FallOff = _raw;
            }
        }

        /// <summary>
		/// Gets or sets the minimum, and re-calculated the fall-off accordingly.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Minimum Bounds")]
        [Description("Sets the minimum bounds of the select operation.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Minimum
        {
            get { return _min; }
            set
            {
                _min = value;
                FallOff = _raw;
            }
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
            _min = min;
            _max = max;
            FallOff = _fallOff;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that outputs the value selected from one of two source modules chosen by the output value from a control module. By default, there is an abrupt transition between the output values from the two source modules at the selection-range boundary. To smooth the transition, pass a non-zero value to the Edge Falloff. Higher values result in a smoother transition.";
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
            double cv = Modules[2].GetValue(x, y, z, scale);

            if (_fallOff > 0.0)
            {
                double a;

                if (cv < (_min - _fallOff)) return Modules[0].GetValue(x, y, z, scale);
                
                if (cv < (_min + _fallOff))
                {
                    double lc = (_min - _fallOff);
                    double uc = (_min + _fallOff);

                    a = Utils.MapCubicSCurve((cv - lc) / (uc - lc));
                    
                    return Utils.InterpolateLinear(Modules[0].GetValue(x, y, z, scale), Modules[1].GetValue(x, y, z, scale), a);
                }

                if (cv < (_max - _fallOff)) return Modules[1].GetValue(x, y, z, scale);
                
                if (cv < (_max + _fallOff))
                {
                    double lc = (_max - _fallOff);
                    double uc = (_max + _fallOff);

                    a = Utils.MapCubicSCurve((cv - lc) / (uc - lc));
                    
                    return Utils.InterpolateLinear(Modules[1].GetValue(x, y, z, scale), Modules[0].GetValue(x, y, z, scale), a);
                }

                return Modules[0].GetValue(x, y, z, scale);
            }

            if (cv < _min || cv > _max) return Modules[0].GetValue(x, y, z, scale);
            
            return Modules[1].GetValue(x, y, z, scale);
        }

        #endregion
    }
}