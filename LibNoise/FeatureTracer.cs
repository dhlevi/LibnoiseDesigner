using System;
using System.Collections.Generic;

namespace LibNoise
{
    /// <summary>
    /// Traces features from noise based on neighbouring values. Taken from https://github.com/dhlevi/JavaLibNoise/blob/master/src/main/java/ca/dhlevi/libnoise/FeatureTracer.java
    /// Pretty gnarly implementation, and should be revisted and reworked.
    /// </summary>
    public class FeatureTracer
    {
        private enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };
        public static List<Point> TraceEqualOrBelowValue(double[,] noise, int x, int y, int width, int height, float value, bool wrap)
        {
            List<Point> points = new List<Point>();

            try
            {
                bool traceComplete = false;
                bool firstRun = true;
                Point firstPoint = new Point(x, y);
                Point lastPoint = firstPoint;
                Direction lastDirection = Direction.East;

                while (!traceComplete)
                {
                    if (!firstRun && lastPoint.X == x && lastPoint.Y == y)
                    {
                        traceComplete = true;
                    }
                    else
                    {
                        if (firstRun) firstRun = false;
                        points.Add(lastPoint);
                        switch (lastDirection)
                        {
                            case Direction.NorthWest:
                                // check SW W NW N NE E SE S
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else traceComplete = true;
                                break;
                            case Direction.North:
                                // check W NW N NE E SE S SW
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else traceComplete = true;
                                break;
                            case Direction.NorthEast:
                                // check NW N NE E SE S SW W
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else traceComplete = true;
                                break;
                            case Direction.East:
                                // check N NE E SE S SW W NW
                                // first hit that is <= value is the direction to move
                                if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else traceComplete = true;
                                break;
                            case Direction.SouthEast:
                                // check NE E SE S SW W NW N
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else traceComplete = true;
                                break;
                            case Direction.South:
                                // check E SE S SW W NW N NE
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else traceComplete = true;
                                break;
                            case Direction.SouthWest:
                                // check SE S SW W NW N NE E
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else traceComplete = true;
                                break;
                            case Direction.West:
                                // check S SW W NW N NE E SE
                                // first hit that is <= value is the direction to move
                                if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] <= value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] <= value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] <= value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] <= value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] <= value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else traceComplete = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                points = new List<Point>();
            }

            return points;
        }

        public static List<Point> TraceEqualValue(double[,] noise, int x, int y, int width, int height, float value, bool wrap)
        {
            bool traceAngles = false;

            List<Point> points = new List<Point>();

            try
            {
                bool traceComplete = false;
                bool firstRun = true;
                Point firstPoint = new Point(x, y);
                Point lastPoint = firstPoint;
                Direction lastDirection = Direction.East;

                while (!traceComplete)
                {
                    if (!firstRun && lastPoint.X == x && lastPoint.Y == y)
                    {
                        traceComplete = true;
                    }
                    else
                    {
                        if (firstRun) firstRun = false;
                        points.Add(lastPoint);
                        switch (lastDirection)
                        {
                            case Direction.NorthWest:
                                // check SW W NW N NE E SE S
                                // first hit that is <= value is the direction to move
                                if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else traceComplete = true;
                                break;
                            case Direction.North:
                                // check W NW N NE E SE S SW
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else traceComplete = true;
                                break;
                            case Direction.NorthEast:
                                // check NW N NE E SE S SW W
                                // first hit that is <= value is the direction to move
                                if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else traceComplete = true;
                                break;
                            case Direction.East:
                                // check N NE E SE S SW W NW
                                // first hit that is <= value is the direction to move
                                if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else traceComplete = true;
                                break;
                            case Direction.SouthEast:
                                // check NE E SE S SW W NW N
                                // first hit that is <= value is the direction to move
                                if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else traceComplete = true;
                                break;
                            case Direction.South:
                                // check E SE S SW W NW N NE
                                // first hit that is <= value is the direction to move
                                if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else traceComplete = true;
                                break;
                            case Direction.SouthWest:
                                // check SE S SW W NW N NE E
                                // first hit that is <= value is the direction to move
                                if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else traceComplete = true;
                                break;
                            case Direction.West:
                                // check S SW W NW N NE E SE
                                // first hit that is <= value is the direction to move
                                if (lastPoint.Y < height - 1 && noise[lastPoint.X, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y + 1); lastDirection = Direction.South; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y < height - 1 && noise[lastPoint.X - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y < height - 1 && noise[width - 1, lastPoint.Y + 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y + 1); lastDirection = Direction.SouthWest; }
                                else if (lastPoint.X > 0 && noise[lastPoint.X - 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (wrap && lastPoint.X == 0 && noise[width - 1, lastPoint.Y] == value) { lastPoint = new Point(width - 1, lastPoint.Y); lastDirection = Direction.West; }
                                else if (traceAngles && lastPoint.X > 0 && lastPoint.Y > 0 && noise[lastPoint.X - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (traceAngles && wrap && lastPoint.X == 0 && lastPoint.Y > 0 && noise[width - 1, lastPoint.Y - 1] == value) { lastPoint = new Point(width - 1, lastPoint.Y - 1); lastDirection = Direction.NorthWest; }
                                else if (lastPoint.Y > 0 && noise[lastPoint.X, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X, lastPoint.Y - 1); lastDirection = Direction.North; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y > 0 && noise[lastPoint.X + 1, lastPoint.Y - 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y > 0 && noise[0, lastPoint.Y - 1] == value) { lastPoint = new Point(0, lastPoint.Y - 1); lastDirection = Direction.NorthEast; }
                                else if (lastPoint.X < width - 1 && noise[lastPoint.X + 1, lastPoint.Y] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y); lastDirection = Direction.East; }
                                else if (wrap && lastPoint.X == width - 1 && noise[0, lastPoint.Y] == value) { lastPoint = new Point(0, lastPoint.Y); lastDirection = Direction.East; }
                                else if (traceAngles && lastPoint.X < width - 1 && lastPoint.Y < height - 1 && noise[lastPoint.X + 1, lastPoint.Y + 1] == value) { lastPoint = new Point(lastPoint.X + 1, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else if (traceAngles && wrap && lastPoint.X == width - 1 && lastPoint.Y < height - 1 && noise[0, lastPoint.Y + 1] == value) { lastPoint = new Point(0, lastPoint.Y + 1); lastDirection = Direction.SouthEast; }
                                else traceComplete = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                points = new List<Point>();
            }

            return points;
        }
    }
}
