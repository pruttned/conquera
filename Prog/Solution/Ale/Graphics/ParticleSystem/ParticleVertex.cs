using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Ale.Graphics
{
    /// <summary>
    /// Custom vertex structure for drawing particles.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ParticleVertex
    {
        public Vector3 Position;

        /// <summary>
        /// Uv for bilboard computing
        /// </summary>
        public HalfVector2 NormalizedUv;
        
        public float Lerp;
        public float Seed;
        public float Rotation;
        public float Size;


        // Describe the layout of this vertex structure.
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),


            new VertexElement(0, 12, VertexElementFormat.HalfVector2,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 0),

            new VertexElement(0, 16, VertexElementFormat.Single,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 1),

            new VertexElement(0, 20, VertexElementFormat.Single ,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 2),

            new VertexElement(0, 24, VertexElementFormat.Single ,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 3),

            new VertexElement(0, 28, VertexElementFormat.Single ,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 4),
        };


        public const int SizeInBytes = 32;
    }
}
