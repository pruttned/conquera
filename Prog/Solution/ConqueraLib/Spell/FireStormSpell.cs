////////////////////////////////////////////////////////////////////////
////  Copyright (C) 2010 by Conquera Team
////  Part of the Conquera Project
////
////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 2 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//////////////////////////////////////////////////////////////////////////


//using System;
//using System.Collections.ObjectModel;
//using Ale.Gui;
//using Ale;
//using System.Collections.Generic;
//using Ale.Tools;
//using Microsoft.Xna.Framework;
//using Ale.Graphics;
//using Microsoft.Xna.Framework.Graphics;
//using Conquera.Gui;

//namespace Conquera
//{
//    public class FireStormSpell : Spell
//    {
//        private static int Damage = 30;
//        private static float FireBallPsysSpeed = 2.5f;
//        private static Vector3 FireBallPos = new Vector3(0.2f, 0.4f, 4);
//        private static readonly string FireBallPsys = "FireBallPsys";
//        private static readonly string ExplosionPsys = "FireExplosionPsys";
//        private static GraphicElement mPictureGraphicElement = ConqueraPalette.SpellIconFireStorm;
//        private static GraphicElement mIconGraphicElement = ConqueraPalette.SpellIconFireStorm;

//        List<ParticleSystemMissile> mMissiles = new List<ParticleSystemMissile>();

//        public override GraphicElement Picture
//        {
//            get { return mPictureGraphicElement; }
//        }

//        public override GraphicElement Icon
//        {
//            get { return mIconGraphicElement; }
//        }

//        public override string Name
//        {
//            get { return "FireStorm"; }
//        }

//        public override string DisplayName
//        {
//            get { return "Fire storm spell"; }
//        }

//        public override string Description
//        {
//            get { return "Fireballs for everyone!"; }
//        }

//        public override int Cost
//        {
//            get { return 100; }
//        }

//        public override int ApplyAttackModifiers(int baseAttack)
//        {
//            return baseAttack;
//        }

//        protected override void BeforeAttackCastImpl()
//        {
//            var targetCell = Target.Tile;
//            Target.BattleScene.FireTileNotificationLabel("", CellNotificationIcons.FireStorm, Color.Red, targetCell.Index);
//            foreach (var cell in targetCell.GetSiblings())
//            {
//                if (null != cell.GameUnit && cell.GameUnit.OwningPlayer != Caster.OwningPlayer)
//                {
//                    Target.BattleScene.FireTileNotificationLabel("", CellNotificationIcons.FireStorm, Color.Red, cell.Index);
//                }
//            }
//        }

//        protected override bool BeforeAttackUpdateImpl(AleGameTime time)
//        {
//            return false;
//        }

//        protected override void AfterAttackHitCastImpl()
//        {
//            var targetCell = Target.Tile;

//            if(null != targetCell.GameUnit)
//            {
//                var missile2 = new SpellParticleSystemMissile(Target, Target.Position + FireBallPos, Target.Position, FireBallPsys, FireBallPsysSpeed);
//                missile2.OnHit += new ParticleSystemMissile.OnHitHandler(missile_OnHit);
//                mMissiles.Add(missile2);
//            }
            
//            foreach (var cell in targetCell.GetSiblings())
//            {
//                if (null != cell.GameUnit && cell.GameUnit.OwningPlayer != Caster.OwningPlayer)
//                {
//                    var missile2 = new SpellParticleSystemMissile(cell.GameUnit, cell.GameUnit.Position + FireBallPos, cell.GameUnit.Position, FireBallPsys, FireBallPsysSpeed);
//                    missile2.OnHit += new ParticleSystemMissile.OnHitHandler(missile_OnHit);
//                    mMissiles.Add(missile2);
//                }
//            }
//        }

//        protected override bool AfterAttackHitUpdateImpl(AleGameTime time)
//        {
//            if (0 == mMissiles.Count)
//            {
//                return false;
//            }

//            for (int i = mMissiles.Count - 1; i >= 0; --i)
//            {
//                if (!mMissiles[i].Update(time))
//                {
//                    mMissiles[i].Dispose();
//                    mMissiles.RemoveAt(i);
//                }
//            }

//            return true;
//        }

//        private void missile_OnHit(ParticleSystemMissile missile)
//        {
//            var target = ((SpellParticleSystemMissile)missile).Target;

//            target.ReceiveDamage(Damage, false);
//            target.BattleScene.ParticleSystemManager.CreateFireAndForgetParticleSystem(
//                target.BattleScene.Content.Load<ParticleSystemDesc>(ExplosionPsys), target.Position);
//            target.BattleScene.GameCamera.Shake();
//        }
//    }

//}
