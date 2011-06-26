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
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Input;
using Ale.Content;
using Ale.Sound;
using Ale.SpecialEffects;

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
        private IParticleSystemManager mParticleSystemManager;
        private IRenderableProvider mRenderableProvider;
        private ContentGroup mContent;
        private IPostProcessEffectManager mPostProcessEffectManager;
        private IRenderer mRenderer;
        private IRenderTargetManager mRenderTargetManager;

        private List<ISceneDrawableComponent> mSceneDrawableComponents = new List<ISceneDrawableComponent>();

        public IRenderer Renderer
        {
            get { return mRenderer; }
        }
        public IRenderTargetManager RenderTargetManager
        {
            get { return mRenderTargetManager; }
        }
        public SceneManager SceneManager
        {
            get { return mSceneManager; }
        }

        public IParticleSystemManager ParticleSystemManager
        {
            get { return mParticleSystemManager; }
        }

        public ICamera MainCamera
        {
            get { return mMainScenePass.Camera; }
        }

        public IRenderableProvider RenderableProvider
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

        public IPostProcessEffectManager PostProcessEffectManager
        {
            get { return mPostProcessEffectManager; }
        }

        public SoundManager SoundManager
        {
            get { return mSceneManager.SoundManager; }
        }
        public ISpecialEffectManager SpecialEffectManager { get; private set; }

        public IAleServiceProvider Services { get; private set; }

        public BaseScene(SceneManager sceneManager, ContentGroup content)
        {
            if (null == sceneManager) { throw new NullReferenceException("sceneManager"); }

            Services = new AleServiceProvider(sceneManager.Services);

            mSceneManager = sceneManager;
            mContent = content;
            Services.RegisterService(typeof(ContentGroup), mContent);
            mRenderTargetManager = CreateRenderTargetManager();
            Services.RegisterService(typeof(IRenderTargetManager), mRenderTargetManager);
            mRenderer = CreateRenderer();
            Services.RegisterService(typeof(IRenderer), mRenderer);
            mPostProcessEffectManager = CreatePostProcessEffectManager();
            Services.RegisterService(typeof(IPostProcessEffectManager), mPostProcessEffectManager);

            mScenePasses = CreateScenePasses(sceneManager.GraphicsDeviceManager, RenderTargetManager, content);
            if(null == mScenePasses ||  0 == mScenePasses.Count)
            {
                throw new ArgumentException("No scene passes were created");
            }
            mMainScenePass = mScenePasses[mScenePasses.Count - 1];
            if (!mMainScenePass.NameId.Equals("Default"))
            {
                throw new ArgumentException("Name of the main scene phase must be 'Default'");
            }

            mParticleSystemManager = CreateParticleSystemManager();
            SceneDrawableComponents.Add(mParticleSystemManager);
            RegisterFrameListener(mParticleSystemManager);
            Services.RegisterService(typeof(IParticleSystemManager), mParticleSystemManager);

            mRenderableProvider = CreateRenderableProvider();
            RegisterRenderableFactories(mRenderableProvider);
            Services.RegisterService(typeof(IRenderableProvider), mRenderableProvider);

            SpecialEffectManager = CreateSpecialEffectManager();
            SceneDrawableComponents.Add(SpecialEffectManager);
            Services.RegisterService(typeof(ISpecialEffectManager), SpecialEffectManager);

            MainCamera.ViewTransformationChanged += new CameraTransformationChangedHandler(MainCamera_ViewTransformationChanged);
        }

        public abstract void Update(AleGameTime gameTime);

        public virtual void Draw(AleGameTime gameTime)
        {
            foreach (ScenePass scenePass in mScenePasses)
            {
                if (scenePass.IsEnabled)
                {
                    scenePass.Draw(mSceneManager.GraphicsDeviceManager.GraphicsDevice, Renderer, gameTime);
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
                sceneDrawableComponent.EnqueRenderableUnits(gameTime, Renderer, scenePass);
            }
        }

        public virtual void RegisterRenderableFactories(IRenderableProvider renderableProvider)
        {
            renderableProvider.RegisterFactory(new GraphicModelRenderableFactory(renderableProvider));
            renderableProvider.RegisterFactory(new ParticleSystemRenderableFactory(mParticleSystemManager));
            renderableProvider.RegisterFactory(new PointLightRenderableFactory());
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
                    foreach (ScenePass scenePass in mScenePasses)
                    {
                        scenePass.Dispose();
                    }
                    MainCamera.ViewTransformationChanged -= MainCamera_ViewTransformationChanged;

                    mPostProcessEffectManager.Dispose();
                    mRenderer.Dispose();
                    mRenderTargetManager.Dispose();
                    mParticleSystemManager.Dispose();
                    SpecialEffectManager.Dispose();
                    mRenderableProvider.Dispose();
                }
                mIsDisposed = true;
            }
        }
        
        /// <summary>
        /// Creates scene passes. Last scene pass is considered as a main scene phase. It name must be "Default"
        /// </summary>
        /// <param name="GraphicsDeviceManager"></param>
        /// <param name="renderTargetManager"></param>
        /// <param name="content"></param>
        /// <returns>Scene passes or null</returns>
        protected abstract List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, IRenderTargetManager renderTargetManager, ContentGroup content);

        protected virtual void OnActivatedImpl()
        {
        }

        protected virtual void OnDeactivateImpl()
        {
        }

        protected abstract void UpdateSoundListener(SoundManager SoundManager);

        protected virtual IRenderableProvider CreateRenderableProvider()
        {
            return new RenderableProvider();
        }
        protected virtual IParticleSystemManager CreateParticleSystemManager()
        {
            return new ParticleSystemManager(SceneManager.ParticleDynamicGeometryManager, MainCamera);
        }
        protected virtual IRenderTargetManager CreateRenderTargetManager()
        {
            return new RenderTargetManager(SceneManager.GraphicsDeviceManager);
        }
        protected virtual IPostProcessEffectManager CreatePostProcessEffectManager()
        {
            return new PostProcessEffectManager(SceneManager.GraphicsDeviceManager);
        }
        protected virtual IRenderer CreateRenderer()
        {
            return new Renderer(RenderTargetManager, Content);
        }
        protected virtual ISpecialEffectManager CreateSpecialEffectManager()
        {
            var specialEffectManager =  new SpecialEffectManager(Content, Services);
            specialEffectManager.RegisterTriggerAction(new PsysExplosionTimeTriggerAction(ParticleSystemManager, Content));
            specialEffectManager.RegisterTriggerAction(new Sound3dTimeTriggerAction(SoundManager, Content));
            specialEffectManager.RegisterTriggerAction(new DestroyObjTimeTriggerAction());

            return specialEffectManager;
        } 
        private void MainCamera_ViewTransformationChanged(ICamera camera)
        {
            UpdateSoundListener(SoundManager);
        }
    }
}
