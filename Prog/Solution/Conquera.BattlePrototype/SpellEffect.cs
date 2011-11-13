using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquera.BattlePrototype
{
    public interface IBattleUnitSpellEffect
    {
        bool OnStartTurn(int turnNum, BattlePlayer playerOnTurn);
    }

    public interface IBattlePlayerSpellEffect
    {
        bool OnStartTurn();
        bool OnUnitDeath();
    }


    //public class DefenseBattleUnitSpellEffect : IBattleUnitSpellEffect
    //{
    //    bool OnStartTurn();
    //    int GetDefenseModifier();
    //    int GetAttackModifier();
    //    int GetMoveDistanceModifier();
    //}

    public interface IBattleUnitDefenseModifier
    {
        int GetModifier(BattleUnit unit);
    }
    public interface IBattleUnitAttackModifier
    {
        int GetModifier(BattleUnit unit);
    }
    public interface IBattleUnitMovementDistanceModifier
    {
        int GetModifier(BattleUnit unit);
    }

}
