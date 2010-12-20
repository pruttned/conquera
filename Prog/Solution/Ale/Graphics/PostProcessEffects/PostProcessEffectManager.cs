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
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// Manager of the post-prcess effects
    /// </summary>
    public sealed class PostProcessEffectManager : IDisposable
    {
        #region Fields
        
        /// <summary>
        /// Post-process effect
        /// </summary>
        private PostProcessEffectCollection mPostProcessEffects = new PostProcessEffectCollection();
        
        /// <summary>
        /// 
        /// </summary>
        private GraphicsDeviceManager mGraphicsDeviceManager;

        /// <summary>
        /// 
        /// </summary>
        private ResolveTexture2D mScreenResolveTexture = null;

        /// <summary>
        /// 
        /// </summary>
        private RenderTarget2D mScreenRenderTarget = null;

        /// <summary>
        /// 
        /// </summary>
        private bool mIsDisposed = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the graphics device
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return mGraphicsDeviceManager.GraphicsDevice; }
        }

        /// <summary>
        /// Gets the collection of registered post-process effects. Effect are applied in order as they are present in
        /// the collection (from 0 to last).
        /// </summary>
        public PostProcessEffectCollection PostProcessEffects
        {
            get { return mPostProcessEffects; }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool ContentIsLoaded
        {
            get { return (null != mScreenResolveTexture); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public PostProcessEffectManager(GraphicsDeviceManager graphicsDeviceManager)
        {
            mGraphicsDeviceManager = graphicsDeviceManager;

            mGraphicsDeviceManager.DeviceReset += new EventHandler(mGraphicsDeviceManager_DeviceReset);
        }

        /// <summary>
        /// Applies all enabled registered post-process effects
        /// </summary>
        /// <param name="gameTime"></param>
        public void Apply(AleGameTime gameTime)
        {
            if (0 < PostProcessEffects.EnabledEffectCnt && GraphicsDeviceStatus.Normal == GraphicsDevice.GraphicsDeviceStatus)
            {
                if (!ContentIsLoaded)
                {
                    LoadContent();
                }

                GraphicsDevice.ResolveBackBuffer(mScreenResolveTexture);
                Texture2D screen = mScreenResolveTexture;

                int enabledEffectsToApply = PostProcessEffects.EnabledEffectCnt;
                for (int i = 0; i < mPostProcessEffects.Count && 0 < enabledEffectsToApply; ++i)
                {
                    if (mPostProcessEffects[i].Enabled)
                    {
                        if (1 == enabledEffectsToApply) //last effect
                        {
                            mPostProcessEffects[i].Apply(screen, null, gameTime);
                        }
                        else
                        {
                            mPostProcessEffects[i].Apply(screen, mScreenRenderTarget, gameTime);
                            GraphicsDevice.SetRenderTarget(0, null);
                            screen = mScreenRenderTarget.GetTexture();
                        }
                        enabledEffectsToApply--;
                    }
                }

                MaterialEffect.Finish();
            }
        }

        #region IDisposable

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                mGraphicsDeviceManager.DeviceReset -= mGraphicsDeviceManager_DeviceReset;

                UnloadContent();
                
                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        private void UnloadContent()
        {
            if (ContentIsLoaded)
            {
                mScreenResolveTexture.Dispose();
                mScreenRenderTarget.Dispose();

                mScreenResolveTexture = null;
                mScreenRenderTarget = null;
            }
        }

        #endregion IDisposable

        void mGraphicsDeviceManager_DeviceReset(object sender, EventArgs e)
        {
            UnloadContent();

            //Resolution may have changed
            foreach (var effect in mPostProcessEffects)
            {
                effect.UnloadContent();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadContent()
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            mScreenResolveTexture = new ResolveTexture2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 1, pp.BackBufferFormat);
            mScreenRenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 1, pp.BackBufferFormat);
        }

        #endregion Methods
    }
}
