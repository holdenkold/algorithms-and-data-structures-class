using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab06
{

    public class SmellsChecker : MarshalByRefObject
    {
        private bool solveAssignSmells(int[] satisfactions, int[][] customerPreferences, int satisfactionLevel, bool[] smells, int used)
        {
            for (int i = 0; i < customerPreferences.Length; i++)
            {
                if (satisfactions[i] + customerPreferences[i].Count(e => e == 1) < satisfactionLevel)
                    return false;
            }

            if (satisfactions.All(el => el >= satisfactionLevel))
                return true;


            for (int idx = used; idx < smells.Length; idx++)
            {
                smells[idx] = true;
                for (int i = 0; i < customerPreferences.Length; i++)
                {
                    var add = customerPreferences[i][idx];
                    satisfactions[i] += customerPreferences[i][idx];
                }

                if (satisfactions.All(el => el + smells.Length - 1 - used >= satisfactionLevel))
                    if (solveAssignSmells(satisfactions, customerPreferences, satisfactionLevel, smells, idx + 1))
                        return true;

                smells[idx] = false;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] -= customerPreferences[i][idx];

            }
            return false;
        }


        private int maxsolveAssignSmells_old(int[] satisfactions, int[][] customerPreferences, int satisfactionLevel, bool[] smells, int used)
        {
            int max_client = satisfactions.Count(p => p >= satisfactionLevel);
            if (max_client == customerPreferences.Length)
                return max_client;

            for (int idx = used; idx < smells.Length; idx++)
            {
                smells[idx] = true;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] += customerPreferences[i][idx];

                int curr_max = maxsolveAssignSmells_old(satisfactions, customerPreferences, satisfactionLevel, smells, idx + 1);
                if (curr_max > max_client)
                {
                    max_client = curr_max;
                }

                if (max_client == customerPreferences.Length)
                    return max_client;

                smells[idx] = false;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] -= customerPreferences[i][idx];
            }
            return max_client;
        }

        //private (int, HashSet<int>) maxsolveAssignSmells(int smellCount, int[] satisfactions, int[][] customerPreferences, int satisfactionLevel, int used)
        //{
        //    HashSet<int> smells = new HashSet<int>();
        //    if (used < smellCount)
        //        smells.Add(used);

        //    int max_client = satisfactions.Count(p => p >= satisfactionLevel);
        //    if (max_client == customerPreferences.Length)
        //        return (max_client, smells);

        //    for (int idx = used; idx < smellCount; idx++)
        //    {
        //        for (int i = 0; i < customerPreferences.Length; i++)
        //            satisfactions[i] += customerPreferences[i][idx];

        //        var (currmax, maxsmell) = maxsolveAssignSmells(smellCount, satisfactions, customerPreferences, satisfactionLevel, idx + 1);
        //        if (currmax > max_client)
        //        {
        //            foreach (var el in maxsmell)
        //            {
        //                smells.Add(el);
        //            }
        //            max_client = currmax;
        //        }

        //        for (int i = 0; i < customerPreferences.Length; i++)
        //            satisfactions[i] -= customerPreferences[i][idx];
        //    }
        //    return (max_client, smells);
        //}


        /// <summary>
        /// Implementacja etapu 1
        /// </summary>
        /// <returns><c>true</c>, jeśli przypisanie jest możliwe <c>false</c> w p.p.</returns>
        /// <param name="smellCount">Liczba zapachów, którymi dysponuje sklep</param>
        /// <param name="customerPreferences">Preferencje klientów
        /// Każda tablica -- element tablicy tablic -- to preferencje jednego klienta.
        /// Preferencje klienta mają długość smellCount, na i-tej pozycji jest
        ///  1 -- klient preferuje zapach
        ///  0 -- zapach neutralny
        /// -1 -- klient nie lubi zapachu
        ///
        /// Zapachy numerujemy od 0
        /// </param>
        /// <param name="satisfactionLevel">Oczekiwany poziom satysfakcji</param>
        /// <param name="smells">Wyjściowa tablica rozpylonych zapachów realizująca rozwiązanie, jeśli się da. null w p.p. </param>
        public bool AssignSmells(int smellCount, int[][] customerPreferences, int satisfactionLevel, out bool[] smells)
        {
            var satisfactions = new int[customerPreferences.Length];
            smells = new bool[smellCount];

            bool can_assign = solveAssignSmells(satisfactions, customerPreferences, satisfactionLevel, smells, 0);
            if (!can_assign)
                smells = null;
            return can_assign;
        }


        /// <summary>
        /// Implementacja etapu 2
        /// </summary>
        /// <returns>Maksymalna liczba klientów, których można usatysfakcjonować</returns>
        /// <param name="smellCount">Liczba zapachów, którymi dysponuje sklep</param>
        /// <param name="customerPreferences">Preferencje klientów
        /// Każda tablica -- element tablicy tablic -- to preferencje jednego klienta.
        /// Preferencje klienta mają długość smellCount, na i-tej pozycji jest
        ///  1 -- klient preferuje zapach
        ///  0 -- zapach neutralny
        /// -1 -- klient nie lubi zapachu
        ///
        /// Zapachy numerujemy od 0
        /// </param>
        /// <param name="satisfactionLevel">Oczekiwany poziom satysfakcji</param>
        /// <param name="smells">Wyjściowa tablica rozpylonych zapachów, realizująca wyliczony poziom satysfakcji</param>
        public int AssignSmellsMaximizeHappyCustomers(int smellCount, int[][] customerPreferences, int satisfactionLevel, out bool[] smells)
        {
            int used = 0;
            smells = new bool[smellCount];
            var satisfactions = new int[customerPreferences.Length];
            int max_clients = maxsolveAssignSmells_old(satisfactions, customerPreferences, satisfactionLevel, smells, used);
            //int max_clients;
            //HashSet<int> assigned;
            //(max_clients, assigned) = maxsolveAssignSmells(smellCount, satisfactions, customerPreferences, satisfactionLevel, used);
            //foreach (var idx in assigned)
            //{
            //    smells[idx] = true;
            //}
            //int max_client = customerPreferences.Count(pref => pref.Sum(i => i < 0 ? (smells[-i - 1] ? -1 : 0) : (smells[i - 1] ? 1 : 0)) >= satisfactionLevel)
            System.Console.WriteLine($"return value is {max_clients}");
            return max_clients;
        }
    }
}

