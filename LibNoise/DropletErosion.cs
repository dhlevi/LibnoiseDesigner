using System.Collections.Generic;
using System.Linq;

namespace LibNoise
{
    public class DropletErosion
    {
        public static void Erode(float[,] data, float[,] moisture, float seaLevel, int iterations, float erosionFactor, int buffer)
        {
            int width = data.GetLength(0) - 2;
            int height = data.GetLength(1) - 2;

            float[,] adjustedHeight = new float[width, height];
            float[,] pooledWater = new float[width, height];

            int count = 0;
            while (count < iterations)
            {
                for (int y = 0; y < height; y++ )
                {
                    for (int x = 0; x < width; x++)
                    {
                        float heightValue = data[x + buffer, y + buffer] + pooledWater[x, y];

                        if (heightValue > seaLevel)
                        {
                            float rain = (moisture[x, y] < 0.1f ? 0.1f : moisture[x, y]) * erosionFactor;

                            //determine slopes
                            int bx = x + buffer;
                            int by = y + buffer;

                            // surrounding heights
                            float nw = data[bx - 1, by - 1];
                            float n = data[bx, by - 1];
                            float ne = data[bx + 1, by - 1];
                            float w = data[bx - 1, by];
                            float e = data[bx + 1, by];
                            float sw = data[bx - 1, by + 1];
                            float s = data[bx, by + 1];
                            float se = data[bx + 1, by + 1];

                            List<KeyValuePair<int, float>> flows = new List<KeyValuePair<int, float>>();

                            flows.Add(new KeyValuePair<int, float>(1, nw));
                            flows.Add(new KeyValuePair<int, float>(2, n));
                            flows.Add(new KeyValuePair<int, float>(3, ne));
                            flows.Add(new KeyValuePair<int, float>(4, w));
                            flows.Add(new KeyValuePair<int, float>(5, e));
                            flows.Add(new KeyValuePair<int, float>(6, sw));
                            flows.Add(new KeyValuePair<int, float>(7, s));
                            flows.Add(new KeyValuePair<int, float>(8, se));

                            // order slopes by highest to lowest
                            flows = flows.OrderBy(kvp => kvp.Value).ToList();

                            foreach(KeyValuePair<int, float> slope in flows)
                            {
                                if(slope.Value < heightValue && rain > 0)
                                {
                                    // if we're not at the north/south border
                                    if (!((y == 0 && slope.Key == 1) || (y == 0 && slope.Key == 2) || (y == 0 && slope.Key == 3) ||
                                        (y == height - 1 && slope.Key == 6) || (y == height - 1 && slope.Key == 7) || (y == height - 1 && slope.Key == 8)))
                                    {
                                        float flowReduction = rain * (slope.Value / heightValue);
                                        if (slope.Key.Equals(1)) adjustedHeight[x > 0 ? x - 1 : width - 1, y - 1] -= flowReduction;
                                        else if (slope.Key.Equals(2)) adjustedHeight[x, y - 1] -= flowReduction;
                                        else if (slope.Key.Equals(3)) adjustedHeight[x < width - 1 ? x + 1 : 0, y - 1] -= flowReduction;
                                        else if (slope.Key.Equals(4)) adjustedHeight[x > 0 ? x - 1 : width - 1, y] -= flowReduction;
                                        else if (slope.Key.Equals(5)) adjustedHeight[x < width - 1 ? x + 1 : 0, y] -= flowReduction;
                                        else if (slope.Key.Equals(6)) adjustedHeight[x > 0 ? x - 1 : width - 1, y + 1] -= flowReduction;
                                        else if (slope.Key.Equals(7)) adjustedHeight[x, y + 1] -= flowReduction;
                                        else if (slope.Key.Equals(8)) adjustedHeight[x < width - 1 ? x + 1 : 0, y + 1] -= flowReduction;
                                        
                                        rain -= flowReduction;
                                        if (rain < 0) rain = 0.0f;
                                    }
                                }
                            }

                            // add any remaining water to the water pool
                            if(rain > 0) pooledWater[x, y] += rain;
                        }
                    }
                }

                count++;
            }

            //iterations done, so subtract diff from height
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (data[x + buffer, y + buffer] > seaLevel)
                    {
                        float adjustedval = data[x + buffer, y + buffer] + adjustedHeight[x, y];

                        if (adjustedval <= seaLevel) adjustedval = seaLevel + 0.01f;
                        else if (adjustedval > 1.0f) adjustedval = 1.0f;

                        data[x + buffer, y + buffer] = adjustedval;
                    }
                }
            }
        }
    }
}
