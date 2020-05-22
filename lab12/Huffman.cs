
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
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
        public Dictionary<char, string> codes = new Dictionary<char, string>();
        public StringBuilder encoderedString = new StringBuilder(); 
        /// <summary>
        /// Metoda tworzy drzewo Huffmana dla zadanego tekstu
        /// </summary>
        /// <param name="baseText">Zadany tekst</param>
        /// <returns>Drzewo Huffmana</returns>
        public HuffmanNode CreateHuffmanTree(string baseText)
        {
            if (baseText == null || baseText.Length == 0)
                throw new ArgumentNullException("invalid string");

            var counter = Counter(baseText);

            HuffmanPriorityQueue pq = new HuffmanPriorityQueue();

            //var orderedCount = counter.OrderBy(x => x.Value);
            foreach (var c in counter)
            {
                var hn = new HuffmanNode();
                hn.Character = c.Key;
                hn.Frequency = c.Value;
                hn.Left = null;
                hn.Right = null;
                pq.Put(hn);
            }

            while (pq.Count > 1)
            {
                var t1 = pq.Get();
                var t2 = pq.Get();

                var hn = new HuffmanNode();
                hn.Frequency = t1.Frequency + t2.Frequency;
                hn.Left = t1;
                hn.Right = t2;
                pq.Put(hn);
            }

            return pq.Get();
        }

        private Dictionary<char, int> Counter(string text)
        {
            Dictionary<char, int> Counter = new Dictionary<char, int>();
            foreach (var c in text)
            {
                if (Counter.ContainsKey(c))
                    Counter[c]++;
                else
                    Counter[c] = 1;
            }

            return Counter;
        }

        private void GetCodes(HuffmanNode root, StringBuilder code, int level)
        {
            if (root == null)
                return;

            if (level == 0 && root.Left == null && root.Right == null)
            {
                codes[root.Character] = "0";
                return;
            }

            if (root.Left == null && root.Right == null)
            {
                codes[root.Character] = code.ToString();
            }
            else
            {
                GetCodes(root.Left, code.Append("0"), level + 1);
                code[code.Length - 1] = '1';
                GetCodes(root.Right, code, level + 1);
            }
            return;
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
            if (root == null || content == null || content.Length == 0)
                throw new ArgumentNullException("invalid inputs");

            codes = new Dictionary<char, string>();
            StringBuilder code = new StringBuilder("");
            GetCodes(root, code, 0);

            StringBuilder convertedCodes = new StringBuilder();

            foreach (var c in content)
            {
                if (!codes.ContainsKey(c))
                    throw new ArgumentOutOfRangeException("key is not present in a dictionary");
                convertedCodes.Append(codes[c]);
            }

            return new BitArray(convertedCodes.ToString().Select(s => s == '1').ToArray());
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
            if (root == null || encoding == null || encoding.Length == 0)
                throw new ArgumentNullException("invalid inputs");

            encoderedString = new StringBuilder();
            if (root.Left == null && root.Right == null && encoding.Cast<bool>().Any(x => x == true))
                throw new ArgumentException("incorrect encoding");

            Decode(root, root, encoding, 0);
            return encoderedString.ToString();
        }

        private void Decode(HuffmanNode node, HuffmanNode root, BitArray encoding, int idx)
        {
            if (idx == encoding.Length  && (node.Left != null || node.Right != null))
            {
                throw new ArgumentException("encoding doesnt finish at leave");
            }
            if (idx == encoding.Length - 1)
            { 
                encoderedString.Append(node.Character);
                return;
            }

            if (node.Left == null && node.Right == null)
            {
                encoderedString.Append(node.Character);
                Decode(root, root, encoding, idx + 1);
                return;
            }

            if (encoding[idx])
                Decode(node.Right, root, encoding, idx + 1);
            else
                Decode(node.Left, root, encoding, idx + 1);
        }

    }

}
