using System;
using System.Collections.Generic;
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
            return (-1, null);
            }

        // można dodawać pomocnicze klasy i metody (prywatne!)

    }

}

