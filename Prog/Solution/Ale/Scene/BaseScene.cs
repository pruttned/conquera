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
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Input;
using Ale.Content;
using Ale.Sound;

namespace Ale.Scene
{
    public abstract class BaseScene : CompositeFrameListener, IDisposable
    {
        /// <summary>
        /// Scene passes that are rendered before the main scnene pass
        /// </summary>
        private List<ScenePass> mScenePasses;
        private SceneManager mSceneManager;
        private bool mIsDisposed = false;
        private ScenePass mMainScenePass;
        private ParticleSystemManager mParticleSystemManager;
        private RenderableProvider mRenderableProvider = new RenderableProvider();
        private ContentGroup mContent;
        private PostProcessEffectManager mPostProcessEffectManager;

        private List<ISceneDrawableComponent> mSceneDrawableComponents = new List<ISceneDrawableComponent>();

        public SceneManager SceneManager
        {
            get { return mSceneManager; }
        }

        public ParticleSystemManager ParticleSystemManager
        {
            get { return mParticleSystemManager; }
        }

        public ICamera MainCamera
        {
            get { return mMainScenePass.Camera; }
        }

        public RenderableProvider RenderableProvider
        {
            get { return mRenderableProvider; }
        }

        public ContentGroup Content
        {
            get { return mContent; }
        }

        protected List<ISceneDrawableComponent> SceneDrawableComponents
        {
            get { return mSceneDrawableComponents; }
        }

        public PostProcessEffectManager PostProcessEffectManager
        {
            get { return mPostProcessEffectManager; }
        }

        public SoundManager SoundManager
        {
            get { return mSceneManager.SoundManager; }
        }

        public BaseScene(SceneManager sceneManager, ContentGroup content)
        {
            if (null == sceneManager) { throw new NullReferenceException("sceneManager"); }

            mSceneManager = sceneManager;
            mContent = content;

            mScenePasses = CreateScenePasses(sceneManager.GraphicsDeviceManager, sceneManager.RenderTargetManager, content);
            if(null == mScenePasses ||  0 == mScenePasses.Count)
            {
                throw new ArgumentException("No scene passes were created");
            }
            mMainScenePass = mScenePasses[mScenePasses.Count - 1];
            if (!mMainScenePass.NameId.Equals("Default"))
            {
                throw new ArgumentException("Name of the main scene phase must be 'Default'");
            }

            mPostProcessEffectManager = new PostProcessEffectManager(sceneManager.GraphicsDeviceManager);

            mParticleSystemManager = new ParticleSystemManager(sceneManager.ParticleDynamicGeometryManager, MainCamera);
            SceneDrawableComponents.Add(mParticleSystemManager);
            RegisterFrameListener(mParticleSystemManager);

            RegisterRenderableFactories(RenderableProvider);

            MainCamera.ViewTransformationChanged += new CameraTransformationChangedHandler(MainCamera_ViewTransformationChanged);
        }

        public abstract void Update(AleGameTime gameTime);

        public virtual void Draw(AleGameTime gameTime)
        {
            foreach (ScenePass scenePass in mScenePasses)
            {
                if (scenePass.IsEnabled)
                {
                    scenePass.Draw(mSceneManager.GraphicsDeviceManager.GraphicsDevice, mSceneManager.Renderer, gameTime, mSceneManager.RenderTargetManager);
                }
            }
            mPostProcessEffectManager.Apply(gameTime);
        }

        /// <summary>
        /// This method should be called from scene passes
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void EnqueRenderableUnits(AleGameTime gameTime, ScenePass scenePass)
        {
            foreach (var sceneDrawableComponent in mSceneDrawableComponents)
            {
                sceneDrawableComponent.EnqueRenderableUnits(gameTime, mSceneManager.Renderer, scenePass);
            }
        }

        public virtual void RegisterRenderableFactories(RenderableProvider renderableProvider)
        {
            renderableProvider.RegisterFactory(new GraphicModelRenderableFactory(renderableProvider));
            renderableProvider.RegisterFactory(new ParticleSystemRenderableFactory(mParticleSystemManager));
        }

        /// <summary>
        /// Gets the scene pass by its name (Null if it doesn't exists)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Pass or null</returns>
        public ScenePass GetScenePass(string name)
        {
            return GetScenePass(NameId.GetByName(name));
        }

        /// <summary>
        /// Gets the scene pass by its name (Null if it doesn't exists)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Pass or null</returns>
        public ScenePass GetScenePass(NameId name)
        {
            if (null != mScenePasses && 0 != mScenePasses.Count)
            {
                foreach (ScenePass scenePass in mScenePasses)
                {
                    if (scenePass.NameId == name)
                    {
                        return scenePass;
                    }
                }
            }
            return null;
        }

        internal void OnActivated()
        {
            UpdateSoundListener(SoundManager);

            OnActivatedImpl();
        }

        internal void OnDeactivate()
        {
            OnDeactivateImpl();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    MainCamera.ViewTransformationChanged -= MainCamera_ViewTransformationChanged;

                    foreach (ScenePass scenePass in mScenePasses)
                    {
                        scenePass.Dispose();
                    }
                    mPostProcessEffectManager.Dispose();
                }
                mIsDisposed = true;
            }
        }
        
        //protected abstract void EnqueRenderableUnits(AleGameTime gameTime, Renderer renderer, ICamera camera);
        
        /// <summary>
        /// Creates scene passes. Last scene pass is considered as a main scene phase. It name must be "Default"
        /// </summary>
        /// <param name="GraphicsDeviceManager"></param>
        /// <param name="renderTargetManager"></param>
        /// <param name="content"></param>
        /// <returns>Scene passes or null</returns>
        protected abstract List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, RenderTargetManager renderTargetManager, ContentGroup content);

        protected virtual void OnActivatedImpl()
        {
        }

        protected virtual void OnDeactivateImpl()
        {
        }

        protected abstract void UpdateSoundListener(SoundManager SoundManager);

        private void MainCamera_ViewTransformationChanged(ICamera camera)
        {
            UpdateSoundListener(SoundManager);
        }
    }
}
