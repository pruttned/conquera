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
        public string MoveAnimation { get; set; }
        [DataProperty(NotNull = true)]
        public string AttackAnimation { get; set; }

        [DataProperty(NotNull = true)]
        public int Cost { get; set; }
        [DataProperty(NotNull = true)]
        public int MaxHp { get; set; }

        [DataProperty(NotNull = true)]
        public int Attack { get; set; }
        [DataProperty(NotNull = true)]
        public int Defense { get; set; }
        [DataProperty(NotNull = true)]
        public int BaseAttackGreen { get; set; }
        [DataProperty(NotNull = true)]
        public int BaseDefenseGreen { get; set; }
        [DataProperty(NotNull = true)]
        public int BaseAttackBlack { get; set; }
        [DataProperty(NotNull = true)]
        public int BaseDefenseBlack { get; set; }

        [DataProperty(NotNull = true)]
        public float DamageAnimationTime{ get; set; }

        [DataProperty(NotNull = true)]
        public long BloodParticleSystem { get; set; }
    }
}
