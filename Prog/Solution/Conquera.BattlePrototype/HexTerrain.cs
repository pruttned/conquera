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
        public event EventHandler<ValueChangeEventArgs<HexTerrainTile>> TileSet;

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

        public HexTerrain(XElement parentElm, IList<BattlePlayer> players)
        {
            if (null == parentElm) throw new ArgumentNullException("parentElm");
            if (null == players) throw new ArgumentNullException("players");

            var terrainElm = parentElm.Element("terrain");
            Width = (int)terrainElm.Attribute("width");
            Height = (int)terrainElm.Attribute("height");

            mTiles = new HexTerrainTile[Width, Height];

            int i = 0;
            foreach (var columnElement in terrainElm.Elements("column"))
            {
                int j = 0;
                foreach (var tileElm in columnElement.Elements("tile"))
                {
                    HexTerrainTile tile = HexTerrainTileFactory.CreateTile(tileElm.Attribute("template").Value, new Point(i, j), Height);
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
                    mTiles[i, j] = new LandHexTerrainTile(new Point(i, j), height);
                }
            }
        }

        public void SetTile(Point index, string template)
        {
            if (string.IsNullOrEmpty(template)) throw new ArgumentNullException("template");

            var oldTile = mTiles[index.X, index.Y];
            if(!string.Equals(template, HexTerrainTileFactory.GetTemplateName(oldTile.GetType()), StringComparison.OrdinalIgnoreCase))
            {
                var newTile = HexTerrainTileFactory.CreateTile(template, index, Height);
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

                newTile.Unit = oldTile.Unit;

                EventHelper.RaiseValueChange<HexTerrainTile>(TileSet, this, oldTile, newTile);
            }
        }

        public void Save(XElement parentElm)
        {
            XElement terrainElm = new XElement("terrain",
                new XAttribute("width", Width),
                new XAttribute("height", Height));
            parentElm.Add(terrainElm);

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsInTerrain(Point index)
        {
            return IsInTerrain(index.X, index.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsInTerrain(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 & y < Height);
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

        public void ForEachSibling(Point index, Action<HexTerrainTile, HexDirection> fun)
        {
            int i = index.X;
            int j = index.Y;

            ForValidSibling(i - 1, j, HexDirection.Left, fun);
            ForValidSibling(i + 1, j, HexDirection.Right, fun);
            if (0 != (j & 1))
            {
                ForValidSibling(i, j - 1, HexDirection.LowerLeft, fun);
                ForValidSibling(i + 1, j - 1, HexDirection.LowerRight, fun);
                ForValidSibling(i, j + 1, HexDirection.UperLeft, fun);
                ForValidSibling(i + 1, j + 1, HexDirection.UperRight, fun);
            }
            else
            {
                ForValidSibling(i - 1, j - 1, HexDirection.LowerLeft, fun);
                ForValidSibling(i, j - 1, HexDirection.LowerRight, fun);
                ForValidSibling(i - 1, j + 1, HexDirection.UperLeft, fun);
                ForValidSibling(i, j + 1, HexDirection.UperRight, fun);
            }
        }

        public void ShowOverlay()
        {
            foreach (HexTerrainTile tile in mTiles)
            {
                tile.IsOverlayVisible = true;
            }
        }

        public void HideOverlay()
        {
            foreach (HexTerrainTile tile in mTiles)
            {
                tile.IsOverlayVisible = false;
            }
        }

        public void SetOverlayVisibility(bool value)
        {
            if (value)
            {
                ShowOverlay();
            }
            else
            {
                HideOverlay();
            }
        }

        private void ForValidSibling(int i, int j, HexDirection direction, Action<HexTerrainTile, HexDirection> fun)
        {
            if (IsInTerrain(i, j))
            {
                fun(mTiles[i, j], direction);
            }
        }
    }
}
