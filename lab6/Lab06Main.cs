using System;
using System.Linq;
using ASD;

namespace Lab06
{
    static class Util
    {

        public static int[][] ConvertPreferences(int[][] truePreferences, int smellCount)
        {
            int[][] output = new int[truePreferences.Length][];

            for (int i = 0; i < truePreferences.Length; i++)
            {
                output[i] = new int[smellCount];
                foreach (int p in truePreferences[i])
                {
                    output[i][Math.Abs(p) - 1] = Math.Sign(p);
                }
            }
            return output;
        }
    }

    public class ExistenceTestCase : TestCase
    {
        private readonly int[][] preferences;
        private readonly int smellCount;
        private readonly int satisfactionLevel;
        private readonly int[][] thePreferences;
        private readonly int[][] thePreferencesCopy;
        public readonly bool expectedResult;

        private bool result;
        private bool[] assignment;

        public static String ArrayToString<T>(T[] array)
        {
            return array != null ? String.Join(", ", array) : "null";
        }

        public ExistenceTestCase(int[][] preferences, int smellCount, int satisfactionLevel, bool expectedResult, double timeout, string desc) : base(timeout, null, desc)
        {
            this.preferences = preferences;
            this.smellCount = smellCount;
            this.expectedResult = expectedResult;
            this.satisfactionLevel = satisfactionLevel;
            this.thePreferences = Util.ConvertPreferences(this.preferences, this.smellCount);
            this.thePreferencesCopy = Util.ConvertPreferences(this.preferences, this.smellCount);
        }

        public (Result, string) SUCCESS() => (Result.Success, $"OK {PerformanceTime:#0.00}");

        private (Result, string) CheckAssignment(bool result, bool[] assignment)
        {
            if (result)
            {
                if (assignment is null)
                    return (Result.WrongResult, "Tablica jest nullem, mimo że jest rozwiązanie");
                else
                {
                    if (assignment.Length == smellCount)
                    {
                        if (preferences.All(
                                    pref => pref.Sum(i => i < 0 ? (assignment[-i - 1] ? -1 : 0) : (assignment[i - 1] ? 1 : 0)) >= satisfactionLevel))
                            return SUCCESS();
                        else
                        {
                            var notmatch = Enumerable.Range(0, preferences.Length).First(idx => preferences[idx].Sum(i => i < 0 ? (assignment[-i - 1] ? -1 : 0) : (assignment[i - 1] ? 1 : 0)) < satisfactionLevel);
                            return (Result.WrongResult, $"Zwrócona tablica nie daje klientowi {notmatch} wymaganego poziomu zadowolenia");
                        }
                    }
                    else
                    {
                        return (Result.WrongResult, "Zwrócona tablica ma inną długość niż liczba zapachów");
                    }
                }
            }
            else
            {
                if (!(assignment is null))
                    return (Result.WrongResult, "Zwrócono tablicę, mimo że nie ma rozwiązania");
                else
                    return SUCCESS();
            }
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            SmellsChecker sc = (SmellsChecker)prototypeObject;
            result = sc.AssignSmells(this.smellCount, thePreferences, this.satisfactionLevel, out assignment);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            for (int i = 0; i < thePreferences.Length; ++i)
                for (int j = 0; j < thePreferences[i].Length; ++j)
                    if (thePreferences[i][j] != thePreferencesCopy[i][j])
                        return (Result.WrongResult, "Niedozwolona modyfikacja tablicy preferencji");
            if (this.expectedResult != result)
                return (Result.WrongResult, $"Błędny wynik. Oczekiwano {expectedResult}, otrzymano {result}");
            else
                return CheckAssignment(result, assignment);
        }
    }

    public class OptimizeTestCase : TestCase
    {
        private readonly int[][] preferences;
        private readonly int smellCount;
        private readonly int satisfactionLevel;
        private readonly int[][] thePreferences;
        private readonly int[][] thePreferencesCopy;
        private readonly int expectedResult;

