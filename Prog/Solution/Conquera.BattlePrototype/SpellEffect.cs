using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquera.BattlePrototype
{
    public interface IBattlePlayerSpellEffect
    {
        bool OnStartTurn();
        bool OnUnitDeath();
    }

}
