using System;
using Ale.Content;

namespace Ale.Scene
{
    public class OctreeSceneObjectDescLoader : BaseAssetLoader<OctreeSceneObjectSettings>
    {
        protected override object CreateDesc(OctreeSceneObjectSettings settings, ContentGroup contentGroup)
        {
            return new OctreeSceneObjectDesc(settings, contentGroup);
        }

        protected override string GetName(OctreeSceneObjectSettings settings)
        {
            return settings.Name;
        }
    }
}
