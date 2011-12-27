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

        public abstract void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players);

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

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
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
        private string mDescription;

        public override string Name
        {
            get { return string.Format("Summon {0}", mUnitCtor.DeclaringType.Name); }
        }

        public override string Description
        {
            get
            {
                return mDescription;
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

            //desription
            BattleUnit unit = (BattleUnit)Activator.CreateInstance(unitType);
            mDescription = string.Format("Attack: {0}\nDefense: {1}\nMovement: {2}", unit.BaseAttack, unit.BaseDefense, unit.BaseMovementDistance);
            

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

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            if (null == player) throw new ArgumentNullException("player");
            if (null == tile) throw new ArgumentNullException("tile");


            //todo: cast on existing unit

            mUnitCtor.Invoke(new object[] { player, terrain, tile.Index });
        }
    }




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


    public class IncDecDefenseSpellCard : SpellCard
    {
        private int mCost;

        public override string Name
        {
            get { return string.Format("Defense {0}{1} for {2} turn(s)", 0 < DefenseInc ? "+" : null, DefenseInc, Duration); }
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

        public IncDecDefenseSpellCard(int cost, int duration, int defenseInc)
        {
            mCost = cost;
            Duration = duration;
            DefenseInc = defenseInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && (player == tile.Unit.Player && 0 < DefenseInc || player != tile.Unit.Player && 0 > DefenseInc));
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.AddSpellEffect(turnNum, new ConstIncDefenseBattleUnitSpellEffect(DefenseInc, Duration));
        }
    }
    
    public class IncDecAttackSpellCard : SpellCard
    {
        private int mCost;

        public override string Name
        {
            get { return string.Format("Attack {0}{1} for {2} turn(s)", 0 < AttackInc ? "+" : null, AttackInc, Duration); }
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

        public IncDecAttackSpellCard(int cost, int duration, int attackInc)
        {
            mCost = cost;
            Duration = duration;
            AttackInc = attackInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && (player == tile.Unit.Player && 0 < AttackInc || player != tile.Unit.Player && 0 > AttackInc));
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.AddSpellEffect(turnNum, new ConstIncAttackBattleUnitSpellEffect(AttackInc, Duration));
        }
    }
    
    public class IncDecMovementDistanceSpellCard : SpellCard
    {
        private int mCost;

        public override string Name
        {
            get { return string.Format("Movement {0}{1} for {2} turn(s)", 0 < MovementDistanceInc ? "+" : null, MovementDistanceInc, Duration); }
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

        public IncDecMovementDistanceSpellCard(int cost, int duration, int movementDistanceInc)
        {
            mCost = cost;
            Duration = duration;
            MovementDistanceInc = movementDistanceInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && (player == tile.Unit.Player && 0 < MovementDistanceInc || player != tile.Unit.Player && 0 > MovementDistanceInc));
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.AddSpellEffect(turnNum, new ConstIncMovementDistanceBattleUnitSpellEffect(MovementDistanceInc, Duration));
        }
    }


    public class DiscardCardsSpellCard : SpellCard
    {
        private int mCost;
        private int mCardNumber;

        public override string Name
        {
            get { return string.Format("Oponents discards {0} random cards", mCardNumber); }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return mCost; }
        }

        public DiscardCardsSpellCard(int cost, int cardNumber)
        {
            mCost = cost;
            mCardNumber = cardNumber;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return true;
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            foreach (var p in players)
            {
                if (p != player)
                {
                    for(int i =0; i < mCardNumber && 0 < p.CardsInHand.Count ; ++i)
                    {
                        p.CardsInHand.RemoveAt(MathExt.Random.Next(p.CardsInHand.Count));
                    }
                }

            }
        }
    }


    public class DisableMovementSpellCard : SpellCard
    {
        int mCost;

        public override string Name
        {
            get { return string.Format("Disable movement for {0} turns", Duration); }
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

        public DisableMovementSpellCard(int cost, int duration)
        {
            mCost = cost;
            Duration = duration;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player != tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.AddSpellEffect(turnNum, new DisableMovementBattleUnitSpellEffect(Duration+1));
        }
    }

    public class DisableAttackSpellCard : SpellCard
    {
        int mCost;

        public override string Name
        {
            get { return string.Format("Disable attack for {0} turns", Duration); }
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

        public DisableAttackSpellCard(int cost, int duration)
        {
            mCost = cost;
            Duration = duration;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player != tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.AddSpellEffect(turnNum, new DisableAttackBattleUnitSpellEffect(Duration));
        }
    }

    public class HealSpellCard : SpellCard
    {
        int mCost;

        public override string Name
        {
            get { return string.Format("Heals {0} HP", HpInc); }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return mCost; }
        }

        public int HpInc { get; private set; }

        public HealSpellCard(int cost, int hpInc)
        {
            mCost = cost;
            HpInc = hpInc;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player == tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.Hp += HpInc;
        }
    }

    public class DamageSpellCard : SpellCard
    {
        int mCost;

        public override string Name
        {
            get { return string.Format("Removes {0} HP", HpDec); }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return mCost; }
        }

        public int HpDec { get; private set; }

        public DamageSpellCard(int cost, int hpDec)
        {
            mCost = cost;
            HpDec = hpDec;
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player != tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.Hp -= HpDec;
        }
    }

    public class RemoveDisableMovementsSpellCard : SpellCard
    {
        public override string Name
        {
            get { return "Removes all disable movements"; }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return 3; }
        }

        public int HpDec { get; private set; }

        public RemoveDisableMovementsSpellCard()
        {
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player == tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.RemoveSpellEffects(e => e is DisableMovementBattleUnitSpellEffect);
        }
    }

    public class RemoveDisableAttacksSpellCard : SpellCard
    {
        public override string Name
        {
            get { return "Removes all disable attacks"; }
        }

        public override string Description
        {
            get { return Name; }
        }

        public override int Cost
        {
            get { return 4; }
        }

        public int HpDec { get; private set; }

        public RemoveDisableAttacksSpellCard()
        {
        }

        public override bool IsValidTarget(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            return (null != tile.Unit && player == tile.Unit.Player);
        }

        public override void Cast(int turnNum, BattlePlayer player, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
        {
            tile.Unit.RemoveSpellEffects(e => e is DisableAttackBattleUnitSpellEffect);
        }
    }
}
