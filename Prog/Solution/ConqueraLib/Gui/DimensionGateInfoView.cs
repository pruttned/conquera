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

using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Gui;

namespace Conquera.Gui
{
    public class DimensionGateInfoView : TileInfoView
    {
        private SpellSlot mCardSlot1 = new SpellSlot();
        private SpellSlot mCardSlot2 = new SpellSlot();
        private SpellSlot mCardSlot3 = new SpellSlot();

        public DimensionGateInfoView()
        {
            InitializeCardSlot(mCardSlot1, GuiManager.Instance.Palette.CreateRectangle("DimensionGateCardSlot1").Location);
            InitializeCardSlot(mCardSlot2, GuiManager.Instance.Palette.CreateRectangle("DimensionGateCardSlot2").Location);
            InitializeCardSlot(mCardSlot3, GuiManager.Instance.Palette.CreateRectangle("DimensionGateCardSlot3").Location);

            Setup("Dimension Gate", GuiManager.Instance.Palette.CreateGraphicElement("TileIconDimensionGate"), "This is a dimension gate. It is very important and it produces cards, and so on and so on. The Emperor be praised!");
        }

        public override void Update(HexCell cell)
        {
            //TODO: get cards from descriptor
            GameCard card1 = new GameCard();
            card1.Name = "card1";
            card1.Description = "card1 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card1.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconCastle");
            //card1.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            mCardSlot1.Spell = card1;

            GameCard card2 = new GameCard();
            card2.Name = "card2";
            card2.Description = "card2 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card2.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconDimensionGate");
            //card2.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            mCardSlot2.Spell = card2;

            GameCard card3 = new GameCard();
            card3.Name = "card3";
            card3.Description = "card3 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card3.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconLand");
            //card3.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            mCardSlot3.Spell = card3;
            //END TODO
        }

        private void InitializeCardSlot(SpellSlot slot, Point locaton)
        {
            slot.Location = locaton;
            ChildControls.Add(slot);
        }
    }
}
