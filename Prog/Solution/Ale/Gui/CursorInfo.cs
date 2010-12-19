using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    public class CursorInfo
    {
        public GraphicElement GraphicElement { get; set; }
        public Point HotSpot { get; set; }

        public CursorInfo(GraphicElement graphicElement, Point hotSpot)
        {
            GraphicElement = graphicElement;
            HotSpot = hotSpot;
        }
    }
}
