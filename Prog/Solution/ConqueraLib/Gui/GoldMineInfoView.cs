using System;
using Ale.Gui;

namespace Conquera.Gui
{
    public class GoldMineInfoView : TileInfoView
    {
        public GoldMineInfoView()
        {
            Setup("Gold Mine", GuiManager.Instance.Palette.CreateGraphicElement("TileIconCastle"), "This is a gold mine. It produces gold.");
        }

        public override void Update(HexCell cell)
        {
        }
    }
}