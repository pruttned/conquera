using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Scene
{
    /// <summary>
    /// Gets all objects that are in a given frustum
    /// </summary>
    public class FrustumOctreeObjectNodeFilter : IOctreeObjectNodeFilter
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private BoundingFrustum mFrustum;
        
        #endregion Fields

        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        public BoundingFrustum Frustum
        {
            get { return mFrustum; }
            set { mFrustum = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public FrustumOctreeObjectNodeFilter()
        {
            Frustum = new BoundingFrustum(Matrix.Identity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frustum"></param>
        public FrustumOctreeObjectNodeFilter(BoundingFrustum frustum)
        {
            Frustum = frustum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeNode"></param>
        /// <returns></returns>
        public NodeFilterResult CheckNode(OctreeSceneNode octreeNode)
        {
            switch (Frustum.Contains(octreeNode.Bounds))
            {
                case ContainmentType.Contains:
                    return NodeFilterResult.Include;
                case ContainmentType.Intersects:
                    return NodeFilterResult.IncludePartially;
                default: return NodeFilterResult.DontInclude;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        public bool CheckObject(IOctreeObject octreeObject)
        {
            return (ContainmentType.Disjoint != Frustum.Contains(octreeObject.WorldBounds));
        }
        
        #endregion Methods
    }
}
