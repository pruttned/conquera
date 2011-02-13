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

        private static SpriteBatch mSpriteBatch;

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

        protected Viewport Viewport { get; private set; }

        #endregion Properties
        
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public PostProcessEffect(GraphicsDeviceManager graphicsDeviceManager)
        {
            mGraphicsDeviceManager = graphicsDeviceManager;

            if (null == mSpriteBatch)
            {
                mSpriteBatch = new SpriteBatch(GraphicsDevice);
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
            Viewport = GraphicsDevice.Viewport;
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
        /// <param name="texture">- First texture of the effect - it is set by spritebatch</param>
        /// <param name="rtWidth"></param>
        /// <param name="rtHeight"></param>
        protected void DrawFullscreenQuad(MaterialEffect effect, AleGameTime gameTime, Texture2D texture, int rtWidth, int rtHeight)
        {
            mSpriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);


            effect.Apply(gameTime, effect.DefaultTechnique.Passes[0]);

            mSpriteBatch.Draw(texture, new Rectangle(0, 0, rtWidth, rtHeight), Color.White);
            mSpriteBatch.End();

            MaterialEffect.Finish();
        }

        /// <summary>
        /// Draws fullscreen quad. (First pass of the default technique is used)
        /// Using Spritebatch
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="gameTime"></param>
        /// <param name="texture">- First texture of the effect - it is set by spritebatch</param>
        protected void DrawFullscreenQuad(MaterialEffect effect, AleGameTime gameTime, Texture2D texture)
        {
            DrawFullscreenQuad(effect, gameTime, texture, Viewport.Width, Viewport.Height);
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
