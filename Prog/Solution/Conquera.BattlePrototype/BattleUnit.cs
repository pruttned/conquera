/////////////////////////////////////////////////////////////////////
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
using System.Windows.Shapes;

namespace Conquera.BattlePrototype
{
    public class Die
    {
        public static Die D6 { get; private set; }
        public static Die D8 { get; private set; }
        public static Die D10 { get; private set; }
        public static Die D12 { get; private set; }
        public static Die D20 { get; private set; }

        public int MaxNum { get; private set; }

        static Die()
        {
            D6 = new Die(6);
            D8 = new Die(8);
            D10 = new Die(10);
            D12 = new Die(12);
            D20 = new Die(20);
        }

        public int Roll()
        {
            return MathExt.Random.Next(1, MaxNum + 1);
        }

        private Die(int maxNum)
        {
            MaxNum = maxNum;
        }
    }

    public struct DieAttackRoll
    {
        public Die Die;
        public int Num;
        public bool IsHit;
    }

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
        
        private static int[] mAdditionalAttackPoints = new int[] { -2, 2 };

        //promoted collections
        private static HashSet<BattleUnit> CheckedUnits = new HashSet<BattleUnit>();
        private static HashSet<Point> CheckedPoints = new HashSet<Point>();
        private static Queue<HexTileSeed> Seeds = new Queue<HexTileSeed>();

        private HexTerrain mTerrain;

        private Point mTileIndex;

        private List<IBattleUnitDefenseModifier> mDefenseModifiers = new List<IBattleUnitDefenseModifier>();
        private List<IBattleUnitAttackModifier> mAttackModifiers = new List<IBattleUnitAttackModifier>();
        private List<IBattleUnitMovementDistanceModifier> mMovementDistanceModifiers = new List<IBattleUnitMovementDistanceModifier>();


        private TextBlock mPropertiesTextBlock;
        private Border mBorder;
        private bool mHasMovedThisTurn;
        private bool mIsSelected = false;

        private int mMovementDistance;

        private int mAttackPreventerCnt;
        private int mMovementPreventerCnt;

        private int mFlyEnablerCnt;
        private int mFirstStrikeEnablerCnt;
        private int mFlyPreventerCnt;
        private int mFirstStrikePreventerCnt;

        private int mBersekerEnablerCnt;

        private List<IBattleUnitSpellEffect> mSpellEffects = new List<IBattleUnitSpellEffect>();

        private Line mMovementLine = null;
        private ListBox mDieRollListBox;
        private UnitDamages mDamages;
        private System.Windows.Shapes.Rectangle mHasDamageIndicator;

        public string ImageFileName { get; private set; }

        public BattlePlayer Player { get; private set; }
        public HexTerrain Terrain { get { return mTerrain; } }

        public int BaseMovementDistance { get; private set; }

        public bool HasEnabledAttack { get { return 0 == AttackPreventerCnt; } }
        public bool HasEnabledMovement { get { return 0 == MovementPreventerCnt; } }

        public bool IsFlying { get { return 0 < FlyEnablerCnt && 0 == FlyPreventerCnt; } }
        public bool HasFirstStrike { get { return 0 < FirstStrikeEnablerCnt && 0 == FirstStrikePreventerCnt; } }

        public bool IsBerserker { get { return 0 < BerserkerEnablerCnt; } }

        public HexDirection Direction { get; set; }

        public int AttackDistance { get; private set; }

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

                    PreviousTileIndex = mTileIndex;
                    mTerrain[PreviousTileIndex.Value.X, PreviousTileIndex.Value.Y].Unit = null;

                    mTileIndex = value;

                    //todo
                    //UpdatePositionFromIndex();
                    mTerrain[mTileIndex.X, mTileIndex.Y].Unit = this;

