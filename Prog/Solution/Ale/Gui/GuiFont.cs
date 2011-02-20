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

namespace Ale.Gui
{
    public class GuiFont
    {
        private SpriteFont mInnerFont;
        private int mMaxCharWidth;
        private int mAverageCharWidth;
        private int mSpaceWidth;
        private int mFirstLineHeight;

        public SpriteFont InnerFont
        {
            get { return mInnerFont; }
        }

        public int MaxCharWidth
        {
            get { return mMaxCharWidth; }
        }

        public int AverageCharWidth
        {
            get { return mAverageCharWidth; }
        }

        public int SpaceWidth
        {
            get { return mSpaceWidth; }
        }

        public int FirstLineHeight
        {
            get { return mFirstLineHeight; }
        }

        public int FirstLineMargin { get; private set; } //first line has larger height than others - this is that delta

        public GuiFont(SpriteFont innerFont)
        {
            mInnerFont = innerFont;

            Vector2 spaceSize = InnerFont.MeasureString(" ");
            mSpaceWidth = (int)spaceSize.X;
            mFirstLineHeight = (int)spaceSize.Y;
            FirstLineMargin = mFirstLineHeight - InnerFont.LineSpacing;

            int charWidth;
            mAverageCharWidth = 0;
            mMaxCharWidth = 0;
            foreach (char character in InnerFont.Characters)
            {
                charWidth = (int)InnerFont.MeasureString(character.ToString()).X;
                
                mAverageCharWidth += charWidth;
                mMaxCharWidth = Math.Max(mMaxCharWidth, charWidth);
            }
            mAverageCharWidth /= InnerFont.Characters.Count;
        }
    }
}
