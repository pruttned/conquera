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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Tools;
using Ale.Content;
using Ale.Scene;

namespace Ale.SpecialEffects
{
    public abstract class SpecialEffectObject : IDisposable
    {
        private Vector3 mPosition;
        protected Vector3 mEffectPosition;
        private bool mIsDisposed = false;
        private bool mFirstEnqueRenderableUnits = true;
        
        public Vector3 Position
        {
            get { return mPosition; }
        }
        public SpecialEffectObjectDesc Desc { get; private set; }
        protected Vector3 EffectPosition
        {
            get { return mEffectPosition; }
        }


        public SpecialEffectObject(SpecialEffectObjectDesc desc, Vector3 effectPosition)
        {
            Desc = desc;
            mEffectPosition = effectPosition;

            mPosition = effectPosition + desc.Position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>false - has been destroyed and its ready to be removed</returns>
        public bool EnqueRenderableUnits(IRenderer renderer, AleGameTime gameTime, float timeInAnimation, bool firstInFrame)
        {
            //anim update
            if (null == Desc.Anim)
            {
                if (mFirstEnqueRenderableUnits)
                {
                    Quaternion orientation = Desc.Orientation;
                    UpdateTransformation(ref mPosition, ref orientation, Desc.Scale);
                    mFirstEnqueRenderableUnits = false;
                }
            }
            else
            {
                if(firstInFrame)
                {
                    Quaternion orientation;
                    float scale;
                    Desc.GetTransformation(timeInAnimation, out mPosition, out orientation, out scale);
                    Vector3.Add(ref mPosition, ref mEffectPosition, out mPosition);
                    UpdateTransformation(ref mPosition, ref orientation, scale);
                }
            }

            return EnqueRenderableUnitsImpl(renderer, gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether can be object immediately removed (true) or it will be removed when EnqueRenderableUnits return false</returns>
        public abstract bool Destroy();

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void UpdateTransformation(ref Vector3 position, ref Quaternion orientation, float scale);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        /// <returns>false - has been destroyed and its ready to be removed</returns>
        protected abstract bool EnqueRenderableUnitsImpl(IRenderer renderer, AleGameTime gameTime);

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

        protected virtual void OnDispose() { }

    }

    public class MeshSpecialEffectObject : SpecialEffectObject
    {
        private GraphicModel mGraphicModel;

        public MeshSpecialEffectObject(MeshSpecialEffectObjectDesc desc, Vector3 effectPosition)
            :base(desc, effectPosition)
        {
            mGraphicModel = new GraphicModel(desc.Mesh, desc.Materials);
            mGraphicModel.Position = desc.Position + effectPosition;
        }

        public override bool Destroy()
        {
            return true;
        }

        protected override void UpdateTransformation(ref Vector3 position, ref Quaternion orientation, float scale)
        {
            mGraphicModel.SetTransformation(ref position, ref orientation, scale);
        }

        protected override bool EnqueRenderableUnitsImpl(IRenderer renderer, AleGameTime gameTime)
        {
            mGraphicModel.EnqueRenderableUnits(renderer, gameTime);
            return true;
        }

        protected override void OnDispose()
        {
            mGraphicModel.Dispose();
            base.OnDispose();
        }
    }

    public class ParticleSystemSpecialEffectObject : SpecialEffectObject
    {
        private ParticleSystem mParticleSystem;

        public ParticleSystemSpecialEffectObject(ParticleSystemSpecialEffectObjectDesc desc, IParticleSystemManager particleSystemManager, Vector3 effectPosition)
            :base(desc, effectPosition)
        {
            mParticleSystem = particleSystemManager.CreateParticleSystem(desc.Psys);
            mParticleSystem.Position = desc.Position+effectPosition;
        }

        public override bool Destroy()
        {
            mParticleSystem.IsEnabled = false;
            return false;
        }

        protected override void UpdateTransformation(ref Vector3 position, ref Quaternion orientation, float scale)
        {
            mParticleSystem.SetTransformation(ref position, ref orientation, scale);
        }

        protected override bool EnqueRenderableUnitsImpl(IRenderer renderer, AleGameTime gameTime)
        {
            mParticleSystem.EnqueRenderableUnits(renderer, gameTime);

            return (mParticleSystem.IsLoaded);
        }

        protected override void OnDispose()
        {
            mParticleSystem.Dispose();
            base.OnDispose();
        }
    }

    public class DummySpecialEffectObject : SpecialEffectObject
    {
        public DummySpecialEffectObject(DummySpecialEffectObjectDesc desc, Vector3 effectPosition)
            :base(desc, effectPosition)
        {
        }

        protected override void UpdateTransformation(ref Vector3 position, ref Quaternion orientation, float scale)
        {
        }

        public override bool Destroy()
        {
            return true;
        }
    
        protected override bool EnqueRenderableUnitsImpl(IRenderer renderer, AleGameTime gameTime)
        {
            return true;
        }
    }
}
