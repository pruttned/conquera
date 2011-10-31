using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Conquera.BattlePrototype
{
    public class BattleUnit
    {
        public delegate void CellIndexChangedHandler(BattleUnit obj, Point oldValue);
        public event CellIndexChangedHandler TileIndexChanged;

        private Point mTileIndex;

        public Vector2 Position { get; private set; }

        public Point TileIndex
        {
            get { return mTileIndex; }
            set
            {
                if (mTileIndex != value)
                {
                    throw new NotImplementedException();
                    //Point oldValue = mTileIndex;
                    //mTileIndex = value;

                    //UpdatePositionFromIndex();

                    //OnCellIndexChanged(oldValue);

                    //if (null != CellIndexChanged)
                    //{
                    //    CellIndexChanged.Invoke(this, oldValue);
                    //}
                }
            }
        }
    }
}
