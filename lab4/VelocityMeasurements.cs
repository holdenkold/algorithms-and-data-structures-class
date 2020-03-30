
namespace ASD
{
    using System;
    using System.Linq;

    public class VelocityMeasurements : System.MarshalByRefObject
    {
        struct node
        {
            public node(int val, int refer, int row)
            {
                this.val = val;
                this.refer = refer;
                this.row = row;
            }
            public int val;
            public int refer;
            public int row;
        };


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
            node[] dp = new node[target + 1];


            for (int i = 0; i < measurements.Length; i++)
            {
                for (int j = target; j >= measurements[i]; j--)
                {
                    var value = measurements[i];
                    var prev = dp[j - value].val;
                    if (prev + value > dp[j].val)
                    {
                        dp[j] = new node(prev + value, j - value, i);
                    }
                }
            }

            int idx = dp.Length - 1;
            while (dp[idx].refer != idx)
            {
                isBraking[dp[idx].row] = true;
                idx = dp[idx].refer;
            }

            int minVelocity = Math.Abs(maxVelocity - 2 * dp[target].val);
            return (minVelocity, maxVelocity, measurements.Length);
        }

        /// <summary>
        /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w trakcie całego okresu trwania podróży.
        /// </summary>
        /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
        /// <returns>Krotka z informacjami o najniższej i najwyższej możliwej prędkości na trasie oraz informacją o numerze pomiaru dla którego najniższe prędkość może być osięgnięta</returns>
        public (int minVelocity, int maxVelocity, int nr) JourneyVelocities(int[] measurements)
        {
            int maxVelocity = measurements.Sum();
            int target = (int)maxVelocity / 2;
            int[] dp = new int[target + 1];
            int idx = 0;
            int value;
            int curMaxVelocity = measurements[0];
            for (int i = 0; i < measurements.Length; i++)
            {
                for (int j = target; j >= measurements[i]; j--)
                {
                    value = measurements[i];
                    var prev = dp[j - value];
                    if (prev + value > dp[j])
                        dp[j] = prev + value;
                }
            }

            target = (int)(curMaxVelocity / 2);
            int minVelocity = Math.Abs(2 * dp[target] - curMaxVelocity);
            for (int i = 0; i < measurements.Length; i++)
            {
                curMaxVelocity += measurements[i];
                target = (int)(curMaxVelocity / 2);
                if (dp[i] < minVelocity)
                {
                    idx = i;
                    minVelocity = Math.Min(minVelocity, Math.Abs(2 * dp[i] - curMaxVelocity));
                }
            }
            return (minVelocity, measurements.Sum(), idx);
        }

    }

}
