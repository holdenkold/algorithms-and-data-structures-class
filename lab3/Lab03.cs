
namespace ASD
{
using ASD.Graphs;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Lab03 : System.MarshalByRefObject
    {

        // czesc I
        public Graph Square(Graph graph)
        {
            Graph output_graph = graph.Clone();
            HashSet<(int, int)> connected = new HashSet<(int, int)>();
            for (int v = 0; v < graph.VerticesCount; ++v)
            {
                foreach (Edge n in graph.OutEdges(v))
                {
                    foreach (Edge e in graph.OutEdges(n.To))
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
            if (graph.Directed)
                throw new ArgumentException();

            colors = Enumerable.Repeat(-1, graph.VerticesCount).ToArray();
            for (int v = 0; v < graph.VerticesCount; ++v)
            {
                if (colors[v] != -1)
                    continue;
                colors[v] = 0;
                foreach (Edge n in graph.OutEdges(v))
                {
                    var ver = n.To;
                    if (colors[ver] != -1)
                    {
                        if (colors[v] == colors[ver])
                            colors[v]++;
                    }
                }
            }
                    
            return colors.Max() + 1;
        }

    // czesc IV
    public int StrongEdgeColoring(Graph graph, out Graph coloredGraph)
        {
        coloredGraph = null;
        return -1;
        }

    }

}
