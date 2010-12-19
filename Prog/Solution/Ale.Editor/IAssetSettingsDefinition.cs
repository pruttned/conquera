using Ale.Graphics;
using Microsoft.Xna.Framework.Content;
using Ale.Content;

namespace Ale.Editor
{
    public abstract class AssetSettingsDefinitionBase<T> : IAssetSettingsDefinition
    {
        public object GetSettingsAsObject(ContentGroup content)
        {
            return GetSettings(content);
        }

        public abstract T GetSettings(ContentGroup content);
    }

    public interface IAssetSettingsDefinition
    {
        object GetSettingsAsObject(ContentGroup content);
    }
}
