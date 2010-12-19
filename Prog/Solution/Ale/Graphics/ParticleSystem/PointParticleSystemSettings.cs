using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public class PointParticleSystemSettings : ParticleEmitterSettings
    {
        public override ParticleEmitterDesc CreateParticleSystemDesc(GraphicsDevice graphicsDevice, ContentManager content)
        {
            return new PointParticleSystemDesc(graphicsDevice, this, content);
        }
    }
}
