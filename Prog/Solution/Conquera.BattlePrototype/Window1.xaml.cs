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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Xml.Linq;

namespace Conquera.BattlePrototype
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        static readonly string mMapsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrototypeMaps");

        HexTerrain mTerrain = new HexTerrain(20, 20);
        BattlePlayer[] mPlayers = new BattlePlayer[]{
            new BattlePlayer(Colors.Blue, 0),
            new BattlePlayer(Colors.Red, 1)};        
        private BattlePlayer mActivePlayer;

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
                    EventHelper.RaisePropertyChanged(PropertyChanged, this, "ActivePlayer");
                }
            }
        }

        public BattleUnit SelectedUnit {get; private set; }

        int mTurnNum = 0;

        public Window1()
        {
            mPlayers[0].CardDeck = SpellCardDecks.FullDeck;
            mPlayers[1].CardDeck = SpellCardDecks.FullDeck;
            mPlayers[0].FillCardsInHand();
            mPlayers[1].FillCardsInHand();
            mPlayers[0].Mana = 5;
            mPlayers[1].Mana = 5;



            Resources.Add("This", this);
            InitializeComponent();

            if (!System.IO.Directory.Exists(mMapsDir))
            {
                System.IO.Directory.CreateDirectory(mMapsDir);
            }
            LoadMap("test");
                
            ActivePlayer = mPlayers[0];
            
            //LoadTerrain();
            mTerrain.SetTile(new Microsoft.Xna.Framework.Point(1, 2), "Outpost");
            mTerrain.SetTile(new Microsoft.Xna.Framework.Point(0, 2), "Outpost");

            mPlayersListBox.ItemsSource = mPlayers;
            mPlayersListBox.SelectedIndex = 0;
            mSetTilesListBox.ItemsSource = HexTerrainTileFactory.TemplateNames;

            new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 2));
            new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 3));
            new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 5));
            new SkeletonLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(3, 2));
            new SkeletonLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(3, 3));
            new SkeletonLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(3, 5));
            new ZombieLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(4, 5));

            UpdateMapsListBox();
        }

        private void UpdateMapsListBox()
        {
            List<string> mapNames = new List<string>();            
            foreach(string mapName in System.IO.Directory.GetFiles(mMapsDir, "*.map"))
            {
                mapNames.Add(System.IO.Path.GetFileNameWithoutExtension(mapName));
            }
            mMapsListBox.ItemsSource = mapNames;
        }

        private void EndTurn()
        {
            //Resolve battle
            List<BattleUnit> unitsToKill = new List<BattleUnit>();
            foreach (var player in mPlayers)
            {

                //if (player != ActivePlayer)
                {
                    foreach (var unit in player.Units)
                    {
                        if (unit.ComputeDamageFromEnemies() >= unit.Defense)
                        {
                            int damage = Math.Max(0, unit.ComputeDamageFromEnemies() - unit.Defense );
                            unit.Hp -= damage;
                            if (unit.Hp <= 0)
                            {
                                unitsToKill.Add(unit);
                            }
                        }
                    }
                }
            }
            //kill units
            foreach (var unit in unitsToKill)
            {
                unit.Kill();
            }

            mTurnNum++;
            ActivePlayer = mPlayers[mTurnNum % 2];

            //Update defenses
            foreach (var player in mPlayers)
            {
                foreach (var unit in player.Units)
                {
                    unit.UpdateDefenseFromAlies();
                    unit.UpdateGraphics();
                }
            }

            ActivePlayer.OnTurnStart(mTurnNum);
            SelectUnit(null);


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

        private void SetMoveIndicatorsVisibility(bool isVisible)
        {
            if (!isVisible || !SelectedUnit.HasMovedThisTurn)
            {
                List<Microsoft.Xna.Framework.Point> indices = new List<Microsoft.Xna.Framework.Point>();
                SelectedUnit.GetPossibleMoves(indices);

                foreach (Microsoft.Xna.Framework.Point index in indices)
                {
                    mTerrain[index].IsMoveIndicatorVisible = isVisible;
                }
            }
        }

        private void LoadTerrain()
        {
            //Canvas size
            Microsoft.Xna.Framework.Vector2 lastTilePos = HexHelper.Get2DPosFromIndex(new Microsoft.Xna.Framework.Point(mTerrain.Width -1 , mTerrain.Height - 1));
            mMainCanvas.Width = lastTilePos.X + HexHelper.TileW;
            if (mTerrain.Height > 1 && 0 != mTerrain.Height % 2)
            {
                mMainCanvas.Width += HexHelper.HalfTileW;
            }
            mMainCanvas.Height = lastTilePos.Y + 2*HexHelper.TileR;

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

        private void mMainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                HexTerrainTile tile = GetParent<HexTerrainTile>(e.Source as DependencyObject);
                if (tile != null)
                {
                    if (mTabControl.SelectedItem == mEditorTabItem) //EDITOR
                    {
                        //Set tile
                        string tileName = (string)mSetTilesListBox.SelectedItem;
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
                    else if (mTabControl.SelectedItem == mGameTabItem) //GAME
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
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
                        else if (e.RightButton == MouseButtonState.Pressed)
                        {
                            //Move unit
                            if (tile.IsMoveIndicatorVisible)
                            {
                                SetMoveIndicatorsVisibility(false);
                                SelectedUnit.Move(tile.Index);
                            }
                        }
                    }
                }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EndTurn();
        }

        private void mTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectUnit(null);
            SetStartPosIndicatorsVisibility(mTabControl.SelectedItem == mEditorTabItem);
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

        private void mSetTilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void mResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMap();
        }

        private void ResetMap()
        {
            mPlayers[0].CardDeck = SpellCardDecks.FullDeck;
            mPlayers[1].CardDeck = SpellCardDecks.FullDeck;
            mPlayers[0].FillCardsInHand();
            mPlayers[1].FillCardsInHand();
            mPlayers[0].Mana = 5;
            mPlayers[1].Mana = 5;

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
}
