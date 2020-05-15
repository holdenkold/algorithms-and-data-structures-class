using System;
using System.ComponentModel.Design;
using System.Linq;

namespace ASD
{
    [Serializable]
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point(double x, double y) { X = x; Y = y; }
        public override string ToString() => $"({X};{Y})";
    }

    public class ClosestPointsFinder : MarshalByRefObject
    {
        /// <summary>
        /// Szukanie najbliższej pary punktów (Złożoność O(n*logn)).
        /// </summary>
        /// <param name="points">
        /// Tablica unikatowych punktów wśród, który wyznaczana jest najbliższa para punktów.
        /// </param>
        /// <returns>
        /// Znaleziona najbliżej położona para punktów. Jeżeli jest kilka należy zwrócić dowolną.
        /// </returns>
        public (Point p1, Point p2) findClosestPoints(Point[] points)
        {
            if (points.Length == 2)
                return (points[0], points[1]);

            var sorted_points = points.OrderBy(p => p.X).ToArray();
            (Point p1, Point p2, double dist) min_dist_points = findClosestPointsRec(sorted_points);
            return (min_dist_points.p1, min_dist_points.p2);
        }

        public double distnace(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public (Point p1, Point p2, double dist) findClosestPointsRec(Point[] points)
        {
            if (points.Length == 2)
                return (points[0], points[1], distnace(points[0], points[1]));

            double x_mid;
            int mid = points.Length / 2;
            int make_even;
            if (points.Length % 2 == 0)
            {
                make_even = 0;
                x_mid = (points[points.Length / 2].X + points[points.Length / 2 - 1].X) / 2;
            }
            else
            {
                make_even = 1;
                x_mid = points[(points.Length - 1) / 2].X;
            }

            var dL = findClosestPointsRec(points.Take(mid + make_even).ToArray());
            var dR = findClosestPointsRec(points.Skip(mid).ToArray());

            (Point p1, Point p2, double dist) dMin = dL.dist < dR.dist ? dL : dR;

            var Band = points.Where(k => Math.Abs(k.X - x_mid) <= dMin.dist);

            Point[] sortedBand = Band.OrderBy(b => b.Y).ToArray();
            for (int i = 0; i < sortedBand.Length - 1; i++)
            {
                for (int j = i + 1; j < sortedBand.Length; j++)
                {
                    double difference = Math.Abs(sortedBand[i].Y - sortedBand[j].Y);

                    if (difference < dMin.dist && distnace(sortedBand[i], sortedBand[j]) < dMin.dist)
                        dMin = (sortedBand[i], sortedBand[j], distnace(sortedBand[i], sortedBand[j]));
                }
            }
            return dMin;
        }
    }
}
