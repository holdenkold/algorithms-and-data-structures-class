
namespace ASD
{
using System;

class VelocityMeasurementsTestCase : TestCase
    {
    private readonly int[] measurements;
    private readonly bool isJourney;
    private readonly (int minVelocity, int maxVelocity, int index) expected;
    private (int minVelocity, int maxVelocity, int index) result;
    private bool[] isBraking;

    public VelocityMeasurementsTestCase(double timeLimit, string desc, int[] measurements, (int minVelocity, int maxVelocity, int index) expected, bool isJourney) : base(timeLimit,null,desc)
        {
        this.measurements = measurements;
        this.expected = expected;
        this.isJourney = isJourney;
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        VelocityMeasurements vm = prototypeObject as VelocityMeasurements;
        result = isJourney ? vm.JourneyVelocities(measurements) : vm.FinalVelocities(measurements, out isBraking) ;
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings=null)
        {
        if ( expected.minVelocity!=result.minVelocity || expected.maxVelocity!=result.maxVelocity || expected.index!=result.index )
            return (Result.WrongResult,$"wrong result: {result}, expected {expected}");
        if ( !IsBrakingArrayCorrect() )
            return (Result.WrongResult,"incorrect isBraking array");
        return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");
        }

    private bool IsBrakingArrayCorrect()
        {
        if ( isJourney ) return true;
        if ( isBraking==null || isBraking.Length!=measurements.Length ) return false;
        int currentSpeedValue = 0;
        for ( int i=0 ; i<measurements.Length ; ++i )
            currentSpeedValue += isBraking[i]?-measurements[i]:measurements[i];
        return Math.Abs(currentSpeedValue)==expected.minVelocity;
        }

    }

class VelocityMeasurementsTestModule : TestModule
    {

    public override void PrepareTestSets()
        {
        TestSet ts;

        ts = TestSets["finalVelocitiesTestsLab"] = new TestSet(new VelocityMeasurements(),"Final velocities lab tests");
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 1", new int[] { 10, 9, 3 }, (2,22,3), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 2", new int[] { 0 }, (0,0,1), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 3", new int[] { 10 }, (10,10,1), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 4", new int[] { 10, 3, 5, 4 }, (2,22,4), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 5", new int[] { 4, 11, 5, 5, 5 }, (0,30,5), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 6", new int[] { 10, 10, 5, 3, 1 }, (1,29,5), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 7", new int[] { 10, 10, 5, 3, 1, 9, 24, 3, 4, 19, 18, 7, 7, 8, 10, 5 }, (1,143,16), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 8", new int[] { 7, 10, 2, 18, 4, 6, 6 }, (1,53,7), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(20, 1023), (0,1100,20), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(100, 1025), (1,4825,100), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(100, 12345), (1,4471,100), false));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(1000, 12347), (0,50018,1000), false));

        ts = TestSets["journeyVelocitiesTestsLab"] = new TestSet(new VelocityMeasurements(),"Journey velocities lab tests");
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 1", new int[] { 10, 9, 3 }, (1,22,2), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 2", new int[] { 10 }, (10,10,1), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 3", new int[] { 10, 1, 1, 1 }, (7,13,4), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 4", new int[] { 10, 3, 5, 4 }, (2,22,3), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 5", new int[] { 4, 11, 5, 5, 5 }, (0,30,5), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 6", new int[] { 10, 10, 5, 3, 1 }, (0,29,2), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 7", new int[] { 5, 7, 10, 23, 55, 2, 1, 23, 9, 0, 8, 4, 1, 24, 86, 5, 6, 100, 353, 4, 5, 67, 32, 45, 23, 34, 56, 32, 23 }, (0,1043,8), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(20, 1023), (0,1100,7), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(100, 1025), (0,4825,12), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(100, 12345), (0,4471,7), true));
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test losowy", GenerateTestArray(1000, 12347), (0,50018,4), true));
        int[] x = GenerateTestArray(10000, 1234567);
        x[0]=x[1]=2;
        ts.TestCases.Add(new VelocityMeasurementsTestCase(1, "Test 12", x, (0,491939,2), true));
        }

    private static int[] GenerateTestArray(int numberOfElements, int seed)
        {
        Random r = new Random(seed);
        int[] testArray = new int[numberOfElements];
        for ( int i=0 ; i<numberOfElements ; ++i )
            testArray[i] = r.Next(100);
        return testArray;
        }

    }

class Lab04Main
    {

    public static void Main()
        {
        var lab04tests = new VelocityMeasurementsTestModule();
        lab04tests.PrepareTestSets();
        foreach ( var ts in lab04tests.TestSets )
            ts.Value.PerformTests(verbose:true, checkTimeLimit:false);
        }

    }

}