
namespace ASD
{
using System;
using System.Linq;

public class DictionaryTestCase : TestCase
    {

    private IListDictionary testedDict;
    private OperationInfo[] operations;
    private int[] expectedFinalState;
    private bool[] results;

    public DictionaryTestCase(double timeLimit, Exception expectedException, string description, IListDictionary testedDict, OperationInfo[] operations, int[] expectedFinalState)
            : base(timeLimit, expectedException, description)
        {
        this.testedDict = testedDict;
        this.operations = operations;
        this.expectedFinalState = expectedFinalState;
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        results = (prototypeObject as OperationExecutor).Execute(testedDict,operations);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
        for ( int i=0 ; i<operations.Length ; ++i )
            if ( results[i]!=operations[i].expRes )
                return (Result.WrongResult,$"incorrect operation result (operation No {i})");
        int[] finalState = testedDict.ToArray();
        if ( !expectedFinalState.SequenceEqual(finalState) )
            return (Result.WrongResult,"incorrect final dictionary state: ["+string.Join(",",finalState)+"] (expected: ["+string.Join(",",expectedFinalState)+"])");
        return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");
        }

    }

class DictionaryTestModule : TestModule
    {

    public override void PrepareTestSets()
        {

        TestSets["LabSimpleListTestsAdd"]  = new TestSet(new OperationExecutor(), "Lab. SimpleList Tests - Add (0.5p)");
        TestSets["LabSimpleListTestsAll"]  = new TestSet(new OperationExecutor(), "Lab. SimpleList Tests - All (0.5p)");
        TestSets["LabSortedListTestsAdd"]  = new TestSet(new OperationExecutor(), "Lab. SortedList Tests - Add (1.0p)");
        TestSets["LabSortedListTestsAll"]  = new TestSet(new OperationExecutor(), "Lab. SortedList Tests - All (0.5p)");
        TestSets["LabMoveToHeadListTestsAdd"] = new TestSet(new OperationExecutor(), "Lab. MoveToHeadList Tests - Add (0.5p)");
        TestSets["LabMoveToHeadListTestsAll"] = new TestSet(new OperationExecutor(), "Lab. MoveToHeadList Tests - All (1.0p)");

        {
        int n=3;
        OperationInfo[] oper= new OperationInfo[2*n+1];
        int[] final1 = new int[2*n+1];
        int[] final2 = new int[2*n+1];
        int[] final3 = new int[2*n+1];
        for (int i=-n ; i<=n ; ++i )
            {
            oper[i+n].oper = 'a';
            oper[i+n].val = i;
            oper[i+n].expRes = true;
            final1[i+n] = final2[i+n] = i;
            final3[-i+n] = i;
            }
        TestSets["LabSimpleListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - elementy posortowane", new SimpleList(), oper,final1));
        TestSets["LabSortedListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - elementy posortowane", new SortedList(), oper,final2));
        TestSets["LabMoveToHeadListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - elementy posortowane", new MoveToHeadList(), oper,final3));
        }

        {
        int n=3;
        OperationInfo[] oper= new OperationInfo[2*n+1];
        int[] final1 = new int[2*n+1];
        int[] final2 = new int[2*n+1];
        int[] final3 = new int[2*n+1];
        for (int i=-n ; i<=n ; ++i )
            {
            oper[i+n].oper = 'a';
            oper[i+n].val = -i;
            oper[i+n].expRes = true;
            final1[i+n] = -i;
            final2[i+n] = i;
            final3[-i+n] = -i;
            }
        TestSets["LabSimpleListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - elementy odwrotnie posortowane", new SimpleList(), oper,final1));
        TestSets["LabSortedListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - elementy odwrotnie posortowane", new SortedList(), oper,final2));
        TestSets["LabMoveToHeadListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - elementy odwrotnie posortowane", new MoveToHeadList(), oper,final3));
        }

        {
        OperationInfo[] oper= new OperationInfo[] {
            new OperationInfo('a', 2,true)  ,
            new OperationInfo('a', 5,true)  ,
            new OperationInfo('a', 3,true)  ,
            new OperationInfo('a', 3,false) ,
            new OperationInfo('a', 2,false) ,
            new OperationInfo('a', 7,true)  ,
            new OperationInfo('a', 5,false) ,
            new OperationInfo('a',-1,true)  ,
            };
        int[] final1 = new int[] { 2, 5, 3, 7, -1 };
        int[] final2 = new int[] { -1, 2, 3, 5, 7 };
        int[] final3 = new int[] { -1, 5, 7, 2, 3 };
        TestSets["LabSimpleListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - z powtorzeniami", new SimpleList(), oper,final1));
        TestSets["LabSortedListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - z powtorzeniami", new SortedList(), oper,final2));
        TestSets["LabMoveToHeadListTestsAdd"].TestCases.Add(new DictionaryTestCase(1, null, "tylko dodawanie - z powtorzeniami", new MoveToHeadList(), oper,final3));
        }

        {
        OperationInfo[] oper= new OperationInfo[] {
            new OperationInfo('s', 2,false) ,  //  0
            new OperationInfo('a', 2,true)  ,  //  1
            new OperationInfo('s', 2,true)  ,  //  2
            new OperationInfo('s', 6,false) ,  //  3
            new OperationInfo('a', 5,true)  ,  //  4
            new OperationInfo('s',-3,false) ,  //  5
            new OperationInfo('s', 6,false) ,  //  6
            new OperationInfo('a', 3,true)  ,  //  7
            new OperationInfo('a', 3,false) ,  //  8
            new OperationInfo('a', 2,false) ,  //  9
            new OperationInfo('a', 7,true)  ,  // 10
            new OperationInfo('s', 5,true)  ,  // 11
            new OperationInfo('a',-1,true)  ,  // 12
            new OperationInfo('s', 3,true)  ,  // 13
            new OperationInfo('s', 5,true)  ,  // 14
            };
        int[] final1 = new int[] { 2, 5, 3, 7, -1 };
        int[] final2 = new int[] { -1, 2, 3, 5, 7 };
        int[] final3 = new int[] { 5, 3, -1, 7, 2 };
        TestSets["LabSimpleListTestsAll"].TestCases.Add(new DictionaryTestCase(1, null, "dodawanie i wyszukiwanie", new SimpleList(), oper,final1));
        TestSets["LabSortedListTestsAll"].TestCases.Add(new DictionaryTestCase(1, null, "dodawanie i wyszukiwanie", new SortedList(), oper,final2));
        TestSets["LabMoveToHeadListTestsAll"].TestCases.Add(new DictionaryTestCase(1, null, "dodawanie i wyszukiwanie", new MoveToHeadList(), oper,final3));
        }

        {
        OperationInfo[] oper= new OperationInfo[] {
            new OperationInfo('d', 2,false) ,  //  0
            new OperationInfo('a', 2,true)  ,  //  1
            new OperationInfo('d', 2,true)  ,  //  2
            new OperationInfo('a', 2,true)  ,  //  3
            new OperationInfo('d', 6,false) ,  //  4
            new OperationInfo('a', 5,true)  ,  //  5
            new OperationInfo('d',-3,false) ,  //  6
            new OperationInfo('d', 6,false) ,  //  7
            new OperationInfo('a', 6,true)  ,  //  8
            new OperationInfo('a', 3,true)  ,  //  9
            new OperationInfo('a', 3,false) ,  // 10
            new OperationInfo('a', 2,false) ,  // 11
            new OperationInfo('a', 7,true)  ,  // 12
            new OperationInfo('d', 5,true)  ,  // 13
            new OperationInfo('a',-1,true)  ,  // 14
            new OperationInfo('d', 7,true)  ,  // 15
            new OperationInfo('d',-1,true)  ,  // 16
            new OperationInfo('d', 2,true)  ,  // 17
            };
        int[] final1 = new int[] { 6, 3 };
        int[] final2 = new int[] { 3, 6 };
        int[] final3 = new int[] { 3, 6 };
        TestSets["LabSimpleListTestsAll"].TestCases.Add(new DictionaryTestCase(1, null, "dodawanie i usuwanie", new SimpleList(), oper,final1));
        TestSets["LabSortedListTestsAll"].TestCases.Add(new DictionaryTestCase(1, null, "dodawanie i usuwanie", new SortedList(), oper,final2));
        TestSets["LabMoveToHeadListTestsAll"].TestCases.Add(new DictionaryTestCase(1, null, "dodawanie i usuwanie", new MoveToHeadList(), oper,final3));
        }

        }

    }

class Lab01
    {

    public static void Main()
        {
        DictionaryTestModule lab01tests = new DictionaryTestModule();
        lab01tests.PrepareTestSets();
        foreach (var ts in lab01tests.TestSets)
            ts.Value.PerformTests(verbose:true, checkTimeLimit:false);
        }

    }

}