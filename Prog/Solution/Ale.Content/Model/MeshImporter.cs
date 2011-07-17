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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Xml;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Ale.Content.Tools;
using Microsoft.Xna.Framework.Content;
using System.Collections.ObjectModel;

namespace Ale.Content
{
    /// <summary>
    /// Ale mesh importer
    /// </summary>
    [ContentImporter(".alm", DisplayName = "Mesh - Ale", CacheImportedData = true, DefaultProcessor = "MeshProcessor")]
    public class MeshImporter : ContentImporter<AleMeshContent>
    {
        #region Fields
        #endregion Fields

        #region Properties

        #endregion Properties

        #region Methods

        public virtual AleMeshContent ImportFromXmlNode(XmlNode node)
        {
            return ImportFromXmlNode(node, false);
        }

        public virtual AleMeshContent ImportFromXmlNode(XmlNode node, bool onlyMesh)
        {
            MeshBuilder meshBuilder = MeshBuilder.StartMesh(node.SelectSingleNode(@"./mesh/@name").Value);
            int normalDataIndex = meshBuilder.CreateVertexChannel<Vector3>(VertexChannelNames.Normal());
            int uvDataIndex = meshBuilder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            BoneContent[] flatSkeleton = null;
            int skinningDataIndex = -1;
            List<ConnectionPointContent> connectionPoints = new List<ConnectionPointContent>();
            SkeletalAnimationContentCollection skeletalAnimations = null;
            BoneContent skeleton = null;

            if (!onlyMesh)
            {
                //skeleton
                skeleton = LoadSkeleton(node, out flatSkeleton);
                if (null != skeleton)
                {
                    //skeletal animations
                    skeletalAnimations = new SkeletalAnimationContentCollection();
                    foreach (XmlNode animNode in node.SelectNodes(@"./anims/anim"))
                    {
                        SkeletalAnimationContent anim = new SkeletalAnimationContent(animNode);
                        skeletalAnimations.Add(anim.Name, anim);
                    }

                    skinningDataIndex = meshBuilder.CreateVertexChannel<BoneWeightCollection>(VertexChannelNames.Weights());
                }


                //connection points
                foreach (XmlNode connectionPointNode in node.SelectNodes(@"./connectionPoints/connectionPoint"))
                {
                    connectionPoints.Add(LoadConnectionPoint(connectionPointNode));
                }
            }

            //Verticies
            List<MeshVertex> verticies = new List<MeshVertex>();
            foreach (XmlNode vertex in node.SelectNodes(@"./mesh/vertices/vertex"))
            {
                verticies.Add(new MeshVertex(vertex, flatSkeleton));
            }


            //register positions and create position map
            int[] positionMap = new int[verticies.Count];
            for (int i = 0; i < verticies.Count; ++i)
            {
                positionMap[i] = meshBuilder.CreatePosition(verticies[i].Position);
            }

            //submeshes
            foreach (XmlNode submesh in node.SelectNodes(@"./mesh/submesh"))
            {
                BasicMaterialContent materialContent = new BasicMaterialContent();
                materialContent.Name = submesh.Attributes["material"].Value;
                meshBuilder.SetMaterial(materialContent);

                foreach (XmlNode face in submesh.SelectNodes(@"./face"))
                {
                    AddFaceVertex(meshBuilder, normalDataIndex, uvDataIndex, skinningDataIndex, verticies, positionMap[int.Parse(face.Attributes["v0"].Value, CultureInfo.InvariantCulture)]);
                    AddFaceVertex(meshBuilder, normalDataIndex, uvDataIndex, skinningDataIndex, verticies, positionMap[int.Parse(face.Attributes["v1"].Value, CultureInfo.InvariantCulture)]);
                    AddFaceVertex(meshBuilder, normalDataIndex, uvDataIndex, skinningDataIndex, verticies, positionMap[int.Parse(face.Attributes["v2"].Value, CultureInfo.InvariantCulture)]);
                }
            }

            return (new AleMeshContent(meshBuilder.FinishMesh(), skeleton, skeletalAnimations, connectionPoints));
        }

