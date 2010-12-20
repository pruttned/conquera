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
    /// Handler for WorldBoundsChanged event
    /// </summary>
    /// <param name="octreeObject"></param>
    public delegate void WorldBoundsChangedHandler(IOctreeObject octreeObject);

    /// <summary>
    /// Object that can be stored in the octree
    /// </summary>
    public interface IOctreeObject
    {
        #region Events

        /// <summary>
        /// Raised whenever the bounds in the world space has been changed
        /// </summary>
        event WorldBoundsChangedHandler WorldBoundsChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Bounds in world space
        /// </summary>
        BoundingSphere WorldBounds
        {
            get;
        }

        /// <summary>
        /// Gets whether is object visible (if it should be queried)
        /// </summary>
        bool IsVisible
        {
            get;
        }

        #endregion Properties
    }
}
