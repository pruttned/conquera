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

            var activeSpell = mGameUnit.GameScene.ActiveSpell;
            if (null != activeSpell)
            {
                activeSpell.Spell.BeforeAttack(mGameUnit, TargetUnit);
            }

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
                var activeSpell = mGameUnit.GameScene.ActiveSpell;

                int damage = mGameUnit.ComputeDamageTo(TargetUnit, activeSpell.Spell);
                TargetUnit.ReceiveDamage(damage);

                if (null != activeSpell)
                {
                    var spellState = (CastingSpellGameUnitState)mGameUnit.States["CastingSpell"];
                    spellState.TargetUnit = TargetUnit;
                    mGameUnit.State = mGameUnit.States["CastingSpell"];
                }
                else
                {
                    mGameUnit.State = mGameUnit.States["Idle"];
                }
            }
        }
    }

    public class CastingSpellGameUnitState : IGameUnitState
    {
        private GameUnit mGameUnit;
        public GameUnit TargetUnit { get; set; }

        public bool IsIdle
        {
            get { return false; }
        }

        public CastingSpellGameUnitState(GameUnit gameUnit)
        {
            if (null == gameUnit) throw new ArgumentNullException("gameUnit");
            mGameUnit = gameUnit;
        }

        public void OnStart()
        {
            if (null == TargetUnit) throw new ArgumentNullException("TargetUnit");

            if (!mGameUnit.GameScene.ActiveSpell.Spell.Cast(mGameUnit, TargetUnit))
            {
                //no spell animation
                mGameUnit.State = mGameUnit.States["Idle"];
            }
        }

        public void Update(AleGameTime gameTime)
        {
            if (!mGameUnit.GameScene.ActiveSpell.Spell.Update(gameTime))
            {
                  mGameUnit.State = mGameUnit.States["Idle"];
            }
        }
    }

}