        private int result;
        private bool[] assignment;

        public OptimizeTestCase(int[][] preferences, int smellCount, int satisfactionLevel, int expectedResult, double timeout, string desc) : base(timeout, null, desc)
        {
            this.preferences = preferences;
            this.smellCount = smellCount;
            this.expectedResult = expectedResult;
            this.satisfactionLevel = satisfactionLevel;
            this.thePreferences = Util.ConvertPreferences(this.preferences, smellCount);
            this.thePreferencesCopy = Util.ConvertPreferences(this.preferences, this.smellCount);
        }

        public (Result, string) SUCCESS() => (Result.Success, $"OK {PerformanceTime:#0.00}");

        private (Result, string) CheckAssignment(int result, bool[] assignment)
        {
            if (assignment == null)
                return (Result.WrongResult, $"Tablica zapachów jest nullem");
            else
            {
                if (assignment.Length != smellCount)
                    return (Result.WrongResult, $"Tablica zapachów ma inny rozmiar niż liczba zapachów");
                else
                {
                    var ct = preferences.Count(pref => pref.Sum(i => i < 0 ? (assignment[-i - 1] ? -1 : 0) : (assignment[i - 1] ? 1 : 0)) >= satisfactionLevel);
                    if (ct == result)
                        return SUCCESS();
                    else
                        return (Result.WrongResult, $"Na podstawie przypisań {ct} klientów jest zadowolonych, a powinno być {result}");
                }
            }
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            SmellsChecker sc = (SmellsChecker)prototypeObject;
            result = sc.AssignSmellsMaximizeHappyCustomers(this.smellCount, thePreferences, this.satisfactionLevel, out assignment);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            for (int i = 0; i < thePreferences.Length; ++i)
                for (int j = 0; j < thePreferences[i].Length; ++j)
                    if (thePreferences[i][j] != thePreferencesCopy[i][j])
                        return (Result.WrongResult, "Niedozwolona modyfikacja tablicy preferencji");
            if (this.expectedResult != result)
                return (Result.WrongResult, $"Zwrócono nieprawidłową liczbę zadowolonych, oczekiwano {expectedResult}, otrzymano {result}");
            else
                return CheckAssignment(result, assignment);
        }
    }

    public class SmellTests : TestModule
    {
        private readonly TestSet existenceTests = new TestSet(new SmellsChecker(), "Etap 1 -- istnienie rozwiązania");
        private readonly TestSet optimizeTest = new TestSet(new SmellsChecker(), "Etap 2 -- maksymalizacja liczby zadowolonych");

        public override void PrepareTestSets()
        {
            TestSets["etap1-stud"] = existenceTests;
            TestSets["etap2-stud"] = optimizeTest;

            int[][] test1 = new int[][]
                {
                new int[]{ -1, 2, 3 },
                new int[]{ 1, 2, 3 },
                new int[]{ 2, -3 }
                };

            int[][] test2 = new int[][]
                {
                new int[]{ -1, 2, 3 },
                new int[]{ 1, 2, 3 },
                new int[]{ -2, -3 }
                };

            int[][] testNeg = new int[][]
                {
                new int[] { -15, -24 },
                new int[] { -10, -24 },
                new int[] { -14 },
                new int[] { 10 },
                new int[] { 4 }
                };

            int[][] test3a = new int[24][];
            for (int i = 0; i < 24; i++)
            {
                test3a[i] = new int[] { i + 1, -((i + 1) % 24 + 1) };
            }

            int[][] test3 = new int[50][];
            for (int i = 0; i < 50; i++)
            {
                test3[i] = new int[] { i + 1, -((i + 1) % 50 + 1) };
            }

            int[][] test4 = new int[50][];
            for (int i = 0; i < 50; i++)
            {
                test4[i] = new int[] { i + 1, (i + 1) % 50 + 1, -((i + 2) % 50 + 1) };
            }

            int[][] test4o = new int[21][];
            for (int i = 0; i < 21; i++)
            {
                test4o[i] = new int[] { i + 1, (i + 1) % 21 + 1, -((i + 2) % 21 + 1) };
            }

            int[][] test5 = new int[102][];
            for (int i = 0; i < 102; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        test5[i] = new int[] { i % 21 + 1, -(i + 1) % 21 - 1, (i + 2) % 21 + 1, (i + 3) % 21 + 1 };
                        break;
                    case 1:
                        test5[i] = new int[] { -(i % 21) - 1, (i + 2) % 21 + 1, (i + 1) % 21 + 1, (i + 4) % 21 + 1 };
                        break;
                    case 2:
                        test5[i] = new int[] { i % 21 + 1, (i + 2) % 21 + 1, (i + 1) % 21 + 1 };
                        break;
                }
            }

