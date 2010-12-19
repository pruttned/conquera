using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// Loader for material asset
    /// </summary>
    public class ParticleSystemLoader : BaseAssetLoader<ParticleSystemSettings>
    {
        protected override object CreateDesc(ParticleSystemSettings settings, ContentGroup contentGroup)
        {
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)contentGroup.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            return new ParticleSystemDesc(graphicsDevice, settings, contentGroup);
        }

        protected override string GetName(ParticleSystemSettings settings)
        {
            return settings.Name;
        } 
    }
}
