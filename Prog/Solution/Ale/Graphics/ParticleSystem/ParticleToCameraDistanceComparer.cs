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
