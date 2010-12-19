using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Scene
{
    /// <summary>
    /// Gets all objects that are in intersected by a given ray
    /// </summary>
    public class RayOctreeObjectNodeFilter : IOctreeObjectNodeFilter
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private Ray mRay;

        #endregion Fields

        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        public Ray Ray
        {
            get { return mRay; }
            set { mRay = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public RayOctreeObjectNodeFilter()
        {
            Ray = new Ray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        public RayOctreeObjectNodeFilter(Ray ray)
        {
            Ray = ray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeNode"></param>
        /// <returns></returns>
        public NodeFilterResult CheckNode(OctreeSceneNode octreeNode)
        {
            if (null != Ray.Intersects(octreeNode.Bounds))
            {
                return NodeFilterResult.IncludePartially;
            }
            else
            {
                return NodeFilterResult.DontInclude;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        public bool CheckObject(IOctreeObject octreeObject)
        {
            return (null != Ray.Intersects(octreeObject.WorldBounds));
        }

        #endregion Methods

    }
}
