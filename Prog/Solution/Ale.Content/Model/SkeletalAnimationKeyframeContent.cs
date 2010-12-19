using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Ale.Content.Tools;
using System.Globalization;

namespace Ale.Content
{
    public class SkeletalAnimationKeyframeContent : IComparable<SkeletalAnimationKeyframeContent>
    {
        #region Fields
        private float mTime;
        private Vector3 mTranslation;
        private Quaternion mOrientation;
        #endregion Fields

        #region Properties

        public float Time
        {
            get { return mTime; }
        }

        public Vector3 Translation
        {
            get { return mTranslation; }
        }

        public Quaternion Orientation
        {
            get { return mOrientation; }
        }

        #endregion Properties
        #region Methods
        #region IComparable

        public SkeletalAnimationKeyframeContent(XmlNode keyframeNode)
        {
            mTime = float.Parse(keyframeNode.Attributes["time"].Value, CultureInfo.InvariantCulture);

            //transformation

            XmlNode orientationNode = keyframeNode.SelectSingleNode(@"./orientation");
            if (null != orientationNode)
            {
                mOrientation = XmlCommonParser.ParseQuaternion(orientationNode);
            }
            else
            {
                mOrientation = Quaternion.Identity;
            }
            XmlNode translationNode = keyframeNode.SelectSingleNode(@"./translation");
            if (null != translationNode)
            {
                mTranslation = XmlCommonParser.ParseVector3(translationNode);
            }
            else
            {
                mTranslation = Vector3.Zero;
            }
        }

        public int CompareTo(SkeletalAnimationKeyframeContent other)
        {
            return Comparer<float>.Default.Compare(mTime, other.mTime);
        }

        #endregion IComparable
        #endregion Methods
    }
}
