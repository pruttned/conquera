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
            private HexCell mStartCell;
            private HexCell mEndCell;
            private Material mMaterial;
            private DynamicVertexBuffer mVertexBuffer;
            private IndexBuffer mIndexBuffer;
            private Vector3MaterialEffectParam mColorParam;

            private bool mIsDisposed = false;

            public GraphicsDevice GraphicsDevice
            {
                get { return mVertexDeclaration.GraphicsDevice; }
            }

            public HexCell StartCell
            {
                get { return mStartCell; }
                set
                {
                    if (mStartCell != value)
                    {
                        mStartCell = value;
                        RebuildGeometry();
                    }
                }
            }

            public HexCell EndCell
            {
                get { return mEndCell; }
                set
                {
                    if (mEndCell != value)
                    {
                        mEndCell = value;
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

                short[] indices = new short[]
                {
                    1, 2, 0,
                    3, 2, 1,
                    6, 4, 5,
                };
                mIndexBuffer = new IndexBuffer(graphicsDevice, 9 * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
                mIndexBuffer.SetData<short>(indices);
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

            protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
            {
                if (null != EndCell && null != StartCell)
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
                if (null != EndCell && null != StartCell)
                {
                    if (null != mVertexBuffer && StartCell != EndCell)
                    {
                        float lineThickness = 0.3f;
                        float halfLineThickness = lineThickness * 0.5f;
                        float lineZPos = 0.05f;

                        Vector3 cell1Center = mStartCell.CenterPos;
                        Vector3 cell2Center = mEndCell.CenterPos;

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

        public HexCell StartCell
        {
            get { return mMovementArrowRenderable.StartCell; }
            set { mMovementArrowRenderable.StartCell = value; }
        }

        public HexCell EndCell
        {
            get { return mMovementArrowRenderable.EndCell; }
            set { mMovementArrowRenderable.EndCell = value; }
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

        void ISceneDrawableComponent.EnqueRenderableUnits(AleGameTime gameTime, Renderer renderer, ScenePass scenePass)
        {
            if (null != StartCell && null != EndCell && StartCell != EndCell)
            {
                mMovementArrowRenderable.EnqueRenderableUnits(renderer, gameTime);
            }
        }
    }
}
