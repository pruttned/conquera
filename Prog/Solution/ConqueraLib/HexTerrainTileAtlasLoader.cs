using System;
using System.Collections.Generic;
using System.Text;
using Ale.Content;

namespace Conquera
{
    public class HexTerrainTileAtlasLoader : BaseAssetLoader<HexTerrainTileAtlasSettings>
    {
        protected override object CreateDesc(HexTerrainTileAtlasSettings settings, ContentGroup contentGroup)
        {
            return new HexTerrainTileAtlas(settings, contentGroup);
        }

        protected override string GetName(HexTerrainTileAtlasSettings settings)
        {
            return settings.Name;
        }
    }
}