        /// <summary>
        /// Imports the mesh
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="context"></param>
        /// <returns>AleMeshContent</returns>
        public override AleMeshContent Import(string filename, ContentImporterContext context)
        {
            XmlDocument modelDocument = new XmlDocument();
            modelDocument.Load(filename);


            return ImportFromXmlNode(modelDocument.SelectSingleNode("model"));
        }

        /// <summary>
        /// Adds a vertex to the face
        /// </summary>
        /// <param name="meshBuilder">- Mesh builder</param>
        /// <param name="normalDataIndex">- Index of the vertex's normal data</param>
        /// <param name="uvDataIndex">- Index of the vertex's uv data</param>
        /// <param name="skinningDataIndex">-1 = no skinning data</param>
        /// <param name="verticies">- List of all vertices</param>
        /// <param name="vertexIndex">- Index of the vertex in the specified list</param>
        protected void AddFaceVertex(MeshBuilder meshBuilder, int normalDataIndex, int uvDataIndex, int skinningDataIndex, List<MeshVertex> verticies, int vertexIndex)
        {
            meshBuilder.SetVertexChannelData(normalDataIndex, verticies[vertexIndex].Normal);
            meshBuilder.SetVertexChannelData(uvDataIndex, verticies[vertexIndex].Uv);
            if (-1 != skinningDataIndex)
            {
                meshBuilder.SetVertexChannelData(skinningDataIndex, verticies[vertexIndex].BoneWeights);
            }

            meshBuilder.AddTriangleVertex(vertexIndex);
        }

        protected BoneContent LoadSkeleton(XmlNode modelNode, out BoneContent[] flatSkeleton)
        {
            XmlNode skeletonNode = modelNode.SelectSingleNode(@"./bones");
            if(null == skeletonNode)
            {
                flatSkeleton = null;
                return null;
            }
            int rootBoneIndex = Int32.Parse(skeletonNode.Attributes["root"].Value);
            
            XmlNodeList boneNodes = skeletonNode.SelectNodes("./bone");
            BoneContent[] bones = new BoneContent[boneNodes.Count];
            for(int i = 0; i < bones.Length; ++i)
            {
                bones[i] = new BoneContent();
            }

            foreach (XmlNode boneNode in boneNodes)
            {
                int index = Int32.Parse(boneNode.Attributes["index"].Value);
                BoneContent bone = bones[index];

                bone.Name = boneNode.Attributes["name"].Value;
                XmlAttribute parentIdAttribute = boneNode.Attributes["parent"];
                if (null != parentIdAttribute)
                {
                    int parentIndex = Int32.Parse(parentIdAttribute.Value);
                    bones[parentIndex].Children.Add(bone);
                }

                //transformation
                bone.Transform = XmlCommonParser.LoadTransformation(boneNode);
            }

            flatSkeleton = bones;
            return bones[rootBoneIndex];
        }

        protected ConnectionPointContent LoadConnectionPoint(XmlNode connectionPointElement)
        {
            ConnectionPointContent connectionPointContent = new ConnectionPointContent();
            XmlAttribute parentBoneNameAtt = connectionPointElement.Attributes["parentBone"];
            if (null != parentBoneNameAtt)
            {
                connectionPointContent.ParentBoneName = (string)parentBoneNameAtt.Value;
            }
            XmlAttribute nameAtt = connectionPointElement.Attributes["name"];
            if (null == nameAtt || string.IsNullOrEmpty(nameAtt.Value)) throw new ArgumentNullException("BoneName xml attribute");

            connectionPointContent.Name = (string)nameAtt.Value;

            Quaternion orientation;
            Vector3 position;
            XmlCommonParser.LoadTransformation(connectionPointElement, out position, out orientation);
            connectionPointContent.Position = position;
            connectionPointContent.Orientation = orientation;

            return connectionPointContent;
        }


        #endregion Methods

    }




    






    
}
