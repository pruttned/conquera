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
