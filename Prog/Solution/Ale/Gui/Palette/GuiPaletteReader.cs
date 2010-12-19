using Ale.Gui;
using Microsoft.Xna.Framework.Content;

namespace Ale.Content
{
    public class GuiPaletteReader : ContentTypeReader<Palette>
    {
        protected override Palette Read(ContentReader input, Palette existingInstance)
        {
            return new Palette(input);
        }
    }
}
