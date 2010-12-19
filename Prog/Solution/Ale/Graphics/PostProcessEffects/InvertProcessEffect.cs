//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
