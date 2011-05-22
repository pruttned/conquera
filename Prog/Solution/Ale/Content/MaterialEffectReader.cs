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
