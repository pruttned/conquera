using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    /// <summary>
    /// Represents a single 2D image.
    /// </summary>
    public class Image : GraphicElement
    {
        /// <summary>
        /// Bounds of the source image in 'SourceTexture'.
        /// </summary>
        private Rectangle mSourceRectangle;

        /// <summary>
        /// Source texture, containing the source image.
        /// </summary>
        private Texture2D mSourceTexture;

        /// <summary>
        /// Gets or sets the bounds of the source image in 'SourceTexture'.
        /// </summary>
        public Rectangle SourceRectangle
        {
            get { return mSourceRectangle; }
            set { mSourceRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the source texture, containing the source image.
        /// </summary>
        public Texture2D SourceTexture
        {
            get { return mSourceTexture; }
            set { mSourceTexture = value; }
        }

        public Color Color { get; set; }

        /// <summary>
        /// Constructs a new Image instance.
        /// </summary>
        /// <param name="sourceRectangle">Bounds of the source image in 'SourceTexture'.</param>
        /// <param name="sourceTexture">Source texture, containing the source image.</param>
        public Image(Rectangle sourceRectangle, Texture2D sourceTexture)
        {
            SourceRectangle = sourceRectangle;
            SourceTexture = sourceTexture;

            Color = Color.White;
        }

        #region GraphicElement

        /// <summary>
        /// Gets the size of this graphic element.
        /// </summary>
        public override System.Drawing.SizeF Size
        {
            get { return new System.Drawing.Size(SourceRectangle.Width, SourceRectangle.Height); }
        }

        /// <summary>
        /// Draws this graphic element.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw.</param>
        /// <param name="screenLocation">Destination location, where to draw.</param>
        /// <param name="gameTime">Current game time.</param>
        public override void Draw(SpriteBatch spriteBatch, Point screenLocation, AleGameTime gameTime)
        {
            spriteBatch.Draw(SourceTexture, new Vector2((float)screenLocation.X, (float)screenLocation.Y), SourceRectangle, Color);
        }

        #endregion GraphicElement
    }
}
