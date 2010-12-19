//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Ale.Graphics;

//namespace Conquera
//{
//    class WaterReflectionCamera : Camera, IDisposable
//    {
//        Camera mMainCamera;
//        bool mIsDisposed = false;

//        public WaterReflectionCamera(Camera mainCamera)
//            : base(mainCamera.TargetWorldPosition, mainCamera.DistanceToTarget, Vector2.Zero, mainCamera.MaxDistanceToTarget,
//            mainCamera.MinDistanceToTarget, -mainCamera.MinRotX, -mainCamera.MaxRotX)
//        {
//            if (null == mainCamera) throw new ArgumentNullException("mainCamera");

//            CameraUp = -mainCamera.CameraUp;

//            mMainCamera = mainCamera;

//            mMainCamera.ViewProjectionTransformationChanged += new CameraTransformationChangedHandler(mMainCamera_ViewProjectionTransformationChanged);

//            Update();
//        }

//        /// <summary>
//        /// Dispose
//        /// </summary>
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        protected virtual void Dispose(bool isDisposing)
//        {
//            if (!mIsDisposed)
//            {
//                if (isDisposing)
//                {
//                    mMainCamera.ViewProjectionTransformationChanged -= mMainCamera_ViewProjectionTransformationChanged;
//                }
//                mIsDisposed = true;
//            }
//        }

//        private void mMainCamera_ViewProjectionTransformationChanged(ICamera camera)
//        {
//            Update();
//        }
        
//        private void Update()
//        {
//            if (null != mMainCamera)
//            {
//                Vector3 targetPos = mMainCamera.TargetWorldPosition;
//                targetPos.Z *= -1;
//                TargetWorldPosition = targetPos;
//                Vector2 rotationArroundTarget = mMainCamera.RotationArroundTarget;
//                rotationArroundTarget.X = -rotationArroundTarget.X;
//                RotationArroundTarget = rotationArroundTarget;
//                DistanceToTarget = mMainCamera.DistanceToTarget;
//            }
//        }
//    }
//}
