using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Tools
{

    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Fields

        /// <summary>
        /// Internal storage
        /// </summary>
        Dictionary<TKey, TValue> mDictionary;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number elements
        /// </summary>
        public int Count
        {
            get { return mDictionary.Count; }
        }

        /// <summary>
        /// Gets a value specified by its key
        /// </summary>
        /// <param name="key">Value's key</param>
        /// <returns>Value with specified key</returns>
        public TValue this[TKey key]
        {
            get { return mDictionary[key]; }
        }

        /// <summary>
        /// Gets the key collection
        /// </summary>
        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get { return mDictionary.Keys; }
        }

        /// <summary>
        /// Gets the value collection
        /// </summary>
        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get { return mDictionary.Values; }
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)mDictionary).Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return mDictionary[key];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { throw new NotImplementedException(); }
        }
        
        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="manualParams">- Dictionary that will be used as a internal storage (no copying is performed)</param>
        public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)
        {
            if (null == dictionary) throw new ArgumentNullException("dictionary");
            mDictionary = dictionary;
        }

        /// <summary>
        /// Tries to get a value by its key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return mDictionary.TryGetValue(key, out value);
        }

        #region IEnumerable

        /// <summary>
        /// Gets the value key-value pair enumerator
        /// </summary>
        /// <returns>Key-value pair enumerator</returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return mDictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets the value key-value pair enumerator
        /// </summary>
        /// <returns>Key-value pair enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mDictionary.GetEnumerator();
        }

        #endregion IEnumerable

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        public bool ContainsKey(TKey key)
        {
            return mDictionary.ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return mDictionary.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)mDictionary).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)mDictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        #endregion Methods
    }
}
