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
using Microsoft.Xna.Framework;

namespace Conquera.BattlePrototype
{
    public class SpellCardDeck
    {
        List<SpellCard> mCards;

        public bool IsEmpty { get { return 0 == mCards.Count; } }
        public int Count { get { return mCards.Count; } }

        public SpellCardDeck(IList<SpellCard> cards)
        {
            mCards = new List<SpellCard>(cards);
            
            //http://en.wikipedia.org/wiki/Fisher-Yates_shuffle#The_modern_algorithm
            for (int i = 0; i < mCards.Count; i++)
            {
                int j = MathExt.Random.Next(i, mCards.Count);
                SpellCard card = mCards[j];
                mCards[j] = mCards[i];
                mCards[i] = card;
            }
        }

        public SpellCard PopCard()
        {
            if(IsEmpty)
            {
                return null;
            }
            SpellCard card = mCards[mCards.Count-1];
            mCards.RemoveAt(mCards.Count - 1);
            return card;
        }
    }
    static class BaseSpellCardDecks
    {
        public static List<SpellCard> Deck1 = new List<SpellCard>()
        {
            new IncDecAttackSpellCard(2,1,new Point(10,10)),
            new IncDecAttackSpellCard(2,1,new Point(10,10)),
            new IncDecAttackSpellCard(2,1,new Point(10,10)),
            new IncDecAttackSpellCard(2,1,new Point(10,10)),

            new IncDecDefenseSpellCard(2,1, 30),
            new IncDecDefenseSpellCard(2,1, 30),
            new IncDecDefenseSpellCard(2,1, 30),
            new IncDecDefenseSpellCard(2,1, 30),
   
            new DiscardCardsSpellCard(4, 1),
            new DiscardCardsSpellCard(4, 1),
            new DiscardCardsSpellCard(4, 1),
            new DiscardCardsSpellCard(4, 1),

            new DisableMovementSpellCard(4, 2),
            new DisableMovementSpellCard(4, 2),
            new DisableMovementSpellCard(4, 2),
            new DisableMovementSpellCard(4, 2),

            new DisableAttackSpellCard(5, 1),
            new DisableAttackSpellCard(5, 1),
            new DisableAttackSpellCard(5, 1),
            new DisableAttackSpellCard(5, 1),
        };



  //    new SummonSkeletonLv1UnitSpellCard(),
        //    new SummonZombieLv1UnitSpellCard(),
        //    new SummonBansheeLv1UnitSpellCard(),
            //new SummonSpectreLv1UnitSpellCard(),
            
          //  new SummonSkeletonLv2UnitSpellCard(),
            //new SummonZombieLv2UnitSpellCard(),
            //new SummonBansheeLv2UnitSpellCard(),
          //  new SummonSpectreLv2UnitSpellCard(),

          //  new IncDecDefenseSpellCard(3,1,2),
            //new IncDecDefenseSpellCard(4,1,3),
            //new IncDecDefenseSpellCard(3,2,1),

     //       new IncDecAttackSpellCard(3,1,new Point(10,10)),
            //new IncDecAttackSpellCard(4,1,3),
            //new IncDecAttackSpellCard(3,2,1),

            //new IncDecMovementDistanceSpellCard(3,1,2),
            //new IncDecMovementDistanceSpellCard(4,1,4),
            //new IncDecMovementDistanceSpellCard(3,2,1),

            //new IncDecDefenseSpellCard(3,1,-2),
            //new IncDecDefenseSpellCard(4,1,-3),
            //new IncDecDefenseSpellCard(3,2,-1),

            //new IncDecAttackSpellCard(3,1,-2),
            //new IncDecAttackSpellCard(4,1,-3),
            //new IncDecAttackSpellCard(3,2,-1),

            //new IncDecMovementDistanceSpellCard(3,1,-2),
            //new IncDecMovementDistanceSpellCard(4,1,-4),
            //new IncDecMovementDistanceSpellCard(3,2,-1),


            //new AddManaSpellCard(10),
            //new AddManaSpellCard(15),

            //new DiscardCardsSpellCard(4, 1),
            //new DiscardCardsSpellCard(6, 2),
            //new DiscardCardsSpellCard(8, 3),

            //new DisableMovementSpellCard(6, 2),
            //new DisableMovementSpellCard(9, 4),

            //new DisableAttackSpellCard(5, 1),
            //new DisableAttackSpellCard(7, 2),

         //   new HealSpellCard(5,1),
         //   new HealSpellCard(8,2),
           // new HealSpellCard(13,3),

           // new DamageSpellCard(9, 1),
        //    new DamageSpellCard(14, 2),

            //new RemoveDisableMovementsSpellCard(),
            //new RemoveDisableAttacksSpellCard()
    }
}
