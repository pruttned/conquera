using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquera.BattlePrototype
{
    public static class MathExt
    {
        public static Random Random = new Random();

        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(max, Math.Max(min, value));
        }
    }
}
