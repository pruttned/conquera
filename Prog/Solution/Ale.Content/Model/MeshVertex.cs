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
using System.Xml;
using System.Globalization;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Diagnostics;
using Ale.Content.Tools;

namespace Ale.Content
{
    public class MeshVertex
    {

        #region Fields

        private Vector3 mPosition;
        private Vector3 mNormal;
        private Vector2 mUv;
        private BoneWeightCollection mBoneWeights;

        #endregion Fields

        #region Properties

        public Vector3 Position
        {
            get { return mPosition; }
        }
        
        public Vector3 Normal
        {
            get { return mNormal; }
        }
        
        public Vector2 Uv
        {
            get { return mUv; }
        }

        public BoneWeightCollection BoneWeights
        {
            get { return mBoneWeights; }
        }


        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexNode"></param>
        /// <param name="flatSkeleton">- nullable</param>
        public MeshVertex(XmlNode vertexNode, BoneContent[] flatSkeleton)
        {
            mPosition = XmlCommonParser.ParseVector3(vertexNode.SelectSingleNode("./position"));
            mNormal = XmlCommonParser.ParseVector3(vertexNode.SelectSingleNode("./normal"));
            mUv = XmlCommonParser.ParseVector2(vertexNode.SelectSingleNode("./uv"));

            if (null != flatSkeleton && 0 < flatSkeleton.Length)
            {
                //Debugger.Launch();
                mBoneWeights = new BoneWeightCollection();

                foreach(XmlNode boneWeightNode in vertexNode.SelectNodes("./boneWeight"))
                {
                    mBoneWeights.Add(new BoneWeight(flatSkeleton[Int32.Parse(boneWeightNode.Attributes["bone"].Value)].Name,
                        float.Parse(boneWeightNode.Attributes["weight"].Value, CultureInfo.InvariantCulture)));
                }
            }
        }



        #endregion Methods
    }
}
