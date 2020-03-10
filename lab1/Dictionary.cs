
using System;

namespace ASD
{

    // w tym interfejsie nie wolno nic zmieniać !!!
    public interface IListDictionary
    {
        bool Add(int v);
        bool Search(int v);
        bool Delete(int v);
        int[] ToArray();
    }

    // w tej strukturze nie wolno nic zmieniać !!!
    [Serializable]
    public struct OperationInfo
    {
        public char oper;
        public int val;
        public bool expRes;
        public OperationInfo(char op, int v, bool e)
        {
            oper = op;
            val = v;
            expRes = e;
        }
    }

    // w tej klasie nie wolno nic zmieniać !!!
    public class OperationExecutor : MarshalByRefObject
    {

        public bool[] Execute(IListDictionary TestedDict, OperationInfo[] operations)
        {
            bool[] res = new bool[operations.Length];
            for (int i = 0; i < operations.Length; ++i)
                switch (operations[i].oper)
                {
                    
                    case 'a':
                        res[i] = TestedDict.Add(operations[i].val);
                        break;
                    case 's':
                        res[i] = TestedDict.Search(operations[i].val);
                        break;
                    case 'd':
                        res[i] = TestedDict.Delete(operations[i].val);
                        break;
                }
            return res;
        }

    }

    // w tej klasie nie wolno nic zmieniać !!!
    public abstract class AbstractList : MarshalByRefObject, IListDictionary
    {
        protected class Elem
        {
            public int Val;
            public Elem Next;
            public Elem(int v, Elem n = null) { Val = v; Next = n; }
        }

        protected Elem head = null;

        public abstract bool Add(int v);

        public abstract bool Search(int v);

        public abstract bool Delete(int v);

        public int[] ToArray()
        {
            var list = new System.Collections.Generic.List<int>();
            for (Elem e = head; e != null; e = e.Next)
                list.Add(e.Val);
            return list.ToArray();
        }

    }

    public class SimpleList : AbstractList
    {
        public override bool Add(int v)
        {
            return true;
        }

        public override bool Search(int v)
        {
            return false;
        }

        public override bool Delete(int v)
        {
            return false;
        }
    }

    public class SortedList : AbstractList
    {
        public override bool Add(int v)
        {
            return false;
            
        }

        public override bool Search(int v)
        {
            return false;
        }

        public override bool Delete(int v)
        {
            return false;
        }

    }

    public class MoveToHeadList : AbstractList
    {

        public override bool Add(int v)
        {
            return true;
        }

        public override bool Search(int v)
        {
            return false;
        }

        public override bool Delete(int v)
        {
            return false;
        }

    }

}
