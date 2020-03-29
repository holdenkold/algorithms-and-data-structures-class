
namespace ASD
{
    using System;
    using System.Linq;

    public class VelocityMeasurements : System.MarshalByRefObject
    {

        /// <summary>
        /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w momencie wypadku.
        /// </summary>
        /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
        /// <param name="isBraking">Tablica zwracająca informację dla każdego z pomiarów z tablicy measurements informację bool czy dla sekwencji dającej
        /// minimalną prędkość wynikową traktować dany pomiar jako hamujący (true) przy przyspieszający (false)</param>
        /// <returns>Krotka z informacjami o najniższej i najwyższej możliwej prędkości w momencie wypadku, numer pomiaru (nr) to w tym przypadku zawsze rozmiar tablicy pomiarów</returns>
        public (int minVelocity, int maxVelocity, int nr) FinalVelocities(int[] measurements, out bool[] isBraking)
        {
            isBraking = new bool[measurements.Length];

            int maxVelocity = measurements.Sum();
            int target = (int)maxVelocity / 2;
            int[,] dp = new int[measurements.Length + 1, target + 1];
            int minVelocity = 0;

            for (int i = 1; i < dp.GetLength(0); i++)
            {
                var value = measurements[i - 1];
                for (int j = 1; j < dp.GetLength(1); j++)
                {
                    if (j < value)
                        dp[i, j] = dp[i - 1, j];
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i - 1, j - value] + value);
                    }
                }
            }

            int first_subset_sum = dp[dp.GetLength(0) - 1, dp.GetLength(1) - 1];
            int second_subset_sum = maxVelocity - first_subset_sum;
            minVelocity = Math.Abs(first_subset_sum - second_subset_sum);

            int last = target;
            for (int i = measurements.Length; i > 0; i--)
            {
                var cur = dp[i, last];
                var prev = dp[i - 1, last];
                if (dp[i, last] == dp[i - 1, last])
                    continue;
                else
                {
                    var value = measurements[i - 1];
                    isBraking[i - 1] = true;
                    last -= measurements[i - 1];
                }
            }

            return (minVelocity, maxVelocity, measurements.Length);
        }

            /// <summary>
            /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w trakcie całego okresu trwania podróży.
            /// </summary>
            /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
            /// <returns>Krotka z informacjami o najniższej i najwyższej możliwej prędkości na trasie oraz informacją o numerze pomiaru dla którego najniższe prędkość może być osięgnięta</returns>
            public (int minVelocity, int maxVelocity, int nr) JourneyVelocities(int[] measurements)
        {
            return (-1, -1, -1);
        }

    }

}
