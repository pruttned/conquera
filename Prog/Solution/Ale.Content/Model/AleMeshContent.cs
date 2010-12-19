using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Content
{
    /// <summary>
    /// Ale mesh content
    /// </summary>
    public class AleMeshContent
    {
        /// <summary>
        /// Xna mesh content
        /// </summary>
        public MeshContent MeshContent { get; set; }

        /// <summary>
        /// Skeleton
        /// </summary>
        public BoneContent SkeletonContent { get; set; }

        /// <summary>
        /// Skeletal animations
        /// </summary>
        public SkeletalAnimationContentCollection SkeletalAnimations { get; set;}

        public List<ConnectionPointContent> ConnectionPoints { get; set; }

        public AleMeshContent()
        {}

        /// <summary>
        /// Ctor
        /// </summary>
        public AleMeshContent(MeshContent meshContent, BoneContent skeletonContent, SkeletalAnimationContentCollection skeletalAnimations,
            List<ConnectionPointContent> connectionPoints)
        {
            MeshContent = meshContent;
            SkeletonContent = skeletonContent;
            SkeletalAnimations = skeletalAnimations;
            ConnectionPoints = connectionPoints;
        }
    }
}
