using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public static class AleMathUtils
    {
        public static Random Random = new Random();

        /// <summary>
        /// Gets the perpendicular vector to a specified vector
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="perp"></param>
        /// <returns></returns>
        public static void GetPerpVector(ref Vector3 vec, out Vector3 perp)
        {
            Vector3 vecAbs = new Vector3(Math.Abs(vec.X), Math.Abs(vec.Y), Math.Abs(vec.Z));
            //avoid parallel  vector
            if (vecAbs.X < vecAbs.Y)
            {
                if (vecAbs.X < vecAbs.Z)
                {
                    perp = Vector3.UnitX;
                }
                else
                {
                    perp = Vector3.UnitZ;
                }
            }
            else
            {
                if (vecAbs.Y < vecAbs.Z)
                {
                    perp = Vector3.UnitY;
                }
                else
                {
                    perp = Vector3.UnitZ;
                }
            }
            perp.Normalize();
            Vector3.Cross(ref vec, ref perp, out perp);
        }

        /// <summary>
        /// Gets the perpendicular vector to a specified vector
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 GetPerpVector(ref Vector3 vec)
        {
            Vector3 perp;
            GetPerpVector(ref vec, out perp);
            return perp;
        }

        /// <summary>
        /// Gets the perpendicular vector to a specified vector
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="perp"></param>
        /// <returns></returns>
        public static void GetPerpVector(ref Vector2 vec, out Vector2 perp)
        {
            perp = new Vector2(-vec.Y, vec.X);
        }

        /// <summary>
        /// Gets the perpendicular vector to a specified vector
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 GetPerpVector(Vector2 vec)
        {
            Vector2 perp;
            GetPerpVector(ref vec, out perp);
            return perp;
        }

        public static Vector3 GetRandomVector3(ref Vector3 vec, ref Vector3 variation)
        {
            Vector3 outVec;
            GetRandomVector3(ref vec, ref variation, out outVec);
            return outVec;
        }
        public static void GetRandomVector3(ref Vector3 vec, ref Vector3 variation, out Vector3 outVec)
        {
            outVec = new Vector3(
                GetRandomFloat(vec.X, variation.X),
                GetRandomFloat(vec.Y, variation.Y),
                GetRandomFloat(vec.Z, variation.Z));
        }

        public static Vector3 GetRandomVector3(ref Vector3 vec, float variation)
        {
            Vector3 outVec;
            GetRandomVector3(ref vec, variation, out outVec);
            return outVec;
        }

        public static void GetRandomVector3(ref Vector3 vec, float variation, out Vector3 outVec)
        {
            outVec = new Vector3(
                GetRandomFloat(vec.X, variation),
                GetRandomFloat(vec.Y, variation),
                GetRandomFloat(vec.Z, variation));
        }

        public static float GetRandomFloat(float value, float variation)
        {
            return ((float)Random.NextDouble() - 0.5f) * variation + value;
        }

        /// <summary>
        /// Get a random number arround 0
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        public static float GetRandomFloat(float variation)
        {
            return ((float)Random.NextDouble() - 0.5f) * variation;
        }

        /// <summary>
        /// Compares values for equality
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Equals(float v1, float v2, float epsilon)
        {
            return (Math.Abs(v1 - v2) < epsilon);
        }

        /// <summary>
        /// Compares values for equality (epsion = 0.00001)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Equals(float v1, float v2)
        {
            return Equals(v1, v2, 0.00001f);
        }

        /// <summary>
        /// Compares values for equality
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Equals(ref Vector2 v1, ref Vector2 v2, float epsilon)
        {
            return (Equals(v1.X, v2.X, epsilon) &&
                    Equals(v1.Y, v2.Y, epsilon));

        }

        /// <summary>
        /// Compares values for equality (epsion = 0.00001)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Equals(ref Vector2 v1, ref Vector2 v2)
        {
            return Equals(ref v1, ref v2, 0.00001f);
        }

        /// <summary>
        /// Compares values for equality
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Equals(Vector2 v1, Vector2 v2, float epsilon)
        {
            return Equals(ref v1, ref v2, epsilon);
        }

        /// <summary>
        /// Compares values for equality (epsion = 0.00001)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Equals(Vector2 v1, Vector2 v2)
        {
            return Equals(ref v1, ref v2, 0.00001f);
        }

        /// <summary>
        /// Compares values for equality
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Equals(ref Vector3 v1, ref Vector3 v2, float epsilon)
        {
            return (Equals(v1.X, v2.X, epsilon) &&
                    Equals(v1.Y, v2.Y, epsilon) &&
                    Equals(v1.Z, v2.Z, epsilon));

        }

        /// <summary>
        /// Compares values for equality (epsion = 0.00001)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Equals(ref Vector3 v1, ref Vector3 v2)
        {
            return Equals(ref v1, ref v2, 0.00001f);
        }

        /// <summary>
        /// Compares values for equality
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool Equals(Vector3 v1, Vector3 v2, float epsilon)
        {
            return Equals(ref v1, ref v2, epsilon);
        }

        /// <summary>
        /// Compares values for equality (epsion = 0.00001)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Equals(Vector3 v1, Vector3 v2)
        {
            return Equals(ref v1, ref v2, 0.00001f);
        }

        /// <summary>
        /// Transforms the bounding sphere (BoundingSphere.Transform doesn't work with rotation)
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="scale"></param>
        /// <param name="bounds"></param>
        /// <param name="outBounds"></param>
        public static void TransformBoundingSphere(ref Vector3 translation, float scale, ref BoundingSphere bounds, out BoundingSphere outBounds)
        {
            //float uniformScale = 1.0f;
            //if (Vector3.One != scale)
            //{
            //    uniformScale = (scale.X + scale.Y + scale.Z) / 3.0f;
            //}
            outBounds = new BoundingSphere(bounds.Center * scale + translation, bounds.Radius * scale);
        }

        public static void BuildTransformation(ref Vector3 postion, ref Quaternion orientation, float scale, out Matrix matrix)
        {
            if (1.0f != scale)
            {
                Matrix.CreateScale(scale, out matrix);

                if (orientation != Quaternion.Identity)
                {
                    Matrix.Transform(ref matrix, ref orientation, out matrix);
                }

                matrix.Translation = postion;
            }
            else
            {
                if (orientation != Quaternion.Identity)
                {
                    Matrix.CreateFromQuaternion(ref orientation, out matrix);

                    matrix.Translation = postion;
                }
                else
                {
                    Matrix.CreateTranslation(ref postion, out matrix);
                }
            }
        }

        public static void BuildTransformation(ref Vector3 postion, ref Quaternion orientation, out Matrix matrix)
        {
            if (orientation != Quaternion.Identity)
            {
                Matrix.CreateFromQuaternion(ref orientation, out matrix);

                matrix.Translation = postion;
            }
            else
            {
                Matrix.CreateTranslation(ref postion, out matrix);
            }
        }
    }
}
