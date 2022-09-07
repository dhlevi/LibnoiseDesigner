using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibNoise
{
    /// <summary>
    /// Implementation of Brownian Motion Noise
    /// </summary>
    public class BrownianMotionNoise
    {
        // static Random random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour);

        /// <summary>
        /// Create noise tiled left-to-right
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="buffer"></param>
        public static void TileNoiseLeftRight(float[,] noise, int buffer)
        {
            int width = noise.GetLength(0);
            int height = noise.GetLength(1);

            for (int x = 0; x < buffer; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noise[(width - 1) - x, y] = noise[x, y];
                }
            }
        }

        /// <summary>
        /// Merge noise layer into white noise, by the provided percent
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="percent"></param>
        /// <param name="seed"></param>
        public static void AddNoise(float[,] noise, double percent, int seed)
        {
            int width = noise.GetLength(0);
            int height = noise.GetLength(1);

            MergeNoise(noise, GenerateWhiteNoise(width, height, seed), percent);
        }

        /// <summary>
        /// Merge noise layers together, by the provided percent
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="mergeValues"></param>
        /// <param name="percent"></param>
        public static void MergeNoise(float[,] noise, float[,] mergeValues, double percent)
        {
            int width = noise.GetLength(0);
            int height = noise.GetLength(1);

            Parallel.For(0, width, i =>
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i, j] += mergeValues[i, j] * Convert.ToSingle(percent / 100.0);
                    if (noise[i, j] > 1) noise[i, j] = 1;
                }
            });
        }

        /// <summary>
        /// Interpolate noise value
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        /// <summary>
        /// Generate a white noise base set
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static float[,] GenerateWhiteNoise(int width, int height, int seed)
        {
            Random random = new Random(seed);
            float[,] noise = new float[width, height];

            Parallel.For(0, width, i =>
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i, j] = (float)random.NextDouble() % 1;
                }
            });

            return noise;
        }

        /// <summary>
        /// Create smoothed noise from a base noise layer (usually white noise), with the provided octave
        /// </summary>
        /// <param name="baseNoise"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        public static float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
        {
            int width = baseNoise.GetLength(0);
            int height = baseNoise.GetLength(1);

            float[,] smoothNoise = new float[width, height];

            int samplePeriod = 1 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                // calculate the horizontal sampling indices
                int sample_i0 = (i / samplePeriod) * samplePeriod;
                int sample_i1 = (sample_i0 + samplePeriod) % width; // wrap around
                float horizontal_blend = (i - sample_i0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    // calculate the vertical sampling indices
                    int sample_j0 = (j / samplePeriod) * samplePeriod;
                    int sample_j1 = (sample_j0 + samplePeriod) % height; // wrap around
                    float vertical_blend = (j - sample_j0) * sampleFrequency;

                    // blend the top two corners
                    float top = Interpolate(baseNoise[sample_i0, sample_j0],
                        baseNoise[sample_i1, sample_j0], horizontal_blend);

                    // blend the bottom two corners
                    float bottom = Interpolate(baseNoise[sample_i0, sample_j1],
                        baseNoise[sample_i1, sample_j1], horizontal_blend);

                    // final blend
                    smoothNoise[i, j] = Interpolate(top, bottom, vertical_blend);
                }
            }

            return smoothNoise;
        }

        /// <summary>
        /// Generate Perlin noise
        /// </summary>
        /// <param name="baseNoise"></param>
        /// <param name="octaveCount"></param>
        /// <returns></returns>
        public static float[,] GeneratePerlinNoise(float[,] baseNoise, int octaveCount)
        {
            int width = baseNoise.GetLength(0);
            int height = baseNoise.GetLength(1);

            List<float[,]> smoothNoise = new List<float[,]>(octaveCount);

            float persistance = 0.7f;

            // generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise.Add(GenerateSmoothNoise(baseNoise, i));
            }

            float[,] bmNoise = new float[width, height];

            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            // blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        bmNoise[i, j] += smoothNoise[octave][i, j] * amplitude;
                    }
                }
            }

            // normalisation
            Parallel.For(0, width, i =>
            {
                for (int j = 0; j < height; j++)
                {
                    bmNoise[i, j] /= totalAmplitude;
                }
            });

            return bmNoise;
        }

        /// <summary>
        /// Create a noise layer of brownian motion noise, with the provided width, height, octave and seed
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="octaveCount"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static float[,] GenerateBrownianMotionNoise(int width, int height, int octaveCount, int seed)
        {
            float[,] baseNoise = GenerateWhiteNoise(width, height, seed);

            return GeneratePerlinNoise(baseNoise, octaveCount);
        }
    }
}
