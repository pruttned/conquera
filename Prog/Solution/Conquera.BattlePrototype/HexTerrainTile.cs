using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Conquera.BattlePrototype
{
    public abstract class HexTerrainTile
    {
        public Point Index { get; private set; }

        public Vector2 CenterPos { get; private set; }

        public abstract bool IsPassable { get; }

        public abstract System.Windows.Controls.Image Image { get; }

        public HexTerrainTile(Point index)
        {
            Index = index;
            CenterPos = HexHelper.Get2DPosFromIndex(index);
        }

        public bool IsSiblingTo(HexTerrainTile tile)
        {
            if (null == tile) throw new ArgumentNullException("tile");

            int i = Index.X;
            int j = Index.Y;

            int i2 = tile.Index.X;
            int j2 = tile.Index.Y;

            return (i2 == i - 1 && j2 == j) ||
                    (i2 == i + 1 && j2 == j) ||
                    ((0 != (j & 1)) && ((i2 == i && j2 == j - 1) || (i2 == i + 1 && j2 == j - 1) || (i2 == i && j2 == j + 1) || (i2 == i + 1 && j2 == j + 1))) ||
                    ((0 == (j & 1)) && ((i2 == i - 1 && j2 == j - 1) || (i2 == i && j2 == j - 1) || (i2 == i - 1 && j2 == j + 1) || (i2 == i && j2 == j + 1)));
        }
    }

    public class GapHexTerrainTile : HexTerrainTile
    {
        public override bool IsPassable
        {
            get { return false; }
        }

        public GapHexTerrainTile(Point index)
            : base(index)
        {
            //todo - load image
            //Image = ...;
        }

        public override System.Windows.Controls.Image Image
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class LandHexTerrainTile : HexTerrainTile
    {
        public override bool IsPassable
        {
            get { return true; }
        }

        public LandHexTerrainTile(Point index)
            : base(index)
        {
            //todo - load image
            //Image = ...;
        }

        public override System.Windows.Controls.Image Image
        {
            get { throw new NotImplementedException(); }
        }
    }

    public abstract class CapturableHexTerrainTile : HexTerrainTile
    {
        public override bool IsPassable
        {
            get { return true; }
        }
 
        public CapturableHexTerrainTile(Point index)
            : base(index)
        {
        }

        public virtual void OnCaptured()
        {
        }

        public virtual void OnLost(BattlePlayer newPlayer)
        {
        }

        public virtual void OnBeginTurn()
        {
        }
    }


}
