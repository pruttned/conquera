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
            ElapsedTime
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

        #endregion Fields

        #region Methods

        /// <summary>
        /// Creates a new auto parameter or returns null if its semantic is incompatible with per frame parameter semantics
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <returns>New auto parameter or null if its semantic is incompatible with per frame parameter semantics</returns>
        static public PerFrameAutoParam TryCreateAutoParam(EffectParameter effectParameter)
        {
            for(int i = 0; i <  SemanticNames.Length; ++i)
            { //Scaning through semantic names is better then using Enum.parse because it is possible that 
                //the semantic will be not found in Semantic enum and Enum.Parse will throws exception in that case 
                //which is slow. If parameter's semantic is not found among Semantic enumeration members, then it just
                //meens that is is not a PerFrameAutoParam but another parameter type.
                if(string.Equals(SemanticNames[i], effectParameter.Semantic, StringComparison.OrdinalIgnoreCase))
                {
                    return new PerFrameAutoParam(effectParameter, (Semantic)i);
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
            }
        }

        #endregion Methods
    
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <param name="semantic">- Parameter's semantic</param>
        private PerFrameAutoParam(EffectParameter effectParameter, Semantic semantic)
        {
            mEffectParameter = effectParameter;
            mSemantic = semantic;
        }
    }
}
