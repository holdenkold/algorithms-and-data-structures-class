using System;
using System.Collections.Generic;
using System.Linq;
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
            //Dictionary<int, Edge> map = new Dictionary<int, Edge>();
            int source = flow_graph.VerticesCount - 2;
            int target = flow_graph.VerticesCount - 1;

            for (int i = 0; i < g.VerticesCount; i++)
            {
                Edge e = new Edge(i, i + g.VerticesCount, levels[i]);
                flow_graph.AddEdge(e);
                //map.Add(i, e);
                // Console.WriteLine(e.From.ToString() + "-" + e.To.ToString());
            }

            for (int i = 0; i < g.VerticesCount; i++)
            {
                foreach (var e in g.OutEdges(i))
                {
                    //var from = map[i].To;
                    //var to = map[e.To].From;
                    var from = i + g.VerticesCount;
                    var to = e.To;
                    flow_graph.AddEdge(from, to, int.MaxValue);
                    //Console.WriteLine("i: "+ i.ToString() +", " + from.ToString() + "-" + to.ToString());
                }
            }

            foreach (var s in entrances)
                flow_graph.AddEdge(source, s, int.MaxValue);

            foreach (var t in exits)
                flow_graph.AddEdge(t + g.VerticesCount, target, int.MaxValue);

            Graph flow;
            double max_flow = int.MinValue;

            (max_flow, flow) = flow_graph.FordFulkersonDinicMaxFlow(source, target, MaxFlowGraphExtender.BFPath, true);

            routes = null;
            //int routes_count = 0;
            //routes = new int[][];
            //foreach (var s in entrances)
            //{
            //    Stack<int> to_see = new Stack<int>();
            //    to_see.Push(s);

            //    while (to_see.Count > 0)
            //    {
            //        var v = to_see.Pop();
            //        foreach (var e in flow.OutEdges(v).Where(e => e.Weight > 0))
            //        {
            //            routes_count += 1;
            //            to_see.Push(e.To);
            //        }
            //    }
            //}
            return (int)max_flow;
        }

    }

}

