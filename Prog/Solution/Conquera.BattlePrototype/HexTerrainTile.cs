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
using Microsoft.Xna.Framework;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Conquera.BattlePrototype
{
    public abstract class HexTerrainTile : Grid
    {
        private BattleUnit mUnit = null;
        private Border mMoveIndicator;
        private bool mIsMoveIndicatorVisible = false;

        public Point Index { get; private set; }
        
        public System.Windows.Point TopLeftPos { get; private set; }

        public abstract bool IsPassable { get; }

        public BattleUnit Unit
        {
            get { return mUnit; }
            set
            {
                if (mUnit != value)
                {
                    if (mUnit != null) //old
                    {
                        Children.Remove(mUnit);
                    }

                    mUnit = value;

                    if (mUnit != null) //new
                    {
                        if (null != mUnit.Parent)
                        {
                            ((HexTerrainTile)mUnit.Parent).Children.Remove(mUnit);
                        }
                        Children.Add(mUnit);
                    }
                }
            }
        }

        public virtual string ImageName
        {
            get { return null; }
        }
        
        public bool IsPassableAndEmpty
        {
            get { return IsPassable && null == Unit; }
        }

        public virtual Brush Fill        
        {
            get { return Brushes.Green; }
        }

        public bool IsMoveIndicatorVisible
        {
            get { return mIsMoveIndicatorVisible; }
            set 
            {
                if (value != mIsMoveIndicatorVisible)
                {
                    mIsMoveIndicatorVisible = value;

                    if (mIsMoveIndicatorVisible)
                    {
                        Children.Add(mMoveIndicator);
                    }
                    else
                    {
                        Children.Remove(mMoveIndicator);
                    }
                }
            }
        }

        public HexTerrainTile(Point index)
        {
            Index = index;
            Vector2 topLeftPosAsVector = HexHelper.Get2DPosFromIndex(index);
            TopLeftPos = new System.Windows.Point(topLeftPosAsVector.X, topLeftPosAsVector.Y);

            Polygon polygon = new Polygon();
            polygon.Stroke = Brushes.Black;
            polygon.Fill = Fill;
            polygon.StrokeThickness = 1.0;

            polygon.Points.Add(GetCornerPosition(HexTileCorner.Top));
            polygon.Points.Add(GetCornerPosition(HexTileCorner.UperRight));
            polygon.Points.Add(GetCornerPosition(HexTileCorner.LowerRight));
            polygon.Points.Add(GetCornerPosition(HexTileCorner.Down));
            polygon.Points.Add(GetCornerPosition(HexTileCorner.LowerLeft));
            polygon.Points.Add(GetCornerPosition(HexTileCorner.UperLeft));
            
            Children.Add(polygon);

            if (ImageName != null)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(string.Format("Images/{0}", ImageName), UriKind.Relative));
                image.Width = 60;
                image.Height = 60;
                Children.Add(image);
            }

            mMoveIndicator = new Border()
            {
                Width = 16,
                Height = 16,
                Background = Brushes.Yellow
            };
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

        private System.Windows.Point GetCornerPosition(HexTileCorner corner)
        {
            double centerX = HexHelper.HalfTileW;
            double centerY = HexHelper.TileR;

            Vector2 cornerBasePosition = HexHelper.GetHexTileCornerPos2D(corner);
            return new System.Windows.Point(centerX + cornerBasePosition.X, centerY + cornerBasePosition.Y);
        }
    }
    
    [HexTerrainTile("Gap")]
    public class GapHexTerrainTile : HexTerrainTile
    {
        public override bool IsPassable
        {
            get { return false; }
        }

        public override Brush Fill
        {
            get { return Brushes.DarkGray; }
        }

        public GapHexTerrainTile(Point index)
            : base(index)
        {
        }
    }

    [HexTerrainTile("Land")]
    public class LandHexTerrainTile : HexTerrainTile
    {
        public override bool IsPassable
        {
            get { return true; }
        }

        public LandHexTerrainTile(Point index)
            : base(index)
        {
        }
    }

    public abstract class CapturableHexTerrainTile : HexTerrainTile
    {
        BattlePlayer mOwningPlayer = null;
        
        public BattlePlayer OwningPlayer
        {
            get { return mOwningPlayer; }
            set 
            {
                if (value != mOwningPlayer)
                {
                    if (null != mOwningPlayer)
                    {
                        OnLost(mOwningPlayer);
                    }
                    mOwningPlayer = value;
                    if (null != mOwningPlayer)
                    {
                        OnCaptured(mOwningPlayer);
                    }
                }
            }
        }

        public override bool IsPassable
        {
            get { return true; }
        }
 
        public CapturableHexTerrainTile(Point index)
            : base(index)
        {
        }

        public virtual void OnCaptured(BattlePlayer newPlayer)
        {
        }

        public virtual void OnLost(BattlePlayer oldPlayer)
        {
        }

        public virtual void OnBeginTurn()
        {
        }
    }

    [HexTerrainTile("Outpost")]
    public class OutpostHexTerrainTile : CapturableHexTerrainTile
    {
        public override string ImageName
        {
            get { return "Outpost.bmp"; }
        }

        public OutpostHexTerrainTile(Point index)
            : base(index)
        {
        }
    }
}
