using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework;
using Ale.Content.Tools;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Ale.Graphics;
using Ale.Tools;

namespace Ale.Content
{
    /// <summary>
    /// Mesh processor
    /// </summary>
    [ContentProcessor(DisplayName = "Mesh - Ale")]
    public class MeshProcessor : ContentProcessor<AleMeshContent, AleCompiledMesh>
    {
        /// <summary>
        /// Max number of bones supported by shader
        /// </summary>
        const int MaxBoneCnt = 59;

        /// <summary>
        /// Preocess the mesh
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns>AleCompiledMesh</returns>
        public override AleCompiledMesh Process(AleMeshContent input, ContentProcessorContext context)
        {
            MeshContent mesh = input.MeshContent;
            BoneContent xnaSkeleton = input.SkeletonContent;

            Skeleton skeleton = ProcessSkeleton(input);

            List<SkeletalAnimation> skeletalAnimations = ProcessAnimationData(input, skeleton);

            NodeContent rootNodeContent;
            if (null == skeleton) //no skeleton - mesh is root
            {
                rootNodeContent = mesh;
            }
            else
            {//modelProcessor.Process needs this
                rootNodeContent = new NodeContent();
                rootNodeContent.Children.Add(xnaSkeleton);
                rootNodeContent.Children.Add(mesh);
            }

            ModelProcessor modelProcessor = new ModelProcessor();

            ModelContent modelContent = modelProcessor.Process(rootNodeContent, context);

            ModelMeshContent modelMeshContent = modelContent.Meshes[0]; //there must be exactly one mesh

            //bounds
            BoundingSphere bounds = modelMeshContent.BoundingSphere;
            //bounds.Radius *= 2;  //just to be sure - vertex shaders;  - this is setable in graphic model

            //mesh parts
            AleCompiledMeshPart[] meshParts = new AleCompiledMeshPart[modelMeshContent.MeshParts.Count];
            for (int i = 0; i < modelMeshContent.MeshParts.Count; ++i)
            {
                ModelMeshPartContent xnaMeshPart = modelMeshContent.MeshParts[i];
                meshParts[i] = new AleCompiledMeshPart(xnaMeshPart.BaseVertex, xnaMeshPart.NumVertices, xnaMeshPart.PrimitiveCount, xnaMeshPart.StartIndex, xnaMeshPart.Material.Name);
            }



            return new AleCompiledMesh(modelMeshContent.SourceMesh, bounds, modelMeshContent.IndexBuffer, modelMeshContent.VertexBuffer,
                modelMeshContent.MeshParts[0].GetVertexDeclaration(), meshParts, skeleton, skeletalAnimations, input.ConnectionPoints);
        }



        private List<SkeletalAnimation> ProcessAnimationData(AleMeshContent input, Skeleton skeleton)
        {
            SkeletalAnimationContentCollection animationContens = input.SkeletalAnimations;
            if (null == animationContens)
            {
                return null;
            }

            List<SkeletalAnimation> animations = new List<SkeletalAnimation>();

            foreach (SkeletalAnimationContent animationContent in animationContens.Values)
            {
                List<SkeletalAnimationChannel> skeletalAnimationChannels = new List<SkeletalAnimationChannel>();
                foreach (SkeletalAnimationChannelContent animChannelContent in animationContent.Channels)
                {
                    SkeletalAnimationKeyframe[] keyFrames = new SkeletalAnimationKeyframe[animChannelContent.Keyframes.Count];
                    for (int i = 0; i < keyFrames.Length; ++i)
                    {
                        keyFrames[i] = new SkeletalAnimationKeyframe(animChannelContent.Keyframes[i].Time,
                            animChannelContent.Keyframes[i].Translation,
                            animChannelContent.Keyframes[i].Orientation);
                    }
                    int boneId = skeleton.FindBoneByName(animChannelContent.Bone);
                    if (-1 == boneId)
                    {
                        throw new AleInvalidContentException(string.Format("Bone '{0}' was found in anim channels but it doesn't exists in the skeleton", animChannelContent.Bone));
                    }
                    skeletalAnimationChannels.Add(new SkeletalAnimationChannel(boneId, keyFrames));
                }

                animations.Add(new SkeletalAnimation(animationContent.Name, animationContent.Duration, skeletalAnimationChannels));
            }

            return animations;
        }

        //SkeletalAnimationChannel

        Skeleton ProcessSkeleton(AleMeshContent input)
        {
            BoneContent xnaSkeleton = input.SkeletonContent;
            if (null == xnaSkeleton)
            {
                return null;
            }

            IList<BoneContent> xnaBones = MeshHelper.FlattenSkeleton(xnaSkeleton);

            if (MaxBoneCnt < xnaBones.Count)
            {
                throw new AleInvalidContentException(string.Format("Maximum number of bones ({0}) exceeded.", MaxBoneCnt));
            }

            SkeletonBone[] skeletonBones = new SkeletonBone[xnaBones.Count];
            for (int i = 0; i < skeletonBones.Length; ++i)
            {
                BoneContent xnaBone = xnaBones[i];
                skeletonBones[i] = new SkeletonBone(xnaBone.Name, xnaBone.Transform,
                    Matrix.Invert(xnaBone.AbsoluteTransform), xnaBones.IndexOf(xnaBone.Parent as BoneContent));
            }
            return new Skeleton(skeletonBones);
        }
    }



}
