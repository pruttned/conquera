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
using Ale.Graphics;

namespace Ale.Scene
{
    /// <summary>
    /// Octree
    /// </summary>
    public class Octree : OctreeSceneNode
    {
        #region Fields

        /// <summary>
        /// Maximum number of objects that are outside of the octree's bounds before it is resized
        /// </summary>
     //   private const int MaxOutsideOctreeObjectCnt = 16;

        /// <summary>
        /// Temporal storage for objects that are outside of the bounds of the octree.
        /// Whenever size of this list exceeds its maximum, then the octree is resized and objects are inserted to octree tree.
        /// </summary>
     //   private List<IOctreeObject> mOutsideOctreeObjects = new List<IOctreeObject>();

        /// <summary>
        /// Objects queried in foreach. (Promoted to member)
        /// </summary>
        private List<IOctreeObject> mQueriedObjects = new List<IOctreeObject>(100);
        #region Common filters
        
        /// <summary>
        /// 
        /// </summary>
        private CameraFrustumOctreeObjectNodeFilter mCameraFrustumOctreeObjectNodeFilter = new CameraFrustumOctreeObjectNodeFilter();

        /// <summary>
        /// 
        /// </summary>
        private FrustumOctreeObjectNodeFilter mFrustumOctreeObjectNodeFilter = new FrustumOctreeObjectNodeFilter();

        /// <summary>
        /// 
        /// </summary>
        private RayOctreeObjectNodeFilter mRayOctreeObjectNodeFilter = new RayOctreeObjectNodeFilter();

        #endregion Common filters
        
        #endregion Fields

        #region Properties

        #endregion Properties

        #region Methods
        
        ///// <summary>
        ///// 
        ///// </summary>
        //public Octree()
        //    :this(new BoundingBox(new Vector3(-1, -1, -1), Vector3.One))
        //{
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initBounds"></param>
        public Octree(BoundingBox bounds)
            :base(null, bounds)
        {
        }

        /// <summary>
        /// Add object
        /// </summary>
        /// <param name="octreeObject"></param>
        public void AddObject(IOctreeObject octreeObject)
        {
            if (null == octreeObject) { throw new NullReferenceException("octreeObject"); } 
            
            if (!TryAddObject(octreeObject))
            {
                throw new ArgumentException("Object was outside of octree's bounds");
               // AddOutsideOctreeObject(octreeObject);
            }
        }

        /// <summary>
        /// Enques debug renderables that displays the octree
        /// Empty node = white; other(has objects or childs) = DeepPink
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        public void EnqueDebugRenderables(ICamera camera, Renderer renderer, AleGameTime gameTime)
        {
            mCameraFrustumOctreeObjectNodeFilter.Camera = camera;
            EnqueDebugRenderables(renderer, gameTime, mCameraFrustumOctreeObjectNodeFilter);
        }

        /// <summary>
        /// Executes an action on all objects that matches a given filter
        /// </summary>
        /// <param name="action"></param>
        /// <param name="octreeObjectNodeFilter"></param>
        /// <returns></returns>
        public OctreeFilterStatistics ForEachObject(Action<IOctreeObject> action, IOctreeObjectNodeFilter octreeObjectNodeFilter)
        {
            OctreeFilterStatistics octreeFilterStatistics = new OctreeFilterStatistics();
            QueryObjects(mQueriedObjects, octreeObjectNodeFilter, ref octreeFilterStatistics);
            
            //objects outside of octree
            //for (int i = 0; i < mOutsideOctreeObjects.Count; ++i)
            //{
            //    octreeFilterStatistics.IncCheckedObjectCnt();
            //    if (octreeObjectNodeFilter.CheckObject(mOutsideOctreeObjects[i]))
            //    {
            //        octreeFilterStatistics.IncIncludedObjectCnt();
            //        mQueriedObjects.Add(mOutsideOctreeObjects[i]);
            //    }
            //}

            //what if action changes the octree (particle system bounds are changed in OnEnqueRenderableUnits)
            for (int i = 0; i < mQueriedObjects.Count; ++i)
            {
                action(mQueriedObjects[i]);
            }
            mQueriedObjects.Clear();

            return octreeFilterStatistics;
        }

