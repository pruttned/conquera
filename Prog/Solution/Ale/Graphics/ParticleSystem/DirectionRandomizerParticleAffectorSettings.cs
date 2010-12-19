using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;
using SimpleOrmFramework;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public class DirectionRandomizerParticleAffectorSettings : ParticleAffectorSettings
    {
        [DataProperty]
        public float DirectionVariation { get; set; }
        [DataProperty]
        public float BounceDistanceFromPsys { get; set; }
        [DataProperty]
        public float MaxDistanceFromPsys { get; set; }
        [DataProperty]
        public float DirectionCahngeProbability { get; set; }

        public DirectionRandomizerParticleAffectorSettings()
        {
            DirectionVariation = 1;
            BounceDistanceFromPsys = 10;
            MaxDistanceFromPsys = 12;
            DirectionCahngeProbability = 0.8f;
        }

        public override IParticleAffector CreateParticleAffector()
        {
            return new DirectionRandomizerParticleAffector(this);
        }
    }
}
