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
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Ale.Graphics;

namespace Ale.Content
{
    public class AleCompiledMesh
    {
        public IndexCollection IndexBuffer {get; set;}

        public VertexBufferContent VertexBuffer {get; set;}

        public VertexElement[] VertexDeclaration {get; set;}

        public AleCompiledMeshPart[] MeshParts {get; set;}

        public Skeleton Skeleton {get; set;}

        public List<SkeletalAnimation> SkeletalAnimations {get; set;}


        /// <summary>
        /// Gets the Xna mesh content
        /// </summary>
        public MeshContent MeshContent {get; set;}

        /// <summary>
        /// Gets the bounds of the mesh
        /// </summary>
        public BoundingSphere Bounds { get; set; }

        public List<ConnectionPointContent> ConnectionPoints { get; set; }

        #region Methods

        public AleCompiledMesh()
        {}
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="meshContent">- Xna mesh content</param>
        public AleCompiledMesh(MeshContent meshContent, BoundingSphere bounds, IndexCollection indexBuffer, VertexBufferContent vertexBuffer,
            VertexElement[] vertexDeclaration, AleCompiledMeshPart[] meshParts, Skeleton skeleton, List<SkeletalAnimation> skeletalAnimations,
            List<ConnectionPointContent> connectionPoints)
        {
            MeshContent = meshContent;
            Bounds = bounds;
            IndexBuffer = indexBuffer;
            VertexBuffer = vertexBuffer;
            VertexDeclaration = vertexDeclaration;
            MeshParts = meshParts;
            Skeleton = skeleton;
            SkeletalAnimations = skeletalAnimations;
            ConnectionPoints = connectionPoints;
        }

        public virtual void Write(ContentWriter output)
        {
            output.WriteObject<VertexElement[]>(VertexDeclaration);
            output.WriteObject<VertexBufferContent>(VertexBuffer);
            output.WriteObject<IndexCollection>(IndexBuffer);

            //bounds
            output.Write(Bounds.Center);
            output.Write(Bounds.Radius);

            //mehs parts
            output.Write(MeshParts.Length);
            foreach (AleCompiledMeshPart meshPart in MeshParts)
            {
                meshPart.Write(output);
            }

            //skeleton
            WriteSkeleton(output);

            //skeletal animations
            WriteSkeletalAnimations(output);

            //connection points
            WriteConnectionPoints(output);
        }

        protected void WriteSkeleton(ContentWriter output)
        {
            if (null != Skeleton)
            {
                output.Write(Skeleton.BoneCnt);
                foreach (SkeletonBone bone in Skeleton)
                {
                    output.Write(bone.Name.Name);
                    output.Write(bone.Parent);
                    output.Write(bone.BindPose);
                    output.Write(bone.InverseAbsoluteBindPose);
                }
            }
            else
            {
                output.Write(0);
            }
        }

        protected void WriteSkeletalAnimations(ContentWriter output)
        {
            if (null != SkeletalAnimations)
            {
                output.Write(SkeletalAnimations.Count);
                foreach (SkeletalAnimation skeletalAnimation in SkeletalAnimations)
                {
                    output.Write(skeletalAnimation.Name.Name);
                    output.Write(skeletalAnimation.Duration);
                    output.Write(skeletalAnimation.DefaultSpeed);
                    output.Write(skeletalAnimation.Channels.Count);
                    foreach (SkeletalAnimationChannel channel in skeletalAnimation.Channels)
                    {
                        output.Write(channel.Bone);
                        output.Write(channel.KeyframeCnt);
                        foreach (SkeletalAnimationKeyframe keyframe in channel)
                        {
                            output.Write(keyframe.Time);
                            output.Write(keyframe.Translation);
                            output.Write(keyframe.Orientation);
                        }
                    }
                }
            }
            else
            {
                output.Write(0);
            }
        }

        protected void WriteConnectionPoints(ContentWriter output)
        {
            if (null != ConnectionPoints)
            {
                output.Write(ConnectionPoints.Count);
                foreach (ConnectionPointContent cp in ConnectionPoints)
                {
                    output.Write(cp.Name);
                    bool hasParentBone = !string.IsNullOrEmpty(cp.ParentBoneName);
                    output.Write(hasParentBone);
                    if (hasParentBone)
                    {
                        output.Write(cp.ParentBoneName);
                    }
                    output.Write(cp.Position);
                    output.Write(cp.Orientation);
                }
            }
            else
            {
                output.Write(0);
            }
        }

        #endregion Methods
    }
}
