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
using System.Linq;
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Settings;
using Ale.Tools;
using Ale;

namespace Conquera
{
    public sealed class GameCamera : ICamera, IDisposable, IFrameListener
    {
        public event CameraTransformationChangedHandler ViewTransformationChanged;
        public event CameraTransformationChangedHandler ProjectionTransformationChanged;
        public event CameraTransformationChangedHandler ViewProjectionTransformationChanged;

        /// <summary>
        /// x = distance form target
        /// y = x rot
        /// z = y rot
        /// </summary>
        private static Vector3[] ZoomLevels = new Vector3[] { new Vector3(15, -1.5f, 0.0f), new Vector3(10, -1.1f, 0.0f), new Vector3(5, -0.9f, 0.0f) };

        private Camera mCamera;
        private GameScene mGameScene;
        private bool mIsDisposed = false;


        private int mZoomLevel = 1;

        private Vector3LinearAnimator mPositionAnimator = new Vector3LinearAnimator();
        private Vector3LinearAnimator mZoomLevelAnimator = new Vector3LinearAnimator();

        private float mShakeMagnitude = -1f;
        private Vector2 mBaseRotationArroundTarget;

        public Matrix ViewTransformation
        {
            get { return mCamera.ViewTransformation; }
        }

        public Matrix ProjectionTransformation
        {
            get { return mCamera.ProjectionTransformation; }
        }

        public Matrix ViewProjectionTransformation
        {
            get { return mCamera.ViewProjectionTransformation; }
        }

        public Matrix ViewProjectionTransformationInv
        {
            get { return mCamera.ViewProjectionTransformationInv; }
        }

        public Vector3 CameraUp
        {
            get
            {
                return mCamera.CameraUp;
            }
            set
            {
                mCamera.CameraUp = value;
            }
        }

