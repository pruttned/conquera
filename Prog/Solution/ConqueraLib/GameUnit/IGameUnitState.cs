using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale;

namespace Conquera
{
    public interface IGameUnitState
    {
        bool IsIdle { get; }
        void OnStart();
        void Update(AleGameTime gameTime);
    }

    public class IdleGameUnitState : IGameUnitState
    {
        private GameUnit mGameUnit;

        public bool IsIdle
        {
            get { return true; }
        }

        public IdleGameUnitState(GameUnit gameUnit)
        {
            if (null == gameUnit) throw new ArgumentNullException("gameUnit");
            mGameUnit = gameUnit;
        }

        public void OnStart()
        {
        }

        public void Update(AleGameTime gameTime)
        {
        }
    }
    public class MovingGameUnitState : IGameUnitState
    {
        private GameUnit mGameUnit;
        private Vector3LinearAnimator mMovementAnimator = new Vector3LinearAnimator();

        public Point TargetCell { get; set; }

        public bool IsIdle
        {
            get { return false; }
        }

        public MovingGameUnitState(GameUnit gameUnit)
        {
            if (null == gameUnit) throw new ArgumentNullException("gameUnit");
            mGameUnit = gameUnit;
        }

        public void OnStart()
        {
            mGameUnit.RotateTo(TargetCell);
            mMovementAnimator.Animate(5, mGameUnit.Position, mGameUnit.GetPositionFromIndex(TargetCell));
            
            mGameUnit.AnimationPlayer.Animation = mGameUnit.GameUnitDesc.MoveAnimation;
            mGameUnit.AnimationPlayer.Play(true);
        }

        public void Update(AleGameTime gameTime)
        {
            if (mMovementAnimator.Update(gameTime))
            {
                mGameUnit.Position = mMovementAnimator.CurrentValue;
            }
            else
            {
                mGameUnit.AnimationPlayer.Animation = mGameUnit.GameUnitDesc.IdleAnimation;
                mGameUnit.AnimationPlayer.Play(true);

                mGameUnit.State = mGameUnit.States["Idle"];
                mGameUnit.CellIndex = TargetCell;
            }
        }
    }
    public class AttackingGameUnitState : IGameUnitState
    {
        private GameUnit mGameUnit;
        private float mDamageTime;
        public GameUnit TargetUnit { get; set; }

        public bool IsIdle
        {
            get { return false; }
        }

        public AttackingGameUnitState(GameUnit gameUnit)
        {
            if (null == gameUnit) throw new ArgumentNullException("gameUnit");
            mGameUnit = gameUnit;
        }

        public void OnStart()
        {
            if (null == TargetUnit) throw new ArgumentNullException("TargetUnit");
            mGameUnit.RotateTo(TargetUnit.CellIndex);

            mGameUnit.AnimationPlayer.Animation = mGameUnit.GameUnitDesc.AttackAnimation;
            mGameUnit.AnimationPlayer.Play(false);

            mDamageTime = -1;
        }

        public void Update(AleGameTime gameTime)
        {
            if(0 > mDamageTime)
            {
                mDamageTime = gameTime.TotalTime + mGameUnit.GameUnitDesc.DamageAnimationTime;
            }
            if(gameTime.TotalTime > mDamageTime)
            {
                int damage = mGameUnit.ComputeDamageTo(TargetUnit);
                TargetUnit.ReceiveDamage(damage);
                mGameUnit.State = mGameUnit.States["Idle"];
            }
        }
    }
}
