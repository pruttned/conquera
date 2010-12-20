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
