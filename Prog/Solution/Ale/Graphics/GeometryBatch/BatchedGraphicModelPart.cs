using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Graphics
{
    /// <summary>
    /// Part of the graphic model with a single material
    /// </summary>
    internal class BatchedGraphicModelPart
    {
        #region Fields

        /// <summary>
        /// Parent model
        /// </summary>
        private BatchedGraphicModel mBatchedGraphicModel;

        /// <summary>
        /// 
        /// </summary>
        private Material mMaterial;

        /// <summary>
        /// 
        /// </summary>
        private MeshPart mMeshPart;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public BatchedGraphicModel BatchedGraphicModel
        {
            get { return mBatchedGraphicModel; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get { return mMaterial; }
        }


        /// <summary>
        /// 
        /// </summary>
        public MeshPart MeshPart
        {
            get { return mMeshPart; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicModelPart"></param>
        /// <param name="batchedGraphicModel"></param>
        public BatchedGraphicModelPart(GraphicModelPart graphicModelPart, BatchedGraphicModel batchedGraphicModel)
        {
            mBatchedGraphicModel = batchedGraphicModel;
            mMaterial = graphicModelPart.Material;
            mMeshPart = graphicModelPart.MeshPart;
        }

        #endregion Methods
    }
}
