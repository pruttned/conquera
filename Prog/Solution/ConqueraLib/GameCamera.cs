﻿using System;
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
        /// </summary>
        private static Vector2[] ZoomLevels = new Vector2[] { new Vector2(15, -1.5f), new Vector2(10, -1.1f), new Vector2(5, -0.9f) };

        private Camera mCamera;
        private GameScene mGameScene;
        private bool mIsDisposed = false;


        private int mZoomLevel = 1;

        private Vector3LinearAnimator mPositionAnimator = new Vector3LinearAnimator();
        private Vector2LinearAnimator mZoomLevelAnimator = new Vector2LinearAnimator();

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
            if (mZoomLevel < ZoomLevels.Length-1)
            {
                SetCameraAnimationGameSceneState();

                mZoomLevel++;

                Vector2 curZommLevel = new Vector2(mCamera.DistanceToTarget, mCamera.RotationArroundTarget.X);
                mZoomLevelAnimator.Animate(15, curZommLevel, ZoomLevels[mZoomLevel]);
            }
        }

        public void DecZoomLevel()
        {
            if (mZoomLevel > 0)
            {
                SetCameraAnimationGameSceneState();
            
                mZoomLevel--;

                Vector2 curZommLevel = new Vector2(mCamera.DistanceToTarget, mCamera.RotationArroundTarget.X);
                mZoomLevelAnimator.Animate(15, curZommLevel, ZoomLevels[mZoomLevel]);
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

        private void UpdateDistanceToTarget(Vector2 zoomLevel)
        {
            mCamera.DistanceToTarget = zoomLevel.X;
            mCamera.RotationArroundTarget = new Vector2(zoomLevel.Y, mCamera.RotationArroundTarget.Y);
        }

        private void SetCameraAnimationGameSceneState()
        {
            CameraAnimationGameSceneState cameraAnimationGameSceneState = (CameraAnimationGameSceneState)mGameScene.States[typeof(CameraAnimationGameSceneState)];
            cameraAnimationGameSceneState.PreviousGameSceneState = mGameScene.State;
            mGameScene.State = cameraAnimationGameSceneState;
        }
    }
}
