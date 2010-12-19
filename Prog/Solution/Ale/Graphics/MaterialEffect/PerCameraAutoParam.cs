using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Auto parameter that is updated ones per camera
    /// </summary>
    internal class PerCameraAutoParam
    {
        #region Enums

        /// <summary>
        /// Defines parameter's semantic
        /// </summary>
        enum Semantic
        {
            /// <summary>
            /// View transformation metrix
            /// </summary>
            View,

            /// <summary>
            /// Projection transformation metrix
            /// </summary>
            Projection,
            
            /// <summary>
            /// View*Projection transformation metrix
            /// </summary>
            ViewProjection,

            /// <summary>
            /// Untransformed position of the camera's eye
            /// </summary>
            EyePosition
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
        /// Creates a new auto parameter or returns null if its semantic is incompatible with per camera parameter semantics
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <returns>New auto parameter or null if its semantic is incompatible with per camera parameter semantics</returns>
        static public PerCameraAutoParam TryCreateAutoParam(EffectParameter effectParameter)
        {
            for(int i = 0; i <  SemanticNames.Length; ++i)
            { //Scaning through semantic names is better then using Enum.parse because it is possible that 
                //the semantic will be not found in Semantic enum and Enum.Parse will throws exception in that case 
                //which is slow. If parameter's semantic is not found among Semantic enumeration members, then it just
                //meens that is is not a PerCameraAutoParam but another parameter type.
                if(string.Equals(SemanticNames[i], effectParameter.Semantic, StringComparison.OrdinalIgnoreCase))
                {
                    return new PerCameraAutoParam(effectParameter, (Semantic)i);
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
        /// <param name="camera">- Actual camera (null = reset parameter)</param>
        public void Update(ICamera camera)
        {
            switch (mSemantic)
            {
                case Semantic.View:
                    mEffectParameter.SetValue((null != camera) ? camera.ViewTransformation : Matrix.Identity);
                    break;
                case Semantic.Projection:
                    mEffectParameter.SetValue((null != camera) ? camera.ProjectionTransformation : Matrix.Identity);
                    break;
                case Semantic.ViewProjection:
                    mEffectParameter.SetValue((null != camera) ? camera.ViewProjectionTransformation: Matrix.Identity);
                    break;
                case Semantic.EyePosition:
                    mEffectParameter.SetValue((null != camera) ? camera.WorldPosition : Vector3.Zero);
                    break;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <param name="semantic">- Parameter's semantic</param>
        private PerCameraAutoParam(EffectParameter effectParameter, Semantic semantic)
        {
            mEffectParameter = effectParameter;
            mSemantic = semantic;
        }

        #endregion Methods
    }
}
