using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    internal class ParticleToCameraDistanceComparer : IComparer<Particle>
    {
        private Vector3 mCameraPosition;

        public Vector3 CameraPosition
        {
            get { return mCameraPosition; }
            set { mCameraPosition = value; }
        }

        #region IComparer

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>
        /// 0 - equals
        /// 1 - p1 is less then  p2
        /// -1 - p1 is greater then p2
        /// </returns>
        public int Compare(Particle p1, Particle p2)
        {
            if (null == p1)
            {
                if (null == p2)
                {
                    return 0;
                }
                return 1;
            }
            if (null == p2)
            {
                return -1;
            }

            float dist1, dist2;
            Vector3.DistanceSquared(ref mCameraPosition, ref p1.Position, out dist1);
            Vector3.DistanceSquared(ref mCameraPosition, ref p2.Position, out dist2);
            return -Comparer<float>.Default.Compare(dist1, dist2);
        }

        #endregion IComparer
    }
}
