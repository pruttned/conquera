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

}
