using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class Lab07 : MarshalByRefObject
    {
        private const int maxBuildings = 1000000;
        private const int minBuildings = 2;

        /// <summary>
        /// funkcja do sprawdzania czy da się wybudować k obiektów w odległości co najmniej dist od siebie
        /// </summary>
        /// <param name="a">posortowana tablica potencjalnych lokalizacji obiektów</param>
        /// <param name="dist">zadany dystans</param>
        /// <param name="k">liczba obiektów do wybudowania</param>
        /// <param name="exampleSolution">wybrane lokalizacje</param>
        /// <returns>true - jeśli zadanie da się zrealizować</returns>
        public bool CanPlaceBuildingsInDistance(int[] a, int dist, int k, out List<int> exampleSolution)
        {
            exampleSolution = new List<int>();
            int curr_pos = a[0];
            exampleSolution.Add(curr_pos);
            for (int i = 0; i < a.Length; i++)
            {
                if (curr_pos + dist <= a[i])
                {
                    curr_pos = a[i];
                    exampleSolution.Add(curr_pos);
                }
                if (exampleSolution.Count == k)
                    return true;
            }
            exampleSolution = null;
            return false;
        }

        /// <summary>
        /// Funkcja wybiera k lokalizacji z tablicy a, tak aby minimalny dystans
        /// pomiędzy dowolnymi dwoma lokalizacjami (spośród wybranych) był maksymalny
        /// </summary>
        /// <param name="a">posortowana tablica potencjalnych lokalizacji</param>
        /// <param name="k">liczba lokalizacji do wybrania</param>
        /// <param name="exampleSolution">wybrane lokalizacje</param>
        /// <returns>Maksymalny dystans jaki można uzyskać pomiędzy dwoma najbliższymi budynkami</returns>
        public int LargestMinDistance(int[] a, int k, out List<int> exampleSolution)
        {
            if (1 >= a.Length || a.Length > 1000000)
            {
                exampleSolution = null;
                throw new ArgumentException("Uncorrect a array");
            }

            if (a.Length < k || k < 2)
            {
                exampleSolution = null;
                throw new ArgumentException("Uncorrect k parameter");
            }


            exampleSolution = new List<int>();
            List<int> solution;
            int max_dist = int.MaxValue;//a[a.Length - 1];
            int min_dist = 0; //max_dist / k;
            int dist;


            while (min_dist < max_dist)
            {
                dist = (max_dist + min_dist) / 2;

                if (CanPlaceBuildingsInDistance(a, dist, k, out solution))
                {
                    exampleSolution = solution;
                    min_dist = dist + 1;
                }
                else
                {
                    max_dist = dist;
                }
            }
            return max_dist - 1;
        }

    }

}
