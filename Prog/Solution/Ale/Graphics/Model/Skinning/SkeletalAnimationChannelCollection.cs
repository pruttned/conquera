using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of skeletal animation channels
    /// </summary>
    public class SkeletalAnimationChannelCollection : ReadOnlyCollection<SkeletalAnimationChannel>
    {
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="channels">channel list that will be wrapped by this collection</param>
        public SkeletalAnimationChannelCollection(List<SkeletalAnimationChannel> channels)
            : base(SortListByBoneId(channels))
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="input"></param>
        internal SkeletalAnimationChannelCollection(ContentReader input)
            : this(ReadChannels(input))
        {
        }

        /// <summary>
        /// Gets the channel of a specified bone (or null if it doesn't exists)
        /// </summary>
        /// <param name="boneId"></param>
        /// <returns></returns>
        public SkeletalAnimationChannel FindBoneChannel(int boneId)
        {
            int start = 0;
            int end = this.Count;
            while (start <= end)
            {
                int middle = start + ((end - start) >> 1);

                int cmpResult = this[middle].Bone.CompareTo(boneId);

                if (0 == cmpResult)
                {
                    return this[middle];
                }
                if (0 > cmpResult)
                {
                    start = middle + 1;
                }
                else
                {
                    end = middle - 1;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static List<SkeletalAnimationChannel> ReadChannels(ContentReader input)
        {
            int channelCnt = input.ReadInt32();
            List<SkeletalAnimationChannel> channels = new List<SkeletalAnimationChannel>(channelCnt);
            for (int i = 0; i < channelCnt; ++i)
            {
                channels.Add(new SkeletalAnimationChannel(input));
            }
            return channels;
        }

        /// <summary>
        /// Sorts the keletalAnimationChannel list by bone ids
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<SkeletalAnimationChannel> SortListByBoneId(List<SkeletalAnimationChannel> list)
        {
            list.Sort(delegate(SkeletalAnimationChannel ch1, SkeletalAnimationChannel ch2)
                {
                    return ch1.Bone.CompareTo(ch2.Bone);
                }); 
            return list;
        }

        #endregion Methods
    }
}
