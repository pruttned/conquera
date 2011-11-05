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
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Conquera.BattlePrototype
{
    public abstract class BattleUnit : Grid
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

        protected static readonly string mBattleUnitImagesDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BattleUnitImages");

        //promoted collections
        private static HashSet<Point> CheckedPoints = new HashSet<Point>();
        private static Queue<HexTileSeed> Seeds = new Queue<HexTileSeed>();

        private HexTerrain mTerrain;

        private Point mTileIndex;

        private TextBlock mPropertiesTextBlock;
        private Border mBorder;
        private bool mHasMovedThisTurn;
        private bool mIsSelected = false;

        public BattlePlayer Player { get; private set; }

        public abstract int BaseAttack { get; }
        public abstract int BaseDefense { get; }
        public abstract int BaseMovementDistance { get; }

        //public int Attack { get; private set; }
        //public int Defense { get; private set; }
        //public int MovementDistance { get; private set; }

        //temp
        public int Attack { get { return BaseAttack; } }
        public int Defense { get; private set; }
        public int MovementDistance { get { return BaseMovementDistance; } }

        public bool HasMovedThisTurn
        {
            get { return mHasMovedThisTurn; }
            set
            {
                if (value != mHasMovedThisTurn)
                {
                    mHasMovedThisTurn = value;
                    UpdateGraphics();
                }
            }
        }

        public Point TileIndex
        {
            get { return mTileIndex; }
            set
            {

                if (value != mTileIndex)
                {
                    if (null != mTerrain[value.X, value.Y].Unit)
                    {
                        throw new ArgumentException("Destination tile already contains a unit");
                    }

                    Point oldValue = mTileIndex;
                    mTerrain[oldValue.X, oldValue.Y].Unit = null;

                    mTileIndex = value;

                    //todo
                    //UpdatePositionFromIndex();
                    mTerrain[mTileIndex.X, mTileIndex.Y].Unit = this;

                    UpdateSiblingsDefenses(oldValue);
                    UpdateSiblingsDefenses(mTileIndex);
                    UpdateDefenseFromAlies();

                    if (null != TileIndexChanged)
                    {
                        TileIndexChanged.Invoke(this, oldValue);
                    }
                }
            }
        }

        public bool IsSelected
        {
            get { return mIsSelected; }
            internal set
            {
                if (value != mIsSelected)
                {
                    mIsSelected = value;
                    UpdateGraphics();
                }
            }
        }

        public BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
        {
            if (null == player) throw new ArgumentNullException("player");
            if (null == terrain) throw new ArgumentNullException("terrain");

            Player = player;
            mTerrain = terrain;
            mTileIndex = tileIndex;

            if(null != terrain[tileIndex.X, tileIndex.Y].Unit)
            {
                throw new ArgumentException("Destination tile already contains a unit");
            }

            terrain[tileIndex.X, tileIndex.Y].Unit = this;

            player.Units.Add(this);

            //Graphics
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            mBorder = new Border();
            mBorder.BorderBrush = Brushes.Yellow;
            mPropertiesTextBlock = new TextBlock();
            mPropertiesTextBlock.Background = new SolidColorBrush(Color.FromArgb(255, player.Color.R, player.Color.G, player.Color.B));
            mPropertiesTextBlock.Foreground = Brushes.White;
            mPropertiesTextBlock.FontSize = 10;
            mBorder.Child = mPropertiesTextBlock;
            Children.Add(mBorder);
            UpdateGraphics();

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(System.IO.Path.Combine(mBattleUnitImagesDirectory, GetImageFileName())));
            image.Width = 22;
            image.Height = 22;
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            image.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Children.Add(image);

            UpdateDefenseFromAlies();
            UpdateSiblingsDefenses(mTileIndex);
        }

        public void Kill()
        {
            if (Player.Units.Remove(this))
            {
                mTerrain[mTileIndex.X, mTileIndex.Y].Unit = null;
            }
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

                mTerrain.ForEachSibling(seed.Tile.Index,
                    sibling =>
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
                    });
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
                    bool found = false;
                    var seed = Seeds.Dequeue();
                    mTerrain.ForEachSibling(seed.Tile.Index,
                        sibling =>
                        {
                            if (sibling.Index == targetIndex)
                            {
                                found = true;
                            }
                            else
                            {
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
                        });
                    if (found)
                    {
                        return true;
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
            mTerrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player != Player)
                    {
                        damage += sibling.Unit.Attack;
                    }
                });

            return damage;
        }

        public void UpdateDefenseFromAlies()
        {
            int defense = 0;
            mTerrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        defense++;
                    }
                });

            //todo: spell effectss
            Defense = BaseDefense + defense;

            UpdateGraphics();
        }

        public void Move(Point tileIndex)
        {
            HasMovedThisTurn = true;
            TileIndex = tileIndex;
        }

        protected abstract string GetImageFileName();

        private void UpdateGraphics()
        {
            mPropertiesTextBlock.Text = IsSelected ? "[SELECTED]\n" : "\n";
            mPropertiesTextBlock.Text += string.Format("A = {0}\nD = {1}\nM = {2}", Attack, Defense, MovementDistance);
            mBorder.BorderThickness = new System.Windows.Thickness(HasMovedThisTurn ? 0.0 : 5.0);            
        }

        private void UpdateSiblingsDefenses(Point pos)
        {
            mTerrain.ForEachSibling(pos,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        sibling.Unit.UpdateDefenseFromAlies();
                    }
                });
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

        public SkeletonLv1BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            :base(player, terrain, tileIndex)
        {
        }

        protected override string GetImageFileName()
        {
            return "SlayerIcon.png";
        }
    }

    public class ZombieLv1BattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 2; }
        }

        public override int BaseDefense
        {
            get { return 2; }
        }

        public override int BaseMovementDistance
        {
            get { return 2; }
        }

        public ZombieLv1BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }

        protected override string GetImageFileName()
        {
            return "PlagueIcon.png";
        }
    }
}
