using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Contains keyframes for one bone in the skeletal animation
    /// </summary>
    public class SkeletalAnimationChannel : IEnumerable<SkeletalAnimationKeyframe>
    {
        #region Fields
        
        /// <summary>
        /// Bone's id
        /// </summary>
        private int mBone;

        /// <summary>
        /// Sorted keyframes
        /// </summary>
        private SkeletalAnimationKeyframe[] mKeyframes;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the bone's id
        /// </summary>
        public int Bone
        {
            get { return mBone; }
        }

        /// <summary>
        /// Gets the number of keyframes
        /// </summary>
        public int KeyframeCnt
        {
            get { return mKeyframes.Length; }
        }

        #endregion Properties
        
        #region Methods 

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="keyframes"></param>
        public SkeletalAnimationChannel(int bone, SkeletalAnimationKeyframe[] keyframes)
        {
            mBone = bone;
            mKeyframes = keyframes;
            Array.Sort<SkeletalAnimationKeyframe>(mKeyframes);
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="input"></param>
        internal SkeletalAnimationChannel(ContentReader input)
        {
            mBone = input.ReadInt32();
            int keyframeCnt = input.ReadInt32();
            mKeyframes = new SkeletalAnimationKeyframe[keyframeCnt];
            for (int i = 0; i < keyframeCnt; ++i)
            {
                mKeyframes[i] = new SkeletalAnimationKeyframe(input);
            }
        }

        /// <summary>
        /// Gets the transformation at a given time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="transformation"></param>
        public void GetTransformation(float time, out Matrix transformation)
        {
            int keyFrameIndex = FindKeyFrame(time);
            if(mKeyframes.Length <= keyFrameIndex)
            {
                keyFrameIndex = mKeyframes.Length - 1;
            }
            SkeletalAnimationKeyframe keyFrame = mKeyframes[keyFrameIndex];

            if (time != keyFrame.Time && 0 != keyFrameIndex)
            {//interpolation
                SkeletalAnimationKeyframe prevKeyFrame = mKeyframes[keyFrameIndex - 1];

                float iWeight = (time - prevKeyFrame.Time) / (keyFrame.Time - prevKeyFrame.Time);
                Quaternion iOrientation = Quaternion.Lerp(prevKeyFrame.Orientation, keyFrame.Orientation, iWeight);
                Vector3 iTranslation = Vector3.Lerp(prevKeyFrame.Translation, keyFrame.Translation, iWeight);
                Matrix.CreateFromQuaternion(ref iOrientation, out transformation);
                transformation.Translation = iTranslation;
            }
            else
            {
                transformation = Matrix.CreateFromQuaternion(keyFrame.Orientation);
                transformation.Translation = keyFrame.Translation;
            }
        }

        #region IEnumerable

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SkeletalAnimationKeyframe> GetEnumerator()
        {
            return ((IEnumerable<SkeletalAnimationKeyframe>)mKeyframes).GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mKeyframes.GetEnumerator();
        }

        #endregion IEnumerable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int FindKeyFrame(float time)
        {
            int start = 0;
            int cnt = mKeyframes.Length;

            if (10 > cnt)
            {
                for (int i = 0; i < cnt; ++i)
                {
                    float value = mKeyframes[i].Time;
                    if (value == time)
                    {
                        return i;
                    }
                    else
                    {
                        if (value > time)
                        {
                            return i;
                        }
                    }
                }
            }
            else
            {
                //binary search
                int end = cnt;
                while (start <= end)
                {
                    int middle = start + ((end - start) >> 1);

                    if (cnt <= middle)
                    {
                        end = middle - 1;
                    }
                    else
                    {
                        float middleValue = mKeyframes[middle].Time;
                        if (middleValue == time)
                        {
                            return middle;
                        }
                        if (time > middleValue)
                        {
                            start = middle + 1;
                        }
                        else
                        {
                            end = middle - 1;
                        }
                    }
                }
            }
            return start;
        }

        #endregion Methods
    }
}
