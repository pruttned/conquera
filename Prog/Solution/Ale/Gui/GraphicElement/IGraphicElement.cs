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
    public delegate void GraphicElementEventHandler(GraphicElement graphicElement);

    /// <summary>
    /// Interface for all graphic elements (such as images and animations), which can be drawn and updated.
    /// </summary>
    public abstract class GraphicElement
    {
        /// <summary>
        /// Raises, when Size property changes.
        /// </summary>
        public event GraphicElementEventHandler SizeChanged;

        /// <summary>
        /// Gets the size of this graphic element.
        /// </summary>
        public abstract System.Drawing.SizeF Size { get; }

        /// <summary>
        /// Using spritebatch and gametime of the gui manager.
        /// </summary>
        /// <param name="screenLocation"></param>
        public void Draw(Point screenLocation)
        {
            Draw(GuiManager.Instance.SpriteBatch, screenLocation, GuiManager.Instance.GameTime);
        }

        /// <summary>
        /// Draws this graphic element.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw.</param>
        /// <param name="screenLocation">Destination location, where to draw.</param>
        /// <param name="gameTime">Current game time.</param>
        public abstract void Draw(SpriteBatch spriteBatch, Point screenLocation, AleGameTime gameTime);

        /// <summary>
        /// Activates this graphic element.
        /// </summary>
        public virtual void Activate() { }

        /// <summary>
        /// Deactivates this graphic element.
        /// </summary>
        public virtual void Deactivate() { }

        protected void RaiseSizeChanged()
        {
            if (SizeChanged != null)
            {
                SizeChanged(this);
            }
        }
    }
}
