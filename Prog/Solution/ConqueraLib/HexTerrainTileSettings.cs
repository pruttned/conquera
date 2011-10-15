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
        [DataProperty(NotNull = true, Unique = true)]
        public string Name { get; set; }

        [DataProperty(NotNull = true)]
        public string DisplayName { get; set; }

        [DataListProperty(NotNull = true)]
        public List<long> GraphicModels { get; private set; }

        public HexTerrainTileSettings()
        {
            GraphicModels = new List<long>();
        }

        public abstract HexTerrainTileDesc CreateDesc(ContentGroup content);
    }

    [DataObject(MaxCachedCnt = 5)]
    public class GapWallSettings : BaseDataObject
    {
        [DataProperty(NotNull = true, Unique = true)]
        public string Name { get; set; }

        [DataProperty(NotNull = true)]
        public List<long> GraphicModelSamples { get; set; }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class GapHexTerrainTileSettings : HexTerrainTileSettings
    {
        [DataProperty(NotNull = true)]
        public long GapWallGraphicModel { get; set; }

        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new GapHexTerrainTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(Point), typeof(FieldCustomBasicTypeProvider<Point>))]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    abstract public class GroundHexTerrainTileSettings : HexTerrainTileSettings
    {
        [DataProperty(NotNull = true)]
        public bool IsPassable { get; set; }

        [DataProperty(NotNull = true)]
        public long HexTerrainTileAtlas { get; set; }

        [DataProperty(NotNull = true)]
        public Point TileIndex { get; set; }

        [DataProperty(NotNull = true)]
        public Vector3 UnitPosition { get; set; }

        [DataProperty(NotNull = true)]
        public string Icon { get; set; }

        public GroundHexTerrainTileSettings()
        {
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class CastleTileSettings : GroundHexTerrainTileSettings
    {
        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new CastleTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class ManaMineTileSettings : GroundHexTerrainTileSettings
    {
        [DataProperty(NotNull = true)]
        public int ManaIncrement { get; set; }

        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new ManaMineTileDesc(this, content);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class LandTileSettings : GroundHexTerrainTileSettings
    {
        [DataProperty]
        public string HpIncNegatingTemple { get; set; }

        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new LandTileDesc(this, content);
        }
    }
}