        public Vector3 WorldPosition
        {
            get { return mCamera.WorldPosition; }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<Vector3> FrustumCorners
        {
            get { return mCamera.FrustumCorners; }
        }

        public Plane FrustumBottom
        {
            get { return mCamera.FrustumBottom; }
        }

        public Plane FrustumFar
        {
            get { return mCamera.FrustumFar; }
        }

        public Plane FrustumLeft
        {
            get { return mCamera.FrustumLeft; }
        }

        public Plane FrustumNear
        {
            get { return mCamera.FrustumNear; }
        }

        public Plane FrustumRight
        {
            get { return mCamera.FrustumRight; }
        }

        public Plane FrustumTop
        {
            get { return mCamera.FrustumTop; }
        }

        public bool IsAnimating 
        {
            get { return mZoomLevelAnimator.IsAnimating || mPositionAnimator.IsAnimating; }
        }

        internal Camera RealCamera
        {
            get { return mCamera; }
        }
        
        public Vector3 TargetWorldPosition
        {
            get { return mCamera.TargetWorldPosition; }
            set
            {
                mCamera.TargetWorldPosition = Vector3.Clamp(value, mGameScene.Terrain.LowerLeftTileCenter, mGameScene.Terrain.UpperRightTileCenter);
            }
        }

        public GameCamera(GameScene gameScene)
        {
            if (null == gameScene) throw new ArgumentNullException("gameScene");

            mGameScene = gameScene;
            mCamera = new Camera(Vector3.Zero, 100, new Vector2(-1.1f, 0), 20000, 3, 1.55f, -1.57f);
            mCamera.ProjectionTransformationChanged += new CameraTransformationChangedHandler(mCamera_ProjectionTransformationChanged);
            mCamera.ViewTransformationChanged += new CameraTransformationChangedHandler(mCamera_ViewTransformationChanged);
            mCamera.ViewProjectionTransformationChanged += new CameraTransformationChangedHandler(mCamera_ViewProjectionTransformationChanged);

            UpdateDistanceToTarget(ZoomLevels[mZoomLevel]);

            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(Default_AppSettingsCommitted);

            UpdateAspectRatio();
        }

        public void Shake()
        {
            mShakeMagnitude = 0.03f;
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                AppSettingsManager.Default.AppSettingsCommitted -= Default_AppSettingsCommitted;

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        public void CenterCameraOnCell(HexCell cell)
        {
            if (null == cell) throw new ArgumentNullException("cell");

            mCamera.TargetWorldPosition = cell.CenterPos;
        }

        public void MoveCameraToCell(HexCell cell)
        {
            if (null == cell) throw new ArgumentNullException("cell");

            SetCameraAnimationGameSceneState();

            mPositionAnimator.Animate(20, mCamera.TargetWorldPosition, cell.CenterPos);
        }

        public void MoveCameraTo(Vector3 target)
        {
            SetCameraAnimationGameSceneState();
            mPositionAnimator.Animate(20, mCamera.TargetWorldPosition, target);
        }

        public void IncZoomLevel()
        {
            IncZoomLevel(true);
        }

        public void DecZoomLevel()
        {
            DecZoomLevel(true);
        }

        public void IncZoomLevel(bool anim)
        {
            if (mZoomLevel < ZoomLevels.Length - 1)
            {
                if (anim)
                {
                    SetCameraAnimationGameSceneState();

                    mZoomLevel++;

                    Vector3 curZommLevel = new Vector3(mCamera.DistanceToTarget, mBaseRotationArroundTarget.X, mBaseRotationArroundTarget.Y);
                    mZoomLevelAnimator.Animate(15, curZommLevel, ZoomLevels[mZoomLevel]);
                }
                else
                {
                    mZoomLevel++;
                    UpdateDistanceToTarget(ZoomLevels[mZoomLevel]);
                }
            }
        }

        public void DecZoomLevel(bool anim)
        {
            if (mZoomLevel > 0)
            {
                if (anim)
                {
                    SetCameraAnimationGameSceneState();

                    mZoomLevel--;

                    Vector3 curZommLevel = new Vector3(mCamera.DistanceToTarget, mBaseRotationArroundTarget.X, mBaseRotationArroundTarget.Y);
                    mZoomLevelAnimator.Animate(15, curZommLevel, ZoomLevels[mZoomLevel]);
                }
                else
                {
                    mZoomLevel--;
                    UpdateDistanceToTarget(ZoomLevels[mZoomLevel]);
                }
            }
        }

        public void CameraToViewport(Vector2 point, Viewport viewport, out Ray ray)
        {
            mCamera.CameraToViewport(point, viewport, out ray);
        }

        public bool IsInSight(ref BoundingBox boundingBox)
        {
            return mCamera.IsInSight(ref boundingBox);
        }

        public bool IsInSight(ref BoundingSphere boundingSphere)
        {
            return mCamera.IsInSight(ref boundingSphere);
        }

        public bool IsInSight(BoundingBox boundingBox)
        {
            return mCamera.IsInSight(boundingBox);
        }

        public bool IsInSight(BoundingSphere boundingSphere)
        {
            return mCamera.IsInSight(boundingSphere);
        }

        public ContainmentType IsInSightEx(ref BoundingBox boundingBox)
        {
            return mCamera.IsInSightEx(ref boundingBox);
        }

        public ContainmentType IsInSightEx(ref BoundingSphere boundingSphere)
        {
            return mCamera.IsInSightEx(ref boundingSphere);
        }

        public ContainmentType IsInSightEx(BoundingBox boundingBox)
        {
            return mCamera.IsInSightEx(boundingBox);
        }

        public ContainmentType IsInSightEx(BoundingSphere boundingSphere)
        {
            return mCamera.IsInSightEx(boundingSphere);
        }

        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
            if (mZoomLevelAnimator.Update(gameTime))
            {
                UpdateDistanceToTarget(mZoomLevelAnimator.CurrentValue);
            }
            if(mPositionAnimator.Update(gameTime))
            {
                mCamera.TargetWorldPosition = mPositionAnimator.CurrentValue;
            }

            if (0.0f < mShakeMagnitude)
            {
                Vector2 rotation = mBaseRotationArroundTarget;
                rotation.Y += (float)Math.Sin(gameTime.TotalTime * 30) * mShakeMagnitude;
                rotation.X += (float)Math.Sin(gameTime.TotalTime * 20) * mShakeMagnitude;
                mCamera.RotationArroundTarget = rotation;

                mShakeMagnitude -= 0.001f;
                if (0.0f > mShakeMagnitude)
                {
                    mCamera.RotationArroundTarget = mBaseRotationArroundTarget;
                    mShakeMagnitude = -1.0f;
                }
            }
        }

        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {
        }

        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
        }

        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
        }

        private void Default_AppSettingsCommitted(IAppSettings settings)
        {
            if (settings is VideoSettings)
            {
                UpdateAspectRatio();
            }
        }

        private void UpdateAspectRatio()
        {
            var vs = AppSettingsManager.Default.GetSettings<VideoSettings>();
            mCamera.AspectRatio = (float)vs.ScreenWidth / (float)vs.ScreenHeight;
        }

        private void mCamera_ViewProjectionTransformationChanged(ICamera camera)
        {
            if (null != ViewProjectionTransformationChanged)
            {
                ViewProjectionTransformationChanged.Invoke(this);
            }
        }

        private void mCamera_ViewTransformationChanged(ICamera camera)
        {
            if (null != ViewTransformationChanged)
            {
                ViewTransformationChanged.Invoke(this);
            }
        }

        private void mCamera_ProjectionTransformationChanged(ICamera camera)
        {
            if (null != ProjectionTransformationChanged)
            {
                ProjectionTransformationChanged.Invoke(this);
            }
        }

        private void UpdateDistanceToTarget(Vector3 zoomLevel)
        {
            mCamera.DistanceToTarget = zoomLevel.X;
            mCamera.RotationArroundTarget = new Vector2(zoomLevel.Y, zoomLevel.Z);
            mBaseRotationArroundTarget = mCamera.RotationArroundTarget;
        }

        private void SetCameraAnimationGameSceneState()
        {
            CameraAnimationGameSceneState cameraAnimationGameSceneState = (CameraAnimationGameSceneState)mGameScene.GetGameSceneState(GameSceneStates.CameraAnimation);
            cameraAnimationGameSceneState.PreviousGameSceneState = mGameScene.State;
            mGameScene.State = cameraAnimationGameSceneState;
        }
    }
}
