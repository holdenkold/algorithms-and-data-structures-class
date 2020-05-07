using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ASD.Graphs;

namespace Lab09
{

    public class Museum : MarshalByRefObject
    {

        /// <summary>
        /// Znajduje najliczniejszy multizbiór tras
        /// </summary>
        /// <returns>Liczność wyznaczonego multizbioru tras (liczba ścieżek w tym multizbiorze)</returns>
        /// <param name="g">Graf opisujący muzeum</param>
        /// <param name="levels">Tablica o długości równej liczbie wierzchołków w grafie -- poziomy ciekawości wystaw</param>
        /// <param name="entrances">Wejścia</param>
        /// <param name="exits">Wyjścia</param>
        /// <param name="routes">Ścieżki należące do wyznaczonego multizbioru</param>
        /// <remarks>
        /// Parametr routes to rablica tablic.
        /// Rozmiar zewnętrznej tablicy musi być równy liczbie ścieżek w multizbiorze.
        /// Każda wewnętrzna tablica opisuje jedną ścieżkę, a ściślej wymienia sale przez, które ona przechodzi.
        /// Kolejność sal jest istotna.
        /// </remarks>
        public int FindRoutes(Graph g, int[] levels, int[] entrances, int[] exits, out int[][] routes)
        {
            Graph flow_graph = g.IsolatedVerticesGraph(true, g.VerticesCount * 2 + 2);
            int source = flow_graph.VerticesCount - 2;
            int target = flow_graph.VerticesCount - 1;

            for (int i = 0; i < g.VerticesCount; i++)
            {
                Edge e = new Edge(i, i + g.VerticesCount, levels[i]);
                flow_graph.AddEdge(e);

                foreach (var edge in g.OutEdges(i))
                {
                    var from = i + g.VerticesCount;
                    var to = edge.To;
                    flow_graph.AddEdge(from, to, int.MaxValue);
                }
            }

            foreach (var s in entrances)
                flow_graph.AddEdge(source, s, int.MaxValue);

            foreach (var t in exits)
                flow_graph.AddEdge(t + g.VerticesCount, target, int.MaxValue);

            Graph flow;
            double max_flow = int.MinValue;

            (max_flow, flow) = flow_graph.FordFulkersonDinicMaxFlow(source, target, MaxFlowGraphExtender.OriginalDinicBlockingFlow, false);
            List<int[]> paths = new List<int[]>();
            int idx = 0;
            foreach (var s in entrances)
            {
                findFlows(s, paths, exits, flow, new List<int> { s }, g.VerticesCount, idx, (int)flow.GetEdgeWeight(s, s + g.VerticesCount));
            }
            routes = paths.ToArray();
            //Console.WriteLine(routes.Length.ToString() + " " + max_flow.ToString());
            return (int)max_flow;
        }
        private void findFlows(int s, List<int[]> paths, int[] targets, Graph flow, List<int> path, int map, int idx, int times)
        {
            if (targets.Contains(s))
            {
                var w = Math.Min(flow.GetEdgeWeight(s, s + map), times);
                for (int i = 0; i < w; i++)
                {
                    paths.Add(path.ToArray());
                    foreach (var el in path)
                    {
                        flow.ModifyEdgeWeight(el, el + map, -1);
                    }
                }
                return;
            }

            foreach (var e in flow.OutEdges(s + map).Where(e => e.Weight > 0))
            {
                path.Add(e.To);
                findFlows(e.To, paths, targets, flow, path, map, idx, Math.Min((int)e.Weight, times));
                idx += 1;
                path.RemoveAt(path.Count - 1);
            }
        }

    }

}