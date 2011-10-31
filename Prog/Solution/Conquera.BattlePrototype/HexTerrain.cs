using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Conquera.BattlePrototype
{
    class HexTerrain
    {
        private HexTerrainTile[,] mTiles;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public HexTerrainTile this[int i, int j]
        {
            get
            {
                return mTiles[i, j];
            }
        }

        public HexTerrainTile this[Point index]
        {
            get
            {
                return mTiles[index.X, index.Y];
            }
        }

        public static HexTerrain Load(string file)
        {
            throw new NotImplementedException();
        }

        public HexTerrain(int width, int height)
        {
            Width = width;
            Height = height;

            mTiles = new HexTerrainTile[width, height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    mTiles[i, j] = new GapHexTerrainTile(new Point(i, j));
                }
            }
        }

        public void Save(string file)
        {
            throw new NotImplementedException();
        }
    }
}
