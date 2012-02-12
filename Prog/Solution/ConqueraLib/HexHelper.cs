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

namespace Conquera
{
    public static class HexHelper
    {
        /// <summary>
        /// Corners around 0,0
        /// </summary>
        private static Vector3[] Corners;

        /// <summary>
        /// Corners around 0,0 with unit R
        /// </summary>
        private static Vector3[] UnitCorners;

        public static float TileR = 1f;
        private static float HalfTileR = 0.5f * TileR;
        private static float HalfTileW = (float)Math.Cos(MathHelper.ToRadians(30)) * TileR;
        private static float TileW = 2 * HalfTileW;
        private static float TileH = 1.5f * TileR;

        static HexHelper()
        {
            UnitCorners = new Vector3[6];
            Vector3 baseVec = new Vector3(0, 1, 0);
            for (int i = 0; i < 6; ++i)
            {
                Quaternion rotQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -MathHelper.ToRadians(i * 60));
                Vector3.Transform(ref baseVec, ref rotQuat, out UnitCorners[i]);
            }
            Corners = new Vector3[6];
            for (int i = 0; i < 6; ++i)
            {
                Corners[i] = TileR * UnitCorners[i];
            }
        }

        /// <summary>
        /// Gets the distance (in cells) between two hex cells
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetDistance(Point a, Point b)
        {
            //based on  http://www-cs-students.stanford.edu/~amitp/Articles/HexLOS.html
            if (a.Y == b.Y)
            {
                return Math.Abs(Math.Abs(a.X) - Math.Abs(b.X));
            }
            else
            {
                Point aa = new Point(a.X - Floor2(a.Y), a.X + Ceil2(a.Y));
                Point bb = new Point(b.X - Floor2(b.Y), b.Y = b.X + Ceil2(b.Y));

                int dx = bb.X - aa.X;
                int dy = bb.Y - aa.Y;
                if ((dx >= 0 && dy >= 0) || (dx < 0 && dy < 0))
                {
                    return Math.Max(Math.Abs(dx), Math.Abs(dy));
                }
                else
                {
                    return Math.Abs(dx) + Math.Abs(dy);
                }
            }
        }

        /// <summary>
        /// Gets the center of the cell given by its index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="groundHeight"></param>
        /// <returns></returns>
        public static Vector3 Get3DPosFromIndex(Point index, float groundHeight)
        {
            Vector3 pos;
            Get3DPosFromIndex(index, groundHeight, out pos);
            return pos;
        }

        /// <summary>
        /// Gets the center of the tile given by its index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="groundHeight"></param>
        /// <param name="pos"></param>
        public static void Get3DPosFromIndex(Point index, float groundHeight, out Vector3 pos)
        {
            //http://www.gamedev.net/reference/articles/article747.asp
            //PlotX=MapX*Width+(MapY AND 1)*(Width/2)
            //HeightOverLapping=)*0.75
            pos = new Vector3(
                index.X * TileW + (index.Y & 1) * HalfTileW,
                index.Y * TileH, groundHeight);
        }

        /// <summary>
        /// Gets the center of the tile given by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector2 Get2DPosFromIndex(Point index)
        {
            Vector2 pos;
            Get2DPosFromIndex(index, out pos);
            return pos;
        }
        
        /// <summary>
        /// Gets the center of the tile given by its index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pos"></param>
        public static void Get2DPosFromIndex(Point index, out Vector2 pos)
        {
            //http://www.gamedev.net/reference/articles/article747.asp
            //PlotX=MapX*Width+(MapY AND 1)*(Width/2)
            //HeightOverLapping=)*0.75
            pos = new Vector2(
                index.X * TileW + (index.Y & 1) * HalfTileW,
                index.Y * TileH);
        }

        /// <summary>
        /// Gets the tile index from a 2D position on the hex terrain
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="terrainWidth"></param>
        /// <param name="terrainHeight"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetIndexFromPos(Vector2 pos, int terrainWidth, int terrainHeight, out Point index)
        {
            index = new Point();

            pos.X += HalfTileW;
            pos.Y += HalfTileR;

            if (pos.X < 0 || pos.Y < -HalfTileR)
            {
                return false;
            }

            //must be floor to handle (-1,0)
            int j = (int)Math.Floor(pos.Y / TileH);
            int i = (int)Math.Floor((pos.X - (j & 1) * HalfTileW) / TileW);

            float inRecX = pos.X - (i * TileW + (j & 1) * HalfTileW);
            float inRecY = pos.Y - (j * TileH);


            if (inRecY < TileR)
            {
                index.X = i;
                index.Y = j;
            }
            else
            {
                if (inRecX < HalfTileW)
                {
                    float my = TileR + (TileH - TileR) * (inRecX / HalfTileW);
                    if (inRecY < my)
                    {
                        index.X = i;
                        index.Y = j;
                    }
                    else
                    {
                        if (0 == (j & 1))
                        {
                            index.X = i - 1;
                        }
                        else
                        {
                            index.X = i;
                        }
                        index.Y = j + 1;
                    }
                }
                else
                {
                    float my = TileH + (TileR - TileH) * ((inRecX / HalfTileW) - 1);
                    if (inRecY < my)
                    {
                        index.X = i;
                        index.Y = j;
                    }
                    else
                    {
                        if (0 == (j & 1))
                        {
                            index.X = i;
                        }
                        else
                        {
                            index.X = i + 1;
                        }
                        index.Y = j + 1;
                    }
                }
            }

            return (index.X >= 0 && index.X < terrainWidth && index.Y >= 0 && index.Y < terrainHeight);
        }

