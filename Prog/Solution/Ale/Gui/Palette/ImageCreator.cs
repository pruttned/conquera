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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    /// <summary>
    /// Used by Palette to create concrete Image instances - parametrized factory.
    /// </summary>
    [GraphicElementCreator(0)]
    public class ImageCreator : IGraphicElementCreator
    {
        private Texture2D mSourceTexture;
        private Rectangle mSourceRectangle;

        public GraphicElement CreateGraphicElement()
        {
            return new Image(mSourceRectangle, mSourceTexture);
        }

        public void Initialize(ContentReader input, Palette palette)
        {
            mSourceTexture = palette.GetTexture(input.ReadString());
            mSourceRectangle = Palette.ReadRectangle(input);
        }
    }
}
