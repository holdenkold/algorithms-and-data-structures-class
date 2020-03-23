
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
                Dictionary<(int, int), int> mapping = new Dictionary<(int, int), int>();
                names = new (int, int)[graph.EdgesCount];
                for (int v = 0, i = 0; v < graph.VerticesCount; ++v)
                {
                    foreach (Edge e in graph.OutEdges(v))
                    {
                        if (e.To > e.From)
                        {
                            mapping[(e.From, e.To)] = i;
                            names[i] = (e.From, e.To);
                            i++;
                        }
                        else if (graph.Directed)
                        {
                            mapping[(e.To, e.From)] = i;
                            names[i] = (e.From, e.To);
                            i++;
                        }
                    }
                }
                Graph output_graph = graph.IsolatedVerticesGraph(false, graph.EdgesCount);
                for (int v = 0; v < graph.VerticesCount; ++v)
                {
                if (graph.Directed)
                {
                    foreach (Edge f in graph.OutEdges(v))
                    {
                        foreach (Edge s in graph.OutEdges(f.To))
                        {
                            int from = f.To > f.From ? mapping[(f.From, f.To)] : mapping[(f.To, f.From)];
                            int to = s.To > s.From ? mapping[(s.From, s.To)] : mapping[(s.To, s.From)];

                            output_graph.AddEdge(from, to);
                        }
                    }
                }
                else
                {
                    foreach (Edge f in graph.OutEdges(v))
                    {
                        foreach (Edge s in graph.OutEdges(v).Where(e => !e.Equals(f)))
                        {
                            int from = f.To > f.From ? mapping[(f.From, f.To)] : mapping[(f.To, f.From)];
                            int to = s.To > s.From ? mapping[(s.From, s.To)] : mapping[(s.To, s.From)];

                            output_graph.AddEdge(from, to);
                        }
                    }
                }
            }
            return output_graph;
        }

    // czesc III
    public int VertexColoring(Graph graph, out int[] colors)
        {
            if (graph.VerticesCount == 0)
            {
                colors = new int[0];
                return 0;
            }
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
            coloredGraph = Square(graph);
        return -1;
        }

    }

}
