using System.Collections;
using System.Collections.Generic;

namespace Ale.Gui
{
    public class ListBase<T> : IList<T>
    {
        private List<T> mInnerList = new List<T>();

        public T this[int index]
        {
            get { return mInnerList[index]; }
            set { mInnerList[index] = value; }
        }

        public int Count
        {
            get { return mInnerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return mInnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mInnerList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return mInnerList.IndexOf(item);
        }

        public virtual void Add(T item)
        {
            mInnerList.Add(item);
        }

        public virtual void Insert(int index, T item)
        {
            mInnerList.Insert(index, item);
        }

        public virtual bool Remove(T item)
        {
            return mInnerList.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            mInnerList.RemoveAt(index);
        }

        public virtual void Clear()
        {
            mInnerList.Clear();
        }

        public bool Contains(T item)
        {
            return mInnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            mInnerList.CopyTo(array, arrayIndex);
        }
    }
}