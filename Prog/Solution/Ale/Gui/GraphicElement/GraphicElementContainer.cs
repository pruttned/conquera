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
    public class GraphicElementContainer
    {
        public GraphicElement GraphicElement { get; set; }
        public Point Location { get; set; }

        public GraphicElementContainer(GraphicElement graphicElement, Point location)
        {
            GraphicElement = graphicElement;
            Location = location;
        }

        public void Draw(Control ownerControl)
        {
            if (GraphicElement != null)
            {
                Point screenLocation = new Point(ownerControl.ScreenLocation.X + Location.X, ownerControl.ScreenLocation.Y + Location.Y);
                GraphicElement.Draw(screenLocation);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Control ownerControl, AleGameTime gameTime)
        {
            if (GraphicElement != null)
            {
                Point screenLocation = new Point(ownerControl.ScreenLocation.X + Location.X, ownerControl.ScreenLocation.Y + Location.Y);
                GraphicElement.Draw(spriteBatch, screenLocation, gameTime);
            }
        }
    }
}
