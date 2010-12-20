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
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    class ShadowOrthoCamera : ICamera, IDisposable
    {
        public event CameraTransformationChangedHandler ViewTransformationChanged;
        public event CameraTransformationChangedHandler ProjectionTransformationChanged;
        public event CameraTransformationChangedHandler ViewProjectionTransformationChanged;

        private ICamera mMainCamera;
        bool mIsDisposed = false;
//        private Vector3 mLightDir = new Vector3(-0.3333333f, 0.1f, 1.4f);
        private Vector3 mLightDir;
        private Vector3 mWorldPosition = Vector3.Zero;
        private Plane mGroundPlane;

        /// <summary>
        /// Current view transformation matrix
        /// </summary>
        private Matrix mViewTransformation;

        /// <summary>
        /// Current projection transformation matrix
        /// </summary>
        private Matrix mProjectionTransformation;

        /// <summary>
        /// Current view-projection transformation matrix
        /// </summary>
        private Matrix mViewProjectionTransformation;

        /// <summary>
        /// Inverted mViewProjectionTransformation
        /// </summary>
        private Matrix mViewProjectionTransformationInv;

        private BoundingFrustum mWorldFrustum = new BoundingFrustum(Matrix.Identity);
        private ReadOnlyCollection<Vector3> mFrustumCorners;


        #region Property

        /// <summary>
        /// Gets the current view transformation matrix
        /// </summary>
        public Matrix ViewTransformation
        {
            get { return mViewTransformation; }
        }

        /// <summary>
        /// Gets the current projection transformation matrix
        /// </summary>
        public Matrix ProjectionTransformation
        {
            get { return mProjectionTransformation; }
        }

        /// <summary>
        /// Gets the current view*projection transformation matrix
        /// </summary>
        public Matrix ViewProjectionTransformation
        {
            get { return mViewProjectionTransformation; }
        }

        /// <summary>
        /// Inverted ViewProjectionTransformation
        /// </summary>
        public Matrix ViewProjectionTransformationInv
        {
            get { return mViewProjectionTransformationInv; }
        }

        /// <summary>
        /// Gets the corners of the bounding Frustum
        /// </summary>
        public ReadOnlyCollection<Vector3> FrustumCorners
        {
            get { return mFrustumCorners; }
        }

        /// <summary>
        /// Camera's up vector
        /// </summary>
        public Vector3 CameraUp
        {
            get { return Vector3.UnitZ; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the actual position of the camera
        /// </summary>
        public Vector3 WorldPosition
        {
            get { return mWorldPosition; }
        }

        /// <summary>
        /// Gets the bottom plane of the BoundingFrustum.
        /// </summary>
        public Plane FrustumBottom
        {
            get { return mWorldFrustum.Bottom; }
        }

        /// <summary>
        /// Gets the far plane of the BoundingFrustum.
        /// </summary>
        public Plane FrustumFar
        {
            get { return mWorldFrustum.Far; }
        }

        /// <summary>
        /// Gets the left plane of the BoundingFrustum.
        /// </summary>
        public Plane FrustumLeft
        {
            get { return mWorldFrustum.Left; }
        }

        /// <summary>
        /// Gets the near plane of the BoundingFrustum.
        /// </summary>
        public Plane FrustumNear
        {
            get { return mWorldFrustum.Near; }
        }

        /// <summary>
        /// Gets the right plane of the BoundingFrustum.
        /// </summary>
        public Plane FrustumRight
        {
            get { return mWorldFrustum.Right; }
        }

        /// <summary>
        /// Gets the top plane of the BoundingFrustum.
        /// </summary>
        public Plane FrustumTop
        {
            get { return mWorldFrustum.Top; }
        }

        #endregion Property

        public ShadowOrthoCamera(ICamera mainCamera, Vector3 lightDir, Plane groundPlane)
        {
            if (null == mainCamera) throw new ArgumentNullException("mainCamera");

            mLightDir = lightDir;
            mLightDir.Normalize();

            mGroundPlane = groundPlane;

            mMainCamera = mainCamera;

            mMainCamera.ViewProjectionTransformationChanged += new CameraTransformationChangedHandler(mMainCamera_ViewProjectionTransformationChanged);

            UpdateViewProjectionTransformation();
        }

        void mMainCamera_ViewProjectionTransformationChanged(ICamera camera)
        {
            UpdateViewProjectionTransformation();
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

        public void CameraToViewport(Vector2 point, Viewport viewport, out Ray ray)
        {
            throw new NotImplementedException();
        }

        public bool IsInSight(ref BoundingBox boundingBox)
        {
            return mMainCamera.IsInSight(ref boundingBox);
        }

        public bool IsInSight(ref BoundingSphere boundingSphere)
        {
            return mMainCamera.IsInSight(ref boundingSphere);
        }

        public bool IsInSight(BoundingBox boundingBox)
        {
            return mMainCamera.IsInSight(boundingBox);
        }

        public bool IsInSight(BoundingSphere boundingSphere)
        {
            return mMainCamera.IsInSight(boundingSphere);
        }

        public ContainmentType IsInSightEx(ref BoundingBox boundingBox)
        {
            return mMainCamera.IsInSightEx(ref boundingBox);
        }

        public ContainmentType IsInSightEx(ref BoundingSphere boundingSphere)
        {
            return mMainCamera.IsInSightEx(ref boundingSphere);
        }

        public ContainmentType IsInSightEx(BoundingBox boundingBox)
        {
            return mMainCamera.IsInSightEx(boundingBox);
        }

        public ContainmentType IsInSightEx(BoundingSphere boundingSphere)
        {
            return mMainCamera.IsInSightEx(boundingSphere);
        }

        private void UpdateViewProjectionTransformation()
        {
            if (null != mMainCamera)
            {
                //based on http://creators.xna.com/en-US/sample/shadowmapping1

                Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                           -mLightDir,
                                                           CameraUp);
                // Get the corners of the frustum
                //                List<Vector3> frustumCorners = new List<Vector3>(mMainCamera.FrustumCorners);
                IList<Vector3> frustumCorners = ComputeMainCameraClippedFrustumPoints();

                // Transform the positions of the corners into the direction of the light
                for (int i = 0; i < frustumCorners.Count; i++)
                {
                    frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);
                }

                // Find the smallest box around the points
                BoundingBox lightBox = BoundingBox.CreateFromPoints(frustumCorners);

                Vector3 boxSize = lightBox.Max - lightBox.Min;
                Vector3 halfBoxSize = boxSize * 0.5f;

                // The position of the light should be in the center of the back pannel of the box. 
                mWorldPosition = lightBox.Min + halfBoxSize;
                mWorldPosition.Z = lightBox.Min.Z;

                // We need the position back in world coordinates so we transform  the light position by the inverse of the lights rotation
                mWorldPosition = Vector3.Transform(mWorldPosition,
                                                  Matrix.Invert(lightRotation));

                // Create the view matrix for the light
                mViewTransformation = Matrix.CreateLookAt(mWorldPosition, mWorldPosition - mLightDir, CameraUp);
                mProjectionTransformation = Matrix.CreateOrthographic(boxSize.X, boxSize.Y, -boxSize.Z, boxSize.Z);

                Matrix.Multiply(ref mViewTransformation, ref mProjectionTransformation, out mViewProjectionTransformation);
                mWorldFrustum.Matrix = mViewProjectionTransformation;
                mFrustumCorners = new ReadOnlyCollection<Vector3>(mWorldFrustum.GetCorners());

                Matrix.Invert(ref mViewProjectionTransformation, out mViewProjectionTransformationInv);
            }
            else
            {
                mViewTransformation = Matrix.Identity;
            }

            if (null != ViewTransformationChanged)
            {
                ViewTransformationChanged.Invoke(this);
            }
            if (null != ProjectionTransformationChanged)
            {
                ProjectionTransformationChanged.Invoke(this);
            }
            if (null != ViewProjectionTransformationChanged)
            {
                ViewProjectionTransformationChanged.Invoke(this);
            }
        }


        IList<Vector3> ComputeMainCameraClippedFrustumPoints()
        {
            //Far clipping plane will be a ground plane

            Vector3[] corners = new Vector3[8];

            Plane frustumNear = mMainCamera.FrustumNear;
            Plane frustumFar = mGroundPlane; //ground plane
            Plane frustumLeft = mMainCamera.FrustumLeft;
            Plane frustumRight = mMainCamera.FrustumRight;
            Plane frustumTop = mMainCamera.FrustumTop;
            Plane frustumBottom = mMainCamera.FrustumBottom;

            Ray intRay;

            GetPlanePlaneIntersection(ref frustumNear, ref frustumLeft, out intRay);
            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[0]);
            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[3]);

            GetPlanePlaneIntersection(ref frustumRight, ref frustumNear, out intRay);
            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[1]);
            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[2]);

            GetPlanePlaneIntersection(ref frustumLeft, ref frustumFar, out intRay);
            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[4]);
            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[7]);

            GetPlanePlaneIntersection(ref frustumFar, ref frustumRight, out intRay);
            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[5]);
            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[6]);

            return corners;
        }

        private void GetPlanePlaneIntersection(ref Plane p1, ref Plane p2, out Ray ray)
        {
            Vector3.Cross(ref p1.Normal, ref p2.Normal, out ray.Direction);
            ray.Position = Vector3.Cross(((-p1.D * p2.Normal) + (p2.D * p1.Normal)), ray.Direction) / ray.Direction.LengthSquared();
        }

        private void GetPlaneRayIntersection(ref Plane plane, ref Ray ray, out Vector3 point)
        {
            point = ray.Position + (ray.Direction * ((-plane.D - Vector3.Dot(plane.Normal, ray.Position)) / Vector3.Dot(plane.Normal, ray.Direction)));
        }
    }
}
