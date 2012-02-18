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
    /// Collections where each item has assigned auto-generated Id. 
    /// Item order depends on its Id. Unused Ids are reused.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdValueCollection<T> : IEnumerable<T> where T : class
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
            private IdValueCollection<T> mCollection;
            
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
            internal Enumerator(IdValueCollection<T> collection)
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

        #endregion Fields

        #region Properties

        /// <summary>
        /// Number of items
        /// </summary>
        public int Count
        {
            get { return mItemCnt; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">- Item doesn't exists</exception>
        public T this[int id]
        {
            get
            {
                T item = GetItem(id);
                if (null == item)
                {
                    throw new ArgumentException(string.Format("Item with id '{0}' doesn't exists", id));
                }
                return item;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public IdValueCollection()
        {
            mItems = new List<T>();
            mItemCnt = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public IdValueCollection(int capacity)
        {
            mItems = new List<T>(capacity);
            mItemCnt = 0;
        }

        /// <summary>
        /// Adds new item to the collection (item can't be null)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Id of the item</returns>
        public int Add(T item)
        {
            if(null == item)
            {
                throw new ArgumentNullException("item");
            }

            if (mItems.Count != Count) //has free space
            {
                for (int i = 0; i < mItems.Count; ++i)
                {
                    if (null == mItems[i]) //found empty space
                    {
                        mItems[i] = item;
                        mItemCnt++;
                        return i;
                    }
                }
            }

            //add to end
            mItems.Add(item);
            mItemCnt++;

            return mItems.Count - 1; 
        }

        /// <summary>
        /// Removes item with a given Id from the collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Removed item or null</returns>
        public T Remove(int id)
        {
            if (0 > id || mItems.Count <= id)
            {
                return null;
            }

            T item = mItems[id];
            mItems[id] = null;

            if (null != item)
            {
                mItemCnt--;
            }

            return item;
        }

        /// <summary>
        /// Gets the item with a given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Item or null</returns>
        public T GetItem(int id)
        {
            if (0 > id || mItems.Count <= id)
            {
                return null;
            }

            return mItems[id];
        }

        /// <summary>
        /// Gets the id of a given item
        /// </summary>
        /// <param name="item">Can't be null</param>
        /// <returns>Id or -1</returns>
        public int GetItemId(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException("item");
            } 
            return mItems.IndexOf(item);
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
            mItemCnt = 0;
        }

        #endregion Methods

        #region IEnumerable
        
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

        #endregion IEnumerable
    }
}
