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

using Microsoft.Xna.Framework.Graphics;
using Ale.Gui;

namespace Conquera.Gui
{
    public class MegaDebugLabel : Control
    {
        private TextElement mTextElement;

        public string Text
        {
            get { return mTextElement.Text; }
            set { mTextElement.Text = value; }
        }

        public override System.Drawing.SizeF Size
        {
            get { return new System.Drawing.SizeF(800.0f, 600.0f); }
        }

        public MegaDebugLabel()
        {
            mTextElement = new TextElement(800, 600, GuiManager.Instance.GetGuiFont("SpriteFont1"), true, Color.White);
            Text = "Praise the Emperor! I am visible!";
        }

        protected override void OnDrawForeground()
        {
            mTextElement.Draw(ScreenLocation);
        }
    }
}
