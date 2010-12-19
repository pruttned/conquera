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
        private int mLineHeight;

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

        public int LineHeight
        {
            get { return mLineHeight; }
        }

        public GuiFont(SpriteFont innerFont)
        {
            mInnerFont = innerFont;

            Vector2 spaceSize = InnerFont.MeasureString(" ");
            mSpaceWidth = (int)spaceSize.X;
            mLineHeight = (int)spaceSize.Y;

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