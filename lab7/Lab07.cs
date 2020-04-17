using System;
using System.Collections.Generic;

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
            exampleSolution = null;
            return true;
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
            exampleSolution = null;
            return -1;
        }

    }

}
