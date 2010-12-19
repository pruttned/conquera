//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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

namespace Ale.Gui
{
    public class SimpleTextButton : SimpleControl
    {
        private TextElement mTextElement;

        public string Text
        {
            get { return mTextElement.Text; }
            set { mTextElement.Text = value; }
        }

        protected TextElement TextElement
        {
            get { return mTextElement; }
        }

        public SimpleTextButton(GuiFont font)
            :this(typeof(SimpleTextButton).Name, font)
        {
        }

        public SimpleTextButton(string text, GuiFont font)
            :this(text, font, Color.Black)
        {
        }

        public SimpleTextButton(string text, GuiFont font, Color color)
        {
            mTextElement = new TextElement(font, color);
            Text = text;
            MainGraphicElementRepository.Repositories.Add(new GraphicElementRepository(mTextElement));
        }
    }
}
