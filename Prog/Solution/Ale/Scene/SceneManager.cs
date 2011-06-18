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
using System.Windows.Forms;
using Ale.Sound;
using Ale.Content;

namespace Ale.Scene
{
    public class SceneManager : CompositeFrameListener, IDisposable
    {
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private IMouseManager mMouseManager;
        private IKeyboardManager mKeyboardManager;
        private IParticleDynamicGeometryManager mParticleDynamicGeometryManager;
        private BaseScene mActiveScene;
        private bool mExitingApp = false;

        private bool mIsDisposed = false;

        public BaseScene ActiveScene
        {
            get { return mActiveScene; }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mGraphicsDeviceManager; }
        }

        /// <summary>
        /// Gets the mouse manager
        /// </summary>
        public IMouseManager MouseManager
        {
            get { return mMouseManager; }
        }

        /// <summary>
        /// Gets the keyboard manager
        /// </summary>
        public IKeyboardManager KeyboardManager
        {
            get { return mKeyboardManager; }
        }

        public IParticleDynamicGeometryManager ParticleDynamicGeometryManager
        {
            get { return mParticleDynamicGeometryManager; }
        }

        public SoundManager SoundManager { get; private set; }

        public IAleServiceProvider Services { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="renderControl"></param>
        /// <param name="graphicsDeviceManager"></param>
        /// <param name="services"></param>
        /// <param name="content"></param>
        public SceneManager(GraphicsDeviceManager graphicsDeviceManager, Control renderControl, IAleServiceProvider services, AleContentManager content)
        {
            if (null == graphicsDeviceManager) { throw new NullReferenceException("graphicsDeviceManager"); }
            if (null == renderControl) { throw new NullReferenceException("renderControl"); }
            if (null == services) throw new ArgumentNullException("services");
            if (null == content) throw new ArgumentNullException("content");

            Services = services;
            mGraphicsDeviceManager = graphicsDeviceManager;

            mMouseManager = CreateMouseManager(renderControl);
            RegisterFrameListener(mMouseManager);
            Services.RegisterService(typeof(IMouseManager), mMouseManager);

            mKeyboardManager = CreateKeyboardManager();
            RegisterFrameListener(mKeyboardManager);
            Services.RegisterService(typeof(IKeyboardManager), mKeyboardManager);

            mParticleDynamicGeometryManager = CreateParticleDynamicGeometryManager();
            RegisterFrameListener(mParticleDynamicGeometryManager);
            Services.RegisterService(typeof(IParticleDynamicGeometryManager), mParticleDynamicGeometryManager);

            SoundManager = new SoundManager(content.RootDirectory);
            RegisterFrameListener(SoundManager);
            Services.RegisterService(typeof(SoundManager), SoundManager);
        }

        public void ActivateScene(BaseScene scene)
        {
            if (null == scene) throw new ArgumentNullException("scene");
            if (this != scene.SceneManager)
            {
                throw new ArgumentException("Scene belongs to a diferent scene manager");
            }

            if (mActiveScene != scene)
            {

                if (null != mActiveScene)
                {
                    RemoveFrameListener(mActiveScene);
                }

                //temp (caching ?)
                if (null != mActiveScene)
                {
                    mActiveScene.OnDeactivate();
                    //temp (caching ?)
                    mActiveScene.Dispose();
                }
                mActiveScene = scene;
                mActiveScene.OnActivated();

                RegisterFrameListener(mActiveScene);
            }
        }

        public void ExitApplication()
        {
            mExitingApp = true;
        }

        /// <summary>
        /// Draws the frame
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void DrawActiveScene(AleGameTime gameTime)
        {
            if (null != mActiveScene)
            {
                mActiveScene.Draw(gameTime);
            }
        }

        /// <summary>
        /// Updates the game logic
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual bool UpdateActiveScene(AleGameTime gameTime)
        {
            if (mExitingApp)
            {
                return false;
            }

            if (null != mActiveScene)
            {
                mActiveScene.Update(gameTime);
            }
            return true;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void OnAppActivate()
        {
            SoundManager.PauseAll(false);
        }

        public void OnAppDeactivate()
        {
            SoundManager.PauseAll(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    if (null != mActiveScene)
                    {
                        mActiveScene.Dispose();
                    }
                    mParticleDynamicGeometryManager.Dispose();
                    mMouseManager.Dispose();
                    SoundManager.Dispose();

                }
                mIsDisposed = true;
            }
        }

        protected virtual IKeyboardManager CreateKeyboardManager()
        {
            return new KeyboardManager();
        }

        protected virtual IMouseManager CreateMouseManager(System.Windows.Forms.Control renderControl)
        {
            return new MouseManager(renderControl);
        }

        protected virtual IParticleDynamicGeometryManager CreateParticleDynamicGeometryManager()
        {
            return new ParticleDynamicGeometryManager(GraphicsDeviceManager);
        }
    }
}
