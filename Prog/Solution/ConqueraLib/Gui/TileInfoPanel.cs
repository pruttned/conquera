using System.Collections.Generic;
using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Gui;

namespace Conquera.Gui
{
    public class TileInfoPanel : ContentControl
    {
        private Dictionary<string, TileInfoView> mTileInfoViews = new Dictionary<string, TileInfoView>();
        private GraphicElement mBackground;
        private Rectangle mTileInfoViewRectangle;

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public TileInfoPanel()
        {
            mBackground = GuiManager.Instance.Palette.CreateGraphicElement("TileInfoPanelBackground");
            mTileInfoViewRectangle = GuiManager.Instance.Palette.CreateRectangle("TileInfoViewBounds");

            RegisterView("DimensionGate", new DimensionGateInfoView());
            RegisterView("Castle", new CastleInfoView());
            RegisterView("GoldMine", new GoldMineInfoView());
            RegisterView("Land", new LandInfoView());
            RegisterView("Village", new VillageInfoView());
            RegisterView("LandTemple", new LandTempleInfoView());
        }

        public void Update(HexCell cell)
        {
            TileInfoView view = mTileInfoViews[cell.HexTerrainTile.InfoViewType];
            view.Update(cell);
            Content = view;
        }

        protected override void OnDrawBackground()
        {
            mBackground.Draw(ScreenLocation);
        }

        private void RegisterView(string name, TileInfoView view)
        {
            view.Location = mTileInfoViewRectangle.Location;
            view.SetSize(new System.Drawing.SizeF(mTileInfoViewRectangle.Width, mTileInfoViewRectangle.Height));
            mTileInfoViews.Add(name, view);
        }
    }
}