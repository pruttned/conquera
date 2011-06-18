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
using System.Linq;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;

namespace Ale.SpecialEffects
{
    public abstract class SpecialEffectObjectDesc : IDisposable
    {
        private bool mIsDisposed = false;

        public NameId Name { get; internal set; }
        public Vector3 Position { get; internal set; }
        public Quaternion Orientation { get; internal set; }
        public float Scale { get; internal set; }

        public SpecialEffectObjectAnim Anim { get; internal set; }

        public abstract SpecialEffectObject CreateObjectInstance(IAleServiceProvider services, Vector3 effectPos);

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the transformation at a given time
        /// </summary>
        public void GetTransformation(float time, out Vector3 translation, out Quaternion orientation, out float scale)
        {
            if (null == Anim)
            {
                throw new InvalidOperationException("Anim == null");
            }

            var keyframes = Anim.Keyframes;

            int keyFrameIndex = FindKeyFrame(time);
            if (keyframes.Count <= keyFrameIndex)
            {
                keyFrameIndex = keyframes.Count - 1;
            }
            var keyFrame = keyframes[keyFrameIndex];

            if (time != keyFrame.Time && 0 != keyFrameIndex)
            {//interpolation
                var prevKeyFrame = keyframes[keyFrameIndex - 1];

                float iWeight = (time - prevKeyFrame.Time) / (keyFrame.Time - prevKeyFrame.Time);
                translation = Vector3.Lerp(prevKeyFrame.Translation, keyFrame.Translation, iWeight);
                orientation = Quaternion.Lerp(prevKeyFrame.Orientation, keyFrame.Orientation, iWeight);
                scale = MathHelper.Lerp(prevKeyFrame.Scale, keyFrame.Scale, iWeight);
            }
            else
            {
                translation = keyFrame.Translation;
                orientation = keyFrame.Orientation;
                scale = keyFrame.Scale;
            }
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    OnDispose();
                }
                mIsDisposed = true;
            }
        }

        protected virtual void OnDispose() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int FindKeyFrame(float time)
        {
            int start = 0;
            var keyframes = Anim.Keyframes;
            int cnt = keyframes.Count;

            if (10 > cnt)
            {
                for (int i = 0; i < cnt; ++i)
                {
                    float value = keyframes[i].Time;
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
                        float middleValue = keyframes[middle].Time;
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
    }

    public class MeshSpecialEffectObjectDesc : SpecialEffectObjectDesc
    {
        public Mesh Mesh { get; internal set; } //don't dispose - its content resource
        public ReadOnlyDictionary<NameId, Material> Materials { get; internal set; }

        public override SpecialEffectObject CreateObjectInstance(IAleServiceProvider services, Vector3 effectPos)
        {
            return new MeshSpecialEffectObject(this, effectPos);
        }
    }
    public class ParticleSystemSpecialEffectObjectDesc : SpecialEffectObjectDesc
    {
        public ParticleSystemDesc Psys { get; internal set; }//don't dispose - its content resource

        public override SpecialEffectObject CreateObjectInstance(IAleServiceProvider services, Vector3 effectPos)
        {
            return new ParticleSystemSpecialEffectObject(this, services.GetServiceWithCheck <IParticleSystemManager>(), effectPos);
        }
    }
    public class DummySpecialEffectObjectDesc : SpecialEffectObjectDesc
    {
        public override SpecialEffectObject CreateObjectInstance(IAleServiceProvider services, Vector3 effectPos)
        {
            return new DummySpecialEffectObject(this, effectPos);
        }
    }
}
