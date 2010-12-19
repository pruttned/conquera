using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of GeometryBatchVertex that has internal array storage
    /// </summary>
    internal interface ISimpleVertexArrayCollection
    {
        /// <summary>
        /// Gets the number of items
        /// </summary>
        int VertexCount
        {
            get;
        }

        /// <summary>
        /// Gets the internal array of vertices
        /// </summary>
        SimpleVertex[] Vertices
        {
            get;
        }
    }
}
