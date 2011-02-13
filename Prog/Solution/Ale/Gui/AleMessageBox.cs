﻿//////////////////////////////////////////////////////////////////////
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    public class AleMessageBox : Dialog
    {
        private GraphicElement mBackground;
        private TextElement mTextElement;

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public AleMessageBox(string text)
        {
            mBackground = GuiManager.Instance.Palette.CreateGraphicElement("MainMenuDialogBackground");

            mTextElement = new TextElement(300, 100, GuiManager.Instance.GetGuiFont("SpriteFont1"), true, Color.White);
            mTextElement.Text = text;

            TextButton closeButton = new TextButton(GuiManager.Instance.Palette.CreateGraphicElement("ShowMainMenuButtonDefault"),
                GuiManager.Instance.Palette.CreateGraphicElement("ShowMainMenuButtonMouseOver"), GuiManager.Instance.GetGuiFont("SpriteFont1"),
                Color.White, "OK");
            closeButton.Location = new Point(150, 150);
            closeButton.Click += new EventHandler<ControlEventArgs>(closeButton_Click);
            ChildControls.Add(closeButton);
        }

        protected override void OnDrawBackground()
        {
            mBackground.Draw(ScreenLocation);
            mTextElement.Draw(ScreenLocation);
        }

        private void closeButton_Click(object sender, ControlEventArgs e)
        {
            Hide();
        }
    }
}
