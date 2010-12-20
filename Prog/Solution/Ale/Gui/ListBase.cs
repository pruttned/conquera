//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

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
