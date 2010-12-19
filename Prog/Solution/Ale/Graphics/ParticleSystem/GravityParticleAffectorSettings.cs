using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SimpleOrmFramework;
using Ale.Tools;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    public class GravityParticleAffectorSettings : ParticleAffectorSettings
    {
        [DataProperty]
        public Vector3 GravityDirection { get; set; }

        public GravityParticleAffectorSettings()
        {
            GravityDirection = new Vector3(0, 0, -1);
        }

        public override IParticleAffector CreateParticleAffector()
        {
            return new GravityParticleParticleAffector(this);
        }
    }
}
