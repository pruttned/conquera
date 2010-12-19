using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SimpleOrmFramework;

namespace Ale.Graphics
{
    public class GravityParticleParticleAffector : IParticleAffector
    {
        private Vector3 mGravityDirection;

        public GravityParticleParticleAffector(GravityParticleAffectorSettings settings)
        {
            mGravityDirection = settings.GravityDirection;
        }

        public void AffectParticles(ParticleEmitter emitter, ref Vector3 emitterWorldPosition, float totalTime, float elapsedTime)
        {
            foreach (Particle particle in emitter)
            {
                particle.Direction += mGravityDirection * elapsedTime;
            }
        }
    }
}
