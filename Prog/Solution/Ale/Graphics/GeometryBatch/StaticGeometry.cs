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
using Ale.Tools;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Ale.Scene;

namespace Ale.Graphics
{
    /// <summary>
    /// Used for batching many graphic models to bigger vertex and index buffers. 
    /// Batched models are static (no skinning animation is possible).
    /// It can be used for instance for batching many trees or grass.
    ///
    /// Static geometry is divided to geometry batches that are loaded (bulding of its vb and ib) and unloaded dynamically according to view frustum of a 
    /// given camera
    /// </summary>
    public sealed class StaticGeometry : IFrameListener, IDisposable
    {
        private DynamicallyLoadableObjectsDistanceUnloader mDynamicallyLoadableObjectsDistanceUnloader;

        private GraphicsDeviceManager mGraphicsDeviceManager;
        
        //private List<List<List<List<GeometryBatch>>>> mGeometryBatches = new List<List<List<List<GeometryBatch>>>>();
        private Dictionary<Point3D, List<GeometryBatch>> mGeometryBatches = new Dictionary<Point3D, List<GeometryBatch>>();
        
        bool mShowWorldBounds = false;

        /// <summary>
        /// Flat list of geometr batches
        /// </summary>
        private List<GeometryBatch> mGeometryBatchesFlat = new List<GeometryBatch>();

        private float mBatchCellSize;

        private Octree mOctree;

        private bool mIsDisposed = false;

		public GraphicsDeviceManager GraphicsDeviceManager 
		{
			get { return mGraphicsDeviceManager; }
		}

		/// <summary>
        /// Main scene camera for batch dynamic loading/unloading
        /// </summary>
        public ICamera MainCamera
        {
            get { return mDynamicallyLoadableObjectsDistanceUnloader.MainCamera; }
            set { mDynamicallyLoadableObjectsDistanceUnloader.MainCamera = value; }
        }

        /// <summary>
        /// Gets/Sets whether should be world bounds rendered
        /// </summary>
        public bool ShowWorldBounds
        {
            get { return mShowWorldBounds; }
            set 
            { 
                mShowWorldBounds = value;
                foreach (GeometryBatch batch in mGeometryBatchesFlat)
                {
                    batch.ShowWorldBounds = mShowWorldBounds;
                }
            }
        }


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicsDeviceManager"></param>
        /// <param name="octree">- Octree for querying geometry batches</param>
        /// <param name="mainCamera">- Camera used for loading and unloading of geometry batches</param>
        /// <param name="batchCellSize">- Approximated size of one geometry batch that is loaded (bulding of its vb and ib) and unloaded dynamically according to view frustum of a 
        /// given camera (Approximated - Model is inserted to a given batch when its bounding center lies in batch bounds (given by batchCellSize) and therefore the real size of the batch may be different</param>
        public StaticGeometry(GraphicsDeviceManager graphicsDeviceManager, Octree octree, ICamera mainCamera, float batchCellSize)
        {
            if (graphicsDeviceManager == null) throw new ArgumentNullException("graphicsDeviceManager");
            if (octree == null) throw new ArgumentNullException("octree");
            if (mainCamera == null) throw new ArgumentNullException("mainCamera");

            mDynamicallyLoadableObjectsDistanceUnloader = new DynamicallyLoadableObjectsDistanceUnloader(3);

            mGraphicsDeviceManager = graphicsDeviceManager;
            mBatchCellSize = batchCellSize;
            MainCamera = mainCamera;
            mOctree = octree;
        }

