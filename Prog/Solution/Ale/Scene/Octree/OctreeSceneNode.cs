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
using Ale.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Scene
{
    /// <summary>
    /// Octree node
    /// </summary>
    public class OctreeSceneNode
    {
        #region Fields

        /// <summary>
        /// Maximum number of objects that can be stored in the node before it is split
        /// </summary>
        private const int MaxOctreeObjectCntBeforeSplit = 16;

        /// <summary>
        /// 
        /// </summary>
        private OctreeSceneNode[] mChildNodes = null;

        /// <summary>
        /// 
        /// </summary>
        private OctreeSceneNode mParentNode;

        /// <summary>
        /// 
        /// </summary>
        private List<IOctreeObject> mOctreeObjects = null;

        /// <summary>
        /// Bounds in world space (same as local space)
        /// </summary>
        private BoundingBox mBounds;

        /// <summary>
        /// Renderable for rendering node's bounds
        /// </summary>
        private BoundingBoxRenderable mBoundingBoxRenderable;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the node's bounds in world space (same as local space)
        /// </summary>
        public BoundingBox Bounds
        {
            get { return mBounds; }
        }

        /// <summary>
        /// Enques bounding box renderable of this node (doesn't enques childs).
        /// Empty node = white; other(has objects or childs) = DeepPink
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="renderer"></param>
        protected internal void EnqueBoundingBoxRenderable(Renderer renderer, AleGameTime gameTime)
        {
            if(null == mBoundingBoxRenderable)
            {
                mBoundingBoxRenderable = new BoundingBoxRenderable(Color.White);
                mBoundingBoxRenderable.SetBoundingBox(mBounds);
            }

            if (null == mOctreeObjects)
            {
                mBoundingBoxRenderable.Color = Color.White;
            }
            else
            {
                mBoundingBoxRenderable.Color = Color.DeepPink;
            }

            mBoundingBoxRenderable.EnqueRenderableUnits(renderer, gameTime);
        }

        /// <summary>
        /// Whether is node empty
        /// </summary>
        protected bool IsEmpty
        {
            //mOctreeObjects must be null in an empty node - see RemoveObjectFromThisNode
            get { return ((null == mOctreeObjects) && (null == mChildNodes)); }
        }

        /// <summary>
        /// Whether are all child nodes empty
        /// </summary>
        protected bool AllChildNodesAreEmpty
        {
            get
            {
                if (null == mChildNodes)
                {
                    return true;
                }

                for (int i = 0; i < 8; ++i)
                {
                    if (!mChildNodes[i].IsEmpty)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Whether is node split
        /// </summary>
        private bool IsSplit
        {
            get { return (null != mChildNodes); }
        }

        /// <summary>
        /// Whether node needs to be split
        /// </summary>
        private bool NeedSplit
        {
            get { return (null != mOctreeObjects && MaxOctreeObjectCntBeforeSplit < mOctreeObjects.Count); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Removes an object from the octree and call its Dispose method if it implements IDiposable
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        public bool DestroyObject(IOctreeObject octreeObject)
        {
            if (RemoveObject(octreeObject))
            {
                if(octreeObject is  IDisposable)
                {
                    ((IDisposable)octreeObject).Dispose();
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Removes an object from the octree
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        public virtual bool RemoveObject(IOctreeObject octreeObject)
        {
            if (null == octreeObject) { throw new NullReferenceException("octreeObject"); }

            if(!CanContain(octreeObject)) //Object can't be in this node - so don't even check this octree subtree
            {
                return false;
            }

            if (RemoveObjectFromThisNode(octreeObject))//was pressent in this node
            {
                return true;
            }
            else //check child nodes
            {
                if (null == mChildNodes)
                {
                    return false;
                }

                foreach (OctreeSceneNode childNode in mChildNodes)
                {
                    if (childNode.RemoveObject(octreeObject))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="bounds"></param>
        protected OctreeSceneNode(OctreeSceneNode parentNode, BoundingBox bounds)
        {
            mParentNode = parentNode;

            mBounds = bounds;

            mBoundingBoxRenderable = null;
        }

        /// <summary>
        /// Queries all objects that matches a given filter
        /// </summary>
        /// <param name="queriedObjects">- list to be filled</param>
        /// <param name="octreeObjectNodeFilter"></param>
        /// <param name="octreeFilterStatistics"></param>
        protected void QueryObjects(List<IOctreeObject> queriedObjects, IOctreeObjectNodeFilter octreeObjectNodeFilter, ref OctreeFilterStatistics octreeFilterStatistics)
        {
            QueryObjects(queriedObjects, octreeObjectNodeFilter, true, ref octreeFilterStatistics);
        }

        /// <summary>
        /// Enques debug renderables that displays the octree
        /// Empty node = white; other(has objects or childs) = DeepPink
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        /// <param name="cameraFrustumOctreeObjectNodeFilter"></param>
        protected void EnqueDebugRenderables(Renderer renderer, AleGameTime gameTime, CameraFrustumOctreeObjectNodeFilter cameraFrustumOctreeObjectNodeFilter)
        {
            EnqueDebugRenderables(renderer, gameTime, cameraFrustumOctreeObjectNodeFilter, true);
        }


        /// <summary>
        /// Queries all objects that matches a given filter
        /// </summary>
        /// <param name="action"></param>
        /// <param name="octreeObjectNodeFilter"></param>
        /// <param name="checkIfVisible"></param>
        /// <param name="octreeFilterStatistics"></param>
        private void QueryObjects(List<IOctreeObject> queriedObjects, IOctreeObjectNodeFilter octreeObjectNodeFilter, bool checkIfVisible, ref OctreeFilterStatistics octreeFilterStatistics)
        {
            NodeFilterResult thisNodeFilterResult;
            if (checkIfVisible)
            {
                octreeFilterStatistics.IncCheckedNodeCnt();
                thisNodeFilterResult = octreeObjectNodeFilter.CheckNode(this);
            }
            else
            {
                thisNodeFilterResult = NodeFilterResult.Include;
            }

            if (NodeFilterResult.DontInclude != thisNodeFilterResult)
            {
                if (null != mOctreeObjects) //objects
                {
                    int octreeObjectsCnt = mOctreeObjects.Count;
                    if (NodeFilterResult.Include == thisNodeFilterResult) //whole node is vissible => don't check  child visibility
                    {
                        for (int i = 0; i < octreeObjectsCnt; ++i)
                        {
                            octreeFilterStatistics.IncIncludedObjectCnt();
                            queriedObjects.Add(mOctreeObjects[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < octreeObjectsCnt; ++i) //check each object
                        {
                            IOctreeObject octreeObject = mOctreeObjects[i];
                            if (octreeObject.IsVisible)
                            {
                                octreeFilterStatistics.IncCheckedObjectCnt();
                                if (octreeObjectNodeFilter.CheckObject(mOctreeObjects[i]))
                                {
                                    octreeFilterStatistics.IncIncludedObjectCnt();
                                    queriedObjects.Add(mOctreeObjects[i]);
                                }
                            }
                        }
                    }
                }

                if (null != mChildNodes) //child nodes
                {
                    for (int i = 0; i < mChildNodes.Length; ++i)
                    {
                        mChildNodes[i].QueryObjects(queriedObjects, octreeObjectNodeFilter, (NodeFilterResult.Include != thisNodeFilterResult), ref octreeFilterStatistics);
                    }
                }
            }
        }

        /// <summary>
        /// Enques debug renderables that displays the octree
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        /// <param name="cameraFrustumOctreeObjectNodeFilter"></param>
        /// <param name="checkIfVisible"></param>
        private void EnqueDebugRenderables(Renderer renderer, AleGameTime gameTime, CameraFrustumOctreeObjectNodeFilter cameraFrustumOctreeObjectNodeFilter, bool checkIfVisible)
        {
            NodeFilterResult thisNodeFilterResult;
            if (checkIfVisible)
            {
                thisNodeFilterResult = cameraFrustumOctreeObjectNodeFilter.CheckNode(this);
            }
            else
            {
                thisNodeFilterResult = NodeFilterResult.Include;
            }

            if (NodeFilterResult.DontInclude != thisNodeFilterResult)
            {
                EnqueBoundingBoxRenderable(renderer, gameTime);
                if (null != mChildNodes) //child nodes
                {
                    for (int i = 0; i < mChildNodes.Length; ++i)
                    {
                        mChildNodes[i].EnqueDebugRenderables(renderer, gameTime, cameraFrustumOctreeObjectNodeFilter, (NodeFilterResult.Include != thisNodeFilterResult));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        protected bool TryAddObject(IOctreeObject octreeObject)
        {
            if (!CanContain(octreeObject))
            {
                return false;
            }

            if (!IsSplit) //not yet split
            {
                AddObjectToThisNode(octreeObject);

                if (NeedSplit)
                {
                    Split();
                }
            }
            else
            {
                if (!TryAddObjectToChildNodes(octreeObject))
                {
                    AddObjectToThisNode(octreeObject);
                }
            }

            return true;
        }

        /// <summary>
        /// Called by a child node when it isn't cappable to hold some object any longer (e.g. it has been resized)
        /// </summary>
        /// <param name="octreeObject"></param>
        protected virtual void AddObjectFromChild(IOctreeObject octreeObject)
        {
            if (!TryAddObject(octreeObject))
            {
                mParentNode.AddObjectFromChild(octreeObject);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="newBounds"></param>
        //protected void Resize(BoundingBox newBounds)
        //{
        //    if (ContainmentType.Contains != newBounds.Contains(mBounds))
        //    {
        //        throw new ArgumentException("New bounds doesn't constains old bounds");
        //    }

        //    mBounds = newBounds;
        //    if (null != mBoundingBoxRenderable)
        //    {
        //        mBoundingBoxRenderable.SetBoundingBox(mBounds);
        //    }
        //    //resize childrens
        //    if (null != mChildNodes)
        //    {
        //        BoundingBox[] childBounds = ComputeChildBounds();
        //        for (int i = 0; i < 8; ++i)
        //        {
        //            mChildNodes[i].Resize(childBounds[i]);
        //        }
        //    }

        //    if (!IsSplit)
        //    {
        //        Split(); //this will call DistributeObjectsToChildrens
        //    }
        //    else
        //    {
        //        //Try to distribute objects to childrens
        //        DistributeObjectsToChildrens();
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        protected bool CanContain(ref BoundingSphere bounds)
        {
            ContainmentType containmentType;
            mBounds.Contains(ref bounds, out containmentType);
            return (ContainmentType.Contains == containmentType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        protected bool CanContain(IOctreeObject octreeObject)
        {
            return (ContainmentType.Contains == mBounds.Contains(octreeObject.WorldBounds));
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Split()
        {
            mChildNodes = new OctreeSceneNode[8];
            BoundingBox[] childBounds = ComputeChildBounds();
            for (int i = 0; i < 8; ++i)
            {
                mChildNodes[i] = new OctreeSceneNode(this, childBounds[i]);
            }

            DistributeObjectsToChildrens();

            if (AllChildNodesAreEmpty)//unable to distribute objects - remove childs
            {
                mChildNodes = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void DistributeObjectsToChildrens()
        {
            if (null != mOctreeObjects && 0 != mOctreeObjects.Count)
            {
                for (int i = mOctreeObjects.Count-1; i >=0 ; --i)
                {
                    IOctreeObject octreeObject = mOctreeObjects[i];
                    if (TryAddObjectToChildNodes(octreeObject))
                    {
                        RemoveObjectFromThisNode(i);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected BoundingBox[] ComputeChildBounds()
        {
            BoundingBox[] childBounds = new BoundingBox[8];

            Vector3 boundsMin = mBounds.Min;
            Vector3 size = mBounds.Max - boundsMin;
            Vector3 childSize = size / 2;
            Vector3 sizeOctan = size / 8;
            Vector3 halfChildSize = childSize / 2;

            Vector3 childPosition = boundsMin + sizeOctan;
            childBounds[0] = new BoundingBox(childPosition, childPosition + childSize);
            childPosition.X += halfChildSize.X;
            childBounds[1] = new BoundingBox(childPosition, childPosition + childSize);
            childPosition.Y += halfChildSize.Y;
            childBounds[2] = new BoundingBox(childPosition, childPosition + childSize);
            childPosition.X -= halfChildSize.X;
            childBounds[3] = new BoundingBox(childPosition, childPosition + childSize);
            childPosition.Z += halfChildSize.Z;
            childBounds[4] = new BoundingBox(childPosition, childPosition + childSize);
            childPosition.X += halfChildSize.X;
            childBounds[5] = new BoundingBox(childPosition, childPosition + childSize);
            childPosition.Y -= halfChildSize.Y;
            childBounds[6] = new BoundingBox(childPosition, childPosition + childSize);
            childPosition.X -= halfChildSize.X;
            childBounds[7] = new BoundingBox(childPosition, childPosition + childSize);

            return childBounds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        protected bool TryAddObjectToChildNodes(IOctreeObject octreeObject)
        {
            Vector3 objCenter = octreeObject.WorldBounds.Center;
            OctreeSceneNode closestChildNode = mChildNodes[0];
            Vector3 childHalfSize = (closestChildNode.Bounds.Max - closestChildNode.Bounds.Min)/2.0f;

            float objBoundsRadius = octreeObject.WorldBounds.Radius;
            if (childHalfSize.X < objBoundsRadius || childHalfSize.Y < objBoundsRadius)
            {
                return false;
            }

            float minDist = ((childHalfSize + closestChildNode.Bounds.Min) - objCenter).LengthSquared();
            for (int i = 1; i < 8; ++i) //find child node to which center is object closest
            {
                float dist = ((childHalfSize + mChildNodes[i].Bounds.Min) - objCenter).LengthSquared();
                if (dist < minDist)
                {
                    minDist = dist;
                    closestChildNode = mChildNodes[i];
                }
            }

            return closestChildNode.TryAddObject(octreeObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        protected bool RemoveObjectFromThisNode(IOctreeObject octreeObject)
        {
            if (null == mOctreeObjects || 0 == mOctreeObjects.Count)
            {
                return false;
            }

            int objIndex = mOctreeObjects.IndexOf(octreeObject);
            if (-1 == objIndex)
            {
                return false;
            }
            RemoveObjectFromThisNode(objIndex);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeSceneNode"></param>
        protected void OnChildNodeAbandoned(OctreeSceneNode octreeSceneNode)
        {
            if (AllChildNodesAreEmpty)
            {
                mChildNodes = null;

                if (null != mParentNode && null == mOctreeObjects)
                {
                    mParentNode.OnChildNodeAbandoned(this);
                }
            }
        }

        /// <summary>
        /// Adds object to this node (doesn't try childrens)
        /// </summary>
        /// <param name="octreeObject"></param>
        protected void AddObjectToThisNode(IOctreeObject octreeObject)
        {
            //insert to this node
            if (null == mOctreeObjects)
            {
                mOctreeObjects = new List<IOctreeObject>();
            }

            mOctreeObjects.Add(octreeObject);
            octreeObject.WorldBoundsChanged += new WorldBoundsChangedHandler(OnOctreeObjectWorldBoundsChanged);
        }

        /// <summary>
        /// Called whenever world bounds of a stored object changes
        /// </summary>
        /// <param name="octreeObject"></param>
        protected virtual void OnOctreeObjectWorldBoundsChanged(IOctreeObject octreeObject)
        {
            if (!CanContain(octreeObject))
            {
                mParentNode.AddObjectFromChild(octreeObject);
                RemoveObjectFromThisNode(octreeObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        private void RemoveObjectFromThisNode(int octreeObjectIndex)
        {
            IOctreeObject octreeObject = mOctreeObjects[octreeObjectIndex];
            mOctreeObjects.RemoveAt(octreeObjectIndex);
         
            octreeObject.WorldBoundsChanged -= OnOctreeObjectWorldBoundsChanged;

            if (0 == mOctreeObjects.Count)
            {
                mOctreeObjects = null;
            }
            if (AllChildNodesAreEmpty)
            {
                mChildNodes = null;
            }

            //check if this node should be removed
            if (IsEmpty)
            {
                if (null != mParentNode)
                {
                    mParentNode.OnChildNodeAbandoned(this);
                }
            }
        }

        #endregion Methods
    }
}
