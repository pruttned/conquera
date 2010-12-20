//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
