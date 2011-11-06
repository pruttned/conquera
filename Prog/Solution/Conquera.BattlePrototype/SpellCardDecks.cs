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

            new Add2ManaCard(),
            new Add5ManaCard(),
            new Add7ManaCard(),
            new Add10ManaCard(),
            new Add15ManaCard()
        };
    }
}
