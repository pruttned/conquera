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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// Auto parameter that is updated ones per frame
    /// </summary>
    internal class PerFrameAutoParam
    {
        #region Enums

        /// <summary>
        /// Defines parameter's semantic
        /// </summary>
        enum Semantic
        {
            /// <summary>
            /// Time since the game start in seconds (Semantic 'Time')
            /// </summary>
            Time,

            /// <summary>
            /// Time since the previous frame in seconds (Semantic 'ElapsedTime')
            /// </summary>
            ElapsedTime,

            ScreenWidth,
            ScreenHeight,
            ScreenHalfPixel
        }

        #endregion Enums

        #region Fields

        /// <summary>
        /// Semantic names
        /// </summary>
        private static readonly string[] SemanticNames = Enum.GetNames(typeof(Semantic));

        /// <summary>
        /// Effect parameter to which is this auto parameter binded
        /// </summary>
        private EffectParameter mEffectParameter;

        /// <summary>
        /// Parameter's semantic
        /// </summary>
        private Semantic mSemantic;

        private GraphicsDevice mGraphicsDevice;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Creates a new auto parameter or returns null if its semantic is incompatible with per frame parameter semantics
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <returns>New auto parameter or null if its semantic is incompatible with per frame parameter semantics</returns>
        static public PerFrameAutoParam TryCreateAutoParam(EffectParameter effectParameter, GraphicsDevice graphicsDevice)
        {
            for(int i = 0; i <  SemanticNames.Length; ++i)
            { //Scaning through semantic names is better then using Enum.parse because it is possible that 
                //the semantic will be not found in Semantic enum and Enum.Parse will throws exception in that case 
                //which is slow. If parameter's semantic is not found among Semantic enumeration members, then it just
                //meens that is is not a PerFrameAutoParam but another parameter type.
                if(string.Equals(SemanticNames[i], effectParameter.Semantic, StringComparison.OrdinalIgnoreCase))
                {
                    return new PerFrameAutoParam(effectParameter, (Semantic)i, graphicsDevice);
                }
            }
            return null;
        }

        internal bool IsUsedInTechnique(EffectTechnique technique)
        {
            return technique.IsParameterUsed(mEffectParameter);
        }

        /// <summary>
        /// Updates the binded parameter's value
        /// </summary>
        /// <param name="gameTime">- Actual game time (null = reset parameter)</param>
        public void Update(AleGameTime gameTime)
        {
            switch (mSemantic)
            {
                case Semantic.Time:
                    mEffectParameter.SetValue((null != gameTime) ? gameTime.TotalTime : 0);
                      break;
                case Semantic.ElapsedTime:
                      mEffectParameter.SetValue((null != gameTime) ? gameTime.TimeSinceLastFrame : 0);
                      break;
                case Semantic.ScreenWidth:
                      mEffectParameter.SetValue(mGraphicsDevice.PresentationParameters.BackBufferWidth);
                      break;
                case Semantic.ScreenHeight:
                      mEffectParameter.SetValue(mGraphicsDevice.PresentationParameters.BackBufferHeight);
                      break;
                case Semantic.ScreenHalfPixel:
                    var pp = mGraphicsDevice.PresentationParameters;
                      mEffectParameter.SetValue(new Vector2(
                          0.5f / (float)pp.BackBufferWidth,
                          0.5f / (float)pp.BackBufferHeight));
                      break;
            }
        }

        #endregion Methods
    
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <param name="semantic">- Parameter's semantic</param>
        private PerFrameAutoParam(EffectParameter effectParameter, Semantic semantic, GraphicsDevice graphicsDevice)
        {
            mEffectParameter = effectParameter;
            mSemantic = semantic;
            mGraphicsDevice = graphicsDevice;
        }
    }
}
