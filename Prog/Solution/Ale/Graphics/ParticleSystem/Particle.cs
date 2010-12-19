//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using Ale.Tools;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public class Particle : PoolableResource
    {
        #region Fields

        private static Random Random = new Random();

        public Vector3 Position;
        public Vector3 Direction;
        public float BirthTime;
        public float LifeDuration;
        public float Size;
        public float Rotation;
        public bool IsAlive;

        public float Seed;

        #endregion Fields

        #region Properties


        #endregion Properties

        #region Methods

        internal Particle(ParticlePool particleManager)
            :base(particleManager)
        {
            Seed = (float)Random.NextDouble();
        }

        public bool Update(float totalTime, float elapsedTime, ParticleEmitterDesc desc)
        {
            if(!IsAlive)
            {
                return false;
            }

            if (totalTime > BirthTime + LifeDuration)
            {
                IsAlive = false;
                return false;
            }

            float particleLerp = GetLerp(totalTime);
            Position += Direction * desc.GetParticleSpeed(particleLerp) * elapsedTime;

            return true;
        }

        public float GetLerp(float totalTime)
        {
            return (totalTime - BirthTime) / LifeDuration;
        }

        public int SetVertices(ParticleVertex[] vertices, int startVerticeIndex, float totalTime, ParticleEmitterDesc desc)
        {
            float particleLerp = GetLerp(totalTime);

            Rotation = desc.GetParticleRotation(particleLerp);
            Size = desc.GetParticleSize(particleLerp);

            for (int i = 0; i < 4; ++i)
            {
                vertices[startVerticeIndex].Position = Position;
                vertices[startVerticeIndex].Seed = Seed;
                vertices[startVerticeIndex].Lerp = particleLerp;
                vertices[startVerticeIndex].Rotation = Rotation;
                vertices[startVerticeIndex++].Size = Size;
            }
            return startVerticeIndex;
        }

        #endregion Methods


        protected internal override void OnInit(bool reusing)
        {
        }

        protected internal override void RealDispose()
        {
        }
    }
}
