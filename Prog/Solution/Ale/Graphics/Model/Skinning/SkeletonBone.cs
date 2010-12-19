//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Skeleton bone
    /// </summary>
    public class SkeletonBone
    {
        #region Fields

        /// <summary>
        /// Bone index in skeleton
        /// </summary>
        private int mIndex;

        /// <summary>
        /// Name
        /// </summary>
        private NameId mName;

        /// <summary>
        /// Relative bone transformations in bind pose
        /// </summary>
        private Matrix mBindPose;

        /// <summary>
        /// Inverse absolute transformations of bones in bind pose
        /// </summary>
        private Matrix mInverseAbsoluteBindPose;

        /// <summary>
        /// Parent of the bone (-1 = root)
        /// </summary>
        private int mParent;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the bone's index in skeleton
        /// </summary>
        public int Index
        {
            get { return mIndex; }
        }

        /// <summary>
        /// Gets the bone's name
        /// </summary>
        public NameId Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the relative bone transformations in bind pose
        /// </summary>
        public Matrix BindPose
        {
            get { return mBindPose; }
        }

        /// <summary>
        /// Gets the inverse absolute transformations of bones in bind pose
        /// </summary>
        public Matrix InverseAbsoluteBindPose
        {
            get { return mInverseAbsoluteBindPose; }
        }

        /// <summary>
        /// Gets the parent of the bone (-1 = root)
        /// </summary>
        public int Parent
        {
            get { return mParent; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bindPose"></param>
        /// <param name="inverseAbsoluteBindPose"></param>
        /// <param name="parent"></param>
        public SkeletonBone(NameId name, Matrix bindPose, Matrix inverseAbsoluteBindPose, int parent)
        {
            mName = name;
            mBindPose = bindPose;
            mInverseAbsoluteBindPose = inverseAbsoluteBindPose;
            mParent = parent;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="input"></param>
        internal SkeletonBone(ContentReader input, int index)
        {
            mIndex = index;
            mName = input.ReadString();
            mParent = input.ReadInt32();
            mBindPose = input.ReadMatrix();
            mInverseAbsoluteBindPose = input.ReadMatrix();
        }

        public override string ToString()
        {
            return mName.Name;
        }

        #endregion Methods
    }
}
