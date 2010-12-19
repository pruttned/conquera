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
