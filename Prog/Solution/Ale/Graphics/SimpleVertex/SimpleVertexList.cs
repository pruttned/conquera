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
using Ale.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Ale.Graphics
{
    /// <summary>
    /// List of vertices (GeometryBatchVertex)
    /// </summary>
    internal class SimpleVertexList : FastUnsafeList<SimpleVertex>, IEnumerable<Vector3>, ISimpleVertexArrayCollection
    {
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="capacity">- Init capacity</param>
        public SimpleVertexList(int capacity)
            :base(capacity)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public SimpleVertexList()
            : base()
        {
        }

        /// <summary>
        /// Adds new range of vertices
        /// </summary>
        /// <param name="vertices">- Array of vertices</param>
        /// <param name="startIndex"></param>
        /// <param name="elmCnt"></param>
        public void AddRange(IList<SimpleVertex> vertices, int startIndex, int elmCnt)
        {
            int destI = Count;
            Resize(elmCnt);
            int srcEnd = elmCnt + startIndex;
            SimpleVertex[] internalArray = InternalArray;
            
            for (int srcI = startIndex; srcI < srcEnd; ++destI, ++srcI)
            {
                internalArray[destI] = vertices[srcI];
            }
        }

        /// <summary>
        /// Adds new range of vertices
        /// </summary>
        /// <param name="vertices">- Array of vertices</param>
        /// <param name="worldTransformation">- Transformation of each vertex</param>
        /// <param name="startIndex"></param>
        /// <param name="elmCnt"></param>
        public void AddRange(SimpleVertex[] vertices, int startIndex, int elmCnt, ref Matrix worldTransformation)
        {
            int destI = Count;
            Resize(elmCnt);
            int srcEnd = elmCnt + startIndex;
            SimpleVertex[] internalArray = InternalArray;
            for (int srcI = startIndex; srcI < srcEnd; ++destI, ++srcI)
            {
                internalArray[destI] = vertices[srcI];
                Vector3.Transform(ref vertices[srcI].Position, ref worldTransformation, out internalArray[destI].Position);
                Vector3.TransformNormal(ref vertices[srcI].Normal, ref worldTransformation, out internalArray[destI].Normal);
 
                internalArray[destI].Normal.Normalize();
            }
        }

        /// <summary>
        /// Adds new range of vertices
        /// </summary>
        /// <param name="vertices">- Array of vertices</param>
        /// <param name="worldTransformation">- Transformation of each vertex</param>
        /// <param name="startIndex"></param>
        /// <param name="elmCnt"></param>
        public void AddRange(SimpleVertex[] vertices, int startIndex, int elmCnt, Matrix worldTransformation)
        {
            AddRange(vertices, startIndex, elmCnt, ref worldTransformation);
        }

        /// <summary>
        /// Adds new range of vertices
        /// </summary>
        /// <param name="vertices">- Array of vertices</param>
        /// <param name="worldTransformation">- Translation of each vertex</param>
        /// <param name="startIndex"></param>
        /// <param name="elmCnt"></param>
        public void AddRange(SimpleVertex[] vertices, int startIndex, int elmCnt, ref Vector3 worldTranslation)
        {
            int destI = Count;
            Resize(elmCnt);
            int srcEnd = elmCnt + startIndex;
            SimpleVertex[] internalArray = InternalArray;
            for (int srcI = startIndex; srcI < srcEnd; ++destI, ++srcI)
            {
                internalArray[destI] = vertices[srcI];
                internalArray[destI].Position += worldTranslation;
            }
        }
        
        /// <summary>
        /// Adds new range of vertices
        /// </summary>
        /// <param name="vertices">- Array of vertices</param>
        /// <param name="worldTransformation">- Translation of each vertex</param>
        /// <param name="startIndex"></param>
        /// <param name="elmCnt"></param>
        public void AddRange(SimpleVertex[] vertices, int startIndex, int elmCnt, Vector3 worldTranslation)
        {
            AddRange(vertices, startIndex, elmCnt, ref worldTranslation);
        }

        /// <summary>
        /// Creates VertexBuffer from stored vertices
        /// </summary>
        /// <param name="bufferUsage"></param>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public VertexBuffer CreateVertexBuffer(GraphicsDevice graphicsDevice, BufferUsage bufferUsage)
        {
            VertexBuffer vb = new VertexBuffer(graphicsDevice, Count * SimpleVertex.SizeInBytes, bufferUsage);
            vb.SetData<SimpleVertex>(InternalArray, 0, Count);
            return vb;            
        }

        /// <summary>
        /// Creates VertexBuffer from stored vertices
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="usePool"></param>
        /// <returns></returns>
        internal VertexBuffer CreateVertexBuffer(GraphicsDevice graphicsDevice, bool usePool)
        {
            VertexBuffer vb;
            if (usePool)
            {
                vb = SimpleVertexBufferManager.Instance.GetBuffer(Count, graphicsDevice);
            }
            else
            {
                vb = new VertexBuffer(graphicsDevice, Count * SimpleVertex.SizeInBytes, BufferUsage.None);
            }
            vb.SetData<SimpleVertex>(InternalArray, 0, Count);
            return vb;
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
            get { return InternalArray; }
        }

        #endregion IGeometryBatchVertexArrayCollection

        #endregion Methods
    }
}
