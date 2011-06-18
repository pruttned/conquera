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

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Ale.Scene;
using Ale.Tools;
using System;

namespace Ale.Graphics
{
    /// <summary>
    /// Specifies abstract for all objects that can be rendered. Each renderable objects consist of a renderable
    /// units specified by IRenderableUnit interface
    /// </summary>
    public abstract class Renderable : IOctreeObject
    {
        #region Events

        #region IOctreeObject

        /// <summary>
        /// Raised whenever the bounds in the world space has been changed
        /// </summary>
        public event WorldBoundsChangedHandler WorldBoundsChanged;

        #endregion IOctreeObject
        
        #endregion Events

        #region Fields

        /// <summary>
        /// Position
        /// </summary>
        private Vector3 mPosition = Vector3.Zero;

        /// <summary>
        /// Scale
        /// </summary>
        private float mScale = 1.0f;

        /// <summary>
        /// Orientation
        /// </summary>
        private Quaternion mOrientation = Quaternion.Identity;

        /// <summary>
        /// Untransformed renderable's bounding sphere
        /// </summary>
        private BoundingSphere mBounds;

        /// <summary>
        /// Renderable's bounding sphere used for visibility determination in a world space
        /// </summary>
        private BoundingSphere mWorldBounds;

        /// <summary>
        /// World transformation matrix
        /// </summary>
        private Matrix mWorldTransformation = Matrix.Identity;

        /// <summary>
        /// Wether this renderable suuport scaling
        /// </summary>
        private bool mSupportsScale;

        /// <summary>
        /// BoundingSphereRenderable for displaying world bounds
        /// </summary>
        private BoundingSphereRenderable mWorldBoundsRenderable = null;

        private bool mIsVisible = true;

        private GraphicModelConnectionPoint mParentConnectionPoint = null;

        private IRenderableListener mRenderableListener = null;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets/sets the position
        /// </summary>
        public Vector3 Position
        {
            get { return mPosition; }
            set 
            { 
                mPosition = value;

                UpdateTransformation();
            }
        }

        /// <summary>
        /// Gets the position from the world transf
        /// </summary>
        public Vector3 WorldPosition
        {
            get { return mWorldTransformation.Translation; }
        }

        /// <summary>
        /// Gets the object's center in world space
        /// </summary>
        public Vector3 WorldBoundsCenter
        {
            get { return mWorldBounds.Center; }
        }

        /// <summary>
        /// Gets/sets the relative scale in the world space
        /// </summary>
        public float Scale
        {
            get { return mScale; }
            set 
            {
                if (SupportsScale)
                {
                    mScale = value;

                    UpdateTransformation();
                }
            }
        }

        /// <summary>
        /// Gets/sets the relative orientation
        /// </summary>
        public Quaternion Orientation
        {
            get { return mOrientation; }
            set 
            {
                mOrientation = value;

                UpdateTransformation();
            }
        }

        /// <summary>
        /// Gets the renderable's bounding sphere used for visibility determination in a world space
        /// </summary>
        public BoundingSphere WorldBounds
        {
            get { return mWorldBounds; }
        }

        /// <summary>
        /// Gets the world transformation matrix
        /// </summary>
        public Matrix WorldTransformation
        {
            get { return mWorldTransformation; }
        }

        /// <summary>
        /// Gets wether this renderable support scaling
        /// </summary>
        public bool SupportsScale
        {
            get { return mSupportsScale; }
        }

        /// <summary>
        /// Gets/Sets whether should be world bounds rendered
        /// </summary>
        public bool ShowWorldBounds
        {
            get { return (null != mWorldBoundsRenderable); }
            set
            {
                if (value)
                {
                    if (null == mWorldBoundsRenderable)
                    {
                        mWorldBoundsRenderable = new BoundingSphereRenderable(Color.White);
                        mWorldBoundsRenderable.SetBoundingSphere(WorldBounds);
                    }
                }
                else
                {
                    mWorldBoundsRenderable = null;
                }
            }
        }

