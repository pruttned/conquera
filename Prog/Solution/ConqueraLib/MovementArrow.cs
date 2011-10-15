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
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;
using Microsoft.Xna.Framework;
using Ale.Tools;
using Ale.Scene;
using Ale;

namespace Conquera
{
    public class MovementArrow : ISceneDrawableComponent, IDisposable
    {
        #region Types

        class MovementArrowRenderable : Renderable, IRenderableUnit, IDisposable
        {
            private VertexDeclaration mVertexDeclaration;
            private HexTerrainTile mStartTile;
            private HexTerrainTile mEndTile;
            private Material mMaterial;
            private DynamicVertexBuffer mVertexBuffer;
            private IndexBuffer mIndexBuffer;
            private Vector3MaterialEffectParam mColorParam;

            private bool mIsDisposed = false;

            public GraphicsDevice GraphicsDevice
            {
                get { return mVertexDeclaration.GraphicsDevice; }
            }

            public HexTerrainTile StartTile
            {
                get { return mStartTile; }
                set
                {
                    if (mStartTile != value)
                    {
                        mStartTile = value;
                        RebuildGeometry();
                    }
                }
            }

            public HexTerrainTile EndTile
            {
                get { return mEndTile; }
                set
                {
                    if (mEndTile != value)
                    {
                        mEndTile = value;
                        RebuildGeometry();
                    }
                }
            }

            public Vector3 Color { get; set; }

            internal MovementArrowRenderable(GraphicsDevice graphicsDevice, ContentGroup content)
                : base(new BoundingSphere(), false)
            {
                IsVisible = true;

                MaterialSettings settings = new MaterialSettings("MovementArrowMat", "MovementArrowFx", DefaultRenderLayers.MovementArrow);
                mMaterial = new Material(settings, content);
                Color = Vector3.One;
                mColorParam = (Vector3MaterialEffectParam)mMaterial.MaterialEffect.ManualParameters["gColor"];
                if (null == mColorParam) throw new ArgumentNullException("Missing parameter gColor in MovementArrowFx");
                mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]{ new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0)});

                ushort[] indices = new ushort[]
                {
                    1, 2, 0,
                    3, 2, 1,
                    6, 4, 5,
                };
                mIndexBuffer = new IndexBuffer(graphicsDevice, 9 * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
                mIndexBuffer.SetData<ushort>(indices);
            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool isDisposing)
            {
                if (!mIsDisposed)
                {
                    if (isDisposing)
                    {
                        if (null != mVertexBuffer)
                        {
                            mVertexBuffer.Dispose();
                        }
                        mIndexBuffer.Dispose();
                        mVertexDeclaration.Dispose();
                    }
                    mIsDisposed = true;
                }
            }

            public Renderable ParentRenderable
            {
                get { return this; }
            }

            public Material Material
            {
                get { return mMaterial; }
            }

            public void Render(AleGameTime gameTime)
            {
                GraphicsDevice graphicsDevice = mVertexDeclaration.GraphicsDevice;
                graphicsDevice.VertexDeclaration = mVertexDeclaration;
                graphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, 12);
                graphicsDevice.Indices = mIndexBuffer;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 3);
            }

            public void UpdateMaterialEffectParameters()
            {
                mColorParam.Value = Color;
            }

            protected override void OnEnqueRenderableUnits(IRenderer renderer, AleGameTime gameTime)
            {
                if (null != EndTile && null != StartTile)
                {
                    if (null == mVertexBuffer || mVertexBuffer.IsContentLost)
                    {
                        if (null == mVertexBuffer)
                        {
                            mVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, 12 * 7, BufferUsage.WriteOnly);
                        }
                        RebuildGeometry();
                    }

                    renderer.EnqueueRenderable(this);
                }
            }

            private void RebuildGeometry()
            {
                if (null != EndTile && null != StartTile)
                {
                    if (null != mVertexBuffer && StartTile != EndTile)
                    {
                        float lineThickness = 0.3f;
                        float halfLineThickness = lineThickness * 0.5f;
                        float lineZPos = 0.05f;

                        Vector3 cell1Center = mStartTile.CenterPos;
                        Vector3 cell2Center = mEndTile.CenterPos;

                        Vector2 p1 = new Vector2(cell1Center.X, cell1Center.Y);
                        Vector2 p2 = new Vector2(cell2Center.X, cell2Center.Y);
                        Vector2 p1p2 = p2 - p1;
                        p1p2.Normalize();
                        Vector2 p1p2Perp = AleMathUtils.GetPerpVector(p1p2); ;
                        Vector2 p3 = p2 - p1p2 * lineThickness * 2;

                        Vector3[] vertices = new Vector3[]
                        {
                            new Vector3(p1.X - p1p2Perp.X * halfLineThickness, p1.Y - p1p2Perp.Y * halfLineThickness, lineZPos),
                            new Vector3(p1.X + p1p2Perp.X * halfLineThickness, p1.Y + p1p2Perp.Y * halfLineThickness, lineZPos),
                            new Vector3(p3.X - p1p2Perp.X * halfLineThickness, p3.Y - p1p2Perp.Y * halfLineThickness, lineZPos),
                            new Vector3(p3.X + p1p2Perp.X * halfLineThickness, p3.Y + p1p2Perp.Y * halfLineThickness, lineZPos),
                            new Vector3(p2.X, p2.Y, lineZPos),
                            new Vector3(p3.X - p1p2Perp.X * lineThickness, p3.Y - p1p2Perp.Y * lineThickness, lineZPos),
                            new Vector3(p3.X + p1p2Perp.X * lineThickness, p3.Y + p1p2Perp.Y * lineThickness, lineZPos)
                        };
                        mVertexBuffer.SetData<Vector3>(vertices);
                    }
                }
            }
        }

        #endregion Types

        private bool mIsDisposed = false;
        private MovementArrowRenderable mMovementArrowRenderable;

        public HexTerrainTile StartCell
        {
            get { return mMovementArrowRenderable.StartTile; }
            set { mMovementArrowRenderable.StartTile = value; }
        }

        public HexTerrainTile EndCell
        {
            get { return mMovementArrowRenderable.EndTile; }
            set { mMovementArrowRenderable.EndTile = value; }
        }

        public bool IsVisible
        {
            get { return mMovementArrowRenderable.IsVisible; }
            set { mMovementArrowRenderable.IsVisible = value; }
        }

        public Vector3 Color 
        {
            get { return mMovementArrowRenderable.Color; }
            set { mMovementArrowRenderable.Color = value; } 
        }

        internal MovementArrow(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            mMovementArrowRenderable = new MovementArrowRenderable(graphicsDevice, content);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    mMovementArrowRenderable.Dispose();
                }
                mIsDisposed = true;
            }
        }

        void ISceneDrawableComponent.EnqueRenderableUnits(AleGameTime gameTime, IRenderer renderer, ScenePass scenePass)
        {
            if (null != StartCell && null != EndCell && StartCell != EndCell)
            {
                mMovementArrowRenderable.EnqueRenderableUnits(renderer, gameTime);
            }
        }
    }
}
