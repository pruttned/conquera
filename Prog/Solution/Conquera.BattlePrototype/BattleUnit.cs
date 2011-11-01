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

namespace Conquera.BattlePrototype
{
    public abstract class BattleUnit
    {


        #region Types
        struct HexTileSeed
        {
            public HexTerrainTile Tile;
            public int Live;

            public HexTileSeed(HexTerrainTile tile, int live)
            {
                Tile = tile;
                Live = live;
            }
        }
        #endregion Types

        public delegate void CellIndexChangedHandler(BattleUnit obj, Point oldValue);
        public event CellIndexChangedHandler TileIndexChanged;


        //promoted collections
        private static List<HexTerrainTile> Siblings = new List<HexTerrainTile>(6);
        private static HashSet<Point> CheckedPoints = new HashSet<Point>();
        private static Queue<HexTileSeed> Seeds = new Queue<HexTileSeed>();

        private HexTerrain mTerrain;


        private Point mTileIndex;

        public BattlePlayer Player { get; private set; }

        public abstract int BaseAttack { get; }
        public abstract int BaseDefense { get; }
        public abstract int BaseMovementDistance { get; }

        //public int Attack { get; private set; }
        //public int Defense { get; private set; }
        //public int MovementDistance { get; private set; }

        //temp
        public int Attack { get { return BaseAttack; } }
        public int Defense { get { return BaseDefense; } }
        public int MovementDistance { get { return BaseMovementDistance; } }

        public bool HasMovedThisTurn { get; set; }

        public Point TileIndex
        {
            get { return mTileIndex; }
            set
            {
                if (mTileIndex != value)
                {
                    Point oldValue = mTileIndex;
                    mTileIndex = value;

                    //todo
                    //UpdatePositionFromIndex();

                    if (null != TileIndexChanged)
                    {
                        TileIndexChanged.Invoke(this, oldValue);
                    }
                }
            }
        }

        public BattleUnit(BattlePlayer player, HexTerrain terrain)
        {
            if (null == player) throw new ArgumentNullException("player");
            if (null == terrain) throw new ArgumentNullException("terrain");

            Player = player;
            mTerrain = terrain;
        }

        /// <summary>
        /// Gets all poitions where is possible for unit to move
        /// </summary>
        /// <param name="points"></param>
        public void GetPossibleMoves(List<Point> points)
        {
            Seeds.Clear();
            CheckedPoints.Clear();

            Seeds.Enqueue(new HexTileSeed(mTerrain[TileIndex], MovementDistance));
            while (0 < Seeds.Count)
            {
                var seed = Seeds.Dequeue();
                Siblings.Clear();

                mTerrain.GetSiblings(seed.Tile.Index, Siblings);
                foreach (var sibling in Siblings)
                {
                    Point index = sibling.Index;
                    if (sibling.IsPassableAndEmpty && !CheckedPoints.Contains(index))
                    {
                        points.Add(index);
                        CheckedPoints.Add(index);
                        if (0 < seed.Live - 1)
                        {
                            Seeds.Enqueue(new HexTileSeed(sibling, seed.Live - 1));
                        }
                    }
                }
            }
        }

        public bool CanMoveTo(Point index)
        {
            if (index == TileIndex)
            {
                return false;
            }
            HexTerrainTile srcCell = mTerrain[TileIndex];
            HexTerrainTile targetCell = mTerrain[index];

            if (srcCell == targetCell)
            {
                return false;
            }

            if (targetCell.IsPassable && MovementDistance >= HexHelper.GetDistance(srcCell.Index, targetCell.Index))
            {
                Point targetIndex = targetCell.Index;

                Seeds.Clear();
                CheckedPoints.Clear();

                Seeds.Enqueue(new HexTileSeed(srcCell, MovementDistance));
                while (0 < Seeds.Count)
                {
                    var seed = Seeds.Dequeue();
                    Siblings.Clear();
                    mTerrain.GetSiblings(seed.Tile.Index, Siblings);
                    foreach (var sibling in Siblings)
                    {
                        if (sibling.Index == targetIndex)
                        {
                            return true;
                        }

                        Point siblingIndex = sibling.Index;
                        if (sibling.IsPassable && !CheckedPoints.Contains(siblingIndex))
                        {
                            CheckedPoints.Add(siblingIndex);
                            if (0 < seed.Live - 1)
                            {
                                Seeds.Enqueue(new HexTileSeed(sibling, seed.Live - 1));
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// No defense is considered here
        /// </summary>
        /// <returns></returns>
        public int ComputeDamageFromEnemies()
        {
            int damage = 0;
            Siblings.Clear();
            mTerrain.GetSiblings(TileIndex, Siblings);
            foreach (var tile in Siblings)
            {
                if (null != tile.Unit && tile.Unit.Player != Player)
                {
                    damage += tile.Unit.Attack;
                }
            }

            return damage;
        }
    }

    public class SkeletonLv1BattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 1; }
        }

        public override int BaseDefense
        {
            get { return 1; }
        }

        public override int BaseMovementDistance
        {
            get { return 3; }
        }

        public SkeletonLv1BattleUnit(BattlePlayer player, HexTerrain terrain)
            :base(player, terrain)
        {
        }
    }
}
