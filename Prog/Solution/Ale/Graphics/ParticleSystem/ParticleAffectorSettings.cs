using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public abstract class ParticleAffectorSettings : BaseDataObject
    {
        abstract public IParticleAffector CreateParticleAffector();
    }
}
