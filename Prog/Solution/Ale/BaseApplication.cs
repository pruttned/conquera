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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Ale.Input;
using System.Windows.Forms;
using Ale.Content;
using System.IO;
using Ale.Scene;
using Ale.Graphics;
using Ale.Settings;
using Ale.Sound;
using Ale.Tools;

namespace Ale
{
    /// <summary>
    /// Facade for applications based on a Ale
    /// </summary>
    public abstract class BaseApplication : IDisposable
    {
        #region Delegates

        /// <summary>
        /// Handler for ApplicationFocusChanged event
        /// </summary>
        /// <param name="activated">Whether the application currently has focus</param>
        public delegate void ApplicationFocusChangedHandler(bool hasFocus);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Raised whenever is application activated or deactivated
        /// </summary>
        public event ApplicationFocusChangedHandler ApplicationFocusChanged;

        #endregion Events

        #region Fields

        /// <summary>
        /// Xna game
        /// </summary>
        private AleGame mGame;

        /// <summary>
        /// Game time
        /// </summary>
        private AleGameTime mGameTime;

        /// <summary>
        /// Render control (if it is used)
        /// </summary>
        private AleRenderControl mRenderControl;

        /// <summary>
        /// Frame listeners
        /// </summary>
        private SafeModifiableIterableCollection<IFrameListener> mFrameListeners;

        /// <summary>
        /// Whether was object already disposed
        /// </summary>
        private bool mIsDisposed = false;

        private SceneManager mSceneManager;

        private bool mInitialized = false;

        
        private Control mRenderWindow;
        private bool mRenderWindowHasFocus = true;

        #endregion Fields

        #region Properties


        /// <summary>
        /// Gets the acutall game time
        /// </summary>
        public AleGameTime GameTime
        {
            get { return mGameTime; }
        }

        /// <summary>
        /// Gets the content manager
        /// </summary>
        public AleContentManager Content
        {
            get { return mGame.AleContentManager; }
        }

        /// <summary>
        /// Indicates whether is this application currently active
        /// </summary>
        public bool IsActive
        {
            get { return mGame.IsActive; }
        }

        /// <summary>
        /// Render window handle
        /// </summary>
        public IntPtr RenderWindowHwnd
        {
            get { return mGame.Window.Handle; }
        }
        public Control RenderWindow
        {
            get { return mRenderWindow; }
        }


        public GraphicsDevice GraphicsDevice
        {
            get { return mGame.GraphicsDevice; }
        }