        /// <summary>
        /// Executes an action on all objects that are in camera's view frustum
        /// </summary>
        /// <param name="action"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public OctreeFilterStatistics ForEachObject(Action<IOctreeObject> action, ICamera camera)
        {
            mCameraFrustumOctreeObjectNodeFilter.Camera = camera;
            return ForEachObject(action, mCameraFrustumOctreeObjectNodeFilter);
        }

        /// <summary>
        /// Executes an action on all objects that are in a given frustum
        /// </summary>
        /// <param name="action"></param>
        /// <param name="frustum"></param>
        /// <returns></returns>
        public OctreeFilterStatistics ForEachObject(Action<IOctreeObject> action, BoundingFrustum frustum)
        {
            mFrustumOctreeObjectNodeFilter.Frustum = frustum;
            return ForEachObject(action, mFrustumOctreeObjectNodeFilter);
        }

        /// <summary>
        /// Executes an action on all objects that are intersected by a given ray
        /// </summary>
        /// <param name="action"></param>
        /// <param name="ray"></param>
        /// <returns></returns>
        public OctreeFilterStatistics ForEachObject(Action<IOctreeObject> action, Ray ray)
        {
            mRayOctreeObjectNodeFilter.Ray = ray;
            return ForEachObject(action, mRayOctreeObjectNodeFilter);
        }

        /// <summary>
        /// Called by a child node when it isn't cappable to hold some object any longer (e.g. it has been resized)
        /// </summary>
        /// <param name="octreeObject"></param>
        protected override void AddObjectFromChild(IOctreeObject octreeObject)
        {
            AddObject(octreeObject);
        }

        /// <summary>
        /// Called whenever world bounds of a stored object changes
        /// </summary>
        /// <param name="octreeObject"></param>
        protected override void OnOctreeObjectWorldBoundsChanged(IOctreeObject octreeObject)
        {
            if (!CanContain(octreeObject))
            {
                throw new ArgumentException("Object was outside of octree's bounds");
//                RemoveObjectFromThisNode(octreeObject);
  //              AddOutsideOctreeObject(octreeObject);
            }
        }

        /// <summary>
        /// Adds object that can't be stored in octree because it is outside of its bounds
        /// </summary>
        /// <param name="octreeObject"></param>
        //private void AddOutsideOctreeObject(IOctreeObject octreeObject)
        //{
        //    //doesn't subscribes for a WorldBoundsChanged event
        //    mOutsideOctreeObjects.Add(octreeObject);

        //    if (MaxOutsideOctreeObjectCnt < mOutsideOctreeObjects.Count)
        //    {//resize the octree
        //        //compute new bounds
        //        BoundingBox oldBounds = Bounds;
        //        BoundingBox newBounds = oldBounds;
        //        foreach (Renderable outRenderable in mOutsideOctreeObjects)
        //        {
        //            BoundingBox renderableBounds = BoundingBox.CreateFromSphere(outRenderable.WorldBounds);
        //            BoundingBox.CreateMerged(ref newBounds, ref renderableBounds, out newBounds);
        //        }

        //        //keep the same center position
        //        Vector3 oldMax = oldBounds.Max;
        //        Vector3 oldMin = oldBounds.Min;
        //        Vector3 newMax = newBounds.Max;
        //        Vector3 newMin = newBounds.Min;
        //        Vector3 bInc = new Vector3(
        //            Math.Max(newMax.X - oldMax.X, Math.Abs(newMin.X - oldMin.X)),
        //            Math.Max(newMax.Y - oldMax.Y, Math.Abs(newMin.Y - oldMin.Y)),
        //            Math.Max(newMax.Z - oldMax.Z, Math.Abs(newMin.Z - oldMin.Z)));

        //        bInc = new Vector3(Math.Max(bInc.X, Math.Max(bInc.Y, bInc.Z))*2);

        //        newBounds = new BoundingBox(oldMin - bInc, oldMax + bInc);

        //        //resize
        //        Resize(newBounds);

        //        foreach (IOctreeObject outObject in mOutsideOctreeObjects)
        //        {
        //            if (!TryAddObject(outObject))
        //            {//this shouldn't happen
        //                throw new Exception("Failed to add renderable in AddOutsideOctreeObject");
        //            }
        //        }

        //        mOutsideOctreeObjects.Clear();
        //    }
        //}

        #endregion Methods
    }
}
