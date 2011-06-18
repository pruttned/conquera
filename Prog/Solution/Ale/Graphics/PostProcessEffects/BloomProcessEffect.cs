//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Bloom post-process effect.
    /// Requieres "PostProcessEffects/ExtractBrightness.fx",
    ///           "PostProcessEffects/BlurHoriz.fx",
    ///           "PostProcessEffects/BlurVert.fx",
    ///           "PostProcessEffects/BloomCombine.fx"
    /// </summary>
    public class BloomProcessEffect : PostProcessEffect
    {
        #region Fields

        //#region ExtractBrightness
        
        ///// <summary>
        ///// 
        ///// </summary>
        //private RenderTarget2D mExtractBrightnessEffectTarget;

        ///// <summary>
        ///// 
        ///// </summary>
        //private MaterialEffect mExtractBrightnessEffect;

        ///// <summary>
        ///// 
        ///// </summary>
        //private Texture2DMaterialEffectParam mExtractBrightnessEffectScreenMap;
        
        //#endregion ExtractBrightness

        #region BlurHoriz
        
        /// <summary>
        /// 
        /// </summary>
        private AleRenderTarget mBlurHorizEffectTarget;
        
        /// <summary>
        /// 
        /// </summary>
        private MaterialEffect mBlurHorizEffect;
        
        /// <summary>
        /// 
        /// </summary>
        private Texture2DMaterialEffectParam mBlurHorizEffectScreenMap;

        #endregion BlurHoriz
        
        #region BlurVert

        /// <summary>
        /// 
        /// </summary>
        private AleRenderTarget mBlurVertEffectTarget;

        /// <summary>
        /// 
        /// </summary>
        private MaterialEffect mBlurVertEffect;

        /// <summary>
        /// 
        /// </summary>
        private Texture2DMaterialEffectParam mBlurVertEffectScreenMap;
        
        #endregion BlurVert
        
        #region Bloom

        /// <summary>
        /// 
        /// </summary>
        private MaterialEffect mBloomEffect;
        
        /// <summary>
        /// 
        /// </summary>
        private Texture2DMaterialEffectParam mBloomEffectScreenMap;
        
        /// <summary>
        /// 
        /// </summary>
        private Texture2DMaterialEffectParam mBloomEffectBluredBrightnessMap;
        
        #endregion Bloom

        #endregion Fields

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDeviceManager"></param>
        /// <param name="content"></param>
        public BloomProcessEffect(GraphicsDeviceManager graphicsDeviceManager, IRenderTargetManager renderTargetManager, ContentManager content)
            : base(graphicsDeviceManager, renderTargetManager)
        {
            //mExtractBrightnessEffect = content.Load<MaterialEffect>(@"PostProcessEffects/ExtractBrightness");
            //mExtractBrightnessEffectScreenMap = (Texture2DMaterialEffectParam)mExtractBrightnessEffect.ManualParameters["gScreenMap"];

            mBlurHorizEffect = content.Load<MaterialEffect>(@"PostProcessEffects/BlurHoriz");
            mBlurHorizEffectScreenMap = (Texture2DMaterialEffectParam)mBlurHorizEffect.ManualParameters["gScreenMap"];

            mBlurVertEffect = content.Load<MaterialEffect>(@"PostProcessEffects/BlurVert");
            mBlurVertEffectScreenMap = (Texture2DMaterialEffectParam)mBlurVertEffect.ManualParameters["gScreenMap"];

            mBloomEffect = content.Load<MaterialEffect>(@"PostProcessEffects/BloomCombine");
            mBloomEffectScreenMap = (Texture2DMaterialEffectParam)mBloomEffect.ManualParameters["gScreenMap"];
            mBloomEffectBluredBrightnessMap = (Texture2DMaterialEffectParam)mBloomEffect.ManualParameters["gBluredBrightnessMap"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="resultRenderTarget"></param>
        /// <param name="gameTime"></param>
        protected override void ApplyImpl(Texture2D screen, RenderTarget2D resultRenderTarget, AleGameTime gameTime)
        {
            //Extract brightness pass
            //GraphicsDevice.SetRenderTarget(0, mExtractBrightnessEffectTarget);
            //mExtractBrightnessEffectScreenMap.Value = screen;
            //mExtractBrightnessEffect.Apply(gameTime, mExtractBrightnessEffect.DefaultTechnique.Passes[0]);
            //DrawFullscreenQuad();

            //Horizontal blur pass
            mBlurHorizEffectScreenMap.Value = screen;
            mBlurHorizEffectTarget.Begin();
            //            mBlurHorizEffectScreenMap.Value = mExtractBrightnessEffectTarget.GetTexture();
            //mBlurHorizEffect.Apply(gameTime, mBlurHorizEffect.DefaultTechnique.Passes[0]);
            DrawFullscreenQuad(mBlurHorizEffect, gameTime, mBlurHorizEffectTarget.Width, mBlurHorizEffectTarget.Height);
            mBlurHorizEffectTarget.End(false);

            //Vertical blur pass
            mBlurVertEffectTarget.Begin();
            mBlurVertEffectScreenMap.Value = mBlurHorizEffectTarget.Texture;
            //mBlurVertEffect.Apply(gameTime, mBlurVertEffect.DefaultTechnique.Passes[0]);
            DrawFullscreenQuad(mBlurVertEffect, gameTime, mBlurVertEffectTarget.Width, mBlurVertEffectTarget.Height);
            mBlurVertEffectTarget.End();

            //Bloom pass
            mBloomEffectScreenMap.Value = screen;
            mBloomEffectBluredBrightnessMap.Value = mBlurVertEffectTarget.Texture;
            //mBloomEffect.Apply(gameTime, mBloomEffect.DefaultTechnique.Passes[0]);
            DrawFullscreenQuad(mBloomEffect, gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContentImpl()
        {
            PresentationParameters pp = GraphicsDevice.PresentationParameters;

         //   mExtractBrightnessEffectTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth / 4, pp.BackBufferHeight / 4, 1, pp.BackBufferFormat);
            //mBlurHorizEffectTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth / 4, pp.BackBufferHeight / 4, 1, pp.BackBufferFormat);
            //mBlurVertEffectTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth / 4, pp.BackBufferHeight / 4, 1, pp.BackBufferFormat);
            mBlurHorizEffectTarget = new AleRenderTarget(GraphicsDeviceManager, "BlurHoriz", pp.BackBufferWidth / 4, pp.BackBufferHeight / 4, 1, pp.BackBufferFormat);
            mBlurVertEffectTarget = new AleRenderTarget(GraphicsDeviceManager, "BlurVert", pp.BackBufferWidth / 4, pp.BackBufferHeight / 4, 1, pp.BackBufferFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UnloadContentImpl()
        {
            //mExtractBrightnessEffectTarget.Dispose();
            if (null != mBlurHorizEffectTarget)
            {
                mBlurHorizEffectTarget.Dispose();
            }

            if (null != mBlurVertEffectTarget)
            {
                mBlurVertEffectTarget.Dispose();
            }
        }

        #endregion Methods
    }
}
