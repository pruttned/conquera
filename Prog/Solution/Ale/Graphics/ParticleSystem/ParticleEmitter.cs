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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using SimpleOrmFramework;
using Microsoft.Xna.Framework.Content;
using System.Collections.ObjectModel;

namespace Ale.Graphics
{
    public class ParticleEmitter : Renderable, IRenderableUnit, IEnumerable<Particle>
    {
        #region Types
        struct Enumerator : IEnumerator<Particle>
        {
            Particle[] mParticles;
            int mIndex;
            int mParticleCntToIterate;
            int mActiveParticleCnt;

            public Particle Current
            {
                get { return mParticles[mIndex]; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return mParticles[mIndex]; }
            }

            internal Enumerator(Particle[] particles, int activeParticleCnt)
            {
                mParticles = particles;
                mIndex = -1;
                mParticleCntToIterate = mActiveParticleCnt = activeParticleCnt;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (0 == mParticleCntToIterate)
                {
                    return false;
                }

                do
                {
                    mIndex++;
                    if (mParticles.Length <= mIndex)
                    {
                        return false;
                    }
                } while (null == mParticles[mIndex] || !mParticles[mIndex].IsAlive);

                mParticleCntToIterate--;

                return true;
            }

            public void Reset()
            {
                mIndex = -1;
                mParticleCntToIterate = mActiveParticleCnt;
            }
        }
        #endregion Types

        private static ParticleToCameraDistanceComparer ParticleToCameraDistanceComparer = new ParticleToCameraDistanceComparer();

        private static ParticleVertex[] mParticleVertices;
        private static ParticlePool ParticlePool = new ParticlePool();

        private float mStartTime = -1;
        private ParticleEmitterDesc mDesc;
        private Particle[] mParticles = null;

        private float mEmissionReminder = 0;
        private float mLastUpdateTime = -1;

        private long mLastUpdateFrameNum = -1;
        private long mLastRenderedFrameNum = -1;

        private int mActiveParticleCnt = 0;

        private bool mIsActive = true;
        private bool mFirstUpdate = true;

        private bool mDeactivateAfterCycle;

        private ParticleSystemManager mParticleSystemManager;
        private ICamera mSortedForCamera = null;

        private ParticleSystem mParticleSystem;
        private TransientQuadGeometry<ParticleVertex> mTransientQuadGeometry = new TransientQuadGeometry<ParticleVertex>();


        public bool IsActive
        {
            get { return mIsActive; }
            set { mIsActive = value; }
        }

        public bool IsLoaded
        {
            get { return (null != mParticles); }
        }

        #region IRenderableUnit

        public Renderable ParentRenderable
        {
            get { return this; }
        }

        public Material Material
        {
            get { return mDesc.Material; }
        }

        #endregion IRenderableUnit


        internal ParticleEmitter(ParticleSystemManager particleSystemManager, ParticleEmitterDesc desc, ParticleSystem particleSystem, bool deactivateAfterCycle)
            :base(new BoundingSphere(), false)
        {
            mParticleSystemManager = particleSystemManager;
            mDesc = desc;
            mParticleSystem = particleSystem;
            mDeactivateAfterCycle = deactivateAfterCycle;

            Position = desc.RelativePosition;
        }

        #region IRenderableUnit

        public void Render(AleGameTime gameTime)
        {
            mTransientQuadGeometry.Draw();
        }

        public void UpdateMaterialEffectParameters()
        {
            mDesc.UpdateMaterialEffectParameters();
        }

        #endregion IRenderableUnit

        public void Unload()
        {
            if (IsLoaded)
            {
                foreach (Particle particle in mParticles)
                {
                    if (null != particle)
                    {
                        particle.Dispose();
                    }
                }
                mParticles = null;
                mLastUpdateTime = -1;
                mStartTime = -1;
                mActiveParticleCnt = 0;
                mEmissionReminder = 0;

                mParticleSystem.OnEmitterUnload();
            }
        }

