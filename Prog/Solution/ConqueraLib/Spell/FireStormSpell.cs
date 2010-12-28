﻿//////////////////////////////////////////////////////////////////////
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
using System.Collections.ObjectModel;
using Ale.Gui;
using Ale;
using System.Collections.Generic;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    public class FireStormSpell : Spell
    {
        private static int Damage = 3;
        private static float FireBallPsysSpeed = 2.5f;
        private static Vector3 FireBallPos = new Vector3(0.2f, 0.4f, 4);
        private static readonly string FireBallPsys = "FireBallPsys";
        private static readonly string ExplosionPsys = "FireExplosionPsys";
        private static GraphicElement mPictureGraphicElement = GuiManager.Instance.Palette.CreateGraphicElement("SpellIconFireStorm");
        private static GraphicElement mIconGraphicElement = GuiManager.Instance.Palette.CreateGraphicElement("SpellIconFireStorm");

        List<ParticleSystemMissile> mMissiles = new List<ParticleSystemMissile>();

        public override GraphicElement Picture
        {
            get { return mPictureGraphicElement; }
        }

        public override GraphicElement Icon
        {
            get { return mIconGraphicElement; }
        }

        public override string Name
        {
            get { return "FireStorm"; }
        }

        public override string DisplayName
        {
            get { return "Fire storm spell"; }
        }

        public override string Description
        {
            get { return "Fireballs for everyone!"; }
        }

        public override void ApplyAttackDefenseModifiers(ref int attack, ref int defense)
        {
        }

        protected override void BeforeAttackCastImpl()
        {
            foreach (var cell in Caster.GameScene.GetCell(Caster.CellIndex).GetSiblings())
            {
                if (null != cell.GameUnit && cell.GameUnit.OwningPlayer != Caster.OwningPlayer)
                {
                    Caster.GameScene.FireCellNotificationLabel("", CellNotificationIcons.FireStorm, Color.Red, cell.Index);
                }
            }
        }

        protected override bool BeforeAttackUpdateImpl(AleGameTime time)
        {
            return false;
        }

        protected override void AfterAttackHitCastImpl()
        {
            foreach (var cell in Caster.GameScene.GetCell(Caster.CellIndex).GetSiblings())
            {
                if (null != cell.GameUnit && cell.GameUnit.OwningPlayer != Caster.OwningPlayer)
                {
                    var missile2 = new ParticleSystemMissile(cell.GameUnit, cell.GameUnit.Position + FireBallPos, cell.GameUnit.Position, FireBallPsys, FireBallPsysSpeed);
                    missile2.OnHit += new ParticleSystemMissile.OnHitHandler(missile_OnHit);
                    mMissiles.Add(missile2);
                }
            }
        }

        protected override bool AfterAttackHitUpdateImpl(AleGameTime time)
        {
            if (0 == mMissiles.Count)
            {
                return false;
            }

            for (int i = mMissiles.Count - 1; i >= 0; --i)
            {
                if (!mMissiles[i].Update(time))
                {
                    mMissiles[i].Dispose();
                    mMissiles.RemoveAt(i);
                }
            }

            return true;
        }

        private void missile_OnHit(ParticleSystemMissile missile, GameUnit target)
        {
            target.ReceiveDamage(Damage, false);
            target.GameScene.ParticleSystemManager.CreateFireAndforgetParticleSystem(
                target.GameScene.Content.Load<ParticleSystemDesc>(ExplosionPsys), target.Position);
            target.GameScene.GameCamera.Shake();
        }
    }

}
