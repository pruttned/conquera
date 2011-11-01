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
using System.Windows.Media;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Conquera.BattlePrototype
{
    public class BattlePlayer
    {
        public const int MaxCardsInHandCnt = 7;

        private int mMana;
        private int mMaxMana;

        public Color Color { get; private set; }
        public int Index { get; private set; }

        /// <summary>
        /// Also clamps to 0,MaxMana interval
        /// </summary>
        public int Mana
        {
            get { return mMana; }
            set { mMana = MathExt.Clamp(value, 0, MaxMana);}
        }

        public int MaxMana
        {
            get { return mMaxMana; }
            set 
            { 
                mMaxMana = value;
                Mana = Mana; //clamp
            }
        }

        public List<SpellCard> CardDeck { get; set; }
        
        public List<SpellCard> CardsInHand { get; set; }

        public List<BattleUnit> Units { get; private set; }

        public BattlePlayer(Color color, int index)
        {
            Color = color;
            Index = index;

            Units = new List<BattleUnit>();
            CardDeck = new List<SpellCard>();
            CardsInHand = new List<SpellCard>();
        }

        public void CastSpellCard(int indexInHand, HexTerrainTile tile, HexTerrain terrain)
        {
            if (null == tile) throw new ArgumentNullException("tile");
            if (null == terrain) throw new ArgumentNullException("terrain");

            var card = CardsInHand[indexInHand];
            if (Mana < card.Cost)
            {
                throw new ArgumentException("Not enough mana for a specified card");
            }
            Mana -= card.Cost;
            CardsInHand.RemoveAt(indexInHand);
            card.Cast(this, tile, terrain);
        }

        public void OnTurnStart(int turnNum)
        {
            foreach (var unit in Units)
            {
                unit.HasMovedThisTurn = false;
            }

            while (CardsInHand.Count < MaxCardsInHandCnt)
            {
                CardsInHand.Add(CardDeck[MathExt.Random.Next(CardDeck.Count)]);
            }
        }
    }
}
