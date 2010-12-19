using System;
using System.Collections.Generic;
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework;
using Ale.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Scene
{
    public class SkyPlaneScenePass : ScenePass
    {
        public static readonly string Name = "SkyPlaneScenePass";
        private SkyPlane mSkyPlane;

        public SkyPlaneScenePass(Camera mainCamera, BaseScene scene, ContentGroup content, Material skyPlaneMaterial)
            : base(Name, scene, CreateCamera(mainCamera), null)
        {
            mSkyPlane = new SkyPlane(scene.SceneManager.GraphicsDeviceManager.GraphicsDevice, skyPlaneMaterial, -100);
        }

        private static ICamera CreateCamera(Camera mainCamera)
        {
            return new SkyPlaneCamera(mainCamera);
        }

        protected override void EnqueRenderableUnits(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, AleGameTime gameTime, Renderer renderer)
        {
            mSkyPlane.EnqueRenderableUnits(gameTime, renderer, this);
        }

        protected override void Clear(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.AliceBlue, 1.0f, 0);
        }

        #region IDisposable Members

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                ((SkyPlaneCamera)Camera).Dispose();
                mSkyPlane.Dispose();
            }

            base.Dispose(isDisposing);
        }

        #endregion
    }

    class SkyPlaneCamera : Camera, IDisposable
    {
        private Camera mMainCamera;
        private bool mIsDisposed = false;

        public SkyPlaneCamera(Camera mainCamera)
            : base(mainCamera.TargetWorldPosition, mainCamera.DistanceToTarget, mainCamera.RotationArroundTarget, mainCamera.MaxDistanceToTarget,
            mainCamera.MinDistanceToTarget, mainCamera.MaxRotX, mainCamera.MinRotX)
        {
            if (null == mainCamera) throw new ArgumentNullException("mainCamera");
            FarPlaneDistance = 200000;
            mMainCamera = mainCamera;

            mMainCamera.ViewProjectionTransformationChanged += new CameraTransformationChangedHandler(mMainCamera_ViewProjectionTransformationChanged);

            Update();
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
                    mMainCamera.ViewProjectionTransformationChanged -= mMainCamera_ViewProjectionTransformationChanged;
                }
                mIsDisposed = true;
            }
        }

        private void mMainCamera_ViewProjectionTransformationChanged(ICamera camera)
        {
            Update();
        }

        private void Update()
        {
            if (null != mMainCamera)
            {
                TargetWorldPosition = mMainCamera.TargetWorldPosition;
                RotationArroundTarget = mMainCamera.RotationArroundTarget;
                DistanceToTarget = mMainCamera.DistanceToTarget;
            }
        }
    }
}
