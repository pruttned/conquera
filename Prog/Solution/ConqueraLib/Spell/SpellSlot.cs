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
using System.Collections.ObjectModel;
using Ale.Gui;
using Ale;
using System.Collections.Generic;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using SimpleOrmFramework;

namespace Conquera
{
    public class SpellSlot :BaseDataObject
    {
        public event EventHandler TotalCountChanged;
        public event EventHandler AvailableCountChanged;

        private int mTotalCount;

        public Spell Spell { get; private set; }

        public int TotalCount
        {
            get { return mTotalCount; }
            set
            {
                if (value != mTotalCount)
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Value must be greater or equal to zero.");
                    }

                    mTotalCount = value;
                    EventHelper.RaiseEvent(TotalCountChanged, this);

                    if (mTotalCount < UsedCount)
                    {
                        UsedCount = mTotalCount;
                    }

                    EventHelper.RaiseEvent(AvailableCountChanged, this);
                }
            }
        }

        public int AvailableCount
        {
            get { return mTotalCount - UsedCount; }
        }

        /// <summary>
        /// Only for save/load purposes - used in SpellSlotCollection
        /// </summary>
        internal int UsedCount { get; set; }

        public void OnCast()
        {
            if (0 == AvailableCount)
            {
                throw new InvalidOperationException("AvailableCount < 0");
            }
            UsedCount++;
            EventHelper.RaiseEvent(AvailableCountChanged, this);
        }

        public SpellSlot(Spell spell)
        {
            Spell = spell;
        }

        public void ResetAvailableCount()
        {
            UsedCount = 0;
            EventHelper.RaiseEvent(AvailableCountChanged, this);
        }

    }
}
