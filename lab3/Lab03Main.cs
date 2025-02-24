﻿
namespace ASD
{
using System;
using System.Collections.Generic;
using ASD.Graphs;

public class SquareofGraphTestCase : TestCase
    {
    private Graph input_graph;
    private Graph graph;
    private Graph expected_result_graph;
    private Graph result_graph;

    public SquareofGraphTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph expected_result_graph)
        : base(timeLimit, expectedException, description)
        {
        this.graph = graph;
        this.expected_result_graph = expected_result_graph;
        input_graph = graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        result_graph = ((Lab03)prototypeObject).Square(graph);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings=null)
        {
        if ( !input_graph.IsEqual(graph) )
            return (Result.WrongResult,"Changed input graph");
        if ( result_graph==null )
            return (Result.WrongResult,"No solution");
        if ( result_graph.GetType()!=expected_result_graph.GetType() )
            return (Result.WrongResult,"Incorrect result graph representation");
        //tu rozwiazanie jest jednoznaczne - sprawdzamy tylko rownosc grafu, izomorfizm niepotrzebny
        if ( !expected_result_graph.IsEqual(result_graph) )
            return (Result.WrongResult,"Incorrect result graph");
        return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");
        }

    }

public class LineGraphTestCase : TestCase
    {
    private Graph input_graph;
    private Graph graph;
    private Graph expected_result_graph;
    private Graph result_graph;
    private (int,int)[] names;

    public LineGraphTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph res_graph)
        : base(timeLimit, expectedException, description)
        {
        this.graph = graph;
        this.expected_result_graph = res_graph;
        input_graph = graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        result_graph = ((Lab03)prototypeObject).LineGraph(graph, out names);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings=null)
        {
        if ( !input_graph.IsEqual(graph) )
            return (Result.WrongResult,"Changed input graph");
        if ( result_graph==null )
            return (Result.WrongResult,"No solution");
        if ( result_graph.GetType()!=expected_result_graph.GetType() )
            return (Result.WrongResult,"Incorrect result graph representation");

        if ( expected_result_graph.EdgesCount!=result_graph.EdgesCount || expected_result_graph.VerticesCount!=result_graph.VerticesCount || result_graph.Directed )
            return (Result.WrongResult,"Incorrect result graph");
        if ( !( result_graph.VerticesCount>=4900 || expected_result_graph.IsEqual(result_graph) || expected_result_graph.Isomorphism(result_graph)!=null ) )
            return (Result.WrongResult,"Incorrect result graph");

        if ( names==null )
            return (Result.WrongResult,"names table is null");
        if ( names.Length!=graph.EdgesCount )
            return (Result.WrongResult,"Incorrect size of names table");
        foreach ( var elem in names )
            if ( graph.GetEdgeWeight(elem.Item1,elem.Item2).IsNaN() )
                return (Result.WrongResult,$"Incorrect names table element ({elem.Item1},{elem.Item2})");

        return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");
        }

    }

public class VertexColoringTestCase : TestCase
    {
    private Graph input_graph;
    private Graph graph;
    private int expected_colors_number;
    private int colors_number;
    private int[] expected_colors;
    private int[] colors;

    public VertexColoringTestCase(double timeLimit, Exception expectedException, string description, Graph graph, int num, int[] colors)
        : base(timeLimit, expectedException, description)
        {
        this.graph = graph;
        this.expected_colors_number = num;
        this.expected_colors = colors;
        input_graph = graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        colors_number = ((Lab03)prototypeObject).VertexColoring(graph, out colors);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings=null)
        {
        if ( !input_graph.IsEqual(graph) )
            return (Result.WrongResult,"Changed input graph");
        if ( colors==null )
            return (Result.WrongResult,"No solution");
        if ( expected_colors_number!=colors_number )
            return (Result.WrongResult,$"Incorrect numbers of colors: {colors_number}, should be: {expected_colors_number}");

        //algorytm podany jest w taki, sposob, ze kolorowanie mozliwe jest tylko jedno - sprawdzam czy tablice sa identyczne
        for (int id = 0; id < colors_number; ++id)
            {
            if ( expected_colors[id]!=colors[id] )
                {
                string message = $"Incorrect coloring";
                string c1 = "", c2 = "";
                for ( int i=0 ; i<graph.VerticesCount ; ++i )
                    {
                    c1 += "[ " + i + ": " + colors[i] + "] ";
                    c2 += "[ " + i + ": " + expected_colors[i] + "] ";
                    }
                message += "\nResult: \n" + c1 + "\n" + "Expected: \n" + c2;
                //wyswietlic kolorowanie
                return (Result.WrongResult,message);
                }
            }
        return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");
        }

    }

