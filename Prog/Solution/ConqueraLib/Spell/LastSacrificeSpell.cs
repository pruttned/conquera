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
using Conquera.Gui;

namespace Conquera
{
    public class LastSacrificeSpell : Spell
    {
        private static GraphicElement mPictureGraphicElement = ConqueraPalette.SpellIconLastSacrifice;
        private static GraphicElement mIconGraphicElement = ConqueraPalette.SpellIconLastSacrifice;
        private static float DivCoef = 2;

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
            get { return "LastSacrifice"; }
        }

        public override string DisplayName
        {
            get { return "LastSacrifice spell"; }
        }

        public override string Description
        {
            get { return "etc."; }
        }

        public override int Cost
        {
            get { return 100; }
        }

        public override int ApplyAttackModifiers(int baseAttack)
        {
            return baseAttack;
        }

        protected override void BeforeAttackCastImpl()
        {
            Caster.GameScene.FireCellNotificationLabel("", CellNotificationIcons.LastSacrifice, Color.Red, Caster.CellIndex);
        }

        protected override bool BeforeAttackUpdateImpl(AleGameTime time)
        {
            return false;
        }

        protected override void AfterAttackHitCastImpl()
        {
            //todo: fire some cool animation
            mAttackDelay.Start(1);
        }

        protected override bool AfterAttackHitUpdateImpl(AleGameTime time)
        {
            if (mAttackDelay.HasPassed(time))
            {
                var casterCell = Caster.Cell;
                List<GameUnit> friends = new List<GameUnit>();
                List<GameUnit> enemies = new List<GameUnit>();

                foreach (var cell in casterCell.GetSiblings())
                {
                    if (null != cell.GameUnit)
                    {
                        if (cell.GameUnit.OwningPlayer == Caster.OwningPlayer)
                        {
                            friends.Add(cell.GameUnit);
                        }
                        else
                        {
                            enemies.Add(cell.GameUnit);
                        }
                    }
                }

                if (0 < friends.Count + enemies.Count)
                {
                    int amount = (int)Math.Ceiling(Caster.Hp / (float)((friends.Count + enemies.Count) * DivCoef));
                    foreach (var unit in friends)
                    {
                        unit.Heal(amount);
                    }
                    foreach (var unit in enemies)
                    {
                        unit.ReceiveDamage(amount);
                    }

                }

                Caster.ReceiveDamage(Caster.Hp);

                return false;
            }
            return true;
        }
    }

}
