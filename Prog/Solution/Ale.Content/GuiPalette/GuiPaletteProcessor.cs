using Microsoft.Xna.Framework.Content.Pipeline;

namespace Ale.Content
{
    [ContentProcessor(DisplayName = "Gui palette - Ale")]
    public class GuiPaletteProcessor : ContentProcessor<GuiPaletteContent, GuiPaletteContent>
    {
        public override GuiPaletteContent Process(GuiPaletteContent input, ContentProcessorContext context)
        {
            return input;
        }
    }
}