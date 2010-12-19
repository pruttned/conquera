using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Specifies interface for all objects that can be rendered by a single 
    /// render operation and are parts of Renderable
    /// </summary>
    public interface IRenderableUnit
    {
        #region Properties

        /// <summary>
        /// Gets the parent renderable
        /// </summary>
        Renderable ParentRenderable
        {
            get;
        }

        /// <summary>
        /// Gets the material that should be used during rendering process
        /// </summary>
        Material Material
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Renders the renderable primitive
        /// </summary>
        /// <remarks>
        /// All effect parameters will be setted prior to calling this method
        /// </remarks>
        void Render(AleGameTime gameTime);

        /// <summary>
        /// Updates manual per renderable unit effect parameters.
        /// </summary>
        /// <remarks>
        /// Here you can update effect parameters (MaterialEffectParam) just before is renderable unit rendered.
        /// Note that you dont't need to update parameters that are present in Material or are handled by PerCameraAutoParam, PerFrameAutoParam, or PerRenderableAutoParam.
        /// </remarks>
        void UpdateMaterialEffectParameters();

        #endregion Methods
    }
}


