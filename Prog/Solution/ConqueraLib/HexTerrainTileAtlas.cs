using System;
using System.Collections.Generic;
using System.Text;
using Ale.Content;
using Ale.Graphics;
using Ale.Tools;

namespace Conquera
{
    [NonContentPipelineAsset(typeof(HexTerrainTileAtlasLoader))]
    public class HexTerrainTileAtlas
    {
        public NameId Name { get; private set; }

        public Material Material { get; private set; }

        /// <summary>
        /// Size in tiles
        /// </summary>
        public int Size { get; private set; }

        public float TextureCellSpacing { get; private set; }

        public HexTerrainTileAtlas(HexTerrainTileAtlasSettings settings, ContentGroup contentGroup)
        {
            Name = settings.Name;
            Material = contentGroup.Load<Material>(settings.Material);
            Size = settings.Size;
            TextureCellSpacing = settings.TextureCellSpacing;
        }
    }
}
