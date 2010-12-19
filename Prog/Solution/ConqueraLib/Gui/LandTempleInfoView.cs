using System;
using Ale.Gui;

namespace Conquera.Gui
{
    public class LandTempleInfoView : TileInfoView
    {
        public LandTempleInfoView()
        {
            Setup("Land Temple", GuiManager.Instance.Palette.CreateGraphicElement("TileIconCastle"), "This is a land temple.");
        }

        public override void Update(HexCell cell)
        {
        }
    }
}