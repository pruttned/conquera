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
