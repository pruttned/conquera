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
