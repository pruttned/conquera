using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Tools
{
    public abstract class LinearAnimator<T>
    {
        private bool mIsAnimating = false;
        private float mStartTime;
        private float mDuration;
        private T mStartValue;
        private T mEndValue;
        private float mSpeed;

        private T mCurrentValue;

        public T CurrentValue
        {
            get { return mCurrentValue; }
        }
        public bool IsAnimating
        {
            get { return mIsAnimating; }
        }

        public void Animate(float speed, T start, T end)
        {
            mSpeed = speed;
            mStartTime = -1;
            mIsAnimating = true;

            ChangeStartEnd(start, end);

            mCurrentValue = start;
        }

        public void ChangeStartEnd(T start, T end)
        {
            if (mIsAnimating)
            {
                mStartValue = start;
                mEndValue = end;

                mDuration = GetAnimationDuration(mSpeed, mStartValue, mEndValue);
                if (mDuration < 0.000001f)
                {
                    mIsAnimating = false;
                    mCurrentValue = mEndValue;
                }
            }
        }

        public bool Update(AleGameTime gameTime)
        {
            if (mIsAnimating)
            {
                if (0 > mStartTime)
                {
                    mStartTime = gameTime.TotalTime;
                }

                if (gameTime.TotalTime > mStartTime + mDuration)
                {
                    mIsAnimating = false;
                    mCurrentValue = mEndValue;
                }
                else
                {
                    float lerp = (gameTime.TotalTime - mStartTime) / mDuration;

                    mCurrentValue = GetValueFromLerp(lerp, mStartValue, mEndValue);
                }

                return true;
            }

            return false;
        }

        protected abstract float GetAnimationDuration(float speed, T start, T end);
        protected abstract T GetValueFromLerp(float lerp, T start, T end);
    }

    public class Vector3LinearAnimator : LinearAnimator<Vector3>
    {
        protected override float GetAnimationDuration(float speed, Vector3 start, Vector3 end)
        {
            float dist;
            Vector3.Distance(ref start, ref end, out dist);
            return dist / speed;
        }

        protected override Vector3 GetValueFromLerp(float lerp, Vector3 start, Vector3 end)
        {
            Vector3 value;
            Vector3.Lerp(ref start, ref end, lerp, out value);
            return value;
        }
    }

    public class Vector2LinearAnimator : LinearAnimator<Vector2>
    {
        protected override float GetAnimationDuration(float speed, Vector2 start, Vector2 end)
        {
            float dist;
            Vector2.Distance(ref start, ref end, out dist);
            return dist / speed;
        }

        protected override Vector2 GetValueFromLerp(float lerp, Vector2 start, Vector2 end)
        {
            Vector2 value;
            Vector2.Lerp(ref start, ref end, lerp, out value);
            return value;
        }
    }

    public class FloatLinearAnimator : LinearAnimator<float>
    {
        protected override float GetAnimationDuration(float speed, float start, float end)
        {
            float dist = Math.Abs(end - start);
            return dist / speed;
        }

        protected override float GetValueFromLerp(float lerp, float start, float end)
        {
            return MathHelper.Lerp(start, end, lerp);
        }
    }

    public class PointLinearAnimator : LinearAnimator<Point>
    {
        protected override float GetAnimationDuration(float speed, Point start, Point end)
        {
            float dist = Vector2.Distance(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y));
            return dist / speed;
        }

        protected override Point GetValueFromLerp(float lerp, Point start, Point end)
        {
            return new Point((int)(start.X + ((end.X - start.X) * lerp)),
                (int)(start.Y + ((end.Y - start.Y) * lerp)));
        }
    }

}
