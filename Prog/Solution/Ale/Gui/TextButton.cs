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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    public class TextButton : GraphicButton
    {
        private TextElement mTextElement;

        public string Text
        {
            get { return mTextElement.Text; }
            set { mTextElement.Text = value; }
        }

        public TextButton(GraphicElement defaultGraphicElement, GraphicElement mouseOverGraphicElement, GuiFont font, Color textColor, string text)
            : this(defaultGraphicElement, mouseOverGraphicElement, null, font, textColor, text)
        {
        }

        public TextButton(GraphicElement defaultGraphicElement, GraphicElement mouseOverGraphicElement, GraphicElement disabledGraphicElement,
            GuiFont font, Color textColor, string text)
            : base(defaultGraphicElement, mouseOverGraphicElement, disabledGraphicElement)
        {
            mTextElement = new TextElement(font, textColor);
            mTextElement.Text = text;
        }

        protected override void OnDrawForeground()
        {
            Point location = new Point((int)(ScreenLocation.X + Size.Width / 2 - mTextElement.Width / 2),
                                       (int)(ScreenLocation.Y + Size.Height / 2 - mTextElement.Height / 2));
            mTextElement.Draw(location);
        }
    }
}
