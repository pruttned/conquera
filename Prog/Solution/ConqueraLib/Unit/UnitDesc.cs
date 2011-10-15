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
using Ale.Content;
using Ale.Scene;
using Ale.Graphics;
using Microsoft.Xna.Framework;

namespace Conquera
{
    [NonContentPipelineAsset(typeof(GameUnitDescLoader))]
    public class GameUnitDesc : OctreeSceneObjectDesc
    {
        private GameUnitSettings mSettings;

        public string Name
        {
            get { return mSettings.Name; }
        }
        public string DisplayName
        {
            get { return mSettings.DisplayName; }
        }
        public string IdleAnimation
        {
            get { return mSettings.IdleAnimation; }
        }
        public string AttackAnimation
        {
            get { return mSettings.AttackAnimation; }
        }
        public int Cost
        {
            get { return mSettings.Cost; }
        }
        public int MaxHp
        {
            get { return mSettings.MaxHp; }
        }
        public int MinAttack
        {
            get { return mSettings.MinAttack; }
        }
        public int MaxAttack
        {
            get { return mSettings.MaxAttack; }
        }
        public float DamageAnimationTime
        {
            get { return mSettings.DamageAnimationTime; }
        }
        public int MovementDistance
        {
            get { return mSettings.MovementDistance; }
        }

        public ParticleSystemDesc BloodParticleSystem { get; private set; }

        public GameUnitDesc(GameUnitSettings settings, ContentGroup content)
            : base(settings, content)
        {
            mSettings = settings;
            BloodParticleSystem = content.Load<ParticleSystemDesc>(settings.BloodParticleSystem);
        }

        public List<AdditionalAttackTarget> GetAdditionalAttackTargets(Point attacker, Point target)
        {
            List<AdditionalAttackTarget> points = new List<AdditionalAttackTarget>();
            GetAdditionalAttackTargets(attacker, target, points);
            return points;
        }

        public void GetAdditionalAttackTargets(Point attacker, Point target, List<AdditionalAttackTarget> points)
        {
            //todo cahce?

            int rot = (int)HexHelper.GetDirectionToSibling(attacker, target);
            foreach (var additionalAttackTarget in mSettings.AdditionalAttackTargets)
            {
                Point index = attacker;
                foreach (var dir in additionalAttackTarget.Target)
                {
                    index = HexHelper.GetSibling(index, HexHelper.RotateDirection(dir, rot));
                }
                points.Add(new AdditionalAttackTarget(index, additionalAttackTarget.AttackMultiplier));
            }
        }
    }

    public class AdditionalAttackTarget
    {
        public Point Position {get; set;}
        public float AttackMultiplier {get; set;}

        public AdditionalAttackTarget ()
	    {
	    }

        public AdditionalAttackTarget(Point position, float attackMultiplier)
        {
            Position = position;
            AttackMultiplier = attackMultiplier;
        }
    }
}
