using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 5)]
    public class HexTerrainTileAtlasSettings : BaseDataObject
    {
        [DataProperty(Unique = true, NotNull = true)]
        public string Name { get; set; }

        [DataProperty(NotNull = true)]
        public long Material { get; set; }

        /// <summary>
        /// Size in tiles
        /// </summary>
        [DataProperty(NotNull = true)]
        public int Size { get; set; }

        [DataProperty(NotNull = true)]
        public float TextureCellSpacing { get; set; }
    }
}
