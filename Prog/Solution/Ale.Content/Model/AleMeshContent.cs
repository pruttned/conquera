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
