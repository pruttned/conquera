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
    public class ImageDrawing
    {
        private Texture2D mSourceTexture;
        private Rectangle mSourceRectangle;

        public Color Color { get; set; }
        
        public int Width
        {
            get { return mSourceRectangle.Width; }
        }

        public int Height
        {
            get { return mSourceRectangle.Height; }
        }

        public ImageDrawing (Texture2D sourceTexture, Rectangle sourceRectangle)
            :this(sourceTexture, sourceRectangle, Color.White)
	    {
	    }

        public ImageDrawing (Texture2D sourceTexture, Rectangle sourceRectangle, Color color)
        {
            mSourceTexture = sourceTexture;
            mSourceRectangle = sourceRectangle;
            Color = color;
        }

        public void Draw(SpriteBatch spriteBatch, Point positionOnScreen)
        {
            Draw(spriteBatch, GuiHelper.ConvertPointToVector(positionOnScreen));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 positionOnScreen)
        {
            spriteBatch.Draw(mSourceTexture, positionOnScreen, mSourceRectangle, Color);
        }
    }
}