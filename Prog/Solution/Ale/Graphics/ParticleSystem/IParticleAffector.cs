using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public interface IParticleAffector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emitter"></param>
        /// <param name="totalTime"></param>
        /// <param name="elapsedTime">Elapsed time from last pSys update</param>
        void AffectParticles(ParticleEmitter emitter, ref Vector3 emitterWorldPosition, float totalTime, float elapsedTime);
    }
}
