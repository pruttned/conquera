using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// Read-only array of short indices
    /// </summary>
    internal class ShortIndexReadOnlyArray : ReadOnlyArray<ushort>
    {
        #region Methods

        /// <summary>
        /// Creates a new read-only array from the existing collection. Items of the given collection
        /// are copied.
        /// </summary>
        /// <param name="items"></param>
        public ShortIndexReadOnlyArray(ushort[] items)
            : base(items)
        {
        }

        /// <summary>
        /// Creates IntIndexReadOnlyArray from a given index buffer
        /// </summary>
        /// <param name="srcIndexBuffer"></param>
        /// <returns></returns>
        public static ShortIndexReadOnlyArray FromIndexBuffer(IndexBuffer srcIndexBuffer)
        {
            if (srcIndexBuffer.IndexElementSize == IndexElementSize.ThirtyTwoBits)
            {
                throw new ArgumentException("IndexBuffer must have 16bit indices");
            }

            ushort[] srcIndices = new ushort[srcIndexBuffer.SizeInBytes / 2];
            srcIndexBuffer.GetData<ushort>(srcIndices);

            return new ShortIndexReadOnlyArray(srcIndices, false);

        }

        /// <summary>
        /// Creates a new read-only array from the existing collection. 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="createCopy">- Whether should be all items copied or no</param>
        private ShortIndexReadOnlyArray(ushort[] items, bool createCopy)
            : base(items, createCopy)
        {
        }

        #endregion Methods
    }
}
