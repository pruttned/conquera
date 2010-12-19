using System;
using Ale.Content;

namespace Ale.Graphics
{
    public class ParticleSystemRenderableFactory : IRenderableFactory
    {
        private ParticleSystemManager mParticleSystemManager;

        public int Id
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "ParticleSys"; }
        }

        public ParticleSystemRenderableFactory(ParticleSystemManager particleSystemManager)
        {
            mParticleSystemManager = particleSystemManager;
        }

        public Renderable CreateRenderable(string name, ContentGroup content)
        {
            ParticleSystemDesc desc = content.Load<ParticleSystemDesc>(name);
            return mParticleSystemManager.CreateParticleSystem(desc);
        }

        public Renderable CreateRenderable(long id, ContentGroup content)
        {
            ParticleSystemDesc desc = content.Load<ParticleSystemDesc>(id);
            return mParticleSystemManager.CreateParticleSystem(desc);
        }
    }
}
