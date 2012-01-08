using System;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using System.Windows;

namespace Conquera.BattlePrototype
{
    public static class Logger
    {
        internal class LogEventArgs : EventArgs
        {
            public object Record;
            public ListBoxItem Item;
        }

        public delegate void RecordsDoubleClickHandler(object record);

        internal static event EventHandler<LogEventArgs> Logged;

        public static void Log(object record, params HexTerrainTile[] tiles)
        {
            Log(record, null, tiles);
        }

        public static void Log(object record, RecordsDoubleClickHandler onDoubleClick, params HexTerrainTile[] tiles)
        {
            if (Logged != null)
            {
                LogEventArgs args = new LogEventArgs { Record = record };
                Logged(null, args);

                if (onDoubleClick != null)
                {
                    args.Item.MouseDoubleClick += (sender, e) => onDoubleClick(record);
                }

                if (tiles != null && tiles.Length != 0)
                {
                    args.Item.DataContext = tiles;
                    args.Item.Selected +=new System.Windows.RoutedEventHandler(Item_Selected);
                    args.Item.Unselected += new System.Windows.RoutedEventHandler(Item_Unselected);
                }
            }
        }

        private static void Item_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            HexTerrainTile[] tiles = (HexTerrainTile[])item.DataContext;

            foreach(HexTerrainTile tile in tiles)
            {
                tile.IsHighlighted = true;
            }
        }

        private static void Item_Unselected(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            HexTerrainTile[] tiles = (HexTerrainTile[])item.DataContext;

            foreach (HexTerrainTile tile in tiles)
            {
                tile.IsHighlighted = false;
            }
        }
    }
}
