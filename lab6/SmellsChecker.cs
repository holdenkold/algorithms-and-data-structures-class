using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab06
{

    public class SmellsChecker : MarshalByRefObject
    {
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
            fill_memory(customerPreferences, out int[,] memorisation, smellCount);
            bool can_assign = solveAssignSmells(satisfactions, memorisation, customerPreferences, satisfactionLevel, smells, 0);
            if (!can_assign)
                smells = null;
            return can_assign;
        }

        public bool solveAssignSmells(int[] satisfactions, int[,] memorisation, int[][] customerPreferences, int satisfactionLevel, bool[] smells, int used)
        {
            if (satisfactions.All(el => el >= satisfactionLevel))
                return true;

            if (!moveMakeSense(used, memorisation, customerPreferences, satisfactions, satisfactionLevel))
                return false;

            for (int idx = used; idx < smells.Length; idx++)
            {
                smells[idx] = true;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] += customerPreferences[i][idx];

                if (satisfactions.All(el => el + smells.Length - 1 - used >= satisfactionLevel))
                    if (solveAssignSmells(satisfactions, memorisation, customerPreferences, satisfactionLevel, smells, idx + 1))
                        return true;

                smells[idx] = false;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] -= customerPreferences[i][idx];

            }
            return false;
        }

        public void fill_memory(int[][] customerPreferences, out int[,] memorisation, int smellCount)
        {
            int n = customerPreferences.Length;
            memorisation = new int[n, smellCount + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = smellCount - 1; j >= 0; j--)
                {
                    memorisation[i, j] = memorisation[i, j + 1];
                    if (customerPreferences[i][j] == 1)
                        memorisation[i, j] += 1;
                }
            }
        }
        public bool moveMakeSense(int used, int[,] memorisation, int[][] customerPreferences, int[] satisfactions, int satisfactionLevel)
        {
            for (int i = 0; i < customerPreferences.Length; i++)
            {
                if (satisfactions[i] + memorisation[i, used] < satisfactionLevel)
                    return false;
            }
            return true;
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

            bool[] temp_smells = new bool[smellCount];
            smells = new bool[smellCount];

            var satisfactions = new int[customerPreferences.Length];
            fill_memory(customerPreferences, out int[,] memorisation, smellCount);
            int max_clients = maxsolveAssignSmells(satisfactions, memorisation, customerPreferences, satisfactionLevel, temp_smells, smells, 0, used);
            System.Console.WriteLine($"return value is {max_clients}");
            return max_clients;
        }

        public int maxsolveAssignSmells(int[] satisfactions, int[,] memorisation, int[][] customerPreferences, int satisfactionLevel, bool[] smells, bool[] max_smells, int max_clients, int used)
        {
            if (!maxMoveMakeSense(used, memorisation, customerPreferences, satisfactions, satisfactionLevel, max_clients))
                return max_clients;

            int max_client = satisfactions.Count(p => p >= satisfactionLevel);
            if (max_client == customerPreferences.Length)
            {
                smells.CopyTo(max_smells, 0);
                return max_client;
            }

            if (max_client > max_clients)
            {
                max_clients = max_client;
                smells.CopyTo(max_smells, 0);
            }
            for (int idx = used; idx < smells.Length; idx++)
            {
                smells[idx] = true;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] += customerPreferences[i][idx];

                int curr_max = maxsolveAssignSmells(satisfactions, memorisation, customerPreferences, satisfactionLevel, smells, max_smells, max_clients, idx + 1);
                if (curr_max > max_clients)
                    max_clients = curr_max;


                if (max_clients == customerPreferences.Length)
                {
                    smells.CopyTo(max_smells, 0);
                    return max_clients;
                }
                smells[idx] = false;
                for (int i = 0; i < customerPreferences.Length; i++)
                    satisfactions[i] -= customerPreferences[i][idx];
            }

            return max_clients;
        }

        public bool maxMoveMakeSense(int used, int[,] memorisation, int[][] customerPreferences, int[] satisfactions, int satisfactionLevel, int max_clients)
        {
            int unsatisfied = 0;
            for (int i = 0; i < customerPreferences.Length; i++)
            {
                if (satisfactions[i] + memorisation[i, used] < satisfactionLevel)
                    unsatisfied += 1;
            }
            return satisfactions.Length - max_clients > unsatisfied;
        }



    }
}

