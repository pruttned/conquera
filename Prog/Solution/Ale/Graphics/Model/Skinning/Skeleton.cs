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
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Skeleton
    /// </summary>
    public class Skeleton : IEnumerable<SkeletonBone>
    {
        #region Fields

        /// <summary>
        /// Relative bone transformations in bind pose
        /// </summary>
        private SkeletonBone[] mBones;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the bone by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SkeletonBone this[int index]
        {
            get { return mBones[index]; }
        }

        /// <summary>
        /// Gets the number of bones
        /// </summary>
        public int BoneCnt
        {
            get { return mBones.Length; }
        }

        #endregion Properties

        #region  Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bones">Array that will be wrapped by skeleton class. 
        /// Parent of each bone must have a lower index in the array</param>
        /// <exception cref="ArgumentException">"Some bone has a parent with a bigger index</exception>
        public Skeleton(SkeletonBone[] bones)
        {
            mBones = bones;

            for (int i = 0; i < bones.Length; ++i)
            {
                if (bones[i].Parent > i)
                {
                    throw new ArgumentException(string.Format("Bone '{0}' has a parent with a bigger index - this is not allowed"));
                }
            }
        }

        /// <summary>
        /// Finds index of a specified bone
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Index of the bone or -1</returns>
        public int FindBoneByName(NameId name)
        {
            for (int i = 0; i < mBones.Length; ++i)
            {
                if (name == mBones[i].Name)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the bone by its index
        /// </summary>
        /// <param name="index"></param>
        public SkeletonBone GetBone(int index)
        {
            return mBones[index];
        }


        #region IEnumerable

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SkeletonBone> GetEnumerator()
        {
            return ((IEnumerable<SkeletonBone>)mBones).GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mBones.GetEnumerator();
        }

        #endregion IEnumerable

        /// <summary>
        /// Reads the skelton from a input (if boneCnt = 0 then this method returns null)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static Skeleton Read(ContentReader input)
        {
            int boneCnt = input.ReadInt32();
            if (0 == boneCnt)
            {
                return null;
            }
            SkeletonBone[] bones = new SkeletonBone[boneCnt];
            for (int i = 0; i < boneCnt; ++i)
            {
                bones[i] = new SkeletonBone(input, i);
            }
            return new Skeleton(bones);
        }

        #endregion  Methods
    }
}
