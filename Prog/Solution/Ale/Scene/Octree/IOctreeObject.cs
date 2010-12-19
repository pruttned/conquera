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
