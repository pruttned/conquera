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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Tools
{
    /// <summary>
    /// Read-only list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReadOnlyArray<T> : IEnumerable<T>
    {
        #region Fields

        /// <summary>
        /// Items
        /// </summary>
        T[] mItems;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number of items
        /// </summary>
        public int Count
        {
            get { return mItems.Length; }
        }

        /// <summary>
        /// Gets an items under a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return mItems[index]; }
        }

        /// <summary>
        /// Gets the internal storage of items
        /// </summary>
        internal T[] Items
        {
            get { return mItems; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a new read-only array from the existing collection. Items of the given collection
        /// are copied.
        /// </summary>
        /// <param name="items"></param>
        public ReadOnlyArray(T[] items)
            : this(items, true)
        {
        }

        /// <summary>
        /// Creates a new read-only array from the existing collection. 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="createCopy">- Whether should be all items copied or no</param>
        internal ReadOnlyArray(T[] items, bool createCopy)
        {
            if (createCopy)
            {
                mItems = new T[items.Length];
                Array.Copy(items, mItems, items.Length);
            }
            else
            {
                mItems = items;
            }
        }

        #region IEnumerable

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)mItems).GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion IEnumerable

        #endregion Methods
    }
}
