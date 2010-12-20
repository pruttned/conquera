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

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Ale.Graphics
{
    /// <summary>
    /// Renderable part of the graphic model
    /// </summary>
    public class GraphicModelPart : IRenderableUnit
    {
        #region Fields

        /// <summary>
        /// Parent graphic model
        /// </summary>
        private GraphicModel mGraphicModel;

        /// <summary>
        /// Mesh part on which is this model part based
        /// </summary>
        private MeshPart mMeshPart;

        /// <summary>
        /// Material that should be used during rendering process
        /// </summary>
        private Material mMaterial;

        private MatrixArrayMaterialEffectParam mBonesMaterialEffectParam;

        #endregion Fields

        #region Properties

        #region IRenderableUnit

        /// <summary>
        /// Gets the parent renderable
        /// </summary>
        public Renderable ParentRenderable
        {
            get { return mGraphicModel; }
        }

        /// <summary>
        /// Gets/sets the material that should be used during rendering process
        /// </summary>
        public Material Material
        {
            get { return mMaterial; }
            set 
            { 
                mMaterial = value;
                mBonesMaterialEffectParam = (MatrixArrayMaterialEffectParam)mMaterial.MaterialEffect.ManualParameters.GetParamBySemantic("Skin");
            }
        }

        public IMaterialEffectParametersUpdater CustomMaterialEffectParametersUpdater { get; set; }

        /// <summary>
        /// Gets the mesh part on which is this model part based
        /// </summary>
        public MeshPart MeshPart
        {
            get { return mMeshPart; }
        }

        #endregion IRenderableUnit

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicModel">- Parent graphic model</param>
        /// <param name="meshPart">- Mesh part on which should be this model part based</param>
        /// <param name="material">- Material that should be used during rendering process</param>
        public GraphicModelPart(GraphicModel graphicModel, MeshPart meshPart, Material material)
        {
            mGraphicModel = graphicModel;
            mMeshPart = meshPart;
            Material = material;
        }

        #region IRenderableUnit

        /// <summary>
        /// Renders the renderable primitive
        /// </summary>
        /// <remarks>
        /// All effect parameters must be setted prior to calling this method
        /// </remarks>
        public void Render(AleGameTime gameTime)
        {
            mMeshPart.Render();
        }

        /// <summary>
        /// Updates manual per-renderable unit effect parameters.
        /// </summary>
        public void UpdateMaterialEffectParameters()
        {
            if (null != mBonesMaterialEffectParam && (null != mGraphicModel.SkinTransformations))
            {
                mBonesMaterialEffectParam.Value = mGraphicModel.SkinTransformations;
            }

            if (null != CustomMaterialEffectParametersUpdater)
            {
                CustomMaterialEffectParametersUpdater.UpdateMaterialEffectParameters();
            }
        }

        #endregion IRenderableUnit

        #endregion Methods
    }

    public interface IMaterialEffectParametersUpdater
    {
        void UpdateMaterialEffectParameters();
    }
}
