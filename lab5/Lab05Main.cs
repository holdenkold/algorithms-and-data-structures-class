
using ASD;
using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab05
{

abstract class BasicSecondPathTestCase : TestCase
    {
    protected Graph g;
    protected Graph gCopy;
    protected Edge[] path;
    protected int a;
    protected int b;
    protected double? result;
    protected double? expectedResult;

    protected BasicSecondPathTestCase(double timeLimit, string description, Graph g, int a, int b, double? expectedResult) : base(timeLimit, null, description)
        {
        this.g = g;
        this.gCopy = g.Clone();
        this.a = a;
        this.b = b;
        this.expectedResult = expectedResult;
        }

    protected (Result,string) CheckResult()
        {
        if ( !g.IsEqual(gCopy) )
            return (Result.WrongResult,"illegal graph modification");
        if ( expectedResult.HasValue && !result.HasValue )
            return (Result.WrongResult,"no path found, but it exists");
        if  ( !expectedResult.HasValue && result.HasValue )
            return (Result.WrongResult,"path found, but it does not exist");
        if ( !expectedResult.HasValue && !result.HasValue )
            return path==null ? (Result.Success,$"OK, no second path exists (time:{PerformanceTime,6:#0.000})") : (Result.WrongResult,"inconsistent result (returned value: null, path: not null)");
        if ( expectedResult.Value != result.Value )
            return (Result.WrongResult,$"wrong length, expected {expectedResult}, returned {result}");
        return (Result.NotPerformed,null);
        }

    protected (Result,string) CheckIfProperPath()
        {
        if ( path.Length==0 )
            return (Result.WrongResult,"empty path returned");
        if ( path[0].From!=a )
            return (Result.WrongResult,"wrong starting point");
        if ( path[path.Length-1].To!=b )
            return (Result.WrongResult,"wrong final point");

        double sum = 0;
        Edge last = new Edge();
        bool first = true;
        foreach ( Edge e in path )
            {
            if ( e.From==b )
                return (Result.WrongResult,"destination vertex appears inside the path");
            if ( !g.OutEdges(e.From).Contains(e) )
                return (Result.WrongResult,"edge {e} does not exist");
            if ( first )
                first = false;
            else
                if ( e.From!=last.To )
                    return (Result.WrongResult,$"edge {last} cannot be followed by {e}");
            sum += e.Weight;
            last = e;
            }
        if ( sum!=result.Value )
            return (Result.WrongResult,$"path does not match the length, expected {result}, is {sum}");
        return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");
        }

    }

class RepSecondPathTestCase : BasicSecondPathTestCase
    {

    public RepSecondPathTestCase(double timeLimit, string description, Graph g, int a, int b, double? expectedLength) : base(timeLimit, description, g, a, b, expectedLength) {}

    protected override void PerformTestCase(object prototypeObject)
        {
        result = (prototypeObject as PathFinder).FindSecondShortestPath(g, a, b, out path);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings=null)
        {
        (Result resultCode, string message) res = CheckResult();
        if ( res.resultCode!=Result.NotPerformed )
            return res;
        return CheckIfProperPath() ;
        }

    }

class SimpleSecondPathTestCase : BasicSecondPathTestCase
    {

    public SimpleSecondPathTestCase(double timeLimit, string description, Graph g, int a, int b, double? expectedLength) : base(timeLimit, description, g, a, b, expectedLength) {}

    protected override void PerformTestCase(object prototypeObject)
        {
        result = (prototypeObject as PathFinder).FindSecondSimpleShortestPath(g, a, b, out path);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings=null)
        {
        (Result resultCode, string message) res = CheckResult();
        if ( res.resultCode!=Result.NotPerformed )
            return res;
        if ( !CheckSimple() )
            return (Result.WrongResult,"vertex repeated on a path" );
        return CheckIfProperPath() ;
        }

    private bool CheckSimple()
        {
        HashSet<int> vertices = new HashSet<int>();
        vertices.Add(path[0].From);
        vertices.Add(path[0].To);
        for ( int i=1 ; i<path.Length ; ++i )
            {
            if (vertices.Contains(path[i].To))
                return false;
            vertices.Add(path[i].To);
            }
        return true;
        }

    }

class SecondPathTestModule : TestModule
    {

    public override void PrepareTestSets()
        {

        TestSets["PathWithRepUndirLab"]    = new TestSet(new PathFinder(),"Path with repetitions, undirected graphs, lab. tests");
        TestSets["PathWithRepDirLab"]      = new TestSet(new PathFinder(),"Path with repetitions, directed graphs, lab. tests");
        TestSets["PathWithoutRepUndirLab"] = new TestSet(new PathFinder(),"Path without repetitions, undirected graphs, lab. tests");
        TestSets["PathWithoutRepDirLab"]   = new TestSet(new PathFinder(),"Path without repetitions, directed graphs, lab. tests");

        Random rnd;
        Graph g;
        int n;

        #region lab.: undirected, with repetitions

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(0, 2, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 1", g, 0, 4, null));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 2", g, 0, 4, 6));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        g.AddEdge(0, 4, 7);
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 3", g, 0, 4, 6));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 2);
        g.AddEdge(1, 2, 2);
        g.AddEdge(2, 3, 2);
        g.AddEdge(3, 4, 2);
        g.AddEdge(2, 4, 5);
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 4", g, 0, 4, 9));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 6);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(0, 4, 1);
        g.AddEdge(4, 5, 1);
        g.AddEdge(5, 3, 1);
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 5", g, 0, 3, 3));

        rnd = new Random(1001);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
            }
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(2, "Medium graph 1", g, 0, 11, 32));

        rnd = new Random(1003);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 3)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
            }
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(2, "Medium graph 2", g, 0, 11, 19));

        rnd = new Random(1005);
        n = 500;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 3)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
            }
        TestSets["PathWithRepUndirLab"].TestCases.Add(new RepSecondPathTestCase(14, "Large graph 1", g, 0, 11, 7));

        #endregion

        #region lab.: directed, with repetitions

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(0, 2, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 1", g, 0, 4, null));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 2", g, 0, 4, null));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        g.AddEdge(0, 4, 7);
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 3", g, 0, 4, 7));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 2);
        g.AddEdge(1, 2, 2);
        g.AddEdge(2, 3, 2);
        g.AddEdge(3, 4, 2);
        g.AddEdge(2, 4, 5);
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 4", g, 0, 4, 9));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(0, 4, 1);
        g.AddEdge(4, 5, 1);
        g.AddEdge(5, 3, 1);
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(1, "Small graph 5", g, 0, 3, 3));

        rnd = new Random(2001);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
                if (rnd.Next(10) <= 2)
                    g.AddEdge(j, i, 1 + rnd.Next(100));
            }
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(2, "Medium graph 1", g, 0, 11, 20));

        rnd = new Random(2003);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
                if (rnd.Next(10) <= 2)
                    g.AddEdge(j, i, 1 + rnd.Next(100));
            }
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(2, "Medium graph 2", g, 0, 11, 18));

        rnd = new Random(2005);
        n = 500;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 3)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
                if (rnd.Next(10) <= 3)
                    g.AddEdge(j, i, 1 + rnd.Next(100));
            }
        TestSets["PathWithRepDirLab"].TestCases.Add(new RepSecondPathTestCase(14, "Large graph 1", g, 0, 11, 8));

        #endregion

        #region lab.: undirected, without repetitions

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(0, 2, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 1", g, 0, 4, null));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 2", g, 0, 4, null));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        g.AddEdge(0, 4, 7);
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 3", g, 0, 4, 7));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        g.AddEdge(0, 1, 2);
        g.AddEdge(1, 2, 2);
        g.AddEdge(2, 3, 2);
        g.AddEdge(3, 4, 2);
        g.AddEdge(2, 4, 5);
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 4", g, 0, 4, 9));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 6);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(0, 4, 1);
        g.AddEdge(4, 5, 1);
        g.AddEdge(5, 3, 1);
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 5", g, 0, 3, 3));

        rnd = new Random(3001);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
            }
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(2, "Medium graph 1", g, 0, 11, 22));

        rnd = new Random(3003);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
            }
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(2, "Medium graph 2", g, 0, 11, 14));

        rnd = new Random(3005);
        n = 500;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
            }
        TestSets["PathWithoutRepUndirLab"].TestCases.Add(new SimpleSecondPathTestCase(16, "Large graph 1", g, 0, 11, 6));

        #endregion

        #region lab.: directed, without repetitions

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(0, 2, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 1", g, 0, 4, null));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 2", g, 0, 4, null));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(3, 4, 1);
        g.AddEdge(0, 4, 7);
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 3", g, 0, 4, 7));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
        g.AddEdge(0, 1, 2);
        g.AddEdge(1, 2, 2);
        g.AddEdge(2, 3, 2);
        g.AddEdge(3, 4, 2);
        g.AddEdge(2, 4, 5);
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 4", g, 0, 4, 9));

        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
        g.AddEdge(0, 1, 1);
        g.AddEdge(1, 2, 1);
        g.AddEdge(2, 3, 1);
        g.AddEdge(0, 4, 1);
        g.AddEdge(4, 5, 1);
        g.AddEdge(5, 3, 1);
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(1, "Small graph 5", g, 0, 3, 3));

        rnd = new Random(4001);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
                if (rnd.Next(10) <= 2)
                    g.AddEdge(j, i, 1 + rnd.Next(100));
            }
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(2, "Medium graph 1", g, 0, 11, 22));

        rnd = new Random(4003);
        n = 100;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 2)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
                if (rnd.Next(10) <= 2)
                    g.AddEdge(j, i, 1 + rnd.Next(100));
            }
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(2, "Medium graph 2", g, 0, 11, 19));

        rnd = new Random(4005);
        n = 500;
        g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
            {
                if (rnd.Next(10) <= 3)
                    g.AddEdge(i, j, 1 + rnd.Next(100));
                if (rnd.Next(10) <= 3)
                    g.AddEdge(j, i, 1 + rnd.Next(100));
            }
        TestSets["PathWithoutRepDirLab"].TestCases.Add(new SimpleSecondPathTestCase(16, "Large graph 1", g, 0, 11, 6));

        #endregion

        }

    }

class Program
    {

    static void Main(string[] args)
        {
        var lab05tests = new SecondPathTestModule();
        lab05tests.PrepareTestSets();
        foreach ( var ts in lab05tests.TestSets )
            ts.Value.PerformTests(verbose:true, checkTimeLimit:false);
        }

    }

}
