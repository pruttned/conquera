using System;
using Ale.Gui;

namespace Conquera.Gui
{
    public class VillageInfoView : TileInfoView
    {
        public VillageInfoView()
        {
            Setup("Village", GuiManager.Instance.Palette.CreateGraphicElement("TileIconCastle"), "This village can increase your unit cap.");
        }

        public override void Update(HexCell cell)
        {
        }
    }
}