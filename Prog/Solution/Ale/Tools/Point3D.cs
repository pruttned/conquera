//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using System.Text;

namespace Ale.Tools
{
    struct Point3D : IEquatable<Point3D>
    {
        int X;
        int Y;
        int Z;

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Z;
        }
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }
        public override bool Equals(object obj)
        {
            return Equals((Point3D)obj);
        }

        public bool Equals(Point3D otherPoint)
        {
            return ((X == otherPoint.X) && (Y == otherPoint.Y) && (Z == otherPoint.Z));
        }
    }
}
