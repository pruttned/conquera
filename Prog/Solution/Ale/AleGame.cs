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
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;
using Ale.Settings;
using Ale.Graphics;
using Ale.Tools;
using Ale.Sound;

namespace Ale
{
    /// <summary>
    /// Internal Xna game implementation that is used by BaseApplication
    /// </summary>
    internal class AleGame : Game
    {
        #region Fields

       private static bool EnablePerfHud = false;

        /// <summary>
        /// Base application that owns this game instance
        /// </summary>
        private BaseApplication mBaseApplication;

        /// <summary>
        /// Graphics device manager
        /// </summary>
        private GraphicsDeviceManager mGraphicsDeviceManager;

        /// <summary>
        /// AleRenderControl that holds this game
        /// </summary>
        private AleRenderControl mAleRenderControl = null;

        private AleContentManager mAleContentManager;

        private bool mVideoInitialized = false;

        private bool mIsDisposed = false;


        #endregion Fields

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public AleContentManager AleContentManager
        {
            get { return mAleContentManager; }
        }

        /// <summary>
        /// Gets the GraphicsDeviceManager
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mGraphicsDeviceManager; }
        }

        #endregion Properties

        #region Methods
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="baseApplication"></param>
        /// <param name="renderControl"></param>
        /// <param name="modFile"></param>
        /// <param name="dataDirectory"></param>
        public AleGame(BaseApplication baseApplication, AleRenderControl renderControl, string dataDirectory, string modFile)
            :base()
        {
            Content.Dispose();
            Content = mAleContentManager = new AleContentManager(Services, modFile, dataDirectory);
            mAleRenderControl = renderControl;

            AleContentManager.RegisterAssetTypeDirectory(typeof(Texture), "Textures");
            AleContentManager.RegisterAssetTypeDirectory(typeof(Texture2D), "Textures");
            AleContentManager.RegisterAssetTypeDirectory(typeof(TextureCube), "Textures");
            AleContentManager.RegisterAssetTypeDirectory(typeof(Texture3D), "Textures");
            AleContentManager.RegisterAssetTypeDirectory(typeof(Ale.Graphics.MaterialEffect), "MaterialEffects");
            AleContentManager.RegisterAssetTypeDirectory(typeof(Ale.Gui.Palette), "GuiPalettes");
            AleContentManager.RegisterAssetTypeDirectory(typeof(SpriteFont), "Fonts");
            AleContentManager.RegisterAssetTypeDirectory(typeof(Mesh), "Models");

            mBaseApplication = baseApplication;

            mGraphicsDeviceManager = new GraphicsDeviceManager(this);

            mGraphicsDeviceManager.PreparingDeviceSettings +=new EventHandler<PreparingDeviceSettingsEventArgs>(mGraphicsDeviceManager_PreparingDeviceSettings);

            this.IsMouseVisible = true;

            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(AppSettingsManager_AppSettingsCommitted);
            
        //    IsFixedTimeStep = false;

            VideoSettings videoSettings;
            if (AppSettingsManager.Default.TryGetSettings<VideoSettings>(out videoSettings))
            {
                ReloadVideoSettings(videoSettings, false);
                mVideoInitialized = true;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            //Xna doesn't dispose Content ... great!! - So, for instance, AleSound's finalizer will blow because SoundManager will be already disposed
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    Content.Dispose();
                }
                mIsDisposed = true;
            }
        }

        /// <summary>
        /// Must be called whenever is ale render control resized (Caled from AleRenderControl)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal void OnRenderControlResize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                mGraphicsDeviceManager.PreferredBackBufferWidth = width;
                mGraphicsDeviceManager.PreferredBackBufferHeight = height;

                mGraphicsDeviceManager.ApplyChanges();
            }
        }

        /// <summary>
        /// Runs the game in an AleRenderControl
        /// </summary>
        /// <param name="renderControl">- Control in which should be game runned</param>
        public void RunInControl()
        {
            this.IsFixedTimeStep = false;

            //graphicsDeviceManager
            Type gameType = typeof(Game);
            FieldInfo graphicsDeviceManagerFieldInfo = gameType.GetField("graphicsDeviceManager", BindingFlags.NonPublic | BindingFlags.Instance);
            if (null == graphicsDeviceManagerFieldInfo)
            {
                throw new InvalidOperationException("Couldn't find graphicsDeviceManager field in the Game class");
            }
            IGraphicsDeviceManager graphicsDeviceManager = this.Services.GetService(typeof(IGraphicsDeviceManager)) as IGraphicsDeviceManager;
            if (graphicsDeviceManager != null)
            {
                graphicsDeviceManager.CreateDevice();
            }
            graphicsDeviceManagerFieldInfo.SetValue(this, graphicsDeviceManager);

            Initialize();

            Update(new GameTime());

            mAleRenderControl.Start(this);
        }

        /// <summary>
        /// On draw
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                if (GraphicsDeviceManager.GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
                {
                    mBaseApplication.OnDraw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// On update
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                //handle xna bug  ("The operation was aborted. You may not modify a resource that has been set on a device, or after it has been used within a tiling bracket." On SetData)
                GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
                GraphicsDevice.Indices = null;

                mBaseApplication.OnUpdate(gameTime);
            }

            base.Update(gameTime);
        }

        
        /// <summary>
        /// On content load
        /// </summary>
        protected override void LoadContent()
        {
            Tracer.WriteInfo("AleGame.LoadContent");

            if (!mVideoInitialized)
            {
                VideoSettings videoSettings;
                if (!AppSettingsManager.Default.TryGetSettings<VideoSettings>(out videoSettings))
                {
                    videoSettings = new VideoSettings();
                    AppSettingsManager.Default.CommitSettings(videoSettings);
                }
                ReloadVideoSettings(videoSettings, false);
                mVideoInitialized = true;
            }
            mBaseApplication.OnLoadContent();
        }

        /// <summary>
        /// On content unload
        /// </summary>
        protected override void UnloadContent()
        {
            Tracer.WriteInfo("AleGame.UnloadContent");
            mBaseApplication.OnUnloadContent();
        }

        private void AppSettingsManager_AppSettingsCommitted(IAppSettings settings)
        {
            if (settings is VideoSettings)
            {
                ReloadVideoSettings((VideoSettings)settings, true);
            }
        }

        /// <summary>
        /// Registers the AleRenderControl as a render target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mGraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            if (null != mAleRenderControl)
            {
                e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = mAleRenderControl.Handle;
            }

            if (EnablePerfHud)
            {
                foreach (GraphicsAdapter curAdapter in GraphicsAdapter.Adapters)
                {
                    if (curAdapter.Description.Contains("PerfHUD"))
                    {
                        e.GraphicsDeviceInformation.Adapter = curAdapter;
                        e.GraphicsDeviceInformation.DeviceType = DeviceType.Reference;
                        break;
                    }
                }
            }
        }

        private void ReloadVideoSettings(VideoSettings videoSettings, bool applyChanges)
        {
            Tracer.WriteInfo("AleGame.ReloadSettings");

            mGraphicsDeviceManager.PreferredBackBufferWidth = videoSettings.ScreenWidth;
            mGraphicsDeviceManager.PreferredBackBufferHeight = videoSettings.ScreenHeight;
            mGraphicsDeviceManager.IsFullScreen = (null == mAleRenderControl) && videoSettings.Fullscreen;
            //mGraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;

            if (applyChanges)
            {
                mGraphicsDeviceManager.ApplyChanges();
            }
        }

        #endregion Methods
    }
}
