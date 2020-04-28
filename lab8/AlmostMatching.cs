using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace lab08
{
public class AlmostMatching : MarshalByRefObject
    {

        /// <summary>
        /// Metoda zwraca najliczniejszy możliwy zbiór krawędzi, którego poziom ryzyka nie przekracza limitu.
        /// W przypadku istnenia kilku takich zbiorów zwraca zbiór o najmniejszej sumie wag ze wszystkich najliczniejszych.
        /// </summary>
        /// <returns>Liczba i lista linek (krawędzi)</returns>
        /// <param name="g">Graf linek</param>
        /// <param name="allowedCollisions">Limit ryzyka</param>
        public (int edgesCount, List<Edge> solution) LargestS(Graph g, int allowedCollisions)
        {
            if (g.VerticesCount == 0)
                return (0, null);

            OptimiseDryer od = new OptimiseDryer(g, allowedCollisions);
            od.solveLargestS(0, od.edgeSet, new List<Edge>(), 0);
            return (od.solution.Count, od.solution);
        }

        class OptimiseDryer
        {
            private Graph graph;
            private int[] assigned;
            private int k;
            public List<Edge> solution;
            private double solutionWeight;
            public Edge[] edgeSet;
            public OptimiseDryer(Graph g, int k)
            {
                graph = g;
                assigned = new int[g.VerticesCount];
                this.k = k;
                solution = new List<Edge>();
                edgeSet = new Edge[graph.EdgesCount];
                solutionWeight = int.MaxValue;
                createEdgeSet();
            }

            private void createEdgeSet()
            {
                int count = 0;
                for (int i = 0; i < graph.VerticesCount; i++)
                {
                    foreach (Edge e in graph.OutEdges(i).Where(e => e.To > e.From))
                    {
                        edgeSet[count++] = e;
                    }
                }
            }
            public void solveLargestS(int idx, Edge[] available, List<Edge> placements, double placementWeight)
            {
                if (solution.Count >= placements.Count && placementWeight >= solutionWeight)
                    return;

                if (idx == available.Length)
                {
                    if (placementWeight < solutionWeight)
                    {
                        solutionWeight = placementWeight;
                        solution = new List<Edge>(placements);
                        //solutionWeight = placementWeight;
                        //solution = placements;
                    }
                    return;
                }

                //if (available.Length + placementWeight - idx < solution.Count)
                //    return;

                assign(available[idx], placements);
                if (isValid(available[idx]))
                {
                    solveLargestS(idx + 1, available, placements, placementWeight + available[idx].Weight);
                }
                unassign(available[idx], placements);
                solveLargestS(idx + 1, available, placements, placementWeight + available[idx].Weight);
            }

            private void assign(Edge e, List<Edge> placements)
            {
                assigned[e.To]+= 1;
                assigned[e.From] += 1;
                placements.Add(e);
            }
            private void unassign(Edge e, List<Edge> placements)
            {
                assigned[e.To] -= 1;
                assigned[e.From] -= 1;
                placements.RemoveAt(placements.Count - 1);
            }
            private bool isValid(Edge e) => assigned[e.To] <= k + 1 && assigned[e.From] <= k + 1;
        }
    }
}

