using System;
using Ale.Gui;

namespace Conquera.Gui
{
    public class LandInfoView : TileInfoView
    {
        public LandInfoView()
        {
            Setup("Land", GuiManager.Instance.Palette.CreateGraphicElement("TileIconCastle"), "This is yust an empty land.");
        }

        public override void Update(HexCell cell)
        {
        }
    }
}