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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public sealed class FullScreenQuad : IDisposable
    {
        private IndexBuffer mIndexBuffer;
        private DynamicVertexBuffer mVertexBuffer;
        private VertexDeclaration mVertexDeclaration;
        private bool mIsDisposed = false;

        private VertexPositionTexture[] mVertices;

        private GraphicsDeviceManager mGraphicsDeviceManager;

        public FullScreenQuad(GraphicsDeviceManager graphicsDeviceManager)
        {
            mGraphicsDeviceManager = graphicsDeviceManager;

            var graphicsDevice = mGraphicsDeviceManager.GraphicsDevice;

            mIndexBuffer = new IndexBuffer(graphicsDevice, 12, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
            mIndexBuffer.SetData<short>(new short[] { 0, 1, 2, 0, 2, 3 });

            mVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);

            mVertices = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture( new Vector3( -1,  1, 1 ), new Vector2( 0, 0 ) ),
                            new VertexPositionTexture( new Vector3(  1,  1, 1 ), new Vector2( 1, 0 ) ),
                            new VertexPositionTexture( new Vector3(  1, -1, 1 ), new Vector2( 1, 1 ) ),
                            new VertexPositionTexture( new Vector3( -1, -1, 1 ), new Vector2( 0, 1 ) )
                        };

            mVertexBuffer = new DynamicVertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
        }

        public void Draw(MaterialEffect effect, AleGameTime gameTime, IRenderTargetManager renderTargetManager)
        {
            PresentationParameters pp = mGraphicsDeviceManager.GraphicsDevice.PresentationParameters;
            Draw(effect, gameTime, renderTargetManager, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public void Draw(MaterialEffect effect, AleGameTime gameTime, IRenderTargetManager renderTargetManager, int width, int height)
        {
            var graphicsDevice = mGraphicsDeviceManager.GraphicsDevice;

            graphicsDevice.Vertices[0].SetSource(null, 0, 0);
            FillVertexBuffer(width, height);

            graphicsDevice.VertexDeclaration = mVertexDeclaration;
            graphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, 20);
            graphicsDevice.Indices = mIndexBuffer;

            effect.Apply(gameTime,null, null, null, renderTargetManager, effect.DefaultTechnique.Passes[0]);
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            MaterialEffect.Finish();
        }

        public void Draw(MaterialEffect effect, AleGameTime gameTime)
        {
            Draw(effect, gameTime, null);
        }
        public void Draw(MaterialEffect effect, AleGameTime gameTime, int width, int height)
        {
            Draw(effect, gameTime, null, width, height);
        }


        public void Dispose()
        {
            if (!mIsDisposed)
            {
                mVertexBuffer.Dispose();
                mIndexBuffer.Dispose();
                mVertexDeclaration.Dispose();

                mIndexBuffer = null;
                mVertexBuffer = null;
                mVertexDeclaration = null;

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        private void FillVertexBuffer(int width, int height)
        {
            var graphicsDevice = mGraphicsDeviceManager.GraphicsDevice;
            float hpw = (0.5f / (float)width);
            float hph = (0.5f / (float)height);

            mVertices[0].TextureCoordinate.X = 0 + hpw;
            mVertices[0].TextureCoordinate.Y = 0 + hph;
            mVertices[1].TextureCoordinate.X = 1 + hpw;
            mVertices[1].TextureCoordinate.Y = 0 + hph;
            mVertices[2].TextureCoordinate.X = 1 + hpw;
            mVertices[2].TextureCoordinate.Y = 1 + hph;
            mVertices[3].TextureCoordinate.X = 0 + hpw;
            mVertices[3].TextureCoordinate.Y = 1 + hph;

            mVertexBuffer.SetData<VertexPositionTexture>(mVertices);
        }
    }

}
