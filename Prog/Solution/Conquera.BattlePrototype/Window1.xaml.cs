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

namespace Conquera.BattlePrototype
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                if (mActivePlayer != value)
                {
                    mActivePlayer = value;
                    EventHelper.RaisePropertyChanged(PropertyChanged, this, "ActivePlayer");
                }
            }
        }

        public BattleUnit SelectedUnit {get; private set; }

        int mTurnNum = 0;

        public Window1()
        {
            ActivePlayer = mPlayers[0];
            Resources.Add("This", this);

            InitializeComponent();
            
            LoadTerrain();
            mTerrain.SetTile(new Microsoft.Xna.Framework.Point(1, 2), "Outpost");

            mPlayersListBox.ItemsSource = mPlayers;
            mPlayersListBox.SelectedIndex = 0;
            mSetTilesListBox.ItemsSource = HexTerrainTileFactory.TemplateNames;

            BattleUnit unit1 = new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 2));
            BattleUnit unit2 = new SkeletonLv1BattleUnit(mPlayers[0], mTerrain, new Microsoft.Xna.Framework.Point(1, 3));
            BattleUnit unit3 = new SkeletonLv1BattleUnit(mPlayers[1], mTerrain, new Microsoft.Xna.Framework.Point(1, 4));

            //unit1.Kill();
            //mTerrain = new HexTerrain("aaa.xml", new BattlePlayer[]{
            //    new BattlePlayer(Microsoft.Xna.Framework.Graphics.Color.Blue, 0),
            //    new BattlePlayer(Microsoft.Xna.Framework.Graphics.Color.Red, 1)});
        }

        private void EndTurn()
        {
            mTurnNum++;
            ActivePlayer = mPlayers[mTurnNum % 2];
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

            //Tile set event
            mTerrain.TileSet += new EventHandler<ValueChangeEventArgs<HexTerrainTile>>(mTerrain_TileSet);
        }

        private void mTerrain_TileSet(object sender, ValueChangeEventArgs<HexTerrainTile> e)
        {
            RemoveTile(e.OldValue);
            AddTile(e.NewValue);
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
                    }
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
