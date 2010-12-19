//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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

using System;
using System.Collections.Generic;

namespace Ale.Tools
{
    /// <summary>
    /// List whose main purpose is to get a fast conversion to an array because Xna SetData methods
    /// takes only arrays. 
    /// Use InternalArray property with caution (see InternalArray's summary).
    /// </summary>
    /// <typeparam name="T">Type of the list's element</typeparam>
    internal class FastUnsafeList<T> : IEnumerable<T>
    {
        #region Fields

        /// <summary>
        /// Default capacity
        /// </summary>
        private const int DefaultCapacity = 10;

        /// <summary>
        /// Internal array of elements
        /// </summary>
        T[] mItems;

        /// <summary>
        /// Actual size of the list
        /// </summary>
        int mItemCnt = 0;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the size of the list
        /// </summary>
        public int Count
        {
            get { return mItemCnt; }
        }

        /// <summary>
        /// Gets the actual internal array. 
        /// Length of the array may be biger then the Count so don't use iterators!
        /// Array may become invalid after calling an add method!
        /// </summary>
        public T[] InternalArray
        {
            get { return mItems; }
        }

        /// <summary>
        /// Gets/sets the element's value
        /// </summary>
        /// <param name="index">- Index of the element (not checked)</param>
        /// <returns>Element's value</returns>
        public T this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="capacity">- Init capacity</param>
        public FastUnsafeList(int capacity)
        {
            mItems = new T[capacity];
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public FastUnsafeList()
        {
            mItems = new T[DefaultCapacity];
        }

        /// <summary>
        /// Adds a new item to the list
        /// </summary>
        /// <param name="item">- Item that should be added</param>
        public void Add(T item)
        {
            if (mItems.Length == mItemCnt)
            {
                this.EnsureCapacity(mItemCnt + 1);
            }
            mItems[mItemCnt++] = item;
        }

        /// <summary>
        /// Clears the list (doesn't change its capacity)
        /// </summary>
        public void Clear()
        {
            if (0 < mItemCnt)
            {
                Array.Clear(mItems, 0, mItemCnt);
                mItemCnt = 0;
            }
        }

        #region IEnumerable<T>

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)mItems.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion IEnumerable<T>

        /// <summary>
        /// Resize the list (new items have default value - 0/null/..) but never shrink
        /// </summary>
        /// <param name="sizeInc">- New capacity</param>
        protected void Resize(int sizeInc)
        {
            if (sizeInc < 0)
            {
                throw new ArgumentException("sizeInc can't be lower then 0");
            }

            int newItemCnt = Count + sizeInc;
            EnsureCapacity(newItemCnt);
            mItemCnt = newItemCnt;
        }

        /// <summary>
        /// Updates the capacity of the list
        /// </summary>
        /// <param name="min">- Min capacity</param>
        private void EnsureCapacity(int min)
        {
            if (min > mItems.Length)
            {
                int newCapacity = Math.Max((0 == mItems.Length) ? DefaultCapacity : (mItems.Length * 2), min);

                T[] newItems = new T[newCapacity];
                if (0 < mItemCnt)
                {
                    Array.Copy(mItems, 0, newItems, 0, mItemCnt);
                }
                mItems = newItems;
            }
        }

        #endregion Methods
    }

}
