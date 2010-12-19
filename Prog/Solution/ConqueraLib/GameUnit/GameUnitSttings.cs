using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;
using Ale.Scene;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    public class GameUnitSttings : OctreeSceneObjectSettings
    {
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
        public int BaseAttackPurple { get; set; }
        [DataProperty(NotNull = true)]
        public int BaseDefensePurple { get; set; }
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
