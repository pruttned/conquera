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
using Ale.Scene;
using Ale.Gui;

namespace Conquera.Gui
{
    public class GameCardSlot : Control
    {
        private GameCardInfoDialog mDialog = new GameCardInfoDialog();

        public GameCard Card { get; set; }

        public override System.Drawing.SizeF Size
        {
            get
            {
                if (Card == null)
                {
                    return System.Drawing.SizeF.Empty;
                }
                return Card.Icon.Size;
            }
        }

        public GameCardSlot()
        {
            MouseEnter += new EventHandler<ControlEventArgs>(GameCardInfoSlot_MouseEnter);
            MouseLeave += new EventHandler<ControlEventArgs>(GameCardInfoSlot_MouseLeave);
        }

        protected override void OnDrawBackground()
        {
            if (Card != null)
            {
                Card.Icon.Draw(ScreenLocation);
            }
        }

        private void GameCardInfoSlot_MouseEnter(object sender, ControlEventArgs e)
        {
            if (Card != null)
            {
                mDialog.SetGameCard(Card);
                mDialog.Show(false);
            }
        }

        private void GameCardInfoSlot_MouseLeave(object sender, ControlEventArgs e)
        {
            mDialog.Hide();
        }
    }
}
