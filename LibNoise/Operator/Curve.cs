using System.Collections.Generic;
using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that maps the output value from a source module onto an
    /// arbitrary function curve. [OPERATOR]
    /// </summary>
    public class Curve : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets the number of control points.
        /// </summary>
        [Category("Control Point Info")]
        [DisplayName("Total Control Points")]
        [Description("How many control points have been added to this Curve operator.")]
        [Browsable(false)]
        public int ControlPointCount
        {
            get { return ControlPoints.Count; }
        }

        /// <summary>
        /// Gets the list of control points.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Control Points")]
        [Description("Set the control points required to form a curve operation.")]
        [Editor("CollectionEditor ", "CollectionEditor ")]
        public List<CurveControlPoint> ControlPoints { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Curve.
        /// </summary>
        public Curve()
            : base(1)
        {
            ControlPoints = new List<CurveControlPoint>();
        }

        /// <summary>
        /// Initializes a new instance of Curve.
        /// </summary>
        /// <param name="input">The input module.</param>
        public Curve(ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            ControlPoints = new List<CurveControlPoint>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a control point to the curve.
        /// </summary>
        /// <param name="input">The curves input value.</param>
        /// <param name="output">The curves output value.</param>
        public void Add(double input, double output)
        {
            var kvp = new CurveControlPoint()
            {
                X = input, 
                Y = output
            };

            if (!ControlPoints.Contains(kvp))
            {
                ControlPoints.Add(kvp);
            }

            ControlPoints.Sort(delegate(CurveControlPoint lhs, CurveControlPoint rhs)
            {
                return lhs.X.CompareTo(rhs.X);
            });
        }

        /// <summary>
        /// Clears the control points.
        /// </summary>
        public void Clear()
        {
            ControlPoints.Clear();
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that maps the output value from a source module onto an arbitrary function curve. This noise module maps the output value from the source module onto an application-defined curve. This curve is defined by a number of control points; each control point has an input value that maps to an output value.";
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
            foreach (CurveControlPoint cp in ControlPoints)
            {
                if (smv < ControlPoints[ip].X) break;
                ip++;
            }

            int i0 = Utils.Clamp(ip - 2, 0, ControlPoints.Count - 1);
            int i1 = Utils.Clamp(ip - 1, 0, ControlPoints.Count - 1);
            int i2 = Utils.Clamp(ip, 0, ControlPoints.Count - 1);
            int i3 = Utils.Clamp(ip + 1, 0, ControlPoints.Count - 1);
            
            if (i1 == i2) return ControlPoints[i1].Y;
                        
            double ip0 = ControlPoints[i1].X;
            double ip1 = ControlPoints[i2].X;
            double a = (smv - ip0) / (ip1 - ip0);

            return Utils.InterpolateCubic(ControlPoints[i0].Y, ControlPoints[i1].Y, ControlPoints[i2].Y, ControlPoints[i3].Y, a);
        }

        #endregion
    }

    /// <summary>
    /// Helper Point class for Curve control points. For use with Property grids, we need an empty construction, which Point class does not have 
    /// </summary>
    public class CurveControlPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}