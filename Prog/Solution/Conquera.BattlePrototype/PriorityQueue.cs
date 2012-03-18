using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquera.BattlePrototype
{
    class PriorityQueue<T> : ICollection<T> where T : IComparable<T>
    {
        List<T> mInnerList;

        public int Count
        {
            get { return mInnerList.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; ; }
        }

        public PriorityQueue()
        {
            mInnerList = new List<T>();
        }

        public PriorityQueue(int capacity)
        {
            mInnerList = new List<T>(capacity);
        }

        public void Push(T item)
        {
            if (null == item) throw new ArgumentNullException("item");

            mInnerList.Insert(FindPosFor(item), item);
        }

        public T Pop()
        {
            int lastIndex = mInnerList.Count - 1;
            T item = mInnerList[lastIndex];
            mInnerList.RemoveAt(lastIndex);
            return item;
        }

        public void Clear()
        {
            mInnerList.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return mInnerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mInnerList.GetEnumerator();
        }

        public void Add(T item)
        {
            Push(item);
        }

        public bool Contains(T item)
        {
            return mInnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            mInnerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return mInnerList.Remove(item);
        }

        private int FindPosFor(T item)
        {
            if (0 == mInnerList.Count)
            {
                return 0;
            }
            int cnt = mInnerList.Count;
            if (10 > cnt)
            {
                for (int i = 0; i < cnt; ++i)
                {
                    int cmpResult = item.CompareTo(mInnerList[i]);
                    if (0 == cmpResult)
                    {
                        return i;
                    }
                    else
                    {
                        if (-1 == cmpResult)
                        {
                            return i;
                        }
                    }
                }
                return mInnerList.Count;
            }
            else
            {
                int start = 0;
                //binary search
                int end = cnt;
                while (start <= end)
                {
                    int middle = start + ((end - start) >> 1);

                    if (cnt <= middle)
                    {
                        end = middle - 1;
                    }
                    else
                    {
                        int cmpResult = item.CompareTo(mInnerList[middle]);
                        if (0 == cmpResult)
                        {
                            return middle;
                        }
                        if (1 == cmpResult)
                        {
                            start = middle + 1;
                        }
                        else
                        {
                            end = middle - 1;
                        }
                    }
                }
                return start;
            }
        }
    }

}
