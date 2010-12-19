using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Ale.Graphics;

namespace Ale.Scene
{
    /// <summary>
    /// Gets all objects that are in a given camera's frustum
    /// </summary>
    public class CameraFrustumOctreeObjectNodeFilter : IOctreeObjectNodeFilter
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private ICamera mCamera;
        
        #endregion Fields

        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        public ICamera Camera
        {
            get { return mCamera; }
            set { mCamera = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public CameraFrustumOctreeObjectNodeFilter()
        {
            mCamera = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public CameraFrustumOctreeObjectNodeFilter(ICamera camera)
        {
            Camera = camera;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeNode"></param>
        /// <returns></returns>
        public NodeFilterResult CheckNode(OctreeSceneNode octreeNode)
        {
            switch (Camera.IsInSightEx(octreeNode.Bounds))
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
            return (Camera.IsInSight(octreeObject.WorldBounds));
        }
        
        #endregion Methods
    }
}

