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

namespace Conquera.BattlePrototype
{
    public abstract class SpellCard
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract int Cost { get; }

        public abstract bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain);

        public abstract void Cast(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain);
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
            get { return string.Format("Adds +{0} mana", mManaInc); }
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

        public override void Cast(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            if (null == player) throw new ArgumentNullException("player");

            player.Mana += mManaInc;
        }
    }

    public class SummonUnitSpellCard : SpellCard
    {
        private static Type[] mUnitCtorArgTypes = new Type[] { typeof(BattlePlayer), typeof(HexTerrain), typeof(Point) };
        private ConstructorInfo mUnitCtor;
        protected int mCost;

        public override string Name
        {
            get { return string.Format("Summon {0}", mUnitCtor.DeclaringType.Name); }
        }

        public override string Description
        {
            get { return string.Format("Summons the {0} unit", mUnitCtor.DeclaringType.Name); }
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
            //todo: cast on existing unit
            return (null == tile.Unit && tile is OutpostHexTerrainTile);
        }

        public override void Cast(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            if (null == player) throw new ArgumentNullException("player");
            if (null == tile) throw new ArgumentNullException("tile");


            //todo: cast on existing unit

            mUnitCtor.Invoke(new object[] { player, terrain, tile });
        }
    }



    #region ManaCards

    public class Add2ManaCard : AddManaSpellCard
    {
        public Add2ManaCard()
            :base(2)
        {
        }
    }
    public class Add5ManaCard : AddManaSpellCard
    {
        public Add5ManaCard()
            : base(5)
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

}
