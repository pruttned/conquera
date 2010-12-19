//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
    public class Renderer
    {
        #region Fields

        /// <summary>
        /// Currently active camera
        /// </summary>
        private ICamera mActiveCamera = null;

        /// <summary>
        /// Render layers
        /// </summary>
        private SortedList<int, RenderQueue> mRenderLayers = new SortedList<int,RenderQueue>(10);

        private int mLastRenderQueueLayer = -1;

        private RenderQueue mLastRenderQueue = null;

        private RenderTargetManager mRenderTargetManager;

        /// <summary>
        /// Active scene pass
        /// </summary>
        private NameId mActiveScenePass;

        private NameId mDefaultScenePass = "Default";

        private Scene.BaseScene mActviveScene = null;

        private int mEnquedRenderableUnitCnt = 0;

        #endregion Fields

        #region Properties
        
        /// <summary>
        /// Gets the currently active camera
        /// </summary>
        public ICamera ActiveCamera
        {
            get { return mActiveCamera; }
        }

        #endregion Properties

        #region Methods

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
        /// <param name="renderTargetManager">- (nullable)</param>
        public void Begin(ICamera camera, RenderTargetManager renderTargetManager)
        {
            Begin(camera, renderTargetManager, null, null);
        }

        /// <summary>
        /// Begins the render process. 
        /// </summary>
        /// <param name="camera">- Currently active camera</param>
        /// <param name="renderTargetManager">- (nullable)</param>
        /// <param name="actviveScene">- (nullable)</param>
        /// <param name="scenePass">- Active scene pass (null = default)</param>
        public void Begin(ICamera camera, RenderTargetManager renderTargetManager, Scene.BaseScene actviveScene, NameId scenePass)
        {
            if (null != mActiveCamera)
            {
                throw new InvalidOperationException("Cannot call two subsequent Begin methods without calling End method");
            }

            mRenderTargetManager = renderTargetManager;
            mActiveCamera = camera;
            mActviveScene = actviveScene;
            
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
                mEnquedRenderableUnitCnt++;
                renderQueue.Enqueue(renderableUnit, renderableUnit.Material.DefaultTechnique, ActiveCamera);
                return true;
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

            if (0 < mEnquedRenderableUnitCnt)
            {
                MaterialPass lastUsedMaterialPass = null;

                //Opaque
                for (int i = mRenderLayers.Count - 1; i >= 0; --i)
                {
                    mRenderLayers.Values[i].ForEachOpaqueObject(
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

                //Transparent
                for (int i = 0, count = mRenderLayers.Count; i < count; ++i)
                {
                    mRenderLayers.Values[i].ForEachTransparentObject(
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

                MaterialEffect.Finish();

                //Clear render queues for next frame
                IList<RenderQueue> queues = mRenderLayers.Values;
                for (int i = 0; i < queues.Count; ++i)
                {
                    queues[i].Clear();
                }

                mEnquedRenderableUnitCnt = 0;
            }

            mActiveCamera = null;
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

        #endregion Methods
    }
}
