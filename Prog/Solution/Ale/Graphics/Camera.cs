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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;

namespace Ale.Graphics
{
    /// <summary>
    /// Camera that is capable of rotating around its target and it always looks at this target
    /// </summary>
    public class Camera : BaseCamera
    {
        #region Fields

        /// <summary>
        /// Camera's up vector
        /// </summary>
        private Vector3 mCameraUp = Vector3.UnitZ;

        /// <summary>
        /// Field of view in rad (default = 0.785398rad = 45deg)
        /// </summary>
        private float mFieldOfView = MathHelper.PiOver4;

        /// <summary>
        /// Aspect ratio (default = 1.3333)
        /// </summary>
        private float mAspectRatio = 1.3333f;

        /// <summary>
        /// Distance to the near view plane (default = 0.1f)
        /// </summary>
        private float mNearPlaneDistance = 0.1f;

        /// <summary>
        /// Distance to the far view plane (default = 100)
        /// </summary>
        private float mFarPlaneDistance = 100;

        /// <summary>
        /// Target position to which is camera currently looking in the world space
        /// </summary>
        private Vector3 mTargetWorldPosition;

        /// <summary>
        /// Defines camera's distance to its target (you must decrease this value to zoom in)
        /// </summary>
        private float mDistanceToTarget;

        /// <summary>
        /// Camera's rotation arround its target
        /// </summary>
        private Vector2 mRotationArroundTarget;

        /// <summary>
        /// How far may be camera from its target
        /// </summary>
        private float mMaxDistanceToTarget;

        /// <summary>
        /// How near may be camera to its target
        /// </summary>
        private float mMinDistanceToTarget;

        /// <summary>
        /// Max rotation angle in radians arround x
        /// </summary>
        private float mMaxRotX;

        /// <summary>
        /// Min rotation angle in radians arround x
        /// </summary>
        private float mMinRotX;

        /// <summary>
        /// Actual position of the camera in the world space
        /// </summary>
        private Vector3 mWorldPosition;

        #endregion Fields

        #region Property
   
