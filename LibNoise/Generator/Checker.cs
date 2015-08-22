using System;

namespace LibNoise.Generator
{
    /// <summary>
    /// Provides a noise module that outputs a checkerboard pattern. [GENERATOR]
    /// </summary>
    public class Checker : ModuleBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of Checker.
        /// </summary>
        public Checker()
            : base(0)
        {
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "This noise module outputs unit-sized blocks of alternating values. The values of these blocks alternate between -1.0 and +1.0. Because of this, the preview panel won't looks all that interesting (it forces values between 0 and 1). This noise module is not really useful by itself, but it is often used for debugging purposes.";
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
            int ix = (int)(Math.Floor(Utils.MakeInt32Range(x)));
            int iy = (int)(Math.Floor(Utils.MakeInt32Range(y)));
            int iz = (int)(Math.Floor(Utils.MakeInt32Range(z)));

            return (ix & 1 ^ iy & 1 ^ iz & 1) != 0 ? -1.0 : 1.0;
        }

        #endregion
    }
}