public class StrongEdgeColoringTestCase : TestCase
    {
    private Graph input_graph;
    private Graph graph;
    private Graph expected_result_graph;
    private Graph result_graph;
    private int colors_number;

    public StrongEdgeColoringTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph res_graph)
        : base(timeLimit, expectedException, description)
        {
        this.graph = graph;
        this.expected_result_graph = res_graph;
        input_graph = graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        colors_number = ((Lab03)prototypeObject).StrongEdgeColoring(graph, out result_graph);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings=null)
        {
        // kolorowanie, a nawet liczba uzytych kolorow, moze byc inna niż w naszym rozwiazaniu
        // ponieważ po drodze może byc inny (chociaż izomorficzny) graf krawedziowy

        if ( !input_graph.IsEqual(graph) )
            return (Result.WrongResult,"Changed input graph");
        if ( result_graph==null )
            return (Result.WrongResult,"No solution");
        if ( result_graph.GetType()!=expected_result_graph.GetType() )
            return (Result.WrongResult,"Incorrect result graph representation");

        if ( expected_result_graph.EdgesCount!=result_graph.EdgesCount || expected_result_graph.VerticesCount!=result_graph.VerticesCount )
            return (Result.WrongResult,"Incorrect result graph");

        if ( expected_result_graph.IsEqual(result_graph) )
            return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");   // kolorowanie takie jak u nas

        if ( colors_number<0 || colors_number>result_graph.EdgesCount )
            return (Result.WrongResult,$"Incorrect numbers of colors: {colors_number}");

        bool[] used = new bool[colors_number];
        //tu sprawdzam czy zwrocone kolorowanie krawedzi jest poprawne
        for ( int i=0 ; i<result_graph.VerticesCount ; ++i )
            foreach ( Edge e1 in result_graph.OutEdges(i) )
                {
                if ( e1.Weight<0 || e1.Weight>=colors_number )
                    return (Result.WrongResult,$"Invalid color: {e1.Weight}");
                used[(int)e1.Weight] = true;
                foreach ( Edge e2 in result_graph.OutEdges(e1.To) )
                    {
                    if ( e1.From!=e2.To && e1.Weight==e2.Weight )
                        return (Result.WrongResult,$"Incorrect coloring. Edges [{e1.From}, {e1.To}] and [{e2.From}, {e2.To}] have the same color!");
                    foreach ( Edge e3 in result_graph.OutEdges(e2.To) )
                        if ( !( e1.From==e3.From && e1.To==e3.To ) && !( e1.From==e3.To && e1.To==e3.From ) && e1.Weight==e3.Weight )
                            return (Result.WrongResult,$"Incorrect coloring. Edges [{e1.From}, {e1.To}] and [{e3.From}, {e3.To}] have the same color!");
                    }
                }
        if ( !System.Linq.Enumerable.All(used,x=>x) )
            return (Result.WrongResult,"Incorrect result graph");

        return (Result.Success,$"OK (time:{PerformanceTime,6:#0.000})");
        }

    }


