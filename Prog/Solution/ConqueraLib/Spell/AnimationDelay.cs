﻿/////////////////////////////////////////////////////////////////////
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
using System.Collections.ObjectModel;
using Ale.Gui;
using Ale;
using System.Collections.Generic;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    public class AnimationDelay
    {
        private float mEndTime = -1;
        private float mDelay;

        public void Start(float delay)
        {
            mEndTime = -1;
            mDelay = delay;
        }

        public bool HasPassed(AleGameTime time)
        {
            if (0 > mEndTime)
            {
                mEndTime = time.TotalTime + mDelay;
            }
            if (time.TotalTime > mEndTime)
            {
                return true;
            }
            return false;
        }
    }
}
