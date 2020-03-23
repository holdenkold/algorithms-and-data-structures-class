
namespace ASD
{
using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    public class Lab03 : System.MarshalByRefObject
    {

        // czesc I
        public Graph Square(Graph graph)
        {
            Graph output_graph = graph.Clone();
            HashSet<(int, int)> connected = new HashSet<(int, int)>();
            for (int v = 0; v < graph.VerticesCount; ++v)
            {
                var out_vers = graph.OutEdges(v);
                foreach (Edge n in out_vers)
                {
                    var out_vers2 = graph.OutEdges(n.To);
                    foreach (Edge e in out_vers2)
                    {
                        if (e.To == v)
                            continue;

                        if (connected.Contains((v, e.To)) || connected.Contains((v, e.To)))
                            continue;
                        else
                        {
                            output_graph.AddEdge(v, e.To);
                            connected.Add((v, e.To));
                        }
                    }
                }
            }
            return output_graph;
        }

    // czesc II
    public Graph LineGraph(Graph graph, out (int x, int y)[] names)
        {
        names = null;
        return null;
        }

    // czesc III
    public int VertexColoring(Graph graph, out int[] colors)
        {
        colors = null;
        return -1;
        }

    // czesc IV
    public int StrongEdgeColoring(Graph graph, out Graph coloredGraph)
        {
        coloredGraph = null;
        return -1;
        }

    }

}
