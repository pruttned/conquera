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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameId"></param>
        /// <param name="camera"></param>
        /// <param name="scene"></param>
        /// <param name="renderTarget">- if null, then the content is rendered to the backbuffer</param>
        /// <param name="backColor"></param>
        public ScenePass(NameId nameId, BaseScene scene, ICamera camera, AleRenderTarget renderTarget)
        {
            if (null == nameId) { throw new NullReferenceException("nameId"); }
            if (null == camera) { throw new NullReferenceException("camera"); }
            if (null == scene) { throw new NullReferenceException("scene"); }

            mScene = scene;
            mNameId = nameId;
            mCamera = camera;
            mRenderTarget = renderTarget;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Draw(GraphicsDevice graphicsDevice, Renderer renderer, AleGameTime gameTime, RenderTargetManager renderTargetManager)
        {
            Begin(graphicsDevice, renderer, renderTargetManager);
            EnqueRenderableUnits(graphicsDevice, gameTime, renderer);
            End(graphicsDevice, gameTime, renderer);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
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
            renderTarget.Dispose();
        }

        protected virtual void Begin(GraphicsDevice graphicsDevice, Renderer renderer, RenderTargetManager renderTargetManager)
        {
            renderer.Begin(mCamera, mScene, mNameId, mRenderTarget);
        }

        protected virtual void End(GraphicsDevice graphicsDevice, AleGameTime gameTime, Renderer renderer)
        {
            renderer.End(gameTime);
        }

        protected virtual void EnqueRenderableUnits(GraphicsDevice graphicsDevice, AleGameTime gameTime, Renderer renderer)
        {
            mScene.EnqueRenderableUnits(gameTime, this);
        }
    }
}
