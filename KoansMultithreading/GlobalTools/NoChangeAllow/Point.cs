using System;

namespace KoansMultithreading.GlobalTools.NoChangeAllow
{
    public struct Point
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point(double[] coords)
        {
            if (coords.Length != 2)
            {
                throw new BadArgumentException("Initalizing a point require with array require an array of two double values.");
            }
            X = coords[0];
            Y = coords[1];
        }

        public double SquareNorm() => Math.Sqrt(X * X + Y * Y);
    }
}