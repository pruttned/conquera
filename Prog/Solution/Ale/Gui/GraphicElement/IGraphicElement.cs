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
