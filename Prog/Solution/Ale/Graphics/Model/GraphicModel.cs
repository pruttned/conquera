using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SimpleOrmFramework;
using Ale.Tools;
using Ale.Content;

namespace Ale.Graphics
{
    public class GraphicModel : Renderable, IDisposable
    {
        #region Fields

        private Mesh mMesh;

        private GraphicModelPartCollection mGraphicModelParts;

        private SkeletalAnimationPlayer mSkeletalAnimationPlayer = null;

        private long mLastUpdateFrame = -1;
        private Matrix[] mSkinTransformations = null;

        private float mBoundsMultiplicator = 1.0f;

        private List<GraphicModelConnectionPoint> mConnectionPoints = null;

        private bool mIsDisposed = false;

        #endregion Fields

        #region Properties

        public GraphicModelPartCollection GraphicModelParts
        {
            get { return mGraphicModelParts; }
        }

        public Mesh Mesh
        {
            get { return mMesh; }
        }

        public SkeletalAnimationPlayer AnimationPlayer
        {
            get 
            {
                if (null == mSkeletalAnimationPlayer)
                {
                     throw new InvalidOperationException("Mesh hasn't any animation");
                }
                return mSkeletalAnimationPlayer; 
            }
        }

        /// <summary>
        /// Actual skin transformations.
        /// </summary>
        internal Matrix[] SkinTransformations
        {
            get { return mSkinTransformations; }
        }

        /// <summary>
        /// Multiplicator applied to a mesh bounds
        /// </summary>
        public float BoundsMultiplicator
        {
            get { return mBoundsMultiplicator; }
            set 
            { 
                mBoundsMultiplicator = value;
                BoundingSphere bounds = mMesh.Bounds;
                bounds.Radius *= mBoundsMultiplicator;
                Bounds = bounds;
            }
        }

        #endregion Properties

        #region Methods

        public GraphicModel(Mesh mesh, IList<Material> partMaterials)
            :base(mesh.Bounds, true)
        {
            mMesh = mesh;
            Init(mesh, partMaterials);
        }

        /// <summary>
        /// Create a new graphic model with a specified material for all submeshes
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="partMaterials"></param>
        public GraphicModel(Mesh mesh, Material partMaterial)
            : base(mesh.Bounds, true)
        {
            mMesh = mesh;
            Material[] partMaterials = new Material[mesh.MeshParts.Count];
            for (int i = 0; i < partMaterials.Length; ++i)
            {
                partMaterials[i] = partMaterial;
            }
            Init(mesh, partMaterials);
        }

        /// <summary>
        /// Create a new graphic model with a materials assigned to material groups
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="partMaterials"></param>
        public GraphicModel(Mesh mesh, IDictionary<NameId, Material> partMaterials)
            : base(mesh.Bounds, true)
        {
            mMesh = mesh;
            Material[] partMaterialsList = MaterialDictToList(partMaterials);
            Init(mesh, partMaterialsList);
        }

        public GraphicModel(GraphicModelDesc desc, RenderableProvider renderableProvider, ContentGroup content)
            :this(desc.Mesh,desc.PartMaterials) 
        {
            BoundsMultiplicator = desc.BoundsMultiplicator;

            Quaternion orientation = desc.Orientation;
            Vector3 position = desc.Position;
            SetTransformation(ref position, ref orientation, desc.Scale);

            //child renderables
            foreach (ConnectionPointAssigmentDesc cpAssigment in desc.ConnectionPointAssigments)
            {
                Renderable renderable = renderableProvider.CreateRenderable(cpAssigment.RenderableFactory, cpAssigment.Renderable,
                    content);
                GetConnectionPoint(cpAssigment.ConnectionPoint).AddRenderable(renderable);
            }
        }

        /// <summary>
        /// Doesn't load child renderables
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="content"></param>
        public GraphicModel(GraphicModelDesc desc, ContentGroup content)
            : this(desc.Mesh, desc.PartMaterials)
        {
            BoundsMultiplicator = desc.BoundsMultiplicator;

            Quaternion orientation = desc.Orientation;
            Vector3 position = desc.Position;
            SetTransformation(ref position, ref orientation, desc.Scale);
        }

        public GraphicModelConnectionPoint GetConnectionPoint(NameId connectionPointName)
        {
            var cp = TryGetConnectionPoint(connectionPointName);
            if (null == cp)
            {
                throw new ArgumentException(string.Format("Connection point '{0}' doesn't exists", connectionPointName));
            }

            return cp;
        }


        /// <summary>
        /// Gets a connection point or null
        /// </summary>
        /// <param name="connectionPointName"></param>
        /// <returns></returns>
        public GraphicModelConnectionPoint TryGetConnectionPoint(NameId connectionPointName)
        {
            if (null == mConnectionPoints)
            {
                return TryAddGraphicModelConnectionPoint(connectionPointName);
            }

            foreach(GraphicModelConnectionPoint connectionPoint in mConnectionPoints)
            {
                if (connectionPoint.MeshConnectionPoint.Name == connectionPointName)
                {
                    return connectionPoint;
                }
            }

            return TryAddGraphicModelConnectionPoint(connectionPointName);
        }

        /// <summary>
        /// Sets a given material in all submeshes
        /// </summary>
        /// <param name="partMaterial"></param>
        public void SetMaterials(Material partMaterial)
        {
            Material[] partMaterials = new Material[Mesh.MeshParts.Count];
            for (int i = 0; i < partMaterials.Length; ++i)
            {
                partMaterials[i] = partMaterial;
            }
            SetMaterials(partMaterials);
        }