        /// <summary>
        /// Reloade unloaded particle sytem
        /// </summary>
        internal void Load(AleGameTime gameTime)
        {
            if (!IsLoaded)
            {
                mParticles = new Particle[mDesc.MaxParticleCnt];

                mParticleSystem.OnEmitterLoad();

                FastForward(gameTime);
            }
        }

        public bool Update(AleGameTime gameTime)
        {
            if (IsLoaded && mLastUpdateFrameNum != gameTime.FrameNum)
            {
                float totalTime = gameTime.TotalTime;
                float elapsedTime = totalTime - mLastUpdateTime;

                if (mFirstUpdate || elapsedTime > 0.033333f) //limit fps to 30fps
                {
                    if (mFirstUpdate)
                    {
                        mFirstUpdate = false;
                        mStartTime = totalTime;
                        mLastUpdateTime = totalTime;
                    }

                    mLastUpdateFrameNum = gameTime.FrameNum;

                    if (elapsedTime > mDesc.SampleRateOnFastForward) //assure min update fps and catch up if psys was not update for a while
                    {
                        if (elapsedTime > mDesc.MaxFastForwardTime) //constrain max time to catch up
                        {
                            elapsedTime = 3;
                            mLastUpdateTime = totalTime - elapsedTime;
                        }

                        for (float time = mLastUpdateTime; time <= totalTime; time += mDesc.SampleRateOnFastForward)
                        {
                            if (!IsLoaded)
                            {
                                return false;
                            }
                            Update(time);
                        }
                    }
                    else
                    {
                        Update(totalTime);
                    }

                    return true;
                }
            }
            return false;
        }

        #region IEnumerable

        public IEnumerator<Particle> GetEnumerator()
        {
            return new Enumerator(mParticles, mActiveParticleCnt);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(mParticles, mActiveParticleCnt);
        }

        #endregion IEnumerable

        protected static ParticleVertex[] GetParticleVerticesArray(int minParticleCnt)
        {
            int verticeCnt = minParticleCnt * 4;
            if (null == mParticleVertices || mParticleVertices.Length < verticeCnt)
            {
                mParticleVertices = new ParticleVertex[verticeCnt];

                int vIndex = 0;
                for (int i = 0; i < minParticleCnt; ++i)
                {
                    mParticleVertices[vIndex++].NormalizedUv = new HalfVector2(0, 0);
                    mParticleVertices[vIndex++].NormalizedUv = new HalfVector2(1, 0);
                    mParticleVertices[vIndex++].NormalizedUv = new HalfVector2(0, 1);
                    mParticleVertices[vIndex++].NormalizedUv = new HalfVector2(1, 1);
                }

            }
            return mParticleVertices;
        }

        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            if (mLastRenderedFrameNum != gameTime.FrameNum) //first render a current frame
            {
                Update(gameTime);

                float totalTime = gameTime.TotalTime;
                //prepare geometry  (mTransientQuadGeometry will be allocated for an entire frame)
                if (0 < mActiveParticleCnt)
                {
                    if (renderer.EnqueueRenderable(this)) //will be realy rendered
                    {

                        if (mDesc.Sort) //new frame - must sort
                        {
                            //sort
                            ParticleToCameraDistanceComparer.CameraPosition = renderer.ActiveCamera.WorldPosition;
                            Array.Sort(mParticles, ParticleToCameraDistanceComparer);
                            mSortedForCamera = renderer.ActiveCamera;
                        }

                        int vIndex = 0;
                        int particlesToRender = mActiveParticleCnt;
                        ParticleVertex[] particleVertices = GetParticleVerticesArray(mActiveParticleCnt);
                        for (int i = 0; i < mParticles.Length && 0 != particlesToRender; ++i)
                        {
                            Particle particle = mParticles[i];
                            if (null != particle)
                            {
                                vIndex = particle.SetVertices(particleVertices, vIndex, totalTime, mDesc);
                            }
                        }

                        mParticleSystemManager.ParticleDynamicGeometryManager.AllocGeometry(particleVertices, 0, particlesToRender * 4, ref mTransientQuadGeometry);
                        mLastRenderedFrameNum = gameTime.FrameNum;
                    }
                }
            }
            else
            {
                if (0 < mActiveParticleCnt)
                {
                    if (renderer.EnqueueRenderable(this))
                    {
                        if (mDesc.Sort && mSortedForCamera != renderer.ActiveCamera)
                        {
                            //sort
                            ParticleToCameraDistanceComparer.CameraPosition = renderer.ActiveCamera.WorldPosition;
                            Array.Sort(mParticles, ParticleToCameraDistanceComparer);
                            mSortedForCamera = renderer.ActiveCamera;
                        }
                    }
                }
            }
        }

