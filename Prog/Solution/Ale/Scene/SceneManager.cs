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
using System.Windows.Forms;
using Ale.Sound;

namespace Ale.Scene
{
    public class SceneManager : CompositeFrameListener, IDisposable
    {
        private Renderer mRenderer;
        private RenderTargetManager mRenderTargetManager;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private MouseManager mMouseManager;
        private KeyboardManager mKeyboardManager;
        private ParticleDynamicGeometryManager mParticleDynamicGeometryManager;
        private BaseScene mActiveScene;
        private bool mExitingApp = false;

        private bool mIsDisposed = false;

        public Renderer Renderer
        {
            get { return mRenderer; }
        }

        public RenderTargetManager RenderTargetManager
        {
            get { return mRenderTargetManager; }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mGraphicsDeviceManager; }
        }

        /// <summary>
        /// Gets the mouse manager
        /// </summary>
        public MouseManager MouseManager
        {
            get { return mMouseManager; }
        }

        /// <summary>
        /// Gets the keyboard manager
        /// </summary>
        public KeyboardManager KeyboardManager
        {
            get { return mKeyboardManager; }
        }

        public ParticleDynamicGeometryManager ParticleDynamicGeometryManager
        {
            get { return mParticleDynamicGeometryManager; }
        }

        public SoundManager SoundManager { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="renderControl"></param>
        public SceneManager(GraphicsDeviceManager graphicsDeviceManager, SoundManager mSoundManager, Control renderControl)
        {
            if (null == graphicsDeviceManager) { throw new NullReferenceException("graphicsDeviceManager"); }
            if (null == renderControl) { throw new NullReferenceException("renderControl"); }

            mRenderer = new Renderer();
            mGraphicsDeviceManager = graphicsDeviceManager;
            mRenderTargetManager = new RenderTargetManager(mGraphicsDeviceManager);

            SoundManager = mSoundManager;

            mMouseManager = new MouseManager(renderControl);
            RegisterFrameListener(mMouseManager);
            
            mKeyboardManager = new KeyboardManager();
            RegisterFrameListener(mKeyboardManager);
            
            mParticleDynamicGeometryManager = new ParticleDynamicGeometryManager(graphicsDeviceManager);
            RegisterFrameListener(mParticleDynamicGeometryManager);
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
        public void DrawActiveScene(AleGameTime gameTime)
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
        public bool UpdateActiveScene(AleGameTime gameTime)
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

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    mRenderTargetManager.Dispose();
                    mParticleDynamicGeometryManager.Dispose();
                    mMouseManager.Dispose();

                    if (null != mActiveScene)
                    {
                        mActiveScene.Dispose();
                    }
                    //todo: dispose inactive scenes ?
                }
                mIsDisposed = true;
            }
        }
    }
}
