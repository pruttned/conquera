//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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

namespace Ale.Graphics
{
    /// <summary>
    /// Unloads objects when they moves a specified distance away from the camera's frustum 
    /// </summary>
    public class DynamicallyLoadableObjectsDistanceUnloader : DynamicallyLoadableObjectsUnloader
    {
        #region Fields

        private ICamera mMainCamera = null;
        private float mBoundsMultiplicator;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Main scene camera for checking object distance
        /// </summary>
        public ICamera MainCamera
        {
            get { return mMainCamera; }
            set { mMainCamera = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="boundsMultiplicator">- Multiplicator that is applied to the object's bounding radius before checking its visibility for unloading purposes</param>
        public DynamicallyLoadableObjectsDistanceUnloader(float boundsMultiplicator)
            :base(2)
        {
            mBoundsMultiplicator = boundsMultiplicator;
        }

        /// <summary>
        /// Whether should be an object unloaded
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override bool ObjectCanBeUnloaded(IDynamicallyLoadableObject obj)
        {
            if (null == mMainCamera)
            {
                throw new InvalidOperationException("MainCamera is not set");
            }

            BoundingSphere bounds = obj.WorldBounds;
            bounds.Radius *= mBoundsMultiplicator;
            return !mMainCamera.IsInSight(ref bounds);
        }

        #endregion Methods
    }

}
