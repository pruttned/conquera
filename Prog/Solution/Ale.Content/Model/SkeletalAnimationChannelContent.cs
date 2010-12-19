using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ale.Content
{
    public class SkeletalAnimationChannelContent
    {
        #region Fields
        private string mBone;

        private List<SkeletalAnimationKeyframeContent> mKeyframes;

        #endregion Fields

        #region Properties
        public string Bone
        {
            get { return mBone; }
        }

        public List<SkeletalAnimationKeyframeContent> Keyframes
        {
            get { return mKeyframes; }
        }
        #endregion Properties

        #region Methods
        public SkeletalAnimationChannelContent(XmlNode channelNode)
        {
            mBone = channelNode.Attributes["bone"].Value;
            mKeyframes = new List<SkeletalAnimationKeyframeContent>();
            foreach (XmlNode keyframeNode in channelNode.SelectNodes("./keyframe"))
            {
                mKeyframes.Add(new SkeletalAnimationKeyframeContent(keyframeNode));
            }
            mKeyframes.Sort();
        }
        #endregion Methods
    }
}
