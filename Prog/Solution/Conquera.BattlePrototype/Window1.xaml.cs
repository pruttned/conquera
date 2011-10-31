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

            mTerrain.SetTile(new Microsoft.Xna.Framework.Point(1,2), "Outpost");
            //mTerrain = new HexTerrain("aaa.xml", new BattlePlayer[]{
            //    new BattlePlayer(Microsoft.Xna.Framework.Graphics.Color.Blue, 0),
            //    new BattlePlayer(Microsoft.Xna.Framework.Graphics.Color.Red, 1)});
        }

        private void LoadTerrain()
        {
            mMainCanvas.Width = HexHelper.TileW * mTerrain.Width;
            mMainCanvas.Height = HexHelper.TileH * mTerrain.Height;

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
