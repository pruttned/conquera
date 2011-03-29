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

namespace Ale.Graphics
{
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
    public sealed class Renderer : IDisposable
    {
        #region Fields

        private bool mIsDisposed = false;

        AleRenderTarget mColorRenderTarget;
        AleRenderTarget mNormalRenderTarget;
        AleRenderTarget mDepthRenderTarget;

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

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the currently active camera
        /// </summary>
        public ICamera ActiveCamera
        {
            get { return mActiveCamera; }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return RenderTargetManager.GraphicsDeviceManager; }
        }
        public RenderTargetManager RenderTargetManager { get; private set; }

        #endregion Properties

        #region Methods

        public Renderer(RenderTargetManager renderTargetManager)
        {
            if (null == renderTargetManager) throw new ArgumentNullException("renderTargetManager");

            RenderTargetManager = renderTargetManager;
            GraphicsDeviceManager.DeviceReset += new EventHandler(GraphicsDeviceManager_DeviceReset);
        }

        /// <summary>
        /// Begins the render process. 
        /// </summary>
        /// <param name="camera">- Currently active camera</param>
        public void Begin(ICamera camera)
        {
            Begin(camera, null, null, null);
        }

        /// <summary>
        /// Begins the render process. 
        /// </summary>
        /// <param name="camera">- Currently active camera</param>
        /// <param name="actviveScene">- (nullable)</param>
        /// <param name="scenePass">- Active scene pass (null = default)</param>
        /// <param name="renderTarget"></param>
        public void Begin(ICamera camera, Scene.BaseScene actviveScene, NameId scenePass, AleRenderTarget renderTarget)
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

        /// <summary>
        /// Renders all the enqueued renderables
        /// </summary>
        public void End(AleGameTime gameTime)
        {
            if (null == mActiveCamera)
            {
                throw new InvalidOperationException("Cannot call End method without calling Begin method first");
            }

            MaterialPass lastUsedMaterialPass = null;

            if (0 < mEnquedRenderableUnitCnt)
            {

                //Deffered pass
                //if (!mContentIsLoaded)
                //{
                //    LoadContent();
                //}
                //mColorRenderTarget.Begin(0);
                //mNormalRenderTarget.Begin(1);
                //mDepthRenderTarget.Begin(2);

                //for (int i = mRenderLayers.Count - 1; i >= 0; --i)
                //{
                //    if (0 < mRenderLayers.Values[i].LightReceivingOpaqueRenderableUnitsCnt)
                //    {
                //        mRenderLayers.Values[i].ForEachLightReceivingOpaqueRenderable(
                //            delegate(IRenderableUnit renderableUnit, MaterialPass materialPass)
                //            {
                //                if (lastUsedMaterialPass != materialPass)
                //                {
                //                    materialPass.Apply();
                //                    lastUsedMaterialPass = materialPass;
                //                }

                //                materialPass.MaterialEffectPass.Apply(gameTime, ActiveCamera, renderableUnit, mActviveScene, mRenderTargetManager);

                //                renderableUnit.Render(gameTime);
                //            });
                //    }
                //}

                //mColorRenderTarget.End();
                //mNormalRenderTarget.End();
                //mDepthRenderTarget.End();
            }


            //Forward pass

            if (null != mActiveRenderTarget)
            {
                mActiveRenderTarget.Begin();
            }
            else
            {
                GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, null);
                GraphicsDeviceManager.GraphicsDevice.Clear(Color.White);
            }

            if (0 < mEnquedRenderableUnitCnt)
            {

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

                                materialPass.MaterialEffectPass.Apply(gameTime, ActiveCamera, renderableUnit, mActviveScene, RenderTargetManager);

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

                                materialPass.MaterialEffectPass.Apply(gameTime, ActiveCamera, renderableUnit, mActviveScene, RenderTargetManager);

                                renderableUnit.Render(gameTime);
                            });
                    }
                }

                MaterialEffect.Finish();
            }
            if (null != mActiveRenderTarget)
            {
                mActiveRenderTarget.End();
            }


            //Clear render queues for next frame
            IList<RenderQueue> queues = mRenderLayers.Values;
            for (int i = 0; i < queues.Count; ++i)
            {
                queues[i].Clear();
            }

            mEnquedRenderableUnitCnt = 0;

            mActiveCamera = null;
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                UnloadContent();
                GraphicsDeviceManager.DeviceReset -= GraphicsDeviceManager_DeviceReset;

                GC.SuppressFinalize(this);
                mIsDisposed = true;
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
            UnloadContent();
            mContentIsLoaded = false;
        }

        private void LoadContent()
        {
            PresentationParameters pp = GraphicsDeviceManager.GraphicsDevice.PresentationParameters;
            mColorRenderTarget = RenderTargetManager.CreateRenderTarget("ScreenColorMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Color, DepthFormat.Depth16);
            mDepthRenderTarget = RenderTargetManager.CreateRenderTarget("ScreenDepthMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Color, DepthFormat.Depth16);
            mNormalRenderTarget = RenderTargetManager.CreateRenderTarget("ScreenNormalMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Color, DepthFormat.Depth16);

            mContentIsLoaded = true;
        }

        private void UnloadContent()
        {
            if (mContentIsLoaded)
            {
                if (null != mColorRenderTarget)
                {
                    RenderTargetManager.DestroyRenderTarget(mColorRenderTarget.Name);
                }
                if (null != mDepthRenderTarget)
                {
                    RenderTargetManager.DestroyRenderTarget(mDepthRenderTarget.Name);
                }
                if (null != mNormalRenderTarget)
                {
                    RenderTargetManager.DestroyRenderTarget(mNormalRenderTarget.Name);
                }
            }
        }

        #endregion Methods
    }
}
