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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Simple vertex structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SimpleVertex
    {
        #region Fields

        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Normal
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Uv
        /// </summary>
        public Vector2 Uv;

        /// <summary>
        /// Layout of the vertex structure
        /// </summary>
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0),

            new VertexElement(0, 12, VertexElementFormat.Vector3,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.Normal, 0),

            new VertexElement(0, 24, VertexElementFormat.Vector2,
                                     VertexElementMethod.Default,
                                     VertexElementUsage.TextureCoordinate, 0)
        };

        /// <summary>
        /// Size of the structure in bytes
        /// </summary>
        public const int SizeInBytes = 32;

        #endregion Fields

        #region Methods

        public SimpleVertex(Vector3 position,Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            Uv = uv;
        }

        /// <summary>
        /// Gets twhether a given vertex declaration matchs the GeometryBatchVertex structure
        /// </summary>
        /// <param name="vertexDeclaration"></param>
        /// <returns></returns>
        public static bool IsGeometryBatchVertex(VertexDeclaration vertexDeclaration)
        {
            if (vertexDeclaration.GetVertexStrideSize(0) != SizeInBytes)
            {
                return false;
            }

            VertexElement[] otherVertexElements = vertexDeclaration.GetVertexElements();
            if (otherVertexElements.Length != VertexElements.Length)
            {
                return false;
            }
            for (int i = 0; i < VertexElements.Length; ++i)
            {
                if (otherVertexElements[i] != VertexElements[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares values for equality
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(ref SimpleVertex other, float epsilon)
        {
            return (AleMathUtils.Equals(ref Position, ref other.Position, epsilon) &&
                AleMathUtils.Equals(ref Normal, ref other.Normal, epsilon) &&
                AleMathUtils.Equals(ref Uv, ref other.Uv, epsilon));
        }

        /// <summary>
        /// Compares values for equality
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(SimpleVertex other, float epsilon)
        {
            return Equals(ref other, epsilon);
        }


        /// <summary>
        /// Compares values for equality (epsion = 0.00001)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ref SimpleVertex other)
        {
            return Equals(ref other, 0.00001f);
        }

        /// <summary>
        /// Compares values for equality (epsion = 0.00001)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SimpleVertex other)
        {
            return Equals(ref other);
        }

        public override string ToString()
        {
            return string.Format("p:{0}, uv:{1}, n:{2}", Position, Uv, Normal); 
        }
        #endregion Methods
    }
}
