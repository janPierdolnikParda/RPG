using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public class PriorityQueue<E>
    {
        List<Pair<float, E>> Tree = new List<Pair<float, E>>();

        public void Swap(int i, int j)
        {
            Pair<float, E> tmp = Tree[i];
            Tree[i] = Tree[j];
            Tree[j] = tmp;
        }

        public void Push(float p, E e)
        {
            Tree.Add(new Pair<float, E>(p, e));
            int i = Tree.Count - 1;
            while (i != 0 && Tree[i / 2].first > Tree[i].first)
            {
                Swap(i, i / 2);
                i /= 2;
            }
        }

        public E Pop()
        {
            E popped = Tree[0].second;

            Swap(0, Tree.Count - 1);
            Tree.RemoveAt(Tree.Count - 1);
            int i = 0;
            while (true)
            {
                int lower = i;
                if (i * 2 < Tree.Count && Tree[i * 2].first < Tree[lower].first)
                    lower = i * 2;
                if (i * 2 + 1 < Tree.Count && Tree[i * 2 + 1].first < Tree[lower].first)
                    lower = i * 2 + 1;
                if (lower != i)
                {
                    Swap(i, lower);
                    i = lower;
                }
                else
                    break;
            }
            return popped;
        }

        public void Clear()
        {
            Tree.Clear();
        }

        public bool IsEmpty()
        {
            return Tree.Count == 0;
        }
    }
}
