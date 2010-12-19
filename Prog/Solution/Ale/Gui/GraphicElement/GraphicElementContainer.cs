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
