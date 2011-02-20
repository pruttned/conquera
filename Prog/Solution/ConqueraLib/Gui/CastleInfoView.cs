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

using System;
using Ale.Gui;

namespace Conquera.Gui
{
    public class CastleInfoView : TileInfoView
    {
        private ConqueraTextButton mBuyUnitButton;
        private HexCell mCell = null;

        public CastleInfoView()
        {
            mBuyUnitButton = new ConqueraTextButton("Buy1");
            mBuyUnitButton.Location = ConqueraPalette.CastleBuyUnit1ButtonLocation;
            mBuyUnitButton.Click += new EventHandler<ControlEventArgs>(mBuyUnitButton_Click);
            ChildControls.Add(mBuyUnitButton);
        }

        public override void Update(HexCell cell)
        {
            base.Update(cell);

            mCell = cell;
            mBuyUnitButton.Visible = mCell.OwningPlayer == cell.Scene.CurrentPlayer && mCell.GameUnit == null &&
                mCell.OwningPlayer.HasEnoughGoldForUnit("GameUnit1") && mCell.OwningPlayer.Units.Count < mCell.OwningPlayer.MaxUnitCnt;
        }

        private void mBuyUnitButton_Click(object sender, ControlEventArgs e)
        {
            mBuyUnitButton.Visible = false;
            mCell.OwningPlayer.BuyUnit("GameUnit1", mCell.Index);
        }
    }
}
