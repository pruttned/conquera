//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// Dynamic geometry represented by multiple quads that must be initialized each frame by calling DynamicGeometryManager.AllocGeometry.
    /// 
    /// By using transient geometry instead allocating vb and ib for each geometry it is possible to reuse fewer vbs and ibs among
    /// multiple geometries, because usually not all allocated geometries are rendered in each frame (some of them are outside of the camera's frustum).
    ///
    /// Transient geometry is faster then DrawUser*Primitives.
    /// </summary>
    /// <typeparam name="T">Vertex type</typeparam>
    public class TransientQuadGeometry<T> where T : struct
    {
        #region Fields

        /// <summary>
        /// Index of the first vertex
        /// </summary>
        private int mBaseVertex = -1;

        /// <summary>
        /// Number of verticies
        /// </summary>
        private int mNumVertices = -1;

        /// <summary>
        /// Quad buffer that has initialized the the geometry
        /// </summary>
        private QuadBuffer<T> mQuadBuffer = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initializes the geometery. This method should be called only by QuadBuffer class
        /// </summary>
        /// <param name="baseVertex">- Index of the first vertex</param>
        /// <param name="numVertices">- Number of verticies</param>
        /// <param name="quadBuffer">- Quad buffer that has called this method</param>
        internal void Init(int baseVertex, int numVertices, QuadBuffer<T> quadBuffer)
        {
            mBaseVertex = baseVertex;
            mNumVertices = numVertices;
            mQuadBuffer = quadBuffer;
        }

        /// <summary>
        /// Draws the geometry. Dont't forget to initialize the geometry each frame by calling DynamicGeometryManager.AllocGeometry
        /// </summary>
        /// <exception cref="InvalidOperationException">- TransientQuadGeometry was not initialized by calling DynamicGeometryManager.AllocGeometry method</exception>
        public void Draw()
        {
            if (mBaseVertex < 0)
            {
                throw new InvalidOperationException("You must initialize the TransientQuadGeometry by calling DynamicGeometryManager.AllocGeometry method before you can render it");
            }

            mQuadBuffer.PrepareForRender();
            mQuadBuffer.GraphicsDeviceManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, mBaseVertex, 0, mNumVertices, 0, mNumVertices / 2);
        }

        #endregion Methods
    }
}
