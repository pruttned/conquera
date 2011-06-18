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

namespace Ale.Scene
{
    public class ScenePass : IDisposable
    {
        private NameId mNameId;
        private ICamera mCamera;
        private AleRenderTarget mRenderTarget;
        private bool mIsDisposed = false;
        private BaseScene mScene;
        private bool mEnabled = true;
        private bool mIsDeferred;
        private bool mNeedReload = false;

        public NameId NameId
        {
            get { return mNameId; }
        }

        public ICamera Camera
        {
            get { return mCamera; }
        }

        public BaseScene Scene
        {
            get { return mScene; }
        }
        
        public AleRenderTarget RenderTarget 
        {
			get { return mRenderTarget; }
		}

		public bool IsEnabled 
		{
			get { return mEnabled; }
			set { mEnabled = value; }
		}

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return Scene.SceneManager.GraphicsDeviceManager; }
        }
        public IRenderTargetManager RenderTargetManager 
        {
            get { return Scene.RenderTargetManager; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameId"></param>
        /// <param name="camera"></param>
        /// <param name="scene"></param>
        /// <param name="renderTarget">- if null, then the content is rendered to the backbuffer</param>
        /// <param name="backColor"></param>
        public ScenePass(NameId nameId, BaseScene scene, ICamera camera, bool isDeferred)
        {
            if (null == nameId) { throw new NullReferenceException("nameId"); }
            if (null == camera) { throw new NullReferenceException("camera"); }
            if (null == scene) { throw new NullReferenceException("scene"); }

            mIsDeferred = isDeferred;
            mScene = scene;
            mNameId = nameId;
            mCamera = camera;
            mRenderTarget = CreateRenderTarget(RenderTargetManager);

            if (isDeferred && null != mRenderTarget)
            {
                throw new ArgumentException("Deferred doesn't supports render target");
            }


            GraphicsDeviceManager.DeviceReset += new EventHandler(mGraphicsDeviceManager_DeviceReset);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Draw(GraphicsDevice graphicsDevice, IRenderer renderer, AleGameTime gameTime)
        {
            Begin(graphicsDevice, renderer);
            EnqueRenderableUnits(graphicsDevice, gameTime, renderer);
            End(graphicsDevice, gameTime, renderer);
        }

        /// <summary>
        /// Creates render target or null
        /// </summary>
        /// <param name="renderTargetManager"></param>
        /// <returns></returns>
        protected virtual AleRenderTarget CreateRenderTarget(IRenderTargetManager renderTargetManager)
        {
            return null;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    GraphicsDeviceManager.DeviceReset -= mGraphicsDeviceManager_DeviceReset;
                    if (null != mRenderTarget)
                    {
                        DestroyRenderTarget(mRenderTarget);
                    }
                }
                mIsDisposed = true;
            }
        }

        protected virtual void DestroyRenderTarget(AleRenderTarget renderTarget)
        {
            RenderTargetManager.DestroyRenderTarget(renderTarget.Name);
            mRenderTarget = null;
        }

        protected virtual void Begin(GraphicsDevice graphicsDevice, IRenderer renderer)
        {
            if (mNeedReload)
            {
                mRenderTarget = CreateRenderTarget(RenderTargetManager);
                mNeedReload = false;
            }

            if (mIsDeferred)
            {
                renderer.BeginDeferred(mCamera, mScene, mNameId);
            }
            else
            {
                renderer.BeginForward(mCamera, mScene, mNameId, mRenderTarget);
            }
        }

        protected virtual void End(GraphicsDevice graphicsDevice, AleGameTime gameTime, IRenderer renderer)
        {
            renderer.End(gameTime);
        }

        protected virtual void EnqueRenderableUnits(GraphicsDevice graphicsDevice, AleGameTime gameTime, IRenderer renderer)
        {
            mScene.EnqueRenderableUnits(gameTime, this);
        }

        void mGraphicsDeviceManager_DeviceReset(object sender, EventArgs e)
        {
            if (null != RenderTarget)
            {
                DestroyRenderTarget(RenderTarget);
                mNeedReload = true;
            }
        }
    }
}
