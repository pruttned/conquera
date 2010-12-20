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
using Ale.Tools;

namespace Ale.Graphics
{
    public sealed class ParticleSystem : Renderable, IDisposable, IDynamicallyLoadableObject
    {
        #region IDynamicallyLoadableObject

        public event DynamicallyLoadableObjectAfterLoadHandler AfterLoad;
        public event EventHandler Disposing;

        #endregion IDynamicallyLoadableObject
        
        private ParticleSystemDesc mDesc;
        private ParticleEmitter[] mEmitters;
        private long mLastRenderFrameNum = -1;
        private float mNextBoundsUpdateTime = -1;
        private bool mLoop;
        private int mLoadedEmitterCnt;
        private bool mIsDisposed = false;
        private bool mHasDefaultBounds = true;

        #region IDynamicallyLoadableObject

        public long LastRenderFrameNum
        {
            get { return mLastRenderFrameNum; }
        }

        public bool IsLoaded
        {
            get { return (0 < mLoadedEmitterCnt); }
        }

        #endregion IDynamicallyLoadableObject

        internal ParticleSystem(ParticleSystemManager particleSystemManager, ParticleSystemDesc desc, bool loop)
            :base(new BoundingSphere(Vector3.Zero, 1), false)
        {
            mDesc = desc;
            mEmitters = new ParticleEmitter[desc.Emitters.Count];

            for (int i = 0; i < mEmitters.Length; ++i)
            {
                mEmitters[i] = new ParticleEmitter(particleSystemManager, desc.Emitters[i], this, !loop);
            }
            mLoop = loop;
        }

        internal void OnEmitterUnload()
        {
            mLoadedEmitterCnt--;
        }

        internal void OnEmitterLoad()
        {
            mLoadedEmitterCnt++;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                Unload();

                if (null != Disposing)
                {
                    Disposing.Invoke(this, null);
                }

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            if (IsVisible)
            {
                if (!IsLoaded)
                {
                    Load(gameTime);
                }

                mLastRenderFrameNum = gameTime.FrameNum;

                for (int i = 0; i < mEmitters.Length; ++i)
                {
                    mEmitters[i].EnqueRenderableUnits(renderer, gameTime);
                }

                UpdateBounds(gameTime.TotalTime);

                mLastRenderFrameNum = gameTime.FrameNum;
            }
        }

        #region IDynamicallyLoadableObject

        public void Unload()
        {
            if (IsLoaded)
            {
                foreach (ParticleEmitter emitter in mEmitters)
                {
                    emitter.Unload();
                }
            }
        }

        /// <summary>
        /// Reloade unloaded particle sytem
        /// </summary>
        internal void Load(AleGameTime gameTime)
        {
            foreach (ParticleEmitter emitter in mEmitters)
            {
                emitter.Load(gameTime);
            }

            if (null != AfterLoad)
            {
                AfterLoad.Invoke(this);
            }
        }
        #endregion IDynamicallyLoadableObject

        protected override void OnVisibleChanged(bool visible)
        {
            if (!IsVisible)
            {
                Unload();
            }
        }

        protected override void OnWorldBoundsChanged()
        {
            if (null != mEmitters) //base ctor
            {
                Vector3 worldPosition = WorldPosition;
                foreach (ParticleEmitter emitter in mEmitters)
                {
                    emitter.UpdatePosition(ref worldPosition);
                }
            }
        }

        private void UpdateBounds(float totalTime)
        {
            if (totalTime > mNextBoundsUpdateTime)
            {
                BoundingSphere newBounds = new BoundingSphere();
                bool first = true;
                for (int i = 0; i < mEmitters.Length; ++i)
                {
                    BoundingSphere emitterBounds;
                    mEmitters[i].ComputeBounds(out emitterBounds);

                    if (0 < emitterBounds.Radius)
                    {
                        if (first)
                        {
                            newBounds = emitterBounds;
                            first = false;
                        }
                        else
                        {
                            BoundingSphere.CreateMerged(ref newBounds, ref emitterBounds, out newBounds);
                        }
                    }
                }

                if (0 < newBounds.Radius)
                {
                    if (mHasDefaultBounds)
                    {
                        Bounds = newBounds;
                    }
                    else
                    {
                        ContainmentType containmentType;
                        Bounds.Contains(ref newBounds, out containmentType);
                        if (ContainmentType.Contains != containmentType)
                        {
                            Bounds = newBounds;
                        }
                    }
                }
                else
                {
                    Bounds = new BoundingSphere(WorldPosition, 1.0f);
                }

                mNextBoundsUpdateTime = totalTime + AleMathUtils.GetRandomFloat(4, 1);
            }
        }
    }
}
