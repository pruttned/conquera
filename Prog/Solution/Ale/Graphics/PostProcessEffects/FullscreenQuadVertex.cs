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
