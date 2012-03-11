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
//    public class PlagueSpell : Spell
//    {
//        private static GraphicElement mPictureGraphicElement = ConqueraPalette.SpellIconPlague;
//        private static GraphicElement mIconGraphicElement = ConqueraPalette.SpellIconPlague;
//        private static int FirstZoneDamage = 60;
//        private static int[] RestZoneDamages = new int[] { 20, 10 };

//        private AnimationDelay mAttackDelay = new AnimationDelay();
//        private Dictionary<BattleUnit, int> mUnitDamages = new Dictionary<BattleUnit, int>();

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
//            get { return "Plague"; }
//        }

//        public override string DisplayName
//        {
//            get { return "Plague spell"; }
//        }

//        public override string Description
//        {
//            get { return "etc."; }
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
//            mUnitDamages.Clear();

//            var targetCell = Target.Tile;

//            List<BattleUnit> leafUnits = new List<BattleUnit>();
//            List<BattleUnit> leafUnits2 = new List<BattleUnit>();
//            List<HexCell> siblings = new List<HexCell>();

//            mUnitDamages.Add(Target, FirstZoneDamage);
//            leafUnits.Add(Target);

//            foreach (int dmg in RestZoneDamages)
//            {
//                if (0 == leafUnits.Count)
//                {
//                    break;
//                }

//                leafUnits2.Clear();

//                foreach (var leafUnit in leafUnits)
//                {
//                    siblings.Clear();
//                    leafUnit.Tile.GetSiblings(siblings);
//                    foreach (var cell in siblings)
//                    {
//                        if (null != cell.GameUnit && cell.GameUnit.OwningPlayer != Caster.OwningPlayer && !mUnitDamages.ContainsKey(cell.GameUnit))
//                        {
//                            mUnitDamages.Add(cell.GameUnit, dmg);
//                            leafUnits2.Add(cell.GameUnit);
//                        }
//                    }
//                }

//                List<BattleUnit> auxLeafUnits = leafUnits;
//                leafUnits = leafUnits2;
//                leafUnits2 = auxLeafUnits;
//            }
//        }

//        protected override bool BeforeAttackUpdateImpl(AleGameTime time)
//        {
//            return false;
//        }

//        protected override void AfterAttackHitCastImpl()
//        {
//            mAttackDelay.Start(1);
//        }

//        protected override bool AfterAttackHitUpdateImpl(AleGameTime time)
//        {
//            if (mAttackDelay.HasPassed(time))
//            {
//                foreach (var dmg in mUnitDamages)
//                {
//                    if (0 < dmg.Key.Hp)
//                    {
//                        dmg.Key.ReceiveDamage(dmg.Value);
//                    }
//                }
//                return false;
//            }
//            return true;
//        }
//    }

//}
