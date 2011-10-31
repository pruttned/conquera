using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera.BattlePrototype
{
    public class BattlePlayer
    {
        public Color Color { get; private set; }
        public int Index { get; private set; }

        public BattlePlayer(Color color, int index)
        {
            Color = color;
            Index = index;
        }
    }
}
