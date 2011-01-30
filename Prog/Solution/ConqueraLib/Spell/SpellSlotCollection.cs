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
    [DataObject(MaxCachedCnt=0)]
    public class SpellSlotCollection : BaseDataObject, IEnumerable<SpellSlot>
    {
        public static Spell[] Spells;

        private SpellSlot[] mSpellSlots;

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
                return mSpellSlots[i];
            }
        }

        /// <summary>
        /// Only for load/save purposes
        /// </summary>
        [DataListProperty]
        private List<int> SpellUsageCnts { get; set; }

        static SpellSlotCollection()
        {
            Spells = new Spell[] 
            { 
                new SlayerSpell(),
                new SpikesSpell(),
                new FireStormSpell(),
                new VampiricTouchSpell(),
                new PackReinforcementSpell(),
                new MindControlSpell(),
                new PlagueSpell(),
                new BloodMadnessSpell(),
                new LastSacrificeSpell()
            };
        }

        public SpellSlotCollection()
        {
            mSpellSlots = CreateSpellSlotList();
        }

        public void ResetSpellAvailabilities()
        {
            foreach (var spell in mSpellSlots)
            {
                spell.ResetAvailableCount();
            }
        }

        public IEnumerator<SpellSlot> GetEnumerator()
        {
            return ((IEnumerable<SpellSlot>)mSpellSlots).GetEnumerator();
        }

        public static Spell GetSpell(string name)
        {
            foreach (Spell spell in Spells)
            {
                if(string.Equals(spell.Name, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return spell;
                }
            }
            throw new KeyNotFoundException(string.Format("Spell with name '{0}' doesn't exists", name));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mSpellSlots.GetEnumerator();
        }

        private static SpellSlot[] CreateSpellSlotList()
        {
            SpellSlot[] slots = new SpellSlot[Spells.Length];
            for (int i = 0; i < slots.Length; ++i)
            {
                slots[i] = new SpellSlot(Spells[i]);
            }

            return slots;
        }

        protected override void BeforeSaveImpl(OrmManager ormManager)
        {
            if (null == SpellUsageCnts)
            {
                SpellUsageCnts = new List<int>(new int[mSpellSlots.Length]);
            }
            for(int i =0; i< mSpellSlots.Length; ++i)
            {
                SpellUsageCnts[i] = mSpellSlots[i].UsedCount;
            }
            base.BeforeSaveImpl(ormManager);
        }

        protected override void AfterLoadImpl(OrmManager ormManager)
        {
            for (int i = 0; i < mSpellSlots.Length; ++i)
            {
                mSpellSlots[i].UsedCount = SpellUsageCnts[i];
            }
            
            base.AfterLoadImpl(ormManager);
        }
    }
}
