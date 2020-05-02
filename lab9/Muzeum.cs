using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab09
{

public class Museum : MarshalByRefObject
    {

    /// <summary>
    /// Znajduje najliczniejszy multizbiór tras
    /// </summary>
    /// <returns>Liczność wyznaczonego multizbioru tras (liczba ścieżek w tym multizbiorze)</returns>
    /// <param name="g">Graf opisujący muzeum</param>
    /// <param name="levels">Tablica o długości równej liczbie wierzchołków w grafie -- poziomy ciekawości wystaw</param>
    /// <param name="entrances">Wejścia</param>
    /// <param name="exits">Wyjścia</param>
    /// <param name="routes">Ścieżki należące do wyznaczonego multizbioru</param>
    /// <remarks>
    /// Parametr routes to rablica tablic.
    /// Rozmiar zewnętrznej tablicy musi być równy liczbie ścieżek w multizbiorze.
    /// Każda wewnętrzna tablica opisuje jedną ścieżkę, a ściślej wymienia sale przez, które ona przechodzi.
    /// Kolejność sal jest istotna.
    /// </remarks>
    public int FindRoutes(Graph g, int[] levels, int[] entrances, int[] exits, out int[][] routes)
        {
        routes = null;
        return -1;
        }

    }

}

