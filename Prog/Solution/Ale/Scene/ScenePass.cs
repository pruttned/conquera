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
        //private bool mC
        
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

            mNameId = nameId;
            mCamera = camera;
            mRenderTarget = renderTarget;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="camera"></param>
        /// <param name="scene"></param>
        /// <param name="renderTarget">- if null, then the content is rendered to the backbuffer</param>
        /// <param name="backColor"></param>
        public ScenePass(string name, BaseScene scene,  ICamera camera, AleRenderTarget renderTarget)
        {
            if (string.IsNullOrEmpty(name)) { throw new NullReferenceException("name"); }
            if (null == camera) { throw new NullReferenceException("camera"); }
            if (null == scene) { throw new NullReferenceException("scene"); }
            
            mNameId = name;
            mScene = scene;
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

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    if (null != mRenderTarget)
                    {
                        mRenderTarget.Dispose();
                    }
                }
                mIsDisposed = true;
            }
        }

        public virtual void Draw(GraphicsDevice graphicsDevice, Renderer renderer, AleGameTime gameTime, RenderTargetManager renderTargetManager)
        {
            Begin(graphicsDevice, renderer, renderTargetManager);
            EnqueRenderableUnits(graphicsDevice, gameTime, renderer);
            End(graphicsDevice, gameTime, renderer);
        }

        protected virtual void Begin(GraphicsDevice graphicsDevice, Renderer renderer, RenderTargetManager renderTargetManager)
        {
            if (null != mRenderTarget)
            {
                mRenderTarget.Begin();
            }
            else
            {
                graphicsDevice.SetRenderTarget(0, null);
            }
            Clear(graphicsDevice);
            renderer.Begin(mCamera, renderTargetManager, mScene, mNameId);
        }

        protected virtual void End(GraphicsDevice graphicsDevice, AleGameTime gameTime, Renderer renderer)
        {
            renderer.End(gameTime);
            if (null != mRenderTarget)
            {
                mRenderTarget.End();
            }
        }

        protected virtual void Clear(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);
        }

        protected virtual void EnqueRenderableUnits(GraphicsDevice graphicsDevice, AleGameTime gameTime, Renderer renderer)
        {
            mScene.EnqueRenderableUnits(gameTime, this);
        }

    }
}
