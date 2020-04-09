using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab06
{

    public class SmellsChecker : MarshalByRefObject
    {
        private bool solveAssignSmells(int[] satisfactions, int[][] customerPreferences, int satisfactionLevel, bool[] smells, int used)
        {
            if (Array.TrueForAll(satisfactions, el => el >= satisfactionLevel))
                return true;

            for (int idx = used; idx < smells.Length; idx++)
            {
                smells[idx] = true;
                for (int i = 0; i < customerPreferences.Length; i++)
                {
                    var add = customerPreferences[i][idx];
                    satisfactions[i] += customerPreferences[i][idx];
                }

                if (solveAssignSmells(satisfactions, customerPreferences, satisfactionLevel, smells, idx + 1))
                    return true;

                smells[idx] = false;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] -= customerPreferences[i][idx];

            }
            return false;
        }

        private int maxsolveAssignSmells(int[] satisfactions, int[][] customerPreferences, int satisfactionLevel, bool[] smells, int used)
        {
            int max_client = satisfactions.Count(p => p >= satisfactionLevel);
            if (max_client == customerPreferences.Length)
                return max_client;

            for (int idx = used; idx < smells.Length; idx++)
            {
                smells[idx] = true;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] += customerPreferences[i][idx];

                int curr_max = maxsolveAssignSmells(satisfactions, customerPreferences, satisfactionLevel, smells, idx + 1);
                if (curr_max > max_client)
                    max_client = curr_max;

                if (max_client == customerPreferences.Length)
                    return max_client;

                smells[idx] = false;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] -= customerPreferences[i][idx];
            }

            return max_client;
        }


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
            int used = 0;
            smells = new bool[smellCount];
            var satisfactions = new int[customerPreferences.Length];
            bool can_assign = solveAssignSmells(satisfactions, customerPreferences, satisfactionLevel, smells, used);
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
            int max_clients = maxsolveAssignSmells(satisfactions, customerPreferences, satisfactionLevel, smells, used);
            System.Console.WriteLine($"return value is {max_clients}");
            return max_clients;
        }
    }
}

