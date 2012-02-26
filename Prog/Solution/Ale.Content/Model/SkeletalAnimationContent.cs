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
using System.Xml;
using System.Globalization;

namespace Ale.Content
{
    public class SkeletalAnimationContent
    {
        private string mName;
        private float mDuration;
        private float mDefaultSpeed;
        private List<SkeletalAnimationChannelContent> mChannels;

        public string Name
        {
            get { return mName; }
        }
        public float Duration
        {
            get { return mDuration; }
        }
        public float DefaultSpeed
        {
            get { return mDefaultSpeed; }
        }

        public List<SkeletalAnimationChannelContent>  Channels
        {
            get { return mChannels; }
        }

        public SkeletalAnimationContent(XmlNode animNode)
        {
            mDuration = float.Parse(animNode.Attributes["duration"].Value, CultureInfo.InvariantCulture);
            var defaultSpeedAtt = animNode.Attributes["defaultSpeed"];
            if (null != defaultSpeedAtt)
            {
                mDefaultSpeed = float.Parse(defaultSpeedAtt.Value, CultureInfo.InvariantCulture);
            }
            else
            {
                mDefaultSpeed = 1.0f;
            }
            mName = animNode.Attributes["name"].Value;
            mChannels = new List<SkeletalAnimationChannelContent>();
            foreach (XmlNode channelNode in animNode.SelectNodes("./channel"))
            {
                mChannels.Add(new SkeletalAnimationChannelContent(channelNode));
            }
        }
    }

}