        /// <summary>
        /// Adds a graphic model to the batch. 
        /// Elements of the graphic model are copied. Therefore changing them in the graphic model after it has been
        /// inserted into the static geometry will not affect the geometry. You must remove and insert the model from the batch if you want to change batched graphic model.
        /// It is possible to use one GraphicModel instance to add multiple batched graphic models (set transformation or materials and call AddGraphicModel, then again
        /// set its transformation or materials and again call AddGraphicModel, ...).
        /// 
        /// Calling this method will invalidate the geometry and therefore it will be released and built again when it is rendered. If you need to 
        /// edit the geometry, then make many changes at once before drawing the geometry.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Id</returns>
        public BathcedModelIdentifier AddGraphicModel(GraphicModel model)
        {
        	if (model == null) throw new ArgumentNullException("model");

            Vector3 modelCenter = model.Bounds.Center;

            Matrix transf = model.WorldTransformation;
            Vector3.Transform(ref modelCenter, ref transf, out modelCenter);

            int bX = (int)(modelCenter.X / mBatchCellSize);
            int bY = (int)(modelCenter.Y / mBatchCellSize);
            int bZ = (int)(modelCenter.Z / mBatchCellSize);



            GeometryBatch batch = GetGeometryBatch(bX, bY, bZ, model.Mesh.Vertices.Count);

            int idInBatch = batch.AddGraphicModel(model);

            BathcedModelIdentifier modelIdentifier = new BathcedModelIdentifier(this, batch, idInBatch);

            return modelIdentifier;
        }

        /// <summary>
        /// Removes a given model from the geometry
        /// 
        /// Calling this method will invalidate the geometry and therefore it will be released and built again when it is rendered. If you need to 
        /// edit the geometry, then make many changes at once before drawing the geometry.
        /// </summary>
        /// <param name="model"></param>
        public void RemoveGraphicModel(BathcedModelIdentifier model)
        {
        	if (model == null) throw new ArgumentNullException("model");
    
            if (this != model.StaticGeometry)
            {
                throw new ArgumentException("Model doesn't belong to this static geometry");
            }

            model.Batch.RemoveGeometryUnit(model.IdInBatch);
        }

        #region IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                foreach (GeometryBatch batch in mGeometryBatchesFlat)
                {
                    mOctree.RemoveObject(batch);
                    batch.Dispose();
                }

                mGeometryBatchesFlat.Clear();
                mGeometryBatches.Clear();

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable


        #region IFrameListener Members

        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).BeforeUpdate(gameTime);
        }

        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).AfterUpdate(gameTime);
        }

        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).BeforeRender(gameTime);
        }

        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).AfterRender(gameTime);
        }

        #endregion

        private GeometryBatch GetGeometryBatch(int x, int y, int z, int vertCnt)
        {
            if (vertCnt > Int16.MaxValue)
            {
                throw new ArgumentException("Model has to many vertices to be stored in a static geometry");
            }

            //while (mGeometryBatches.Count <= x)
            //{
            //    mGeometryBatches.Add(null);
            //}
            //List<List<List<GeometryBatch>>> xb = mGeometryBatches[x];
            //if (null == xb)
            //{
            //    mGeometryBatches[x] = xb = new List<List<List<GeometryBatch>>>();
            //}

            //while (xb.Count <= y)
            //{
            //    xb.Add(null);
            //}
            //List<List<GeometryBatch>> yb = xb[y];
            //if (null == yb)
            //{
            //    xb[y] = yb = new List<List<GeometryBatch>>();
            //}

            //while (yb.Count <= z)
            //{
            //    yb.Add(null);
            //}
            //List<GeometryBatch> batches = yb[z];
            //if (null == batches)
            //{
            //    yb[z] = batches = new List<GeometryBatch>();
            //}

            List<GeometryBatch> batchList;
            Point3D bIndex = new Point3D(x, y, z);
            if (!mGeometryBatches.TryGetValue(bIndex, out batchList))
            {
                batchList = new List<GeometryBatch>();
                mGeometryBatches.Add(bIndex, batchList);
            }
            GeometryBatch batch = null;
            for (int i = 0; i < batchList.Count; ++i)
            {
                batch = batchList[i];
                if (batch.HasRoomFor(vertCnt))
                {
                    return batch;
                }
            }

            batch = new GeometryBatch(mGraphicsDeviceManager.GraphicsDevice);
            
            //batch.WorldPosition = ;
            mOctree.AddObject(batch);

            mDynamicallyLoadableObjectsDistanceUnloader.RegisterObject(batch);
            batchList.Add(batch);

            batch.ShowWorldBounds = mShowWorldBounds;

            mGeometryBatchesFlat.Add(batch);

            return batch;
        }
    }
}
