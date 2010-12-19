using Microsoft.Xna.Framework.Graphics;

namespace Ale.Content
{
    /// <summary>
    /// Compiled material effect. Just a wrapper arround CompiledEffect because it is not possible to have
    /// two ContentTypeWriters that use a same content type
    /// </summary>
    public struct CompiledMaterialEffect
    {        
        #region Fields
        
        /// <summary>
        /// Internal CompiledEffect
        /// </summary>
        private CompiledEffect mCompiledEffect;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the internal CompiledEffect
        /// </summary>
        public CompiledEffect CompiledEffect
        {
            get { return mCompiledEffect; }
        }

        #endregion Properties

        #region Methods
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="compiledEffect">- Internal CompiledEffect</param>
        public CompiledMaterialEffect(CompiledEffect compiledEffect)
        {
            mCompiledEffect = compiledEffect;
        }

        #endregion Methods
    }
}
