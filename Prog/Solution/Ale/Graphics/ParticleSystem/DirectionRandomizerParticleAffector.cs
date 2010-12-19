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
using System.Linq;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public class DirectionRandomizerParticleAffector : IParticleAffector
    {
        private float mDirectionVariation;
        private float mSquaredBounceDistanceFromPsys;
        private float mSquaredMaxDistanceFromPsys;
        private float mDirectionCahngeProbability;

        public DirectionRandomizerParticleAffector(DirectionRandomizerParticleAffectorSettings settings)
        {
            mDirectionVariation = settings.DirectionVariation;
            mSquaredBounceDistanceFromPsys = settings.BounceDistanceFromPsys * settings.BounceDistanceFromPsys;
            mSquaredMaxDistanceFromPsys = settings.MaxDistanceFromPsys * settings.MaxDistanceFromPsys;
            mDirectionCahngeProbability = settings.DirectionCahngeProbability;
        }

        public void AffectParticles(ParticleEmitter pSys, ref Vector3 emitterWorldPosition, float totalTime, float elapsedTime)
        {
            foreach (Particle particle in pSys)
            {
                float dist = (particle.Position - emitterWorldPosition).LengthSquared();

                if (dist > mSquaredMaxDistanceFromPsys)
                {
                    particle.IsAlive = false;
                }
                else
                {
                    if (AleMathUtils.Random.NextDouble() < mDirectionCahngeProbability)
                    {
                        //if (AleMathUtils.Random.NextDouble() < 0.9f)
                        //{
                        //    Vector3 perp;
                        //    AleMathUtils.GetPerpVector(ref particle.Direction, out perp);
                        //    perp *= 0.005f;
                        //    particle.Direction += perp;
                        //}
                        //else
                        //{
                        AleMathUtils.GetRandomVector3(ref particle.Direction, mDirectionVariation, out particle.Direction);
                        //}
                        //particle.Direction += new Vector3(
                        //    ((float)AleMathUtils.Random.NextDouble() - 0.5f) * mDirectionVariation * 2 * elapsedTime,
                        //    ((float)AleMathUtils.Random.NextDouble() - 0.5f) * mDirectionVariation * 2 * elapsedTime,
                        //    ((float)AleMathUtils.Random.NextDouble() - 0.5f) * mDirectionVariation * 2 * elapsedTime);


                        particle.Direction.Normalize();
                    }

                    if (dist > mSquaredBounceDistanceFromPsys)
                    {
                        particle.Direction = -particle.Direction;
                    }
                }
            }
        }
    }
}
