using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Enumerator that enumerates trough positions of the vertices stored in a given GeometryBatchVertex list
    /// </summary>
    struct SimpleVertexPositionEnumerator : IEnumerator<Vector3>
    {
        #region Fields

        /// <summary>
        /// Collection that is being iterated
        /// </summary>
        private ISimpleVertexArrayCollection mCollection;

        /// <summary>
        /// Index of the next item
        /// </summary>
        private int mNextIndex;

        /// <summary>
        /// Current item
        /// </summary>
        private Vector3 mCurrent;

        #endregion Fields

        #region Properties

        #region IEnumerator

        /// <summary>
        /// Gets the current item
        /// </summary>
        public Vector3 Current
        {
            get { return mCurrent; }
        }

        /// <summary>
        /// Gets the current item
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return mCurrent; }
        }

        #endregion IEnumerator

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="array"></param>
        public SimpleVertexPositionEnumerator(ISimpleVertexArrayCollection collection)
        {
            mCollection = collection;
            mNextIndex = 0;
            mCurrent = Vector3.Zero;
        }

        #region IEnumerator

        /// <summary>
        /// Moves to nex item
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (mNextIndex < mCollection.VertexCount)
            {
                mCurrent = mCollection.Vertices[mNextIndex].Position; //Array indexer is faster because GeometryBatchVertex is value type
                mNextIndex++;
                return true;
            }

            if (mNextIndex == mCollection.VertexCount)
            {
                mCurrent = Vector3.Zero;
                mNextIndex++; // mArray.Count + 1 
            }

            return false;
        }

        /// <summary>
        /// Resets the iterator
        /// </summary>
        public void Reset()
        {
            mNextIndex = 0;
            mCurrent = Vector3.Zero;
        }

        #endregion IEnumerator

        #region IDisposable

        /// <summary>
        /// Nothing
        /// </summary>
        public void Dispose()
        {
        }

        #endregion IDisposable

        #endregion Methods
    }
}