        /// <summary>
        /// Gets/sets the field of view in rad (default = 0.785398rad = 45deg)
        /// </summary>
        public float FieldOfView
        {
            get { return mFieldOfView; }
            set
            {
                //value is checked by UpdateProjectionTransformation
                mFieldOfView = value;
                UpdateProjectionTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the aspect ratio (default = 1.3333)
        /// </summary>
        public float AspectRatio
        {
            get { return mAspectRatio; }
            set
            {
                mAspectRatio = value;
                UpdateProjectionTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the distance to the near view plane (default = 0.1f)
        /// </summary>
        public float NearPlaneDistance
        {
            get { return mNearPlaneDistance; }
            set
            {
                //value is checked by UpdateProjectionTransformation
                mNearPlaneDistance = value;
                UpdateProjectionTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the distance to the far view plane (default = 100)
        /// </summary>
        public float FarPlaneDistance
        {
            get { return mFarPlaneDistance; }
            set
            {
                //value is checked by UpdateProjectionTransformation
                mFarPlaneDistance = value;
                UpdateProjectionTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the position on which is camera looking in the world space
        /// </summary>
        public Vector3 TargetWorldPosition
        {
            get { return mTargetWorldPosition; }
            set
            {
                mTargetWorldPosition = value;
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the defines camera distance to its target (you must decrease this value to zoom in)
        /// </summary>
        public float DistanceToTarget
        {
            get { return mDistanceToTarget; }
            set
            {
                mDistanceToTarget = value;
                SatisfyCameraZoomConstraints();
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the camera rotation arround its target (rot arround x and z axis)
        /// </summary>
        public Vector2 RotationArroundTarget
        {
            get { return mRotationArroundTarget; }
            set
            {
                mRotationArroundTarget = value;
                SatisfyCameraRotationConstraints();
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets the actual position of the camera (it can be changed by defining rotation, distance from target and target position) in the world space
        /// </summary>
        public override Vector3 WorldPosition
        {
            get { return mWorldPosition; }
        }

        /// <summary>
        /// Gets/sets how far may be camera from its target
        /// </summary>
        /// <exception cref="ArgumentException">MinDistanceToTarget is bigger then MaxDistanceToTarget</exception>
        public float MaxDistanceToTarget
        {
            get { return mMaxDistanceToTarget; }
            set
            {
                if (value < MinDistanceToTarget)
                {
                    throw new ArgumentException("MinDistanceToTarget can`t be bigger then MaxDistanceToTarget");
                }

                mMaxDistanceToTarget = value;
                SatisfyCameraZoomConstraints();
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets how near may be camera to its target
        /// </summary>
        /// <exception cref="ArgumentException">MinDistanceToTarget is bigger then MaxDistanceToTarget</exception>
        public float MinDistanceToTarget
        {
            get { return mMinDistanceToTarget; }
            set
            {
                if (value > MaxDistanceToTarget)
                {
                    throw new ArgumentException("MinDistanceToTarget can`t be bigger then MaxDistanceToTarget");
                }

                mMinDistanceToTarget = value;
                SatisfyCameraZoomConstraints();
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the max rotation angle in radians arround x
        /// </summary>
        /// <exception cref="ArgumentException">MinRotX is bigger then maxRotX</exception>
        public float MaxRotX
        {
            get { return mMaxRotX; }
            set
            {
                if (value < MinRotX)
                {
                    throw new ArgumentException("MinRotX can`t be bigger then MaxRotX");
                }

                mMaxRotX = value;
                SatisfyCameraRotationConstraints();
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        /// <summary>
        /// Gets/sets the min rotation angle in radians arround x
        /// </summary>
        /// <exception cref="ArgumentException">MinRotX is bigger then maxRotX</exception>
        public float MinRotX
        {
            get { return mMinRotX; }
            set
            {
                if (value > MaxRotX)
                {
                    throw new ArgumentException("MinRotX can`t be bigger then MaxRotX");
                }

                mMinRotX = value;
                SatisfyCameraRotationConstraints();
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        public override Vector3 CameraUp
        {
            get { return mCameraUp; }
            set 
            { 
                mCameraUp = value;
                UpdateViewTransformation();
                UpdateViewProjectionTransformation();
            }
        }

        #endregion Property

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="target">- Target position</param>
        /// <param name="distanceToTarget">- Camera's initial distance to target (you must decrease this value to zoom in)</param>
        /// <param name="cameraRot">- Camera's initial rotation arround target (rot arround x and y axis)</param>
        /// <param name="maxDistanceToTarget">- Max distance to target (How far may be camera from its target)</param>
        /// <param name="minDistanceToTarget">- Min distance to target (How near may be camera to its target)</param>
        /// <param name="maxRotX">- Max rotation angle in radians arround x</param>
        /// <param name="minRotX">- Min rotation angle in radians arround x</param>
        /// <exception cref="ArgumentException">Min param is bigger then Max param</exception>
        public Camera(Vector3 target, float distanceToTarget, Vector2 cameraRot, float maxDistanceToTarget,
            float minDistanceToTarget, float maxRotX, float minRotX)
        {
            if (minDistanceToTarget > maxDistanceToTarget)
            {
                throw new ArgumentException("minDistanceToTarget can`t be bigger then maxDistanceToTarget");
            }
            if (minRotX > maxRotX)
            {
                throw new ArgumentException("minRotX can`t be bigger then maxRotX");
            }

            mTargetWorldPosition = target;
            mDistanceToTarget = distanceToTarget;
            mRotationArroundTarget = cameraRot;
            mMaxDistanceToTarget = maxDistanceToTarget;
            mMinDistanceToTarget = minDistanceToTarget;
            mMaxRotX = maxRotX;
            mMinRotX = minRotX;

            UpdateViewTransformation();
            UpdateProjectionTransformation();
            UpdateViewProjectionTransformation();
        }

        /// <summary>
        /// Creates a view transformation
        /// </summary>
        /// <param name="viewTransformation"></param>
        protected override void CreateViewTransformation(out Matrix viewTransformation)
        {
            //rotation arround x axis
            Vector2 xRotPos = new Vector2((float)System.Math.Cos(RotationArroundTarget.X) * DistanceToTarget,
                (float)System.Math.Sin(RotationArroundTarget.X) * DistanceToTarget);
            mWorldPosition = new Vector3((float)System.Math.Sin(mRotationArroundTarget.Y) * xRotPos.X + TargetWorldPosition.X,
               -(float)System.Math.Cos(mRotationArroundTarget.Y) * xRotPos.X + TargetWorldPosition.Y,
               -xRotPos.Y + TargetWorldPosition.Z);

            //View transformation
            Vector3 cameraUp = CameraUp;
            Matrix.CreateLookAt(ref mWorldPosition, ref mTargetWorldPosition, ref cameraUp, out viewTransformation);      
        }

        /// <summary>
        /// Creates a projection transformation
        /// </summary>
        /// <param name="projectionTransformation"></param>
        protected override void CreateProjectionTransformation(out Matrix projectionTransformation)
        {
            Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlaneDistance, FarPlaneDistance, out projectionTransformation);
        }

        /// <summary>
        /// Updates camera rotation to satisfy rotation constraints
        /// </summary>
        /// <remarks>
        /// constraints:
        /// rotation arround x axis -> min = mMinRotX and max = mMaxRotX
        /// rotation arround y axis -> min = 0 and max = 6.283185f
        /// </remarks>
        private void SatisfyCameraRotationConstraints()
        {
            if (mMinRotX > RotationArroundTarget.X)
            {
                mRotationArroundTarget.X = mMinRotX;
            }
            else
            {
                if (mMaxRotX < RotationArroundTarget.X)
                {
                    mRotationArroundTarget.X = mMaxRotX;
                }
            }
            // Console.WriteLine(CameraRot.Y);
            if (0.0f > RotationArroundTarget.Y)
            {
                mRotationArroundTarget.Y = 6.283185f + RotationArroundTarget.Y; //360 deg
            }
            else
            {
                if (6.283185f < RotationArroundTarget.Y)
                {
                    mRotationArroundTarget.Y = RotationArroundTarget.Y - 6.283185f;
                }
            }
        }

        /// <summary>
        /// Updates camera zoom to satisfy zooming constraints
        /// </summary>
        /// <remarks>
        /// Constraints: specified min and max value
        /// </remarks>
        private void SatisfyCameraZoomConstraints()
        {
            if (DistanceToTarget < mMinDistanceToTarget)
            {
                DistanceToTarget = mMinDistanceToTarget;
            }
            else
            {
                if (DistanceToTarget > mMaxDistanceToTarget)
                {
                    DistanceToTarget = mMaxDistanceToTarget;
                }
            }
        }

        #endregion Methods
    }

    //aux
    class OrthoCamera : Camera
    {
        public OrthoCamera(Vector3 target, float distanceToTarget, Vector2 cameraRot, float maxDistanceToTarget,
            float minDistanceToTarget, float maxRotX, float minRotX)
            : base(target, distanceToTarget, cameraRot, maxDistanceToTarget, minDistanceToTarget, maxRotX, minRotX)
        {
        }

        protected override void CreateProjectionTransformation(out Matrix projectionTransformation)
        {
            Matrix.CreateOrthographic(2, 2, -100f, 100f, out projectionTransformation);
        }
    }


    public delegate void CameraTransformationChangedHandler(ICamera camera);

    public interface ICamera
    {
        #region Events

        event CameraTransformationChangedHandler ViewTransformationChanged;
        event CameraTransformationChangedHandler ProjectionTransformationChanged;
        event CameraTransformationChangedHandler ViewProjectionTransformationChanged;

        #endregion Events

        #region Property

        /// <summary>
        /// Gets the current view transformation matrix
        /// </summary>
        Matrix ViewTransformation
        {
            get;
        }

        /// <summary>
        /// Gets the current projection transformation matrix
        /// </summary>
        Matrix ProjectionTransformation
        {
            get;
        }

        /// <summary>
        /// Gets the current view*projection transformation matrix
        /// </summary>
        Matrix ViewProjectionTransformation
        {
            get;
        }

        /// <summary>
        /// Inverted ViewProjectionTransformation
        /// </summary>
        Matrix ViewProjectionTransformationInv
        {
            get;
        }

        /// <summary>
        /// Camera's up vector
        /// </summary>
        Vector3 CameraUp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the actual position of the camera (it can be changed by defining rotation, distance from target and target position) in the world space
        /// </summary>
        Vector3 WorldPosition
        {
            get;
        }

        /// <summary>
        /// Gets the corners of the bounding Frustum
        /// </summary>
        ReadOnlyCollection<Vector3> FrustumCorners
        {
            get;
        }

        /// <summary>
        /// Gets the bottom plane of the BoundingFrustum.
        /// </summary>
        Plane FrustumBottom 
        { 
            get; 
        }

        /// <summary>
        /// Gets the far plane of the BoundingFrustum.
        /// </summary>
        Plane FrustumFar
        { 
            get; 
        }
        
        /// <summary>
        /// Gets the left plane of the BoundingFrustum.
        /// </summary>
        Plane FrustumLeft
        { 
            get; 
        }
        
        /// <summary>
        /// Gets the near plane of the BoundingFrustum.
        /// </summary>
        Plane FrustumNear
        { 
            get; 
        }
        
        /// <summary>
        /// Gets the right plane of the BoundingFrustum.
        /// </summary>
        Plane FrustumRight
        { 
            get; 
        }
        
        /// <summary>
        /// Gets the top plane of the BoundingFrustum.
        /// </summary>
        Plane FrustumTop
        { 
            get; 
        }

        #endregion Property

        #region Methods

        /// <summary>
        /// Gets the ray from camera to point in viewport
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="viewport"></param>
        /// <param name="ray"></param>
        void CameraToViewport(Vector2 point, Viewport viewport, out Ray ray);
      
        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingBox</returns>
        bool IsInSight(ref BoundingBox boundingBox);
       

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingSphere</returns>
        bool IsInSight(ref BoundingSphere boundingSphere);
        

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingBox</returns>
        bool IsInSight(BoundingBox boundingBox);
       
        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingSphere</returns>
        bool IsInSight(BoundingSphere boundingSphere);
        
        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>ContainmentType</returns>
        ContainmentType IsInSightEx(ref BoundingBox boundingBox);
        
        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>ContainmentType</returns>
        ContainmentType IsInSightEx(ref BoundingSphere boundingSphere);
        

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>ContainmentType</returns>
        ContainmentType IsInSightEx(BoundingBox boundingBox);
     
        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>ContainmentType</returns>
        ContainmentType IsInSightEx(BoundingSphere boundingSphere);
        
        #endregion Methods
    }


    public abstract class BaseCamera : ICamera, ICloneable
    {
        #region Events

        public event CameraTransformationChangedHandler ViewTransformationChanged;
        public event CameraTransformationChangedHandler ProjectionTransformationChanged;
        public event CameraTransformationChangedHandler ViewProjectionTransformationChanged;

        #endregion Events

        #region Fields

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

        /// <summary>
        /// Frustum of the camera in the world space
        /// </summary>
        private BoundingFrustum mWorldFrustum;

        private ReadOnlyCollection<Vector3> mFrustumCorners;

        #endregion Fields

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
        public abstract Vector3 CameraUp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the actual position of the camera
        /// </summary>
        public abstract Vector3 WorldPosition
        {
            get;
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

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseCamera()
        {
            mWorldFrustum = new BoundingFrustum(Matrix.Identity);

            UpdateViewTransformation();
            UpdateProjectionTransformation();
            UpdateViewProjectionTransformation();
        }

        /// <summary>
        /// Gets the ray from camera to point in viewport
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="viewport"></param>
        /// <param name="ray"></param>
        public void CameraToViewport(Vector2 point, Viewport viewport, out Ray ray)
        {
            Vector3 p1;
            Vector3 p2;
            Vector3 vpMousePos;

            //p1
            vpMousePos = new Vector3(
                (point.X - viewport.X) / (float)viewport.Width * 2f - 1f,
                vpMousePos.Y = -((point.Y - viewport.Y) / (float)viewport.Height * 2f - 1f),
                vpMousePos.Z = -viewport.MinDepth / (viewport.MaxDepth - viewport.MinDepth));

            Vector3.Transform(ref vpMousePos, ref mViewProjectionTransformationInv, out p1);
            float a = vpMousePos.X * mViewProjectionTransformationInv.M14 + vpMousePos.Y * mViewProjectionTransformationInv.M24 +
                vpMousePos.Z * mViewProjectionTransformationInv.M34 + mViewProjectionTransformationInv.M44;
            if (float.Epsilon < Math.Abs(a - 1.0f)) //not 1.0f
            {
                p1 = (Vector3)(p1 / a);
            }

            //p2
            vpMousePos.Z = (1 - viewport.MinDepth) / (viewport.MaxDepth - viewport.MinDepth);

            Vector3.Transform(ref vpMousePos, ref mViewProjectionTransformationInv, out p2);


            a = vpMousePos.X * mViewProjectionTransformationInv.M14 + vpMousePos.Y * mViewProjectionTransformationInv.M24 +
                vpMousePos.Z * mViewProjectionTransformationInv.M34 + mViewProjectionTransformationInv.M44;
            if (float.Epsilon < Math.Abs(a - 1.0f)) //not 1.0f
            {
                p2 = (Vector3)(p2 / a);
            }


            ray = new Ray(p1, Vector3.Normalize(p2 - p1));
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingBox</returns>
        public bool IsInSight(ref BoundingBox boundingBox)
        {
            ContainmentType containmentType;
            mWorldFrustum.Contains(ref boundingBox, out containmentType);
            return (ContainmentType.Disjoint != containmentType);
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingSphere</returns>
        public bool IsInSight(ref BoundingSphere boundingSphere)
        {
            ContainmentType containmentType;
            mWorldFrustum.Contains(ref boundingSphere, out containmentType);
            return (ContainmentType.Disjoint != containmentType);
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingBox</returns>
        public bool IsInSight(BoundingBox boundingBox)
        {
            return (ContainmentType.Disjoint != mWorldFrustum.Contains(boundingBox));
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>Whether camera's Frustum contatins a specified BoundingSphere</returns>
        public bool IsInSight(BoundingSphere boundingSphere)
        {
            return (ContainmentType.Disjoint != mWorldFrustum.Contains(boundingSphere));
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>ContainmentType</returns>
        public ContainmentType IsInSightEx(ref BoundingBox boundingBox)
        {
            ContainmentType containmentType;
            mWorldFrustum.Contains(ref boundingBox, out containmentType);
            return containmentType;
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>ContainmentType</returns>
        public ContainmentType IsInSightEx(ref BoundingSphere boundingSphere)
        {
            ContainmentType containmentType;
            mWorldFrustum.Contains(ref boundingSphere, out containmentType);
            return containmentType;
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingBox
        /// </summary>
        /// <param name="boundingBox">BoundingBox to check</param>
        /// <returns>ContainmentType</returns>
        public ContainmentType IsInSightEx(BoundingBox boundingBox)
        {
            return mWorldFrustum.Contains(boundingBox);
        }

        /// <summary>
        /// Gets whether camera's Frustum contatins a specified BoundingSphere
        /// </summary>
        /// <param name="boundingSphere">BoundingSphere to check</param>
        /// <returns>ContainmentType</returns>
        public ContainmentType IsInSightEx(BoundingSphere boundingSphere)
        {
            return mWorldFrustum.Contains(boundingSphere);
        }


        #region ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns>Object's copy</returns>
        public object Clone()
        {
            BaseCamera camera = (BaseCamera)MemberwiseClone();
            camera.mWorldFrustum = new BoundingFrustum(mViewProjectionTransformation);
            camera.mFrustumCorners = new ReadOnlyCollection<Vector3>(mWorldFrustum.GetCorners());
            return camera;
        }

        #endregion ICloneables

        /// <summary>
        /// Creates a view transformation
        /// </summary>
        /// <param name="viewTransformation"></param>
        protected abstract void CreateViewTransformation(out Matrix viewTransformation);

        /// <summary>
        /// Creates a projection transformation
        /// </summary>
        /// <param name="projectionTransformation"></param>
        protected abstract void CreateProjectionTransformation(out Matrix projectionTransformation);

        /// <summary>
        /// Updates the view transformation
        /// </summary>
        protected void UpdateViewTransformation()
        {
            CreateViewTransformation(out mViewTransformation);

            if (null != ViewTransformationChanged)
            {
                ViewTransformationChanged.Invoke(this);
            }
        }

        /// <summary>
        /// Updates the projection transformation
        /// </summary>
        protected void UpdateProjectionTransformation()
        {
            CreateProjectionTransformation(out mProjectionTransformation);

            if (null != ProjectionTransformationChanged)
            {
                ProjectionTransformationChanged.Invoke(this);
            }
        }

        /// <summary>
        /// Updates the view*projection transformation
        /// </summary>
        protected void UpdateViewProjectionTransformation()
        {
            Matrix.Multiply(ref mViewTransformation, ref mProjectionTransformation, out mViewProjectionTransformation);
            mWorldFrustum.Matrix = mViewProjectionTransformation;
            mFrustumCorners = new ReadOnlyCollection<Vector3>(mWorldFrustum.GetCorners());

            Matrix.Invert(ref mViewProjectionTransformation, out mViewProjectionTransformationInv);

            if (null != ViewProjectionTransformationChanged)
            {
                ViewProjectionTransformationChanged.Invoke(this);
            }
        }

        #endregion Methods
    }










}
