using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ale.Tools;
using SimpleOrmFramework;
using Ale.Content;

namespace Ale.Graphics
{
    public class AabbParticleEmitterDesc : ParticleEmitterDesc
    {
        private float mXSize;
        private float mYSize;
        private float mZSize;

        public AabbParticleEmitterDesc(GraphicsDevice graphicsDevice, AabbParticleEmitterSettings settings, ContentGroup content)
            : base(graphicsDevice, settings, content)
        {
            mXSize = settings.XSize;
            mYSize = settings.YSize;
            mZSize = settings.ZSize;
        }

        protected override void GenerateParticlePosition(ref Vector3 emitterWorldPosition, out Vector3 particlePosition)
        {
            particlePosition.X = AleMathUtils.GetRandomFloat(emitterWorldPosition.X, mXSize);
            particlePosition.Y = AleMathUtils.GetRandomFloat(emitterWorldPosition.Y, mYSize);
            particlePosition.Z = AleMathUtils.GetRandomFloat(emitterWorldPosition.Z, mZSize);
        }
    }


    [DataObject(MaxCachedCnt = 5)]
    public class AabbParticleEmitterSettings : ParticleEmitterSettings
    {
        [DataProperty]
        public float XSize { get; set; }
        [DataProperty]
        public float YSize { get; set; }
        [DataProperty]
        public float ZSize { get; set; }

        public override ParticleEmitterDesc CreateParticleEmitterDesc(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            return new AabbParticleEmitterDesc(graphicsDevice, this, content);
        }
    }
}
