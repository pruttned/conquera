//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
