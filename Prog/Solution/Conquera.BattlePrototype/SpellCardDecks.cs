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


            
            new IncDecDefenseSpellCard(3,1,2),
            new IncDecDefenseSpellCard(4,1,3),
            new IncDecDefenseSpellCard(3,2,1),

            new IncDecAttackSpellCard(3,1,2),
            new IncDecAttackSpellCard(4,1,3),
            new IncDecAttackSpellCard(3,2,1),

            new IncDecMovementDistanceSpellCard(3,1,2),
            new IncDecMovementDistanceSpellCard(4,1,4),
            new IncDecMovementDistanceSpellCard(3,2,1),

            new IncDecDefenseSpellCard(3,1,-2),
            new IncDecDefenseSpellCard(4,1,-3),
            new IncDecDefenseSpellCard(3,2,-1),

            new IncDecAttackSpellCard(3,1,-2),
            new IncDecAttackSpellCard(4,1,-3),
            new IncDecAttackSpellCard(3,2,-1),

            new IncDecMovementDistanceSpellCard(3,1,-2),
            new IncDecMovementDistanceSpellCard(4,1,-4),
            new IncDecMovementDistanceSpellCard(3,2,-1),


            new AddManaSpellCard(10),
            new AddManaSpellCard(15),

            new DiscardCardsSpellCard(4, 1),
            new DiscardCardsSpellCard(6, 2),
            new DiscardCardsSpellCard(8, 3),

            new DisableMovementSpellCard(6, 2),
            new DisableMovementSpellCard(9, 4),

            new DisableAttackSpellCard(5, 1),
            new DisableAttackSpellCard(7, 2),

            new HealSpellCard(5,1),
            new HealSpellCard(8,2),
            new HealSpellCard(13,3),

            new DamageSpellCard(9, 1),
            new DamageSpellCard(14, 2),

            new RemoveDisableMovementsSpellCard(),
            new RemoveDisableAttacksSpellCard()
        };

        //public static List<SpellCard> FullDeck = new List<SpellCard>()
        //{
        //    new AddDefenseSpellCard(1,1,2)
        //};
    }
}