class Lab03TestModule : TestModule
    {

    public override void PrepareTestSets()
        {
        var rgg = new RandomGraphGenerator();
        int n = 6;
        Graph[] graphs = new Graph[n];
        string[] descriptions = new string[n];

        Graph[] squares = new Graph[n];
        Graph[] linegraphs = new Graph[n];
        List<int[]> colors = new List<int[]>();
        int[] chn = new int[n]; //chromatic number
        Graph[] sec_graphs = new Graph[n];

        //przyklady z zajec
        graphs[0] = new AdjacencyMatrixGraph(false, 5);
        graphs[0].AddEdge(0, 1);
        graphs[0].AddEdge(0, 3);
        graphs[0].AddEdge(1, 2);
        graphs[0].AddEdge(2, 0);
        graphs[0].AddEdge(2, 3);
        graphs[0].AddEdge(3, 4);

        //sciezka P6
        graphs[1] = new AdjacencyListsGraph<AVLAdjacencyList>(false, 6);
        graphs[1].AddEdge(0, 1);
        graphs[1].AddEdge(1, 2);
        graphs[1].AddEdge(2, 3);
        graphs[1].AddEdge(3, 4);
        graphs[1].AddEdge(4, 5);

        graphs[2] = new AdjacencyMatrixGraph(false, 9);
        graphs[2].AddEdge(0, 3);
        graphs[2].AddEdge(1, 0);
        graphs[2].AddEdge(6, 0);
        graphs[2].AddEdge(1, 5);
        graphs[2].AddEdge(5, 4);
        graphs[2].AddEdge(5, 2);
        graphs[2].AddEdge(5, 6);
        graphs[2].AddEdge(6, 7);
        graphs[2].AddEdge(7, 8);

        //cykl C5
        graphs[3] = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        graphs[3].AddEdge(0, 1);
        graphs[3].AddEdge(1, 2);
        graphs[3].AddEdge(2, 3);
        graphs[3].AddEdge(3, 4);
        graphs[3].AddEdge(4, 0);

        //klika K5
        rgg.SetSeed(12345);
        graphs[4] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 5, 1.0);

        //duzy cykl skierowany
        int g5vc = 9000;
        graphs[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(true,g5vc);
        for ( int i=1 ; i<g5vc ; ++i )
            graphs[5].AddEdge(i-1,i);
        graphs[5].AddEdge(g5vc-1,0);

        squares[0] = graphs[0].Clone();
        squares[0].AddEdge(0, 4);
        squares[0].AddEdge(1, 3);
        squares[0].AddEdge(2, 4);

        squares[1] = graphs[1].Clone();
        squares[1].AddEdge(0, 2);
        squares[1].AddEdge(1, 3);
        squares[1].AddEdge(2, 4);
        squares[1].AddEdge(3, 5);

        squares[2] = graphs[2].Clone();
        squares[2].AddEdge(0, 5);
        squares[2].AddEdge(0, 6);
        squares[2].AddEdge(0, 7);
        squares[2].AddEdge(1, 2);
        squares[2].AddEdge(1, 3);
        squares[2].AddEdge(1, 4);
        squares[2].AddEdge(1, 6);
        squares[2].AddEdge(2, 4);
        squares[2].AddEdge(2, 6);
        squares[2].AddEdge(3, 6);
        squares[2].AddEdge(4, 6);
        squares[2].AddEdge(5, 7);
        squares[2].AddEdge(6, 8);

        squares[3] = rgg.UndirectedGraph(typeof(AdjacencyListsGraph<HashTableAdjacencyList>), 5, 1.0);

        squares[4] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 5, 1.0);

        squares[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(true,g5vc);
        for ( int i=0 ; i<g5vc ; ++i )
            {
            squares[5].AddEdge(i,(i+1)%g5vc);
            squares[5].AddEdge(i,(i+2)%g5vc);
            }

        linegraphs[0] = new AdjacencyMatrixGraph(false, 6);
        linegraphs[0].AddEdge(0, 1);
        linegraphs[0].AddEdge(0, 2);
        linegraphs[0].AddEdge(0, 3);
        linegraphs[0].AddEdge(1, 2);
        linegraphs[0].AddEdge(1, 3);
        linegraphs[0].AddEdge(1, 4);
        linegraphs[0].AddEdge(2, 4);
        linegraphs[0].AddEdge(2, 5);
        linegraphs[0].AddEdge(3, 4);
        linegraphs[0].AddEdge(4, 5);

        linegraphs[1] = new AdjacencyListsGraph<AVLAdjacencyList>(false, 5);
        linegraphs[1].AddEdge(0, 1);
        linegraphs[1].AddEdge(1, 2);
        linegraphs[1].AddEdge(2, 3);
        linegraphs[1].AddEdge(3, 4);

        linegraphs[2] = new AdjacencyMatrixGraph(false, 9);
        linegraphs[2].AddEdge(0, 1);
        linegraphs[2].AddEdge(0, 2);
        linegraphs[2].AddEdge(0, 3);
        linegraphs[2].AddEdge(1, 2);
        linegraphs[2].AddEdge(2, 6);
        linegraphs[2].AddEdge(2, 7);
        linegraphs[2].AddEdge(3, 4);
        linegraphs[2].AddEdge(3, 5);
        linegraphs[2].AddEdge(3, 6);
        linegraphs[2].AddEdge(4, 5);
        linegraphs[2].AddEdge(4, 6);
        linegraphs[2].AddEdge(5, 6);
        linegraphs[2].AddEdge(6, 7);
        linegraphs[2].AddEdge(7, 8);

        linegraphs[3] = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
        linegraphs[3].AddEdge(0, 1);
        linegraphs[3].AddEdge(0, 2);
        linegraphs[3].AddEdge(1, 4);
        linegraphs[3].AddEdge(2, 3);
        linegraphs[3].AddEdge(3, 4);

        linegraphs[4] = new AdjacencyMatrixGraph(false, 10);
        linegraphs[4].AddEdge(0, 1);
        linegraphs[4].AddEdge(0, 2);
        linegraphs[4].AddEdge(0, 3);
        linegraphs[4].AddEdge(0, 4);
        linegraphs[4].AddEdge(0, 5);
        linegraphs[4].AddEdge(0, 6);
        linegraphs[4].AddEdge(1, 2);
        linegraphs[4].AddEdge(1, 3);
        linegraphs[4].AddEdge(1, 4);
        linegraphs[4].AddEdge(1, 7);
        linegraphs[4].AddEdge(1, 8);
        linegraphs[4].AddEdge(2, 3);
        linegraphs[4].AddEdge(2, 5);
        linegraphs[4].AddEdge(2, 7);
        linegraphs[4].AddEdge(2, 9);
        linegraphs[4].AddEdge(3, 6);
        linegraphs[4].AddEdge(3, 8);
        linegraphs[4].AddEdge(3, 9);
        linegraphs[4].AddEdge(4, 5);
        linegraphs[4].AddEdge(4, 6);
        linegraphs[4].AddEdge(4, 7);
        linegraphs[4].AddEdge(4, 8);
        linegraphs[4].AddEdge(5, 6);
        linegraphs[4].AddEdge(5, 7);
        linegraphs[4].AddEdge(5, 9);
        linegraphs[4].AddEdge(6, 8);
        linegraphs[4].AddEdge(6, 9);
        linegraphs[4].AddEdge(7, 8);
        linegraphs[4].AddEdge(7, 9);
        linegraphs[4].AddEdge(8, 9);

        linegraphs[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(false,g5vc);
        for ( int i=0 ; i<g5vc ; ++i )
            linegraphs[5].AddEdge(i,(i+1)%g5vc);

        colors.Add(new int[5]);
        colors[0][0] = 0;
        colors[0][1] = 1;
        colors[0][2] = 2;
        colors[0][3] = 1;
        colors[0][4] = 0;
        chn[0] = 3;

        colors.Add(new int[6]);
        colors[1][0] = 0;
        colors[1][1] = 1;
        colors[1][2] = 0;
        colors[1][3] = 1;
        colors[1][4] = 0;
        colors[1][5] = 1;
        chn[1] = 2;

        colors.Add(new int[9]);
        colors[2][0] = 0;
        colors[2][1] = 1;
        colors[2][2] = 0;
        colors[2][3] = 1;
        colors[2][4] = 0;
        colors[2][5] = 2;
        colors[2][6] = 1;
        colors[2][7] = 0;
        colors[2][8] = 1;
        chn[2] = 3;

        colors.Add(new int[5]);
        colors[3][0] = 0;
        colors[3][1] = 1;
        colors[3][2] = 0;
        colors[3][3] = 1;
        colors[3][4] = 2;
        chn[3] = 3;

        colors.Add(new int[5]);
        colors[4][0] = 0;
        colors[4][1] = 1;
        colors[4][2] = 2;
        colors[4][3] = 3;
        colors[4][4] = 4;
        chn[4] = 5;

        colors.Add(new int[g5vc]);  // nie bedzie uzywana

        sec_graphs[0] = graphs[0].IsolatedVerticesGraph();
        sec_graphs[0].AddEdge(0, 1, 0);
        sec_graphs[0].AddEdge(0, 3, 2);
        sec_graphs[0].AddEdge(1, 2, 3);
        sec_graphs[0].AddEdge(2, 0, 1);
        sec_graphs[0].AddEdge(2, 3, 4);
        sec_graphs[0].AddEdge(3, 4, 5);

        sec_graphs[1] = graphs[1].IsolatedVerticesGraph();
        sec_graphs[1].AddEdge(0, 1, 0);
        sec_graphs[1].AddEdge(1, 2, 1);
        sec_graphs[1].AddEdge(2, 3, 2);
        sec_graphs[1].AddEdge(3, 4, 0);
        sec_graphs[1].AddEdge(4, 5, 1);

        sec_graphs[2] = graphs[2].IsolatedVerticesGraph();
        sec_graphs[2].AddEdge(0, 3, 1);
        sec_graphs[2].AddEdge(1, 0, 0);
        sec_graphs[2].AddEdge(6, 0, 2);
        sec_graphs[2].AddEdge(1, 5, 3);
        sec_graphs[2].AddEdge(5, 4, 4);
        sec_graphs[2].AddEdge(5, 2, 1);
        sec_graphs[2].AddEdge(5, 6, 5);
        sec_graphs[2].AddEdge(6, 7, 6);
        sec_graphs[2].AddEdge(7, 8, 0);

        sec_graphs[3] = graphs[3].IsolatedVerticesGraph();
        sec_graphs[3].AddEdge(0, 1, 0);
        sec_graphs[3].AddEdge(1, 2, 2);
        sec_graphs[3].AddEdge(2, 3, 3);
        sec_graphs[3].AddEdge(3, 4, 4);
        sec_graphs[3].AddEdge(4, 0, 1);

        sec_graphs[4] = graphs[4].IsolatedVerticesGraph();
        sec_graphs[4].AddEdge(0, 1, 0);
        sec_graphs[4].AddEdge(0, 2, 1);
        sec_graphs[4].AddEdge(0, 3, 2);
        sec_graphs[4].AddEdge(0, 4, 3);
        sec_graphs[4].AddEdge(1, 2, 4);
        sec_graphs[4].AddEdge(1, 3, 5);
        sec_graphs[4].AddEdge(1, 4, 6);
        sec_graphs[4].AddEdge(2, 3, 7);
        sec_graphs[4].AddEdge(2, 4, 8);
        sec_graphs[4].AddEdge(3, 4, 9);

        sec_graphs[5] = graphs[5].IsolatedVerticesGraph();
        for ( int i=0 ; i<g5vc ; ++i )
            sec_graphs[5].AddEdge(i,(i+1)%g5vc,i%3);

        TestSets["LabSquareOfGraphTests"] = new TestSet(new Lab03(), "Lab. - Part 1 - square of graph");
        TestSets["LabLineGraphTests"] = new TestSet(new Lab03(), "Lab. - Part 2 - line graph");
        //TestSets["LabVertexColoringTests"] = new TestSet(new Lab03(), "Lab. - Part 3 - vertex coloring");
        TestSets["LabStrongEdgeColoringTests"] = new TestSet(new Lab03(), "Lab. - Part 4 - strong edge coloring");

        for ( int i=0 ; i<n ; ++i )
            {
            TestSets["LabSquareOfGraphTests"].TestCases.Add(new SquareofGraphTestCase(i==5?30:1, null, descriptions[i], graphs[i], squares[i]));
            TestSets["LabLineGraphTests"].TestCases.Add(new LineGraphTestCase(i==5?30:1, null, descriptions[i], graphs[i], linegraphs[i]));
            //TestSets["LabVertexColoringTests"].TestCases.Add(new VertexColoringTestCase(i==5?30:1, graphs[i].Directed?new ArgumentException():null, descriptions[i], graphs[i], chn[i], colors[i]));
            TestSets["LabStrongEdgeColoringTests"].TestCases.Add(new StrongEdgeColoringTestCase(i==5?30:1, null, descriptions[i], graphs[i], sec_graphs[i]));
            }

        }

    }

class Lab03Main
    {

    public static void Main()
        {
        Lab03TestModule lab03test = new Lab03TestModule();
        lab03test.PrepareTestSets();
        foreach ( var ts in lab03test.TestSets )
            ts.Value.PerformTests(verbose:true, checkTimeLimit:false);
        }

    }

}