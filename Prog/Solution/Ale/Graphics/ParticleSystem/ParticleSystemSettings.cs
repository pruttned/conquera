using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public class ParticleSystemSettings : BaseDataObject
    {
        [DataProperty(CaseSensitive = false, Unique = true, NotNull = true)]
        public string Name { get; set; }

        [DataListProperty(NotNull = true)]
        public List<ParticleEmitterSettings> Emitters { get; private set; }

        public ParticleSystemSettings()
        {
            Emitters = new List<ParticleEmitterSettings>();
        }
    }
}
