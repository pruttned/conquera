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

        public float DefaultSpeed { get; private set; }

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
        /// <param name="defaultSpeed"></param>
        /// <param name="channels">channel list that will be wrapped by this class</param>
        public SkeletalAnimation(NameId name, float duration, float defaultSpeed, List<SkeletalAnimationChannel> channels)
        {
            Name = name;
            Duration = duration;
            DefaultSpeed = defaultSpeed;
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
            DefaultSpeed = input.ReadSingle();
            Channels = new SkeletalAnimationChannelCollection(input);
        }

        #endregion Methods
    }
}
