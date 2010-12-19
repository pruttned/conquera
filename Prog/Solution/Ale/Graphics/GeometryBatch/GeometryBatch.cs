using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// Renderable that contains multiple static graphic models batched to a single vertex and index buffer.
    /// Geometry is built automatically whenever it is not built and GeometryBatch needs to be rendered
    /// </summary>
    public sealed class GeometryBatch : Renderable, IDynamicallyLoadableObject, IDisposable
    {
        public event DynamicallyLoadableObjectAfterLoadHandler AfterLoad;
        public event EventHandler Disposing;

        #region Fields

        /// <summary>
        /// Vertice list used during building of the geometry
        /// Static member to inc speed.
        /// </summary>
        private static SimpleVertexList mVertices = new SimpleVertexList(1000);

        /// <summary>
        /// Indices list used during building of the geometry
        /// Static member to inc speed.
        /// </summary>
        private static ShortIndexList mIndices = new ShortIndexList(1000);

        /// <summary>
        /// Batched model parts sorted by render layer and material
        /// </summary>
        private Dictionary<int, Dictionary<Material, List<BatchedGraphicModelPart>>> mBatchedGraphicModelParts = new Dictionary<int, Dictionary<Material, List<BatchedGraphicModelPart>>>();

        /// <summary>
        /// Batched graphic models sorted by Id
        /// </summary>
        private IdValueCollection<BatchedGraphicModel> mBatchedGraphicModels = new IdValueCollection<BatchedGraphicModel>();

        /// <summary>
        /// Units of the GeometryBatch.
        /// </summary>
        private List<GeometryBatchUnit> mGeometryBatches = new List<GeometryBatchUnit>();

        /// <summary>
        /// Vb
        /// </summary>
        private VertexBuffer mVertexBuffer = null;

        /// <summary>
        /// Ib
        /// </summary>
        private IndexBuffer mIndexBuffer = null;

        /// <summary>
        /// Vertex stride
        /// </summary>
        private int mVertexStride;

        /// <summary>
        /// Bounding box
        /// </summary>
        private BoundingBox mBoundingBox;

        /// <summary>
        /// Whether was object already disposed
        /// </summary>
        private bool mIsDisposed = false;

        /// <summary>
        /// Vertex declaration
        /// </summary>
        private VertexDeclaration mVertexDeclaration;

        private int mVertCnt = 0;

        private long mLastRenderFrameNum  = -1;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the GraphicsDevice
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return mVertexDeclaration.GraphicsDevice; }
        }

        #region IDynamicallyLoadableObject

        public long LastRenderFrameNum
        {
            get { return mLastRenderFrameNum; }
        }

        /// <summary>
        /// Gets whether is geometry currently built
        /// </summary>
        public bool IsLoaded
        {
            get { return null != mVertexBuffer; }
        }

        public bool IsEmpty
        {
            get { return 0 == mBatchedGraphicModels.Count; }
        }

        #endregion IDynamicallyLoadableObject

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public GeometryBatch(GraphicsDevice graphicsDevice)
            : base(new BoundingSphere(), false) //bounds are unknown
        {
            mVertexDeclaration = new VertexDeclaration(graphicsDevice, SimpleVertex.VertexElements);
            mVertexStride = mVertexDeclaration.GetVertexStrideSize(0);
        }

        public bool HasRoomFor(int vertCnt)
        {
            return (Int16.MaxValue >= mVertCnt + vertCnt);
        }

        /// <summary>
        /// Adds a graphic model to the batch. 
        /// Elements of the graphic model are copied. Therefore changing them in the graphic model after it has been
        /// inserted into the batch will not affect the batch. You must remove and insert the model from the batch if you want to change batched graphic model.
        /// It is possible to use one GraphicModel instance to add multiple batched graphic models (set transformation or materials and call AddGraphicModel, then again
        /// set its transformation or materials and again call AddGraphicModel, ...).
        /// 
        /// Calling this method will invalidate the geometry and therefore it will be released and built again when it is rendered. If you need to 
        /// edit the geometry, then make many changes at once before drawing the geometry to minimise calls of the ReleaseGeometry and BuildGeometry.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Id</returns>
        public int AddGraphicModel(GraphicModel graphicModel)
        {
            int mehVertCnt = graphicModel.Mesh.Vertices.Count;
            if (!HasRoomFor(mehVertCnt))
            {
                throw new ArgumentException("Not enough free space in batch to hold model's vertices");
            }
            mVertCnt += mehVertCnt;

            Unload(); //release built geometry

            BatchedGraphicModel batchedGraphicModel = new BatchedGraphicModel(graphicModel);

            //update bounds
            BoundingSphere worldBounds = graphicModel.WorldBounds;
            if (0 == mBatchedGraphicModels.Count) //first renderable
            {
                BoundingBox.CreateFromSphere(ref worldBounds, out mBoundingBox);
                Bounds = BoundingSphere.CreateFromBoundingBox(mBoundingBox);
            }
            else
            {
                BoundingBox renderableBb = BoundingBox.CreateFromSphere(worldBounds);
                BoundingBox.CreateMerged(ref mBoundingBox, ref renderableBb, out mBoundingBox);

                Bounds = BoundingSphere.CreateFromBoundingBox(mBoundingBox);
            }

            int id = mBatchedGraphicModels.Add(batchedGraphicModel);

            //Parts
            for (int i = 0; i < batchedGraphicModel.Parts.Count; ++i)
            {
                BatchedGraphicModelPart part = batchedGraphicModel.Parts[i];
                //Get Material-Geometry Dictionary
                Dictionary<Material, List<BatchedGraphicModelPart>> materialPartDict;
                if (!mBatchedGraphicModelParts.TryGetValue(part.Material.RenderLayer, out materialPartDict))
                {
                    materialPartDict = new Dictionary<Material, List<BatchedGraphicModelPart>>();
                    mBatchedGraphicModelParts.Add(part.Material.RenderLayer, materialPartDict);
                }

                //Get Geometry list
                List<BatchedGraphicModelPart> partList;
                if (!materialPartDict.TryGetValue(part.Material, out partList))
                {
                    partList = new List<BatchedGraphicModelPart>();
                    materialPartDict.Add(part.Material, partList);
                }

                partList.Add(part);
            }

            return id;
        }

        /// <summary>
        /// Removes the geometry unit.
        /// 
        /// Calling this method will invalidate the geometry and therefore it will be released and built again when it is rendered. If you need to 
        /// edit the geometry, then make many changes at once before drawing the geometry to minimise calls of the ReleaseGeometry and BuildGeometry.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Whether it was found and removed</returns>
        public bool RemoveGeometryUnit(int id)
        {
            BatchedGraphicModel removedModel = mBatchedGraphicModels.Remove(id);
            if (null != removedModel)
            {
                int mehVertCnt = removedModel.Mesh.Vertices.Count;
                mVertCnt -= mehVertCnt;

                Unload(); //release built geometry

                //parts
                for(int i = 0; i< removedModel.Parts.Count; ++i)
                {
                    BatchedGraphicModelPart part = removedModel.Parts[i];
                    mBatchedGraphicModelParts[part.Material.RenderLayer][part.Material].Remove(part);
                }

                //bounds (BoundingBox -> BoundingSphere produces better results then BoundingSphere.CreateMerged or BoundingSphereBoundingSphere.CreateFromPoints)
                bool first = true;
                foreach (BatchedGraphicModel model in mBatchedGraphicModels)
                {
                    BoundingSphere worldBounds;
                    model.ComputeWorldBounds(out worldBounds);

                    if (first)
                    {
                        mBoundingBox = BoundingBox.CreateFromSphere(worldBounds);
                        first = false;
                    }
                    else
                    {
                        BoundingBox renderableBb = BoundingBox.CreateFromSphere(worldBounds);
                        BoundingBox.CreateMerged(ref mBoundingBox, ref renderableBb, out mBoundingBox);
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Builds geometry batch from currently stored geometries. 
        /// Subsequent calls without calling ReleaseGeometry are ignored.
        /// </summary>
        public void Load()
        {
            if (!IsEmpty)
            {
                if (!IsLoaded)
                {
                    foreach (Dictionary<Material, List<BatchedGraphicModelPart>> materialPartDict in mBatchedGraphicModelParts.Values)
                    {
                        foreach (List<BatchedGraphicModelPart> renderableUnitList in materialPartDict.Values)
                        {
                            if (0 < renderableUnitList.Count)
                            {
                                int baseVertex = mVertices.Count;
                                int baseIndex = mIndices.Count;
                                Material material = renderableUnitList[0].Material;

                                int indicesCnt = 0;
                                int verticesCnt = 0;

                                //int unitBaseIndex = 0;

                                int renderableUnitCnt = renderableUnitList.Count;
                                for (int i = 0; i < renderableUnitCnt; ++i)
                                {
                                    BatchedGraphicModelPart part = renderableUnitList[i];
                                    if (part.BatchedGraphicModel.OnlyTranslation)
                                    {
                                        mVertices.AddRange(part.BatchedGraphicModel.Mesh.Vertices.Items, part.MeshPart.BaseVertex, part.MeshPart.NumVertices, part.BatchedGraphicModel.WorldTranslation);
                                    }
                                    else
                                    {
                                        Matrix worldTransformation;
                                        part.BatchedGraphicModel.ComputeWorldTransformation(out worldTransformation);
                                        mVertices.AddRange(part.BatchedGraphicModel.Mesh.Vertices.Items, part.MeshPart.BaseVertex, part.MeshPart.NumVertices, ref worldTransformation);
                                    }

                                    mIndices.AddRange(part.BatchedGraphicModel.Mesh.Indices.Items, part.MeshPart.StartIndex, part.MeshPart.PrimitiveCnt * 3, verticesCnt);

                                    indicesCnt += part.MeshPart.PrimitiveCnt * 3;
                                    verticesCnt += part.MeshPart.NumVertices;
                                }

                                mGeometryBatches.Add(new GeometryBatchUnit(this, material, baseVertex,
                                        verticesCnt, indicesCnt / 3, baseIndex));
                            }
                        }
                    }

                    mVertexBuffer = mVertices.CreateVertexBuffer(GraphicsDevice, true);
                    mIndexBuffer = mIndices.CreateIndexBuffer(GraphicsDevice, true);

                    mVertices.Clear();
                    mIndices.Clear();

                    if (null != AfterLoad)
                    {
                        AfterLoad.Invoke(this);
                    }
                }
            }
        }

        #region IDynamicallyLoadableObject

        /// <summary>
        /// Relesase geometry that was built by BuildGeometry method. This method doesn't destroy the geometry batch and its
        /// geometry can be built again by calling BuildGeometry. Only VertexBuffer and IndexBuffer are released to save memory.
        /// 
        /// Geometry is automatically built again when it is going to be rendered
        /// </summary>
        public void Unload()
        {
            if (IsLoaded)
            {
                mGeometryBatches.Clear();

                mVertexBuffer.Dispose();
                mVertexBuffer = null;

                mIndexBuffer.Dispose();
                mIndexBuffer = null;
            }
        }
        #endregion IDynamicallyLoadableObject

        #region IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                Unload();

                mBatchedGraphicModelParts.Clear();
                mBatchedGraphicModels.Clear();

                if (null != Disposing)
                {
                    Disposing.Invoke(this, null);
                }

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable

        /// <summary>
        /// 
        /// </summary>
        internal void PrepareForRender()
        {
            GraphicsDevice.VertexDeclaration = mVertexDeclaration;
            GraphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, mVertexStride);
            GraphicsDevice.Indices = mIndexBuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            if (!IsLoaded)
            {
                Load();
            }
            
            mLastRenderFrameNum = gameTime.FrameNum;

            for (int i = 0; i < mGeometryBatches.Count; ++i)
            {
                renderer.EnqueueRenderable(mGeometryBatches[i]);
            }
        }

        #endregion Methods
    }
}