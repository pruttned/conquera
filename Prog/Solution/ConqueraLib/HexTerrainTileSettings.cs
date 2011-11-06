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

    public struct GapWallSampleSettings
    {
        private long mGraphicModel;
        private int mPriority;

        public long GraphicModel
        {
            get
            {
                return mGraphicModel;
            }
            set
            {
                mGraphicModel = value;
            }
        }
        public int Priority
        {
            get
            {
                return mPriority;
            }
            set
            {
                mPriority = value;
            }
        }

        public GapWallSampleSettings(int priority, long graphicModel)
        {
            mGraphicModel = graphicModel;
            mPriority = priority;
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(GapWallSampleSettings), typeof(PropertyCustomBasicTypeProvider<GapWallSampleSettings>))]
    public class GapHexTerrainTileSettings : HexTerrainTileSettings
    {
        [DataListProperty(NotNull = true)]
        public List<GapWallSampleSettings> GapWallGraphicModelSamples { get; set; }

        public GapHexTerrainTileSettings()
        {
            GapWallGraphicModelSamples = new List<GapWallSampleSettings>();
        }

        public override HexTerrainTileDesc CreateDesc(ContentGroup content)
        {
            return new GapHexTerrainTileDesc(this, content);
        }


        protected override void AfterLoadImpl(OrmManager ormManager)
        {
            GapWallGraphicModelSamples.Sort((x, y) => -Comparer<int>.Default.Compare(x.Priority, y.Priority)); //sort from greatest priority to lowest
            base.AfterLoadImpl(ormManager);
        }
        protected override void BeforeSaveImpl(OrmManager ormManager)
        {
            if (0 == GapWallGraphicModelSamples.Count)
            {
                throw new ArgumentException("At least one GapWallGraphicModelSample must be specified");
            }
            base.BeforeSaveImpl(ormManager);
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
