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
using Ale.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// Read-only array of GeometryBatchVertex
    /// </summary>
    internal class SimpleVertexReadOnlyArray : ReadOnlyArray<SimpleVertex>, IEnumerable<Vector3>, ISimpleVertexArrayCollection
    {
        #region Properties

        #region IGeometryBatchVertexArrayCollection

        /// <summary>
        /// Gets the number of vertices
        /// </summary>
        int ISimpleVertexArrayCollection.VertexCount
        {
            get { return Count; }
        }

        /// <summary>
        /// Gets the internal array of vertices
        /// </summary>
        SimpleVertex[] ISimpleVertexArrayCollection.Vertices
        {
            get { return Items; }
        }

        #endregion IGeometryBatchVertexArrayCollection

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a new read-only array from the existing collection. Items of the given collection
        /// are copied.
        /// </summary>
        /// <param name="items"></param>
        public SimpleVertexReadOnlyArray(SimpleVertex[] items)
            : base(items)
        {
        }

        /// <summary>
        /// Creates GeometryBatchVertexReadOnlyArray from a given vertex buffer
        /// </summary>
        /// <param name="srcVertexBuffer"></param>
        /// <param name="srcVertexDeclaration">- Vertex declaration of a given vertex buffer</param>
        /// <returns></returns>
        public static SimpleVertexReadOnlyArray FromVertexBuffer(VertexBuffer srcVertexBuffer, VertexDeclaration srcVertexDeclaration)
        {
            int srcVertexStride = srcVertexDeclaration.GetVertexStrideSize(0);
            int vCnt = srcVertexBuffer.SizeInBytes / srcVertexStride;

            if (SimpleVertex.IsGeometryBatchVertex(srcVertexDeclaration)) //same format - no conversion needed
            {
                SimpleVertex[] srcVertices = new SimpleVertex[vCnt];
                srcVertexBuffer.GetData<SimpleVertex>(srcVertices);

                return new SimpleVertexReadOnlyArray(srcVertices);
            }
            else //with vb format conversion    - based on http://www.ziggyware.com/readarticle.php?article_id=95
            {
                VertexElement[] destVertexElements = SimpleVertex.VertexElements;
                VertexElement[] srcVertexElements = srcVertexDeclaration.GetVertexElements();

                //stores VertexElement's offsets
                int[] srcVertexElementOffsets = new int[destVertexElements.Length]; //0=position 1=normal 2=uv 

                for (int destI = 0; destI < destVertexElements.Length; ++destI) //Find mappings
                {
                    VertexElement destVertexElement = destVertexElements[destI];
                    bool found = false;
                    for (int srcI = 0; !found && srcI < srcVertexElements.Length; ++srcI)
                    {
                        VertexElement srcVertexElement = srcVertexElements[srcI];

                        if (0 == srcVertexElement.Stream &&
                            destVertexElement.VertexElementFormat == srcVertexElement.VertexElementFormat &&
                            destVertexElement.UsageIndex == srcVertexElement.UsageIndex &&
                            destVertexElement.VertexElementUsage == srcVertexElement.VertexElementUsage)
                        {
                            srcVertexElementOffsets[destI] = srcVertexElement.Offset;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        throw new ArgumentException(string.Format("Vertex buffer cannot be converted to the GeometryBatchVertexReadOnlyArray because '{0} {1}{2}' element is missing in its vertexDeclaration",
                            destVertexElements[destI].VertexElementFormat, destVertexElements[destI].VertexElementUsage, destVertexElements[destI].UsageIndex));
                    }
                }

                //convert
                byte[] srcData = new byte[srcVertexStride];
                srcVertexBuffer.GetData<byte>(srcData);

                SimpleVertex[] destVertices = new SimpleVertex[vCnt];

                for (int i = 0; i < vCnt; ++i)
                {
                    destVertices[i].Position = VectorConverter.GetVector3FromBytes(srcData, srcVertexElementOffsets[0]);
                    destVertices[i].Normal = VectorConverter.GetVector3FromBytes(srcData, srcVertexElementOffsets[1]);
                    destVertices[i].Uv = VectorConverter.GetVector2FromBytes(srcData, srcVertexElementOffsets[2]);
                }
                srcData = null;
                
                return new SimpleVertexReadOnlyArray(destVertices, false);
            }
        }

        /// <summary>
        /// Computes bounds of the vertex collection
        /// </summary>
        /// <returns></returns>
        public BoundingSphere ComputeBounds()
        {
            //BoundingBox -> BoundingSphere produces better results then BoundingSphere.CreateMerged or BoundingSphereBoundingSphere.CreateFromPoints
            return BoundingSphere.CreateFromBoundingBox(BoundingBox.CreateFromPoints(this));
        }

        #region IEnumerable<Vector3>

        /// <summary>
        /// Gets the enumerator that enumerates trough positions of the stored vertices
        /// </summary>
        /// <returns></returns>
        IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator()
        {
            return new SimpleVertexPositionEnumerator(this);
        }

        #endregion IEnumerable<Vector3>

        /// <summary>
        /// Creates a new read-only array from the existing collection
        /// </summary>
        /// <param name="items"></param>
        /// <param name="createCopy">- Whether should be all items copied or no</param>
        private SimpleVertexReadOnlyArray(SimpleVertex[] items, bool createCopy)
            : base(items, createCopy)
        {
        }

        #endregion Methods
    }
}
