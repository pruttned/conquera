using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Gui;

namespace Conquera.Gui
{
    public class DimensionGateInfoView : TileInfoView
    {
        private GameCardSlot mCardSlot1 = new GameCardSlot();
        private GameCardSlot mCardSlot2 = new GameCardSlot();
        private GameCardSlot mCardSlot3 = new GameCardSlot();

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
            mCardSlot1.Card = card1;

            GameCard card2 = new GameCard();
            card2.Name = "card2";
            card2.Description = "card2 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card2.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconDimensionGate");
            //card2.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            mCardSlot2.Card = card2;

            GameCard card3 = new GameCard();
            card3.Name = "card3";
            card3.Description = "card3 description  - tralala tsdlakjf hslkdjh fklsajdhf lkjsdhf lkjsdhf lkjdhfkljdlfkjhsdl kfjhdskljfh dskjfh d";
            card3.Icon = GuiManager.Instance.Palette.CreateGraphicElement("TileIconLand");
            //card3.Picture = GuiManager.Instance.Palette.CreateGraphicElement("composite");
            mCardSlot3.Card = card3;
            //END TODO
        }

        private void InitializeCardSlot(GameCardSlot slot, Point locaton)
        {
            slot.Location = locaton;
            ChildControls.Add(slot);
        }
    }
}