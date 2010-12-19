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
            mBuyHeroButton.Visible = mCell.OwningPlayer == cell.Scene.CurrentPlayer && mCell.GameUnit == null && mCell.OwningPlayer.HasEnoughGoldForUnit("GameUnit1");
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