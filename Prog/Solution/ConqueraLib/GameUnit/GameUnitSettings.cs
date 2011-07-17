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
using SimpleOrmFramework;
using Ale.Scene;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    public class GameUnitSettings : OctreeSceneObjectSettings
    {
        [DataProperty(NotNull = true)]
        public string DisplayName { get; set; }

        [DataProperty(NotNull = true)]
        public string IdleAnimation { get; set; }
        [DataProperty(NotNull = true)]
        public string AttackAnimation { get; set; }

        [DataProperty(NotNull = true)]
        public int Cost { get; set; }
        [DataProperty(NotNull = true)]
        public int MaxHp { get; set; }

        [DataProperty(NotNull = true)]
        public int MinAttack { get; set; }
        
        [DataProperty(NotNull = true)]
        public int MaxAttack { get; set; }

        [DataProperty(NotNull = true)]
        public int MovementDistance { get; set; }
        
        [DataProperty(NotNull = true)]
        public float DamageAnimationTime{ get; set; }

        [DataProperty(NotNull = true)]
        public long BloodParticleSystem { get; set; }

        [DataListProperty(NotNull = false)]
        public List<AdditionalAttackTargetSettings> AdditionalAttackTargets { get; set; }
    }

    [DataObject(MaxCachedCnt = 0)]
    public class AdditionalAttackTargetSettings : BaseDataObject
    {
        [DataProperty(NotNull = true)]
        public float AttackMultiplier { get; set; }

        /// <summary>
        /// Target cell to attack given by a set of directions when the direction from attacker to the target is UperRight
        /// </summary>
        [DataListProperty(NotNull=true)]
        public List<HexDirection> Target { get; set; }

        public AdditionalAttackTargetSettings()
        {
        }

        public AdditionalAttackTargetSettings(float attackMultiplier, params HexDirection[] target)
        {
            AttackMultiplier = attackMultiplier;
            Target = new List<HexDirection>(target);
        }
    }
}
