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

//todo: spell

namespace Conquera.Gui
{
    public class SpellSlot : Control
    {
        private SpellInfoDialog mDialog = new SpellInfoDialog();

        public Spell Spell { get; set; }

        //public override System.Drawing.SizeF Size
        //{
        //    get
        //    {
        //        if (Spell == null)
        //        {
        //            return System.Drawing.SizeF.Empty;
        //        }
        //        return Spell.Icon.Size;
        //    }
        //}

        //public SpellSlot()
        //{
        //    MouseEnter += new EventHandler<ControlEventArgs>(GameCardInfoSlot_MouseEnter);
        //    MouseLeave += new EventHandler<ControlEventArgs>(GameCardInfoSlot_MouseLeave);
        //}

        //protected override void OnDrawBackground()
        //{
        //    if (Spell != null)
        //    {
        //        Spell.Icon.Draw(ScreenLocation);
        //    }
        //}

        //private void GameCardInfoSlot_MouseEnter(object sender, ControlEventArgs e)
        //{
        //    if (Spell != null)
        //    {
        //        mDialog.SetGameCard(Spell);
        //        mDialog.Show(false);
        //    }
        //}

        //private void GameCardInfoSlot_MouseLeave(object sender, ControlEventArgs e)
        //{
        //    mDialog.Hide();
        //}
    }
}
