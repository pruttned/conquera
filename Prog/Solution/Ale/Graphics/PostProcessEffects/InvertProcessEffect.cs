using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Color inverting post-process effect.
    /// Requieres "PostProcessEffects/Invert.fx"
    /// </summary>
    public class InvertProcessEffect : PostProcessEffect
    {
        #region Fields
        
        /// <summary>
        /// 
        /// </summary>
        private MaterialEffect mInvertEffect;
        
        /// <summary>
        /// 
        /// </summary>
        private Texture2DMaterialEffectParam mScreenMapMaterialEffectParam;
        
        #endregion Fields

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicsDeviceManager"></param>
        /// <param name="content"></param>
        public InvertProcessEffect(GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
            : base(graphicsDeviceManager)
        {
            mInvertEffect = content.Load<MaterialEffect>(@"PostProcessEffects/Invert");
            mScreenMapMaterialEffectParam = (Texture2DMaterialEffectParam)mInvertEffect.ManualParameters["gScreenMap"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="resultRenderTarget"></param>
        /// <param name="gameTime"></param>
        protected override void ApplyImpl(Texture2D screen, RenderTarget2D resultRenderTarget, AleGameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(0, resultRenderTarget);
            mScreenMapMaterialEffectParam.Value = screen;
            var viewport = GraphicsDevice.Viewport;
            //mInvertEffect.Apply(gameTime, mInvertEffect.DefaultTechnique.Passes[0]);
            DrawFullscreenQuad(mInvertEffect, gameTime, screen, viewport.Width, viewport.Height);
        }

        /// <summary>
        /// Nothing
        /// </summary>
        protected override void LoadContentImpl()
        {
        }
        
        /// <summary>
        /// Nothing
        /// </summary>
        protected override void UnloadContentImpl()
        {
        }

        #endregion Methods
    }
}
