using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Ale.Tools;

namespace Ale.Graphics
{
    public class SkeletalAnimationPlayer
    {
        #region Enums

        public enum AnimState
        {
            Stopped,
        //    Paused,
            Playing
        }

        #endregion Enums

        #region Delegates

        public delegate void AnimStoppedHandler(SkeletalAnimationPlayer animation);

        #endregion Delegates

        #region Events

        public event AnimStoppedHandler AnimStopped;

        #endregion Events

        #region Fields

        private static Matrix[] mBoneWorldTransformations;

        private GraphicModel mGraphicModel;
        private SkeletalAnimation mAnimation;
        private bool mLoop;
        private bool mRandomStart;
        private float mAnimStart;
        private bool mFirstUpdate;

        private float mTimeInAnimation = 0;


        AnimState mAnimationState = AnimState.Stopped;

        float mAnimSpeed = 1.0f;

        #endregion Fields

        #region Properties

        public AnimState AnimationState
        {
            get { return mAnimationState; }
        }

        public float AnimSpeed
        {
            get { return mAnimSpeed; }
            set { mAnimSpeed = value; }
        }

        public NameId Animation
        {
            get { return mAnimation.Name; }
            set 
            {
                if (null == value) throw new ArgumentNullException("Animation");

                SkeletalAnimationCollection skeletalAnimations = mGraphicModel.Mesh.SkeletalAnimations;
                int skeletalAnimationIndex = -1;
                if (null != mGraphicModel.Mesh.SkeletalAnimations)
                {
                    skeletalAnimationIndex = skeletalAnimations.IndexOf(value);
                }
                if (-1 == skeletalAnimationIndex)
                {
                    throw new ArgumentException(string.Format("Animation '{0}' doesn't exists", value));
                }

                mAnimation = skeletalAnimations[skeletalAnimationIndex];
                Rewind();
            }
        }

        #endregion Properties

        #region Method

        public SkeletalAnimationPlayer(GraphicModel graphicModel, NameId animationName)
        {
            if (null == graphicModel) throw new ArgumentNullException("graphicModel");
            if (null == animationName) throw new ArgumentNullException("animationName");

            mGraphicModel = graphicModel;
            Animation = animationName;
            Skeleton skeleton = graphicModel.Mesh.Skeleton;
            if (null == mBoneWorldTransformations || mBoneWorldTransformations.Length < skeleton.BoneCnt)
            {
                mBoneWorldTransformations = new Matrix[skeleton.BoneCnt];
            }
        }

        public void Play(bool loop)
        {
            Play(loop, false);
        }

        public void Play(bool loop, bool randomStart)
        {
            mLoop = loop;
            mRandomStart = randomStart;
            mFirstUpdate = true;
            mAnimationState = AnimState.Playing;
        }

 /*       public void Pause()
        {
            mAnimationState = AnimState.Paused;
            throw new NotImplementedException();
        }

        public void Resume()
        {
            mAnimationState = AnimState.Playing;
            throw new NotImplementedException();
        }
        */
        public void Stop()
        {
            mAnimationState = AnimState.Stopped;
            if (null != AnimStopped)
            {
                AnimStopped.Invoke(this);
            }
        }

        public void Rewind()
        {
            mFirstUpdate = true;
        }

        public void Update(AleGameTime gameTime, Matrix[] skinTransformations)
        {
            if (null == gameTime) throw new ArgumentNullException("gameTime");
            if (null == skinTransformations) throw new ArgumentNullException("skinTransformations");

            if (AnimState.Stopped != mAnimationState)
            {
                if (AnimState.Playing == mAnimationState)
                {
                    float timeNow = gameTime.TotalTime;
                    if (mFirstUpdate)
                    {
                        if (mRandomStart)
                        {
                            mAnimStart = timeNow - ((float)AleMathUtils.Random.NextDouble() * mAnimation.Duration);
                        }
                        else
                        {
                            mAnimStart = timeNow;
                        }


                        mFirstUpdate = false;
                    }

                    mTimeInAnimation = (timeNow - mAnimStart) * mAnimSpeed;

                    if (mAnimation.Duration < mTimeInAnimation)
                    {
                        if (mLoop)
                        {
                            mTimeInAnimation = mTimeInAnimation % mAnimation.Duration;
                            mAnimStart = timeNow - mTimeInAnimation;
                        }
                        else
                        {
                            mTimeInAnimation = 0;
                            Stop();
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            UpdateSkinTransformations(skinTransformations);
        }

        /// <summary>
        /// Updates skin transformations using last update time. 
        /// Can be used when the animation is stoped and world transf has been changed. Otherwise use Update
        /// </summary>
        /// <param name="skinTransformations"></param>
        public void UpdateSkinTransformations(Matrix[] skinTransformations)
        {
            if (null == skinTransformations) throw new ArgumentNullException("skinTransformations");

            UpdateWorldBoneTransformations(mTimeInAnimation, mGraphicModel.WorldTransformation);

            Skeleton skeleton = mGraphicModel.Mesh.Skeleton;
            for (int i = 0; i < skeleton.BoneCnt; ++i)
            {
                Matrix inverseAbsoluteBindPose = skeleton[i].InverseAbsoluteBindPose;
                Matrix.Multiply(ref inverseAbsoluteBindPose, ref mBoneWorldTransformations[i], out skinTransformations[i]);
                //skinTransformations[i] = skeleton[i].InverseAbsoluteBindPose * mBoneWorldTransformations[i];
            }
        }

        private void UpdateWorldBoneTransformations(float timeInAnimation, Matrix worldTransf)
        {
            Skeleton skeleton = mGraphicModel.Mesh.Skeleton;

            { //root bone
                SkeletalAnimationChannel boneChannel = mAnimation.Channels.FindBoneChannel(0);
                SkeletonBone bone = skeleton[0];
                Matrix boneTransf;
                if (null != boneChannel)
                {
                    boneChannel.GetTransformation(timeInAnimation, out boneTransf);
                }
                else
                {
                    boneTransf = bone.BindPose;
                }
                Matrix.Multiply(ref boneTransf, ref worldTransf, out mBoneWorldTransformations[0]);
            }
            //other bones
            for (int i = 1; i < skeleton.BoneCnt; ++i) 
            {
                SkeletalAnimationChannel boneChannel = mAnimation.Channels.FindBoneChannel(i);
                SkeletonBone bone = skeleton[i];
                Matrix boneTransf;
                if (null != boneChannel)
                {
                    boneChannel.GetTransformation(timeInAnimation, out boneTransf);
                }
                else
                {
                    boneTransf = bone.BindPose;
                }
                Matrix.Multiply(ref boneTransf, ref mBoneWorldTransformations[bone.Parent], out mBoneWorldTransformations[i]);
            }
        }

        #endregion Method
    }
}
