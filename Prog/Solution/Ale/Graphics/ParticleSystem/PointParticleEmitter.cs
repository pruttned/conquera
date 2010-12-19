using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SimpleOrmFramework;
using Ale.Content;

namespace Ale.Graphics
{
    public class PointParticleSystemDesc : ParticleEmitterDesc
    {
        public PointParticleSystemDesc(GraphicsDevice graphicsDevice, PointParticleEmitterSettings settings, ContentGroup content)
            : base(graphicsDevice, settings, content)
        {
        }

        protected override void GenerateParticlePosition(ref Vector3 emitterWorldPosition, out Vector3 particlePosition)
        {
            particlePosition =  emitterWorldPosition;
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class PointParticleEmitterSettings : ParticleEmitterSettings
    {
        public override ParticleEmitterDesc CreateParticleEmitterDesc(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            return new PointParticleSystemDesc(graphicsDevice, this, content);
        }
    }
}