            int[][] test6 =
                {
                new int[]{ 1, -2, 3, 4, 5, -6, -7, -8, 9, 10 },
                new int[]{ -1, 2, 4, 6, -8, 9, 10 },
                new int[]{ 2, 6, 9, 10 },
                new int[]{ 1, 2, 3, -8, -9, 5 },
                new int[]{ -8, 2, 5, 7 },
                new int[]{ -1, -2, 3, 4, 5, 6, 7, 8, 9 },
                new int[]{ 1, 2, -3, -4, 5, 6, 7, 8, 9 }
                };

            //Test 1
            existenceTests.TestCases.Add(new ExistenceTestCase(test1, 3, 1, true, 1, "Prosty test możliwy"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test1, 3, 2, false, 1, "Prosty test niemożliwy"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test2, 3, 1, false, 1, "Prosty test przeciwne preferencje"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test6, 10, 3, true, 1, "10 zapachów"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test3a, 24, 1, false, 1, "sąsiedni klienci niezgodni"));
            //Test6
            existenceTests.TestCases.Add(new ExistenceTestCase(testNeg, 24, 1, false, 1, "klienci wiecznie niezadowoleni"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test4, 50, 0, true, 1, "wymagany poziom 0"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test3, 50, 1, false, 1, "niemożliwy 50"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test4, 50, 1, true, 1, "klienci 2×tak, 1×nie"));
            existenceTests.TestCases.Add(new ExistenceTestCase(test4, 50, 2, false, 1, "klienci 2×tak, 1×nie niemożliwy"));
            //Test 11
            existenceTests.TestCases.Add(new ExistenceTestCase(test5, 21, 2, true, 1, "niektórzy klienci wybredni"));


            // Test 1
            optimizeTest.TestCases.Add(new OptimizeTestCase(test1, 3, 1, 3, 1, "prosty wszyscy"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test1, 3, 2, 2, 1, "prosty bez jednego"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test1, 3, 5, 0, 1, "prosty nikt"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test2, 3, 1, 2, 1, "prosty bez jednego II"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test2, 3, 0, 3, 1, "prosty poziom 0"));
            //Test 6
            optimizeTest.TestCases.Add(new OptimizeTestCase(test6, 10, 4, 4, 1, "średni 1"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test6, 10, 3, 7, 1, "średni 2"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test3a, 24, 1, 12, 7, "średni 3"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test3a, 24, 2, 0, 1, "średni 4"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(testNeg, 24, 1, 2, 1, "klienci wiecznie niezadowoleni, oprócz 2"));
            //Test11
            optimizeTest.TestCases.Add(new OptimizeTestCase(test4o, 21, 1, 21, 1, "średni 5"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test4o, 21, 2, 7, 3, "średni 6"));
            optimizeTest.TestCases.Add(new OptimizeTestCase(test5, 21, 3, 68, 1, "102 klientów"));

        }

        public override double ScoreResult()
        {
            return 3;
        }

    }


    public class Program
    {

        public static void Main(string[] args)
        {
            var tests = new SmellTests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }

}
