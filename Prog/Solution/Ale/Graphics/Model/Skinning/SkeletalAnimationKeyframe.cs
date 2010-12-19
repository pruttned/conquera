using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// KEyframe of the skeletal animation
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

        #region IComparable

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

        #endregion IComparable

        #endregion Methods
    }
}
