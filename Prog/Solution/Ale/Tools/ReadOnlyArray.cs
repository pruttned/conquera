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
