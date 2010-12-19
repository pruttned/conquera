using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public class ParticleDynamicGeometryManager : DynamicQuadGeometryManager<ParticleVertex>
    {
        public ParticleDynamicGeometryManager(GraphicsDeviceManager graphicsDeviceManager)
            : base(graphicsDeviceManager, new VertexDeclaration(graphicsDeviceManager.GraphicsDevice, ParticleVertex.VertexElements))
        {
        }
    }
}
