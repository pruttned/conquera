using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Tools;
using Ale.Content;

namespace Ale.Scene
{
    public abstract class OctreeSceneObject : SceneObject
    {
        private OctreeSceneObjectDesc mDesc;
        private GraphicModel mGraphicModel;
        private Vector3 mPosition = Vector3.Zero;
        private Quaternion mOrientation = Quaternion.Identity;
        private bool mShowWorldBounds = false;
        private bool mIsVisible = true;

        public Vector3 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                if (mPosition != value)
                {
                    mPosition = value;
                    if (null != mGraphicModel)
                    {
                        mGraphicModel.Position = mPosition + Desc.GraphicModel.Position;
                    }
                    OnPositionChanged();
                }
            }
        }

        public bool IsVisible
        {
            get
            {
                return mIsVisible;
            }
            set 
            {
                if (mIsVisible != value)
                {
                    mIsVisible = value;
                    if (null != mGraphicModel)
                    {
                        mGraphicModel.IsVisible = mIsVisible;
                    }
                    OnVisibleChanged();
                }
            }
        }

        public bool ShowWorldBounds
        {
            get
            {
                return mShowWorldBounds;
            }
            set
            {
                if (mShowWorldBounds != value)
                {
                    mShowWorldBounds = value;
                    if (null != mGraphicModel)
                    {
                        mGraphicModel.ShowWorldBounds = mShowWorldBounds;
                    }
                    OnShowWorldBoundsChanged();
                }
            }
        }

        public Quaternion Orientation
        {
            get
            {
                return mOrientation;
            }
            set
            {
                if (mOrientation != value)
                {
                    mOrientation = value;
                    if (null != mGraphicModel)
                    {
                        mGraphicModel.Orientation = mDesc.GraphicModel.Orientation * mOrientation;
                    }
                    OnOrientationChanged();
                }
            }
        }

        protected OctreeSceneObjectDesc Desc
        {
            get
            {
                if (null == mDesc)
                {
                    throw new InvalidOperationException("It it not possible to get the object's desc because it was not yet inserted into a scene");
                }
                return mDesc;
            }
        }

        /// <summary>
        /// Valid after inserting to a scene
        /// </summary>
        protected GraphicModel GraphicModel
        {
            get 
            {
                IsInSceneCheck();

                return mGraphicModel; 
            }
        }

        public abstract void Update(AleGameTime gameTime);

        protected override void OnAddToSceneImpl(BaseScene scene)
        {
            mDesc = LoadDesc(scene.Content);

            mGraphicModel = new GraphicModel(mDesc.GraphicModel, scene.RenderableProvider, scene.Content);
            Vector3 pos = mPosition + mDesc.GraphicModel.Position;
            Quaternion orientation = mDesc.GraphicModel.Orientation * mOrientation;
            mGraphicModel.SetTransformation(ref pos, ref orientation);
            mGraphicModel.ShowWorldBounds = mShowWorldBounds;
            mGraphicModel.IsVisible = mIsVisible;
            ((OctreeScene)Scene).Octree.AddObject(mGraphicModel);
        }
        
        protected virtual void OnPositionChanged()
        {
        }

        protected virtual void OnVisibleChanged()
        {
        }

        protected virtual void OnShowWorldBoundsChanged()
        {
        }

        protected virtual void OnOrientationChanged()
        {
        }

        protected abstract OctreeSceneObjectDesc LoadDesc(ContentGroup content);

        protected override void OnDispose()
        {
            if (mGraphicModel != null)
            {
                ((OctreeScene)Scene).Octree.RemoveObject(mGraphicModel);
            }
        }

        protected override bool IsSceneValid(BaseScene scene)
        {
            return scene is OctreeScene;
        }

        protected OctreeSceneObject()
        { }
    }
}
