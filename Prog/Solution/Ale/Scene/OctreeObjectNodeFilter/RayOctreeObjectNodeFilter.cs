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
