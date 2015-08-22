using System;
using System.ComponentModel;

namespace LibNoise.Operator
{
    /// <summary>
    /// Provides a noise module that rotates the input value around the origin before
    /// returning the output value from a source module. [OPERATOR]
    /// </summary>
    public class Rotate : ModuleBase
    {
        #region Fields

        private double _x;
        private double _x1Matrix;
        private double _x2Matrix;
        private double _x3Matrix;
        private double _y;
        private double _y1Matrix;
        private double _y2Matrix;
        private double _y3Matrix;
        private double _z;
        private double _z1Matrix;
        private double _z2Matrix;
        private double _z3Matrix;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Rotate.
        /// </summary>
        public Rotate()
            : base(1)
        {
            SetAngles(0.0, 0.0, 0.0);
        }

        /// <summary>
        /// Initializes a new instance of Rotate.
        /// </summary>
        /// <param name="input">The input module.</param>
        public Rotate(ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
        }

        /// <summary>
        /// Initializes a new instance of Rotate.
        /// </summary>
        /// <param name="x">The rotation around the x-axis.</param>
        /// <param name="y">The rotation around the y-axis.</param>
        /// <param name="z">The rotation around the z-axis.</param>
        /// <param name="input">The input module.</param>
        public Rotate(double x, double y, double z, ModuleBase input)
            : base(1)
        {
            Modules[0] = input;
            SetAngles(x, y, z);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the rotation around the x-axis in degree.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("X Rotation")]
        [Description("Sets the rotation amount to apply to the x coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double X
        {
            get { return _x; }
            set { SetAngles(value, _y, _z); }
        }

        /// <summary>
        /// Gets or sets the rotation around the y-axis in degree.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Y Rotation")]
        [Description("Sets the rotation amount to apply to the y coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Y
        {
            get { return _y; }
            set { SetAngles(_x, value, _z); }
        }

        /// <summary>
        /// Gets or sets the rotation around the z-axis in degree.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Z Rotation")]
        [Description("Sets the rotation amount to apply to the z coordinate of the input value.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Z
        {
            get { return _x; }
            set { SetAngles(_x, _y, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the rotation angles.
        /// </summary>
        /// <param name="x">The rotation around the x-axis.</param>
        /// <param name="y">The rotation around the y-axis.</param>
        /// <param name="z">The rotation around the z-axis.</param>
        private void SetAngles(double x, double y, double z)
        {
            double xc = Math.Cos(x * Utils.DegreesToRadians());
            double yc = Math.Cos(y * Utils.DegreesToRadians());
            double zc = Math.Cos(z * Utils.DegreesToRadians());
            double xs = Math.Sin(x * Utils.DegreesToRadians());
            double ys = Math.Sin(y * Utils.DegreesToRadians());
            double zs = Math.Sin(z * Utils.DegreesToRadians());

            _x1Matrix = ys * xs * zs + yc * zc;
            _y1Matrix = xc * zs;
            _z1Matrix = ys * zc - yc * xs * zs;
            _x2Matrix = ys * xs * zc - yc * zs;
            _y2Matrix = xc * zc;
            _z2Matrix = -yc * xs * zc - ys * zs;
            _x3Matrix = -ys * xc;
            _y3Matrix = xs;
            _z3Matrix = yc * xc;

            _x = x;
            _y = y;
            _z = z;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that rotates the input value around the origin before returning the output value from a source module.";
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
            double nx = (_x1Matrix * x) + (_y1Matrix * y) + (_z1Matrix * z);
            double ny = (_x2Matrix * x) + (_y2Matrix * y) + (_z2Matrix * z);
            double nz = (_x3Matrix * x) + (_y3Matrix * y) + (_z3Matrix * z);

            return Modules[0].GetValue(nx, ny, nz, scale);
        }

        #endregion
    }
}