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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Base class for post-process effect
    /// </summary>
    public abstract class PostProcessEffect : IDisposable
    {
        #region Delegates

        /// <summary>
        /// EnableChanged handler
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="effect"></param>
        public delegate void EnableChangedHandler(bool enabled, PostProcessEffect effect);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Raised whenever is post-process efect enabled or disabled
        /// </summary>
        public event EnableChangedHandler EnableChanged;

        #endregion Events

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private GraphicsDeviceManager mGraphicsDeviceManager;

        /// <summary>
        /// 
        /// </summary>
        private bool mIsDisposed = false;

        /// <summary>
        /// Whether is effect currently enabled
        /// </summary>
        private bool mEnabled = false;

        private bool mIsLoaded = false;

        private FullScreenQuad mFullScreenQuad;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the graphic device manager
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mGraphicsDeviceManager; }
        }

        /// <summary>
        /// Gets the graphic device
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return mGraphicsDeviceManager.GraphicsDevice; }
        }

        /// <summary>
        /// Gets/sets whether is effect currently enabled
        /// </summary>
        public bool Enabled
        {
            get { return mEnabled; }
            set
            {
                if (value != mEnabled)
                {
                    mEnabled = value;
                    OnEnableChanged(value);
                    if (null != EnableChanged)
                    {
                        EnableChanged.Invoke(value, this);
                    }
                }
            }
        }

        #endregion Properties
        
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public PostProcessEffect(GraphicsDeviceManager graphicsDeviceManager)
        {
            mGraphicsDeviceManager = graphicsDeviceManager;
            mFullScreenQuad = new FullScreenQuad(graphicsDeviceManager);
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
                    mFullScreenQuad.Dispose();
                    UnloadContent();
                }
                mIsDisposed = true;
            }
        }

        #endregion IDisposable

        /// <summary>
        /// Aplies the effect
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="resultRenderTarget"></param>
        /// <param name="gameTime"></param>
        internal void Apply(Texture2D screen, RenderTarget2D resultRenderTarget, AleGameTime gameTime)
        {
            if (Enabled)
            {
                if (!mIsLoaded)
                {
                    LoadContent();
                }

                ApplyImpl(screen, resultRenderTarget, gameTime);
            }
        }


        /// <summary>
        /// Loads all non ContentManager content
        /// </summary>
        internal void LoadContent()
        {
            LoadContentImpl();
            mIsLoaded = true;
        }

        /// <summary>
        /// Unloads all non ContentManager content
        /// </summary>
        internal void UnloadContent()
        {
            UnloadContentImpl();
            mIsLoaded = false;
        }

        /// <summary>
        /// Applies the effect
        /// </summary>
        /// <param name="screen">- Texture that contains rendered frame</param>
        /// <param name="resultRenderTarget">Render target to which should be result rendered</param>
        /// <param name="gameTime"></param>
        protected abstract void ApplyImpl(Texture2D screen, RenderTarget2D resultRenderTarget, AleGameTime gameTime);

        /// <summary>
        /// Draws fullscreen quad. (First pass of the default technique is used)
        /// Using Spritebatch
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="gameTime"></param>
        /// <param name="rtWidth"></param>
        /// <param name="rtHeight"></param>
        protected void DrawFullscreenQuad(MaterialEffect effect, AleGameTime gameTime, int rtWidth, int rtHeight)
        {
            mFullScreenQuad.Draw(effect, gameTime, rtWidth, rtHeight);
        }

        /// <summary>
        /// Draws fullscreen quad. (First pass of the default technique is used)
        /// Using Spritebatch
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="gameTime"></param>
        protected void DrawFullscreenQuad(MaterialEffect effect, AleGameTime gameTime)
        {
            mFullScreenQuad.Draw(effect, gameTime);
        }

        /// <summary>
        /// Loads all non ContentManager content
        /// </summary>
        protected abstract void LoadContentImpl();

        /// <summary>
        /// Unloads all non ContentManager content
        /// </summary>
        protected abstract void UnloadContentImpl();

        /// <summary>
        /// Executes whenever is post-process efect enabled or disabled
        /// </summary>
        /// <param name="enabled"></param>
        protected virtual void OnEnableChanged(bool enabled)
        {
        }

        #endregion Methods
    }
}
