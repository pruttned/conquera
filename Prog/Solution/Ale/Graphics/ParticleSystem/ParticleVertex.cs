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
