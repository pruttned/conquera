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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Dynamic buffer of multiple quads used for initializing a TransientQuadGeometry
    /// </summary>
    /// <typeparam name="T">Vertex type</typeparam>
    internal sealed class QuadBuffer<T> : IDisposable where T : struct
    {
        #region Fields

        /// <summary>
        /// Vertex buffer
        /// </summary>
        private DynamicVertexBuffer mVertexBuffer;

        /// <summary>
        /// MAximum number of quads that can this buffer hold
        /// </summary>
        private int mMaxQuadCnt;

        /// <summary>
        /// Free space that remains in the buffer for this frame in number of quads that it can holds
        /// </summary>
        private int mFreeQuadCnt;

        /// <summary>
        /// Graphics device manager
        /// </summary>
        private GraphicsDeviceManager mGraphicsDeviceManager;

        /// <summary>
        /// Vertex declaration
        /// </summary>
        private VertexDeclaration mVertexDeclaration;

        /// <summary>
        /// Size of one vertex
        /// </summary>
        private int mVertexStride;

        /// <summary>
        /// Index of the first vertex in the next allocated geometry
        /// </summary>
        private int mNextVertexIndex = 0;

        /// <summary>
        /// Parent dynamic geometry manager
        /// </summary>
        DynamicQuadGeometryManager<T> mDynamicQuadGeometryManager;

        /// <summary>
        /// Whether is this object disposed
        /// </summary>
        bool mIsDisposed = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the free space that remains in the buffer for this frame in number of quads that it can holds
        /// </summary>
        public int FreeQuadCnt
        {
            get { return mFreeQuadCnt; }
        }

        /// <summary>
        /// Gets the graphics device
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mGraphicsDeviceManager; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="quadCnt">Number of quads that can be stored in the buffer</param>
        /// <param name="graphicsDeviceManager">- Graphics device manager</param>
        /// <param name="vertexDeclaration">- Vertex declaration</param>
        /// <param name="dynamicQuadGeometryManager">- Parent DynamicGeometryManager</param>
        public QuadBuffer(int quadCnt, GraphicsDeviceManager graphicsDeviceManager, VertexDeclaration vertexDeclaration, DynamicQuadGeometryManager<T> dynamicQuadGeometryManager)
        {
            mDynamicQuadGeometryManager = dynamicQuadGeometryManager;

            mGraphicsDeviceManager = graphicsDeviceManager;
            mVertexDeclaration = vertexDeclaration;
            mVertexStride = mVertexDeclaration.GetVertexStrideSize(0);

            mVertexBuffer = new DynamicVertexBuffer(graphicsDeviceManager.GraphicsDevice, quadCnt * 4 * mVertexStride, BufferUsage.WriteOnly);

            mMaxQuadCnt = quadCnt;
            mFreeQuadCnt = quadCnt;
        }

        /// <summary>
        /// Allocates a geometery for an actual frame
        /// </summary>
        /// <param name="vData">- Vertex data. Number of verticies must be multiple of 4 - not checked here</param>
        /// <param name="startIndex">- Start index in the source array</param>
        /// <param name="vCnt">- Number of vertices to be copied</param>
        /// <param name="geometry">- TransientQuadGeometry that should be initialized by this call</param>
        public void AllocGeometry(T[] vData, int startIndex, int vCnt, ref TransientQuadGeometry<T> geometry)
        {
            SetDataOptions setDataOptions;
            if (mFreeQuadCnt == mMaxQuadCnt) //first set in the frame
            {
                setDataOptions = SetDataOptions.Discard;
            }
            else
            {
                setDataOptions = SetDataOptions.NoOverwrite;
            }

            mFreeQuadCnt -= vCnt / 4;

            mVertexBuffer.SetData<T>(mNextVertexIndex * mVertexStride, vData, startIndex, vCnt, mVertexStride, setDataOptions);

            geometry.Init(mNextVertexIndex, vCnt, this);

            mNextVertexIndex += vCnt;
        }

        /// <summary>
        /// Allocates a geometery for an actual frame
        /// </summary>
        /// <param name="vData">- Vertex data. Number of verticies must be multiple of 4 - not checked here</param>
        /// <param name="geometry">- TransientQuadGeometry that should be initialized by this call</param>
        public void AllocGeometry(T[] vData, ref TransientQuadGeometry<T> geometry)
        {
            AllocGeometry(vData, 0, vData.Length, ref geometry);
        }

        /// <summary>
        /// Clears the buffer (This method must be called on the start of each frame)
        /// </summary>
        public void Clear()
        {
            mFreeQuadCnt = mMaxQuadCnt;
            mNextVertexIndex = 0;
        }

        /// <summary>
        /// Prepares the graphics device for rendering geometry stored in this buffer
        /// </summary>
        public void PrepareForRender()
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.GraphicsDevice;
            graphicsDevice.VertexDeclaration = mVertexDeclaration;
            graphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, mVertexStride);
            mDynamicQuadGeometryManager.PrepareForRender();
        }

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                mVertexBuffer.Dispose();

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion Dispose

        #endregion Methods

    }
}
