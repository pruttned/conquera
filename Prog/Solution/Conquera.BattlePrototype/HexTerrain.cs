﻿//////////////////////////////////////////////////////////////////////
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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace Conquera.BattlePrototype
{
    public class HexTerrain
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

        public HexTerrain(string file, IList<BattlePlayer> players)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException("file");
            if (null == players) throw new ArgumentNullException("players");

            var terrainElm = XElement.Load(file);
            Width = (int)terrainElm.Attribute("width");
            Height = (int)terrainElm.Attribute("height");

            mTiles = new HexTerrainTile[Width, Height];

            int i = 0;
            foreach (var columnElement in terrainElm.Elements("column"))
            {
                int j = 0;
                foreach (var tileElm in columnElement.Elements("tile"))
                {
                    HexTerrainTile tile = HexTerrainTileFactory.CreateTile(tileElm.Attribute("template").Value, new Point(i, j));
                    if (tile is CapturableHexTerrainTile)
                    {
                        var owningPlayerAtt = tileElm.Attribute("owningPlayer");
                        if (null != owningPlayerAtt)
                        {
                            ((CapturableHexTerrainTile)tile).OwningPlayer = players[(int)owningPlayerAtt];
                        }
                    }
                    mTiles[i, j] = tile;
                    j++;
                }
                i++;
            }
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

        public void SetTile(Point index, string template)
        {
            if (string.IsNullOrEmpty(template)) throw new ArgumentNullException("template");

            var oldTile = mTiles[index.X, index.Y];
            if(!string.Equals(template, HexTerrainTileFactory.GetTemplateName(oldTile.GetType()), StringComparison.OrdinalIgnoreCase))
            {
                var newTile = HexTerrainTileFactory.CreateTile(template, index);
                mTiles[index.X, index.Y] = newTile;
                BattlePlayer owningPlayer = null;

                CapturableHexTerrainTile oldTileAsCapturable = oldTile as CapturableHexTerrainTile;
                CapturableHexTerrainTile newTileAsCapturable = newTile as CapturableHexTerrainTile;
                if (null != oldTileAsCapturable)
                {
                    owningPlayer = oldTileAsCapturable.OwningPlayer;
                }
                if (null != owningPlayer && null != newTileAsCapturable)
                {
                    newTileAsCapturable.OwningPlayer = owningPlayer;
                }
            }
        }

        public void Save(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException("file");

            XElement terrainElm = new XElement("terrain",
                new XAttribute("width", Width),
                new XAttribute("height", Height));

            for (int i = 0; i < Width; ++i)
            {
                var columnElement = new XElement("column");
                terrainElm.Add(columnElement);
                for (int j = 0; j < Height; ++j)
                {
                    var tile = mTiles[i, j];
                    var tileElm = new XElement("tile",
                        new XAttribute("template", HexTerrainTileFactory.GetTemplateName(tile.GetType())));

                    if (tile is CapturableHexTerrainTile)
                    {
                        var owningPlayer = ((CapturableHexTerrainTile)tile).OwningPlayer;
                        if (null != owningPlayer)
                        {
                            tileElm.Add(new XAttribute("owningPlayer", owningPlayer.Index));
                        }
                    }

                    columnElement.Add(tileElm);
                }
            }

            terrainElm.Save(file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsInTerrain(Point index)
        {
            return (index.X >= 0 && index.X < Width && index.Y >= 0 & index.Y < Height);
        }

        /// <summary>
        /// Gets a tile sibling in a given direction (or null in case of terrain end)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="direction"></param>
        /// <param name="sibling"></param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public HexTerrainTile GetSibling(Point index, HexDirection direction)
        {
            Point siblingIndex = HexHelper.GetSibling(index, direction);
            if (IsInTerrain(siblingIndex))
            {
                return mTiles[siblingIndex.X, siblingIndex.Y];
            }
            return null;
        }

        /// <summary>
        /// Gets all siblings of a given tile
        /// </summary>
        /// <param name="index"></param>
        /// <param name="siblings"></param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public void GetSiblings(Point index, IList<HexTerrainTile> siblings)
        {
            int i = index.X;
            int j = index.Y;

            TryAddSibling(i - 1, j, siblings);
            TryAddSibling(i + 1, j, siblings);
            if (0 != (j & 1))
            {
                TryAddSibling(i, j - 1, siblings);
                TryAddSibling(i + 1, j - 1, siblings);
                TryAddSibling(i, j + 1, siblings);
                TryAddSibling(i + 1, j + 1, siblings);
            }
            else
            {
                TryAddSibling(i - 1, j - 1, siblings);
                TryAddSibling(i, j - 1, siblings);
                TryAddSibling(i - 1, j + 1, siblings);
                TryAddSibling(i, j + 1, siblings);
            }
        }

        /// <summary>
        /// Gets all siblings of a given tile
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public IList<HexTerrainTile> GetSiblings(Point index)
        {
            List<HexTerrainTile> siblings = new List<HexTerrainTile>();
            GetSiblings(index, siblings);
            return siblings;
        }

        private void TryAddSibling(int i, int j, IList<HexTerrainTile> siblings)
        {
            if (i >= 0 && i < Width && j >= 0 && j < Height)
            {
                siblings.Add(this[i, j]);
            }
        }
    }
}
