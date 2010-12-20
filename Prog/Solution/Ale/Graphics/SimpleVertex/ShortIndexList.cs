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
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// List of indices
    /// </summary>
    internal class ShortIndexList : FastUnsafeList<ushort>
    {
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="capacity">- Init capacity</param>
        public ShortIndexList(int capacity)
            :base(capacity)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ShortIndexList()
            : base()
        {
        }

        /// <summary>
        /// Adds new range of indices and adds baseIndex to each index
        /// </summary>
        /// <param name="baseIndex">- Base index that is add to each index</param>
        /// <param name="indices">- Array of indices</param>
        /// <param name="startIndex"></param>
        /// <param name="elmCnt"></param>
        public void AddRange(IList<ushort> indices, int startIndex, int elmCnt, int baseIndex)
        {
            int destI = Count;
            Resize(elmCnt);
            int srcEnd = elmCnt + startIndex;

            ushort[] internalArray = InternalArray;
            for (int srcI = startIndex; srcI < srcEnd; ++destI, ++srcI)
            {
                internalArray[destI] = (ushort)(indices[srcI] + baseIndex);
            }
        }

        /// <summary>
        /// Creates IndexBuffer from stored indices
        /// </summary>
        /// <param name="bufferUsage"></param>
        /// <param name="graphicsDevice"?
        /// <returns></returns>
        public IndexBuffer CreateIndexBuffer(GraphicsDevice graphicsDevice, BufferUsage bufferUsage)
        {
            IndexBuffer ib = new IndexBuffer(graphicsDevice, Count * 2, bufferUsage, IndexElementSize.SixteenBits);
            ib.SetData<ushort>(InternalArray, 0, Count);
            return ib;
        }

        /// <summary>
        /// Creates IndexBuffer from stored indices
        /// </summary>
        /// <param name="bufferUsage"></param>
        /// <param name="graphicsDevice"?
        /// <returns></returns>
        public IndexBuffer CreateIndexBuffer(GraphicsDevice graphicsDevice, bool usePool)
        {
            IndexBuffer ib;
            if (usePool)
            {
                ib = ShortIndexBufferManager.Instance.GetBuffer(Count, graphicsDevice);
            }
            else
            {
                ib = new IndexBuffer(graphicsDevice, Count * 2, BufferUsage.None, IndexElementSize.SixteenBits);
            }
            ib.SetData<ushort>(InternalArray, 0, Count);
            return ib;
        }

        #endregion Methods
    }
}
