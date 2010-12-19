using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// IRenderableUnit of the GeometryBatch
    /// </summary>
    internal class GeometryBatchUnit : IRenderableUnit
    {
        #region Fields

        /// <summary>
        /// Material that should be used during rendering process
        /// </summary>
        private Material mMaterial;

        /// <summary>
        /// 
        /// </summary>
        private int mBaseVertex;

        /// <summary>
        /// 
        /// </summary>
        private int mNumVertices;

        /// <summary>
        /// 
        /// </summary>
        private int mPrimitiveCnt;

        /// <summary>
        /// 
        /// </summary>
        private int mStartIndex;

        /// <summary>
        /// 
        /// </summary>
        private GeometryBatch mGeometryBatch;

        #endregion Fields

        #region Properties

        #region IRenderableUnit

        /// <summary>
        /// 
        /// </summary>
        public Renderable ParentRenderable
        {
            get { return mGeometryBatch; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get { return mMaterial; }
        }

        #endregion IRenderableUnit

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryBatch"></param>
        /// <param name="material"></param>
        /// <param name="baseVertex"></param>
        /// <param name="numVertices"></param>
        /// <param name="primitiveCnt"></param>
        /// <param name="startIndex"></param>
        public GeometryBatchUnit(GeometryBatch geometryBatch, Material material, int baseVertex, int numVertices, int primitiveCnt, int startIndex)
        {
            mMaterial = material;
            mBaseVertex = baseVertex;
            mNumVertices = numVertices;
            mPrimitiveCnt = primitiveCnt;
            mStartIndex = startIndex;
            mGeometryBatch = geometryBatch;
        }

        #region IRenderableUnit

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Render(AleGameTime gameTime)
        {
            mGeometryBatch.PrepareForRender();
            mGeometryBatch.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, mBaseVertex, 0, mNumVertices, mStartIndex, mPrimitiveCnt);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateMaterialEffectParameters()
        {
        }

        #endregion IRenderableUnit

        #endregion Methods
    }
}
