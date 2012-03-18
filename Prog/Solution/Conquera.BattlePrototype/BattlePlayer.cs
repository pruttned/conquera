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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Conquera.BattlePrototype
{
    public class BattlePlayer : INotifyPropertyChanged
    {
        static int MaxCardCntInHand = 4;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ManaChanged;

        public const int MaxCardsInHandCnt = 7;
        private SpellCardDeck mCardDeck = null;

        private int mMana;
        private int mMaxMana;

        public Color Color { get; private set; }
        public int Index { get; private set; }

        public Point StartPos { get; set; }

        /// <summary>
        /// Also clamps to 0,MaxMana interval
        /// </summary>
        public int Mana
        {
            get { return mMana; }
            set
            {
                mMana = MathExt.Clamp(value, 0, MaxMana);
                EventHelper.RaiseEvent(ManaChanged, this);
                EventHelper.RaisePropertyChanged(PropertyChanged, this, "Mana");
            }
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

        public bool IsActive { get; set; }

        public SpellCardDeck CardDeck 
        {
            get 
            { 
              //  if(null == mCardDeck) throw new InvalidOperationException("Card deck has not yet been initialized");
                return mCardDeck; 

            }
            set 
            { 
                mCardDeck = value;
                FillCardsInHand();
            }
        }
        public Window1 Window { get; private set; }
        public ObservableCollection<SpellCard> CardsInHand { get; set; }

        public SafeModifiableIterableCollection<BattleUnit> Units { get; private set; }

        public BattlePlayer( Window1 window, Color color, int index)
        {
            Window = window;
            Color = color;
            Index = index;

            Units = new SafeModifiableIterableCollection<BattleUnit>();
            CardsInHand = new ObservableCollection<SpellCard>();
        }

        public void CastSpellCard(int turnNum, int indexInHand, HexTerrainTile tile, HexTerrain terrain, IList<BattlePlayer> players)
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
            card.Cast(turnNum, this, tile, terrain, players);

            Logger.Log(card.Name, tile);
        }


        public void DiscardSpellCard(int turnNum, int indexInHand)
        {
            var card = CardsInHand[indexInHand];
            CardsInHand.RemoveAt(indexInHand);
            card.Discard(turnNum, this);
        }

        public void OnTurnStart(int turnNum, bool isActive)
        {
            foreach (var unit in Units)
            {
                unit.HasMovedThisTurn = false;
            }

            foreach (var unit in Units)
            {
                unit.OnTurnStart(turnNum);
            }
            Units.Tidy();

            if (isActive && !CardDeck.IsEmpty && MaxCardCntInHand > CardsInHand.Count )
            {
                CardsInHand.Add(CardDeck.PopCard());
            }
            //FillCardsInHand();
            //if (isActive)
            //{
            //    AddRandomCard();
            //}

//            Mana++;

            OnTurnStartImpl(turnNum, isActive);
        }

        public void FillCardsInHand()
        {
            CardsInHand.Clear();
            while (!CardDeck.IsEmpty && MaxCardCntInHand > CardsInHand.Count)
            {
                CardsInHand.Add(CardDeck.PopCard());
            }
            //if (CardDeck.Count > 0)
            //{
            //    while (CardsInHand.Count < MaxCardsInHandCnt)
            //    {
            //        CardsInHand.Add(CardDeck[MathExt.Random.Next(CardDeck.Count)]);
            //    }
            //}

            //foreach (var card in CardDeck)
            //{
            //    CardsInHand.Add(card);
            //}

        }

        //private void AddRandomCard()
        //{
        //    if (CardDeck.Count > 0)
        //    {
        //        CardsInHand.Add(CardDeck[MathExt.Random.Next(CardDeck.Count)]);
        //    }
        //}

        protected virtual void OnTurnStartImpl(int turnNum, bool isActive) { }
    }
}
