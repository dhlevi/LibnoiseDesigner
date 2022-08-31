using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNoise
{
    public class Erosion
    {
        // Thermal erosion "collapses" cliffs and evens out heights based
        // from the difference in height between a point and its neighbours
        // This difference is called the "Talus Angle". The lower the value of
        // the talus angle, the more erosion will occur. Higher values will 
        // collapse extreme cliffs, but keep the terrain more jagged
        public static void ThermalErosion(float[,] data, float talusAngle, int iterations)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            for (int i = 0; i < iterations; i++)
            {
                Parallel.For(0, height, y =>
                {
                    Parallel.For(0, width, x =>
                    //for (int x = 0; x < width; x++)
                    {
                        // this pixel height value
                        float heightValue = data[x, y];

                        // neighbouring height values
                        // if we're on an e/w edge, loop around the map. North and south do not loop.

                        float nw = y == 0 ? -1 : x == 0 ? data[width - 1, y - 1] : data[x - 1, y - 1];
                        float n = y == 0 ? -1 : data[x, y - 1];
                        float ne = y == 0 ? -1 : x == width - 1 ? data[0, y - 1] : data[x + 1, y - 1];
                        float e = x == width - 1 ? data[0, y] : data[x + 1, y];
                        float se = y == height - 1 ? -1 : x == width - 1 ? data[0, y + 1] : data[x + 1, y + 1];
                        float s = y == height - 1 ? -1 : data[x, y + 1];
                        float sw = y == height - 1 ? -1 : x == 0 ? data[width - 1, y + 1] : data[x - 1, y + 1];
                        float w = x == 0 ? data[width - 1, y] : data[x - 1, y];

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

                        foreach (KeyValuePair<int, float> slope in flows)
                        {
                            if (slope.Value != -1 && heightValue > slope.Value)
                            {
                                float difference = heightValue - slope.Value;

                                if (difference >= talusAngle)
                                {
                                    heightValue = heightValue - (difference / 2);
                                    // apply the difference that moved from the height to the correct neighbour
                                    if (slope.Key.Equals(1)) data[x > 0 ? x - 1 : width - 1, y - 1] += difference / 2;
                                    else if (slope.Key.Equals(2)) data[x, y - 1] += difference / 2;
                                    else if (slope.Key.Equals(3)) data[x < width - 1 ? x + 1 : 0, y - 1] += difference / 2;
                                    else if (slope.Key.Equals(4)) data[x > 0 ? x - 1 : width - 1, y] += difference / 2;
                                    else if (slope.Key.Equals(5)) data[x < width - 1 ? x + 1 : 0, y] += difference / 2;
                                    else if (slope.Key.Equals(6)) data[x > 0 ? x - 1 : width - 1, y + 1] += difference / 2;
                                    else if (slope.Key.Equals(7)) data[x, y + 1] += difference / 2;
                                    else if (slope.Key.Equals(8)) data[x < width - 1 ? x + 1 : 0, y + 1] += difference / 2;
                                }
                            }
                        }

                        // we're done, update the main pixel height data
                        data[x, y] = heightValue;
                    });
                });
            }
        }

        // Hydraulic erosion without sediment deposition
        // set mustHitSeaLevel to TRUE for coastal erosion
        public static void HydraulicErosion(float[,] data, float erosionAmount, float seaLevel, bool mustHitSealevel, float minimumHeight, int iterations)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            float[,] waterBuildup = new float[width, height];

            for (int i = 0; i < iterations; i++)
            {
                Parallel.For(0, height, y =>
                {
                    Parallel.For(0, width, x =>
                    {
                        //for (int x = 0; x < width; x++)
                        //{
                        int newX = x;
                        int newY = y;
                        int lastX = -1;
                        int lastY = -1;

                        bool stuck = false;
                        bool isLoop = false;
                        bool hitTheSea = false;
                        float thisHeight = data[newX, newY];

                        List<Point> points = new List<Point>();

                        if (thisHeight >= minimumHeight)
                        {
                            while (!stuck)
                            {
                                points.Add(new Point(newX, newY));

                                float nw = newY == 0 ? -1 : newX == 0 ? data[width - 1, newY - 1] : data[newX - 1, newY - 1];
                                float n = newY == 0 ? -1 : data[newX, newY - 1];
                                float ne = newY == 0 ? -1 : newX == width - 1 ? data[0, newY - 1] : data[newX + 1, newY - 1];
                                float e = newX == width - 1 ? data[0, newY] : data[newX + 1, newY];
                                float se = newY == height - 1 ? -1 : newX == width - 1 ? data[0, newY + 1] : data[newX + 1, newY + 1];
                                float s = newY == height - 1 ? -1 : data[newX, newY + 1];
                                float sw = newY == height - 1 ? -1 : newX == 0 ? data[width - 1, newY + 1] : data[newX - 1, newY + 1];
                                float w = newX == 0 ? data[width - 1, newY] : data[newX - 1, newY];

                                List<KeyValuePair<int, float>> flows = new List<KeyValuePair<int, float>>();

                                flows.Add(new KeyValuePair<int, float>(1, nw));
                                flows.Add(new KeyValuePair<int, float>(2, n));
                                flows.Add(new KeyValuePair<int, float>(3, ne));
                                flows.Add(new KeyValuePair<int, float>(4, w));
                                flows.Add(new KeyValuePair<int, float>(5, e));
                                flows.Add(new KeyValuePair<int, float>(6, sw));
                                flows.Add(new KeyValuePair<int, float>(7, s));
                                flows.Add(new KeyValuePair<int, float>(8, se));

                                // order slopes by lowest to highest
                                flows = flows.OrderBy(kvp => kvp.Value).Where(kvp => kvp.Value > 0).ToList();

                                if (flows.Count > 0)
                                {
                                    KeyValuePair<int, float> lowestHeight = flows.First();

                                    float heightPlusWater = thisHeight + waterBuildup[newX, newY];

                                    // get the next direction
                                    if (heightPlusWater >= lowestHeight.Value && data[newX, newY] > seaLevel)
                                    //if (data[newX, newY] > seaLevel)
                                    {
                                        lastX = newX;
                                        lastY = newY;

                                        if (lowestHeight.Key.Equals(1)) { newX = newX > 0 ? newX - 1 : width - 1; newY = newY - 1; }
                                        else if (lowestHeight.Key.Equals(2)) { newY = newY - 1; }
                                        else if (lowestHeight.Key.Equals(3)) { newX = newX < width - 1 ? newX + 1 : 0; newY = newY - 1; }
                                        else if (lowestHeight.Key.Equals(4)) { newX = newX > 0 ? newX - 1 : width - 1; }
                                        else if (lowestHeight.Key.Equals(5)) { newX = newX < width - 1 ? newX + 1 : 0; }
                                        else if (lowestHeight.Key.Equals(6)) { newX = newX > 0 ? newX - 1 : width - 1; newY = newY + 1; }
                                        else if (lowestHeight.Key.Equals(7)) { newY = newY + 1; }
                                        else if (lowestHeight.Key.Equals(8)) { newX = newX < width - 1 ? newX + 1 : 0; newY = newY + 1; }

                                        if (newY < 0) newY = 0;
                                        else if (newY >= height) newY = height - 1;

                                        if (newX < 0) newX = width - 1;
                                        else if (newX >= width) newX = 0;

                                        thisHeight = lowestHeight.Value;

                                        if (lastX == newX && lastY == newY) stuck = true;
                                        if (points.Count(p => p.X == newX && p.Y == newY) > 0)
                                        {
                                            // we've hit a looping scenario where a flow loops back over itself
                                            isLoop = true;
                                            stuck = true;
                                        }
                                    }
                                    else
                                    {
                                        hitTheSea = data[newX, newY] <= seaLevel;
                                        stuck = true;
                                        if (!hitTheSea) waterBuildup[newX, newY] += erosionAmount;
                                    }
                                }
                                else
                                {
                                    stuck = true;
                                    waterBuildup[newX, newY] += erosionAmount;
                                }
                            }
                        }

                        // lower the heights in the points we covered
                        // ignore any set of points where we've looped back on ourselves, or we're less than a tolerance of points
                        if (!isLoop && points.Count > 10 && ((mustHitSealevel && hitTheSea) || !mustHitSealevel))
                        {
                            foreach (Point p in points)
                            {
                                data[p.X, p.Y] -= erosionAmount;
                            }
                        }
                    });
                });
            }
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
