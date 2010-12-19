using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Object that can be dynamically loaded and unloaded (its internal resources)
    /// </summary>
    public interface IDynamicallyLoadableObject
    {
        #region Events

        /// <summary>
        /// Raised whenever has been object loaded
        /// </summary>
        event DynamicallyLoadableObjectAfterLoadHandler AfterLoad;

        event EventHandler Disposing;

        #endregion Events

        #region Properties

        /// <summary>
        /// Frame when was object last rendered
        /// </summary>
        long LastRenderFrameNum
        {
            get;
        }

        /// <summary>
        /// Whether is object currently loaded
        /// </summary>
        bool IsLoaded
        {
            get;
        }

        /// <summary>
        /// World bounds of the object
        /// </summary>
        BoundingSphere WorldBounds
        {
            get;
        }

        #endregion Properties


        #region Methods

        /// <summary>
        /// Unloads the object
        /// </summary>
        void Unload();

        #endregion Methods

    }

    /// <summary>
    /// Delegate for AfterLoad event of the IDynamicallyLoadableObject
    /// </summary>
    /// <param name="dynamicallyLoadableObject"></param>
    public delegate void DynamicallyLoadableObjectAfterLoadHandler(IDynamicallyLoadableObject dynamicallyLoadableObject);

}
