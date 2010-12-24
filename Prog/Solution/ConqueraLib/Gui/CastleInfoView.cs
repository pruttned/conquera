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
using Ale.Gui;

namespace Conquera.Gui
{
    public class CastleInfoView : TileInfoView
    {
        private GraphicButton mBuyHeroButton;
        private HexCell mCell = null;

        public CastleInfoView()
        {
            Setup("Castle", GuiManager.Instance.Palette.CreateGraphicElement("TileIconCastle"), "This is a castle. Here you can buy heroes and give them cards. And so on, and os on...");

            mBuyHeroButton = new GraphicButton(GuiManager.Instance.Palette.CreateGraphicElement("CastleBuyHeroDefault"),
                                                             GuiManager.Instance.Palette.CreateGraphicElement("CastleBuyHeroMouseOver"));
            mBuyHeroButton.Location = GuiManager.Instance.Palette.CreateRectangle("CastleBuyHeroButton").Location;
            mBuyHeroButton.Click += new EventHandler<ControlEventArgs>(mBuyHeroButton_Click);
            ChildControls.Add(mBuyHeroButton);
        }

        public override void Update(HexCell cell)
        {
            mCell = cell;
            mBuyHeroButton.Visible = mCell.OwningPlayer == cell.Scene.CurrentPlayer && mCell.GameUnit == null &&
                mCell.OwningPlayer.HasEnoughGoldForUnit("GameUnit1") && mCell.OwningPlayer.Units.Count < mCell.OwningPlayer.MaxUnitCnt;
        }

        private void mBuyHeroButton_Click(object sender, ControlEventArgs e)
        {
            mBuyHeroButton.Visible = false;
            GameUnit hero = mCell.OwningPlayer.BuyUnit("GameUnit1", mCell.Index);

            //TODO: VYHODIT
            GameCard card1 = new GameCard();
            card1.Name = "card1";
            card1.Description = "card1 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card1.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconCastle");
            card1.AttackPurple = 1;
            card1.DefensePurple = 2;
            //card1.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            hero.AddCard(card1);

            GameCard card2 = new GameCard();
            card2.Name = "card2";
            card2.Description = "card2 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card2.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconDimensionGate");
            //card2.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            hero.AddCard(card2);

            GameCard card3 = new GameCard();
            card3.Name = "card3";
            card3.Description = "card3 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card3.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconLand");
            //card3.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            hero.AddCard(card3);
            //END TODO
        }
    }
}