        /// <summary>
        /// Gets/sets the untransformed renderable's bounding sphere
        /// </summary>
        public BoundingSphere Bounds
        {
            get { return mBounds; }
            protected set
            {
                mBounds = value;
                UpdateWorldBounds();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible
        {
            get { return mIsVisible; }
            set 
            {
                if (mIsVisible != value)
                {
                    mIsVisible = value;
                    OnVisibleChanged(value);

                    if (null != mRenderableListener)
                    {
                        mRenderableListener.OnVisibleChanged(this);
                    }
                }
            }
        }
        
        public IRenderableListener RenderableListener 
		{
			get { return mRenderableListener; }
			set { mRenderableListener = value; }
		}

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bounds">- Untransformed bounding sphere of the renderable</param>
        /// <param name="supportsScale">- Wether this renderable suuport scaling</param>
        public Renderable(BoundingSphere bounds, bool supportsScale)
        {
            Bounds = bounds;
            mSupportsScale = supportsScale;
        }

        /// <summary>
        /// Sets the position, orientation and scale at once. It is more effective then setting multiple properties individually.
        /// World transformation is computed after all properties are updated and not after update of a each property.
        /// </summary>
        public void SetTransformation(Vector3 position, Quaternion orientation, float scale)
        {
            SetTransformation(ref position, ref orientation, scale);
        }

        /// <summary>
        /// Sets the position, orientation and scale at once. It is more effective then setting multiple properties individually.
        /// World transformation is computed after all properties are updated and not after update of a each property.
        /// </summary>
        public void SetTransformation(ref Vector3 position, ref Quaternion orientation, float scale)
        {
            mPosition = position;
            mOrientation = orientation;
            mScale = scale;
            
            UpdateTransformation();
        }

        /// <summary>
        /// Sets the position, orientation and scale at once. It is more effective then setting multiple properties individually.
        /// World transformation is computed after all properties are updated and not after update of a each property.
        /// </summary>
        public void SetTransformation(Vector3 worldPosition, Quaternion worldOrientation)
        {
            SetTransformation(ref worldPosition, ref worldOrientation);
        }

        /// <summary>
        /// Sets the position, orientation and scale at once. It is more effective then setting multiple properties individually.
        /// World transformation is computed after all properties are updated and not after update of a each property.
        /// </summary>
        /// <param name="worldPosition">- Position in world space</param>
        /// <param name="worldOrientation">- Orientationin int world space</param>
        public void SetTransformation(ref Vector3 position, ref Quaternion orientation)
        {
            mPosition = position;
            mOrientation = orientation;

            UpdateTransformation();
        }

        /// <summary>
        /// Enque renderable units that should be rendered in the actual frame
        /// </summary>
        /// <param name="renderer">- Renderer that should be used for enqueueing (Use Renderer.EnqueueRenderable method)</param>
        /// <param name="gameTime">- Actual game time</param>
        public void EnqueRenderableUnits(IRenderer renderer, AleGameTime gameTime)
        {
            if (IsVisible)
            {
                if (ShowWorldBounds)
                {
                    mWorldBoundsRenderable.EnqueRenderableUnits(renderer, gameTime);
                }

                OnEnqueRenderableUnits(renderer, gameTime);

                if (null != mRenderableListener)
                {
                    mRenderableListener.OnEnqueRenderableUnits(this, renderer, gameTime);
                }
            }
        }
        
        internal void OnAttachToConnectionPoint(GraphicModelConnectionPoint connectionPoint)
        {
            mParentConnectionPoint = connectionPoint;
            UpdateTransformation();
        }

        internal void OnDetachFromConnectionPoint()
        {
            mParentConnectionPoint = null;
            UpdateTransformation();
        }

        /// <summary>
        /// Updates the local transformation matrix according to scale, orientation and position
        /// </summary>
        internal void UpdateTransformation()
        {
            if (null == mParentConnectionPoint)
            {
                BuildTransformation(out mWorldTransformation);
            }
            else
            {
                Matrix localTransf;
                BuildTransformation(out localTransf);

                Matrix parentTransf = mParentConnectionPoint.WorldTransformation;

                Matrix.Multiply(ref localTransf, ref parentTransf, out mWorldTransformation);
            }

            UpdateWorldBounds();
        }

        /// <summary>
        /// Enque renderable units that should be rendered in the actual frame
        /// </summary>
        /// <param name="renderer">- Renderer that should be used for enqueueing (Use Renderer.EnqueueRenderable method)</param>
        /// <param name="gameTime">- Actual game time</param>
        protected abstract void OnEnqueRenderableUnits(IRenderer renderer, AleGameTime gameTime);

        private void BuildTransformation(out Matrix matrix)
        {
            if (SupportsScale)
            {
                AleMathUtils.BuildTransformation(ref mPosition, ref mOrientation, mScale, out matrix);
            }
            else
            {
                AleMathUtils.BuildTransformation(ref mPosition, ref mOrientation, out matrix);
            }
        }

        /// <summary>
        /// Updates the WorldBounds according to the world transformation
        /// </summary>
        private void UpdateWorldBounds()
        {
            //mBounds.Transform(ref mWorldTransformation, out mWorldBounds);  -not working with rotation

            if (SupportsScale)
            {
                //doesn't take parent renderable scale to consideration  - wrong but not neceseary because attached objects are not checked for visibility
                Vector3 worldPos = WorldPosition;
                AleMathUtils.TransformBoundingSphere(ref worldPos, mScale, ref mBounds, out mWorldBounds);
            }
            else
            {
                mWorldBounds.Radius = mBounds.Radius;
                mWorldBounds.Center = mBounds.Center + WorldPosition;
            }

            if (ShowWorldBounds)
            {
                mWorldBoundsRenderable.SetBoundingSphere(mWorldBounds);
            }

            OnWorldBoundsChanged();

            if (null != mRenderableListener)
            {
                mRenderableListener.OnWorldBoundsChanged(this);
            }

            if (null != WorldBoundsChanged)
            {
                WorldBoundsChanged.Invoke(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        protected virtual void OnVisibleChanged(bool visible) { }

        /// <summary>
        /// Executed whenever has been bounds in the world space changed (nothing by default)
        /// </summary>
        protected virtual void OnWorldBoundsChanged() { }

        #endregion Methods
    }
}


