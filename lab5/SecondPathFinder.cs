
using ASD.Graphs;
using System.Collections.Generic;
using System.Linq;

namespace Lab05
{

public class PathFinder : System.MarshalByRefObject
    {
        public List<int> CalculatePath(int start, int end, PathsInfo[] paths)
        {
            List<int> path = new List<int> { end };
            int p = end;
            while (paths[p].Last.HasValue)
            {
                p = paths[p].Last.Value.From;
                path.Insert(0, p);
                if (p == start)
                    break;
            }
            return path;
        }

        public List<int> CalculateFullPath(int a, Edge medium, int b, PathsInfo[] paths1, PathsInfo[] paths2)
        {
            List<int> path = CalculatePath(a, medium.From, paths1);
            path.Add(medium.To);
            var end = CalculatePath(medium.To, b, paths2);
            path.AddRange(end);
            return path;
        }

        public Edge[] GenerateEdgePath(Graph g, List<int> int_path)
        {
            Edge[] path = new Edge[int_path.Count - 1];
            for (int i = 1; i < int_path.Count; i++)
            {
                var from = int_path[i - 1];
                var to = int_path[i];
                path[i - 1] = new Edge(from, to, g.GetEdgeWeight(from, to));
            }
            return path;
        }

        //public List<int> CalculatePath(int start, int end, PathsInfo[] paths)
        //{
        //    List<int> path = new List<int> { end };
        //    int p = end;
        //    while (paths[p].Last.HasValue)
        //    {
        //        p = paths[p].Last.Value.From;
        //        path.Insert(0, p);
        //        if (p == start)
        //            break;
        //    }
        //    return path;
        //}

        //public List<int> CalculateFullPath(int a, Edge medium, int b, PathsInfo[] paths1, PathsInfo[] paths2)
        //{
        //    List<int> path = CalculatePath(a, medium.From, paths1);
        //    path.Add(medium.To);
        //    var end = CalculatePath(medium.To, b, paths2);
        //    path.AddRange(end);
        //    return path;
        //}

        //public Edge[] GenerateEdgePath(Graph g, List<int> int_path)
        //{
        //    Edge[] path = new Edge[int_path.Count -1];
        //    for (int i = 1; i < int_path.Count; i++)
        //    {
        //        var from = int_path[i - 1];
        //        var to = int_path[i];
        //        path[i - 1] = new Edge(from, to, g.GetEdgeWeight(from, to));
        //    }
        //    return path;
        //}

        /// <summary>
        /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
        /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki, algorytm zwróci jedną z nich).
        /// Dopuszczamy, aby na ścieżce powtarzały się wierzchołki i/lub krawędzie.
        /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
        /// </summary>
        /// <remarks>
        /// Wymagana złożoność do O(D), gdzie D jest złożonością implementacji alogorytmu Dijkstry w bibliotece Graph.
        /// </remarks>
        /// <param name="g">badany graf</param>
        /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako tablica krawędzi</param>
        /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość znalezionej ścieżki</returns>
        public double? FindSecondShortestPath(Graph g, int a, int b, out Edge[] path)
        {
            path = null;
            g.DijkstraShortestPaths(a, out PathsInfo[] paths);

            if (double.IsNaN(paths[b].Dist))
                return null;

            List<int> min_path = CalculatePath(a, b, paths);

            double min_path_len = paths1[b].Dist;
            double second_min_path_len = double.MaxValue;
            List<int> second_min_path = new List<int>();
            for (int v = 0; v < g.VerticesCount; v++)
            {
                if (v == b)
                    continue;

                foreach (Edge e in g.OutEdges(v))
                {
                    var dist = paths1[v].Dist + e.Weight + paths2[e.To].Dist;
                    if (dist >= min_path_len && dist < second_min_path_len)
                    {
                        var candidate = CalculateFullPath(a, e, b, paths1, paths2);
                        if (dist == min_path_len)
                        {
                            if (!Enumerable.SequenceEqual(second_min_path, min_path))
                            {
                                path = GenerateEdgePath(g, candidate);
                                return min_path_len;
                            }
                        }
                        else
                        {
                            second_min_path_len = dist;
                            second_min_path = candidate;
                        }
                    }
                }

            }

            System.Console.WriteLine("Path len is:" + second_min_path_len.ToString());
            if (second_min_path.Count != 0)
            {
                path = GenerateEdgePath(g, second_min_path);
                return second_min_path_len;
            }
            else
                return null;
        }
        public double? FindSecondShortestPath1(Graph g, int a, int b, out Edge[] path)
        {
            path = null;
            g.DijkstraShortestPaths(a, out PathsInfo[] paths1);
            g.DijkstraShortestPaths(b, out PathsInfo[] paths2);

            if (double.IsNaN(paths1[b].Dist))
                return null;

            List<int> min_path = CalculatePath(a, b, paths1);

            double min_path_len = paths1[b].Dist;
            double second_min_path_len = double.MaxValue;
            List<int> second_min_path = new List<int>();
            for(int v = 0; v < g.VerticesCount; v++)
            {
                if (v == b)
                    continue;

                foreach (Edge e in g.OutEdges(v))
                {
                    var dist = paths1[v].Dist + e.Weight + paths2[e.To].Dist;
                    if (dist >= min_path_len && dist < second_min_path_len)
                    {
                        var candidate = CalculateFullPath(a, e, b, paths1, paths2);
                        if (dist == min_path_len)
                        {
                            if (!Enumerable.SequenceEqual(second_min_path, min_path))
                            {
                                path = GenerateEdgePath(g, candidate);
                                return min_path_len;
                            }
                        }
                        else
                        {
                            second_min_path_len = dist;
                            second_min_path = candidate;
                        }
                    }
                }
                
            }

            System.Console.WriteLine("Path len is:" + second_min_path_len.ToString());
            if (second_min_path.Count != 0)
            {
                path = GenerateEdgePath(g, second_min_path);
                return second_min_path_len;
            }
            else
                return null;
        }

    /// <summary>
    /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
    /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki, algorytm zwróci jedną z nich).
    /// Wymagamy, aby na ścieżce nie było powtórzeń wierzchołków ani krawędzi.
    /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
    /// </summary>
    /// <remarks>
    /// Wymagana złożoność to O(nD), gdzie D jest złożonością implementacji algorytmu Dijkstry w bibliotece Graph.
    /// </remarks>
    /// <param name="g">badany graf</param>
    /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako tablica krawędzi</param>
    /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość tej ścieżki</returns>
    public double? FindSecondSimpleShortestPath(Graph g, int a, int b, out Edge[] path)
        {
        path = null;
        return -1.0;
        }

    }

}
