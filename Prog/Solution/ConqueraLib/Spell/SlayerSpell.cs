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
    public class SlayerSpell : Spell
    {
        private static int Damage = 50;
        private static readonly string SlayerPsys = "SlayerPsys";
        private static GraphicElement mPictureGraphicElement = GuiManager.Instance.Palette.CreateGraphicElement("SpellIconSlayer");
        private static GraphicElement mIconGraphicElement = GuiManager.Instance.Palette.CreateGraphicElement("SpellIconSlayer");

        private AnimationDelay mAttackDelay = new AnimationDelay();

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
            get { return "Slayer"; }
        }

        public override string DisplayName
        {
            get { return "Slayer spell"; }
        }

        public override string Description
        {
            get { return string.Format("Increases attack +{0} or so.", Damage); }
        }

        public override int ApplyAttackModifiers(int baseAttack)
        {
            return baseAttack + Damage;
        }

        protected override void BeforeAttackCastImpl()
        {
            Caster.GameScene.FireCellNotificationLabel("", CellNotificationIcons.Slayer, Color.Red, Caster.CellIndex);
            Caster.GameScene.ParticleSystemManager.CreateFireAndforgetParticleSystem(
                Caster.GameScene.Content.Load<ParticleSystemDesc>(SlayerPsys), Caster.Position);

            mAttackDelay.Start(1);
        }

        protected override bool BeforeAttackUpdateImpl(AleGameTime time)
        {
            return !mAttackDelay.HasPassed(time);
        }

        protected override void AfterAttackHitCastImpl()
        {
        }

        protected override bool AfterAttackHitUpdateImpl(AleGameTime time)
        {
            return false;
        }
    }

}