        /// <summary>
        /// Sets materials in submeshes by material groups
        /// </summary>
        /// <param name="partMaterials"></param>
        public void SetMaterials(IDictionary<NameId, Material> partMaterials)
        {
            SetMaterials(MaterialDictToList(partMaterials));
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
                    if (null != mConnectionPoints)
                    {
                        foreach (GraphicModelConnectionPoint connectionPoint in mConnectionPoints)
                        {
                            foreach (Renderable renderable in connectionPoint)
                            {
                                if (renderable is IDisposable)
                                {
                                    ((IDisposable)renderable).Dispose();
                                }
                            }
                        }
                    }
                }
                mIsDisposed = true;
            }
        }

        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            for (int i = 0; i < mGraphicModelParts.Count; ++i)
            {
                renderer.EnqueueRenderable(mGraphicModelParts[i]);
            }

            //not yet updated in this frame and has a skeletal animation playing
            if (null != mSkeletalAnimationPlayer)
            {
                if (mLastUpdateFrame != gameTime.FrameNum && SkeletalAnimationPlayer.AnimState.Stopped != mSkeletalAnimationPlayer.AnimationState)
                {
                    mSkeletalAnimationPlayer.Update(gameTime, mSkinTransformations);

                    UpdateAttachedRenderables();
                }
                mLastUpdateFrame = gameTime.FrameNum;
            }

            //connection points
            if (null != mConnectionPoints)
            {
                for (int i = 0; i < mConnectionPoints.Count; ++i)
                {
                    mConnectionPoints[i].EnqueRenderableUnits(renderer, gameTime);
                }
            }
        }

        protected override void OnWorldBoundsChanged()
        {
            //update skinning matrices
            if (null != Mesh) // null != Mesh - in Renderable ctor is Mesh not yet initialized
            {
                if (null != mSkeletalAnimationPlayer)
                {
                    if (mSkeletalAnimationPlayer.AnimationState == SkeletalAnimationPlayer.AnimState.Stopped) //otherwise will be updated during OnEnqueRenderableUnits
                    {
                        mSkeletalAnimationPlayer.UpdateSkinTransformations(mSkinTransformations);
                    }
                }

            }

            UpdateAttachedRenderables();
        }

        private void UpdateAttachedRenderables()
        {
            if (null != mConnectionPoints)
            {
                for (int i = 0; i < mConnectionPoints.Count; ++i)
                {
                    mConnectionPoints[i].UpdateWorldTransformation();
                }
            }
        }

        /// <summary>
        /// Sets a given materials in all submeshes
        /// </summary>
        /// <param name="partMaterial"></param>
        private void SetMaterials(Material[] partMaterials)
        {
            if (partMaterials.Length != Mesh.MeshParts.Count)
            {
                throw new ArgumentException("Number of part materials is different then the number of mesh parts in the provided mesh");
            }

            for (int i = 0; i < Mesh.MeshParts.Count; ++i)
            {
                mGraphicModelParts[i].Material = partMaterials[i];
            }
        }

        private Material[] MaterialDictToList(IDictionary<NameId, Material> partMaterials)
        {
            Material[] partMaterialsList = new Material[Mesh.MeshParts.Count];
            for (int i = 0; i < partMaterialsList.Length; ++i)
            {
                NameId materialGroup = Mesh.MeshParts[i].MaterialGroup;
                Material partMaterial;
                if (!partMaterials.TryGetValue(materialGroup, out partMaterial))
                {
                    throw new ArgumentException(string.Format("Missing material assignment for material group '{0}'", materialGroup));
                }
                partMaterialsList[i] = partMaterial;
            }

            return partMaterialsList;
        }

        private void Init(Mesh mesh, IList<Material> partMaterials)
        {
            if (partMaterials.Count != mesh.MeshParts.Count)
            {
                throw new ArgumentException("Number of part materials is different then the number of mesh parts in the provided mesh");
            }

            GraphicModelPart[] graphicModelParts = new GraphicModelPart[mesh.MeshParts.Count];
            for (int i = 0; i < mesh.MeshParts.Count; ++i)
            {
                graphicModelParts[i] = new GraphicModelPart(this, mesh.MeshParts[i], partMaterials[i]);
            }
            mGraphicModelParts = new GraphicModelPartCollection(graphicModelParts);

            //No lazy loading here - mSkinTransformations can't be null because it is not possible to set null value in effect
            if (null != Mesh.SkeletalAnimations && 0 != Mesh.SkeletalAnimations.Count)
            {
                //Skin transformations
                mSkinTransformations = new Matrix[Mesh.Skeleton.BoneCnt];
                mSkeletalAnimationPlayer = new SkeletalAnimationPlayer(this, Mesh.SkeletalAnimations[0].Name);
                mSkeletalAnimationPlayer.UpdateSkinTransformations(mSkinTransformations);
            }
        }

        private GraphicModelConnectionPoint TryAddGraphicModelConnectionPoint(NameId connectionPointName)
        {
            MeshConnectionPoint meshConnectionPoint;
            if (!Mesh.ConnectionPoints.TryGetValue(connectionPointName, out meshConnectionPoint))
            {
                return null;
            }

            if (null == mConnectionPoints)
            {
                mConnectionPoints = new List<GraphicModelConnectionPoint>();
            }

            GraphicModelConnectionPoint connectionPoint = new GraphicModelConnectionPoint(this, meshConnectionPoint);

            mConnectionPoints.Add(connectionPoint);

            return connectionPoint;
        }

        #endregion Methods
    }
}
