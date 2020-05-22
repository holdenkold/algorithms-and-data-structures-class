
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASD_2020_12
{
    /// <summary>
    ///     Kolejka priorytetowa węzłów Huffmana. Metoda Get pobiera (i usuwa z kolejki) element o minimalnej częstotliwości
    /// </summary>
    public class HuffmanPriorityQueue : MarshalByRefObject
    {
        private readonly ASD.Graphs.PriorityQueue<HuffmanNode, long> queue = 
            new ASD.Graphs.PriorityQueue<HuffmanNode, long>((lhs, rhs) => lhs.Key.Frequency < rhs.Key.Frequency);

        public bool Put(HuffmanNode node) => queue.Put(node, node.Frequency); // Dodaje węzeł Huffmana do kolejki

        public HuffmanNode Get() => queue.Get(); // Pobiera węzeł Huffmana o najmniejszej częstotliwości

        public int Count => queue.Count; // Zwraca liczbę węzłów w kolejce
    }

    [Serializable]
    public class HuffmanNode
    {
        public char Character { get; set; }
        public long Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }
    }

    // Od tego miejsca można modyfikować
    // zaklęcia MarshalByRefObject nie ruszamy

    // można dodawać prywatne metody pomocnicze

    public class Huffman : MarshalByRefObject
    {

    // ETap I

        /// <summary>
        /// Metoda tworzy drzewo Huffmana dla zadanego tekstu
        /// </summary>
        /// <param name="baseText">Zadany tekst</param>
        /// <returns>Drzewo Huffmana</returns>
        public HuffmanNode CreateHuffmanTree(string baseText)
        {
            return null;
        }

    // Etap II

        /// <summary>
        /// Metoda dokonuje kompresji Huffmana zadanego tekstu
        /// </summary>
        /// <param name="root">Drzewo Huffmana wykorzystywane do kompresji</param>
        /// <param name="content">Zadany tekst</param>
        /// <returns>Skompresowany tekst</returns>
        public BitArray Compress(HuffmanNode root, string content)
        {
            return null;
        }

    // Etap III

        /// <summary>
        /// Metoda dokonuje dekompresji tekstu skompresowanego metodą Huffmana
        /// </summary>
        /// <param name="root">Drzewo Huffmana wykorzystywane do dekompresji</param>
        /// <param name="encoding">Skompresowany tekst</param>
        /// <returns>Odtworzony tekst</returns>
        public string Decompress(HuffmanNode root, BitArray encoding)
        {
            return null;
        }

    }

}
