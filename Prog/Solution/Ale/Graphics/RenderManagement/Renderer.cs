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
using Microsoft.Xna.Framework;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;

namespace Ale.Graphics
{

    public interface IRenderer : IDisposable
    {
        void BeginForward(ICamera camera, Scene.BaseScene actviveScene, NameId scenePass, AleRenderTarget renderTarget);
        void BeginDeferred(ICamera camera, Scene.BaseScene actviveScene, NameId scenePass);
        bool EnqueueRenderable(IRenderableUnit renderableUnit);
        bool EnqueLight(ILightRenderableUnit light);
        void End(AleGameTime gameTime);
        ICamera ActiveCamera { get; }
    }


    /// <summary>
    /// Manages the rendering process
    /// </summary>
    /// <example> This sample shows how to use Renderer class
    /// <code>
    /// renderer.Begin(SomeCamera);
    /// renderer.EnqueueRenderable(..);
    /// renderer.EnqueueRenderable(..);
    /// renderer.EnqueueRenderable(..);
    /// //...
    /// renderer.End(gameTime); 
    ///
    /// renderer.Begin(SomeOtherCamera);
    /// renderer.EnqueueRenderable(..);
    /// renderer.EnqueueRenderable(..);
    /// renderer.EnqueueRenderable(..);
    /// //...
    /// renderer.End(gameTime); 
    /// </code>
    /// </example>        
    public sealed class Renderer : IRenderer
    {
        //Deferred rendering  is based on http://www.catalinzima.com/tutorials/deferred-rendering-in-xna/

        #region Fields

        private bool mIsDisposed = false;

        //deferred
        private AleRenderTarget mColorRenderTarget;
        private AleRenderTarget mNormalRenderTarget;
        private AleRenderTarget mDepthRenderTarget;
        private AleRenderTarget mLightRenderTarget;
        private MaterialEffect mDeferredClearEffect;
        private MaterialEffect mCombineEffect;
        private FullScreenQuad mFullScreenQuad;
        private List<ILightRenderableUnit> mEnquedLights = new List<ILightRenderableUnit>();
        private bool mIsDeferred;

        /// <summary>
        /// Currently active camera
        /// </summary>
        private ICamera mActiveCamera = null;

        /// <summary>
        /// Render layers
        /// </summary>
        private SortedList<int, RenderQueue> mRenderLayers = new SortedList<int, RenderQueue>(10);

        private int mLastRenderQueueLayer = -1;

        private RenderQueue mLastRenderQueue = null;

        /// <summary>
        /// Active scene pass
        /// </summary>
        private NameId mActiveScenePass;

        private NameId mDefaultScenePass = "Default";

        private Scene.BaseScene mActviveScene = null;

        private int mEnquedRenderableUnitCnt = 0;

        public bool mContentIsLoaded = false;

        public AleRenderTarget mActiveRenderTarget = null;
       
        private IRenderTargetManager mRenderTargetManager;


        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the currently active camera
        /// </summary>
        public ICamera ActiveCamera
        {
            get { return mActiveCamera; }
        }

