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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Ale.Gui
{
    public class GuiFont
    {
        private static Dictionary<SpriteFont, GuiFont> mGuiFontCache = new Dictionary<SpriteFont, GuiFont>();

        public SpriteFont InnerFont { get; private set; }
        public int MaxCharWidth { get; private set; }
        public int AverageCharWidth { get; private set; }
        public int SpaceWidth { get; private set; }
        public int FirstLineHeight { get; private set; }
        public int FirstLineMargin { get; private set; } //first line has larger height than others - this is that delta

        private GuiFont(SpriteFont innerFont)
        {
            InnerFont = innerFont;

            Vector2 spaceSize = InnerFont.MeasureString(" ");
            SpaceWidth = (int)spaceSize.X;
            FirstLineHeight = (int)spaceSize.Y;
            FirstLineMargin = FirstLineHeight - InnerFont.LineSpacing;

            int charWidth;
            AverageCharWidth = 0;
            MaxCharWidth = 0;
            foreach (char character in InnerFont.Characters)
            {
                charWidth = (int)InnerFont.MeasureString(character.ToString()).X;
                
                AverageCharWidth += charWidth;
                MaxCharWidth = Math.Max(MaxCharWidth, charWidth);
            }
            AverageCharWidth /= InnerFont.Characters.Count;
        }

        public static GuiFont Get(SpriteFont spriteFont)
        {
            GuiFont guiFont;
            if(!mGuiFontCache.TryGetValue(spriteFont, out guiFont))
            {
                guiFont = new GuiFont(spriteFont);
                mGuiFontCache.Add(spriteFont, guiFont);
            }
            return guiFont;
        }
    }
}