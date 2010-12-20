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

namespace Conquera
{
    [NonContentPipelineAsset(typeof(GameUnitDescLoader))]
    public class GameUnitDesc : OctreeSceneObjectDesc
    {
        private GameUnitSttings mSettings;

        public string IdleAnimation
        {
            get { return mSettings.IdleAnimation; }
        }
        public string MoveAnimation
        {
            get { return mSettings.MoveAnimation; }
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
        public int BaseRedAttack
        {
            get { return mSettings.BaseAttackPurple; }
        }
        public int BaseRedDefense
        {
            get { return mSettings.BaseDefensePurple; }
        }
        public int BaseGreenAttack
        {
            get { return mSettings.BaseAttackGreen; }
        }
        public int BaseGreenDefense
        {
            get { return mSettings.BaseDefenseGreen; }
        }
        public int BaseBlackAttack
        {
            get { return mSettings.BaseAttackBlack; }
        }
        public int BaseBlackDefense
        {
            get { return mSettings.BaseDefenseBlack; }
        }
        public float DamageAnimationTime
        {
            get { return mSettings.DamageAnimationTime; }
        }

        public ParticleSystemDesc BloodParticleSystem { get; private set; }

        public GameUnitDesc(GameUnitSttings settings, ContentGroup content)
            : base(settings, content)
        {
            mSettings = settings;
            BloodParticleSystem = content.Load<ParticleSystemDesc>(settings.BloodParticleSystem);
        }
    }

}
