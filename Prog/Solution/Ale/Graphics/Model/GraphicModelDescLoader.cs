using System;
using Ale.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// Loader for GraphicModelDesc asset
    /// </summary>
    public class GraphicModelDescLoader : BaseAssetLoader<GraphicModelSettings>
    {
        protected override object CreateDesc(GraphicModelSettings settings, ContentGroup contentGroup)
        {
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)contentGroup.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            return new GraphicModelDesc(graphicsDevice, settings, contentGroup);
        }

        protected override string GetName(GraphicModelSettings settings)
        {
            return settings.Name;
        }
    }
}
