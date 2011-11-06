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
        private BattleUnit mGameUnit;

        public bool IsIdle
        {
            get { return true; }
        }

        public IdleGameUnitState(BattleUnit gameUnit)
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
        private BattleUnit mGameUnit;
        private Vector3LinearAnimator mMovementAnimator = new Vector3LinearAnimator();

        public Point TargetCell { get; set; }

        public bool IsIdle
        {
            get { return false; }
        }

        public MovingGameUnitState(BattleUnit gameUnit)
        {
            if (null == gameUnit) throw new ArgumentNullException("gameUnit");
            mGameUnit = gameUnit;
        }

        public void OnStart()
        {
            mGameUnit.RotateTo(TargetCell);
            mMovementAnimator.Animate(15, mGameUnit.Position, mGameUnit.GetPositionFromIndex(TargetCell));
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
                mGameUnit.TileIndex = TargetCell;
            }
        }
    }

    public class AttackingGameUnitState : IGameUnitState
    {
        private BattleUnit mGameUnit;
        private float mDamageTime;
        public BattleUnit TargetUnit { get; set; }

        public bool IsIdle
        {
            get { return false; }
        }

        private BattleScene GameScene
        {
            get { return mGameUnit.BattleScene; }
        }

        public AttackingGameUnitState(BattleUnit gameUnit)
        {
            if (null == gameUnit) throw new ArgumentNullException("gameUnit");
            mGameUnit = gameUnit;
        }

        public void OnStart()
        {
            if (null == TargetUnit) throw new ArgumentNullException("TargetUnit");

            mGameUnit.AnimationPlayer.Animation = mGameUnit.GameUnitDesc.AttackAnimation;
            mGameUnit.AnimationPlayer.Play(false);

            mDamageTime = -1;
        }

        public void Update(AleGameTime gameTime)
        {
            throw new NotImplementedException();
            //todo

            //if(0 > mDamageTime)
            //{
            //    mDamageTime = gameTime.TotalTime + mGameUnit.GameUnitDesc.DamageAnimationTime;
            //}
            //if(gameTime.TotalTime > mDamageTime)
            //{
            //    //main target
            //    int damage = mGameUnit.ComputeDamageTo(TargetUnit);
            //    TargetUnit.ReceiveDamage(damage);

            //    //additional targets
            //    var targets = mGameUnit.GetAdditionalAttackTargets(TargetUnit);
            //    foreach (var t in targets)
            //    {
            //        var targetUnit = GameScene.Terrain[t.Position].GameUnit;
            //        if (null != targetUnit && targetUnit.OwningPlayer != mGameUnit.OwningPlayer)
            //        {
            //            float damage2 = (float)damage * t.AttackMultiplier;
            //            targetUnit.ReceiveDamage((int)Math.Ceiling(damage2));
            //        }
            //    }

            //    mGameUnit.State = mGameUnit.States["Idle"];
            //}
        }
    }
}
