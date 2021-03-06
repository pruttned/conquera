﻿//////////////////////////////////////////////////////////////////////
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
        protected Polygon mHexPolygon;
        private BattleUnit mUnit = null;
        private Border mMoveIndicator;
        private bool mIsMoveIndicatorVisible = false;
        private Line mStartPosIndicator;
        private Polygon mCanCasCardIndicator;
        private Border mHighlightIndicator;
        private Polygon mOverlayPolygon;
        private TextBlock mOverlayTextBlock;
        private byte mOverlayBackgroundAlpha = 255;

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

                        OnUnitEnter(mUnit);
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
            get { return IsPassable && IsEmpty; }
        }

        public bool IsEmpty
        {
            get { return null == Unit; }
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

        public bool IsHighlighted
        {
            get { return mHighlightIndicator.IsVisible; }
            set { mHighlightIndicator.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; }
        }

        /// <summary>
        /// Alpha is ignored (Use OverlayBackgroundAlpha) 
        /// </summary>
        public Color OverlayBackground
        {
            get { return ((SolidColorBrush)mOverlayPolygon.Fill).Color; }
            set 
            {
                value.A = OverlayBackgroundAlpha;
                ((SolidColorBrush)mOverlayPolygon.Fill).Color = value; 
            }
        }

        public byte OverlayBackgroundAlpha
        {
            get { return mOverlayBackgroundAlpha; }
            set
            {
                if (value != mOverlayBackgroundAlpha)
                {
                    mOverlayBackgroundAlpha = value;
                    SolidColorBrush brush = (SolidColorBrush)mOverlayPolygon.Fill;
                    var color = brush.Color;
                    color.A = mOverlayBackgroundAlpha;
                    brush.Color = color;
                }
            }
        }

        public string OverlayText
        {
            get { return mOverlayTextBlock.Text; }
            set { mOverlayTextBlock.Text = value; }
        }

        public bool IsOverlayVisible
        {
            get { return mOverlayPolygon.IsVisible; }
            set
            {
                System.Windows.Visibility visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                mOverlayPolygon.Visibility = visibility;
                mOverlayTextBlock.Visibility = visibility;
            }
        }

        public HexTerrainTile(Point index, int terrainHeight)
        {
            Index = index;
            Vector2 topLeftPosAsVector = HexHelper.Get2DPosFromIndex(index);
            topLeftPosAsVector.Y = ((terrainHeight-1) * HexHelper.TileH) - topLeftPosAsVector.Y;
            TopLeftPos = new System.Windows.Point(topLeftPosAsVector.X, topLeftPosAsVector.Y);

            mHexPolygon = new Polygon();
            mHexPolygon.Stroke = Brushes.Black;
            mHexPolygon.Fill = Fill;
            mHexPolygon.StrokeThickness = 1.0;

            mHexPolygon.Points.Add(GetCornerPosition(HexTileCorner.Top));
            mHexPolygon.Points.Add(GetCornerPosition(HexTileCorner.UperRight));
            mHexPolygon.Points.Add(GetCornerPosition(HexTileCorner.LowerRight));
            mHexPolygon.Points.Add(GetCornerPosition(HexTileCorner.Down));
            mHexPolygon.Points.Add(GetCornerPosition(HexTileCorner.LowerLeft));
            mHexPolygon.Points.Add(GetCornerPosition(HexTileCorner.UperLeft));

            Children.Add(mHexPolygon);

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
            
            System.Windows.Point upperLeftCornerPosition = GetCornerPosition(HexTileCorner.UperLeft);
            System.Windows.Point lowerRightCornerPosition = GetCornerPosition(HexTileCorner.LowerRight);
            mStartPosIndicator = new Line()            
            {
                Stroke = Brushes.Transparent,
                StrokeThickness = 10,
                X1 = upperLeftCornerPosition.X,
                Y1 = upperLeftCornerPosition.Y,
                X2 = lowerRightCornerPosition.X,
                Y2 = lowerRightCornerPosition.Y
            };
            Children.Add(mStartPosIndicator);

            mCanCasCardIndicator = new Polygon();
            mCanCasCardIndicator.Points.Add(GetCornerPosition(HexTileCorner.Top));
            mCanCasCardIndicator.Points.Add(GetCornerPosition(HexTileCorner.UperRight));
            mCanCasCardIndicator.Points.Add(GetCornerPosition(HexTileCorner.LowerRight));
            mCanCasCardIndicator.Points.Add(GetCornerPosition(HexTileCorner.Down));
            mCanCasCardIndicator.Points.Add(GetCornerPosition(HexTileCorner.LowerLeft));
            mCanCasCardIndicator.Points.Add(GetCornerPosition(HexTileCorner.UperLeft));
            mCanCasCardIndicator.Visibility = System.Windows.Visibility.Hidden;
            mCanCasCardIndicator.Opacity = 0.8;
            mCanCasCardIndicator.StrokeThickness = 1.0;
            Children.Add(mCanCasCardIndicator);

            mHighlightIndicator = new Border()
            {
                Width = 32,
                Height = 32,
                Background = Brushes.Blue,
                Visibility = System.Windows.Visibility.Hidden
            };
            Grid.SetZIndex(mHighlightIndicator, 99);
            Children.Add(mHighlightIndicator);

            mOverlayPolygon = new Polygon();
            mOverlayPolygon.Stroke = Brushes.Black;
            mOverlayPolygon.StrokeThickness = 1.0;
            mOverlayPolygon.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Grid.SetZIndex(mOverlayPolygon, 1000);
            mOverlayPolygon.IsHitTestVisible = false;
            mOverlayPolygon.Points.Add(GetCornerPosition(HexTileCorner.Top));
            mOverlayPolygon.Points.Add(GetCornerPosition(HexTileCorner.UperRight));
            mOverlayPolygon.Points.Add(GetCornerPosition(HexTileCorner.LowerRight));
            mOverlayPolygon.Points.Add(GetCornerPosition(HexTileCorner.Down));
            mOverlayPolygon.Points.Add(GetCornerPosition(HexTileCorner.LowerLeft));
            mOverlayPolygon.Points.Add(GetCornerPosition(HexTileCorner.UperLeft));
            Children.Add(mOverlayPolygon);

            mOverlayTextBlock = new TextBlock();
            mOverlayTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            mOverlayTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            mOverlayTextBlock.Background = Brushes.White;            
            Grid.SetZIndex(mOverlayTextBlock, 1001);
            mOverlayTextBlock.IsHitTestVisible = false;
            Children.Add(mOverlayTextBlock);

            IsOverlayVisible = false;
        }

        public void ShowCanCastCardIndicator(bool canCast)
        {
            mCanCasCardIndicator.Fill = canCast ? Brushes.Blue : Brushes.Red;
            mCanCasCardIndicator.Stroke = mCanCasCardIndicator.Fill;
            mCanCasCardIndicator.Visibility = System.Windows.Visibility.Visible;
        }

        public void HideCanCastCardIndicator()
        {
            mCanCasCardIndicator.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetStartPosIndicator(BattlePlayer player)
        {
            if (player == null)
            {
                mStartPosIndicator.Stroke = Brushes.Transparent;
            }
            else
            {
                mStartPosIndicator.Stroke = new SolidColorBrush(player.Color);
            }
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

        public virtual void OnTurnStart(int turnNum, BattlePlayer activePlayer)
        {}

        protected virtual void OnUnitEnter(BattleUnit unit)
        {}

        public System.Windows.Point GetCornerPosition(HexTileCorner corner)
        {
            double centerX = HexHelper.HalfTileW;
            double centerY = HexHelper.TileR;

            Vector2 cornerBasePosition = HexHelper.GetHexTileCornerPos2D(corner);
            return new System.Windows.Point(centerX + cornerBasePosition.X, centerY + cornerBasePosition.Y);
        }

        public System.Windows.Point GetCornerPositionWpf(HexTileCorner corner)
        {
            double centerX = HexHelper.HalfTileW;
            double centerY = HexHelper.TileR;

            Vector2 cornerBasePosition = HexHelper.GetHexTileCornerPos2D(corner);
            Vector2 cornerBasePositionWpf = new Vector2(cornerBasePosition.X, -cornerBasePosition.Y);
            return new System.Windows.Point(centerX + cornerBasePositionWpf.X, centerY + cornerBasePositionWpf.Y);
        }

        public System.Windows.Point GetEdgeCenter(HexDirection direction, System.Windows.Vector offset)
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

            System.Windows.Point corner1Position = GetCornerPositionWpf(corner1);
            System.Windows.Point corner2Position = GetCornerPositionWpf(corner2);

            System.Windows.Point center = new System.Windows.Point((corner1Position.X + corner2Position.X) / 2, (corner1Position.Y + corner2Position.Y) / 2);
            return offset + center;
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
            get { return new SolidColorBrush(Color.FromArgb(255, 40, 40, 40)); }
        }

        public GapHexTerrainTile(Point index, int terrainHeight)
            : base(index, terrainHeight)
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

        public LandHexTerrainTile(Point index, int terrainHeight)
            : base(index, terrainHeight)
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

                    if (mOwningPlayer != null)
                    {
                        mHexPolygon.StrokeThickness = 5.0;
                        mHexPolygon.Stroke = new SolidColorBrush(mOwningPlayer.Color);
                    }
                    else
                    {
                        mHexPolygon.StrokeThickness = 1.0;
                        mHexPolygon.Stroke = Brushes.Black;
                    }
                }
            }
        }

        public override bool IsPassable
        {
            get { return true; }
        }

        public CapturableHexTerrainTile(Point index, int terrainHeight)
            : base(index, terrainHeight)
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

        protected override void OnUnitEnter(BattleUnit unit)
        {
            OwningPlayer = unit.Player;
        }
    }

    [HexTerrainTile("Outpost")]
    public class OutpostHexTerrainTile : CapturableHexTerrainTile
    {
        public override string ImageName
        {
            get { return "Outpost.bmp"; }
        }

        public override Brush Fill
        {
            get { return Brushes.Brown; }
        }

        public OutpostHexTerrainTile(Point index, int terrainHeight)
            : base(index, terrainHeight)
        {
        }
    }

    [HexTerrainTile("ManaShrine")]
    public class ManaShrineHexTerrainTile : CapturableHexTerrainTile
    {
        public override Brush Fill
        {
            get { return Brushes.Plum; }
        }

        public ManaShrineHexTerrainTile(Point index, int terrainHeight)
            : base(index, terrainHeight)
        {
        }

        public override void OnTurnStart(int turnNum, BattlePlayer activePlayer)
        {
            if (activePlayer == OwningPlayer)
            {
                activePlayer.Mana += 1;
            }
        }
        public override void OnCaptured(BattlePlayer newPlayer)
        {
            newPlayer.Mana += 1;
        }
    }
}
