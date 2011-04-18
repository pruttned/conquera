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
        public System.Drawing.SizeF Size { get; set; } //if not empty, graphic element is center-aligned; otherwise it is topLeft-aligned

        public GraphicElementContainer(GraphicElement graphicElement, Rectangle rectangle)
            :this(graphicElement, rectangle.Location, new System.Drawing.SizeF(rectangle.Width, rectangle.Height))
        {
        }

        public GraphicElementContainer(GraphicElement graphicElement, Point location)
            :this(graphicElement, location, System.Drawing.SizeF.Empty)
        {
        }

        public GraphicElementContainer(GraphicElement graphicElement, Point location, System.Drawing.SizeF size)
        {
            GraphicElement = graphicElement;
            Location = location;
            Size = size;
        }

        public void Draw(Control ownerControl)
        {
            if (GraphicElement != null)
            {
                GraphicElement.Draw(GetGraphicElementScreenLocation(ownerControl));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Control ownerControl, AleGameTime gameTime)
        {
            if (GraphicElement != null)
            {                
                GraphicElement.Draw(spriteBatch, GetGraphicElementScreenLocation(ownerControl), gameTime);
            }
        }

        public Point GetGraphicElementScreenLocation(Control ownerControl)
        {
            int containerLeft = ownerControl.ScreenLocation.X + Location.X;
            int containerTop = ownerControl.ScreenLocation.Y + Location.Y;

            if (!Size.IsEmpty) //center-aligned
            {
                return new Point(containerLeft + (int)(Size.Width / 2 - GraphicElement.Size.Width / 2),
                                 containerTop + (int)(Size.Height / 2 - GraphicElement.Size.Height / 2));
            }
            else //topLeft-aligned
            {
                return new Point(containerLeft, containerTop);
            }
        }
    }
}
