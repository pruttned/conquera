using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    public class PointParticleSystemDesc : ParticleEmitterDesc
    {
        public PointParticleSystemDesc(GraphicsDevice graphicsDevice, ParticleEmitterSettings particleSystemSettings, ContentManager content)
            : base(graphicsDevice, particleSystemSettings, content)
        {
        }

        protected override Vector3 GenerateParticlePosition(ref Vector3 particleSystemPosition)
        {
            return particleSystemPosition;
        }
    }
}
