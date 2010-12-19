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
