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
            od.solveLargestS(0, od.edgeSet, new List<Edge>(), 0, 0);
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
            public void solveLargestS(int idx, Edge[] available, List<Edge> placements, double placementWeight, int currentK)
            {

                if (idx == available.Length)
                {
                    if (placements.Count > solution.Count || (placements.Count == solution.Count && placementWeight < solutionWeight))
                    {
                        solutionWeight = placementWeight;
                        solution = new List<Edge>(placements);
                    }
                    return;
                }

                if (available.Length - idx < solution.Count - placements.Count)
                    return;


                int p = assign(available[idx], placements);
                currentK += p;
                if (isValid(currentK))
                {
                    solveLargestS(idx + 1, available, placements, placementWeight + available[idx].Weight, currentK);
                }
                unassign(available[idx], placements);
                currentK -= p;
                solveLargestS(idx + 1, available, placements, placementWeight, currentK);

            }

            private int assign(Edge e, List<Edge> placements)
            {
                int x = 0;
                assigned[e.To] += 1;
                if (assigned[e.To] > 1)
                    x++;
                assigned[e.From] += 1;
                if (assigned[e.From] > 1)
                    x++;
                placements.Add(e);
                return x;
            }
            private void unassign(Edge e, List<Edge> placements)
            {
                assigned[e.To] -= 1;
                assigned[e.From] -= 1;
                placements.RemoveAt(placements.Count - 1);
            }
            private bool isValid(int currentK)
            {
                return currentK <= k;
            }
        }
    }
}