        /// <summary>
        /// Gets the positions of hex tile corners around 0,0
        /// </summary>
        /// <returns></returns>
        public static Vector3[] GetHexTileCorners()
        {
            Vector3[] corners = new Vector3[6];
            Corners.CopyTo(corners, 0);
            return corners;
        }

        /// <summary>
        /// Gets the position of a given hex tile corner around 0,0
        /// </summary>
        public static void GetHexTileCornerPos3D(HexTileCorner corner, out Vector3 pos)
        {
            pos = Corners[(int)corner];
        }

        /// <summary>
        /// Gets the position of a given hex tile corner around 0,0
        /// </summary>
        public static Vector3 GetHexTileCornerPos3D(HexTileCorner corner)
        {
            return Corners[(int)corner];
        }
        
        /// <summary>
        /// Gets the positions of hex tile corners around 0,0 with R = 1
        /// </summary>
        /// <returns></returns>
        public static Vector3[] GetUnitHexTileCorners()
        {
            Vector3[] corners = new Vector3[6];
            UnitCorners.CopyTo(corners, 0);
            return corners;
        }

        /// <summary>
        /// Gets the position of a given hex tile corner around 0,0 with R = 1
        /// </summary>
        public static void GetUnitHexTileCornerPos3D(HexTileCorner corner, out Vector3 pos)
        {
            pos = UnitCorners[(int)corner];
        }

        /// <summary>
        /// Gets the position of a given hex tile corner around 0,0 with R = 1
        /// </summary>
        public static Vector3 GetUnitHexTileCornerPos3D(HexTileCorner corner)
        {
            return UnitCorners[(int)corner];
        }

        /// <summary>
        /// Gets the index of a hex tile's sibling
        /// </summary>
        /// <param name="index"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Point GetSibling(Point index, HexDirection direction)
        {
            int i = index.X;
            int j = index.Y;

            switch (direction)
            {
                case HexDirection.UperRight:
                    return (0 != (j & 1)) ? new Point(i + 1, j + 1) : new Point(i, j + 1);
                case HexDirection.Right:
                    return new Point(i + 1, j);
                case HexDirection.LowerRight:
                    return (0 != (j & 1)) ? new Point(i + 1, j - 1) : new Point(i, j - 1);
                case HexDirection.LowerLeft:
                    return (0 != (j & 1)) ? new Point(i, j - 1) : new Point(i - 1, j - 1);
                case HexDirection.Left:
                    return new Point(i - 1, j);
                case HexDirection.UperLeft:
                    return (0 != (j & 1)) ? new Point(i, j + 1) : new Point(i - 1, j + 1);
                default:
                    throw new ArgumentException("Invalid direction");
            }
        }

        /// <summary>
        /// Gets the direction to a sibling hex tile
        /// </summary>
        /// <param name="src"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static HexDirection GetDirectionToSibling(Point src, Point desc)
        {
            if (desc.Y == src.Y + 1) //UperRight, UperLeft
            {
                if (0 != (src.Y & 1))
                {
                    if (desc.X == src.X + 1) return HexDirection.UperRight;
                    if (desc.X == src.X) return HexDirection.UperLeft;
                }
                else
                {
                    if (desc.X == src.X) return HexDirection.UperRight;
                    if (desc.X == src.X - 1) return HexDirection.UperLeft;
                }
            }
            if (desc.Y == src.Y) //Right, Left
            {
                if (desc.X == src.X + 1) return HexDirection.Right;
                if (desc.X == src.X - 1) return HexDirection.Left;
            }
            if (desc.Y == src.Y - 1) //LowerRight, LowerLeft
            {
                if (0 != (src.Y & 1))
                {
                    if (desc.X == src.X + 1) return HexDirection.LowerRight;
                    if (desc.X == src.X) return HexDirection.LowerLeft;
                }
                else
                {
                    if (desc.X == src.X) return HexDirection.LowerRight;
                    if (desc.X == src.X - 1) return HexDirection.LowerLeft;
                }
            }

            throw new ArgumentException("Hex tiles are not siblings");
        }

        /// <summary>
        /// Rotats a given direction
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static HexDirection RotateDirection(HexDirection dir, int rot)
        {
            int resDir = (((int)dir + rot) % 6);
            return (0 <= resDir) ? (HexDirection)resDir : (HexDirection)(6 + resDir);
        }

        private static int Floor2(int X)
        {
            //from http://www-cs-students.stanford.edu/~amitp/Articles/HexLOS.html            
            return ((X >= 0) ? (X >> 1) : (X - 1) / 2);
        }
        private static int Ceil2(int X)
        {
            //from http://www-cs-students.stanford.edu/~amitp/Articles/HexLOS.html
            return ((X >= 0) ? ((X + 1) >> 1) : (X / 2));
        }
    }


    public enum HexDirection
    {
        UperRight = 0,
        Right = 1,
        LowerRight = 2,
        LowerLeft = 3,
        Left = 4,
        UperLeft = 5
    }

    public enum HexTileCorner
    {
        Top = 0,
        UperRight = 1,
        LowerRight = 2,
        Down = 3,
        LowerLeft = 4,
        UperLeft = 5
    }
}
