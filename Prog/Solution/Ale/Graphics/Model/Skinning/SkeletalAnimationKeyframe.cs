//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
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
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// Keyframe of the skeletal animation
    /// </summary>
    public class SkeletalAnimationKeyframe : IComparable<SkeletalAnimationKeyframe>
    {
        #region Properties

        /// <summary>
        /// Gets the time of this keyframe in the animation
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// Gets the translation
        /// </summary>
        public Vector3 Translation { get; private set; }

        /// <summary>
        /// Gets the orientation
        /// </summary>
        public Quaternion Orientation{ get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="time"></param>
        /// <param name="translation"></param>
        /// <param name="orientation"></param>
        public SkeletalAnimationKeyframe(float time, Vector3 translation, Quaternion orientation)
        {
            Time = time;
            Translation = translation;
            Orientation = orientation;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="input"></param>
        internal SkeletalAnimationKeyframe(ContentReader input)
        {
            Time = input.ReadSingle();
            Translation = input.ReadVector3();
            Orientation = input.ReadQuaternion();
        }

        /// <summary>
        /// Compares keyframe's time
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SkeletalAnimationKeyframe other)
        {
            return Comparer<float>.Default.Compare(Time, other.Time);
        }

        #endregion Methods
    }
}
