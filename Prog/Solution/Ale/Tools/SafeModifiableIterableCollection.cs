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
using System.Collections;
using System.Runtime.InteropServices;

namespace Ale.Tools
{
    /// <summary>
    /// Collection that can be modified during iteration.
    /// Removeing item sets slot to null. Its neceseary to call Tidy to remove empty slots.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SafeModifiableIterableCollection<T> : IEnumerable<T> where T : class
    {
        #region Nested Types
        
        /// <summary>
        /// Value enumerator
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            #region Fields

            /// <summary>
            /// 
            /// </summary>
            private SafeModifiableIterableCollection<T> mCollection;
            
            /// <summary>
            /// 
            /// </summary>
            private T mCurrent;

            /// <summary>
            /// 
            /// </summary>
            private int mId;

            /// <summary>
            /// Number of items alredy traversed
            /// </summary>
            private int mTraversedItemCnt;

            #endregion Fields

            #region Properties

            /// <summary>
            /// 
            /// </summary>
            public T Current 
            { 
                get {return mCurrent;} 
            }

            /// <summary>
            /// Id
            /// </summary>
            public int Id
            {
                get { return mId; }
            }

            /// <summary>
            /// 
            /// </summary>
            object IEnumerator.Current 
            {
                get { return mCurrent; }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="collection"></param>
            internal Enumerator(SafeModifiableIterableCollection<T> collection)
            {
                mCollection = collection;
                mTraversedItemCnt = 0;
                mCurrent = null;
                mId = -1;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (mCollection.Count > mTraversedItemCnt)
                {
                    while (null == mCollection.GetItem(++mId)) ;
                    
                    mTraversedItemCnt++;
                    mCurrent = mCollection.GetItem(mId);

                    return true;
                }

                mCurrent = null;
                mId = -1;
                return false;
            }

            /// <summary>
            /// 
            /// </summary>
            void IEnumerator.Reset()
            {
                mTraversedItemCnt = 0;
                mCurrent = null;
                mId = -1;
            }

            #endregion Methods
        }

        #endregion Nested Types

        #region Fields

        /// <summary>
        /// Items
        /// </summary>
        private List<T> mItems;

        /// <summary>
        /// Number of items
        /// </summary>
        private int mItemCnt;

        private bool mUntidy = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Number of items
        /// </summary>
        public int Count
        {
            get { return mItemCnt; }
        }

        #endregion Properties


        /// <summary>
        /// 
        /// </summary>
        public SafeModifiableIterableCollection()
        {
            mItems = new List<T>();
            mItemCnt = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public SafeModifiableIterableCollection(int capacity)
        {
            mItems = new List<T>(capacity);
            mItemCnt = 0;
        }

        /// <summary>
        /// Adds new item to the collection (item can't be null)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Id of the item</returns>
        public void Add(T item)
        {
            if(null == item)
            {
                throw new ArgumentNullException("item");
            }

            mItems.Add(item);
            mItemCnt++;
        }

        public bool Remove(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException("item");
            }

            int index = mItems.IndexOf(item);
            if (-1 < index)
            {
                mItems[index] = null;
                mItemCnt--;
                mUntidy = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the item 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Item or null</returns>
        public T GetItem(int index)
        {
            if (0 > index || mItems.Count <= index)
            {
                return null;
            }

            return mItems[index];
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
            mItemCnt = 0;
        }

        public void Tidy()
        {
            if (mUntidy)
            {
                //clear removed listeners
                for (int i = mItems.Count - 1; i >= 0; --i)
                {
                    if (null == mItems[i])
                    {
                        mItems.RemoveAt(i);
                    }
                }

                mUntidy = false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

    }
}

