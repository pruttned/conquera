using Microsoft.Xna.Framework.Content;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ale.Content
{
    /// <summary>
    /// ContentTypeReader for MaterialEffect
    /// </summary>
    public class MaterialEffectReader : ContentTypeReader<MaterialEffect>
    {
        #region Fields

        /// <summary>
        /// Shared effect pool
        /// </summary>
        private static EffectPool mSharedEffectPool;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the shared effect pool
        /// </summary>
        private static EffectPool SharedEffectPool
        {
            get
            {
                if (null == mSharedEffectPool)
                {
                    mSharedEffectPool = new EffectPool();
                }
                return mSharedEffectPool;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// see ContentTypeReader
        /// </summary>
        /// <param name="input"></param>
        /// <param name="existingInstance"></param>
        /// <returns></returns>
        protected override MaterialEffect Read(ContentReader input, MaterialEffect existingInstance)
        {
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(
                      typeof(IGraphicsDeviceService))).GraphicsDevice;
            
            int effectCodeSize = input.ReadInt32();
            try
            {
                return new MaterialEffect(new Effect(graphicsDevice, input.ReadBytes(effectCodeSize), CompilerOptions.None, SharedEffectPool));
            }
            catch (Exception ex)
            {
                throw new ContentLoadException(string.Format("Error occured during load of the '{0}' material effect", input.AssetName), ex);
            }
        }
        #endregion Methods
    }
}
