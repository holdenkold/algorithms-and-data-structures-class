
using System;
using System.Collections.Generic;

namespace ASD
{
    public class Game : MarshalByRefObject
    {

        /// <summary>
        /// Zadanie na lab 2 z ASD2
        /// </summary>
        /// <param name="numbers">Zadany ciąg liczb</param>
        /// <param name="moves">Przykładowa sekwencja ruchów</param>
        /// <returns>Wynik gry dla gracza rozpoczynającego grę (suma zebranych liczb)</returns>
        /// 

        struct edge
        {
            public edge(int p1, int p2, bool tf)
            {
                this.p1 = p1;
                this.p2 = p2;
                this.take_first = tf;
            }
            public int p1;
            public int p2;
            public bool take_first;
        };


        public int OptimalStrategy(int[] numbers, out int[] moves)
        {
            moves = null;

            var dp = new edge[numbers.Length][];
            for (int i = 0; i < numbers.Length; i++)
            {
                dp[i] = new edge[numbers.Length - i];
            }

            for (int k = 0; k < dp.Length; k++)
            {
                for (int i = 0; i < dp[k].Length; i++)
                {
                    if (k == 0)
                    {
                        dp[k][i] = new edge(numbers[i], 0, true);
                        continue;
                    }

                    int p1_first = numbers[i] + dp[k - 1][i + 1].p2;
                    int p1_last = numbers[i + k] + dp[k - 1][i].p2;

                    if (p1_first > p1_last)
                    {
                        dp[k][i] = new edge(p1_first, dp[k - 1][i + 1].p1, true);
                    }
                    else
                    {
                        dp[k][i] = new edge(p1_last, dp[k - 1][i].p1, false);
                    }
                }
            }
            return dp[numbers.Length - 1][0].p1;
        }
    }
}
