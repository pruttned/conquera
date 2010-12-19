using System;
using System.Collections.Generic;
using System.Text;
using Ale.Content;

namespace Conquera
{
    public class HexTerrainTileDescLoader : BaseAssetLoader<HexTerrainTileSettings>
    {
        protected override object CreateDesc(HexTerrainTileSettings settings, ContentGroup contentGroup)
        {
            return settings.CreateDesc(contentGroup);
        }

        protected override string GetName(HexTerrainTileSettings settings)
        {
            return settings.Name;
        }
    }
}
