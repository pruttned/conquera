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
