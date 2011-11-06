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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera.Gui
{
    public class SpellSlotButton : Control
    {
        private SpellView mSpellView = new SpellView();
        private SpellSlot mSpellSlot = null;
        private TextElement mCountLabel;
        private GraphicElementContainer mCountLabelContainer;
        private BattleScene mGameScene;
        private GraphicElementContainer mIsActiveLabelContainer;

        public override System.Drawing.SizeF Size
        {
            get
            {
                //TODO: from palette
                return new System.Drawing.SizeF(70, 70);
            }
        }

        public SpellSlot SpellSlot
        {
            get { return mSpellSlot; }
            set
            {
                if (value != mSpellSlot)
                {
                    //if (mSpellSlot != null) //old
                    //{
                    //    mSpellSlot.TotalCountChanged -= mSpellSlot_TotalCountChanged;
                    //    mSpellSlot.AvailableCountChanged -= mSpellSlot_AvailableCountChanged;
                    //}

                    mSpellSlot = value;
                    mSpellView.Spell = mSpellSlot.Spell;

                    //if (mSpellSlot != null) //new
                    //{
                    //    mSpellSlot.TotalCountChanged += new EventHandler(mSpellSlot_TotalCountChanged);
                    //    mSpellSlot.AvailableCountChanged += new EventHandler(mSpellSlot_AvailableCountChanged);
                    //}

                    //UpdateCount();
                }
            }
        }

        public SpellSlotButton(BattleScene gameScene)
        {
            mGameScene = gameScene;

            mSpellView.Click += new EventHandler<ControlEventArgs>(mSpellView_Click);
            ChildControls.Add(mSpellView);

            mCountLabel = new TextElement(ConqueraFonts.SpriteFontSmall, Color.White);
            mCountLabelContainer = new GraphicElementContainer(mCountLabel, Point.Zero);

            TextElement isActiveLabel = new TextElement(ConqueraFonts.SpriteFontSmall, Color.White);
            isActiveLabel.Text = "[A]";
            mIsActiveLabelContainer = new GraphicElementContainer(isActiveLabel, new Point(30, 30));

            Click += new EventHandler<ControlEventArgs>(SpellSlotButton_Click);
        }

        public void Toggle()
        {
           // mGameScene.ActiveSpellSlot = mGameScene.ActiveSpellSlot != SpellSlot ? SpellSlot : null;
        }

        protected override void OnDrawForeground()
        {
            mCountLabelContainer.Draw(this);

            //if (SpellSlot == mGameScene.ActiveSpellSlot)
            //{
            //    mIsActiveLabelContainer.Draw(this);
            //}
        }

        //private void mSpellSlot_TotalCountChanged(object sender, EventArgs e)
        //{
        //    UpdateCount();
        //}

        //private void mSpellSlot_AvailableCountChanged(object sender, EventArgs e)
        //{
        //    UpdateCount();
        //}

        //private void UpdateCount()
        //{
        //    if (SpellSlot == null)
        //    {
        //        mCountLabel.Text = string.Empty;
        //    }
        //    else
        //    {
        //        mCountLabel.Text = string.Format("{0}/{1}", SpellSlot.AvailableCount, SpellSlot.TotalCount);
        //    }
        //}

        private void mSpellView_Click(object sender, ControlEventArgs e)
        {
            OnClick();
        }

        private void SpellSlotButton_Click(object sender, ControlEventArgs e)
        {
            Toggle();
        }
    }
}