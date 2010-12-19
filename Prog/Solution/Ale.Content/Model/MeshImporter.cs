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


            MeshBuilder meshBuilder = MeshBuilder.StartMesh(modelDocument.SelectSingleNode(@"/model/mesh/@name").Value);
            int normalDataIndex = meshBuilder.CreateVertexChannel<Vector3>(VertexChannelNames.Normal());
            int uvDataIndex = meshBuilder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));


            //skeleton
            SkeletalAnimationContentCollection skeletalAnimations = null;
            BoneContent[] flatSkeleton;
            BoneContent skeleton = LoadSkeleton(modelDocument, out flatSkeleton);
            int skinningDataIndex = -1;
            if (null != skeleton)
            {
                //skeletal animations
                skeletalAnimations = new SkeletalAnimationContentCollection();
                foreach (XmlNode animNode in modelDocument.SelectNodes(@"/model/anims/anim"))
                {
                    SkeletalAnimationContent anim = new SkeletalAnimationContent(animNode);
                    skeletalAnimations.Add(anim.Name, anim);
                }

                skinningDataIndex = meshBuilder.CreateVertexChannel<BoneWeightCollection>(VertexChannelNames.Weights());
            }

            //Verticies
            List<MeshVertex> verticies = new List<MeshVertex>();
            foreach (XmlNode vertex in modelDocument.SelectNodes(@"/model/mesh/vertices/vertex"))
            {
                verticies.Add(new MeshVertex(vertex, flatSkeleton));
            }


            //register positions and create position map
            int[] positionMap = new int[verticies.Count];
            for(int i = 0; i<verticies.Count; ++i)
            {
                positionMap[i] = meshBuilder.CreatePosition(verticies[i].Position);
            }

            //submeshes
            foreach (XmlNode submesh in modelDocument.SelectNodes(@"/model/mesh/submesh"))
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

            //connection points
            List<ConnectionPointContent> connectionPoints = new List<ConnectionPointContent>();
            foreach(XmlNode connectionPointNode in modelDocument.SelectNodes(@"/model/connectionPoints/connectionPoint"))
            {
                connectionPoints.Add(LoadConnectionPoint(connectionPointNode));
            }

            return (new AleMeshContent(meshBuilder.FinishMesh(), skeleton, skeletalAnimations, connectionPoints));
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

        protected BoneContent LoadSkeleton(XmlDocument modelDocument, out BoneContent[] flatSkeleton)
        {
            XmlNode skeletonNode = modelDocument.SelectSingleNode(@"/model/bones");
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
                bone.Transform = LoadTransformation(boneNode);
            }

            flatSkeleton = bones;
            return bones[rootBoneIndex];
        }

        /// <summary>
        /// No scale
        /// </summary>
        /// <param name="boneNode"></param>
        /// <returns></returns>
        protected Matrix LoadTransformation(XmlNode boneNode)
        {
            Matrix transf;
            XmlNode orientationNode = boneNode.SelectSingleNode(@"./orientation");
            if (null != orientationNode)
            {
                transf = Matrix.CreateFromQuaternion(XmlCommonParser.ParseQuaternion(orientationNode));
            }
            else
            {
                transf = Matrix.Identity;
            }
            XmlNode translationNode = boneNode.SelectSingleNode(@"./translation");
            if (null != translationNode)
            {
                transf.Translation = XmlCommonParser.ParseVector3(translationNode);
            }
            return transf;
        }

        /// <summary>
        /// No scale
        /// </summary>
        /// <param name="boneNode"></param>
        /// <returns></returns>
        protected void LoadTransformation(XmlNode boneNode, out Vector3 position, out Quaternion orientation)
        {
            XmlNode orientationNode = boneNode.SelectSingleNode(@"./orientation");
            if (null != orientationNode)
            {
                orientation = XmlCommonParser.ParseQuaternion(orientationNode);
            }
            else
            {
                orientation = Quaternion.Identity;
            }
            XmlNode translationNode = boneNode.SelectSingleNode(@"./translation");
            if (null != translationNode)
            {
                position = XmlCommonParser.ParseVector3(translationNode);
            }
            else
            {
                position = Vector3.Zero;
            }
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
            LoadTransformation(connectionPointElement, out position, out orientation);
            connectionPointContent.Position = position;
            connectionPointContent.Orientation = orientation;

            return connectionPointContent;
        }


        #endregion Methods

    }




    






    
}