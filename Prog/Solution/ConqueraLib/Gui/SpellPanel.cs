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

using Ale.Gui;
using Microsoft.Xna.Framework;

namespace Conquera.Gui
{
    public class SpellPanel : Control
    {
        private SpellSlotButton[] mButtons;
        private SpellSlotCollection mSpellSlotCollection;

        public SpellSlotCollection SpellSlotCollection
        {
            get { return mSpellSlotCollection; }
            set
            {
                if (value != mSpellSlotCollection)
                {
                    mSpellSlotCollection = value;

                    if (mSpellSlotCollection == null)
                    {
                        for (int i = 0; i < mButtons.Length; i++)
                        {
                            mButtons[i].SpellSlot = null;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < mButtons.Length; i++)
                        {
                            mButtons[i].SpellSlot = mSpellSlotCollection[i];
                        }
                    }
                }
            }
        }

        public override System.Drawing.SizeF Size
        {
            get
            {
                //TODO: from palette
                return new System.Drawing.SizeF(800, 70);
            }
        }

        public SpellPanel(GameScene gameScene)
        {
            mButtons = new SpellSlotButton[SpellSlotCollection.Spells.Length];
            for (int i = 0; i < mButtons.Length; i++)
            {
                mButtons[i] = new SpellSlotButton(gameScene);
                mButtons[i].Location = new Point(i * 72, 0);
                ChildControls.Add(mButtons[i]);
            }
        }
    }
}