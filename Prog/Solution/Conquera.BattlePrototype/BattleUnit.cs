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

        public int mHp;

        private List<IBattleUnitDefenseModifier> mDefenseModifiers = new List<IBattleUnitDefenseModifier>();
        private List<IBattleUnitAttackModifier> mAttackModifiers = new List<IBattleUnitAttackModifier>();
        private List<IBattleUnitMovementDistanceModifier> mMovementDistanceModifiers = new List<IBattleUnitMovementDistanceModifier>();


        private TextBlock mPropertiesTextBlock;
        private Border mBorder;
        private bool mHasMovedThisTurn;
        private bool mIsSelected = false;

        private int mDefense;
        private int mAttack;
        private int mMovementDistance;

        private List<IBattleUnitSpellEffect> mSpellEffects = new List<IBattleUnitSpellEffect>();



        public BattlePlayer Player { get; private set; }

        public abstract int BaseAttack { get; }
        public abstract int BaseDefense { get; }
        public abstract int BaseMovementDistance { get; }
        public abstract int MaxHp { get; }

        public bool IsKilled { get; private set; }

        public int Hp
        {
            get { return mHp; }
            set 
            { 
                mHp = MathExt.Clamp(value, 0, MaxHp);
                UpdateGraphics();
            }
        }

        //public int Attack { get; private set; }
        //public int Defense { get; private set; }
        //public int MovementDistance { get; private set; }

        //temp
        public int Attack
        {
            get { return mAttack; }
            private set { mAttack = Math.Max(0, value); }
        }
        public int Defense
        {
            get { return mDefense; }
            private set { mDefense = Math.Max(0, value); }
        }
        public int MovementDistance
        {
            get { return mMovementDistance; }
            private set { mMovementDistance = Math.Max(0, value); }
        }

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

                    NotifySiblingsAfterDeparture(oldValue);
                    NotifySiblingsAfterArrival();
                    UpdateDefense();

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

        public virtual int Level
        {
            get { return 1; }
        }

        public BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
        {
            if (null == player) throw new ArgumentNullException("player");
            if (null == terrain) throw new ArgumentNullException("terrain");

            IsKilled = false;
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
            mPropertiesTextBlock = new TextBlock();
            mPropertiesTextBlock.Background = new SolidColorBrush(Color.FromArgb(255, player.Color.R, player.Color.G, player.Color.B));
            mPropertiesTextBlock.Foreground = Brushes.White;
            mPropertiesTextBlock.FontSize = 10;
            mBorder.BorderThickness = new System.Windows.Thickness(5);
            mBorder.Child = mPropertiesTextBlock;
            Children.Add(mBorder);
            UpdateGraphics();

            Image image = CreateImage();            
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            image.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Children.Add(image);

            UpdateDefense();
            UpdateAttack();
            UpdateMovementDistance();
            NotifySiblingsAfterArrival();

            Hp = MaxHp;

            TextBlock levelTextBlock = new TextBlock();
            levelTextBlock.Text = Level.ToString();
            levelTextBlock.Foreground = Brushes.Yellow;
            levelTextBlock.Background = Brushes.Black;
            levelTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            levelTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Children.Add(levelTextBlock);
        }

        //Only for temporary unit (to read base attributes while constructing description)
        public BattleUnit()
        {
        }

        public void AddDefenseModifier(IBattleUnitDefenseModifier modifier)
        {
            if (null == modifier) throw new ArgumentNullException("modifier");
            mDefenseModifiers.Add(modifier);
            UpdateDefense();
        }
        public bool RemoveDefenseModifier(IBattleUnitDefenseModifier modifier)
        {
            if (null == modifier) throw new ArgumentNullException("modifier");
            bool removed = mDefenseModifiers.Remove(modifier);
            UpdateDefense();
            return removed;
        }
        public void AddAttackModifier(IBattleUnitAttackModifier modifier)
        {
            if (null == modifier) throw new ArgumentNullException("modifier");
            mAttackModifiers.Add(modifier);
            UpdateAttack();
        }
        public bool RemoveAttackModifier(IBattleUnitAttackModifier modifier)
        {
            if (null == modifier) throw new ArgumentNullException("modifier");
            bool removed = mAttackModifiers.Remove(modifier);
            UpdateAttack();
            return removed;
        }
        public void AddMovementDistanceModifier(IBattleUnitMovementDistanceModifier modifier)
        {
            if (null == modifier) throw new ArgumentNullException("modifier");
            mMovementDistanceModifiers.Add(modifier);
            UpdateMovementDistance();
        }
        public bool RemoveMovementDistanceModifier(IBattleUnitMovementDistanceModifier modifier)
        {
            if (null == modifier) throw new ArgumentNullException("modifier");
            bool removed = mMovementDistanceModifiers.Remove(modifier);
            UpdateMovementDistance();
            return removed;
        }




        public void OnTurnStart(int turnNum)
        {
            //update spell effects
            for (int i = mSpellEffects.Count - 1; i >= 0 && !IsKilled; --i)
            {
                if (!mSpellEffects[i].OnStartTurn(turnNum))
                {
                    mSpellEffects[i].OnEnd();
                    mSpellEffects.RemoveAt(i);
                }
            }

            if (!IsKilled)
            {
                UpdateGraphics();
            }
        }
        
        public void Kill()
        {
            IsKilled = true;
            if (Player.Units.Remove(this))
            {
                mTerrain[mTileIndex.X, mTileIndex.Y].Unit = null;
            }
            NotifySiblingsAfterDeparture(mTileIndex);
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

        public void Move(Point tileIndex)
        {
            HasMovedThisTurn = true;
            TileIndex = tileIndex;
        }

        public void UpdateGraphics()
        {
            mPropertiesTextBlock.Text = IsSelected ? "[SELECTED]\n" : "[]\n";
            mPropertiesTextBlock.Text += string.Format("A = {0}+{5}\nD = {1}+{6}\nM = {2}+{7}\nHp = {3}/{4}", BaseAttack, BaseDefense, BaseMovementDistance, Hp, MaxHp,
                Attack - BaseAttack, Defense - BaseDefense, MovementDistance-BaseMovementDistance);
            mBorder.BorderBrush = (!HasMovedThisTurn && Player.IsActive ? Brushes.Yellow : Brushes.Black);
        }

        public void AddSpellEffect(int turnNum, IBattleUnitSpellEffect spellEffect)
        {
            mSpellEffects.Add(spellEffect);
            spellEffect.OnCast(turnNum, this);
        }

        public Image CreateImage()
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(System.IO.Path.Combine(mBattleUnitImagesDirectory, GetImageFileName())));
            image.Width = 22;
            image.Height = 22;
            return image;
        }

        protected abstract string GetImageFileName();

        private void AfterSiblingArrival(BattleUnit battleUnit)
        {
            UpdateDefense();
        }
        private void AfterSiblingDeparture(BattleUnit battleUnit)
        {
            UpdateDefense();
        }

        private void UpdateDefense()
        {
            int defenseFromAllies = 0;
            mTerrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        defenseFromAllies++;
                    }
                });

            Defense = BaseDefense + defenseFromAllies + GetDefenseModifier();

            UpdateGraphics();
        }
        private void UpdateAttack()
        {
            Attack = BaseAttack + GetAttackModifier();

            UpdateGraphics();
        }
        private void UpdateMovementDistance()
        {
            MovementDistance = BaseMovementDistance + GetMovementDistanceModifier();

            UpdateGraphics();
        }

        private void NotifySiblingsAfterArrival()
        {
            mTerrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        sibling.Unit.AfterSiblingArrival(this);
                    }
                });
        }

        private void NotifySiblingsAfterDeparture(Point oldPos)
        {
            mTerrain.ForEachSibling(oldPos,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        sibling.Unit.AfterSiblingDeparture(this);
                    }
                });
        }

        private int GetDefenseModifier()
        {
            int sum = 0;
            foreach (var modif in mDefenseModifiers)
            {
                sum += modif.GetModifier(this);
            }
            return sum;
        }
        private int GetAttackModifier()
        {
            int sum = 0;
            foreach (var modif in mAttackModifiers)
            {
                sum += modif.GetModifier(this);
            }
            return sum;
        }
        private int GetMovementDistanceModifier()
        {
            int sum = 0;
            foreach (var modif in mMovementDistanceModifiers)
            {
                sum += modif.GetModifier(this);
            }
            return sum;
        }
    }

    public class GeneralBattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 3; }
        }

        public override int BaseDefense
        {
            get { return 10; }
        }

        public override int BaseMovementDistance
        {
            get { return 1; }
        }

        public override int MaxHp
        {
            get { return 10; }
        }
        public GeneralBattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
        
        //Only for temporary unit (to read base attributes while constructing description)
        public GeneralBattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "SpikesIcon.png";
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

        public override int MaxHp
        {
            get { return 3; }
        }

        public SkeletonLv1BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            :base(player, terrain, tileIndex)
        {
        }

        //Only for temporary unit (to read base attributes while constructing description)
        public SkeletonLv1BattleUnit()
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

        public override int MaxHp
        {
            get { return 3; }
        }

        public ZombieLv1BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
        
        //Only for temporary unit (to read base attributes while constructing description)
        public ZombieLv1BattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "PlagueIcon.png";
        }
    }

    public class BansheeLv1BattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 3; }
        }

        public override int BaseDefense
        {
            get { return 1; }
        }

        public override int BaseMovementDistance
        {
            get { return 2; }
        }

        public override int MaxHp
        {
            get { return 3; }
        }

        public BansheeLv1BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
                
        //Only for temporary unit (to read base attributes while constructing description)
        public BansheeLv1BattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "BloodMadnessIcon.png";
        }
    }

    public class SpectreLv1BattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 1; }
        }

        public override int BaseDefense
        {
            get { return 3; }
        }

        public override int BaseMovementDistance
        {
            get { return 3; }
        }

        public override int MaxHp
        {
            get { return 3; }
        }

        public SpectreLv1BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
                
        //Only for temporary unit (to read base attributes while constructing description)
        public SpectreLv1BattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "VampiricTouchIcon.png";
        }
    }



    public class SkeletonLv2BattleUnit : BattleUnit
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
            get { return 3; }
        }

        public override int MaxHp
        {
            get { return 4; }
        }

        public override int Level
        {
            get { return 2; }
        }

        public SkeletonLv2BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
                
        //Only for temporary unit (to read base attributes while constructing description)
        public SkeletonLv2BattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "SlayerIcon.png";
        }
    }

    public class ZombieLv2BattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 3; }
        }

        public override int BaseDefense
        {
            get { return 3; }
        }

        public override int BaseMovementDistance
        {
            get { return 2; }
        }

        public override int MaxHp
        {
            get { return 4; }
        }

        public override int Level
        {
            get { return 2; }
        }

        public ZombieLv2BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
                
        //Only for temporary unit (to read base attributes while constructing description)
        public ZombieLv2BattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "PlagueIcon.png";
        }
    }

    public class BansheeLv2BattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 4; }
        }

        public override int BaseDefense
        {
            get { return 2; }
        }

        public override int BaseMovementDistance
        {
            get { return 2; }
        }

        public override int MaxHp
        {
            get { return 4; }
        }

        public override int Level
        {
            get { return 2; }
        }

        public BansheeLv2BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
                
        //Only for temporary unit (to read base attributes while constructing description)
        public BansheeLv2BattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "BloodMadnessIcon.png";
        }
    }

    public class SpectreLv2BattleUnit : BattleUnit
    {
        public override int BaseAttack
        {
            get { return 2; }
        }

        public override int BaseDefense
        {
            get { return 4; }
        }

        public override int BaseMovementDistance
        {
            get { return 3; }
        }

        public override int MaxHp
        {
            get { return 4; }
        }

        public override int Level
        {
            get { return 2; }
        }

        public SpectreLv2BattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
                
        //Only for temporary unit (to read base attributes while constructing description)
        public SpectreLv2BattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "VampiricTouchIcon.png";
        }
    }



    public class HeroBattleUnit : BattleUnit
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

        public override int MaxHp
        {
            get { return 15; }
        }

        public override int Level
        {
            get { return 0; }
        }

        public HeroBattleUnit(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(player, terrain, tileIndex)
        {
        }
                
        //Only for temporary unit (to read base attributes while constructing description)
        public HeroBattleUnit()
        {
        }

        protected override string GetImageFileName()
        {
            return "PackReinforcementIcon.png";
        }
    }
}
