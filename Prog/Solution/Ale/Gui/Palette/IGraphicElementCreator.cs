using Microsoft.Xna.Framework.Content;

namespace Ale.Gui
{
    /// <summary>
    /// Interface for all factories, which Palette uses to create graphic elements.
    /// </summary>
    public interface IGraphicElementCreator
    {
        /// <summary>
        /// Creates a graphic element.
        /// </summary>
        /// <returns></returns>
        GraphicElement CreateGraphicElement();

        /// <summary>
        /// Called before any 'CreateGraphicElement' call.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="palette"></param>
        void Initialize(ContentReader input, Palette palette);
    }
}
