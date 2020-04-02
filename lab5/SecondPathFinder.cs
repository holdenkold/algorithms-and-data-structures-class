
using ASD.Graphs;

namespace Lab05
{

public class PathFinder : System.MarshalByRefObject
    {

    /// <summary>
    /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
    /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki, algorytm zwróci jedną z nich).
    /// Dopuszczamy, aby na ścieżce powtarzały się wierzchołki i/lub krawędzie.
    /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
    /// </summary>
    /// <remarks>
    /// Wymagana złożoność do O(D), gdzie D jest złożonością implementacji alogorytmu Dijkstry w bibliotece Graph.
    /// </remarks>
    /// <param name="g">badany graf</param>
    /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako tablica krawędzi</param>
    /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość znalezionej ścieżki</returns>
    public double? FindSecondShortestPath(Graph g, int a, int b, out Edge[] path)
        {
        path = null;
        return -1.0;
        }

    /// <summary>
    /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
    /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki, algorytm zwróci jedną z nich).
    /// Wymagamy, aby na ścieżce nie było powtórzeń wierzchołków ani krawędzi.
    /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
    /// </summary>
    /// <remarks>
    /// Wymagana złożoność to O(nD), gdzie D jest złożonością implementacji algorytmu Dijkstry w bibliotece Graph.
    /// </remarks>
    /// <param name="g">badany graf</param>
    /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako tablica krawędzi</param>
    /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość tej ścieżki</returns>
    public double? FindSecondSimpleShortestPath(Graph g, int a, int b, out Edge[] path)
        {
        path = null;
        return -1.0;
        }

    }

}
