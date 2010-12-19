using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Skeletal animation
    /// </summary>
    public class SkeletalAnimation
    {
        #region Properties

        /// <summary>
        /// Name
        /// </summary>
        public NameId Name { get; private set; }

        /// <summary>
        /// Gets the duration in sec
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// Gets the animation channels
        /// </summary>
        public SkeletalAnimationChannelCollection Channels { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="duration"></param>
        /// <param name="channels">channel list that will be wrapped by this class</param>
        public SkeletalAnimation(NameId name, float duration, List<SkeletalAnimationChannel> channels)
        {
            Name = name;
            Duration = duration;
            Channels = new SkeletalAnimationChannelCollection(channels);
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="input"></param>
        internal SkeletalAnimation(ContentReader input)
        {
            Name = input.ReadString();
            Duration = input.ReadSingle();
            Channels = new SkeletalAnimationChannelCollection(input);
        }

        #endregion Methods
    }
}
