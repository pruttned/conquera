﻿using System;
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
        private List<IFrameListener> mFrameListeners;

        /// <summary>
        /// Whether was object already disposed
        /// </summary>
        private bool mIsDisposed = false;

        private SceneManager mSceneManager;

        private bool mInitialized = false;

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
        /// temp
        /// </summary>
        public IntPtr AuxWinHwnd
        {
            get { return mGame.Window.Handle; }
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

        public SoundManager SoundManager { get; private set; }

        protected abstract string GuiPaletteName { get; }
        protected abstract Ale.Gui.CursorInfo DefaultCursor { get; }

        #endregion Properties

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="modFile"></param>
        public BaseApplication(string modFile)
            : this(null, modFile)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="renderControl">- Control that should be used as a target for rendering (null - window will be created)</param>
        /// <param name="modFile"></param>
        public BaseApplication(AleRenderControl renderControl, string modFile)
        {
            string dataDirectory = MainSettings.Instance.DataDirectory;
            if (!Path.IsPathRooted(modFile))
            {
                modFile = Path.Combine(dataDirectory, modFile);
            }

            mRenderControl = renderControl;

            mGame = new AleGame(this, mRenderControl, dataDirectory, modFile);
            mGameTime = new AleGameTime();

            mGame.Activated += new EventHandler(mGame_Activated);
            mGame.Deactivated += new EventHandler(mGame_Deactivated);

            SoundManager = new SoundManager(dataDirectory);
            mGame.Services.AddService(typeof(SoundManager), SoundManager);
            RegisterFrameListener(SoundManager);
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
                mFrameListeners = new List<IFrameListener>();
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
            return new SceneManager(graphicsDeviceManager, SoundManager, renderControl);
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
        /// Runed only once at first LoadContent
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
                Ale.Gui.GuiManager.Initialize(GraphicsDeviceManager, Content.Load<Ale.Gui.Palette>(GuiPaletteName), Content.DefaultContentGroup, SceneManager.MouseManager);

                if (mRenderControl == null)
                {
                    Ale.Gui.GuiManager.Instance.Cursor = DefaultCursor;
                    SceneManager.MouseManager.ClipRealCursor = AppSettingsManager.Default.GetSettings<CommonSettings>().ConstraintCursor;
                    mGame.IsMouseVisible = false;
                }

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
            GameTime.UpdateOnFrameStart(gameTime);

            //call frame listeners
            if (null != mFrameListeners)
            {
                for (int i = 0; i < mFrameListeners.Count; ++i)
                {
                    mFrameListeners[i].BeforeUpdate(mGameTime);
                }
            }

            Update(GameTime);

            //call frame listeners
            if (null != mFrameListeners)
            {
                for (int i = 0; i < mFrameListeners.Count; ++i)
                {
                    mFrameListeners[i].AfterUpdate(mGameTime);
                }
            }

        }

        /// <summary>
        /// Called on frame draw
        /// </summary>
        /// <param name="gameTime"></param>
        internal void OnDraw(GameTime gameTime)
        {
            //call frame listeners
            if (null != mFrameListeners)
            {
                for (int i = 0; i < mFrameListeners.Count; ++i)
                {
                    mFrameListeners[i].BeforeRender(mGameTime);
                }
            }

            Draw(GameTime);

            //call frame listeners
            if (null != mFrameListeners)
            {
                for (int i = 0; i < mFrameListeners.Count; ++i)
                {
                    mFrameListeners[i].AfterRender(mGameTime);
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

                    SoundManager.Dispose();
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

            SoundManager.SetPauseAll(false);

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

            SoundManager.SetPauseAll(true);

            if (null != ApplicationFocusChanged)
            {
                ApplicationFocusChanged.Invoke(false);
            }
        }


    }
}
