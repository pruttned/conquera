using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Partice resource pool
    /// </summary>
    public class ParticlePool : ResourcePool
    {
        #region Fields

        #endregion Fields

        #region Methods

        public Particle AllocParticle()
        {
            return (Particle)Alloc();
        }

        /// <summary>
        /// Creates a new particle
        /// </summary>
        /// <returns></returns>
        protected override PoolableResource CreateNewResource()
        {
            return new Particle(this);
        }

        /// <summary>
        /// Called whenever is an used resource going to be returned to the pool
        /// </summary>
        /// <param name="poolableResource"></param>
        /// <returns>- Whether should be resource returned to the pool or it is in invalid state and can't be reused</returns>
        protected override bool OnResourceReturningToPool(PoolableResource poolableResource)
        {
            return true;
        }
        #endregion Methods
    }
}
