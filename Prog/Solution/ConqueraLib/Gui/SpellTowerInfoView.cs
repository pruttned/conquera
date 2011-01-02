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

using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Gui;

namespace Conquera.Gui
{
    public class SpellTowerInfoView : TileInfoView
    {
        private SpellView mSpellView = new SpellView();

        public SpellTowerInfoView()
        {
            mSpellView.Location = new Point(0, 100);
            ChildControls.Add(mSpellView);
        }

        public override void Update(HexCell cell)
        {
            base.Update(cell);
            mSpellView.Spell = ((SpellTowerTileDesc)cell.HexTerrainTile).Spell;
        }
    }
}