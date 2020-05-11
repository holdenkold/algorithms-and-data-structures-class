using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class ProductionPlanner : MarshalByRefObject
    {
        /// <summary>
        /// Flaga pozwalająca na włączenie wypisywania szczegółów skonstruowanego planu na konsolę.
        /// Wartość <code>true</code> spoeoduje wypisanie planu.
        /// </summary>
        public bool ShowDebug { get; } = false;

        /// <summary>
        /// Część 1. zadania - zaplanowanie produkcji telewizorów dla pojedynczego kontrahenta.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających maksymalną produkcję i zysk wymagane jest jedynie zwrócenie krotki.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica krotek zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartości pola Quantity oznaczają limit produkcji w danym tygodniu,
        /// a pola Value - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Tablica krotek zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Wartości pola Quantity oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola Value - cenę sprzedaży jednej sztuki.
        /// </param>
        /// <param name="storageInfo">
        /// Krotka zawierająca informacje o magazynie.
        /// Wartość pola Quantity oznacza pojemność magazynu,
        /// a pola Value - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Krotka opisująca wyznaczony plan.
        /// W polu Quantity powinna znaleźć się maksymalna liczba wyprodukowanych telewizorów,
        /// a w polu Value - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public (int Quantity, double Value) CreateSimplePlan((int Quantity, double Value)[] production, (int Quantity, double Value)[] sales,
            (int Quantity, double Value) storageInfo, out (int UnitsProduced, int UnitsSold, int UnitsStored)[] weeklyPlan)
        {
            CheckArguments(production, sales, storageInfo);
            int weeks = production.Length;
            int source, target;

            (Graph fabric_production, Graph fabric_profit) = CreateFlowGraphs(weeks, production, sales, storageInfo, out source, out target);
            (double value, double min_cost, Graph flow) = fabric_production.MinCostFlow(fabric_profit, source, target, true, MaxFlowGraphExtender.PushRelabelMaxFlow); //OriginalDinicBlockingFlow

            weeklyPlan = CreateWeeklyPlan(weeks, flow, source, target);
            return ((int)value, -min_cost);
        }
        public void CheckArguments((int Quantity, double Value)[] production, (int Quantity, double Value)[] sales, (int Quantity, double Value) storageInfo)
        {
            if(production.Length == 0)
                throw new ArgumentException("0 weeks");

            if (production.Length != sales.Length)
                throw new ArgumentException("input array have different length");

            if (production.Any(p => p.Value < 0))
                throw new ArgumentException("fabric has negative value");

            if (storageInfo.Value < 0)
                throw new ArgumentException("storage has negative cost");

            if (sales.Any(s => s.Value < 0))
                throw new ArgumentException("sale has negative cost");

        }

        public (Graph, Graph) CreateFlowGraphs(int weeks, (int Quantity, double Value)[] production, (int Quantity, double Value)[] sales, (int Quantity, double Value) storageInfo, out int source, out int target)
        {
            Graph fabric_production = new AdjacencyListsGraph<HashTableAdjacencyList>(true, weeks + 2);
            Graph fabric_profit = new AdjacencyListsGraph<HashTableAdjacencyList>(true, weeks + 2);

            source = 0;
            target = weeks + 1;

            for (int i = source + 1; i < weeks; i++)
            {
                fabric_production.AddEdge(source, i, production[i - 1].Quantity);
                fabric_production.AddEdge(i, target, sales[i - 1].Quantity);

                fabric_profit.AddEdge(source, i, production[i - 1].Value);
                fabric_profit.AddEdge(i, target, -sales[i - 1].Value);

                fabric_production.AddEdge(i, i + 1, storageInfo.Quantity);
                fabric_profit.AddEdge(i, i + 1, storageInfo.Value);
            }

            // Adding edge cases
            fabric_production.AddEdge(source, weeks, production[weeks - 1].Quantity);
            fabric_production.AddEdge(weeks, target, sales[weeks - 1].Quantity);

            fabric_profit.AddEdge(source, weeks, production[weeks - 1].Value);
            fabric_profit.AddEdge(weeks, target, -sales[weeks - 1].Value);

            return (fabric_production, fabric_profit);
        }

        public (int UnitsProduced, int UnitsSold, int UnitsStored)[] CreateWeeklyPlan(int weeks, Graph flow, int source, int target)
        {
            var weeklyPlan = new (int UnitsProduced, int UnitsSold, int UnitsStored)[weeks];

            for (int i = source + 1; i < weeks; i++)
            {
                weeklyPlan[i - 1].UnitsProduced = (int)flow.GetEdgeWeight(source, i);
                weeklyPlan[i - 1].UnitsSold = (int)flow.GetEdgeWeight(i, target);
                weeklyPlan[i - 1].UnitsStored = (int)flow.GetEdgeWeight(i, i + 1);
            }

            weeklyPlan[weeks - 1].UnitsProduced = (int)flow.GetEdgeWeight(source, weeks);
            weeklyPlan[weeks - 1].UnitsSold = (int)flow.GetEdgeWeight(weeks, target);
            return weeklyPlan;
        }


        /// <summary>
        /// Część 2. zadania - zaplanowanie produkcji telewizorów dla wielu kontrahentów.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających produkcję dającą maksymalny zysk wymagane jest jedynie zwrócenie krotki.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica krotek zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartość pola Quantity oznacza limit produkcji w danym tygodniu,
        /// a pola Value - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Dwuwymiarowa tablica krotek zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Pierwszy wymiar tablicy jest równy liczbie kontrahentów, zaś drugi - liczbie tygodni w planie.
        /// Wartości pola Quantity oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola Value - cenę sprzedaży jednej sztuki.
        /// Każdy wiersz tablicy odpowiada jednemu kontrachentowi.
        /// </param>
        /// <param name="storageInfo">
        /// Krotka zawierająca informacje o magazynie.
        /// Wartość pola Quantity oznacza pojemność magazynu,
        /// a pola Value - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Krotka opisujący wyznaczony plan.
        /// W polu Quantity powinna znaleźć się optymalna liczba wyprodukowanych telewizorów,
        /// a w polu Value - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public (int Quantity, double Value) CreateComplexPlan((int Quantity, double Value)[] production, (int Quantity, double Value)[,] sales, (int Quantity, double Value) storageInfo,
            out (int UnitsProduced, int[] UnitsSold, int UnitsStored)[] weeklyPlan)
        {
            CheckArguments(production, sales, storageInfo);
            int weeks = production.Length;
            int contractors = sales.GetLength(0);

            int source, target;
            (Graph fabric_production, Graph fabric_profit) = CreateFlowGraphs(weeks, production, sales, storageInfo, out source, out target);
            (double value, double min_cost, Graph flow) = fabric_production.MinCostFlow(fabric_profit, source, target, true, MaxFlowGraphExtender.PushRelabelMaxFlow); //OriginalDinicBlockingFlow

            weeklyPlan = CreateWeeklyPlan(weeks, contractors, flow, source, target);
            int quantity = Enumerable.Range(0, weeks).Sum(i => (int)flow.GetEdgeWeight(i + 1, weeks + i + 1));            
            return (quantity, -min_cost);
        }

        public void CheckArguments((int Quantity, double Value)[] production, (int Quantity, double Value)[,] sales, (int Quantity, double Value) storageInfo)
        {
            if (production.Length == 0)
                throw new ArgumentException("0 weeks");

            if(sales.GetLength(0) == 0)
                throw new ArgumentException("0 sales");

            if (production.Any(p => p.Value < 0))
                throw new ArgumentException("fabric has negative value");

            if (storageInfo.Value < 0)
                throw new ArgumentException("storage has negative cost");

            foreach (var s in sales)
            {
                if(s.Value < 0)
                    throw new ArgumentException("sale has negative cost");
            }
        }

        public (Graph, Graph) CreateFlowGraphs(int weeks, (int Quantity, double Value)[] production, (int Quantity, double Value)[,] sales, (int Quantity, double Value) storageInfo, out int source, out int target)
        {
            Graph fabric_production = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 2 * weeks + sales.GetLength(0) + 2);
            Graph fabric_profit = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 2 * weeks + sales.GetLength(0) + 2);

            source = 0;
            target = sales.GetLength(0) + 2 * weeks + 1;

            for (int i = source + 1; i < weeks; i++)
            {
                fabric_production.AddEdge(source, i, production[i - 1].Quantity);
                fabric_production.AddEdge(i, target, production[i - 1].Quantity);

                fabric_profit.AddEdge(source, i, 0);
                fabric_profit.AddEdge(i, target, 0);

                fabric_production.AddEdge(weeks + i, weeks + i + 1, storageInfo.Quantity);
                fabric_profit.AddEdge(weeks + i, weeks + i + 1, storageInfo.Value);                
            }

            fabric_production.AddEdge(source, weeks, production[weeks - 1].Quantity);
            fabric_production.AddEdge(weeks, target, production[weeks - 1].Quantity);

            fabric_profit.AddEdge(source, weeks, 0);
            fabric_profit.AddEdge(weeks, target, 0);

            for (int i = source + 1; i < weeks + 1; i++)
            {
                fabric_production.AddEdge(i, i + weeks, production[i - 1].Quantity);
                fabric_profit.AddEdge(i, i + weeks, production[i - 1].Value);
                for (int j = 0; j < sales.GetLength(0); j++)
                {
                    fabric_production.AddEdge(weeks + i, 2 * weeks + 1 + j, sales[j, i - 1].Quantity);
                    fabric_production.AddEdge(2 * weeks + 1 + j, target, double.MaxValue);

                    fabric_profit.AddEdge(weeks + i, 2 * weeks + 1 + j, -sales[j, i - 1].Value);
                    fabric_profit.AddEdge(2 * weeks + 1 + j, target, 0);
                }
            }

            return (fabric_production, fabric_profit);
        }

        public (int UnitsProduced, int[] UnitsSold, int UnitsStored)[] CreateWeeklyPlan(int weeks,int contractors, Graph flow, int source, int target)
        {
            var weeklyPlan = new (int UnitsProduced, int[] UnitsSold, int UnitsStored)[weeks];

            for (int i = 0; i < weeks; i++)
                weeklyPlan[i].UnitsSold = new int[contractors];

            for (int i = source + 1; i < weeks; i++)
            {
                weeklyPlan[i - 1].UnitsProduced = (int)flow.GetEdgeWeight(i, weeks + i);
                weeklyPlan[i - 1].UnitsStored = (int)flow.GetEdgeWeight(weeks + i, weeks + i + 1);
                for (int j = 0; j < contractors; j++)
                    weeklyPlan[i-1].UnitsSold[j] = (int)flow.GetEdgeWeight(weeks + i, 2 * weeks + j + 1);
            }

            // Egde cases
            weeklyPlan[weeks - 1].UnitsProduced = (int)flow.GetEdgeWeight(weeks, 2*weeks);
            for (int j = 0; j < contractors; j++)
                weeklyPlan[weeks - 1].UnitsSold[j] = (int)flow.GetEdgeWeight(2*weeks, 2*weeks + j + 1);

            return weeklyPlan;
        }

    }
}