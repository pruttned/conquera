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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Microsoft.Xna.Framework.Graphics.Effect proxy that updates effect parameters only if it is really neaded
    /// </summary>
    public sealed class MaterialEffect : IDisposable
    {
        #region Fields

        /// <summary>
        /// Id counter
        /// </summary>
        private static ushort mNextEffectId = 0;

        /// <summary>
        /// Last applied material effect
        /// </summary>
        private static MaterialEffect mLastAppliedMaterialEffect = null;

        /// <summary>
        /// Last applied pass
        /// </summary>
        private static MaterialEffectPass mLastAppliedPass = null;

        /// <summary>
        /// Effect's id 
        /// </summary>
        private ushort mId;

        /// <summary>
        /// Effect to which is this proxy binded
        /// </summary>
        private Effect mEffect;

        /// <summary>
        /// Active technique
        /// </summary>
        private MaterialEffectTechnique mActiveTechnique;

        /// <summary>
        /// Techniques supported by this material effect
        /// </summary>
        private MaterialEffectTechniqueCollection mTechniques;

        /// <summary>
        /// Frame number when was effect applied last 
        /// (Used to check whether was effect already applied during rendering of the actual frame)
        /// </summary>
        private long mLastUpdateFrameNum = -1;

        /// <summary>
        /// Camera that was used previously during a application of the effect
        /// </summary>
        private ICamera mLastCameraUpdate = null;

        /// <summary>
        /// 
        /// </summary>
        private Scene.BaseScene mLastSceneUpdate = null;

        private RenderTargetManager mLastRenderTargetManagerUpdate = null;

        /// <summary>
        /// Effect parameters that are configured manually
        /// </summary>
        private MaterialEffectParamCollection mManualParameters;

        /// <summary>
        /// Default technique of this effect
        /// </summary>
        private MaterialEffectTechnique mDefaultTechnique = null;

        /// <summary>
        /// Whether was this object already disposed
        /// </summary>
        private bool mIsDisposed = false;

        private List<RenderTargetAutoParam> mRenderTargetAutoParams;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the effect's id
        /// </summary>
        public ushort Id
        {
            get { return mId; }
        }

        /// <summary>
        /// Gets the collection of techniques supported by this material effect.
        /// Keys are scene passes
        /// </summary>
        public MaterialEffectTechniqueCollection Techniques
        {
            get { return mTechniques; }
        }

        /// <summary>
        /// Gets the effect's parameters that are configured manually
        /// </summary>
        public MaterialEffectParamCollection ManualParameters
        {
            get { return mManualParameters; }
        }

        /// <summary>
        /// Gets the default technique of this effect
        /// </summary>
        public MaterialEffectTechnique DefaultTechnique
        {
            get { return mDefaultTechnique; }
        }

        private MaterialEffectTechnique ActiveTechnique
        {
            get { return mActiveTechnique; }
            set 
            {
                if (mActiveTechnique != value)
                {
                    mActiveTechnique = value;
                    mEffect.CurrentTechnique = mActiveTechnique.EffectTechnique;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Internal ctor (Use ContentManager.Load)
        /// </summary>
        /// <param name="effect">- Effect to which should be this material effect bound</param>
        internal MaterialEffect(Effect effect)
        {
            mEffect = effect;
            effect.Lost += new EventHandler(effect_Lost);
            mId = mNextEffectId++;
            
            List<MaterialEffectParam> manualParams;
            List<PerFrameAutoParam> perFrameAutoParams;
            List<PerCameraAutoParam> perCameraAutoParams;
            List<PerRenderableAutoParam> perRenderableAutoParams;
            List<PerSceneAutoParam> perSceneAutoParams;

            //Effect's parameters
            InitEffectParameters(out manualParams, out perFrameAutoParams, out perCameraAutoParams,
                out perRenderableAutoParams, out mRenderTargetAutoParams, out perSceneAutoParams);
            mManualParameters = new MaterialEffectParamCollection(manualParams);

            //Techniques
            Dictionary<NameId, MaterialEffectTechnique> techniques = new Dictionary<NameId, MaterialEffectTechnique>();
            NameId defaultTechniqueNameId = "Default";
            mTechniques = new MaterialEffectTechniqueCollection(techniques);
            foreach (EffectTechnique effectTechnique in effect.Techniques)
            {
                MaterialEffectTechnique materialEffectTechnique = new MaterialEffectTechnique(this, effectTechnique, 
                    ManualParameters, perFrameAutoParams, perCameraAutoParams,
                    perRenderableAutoParams, mRenderTargetAutoParams, perSceneAutoParams);
                techniques.Add(materialEffectTechnique.ScenePass, materialEffectTechnique);
                if (defaultTechniqueNameId == materialEffectTechnique.ScenePass)
                {
                    mDefaultTechnique = materialEffectTechnique;
                }
            }

            //if (null == mDefaultTechnique) //Default technique is required
            //{
            //    throw new ArgumentException("Unable to find effect's technique with name 'Default', which is required");
            //}

            ActiveTechnique = mDefaultTechnique;
        }

        /// <summary>
        /// Finish the application of a last applied material effect. Call this method whenever you are done with all
        /// applications of MaterialEffects (their passes) in a frame and before you are going to use other Microsoft.Xna.Framework.Graphics.Effect
        /// directly or you are going to change device states directly.
        /// </summary>
        /// <example> This sample shows how to use MaterialEffectPass.Apply and MaterialEffect.Finish
        /// <code>
        /// somePass.Apply(...);
        /// //render something
        /// someOtherPass.Apply(...);
        /// //render something
        /// 
        /// MaterialEffect.Finish();
        /// //render something for example using a Microsoft.Xna.Framework.Graphics.Effect
        /// </code>
        /// </example>        
        public static void Finish()
        {
            if (null != mLastAppliedMaterialEffect) //end last pass and effect
            {
                mLastAppliedPass.EffectPass.End();
                mLastAppliedMaterialEffect.mEffect.End();

                mLastAppliedPass = null;
                mLastAppliedMaterialEffect = null;
            }
        }

        /// <summary>
        /// Applies the effect for non-camera and non-renderable unit mode - Used for instance when rendering post-process effects. 
        /// Duplicate state changes are discarded and so multiple following calls of the Apply (with same parameters) method 
        /// of a same MaterialEffect instance has no performance penalty
        /// </summary>
        /// <param name="gameTime">- Actual game time</param>
        /// <param name="pass">- pass that should be used</param>
        internal void Apply(AleGameTime gameTime, MaterialEffectPass pass)
        {
            Apply(gameTime, null, null, null, null, pass);
        }

        /// <summary>
        /// Applies the effect. 
        /// Duplicate state changes are discarded and so multiple following calls of the Apply (with same parameters) method 
        /// of a same MaterialEffect instance has no performance penalty
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="camera"></param>
        /// <param name="renderableUnit"></param>
        /// <param name="renderTargetManager"></param>
        /// <param name="pass"></param>
        internal void Apply(AleGameTime gameTime, ICamera camera, IRenderableUnit renderableUnit,
            RenderTargetManager renderTargetManager, MaterialEffectPass pass)
        {
            Apply(gameTime, camera, renderableUnit, null, renderTargetManager, pass);
        }

        /// <summary>
        /// Applies the effect. 
        /// Duplicate state changes are discarded and so multiple following calls of the Apply (with same parameters) method 
        /// of a same MaterialEffect instance has no performance penalty
        /// </summary>
        /// <param name="gameTime">- Actual game time</param>
        /// <param name="camera">- Actual camera</param>
        /// <param name="renderableUnit">- Actual renderable unit that is going to be rendered</param>
        /// <param name="renderTargetManager"></param>
        /// <param name="pass">- pass that should be used</param>
        /// <param name="scene"></param>
        internal void Apply(AleGameTime gameTime, ICamera camera, IRenderableUnit renderableUnit, Scene.BaseScene scene, 
            RenderTargetManager renderTargetManager, MaterialEffectPass pass)
        {
            MaterialEffectTechnique passTechnique = pass.ParentTechnique;
            //New frame or different technique => update all
            if (gameTime.FrameNum != mLastUpdateFrameNum || passTechnique != ActiveTechnique)
            {
                passTechnique.PerFrameUpdate(gameTime);
                passTechnique.PerCameraUpdate(camera);
                passTechnique.PerRenderTargetManagerUpdate(renderTargetManager);
                passTechnique.PerSceneUpdate(scene);

                mLastUpdateFrameNum = gameTime.FrameNum;
                mLastCameraUpdate = camera;
                mLastRenderTargetManagerUpdate = renderTargetManager;
            }
            else
            {
                //Needs per camera update
                if (camera != mLastCameraUpdate)
                {
                    pass.ParentTechnique.PerCameraUpdate(camera);
                    mLastCameraUpdate = camera;
                }

                if (mLastRenderTargetManagerUpdate != renderTargetManager)
                {
                    pass.ParentTechnique.PerRenderTargetManagerUpdate(renderTargetManager);
                    mLastRenderTargetManagerUpdate = renderTargetManager;
                }

                if (scene != mLastSceneUpdate)
                {
                    passTechnique.PerSceneUpdate(scene);
                    mLastSceneUpdate = scene;
                }
            }
 
            pass.ParentTechnique.PerRenderableUpdate((null != renderableUnit ? renderableUnit.ParentRenderable : null), camera);

            //update manual parameters that are handled by renderable unit
            if (null != renderableUnit)
            {
                renderableUnit.UpdateMaterialEffectParameters();
            }

            //update effect
            if (this != mLastAppliedMaterialEffect || passTechnique != ActiveTechnique) //another effect was applied until now
            {
                if (null != mLastAppliedMaterialEffect) //end previous pass and effect
                {
                    mLastAppliedPass.EffectPass.End();
                    mLastAppliedMaterialEffect.mEffect.End();
                }

                //begin this effect
                ActiveTechnique = passTechnique;
                mEffect.Begin();
                mLastAppliedMaterialEffect = this;

                //begin specified pass and technique
                pass.EffectPass.Begin();
                mLastAppliedPass = pass;
            }
            else
            {
                if (mLastAppliedPass != pass)
                {
                    //end old pass
                    mLastAppliedPass.EffectPass.End();

                    //begin specified pass and technique
                    pass.EffectPass.Begin();
                    mLastAppliedPass = pass;
                }
                else
                {
                    mEffect.CommitChanges();
                }
            }
        }

        #region IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                mEffect.Lost -= effect_Lost;
                mEffect.Dispose();
                
                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable

        #region Ctor
        
        /// <summary>
        /// Initializes effect's parameters
        /// </summary>
        private void InitEffectParameters( out List<MaterialEffectParam> manualParams,
            out List<PerFrameAutoParam> perFrameAutoParams, out List<PerCameraAutoParam> perCameraAutoParams,
            out List<PerRenderableAutoParam> perRenderableAutoParams, out List<RenderTargetAutoParam> renderTargetAutoParams, 
            out List<PerSceneAutoParam> perSceneAutoParams)
        {
            manualParams = new List<MaterialEffectParam>();
            perFrameAutoParams = null;
            perCameraAutoParams = null;
            perRenderableAutoParams = null;
            renderTargetAutoParams = null;
            perSceneAutoParams = null;

            foreach (EffectParameter effectParameter in mEffect.Parameters)
            {
                //whether it is PerFrameAutoParam
                PerFrameAutoParam perFrameAutoParam = PerFrameAutoParam.TryCreateAutoParam(effectParameter);
                if (null != perFrameAutoParam)
                {
                    if (null == perFrameAutoParams)
                    {
                        perFrameAutoParams = new List<PerFrameAutoParam>();
                    }
                    perFrameAutoParams.Add(perFrameAutoParam);
                }
                else
                {
                    //whether it is PerCameraAutoParam
                    PerCameraAutoParam perCameraAutoParam = PerCameraAutoParam.TryCreateAutoParam(effectParameter);
                    if (null != perCameraAutoParam)
                    {
                        if (null == perCameraAutoParams)
                        {
                            perCameraAutoParams =new List<PerCameraAutoParam>();
                        } 
                        perCameraAutoParams.Add(perCameraAutoParam);
                    }
                    else
                    {
                        //whether it is PerRenderableAutoParam
                        PerRenderableAutoParam perRenderableAutoParam = PerRenderableAutoParam.TryCreateAutoParam(effectParameter);
                        if (null != perRenderableAutoParam)
                        {
                            if (null == perRenderableAutoParams)
                            {
                                perRenderableAutoParams = new List<PerRenderableAutoParam>();
                            }
                            perRenderableAutoParams.Add(perRenderableAutoParam);
                        }
                        else
                        {
                            //whether it is RenderTargetAutoParam
                            RenderTargetAutoParam renderTargetAutoParam = RenderTargetAutoParam.TryCreateAutoParam(effectParameter);
                            if (null != renderTargetAutoParam) 
                            {
                                if (null == renderTargetAutoParams)
                                {
                                    renderTargetAutoParams = new List<RenderTargetAutoParam>();
                                }
                                renderTargetAutoParams.Add(renderTargetAutoParam);
                            }
                            else 
                            {
                                //whether it is PerSceneAutoParam
                                PerSceneAutoParam perSceneAutoParam = PerSceneAutoParam.TryCreateAutoParam(effectParameter);
                                if(null != perSceneAutoParam)
                                {
                                    if(null == perSceneAutoParams)
                                    {
                                        perSceneAutoParams = new List<PerSceneAutoParam>();
                                    }
                                    perSceneAutoParams.Add(perSceneAutoParam);
                                }
                                else //it is manual parameter
                                {
                                    MaterialEffectParam manualParam = MaterialEffectParam.TryCreateParam(effectParameter);
                                    if (null != manualParam) //supported type
                                    {
                                        manualParams.Add(manualParam);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion Ctor

        void effect_Lost(object sender, EventArgs e)
        {
            mLastUpdateFrameNum = -1;
            mLastCameraUpdate = null;
            mLastRenderTargetManagerUpdate = null;


            //Textures may be recreated on device reset so thay must be set in the effect again. 
            //(Internal texture object changes but not a xna texture object and so the ValueChanged in GenericMaterialEffectParam will return false and that's not good. 
            foreach (var param in mManualParameters)
            {
                if (param is Texture2DMaterialEffectParam)
                {
                    ((Texture2DMaterialEffectParam)param).Value = null;
                }
                else
                {
                    if (param is Texture3DMaterialEffectParam)
                    {
                        ((Texture3DMaterialEffectParam)param).Value = null;
                    }
                    else
                    {
                        if (param is TextureCubeMaterialEffectParam)
                        {
                            ((TextureCubeMaterialEffectParam)param).Value = null;
                        }
                    }
                }
            }
            if (null != mRenderTargetAutoParams)
            {
                foreach (var rtAutoParam in mRenderTargetAutoParams)
                {
                    rtAutoParam.OnLost();
                }
            }

        }

        #endregion Methods
    }
}
