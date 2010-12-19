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
        private List<SkeletalAnimationChannelContent> mChannels;

        public string Name
        {
            get { return mName; }
        }
        public float Duration
        {
            get { return mDuration; }
        }

        public List<SkeletalAnimationChannelContent>  Channels
        {
            get { return mChannels; }
        }

        public SkeletalAnimationContent(XmlNode animNode)
        {
            mDuration = float.Parse(animNode.Attributes["duration"].Value, CultureInfo.InvariantCulture);
            mName = animNode.Attributes["name"].Value;
            mChannels = new List<SkeletalAnimationChannelContent>();
            foreach (XmlNode channelNode in animNode.SelectNodes("./channel"))
            {
                mChannels.Add(new SkeletalAnimationChannelContent(channelNode));
            }
        }
    }

}
