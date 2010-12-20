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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Ale.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// Renderable for displaying bounding sphere of a renderable
    /// </summary>
    public class BoundingSphereRenderable : Renderable, IRenderableUnit
    {
        #region Fields

        /// <summary>
        /// Number of circle steps (must be divisible by 4)
        /// </summary>
        private const int StepCnt = 32;

        /// <summary>
        /// Total number of line segments
        /// </summary>
        private static int mLineCnt;

        /// <summary>
        /// Vertex buffer
        /// </summary>
        private static VertexBuffer mVertexBuffer = null;

        /// <summary>
        /// Material
        /// </summary>
        private static Material mMaterial;

        /// <summary>
        /// Color effect param
        /// </summary>
        private static Vector3MaterialEffectParam mColorParam;

        /// <summary>
        /// Vertex declaration
        /// </summary>
        private static VertexDeclaration mVertexDeclaration;

        /// <summary>
        /// Line color
        /// </summary>
        private Vector3 mColor;

        #endregion Fields

        #region Properties

        #region IRenderableUnit

        /// <summary>
        /// Parent renderable
        /// </summary>
        Renderable IRenderableUnit.ParentRenderable
        {
            get { return this; }
        }

        /// <summary>
        /// Material
        /// </summary>
        Material IRenderableUnit.Material
        {
            get { return mMaterial; }
        }

        #endregion IRenderableUnit

        /// <summary>
        /// Gets/sets the colot
        /// </summary>
        public Color Color
        {
            set { mColor = value.ToVector3(); }
            get { return new Color(mColor); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="renderable">- Renderable whose bounding sphere should be rendered</param>
        /// <param name="color">- Line color</param>
        public BoundingSphereRenderable(Color color)
            : base(new BoundingSphere(), true)
        {
            if (null == mVertexBuffer)
            {
                throw new InvalidOperationException("You must call static Init before calling ctor");
            }

            mColor = color.ToVector3();
        }

        /// <summary>
        /// Static init. Call this method once before contructing bounding sphere renderables
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="content"></param>
        static public void Init(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            if (0 != (StepCnt % 4))
            {
                throw new ArgumentException("LineCnt must be divisible by 4");
            }

            if (null != mVertexBuffer)
            {
                throw new InvalidOperationException("BoundingSphereRenderable is already initialized");
            }

            mMaterial = new Material(content.Load<MaterialEffect>("BoundingPrimitiveFx"), DefaultRenderLayers.GroundStandingObjects);
            mColorParam = (Vector3MaterialEffectParam)mMaterial.MaterialEffect.ManualParameters["gColor"];


            mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]{ new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0)});

            //geometry
            int pointCnt = (StepCnt + 1) * 3;

            mLineCnt = pointCnt - 1;

            mVertexBuffer = new VertexBuffer(graphicsDevice, 12 * pointCnt, BufferUsage.WriteOnly);
            Vector3[] vertices = new Vector3[pointCnt];

            float aInc = 2 * MathHelper.Pi / (float)StepCnt;

            int vIndex = 0;

            int yAxisAngleIndex = 0;

            for (; yAxisAngleIndex < StepCnt / 4 + 1; ++yAxisAngleIndex) //Y
            {
                vertices[vIndex++] = new Vector3((float)Math.Cos(aInc * yAxisAngleIndex), 0, (float)Math.Sin(aInc * yAxisAngleIndex));
            }

            for (int xAxisAngleIndex = StepCnt / 4; xAxisAngleIndex < StepCnt + 1 + StepCnt / 4; ++xAxisAngleIndex) //X
            {
                vertices[vIndex++] = new Vector3(0, (float)Math.Cos(aInc * xAxisAngleIndex), (float)Math.Sin(aInc * xAxisAngleIndex));
            }

            for (; yAxisAngleIndex < StepCnt / 2; ++yAxisAngleIndex) //Y
            {
                vertices[vIndex++] = new Vector3((float)Math.Cos(aInc * yAxisAngleIndex), 0, (float)Math.Sin(aInc * yAxisAngleIndex));
            }

            for (int zAxisAngleIndex = StepCnt / 2; zAxisAngleIndex < StepCnt + 1 + StepCnt / 2; ++zAxisAngleIndex) //Z
            {
                vertices[vIndex++] = new Vector3((float)Math.Cos(aInc * zAxisAngleIndex), (float)Math.Sin(aInc * zAxisAngleIndex), 0);
            }

            for (; yAxisAngleIndex < StepCnt + 1; ++yAxisAngleIndex) //Y
            {
                vertices[vIndex++] = new Vector3((float)Math.Cos(aInc * yAxisAngleIndex), 0, (float)Math.Sin(aInc * yAxisAngleIndex));
            }

            mVertexBuffer.SetData<Vector3>(vertices);
        }

        /// <summary>
        /// Updates transformations according to a bounding sphere
        /// </summary>
        /// <param name="boundingSphere">- Bounding sphere in world space</param>
        public void SetBoundingSphere(BoundingSphere boundingSphere)
        {
            SetTransformation(boundingSphere.Center, Quaternion.Identity, boundingSphere.Radius);
        }

        #region IRenderableUnit

        /// <summary>
        /// Renders the renderable
        /// </summary>
        /// <param name="gameTime"></param>
        public void Render(AleGameTime gameTime)
        {
            GraphicsDevice graphicsDevice = mVertexDeclaration.GraphicsDevice;
            graphicsDevice.VertexDeclaration = mVertexDeclaration;
            graphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, 12);
            graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, mLineCnt);
        }

        /// <summary>
        /// Updates the material effect params just before render
        /// </summary>
        public void UpdateMaterialEffectParameters()
        {
            mColorParam.Value = mColor;
        }

        #endregion IRenderableUnit

        /// <summary>
        /// Enques the itself to the render queue
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            renderer.EnqueueRenderable(this);
        }

        #endregion Methods
    }
}
