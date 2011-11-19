using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquera.BattlePrototype
{
    public interface IBattleUnitSpellEffect
    {
        void OnCast(int turnNum, BattleUnit unit);
        void OnEnd();
        bool OnStartTurn(int turnNum, BattlePlayer playerOnTurn);
    }

    public interface IBattlePlayerSpellEffect
    {
        bool OnStartTurn();
        bool OnUnitDeath();
    }

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

    public abstract class BattleUnitSpellEffectWithDuration : IBattleUnitSpellEffect
    {
        private int mDuration;
        private int mEndTurn;

        public BattleUnitSpellEffectWithDuration(int duration)
        {
            mDuration = duration;
        }

        public void OnCast(int turnNum, BattleUnit unit)
        {
            mEndTurn= turnNum + mDuration;

            OnCastImpl(turnNum, unit);
        }

        public bool OnStartTurn(int turnNum, BattlePlayer playerOnTurn)
        {
            if (mEndTurn <= turnNum)
            {
                return false;
            }

            return OnStartTurnImpl(turnNum, playerOnTurn);
        }

        public abstract void OnEnd();

        protected abstract void OnCastImpl(int turnNum, BattleUnit unit);

        protected abstract bool OnStartTurnImpl(int turnNum, BattlePlayer playerOnTurn);

        #region IBattleUnitSpellEffect Members

        #endregion
    }

    public class ConstIncDefenseBattleUnitSpellEffect : BattleUnitSpellEffectWithDuration, IBattleUnitDefenseModifier
    {
        private int mAmount;
        private BattleUnit mUnit;

        public int GetModifier(BattleUnit unit)
        {
            return mAmount;
        }

        public ConstIncDefenseBattleUnitSpellEffect(int amount, int duration)
            :base(duration)
        {
            mAmount = amount;
        }

        public override void OnEnd()
        {
            mUnit.RemoveDefenseModifier(this);
        }

        protected override void OnCastImpl(int turnNum, BattleUnit unit)
        {
            unit.AddDefenseModifier(this);
            mUnit = unit;
        }

        protected override bool OnStartTurnImpl(int turnNum, BattlePlayer playerOnTurn)
        {
            return true;
        }
    }
}
