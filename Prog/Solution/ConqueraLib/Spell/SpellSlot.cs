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
    [DataObject(MaxCachedCnt = 0)]
    public class SpellSlot :BaseDataObject
    {
        public static List<string> SpellNames = new List<string>();
        private static Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();

        public Spell Spell { get; private set; }
        [DataProperty(NotNull = true)]
        public bool Enabled { get; private set; }
        
        [DataProperty(Column="Spell", NotNull = true)]
        private string SpellName { get; set; }

        static SpellSlot()
        {
            RegisterSpell(new SlayerSpell());
            RegisterSpell(new SpikesSpell());
            RegisterSpell(new FireStormSpell());
            RegisterSpell(new VampiricTouchSpell());
            RegisterSpell(new PackReinforcementSpell());
            RegisterSpell(new MindControlSpell());
            RegisterSpell(new PlagueSpell());
            RegisterSpell(new BloodMadnessSpell());
            RegisterSpell(new LastSacrificeSpell());
        }

        public SpellSlot(string spellName)
        {
            SpellName = spellName;
            Spell = Spells[SpellName];
            Enabled = true;
        }

        protected override void AfterLoadImpl(OrmManager ormManager)
        {
            base.AfterLoadImpl(ormManager);
            Spell = Spells[SpellName];
        }

        private SpellSlot()
        {
        }

        private static void RegisterSpell(Spell spell)
        {
            SpellNames.Add(spell.Name);
            Spells.Add(spell.Name, spell);
        }
    }
}
