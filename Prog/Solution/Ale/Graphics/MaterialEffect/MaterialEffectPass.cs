using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Pass of the effect's technique
    /// </summary>
    public class MaterialEffectPass
    {
        #region Fields

        /// <summary>
        /// Microsoft.Xna.Framework.Graphics.EffectPass to which is this MaterialEffectPass binded
        /// </summary>
        private EffectPass mEffectPass;

        /// <summary>
        /// Whether this pass enables the alpha blending
        /// </summary>
        private bool mIsTransparent;

        /// <summary>
        /// Texture parameter that should be used for batching
        /// </summary>
        private MaterialEffectParam mMainTextureParameter;

        private MaterialEffectTechnique mParentTechnique;

        /// <summary>
        /// Order of this pass in the parent technique
        /// </summary>
        private byte mOrderInTechnique;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the Microsoft.Xna.Framework.Graphics.EffectPass to which is this MaterialEffectPass binded
        /// </summary>
        internal EffectPass EffectPass
        {
            get { return mEffectPass; }
        }

        /// <summary>
        /// Gets the name of this pass
        /// </summary>
        public string Name
        {
            get { return mEffectPass.Name; }
        }

        public MaterialEffectTechnique ParentTechnique
        {
            get { return mParentTechnique; }
        }

        /// <summary>
        /// Gets the annotations of this pass
        /// </summary>
        public EffectAnnotationCollection Annotations
        {
            get { return mEffectPass.Annotations; }
        }

        /// <summary>
        /// Gets whether this pass enables the alpha blending
        /// </summary>
        public bool IsTransparent
        {
            get { return mIsTransparent; }
        }

        /// <summary>
        /// Gets the texture parameter that should be used for batching
        /// </summary>
        public MaterialEffectParam MainTextureParameter
        {
            get { return mMainTextureParameter; }
        }

        /// <summary>
        /// Gets the order of this pass in the parent technique
        /// </summary>
        public byte OrderInTechnique
        {
            get { return mOrderInTechnique; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Internal ctor
        /// </summary>
        /// <param name="effectPass">- Microsoft.Xna.Framework.Graphics.EffectPass to which should be this MaterialEffectPass binded</param>
        /// <param name="orderInTechnique">- Order of this pass in the parent technique</param>
        /// <exception cref="ArgumentException">- Incorrect annotation type</exception>
        internal MaterialEffectPass(MaterialEffectTechnique parentTechnique, EffectPass effectPass, byte orderInTechnique)
        {
            mEffectPass = effectPass;
            mOrderInTechnique = orderInTechnique;
            mParentTechnique = parentTechnique;

            //IsTransparent
            LoadIsTransparentAnnotation();

            //MainTexture
            LoadMainTextureAnnotation();
        }

        /// <summary>
        /// Applies the effect's pass. 
        /// Duplicate state changes are discarded and so multiple following calls of the Apply (with same parameters) method 
        /// of a same pass instance has no performance penalty
        /// </summary>
        /// <seealso cref="MaterialEffect.Finish"/>
        /// <param name="gameTime">- Actual game time</param>
        /// <param name="camera">- Actual camera</param>
        /// <param name="renderableUnit">- Actual renderable unit that is going to be rendered</param>
        public void Apply(AleGameTime gameTime, ICamera camera, IRenderableUnit renderableUnit)
        {
            mParentTechnique.ParentMaterialEffect.Apply(gameTime, camera, renderableUnit, null, null, this);
        }

        /// <summary>
        /// Applies the effect's pass. 
        /// Duplicate state changes are discarded and so multiple following calls of the Apply (with same parameters) method 
        /// of a same pass instance has no performance penalty
        /// </summary>
        /// <seealso cref="MaterialEffect.Finish"/>
        /// <param name="gameTime">- Actual game time</param>
        /// <param name="camera">- Actual camera</param>
        /// <param name="renderableUnit">- Actual renderable unit that is going to be rendered</param>
        /// <param name="renderTargetManager">- Render target manager (nullable)</param>
        public void Apply(AleGameTime gameTime, ICamera camera, IRenderableUnit renderableUnit, RenderTargetManager renderTargetManager)
        {
            mParentTechnique.ParentMaterialEffect.Apply(gameTime, camera, renderableUnit, null, renderTargetManager, this);
        }

        /// <summary>
        /// Applies the effect's pass. 
        /// Duplicate state changes are discarded and so multiple following calls of the Apply (with same parameters) method 
        /// of a same pass instance has no performance penalty
        /// </summary>
        /// <seealso cref="MaterialEffect.Finish"/>
        /// <param name="gameTime">- Actual game time</param>
        /// <param name="camera">- Actual camera</param>
        /// <param name="renderableUnit">- Actual renderable unit that is going to be rendered</param>
        /// <param name="scene">- nullable</param>
        /// <param name="renderTargetManager">- Render target manager (nullable)</param>
        public void Apply(AleGameTime gameTime, ICamera camera, IRenderableUnit renderableUnit, Scene.BaseScene scene, RenderTargetManager renderTargetManager)
        {
            mParentTechnique.ParentMaterialEffect.Apply(gameTime, camera, renderableUnit, scene, renderTargetManager, this);
        }

        #region MaterialEffectPass ctor

        /// <summary>
        /// Loads the IsTransparent annotation
        /// </summary>
        private void LoadIsTransparentAnnotation()
        {
            EffectAnnotation isTransparentAnnotation = Annotations["IsTransparent"];
            if (null != isTransparentAnnotation)
            {
                //check type
                if (EffectParameterType.Bool != isTransparentAnnotation.ParameterType)
                {
                    throw new ArgumentException(string.Format("Annotation IsTransparent of the pass '{0}' has incorect type. Its type must be bool", Name));
                }

                mIsTransparent = isTransparentAnnotation.GetValueBoolean();
            }
            else //use default value
            {
                mIsTransparent = false;
            }
        }

        /// <summary>
        /// Loads the MainTexture annotation
        /// </summary>
        private void LoadMainTextureAnnotation()
        {
            EffectAnnotation mainTextureAnnotation = Annotations["MainTexture"];
            if (null != mainTextureAnnotation)
            {
                //check type
                if (EffectParameterType.String != mainTextureAnnotation.ParameterType)
                {
                    throw new ArgumentException(string.Format("Annotation MainTexture of the pass '{0}' has incorect type. Its type must be string", Name));
                }

                string mainTextureAnnotationValue = mainTextureAnnotation.GetValueString();

                if (string.IsNullOrEmpty(mainTextureAnnotationValue)) //no texture param
                {
                    mMainTextureParameter = null;
                }
                else
                {//get texture param according to a specified name
                    mMainTextureParameter = mParentTechnique.ParentMaterialEffect.ManualParameters[mainTextureAnnotationValue];

                    if (null == mMainTextureParameter)
                    {
                        throw new ArgumentException(string.Format("Texture parameter that is specified in the MainTexture annotation of the pass '{0}' doesn't exists", Name));
                    }
                }
            }
            else //use default value
            {
                mMainTextureParameter = null;
            }
        }

        #endregion MaterialEffectPass ctor

        #endregion Methods

    }
}
