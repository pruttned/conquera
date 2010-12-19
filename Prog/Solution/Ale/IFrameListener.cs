namespace Ale
{
    /// <summary>
    /// Frame listener is called on the start of each frame
    /// </summary>
    public interface IFrameListener
    {
        /// <summary>
        /// Called before updating a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void BeforeUpdate(AleGameTime gameTime);

        /// <summary>
        /// Called after updating a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void AfterUpdate(AleGameTime gameTime);

        /// <summary>
        /// Called before rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void BeforeRender(AleGameTime gameTime);

        /// <summary>
        /// Called after rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void AfterRender(AleGameTime gameTime);
    }
}
