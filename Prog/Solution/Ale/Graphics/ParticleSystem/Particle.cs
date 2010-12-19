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
