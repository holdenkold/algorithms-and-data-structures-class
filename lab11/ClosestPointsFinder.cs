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
        public (Point P1, Point P2) findClosestPoints(Point[] points)
        {
            if (points.Length == 2)
                return (points[0], points[1]);

            var sorted_points = points.OrderBy(p => p.X).ToArray();

            findClosestPointsRec(sorted_points, out (Point, Point) min_dist_points);
            return min_dist_points;
        }

        public double distnace(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public double findClosestPointsRec(Point[] points, out (Point, Point) p)
        {
            if (points.Length == 2)
            {
                p = (points[0], points[1]);
                return distnace(points[0], points[1]);
            }

            double dMin;
            int x_mid = points.Length / 2;
            int make_even = points.Length % 2 == 0 ? 0 : 1;
            double dL = findClosestPointsRec(points.Take(x_mid + make_even).ToArray(), out (Point, Point) l);
            double dR = findClosestPointsRec(points.Skip(x_mid).ToArray(), out (Point, Point) r);

            if (dL < dR)
            {
                dMin = dL;
                p = l;
            }
            else
            {
                dMin = dR;
                p = r;
            }

            var Band = points.Where(k => Math.Abs(k.X - x_mid) <= dMin);
            Point[] sortedBand = Band.OrderBy(b => b.Y).ToArray();
            for (int i = 0; i < sortedBand.Length; i++)
            {
                for (int j = i + 1; j < sortedBand.Length; j++)
                {
                    double difference = Math.Abs(sortedBand[i].Y - sortedBand[j].Y);
                    if (difference >= dMin)
                        continue;
                    else if(distnace(sortedBand[i], sortedBand[j]) < dMin)
                    {
                        p = (sortedBand[i], sortedBand[j]);
                        dMin = distnace(sortedBand[i], sortedBand[j]);
                    }
                }
            }

            return dMin;
        }
    }
}
