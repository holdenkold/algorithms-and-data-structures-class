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
            int right = a[a.Length - 1];
            int left = a[0];
            int mid;
            int maxDist = 0;
            List<int> solution;
            while (left <= right)
            {
                mid = (right + left) / 2;

                if (CanPlaceBuildingsInDistance(a, mid, k, out solution))
                {
                    if (mid > maxDist)
                    {
                        exampleSolution = solution;
                        maxDist = mid;
                    }
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }
            if (maxDist == 0)
            {
                for (int i = 0; i < k; i++)
                    exampleSolution.Add(a[0]);
            }
            return maxDist;
        }

    }

}
