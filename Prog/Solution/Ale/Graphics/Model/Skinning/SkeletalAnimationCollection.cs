using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework.Content;
using System.Collections.ObjectModel;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of skeletal animations
    /// </summary>
    public class SkeletalAnimationCollection : ReadOnlyCollection<SkeletalAnimation>
    {
        #region Properties

        /// <summary>
        /// Gets the animation by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Skeletal animation or Null</returns>
        public SkeletalAnimation this[NameId name]
        {
            get
            {
                for (int i = 0; i < Count; ++i)
                {
                    SkeletalAnimation anim = this[i];
                    if (name == anim.Name)
                    {
                        return anim;
                    }
                }
                return null;
            }

        }
        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="animations">- Dictionary that will be wrapped by this class</param>
        public SkeletalAnimationCollection(IList<SkeletalAnimation> animations)
            : base(animations)
        {
        }

        /// <summary>
        /// Reads the SkeletalAnimationCollection from a input (if animCnt = 0 then this method returns null)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static SkeletalAnimationCollection Read(ContentReader input)
        {
            int animCnt = input.ReadInt32();
            if (0 == animCnt)
            {
                return null;
            }

            List<SkeletalAnimation> animations = new List<SkeletalAnimation>();

            for (int i = 0; i < animCnt; ++i)
            {
                animations.Add(new SkeletalAnimation(input));
            }
            return new SkeletalAnimationCollection(animations);
        }

        /// <summary>
        /// Gets the index of a animation specified by its name
        /// </summary>
        /// <param name="animName"></param>
        /// <returns>Index or -1</returns>
        public int IndexOf(NameId animName)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (animName == this[i].Name)
                {
                    return i;
                }
            }
            return -1;
        }

        
        #endregion Methods
    }
}
