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

namespace Conquera
{
    public class SpellDescriptor
    {
        public GraphicElement Picture { get; set; }
        public GraphicElement Icon { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public class Spell
    {
        public event EventHandler TotalCountChanged;
        public event EventHandler AvailableCountChanged;

        private int mTotalCount;
        private int mAvailableCount;

        public SpellDescriptor SpellDescriptor { get; private set; }        

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

                    if (value < AvailableCount)
                    {
                        AvailableCount = value;
                    }

                    mTotalCount = value;
                    EventHelper.RaiseEvent(TotalCountChanged, this);
                }
            }
        }

        public int AvailableCount
        {
            get { return mAvailableCount; }
            set
            {
                if (value != mAvailableCount)
                {
                    if (value < 0 || value > TotalCount)
                    {
                        throw new ArgumentException("Value must be greater or equal to zero and lesser or equal than TotalCount.");
                    }
                    
                    mAvailableCount = value;
                    EventHelper.RaiseEvent(AvailableCountChanged, this);
                }
            }
        }

        public Spell(SpellDescriptor spellDescriptor)
        {
            SpellDescriptor = spellDescriptor;
        }
    }

    public class SpellCollection : ReadOnlyCollection<Spell>
    {
        public SpellCollection()
            : base(new Spell[10])
        {
            //todo: Fill Items with new Spell instances; bind to events of each spells and raise common events here
        }

        public void SetSpellAvailabilitiesToMax()
        {
            foreach(Spell spell in Items)
            {
                spell.AvailableCount = spell.TotalCount;
            }
        }
    }

    public static class EventHelper
    {
        public static void RaiseEvent(EventHandler handler, object sender)
        {
            RaiseEvent(handler, sender, EventArgs.Empty);
        }

        public static void RaiseEvent(EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
