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
    [DataObject(MaxCachedCnt=0)]
    public class SpellSlotCollection : BaseDataObject, IEnumerable<SpellSlot>
    {
        private List<SpellSlot> mSpellSlots = null;

        public SpellSlot this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
                foreach (var spell in this)
                {
                    if (string.Equals(name, spell.Spell.Name))
                    {
                        return spell;
                    }
                }
                throw new KeyNotFoundException(string.Format("Spell with name '{0}' doesn't exists", name));
            }
        }

        public SpellSlot this[int i]
        {
            get
            {
                return SpellSlots[i];
            }
        }

        public int Count
        {
            get { return SpellSlots.Count; }
        }

        [DataListProperty]
        private List<SpellSlot> SpellSlots 
        {
            get
            {
                if (null == mSpellSlots)
                {
                    mSpellSlots = new List<SpellSlot>();
                    foreach (var spellName in SpellSlot.SpellNames)
                    {
                        mSpellSlots.Add(new SpellSlot(spellName));
                    }
                }
                return mSpellSlots;
            }
            set
            {
                mSpellSlots = value;
            }
        }

        public IEnumerator<SpellSlot> GetEnumerator()
        {
            return ((IEnumerable<SpellSlot>)SpellSlots).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return SpellSlots.GetEnumerator();
        }


    }
}
