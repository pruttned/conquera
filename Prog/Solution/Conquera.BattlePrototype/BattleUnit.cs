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

        private int mHp;

        private List<IBattleUnitAttackModifier> mAttackModifiers = new List<IBattleUnitAttackModifier>();
        private List<IBattleUnitMovementDistanceModifier> mMovementDistanceModifiers = new List<IBattleUnitMovementDistanceModifier>();


        private TextBlock mPropertiesTextBlock;
        private Border mBorder;
        private bool mHasMovedThisTurn;
        private bool mIsSelected = false;

        private int mMovementDistance;

        private int mAttackPreventerCnt;
        private int mMovementPreventerCnt;
        private int mDamagePreventerCnt;

        private int mDamage = 0;

        private List<IBattleUnitSpellEffect> mSpellEffects = new List<IBattleUnitSpellEffect>();

        private Line mMovementLine = null;
        private ListBox mDieRollListBox;
        private UnitDamages mDamages;
        private System.Windows.Shapes.Rectangle mHasDamageIndicator;
        private Line mDirectionLine;
        private Grid mGridInBorder;
        private HexDirection mDirection;
        private List<Button> mDirectionButtons;
        private Canvas mCanvas;

        public string ImageFileName { get; private set; }

        public BattlePlayer Player { get; private set; }
        public HexTerrain Terrain { get { return mTerrain; } }

        public int BaseMovementDistance { get; private set; }

        public bool HasEnabledAttack { get { return 0 == AttackPreventerCnt; } }
        public bool HasEnabledMovement { get { return 0 == MovementPreventerCnt; } }

        public HexDirection Direction
        {
            get { return mDirection;; }
            set
            {
                mDirection = value;
                UpdateDirectionLine();
            }
        }

        public int AttackDistance { get; private set; }

        public int MaxHp { get; private set; }
        public int Hp
        {
            get { return mHp; }
            set
            {
                mHp = MathExt.Clamp(value, 0, MaxHp);
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

        public int Damage
        {
            get { return mDamage; }
            set
            {
                mDamage = Math.Max(0, value);
                UpdateGraphics();
            }
        }

        public int DamagePreventerCnt
        {
            get { return mDamagePreventerCnt; }
            set
            {
                mDamagePreventerCnt = Math.Max(0, value);
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
                    mHasDamageIndicator.Stroke = Brushes.Black;
                    mHasDamageIndicator.Width = 15;
                    mHasDamageIndicator.Height = 15;
                    int hitCnt = value.GetHitCnt();
                    mHasDamageIndicator.Fill = (2 <= hitCnt ? Brushes.OrangeRed : (1 == hitCnt ? Brushes.Yellow : Brushes.Gray));
                    Children.Add(mHasDamageIndicator);
                }
                else
                {
                    Children.Remove(mHasDamageIndicator);
                }
            }
        }

        public BattleUnit(int attackDistance, int baseMovementDistance, int maxHp,
            string imageFileName, BattlePlayer player, HexTerrain terrain, Point tileIndex)
        {
            if (string.IsNullOrEmpty(imageFileName)) throw new ArgumentNullException("imageName");
            if (null == player) throw new ArgumentNullException("player");
            if (null == terrain) throw new ArgumentNullException("terrain");

            BaseMovementDistance = baseMovementDistance;

            ImageFileName = imageFileName;
            
            AttackDistance = attackDistance;
            IsKilled = false;
            Player = player;
            mTerrain = terrain;
            mTileIndex = tileIndex;
            mHp = MaxHp = maxHp;

            if (null != terrain[tileIndex.X, tileIndex.Y].Unit)
            {
                throw new ArgumentException("Destination tile already contains a unit");
            }

            terrain[tileIndex.X, tileIndex.Y].Unit = this;

            player.Units.Add(this);

            //Graphics
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            mCanvas = new Canvas()
            {
                Width = HexHelper.TileW,
                Height = mTerrain[0, 0].GetCornerPosition(HexTileCorner.Top).Y - mTerrain[0, 0].GetCornerPosition(HexTileCorner.Down).Y,
            };
            Children.Add(mCanvas);
            mDirectionLine = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 5,
                X1 = mCanvas.Width / 2,
                Y1 = mCanvas.Height / 2,
            };
            mCanvas.Children.Add(mDirectionLine);
            UpdateDirectionLine();

            if (tileIndex.X > terrain.Width / 2)
            {
                Direction = HexDirection.Left;
            }
            else
            {
                Direction = HexDirection.Right;
            }

            mBorder = new Border();
            mBorder.Width = 80;
            mBorder.Height = 80;           
            mBorder.BorderThickness = new System.Windows.Thickness(5);
            Children.Add(mBorder);
            mGridInBorder = new Grid();
            mBorder.Child = mGridInBorder;

            mPropertiesTextBlock = new TextBlock();
            mPropertiesTextBlock.Background = new SolidColorBrush(Color.FromArgb(255, player.Color.R, player.Color.G, player.Color.B));
            mPropertiesTextBlock.Foreground = Brushes.White;
            mPropertiesTextBlock.FontSize = 10;
            mGridInBorder.Children.Add(mPropertiesTextBlock);
            
            UpdateGraphics();

            Image image = CreateImage();
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            image.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            mGridInBorder.Children.Add(image);

            UpdateMovementDistance();

            TextBlock levelTextBlock = new TextBlock();
            levelTextBlock.Text = Level.ToString();
            levelTextBlock.Foreground = Brushes.Yellow;
            levelTextBlock.Background = Brushes.Black;
            levelTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            levelTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            mGridInBorder.Children.Add(levelTextBlock);

            mDirectionButtons = new List<Button>()
            {
                CreateDirectionButton(HexDirection.Left),
                CreateDirectionButton(HexDirection.LowerLeft),
                CreateDirectionButton(HexDirection.LowerRight),
                CreateDirectionButton(HexDirection.Right),
                CreateDirectionButton(HexDirection.UperLeft),
                CreateDirectionButton(HexDirection.UperRight)
            };

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
                        if (sibling.IsPassableAndEmpty && !CheckedPoints.Contains(index))
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
            int damageNum = 5;
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
                    new DieAttackRoll() {Die = die, Num = rollNum1, IsHit = (damageNum <= rollNum1)},
                    new DieAttackRoll() {Die = die, Num = rollNum2, IsHit = (damageNum <= rollNum2)},
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
                    return new DieAttackRoll[] { new DieAttackRoll() { Die = die, Num = rollNum, IsHit = (damageNum <= rollNum) } };
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
            mPropertiesTextBlock.Text += string.Format("[{0}{1}]\n", (!HasEnabledMovement ? " MD" : null), (!HasEnabledAttack ? " AD" : null));
            mPropertiesTextBlock.Text += string.Format("Hp = {0}/{1}\nD = {2}({3})\nM = {4}({5})", Hp, MaxHp, Damage, DamagePreventerCnt, BaseMovementDistance, MovementDistance);
            mBorder.BorderBrush = (!HasMovedThisTurn && HasEnabledMovement && Player.IsActive && HasEnabledMovement ? Brushes.Yellow : Brushes.Black);

            if (mDirectionButtons != null)
            {
                foreach (Button button in mDirectionButtons)
                {
                    button.Visibility = IsSelected ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                }
            }
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
            mGridInBorder.Children.Add(mDieRollListBox);
        }

        private void HideDieRolls()
        {
            mGridInBorder.Children.Remove(mDieRollListBox);
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

        private void UpdateDirectionLine()
        {
            System.Windows.Point edgeCenter = GetEdgeCenter(Direction);
            mDirectionLine.X2 = edgeCenter.X;
            mDirectionLine.Y2 = edgeCenter.Y;
        }

        private Button CreateDirectionButton(HexDirection hexDirection)
        {
            double size = 20;

            Button button = new Button();
            button.Width = size;
            button.Height = size;
            button.Click += (s, e) => Direction = hexDirection;
            //button.Visibility = System.Windows.Visibility.Hidden;

            System.Windows.Point position = GetEdgeCenter(hexDirection);
            Canvas.SetLeft(button, position.X - size / 2);
            Canvas.SetTop(button, position.Y - size / 2);
            
            mCanvas.Children.Add(button);
            return button;
        }

        private System.Windows.Point GetEdgeCenter(HexDirection direction)
        {
            HexTileCorner corner1;
            HexTileCorner corner2;
            switch (direction)
            {
                case HexDirection.Left:
                    corner1 = HexTileCorner.LowerLeft;
                    corner2 = HexTileCorner.UperLeft;
                    break;
                case HexDirection.LowerLeft:
                    corner1 = HexTileCorner.LowerLeft;
                    corner2 = HexTileCorner.Down;
                    break;
                case HexDirection.LowerRight:
                    corner1 = HexTileCorner.Down;
                    corner2 = HexTileCorner.LowerRight;
                    break;
                case HexDirection.Right:
                    corner1 = HexTileCorner.LowerRight;
                    corner2 = HexTileCorner.UperRight;
                    break;
                case HexDirection.UperRight:
                    corner1 = HexTileCorner.Top;
                    corner2 = HexTileCorner.UperRight;
                    break;
                case HexDirection.UperLeft:
                default:
                    corner1 = HexTileCorner.UperLeft;
                    corner2 = HexTileCorner.Top;
                    break;
            }

            HexTerrainTile tile = mTerrain[mTileIndex];
            System.Windows.Point corner1Position = tile.GetCornerPositionWpf(corner1);
            System.Windows.Point corner2Position = tile.GetCornerPositionWpf(corner2);

            return new System.Windows.Point((corner1Position.X + corner2Position.X) / 2, (corner1Position.Y + corner2Position.Y) / 2);
        }
    }

    public class Swordsman : BattleUnit
    {
        public Swordsman(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            1 //attack distance
            ,20 //movement distance
            ,3 //Hp
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
            2 //attack distance
            ,2//movement distance
            , 3 //Hp
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
            1 //attack distance
            ,4 //movement distance
            , 3 //Hp
            , "Cavalry.png" //image
            , player, terrain, tileIndex)
        { }

        protected override Die GetDieAgainst(BattleUnit target)
        {
            return Die.D10;
        }

        protected override void OnMove(int turnNum, Point oldIndex, Point tileIndex)
        {
            //if (1 < HexHelper.GetDistance(oldIndex, tileIndex))
            //{
            //    bool hasEnemySpearmanSibling = false;
            //    bool hasEnemySibling = false;
            //    Terrain.ForEachSibling(tileIndex, tile =>
            //        {
            //            if (!hasEnemySpearmanSibling && null != tile.Unit && tile.Unit.Player != Player)
            //            {
            //                if (tile.Unit is Spearman)
            //                {
            //                    hasEnemySpearmanSibling = true;
            //                }
            //                hasEnemySibling = true;
            //            }
            //        });

            //    if (!hasEnemySpearmanSibling && hasEnemySibling)
            //    {
            //        AddSpellEffect(turnNum, new FirstStrikeBattleUnitSpellEffect(1));
            //    }
            //}
        }
    }

    public class Spearman : BattleUnit
    {
        public Spearman(BattlePlayer player, HexTerrain terrain, Point tileIndex)
            : base(
            1 //attack distance
            ,2 //movement distance
            , 3 //Hp
            , "Spearman.png" //image
            , player, terrain, tileIndex)
        { }

        protected override Die GetDieAgainst(BattleUnit target)
        {
            return (target is Cavalry) ? Die.D10 : Die.D6;
        }
    }
}
