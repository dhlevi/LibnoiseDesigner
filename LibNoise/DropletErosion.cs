using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNoise
{
    /// <summary>
    /// Simple Erosion simulation for adjusting noise. Taken predominantly
    /// from my existing Java project JavaLibNoise
    /// </summary>
    public class Erosion
    {
        /// <summary>
        /// Thermal erosion "collapses" cliffs and evens out heights based
        /// from the difference in height between a point and its neighbours
        /// This difference is called the "Talus Angle". The lower the value of
        /// the talus angle, the more erosion will occur. Higher values will 
        /// collapse extreme cliffs, but keep the terrain more jagged
        /// </summary>
        /// <param name="data"></param>
        /// <param name="talusAngle"></param>
        /// <param name="iterations"></param>
        public static void ThermalErosion(double[,] data, float talusAngle, int iterations)
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
                        double heightValue = data[x, y];

                        // neighbouring height values
                        // if we're on an e/w edge, loop around the map. North and south do not loop.

                        double nw = y == 0 ? -1 : x == 0 ? data[width - 1, y - 1] : data[x - 1, y - 1];
                        double n = y == 0 ? -1 : data[x, y - 1];
                        double ne = y == 0 ? -1 : x == width - 1 ? data[0, y - 1] : data[x + 1, y - 1];
                        double e = x == width - 1 ? data[0, y] : data[x + 1, y];
                        double se = y == height - 1 ? -1 : x == width - 1 ? data[0, y + 1] : data[x + 1, y + 1];
                        double s = y == height - 1 ? -1 : data[x, y + 1];
                        double sw = y == height - 1 ? -1 : x == 0 ? data[width - 1, y + 1] : data[x - 1, y + 1];
                        double w = x == 0 ? data[width - 1, y] : data[x - 1, y];

                        List<KeyValuePair<int, double>> flows = new List<KeyValuePair<int, double>>();

                        flows.Add(new KeyValuePair<int, double>(1, nw));
                        flows.Add(new KeyValuePair<int, double>(2, n));
                        flows.Add(new KeyValuePair<int, double>(3, ne));
                        flows.Add(new KeyValuePair<int, double>(4, w));
                        flows.Add(new KeyValuePair<int, double>(5, e));
                        flows.Add(new KeyValuePair<int, double>(6, sw));
                        flows.Add(new KeyValuePair<int, double>(7, s));
                        flows.Add(new KeyValuePair<int, double>(8, se));

                        // order slopes by highest to lowest
                        flows = flows.OrderBy(kvp => kvp.Value).ToList();

                        foreach (KeyValuePair<int, double> slope in flows)
                        {
                            if (slope.Value != -1 && heightValue > slope.Value)
                            {
                                double difference = heightValue - slope.Value;

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

        /// <summary>
        /// Hydraulic erosion without sediment deposition
        /// set mustHitSeaLevel to TRUE for coastal erosion
        /// </summary>
        /// <param name="data"></param>
        /// <param name="erosionAmount"></param>
        /// <param name="seaLevel"></param>
        /// <param name="mustHitSealevel"></param>
        /// <param name="minimumHeight"></param>
        /// <param name="iterations"></param>
        public static void HydraulicErosion(double[,] data, float erosionAmount, float seaLevel, bool mustHitSealevel, float minimumHeight, int iterations)
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
                        double thisHeight = data[newX, newY];

                        List<Point> points = new List<Point>();

                        if (thisHeight >= minimumHeight)
                        {
                            while (!stuck)
                            {
                                points.Add(new Point(newX, newY));

                                double nw = newY == 0 ? -1 : newX == 0 ? data[width - 1, newY - 1] : data[newX - 1, newY - 1];
                                double n = newY == 0 ? -1 : data[newX, newY - 1];
                                double ne = newY == 0 ? -1 : newX == width - 1 ? data[0, newY - 1] : data[newX + 1, newY - 1];
                                double e = newX == width - 1 ? data[0, newY] : data[newX + 1, newY];
                                double se = newY == height - 1 ? -1 : newX == width - 1 ? data[0, newY + 1] : data[newX + 1, newY + 1];
                                double s = newY == height - 1 ? -1 : data[newX, newY + 1];
                                double sw = newY == height - 1 ? -1 : newX == 0 ? data[width - 1, newY + 1] : data[newX - 1, newY + 1];
                                double w = newX == 0 ? data[width - 1, newY] : data[newX - 1, newY];

                                List<KeyValuePair<int, double>> flows = new List<KeyValuePair<int, double>>();

                                flows.Add(new KeyValuePair<int, double>(1, nw));
                                flows.Add(new KeyValuePair<int, double>(2, n));
                                flows.Add(new KeyValuePair<int, double>(3, ne));
                                flows.Add(new KeyValuePair<int, double>(4, w));
                                flows.Add(new KeyValuePair<int, double>(5, e));
                                flows.Add(new KeyValuePair<int, double>(6, sw));
                                flows.Add(new KeyValuePair<int, double>(7, s));
                                flows.Add(new KeyValuePair<int, double>(8, se));

                                // order slopes by lowest to highest
                                flows = flows.OrderBy(kvp => kvp.Value).Where(kvp => kvp.Value > 0).ToList();

                                if (flows.Count > 0)
                                {
                                    KeyValuePair<int, double> lowestHeight = flows.First();

                                    double heightPlusWater = thisHeight + waterBuildup[newX, newY];

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

        /// <summary>
        /// This is used to clear out any single pixel dangles and define the coastline areas a little clearer
        /// It helps clear up the terrain for a nicer map
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="SeaLevel"></param>
        public static void Normalize(double[,] noise, int Width, int Height, float SeaLevel)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double val = noise[x, y];

                    // get the values N,S,E,W
                    double n = y > 0 ? noise[x, y - 1] : -99.0;
                    double s = y < Height - 1 ? noise[x, y + 1] : -99.0;
                    double e = x < Width - 1 ? noise[x + 1, y] : noise[0, y]; // wrap to the other edge
                    double w = x > 0 ? noise[x - 1, y] : noise[Width - 1, y]; // wrap to the other edge

                    int waterNeighbours = 0;
                    int landNeighbours = 0;

                    if (n <= SeaLevel) waterNeighbours++;
                    else landNeighbours++;
                    if (e <= SeaLevel) waterNeighbours++;
                    else landNeighbours++;
                    if (s <= SeaLevel) waterNeighbours++;
                    else landNeighbours++;
                    if (w <= SeaLevel) waterNeighbours++;
                    else landNeighbours++;

                    // If this is a dangle (single pixel surrounded by water or land) then fill it in or sink it down
                    if (val <= SeaLevel && waterNeighbours < 2) noise[x, y] = SeaLevel + 0.05;
                    else if (val > SeaLevel && landNeighbours < 2) noise[x, y] = SeaLevel - 0.05;
                }
            }
        }

        /// <summary>
        /// Detects any low basin areas within a tolerance. These are areas that appear like
        /// big hollow lakes or blobs in the middle of larger more continental terrain. This
        /// helper method will return the points that are considered within a basin. If you
        /// set "fillBasins" to true, it will automatically collapse them to the defined 
        /// "Sea Level". keepSmallLakes will ignore smaller sections that you might want to
        /// keep as lakes
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="tolerance"></param>
        /// <param name="SeaLevel"></param>
        /// <param name="fillBasins"></param>
        /// <param name="keepSmallLakes"></param>
        /// <returns></returns>
        public static int[,] DetectBasins(double[,] noise, int Width, int Height, int tolerance, float SeaLevel, bool fillBasins, bool keepSmallLakes)
        {
            int[,] setPoints = new int[Width, Height];

            // create a temporary array for water levels and land
            // 1 = land, 2 = lake, 3 = large waterbody (ocean/sea), 4 = Unfilled lake, 0 is unassigned
            // unfilled lakes are small lakes that are underneath the tolerance for a basin fill
            // these lakes will remain as water after the basin fill process.
            // 
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // only process unassigned points
                    if (setPoints[x, y] == 0)
                    {
                        double val = noise[x, y];

                        if (val <= SeaLevel)
                        {
                            // check if neighbouring an "ocean" pixel first, before walking points
                            int n = y > 0 ? setPoints[x, y - 1] : -1;
                            int s = y < Height - 1 ? setPoints[x, y + 1] : -1;
                            int e = x < Width - 1 ? setPoints[x + 1, y] : setPoints[0, y]; // wrap to the other edge
                            int w = x > 0 ? setPoints[x - 1, y] : setPoints[Width - 1, y]; // wrap to the other edge

                            int nw = x > 0 && y > 0 ? setPoints[x - 1, y - 1] : -1;
                            int ne = x < Width - 1 && y > 0 ? setPoints[x + 1, y - 1] : -1;
                            int sw = x > 0 && y < Height - 1 ? setPoints[x - 1, y + 1] : -1;
                            int se = x < Width - 1 && y < Height - 1 ? setPoints[x + 1, y + 1] : -1;

                            // neighbour is an ocean, and this is water, so it too must be part of the ocean.
                            // otherwise, this is unassigned and next to land or a small waterbody. We need to scan the points
                            if (n == 3 || s == 3 || e == 3 || w == 3 || nw == 3 || ne == 3 || sw == 3 || se == 3) setPoints[x, y] = 3;
                            else if (n == 2 || s == 2 || e == 2 || w == 2 || nw == 2 || ne == 2 || sw == 2 || se == 2)
                            {
                                setPoints[x, y] = 2;
                            }
                            else if (n == 4 || s == 4 || e == 4 || w == 4 || nw == 4 || ne == 4 || sw == 4 || se == 4)
                            {
                                setPoints[x, y] = 4;
                            }
                            else
                            {
                                List<Point> scannedPoints = FeatureTracer.TraceEqualOrBelowValue(noise, x, y, Width, Height, SeaLevel, true);
                                foreach (Point p in scannedPoints)
                                {
                                    setPoints[p.X, p.Y] = scannedPoints.Count() <= tolerance ? 2 : 3;
                                    if (keepSmallLakes && scannedPoints.Count() < tolerance / 4) setPoints[p.X, p.Y] = 4;
                                }
                            }
                        }
                        else setPoints[x, y] = 1; // set as "land"
                    }
                }
            }

            //second pass, sw to ne this time, to clean up any missed internal areas
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double val = noise[x, y];

                    if (val <= SeaLevel)
                    {
                        int n = y > 0 ? setPoints[x, y - 1] : -1;
                        int s = y < Height - 1 ? setPoints[x, y + 1] : -1;
                        int e = x < Width - 1 ? setPoints[x + 1, y] : setPoints[0, y]; // wrap to the other edge
                        int w = x > 0 ? setPoints[x - 1, y] : setPoints[Width - 1, y]; // wrap to the other edge

                        int nw = x > 0 && y > 0 ? setPoints[x - 1, y - 1] : -1;
                        int ne = x < Width - 1 && y > 0 ? setPoints[x + 1, y - 1] : -1;
                        int sw = x > 0 && y < Height - 1 ? setPoints[x - 1, y + 1] : -1;
                        int se = x < Width - 1 && y < Height - 1 ? setPoints[x + 1, y + 1] : -1;

                        // neighbour is an ocean, and this is water, so it too must be part of the ocean.
                        // otherwise, this is unassigned and next to land or a small waterbody. We need to scan the points
                        if (n == 3 || s == 3 || e == 3 || w == 3 || nw == 3 || ne == 3 || sw == 3 || se == 3) setPoints[x, y] = 3;
                    }
                }
            }

            //final pass, there shouldn't be any corrections needed
            //but we'll ensure the ocean/lake values are set, just in case
            //at this point, we'll also fill in the basins, if required.
            for (int x = Width - 1; x >= 0; x--)
            {
                for (int y = Height - 1; y >= 0; y--)
                {
                    double val = noise[x, y];

                    if (val <= SeaLevel)
                    {
                        int n = y > 0 ? setPoints[x, y - 1] : -1;
                        int s = y < Height - 1 ? setPoints[x, y + 1] : -1;
                        int e = x < Width - 1 ? setPoints[x + 1, y] : setPoints[0, y]; // wrap to the other edge
                        int w = x > 0 ? setPoints[x - 1, y] : setPoints[Width - 1, y]; // wrap to the other edge

                        int nw = x > 0 && y > 0 ? setPoints[x - 1, y - 1] : -1;
                        int ne = x < Width - 1 && y > 0 ? setPoints[x + 1, y - 1] : -1;
                        int sw = x > 0 && y < Height - 1 ? setPoints[x - 1, y + 1] : -1;
                        int se = x < Width - 1 && y < Height - 1 ? setPoints[x + 1, y + 1] : -1;

                        // neighbour is an ocean, and this is water, so it too must be part of the ocean.
                        // otherwise, this is unassigned and next to land or a small waterbody. We need to scan the points
                        if (n == 3 || s == 3 || e == 3 || w == 3 || nw == 3 || ne == 3 || sw == 3 || se == 3) setPoints[x, y] = 3;
                        else if (setPoints[x, y] == 2 && fillBasins) noise[x, y] = SeaLevel + 0.005f;
                    }
                }
            }
            return setPoints;
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