        private void FastForward(AleGameTime gameTime)
        {
            //fast forward
            float realTotalTime = gameTime.TotalTime;
            mLastUpdateTime = realTotalTime - mDesc.FastForwardTimeOnLoad;
            mStartTime = mLastUpdateTime;
            mFirstUpdate = false;

            Update(gameTime);
        }

        private void Update(float totalTime)
        {
            float cycleTime = mDesc.CycleTime;

            if (mDeactivateAfterCycle && (totalTime > (mStartTime + cycleTime)))
            {
                mIsActive = false;
            }

            if (!mIsActive && 0 == mActiveParticleCnt)
            {
                Unload();
            }
            else
            {
                float duration = totalTime - mStartTime;
                float lerp = (duration % cycleTime) / cycleTime;

                float elapsedTime = totalTime - mLastUpdateTime;
                mLastUpdateTime = totalTime;

                mEmissionReminder += mDesc.GetEmissionRate(lerp) * elapsedTime;
                int particleCntToEmit = (int)mEmissionReminder;
                mEmissionReminder -= particleCntToEmit;

                //update particles
                mDesc.ApplyParticleAffectors(this, totalTime, elapsedTime);
                int particlesToUpdate = mActiveParticleCnt;
                for (int i = 0; i < mParticles.Length && 0 != particlesToUpdate; ++i)
                {
                    if (null != mParticles[i])
                    {
                        particlesToUpdate--;
                        if (!mParticles[i].Update(totalTime, elapsedTime, mDesc))
                        {
                            //particle has died
                            mActiveParticleCnt--;
                            mParticles[i].Dispose(); //return to pool
                            mParticles[i] = null;
                        }
                    }
                }

                if (mIsActive)
                {
                    //particle generation
                    if (particleCntToEmit + mActiveParticleCnt > mDesc.MaxParticleCnt)
                    {
                        particleCntToEmit = mDesc.MaxParticleCnt - mActiveParticleCnt;
                    }
                    Vector3 worldPosition = WorldBoundsCenter;
                    for (int i = 0; i < mParticles.Length && 0 != particleCntToEmit; ++i)
                    {
                        if (null == mParticles[i]) //found free particle
                        {
                            mParticles[i] = ParticlePool.AllocParticle();
                            mDesc.InitParticle(totalTime, ref worldPosition, mParticles[i]);
                            particleCntToEmit--;
                            mActiveParticleCnt++;
                        }
                    }
                }
            }
        }

        internal void ComputeBounds(out BoundingSphere bounds)
        {
            if (0 == mActiveParticleCnt)
            {
                bounds = new BoundingSphere();
            }
            else
            {
                bounds = BoundingSphere.CreateFromPoints(new ParticlePosEnumerable(mParticles, mActiveParticleCnt));
                bounds.Center -= mParticleSystem.WorldPosition;
                bounds.Radius += mDesc.MaxParticleSize;
            }

        }

        internal void UpdatePosition(ref Vector3 pSysWorldPosition)
        {
            Position = pSysWorldPosition + mDesc.RelativePosition;
        }
    }

}