        /// <summary>
        /// Gets the GraphicsDeviceManager
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mGame.GraphicsDeviceManager; }
        }

        public SceneManager SceneManager
        {
            get { return mSceneManager; }
        }

        protected abstract string GuiPaletteName { get; }        
        protected bool ShowSysCursor
        {
            get
            {
                return mGame.IsMouseVisible;
            }
            set
            {
                mGame.IsMouseVisible = value;
            }
        }

        #endregion Properties

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseApplication()
            : this(null)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="renderControl">- Control that should be used as a target for rendering (null - window will be created)</param>
        /// <param name="modFile"></param>
        public BaseApplication(AleRenderControl renderControl)
        {
            mRenderWindowHasFocus = true;

            string dataDirectory = MainSettings.Instance.DataDirectory;
            string modFile = MainSettings.Instance.Mod;
            if (!Path.IsPathRooted(modFile))
            {
                modFile = Path.Combine(dataDirectory, modFile);
            }

            mRenderControl = renderControl;

            mGame = new AleGame(this, mRenderControl, dataDirectory, modFile);
            mGameTime = new AleGameTime();

            mGame.Activated += new EventHandler(mGame_Activated);
            mGame.Deactivated += new EventHandler(mGame_Deactivated);
        }

        /// <summary>
        /// Runs the application
        /// </summary>
        public void Run()
        {
            if (null == mRenderControl)
            {
                mGame.Run();
            }
            else
            {
                mGame.RunInControl();
            }
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        public void Exit()
        {
            mGame.Exit();
        }

        /// <summary>
        /// Adds a new frame listener
        /// </summary>
        /// <param name="frameListener"></param>
        public void RegisterFrameListener(IFrameListener frameListener)
        {
            if (null == frameListener) { throw new ArgumentNullException("frameListener"); }

            if (null == mFrameListeners)
            {
                mFrameListeners = new SafeModifiableIterableCollection<IFrameListener>();
            }
            mFrameListeners.Add(frameListener);
        }

        /// <summary>
        /// Adds a new frame listener
        /// </summary>
        /// <param name="frameListener"></param>
        /// <returns>Whether was listener removed</returns>
        public bool RemoveFrameListener(IFrameListener frameListener)
        {
            if (null != mFrameListeners)
            {
                return mFrameListeners.Remove(frameListener);
            }
            return false;
        }

        protected virtual SceneManager CreateSceneManager(GraphicsDeviceManager graphicsDeviceManager, Control renderControl)
        {
            return new SceneManager(graphicsDeviceManager, renderControl, mGame.AleServiceProvider, Content);
        }
        
        protected abstract BaseScene CreateDefaultScene(SceneManager sceneManager);

        /// <summary>
        /// Draws the frame
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void Draw(AleGameTime gameTime)
        {
            SceneManager.DrawActiveScene(gameTime);
        }

        /// <summary>
        /// Updates the game logic
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void Update(AleGameTime gameTime)
        {
            if (!SceneManager.UpdateActiveScene(gameTime))
            {
                Exit();
            }
        }

        /// <summary>
        /// Runs only once at a first LoadContent
        /// </summary>
        protected virtual void OnInit()
        {
        }

        /// <summary>
        /// Loads content
        /// </summary>
        protected virtual void LoadContent()
        {
        }

        /// <summary>
        /// Unloads content (only non ContentManager content)
        /// </summary>
        protected virtual void UnloadContent()
        {
        }

        /// <summary>
        /// Called on content loading
        /// </summary>
        internal void OnLoadContent()
        {
            if (!mInitialized)
            {
                BoundingSphereRenderable.Init(GraphicsDevice, Content.DefaultContentGroup);
                BoundingBoxRenderable.Init(GraphicsDevice, Content.DefaultContentGroup);

                mSceneManager = CreateSceneManager(GraphicsDeviceManager, null != mRenderControl ? mRenderControl : Control.FromHandle(mGame.Window.Handle));
                
                if (mRenderControl == null)
                {
                    SceneManager.MouseManager.ClipRealCursor = AppSettingsManager.Default.GetSettings<CommonSettings>().ConstraintCursor;
                    mGame.IsMouseVisible = true;
                }

                mRenderWindow = Control.FromHandle(RenderWindowHwnd);
                mRenderWindow.GotFocus += new EventHandler(mRenderWindow_GotFocus);
                mRenderWindow.LostFocus += new EventHandler(mRenderWindow_LostFocus);

                RegisterFrameListener(mSceneManager);
                SceneManager.ActivateScene(CreateDefaultScene(SceneManager));                

                mInitialized = true;

                OnInit();
            }
            
            LoadContent();
        }

        /// <summary>
        /// Called on content unloading
        /// </summary>
        internal void OnUnloadContent()
        {
            UnloadContent();
        }

        /// <summary>
        /// Called on 
        /// </summary>
        /// <param name="gameTime"></param>
        internal void OnUpdate(GameTime gameTime)
        {
            if (mRenderWindowHasFocus)
            {
                GameTime.UpdateOnFrameStart(gameTime);

                //call frame listeners
                if (null != mFrameListeners)
                {
                    foreach (var listener in mFrameListeners)
                    {
                        listener.BeforeUpdate(mGameTime);
                    }
                }

                Update(GameTime);

                //call frame listeners
                if (null != mFrameListeners)
                {
                    foreach (var listener in mFrameListeners)
                    {
                        listener.AfterUpdate(mGameTime);
                    }

                    //clear removed listeners
                    mFrameListeners.Tidy();
                }
            }
        }

        /// <summary>
        /// Called on frame draw
        /// </summary>
        /// <param name="gameTime"></param>
        internal void OnDraw(GameTime gameTime)
        {
            if (mRenderWindowHasFocus)
            {
                //call frame listeners
                if (null != mFrameListeners)
                {
                    foreach (var listener in mFrameListeners)
                    {
                        listener.BeforeRender(mGameTime);
                    }
                }

                Draw(GameTime);

                //call frame listeners
                if (null != mFrameListeners)
                {
                    foreach (var listener in mFrameListeners)
                    {
                        listener.AfterRender(mGameTime);
                    }

                    //clear removed listeners
                    mFrameListeners.Tidy();
                }
            }
        }

        #region IDisposable

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
                    mGame.Dispose();

                    mSceneManager.Dispose();
                }
                mIsDisposed = true;
            }
        }

        #endregion IDisposable

        /// <summary>
        /// On activation of the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mGame_Activated(object sender, EventArgs e)
        {
            if (null != mRenderControl)
            {
                mRenderControl.Resume();
            }

            mSceneManager.OnAppActivate();

            if (null != ApplicationFocusChanged)
            {
                ApplicationFocusChanged.Invoke(true);
            }
        }

        /// <summary>
        /// On deactivation of the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mGame_Deactivated(object sender, EventArgs e)
        {
            if (null != mRenderControl)
            {
                mRenderControl.Pause();
            }

            mSceneManager.OnAppDeactivate();

            if (null != ApplicationFocusChanged)
            {
                ApplicationFocusChanged.Invoke(false);
            }
        }

        void mRenderWindow_LostFocus(object sender, EventArgs e)
        {
            mRenderWindowHasFocus = false;
        }

        void mRenderWindow_GotFocus(object sender, EventArgs e)
        {
            mRenderWindowHasFocus = true;
        }
    }
}
