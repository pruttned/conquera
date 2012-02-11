using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Conquera.BattlePrototype
{
    public interface IBattleUnitSpellEffect
    {
        string Description { get; }
        void OnCast(int turnNum, BattleUnit unit);
        void OnEnd();
        bool OnStartTurn(int turnNum);        
    }

    public interface IBattleUnitDefenseModifier
    {
        int GetModifier(BattleUnit unit);
    }
    public interface IBattleUnitAttackModifier
    {
        Point GetModifier(BattleUnit unit);
    }
    public interface IBattleUnitMovementDistanceModifier
    {
        int GetModifier(BattleUnit unit);
    }

    public abstract class BattleUnitSpellEffectWithDuration : IBattleUnitSpellEffect
    {
        private int mDuration;
        private int mEndTurn;
        private int mTurnNum = -1;

        public string Description
        {
            get { return string.Format("{0} ({1})", EffectDescription, mEndTurn - mTurnNum); }
        }

        public abstract string EffectDescription { get; }

        public BattleUnitSpellEffectWithDuration(int duration)
        {
            mDuration = duration;
        }

        public void OnCast(int turnNum, BattleUnit unit)
        {
            mEndTurn = turnNum + mDuration;
            mTurnNum = turnNum;

            OnCastImpl(turnNum, unit);
        }

        public bool OnStartTurn(int turnNum)
        {
            if (mEndTurn <= turnNum)
            {
                return false;
            }
            mTurnNum = turnNum;

            return OnStartTurnImpl(turnNum);
        }

        public abstract void OnEnd();

        protected abstract void OnCastImpl(int turnNum, BattleUnit unit);

        protected abstract bool OnStartTurnImpl(int turnNum);

        #region IBattleUnitSpellEffect Members

        #endregion
    }

    public class ConstIncMovementDistanceBattleUnitSpellEffect : BattleUnitSpellEffectWithDuration, IBattleUnitMovementDistanceModifier
    {
        private int mAmount;
        private BattleUnit mUnit;

        public override string EffectDescription
        {
            get { return string.Format("Movement {0}{1}", 0 < mAmount ? "+" : null, mAmount); }
        }

        public int GetModifier(BattleUnit unit)
        {
            return mAmount;
        }

        public ConstIncMovementDistanceBattleUnitSpellEffect(int amount, int duration)
            : base(duration)
        {
            mAmount = amount;
        }

        public override void OnEnd()
        {
            mUnit.RemoveMovementDistanceModifier(this);
        }

        protected override void OnCastImpl(int turnNum, BattleUnit unit)
        {
            unit.AddMovementDistanceModifier(this);
            mUnit = unit;
        }

        protected override bool OnStartTurnImpl(int turnNum)
        {
            return true;
        }
    }

    public class DisableMovementBattleUnitSpellEffect : BattleUnitSpellEffectWithDuration
    {
        BattleUnit mUnit;
        public override string EffectDescription
        {
            get { return "Disables movement"; }
        }

        public DisableMovementBattleUnitSpellEffect(int duration)
            :base(duration)
        {
        }

        public override void OnEnd()
        {
            mUnit.MovementPreventerCnt--;
        }

        protected override void OnCastImpl(int turnNum, BattleUnit unit)
        {
            mUnit = unit;
            unit.MovementPreventerCnt++;
        }

        protected override bool OnStartTurnImpl(int turnNum)
        {
            return true;
        }
    }
    public class DisableAttackBattleUnitSpellEffect : BattleUnitSpellEffectWithDuration
    {
        BattleUnit mUnit;
        public override string EffectDescription
        {
            get { return "Disables attack"; }
        }

        public DisableAttackBattleUnitSpellEffect(int duration)
            : base(duration)
        {
        }

        public override void OnEnd()
        {
            mUnit.AttackPreventerCnt--;
        }

        protected override void OnCastImpl(int turnNum, BattleUnit unit)
        {
            mUnit = unit;
            unit.AttackPreventerCnt++;
        }

        protected override bool OnStartTurnImpl(int turnNum)
        {
            return true;
        }
    }

    public class BerserkerBattleUnitSpellEffect : BattleUnitSpellEffectWithDuration
    {
        BattleUnit mUnit;
        public override string EffectDescription
        {
            get { return "Berserker"; }
        }

        public BerserkerBattleUnitSpellEffect(int duration)
            : base(duration)
        {
        }

        public override void OnEnd()
        {
            mUnit.BerserkerEnablerCnt--;
        }

        protected override void OnCastImpl(int turnNum, BattleUnit unit)
        {
            mUnit = unit;
            unit.BerserkerEnablerCnt++;
        }

        protected override bool OnStartTurnImpl(int turnNum)
        {
            return true;
        }
    }



    public class FirstStrikeBattleUnitSpellEffect : BattleUnitSpellEffectWithDuration
    {
        BattleUnit mUnit;
        public override string EffectDescription
        {
            get { return "Firstrike"; }
        }

        public FirstStrikeBattleUnitSpellEffect(int duration)
            : base(duration)
        {
        }

        public override void OnEnd()
        {
            mUnit.FirstStrikeEnablerCnt--;
        }

        protected override void OnCastImpl(int turnNum, BattleUnit unit)
        {
            mUnit = unit;
            unit.FirstStrikeEnablerCnt++;
        }

        protected override bool OnStartTurnImpl(int turnNum)
        {
            return true;
        }
    }

}
