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
