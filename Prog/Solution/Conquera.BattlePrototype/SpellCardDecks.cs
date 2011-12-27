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

namespace Conquera.BattlePrototype
{
    static class SpellCardDecks
    {
        public static List<SpellCard> FullDeck = new List<SpellCard>()
        {
            new SummonSkeletonLv1UnitSpellCard(),
            new SummonZombieLv1UnitSpellCard(),
            new SummonBansheeLv1UnitSpellCard(),
            new SummonSpectreLv1UnitSpellCard(),
            
            new SummonSkeletonLv2UnitSpellCard(),
            new SummonZombieLv2UnitSpellCard(),
            new SummonBansheeLv2UnitSpellCard(),
            new SummonSpectreLv2UnitSpellCard(),


            new AddDefenseSpellCard(3,1,2),
            new AddDefenseSpellCard(4,1,3),
            new AddDefenseSpellCard(3,2,1),

            new AddAttackSpellCard(3,1,2),
            new AddAttackSpellCard(4,1,3),
            new AddAttackSpellCard(3,2,1),

            new AddMovementDistanceSpellCard(3,1,2),
            new AddMovementDistanceSpellCard(4,1,4),
            new AddMovementDistanceSpellCard(3,2,1)


            //new Add2ManaCard(),
            //new Add5ManaCard(),
            //new Add7ManaCard(),
            //new Add10ManaCard(),
            //new Add15ManaCard()
        };

        //public static List<SpellCard> FullDeck = new List<SpellCard>()
        //{
        //    new AddDefenseSpellCard(1,1,2)
        //};
    }
}
