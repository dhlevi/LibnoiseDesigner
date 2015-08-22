using System;
using System.ComponentModel;

namespace LibNoise.Generator
{
    /// <summary>
    /// Provides a noise module that outputs 3-dimensional ridged-multifractal noise. [GENERATOR]
    /// </summary>
    public class RidgedMultifractal : ModuleBase
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
        /// Gets or sets the lacunarity of the ridged-multifractal noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Lacunarity")]
        [Description("Sets the lacunarity of the ridged multifractal noise. Lacunarity, from the Latin lacuna meaning \"gap\" or \"lake\", is a counterpart to the fractal dimension that describes the texture of a fractal. It has to do with the size distribution of the holes. Roughly speaking, if a fractal has large gaps or holes, it has high lacunarity; on the other hand, if a fractal is almost translationally invariant, it has low lacunarity.")]
        [Editor("DoubleUpDownEditor", "DoubleUpDownEditor")]
        public double Lacunarity
        {
            get 
            { 
                return _lacunarity; 
            }
            set
            {
                _lacunarity = value;
                UpdateWeights();
            }
        }

        /// <summary>
        /// Gets or sets the quality of the ridged-multifractal noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Quality")]
        [Description("Sets the quality used to generate the perlin noise.")]
        public QualityMode Quality { get; set; }

        /// <summary>
        /// Gets or sets the number of octaves of the ridged-multifractal noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Octave")]
        [Description("Sets the number of octaves that generate the ridged multifractal noise. The number of octaves determines how many \"steps\" of frequency will be used to generate the noise. The higher the octave number, the \"busier\" the associated noise becomes. This is because higher octaves have higher frequencies than lower octaves.")]
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
        /// Gets or sets the seed of the ridged-multifractal noise.
        /// </summary>
        [Category("Noise Settings")]
        [DisplayName("Seed")]
        [Description("This property defines the random seed used to generate coherent noise values.")]
        [Editor("IntegerUpDownEditor", "IntegerUpDownEditor")]
        public int Seed { get; set; }

        #endregion

        #region Fields

        private double _lacunarity = 2.0;
        private int _octaveCount = 6;
        private readonly double[] _weights = new double[Utils.OctavesMaximum];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of RidgedMultifractal.
        /// </summary>
        public RidgedMultifractal()
            : base(0)
        {
            Frequency = 1.0;
            Quality = QualityMode.Medium;
            UpdateWeights();
        }

        /// <summary>
        /// Initializes a new instance of RidgedMultifractal.
        /// </summary>
        /// <param name="frequency">The frequency of the first octave.</param>
        /// <param name="lacunarity">The lacunarity of the ridged-multifractal noise.</param>
        /// <param name="octaves">The number of octaves of the ridged-multifractal noise.</param>
        /// <param name="seed">The seed of the ridged-multifractal noise.</param>
        /// <param name="quality">The quality of the ridged-multifractal noise.</param>
        public RidgedMultifractal(double frequency, double lacunarity, int octaves, int seed, QualityMode quality)
            : base(0)
        {
            Frequency = frequency;
            Lacunarity = lacunarity;
            OctaveCount = octaves;
            Seed = seed;
            Quality = quality;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the weights of the ridged-multifractal noise.
        /// </summary>
        private void UpdateWeights()
        {
            double f = 1.0;

            for (var i = 0; i < Utils.OctavesMaximum; i++)
            {
                _weights[i] = Math.Pow(f, -1.0);

                f *= _lacunarity;
            }
        }

        #endregion

        #region ModuleBase Members

        public override string GetDescription()
        {
            return "Noise module that outputs 3-dimensional ridged-multifractal noise. This noise module is heavily based on the Perlin-noise module. Ridged-multifractal noise is generated in much of the same way as Perlin noise, except the output of each octave is modified by an absolute-value function. Modifying the octave values in this way produces ridge-like formations. Ridged-multifractal noise does not use a persistence value. This is because the persistence values of the octaves are based on the values generated from from previous octaves, creating a feedback loop (or that's what it looks like after reading the code.) This noise module outputs ridged-multifractal-noise values that usually range from -1.0 to +1.0, but there are no guarantees that all output values will exist within that range.";
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
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            double value = 0.0;
            double weight = 1.0;
            double offset = 1.0; // TODO: Review why Offset is never assigned
            double gain = 2.0;   // TODO: Review why gain is never assigned

            for (var i = 0; i < _octaveCount + scale; i++)
            {
                double nx = Utils.MakeInt32Range(x);
                double ny = Utils.MakeInt32Range(y);
                double nz = Utils.MakeInt32Range(z);

                long seed = (Seed + i) & 0x7fffffff;
                double signal = Utils.GradientCoherentNoise3D(nx, ny, nz, seed, Quality);

                signal = Math.Abs(signal);
                signal = offset - signal;
                signal *= signal;
                signal *= weight;
                
                weight = signal * gain;
                weight = Utils.Clamp01((float) weight);
                
                value += (signal * _weights[i]);
                
                x *= _lacunarity;
                y *= _lacunarity;
                z *= _lacunarity;
            }

            return (value * 1.25) - 1.0;
        }

        #endregion
    }
}