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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Shapes;
using System.Reflection;

namespace Conquera.BattlePrototype
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, INotifyPropertyChanged
    {
        private readonly string mPlayerTextBlockStrTemplate = "M:{0}/{1}; Hand:{2} Deck:{3}";

        int mInitMana = 0;
        int mMaxMana = 99;
        HexTerrainTile mMovementTargetTile;
        Line mMoveDirectionIndicator;
        HexDirection mMoveDirection;

        //Type[] mUnits1 = new Type[] 
        //{
        //    typeof(SoldierAtt),
        //    typeof(SoldierAtt),
        //    typeof(SoldierBl),
        //    typeof(SoldierBl),
        //    typeof(SoldierDef),
        //    typeof(SoldierDef),
        //};
        //Type[] mUnits2 = new Type[] 
        //{
        //    typeof(ScoutBase),
        //    typeof(ScoutFly),
        //    typeof(ScoutFs),
        //    typeof(SupportAtt),
        //    typeof(SupportDef),
        //    typeof(SupportHeal),
        //};
        Type[] mUnits1 = new Type[] 
        {
            typeof(Swordsman),
            typeof(Swordsman),
            typeof(Swordsman),
            typeof(Cavalry),
            typeof(Archer),
            typeof(Archer),
        };
        Type[] mUnits2 = new Type[] 
        {
            typeof(Swordsman),
            typeof(Spearman),
            typeof(Spearman),
            typeof(Swordsman),
            typeof(Archer),
            typeof(Cavalry),
        };
        private class SpellCardListBoxItem
        {
            public SpellCard Card { get; private set; }

            public SpellCardListBoxItem(SpellCard card)
            {
                Card = card;
            }

            public override string ToString()
            {
                return Card.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        static readonly string mMapsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrototypeMaps");

        HexTerrain mTerrain = new HexTerrain(20, 20);
        BattlePlayer[] mPlayers;
        private BattlePlayer mActivePlayer;

        public HexTerrain Terrain
        {
            get { return mTerrain; }
        }

        public IEnumerable<BattlePlayer> Players
        {
            get { return mPlayers; }
        }
        
        public BattlePlayer ActivePlayer
        {
            get { return mActivePlayer; }
            private set
            {
                if (null == value) throw new ArgumentNullException("ActivePlayer");
                if (mActivePlayer != value)
                {
                    if (null != mActivePlayer)
                    {
                        mActivePlayer.IsActive = false;
                    }
                    mActivePlayer = value;
                    mActivePlayer.IsActive = true;
                    UpdateCardsListBox();
                    EventHelper.RaisePropertyChanged(PropertyChanged, this, "ActivePlayer");
                }
            }
        }

        public BattleUnit SelectedUnit { get; private set; }

        int mTurnNum = 0;

        private enum EndTurnButtonActions { ResolveBattlePhase, KillAndEndTurn };
        private EndTurnButtonActions mNextEndTurnButtonAction = EndTurnButtonActions.ResolveBattlePhase;
        private List<UnitDamages> mUnitDamages;

        public Window1()
        {
            mPlayers = new BattlePlayer[]{
                new BattlePlayer(this, Colors.Blue, 0),
                //new AiBattlePlayer(Colors.Blue, 0,this),
                //new BattlePlayer(this, Colors.Red, 1)};
                new AiBattlePlayer(this, Colors.Red, 1)};


            InitializeComponent();

            if (!System.IO.Directory.Exists(mMapsDir))
            {
                System.IO.Directory.CreateDirectory(mMapsDir);
            }

            mPlayers[0].ManaChanged += new EventHandler(Player_ManaChanged);
            mPlayers[1].ManaChanged += new EventHandler(Player_ManaChanged);

            mPlayers[0].CardsInHand.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(mPlayerCardsInHand_CollectionChanged);
            mPlayers[1].CardsInHand.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(mPlayerCardsInHand_CollectionChanged);


            ActivePlayer = mPlayers[0];
            LoadMap("test");

            ////LoadTerrain();
            //mTerrain.SetTile(new Microsoft.Xna.Framework.Point(1, 2), "Outpost");
            //mTerrain.SetTile(new Microsoft.Xna.Framework.Point(0, 2), "Outpost");

            mPlayersListBox.ItemsSource = mPlayers;
            mPlayersListBox.SelectedIndex = 0;
            mSetTilesListBox.ItemsSource = HexTerrainTileFactory.TemplateNames;

            //new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 2));
            //new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 3));
            //new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 5));
            //new SkeletonLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(3, 2));
            //new SkeletonLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(3, 3));
            //new SkeletonLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(3, 5));
            //new ZombieLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(4, 5));

            UpdateMapsListBox();

            mSetTilesLeftButtonTextBlock.Text = "Land";
            mSetTilesRightButtonTextBlock.Text = "Gap";

            UpdatePlayerInfoTextBlocks();


            Logger.Logged += new EventHandler<Logger.LogEventArgs>(Logger_Logged);

            foreach (var player in mPlayers)
            {
                player.OnTurnStart(mTurnNum, (ActivePlayer == player));
            }

            mMoveDirectionIndicator = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 5,
                Visibility = Visibility.Hidden
            };
            mMainCanvas.Children.Add(mMoveDirectionIndicator);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (mNextEndTurnButtonAction)
            {
                case EndTurnButtonActions.ResolveBattlePhase:
                    mUnitDamages = ResolveBattle();
                    VisualizeUnitDamages(mUnitDamages);
                    mNextEndTurnButtonAction = EndTurnButtonActions.KillAndEndTurn;
                    mEndTurnButton.Content = "Kill & End Turn";
                    break;

                case EndTurnButtonActions.KillAndEndTurn:
                    ResolveDamages(mUnitDamages);
                    VisualizeUnitDamages(null);
                    NextTurn();
                    mNextEndTurnButtonAction = EndTurnButtonActions.ResolveBattlePhase;
                    mEndTurnButton.Content = "Resolve Pre-Battle";
                    break;
            }
        }

        public void NextTurn()
        {
            mTurnNum++;
            ActivePlayer = mPlayers[mTurnNum % 2];


            SelectUnit(null);
            mCardsListBox.IsEnabled = true;

            foreach (var player in mPlayers)
            {
                player.Mana = 0;
                player.OnTurnStart(mTurnNum, (ActivePlayer == player));
            }

            UpdateAiMapInfo();

            NotifyTilesTurnStart();
        }

        private void VisualizeUnitDamages(List<UnitDamages> unitDamages)
        {
            foreach (BattlePlayer player in mPlayers)
            {
                foreach (BattleUnit unit in player.Units)
                {
                    unit.Damages = null;
                }
            }

            if (unitDamages != null)
            {
                foreach (UnitDamages damage in unitDamages)
                {
                    damage.Target.Damages = damage;
                }
            }
        }






        private void Logger_Logged(object sender, Logger.LogEventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = e.Record;
            mLogBox.Items.Add(item);
            mLogBox.ScrollIntoView(item);
            e.Item = item;
        }

        private void ResetPlayers()
        {
            foreach (var player in mPlayers)
            {
                player.CardDeck = new SpellCardDeck(BaseSpellCardDecks.Deck1);
                player.MaxMana = mMaxMana;
                player.Mana = mInitMana;
                player.Units.Clear();
                //                new HeroBattleUnit(player, mTerrain, player.StartPos);

                bool swap = player.StartPos.X > mTerrain.Width*0.5;

                for (int i = 0; i < mUnits1.Length; ++i)
                {
                    Activator.CreateInstance(mUnits1[i], player, mTerrain, mTerrain.GetSibling(player.StartPos, swap ? (HexDirection)5 - i : (HexDirection)i).Index);
                }
                var p2 = player.StartPos;
                p2.Y += 3;
                for (int i = 0; i < mUnits2.Length; ++i)
                {
                    Activator.CreateInstance(mUnits2[i], player, mTerrain, mTerrain.GetSibling(p2, swap ? (HexDirection)5 - i : (HexDirection)i).Index);
                }
                player.Mana = 0;
            }

            NotifyTilesTurnStart();
        }

        private void mPlayerCardsInHand_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCardsListBox();
            UpdatePlayerInfoTextBlocks();
        }

        private void UpdateCardsListBox()
        {
            mCardsListBox.Items.Clear();
            foreach (SpellCard card in ActivePlayer.CardsInHand)
            {
                mCardsListBox.Items.Add(new SpellCardListBoxItem(card));
            }
        }

        private void Player_ManaChanged(object sender, EventArgs e)
        {
            UpdatePlayerInfoTextBlocks();
        }

        private void UpdatePlayerInfoTextBlocks()
        {
            mBlueTextBlock.Text = string.Format(mPlayerTextBlockStrTemplate, mPlayers[0].Mana, mPlayers[0].MaxMana, mPlayers[0].CardsInHand.Count, (null != mPlayers[0].CardDeck ? mPlayers[0].CardDeck.Count : 0));
            mRedTextBlock.Text = string.Format(mPlayerTextBlockStrTemplate, mPlayers[1].Mana, mPlayers[1].MaxMana, mPlayers[1].CardsInHand.Count, (null != mPlayers[1].CardDeck ? mPlayers[1].CardDeck.Count : 0));
        }

        private void UpdateMapsListBox()
        {
            List<string> mapNames = new List<string>();
            foreach (string mapName in System.IO.Directory.GetFiles(mMapsDir, "*.map"))
            {
                mapNames.Add(System.IO.Path.GetFileNameWithoutExtension(mapName));
            }
            mMapsListBox.ItemsSource = mapNames;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<UnitDamages> ResolveBattle()
        {
            List<UnitDamages> unitDamages = new List<UnitDamages>();

            foreach (var player in mPlayers)
            {
                foreach (var targetUnit in player.Units)
                {
                    UnitDamages unitDamage = null;
                    foreach (var player2 in (from p in mPlayers where p != player select p))
                    {
                        foreach (var attackingUnit in player2.Units)
                        {
                            var rolls = attackingUnit.RollDiceAgainst(targetUnit);
                            if (null != rolls)
                            {
                                if (null == unitDamage)
                                {
                                    unitDamage = new UnitDamages(targetUnit);
                                }
                                unitDamage.Attacks.Add(new UnitAttack(attackingUnit, rolls));
                            }
                        }
                    }
                    if (null != unitDamage)
                    {
                        unitDamages.Add(unitDamage);
                    }
                }
            }
            return unitDamages;
        }

        public void ResolveDamages(List<UnitDamages> damages)
        {
            foreach (var damage in damages)
            {
                damage.Target.Damage += damage.GetHitCnt();
            }

            foreach (var player in mPlayers)
            {
                foreach (var unit in player.Units)
                {
                    unit.Damage -= unit.DamagePreventerCnt;

                    int hpLost = unit.Damage / 2;
                    unit.Hp -= hpLost;
                    if (unit.Hp == 0)
                    {
                        unit.Kill();
                    }
                    else if (unit.Damage >= 1)
                    {
                        //unit.AddSpellEffect(mTurnNum, new DisableMovementBattleUnitSpellEffect(2));
                        unit.AddSpellEffect(mTurnNum, new ConstIncMovementDistanceBattleUnitSpellEffect(-unit.BaseMovementDistance, 2)); 

                    }

                    unit.Damage = 0;
                }
            }
        }

        public void SetMoveIndicatorsVisibility(bool isVisible)
        {
            if (!isVisible || (!SelectedUnit.HasMovedThisTurn && SelectedUnit.HasEnabledMovement))
            {
                List<Microsoft.Xna.Framework.Point> indices = new List<Microsoft.Xna.Framework.Point>();
                SelectedUnit.GetPossibleMoves(OccupationIgnoreMode.None, indices);
                indices.Add(SelectedUnit.TileIndex);

                foreach (Microsoft.Xna.Framework.Point index in indices)
                {
                    mTerrain[index].IsMoveIndicatorVisible = isVisible;
                }
            }
        }

        private void NotifyTilesTurnStart()
        {
            for (int i = 0; i < mTerrain.Width; i++)
            {
                for (int j = 0; j < mTerrain.Width; j++)
                {
                    mTerrain[i, j].OnTurnStart(mTurnNum, ActivePlayer);
                }
            }
        }

        private void SelectUnit(BattleUnit unit)
        {
            if (unit != SelectedUnit)
            {
                if (!CanSelectUnit(unit))
                {
                    throw new ArgumentException("Unit does not belong to the active player.");
                }

                if (SelectedUnit != null)
                {
                    SelectedUnit.IsSelected = false;
                    SetMoveIndicatorsVisibility(false);
                }

                SelectedUnit = unit;

                if (SelectedUnit != null)
                {
                    SelectedUnit.IsSelected = true;
                    SetMoveIndicatorsVisibility(true);
                }
            }
        }

        private bool CanSelectUnit(BattleUnit unit)
        {
            return unit == null || unit.Player == ActivePlayer;
        }

        private void LoadTerrain()
        {
            //Canvas size
            Microsoft.Xna.Framework.Vector2 lastTilePos = HexHelper.Get2DPosFromIndex(new Microsoft.Xna.Framework.Point(mTerrain.Width - 1, mTerrain.Height - 1));
            mMainCanvas.Width = lastTilePos.X + HexHelper.TileW;
            if (mTerrain.Height > 1 && 0 != mTerrain.Height % 2)
            {
                mMainCanvas.Width += HexHelper.HalfTileW;
            }
            mMainCanvas.Height = lastTilePos.Y + 2 * HexHelper.TileR;

            //Adding tiles
            for (int i = 0; i < mTerrain.Width; i++)
            {
                for (int j = 0; j < mTerrain.Height; j++)
                {
                    AddTile(mTerrain[i, j]);
                }
            }

            //Starting pos
            SetStartPosIndicatorsVisibility(true);

            //Tile set event
            mTerrain.TileSet += new EventHandler<ValueChangeEventArgs<HexTerrainTile>>(mTerrain_TileSet);
        }

        private void UnloadTerrain()
        {
            if (mTerrain != null)
            {
                mMainCanvas.Children.Clear();
                mTerrain.TileSet -= mTerrain_TileSet;
                SelectUnit(null);
            }
        }

        private void mTerrain_TileSet(object sender, ValueChangeEventArgs<HexTerrainTile> e)
        {
            RemoveTile(e.OldValue);
            AddTile(e.NewValue);
        }

        private void SaveMap(string name)
        {
            XElement mapElm = new XElement("map");
            for (int i = 0; i < mPlayers.Length; ++i)
            {
                mapElm.Add(new XElement("player",
                    new XAttribute("index", i),
                    new XElement("startPos",
                        new XAttribute("x", mPlayers[i].StartPos.X),
                        new XAttribute("y", mPlayers[i].StartPos.Y))));
            }
            mTerrain.Save(mapElm);

            mapElm.Save(GetMapFileName(name));
        }


        private void LoadMap(string name)
        {
            UnloadTerrain();

            XElement mapElm = XElement.Load(GetMapFileName(name));

            foreach (var playerElm in mapElm.Elements("player"))
            {
                int index = (int)playerElm.Attribute("index");
                var startPosElm = playerElm.Element("startPos");
                mPlayers[index].StartPos = new Microsoft.Xna.Framework.Point((int)startPosElm.Attribute("x"), (int)startPosElm.Attribute("y"));
            }
            mTerrain = new HexTerrain(mapElm, mPlayers);
            mTurnNum = 0;

            LoadTerrain();

            ResetPlayers();
        }

        private static string GetMapFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            return System.IO.Path.Combine(mMapsDir, name + ".map");
        }

        private void AddTile(HexTerrainTile tile)
        {
            Canvas.SetLeft(tile, tile.TopLeftPos.X);
            Canvas.SetTop(tile, tile.TopLeftPos.Y);
            mMainCanvas.Children.Add(tile);
        }

        private void RemoveTile(HexTerrainTile tile)
        {
            mMainCanvas.Children.Remove(tile);
        }

        private void mMainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DoMouseAction(e, false);
        }

        private void mMainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            DoMouseAction(e, true);
        }

        private void DoMouseAction(MouseEventArgs e, bool isFromMouseMoveEvent)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                HexTerrainTile tile = GetParent<HexTerrainTile>(e.Source as DependencyObject);
                if (tile != null)
                {
                    if (mTabControl.SelectedItem == mEditorTabItem) //EDITOR
                    {
                        //Set tile
                        string tileName = e.LeftButton == MouseButtonState.Pressed ? mSetTilesLeftButtonTextBlock.Text : mSetTilesRightButtonTextBlock.Text;
                        if ((bool)mSetTilesOptionBox.IsChecked && tileName != null)
                        {
                            mTerrain.SetTile(tile.Index, tileName);

                            CapturableHexTerrainTile capturableTile = mTerrain[tile.Index] as CapturableHexTerrainTile;
                            if (capturableTile != null)
                            {
                                capturableTile.OwningPlayer = (BattlePlayer)mPlayersListBox.SelectedItem;
                            }
                        }

                        //Capture
                        if ((bool)mCaptureOptionBox.IsChecked)
                        {
                            CapturableHexTerrainTile capturableTile = tile as CapturableHexTerrainTile;
                            if (capturableTile != null)
                            {
                                capturableTile.OwningPlayer = (BattlePlayer)mPlayersListBox.SelectedItem;
                            }
                        }

                        //Start pos
                        if ((bool)mStartPosOptionBox.IsChecked)
                        {
                            BattlePlayer player = (BattlePlayer)mPlayersListBox.SelectedItem;
                            if (player != null)
                            {
                                mTerrain[player.StartPos].SetStartPosIndicator(null);
                                player.StartPos = tile.Index;
                            }
                        }
                        SetStartPosIndicatorsVisibility(true);
                    }
                    else if (mTabControl.SelectedItem == mGameTabItem && !(e.Source is Button)) //GAME
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            //Cast card
                            SpellCardListBoxItem cardListBoxItem = (SpellCardListBoxItem)mCardsListBox.SelectedItem;
                            if (cardListBoxItem != null)
                            {
                                SpellCard card = cardListBoxItem.Card;
                                if (card.Cost <= ActivePlayer.Mana && card.IsValidTarget(ActivePlayer, tile, mTerrain))
                                {
                                    ActivePlayer.CastSpellCard(mTurnNum, mCardsListBox.SelectedIndex, tile, mTerrain, mPlayers);
                                    mCardsListBox.UnselectAll();
                                    //mCardsListBox.IsEnabled = false;
                                }
                            }
                            else
                            {
                                //Select / deselect unit
                                BattleUnit unit = GetParent<BattleUnit>(e.Source as DependencyObject);
                                if (unit != null && CanSelectUnit(unit))
                                {
                                    SelectUnit(unit);
                                }
                                else if (unit == null)
                                {
                                    SelectUnit(null);
                                }
                            }
                        }
                        else if (e.RightButton == MouseButtonState.Pressed)
                        {
                            //Move unit                            
                            if (!isFromMouseMoveEvent && tile.IsMoveIndicatorVisible) //mouse down on a move indicator
                            {                                
                                SetMoveIndicatorsVisibility(false);
                                mMovementTargetTile = tile;

                                Point tileCenter = new Point(Canvas.GetLeft(tile) + tile.ActualWidth / 2.0, Canvas.GetTop(tile) + tile.ActualHeight / 2.0);
                                mMoveDirectionIndicator.X1 = tileCenter.X;
                                mMoveDirectionIndicator.Y1 = tileCenter.Y;
                                mMoveDirectionIndicator.Visibility = Visibility.Visible;
                            }
                            else if (isFromMouseMoveEvent && mMovementTargetTile != null)
                            {
                                Point movementTargetTileCenter = new Point(Canvas.GetLeft(mMovementTargetTile) + mMovementTargetTile.ActualWidth / 2.0, Canvas.GetTop(mMovementTargetTile) + mMovementTargetTile.ActualHeight / 2.0);
                                Vector vector = Mouse.GetPosition(mMainCanvas) - movementTargetTileCenter;
                                vector.Normalize();
                                double angle = -Math.Atan2(vector.Y, vector.X) + 1.570;
                                angle = Microsoft.Xna.Framework.MathHelper.ToDegrees((float)angle);                                
                                int edgeIndex = (int)((angle + 60) / 60);

                                mMoveDirection = HexDirection.Left;
                                if (edgeIndex == 0) mMoveDirection = HexDirection.LowerLeft;
                                if (edgeIndex == 1) mMoveDirection = HexDirection.LowerRight;
                                if (edgeIndex == 2) mMoveDirection = HexDirection.Right;
                                if (edgeIndex == 3) mMoveDirection = HexDirection.UperRight;
                                if (edgeIndex == 4) mMoveDirection = HexDirection.UperLeft;
                                if (edgeIndex == 5 || (angle < -60)) mMoveDirection = HexDirection.Left;

                                Point edgeCenter = mMovementTargetTile.GetEdgeCenter(mMoveDirection, new Vector(Canvas.GetLeft(mMovementTargetTile), Canvas.GetTop(mMovementTargetTile)));
                                mMoveDirectionIndicator.X2 = edgeCenter.X;
                                mMoveDirectionIndicator.Y2 = edgeCenter.Y;                                
                            }
                        }
                    }
                }
            }
        }

        private void mMainCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mMovementTargetTile != null)
            {
                SelectedUnit.Direction = mMoveDirection;
                SelectedUnit.Move(mTurnNum, mMovementTargetTile.Index);

                mMoveDirectionIndicator.Visibility = Visibility.Hidden;
                mMovementTargetTile = null;
            }
        }

        private void SetStartPosIndicatorsVisibility(bool visible)
        {
            if (visible)
            {
                mTerrain[mPlayers[0].StartPos].SetStartPosIndicator(mPlayers[0]);
                mTerrain[mPlayers[1].StartPos].SetStartPosIndicator(mPlayers[1]);
            }
            else
            {
                mTerrain[mPlayers[0].StartPos].SetStartPosIndicator(null);
                mTerrain[mPlayers[1].StartPos].SetStartPosIndicator(null);
            }
        }

        private T GetParent<T>(DependencyObject element) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
            {
                return null;
            }
            if (parent is T)
            {
                return parent as T;
            }
            return GetParent<T>(parent);
        }

        private void mTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectUnit(null);
            SetStartPosIndicatorsVisibility(mTabControl.SelectedItem == mEditorTabItem);
            mCardsListBox.SelectedIndex = -1;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(mSaveMapTextBox.Text))
            {
                MessageBox.Show("Enter map name");
            }
            else
            {
                SaveMap(mSaveMapTextBox.Text);
                UpdateMapsListBox();
            }
            //LoadMap("test");
        }

        private void mMapsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string mapName = (string)mMapsListBox.SelectedItem;
            if (!string.IsNullOrEmpty(mapName))
            {
                LoadMap(mapName);
            }
        }

        private void mSelectNonePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            mPlayersListBox.SelectedItem = null;
        }

        private void mResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMap();
        }

        private void ResetMap()
        {
            mPlayers[0].StartPos = new Microsoft.Xna.Framework.Point(0, 0);
            mPlayers[1].StartPos = new Microsoft.Xna.Framework.Point(1, 0);

            ActivePlayer = mPlayers[0];

            UnloadTerrain();
            mTerrain = new HexTerrain(20, 20);
            LoadTerrain();
        }

        private void mSetTilesListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mSetTilesOptionBox.IsChecked = true;
        }

        private void mSetTilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                mSetTilesRightButtonTextBlock.Text = (string)mSetTilesListBox.SelectedItem;
            }
            else
            {
                mSetTilesLeftButtonTextBlock.Text = (string)mSetTilesListBox.SelectedItem;
            }
        }

        private void mUnselectCardButton_Click(object sender, RoutedEventArgs e)
        {
            mCardsListBox.SelectedIndex = -1;
        }

        private void mCardsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCanCastCardIndicators();
            SelectUnit(null);
            e.Handled = true;
        }

        public void UpdateCanCastCardIndicators()
        {
            SpellCardListBoxItem cardItem = (SpellCardListBoxItem)mCardsListBox.SelectedItem;
            if (cardItem == null)
            {
                for (int i = 0; i < mTerrain.Width; i++)
                {
                    for (int j = 0; j < mTerrain.Height; j++)
                    {
                        mTerrain[i, j].HideCanCastCardIndicator();
                    }
                }
            }
            else
            {
                SpellCard card = cardItem.Card;

                for (int i = 0; i < mTerrain.Width; i++)
                {
                    for (int j = 0; j < mTerrain.Height; j++)
                    {
                        HexTerrainTile tile = mTerrain[i, j];
                        tile.ShowCanCastCardIndicator(card.IsValidTarget(ActivePlayer, tile, mTerrain));
                    }
                }
            }
        }

        private void CardItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                SpellCardListBoxItem cardItem = (SpellCardListBoxItem)((TextBlock)sender).DataContext;
                ActivePlayer.DiscardSpellCard(mTurnNum, mCardsListBox.Items.IndexOf(cardItem));
                //mCardsListBox.IsEnabled = false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mLogBox.SelectedItem = null;
        }


        private void mAiMapInfoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAiMapInfo();
        }

        private void InitAiMapInfoComboBox()
        {
            mAiMapInfoComboBox.Items.Add("None");
            Type iAiPlayerMapInformationDisplayType = typeof(IAiPlayerMapInformationDisplay);
            foreach(var type in (from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && !t.IsAbstract && iAiPlayerMapInformationDisplayType.IsAssignableFrom(t) select t))
            {
                mAiMapInfoComboBox.Items.Add(Activator.CreateInstance(type));
            }
            mAiMapInfoComboBox.SelectedIndex = 0;
        }

        private void UpdateAiMapInfo()
        {
            if (0 < mAiMapInfoComboBox.SelectedIndex && mActivePlayer is AiBattlePlayer)
            {
                ((IAiPlayerMapInformationDisplay)mAiMapInfoComboBox.SelectedItem).UpdateMapDisplay((AiBattlePlayer)mActivePlayer, Terrain);
                Terrain.SetOverlayVisibility(true);
            }
            else
            {
                Terrain.SetOverlayVisibility(false);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitAiMapInfoComboBox();
            mAiMapInfoAlphaSlider.Value = 230;
        }

        private void mAiMapInfoAlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < mTerrain.Width; i++)
            {
                for (int j = 0; j < mTerrain.Height; j++)
                {
                    mTerrain[i, j].OverlayBackgroundAlpha = (byte)mAiMapInfoAlphaSlider.Value;
                }
            }
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CanCastCardConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                int cost = (int)values[0];
                int playerMana = (int)values[1];
                return cost <= playerMana ? Brushes.Transparent : Brushes.Red;
            }
            catch //Some error in designer...
            {
                return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class UnitDamages
    {
        public BattleUnit Target {get; private set;}
        public List<UnitAttack> Attacks { get; private set; }

        public UnitDamages(BattleUnit target)
        {
            Target = target;
            Attacks = new List<UnitAttack>();
        }
    
        public int GetHitCnt()
        {
            return (from a in Attacks select (from r in a.AttackRolls where r.IsHit select 1).Sum()).Sum();
        }
    }
    public class UnitAttack
    {
        public BattleUnit Attacker { get; private set; }
        public IList<DieAttackRoll> AttackRolls { get; private set; }
        
        public UnitAttack(BattleUnit attacker, IList<DieAttackRoll> attackRolls)
        {
            Attacker = attacker;
            AttackRolls = attackRolls;
        }
    }


}
