using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Ale.Content;

namespace Ale.Graphics
{
    [NonContentPipelineAsset(typeof(ParticleSystemLoader))]
    public class ParticleSystemDesc
    {
        private ReadOnlyCollection<ParticleEmitterDesc> mEmitters;

        public ReadOnlyCollection<ParticleEmitterDesc> Emitters
        {
            get { return mEmitters; }
        }

        public ParticleSystemDesc(GraphicsDevice graphicsDevice, ParticleSystemSettings settings, ContentGroup content)
        {
            ParticleEmitterDesc[] emitters = new ParticleEmitterDesc[settings.Emitters.Count];

            for(int i = 0; i < emitters.Length; ++i) 
            {
                emitters[i] = settings.Emitters[i].CreateParticleEmitterDesc(graphicsDevice, content);
            }

            mEmitters = new ReadOnlyCollection<ParticleEmitterDesc>(emitters);
        }
    }
}
