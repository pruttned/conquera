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
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    /// <summary>
    /// Represents a single 2D image.
    /// </summary>
    public class Image : GraphicElement
    {
        /// <summary>
        /// Bounds of the source image in 'SourceTexture'.
        /// </summary>
        private Rectangle mSourceRectangle;

        /// <summary>
        /// Source texture, containing the source image.
        /// </summary>
        private Texture2D mSourceTexture;

        /// <summary>
        /// Gets or sets the bounds of the source image in 'SourceTexture'.
        /// </summary>
        public Rectangle SourceRectangle
        {
            get { return mSourceRectangle; }
            set { mSourceRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the source texture, containing the source image.
        /// </summary>
        public Texture2D SourceTexture
        {
            get { return mSourceTexture; }
            set { mSourceTexture = value; }
        }

        public Color Color { get; set; }

        /// <summary>
        /// Constructs a new Image instance.
        /// </summary>
        /// <param name="sourceRectangle">Bounds of the source image in 'SourceTexture'.</param>
        /// <param name="sourceTexture">Source texture, containing the source image.</param>
        public Image(Rectangle sourceRectangle, Texture2D sourceTexture)
        {
            SourceRectangle = sourceRectangle;
            SourceTexture = sourceTexture;

            Color = Color.White;
        }

        #region GraphicElement

        /// <summary>
        /// Gets the size of this graphic element.
        /// </summary>
        public override System.Drawing.SizeF Size
        {
            get { return new System.Drawing.Size(SourceRectangle.Width, SourceRectangle.Height); }
        }

        /// <summary>
        /// Draws this graphic element.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw.</param>
        /// <param name="screenLocation">Destination location, where to draw.</param>
        /// <param name="gameTime">Current game time.</param>
        public override void Draw(SpriteBatch spriteBatch, Point screenLocation, AleGameTime gameTime)
        {
            spriteBatch.Draw(SourceTexture, new Vector2((float)screenLocation.X, (float)screenLocation.Y), SourceRectangle, Color);
        }

        #endregion GraphicElement
    }
}
