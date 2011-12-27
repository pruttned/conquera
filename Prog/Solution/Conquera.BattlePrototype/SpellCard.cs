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
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Windows.Controls;

namespace Conquera.BattlePrototype
{
    public abstract class SpellCard
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual object ToolTip
        {
            get { return Description; }
        }

        public abstract int Cost { get; }

        public abstract bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain);

        public abstract void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain);

        public override string ToString()
        {
            return string.Format("{0}: {1}", Cost, Name);
        }

        public virtual void Discard(int turnNum, BattlePlayer player)
        {
            player.Mana += Cost;
        }

    }

    public class AddManaSpellCard : SpellCard
    {
        private int mManaInc;

        public override string Name
        {
            get { return string.Format("Mana +{0}", mManaInc); }
        }

        public override string Description
        {
            get { return string.Format("Mana +{0}", mManaInc); }
        }

        public override int Cost { get { return 0; } }

        public AddManaSpellCard(int manaInc)
        {
            mManaInc = manaInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return true;
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            if (null == player) throw new ArgumentNullException("player");

            player.Mana += mManaInc;
        }
    }

    public class SummonUnitSpellCard : SpellCard
    {
        private static Type[] mUnitCtorArgTypes = new Type[] { typeof(BattlePlayer), typeof(HexTerrain), typeof(Point) };
        private ConstructorInfo mUnitCtor;
        private static List<HexTerrainTile> mSiblings = new List<HexTerrainTile>();
        private int mCost;

        public override string Name
        {
            get { return string.Format("Summon {0}", mUnitCtor.DeclaringType.Name); }
        }

        public override string Description
        {
            get
            {
                BattleUnit unit = (BattleUnit)Activator.CreateInstance(mUnitCtor.DeclaringType);
                return string.Format("Attack: {0}\nDefense: {1}\nMovement: {2}", unit.BaseAttack, unit.BaseDefense, unit.BaseMovementDistance);
            }
        }

        public override object ToolTip
        {
            get
            {
                StackPanel panel = new StackPanel();
                
                BattleUnit unit = (BattleUnit)Activator.CreateInstance(mUnitCtor.DeclaringType);
                panel.Children.Add(unit.CreateImage());

                TextBlock textBlock = new TextBlock();
                textBlock.Text = Description;
                panel.Children.Add(textBlock);

                return panel;
            }
        }

        public override int Cost { get { return mCost; } }

        public SummonUnitSpellCard(Type unitType, int cost)
        {
            if (null == unitType) throw new ArgumentNullException("unitType");

            mUnitCtor = unitType.GetConstructor(mUnitCtorArgTypes);
            if (null == mUnitCtor)
            {
                throw new Exception(string.Format("Type '{0}' is missing public ctor with arguments '{1}'", unitType.Name,
                    string.Join(",", (from t in mUnitCtorArgTypes select t.Name).ToArray())));
            }

            mCost = cost;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            if (tile.IsPassableAndEmpty)
            {
                //todo: cast on existing unit
                OutpostHexTerrainTile outpost = tile as OutpostHexTerrainTile;
                if (outpost != null && outpost.OwningPlayer == player && null == tile.Unit)
                {
                    return true;
                }
                mSiblings.Clear();
                terrain.GetSiblings(tile.Index, mSiblings);
                return (from s in mSiblings where s.Unit is HeroBattleUnit && s.Unit.Player == player select 1).Any();
            }
            return false;
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            if (null == player) throw new ArgumentNullException("player");
            if (null == tile) throw new ArgumentNullException("tile");


            //todo: cast on existing unit

            mUnitCtor.Invoke(new object[] { player, terrain, tile.Index });
        }
    }



    #region ManaCards

    public class Add5ManaCard : AddManaSpellCard
    {
        public Add5ManaCard()
            :base(5)
        {
        }
    }
    public class Add7ManaCard : AddManaSpellCard
    {
        public Add7ManaCard()
            : base(7)
        {
        }
    }
    public class Add10ManaCard : AddManaSpellCard
    {
        public Add10ManaCard()
            : base(10)
        {
        }
    }
    public class Add15ManaCard : AddManaSpellCard
    {
        public Add15ManaCard()
            : base(15)
        {
        }
    }
    public class Add20ManaCard : AddManaSpellCard
    {
        public Add20ManaCard()
            : base(20)
        {
        }
    }
    #endregion ManaCards

    #region UnitCards

    public class SummonSkeletonLv1UnitSpellCard : SummonUnitSpellCard
    {
        public SummonSkeletonLv1UnitSpellCard()
            :base(typeof(SkeletonLv1BattleUnit), 3)
        {
        }
    }
    public class SummonZombieLv1UnitSpellCard : SummonUnitSpellCard
    {
        public SummonZombieLv1UnitSpellCard()
            : base(typeof(ZombieLv1BattleUnit), 3)
        {
        }
    }
    public class SummonBansheeLv1UnitSpellCard : SummonUnitSpellCard
    {
        public SummonBansheeLv1UnitSpellCard()
            : base(typeof(BansheeLv1BattleUnit), 3)
        {
        }
    }
    public class SummonSpectreLv1UnitSpellCard : SummonUnitSpellCard
    {
        public SummonSpectreLv1UnitSpellCard()
            : base(typeof(SpectreLv1BattleUnit), 3)
        {
        }
    }

    public class SummonSkeletonLv2UnitSpellCard : SummonUnitSpellCard
    {
        public SummonSkeletonLv2UnitSpellCard()
            : base(typeof(SkeletonLv2BattleUnit), 5)
        {
        }
    }
    public class SummonZombieLv2UnitSpellCard : SummonUnitSpellCard
    {
        public SummonZombieLv2UnitSpellCard()
            : base(typeof(ZombieLv2BattleUnit), 5)
        {
        }
    }
    public class SummonBansheeLv2UnitSpellCard : SummonUnitSpellCard
    {
        public SummonBansheeLv2UnitSpellCard()
            : base(typeof(BansheeLv2BattleUnit), 5)
        {
        }
    }
    public class SummonSpectreLv2UnitSpellCard : SummonUnitSpellCard
    {
        public SummonSpectreLv2UnitSpellCard()
            : base(typeof(SpectreLv2BattleUnit), 5)
        {
        }
    }

    #endregion UnitCards


    public class AddDefenseSpellCard : SpellCard
    {
        protected int mCost;

        public override string Name
        {
            get { return string.Format("Defense +{0} for {1} turn(s)", DefenseInc, Duration); }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return mCost; }
        }

        public int Duration { get; private set; }
        public int DefenseInc { get; private set; }

        public AddDefenseSpellCard(int cost, int duration, int defenseInc)
        {
            mCost = cost;
            Duration = duration;
            DefenseInc = defenseInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player == tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            tile.Unit.AddSpellEffect(turnNum, new ConstIncDefenseBattleUnitSpellEffect(DefenseInc, Duration));
        }
    }
    
    public class AddAttackSpellCard : SpellCard
    {
        protected int mCost;

        public override string Name
        {
            get { return string.Format("Attack +{0} for {1} turn(s)", AttackInc, Duration); }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return mCost; }
        }

        public int Duration { get; private set; }
        public int AttackInc { get; private set; }

        public AddAttackSpellCard(int cost, int duration, int attackInc)
        {
            mCost = cost;
            Duration = duration;
            AttackInc = attackInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player == tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            tile.Unit.AddSpellEffect(turnNum, new ConstIncAttackBattleUnitSpellEffect(AttackInc, Duration));
        }
    }
    
    public class AddMovementDistanceSpellCard : SpellCard
    {
        protected int mCost;

        public override string Name
        {
            get { return string.Format("Movement +{0} for {1} turn(s)", MovementDistanceInc, Duration); }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return mCost; }
        }

        public int Duration { get; private set; }
        public int MovementDistanceInc { get; private set; }

        public AddMovementDistanceSpellCard(int cost, int duration, int movementDistanceInc)
        {
            mCost = cost;
            Duration = duration;
            MovementDistanceInc = movementDistanceInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player == tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            tile.Unit.AddSpellEffect(turnNum, new ConstIncMovementDistanceBattleUnitSpellEffect(MovementDistanceInc, Duration));
        }
    }
}
