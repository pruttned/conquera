using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Vertex structure for the fullscreen quad
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct FullscreenQuadVertex
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Uv
        /// </summary>
        public Vector2 Uv;

        /// <summary>
        /// Layout of the vertex structure
        /// </summary>
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector2,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),


            new VertexElement(0, 8, VertexElementFormat.Vector2,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 0)
        };

        /// <summary>
        /// Size of the structure in bytes
        /// </summary>
        public const int SizeInBytes = 16;
    }
}
