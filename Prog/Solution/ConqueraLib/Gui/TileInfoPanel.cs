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

using System.Collections.Generic;
using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Gui;
using System;

namespace Conquera.Gui
{
    public class TileInfoPanel : ContentControl
    {
        private TileInfoView mDefaultTileInfoView = new TileInfoView();
        private Dictionary<Type, TileInfoView> mDerivedTileInfoViews = new Dictionary<Type, TileInfoView>();
        private GraphicElement mBackground;        

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public TileInfoPanel()
        {
            mBackground = ConqueraPalette.TileInfoPanelBackground;            

            InitializeView(mDefaultTileInfoView);
            RegisterDerivedView(typeof(SpellTowerTileDesc), new SpellTowerInfoView());
            RegisterDerivedView(typeof(CastleTileDesc), new CastleInfoView());
        }

        public void Update(HexCell cell)
        {
            TileInfoView view;
            if (!mDerivedTileInfoViews.TryGetValue(cell.HexTerrainTile.GetType(), out view))
            {
                view = mDefaultTileInfoView;
            }

            view.Update(cell);
            Content = view;
        }

        protected override void OnDrawBackground()
        {
            mBackground.Draw(ScreenLocation);
        }

        private void RegisterDerivedView(Type tileDescriptorType, TileInfoView view)
        {
            InitializeView(view);
            mDerivedTileInfoViews.Add(tileDescriptorType, view);
        }

        private void InitializeView(TileInfoView view)
        {
            view.Location = ConqueraPalette.TileInfoViewLocation;
            view.SetSize(ConqueraPalette.TileInfoViewSize);
        }
    }
}
