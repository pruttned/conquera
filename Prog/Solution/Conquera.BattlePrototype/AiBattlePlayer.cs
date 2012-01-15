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
using System.Windows.Media;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Conquera.BattlePrototype
{
    public class AiBattlePlayer : BattlePlayer
    {
        Window1 mWindow;
        public AiBattlePlayer(Color color, int index, Window1 window)
            :base(color, index)
        {
            mWindow = window; 
        }
        protected override void OnTurnStartImpl(int turnNum, bool isActive)
        {
            if (isActive)
            {
                Microsoft.Xna.Framework.Point center = new Microsoft.Xna.Framework.Point(9, 6);

                var points = new List<Microsoft.Xna.Framework.Point>();
                foreach (var unit in Units)
                {
                    if (unit.HasEnabledMovement && !unit.HasMovedThisTurn)
                    {
                        points.Clear();
                        unit.GetPossibleMoves(points);
                        if (0 < points.Count)
                        {
                            points.Sort((a, b) => Comparer<int>.Default.Compare(Math.Abs(a.X - center.X) + Math.Abs(a.Y - center.Y), Math.Abs(b.X - center.X) + Math.Abs(b.Y - center.Y)));
                            int i = 0;
                            for (; i < points.Count && MathExt.Random.NextDouble() < 0.5; ++i) { }
                            if (points.Count <= i)
                            {
                                i = MathExt.Random.Next(points.Count);
                            }

                            unit.Move(points[i]);
                        }
                    }
                }

                mWindow.EndTurn();
            }
        }

    }
}