        private GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mRenderTargetManager.GraphicsDeviceManager; }
        }

        #endregion Properties

        #region Methods

        public Renderer(IRenderTargetManager renderTargetManager, ContentGroup content)
        {
            if (null == renderTargetManager) throw new ArgumentNullException("renderTargetManager");

            mRenderTargetManager = renderTargetManager;
            GraphicsDeviceManager.DeviceReset += new EventHandler(GraphicsDeviceManager_DeviceReset);

            mFullScreenQuad = new FullScreenQuad(GraphicsDeviceManager);
            LoadEffects(content);
        }

        /// <summary>
        /// Begins the render process. 
        /// </summary>
        /// <param name="camera">- Currently active camera</param>
        /// <param name="actviveScene">- (nullable)</param>
        /// <param name="scenePass">- Active scene pass (null = default)</param>
        /// <param name="renderTarget"></param>
        public void BeginForward(ICamera camera, Scene.BaseScene actviveScene, NameId scenePass, AleRenderTarget renderTarget)
        {
            if (null != mActiveCamera)
            {
                throw new InvalidOperationException("Cannot call two subsequent Begin methods without calling End method");
            }

            mActiveCamera = camera;
            mActviveScene = actviveScene;
            mActiveRenderTarget = renderTarget;

            if (mDefaultScenePass == scenePass)
            {
                mActiveScenePass = null;
            }
            else
            {
                mActiveScenePass = scenePass;
            }
            mIsDeferred = false;
        }

        /// <summary>
        /// Begins the render process. 
        /// </summary>
        /// <param name="camera">- Currently active camera</param>
        /// <param name="actviveScene">- (nullable)</param>
        /// <param name="scenePass">- Active scene pass (null = default)</param>
        /// <param name="renderTarget"></param>
        public void BeginDeferred(ICamera camera, Scene.BaseScene actviveScene, NameId scenePass)
        {
            BeginForward(camera, actviveScene, scenePass, null);
            mIsDeferred = true;
        }

        /// <summary>
        /// Enqueue a renderable unit for rendering in actual frame
        /// </summary>
        /// <param name="renderableUnit">- Renderable unit that should be enqueued for rendering</param>
        /// <return>Whether was renderable unit enqued (true) or filtered out</return>
        public bool EnqueueRenderable(IRenderableUnit renderableUnit)
        {
            if (null == mActiveCamera)
            {
                throw new InvalidOperationException("You must first call Renderer.Begin method in order to call EnqueueRenderable");
            }

            RenderQueue renderQueue = GetLayerRenderQueue(renderableUnit.Material.RenderLayer);

            if (null == mActiveScenePass)
            {
                if (null != renderableUnit.Material.DefaultTechnique)
                {
                    mEnquedRenderableUnitCnt++;
                    renderQueue.Enqueue(renderableUnit, renderableUnit.Material.DefaultTechnique, ActiveCamera);
                    return true;
                }
            }
            else
            {
                MaterialTechnique technique;
                if (renderableUnit.Material.Techniques.TryGetValue(mActiveScenePass, out technique)) //enque only if material supports active scene pass
                {
                    mEnquedRenderableUnitCnt++;
                    renderQueue.Enqueue(renderableUnit, technique, ActiveCamera);
                    return true;
                }
            }
            return false;
        }

        public bool EnqueLight(ILightRenderableUnit light)
        {
            if (mIsDeferred)
            {
                mEnquedLights.Add(light);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Renders all the enqueued renderables
        /// </summary>
        public void End(AleGameTime gameTime)
        {
            if (null == mActiveCamera)
            {
                throw new InvalidOperationException("Cannot call End method without calling Begin method first");
            }

            if (mIsDeferred)
            {
                EndDeferred(gameTime);
            }
            else
            {
                EndForward(gameTime);
            }

            //Clear render queues for next frame
            IList<RenderQueue> queues = mRenderLayers.Values;
            for (int i = 0; i < queues.Count; ++i)
            {
                queues[i].Clear();
            }
            mEnquedLights.Clear();
            mEnquedRenderableUnitCnt = 0;

            mActiveCamera = null;
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                UnloadRenderTargets();
                GraphicsDeviceManager.DeviceReset -= GraphicsDeviceManager_DeviceReset;
                mFullScreenQuad.Dispose();

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        private void EndDeferred(AleGameTime gameTime)
        {
            //???? todo: Optimize buffers ????
            //http://www.gamedev.net/topic/562818-xna-deferred-rendering---performance-issues/
            //http://aras-p.info/texts/CompactNormalStorage.html
            //????


            //////////////////////
            //Deferred pass
            //////////////////////
            if (!mContentIsLoaded)
            {
                LoadRenderTargets();
            }

            if (0 < mEnquedRenderableUnitCnt && HasLightReceivingOpaqueRenderables())
            {
                MaterialPass lastUsedMaterialPass = null;

                mColorRenderTarget.Begin(0);
                mNormalRenderTarget.Begin(1);
                mDepthRenderTarget.Begin(2);

                //clear
                GraphicsDeviceManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
                mFullScreenQuad.Draw(mDeferredClearEffect, gameTime);

                //opaque
                for (int i = mRenderLayers.Count - 1; i >= 0; --i)
                {
                    if (0 < mRenderLayers.Values[i].LightReceivingOpaqueRenderableUnitsCnt)
                    {
                        mRenderLayers.Values[i].ForEachLightReceivingOpaqueRenderable(
                            delegate(IRenderableUnit renderableUnit, MaterialPass materialPass)
                            {
                                if (lastUsedMaterialPass != materialPass)
                                {
                                    materialPass.Apply();
                                    lastUsedMaterialPass = materialPass;
                                }

                                materialPass.MaterialEffectPass.Apply(gameTime, ActiveCamera, renderableUnit, mActviveScene, mRenderTargetManager);

                                renderableUnit.Render(gameTime);
                            });
                    }
                }

                mColorRenderTarget.End();
                mNormalRenderTarget.End();
                mDepthRenderTarget.End();

                //Lights
                // if (0 < mEnquedLights.Count)   //- clear target
                {
                    mLightRenderTarget.Begin();

                    foreach (var light in mEnquedLights)
                    {
                        var matPass = light.Material.DefaultTechnique.Passes[0];
                        if (lastUsedMaterialPass != matPass)
                        {
                            matPass.Apply();
                            lastUsedMaterialPass = matPass;
                        }

                        matPass.MaterialEffectPass.Apply(gameTime, ActiveCamera, light, mActviveScene, mRenderTargetManager);

                        light.Render(gameTime);
                    }

                    mLightRenderTarget.End();
                }
            }


            //combine + restore depth
            GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, null);
            GraphicsDeviceManager.GraphicsDevice.Clear(Color.White);
            mFullScreenQuad.Draw(mCombineEffect, gameTime, mRenderTargetManager);

            //////////////////////
            //Forward pass
            //////////////////////
            DrawForward(gameTime);

            MaterialEffect.Finish();
        }

        private void EndForward(AleGameTime gameTime)
        {

            if (null != mActiveRenderTarget)
            {
                mActiveRenderTarget.Begin();
            }
            else
            {
                GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, null);
                GraphicsDeviceManager.GraphicsDevice.Clear(Color.White);
            }

            DrawForward(gameTime);

            MaterialEffect.Finish();


            if (null != mActiveRenderTarget)
            {
                mActiveRenderTarget.End();
            }
        }

        private void DrawForward(AleGameTime gameTime)
        {
            if (0 < mEnquedRenderableUnitCnt)
            {
                MaterialPass lastUsedMaterialPass = null;

                //Opaque
                for (int i = mRenderLayers.Count - 1; i >= 0; --i)
                {
                    if (0 < mRenderLayers.Values[i].OpaqueRenderableUnitsCnt)
                    {
                        mRenderLayers.Values[i].ForEachOpaqueRenderable(
                            delegate(IRenderableUnit renderableUnit, MaterialPass materialPass)
                            {
                                if (lastUsedMaterialPass != materialPass)
                                {
                                    materialPass.Apply();
                                    lastUsedMaterialPass = materialPass;
                                }

                                materialPass.MaterialEffectPass.Apply(gameTime, ActiveCamera, renderableUnit, mActviveScene, mRenderTargetManager);

                                renderableUnit.Render(gameTime);
                            });
                    }
                }

                //Transparent
                for (int i = 0, count = mRenderLayers.Count; i < count; ++i)
                {
                    if (0 < mRenderLayers.Values[i].TransparentRenderableUnitsCnt)
                    {
                        mRenderLayers.Values[i].ForEachTransparentRenderable(
                            delegate(IRenderableUnit renderableUnit, MaterialPass materialPass)
                            {
                                if (lastUsedMaterialPass != materialPass)
                                {
                                    materialPass.Apply();
                                    lastUsedMaterialPass = materialPass;
                                }

                                materialPass.MaterialEffectPass.Apply(gameTime, ActiveCamera, renderableUnit, mActviveScene, mRenderTargetManager);

                                renderableUnit.Render(gameTime);
                            });
                    }
                }
            }
        }

        /// <summary>
        /// Gets the render queue for a give layer. If specified layer doesn't exists, then it is created
        /// </summary>
        /// <param name="layer">- Layer whose queue should be retreived</param>
        /// <returns>Render queue for a give layer</returns>
        private RenderQueue GetLayerRenderQueue(int layer)
        {
            if (mLastRenderQueueLayer == layer)
            {
                return mLastRenderQueue;
            }

            RenderQueue renderQueue;
            if (!mRenderLayers.TryGetValue(layer, out renderQueue))
            {
                renderQueue = new RenderQueue();
                mRenderLayers.Add(layer, renderQueue);
            }
            mLastRenderQueue = renderQueue;
            mLastRenderQueueLayer = layer;
            return renderQueue;
        }

        private void GraphicsDeviceManager_DeviceReset(object sender, EventArgs e)
        {
            UnloadRenderTargets();
            mContentIsLoaded = false;
        }

        private void LoadRenderTargets()
        {
            PresentationParameters pp = GraphicsDeviceManager.GraphicsDevice.PresentationParameters;
            mColorRenderTarget = mRenderTargetManager.CreateRenderTarget("ScreenColorMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Color, DepthFormat.Depth24);
            mDepthRenderTarget = mRenderTargetManager.CreateRenderTarget("ScreenDepthMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Single, DepthFormat.Depth24);
            mNormalRenderTarget = mRenderTargetManager.CreateRenderTarget("ScreenNormalMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Color, DepthFormat.Depth16);
            mLightRenderTarget = mRenderTargetManager.CreateRenderTarget("ScreenLightMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Color);
            mColorRenderTarget.ClearOnBegin = false;
            mDepthRenderTarget.ClearOnBegin = false;
            mNormalRenderTarget.ClearOnBegin = false;
            mLightRenderTarget.ClearOnBegin = true;
            mLightRenderTarget.Color = Color.TransparentBlack;

            mContentIsLoaded = true;
        }

        private void LoadEffects(ContentGroup content)
        {
            mDeferredClearEffect = content.Load<MaterialEffect>(@"Deferred/DeferredClearFx");
            mCombineEffect = content.Load<MaterialEffect>(@"Deferred/CombineFx");
        }

        private void UnloadRenderTargets()
        {
            if (mContentIsLoaded)
            {
                if (null != mColorRenderTarget)
                {
                    mRenderTargetManager.DestroyRenderTarget(mColorRenderTarget.Name);
                }
                if (null != mDepthRenderTarget)
                {
                    mRenderTargetManager.DestroyRenderTarget(mDepthRenderTarget.Name);
                }
                if (null != mNormalRenderTarget)
                {
                    mRenderTargetManager.DestroyRenderTarget(mNormalRenderTarget.Name);
                }
                if (null != mLightRenderTarget)
                {
                    mRenderTargetManager.DestroyRenderTarget(mLightRenderTarget.Name);
                }
            }
        }

        private bool HasLightReceivingOpaqueRenderables()
        {
            for (int i = mRenderLayers.Count - 1; i >= 0; --i)
            {
                if (0 < mRenderLayers.Values[i].LightReceivingOpaqueRenderableUnitsCnt)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Methods
    }
}
