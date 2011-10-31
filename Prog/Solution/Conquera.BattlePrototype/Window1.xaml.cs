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

namespace Conquera.BattlePrototype
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        HexTerrain mTerrain = new HexTerrain(20, 20);

        public Window1()
        {
            InitializeComponent();
            LoadTerrain();

         //   mTerrain.SetTile(new Microsoft.Xna.Framework.Point(1,2), "Outpost");
            //mTerrain = new HexTerrain("aaa.xml", new BattlePlayer[]{
            //    new BattlePlayer(Microsoft.Xna.Framework.Graphics.Color.Blue, 0),
            //    new BattlePlayer(Microsoft.Xna.Framework.Graphics.Color.Red, 1)});
        }

        private void LoadTerrain()
        {
            Microsoft.Xna.Framework.Vector2 lastTilePos = HexHelper.Get2DPosFromIndex(new Microsoft.Xna.Framework.Point(mTerrain.Width -1 , mTerrain.Height - 1));
            mMainCanvas.Width = lastTilePos.X + HexHelper.TileW;
            if (mTerrain.Height > 1 && 0 != mTerrain.Height % 2)
            {
                mMainCanvas.Width += HexHelper.HalfTileW;
            }
            mMainCanvas.Height = lastTilePos.Y + 2*HexHelper.TileR;

            for (int i = 0; i < mTerrain.Width; i++)
            {
                for (int j = 0; j < mTerrain.Height; j++)
                {
                    HexTerrainTile tile = mTerrain[i,j];
                    Canvas.SetLeft(tile, tile.TopLeftPos.X);
                    Canvas.SetTop(tile, tile.TopLeftPos.Y);

                    mMainCanvas.Children.Add(tile);
                }
            }
        }
    }
}
