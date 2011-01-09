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
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    public class HexCell
    {
        private bool mIsPassable;
        private HexRegion mRegion = null;
        private GameScene mScene;
        private HexTerrainTile mHexTerrainTile;
        private GameUnit mGameUnit;

        public bool IsPassable
        {
            get { return mIsPassable; }
            private set 
            { 
                mIsPassable = value;
            }
        }

        public bool IsGap
        {
            get {return null == mHexTerrainTile; }
        }

        public HexTerrainTileDesc HexTerrainTile
        {
            get { return null != mHexTerrainTile ? mHexTerrainTile.Desc : null; }
        }

        public GameUnit GameUnit 
        {
            get { return mGameUnit; }
            set 
            {
                if (null != value && null != mGameUnit)
                {
                    throw new ArgumentException(string.Format("Cell {0} already contains a game unit", Index));
                }
                mGameUnit = value;
                UpdatePassableness();
            }
        }

        internal HexRegion NewRegion { get; set; }

        public HexRegion Region
        {
            get { return mRegion; }
        }

        public GamePlayer OwningPlayer
        {
            get 
            {
                return null != mRegion ? mRegion.OwningPlayer : null;
            }
        }

        public Point Index { get; private set; }

        public Vector3 CenterPos
        {
            get 
            {
                if (null != mHexTerrainTile)
                {
                    return mHexTerrainTile.CenterPos;
                }
                else
                {
                    return HexTerrain.GetPosFromIndex(Index);
                }
            }
        }

        public bool IsActive
        {
            get { return (null != Region && Region.IsActive); }
        }

        public bool BelongsToCurrentPlayer
        {
            get { return (null != OwningPlayer && OwningPlayer == Scene.CurrentPlayer); }
        }

        public GameScene Scene
        {
            get { return mScene; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="hexTerrainTile">Nullable</param>
        internal HexCell(GameScene scene, Point index)
        {
            mScene = scene;
            mHexTerrainTile = scene.Terrain.GetTile(index);
            UpdatePassableness();
            Index = index;
        }

        internal void UpdateRegion()
        {
            GamePlayer oldOwner = OwningPlayer;
            GamePlayer newOwner = null != NewRegion ? NewRegion.OwningPlayer : null;
            bool oldIsActive = (null != mRegion && mRegion.IsActive);
            bool newIsActive = (null != NewRegion && (NewRegion.IsActive));

            if (oldOwner != newOwner)
            {
                if (null != oldOwner)
                {
                    oldOwner.RemoveCell(this);
                }
                if (null != newOwner)
                {
                    newOwner.AddCell(this);
                }

                if (null != mHexTerrainTile)
                {
                    if (oldIsActive)
                    {
                        mHexTerrainTile.OnDeactivating(this);
                    }
                    mRegion = NewRegion;

                    if (newIsActive)
                    {
                        mHexTerrainTile.OnActivated(this);
                    }
                }
                else
                {
                    mRegion = NewRegion;
                }
            }
            else
            {
                if (null != mHexTerrainTile && oldIsActive != newIsActive)
                {
                    if (newIsActive)
                    {
                        mHexTerrainTile.OnActivated(this);
                    }
                    else
                    {
                        mHexTerrainTile.OnDeactivating(this);
                    }
                }
                mRegion = NewRegion;
            }
        }

        public void GetCornerPos(HexTileCorner corner, out Vector3 pos)
        {
            HexTerrainTileDesc.GetCornerPos(corner, out pos);
            Vector3 centerPos = CenterPos;
            Vector3.Add(ref centerPos, ref pos, out pos);
        }

        public Vector3 GetCornerPos(HexTileCorner corner)
        {
            Vector3 pos;
            GetCornerPos(corner, out pos);
            return pos;
        }

        public void GetCorner2DPos(HexTileCorner corner, out Vector2 pos)
        {
            Vector3 pos3D;
            GetCornerPos(corner, out pos3D);
            pos = new Vector2(pos3D.X, pos3D.Y);
        }

        public Vector2 GetCorner2DPos(HexTileCorner corner)
        {
            Vector2 pos;
            GetCorner2DPos(corner, out pos);
            return pos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tile">Null = gap</param>
        public void SetTile(string tile)
        {
            var newTile = mScene.Terrain.SetTile(Index, tile);
            if(newTile != mHexTerrainTile)
            {
                HexTerrainTileDesc oldDesc = HexTerrainTile;
                mHexTerrainTile = newTile;
                UpdatePassableness();
                mScene.HexCellTileChanged(this, oldDesc);
            }
        }

        public void GetCornerIndex(HexTileCorner corner, out Point index)
        {
            int i = Index.X;
            int j = Index.Y;
            bool isEven = (0 == (j & 1));

            switch (corner)
            {
                case HexTileCorner.Top:
                    index = isEven ? new Point(i, j * 2 + 3) : new Point(i + 1, (j - 1) * 2 + 5);
                    break;
                case HexTileCorner.UperRight:
                    index = new Point(i + 1, isEven ? j * 2 + 2 : (j - 1) * 2 + 4);
                    break;
                case HexTileCorner.LowerRight:
                    index = new Point(i + 1, isEven ? j * 2 + 1 : (j - 1) * 2 + 3);
                    break;
                case HexTileCorner.Down:
                    index = isEven ? new Point(i, j * 2) : new Point(i + 1, (j - 1) * 2 + 2);
                    break;
                case HexTileCorner.LowerLeft:
                    index = new Point(i, isEven ? j * 2 + 1 : (j - 1) * 2 + 3);
                    break;
                case HexTileCorner.UperLeft:
                    index = new Point(i, isEven ? j * 2 + 2 : (j - 1) * 2 + 4);
                    break;
                default:
                    throw new ArgumentException("Invalid corner");
            }
        }

        /// <summary>
        /// Sibling or null for a cell otside of map
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="includeGaps"></param>
        /// <returns></returns>
        public HexCell GetSibling(HexDirection direction)
        {
            Point siblingPos;
            if (mScene.Terrain.GetSiblingPos(Index, true, direction, out siblingPos))
            {
                return mScene.GetCell(siblingPos);
            }
            return null;
        }

        public List<HexCell> GetSiblings()
        {
            List<HexCell> siblings = new List<HexCell>();
            GetSiblings(siblings);
            return siblings;
        }

        public void GetSiblings(List<HexCell> siblings)
        {
            int i = Index.X;
            int j = Index.Y;

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

        public bool IsSiblingTo(HexCell cell)
        {
            int i = Index.X;
            int j = Index.Y;

            int i2 = cell.Index.X;
            int j2 = cell.Index.Y;

            return (i2==i - 1 && j2==j) ||
                    (i2==i + 1 && j2 == j) ||
                    ((0 != (j & 1)) && ((i2 == i && j2 == j - 1 )|| (i2 == i + 1 && j2 == j - 1) || (i2 == i && j2 == j + 1) || (i2 == i + 1 && j2 == j + 1))) ||
                    ((0 == (j & 1)) && ((i2 == i - 1 && j2 == j - 1 )|| (i2 == i && j2 == j - 1) || (i2 == i - 1 && j2 == j + 1) || (i2 == i && j2 == j + 1)));
        }

        internal void OnBeginTurn()
        {
            mHexTerrainTile.OnBeginTurn(this);
        }

        private void UpdatePassableness()
        {
            IsPassable = (null != mHexTerrainTile) && mHexTerrainTile.Desc.IsPassable && null == mGameUnit;
        }

        private void TryAddSibling(int i, int j, IList<HexCell> siblings)
        {
            HexTerrain terrain = mScene.Terrain;
            if (i >= 0 && i < terrain.Width && j >= 0 && j < terrain.Height)
            {
                siblings.Add(mScene.GetCell(new Point(i, j)));
            }
        }
    }

}