                    if (null != TileIndexChanged)
                    {
                        TileIndexChanged.Invoke(this, PreviousTileIndex.Value);
                    }
                }
            }
        }

        public Point? PreviousTileIndex { get; private set; }

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

        public UnitDamages Damages
        {
            get { return mDamages; }
            set
            {
                mDamages = value;

                if (mDamages != null)
                {
                    mHasDamageIndicator = new System.Windows.Shapes.Rectangle();
                    mHasDamageIndicator.Width = 15;
                    mHasDamageIndicator.Height = 15;
                    mHasDamageIndicator.Fill = Brushes.GreenYellow;
                    Children.Add(mHasDamageIndicator);
                }
                else
                {
                    Children.Remove(mHasDamageIndicator);
                }
            }
        }

        public BattleUnit(int attackDistance, int baseMovementDistance, bool isFlying, bool hasFirstStrike,
            string imageFileName, BattlePlayer player, HexTerrain terrain, Point tileIndex)
        {
            if (string.IsNullOrEmpty(imageFileName)) throw new ArgumentNullException("imageName");
            if (null == player) throw new ArgumentNullException("player");
            if (null == terrain) throw new ArgumentNullException("terrain");

            BaseMovementDistance = baseMovementDistance;

            ImageFileName = imageFileName;

            Direction = HexDirection.UperRight;
            AttackDistance = attackDistance;
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

            UpdateMovementDistance();

            TextBlock levelTextBlock = new TextBlock();
            levelTextBlock.Text = Level.ToString();
            levelTextBlock.Foreground = Brushes.Yellow;
            levelTextBlock.Background = Brushes.Black;
            levelTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            levelTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Children.Add(levelTextBlock);

            MouseEnter += new System.Windows.Input.MouseEventHandler(BattleUnit_MouseEnter);
            MouseLeave += new System.Windows.Input.MouseEventHandler(BattleUnit_MouseLeave);
        }

        //Only for temporary unit (for reading base attributes while constructing description)
        public BattleUnit()
        {
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
                ////constraint movement  - temp? (Cavalry  without constraint ? )
                //bool hasEnemySibling = false;
                //Terrain.ForEachSibling(TileIndex,
                //    tile =>
                //    {
                //        if (!hasEnemySibling && null != tile.Unit && tile.Unit.Player != Player)
                //        {
                //             hasEnemySibling = true;
                //        }
                //    });
                //if (hasEnemySibling)
                //{
                //    AddSpellEffect(turnNum, new ConstIncMovementDistanceBattleUnitSpellEffect(-BaseMovementDistance + 1, 1)); 
                //}



                OnTurnStartImpl(turnNum);
                if (!IsKilled)
                {
                    UpdateGraphics();
                }
            }

            UpdateSpellEffectsToolTip();
        }

        public void Kill()
        {
            IsKilled = true;
            if (Player.Units.Remove(this))
            {
                mTerrain[mTileIndex.X, mTileIndex.Y].Unit = null;
            }
            Logger.Log(GetType().Name + " died", mTerrain[mTileIndex.X, mTileIndex.Y]);
        }

        //public void GetEnemyUnitsInRange(List<BattleUnit> units)
        //{

        //}

        public void ForEachEnemyInRange(Action<BattleUnit> action)
        {
            ForEachEnemyInRange(MovementDistance + 1, action);
        }

        public void ForEachEnemyInRange(int range, Action<BattleUnit> action)
        {
            CheckedUnits.Clear();
            ForEachPassableCellInRange(range,
                index => Terrain.ForEachSibling(index,
                    tile =>
                    {
                        if (null != tile.Unit && tile.Unit.Player != Player && !CheckedUnits.Contains(tile.Unit))
                        {
                            CheckedUnits.Add(tile.Unit);
                            action(tile.Unit);
                        }
                    }));
        }

        public void ForEachPassableCellInRange(int range, Action<Point> action)
        {
            Seeds.Clear();
            CheckedPoints.Clear();

            Seeds.Enqueue(new HexTileSeed(mTerrain[TileIndex], range));
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
                                action(index);
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

        /// <summary>
        /// Gets all poitions where is possible for unit to move
        /// </summary>
        /// <param name="points"></param>
        public void GetPossibleMoves(List<Point> points)
        {
            ForEachPassableCellInRange(MovementDistance, index => points.Add(index));
        }

        /// <summary>
        /// Gets all poitions where is possible for unit to move
        /// </summary>
        /// <param name="points"></param>
        public void GetPossibleMoves(HashSet<Point> points)
        {
            ForEachPassableCellInRange(MovementDistance, index => points.Add(index));
        }

        public bool CanMoveTo(Point index)
        {
            //todo  refactor a* / depth first

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

        ///// <summary>
        ///// No defense is considered here
        ///// </summary>
        ///// <param name="isPrebatlePhase">Take damage only from units with first strike</param>
        ///// <returns></returns>
        //public int ComputeDamageFromEnemies(bool isPrebatlePhase)
        //{
        //    int damage = 0;
        //    //mTerrain.ForEachSibling(TileIndex,
        //    //    sibling =>
        //    //    {
        //    //        if (null != sibling.Unit && (sibling.Unit.IsBerserker || sibling.Unit.Player != Player) && sibling.Unit.HasEnabledAttack && ((isPrebatlePhase && sibling.Unit.HasFirstStrike) || (!isPrebatlePhase && !sibling.Unit.HasFirstStrike)))
        //    //        {
        //    //            damage += MathExt.Random.Next(sibling.Unit.Attack.X, sibling.Unit.Attack.Y + 1);
        //    //        }
        //    //    });

        //    return damage;
        //}


        /// <summary>
        /// All attack rolls or null in case that the unit is not capable of attacking the specified target (out of range, ...)
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public IList<DieAttackRoll> RollDiceAgainst(BattleUnit target)
        {
            if (null == target) throw new ArgumentNullException("target");

            if (!HasEnabledAttack || AttackDistance != HexHelper.GetDistance(TileIndex, target.TileIndex))
            {
                return null;
            }

            //get main attack point
            Point mainAttackPoint = TileIndex;
            for (int i = 0; i < AttackDistance; ++i)//todo cahce
            {
                mainAttackPoint = HexHelper.GetSibling(mainAttackPoint, Direction);
            }
            if (mainAttackPoint == target.TileIndex) //is main target
            {
                Die die = GetDieAgainst(target);
                int rollNum1 = die.Roll();
                int rollNum2 = die.Roll();
                return new DieAttackRoll[]
                {
                    new DieAttackRoll() {Die = die, Num = rollNum1, IsHit = (6 <= rollNum1)},
                    new DieAttackRoll() {Die = die, Num = rollNum2, IsHit = (6 <= rollNum2)},
                };
            }


            //additional points
            foreach (var additionalAttackPointRot in mAdditionalAttackPoints)
            {
                Point additionalAttackPoint = HexHelper.GetSibling(mainAttackPoint, HexHelper.RotateDirection(Direction, additionalAttackPointRot));
                if (target.TileIndex == additionalAttackPoint)
                {
                    Die die = GetDieAgainst(target);
                    int rollNum = die.Roll();
                    return new DieAttackRoll[] { new DieAttackRoll() { Die = die, Num = rollNum, IsHit = (6 <= rollNum) } };
                }
            }

            return null;
        }

        public void Move(int turnNum, Point tileIndex)
        {
            if(tileIndex != TileIndex)
            {
                HasMovedThisTurn = true;
                Point oldIndex = TileIndex;
                TileIndex = tileIndex;

                OnMove(turnNum, oldIndex, tileIndex);
            }
        }

        public void UpdateGraphics()
        {
            mPropertiesTextBlock.Text = IsSelected ? "[SELECTED]\n" : "[]\n";
            mPropertiesTextBlock.Text += string.Format("[{0}{1}{2}{3}{4}]\n", (IsFlying ? " F" : null), (HasFirstStrike ? " Fs" : null), (!HasEnabledMovement ? " MD" : null), (!HasEnabledAttack ? " AD" : null), (IsBerserker ? " B" : null));
            mPropertiesTextBlock.Text += string.Format("M = {0}({1})", BaseMovementDistance, MovementDistance);
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

        protected virtual void OnMove(int turnNum, Point oldIndex, Point tileIndex)
        {
        }

        protected virtual void OnTurnStartImpl(int turnNum) { }

        protected abstract Die GetDieAgainst(BattleUnit target);

        private void UpdateMovementDistance()
        {
            MovementDistance = BaseMovementDistance + GetMovementDistanceModifier();

            UpdateGraphics();
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

        private void BattleUnit_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Damages != null)
            {
                foreach (UnitAttack attack in Damages.Attacks)
                {
                    attack.Attacker.ShowDieRolls(attack.AttackRolls);
                }
            }
            else if (PreviousTileIndex != null)
            {
                mMovementLine = new Line();
                mMovementLine.StrokeThickness = 5;
                mMovementLine.Stroke = Brushes.Yellow;

                HexTerrainTile previousTile = mTerrain[(int)PreviousTileIndex.Value.X, (int)PreviousTileIndex.Value.Y];
                mMovementLine.X1 = Canvas.GetLeft(previousTile) + previousTile.ActualWidth / 2.0;
                mMovementLine.Y1 = Canvas.GetTop(previousTile) + previousTile.ActualHeight / 2.0;

                HexTerrainTile currentTile = mTerrain[(int)TileIndex.X, (int)TileIndex.Y];
                mMovementLine.X2 = Canvas.GetLeft(currentTile) + currentTile.ActualWidth / 2.0;
                mMovementLine.Y2 = Canvas.GetTop(currentTile) + currentTile.ActualHeight / 2.0;

                GetParent<Canvas>(this).Children.Add(mMovementLine);
            }
        }

        private void BattleUnit_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Damages != null)
            {
                foreach (UnitAttack attack in Damages.Attacks)
                {
                    attack.Attacker.HideDieRolls();
                }
            }
            else if (mMovementLine != null)
            {
                GetParent<Canvas>(this).Children.Remove(mMovementLine);
            }
        }

        private void ShowDieRolls(IList<DieAttackRoll> rolls)
        {
            mDieRollListBox = new ListBox();
            foreach (DieAttackRoll roll in rolls)
            {
                TextBlock block = new TextBlock();
                block.Text = string.Format("{0} / D{1}", roll.Num, roll.Die.MaxNum);
                block.Background = roll.IsHit ? Brushes.Red : Brushes.Transparent;
                mDieRollListBox.Items.Add(block);
            }
            Children.Add(mDieRollListBox);
        }

        private void HideDieRolls()
        {
            Children.Remove(mDieRollListBox);
        }

        private T GetParent<T>(System.Windows.DependencyObject element) where T : System.Windows.DependencyObject
        {
            System.Windows.DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null)
            {
                return null;
            }
            if (parent is T)
            {
                return (T)parent;
            }
            return GetParent<T>(parent);
        }

        private void UpdateSpellEffectsToolTip()
        {
            StackPanel panel = new StackPanel();
            panel.Children.Add(new TextBlock() { Text = GetType().Name, FontWeight = System.Windows.FontWeights.Bold });

            foreach (IBattleUnitSpellEffect effect in mSpellEffects)
            {
                panel.Children.Add(new TextBlock() { Text = effect.Description });
            }
            ToolTip = panel;
        }
    }

    public class Swordsman : BattleUnit
    {
        public Swordsman(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            1,
            20 //movement distance
            , false //flying
            , false //first strike
            , "Swordsman.png" //image
            , player, terrain, tileIndex)
        { }

        protected override Die GetDieAgainst(BattleUnit target)
        {
            return Die.D8;
        }
    }

    public class Archer : BattleUnit
    {
        public Archer(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            2,
            20 //movement distance
            , false //flying
            , false //first strike
            , "Archer.png" //image
            , player, terrain, tileIndex)
        { }

        protected override Die GetDieAgainst(BattleUnit target)
        {
            return Die.D6;
        }
    }

    public class Cavalry : BattleUnit
    {
        public Cavalry(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            1,
            40 //movement distance
            , false //flying
            , false //first strike
            , "Cavalry.png" //image
            , player, terrain, tileIndex)
        { }

        protected override Die GetDieAgainst(BattleUnit target)
        {
            return Die.D10;
        }

        protected override void OnMove(int turnNum, Point oldIndex, Point tileIndex)
        {
            if (1 < HexHelper.GetDistance(oldIndex, tileIndex))
            {
                bool hasEnemySpearmanSibling = false;
                bool hasEnemySibling = false;
                Terrain.ForEachSibling(tileIndex, tile =>
                    {
                        if (!hasEnemySpearmanSibling && null != tile.Unit && tile.Unit.Player != Player)
                        {
                            if (tile.Unit is Spearman)
                            {
                                hasEnemySpearmanSibling = true;
                            }
                            hasEnemySibling = true;
                        }
                    });

                if (!hasEnemySpearmanSibling && hasEnemySibling)
                {
                    AddSpellEffect(turnNum, new FirstStrikeBattleUnitSpellEffect(1));
                }
            }
        }
    }

    public class Spearman : BattleUnit
    {
        public Spearman(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            1,
            20 //movement distance
            , false //flying
            , false //first strike
            , "Spearman.png" //image
            , player, terrain, tileIndex)
        { }

        protected override Die GetDieAgainst(BattleUnit target)
        {
            return (target is Cavalry) ? Die.D10 : Die.D6;
        }
    }
}
