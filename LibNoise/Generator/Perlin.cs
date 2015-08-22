using System.ComponentModel;
namespace LibNoise.Generator
{
    /// <summary>
    /// Provides a noise module that outputs a three-dimensional perlin noise. [GENERATOR]
    /// </summary>
    public class Perlin : ModuleBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the frequency of the first octave.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Frequency")]
        [Description("Sets the frequency of the first octave. The frequency determines how many changes occur along a unit length. Increasing the frequency will increase the number of features (and also decrease the size of these features) in a noise map.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the lacunarity of the perlin noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Lacunarity")]
        [Description("Sets the lacunarity of the perlin noise. Lacunarity, from the Latin lacuna meaning \"gap\" or \"lake\", is a counterpart to the fractal dimension that describes the texture of a fractal. It has to do with the size distribution of the holes. Roughly speaking, if a fractal has large gaps or holes, it has high lacunarity; on the other hand, if a fractal is almost translationally invariant, it has low lacunarity.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Lacunarity { get; set; }

        /// <summary>
        /// Gets or sets the quality of the perlin noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Quality")]
        [Description("Sets the quality used to generate the perlin noise.")]
        public QualityMode Quality { get; set; }

        /// <summary>
        /// Gets or sets the number of octaves of the perlin noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Octave")]
        [Description("Sets the number of octaves that generate the perlin noise. The number of octaves determines how many \"steps\" of frequency will be used to generate the noise. The higher the octave number, the \"busier\" the associated noise becomes. This is because higher octaves have higher frequencies than lower octaves.")]
        [Editor("IntegerUpDownEditor", "IntegerUpDownEditor")]
        public int OctaveCount
        {
            get 
            { 
                return _octaveCount; 
            }
            set 
            { 
                _octaveCount = Utils.Clamp(value, 1, Utils.OctavesMaximum); 
            }
        }

        /// <summary>
        /// Gets or sets the persistence of the perlin noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Persistence")]
        [Description("Sets the persistence value of the perlin noise. The persistence value determines how quickly the amplitudes fall for each successive octave. Increasing the persistence value will create a rougher noise map, while decreasing the persistence value will create a smoother noise map.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Persistence { get; set; }

        /// <summary>
        /// Gets or sets the seed of the perlin noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Seed")]
        [Description("This property defines the random seed used to generate coherent noise values.")]
        [Editor("IntegerUpDownEditor", "IntegerUpDownEditor")]
        public int Seed { get; set; }

        #endregion

        #region Fields

        private int _octaveCount = 6;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Perlin.
        /// </summary>
        public Perlin()
            : base(0)
        {
            Frequency = 1.0;
            Lacunarity = 2.0;
            Quality = QualityMode.Medium;
            Persistence = 0.5f;
        }

        /// <summary>
        /// Initializes a new instance of Perlin.
        /// </summary>
        /// <param name="frequency">The frequency of the first octave.</param>
        /// <param name="lacunarity">The lacunarity of the perlin noise.</param>
        /// <param name="persistence">The persistence of the perlin noise.</param>
        /// <param name="octaves">The number of octaves of the perlin noise.</param>
        /// <param name="seed">The seed of the perlin noise.</param>
        /// <param name="quality">The quality of the perlin noise.</param>
        public Perlin(double frequency, double lacunarity, double persistence, int octaves, int seed,
            QualityMode quality)
            : base(0)
        {
            Frequency = frequency;
            Lacunarity = lacunarity;
            OctaveCount = octaves;
            Persistence = persistence;
            Seed = seed;
            Quality = quality;
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that outputs 3-dimensional Perlin noise. Perlin noise is the sum of several coherent-noise functions of ever-increasing frequencies and ever-decreasing amplitudes. An important property of Perlin noise is that a small change in the input value will produce a small change in the output value, while a large change in the input value will produce a random change in the output value. This noise module outputs Perlin-noise values that usually range from -1.0 to +1.0, but there are no guarantees that all output values will exist within that range.";
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
            double value = 0.0;
            double cp = 1.0;

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            
            for (int i = 0; i < _octaveCount + scale; i++)
            {
                double nx = Utils.MakeInt32Range(x);
                double ny = Utils.MakeInt32Range(y);
                double nz = Utils.MakeInt32Range(z);

                long seed = (Seed + i) & 0xffffffff;
                double signal = Utils.GradientCoherentNoise3D(nx, ny, nz, seed, Quality);
                
                value += signal * cp;

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                
                cp *= Persistence;
            }

            return value;
        }

        #endregion
    }
}