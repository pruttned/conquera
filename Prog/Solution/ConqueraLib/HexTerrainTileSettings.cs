//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using Ale.Tools;
using Ale.Content;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(Point), typeof(FieldCustomBasicTypeProvider<Point>))]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    abstract public class HexTerrainTileSettings : BaseDataObject
    {
        [DataProperty(NotNull = true, Unique=true)]
        public string Name { get; set; }

        [DataProperty(NotNull = true)]
        public string DisplayName { get; set; }

        [DataProperty(NotNull = true, WeakReference=true)]
        public StringResource Description { get; set; }

        [DataProperty(NotNull = true)]
        public bool IsPassable { get; set; }

        [DataProperty(NotNull = true)]
        public long WallGraphicModel { get; set; }

        [DataListProperty(NotNull = true)]
        public List<long> GraphicModels { get; private set; }

        /// <summary>
        /// This graphic model will be never placed into the static geometry
        /// </summary>
        [DataProperty(NotNull = false)]
        public long ActiveGraphicModel { get; set; }

        /// <summary>
        /// This graphic model will be never placed into the static geometry
        /// </summary>
        [DataProperty(NotNull = false)]
        public long InactiveGraphicModel { get; set; }

        [DataProperty(NotNull = true)]
        public long HexTerrainTileAtlas { get; set; }

        [DataProperty(NotNull = true)]
        public Point TileIndex { get; set; }

        [DataProperty(NotNull = true)]
        public Vector3 UnitPosition { get; set; }

        /// <summary>
        /// Per turn healt points increment for a unit that occupies the tile
        /// </summary>
        [DataProperty(NotNull = true)]
        public int HpIncrement  { get; set; }

        [DataProperty(NotNull = true)]
        public string Icon { get; set; }

        public HexTerrainTileSettings()
        {
            GraphicModels = new List<long>();
            InactiveGraphicModel = -1;
            ActiveGraphicModel = -1;
        }
          
        public abstract HexTerrainTileDesc CreateDesc(ContentGroup content);
    }

    [DataObject(MaxCachedCnt = 5)]
    public class CastleTileSettings : HexTerrainTileSettings
    {
        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new CastleTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class DimensionGateTileSettings : HexTerrainTileSettings
    {
        [DataListProperty(NotNull = true)]
        public List<string> GameCards { get; private set; }

        public DimensionGateTileSettings()
        {
            GameCards = new List<string>();
        }

        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new DimensionGateTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class GoldMineTileSettings : HexTerrainTileSettings
    {
        [DataProperty(NotNull = true)]
        public int GoldIncrement { get; set; }

        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new GoldMineTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class LandTileSettings : HexTerrainTileSettings
    {
        [DataProperty]
        public string HpIncNegatingTemple { get; set; }

        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new LandTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class VillageTileSettings : HexTerrainTileSettings
    {
        [DataProperty(NotNull = true)]
        public int MaxUnitCntIncrement { get; set; }
        
        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new VillageTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class LandTempleTileSettings : HexTerrainTileSettings
    {
        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new LandTempleTileDesc(this, content);
        }
    }

    //[DataObject(MaxCachedCnt = 5)]
    //public class ShrineTileSettings : HexTerrainTileSettings
    //{

    //}

}
