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
