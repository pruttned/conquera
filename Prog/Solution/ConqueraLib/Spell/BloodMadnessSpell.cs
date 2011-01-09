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
    public class BloodMadnessSpell : Spell
    {
        class UnitDamage
        {
            public GameUnit GameUnit { get; private set; }
            public int Damage { get; set; }

            public UnitDamage(GameUnit gameUnit)
            {
                GameUnit = gameUnit;
            }

            public override string ToString()
            {
                return string.Format("{0}/{1}", Damage, GameUnit.Hp);
            }
        }

        private static GraphicElement mPictureGraphicElement = GuiManager.Instance.Palette.CreateGraphicElement("SpellIconBloodMadness");
        private static GraphicElement mIconGraphicElement = GuiManager.Instance.Palette.CreateGraphicElement("SpellIconBloodMadness");

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
            get { return "BloodMadness"; }
        }

        public override string DisplayName
        {
            get { return "BloodMadness spell"; }
        }

        public override string Description
        {
            get { return "etc."; }
        }

        public override int ApplyAttackModifiers(int baseAttack)
        {
            return baseAttack;
        }

        protected override void BeforeAttackCastImpl()
        {
            Target.GameScene.FireCellNotificationLabel("", CellNotificationIcons.BloodMadness, Color.Red, Target.CellIndex);
        }

        protected override bool BeforeAttackUpdateImpl(AleGameTime time)
        {
            return false;
        }

        protected override void AfterAttackHitCastImpl()
        {
            if (0 < Target.Hp)
            {
                mAttackDelay.Start(1);
            }
        }

        protected override bool AfterAttackHitUpdateImpl(AleGameTime time)
        {
            if (mAttackDelay.HasPassed(time))
            {
                if (0 < Target.Hp)
                {
                    int hpToDrain = Target.GameUnitDesc.MaxHp - Target.Hp;
                    int drainedHp = 0;

                    if (0 < hpToDrain)
                    {
                        List<UnitDamage> fellowUnits = new List<UnitDamage>();
                        var targetCell = Target.Cell;
                        foreach (var cell in targetCell.GetSiblings())
                        {
                            if (null != cell.GameUnit && cell.GameUnit.OwningPlayer != Caster.OwningPlayer)
                            {
                                fellowUnits.Add(new UnitDamage(cell.GameUnit));
                            }
                        }

                        while (0 < fellowUnits.Count && 0 < hpToDrain)
                        {
                            int singleDamage = Math.Max(1, hpToDrain / fellowUnits.Count);
                            for (int i = fellowUnits.Count - 1; i >= 0 && 0 < hpToDrain; --i)
                            {
                                var unitDmg = fellowUnits[i];
                                int singleDrainedHp = Math.Min((unitDmg.GameUnit.Hp - unitDmg.Damage), singleDamage);
                                drainedHp += singleDrainedHp;
                                hpToDrain -= singleDrainedHp;
                                unitDmg.Damage += singleDrainedHp;
                                if (unitDmg.Damage == unitDmg.GameUnit.Hp) //is going to kill the unit
                                {
                                    unitDmg.GameUnit.ReceiveDamage(unitDmg.Damage);
                                    fellowUnits.RemoveAt(i);
                                }
                            }
                        }

                        if (0 < fellowUnits.Count)
                        {
                            foreach (var unitDmg in fellowUnits)
                            {
                                unitDmg.GameUnit.ReceiveDamage(unitDmg.Damage);
                            }
                        }

                        Target.Heal(drainedHp);
                    }
                }
                return false;
            }
            return true;
        }
    }

}
