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
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public interface IDynamicQuadGeometryManager<T> : IFrameListener, IDisposable where T : struct
    {
        void AllocGeometry(T[] vData, int startIndex, int vCnt, ref TransientQuadGeometry<T> geometry);
        void AllocGeometry(T[] vData, ref TransientQuadGeometry<T> geometry);
        void PrepareForRender();
     }

    /// <summary>
    /// Manager of the dynamic quad geometry (see TransientQuadGeometry).
    /// </summary>
    /// <typeparam name="T">Vertex type</typeparam>
    public class DynamicQuadGeometryManager<T> : IDynamicQuadGeometryManager<T> where T : struct
    {
        #region Fields

        /// <summary>
        /// Minimal size of the quad buffer
        /// </summary>
        private const int MinQuadBufferSize = 1000;

        /// <summary>
        /// Min size of the index buffer in number of quads (size / 6)
        /// </summary>
        private const int MinIbSizeInQuads = 500;

        /// <summary>
        /// Quad buffers
        /// </summary>
        private List<QuadBuffer<T>> mQuadBuffers = new List<QuadBuffer<T>>();

        /// <summary>
        /// Graphics device
        /// </summary>
        private GraphicsDeviceManager mGraphicsDeviceManager;

        /// <summary>
        /// Vertex declaration
        /// </summary>
        private VertexDeclaration mVertexDeclaration;

        /// <summary>
        /// Whether is this object disposed
        /// </summary>
        bool mIsDisposed = false;

        /// <summary>
        /// Shared index buffer among all quad buffers
        /// </summary>
        private IndexBuffer mIndexBuffer = null;

        /// <summary>
        /// Size of the current index buffer in quads (size / 6)
        /// </summary>
        private int mIndexBufferSizeInQuads = -1;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicsDevice">-Graphics device</param>
        /// <param name="vertexDeclaration">- Vertex declaration</param>
        public DynamicQuadGeometryManager(GraphicsDeviceManager graphicsDeviceManager, VertexDeclaration vertexDeclaration)
        {
            mGraphicsDeviceManager = graphicsDeviceManager;
            mVertexDeclaration = vertexDeclaration;
        }

        #region IFrameListener
        
        /// <summary>
        /// Clears buffers from the last frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
            //Console.WriteLine(mQuadBuffers.Count);
            for (int i = 0; i < mQuadBuffers.Count; ++i)
            {
                mQuadBuffers[i].Clear();
            }
        }

        /// <summary>
        /// Called after updating a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {
        }

        /// <summary>
        /// Called before rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
        }

        /// <summary>
        /// Called after rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
        }

        #endregion IFrameListener

        /// <summary>
        /// Allocates the transient geometry for a current frame. You must call this method in each frame in which you want to render the geometry.
        /// </summary>
        /// <param name="vData">- Vertex data. Number of verticies must be multiple of 4. Each 4 verticies represents one quad corners=[upper-left; upper-right; lower-left; lower-right] </param>
        /// <param name="startIndex">- Start index in the source array</param>
        /// <param name="vCnt">- Number of vertices to be copied</param>
        /// <param name="geometry">- TransientQuadGeometry that should be initialized by this call</param>
        public void AllocGeometry(T[] vData, int startIndex, int vCnt, ref TransientQuadGeometry<T> geometry)
        {
            if (0 != vCnt % 4)
            {
                throw new ArgumentException("number of verticies must be multiple of 4");
            }

            int quadCnt = vCnt / 4;

            //check whether is current index buffer big enough
            if (quadCnt > mIndexBufferSizeInQuads)
            {
                InitIndexBuffer(Math.Max(quadCnt, MinIbSizeInQuads));
            }

            //try to find first free buffer
            QuadBuffer<T> quadBuffer = FindFreeBuffer(quadCnt);
            if (null == quadBuffer) //create a new buffer
            {
                quadBuffer = new QuadBuffer<T>(Math.Max(quadCnt * 2, MinQuadBufferSize), mGraphicsDeviceManager, mVertexDeclaration, this); // quadCnt * 2 -> buffer should be bigger then the actual requirement so it can be used also for next requirements
                mQuadBuffers.Add(quadBuffer);
            }

            quadBuffer.AllocGeometry(vData, startIndex, vCnt, ref geometry);
        }
    
        /// <summary>
        /// Allocates the transient geometry for a current frame. You must call this method in each frame in which you want to render the geometry.
        /// </summary>
        /// <param name="vData">- Vertex data. Number of verticies must be multiple of 4. Each 4 verticies represents one quad corners=[upper-left; upper-right; lower-left; lower-right] </param>
        /// <param name="geometry">- TransientQuadGeometry that should be initialized by this call</param>
        public void AllocGeometry(T[] vData, ref TransientQuadGeometry<T> geometry)
        {
            AllocGeometry(vData, 0, vData.Length, ref geometry);
        }

        /// <summary>
        /// Prepares the graphics device for rendering the geometry based on this manager
        /// </summary>
        public void PrepareForRender()
        {
            mGraphicsDeviceManager.GraphicsDevice.Indices = mIndexBuffer;
        }

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    foreach (QuadBuffer<T> quadBuffer in mQuadBuffers)
                    {
                        quadBuffer.Dispose();
                    }

                    if (null != mIndexBuffer)
                    {
                        mIndexBuffer.Dispose();
                    }

                }
                mIsDisposed = true;
            }
        }

        #endregion Dispose


        /// <summary>
        /// Initializes the index buffer for a specified number of quads
        /// </summary>
        /// <param name="sizeInQuads">- Number of quads that the index buffer must holds</param>
        private void InitIndexBuffer(int sizeInQuads)
        {
            mIndexBufferSizeInQuads = sizeInQuads;

            if (null != mIndexBuffer)
            {
                mIndexBuffer.Dispose();
            }

            mIndexBuffer = new IndexBuffer(mGraphicsDeviceManager.GraphicsDevice, sizeInQuads * 12, BufferUsage.WriteOnly, IndexElementSize.SixteenBits); //12 = 2 * 6

            //init indexes
            ushort[] indicies = new ushort[sizeInQuads * 6];
            ushort vIndex = 0, iIndex = 0;
            for (int i = 0; i < sizeInQuads; ++i, vIndex += 4)
            {
                indicies[iIndex++] = vIndex;
                indicies[iIndex++] = (ushort)(vIndex + 1);
                indicies[iIndex++] = (ushort)(vIndex + 2);
                indicies[iIndex++] = (ushort)(vIndex + 1);
                indicies[iIndex++] = (ushort)(vIndex + 3);
                indicies[iIndex++] = (ushort)(vIndex + 2);
            }

            mIndexBuffer.SetData<ushort>(indicies);
        }

        /// <summary>
        /// Finds the first free buffer that can store specified number of quads
        /// </summary>
        /// <param name="quadCnt">- Number of quads that the buffer must be capable of storing</param>
        /// <returns>QuadBuffer or null if no free buffer was found</returns>
        private QuadBuffer<T> FindFreeBuffer(int quadCnt)
        {
            for (int i = 0; i < mQuadBuffers.Count; ++i)
            {
                if (mQuadBuffers[i].FreeQuadCnt >= quadCnt)
                {
                    return mQuadBuffers[i];
                }
            }

            return null;
        }

        #endregion Methods
    }
}
