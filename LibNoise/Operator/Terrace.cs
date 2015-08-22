using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that maps the output value from a source module onto a
    /// terrace-forming curve. [OPERATOR]
    /// </summary>
    public class Terrace : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets the number of control points.
        /// </summary>
        [Category("Control Point Info")]
        [DisplayName("Control Point Count")]
        [Description("The current count of control points added to this terrace.")]
        [Browsable(false)]
        public int ControlPointCount
        {
            get 
            {
                return ControlPoints.Count; 
            }
        }

        /// <summary>
        /// Gets the list of control points.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Control Points")]
        [Description("Add control points for the terrace. A minimum of two are required.")]
        [Editor("CollectionEditor", "CollectionEditor")]
        public List<double> ControlPoints { get; set; }

        /// <summary>
        /// Gets or sets a value whether the terrace curve is inverted.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Inverted")]
        [Description("Sets if the terrace-forming curve between the control points is inverted.")]
        public bool IsInverted { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Terrace.
        /// </summary>
        public Terrace()
            : base(1)
        {
            ControlPoints = new List<double>();
        }

        /// <summary>
        /// Initializes a new instance of Terrace.
        /// </summary>
        /// <param name="input">The input module.</param>
        public Terrace(ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            ControlPoints = new List<double>();
        }

        /// <summary>
        /// Initializes a new instance of Terrace.
        /// </summary>
        /// <param name="inverted">Indicates whether the terrace curve is inverted.</param>
        /// <param name="input">The input module.</param>
        public Terrace(bool inverted, ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            IsInverted = inverted;
            ControlPoints = new List<double>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a control point to the curve.
        /// </summary>
        /// <param name="input">The curves input value.</param>
        public void Add(double input)
        {
            if (!ControlPoints.Contains(input)) ControlPoints.Add(input);
            
            ControlPoints.Sort(delegate(double lhs, double rhs) 
            { 
                return lhs.CompareTo(rhs); 
            });
        }

        /// <summary>
        /// Clears the control points.
        /// </summary>
        public void Clear()
        {
            ControlPoints.Clear();
        }

        /// <summary>
        /// Auto-generates a terrace curve.
        /// </summary>
        /// <param name="steps">The number of steps.</param>
        public void Generate(int steps)
        {
            if (steps < 2) throw new ArgumentException("A minimum of two Control Points are required to process the Terrace operation.");
            
            Clear();

            double ts = 2.0 / (steps - 1.0);
            double cv = -1.0;
            
            for (var i = 0; i < steps; i++)
            {
                Add(cv);
                cv += ts;
            }
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "This noise module maps the output value from the source module onto a terrace-forming curve. The start of this curve has a slope of zero; its slope then smoothly increases. This curve also contains control points which resets the slope to zero at that point, producing a \"terracing\" effect. A minimum of two control points are required for this operator.";
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
            double smv = Modules[0].GetValue(x, y, z, scale);

            int ip = 0;

            foreach (double cp in ControlPoints)
            {
                if (smv < cp) break;
                ip++;
            }

            int i0 = Utils.Clamp(ip - 1, 0, ControlPoints.Count - 1);
            int i1 = Utils.Clamp(ip, 0, ControlPoints.Count - 1);
            
            if (i0 == i1) return ControlPoints[i1];

            double v0 = ControlPoints[i0];
            double v1 = ControlPoints[i1];
            double a = (smv - v0) / (v1 - v0);

            if (IsInverted)
            {
                a = 1.0 - a;

                double t = v0;
                
                v0 = v1;
                v1 = t;
            }

            a *= a;

            return Utils.InterpolateLinear(v0, v1, a);
        }

        #endregion
    }
}