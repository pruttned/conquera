using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ale.Tools;

namespace Ale.Graphics
{
    public class GraphicModelConnectionPoint : IEnumerable<Renderable>
    {
        private MeshConnectionPoint mMeshConnectionPoint;
        private List<Renderable> mRenderables = null;
        private GraphicModel mParentGraphicModel;
        private Matrix mWorldTransformation;

        public MeshConnectionPoint MeshConnectionPoint
        {
            get { return mMeshConnectionPoint; }
        }

        public GraphicModel ParentGraphicModel
        {
            get { return mParentGraphicModel; }
        }

        public int RenderableCount
        {
            get 
            {
                if (null == mRenderables)
                {
                    return 0;
                }
                return mRenderables.Count; 
            }
        }

        public Matrix WorldTransformation
        {
            get 
            {
                return mWorldTransformation;
            }
        }

        public GraphicModelConnectionPoint(GraphicModel parentGraphicModel, MeshConnectionPoint meshConnectionPoint)
        {
            mParentGraphicModel = parentGraphicModel;
            mMeshConnectionPoint = meshConnectionPoint;

            UpdateWorldTransformation();
        }

        public void EnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            if (null != mRenderables)
            {
                foreach (Renderable renderable in mRenderables)
                {
                    renderable.EnqueRenderableUnits(renderer, gameTime);
                }
            }
        }

        internal void UpdateWorldTransformation()
        {
            Matrix parentWorldTransf;
            if (mMeshConnectionPoint.HasParentBone)
            {
                SkeletonBone bone = mMeshConnectionPoint.ParentBone;
                parentWorldTransf = mParentGraphicModel.SkinTransformations[bone.Index];
            }
            else
            {
                parentWorldTransf = mParentGraphicModel.WorldTransformation;
            }

            Matrix localTranf = MeshConnectionPoint.LocTransf;

            Matrix.Multiply(ref localTranf, ref parentWorldTransf, out mWorldTransformation);

            if (null != mRenderables)
            {
                foreach (Renderable renderable in mRenderables)
                {
                    renderable.UpdateTransformation();
                }
            }
        }

        public void AddRenderable(Renderable renderable)
        {
            if (null == mRenderables)
            {
                mRenderables = new List<Renderable>();
            }
            mRenderables.Add(renderable);
            renderable.OnAttachToConnectionPoint(this);
        }

        public bool RemoveRenderable(Renderable renderable)
        {
            if (mRenderables.Remove(renderable))
            {
                renderable.OnDetachFromConnectionPoint();
                if (0 == mRenderables.Count)
                {
                    mRenderables = null;
                }
                return true;
            }
            return false;
        }

        public IEnumerator<Renderable> GetEnumerator()
        {
            if (null == mRenderables)
            {
                return EmptyListEnumerator<Renderable>.Instance;
            }
            return mRenderables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (null == mRenderables)
            {
                return EmptyListEnumerator<Renderable>.Instance;
            }
            return mRenderables.GetEnumerator();
        }
    }
}
