using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Auto parameter that is updated ones per renderable
    /// </summary>
    internal class PerRenderableAutoParam
    {
        #region Enums

        /// <summary>
        /// Defines parameter's semantic
        /// </summary>
        enum Semantic
        {
            /// <summary>
            /// World transformation metrix
            /// </summary>
            World,

            /// <summary>
            /// World*View*Projection transformation matrix
            /// </summary>
            WorldViewProjection,
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
        /// <returns>New auto parameter or null if its semantic is incompatible with per renderable parameter semantics</returns>
        static public PerRenderableAutoParam TryCreateAutoParam(EffectParameter effectParameter)
        {
            for(int i = 0; i <  SemanticNames.Length; ++i)
            { //Scaning through semantic names is better then using Enum.parse because it is possible that 
                //the semantic will be not found in Semantic enum and Enum.Parse will throws exception in that case 
                //which is slow. If parameter's semantic is not found among Semantic enumeration members, then it just
                //meens that is is not a PerFrameAutoParam but another parameter type.
                if(string.Equals(SemanticNames[i], effectParameter.Semantic, StringComparison.OrdinalIgnoreCase))
                {
                    return new PerRenderableAutoParam(effectParameter, (Semantic)i);
                }
            }
            return null;
        }

        internal bool IsUsedInTechnique(EffectTechnique technique)
        {
            return technique.IsParameterUsed(mEffectParameter);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <param name="semantic">- Parameter's semantic</param>
        private PerRenderableAutoParam(EffectParameter effectParameter, Semantic semantic)
        {
            mEffectParameter = effectParameter;
            mSemantic = semantic;
        }

        /// <summary>
        /// Updates the binded parameter's value
        /// </summary>
        /// <param name="renderable">- Renderable (null = reset parameter)</param>
        /// <param name="camera">- Actual camera</param>
        public void Update(Renderable renderable, ICamera camera)
        {
            switch (mSemantic)
            {
                case Semantic.World:
                    mEffectParameter.SetValue((null != renderable) ? renderable.WorldTransformation : Matrix.Identity);
                      break;
                case Semantic.WorldViewProjection:
                      if ((null != renderable))
                      {
                          mEffectParameter.SetValue(Matrix.Multiply(renderable.WorldTransformation, camera.ViewProjectionTransformation));
                      }
                      else
                      {
                          mEffectParameter.SetValue(Matrix.Identity);
                      }
                      break;
            }
        }

        #endregion Methods
    }
}
