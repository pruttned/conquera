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