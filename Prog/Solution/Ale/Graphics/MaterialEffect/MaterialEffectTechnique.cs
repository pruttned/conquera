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

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Effect's technique
    /// </summary>
    public class MaterialEffectTechnique
    {
        #region Fields

        /// <summary>
        /// Microsoft.Xna.Framework.Graphics.EffectTechnique to which is this MaterialEffectTechnique binded
        /// </summary>
        private EffectTechnique mEffectTechnique;

        /// <summary>
        /// MaterialEffect to which this technique belongs
        /// </summary>
        private MaterialEffect mParentMaterialEffect;

        /// <summary>
        /// Passes of this technique
        /// </summary>
        private MaterialEffectPassCollection mPasses;

        /// <summary>
        /// Scene pass in which should be this technique used
        /// </summary>
        private NameId mScenePass;

        /// <summary>
        /// Manual parameters used in this technique
        /// </summary>
        private MaterialEffectParamCollection mManualParameters;

        /// <summary>
        /// Automatic parameters specific to frame used in this technique
        /// </summary>
        private List<PerFrameAutoParam> mPerFrameAutoParams;

        /// <summary>
        /// Automatic parameters specific to camera used in this technique
        /// </summary>
        private List<PerCameraAutoParam> mPerCameraAutoParams;

        /// <summary>
        /// Automatic parameters specific to a renderable used in this technique
        /// </summary>
        private List<PerRenderableAutoParam> mPerRenderableAutoParams;

        /// <summary>
        /// Automatic render target params used in this technique
        /// </summary>
        private List<RenderTargetAutoParam> mRenderTargetAutoParams;

        /// <summary>
        /// Automatic parameters specific to scene used in this technique
        /// </summary>
        private List<PerSceneAutoParam> mPerSceneAutoParams;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the collection of the technique's passes
        /// </summary>
        public MaterialEffectPassCollection Passes
        {
            get { return mPasses; }
        }

        /// <summary>
        /// Gets the name of this technique
        /// </summary>
        public string Name
        {
            get { return mEffectTechnique.Name; }
        }

        /// <summary>
        /// Gets the annotations of this technique
        /// </summary>
        public EffectAnnotationCollection Annotations
        {
            get { return mEffectTechnique.Annotations; }
        }

        /// <summary>
        /// Gets the scene pass in which should be this technique used
        /// </summary>
        public NameId ScenePass
        {
            get { return mScenePass; }
        }

        /// <summary>
        /// Gets the MaterialEffect to which this tecnique belongs
        /// </summary>
        public MaterialEffect ParentMaterialEffect
        {
            get { return mParentMaterialEffect; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal EffectTechnique EffectTechnique
        {
            get { return mEffectTechnique; }
        }

        /// <summary>
        /// Gets the effect's parameters that are configured manually
        /// </summary>
        public MaterialEffectParamCollection ManualParameters
        {
            get { return mManualParameters; }
        }
  
        /// <summary>
        /// Automatic render target params used in this technique
        /// </summary>
        internal List<RenderTargetAutoParam> RenderTargetAutoParams
        {
            get { return mRenderTargetAutoParams; }
        }

        #endregion Properties

        #region Methods

        public bool IsParameterUsed(MaterialEffectParam param)
        {
            return mManualParameters.Contains(param);
        }

        /// <summary>
        /// Internal ctor
        /// </summary>
        /// <param name="parentMaterial">- MaterialEffect to which will this pass belong</param>
        /// <param name="effectTechnique">- Microsoft.Xna.Framework.Graphics.EffectTechnique to which should be this MaterialEffectTechnique binded</param>
        /// <param name="manualParameters"></param>
        /// <param name="perCameraAutoParams"></param>
        /// <param name="perFrameAutoParams"></param>
        /// <param name="perRenderableAutoParams"></param>
        /// <param name="perSceneAutoParams"></param>
        /// <param name="renderTargetAutoParams"></param>
        internal MaterialEffectTechnique(MaterialEffect parentMaterial, EffectTechnique effectTechnique,
            IEnumerable<MaterialEffectParam> manualParameters,
            IEnumerable<PerFrameAutoParam> perFrameAutoParams, IEnumerable<PerCameraAutoParam> perCameraAutoParams,
            IEnumerable<PerRenderableAutoParam> perRenderableAutoParams, IEnumerable<RenderTargetAutoParam> renderTargetAutoParams,
            IEnumerable<PerSceneAutoParam> perSceneAutoParams)
        {
            mParentMaterialEffect = parentMaterial;

            //Only 16 passes are supported because it is stored as a 4b number in the render batch number
            if (effectTechnique.Passes.Count > 16)
            {
                throw new ArgumentException("Exceeded maximum supported number of passes in the effect's technique, which is 16");
            }

            mEffectTechnique = effectTechnique;

            mScenePass = effectTechnique.Name;

            //passes
            List<MaterialEffectPass> passes = new List<MaterialEffectPass>(effectTechnique.Passes.Count);
            mPasses = new MaterialEffectPassCollection(passes);
            for (int i = 0; i < effectTechnique.Passes.Count; ++i)
            {
                passes.Add(new MaterialEffectPass(this, effectTechnique.Passes[i], (byte)i));
            }

            //init parameters
            InitParameters(manualParameters, perFrameAutoParams, perCameraAutoParams, perRenderableAutoParams, renderTargetAutoParams, perSceneAutoParams);
        }

        /// <summary>
        /// Updates per render target manager
        /// </summary>
        /// <param name="renderTargetManager"></param>
        internal void PerRenderTargetManagerUpdate(RenderTargetManager renderTargetManager)
        {
            if (null != mRenderTargetAutoParams)
            {
                for (int i = 0; i < mRenderTargetAutoParams.Count; ++i)
                {
                    mRenderTargetAutoParams[i].Update(renderTargetManager);
                }
            }
        }

        /// <summary>
        /// Updates per frame automatic parameters
        /// </summary>
        /// <param name="gameTime">- Actual game time</param>
        internal void PerFrameUpdate(AleGameTime gameTime)
        {
            if (null != mPerFrameAutoParams)
            {
                for (int i = 0; i < mPerFrameAutoParams.Count; ++i)
                {
                    mPerFrameAutoParams[i].Update(gameTime);
                }
            }
        }

        /// <summary>
        /// Updates per camera automatic parameters
        /// </summary>
        /// <param name="camera">- Actual camera</param>
        internal void PerCameraUpdate(ICamera camera)
        {
            if (null != mPerCameraAutoParams)
            {
                for (int i = 0; i < mPerCameraAutoParams.Count; ++i)
                {
                    mPerCameraAutoParams[i].Update(camera);
                }
            }
        }

        /// <summary>
        /// Updates per renderable automatic parameters
        /// </summary>
        /// <param name="renderable">- Renderable that is going to be rendered</param>
        /// <param name="camera">- Actual camera</param>
        internal void PerRenderableUpdate(Renderable renderable, ICamera camera)
        {
            if (null != mPerRenderableAutoParams)
            {
                for (int i = 0; i < mPerRenderableAutoParams.Count; ++i)
                {
                    mPerRenderableAutoParams[i].Update(renderable, camera);
                }
            }
        }


        /// <summary>
        /// Updates per scene automatic parameters
        /// </summary>
        /// <param name="scene">- Actual scene</param>
        internal void PerSceneUpdate(Scene.BaseScene scene)
        {
            if (null != mPerSceneAutoParams)
            {
                for (int i = 0; i < mPerSceneAutoParams.Count; ++i)
                {
                    mPerSceneAutoParams[i].Update(scene);
                }
            }
        }

        private void InitParameters(IEnumerable<MaterialEffectParam> manualParameters, IEnumerable<PerFrameAutoParam> perFrameAutoParams, 
            IEnumerable<PerCameraAutoParam> perCameraAutoParams, IEnumerable<PerRenderableAutoParam> perRenderableAutoParams,
            IEnumerable<RenderTargetAutoParam> renderTargetAutoParams, IEnumerable<PerSceneAutoParam> perSceneAutoParams)
        {
            List<MaterialEffectParam> techniqueManualParams = new List<MaterialEffectParam>();
            foreach (MaterialEffectParam param in manualParameters)
            {
                if (param.IsUsedInTechnique(mEffectTechnique))
                {
                    techniqueManualParams.Add(param);
                }
            }
            mManualParameters = new MaterialEffectParamCollection(techniqueManualParams);

            if (null != perFrameAutoParams)
            {
                mPerFrameAutoParams = new List<PerFrameAutoParam>();
                foreach (PerFrameAutoParam param in perFrameAutoParams)
                {
                    if (param.IsUsedInTechnique(mEffectTechnique))
                    {
                        mPerFrameAutoParams.Add(param);
                    }
                }
                if (0 == mPerFrameAutoParams.Count)
                {
                    mPerFrameAutoParams = null;
                }
            }


            if (null != perCameraAutoParams)
            {
                mPerCameraAutoParams = new List<PerCameraAutoParam>();
                foreach (PerCameraAutoParam param in perCameraAutoParams)
                {
                    if (param.IsUsedInTechnique(mEffectTechnique))
                    {
                        mPerCameraAutoParams.Add(param);
                    }
                }
                if (0 == mPerCameraAutoParams.Count)
                {
                    mPerCameraAutoParams = null;
                }
            }

            if (null != perRenderableAutoParams)
            {
                mPerRenderableAutoParams = new List<PerRenderableAutoParam>();
                foreach (PerRenderableAutoParam param in perRenderableAutoParams)
                {
                    if (param.IsUsedInTechnique(mEffectTechnique))
                    {
                        mPerRenderableAutoParams.Add(param);
                    }
                }
                if (0 == mPerRenderableAutoParams.Count)
                {
                    mPerRenderableAutoParams = null;
                }
            }

            if (null != renderTargetAutoParams)
            {
                mRenderTargetAutoParams = new List<RenderTargetAutoParam>();
                foreach (RenderTargetAutoParam param in renderTargetAutoParams)
                {
                    if (param.IsUsedInTechnique(mEffectTechnique))
                    {
                        mRenderTargetAutoParams.Add(param);
                    }
                }
                if (0 == mRenderTargetAutoParams.Count)
                {
                    mRenderTargetAutoParams = null;
                }
            }

            if (null != perSceneAutoParams)
            {
                mPerSceneAutoParams = new List<PerSceneAutoParam>();
                foreach (PerSceneAutoParam param in perSceneAutoParams)
                {
                    if (param.IsUsedInTechnique(mEffectTechnique))
                    {
                        mPerSceneAutoParams.Add(param);
                    }
                }
                if (0 == mPerSceneAutoParams.Count)
                {
                    mPerSceneAutoParams = null;
                }
            }
        }
        #endregion Methods
    }
}
