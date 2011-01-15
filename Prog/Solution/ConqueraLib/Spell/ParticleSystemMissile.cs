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
using System.Collections.ObjectModel;
using Ale.Gui;
using Ale;
using System.Collections.Generic;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    public class ParticleSystemMissile : IDisposable
    {
        public delegate void OnHitHandler(ParticleSystemMissile missile, GameUnit target);

        public event OnHitHandler OnHit;

        private bool mIsDisposed = false;
        private ParticleSystem mParticleSystem;
        private Vector3LinearAnimator mPosAnimator = new Vector3LinearAnimator();

        public GameUnit Target { get; private set; }
        public GameScene GameScene { get { return Target.GameScene; } }

        public ParticleSystemMissile(GameUnit target, Vector3 srcPos, Vector3 destPos, string pSysName, float speed)
        {
            Target = target;
            mParticleSystem = GameScene.ParticleSystemManager.CreateParticleSystem(GameScene.Content, pSysName);
            mPosAnimator.Animate(speed, srcPos, destPos);
            mParticleSystem.Position = mPosAnimator.CurrentValue;

            GameScene.Octree.AddObject(mParticleSystem);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    mParticleSystem.Dispose();
                }
                mIsDisposed = true;
            }
        }

        public bool Update(AleGameTime time)
        {
            if (mPosAnimator.Update(time))
            {
                mParticleSystem.Position = mPosAnimator.CurrentValue;
            }
            else
            {
                if (mParticleSystem.IsEnabled)
                {
                    mParticleSystem.IsEnabled = false;
                    if (null != OnHit)
                    {
                        OnHit.Invoke(this, Target);
                    }
                }
                else
                {
                    if (!mParticleSystem.IsLoaded)
                    {
                        GameScene.Octree.RemoveObject(mParticleSystem);
                        return false;
                    }
                }
            }
            return true;
        }
    }

}
