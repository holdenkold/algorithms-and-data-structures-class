
using ASD.Graphs;
using System.Collections.Generic;
using System.Linq;

namespace Lab05
{

public class PathFinder : System.MarshalByRefObject
    {
        public List<Edge> CalculateMinPath(int start, int end, PathsInfo[] paths)
        {
            List<Edge> path = new List<Edge>();
            int p = 0;
            while (paths[p].Last.HasValue)
            {
                Edge reversed = paths[p].Last.Value;
                path.Add(new Edge(reversed.To, reversed.From, reversed.Weight));
                p = paths[p].Last.Value.From;
                if (p == end)
                    break;
            }
            return path;
        }

        public Graph ReverseGraph(Graph g)
        {
            Graph graph = g.IsolatedVerticesGraph();
            for (int i = 0; i < g.VerticesCount; i++)
            {
                foreach (var e in g.OutEdges(i))
                {
                    graph.AddEdge(e.To, e.From, e.Weight);
                }
            }
            return graph;
        }

        public PathsInfo[] ChceckDijkstraPath(Graph g, Edge to_delete, int v)
        {
            g.DelEdge(to_delete);
            g.DijkstraShortestPaths(v, out PathsInfo[] paths);
            g.AddEdge(to_delete);
            return paths;
        }

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
            Graph graph = g;
            path = null;
            if (g.Directed)
            {
                graph = ReverseGraph(g); 
            }
            graph.DijkstraShortestPaths(b, out PathsInfo[] paths);

            if (double.IsNaN(paths[a].Dist))
                return null;

            List<Edge> min_path = CalculateMinPath(a, b, paths);

            double min_path_len = paths[b].Dist;
            double second_min_path_len = double.MaxValue;
            List<Edge> second_min_path = new List<Edge>();
            List<Edge> second = new List<Edge>();

            double curr_len = 0;

            foreach (var edge in min_path)
            {
                foreach (var e in g.OutEdges(edge.From).Where(x => x != edge))
                {
                    if (curr_len + e.Weight + paths[e.To].Dist < second_min_path_len)
                    {
                        second_min_path_len = curr_len + e.Weight + paths[e.To].Dist;
                        second = new List<Edge>();
                        foreach (var el in second_min_path)
                            second.Add(el);
                        second.Add(e);
                        int v = e.To;
                        while (paths[v].Last.HasValue)
                        {
                            Edge reversed = paths[v].Last.Value;
                            second.Add(new Edge(reversed.To, reversed.From, reversed.Weight));
                            v = paths[v].Last.Value.From;
                        }
                        if (second_min_path_len == min_path_len)
                        {
                            path = second.ToArray();
                            return second_min_path_len;
                        }

                    }
                }
                curr_len += edge.Weight;
                second_min_path.Add(edge);
            }
         
            System.Console.WriteLine("Path len is:" + second_min_path_len.ToString());
            if (second.Count != 0)
            {
                path = second.ToArray();
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
            Graph graph = g.Clone();
            path = null;
            if (g.Directed)
            {
                graph = ReverseGraph(g);
            }
            graph.DijkstraShortestPaths(b, out PathsInfo[] paths);

            if (double.IsNaN(paths[a].Dist))
                return null;

            List<Edge> min_path = CalculateMinPath(a, b, paths);

            double min_path_len = paths[b].Dist;
            double second_min_path_len = double.MaxValue;
            List<Edge> second_min_path = new List<Edge>();
            List<Edge> second = new List<Edge>();

            double curr_len = 0;

            foreach (var edge in min_path)
            {
                paths = ChceckDijkstraPath(graph, new Edge(edge.To, edge.From, edge.Weight), b); //e.from
                if (paths[edge.From].Dist + curr_len < second_min_path_len)
                {
                    second_min_path_len = paths[edge.From].Dist + curr_len;

                    second = new List<Edge>();
                    foreach (var el in second_min_path)
                        second.Add(el);

                    int v = edge.From;
                    while (paths[v].Last.HasValue)
                    {
                        Edge reversed = paths[v].Last.Value;
                        second.Add(new Edge(reversed.To, reversed.From, reversed.Weight));
                        v = paths[v].Last.Value.From;
                    }
                    if (second_min_path_len == min_path_len)
                    {
                        path = second.ToArray();
                        return second_min_path_len;
                    }

                }
                curr_len += edge.Weight;
                second_min_path.Add(edge);
            }

            System.Console.WriteLine("Path len is:" + second_min_path_len.ToString());
            if (second.Count != 0)
            {
                path = second.ToArray();
                return second_min_path_len;
            }
            else
                return null;
        }

    }
}
