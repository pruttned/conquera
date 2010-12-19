using System;
using System.Collections.Generic;
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework.Content;
using SimpleOrmFramework;

namespace Ale.Content
{
    /// <summary>
    /// Loader for material asset
    /// </summary>
    public class MaterialLoader : BaseAssetLoader<MaterialSettings>
    {
        protected override object CreateDesc(MaterialSettings settings, ContentGroup contentGroup)
        {
            return new Material(settings, contentGroup);
        }

        protected override string GetName(MaterialSettings settings)
        {
            return settings.Name;
        }
    }
}
