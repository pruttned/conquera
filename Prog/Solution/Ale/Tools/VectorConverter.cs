using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Tools
{
    /// <summary>
    /// Tool class for converting vectors between byte arrays
    /// </summary>
    internal static class VectorConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static Vector3 GetVector3FromBytes(byte[] value, int startIndex)
        {
            return new Vector3(BitConverter.ToSingle(value, startIndex),
                                BitConverter.ToSingle(value, startIndex + 4),
                                BitConverter.ToSingle(value, startIndex) + 8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static Vector2 GetVector2FromBytes(byte[] value, int startIndex)
        {
            return new Vector2(BitConverter.ToSingle(value, startIndex),
                                BitConverter.ToSingle(value, startIndex + 4));
        }
    }
}
