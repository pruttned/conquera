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
        private Point mAttack;
        private int mMovementDistance;

        private int mAttackPreventerCnt;
        private int mMovementPreventerCnt;

        private int mFlyEnablerCnt;
        private int mFirstStrikeEnablerCnt;
        private int mFlyPreventerCnt;
        private int mFirstStrikePreventerCnt;

        private int mBersekerEnablerCnt;

        private List<IBattleUnitSpellEffect> mSpellEffects = new List<IBattleUnitSpellEffect>();


        public string ImageFileName { get; private set; }

        public BattlePlayer Player { get; private set; }
        public HexTerrain Terrain { get { return mTerrain; } }

        public Point BaseAttack { get; private set; }
        public int BaseDefense { get; private set; }
        public int BaseMovementDistance { get; private set; }
        public int MaxHp { get; private set; }

        public bool HasEnabledAttack { get { return 0 == AttackPreventerCnt; } }
        public bool HasEnabledMovement { get { return 0 == MovementPreventerCnt; } }

        public bool IsFlying { get { return 0 < FlyEnablerCnt && 0 == FlyPreventerCnt; } }
        public bool HasFirstStrike { get { return 0 < FirstStrikeEnablerCnt && 0 == FirstStrikePreventerCnt; } }

        public bool IsBerserker { get { return 0 < BerserkerEnablerCnt; } }

        public int FlyEnablerCnt
        {
            get { return mFlyEnablerCnt; }
            set
            {
                mFlyEnablerCnt = Math.Max(0, value);
                UpdateGraphics();
            }
        }
        public int FirstStrikeEnablerCnt
        {
            get { return mFirstStrikeEnablerCnt; }
            set
            {
                mFirstStrikeEnablerCnt = Math.Max(0, value);
                UpdateGraphics();
            }
        }
        public int FlyPreventerCnt
        {
            get { return mFlyPreventerCnt; }
            set
            {
                mFlyPreventerCnt = Math.Max(0, value);
                UpdateGraphics();
            }
        }
        public int FirstStrikePreventerCnt
        {
            get { return mFirstStrikePreventerCnt; }
            set
            {
                mFirstStrikePreventerCnt = Math.Max(0, value);
                UpdateGraphics();
            }
        }
        public int AttackPreventerCnt
        {
            get { return mAttackPreventerCnt; }
            set
            {
                mAttackPreventerCnt = Math.Max(0, value);
                UpdateGraphics();
            }
        }
        public int MovementPreventerCnt
        {
            get { return mMovementPreventerCnt; }
            set
            {
                mMovementPreventerCnt = Math.Max(0, value);
                UpdateGraphics();
            }
        }
        public int BerserkerEnablerCnt
        {
            get { return mBersekerEnablerCnt; }
            set
            {
                mBersekerEnablerCnt = Math.Max(0, value);
                UpdateGraphics();
            }
        }

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

        public Point Attack
        {
            get { return mAttack; }
            private set
            {
                mAttack = new Point(Math.Max(0, value.X), Math.Max(0, value.Y));
                mAttack.Y = Math.Max(mAttack.X, mAttack.Y);
            }
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

        public BattleUnit(Point baseAttack, int baseDefense, int baseMovementDistance, int maxHp, bool isFlying, bool hasFirstStrike,
            string imageFileName, BattlePlayer player, HexTerrain terrain, Point tileIndex)
        {
            if (string.IsNullOrEmpty(imageFileName)) throw new ArgumentNullException("imageName");
            if (null == player) throw new ArgumentNullException("player");
            if (null == terrain) throw new ArgumentNullException("terrain");

            BaseAttack = baseAttack;
            BaseDefense = baseDefense;
            BaseMovementDistance = baseMovementDistance;
            mHp = MaxHp = maxHp;

            ImageFileName = imageFileName;

            IsKilled = false;
            Player = player;
            mTerrain = terrain;
            mTileIndex = tileIndex;
            mFlyEnablerCnt = isFlying ? 1 : 0;
            mFirstStrikeEnablerCnt = hasFirstStrike ? 1 : 0;

            if (null != terrain[tileIndex.X, tileIndex.Y].Unit)
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

            TextBlock levelTextBlock = new TextBlock();
            levelTextBlock.Text = Level.ToString();
            levelTextBlock.Foreground = Brushes.Yellow;
            levelTextBlock.Background = Brushes.Black;
            levelTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            levelTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Children.Add(levelTextBlock);
        }

        private void UpdateSpellEffectsToolTip()
        {
            StackPanel panel = new StackPanel();
            panel.Children.Add(new TextBlock() { Text = GetType().Name, FontWeight = System.Windows.FontWeights.Bold});            

            foreach (IBattleUnitSpellEffect effect in mSpellEffects)
            {
                panel.Children.Add(new TextBlock() { Text = effect.Description });
            }
            ToolTip = panel;
        }

        //Only for temporary unit (for reading base attributes while constructing description)
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

            UpdateSpellEffectsToolTip();

            if (!IsKilled)
            {
                OnTurnStartImpl(turnNum);
                if (!IsKilled)
                {
                    UpdateGraphics();
                }
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

            Logger.Log(Name + " has been killed", mTerrain[mTileIndex.X, mTileIndex.Y]);
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
                        if ((sibling.IsPassableAndEmpty || IsFlying) && !CheckedPoints.Contains(index))
                        {
                            if (sibling.IsPassableAndEmpty)
                            {
                                points.Add(index);
                            }
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
        /// <param name="isPrebatlePhase">Take damage only from units with first strike</param>
        /// <returns></returns>
        public int ComputeDamageFromEnemies(bool isPrebatlePhase)
        {
            int damage = 0;
            mTerrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && (sibling.Unit.IsBerserker || sibling.Unit.Player != Player) && sibling.Unit.HasEnabledAttack && ((isPrebatlePhase && sibling.Unit.HasFirstStrike) || (!isPrebatlePhase && !sibling.Unit.HasFirstStrike)))
                    {
                        damage += MathExt.Random.Next(sibling.Unit.Attack.X, sibling.Unit.Attack.Y + 1);
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
            mPropertiesTextBlock.Text += string.Format("[{0}{1}{2}{3}{4}]\n", (IsFlying ? " F" : null), (HasFirstStrike ? " Fs" : null), (!HasEnabledMovement ? " MD" : null), (!HasEnabledAttack ? " AD" : null), (IsBerserker ? " B" : null));
            mPropertiesTextBlock.Text += string.Format("A = {0}-{1}({6}-{7})\nD = {2}({8})\nM = {3}({9})\nHp = {4}/{5}", BaseAttack.X, BaseAttack.Y, BaseDefense, BaseMovementDistance, Hp, MaxHp,
                Attack.X, Attack.Y, Defense, MovementDistance);
            mBorder.BorderBrush = (!HasMovedThisTurn && HasEnabledMovement && Player.IsActive && HasEnabledMovement ? Brushes.Yellow : Brushes.Black);
        }

        public void AddSpellEffect(int turnNum, IBattleUnitSpellEffect spellEffect)
        {
            mSpellEffects.Add(spellEffect);
            spellEffect.OnCast(turnNum, this);
            UpdateSpellEffectsToolTip();
        }

        public void RemoveSpellEffects(Predicate<IBattleUnitSpellEffect> effectPredicate)
        {
            for (int i = mSpellEffects.Count - 1; i >= 0 && !IsKilled; --i)
            {
                if (effectPredicate(mSpellEffects[i]))
                {
                    mSpellEffects[i].OnEnd();
                    mSpellEffects.RemoveAt(i);
                }
            }
            UpdateSpellEffectsToolTip();
        }

        public Image CreateImage()
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(System.IO.Path.Combine(mBattleUnitImagesDirectory, ImageFileName)));
            image.Width = 22;
            image.Height = 22;
            return image;
        }

        protected virtual void OnTurnStartImpl(int turnNum) { }

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
                        defenseFromAllies+=5;
                    }
                });

            Defense = BaseDefense + defenseFromAllies + GetDefenseModifier();

            UpdateGraphics();
        }
        private void UpdateAttack()
        {
            Point modif = GetAttackModifier();
            mAttack.X = BaseAttack.X + modif.X;
            mAttack.Y = BaseAttack.Y + modif.Y;

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
        private Point GetAttackModifier()
        {
            Point sum = new Point();
            foreach (var modif in mAttackModifiers)
            {
                Point m = modif.GetModifier(this);
                sum.X += m.X;
                sum.Y += m.Y;
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

    public class SoldierAtt : BattleUnit
    {
        public SoldierAtt(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(30, 40) //attack
            , 20 //defense
            , 3 //movement distance
            , 40 //maxHp
            , false //flying
            , false //first strike
            , "BloodMadnessIcon.png" //image
            , player, terrain, tileIndex)
        { }
    }
    public class SoldierBl : BattleUnit
    {
        public SoldierBl(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(20, 30) //attack
            , 30 //defense
            , 3 //movement distance
            , 40 //maxHp
            , false //flying
            , false //first strike
            , "BloodMadnessIcon.png" //image
            , player, terrain, tileIndex)
        { }
    }
    public class SoldierDef : BattleUnit
    {
        public SoldierDef(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(10, 40) //attack
            , 40 //defense
            , 3 //movement distance
            , 40 //maxHp
            , false //flying
            , false //first strike
            , "BloodMadnessIcon.png" //image
            , player, terrain, tileIndex)
        { }
    }


    public class ScoutBase : BattleUnit
    {
        public ScoutBase(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(15, 25) //attack
            , 20 //defense
            , 6 //movement distance
            , 30 //maxHp
            , false //flying
            , false //first strike
            , "SlayerIcon.png" //image
            , player, terrain, tileIndex)
        { }
    }

    public class ScoutFs : BattleUnit
    {
        public ScoutFs(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(15, 20) //attack
            , 15 //defense
            , 5 //movement distance
            , 30 //maxHp
            , false //flying
            , true //first strike
            , "SlayerIcon.png" //image
            , player, terrain, tileIndex)
        { }
    }

    public class ScoutFly : BattleUnit
    {
        public ScoutFly(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(15, 20) //attack
            , 15 //defense
            , 4 //movement distance
            , 30 //maxHp
            , true //flying
            , false //first strike
            , "SlayerIcon.png" //image
            , player, terrain, tileIndex)
        { }
    }

    #region Support base
    public class SupportHealBase : BattleUnit
    {
        int mHpIncrement;
        
        public SupportHealBase(Point baseAttack, int baseDefense, int baseMovementDistance, int maxHp, bool isFlying, bool hasFirstStrike,
                int hpIncrement, 
                string imageFileName, BattlePlayer player, HexTerrain terrain, Point tileIndex)
            :base(baseAttack, baseDefense, baseMovementDistance, maxHp, isFlying, hasFirstStrike, imageFileName, player, terrain, tileIndex)
        {
            mHpIncrement = hpIncrement;
        }

        protected override void OnTurnStartImpl(int turnNum)
        {
            base.OnTurnStartImpl(turnNum);

            Terrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        sibling.Unit.Hp += mHpIncrement;
                    }
                });
        }
    }
    public class SupportAttBase : BattleUnit
    {
        Point mAttIncrement;

        public SupportAttBase(Point baseAttack, int baseDefense, int baseMovementDistance, int maxHp, bool isFlying, bool hasFirstStrike,
                Point attIncrement, 
                string imageFileName, BattlePlayer player, HexTerrain terrain, Point tileIndex)
            :base(baseAttack, baseDefense, baseMovementDistance, maxHp, isFlying, hasFirstStrike, imageFileName, player, terrain, tileIndex)
        {
            mAttIncrement = attIncrement;
        }

        protected override void OnTurnStartImpl(int turnNum)
        {
            base.OnTurnStartImpl(turnNum);

            Terrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        sibling.Unit.AddSpellEffect(turnNum, new ConstIncAttackBattleUnitSpellEffect(mAttIncrement, 1));
                    }
                });
        }
    }
    public class SupportDefBase : BattleUnit
    {
        int mDefIncrement;

        public SupportDefBase(Point baseAttack, int baseDefense, int baseMovementDistance, int maxHp, bool isFlying, bool hasFirstStrike,
                int defIncrement,
                string imageFileName, BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(baseAttack, baseDefense, baseMovementDistance, maxHp, isFlying, hasFirstStrike, imageFileName, player, terrain, tileIndex)
        {
            mDefIncrement = defIncrement;
        }

        protected override void OnTurnStartImpl(int turnNum)
        {
            base.OnTurnStartImpl(turnNum);

            Terrain.ForEachSibling(TileIndex,
                sibling =>
                {
                    if (null != sibling.Unit && sibling.Unit.Player == Player)
                    {
                        sibling.Unit.AddSpellEffect(turnNum, new ConstIncDefenseBattleUnitSpellEffect(mDefIncrement, 1));
                    }
                });
        }
    }
    #endregion Support base

    public class SupportHeal : SupportHealBase
    {
        public SupportHeal(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(5, 10) //attack
            , 5 //defense
            , 3 //movement distance
            , 20 //maxHp
            , false //flying
            , false //first strike
            , 3 //hpInc
            , "VampiricTouchIcon.png" //image
            , player, terrain, tileIndex)
        {}
    }
    public class SupportAtt : SupportAttBase
    {
        public SupportAtt(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(5, 10) //attack
            , 5 //defense
            , 3 //movement distance
            , 20 //maxHp
            , false //flying
            , false //first strike,
            , new Point(5,5) //attInc
            , "SpikesIcon.png" //image
            , player, terrain, tileIndex)
        {}
    }
    public class SupportDef : SupportDefBase
    {
        public SupportDef(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            new Point(5, 10) //attack
            , 5 //defense
            , 3 //movement distance
            , 20 //maxHp
            , false //flying
            , false //first strike,
            , 10 //defInc
            , "PackReinforcementIcon.png" //image
            , player, terrain, tileIndex)
        { }
    }
}
